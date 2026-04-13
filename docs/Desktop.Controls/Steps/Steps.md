# Steps 步骤条

## 概述

步骤条（Steps）是一种引导用户按照流程完成任务的导航控件。它清晰地展示当前所处的步骤、已完成的步骤和待完成的步骤，帮助用户了解整体进度。

AtomUI 的 Steps 控件完整复刻了 [Ant Design 5.0 Steps](https://ant.design/components/steps-cn) 的设计规范，包含三个协作控件：

| 控件 | 说明 |
|---|---|
| **`Steps`** | 步骤条容器，管理步骤项的排列、当前步骤状态和交互 |
| **`StepsItem`** | 单个步骤项，展示步骤标题、描述、图标和状态 |
| **`StepsItemIndicator`** | 步骤指示器（内部控件），渲染数字/图标/点状指示器和进度环 |

---

## 设计原理

### Ant Design 的步骤条设计哲学

Ant Design 对 Steps 的定位是：**「引导用户按照流程完成任务的导航条」**。其核心设计理念包括：

- **清晰的流程感**：通过步骤编号/图标 + 连接线展示流程的线性进展
- **四种步骤状态**：Wait（等待）、Process（进行中）、Finish（完成）、Error（错误）
- **多种展示风格**：默认、点状（Dot）、导航（Navigation）、内联（Inline）
- **方向灵活**：支持水平和垂直两种排列方向
- **可交互**：支持点击步骤进行跳转
- **进度指示**：支持在当前步骤显示进度环

### 控件架构设计

Steps 采用 **ItemsControl 模式**，基于 Avalonia 的 `SelectingItemsControl`：

```
SelectingItemsControl (Avalonia)
  └── Steps (步骤条容器，管理 Grid 面板布局)
        └── StepsItem (步骤项，继承 HeaderedContentControl + ISelectable)
              └── StepsItemIndicator (内部指示器，渲染数字/图标/点/进度环)
```

**`Steps`** 继承 `SelectingItemsControl`，使用 `Grid` 作为 ItemsPanel。它根据 `Orientation` 自动配置 `Grid.ColumnDefinitions`（水平）或 `Grid.RowDefinitions`（垂直），并根据 `CurrentStep` 和 `CurrentStepStatus` 自动管理每个 `StepsItem` 的状态（Wait/Process/Finish/Error）。

**`StepsItem`** 继承 `HeaderedContentControl`，同时实现 `ISelectable`。每个步骤项包含标题（Header）、副标题（SubHeader）、描述（Description）和自定义图标（Icon），并支持内容模板用于步骤内容展示。

**`StepsItemIndicator`** 是内部控件，负责渲染步骤指示器的视觉表现——数字圆圈、自定义图标、点状指示器，以及当前步骤的进度环。

### AtomUI 的扩展设计

| 扩展能力 | 实现方式 | 设计动机 |
|---|---|---|
| **三种展示风格** | `StepsStyle`（Default / Navigation / Inline） | 适应不同场景：流程向导、导航菜单、列表内嵌状态 |
| **点状指示器** | `StepsItemIndicatorType.Dot` | 轻量化的流程展示，适合简洁风格 |
| **进度环** | `IsShowItemProgress` + `ProgressValue` | 在当前步骤显示百分比进度，提供更精细的进度信息 |
| **可点击步骤** | `IsItemClickable` | 允许用户点击任意步骤进行跳转 |
| **标签位置** | `LabelPlacement`（Horizontal / Vertical） | 标题/描述相对于指示器的排列方向 |
| **尺寸变体** | `ISizeTypeAware`（Small / Middle / Large） | 适应不同空间需求 |
| **步骤内容** | `CurrentContent` / `CurrentContentTemplate` | 自动展示当前步骤的关联内容 |
| **过渡动画** | `IMotionAwareControl` + 颜色/变换过渡 | 步骤切换时的平滑视觉过渡 |
| **Design Token 集成** | 丰富的 `StepsToken`（~50+ Token） | 四种状态 × 多种样式的完整颜色和尺寸 Token |

---

## 功能详解

### 步骤状态（StepsItemStatus）

每个步骤项有四种状态：

| 状态 | 说明 | 视觉表现 |
|---|---|---|
| `Wait` | 等待执行 | 灰色圆圈/数字，灰色标题和描述 |
| `Process` | 正在执行 | 主色调（Primary）填充圆圈，白色数字，深色标题 |
| `Finish` | 已完成 | 完成态背景，主色调图标，深色标题 |
| `Error` | 出错 | 错误色（Error）填充圆圈和标题 |

Steps 会根据 `CurrentStep` 自动设置步骤状态：当前步骤之前的项设为 `Finish`，当前步骤设为 `CurrentStepStatus`（默认 `Process`），之后的项设为 `Wait`。

### 展示风格（StepsStyle）

| 风格 | 说明 | 典型用途 |
|---|---|---|
| `Default` | 标准步骤条，带数字/图标指示器和连接线 | 表单向导、流程引导 |
| `Navigation` | 导航式步骤条，带底部指示线和箭头 | 顶部导航、分步操作 |
| `Inline` | 内联步骤条，紧凑的点状展示 | 列表项内嵌状态展示 |

### 指示器类型（StepsItemIndicatorType）

| 类型 | 说明 |
|---|---|
| `Default` | 数字圆圈或自定义图标 |
| `Dot` | 小圆点指示器 |

### 方向与标签位置

- **Orientation**：`Horizontal`（水平排列，默认）/ `Vertical`（垂直排列）
- **LabelPlacement**：`Horizontal`（标题在指示器右侧，默认）/ `Vertical`（标题在指示器下方）

### 步骤内容（Content）

Steps 支持为每个步骤关联内容（通过 `StepsItem.Content`）。当前步骤的内容可通过 `Steps.CurrentContent` 和 `Steps.CurrentContentTemplate` 访问，配合 `ContentPresenter` 实现步骤内容区域的动态切换。

### 进度环

当 `IsShowItemProgress = true` 时，当前步骤的指示器会显示环形进度条，进度值由 `ProgressValue`（0-100）控制。进度环仅在 Default 指示器类型和非 Inline 风格下生效。

---

## 与 Ant Design 规范的对齐程度

| 特性 | Ant Design (React) | AtomUI (Avalonia) | 对齐情况 |
|---|---|---|---|
| 基础步骤条 | ✅ | ✅ Steps + StepsItem | ✅ 完全对齐 |
| 四种状态 | ✅ wait/process/finish/error | ✅ Wait/Process/Finish/Error | ✅ 完全对齐 |
| `current` | ✅ 当前步骤索引 | ✅ `CurrentStep` | ✅ 完全对齐 |
| `status` | ✅ 当前步骤状态 | ✅ `CurrentStepStatus` | ✅ 完全对齐 |
| 水平/垂直方向 | ✅ `direction` | ✅ `Orientation` | ✅ 完全对齐 |
| 标签位置 | ✅ `labelPlacement` | ✅ `LabelPlacement` | ✅ 完全对齐 |
| 迷你版本 | ✅ `size="small"` | ✅ `SizeType="Small"` | ✅ 完全对齐 |
| 自定义图标 | ✅ `icon` | ✅ `StepsItem.Icon` | ✅ 完全对齐 |
| 描述文本 | ✅ `description` | ✅ `StepsItem.Description` | ✅ 完全对齐 |
| 副标题 | ✅ `subTitle` | ✅ `StepsItem.SubHeader` | ✅ 完全对齐 |
| 点状样式 | ✅ `progressDot` | ✅ `ItemIndicatorType="Dot"` | ✅ 完全对齐 |
| 可点击 | ✅ `onChange` 回调 | ✅ `IsItemClickable` + 选择事件 | ✅ 完全对齐 |
| 导航风格 | ✅ `type="navigation"` | ✅ `Style="Navigation"` | ✅ 完全对齐 |
| 内联风格 | ✅ `type="inline"` | ✅ `Style="Inline"` | ✅ 完全对齐 |
| 步骤进度 | ✅ `percent` | ✅ `ProgressValue` + `IsShowItemProgress` | ✅ 完全对齐 |
| `initial` | ✅ 起始步骤号 | ✅ `InitialStep` | ✅ 完全对齐 |

---

## 继承关系

```
Avalonia.Controls.Primitives.TemplatedControl
  └── Avalonia.Controls.ItemsControl
        └── Avalonia.Controls.SelectingItemsControl
              └── AtomUI.Desktop.Controls.Steps
                    └── implements ISizeTypeAware, IMotionAwareControl

Avalonia.Controls.Primitives.TemplatedControl
  └── Avalonia.Controls.Primitives.HeaderedContentControl
        └── AtomUI.Desktop.Controls.StepsItem
              └── implements ISelectable

Avalonia.Controls.Primitives.TemplatedControl
  └── AtomUI.Desktop.Controls.StepsItemIndicator (internal)
```

> 注意：Steps 控件家族均直接定义在 `AtomUI.Desktop.Controls` 中，没有对应的 `AtomUI.Controls` 设备无关基类。

---

## 源码位置

| 层级 | 文件路径 | 说明 |
|---|---|---|
| 步骤条容器 | `src/AtomUI.Desktop.Controls/Steps/Steps.cs` | Steps 实现（含枚举定义） |
| 步骤项 | `src/AtomUI.Desktop.Controls/Steps/StepsItem.cs` | StepsItem 实现 |
| 步骤指示器 | `src/AtomUI.Desktop.Controls/Steps/StepsItemIndicator.cs` | 内部指示器控件 |
| 伪类常量 | `src/AtomUI.Desktop.Controls/Steps/StepsPseudoClass.cs` | `:finished` 伪类 |
| Token 定义 | `src/AtomUI.Desktop.Controls/Steps/StepsToken.cs` | 组件级 Design Token（~50+ Token） |
| Steps 主题 | `src/AtomUI.Desktop.Controls/Steps/Themes/StepsTheme.axaml` | Steps ControlTheme |
| StepsItem 主题 | `src/AtomUI.Desktop.Controls/Steps/Themes/StepsItemTheme.axaml` | StepsItem ControlTheme（~1249 行） |
| 指示器主题 | `src/AtomUI.Desktop.Controls/Steps/Themes/StepsItemIndicatorTheme.axaml` | StepsItemIndicator ControlTheme |
| 主题常量 | `src/AtomUI.Desktop.Controls/Steps/Themes/StepsThemeConstants.cs` | 模板部件名常量 |
| Gallery 示例 | `controlgallery/AtomUIGallery/ShowCases/Views/Navigation/StepsShowCase.axaml` | 使用范例 |

---

## 模板结构

### Steps 模板

```
ItemsPresenter#PART_ItemsPresenter
└── Grid (ItemsPanel，行/列间距由 Token 控制)
    ├── StepsItem[0] (Grid.Column=0 或 Grid.Row=0)
    ├── StepsItem[1] (Grid.Column=1 或 Grid.Row=1)
    └── ...
```

Steps 使用 `Grid` 作为 ItemsPanel，水平模式下使用 `ColumnDefinitions`（前 N-1 列 Star，最后一列 Auto），垂直模式下使用 `RowDefinitions`（全部 Auto）。

### StepsItem 模板（简化描述）

StepsItem 的模板因风格（Default/Navigation/Inline）和方向（Horizontal/Vertical）不同而有显著差异，但核心结构为：

```
DockPanel#RootLayout
├── StepsItemIndicator#PART_Indicator      ← 步骤指示器（数字/图标/点）
├── Rectangle.IndicatorLine                ← 连接线（连接到下一步骤）
├── ContentPresenter#HeaderPresenter       ← 标题
├── ContentPresenter#SubHeaderPresenter    ← 副标题
└── ContentPresenter#DescriptionPresenter  ← 描述
```
