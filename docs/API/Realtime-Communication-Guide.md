# 实时通信指南

## 概述

MessageCenter 系统集成了 SignalR 实时通信功能，支持消息的实时推送、状态更新通知等功能。本文档介绍如何使用实时通信功能。

## 架构说明

### 技术栈
- **SignalR**: Microsoft ASP.NET Core SignalR 8.0
- **ABP Framework SignalR**: Volo.Abp.AspNetCore.SignalR 8.1.1
- **认证方式**: JWT Bearer Token

### 通信模式

1. **点对点推送**: 向特定用户推送消息
2. **组播推送**: 向特定组（如部门、业务组）推送消息
3. **广播推送**: 向所有连接的客户端推送消息
4. **状态通知**: 轻量级的状态变更通知

## SignalR Hub

### Hub 端点
```
/hubs/messages
```

### 连接方式

#### JavaScript/TypeScript

```javascript
import * as signalR from "@microsoft/signalr";

// 获取访问令牌（从你的认证系统）
const token = "your_jwt_token";

// 创建连接
const connection = new signalR.HubConnectionBuilder()
    .withUrl("https://api.example.com/hubs/messages", {
        accessTokenFactory: () => token,
        // 或者通过查询参数传递
        // skipNegotiation: true,
        // transport: signalR.HttpTransportType.WebSockets
    })
    .withAutomaticReconnect() // 自动重连
    .configureLogging(signalR.LogLevel.Information)
    .build();

// 连接事件
connection.onclose((error) => {
    console.log("连接已关闭", error);
});

connection.onreconnecting((error) => {
    console.log("正在重连...", error);
});

connection.onreconnected((connectionId) => {
    console.log("已重新连接，连接ID:", connectionId);
});

// 启动连接
connection.start()
    .then(() => {
        console.log("SignalR 连接已建立");
    })
    .catch((error) => {
        console.error("连接失败:", error);
    });

// 接收消息
connection.on("ReceiveMessage", (message) => {
    console.log("收到新消息:", message);
    // 处理消息
    displayMessage(message);
});

// 接收新消息通知（轻量级）
connection.on("NotifyNewMessage", (notification) => {
    console.log("有新消息通知:", notification);
    // 更新未读数量
    updateUnreadCount(notification.unreadCount);
});

// 接收消息状态变更通知
connection.on("MessageStatusChanged", (statusInfo) => {
    console.log("消息状态变更:", statusInfo);
    // 更新消息状态
    updateMessageStatus(statusInfo.messageId, statusInfo.status);
});

// 加入组（可选）
connection.invoke("JoinGroup", "department_sales")
    .then(() => console.log("已加入销售部门组"))
    .catch((error) => console.error("加入组失败:", error));

// 离开组
connection.invoke("LeaveGroup", "department_sales")
    .then(() => console.log("已离开销售部门组"))
    .catch((error) => console.error("离开组失败:", error));
```

#### C# 客户端

```csharp
using Microsoft.AspNetCore.SignalR.Client;

public class MessageRealtimeClient
{
    private HubConnection? _connection;
    private readonly string _hubUrl;
    private readonly string _token;

    public MessageRealtimeClient(string hubUrl, string token)
    {
        _hubUrl = hubUrl;
        _token = token;
    }

    public async Task ConnectAsync()
    {
        _connection = new HubConnectionBuilder()
            .WithUrl(_hubUrl, options =>
            {
                options.AccessTokenProvider = () => Task.FromResult(_token)!;
            })
            .WithAutomaticReconnect()
            .Build();

        // 注册事件处理
        _connection.On<MessageDto>("ReceiveMessage", OnReceiveMessage);
        _connection.On<object>("NotifyNewMessage", OnNotifyNewMessage);
        _connection.On<object>("MessageStatusChanged", OnMessageStatusChanged);

        // 连接事件
        _connection.Closed += async (error) =>
        {
            Console.WriteLine($"连接已关闭: {error?.Message}");
            await Task.CompletedTask;
        };

        _connection.Reconnecting += async (error) =>
        {
            Console.WriteLine($"正在重连: {error?.Message}");
            await Task.CompletedTask;
        };

        _connection.Reconnected += async (connectionId) =>
        {
            Console.WriteLine($"已重新连接: {connectionId}");
            await Task.CompletedTask;
        };

        await _connection.StartAsync();
        Console.WriteLine("SignalR 连接已建立");
    }

    private void OnReceiveMessage(MessageDto message)
    {
        Console.WriteLine($"收到新消息: {message.Title}");
        // 处理消息
    }

    private void OnNotifyNewMessage(object notification)
    {
        Console.WriteLine($"有新消息通知: {notification}");
        // 更新未读数量
    }

    private void OnMessageStatusChanged(object statusInfo)
    {
        Console.WriteLine($"消息状态变更: {statusInfo}");
        // 更新消息状态
    }

    public async Task JoinGroupAsync(string groupName)
    {
        if (_connection != null)
        {
            await _connection.InvokeAsync("JoinGroup", groupName);
        }
    }

    public async Task LeaveGroupAsync(string groupName)
    {
        if (_connection != null)
        {
            await _connection.InvokeAsync("LeaveGroup", groupName);
        }
    }

    public async Task DisconnectAsync()
    {
        if (_connection != null)
        {
            await _connection.StopAsync();
            await _connection.DisposeAsync();
        }
    }
}

// 使用示例
var client = new MessageRealtimeClient(
    "https://api.example.com/hubs/messages",
    "your_jwt_token");

await client.ConnectAsync();
await client.JoinGroupAsync("department_sales");
```

