using MessageCenter.Domain.Entities;
using MessageCenter.Domain.Shared.Enums;
using Volo.Abp.Domain.Repositories;

namespace MessageCenter.Domain.Repositories;

/// <summary>
/// 消息模板仓储接口
/// </summary>
public interface IMessageTemplateRepository : IRepository<MessageTemplate, Guid>
{
    /// <summary>
    /// 根据模板代码获取模板
    /// </summary>
    Task<MessageTemplate?> GetByCodeAsync(
        string code,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 根据消息类型和渠道获取模板列表
    /// </summary>
    Task<List<MessageTemplate>> GetByTypeAndChannelAsync(
        MessageType messageType,
        MessageChannel channel,
        bool onlyEnabled = true,
        CancellationToken cancellationToken = default);
}
