using BRC.Application.DTOs.Assignments;
using BRC.Application.DTOs.Common;
using BRC.Application.Services.Interfaces;
using BRC.Domain.Entities;
using BRC.Domain.Enums;
using BRC.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace BRC.Application.Services;

public class AssignmentService : IAssignmentService
{
    private readonly BrcDbContext _context;
    private readonly ILogger<AssignmentService> _logger;

    public AssignmentService(BrcDbContext context, ILogger<AssignmentService> logger)
    {
        _context = context;
        _logger = logger;
    }

    // ─── Admin: view all assignments ────────────────────────────
    public async Task<PaginatedResponse<AssignmentDto>> GetAllAssignmentsAsync(PaginationParams pagination, AssignmentFilterParams filters)
    {
        var query = BuildAssignmentQuery(filters);
        return await PaginateAssignmentsAsync(query, pagination);
    }

    // ─── Teacher: view own assignments ────────────────────────────
    public async Task<PaginatedResponse<AssignmentDto>> GetTeacherAssignmentsAsync(Guid teacherId, PaginationParams pagination, AssignmentFilterParams filters)
    {
        var query = BuildAssignmentQuery(filters)
            .Where(a => a.TeacherId == teacherId);
        return await PaginateAssignmentsAsync(query, pagination);
    }

    public async Task<AssignmentDto?> GetAssignmentByIdAsync(Guid id)
    {
        var assignment = await GetAssignmentQueryable()
            .FirstOrDefaultAsync(a => a.Id == id);
        return assignment == null ? null : MapToDto(assignment);
    }

    public async Task<AssignmentDto> CreateAssignmentAsync(Guid teacherId, CreateAssignmentDto dto)
    {
        // Validate teacher is authorized for this subject-class combination
        var isAuthorized = await _context.TeacherSubjectClasses
            .AnyAsync(t => t.TeacherId == teacherId && t.SubjectId == dto.SubjectId && t.ClassId == dto.ClassId);

        if (!isAuthorized)
            throw new InvalidOperationException("You are not authorized to create assignments for this subject-class combination.");

        if (dto.MaximumMarks <= 0)
            throw new InvalidOperationException("Maximum marks must be a positive number.");

        if (dto.Deadline <= DateTime.UtcNow)
            throw new InvalidOperationException("Deadline must be in the future.");

        var assignment = new Assignment
        {
            Id = Guid.NewGuid(),
            Title = dto.Title.Trim(),
            Description = dto.Description,
            SubjectId = dto.SubjectId,
            ClassId = dto.ClassId,
            TeacherId = teacherId,
            Deadline = dto.Deadline,
            MaximumMarks = dto.MaximumMarks,
            Status = AssignmentStatus.Draft,
            CreatedAt = DateTime.UtcNow
        };

        _context.Assignments.Add(assignment);
        await _context.SaveChangesAsync();

        _logger.LogInformation("Teacher {TeacherId} created assignment {Title}", teacherId, assignment.Title);
        return (await GetAssignmentByIdAsync(assignment.Id))!;
    }

    public async Task<AssignmentDto?> UpdateAssignmentAsync(Guid assignmentId, Guid teacherId, UpdateAssignmentDto dto)
    {
        var assignment = await _context.Assignments.FindAsync(assignmentId);
        if (assignment == null) return null;

        if (assignment.TeacherId != teacherId)
            throw new InvalidOperationException("You are not authorized to update this assignment.");

        if (assignment.Status == AssignmentStatus.Closed)
            throw new InvalidOperationException("Cannot update a closed assignment.");

        if (dto.MaximumMarks <= 0)
            throw new InvalidOperationException("Maximum marks must be a positive number.");

        assignment.Title = dto.Title.Trim();
        assignment.Description = dto.Description;
        assignment.Deadline = dto.Deadline;
        assignment.MaximumMarks = dto.MaximumMarks;
        assignment.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();
        _logger.LogInformation("Teacher {TeacherId} updated assignment {AssignmentId}", teacherId, assignmentId);

        return await GetAssignmentByIdAsync(assignmentId);
    }

