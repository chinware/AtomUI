# SplitView 分割视图

## 概述

分割视图（SplitView）是一个带有可折叠侧边栏（Pane）的布局容器控件。常用于应用的主框架布局——侧边栏承载导航菜单、工具面板等辅助内容，主内容区域展示核心业务页面。用户可以通过切换 `IsPaneOpen` 属性来展开或收起侧边栏。

AtomUI 的 `SplitView` 是对 Avalonia 原生 `Avalonia.Controls.SplitView` 的增强封装。它在继承 Avalonia 所有基础能力的同时，集成了 AtomUI Design Token 系统和平滑过渡动画，使侧边栏的展开/收起过程更加流畅自然。

> 这是 AtomUI 的扩展控件，在 Ant Design 中没有直接对应的组件。其设计灵感来源于 UWP/WinUI 的 NavigationView 模式，同时遵循 AtomUI 的 Design Token 视觉规范。

---

## 设计原理

### 分割视图的设计哲学

SplitView 解决的核心问题是：**在有限的屏幕空间内，同时展示导航/工具内容与主体内容，并提供灵活的空间分配策略**。其设计围绕以下几个关键概念：

**四种显示模式**（DisplayMode），决定了侧边栏与主内容的空间关系：

| 模式 | 设计意图 | 典型用途 |
|---|---|---|
| 📌 **Inline** | 侧边栏与主内容并排，展开时主内容区域被压缩 | 宽屏应用，侧边栏常驻时不遮挡内容 |
| 📐 **CompactInline** | 收起时保留紧凑条（CompactPaneLength），展开时压缩主内容 | 侧导航图标常驻 + 展开显示文字 |
| 🔲 **Overlay** | 侧边栏覆盖在主内容之上，展开时不影响主内容布局 | 窄屏应用，临时弹出导航面板 |
| 📏 **CompactOverlay** | 收起时保留紧凑条，展开时覆盖在主内容之上 | 移动端/窄屏的图标导航 + 弹出详情 |

**四种面板放置位置**（PanePlacement）：

| 位置 | 说明 |
|---|---|
| `Left` | 侧边栏在左侧（默认，最常用的导航布局） |
| `Right` | 侧边栏在右侧（如属性面板、详情面板） |
| `Top` | 面板在顶部 |
| `Bottom` | 面板在底部 |

### Avalonia SplitView 基础能力

AtomUI 的 `SplitView` 直接继承自 `Avalonia.Controls.SplitView`。了解 Avalonia SplitView 的基础能力有助于理解 AtomUI 在其之上做了哪些增强。

**Avalonia SplitView 的核心职责：**

Avalonia 的 `SplitView` 是一个将布局区域一分为二的容器控件——一侧为可折叠的 Pane（面板），另一侧为 Content（主内容）。它通过 `SplitViewTemplateSettings` 内部管理面板的列宽/行高等模板参数，支持四种显示模式和四种面板位置。其继承链为：

```
Control → TemplatedControl → ContentControl → SplitView
```

**Avalonia SplitView 提供的基础属性：**

| 属性 | 说明 |
|---|---|
| `Content` | 主内容区域，可以是任意控件 |
| `Pane` | 侧边栏内容，可以是任意控件 |
| `PaneTemplate` / `ContentTemplate` | 面板/主内容的数据模板 |
| `IsPaneOpen` | 面板是否处于展开状态 |
| `DisplayMode` | 显示模式（`Overlay`、`Inline`、`CompactInline`、`CompactOverlay`） |
| `PanePlacement` | 面板放置位置（`Left`、`Right`、`Top`、`Bottom`） |
| `OpenPaneLength` | 面板完全展开时的宽度（水平）或高度（垂直） |
| `CompactPaneLength` | 紧凑模式下面板的宽度/高度 |
| `PaneBackground` | 面板背景画刷 |
| `UseLightDismissOverlayMode` | 是否在 Overlay 模式下启用点击遮罩自动关闭 |

