using MessageCenter.Application.Contracts.DTOs;

namespace MessageCenter.Application.Contracts.Events;

/// <summary>
/// 发消息请求事件
/// 由外部服务发布此事件，请求 MessageCenter 发送一条消息（不直接调用 API/应用服务），实现与调用方解耦
/// </summary>
public class SendMessageRequestedEvent
{
    /// <summary>
    /// 发消息所需数据（与现有创建接口一致）
    /// </summary>
    public CreateMessageDto CreateMessageDto { get; set; } = null!;

    /// <summary>
    /// 幂等键，便于 MessageCenter 或调用方去重，避免重复创建消息
    /// </summary>
    public string? RequestId { get; set; }

    /// <summary>
    /// 来源服务名，便于排查与审计
    /// </summary>
    public string? SourceService { get; set; }
}
