# NavMenu 桌面版实现原理

本文档描述 NavMenu 桌面版的内部容器生成、交互 handler、选择协调、默认路径 replay、popup 接入和主题状态维护。公共设计与 API 契约见 [NavMenu 桌面版架构设计](overview.md)，Token 语义见 [NavMenu Token 设计](token.md)，变化记录见 [NavMenu Changelog](changelog.md)。

## 1. 实现定位

NavMenu 的实现目标是在 `ItemsControl` 容器体系内维护树形导航状态，并按 mode 选择不同交互策略。实现文档聚焦 `NavMenu`、`NavMenuItem`、节点模型、命令投影、handler、selection coordinator、keyboard navigation coordinator、inline collapsed coordinator 和 theme part 的协作关系。

路由切换、权限过滤、业务命令编排和页面生命周期不属于 NavMenu 实现范围。

## 2. 源码文件结构

主要源码：

- `src/AtomUI.Desktop.Controls/NavMenu/NavMenu.cs`：公开控件、属性、ItemsControl 容器入口、默认路径 replay 和 mode 状态同步。
- `src/AtomUI.Desktop.Controls/NavMenu/NavMenuItem.cs`：内部容器、header 转发、子菜单、popup、选中和打开状态。
- `src/AtomUI.Desktop.Controls/NavMenu/NavMenuNode.cs`：`INavMenuNode` 节点契约和节点数据模型；`NavMenuNode` 使用 `[GenerateScopedResourceHost]` 获得 scoped `IResourceHost` / `IThemeVariantHost` 生命周期。
- `src/AtomUI.Desktop.Controls/NavMenu/INavMenu.cs`、`INavMenuItem.cs`、`INavMenuElement.cs`：菜单和容器的内部/公共契约。
- `src/AtomUI.Desktop.Controls/NavMenu/NavMenuSelectionCoordinator.cs`：选择状态和祖先路径状态同步。
- `src/AtomUI.Desktop.Controls/NavMenu/NavMenuItemContainerBinder.cs`：节点数据与容器状态绑定。
- `src/AtomUI.Desktop.Controls/NavMenu/NavMenuInteractionHandlerBase.cs`：交互 handler 基类。
- `src/AtomUI.Desktop.Controls/NavMenu/DefaultNavMenuInteractionHandler.cs`：Vertical / Horizontal popup 模式交互策略。
- `src/AtomUI.Desktop.Controls/NavMenu/InlineNavMenuInteractionHandler.cs`：Inline 展开收起交互策略。
- `src/AtomUI.Desktop.Controls/NavMenu/Header/`：三种 header 控件。
- `src/AtomUI.Desktop.Controls/NavMenu/Themes/`：root、item、header 和 popup 主题。
- `src/AtomUI.Desktop.Controls/NavMenu/NavMenuToken.cs`：控件 Token。

## 3. 核心类职责

`NavMenu` 是树形导航根，负责 mode、theme、items source、默认路径、受控选择、事件和子容器状态下发。

`NavMenuItem` 是内部容器，负责承载单个节点的 header、icon、子节点、popup 或 inline child items，并维护 `IsSelected`、`IsInSelectedPath`、`IsSubMenuOpen`、`Level`、`IsTopLevel` 等状态。它实现 `ICommandSource`，是节点命令的唯一执行者和 `CanExecuteChanged` 订阅 owner。

`NavMenuNode` 是 owner-managed 非 Visual `AvaloniaObject`。它保存节点数据、`Command` 和 `CommandParameter`，但不持有 generated container，不执行命令，也不直接订阅 `ICommand.CanExecuteChanged`。scoped resource-host 样板由 generator 生成，owner 侧 attach token 与节点属性 relay binding 使用同一个容器 disposable 生命周期。

inline collapsed coordinator 由 `NavMenu` 拥有，负责根据 `Mode` 和 `IsInlineCollapsed` 计算 effective mode，缓存 inline 打开路径，关闭折叠期间的临时 popup，并把折叠视觉状态下发到 `NavMenuItem` 和 header。它不拥有选择状态，也不直接修改 `Mode`。

