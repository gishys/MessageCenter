using Volo.Abp.Domain.Entities.Auditing;
using Volo.Abp.MultiTenancy;

namespace MessageCenter.Domain.Entities;

/// <summary>
/// 会话实体：用于按会话聚合消息（谁和谁的聊天）
/// </summary>
public class Conversation : FullAuditedAggregateRoot<Guid>, IMultiTenant
{
    /// <summary>
    /// 租户ID
    /// </summary>
    public Guid? TenantId { get; set; }

    /// <summary>
    /// 会话类型：点对点 / 系统 / 群组等
    /// </summary>
    public string Type { get; set; } = "System";

    /// <summary>
    /// 会话标题（可选）
    /// </summary>
    public string? Title { get; set; }

    /// <summary>
    /// 参与者ID集合（JSON 数组字符串）
    /// </summary>
    public string ParticipantIds { get; set; } = "[]";

    /// <summary>
    /// 最近一条消息时间
    /// </summary>
    public DateTime? LastMessageAt { get; set; }

    protected Conversation()
    {
    }

    public Conversation(Guid id) : base(id)
    {
    }
}

