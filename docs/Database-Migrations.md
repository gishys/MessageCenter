# 数据库迁移指南

## 概述

本文档介绍如何使用 Entity Framework Core 进行数据库迁移，遵循 ABP Framework 最佳实践。

## 前置要求

- .NET 8.0 SDK
- PostgreSQL 数据库
- Entity Framework Core Tools（已包含在项目中）

## 项目结构

```
MessageCenter.EntityFrameworkCore/
├── MessageCenterDbContext.cs              # DbContext 定义
├── MessageCenterDbContextFactory.cs        # 迁移工具使用的工厂类
├── MessageCenterEntityFrameworkCoreModule.cs
└── Migrations/                             # 迁移文件目录（自动生成）
    └── [迁移文件]
```

## 快速开始

### 1. 配置数据库连接

编辑 `src/MessageCenter.HttpApi.Host/appsettings.json`：

```json
{
  "ConnectionStrings": {
    "Default": "User ID=postgres;Password=postgres;Host=localhost;Port=5432;Database=MessageCenter;Timezone=UTC;"
  }
}
```

### 2. 创建初始迁移

#### Windows (PowerShell)
```powershell
.\scripts\migrate-database.ps1 -MigrationName "InitialCreate"
```

#### Linux/macOS (Bash)
```bash
chmod +x scripts/migrate-database.sh
./scripts/migrate-database.sh --name InitialCreate
```

#### 手动命令
```bash
cd src/MessageCenter.HttpApi.Host
dotnet ef migrations add InitialCreate --project ../MessageCenter.EntityFrameworkCore
```

### 3. 应用迁移到数据库

#### 使用脚本
```powershell
# Windows
.\scripts\migrate-database.ps1 -Update

# Linux/macOS
./scripts/migrate-database.sh --update
```

#### 手动命令
```bash
cd src/MessageCenter.HttpApi.Host
dotnet ef database update --project ../MessageCenter.EntityFrameworkCore
```

## 常用操作

### 创建新迁移

当您修改了实体类或 DbContext 配置后，需要创建新的迁移：

```powershell
# 使用脚本
.\scripts\migrate-database.ps1 -MigrationName "AddMessageTags"

# 手动命令
dotnet ef migrations add AddMessageTags --project ../MessageCenter.EntityFrameworkCore --startup-project .
```

### 查看所有迁移

```powershell
# 使用脚本
.\scripts\migrate-database.ps1 -List

# 手动命令
dotnet ef migrations list --project ../MessageCenter.EntityFrameworkCore --startup-project .
```

### 删除最后一个迁移

如果迁移还未应用到数据库，可以删除：

```powershell
# 使用脚本
.\scripts\migrate-database.ps1 -Remove

# 手动命令
dotnet ef migrations remove --project ../MessageCenter.EntityFrameworkCore --startup-project .
```

### 生成 SQL 脚本

生成迁移的 SQL 脚本，用于手动执行或审查：

```powershell
# 使用脚本
.\scripts\migrate-database.ps1 -Script

# 手动命令
dotnet ef migrations script --project ../MessageCenter.EntityFrameworkCore --startup-project . --output scripts/migrations/migration-script.sql
```

### 回滚到指定迁移

```bash
dotnet ef database update <迁移名称> --project ../MessageCenter.EntityFrameworkCore --startup-project .
```

例如，回滚到 `InitialCreate` 迁移：
```bash
dotnet ef database update InitialCreate --project ../MessageCenter.EntityFrameworkCore --startup-project .
```

### 应用所有待处理的迁移

```bash
dotnet ef database update --project ../MessageCenter.EntityFrameworkCore --startup-project .
```

## 迁移文件说明

迁移文件位于 `src/MessageCenter.EntityFrameworkCore/Migrations/` 目录下，包含：

- `[时间戳]_[迁移名称].cs` - 迁移的 C# 代码
- `MessageCenterDbContextModelSnapshot.cs` - 当前数据库模型的快照

### 迁移文件示例

