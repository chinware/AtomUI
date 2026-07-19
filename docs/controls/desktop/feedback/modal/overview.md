# Modal 桌面版架构设计

本文档定义 `Dialog` 和 `MessageBox` 的当前公共设计。内部状态机与宿主实现见 [Modal 桌面版实现原理](implementation.md)，视觉变量见 [Modal Token 设计](token.md)，历史变化见 [Modal Changelog](changelog.md)。

## 1. 控件定位

| 项 | 值 |
| --- | --- |
| NuGet 包 | `AtomUI.Desktop.Controls` |
| .NET 命名空间 | `AtomUI.Desktop.Controls` |
| AXAML 命名空间 | `https://atomui.net` |
| Gallery 页面 | `controlgallery/AtomUIGallery/ShowCases/Feedback/Modal` |
| 控件状态 | Stable |
| LLMS 可见性 | public |

`Dialog` 用于需要独立内容表面、明确结果和受控关闭流程的桌面对话。它支持 Overlay 与原生 Window 两种宿主、modal 与 modeless 两种交互模式、声明式 `IsOpen` 和静态异步 API。

`MessageBox` 是 `Dialog` 的语义专化，用于信息、成功、警告、错误和确认消息。它复用 Dialog 的 Session、Presenter、关闭、尺寸、资源和焦点模型，不维护第二套隐藏 Dialog。

Modal 不承担通知队列、轻量 Tooltip、Popup 菜单或业务级导航服务职责。

## 2. 设计语言

- 对话表面由标题、内容、Footer 和操作按钮组成，Overlay 与 Window 共享同一个内容模型。
- modal 通过 mask 或原生 owner 关系阻断底层输入；modeless 保持底层可交互。
- 打开与关闭 motion 属于 Session 生命周期的一部分。异步打开在入场完成后触发 `Opened`，异步关闭在退出 motion、宿主移除和资源释放完成后结束。
- `MessageBoxStyle` 只表达消息语义和默认图标/按钮策略，不改变 Dialog 生命周期。

## 3. API 与契约模型

### 3.1 Dialog 契约组

| 契约组 | 代表成员 | 语义 |
| --- | --- | --- |
| 内容 | `Title`, `TitleIcon`, `Content`, `ContentTemplate`, `DataContext` | 定义标题和任意内容对象或模板。 |
| 打开状态 | `IsOpen`, `OpenAsync(...)` | `IsOpen` 是默认 TwoWay 的声明式意图；`OpenAsync` 表示一次完整 Session。 |
| 展示方式 | `DialogHostType`, `IsModal`, `PlacementTarget`, startup anchor/offset | 选择 Overlay/Window、交互模态和初始位置。 |
| 尺寸与窗口能力 | `HostWidth/Height/Min/Max`, `IsResizable`, `IsClosable`, `IsDragMovable`, `IsMaximizable`, `IsMinimizable`, `IsTopmost` | 同一组请求映射到 Overlay Surface 或原生 Window。`NaN` 表示自然尺寸。 |
| 操作 | `StandardButtons`, `CustomButtons`, `DefaultStandardButton`, `EscapeStandardButton`, `ButtonsConfigure` | 生成标准按钮、加入自定义按钮并配置当前有效按钮序列。 |
| 状态与策略 | `IsLoading`, `IsConfirmLoading`, `IsFooterVisible`, `IsMotionEnabled`, `BeforeCloseAsync` | 控制加载、确认按钮 loading、Footer、motion 和关闭前校验。 |
| 结果 | `Result`, `Accept()`, `Reject()`, `Done(...)` | 所有关闭来源归一为结果与 `DialogCloseReason`。 |

静态入口只有异步形式：

- `ShowDialogAsync(...)` 创建 modeless Dialog。
- `ShowDialogModalAsync(...)` 创建 modal Dialog。
- 两者都在完整 teardown 后返回 `Task<object?>`。

同一个实例在 `Opening`、`Open`、`ClosePending` 或 `Closing` 时再次调用 `OpenAsync` 会抛出 `InvalidOperationException`。声明式 `IsOpen=true` 可以在旧 Session 完整关闭后创建新 Session。

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

`ButtonClicked.Handled=true`、`Closing.Cancel=true` 或 `BeforeCloseAsync` 返回 `false` 会取消本次普通关闭。owner close、placement target detach、外部取消和 presenter 打开失败属于强制 teardown；它们忽略业务 veto 和 confirm loading，但仍执行完整释放。

事件处理器或 presenter 在结果提交后抛出的首个异常，只会在 teardown 完成后传播。

### 3.3 MessageBox 契约

`MessageBox : Dialog` 增加 `Style`、`Icon`、`OkButtonStyle`、`OkButtonText`、`CancelButtonText`、`IsCenterOnStartup`、`Confirmed`、`Cancelled`、`Confirm()` 和 `Cancel()`。

`ShowMessageBoxAsync(...)` 与 `ShowMessageBoxModalAsync(...)` 复用继承的异步 Session 语义。未显式指定 `MessageBoxOptions.MinWidth` 时保留 MessageBox Token 的默认最小宽度。

### 3.4 Template Parts

| Part | 所属类型 | 职责 |
| --- | --- | --- |
| `PART_Header` | `DialogSurface` | Overlay 标题栏、拖动与 caption 操作。 |
| `PART_ButtonBox` | `DialogSurface` | 当前标准/自定义按钮序列。 |
| `PART_Resizer` | `DialogSurface` | Overlay resize handles。 |
| `PART_LeftGroup` / `PART_CenterGroup` / `PART_RightGroup` | `DialogButtonBox` | 按按钮角色布局。 |
| `PART_MaskMotionActor` | `OverlayDialogPresenter` | modal mask 及其 motion。 |
| `PART_SurfaceMotionActor` | `OverlayDialogPresenter` | DialogSurface 入场/退出 motion。 |

