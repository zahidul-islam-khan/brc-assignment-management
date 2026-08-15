using BRC.Application.DTOs.Common;
using BRC.Application.DTOs.Notifications;
using BRC.Application.Services.Interfaces;
using BRC.Domain.Entities;
using BRC.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace BRC.Application.Services;

public class NotificationService : INotificationService
{
    private readonly BrcDbContext _context;

    public NotificationService(BrcDbContext context) => _context = context;

    public async Task<PaginatedResponse<NotificationDto>> GetNotificationsAsync(Guid userId, PaginationParams pagination, bool? unreadOnly = null)
    {
        var query = _context.Notifications.Where(n => n.UserId == userId);

        if (unreadOnly == true)
            query = query.Where(n => !n.IsRead);

        var totalItems = await query.CountAsync();
        var items = await query
            .OrderByDescending(n => n.CreatedAt)
            .Skip((pagination.Page - 1) * pagination.PageSize)
            .Take(pagination.PageSize)
            .Select(n => new NotificationDto
            {
                Id = n.Id, Title = n.Title, Message = n.Message,
                Type = n.Type, IsRead = n.IsRead, CreatedAt = n.CreatedAt
            }).ToListAsync();

        return new PaginatedResponse<NotificationDto>
        {
            Items = items, Page = pagination.Page,
            PageSize = pagination.PageSize, TotalItems = totalItems
        };
    }

    public async Task<int> GetUnreadCountAsync(Guid userId)
        => await _context.Notifications.CountAsync(n => n.UserId == userId && !n.IsRead);

    public async Task<bool> MarkAsReadAsync(Guid notificationId, Guid userId)
    {
        var n = await _context.Notifications.FirstOrDefaultAsync(n => n.Id == notificationId && n.UserId == userId);
        if (n == null) return false;
        n.IsRead = true;
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> MarkAllAsReadAsync(Guid userId)
    {
        await _context.Notifications
            .Where(n => n.UserId == userId && !n.IsRead)
            .ExecuteUpdateAsync(s => s.SetProperty(n => n.IsRead, true));
        return true;
    }

    public async Task CreateNotificationAsync(Guid userId, string title, string message, string? type = null)
    {
        _context.Notifications.Add(new Notification
        {
            Id = Guid.NewGuid(), UserId = userId,
            Title = title, Message = message, Type = type,
            CreatedAt = DateTime.UtcNow
        });
        await _context.SaveChangesAsync();
    }
}
