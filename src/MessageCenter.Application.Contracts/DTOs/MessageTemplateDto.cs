using MessageCenter.Domain.Shared.Enums;
using Volo.Abp.Application.Dtos;

namespace MessageCenter.Application.Contracts.DTOs;

/// <summary>
/// 消息模板DTO
/// </summary>
public class MessageTemplateDto : FullAuditedEntityDto<Guid>
{
    /// <summary>
    /// 模板名称
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// 模板代码
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
    /// 模板变量说明
    /// </summary>
    public string? Variables { get; set; }

    /// <summary>
    /// 扩展属性
    /// </summary>
    public string? Extension { get; set; }
}