`NavMenuSelectionCoordinator` 统一处理旧选中节点清理、新选中节点设置、祖先路径标记和事件派发，避免选择逻辑散落在 click handler、默认路径 replay 和 property changed 分支中。

interaction handler 按 mode 分工：Inline handler 处理视觉树内展开，Default handler 处理 popup 打开、延迟关闭、窗口失焦和同级互斥。键盘导航由 interaction handler 层统一接入，负责 active/focus 漫游、层级进入/返回、Enter 提交和 Esc 关闭当前 popup 分支，不能散落到各个 `NavMenuItem` 的局部 key handler 中。

keyboard navigation coordinator 只拥有临时 active/focus 状态，不拥有选择状态。它可以请求打开或关闭子菜单，但叶子节点提交必须进入 `NavMenuSelectionCoordinator`，以保持 click、默认路径 replay 和键盘提交使用同一个选择入口。

Header 控件只承担显示和局部视觉状态，不拥有选择或打开逻辑。

## 4. 状态与数据流

状态流：

```text
NavMenu public API / NavMenuNode
  Mode / IsInlineCollapsed / InlineCollapsedWidth
  IsDarkStyle / IsItemBackgroundEnabled
  SelectedItem / DefaultSelectedPath / DefaultOpenPaths
  Header / Icon / ItemKey / Children / IsEnabled
  Command / CommandParameter
      ↓
Effective mode + inline collapsed open path cache
      ↓
ItemsControl container generation
      ↓
NavMenuItemContainerBinder
  sync node data and owner menu state
      ↓
NavMenuItem
  Level / IsTopLevel / HasSubMenu / IsSubMenuOpen
  ICommandSource / effective command enabled state
      ↓
Interaction handler + KeyboardNavigationCoordinator + SelectionCoordinator
      ↓
Header theme / Popup frame / Inline child frame
```

`SelectedItem` 是持续选择状态。`DefaultSelectedPath` 和 `DefaultOpenPaths` 只在初始路径应用中参与 replay。程序连续设置多个选择时，过期 replay 必须被忽略，只应用最新 revision。

`IsInlineCollapsed` 是 `Inline` 模式附加状态。进入折叠时，当前 inline 打开路径写入 cache，主视觉树中的 inline 子菜单关闭，effective mode 切为 `Vertical`；退出折叠时，折叠期间打开的 popup 关闭，再从 cache 恢复 inline 打开路径。这个流程不能调用 `NavMenu.Close()`，不能改写 `SelectedItem`。

`InlineCollapsedWidth` 是布局输入。默认值由 `NavMenuTheme.axaml` 通过 `NavMenuToken.InlineCollapsedWidth` 提供；本地属性值覆盖 token 默认值。该属性只影响 `Mode=Inline && IsInlineCollapsed=true` 的根宽度和测量，不影响 `Vertical` / `Horizontal`。宽度约束由 `NavMenu` 内部通过 `Width` / `MinWidth` metadata coercion 表达：折叠时 effective `Width` 收敛到 `InlineCollapsedWidth`，较大的 effective `MinWidth` 向下收敛，展开后恢复原始 base value 或绑定。过渡动画不能挂在 root `Width` 的 `DoubleTransition` 上，因为 `IsInlineCollapsed` 切换时宽度来自 coercion，不是普通 styled value 变化；动画应由内部 `InlineCollapsedLayoutWidth` motion 按帧驱动 coercion。不要使用 `BindingPriority.Animation` relay binding 控制根宽度，也不要设置 `MaxWidth`，否则会破坏用户 base `Width` 或把收缩动画立即夹到目标宽度。

键盘 active/focus 是临时交互状态。active 项变化不能写入 `SelectedItem`，不能触发 `NavMenuNodeSelected`，不能改变 `IsInSelectedPath`。只有 Enter 在叶子节点上提交时，才进入 selection coordinator。

`IsItemBackgroundEnabled` 下发到 `NavMenuItem` 和 header theme，但它只控制 item / submenu 背景块，不关闭 header 文本状态。

