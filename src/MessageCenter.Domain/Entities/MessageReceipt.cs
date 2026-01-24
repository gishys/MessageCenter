using MessageCenter.Domain.Shared.Enums;
using Volo.Abp.Domain.Entities;
using Volo.Abp.MultiTenancy;

namespace MessageCenter.Domain.Entities;

/// <summary>
/// 消息接收记录实体
/// 记录消息的接收和阅读状态
/// </summary>
public class MessageReceipt : Entity<Guid>, IMultiTenant
{
    /// <summary>
    /// 租户ID
    /// </summary>
    public Guid? TenantId { get; set; }

    /// <summary>
    /// 消息ID
    /// </summary>
    public Guid MessageId { get; set; }

    /// <summary>
    /// 接收者ID
    /// </summary>
    public string ReceiverId { get; set; } = string.Empty;

    /// <summary>
    /// 接收状态
    /// </summary>
    public MessageStatus Status { get; set; }

    /// <summary>
    /// 接收时间
    /// </summary>
    public DateTime? ReceivedTime { get; set; }

    /// <summary>
    /// 阅读时间
    /// </summary>
    public DateTime? ReadTime { get; set; }

    /// <summary>
    /// 是否已读
    /// </summary>
    public bool IsRead { get; set; }

    /// <summary>
    /// 消息实体（导航属性）
    /// </summary>
    public virtual Message? Message { get; set; }

    protected MessageReceipt()
    {
    }

    public MessageReceipt(
        Guid id,
        Guid messageId,
        string receiverId)
        : base(id)
    {
        MessageId = messageId;
        ReceiverId = receiverId;
        Status = MessageStatus.Pending;
        IsRead = false;
    }

    /// <summary>
    /// 标记为已接收
    /// </summary>
    public void MarkAsReceived(DateTime receivedTime)
    {
        Status = MessageStatus.Delivered;
        ReceivedTime = receivedTime;
    }

    /// <summary>
    /// 标记为已读
    /// </summary>
    public void MarkAsRead(DateTime readTime)
    {
        Status = MessageStatus.Read;
        IsRead = true;
        ReadTime = readTime;
    }
}