**Avalonia SplitView 提供的基础伪类：**

| 伪类 | 说明 |
|---|---|
| `:open` | 面板处于展开状态 |
| `:closed` | 面板处于收起状态 |
| `:left` | 面板在左侧 |
| `:right` | 面板在右侧 |
| `:top` | 面板在顶部 |
| `:bottom` | 面板在底部 |
| `:overlay` | Overlay 显示模式 |
| `:inline` | Inline 显示模式 |
| `:compactinline` | CompactInline 显示模式 |
| `:compactoverlay` | CompactOverlay 显示模式 |
| `:lightDismiss` | 启用了轻量关闭遮罩 |

**Avalonia SplitView 提供的事件：**

| 事件 | 说明 |
|---|---|
| `PaneClosing` | 面板即将关闭时触发，可取消 |
| `PaneClosed` | 面板关闭完成后触发 |

### AtomUI 的扩展设计

AtomUI `SplitView` 在 Avalonia SplitView 的基础上做了以下增强：

| 扩展能力 | 实现方式 | 设计动机 |
|---|---|---|
| **Design Token 集成** | `SplitViewToken` + `RegisterTokenResourceScope` | 面板尺寸、动画参数等可通过 Token 统一管理，支持主题切换 |
| **平滑过渡动画** | `PaneOpenTransitions` / `PaneCloseTransitions` + 自动配置 | 面板展开/收起时使用缓动动画，而非瞬间切换，提升交互体验 |
| **分离的展开/收起动画时长** | `PaneOpenMotionDuration` / `PaneCloseMotionDuration` | 允许展开和收起使用不同的动画时长，精细控制交互节奏 |
| **自定义缓动曲线** | `PaneMotionEasing` 属性 | 支持自定义贝塞尔缓动曲线，默认使用 `cubic-bezier(0.1, 0.9, 0.2, 1.0)` 实现自然的减速效果 |
| **动画开关** | `IMotionAwareControl` 接口 + `IsMotionEnabled` | 全局或单控件级别禁用动画，适用于性能敏感场景或无障碍需求 |
| **方向感知的过渡** | 根据 `PanePlacement` 自动选择 Width 或 Height 过渡 | 水平面板过渡宽度，垂直面板过渡高度，动画方向始终正确 |
| **Token 驱动的面板背景** | `PaneBackground` 默认绑定 `SharedToken.ColorBgContainer` | 面板背景色跟随主题自动切换（亮色/暗色模式） |
| **Token 驱动的边框颜色** | `HCPaneBorder` 使用 `SharedToken.ColorBorder` | 面板分隔线颜色跟随主题 |

---

## 功能详解

### 显示模式（DisplayMode）

显示模式通过 `DisplayMode` 属性设置，控制侧边栏展开/收起时与主内容区域的空间关系。

| 模式 | 收起时行为 | 展开时行为 |
|---|---|---|
| `Overlay` | 面板完全隐藏（宽度/高度为 0） | 面板覆盖在主内容之上，主内容不移动 |
| `Inline` | 面板完全隐藏（宽度/高度为 0） | 面板与主内容并排，主内容被压缩 |
| `CompactInline` | 面板保留紧凑宽度（`CompactPaneLength`） | 面板展开至 `OpenPaneLength`，主内容被压缩 |
| `CompactOverlay` | 面板保留紧凑宽度（`CompactPaneLength`） | 面板展开并覆盖主内容 |

### 面板放置位置（PanePlacement）

面板位置通过 `PanePlacement` 属性设置。AtomUI 支持四个方向：

- **Left / Right**：面板在水平方向，使用 `Grid.ColumnDefinitions` 布局，过渡 `Width` 属性
- **Top / Bottom**：面板在垂直方向，使用 `Grid.RowDefinitions` 布局，过渡 `Height` 属性

当 `PanePlacement` 改变时，SplitView 会自动重新配置过渡动画（从 Width 过渡切换到 Height 过渡，反之亦然）。

