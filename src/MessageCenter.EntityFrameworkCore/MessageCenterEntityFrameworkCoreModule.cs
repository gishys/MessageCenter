using MessageCenter.Domain;
using MessageCenter.EntityFrameworkCore.Repositories;
using Microsoft.Extensions.DependencyInjection;
using Volo.Abp.EntityFrameworkCore;
using Volo.Abp.Modularity;

namespace MessageCenter.EntityFrameworkCore;

[DependsOn(
    typeof(MessageCenterDomainModule),
    typeof(AbpEntityFrameworkCoreModule)
)]
public class MessageCenterEntityFrameworkCoreModule : AbpModule
{
    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        context.Services.AddAbpDbContext<MessageCenterDbContext>(options =>
        {
            options.AddDefaultRepositories(includeAllEntities: true);

            // 添加自定义仓储
            options.AddRepository<Domain.Entities.Message, MessageRepository>();
            options.AddRepository<Domain.Entities.MessageTemplate, MessageTemplateRepository>();
            options.AddRepository<Domain.Entities.MessageChannelConfig, MessageChannelConfigRepository>();
        });

        Configure<AbpDbContextOptions>(options =>
        {
            options.UseNpgsql();
        });
    }
}
