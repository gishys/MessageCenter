namespace MessageCenter.Application.Contracts.Events;

/// <summary>
/// 消息状态变更事件
/// 当消息状态变更时发布此事件
/// </summary>
public class MessageStatusChangedEvent
{
    public Guid MessageId { get; set; }
    public string ReceiverId { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public DateTime ChangedTime { get; set; }
}
