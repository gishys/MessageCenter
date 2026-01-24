using MessageCenter.Domain.Shared.Enums;
using Volo.Abp.Application.Dtos;

namespace MessageCenter.Application.Contracts.DTOs;

/// <summary>
/// 消息查询DTO
/// </summary>
public class MessageQueryDto : PagedAndSortedResultRequestDto
{
    /// <summary>
    /// 接收者ID
    /// </summary>
    public string? ReceiverId { get; set; }

    /// <summary>
    /// 发送者ID
    /// </summary>
    public Guid? SenderId { get; set; }

    /// <summary>
    /// 消息类型
    /// </summary>
    public MessageType? MessageType { get; set; }

    /// <summary>
    /// 消息渠道
    /// </summary>
    public MessageChannel? Channel { get; set; }

    /// <summary>
    /// 消息状态
    /// </summary>
    public MessageStatus? Status { get; set; }

    /// <summary>
    /// 消息优先级
    /// </summary>
    public MessagePriority? Priority { get; set; }

    /// <summary>
    /// 业务类型
    /// </summary>
    public string? BusinessType { get; set; }

    /// <summary>
    /// 业务ID
    /// </summary>
    public string? BusinessId { get; set; }

    /// <summary>
    /// 是否已读
    /// </summary>
    public bool? IsRead { get; set; }

    /// <summary>
    /// 关键词搜索（标题、内容）
    /// </summary>
    public string? Keyword { get; set; }

    /// <summary>
    /// 开始时间
    /// </summary>
    public DateTime? StartTime { get; set; }

    /// <summary>
    /// 结束时间
    /// </summary>
    public DateTime? EndTime { get; set; }

    /// <summary>
    /// 标签
    /// </summary>
    public string? Tags { get; set; }
}
