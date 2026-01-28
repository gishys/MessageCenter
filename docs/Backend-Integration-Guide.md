# 后端模块集成指南

## 概述

本文档说明如何从其他后端模块或微服务中调用 MessageCenter 的消息服务，包括如何引用 NuGet 包、使用 DTO 类、调用应用服务接口等。

## 架构说明

MessageCenter 采用分层架构设计，其他后端模块主要通过以下方式集成：

1. **引用 Application.Contracts 层**：获取服务接口和 DTO 定义
2. **引用 Application 层**：获取应用服务实现（可选，如果通过 HTTP API 调用则不需要）
3. **通过依赖注入使用服务**：在 ABP Framework 模块中注册和使用

## 依赖引用

### 方式一：通过 NuGet 包引用（推荐）

如果 MessageCenter 已发布为 NuGet 包，可以通过以下方式引用：

```xml
<ItemGroup>
  <!-- 引用 Application.Contracts 层（必需）-->
  <PackageReference Include="MessageCenter.Application.Contracts" Version="1.0.0" />
  
  <!-- 引用 Application 层（如果需要在同一进程内调用）-->
  <PackageReference Include="MessageCenter.Application" Version="1.0.0" />
  
  <!-- 引用 Domain.Shared 层（自动包含，包含枚举和常量）-->
  <PackageReference Include="MessageCenter.Domain.Shared" Version="1.0.0" />
</ItemGroup>
```

### 方式二：通过项目引用（开发阶段）

在开发阶段，可以直接引用项目：

```xml
<ItemGroup>
  <ProjectReference Include="..\..\message-center-management\src\MessageCenter.Application.Contracts\MessageCenter.Application.Contracts.csproj" />
  <ProjectReference Include="..\..\message-center-management\src\MessageCenter.Application\MessageCenter.Application.csproj" />
</ItemGroup>
```

### 方式三：通过 HTTP API 调用（微服务架构）

在微服务架构中，可以通过 HTTP API 调用，无需引用 NuGet 包：

```csharp
// 使用 HttpClient 或 ABP 的远程服务调用
var httpClient = _httpClientFactory.CreateClient();
var response = await httpClient.PostAsJsonAsync(
    "https://message-center-api/api/messages", 
    createMessageDto);
```

## 模块注册

如果使用 ABP Framework 并在同一进程内调用，需要在模块中注册依赖：

```csharp
[DependsOn(
    typeof(MessageCenterApplicationContractsModule),
    typeof(MessageCenterApplicationModule)  // 如果直接调用应用服务
)]
public class YourModule : AbpModule
{
    // 模块配置
}
```

## 信息类（DTO）说明

### 1. MessageDto - 消息信息类

消息的完整信息类，用于返回和传递消息数据。

**命名空间**：`MessageCenter.Application.Contracts.DTOs`

**属性说明**：

| 属性 | 类型 | 说明 | 必填 |
|------|------|------|------|
| `Id` | `Guid` | 消息ID | 是 |
| `Title` | `string` | 消息标题 | 是 |
| `Content` | `string` | 消息内容 | 是 |
| `Summary` | `string?` | 消息摘要 | 否 |
| `MessageType` | `MessageType` | 消息类型（枚举） | 是 |
| `Channel` | `MessageChannel` | 消息渠道（枚举） | 是 |
| `Status` | `MessageStatus` | 消息状态（枚举） | 是 |
| `Priority` | `MessagePriority` | 消息优先级（枚举） | 是 |
| `SenderId` | `Guid?` | 发送者ID | 否 |
| `SenderName` | `string?` | 发送者名称 | 否 |
| `ReceiverId` | `string` | 接收者ID | 是 |
| `ReceiverName` | `string?` | 接收者名称 | 否 |
| `ReceiverEmail` | `string?` | 接收者邮箱 | 否 |
| `ReceiverPhone` | `string?` | 接收者手机号 | 否 |
| `TemplateId` | `Guid?` | 模板ID | 否 |
| `BusinessType` | `string?` | 业务类型 | 否 |
| `BusinessId` | `string?` | 业务ID | 否 |
| `ScheduledSendTime` | `DateTime?` | 计划发送时间 | 否 |
| `ActualSendTime` | `DateTime?` | 实际发送时间 | 否 |
| `DeliveredTime` | `DateTime?` | 送达时间 | 否 |
| `ReadTime` | `DateTime?` | 阅读时间 | 否 |
| `ExpirationTime` | `DateTime?` | 过期时间 | 否 |
| `RetryCount` | `int` | 重试次数 | 是 |
| `MaxRetryCount` | `int` | 最大重试次数 | 是 |
| `FailureReason` | `string?` | 失败原因 | 否 |
| `Extension` | `string?` | 扩展属性（JSON格式） | 否 |
| `Tags` | `string?` | 消息标签 | 否 |
| `LinkUrl` | `string?` | 消息链接 | 否 |
| `AttachmentIds` | `string?` | 附件ID列表 | 否 |
| `IsRead` | `bool` | 是否已读 | 是 |
| `CreationTime` | `DateTime` | 创建时间（继承自审计基类） | 是 |
| `LastModificationTime` | `DateTime?` | 最后修改时间 | 否 |
| `CreatorId` | `Guid?` | 创建者ID | 否 |
| `LastModifierId` | `Guid?` | 最后修改者ID | 否 |

**使用示例**：

```csharp
using MessageCenter.Application.Contracts.DTOs;

// 获取消息
var message = await _messageAppService.GetAsync(messageId);

// 使用消息信息
Console.WriteLine($"消息标题: {message.Title}");
Console.WriteLine($"消息内容: {message.Content}");
Console.WriteLine($"接收者: {message.ReceiverName}");
```

