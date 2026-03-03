namespace MessageCenter.Application.Contracts.DTOs;

/// <summary>
/// 更新消息 DTO（当前主要用于更新消息体内容）
/// </summary>
public class UpdateMessageDto
{
    /// <summary>
    /// 可选：更新标题（为空则不修改）
    /// </summary>
    public string? Title { get; set; }

    /// <summary>
    /// 可选：更新摘要（为空字符串则视为清空，null 不修改）
    /// </summary>
    public string? Summary { get; set; }

    /// <summary>
    /// 消息体（块列表 JSON），必填
    /// </summary>
    public string Body { get; set; } = string.Empty;
}

