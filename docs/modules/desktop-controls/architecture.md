# AtomUI.Desktop.Controls 内部架构

桌面控件包按控件目录组织。多数控件目录包含控件类、Token、主题、可选转换器、数据加载和本地化资源。

## 控件组成模式

一个典型桌面控件由以下部分组成：

| 部分 | 说明 |
|---|---|
| 控件类 | 继承 Avalonia 控件或 AtomUI 抽象控件，声明 StyledProperty、事件和行为 |
| Token 类 | 继承 `AbstractControlDesignToken`，声明控件级设计 Token |
| AXAML 主题 | 位于 `Themes/`，定义 ControlTheme、模板和样式 |
| 主题聚合 | 通过 `DesktopControlThemesProvider.axaml` 汇总控件主题 |
| 语言 Provider | 位于 `Localization/`，由源生成器收集 |
| 辅助类型 | Converters、DataLoad、EventArgs、PseudoClass、ReflectionExtensions 等 |

## 横向基础系统

- Popup/Overlay：`Popup/`、`Flyouts/`、`Dialog/OverlayHost/`、`Primitives/OverlayLayerResolver.cs`。
- Window：`Window/`、`WindowTitleBar/`，并依赖 Native 和 Avalonia 平台能力。
- 媒体断点：`Window/MediaBreakPointThemeBootstrapper.cs` 与 `AtomUI.Controls.Shared/MediaQuery/` 配合。
- 异步加载：AutoComplete、Mentions、Select、Cascader、TreeView 等控件使用共享协调器。
- 过滤：Select、Cascader、TreeView、Transfer、ListBox/ListView 等控件共享过滤契约。

## 与具体控件文档的关系

本文件描述源码组织和内部系统。具体控件 API、使用方式、行为说明应写入 `docs/controls/desktop/` 下对应分类文件。

