# ScrollViewer 滚动视图

## 概述

ScrollViewer（滚动视图）是用于在有限的显示区域内查看超出边界内容的容器控件。当内容尺寸超过 ScrollViewer 的可视区域时，用户可以通过滚动条（水平和/或垂直）来浏览全部内容。ScrollViewer 是构建可滚动列表、长表单、文档阅读器等场景的基础设施组件。

AtomUI 的 `ScrollViewer` 是对 Avalonia 原生 `ScrollViewer` 的**主题化增强**，通过 Design Token 系统提供了统一的滚动条视觉风格，并增加了**极简模式（LiteMode）**、**自动隐藏（AllowAutoHide）**以及**平滑过渡动画**等能力。

> **注意**：ScrollViewer 是 AtomUI 的扩展控件，在 Ant Design 中没有直接对应的组件。其设计灵感来自 macOS 系统滚动条和现代 UI 框架的滚动条风格。

---

## 设计原理

### 滚动视图的设计目标

滚动视图是 UI 框架中最基础的布局容器之一。AtomUI 对滚动视图的设计目标是：

1. **视觉一致性**：所有使用 ScrollViewer 的地方（列表、树、菜单、内容面板等）呈现统一的滚动条风格。
2. **不干扰内容**：滚动条应尽可能低调，不抢占内容空间，不遮挡关键信息。
3. **智能显隐**：根据使用场景，滚动条可以常驻显示，也可以在鼠标悬浮时自动出现。
4. **主题化**：滚动条的颜色、粗细、圆角等视觉属性均由 Design Token 控制，随主题（亮色/暗色）自动适配。

### 两种显示模式

AtomUI ScrollViewer 提供两种显示模式：

| 模式 | `IsLiteMode` | 行为描述 |
|---|---|---|
| **标准模式（Normal）** | `false`（默认） | 滚动条常驻显示，占据独立空间（不覆盖内容），滑块粗细为 `NormalModeThumbThickness`（默认 `SizeXS`） |
| **极简模式（Lite）** | `true` | 滚动条默认隐藏，鼠标进入 ScrollViewer 区域时淡入显示；滑块较细（`LiteModeThumbThickness = LineWidthBold`），悬浮或拖拽时膨胀为标准粗细；内容区域跨越滚动条空间，不为滚动条预留位置 |

### 自动隐藏机制

当 `AllowAutoHide = true` 时，ScrollViewer 会监听全局指针移动事件（通过 `IInputManager`），判断指针是否在 ScrollViewer 区域内：

- **鼠标进入区域**：滚动条不透明度渐变至 1.0（可见）
- **鼠标离开区域**：滚动条不透明度渐变至 0.0（隐藏）
- **滑块拖拽中**：即使鼠标离开区域，滚动条保持可见；拖拽结束后，如果鼠标不在区域内则隐藏

### 滚动条分隔符

当同时存在水平和垂直滚动条时，右下角会出现一个分隔面板（`ScrollBarsSeparator`），使用 `ColorBgContainer` 背景色。该面板在 `IsExpanded = true` 时显示，否则隐藏。

### Avalonia ScrollViewer 基础能力

AtomUI 的 `ScrollViewer` 继承自 Avalonia 框架的 `Avalonia.Controls.ScrollViewer`。理解 Avalonia ScrollViewer 的基础能力有助于理解 AtomUI 在其之上做了哪些扩展。

**Avalonia ScrollViewer 的核心职责：**

Avalonia 的 `ScrollViewer` 是一个专用容器控件，内部通过 `ScrollContentPresenter` 呈现子内容，并根据内容尺寸与可视区域的关系自动显示水平/垂直 `ScrollBar`。它支持滚动惯性（`IsScrollInertiaEnabled`）、延迟滚动（`IsDeferredScrollingEnabled`）、SnapPoints 对齐等高级特性。

**Avalonia ScrollViewer 提供的基础属性：**

| 属性 | 说明 |
|---|---|
| `HorizontalScrollBarVisibility` | 水平滚动条显示策略：`Disabled`、`Auto`（按需）、`Visible`（始终）、`Hidden`（隐藏但可滚动） |
| `VerticalScrollBarVisibility` | 垂直滚动条显示策略，选项同上 |
| `AllowAutoHide` | 是否允许滚动条在无交互时自动隐藏 |
| `IsScrollInertiaEnabled` | 是否启用触控/滚轮的滚动惯性效果 |
| `IsDeferredScrollingEnabled` | 延迟滚动模式，拖拽滑块时不实时滚动内容，释放后才更新 |
| `Offset` | 当前滚动偏移量（`Vector` 类型） |
| `Extent` | 内容的总尺寸 |
| `Viewport` | 可视区域尺寸 |
| `IsExpanded` | 滚动条是否处于展开状态（鼠标悬浮/拖拽时） |
| `BringIntoViewOnFocusChange` | 焦点变化时是否自动滚动到焦点元素 |

