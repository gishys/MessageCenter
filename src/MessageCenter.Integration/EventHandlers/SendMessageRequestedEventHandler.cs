using MessageCenter.Application.Contracts.Events;
using MessageCenter.Application.Contracts.Services;
using Microsoft.Extensions.Logging;
using Volo.Abp.DependencyInjection;
using Volo.Abp.EventBus.Distributed;

namespace MessageCenter.Integration.EventHandlers;

/// <summary>
/// 发消息请求事件处理器
/// 订阅外部服务发布的 SendMessageRequestedEvent，调用 IMessageAppService.CreateAsync 执行发消息；
/// 创建后现有逻辑会继续发布 MessageCreatedEvent 等，SignalR 与其它订阅者行为不变
/// </summary>
public class SendMessageRequestedEventHandler(
    IMessageAppService messageAppService,
    ILogger<SendMessageRequestedEventHandler> logger)
    : IDistributedEventHandler<SendMessageRequestedEvent>, ITransientDependency
{
    private readonly IMessageAppService _messageAppService = messageAppService;
    private readonly ILogger<SendMessageRequestedEventHandler> _logger = logger;

    public async Task HandleEventAsync(SendMessageRequestedEvent eventData)
    {
        if (eventData?.CreateMessageDto == null)
        {
            _logger.LogWarning("SendMessageRequestedEvent 收到空事件或 CreateMessageDto 为空，已忽略");
            return;
        }

        try
        {
            // 可选：若存在 RequestId，可在此做幂等（如分布式缓存“已处理 RequestId”），避免重复创建消息
            // 当前未实现分布式幂等，由调用方保证或后续扩展

            await _messageAppService.CreateAsync(eventData.CreateMessageDto);

            _logger.LogInformation(
                "已通过事件完成发消息请求，RequestId: {RequestId}, SourceService: {SourceService}, ReceiverId: {ReceiverId}",
                eventData.RequestId,
                eventData.SourceService,
                eventData.CreateMessageDto.ReceiverId ?? "(batch)");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex,
                "处理发消息请求事件失败，RequestId: {RequestId}, SourceService: {SourceService}, Title: {Title}",
                eventData.RequestId,
                eventData.SourceService,
                eventData.CreateMessageDto.Title);
            // 不重新抛出，避免事件总线无限重试影响其它订阅者
        }
    }
}