### 2. CreateMessageDto - 创建消息信息类

用于创建新消息的输入信息类。

**命名空间**：`MessageCenter.Application.Contracts.DTOs`

**属性说明**：

| 属性 | 类型 | 说明 | 必填 |
|------|------|------|------|
| `Title` | `string` | 消息标题 | 是 |
| `Content` | `string` | 消息内容 | 是 |
| `Summary` | `string?` | 消息摘要 | 否 |
| `MessageType` | `MessageType` | 消息类型（枚举） | 是 |
| `Channel` | `MessageChannel` | 消息渠道（枚举） | 是 |
| `Priority` | `MessagePriority` | 消息优先级（枚举，默认Normal） | 否 |
| `SenderId` | `Guid?` | 发送者ID | 否 |
| `SenderName` | `string?` | 发送者名称 | 否 |
| `ReceiverId` | `string?` | 接收者ID（单个） | 否* |
| `ReceiverIds` | `List<string>?` | 接收者ID列表（批量） | 否* |
| `ReceiverName` | `string?` | 接收者名称 | 否 |
| `ReceiverEmail` | `string?` | 接收者邮箱 | 否 |
| `ReceiverPhone` | `string?` | 接收者手机号 | 否 |
| `TemplateId` | `Guid?` | 模板ID | 否 |
| `TemplateVariables` | `string?` | 模板变量（JSON格式） | 否 |
| `BusinessType` | `string?` | 业务类型 | 否 |
| `BusinessId` | `string?` | 业务ID | 否 |
| `ScheduledSendTime` | `DateTime?` | 计划发送时间 | 否 |
| `ExpirationTime` | `DateTime?` | 过期时间 | 否 |
| `Extension` | `string?` | 扩展属性（JSON格式） | 否 |
| `Tags` | `string?` | 消息标签 | 否 |
| `LinkUrl` | `string?` | 消息链接 | 否 |
| `AttachmentIds` | `List<Guid>?` | 附件ID列表 | 否 |
| `MaxRetryCount` | `int` | 最大重试次数（默认3） | 否 |

*注意：`ReceiverId` 和 `ReceiverIds` 至少需要提供一个。

**使用示例**：

```csharp
using MessageCenter.Application.Contracts.DTOs;
using MessageCenter.Domain.Shared.Enums;

// 创建单个消息
var createDto = new CreateMessageDto
{
    Title = "订单通知",
    Content = "您的订单已发货",
    MessageType = MessageType.Notification,
    Channel = MessageChannel.InApp,
    Priority = MessagePriority.High,
    ReceiverId = "user-123",
    ReceiverName = "张三",
    BusinessType = "Order",
    BusinessId = "order-456",
    LinkUrl = "/orders/456"
};

var message = await _messageAppService.CreateAsync(createDto);

// 批量创建消息
var batchDtos = new List<CreateMessageDto>
{
    new CreateMessageDto
    {
        Title = "系统通知",
        Content = "系统维护通知",
        MessageType = MessageType.System,
        Channel = MessageChannel.InApp,
        ReceiverId = "user-1"
    },
    new CreateMessageDto
    {
        Title = "系统通知",
        Content = "系统维护通知",
        MessageType = MessageType.System,
        Channel = MessageChannel.InApp,
        ReceiverId = "user-2"
    }
};

var messages = await _messageAppService.CreateBatchAsync(batchDtos);
```

### 3. MessageQueryDto - 消息查询信息类

用于查询消息列表的查询条件信息类。

**命名空间**：`MessageCenter.Application.Contracts.DTOs`

**继承自**：`PagedAndSortedResultRequestDto`（ABP Framework 分页基类）

**属性说明**：

| 属性 | 类型 | 说明 | 必填 |
|------|------|------|------|
| `ReceiverId` | `string?` | 接收者ID | 否 |
| `SenderId` | `Guid?` | 发送者ID | 否 |
| `MessageType` | `MessageType?` | 消息类型 | 否 |
| `Channel` | `MessageChannel?` | 消息渠道 | 否 |
| `Status` | `MessageStatus?` | 消息状态 | 否 |
| `Priority` | `MessagePriority?` | 消息优先级 | 否 |
| `BusinessType` | `string?` | 业务类型 | 否 |
| `BusinessId` | `string?` | 业务ID | 否 |
| `IsRead` | `bool?` | 是否已读 | 否 |
| `Keyword` | `string?` | 关键词搜索（标题、内容） | 否 |
| `StartTime` | `DateTime?` | 开始时间 | 否 |
| `EndTime` | `DateTime?` | 结束时间 | 否 |
| `Tags` | `string?` | 标签 | 否 |
| `SkipCount` | `int` | 跳过数量（分页，继承自基类） | 否 |
| `MaxResultCount` | `int` | 最大结果数量（分页，继承自基类） | 否 |
| `Sorting` | `string?` | 排序字段（继承自基类） | 否 |

**使用示例**：

```csharp
using MessageCenter.Application.Contracts.DTOs;
using MessageCenter.Domain.Shared.Enums;
using Volo.Abp.Application.Dtos;

// 查询未读消息
var queryDto = new MessageQueryDto
{
    ReceiverId = "user-123",
    IsRead = false,
    SkipCount = 0,
    MaxResultCount = 20,
    Sorting = "CreationTime DESC"
};

var result = await _messageAppService.GetListAsync(queryDto);

// 查询特定业务类型的消息
var businessQuery = new MessageQueryDto
{
    ReceiverId = "user-123",
    BusinessType = "Order",
    BusinessId = "order-456",
    StartTime = DateTime.Now.AddDays(-7),
    EndTime = DateTime.Now
};

var businessMessages = await _messageAppService.GetListAsync(businessQuery);

// 使用关键词搜索
var searchQuery = new MessageQueryDto
{
    ReceiverId = "user-123",
    Keyword = "订单",
    MessageType = MessageType.Notification,
    SkipCount = 0,
    MaxResultCount = 10
};

var searchResults = await _messageAppService.GetListAsync(searchQuery);
```

