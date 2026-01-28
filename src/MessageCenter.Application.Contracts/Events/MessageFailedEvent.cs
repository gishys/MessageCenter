namespace MessageCenter.Application.Contracts.Events;

/// <summary>
/// 消息发送失败事件
/// 当消息发送失败时发布此事件，供外部模块订阅以执行重试或告警等操作
/// </summary>
public class MessageFailedEvent
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
    /// 失败原因
    /// </summary>
    public string FailureReason { get; set; } = string.Empty;

    /// <summary>
    /// 重试次数
    /// </summary>
    public int RetryCount { get; set; }

    /// <summary>
    /// 最大重试次数
    /// </summary>
    public int MaxRetryCount { get; set; }

    /// <summary>
    /// 业务类型
    /// </summary>
    public string? BusinessType { get; set; }

    /// <summary>
    /// 业务ID
    /// </summary>
    public string? BusinessId { get; set; }

    /// <summary>
    /// 失败时间
    /// </summary>
    public DateTime FailedTime { get; set; }
}
