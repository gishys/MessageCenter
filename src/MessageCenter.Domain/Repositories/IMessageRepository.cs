using MessageCenter.Domain.Entities;
using MessageCenter.Domain.Shared.Enums;
using Volo.Abp.Domain.Repositories;

namespace MessageCenter.Domain.Repositories;

/// <summary>
/// 消息仓储接口
/// </summary>
public interface IMessageRepository : IRepository<Message, Guid>
{
    /// <summary>
    /// 根据接收者ID获取消息列表
    /// </summary>
    Task<List<Message>> GetByReceiverIdAsync(
        string receiverId,
        MessageStatus? status = null,
        MessageType? messageType = null,
        int skipCount = 0,
        int maxResultCount = 10,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 根据发送者ID获取消息列表
    /// </summary>
    Task<List<Message>> GetBySenderIdAsync(
        Guid senderId,
        MessageStatus? status = null,
        int skipCount = 0,
        int maxResultCount = 10,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 获取待发送的消息列表
    /// </summary>
    Task<List<Message>> GetPendingMessagesAsync(
        MessageChannel? channel = null,
        int maxResultCount = 100,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 获取需要重试的消息列表
    /// </summary>
    Task<List<Message>> GetRetryMessagesAsync(
        int maxResultCount = 100,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 根据业务类型和业务ID获取消息
    /// </summary>
    Task<Message?> GetByBusinessAsync(
        string businessType,
        string businessId,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 统计未读消息数量
    /// </summary>
    Task<long> CountUnreadAsync(
        string receiverId,
        MessageType? messageType = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 获取消息统计信息
    /// </summary>
    Task<Dictionary<MessageStatus, long>> GetStatusStatisticsAsync(
        string? receiverId = null,
        MessageType? messageType = null,
        DateTime? startTime = null,
        DateTime? endTime = null,
        CancellationToken cancellationToken = default);
}
