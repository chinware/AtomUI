# WindowTitleBar 语义结构

> 生成产物：由源文档生成，不要手工编辑。修改内容请回到控件文档、源码 public surface、Token 类型或生成数据、Gallery ShowCase 或源码结构。

## Semantic Parts

| Part | AtomUI 节点 | 职责 | 相关 API | 相关 Token | 稳定性 |
| --- | --- | --- | --- | --- | --- |
| `root` | `WindowTitleBar` | 承载公共内容契约、平台状态和标题栏主题入口。 | `Logo`、`Title`、`TitleAlignment` | `Height`、`TitleBarPadding`、标题字体与颜色 | public |
| `frame` | `Border#Frame` | 绘制标题栏背景并定义完整可见 frame。 | `Background`、`Padding` | `Height`、`TitleBarPadding` | template-stable |
| `leading` | Windows/Linux: `PART_Logo` + `PART_LeftAddOn`；macOS: `PART_LeftAddOn` | 承载起始侧应用操作并占用标题安全空间。Windows/Linux 中 Logo 是物理最左内容。 | `Logo`、`LogoTemplate`、`LogoVisibility`、`LeftAddOn`、`LeftAddOnTemplate` | `LogoSize`、`HeaderHorizontalSpacing` | template-stable |
| `title` | Windows/Linux: `PART_ContentPresenter`；macOS: `PART_Logo` + `PART_ContentPresenter` | 展示、测量、对齐和裁剪标题内容；macOS 同时保留 Logo/Title 连续标题组。 | `Logo`、`LogoTemplate`、`LogoVisibility`、`Title`、`TitleTemplate` | `LogoAndTitleSpacing`、标题字体与颜色 | template-stable |
| `trailing` | `PART_RightAddOn` + `PART_CaptionButtonGroup` | 承载结束侧应用操作和 managed window operations。 | `RightAddOn`、`RightAddOnTemplate`；五个 Window caption visibility 属性 | `HeaderHorizontalSpacing`、caption button 尺寸、间距与状态颜色 | template-stable |
| `native-chrome` | 平台原生窗口按钮安全区 | 以逻辑像素 inset 约束标题安全空间，不进入 visual tree。 | 平台、CSD、WindowState | 不适用 | internal-observable |

## Abstract AXAML Structure

来源：`src/AtomUI.Desktop.Controls/WindowTitleBar/Themes/WindowTitleBarTheme.axaml`

```xml
<Border Name="Frame">
    <WindowTitleBarLayoutPanel>
        <DockPanel>
            <ContentPresenter Name="PART_Logo" />
            <ContentPresenter Name="PART_LeftAddOn" />
        </DockPanel>
        <DockPanel>
            <ContentPresenter Name="PART_ContentPresenter" />
        </DockPanel>
        <StackPanel>
            <ContentPresenter Name="PART_RightAddOn" />
            <CaptionButtonGroup Name="PART_CaptionButtonGroup" />
        </StackPanel>
    </WindowTitleBarLayoutPanel>
</Border>
```

## Composition Model

该章节由控件 `Themes/` 文件夹中的真实主题文件生成，用于说明 public 控件与内部协作对象之间的运行时结构。内部节点只用于理解和维护，不应指导用户代码直接依赖。

### 控件角色图

