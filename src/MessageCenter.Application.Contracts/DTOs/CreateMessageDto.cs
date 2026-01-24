using MessageCenter.Domain.Shared.Enums;

namespace MessageCenter.Application.Contracts.DTOs;

/// <summary>
/// 创建消息DTO
/// </summary>
public class CreateMessageDto
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
    /// 消息优先级
    /// </summary>
    public MessagePriority Priority { get; set; } = MessagePriority.Normal;

    /// <summary>
    /// 发送者ID
    /// </summary>
    public Guid? SenderId { get; set; }

    /// <summary>
    /// 发送者名称
    /// </summary>
    public string? SenderName { get; set; }

    /// <summary>
    /// 接收者ID（单个）
    /// </summary>
    public string? ReceiverId { get; set; }

    /// <summary>
    /// 接收者ID列表（批量）
    /// </summary>
    public List<string>? ReceiverIds { get; set; }

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
    /// 模板变量（JSON格式）
    /// </summary>
    public string? TemplateVariables { get; set; }

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
    /// 过期时间
    /// </summary>
    public DateTime? ExpirationTime { get; set; }

    /// <summary>
    /// 扩展属性（JSON格式）
    /// </summary>
    public string? Extension { get; set; }

    /// <summary>
    /// 消息标签
    /// </summary>
    public string? Tags { get; set; }

    /// <summary>
    /// 消息链接
    /// </summary>
    public string? LinkUrl { get; set; }

    /// <summary>
    /// 附件ID列表
    /// </summary>
    public List<Guid>? AttachmentIds { get; set; }

    /// <summary>
    /// 最大重试次数
    /// </summary>
    public int MaxRetryCount { get; set; } = 3;
}
