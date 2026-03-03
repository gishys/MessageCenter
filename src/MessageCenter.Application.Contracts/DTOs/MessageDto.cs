using MessageCenter.Domain.Shared.Enums;
using Volo.Abp.Application.Dtos;

namespace MessageCenter.Application.Contracts.DTOs;

/// <summary>
/// 消息DTO
/// </summary>
public class MessageDto : FullAuditedEntityDto<Guid>
{
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
    /// 接收者邮箱
    /// </summary>
    public string? ReceiverEmail { get; set; }

    /// <summary>
    /// 接收者手机号
    /// </summary>
    public string? ReceiverPhone { get; set; }

    /// <summary>
    /// 模板ID
    /// </summary>
    public Guid? TemplateId { get; set; }

    /// <summary>
    /// 会话ID（用于按会话聚合消息）
    /// </summary>
    public Guid? ConversationId { get; set; }

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
    /// 扩展属性
    /// </summary>
    public string? Extension { get; set; }

    /// <summary>
    /// 消息体（块列表 JSON），用于前端富展示
    /// </summary>
    public string? Body { get; set; }

    /// <summary>
    /// 消息标签
    /// </summary>
    public string? Tags { get; set; }

    /// <summary>
    /// 消息链接
    /// </summary>
    public string? LinkUrl { get; set; }

    /// <summary>
    /// 消息附件ID列表
    /// </summary>
    public string? AttachmentIds { get; set; }

    /// <summary>
    /// 是否已读
    /// </summary>
    public bool IsRead { get; set; }
}
