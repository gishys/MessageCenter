namespace MessageCenter.Domain.Shared.Constants;

/// <summary>
/// 消息中心常量定义
/// </summary>
public static class MessageCenterConsts
{
    /// <summary>
    /// 默认数据库表前缀
    /// </summary>
    public const string DbTablePrefix = "Msg";

    /// <summary>
    /// 默认数据库架构
    /// </summary>
    public const string DbSchema = null;

    /// <summary>
    /// 消息标题最大长度
    /// </summary>
    public const int MaxTitleLength = 500;

    /// <summary>
    /// 消息内容最大长度
    /// </summary>
    public const int MaxContentLength = 10000;

    /// <summary>
    /// 消息摘要最大长度
    /// </summary>
    public const int MaxSummaryLength = 1000;

    /// <summary>
    /// 模板名称最大长度
    /// </summary>
    public const int MaxTemplateNameLength = 200;

    /// <summary>
    /// 模板代码最大长度
    /// </summary>
    public const int MaxTemplateCodeLength = 100;

    /// <summary>
    /// 接收者ID最大长度
    /// </summary>
    public const int MaxReceiverIdLength = 100;

    /// <summary>
    /// 业务类型最大长度
    /// </summary>
    public const int MaxBusinessTypeLength = 100;

    /// <summary>
    /// 业务ID最大长度
    /// </summary>
    public const int MaxBusinessIdLength = 100;

    /// <summary>
    /// 扩展属性最大长度（JSON）
    /// </summary>
    public const int MaxExtensionLength = 5000;

    /// <summary>
    /// 默认消息过期时间（天）
    /// </summary>
    public const int DefaultExpirationDays = 30;

    /// <summary>
    /// 批量发送最大数量
    /// </summary>
    public const int MaxBatchSendCount = 1000;
}