```text
WindowTitleBar
  -> CaptionButtonGroup (control theme, CaptionButtonGroupTheme.axaml)
     -> StackPanel#RootLayout (template-stable)
        -> CaptionButton#PART_FullScreenButton (template-stable)
        -> CaptionButton#PART_PinButton (template-stable)
        -> CaptionButton#PART_MinimizeButton (template-stable)
        -> CaptionButton#PART_MaximizeButton (template-stable)
        -> CaptionButton#PART_CloseButton (template-stable)
     -> StackPanel#RootLayout (template-stable)
        -> CaptionButton#PART_PinButton (template-stable)
     -> StackPanel#RootLayout (template-stable)
        -> WindowsCaptionButton#PART_FullScreenButton (template-stable)
        -> WindowsCaptionButton#PART_PinButton (template-stable)
        -> WindowsCaptionButton#PART_MinimizeButton (template-stable)
        -> WindowsCaptionButton#PART_MaximizeButton (template-stable)
        -> WindowsCaptionButton#PART_CloseButton (template-stable)
  -> CaptionButton (control theme, CaptionButtonTheme.axaml)
     -> Panel (template-stable)
        -> Border#PART_Frame (template-stable)
        -> Border (template-stable)
           -> IconPresenter#PART_IconPresenter (template-stable)
  -> WindowTitleBar (control theme, WindowTitleBarTheme.axaml)
     -> Border#Frame (template-stable)
        -> WindowTitleBarLayoutPanel (internal-observable)
           -> DockPanel (template-stable)
              -> ContentPresenter#PART_Logo (template-stable)
              -> ContentPresenter#PART_LeftAddOn (template-stable)
           -> DockPanel (template-stable)
              -> ContentPresenter#PART_ContentPresenter (template-stable)
           -> StackPanel (template-stable)
              -> ContentPresenter#PART_RightAddOn (template-stable)
              -> CaptionButtonGroup#PART_CaptionButtonGroup (template-stable)
     -> Border#Frame (template-stable)
        -> WindowTitleBarLayoutPanel (internal-observable)
           -> DockPanel (template-stable)
              -> ContentPresenter#PART_Logo (template-stable)
              -> ContentPresenter#PART_LeftAddOn (template-stable)
           -> DockPanel (template-stable)
              -> ContentPresenter#PART_ContentPresenter (template-stable)
           -> StackPanel (template-stable)
              -> ContentPresenter#PART_RightAddOn (template-stable)
              -> CaptionButtonGroup#PART_CaptionButtonGroup (template-stable)
     -> Border#Frame (template-stable)
        -> WindowTitleBarLayoutPanel (internal-observable)
           -> ContentPresenter#PART_LeftAddOn (template-stable)
           -> DockPanel (template-stable)
              -> ContentPresenter#PART_Logo (template-stable)
              -> ContentPresenter#PART_ContentPresenter (template-stable)
           -> StackPanel (template-stable)
              -> ContentPresenter#PART_RightAddOn (template-stable)
              -> CaptionButtonGroup#PART_CaptionButtonGroup (template-stable)
  -> WindowsCaptionButton (control theme, WindowsCaptionButtonTheme.axaml)
     -> Border#PART_Frame (template-stable)
        -> IconPresenter#PART_IconPresenter (template-stable)
```

### 协作节点

