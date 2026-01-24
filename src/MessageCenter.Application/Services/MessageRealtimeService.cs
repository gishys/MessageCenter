using MessageCenter.Application.Contracts.DTOs;
using MessageCenter.Application.Contracts.Events;
using MessageCenter.Application.Contracts.Services;
using MessageCenter.Domain.Shared.Enums;
using Microsoft.Extensions.Logging;
using Volo.Abp.DependencyInjection;
using Volo.Abp.EventBus.Distributed;

namespace MessageCenter.Application.Services;

/// <summary>
/// 实时消息推送服务实现（Application层）
/// 
/// 设计说明：
/// 1. Application层不直接依赖SignalR（避免层间耦合）
/// 2. 使用事件总线模式解耦，通过发布事件触发实时推送
/// 3. HttpApi层的事件处理器负责实际的SignalR推送
/// 4. 提供完整的业务逻辑验证和参数校验
/// 5. 实现Null Object Pattern，确保即使HttpApi层未注册也能正常工作
/// </summary>
public class MessageRealtimeService(
    IDistributedEventBus distributedEventBus,
    ILogger<MessageRealtimeService> logger) : IMessageRealtimeService, ITransientDependency
{
    private readonly IDistributedEventBus _distributedEventBus = distributedEventBus;
    private readonly ILogger<MessageRealtimeService> _logger = logger;

    /// <summary>
    /// 向指定用户推送消息
    /// </summary>
    public virtual async Task SendToUserAsync(string receiverId, MessageDto message)
    {
        try
        {
            // 参数验证
            if (string.IsNullOrWhiteSpace(receiverId))
            {
                _logger.LogWarning("接收者ID为空，跳过实时推送");
                return;
            }

            if (message == null)
            {
                _logger.LogWarning("消息对象为空，跳过实时推送");
                return;
            }

            // 业务规则：只有站内信才实时推送
            if (message.Channel != MessageChannel.InApp)
            {
                _logger.LogDebug("消息渠道 {Channel} 不需要实时推送，跳过", message.Channel);
                return;
            }

            // 发布事件，由HttpApi层的事件处理器处理实际推送
            await _distributedEventBus.PublishAsync(new MessageCreatedEvent
            {
                Message = message,
                ReceiverId = receiverId,
                ShouldPushRealtime = true
            });

            _logger.LogInformation("已发布消息创建事件，接收者: {ReceiverId}, 消息ID: {MessageId}", 
                receiverId, message.Id);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "发布消息创建事件失败，接收者: {ReceiverId}, 消息ID: {MessageId}", 
                receiverId, message?.Id);
            // 不抛出异常，避免影响主流程
        }
    }

    /// <summary>
    /// 向多个用户推送消息
    /// </summary>
    public virtual async Task SendToUsersAsync(List<string> receiverIds, MessageDto message)
    {
        try
        {
            // 参数验证
            if (receiverIds == null || receiverIds.Count == 0)
            {
                _logger.LogWarning("接收者ID列表为空，跳过批量实时推送");
                return;
            }

            if (message == null)
            {
                _logger.LogWarning("消息对象为空，跳过批量实时推送");
                return;
            }

            // 业务规则：只有站内信才实时推送
            if (message.Channel != MessageChannel.InApp)
            {
                _logger.LogDebug("消息渠道 {Channel} 不需要实时推送，跳过批量推送", message.Channel);
                return;
            }

            // 过滤空值
            var validReceiverIds = receiverIds.Where(id => !string.IsNullOrWhiteSpace(id)).ToList();
            if (validReceiverIds.Count == 0)
            {
                _logger.LogWarning("有效的接收者ID列表为空，跳过批量实时推送");
                return;
            }

            // 批量发布事件
            var tasks = validReceiverIds.Select(receiverId =>
                _distributedEventBus.PublishAsync(new MessageCreatedEvent
                {
                    Message = message,
                    ReceiverId = receiverId,
                    ShouldPushRealtime = true
                }));

            await Task.WhenAll(tasks);

            _logger.LogInformation("已批量发布消息创建事件，接收者数量: {Count}, 消息ID: {MessageId}", 
                validReceiverIds.Count, message.Id);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "批量发布消息创建事件失败，消息ID: {MessageId}", message?.Id);
            // 不抛出异常，避免影响主流程
        }
    }

    /// <summary>
    /// 向指定组推送消息
    /// </summary>
    public virtual async Task SendToGroupAsync(string groupName, MessageDto message)
    {
        try
        {
            // 参数验证
            if (string.IsNullOrWhiteSpace(groupName))
            {
                _logger.LogWarning("组名称为空，跳过组播推送");
                return;
            }

            if (message == null)
            {
                _logger.LogWarning("消息对象为空，跳过组播推送");
                return;
            }

            // 业务规则：只有站内信才实时推送
            if (message.Channel != MessageChannel.InApp)
            {
                _logger.LogDebug("消息渠道 {Channel} 不需要实时推送，跳过组播", message.Channel);
                return;
            }

            // 发布事件，由HttpApi层的事件处理器处理实际推送
            await _distributedEventBus.PublishAsync(new MessageCreatedEvent
            {
                Message = message,
                ReceiverId = groupName, // 使用组名作为接收者标识
                ShouldPushRealtime = true
            });

            _logger.LogInformation("已发布组播消息事件，组名称: {GroupName}, 消息ID: {MessageId}", 
                groupName, message.Id);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "发布组播消息事件失败，组名称: {GroupName}, 消息ID: {MessageId}", 
                groupName, message?.Id);
            // 不抛出异常，避免影响主流程
        }
    }

    /// <summary>
    /// 广播消息给所有连接的客户端
    /// </summary>
    public virtual async Task BroadcastAsync(MessageDto message)
    {
        try
        {
            // 参数验证
            if (message == null)
            {
                _logger.LogWarning("消息对象为空，跳过广播推送");
                return;
            }

            // 业务规则：只有站内信才实时推送
            if (message.Channel != MessageChannel.InApp)
            {
                _logger.LogDebug("消息渠道 {Channel} 不需要实时推送，跳过广播", message.Channel);
                return;
            }

            // 发布事件，由HttpApi层的事件处理器处理实际推送
            await _distributedEventBus.PublishAsync(new MessageCreatedEvent
            {
                Message = message,
                ReceiverId = "broadcast", // 使用特殊标识表示广播
                ShouldPushRealtime = true
            });

            _logger.LogInformation("已发布广播消息事件，消息ID: {MessageId}", message.Id);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "发布广播消息事件失败，消息ID: {MessageId}", message?.Id);
            // 不抛出异常，避免影响主流程
        }
    }

    /// <summary>
    /// 通知用户有新消息（轻量级通知）
    /// </summary>
    public virtual async Task NotifyNewMessageAsync(string receiverId, long unreadCount)
    {
        try
        {
            // 参数验证
            if (string.IsNullOrWhiteSpace(receiverId))
            {
                _logger.LogWarning("接收者ID为空，跳过新消息通知");
                return;
            }

            if (unreadCount < 0)
            {
                _logger.LogWarning("未读数量无效: {UnreadCount}，跳过新消息通知", unreadCount);
                return;
            }

            // 发布未读数量变更事件
            await _distributedEventBus.PublishAsync(new UnreadCountChangedEvent
            {
                ReceiverId = receiverId,
                UnreadCount = unreadCount
            });

            _logger.LogInformation("已发布未读数量变更事件，接收者: {ReceiverId}, 未读数量: {UnreadCount}", 
                receiverId, unreadCount);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "发布未读数量变更事件失败，接收者: {ReceiverId}", receiverId);
            // 不抛出异常，避免影响主流程
        }
    }

    /// <summary>
    /// 通知用户消息状态变更
    /// </summary>
    public virtual async Task NotifyMessageStatusChangedAsync(string receiverId, Guid messageId, string status)
    {
        try
        {
            // 参数验证
            if (string.IsNullOrWhiteSpace(receiverId))
            {
                _logger.LogWarning("接收者ID为空，跳过状态变更通知");
                return;
            }

            if (messageId == Guid.Empty)
            {
                _logger.LogWarning("消息ID无效，跳过状态变更通知");
                return;
            }

            if (string.IsNullOrWhiteSpace(status))
            {
                _logger.LogWarning("状态为空，跳过状态变更通知");
                return;
            }

            // 发布消息状态变更事件
            await _distributedEventBus.PublishAsync(new MessageStatusChangedEvent
            {
                MessageId = messageId,
                ReceiverId = receiverId,
                Status = status,
                ChangedTime = DateTime.UtcNow
            });

            _logger.LogInformation("已发布消息状态变更事件，接收者: {ReceiverId}, 消息ID: {MessageId}, 状态: {Status}", 
                receiverId, messageId, status);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "发布消息状态变更事件失败，接收者: {ReceiverId}, 消息ID: {MessageId}", 
                receiverId, messageId);
            // 不抛出异常，避免影响主流程
        }
    }
}
