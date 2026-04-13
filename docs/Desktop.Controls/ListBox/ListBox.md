# ListBox 列表框

## 概述

列表框（ListBox）是一种用于展示可选项集合的容器控件，用户可以从中选择一个或多个选项。AtomUI 的 ListBox 在 Avalonia 原生 `ListBox` 的基础上，增强了 Ant Design 风格的视觉体验，并扩展了项目过滤、高亮搜索、空状态指示器、选中指示器等实用功能。

AtomUI ListBox 控件族包含：

| 控件 | 说明 |
|---|---|
| `ListBox` | 列表框容器，管理列表项集合与选择行为 |
| `ListBoxItem` | 列表框中的单个选项项 |

> ListBox 不直接对标某个特定的 Ant Design 组件，而是作为 AtomUI 体系中 Select、AutoComplete、Transfer 等复合控件的底层选项容器使用。其设计兼顾独立使用和作为子组件嵌入的两种场景。

---

## 设计原理

### 定位与设计动机

在 Ant Design 体系中，`List` 组件用于展示数据列表，而 `Select` 的下拉面板中也包含类似列表的选项容器。AtomUI 的 ListBox 定位为：

1. **独立列表控件** — 可直接使用，展示可选项列表。
2. **复合控件的内部选项容器** — 作为 Select、AutoComplete、Cascader、Transfer 等控件的底层下拉选项列表。

因此，ListBox 在设计上注重：
- **灵活的选择模式** — 支持单选、多选，以及可选的"不可选"模式（纯展示）。
- **内置过滤与高亮** — 支持按关键字过滤列表项并高亮匹配文本，为搜索场景提供原生支持。
- **空状态指示器** — 当列表为空或过滤无结果时，自动显示空状态提示。
- **选中指示器** — 可选显示勾选图标标记已选项。
- **虚拟化上下文管理** — 支持 UI 虚拟化场景下的状态保存与恢复。

### Avalonia ListBox 基础能力

AtomUI ListBox 继承自 `Avalonia.Controls.ListBox`，其继承链为：

```
Control → TemplatedControl → ItemsControl → SelectingItemsControl → ListBox
```

**Avalonia ListBox 提供的核心能力：**

| 能力 | 说明 |
|---|---|
| 项目管理 | `Items` 集合、`ItemsSource` 数据绑定、`ItemTemplate` 数据模板 |
| 选择模型 | `SelectionMode`（Single / Multiple / Toggle / AlwaysSelected）、`SelectedItem`、`SelectedIndex`、`SelectedItems` |
| 虚拟化 | `VirtualizationMode` 支持项目虚拟化以提升大数据量性能 |
| 滚动 | 内置 `ScrollViewer` 支持 |
| 容器化 | 自动将数据项包装为 `ListBoxItem` 容器 |

### AtomUI 的扩展设计

| 扩展能力 | 实现方式 | 设计动机 |
|---|---|---|
| **可选性控制** | `IsSelectable` 属性 | 允许禁用选择行为，用于纯展示场景 |
| **三种尺寸** | `ISizeTypeAware` 接口 | 全局统一的 Large / Middle / Small 尺寸系统 |
| **无边框模式** | `IsBorderless` 属性 | 嵌入其他控件时去除边框 |
| **自定义悬浮/选中背景** | `ItemHoverBg` / `ItemSelectedBg` | 允许精细控制列表项的交互态颜色 |
| **选中指示器** | `IsShowSelectedIndicator` + `SelectedIndicator` | 可选显示勾选图标 |
| **项目过滤** | `ItemFilter` + `ItemFilterValue` | 内置过滤机制，支持自定义过滤器 |
| **过滤高亮** | `ItemFilterHighlightStrategy` + `FilterHighlightForeground` | 过滤时高亮匹配文本 |
| **空状态指示器** | `EmptyIndicator` + `IsShowEmptyIndicator` | 列表为空时自动显示空状态 |
| **项目点击事件** | `ItemClicked` 事件 | 除选择外还提供独立的点击事件 |
| **虚拟化上下文** | `IListVirtualizingContextAware` | 虚拟化场景下保存/恢复项目状态 |
| **Design Token** | `ListBoxToken` | 所有视觉值从全局 Token 派生 |

