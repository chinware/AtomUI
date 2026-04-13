# Menu 菜单

## 概述

菜单（Menu）用于为页面和功能提供导航菜单。AtomUI 的菜单控件家族包括三个核心组件：`Menu`（顶部菜单栏）、`MenuItem`（菜单项）和 `ContextMenu`（右键上下文菜单），以及辅助的 `MenuSeparator`（分隔线）和 `MenuFlyout`（弹出菜单）。

AtomUI 的 `Menu` 控件对应 [Ant Design 5.0 Menu](https://ant.design/components/menu-cn) 组件中的水平菜单模式，在 .NET / Avalonia 平台上提供一致的交互体验。

> **注意**：Ant Design 的 `Menu` 组件还包含 Inline（内嵌）和 Vertical（垂直弹出）模式，这些在 AtomUI 中由独立的 `NavMenu` 控件实现，详见 `docs/Desktop.Controls/NavMenu/` 文档。

---

## 设计原理

### Ant Design 的 Menu 设计哲学

Ant Design 的 Menu 定位为：**应用和页面的功能导航载体**。核心设计目标包括：

- **层级嵌套**：支持多级子菜单，通过弹出层展示下级菜单项
- **多种模式**：水平（horizontal）、垂直弹出（vertical）、内嵌展开（inline）
- **丰富的菜单项类型**：普通项、分组、分隔线、CheckBox/Radio 切换项
- **图标支持**：菜单项可包含前置图标
- **快捷键提示**：显示键盘快捷方式

### Avalonia 基础能力

AtomUI 的 `Menu`、`MenuItem`、`ContextMenu` 分别继承自 Avalonia 的同名控件，通过别名避免冲突：

```csharp
using AvaloniaMenu = Avalonia.Controls.Menu;
using AvaloniaMenuItem = Avalonia.Controls.MenuItem;
using AvaloniaContextMenu = Avalonia.Controls.ContextMenu;
```

**Avalonia Menu 体系的核心能力：**

| 能力 | 说明 |
|---|---|
| `Menu` | 顶部水平菜单栏，继承自 `MenuBase`，管理顶层菜单项 |
| `MenuItem` | 菜单项，继承自 `HeaderedSelectingItemsControl`，支持嵌套子项、图标、快捷键 |
| `ContextMenu` | 右键弹出菜单，绑定到控件的 `ContextMenu` 属性 |
| `MenuBase.IsOpen` | 菜单是否处于打开状态 |
| `MenuItem.IsSubMenuOpen` | 子菜单是否打开 |
| `MenuItem.ToggleType` | 支持 None/CheckBox/Radio 三种切换类型 |
| `MenuItem.InputGesture` | 快捷键手势绑定 |
| `MenuItem.Header` | 菜单项标题内容 |
| `MenuItem.Command` | 点击命令绑定 |

### AtomUI 的扩展设计

| 扩展能力 | 实现方式 | 设计动机 |
|---|---|---|
| **三种尺寸** | `ISizeTypeAware`（Large/Middle/Small） | 菜单栏适配不同布局密度 |
| **弹出动画** | `IMotionAwareControl` + Popup 动画 | 子菜单弹出/关闭的过渡动画 |
| **自定义图标** | `PathIcon` 类型的 `Icon` 属性（重写基类） | 使用 Ant Design 图标集 |
| **数据驱动** | `IMenuItemData` / `MenuItemData` / `MenuSeparatorData` | 支持通过数据模型和 `ItemsSource` 生成菜单结构 |
| **可滚动子菜单** | `DisplayPageSize` + `ScrollViewer` | 子菜单项过多时自动出现滚动 |
| **分隔线** | `MenuSeparator` + `MenuSeparatorData` | 自定义分隔线控件，从 Token 系统获取样式 |
| **CheckBox/Radio** | `ToggleType` + `IsCheckStateChanged` 事件 | 菜单项支持选中/取消选中交互 |
| **Design Token** | `MenuToken` + `RegisterTokenResourceScope` | 所有视觉值从 Token 系统派生 |
| **OverlayLayer** | `ShouldUseOverlayLayer` 属性 | 支持使用 OverlayLayer 作为 Popup 宿主 |

---

## 功能详解

### Menu（顶部菜单栏）

`Menu` 是水平排列的顶部菜单栏，包含多个顶层 `MenuItem`。每个顶层项可有子菜单，点击后以 Popup 弹出展示。

**关键行为：**
- 顶层菜单项使用 `TopLevelMenuItemTheme` 主题，样式与子菜单项不同
- 使用自定义 `DefaultMenuInteractionHandler` 处理菜单交互
- 关闭时支持动画（`IsMotionEnabled`），逐级关闭子菜单的 Popup

### MenuItem（菜单项）

`MenuItem` 是菜单的基本构建块。支持以下特性：

| 特性 | 说明 |
|---|---|
| 嵌套子菜单 | `MenuItem` 可包含子 `MenuItem`，子菜单以 Popup 弹出 |
| 图标 | `Icon` 属性（`PathIcon?`），与 Ant Design 图标集配合 |
| 快捷键 | `InputGesture` 属性，显示在菜单项右侧 |
| CheckBox/Radio 切换 | `ToggleType` 属性，支持 None/CheckBox/Radio |
| 分组 | `GroupName` 属性，用于 Radio 类型的互斥分组 |
| 禁用 | `IsEnabled = false`，灰色调显示 |
| 危险样式 | 通过 Token 支持危险色（`DangerItemColor`） |

### ContextMenu（右键菜单）

`ContextMenu` 通过右键触发弹出的浮动菜单。与 `Menu` 共享 `MenuToken`，使用相同的菜单项样式。

**关键行为：**
- 自动管理 Popup 生命周期
- 支持窗口失活时自动关闭
- 支持检测外部点击关闭
- 支持动画开关

### MenuSeparator（分隔线）

自定义分隔线控件，继承自 Avalonia `Separator`。在菜单中水平显示，用于分组菜单项。

### 数据驱动模式

通过 `IMenuItemData` 接口和 `MenuItemData` 类，支持使用数据模型 + `ItemTemplate` 生成菜单结构：

```csharp
public interface IMenuItemData : ITreeNode<IMenuItemData>
{
    KeyGesture? InputGesture { get; }
}
```

`MenuSeparatorData` 继承自 `MenuItemData`，当数据项为 `MenuSeparatorData` 类型时，自动创建 `MenuSeparator` 容器。

---

## 与 Ant Design 规范的对齐程度

| 特性 | Ant Design (React) | AtomUI (Avalonia) | 对齐情况 |
|---|---|---|---|
| 水平顶部菜单 | ✅ `mode="horizontal"` | ✅ `Menu`（水平菜单栏） | ✅ 对齐 |
| 垂直弹出菜单 | ✅ `mode="vertical"` | ✅ `NavMenu Mode="Vertical"` | ✅ 对齐（独立控件） |
| 内嵌展开菜单 | ✅ `mode="inline"` | ✅ `NavMenu Mode="Inline"` | ✅ 对齐（独立控件） |
| 子菜单嵌套 | ✅ `children` 属性 | ✅ `MenuItem` 嵌套子项 | ✅ 对齐 |
| 图标 | ✅ `icon` 属性 | ✅ `Icon`（`PathIcon`） | ✅ 对齐 |
| 禁用 | ✅ `disabled` | ✅ `IsEnabled` | ✅ 对齐 |
| 分隔线 | ✅ `type: "divider"` | ✅ `MenuSeparator` / `MenuSeparatorData` | ✅ 对齐 |
| 选中状态 | ✅ `selectedKeys` | ✅ `ToggleType` + `IsChecked` | ✅ 对齐 |
| 快捷键提示 | ❌ 无原生支持 | ✅ `InputGesture` | ✅ 超集 |
| CheckBox/Radio | ❌ 无原生支持 | ✅ `ToggleType` | ✅ 超集 |
| 数据驱动 | ✅ `items` 属性 | ✅ `ItemsSource` + `IMenuItemData` | ✅ 对齐 |
| 右键菜单 | ❌（使用 Dropdown） | ✅ `ContextMenu` | ✅ 扩展 |
| 弹出动画 | ✅ 内置动画 | ✅ `IsMotionEnabled` | ✅ 对齐 |
| 危险菜单项 | ✅ `danger` 属性 | ✅ `DangerItemColor` Token | ✅ 对齐 |

---

## 继承关系

### Menu

```
Avalonia.Controls.MenuBase
  └── Avalonia.Controls.Menu
        └── AtomUI.Desktop.Controls.Menu
              ├── implements ISizeTypeAware
              └── implements IMotionAwareControl
```

### MenuItem

```
Avalonia.Controls.HeaderedSelectingItemsControl
  └── Avalonia.Controls.MenuItem
        └── AtomUI.Desktop.Controls.MenuItem
              └── implements IMenuItemData
```

### ContextMenu

```
Avalonia.Controls.MenuBase
  └── Avalonia.Controls.ContextMenu
        └── AtomUI.Desktop.Controls.ContextMenu
              ├── implements ISizeTypeAware
              └── implements IMotionAwareControl
```

### MenuSeparator

```
Avalonia.Controls.Separator
  └── AtomUI.Desktop.Controls.MenuSeparator
```

---

## 源码位置

| 层级 | 文件路径 | 说明 |
|---|---|---|
| Menu | `src/AtomUI.Desktop.Controls/Menu/Menu.cs` | 顶部菜单栏 |
| MenuItem | `src/AtomUI.Desktop.Controls/Menu/MenuItem.cs` | 菜单项 |
| ContextMenu | `src/AtomUI.Desktop.Controls/Menu/ContextMenu.cs` | 右键上下文菜单 |
| MenuSeparator | `src/AtomUI.Desktop.Controls/Menu/MenuSeparator.cs` | 菜单分隔线 |
| MenuItemData | `src/AtomUI.Desktop.Controls/Menu/MenuItemData.cs` | 数据模型接口和类 |
| MenuSeparatorData | `src/AtomUI.Desktop.Controls/Menu/MenuSeparatorData.cs` | 分隔线数据模型 |
| MenuItemPseudoClass | `src/AtomUI.Desktop.Controls/Menu/MenuItemPseudoClass.cs` | 伪类常量 |
| Token 定义 | `src/AtomUI.Desktop.Controls/Menu/MenuToken.cs` | 组件级 Design Token（40+ 属性） |
| Menu 主题 | `src/AtomUI.Desktop.Controls/Menu/Themes/MenuTheme.axaml` | Menu ControlTheme |
| MenuItem 主题 | `src/AtomUI.Desktop.Controls/Menu/Themes/MenuItemTheme.axaml` | MenuItem ControlTheme |
| ContextMenu 主题 | `src/AtomUI.Desktop.Controls/Menu/Themes/ContextMenuTheme.axaml` | ContextMenu ControlTheme |
| TopLevelMenuItem 主题 | `src/AtomUI.Desktop.Controls/Menu/Themes/TopLevelMenuItemTheme.axaml` | 顶层菜单项 ControlTheme |
| MenuSeparator 主题 | `src/AtomUI.Desktop.Controls/Menu/Themes/MenuSeparatorTheme.axaml` | 分隔线 ControlTheme |
| 模板常量 | `src/AtomUI.Desktop.Controls/Menu/Themes/MenuThemeConstants.cs` | 模板部件常量 |
| Gallery 示例 | `controlgallery/AtomUIGallery/ShowCases/Views/Navigation/MenuShowCase.axaml` | 使用范例 |

---

## 模板结构

### Menu 模板

```
Border (背景、边框、圆角)
└── ItemsPresenter (PART_ItemsPresenter)
    └── StackPanel (Horizontal)  ← 水平排列顶层菜单项
```

### MenuItem 模板

```
Panel
├── Border#Frame (背景、圆角、内间距)
│   └── Grid (5 列)
│       ├── Panel#ToggleItemsLayout       ← CheckBox/Radio 切换控件
│       │   ├── CheckBox (PART_ToggleCheckbox)
│       │   └── RadioButton (PART_ToggleRadio)
│       ├── IconPresenter#ItemIconPresenter ← 图标
│       ├── ContentPresenter#ItemTextPresenter ← 标题文本
│       ├── TextBlock#InputGestureText      ← 快捷键提示
│       └── RightOutlined#MenuIndicatorIcon ← 子菜单箭头
└── Popup (PART_Popup)                     ← 子菜单弹窗
    └── Border#PopupFrame
        └── ScrollViewer
            └── ItemsPresenter (PART_ItemsPresenter)
```

### 模板部件常量

| 类 | 常量名 | 值 | 说明 |
|---|---|---|---|
| `MenuThemeConstants` | `ItemsPresenterPart` | `"PART_ItemsPresenter"` | Menu 项列表 |
| `MenuItemThemeConstants` | `ToggleCheckboxPart` | `"PART_ToggleCheckbox"` | CheckBox 切换 |
| `MenuItemThemeConstants` | `ToggleRadioPart` | `"PART_ToggleRadio"` | Radio 切换 |
| `MenuItemThemeConstants` | `ItemsPresenterPart` | `"PART_ItemsPresenter"` | 子菜单项列表 |
| `ContextMenuThemeConstants` | `ItemsPresenterPart` | `"PART_ItemsPresenter"` | ContextMenu 项列表 |
| `TopLevelMenuItemThemeConstants` | `ItemsPresenterPart` | `"PART_ItemsPresenter"` | 顶层菜单项子菜单 |
