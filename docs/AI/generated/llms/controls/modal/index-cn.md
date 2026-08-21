# Modal

> 生成产物：由源文档生成，不要手工编辑。修改内容请回到控件文档、源码 public surface、Token 类型或生成数据、Gallery ShowCase 或源码结构。

## 概述

`Dialog` 用于需要独立内容表面、明确结果和受控关闭流程的桌面对话。它支持 Overlay 与原生 Window 两种宿主、modal 与 modeless 两种交互模式、声明式 `IsOpen` 和静态异步 API。

`MessageBox` 是 `Dialog` 的语义专化，用于信息、成功、警告、错误和确认消息。它复用 Dialog 的 Session、Presenter、关闭、尺寸、资源和焦点模型，不维护第二套隐藏 Dialog。

Modal 不承担通知队列、轻量 Tooltip、Popup 菜单或业务级导航服务职责。

## 包与命名空间

| 项 | 值 |
| --- | --- |
| NuGet 包 | `AtomUI.Desktop.Controls` |
| .NET 命名空间 | `AtomUI.Desktop.Controls` |
| AXAML 命名空间 | `https://atomui.net` |
| Gallery 页面 | `controlgallery/AtomUIGallery/ShowCases/Feedback/Modal` |
| 状态 | Stable |

## 何时使用

- 对话表面由标题、内容、Footer 和操作按钮组成，Overlay 与 Window 共享同一个内容模型。
- modal 通过 mask 或原生 owner 关系阻断底层输入；modeless 保持底层可交互。
- Presenter 的真实展示边界属于 Session 生命周期的一部分。Overlay 在入场 motion 完成后触发 `Opened`，Window 在原生 `DialogWindow.Opened` 后触发；关闭任务分别等待 Overlay 退出 motion 或原生 `DialogWindow.Closed`，并在宿主移除和资源释放完成后结束。
- Overlay 关闭时，外层 Surface motion 与 `DialogSurface` 内容层的前景 opacity 动画并行；内容和按钮保持附着，直到所有关闭任务完成后才 teardown。Window 继续使用原生 Window 生命周期。
- `MessageBoxStyle` 只表达消息语义和默认图标/按钮策略，不改变 Dialog 生命周期。

## 公共 API

### 3.1 Dialog 契约组

| 契约组 | 代表成员 | 语义 |
| --- | --- | --- |
| 内容 | `Title`, `TitleIcon`, `Content`, `ContentTemplate`, `DataContext` | 定义标题和任意内容对象或模板。 |
| 打开状态 | `IsOpen`, `OpenAsync(...)` | `IsOpen` 是默认 TwoWay 的声明式意图；`OpenAsync` 表示一次完整 Session。 |
| 展示方式 | `DialogHostType`, `IsModal`, `PlacementTarget`, startup anchor/offset | 选择 Overlay/Window、交互模态和初始位置。直接实例化与静态 API 的水平、垂直 startup anchor 默认均为 `Center`；显式 `Custom` 时由对应 offset 决定位置。 |
| 尺寸与窗口能力 | `HostWidth/Height/Min/Max`, `IsResizable`, `IsClosable`, `IsMaskClosable`, `IsDragMovable`, `IsMaximizable`, `IsMinimizable`, `IsTopmost` | 同一组 Surface 正文尺寸请求映射到 Overlay 或原生 Window。`NaN` 表示初始自然尺寸；有效最小尺寸还必须满足 Dialog 的结构性下限。`IsClosable` 控制标题栏关闭入口，`IsMaskClosable` 控制 Overlay modal mask 外点关闭入口，两者正交且默认都为 `true`。 |
| 操作 | `StandardButtons`, `CustomButtons`, `DefaultStandardButton`, `EscapeStandardButton`, `ButtonsConfigure` | 生成标准按钮、加入自定义按钮并配置当前有效按钮序列。 |
| 状态与策略 | `IsLoading`, `IsConfirmLoading`, `IsFooterVisible`, `IsMotionEnabled`, `BeforeCloseAsync` | 控制加载、确认按钮 loading、Footer、motion 和关闭前校验。 |
| 结果 | `Result`, `Accept()`, `Reject()`, `Done(...)` | 所有关闭来源归一为结果与 `DialogCloseReason`。 |

静态入口只有异步形式：