### 过渡动画

SplitView 的面板展开/收起动画是 AtomUI 的核心增强之一。动画行为如下：

1. **OnLoaded 时配置**：控件加载后自动根据 `PanePlacement` 创建对应的 `DoubleTransition`
2. **OnUnloaded 时清除**：控件卸载时清空过渡，避免不可见控件消耗资源
3. **方向自适应**：水平面板（Left/Right）过渡 `Width`，垂直面板（Top/Bottom）过渡 `Height`
4. **展开/收起独立控制**：`PaneOpenTransitions` 和 `PaneCloseTransitions` 分别应用于展开和收起过程，支持不同时长

动画参数通过 Token 或属性控制：

| 参数 | Token 来源 | 默认值 | 说明 |
|---|---|---|---|
| `PaneOpenMotionDuration` | `SplitViewToken.PaneOpenMotionDuration` | 200ms | 展开动画时长 |
| `PaneCloseMotionDuration` | `SplitViewToken.PaneCloseMotionDuration` | 100ms | 收起动画时长 |
| `PaneMotionEasing` | `SplitViewToken.PaneMotionEasing` | `cubic-bezier(0.1, 0.9, 0.2, 1.0)` | 缓动曲线 |

> 缓动曲线 `(0.1, 0.9, 0.2, 1.0)` 实现了「快启动、缓停止」的自然减速效果，符合 Material Design 和 Ant Design 的动效原则。

### 轻量关闭遮罩（Light Dismiss）

在 `Overlay` 和 `CompactOverlay` 模式下，当面板展开时，主内容区域会覆盖一层 `LightDismissLayer`（半透明遮罩），颜色由 `SharedToken.ColorBgMask` 控制。点击遮罩可自动关闭面板（需启用 `UseLightDismissOverlayMode`）。

### 面板边框分隔线

每个面板模板中都包含一条 `HCPaneBorder` 分隔线（`Rectangle`），颜色由 `SharedToken.ColorBorder` 控制：
- Left 面板：分隔线在右边缘
- Right 面板：分隔线在左边缘
- Top 面板：分隔线在底部
- Bottom 面板：分隔线在顶部

---

## 与 Ant Design 规范的对齐程度

SplitView 是 AtomUI 的**扩展控件**，在 Ant Design 5.0 中没有直接对应的组件。但它遵循了 AtomUI Design Token 系统，确保视觉风格与其他 Ant Design 组件保持一致：

| 方面 | 对齐情况 | 说明 |
|---|---|---|
| 颜色系统 | ✅ | 面板背景使用 `ColorBgContainer`，边框使用 `ColorBorder`，遮罩使用 `ColorBgMask`——均来自 SharedToken |
| 动画系统 | ✅ | 缓动曲线和动画时长遵循 Ant Design 动效规范 |
| Token 架构 | ✅ | 使用 `SplitViewToken` 组件级 Token，支持主题定制 |
| 对应 Ant Design 组件 | — | 无直接对应，Ant Design 的 `Layout.Sider` 提供类似功能但 API 不同 |

---

## 继承关系

```
Avalonia.Controls.Primitives.TemplatedControl
  └── Avalonia.Controls.ContentControl
        └── Avalonia.Controls.SplitView
              └── AtomUI.Desktop.Controls.SplitView
                    └── implements IMotionAwareControl
```

`SplitView` 通过 `using AvaloniaSplitView = Avalonia.Controls.SplitView;` 别名引用 Avalonia 原生 SplitView，避免类名冲突。

**各层级职责划分：**

| 层级 | 提供的能力 |
|---|---|
| `ContentControl` | 主内容容纳、`Content` / `ContentTemplate`、`ContentPresenter` 模板化呈现 |
| `Avalonia.Controls.SplitView` | 面板/主内容双区域布局、四种显示模式、四种面板位置、`IsPaneOpen` 状态管理、`PaneClosing` / `PaneClosed` 事件、`SplitViewTemplateSettings` 模板参数计算 |
| `AtomUI.Desktop.Controls.SplitView` | Design Token 集成、平滑过渡动画（展开/收起独立时长）、自定义缓动曲线、动画开关、Token 驱动的背景/边框颜色 |

