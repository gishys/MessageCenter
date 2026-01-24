using MessageCenter.Application.Contracts.Events;
using MessageCenter.Integration.Hubs;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using Volo.Abp.DependencyInjection;
using Volo.Abp.EventBus.Distributed;

namespace MessageCenter.Integration.EventHandlers;

/// <summary>
/// 消息状态变更事件处理器
/// 处理消息状态变更事件，执行实际的SignalR推送
/// 位于集成层，负责将业务事件转换为基础设施调用
/// </summary>
public class MessageStatusChangedEventHandler(
    IHubContext<MessageHub> hubContext,
    ILogger<MessageStatusChangedEventHandler> logger) : IDistributedEventHandler<MessageStatusChangedEvent>, ITransientDependency
{
    private readonly IHubContext<MessageHub> _hubContext = hubContext;
    private readonly ILogger<MessageStatusChangedEventHandler> _logger = logger;

    public async Task HandleEventAsync(MessageStatusChangedEvent eventData)
    {
        try
        {
            var groupName = $"user_{eventData.ReceiverId}";
            await _hubContext.Clients.Group(groupName).SendAsync("MessageStatusChanged", new
            {
                messageId = eventData.MessageId,
                status = eventData.Status,
                timestamp = eventData.ChangedTime
            });

            _logger.LogInformation("已通过事件处理器通知用户 {ReceiverId} 消息 {MessageId} 状态变更为 {Status}", 
                eventData.ReceiverId, eventData.MessageId, eventData.Status);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "处理消息状态变更事件失败，消息ID: {MessageId}", eventData.MessageId);
            // 不抛出异常，避免事件总线重试
        }
    }
}
