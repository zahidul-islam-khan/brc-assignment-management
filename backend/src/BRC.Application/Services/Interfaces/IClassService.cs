using BRC.Application.DTOs.Classes;
using BRC.Application.DTOs.Common;
using BRC.Application.DTOs.Subjects;

namespace BRC.Application.Services.Interfaces;

public interface IClassService
{
    Task<PaginatedResponse<ClassDto>> GetClassesAsync(PaginationParams pagination, string? search = null, Guid? groupId = null);
    Task<ClassDto?> GetClassByIdAsync(Guid id);
    Task<ClassDto> CreateClassAsync(CreateClassDto dto);
    Task<ClassDto?> UpdateClassAsync(Guid id, UpdateClassDto dto);
    Task<bool> DeleteClassAsync(Guid id);
    Task<List<AcademicGroupDto>> GetAcademicGroupsAsync();
}
