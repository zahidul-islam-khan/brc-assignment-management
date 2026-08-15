using BRC.Application.DTOs.Dashboard;

namespace BRC.Application.Services.Interfaces;

public interface IDashboardService
{
    Task<AdminDashboardDto> GetAdminDashboardAsync();
    Task<TeacherDashboardDto> GetTeacherDashboardAsync(Guid teacherId);
    Task<StudentDashboardDto> GetStudentDashboardAsync(Guid studentId);
}