## 5. 生命周期与模板接入

`NavMenu` 在 mode 或 inline collapsed 状态变化时同步 root pseudo-class、ItemsPanel 方向、interaction handler、effective mode 和已打开子菜单状态。切换 public `Mode` 必须关闭旧模式下的 popup 或 inline 子菜单，避免旧 handler 的 pointer、delay、popup 或 motion 状态泄漏。切换 `IsInlineCollapsed` 不能走 public mode change 的 `Close()` 路径，而要走 inline collapsed coordinator 的 cache / restore 流程。

`IsInlineCollapsed` 变化时的生命周期顺序必须固定：

```text
collapse:
  collect open inline paths
  close inline child frames without clearing SelectedItem
  switch effective mode to Vertical
  reattach interaction handler if needed
  sync collapsed visual state

expand:
  close transient popup branches
  switch effective mode to Inline
  reattach interaction handler if needed
  replay cached inline paths
  sync expanded visual state
```

`NavMenuItem.OnApplyTemplate` 获取 header、popup、popup frame、inline motion actor、child frame、items presenter 和 active indicator。模板替换时必须解除旧 part 事件订阅，并重新绑定 handler 需要的 part。

默认路径 replay 依赖容器生成和模板应用。实现必须使用有界 replay，不使用固定 sleep 或 timer 作为容器可用性的长期机制。

节点进入容器时必须建立一条完整的 scoped 生命周期：

```text
PrepareContainerForItemOverride
  -> reset current container CompositeDisposable
  -> NavMenuNode.AttachResourceHost(owner)
  -> bind node visual properties
  -> bind Command / CommandParameter to NavMenuItem

ClearContainerForItemOverride / rebind / recycle
  -> dispose the same CompositeDisposable
  -> release node-to-container bindings
  -> release resource-host attachment token
  -> clear command value from the old NavMenuItem
  -> NavMenuItem unsubscribes old Command.CanExecuteChanged
```

资源宿主 attachment、节点属性 binding 和命令 binding 不能分散到不同的无 owner subscription 中。re-template、Items reset、container recycle 或节点替换必须复用同一个 clear 路径；不允许依赖 GC、DataContext 清空或页面导航释放旧关系。

## 6. 交互与事件处理

Inline handler：

- 点击带子菜单项切换 `IsSubMenuOpen`。
- 点击叶子节点进入 selection coordinator。
- `IsAccordionMode=true` 时关闭同层其他打开项。
- Inline 展开收起保持 motion，不通过临时关闭 motion 规避问题。
- 当 `IsInlineCollapsed=true` 时不使用 Inline handler；有效交互切换到 Default handler，使带子菜单的顶层项通过 popup 打开。

Default handler：

- pointer enter 可延迟打开 popup。
- pointer leave 可延迟关闭 popup。
- 点击叶子节点进入 selection coordinator。
- popup close 由 pointer、窗口失焦、非客户端点击和同级打开状态共同控制。
- Horizontal 顶层 popup 放置在下方，非顶层和 vertical popup 使用侧向层级。
- inline collapsed 使用 Default handler 的 popup 路径，但仍保留 public `Mode=Inline`，以保持 API 语义和文档语义一致。

Keyboard navigation：

- NavMenu 在根菜单范围接收方向键、Enter 和 Esc，并根据当前 active/focus 项分派给当前 mode 的导航策略。
- active 项优先来自当前键盘焦点所在的 `NavMenuItem`；没有可用 focus 时，先解析当前 `SelectedItem` 对应的可见容器作为方向键移动锚点，并把本次方向键 delta 应用到该锚点上；找不到选中容器时再使用当前可见层级中的第一个可交互项。
- Up / Down 在当前可见层级内循环移动，跳过禁用项和不可交互项。
- Horizontal 根层使用 Left / Right 在顶层项之间循环移动，Down 或 Enter 打开 active 子菜单并进入第一项。
- Vertical 根层和 popup 子菜单使用 Right 或 Enter 进入子菜单，Left 或 Esc 返回父级并关闭当前 popup 分支。
- Inline 模式使用 Up / Down 遍历当前展开后的可见树；Left / Right 只折叠或展开当前 active 子菜单并保持 active 项不变，叶子项上为 no-op；Enter 在父节点上切换展开，在叶子节点上提交选择。
- Inline collapsed 模式使用 effective vertical 键盘策略，根层只遍历顶层项，打开子菜单后进入 popup 子级。
- 键盘打开 popup 后必须确保子容器可生成，并把 active/focus 移动到第一个可交互子项；不能依赖固定 timer 等待 popup content。
- Esc 只关闭当前 popup 分支，active/focus 回到父项；不能调用 `NavMenu.Close()`，避免清空 `SelectedItem`。