## 服务器端推送

### 自动推送场景

系统会在以下场景自动推送实时消息：

1. **创建新消息**: 当通过 API 创建消息时，如果渠道是站内信（InApp），会自动推送给接收者
2. **批量创建消息**: 批量创建消息时，会向所有接收者推送
3. **消息状态变更**: 当消息被标记为已读时，会通知发送者

### 手动推送

如果需要手动触发推送，可以使用 `IMessageRealtimeService`：

```csharp
public class MyService
{
    private readonly IMessageRealtimeService _realtimeService;

    public MyService(IMessageRealtimeService realtimeService)
    {
        _realtimeService = realtimeService;
    }

    // 向单个用户推送
    public async Task SendToUser(string userId, MessageDto message)
    {
        await _realtimeService.SendToUserAsync(userId, message);
    }

    // 向多个用户推送
    public async Task SendToUsers(List<string> userIds, MessageDto message)
    {
        await _realtimeService.SendToUsersAsync(userIds, message);
    }

    // 向组推送
    public async Task SendToGroup(string groupName, MessageDto message)
    {
        await _realtimeService.SendToGroupAsync(groupName, message);
    }

    // 广播
    public async Task Broadcast(MessageDto message)
    {
        await _realtimeService.BroadcastAsync(message);
    }
}
```

## 事件说明

### ReceiveMessage
完整消息推送事件，包含完整的消息对象。

**触发时机**:
- 创建新消息时
- 手动调用推送服务时

**数据格式**:
```json
{
  "id": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
  "title": "系统通知",
  "content": "这是一条测试消息",
  "messageType": 1,
  "channel": 1,
  "status": 1,
  "receiverId": "user123",
  "creationTime": "2024-01-01T09:00:00Z"
}
```

### NotifyNewMessage
轻量级新消息通知，仅包含未读数量。

**触发时机**:
- 有新消息时（不推送完整消息内容）
- 需要更新未读数量时

**数据格式**:
```json
{
  "receiverId": "user123",
  "unreadCount": 5,
  "timestamp": "2024-01-01T09:00:00Z"
}
```

### MessageStatusChanged
消息状态变更通知。

**触发时机**:
- 消息被标记为已读时
- 消息状态变更时

**数据格式**:
```json
{
  "messageId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
  "status": "Read",
  "timestamp": "2024-01-01T09:00:00Z"
}
```

## 客户端方法

### JoinGroup
加入指定的组，用于接收组播消息。

```javascript
connection.invoke("JoinGroup", "department_sales");
```

### LeaveGroup
离开指定的组。

```javascript
connection.invoke("LeaveGroup", "department_sales");
```

## 认证

SignalR 连接使用 JWT Bearer Token 进行认证。可以通过以下方式传递 token：

1. **查询参数**: `?access_token=your_token`
2. **请求头**: `Authorization: Bearer your_token`（需要配置）

### 获取连接信息

可以通过 API 获取连接信息：

```http
GET /api/messages/realtime/info
Authorization: Bearer your_jwt_token
```

响应：
```json
{
  "hubUrl": "https://api.example.com/hubs/messages",
  "accessToken": "your_jwt_token",
  "supportedMethods": [
    "ReceiveMessage",
    "NotifyNewMessage",
    "MessageStatusChanged"
  ]
}
```

## 最佳实践

### 1. 连接管理
- 使用自动重连功能
- 在应用启动时建立连接
- 在应用关闭时断开连接
- 处理网络异常情况

### 2. 错误处理
```javascript
connection.onclose((error) => {
    if (error) {
        console.error("连接错误:", error);
        // 实现重连逻辑
        setTimeout(() => {
            connection.start();
        }, 5000);
    }
});
```

### 3. 性能优化
- 使用轻量级通知（NotifyNewMessage）而不是完整消息推送
- 按需加载消息详情
- 实现消息缓存机制

### 4. 安全性
- 始终使用 HTTPS
- 定期刷新访问令牌
- 验证消息来源

### 5. 用户体验
- 显示连接状态指示器
- 提供离线消息队列
- 实现消息同步机制

## 故障排查

### 连接失败
1. 检查网络连接
2. 验证 JWT Token 是否有效
3. 检查 CORS 配置
4. 查看服务器日志

### 消息未收到
1. 确认用户已连接到 Hub
2. 检查用户是否在正确的组中
3. 验证消息渠道是否为 InApp
4. 查看服务器推送日志

### 性能问题
1. 减少不必要的完整消息推送
2. 使用组播代替广播
3. 实现消息批处理
4. 监控连接数量

## 示例项目

完整的示例代码请参考：
- [JavaScript 示例](./examples/realtime-client.js)
- [C# 客户端示例](./examples/RealtimeClient.cs)
- [React 示例](./examples/RealtimeHook.tsx)

## 相关文档

- [SignalR 官方文档](https://docs.microsoft.com/aspnet/core/signalr)
- [ABP Framework SignalR 文档](https://docs.abp.io/en/abp/latest/SignalR-Integration)
- [Message Controller API 文档](./MessageController-API-Documentation.md)
