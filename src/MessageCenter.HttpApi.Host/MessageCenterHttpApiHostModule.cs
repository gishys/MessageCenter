using MessageCenter.Application;
using MessageCenter.EntityFrameworkCore;
using MessageCenter.Integration;
using MessageCenter.Integration.Hubs;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Cors;
using Microsoft.OpenApi.Models;
using Volo.Abp;
using Volo.Abp.AspNetCore.Mvc;
using Volo.Abp.AspNetCore.Serilog;
using Volo.Abp.Autofac;
using Volo.Abp.Modularity;

namespace MessageCenter.HttpApi.Host;

[DependsOn(
    typeof(MessageCenterHttpApiModule),
    typeof(MessageCenterApplicationModule),
    typeof(MessageCenterEntityFrameworkCoreModule),
    typeof(MessageCenterIntegrationModule),
    typeof(AbpAspNetCoreMvcModule),
    typeof(AbpAutofacModule),
    typeof(AbpAspNetCoreSerilogModule)
)]
public class MessageCenterHttpApiHostModule : AbpModule
{
    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        var configuration = context.Services.GetConfiguration();

        ConfigureConventionalControllers();
        ConfigureAuthentication(context, configuration);
        ConfigureSignalR(context);
        ConfigureLocalization();
        ConfigureSwaggerServices(context);
        ConfigureCors(context, configuration);
    }

    private void ConfigureConventionalControllers()
    {
        Configure<AbpAspNetCoreMvcOptions>(options =>
        {
            options.ConventionalControllers.Create(typeof(MessageCenterApplicationModule).Assembly);
        });
    }

    private static void ConfigureAuthentication(ServiceConfigurationContext context, IConfiguration configuration)
    {
        context.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                options.Authority = configuration["AuthServer:Authority"];
                options.RequireHttpsMetadata = Convert.ToBoolean(configuration["AuthServer:RequireHttpsMetadata"]);
                options.Audience = configuration["AuthServer:Audience"];

                // 配置SignalR的JWT认证
                options.Events = new JwtBearerEvents
                {
                    OnMessageReceived = context =>
                    {
                        var accessToken = context.Request.Query["access_token"];
                        var path = context.HttpContext.Request.Path;
                        
                        // 如果是SignalR Hub路径，从查询字符串获取token
                        if (!string.IsNullOrEmpty(accessToken) && path.StartsWithSegments("/hubs"))
                        {
                            context.Token = accessToken;
                        }
                        return Task.CompletedTask;
                    }
                };
            });
    }

    private static void ConfigureSignalR(ServiceConfigurationContext context)
    {
        context.Services.AddSignalR(options =>
        {
            // 配置SignalR选项
            options.EnableDetailedErrors = context.Services.GetHostingEnvironment().IsDevelopment();
            options.ClientTimeoutInterval = TimeSpan.FromSeconds(30);
            options.KeepAliveInterval = TimeSpan.FromSeconds(15);
            options.MaximumReceiveMessageSize = 1024 * 1024; // 1MB
        });
    }

    private static void ConfigureLocalization()
    {
        // 配置本地化
    }

    private static void ConfigureSwaggerServices(ServiceConfigurationContext context)
    {
        context.Services.AddAbpSwaggerGen(
            options =>
            {
                options.SwaggerDoc("v1", new OpenApiInfo { Title = "MessageCenter API", Version = "v1" });
                options.DocInclusionPredicate((docName, description) => true);
                options.CustomSchemaIds(type => type.FullName);
            });
    }

    private static void ConfigureCors(ServiceConfigurationContext context, IConfiguration configuration)
    {
        context.Services.AddCors(options =>
        {
            options.AddDefaultPolicy(builder =>
            {
                builder
                    .WithOrigins(
                        configuration["App:CorsOrigins"]?
                            .Split(",", StringSplitOptions.RemoveEmptyEntries)
                            .Select(o => o.RemovePostFix("/"))
                            .ToArray() ?? []
                    )
                    .WithAbpExposedHeaders()
                    .SetIsOriginAllowedToAllowWildcardSubdomains()
                    .AllowAnyHeader()
                    .AllowAnyMethod()
                    .AllowCredentials();
            });
        });
    }

    public override void OnApplicationInitialization(ApplicationInitializationContext context)
    {
        var app = context.GetApplicationBuilder();
        var env = context.GetEnvironment();

        if (env.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
        }

        app.UseAbpRequestLocalization();
        app.UseCorrelationId();
        app.UseStaticFiles();
        app.UseRouting();
        app.UseCors();
        app.UseAuthentication();
        app.UseAbpClaimsMap();
        app.UseAuthorization();
        
        app.UseSwagger();
        app.UseSwaggerUI(options =>
        {
            options.SwaggerEndpoint("/swagger/v1/swagger.json", "MessageCenter API");
            options.RoutePrefix = "swagger";
        });
        
        app.UseAuditing();
        app.UseAbpSerilogEnrichers();
        app.UseUnitOfWork();
        
        // 配置SignalR Hub和API端点（必须在UseConfiguredEndpoints之前）
        app.UseEndpoints(endpoints =>
        {
            endpoints.MapHub<MessageHub>("/hubs/messages");
        });
        
        app.UseConfiguredEndpoints();
    }
}
