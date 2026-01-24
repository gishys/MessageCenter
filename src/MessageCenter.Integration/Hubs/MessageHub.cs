using Microsoft.Extensions.Logging;
using Volo.Abp.AspNetCore.SignalR;

namespace MessageCenter.Integration.Hubs;

/// <summary>
/// 消息实时推送Hub
/// 提供实时消息推送功能，支持点对点和广播消息
/// 位于集成层，作为基础设施组件
/// </summary>
public class MessageHub(ILogger<MessageHub> logger) : AbpHub
{
    private readonly ILogger<MessageHub> _logger = logger;

    /// <summary>
    /// 客户端连接时触发
    /// </summary>
    public override async Task OnConnectedAsync()
    {
        var userId = GetUserId();
        if (!string.IsNullOrEmpty(userId))
        {
            // 将用户添加到对应的组
            await Groups.AddToGroupAsync(Context.ConnectionId, $"user_{userId}");
            _logger.LogInformation("用户 {UserId} 已连接到消息Hub，连接ID: {ConnectionId}", userId, Context.ConnectionId);
        }

        await base.OnConnectedAsync();
    }

    /// <summary>
    /// 客户端断开连接时触发
    /// </summary>
    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        var userId = GetUserId();
        if (!string.IsNullOrEmpty(userId))
        {
            // 从组中移除用户
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, $"user_{userId}");
            _logger.LogInformation("用户 {UserId} 已断开消息Hub连接，连接ID: {ConnectionId}", userId, Context.ConnectionId);
        }

        if (exception != null)
        {
            _logger.LogError(exception, "用户 {UserId} 断开连接时发生错误", userId);
        }

        await base.OnDisconnectedAsync(exception);
    }

    /// <summary>
    /// 客户端加入指定组（如加入特定业务组）
    /// </summary>
    public async Task JoinGroup(string groupName)
    {
        await Groups.AddToGroupAsync(Context.ConnectionId, groupName);
        _logger.LogInformation("连接 {ConnectionId} 已加入组 {GroupName}", Context.ConnectionId, groupName);
    }

    /// <summary>
    /// 客户端离开指定组
    /// </summary>
    public async Task LeaveGroup(string groupName)
    {
        await Groups.RemoveFromGroupAsync(Context.ConnectionId, groupName);
        _logger.LogInformation("连接 {ConnectionId} 已离开组 {GroupName}", Context.ConnectionId, groupName);
    }

    /// <summary>
    /// 获取当前用户ID
    /// </summary>
    private string? GetUserId()
    {
        return Context.User?.FindFirst("sub")?.Value 
            ?? Context.User?.FindFirst("nameid")?.Value
            ?? Context.UserIdentifier;
    }
}