当前没有 Modal 专属 pseudo class。

## 4. 行为与状态模型

- `IsOpen` 表示最新声明式意图；`DialogSession` 表示一次实际展示。两者不能由 presenter 或 template part 反向拥有。
- modal Overlay 的真实 pointer 输入命中 mask，modeless Overlay 在 Surface 外穿透到底层。只有栈顶 presenter 响应 mask 与 Escape。
- Enter/Escape 根据当前有效按钮序列查找 default/escape 按钮，运行时修改标准按钮或自定义按钮会立即生效。
- `IsConfirmLoading=true` 只阻止用户发起的普通关闭，不阻止 owner close、detach、取消和失败 teardown。
- 打开后焦点进入 DialogSurface；嵌套 Dialog 关闭时恢复下层 Surface，最后一层关闭时恢复原触发控件。
- Overlay 与 Window 都等待 opening/closing motion；`IsMotionEnabled=false` 跳过 motion，但不跳过宿主打开、关闭和释放。

## 5. 视觉与主题模型

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

## 6. 控件家族或集成关系

- `MessageBox` 继承 `Dialog`，只增加语义内容和按钮策略。
- Overlay 使用最近的 `ScopeAwareOverlayLayer`，不会使用全局静态 TopLevel 字典。
- Window 使用 Avalonia 原生 `Window` modal owner 能力；不支持原生 Window 的平台会回退到 Overlay。
- `IDialogAwareDataContext` 在 DataContext attach/detach 和 Session closed 时接收通知。
- Gallery 展示 Overlay/Window、modal/modeless、异步关闭、loading、自定义按钮、拖动与尺寸场景。

## 7. 兼容性不变量

后续维护必须保持当前稳定契约：

- 一次实际展示只有一个 `DialogSession`，一次关闭只有一个提交结果。
- Overlay 与 Window 使用相同内容、按钮、关闭、焦点和 teardown 语义。
- Content 可以是字符串、POCO 或 Control；实现不得修改用户 Control 的 TemplatedParent。
- 自定义按钮集合的 Add/Remove/Replace/Move/Reset/Clear 都要更新有效序列并对称管理事件订阅。
- mask、Surface、内容、按钮、binding、逻辑/资源 parent、owner/target 订阅必须在所有关闭路径释放。
- 不重新引入同步 DispatcherFrame、callback close、隐藏 MessageBox Dialog 或分离的 Popup mask。

## 8. 专项模型

### 8.1 Session 状态

```text
Created -> Opening -> Open -> ClosePending -> Closing -> Closed
                         ^          |
                         +-- veto --+
```

`ClosePending` 只用于一个正在执行的普通关闭策略。强制关闭可从 Created、Opening、Open 或 ClosePending 进入 Closing。

### 8.2 宿主选择

- `DialogHostType.Overlay` 使用 owner 范围内的 Dialog overlay stack。
- `DialogHostType.Window` 使用原生 Window；平台不支持时回退 Overlay。
- `IsModal` 只控制交互模态，不改变 `OpenAsync` 的任务边界。

## 9. 文档导航、LLMS 导出与验证策略

- [实现原理](implementation.md)
- [Token 设计](token.md)
- [控件级 Changelog](changelog.md)

LLMS 语义区域：

| Part | AtomUI 节点 | 职责 | 相关 API | 相关 Token | 稳定性 |
| --- | --- | --- | --- | --- | --- |
| `root` | `Dialog` / `MessageBox` | public 打开意图、内容、结果和事件入口。 | `IsOpen`, `OpenAsync`, `Result`, `BeforeCloseAsync` | `DialogToken` | stable |
| `host` | Overlay presenter / native Window | 承载模态、placement、尺寸和宿主生命周期。 | `DialogHostType`, `IsModal`, `PlacementTarget` | SharedToken motion | internal-observable |
| `surface` | `DialogSurface` | 共享标题、正文、Footer、按钮和 focus scope。 | `Content`, `StandardButtons`, `IsLoading` | `ContentBg`, padding/footer tokens | internal-observable |
| `content` | Content / `MessageBoxContent` | 呈现任意 Dialog 内容或 MessageBox 语义内容。 | `Content`, `ContentTemplate`, `Style`, `Icon` | typography/color tokens | stable |
| `motion` | `MotionActor` | 等待 opening/closing motion 并维持 task 边界。 | `IsMotionEnabled` | `MotionDurationMid` | internal-observable |

LLMS 导出来源：

| LLMS 内容 | 来源 | 说明 |
| --- | --- | --- |
| 单控件完整文档 | overview.md + Gallery API / Token / ShowCase | 生成 `controls/modal/index-cn.md` |
| 单控件语义文档 | overview.md + implementation.md + Themes 文件夹 + theme/template 信息 | 生成 `controls/modal/semantic-cn.md` |
| API 表 | Gallery ApiDataGrid 或源码 public surface | 不在 overview.md 中手工复制完整表 |
| Design Token 表 | Gallery DesignTokenDataGrid 或 Token 类型 | 不在 token.md 中手工复制生成表 |
| 示例 | Gallery ShowCase + source snippet catalog | 只引用稳定示例 |
| 源码索引 | implementation.md | 用于定位控件源码、主题和测试 |

验证按改动范围运行 Dialog/MessageBox 定向测试、完整 Desktop Controls 测试、Gallery 测试与构建；涉及 AOT 发布路径时执行 Gallery NativeAOT publish，并始终运行 `git diff --check`。
