using MessageCenter.Domain.Shared.Enums;
using Volo.Abp.Domain.Entities.Auditing;
using Volo.Abp.MultiTenancy;

namespace MessageCenter.Domain.Entities;

/// <summary>
/// 消息模板实体
/// 支持多种模板类型和渠道
/// </summary>
public class MessageTemplate : FullAuditedAggregateRoot<Guid>, IMultiTenant
{
    /// <summary>
    /// 租户ID
    /// </summary>
    public Guid? TenantId { get; set; }

    /// <summary>
    /// 模板名称
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// 模板代码（唯一标识）
    /// </summary>
    public string Code { get; set; } = string.Empty;

    /// <summary>
    /// 模板类型
    /// </summary>
    public TemplateType TemplateType { get; set; }

    /// <summary>
    /// 消息类型
    /// </summary>
    public MessageType MessageType { get; set; }

    /// <summary>
    /// 消息渠道
    /// </summary>
    public MessageChannel Channel { get; set; }

    /// <summary>
    /// 模板标题
    /// </summary>
    public string Title { get; set; } = string.Empty;

    /// <summary>
    /// 模板内容
    /// </summary>
    public string Content { get; set; } = string.Empty;

    /// <summary>
    /// 模板描述
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// 是否启用
    /// </summary>
    public bool IsEnabled { get; set; }

    /// <summary>
    /// 模板变量说明（JSON格式）
    /// </summary>
    public string? Variables { get; set; }

    /// <summary>
    /// 扩展属性（JSON格式）
    /// </summary>
    public string? Extension { get; set; }

    protected MessageTemplate()
    {
    }

    public MessageTemplate(
        Guid id,
        string name,
        string code,
        TemplateType templateType,
        MessageType messageType,
        MessageChannel channel,
        string title,
        string content)
        : base(id)
    {
        Name = name;
        Code = code;
        TemplateType = templateType;
        MessageType = messageType;
        Channel = channel;
        Title = title;
        Content = content;
        IsEnabled = true;
    }
}