| 节点 | 类型 | 来源 | 生命周期 owner | 影响的 public API | 稳定性 | Agent 使用边界 |
| --- | --- | --- | --- | --- | --- | --- |
| `WindowTitleBar` | public control | `源文档 + public API` | 用户代码 / 控件宿主 | public API | public | 用户可直接使用 public 控件；可作为示例和 API 入口。 |
| `CaptionButtonGroup` | control theme | `CaptionButtonGroupTheme.axaml` | WindowTitleBar | `CaptionButtonCommand`, `HostWindowState`, `IsCloseButtonEffectivelyVisible`, `IsFullScreenButtonEffectivelyVisible`, `IsMaximizeButtonEffectivelyVisible`, `IsMinimizeButtonEffectivelyVisible` | internal-observable | 用于理解结构和状态流，不应指导用户代码直接依赖。 |
| `RootLayout` | template node (StackPanel) | `CaptionButtonGroupTheme.axaml` | CaptionButtonGroup | `CaptionButtonCommand`, `IsCloseButtonEffectivelyVisible`, `IsFullScreenButtonEffectivelyVisible`, `IsMaximizeButtonEffectivelyVisible`, `IsMinimizeButtonEffectivelyVisible`, `IsMotionEnabled` | template-stable | 用于主题维护；变更需同步主题、实现和 LLMS。 |
| `PART_FullScreenButton` | template node (CaptionButton) | `CaptionButtonGroupTheme.axaml` | CaptionButtonGroup | `CaptionButtonCommand`, `IsFullScreenButtonEffectivelyVisible`, `IsMotionEnabled`, `IsWindowActive`, `IsWindowFullScreen` | template-stable | 用于主题维护；变更需同步主题、实现和 LLMS。 |
| `PART_PinButton` | template node (CaptionButton) | `CaptionButtonGroupTheme.axaml` | CaptionButtonGroup | `CaptionButtonCommand`, `IsMotionEnabled`, `IsPinButtonEffectivelyVisible`, `IsWindowActive`, `IsWindowPinned` | template-stable | 用于主题维护；变更需同步主题、实现和 LLMS。 |
| `PART_MinimizeButton` | template node (CaptionButton) | `CaptionButtonGroupTheme.axaml` | CaptionButtonGroup | `CaptionButtonCommand`, `IsMinimizeButtonEffectivelyVisible`, `IsMotionEnabled`, `IsWindowActive` | template-stable | 用于主题维护；变更需同步主题、实现和 LLMS。 |
| `PART_MaximizeButton` | template node (CaptionButton) | `CaptionButtonGroupTheme.axaml` | CaptionButtonGroup | `CaptionButtonCommand`, `IsMaximizeButtonEffectivelyVisible`, `IsMotionEnabled`, `IsWindowActive`, `IsWindowMaximized` | template-stable | 用于主题维护；变更需同步主题、实现和 LLMS。 |
| `PART_CloseButton` | template node (CaptionButton) | `CaptionButtonGroupTheme.axaml` | CaptionButtonGroup | `CaptionButtonCommand`, `IsCloseButtonEffectivelyVisible`, `IsMotionEnabled`, `IsWindowActive` | template-stable | 用于主题维护；变更需同步主题、实现和 LLMS。 |
| `PART_FullScreenButton` | template node (WindowsCaptionButton) | `CaptionButtonGroupTheme.axaml` | CaptionButtonGroup | `CaptionButtonCommand`, `HostWindowState`, `IsFullScreenButtonEffectivelyVisible`, `IsMotionEnabled`, `IsWindowActive`, `IsWindowFullScreen` | template-stable | 用于主题维护；变更需同步主题、实现和 LLMS。 |
| `PART_PinButton` | template node (WindowsCaptionButton) | `CaptionButtonGroupTheme.axaml` | CaptionButtonGroup | `CaptionButtonCommand`, `HostWindowState`, `IsMotionEnabled`, `IsPinButtonEffectivelyVisible`, `IsWindowActive`, `IsWindowPinned` | template-stable | 用于主题维护；变更需同步主题、实现和 LLMS。 |
| `PART_MinimizeButton` | template node (WindowsCaptionButton) | `CaptionButtonGroupTheme.axaml` | CaptionButtonGroup | `CaptionButtonCommand`, `HostWindowState`, `IsMinimizeButtonEffectivelyVisible`, `IsMotionEnabled`, `IsWindowActive` | template-stable | 用于主题维护；变更需同步主题、实现和 LLMS。 |
| `PART_MaximizeButton` | template node (WindowsCaptionButton) | `CaptionButtonGroupTheme.axaml` | CaptionButtonGroup | `CaptionButtonCommand`, `HostWindowState`, `IsMaximizeButtonEffectivelyVisible`, `IsMotionEnabled`, `IsWindowActive`, `IsWindowMaximized` | template-stable | 用于主题维护；变更需同步主题、实现和 LLMS。 |
| `PART_CloseButton` | template node (WindowsCaptionButton) | `CaptionButtonGroupTheme.axaml` | CaptionButtonGroup | `CaptionButtonCommand`, `HostWindowState`, `IsCloseButtonEffectivelyVisible`, `IsMotionEnabled`, `IsWindowActive` | template-stable | 用于主题维护；变更需同步主题、实现和 LLMS。 |
| `CaptionButton` | control theme | `CaptionButtonTheme.axaml` | WindowTitleBar | `Background`, `BackgroundInset`, `EffectiveCornerRadius`, `EffectiveIcon`, `HorizontalAlignment`, `IconHeight` | internal-observable | 用于理解结构和状态流，不应指导用户代码直接依赖。 |
| `Panel` | template node (Panel) | `CaptionButtonTheme.axaml` | CaptionButton | `Background`, `BackgroundInset`, `EffectiveCornerRadius`, `EffectiveIcon`, `HorizontalAlignment`, `IconHeight` | template-stable | 用于主题维护；变更需同步主题、实现和 LLMS。 |
| `PART_Frame` | template node (Border) | `CaptionButtonTheme.axaml` | CaptionButton | `Background`, `BackgroundInset`, `EffectiveCornerRadius` | template-stable | 用于主题维护；变更需同步主题、实现和 LLMS。 |
| `PART_IconPresenter` | template node (IconPresenter) | `CaptionButtonTheme.axaml` | CaptionButton | `EffectiveIcon`, `IconHeight`, `IconWidth` | template-stable | 用于主题维护；变更需同步主题、实现和 LLMS。 |
| `WindowTitleBar` | control theme | `WindowTitleBarTheme.axaml` | 用户代码 / 控件宿主 | `Background`, `CanMaximize`, `CanMinimize`, `CaptionButtonCommand`, `HostWindowState`, `IsCloseCaptionButtonVisible` | public | 用户可直接使用 public 控件；可作为示例和 API 入口。 |
| `Frame` | template node (Border) | `WindowTitleBarTheme.axaml` | WindowTitleBar | `Background`, `CanMaximize`, `CanMinimize`, `CaptionButtonCommand`, `HostWindowState`, `IsCloseCaptionButtonVisible` | template-stable | 用于主题维护；变更需同步主题、实现和 LLMS。 |
| `WindowTitleBarLayoutPanel` | template node (WindowTitleBarLayoutPanel) | `WindowTitleBarTheme.axaml` | WindowTitleBar | `CanMaximize`, `CanMinimize`, `CaptionButtonCommand`, `HostWindowState`, `IsCloseCaptionButtonVisible`, `IsCsdEnabled` | internal-observable | 用于理解结构和状态流，不应指导用户代码直接依赖。 |
| `DockPanel` | template node (DockPanel) | `WindowTitleBarTheme.axaml` | WindowTitleBar | `IsEffectiveLogoVisible`, `LeftAddOn`, `LeftAddOnTemplate`, `Logo`, `LogoTemplate` | template-stable | 用于主题维护；变更需同步主题、实现和 LLMS。 |
| `PART_Logo` | template node (ContentPresenter) | `WindowTitleBarTheme.axaml` | WindowTitleBar | `IsEffectiveLogoVisible`, `Logo`, `LogoTemplate` | template-stable | 用于主题维护；变更需同步主题、实现和 LLMS。 |
| `PART_LeftAddOn` | template node (ContentPresenter) | `WindowTitleBarTheme.axaml` | WindowTitleBar | `LeftAddOn`, `LeftAddOnTemplate` | template-stable | 用于主题维护；变更需同步主题、实现和 LLMS。 |
| `PART_ContentPresenter` | template node (ContentPresenter) | `WindowTitleBarTheme.axaml` | WindowTitleBar | `Title`, `TitleTemplate` | template-stable | 用于主题维护；变更需同步主题、实现和 LLMS。 |
| `StackPanel` | template node (StackPanel) | `WindowTitleBarTheme.axaml` | WindowTitleBar | `CanMaximize`, `CanMinimize`, `CaptionButtonCommand`, `HostWindowState`, `IsCloseCaptionButtonVisible`, `IsFullScreenCaptionButtonVisible` | template-stable | 用于主题维护；变更需同步主题、实现和 LLMS。 |
| `PART_RightAddOn` | template node (ContentPresenter) | `WindowTitleBarTheme.axaml` | WindowTitleBar | `RightAddOn`, `RightAddOnTemplate` | template-stable | 用于主题维护；变更需同步主题、实现和 LLMS。 |
| `PART_CaptionButtonGroup` | template node (CaptionButtonGroup) | `WindowTitleBarTheme.axaml` | WindowTitleBar | `CanMaximize`, `CanMinimize`, `CaptionButtonCommand`, `HostWindowState`, `IsCloseCaptionButtonVisible`, `IsFullScreenCaptionButtonVisible` | template-stable | 用于主题维护；变更需同步主题、实现和 LLMS。 |
| `WindowsCaptionButton` | control theme | `WindowsCaptionButtonTheme.axaml` | WindowTitleBar | `Background`, `EffectiveCornerRadius`, `EffectiveIcon`, `IconHeight`, `IconWidth`, `Padding` | internal-observable | 用于理解结构和状态流，不应指导用户代码直接依赖。 |
| `PART_Frame` | template node (Border) | `WindowsCaptionButtonTheme.axaml` | WindowsCaptionButton | `Background`, `EffectiveCornerRadius`, `EffectiveIcon`, `IconHeight`, `IconWidth`, `Padding` | template-stable | 用于主题维护；变更需同步主题、实现和 LLMS。 |
| `PART_IconPresenter` | template node (IconPresenter) | `WindowsCaptionButtonTheme.axaml` | WindowsCaptionButton | `EffectiveIcon`, `IconHeight`, `IconWidth` | template-stable | 用于主题维护；变更需同步主题、实现和 LLMS。 |

