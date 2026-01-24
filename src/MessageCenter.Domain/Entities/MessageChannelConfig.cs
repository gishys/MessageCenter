using MessageCenter.Domain.Shared.Enums;
using Volo.Abp.Domain.Entities.Auditing;
using Volo.Abp.MultiTenancy;

namespace MessageCenter.Domain.Entities;

/// <summary>
/// 消息渠道配置实体
/// 配置不同渠道的发送参数
/// </summary>
public class MessageChannelConfig : FullAuditedAggregateRoot<Guid>, IMultiTenant
{
    /// <summary>
    /// 租户ID
    /// </summary>
    public Guid? TenantId { get; set; }

    /// <summary>
    /// 渠道类型
    /// </summary>
    public MessageChannel Channel { get; set; }

    /// <summary>
    /// 配置名称
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// 是否启用
    /// </summary>
    public bool IsEnabled { get; set; }

    /// <summary>
    /// 是否默认配置
    /// </summary>
    public bool IsDefault { get; set; }

    /// <summary>
    /// 配置参数（JSON格式）
    /// </summary>
    public string ConfigJson { get; set; } = string.Empty;

    /// <summary>
    /// 配置描述
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// 每日发送限额
    /// </summary>
    public int? DailyLimit { get; set; }

    /// <summary>
    /// 每小时发送限额
    /// </summary>
    public int? HourlyLimit { get; set; }

    protected MessageChannelConfig()
    {
    }

    public MessageChannelConfig(
        Guid id,
        MessageChannel channel,
        string name,
        string configJson)
        : base(id)
    {
        Channel = channel;
        Name = name;
        ConfigJson = configJson;
        IsEnabled = true;
        IsDefault = false;
    }
}
