using MessageCenter.Domain.Shared.Enums;

namespace MessageCenter.Application.Contracts.DTOs;

/// <summary>
/// 创建消息模板DTO
/// </summary>
public class CreateMessageTemplateDto
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
    public bool IsEnabled { get; set; } = true;

    /// <summary>
    /// 模板变量说明（JSON格式）
    /// </summary>
    public string? Variables { get; set; }

    /// <summary>
    /// 扩展属性（JSON格式）
    /// </summary>
    public string? Extension { get; set; }
}
