# AtomUI.Mobile.Controls 平台 Adapter

> 状态：预实现架构。本文定义已批准的目标契约，不表示当前仓库已经包含或发布 `AtomUI.Mobile.Controls`。

Mobile 使用一个内部 capability contract，由 Headless、iOS 和 Android adapter 分别实现。Control 和 Runtime scope 只消费
统一 snapshot、事件与 hook，不读取 OS 名称，也不在具体 Control 中散布条件编译。

## Capability Contract

| 能力 | 统一输出/入口 | Owner |
| --- | --- | --- |
| Safe Area 与 system bars | Insets、available bounds 和变化通知 | Adapter 读取平台；`MobileViewportContext` 合并状态 |
| Input pane / IME | Occlusion、可用性和变化通知 | Adapter 投递；Viewport/输入 Control 协调消费 |
| Back 与 navigation gesture | 单次可仲裁的系统输入 | Adapter 投递；Overlay/Navigation 决定 owner |
| Haptic | 可用性与受约束调用入口 | Adapter 执行；Control policy 决定是否请求 |
| Touch profile | 已解析的最小触控目标和 interaction profile | Adapter/profile 提供；Theme/Control 消费值 |
| Foreground/background | Host 状态、恢复和 detach/recreation 通知 | Host/adapter 投递；Runtime scope 统一清理或恢复 |
| Reduce Motion 与 font scale | 一致 snapshot 字段 | Adapter 投递；Viewport 发布 |
| Accessibility environment | 平台环境和能力状态 | Adapter 投递；Control 仍拥有 AutomationPeer 语义 |

平台 adapter 提供事实与 hook，不决定 Control 视觉、Token、导航结果、Overlay 关闭原因或默认产品行为。

## 实现角色

| Adapter | 目标用途 | 不能证明 |
| --- | --- | --- |
| Headless | 注入确定 snapshot、模拟能力缺失、验证状态机和 acquire/release | iOS/Android 触摸、IME、系统返回或无障碍体验 |
| iOS | 投射 iOS Host 生命周期、viewport、输入、返回手势、haptic 和 accessibility 环境 | Android 行为或发布状态 |
| Android | 投射 Android Host 生命周期、system back、viewport、输入、haptic 和 accessibility 环境 | iOS 行为或发布状态 |

iOS 是首个实现平台，但三个 adapter 从 Foundation 使用相同 contract。iOS 适配不能把平台 SDK 类型、pt 特例或 Host 生命周期
泄漏到 Public Control API；Android 适配不能在后续通过修改公共契约追补基本能力。

## Owner 生命周期

1. Host 在 AtomUI 组合根提供 adapter。
2. 每个 `TopLevel` attach 时，Mobile Runtime scope acquire 与 adapter 建立 owner-scoped subscription。
3. Adapter 在 UI thread 边界投递原始变化；Runtime 合并为一致 snapshot。
4. `TopLevel` detach、Host recreation 或应用终止时取消 pending work，并释放 subscription、hook 和 owner 引用。
5. 新 Host/TopLevel 创建新的 scope；旧 adapter 不通过 static cache 保留 Visual、Control 或 session。

如果 native hook 需要跨线程回调，adapter 必须在进入 Runtime 前完成线程和 lifetime 归一化，不能让 Control 直接处理原生回调。

## 能力缺失

| 缺失能力 | 退化行为 |
| --- | --- |
| Safe Area/system bar 来源 | 使用零 inset 并保留可诊断状态 |
| Input pane bounds | 保持最近一致 viewport，不猜测键盘高度 |
| Haptic | 跳过反馈，不改变逻辑终态和事件顺序 |
| 系统返回/gesture | 显式导航命令继续使用同一事务 |
| Reduce Motion | 移除非必要动效，保持提交、焦点和 cleanup |
| Host detach | 以统一原因终止 session、pointer chain 和订阅 |

必需 adapter 缺失或重复属于启动/acquire 配置错误，不使用静默 fallback。可选 capability 缺失则按上表退化，Control 不通过
OS 名称猜测值。

## Native 边界

Adapter 优先使用 Avalonia 公共 API。只有能力涉及 P/Invoke、native handle、原生结构体、协议对象或可释放 hook，且 Avalonia
公共 API 无法表达时，才调用 `AtomUI.Native`。Native 负责参数/handle 校验、底层调用、错误转换和确定释放；Mobile adapter
负责 owner、订阅、状态归一化和产品侧调用时机。

## 验证

- Contract/Headless：snapshot 合并、能力缺失、重复 adapter、detach、取消和无残留引用。
- iOS：Simulator 与真机分别验证 Safe Area、IME、返回手势、旋转、前后台、VoiceOver 和 haptic 降级。
- Android：Emulator 与真机分别验证 system bars、IME、system back、multi-window、Activity recreation、TalkBack 和 haptic 降级。
- Release：两个平台分别验证 trim/AOT 后 adapter、Theme、Localization 和 DataTemplate 可发现，产物可安装启动。

测试层级与证据要求见 [Mobile 验证架构](../../architecture/systems/mobile/verification.md)。当前没有 adapter 源码或平台通过证据。