### 4. MessageStatisticsDto - 消息统计信息类

用于返回消息统计信息。

**命名空间**：`MessageCenter.Application.Contracts.DTOs`

**属性说明**：

| 属性 | 类型 | 说明 |
|------|------|------|
| `TotalCount` | `long` | 总消息数 |
| `UnreadCount` | `long` | 未读消息数 |
| `ReadCount` | `long` | 已读消息数 |
| `StatusStatistics` | `Dictionary<MessageStatus, long>` | 按状态统计 |
| `TypeStatistics` | `Dictionary<MessageType, long>` | 按类型统计 |
| `ChannelStatistics` | `Dictionary<MessageChannel, long>` | 按渠道统计 |
| `StartTime` | `DateTime?` | 统计时间范围开始 |
| `EndTime` | `DateTime?` | 统计时间范围结束 |

**使用示例**：

```csharp
using MessageCenter.Application.Contracts.DTOs;

// 获取用户消息统计
var statistics = await _messageAppService.GetStatisticsAsync(
    receiverId: "user-123",
    startTime: DateTime.Now.AddDays(-30),
    endTime: DateTime.Now
);

Console.WriteLine($"总消息数: {statistics.TotalCount}");
Console.WriteLine($"未读消息数: {statistics.UnreadCount}");
Console.WriteLine($"已读消息数: {statistics.ReadCount}");

// 按状态统计
foreach (var statusStat in statistics.StatusStatistics)
{
    Console.WriteLine($"{statusStat.Key}: {statusStat.Value}");
}
```

### 5. 枚举类型

所有枚举类型定义在 `MessageCenter.Domain.Shared.Enums` 命名空间中。

#### MessageType - 消息类型

```csharp
public enum MessageType
{
    Notification = 1,  // 通知
    Workflow = 2,      // 工作流
    Alert = 3,         // 告警
    Transaction = 4,   // 交易
    Marketing = 5,     // 营销
    Social = 6,        // 社交
    System = 7,        // 系统
    Realtime = 8       // 实时
}
```

#### MessageChannel - 消息渠道

```csharp
public enum MessageChannel
{
    InApp = 1,      // 站内信
    Email = 2,      // 邮件
    Sms = 3,        // 短信
    Push = 4,       // 推送通知
    WeChat = 5,     // 微信
    DingTalk = 6,   // 钉钉
    WebSocket = 7,  // WebSocket
    External = 8    // 外部渠道
}
```

#### MessageStatus - 消息状态

```csharp
public enum MessageStatus
{
    Pending = 1,    // 待发送
    Sending = 2,    // 发送中
    Sent = 3,       // 已发送
    Delivered = 4,  // 已送达
    Read = 5,       // 已读
    Failed = 6,     // 发送失败
    Cancelled = 7,  // 已取消
    Expired = 8     // 已过期
}
```

#### MessagePriority - 消息优先级

```csharp
public enum MessagePriority
{
    Low = 1,        // 低
    Normal = 2,     // 普通
    High = 3,       // 高
    Urgent = 4      // 紧急
}
```

## 事件订阅说明

MessageCenter 提供了丰富的事件，供其他后端模块订阅以响应消息状态变更。通过事件驱动的方式，可以实现模块间的解耦和异步处理。

### 可用事件列表

| 事件名称 | 说明 | 发布时机 |
|---------|------|---------|
| `MessageCreatedEvent` | 消息创建事件 | 创建新消息时 |
| `MessageBatchCreatedEvent` | 批量消息创建事件 | 批量创建消息时 |
| `MessageReadEvent` | 消息已读事件 | 消息被标记为已读时 |
| `MessageStatusChangedEvent` | 消息状态变更事件 | 消息状态变更时 |
| `MessageDeletedEvent` | 消息删除事件 | 消息被删除时 |
| `MessageFailedEvent` | 消息发送失败事件 | 消息发送失败时 |
| `UnreadCountChangedEvent` | 未读数量变更事件 | 未读消息数量变更时 |

### 事件定义详情

#### 1. MessageCreatedEvent - 消息创建事件

**命名空间**：`MessageCenter.Application.Contracts.Events`

**属性**：
- `Message` (MessageDto): 创建的消息信息
- `ReceiverId` (string): 接收者ID
- `ShouldPushRealtime` (bool): 是否应该实时推送

**使用场景**：
- 记录消息创建日志
- 触发外部系统通知
- 更新相关业务状态

#### 2. MessageBatchCreatedEvent - 批量消息创建事件

**命名空间**：`MessageCenter.Application.Contracts.Events`

**属性**：
- `Messages` (List<MessageDto>): 创建的消息列表
- `ReceiverIds` (List<string>): 接收者ID列表
- `ShouldPushRealtime` (bool): 是否应该实时推送
- `BusinessType` (string?): 业务类型

**使用场景**：
- 批量消息处理
- 统计和分析
- 批量通知外部系统

#### 3. MessageReadEvent - 消息已读事件

**命名空间**：`MessageCenter.Application.Contracts.Events`

**属性**：
- `MessageId` (Guid): 消息ID
- `ReceiverId` (string): 接收者ID
- `Message` (MessageDto?): 消息信息（可选）
- `BusinessType` (string?): 业务类型
- `BusinessId` (string?): 业务ID
- `ReadTime` (DateTime): 阅读时间

**使用场景**：
- 更新业务状态（如订单已查看通知）
- 记录用户行为
- 触发后续业务流程

