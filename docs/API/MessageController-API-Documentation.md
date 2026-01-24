# Message Controller API 文档

## 概述

Message Controller 提供消息中心管理的完整 API 接口，支持消息的创建、查询、更新、删除以及统计等功能。

**基础路径**: `/api/messages`

**认证方式**: JWT Bearer Token

---

## 目录

- [认证](#认证)
- [数据模型](#数据模型)
- [枚举类型](#枚举类型)
- [API 端点](#api-端点)
  - [消息管理](#消息管理)
  - [消息查询](#消息查询)
  - [消息状态操作](#消息状态操作)
  - [消息统计](#消息统计)
- [错误处理](#错误处理)
- [示例代码](#示例代码)

---

## 认证

所有 API 端点都需要在请求头中包含有效的 JWT Bearer Token：

```
Authorization: Bearer {your_jwt_token}
```

---

## 数据模型

### CreateMessageDto

创建消息的请求模型。

| 字段 | 类型 | 必填 | 说明 |
|------|------|------|------|
| title | string | 是 | 消息标题 |
| content | string | 是 | 消息内容 |
| summary | string | 否 | 消息摘要 |
| messageType | MessageType | 是 | 消息类型（枚举） |
| channel | MessageChannel | 是 | 消息渠道（枚举） |
| priority | MessagePriority | 否 | 消息优先级，默认 Normal |
| senderId | Guid? | 否 | 发送者ID |
| senderName | string? | 否 | 发送者名称 |
| receiverId | string? | 否 | 接收者ID（单个） |
| receiverIds | List<string>? | 否 | 接收者ID列表（批量） |
| receiverName | string? | 否 | 接收者名称 |
| receiverEmail | string? | 否 | 接收者邮箱（用于邮件渠道） |
| receiverPhone | string? | 否 | 接收者手机号（用于短信渠道） |
| templateId | Guid? | 否 | 消息模板ID |
| templateVariables | string? | 否 | 模板变量（JSON格式） |
| businessType | string? | 否 | 业务类型 |
| businessId | string? | 否 | 业务ID |
| scheduledSendTime | DateTime? | 否 | 计划发送时间 |
| expirationTime | DateTime? | 否 | 过期时间 |
| extension | string? | 否 | 扩展属性（JSON格式） |
| tags | string? | 否 | 消息标签 |
| linkUrl | string? | 否 | 消息链接 |
| attachmentIds | List<Guid>? | 否 | 附件ID列表 |
| maxRetryCount | int | 否 | 最大重试次数，默认 3 |

### MessageDto

消息响应模型。

| 字段 | 类型 | 说明 |
|------|------|------|
| id | Guid | 消息ID |
| title | string | 消息标题 |
| content | string | 消息内容 |
| summary | string? | 消息摘要 |
| messageType | MessageType | 消息类型 |
| channel | MessageChannel | 消息渠道 |
| status | MessageStatus | 消息状态 |
| priority | MessagePriority | 消息优先级 |
| senderId | Guid? | 发送者ID |
| senderName | string? | 发送者名称 |
| receiverId | string | 接收者ID |
| receiverName | string? | 接收者名称 |
| receiverEmail | string? | 接收者邮箱 |
| receiverPhone | string? | 接收者手机号 |
| templateId | Guid? | 模板ID |
| businessType | string? | 业务类型 |
| businessId | string? | 业务ID |
| scheduledSendTime | DateTime? | 计划发送时间 |
| actualSendTime | DateTime? | 实际发送时间 |
| deliveredTime | DateTime? | 送达时间 |
| readTime | DateTime? | 阅读时间 |
| expirationTime | DateTime? | 过期时间 |
| retryCount | int | 重试次数 |
| maxRetryCount | int | 最大重试次数 |
| failureReason | string? | 失败原因 |
| extension | string? | 扩展属性 |
| tags | string? | 消息标签 |
| linkUrl | string? | 消息链接 |
| attachmentIds | string? | 附件ID列表（JSON格式） |
| isRead | bool | 是否已读 |
| creationTime | DateTime | 创建时间 |
| lastModificationTime | DateTime? | 最后修改时间 |

### MessageQueryDto

消息查询参数模型。

| 字段 | 类型 | 说明 |
|------|------|------|
| receiverId | string? | 接收者ID |
| senderId | Guid? | 发送者ID |
| messageType | MessageType? | 消息类型 |
| channel | MessageChannel? | 消息渠道 |
| status | MessageStatus? | 消息状态 |
| priority | MessagePriority? | 消息优先级 |
| businessType | string? | 业务类型 |
| businessId | string? | 业务ID |
| isRead | bool? | 是否已读 |
| keyword | string? | 关键词搜索（标题、内容） |
| startTime | DateTime? | 开始时间 |
| endTime | DateTime? | 结束时间 |
| tags | string? | 标签 |
| skipCount | int | 跳过数量，默认 0 |
| maxResultCount | int | 最大返回数量，默认 10 |
| sorting | string? | 排序字段，如 "creationTime desc" |

### MessageStatisticsDto

消息统计响应模型。

| 字段 | 类型 | 说明 |
|------|------|------|
| totalCount | long | 总消息数 |
| unreadCount | long | 未读消息数 |
| readCount | long | 已读消息数 |
| statusStatistics | Dictionary<MessageStatus, long> | 按状态统计 |
| typeStatistics | Dictionary<MessageType, long> | 按类型统计 |
| channelStatistics | Dictionary<MessageChannel, long> | 按渠道统计 |
| startTime | DateTime? | 统计开始时间 |
| endTime | DateTime? | 统计结束时间 |

---

## 枚举类型

### MessageType（消息类型）

| 值 | 说明 |
|----|------|
| 1 | Notification - 通知消息 |
| 2 | Workflow - 工作流消息 |
| 3 | Alert - 警报消息 |
| 4 | Transaction - 事务消息 |
| 5 | Marketing - 营销消息 |
| 6 | Social - 社交消息 |
| 7 | System - 系统消息 |
| 8 | Realtime - 实时消息 |

### MessageChannel（消息渠道）

| 值 | 说明 |
|----|------|
| 1 | InApp - 站内信 |
| 2 | Email - 邮件 |
| 3 | Sms - 短信 |
| 4 | Push - 推送通知 |
| 5 | WeChat - 微信 |
| 6 | DingTalk - 钉钉 |
| 7 | WebSocket - WebSocket实时推送 |
| 8 | External - 外部渠道 |

### MessageStatus（消息状态）

| 值 | 说明 |
|----|------|
| 1 | Pending - 待发送 |
| 2 | Sending - 发送中 |
| 3 | Sent - 已发送 |
| 4 | Delivered - 已送达 |
| 5 | Read - 已读 |
| 6 | Failed - 发送失败 |
| 7 | Cancelled - 已取消 |
| 8 | Expired - 已过期 |

### MessagePriority（消息优先级）

| 值 | 说明 |
|----|------|
| 1 | Low - 低优先级 |
| 2 | Normal - 普通优先级 |
| 3 | High - 高优先级 |
| 4 | Urgent - 紧急优先级 |

---

## API 端点

### 消息管理

#### 1. 创建并发送消息

创建一条新消息并触发发送。

**端点**: `POST /api/messages`

**请求体**:
```json
{
  "title": "系统通知",
  "content": "这是一条测试消息",
  "summary": "消息摘要",
  "messageType": 1,
  "channel": 1,
  "priority": 2,
  "receiverId": "user123",
  "receiverName": "张三",
  "businessType": "Order",
  "businessId": "order001",
  "scheduledSendTime": "2024-01-01T10:00:00Z",
  "expirationTime": "2024-01-31T23:59:59Z",
  "tags": "重要,通知",
  "linkUrl": "https://example.com/message/123",
  "maxRetryCount": 3
}
```

**响应**: `200 OK`
```json
{
  "id": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
  "title": "系统通知",
  "content": "这是一条测试消息",
  "summary": "消息摘要",
  "messageType": 1,
  "channel": 1,
  "status": 1,
  "priority": 2,
  "receiverId": "user123",
  "receiverName": "张三",
  "isRead": false,
  "creationTime": "2024-01-01T09:00:00Z"
}
```

**错误响应**:
- `400 Bad Request`: 请求参数无效
- `401 Unauthorized`: 未授权
- `500 Internal Server Error`: 服务器内部错误

---

#### 2. 批量创建并发送消息

批量创建多条消息并触发发送。

**端点**: `POST /api/messages/batch`

**请求体**:
```json
[
  {
    "title": "消息1",
    "content": "内容1",
    "messageType": 1,
    "channel": 1,
    "receiverId": "user1"
  },
  {
    "title": "消息2",
    "content": "内容2",
    "messageType": 1,
    "channel": 1,
    "receiverId": "user2"
  }
]
```

**响应**: `200 OK`
```json
[
  {
    "id": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
    "title": "消息1",
    "content": "内容1",
    "messageType": 1,
    "channel": 1,
    "status": 1,
    "receiverId": "user1"
  },
  {
    "id": "4fa85f64-5717-4562-b3fc-2c963f66afa7",
    "title": "消息2",
    "content": "内容2",
    "messageType": 1,
    "channel": 1,
    "status": 1,
    "receiverId": "user2"
  }
]
```

**限制**: 批量发送数量不能超过 1000 条

---

#### 3. 根据ID获取消息

获取指定ID的消息详情。

**端点**: `GET /api/messages/{id}`

**路径参数**:
- `id` (Guid): 消息ID

**响应**: `200 OK`
```json
{
  "id": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
  "title": "系统通知",
  "content": "这是一条测试消息",
  "messageType": 1,
  "channel": 1,
  "status": 5,
  "priority": 2,
  "receiverId": "user123",
  "isRead": true,
  "readTime": "2024-01-01T10:30:00Z",
  "creationTime": "2024-01-01T09:00:00Z"
}
```

**错误响应**:
- `404 Not Found`: 消息不存在

---

#### 4. 删除消息

删除指定ID的消息。

**端点**: `DELETE /api/messages/{id}`

**路径参数**:
- `id` (Guid): 消息ID

**响应**: `200 OK` (无响应体)

**错误响应**:
- `404 Not Found`: 消息不存在

---

#### 5. 批量删除消息

批量删除多条消息。

**端点**: `DELETE /api/messages/batch`

**请求体**:
```json
[
  "3fa85f64-5717-4562-b3fc-2c963f66afa6",
  "4fa85f64-5717-4562-b3fc-2c963f66afa7"
]
```

**响应**: `200 OK` (无响应体)

---

### 消息查询

#### 6. 查询消息列表

根据查询条件获取消息列表（分页）。

**端点**: `GET /api/messages`

**查询参数**:
- `receiverId` (string?): 接收者ID
- `senderId` (Guid?): 发送者ID
- `messageType` (MessageType?): 消息类型
- `channel` (MessageChannel?): 消息渠道
- `status` (MessageStatus?): 消息状态
- `priority` (MessagePriority?): 消息优先级
- `businessType` (string?): 业务类型
- `businessId` (string?): 业务ID
- `isRead` (bool?): 是否已读
- `keyword` (string?): 关键词搜索
- `startTime` (DateTime?): 开始时间
- `endTime` (DateTime?): 结束时间
- `tags` (string?): 标签
- `skipCount` (int): 跳过数量，默认 0
- `maxResultCount` (int): 最大返回数量，默认 10
- `sorting` (string?): 排序字段

**示例请求**:
```
GET /api/messages?receiverId=user123&messageType=1&status=5&skipCount=0&maxResultCount=20&sorting=creationTime desc
```

**响应**: `200 OK`
```json
{
  "totalCount": 100,
  "items": [
    {
      "id": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
      "title": "系统通知",
      "content": "这是一条测试消息",
      "messageType": 1,
      "channel": 1,
      "status": 5,
      "receiverId": "user123",
      "isRead": true,
      "creationTime": "2024-01-01T09:00:00Z"
    }
  ]
}
```

---

#### 7. 获取接收者的消息列表

获取指定接收者的消息列表（分页）。

**端点**: `GET /api/messages/receiver/{receiverId}`

**路径参数**:
- `receiverId` (string): 接收者ID

**查询参数**: 同 [查询消息列表](#6-查询消息列表)

**示例请求**:
```
GET /api/messages/receiver/user123?messageType=1&isRead=false&skipCount=0&maxResultCount=10
```

**响应**: `200 OK` (格式同查询消息列表)

---

### 消息状态操作

#### 8. 标记消息为已读

将指定消息标记为已读。

**端点**: `PUT /api/messages/{id}/read`

**路径参数**:
- `id` (Guid): 消息ID

**响应**: `200 OK` (无响应体)

**错误响应**:
- `404 Not Found`: 消息不存在

---

#### 9. 批量标记消息为已读

批量将多条消息标记为已读。

**端点**: `PUT /api/messages/read/batch`

**请求体**:
```json
[
  "3fa85f64-5717-4562-b3fc-2c963f66afa6",
  "4fa85f64-5717-4562-b3fc-2c963f66afa7"
]
```

**响应**: `200 OK` (无响应体)

---

#### 10. 标记所有消息为已读

将指定接收者的所有消息标记为已读。

**端点**: `PUT /api/messages/read/all/{receiverId}`

**路径参数**:
- `receiverId` (string): 接收者ID

**响应**: `200 OK` (无响应体)

---

#### 11. 重试发送失败的消息

重试发送失败的消息。

**端点**: `POST /api/messages/{id}/retry`

**路径参数**:
- `id` (Guid): 消息ID

**响应**: `200 OK` (无响应体)

**错误响应**:
- `400 Bad Request`: 消息不能重试（已达到最大重试次数或状态不允许）
- `404 Not Found`: 消息不存在

---

#### 12. 取消消息发送

取消待发送或发送中的消息。

**端点**: `POST /api/messages/{id}/cancel`

**路径参数**:
- `id` (Guid): 消息ID

**响应**: `200 OK` (无响应体)

**错误响应**:
- `400 Bad Request`: 已发送的消息不能取消
- `404 Not Found`: 消息不存在

---

### 消息统计

#### 13. 获取未读消息数量

获取指定接收者的未读消息数量。

**端点**: `GET /api/messages/unread-count/{receiverId}`

**路径参数**:
- `receiverId` (string): 接收者ID

**响应**: `200 OK`
```json
42
```

---

#### 14. 获取消息统计信息

获取消息的统计信息。

**端点**: `GET /api/messages/statistics`

**查询参数**:
- `receiverId` (string?): 接收者ID（可选）
- `startTime` (DateTime?): 开始时间（可选）
- `endTime` (DateTime?): 结束时间（可选）

**示例请求**:
```
GET /api/messages/statistics?receiverId=user123&startTime=2024-01-01T00:00:00Z&endTime=2024-01-31T23:59:59Z
```

**响应**: `200 OK`
```json
{
  "totalCount": 1000,
  "unreadCount": 150,
  "readCount": 850,
  "statusStatistics": {
    "1": 10,
    "2": 5,
    "3": 800,
    "4": 100,
    "5": 85
  },
  "typeStatistics": {
    "1": 500,
    "2": 200,
    "3": 150,
    "4": 100,
    "5": 50
  },
  "channelStatistics": {
    "1": 600,
    "2": 200,
    "3": 100,
    "4": 100
  },
  "startTime": "2024-01-01T00:00:00Z",
  "endTime": "2024-01-31T23:59:59Z"
}
```

---

#### 15. 获取SignalR连接信息

获取SignalR Hub的连接地址和认证信息，用于建立实时通信连接。

**端点**: `GET /api/messages/realtime/info`

**响应**: `200 OK`
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

**响应字段说明**:
- `hubUrl` (string): SignalR Hub的连接地址
- `accessToken` (string?): 当前用户的访问令牌（从请求头中提取）
- `supportedMethods` (List<string>): 支持的事件方法列表

**使用场景**:
- 前端应用启动时获取连接信息
- 建立SignalR连接前获取Hub地址
- 获取支持的事件方法列表

**示例请求**:
```
GET /api/messages/realtime/info
Authorization: Bearer your_jwt_token
```

**注意**: 
- 此端点会从请求头中提取JWT Token并返回，方便前端直接使用
- 如果请求头中没有Token，`accessToken`字段可能为空
- 建议在建立SignalR连接前调用此接口获取最新的连接信息

---

## 错误处理

### 标准HTTP状态码

| 状态码 | 说明 |
|--------|------|
| 200 | 请求成功 |
| 400 | 请求参数错误 |
| 401 | 未授权，需要有效的JWT Token |
| 403 | 禁止访问，权限不足 |
| 404 | 资源不存在 |
| 500 | 服务器内部错误 |

### 错误响应格式

```json
{
  "error": {
    "code": "INVALID_INPUT",
    "message": "接收者ID不能为空",
    "details": "The receiverId field is required."
  }
}
```

---

## 示例代码

### C# (HttpClient)

```csharp
using System;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

public class MessageApiClient
{
    private readonly HttpClient _httpClient;
    private readonly string _baseUrl = "https://api.example.com/api/messages";
    private readonly string _token = "your_jwt_token";

    public MessageApiClient()
    {
        _httpClient = new HttpClient();
        _httpClient.DefaultRequestHeaders.Authorization = 
            new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", _token);
    }

    // 创建消息
    public async Task<MessageDto> CreateMessageAsync(CreateMessageDto dto)
    {
        var json = JsonSerializer.Serialize(dto);
        var content = new StringContent(json, Encoding.UTF8, "application/json");
        
        var response = await _httpClient.PostAsync(_baseUrl, content);
        response.EnsureSuccessStatusCode();
        
        var responseJson = await response.Content.ReadAsStringAsync();
        return JsonSerializer.Deserialize<MessageDto>(responseJson);
    }

    // 获取消息列表
    public async Task<PagedResultDto<MessageDto>> GetMessagesAsync(
        string receiverId, 
        int skipCount = 0, 
        int maxResultCount = 10)
    {
        var url = $"{_baseUrl}?receiverId={receiverId}&skipCount={skipCount}&maxResultCount={maxResultCount}";
        var response = await _httpClient.GetAsync(url);
        response.EnsureSuccessStatusCode();
        
        var responseJson = await response.Content.ReadAsStringAsync();
        return JsonSerializer.Deserialize<PagedResultDto<MessageDto>>(responseJson);
    }

    // 标记为已读
    public async Task MarkAsReadAsync(Guid messageId)
    {
        var url = $"{_baseUrl}/{messageId}/read";
        var response = await _httpClient.PutAsync(url, null);
        response.EnsureSuccessStatusCode();
    }
}
```

### JavaScript (Fetch API)

```javascript
const API_BASE_URL = 'https://api.example.com/api/messages';
const TOKEN = 'your_jwt_token';

// 创建消息
async function createMessage(messageData) {
  const response = await fetch(API_BASE_URL, {
    method: 'POST',
    headers: {
      'Content-Type': 'application/json',
      'Authorization': `Bearer ${TOKEN}`
    },
    body: JSON.stringify(messageData)
  });
  
  if (!response.ok) {
    throw new Error(`HTTP error! status: ${response.status}`);
  }
  
  return await response.json();
}

// 获取消息列表
async function getMessages(receiverId, skipCount = 0, maxResultCount = 10) {
  const url = `${API_BASE_URL}?receiverId=${receiverId}&skipCount=${skipCount}&maxResultCount=${maxResultCount}`;
  const response = await fetch(url, {
    headers: {
      'Authorization': `Bearer ${TOKEN}`
    }
  });
  
  if (!response.ok) {
    throw new Error(`HTTP error! status: ${response.status}`);
  }
  
  return await response.json();
}

// 标记为已读
async function markAsRead(messageId) {
  const url = `${API_BASE_URL}/${messageId}/read`;
  const response = await fetch(url, {
    method: 'PUT',
    headers: {
      'Authorization': `Bearer ${TOKEN}`
    }
  });
  
  if (!response.ok) {
    throw new Error(`HTTP error! status: ${response.status}`);
  }
}

// 使用示例
(async () => {
  try {
    // 创建消息
    const message = await createMessage({
      title: '系统通知',
      content: '这是一条测试消息',
      messageType: 1,
      channel: 1,
      receiverId: 'user123'
    });
    console.log('消息已创建:', message);
    
    // 获取消息列表
    const messages = await getMessages('user123');
    console.log('消息列表:', messages);
    
    // 标记为已读
    if (messages.items.length > 0) {
      await markAsRead(messages.items[0].id);
      console.log('消息已标记为已读');
    }
  } catch (error) {
    console.error('错误:', error);
  }
})();
```

### cURL

```bash
# 创建消息
curl -X POST "https://api.example.com/api/messages" \
  -H "Authorization: Bearer your_jwt_token" \
  -H "Content-Type: application/json" \
  -d '{
    "title": "系统通知",
    "content": "这是一条测试消息",
    "messageType": 1,
    "channel": 1,
    "receiverId": "user123"
  }'

# 获取消息列表
curl -X GET "https://api.example.com/api/messages?receiverId=user123&skipCount=0&maxResultCount=10" \
  -H "Authorization: Bearer your_jwt_token"

# 标记为已读
curl -X PUT "https://api.example.com/api/messages/3fa85f64-5717-4562-b3fc-2c963f66afa6/read" \
  -H "Authorization: Bearer your_jwt_token"

# 获取统计信息
curl -X GET "https://api.example.com/api/messages/statistics?receiverId=user123" \
  -H "Authorization: Bearer your_jwt_token"
```

---

## 最佳实践

1. **批量操作**: 使用批量接口处理多条消息，提高效率
2. **分页查询**: 查询消息列表时始终使用分页参数，避免一次性加载过多数据
3. **错误处理**: 实现完善的错误处理机制，根据HTTP状态码和错误信息进行相应处理
4. **重试机制**: 对于失败的消息，使用重试接口进行重试，但注意不要超过最大重试次数
5. **状态管理**: 及时标记消息为已读，保持消息状态的准确性
6. **性能优化**: 使用统计接口获取汇总信息，避免频繁查询详细数据

---

## 版本信息

- **API版本**: v1
- **最后更新**: 2024-01-01
- **文档版本**: 1.0.0

---

## 支持

如有问题或建议，请联系开发团队或提交 Issue。