### AtomUI 的扩展设计

AtomUI `ScrollViewer` 在 Avalonia ScrollViewer 的基础上做了以下扩展：

| 扩展能力 | 实现方式 | 设计动机 |
|---|---|---|
| **极简模式** | `IsLiteMode` 附加属性 + 主题样式切换 | 提供更轻量、不干扰内容的滚动条体验 |
| **自定义 ScrollBar** | `atom:ScrollBar` 替代原生 `ScrollBar` | 统一滚动条视觉风格，支持 LiteMode 和 Token 系统 |
| **自定义 ScrollBarThumb** | `atom:ScrollBarThumb` 替代原生 `Thumb` | 支持方向感知、粗细动画、Token 驱动的背景色 |
| **滚动条透明度动画** | `ScrollBarOpacity` 内部属性 + `DoubleTransition` | 平滑的显隐过渡效果 |
| **全局指针监听** | 通过 `IInputManager` 订阅指针事件 | 精准判断指针是否在 ScrollViewer 区域内，控制滚动条显隐 |
| **Design Token** | `ScrollViewerToken` + `RegisterTokenResourceScope` | 所有视觉值从全局/组件级 Token 派生，支持主题切换 |
| **Avalonia 原生兼容** | `AvaScrollViewerTheme` + `AvaScrollBarTheme` | 同时为 Avalonia 原生 `ScrollViewer` / `ScrollBar` 提供主题，确保三方控件也有一致的滚动条风格 |
| **过渡动画** | `IMotionAwareControl` 接口 + `IsMotionEnabled` 属性 | 支持全局动画开关，可禁用滚动条的过渡效果 |

---

## 功能详解

### 极简模式（IsLiteMode）

极简模式是 AtomUI ScrollViewer 的特色功能，适用于侧边栏、导航菜单等不希望滚动条占据空间的场景：

- **滑块细线显示**：默认状态下，滑块以 `LiteModeThumbThickness`（= `SharedToken.LineWidthBold`，约 2-3px）的细线呈现
- **悬浮膨胀**：鼠标悬浮到滑块上时，滑块宽度/高度过渡到 `NormalModeThumbThickness`（= `SharedToken.SizeXS`，约 8px），方便拖拽操作
- **内容全覆盖**：内容区域的 Grid.ColumnSpan / Grid.RowSpan 被设为 2，使内容延伸到滚动条区域下方，滚动条叠加在内容之上

`IsLiteMode` 是一个**附加属性**（`AttachedProperty`），可以设置在 ScrollViewer 本身或使用 ScrollViewer 的其他控件上（例如 `atom:NavMenu`）：

```xml
<!-- 设置在 ScrollViewer 上 -->
<atom:ScrollViewer IsLiteMode="True">...</atom:ScrollViewer>

<!-- 通过附加属性设置在其他控件上 -->
<atom:NavMenu atom:ScrollViewer.IsLiteMode="True">...</atom:NavMenu>
```

### 自动隐藏（AllowAutoHide）

`AllowAutoHide` 控制滚动条是否在无交互时自动隐藏：

- **`AllowAutoHide = true`（默认）**：滚动条根据鼠标位置自动显隐，内容区域覆盖滚动条空间
- **`AllowAutoHide = false`**：滚动条常驻显示，滚动条占据独立空间

### 过渡动画（Transitions）

ScrollViewer 系列控件实现了 `IMotionAwareControl` 接口，支持以下过渡动画：

| 控件 | 过渡属性 |
|---|---|
| `ScrollViewer` | `ScrollBarsSeparatorOpacity`、`ScrollBarOpacity` |
| `ScrollBarThumb` | `Background`（颜色过渡）、`Width`（垂直方向粗细变化）、`Height`（水平方向粗细变化） |

动画在 `OnLoaded` 时配置，`OnUnloaded` 时清除。通过 `IsMotionEnabled` 属性可全局开关：

```xml
<atom:ScrollViewer IsMotionEnabled="False"><!-- 禁用动画 --></atom:ScrollViewer>
```

### Avalonia 原生控件主题兼容

AtomUI 不仅为自己的 `atom:ScrollViewer` 和 `atom:ScrollBar` 提供主题，还为 Avalonia 原生的 `ScrollViewer` 和 `ScrollBar` 提供了兼容主题（`AvaScrollViewerTheme.axaml` 和 `AvaScrollBarTheme.axaml`）。这意味着：

- 即使三方控件内部使用原生 Avalonia `ScrollViewer`，也会呈现 AtomUI 的滚动条风格
- 原生滚动条使用标准粗细，不支持 LiteMode
- 同样通过 `ScrollViewerToken` 获取颜色和间距值

---

## 与 Ant Design 规范的对齐程度