## Template Parts

| 节点 | 类型 | 契约 |
| --- | --- | --- |
| `Frame` | `Border` | 绘制标题栏背景并提供完整可见 frame 的布局边界。 |
| `PART_Logo` | `ContentPresenter` | 展示有效 Logo；Windows/Linux 模板中位于 Leading 最左侧，macOS 模板中位于 Title 内容前。 |
| `PART_ContentPresenter` | `ContentPresenter` | 展示标题；字符串标题在安全宽度不足时使用字符省略号，且不参与命中测试。 |
| `PART_LeftAddOn` | `ContentPresenter` | 展示 Leading 内容。 |
| `PART_RightAddOn` | `ContentPresenter` | 展示 Trailing add-on。 |
| `PART_CaptionButtonGroup` | `CaptionButtonGroup` | 消费宿主投影，推导 managed button 状态并转发固定窗口操作。 |

## Pseudo Classes

`WindowTitleBar` 从宿主 `Window` 的单向属性投影接收状态并维护以下伪类：

| 伪类 | 条件 |
| --- | --- |
| `:active` | 宿主窗口处于激活状态。 |
| `:normal` | `WindowState.Normal`。 |
| `:minimized` | `WindowState.Minimized`。 |
| `:maximized` | `WindowState.Maximized`。 |
| `:fullscreen` | `WindowState.FullScreen`。 |

