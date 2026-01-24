namespace MessageCenter.Domain.Shared.Enums;

/// <summary>
/// 消息模板类型枚举
/// </summary>
public enum TemplateType
{
    /// <summary>
    /// 文本模板
    /// </summary>
    Text = 1,

    /// <summary>
    /// HTML模板
    /// </summary>
    Html = 2,

    /// <summary>
    /// Markdown模板
    /// </summary>
    Markdown = 3,

    /// <summary>
    /// JSON模板
    /// </summary>
    Json = 4
}
