# MessageCenter Management - 消息中心管理系统

## 项目简介

MessageCenter Management 是一个通用的消息中心管理项目，基于 ABP Framework 8.1.1 和 .NET 8.0 构建。项目体现了"集成、统一、中心化"的设计理念，支持多种消息类型和渠道，适用于各种业务场景。

## 核心特性

### 集成性
- 统一的消息管理接口
- 支持多种消息类型：通知、工作流、警报、事务、营销、社交、系统、实时
- 支持多种消息渠道：站内信、邮件、短信、推送、微信、钉钉、WebSocket、外部渠道

### 统一性
- 统一的消息模型和数据结构
- 统一的消息发送和接收机制
- 统一的消息状态管理
- 统一的消息模板管理

### 中心化
- 集中式消息存储和管理
- 集中式消息发送调度
- 集中式消息统计和分析
- 集中式渠道配置管理

## 技术栈

- **框架**: ABP Framework 8.1.1
- **运行时**: .NET 8.0
- **数据库**: PostgreSQL 8.0.2
- **ORM**: Entity Framework Core 8.0.2
- **日志**: Serilog 8.0.0
- **认证**: JWT Bearer
- **API文档**: Swagger

## 项目结构

```
MessageCenterManagement/
├── src/
│   ├── MessageCenter.Domain.Shared/      # 领域共享层（枚举、常量）
│   ├── MessageCenter.Domain/              # 领域层（实体、仓储接口）
│   ├── MessageCenter.Application.Contracts/ # 应用契约层（DTO、接口）
│   ├── MessageCenter.Application/         # 应用层（服务实现）
│   ├── MessageCenter.EntityFrameworkCore/  # 数据访问层（EF Core）
│   ├── MessageCenter.HttpApi/             # API层（控制器）
│   └── MessageCenter.HttpApi.Host/        # 宿主项目（启动配置）
└── MessageCenterManagement.sln            # 解决方案文件
```

## 核心功能

### 消息管理
- 创建和发送消息（单个/批量）
- 消息查询和筛选
- 消息状态管理（待发送、发送中、已发送、已送达、已读、失败等）
- 消息重试机制
- 消息取消功能

### 消息模板
- 模板创建和管理
- 模板类型支持（文本、HTML、Markdown、JSON）
- 模板变量支持
- 模板启用/禁用

### 消息统计
- 未读消息统计
- 按状态统计
- 按类型统计
- 按渠道统计
- 时间范围统计

### 消息接收
- 接收记录管理
- 已读/未读状态跟踪
- 批量标记已读
- 全部标记已读

## 快速开始

### 前置要求
- .NET 8.0 SDK
- PostgreSQL 数据库
- Visual Studio 2022 或 VS Code

### 配置步骤

1. **配置数据库连接**
   
   编辑 `src/MessageCenter.HttpApi.Host/appsettings.json`，修改数据库连接字符串：
   ```json
   "ConnectionStrings": {
     "Default": "User ID=postgres;Password=postgres;Host=localhost;Port=5432;Database=MessageCenter;Timezone=UTC;"
   }
   ```

2. **运行数据库迁移**
   ```bash
   cd src/MessageCenter.HttpApi.Host
   dotnet ef migrations add InitialCreate --project ../MessageCenter.EntityFrameworkCore
   dotnet ef database update
   ```

3. **运行项目**
   ```bash
   cd src/MessageCenter.HttpApi.Host
   dotnet run
   ```

4. **访问Swagger文档**
   
   打开浏览器访问：`https://localhost:44300/swagger`

## API端点

### 消息管理
- `POST /api/messages` - 创建并发送消息
- `POST /api/messages/batch` - 批量创建并发送消息
- `GET /api/messages/{id}` - 获取消息详情
- `GET /api/messages` - 查询消息列表
- `GET /api/messages/receiver/{receiverId}` - 获取接收者消息列表
- `PUT /api/messages/{id}/read` - 标记消息为已读
- `PUT /api/messages/read/batch` - 批量标记已读
- `PUT /api/messages/read/all/{receiverId}` - 标记所有消息为已读
- `DELETE /api/messages/{id}` - 删除消息
- `GET /api/messages/unread-count/{receiverId}` - 获取未读消息数量
- `GET /api/messages/statistics` - 获取消息统计信息
- `POST /api/messages/{id}/retry` - 重试发送失败的消息
- `POST /api/messages/{id}/cancel` - 取消消息发送

### 消息模板
- `POST /api/message-templates` - 创建消息模板
- `PUT /api/message-templates/{id}` - 更新消息模板
- `GET /api/message-templates/{id}` - 获取消息模板
- `GET /api/message-templates/code/{code}` - 根据代码获取模板
- `GET /api/message-templates` - 获取模板列表
- `DELETE /api/message-templates/{id}` - 删除模板
- `PUT /api/message-templates/{id}/enabled` - 启用/禁用模板

## 消息类型

项目支持以下消息类型：
- **Notification** - 通知消息
- **Workflow** - 工作流消息
- **Alert** - 警报消息
- **Transaction** - 事务消息
- **Marketing** - 营销消息
- **Social** - 社交消息
- **System** - 系统消息
- **Realtime** - 实时消息

## 消息渠道

项目支持以下消息渠道：
- **InApp** - 站内信
- **Email** - 邮件
- **Sms** - 短信
- **Push** - 推送通知
- **WeChat** - 微信
- **DingTalk** - 钉钉
- **WebSocket** - WebSocket实时推送
- **External** - 外部渠道

## 最佳实践

1. **消息创建**
   - 使用模板创建消息以提高效率
   - 合理设置消息优先级
   - 设置合适的过期时间

2. **批量发送**
   - 批量发送数量建议不超过1000条
   - 使用计划发送时间分散发送压力

3. **消息重试**
   - 合理设置最大重试次数
   - 监控失败消息并及时处理

4. **性能优化**
   - 使用索引优化查询性能
   - 定期清理过期消息
   - 使用消息统计功能监控系统状态

## 扩展开发

项目采用分层架构，便于扩展：

1. **添加新的消息渠道**
   - 在 `MessageChannel` 枚举中添加新渠道
   - 实现对应的渠道发送服务
   - 配置渠道参数

2. **添加新的消息类型**
   - 在 `MessageType` 枚举中添加新类型
   - 根据需要扩展消息实体

3. **自定义消息处理**
   - 实现自定义的消息处理服务
   - 注册到依赖注入容器

## 许可证

本项目采用 MIT 许可证。

## 贡献

欢迎提交 Issue 和 Pull Request。
