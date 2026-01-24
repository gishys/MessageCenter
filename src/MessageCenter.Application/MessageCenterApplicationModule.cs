using MessageCenter.Application.Contracts;
using MessageCenter.Application.Mappings;
using MessageCenter.Domain;
using MessageCenter.Domain.Shared;
using Microsoft.Extensions.DependencyInjection;
using Volo.Abp.AutoMapper;
using Volo.Abp.Modularity;

namespace MessageCenter.Application;

[DependsOn(
    typeof(MessageCenterDomainModule),
    typeof(MessageCenterDomainSharedModule),
    typeof(MessageCenterApplicationContractsModule),
    typeof(AbpAutoMapperModule)
)]
public class MessageCenterApplicationModule : AbpModule
{
    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        context.Services.AddAutoMapperObjectMapper<MessageCenterApplicationModule>();
        Configure<AbpAutoMapperOptions>(options =>
        {
            options.AddMaps<MessageMappingProfile>();
            options.AddMaps<MessageTemplateMappingProfile>();
        });
    }
}
