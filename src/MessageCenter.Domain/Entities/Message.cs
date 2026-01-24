using MessageCenter.Domain.Shared.Enums;
using Volo.Abp.Domain.Entities.Auditing;
using Volo.Abp.MultiTenancy;

namespace MessageCenter.Domain.Entities;

/// <summary>
/// 消息实体
/// 核心消息实体，支持多种消息类型和渠道
/// </summary>
public class Message : FullAuditedAggregateRoot<Guid>, IMultiTenant
{
    /// <summary>
    /// 租户ID
    /// </summary>
    public Guid? TenantId { get; set; }

    /// <summary>
    /// 消息标题
    /// </summary>
    public string Title { get; set; } = string.Empty;

    /// <summary>
    /// 消息内容
    /// </summary>
    public string Content { get; set; } = string.Empty;

    /// <summary>
    /// 消息摘要
    /// </summary>
    public string? Summary { get; set; }

    /// <summary>
    /// 消息类型
    /// </summary>
    public MessageType MessageType { get; set; }

    /// <summary>
    /// 消息渠道
    /// </summary>
    public MessageChannel Channel { get; set; }

    /// <summary>
    /// 消息状态
    /// </summary>
    public MessageStatus Status { get; set; }

    /// <summary>
    /// 消息优先级
    /// </summary>
    public MessagePriority Priority { get; set; }

    /// <summary>
    /// 发送者ID
    /// </summary>
    public Guid? SenderId { get; set; }

    /// <summary>
    /// 发送者名称
    /// </summary>
    public string? SenderName { get; set; }

    /// <summary>
    /// 接收者ID
    /// </summary>
    public string ReceiverId { get; set; } = string.Empty;

    /// <summary>
    /// 接收者名称
    /// </summary>
    public string? ReceiverName { get; set; }

    /// <summary>
    /// 接收者邮箱（用于邮件渠道）
    /// </summary>
    public string? ReceiverEmail { get; set; }

    /// <summary>
    /// 接收者手机号（用于短信渠道）
    /// </summary>
    public string? ReceiverPhone { get; set; }

    /// <summary>
    /// 模板ID
    /// </summary>
    public Guid? TemplateId { get; set; }

    /// <summary>
    /// 业务类型
    /// </summary>
    public string? BusinessType { get; set; }

    /// <summary>
    /// 业务ID
    /// </summary>
    public string? BusinessId { get; set; }

    /// <summary>
    /// 计划发送时间
    /// </summary>
    public DateTime? ScheduledSendTime { get; set; }

    /// <summary>
    /// 实际发送时间
    /// </summary>
    public DateTime? ActualSendTime { get; set; }

    /// <summary>
    /// 送达时间
    /// </summary>
    public DateTime? DeliveredTime { get; set; }

    /// <summary>
    /// 阅读时间
    /// </summary>
    public DateTime? ReadTime { get; set; }

    /// <summary>
    /// 过期时间
    /// </summary>
    public DateTime? ExpirationTime { get; set; }

    /// <summary>
    /// 重试次数
    /// </summary>
    public int RetryCount { get; set; }

    /// <summary>
    /// 最大重试次数
    /// </summary>
    public int MaxRetryCount { get; set; }

    /// <summary>
    /// 失败原因
    /// </summary>
    public string? FailureReason { get; set; }

    /// <summary>
    /// 扩展属性（JSON格式）
    /// </summary>
    public string? Extension { get; set; }

    /// <summary>
    /// 消息标签（用于分类和搜索）
    /// </summary>
    public string? Tags { get; set; }

    /// <summary>
    /// 消息链接（用于跳转）
    /// </summary>
    public string? LinkUrl { get; set; }

    /// <summary>
    /// 消息附件ID列表（JSON格式）
    /// </summary>
    public string? AttachmentIds { get; set; }

    /// <summary>
    /// 消息接收记录集合
    /// </summary>
    public virtual ICollection<MessageReceipt> Receipts { get; set; } = [];

    protected Message()
    {
    }

    public Message(
        Guid id,
        string title,
        string content,
        MessageType messageType,
        MessageChannel channel,
        string receiverId,
        MessagePriority priority = MessagePriority.Normal)
        : base(id)
    {
        Title = title;
        Content = content;
        MessageType = messageType;
        Channel = channel;
        ReceiverId = receiverId;
        Priority = priority;
        Status = MessageStatus.Pending;
        MaxRetryCount = 3;
    }

    /// <summary>
    /// 标记为发送中
    /// </summary>
    public void MarkAsSending()
    {
        Status = MessageStatus.Sending;
    }

    /// <summary>
    /// 标记为已发送
    /// </summary>
    public void MarkAsSent(DateTime sendTime)
    {
        Status = MessageStatus.Sent;
        ActualSendTime = sendTime;
    }

    /// <summary>
    /// 标记为已送达
    /// </summary>
    public void MarkAsDelivered(DateTime deliveredTime)
    {
        Status = MessageStatus.Delivered;
        DeliveredTime = deliveredTime;
    }

    /// <summary>
    /// 标记为已读
    /// </summary>
    public void MarkAsRead(DateTime readTime)
    {
        Status = MessageStatus.Read;
        ReadTime = readTime;
    }

    /// <summary>
    /// 标记为发送失败
    /// </summary>
    public void MarkAsFailed(string reason)
    {
        Status = MessageStatus.Failed;
        FailureReason = reason;
        RetryCount++;
    }

    /// <summary>
    /// 是否可以重试
    /// </summary>
    public bool CanRetry()
    {
        return Status == MessageStatus.Failed && RetryCount < MaxRetryCount;
    }

    /// <summary>
    /// 检查是否过期
    /// </summary>
    public bool IsExpired()
    {
        return ExpirationTime.HasValue && ExpirationTime.Value < DateTime.UtcNow;
    }
}
