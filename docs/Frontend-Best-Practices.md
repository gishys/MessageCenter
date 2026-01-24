# 前端开发最佳实践指南

本文档为使用 Cursor AI 创建 MessageCenter 前端应用提供全面的最佳实践指南。

---

## 第一部分：项目结构和环境配置

## 概述

本文档为使用 Cursor AI 创建 MessageCenter 前端应用提供全面的最佳实践指南。文档分为多个部分，本部分重点介绍项目结构、技术栈选择和基础环境配置。

## 目录

- [技术栈推荐](#技术栈推荐)
- [项目结构](#项目结构)
- [环境配置](#环境配置)
- [依赖管理](#依赖管理)
- [开发工具配置](#开发工具配置)

---

## 技术栈推荐

### 核心框架选择

#### React + TypeScript（推荐）

**优势**：
- 类型安全，减少运行时错误
- 丰富的生态系统和社区支持
- 与 SignalR 集成良好
- 适合大型项目

**推荐版本**：
- React: 19.2+
- TypeScript: 5.x
- Node.js: 18.x 或更高

**React 19 新特性**：
- Actions 和 useActionState（表单处理）
- useOptimistic（乐观更新）
- useFormStatus（表单状态）
- 改进的并发渲染
- 更好的 TypeScript 支持

#### Vue 3 + TypeScript（备选）

**优势**：
- 学习曲线平缓
- 性能优秀
- 组合式 API 灵活

**推荐版本**：
- Vue: 3.3.x
- TypeScript: 5.x

### 状态管理

#### 推荐方案

1. **Zustand**（轻量级，推荐）
   - 简单易用，适合中小型项目
   - 与 React 集成良好
   - 支持 TypeScript

2. **Redux Toolkit**（复杂状态管理）
   - 适合大型项目
   - 强大的中间件支持
   - 完善的开发工具

3. **React Query / TanStack Query**（服务端状态）
   - 自动缓存和同步
   - 请求去重和重试
   - 与 REST API 完美配合

### HTTP 客户端

#### 推荐方案

1. **Axios**（推荐）
   - 功能完善，拦截器支持
   - 请求/响应转换
   - 自动 JSON 处理
   - 请求取消和超时控制

2. **Fetch API**（原生）
   - 无需额外依赖
   - 现代浏览器原生支持
   - 需要手动处理 JSON

### 表单处理

#### 推荐方案

1. **React Hook Form**（强烈推荐）
   - 性能优秀，减少不必要的重渲染
   - 与 Ant Design 集成良好
   - 支持复杂表单验证
   - 类型安全，支持 TypeScript
   - 与 React 19 Actions 完美配合

2. **Formik**（备选）
   - 功能全面
   - 社区活跃

### 路由管理

#### 推荐方案

1. **React Router 7.9+**（推荐）
   - 支持数据路由（Data Router）
   - 支持 React Server Components
   - 更好的 TypeScript 支持
   - 支持路由懒加载和代码分割
   - 与 React 19 完美兼容

### SignalR 客户端

#### 推荐方案

- **@microsoft/signalr**（官方）
  - 官方维护，稳定可靠
  - 支持 TypeScript
  - 自动重连功能

### UI 组件库

#### 推荐方案

1. **Ant Design 5.29+**（推荐）
   - 组件丰富，文档完善
   - 企业级应用首选
   - 支持 TypeScript
   - 支持 CSS-in-JS 和 CSS 变量主题定制
   - 更好的性能优化

2. **@ant-design/pro-components**（企业级高级组件）
   - 基于 Ant Design 的企业级高级组件
   - 提供 ProTable、ProForm、ProLayout 等开箱即用的高级组件
   - 大幅提升开发效率
   - 适合中后台管理系统

3. **Material-UI (MUI)**（备选）
   - Material Design 风格
   - 组件丰富

4. **Element Plus**（Vue 生态）
   - Vue 3 生态
   - 组件齐全

### 构建工具

#### 推荐方案

1. **Vite**（强烈推荐）
   - 极速启动和热更新
   - 原生 ES 模块支持
   - 插件生态丰富

2. **Create React App**
   - 零配置，快速开始
   - 适合小型项目

---

## 项目结构

### 推荐的目录结构

```
message-center-frontend/
├── public/                 # 静态资源
│   ├── favicon.ico
│   └── index.html
├── src/
│   ├── api/               # API 相关
│   │   ├── client.ts      # HTTP 客户端配置
│   │   ├── message.ts     # 消息相关 API
│   │   ├── template.ts    # 模板相关 API
│   │   └── types.ts       # API 类型定义
│   ├── services/          # 业务服务层
│   │   ├── messageService.ts
│   │   ├── templateService.ts
│   │   └── realtimeService.ts
│   ├── hooks/             # 自定义 Hooks
│   │   ├── useMessages.ts
│   │   ├── useRealtime.ts
│   │   └── useAuth.ts
│   ├── stores/            # 状态管理
│   │   ├── messageStore.ts
│   │   └── authStore.ts
│   ├── components/       # 组件
│   │   ├── common/        # 通用组件
│   │   ├── message/       # 消息相关组件
│   │   └── template/      # 模板相关组件
│   ├── pages/            # 页面组件
│   │   ├── MessageList.tsx
│   │   ├── MessageDetail.tsx
│   │   └── TemplateManagement.tsx
│   ├── routes/           # 路由配置
│   │   ├── index.tsx      # 路由定义
│   │   └── routes.ts      # 路由配置
│   ├── utils/            # 工具函数
│   │   ├── date.ts
│   │   ├── format.ts
│   │   └── validation.ts
│   ├── schemas/          # 表单验证 Schema（Zod/Yup）
│   │   ├── message.ts
│   │   └── template.ts
│   ├── types/            # TypeScript 类型定义
│   │   ├── message.ts
│   │   ├── template.ts
│   │   └── api.ts
│   ├── constants/        # 常量定义
│   │   ├── enums.ts
│   │   └── config.ts
│   ├── styles/           # 样式文件
│   │   ├── global.css
│   │   └── variables.css
│   ├── App.tsx           # 根组件
│   └── main.tsx          # 入口文件
├── .env                  # 环境变量
├── .env.development      # 开发环境变量
├── .env.production       # 生产环境变量
├── package.json
├── tsconfig.json         # TypeScript 配置
├── vite.config.ts        # Vite 配置
└── README.md
```

### 目录说明

#### api/
存放所有 API 相关的代码，包括：
- HTTP 客户端配置
- API 端点定义
- 请求/响应类型

#### services/
业务逻辑层，封装 API 调用，提供业务方法。

#### hooks/
自定义 React Hooks，封装可复用的逻辑。

#### stores/
状态管理，使用 Zustand 或 Redux 管理全局状态。

#### components/
UI 组件，按功能模块组织。

#### types/
TypeScript 类型定义，与后端 DTO 对应。

---

## 环境配置

### 环境变量

创建 `.env.development` 文件：

```env
# API 基础地址
VITE_API_BASE_URL=http://localhost:5000

# SignalR Hub 地址（可选，通常从 API 获取）
VITE_HUB_URL=http://localhost:5000/hubs/messages

# 应用标题
VITE_APP_TITLE=Message Center

# 是否启用调试模式
VITE_DEBUG=true
```

创建 `.env.production` 文件：

```env
VITE_API_BASE_URL=https://api.example.com
VITE_HUB_URL=https://api.example.com/hubs/messages
VITE_APP_TITLE=Message Center
VITE_DEBUG=false
```

### TypeScript 配置

`tsconfig.json` 推荐配置：

```json
{
  "compilerOptions": {
    "target": "ES2020",
    "useDefineForClassFields": true,
    "lib": ["ES2020", "DOM", "DOM.Iterable"],
    "module": "ESNext",
    "skipLibCheck": true,
    "moduleResolution": "bundler",
    "allowImportingTsExtensions": true,
    "resolveJsonModule": true,
    "isolatedModules": true,
    "noEmit": true,
    "jsx": "react-jsx",
    "strict": true,
    "noUnusedLocals": true,
    "noUnusedParameters": true,
    "noFallthroughCasesInSwitch": true,
    "baseUrl": ".",
    "paths": {
      "@/*": ["src/*"],
      "@/api/*": ["src/api/*"],
      "@/components/*": ["src/components/*"],
      "@/hooks/*": ["src/hooks/*"],
      "@/services/*": ["src/services/*"],
      "@/stores/*": ["src/stores/*"],
      "@/types/*": ["src/types/*"],
      "@/utils/*": ["src/utils/*"]
    }
  },
  "include": ["src"],
  "references": [{ "path": "./tsconfig.node.json" }]
}
```

### Vite 配置

`vite.config.ts` 推荐配置：

```typescript
import { defineConfig } from 'vite';
import react from '@vitejs/plugin-react';
import path from 'path';

export default defineConfig({
  plugins: [react()],
  resolve: {
    alias: {
      '@': path.resolve(__dirname, './src'),
    },
  },
  server: {
    port: 3000,
    proxy: {
      '/api': {
        target: 'http://localhost:5000',
        changeOrigin: true,
      },
      '/hubs': {
        target: 'ws://localhost:5000',
        ws: true,
        changeOrigin: true,
      },
    },
  },
  build: {
    outDir: 'dist',
    sourcemap: true,
  },
});
```

---

## 依赖管理

### 核心依赖

`package.json` 推荐依赖：

```json
{
  "dependencies": {
    "react": "^19.2.0",
    "react-dom": "^19.2.0",
    "react-router": "^7.9.0",
    "react-router-dom": "^7.9.0",
    "axios": "^1.7.9",
    "@microsoft/signalr": "^8.0.0",
    "zustand": "^5.0.2",
    "@tanstack/react-query": "^5.62.0",
    "antd": "^5.29.0",
    "@ant-design/pro-components": "^3.5.0",
    "react-hook-form": "^7.54.0",
    "@hookform/resolvers": "^3.9.1",
    "zod": "^3.24.1",
    "dayjs": "^1.11.13",
    "lodash-es": "^4.17.21"
  },
  "devDependencies": {
    "@types/react": "^19.0.0",
    "@types/react-dom": "^19.0.0",
    "@types/lodash-es": "^4.17.12",
    "@vitejs/plugin-react": "^4.3.4",
    "typescript": "^5.7.2",
    "vite": "^6.0.5",
    "eslint": "^9.18.0",
    "@typescript-eslint/eslint-plugin": "^8.18.0",
    "@typescript-eslint/parser": "^8.18.0",
    "eslint-plugin-react": "^7.37.3",
    "eslint-plugin-react-hooks": "^5.1.0"
  }
}
```

### 安装命令

```bash
# 使用 npm
npm install

# 或使用 yarn
yarn install

# 或使用 pnpm（推荐，更快）
pnpm install
```

---

## 开发工具配置

### ESLint 配置

`.eslintrc.json`：

```json
{
  "extends": [
    "eslint:recommended",
    "plugin:@typescript-eslint/recommended",
    "plugin:react/recommended",
    "plugin:react-hooks/recommended"
  ],
  "parser": "@typescript-eslint/parser",
  "parserOptions": {
    "ecmaVersion": 2020,
    "sourceType": "module",
    "ecmaFeatures": {
      "jsx": true
    }
  },
  "rules": {
    "react/react-in-jsx-scope": "off",
    "@typescript-eslint/no-explicit-any": "warn",
    "@typescript-eslint/explicit-function-return-type": "off"
  }
}
```

### Prettier 配置

`.prettierrc`：

```json
{
  "semi": true,
  "trailingComma": "es5",
  "singleQuote": true,
  "printWidth": 100,
  "tabWidth": 2,
  "useTabs": false
}
```

---



---

## 第二部分：API 集成和 HTTP 客户端

## 概述

本部分详细介绍如何集成 MessageCenter API，包括 HTTP 客户端配置、类型定义、API 封装和错误处理。

## 目录

- [类型定义](#类型定义)
- [HTTP 客户端配置](#http-客户端配置)
- [API 封装](#api-封装)
- [错误处理](#错误处理)
- [请求拦截器](#请求拦截器)
- [响应处理](#响应处理)

---

## 类型定义

### 重要说明

在定义前端类型时，需要注意以下与后端的差异：

1. **ID 类型转换**：后端使用 `Guid` 类型，前端使用 `string` 类型。所有 ID 字段（如 `id`, `senderId`, `templateId` 等）在前端都是字符串。

2. **日期时间格式**：后端返回 `DateTime` 类型，前端接收为 ISO 8601 格式的字符串（如 `"2024-01-01T12:00:00Z"`）。

3. **统计字段类型**：后端的 `Dictionary<枚举, long>` 在前端转换为 `Record<string, number>`，其中键为枚举值的字符串形式。

4. **附件ID**：`CreateMessageDto` 中 `attachmentIds` 为 `string[]`（Guid 数组转字符串数组），`MessageDto` 中为 `string?`（后端存储为字符串）。

5. **审计字段**：`MessageDto` 和 `MessageTemplateDto` 继承自 `FullAuditedEntityDto<Guid>`，包含审计字段（`creatorId`, `lastModifierId` 等）。

6. **分页参数**：`MessageQueryDto` 继承自 `PagedAndSortedResultRequestDto`，包含 `skipCount`、`maxResultCount` 和 `sorting` 字段。

### 基础类型

创建 `src/types/message.ts`：

```typescript
// 枚举类型（与后端对应）
export enum MessageType {
  Notification = 1,
  Workflow = 2,
  Alert = 3,
  Transaction = 4,
  Marketing = 5,
  Social = 6,
  System = 7,
  Realtime = 8,
}

export enum MessageChannel {
  InApp = 1,
  Email = 2,
  Sms = 3,
  Push = 4,
  WeChat = 5,
  DingTalk = 6,
  WebSocket = 7,
  External = 8,
}

export enum MessageStatus {
  Pending = 1,
  Sending = 2,
  Sent = 3,
  Delivered = 4,
  Read = 5,
  Failed = 6,
  Cancelled = 7,
  Expired = 8,
}

export enum MessagePriority {
  Low = 1,
  Normal = 2,
  High = 3,
  Urgent = 4,
}

// DTO 类型
export interface CreateMessageDto {
  title: string;
  content: string;
  summary?: string;
  messageType: MessageType;
  channel: MessageChannel;
  priority?: MessagePriority;
  senderId?: string; // Guid 转 string
  senderName?: string;
  receiverId?: string;
  receiverIds?: string[]; // 批量发送时使用
  receiverName?: string;
  receiverEmail?: string;
  receiverPhone?: string;
  templateId?: string; // Guid 转 string
  templateVariables?: string; // JSON 格式字符串
  businessType?: string;
  businessId?: string;
  scheduledSendTime?: string; // ISO 8601 格式日期时间字符串
  expirationTime?: string; // ISO 8601 格式日期时间字符串
  extension?: string; // JSON 格式字符串
  tags?: string;
  linkUrl?: string;
  attachmentIds?: string[]; // Guid[] 转 string[]
  maxRetryCount?: number; // 默认 3
}

export interface MessageDto {
  // 基础字段（继承自 FullAuditedEntityDto<Guid>）
  id: string; // Guid 转 string
  creationTime: string; // ISO 8601 格式日期时间字符串
  lastModificationTime?: string; // ISO 8601 格式日期时间字符串
  creatorId?: string; // Guid 转 string（审计字段）
  lastModifierId?: string; // Guid 转 string（审计字段）
  
  // 消息内容
  title: string;
  content: string;
  summary?: string;
  messageType: MessageType;
  channel: MessageChannel;
  status: MessageStatus;
  priority: MessagePriority;
  
  // 发送者信息
  senderId?: string; // Guid 转 string
  senderName?: string;
  
  // 接收者信息
  receiverId: string;
  receiverName?: string;
  receiverEmail?: string;
  receiverPhone?: string;
  
  // 模板信息
  templateId?: string; // Guid 转 string
  
  // 业务信息
  businessType?: string;
  businessId?: string;
  
  // 时间信息
  scheduledSendTime?: string; // ISO 8601 格式日期时间字符串
  actualSendTime?: string; // ISO 8601 格式日期时间字符串
  deliveredTime?: string; // ISO 8601 格式日期时间字符串
  readTime?: string; // ISO 8601 格式日期时间字符串
  expirationTime?: string; // ISO 8601 格式日期时间字符串
  
  // 重试信息
  retryCount: number;
  maxRetryCount: number;
  failureReason?: string;
  
  // 扩展信息
  extension?: string; // JSON 格式字符串
  tags?: string;
  linkUrl?: string;
  attachmentIds?: string; // 后端存储为字符串，可能是 JSON 数组字符串或逗号分隔的字符串
  
  // 状态信息
  isRead: boolean;
}

export interface MessageQueryDto {
  // 查询条件
  receiverId?: string;
  senderId?: string; // Guid 转 string
  messageType?: MessageType;
  channel?: MessageChannel;
  status?: MessageStatus;
  priority?: MessagePriority;
  businessType?: string;
  businessId?: string;
  isRead?: boolean;
  keyword?: string; // 关键词搜索（标题、内容）
  startTime?: string; // ISO 8601 格式日期时间字符串
  endTime?: string; // ISO 8601 格式日期时间字符串
  tags?: string;
  
  // 分页和排序（继承自 PagedAndSortedResultRequestDto）
  skipCount?: number; // 跳过记录数，默认 0
  maxResultCount?: number; // 最大返回记录数，默认 10
  sorting?: string; // 排序字段，格式如 "creationTime desc" 或 "title asc"
}

export interface MessageStatisticsDto {
  totalCount: number; // long 类型，前端使用 number
  unreadCount: number; // long 类型，前端使用 number
  readCount: number; // long 类型，前端使用 number
  // 注意：后端返回的是 Dictionary<枚举, long>，前端需要转换为 Record<string, number>
  // 枚举值会作为字符串键（如 "1", "2" 等）
  statusStatistics: Record<string, number>; // 键为 MessageStatus 枚举值的字符串形式
  typeStatistics: Record<string, number>; // 键为 MessageType 枚举值的字符串形式
  channelStatistics: Record<string, number>; // 键为 MessageChannel 枚举值的字符串形式
  startTime?: string; // ISO 8601 格式日期时间字符串（统计时间范围）
  endTime?: string; // ISO 8601 格式日期时间字符串（统计时间范围）
}

export interface PagedResultDto<T> {
  totalCount: number;
  items: T[];
}

export interface RealtimeConnectionInfo {
  hubUrl: string;
  accessToken?: string;
  supportedMethods: string[];
}
```

### 模板类型

创建 `src/types/template.ts`：

```typescript
export enum TemplateType {
  Text = 1,
  Html = 2,
  Markdown = 3,
  Json = 4,
}

export interface CreateMessageTemplateDto {
  name: string;
  code: string; // 模板代码，需唯一
  templateType: TemplateType;
  messageType: MessageType;
  channel: MessageChannel;
  title: string;
  content: string; // 模板内容，支持变量占位符
  description?: string;
  isEnabled?: boolean; // 默认 true
  variables?: string; // JSON 格式字符串，说明模板变量
  extension?: string; // JSON 格式字符串
}

export interface MessageTemplateDto {
  // 基础字段（继承自 FullAuditedEntityDto<Guid>）
  id: string; // Guid 转 string
  creationTime: string; // ISO 8601 格式日期时间字符串
  lastModificationTime?: string; // ISO 8601 格式日期时间字符串
  creatorId?: string; // Guid 转 string（审计字段）
  lastModifierId?: string; // Guid 转 string（审计字段）
  
  // 模板信息
  name: string;
  code: string;
  templateType: TemplateType;
  messageType: MessageType;
  channel: MessageChannel;
  title: string;
  content: string;
  description?: string;
  isEnabled: boolean;
  variables?: string; // JSON 格式字符串
  extension?: string; // JSON 格式字符串
}
```

---

## HTTP 客户端配置

### Axios 配置

创建 `src/api/client.ts`：

```typescript
import axios, { AxiosInstance, AxiosError, InternalAxiosRequestConfig } from 'axios';

// 创建 axios 实例
const apiClient: AxiosInstance = axios.create({
  baseURL: import.meta.env.VITE_API_BASE_URL || 'http://localhost:5000',
  timeout: 30000,
  headers: {
    'Content-Type': 'application/json',
  },
});

// 请求拦截器 - 添加认证 Token
apiClient.interceptors.request.use(
  (config: InternalAxiosRequestConfig) => {
    // 从 localStorage 或状态管理获取 token
    const token = localStorage.getItem('access_token');
    if (token && config.headers) {
      config.headers.Authorization = `Bearer ${token}`;
    }
    return config;
  },
  (error: AxiosError) => {
    return Promise.reject(error);
  }
);

// 响应拦截器 - 统一错误处理
apiClient.interceptors.response.use(
  (response) => {
    return response;
  },
  (error: AxiosError) => {
    // 处理 HTTP 错误
    if (error.response) {
      const { status, data } = error.response;
      
      switch (status) {
        case 401:
          // 未授权，清除 token 并跳转到登录页
          localStorage.removeItem('access_token');
          window.location.href = '/login';
          break;
        case 403:
          console.error('权限不足');
          break;
        case 404:
          console.error('资源不存在');
          break;
        case 500:
          console.error('服务器内部错误');
          break;
        default:
          console.error('请求失败:', data);
      }
    } else if (error.request) {
      // 请求已发出但没有收到响应
      console.error('网络错误，请检查网络连接');
    } else {
      // 其他错误
      console.error('请求配置错误:', error.message);
    }
    
    return Promise.reject(error);
  }
);

export default apiClient;
```

---

## API 封装

### 消息 API

创建 `src/api/message.ts`：

```typescript
import apiClient from './client';
import type {
  CreateMessageDto,
  MessageDto,
  MessageQueryDto,
  MessageStatisticsDto,
  PagedResultDto,
  RealtimeConnectionInfo,
} from '@/types/message';

/**
 * 消息 API
 */
export const messageApi = {
  /**
   * 创建并发送消息
   */
  create: async (data: CreateMessageDto): Promise<MessageDto> => {
    const response = await apiClient.post<MessageDto>('/api/messages', data);
    return response.data;
  },

  /**
   * 批量创建并发送消息
   * 注意：最多支持 1000 条消息
   */
  createBatch: async (data: CreateMessageDto[]): Promise<MessageDto[]> => {
    if (data.length > 1000) {
      throw new Error('批量消息数量不能超过 1000 条');
    }
    const response = await apiClient.post<MessageDto[]>('/api/messages/batch', data);
    return response.data;
  },

  /**
   * 根据ID获取消息
   * @param id 消息ID（Guid 字符串格式）
   */
  getById: async (id: string): Promise<MessageDto> => {
    const response = await apiClient.get<MessageDto>(`/api/messages/${id}`);
    return response.data;
  },

  /**
   * 查询消息列表
   */
  getList: async (params: MessageQueryDto): Promise<PagedResultDto<MessageDto>> => {
    const response = await apiClient.get<PagedResultDto<MessageDto>>('/api/messages', {
      params,
    });
    return response.data;
  },

  /**
   * 获取接收者的消息列表
   */
  getReceiverMessages: async (
    receiverId: string,
    params?: MessageQueryDto
  ): Promise<PagedResultDto<MessageDto>> => {
    const response = await apiClient.get<PagedResultDto<MessageDto>>(
      `/api/messages/receiver/${receiverId}`,
      { params }
    );
    return response.data;
  },

  /**
   * 标记消息为已读
   */
  markAsRead: async (id: string): Promise<void> => {
    await apiClient.put(`/api/messages/${id}/read`);
  },

  /**
   * 批量标记消息为已读
   * 注意：后端接收 List<Guid>，前端传递 string[]（Guid 字符串数组）
   */
  markAsReadBatch: async (ids: string[]): Promise<void> => {
    await apiClient.put('/api/messages/read/batch', ids);
  },

  /**
   * 标记所有消息为已读
   */
  markAllAsRead: async (receiverId: string): Promise<void> => {
    await apiClient.put(`/api/messages/read/all/${receiverId}`);
  },

  /**
   * 删除消息
   */
  delete: async (id: string): Promise<void> => {
    await apiClient.delete(`/api/messages/${id}`);
  },

  /**
   * 批量删除消息
   * 注意：后端接收 List<Guid>，前端传递 string[]（Guid 字符串数组）
   */
  deleteBatch: async (ids: string[]): Promise<void> => {
    await apiClient.delete('/api/messages/batch', { data: ids });
  },

  /**
   * 获取未读消息数量
   * 注意：后端返回 long 类型，前端接收为 number
   */
  getUnreadCount: async (receiverId: string): Promise<number> => {
    const response = await apiClient.get<number>(`/api/messages/unread-count/${receiverId}`);
    return response.data;
  },

  /**
   * 获取消息统计信息
   */
  getStatistics: async (
    receiverId?: string,
    startTime?: string,
    endTime?: string
  ): Promise<MessageStatisticsDto> => {
    const response = await apiClient.get<MessageStatisticsDto>('/api/messages/statistics', {
      params: { receiverId, startTime, endTime },
    });
    return response.data;
  },

  /**
   * 重试发送失败的消息
   * 注意：只有状态为 Failed 且未达到最大重试次数的消息才能重试
   * @param id 消息ID（Guid 字符串格式）
   */
  retry: async (id: string): Promise<void> => {
    await apiClient.post(`/api/messages/${id}/retry`);
  },

  /**
   * 取消消息发送
   * 注意：只有未发送的消息（Pending 状态）才能取消
   * @param id 消息ID（Guid 字符串格式）
   */
  cancel: async (id: string): Promise<void> => {
    await apiClient.post(`/api/messages/${id}/cancel`);
  },

  /**
   * 获取SignalR连接信息
   */
  getRealtimeInfo: async (): Promise<RealtimeConnectionInfo> => {
    const response = await apiClient.get<RealtimeConnectionInfo>('/api/messages/realtime/info');
    return response.data;
  },
};
```

### 模板 API

创建 `src/api/template.ts`：

```typescript
import apiClient from './client';
import type {
  CreateMessageTemplateDto,
  MessageTemplateDto,
} from '@/types/template';

/**
 * 消息模板 API
 */
export const templateApi = {
  /**
   * 创建消息模板
   */
  create: async (data: CreateMessageTemplateDto): Promise<MessageTemplateDto> => {
    const response = await apiClient.post<MessageTemplateDto>('/api/message-templates', data);
    return response.data;
  },

  /**
   * 更新消息模板
   */
  update: async (id: string, data: CreateMessageTemplateDto): Promise<MessageTemplateDto> => {
    const response = await apiClient.put<MessageTemplateDto>(`/api/message-templates/${id}`, data);
    return response.data;
  },

  /**
   * 根据ID获取消息模板
   */
  getById: async (id: string): Promise<MessageTemplateDto> => {
    const response = await apiClient.get<MessageTemplateDto>(`/api/message-templates/${id}`);
    return response.data;
  },

  /**
   * 根据代码获取消息模板
   */
  getByCode: async (code: string): Promise<MessageTemplateDto> => {
    const response = await apiClient.get<MessageTemplateDto>(`/api/message-templates/code/${code}`);
    return response.data;
  },

  /**
   * 获取消息模板列表
   */
  getList: async (): Promise<MessageTemplateDto[]> => {
    const response = await apiClient.get<MessageTemplateDto[]>('/api/message-templates');
    return response.data;
  },

  /**
   * 删除消息模板
   */
  delete: async (id: string): Promise<void> => {
    await apiClient.delete(`/api/message-templates/${id}`);
  },

  /**
   * 启用/禁用消息模板
   * 注意：请求体直接传递 boolean 值，不是对象
   */
  setEnabled: async (id: string, isEnabled: boolean): Promise<void> => {
    await apiClient.put(`/api/message-templates/${id}/enabled`, isEnabled);
  },
};
```

---

## 错误处理

### 错误类型定义

创建 `src/types/error.ts`：

```typescript
export interface ApiError {
  code: string;
  message: string;
  details?: string;
}

export interface ErrorResponse {
  error: ApiError;
}
```

### 错误处理工具

创建 `src/utils/errorHandler.ts`：

```typescript
import { AxiosError } from 'axios';
import type { ErrorResponse } from '@/types/error';
import { message } from 'antd';

/**
 * 处理 API 错误
 */
export function handleApiError(error: unknown): void {
  if (error instanceof AxiosError) {
    const errorResponse = error.response?.data as ErrorResponse;
    
    if (errorResponse?.error) {
      const { code, message: errorMessage, details } = errorResponse.error;
      
      // 根据错误代码显示不同的提示
      switch (code) {
        case 'INVALID_INPUT':
          message.error(errorMessage || '请求参数无效');
          break;
        case 'UNAUTHORIZED':
          message.error('未授权，请重新登录');
          // 跳转到登录页
          window.location.href = '/login';
          break;
        case 'FORBIDDEN':
          message.error('权限不足');
          break;
        case 'NOT_FOUND':
          message.error('资源不存在');
          break;
        default:
          message.error(errorMessage || '操作失败');
      }
      
      // 开发环境显示详细错误信息
      if (import.meta.env.DEV && details) {
        console.error('错误详情:', details);
      }
    } else {
      // 处理 HTTP 状态码错误
      const status = error.response?.status;
      switch (status) {
        case 400:
          message.error('请求参数错误');
          break;
        case 401:
          message.error('未授权，请重新登录');
          window.location.href = '/login';
          break;
        case 403:
          message.error('权限不足');
          break;
        case 404:
          message.error('资源不存在');
          break;
        case 500:
          message.error('服务器内部错误');
          break;
        default:
          message.error('网络错误，请稍后重试');
      }
    }
  } else if (error instanceof Error) {
    message.error(error.message);
  } else {
    message.error('未知错误');
  }
}
```

---

## 请求拦截器

### Token 刷新拦截器

在 `src/api/client.ts` 中添加 Token 刷新逻辑：

```typescript
// Token 刷新逻辑
let isRefreshing = false;
let failedQueue: Array<{
  resolve: (value?: any) => void;
  reject: (reason?: any) => void;
}> = [];

const processQueue = (error: AxiosError | null, token: string | null = null) => {
  failedQueue.forEach((prom) => {
    if (error) {
      prom.reject(error);
    } else {
      prom.resolve(token);
    }
  });
  
  failedQueue = [];
};

// 在响应拦截器中添加 401 处理
apiClient.interceptors.response.use(
  (response) => response,
  async (error: AxiosError) => {
    const originalRequest = error.config as InternalAxiosRequestConfig & { _retry?: boolean };
    
    if (error.response?.status === 401 && !originalRequest._retry) {
      if (isRefreshing) {
        // 如果正在刷新，将请求加入队列
        return new Promise((resolve, reject) => {
          failedQueue.push({ resolve, reject });
        })
          .then((token) => {
            if (originalRequest.headers) {
              originalRequest.headers.Authorization = `Bearer ${token}`;
            }
            return apiClient(originalRequest);
          })
          .catch((err) => {
            return Promise.reject(err);
          });
      }

      originalRequest._retry = true;
      isRefreshing = true;

      try {
        // 调用刷新 Token 的 API
        const refreshToken = localStorage.getItem('refresh_token');
        const response = await axios.post('/api/auth/refresh', {
          refreshToken,
        });
        
        const { accessToken } = response.data;
        localStorage.setItem('access_token', accessToken);
        
        if (originalRequest.headers) {
          originalRequest.headers.Authorization = `Bearer ${accessToken}`;
        }
        
        processQueue(null, accessToken);
        return apiClient(originalRequest);
      } catch (refreshError) {
        processQueue(refreshError as AxiosError, null);
        localStorage.removeItem('access_token');
        localStorage.removeItem('refresh_token');
        window.location.href = '/login';
        return Promise.reject(refreshError);
      } finally {
        isRefreshing = false;
      }
    }

    return Promise.reject(error);
  }
);
```

---

## 响应处理

### 响应数据转换

创建 `src/utils/response.ts`：

```typescript
import type { PagedResultDto } from '@/types/message';
import dayjs from 'dayjs';

/**
 * 格式化日期时间
 */
export function formatDateTime(dateTime?: string): string {
  if (!dateTime) return '-';
  return dayjs(dateTime).format('YYYY-MM-DD HH:mm:ss');
}

/**
 * 格式化日期
 */
export function formatDate(date?: string): string {
  if (!date) return '-';
  return dayjs(date).format('YYYY-MM-DD');
}

/**
 * 处理分页响应
 */
export function handlePagedResponse<T>(
  response: PagedResultDto<T>
): {
  data: T[];
  total: number;
  current: number;
  pageSize: number;
} {
  return {
    data: response.items || [],
    total: response.totalCount || 0,
    current: 1, // 需要根据 skipCount 和 maxResultCount 计算
    pageSize: 10, // 需要从请求参数中获取
  };
}
```

---

## 使用示例

### 在组件中使用 API

```typescript
import { useState, useEffect } from 'react';
import { messageApi } from '@/api/message';
import { handleApiError } from '@/utils/errorHandler';
import { message } from 'antd';
import type { MessageDto, MessageQueryDto, MessageStatus } from '@/types/message';

function MessageList() {
  const [messages, setMessages] = useState<MessageDto[]>([]);
  const [loading, setLoading] = useState(false);
  const [totalCount, setTotalCount] = useState(0);
  const [currentPage, setCurrentPage] = useState(1);
  const pageSize = 20;

  useEffect(() => {
    loadMessages();
  }, [currentPage]);

  const loadMessages = async () => {
    try {
      setLoading(true);
      const queryParams: MessageQueryDto = {
        receiverId: 'user123', // 从认证信息或状态管理获取
        skipCount: (currentPage - 1) * pageSize,
        maxResultCount: pageSize,
        sorting: 'creationTime desc', // 按创建时间倒序
      };
      
      const result = await messageApi.getList(queryParams);
      setMessages(result.items);
      setTotalCount(result.totalCount);
    } catch (error) {
      handleApiError(error);
    } finally {
      setLoading(false);
    }
  };

  const handleMarkAsRead = async (id: string) => {
    try {
      await messageApi.markAsRead(id);
      message.success('已标记为已读');
      // 更新本地状态，避免重新加载
      setMessages((prev) =>
        prev.map((msg) =>
          msg.id === id ? { ...msg, isRead: true, status: MessageStatus.Read } : msg
        )
      );
    } catch (error) {
      handleApiError(error);
    }
  };

  return (
    <div>
      {/* 消息列表 UI */}
    </div>
  );
}
```

### 处理统计数据的类型转换

```typescript
import type { MessageStatisticsDto } from '@/types/message';
import { MessageStatus, MessageType, MessageChannel } from '@/types/message';

// 后端返回的统计字典键是枚举值的字符串形式，需要转换
function formatStatistics(statistics: MessageStatisticsDto) {
  // 将字符串键转换为枚举值
  const statusStats = Object.entries(statistics.statusStatistics).map(([key, value]) => ({
    status: Number(key) as MessageStatus,
    count: value,
  }));

  const typeStats = Object.entries(statistics.typeStatistics).map(([key, value]) => ({
    type: Number(key) as MessageType,
    count: value,
  }));

  const channelStats = Object.entries(statistics.channelStatistics).map(([key, value]) => ({
    channel: Number(key) as MessageChannel,
    count: value,
  }));

  return { statusStats, typeStats, channelStats };
}
```

---



---

## 第三部分：SignalR 实时通信

## 概述

本部分详细介绍如何集成 SignalR 实时通信功能，包括连接管理、事件处理、重连机制和最佳实践。

## 目录

- [SignalR 服务封装](#signalr-服务封装)
- [React Hook 封装](#react-hook-封装)
- [连接管理](#连接管理)
- [事件处理](#事件处理)
- [状态管理集成](#状态管理集成)
- [错误处理和重连](#错误处理和重连)
- [性能优化](#性能优化)

---

## SignalR 服务封装

### 基础服务类

创建 `src/services/realtimeService.ts`：

```typescript
import * as signalR from '@microsoft/signalr';
import type { MessageDto } from '@/types/message';
import { messageApi } from '@/api/message';

/**
 * SignalR 连接状态
 */
export enum ConnectionState {
  Disconnected = 'Disconnected',
  Connecting = 'Connecting',
  Connected = 'Connected',
  Reconnecting = 'Reconnecting',
  Disconnecting = 'Disconnecting',
}

/**
 * 实时消息通知
 */
export interface NewMessageNotification {
  receiverId: string;
  unreadCount: number;
  timestamp: string;
}

/**
 * 消息状态变更通知
 */
export interface MessageStatusChangedNotification {
  messageId: string;
  status: string;
  timestamp: string;
}

/**
 * SignalR 实时通信服务
 */
export class RealtimeService {
  private connection: signalR.HubConnection | null = null;
  private connectionState: ConnectionState = ConnectionState.Disconnected;
  private reconnectAttempts = 0;
  private maxReconnectAttempts = 5;
  private reconnectDelay = 1000; // 初始重连延迟（毫秒）

  // 事件回调
  private onMessageCallbacks: Array<(message: MessageDto) => void> = [];
  private onNewMessageCallbacks: Array<(notification: NewMessageNotification) => void> = [];
  private onStatusChangedCallbacks: Array<(notification: MessageStatusChangedNotification) => void> = [];
  private onStateChangedCallbacks: Array<(state: ConnectionState) => void> = [];

  /**
   * 获取连接状态
   */
  getState(): ConnectionState {
    return this.connectionState;
  }

  /**
   * 是否已连接
   */
  isConnected(): boolean {
    return this.connectionState === ConnectionState.Connected;
  }

  /**
   * 建立连接
   */
  async connect(): Promise<void> {
    if (this.connection && this.isConnected()) {
      console.log('SignalR 已连接，跳过重复连接');
      return;
    }

    try {
      this.updateState(ConnectionState.Connecting);

      // 获取连接信息
      const connectionInfo = await messageApi.getRealtimeInfo();
      
      // 创建连接
      this.connection = new signalR.HubConnectionBuilder()
        .withUrl(connectionInfo.hubUrl, {
          accessTokenFactory: () => {
            // 优先使用 API 返回的 token，否则从 localStorage 获取
            return connectionInfo.accessToken || localStorage.getItem('access_token') || '';
          },
        })
        .withAutomaticReconnect({
          nextRetryDelayInMilliseconds: (retryContext) => {
            // 指数退避策略
            if (retryContext.previousRetryCount < 3) {
              return 1000 * Math.pow(2, retryContext.previousRetryCount);
            }
            return 10000; // 最多等待 10 秒
          },
        })
        .configureLogging(signalR.LogLevel.Information)
        .build();

      // 注册事件处理器
      this.registerHandlers();

      // 启动连接
      await this.connection.start();
      this.updateState(ConnectionState.Connected);
      this.reconnectAttempts = 0;
      console.log('SignalR 连接已建立');

      // 加入用户组（如果需要）
      const userId = this.getUserId();
      if (userId) {
        await this.joinUserGroup(userId);
      }
    } catch (error) {
      console.error('SignalR 连接失败:', error);
      this.updateState(ConnectionState.Disconnected);
      throw error;
    }
  }

  /**
   * 断开连接
   */
  async disconnect(): Promise<void> {
    if (!this.connection) {
      return;
    }

    try {
      this.updateState(ConnectionState.Disconnecting);
      await this.connection.stop();
      this.connection = null;
      this.updateState(ConnectionState.Disconnected);
      console.log('SignalR 连接已断开');
    } catch (error) {
      console.error('断开 SignalR 连接失败:', error);
      throw error;
    }
  }

  /**
   * 注册事件处理器
   */
  private registerHandlers(): void {
    if (!this.connection) return;

    // 接收完整消息
    this.connection.on('ReceiveMessage', (message: MessageDto) => {
      console.log('收到新消息:', message);
      this.onMessageCallbacks.forEach((callback) => callback(message));
    });

    // 接收新消息通知（轻量级）
    this.connection.on('NotifyNewMessage', (notification: NewMessageNotification) => {
      console.log('收到新消息通知:', notification);
      this.onNewMessageCallbacks.forEach((callback) => callback(notification));
    });

    // 接收消息状态变更通知
    this.connection.on('MessageStatusChanged', (notification: MessageStatusChangedNotification) => {
      console.log('消息状态变更:', notification);
      this.onStatusChangedCallbacks.forEach((callback) => callback(notification));
    });

    // 连接事件
    this.connection.onclose((error) => {
      console.log('SignalR 连接已关闭', error);
      this.updateState(ConnectionState.Disconnected);
      
      if (error) {
        // 有错误，尝试重连
        this.handleReconnect();
      }
    });

    this.connection.onreconnecting((error) => {
      console.log('SignalR 正在重连...', error);
      this.updateState(ConnectionState.Reconnecting);
    });

    this.connection.onreconnected((connectionId) => {
      console.log('SignalR 已重新连接，连接ID:', connectionId);
      this.updateState(ConnectionState.Connected);
      this.reconnectAttempts = 0;
    });
  }

  /**
   * 处理重连
   */
  private async handleReconnect(): Promise<void> {
    if (this.reconnectAttempts >= this.maxReconnectAttempts) {
      console.error('达到最大重连次数，停止重连');
      return;
    }

    this.reconnectAttempts++;
    const delay = this.reconnectDelay * Math.pow(2, this.reconnectAttempts - 1);
    
    console.log(`等待 ${delay}ms 后尝试重连 (${this.reconnectAttempts}/${this.maxReconnectAttempts})`);
    
    setTimeout(async () => {
      try {
        await this.connect();
      } catch (error) {
        console.error('重连失败:', error);
        this.handleReconnect();
      }
    }, delay);
  }

  /**
   * 加入用户组
   */
  private async joinUserGroup(userId: string): Promise<void> {
    if (!this.connection || !this.isConnected()) {
      return;
    }

    try {
      // 用户组由服务器自动管理，这里可以加入其他业务组
      // await this.connection.invoke('JoinGroup', `user_${userId}`);
      console.log('用户组已自动加入');
    } catch (error) {
      console.error('加入用户组失败:', error);
    }
  }

  /**
   * 加入业务组
   */
  async joinGroup(groupName: string): Promise<void> {
    if (!this.connection || !this.isConnected()) {
      throw new Error('SignalR 未连接');
    }

    try {
      await this.connection.invoke('JoinGroup', groupName);
      console.log(`已加入组: ${groupName}`);
    } catch (error) {
      console.error('加入组失败:', error);
      throw error;
    }
  }

  /**
   * 离开业务组
   */
  async leaveGroup(groupName: string): Promise<void> {
    if (!this.connection || !this.isConnected()) {
      throw new Error('SignalR 未连接');
    }

    try {
      await this.connection.invoke('LeaveGroup', groupName);
      console.log(`已离开组: ${groupName}`);
    } catch (error) {
      console.error('离开组失败:', error);
      throw error;
    }
  }

  /**
   * 订阅消息事件
   */
  onMessage(callback: (message: MessageDto) => void): () => void {
    this.onMessageCallbacks.push(callback);
    return () => {
      const index = this.onMessageCallbacks.indexOf(callback);
      if (index > -1) {
        this.onMessageCallbacks.splice(index, 1);
      }
    };
  }

  /**
   * 订阅新消息通知事件
   */
  onNewMessage(callback: (notification: NewMessageNotification) => void): () => void {
    this.onNewMessageCallbacks.push(callback);
    return () => {
      const index = this.onNewMessageCallbacks.indexOf(callback);
      if (index > -1) {
        this.onNewMessageCallbacks.splice(index, 1);
      }
    };
  }

  /**
   * 订阅消息状态变更事件
   */
  onStatusChanged(callback: (notification: MessageStatusChangedNotification) => void): () => void {
    this.onStatusChangedCallbacks.push(callback);
    return () => {
      const index = this.onStatusChangedCallbacks.indexOf(callback);
      if (index > -1) {
        this.onStatusChangedCallbacks.splice(index, 1);
      }
    };
  }

  /**
   * 订阅连接状态变更事件
   */
  onStateChanged(callback: (state: ConnectionState) => void): () => void {
    this.onStateChangedCallbacks.push(callback);
    return () => {
      const index = this.onStateChangedCallbacks.indexOf(callback);
      if (index > -1) {
        this.onStateChangedCallbacks.splice(index, 1);
      }
    };
  }

  /**
   * 更新连接状态
   */
  private updateState(state: ConnectionState): void {
    if (this.connectionState !== state) {
      this.connectionState = state;
      this.onStateChangedCallbacks.forEach((callback) => callback(state));
    }
  }

  /**
   * 获取当前用户ID
   */
  private getUserId(): string | null {
    // 从 localStorage 或状态管理获取用户ID
    return localStorage.getItem('user_id');
  }
}

// 导出单例实例
export const realtimeService = new RealtimeService();
```

---

## React Hook 封装

### useRealtime Hook

创建 `src/hooks/useRealtime.ts`：

```typescript
import { useEffect, useState, useCallback, useRef } from 'react';
import { realtimeService, ConnectionState, type MessageDto, type NewMessageNotification, type MessageStatusChangedNotification } from '@/services/realtimeService';

/**
 * SignalR 实时通信 Hook
 */
export function useRealtime() {
  const [connectionState, setConnectionState] = useState<ConnectionState>(
    realtimeService.getState()
  );
  const [isConnected, setIsConnected] = useState(realtimeService.isConnected());

  useEffect(() => {
    // 订阅连接状态变更
    const unsubscribe = realtimeService.onStateChanged((state) => {
      setConnectionState(state);
      setIsConnected(state === ConnectionState.Connected);
    });

    return unsubscribe;
  }, []);

  const connect = useCallback(async () => {
    try {
      await realtimeService.connect();
    } catch (error) {
      console.error('连接失败:', error);
      throw error;
    }
  }, []);

  const disconnect = useCallback(async () => {
    try {
      await realtimeService.disconnect();
    } catch (error) {
      console.error('断开连接失败:', error);
      throw error;
    }
  }, []);

  return {
    connectionState,
    isConnected,
    connect,
    disconnect,
  };
}

/**
 * 消息接收 Hook
 */
export function useRealtimeMessage(
  onMessage?: (message: MessageDto) => void,
  onNewMessage?: (notification: NewMessageNotification) => void,
  onStatusChanged?: (notification: MessageStatusChangedNotification) => void
) {
  const onMessageRef = useRef(onMessage);
  const onNewMessageRef = useRef(onNewMessage);
  const onStatusChangedRef = useRef(onStatusChanged);

  // 更新 ref，避免闭包问题
  useEffect(() => {
    onMessageRef.current = onMessage;
    onNewMessageRef.current = onNewMessage;
    onStatusChangedRef.current = onStatusChanged;
  }, [onMessage, onNewMessage, onStatusChanged]);

  useEffect(() => {
    const unsubscribers: Array<() => void> = [];

    if (onMessageRef.current) {
      const unsubscribe = realtimeService.onMessage((message) => {
        onMessageRef.current?.(message);
      });
      unsubscribers.push(unsubscribe);
    }

    if (onNewMessageRef.current) {
      const unsubscribe = realtimeService.onNewMessage((notification) => {
        onNewMessageRef.current?.(notification);
      });
      unsubscribers.push(unsubscribe);
    }

    if (onStatusChangedRef.current) {
      const unsubscribe = realtimeService.onStatusChanged((notification) => {
        onStatusChangedRef.current?.(notification);
      });
      unsubscribers.push(unsubscribe);
    }

    return () => {
      unsubscribers.forEach((unsubscribe) => unsubscribe());
    };
  }, []);
}
```

---

## 连接管理

### 应用启动时连接

在 `src/App.tsx` 中：

```typescript
import { useEffect } from 'react';
import { useRealtime } from '@/hooks/useRealtime';
import { ConnectionState } from '@/services/realtimeService';

function App() {
  const { connect, connectionState, isConnected } = useRealtime();

  useEffect(() => {
    // 应用启动时建立连接
    connect().catch((error) => {
      console.error('SignalR 连接失败:', error);
    });

    // 应用关闭时断开连接
    return () => {
      realtimeService.disconnect().catch(console.error);
    };
  }, [connect]);

  return (
    <div>
      {/* 连接状态指示器 */}
      <ConnectionIndicator state={connectionState} />
      
      {/* 应用内容 */}
      <YourAppContent />
    </div>
  );
}

// 连接状态指示器组件
function ConnectionIndicator({ state }: { state: ConnectionState }) {
  const getStatusColor = () => {
    switch (state) {
      case ConnectionState.Connected:
        return 'green';
      case ConnectionState.Connecting:
      case ConnectionState.Reconnecting:
        return 'orange';
      case ConnectionState.Disconnected:
      case ConnectionState.Disconnecting:
        return 'red';
      default:
        return 'gray';
    }
  };

  const getStatusText = () => {
    switch (state) {
      case ConnectionState.Connected:
        return '已连接';
      case ConnectionState.Connecting:
        return '连接中...';
      case ConnectionState.Reconnecting:
        return '重连中...';
      case ConnectionState.Disconnected:
        return '未连接';
      case ConnectionState.Disconnecting:
        return '断开中...';
      default:
        return '未知';
    }
  };

  return (
    <div style={{ color: getStatusColor(), fontSize: '12px' }}>
      {getStatusText()}
    </div>
  );
}
```

---

## 事件处理

### 在组件中使用实时消息

```typescript
import { useEffect, useState } from 'react';
import { useRealtime, useRealtimeMessage } from '@/hooks/useRealtime';
import { messageApi } from '@/api/message';
import { message } from 'antd';
import type { MessageDto, NewMessageNotification } from '@/types/message';

function MessageCenter() {
  const { isConnected, connect } = useRealtime();
  const [unreadCount, setUnreadCount] = useState(0);
  const [messages, setMessages] = useState<MessageDto[]>([]);

  // 处理接收到的完整消息
  const handleMessage = (msg: MessageDto) => {
    setMessages((prev) => [msg, ...prev]);
    message.info(`收到新消息: ${msg.title}`);
  };

  // 处理新消息通知（轻量级）
  const handleNewMessage = (notification: NewMessageNotification) => {
    setUnreadCount(notification.unreadCount);
    // 可以选择是否自动刷新消息列表
    // loadMessages();
  };

  // 处理消息状态变更
  const handleStatusChanged = (notification: { messageId: string; status: string }) => {
    setMessages((prev) =>
      prev.map((msg) =>
        msg.id === notification.messageId
          ? { ...msg, status: notification.status as any }
          : msg
      )
    );
  };

  // 订阅实时消息事件
  useRealtimeMessage(handleMessage, handleNewMessage, handleStatusChanged);

  // 加载未读数量
  useEffect(() => {
    if (isConnected) {
      loadUnreadCount();
    }
  }, [isConnected]);

  const loadUnreadCount = async () => {
    try {
      const userId = localStorage.getItem('user_id');
      if (userId) {
        const count = await messageApi.getUnreadCount(userId);
        setUnreadCount(count);
      }
    } catch (error) {
      console.error('加载未读数量失败:', error);
    }
  };

  return (
    <div>
      <div>未读消息: {unreadCount}</div>
      {/* 消息列表 */}
    </div>
  );
}
```

---

## 状态管理集成

### 与 Zustand 集成

创建 `src/stores/realtimeStore.ts`：

```typescript
import { create } from 'zustand';
import { realtimeService, ConnectionState } from '@/services/realtimeService';
import type { MessageDto, NewMessageNotification } from '@/types/message';

interface RealtimeState {
  connectionState: ConnectionState;
  isConnected: boolean;
  unreadCount: number;
  recentMessages: MessageDto[];
  
  // Actions
  setConnectionState: (state: ConnectionState) => void;
  setUnreadCount: (count: number) => void;
  addMessage: (message: MessageDto) => void;
  updateMessageStatus: (messageId: string, status: string) => void;
  connect: () => Promise<void>;
  disconnect: () => Promise<void>;
}

export const useRealtimeStore = create<RealtimeState>((set, get) => {
  // 初始化时订阅状态变更
  realtimeService.onStateChanged((state) => {
    set({
      connectionState: state,
      isConnected: state === ConnectionState.Connected,
    });
  });

  // 订阅消息事件
  realtimeService.onMessage((message) => {
    set((state) => ({
      recentMessages: [message, ...state.recentMessages.slice(0, 49)], // 保留最近50条
    }));
  });

  // 订阅新消息通知
  realtimeService.onNewMessage((notification) => {
    set({ unreadCount: notification.unreadCount });
  });

  return {
    connectionState: ConnectionState.Disconnected,
    isConnected: false,
    unreadCount: 0,
    recentMessages: [],

    setConnectionState: (state) => set({ connectionState: state }),
    setUnreadCount: (count) => set({ unreadCount: count }),
    addMessage: (message) =>
      set((state) => ({
        recentMessages: [message, ...state.recentMessages.slice(0, 49)],
      })),
    updateMessageStatus: (messageId, status) =>
      set((state) => ({
        recentMessages: state.recentMessages.map((msg) =>
          msg.id === messageId ? { ...msg, status: status as any } : msg
        ),
      })),
    connect: async () => {
      await realtimeService.connect();
    },
    disconnect: async () => {
      await realtimeService.disconnect();
    },
  };
});
```

---

## 错误处理和重连

### 增强的错误处理

在 `src/services/realtimeService.ts` 中增强错误处理：

```typescript
// 在 RealtimeService 类中添加
private handleError(error: Error | string): void {
  const errorMessage = error instanceof Error ? error.message : error;
  console.error('SignalR 错误:', errorMessage);
  
  // 可以发送错误到监控系统
  // errorTrackingService.track(error);
  
  // 显示用户友好的错误提示
  // message.error('实时通信连接异常，正在重连...');
}
```

---

## 性能优化

### 消息去重和节流

```typescript
// 在 RealtimeService 中添加消息去重
private messageCache = new Map<string, MessageDto>();
private readonly CACHE_TTL = 60000; // 1分钟

private isDuplicateMessage(message: MessageDto): boolean {
  const cached = this.messageCache.get(message.id);
  if (cached) {
    return true;
  }
  
  this.messageCache.set(message.id, message);
  setTimeout(() => {
    this.messageCache.delete(message.id);
  }, this.CACHE_TTL);
  
  return false;
}

// 在 registerHandlers 中使用
this.connection.on('ReceiveMessage', (message: MessageDto) => {
  if (this.isDuplicateMessage(message)) {
    console.log('收到重复消息，已忽略:', message.id);
    return;
  }
  
  console.log('收到新消息:', message);
  this.onMessageCallbacks.forEach((callback) => callback(message));
});
```

---



---

## 第四部分：状态管理和最佳实践

## 概述

本部分介绍状态管理、组件设计、性能优化、测试策略和部署最佳实践。

## 目录

- [状态管理](#状态管理)
- [组件设计模式](#组件设计模式)
- [性能优化](#性能优化)
- [测试策略](#测试策略)
- [代码规范](#代码规范)
- [部署和构建](#部署和构建)
- [常见问题解决](#常见问题解决)

---

## 状态管理

### 使用 Zustand 管理消息状态

创建 `src/stores/messageStore.ts`：

```typescript
import { create } from 'zustand';
import { messageApi } from '@/api/message';
import type {
  MessageDto,
  MessageQueryDto,
  MessageStatisticsDto,
  CreateMessageDto,
} from '@/types/message';
import { handleApiError } from '@/utils/errorHandler';

interface MessageState {
  // 状态
  messages: MessageDto[];
  currentMessage: MessageDto | null;
  statistics: MessageStatisticsDto | null;
  unreadCount: number;
  loading: boolean;
  totalCount: number;
  currentPage: number;
  pageSize: number;
  queryParams: MessageQueryDto;

  // Actions
  setMessages: (messages: MessageDto[]) => void;
  setCurrentMessage: (message: MessageDto | null) => void;
  setStatistics: (statistics: MessageStatisticsDto) => void;
  setUnreadCount: (count: number) => void;
  setLoading: (loading: boolean) => void;
  setQueryParams: (params: Partial<MessageQueryDto>) => void;
  
  // API Actions
  loadMessages: (params?: MessageQueryDto) => Promise<void>;
  loadMessage: (id: string) => Promise<void>;
  loadStatistics: (receiverId?: string) => Promise<void>;
  loadUnreadCount: (receiverId: string) => Promise<void>;
  createMessage: (data: CreateMessageDto) => Promise<MessageDto>;
  markAsRead: (id: string) => Promise<void>;
  markAllAsRead: (receiverId: string) => Promise<void>;
  deleteMessage: (id: string) => Promise<void>;
  addMessage: (message: MessageDto) => void;
  updateMessage: (id: string, updates: Partial<MessageDto>) => void;
}

export const useMessageStore = create<MessageState>((set, get) => ({
  // 初始状态
  messages: [],
  currentMessage: null,
  statistics: null,
  unreadCount: 0,
  loading: false,
  totalCount: 0,
  currentPage: 1,
  pageSize: 20,
  queryParams: {
    skipCount: 0,
    maxResultCount: 20,
  },

  // 基础 Actions
  setMessages: (messages) => set({ messages }),
  setCurrentMessage: (message) => set({ currentMessage: message }),
  setStatistics: (statistics) => set({ statistics }),
  setUnreadCount: (count) => set({ unreadCount: count }),
  setLoading: (loading) => set({ loading }),
  setQueryParams: (params) =>
    set((state) => ({
      queryParams: { ...state.queryParams, ...params },
    })),

  // API Actions
  loadMessages: async (params) => {
    try {
      set({ loading: true });
      const queryParams = params || get().queryParams;
      const result = await messageApi.getList(queryParams);
      set({
        messages: result.items,
        totalCount: result.totalCount,
        loading: false,
      });
    } catch (error) {
      set({ loading: false });
      handleApiError(error);
      throw error;
    }
  },

  loadMessage: async (id) => {
    try {
      set({ loading: true });
      const message = await messageApi.getById(id);
      set({ currentMessage: message, loading: false });
    } catch (error) {
      set({ loading: false });
      handleApiError(error);
      throw error;
    }
  },

  loadStatistics: async (receiverId) => {
    try {
      const statistics = await messageApi.getStatistics(receiverId);
      set({ statistics });
    } catch (error) {
      handleApiError(error);
      throw error;
    }
  },

  loadUnreadCount: async (receiverId) => {
    try {
      const count = await messageApi.getUnreadCount(receiverId);
      set({ unreadCount: count });
    } catch (error) {
      handleApiError(error);
      throw error;
    }
  },

  createMessage: async (data) => {
    try {
      set({ loading: true });
      const message = await messageApi.create(data);
      set((state) => ({
        messages: [message, ...state.messages],
        loading: false,
      }));
      return message;
    } catch (error) {
      set({ loading: false });
      handleApiError(error);
      throw error;
    }
  },

  markAsRead: async (id) => {
    try {
      await messageApi.markAsRead(id);
      set((state) => ({
        messages: state.messages.map((msg) =>
          msg.id === id ? { ...msg, isRead: true, status: 5 } : msg
        ),
        unreadCount: Math.max(0, state.unreadCount - 1),
      }));
    } catch (error) {
      handleApiError(error);
      throw error;
    }
  },

  markAllAsRead: async (receiverId) => {
    try {
      await messageApi.markAllAsRead(receiverId);
      set((state) => ({
        messages: state.messages.map((msg) => ({
          ...msg,
          isRead: true,
          status: 5,
        })),
        unreadCount: 0,
      }));
    } catch (error) {
      handleApiError(error);
      throw error;
    }
  },

  deleteMessage: async (id) => {
    try {
      await messageApi.delete(id);
      set((state) => ({
        messages: state.messages.filter((msg) => msg.id !== id),
        totalCount: state.totalCount - 1,
      }));
    } catch (error) {
      handleApiError(error);
      throw error;
    }
  },

  addMessage: (message) =>
    set((state) => ({
      messages: [message, ...state.messages],
      totalCount: state.totalCount + 1,
      unreadCount: state.unreadCount + 1,
    })),

  updateMessage: (id, updates) =>
    set((state) => ({
      messages: state.messages.map((msg) =>
        msg.id === id ? { ...msg, ...updates } : msg
      ),
    })),
}));
```

### 使用 React Query 管理服务端状态

创建 `src/hooks/useMessages.ts`：

```typescript
import { useQuery, useMutation, useQueryClient } from '@tanstack/react-query';
import { messageApi } from '@/api/message';
import type { MessageQueryDto, CreateMessageDto } from '@/types/message';
import { handleApiError } from '@/utils/errorHandler';

/**
 * 查询消息列表
 */
export function useMessages(params: MessageQueryDto) {
  return useQuery({
    queryKey: ['messages', params],
    queryFn: () => messageApi.getList(params),
    staleTime: 30000, // 30秒内不重新获取
  });
}

/**
 * 查询单个消息
 */
export function useMessage(id: string) {
  return useQuery({
    queryKey: ['message', id],
    queryFn: () => messageApi.getById(id),
    enabled: !!id, // 只有 id 存在时才查询
  });
}

/**
 * 查询未读数量
 */
export function useUnreadCount(receiverId: string) {
  return useQuery({
    queryKey: ['unreadCount', receiverId],
    queryFn: () => messageApi.getUnreadCount(receiverId),
    refetchInterval: 60000, // 每60秒自动刷新
  });
}

/**
 * 创建消息 Mutation
 */
export function useCreateMessage() {
  const queryClient = useQueryClient();

  return useMutation({
    mutationFn: (data: CreateMessageDto) => messageApi.create(data),
    onSuccess: () => {
      // 使相关查询失效，触发重新获取
      queryClient.invalidateQueries({ queryKey: ['messages'] });
      queryClient.invalidateQueries({ queryKey: ['unreadCount'] });
    },
    onError: handleApiError,
  });
}

/**
 * 标记为已读 Mutation
 */
export function useMarkAsRead() {
  const queryClient = useQueryClient();

  return useMutation({
    mutationFn: (id: string) => messageApi.markAsRead(id),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ['messages'] });
      queryClient.invalidateQueries({ queryKey: ['unreadCount'] });
    },
    onError: handleApiError,
  });
}
```

---

## 表单处理最佳实践

### 使用 React Hook Form + Zod

React Hook Form 提供了高性能的表单处理方案，结合 Zod 可以实现类型安全的表单验证。

#### 安装依赖

```bash
npm install react-hook-form @hookform/resolvers zod
```

#### 创建验证 Schema

创建 `src/schemas/message.ts`：

```typescript
import { z } from 'zod';
import { MessageType, MessageChannel, MessagePriority } from '@/types/message';

export const createMessageSchema = z.object({
  title: z.string().min(1, '标题不能为空').max(200, '标题不能超过200个字符'),
  content: z.string().min(1, '内容不能为空').max(5000, '内容不能超过5000个字符'),
  summary: z.string().max(500, '摘要不能超过500个字符').optional(),
  messageType: z.nativeEnum(MessageType, {
    required_error: '请选择消息类型',
  }),
  channel: z.nativeEnum(MessageChannel, {
    required_error: '请选择消息渠道',
  }),
  priority: z.nativeEnum(MessagePriority).optional().default(MessagePriority.Normal),
  receiverId: z.string().min(1, '接收者ID不能为空').optional(),
  receiverIds: z.array(z.string()).min(1, '至少需要一个接收者').optional(),
  templateId: z.string().uuid('模板ID格式不正确').optional(),
  scheduledSendTime: z.string().datetime('日期时间格式不正确').optional(),
  expirationTime: z.string().datetime('日期时间格式不正确').optional(),
}).refine(
  (data) => data.receiverId || (data.receiverIds && data.receiverIds.length > 0),
  {
    message: '必须指定接收者ID或接收者ID列表',
    path: ['receiverId'],
  }
);

export type CreateMessageFormData = z.infer<typeof createMessageSchema>;
```

#### 使用 React Hook Form 创建表单组件

创建 `src/components/message/CreateMessageForm.tsx`：

```typescript
import { useForm } from 'react-hook-form';
import { zodResolver } from '@hookform/resolvers/zod';
import { Form, Input, Select, DatePicker, Button, Space } from 'antd';
import { ProFormText, ProFormSelect, ProFormDateTimePicker } from '@ant-design/pro-components';
import { messageApi } from '@/api/message';
import { createMessageSchema, type CreateMessageFormData } from '@/schemas/message';
import { MessageType, MessageChannel, MessagePriority } from '@/types/message';
import dayjs from 'dayjs';

export function CreateMessageForm({ onSuccess }: { onSuccess?: () => void }) {
  const {
    control,
    handleSubmit,
    formState: { errors, isSubmitting },
    reset,
  } = useForm<CreateMessageFormData>({
    resolver: zodResolver(createMessageSchema),
    defaultValues: {
      priority: MessagePriority.Normal,
    },
  });

  const onSubmit = async (data: CreateMessageFormData) => {
    try {
      // 转换日期时间格式
      const submitData = {
        ...data,
        scheduledSendTime: data.scheduledSendTime
          ? dayjs(data.scheduledSendTime).toISOString()
          : undefined,
        expirationTime: data.expirationTime
          ? dayjs(data.expirationTime).toISOString()
          : undefined,
      };

      await messageApi.create(submitData);
      message.success('消息创建成功');
      reset();
      onSuccess?.();
    } catch (error) {
      console.error('创建消息失败:', error);
    }
  };

  return (
    <Form
      layout="vertical"
      onFinish={handleSubmit(onSubmit)}
      style={{ maxWidth: 800 }}
    >
      <Form.Item
        label="消息标题"
        validateStatus={errors.title ? 'error' : ''}
        help={errors.title?.message}
      >
        <Input
          {...control.register('title')}
          placeholder="请输入消息标题"
        />
      </Form.Item>

      <Form.Item
        label="消息内容"
        validateStatus={errors.content ? 'error' : ''}
        help={errors.content?.message}
      >
        <Input.TextArea
          {...control.register('content')}
          placeholder="请输入消息内容"
          rows={4}
        />
      </Form.Item>

      <Form.Item
        label="消息类型"
        validateStatus={errors.messageType ? 'error' : ''}
        help={errors.messageType?.message}
      >
        <Select
          {...control.register('messageType')}
          placeholder="请选择消息类型"
          options={Object.entries(MessageType)
            .filter(([_, value]) => typeof value === 'number')
            .map(([key, value]) => ({
              label: key,
              value: value as number,
            }))}
        />
      </Form.Item>

      <Form.Item
        label="消息渠道"
        validateStatus={errors.channel ? 'error' : ''}
        help={errors.channel?.message}
      >
        <Select
          {...control.register('channel')}
          placeholder="请选择消息渠道"
          options={Object.entries(MessageChannel)
            .filter(([_, value]) => typeof value === 'number')
            .map(([key, value]) => ({
              label: key,
              value: value as number,
            }))}
        />
      </Form.Item>

      <Form.Item
        label="计划发送时间"
        validateStatus={errors.scheduledSendTime ? 'error' : ''}
        help={errors.scheduledSendTime?.message}
      >
        <DatePicker
          showTime
          format="YYYY-MM-DD HH:mm:ss"
          onChange={(date) => {
            control._formValues.scheduledSendTime = date?.toISOString();
          }}
        />
      </Form.Item>

      <Form.Item>
        <Space>
          <Button type="primary" htmlType="submit" loading={isSubmitting}>
            创建消息
          </Button>
          <Button onClick={() => reset()}>重置</Button>
        </Space>
      </Form.Item>
    </Form>
  );
}
```

#### 使用 React 19 Actions（推荐）

React 19 提供了 Actions API，可以更好地处理表单提交：

```typescript
import { useActionState } from 'react';
import { useForm } from 'react-hook-form';
import { zodResolver } from '@hookform/resolvers/zod';

export function CreateMessageFormWithAction() {
  const [state, formAction, isPending] = useActionState(
    async (prevState: any, formData: FormData) => {
      try {
        const data = {
          title: formData.get('title') as string,
          content: formData.get('content') as string,
          // ... 其他字段
        };

        await messageApi.create(data);
        return { success: true, message: '消息创建成功' };
      } catch (error) {
        return { success: false, message: '创建失败' };
      }
    },
    null
  );

  const { control, handleSubmit } = useForm({
    resolver: zodResolver(createMessageSchema),
  });

  return (
    <form action={formAction}>
      {/* 表单字段 */}
      <button type="submit" disabled={isPending}>
        {isPending ? '提交中...' : '创建消息'}
      </button>
      {state?.message && (
        <div>{state.message}</div>
      )}
    </form>
  );
}
```

---

## Ant Design Pro Components 使用

### ProTable - 高级表格组件

ProTable 提供了开箱即用的表格功能，包括搜索、分页、排序等。

创建 `src/components/message/MessageTable.tsx`：

```typescript
import { ProTable } from '@ant-design/pro-components';
import type { ProColumns } from '@ant-design/pro-components';
import { messageApi } from '@/api/message';
import type { MessageDto, MessageQueryDto } from '@/types/message';
import { MessageType, MessageStatus, MessageChannel } from '@/types/message';
import { Tag, Button, Space } from 'antd';
import { formatDateTime } from '@/utils/response';

const columns: ProColumns<MessageDto>[] = [
  {
    title: '标题',
    dataIndex: 'title',
    ellipsis: true,
    width: 200,
  },
  {
    title: '类型',
    dataIndex: 'messageType',
    valueType: 'select',
    valueEnum: Object.entries(MessageType)
      .filter(([_, value]) => typeof value === 'number')
      .reduce((acc, [key, value]) => {
        acc[value] = { text: key };
        return acc;
      }, {} as Record<number, { text: string }>),
    width: 120,
  },
  {
    title: '状态',
    dataIndex: 'status',
    valueType: 'select',
    valueEnum: Object.entries(MessageStatus)
      .filter(([_, value]) => typeof value === 'number')
      .reduce((acc, [key, value]) => {
        acc[value] = { text: key };
        return acc;
      }, {} as Record<number, { text: string }>),
    render: (_, record) => (
      <Tag color={record.isRead ? 'green' : 'red'}>
        {record.isRead ? '已读' : '未读'}
      </Tag>
    ),
    width: 100,
  },
  {
    title: '创建时间',
    dataIndex: 'creationTime',
    valueType: 'dateTime',
    sorter: true,
    width: 180,
    render: (_, record) => formatDateTime(record.creationTime),
  },
  {
    title: '操作',
    valueType: 'option',
    width: 200,
    render: (_, record) => (
      <Space>
        <Button type="link" size="small">
          查看
        </Button>
        {!record.isRead && (
          <Button type="link" size="small">
            标记已读
          </Button>
        )}
        <Button type="link" danger size="small">
          删除
        </Button>
      </Space>
    ),
  },
];

export function MessageTable() {
  return (
    <ProTable<MessageDto, MessageQueryDto>
      columns={columns}
      request={async (params, sort) => {
        const queryParams: MessageQueryDto = {
          ...params,
          skipCount: ((params.current || 1) - 1) * (params.pageSize || 20),
          maxResultCount: params.pageSize || 20,
          sorting: sort
            ? `${Object.keys(sort)[0]} ${Object.values(sort)[0] === 'ascend' ? 'asc' : 'desc'}`
            : undefined,
        };

        const result = await messageApi.getList(queryParams);
        return {
          data: result.items,
          success: true,
          total: result.totalCount,
        };
      }}
      rowKey="id"
      search={{
        labelWidth: 'auto',
      }}
      pagination={{
        pageSize: 20,
        showSizeChanger: true,
      }}
      dateFormatter="string"
      headerTitle="消息列表"
      toolBarRender={() => [
        <Button key="button" type="primary">
          新建消息
        </Button>,
      ]}
    />
  );
}
```

### ProForm - 高级表单组件

ProForm 提供了更强大的表单功能：

```typescript
import { ProForm, ProFormText, ProFormSelect, ProFormDateTimePicker } from '@ant-design/pro-components';
import { messageApi } from '@/api/message';
import type { CreateMessageDto } from '@/types/message';

export function CreateMessageProForm() {
  return (
    <ProForm<CreateMessageDto>
      onFinish={async (values) => {
        await messageApi.create(values);
        return true;
      }}
      layout="vertical"
    >
      <ProFormText
        name="title"
        label="消息标题"
        rules={[{ required: true, message: '请输入消息标题' }]}
        placeholder="请输入消息标题"
      />

      <ProFormText
        name="content"
        label="消息内容"
        rules={[{ required: true, message: '请输入消息内容' }]}
        fieldProps={{
          rows: 4,
        }}
      />

      <ProFormSelect
        name="messageType"
        label="消息类型"
        options={[
          { label: '通知', value: MessageType.Notification },
          { label: '工作流', value: MessageType.Workflow },
          // ... 其他选项
        ]}
        rules={[{ required: true, message: '请选择消息类型' }]}
      />

      <ProFormDateTimePicker
        name="scheduledSendTime"
        label="计划发送时间"
      />
    </ProForm>
  );
}
```

---

## React Router 7 路由配置

### 使用 Data Router（推荐）

React Router 7 推荐使用 Data Router，支持数据加载和错误边界。

创建 `src/routes/routes.tsx`：

```typescript
import { createBrowserRouter, Navigate } from 'react-router-dom';
import { lazy } from 'react';
import type { RouteObject } from 'react-router-dom';

// 懒加载页面组件
const MessageList = lazy(() => import('@/pages/MessageList'));
const MessageDetail = lazy(() => import('@/pages/MessageDetail'));
const TemplateManagement = lazy(() => import('@/pages/TemplateManagement'));

// 布局组件
const Layout = lazy(() => import('@/layouts/MainLayout'));

export const routes: RouteObject[] = [
  {
    path: '/',
    element: <Layout />,
    children: [
      {
        index: true,
        element: <Navigate to="/messages" replace />,
      },
      {
        path: 'messages',
        element: <MessageList />,
        // 可以添加 loader 和 action
        // loader: async () => {
        //   const messages = await messageApi.getList({});
        //   return { messages };
        // },
      },
      {
        path: 'messages/:id',
        element: <MessageDetail />,
        // loader: async ({ params }) => {
        //   const message = await messageApi.getById(params.id!);
        //   return { message };
        // },
      },
      {
        path: 'templates',
        element: <TemplateManagement />,
      },
    ],
  },
];

export const router = createBrowserRouter(routes);
```

### 在 App 中使用路由

```typescript
import { RouterProvider } from 'react-router-dom';
import { router } from './routes/routes';
import { Suspense } from 'react';
import { Spin } from 'antd';

function App() {
  return (
    <Suspense fallback={<Spin size="large" style={{ display: 'block', margin: '50% auto' }} />}>
      <RouterProvider router={router} />
    </Suspense>
  );
}

export default App;
```

### 使用 useLoaderData 获取数据

```typescript
import { useLoaderData } from 'react-router-dom';
import type { LoaderFunctionArgs } from 'react-router-dom';
import { messageApi } from '@/api/message';
import type { MessageDto } from '@/types/message';

// 在路由配置中定义 loader
export async function messageLoader({ params }: LoaderFunctionArgs) {
  const message = await messageApi.getById(params.id!);
  return { message };
}

// 在组件中使用
export function MessageDetail() {
  const { message } = useLoaderData() as { message: MessageDto };

  return (
    <div>
      <h1>{message.title}</h1>
      <p>{message.content}</p>
    </div>
  );
}
```

---

## 组件设计模式

### 容器组件和展示组件分离

#### 展示组件：MessageList.tsx

```typescript
import React from 'react';
import { List, Card, Tag, Button, Space } from 'antd';
import type { MessageDto } from '@/types/message';
import { formatDateTime } from '@/utils/response';

interface MessageListProps {
  messages: MessageDto[];
  loading?: boolean;
  onMarkAsRead: (id: string) => void;
  onDelete: (id: string) => void;
  onViewDetail: (message: MessageDto) => void;
}

export function MessageList({
  messages,
  loading,
  onMarkAsRead,
  onDelete,
  onViewDetail,
}: MessageListProps) {
  return (
    <List
      loading={loading}
      dataSource={messages}
      renderItem={(message) => (
        <List.Item>
          <Card
            style={{ width: '100%' }}
            title={
              <Space>
                <span>{message.title}</span>
                {!message.isRead && <Tag color="red">未读</Tag>}
                <Tag>{message.messageType}</Tag>
              </Space>
            }
            extra={
              <Space>
                <Button
                  type="link"
                  onClick={() => onViewDetail(message)}
                >
                  查看
                </Button>
                {!message.isRead && (
                  <Button
                    type="link"
                    onClick={() => onMarkAsRead(message.id)}
                  >
                    标记已读
                  </Button>
                )}
                <Button
                  type="link"
                  danger
                  onClick={() => onDelete(message.id)}
                >
                  删除
                </Button>
              </Space>
            }
          >
            <p>{message.summary || message.content}</p>
            <div style={{ fontSize: '12px', color: '#999' }}>
              {formatDateTime(message.creationTime)}
            </div>
          </Card>
        </List.Item>
      )}
    />
  );
}
```

#### 容器组件：MessageListContainer.tsx

```typescript
import React, { useEffect } from 'react';
import { useMessageStore } from '@/stores/messageStore';
import { MessageList } from '@/components/message/MessageList';
import { message } from 'antd';

export function MessageListContainer() {
  const {
    messages,
    loading,
    loadMessages,
    markAsRead,
    deleteMessage,
    setCurrentMessage,
  } = useMessageStore();

  useEffect(() => {
    loadMessages();
  }, [loadMessages]);

  const handleMarkAsRead = async (id: string) => {
    try {
      await markAsRead(id);
      message.success('已标记为已读');
    } catch (error) {
      // 错误已在 store 中处理
    }
  };

  const handleDelete = async (id: string) => {
    try {
      await deleteMessage(id);
      message.success('已删除');
    } catch (error) {
      // 错误已在 store 中处理
    }
  };

  const handleViewDetail = (msg: MessageDto) => {
    setCurrentMessage(msg);
    // 可以打开详情弹窗或跳转到详情页
  };

  return (
    <MessageList
      messages={messages}
      loading={loading}
      onMarkAsRead={handleMarkAsRead}
      onDelete={handleDelete}
      onViewDetail={handleViewDetail}
    />
  );
}
```

---

## 性能优化

### 1. 使用 React.memo 优化组件

```typescript
import React, { memo } from 'react';

export const MessageItem = memo(({ message, onMarkAsRead }: MessageItemProps) => {
  return (
    <div>
      {/* 消息内容 */}
    </div>
  );
}, (prevProps, nextProps) => {
  // 自定义比较函数
  return (
    prevProps.message.id === nextProps.message.id &&
    prevProps.message.isRead === nextProps.message.isRead
  );
});
```

### 2. 使用 useMemo 和 useCallback

```typescript
import { useMemo, useCallback } from 'react';

function MessageList() {
  const { messages, loading } = useMessageStore();

  // 使用 useMemo 缓存计算结果
  const unreadMessages = useMemo(
    () => messages.filter((msg) => !msg.isRead),
    [messages]
  );

  // 使用 useCallback 缓存函数
  const handleMarkAsRead = useCallback((id: string) => {
    // 处理逻辑
  }, []);

  return (
    // JSX
  );
}
```

### 3. 虚拟滚动（处理大量数据）

```typescript
import { FixedSizeList } from 'react-window';

function VirtualizedMessageList({ messages }: { messages: MessageDto[] }) {
  const Row = ({ index, style }: { index: number; style: React.CSSProperties }) => (
    <div style={style}>
      <MessageItem message={messages[index]} />
    </div>
  );

  return (
    <FixedSizeList
      height={600}
      itemCount={messages.length}
      itemSize={100}
      width="100%"
    >
      {Row}
    </FixedSizeList>
  );
}
```

### 4. 代码分割和懒加载

```typescript
import { lazy, Suspense } from 'react';

const MessageDetail = lazy(() => import('@/pages/MessageDetail'));

function App() {
  return (
    <Suspense fallback={<div>加载中...</div>}>
      <MessageDetail />
    </Suspense>
  );
}
```

---

## 测试策略

### 单元测试示例

创建 `src/components/message/__tests__/MessageList.test.tsx`：

```typescript
import { render, screen, fireEvent } from '@testing-library/react';
import { MessageList } from '../MessageList';
import type { MessageDto } from '@/types/message';

const mockMessages: MessageDto[] = [
  {
    id: '1',
    title: '测试消息1',
    content: '内容1',
    isRead: false,
    // ... 其他字段
  } as MessageDto,
];

describe('MessageList', () => {
  it('应该渲染消息列表', () => {
    render(
      <MessageList
        messages={mockMessages}
        onMarkAsRead={jest.fn()}
        onDelete={jest.fn()}
        onViewDetail={jest.fn()}
      />
    );

    expect(screen.getByText('测试消息1')).toBeInTheDocument();
  });

  it('应该调用 onMarkAsRead 当点击标记已读按钮', () => {
    const onMarkAsRead = jest.fn();
    render(
      <MessageList
        messages={mockMessages}
        onMarkAsRead={onMarkAsRead}
        onDelete={jest.fn()}
        onViewDetail={jest.fn()}
      />
    );

    fireEvent.click(screen.getByText('标记已读'));
    expect(onMarkAsRead).toHaveBeenCalledWith('1');
  });
});
```

---

## 代码规范

### 1. 命名规范

- **组件**: PascalCase (`MessageList.tsx`)
- **Hook**: camelCase with `use` prefix (`useMessages.ts`)
- **工具函数**: camelCase (`formatDateTime.ts`)
- **常量**: UPPER_SNAKE_CASE (`API_BASE_URL`)
- **类型/接口**: PascalCase (`MessageDto`)

### 2. 文件组织

```
components/
  message/
    MessageList.tsx
    MessageList.test.tsx
    MessageItem.tsx
    index.ts  // 导出
```

### 3. 导入顺序

```typescript
// 1. React 相关
import React, { useState, useEffect } from 'react';

// 2. 第三方库
import { Button, message } from 'antd';
import axios from 'axios';

// 3. 内部模块（按类型分组）
import { messageApi } from '@/api/message';
import { useMessageStore } from '@/stores/messageStore';
import { formatDateTime } from '@/utils/response';
import type { MessageDto } from '@/types/message';
```

---

## 部署和构建

### 构建配置

`vite.config.ts` 生产环境配置：

```typescript
import { defineConfig } from 'vite';
import react from '@vitejs/plugin-react';

export default defineConfig({
  plugins: [react()],
  build: {
    outDir: 'dist',
    sourcemap: false,
    minify: 'terser',
    terserOptions: {
      compress: {
        drop_console: true, // 移除 console
        drop_debugger: true,
      },
    },
    rollupOptions: {
      output: {
        manualChunks: {
          'react-vendor': ['react', 'react-dom', 'react-router-dom'],
          'antd-vendor': ['antd'],
          'signalr-vendor': ['@microsoft/signalr'],
        },
      },
    },
  },
});
```

### 环境变量检查

创建 `src/utils/envCheck.ts`：

```typescript
export function checkRequiredEnvVars() {
  const required = ['VITE_API_BASE_URL'];
  const missing: string[] = [];

  required.forEach((key) => {
    if (!import.meta.env[key]) {
      missing.push(key);
    }
  });

  if (missing.length > 0) {
    throw new Error(`缺少必需的环境变量: ${missing.join(', ')}`);
  }
}
```

在 `main.tsx` 中调用：

```typescript
import { checkRequiredEnvVars } from './utils/envCheck';

checkRequiredEnvVars();
```

---

## 常见问题解决

### 1. SignalR 连接失败

**问题**: SignalR 连接失败，返回 401 或 404

**解决方案**:
- 检查 JWT Token 是否有效
- 确认 Hub 路径正确 (`/hubs/messages`)
- 检查 CORS 配置
- 验证服务器是否运行

### 2. API 请求跨域问题

**问题**: CORS 错误

**解决方案**:
- 开发环境使用 Vite proxy
- 生产环境配置服务器 CORS
- 检查请求头是否正确

### 3. 状态更新不及时

**问题**: 实时消息未及时更新 UI

**解决方案**:
- 检查 SignalR 连接状态
- 确认事件监听器已正确注册
- 使用 React Query 的 `invalidateQueries` 刷新数据

### 4. 内存泄漏

**问题**: 长时间运行后内存占用增加

**解决方案**:
- 及时清理事件监听器
- 使用 `useEffect` 的清理函数
- 限制消息列表长度
- 使用虚拟滚动处理大量数据

---

## 总结

本指南涵盖了使用 Cursor AI 创建 MessageCenter 前端应用的完整最佳实践：

1. **项目结构**: 清晰的组织和模块化设计
2. **API 集成**: 类型安全的 HTTP 客户端封装，与后端 DTO 完全对应
3. **实时通信**: SignalR 连接管理和事件处理，支持点对点、组播和广播
4. **状态管理**: Zustand 和 React Query 的合理使用
5. **表单处理**: React Hook Form + Zod 实现高性能、类型安全的表单
6. **企业级组件**: Ant Design Pro Components 提升开发效率
7. **路由管理**: React Router 7 Data Router 实现数据驱动的路由
8. **React 19 特性**: 使用 Actions、useOptimistic 等新特性提升用户体验
9. **性能优化**: 组件优化和代码分割
10. **测试策略**: 单元测试和集成测试
11. **部署构建**: 生产环境优化配置

### 技术栈总览

本文档基于以下技术栈编写，所有示例代码和最佳实践都针对这些版本进行了优化：

**核心框架**:
- **React 19.2+** - 最新版本的 React，支持 Actions、useOptimistic 等新特性
- **TypeScript 5.x** - 类型安全
- **Vite 6+** - 极速构建工具

**路由管理**:
- **React Router 7.9+** - 支持 Data Router、Server Components

**UI 组件库**:
- **Ant Design 5.29+** - 企业级 UI 组件库
- **@ant-design/pro-components** - 企业级高级组件（ProTable、ProForm、ProLayout 等）

**表单处理**:
- **React Hook Form 7.54+** - 高性能表单处理库
- **Zod 3.24+** - 类型安全的 Schema 验证

**HTTP 客户端**:
- **Axios 1.7+** - HTTP 请求库

**实时通信**:
- **@microsoft/signalr 8.0+** - SignalR 客户端

**状态管理**:
- **Zustand 5.0+** - 轻量级状态管理
- **@tanstack/react-query 5.62+** - 服务端状态管理

**工具库**:
- **Day.js 1.11+** - 日期处理
- **lodash-es 4.17+** - 工具函数

### 与后端 API 的一致性

本文档中的所有类型定义、API 端点和数据格式都与后端实现保持一致：

- ✅ **DTO 类型**: 与 `MessageCenter.Application.Contracts.DTOs` 完全对应
- ✅ **枚举类型**: 与 `MessageCenter.Domain.Shared.Enums` 完全对应
- ✅ **API 端点**: 与 `MessageCenter.HttpApi.Controllers` 完全对应
- ✅ **SignalR Hub**: 与 `MessageCenter.Integration.Hubs.MessageHub` 完全对应
- ✅ **实时服务**: 与 `MessageCenter.Integration.Services.MessageRealtimeService` 完全对应

### 类型转换说明

由于 TypeScript/JavaScript 与 C# 的类型差异，前端需要进行以下转换：

- **Guid ↔ string**: 所有 ID 字段在前后端之间以字符串形式传输
- **DateTime ↔ string**: 日期时间以 ISO 8601 格式字符串传输
- **Dictionary<枚举, long> ↔ Record<string, number>**: 统计字典的键为枚举值的字符串形式
- **List<Guid> ↔ string[]**: 批量操作的 ID 列表以字符串数组传输

### 关键 API 端点

所有 API 端点都与后端实现一致：

**消息管理**:
- `POST /api/messages` - 创建消息
- `POST /api/messages/batch` - 批量创建（最多 1000 条）
- `GET /api/messages/{id}` - 获取消息
- `GET /api/messages` - 查询消息列表
- `GET /api/messages/receiver/{receiverId}` - 获取接收者消息
- `PUT /api/messages/{id}/read` - 标记已读
- `PUT /api/messages/read/batch` - 批量标记已读
- `PUT /api/messages/read/all/{receiverId}` - 标记全部已读
- `DELETE /api/messages/{id}` - 删除消息
- `DELETE /api/messages/batch` - 批量删除
- `GET /api/messages/unread-count/{receiverId}` - 获取未读数量
- `GET /api/messages/statistics` - 获取统计信息
- `POST /api/messages/{id}/retry` - 重试发送
- `POST /api/messages/{id}/cancel` - 取消发送
- `GET /api/messages/realtime/info` - 获取 SignalR 连接信息

**模板管理**:
- `POST /api/message-templates` - 创建模板
- `PUT /api/message-templates/{id}` - 更新模板
- `GET /api/message-templates/{id}` - 获取模板
- `GET /api/message-templates/code/{code}` - 根据代码获取模板
- `GET /api/message-templates` - 获取模板列表
- `DELETE /api/message-templates/{id}` - 删除模板
- `PUT /api/message-templates/{id}/enabled` - 启用/禁用模板

---

## 相关文档

- [Message Controller API 文档](./API/MessageController-API-Documentation.md)
- [Message Template Controller API 文档](./API/MessageTemplateController-API-Documentation.md)
- [实时通信指南](./API/Realtime-Communication-Guide.md)
- [前端最佳实践总览](./Frontend-Best-Practices-Overview.md)

---

## 支持

如有问题或建议，请参考：
- 项目 README
- API 文档
- 实时通信指南
- 提交 Issue 或联系开发团队
