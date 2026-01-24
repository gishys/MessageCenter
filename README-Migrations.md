# 数据库迁移快速指南

## 快速命令

### 创建初始迁移
```bash
# Windows PowerShell
.\scripts\migrate-database.ps1 -MigrationName "InitialCreate"

# Linux/macOS
./scripts/migrate-database.sh --name InitialCreate
```

### 应用迁移
```bash
# Windows PowerShell
.\scripts\migrate-database.ps1 -Update

# Linux/macOS
./scripts/migrate-database.sh --update
```

### 查看所有迁移
```bash
.\scripts\migrate-database.ps1 -List
```

### 生成 SQL 脚本
```bash
.\scripts\migrate-database.ps1 -Script
```

## 详细文档

请参阅 [docs/Database-Migrations.md](docs/Database-Migrations.md) 获取完整的迁移指南。
