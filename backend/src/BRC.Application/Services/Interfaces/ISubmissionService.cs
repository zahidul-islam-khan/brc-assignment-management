using BRC.Application.DTOs.Common;
using BRC.Application.DTOs.Submissions;

namespace BRC.Application.Services.Interfaces;

public interface ISubmissionService
{
    // Admin/Teacher
    Task<PaginatedResponse<SubmissionDto>> GetSubmissionsAsync(PaginationParams pagination, SubmissionFilterParams filters);
    Task<PaginatedResponse<SubmissionDto>> GetAssignmentSubmissionsAsync(Guid assignmentId, Guid teacherId, PaginationParams pagination);
    Task<SubmissionDto?> GetSubmissionByIdAsync(Guid submissionId);

    // Teacher grading
    Task<SubmissionDto?> GradeSubmissionAsync(Guid submissionId, Guid teacherId, GradeSubmissionDto dto);

    // Student
    Task<PaginatedResponse<SubmissionDto>> GetStudentSubmissionsAsync(Guid studentId, PaginationParams pagination, SubmissionFilterParams filters);
    Task<SubmissionDto?> GetStudentSubmissionByIdAsync(Guid submissionId, Guid studentId);
    Task<SubmissionDto> CreateSubmissionAsync(Guid assignmentId, Guid studentId, CreateSubmissionDto dto);
    Task<SubmissionDto?> UpdateSubmissionAsync(Guid submissionId, Guid studentId, UpdateSubmissionDto dto);
    Task<SubmissionDto?> SubmitSubmissionAsync(Guid submissionId, Guid studentId);
    Task<SubmissionDto?> UploadFileAsync(Guid submissionId, Guid studentId, Microsoft.AspNetCore.Http.IFormFile file);
}
