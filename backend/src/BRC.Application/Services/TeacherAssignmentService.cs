using BRC.Application.DTOs.Common;
using BRC.Application.DTOs.TeacherAssignments;
using BRC.Application.Services.Interfaces;
using BRC.Domain.Entities;
using BRC.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace BRC.Application.Services;

public class TeacherAssignmentService : ITeacherAssignmentService
{
    private readonly BrcDbContext _context;
    private readonly ILogger<TeacherAssignmentService> _logger;

    public TeacherAssignmentService(BrcDbContext context, ILogger<TeacherAssignmentService> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<PaginatedResponse<TeacherAssignmentDto>> GetTeacherAssignmentsAsync(
        PaginationParams pagination, Guid? teacherId = null, Guid? classId = null, Guid? subjectId = null)
    {
        var query = _context.TeacherSubjectClasses
            .Include(t => t.Teacher).ThenInclude(t => t.User)
            .Include(t => t.Subject)
            .Include(t => t.Class)
            .AsQueryable();

        if (teacherId.HasValue)
            query = query.Where(t => t.TeacherId == teacherId.Value);
        if (classId.HasValue)
            query = query.Where(t => t.ClassId == classId.Value);
        if (subjectId.HasValue)
            query = query.Where(t => t.SubjectId == subjectId.Value);

        var totalItems = await query.CountAsync();
        var items = await query
            .OrderByDescending(t => t.AssignedAt)
            .Skip((pagination.Page - 1) * pagination.PageSize)
            .Take(pagination.PageSize)
            .ToListAsync();

        return new PaginatedResponse<TeacherAssignmentDto>
        {
            Items = items.Select(MapToDto).ToList(),
            Page = pagination.Page,
            PageSize = pagination.PageSize,
            TotalItems = totalItems
        };
    }

    public async Task<TeacherAssignmentDto> CreateAsync(CreateTeacherAssignmentDto dto)
    {
        // Validate teacher exists
        var teacher = await _context.Teachers.Include(t => t.User).FirstOrDefaultAsync(t => t.Id == dto.TeacherId);
        if (teacher == null)
            throw new InvalidOperationException("Teacher not found.");

        // Validate subject exists
        if (!await _context.Subjects.AnyAsync(s => s.Id == dto.SubjectId))
            throw new InvalidOperationException("Subject not found.");

        // Validate class exists
        if (!await _context.Classes.AnyAsync(c => c.Id == dto.ClassId))
            throw new InvalidOperationException("Class not found.");

        // Check for duplicate
        if (await _context.TeacherSubjectClasses.AnyAsync(t =>
            t.TeacherId == dto.TeacherId && t.SubjectId == dto.SubjectId && t.ClassId == dto.ClassId))
            throw new InvalidOperationException("This teacher is already assigned to this subject-class combination.");

        var tsc = new TeacherSubjectClass
        {
            Id = Guid.NewGuid(),
            TeacherId = dto.TeacherId,
            SubjectId = dto.SubjectId,
            ClassId = dto.ClassId,
            AssignedAt = DateTime.UtcNow
        };

        _context.TeacherSubjectClasses.Add(tsc);
        await _context.SaveChangesAsync();

        _logger.LogInformation("Assigned teacher {TeacherId} to subject {SubjectId} in class {ClassId}",
            dto.TeacherId, dto.SubjectId, dto.ClassId);

        // Reload with navigation properties
        var result = await _context.TeacherSubjectClasses
            .Include(t => t.Teacher).ThenInclude(t => t.User)
            .Include(t => t.Subject)
            .Include(t => t.Class)
            .FirstAsync(t => t.Id == tsc.Id);

        return MapToDto(result);
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        var tsc = await _context.TeacherSubjectClasses.FindAsync(id);
        if (tsc == null) return false;

        _context.TeacherSubjectClasses.Remove(tsc);
        await _context.SaveChangesAsync();

        _logger.LogInformation("Removed teacher assignment {Id}", id);
        return true;
    }

    private static TeacherAssignmentDto MapToDto(TeacherSubjectClass tsc)
    {
        return new TeacherAssignmentDto
        {
            Id = tsc.Id,
            TeacherId = tsc.TeacherId,
            TeacherName = $"{tsc.Teacher?.User?.FirstName} {tsc.Teacher?.User?.LastName}",
            EmployeeId = tsc.Teacher?.EmployeeId ?? "",
            SubjectId = tsc.SubjectId,
            SubjectName = tsc.Subject?.Name ?? "",
            SubjectCode = tsc.Subject?.Code ?? "",
            ClassId = tsc.ClassId,
            ClassName = tsc.Class?.Name ?? "",
            AssignedAt = tsc.AssignedAt
        };
    }
}
