# WindowTitleBar 桌面版实现原理

本文档说明 `WindowTitleBar` 控件家族的源码职责、运行时 composition、状态流、模板生命周期和跨平台标题布局。公共契约见 [WindowTitleBar 桌面版架构设计](overview.md)，caption button 的能力与呈现模型见 [WindowTitleBar Caption Button 配置设计](caption-button-configuration-design.md)，视觉变量见 [WindowTitleBar Token 设计](token.md)，契约变化见 [WindowTitleBar Changelog](changelog.md)。

## 1. 实现定位

`WindowTitleBar` 的实现分为四层：

1. `Window` 持有真实窗口状态、平台 chrome 状态和原生窗口能力。
2. `WindowTitleBar` 投影公共内容、窗口状态和标题栏交互。
3. `CaptionButtonGroup` 根据投影输入推导 effective visibility，并通过声明式命令转发固定窗口操作。
4. ControlTheme 声明平台视觉结构、语义 part 和 Token 绑定。

状态从 `Window` 单向流向标题栏及其模板 part。只有 caption button 命令、拖动和双击请求返回宿主窗口；模板视觉不成为窗口状态 owner。

## 2. 源码文件结构

```text
src/AtomUI.Desktop.Controls/
├── Window/
│   ├── Window.cs
│   ├── MacStandardWindowButtons.cs
│   ├── Chrome/
│   │   ├── WindowChromeManager.cs
│   │   ├── AbstractLinuxWindowChromeManager.cs
│   │   ├── GenericLinuxWindowChromeManager.cs
│   │   ├── X11WindowChromeManager.cs
│   │   ├── WaylandWindowChromeManager.cs
│   │   └── WindowsWindowChromeManager.cs
│   ├── Utils/
│   │   ├── FullscreenPopoverLayer.cs
│   │   └── WindowVisualLayerClip.cs
│   └── Themes/
│       ├── WindowTheme.axaml
│       ├── WindowDrawnDecorationsTheme.axaml
│       └── FullscreenPopoverLayerTheme.axaml
├── WindowTitleBar/
│   ├── WindowTitleBar.cs
│   ├── WindowTitleBarLogoVisibility.cs
│   ├── WindowTitleBarTitleAlignment.cs
│   ├── WindowTitleBarLayoutPanel.cs
│   ├── CaptionButtonGroup.cs
│   ├── CaptionButton.cs
│   ├── WindowsCaptionButton.cs
│   ├── WindowTitleBarToken.cs
│   ├── Strategies/
│   │   ├── IWindowTitleBarLayoutStrategy.cs
│   │   ├── MacOSWindowTitleBarLayoutStrategy.cs
│   │   ├── WindowsWindowTitleBarLayoutStrategy.cs
│   │   └── LinuxWindowTitleBarLayoutStrategy.cs
│   └── Themes/
│       ├── WindowTitleBarTheme.axaml
│       ├── CaptionButtonGroupTheme.axaml
│       ├── CaptionButtonTheme.axaml
│       └── WindowsCaptionButtonTheme.axaml
└── ImagePreviewer/
    ├── ImagePreviewerTitleBar.cs
    └── Themes/ImagePreviewerTitleBarTheme.axaml
```

标题布局输入和中间结果使用标量、`Thickness`、`Rect` 与命名 tuple 传递，不建立独立 Context、Plan 或布局结果类型。

## 3. 核心类职责

| 文件或类型 | Owner 职责 |
| --- | --- |
| `Window.cs` | 创建和配置标题栏，持有窗口状态与窗口操作，处理拖动与最大化请求，发布 caption capability、platform、CSD 和 native chrome metrics。 |
| `WindowTitleBar.cs` | 注册公共契约，接收宿主单向投影，维护有效 Logo、窗口伪类和标题栏输入事件。 |
| `WindowTitleBarLayoutPanel.cs` | 测量并排列 Leading、Title、Trailing，集中执行共享标题对齐公式。 |
| `Strategies/*` | 解释平台 `Auto` 值并归一有效 native chrome insets，不操作 Visual。 |
| `CaptionButtonGroup.cs` | 从标题栏投影输入推导按钮 effective visibility 和 checked state，并向固定按钮提供宿主命令。 |
| `CaptionButton.cs` | 计算 effective icon、圆形背景和 transition 初始化时序。 |
| `WindowsCaptionButton.cs` | 提供 Windows 方形按钮尺寸与窗口状态切换后的 pointer-over 修正。 |
| `WindowTitleBarToken.cs` | 从 SharedToken 计算标题栏视觉变量，不保存实例状态。 |
| `Themes/*.axaml` | 声明静态 composition、平台模板、selector、命中测试角色和 Token 消费。 |

## 4. 状态与数据流

### 4.1 Window state 与 active state

