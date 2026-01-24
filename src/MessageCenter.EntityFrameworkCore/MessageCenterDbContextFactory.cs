using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace MessageCenter.EntityFrameworkCore;

/// <summary>
/// DbContext 工厂类
/// 用于 Entity Framework Core 迁移工具（如 dotnet ef migrations add）
/// 此工厂类允许 EF Core 工具在运行时创建 DbContext 实例
/// </summary>
public class MessageCenterDbContextFactory : IDesignTimeDbContextFactory<MessageCenterDbContext>
{
    public MessageCenterDbContext CreateDbContext(string[] args)
    {
        // 构建配置
        var configuration = BuildConfiguration();

        // 构建 DbContextOptions
        var builder = new DbContextOptionsBuilder<MessageCenterDbContext>()
            .UseNpgsql(configuration.GetConnectionString("Default"));

        return new MessageCenterDbContext(builder.Options);
    }

    private static IConfigurationRoot BuildConfiguration()
    {
        var basePath = Path.Combine(Directory.GetCurrentDirectory(), "../MessageCenter.HttpApi.Host");
        var builder = new ConfigurationBuilder()
            .SetBasePath(basePath)
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
        
        // 只有在文件存在时才加载 appsettings.Development.json
        // 这样可以避免覆盖 appsettings.json 中的配置
        var devConfigPath = Path.Combine(basePath, "appsettings.Development.json");
        if (File.Exists(devConfigPath))
        {
            builder.AddJsonFile("appsettings.Development.json", optional: true, reloadOnChange: true);
        }

        return builder.Build();
    }
}
