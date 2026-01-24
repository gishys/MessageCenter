# 分层架构职责说明

## 概述

MessageCenter 系统采用清晰的分层架构，每个层都有明确的职责和依赖关系。

## 分层结构

```
┌─────────────────────────────────────────────────────────┐
│              MessageCenter.HttpApi.Host                   │
│  (应用程序入口，配置和启动)                                  │
└──────────────────────┬──────────────────────────────────┘
                       │
┌──────────────────────▼──────────────────────────────────┐
│              MessageCenter.HttpApi                       │
│  (HTTP API 层：控制器、Hubs)                               │
└──────────────────────┬──────────────────────────────────┘
                       │
┌──────────────────────▼──────────────────────────────────┐
│              MessageCenter.Integration                   │
│  (集成层：事件处理器、外部服务集成)                          │
└──────────────────────┬──────────────────────────────────┘
                       │
┌──────────────────────▼──────────────────────────────────┐
│              MessageCenter.Application                  │
│  (应用层：业务逻辑、应用服务)                              │
└──────────────────────┬──────────────────────────────────┘
                       │
┌──────────────────────▼──────────────────────────────────┐
│              MessageCenter.Domain                       │
│  (领域层：实体、领域服务、仓储接口)                         │
└──────────────────────┬──────────────────────────────────┘
                       │
┌──────────────────────▼──────────────────────────────────┐
│              MessageCenter.EntityFrameworkCore          │
│  (数据访问层：仓储实现、DbContext)                         │
└─────────────────────────────────────────────────────────┘
```

## 各层职责

### 1. MessageCenter.Domain.Shared
**职责**：
- 定义共享的枚举、常量、DTO 基类
- 不依赖任何其他层
- 可被所有层引用

**包含内容**：
- 枚举类型（MessageType, MessageStatus, MessageChannel 等）
- 常量定义（MessageCenterConsts）
- 共享的基础类型

### 2. MessageCenter.Domain
**职责**：
- 定义领域实体和值对象
- 定义仓储接口
- 定义领域服务接口
- 包含核心业务规则

**包含内容**：
- 实体类（Message, MessageTemplate, MessageReceipt 等）
- 仓储接口（IMessageRepository, IMessageTemplateRepository 等）
- 领域服务接口

**依赖**：
- MessageCenter.Domain.Shared

### 3. MessageCenter.Application.Contracts
**职责**：
- 定义应用服务接口
- 定义 DTO
- 定义应用层的事件定义

**包含内容**：
- 应用服务接口（IMessageAppService, IMessageTemplateAppService 等）
- DTO 类（MessageDto, CreateMessageDto 等）
- 事件定义（MessageCreatedEvent 等）

**依赖**：
- MessageCenter.Domain.Shared
- MessageCenter.Domain

### 4. MessageCenter.Application
**职责**：
- 实现应用服务
- 实现业务逻辑
- 发布领域事件
- 协调领域层和基础设施层

**包含内容**：
- 应用服务实现（MessageAppService, MessageTemplateAppService 等）
- AutoMapper 配置
- 业务逻辑验证

**依赖**：
- MessageCenter.Domain
- MessageCenter.Application.Contracts

**不依赖**：
- ❌ 基础设施技术（SignalR、邮件服务等）
- ❌ HTTP 相关技术

### 5. MessageCenter.EntityFrameworkCore
**职责**：
- 实现数据访问
- 实现仓储
- 配置数据库映射
- 数据库迁移

**包含内容**：
- DbContext 实现
- 仓储实现（MessageRepository, MessageTemplateRepository 等）
- 实体配置

**依赖**：
- MessageCenter.Domain
- MessageCenter.Application.Contracts

### 6. MessageCenter.HttpApi
**职责**：
- 定义 HTTP API 端点
- 实现控制器
- HTTP 相关的配置

**包含内容**：
- 控制器（MessageController, MessageTemplateController）
- HTTP 相关的服务实现

**依赖**：
- MessageCenter.Application.Contracts

**不包含**：
- ❌ 事件处理器（应放在 Integration 层）
- ❌ SignalR Hubs（应放在 Integration 层）

