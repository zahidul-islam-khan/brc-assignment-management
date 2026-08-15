using BRC.Application.DTOs.Common;
using BRC.Application.DTOs.Subjects;
using BRC.Application.Services.Interfaces;
using BRC.Domain.Entities;
using BRC.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace BRC.Application.Services;

public class SubjectService : ISubjectService
{
    private readonly BrcDbContext _context;
    private readonly ILogger<SubjectService> _logger;

    public SubjectService(BrcDbContext context, ILogger<SubjectService> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<PaginatedResponse<SubjectDto>> GetSubjectsAsync(PaginationParams pagination, string? search = null, Guid? groupId = null)
    {
        var query = _context.Subjects
            .Include(s => s.SubjectAcademicGroups).ThenInclude(sag => sag.AcademicGroup)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(search))
            query = query.Where(s => s.Name.ToLower().Contains(search.ToLower()) || s.Code.ToLower().Contains(search.ToLower()));

        if (groupId.HasValue)
            query = query.Where(s => s.SubjectAcademicGroups.Any(sag => sag.AcademicGroupId == groupId.Value));

        var totalItems = await query.CountAsync();
        var items = await query
            .OrderBy(s => s.Name)
            .Skip((pagination.Page - 1) * pagination.PageSize)
            .Take(pagination.PageSize)
            .ToListAsync();

        return new PaginatedResponse<SubjectDto>
        {
            Items = items.Select(MapToDto).ToList(),
            Page = pagination.Page,
            PageSize = pagination.PageSize,
            TotalItems = totalItems
        };
    }

    public async Task<SubjectDto?> GetSubjectByIdAsync(Guid id)
    {
        var subject = await _context.Subjects
            .Include(s => s.SubjectAcademicGroups).ThenInclude(sag => sag.AcademicGroup)
            .FirstOrDefaultAsync(s => s.Id == id);

        return subject == null ? null : MapToDto(subject);
    }

    public async Task<SubjectDto> CreateSubjectAsync(CreateSubjectDto dto)
    {
        if (await _context.Subjects.AnyAsync(s => s.Code == dto.Code.ToUpper().Trim()))
            throw new InvalidOperationException("A subject with this code already exists.");

        var subject = new Subject
        {
            Id = Guid.NewGuid(),
            Code = dto.Code.ToUpper().Trim(),
            Name = dto.Name.Trim(),
            Credits = dto.Credits,
            CreatedAt = DateTime.UtcNow
        };

        _context.Subjects.Add(subject);

        foreach (var groupId in dto.AcademicGroupIds)
        {
            _context.SubjectAcademicGroups.Add(new SubjectAcademicGroup
            {
                Id = Guid.NewGuid(),
                SubjectId = subject.Id,
                AcademicGroupId = groupId
            });
        }

        await _context.SaveChangesAsync();
        _logger.LogInformation("Created subject {SubjectCode} - {SubjectName}", subject.Code, subject.Name);

        return (await GetSubjectByIdAsync(subject.Id))!;
    }

    public async Task<SubjectDto?> UpdateSubjectAsync(Guid id, UpdateSubjectDto dto)
    {
        var subject = await _context.Subjects
            .Include(s => s.SubjectAcademicGroups)
            .FirstOrDefaultAsync(s => s.Id == id);

        if (subject == null) return null;

        if (subject.Code != dto.Code.ToUpper().Trim() &&
            await _context.Subjects.AnyAsync(s => s.Code == dto.Code.ToUpper().Trim() && s.Id != id))
            throw new InvalidOperationException("A subject with this code already exists.");

        subject.Code = dto.Code.ToUpper().Trim();
        subject.Name = dto.Name.Trim();
        subject.Credits = dto.Credits;
        subject.IsActive = dto.IsActive;

        // Update academic group associations
        _context.SubjectAcademicGroups.RemoveRange(subject.SubjectAcademicGroups);
        foreach (var groupId in dto.AcademicGroupIds)
        {
            _context.SubjectAcademicGroups.Add(new SubjectAcademicGroup
            {
                Id = Guid.NewGuid(),
                SubjectId = subject.Id,
                AcademicGroupId = groupId
            });
        }

        await _context.SaveChangesAsync();
        _logger.LogInformation("Updated subject {SubjectCode}", subject.Code);

        return await GetSubjectByIdAsync(id);
    }

    public async Task<bool> DeleteSubjectAsync(Guid id)
    {
        var subject = await _context.Subjects.FindAsync(id);
        if (subject == null) return false;

        var hasAssignments = await _context.Assignments.AnyAsync(a => a.SubjectId == id);
        if (hasAssignments)
            throw new InvalidOperationException("Cannot delete subject with existing assignments.");

        _context.Subjects.Remove(subject);
        await _context.SaveChangesAsync();
        _logger.LogInformation("Deleted subject {SubjectCode}", subject.Code);
        return true;
    }

    private static SubjectDto MapToDto(Subject subject)
    {
        return new SubjectDto
        {
            Id = subject.Id,
            Code = subject.Code,
            Name = subject.Name,
            Credits = subject.Credits,
            IsActive = subject.IsActive,
            AcademicGroups = subject.SubjectAcademicGroups?.Select(sag => sag.AcademicGroup?.Name ?? "").ToList() ?? new(),
            CreatedAt = subject.CreatedAt
        };
    }
}
