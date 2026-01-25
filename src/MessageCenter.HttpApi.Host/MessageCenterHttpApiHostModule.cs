using MessageCenter.Application;
using MessageCenter.EntityFrameworkCore;
using MessageCenter.Integration;
using MessageCenter.Integration.Hubs;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Cors;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;
using Volo.Abp;
using Volo.Abp.AspNetCore.Mvc;
using Volo.Abp.AspNetCore.Serilog;
using Volo.Abp.Autofac;
using Volo.Abp.Modularity;
using Volo.Abp.Security.Claims;

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
        // 使用 SecretKey 验证 token（与 TokenService 生成的 token 格式匹配）
        // 注意：SecretKey必须与token来源项目完全一致
        var secretKey = configuration["AuthServer:SecretKey"] 
            ?? "MessageCenter_DefaultSecretKey_For_JWT_Token_Generation_Must_Be_At_Least_32_Characters_Long";
        var issuer = configuration["AuthServer:Authority"] 
            ?? "https://localhost:44307";
        var audience = configuration["AuthServer:Audience"] 
            ?? "MessageCenter";
        
        // 记录配置信息（用于调试）- 显示SecretKey的前10个字符和长度，不显示完整值
        var logger = context.Services.BuildServiceProvider()
            .GetRequiredService<ILogger<MessageCenterHttpApiHostModule>>();
        var secretKeyPreview = secretKey?.Length > 10 
            ? secretKey.Substring(0, 10) + "..." 
            : secretKey;
        logger.LogInformation("JWT认证配置 - Issuer: {Issuer}, Audience: {Audience}, SecretKey预览: {SecretKeyPreview}, SecretKey长度: {SecretKeyLength}", 
            issuer, audience, secretKeyPreview, secretKey?.Length ?? 0);
        
        context.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                // 构建可能的Issuer列表（兼容http和https，以及不同的端口）
                var possibleIssuers = new List<string> { issuer };
                
                // 添加http和https版本
                if (issuer.StartsWith("http://", StringComparison.OrdinalIgnoreCase))
                {
                    possibleIssuers.Add(issuer.Replace("http://", "https://", StringComparison.OrdinalIgnoreCase));
                }
                else if (issuer.StartsWith("https://", StringComparison.OrdinalIgnoreCase))
                {
                    possibleIssuers.Add(issuer.Replace("https://", "http://", StringComparison.OrdinalIgnoreCase));
                }
                
                // 添加常见的端口变体（如果Authority是localhost:44307）
                if (issuer.Contains("localhost:44307", StringComparison.OrdinalIgnoreCase))
                {
                    possibleIssuers.Add("https://localhost:44307");
                    possibleIssuers.Add("http://localhost:44307");
                    // 如果实际运行在不同端口，可以添加
                    // possibleIssuers.Add("https://localhost:44359");
                    // possibleIssuers.Add("http://localhost:44359");
                }
                
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey)),
                    ValidateIssuer = true,
                    // 支持多个可能的Issuer值（兼容http和https，以及不同的端口配置）
                    ValidIssuers = possibleIssuers.Distinct().ToArray(),
                    ValidateAudience = true,
                    ValidAudience = audience,
                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.FromMinutes(5) // 允许5分钟的时间差异，避免时间同步问题
                };
                
                // 配置事件处理（用于调试和日志记录）
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
                    },
                    OnAuthenticationFailed = authContext =>
                    {
                        var eventLogger = authContext.HttpContext.RequestServices
                            .GetRequiredService<ILogger<MessageCenterHttpApiHostModule>>();
                        
                        var exception = authContext.Exception;
                        eventLogger.LogError(exception, 
                            "JWT 认证失败 - 错误类型: {ExceptionType}, 消息: {Message}, Token长度: {TokenLength}",
                            exception.GetType().Name,
                            exception.Message,
                            authContext.Request.Headers["Authorization"].ToString().Length);
                        
                        // 如果是签名验证失败，记录更详细的信息
                        if (exception is SecurityTokenSignatureKeyNotFoundException)
                        {
                            var config = authContext.HttpContext.RequestServices
                                .GetRequiredService<IConfiguration>();
                            var currentSecretKey = config["AuthServer:SecretKey"] 
                                ?? "MessageCenter_DefaultSecretKey_For_JWT_Token_Generation";
                            var secretKeyPreview = currentSecretKey.Length > 20 
                                ? currentSecretKey.Substring(0, 20) + "..." 
                                : currentSecretKey;
                            eventLogger.LogError("签名验证失败 - 当前使用的SecretKey预览: {SecretKeyPreview}, 长度: {SecretKeyLength}", 
                                secretKeyPreview, currentSecretKey.Length);
                            eventLogger.LogError("请检查token来源项目的appsettings.json中AuthServer:SecretKey的值");
                            eventLogger.LogError("确保当前项目使用的SecretKey与token生成时使用的SecretKey完全一致");
                        }
                        else if (exception is SecurityTokenInvalidIssuerException)
                        {
                            eventLogger.LogError("Issuer验证失败 - 期望: {ExpectedIssuers}, 请检查token中的iss声明",
                                string.Join(", ", options.TokenValidationParameters.ValidIssuers ?? new[] { issuer }));
                        }
                        else if (exception is SecurityTokenInvalidAudienceException)
                        {
                            eventLogger.LogError("Audience验证失败 - 期望: {ExpectedAudience}, 请检查token中的aud声明",
                                audience);
                        }
                        else if (exception is SecurityTokenExpiredException)
                        {
                            eventLogger.LogWarning("Token已过期");
                        }
                        
                        return Task.CompletedTask;
                    },
                    OnTokenValidated = validatedContext =>
                    {
                        var eventLogger = validatedContext.HttpContext.RequestServices
                            .GetRequiredService<ILogger<MessageCenterHttpApiHostModule>>();
                        
                        var userId = validatedContext.Principal?.FindFirst(AbpClaimTypes.UserId)?.Value;
                        var userName = validatedContext.Principal?.FindFirst(AbpClaimTypes.UserName)?.Value;
                        var tokenIssuer = validatedContext.Principal?.FindFirst("iss")?.Value;
                        var tokenAudience = validatedContext.Principal?.FindFirst("aud")?.Value;
                        
                        eventLogger.LogInformation("JWT Token 验证成功 - UserId: {UserId}, UserName: {UserName}, Issuer: {Issuer}, Audience: {Audience}",
                            userId, userName, tokenIssuer, tokenAudience);
                        return Task.CompletedTask;
                    },
                    OnChallenge = challengeContext =>
                    {
                        var eventLogger = challengeContext.HttpContext.RequestServices
                            .GetRequiredService<ILogger<MessageCenterHttpApiHostModule>>();
                        eventLogger.LogWarning("JWT 认证挑战 - 错误: {Error}, 错误描述: {ErrorDescription}",
                            challengeContext.Error, challengeContext.ErrorDescription);
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
