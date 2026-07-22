# Window 桌面版实现原理

本文档描述 Window 桌面版的内部实现范围、源码职责、状态流、生命周期、资源边界和维护规则。公共设计与 API 契约见 [Window 桌面版架构设计](overview.md)，Window 与标题栏的对齐协作见 [WindowTitleBar 实现原理](../window-title-bar/implementation.md)，变化记录见 [Window Changelog](changelog.md)。涉及组件 Token 的实现应同时阅读 [Window Token 设计](token.md)。

## 1. 实现定位

本文档覆盖 Window 的控件实现、主题接入、状态同步和 Gallery 可见维护边界。具体属性注册、默认值、绘制细节和 AXAML selector 仍应直接阅读源码；本文只记录维护者必须理解的稳定结构和不变量。

## 2. 源码文件结构

主要源码文件：

- `src/AtomUI.Desktop.Controls/Window/FullscreenPopoverLayer.cs`
- `src/AtomUI.Desktop.Controls/Window/LinuxWindowChromeManager.cs`
- `src/AtomUI.Desktop.Controls/Window/MacStandardWindowButtons.cs`
- `src/AtomUI.Desktop.Controls/Window/MediaBreakPointThemeBootstrapper.cs`
- `src/AtomUI.Desktop.Controls/Window/ReactiveWindow.cs`
- `src/AtomUI.Desktop.Controls/Window/Themes/FullscreenPopoverLayerTheme.axaml`
- `src/AtomUI.Desktop.Controls/Window/Themes/WindowDrawnDecorationsTheme.axaml`
- `src/AtomUI.Desktop.Controls/Window/Themes/WindowResizerTheme.axaml`
- `src/AtomUI.Desktop.Controls/Window/Themes/WindowTheme.axaml`
- `src/AtomUI.Desktop.Controls/Window/Themes/WindowTheme.cs`
- `src/AtomUI.Desktop.Controls/Window/Themes/WindowThemes.axaml`
- `src/AtomUI.Desktop.Controls/Window/Window.cs`
- `src/AtomUI.Desktop.Controls/Window/WindowChromeManager.cs`
- `src/AtomUI.Desktop.Controls/Window/WindowResizer.cs`
- `src/AtomUI.Desktop.Controls/Window/WindowToken.cs`

职责边界：

- 控件主文件保留 public/protected API、Avalonia 属性注册、事件和主要生命周期入口。
- Theme 文件负责静态视觉结构、template part、selector 和资源绑定。
- Token 文件只提供组件视觉变量，不保存实例状态。
- Gallery 文件只展示用法、API 表和 Token 表，不作为运行时逻辑 owner。

## 3. 核心类职责

- `FullscreenPopoverLayer`：控件核心或内部协作类型，维护 public surface 与主题可观察行为。
- `MacStandardWindowButtons`：控件核心或内部协作类型，维护 public surface 与主题可观察行为。
- `ReactiveWindow`：控件核心或内部协作类型，维护 public surface 与主题可观察行为。
- `Window`：控件核心或内部协作类型，维护 public surface 与主题可观察行为。
- `WindowResizer`：控件核心或内部协作类型，维护 public surface 与主题可观察行为。
- `WindowTheme`：ControlTheme 类型入口，连接主题资源和控件类型。
- `WindowToken`：组件 Token scope，负责从全局 token 派生控件语义变量。

核心协作规则：

- 控件实例是 public API 和运行时状态 owner。
- Template part 是视觉协作对象，生命周期必须受 `OnApplyTemplate` 或模板加载流程管理。
- 数据对象、选项对象、任务对象或节点对象只保存业务数据，不应反向持有不可释放的视觉对象。
- 弹层、窗口、计时器、异步 loader 和全局管理器必须有明确关闭、解绑或释放路径。

## 4. 状态与数据流

Window 的状态流遵循下面路径：

```text
Public API / ItemsSource / Command / Event
  -> 控件实例状态
  -> internal state / effective state / pseudo-class
  -> template part property / AXAML selector
  -> renderer / popup / adorner / Gallery observable behavior
```

源码中的状态入口按以下语义维护：

