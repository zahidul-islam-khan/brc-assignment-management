using BRC.Application.DTOs.Common;
using BRC.Application.DTOs.Notifications;
using BRC.Application.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BRC.API.Controllers;

[Authorize]
public class NotificationsController : ApiControllerBase
{
    private readonly INotificationService _notificationService;

    public NotificationsController(INotificationService notificationService)
    {
        _notificationService = notificationService;
    }

    [HttpGet]
    [ProducesResponseType(typeof(ApiResponse<PaginatedResponse<NotificationDto>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetNotifications([FromQuery] PaginationParams pagination, [FromQuery] bool? unreadOnly)
    {
        var result = await _notificationService.GetNotificationsAsync(CurrentUserId, pagination, unreadOnly);
        return OkResult(result);
    }

    [HttpGet("unread-count")]
    [ProducesResponseType(typeof(ApiResponse<int>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetUnreadCount()
    {
        var count = await _notificationService.GetUnreadCountAsync(CurrentUserId);
        return OkResult(count);
    }

    [HttpPatch("{id}/read")]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status200OK)]
    public async Task<IActionResult> MarkAsRead(Guid id)
    {
        var success = await _notificationService.MarkAsReadAsync(id, CurrentUserId);
        if (!success) return ErrorResult("Notification not found", StatusCodes.Status404NotFound);
        return Ok(ApiResponse.Ok("Notification marked as read"));
    }

    [HttpPatch("read-all")]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status200OK)]
    public async Task<IActionResult> MarkAllAsRead()
    {
        await _notificationService.MarkAllAsReadAsync(CurrentUserId);
        return Ok(ApiResponse.Ok("All notifications marked as read"));
    }
}
