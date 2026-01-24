using MessageCenter.Application.Contracts.DTOs;

namespace MessageCenter.Application.Contracts.Events;

/// <summary>
/// 消息创建事件
/// 当消息创建时发布此事件，用于触发实时推送等操作
/// </summary>
public class MessageCreatedEvent
{
    public MessageDto Message { get; set; } = null!;
    public string ReceiverId { get; set; } = string.Empty;
    public bool ShouldPushRealtime { get; set; }
}