#### 4. MessageDeletedEvent - 消息删除事件

**命名空间**：`MessageCenter.Application.Contracts.Events`

**属性**：
- `MessageId` (Guid): 消息ID
- `ReceiverId` (string): 接收者ID
- `BusinessType` (string?): 业务类型
- `BusinessId` (string?): 业务ID
- `DeletedTime` (DateTime): 删除时间

**使用场景**：
- 清理相关数据
- 记录删除日志
- 更新统计信息

#### 5. MessageFailedEvent - 消息发送失败事件

**命名空间**：`MessageCenter.Application.Contracts.Events`

**属性**：
- `MessageId` (Guid): 消息ID
- `ReceiverId` (string): 接收者ID
- `FailureReason` (string): 失败原因
- `RetryCount` (int): 重试次数
- `MaxRetryCount` (int): 最大重试次数
- `BusinessType` (string?): 业务类型
- `BusinessId` (string?): 业务ID
- `FailedTime` (DateTime): 失败时间

**使用场景**：
- 发送告警通知
- 记录失败日志
- 触发重试机制

#### 6. MessageStatusChangedEvent - 消息状态变更事件

**命名空间**：`MessageCenter.Application.Contracts.Events`

**属性**：
- `MessageId` (Guid): 消息ID
- `ReceiverId` (string): 接收者ID
- `Status` (string): 新状态
- `ChangedTime` (DateTime): 变更时间

**使用场景**：
- 同步状态到外部系统
- 更新业务状态
- 记录状态变更历史

#### 7. UnreadCountChangedEvent - 未读数量变更事件

**命名空间**：`MessageCenter.Application.Contracts.Events`

**属性**：
- `ReceiverId` (string): 接收者ID
- `UnreadCount` (long): 未读数量

**使用场景**：
- 更新前端未读数量显示
- 触发未读提醒
- 统计和分析

### 如何订阅事件

#### 步骤1：实现事件处理器

创建实现 `IDistributedEventHandler<TEvent>` 接口的类：

```csharp
using MessageCenter.Application.Contracts.Events;
using Volo.Abp.DependencyInjection;
using Volo.Abp.EventBus.Distributed;

public class YourEventHandler : 
    IDistributedEventHandler<MessageReadEvent>, 
    ITransientDependency
{
    public async Task HandleEventAsync(MessageReadEvent eventData)
    {
        // 处理事件逻辑
    }
}
```

#### 步骤2：注册依赖

确保你的模块依赖了 `MessageCenterApplicationContractsModule`：

```csharp
[DependsOn(typeof(MessageCenterApplicationContractsModule))]
public class YourModule : AbpModule
{
    // 模块配置
}
```

#### 步骤3：配置事件总线

确保事件总线已正确配置（如 Redis、RabbitMQ 等），以便跨模块/跨服务通信。

### 事件订阅最佳实践

1. **幂等性**：确保事件处理器是幂等的，可以安全地重复执行
2. **异常处理**：捕获异常并记录日志，避免影响事件总线
3. **性能优化**：避免在事件处理器中执行耗时操作
4. **业务过滤**：根据 `BusinessType` 和 `BusinessId` 过滤相关事件
5. **异步处理**：使用异步方法处理事件

## 服务接口说明

### IMessageAppService - 消息应用服务接口

**命名空间**：`MessageCenter.Application.Contracts.Services`

**主要方法**：

| 方法 | 说明 | 返回类型 |
|------|------|----------|
| `CreateAsync(CreateMessageDto)` | 创建并发送消息 | `Task<MessageDto>` |
| `CreateBatchAsync(List<CreateMessageDto>)` | 批量创建并发送消息 | `Task<List<MessageDto>>` |
| `GetAsync(Guid)` | 根据ID获取消息 | `Task<MessageDto>` |
| `GetListAsync(MessageQueryDto)` | 查询消息列表 | `Task<PagedResultDto<MessageDto>>` |
| `GetReceiverMessagesAsync(string, MessageQueryDto?)` | 获取接收者的消息列表 | `Task<PagedResultDto<MessageDto>>` |
| `MarkAsReadAsync(Guid)` | 标记消息为已读 | `Task` |
| `MarkAsReadBatchAsync(List<Guid>)` | 批量标记消息为已读 | `Task` |
| `MarkAllAsReadAsync(string)` | 标记所有消息为已读 | `Task` |
| `DeleteAsync(Guid)` | 删除消息 | `Task` |
| `DeleteBatchAsync(List<Guid>)` | 批量删除消息 | `Task` |
| `GetUnreadCountAsync(string)` | 获取未读消息数量 | `Task<long>` |
| `GetStatisticsAsync(string?, DateTime?, DateTime?)` | 获取消息统计信息 | `Task<MessageStatisticsDto>` |
| `RetryAsync(Guid)` | 重试发送失败的消息 | `Task` |
| `CancelAsync(Guid)` | 取消消息发送 | `Task` |

## 使用示例

### 示例1：在应用服务中调用消息服务