- 内容与数据：`ContentFrameBackground`、`ContentFrameLayer`、`ContentFrameLayerOpacity`、`ContentFrameLayerTemplate`、`IsTitleBarVisible`、`LogoTemplate`、`TitleBarFrameBackground`、`TitleBarFrameLayer`、`TitleBarFrameLayerOpacity`、`TitleBarFrameLayerTemplate` 等 11 项。
- 选择与集合：`ViewModel`。
- 交互与状态：`IsCloseCaptionButtonVisible`、`IsFullScreenCaptionButtonVisible`、`IsMoveEnabled`、`IsPinCaptionButtonVisible`。
- 弹层与窗口：`WindowFrameLayer`、`WindowFrameLayerOpacity`。
- 其他稳定入口：`Logo`、`LogoVisibility`、`MediaBreakPoint`、`OsType`、`OsVersion`。

维护要求：

- 外部设置的 Avalonia 属性必须在模板应用前后保持一致。
- 集合、选择、展开、过滤、分页、上传任务或异步 loader 必须能处理 reset、replace 和 clear。
- 伪类和 internal state 必须从单一 owner 推导，避免双向同步导致循环更新。
- Gallery API 表中的状态说明应与源码实际状态流一致。
- `TitleBarFrameLayer` 的数据流终点是标题栏背景/装饰层；它不作为普通 Avalonia 交互控件入口，标题栏按钮、菜单、搜索框等应通过 `TitleBar` 承载。

## 5. 生命周期与模板接入

生命周期规则：

- 构造阶段只注册必要状态，不依赖 template part。
- 模板应用时获取 part、建立事件订阅和绑定，并先释放旧 part 订阅。
- 控件卸载、弹层关闭、窗口关闭、集合替换或 container recycle 时释放事件订阅和资源宿主。
- DynamicResource、TokenResourceBinder 或 C# binding 必须有明确 owner 和释放点。
- Browser 和 Desktop 宿主下的主题加载顺序不得影响 public API 语义。

稳定 template part 接入点：

- `PART_CloseButton`：承载用户触发入口、导航或关闭动作。
- `PART_ContentPresenter`：展示用户内容、文本、图标或模板化数据。
- `PART_FullScreenButton`：承载用户触发入口、导航或关闭动作。
- `PART_FullscreenPopoverLayer`：稳定模板协作入口，重命名前必须同步主题和实现。
- `PART_OverlayWrapper`：稳定模板协作入口，重命名前必须同步主题和实现。
- `PART_PopoverBorder`：承载根视觉、边框、背景或尺寸基线。
- `PART_PopoverCloseButton`：承载用户触发入口、导航或关闭动作。
- `PART_PopoverFullScreenButton`：承载用户触发入口、导航或关闭动作。
- `PART_RootLayout`：承载根视觉、边框、背景或尺寸基线。
- `PART_TitleBar`：稳定模板协作入口，重命名前必须同步主题和实现。
- `PART_TitleBarPresenter`：展示用户内容、文本、图标或模板化数据。
- `PART_TransparencyFallback`：稳定模板协作入口，重命名前必须同步主题和实现。
- `PART_UnderlayWrapper`：稳定模板协作入口，重命名前必须同步主题和实现。
- `PART_VisualLayerManager`：稳定模板协作入口，重命名前必须同步主题和实现。
- `PART_WindowFrame`：承载根视觉、边框、背景或尺寸基线。
- `PART_WindowResizer`：稳定模板协作入口，重命名前必须同步主题和实现。

## 6. 交互与事件处理

Window 的交互事件应从输入源收敛到控件级语义事件：

- Pointer、keyboard、focus 和 command 事件不应绕过 Avalonia 基础控件语义。
- 弹层、窗口或 overlay 类路径必须稳定处理打开、关闭、取消、重复打开和宿主失活。
- 非集合控件不应通过隐藏集合状态模拟业务数据。
- 值提交或命令触发必须保持继承控件的事件顺序。

当前没有抽取到控件专属 public 事件；交互语义主要通过继承事件、命令、属性变化和 Gallery 可观察行为体现。

### 6.1 标题栏背景层、TitleBar 与 CSD 命中模型

Window 的标题栏存在两套输入模型，维护时必须同时成立：

