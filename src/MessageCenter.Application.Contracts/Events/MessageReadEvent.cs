using MessageCenter.Application.Contracts.DTOs;

namespace MessageCenter.Application.Contracts.Events;

/// <summary>
/// 消息已读事件
/// 当消息被标记为已读时发布此事件，供外部模块订阅以执行相关业务逻辑
/// </summary>
public class MessageReadEvent
{
    /// <summary>
    /// 消息ID
    /// </summary>
    public Guid MessageId { get; set; }

    /// <summary>
    /// 接收者ID
    /// </summary>
    public string ReceiverId { get; set; } = string.Empty;

    /// <summary>
    /// 消息信息
    /// </summary>
    public MessageDto? Message { get; set; }

    /// <summary>
    /// 业务类型
    /// </summary>
    public string? BusinessType { get; set; }

    /// <summary>
    /// 业务ID
    /// </summary>
    public string? BusinessId { get; set; }

    /// <summary>
    /// 阅读时间
    /// </summary>
    public DateTime ReadTime { get; set; }
}