`NavMenuItemClick` 表达 item 点击，`NavMenuNodeSelected` 表达叶子节点选择。禁用项不得触发有效点击、命令或选择。节点命令由同一次 `NavMenuItem` 有效点击或键盘提交执行，不能从 `NavMenuNodeSelected` 再次执行；pointer 与 keyboard 必须复用同一命令入口。

## 7. 内部算法与关键流程

### 7.1 容器绑定

节点数据到容器的绑定必须包括 Header、HeaderTemplate、Icon、ItemKey、IsEnabled、Command、CommandParameter、Children、owner menu、mode、dark style、background mode 和 motion 状态。容器解绑时必须释放资源宿主关系、relay binding 和命令事件订阅。

`NavMenuNode` 作为非 Visual Avalonia binding target 时使用 `[GenerateScopedResourceHost]`。container binder 先把 generated `AttachResourceHost(owner)` token 加入当前 `NavMenuItem` 的 `CompositeDisposable`，再通过 AvaloniaProperty overload 建立节点属性投影。自定义 `INavMenuNode` 使用强类型 getter overload；实现了 `INotifyPropertyChanged` 的节点保持运行期更新，普通节点取得初始值，未声明命令的既有实现使用接口的 `null` 默认值。所有 `BindUtils.RelayBind` 返回值必须加入同一个 disposable；禁止丢弃返回值或把最后一次容器永久挂回节点。

### 7.2 命令投影流程

```text
NavMenuNode.Command / CommandParameter changed
      ↓ container-scoped relay binding
NavMenuItem.Command / CommandParameter
      ↓ ICommandSource
CanExecute determines effective enabled state
      ↓ effective click / keyboard commit
Execute once
```

`NavMenuItem` 在 command property 变化时先解除旧命令的 `CanExecuteChanged`，再订阅新命令；logical-tree detach 时解除当前命令订阅。container disposable 释放 command relay binding 后，旧命令不能继续持有已回收容器。`CanExecute=false` 只影响容器 effective enabled state，不写回 `NavMenuNode.IsEnabled`。

`CommandParameter` 不做 `ItemKey` fallback。自动 fallback 会让显式 `null` 失去语义，并在 `ItemKey`、参数 binding 和容器复用之间引入第二套同步状态。业务需要 key 时由调用方显式绑定或赋值。

### 7.3 选择流程

```text
Select leaf item
      ↓
NavMenuSelectionCoordinator
      ↓
old selected container IsSelected=false
old ancestor IsInSelectedPath=false
new selected container IsSelected=true
new ancestors IsInSelectedPath=true
      ↓
NavMenu.SelectedItem + NavMenuNodeSelected
```

祖先路径只标记导航路径，不应通过 ancestor pointer state 让父级 header 进入 hover 背景。

键盘提交必须复用同一流程。active 项不是选择项，方向键移动不进入 selection coordinator。Enter 提交叶子节点时先触发 item click 语义，再由 selection coordinator 更新选中路径，确保键盘与 pointer click 的事件顺序一致。

### 7.4 默认路径 replay

`TreeNodePath` 通过 `ItemKey` 定位节点路径。路径 replay 先打开中间节点，再选中叶子节点。由于容器生成依赖 layout 和 ItemsPresenter，replay 可以在 loaded priority 下有界重试。

