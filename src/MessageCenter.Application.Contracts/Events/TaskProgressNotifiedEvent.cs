namespace MessageCenter.Application.Contracts.Events;

/// <summary>
/// 当需要向客户端推送“任务进度”时发布此事件（与消息业务解耦）。
/// 由集成层订阅并通过 SignalR 推送到对应用户组。
/// </summary>
public class TaskProgressNotifiedEvent
{
    public string ReceiverId { get; set; } = string.Empty;
    public Guid TaskId { get; set; }
    public int Progress { get; set; }
    public string? Message { get; set; }
    public string? Status { get; set; }
}