| 特性 | Ant Design (React) | AtomUI (Avalonia) | 对齐情况 |
|---|---|---|---|
| 滚动视图 | Ant Design 无独立 ScrollViewer 组件 | ✅ 独立控件 | — 不适用 |
| 自定义滚动条样式 | 部分组件（如 Table、List）内置虚拟滚动 | ✅ 统一的 Token 驱动滚动条风格 | ✅ 风格一致性更好 |
| 自动隐藏滚动条 | 由浏览器/操作系统控制 | ✅ `AllowAutoHide` + 极简模式 | ✅ 增强能力 |
| 滚动条颜色随主题 | CSS 有限支持 | ✅ Token 系统自动适配亮色/暗色主题 | ✅ 增强能力 |

---

## 继承关系

### ScrollViewer

```
Avalonia.Controls.Primitives.TemplatedControl
  └── Avalonia.Controls.ContentControl
        └── Avalonia.Controls.ScrollViewer
              └── AtomUI.Controls.Commons.AbstractScrollViewer (IMotionAwareControl)
                    └── AtomUI.Desktop.Controls.ScrollViewer
```

### ScrollBar

```
Avalonia.Controls.Primitives.TemplatedControl
  └── Avalonia.Controls.Primitives.RangeBase
        └── Avalonia.Controls.Primitives.ScrollBar
              └── AtomUI.Controls.Commons.AbstractScrollBar (IMotionAwareControl)
                    └── AtomUI.Desktop.Controls.ScrollBar
```

### ScrollBarThumb

```
Avalonia.Controls.Primitives.TemplatedControl
  └── Avalonia.Controls.Primitives.Thumb
        └── AtomUI.Controls.Commons.AbstractScrollBarThumb (IMotionAwareControl)
              └── AtomUI.Desktop.Controls.ScrollBarThumb
```

**各层级职责划分：**

| 层级 | 提供的能力 |
|---|---|
| `Avalonia.Controls.ScrollViewer` | 滚动逻辑、`ScrollContentPresenter`、滚动条可见性控制、惯性滚动、SnapPoints、`AllowAutoHide` |
| `AbstractScrollViewer` (AtomUI.Controls) | 过渡动画配置（`IMotionAwareControl`）、全局指针监听实现自动显隐、拖拽状态追踪、`ScrollBarOpacity` / `ScrollBarsSeparatorOpacity` 内部属性 |
| `ScrollViewer` (AtomUI.Desktop.Controls) | `IsLiteMode` 附加属性、Token 作用域注册 |
| `Avalonia.Controls.Primitives.ScrollBar` | 滚动条基础逻辑、`Value` / `Minimum` / `Maximum`、`ViewportSize`、`AllowAutoHide`、`IsExpanded` |
| `AbstractScrollBar` (AtomUI.Controls) | `IsEffectiveExpanded` 内部状态、`IMotionAwareControl` 接口、自动隐藏状态管理 |
| `ScrollBar` (AtomUI.Desktop.Controls) | `IsLiteMode` 属性、LiteMode 下的展开逻辑覆写、Token 作用域注册 |
| `Avalonia.Controls.Primitives.Thumb` | 拖拽行为（`DragStarted` / `DragDelta` / `DragCompleted` 事件） |
| `AbstractScrollBarThumb` (AtomUI.Controls) | `Orientation` 属性、过渡动画（Background/Width/Height）、`IMotionAwareControl`、`IsExpanded` 内部属性 |
| `ScrollBarThumb` (AtomUI.Desktop.Controls) | `IsLiteMode` 属性 |

**实现的共享接口：**

| 接口 | 定义位置 | 实现控件 | 作用 |
|---|---|---|---|
| `IMotionAwareControl` | `AtomUI.Controls` | `AbstractScrollViewer`、`AbstractScrollBar`、`AbstractScrollBarThumb` | 支持 `IsMotionEnabled` 属性，全局动画开关 |

---

## 源码位置

### 基类层（AtomUI.Controls）—— 设备无关

| 文件路径 | 说明 |
|---|---|
| `src/AtomUI.Controls/ScrollViewer/AbstractScrollViewer.cs` | 滚动视图抽象基类：动画、自动隐藏逻辑、指针监听 |
| `src/AtomUI.Controls/ScrollViewer/AbstractScrollBar.cs` | 滚动条抽象基类：展开状态管理、动画开关 |
| `src/AtomUI.Controls/ScrollViewer/AbstractScrollBarThumb.cs` | 滚动条滑块抽象基类：方向感知、粗细/背景色过渡动画 |
| `src/AtomUI.Controls/ScrollViewer/ScrollBarReflectionExtensions.cs` | 内部工具：通过反射访问 Avalonia ScrollBar 的私有成员 |
| `src/AtomUI.Controls/ScrollViewer/ScrollBarRepeatButton.cs` | 滚动条翻页按钮（透明、不可聚焦） |

