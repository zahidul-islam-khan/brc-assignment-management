using BRC.Application.DTOs.Common;
using BRC.Application.DTOs.TeacherAssignments;

namespace BRC.Application.Services.Interfaces;

public interface ITeacherAssignmentService
{
    Task<PaginatedResponse<TeacherAssignmentDto>> GetTeacherAssignmentsAsync(PaginationParams pagination, Guid? teacherId = null, Guid? classId = null, Guid? subjectId = null);
    Task<TeacherAssignmentDto> CreateAsync(CreateTeacherAssignmentDto dto);
    Task<bool> DeleteAsync(Guid id);
}