```csharp
public partial class InitialCreate : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        // 创建表的 SQL
        migrationBuilder.CreateTable(
            name: "MsgMessages",
            columns: table => new
            {
                Id = table.Column<Guid>(type: "uuid", nullable: false),
                // ... 其他列
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_MsgMessages", x => x.Id);
            });
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        // 回滚的 SQL
        migrationBuilder.DropTable(name: "MsgMessages");
    }
}
```

## 最佳实践

### 1. 迁移命名规范

使用描述性的迁移名称，清晰说明迁移的目的：

- ✅ `AddMessageTags` - 添加消息标签功能
- ✅ `UpdateMessageStatusEnum` - 更新消息状态枚举
- ✅ `AddMessageTemplateIndex` - 添加消息模板索引
- ❌ `Migration1` - 不推荐
- ❌ `Update` - 不推荐

### 2. 迁移前检查

在创建迁移前，确保：
- ✅ 所有实体变更已完成
- ✅ DbContext 配置已更新
- ✅ 代码可以正常编译
- ✅ 已备份数据库（生产环境）

### 3. 迁移审查

创建迁移后，应该：
- ✅ 检查生成的迁移代码
- ✅ 验证 SQL 语句的正确性
- ✅ 在开发环境测试迁移
- ✅ 生成 SQL 脚本进行审查

### 4. 生产环境部署

在生产环境应用迁移时：
- ✅ 在非高峰时段执行
- ✅ 先备份数据库
- ✅ 使用 SQL 脚本而非直接执行 `database update`
- ✅ 监控迁移执行过程
- ✅ 准备回滚方案

### 5. 多环境管理

不同环境使用不同的迁移策略：

**开发环境**
- 直接使用 `dotnet ef database update`
- 可以频繁创建和删除迁移

**测试环境**
- 使用 SQL 脚本
- 验证迁移的正确性

**生产环境**
- 使用 SQL 脚本
- 需要 DBA 审查
- 记录迁移日志

## 常见问题

### Q1: 迁移工具找不到 DbContext

**问题**: `Unable to create an object of type 'MessageCenterDbContext'`

**解决方案**: 
确保 `MessageCenterDbContextFactory` 类存在且配置正确。该类用于迁移工具创建 DbContext 实例。

### Q2: 连接字符串错误

**问题**: `A network-related or instance-specific error occurred`

**解决方案**:
1. 检查 `appsettings.json` 中的连接字符串
2. 确保数据库服务正在运行
3. 验证数据库用户权限

### Q3: 迁移冲突

**问题**: 多个开发者创建了不同的迁移

**解决方案**:
1. 合并迁移：删除冲突的迁移，重新创建
2. 使用 Git 协调迁移顺序
3. 团队协作时，指定专人负责迁移

### Q4: 迁移回滚失败

**问题**: 回滚时出现错误

**解决方案**:
1. 检查迁移的 `Down` 方法是否正确
2. 手动修复数据库状态
3. 创建新的迁移修复问题

## 自动化迁移

### CI/CD 集成

在 CI/CD 流程中自动应用迁移：

```yaml
# Azure DevOps / GitHub Actions 示例
- name: Apply Database Migrations
  run: |
    cd src/MessageCenter.HttpApi.Host
    dotnet ef database update --project ../MessageCenter.EntityFrameworkCore --startup-project . --no-build
```

### 应用启动时自动迁移

在 `Program.cs` 中添加自动迁移（仅用于开发环境）：

```csharp
if (env.IsDevelopment())
{
    using (var scope = app.Services.CreateScope())
    {
        var context = scope.ServiceProvider.GetRequiredService<MessageCenterDbContext>();
        context.Database.Migrate();
    }
}
```

⚠️ **注意**: 生产环境不建议使用自动迁移，应该手动执行迁移脚本。

## 相关文档

- [Entity Framework Core 迁移文档](https://docs.microsoft.com/ef/core/managing-schemas/migrations/)
- [ABP Framework 文档](https://docs.abp.io/en/abp/latest/Entity-Framework-Core)
- [PostgreSQL 文档](https://www.postgresql.org/docs/)

## 迁移脚本位置

- PowerShell 脚本: `scripts/migrate-database.ps1`
- Bash 脚本: `scripts/migrate-database.sh`
- SQL 脚本输出: `scripts/migrations/`