```text
Window.WindowState / Window.IsActive
  -> Window.NotifyConfigureTitleBar projection
  -> :normal / :minimized / :maximized / :fullscreen / :active
  -> IsWindowActive
  -> ControlTheme selectors and CaptionButtonGroup
```

窗口状态伪类在每次状态通知中完整设置，不能依赖前一个状态自行清除。`IsWindowActive` 继续传给 caption buttons，使标题文本和按钮图标使用同一 active/inactive 状态。

### 4.2 Caption button configuration

```text
Window caption visibility + capability + WindowState + platform support
  -> WindowTitleBar projected inputs
  -> CaptionButtonGroup effective visibility / checked state
  -> CaptionButton TemplateBinding
```

Window 只发布 requested visibility、窗口能力、状态和操作命令。`CaptionButtonGroup` 只计算派生视觉，不通过逻辑祖先重新发现 Window，也不反向写入 `CanMinimize`、`CanMaximize`、`WindowState` 或 `Topmost`。完整公式和平台矩阵见 [WindowTitleBar Caption Button 配置设计](caption-button-configuration-design.md)。

### 4.3 Effective Logo

`Logo`、`LogoTemplate`、`LogoVisibility`、`Title`、`OsType` 或全屏状态变化时重新计算 `IsEffectiveLogoVisible`。Theme 只绑定这一 internal direct property，不在平台模板中复制 Logo 决策。

### 4.4 标题对齐

```text
Window.TitleAlignment + add-on content/templates + platform/native metrics
  -> WindowTitleBar layout inputs
  -> WindowTitleBarLayoutPanel
  -> platform Strategy
  -> shared safe-region and alignment math
  -> Leading / Title / Trailing rectangles
```

平台层只发布逻辑像素 metrics；Panel 不查找 `Window`、不调用 native API。详细输入、CSD 矩阵、公式和失效条件由本文第 8 节集中定义。

### 4.5 Theme 与 Token

```text
SharedToken
  -> WindowTitleBarToken.CalculateTokenValues
  -> generated WindowTitleBarTokenResource keys
  -> WindowTitleBar / CaptionButton ControlTheme
  -> template properties and selectors
```

标题栏背景可以由宿主控件的专属 Token 覆盖，但 caption 尺寸和交互状态仍使用 WindowTitleBar 语义变量。

## 5. 组合结构模型

### 控件角色图

```text
Window
└── Window template / drawn decorations host
    └── WindowTitleBar
        └── WindowTitleBarLayoutPanel
            ├── Leading (Windows/Linux)
            │   └── DockPanel
            │       ├── ContentPresenter#PART_Logo
            │       └── ContentPresenter#PART_LeftAddOn
            ├── Leading (macOS)
            │   └── ContentPresenter#PART_LeftAddOn
            ├── Title (Windows/Linux)
            │   └── DockPanel
            │       └── ContentPresenter#PART_ContentPresenter
            ├── Title (macOS)
            │   └── DockPanel
            │       ├── ContentPresenter#PART_Logo
            │       └── ContentPresenter#PART_ContentPresenter
            └── Trailing
                ├── ContentPresenter#PART_RightAddOn
                └── CaptionButtonGroup#PART_CaptionButtonGroup
                    └── platform caption button template parts
```

### 协作节点

| 节点 | 类型 | 来源 | 生命周期 owner | 影响的 public API | 稳定性 | Agent 使用边界 |
| --- | --- | --- | --- | --- | --- | --- |
| `WindowTitleBar` | public control | `WindowTitleBar.cs` | `Window` 或应用宿主 | 全部标题栏 public surface | public | 可直接使用、派生和替换 ControlTheme。 |
| `WindowTitleBarLayoutPanel` | layout panel | `WindowTitleBarTheme.axaml` | `WindowTitleBar` template | `TitleAlignment`、内容与 add-on | internal-observable | 仅用于理解布局；应用不直接依赖类型或 Role。 |
| `PART_LeftAddOn`、`PART_Logo`、`PART_ContentPresenter`、`PART_RightAddOn` | presenters | `WindowTitleBarTheme.axaml` | `WindowTitleBar` template | 对应内容与模板属性 | template-stable | 可用于主题维护；变更需同步主题、实现和文档。 |
| `PART_CaptionButtonGroup` | internal control part | `WindowTitleBarTheme.axaml` | `WindowTitleBar` | Window caption 配置 | template-stable | 作为稳定协作 part；应用不直接创建 internal 类型。 |
| `CaptionButton` / `WindowsCaptionButton` | internal button | caption themes | `CaptionButtonGroup` | 用户可观察的窗口操作 | internal-observable | 只用于理解平台结构和状态，不作为应用 API。 |
| `ImagePreviewerTitleBar` | internal derived control | ImagePreviewer theme | `ImagePreviewer` | 继承标题栏内容语义 | internal-observable | 只用于维护派生宿主一致性。 |

