# AtomUI.Desktop.Controls 内部架构

桌面控件包按控件目录组织。多数控件目录包含控件类、Token、主题、可选转换器、数据加载和本地化资源。

## 控件组成模式

一个典型桌面控件由以下部分组成：

| 部分 | 说明 |
|---|---|
| 控件类 | 继承 Avalonia 控件或 AtomUI 抽象控件，声明 StyledProperty、事件和行为 |
| Token 类 | 可选；仅在 Control 存在 Own Token 时使用无参数 `[ControlDesignToken]` 并继承 `AbstractControlDesignToken`，不得继承另一个 Control Token |
| AXAML 主题 | 位于 `Themes/`，定义 ControlTheme、模板和样式 |
| 主题注册 | 生成器从 `Themes/**/*.axaml` 产生 asset owner/reference manifest，包级 Provider 按平台接入主题 |
| 本地化 Catalog | enum 与 XLIFF 位于 `Localization/`，由生成器编译为 Catalog descriptor 和内置 Translation Bundle |
| 辅助类型 | Converters、DataLoad、EventArgs、PseudoClass、ReflectionExtensions 等 |

## 横向基础系统

- Popup/Overlay：`Popup/`、`Flyouts/`、`Dialog/OverlayHost/`、`Primitives/OverlayLayerResolver.cs`。
- Window：`Window/`、`WindowTitleBar/`，并依赖 Native 和 Avalonia 平台能力。
- 媒体断点：`Window/MediaBreakPointThemeBootstrapper.cs` 与 `AtomUI.Controls.Shared/MediaQuery/` 配合，响应式规则见 [../controls-shared/responsive-system.md](../controls-shared/responsive-system.md)。
- 异步加载：AutoComplete、Mentions、Select、Cascader、TreeView 等控件使用共享协调器。
- 过滤：Select、Cascader、TreeView、Transfer、ListBox/ListView 等控件共享过滤契约。

## Semantic Part 集成

采用 Semantic Part 的桌面控件必须实现 [Semantic Part 系统设计](../../architecture/systems/theming/semantic-parts.md) 定义的公共契约，
不为桌面平台建立第二套 Part 命名或样式机制。

- ControlTemplate 中的稳定视觉区域使用 `.semantic-*` class；`PART_*` 继续只服务于控件实现查找。
- Desktop 与 Browser 主题变体必须提供相同 Part、ContractType 和 cardinality。
- 父 Control 的 Selector 最多进入自身模板一个 `/template/` 边界，不穿透子 ControlTemplate。
- Popup 模板节点和内容优先使用 Selector。`PopupRoot` 与 `OverlayPopupHost` 都必须验证 owner 样式作用域可达。
- `CrossVisualRoot` 只进入 descriptor 和测试矩阵，不自动产生 `PopupPresenterTheme` 或 `ItemContainerTheme`。
- ContextMenu、Flyout、Dialog、Message、Notification 等独立宿主必须显式确定 Semantic owner、ThemeContext 和
  StyleHost 来源。
- 虚拟化 ItemContainer 在 create、prepare、clear 和 recycle 全生命周期保持 semantic class 与 owner 一致。
- 强类型 Semantic Part Theme 只用于允许完整替换的真实 public 子 Control。

## 与具体控件文档的关系

本文件描述源码组织和内部系统。具体控件 API、使用方式、行为说明应写入 `docs/controls/desktop/` 下对应分类文件。
