# TheRoadOfGroping
Net的摸索之路，写着玩，有很多不足
## 🎃.NET 8 后端框架
### 👕SunBlog.AspNetCore/src 后端自己搭建的框架

- 🛺.Net8
- EF Core 8 适配SqlServer和Mysql🎏
  - 仓储
  - 简单工作单元
- 模块化处理(参考ABP的简易版实现)😇
- 简单EventBus 基于Channels
- Autofac依赖注入，AOP注册处理
    - 横向处理`Application`
- AutoMapper模块化注入 (后续可能使用 `Mapster` 对象映射 )🍔
- ⛑️Log4Net日志
    - 控制台日志
    - 文件日志插入
    - 数据库日志写入
- 👒简单Minio存储桶
- 🎪缓存
    - Redis缓存
    - 本地缓存MemoryCache
- 动态api
- HangFire 后台任务简易集成🎢
- SignalR实时通信🚋
    - 前端的实时通知推送
    - Redis 无序列表缓存
- RabbitMQ封装事件发布订阅(较为简单，待完善)🪇
- 动态授权管理(基础功能已实现)