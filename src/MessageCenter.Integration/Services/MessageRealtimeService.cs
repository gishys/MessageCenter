using MessageCenter.Application.Contracts.DTOs;
using MessageCenter.Application.Contracts.Services;
using MessageCenter.Integration.Hubs;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using Volo.Abp.DependencyInjection;

namespace MessageCenter.Integration.Services;

/// <summary>
/// 实时消息推送服务实现
/// 在集成层实现，可以访问IHubContext
/// 作为基础设施服务，负责实际的SignalR推送
/// </summary>
public class MessageRealtimeService(
    IHubContext<MessageHub> hubContext,
    ILogger<MessageRealtimeService> logger) : IMessageRealtimeService, ITransientDependency
{
    private readonly IHubContext<MessageHub> _hubContext = hubContext;
    private readonly ILogger<MessageRealtimeService> _logger = logger;

    public async Task SendToUserAsync(string receiverId, MessageDto message)
    {
        try
        {
            // 参数验证
            if (string.IsNullOrWhiteSpace(receiverId))
            {
                _logger.LogWarning("接收者ID为空，跳过SignalR推送");
                return;
            }

            if (message == null)
            {
                _logger.LogWarning("消息对象为空，跳过SignalR推送");
                return;
            }

            var groupName = $"user_{receiverId}";
            await _hubContext.Clients.Group(groupName).SendAsync("ReceiveMessage", message);
            _logger.LogInformation("已通过SignalR向用户 {ReceiverId} 推送消息 {MessageId}", receiverId, message.Id);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "通过SignalR向用户 {ReceiverId} 推送消息失败", receiverId);
            // 不抛出异常，避免影响主流程
        }
    }

    public async Task SendToUsersAsync(List<string> receiverIds, MessageDto message)
    {
        try
        {
            var tasks = new List<Task>();
            foreach (var receiverId in receiverIds)
            {
                var groupName = $"user_{receiverId}";
                tasks.Add(_hubContext.Clients.Group(groupName).SendAsync("ReceiveMessage", message));
            }

            await Task.WhenAll(tasks);
            _logger.LogInformation("已向 {Count} 个用户推送消息 {MessageId}", receiverIds.Count, message.Id);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "批量推送消息失败");
        }
    }

    public async Task SendToGroupAsync(string groupName, MessageDto message)
    {
        try
        {
            // 参数验证
            if (string.IsNullOrWhiteSpace(groupName))
            {
                _logger.LogWarning("组名称为空，跳过SignalR组播推送");
                return;
            }

            if (message == null)
            {
                _logger.LogWarning("消息对象为空，跳过SignalR组播推送");
                return;
            }

            await _hubContext.Clients.Group(groupName).SendAsync("ReceiveMessage", message);
            _logger.LogInformation("已通过SignalR向组 {GroupName} 推送消息 {MessageId}", groupName, message.Id);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "通过SignalR向组 {GroupName} 推送消息失败", groupName);
        }
    }

    public async Task BroadcastAsync(MessageDto message)
    {
        try
        {
            // 参数验证
            if (message == null)
            {
                _logger.LogWarning("消息对象为空，跳过SignalR广播推送");
                return;
            }

            await _hubContext.Clients.All.SendAsync("ReceiveMessage", message);
            _logger.LogInformation("已通过SignalR广播消息 {MessageId}", message.Id);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "通过SignalR广播消息失败");
        }
    }

    public async Task NotifyNewMessageAsync(string receiverId, long unreadCount)
    {
        try
        {
            var groupName = $"user_{receiverId}";
            await _hubContext.Clients.Group(groupName).SendAsync("NotifyNewMessage", new
            {
                receiverId,
                unreadCount,
                timestamp = DateTime.UtcNow
            });
            _logger.LogInformation("已通知用户 {ReceiverId} 有新消息，未读数量: {UnreadCount}", receiverId, unreadCount);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "通知用户新消息失败");
        }
    }

    public async Task NotifyMessageStatusChangedAsync(string receiverId, Guid messageId, string status)
    {
        try
        {
            var groupName = $"user_{receiverId}";
            await _hubContext.Clients.Group(groupName).SendAsync("MessageStatusChanged", new
            {
                messageId,
                status,
                timestamp = DateTime.UtcNow
            });
            _logger.LogInformation("已通知用户 {ReceiverId} 消息 {MessageId} 状态变更为 {Status}", receiverId, messageId, status);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "通知消息状态变更失败");
        }
    }

    public async Task NotifyTaskProgressAsync(string receiverId, Guid taskId, int progress, string? message = null, string? status = null)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(receiverId))
            {
                _logger.LogWarning("接收者ID为空，跳过任务进度推送");
                return;
            }
            var groupName = $"user_{receiverId}";
            await _hubContext.Clients.Group(groupName).SendAsync("TaskProgress", new
            {
                taskId,
                progress,
                message,
                status,
                timestamp = DateTime.UtcNow
            });
            _logger.LogDebug("已向用户 {ReceiverId} 推送任务 {TaskId} 进度 {Progress}%", receiverId, taskId, progress);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "推送任务进度失败 TaskId={TaskId}", taskId);
        }
    }
}
