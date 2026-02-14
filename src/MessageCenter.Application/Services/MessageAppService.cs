using MessageCenter.Application.Contracts.DTOs;
using MessageCenter.Application.Contracts.Events;
using MessageCenter.Application.Contracts.Services;
using MessageCenter.Domain.Entities;
using MessageCenter.Domain.Repositories;
using MessageCenter.Domain.Shared.Constants;
using MessageCenter.Domain.Shared.Enums;
using Microsoft.Extensions.Logging;
using System.Linq.Dynamic.Core;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.EventBus.Distributed;

namespace MessageCenter.Application.Services;

/// <summary>
/// 消息应用服务实现
/// </summary>
public class MessageAppService(
    IMessageRepository messageRepository,
    IRepository<MessageReceipt, Guid> receiptRepository,
    IMessageTemplateRepository templateRepository,
    IDistributedEventBus distributedEventBus,
    IMessageRealtimeService? realtimeService = null) : ApplicationService, IMessageAppService
{
    private readonly IMessageRepository _messageRepository = messageRepository;
    private readonly IRepository<MessageReceipt, Guid> _receiptRepository = receiptRepository;
    private readonly IMessageTemplateRepository _templateRepository = templateRepository;
    private readonly IDistributedEventBus _distributedEventBus = distributedEventBus;
    private readonly IMessageRealtimeService? _realtimeService = realtimeService;

    public virtual async Task<MessageDto> CreateAsync(CreateMessageDto input)
    {
        var message = await CreateMessageEntityAsync(input);
        await _messageRepository.InsertAsync(message);

        // 创建接收记录
        var receipt = new MessageReceipt(
            GuidGenerator.Create(),
            message.Id,
            message.ReceiverId);
        await _receiptRepository.InsertAsync(receipt);

        var messageDto = ObjectMapper.Map<Message, MessageDto>(message);

        // 实时推送消息（通过事件总线，解耦设计）
        if (_realtimeService != null && message.Channel == MessageChannel.InApp)
        {
            try
            {
                // 推送完整消息
                await _realtimeService.SendToUserAsync(message.ReceiverId, messageDto);
                
                // 同时发送轻量级通知（未读数量更新）
                var unreadCount = await _messageRepository.CountUnreadAsync(message.ReceiverId);
                await _realtimeService.NotifyNewMessageAsync(message.ReceiverId, unreadCount);
            }
            catch (Exception ex)
            {
                Logger.LogWarning(ex, "实时推送消息失败，消息ID: {MessageId}", message.Id);
                // 不抛出异常，避免影响主流程
            }
        }

        // TODO: 触发消息发送任务（邮件、短信等）

        return messageDto;
    }

    public virtual async Task<List<MessageDto>> CreateBatchAsync(List<CreateMessageDto> inputs)
    {
        if (inputs.Count > MessageCenterConsts.MaxBatchSendCount)
        {
            throw new ArgumentException($"批量发送数量不能超过 {MessageCenterConsts.MaxBatchSendCount}");
        }

        var messages = new List<Message>();
        var receipts = new List<MessageReceipt>();

        foreach (var input in inputs)
        {
            var message = await CreateMessageEntityAsync(input);
            messages.Add(message);

            var receipt = new MessageReceipt(
                GuidGenerator.Create(),
                message.Id,
                message.ReceiverId);
            receipts.Add(receipt);
        }

        await _messageRepository.InsertManyAsync(messages);
        await _receiptRepository.InsertManyAsync(receipts);

        var messageDtos = ObjectMapper.Map<List<Message>, List<MessageDto>>(messages);

        // 发布批量消息创建事件，供外部模块订阅
        try
        {
            var receiverIds = messageDtos.Select(m => m.ReceiverId).Distinct().ToList();
            var businessType = messageDtos.FirstOrDefault()?.BusinessType;
            
            await _distributedEventBus.PublishAsync(new MessageBatchCreatedEvent
            {
                Messages = messageDtos,
                ReceiverIds = receiverIds,
                ShouldPushRealtime = messageDtos.Any(m => m.Channel == MessageChannel.InApp),
                BusinessType = businessType
            });
        }
        catch (Exception ex)
        {
            Logger.LogWarning(ex, "发布批量消息创建事件失败");
        }

        // 实时推送消息（如果支持实时通信）
        if (_realtimeService != null)
        {
            try
            {
                // 按接收者分组推送
                var groupedMessages = messageDtos
                    .Where(m => m.Channel == MessageChannel.InApp)
                    .GroupBy(m => m.ReceiverId)
                    .ToList();

                foreach (var group in groupedMessages)
                {
                    var receiverId = group.Key;
                    var userMessages = group.ToList();
                    
                    // 推送消息列表
                    foreach (var messageDto in userMessages)
                    {
                        await _realtimeService.SendToUserAsync(receiverId, messageDto);
                    }

                    // 更新未读数量通知
                    var unreadCount = await _messageRepository.CountUnreadAsync(receiverId);
                    await _realtimeService.NotifyNewMessageAsync(receiverId, unreadCount);
                }
            }
            catch (Exception ex)
            {
                Logger.LogWarning(ex, "批量实时推送消息失败");
                // 不抛出异常，避免影响主流程
            }
        }

        // TODO: 触发批量消息发送任务（邮件、短信等）

        return messageDtos;
    }

    public virtual async Task<MessageDto> GetAsync(Guid id)
    {
        var message = await _messageRepository.GetAsync(id);
        var dto = ObjectMapper.Map<Message, MessageDto>(message);

        // 检查是否已读
        var receipt = await _receiptRepository.FirstOrDefaultAsync(r => r.MessageId == id && r.ReceiverId == dto.ReceiverId);
        dto.IsRead = receipt?.IsRead ?? false;

        return dto;
    }

    public virtual async Task<PagedResultDto<MessageDto>> GetListAsync(MessageQueryDto input)
    {
        var queryable = await _messageRepository.GetQueryableAsync();

        // 应用过滤条件
        if (!string.IsNullOrWhiteSpace(input.ReceiverId))
        {
            queryable = queryable.Where(m => m.ReceiverId == input.ReceiverId);
        }

        if (input.SenderId.HasValue)
        {
            queryable = queryable.Where(m => m.SenderId == input.SenderId);
        }

        if (input.MessageType.HasValue)
        {
            queryable = queryable.Where(m => m.MessageType == input.MessageType);
        }

        if (input.Channel.HasValue)
        {
            queryable = queryable.Where(m => m.Channel == input.Channel);
        }

        if (input.Status.HasValue)
        {
            queryable = queryable.Where(m => m.Status == input.Status);
        }

        if (input.Priority.HasValue)
        {
            queryable = queryable.Where(m => m.Priority == input.Priority);
        }

        if (!string.IsNullOrWhiteSpace(input.BusinessType))
        {
            queryable = queryable.Where(m => m.BusinessType == input.BusinessType);
        }

        if (!string.IsNullOrWhiteSpace(input.BusinessId))
        {
            queryable = queryable.Where(m => m.BusinessId == input.BusinessId);
        }

        if (!string.IsNullOrWhiteSpace(input.Keyword))
        {
            queryable = queryable.Where(m => m.Title.Contains(input.Keyword) || m.Content.Contains(input.Keyword));
        }

        if (input.StartTime.HasValue)
        {
            queryable = queryable.Where(m => m.CreationTime >= input.StartTime.Value);
        }

        if (input.EndTime.HasValue)
        {
            queryable = queryable.Where(m => m.CreationTime <= input.EndTime.Value);
        }

        if (!string.IsNullOrWhiteSpace(input.Tags))
        {
            queryable = queryable.Where(m => m.Tags != null && m.Tags.Contains(input.Tags));
        }

        // 应用排序
        if (string.IsNullOrWhiteSpace(input.Sorting))
        {
            // 默认按创建时间倒序
            queryable = queryable.OrderByDescending(m => m.CreationTime);
        }
        else
        {
            // 使用 System.Linq.Dynamic.Core 处理动态排序字符串
            // 例如: "CreationTime desc", "Title asc"
            try
            {
                queryable = queryable.OrderBy(input.Sorting);
            }
            catch (Exception ex)
            {
                Logger.LogWarning(ex, "动态排序失败，使用默认排序。排序字符串: {Sorting}", input.Sorting);
                queryable = queryable.OrderByDescending(m => m.CreationTime);
            }
        }

        var totalCount = await AsyncExecuter.CountAsync(queryable);
        var messages = await AsyncExecuter.ToListAsync(
            queryable.Skip(input.SkipCount).Take(input.MaxResultCount));

        var dtos = ObjectMapper.Map<List<Message>, List<MessageDto>>(messages);

        // 填充已读状态
        var messageIds = messages.Select(m => m.Id).ToList();
        var receipts = await _receiptRepository.GetListAsync(r => messageIds.Contains(r.MessageId));
        var receiptDict = receipts.ToDictionary(r => r.MessageId, r => r.IsRead);

        foreach (var dto in dtos)
        {
            dto.IsRead = receiptDict.GetValueOrDefault(dto.Id, false);
        }

        return new PagedResultDto<MessageDto>(totalCount, dtos);
    }

    public virtual async Task<PagedResultDto<MessageDto>> GetReceiverMessagesAsync(
        string receiverId,
        MessageQueryDto? input = null)
    {
        input ??= new MessageQueryDto();
        input.ReceiverId = receiverId;
        return await GetListAsync(input);
    }

    public virtual async Task MarkAsReadAsync(Guid id)
    {
        var message = await _messageRepository.GetAsync(id);
        var receipt = await _receiptRepository.FirstOrDefaultAsync(r => r.MessageId == id && r.ReceiverId == message.ReceiverId);

        if (receipt != null)
        {
            receipt.MarkAsRead(DateTime.UtcNow);
            await _receiptRepository.UpdateAsync(receipt);
        }

        var readTime = DateTime.UtcNow;
        message.MarkAsRead(readTime);
        await _messageRepository.UpdateAsync(message);

        // 发布消息已读事件，供外部模块订阅
        try
        {
            var messageDto = ObjectMapper.Map<Message, MessageDto>(message);
            await _distributedEventBus.PublishAsync(new MessageReadEvent
            {
                MessageId = message.Id,
                ReceiverId = message.ReceiverId,
                Message = messageDto,
                BusinessType = message.BusinessType,
                BusinessId = message.BusinessId,
                ReadTime = readTime
            });
        }
        catch (Exception ex)
        {
            Logger.LogWarning(ex, "发布消息已读事件失败，消息ID: {MessageId}", message.Id);
        }

        // 实时通知状态变更
        if (_realtimeService != null)
        {
            try
            {
                await _realtimeService.NotifyMessageStatusChangedAsync(
                    message.ReceiverId, 
                    message.Id, 
                    "Read");
            }
            catch (Exception ex)
            {
                Logger.LogWarning(ex, "通知消息状态变更失败");
            }
        }
    }

    public virtual async Task MarkAsReadBatchAsync(List<Guid> ids)
    {
        var messages = await _messageRepository.GetListAsync(m => ids.Contains(m.Id));
        var receipts = await _receiptRepository.GetListAsync(r => ids.Contains(r.MessageId));

        var now = DateTime.UtcNow;
        foreach (var message in messages)
        {
            var receipt = receipts.FirstOrDefault(r => r.MessageId == message.Id && r.ReceiverId == message.ReceiverId);
            receipt?.MarkAsRead(now);
            message.MarkAsRead(now);
        }

        await _receiptRepository.UpdateManyAsync(receipts);
        await _messageRepository.UpdateManyAsync(messages);
    }

    public virtual async Task MarkAllAsReadAsync(string receiverId)
    {
        var messages = await _messageRepository.GetListAsync(m => 
            m.ReceiverId == receiverId && 
            m.Status != MessageStatus.Read);
        
        var receipts = await _receiptRepository.GetListAsync(r => 
            r.ReceiverId == receiverId && 
            !r.IsRead);

        var now = DateTime.UtcNow;
        foreach (var message in messages)
        {
            message.MarkAsRead(now);
        }

        foreach (var receipt in receipts)
        {
            receipt.MarkAsRead(now);
        }

        await _messageRepository.UpdateManyAsync(messages);
        await _receiptRepository.UpdateManyAsync(receipts);
    }

    public virtual async Task DeleteAsync(Guid id)
    {
        // 获取消息信息，用于发布事件
        var message = await _messageRepository.FirstOrDefaultAsync(m => m.Id == id);
        
        await _messageRepository.DeleteAsync(id);
        // 同时删除接收记录
        var receipts = await _receiptRepository.GetListAsync(r => r.MessageId == id);
        await _receiptRepository.DeleteManyAsync(receipts);

        // 发布消息删除事件，供外部模块订阅
        if (message != null)
        {
            try
            {
                await _distributedEventBus.PublishAsync(new MessageDeletedEvent
                {
                    MessageId = id,
                    ReceiverId = message.ReceiverId,
                    BusinessType = message.BusinessType,
                    BusinessId = message.BusinessId,
                    DeletedTime = DateTime.UtcNow
                });
            }
            catch (Exception ex)
            {
                Logger.LogWarning(ex, "发布消息删除事件失败，消息ID: {MessageId}", id);
            }
        }
    }

    public virtual async Task DeleteBatchAsync(List<Guid> ids)
    {
        // 获取消息信息，用于发布事件
        var messages = await _messageRepository.GetListAsync(m => ids.Contains(m.Id));
        var deletedTime = DateTime.UtcNow;

        await _messageRepository.DeleteManyAsync(ids);
        var receipts = await _receiptRepository.GetListAsync(r => ids.Contains(r.MessageId));
        await _receiptRepository.DeleteManyAsync(receipts);

        // 发布批量消息删除事件
        foreach (var message in messages)
        {
            try
            {
                await _distributedEventBus.PublishAsync(new MessageDeletedEvent
                {
                    MessageId = message.Id,
                    ReceiverId = message.ReceiverId,
                    BusinessType = message.BusinessType,
                    BusinessId = message.BusinessId,
                    DeletedTime = deletedTime
                });
            }
            catch (Exception ex)
            {
                Logger.LogWarning(ex, "发布消息删除事件失败，消息ID: {MessageId}", message.Id);
            }
        }
    }

    public virtual async Task<long> GetUnreadCountAsync(string receiverId)
    {
        return await _messageRepository.CountUnreadAsync(receiverId);
    }

    public virtual async Task<MessageStatisticsDto> GetStatisticsAsync(
        string? receiverId = null,
        DateTime? startTime = null,
        DateTime? endTime = null)
    {
        var statistics = await _messageRepository.GetStatusStatisticsAsync(
            receiverId,
            null,
            startTime,
            endTime);

        var queryable = await _messageRepository.GetQueryableAsync();
        if (!string.IsNullOrWhiteSpace(receiverId))
        {
            queryable = queryable.Where(m => m.ReceiverId == receiverId);
        }

        if (startTime.HasValue)
        {
            queryable = queryable.Where(m => m.CreationTime >= startTime.Value);
        }

        if (endTime.HasValue)
        {
            queryable = queryable.Where(m => m.CreationTime <= endTime.Value);
        }

        var allMessages = await AsyncExecuter.ToListAsync(queryable);

        var result = new MessageStatisticsDto
        {
            TotalCount = allMessages.Count,
            UnreadCount = allMessages.Count(m => m.Status != MessageStatus.Read),
            ReadCount = allMessages.Count(m => m.Status == MessageStatus.Read),
            StatusStatistics = statistics,
            TypeStatistics = allMessages.GroupBy(m => m.MessageType)
                .ToDictionary(g => g.Key, g => (long)g.Count()),
            ChannelStatistics = allMessages.GroupBy(m => m.Channel)
                .ToDictionary(g => g.Key, g => (long)g.Count()),
            StartTime = startTime,
            EndTime = endTime
        };

        return result;
    }

    public virtual async Task RetryAsync(Guid id)
    {
        var message = await _messageRepository.GetAsync(id);
        if (!message.CanRetry())
        {
            throw new InvalidOperationException("消息不能重试");
        }

        message.Status = MessageStatus.Pending;
        await _messageRepository.UpdateAsync(message);

        // TODO: 触发消息发送任务；若再次失败应调用 MarkAsFailedAndPublishEventAsync
    }

    /// <summary>
    /// 将消息标记为发送失败并发布 <see cref="MessageFailedEvent"/>，供外部订阅告警/重试等。
    /// </summary>
    /// <remarks>
    /// <para><b>何时调用</b></para>
    /// <list type="bullet">
    /// <item>邮件/短信等发送任务执行失败，且不再重试时</item>
    /// <item>重试逻辑达到最大重试次数仍失败时（如 <see cref="RetryAsync"/> 触发的发送任务失败）</item>
    /// <item>外部通道（如第三方 API）返回不可重试错误时</item>
    /// </list>
    /// <para><b>调用方</b>：仅限本程序集（Application）内，例如发送服务在捕获到发送异常后调用。</para>
    /// <para><b>行为</b>：先将消息状态标记为失败并持久化，再发布 MessageFailedEvent；若发布事件失败仅记录警告日志，不抛出异常。</para>
    /// <para><b>外部订阅</b>：参见文档 MessageFailedEvent 的订阅方式与使用场景。</para>
    /// </remarks>
    /// <param name="messageId">要标记为失败的消息 ID</param>
    /// <param name="failureReason">失败原因描述，会写入消息实体并随事件发布，便于告警与排查</param>
    internal async Task MarkAsFailedAndPublishEventAsync(Guid messageId, string failureReason)
    {
        var message = await _messageRepository.GetAsync(messageId);
        message.MarkAsFailed(failureReason);
        await _messageRepository.UpdateAsync(message);

        try
        {
            await _distributedEventBus.PublishAsync(new MessageFailedEvent
            {
                MessageId = message.Id,
                ReceiverId = message.ReceiverId,
                FailureReason = failureReason,
                RetryCount = message.RetryCount,
                MaxRetryCount = message.MaxRetryCount,
                BusinessType = message.BusinessType,
                BusinessId = message.BusinessId,
                FailedTime = DateTime.UtcNow
            });
        }
        catch (Exception ex)
        {
            Logger.LogWarning(ex, "发布 MessageFailedEvent 失败，消息ID: {MessageId}", messageId);
        }
    }

    public virtual async Task CancelAsync(Guid id)
    {
        var message = await _messageRepository.GetAsync(id);
        if (message.Status == MessageStatus.Sent || message.Status == MessageStatus.Delivered || message.Status == MessageStatus.Read)
        {
            throw new InvalidOperationException("已发送的消息不能取消");
        }

        message.Status = MessageStatus.Cancelled;
        await _messageRepository.UpdateAsync(message);
    }

    private async Task<Message> CreateMessageEntityAsync(CreateMessageDto input)
    {
        var message = new Message(
            GuidGenerator.Create(),
            input.Title,
            input.Content,
            input.MessageType,
            input.Channel,
            input.ReceiverId ?? throw new ArgumentException("接收者ID不能为空"),
            input.Priority)
        {
            Summary = input.Summary,
            SenderId = input.SenderId,
            SenderName = input.SenderName,
            ReceiverName = input.ReceiverName,
            ReceiverEmail = input.ReceiverEmail,
            ReceiverPhone = input.ReceiverPhone,
            TemplateId = input.TemplateId,
            BusinessType = input.BusinessType,
            BusinessId = input.BusinessId,
            ScheduledSendTime = input.ScheduledSendTime?.ToUniversalTime(),
            ExpirationTime = (input.ExpirationTime?.ToUniversalTime()) ?? DateTime.UtcNow.AddDays(MessageCenterConsts.DefaultExpirationDays),
            Extension = input.Extension,
            Tags = input.Tags,
            LinkUrl = input.LinkUrl,
            MaxRetryCount = input.MaxRetryCount
        };

        // 处理附件
        if (input.AttachmentIds != null && input.AttachmentIds.Count > 0)
        {
            message.AttachmentIds = System.Text.Json.JsonSerializer.Serialize(input.AttachmentIds);
        }

        // 如果使用模板，渲染模板内容
        if (input.TemplateId.HasValue)
        {
            var template = await _templateRepository.GetAsync(input.TemplateId.Value);
            if (template != null && template.IsEnabled)
            {
                // TODO: 实现模板渲染逻辑
                // message.Content = RenderTemplate(template, input.TemplateVariables);
            }
        }

        return message;
    }
}
