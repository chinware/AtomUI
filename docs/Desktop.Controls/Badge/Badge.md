# Badge 徽标数

## 概述

徽标数（Badge）用于在控件的右上角显示小型数字或状态标记，通常用于提醒用户有未处理的消息或通知。在界面设计中，Badge 是一种轻量级的信息标记手段，它不打断用户的主流程，却能有效地传递「这里有新动态」的信号。

AtomUI 提供三种 Badge 变体，分别覆盖不同的标记场景：

- **CountBadge**：数字徽标，显示具体消息数量
- **DotBadge**：圆点徽标，仅显示有/无状态标记
- **RibbonBadge**：缎带徽标，以缎带形式显示在容器角落

AtomUI 的 Badge 系列控件完整复刻了 [Ant Design 5.0 Badge](https://ant.design/components/badge-cn) 的设计规范，在 .NET / Avalonia 平台上提供与 Ant Design React 版一致的视觉体验和交互行为。

---

## 设计原理

### Ant Design 的 Badge 设计哲学

Ant Design 对 Badge 的定位是：**「图标右上角的圆形徽标数字」**。它的核心设计意图包括：

- **消息提醒**：以数字形式告知用户未读消息、待办事项的数量
- **状态指示**：以彩色小圆点标记在线/离线、成功/错误等状态
- **标签装饰**：以缎带形式在卡片角落展示促销、推荐等信息

Ant Design 将 Badge 设计为一个**装饰器**（Decorator）模式的组件——它不是独立存在的控件，而是「附着」在其他控件上的视觉标记。同时也支持脱离宿主独立使用，作为独立的状态指示器。

**三种 Badge 变体及其设计意图：**

| 变体 | 设计意图 | 典型用途 |
|---|---|---|
| 📛 **CountBadge（数字徽标）** | 显示精确数量，让用户快速判断待处理项的规模。超过封顶值时显示 `99+`，避免数字过长破坏布局 | 未读消息数、购物车商品数、通知计数 |
| 🔴 **DotBadge（圆点徽标）** | 最轻量的提示，仅告知「有/无」状态。支持五种语义颜色（成功/处理中/默认/错误/警告）和自定义颜色 | 在线状态、有新消息提示、Tab 页状态指示 |
| 🎀 **RibbonBadge（缎带徽标）** | 以缎带形式在容器角落展示文字标签，视觉上更具装饰性。支持左上角/右上角放置 | 卡片角标（「热门」「新品」「促销」）、标签分类 |

### Avalonia 基础能力

AtomUI 的三种 Badge 变体均直接继承自 `Avalonia.Controls.Control`，而非 `TemplatedControl`。Badge 采用 **Adorner 模式**实现装饰功能——当设置了 `DecoratedTarget` 时，Badge 将目标控件作为视觉子元素承载，同时通过 `AdornerLayer` 在目标控件上方叠加显示标记。

**Adorner 模式的工作原理：**

```
AdornerLayer（覆盖在所有控件之上的透明层）
├── CountBadgeAdorner / DotBadgeAdorner / RibbonBadgeAdorner
│   └── 标记内容（数字、圆点、缎带）
│
Control Tree
├── CountBadge / DotBadge / RibbonBadge
│   └── DecoratedTarget（被装饰的目标控件）
```

这种设计确保 Badge 标记不受目标控件的 `ClipToBounds` 影响，可以自由溢出到目标控件边界之外。

### AtomUI 的扩展设计

| 扩展能力 | 实现方式 | 设计动机 |
|---|---|---|
| **装饰器模式** | `DecoratedTarget` + `AdornerLayer` | Badge 标记可溢出到目标控件边界外，不受裁剪影响 |
| **独立模式** | 不设 `DecoratedTarget` | 脱离宿主控件独立使用，作为行内状态指示器 |
| **自定义颜色** | `BadgeColor` / `DotColor` / `RibbonColor`（支持预设色名和自定义色值） | 对齐 Ant Design 的 `color` 属性，支持 13 种预设色和任意 CSS 颜色值 |
| **显隐动画** | `BadgeZoomBadgeInMotion` / `BadgeZoomBadgeOutMotion` | 标记出现/消失时的缩放动画，提升交互体验 |
| **偏移量** | `Offset` 属性（`Point` 类型） | 精细调整标记位置，适应不同目标控件的形状 |
| **Design Token** | `BadgeToken` + `RegisterTokenResourceScope` | 所有视觉值从全局/组件级 Token 派生，支持主题切换 |

---

## 功能详解

### CountBadge 数字徽标

数字徽标是最常用的 Badge 变体，用于显示精确的消息数量。

**核心行为：**
- 当 `Count > 0` 时显示数字
- 当 `Count > OverflowCount` 时显示 `{OverflowCount}+`（如 `99+`）
- 当 `Count == 0` 时默认隐藏，设置 `ShowZero = true` 可强制显示
- 支持 `Default` 和 `Small` 两种尺寸（通过 `Size` 属性控制）

**装饰模式 vs 独立模式：**
- **装饰模式**：设置 `DecoratedTarget` 后，数字气泡显示在目标控件的右上角
- **独立模式**：不设 `DecoratedTarget`，数字气泡作为独立元素显示

### DotBadge 圆点徽标

圆点徽标是最轻量的 Badge 变体，仅显示一个彩色小圆点，不包含任何数字。

**五种状态语义：**

| 状态 | 颜色来源 | 语义 |
|---|---|---|
| `Success` | `SharedToken.ColorSuccess` | 成功、在线、正常 |
| `Processing` | `SharedToken.ColorInfo` | 处理中、同步中 |
| `Default` | `SharedToken.ColorTextPlaceholder` | 默认、无特殊状态 |
| `Error` | `SharedToken.ColorError` | 错误、离线、异常 |
| `Warning` | `SharedToken.ColorWarning` | 警告、需注意 |

**独立模式扩展**：不设 `DecoratedTarget` 时，DotBadge 可通过 `Text` 属性在圆点旁显示说明文字，形成「圆点 + 文字」的状态指示器。

### RibbonBadge 缎带徽标

缎带徽标以缎带形式显示在容器的角落，通常用于卡片、列表项的装饰性标注。

**放置位置**：通过 `Placement` 属性控制缎带显示在右上角（`End`，默认）或左上角（`Start`）。缎带下方会自动生成一个加深色的三角形折角，增强立体感。

**颜色系统**：默认使用主题主色（`ColorPrimary`），可通过 `RibbonColor` 属性设置为 13 种预设色名或自定义色值。

### 颜色系统

三种 Badge 变体均支持自定义颜色。颜色值通过 `BadgeColorUtils.CalculateColor()` 解析，支持两种输入格式：

1. **预设色名**（字符串，不区分大小写）：`Pink`、`Red`、`Yellow`、`Orange`、`Cyan`、`Green`、`Blue`、`Purple`、`GeekBlue`、`Magenta`、`Volcano`、`Gold`、`Lime`
2. **CSS 颜色值**（字符串）：`#f50`、`#ff5500`、`rgb(45, 183, 245)`、`hsl(102, 53%, 61%)` 等任意 Avalonia 可解析的颜色字符串

### 显隐动画

CountBadge 和 DotBadge 支持显隐动画（通过 `IMotionAwareControl` 接口）。当 `BadgeIsVisible` 属性变化时：
- **显示**：`BadgeZoomBadgeInMotion` — 从 `scale(0.01)` + `opacity(0)` 缩放渐入到 `scale(1)` + `opacity(1)`，使用 `ExponentialEaseOut` 缓动
- **隐藏**：`BadgeZoomBadgeOutMotion` — 从 `scale(1)` + `opacity(1)` 缩放渐出到 `scale(0.01)` + `opacity(0)`，使用 `ExponentialEaseIn` 缓动

---

## 与 Ant Design 规范的对齐程度

| 特性 | Ant Design (React) | AtomUI (Avalonia) | 对齐情况 |
|---|---|---|---|
| 数字徽标 `count` | ✅ `count` | ✅ `CountBadge.Count` | ✅ 完全对齐 |
| 封顶数字 `overflowCount` | ✅ `overflowCount` | ✅ `CountBadge.OverflowCount` | ✅ 完全对齐 |
| 显示零值 `showZero` | ✅ `showZero` | ✅ `CountBadge.ShowZero` | ✅ 完全对齐 |
| 小号尺寸 `size` | ✅ `size="small"` | ✅ `CountBadge.Size="Small"` | ✅ 完全对齐 |
| 小红点 `dot` | ✅ `dot` 属性 | ✅ `DotBadge` 独立控件 | ✅ 完全对齐（实现方式不同，效果一致） |
| 状态圆点 `status` | ✅ `status` | ✅ `DotBadge.Status` | ✅ 完全对齐 |
| 状态文本 `text` | ✅ `text` | ✅ `DotBadge.Text` | ✅ 完全对齐 |
| 缎带 `Badge.Ribbon` | ✅ `Badge.Ribbon` 子组件 | ✅ `RibbonBadge` 独立控件 | ✅ 完全对齐 |
| 缎带位置 `placement` | ✅ `start / end` | ✅ `RibbonBadgePlacement.Start / End` | ✅ 完全对齐 |
| 自定义颜色 `color` | ✅ 预设色 + 自定义色 | ✅ `BadgeColor` / `DotColor` / `RibbonColor` | ✅ 完全对齐 |
| 独立使用 | ✅ 不包裹子元素 | ✅ 不设 `DecoratedTarget` | ✅ 完全对齐 |
| 偏移量 `offset` | ✅ `[x, y]` 数组 | ✅ `Offset`（`Point` 类型） | ✅ 完全对齐 |
| 动态显隐动画 | ✅ CSS 动画 | ✅ `BadgeZoomBadgeIn/OutMotion` | ✅ 完全对齐 |
| `count` 为 ReactNode | ✅ 支持自定义渲染 | ❌ 仅支持数字 | ⚠️ 暂不支持自定义内容 |
| `title` 悬浮提示 | ✅ `title` 属性 | ❌ 暂未实现 | ⚠️ 可通过 `ToolTip.Tip` 实现 |
| `classNames` / `styles` | ✅ 细粒度样式覆盖 | ❌ 不适用 | — Avalonia 使用 Style 选择器 |

---

## 继承关系

```
Avalonia.Controls.Control
├── AtomUI.Controls.Commons.AbstractCountBadge (IMotionAwareControl)
│     └── AtomUI.Desktop.Controls.CountBadge
│
├── AtomUI.Controls.Commons.AbstractDotBadge (IMotionAwareControl)
│     └── AtomUI.Desktop.Controls.DotBadge
│
└── AtomUI.Controls.Commons.AbstractRibbonBadge
      └── AtomUI.Desktop.Controls.RibbonBadge
```

与 Button 不同，Badge 系列控件不继承自任何 Avalonia 内建控件，而是直接继承 `Control`。这是因为 Badge 的交互模型（装饰器 + Adorner）与 Avalonia 标准控件的模板化模型不同。

**各层级职责划分：**

| 层级 | 提供的能力 |
|---|---|
| `Avalonia.Controls.Control` | 基础视觉树管理、属性系统、布局参与 |
| `Abstract*Badge`（`AtomUI.Controls`） | 跨平台共享行为：属性定义、装饰器/独立模式切换、Adorner 管理、颜色解析、显隐逻辑、动画触发 |
| 具体 Badge（`AtomUI.Desktop.Controls`） | 桌面端特化：注册 `BadgeToken` 作用域、创建桌面端 Adorner 实例 |

**实现的共享接口：**

| 接口 | 控件 | 作用 |
|---|---|---|
| `IMotionAwareControl` | `CountBadge`、`DotBadge` | 支持 `IsMotionEnabled` 属性，控制显隐动画 |

> 注意：`RibbonBadge` 不实现 `IMotionAwareControl`，因为缎带徽标不支持显隐动画。

---

## 源码位置

| 层级 | 文件路径 | 说明 |
|---|---|---|
| 基类 | `src/AtomUI.Controls/Badge/AbstractCountBadge.cs` | 数字徽标基类（跨平台） |
| 基类 | `src/AtomUI.Controls/Badge/AbstractDotBadge.cs` | 圆点徽标基类（跨平台） |
| 基类 | `src/AtomUI.Controls/Badge/AbstractRibbonBadge.cs` | 缎带徽标基类（跨平台） |
| 基类 | `src/AtomUI.Controls/Badge/AbstractCountBadgeAdorner.cs` | 数字徽标装饰器基类 |
| 基类 | `src/AtomUI.Controls/Badge/AbstractDotBadgeAdorner.cs` | 圆点徽标装饰器基类 |
| 基类 | `src/AtomUI.Controls/Badge/AbstractRibbonBadgeAdorner.cs` | 缎带徽标装饰器基类 |
| 工具类 | `src/AtomUI.Controls/Badge/BadgeColorUtils.cs` | 颜色解析工具 |
| 工具类 | `src/AtomUI.Controls/Badge/DotBadgeIndicator.cs` | 圆点绘制控件 |
| 动画 | `src/AtomUI.Controls/Badge/BadgeMotion.cs` | 显隐缩放动画 |
| 控件类 | `src/AtomUI.Desktop.Controls/Badge/CountBadge.cs` | 桌面端数字徽标 |
| 控件类 | `src/AtomUI.Desktop.Controls/Badge/DotBadge.cs` | 桌面端圆点徽标 |
| 控件类 | `src/AtomUI.Desktop.Controls/Badge/RibbonBadge.cs` | 桌面端缎带徽标 |
| Token 定义 | `src/AtomUI.Desktop.Controls/Badge/BadgeToken.cs` | 组件级 Design Token |
| 主题模板 | `src/AtomUI.Desktop.Controls/Badge/Themes/CountBadgeAdornerTheme.axaml` | 数字徽标装饰器主题 |
| 主题模板 | `src/AtomUI.Desktop.Controls/Badge/Themes/DotBadgeAdornerTheme.axaml` | 圆点徽标装饰器主题 |
| 主题模板 | `src/AtomUI.Desktop.Controls/Badge/Themes/DotBadgeIndicatorTheme.axaml` | 圆点指示器主题 |
| 主题模板 | `src/AtomUI.Desktop.Controls/Badge/Themes/RibbonBadgeAdornerTheme.axaml` | 缎带徽标装饰器主题 |
| 主题注册 | `src/AtomUI.Desktop.Controls/Badge/Themes/BadgeThemes.axaml` | 主题资源注册 |
| Gallery 示例 | `controlgallery/AtomUIGallery/ShowCases/Views/DataDisplay/BadgeShowCase.axaml` | 使用范例 |

---

## 模板结构

### CountBadgeAdorner 模板

```
MotionActor (PART_MotionActor)          ← 显隐缩放动画容器
└── Panel#RootLayout                    ← 根布局
    ├── Border#BadgeIndicator           ← 红色圆角背景（承载 BoxShadow）
    └── TextBlock#BadgeText             ← 数字文本（居中显示）
```

### DotBadgeAdorner 模板

```
DockPanel#RootLayout                    ← 根布局（水平排列）
├── MotionActor (PART_MotionActor)      ← 显隐缩放动画容器 (DockPanel.Dock="Left")
│   └── DotBadgeIndicator#Indicator     ← 圆点绘制控件（自定义 Render）
└── Label#Label                         ← 状态文本（仅独立模式可见）
```

### RibbonBadgeAdorner 模板

RibbonBadgeAdorner 使用自定义 `Render` 方法绘制缎带和三角折角，模板结构简洁：

```
Panel
└── TextBlock (PART_LabelPart)          ← 缎带文字
```

缎带背景和折角通过 `OnRender` 直接绘制：
- **缎带背景**：使用 `BorderRenderHelper` 绘制带圆角的矩形
- **三角折角**：使用 `StreamGeometry` 绘制三角形，颜色为缎带色的加深版（`Darken(15)`）
