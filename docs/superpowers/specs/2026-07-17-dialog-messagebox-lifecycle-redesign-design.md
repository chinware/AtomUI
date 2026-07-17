# Dialog 与 MessageBox 生命周期重构设计

## 1. 目标

从源头重构 Dialog 的状态、承载和资源生命周期，完整解决取消残留、快速开关竞态、重复关闭、嵌套 Dialog、焦点恢复、mask 与 surface 分裂、事件异常、内容更新、按钮集合和资源释放问题。

本次重构不保留旧 API、类型层级、文件布局、同步阻塞实现或历史偶然事件时序。需要保留的是现有产品能力、视觉结果和经过重新定义的正确用户行为。

MessageBox 必须随 Dialog 一起迁移到相同生命周期内核。它不再内部创建并同步另一个 Dialog，而是成为 Dialog 的语义特化。

设计原则：

- 每个运行时状态和资源只能有一个明确 owner。
- 所有关闭入口必须汇入同一状态机和 teardown 路径。
- 新抽象必须直接消除已确认的竞态、重复职责或资源所有权问题。
- 复用 AtomUI 已有 OverlayLayer、MotionScene、主题、Token 和 Avalonia 绑定能力。
- 不建设通用 Portal、窗口框架、Presenter 插件系统、异步队列框架或新的 Action/Motion 框架。

## 2. 当前问题与根因

### 2.1 PR #386 的结论

PR #386 的视频证明取消后可见 Dialog Host 消失，但不能证明 Dialog 会话已经结束。

当前取消路径直接调用 `dialogHost.Close()`，没有经过正常的 `CommitClose` 和 `CompleteClose`：

- `_openState` 仍可能保留。
- `IsOpen` 仍可能为 `true`。
- owner、placement target、binding 和 Host 订阅仍可能存活。
- modal completion task 可能没有完成。
- 同一个 Dialog 后续可能无法重新打开。

因此该 PR 修复了视频中的表面残留，没有修复生命周期根因。

### 2.2 多状态 owner

当前以下对象和字段共同决定 Dialog 是否打开或关闭：

- `Dialog.IsOpen`
- `_openState`
- `_opening`
- `_closing`
- `_ignoreIsOpenChanged`
- `DispatcherFrame`
- Window/Overlay Host 的内部打开和关闭状态

这些状态之间没有原子提交协议，导致：

- `true -> false` 时关闭可能在 Host 创建前完成，排队的打开随后仍然执行。
- `true -> false -> true` 时最后一次打开可能在关闭期间被静默丢弃。
- 关闭动画期间可能出现 `IsOpen=true` 但没有活动 Host。
- 重复 Accept、Reject、Done 或 Host close request 可以重复发送结果事件。

### 2.3 Host 协议错误

`IDialogHost.Close(Action?)`：

- 无法等待关闭动画和资源释放。
- 无法表达失败或取消。
- 无法保证重复调用返回同一个关闭完成结果。
- Overlay Host 重复调用会覆盖保存的 callback。
- Dialog 必须通过具体类型判断 Window Host 才能调用 `ShowDialog(owner)`。

Host 同时负责绑定 Dialog、复制按钮、内容、尺寸、定位、动画、焦点和资源释放，职责过多且两个实现不一致。

### 2.4 Overlay 抽象错误

当前 Overlay Dialog 的 surface 位于 Popup，mask 位于独立 OverlayLayer。两者通过 sibling index 临时拼接：

- mask 和 surface 没有共同视觉 owner。
- `BringToFront` 只能可靠移动 Popup，不能原子移动 mask。
- 嵌套 Dialog 的 ESC、焦点和 mask 命中没有统一栈管理。
- opening/closing motion 分别驱动，并用固定 timer 猜测完成时间。
- Popup 关闭和 mask 移除存在多条 cleanup 路径。

Dialog 是 TopLevel 内的受控模态层，不需要 Popup 的锚点窗口和独立 popup lifetime。

### 2.5 内容、按钮与 MessageBox

- 用户 Content 被强制设置 `TemplatedParent=Dialog`，破坏正常模板和 logical tree 语义。
- Content 动态更新经过 `as Control`，数据对象和字符串路径不正确。
- CustomButtons 的 Replace、Move 和 Reset 路径不完整或抛 `NotSupportedException`。
- Dialog、两个 Host 和 ButtonBox 重复复制和同步按钮。
- `DialogButtonBox` 重复构造标准按钮，default/escape 的动态更新不完整。
- MessageBox 创建隐藏 Dialog，并通过大量 relay binding、事件转发和 suppression flag 维护第二套状态。

### 2.6 Ant Design 6 参考结论

本设计参考本地 `../ReferenceProjects/ant-design` 中的：

