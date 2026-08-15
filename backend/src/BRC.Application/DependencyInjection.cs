using BRC.Application.Services;
using BRC.Application.Services.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace BRC.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<IUserService, UserService>();
        services.AddScoped<IClassService, ClassService>();
        services.AddScoped<ISubjectService, SubjectService>();
        services.AddScoped<IAssignmentService, AssignmentService>();
        services.AddScoped<ISubmissionService, SubmissionService>();
        services.AddScoped<ITeacherAssignmentService, TeacherAssignmentService>();
        services.AddScoped<IDashboardService, DashboardService>();
        services.AddScoped<INotificationService, NotificationService>();
        services.AddScoped<IFileStorageService, FileStorageService>();

        return services;
    }
}