```csharp
using MessageCenter.Application.Contracts.DTOs;
using MessageCenter.Application.Contracts.Services;
using MessageCenter.Domain.Shared.Enums;
using Volo.Abp.Application.Services;
using Volo.Abp.DependencyInjection;

namespace YourModule.Application.Services;

public class OrderAppService : ApplicationService
{
    private readonly IMessageAppService _messageAppService;

    public OrderAppService(IMessageAppService messageAppService)
    {
        _messageAppService = messageAppService;
    }

    public async Task<OrderDto> CreateOrderAsync(CreateOrderDto input)
    {
        // 创建订单的业务逻辑
        var order = new Order { /* ... */ };
        
        // 发送订单通知消息
        var message = await _messageAppService.CreateAsync(new CreateMessageDto
        {
            Title = "订单创建成功",
            Content = $"您的订单 {order.OrderNumber} 已创建",
            MessageType = MessageType.Notification,
            Channel = MessageChannel.InApp,
            Priority = MessagePriority.Normal,
            ReceiverId = order.UserId.ToString(),
            ReceiverName = order.UserName,
            BusinessType = "Order",
            BusinessId = order.Id.ToString(),
            LinkUrl = $"/orders/{order.Id}"
        });

        return ObjectMapper.Map<Order, OrderDto>(order);
    }

    public async Task MarkOrderAsShippedAsync(Guid orderId)
    {
        // 更新订单状态
        var order = await _orderRepository.GetAsync(orderId);
        order.Status = OrderStatus.Shipped;
        await _orderRepository.UpdateAsync(order);

        // 发送发货通知
        await _messageAppService.CreateAsync(new CreateMessageDto
        {
            Title = "订单已发货",
            Content = $"您的订单 {order.OrderNumber} 已发货，物流单号：{order.TrackingNumber}",
            MessageType = MessageType.Notification,
            Channel = MessageChannel.InApp,
            Priority = MessagePriority.High,
            ReceiverId = order.UserId.ToString(),
            BusinessType = "Order",
            BusinessId = order.Id.ToString(),
            LinkUrl = $"/orders/{order.Id}"
        });
    }
}
```

### 示例2：订阅消息事件（事件驱动方式）

通过订阅 MessageCenter 发布的事件，可以在消息状态变更时自动执行相关业务逻辑。

```csharp
using MessageCenter.Application.Contracts.Events;
using Microsoft.Extensions.Logging;
using Volo.Abp.DependencyInjection;
using Volo.Abp.EventBus.Distributed;

namespace YourModule.Integration.EventHandlers;

/// <summary>
/// 消息已读事件处理器
/// 当用户阅读订单相关消息时，更新订单状态
/// </summary>
public class MessageReadEventHandler : 
    IDistributedEventHandler<MessageReadEvent>, 
    ITransientDependency
{
    private readonly ILogger<MessageReadEventHandler> _logger;
    private readonly IOrderRepository _orderRepository;

    public MessageReadEventHandler(
        ILogger<MessageReadEventHandler> logger,
        IOrderRepository orderRepository)
    {
        _logger = logger;
        _orderRepository = orderRepository;
    }

    public async Task HandleEventAsync(MessageReadEvent eventData)
    {
        try
        {
            // 只处理订单相关的消息
            if (eventData.BusinessType != "Order" || 
                string.IsNullOrEmpty(eventData.BusinessId))
            {
                return;
            }

            // 解析订单ID
            if (!Guid.TryParse(eventData.BusinessId, out var orderId))
            {
                _logger.LogWarning("无法解析订单ID: {BusinessId}", eventData.BusinessId);
                return;
            }

            // 更新订单状态：标记用户已查看订单通知
            var order = await _orderRepository.GetAsync(orderId);
            order.LastNotificationReadTime = eventData.ReadTime;
            await _orderRepository.UpdateAsync(order);

            _logger.LogInformation(
                "用户 {ReceiverId} 已阅读订单 {OrderId} 的消息通知", 
                eventData.ReceiverId, orderId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "处理消息已读事件失败，消息ID: {MessageId}", eventData.MessageId);
            // 不重新抛出异常，避免影响事件总线
        }
    }
}

/// <summary>
/// 消息删除事件处理器
/// 当用户删除消息时，执行相关清理操作
/// </summary>
public class MessageDeletedEventHandler : 
    IDistributedEventHandler<MessageDeletedEvent>, 
    ITransientDependency
{
    private readonly ILogger<MessageDeletedEventHandler> _logger;

    public MessageDeletedEventHandler(ILogger<MessageDeletedEventHandler> logger)
    {
        _logger = logger;
    }

    public async Task HandleEventAsync(MessageDeletedEvent eventData)
    {
        try
        {
            // 记录消息删除日志
            _logger.LogInformation(
                "用户 {ReceiverId} 删除了消息 {MessageId}，业务类型: {BusinessType}, 业务ID: {BusinessId}",
                eventData.ReceiverId, 
                eventData.MessageId, 
                eventData.BusinessType, 
                eventData.BusinessId);

            // 可以在这里执行其他清理操作
            // 例如：清理相关的缓存、更新统计信息等
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "处理消息删除事件失败，消息ID: {MessageId}", eventData.MessageId);
        }
    }
}

/// <summary>
/// 消息发送失败事件处理器
/// 当消息发送失败时，执行告警或重试逻辑
/// </summary>
public class MessageFailedEventHandler : 
    IDistributedEventHandler<MessageFailedEvent>, 
    ITransientDependency
{
    private readonly ILogger<MessageFailedEventHandler> _logger;
    private readonly IAlertService _alertService;

    public MessageFailedEventHandler(
        ILogger<MessageFailedEventHandler> logger,
        IAlertService alertService)
    {
        _logger = logger;
        _alertService = alertService;
    }

    public async Task HandleEventAsync(MessageFailedEvent eventData)
    {
        try
        {
            _logger.LogWarning(
                "消息发送失败，消息ID: {MessageId}, 接收者: {ReceiverId}, 失败原因: {FailureReason}, 重试次数: {RetryCount}/{MaxRetryCount}",
                eventData.MessageId,
                eventData.ReceiverId,
                eventData.FailureReason,
                eventData.RetryCount,
                eventData.MaxRetryCount);

            // 如果达到最大重试次数，发送告警
            if (eventData.RetryCount >= eventData.MaxRetryCount)
            {
                await _alertService.SendAlertAsync(new AlertDto
                {
                    Title = "消息发送失败告警",
                    Content = $"消息 {eventData.MessageId} 发送失败，已达到最大重试次数。失败原因: {eventData.FailureReason}",
                    Severity = AlertSeverity.High,
                    BusinessType = eventData.BusinessType,
                    BusinessId = eventData.BusinessId
                });
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "处理消息失败事件失败，消息ID: {MessageId}", eventData.MessageId);
        }
    }
}
```