- `components/modal/Modal.tsx`
- `components/modal/confirm.tsx`
- `components/modal/useModal/HookModal.tsx`
- `components/_util/ActionButton.tsx`
- `components/modal/__tests__`

采纳的原则：

- 业务 `open` 意图、动画保活和最终资源销毁不是同一个状态。
- close request 与 after-close teardown 分离，关闭动画完成后才发送最终 Closed/afterClose。
- modal 容器统一管理 topmost、ESC、focus trap 和触发元素焦点恢复。
- 声明式 Modal、静态 confirm 和 hook 入口最终汇入同一展示内核。
- 异步 action 或 close policy 运行期间必须抑制重复提交。

不照搬：

- React Portal、Context、hook 和 DOM scroll-lock 实现。
- 为未来扩展预建 confirm registry、全局 destroyAll 或 config update 框架。
- ActionButton 的 Promise 自动 loading API；AtomUI 当前没有对应产品契约。

Avalonia 实现使用每作用域 DialogOverlayLayer、强类型 DialogSession、原生 Window modal 和 MotionScene completion。

## 3. 保留的产品能力

下列能力必须保留，调用 API 可以重新设计：

- XAML 中通过双向 `IsOpen` 打开和关闭。
- 通过代码创建并异步等待关闭结果。
- Overlay 和 Window 两种承载方式。
- modal 和 modeless。
- 浏览器或无原生 Window 平台回退到 Overlay。
- 标题、图标、正文、DataTemplate、loading 和 footer。
- 标准按钮、自定义按钮、default button、escape button 和按钮配置。
- Accept、Reject、Done 与任意结果对象。
- 同步 `Closing` 取消与异步 `BeforeCloseAsync`。
- mask、close button、ESC、owner close 和 placement target detach。
- Overlay 拖动、缩放、最大化、恢复和 motion anchor。
- Window 拖动、缩放、最大化、最小化、Topmost 和原生 modal。
- 打开和关闭 motion，以及关闭后焦点恢复。
- MessageBox 的 Confirm、Information、Success、Warning、Error 和 Normal 样式。
- MessageBox 的语义图标、OK/Cancel 文案、默认按钮和自定义按钮。
- 主题、Token、局部资源、语言资源和 NativeAOT。

不保留：

- 同步阻塞 `Open()` / `ShowMessageBox()`。
- 嵌套 `DispatcherFrame`。
- callback 风格静态 API。
- `OpenAsync` 在 modeless 下只等待显示、在 modal 下等待关闭的双重语义。
- 旧 Host 接口和具体 Host 类型。
- 历史偶然事件顺序、重复事件或半关闭状态。

## 4. 总体架构

```text
Dialog / MessageBox
        |
        v
DialogSession
        |
        v
IDialogPresenter
   |                         |
   v                         v
OverlayDialogPresenter   WindowDialogPresenter
   |                         |
   +----------+--------------+
              v
        DialogSurface

OverlayDialogPresenter -> DialogOverlayLayer -> ScopeAwareOverlayLayer
```

核心类型：

| 类型 | 职责 | 明确不负责 |
| --- | --- | --- |
| `Dialog` | 公开属性、事件、命令、`IsOpen` 最终意图、当前 Session 引用 | Host 状态、动画细节、资源订阅集合 |
| `DialogSession` | 单次展示状态机、关闭仲裁、结果、取消、订阅、焦点快照、完整 teardown | 具体 Overlay/Window 视觉实现 |
| `IDialogPresenter` | 可等待展示和关闭协议、Host close request | 业务 veto、结果和公开事件 |
| `OverlayDialogPresenter` | 一个 Overlay Dialog 的 mask、surface、布局、拖动缩放和 motion | 多 Dialog 栈策略 |
| `DialogOverlayLayer` | 当前作用域的 Dialog 顺序、topmost、ESC 和输入激活 | 单个 Dialog 内容和业务状态 |
| `WindowDialogPresenter` | 原生 Window shell、modal/modeless、Window 状态和关闭桥接 | Dialog 业务关闭策略 |
| `DialogSurface` | Overlay/Window 共享 header、body、loading、footer 和 button box | Session 状态和 Host lifetime |
| `MessageBox` | Dialog 的语义特化 | 创建或同步隐藏 Dialog |

不新增：

- `DialogOverlayEntry` 或 `DialogOverlayContainer`。
- 单独的 `DialogController`；跨 Session 的 `IsOpen` 协调留在 Dialog。
- `IDialogPresenterFactory`；DialogSession 根据 HostType 创建两个已知 Presenter。
- 公开 Session handle。
- 通用 Dialog Service、Portal、Action 或 Motion 抽象。

## 5. 状态所有权

### 5.1 Dialog 所有的状态