- 非 CSD 自绘模板使用普通 Avalonia hit test。`TitleBarFrameLayer` 是默认 `TitleBar` 下方的背景/装饰层，不应承担用户输入。
- Avalonia `WindowDrawnDecorations` CSD 模板使用 `WindowDecorationProperties.ElementRole` 参与平台 chrome hit test。`ElementRole="TitleBar"` 表示拖拽区域；`TitleBarFrameLayer` 属于背景/装饰语义，可以随 `PART_TitleBar` 进入该 role。
- CSD 路径中承载 `TitleBar` 的 `PART_TitleBarPresenter` 使用 `ElementRole="User"` 或等价 client input 语义；用户需要标题栏按钮、菜单、输入框时，应通过自定义 `TitleBar` 进入该路径。
- 默认 `TitleBar`、caption buttons、全屏弹出层和背景/装饰层职责不能混用：caption buttons 保持自身窗口操作 role，默认或自定义 `TitleBar` 表达标题栏交互，`TitleBarFrameLayer` 只表达背景、遮罩或装饰视觉。
- 不允许通过捕获异常、转发单个按钮 `Click`、延迟重新命中或给特定 Demo 写特殊判断来让 `TitleBarFrameLayer` 支持交互；这会模糊背景层和标题栏交互层的职责。

目标结构按以下模型维护：

| 路径 | 背景/装饰层 | 拖拽 / 默认标题栏层 | 用户交互层 |
| --- | --- | --- | --- |
| 非 CSD `WindowTheme.axaml` | `TitleBarFrameLayer` / `TitleBarFrameBackground` 位于默认 `TitleBar` 下方，表达背景和装饰。 | `TitleBar` / `WindowTitleBar` 负责标题、Logo、caption buttons 和空白区域拖拽。 | 自定义 `TitleBar` 内部控件负责按钮、菜单、搜索框等交互。 |
| CSD `WindowDrawnDecorationsTheme.axaml` | `PART_TitleBar` 可承载 `TitleBarFrameLayer`，并使用 `ElementRole="TitleBar"` 表达标题栏拖拽区域。 | 默认 `TitleBar` 的展示由 overlay presenter 承载，平台 chrome role 不应吞掉该 presenter 内部交互。 | `PART_TitleBarPresenter` 承载 `TitleBar`，使用 `ElementRole="User"`；用户自定义标题栏控件在此获得 client input。 |

实现时必须避免以下错误结构：

- 把按钮、菜单、搜索框等交互控件放入 `TitleBarFrameLayer` 或 `TitleBarFrameLayerTemplate`。
- 为了让 `TitleBarFrameLayer` 内部按钮可点，把背景/装饰层提升到 `TitleBar` 上方或改成 `ElementRole="User"`。
- 给 `TitleBarFrameLayer` 的根节点设置可绘制透明背景后尝试承载整条标题栏交互；这会抢占拖拽区域并混淆职责。
- 为了让某个按钮可点，在 code-behind 中手动转发 pointer 或 click；这会绕过 Avalonia 原生输入、焦点和命令语义。

## 7. 内部算法与关键流程

维护者需要重点关注以下流程：

- API 默认值到 effective state 的归一。
- Template part 重新应用时的状态回放。
- 主题资源、Token 和 SharedToken 计算后的视觉更新。
- 内容、命令和视觉状态在模板节点之间的同步。
- 标题栏背景/装饰层、自定义 `TitleBar` 与 Avalonia CSD chrome hit test 的职责划分。
- 完整 layer、visible frame 和 content bounds 的职责划分；`WindowVisualLayerClip.CalculateClipBounds` 是排除 client-drawn frame shadow 的共享计算入口。
- 状态变化时避免创建不必要的视觉对象、订阅或动画对象。

实现文档不逐行解释私有方法。若某个私有算法成为稳定维护入口，应在本节补充算法不变量，而不是把代码复述为说明书。

### 7.1 首次显示主题表面准备

`Window` 的所有 AtomUI `Show` / `ShowDialog` 入口共用一条平台无关的准备路径：

```text
Resolve owner ThemeContext
  -> attach ThemeContextLease and publish RequestedThemeVariant
  -> synchronously resolve WindowToken.DefaultBackground from that scoped Snapshot
  -> set temporary Background and TransparencyBackgroundFallback values at Template priority
  -> prepare platform chrome geometry
  -> call Avalonia base.Show / base.ShowDialog
  -> release temporary surface values
  -> keep ThemeContextLease until close; WindowTheme owns subsequent theme updates
```