replay 必须具备 revision 控制：新的默认路径或 `SelectedItem` 设置产生新 revision，旧 revision 的异步结果必须丢弃。

### 7.5 背景块模型

`NavMenuItem` 背景和 `NavMenuItemHeader` 背景是两层不同职责：

- item / child frame 背景表达 inline 子菜单背景块和连续层级背景。
- header 背景表达 hover、selected、disabled 等交互视觉。

`IsItemBackgroundEnabled=false` 只关闭 item / child frame 背景块和背景块专用外距。header 前景、hover、selected、selected path 和 disabled 仍由 header theme 处理。

### 7.6 Popup 模型

Popup shell 位于 `NavMenuItem` 模板内，popup content 由 `ItemsPresenter` 承载。打开 popup 前后必须确保子容器可生成，默认路径 replay 不能依赖固定等待时间。

Popup 背景使用 `MenuPopupBg` / `DarkMenuPopupBg`，不能回退成普通 elevated background。

### 7.7 键盘漫游模型

键盘漫游按“层级容器 + 当前 active 项”计算：

```text
KeyDown
      ↓
resolve active item
      ↓
resolve current visible level
      ↓
move sibling / enter child / return parent / commit leaf
      ↓
sync focus + keyboard active visual
```

当前可见层级由根 `NavMenu`、已打开 inline 子树或已打开 popup content 决定。层级内只包含已生成、可见、可交互的 `NavMenuItem` 容器。`ItemsSource` 数据节点不能直接参与键盘导航，必须通过容器定位，避免数据和视觉状态出现两个 owner。

初始化 active 项时只能使用当前已经生成且可交互的容器。`SelectedItem` 可以作为 keyboard active 的初始移动锚点，但第一次方向键必须立即移动到选中项前后相邻节点，不能把 active 停在选中项本身；也不能为了初始化 active 项而强制打开隐藏 popup 或 inline 分支。如果选中节点不可见，导航应回退到第一个可导航节点。

打开子菜单时先设置 `IsSubMenuOpen`，再执行必要的模板和 layout 接入以生成子容器，然后把 active/focus 移入子级第一项。关闭子菜单时先关闭当前 popup 分支，再把 active/focus 放回父级触发项。Inline 模式下关闭子菜单不应清理其子树中的 selection path；selection path 只由 selection coordinator 维护。

keyboard active 视觉通过 header 的内部状态表达，使用 `ItemActiveBg` 语义。该状态优先级低于 selected，高于默认态；它不能复用 `IsSelected` 或 `IsInSelectedPath`，否则会把“浏览候选”和“已提交选择”混为同一个状态。`IsInSelectedPath` 只表达选中路径文字语义，不能屏蔽 keyboard active 背景。

### 7.8 Inline collapsed 模型

Inline collapsed 模型按“public state + effective mode + open path cache”组织：

```text
Mode=Inline + IsInlineCollapsed=false
      ↓
effective mode = Inline
inline child frame owns open visuals

Mode=Inline + IsInlineCollapsed=true
      ↓
effective mode = Vertical
popup owns temporary open visuals
inline open paths live in cache
```

折叠状态切换不应复用 public `Mode` 变化逻辑，因为 public mode change 当前会清理打开状态和选择状态。折叠只是 inline 的紧凑呈现状态，必须保留 `SelectedItem`、selected path 和可恢复的 inline open path。

open path cache 应记录路径语义而不是持有容器引用。容器可能因 ItemsSource reset、template reapply、detached 或 popup 生命周期变化而失效；cache 只保存可重新 replay 的 `TreeNodePath` 或等价节点路径。恢复时复用默认路径 replay 的有界容器生成策略，不能固定 sleep。

折叠期间的 popup 打开状态是临时交互状态。它可以改变 `IsSubMenuOpen` 以驱动 popup，但不得写回 inline open path cache。退出折叠时必须先关闭这些 popup 分支，再恢复 cache，避免 popup 与 inline child frame 同时认为自己拥有同一个分支的打开视觉。