Dialog 只拥有：

- 用户最终希望的 `IsOpen` 值。
- 当前活动 `DialogSession?`。
- 用于重新协调最新 `IsOpen` 的 generation。
- 当前 Session 的 completion observation。

generation 只表示属性意图版本，不表示 Session 状态，不与 Session state 重复。

Dialog 不再拥有 `_opening`、`_closing`、`_ignoreIsOpenChanged`、Host、DispatcherFrame 或 Session 订阅。

### 5.2 DialogSession 所有的状态

每次打开创建一个新的 DialogSession。Session 状态：

```text
Created -> Opening -> Open -> ClosePending -> Closing -> Closed
                         ^           |
                         +--- veto --+
```

更准确地说，veto 从 `ClosePending` 返回 `Open`；进入 `Closing` 后不可逆。

Session 还唯一拥有：

- Presenter。
- 本次结果和 close reason。
- opening cancellation 与 close-policy cancellation。
- owner、placement target、Presenter 事件和 property binding 的 disposables。
- 打开前的 focused element。
- Session completion `TaskCompletionSource`。
- 第一个需要向调用方传播的异常。

Session 最终状态始终为 `Closed`。打开、事件或 Presenter 故障通过 completion task 表达，不增加 `Faulted` 状态，避免绕过 teardown。

## 6. IsOpen 协调

`IsOpen` 同时是声明式请求入口和实际打开状态投影，但它不是 Session 状态本身。

Dialog 的 reconcile 规则：

| 当前 Session | 最新 `IsOpen` | 行为 |
| --- | --- | --- |
| 无 | `true` | 创建并启动新 Session |
| 无 | `false` | 无操作 |
| Opening | `false` | 强制取消本次打开，不运行业务 veto |
| Open | `false` | 发起正常可取消关闭 |
| ClosePending | `true` | 取消尚未提交的 close policy，返回 Open |
| Closing | `true` | 记录最新意图，旧 Session Closed 后创建新 Session |
| 任意活动状态 | 相同值重复到达 | 幂等无操作或等待现有操作 |

规则：

- `IsOpen` 内部更新仍会进入同一个 reconcile，不使用 suppression flag。
- Session 提交关闭时先把 `IsOpen` 设置为 `false`；reconcile 发现 Session 已经 Closing，只确认目标状态。
- close veto 时把 `IsOpen` 恢复为 `true`；reconcile 发现 Session 已经 Open，只确认目标状态。
- 每次异步边界后检查 generation，旧请求不得覆盖新意图。
- 所有状态转换在 UI Dispatcher 上执行；异步 policy 返回 UI Dispatcher 后再提交。

## 7. 打开、关闭与事件时序

### 7.1 打开

```text
resolve placement target / TopLevel
-> create Session and Presenter
-> acquire bindings and owner subscriptions
-> attach Presenter and logical/resource owner
-> layout ready
-> opening motion complete
-> establish focus
-> State=Open
-> Opened
```

`Opened` 只在 Presenter 已可交互、布局和 opening motion 完成后触发。Window 和 Overlay 使用相同定义。

### 7.2 普通关闭

普通关闭来源：

- Accept、Reject、Done。
- 标准或自定义按钮。
- close caption button。
- ESC。
- mask click。
- `IsOpen=false`，前提是 Session 已 Open。

时序：

```text
State=ClosePending
-> Closing(reason, canCancel=true)
-> BeforeCloseAsync(context)
-> veto: State=Open, IsOpen=true
-> allow: atomically commit result/reason, State=Closing, IsOpen=false
-> Accepted / Rejected / Finished
-> Presenter.CloseAsync
-> remove visuals and content
-> release bindings/subscriptions/logical parent
-> restore focus
-> State=Closed
-> data-context closed notification
-> Closed
-> complete ShowAsync
```

`Accepted`、`Rejected` 和 `Finished` 表示结果已经提交，不表示视觉已经移除。`Closed` 表示 motion、视觉和资源 teardown 已全部完成。

### 7.3 强制 teardown

强制来源：

- `ShowAsync` cancellation token。
- owner Window 关闭。
- placement target detach 且不能继续承载。
- Presenter 意外关闭或不可恢复故障。
- 打开过程异常。

强制规则：

- 可以从 Created、Opening、Open 或 ClosePending 进入 Closing。
- 取消正在运行的 close policy。
- Session 已进入 Opening 后必须发送一次 `Closing(reason, canCancel=false)`，但忽略 Cancel；调用前 token 已取消、尚未创建 Session 时不发送生命周期事件。
- 不调用 `BeforeCloseAsync`。
- 不受 `IsConfirmLoading` 阻止。
- 仍执行同一 Presenter close、视觉移除、binding 释放、logical parent 解除和 `Closed` 路径。
- cancellation 必须在 teardown 后才让 `ShowAsync` 抛 `OperationCanceledException`。

