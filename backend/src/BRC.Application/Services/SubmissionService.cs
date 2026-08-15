using BRC.Application.DTOs.Common;
using BRC.Application.DTOs.Submissions;
using BRC.Application.Services.Interfaces;
using BRC.Domain.Entities;
using BRC.Domain.Enums;
using BRC.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace BRC.Application.Services;

public class SubmissionService : ISubmissionService
{
    private readonly BrcDbContext _context;
    private readonly ILogger<SubmissionService> _logger;
    private readonly IFileStorageService _fileStorageService;

    public SubmissionService(BrcDbContext context, ILogger<SubmissionService> logger, IFileStorageService fileStorageService)
    {
        _context = context;
        _logger = logger;
        _fileStorageService = fileStorageService;
    }

    // ─── Admin/Teacher: all submissions ────────────────────────────
    public async Task<PaginatedResponse<SubmissionDto>> GetSubmissionsAsync(PaginationParams pagination, SubmissionFilterParams filters)
    {
        var query = BuildSubmissionQuery(filters);
        return await PaginateAsync(query, pagination);
    }

    // ─── Teacher: submissions for a specific assignment ────────────────────────────
    public async Task<PaginatedResponse<SubmissionDto>> GetAssignmentSubmissionsAsync(Guid assignmentId, Guid teacherId, PaginationParams pagination)
    {
        var assignment = await _context.Assignments.FindAsync(assignmentId);
        if (assignment == null) throw new InvalidOperationException("Assignment not found.");
        if (assignment.TeacherId != teacherId)
            throw new InvalidOperationException("You are not authorized to view submissions for this assignment.");

        var query = GetSubmissionQueryable().Where(s => s.AssignmentId == assignmentId);
        return await PaginateAsync(query, pagination);
    }

    public async Task<SubmissionDto?> GetSubmissionByIdAsync(Guid submissionId)
    {
        var sub = await GetSubmissionQueryable().FirstOrDefaultAsync(s => s.Id == submissionId);
        return sub == null ? null : MapToDto(sub);
    }

    // ─── Teacher: grade submission ────────────────────────────
    public async Task<SubmissionDto?> GradeSubmissionAsync(Guid submissionId, Guid teacherId, GradeSubmissionDto dto)
    {
        var submission = await _context.Submissions
            .Include(s => s.Assignment)
            .FirstOrDefaultAsync(s => s.Id == submissionId);

        if (submission == null) return null;

        // Verify teacher owns the assignment
        if (submission.Assignment.TeacherId != teacherId)
            throw new InvalidOperationException("You are not authorized to grade this submission.");

        // Validate marks
        if (dto.Marks < 0)
            throw new InvalidOperationException("Marks cannot be negative.");
        if (dto.Marks > submission.Assignment.MaximumMarks)
            throw new InvalidOperationException($"Marks cannot exceed the maximum marks ({submission.Assignment.MaximumMarks}).");

        submission.Marks = dto.Marks;
        submission.TeacherFeedback = dto.Feedback;
        submission.GradedAt = DateTime.UtcNow;
        submission.UpdatedAt = DateTime.UtcNow;

        // Determine status
        if (!string.IsNullOrWhiteSpace(dto.Status) && Enum.TryParse<SubmissionStatus>(dto.Status, true, out var gradeStatus))
        {
            if (gradeStatus == SubmissionStatus.Graded || gradeStatus == SubmissionStatus.Returned)
                submission.Status = gradeStatus;
        }
        else
        {
            submission.Status = SubmissionStatus.Graded;
        }

        await _context.SaveChangesAsync();
        _logger.LogInformation("Teacher {TeacherId} graded submission {SubmissionId}: {Marks}/{Max}",
            teacherId, submissionId, dto.Marks, submission.Assignment.MaximumMarks);

        return await GetSubmissionByIdAsync(submissionId);
    }

    // ─── Student: get own submissions ────────────────────────────
    public async Task<PaginatedResponse<SubmissionDto>> GetStudentSubmissionsAsync(Guid studentId, PaginationParams pagination, SubmissionFilterParams filters)
    {
        var query = BuildSubmissionQuery(filters).Where(s => s.StudentId == studentId);
        return await PaginateAsync(query, pagination);
    }

    public async Task<SubmissionDto?> GetStudentSubmissionByIdAsync(Guid submissionId, Guid studentId)
    {
        var sub = await GetSubmissionQueryable()
            .FirstOrDefaultAsync(s => s.Id == submissionId && s.StudentId == studentId);
        return sub == null ? null : MapToDto(sub);
    }

