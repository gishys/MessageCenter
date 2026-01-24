using MessageCenter.Application.Contracts.DTOs;
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

namespace MessageCenter.Application.Services;

/// <summary>
/// 消息应用服务实现
/// </summary>
public class MessageAppService(
    IMessageRepository messageRepository,
    IRepository<MessageReceipt, Guid> receiptRepository,
    IMessageTemplateRepository templateRepository,
    IMessageRealtimeService? realtimeService = null) : ApplicationService, IMessageAppService
{
    private readonly IMessageRepository _messageRepository = messageRepository;
    private readonly IRepository<MessageReceipt, Guid> _receiptRepository = receiptRepository;
    private readonly IMessageTemplateRepository _templateRepository = templateRepository;
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

        message.MarkAsRead(DateTime.UtcNow);
        await _messageRepository.UpdateAsync(message);

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
            if (receipt != null)
            {
                receipt.MarkAsRead(now);
            }
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
        await _messageRepository.DeleteAsync(id);
        // 同时删除接收记录
        var receipts = await _receiptRepository.GetListAsync(r => r.MessageId == id);
        await _receiptRepository.DeleteManyAsync(receipts);
    }

    public virtual async Task DeleteBatchAsync(List<Guid> ids)
    {
        await _messageRepository.DeleteManyAsync(ids);
        var receipts = await _receiptRepository.GetListAsync(r => ids.Contains(r.MessageId));
        await _receiptRepository.DeleteManyAsync(receipts);
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

        // TODO: 触发消息发送任务
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
