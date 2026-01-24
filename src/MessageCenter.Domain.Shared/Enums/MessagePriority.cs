namespace MessageCenter.Domain.Shared.Enums;

/// <summary>
/// 消息优先级枚举
/// </summary>
public enum MessagePriority
{
    /// <summary>
    /// 低优先级
    /// </summary>
    Low = 1,

    /// <summary>
    /// 普通优先级
    /// </summary>
    Normal = 2,

    /// <summary>
    /// 高优先级
    /// </summary>
    High = 3,

    /// <summary>
    /// 紧急优先级
    /// </summary>
    Urgent = 4
}