### 7. MessageCenter.Integration ⭐ 新增
**职责**：
- 处理跨层集成
- 实现事件处理器
- 实现基础设施组件（SignalR Hubs 等）
- 集成外部服务（邮件、短信、推送等）
- 将业务事件转换为基础设施调用

**包含内容**：
- 事件处理器（MessageCreatedEventHandler 等）
- SignalR Hubs（MessageHub）
- 外部服务集成（未来可扩展）

**依赖**：
- MessageCenter.Application.Contracts（事件定义）

**不依赖**：
- ❌ MessageCenter.HttpApi（避免循环依赖）

**设计原则**：
- ✅ 负责将业务事件转换为基础设施调用
- ✅ 包含所有基础设施组件（Hubs、外部服务等）
- ✅ 可以依赖基础设施技术（SignalR、邮件服务等）
- ✅ 保持应用层与基础设施的解耦

### 8. MessageCenter.HttpApi.Host
**职责**：
- 应用程序入口点
- 配置依赖注入
- 配置中间件
- 启动应用程序

**包含内容**：
- Program.cs
- appsettings.json
- 模块配置

**依赖**：
- 所有其他层

## 事件处理器位置说明

### ❌ 错误做法：放在 HttpApi 层
```
HttpApi 层
├── Controllers/          ✅ 正确
├── Hubs/                ✅ 正确
└── EventHandlers/       ❌ 不合适
```

**问题**：
- HttpApi 层应该只负责 HTTP 相关的功能
- 事件处理器属于跨层集成，不应该放在 HttpApi 层
- 违反了单一职责原则

### ✅ 正确做法：放在 Integration 层
```
Integration 层
└── EventHandlers/       ✅ 正确
    ├── MessageCreatedEventHandler
    ├── MessageStatusChangedEventHandler
    └── UnreadCountChangedEventHandler
```

**优势**：
- ✅ 清晰的职责分离
- ✅ 符合 DDD 分层架构原则
- ✅ 便于扩展其他集成功能（邮件、短信等）
- ✅ 保持应用层与基础设施的解耦

## 依赖关系规则

### 依赖方向
```
HttpApi.Host
    ↓
HttpApi → Application.Contracts
    ↓
Integration → Application.Contracts
    ↓
Application → Domain → Domain.Shared
    ↓
EntityFrameworkCore → Domain
```

### 禁止的依赖
- ❌ Application 层不能依赖 HttpApi 层
- ❌ Application 层不能依赖 Integration 层
- ❌ Domain 层不能依赖 Application 层
- ❌ Domain.Shared 不能依赖任何其他层

### 允许的依赖
- ✅ Integration 层可以依赖 HttpApi 层（访问 Hubs）
- ✅ Integration 层可以依赖 Application.Contracts（访问事件定义）
- ✅ HttpApi 层可以依赖 Application.Contracts（访问服务接口）
- ✅ Application 层可以依赖 Domain 层

## 最佳实践

### 1. 事件处理器
- ✅ 放在 Integration 层
- ✅ 负责将业务事件转换为基础设施调用
- ✅ 可以依赖基础设施技术

### 2. 应用服务
- ✅ 放在 Application 层
- ✅ 不依赖基础设施
- ✅ 通过事件总线发布事件

### 3. 控制器
- ✅ 放在 HttpApi 层
- ✅ 只负责 HTTP 请求处理
- ✅ 调用应用服务

### 4. Hubs
- ✅ 放在 Integration 层
- ✅ 负责 SignalR 连接管理
- ✅ 作为基础设施组件
- ✅ 被事件处理器使用（通过 IHubContext）

## 扩展场景

### 未来可能的扩展
1. **邮件服务集成**：放在 Integration 层
2. **短信服务集成**：放在 Integration 层
3. **推送通知集成**：放在 Integration 层
4. **第三方 API 集成**：放在 Integration 层

所有这些集成都可以通过事件处理器实现，保持应用层的纯净。

## 相关文档

- [事件驱动架构](./Event-Driven-Architecture.md)
- [架构优化说明](../Architecture-Improvements.md)
