# Window 窗口

## 概述

Window 是 AtomUI 主题化的窗口控件，提供了自定义标题栏、多层框架（Frame Layer）背景、响应式媒体查询（Media Query）、操作系统感知、窗口拖拽与缩放等能力。它是桌面应用程序的顶层容器，所有 AtomUI 控件都运行在 `Window` 或其子类 `ReactiveWindow<TViewModel>` 之内。

这是 AtomUI 的扩展控件，在 Ant Design 中没有直接对应。其设计灵感部分来源于 [SukiUI](https://github.com/kikipoulet/SukiUI) 项目。

---

## 设计原理

### 窗口控件的设计目标

桌面应用程序与 Web 应用最大的区别之一在于窗口管理。AtomUI 的 Window 控件旨在提供：

1. **统一的主题化外观**：窗口背景、前景色通过 Design Token 系统控制，自动适配亮色/暗色主题切换。
2. **自定义标题栏**：替代操作系统原生标题栏，提供可配置的 Logo、标题、标题按钮（关闭/最大化/最小化/全屏/置顶）。
3. **多层框架背景**：支持 Window 整体框架层、标题栏框架层、内容区框架层的独立背景设置，可用于实现渐变、图片背景等高级视觉效果。
4. **响应式布局**：内置媒体查询系统，基于窗口内容区域宽度自动计算断点，支持响应式 UI 布局。
5. **跨平台适配**：自动识别 Windows/macOS/Linux 平台，针对各平台进行标题栏、边框、装饰等差异化处理。

### Avalonia Window 基础能力

AtomUI 的 `Window` 继承自 Avalonia 框架的 `Avalonia.Controls.Window`。理解 Avalonia Window 的基础能力有助于理解 AtomUI 在其之上做了哪些扩展。

**Avalonia Window 的核心职责：**

Avalonia 的 `Window` 是一个顶层容器控件（`TopLevel`），它负责管理操作系统级别的窗口生命周期、大小、位置、状态（最小化/最大化/全屏/正常）以及系统装饰（标题栏、边框）。其继承链为：

```
Control → TemplatedControl → ContentControl → TopLevel → WindowBase → Window
```

**Avalonia Window 提供的基础属性：**

| 属性 | 说明 |
|---|---|
| `Title` | 窗口标题文本 |
| `Icon` | 窗口图标 |
| `WindowState` | 窗口状态：`Normal`、`Minimized`、`Maximized`、`FullScreen` |
| `WindowStartupLocation` | 启动位置：`Manual`、`CenterScreen`、`CenterOwner` |
| `CanResize` | 是否允许调整窗口大小 |
| `CanMinimize` / `CanMaximize` | 是否允许最小化/最大化 |
| `SystemDecorations` | 系统装饰风格：`None`、`BorderOnly`、`Full` |
| `ExtendClientAreaToDecorationsHint` | 是否将客户区扩展到标题栏装饰区域 |
| `TransparencyLevelHint` | 透明度级别提示 |
| `Content` | 窗口内容 |
| `Topmost` | 是否置顶显示 |
| `ShowInTaskbar` | 是否在任务栏显示 |

### AtomUI 的扩展设计

AtomUI `Window` 在 Avalonia Window 的基础上做了以下扩展：

| 扩展能力 | 实现方式 | 设计动机 |
|---|---|---|
| **自定义标题栏** | `WindowTitleBar` 控件 + `IsTitleBarVisible` 属性 | 替代原生标题栏，提供统一的跨平台外观和可配置性 |
| **多层框架背景** | `WindowFrameLayer` / `ContentFrameLayer` / `TitleBarFrameLayer` 属性 | 支持为窗口不同区域设置独立的背景内容，实现渐变/图片/动画背景 |
| **响应式媒体查询** | `WindowMediaQueryIndicator` + `MediaBreakPoint` + Container Query | 基于内容区域宽度自动计算断点，驱动响应式布局 |
| **操作系统感知** | `IOperationSystemAware` 接口 + `OsType` 属性 | 自动识别平台，针对不同 OS 应用差异化样式 |
| **窗口缩放控制** | `WindowResizer` 控件（Linux 专用） | 在 Linux 无系统装饰环境下提供自定义缩放手柄 |
| **屏幕比例限制** | `MaxWidthScreenRatio` / `MaxHeightScreenRatio` | 按屏幕工作区比例约束窗口最大尺寸 |
| **标题按钮配置** | `IsFullScreenCaptionButtonEnabled` / `IsPinCaptionButtonEnabled` / `IsCloseCaptionButtonEnabled` | 灵活控制标题栏功能按钮的显示/隐藏 |
| **窗口拖拽控制** | `IsMoveEnabled` 属性 | 可禁用标题栏拖拽移动功能 |
| **macOS 原生集成** | `MacOSCaptionGroupOffset` / `MacOSCaptionGroupSpacing` | 精确控制 macOS 红绿灯按钮的位置和间距 |
| **ReactiveUI 集成** | `ReactiveWindow<TViewModel>` 泛型子类 | 原生支持 MVVM 模式，ViewModel 自动激活 |
| **Design Token** | `WindowToken` + Token 资源系统 | 窗口背景/前景/圆角等视觉值从全局 Token 派生 |
| **全局对话框支持** | 模板内嵌 `DialogLayerManager` + `GlobalDialogManager` | 窗口级别的对话框/模态层管理 |

---

## 功能详解

### 自定义标题栏（WindowTitleBar）

Window 默认使用 `WindowTitleBar` 控件替代操作系统原生标题栏。标题栏通过 `ExtendClientAreaToDecorationsHint = True` 将客户区扩展到装饰区域，实现完全自定义的标题栏外观。

标题栏提供以下功能：
- **Logo**：通过 `Window.Logo` 属性设置，通常为应用程序图标
- **标题文本**：通过 `Window.Title` 属性设置，字号和字重由 `TitleFontSize` / `TitleFontWeight` 控制
- **标题按钮组**：关闭、最小化、最大化/还原、全屏、置顶按钮
- **双击最大化/还原**：双击标题栏在 Normal 和 Maximized 之间切换
- **拖拽移动**：鼠标在标题栏按住拖拽以移动窗口，超过 `DragThreshold` 阈值后触发

标题栏可通过 `IsTitleBarVisible = false` 完全隐藏。

### 多层框架背景（Frame Layers）

Window 模板支持三个独立的背景层，每层可设置内容、模板和不透明度：

| 框架层 | 范围 | 属性 |
|---|---|---|
| **WindowFrameLayer** | 覆盖整个窗口区域（标题栏 + 内容区） | `WindowFrameLayer` / `WindowFrameLayerTemplate` / `WindowFrameLayerOpacity` |
| **TitleBarFrameLayer** | 仅覆盖标题栏区域 | `TitleBarFrameLayer` / `TitleBarFrameLayerTemplate` / `TitleBarFrameLayerOpacity` / `TitleBarFrameBackground` |
| **ContentFrameLayer** | 仅覆盖内容区域 | `ContentFrameLayer` / `ContentFrameLayerTemplate` / `ContentFrameLayerOpacity` / `ContentFrameBackground` |

通过这些框架层，可以实现渐变背景、图片背景、动画背景等丰富的视觉效果，而不影响窗口内容的正常渲染。

### 响应式媒体查询（Media Query）

Window 内置了响应式媒体查询系统，基于 Avalonia Container Query 机制实现。内容区域的 `Border#ContentFrame` 被标记为 Container Query 容器，`WindowMediaQueryIndicator` 根据容器宽度自动更新断点值：

| 断点名称 | 容器宽度范围 | 枚举值 |
|---|---|---|
| `ExtraSmall` | ≤ 575px | `MediaBreakPoint.ExtraSmall` |
| `Small` | ≥ 576px | `MediaBreakPoint.Small` |
| `Medium` | ≥ 768px | `MediaBreakPoint.Medium` |
| `Large` | ≥ 992px（默认） | `MediaBreakPoint.Large` |
| `ExtraLarge` | ≥ 1200px | `MediaBreakPoint.ExtraLarge` |
| `ExtraExtraLarge` | ≥ 1600px | `MediaBreakPoint.ExtraExtraLarge` |

断点变化时会更新 `Window.MediaBreakPoint` 属性并触发 `MediaBreakPointChanged` 事件，子控件可通过实现 `IMediaBreakAwareControl` 接口响应布局变化。

### 操作系统感知

Window 实现了 `IOperationSystemAware` 接口，在构造时自动检测当前操作系统并设置 `OsType` 属性。主题系统通过 `[OsType=Windows]`、`[OsType=Linux]`、`[OsType=macOS]` 属性选择器应用平台差异化样式：

| 平台 | 差异化行为 |
|---|---|
| **Windows** | `ExtendClientAreaTitleBarHeightHint = 1`，最大化时自动添加 7px Margin 避免边缘溢出 |
| **Linux** | `SystemDecorations = None`，`ExtendClientAreaChromeHints = NoChrome`，启用自定义 `WindowResizer`，窗口带 1px 边框和 12px 圆角 |
| **macOS** | 使用原生红绿灯按钮（通过 `MacOSCaptionGroupOffset`/`MacOSCaptionGroupSpacing` 定位），支持通过原生 API 控制关闭按钮可用性 |

### 窗口缩放器（WindowResizer）

`WindowResizer` 是一个仅在 Linux 平台可见的内部控件，提供 8 个方向的缩放手柄（N/S/E/W/NW/NE/SW/SE）。由于 Linux 下 `SystemDecorations = None`，没有系统提供的缩放边框，因此由 AtomUI 自行绘制透明的缩放区域。

### ReactiveWindow\<TViewModel\>

`ReactiveWindow<TViewModel>` 是 Window 的 ReactiveUI 集成版本，实现了 `IViewFor<TViewModel>` 接口。它提供：

- `ViewModel` 属性与 `DataContext` 的自动双向同步
- 如果 ViewModel 实现了 `IActivatableViewModel`，窗口显示时自动激活 ViewModel
- 完整的 `WhenActivated` 生命周期管理

---

## 继承关系

```
Avalonia.Controls.Primitives.TemplatedControl
  └── Avalonia.Controls.ContentControl
        └── Avalonia.Controls.TopLevel
              └── Avalonia.Controls.WindowBase
                    └── Avalonia.Controls.Window
                          └── AtomUI.Desktop.Controls.Window
                                ├── implements IOperationSystemAware
                                ├── implements IMediaBreakAwareControl
                                ├── implements IDisposable
                                │
                                └── AtomUI.Desktop.Controls.ReactiveWindow<TViewModel>
                                      └── implements IViewFor<TViewModel>
```

`Window` 通过 `using AvaloniaWindow = Avalonia.Controls.Window;` 别名引用 Avalonia 原生 Window，避免类名冲突。

**各层级职责划分：**

| 层级 | 提供的能力 |
|---|---|
| `TopLevel` / `WindowBase` | 顶层容器管理、平台实现桥接、渲染启动、DPI 缩放 |
| `Avalonia.Controls.Window` | 窗口状态管理（Normal/Minimized/Maximized/FullScreen）、系统装饰、标题栏高度提示、透明度、启动位置、模态窗口、关闭逻辑 |
| `AtomUI.Desktop.Controls.Window` | 自定义标题栏、多层框架背景、响应式媒体查询、操作系统感知、屏幕比例约束、标题按钮配置、Design Token 集成、全局对话框管理 |
| `ReactiveWindow<TViewModel>` | ReactiveUI MVVM 集成、ViewModel 自动激活、DataContext ↔ ViewModel 同步 |

**实现的共享接口：**

| 接口 | 定义位置 | 作用 |
|---|---|---|
| `IOperationSystemAware` | `AtomUI.Controls.Shared` | 自动检测并暴露当前操作系统类型和版本 |
| `IMediaBreakAwareControl` | `AtomUI.Controls.Shared` | 提供 `MediaBreakPoint` 属性和 `MediaBreakPointChanged` 事件，支持响应式布局 |
| `IDisposable` | `System` | 窗口关闭时清理事件订阅等资源 |

---

## 源码位置

| 层级 | 文件路径 | 说明 |
|---|---|---|
| 控件类 | `src/AtomUI.Desktop.Controls/Window/Window.cs` | Window 主控件 |
| 控件类 | `src/AtomUI.Desktop.Controls/Window/ReactiveWindow.cs` | ReactiveUI 集成的泛型窗口 |
| 扩展方法 | `src/AtomUI.Desktop.Controls/Window/WindowExtensions.cs` | 窗口辅助方法（屏幕居中、尺寸约束等） |
| 内部控件 | `src/AtomUI.Desktop.Controls/Window/WindowMediaQueryIndicator.cs` | 媒体查询断点指示器 |
| 内部控件 | `src/AtomUI.Desktop.Controls/Window/WindowResizer.cs` | Linux 窗口缩放器 |
| 反射扩展 | `src/AtomUI.Desktop.Controls/Window/WindowBaseReflectionExtensions.cs` | WindowBase 反射辅助（ShowWithoutActive 等） |
| 转换器 | `src/AtomUI.Desktop.Controls/Window/Converters/WindowShadowsToPaddingConverter.cs` | BoxShadows → Thickness 转换 |
| Token 定义 | `src/AtomUI.Desktop.Controls/Window/WindowToken.cs` | 组件级 Design Token |
| 主题模板 | `src/AtomUI.Desktop.Controls/Window/Themes/WindowTheme.axaml` | ControlTheme AXAML |
| 主题代码 | `src/AtomUI.Desktop.Controls/Window/Themes/WindowTheme.cs` | 主题 code-behind |
| 模板常量 | `src/AtomUI.Desktop.Controls/Window/Themes/WindowThemeConstants.cs` | 模板部件名称常量 |
| 缩放器主题 | `src/AtomUI.Desktop.Controls/Window/Themes/WindowResizerTheme.axaml` | WindowResizer 主题 |
| 主题注册 | `src/AtomUI.Desktop.Controls/Window/Themes/WindowThemes.axaml` | ResourceDictionary 注册 |
| 标题栏 | `src/AtomUI.Desktop.Controls/Chrome/WindowTitleBar.cs` | 标题栏控件 |
| 标题栏 Token | `src/AtomUI.Desktop.Controls/Chrome/ChromeToken.cs` | 标题栏/标题按钮 Design Token |
| Gallery 示例 | `controlgallery/AtomUIGallery/Workspace/Views/WorkspaceWindow.axaml` | Gallery 主窗口使用范例 |

---

## 模板结构

Window 的 ControlTemplate 采用多层 Panel 布局，整体结构如下：

```
Panel
├── Border#WindowFrame                          ← 窗口主框架（承载背景、边框、圆角）
│   └── Panel
│       ├── ContentPresenter#WindowFrameLayer    ← 窗口级框架背景层（可选，覆盖整个窗口）
│       └── Border (ClipToBounds)
│           └── DialogLayerManager               ← 对话框/模态层管理器
│               └── Panel
│                   ├── GlobalDialogManager      ← 全局对话框管理器
│                   └── DockPanel#PART_ContentLayout  ← 内容布局容器
│                       ├── Panel (Dock=Top, 标题栏区域)
│                       │   ├── ContentPresenter#TitleBarFrameLayer  ← 标题栏框架背景层
│                       │   └── ContentPresenter (TitleBar)          ← 标题栏控件
│                       └── Panel (内容区域)
│                           ├── ContentPresenter#ContentFrameLayer   ← 内容区框架背景层
│                           └── Border#ContentFrame                  ← 内容框架（Container Query 容器）
│                               └── Panel
│                                   ├── ContentPresenter#PART_ContentPresenter  ← 窗口内容展示
│                                   └── WindowMediaQueryIndicator#PART_MediaQueryIndicator  ← 媒体查询指示器
└── WindowResizer#PART_WindowResizer             ← 窗口缩放手柄（仅 Linux 可见）
```

**分层设计理由：**
- **框架背景层独立**：`WindowFrameLayer`、`TitleBarFrameLayer`、`ContentFrameLayer` 作为独立的 `ContentPresenter` 层叠在内容之下，可放置图片、渐变等装饰内容而不影响交互。
- **对话框层嵌入**：`DialogLayerManager` 包裹整个内容区域，确保对话框和模态遮罩可以覆盖标题栏和内容。
- **媒体查询容器**：`Border#ContentFrame` 被标记为 Container Query 容器（`Container.Name` + `Container.Sizing`），`WindowMediaQueryIndicator` 通过 Container Query 机制自动响应宽度变化。
- **缩放器叠加**：`WindowResizer` 位于最顶层，覆盖窗口四边和四角，提供透明的缩放交互区域。

### 模板部件常量

| 常量名 | 值 | 说明 |
|---|---|---|
| `WindowThemeConstants.ContentPresenterPart` | `"PART_ContentPresenter"` | 内容展示器 |
| `WindowThemeConstants.WindowResizerPart` | `"PART_WindowResizer"` | 窗口缩放手柄 |
| `WindowThemeConstants.TitleBarPart` | `"PART_TitleBar"` | 标题栏控件 |
| `WindowThemeConstants.ContentLayoutPart` | `"PART_ContentLayout"` | 内容布局面板 |
| `WindowThemeConstants.MediaQueryIndicatorPart` | `"PART_MediaQueryIndicator"` | 媒体查询指示器 |
