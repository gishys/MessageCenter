using MessageCenter.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace MessageCenter.HttpApi.Host.Extensions;

/// <summary>
/// 数据库迁移扩展方法
/// 提供自动迁移和迁移检查功能
/// </summary>
public static class DatabaseMigrationExtensions
{
    /// <summary>
    /// 自动应用数据库迁移（仅用于开发环境）
    /// ⚠️ 警告：生产环境不应使用此方法，应手动执行迁移脚本
    /// </summary>
    public static async Task MigrateDatabaseAsync(this IHost host)
    {
        using var scope = host.Services.CreateScope();
        var services = scope.ServiceProvider;
        var logger = services.GetRequiredService<ILogger<MessageCenterDbContext>>();
        var environment = services.GetRequiredService<IHostEnvironment>();

        try
        {
            var context = services.GetRequiredService<MessageCenterDbContext>();
            var pendingMigrations = await context.Database.GetPendingMigrationsAsync();

            if (pendingMigrations.Any())
            {
                logger.LogWarning(
                    "检测到 {Count} 个待应用的数据库迁移: {Migrations}",
                    pendingMigrations.Count(),
                    string.Join(", ", pendingMigrations));

                if (environment.IsDevelopment())
                {
                    logger.LogInformation("开发环境：自动应用数据库迁移...");
                    await context.Database.MigrateAsync();
                    logger.LogInformation("数据库迁移已成功应用");
                }
                else
                {
                    logger.LogError(
                        "生产环境检测到待应用的迁移！请手动执行迁移脚本。待应用的迁移: {Migrations}",
                        string.Join(", ", pendingMigrations));
                }
            }
            else
            {
                logger.LogInformation("数据库已是最新版本，无需迁移");
            }
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "数据库迁移失败");
            throw;
        }
    }

    /// <summary>
    /// 检查数据库连接和迁移状态
    /// </summary>
    public static async Task<bool> CheckDatabaseHealthAsync(this IHost host)
    {
        using var scope = host.Services.CreateScope();
        var services = scope.ServiceProvider;
        var logger = services.GetRequiredService<ILogger<MessageCenterDbContext>>();

        try
        {
            var context = services.GetRequiredService<MessageCenterDbContext>();
            
            // 检查数据库连接
            var canConnect = await context.Database.CanConnectAsync();
            if (!canConnect)
            {
                logger.LogWarning(
                    "⚠️ 无法连接到数据库。请检查连接字符串配置。\n" +
                    "连接字符串位置: appsettings.json 或 appsettings.Development.json\n" +
                    "请确保:\n" +
                    "  1. PostgreSQL 服务正在运行\n" +
                    "  2. 数据库用户和密码正确\n" +
                    "  3. 数据库已创建\n" +
                    "应用程序将继续启动，但数据库相关功能将不可用。");
                return false;
            }

            // 检查迁移状态
            var pendingMigrations = await context.Database.GetPendingMigrationsAsync();
            if (pendingMigrations.Any())
            {
                logger.LogWarning(
                    "检测到待应用的数据库迁移: {Migrations}",
                    string.Join(", ", pendingMigrations));
            }

            // 获取已应用的迁移
            var appliedMigrations = await context.Database.GetAppliedMigrationsAsync();
            logger.LogInformation(
                "✅ 数据库连接正常。已应用 {Count} 个迁移",
                appliedMigrations.Count());

            return true;
        }
        catch (Exception ex)
        {
            logger.LogWarning(
                ex,
                "⚠️ 数据库健康检查失败: {Message}\n" +
                "请检查:\n" +
                "  1. PostgreSQL 服务是否正在运行\n" +
                "  2. appsettings.Development.json 中的连接字符串是否正确\n" +
                "  3. 数据库用户权限是否正确\n" +
                "应用程序将继续启动，但数据库相关功能将不可用。",
                ex.Message);
            return false;
        }
    }
}
