using MessageCenter.Application.Contracts;
using Volo.Abp.AspNetCore.Mvc;
using Volo.Abp.Modularity;

namespace MessageCenter.HttpApi;

[DependsOn(
    typeof(MessageCenterApplicationContractsModule),
    typeof(AbpAspNetCoreMvcModule)
)]
public class MessageCenterHttpApiModule : AbpModule
{
}