### 7.4 异常

- `Closing` 或 `BeforeCloseAsync` 在提交前抛异常：本次普通关闭不提交，Session 返回 Open；异常被可靠报告。
- Accepted、Rejected、Finished、DataContext 通知或 Closed 在提交后抛异常：继续执行剩余 teardown 和通知，Session Closed 后 completion task 传播第一个异常。
- Presenter Show/Close 抛异常：Session 在 `finally` 中调用 Presenter Dispose、释放所有 owner 资源并进入 Closed。
- 不使用未观察的 fire-and-forget Task。由 Dialog 启动的声明式操作必须安装统一异常观察器并通过 Avalonia Dispatcher 报告。

## 8. Presenter 协议

内部协议保持最小：

```csharp
internal interface IDialogPresenter : IAsyncDisposable
{
    event EventHandler<DialogPresenterCloseRequestedEventArgs>? CloseRequested;

    IInputElement FocusScope { get; }

    ValueTask ShowAsync(CancellationToken cancellationToken);

    ValueTask CloseAsync();
}
```

约束：

- 生产路径由 `DialogSession.Create(...)` 根据 HostType 创建已知 Presenter；internal Session 构造函数可以直接接收 IDialogPresenter 供状态机测试，不提供 factory 类型或 service locator。
- `ShowAsync` 在视觉已附加、布局和 opening motion 完成后结束。
- `CloseAsync` 不接受用户 cancellation token；一旦提交关闭，teardown 必须完成。
- `CloseAsync` 幂等，多次调用返回同一关闭过程。
- `DisposeAsync` 是异常兜底：取消 motion、移除视觉、清空 Content、解除 logical parent 和 Host 事件，不再次请求业务关闭。
- `CloseRequested` 只表达 Host/用户意图，Session 决定是否允许关闭。
- 动态属性通过 AXAML 或明确由 Session 持有的绑定更新，不为每个属性增加 Update 方法。

## 9. Overlay Presenter 与 DialogOverlayLayer

### 9.1 结构

OverlayDialogPresenter 自身是 `TemplatedControl`，也是 DialogOverlayLayer 的直接子项：

```text
DialogOverlayLayer
├── OverlayDialogPresenter
│   ├── mask
│   └── DialogSurface
└── OverlayDialogPresenter
    ├── mask
    └── DialogSurface
```

不增加 Entry 或 Container 中间类型。

OverlayDialogPresenter 负责：

- 当前 Dialog 的 mask 和 surface 共同视觉树。
- owner bounds、startup placement、offset 和约束。
- 拖动、缩放、最大化、恢复和 motion anchor。
- mask 与 surface 的并行 opening/closing motion。
- close button、mask click 和 surface 输入转换为 CloseRequested。
- topmost 激活后的 focus scope 和键盘输入。

### 9.2 Layer

DialogOverlayLayer 复用并安装在现有 `ScopeAwareOverlayLayer` 中。它只服务 Dialog：

- 每个作用域最多一个 DialogOverlayLayer。
- Children 只包含 OverlayDialogPresenter。
- Children 顺序就是 z-order，不维护第二份 entry model。
- BringToFront 移动整个 Presenter，因此 mask 和 surface 原子移动。
- 只有最上层活动 Presenter 响应 ESC、mask click 和 modal input trap。
- modeless Presenter 不阻止底层命中。
- 上层关闭后重新激活下层 Presenter。
- Layer 为空时从宿主层移除并释放宿主引用；不形成永久 TopLevel cache。

### 9.3 焦点

- Session 在 Show 前捕获当前 focused element。
- OverlayDialogPresenter 使用 Avalonia focus scope 和循环 tab navigation。
- 默认焦点优先级：DefaultStandardButton、首个可聚焦正文元素、surface 本身。
- 嵌套 Dialog 关闭时，优先恢复到下层 Dialog 中原先的焦点。
- 最后一个 Dialog 关闭时，恢复到打开前元素；元素已 detach/disabled/invisible 时回退到 placement target 或 owner。
- ESC 只由 DialogOverlayLayer 发送给 topmost Presenter，不由每个 Dialog 独立监听 TopLevel。

### 9.4 布局

- Presenter 覆盖 Dialog 的 owner 区域，mask 和 surface 使用同一坐标空间。
- owner/TopLevel bounds 变化直接触发布局和约束更新。
- 不使用 Dispatcher 延迟补偿位置。
- maximize 状态由 Presenter 的单一状态字段或 DirectProperty 表达，并投影到 header checked state。
- restore bounds 只由 Presenter 拥有，关闭时清空。

