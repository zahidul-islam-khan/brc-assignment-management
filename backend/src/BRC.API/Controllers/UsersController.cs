using BRC.Application.DTOs.Common;
using BRC.Application.DTOs.Users;
using BRC.Application.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BRC.API.Controllers;

[Authorize(Roles = "Admin")]
public class UsersController : ApiControllerBase
{
    private readonly IUserService _userService;

    public UsersController(IUserService userService)
    {
        _userService = userService;
    }

    [HttpGet]
    [ProducesResponseType(typeof(ApiResponse<PaginatedResponse<UserDto>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetUsers([FromQuery] PaginationParams pagination, [FromQuery] UserFilterParams filters)
    {
        var result = await _userService.GetUsersAsync(pagination, filters);
        return OkResult(result);
    }

    [HttpGet("{id}")]
    [ProducesResponseType(typeof(ApiResponse<UserDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetUser(Guid id)
    {
        var user = await _userService.GetUserByIdAsync(id);
        if (user == null) return ErrorResult("User not found", StatusCodes.Status404NotFound);
        return OkResult(user);
    }

    [HttpPost]
    [ProducesResponseType(typeof(ApiResponse<UserDto>), StatusCodes.Status201Created)]
    public async Task<IActionResult> CreateUser([FromBody] CreateUserDto dto)
    {
        var user = await _userService.CreateUserAsync(dto);
        return StatusCode(StatusCodes.Status201Created, ApiResponse<UserDto>.Ok(user, "User created successfully"));
    }

    [HttpPut("{id}")]
    [ProducesResponseType(typeof(ApiResponse<UserDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> UpdateUser(Guid id, [FromBody] UpdateUserDto dto)
    {
        var user = await _userService.UpdateUserAsync(id, dto);
        if (user == null) return ErrorResult("User not found", StatusCodes.Status404NotFound);
        return OkResult(user, "User updated successfully");
    }

    [HttpPatch("{id}/status")]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status200OK)]
    public async Task<IActionResult> UpdateUserStatus(Guid id, [FromBody] UpdateUserStatusDto dto)
    {
        var success = await _userService.UpdateStatusAsync(id, dto);
        if (!success) return ErrorResult("User not found", StatusCodes.Status404NotFound);
        return Ok(ApiResponse.Ok("User status updated successfully"));
    }

    [HttpDelete("{id}")]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status200OK)]
    public async Task<IActionResult> DeleteUser(Guid id)
    {
        var success = await _userService.DeleteUserAsync(id);
        if (!success) return ErrorResult("User not found", StatusCodes.Status404NotFound);
        return Ok(ApiResponse.Ok("User deleted successfully"));
    }
}