    public async Task<bool> DeleteAssignmentAsync(Guid assignmentId, Guid teacherId)
    {
        var assignment = await _context.Assignments.FindAsync(assignmentId);
        if (assignment == null) return false;

        if (assignment.TeacherId != teacherId)
            throw new InvalidOperationException("You are not authorized to delete this assignment.");

        if (assignment.Status != AssignmentStatus.Draft)
            throw new InvalidOperationException("Only draft assignments can be deleted.");

        var hasSubmissions = await _context.Submissions.AnyAsync(s => s.AssignmentId == assignmentId);
        if (hasSubmissions)
            throw new InvalidOperationException("Cannot delete assignment with existing submissions.");

        _context.Assignments.Remove(assignment);
        await _context.SaveChangesAsync();

        _logger.LogInformation("Teacher {TeacherId} deleted assignment {AssignmentId}", teacherId, assignmentId);
        return true;
    }

    public async Task<AssignmentDto?> PublishAssignmentAsync(Guid assignmentId, Guid teacherId)
    {
        var assignment = await _context.Assignments.FindAsync(assignmentId);
        if (assignment == null) return null;

        if (assignment.TeacherId != teacherId)
            throw new InvalidOperationException("You are not authorized to publish this assignment.");

        if (assignment.Status != AssignmentStatus.Draft)
            throw new InvalidOperationException("Only draft assignments can be published.");

        assignment.Status = AssignmentStatus.Published;
        assignment.PublishedAt = DateTime.UtcNow;
        assignment.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();
        _logger.LogInformation("Teacher {TeacherId} published assignment {AssignmentId}", teacherId, assignmentId);

        return await GetAssignmentByIdAsync(assignmentId);
    }

    // ─── Student: view assignments for their class ────────────────────────────
    public async Task<PaginatedResponse<StudentAssignmentDto>> GetStudentAssignmentsAsync(Guid studentId, PaginationParams pagination, AssignmentFilterParams filters)
    {
        var student = await _context.Students.FindAsync(studentId);
        if (student == null) throw new InvalidOperationException("Student not found.");

        var query = BuildAssignmentQuery(filters)
            .Where(a => a.ClassId == student.ClassId && a.Status != AssignmentStatus.Draft);

        var totalItems = await query.CountAsync();
        var assignments = await query
            .OrderByDescending(a => a.Deadline)
            .Skip((pagination.Page - 1) * pagination.PageSize)
            .Take(pagination.PageSize)
            .ToListAsync();

        // Get student's submissions for these assignments
        var assignmentIds = assignments.Select(a => a.Id).ToList();
        var submissions = await _context.Submissions
            .Where(s => s.StudentId == studentId && assignmentIds.Contains(s.AssignmentId))
            .ToListAsync();

        var items = assignments.Select(a =>
        {
            var sub = submissions.FirstOrDefault(s => s.AssignmentId == a.Id);
            var dto = MapToStudentDto(a, sub);
            return dto;
        }).ToList();

        return new PaginatedResponse<StudentAssignmentDto>
        {
            Items = items,
            Page = pagination.Page,
            PageSize = pagination.PageSize,
            TotalItems = totalItems
        };
    }

    public async Task<StudentAssignmentDto?> GetStudentAssignmentByIdAsync(Guid assignmentId, Guid studentId)
    {
        var student = await _context.Students.FindAsync(studentId);
        if (student == null) return null;

        var assignment = await GetAssignmentQueryable()
            .FirstOrDefaultAsync(a => a.Id == assignmentId && a.ClassId == student.ClassId);

        if (assignment == null) return null;

        var submission = await _context.Submissions
            .FirstOrDefaultAsync(s => s.AssignmentId == assignmentId && s.StudentId == studentId);

        return MapToStudentDto(assignment, submission);
    }

    // ─── Private helpers ────────────────────────────

    private IQueryable<Assignment> GetAssignmentQueryable()
    {
        return _context.Assignments
            .Include(a => a.Subject)
            .Include(a => a.Class)
            .Include(a => a.Teacher).ThenInclude(t => t.User)
            .Include(a => a.Attachments)
            .Include(a => a.Submissions);
    }

