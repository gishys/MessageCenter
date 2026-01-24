namespace MessageCenter.Application.Contracts.Events;

/// <summary>
/// 未读数量变更事件
/// 当用户未读消息数量变更时发布此事件
/// </summary>
public class UnreadCountChangedEvent
{
    public string ReceiverId { get; set; } = string.Empty;
    public long UnreadCount { get; set; }
}
