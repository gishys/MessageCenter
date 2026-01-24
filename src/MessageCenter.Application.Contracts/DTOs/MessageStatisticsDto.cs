using MessageCenter.Domain.Shared.Enums;

namespace MessageCenter.Application.Contracts.DTOs;

/// <summary>
/// 消息统计DTO
/// </summary>
public class MessageStatisticsDto
{
    /// <summary>
    /// 总消息数
    /// </summary>
    public long TotalCount { get; set; }

    /// <summary>
    /// 未读消息数
    /// </summary>
    public long UnreadCount { get; set; }

    /// <summary>
    /// 已读消息数
    /// </summary>
    public long ReadCount { get; set; }

    /// <summary>
    /// 按状态统计
    /// </summary>
    public Dictionary<MessageStatus, long> StatusStatistics { get; set; } = [];

    /// <summary>
    /// 按类型统计
    /// </summary>
    public Dictionary<MessageType, long> TypeStatistics { get; set; } = [];

    /// <summary>
    /// 按渠道统计
    /// </summary>
    public Dictionary<MessageChannel, long> ChannelStatistics { get; set; } = [];

    /// <summary>
    /// 统计时间范围
    /// </summary>
    public DateTime? StartTime { get; set; }

    /// <summary>
    /// 统计时间范围
    /// </summary>
    public DateTime? EndTime { get; set; }
}
