using Microsoft.Extensions.Logging;
using System.Security.Claims;
using Volo.Abp.AspNetCore.SignalR;
using Volo.Abp.Security.Claims;

namespace MessageCenter.Integration.Hubs;

/// <summary>
/// Message realtime push hub.
/// </summary>
public class MessageHub(ILogger<MessageHub> logger) : AbpHub
{
    private readonly ILogger<MessageHub> _logger = logger;

    /// <summary>
    /// Triggered when a client connects.
    /// </summary>
    public override async Task OnConnectedAsync()
    {
        var userId = GetUserId();
        if (!string.IsNullOrEmpty(userId))
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, $"user_{userId}");
            _logger.LogInformation(
                "User {UserId} connected to MessageHub. ConnectionId: {ConnectionId}",
                userId,
                Context.ConnectionId);
        }

        await base.OnConnectedAsync();
    }

    /// <summary>
    /// Triggered when a client disconnects.
    /// </summary>
    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        var userId = GetUserId();
        if (!string.IsNullOrEmpty(userId))
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, $"user_{userId}");
            _logger.LogInformation(
                "User {UserId} disconnected from MessageHub. ConnectionId: {ConnectionId}",
                userId,
                Context.ConnectionId);
        }

        if (exception != null)
        {
            if (IsClientTimeoutDisconnect(exception))
            {
                _logger.LogInformation(
                    "User {UserId} disconnected from MessageHub because the SignalR client timed out. ConnectionId: {ConnectionId}, Reason: {Reason}",
                    userId,
                    Context.ConnectionId,
                    exception.Message);
            }
            else
            {
                _logger.LogError(
                    exception,
                    "User {UserId} disconnected from MessageHub with an unexpected error. ConnectionId: {ConnectionId}",
                    userId,
                    Context.ConnectionId);
            }
        }

        await base.OnDisconnectedAsync(exception);
    }

    private static bool IsClientTimeoutDisconnect(Exception exception)
    {
        return exception is OperationCanceledException
            && exception.Message.Contains("ClientTimeoutInterval", StringComparison.OrdinalIgnoreCase);
    }

    /// <summary>
    /// Adds the current client connection to a business group.
    /// </summary>
    public async Task JoinGroup(string groupName)
    {
        await Groups.AddToGroupAsync(Context.ConnectionId, groupName);
        _logger.LogInformation(
            "Connection {ConnectionId} joined group {GroupName}",
            Context.ConnectionId,
            groupName);
    }

    /// <summary>
    /// Removes the current client connection from a business group.
    /// </summary>
    public async Task LeaveGroup(string groupName)
    {
        await Groups.RemoveFromGroupAsync(Context.ConnectionId, groupName);
        _logger.LogInformation(
            "Connection {ConnectionId} left group {GroupName}",
            Context.ConnectionId,
            groupName);
    }

    /// <summary>
    /// Gets the current authenticated user id from ABP, JWT, or SignalR user identifiers.
    /// </summary>
    private string? GetUserId()
    {
        return Context.User?.FindFirst(AbpClaimTypes.UserId)?.Value
            ?? Context.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value
            ?? Context.User?.FindFirst("sub")?.Value
            ?? Context.User?.FindFirst("nameid")?.Value
            ?? Context.UserIdentifier;
    }
}