窗口激活状态同时写入 `IsWindowActive`，供模板中的内部协作控件使用。状态 owner 始终是宿主 `Window`；模板节点不反向维护第二份窗口状态。

## State Flow

### 4.1 Logo 显示模型

`LogoVisibility` 的三个值具有以下语义：

| 值 | 规则 |
| --- | --- |
| `Always` | 存在 `Logo` 或 `LogoTemplate` 时显示。 |
| `Never` | 始终隐藏。 |
| `Auto` | 根据标题内容、平台和窗口状态计算。 |

`Auto` 的计算矩阵：

| 条件 | 结果 |
| --- | --- |
| 不存在 Logo 内容和 Logo 模板 | 隐藏。 |
| 存在有效标题内容 | 显示。 |
| 无标题内容且平台为 macOS | 隐藏。 |
| 无标题内容、平台不是 macOS、窗口非全屏 | 显示。 |
| 无标题内容、平台不是 macOS、窗口全屏 | 隐藏。 |

该模型只控制 Logo 的有效可见性，不修改 `Logo`、`Window.Icon` 或应用图标来源。

### 4.2 窗口状态

`WindowTitleBar` 从宿主 `Window` 的单向属性投影接收状态并维护以下伪类：

| 伪类 | 条件 |
| --- | --- |
| `:active` | 宿主窗口处于激活状态。 |
| `:normal` | `WindowState.Normal`。 |
| `:minimized` | `WindowState.Minimized`。 |
| `:maximized` | `WindowState.Maximized`。 |
| `:fullscreen` | `WindowState.FullScreen`。 |

