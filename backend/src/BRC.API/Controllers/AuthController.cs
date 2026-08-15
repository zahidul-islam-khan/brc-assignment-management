using BRC.Application.DTOs.Auth;
using BRC.Application.DTOs.Common;
using BRC.Application.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BRC.API.Controllers;

[AllowAnonymous]
public class AuthController : ApiControllerBase
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }

    [HttpPost("login")]
    [ProducesResponseType(typeof(ApiResponse<LoginResponseDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Login([FromBody] LoginRequestDto request)
    {
        if (string.IsNullOrWhiteSpace(request.Email) || string.IsNullOrWhiteSpace(request.Password))
            return ErrorResult("Email and password are required.", StatusCodes.Status400BadRequest);

        var response = await _authService.LoginAsync(request);

        if (response == null)
            return ErrorResult("Invalid credentials or inactive account.", StatusCodes.Status401Unauthorized);

        return OkResult(response, "Login successful");
    }

    [HttpGet("me")]
    [Authorize]
    [ProducesResponseType(typeof(ApiResponse<UserInfoDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetCurrentUser([FromServices] IUserService userService)
    {
        var user = await userService.GetUserByIdAsync(CurrentUserId);
        if (user == null)
            return ErrorResult("User not found", StatusCodes.Status404NotFound);

        var userInfo = new UserInfoDto
        {
            Id = user.Id,
            FirstName = user.FirstName,
            LastName = user.LastName,
            Email = user.Email,
            Phone = user.Phone,
            Role = user.Role,
            Status = user.Status,
            StudentId = user.StudentId,
            ClassName = user.ClassName,
            GroupName = user.GroupName,
            EmployeeId = user.EmployeeId,
            Department = user.Department
        };

        return OkResult(userInfo);
    }
}
