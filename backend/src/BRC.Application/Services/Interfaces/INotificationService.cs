using BRC.Application.DTOs.Common;
using BRC.Application.DTOs.Notifications;

namespace BRC.Application.Services.Interfaces;

public interface INotificationService
{
    Task<PaginatedResponse<NotificationDto>> GetNotificationsAsync(Guid userId, PaginationParams pagination, bool? unreadOnly = null);
    Task<int> GetUnreadCountAsync(Guid userId);
    Task<bool> MarkAsReadAsync(Guid notificationId, Guid userId);
    Task<bool> MarkAllAsReadAsync(Guid userId);
    Task CreateNotificationAsync(Guid userId, string title, string message, string? type = null);
}
