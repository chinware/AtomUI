# Collapse 折叠面板

## 概述

折叠面板（Collapse）可以将内容区域折叠/展开，适用于对复杂区域进行分组和隐藏。当页面内容较多时，折叠面板可以有效减少信息密度，让用户按需展开感兴趣的区域。

AtomUI 的 `Collapse` 控件复刻了 [Ant Design 5.0 Collapse](https://ant.design/components/collapse-cn) 的设计规范，在 .NET / Avalonia 平台上提供与 Ant Design React 版一致的视觉体验和交互行为。

---

## 设计原理

### Ant Design 的折叠面板设计哲学

Ant Design 对 Collapse 的定位是：**「可以折叠/展开的内容区域」**。其核心设计理念包括：

1. **信息分组** — 将表单、配置项按类别分组，降低界面复杂度
2. **节省空间** — 隐藏不常用的内容，减少信息密度
3. **渐进式披露** — 按需展示详细信息，引导用户逐步了解

**设计建议（来自 Ant Design）：**
- 对有多个内容区域时，使用折叠面板整理信息层级
- 手风琴模式适合「一次只关注一件事」的场景（如 FAQ 列表）
- 幽灵/无边框模式适合需要更轻量化视觉效果的场景

### Avalonia SelectingItemsControl 基础能力

AtomUI 的 `Collapse` 继承自 Avalonia 框架的 `Avalonia.Controls.SelectingItemsControl`。理解其基础能力有助于理解 AtomUI 在其之上做了哪些扩展。

**Avalonia SelectingItemsControl 的核心职责：**

`SelectingItemsControl` 是 Avalonia 中管理一组可选择项的容器基类。它通过 `SelectionMode` 控制选择行为，支持单选、多选和切换模式。其继承链为：

```
Control → TemplatedControl → ItemsControl → SelectingItemsControl → Collapse
```

**Avalonia SelectingItemsControl 提供的基础属性：**

| 属性 | 说明 |
|---|---|
| `ItemsSource` | 数据源绑定，可绑定集合自动生成子项 |
| `ItemTemplate` | 子项内容模板 |
| `ItemsPanel` | 子项布局面板（Collapse 默认使用垂直 `StackPanel`） |
| `SelectedIndex` / `SelectedItem` | 当前选中项索引/对象 |
| `SelectedItems` | 多选模式下的选中项集合 |
| `SelectionMode` | 选择模式：`Single`、`Multiple`、`Toggle` 等组合 |
| `AutoScrollToSelectedItem` | 选中时是否自动滚动（Collapse 覆盖为 `false`） |

**Avalonia SelectingItemsControl 提供的基础伪类：**

| 伪类 | 说明 |
|---|---|
| `:disabled` | `IsEnabled == false` |
| `:focus` | 获得焦点 |
| `:pointerover` | 鼠标悬浮 |

### AtomUI 的扩展设计

AtomUI `Collapse` 在 Avalonia SelectingItemsControl 的基础上，遵循 Ant Design 5.0 规范做了以下扩展：

| 扩展能力 | 实现方式 | 设计动机 |
|---|---|---|
| **多面板展开** | `SelectionMode.Multiple \| Toggle` | 默认允许多个面板同时展开/折叠 |
| **手风琴模式** | `IsAccordion` 属性 | 同时只能展开一个面板，内部切换 `SelectionMode` |
| **三种尺寸** | `SizeType` 共享属性（通过 `AddOwner`） | 适配不同信息密度场景（Small / Middle / Large） |
| **三种样式** | `IsGhostStyle` / `IsBorderless` 属性 | 默认（有边框）/ 无边框 / 幽灵（最轻量）三种视觉风格 |
| **触发方式** | `TriggerType`（Header / Icon） | 控制点击哪里触发折叠 |
| **图标位置** | `ExpandIconPosition`（Start / End） | 展开图标在左侧或右侧 |
| **折叠动画** | `IMotionAwareControl` + `SlideUpIn/Out` | 平滑的折叠/展开过渡动画 |
| **自定义间距** | `ItemHeaderPadding` / `ItemContentPadding` | 允许自定义头部和内容的内间距 |
| **附加内容** | `CollapseItem.AddOnContent` | 在头部右侧区域放置额外操作按钮或图标 |
| **数据驱动** | `ICollapseItemData` 接口 + `ItemsSource` | 支持数据绑定自动生成面板 |
| **Design Token** | `CollapseToken` + `RegisterTokenResourceScope` | 所有视觉值从全局/组件级 Token 派生 |

---

## 功能详解

### 手风琴模式（IsAccordion）

`IsAccordion = true` 时，同时只能展开一个面板。内部将 `SelectionMode` 从 `Multiple | Toggle` 切换为 `Single | Toggle`，确保选择互斥。

### 三种样式

| 样式 | 属性设置 | 视觉效果 |
|---|---|---|
| **默认** | 无特殊设置 | 有外边框 + 面板间分隔线 + 头部背景色 |
| **无边框** | `IsBorderless="True"` | 无外边框，面板间有分隔线，内容区使用头部背景色 |
| **幽灵** | `IsGhostStyle="True"` | 无边框 + 头部使用内容背景色（透明效果），最轻量 |

### 边框厚度计算逻辑

Collapse 根据面板位置、展开状态和样式模式动态计算每个 CollapseItem 的头部和内容边框厚度：

- **默认模式**：最后一个折叠面板的头部底部边框为 0（避免双线）；最后一个面板展开时内容底部边框也为 0
- **无边框模式**：展开的面板和最后一个面板的头部底部边框为 0
- **幽灵模式**：所有边框厚度均为 0

### 触发方式（TriggerType）

| 触发方式 | 行为 |
|---|---|
| `Header`（默认） | 点击整个头部区域（包括图标、标题、附加内容）触发折叠/展开 |
| `Icon` | 仅点击展开图标按钮触发折叠/展开，头部其他区域点击无效 |

`TriggerType = Header` 时，头部区域显示手形光标（`Cursor = Hand`）；`TriggerType = Icon` 时，仅展开图标按钮显示手形光标。

### 展开图标位置（ExpandIconPosition）

| 位置 | 行为 |
|---|---|
| `Start`（默认） | 展开图标在头部左侧（Grid.Column=0） |
| `End` | 展开图标在头部右侧（Grid.Column=3） |

展开图标默认使用 `RightOutlined`（右箭头），面板展开时通过 `RenderTransform = rotate(90deg)` 旋转为朝下箭头。可通过 `CollapseItem.ExpandIcon` 自定义图标。

### 折叠/展开动画

CollapseItem 使用 `MotionScene` 动画系统实现平滑的折叠/展开过渡：

| 动画 | 方向 | 缓动 | 说明 |
|---|---|---|---|
| `SlideUpInMotion` | 展开 | `CubicEaseOut` | 内容从折叠状态滑入显示 |
| `SlideUpOutMotion` | 折叠 | `CubicEaseIn` | 内容从显示状态滑出隐藏 |

动画时长由 `SharedToken.MotionDurationSlow` 控制。动画期间（`InAnimating = true`）会阻止用户点击操作，防止动画冲突。

当 `IsMotionEnabled = false` 时，折叠/展开直接切换 `IsVisible`，无过渡动画。

CollapseItem 还为 `HeaderBorderThickness` 和 `ContentBorderThickness` 配置了 `ThicknessTransition`，使边框厚度变化在动画启用时也有平滑过渡。

### 附加内容（AddOnContent）

`CollapseItem` 支持在头部右侧放置附加内容（如操作按钮、图标等），通过 `AddOnContent` 属性设置：

```xml
<atom:CollapseItem Header="Panel Header"
                   AddOnContent="{antdicons:AntDesignIconProvider Kind=SettingOutlined}">
    Content
</atom:CollapseItem>
```

### 数据驱动（ItemsSource + ICollapseItemData）

Collapse 支持通过 `ItemsSource` 绑定数据集合自动生成面板。数据项可实现 `ICollapseItemData` 接口，自动映射 `Header`、`IsSelected`、`IsShowExpandIcon` 等属性：

```csharp
public interface ICollapseItemData
{
    bool IsSelected { get; }
    bool IsEnabled { get; }
    bool IsShowExpandIcon { get; }
    object? Header { get; }
}
```

也提供了 `CollapseItemData` record 作为默认实现。

---

## 与 Ant Design 规范的对齐程度

| 特性 | Ant Design (React) | AtomUI (Avalonia) | 对齐情况 |
|---|---|---|---|
| 折叠/展开 | ✅ `activeKey` | ✅ `SelectedItems` / `IsSelected` | ✅ 完全对齐 |
| 手风琴 | ✅ `accordion` | ✅ `IsAccordion` | ✅ 完全对齐 |
| 尺寸 | ✅ `size` | ✅ `SizeType` | ✅ 完全对齐 |
| 幽灵模式 | ✅ `ghost` | ✅ `IsGhostStyle` | ✅ 完全对齐 |
| 无边框 | ✅ `bordered={false}` | ✅ `IsBorderless` | ✅ 完全对齐 |
| 触发区域 | ✅ `collapsible` | ✅ `TriggerType` | ✅ 完全对齐 |
| 展开图标位置 | ✅ `expandIconPosition` | ✅ `ExpandIconPosition` | ✅ 完全对齐 |
| 自定义展开图标 | ✅ `expandIcon` | ✅ `CollapseItem.ExpandIcon` | ✅ 完全对齐 |
| 显示/隐藏箭头 | ✅ `showArrow` | ✅ `CollapseItem.IsShowExpandIcon` | ✅ 完全对齐 |
| 附加内容 `extra` | ✅ ReactNode | ✅ `CollapseItem.AddOnContent` | ✅ 完全对齐 |
| 嵌套折叠 | ✅ 支持 | ✅ 支持 | ✅ 完全对齐 |
| 销毁隐藏内容 | ✅ `destroyInactivePanel` | ❌ 不支持 | ⚠️ 待支持 |
| `onChange` | ✅ 回调 | ✅ `SelectionChanged` 事件 | ✅ 完全对齐 |

---

## 继承关系

### Collapse

```
Avalonia.Controls.Primitives.TemplatedControl
  └── Avalonia.Controls.ItemsControl
        └── Avalonia.Controls.SelectingItemsControl
              └── AtomUI.Desktop.Controls.Collapse
                    └── implements IMotionAwareControl
```

### CollapseItem

```
Avalonia.Controls.Primitives.TemplatedControl
  └── Avalonia.Controls.ContentControl
        └── Avalonia.Controls.Primitives.HeaderedContentControl
              └── AtomUI.Desktop.Controls.CollapseItem
                    └── implements ISelectable
```

**各层级职责划分：**

| 层级 | 提供的能力 |
|---|---|
| `SelectingItemsControl` | 子项集合管理、选择模式（单选/多选/切换）、`ItemsSource` / `ItemTemplate` 数据绑定、`SelectedItems` 选中状态管理 |
| `HeaderedContentControl` | `Header` / `HeaderTemplate` + `Content` / `ContentTemplate` 双区域内容模型 |
| `Collapse` | Ant Design 视觉体系（手风琴/幽灵/无边框/三尺寸）、Design Token 集成、折叠动画、触发方式、图标位置、自定义间距 |
| `CollapseItem` | 面板展开/折叠状态、展开图标管理、附加内容区域、动画执行、边框动态计算 |

**实现的共享接口：**

| 接口 | 控件 | 定义位置 | 作用 |
|---|---|---|---|
| `IMotionAwareControl` | Collapse | `AtomUI.Controls.Shared` | 支持 `IsMotionEnabled` 动画开关 |
| `ISelectable` | CollapseItem | `Avalonia.Controls` | 支持 `IsSelected` 选中状态 |

---

## 源码位置

| 层级 | 文件路径 | 说明 |
|---|---|---|
| 控件类 | `src/AtomUI.Desktop.Controls/Collapse/Collapse.cs` | 折叠面板容器 |
| 子项 | `src/AtomUI.Desktop.Controls/Collapse/CollapseItem.cs` | 折叠面板项 |
| 数据模型 | `src/AtomUI.Desktop.Controls/Collapse/ICollapseItemData.cs` | `ICollapseItemData` 接口和默认实现 |
| 伪类常量 | `src/AtomUI.Desktop.Controls/Collapse/CollapseItemPseudoClass.cs` | CollapseItem 伪类定义 |
| Token 定义 | `src/AtomUI.Desktop.Controls/Collapse/CollapseToken.cs` | 组件级 Design Token |
| 主题模板 | `src/AtomUI.Desktop.Controls/Collapse/Themes/CollapseTheme.axaml` | Collapse ControlTheme AXAML |
| 子项主题 | `src/AtomUI.Desktop.Controls/Collapse/Themes/CollapseItemTheme.axaml` | CollapseItem ControlTheme AXAML |
| 主题注册 | `src/AtomUI.Desktop.Controls/Collapse/Themes/CollapseItemThemes.axaml` | 主题资源注册 |
| 模板常量 | `src/AtomUI.Desktop.Controls/Collapse/Themes/CollapseThemeConstants.cs` | 模板部件名称常量 |
| Gallery 示例 | `controlgallery/AtomUIGallery/ShowCases/Views/DataDisplay/CollapseShowCase.axaml` | 使用范例 |

---

## 模板结构

### Collapse 模板

```
Border#PART_Frame (BorderThickness=EffectiveBorderThickness, ClipToBounds=True)
└── ItemsPresenter#PART_ItemsPresenter (StackPanel, Orientation=Vertical)
    ├── CollapseItem[0]
    ├── CollapseItem[1]
    └── CollapseItem[n]
```

### CollapseItem 模板

```
DockPanel#PART_MainLayout (LastChildFill=True)
├── Border#PART_HeaderDecorator (DockPanel.Dock=Top)             ← 头部区域
│   └── Grid (ColumnDefinitions="Auto, *, Auto, Auto")
│       ├── IconButton#PART_ExpandButton (Column=0 或 3)         ← 展开/折叠图标按钮
│       ├── ContentPresenter#PART_HeaderPresenter (Column=1)     ← 标题内容
│       └── ContentPresenter#PART_AddOnContentPresenter (Column=2) ← 附加内容
└── LayoutAwareMotionActor#PART_ContentMotionActor (ClipToBounds=True) ← 动画容器
    └── ContentPresenter#PART_ContentPresenter                   ← 面板内容
```

**分层设计理由：**
- **DockPanel 布局**：头部 Dock 在顶部，内容填充剩余空间，确保展开时内容向下推进。
- **Grid 四列布局**：Column 0 和 3 预留给展开图标按钮（左或右），Column 1 给标题，Column 2 给附加内容。通过 `ExpandIconPosition` 动态切换按钮所在列。
- **MotionActor 包裹**：`LayoutAwareMotionActor` 包裹内容区域，提供 `SlideUpIn/Out` 折叠/展开动画，`ClipToBounds=True` 确保动画过程中内容不溢出。

### 模板部件常量

#### Collapse

| 常量名 | 值 | 说明 |
|---|---|---|
| `CollapseThemeConstants.FramePart` | `"PART_Frame"` | 外框 Border |
| `CollapseThemeConstants.ItemsPresenterPart` | `"PART_ItemsPresenter"` | 子项展示器 |

#### CollapseItem

| 常量名 | 值 | 说明 |
|---|---|---|
| `CollapseItemThemeConstants.MainLayoutPart` | `"PART_MainLayout"` | 主布局面板 |
| `CollapseItemThemeConstants.ExpandButtonPart` | `"PART_ExpandButton"` | 展开图标按钮 |
| `CollapseItemThemeConstants.HeaderPresenterPart` | `"PART_HeaderPresenter"` | 头部内容展示器 |
| `CollapseItemThemeConstants.HeaderDecoratorPart` | `"PART_HeaderDecorator"` | 头部装饰器 Border |
| `CollapseItemThemeConstants.ContentPresenterPart` | `"PART_ContentPresenter"` | 内容展示器 |
| `CollapseItemThemeConstants.AddOnContentPresenterPart` | `"PART_AddOnContentPresenter"` | 附加内容展示器 |
| `CollapseItemThemeConstants.ContentMotionActorPart` | `"PART_ContentMotionActor"` | 内容动画容器 |
