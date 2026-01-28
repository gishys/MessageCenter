using MessageCenter.Application.Contracts.DTOs;

namespace MessageCenter.Application.Contracts.Events;

/// <summary>
/// 批量消息创建事件
/// 当批量创建消息时发布此事件，供外部模块订阅以执行相关业务逻辑
/// </summary>
public class MessageBatchCreatedEvent
{
    /// <summary>
    /// 创建的消息列表
    /// </summary>
    public List<MessageDto> Messages { get; set; } = [];

    /// <summary>
    /// 接收者ID列表
    /// </summary>
    public List<string> ReceiverIds { get; set; } = [];

    /// <summary>
    /// 是否应该实时推送
    /// </summary>
    public bool ShouldPushRealtime { get; set; }

    /// <summary>
    /// 业务类型
    /// </summary>
    public string? BusinessType { get; set; }
}