    // ─── Student: create submission ────────────────────────────
    public async Task<SubmissionDto> CreateSubmissionAsync(Guid assignmentId, Guid studentId, CreateSubmissionDto dto)
    {
        // Get assignment with class info
        var assignment = await _context.Assignments
            .Include(a => a.Class)
            .FirstOrDefaultAsync(a => a.Id == assignmentId);

        if (assignment == null)
            throw new InvalidOperationException("Assignment not found.");

        if (assignment.Status != AssignmentStatus.Published)
            throw new InvalidOperationException("This assignment is not accepting submissions.");

        // Verify student belongs to the assignment's class
        var student = await _context.Students.FindAsync(studentId);
        if (student == null)
            throw new InvalidOperationException("Student not found.");

        if (student.ClassId != assignment.ClassId)
            throw new InvalidOperationException("You are not enrolled in the class this assignment belongs to.");

        // Check for existing submission
        var existing = await _context.Submissions
            .FirstOrDefaultAsync(s => s.AssignmentId == assignmentId && s.StudentId == studentId);

        if (existing != null)
            throw new InvalidOperationException("You have already submitted this assignment. Use update instead.");

        // Check if late submission is allowed
        var isLate = DateTime.UtcNow > assignment.Deadline;
        if (isLate)
        {
            var allowLate = await GetSettingAsync("AllowLateSubmission");
            if (allowLate != "true")
                throw new InvalidOperationException("Late submissions are not allowed for this assignment.");
        }

        var submission = new Submission
        {
            Id = Guid.NewGuid(),
            AssignmentId = assignmentId,
            StudentId = studentId,
            TextAnswer = dto.TextAnswer,
            Status = SubmissionStatus.Draft, // Start as draft
            UpdatedAt = DateTime.UtcNow
        };

        _context.Submissions.Add(submission);
        await _context.SaveChangesAsync();

        _logger.LogInformation("Student {StudentId} created submission for assignment {AssignmentId}", studentId, assignmentId);
        return (await GetSubmissionByIdAsync(submission.Id))!;
    }

    // ─── Student: update draft submission ────────────────────────────
    public async Task<SubmissionDto?> UpdateSubmissionAsync(Guid submissionId, Guid studentId, UpdateSubmissionDto dto)
    {
        var submission = await _context.Submissions
            .Include(s => s.Assignment)
            .FirstOrDefaultAsync(s => s.Id == submissionId && s.StudentId == studentId);

        if (submission == null) return null;

        // Check if resubmission is allowed
        if (submission.Status != SubmissionStatus.Draft)
        {
            var allowResub = await GetSettingAsync("AllowResubmission");
            if (allowResub != "true")
                throw new InvalidOperationException("Resubmission is not allowed.");

            if (submission.Status == SubmissionStatus.Graded || submission.Status == SubmissionStatus.Returned)
                throw new InvalidOperationException("Cannot modify a graded or returned submission.");
        }

        // Check deadline
        if (DateTime.UtcNow > submission.Assignment.Deadline)
        {
            var allowLate = await GetSettingAsync("AllowLateSubmission");
            if (allowLate != "true")
                throw new InvalidOperationException("The deadline has passed. Updates are not allowed.");
        }

        submission.TextAnswer = dto.TextAnswer;
        submission.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();
        _logger.LogInformation("Student {StudentId} updated submission {SubmissionId}", studentId, submissionId);

        return await GetSubmissionByIdAsync(submissionId);
    }

    public async Task<SubmissionDto?> UploadFileAsync(Guid submissionId, Guid studentId, Microsoft.AspNetCore.Http.IFormFile file)
    {
        var submission = await _context.Submissions
            .Include(s => s.Assignment)
            .FirstOrDefaultAsync(s => s.Id == submissionId && s.StudentId == studentId);

        if (submission == null) return null;

        if (submission.Status != SubmissionStatus.Draft)
        {
            var allowResub = await GetSettingAsync("AllowResubmission");
            if (allowResub != "true")
                throw new InvalidOperationException("Resubmission is not allowed.");

            if (submission.Status == SubmissionStatus.Graded || submission.Status == SubmissionStatus.Returned)
                throw new InvalidOperationException("Cannot modify a graded or returned submission.");
        }

        if (DateTime.UtcNow > submission.Assignment.Deadline)
        {
            var allowLate = await GetSettingAsync("AllowLateSubmission");
            if (allowLate != "true")
                throw new InvalidOperationException("The deadline has passed. Updates are not allowed.");
        }

        var (path, size) = await _fileStorageService.SaveFileAsync(file, "submissions");

        var submissionFile = new SubmissionFile
        {
            Id = Guid.NewGuid(),
            SubmissionId = submissionId,
            FileName = file.FileName,
            ContentType = file.ContentType,
            FilePath = path,
            FileSize = size,
            UploadedAt = DateTime.UtcNow
        };

        _context.SubmissionFiles.Add(submissionFile);
        submission.UpdatedAt = DateTime.UtcNow;
        
        await _context.SaveChangesAsync();
        return await GetSubmissionByIdAsync(submissionId);
    }