**实现的共享接口：**

| 接口 | 定义位置 | 作用 |
|---|---|---|
| `IMotionAwareControl` | `AtomUI.Core` | 支持 `IsMotionEnabled` 动画开关，全局或单控件级别禁用过渡动画 |

---

## 源码位置

| 层级 | 文件路径 | 说明 |
|---|---|---|
| 控件类 | `src/AtomUI.Desktop.Controls/SplitView/SplitView.cs` | 桌面端 SplitView 实现 |
| Token 定义 | `src/AtomUI.Desktop.Controls/SplitView/SplitViewToken.cs` | 组件级 Design Token |
| 主题模板 | `src/AtomUI.Desktop.Controls/SplitView/Themes/SplitViewTheme.axaml` | ControlTheme AXAML（包含四种面板方向的模板定义） |

> 注意：SplitView 没有对应的 `AbstractSplitView` 基类在 `AtomUI.Controls` 中——它直接继承 Avalonia 的 `SplitView`，因为分割视图的行为在各平台上基本一致，不需要抽象基类层。

---

## 模板结构

SplitView 为四种面板位置分别定义了独立的 ControlTemplate。以 Left 面板为例：

```
Grid#Container (Background = TemplateBinding Background)
├── ColumnDefinition (PaneColumnGridLength)     ← 面板列（宽度由 TemplateSettings 计算）
├── ColumnDefinition (*)                         ← 主内容列（填充剩余空间）
│
├── Panel#PART_PaneRoot (Column=0)               ← 面板容器（ClipToBounds, ZIndex=100）
│   ├── ContentPresenter#PART_PanePresenter      ← 面板内容展示器
│   └── Rectangle#HCPaneBorder                   ← 面板右边缘分隔线（1px, ColorBorder）
│
└── Panel#ContentRoot (Column=1)                 ← 主内容容器
    ├── ContentPresenter#PART_ContentPresenter   ← 主内容展示器
    └── Rectangle#LightDismissLayer              ← 遮罩层（Overlay 模式展开时可见）
```

**四种方向的布局差异：**

| 面板位置 | 布局方式 | 面板对齐 | 分隔线位置 |
|---|---|---|---|
| Left | `Grid.ColumnDefinitions` | `HorizontalAlignment="Left"` | 右边缘（`HorizontalAlignment="Right"`） |
| Right | `Grid.ColumnDefinitions` | `HorizontalAlignment="Right"` | 左边缘（`HorizontalAlignment="Left"`） |
| Top | `Grid.RowDefinitions` | `VerticalAlignment="Top"` | 底部（`VerticalAlignment="Bottom"`） |
| Bottom | `Grid.RowDefinitions` | `VerticalAlignment="Bottom"` | 顶部（`VerticalAlignment="Top"`） |

**显示模式对布局的影响：**

不同的 `DisplayMode` 通过伪类选择器调整面板的 `Grid.Column` / `Grid.ColumnSpan`（或 `Grid.Row` / `Grid.RowSpan`）和尺寸绑定，实现不同的空间分配策略。

**过渡动画的应用位置：**

动画通过伪类组合选择器精确控制：
- `:left:open` / `:right:open` → 使用 `PaneOpenTransitions`，目标 Width = `OpenPaneLength`
- `:left:closed` / `:right:closed` → 使用 `PaneCloseTransitions`，目标 Width = `ClosedPaneWidth`
- `:top:open` / `:bottom:open` → 使用 `PaneOpenTransitions`，目标 Height = `OpenPaneLength`
- `:top:closed` / `:bottom:closed` → 使用 `PaneCloseTransitions`，目标 Height = `ClosedPaneHeight`
