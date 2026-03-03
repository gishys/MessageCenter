using MessageCenter.Domain.Entities;
using MessageCenter.Domain.Shared.Constants;
using Microsoft.EntityFrameworkCore;
using Volo.Abp;
using Volo.Abp.EntityFrameworkCore.Modeling;

namespace MessageCenter.EntityFrameworkCore;

public static class MessageCenterDbContextModelCreatingExtensions
{
    public static void ConfigureMessageCenter(this ModelBuilder builder)
    {
        Check.NotNull(builder, nameof(builder));

        /* Configure your own tables/entities inside here */

        builder.Entity<Message>(b =>
        {
            b.ToTable($"{MessageCenterConsts.DbTablePrefix}Messages", MessageCenterConsts.DbSchema);
            b.ConfigureByConvention();

            b.Property(x => x.Title).IsRequired().HasMaxLength(MessageCenterConsts.MaxTitleLength);
            b.Property(x => x.Content).IsRequired().HasMaxLength(MessageCenterConsts.MaxContentLength);
            b.Property(x => x.Summary).HasMaxLength(MessageCenterConsts.MaxSummaryLength);
            b.Property(x => x.ReceiverId).IsRequired().HasMaxLength(MessageCenterConsts.MaxReceiverIdLength);
            b.Property(x => x.SenderName).HasMaxLength(200);
            b.Property(x => x.ReceiverName).HasMaxLength(200);
            b.Property(x => x.ReceiverEmail).HasMaxLength(200);
            b.Property(x => x.ReceiverPhone).HasMaxLength(50);
            b.Property(x => x.BusinessType).HasMaxLength(MessageCenterConsts.MaxBusinessTypeLength);
            b.Property(x => x.BusinessId).HasMaxLength(MessageCenterConsts.MaxBusinessIdLength);
            b.Property(x => x.FailureReason).HasMaxLength(1000);
            b.Property(x => x.Extension).HasMaxLength(MessageCenterConsts.MaxExtensionLength);
            b.Property(x => x.Tags).HasMaxLength(500);
            b.Property(x => x.LinkUrl).HasMaxLength(1000);
            b.Property(x => x.AttachmentIds).HasMaxLength(2000);
            b.Property(x => x.Body).HasColumnType("text");

            b.HasIndex(x => x.ReceiverId);
            b.HasIndex(x => x.SenderId);
            b.HasIndex(x => new { x.MessageType, x.Channel });
            b.HasIndex(x => x.Status);
            b.HasIndex(x => new { x.BusinessType, x.BusinessId });
            b.HasIndex(x => x.CreationTime);
            b.HasIndex(x => x.ScheduledSendTime);
            b.HasIndex(x => x.ConversationId);
        });

        builder.Entity<MessageReceipt>(b =>
        {
            b.ToTable($"{MessageCenterConsts.DbTablePrefix}MessageReceipts", MessageCenterConsts.DbSchema);
            b.ConfigureByConvention();

            b.Property(x => x.ReceiverId).IsRequired().HasMaxLength(MessageCenterConsts.MaxReceiverIdLength);

            b.HasIndex(x => x.MessageId);
            b.HasIndex(x => x.ReceiverId);
            b.HasIndex(x => new { x.MessageId, x.ReceiverId }).IsUnique();
            b.HasIndex(x => x.Status);

            b.HasOne(x => x.Message)
                .WithMany(x => x.Receipts)
                .HasForeignKey(x => x.MessageId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        builder.Entity<MessageTemplate>(b =>
        {
            b.ToTable($"{MessageCenterConsts.DbTablePrefix}MessageTemplates", MessageCenterConsts.DbSchema);
            b.ConfigureByConvention();

            b.Property(x => x.Name).IsRequired().HasMaxLength(MessageCenterConsts.MaxTemplateNameLength);
            b.Property(x => x.Code).IsRequired().HasMaxLength(MessageCenterConsts.MaxTemplateCodeLength);
            b.Property(x => x.Title).IsRequired().HasMaxLength(MessageCenterConsts.MaxTitleLength);
            b.Property(x => x.Content).IsRequired();
            b.Property(x => x.Description).HasMaxLength(1000);
            b.Property(x => x.Variables).HasMaxLength(MessageCenterConsts.MaxExtensionLength);
            b.Property(x => x.Extension).HasMaxLength(MessageCenterConsts.MaxExtensionLength);

            b.HasIndex(x => x.Code).IsUnique();
            b.HasIndex(x => new { x.MessageType, x.Channel });
            b.HasIndex(x => x.IsEnabled);
        });

        builder.Entity<MessageChannelConfig>(b =>
        {
            b.ToTable($"{MessageCenterConsts.DbTablePrefix}MessageChannelConfigs", MessageCenterConsts.DbSchema);
            b.ConfigureByConvention();

            b.Property(x => x.Name).IsRequired().HasMaxLength(200);
            b.Property(x => x.ConfigJson).IsRequired();
            b.Property(x => x.Description).HasMaxLength(1000);

            b.HasIndex(x => x.Channel);
            b.HasIndex(x => new { x.Channel, x.IsDefault });
        });

        builder.Entity<Conversation>(b =>
        {
            b.ToTable($"{MessageCenterConsts.DbTablePrefix}Conversations", MessageCenterConsts.DbSchema);
            b.ConfigureByConvention();

            b.Property(x => x.Type).IsRequired().HasMaxLength(50);
            b.Property(x => x.Title).HasMaxLength(200);
            b.Property(x => x.ParticipantIds).IsRequired();

            b.HasIndex(x => x.Type);
            b.HasIndex(x => x.LastMessageAt);
        });
    }
}