初始加载时如果 `IsInlineCollapsed=true` 且存在 `DefaultOpenPaths`，默认展开路径进入 inline open path cache，不立即展开 inline 子树；首次展开时再 replay cache。`DefaultSelectedPath` / `SelectedItem` 仍可应用 selected leaf 和 selected path，不能因为子树未展开而丢失选中语义。

折叠视觉由 theme 层表达：顶层 header 隐藏标题和箭头，icon 使用 `CollapsedIconSize` 居中；没有 icon 的顶层项显示 header 首字符。root 宽度约束属于控件布局状态，由 C# metadata coercion 表达，以保留用户的 base `Width` / binding。inline collapsed 宽度过渡由内部 `InlineCollapsedLayoutWidth` motion 临时接管 coercion 输入，完成后必须清理回 `double.NaN`，让稳态宽度重新由 `InlineCollapsedWidth` 或用户原始 `Width` / binding 决定。C# 层不应为了折叠视觉改写 `Header` 或临时替换 `HeaderTemplate`。

## 8. 资源、性能与 AOT 边界

NavMenu 不应通过反射访问 template part 或内部状态。Header、popup、inline child frame 和 active indicator 均通过稳定 template part 和 Avalonia 属性接入。

`NavMenuNode` 必须通过 `[GenerateScopedResourceHost]` 生成 `IResourceHost` / `IThemeVariantHost`、attachment count、host generation 和 `IDisposable` attach token。generation 用于使跨 host 切换后遗留的 stale token 失效，尤其不能让 `A -> B -> A` 中第一轮 A token 释放当前 A attachment。scoped resource host 只解决动态资源宿主及其事件订阅，不替代节点到容器 binding 的释放，也不替代 `ICommand.CanExecuteChanged` 的解绑。

命令能力的完整释放边界由三层共同组成：

1. generated scoped resource host 释放 `ResourcesChanged` / `ActualThemeVariantChanged` owner 订阅；
2. container `CompositeDisposable` 释放节点到 `NavMenuItem` 的 `Command`、`CommandParameter` 和其他属性 binding；
3. `NavMenuItem` 在 command replacement 与 logical-tree detach 时解除 `CanExecuteChanged`。

缺少任意一层都不能宣称节点命令生命周期完整。不得通过弱化动态资源、改为静态值、永久 owner 引用、全局 command cache 或延迟清理规避释放问题。

`CanExecuteChanged` 可能在一次同步命令执行中快速发出 `false -> true`，共享同一命令的多个叶子容器如果逐次立即更新 effective enabled，会同时启动 disabled 前景色过渡并产生闪动。`NavMenuItem` 将通知统一 marshal 到 UI Dispatcher，并以 `Input` 优先级合并同一 UI 周期内的重复通知；回调只重新读取一次当前 `Command.CanExecute(CommandParameter)`，因此持续 `false` 仍会在下一轮交互前生效，而瞬时变化不会暴露中间视觉状态。待处理 operation 由当前 container 持有，并在 command replacement、parameter replacement 和 logical-tree detach 时 abort，不能让已回收容器被 dispatcher callback 延迟持有或被旧 command 状态回写。

handler 持有事件订阅时必须在 mode 切换、detached 或模板替换时释放。延迟打开 / 关闭任务必须支持取消，避免旧 pointer 状态影响新 mode 或新 popup。

inline collapsed cache 不得持有 `NavMenuItem`、header、popup 或 template part 引用。状态失效边界包括 ItemsSource reset、container clear、detach、mode change 和 default path replay revision 变化。

键盘导航状态持有的 active item 引用必须随 detach、mode 切换、container clear、popup close 和 item disabled 变化失效。失效时应重新从当前可见层级解析 active 项，不保留悬空容器引用。

默认路径 replay 必须有界，避免容器生成失败时形成无休止 dispatcher 队列。

## 9. 维护不变量

内部重构必须保持以下不变量：

