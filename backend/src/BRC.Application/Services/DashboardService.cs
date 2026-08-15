using BRC.Application.DTOs.Dashboard;
using BRC.Application.Services.Interfaces;
using BRC.Domain.Enums;
using BRC.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace BRC.Application.Services;

public class DashboardService : IDashboardService
{
    private readonly BrcDbContext _context;

    public DashboardService(BrcDbContext context) => _context = context;

    public async Task<AdminDashboardDto> GetAdminDashboardAsync()
    {
        return new AdminDashboardDto
        {
            TotalUsers = await _context.Users.CountAsync(),
            TotalTeachers = await _context.Users.CountAsync(u => u.Role == UserRole.Teacher),
            TotalStudents = await _context.Users.CountAsync(u => u.Role == UserRole.Student),
            TotalClasses = await _context.Classes.CountAsync(c => c.IsActive),
            TotalSubjects = await _context.Subjects.CountAsync(s => s.IsActive),
            TotalAssignments = await _context.Assignments.CountAsync(),
            TotalSubmissions = await _context.Submissions.CountAsync(),
            PendingSubmissions = await _context.Submissions.CountAsync(s =>
                s.Status == SubmissionStatus.Submitted || s.Status == SubmissionStatus.Late),
        };
    }

    public async Task<TeacherDashboardDto> GetTeacherDashboardAsync(Guid teacherId)
    {
        var assignments = _context.Assignments.Where(a => a.TeacherId == teacherId);

        return new TeacherDashboardDto
        {
            TotalAssignments = await assignments.CountAsync(),
            PublishedAssignments = await assignments.CountAsync(a => a.Status == AssignmentStatus.Published),
            DraftAssignments = await assignments.CountAsync(a => a.Status == AssignmentStatus.Draft),
            TotalSubmissions = await _context.Submissions.CountAsync(s => s.Assignment.TeacherId == teacherId),
            PendingGrading = await _context.Submissions.CountAsync(s =>
                s.Assignment.TeacherId == teacherId &&
                (s.Status == SubmissionStatus.Submitted || s.Status == SubmissionStatus.Late)),
            TotalClasses = await _context.TeacherSubjectClasses
                .Where(t => t.TeacherId == teacherId)
                .Select(t => t.ClassId).Distinct().CountAsync(),
            TotalSubjects = await _context.TeacherSubjectClasses
                .Where(t => t.TeacherId == teacherId)
                .Select(t => t.SubjectId).Distinct().CountAsync(),
        };
    }

    public async Task<StudentDashboardDto> GetStudentDashboardAsync(Guid studentId)
    {
        var student = await _context.Students
            .Include(s => s.Class)
            .Include(s => s.AcademicGroup)
            .FirstOrDefaultAsync(s => s.Id == studentId);

        if (student == null) throw new InvalidOperationException("Student not found.");

        var classAssignments = _context.Assignments
            .Where(a => a.ClassId == student.ClassId && a.Status != AssignmentStatus.Draft);

        var totalAssignments = await classAssignments.CountAsync();
        var submissions = _context.Submissions.Where(s => s.StudentId == studentId);

        var gradedSubmissions = await submissions
            .Where(s => s.Status == SubmissionStatus.Graded || s.Status == SubmissionStatus.Returned)
            .ToListAsync();

        var averageMarks = gradedSubmissions.Any() ? gradedSubmissions.Average(s => (double?)(s.Marks ?? 0)) : null;

        var submittedIds = await submissions.Select(s => s.AssignmentId).ToListAsync();
        var overdueCount = await classAssignments
            .Where(a => a.Deadline < DateTime.UtcNow && !submittedIds.Contains(a.Id))
            .CountAsync();

        return new StudentDashboardDto
        {
            TotalAssignments = totalAssignments,
            SubmittedCount = await submissions.CountAsync(s =>
                s.Status == SubmissionStatus.Submitted || s.Status == SubmissionStatus.Late),
            PendingCount = totalAssignments - await submissions.CountAsync(),
            GradedCount = gradedSubmissions.Count,
            OverdueCount = overdueCount,
            AverageMarks = averageMarks.HasValue ? (decimal)averageMarks.Value : null,
            ClassName = student.Class?.Name ?? "",
            GroupName = student.AcademicGroup?.Name ?? ""
        };
    }
}
