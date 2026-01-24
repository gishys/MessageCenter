using MessageCenter.HttpApi.Host.Extensions;
using Serilog;
using Serilog.Events;

namespace MessageCenter.HttpApi.Host;

public class Program
{
    public async static Task<int> Main(string[] args)
    {
        // 配置 Npgsql 以正确处理 DateTime（必须在任何数据库操作之前设置）
        AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", false);
        
        Log.Logger = new LoggerConfiguration()
#if DEBUG
            .MinimumLevel.Debug()
#else
            .MinimumLevel.Information()
#endif
            .MinimumLevel.Override("Microsoft", LogEventLevel.Information)
            .MinimumLevel.Override("Microsoft.EntityFrameworkCore", LogEventLevel.Warning)
            .Enrich.FromLogContext()
            .WriteTo.File("Logs/logs.txt")
            .WriteTo.Console()
            .CreateLogger();

        try
        {
            Log.Information("Starting MessageCenter.HttpApi.Host.");
            var builder = WebApplication.CreateBuilder(args);
            builder.Host.UseAutofac();
            builder.Host.UseSerilog();
            await builder.AddApplicationAsync<MessageCenterHttpApiHostModule>();
            var app = builder.Build();
            
            // 检查数据库健康状态（非阻塞，仅记录警告）
            try
            {
                var dbHealthy = await app.CheckDatabaseHealthAsync();
                if (!dbHealthy)
                {
                    Log.Warning("数据库连接检查失败，但应用程序将继续启动。请检查连接字符串配置。");
                }
            }
            catch (Exception ex)
            {
                Log.Warning(ex, "数据库健康检查时发生异常，但应用程序将继续启动。");
            }
            
            // 开发环境自动迁移（可选，生产环境应禁用）
            // await app.MigrateDatabaseAsync();
            
            await app.InitializeApplicationAsync();
            await app.RunAsync();
            return 0;
        }
        catch (Exception ex)
        {
            if (ex is HostAbortedException)
            {
                throw;
            }

            Log.Fatal(ex, "Host terminated unexpectedly!");
            return 1;
        }
        finally
        {
            Log.CloseAndFlush();
        }
    }
}
