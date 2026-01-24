using MessageCenter.Application.Contracts.Events;
using MessageCenter.Integration.Hubs;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using Volo.Abp.DependencyInjection;
using Volo.Abp.EventBus.Distributed;

namespace MessageCenter.Integration.EventHandlers;

/// <summary>
/// 消息创建事件处理器
/// 处理消息创建事件，执行实际的SignalR推送
/// 位于集成层，负责将业务事件转换为基础设施调用
/// </summary>
public class MessageCreatedEventHandler(
    IHubContext<MessageHub> hubContext,
    ILogger<MessageCreatedEventHandler> logger) : IDistributedEventHandler<MessageCreatedEvent>, ITransientDependency
{
    private readonly IHubContext<MessageHub> _hubContext = hubContext;
    private readonly ILogger<MessageCreatedEventHandler> _logger = logger;

    public async Task HandleEventAsync(MessageCreatedEvent eventData)
    {
        try
        {
            if (!eventData.ShouldPushRealtime)
            {
                return;
            }

            // 根据接收者类型决定推送方式
            if (eventData.ReceiverId == "broadcast")
            {
                // 广播
                await _hubContext.Clients.All.SendAsync("ReceiveMessage", eventData.Message);
                _logger.LogInformation("已通过事件处理器广播消息 {MessageId}", eventData.Message.Id);
            }
            else if (eventData.ReceiverId.StartsWith("group_") || 
                     eventData.ReceiverId.StartsWith("department_") ||
                     eventData.ReceiverId.StartsWith("business_"))
            {
                // 组播
                var groupName = eventData.ReceiverId;
                await _hubContext.Clients.Group(groupName).SendAsync("ReceiveMessage", eventData.Message);
                _logger.LogInformation("已通过事件处理器向组 {GroupName} 推送消息 {MessageId}", 
                    groupName, eventData.Message.Id);
            }
            else
            {
                // 点对点推送
                var groupName = $"user_{eventData.ReceiverId}";
                await _hubContext.Clients.Group(groupName).SendAsync("ReceiveMessage", eventData.Message);
                _logger.LogInformation("已通过事件处理器向用户 {ReceiverId} 推送消息 {MessageId}", 
                    eventData.ReceiverId, eventData.Message.Id);
            }
        }
        catch (System.Exception ex)
        {
            _logger.LogError(ex, "处理消息创建事件失败，消息ID: {MessageId}", eventData.Message?.Id);
            // 不抛出异常，避免事件总线重试
        }
    }
}