### 平台层（AtomUI.Desktop.Controls）—— 桌面端具体实现

| 文件路径 | 说明 |
|---|---|
| `src/AtomUI.Desktop.Controls/ScrollViewer/ScrollViewer.cs` | 桌面端 ScrollViewer：`IsLiteMode` 附加属性、Token 注册 |
| `src/AtomUI.Desktop.Controls/ScrollViewer/ScrollBar.cs` | 桌面端 ScrollBar：`IsLiteMode` 属性、LiteMode 展开逻辑覆写 |
| `src/AtomUI.Desktop.Controls/ScrollViewer/ScrollBarThumb.cs` | 桌面端 ScrollBarThumb：`IsLiteMode` 属性 |
| `src/AtomUI.Desktop.Controls/ScrollViewer/ScrollViewerToken.cs` | 组件级 Design Token |
| `src/AtomUI.Desktop.Controls/ScrollViewer/Themes/ScrollViewerTheme.axaml` | AtomUI ScrollViewer 主题 |
| `src/AtomUI.Desktop.Controls/ScrollViewer/Themes/ScrollBarTheme.axaml` | AtomUI ScrollBar 主题 |
| `src/AtomUI.Desktop.Controls/ScrollViewer/Themes/ScrollBarThumbTheme.axaml` | AtomUI ScrollBarThumb 主题 |
| `src/AtomUI.Desktop.Controls/ScrollViewer/Themes/ScrollBarThumbTheme.cs` | ScrollBarThumb 主题 code-behind |
| `src/AtomUI.Desktop.Controls/ScrollViewer/Themes/AvaScrollViewerTheme.axaml` | Avalonia 原生 ScrollViewer 兼容主题 |
| `src/AtomUI.Desktop.Controls/ScrollViewer/Themes/AvaScrollBarTheme.axaml` | Avalonia 原生 ScrollBar 兼容主题 |
| `src/AtomUI.Desktop.Controls/ScrollViewer/Themes/ScrollBarRepeatButtonTheme.axaml` | ScrollBarRepeatButton 主题 |
| `src/AtomUI.Desktop.Controls/ScrollViewer/Themes/ScrollViewerThemeConstants.cs` | 模板部件名称常量 |
| `src/AtomUI.Desktop.Controls/ScrollViewer/Themes/ScrollViewerThemes.axaml` | 主题资源字典汇总注册 |

---

## 模板结构

### ScrollViewer 模板

```
Grid (ColumnDefinitions="*,Auto", RowDefinitions="*,Auto")
├── ScopeAwareOverlayLayerPanel (Grid.Row=0, Grid.Column=0)
│   └── ScrollContentPresenter (PART_ContentPresenter)  ← 内容呈现器
│       └── ScrollGestureRecognizer                      ← 触控/滚轮手势识别
├── ScrollBar#PART_HorizontalScrollBar                   ← 水平滚动条 (Grid.Row=1, Grid.Column=0)
├── ScrollBar#PART_VerticalScrollBar                     ← 垂直滚动条 (Grid.Row=0, Grid.Column=1)
└── Panel#ScrollBarsSeparator                            ← 右下角分隔面板 (Grid.Row=1, Grid.Column=1)
```

**布局说明：**
- 内容区域位于左上（0,0），水平滚动条位于下方（1,0），垂直滚动条位于右侧（0,1），分隔面板位于右下角（1,1）
- 当 `AllowAutoHide = true` 或 `IsLiteMode = true` 时，内容区域的 `Grid.ColumnSpan` 和 `Grid.RowSpan` 被设为 2，使内容延伸到滚动条区域

### ScrollBar 模板

```
Border#Frame
└── Grid#RootLayout
    └── Track (PART_Track)
        ├── Track.DecreaseButton → ScrollBarRepeatButton (PART_PageUpButton)    ← 向上翻页区域
        ├── Track.Thumb → ScrollBarThumb (PART_Thumb)                           ← 滑块
        └── Track.IncreaseButton → ScrollBarRepeatButton (PART_PageDownButton)  ← 向下翻页区域
```

### ScrollBarThumb 模板

```
Border#Frame (Background, CornerRadius, Margin)  ← 滑块可视外观
```

### 模板部件常量

| 常量名 | 值 | 说明 |
|---|---|---|
| `ScrollViewerThemeConstants.PageUpButtonPart` | `"PART_PageUpButton"` | 向上/向左翻页按钮 |
| `ScrollViewerThemeConstants.PageDownButtonPart` | `"PART_PageDownButton"` | 向下/向右翻页按钮 |
| `ScrollViewerThemeConstants.TrackPart` | `"PART_Track"` | 滚动条轨道 |
| `ScrollViewerThemeConstants.ThumbPart` | `"PART_Thumb"` | 滚动条滑块 |
