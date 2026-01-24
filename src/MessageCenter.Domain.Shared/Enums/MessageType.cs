namespace MessageCenter.Domain.Shared.Enums;

/// <summary>
/// 消息类型枚举
/// 支持多种业务场景：社交互动、工作流、警报监控、事务与营销、实时通知等
/// </summary>
public enum MessageType
{
    /// <summary>
    /// 通知消息 - 通用通知类消息
    /// </summary>
    Notification = 1,

    /// <summary>
    /// 工作流消息 - 工作流相关通知
    /// </summary>
    Workflow = 2,

    /// <summary>
    /// 警报消息 - 系统监控和警报
    /// </summary>
    Alert = 3,

    /// <summary>
    /// 事务消息 - 业务事务通知
    /// </summary>
    Transaction = 4,

    /// <summary>
    /// 营销消息 - 营销推广类消息
    /// </summary>
    Marketing = 5,

    /// <summary>
    /// 社交消息 - 社交互动类消息
    /// </summary>
    Social = 6,

    /// <summary>
    /// 系统消息 - 系统级通知
    /// </summary>
    System = 7,

    /// <summary>
    /// 实时消息 - 实时推送类消息
    /// </summary>
    Realtime = 8
}
