using MessageCenter.Application.Contracts.DTOs;

namespace MessageCenter.Application.Contracts.Services;

/// <summary>
/// 实时消息推送服务接口
/// </summary>
public interface IMessageRealtimeService
{
    /// <summary>
    /// 向指定用户推送消息
    /// </summary>
    /// <param name="receiverId">接收者ID</param>
    /// <param name="message">消息DTO</param>
    Task SendToUserAsync(string receiverId, MessageDto message);

    /// <summary>
    /// 向多个用户推送消息
    /// </summary>
    /// <param name="receiverIds">接收者ID列表</param>
    /// <param name="message">消息DTO</param>
    Task SendToUsersAsync(List<string> receiverIds, MessageDto message);

    /// <summary>
    /// 向指定组推送消息（如业务组、部门组等）
    /// </summary>
    /// <param name="groupName">组名称</param>
    /// <param name="message">消息DTO</param>
    Task SendToGroupAsync(string groupName, MessageDto message);

    /// <summary>
    /// 广播消息给所有连接的客户端
    /// </summary>
    /// <param name="message">消息DTO</param>
    Task BroadcastAsync(MessageDto message);

    /// <summary>
    /// 通知用户有新消息（轻量级通知，不包含完整消息内容）
    /// </summary>
    /// <param name="receiverId">接收者ID</param>
    /// <param name="unreadCount">未读消息数量</param>
    Task NotifyNewMessageAsync(string receiverId, long unreadCount);

    /// <summary>
    /// 通知用户消息状态变更
    /// </summary>
    /// <param name="receiverId">接收者ID</param>
    /// <param name="messageId">消息ID</param>
    /// <param name="status">新状态</param>
    Task NotifyMessageStatusChangedAsync(string receiverId, Guid messageId, string status);
}
