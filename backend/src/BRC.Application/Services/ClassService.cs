using BRC.Application.DTOs.Classes;
using BRC.Application.DTOs.Common;
using BRC.Application.DTOs.Subjects;
using BRC.Application.Services.Interfaces;
using BRC.Domain.Entities;
using BRC.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace BRC.Application.Services;

public class ClassService : IClassService
{
    private readonly BrcDbContext _context;
    private readonly ILogger<ClassService> _logger;

    public ClassService(BrcDbContext context, ILogger<ClassService> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<PaginatedResponse<ClassDto>> GetClassesAsync(PaginationParams pagination, string? search = null, Guid? groupId = null)
    {
        var query = _context.Classes
            .Include(c => c.AcademicGroup)
            .Include(c => c.Students)
            .Include(c => c.TeacherSubjectClasses)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(search))
            query = query.Where(c => c.Name.ToLower().Contains(search.ToLower()));

        if (groupId.HasValue)
            query = query.Where(c => c.AcademicGroupId == groupId.Value);

        var totalItems = await query.CountAsync();
        var items = await query
            .OrderBy(c => c.Name)
            .Skip((pagination.Page - 1) * pagination.PageSize)
            .Take(pagination.PageSize)
            .ToListAsync();

        return new PaginatedResponse<ClassDto>
        {
            Items = items.Select(MapToDto).ToList(),
            Page = pagination.Page,
            PageSize = pagination.PageSize,
            TotalItems = totalItems
        };
    }

    public async Task<ClassDto?> GetClassByIdAsync(Guid id)
    {
        var cls = await _context.Classes
            .Include(c => c.AcademicGroup)
            .Include(c => c.Students)
            .Include(c => c.TeacherSubjectClasses)
            .FirstOrDefaultAsync(c => c.Id == id);

        return cls == null ? null : MapToDto(cls);
    }

    public async Task<ClassDto> CreateClassAsync(CreateClassDto dto)
    {
        if (await _context.Classes.AnyAsync(c => c.Name == dto.Name))
            throw new InvalidOperationException("A class with this name already exists.");

        if (!await _context.AcademicGroups.AnyAsync(g => g.Id == dto.AcademicGroupId))
            throw new InvalidOperationException("Academic group not found.");

        var cls = new Class
        {
            Id = Guid.NewGuid(),
            Name = dto.Name.Trim(),
            AcademicGroupId = dto.AcademicGroupId,
            AcademicYear = dto.AcademicYear,
            Section = dto.Section,
            CreatedAt = DateTime.UtcNow
        };

        _context.Classes.Add(cls);
        await _context.SaveChangesAsync();
        _logger.LogInformation("Created class {ClassName}", cls.Name);

        return (await GetClassByIdAsync(cls.Id))!;
    }

    public async Task<ClassDto?> UpdateClassAsync(Guid id, UpdateClassDto dto)
    {
        var cls = await _context.Classes.FindAsync(id);
        if (cls == null) return null;

        if (cls.Name != dto.Name && await _context.Classes.AnyAsync(c => c.Name == dto.Name && c.Id != id))
            throw new InvalidOperationException("A class with this name already exists.");

        cls.Name = dto.Name.Trim();
        cls.AcademicGroupId = dto.AcademicGroupId;
        cls.AcademicYear = dto.AcademicYear;
        cls.Section = dto.Section;
        cls.IsActive = dto.IsActive;

        await _context.SaveChangesAsync();
        _logger.LogInformation("Updated class {ClassName}", cls.Name);

        return await GetClassByIdAsync(id);
    }

    public async Task<bool> DeleteClassAsync(Guid id)
    {
        var cls = await _context.Classes.FindAsync(id);
        if (cls == null) return false;

        var hasStudents = await _context.Students.AnyAsync(s => s.ClassId == id);
        if (hasStudents)
            throw new InvalidOperationException("Cannot delete class with enrolled students.");

        var hasAssignments = await _context.Assignments.AnyAsync(a => a.ClassId == id);
        if (hasAssignments)
            throw new InvalidOperationException("Cannot delete class with existing assignments.");

        _context.Classes.Remove(cls);
        await _context.SaveChangesAsync();
        _logger.LogInformation("Deleted class {ClassName}", cls.Name);
        return true;
    }

    public async Task<List<AcademicGroupDto>> GetAcademicGroupsAsync()
    {
        var groups = await _context.AcademicGroups
            .Include(g => g.Classes)
            .Include(g => g.Students)
            .OrderBy(g => g.Name)
            .ToListAsync();

        return groups.Select(g => new AcademicGroupDto
        {
            Id = g.Id,
            Name = g.Name,
            Description = g.Description,
            IsActive = g.IsActive,
            ClassCount = g.Classes.Count,
            StudentCount = g.Students.Count
        }).ToList();
    }

    private static ClassDto MapToDto(Class cls)
    {
        return new ClassDto
        {
            Id = cls.Id,
            Name = cls.Name,
            AcademicGroupId = cls.AcademicGroupId,
            GroupName = cls.AcademicGroup?.Name ?? "",
            AcademicYear = cls.AcademicYear,
            Section = cls.Section,
            IsActive = cls.IsActive,
            StudentCount = cls.Students?.Count ?? 0,
            TeacherCount = cls.TeacherSubjectClasses?.Select(t => t.TeacherId).Distinct().Count() ?? 0,
            CreatedAt = cls.CreatedAt
        };
    }
}