---

## 控件层级关系

ListBox 控件族完全定义在桌面端控件层（`AtomUI.Desktop.Controls`），没有对应的设备无关基类。

```
Avalonia.Controls.ListBox
  └── AtomUI.Desktop.Controls.ListBox (ISizeTypeAware, IMotionAwareControl, IListVirtualizingContextAware)

Avalonia.Controls.ListBoxItem
  └── AtomUI.Desktop.Controls.ListBoxItem (IListItemVirtualizingContextAware)
```

### 各层级职责划分

| 层级 | 提供的能力 |
|---|---|
| `Avalonia.Controls.ListBox` | 项目集合管理、选择模型（Single/Multiple/Toggle）、虚拟化、滚动、容器化 |
| `AtomUI.Desktop.Controls.ListBox` | 可选性控制、尺寸系统、无边框模式、自定义交互态颜色、选中指示器、项目过滤与高亮、空状态指示器、项目点击事件、虚拟化上下文管理、Design Token 注册 |
| `Avalonia.Controls.ListBoxItem` | 基础列表项容器、选中态管理 |
| `AtomUI.Desktop.Controls.ListBoxItem` | 点击路由事件、选中指示器显示逻辑、过滤高亮文本渲染、背景色过渡动画、虚拟化上下文感知 |

### 实现的接口

| 接口 | 控件 | 说明 |
|---|---|---|
| `ISizeTypeAware` | ListBox | 支持 `SizeType`（Small / Middle / Large）尺寸切换 |
| `IMotionAwareControl` | ListBox | 支持动画开关 |
| `IListVirtualizingContextAware` | ListBox | 虚拟化场景下保存/恢复项目状态 |
| `IListItemVirtualizingContextAware` | ListBoxItem | 虚拟化场景下的项目级上下文感知 |

---

## 功能详解

### 项目过滤机制

ListBox 内置了完整的项目过滤系统：

1. **`IListBoxItemFilter` 接口**：定义过滤逻辑的抽象接口。
   ```csharp
   public interface IListBoxItemFilter
   {
       bool Filter(ListBox listBox, ListBoxItem listBoxItem, object? filterValue);
   }
   ```

2. **`DefaultListBoxItemFilter`**：默认过滤器实现，基于字符串包含匹配。支持 `ValueFilterMode.Contains`（默认）等过滤模式。

3. **过滤流程**：
   - 设置 `ItemFilterValue` 时触发过滤。
   - 对每个列表项调用 `ItemFilter.Filter()`。
   - 根据 `ItemFilterHighlightStrategy` 控制匹配行为：
     - `HighlightedMatch`：高亮匹配文本。
     - `BoldedMatch`：加粗匹配文本。
     - `HideUnMatched`：隐藏未匹配的项。
     - `All`：以上全部。
   - 过滤结果数通过 `FilterResultCount` 只读属性暴露。

4. **过滤高亮**：过滤时，`ListBoxItem` 内部切换为 `HighlightableTextBlock` 渲染，自动高亮匹配的关键字。

### 选中指示器

通过 `IsShowSelectedIndicator` 启用后，已选中的列表项右侧会显示一个勾选图标。默认使用 `CheckOutlined` 图标，可通过 `SelectedIndicator` 属性自定义。

### 空状态指示器

当 `IsShowEmptyIndicator == true`（默认）时：
- 列表为空（`ItemCount == 0`）时自动显示空状态。
- 过滤无结果（`IsFiltering && FilterResultCount == 0`）时也显示空状态。
- 默认显示 AtomUI 的 `Empty` 控件（Simple 预设图片）。
- 可通过 `EmptyIndicator` / `EmptyIndicatorTemplate` 自定义空状态内容。

