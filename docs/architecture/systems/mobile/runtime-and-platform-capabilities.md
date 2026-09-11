# Mobile Runtime 与平台能力

> 状态：预实现架构。本文定义已批准的目标契约，不表示当前仓库已经包含或发布 `AtomUI.Mobile.Controls`。

本文定义每个 `TopLevel` 的 Mobile Runtime scope、`MobileViewportContext` 和平台 capability contract。Navigation、Overlay、
Gesture 和具体 Control 只能消费这里发布的状态，不能各自订阅或解释平台事件。

## Runtime Scope

每个活动 `TopLevel` 恰好拥有一个 Mobile Runtime scope。scope 创建并连接：

- `MobileViewportContext`。
- `MobileOverlayManager`。
- `MobileGestureCoordinator`。
- 当前 Host 提供的平台 capability adapter。
- TopLevel detach 时的统一释放回调。

scope 以 `TopLevel` 为 owner，不使用进程级可变 singleton。重复注册 adapter、缺少必需 adapter 或 owner 已 detach 必须在
启动或 acquire 阶段给出明确失败，不能等到首次手势或 Overlay 才产生空引用行为。

## Viewport 状态模型

`MobileViewportContext` 聚合：

| 状态 | 语义 |
| --- | --- |
| Safe Area insets | 内容需要避让的设备切口、圆角和系统保留区域 |
| System bar insets | 状态栏、导航栏或等价系统 UI 占用 |
| Input pane occlusion | 当前输入面板遮挡的 TopLevel 区域 |
| Available content bounds | 合并系统区域和输入遮挡后可供内容使用的区域 |
| Orientation | 当前显示方向和方向变化序列 |
| Display scale | Avalonia 与平台坐标换算所需的有效 scale |
| Font scale | 系统文本缩放 profile |
| Reduce Motion | 平台减弱动效设置 |
| App/host state | 前台、后台、恢复和 host detach 状态 |

这些值是运行时状态，不进入 Global Token、Alias Token 或 Control Own Token。

## 更新时序

1. Platform adapter 接收原生或 Avalonia 平台事件。
2. Runtime scope 在 UI thread 合并同一事件批次的 viewport 输入。
3. Context 发布一个一致 snapshot，避免 Control 观察到一半更新的 insets 和 bounds。
4. 新 snapshot 必须在下一次有效布局前可见。
5. SafeArea、Popup、Picker、NumberKeyboard、导航和滚动控件读取同一 snapshot。

方向变化、键盘显示/隐藏和前后台恢复不能由 Control 使用 timer、轮询或延迟刷新自行修补。恢复时先刷新 viewport、insets
和 input pane，再恢复可见 Overlay 和页面交互。

## Capability Contract

平台 adapter 至少提供：

- Safe Area、system bar 和可用 viewport 来源。
- Input pane/IME occlusion 与变化通知。
- 系统返回、返回手势和导航手势入口。
- Haptic 可用性与受限调用入口。
- 平台最小触控目标和 interaction profile。
- 前台、后台、host detach 与 host recreation 通知。
- Reduce Motion、字体缩放和无障碍环境信息。

iOS、Android 和 Headless 实现同一内部契约。显式公共 Control API 在两个移动平台保持同义；差异只进入 adapter 或
platform profile，不进入公共属性名和事件名。

## 能力缺失与退化

| 情况 | 退化规则 |
| --- | --- |
| Haptic 不可用 | 跳过触觉反馈，不改变 Control 逻辑终态 |
| Safe Area 未报告 | 使用零 inset，并保留可诊断的 capability 状态 |
| Input pane bounds 不可用 | 保留最近一致 viewport，不伪造键盘高度 |
| 系统返回入口不可用 | 应用命令和显式导航 API 继续使用统一事务 |
| Reduce Motion 开启 | 移除非必要位移动画，保持状态提交和焦点结果 |
| Host detach | 终止所有 scope-owned session、pointer chain 和平台订阅 |

能力缺失不能通过读取 OS 名称猜测默认值。平台 profile 可以提供已经解析的触控尺寸等策略值，但 Control 不判断 iOS 或
Android。

## 生命周期与释放

- Runtime scope acquire 与 TopLevel attach 配对。
- Platform event subscription 与 adapter detach/dispose 配对。
- Pending viewport update 与 host detach cancellation 配对。
- Overlay session、pointer capture 和 animation owner 在 detach 时统一结束。
- Context 不通过 static event、timer 或 cache 持有 TopLevel。
- Host recreation 创建新 scope；旧 scope 不迁移订阅，只迁移经过明确定义的可恢复状态。

## 维护不变量

1. 每个 `TopLevel` 只有一个 `MobileViewportContext`。
2. 所有 viewport consumer 读取同一 snapshot。
3. UI thread 上发布一致状态，不暴露半更新值。
4. Control 不读取 OS 名称或直接订阅原生事件。
5. detach 后不保留 TopLevel、Control、session 或 adapter 引用。
6. Headless 行为不被描述为平台体验验证。

验证层级见 [Mobile 验证架构](verification.md)。
