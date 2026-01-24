using MessageCenter.Application.Contracts.Events;
using MessageCenter.Integration.Hubs;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using Volo.Abp.DependencyInjection;
using Volo.Abp.EventBus.Distributed;

namespace MessageCenter.Integration.EventHandlers;

/// <summary>
/// 未读数量变更事件处理器
/// 处理未读数量变更事件，执行实际的SignalR推送
/// 位于集成层，负责将业务事件转换为基础设施调用
/// </summary>
public class UnreadCountChangedEventHandler(
    IHubContext<MessageHub> hubContext,
    ILogger<UnreadCountChangedEventHandler> logger) : IDistributedEventHandler<UnreadCountChangedEvent>, ITransientDependency
{
    private readonly IHubContext<MessageHub> _hubContext = hubContext;
    private readonly ILogger<UnreadCountChangedEventHandler> _logger = logger;

    public async Task HandleEventAsync(UnreadCountChangedEvent eventData)
    {
        try
        {
            var groupName = $"user_{eventData.ReceiverId}";
            await _hubContext.Clients.Group(groupName).SendAsync("NotifyNewMessage", new
            {
                receiverId = eventData.ReceiverId,
                unreadCount = eventData.UnreadCount,
                timestamp = DateTime.UtcNow
            });

            _logger.LogInformation("已通过事件处理器通知用户 {ReceiverId} 未读数量变更为 {UnreadCount}", 
                eventData.ReceiverId, eventData.UnreadCount);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "处理未读数量变更事件失败，接收者: {ReceiverId}", eventData.ReceiverId);
            // 不抛出异常，避免事件总线重试
        }
    }
}