### 示例3：在控制器中调用（HTTP API方式）

```csharp
using MessageCenter.Application.Contracts.DTOs;
using MessageCenter.Application.Contracts.Services;
using Microsoft.AspNetCore.Mvc;
using Volo.Abp.AspNetCore.Mvc;

namespace YourModule.HttpApi.Controllers;

[ApiController]
[Route("api/orders")]
public class OrderController : AbpControllerBase
{
    private readonly IMessageAppService _messageAppService;

    public OrderController(IMessageAppService messageAppService)
    {
        _messageAppService = messageAppService;
    }

    [HttpPost]
    public async Task<OrderDto> CreateOrderAsync([FromBody] CreateOrderDto input)
    {
        // 创建订单
        var order = await _orderAppService.CreateAsync(input);

        // 发送通知消息
        await _messageAppService.CreateAsync(new CreateMessageDto
        {
            Title = "订单创建成功",
            Content = $"您的订单已创建，订单号：{order.OrderNumber}",
            MessageType = MessageType.Notification,
            Channel = MessageChannel.InApp,
            ReceiverId = order.UserId.ToString(),
            BusinessType = "Order",
            BusinessId = order.Id.ToString()
        });

        return order;
    }
}
```

### 示例4：查询和统计

```csharp
using MessageCenter.Application.Contracts.DTOs;
using MessageCenter.Application.Contracts.Services;
using MessageCenter.Domain.Shared.Enums;

public class NotificationService
{
    private readonly IMessageAppService _messageAppService;

    public async Task DisplayUserNotificationsAsync(string userId)
    {
        // 获取未读消息数量
        var unreadCount = await _messageAppService.GetUnreadCountAsync(userId);
        Console.WriteLine($"未读消息数: {unreadCount}");

        // 获取未读消息列表
        var unreadMessages = await _messageAppService.GetReceiverMessagesAsync(
            userId,
            new MessageQueryDto
            {
                IsRead = false,
                SkipCount = 0,
                MaxResultCount = 10,
                Sorting = "CreationTime DESC"
            }
        );

        // 获取消息统计
        var statistics = await _messageAppService.GetStatisticsAsync(
            receiverId: userId,
            startTime: DateTime.Now.AddDays(-30),
            endTime: DateTime.Now
        );

        Console.WriteLine($"总消息数: {statistics.TotalCount}");
        Console.WriteLine($"未读: {statistics.UnreadCount}");
        Console.WriteLine($"已读: {statistics.ReadCount}");
    }

    public async Task MarkNotificationAsReadAsync(Guid messageId)
    {
        await _messageAppService.MarkAsReadAsync(messageId);
    }
}
```

## 最佳实践

### 1. 依赖注入

✅ **推荐**：通过构造函数注入服务

```csharp
public class YourService
{
    private readonly IMessageAppService _messageAppService;

    public YourService(IMessageAppService messageAppService)
    {
        _messageAppService = messageAppService;
    }
}
```

❌ **不推荐**：使用服务定位器模式

```csharp
// 不推荐
var messageService = ServiceProvider.GetService<IMessageAppService>();
```

### 2. 异常处理

✅ **推荐**：捕获并处理异常

```csharp
try
{
    var message = await _messageAppService.CreateAsync(createDto);
    // 处理成功情况
}
catch (Exception ex)
{
    _logger.LogError(ex, "创建消息失败");
    // 根据业务需求决定是否重新抛出异常
    // 如果消息发送失败不应该影响主流程，可以只记录日志
}
```

### 3. 业务类型和业务ID

✅ **推荐**：始终设置业务类型和业务ID，便于后续查询和关联

```csharp
var createDto = new CreateMessageDto
{
    // ... 其他属性
    BusinessType = "Order",           // 业务类型
    BusinessId = order.Id.ToString(), // 业务ID
    LinkUrl = $"/orders/{order.Id}"  // 关联链接
};
```

### 4. 批量操作

✅ **推荐**：使用批量方法提高性能

```csharp
// 批量创建消息
var messages = await _messageAppService.CreateBatchAsync(messageList);

// 批量标记已读
await _messageAppService.MarkAsReadBatchAsync(messageIds);
```

### 5. 异步操作

✅ **推荐**：始终使用异步方法

```csharp
// 正确
await _messageAppService.CreateAsync(createDto);

// 错误
_messageAppService.CreateAsync(createDto).Wait();
```

### 6. 消息渠道选择

根据业务场景选择合适的消息渠道：

- **站内信（InApp）**：实时通知，支持实时推送
- **邮件（Email）**：重要通知，需要持久化记录
- **短信（Sms）**：紧急通知，需要立即送达
- **推送通知（Push）**：移动端通知

### 7. 消息优先级

合理设置消息优先级：

- **Low**：一般性通知，不紧急
- **Normal**：常规通知（默认）
- **High**：重要通知，需要用户关注
- **Urgent**：紧急通知，需要立即处理

### 8. 扩展属性

使用 `Extension` 字段存储业务相关的扩展信息（JSON格式）：

```csharp
var extension = new
{
    OrderAmount = 100.00m,
    PaymentMethod = "CreditCard",
    CustomField = "CustomValue"
};

var createDto = new CreateMessageDto
{
    // ... 其他属性
    Extension = JsonSerializer.Serialize(extension)
};
```

