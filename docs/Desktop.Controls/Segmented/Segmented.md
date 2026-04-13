# Segmented 分段控制器

## 概述

分段控制器（Segmented）用于在少量互斥选项之间进行快速切换。它在视觉上提供比 RadioButton 更强的操作感和辨识度，类似于 iOS/macOS 的 SegmentedControl。用户通过点击不同的分段来切换当前选中项，被选中的分段通过一个带有投影的**滑动色块（Thumb）**来标识，选中态切换时色块会平滑动画滑动到目标位置。

AtomUI 的 `Segmented` 控件完整复刻了 [Ant Design 5.0 Segmented](https://ant.design/components/segmented-cn) 的设计规范，在 .NET / Avalonia 平台上提供与 Ant Design React 版一致的视觉体验和交互行为。

---

## 设计原理

### Ant Design 的分段控制器设计哲学

Ant Design 对 Segmented 的定位是：**「分段控制器。当用户需要在几个互斥的选项之间进行选择时使用，相比 Radio 按钮有更强的视觉感知」**。典型应用场景包括：

- 视图模式切换（列表视图 / 卡片视图 / 网格视图）
- 时间维度切换（日 / 周 / 月 / 季 / 年）
- 数据分类筛选（全部 / 待处理 / 已完成）
- 简单的标签页导航

**核心设计特征**：

| 特征 | 说明 |
|---|---|
| 🔘 **互斥单选** | 始终有且仅有一个选项被选中，首次渲染默认选中第一项 |
| 🎯 **滑动色块** | 选中项通过一个独立的色块（Thumb）标识，切换时色块平滑滑动到新位置 |
| 📏 **三种尺寸** | Large / Middle / Small，与全局尺寸体系一致 |
| 🔲 **Block 模式** | 可撑满父容器，选项均分宽度 |
| 🖼️ **图标支持** | 每个选项可包含图标，或仅显示图标 |
| 🚫 **逐项禁用** | 可单独禁用某些选项 |

### Avalonia 基础能力

AtomUI 的 `Segmented` 控件基于 Avalonia 的 `SelectingItemsControl`。理解 Avalonia 提供的基础能力有助于理解 AtomUI 在其之上做了哪些扩展。

**Avalonia SelectingItemsControl 的核心职责：**

`SelectingItemsControl` 继承自 `ItemsControl`，增加了选择管理能力。它维护 `SelectedItem` / `SelectedIndex` 等选择状态，并通过 `SelectionChanged` 事件通知外部。其继承链为：

```
Control → TemplatedControl → ItemsControl → SelectingItemsControl
```

**Avalonia SelectingItemsControl 提供的基础属性：**

| 属性 | 说明 |
|---|---|
| `Items` | 数据项集合（ItemsControl 基础能力） |
| `ItemTemplate` | 数据项的 DataTemplate |
| `SelectedIndex` | 当前选中项的索引 |
| `SelectedItem` | 当前选中的数据对象 |
| `SelectionMode` | 选择模式（Segmented 固定为 `Single`） |
| `SelectionChanged` | 选中项变化事件 |
| `AutoScrollToSelectedItem` | 选中时是否自动滚动到选中项（Segmented 默认 `false`） |

### AtomUI 的扩展设计

AtomUI `Segmented` 在 Avalonia SelectingItemsControl 的基础上，遵循 Ant Design 5.0 规范做了以下扩展：

| 扩展能力 | 实现方式 | 设计动机 |
|---|---|---|
| **滑动色块（Thumb）** | 自定义 `Render` 方法绘制带圆角和阴影的矩形 | 复刻 Ant Design 的选中滑块动画效果 |
| **色块滑动动画** | `SelectedThumbPos` / `SelectedThumbSize` + `PointTransition` / `SizeTransition` | 选中项切换时色块平滑过渡 |
| **三种尺寸** | `ISizeTypeAware` 接口 + `SizeType` 共享属性 | 全局统一的 Large / Middle / Small 尺寸系统 |
| **Block 模式** | `IsExpanding` 属性 + `SegmentedStackPanel` 自定义布局 | 选项均分父容器宽度 |
| **图标支持** | `SegmentedItem.Icon` 属性 + `IconPresenter` | 每个选项可包含 Ant Design 图标 |
| **逐项禁用** | 各 `SegmentedItem` 独立的 `IsEnabled` 属性 | 灵活控制可用性 |
| **默认选中首项** | `OnApplyTemplate` 中检查并设置 `SelectedIndex = 0` | 对齐 Ant Design 默认行为 |
| **表单集成** | `IFormItemAware` 接口 | 可参与 FormItem 验证流程 |
| **Design Token** | `SegmentedToken` + `RegisterTokenResourceScope` | 所有视觉值从全局/组件级 Token 派生 |
| **过渡动画** | `IMotionAwareControl` + `IsMotionEnabled` | 色块滑动、选项背景色过渡动画可全局开关 |

---

## 功能详解

### 滑动色块（Selected Thumb）

Segmented 最具标志性的视觉特征是**滑动色块**——一个在选中项后方渲染的带圆角、带阴影的矩形色块。它的行为如下：

- **位置追踪**：通过 `SelectedThumbPos` 和 `SelectedThumbSize` 记录当前选中项的位置和尺寸
- **平滑动画**：当选中项变化时，`SelectedThumbPos` 通过 `PointTransition` 动画过渡到新位置，`SelectedThumbSize` 通过 `SizeTransition` 动画过渡到新尺寸
- **自定义渲染**：在 `Render` 方法中使用 `DrawRectangle` 直接绘制色块和容器背景，无需额外的视觉层
- **阴影效果**：色块带有 `BoxShadowsTertiary` 投影，增加立体感

### Block 模式（IsExpanding）

当 `IsExpanding = true` 时：
- Segmented 容器撑满父元素宽度（`HorizontalAlignment = Stretch`）
- 所有选项**均分**容器宽度
- 使用 `SegmentedStackPanel`（自定义 Panel）计算等宽布局

### 三种尺寸（SizeType）

尺寸通过 `SizeType` 属性设置，影响容器圆角、选项高度、字体大小和内间距：

| 尺寸 | 容器圆角 | 色块圆角 | 选项最小高度 | 字体大小 | 选项内间距 |
|---|---|---|---|---|---|
| `Large` | `BorderRadiusLG` | `BorderRadius` | `ControlHeightLG - TrackPadding * 2` | `FontSizeLG` | `SegmentedItemPadding` |
| `Middle` | `BorderRadius` | `BorderRadiusSM` | `ControlHeight - TrackPadding * 2` | `FontSize` | `SegmentedItemPadding` |
| `Small` | `BorderRadiusSM` | `BorderRadiusXS` | `ControlHeightSM - TrackPadding * 2` | `FontSize` | `SegmentedItemPaddingSM` |

### 图标支持

每个 `SegmentedItem` 支持 `Icon` 属性（`PathIcon` 类型），可以：
- 仅图标（不设 Content）
- 图标 + 文字（同时设置 Icon 和 Content）

当 `Icon` 不为 `null` 时，`:has-icon` 伪类被激活，文字内容会增加左边距以与图标保持间距。

### 过渡动画

| 控件 | 动画属性 | 说明 |
|---|---|---|
| `Segmented` | `SelectedThumbPos`、`SelectedThumbSize` | 色块位置和尺寸的平滑过渡 |
| `SegmentedItem` | `Background` | 选项背景色的平滑过渡（悬浮/按下态变化） |

---

## 与 Ant Design 规范的对齐程度

| 特性 | Ant Design (React) | AtomUI (Avalonia) | 对齐情况 |
|---|---|---|---|
| 基本选项 `options` | ✅ `string[] \| SegmentedOption[]` | ✅ `SegmentedItem` 子项 | ✅ 完全对齐 |
| 选中项 `value` | ✅ 受控属性 | ✅ `SelectedItem` / `SelectedIndex` | ✅ 完全对齐 |
| 默认选中 `defaultValue` | ✅ 默认值 | ✅ 默认选中首项 | ✅ 完全对齐 |
| Block 模式 `block` | ✅ 布尔属性 | ✅ `IsExpanding` 属性 | ✅ 完全对齐 |
| 三种尺寸 `size` | ✅ `large / middle / small` | ✅ `SizeType` 属性 | ✅ 完全对齐 |
| 禁用选项 `disabled` | ✅ 逐项 / 整体禁用 | ✅ `SegmentedItem.IsEnabled` | ✅ 完全对齐 |
| 图标 `icon` | ✅ ReactNode | ✅ `PathIcon` 属性 | ✅ 对齐（类型不同，语义一致） |
| 选中滑块动画 | ✅ CSS transition | ✅ `PointTransition` + `SizeTransition` | ✅ 完全对齐 |
| 自定义渲染 `label` | ✅ ReactNode | ⚠️ 通过 `ContentTemplate` 部分支持 | ⚠️ 部分对齐 |
| 选项变化回调 `onChange` | ✅ 回调函数 | ✅ `SelectionChanged` 事件 | ✅ 完全对齐 |

---

## 继承关系

### Segmented

```
Avalonia.Controls.Primitives.TemplatedControl
  └── Avalonia.Controls.ItemsControl
        └── Avalonia.Controls.SelectingItemsControl
              └── AtomUI.Controls.Commons.AbstractSegmented
                    ├── implements ISizeTypeAware
                    ├── implements IMotionAwareControl
                    ├── implements IFormItemAware
                    └── AtomUI.Desktop.Controls.Segmented
```

### SegmentedItem

```
Avalonia.Controls.Primitives.TemplatedControl
  └── Avalonia.Controls.ContentControl
        └── AtomUI.Controls.Commons.AbstractSegmentedItem (ISelectable)
              └── AtomUI.Desktop.Controls.SegmentedItem
```

**各层级职责划分：**

| 层级 | 提供的能力 |
|---|---|
| `SelectingItemsControl` (Avalonia) | 数据项管理、选择状态（`SelectedItem` / `SelectedIndex`）、`SelectionChanged` 事件、容器化（Container Virtualization） |
| `AbstractSegmented` (AtomUI.Controls) | 滑动色块渲染与动画、`SizeType` / `IsExpanding` / `IsMotionEnabled` 属性、默认选中首项、`SegmentedItem` 容器准备（传递 SizeType/IsMotionEnabled）、表单集成（IFormItemAware） |
| `Segmented` (AtomUI.Desktop.Controls) | Token 作用域注册、创建桌面端 `SegmentedItem` 容器 |
| `ContentControl` (Avalonia) | 任意内容容纳、`Content` / `ContentTemplate` |
| `AbstractSegmentedItem` (AtomUI.Controls) | `IsSelected` / `Icon` 属性、`:pressed` / `:selected` / `:has-icon` 伪类、鼠标选择交互、背景色过渡动画 |
| `SegmentedItem` (AtomUI.Desktop.Controls) | Token 作用域注册 |

**实现的共享接口：**

| 接口 | 定义位置 | 作用 |
|---|---|---|
| `ISizeTypeAware` | `AtomUI.Controls.Shared` | 支持 `SizeType`（Small / Middle / Large）三种尺寸切换 |
| `IMotionAwareControl` | `AtomUI.Controls` | 支持 `IsMotionEnabled` 属性，全局动画开关 |
| `IFormItemAware` | `AtomUI.Controls.Shared` | 可作为 `FormItem` 的子控件参与表单验证 |
| `ISelectable` | Avalonia | 支持 `IsSelected` 属性，参与 `SelectingItemsControl` 选择逻辑 |

---

## 源码位置

### 基类层（AtomUI.Controls）—— 设备无关

| 文件路径 | 说明 |
|---|---|
| `src/AtomUI.Controls/Segmented/AbstractSegmented.cs` | 分段控制器抽象基类：滑块渲染/动画、选择逻辑、尺寸/扩展属性、表单集成 |
| `src/AtomUI.Controls/Segmented/AbstractSegmentedItem.cs` | 分段选项抽象基类：选择状态、图标、伪类管理、背景色过渡 |
| `src/AtomUI.Controls/Segmented/SegmentedPseudoClass.cs` | 共享伪类常量定义（`:has-icon`） |
| `src/AtomUI.Controls/Segmented/SegmentedStackPanel.cs` | 自定义布局面板：支持普通水平排列和 Block 等宽排列 |

### 平台层（AtomUI.Desktop.Controls）—— 桌面端具体实现

| 文件路径 | 说明 |
|---|---|
| `src/AtomUI.Desktop.Controls/Segmented/Segmented.cs` | 桌面端 Segmented：Token 注册、创建 `SegmentedItem` 容器 |
| `src/AtomUI.Desktop.Controls/Segmented/SegmentedItem.cs` | 桌面端 SegmentedItem：Token 注册 |
| `src/AtomUI.Desktop.Controls/Segmented/SegmentedToken.cs` | 组件级 Design Token |
| `src/AtomUI.Desktop.Controls/Segmented/Themes/SegmentedTheme.axaml` | Segmented ControlTheme |
| `src/AtomUI.Desktop.Controls/Segmented/Themes/SegmentedItemTheme.axaml` | SegmentedItem ControlTheme |
| `src/AtomUI.Desktop.Controls/Segmented/Themes/SegmentedThemes.axaml` | 主题资源字典汇总注册 |
| Gallery 示例 | `controlgallery/AtomUIGallery/ShowCases/Views/DataDisplay/SegmentedShowCase.axaml` |

---

## 模板结构

### Segmented 模板

```
Border#Frame (CornerRadius, Padding, ClipToBounds)
└── ItemsPresenter (PART_ItemsPresenter)
    └── SegmentedStackPanel                    ← 自定义布局面板
        ├── SegmentedItem [0]
        ├── SegmentedItem [1]
        ├── SegmentedItem [2]
        └── ...
```

> **注意**：Segmented 的容器背景和选中色块均通过 `Render` 方法直接绘制（`DrawRectangle`），而非通过模板元素。这是因为色块需要在所有选项的"下方"但在容器背景的"上方"渲染，且需要动画支持。

### SegmentedItem 模板

```
Border#Frame (Background, CornerRadius, Padding)
└── DockPanel (LastChildFill=True)
    ├── IconPresenter#IconPresenter            ← 图标展示器（可选，Icon 不为 null 时显示）
    └── ContentPresenter#Content               ← 文字/自定义内容
```
