# NavMenu 导航菜单

## 概述

导航菜单（NavMenu）是网站和应用中最常见的导航交互控件之一，用于为用户提供清晰的页面导航路径。它支持**水平**（Horizontal）、**垂直**（Vertical）和**内嵌**（Inline）三种展示模式，并可在亮色和暗色主题之间切换。

AtomUI 的 `NavMenu` 控件对标 [Ant Design 5.0 Menu（导航菜单）](https://ant.design/components/menu-cn) 的设计规范，在 .NET / Avalonia 平台上提供一致的视觉体验和交互行为。

---

## 设计原理

### Ant Design 的导航菜单设计哲学

Ant Design 导航菜单的核心定位是：**为页面和功能提供导航的菜单列表，帮助用户在不同层级的内容之间快速跳转**。菜单支持多级层级结构，用户可以展开/收起子菜单来浏览不同层级的导航项。

**三种菜单模式**（按场景区分）：

| 模式 | 设计意图 | 典型场景 |
|---|---|---|
| 🔹 **水平模式（Horizontal）** | 水平排列的一级菜单项，子菜单通过弹出层展示。适合顶部导航栏 | 网站/应用的顶部全局导航 |
| 🔸 **垂直模式（Vertical）** | 垂直排列的菜单项，子菜单通过弹出层（Popup）在侧边展示 | 侧边栏导航（弹出式子菜单） |
| 🔻 **内嵌模式（Inline）** | 垂直排列的菜单项，子菜单在当前面板内展开/折叠，具有展开/收起动画 | 侧边栏导航（折叠式子菜单） |

**关键交互特性**：

| 特性 | 说明 |
|---|---|
| **选中高亮** | 当前选中的菜单项高亮显示，祖先路径上的父级菜单项也会标记为「选中路径」 |
| **手风琴模式** | 同级菜单项中只能有一个展开，展开新子菜单时自动关闭其他已展开的兄弟菜单 |
| **默认展开/选中** | 支持通过路径指定初始展开的子菜单和默认选中的菜单项 |
| **暗色主题** | 支持独立的暗色主题样式，常用于深色背景的侧边导航栏 |
| **图标支持** | 菜单项可配置图标，提升可识别性 |
| **禁用状态** | 单个菜单项可设为禁用，不响应交互 |

### 与 Avalonia 的关系

AtomUI 的 `NavMenu` **不继承自 Avalonia 内置的 `Menu` 控件**，而是直接继承自 `ItemsControl`。这是因为 Ant Design 的导航菜单与传统的应用程序菜单（File/Edit/View 等）有本质区别——它是一个页面级的导航组件，需要支持选中状态跟踪、路径高亮、内嵌展开动画等 Avalonia `Menu` 不具备的功能。

`NavMenu` 使用自定义的 `NavMenuItem`（继承自 `HeaderedSelectingItemsControl`）作为菜单项容器，实现了完整的自定义交互逻辑。

### AtomUI NavMenu 的核心设计

| 设计要素 | 实现方式 | 设计动机 |
|---|---|---|
| **三种菜单模式** | `NavMenuMode` 枚举 + 伪类驱动 + 交互处理器切换 | 不同模式对应不同的布局策略和子菜单展示方式 |
| **交互处理器策略** | `INavMenuInteractionHandler` 接口 + `DefaultNavMenuInteractionHandler` / `InlineNavMenuInteractionHandler` | 将交互逻辑从控件中解耦，不同模式使用不同的处理器 |
| **树形数据模型** | `INavMenuNode` / `NavMenuNode` + `ITreeNode<T>` | 支持层级数据结构，每个节点可包含子节点 |
| **选中路径追踪** | `IsInSelectedPath` 属性 + `CollectSelectPathItems` | 从选中项向上追溯到根节点，标记整条路径上的祖先菜单项 |
| **默认展开/选中** | `DefaultOpenPaths` / `DefaultSelectedPath` + `TreeNodePath` | 通过路径字符串指定初始状态 |
| **手风琴模式** | `IsAccordionMode` 属性 + `NotifySubmenuOpened` 互斥逻辑 | 同级只允许一个子菜单展开 |
| **亮色/暗色主题** | `IsDarkStyle` 属性 + `:dark` / `:light` 伪类 | 通过 Token 系统提供完整的暗色主题 |
| **展开/折叠动画** | Inline 模式下使用 `SlideUpInMotion` / `SlideUpOutMotion` | 平滑的子菜单展开/收起效果 |
| **Design Token** | `NavMenuToken` + 全局 `SharedToken` | 所有视觉属性从 Token 派生，支持主题定制 |

---

## 功能详解

### 菜单模式（NavMenuMode）

#### 水平模式（Horizontal）

菜单项水平排列成一行，通常用于页面顶部导航。子菜单通过 Popup 弹出层在菜单项下方展示。水平模式下选中项有底部指示条效果。

#### 垂直模式（Vertical）

菜单项垂直排列，子菜单通过 Popup 弹出层在菜单项右侧展示。鼠标悬浮时延迟展开子菜单，移出后延迟关闭。

#### 内嵌模式（Inline）

菜单项垂直排列，子菜单在当前面板内通过折叠/展开方式显示。点击有子菜单的菜单项会触发展开/收起动画（`SlideUpInMotion` / `SlideUpOutMotion`）。

**模式与交互处理器的对应关系：**

| 模式 | 交互处理器 | 子菜单展示方式 |
|---|---|---|
| Horizontal | `DefaultNavMenuInteractionHandler` | Popup 弹出层 |
| Vertical | `DefaultNavMenuInteractionHandler` | Popup 弹出层 |
| Inline | `InlineNavMenuInteractionHandler` | 就地展开/折叠 |

### 选中路径追踪

当用户选中一个叶子菜单项时，NavMenu 会：
1. 清除之前选中项及其祖先路径上的 `IsInSelectedPath` 标记
2. 为新选中项的所有祖先菜单项设置 `IsInSelectedPath = true`
3. 触发 `NavMenuNodeSelected` 事件，更新 `SelectedItem` 属性

这使得主题可以为整条选中路径上的菜单项应用高亮样式。

### 手风琴模式（Accordion）

当 `IsAccordionMode = true` 时，同一级别中只能有一个子菜单处于展开状态。当一个子菜单被展开时，其兄弟子菜单会自动关闭。

### 默认展开与默认选中

- **`DefaultOpenPaths`**：通过 `TreeNodePath` 列表指定初始展开的路径。路径格式为 `/Key1/Key2/Key3`，由各节点的 `ItemKey` 拼接而成。
- **`DefaultSelectedPath`**：指定初始选中的菜单项路径。
- **`SelectedItem`**：直接设置选中的 `INavMenuNode` 对象，优先级高于 `DefaultSelectedPath`。

### 亮色/暗色主题

通过 `IsDarkStyle` 属性切换菜单的视觉主题。暗色模式下使用一套独立的 Token（如 `DarkMenuBg`、`DarkItemColor`、`DarkItemSelectedBg` 等），提供深色背景下的完整样式支持。

---

## 与 Ant Design 规范的对齐程度

| 特性 | Ant Design (React) | AtomUI (Avalonia) | 对齐情况 |
|---|---|---|---|
| 三种模式 `mode` | ✅ `vertical / horizontal / inline` | ✅ `NavMenuMode` 枚举 | ✅ 完全对齐 |
| 暗色主题 `theme` | ✅ `light / dark` | ✅ `IsDarkStyle` 属性 | ✅ 完全对齐 |
| 选中项 `selectedKeys` | ✅ 数组 | ✅ `SelectedItem` 单选 | ⚠️ 仅支持单选 |
| 默认展开 `defaultOpenKeys` | ✅ 数组 | ✅ `DefaultOpenPaths` 路径列表 | ✅ 对齐（路径格式不同） |
| 默认选中 `defaultSelectedKeys` | ✅ 数组 | ✅ `DefaultSelectedPath` 单路径 | ⚠️ 仅支持单选 |
| 手风琴模式 | ✅ `openKeys` 受控 | ✅ `IsAccordionMode` 属性 | ✅ 对齐 |
| 图标支持 `icon` | ✅ ReactNode | ✅ `PathIcon` | ✅ 对齐 |
| 禁用项 `disabled` | ✅ 布尔 | ✅ `IsEnabled` | ✅ 完全对齐 |
| 菜单项分组 `ItemGroup` | ✅ 支持 | ❌ 暂未支持 | ⚠️ 待实现 |
| 菜单分割线 `Divider` | ✅ 支持 | ❌ 暂未支持 | ⚠️ 待实现 |
| 折叠收起 `inlineCollapsed` | ✅ 支持 | ❌ 暂未支持 | ⚠️ 待实现 |
| 点击事件 `onClick` | ✅ 回调 | ✅ `NavMenuItemClick` 事件 | ✅ 完全对齐 |
| 选中事件 `onSelect` | ✅ 回调 | ✅ `NavMenuNodeSelected` 事件 | ✅ 完全对齐 |
| `items` 配置式 | ✅ JSON 数组 | ✅ `NavMenuNode` 数据模型 | ✅ 对齐（形式不同） |

---

## 继承关系

### NavMenu

```
Avalonia.Controls.Primitives.TemplatedControl
  └── Avalonia.Controls.ItemsControl
        └── AtomUI.Desktop.Controls.NavMenu
              ├── implements IFocusScope
              ├── implements INavMenu
              ├── implements IMotionAwareControl
              └── implements IMenuChildSelectable (internal)
```

### NavMenuItem

```
Avalonia.Controls.Primitives.TemplatedControl
  └── Avalonia.Controls.ItemsControl
        └── Avalonia.Controls.Primitives.SelectingItemsControl
              └── Avalonia.Controls.Primitives.HeaderedSelectingItemsControl
                    └── AtomUI.Desktop.Controls.NavMenuItem (internal)
                          ├── implements INavMenuItem
                          ├── implements ISelectable
                          ├── implements ICommandSource
                          ├── implements IClickableControl
                          ├── implements ICustomHitTest
                          └── implements IMenuChildSelectable (internal)
```

### NavMenuNode（数据模型）

```
Avalonia.AvaloniaObject
  └── AtomUI.Desktop.Controls.NavMenuNode
        └── implements INavMenuNode
              └── extends ITreeNode<INavMenuNode>
```

**各层级职责划分：**

| 层级 | 提供的能力 |
|---|---|
| `ItemsControl` | 子项集合管理、容器化、`ItemsSource` 绑定、`ItemTemplate` 模板 |
| `NavMenu` | 三种模式切换、交互处理器调度、选中路径追踪、默认展开/选中、手风琴模式、亮色/暗色主题、Design Token 集成 |
| `NavMenuItem` | 单个菜单项行为：子菜单展开/关闭、命令绑定、选中状态、Popup 管理、内嵌展开动画、层级计算 |
| `NavMenuNode` | 数据模型：Header、Icon、ItemKey、子节点集合、父节点引用 |

---

## 核心类型

### 接口体系

| 接口 | 定义位置 | 职责 |
|---|---|---|
| `INavMenu` | `AtomUI.Desktop.Controls` | NavMenu 根控件接口，提供 SelectedItem、DefaultOpenPaths、Close 等能力 |
| `INavMenuElement` | `AtomUI.Desktop.Controls` | 菜单元素基接口，提供 SubItems 子项枚举 |
| `INavMenuItem` | `AtomUI.Desktop.Controls` | 菜单项接口，定义 HasSubMenu、IsSubMenuOpen、Open/Close 等行为 |
| `INavMenuNode` | `AtomUI.Desktop.Controls` | 菜单节点数据接口，继承自 `ITreeNode<INavMenuNode>` |
| `INavMenuInteractionHandler` | `AtomUI.Desktop.Controls` (internal) | 交互处理器接口，策略模式 |
| `IMenuChildSelectable` | `AtomUI.Desktop.Controls` (internal) | 子项选中管理接口 |

### 辅助类型

| 类型 | 说明 |
|---|---|
| `NavMenuNode` | `INavMenuNode` 的默认实现，可直接在 AXAML 中声明使用 |
| `NavNodeKey` | 菜单节点键值结构体，支持字符串比较 |
| `TreeNodePath` | 树节点路径，格式为 `/Key1/Key2`，用于指定默认展开/选中路径 |
| `NavMenuItemClickEventArgs` | 菜单项点击事件参数 |
| `NavMenuNodeSelectedEventArgs` | 菜单节点选中事件参数 |
| `BaseNavMenuItemHeader` | 菜单项头部基类，承载 Header、Icon、动画过渡等通用逻辑 |
| `HorizontalNavMenuItemHeader` | 水平模式专用的菜单项头部 |
| `VerticalNavMenuItemHeader` | 垂直模式专用的菜单项头部 |
| `InlineNavMenuItemHeader` | 内嵌模式专用的菜单项头部 |

---

## 源码位置

| 层级 | 文件路径 | 说明 |
|---|---|---|
| 控件类 | `src/AtomUI.Desktop.Controls/NavMenu/NavMenu.cs` | NavMenu 主控件 |
| 菜单项 | `src/AtomUI.Desktop.Controls/NavMenu/NavMenuItem.cs` | NavMenuItem 菜单项容器（internal） |
| 数据模型 | `src/AtomUI.Desktop.Controls/NavMenu/NavMenuNode.cs` | NavMenuNode 数据模型 + INavMenuNode 接口 |
| Token 定义 | `src/AtomUI.Desktop.Controls/NavMenu/NavMenuToken.cs` | 组件级 Design Token |
| 伪类常量 | `src/AtomUI.Desktop.Controls/NavMenu/NavMenuPseudoClass.cs` | 伪类定义 |
| 模式枚举 | `src/AtomUI.Desktop.Controls/NavMenu/NavMenuMode.cs` | NavMenuMode 枚举 |
| 事件参数 | `src/AtomUI.Desktop.Controls/NavMenu/NavMenuEventArgs.cs` | 事件参数类 |
| 接口定义 | `src/AtomUI.Desktop.Controls/NavMenu/INavMenu.cs` | INavMenu 接口 |
| 接口定义 | `src/AtomUI.Desktop.Controls/NavMenu/INavMenuElement.cs` | INavMenuElement 接口 |
| 接口定义 | `src/AtomUI.Desktop.Controls/NavMenu/INavMenuItem.cs` | INavMenuItem 接口 |
| 键结构体 | `src/AtomUI.Desktop.Controls/NavMenu/NavNodeKey.cs` | NavNodeKey 结构体 |
| 交互处理器 | `src/AtomUI.Desktop.Controls/NavMenu/DefaultNavMenuInteractionHandler.cs` | 默认（Vertical/Horizontal）交互处理器 |
| 交互处理器 | `src/AtomUI.Desktop.Controls/NavMenu/InlineNavMenuInteractionHandler.cs` | 内嵌模式交互处理器 |
| 头部控件 | `src/AtomUI.Desktop.Controls/NavMenu/Header/BaseNavMenuItemHeader.cs` | 菜单项头部基类 |
| 头部控件 | `src/AtomUI.Desktop.Controls/NavMenu/Header/HorizontalNavMenuItemHeader.cs` | 水平模式头部 |
| 头部控件 | `src/AtomUI.Desktop.Controls/NavMenu/Header/VerticalNavMenuItemHeader.cs` | 垂直模式头部 |
| 头部控件 | `src/AtomUI.Desktop.Controls/NavMenu/Header/InlineNavMenuItemHeader.cs` | 内嵌模式头部 |
| 主题模板 | `src/AtomUI.Desktop.Controls/NavMenu/Themes/NavMenuTheme.axaml` | NavMenu ControlTheme |
| 主题模板 | `src/AtomUI.Desktop.Controls/NavMenu/Themes/NavMenuItemTheme.axaml` | NavMenuItem ControlTheme |
| 主题模板 | `src/AtomUI.Desktop.Controls/NavMenu/Themes/BaseNavMenuItemHeaderTheme.axaml` | 基础头部主题 |
| 主题模板 | `src/AtomUI.Desktop.Controls/NavMenu/Themes/HorizontalNavMenuItemHeaderTheme.axaml` | 水平头部主题 |
| 主题模板 | `src/AtomUI.Desktop.Controls/NavMenu/Themes/VerticalNavMenuItemHeaderTheme.axaml` | 垂直头部主题 |
| 主题模板 | `src/AtomUI.Desktop.Controls/NavMenu/Themes/InlineNavMenuItemHeaderTheme.axaml` | 内嵌头部主题 |
| 模板常量 | `src/AtomUI.Desktop.Controls/NavMenu/Themes/NavMenuThemeConstants.cs` | 模板部件名称常量 |
| 工具类 | `src/AtomUI.Desktop.Controls/NavMenu/Utils/MarginMultiplierConverter.cs` | 缩进边距倍数转换器 |
| 工具类 | `src/AtomUI.Desktop.Controls/NavMenu/Utils/NavMenuIsSubMenuOpenConverter.cs` | 子菜单展开状态转换器 |
| Gallery 示例 | `controlgallery/AtomUIGallery/ShowCases/Views/Navigation/MenuShowCase.axaml` | 使用范例 |
| Gallery 代码 | `controlgallery/AtomUIGallery/ShowCases/Views/Navigation/MenuShowCase.axaml.cs` | 示例 code-behind |

---

## 模板结构

### NavMenu 模板

NavMenu 自身模板比较简单，主要是一个带 `ItemsPresenter` 的容器：

- **水平模式**：`ItemsPanel` 使用水平布局的 `StackPanel`，底部有一条指示线
- **垂直/内嵌模式**：`ItemsPanel` 使用垂直布局的 `StackPanel`

### NavMenuItem 模板

NavMenuItem 的模板根据模式不同有较大差异：

- **水平/垂直模式**：使用 `Popup` 弹出子菜单
- **内嵌模式**：使用 `BaseMotionActor`（`PART_ChildItemsLayoutTransform`）包裹子菜单 `ItemsPresenter`，通过 `SlideUpInMotion` / `SlideUpOutMotion` 实现展开/收起动画

每种模式都有对应的 `NavMenuItemHeader` 控件作为菜单项头部区域。

### 模板部件常量

| 常量名 | 值 | 说明 |
|---|---|---|
| `NavMenuItemThemeConstants.HeaderPart` | `"PART_Header"` | 菜单项头部区域 |
| `NavMenuItemThemeConstants.ItemsPresenterPart` | `"PART_ItemsPresenter"` | 子项展示器 |
| `NavMenuItemThemeConstants.PopupFramePart` | `"PART_PopupFrame"` | Popup 框架 |
| `NavMenuItemThemeConstants.PopupPart` | `"PART_Popup"` | Popup 弹出层 |
| `InlineNavMenuItemThemeConstants.ChildItemsLayoutTransformPart` | `"PART_ChildItemsLayoutTransform"` | 内嵌模式子项动画容器 |
| `NavMenuThemeConstants.ItemsPresenterPart` | `"PART_ItemsPresenter"` | NavMenu 子项展示器 |
| `NavMenuThemeConstants.HorizontalLinePart` | `"PART_HorizontalLine"` | 水平模式底部指示线 |
| `TopLevelHorizontalNavMenuItemThemeConstants.ActiveIndicatorPart` | `"PART_ActiveIndicator"` | 水平顶层菜单项的活跃指示器 |
