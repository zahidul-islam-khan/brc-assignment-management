using BRC.Application.DTOs.Common;
using BRC.Application.DTOs.Users;

namespace BRC.Application.Services.Interfaces;

public interface IUserService
{
    Task<PaginatedResponse<UserDto>> GetUsersAsync(PaginationParams pagination, UserFilterParams filters);
    Task<UserDto?> GetUserByIdAsync(Guid id);
    Task<UserDto> CreateUserAsync(CreateUserDto dto);
    Task<UserDto?> UpdateUserAsync(Guid id, UpdateUserDto dto);
    Task<bool> DeleteUserAsync(Guid id);
    Task<bool> UpdateStatusAsync(Guid id, UpdateUserStatusDto dto);
}