    // ─── Student: submit (change from draft to submitted) ────────────────────────────
    public async Task<SubmissionDto?> SubmitSubmissionAsync(Guid submissionId, Guid studentId)
    {
        var submission = await _context.Submissions
            .Include(s => s.Assignment)
            .FirstOrDefaultAsync(s => s.Id == submissionId && s.StudentId == studentId);

        if (submission == null) return null;

        if (submission.Status != SubmissionStatus.Draft &&
            submission.Status != SubmissionStatus.Submitted)
        {
            var allowResub = await GetSettingAsync("AllowResubmission");
            if (allowResub != "true")
                throw new InvalidOperationException("This submission cannot be resubmitted.");
        }

        // Check for closed assignment
        if (submission.Assignment.Status == AssignmentStatus.Closed)
        {
            throw new InvalidOperationException("This assignment is closed and no longer accepts submissions.");
        }

        var now = DateTime.UtcNow;
        submission.SubmittedAt = now;
        submission.UpdatedAt = now;

        // Auto-detect late submission
        if (now > submission.Assignment.Deadline)
        {
            submission.Status = SubmissionStatus.Late;
            _logger.LogInformation("Student {StudentId} submitted late for assignment {AssignmentId}", studentId, submission.AssignmentId);
        }
        else
        {
            submission.Status = SubmissionStatus.Submitted;
        }

        await _context.SaveChangesAsync();
        _logger.LogInformation("Student {StudentId} submitted submission {SubmissionId}", studentId, submissionId);

        return await GetSubmissionByIdAsync(submissionId);
    }

    // ─── Private helpers ────────────────────────────

    private async Task<string?> GetSettingAsync(string key)
    {
        var setting = await _context.ApplicationSettings.FirstOrDefaultAsync(s => s.Key == key);
        return setting?.Value;
    }

    private IQueryable<Submission> GetSubmissionQueryable()
    {
        return _context.Submissions
            .Include(s => s.Assignment).ThenInclude(a => a.Subject)
            .Include(s => s.Assignment).ThenInclude(a => a.Class)
            .Include(s => s.Assignment).ThenInclude(a => a.Teacher).ThenInclude(t => t.User)
            .Include(s => s.Student).ThenInclude(st => st.User)
            .Include(s => s.Files);
    }

    private IQueryable<Submission> BuildSubmissionQuery(SubmissionFilterParams filters)
    {
        var query = GetSubmissionQueryable();

        if (!string.IsNullOrWhiteSpace(filters.Search))
        {
            var search = filters.Search.ToLower();
            query = query.Where(s =>
                s.Student.User.FirstName.ToLower().Contains(search) ||
                s.Student.User.LastName.ToLower().Contains(search) ||
                s.Assignment.Title.ToLower().Contains(search));
        }

        if (filters.AssignmentId.HasValue)
            query = query.Where(s => s.AssignmentId == filters.AssignmentId.Value);

        if (filters.ClassId.HasValue)
            query = query.Where(s => s.Assignment.ClassId == filters.ClassId.Value);

        if (filters.StudentId.HasValue)
            query = query.Where(s => s.StudentId == filters.StudentId.Value);

        if (!string.IsNullOrWhiteSpace(filters.Status) && Enum.TryParse<SubmissionStatus>(filters.Status, true, out var status))
            query = query.Where(s => s.Status == status);

        return query;
    }

    private async Task<PaginatedResponse<SubmissionDto>> PaginateAsync(IQueryable<Submission> query, PaginationParams pagination)
    {
        var totalItems = await query.CountAsync();
        var items = await query
            .OrderByDescending(s => s.SubmittedAt ?? s.UpdatedAt)
            .Skip((pagination.Page - 1) * pagination.PageSize)
            .Take(pagination.PageSize)
            .ToListAsync();

        return new PaginatedResponse<SubmissionDto>
        {
            Items = items.Select(MapToDto).ToList(),
            Page = pagination.Page,
            PageSize = pagination.PageSize,
            TotalItems = totalItems
        };
    }

    private static SubmissionDto MapToDto(Submission s)
    {
        return new SubmissionDto
        {
            Id = s.Id,
            AssignmentId = s.AssignmentId,
            AssignmentTitle = s.Assignment?.Title ?? "",
            SubjectName = s.Assignment?.Subject?.Name ?? "",
            ClassName = s.Assignment?.Class?.Name ?? "",
            StudentId = s.StudentId,
            StudentName = s.Student?.User != null ? $"{s.Student.User.FirstName} {s.Student.User.LastName}" : "",
            StudentRollNumber = s.Student?.RollNumber,
            TextAnswer = s.TextAnswer,
            Status = s.Status.ToString(),
            SubmittedAt = s.SubmittedAt,
            Marks = s.Marks,
            MaximumMarks = s.Assignment?.MaximumMarks ?? 0,
            TeacherFeedback = s.TeacherFeedback,
            GradedAt = s.GradedAt,
            TeacherName = s.Assignment?.Teacher?.User != null
                ? $"{s.Assignment.Teacher.User.FirstName} {s.Assignment.Teacher.User.LastName}" : "",
            Files = s.Files?.Select(f => new SubmissionFileDto
            {
                Id = f.Id,
                FileName = f.FileName,
                ContentType = f.ContentType,
                FileSize = f.FileSize,
                UploadedAt = f.UploadedAt
            }).ToList() ?? new()
        };
    }
}
