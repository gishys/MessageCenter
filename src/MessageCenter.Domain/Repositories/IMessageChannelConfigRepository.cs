using MessageCenter.Domain.Entities;
using MessageCenter.Domain.Shared.Enums;
using Volo.Abp.Domain.Repositories;

namespace MessageCenter.Domain.Repositories;

/// <summary>
/// 消息渠道配置仓储接口
/// </summary>
public interface IMessageChannelConfigRepository : IRepository<MessageChannelConfig, Guid>
{
    /// <summary>
    /// 获取指定渠道的默认配置
    /// </summary>
    Task<MessageChannelConfig?> GetDefaultConfigAsync(
        MessageChannel channel,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 获取指定渠道的所有启用配置
    /// </summary>
    Task<List<MessageChannelConfig>> GetEnabledConfigsAsync(
        MessageChannel channel,
        CancellationToken cancellationToken = default);
}