`ImagePreviewerTitleBar` 将预览 toolbar 放入 Leading，并使用预览图标与标题构成 Title。全屏标题宿主使用同一布局角色和算法，不维护第二套标题居中逻辑。

## 6. 生命周期与模板接入

### 6.1 创建与配置

`Window.OnApplyTemplate` 的标题栏接入顺序为：

1. 从旧标题栏移除最大化、pointer 和尺寸事件。
2. 通过 `NotifyCreateTitleBar(oldTitleBar)` 创建或替换标题栏。
3. 给新标题栏连接最大化请求、拖动 pointer 事件和 `SizeChanged`。
4. 通过 `NotifyConfigureTitleBar` 投影 Window 属性与平台布局输入。
5. 将结果写入 internal `TitleBar`，交给 Window template 展示。

派生 `Window` 可以覆盖两个 protected 方法，但必须保留同等的状态投影和生命周期配对。重复 apply template 不能让旧标题栏继续持有 Window 事件。

### 6.2 宿主投影生命周期

`Window.NotifyConfigureTitleBar` 显式建立标题栏宿主关联，并把 caption requested visibility、窗口能力、WindowState、Topmost、active state、平台支持和宿主命令投影给标题栏。宿主关联与投影 owner 均和标题栏创建生命周期一致；替换标题栏时释放旧关联和投影。`WindowTitleBar` 保留宿主关联供 detached popup、routed event 等标题栏级集成使用，但不通过该引用读取或订阅 caption 状态；`CaptionButtonGroup` 不持有宿主引用，也不通过逻辑祖先建立第二套 Window 订阅。

### 6.3 Template reapply

`WindowTitleBar.OnApplyTemplate` 只接入标题栏自身必须持有的 template part。标题、Logo、add-on 和 caption 配置通过 `TemplateBinding` 获取，不从宿主引用旁路读取，也不重复建立 relay binding。

LeftAddOn 或 RightAddOn 的内容、可见性、子节点、模板和 margin 变化沿 Avalonia visual tree 使布局重新测量。Panel 始终读取当前 `DesiredSize`，不保存 add-on 宽度缓存，也不需要由标题栏代码手工调用 `InvalidateMeasure`。

`CaptionButtonGroupTheme` 为 `PART_CloseButton`、`PART_MinimizeButton`、`PART_MaximizeButton`、`PART_FullScreenButton` 和 `PART_PinButton` 声明固定 command parameter。Template reapply 不注册逐按钮 Click handler；Windows pointer-over 状态由按钮自身消费窗口状态输入并失效。

## 7. 交互与事件处理

`WindowTitleBar` 在主按钮双击的 `PointerPressed` 阶段只记录 pending 状态，在匹配的 `PointerReleased` 阶段发出 `MaximizeWindowRequested`。pointer capture 丢失、释放按钮不匹配或其他结束路径都会清除 pending。

`Window` 负责标题栏拖动：按下时记录窗口坐标，移动超过 `Constants.DragThreshold` 后清理本地状态并调用 `BeginMoveDrag`。`IsMoveEnabled=False`、FullScreen 或非主按钮输入不进入拖动。add-on 与 caption button 的已处理输入不会进入标题栏拖动或双击最大化路径。

Caption button 通过宿主命令提交固定窗口操作；`Window` 统一执行：

- FullScreen 在进入时保存原 WindowState，退出时恢复；无可恢复值时回到 Normal。
- Maximize 在 Normal/Maximized 间切换，并尊重 `CanMaximize` 与 FullScreen。
- Minimize 写入 `WindowState.Minimized`。
- Pin 切换 `Window.Topmost` 并同步 checked state。
- Close 标记用户 caption close 请求后调用 `Window.Close()`。

窗口状态变化后，Windows caption buttons 抑制旧 pointer-over 视觉，直到新的 pointer enter/move 恢复 hover。标题栏和 caption buttons 在初始化阶段禁用 transition，Loaded 后通过 Dispatcher 恢复。

## 8. 内部算法与关键流程

`CaptionButtonGroup` 的有效可见性集中计算：每个按钮先遵循独立 requested visibility，再与窗口 capability、WindowState 和 backend 支持组合。全屏按钮在最大化时隐藏；最小化和最大化按钮在全屏时隐藏；置顶按钮同时受配置与 backend 能力约束；关闭按钮遵循宿主配置。Wayland 不提供置顶能力，Linux backend 识别由 `AbstractLinuxWindowChromeManager` 统一收敛。Visibility 不写回 capability，具体公式见 [WindowTitleBar Caption Button 配置设计](caption-button-configuration-design.md)。