    private IQueryable<Assignment> BuildAssignmentQuery(AssignmentFilterParams filters)
    {
        var query = GetAssignmentQueryable();

        if (!string.IsNullOrWhiteSpace(filters.Search))
        {
            var search = filters.Search.ToLower();
            query = query.Where(a => a.Title.ToLower().Contains(search));
        }

        if (filters.SubjectId.HasValue)
            query = query.Where(a => a.SubjectId == filters.SubjectId.Value);

        if (filters.ClassId.HasValue)
            query = query.Where(a => a.ClassId == filters.ClassId.Value);

        if (filters.TeacherId.HasValue)
            query = query.Where(a => a.TeacherId == filters.TeacherId.Value);

        if (!string.IsNullOrWhiteSpace(filters.Status) && Enum.TryParse<AssignmentStatus>(filters.Status, true, out var status))
            query = query.Where(a => a.Status == status);

        return query;
    }

    private async Task<PaginatedResponse<AssignmentDto>> PaginateAssignmentsAsync(IQueryable<Assignment> query, PaginationParams pagination)
    {
        var totalItems = await query.CountAsync();
        var items = await query
            .OrderByDescending(a => a.CreatedAt)
            .Skip((pagination.Page - 1) * pagination.PageSize)
            .Take(pagination.PageSize)
            .ToListAsync();

        return new PaginatedResponse<AssignmentDto>
        {
            Items = items.Select(MapToDto).ToList(),
            Page = pagination.Page,
            PageSize = pagination.PageSize,
            TotalItems = totalItems
        };
    }

    private static AssignmentDto MapToDto(Assignment a)
    {
        return new AssignmentDto
        {
            Id = a.Id,
            Title = a.Title,
            Description = a.Description,
            SubjectId = a.SubjectId,
            SubjectName = a.Subject?.Name ?? "",
            SubjectCode = a.Subject?.Code ?? "",
            ClassId = a.ClassId,
            ClassName = a.Class?.Name ?? "",
            TeacherId = a.TeacherId,
            TeacherName = a.Teacher?.User != null ? $"{a.Teacher.User.FirstName} {a.Teacher.User.LastName}" : "",
            Deadline = a.Deadline,
            MaximumMarks = a.MaximumMarks,
            Status = a.Status.ToString(),
            SubmissionCount = a.Submissions?.Count ?? 0,
            TotalStudents = a.Class?.Students?.Count ?? 0,
            CreatedAt = a.CreatedAt,
            PublishedAt = a.PublishedAt,
            Attachments = a.Attachments?.Select(att => new AttachmentDto
            {
                Id = att.Id,
                FileName = att.FileName,
                ContentType = att.ContentType,
                FileSize = att.FileSize,
                UploadedAt = att.UploadedAt
            }).ToList() ?? new()
        };
    }

    private static StudentAssignmentDto MapToStudentDto(Assignment a, Submission? sub)
    {
        var dto = new StudentAssignmentDto
        {
            Id = a.Id,
            Title = a.Title,
            Description = a.Description,
            SubjectId = a.SubjectId,
            SubjectName = a.Subject?.Name ?? "",
            SubjectCode = a.Subject?.Code ?? "",
            ClassId = a.ClassId,
            ClassName = a.Class?.Name ?? "",
            TeacherId = a.TeacherId,
            TeacherName = a.Teacher?.User != null ? $"{a.Teacher.User.FirstName} {a.Teacher.User.LastName}" : "",
            Deadline = a.Deadline,
            MaximumMarks = a.MaximumMarks,
            Status = a.Status.ToString(),
            SubmissionCount = a.Submissions?.Count ?? 0,
            TotalStudents = a.Class?.Students?.Count ?? 0,
            CreatedAt = a.CreatedAt,
            PublishedAt = a.PublishedAt,
            Attachments = a.Attachments?.Select(att => new AttachmentDto
            {
                Id = att.Id,
                FileName = att.FileName,
                ContentType = att.ContentType,
                FileSize = att.FileSize,
                UploadedAt = att.UploadedAt
            }).ToList() ?? new()
        };

        if (sub != null)
        {
            dto.SubmissionStatus = sub.Status.ToString();
            dto.MyMarks = sub.Marks;
            dto.SubmissionId = sub.Id;
        }
        else
        {
            // Check if overdue
            dto.SubmissionStatus = a.Deadline < DateTime.UtcNow ? "Overdue" : "Not Submitted";
        }

        return dto;
    }
}
