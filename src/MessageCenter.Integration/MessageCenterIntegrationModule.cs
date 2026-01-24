using MessageCenter.Application.Contracts.Services;
using Microsoft.Extensions.DependencyInjection;
using Volo.Abp.Modularity;

namespace MessageCenter.Integration;

/// <summary>
/// 集成层模块
/// 负责处理跨层集成，如事件处理器、外部服务集成等
/// </summary>
[DependsOn(
    typeof(MessageCenter.Application.Contracts.MessageCenterApplicationContractsModule)
)]
public class MessageCenterIntegrationModule : AbpModule
{
    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        // 注册集成层的实时推送服务实现，替换Application层的占位实现
        context.Services.AddTransient<IMessageRealtimeService, Services.MessageRealtimeService>();
    }
}