窗口激活状态同时写入 `IsWindowActive`，供模板中的内部协作控件使用。状态 owner 始终是宿主 `Window`；模板节点不反向维护第二份窗口状态。

### 4.3 Caption buttons

caption button 的公共配置属于宿主 `Window`：

- `CanMinimize`、`CanMaximize` 和平台能力决定操作是否允许，不承担 managed button 的呈现配置。
- `IsMinimizeCaptionButtonVisible`、`IsMaximizeCaptionButtonVisible`、`IsCloseCaptionButtonVisible`、`IsFullScreenCaptionButtonVisible` 和 `IsPinCaptionButtonVisible` 分别表达 managed button 的 requested visibility。
- `Topmost`、`WindowState`、平台 backend、requested visibility 和 operation capability 共同决定 checked state 与 effective visibility。

Minimize、Maximize 和 Close 默认显示，FullScreen 和 Pin 默认隐藏。全屏时隐藏最小化和最大化按钮；最大化时隐藏进入全屏按钮；capability 为 `false` 时不显示不可执行的 managed button。Wayland backend 不提供置顶按钮。隐藏按钮不修改 `CanMinimize`、`CanMaximize`、`Topmost` 或其他窗口操作入口。完整状态矩阵、平台边界和单向命令流见 [WindowTitleBar Caption Button 配置设计](caption-button-configuration-design.md)。

### 4.4 拖动和双击

`Window` 监听标题栏 pointer 事件并在移动距离超过拖动阈值后调用原生 `BeginMoveDrag`。`IsMoveEnabled=False` 或全屏状态禁止拖动。双击最大化与拖动共用标题栏输入表面，但 caption buttons 和 add-on 的已处理输入不应触发窗口拖动。

## Theme and Token Boundaries

`WindowTitleBarTheme` 是 `WindowTitleBar` 的 ControlTheme 入口。内置主题保留以下稳定 template part 与语义节点：

| 节点 | 类型 | 契约 |
| --- | --- | --- |
| `Frame` | `Border` | 绘制标题栏背景并提供完整可见 frame 的布局边界。 |
| `PART_Logo` | `ContentPresenter` | 展示有效 Logo；Windows/Linux 模板中位于 Leading 最左侧，macOS 模板中位于 Title 内容前。 |
| `PART_ContentPresenter` | `ContentPresenter` | 展示标题；字符串标题在安全宽度不足时使用字符省略号，且不参与命中测试。 |
| `PART_LeftAddOn` | `ContentPresenter` | 展示 Leading 内容。 |
| `PART_RightAddOn` | `ContentPresenter` | 展示 Trailing add-on。 |
| `PART_CaptionButtonGroup` | `CaptionButtonGroup` | 消费宿主投影，推导 managed button 状态并转发固定窗口操作。 |