- mode 切换时重新挂接 handler，并清理旧模式打开状态。
- `IsInlineCollapsed` 切换不得改写 public `Mode`，不得调用 `Close()`，不得清空 `SelectedItem`。
- inline collapsed 进入时缓存 inline open path，退出时恢复 cache；折叠期间 popup 打开状态不得污染 cache。
- `InlineCollapsedWidth` 默认来自 `NavMenuToken.InlineCollapsedWidth`，本地属性值必须按 Avalonia 优先级覆盖 token 默认值。
- 折叠视觉不能通过改写 `Header`、删除 `HeaderTemplate` 或动态创建替代 header 实现。
- 点击 item 不得临时关闭 motion。
- 默认路径应用不使用固定 50ms sleep 作为稳定策略。
- selection coordinator 是选择状态的统一入口。
- keyboard active/focus 状态不能替代 selection coordinator。
- 方向键移动不得触发点击或选中事件。
- keyboard active 初始解析可以复用可见 `SelectedItem` 容器作为方向键移动锚点，但不得吃掉第一次方向键、触发选择事件或自动打开隐藏分支。
- Esc 关闭 popup 分支不得调用 `NavMenu.Close()`，不得清空 `SelectedItem`。
- 键盘打开 popup 或 inline 子项不得依赖固定 timer 生成容器。
- header hover / selected 背景不通过父级 item hover 状态误触发。
- `IsItemBackgroundEnabled=false` 不关闭 header 颜色和交互状态。
- popup、root、inline child frame、header 四类背景职责保持分离。
- handler 取消逻辑不能泄漏事件订阅或延迟任务。
- `NavMenuNode` 不实现 `ICommandSource`，不直接执行命令或订阅 `CanExecuteChanged`。
- scoped resource-host attachment、node relay binding 和 command subscription 必须各自具有确定释放点。
- `CanExecuteChanged` 的合并 operation 必须由当前 container 持有，并在 command / parameter 替换和 logical-tree detach 时取消。
- container rebind、clear、recycle、Items reset 和 re-template 后，旧节点、旧命令和旧 owner 不得继续持有当前容器。
- `CommandParameter` 不隐式使用 `ItemKey`，避免显式 `null` 和容器同步语义分叉。

## 10. 测试与验证

验证范围：

- Inline、Vertical、Horizontal 打开、关闭、hover、click 和同级互斥。
- Inline collapsed 切换、effective mode、open path cache、初始 `DefaultOpenPaths` 缓存、展开恢复、popup 临时打开和 selected path 保持。
- Inline、Vertical、Horizontal 的 Up / Down / Left / Right / Enter / Esc 键盘漫游、层级进入/返回、leaf commit 和 popup close。
- 方向键移动 active 项不触发 `NavMenuItemClick` / `NavMenuNodeSelected`。
- Esc 关闭当前 popup 分支但不清空 `SelectedItem`。
- 禁用项、隐藏项和未展开子项不进入键盘漫游序列。
- `SelectedItem`、`DefaultSelectedPath`、`DefaultOpenPaths`、stale replay 和 clear selection。
- 点击子节点时父级 header 不出现错误 hover 背景。
- `IsItemBackgroundEnabled=true/false` 下 inline 背景块、header 背景和间距分别正确。
- Dark root、popup、submenu、header、selected 和 hover 颜色与 Token 语义一致。
- Popup 打开、关闭、失焦、pointer leave 和 mode 切换后无旧状态残留。
- Motion 不因点击、打开或关闭流程被临时禁用。
- 节点 `Command` / `CommandParameter` 在 pointer 和 keyboard 提交时只执行一次，参数保持显式值。
- `CanExecute=false` 正确影响 effective disabled，命令变化后旧 `CanExecuteChanged` 订阅被解除。
- 同步 `ReactiveCommand` 的瞬时 `false -> true` 不得让当前叶子、共享命令叶子或 selected path 祖先暴露中间 disabled 视觉。
- container rebind、clear、recycle、Items reset、re-template 和页面卸载后，旧节点、旧命令和 ViewModel 可被回收。
- scoped resource host 覆盖 DynamicResource WeakReference、owner resource 优先、resource update、repeated attach 和 attach token release。
- 文档改动运行 `git diff --check`。