### 9. 模板使用

如果消息内容需要动态生成，可以使用模板：

```csharp
var createDto = new CreateMessageDto
{
    TemplateId = templateId,
    TemplateVariables = JsonSerializer.Serialize(new
    {
        UserName = "张三",
        OrderNumber = "ORD-12345",
        Amount = 100.00m
    }),
    ReceiverId = userId
};
```

### 10. 错误处理策略

根据业务需求决定消息发送失败时的处理策略：

- **关键业务**：消息发送失败应该影响主流程，抛出异常
- **非关键业务**：消息发送失败只记录日志，不影响主流程

```csharp
try
{
    await _messageAppService.CreateAsync(createDto);
}
catch (Exception ex)
{
    _logger.LogError(ex, "发送消息失败，但不影响主流程");
    // 不重新抛出异常，允许主流程继续
}
```

## 微服务架构集成

在微服务架构中，如果 MessageCenter 是独立的微服务，可以通过以下方式集成：

### 方式1：HTTP API 调用

```csharp
using System.Net.Http.Json;

public class MessageServiceClient
{
    private readonly HttpClient _httpClient;

    public MessageServiceClient(IHttpClientFactory httpClientFactory)
    {
        _httpClient = httpClientFactory.CreateClient("MessageCenter");
    }

    public async Task<MessageDto> CreateMessageAsync(CreateMessageDto dto)
    {
        var response = await _httpClient.PostAsJsonAsync("/api/messages", dto);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<MessageDto>();
    }
}
```

### 方式2：ABP 远程服务调用

如果使用 ABP Framework 的远程服务功能：

```csharp
// 在 Application.Contracts 中定义远程服务接口
public interface IMessageAppServiceRemote : IRemoteService
{
    Task<MessageDto> CreateAsync(CreateMessageDto input);
    // ... 其他方法
}

// 在调用方注入并使用
public class YourService
{
    private readonly IMessageAppServiceRemote _messageAppService;

    public YourService(IMessageAppServiceRemote messageAppService)
    {
        _messageAppService = messageAppService;
    }

    public async Task ProcessAsync()
    {
        var message = await _messageAppService.CreateAsync(createDto);
    }
}
```

## 注意事项

### 1. 依赖关系

- `Application.Contracts` 层只包含接口和 DTO，不包含实现
- `Application` 层包含服务实现，但需要数据库支持
- 如果只通过 HTTP API 调用，只需引用 `Application.Contracts`

### 2. 数据库依赖

如果直接引用 `Application` 层并在同一进程内调用，需要确保：
- 数据库连接配置正确
- EntityFrameworkCore 模块已正确配置
- 数据库迁移已执行

### 3. 事件总线

MessageCenter 使用事件总线进行实时推送，如果需要在其他模块中监听消息事件，需要：
- 配置相同的事件总线（如 Redis）
- 实现相应的事件处理器

### 4. 多租户支持

如果系统支持多租户，确保：
- 正确设置 `TenantId`
- 消息的租户隔离正确

### 5. 权限控制

MessageCenter 可能包含权限控制，确保：
- 调用方具有相应的权限
- 正确配置授权策略

## 事件驱动集成示例

### 完整示例：订单模块集成

以下是一个完整的示例，展示如何在订单模块中集成 MessageCenter：

