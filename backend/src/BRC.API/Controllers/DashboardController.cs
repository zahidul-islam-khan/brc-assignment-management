using BRC.Application.DTOs.Common;
using BRC.Application.DTOs.Dashboard;
using BRC.Application.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BRC.API.Controllers;

[Authorize]
public class DashboardController : ApiControllerBase
{
    private readonly IDashboardService _dashboardService;
    private readonly BRC.Infrastructure.Data.BrcDbContext _context;

    public DashboardController(IDashboardService dashboardService, BRC.Infrastructure.Data.BrcDbContext context)
    {
        _dashboardService = dashboardService;
        _context = context;
    }

    [HttpGet]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetDashboard()
    {
        if (CurrentUserRole == "Admin")
        {
            var data = await _dashboardService.GetAdminDashboardAsync();
            return OkResult((object)data);
        }
        else if (CurrentUserRole == "Teacher")
        {
            var teacherId = await GetTeacherIdAsync(_context);
            if (teacherId == null) return ErrorResult("Teacher profile not found");
            var data = await _dashboardService.GetTeacherDashboardAsync(teacherId.Value);
            return OkResult((object)data);
        }
        else if (CurrentUserRole == "Student")
        {
            var studentId = await GetStudentIdAsync(_context);
            if (studentId == null) return ErrorResult("Student profile not found");
            var data = await _dashboardService.GetStudentDashboardAsync(studentId.Value);
            return OkResult((object)data);
        }

        return ErrorResult("Unauthorized", StatusCodes.Status403Forbidden);
    }
}