- `ShowDialogAsync(...)` 创建 modeless Dialog。
- `ShowDialogModalAsync(...)` 创建 modal Dialog。
- 两者都在完整 teardown 后返回 `Task<object?>`。

同一个实例存在任何尚未进入 `Closed` 的 Session 时，再次调用 `OpenAsync` 会抛出 `InvalidOperationException`。声明式 `IsOpen=true` 可以在旧 Session 完整关闭后创建新 Session。

### 3.2 关闭事件

Dialog 公开 `Opened`、`Closing`、`Accepted`、`Rejected`、`Finished`、`Closed` 和 `ButtonClicked`。

正常关闭顺序：

```text
按钮来源时 ButtonClicked
  -> Closing
  -> BeforeCloseAsync
  -> 提交 IsOpen=false 与 Result
  -> Accepted 或 Rejected
  -> Finished
  -> 退出 motion 与 presenter teardown
  -> IDialogAwareDataContext.NotifyClosed
  -> Closed
```

`ButtonClicked.Handled=true`、`Closing.Cancel=true` 或 `BeforeCloseAsync` 返回 `false` 会取消本次普通关闭。owner close、placement target detach、外部取消和 presenter 打开失败属于强制 teardown：Session 仍触发一次 `Closing`，但忽略 `Cancel`、跳过 `BeforeCloseAsync`，也不受 confirm loading 阻止，随后执行完整释放。

事件处理器或 presenter 在结果提交后抛出的首个异常，只会在 teardown 完成后传播。

### 3.3 MessageBox 契约

`MessageBox : Dialog` 增加 `Style`、`Icon`、`OkButtonStyle`、`OkButtonText`、`CancelButtonText`、`IsCenterOnStartup`、`Confirmed`、`Cancelled`、`Confirm()` 和 `Cancel()`。

`ShowMessageBoxAsync(...)` 与 `ShowMessageBoxModalAsync(...)` 复用继承的异步 Session 语义。未显式指定 `MessageBoxOptions.MinWidth` 时保留 MessageBox Token 的默认最小宽度。`IsMaskClosable` 由 `Dialog` 继承并经 `MessageBoxOptions` 透传，语义与 Dialog 一致。

### 3.4 Template Parts

| Part | 所属类型 | 职责 |
| --- | --- | --- |
| `PART_Header` | `DialogSurface` | Overlay 标题栏、拖动与 caption 操作。 |
| `PART_ButtonBox` | `DialogSurface` | 当前标准/自定义按钮序列。 |
| `PART_Resizer` | `DialogSurface` | Overlay resize handles。 |
| `PART_LeftGroup` / `PART_CenterGroup` / `PART_RightGroup` | `DialogButtonBox` | 按按钮角色布局。 |
| `PART_MaskMotionActor` | `OverlayDialogPresenter` | modal mask 及其 motion。 |
| `PART_SurfaceMotionActor` | `OverlayDialogPresenter` | DialogSurface 入场/退出 motion。 |
| `PART_SurfaceContentLayer` | `DialogSurface` 内部模板节点 | 包围 Header、ContentFrame 和 FooterFrame；Overlay 关闭时承载前景 opacity 动画，不是 public Semantic Part。 |

当前没有 Modal 专属 pseudo class。

## 事件与命令

### 3.2 关闭事件
事件处理器或 presenter 在结果提交后抛出的首个异常，只会在 teardown 完成后传播。

## 使用示例

稳定示例来源于 Gallery ShowCase 和源码查看片段。生成器只输出可从 `ShowCaseItem` 追溯的示例，不维护第二套手写示例。

- `controlgallery/AtomUIGallery/ShowCases/Feedback/Modal/Views/ModalShowCase.axaml`
- `controlgallery/AtomUIGallery/ShowCases/Feedback/Modal/Views/ModalUserControlView.axaml`

## 状态模型

