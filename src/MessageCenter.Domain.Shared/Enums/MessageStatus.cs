namespace MessageCenter.Domain.Shared.Enums;

/// <summary>
/// 消息状态枚举
/// </summary>
public enum MessageStatus
{
    /// <summary>
    /// 待发送 - 消息已创建，等待发送
    /// </summary>
    Pending = 1,

    /// <summary>
    /// 发送中 - 消息正在发送
    /// </summary>
    Sending = 2,

    /// <summary>
    /// 已发送 - 消息已成功发送
    /// </summary>
    Sent = 3,

    /// <summary>
    /// 已送达 - 消息已送达接收方
    /// </summary>
    Delivered = 4,

    /// <summary>
    /// 已读 - 消息已被接收方阅读
    /// </summary>
    Read = 5,

    /// <summary>
    /// 发送失败 - 消息发送失败
    /// </summary>
    Failed = 6,

    /// <summary>
    /// 已取消 - 消息已被取消
    /// </summary>
    Cancelled = 7,

    /// <summary>
    /// 已过期 - 消息已过期
    /// </summary>
    Expired = 8
}