实现必须满足以下约束：

- 资源读取发生在 `ThemeContextLease` 的资源桥挂载之后，确保 owner 局部主题和 Window control token override 优先于根主题。
- 显示调用是同步临界区，首帧背景只做一次 Snapshot-backed 资源读取和临时属性赋值，不创建 `DynamicResource`、resource observable 或短生命周期 Token 订阅。
- 临时属性值使用 `BindingPriority.Template` 或等价的低优先级可释放值帧，因此不得覆盖用户 local value；释放时也不得影响用户值。
- `base.ShowDialog` 返回窗口生命周期 `Task` 后即可释放首帧临时值，不得让临时值存活到 Dialog 关闭。
- `base.Show` / `base.ShowDialog` 抛出异常时，释放本次新建的临时值和 ThemeContext lease，不保留资源桥、事件订阅或错误 owner。
- 不通过提前应用整套 ControlTheme 改变 `WindowOpenedEvent` 前所有 Window Setter 的可观察时序；共享准备阶段只处理平台可见前不可缺少的 theme context、variant 和 surface background。
- Windows、macOS、X11 和 Wayland chrome manager 不参与 Token 解析。只有在实机证据证明 managed 首帧已经正确但特定后端仍显示原生空白 surface 时，才允许在统一接口后增加平台后备。

回归验证至少覆盖：根 Dark 主题在 `WindowOpenedEvent` 前的背景、owner 局部 ThemeContext、用户显式背景不被覆盖、关闭后 lease/资源桥释放，以及失败显示的回滚。Headless 测试只能证明 managed 状态顺序；Windows、macOS 和 Linux 的最终首帧必须通过各平台实机显示或录屏验证。

## 8. 资源、性能与 AOT 边界

资源和 AOT 约束：

- 不通过运行时反射扫描 public API、Token 或 Gallery 表格数据。
- 不把可静态声明的模板结构迁移到 C# 动态创建。
- 异步加载、上传、弹层和窗口生命周期必须能取消或释放。
- 缓存对象必须与控件、窗口、弹层或数据 owner 生命周期一致。
- Source generator 生成文件不手工编辑；需要修改时改输入源或 generator。

性能边界：

- 控件应优先复用 Avalonia 原生虚拟化、模板绑定和资源系统。
- 避免为每次状态变化创建不必要的视觉对象、订阅或动画对象。
- 首次显示主题表面使用一次性 Snapshot 读取，不为同步 `Show` 临界区创建资源 observable 或订阅。
- 大集合控件必须保证 container recycle 后不会泄漏旧 item 状态。

## 9. 维护不变量

维护 Window 时不得破坏：

- Public API、默认值、事件顺序和 Gallery 可观察行为。
- Template part 名称、ControlTheme key、伪类和资源 key。
- `TitleBarFrameLayer` 的背景/装饰层语义，以及标题栏交互内容必须通过 `TitleBar` 承载的职责边界。
- 上层 Dialog/Drawer 不按 OS 或 CSD 状态复制 Window frame 几何，而是消费 Window 发布的 `FrameShadowThickness` 和实际 drawn host 能力。
- 所有桌面平台共用 Window 首次显示主题表面准备流程，`WindowTheme` 是显示完成后的唯一长期背景所有者。
- 旧 template part、事件订阅、Popup/Flyout/Window host 和 collection view 的释放路径。
- Light/Dark、Browser/Desktop 和不同 SizeType 下的主题一致性。
- 文档、Gallery API 表、Token 表与源码契约的一致性。

## 10. 测试与验证

推荐验证：

- 纯文档改动运行 `git diff --check` 并检查相对链接。
- 控件 API 或行为变更运行对应 `tests/AtomUI.Desktop.Controls.Tests` 或专用包测试。
- DataGrid 相关变更运行 `tests/AtomUI.Desktop.Controls.DataGrid.Tests`。
- Gallery 示例、API 表或 Token 表变更运行 `tests/AtomUIGallery.Tests`。
- AOT、生成器或动态数据路径变更按 Gallery NativeAOT 发布流程验证。