- `IsOpen` 表示最新声明式意图；`DialogSession` 表示一次实际展示。两者不能由 presenter 或 template part 反向拥有。
- modal Overlay 的真实 pointer 输入命中 mask，modeless Overlay 在 Surface 外穿透到底层。只有栈顶 presenter 响应 mask 与 Escape。栈顶 modal mask 外点默认以 `HostCloseRequest` 发起普通关闭；`IsMaskClosable=false` 时该次点击被吞掉且不产生任何关闭请求，不进入 `Closing`/`BeforeCloseAsync` 管道。Window host 没有 mask，外点本来就不触发关闭。
- 所有平台的 `AtomUI.Window` 都把 Overlay presenter 放在 owning `TopLevel` 的 Avalonia `OverlayLayer`。modal 活跃时通过 Window 引用计数租约隐藏 managed/drawn chrome overlay，使 mask 覆盖完整 Avalonia 可绘制窗口轮廓并阻断 caption input；位于客户端 visual tree 外的原生系统 chrome 仍由平台管理。
- Dialog 内容区内的 popup 类控件(ComboBox、Select、DatePicker、Tooltip、Flyout、ContextMenu 等)由同一 Window 的 `PopupOverlayLayer` 或原生 Popup host 承载，位于 Dialog `OverlayLayer` 之上并保持可命中；二者之间的 `LightDismissOverlayLayer` 保证外点关闭与输入穿透语义和普通页面一致。层级与所有权契约见 [Modal 内容弹层叠放设计](popup-layering-design.md)。
- Overlay mask bounds、Window visible frame 与 Dialog 正文 owner bounds 独立：mask 使用完整 layer bounds；所有平台的 Surface 正文定位、拖动、resize 和 maximize 使用 visible frame 按当前有效 drawn frame thickness 内缩后的范围，允许进入 managed/drawn title bar，但不能覆盖窗口 frame。Dialog BoxShadow 只参与绘制并允许在窗口边缘由统一 visual-layer clip 裁剪。
- Enter/Escape 根据当前有效按钮序列查找 default/escape 按钮，运行时修改标准按钮或自定义按钮会立即生效。
- `IsConfirmLoading=true` 只阻止用户发起的普通关闭，不阻止 owner close、detach、取消和失败 teardown。
- 打开后焦点进入 DialogSurface；嵌套 Dialog 关闭时恢复下层 Surface，最后一层关闭时恢复原触发控件。
- Overlay 等待 mask 与 Surface 的 opening/closing motion；关闭时同一 presenter 还等待内容层 opacity 动画，并在聚合任务完成后才断开 composition children、释放 Surface 和移除 layer。`IsMotionEnabled=false` 只跳过这些 motion，不跳过宿主附加、移除和释放。Window 不创建 Surface `MotionActor`，其打开与关闭分别等待原生 `DialogWindow.Opened` 和 `DialogWindow.Closed`。
- `IsResizable=true` 允许在有效尺寸区间内交互缩放，不表示无约束 resize。结构性最小尺寸在宿主容量允许时始终保留标题、Footer 和非零正文 viewport；`HostMin*` 只能提高该下限，`HostMax*=PositiveInfinity` 仍受 owner 或 screen capacity 限制。Overlay handle 捕获 pointer，release 或 capture lost 都会完整结束当前 resize，不复用上一次拖拽 origin。

## 主题与 Design Token

| 主题文件 | 职责 |
| --- | --- |
| `DialogTheme.axaml` | Dialog 默认属性和 Token 映射。 |
| `DialogSurfaceTheme.axaml` | 共享标题、内容、Footer、按钮与 resize 结构。 |
| `OverlayDialogPresenterTheme.axaml` | 同一 presenter 内组合 mask 与 Surface motion。 |
| `DialogButtonBoxTheme.axaml` | 三组按钮布局。 |
| `OverlayDialogHeaderTheme.axaml` | Overlay 标题栏。 |
| `OverlayDialogMaskTheme.axaml` | modal mask。 |
| `OverlayDialogResizerTheme.axaml` | Overlay resize handles。 |
| `MessageBoxTheme.axaml` / `MessageBoxContentTheme.axaml` | MessageBox 默认尺寸和语义内容。 |

`DialogToken` 提供背景、文字、间距、尺寸和 Footer 视觉。motion duration 使用 Dialog scope 的 SharedToken `MotionDurationMid`，不在代码中硬编码。

Token 来源：

Modal Token 只表达组件级视觉变量，例如尺寸、间距、颜色、圆角、阴影、图标尺寸和弹层边界。Token 不承载运行时选择、展开、加载、错误、上传任务、过滤条件或业务状态。

当前 Token scope：