## 10. Window Presenter

WindowDialogPresenter 是 Window shell，并直接实现 IDialogPresenter：

- Content 是共享 DialogSurface。
- modal 且 owner 为 Window、平台支持原生 modal 时使用 `ShowDialog(owner)`。
- modeless 使用 `Show()`。
- 不支持原生 Window 的平台在 Session 创建阶段选择 Overlay Presenter。
- Window Closing 默认先取消原生关闭并发送 CloseRequested；Session 提交后 Presenter 才允许实际 Close。
- owner 销毁或 Window 已不可阻止地关闭时通知 Session 强制 teardown。
- `ShowAsync` 等待 Window Opened、surface layout 和 opening motion，而不是等待 Window lifetime。
- `CloseAsync` 等待 closing motion 和 Window Closed。
- 原生 `WindowState` 是最大化、最小化和恢复的唯一状态 owner。
- Width/Height/Min/Max 使用实际 DialogSurface measure 和 `SizeToContent` 计算，删除魔法尺寸补偿。
- Topmost、CanResize、CanMaximize、CanMinimize 和 move capability 直接映射到 Window。

Window 和 Overlay 的能力差异由两个具体 Presenter 内部处理，不在 Dialog 中进行具体类型判断。

## 11. DialogSurface、内容与资源

### 11.1 共享 Surface

DialogSurface 是 Overlay 和 Window 共同使用的内部 TemplatedControl：

```text
DialogSurface
├── header
├── loading/body presenter
└── footer
    └── DialogButtonBox
```

它持有一个明确的 Dialog source，通过 AXAML 绑定标题、图标、loading、footer、按钮和视觉状态。

DialogSurface 只发送：

- CloseRequested。
- MinimizeRequested。
- MaximizeOrRestoreRequested。
- BeginMove/Resize 请求。
- ButtonClicked。

Presenter 或 Session 处理请求，Surface 不修改 Session 状态。

### 11.2 用户 Content

- 正文始终通过标准 ContentPresenter 承载。
- 支持 Control、字符串、数据对象、DataTemplate 和运行时 Content 替换。
- 不调用 `SetTemplatedParent`、`ApplyTemplate` 或反射 API。
- Session teardown 时先从 Surface 清空 Content，再释放对 Dialog 和用户内容的引用。
- 同一个 Dialog 重新打开时可以重新承载原 Content；不保留旧 Presenter visual。

### 11.3 MessageBox 正文

Dialog 提供一个仅 assembly 内可覆盖的 body presenter 创建点：

- Dialog 默认创建标准 ContentPresenter。
- MessageBox 创建 MessageBoxContent，包含语义图标和用户正文。
- 该创建点不进入 public/protected 扩展契约。
- 运行时创建的 binding 全部存入 Session disposables；关闭、打开失败和重开时对称释放。

### 11.4 logical/resource owner

- 声明式 Dialog 已在 logical tree 中时，Presenter 以 Dialog 为 logical/resource owner。
- 静态创建且未附加的 Dialog 以 placement target 或 owner 为 resource owner，同时显式绑定 Dialog source/DataContext。
- Presenter 加入视觉层前设置 logical owner，移除视觉后立即清除。
- acquire/release 必须位于同一个 Session teardown 中。
- 不创建全局 token binding、永久 TopLevel cache 或 Application 级资源订阅。
- 不新增反射；NativeAOT 绑定使用编译可分析的属性和 AXAML 路径。

## 12. DialogButtonBox

保留现有 `DialogButton` 和 `DialogStandardButton` 产品模型，不引入新的 DialogAction 层。

DialogButtonBox 重写为一个有效按钮序列 owner：

- 标准按钮由一处工厂方法或私有创建逻辑生成，不复制九组近似代码。
- CustomButtons 直接进入当前 Surface 的 ButtonBox，不再经过 Dialog -> Host -> ButtonBox 多次镜像。
- Add、Remove、Replace、Move、Reset 和 Clear 全部支持。
- StandardButtons、DefaultStandardButton、EscapeStandardButton 和 ButtonsConfigure 运行时变化立即更新。
- default 和 escape 只能指向当前有效且可用的按钮。
- 一个 DialogButton 在任意时刻只有一个视觉父级。
- template reapply 时解绑旧按钮事件并释放旧生成按钮。
- Session 关闭时清空生成按钮和 custom visual parent，但不销毁用户提供的 DialogButton 对象。
- `IsConfirmLoading` 抑制 button、caption close、ESC 和 mask click 等用户来源的关闭请求；程序化 Done、`IsOpen=false`、token、owner close 和 detach 不受它阻止。

本次不增加 Ant Design ActionButton 的自动 Promise loading API；当前需求只修复现有按钮行为和生命周期。

