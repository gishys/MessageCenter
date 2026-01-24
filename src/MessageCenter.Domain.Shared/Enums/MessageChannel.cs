namespace MessageCenter.Domain.Shared.Enums;

/// <summary>
/// 消息渠道枚举
/// 支持多种消息发送渠道
/// </summary>
public enum MessageChannel
{
    /// <summary>
    /// 站内信 - 系统内部消息
    /// </summary>
    InApp = 1,

    /// <summary>
    /// 邮件 - 电子邮件
    /// </summary>
    Email = 2,

    /// <summary>
    /// 短信 - 短消息服务
    /// </summary>
    Sms = 3,

    /// <summary>
    /// 推送通知 - 移动端推送
    /// </summary>
    Push = 4,

    /// <summary>
    /// 微信 - 微信公众号/小程序
    /// </summary>
    WeChat = 5,

    /// <summary>
    /// 钉钉 - 钉钉消息
    /// </summary>
    DingTalk = 6,

    /// <summary>
    /// WebSocket - WebSocket实时推送
    /// </summary>
    WebSocket = 7,

    /// <summary>
    /// 站外通知 - 第三方通知渠道
    /// </summary>
    External = 8
}