`PART_CaptionButtonGroup` 是 `WindowTitleBar` 模板中的稳定协作 part，通过 `TemplateBinding` 接收能力、requested visibility、窗口状态和宿主命令。其内部 `PART_CloseButton`、`PART_MinimizeButton`、`PART_MaximizeButton`、`PART_FullScreenButton` 和 `PART_PinButton` 属于 `CaptionButtonGroup` 模板，不是 `WindowTitleBar` 的 public template part。

平台主题可以改变 caption button 外观和 native chrome 来源，但不得改变 Public API 语义、Title/Leading/Trailing 角色或窗口操作行为。应用替换完整 ControlTheme 时负责提供等价区域、裁剪和命中测试；internal caption 类型不作为定制 API。

视觉尺寸、间距、active/inactive 颜色和 caption button 状态颜色由 [WindowTitleBar Token 设计](token.md) 管理。标题对齐值、CSD 状态和窗口状态不是 Token。

Token 边界：

`WindowTitleBarToken` 是 scope id 为 `WindowTitleBar` 的 internal control token，源码位于 `src/AtomUI.Desktop.Controls/WindowTitleBar/WindowTitleBarToken.cs`。它从 `SharedToken` 计算标题栏和 caption button 的视觉变量，并通过生成的 `WindowTitleBarTokenResource` key 供 AXAML 使用。

Token 负责尺寸、间距、字体和状态颜色，不负责以下运行时语义：

- `TitleAlignment`、Leading/Title/Trailing 角色和布局公式。
- CSD、native chrome insets、WindowState 和 backend 能力。
- Logo、标题、add-on 或 caption button 的有效可见性。
- pointer capture、拖动、checked state 和窗口操作。

## Customization Boundaries

- Public API 的类型、默认值、绑定语义和事件时序保持稳定。
- `Auto` Logo 规则和标题对齐的显式枚举语义保持稳定。
- `PART_CaptionButtonGroup`、内容 presenter 名称、ControlTheme key 和伪类保持稳定。
- Title 内容不参与命中测试；add-on 和 caption buttons 保持可交互。
- Windows/Linux 中 Logo 始终位于 Leading 最左侧并参与左侧安全空间；macOS 中 Logo 和 Title 作为连续 Title 组；add-on 不进入标题中心计算。
- CSD 开关只改变 chrome metrics 来源和可见操作区，不改变显式标题对齐含义。
- 标题栏替换时释放旧的 Window 投影；template reapply 不建立逐按钮 Click handler 或 CaptionButtonGroup 到 Window 的宿主引用。
- 平台选择和 Token 发现不依赖运行时反射或程序集扫描。

维护不变量：

- `WindowTitleBar` 与 `Window.NotifyConfigureTitleBar` 的属性投影保持单向且完整；默认标题栏的 `LeftAddOn`、`LeftAddOnTemplate`、`RightAddOn` 和 `RightAddOnTemplate` 由 `Window` 的同名 public API 以 `Template` 优先级提供，派生标题栏 local add-on 不被覆盖。
- `WindowTitleBar` 的宿主投影保持单向且完整；CaptionButtonGroup 不通过 logical attach/detach 建立 Window 状态副本。
- 三个平台 ControlTemplate 保持相同语义角色、稳定 part 名称和平台 caption button 顺序。
- Windows/Linux 的 Logo 始终位于 Leading 最左侧；macOS、ImagePreviewer 与全屏标题宿主可将图标与 Title 保持为连续 Title 组。无论图标位于哪个 role，标题对齐公式只读取 Leading、Title、Trailing 三个 direct role child 的实测宽度。
- Leading/Trailing 为零宽时不产生操作区间距；add-on margin 只通过 `DesiredSize` 计入一次。
- ImagePreviewer 与两个全屏标题宿主复用同一标题布局模型。
- Title 不参与命中测试；add-on 与 caption buttons 保持可交互。
- `WindowTitleBarToken`、generated resource key 和 Theme 消费名保持同步。
