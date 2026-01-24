# 前端开发最佳实践指南 - 总览

## 概述

本文档为使用 Cursor AI 创建 MessageCenter 前端应用提供全面的最佳实践指南。所有内容已合并到单一文档中，方便查阅和参考。

## 完整文档

### [前端开发最佳实践指南（完整版）](./Frontend-Best-Practices.md)

**文档包含以下部分**：

1. **第一部分：项目结构和环境配置**
   - 技术栈推荐（React/Vue、状态管理、HTTP客户端等）
   - 项目目录结构
   - 环境变量配置
   - TypeScript 和 Vite 配置
   - 依赖管理
   - 开发工具配置（ESLint、Prettier）

2. **第二部分：API 集成和 HTTP 客户端**
   - TypeScript 类型定义（与后端 DTO 完全对应）
   - Axios HTTP 客户端配置
   - API 封装（消息 API、模板 API）
   - 错误处理机制
   - 请求/响应拦截器
   - Token 刷新逻辑

3. **第三部分：SignalR 实时通信**
   - SignalR 服务封装
   - React Hook 封装（useRealtime）
   - 连接管理（连接、断开、重连）
   - 事件处理（接收消息、状态变更）
   - 状态管理集成
   - 性能优化（消息去重、节流）

4. **第四部分：状态管理和最佳实践**
   - Zustand 状态管理
   - React Query 服务端状态管理
   - 组件设计模式
   - 性能优化策略
   - 测试策略
   - 代码规范
   - 部署和构建配置
   - 常见问题解决

---

### [第四部分：状态管理和最佳实践](./Frontend-Best-Practices-Part4.md)

**内容**：
- Zustand 状态管理
- React Query 服务端状态管理
- 组件设计模式（容器/展示组件分离）
- 性能优化（React.memo、useMemo、虚拟滚动）
- 测试策略
- 代码规范
- 部署和构建配置
- 常见问题解决

**适合场景**：状态管理、性能优化、测试和部署

---

## 快速开始

### 1. 项目初始化

参考 [第一部分](./Frontend-Best-Practices-Part1.md) 完成：
- 选择技术栈
- 配置项目结构
- 安装依赖
- 配置开发环境

### 2. API 集成

参考 [第二部分](./Frontend-Best-Practices-Part2.md) 完成：
- 定义类型
- 配置 HTTP 客户端
- 封装 API 方法
- 实现错误处理

### 3. 实时通信

参考 [第三部分](./Frontend-Best-Practices-Part3.md) 完成：
- 集成 SignalR
- 实现连接管理
- 处理实时事件

### 4. 状态管理和优化

参考 [第四部分](./Frontend-Best-Practices-Part4.md) 完成：
- 配置状态管理
- 优化组件性能
- 编写测试
- 准备部署

---

## 技术栈总览

### 核心框架
- **React 18** + **TypeScript 5**
- **Vite** 构建工具

### 状态管理
- **Zustand** - 客户端状态
- **React Query** - 服务端状态

### HTTP 客户端
- **Axios** - HTTP 请求

### 实时通信
- **@microsoft/signalr** - SignalR 客户端

### UI 组件库
- **Ant Design** - 企业级 UI 组件

### 工具库
- **dayjs** - 日期处理
- **lodash-es** - 工具函数

---

## 项目结构总览

```
message-center-frontend/
├── src/
│   ├── api/              # API 封装
│   ├── services/         # 业务服务（SignalR等）
│   ├── hooks/            # 自定义 Hooks
│   ├── stores/           # 状态管理（Zustand）
│   ├── components/       # UI 组件
│   ├── pages/            # 页面组件
│   ├── types/            # TypeScript 类型
│   ├── utils/            # 工具函数
│   └── constants/        # 常量定义
```

---

## 核心功能实现

### 1. 消息列表

```typescript
// 使用 Zustand Store
const { messages, loadMessages, markAsRead } = useMessageStore();

// 使用 React Query
const { data, isLoading } = useMessages(queryParams);
```

### 2. 实时消息接收

```typescript
// 使用 Realtime Hook
const { isConnected, connect } = useRealtime();

// 订阅消息事件
useRealtimeMessage(
  (message) => console.log('收到消息', message),
  (notification) => console.log('未读数量', notification.unreadCount)
);
```

### 3. API 调用

```typescript
// 直接调用 API
const message = await messageApi.create(data);

// 使用 React Query Mutation
const { mutate } = useCreateMessage();
mutate(data);
```

---

## 相关 API 文档

- [Message Controller API 文档](./API/MessageController-API-Documentation.md)
- [Message Template Controller API 文档](./API/MessageTemplateController-API-Documentation.md)
- [实时通信指南](./API/Realtime-Communication-Guide.md)

---

## 最佳实践总结

### 1. 类型安全
- 使用 TypeScript 定义所有类型
- 类型与后端 DTO 保持一致
- 避免使用 `any`

### 2. 错误处理
- 统一错误处理机制
- 用户友好的错误提示
- 开发环境显示详细错误

### 3. 性能优化
- 使用 React.memo 优化组件
- 虚拟滚动处理大量数据
- 代码分割和懒加载
- 合理使用缓存

### 4. 代码质量
- 遵循命名规范
- 组件职责单一
- 容器/展示组件分离
- 编写单元测试

### 5. 实时通信
- 自动重连机制
- 连接状态指示
- 消息去重
- 错误处理和日志

---

## 常见问题

### Q: 如何开始一个新项目？

A: 按照以下顺序：
1. 阅读第一部分，完成项目初始化
2. 阅读第二部分，集成 API
3. 阅读第三部分，添加实时通信
4. 阅读第四部分，优化和部署

### Q: SignalR 连接失败怎么办？

A: 检查：
1. JWT Token 是否有效
2. Hub 路径是否正确
3. CORS 配置
4. 网络连接

参考 [第三部分 - 常见问题](./Frontend-Best-Practices-Part3.md)

### Q: 如何优化性能？

A: 参考 [第四部分 - 性能优化](./Frontend-Best-Practices-Part4.md)：
- 使用 React.memo
- 虚拟滚动
- 代码分割
- 合理使用缓存

---

## 更新日志

- **2024-01-01**: 初始版本发布
  - 完成四个部分的文档
  - 包含完整的代码示例
  - 涵盖最佳实践和常见问题

---

## 支持

如有问题或建议：
1. 查看相关部分的详细文档
2. 参考 API 文档
3. 提交 Issue 或联系开发团队

---

## 下一步

1. 阅读 [第一部分：项目结构和环境配置](./Frontend-Best-Practices-Part1.md)
2. 按照文档逐步搭建项目
3. 参考代码示例实现功能
4. 遵循最佳实践优化代码

祝开发顺利！🚀
