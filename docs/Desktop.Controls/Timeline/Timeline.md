# Timeline 时间轴

## 概述

时间轴（Timeline）用于垂直展示一系列按时间排列的信息。常用于展示版本更新记录、操作日志、事件追踪等按时间顺序组织的数据。每个节点由指示器（圆点或自定义图标）和内容组成，通过尾线连接形成视觉上的时间流。

AtomUI 的 `Timeline` 控件复刻了 [Ant Design 5.0 Timeline](https://ant.design/components/timeline-cn) 的设计规范，在 .NET / Avalonia 平台上提供与 Ant Design React 版一致的视觉体验。

---

## 设计原理

### Ant Design 的时间轴设计哲学

Ant Design 对时间轴的定位是：**「垂直展示的时间流信息」**。时间轴通常出现在以下场景：
- **事件记录**：按时间顺序展示一系列操作或事件
- **版本日志**：展示产品版本更新历史
- **审批流程**：展示多步骤流程的执行状态
- **进度追踪**：展示任务进展和未来计划

**三种布局模式**：

| 模式 | 设计意图 | 典型用途 |
|---|---|---|
| 📍 **左对齐（Left）** | 指示器在左侧，内容在右侧。最常见的布局方式 | 基本时间轴、操作日志 |
| 📍 **右对齐（Right）** | 指示器在右侧，内容在左侧 | 从右向左阅读场景 |
| 📍 **交替（Alternate）** | 内容交替出现在指示器的左右两侧 | 双栏时间轴、里程碑展示 |

**关键特性**：
- **指示器颜色**：支持自定义颜色，用于区分不同状态（绿色=成功、红色=错误、蓝色=进行中、灰色=未完成）
- **自定义图标**：可用 Ant Design 图标替代默认圆点
- **待办节点（Pending）**：在时间轴末尾添加一个"进行中"的幽灵节点，表示事件尚未完成
- **反转显示**：可倒序排列时间节点
- **标签（Label）**：可为每个节点附加标签文本（如日期），在双栏布局中显示

### AtomUI 的扩展设计

AtomUI `Timeline` 在 Ant Design 规范基础上做了以下实现：

| 扩展能力 | 实现方式 | 设计动机 |
|---|---|---|
| **三种布局模式** | `TimeLineMode` 枚举（Left/Right/Alternate） | 对齐 Ant Design 的 `mode` 属性 |
| **指示器颜色** | `IndicatorColor`（`IBrush?`）属性 | 支持任意画刷，比 Ant Design 的字符串颜色更灵活 |
| **自定义图标** | `IndicatorIcon`（`PathIcon?`）属性 | 替代默认圆点，与 AtomUI 图标系统集成 |
| **待办节点** | `Pending` + `PendingIcon` 属性 | 自动在末尾生成带旋转 Loading 图标的幽灵节点 |
| **反转排列** | `IsReverse` 属性 | 倒序展示时间节点 |
| **标签系统** | `Label` 属性 + 自动检测标签布局 | 任一节点设置 Label 后自动切换为双栏标签布局 |
| **Design Token** | `TimelineToken` + `RegisterTokenResourceScope` | 所有视觉值从 Token 派生 |

---

## 功能详解

### 布局模式（Mode）

Timeline 通过 `Mode` 属性控制内容和指示器的相对位置：

| 模式 | 无 Label 时 | 有 Label 时 |
|---|---|---|
| **Left** | 指示器靠左、内容在右 | 标签在左、指示器居中、内容在右 |
| **Right** | 指示器靠右、内容在左 | 内容在左、指示器居中、标签在右 |
| **Alternate** | 内容在指示器左右交替 | 标签和内容在指示器左右交替 |

当任一 `TimelineItem` 设置了 `Label` 属性时，所有节点自动切换为**标签布局**（双栏），指示器居中，标签和内容分列两侧。

### 待办节点（Pending）

当 `Pending` 属性不为 `null` 时：
1. 自动在时间轴末尾添加一个幽灵节点
2. 默认使用 `LoadingOutlined` 旋转图标作为指示器
3. 可通过 `PendingIcon` 自定义幽灵节点的图标
4. 幽灵节点前一个节点的下方间距自动加大（`ItemPaddingBottomLG`）

### 反转排列（IsReverse）

当 `IsReverse = true` 时：
- 所有节点在视觉上倒序排列
- 待办节点仍然出现在尾部（逻辑上的最后位置）
- 节点位置信息（IsFirst/IsLast/IsOdd）会重新计算

### 指示器（TimelineIndicator）

每个 `TimelineItem` 包含一个内部的 `TimelineIndicator` 控件，负责渲染：
1. **指示器圆点**：默认空心圆，通过 `IndicatorDotSize` 和 `IndicatorDotBorderWidth` 控制
2. **自定义图标**：当 `IndicatorIcon` 不为 null 时，图标替代圆点
3. **尾线**：连接相邻节点的竖线，通过 `IndicatorTailColor` 和 `IndicatorTailWidth` 控制
4. 首节点不绘制上方尾线，末节点不绘制下方尾线

---

## 与 Ant Design 规范的对齐程度

| 特性 | Ant Design (React) | AtomUI (Avalonia) | 对齐情况 |
|---|---|---|---|
| 布局模式 `mode` | ✅ left/right/alternate | ✅ `TimeLineMode` 枚举 | ✅ 完全对齐 |
| 指示器颜色 `color` | ✅ 字符串颜色 | ✅ `IndicatorColor`（IBrush） | ✅ 对齐（类型更灵活） |
| 自定义圆点 `dot` | ✅ ReactNode | ✅ `IndicatorIcon`（PathIcon） | ✅ 对齐（类型不同，语义一致） |
| 待办节点 `pending` | ✅ boolean/ReactNode | ✅ `Pending`（object） | ✅ 完全对齐 |
| 待办图标 `pendingDot` | ✅ ReactNode | ✅ `PendingIcon`（PathIcon） | ✅ 完全对齐 |
| 反转 `reverse` | ✅ boolean | ✅ `IsReverse` | ✅ 完全对齐 |
| 标签 `label` | ✅ ReactNode | ✅ `Label`（string） | ⚠️ 仅支持字符串，不支持任意内容 |
| 子项 `children` | ✅ ReactNode | ✅ `Content`（object） | ✅ 完全对齐 |

---

## 继承关系

### Timeline

```
Avalonia.Controls.ItemsControl
  └── AtomUI.Controls.Commons.AbstractTimeline    ← 设备无关基类
        └── AtomUI.Desktop.Controls.Timeline       ← 桌面实现
```

### TimelineItem

```
Avalonia.Controls.ContentControl
  └── AtomUI.Controls.Commons.AbstractTimelineItem  ← 设备无关基类
        └── AtomUI.Desktop.Controls.TimelineItem     ← 桌面实现
```

**各层级职责划分（Timeline）：**

| 层级 | 提供的能力 |
|---|---|
| `ItemsControl` | 子项容器基础设施、`Items` / `ItemTemplate`、容器生成与回收 |
| `AbstractTimeline`（基类层） | `Mode` 布局模式、`Pending`/`PendingIcon` 待办节点、`IsReverse` 反转、子项位置信息计算（IsOdd/IsFirst/IsLast）、标签布局自动检测 |
| `Timeline`（桌面层） | 注册 `TimelineToken.ScopeProvider`、创建桌面端 `TimelineItem` 容器、创建待办节点（默认 `LoadingOutlined` 图标） |

**各层级职责划分（TimelineItem）：**

| 层级 | 提供的能力 |
|---|---|
| `ContentControl` | 任意内容容纳、`Content` / `ContentTemplate` |
| `AbstractTimelineItem`（基类层） | `Label` 标签、`IndicatorIcon` 自定义图标、`IndicatorColor` 指示器颜色、位置伪类管理（order-odd/order-even/order-first/order-last/pending-item） |
| `TimelineItem`（桌面层） | 注册 `TimelineToken.ScopeProvider` |

### 内部辅助控件

| 控件 | 位置 | 作用 |
|---|---|---|
| `TimelineIndicator` | `AtomUI.Controls` (internal) | 渲染指示器圆点/图标和尾线 |
| `TimelineItemPanel` | `AtomUI.Controls` (internal) | 自定义 Panel，根据 Mode 和 IsLabelLayout 安排标签、指示器、内容的布局 |
| `TimelineStackPanel` | `AtomUI.Controls` (internal) | 支持 `IsReverse` 的 StackPanel，实现子项反转排列 |

---

## 源码位置

| 层级 | 文件路径 | 说明 |
|---|---|---|
| 基类 | `src/AtomUI.Controls/Timeline/AbstractTimeline.cs` | Timeline 设备无关基类 |
| 基类 | `src/AtomUI.Controls/Timeline/AbstractTimelineItem.cs` | TimelineItem 设备无关基类 |
| 枚举 | `src/AtomUI.Controls/Timeline/TimeLineEnums.cs` | TimeLineMode 枚举 |
| 内部控件 | `src/AtomUI.Controls/Timeline/TimelineIndicator.cs` | 指示器渲染控件 |
| 内部控件 | `src/AtomUI.Controls/Timeline/TimelineItemPanel.cs` | 节点布局面板 |
| 内部控件 | `src/AtomUI.Controls/Timeline/TimelineStackPanel.cs` | 支持反转的 StackPanel |
| 控件类 | `src/AtomUI.Desktop.Controls/Timeline/Timeline.cs` | 桌面端 Timeline |
| 控件类 | `src/AtomUI.Desktop.Controls/Timeline/TimelineItem.cs` | 桌面端 TimelineItem |
| Token 定义 | `src/AtomUI.Desktop.Controls/Timeline/TimelineToken.cs` | 组件级 Design Token |
| 主题模板 | `src/AtomUI.Desktop.Controls/Timeline/Themes/TimelineTheme.axaml` | Timeline ControlTheme |
| 主题模板 | `src/AtomUI.Desktop.Controls/Timeline/Themes/TimelineItemTheme.axaml` | TimelineItem ControlTheme |
| 主题模板 | `src/AtomUI.Desktop.Controls/Timeline/Themes/TimelineIndicatorTheme.axaml` | TimelineIndicator ControlTheme |
| Gallery 示例 | `controlgallery/AtomUIGallery/ShowCases/Views/DataDisplay/TimelineShowCase.axaml` | 使用范例 |

---

## 模板结构

### Timeline 模板

```
Border#Frame                                      ← 外框（背景、边框、圆角）
└── ScrollViewer#ScrollViewer (Lite Mode)          ← 滚动容器
    └── ItemsPresenter#ItemsPresenter             ← 子项呈现
        └── TimelineStackPanel (IsReverse)         ← 支持反转的 StackPanel 布局
```

### TimelineItem 模板

```
TimelineItemPanel#RootLayout                      ← 自定义布局面板
├── TextBlock#Label                                ← 标签文本（标签布局时可见）
├── TimelineIndicator#Indicator                    ← 指示器（圆点/图标 + 尾线）
│   └── IconPresenter#PART_IconPresenter           ← 自定义图标呈现
└── ContentPresenter#ContentPresenter              ← 内容呈现
```

**布局逻辑说明：**
- `TimelineItemPanel` 根据 `Mode` 和 `IsLabelLayout` 计算三个子元素（Label/Indicator/Content）的位置
- 在标签布局或 Alternate 模式下，Label 和 Content 各占 `(总宽度 - 指示器宽度) / 2`
- 在纯 Left/Right 模式下，Content 占 `总宽度 - 指示器宽度`
- Alternate 模式下，奇偶节点的 Label 和 Content 位置互换
