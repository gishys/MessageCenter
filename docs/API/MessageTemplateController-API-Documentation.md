# Message Template Controller API 文档

## 概述

Message Template Controller 提供消息模板管理的完整 API 接口，支持模板的创建、查询、更新、删除以及启用/禁用等功能。

**基础路径**: `/api/message-templates`

**认证方式**: JWT Bearer Token

---

## 目录

- [认证](#认证)
- [数据模型](#数据模型)
- [枚举类型](#枚举类型)
- [API 端点](#api-端点)
  - [模板管理](#模板管理)
  - [模板查询](#模板查询)
  - [模板状态操作](#模板状态操作)
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

### CreateMessageTemplateDto

创建消息模板的请求模型。

| 字段 | 类型 | 必填 | 说明 |
|------|------|------|------|
| name | string | 是 | 模板名称 |
| code | string | 是 | 模板代码（唯一标识） |
| templateType | TemplateType | 是 | 模板类型（枚举） |
| messageType | MessageType | 是 | 消息类型（枚举） |
| channel | MessageChannel | 是 | 消息渠道（枚举） |
| title | string | 是 | 模板标题 |
| content | string | 是 | 模板内容 |
| description | string? | 否 | 模板描述 |
| isEnabled | bool | 否 | 是否启用，默认 true |
| variables | string? | 否 | 模板变量说明（JSON格式） |
| extension | string? | 否 | 扩展属性（JSON格式） |

### MessageTemplateDto

消息模板响应模型。

| 字段 | 类型 | 说明 |
|------|------|------|
| id | Guid | 模板ID |
| name | string | 模板名称 |
| code | string | 模板代码 |
| templateType | TemplateType | 模板类型 |
| messageType | MessageType | 消息类型 |
| channel | MessageChannel | 消息渠道 |
| title | string | 模板标题 |
| content | string | 模板内容 |
| description | string? | 模板描述 |
| isEnabled | bool | 是否启用 |
| variables | string? | 模板变量说明 |
| extension | string? | 扩展属性 |
| creationTime | DateTime | 创建时间 |
| lastModificationTime | DateTime? | 最后修改时间 |

---

## 枚举类型

### TemplateType（模板类型）

| 值 | 说明 |
|----|------|
| 1 | Text - 文本模板 |
| 2 | Html - HTML模板 |
| 3 | Markdown - Markdown模板 |
| 4 | Json - JSON模板 |

### MessageType（消息类型）

参考 [Message Controller API 文档](./MessageController-API-Documentation.md#messagetype消息类型)

### MessageChannel（消息渠道）

参考 [Message Controller API 文档](./MessageController-API-Documentation.md#messagechannel消息渠道)

---

## API 端点

### 模板管理

#### 1. 创建消息模板

创建一个新的消息模板。

**端点**: `POST /api/message-templates`

**请求体**:
```json
{
  "name": "订单通知模板",
  "code": "ORDER_NOTIFICATION",
  "templateType": 1,
  "messageType": 4,
  "channel": 1,
  "title": "订单{{orderNo}}已{{status}}",
  "content": "尊敬的{{userName}}，您的订单{{orderNo}}已{{status}}，订单金额：{{amount}}元。",
  "description": "用于订单状态变更通知的模板",
  "isEnabled": true,
  "variables": "{\"orderNo\":\"订单号\",\"status\":\"订单状态\",\"userName\":\"用户名称\",\"amount\":\"订单金额\"}",
  "extension": "{\"category\":\"order\"}"
}
```

**响应**: `200 OK`
```json
{
  "id": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
  "name": "订单通知模板",
  "code": "ORDER_NOTIFICATION",
  "templateType": 1,
  "messageType": 4,
  "channel": 1,
  "title": "订单{{orderNo}}已{{status}}",
  "content": "尊敬的{{userName}}，您的订单{{orderNo}}已{{status}}，订单金额：{{amount}}元。",
  "description": "用于订单状态变更通知的模板",
  "isEnabled": true,
  "variables": "{\"orderNo\":\"订单号\",\"status\":\"订单状态\",\"userName\":\"用户名称\",\"amount\":\"订单金额\"}",
  "extension": "{\"category\":\"order\"}",
  "creationTime": "2024-01-01T09:00:00Z"
}
```

**错误响应**:
- `400 Bad Request`: 请求参数无效或模板代码已存在
- `401 Unauthorized`: 未授权
- `500 Internal Server Error`: 服务器内部错误

---

#### 2. 更新消息模板

更新指定ID的消息模板。

**端点**: `PUT /api/message-templates/{id}`

**路径参数**:
- `id` (Guid): 模板ID

**请求体**: 同创建消息模板

**响应**: `200 OK`
```json
{
  "id": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
  "name": "订单通知模板（已更新）",
  "code": "ORDER_NOTIFICATION",
  "templateType": 1,
  "messageType": 4,
  "channel": 1,
  "title": "订单{{orderNo}}已{{status}}",
  "content": "尊敬的{{userName}}，您的订单{{orderNo}}已{{status}}，订单金额：{{amount}}元。感谢您的支持！",
  "description": "用于订单状态变更通知的模板（已更新）",
  "isEnabled": true,
  "lastModificationTime": "2024-01-02T10:00:00Z"
}
```

**错误响应**:
- `400 Bad Request`: 请求参数无效或模板代码已被其他模板使用
- `404 Not Found`: 模板不存在
- `401 Unauthorized`: 未授权

---

#### 3. 删除消息模板

删除指定ID的消息模板。

**端点**: `DELETE /api/message-templates/{id}`

**路径参数**:
- `id` (Guid): 模板ID

**响应**: `200 OK` (无响应体)

**错误响应**:
- `404 Not Found`: 模板不存在
- `401 Unauthorized`: 未授权

---

### 模板查询

#### 4. 根据ID获取消息模板

获取指定ID的模板详情。

**端点**: `GET /api/message-templates/{id}`

**路径参数**:
- `id` (Guid): 模板ID

**响应**: `200 OK`
```json
{
  "id": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
  "name": "订单通知模板",
  "code": "ORDER_NOTIFICATION",
  "templateType": 1,
  "messageType": 4,
  "channel": 1,
  "title": "订单{{orderNo}}已{{status}}",
  "content": "尊敬的{{userName}}，您的订单{{orderNo}}已{{status}}，订单金额：{{amount}}元。",
  "description": "用于订单状态变更通知的模板",
  "isEnabled": true,
  "variables": "{\"orderNo\":\"订单号\",\"status\":\"订单状态\",\"userName\":\"用户名称\",\"amount\":\"订单金额\"}",
  "creationTime": "2024-01-01T09:00:00Z"
}
```

**错误响应**:
- `404 Not Found`: 模板不存在
- `401 Unauthorized`: 未授权

---

#### 5. 根据代码获取消息模板

根据模板代码获取模板详情。

**端点**: `GET /api/message-templates/code/{code}`

**路径参数**:
- `code` (string): 模板代码

**响应**: `200 OK` (格式同根据ID获取消息模板)

**错误响应**:
- `404 Not Found`: 模板不存在
- `401 Unauthorized`: 未授权

---

#### 6. 获取消息模板列表

获取所有消息模板的列表。

**端点**: `GET /api/message-templates`

**响应**: `200 OK`
```json
[
  {
    "id": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
    "name": "订单通知模板",
    "code": "ORDER_NOTIFICATION",
    "templateType": 1,
    "messageType": 4,
    "channel": 1,
    "title": "订单{{orderNo}}已{{status}}",
    "content": "尊敬的{{userName}}，您的订单{{orderNo}}已{{status}}。",
    "isEnabled": true,
    "creationTime": "2024-01-01T09:00:00Z"
  },
  {
    "id": "4fa85f64-5717-4562-b3fc-2c963f66afa7",
    "name": "系统通知模板",
    "code": "SYSTEM_NOTIFICATION",
    "templateType": 1,
    "messageType": 7,
    "channel": 1,
    "title": "系统通知",
    "content": "系统通知：{{message}}",
    "isEnabled": true,
    "creationTime": "2024-01-01T10:00:00Z"
  }
]
```

**错误响应**:
- `401 Unauthorized`: 未授权

---

### 模板状态操作

#### 7. 启用/禁用消息模板

启用或禁用指定的消息模板。

**端点**: `PUT /api/message-templates/{id}/enabled`

**路径参数**:
- `id` (Guid): 模板ID

**请求体**:
```json
true
```
或
```json
false
```

**响应**: `200 OK` (无响应体)

**错误响应**:
- `404 Not Found`: 模板不存在
- `401 Unauthorized`: 未授权

**说明**: 
- 禁用的模板不会被用于消息发送
- 已禁用的模板可以通过此接口重新启用

---

## 错误处理

### 标准HTTP状态码

| 状态码 | 说明 |
|--------|------|
| 200 | 请求成功 |
| 400 | 请求参数错误（如模板代码已存在） |
| 401 | 未授权，需要有效的JWT Token |
| 403 | 禁止访问，权限不足 |
| 404 | 资源不存在 |
| 500 | 服务器内部错误 |

### 错误响应格式

```json
{
  "error": {
    "code": "TEMPLATE_CODE_EXISTS",
    "message": "模板代码 ORDER_NOTIFICATION 已存在",
    "details": "The template code 'ORDER_NOTIFICATION' is already in use."
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

public class MessageTemplateApiClient
{
    private readonly HttpClient _httpClient;
    private readonly string _baseUrl = "https://api.example.com/api/message-templates";
    private readonly string _token = "your_jwt_token";

    public MessageTemplateApiClient()
    {
        _httpClient = new HttpClient();
        _httpClient.DefaultRequestHeaders.Authorization = 
            new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", _token);
    }

    // 创建模板
    public async Task<MessageTemplateDto> CreateTemplateAsync(CreateMessageTemplateDto dto)
    {
        var json = JsonSerializer.Serialize(dto);
        var content = new StringContent(json, Encoding.UTF8, "application/json");
        
        var response = await _httpClient.PostAsync(_baseUrl, content);
        response.EnsureSuccessStatusCode();
        
        var responseJson = await response.Content.ReadAsStringAsync();
        return JsonSerializer.Deserialize<MessageTemplateDto>(responseJson);
    }

    // 获取模板列表
    public async Task<List<MessageTemplateDto>> GetTemplatesAsync()
    {
        var response = await _httpClient.GetAsync(_baseUrl);
        response.EnsureSuccessStatusCode();
        
        var responseJson = await response.Content.ReadAsStringAsync();
        return JsonSerializer.Deserialize<List<MessageTemplateDto>>(responseJson);
    }

    // 根据代码获取模板
    public async Task<MessageTemplateDto> GetTemplateByCodeAsync(string code)
    {
        var url = $"{_baseUrl}/code/{code}";
        var response = await _httpClient.GetAsync(url);
        response.EnsureSuccessStatusCode();
        
        var responseJson = await response.Content.ReadAsStringAsync();
        return JsonSerializer.Deserialize<MessageTemplateDto>(responseJson);
    }

    // 更新模板
    public async Task<MessageTemplateDto> UpdateTemplateAsync(Guid id, CreateMessageTemplateDto dto)
    {
        var json = JsonSerializer.Serialize(dto);
        var content = new StringContent(json, Encoding.UTF8, "application/json");
        var url = $"{_baseUrl}/{id}";
        
        var response = await _httpClient.PutAsync(url, content);
        response.EnsureSuccessStatusCode();
        
        var responseJson = await response.Content.ReadAsStringAsync();
        return JsonSerializer.Deserialize<MessageTemplateDto>(responseJson);
    }

    // 启用/禁用模板
    public async Task SetTemplateEnabledAsync(Guid id, bool isEnabled)
    {
        var json = JsonSerializer.Serialize(isEnabled);
        var content = new StringContent(json, Encoding.UTF8, "application/json");
        var url = $"{_baseUrl}/{id}/enabled";
        
        var response = await _httpClient.PutAsync(url, content);
        response.EnsureSuccessStatusCode();
    }

    // 删除模板
    public async Task DeleteTemplateAsync(Guid id)
    {
        var url = $"{_baseUrl}/{id}";
        var response = await _httpClient.DeleteAsync(url);
        response.EnsureSuccessStatusCode();
    }
}

// 使用示例
var client = new MessageTemplateApiClient();

// 创建模板
var template = await client.CreateTemplateAsync(new CreateMessageTemplateDto
{
    Name = "订单通知模板",
    Code = "ORDER_NOTIFICATION",
    TemplateType = TemplateType.Text,
    MessageType = MessageType.Transaction,
    Channel = MessageChannel.InApp,
    Title = "订单{{orderNo}}已{{status}}",
    Content = "尊敬的{{userName}}，您的订单{{orderNo}}已{{status}}。",
    IsEnabled = true
});

// 获取模板列表
var templates = await client.GetTemplatesAsync();

// 根据代码获取模板
var orderTemplate = await client.GetTemplateByCodeAsync("ORDER_NOTIFICATION");

// 禁用模板
await client.SetTemplateEnabledAsync(template.Id, false);
```

### JavaScript (Fetch API)

```javascript
const API_BASE_URL = 'https://api.example.com/api/message-templates';
const TOKEN = 'your_jwt_token';

// 创建模板
async function createTemplate(templateData) {
  const response = await fetch(API_BASE_URL, {
    method: 'POST',
    headers: {
      'Content-Type': 'application/json',
      'Authorization': `Bearer ${TOKEN}`
    },
    body: JSON.stringify(templateData)
  });
  
  if (!response.ok) {
    throw new Error(`HTTP error! status: ${response.status}`);
  }
  
  return await response.json();
}

// 获取模板列表
async function getTemplates() {
  const response = await fetch(API_BASE_URL, {
    headers: {
      'Authorization': `Bearer ${TOKEN}`
    }
  });
  
  if (!response.ok) {
    throw new Error(`HTTP error! status: ${response.status}`);
  }
  
  return await response.json();
}

// 根据代码获取模板
async function getTemplateByCode(code) {
  const url = `${API_BASE_URL}/code/${code}`;
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

// 更新模板
async function updateTemplate(id, templateData) {
  const url = `${API_BASE_URL}/${id}`;
  const response = await fetch(url, {
    method: 'PUT',
    headers: {
      'Content-Type': 'application/json',
      'Authorization': `Bearer ${TOKEN}`
    },
    body: JSON.stringify(templateData)
  });
  
  if (!response.ok) {
    throw new Error(`HTTP error! status: ${response.status}`);
  }
  
  return await response.json();
}

// 启用/禁用模板
async function setTemplateEnabled(id, isEnabled) {
  const url = `${API_BASE_URL}/${id}/enabled`;
  const response = await fetch(url, {
    method: 'PUT',
    headers: {
      'Content-Type': 'application/json',
      'Authorization': `Bearer ${TOKEN}`
    },
    body: JSON.stringify(isEnabled)
  });
  
  if (!response.ok) {
    throw new Error(`HTTP error! status: ${response.status}`);
  }
}

// 删除模板
async function deleteTemplate(id) {
  const url = `${API_BASE_URL}/${id}`;
  const response = await fetch(url, {
    method: 'DELETE',
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
    // 创建模板
    const template = await createTemplate({
      name: '订单通知模板',
      code: 'ORDER_NOTIFICATION',
      templateType: 1,
      messageType: 4,
      channel: 1,
      title: '订单{{orderNo}}已{{status}}',
      content: '尊敬的{{userName}}，您的订单{{orderNo}}已{{status}}。',
      isEnabled: true
    });
    console.log('模板已创建:', template);
    
    // 获取模板列表
    const templates = await getTemplates();
    console.log('模板列表:', templates);
    
    // 根据代码获取模板
    const orderTemplate = await getTemplateByCode('ORDER_NOTIFICATION');
    console.log('订单模板:', orderTemplate);
    
    // 禁用模板
    await setTemplateEnabled(template.id, false);
    console.log('模板已禁用');
  } catch (error) {
    console.error('错误:', error);
  }
})();
```

### cURL

```bash
# 创建模板
curl -X POST "https://api.example.com/api/message-templates" \
  -H "Authorization: Bearer your_jwt_token" \
  -H "Content-Type: application/json" \
  -d '{
    "name": "订单通知模板",
    "code": "ORDER_NOTIFICATION",
    "templateType": 1,
    "messageType": 4,
    "channel": 1,
    "title": "订单{{orderNo}}已{{status}}",
    "content": "尊敬的{{userName}}，您的订单{{orderNo}}已{{status}}。",
    "isEnabled": true
  }'

# 获取模板列表
curl -X GET "https://api.example.com/api/message-templates" \
  -H "Authorization: Bearer your_jwt_token"

# 根据代码获取模板
curl -X GET "https://api.example.com/api/message-templates/code/ORDER_NOTIFICATION" \
  -H "Authorization: Bearer your_jwt_token"

# 根据ID获取模板
curl -X GET "https://api.example.com/api/message-templates/3fa85f64-5717-4562-b3fc-2c963f66afa6" \
  -H "Authorization: Bearer your_jwt_token"

# 更新模板
curl -X PUT "https://api.example.com/api/message-templates/3fa85f64-5717-4562-b3fc-2c963f66afa6" \
  -H "Authorization: Bearer your_jwt_token" \
  -H "Content-Type: application/json" \
  -d '{
    "name": "订单通知模板（已更新）",
    "code": "ORDER_NOTIFICATION",
    "templateType": 1,
    "messageType": 4,
    "channel": 1,
    "title": "订单{{orderNo}}已{{status}}",
    "content": "尊敬的{{userName}}，您的订单{{orderNo}}已{{status}}。感谢您的支持！",
    "isEnabled": true
  }'

# 启用/禁用模板
curl -X PUT "https://api.example.com/api/message-templates/3fa85f64-5717-4562-b3fc-2c963f66afa6/enabled" \
  -H "Authorization: Bearer your_jwt_token" \
  -H "Content-Type: application/json" \
  -d "false"

# 删除模板
curl -X DELETE "https://api.example.com/api/message-templates/3fa85f64-5717-4562-b3fc-2c963f66afa6" \
  -H "Authorization: Bearer your_jwt_token"
```

---

## 模板变量说明

模板支持使用变量占位符，变量格式为 `{{variableName}}`。在创建消息时，可以通过 `templateVariables` 参数传入变量值。

### 示例

**模板内容**:
```
尊敬的{{userName}}，您的订单{{orderNo}}已{{status}}，订单金额：{{amount}}元。
```

**变量说明** (JSON格式):
```json
{
  "userName": "用户名称",
  "orderNo": "订单号",
  "status": "订单状态",
  "amount": "订单金额"
}
```

**创建消息时的变量值**:
```json
{
  "templateId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
  "templateVariables": "{\"userName\":\"张三\",\"orderNo\":\"ORD001\",\"status\":\"已发货\",\"amount\":\"199.00\"}"
}
```

**渲染后的消息内容**:
```
尊敬的张三，您的订单ORD001已已发货，订单金额：199.00元。
```

---

## 最佳实践

1. **模板代码命名**: 使用大写字母和下划线，如 `ORDER_NOTIFICATION`、`SYSTEM_ALERT`
2. **模板变量**: 在 `variables` 字段中详细说明所有可用变量及其用途
3. **模板类型选择**: 
   - 简单文本消息使用 `Text` 类型
   - 需要格式化的内容使用 `Html` 类型
   - 需要结构化数据使用 `Json` 类型
4. **模板管理**: 定期检查和更新模板，确保内容准确
5. **启用状态**: 不使用的模板应禁用而不是删除，以便后续恢复
6. **版本控制**: 更新模板时注意保持向后兼容，或创建新版本的模板

---

## 版本信息

- **API版本**: v1
- **最后更新**: 2024-01-01
- **文档版本**: 1.0.0

---

## 支持

如有问题或建议，请联系开发团队或提交 Issue。
