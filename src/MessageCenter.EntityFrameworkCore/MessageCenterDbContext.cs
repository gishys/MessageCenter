using MessageCenter.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Volo.Abp.Data;
using Volo.Abp.EntityFrameworkCore;
using Volo.Abp.MultiTenancy;

namespace MessageCenter.EntityFrameworkCore;

[ConnectionStringName("Default")]
public class MessageCenterDbContext(DbContextOptions<MessageCenterDbContext> options) : AbpDbContext<MessageCenterDbContext>(options), IMultiTenant
{
    public Guid? TenantId { get; set; }

    public DbSet<Message> Messages { get; set; }
    public DbSet<MessageReceipt> MessageReceipts { get; set; }
    public DbSet<MessageTemplate> MessageTemplates { get; set; }
    public DbSet<MessageChannelConfig> MessageChannelConfigs { get; set; }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.ConfigureMessageCenter();
    }
}