```csharp
using MessageCenter.Application.Contracts.DTOs;
using MessageCenter.Application.Contracts.Events;
using MessageCenter.Application.Contracts.Services;
using MessageCenter.Domain.Shared.Enums;
using Microsoft.Extensions.Logging;
using Volo.Abp.DependencyInjection;
using Volo.Abp.EventBus.Distributed;

namespace OrderModule.Integration;

/// <summary>
/// 订单消息服务
/// 负责发送订单相关的消息
/// </summary>
public class OrderMessageService : ITransientDependency
{
    private readonly IMessageAppService _messageAppService;
    private readonly ILogger<OrderMessageService> _logger;

    public OrderMessageService(
        IMessageAppService messageAppService,
        ILogger<OrderMessageService> logger)
    {
        _messageAppService = messageAppService;
        _logger = logger;
    }

    /// <summary>
    /// 发送订单创建通知
    /// </summary>
    public async Task SendOrderCreatedNotificationAsync(Order order)
    {
        try
        {
            await _messageAppService.CreateAsync(new CreateMessageDto
            {
                Title = "订单创建成功",
                Content = $"您的订单 {order.OrderNumber} 已创建，订单金额：{order.TotalAmount:C}",
                MessageType = MessageType.Notification,
                Channel = MessageChannel.InApp,
                Priority = MessagePriority.Normal,
                ReceiverId = order.UserId.ToString(),
                ReceiverName = order.UserName,
                BusinessType = "Order",
                BusinessId = order.Id.ToString(),
                LinkUrl = $"/orders/{order.Id}",
                Tags = "Order,Notification"
            });

            _logger.LogInformation("已发送订单创建通知，订单ID: {OrderId}", order.Id);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "发送订单创建通知失败，订单ID: {OrderId}", order.Id);
            // 不抛出异常，避免影响订单创建流程
        }
    }

    /// <summary>
    /// 发送订单状态变更通知
    /// </summary>
    public async Task SendOrderStatusChangedNotificationAsync(Order order, string oldStatus, string newStatus)
    {
        var priority = newStatus switch
        {
            "Shipped" => MessagePriority.High,
            "Delivered" => MessagePriority.High,
            "Cancelled" => MessagePriority.Normal,
            _ => MessagePriority.Normal
        };

        try
        {
            await _messageAppService.CreateAsync(new CreateMessageDto
            {
                Title = $"订单状态变更：{newStatus}",
                Content = $"您的订单 {order.OrderNumber} 状态已从 {oldStatus} 变更为 {newStatus}",
                MessageType = MessageType.Notification,
                Channel = MessageChannel.InApp,
                Priority = priority,
                ReceiverId = order.UserId.ToString(),
                BusinessType = "Order",
                BusinessId = order.Id.ToString(),
                LinkUrl = $"/orders/{order.Id}"
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "发送订单状态变更通知失败，订单ID: {OrderId}", order.Id);
        }
    }
}

/// <summary>
/// 消息已读事件处理器
/// 当用户阅读订单消息时，更新订单的已读状态
/// </summary>
public class OrderMessageReadEventHandler : 
    IDistributedEventHandler<MessageReadEvent>, 
    ITransientDependency
{
    private readonly IOrderRepository _orderRepository;
    private readonly ILogger<OrderMessageReadEventHandler> _logger;

    public OrderMessageReadEventHandler(
        IOrderRepository orderRepository,
        ILogger<OrderMessageReadEventHandler> logger)
    {
        _orderRepository = orderRepository;
        _logger = logger;
    }

    public async Task HandleEventAsync(MessageReadEvent eventData)
    {
        // 只处理订单相关的消息
        if (eventData.BusinessType != "Order" || string.IsNullOrEmpty(eventData.BusinessId))
        {
            return;
        }

        try
        {
            if (!Guid.TryParse(eventData.BusinessId, out var orderId))
            {
                _logger.LogWarning("无法解析订单ID: {BusinessId}", eventData.BusinessId);
                return;
            }

            var order = await _orderRepository.GetAsync(orderId);
            order.LastNotificationReadTime = eventData.ReadTime;
            await _orderRepository.UpdateAsync(order);

            _logger.LogInformation(
                "用户 {ReceiverId} 已阅读订单 {OrderId} 的消息", 
                eventData.ReceiverId, orderId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "处理订单消息已读事件失败，消息ID: {MessageId}", eventData.MessageId);
        }
    }
}

/// <summary>
/// 消息失败事件处理器
/// 当订单相关消息发送失败时，记录日志并发送告警
/// </summary>
public class OrderMessageFailedEventHandler : 
    IDistributedEventHandler<MessageFailedEvent>, 
    ITransientDependency
{
    private readonly ILogger<OrderMessageFailedEventHandler> _logger;
    private readonly IAlertService _alertService;

    public OrderMessageFailedEventHandler(
        ILogger<OrderMessageFailedEventHandler> logger,
        IAlertService alertService)
    {
        _logger = logger;
        _alertService = alertService;
    }

    public async Task HandleEventAsync(MessageFailedEvent eventData)
    {
        // 只处理订单相关的消息
        if (eventData.BusinessType != "Order")
        {
            return;
        }

        try
        {
            _logger.LogWarning(
                "订单消息发送失败，消息ID: {MessageId}, 订单ID: {OrderId}, 失败原因: {FailureReason}",
                eventData.MessageId,
                eventData.BusinessId,
                eventData.FailureReason);

            // 如果达到最大重试次数，发送告警
            if (eventData.RetryCount >= eventData.MaxRetryCount)
            {
                await _alertService.SendAlertAsync(new AlertDto
                {
                    Title = "订单消息发送失败告警",
                    Content = $"订单 {eventData.BusinessId} 的消息发送失败，已达到最大重试次数",
                    Severity = AlertSeverity.Medium,
                    BusinessType = "Order",
                    BusinessId = eventData.BusinessId
                });
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "处理订单消息失败事件失败，消息ID: {MessageId}", eventData.MessageId);
        }
    }
}
```

## 相关文档

- [分层架构职责说明](./Architecture/Layer-Responsibilities.md)
- [事件驱动架构说明](./Architecture/Event-Driven-Architecture.md)
- [Message Controller API 文档](./API/MessageController-API-Documentation.md)
- [实时通信指南](./API/Realtime-Communication-Guide.md)

## 总结

通过引用 `MessageCenter.Application.Contracts` NuGet 包，其他后端模块可以：

### 1. 服务调用方式
- ✅ 使用完整的 DTO 信息类（MessageDto、CreateMessageDto 等）
- ✅ 调用消息应用服务接口（IMessageAppService）
- ✅ 实现消息的创建、查询、更新、删除等功能
- ✅ 获取消息统计信息
- ✅ 支持批量操作
- ✅ 支持多租户和权限控制

### 2. 事件驱动方式
- ✅ 订阅消息创建事件（MessageCreatedEvent、MessageBatchCreatedEvent）
- ✅ 订阅消息已读事件（MessageReadEvent）
- ✅ 订阅消息删除事件（MessageDeletedEvent）
- ✅ 订阅消息状态变更事件（MessageStatusChangedEvent）
- ✅ 订阅消息失败事件（MessageFailedEvent）
- ✅ 订阅未读数量变更事件（UnreadCountChangedEvent）

### 3. 集成优势
- ✅ **解耦设计**：通过事件总线实现模块间解耦
- ✅ **异步处理**：事件处理不阻塞主流程
- ✅ **可扩展性**：易于添加新的事件处理器
- ✅ **灵活性**：支持同步调用和异步事件两种方式
- ✅ **可维护性**：清晰的接口定义和文档

### 4. 最佳实践
- ✅ 遵循依赖注入规范
- ✅ 实现异常处理和日志记录
- ✅ 使用业务类型和业务ID关联
- ✅ 合理选择消息渠道和优先级
- ✅ 确保事件处理器的幂等性

遵循本文档的最佳实践，可以确保集成过程顺利，代码质量高，维护成本低。无论是通过服务调用还是事件订阅，都能实现与 MessageCenter 的有效集成。
