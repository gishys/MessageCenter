using MessageCenter.Application.Contracts.DTOs;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;

namespace MessageCenter.Application.Contracts.Services;

/// <summary>
/// 消息应用服务接口
/// </summary>
public interface IMessageAppService : IApplicationService
{
    /// <summary>
    /// 创建并发送消息
    /// </summary>
    Task<MessageDto> CreateAsync(CreateMessageDto input);

    /// <summary>
    /// 批量创建并发送消息
    /// </summary>
    Task<List<MessageDto>> CreateBatchAsync(List<CreateMessageDto> inputs);

    /// <summary>
    /// 根据ID获取消息
    /// </summary>
    Task<MessageDto> GetAsync(Guid id);

    /// <summary>
    /// 查询消息列表
    /// </summary>
    Task<PagedResultDto<MessageDto>> GetListAsync(MessageQueryDto input);

    /// <summary>
    /// 获取接收者的消息列表
    /// </summary>
    Task<PagedResultDto<MessageDto>> GetReceiverMessagesAsync(
        string receiverId,
        MessageQueryDto? input = null);

    /// <summary>
    /// 标记消息为已读
    /// </summary>
    Task MarkAsReadAsync(Guid id);

    /// <summary>
    /// 批量标记消息为已读
    /// </summary>
    Task MarkAsReadBatchAsync(List<Guid> ids);

    /// <summary>
    /// 标记所有消息为已读
    /// </summary>
    Task MarkAllAsReadAsync(string receiverId);

    /// <summary>
    /// 删除消息
    /// </summary>
    Task DeleteAsync(Guid id);

    /// <summary>
    /// 批量删除消息
    /// </summary>
    Task DeleteBatchAsync(List<Guid> ids);

    /// <summary>
    /// 获取未读消息数量
    /// </summary>
    Task<long> GetUnreadCountAsync(string receiverId);

    /// <summary>
    /// 获取消息统计信息
    /// </summary>
    Task<MessageStatisticsDto> GetStatisticsAsync(
        string? receiverId = null,
        DateTime? startTime = null,
        DateTime? endTime = null);

    /// <summary>
    /// 重试发送失败的消息
    /// </summary>
    Task RetryAsync(Guid id);

    /// <summary>
    /// 取消消息发送
    /// </summary>
    Task CancelAsync(Guid id);
}