## 13. MessageBox 设计

`MessageBox : Dialog`。

这是语义继承：MessageBox 是带固定正文布局和默认 action policy 的 Dialog，而不是另一个生命周期 owner。

MessageBox 只增加或覆盖：

- `MessageBoxStyle`。
- 语义 Icon。
- OK/Cancel 文案和 OK button style。
- MessageBoxContent body presenter。
- MessageBox Token 默认最小尺寸。
- Confirmed/Cancelled 语义事件映射。

按钮默认规则通过 MessageBox theme/style setter 或 effective value 计算表达，允许调用方 local value 覆盖：

| Style | 默认标准按钮 | Default | Escape |
| --- | --- | --- | --- |
| Confirm | OK + Cancel | 按 OkButtonStyle 决定 OK 或无 | Cancel |
| Information/Success/Warning/Error/Normal | OK | 按 OkButtonStyle 决定 OK 或无 | 无 |

MessageBox 不再拥有：

- `_dialog`。
- `_dialogBindings` 和 `_dialogContentBindings`。
- `_ignoreIsOpenChanged`。
- Dialog 事件转发器。
- CustomButtons 镜像集合。
- 独立 Open/Close 状态实现。

MessageBox 的声明式、实例和静态调用全部直接使用继承的 Dialog Session。Dialog 生命周期契约测试必须参数化覆盖 MessageBox，不能只测试其语义样式。

## 14. Motion

复用现有 `AtomUI.MotionScene`：

- opening/closing 使用 `AbstractMotion.RunAsync` 的真实 completion task。
- Overlay mask 和 surface 使用 `Task.WhenAll`。
- Window surface 使用同一 surface motion；Window shell 的实际关闭在 motion 后执行。
- motion disabled 时同步设置最终视觉状态并返回完成任务。
- close during opening 时取消 opening motion，设置确定的当前/最终状态，再运行或直接完成 closing。
- forced teardown 若 owner 已失效，可以跳过不可显示的 close motion，但仍执行相同资源释放路径。
- 不使用 `DispatcherTimer.RunOnce`、固定 `Task.Delay` 或 transition duration 猜测完成。
- 不建设新的通用 DialogMotionRunner；motion 方法属于具体 Presenter/Surface。

## 15. API 与事件调整

### 15.1 实例 API

保留：

- `IsOpen`。
- `Accept()`。
- `Reject()`。
- `Done()` / `Done(object?)`。

统一新增或重定义：

```csharp
public Task<object?> ShowAsync(CancellationToken cancellationToken = default);
```

语义：无论 modal 或 modeless，ShowAsync 都等待完整 Session Closed 后返回结果。`IsModal` 只决定 owner 输入是否被阻断。

同一实例已有活动 Session 时再次 ShowAsync 必须给出确定行为：

- Opening、Open 或 ClosePending：抛出 `InvalidOperationException`，禁止多个 cancellation owner 共享一个 Session。
- Closing：等待旧 Session 完成不是隐式行为；调用方应在 Closed 后重新调用或使用 `IsOpen=true` 的声明式重开语义。

删除同步 `Open()`。

### 15.2 静态 API

静态 Dialog 和 MessageBox API 收敛为异步返回结果：

- 创建控件。
- 应用 Options。
- 解析 placement target/owner。
- 调用同一个实例 `ShowAsync`。
- 在 `finally` 中释放静态创建实例的临时 logical/resource attachment。

删除同步、callback 和 ModalAsync/Async 两套 lifetime 语义。modal 由 Options/实例 `IsModal` 指定。

### 15.3 事件

保留功能语义并重新定义稳定时序：

- `Opened`：可见、布局、opening motion 和焦点完成。
- `Closing`：关闭 policy 阶段，携带 Reason 和 CanCancel。
- `Accepted` / `Rejected`：结果已提交。
- `Finished`：任意结果已提交。
- `Closed`：视觉、资源、logical parent 和焦点 teardown 全部完成。

不承诺旧代码中的偶然相对顺序，只承诺本设计第 7 节定义的顺序和单次触发。

## 16. 建议文件边界

