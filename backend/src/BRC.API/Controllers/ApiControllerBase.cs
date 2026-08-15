using System.Security.Claims;
using BRC.Application.DTOs.Common;
using Microsoft.AspNetCore.Mvc;

namespace BRC.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public abstract class ApiControllerBase : ControllerBase
{
    protected Guid CurrentUserId
    {
        get
        {
            var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
            return string.IsNullOrEmpty(userIdString) ? Guid.Empty : Guid.Parse(userIdString);
        }
    }

    protected string CurrentUserRole => User.FindFirstValue(ClaimTypes.Role) ?? string.Empty;

    protected IActionResult OkResult<T>(T data, string message = "")
        => Ok(ApiResponse<T>.Ok(data, message));

    protected IActionResult ErrorResult(string message, int statusCode = 400, List<string>? errors = null)
        => StatusCode(statusCode, ApiResponse.Fail(message, errors));

    protected async Task<Guid?> GetTeacherIdAsync(BRC.Infrastructure.Data.BrcDbContext context)
    {
        var teacher = await Microsoft.EntityFrameworkCore.EntityFrameworkQueryableExtensions.FirstOrDefaultAsync(
            context.Teachers, t => t.UserId == CurrentUserId);
        return teacher?.Id;
    }

    protected async Task<Guid?> GetStudentIdAsync(BRC.Infrastructure.Data.BrcDbContext context)
    {
        var student = await Microsoft.EntityFrameworkCore.EntityFrameworkQueryableExtensions.FirstOrDefaultAsync(
            context.Students, s => s.UserId == CurrentUserId);
        return student?.Id;
    }
}