### 虚拟化上下文管理

ListBox 实现了 `IListVirtualizingContextAware` 接口，在 UI 虚拟化场景下：
- 当列表项被回收时，通过 `SaveVirtualizingContext()` 保存其状态（如 `IsEnabled`）。
- 当列表项被重用时，通过 `RestoreVirtualizingContext()` 恢复其状态。
- 子类可通过重写 `NotifySaveVirtualizingContext()` / `NotifyRestoreVirtualizingContext()` 扩展保存/恢复的上下文。

### 项目点击事件

ListBoxItem 定义了 `Clicked` 路由事件（Bubble 策略），ListBox 监听该事件并通过 `ItemClicked` CLR 事件暴露给外部。这允许在不依赖选择变化的情况下响应项目点击。

---

## 与 Ant Design 规范的对齐程度

| 特性 | Ant Design | AtomUI | 对齐情况 |
|---|---|---|---|
| 列表展示 | ✅ `List` | ✅ `ListBox` | ✅ 功能对齐 |
| 多选 | ✅ `Select` multiple | ✅ `SelectionMode="Multiple"` | ✅ 完全对齐 |
| 空状态 | ✅ `Empty` | ✅ `EmptyIndicator` 内置 `Empty` 控件 | ✅ 完全对齐 |
| 搜索过滤 | ✅ `Select` filterOption | ✅ `ItemFilter` + `ItemFilterValue` | ✅ 完全对齐 |
| 高亮匹配 | ✅ | ✅ `HighlightableTextBlock` | ✅ 完全对齐 |
| 勾选指示器 | ✅ Select 下拉勾选 | ✅ `IsShowSelectedIndicator` | ✅ 完全对齐 |
| 禁用 | ✅ `disabled` | ✅ `IsEnabled` | ✅ 完全对齐 |
| 虚拟滚动 | ✅ `virtual` | ✅ Avalonia 虚拟化 | ✅ 完全对齐 |

---

## 源码位置

| 层级 | 文件路径 | 说明 |
|---|---|---|
| ListBox 控件 | `src/AtomUI.Desktop.Controls/ListBox/ListBox.cs` | 列表框实现 |
| ListBoxItem 控件 | `src/AtomUI.Desktop.Controls/ListBox/ListBoxItem.cs` | 列表项实现 |
| ListBox Token | `src/AtomUI.Desktop.Controls/ListBox/ListBoxToken.cs` | 组件 Token |
| 过滤器接口 | `src/AtomUI.Desktop.Controls/ListBox/IListBoxItemFilter.cs` | 过滤器抽象接口 |
| 默认过滤器 | `src/AtomUI.Desktop.Controls/ListBox/DefaultListBoxItemFilter.cs` | 默认过滤器实现 |
| 点击事件参数 | `src/AtomUI.Desktop.Controls/ListBox/ListBoxItemClickedEventArgs.cs` | 点击事件参数类 |
| ListBox 主题 | `src/AtomUI.Desktop.Controls/ListBox/Themes/ListBoxTheme.axaml` | ListBox ControlTheme |
| ListBoxItem 主题 | `src/AtomUI.Desktop.Controls/ListBox/Themes/ListBoxItemTheme.axaml` | ListBoxItem ControlTheme |

---

## 模板结构

### ListBox 模板

```
Border#Frame
└── Panel
    ├── ScrollViewer (PART_ScrollViewer)          ← 列表内容（非空时可见）
    │   └── ItemsPresenter#ItemsPresenter
    └── ContentPresenter#EmptyIndicator           ← 空状态指示器（空时可见）
```

### ListBoxItem 模板

```
Border#Frame
└── DockPanel
    ├── IconTemplatePresenter#SelectedIndicator   ← 选中指示器（右侧对齐）
    └── Panel
        ├── ContentPresenter#ContentPresenter     ← 常规内容（非过滤时可见）
        └── HighlightableTextBlock                ← 高亮文本（过滤时可见）
```