```text
src/AtomUI.Desktop.Controls/Dialog/
├── Dialog.cs                         # public contract、IsOpen reconcile、commands/events
├── Dialog.StaticAPI.cs               # unified async static API
├── DialogSession.cs                  # one-session state machine and teardown
├── DialogSessionState.cs             # internal enum only
├── DialogClosingContext.cs           # close reason、result、source、cancellation
├── IDialogPresenter.cs               # minimal internal async protocol
├── Presentation/
│   ├── DialogSurface.cs              # shared visual surface
│   ├── OverlayDialogPresenter.cs     # overlay visual + presenter protocol
│   └── WindowDialogPresenter.cs      # native Window + presenter protocol
├── Overlay/
│   └── DialogOverlayLayer.cs         # stack/topmost/input activation
├── ButtonBox/
│   └── DialogButtonBox.cs            # effective buttons and key handling
└── Themes/
    ├── DialogSurfaceTheme.axaml
    ├── OverlayDialogPresenterTheme.axaml
    └── DialogWindowTheme.axaml

src/AtomUI.Desktop.Controls/MessageBox/
├── MessageBox.cs                     # Dialog specialization
├── MessageBox.StaticAPI.cs           # thin async constructors
├── MessageBoxContent.cs              # semantic body presenter
└── Themes/
    ├── MessageBoxTheme.axaml
    └── MessageBoxContentTheme.axaml
```

`DialogSessionState.cs` 可以在实现时留在 `DialogSession.cs`，如果 enum 只被该文件使用；不为了匹配目录图机械拆文件。

最终删除：

- `IDialogHost.cs`。
- `WindowHost/DialogHost.cs`。
- `WindowHost/DialogWindowContent.cs`。
- 当前 `OverlayHost/OverlayDialogHost.cs` 和独立 `OverlayDialogMask.cs`。
- Dialog 内部 `DialogOpenState`。
- MessageBox 隐藏 Dialog 和 relay binding 实现。

现有 header、resizer、caption button 等有独立视觉职责的类型可以复用或改名，不因重构机械删除。

## 17. 测试设计

### 17.1 Session 状态机 P0

- 正常 Opening -> Open -> Closing -> Closed。
- Closing veto 返回 Open。
- 每个关闭入口只提交一次结果。
- Accept/Reject/Done 连续调用只接受第一个提交。
- `IsOpen=true -> false` 在 Host 创建前取消打开。
- `true -> false -> true` 最终重新打开且只有一个活动 Presenter。
- Closing motion 期间 `IsOpen=true` 在旧 Session Closed 后重开。
- ClosePending 期间 `IsOpen=true` 取消 async policy。
- BeforeCloseAsync resolve false、throw、cancel 和延迟完成。
- 事件在提交前、提交后和 Closed 阶段抛异常。

### 17.2 cancellation/forced teardown P0

- token 在调用前已取消。
- token 在 placement resolution、Presenter attach、layout、opening motion、Open、ClosePending 和 closing motion 时取消。
- owner 在 Opening/Open/ClosePending/Closing 时关闭。
- placement target 在相同阶段 detach。
- `IsConfirmLoading=true` 时 owner/token 仍能强制关闭。
- teardown 后 Session、Presenter、Content、owner subscriptions 和 completion 均结束。

### 17.3 Overlay P0

- mask 与 surface 始终位于同一个 OverlayDialogPresenter。
- BringToFront 原子移动 mask 和 surface。
- 两层和三层嵌套只让 topmost 响应 ESC/mask。
- modal 阻止底层输入，modeless 不阻止。
- 关闭上层后恢复下层焦点；关闭最后一层恢复触发元素。
- target/owner resize、move、maximize 和 restore 后布局正确。
- motion anchor 使用显式 target；静态 fallback target 只负责 Host 解析。

### 17.4 Window P0

- modal 使用原生 ShowDialog，modeless 使用 Show。
- Window close button 可以被正常 policy veto。
- owner close 绕过 veto 并完整 teardown。
- Window/Overlay 的 Opened、Finished、Closed 时序一致。
- sizing 不含固定补偿，min/max 和 SizeToContent 正确。
- maximize/minimize/header 状态一致。

### 17.5 Content 与按钮 P0

- Control、字符串、数据对象和 DataTemplate 正确显示。
- Open 状态动态替换 Content 和 ContentTemplate。
- 用户 Control 不被设置 TemplatedParent。
- CustomButtons Add/Remove/Replace/Move/Reset/Clear。
- StandardButtons、Default、Escape 和 ButtonsConfigure 动态变化。
- template reapply 不保留旧按钮 handler 或视觉父级。

### 17.6 MessageBox P0

- 所有 Dialog Session 契约对 MessageBox 同样成立。
- Confirm/Information/Success/Warning/Error/Normal 的图标和按钮正确。
- OkButtonStyle、OK/Cancel 文案和 custom buttons 正确。
- MessageBox 实例不创建隐藏 Dialog。
- 声明式、实例和静态 API 使用同一 Session。
- Overlay/Window、modal/modeless、取消和嵌套组合均覆盖。

### 17.7 生命周期、资源和平台 P1

