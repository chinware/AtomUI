# AtomUI 整体架构

AtomUI 是基于 Avalonia/.NET 的桌面与跨平台控件系统。整体架构按平台能力、运行时基础设施、共享契约、Control 包、
可选扩展包、编译期生成和 Gallery 工具层组织。

## 导航

- [架构基础](foundations/index.md)：依赖关系、运行平台、启动注册、构建和打包。
- [跨模块系统](systems/index.md)：主题、本地化、Control 基础设施、渲染和窗口系统的架构入口。
- [Control 基础设施](systems/control-infrastructure/index.md)：异步加载、过滤与响应式共享契约。
- [渲染系统](systems/rendering/index.md)：边框渲染与跨 VisualRoot 的视觉层规则。

## 架构分层

```mermaid
flowchart TD
    Native["AtomUI.Native\n原生平台窗口能力"]
    Localization["AtomUI.Localization\nCatalog、语言状态、Snapshot、Localizer"]
    Core["AtomUI.Core\n主题、Token、动画、MotionScene"]
    Shared["AtomUI.Controls.Shared\n控件共享契约与数据协调器"]
    Fonts["AtomUI.Fonts.*\n字体包"]
    Icons["AtomUI.Icons.*\n图标基础设施与图标资源"]
    Controls["AtomUI.Controls\n公共控件、Primitives、通用主题"]
    Desktop["AtomUI.Desktop.Controls\n桌面主控件包"]
    DataGrid["AtomUI.Desktop.Controls.DataGrid\n独立 DataGrid 包"]
    ColorPicker["AtomUI.Desktop.Controls.ColorPicker\n独立 ColorPicker 包"]
    Extras["AtomUI.Desktop.Controls.Extras\n稳定补充控件包"]
    Generator["AtomUI.Generator\nToken、主题资产与本地化源生成器"]
    GalleryBase["AtomUI.Toolkits.GalleryBase\nGallery 应用底座库"]
    Gallery["AtomUIGallery\n示例与展示宿主"]

    Native --> Core
    Generator -. analyzer .-> Localization
    Generator -. analyzer .-> Core
    Generator -. analyzer .-> Shared
    Generator -. analyzer .-> Controls
    Generator -. analyzer .-> Desktop
    Generator -. analyzer .-> DataGrid
    Generator -. analyzer .-> ColorPicker
    Generator -. analyzer .-> Extras
    Core --> Localization
    Core --> Shared
    Core --> Icons
    Core --> Fonts
    Shared --> Controls
    Fonts --> Controls
    Icons --> Controls
    Controls --> Desktop
    Desktop --> GalleryBase
    Desktop --> DataGrid
    Desktop --> ColorPicker
    Desktop --> Extras
    GalleryBase --> Gallery
    Desktop --> Gallery
    DataGrid --> Gallery
    ColorPicker --> Gallery
```

## 核心运行链路

AtomUI 应用通常分两步接入：

1. 在 `AppBuilder` 上调用 `WithAtomUIDefaultOptions()` 应用平台默认配置。
2. 在 `Application.Initialize()` 内调用 `UseAtomUI(builder => ...)` 注册主题、本地化、字体、Control 包和可选包。

根 `IAtomUIBuilder` 组合主题与本地化 Builder。主题链路收集生成式 Control descriptor、主题 Provider、算法
descriptor 和 ControlTheme asset manifest；本地化链路收集 Catalog、编译后 Translation Bundle 与支持语言配置。
启动注册的完整顺序见 [启动与注册链路](foundations/startup-and-registration.md)。

## 源码包边界

- `AtomUI.Native` 提供原生平台窗口能力，不决定上层 Control 和主题策略。
- `AtomUI.Localization` 提供应用级本地化运行时。
- `AtomUI.Core` 提供主题、Token、资源、动画和应用入口基础设施。
- `AtomUI.Controls.Shared` 提供跨 Control 共享契约和数据协调能力。
- `AtomUI.Controls` 提供公共 Control 与 Primitives。
- `AtomUI.Desktop.Controls` 提供桌面主 Control 包、Popup、Overlay 和 Window 能力。
- DataGrid、ColorPicker 和 Extras 是按需引用的独立桌面包。
- `AtomUI.Generator` 以 Analyzer 方式提供静态 descriptor、资源键、注册和本地化输出。
- `AtomUI.Toolkits.GalleryBase` 为 Gallery 应用提供产品中立的工具层。

具体项目引用与内部可见性见 [项目依赖关系](foundations/dependency-graph.md)。

## 横切系统

- 主题、Token、Semantic Part 和本地化属于跨模块系统，由 `architecture/systems/` 统一导航。
- 平台差异遵守 [运行平台策略](foundations/runtime-platforms.md)。
- Control 边框遵守 [边框渲染架构](systems/rendering/border-rendering.md)。
- 跨普通视觉树绘制遵守 [视觉层规范](systems/rendering/visual-layers.md)。
- AOT、反射、动态数据和生成器规则遵守 [AOT 编程规范](../engineering/aot-programming-guidelines.md)。
