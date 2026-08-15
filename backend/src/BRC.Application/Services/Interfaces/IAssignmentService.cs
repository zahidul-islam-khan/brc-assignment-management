using BRC.Application.DTOs.Common;
using BRC.Application.DTOs.Assignments;

namespace BRC.Application.Services.Interfaces;

public interface IAssignmentService
{
    // Admin
    Task<PaginatedResponse<AssignmentDto>> GetAllAssignmentsAsync(PaginationParams pagination, AssignmentFilterParams filters);

    // Teacher
    Task<PaginatedResponse<AssignmentDto>> GetTeacherAssignmentsAsync(Guid teacherId, PaginationParams pagination, AssignmentFilterParams filters);
    Task<AssignmentDto?> GetAssignmentByIdAsync(Guid id);
    Task<AssignmentDto> CreateAssignmentAsync(Guid teacherId, CreateAssignmentDto dto);
    Task<AssignmentDto?> UpdateAssignmentAsync(Guid assignmentId, Guid teacherId, UpdateAssignmentDto dto);
    Task<bool> DeleteAssignmentAsync(Guid assignmentId, Guid teacherId);
    Task<AssignmentDto?> PublishAssignmentAsync(Guid assignmentId, Guid teacherId);

    // Student
    Task<PaginatedResponse<StudentAssignmentDto>> GetStudentAssignmentsAsync(Guid studentId, PaginationParams pagination, AssignmentFilterParams filters);
    Task<StudentAssignmentDto?> GetStudentAssignmentByIdAsync(Guid assignmentId, Guid studentId);
}