- WeakReference：关闭后 Dialog、MessageBox、Presenter、Surface、Content 和旧 Gallery ShowCase 可回收。
- 局部 resource/theme variant 在 Overlay 和 Window 中正确更新。
- detach 后旧 resource owner 不再收到订阅。
- 浏览器 fallback 选择 Overlay。
- 原 PR 视频场景重放，并同时断言 Session Closed、IsOpen=false、订阅释放和可重开。
- Gallery Modal 页面视觉回归。
- Gallery NativeAOT publish 和运行 smoke test。

## 18. 实施阶段

### 阶段 1：行为契约

- 建立可注入 fake Presenter 的 DialogSession 测试入口，但不引入 Presenter factory 类型。
- 编写状态机、close policy、cancellation、事件异常和资源 teardown 测试。
- 删除或重写只用于固化 DispatcherFrame、同步返回时点和旧错误时序的测试。

### 阶段 2：Session 与共享 Surface

- 实现 DialogSession 和最小 IDialogPresenter。
- 实现 DialogSurface、正文承载和 ButtonBox 重写。
- 使用 fake Presenter 让核心状态机先稳定，不依赖真实 Window/Overlay。

### 阶段 3：Overlay

- 实现 DialogOverlayLayer 和直接作为 Layer child 的 OverlayDialogPresenter。
- 完成 mask/surface、栈、焦点、布局、拖动缩放和真实 motion completion。
- 迁移 Overlay 定向测试和 Gallery 场景。

### 阶段 4：Window

- 实现 WindowDialogPresenter。
- 对齐 Overlay/Window 的 presenter contract、事件和 teardown。
- 完成原生 modal、modeless、owner close、尺寸和 WindowState 测试。

### 阶段 5：Dialog 切换

- 将 Dialog `IsOpen`、命令、事件和静态 API 切到新 Session。
- 删除 DispatcherFrame、DialogOpenState 和旧 Host 创建路径。
- 完成快速翻转、重开、异常和弱引用验证。

### 阶段 6：MessageBox 切换

- 改为 `MessageBox : Dialog`。
- 迁移 MessageBoxContent、样式、按钮默认值和静态 API。
- 删除隐藏 Dialog、relay bindings、事件转发和 suppression flag。

### 阶段 7：删除旧架构与验收

- 删除旧 Host、Popup mask、callback close 和无引用主题。
- 更新 Gallery API 展示、Dialog/MessageBox 文档和 changelog。
- 运行 Desktop Controls 全量测试、Gallery 测试、WeakReference 测试、NativeAOT publish 和 `git diff --check`。

阶段之间允许新旧内部实现短暂并存以保持提交可构建，但不增加兼容 adapter，也不让最终代码保留两套 lifetime。

## 19. 过度设计防线

实现评审时逐项检查：

- 新类型是否对应本设计中明确的 owner 或视觉职责。
- 是否可以用 DialogSession state 代替新增 boolean flag。
- 是否可以用现有 ScopeAwareOverlayLayer、MotionScene 或 AXAML binding 代替新框架。
- 是否为假想第三种 Presenter、外部插件或通用 Portal 增加扩展点。
- 是否把只使用一次的 enum/helper/record 机械拆成独立文件。
- 是否引入 DialogAction、DialogService、PresenterFactory、operation queue 或全局 manager；除非出现本设计未覆盖且有测试证明的现实需求，否则禁止。
- 是否存在 callback close、timer 猜完成、Dispatcher 延迟同步或 suppression flag；存在即退回重设计。

避免过度设计不能缩减正确性范围。取消、竞态、重开、重复关闭、forced teardown、嵌套焦点、事件异常、按钮集合、MessageBox 和内存回收均为本次必达项。

## 20. 验收标准

- Dialog 的每次展示只有一个 DialogSession 状态 owner。
- 所有关闭和取消入口最终进入同一 teardown。
- 不存在 DispatcherFrame、callback Close、DialogOpenState 或 suppression flag。
- Overlay mask 与 surface 由同一个 OverlayDialogPresenter 拥有。
- DialogOverlayLayer 只维护 Presenter children，不维护重复 entry model。
- Window 和 Overlay 遵循相同 Presenter 和事件契约。
- `Opened` 在可交互后触发，`Closed` 在完整 teardown 后触发，均最多一次。
- cancellation、owner close 和 detach 不受 loading 或 veto 阻止。
- 用户 Content 使用正常 ContentPresenter 和 logical tree，不设置 TemplatedParent。
- CustomButtons 所有集合操作和动态 default/escape 正确。
- MessageBox 继承 Dialog，不创建隐藏 Dialog 或第二套状态。
- opening/closing 等待真实 MotionScene completion，不使用固定 timer。
- 关闭和 Gallery 导航后旧对象可被 GC。
- Overlay、Window、MessageBox、浏览器 fallback 和 NativeAOT 验证全部通过。
