using BRC.Application.DTOs.Common;
using BRC.Application.DTOs.Subjects;

namespace BRC.Application.Services.Interfaces;

public interface ISubjectService
{
    Task<PaginatedResponse<SubjectDto>> GetSubjectsAsync(PaginationParams pagination, string? search = null, Guid? groupId = null);
    Task<SubjectDto?> GetSubjectByIdAsync(Guid id);
    Task<SubjectDto> CreateSubjectAsync(CreateSubjectDto dto);
    Task<SubjectDto?> UpdateSubjectAsync(Guid id, UpdateSubjectDto dto);
    Task<bool> DeleteSubjectAsync(Guid id);
}