- `DialogToken`，scope id 为 `Dialog`，源码位于 `src/AtomUI.Desktop.Controls/Dialog/DialogToken.cs`。

## AOT 与裁剪注意事项

- Overlay presenter 在 Dialog 已附加时以 Dialog 为 inheritance parent，否则以 placement target 为 parent。
- Window 保留 DialogSurface 到 Window `ContentPresenter` 的正常 styling parent 链，避免在未附加树中提前实例化的嵌套控件失去 ControlTheme。Dialog/owner 资源由 presenter-owned `DialogResourceBridge` 转发到 Window resources；bridge 对称转发 `ResourcesChanged`，并在 `DisposeAsync` 中移除和退订。
- runtime binding 只用于动态 presenter/Surface/按钮关系，并由 owning presenter、Surface 或 ButtonBox 对称释放。
- Presenter 为 Surface 复用单一 `MatrixTransform` 作为位置 owner。拖动 `PointerMoved` 只更新 Matrix translation 并同步不触发布局的 `Dialog.OffsetX/Y`；位置先按 DPI 取整，再二次 clamp 到 body owner bounds，避免取整重新越界。
- drawn decorations 反射兼容边界只读取 frame/titlebar 几何；Modal 不反射发现业务 host，也不新增 trimming root。实现不使用反射修改 TemplatedParent，不扫描程序集发现 Dialog API，不使用同步 DispatcherFrame。
- Session、Presenter、Surface 和 Content 的关闭回收由 Overlay/Window WeakReference 测试覆盖。
- 状态机、按钮表和 presenter 选择都是静态类型路径，保持 NativeAOT 友好。

## 源码索引

| 路径 | 职责 |
| --- | --- |
| `Dialog.cs` | public 属性、事件、内容、按钮和内部协作入口。 |
| `Dialog.Lifecycle.cs` | `IsOpen` reconcile、`OpenAsync`、事件通知和 presenter 选择。 |
| `Dialog.StaticAPI.cs` | 静态 modeless/modal 异步创建入口。 |
| `DialogSession.cs` | 单次展示状态机、关闭仲裁、取消、结果、焦点和 teardown。 |
| `IDialogPresenter.cs` | Overlay/Window 共用的最小异步协议。 |
| `DialogSurface.cs` | 标题、内容、Footer、按钮和 Overlay resize 的共享表面；负责 `PART_SurfaceContentLayer` 的 template part 生命周期。 |
| `ButtonBox/DialogButtonBox.cs` | 标准按钮生成、唯一有效按钮序列和自定义集合同步。 |
| `OverlayHost/DialogOverlayLayer.cs` | 解析 owning TopLevel 的 Avalonia `OverlayLayer` 或局部 scope fallback，并管理 owner scope 内的 presenter stack。 |
| `OverlayHost/OverlayDialogPresenter.cs` | 同时拥有 mask、Surface、placement、drag/resize 和 Overlay close motion choreography。 |
| `WindowHost/WindowDialogPresenter.cs` | 原生 Window 属性映射、modal owner、尺寸、位置和生命周期。 |
| `WindowHost/DialogWindow.cs` | 原生 caption close 仲裁和显式尺寸应用。 |
| `MessageBox/MessageBox.cs` | Dialog 派生的消息语义、静态 API 和按钮配置。 |
| `MessageBox/MessageBoxContent.cs` | MessageBox 的图标与内容组合。 |
| `Dialog/Themes` / `MessageBox/Themes` | 共享 Surface、Overlay presenter 和 MessageBox AXAML 结构。 |
| `src/AtomUI.Core/MotionScene/AbstractMotion.cs` | 共享 Motion 的 transition completion boundary；等待全部 transition 或安全超时后才报告完成。 |

对应回归测试位于 `tests/AtomUI.Desktop.Controls.Tests/Dialog` 和 `tests/AtomUI.Desktop.Controls.Tests/MessageBox`。

## 相关文档

- 源设计文档：`docs/controls/desktop/feedback/modal/overview.md`
- 实现文档：`docs/controls/desktop/feedback/modal/implementation.md`
- Token 文档：`docs/controls/desktop/feedback/modal/token.md`
- 变更记录：`docs/controls/desktop/feedback/modal/changelog.md`
- 语义结构：`./semantic-cn.md`
