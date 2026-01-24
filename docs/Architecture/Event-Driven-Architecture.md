# 事件驱动架构说明

## 概述

MessageCenter 系统采用事件驱动架构（Event-Driven Architecture）来实现实时消息推送，确保 Application 层和 HttpApi 层的解耦。

## 架构设计

### 设计原则

1. **分层解耦**: Application 层不直接依赖 SignalR，避免层间耦合
2. **事件驱动**: 通过事件总线发布事件，由事件处理器异步处理
3. **职责分离**: Application 层负责业务逻辑，HttpApi 层负责实时推送
4. **容错设计**: 推送失败不影响主业务流程

### 架构图

```
┌─────────────────────────────────────────────────────────┐
│              MessageAppService (Application层)            │
│  - 创建消息                                               │
│  - 业务逻辑验证                                            │
│  - 发布事件                                                │
└──────────────────────┬──────────────────────────────────┘
                       │
                       │ 发布事件
                       │
┌──────────────────────▼──────────────────────────────────┐
│            DistributedEventBus (事件总线)                │
│  - MessageCreatedEvent                                   │
│  - MessageStatusChangedEvent                             │
│  - UnreadCountChangedEvent                               │
└──────────────────────┬──────────────────────────────────┘
                       │
                       │ 事件分发
                       │
┌──────────────────────▼──────────────────────────────────┐
│         EventHandlers (HttpApi层)                        │
│  - MessageCreatedEventHandler                            │
│  - MessageStatusChangedEventHandler                      │
│  - UnreadCountChangedEventHandler                        │
└──────────────────────┬──────────────────────────────────┘
                       │
                       │ 调用 SignalR
                       │
┌──────────────────────▼──────────────────────────────────┐
│              MessageHub (SignalR Hub)                    │
│  - 实时推送消息                                            │
│  - 管理客户端连接                                          │
└─────────────────────────────────────────────────────────┘
```

## 事件定义

### MessageCreatedEvent

消息创建事件，当消息创建时发布。

**属性**:
- `Message`: 消息DTO
- `ReceiverId`: 接收者ID（或组名称，或"broadcast"）
- `ShouldPushRealtime`: 是否应该实时推送

**发布时机**:
- 创建新消息时
- 批量创建消息时

### MessageStatusChangedEvent

消息状态变更事件。

**属性**:
- `MessageId`: 消息ID
- `ReceiverId`: 接收者ID
- `Status`: 新状态
- `ChangedTime`: 变更时间

**发布时机**:
- 消息被标记为已读时
- 消息状态变更时

### UnreadCountChangedEvent

未读数量变更事件。

**属性**:
- `ReceiverId`: 接收者ID
- `UnreadCount`: 未读数量

**发布时机**:
- 有新消息时
- 消息被标记为已读时

## 事件处理器

### MessageCreatedEventHandler

处理消息创建事件，执行 SignalR 推送。

**处理逻辑**:
1. 检查是否应该推送
2. 根据接收者类型选择推送方式：
   - `broadcast`: 广播
   - `group_*`, `department_*`, `business_*`: 组播
   - 其他: 点对点推送

### MessageStatusChangedEventHandler

处理消息状态变更事件，推送状态更新通知。

### UnreadCountChangedEventHandler

处理未读数量变更事件，推送未读数量更新。

## 实现优势

### 1. 解耦设计
- Application 层不依赖 SignalR
- 可以轻松替换推送实现
- 支持多种推送方式（SignalR、WebSocket、SSE等）

### 2. 可扩展性
- 可以添加多个事件处理器
- 支持事件过滤和路由
- 支持事件重试和失败处理

### 3. 可测试性
- Application 层可以独立测试
- 事件处理器可以独立测试
- 可以模拟事件总线进行集成测试

### 4. 性能优化
- 异步事件处理，不阻塞主流程
- 支持事件批处理
- 支持事件优先级

## 使用示例

### Application 层发布事件

```csharp
// 在 MessageRealtimeService 中
await _distributedEventBus.PublishAsync(new MessageCreatedEvent
{
    Message = messageDto,
    ReceiverId = receiverId,
    ShouldPushRealtime = true
});
```

### HttpApi 层处理事件

```csharp
// 在 MessageCreatedEventHandler 中
public async Task HandleEventAsync(MessageCreatedEvent eventData)
{
    if (!eventData.ShouldPushRealtime) return;
    
    // 执行实际的 SignalR 推送
    await _hubContext.Clients.Group($"user_{eventData.ReceiverId}")
        .SendAsync("ReceiveMessage", eventData.Message);
}
```

## 最佳实践

### 1. 事件设计
- ✅ 事件应该是不可变的
- ✅ 事件应该包含足够的信息
- ✅ 事件应该轻量级

### 2. 事件处理
- ✅ 事件处理器应该是幂等的
- ✅ 事件处理器应该快速执行
- ✅ 事件处理器应该处理异常

### 3. 性能优化
- ✅ 使用异步处理
- ✅ 避免在事件处理器中执行耗时操作
- ✅ 使用批处理优化

### 4. 监控和日志
- ✅ 记录事件发布和处理日志
- ✅ 监控事件处理延迟
- ✅ 监控事件处理失败率

## 故障处理

### 事件处理失败
- 事件处理器捕获异常，不重新抛出
- 记录错误日志
- 可以配置重试策略

### 事件丢失
- 使用持久化事件总线（如 RabbitMQ、Azure Service Bus）
- 实现事件重放机制
- 监控事件处理状态

## 扩展场景

### 1. 多服务器部署
使用 Redis 或 RabbitMQ 作为事件总线后端，支持多服务器部署。

### 2. 事件溯源
可以将所有事件持久化，实现事件溯源和审计。

### 3. CQRS 模式
事件可以用于更新读模型，实现 CQRS 模式。

## 相关文档

- [ABP Framework 事件总线文档](https://docs.abp.io/en/abp/latest/Event-Bus)
- [实时通信指南](../API/Realtime-Communication-Guide.md)
- [架构优化说明](../Architecture-Improvements.md)