标题布局只在 `WindowTitleBarLayoutPanel` 中执行：先归一 native chrome inset，再在原生安全边界后应用 managed Padding，最后把 Leading 和 Trailing 转换为标题安全边界并执行 `Left`、`Center`、`WindowCenter` 或 `Right` 的共享公式。基础边界为 `BL = clamp(NL + PL, 0, W)` 与 `BR = clamp(W - NR - PR, 0, W)`；native extent 和 Padding 各计算一次，`WindowCenter` 仍以完整 frame 的 `W / 2` 为轴。平台 Strategy 不测量 Visual，也不复制对齐公式。

操作区占位使用当前实测宽度与条件间距：

```text
ML = LeadingWidth > 0 ? LeadingWidth + HeaderHorizontalSpacing : 0
MR = TrailingWidth > 0 ? TrailingWidth + HeaderHorizontalSpacing : 0
```

`LeadingWidth` 与 `TrailingWidth` 来自 direct role child 的 `DesiredSize.Width`，已经包含该 child 自身 margin，因此 margin 不再额外累加。区域缺失、不可见、内容为空或孩子实测为零时，对应占位和间距同时为零。Title 组内的 `LogoAndTitleSpacing` 也只在 Logo/Icon 与 Title 两个有效孩子都参与布局时出现。

Windows/Linux 默认模板把有效 Logo 放入 Leading direct role child，并排在 `PART_LeftAddOn` 之前；因此 Logo 宽度作为 `LeadingWidth` 的一部分参与安全空间计算。macOS 默认模板、ImagePreviewer 标题宿主和全屏标题宿主仍可把图标放在 Title role 内，并继续由同一标题对齐公式处理。

## 9. 资源、性能与 AOT 边界

- Window 到标题栏的宿主关联和投影 binding 由标题栏创建和替换生命周期统一所有；宿主关联不承载 caption 状态同步，CaptionButtonGroup 不持有 Window relay binding 或宿主引用。
- 标题布局 Strategy 使用静态无状态实例；measure/arrange 不创建 Context、Plan、binding 或临时 Visual。
- TemplateBinding 和 selector 承担静态视觉投影，不在状态变化时重建模板节点。
- Logo 计算只在相关属性或 WindowState 变化时执行。
- native chrome metrics 缓存属于 Window/platform manager，不能复制到 Panel 或 Strategy。
- 平台 Strategy 使用封闭 `OsType` switch，不使用反射、程序集扫描、字符串类型发现或运行时 DI。
- Token 通过生成的静态资源入口消费；不反射枚举 public API 或 Token 属性。

## 10. 维护不变量

- `WindowTitleBar` 与 `Window.NotifyConfigureTitleBar` 的属性投影保持单向且完整；默认标题栏的 `LeftAddOn`、`LeftAddOnTemplate`、`RightAddOn` 和 `RightAddOnTemplate` 由 `Window` 的同名 public API 以 `Template` 优先级提供，派生标题栏 local add-on 不被覆盖。
- `WindowTitleBar` 的宿主投影保持单向且完整；CaptionButtonGroup 不通过 logical attach/detach 建立 Window 状态副本。
- 三个平台 ControlTemplate 保持相同语义角色、稳定 part 名称和平台 caption button 顺序。
- Windows/Linux 的 Logo 始终位于 Leading 最左侧；macOS、ImagePreviewer 与全屏标题宿主可将图标与 Title 保持为连续 Title 组。无论图标位于哪个 role，标题对齐公式只读取 Leading、Title、Trailing 三个 direct role child 的实测宽度。
- Leading/Trailing 为零宽时不产生操作区间距；add-on margin 只通过 `DesiredSize` 计入一次。
- ImagePreviewer 与两个全屏标题宿主复用同一标题布局模型。
- Title 不参与命中测试；add-on 与 caption buttons 保持可交互。
- `WindowTitleBarToken`、generated resource key 和 Theme 消费名保持同步。

## 11. 测试与验证

- `WindowTitleBarLogoVisibilityTests` 覆盖 Logo 默认值、平台规则、全屏规则和 Window 投影。
- `WindowTitleBarAddOnTests` 覆盖 Window add-on API 默认值、模板类型及到默认标题栏的单向实时投影。
- `WindowCaptionButtonConfigurationTests` 覆盖五个 visibility 属性默认值、capability 隔离、effective truth table、动态状态投影和 template reapply。
- `WindowTitleBarTokenTests` 覆盖 Token 默认值、三平台 caption 视觉和 Windows edge layout。
- `ImagePreviewerTitleBarThemeTests` 覆盖派生标题栏的标题组、操作区和平台模板契约。
- 标题几何测试覆盖所有 alignment、对称与非对称操作区、Padding/native inset、窄窗口和非法 metrics。
- Windows、macOS、Linux 实机验证覆盖 CSD/非 CSD、缩放、最大化和全屏状态。
- 文档改动运行 LLMS `verify`、相对链接检查和 `git diff --check`；行为、Theme 或 Public API 变更运行对应 Desktop Controls 测试。
