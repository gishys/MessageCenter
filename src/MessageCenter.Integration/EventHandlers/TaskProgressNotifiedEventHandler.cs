using MessageCenter.Application.Contracts.Events;
using MessageCenter.Integration.Hubs;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using Volo.Abp.DependencyInjection;
using Volo.Abp.EventBus.Distributed;

namespace MessageCenter.Integration.EventHandlers;

/// <summary>
/// 任务进度事件处理器：将领域/应用层事件转换为 SignalR 推送。
/// </summary>
public class TaskProgressNotifiedEventHandler(
    IHubContext<MessageHub> hubContext,
    ILogger<TaskProgressNotifiedEventHandler> logger)
    : IDistributedEventHandler<TaskProgressNotifiedEvent>, ITransientDependency
{
    private readonly IHubContext<MessageHub> _hubContext = hubContext;
    private readonly ILogger<TaskProgressNotifiedEventHandler> _logger = logger;

    public async Task HandleEventAsync(TaskProgressNotifiedEvent eventData)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(eventData.ReceiverId))
            {
                _logger.LogWarning("接收者ID为空，跳过任务进度推送");
                return;
            }

            var groupName = $"user_{eventData.ReceiverId}";
            await _hubContext.Clients.Group(groupName).SendAsync("TaskProgress", new
            {
                taskId = eventData.TaskId,
                progress = eventData.Progress,
                message = eventData.Message,
                status = eventData.Status
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "任务进度推送失败，ReceiverId: {ReceiverId}, TaskId: {TaskId}", eventData.ReceiverId, eventData.TaskId);
        }
    }
}

