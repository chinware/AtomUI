# Statistic 统计数值

## 概述

统计数值（Statistic）是用于展示统计数据的展示类控件。当需要在页面中突出显示某个或某组数字时，如统计数值、倒计时、动画数值等，可以使用 Statistic 组件。

AtomUI 的 Statistic 控件家族完整复刻了 [Ant Design 5.0 Statistic](https://ant.design/components/statistic-cn) 的设计规范，包含三个控件：

| 控件 | 说明 |
|---|---|
| **`Statistic`** | 基础统计数值，展示格式化的数字或文本 |
| **`TimerStatistic`** | 计时器/倒计时，基于目标时间自动更新剩余时间 |
| **`StatisticCountUp`** | 数值递增动画，从 0 动画过渡到目标数值 |

---

## 设计原理

### Ant Design 的统计数值设计哲学

Ant Design 对 Statistic 的定位是：**「当需要突出某个或某组数字时，或展示带描述的统计类数据时使用」**。其核心设计理念包括：

- **标题 + 数值**的双层结构，标题提供语义上下文，数值是视觉焦点
- 支持**前缀/后缀**装饰，可放置图标、单位等辅助信息
- 支持**数值格式化**（千分位分隔符、小数点、精度控制）
- 支持**倒计时**（Countdown）模式，自动刷新剩余时间
- 支持**加载骨架屏**（Skeleton），在数据未就绪时显示占位

### 控件架构设计

AtomUI 的 Statistic 家族采用了**基类 + 派生类**的设计模式：

```
HeaderedContentControl (Avalonia)
  └── AbstractStatistic (共享基类)
        ├── Statistic (基础统计数值)
        └── TimerStatistic (计时器/倒计时)

TemplatedControl (Avalonia)
  └── StatisticCountUp (数值递增动画，作为 Statistic 的 Content 嵌入)
```

**`AbstractStatistic`** 是抽象基类，定义了所有统计数值控件共享的属性和模板结构：
- 标题（`Header`，继承自 `HeaderedContentControl`）
- 数值格式化参数（`DecimalSeparator`、`GroupSeparator`、`Precision`）
- 前缀/后缀（`ValuePrefixAddOn`、`ValueSuffixAddOn`）
- 内容样式（`ContentForeground`、`ContentFontSize`）
- 加载状态（`IsLoading`）

**`Statistic`** 继承 `AbstractStatistic`，增加了 `Value` 属性和 `Formatter` 自定义格式化回调。

**`TimerStatistic`** 继承 `AbstractStatistic`，增加了基于 `DispatcherTimer` 的自动刷新机制，可用于倒计时和计时。

**`StatisticCountUp`** 是独立的控件（继承 `TemplatedControl`），通过 `DoubleTransition` 实现从 0 到目标值的动画过渡。它被设计为嵌入 `Statistic.Content` 中使用。

### Avalonia 基础能力

**HeaderedContentControl 提供的基础能力：**

| 属性 | 说明 |
|---|---|
| `Header` | 标题内容（在 Statistic 中用作统计项描述） |
| `HeaderTemplate` | 标题数据模板 |
| `Content` | 主内容区域（在 Statistic 中承载格式化后的数值或 StatisticCountUp） |
| `ContentTemplate` | 内容数据模板 |

### AtomUI 的扩展设计

| 扩展能力 | 实现方式 | 设计动机 |
|---|---|---|
| **数值格式化** | `StatisticUtils.FormatNumber` + `Precision` / `GroupSeparator` / `DecimalSeparator` | 自动千分位分隔、小数精度控制，无需手动格式化 |
| **前缀/后缀装饰** | `ValuePrefixAddOn` / `ValueSuffixAddOn` 属性 | 支持在数值前后放置图标、单位文本等 |
| **自定义格式化** | `Formatter` 回调（`Func<Statistic, object?, string?>`） | 完全自定义数值显示逻辑 |
| **加载骨架屏** | `IsLoading` + 内置 `Skeleton` 组件 | 数据加载中时显示占位骨架 |
| **倒计时/计时** | `TimerStatistic` + `DispatcherTimer` | 自动刷新剩余时间，支持毫秒级精度 |
| **数值递增动画** | `StatisticCountUp` + `DoubleTransition` | 从 0 平滑过渡到目标数值 |
| **Design Token 集成** | `StatisticToken` + 共享 Token 引用 | 标题/内容字体大小、颜色由 Token 驱动 |

---

## 功能详解

### Statistic（基础统计数值）

`Statistic` 是最常用的统计数值控件，用于展示一个格式化的数字或文本：

- 通过 `Value` 属性设置原始值（支持 `int`、`long`、`float`、`double`、`decimal` 等数值类型）
- 自动通过 `StatisticUtils.FormatNumber` 进行数值格式化（千分位、小数精度）
- 如果设置了 `Formatter` 回调，将优先使用自定义格式化逻辑
- 如果直接设置了 `Content`（例如嵌入 `StatisticCountUp`），则 `Content` 优先于 `Value` 显示

### TimerStatistic（计时器/倒计时）

`TimerStatistic` 基于目标 `DateTime` 自动计算并刷新剩余时间：

- 当 `Value`（目标时间）在未来 → **倒计时**模式，显示距目标时间的剩余时长
- 当 `Value`（目标时间）在过去 → **计时**模式，显示从目标时间至今的经过时长
- 当剩余时间归零时触发 `CountdownFinished` 事件，并停止计时器
- 通过 `Format` 属性控制时间显示格式（使用 .NET `TimeSpan` 格式字符串）
- 通过 `RefreshDuration` 属性控制刷新间隔（默认 10ms）
- 支持 `Formatter` 回调完全自定义时间文本

### StatisticCountUp（数值递增动画）

`StatisticCountUp` 实现了从 0 到目标值的平滑递增动画：

- 通过 `EndValue` 属性设置目标数值
- 控件加载时自动启动动画（`OnLoaded` → 将 `AnimatingValue` 设为 `EndValue`）
- 动画使用 `DoubleTransition` + `ExponentialEaseOut` 缓动曲线
- 动画时长由 `SharedToken.MotionDurationVerySlow` 控制
- 可嵌入 `Statistic.Content` 中，继承 Statistic 的标题和样式

### 加载骨架屏

当 `IsLoading = true` 时，数值区域会被 `Skeleton` 组件替换为加载占位骨架。标题仍然正常显示。加载态下的 `RootLayout` 间距会从 `SpacingXXS` 增大到 `Spacing`，给骨架屏更多视觉空间。

---

## 与 Ant Design 规范的对齐程度

| 特性 | Ant Design (React) | AtomUI (Avalonia) | 对齐情况 |
|---|---|---|---|
| 标题 + 数值结构 | ✅ `title` + `value` | ✅ `Header` + `Value` / `Content` | ✅ 完全对齐 |
| 前缀 `prefix` | ✅ ReactNode | ✅ `ValuePrefixAddOn`（支持图标和文本） | ✅ 完全对齐 |
| 后缀 `suffix` | ✅ ReactNode | ✅ `ValueSuffixAddOn`（支持图标和文本） | ✅ 完全对齐 |
| 格式化 `formatter` | ✅ 函数 | ✅ `Formatter` 委托 | ✅ 完全对齐 |
| 数值精度 `precision` | ✅ 数字 | ✅ `Precision` 属性 | ✅ 完全对齐 |
| 千分位分隔 `groupSeparator` | ✅ 字符串 | ✅ `GroupSeparator` 属性 | ✅ 完全对齐 |
| 小数分隔符 `decimalSeparator` | ✅ 字符串 | ✅ `DecimalSeparator` 属性 | ✅ 完全对齐 |
| 加载中 `loading` | ✅ 布尔 | ✅ `IsLoading`（使用 Skeleton 骨架屏） | ✅ 完全对齐 |
| 倒计时 `Countdown` | ✅ 独立组件 | ✅ `TimerStatistic` | ✅ 完全对齐 |
| 倒计时格式 `format` | ✅ dayjs 格式 | ✅ `Format`（.NET TimeSpan 格式） | ⚠️ 格式语法不同，功能一致 |
| 倒计时完成事件 `onFinish` | ✅ 回调 | ✅ `CountdownFinished` 事件 | ✅ 完全对齐 |
| 数值动画 `countUp` | ✅ 需配合第三方库 | ✅ `StatisticCountUp` 内置支持 | ✅ 对齐（AtomUI 内置实现） |

---

## 继承关系

```
Avalonia.Controls.Primitives.TemplatedControl
  └── Avalonia.Controls.Primitives.HeaderedContentControl
        └── AtomUI.Desktop.Controls.AbstractStatistic (抽象基类)
              ├── AtomUI.Desktop.Controls.Statistic (基础统计数值)
              └── AtomUI.Desktop.Controls.TimerStatistic (计时器/倒计时)

Avalonia.Controls.Primitives.TemplatedControl
  └── AtomUI.Desktop.Controls.StatisticCountUp (数值递增动画)
```

**各层级职责划分：**

| 层级 | 提供的能力 |
|---|---|
| `HeaderedContentControl` | 标题 + 内容的双区域结构、`Header` / `HeaderTemplate` / `Content` / `ContentTemplate` |
| `AbstractStatistic` | 数值格式化参数（`DecimalSeparator` / `GroupSeparator` / `Precision`）、前缀/后缀（`ValuePrefixAddOn` / `ValueSuffixAddOn`）、内容样式（`ContentForeground` / `ContentFontSize`）、加载状态（`IsLoading`）、Design Token 注册 |
| `Statistic` | `Value` 属性、`Formatter` 自定义格式化、`EffectiveValue` 内部计算、与 `StatisticCountUp` 的集成 |
| `TimerStatistic` | 基于 `DateTime` 的自动计时、`DispatcherTimer` 刷新、`Format` 时间格式、`RefreshDuration` 刷新间隔、`CountdownFinished` 事件 |
| `StatisticCountUp` | `EndValue` 目标值、`AnimatingValue` 动画值、`DoubleTransition` 递增动画、`ExponentialEaseOut` 缓动 |

> 注意：Statistic 家族的所有控件均直接定义在 `AtomUI.Desktop.Controls` 中，没有对应的 `AtomUI.Controls` 设备无关基类。

---

## 源码位置

| 层级 | 文件路径 | 说明 |
|---|---|---|
| 抽象基类 | `src/AtomUI.Desktop.Controls/Statistic/AbstractStatistic.cs` | 共享的属性、模板逻辑 |
| 基础统计 | `src/AtomUI.Desktop.Controls/Statistic/Statistic.cs` | Statistic 实现 |
| 数值动画 | `src/AtomUI.Desktop.Controls/Statistic/StatisticCountUp.cs` | StatisticCountUp 实现 |
| 计时器 | `src/AtomUI.Desktop.Controls/Statistic/TimerStatistic.cs` | TimerStatistic 实现 |
| 工具类 | `src/AtomUI.Desktop.Controls/Statistic/StatisticUtils.cs` | 数值格式化工具 |
| Token 定义 | `src/AtomUI.Desktop.Controls/Statistic/StatisticToken.cs` | 组件级 Design Token |
| 基类主题 | `src/AtomUI.Desktop.Controls/Statistic/Themes/AbstractStatisticTheme.axaml` | AbstractStatistic 模板 |
| 统计主题 | `src/AtomUI.Desktop.Controls/Statistic/Themes/StatisticTheme.axaml` | Statistic 主题 |
| 计时主题 | `src/AtomUI.Desktop.Controls/Statistic/Themes/TimerStatisticTheme.axaml` | TimerStatistic 主题 |
| 动画主题 | `src/AtomUI.Desktop.Controls/Statistic/Themes/StatisticCountUpTheme.axaml` | StatisticCountUp 主题 |
| 注册文件 | `src/AtomUI.Desktop.Controls/Statistic/Themes/StatisticThemes.axaml` | 主题注册 |
| Gallery 示例 | `controlgallery/AtomUIGallery/ShowCases/Views/DataDisplay/StatisticShowCase.axaml` | 使用范例 |

---

## 模板结构

### AbstractStatistic / Statistic 模板

```
Border#Frame (Background, CornerRadius)
└── StackPanel#RootLayout (垂直排列)
    ├── ContentPresenter#HeaderPresenter        ← 标题展示器（当 Header 非 null 时可见）
    └── Skeleton (IsLoading 控制骨架屏)
        └── StackPanel#ContentLayout (水平排列)
            ├── ContentPresenter#ValuePrefixAddOn   ← 前缀展示器（图标/文本）
            ├── ContentPresenter#ContentPresenter   ← 主内容（格式化后的数值或 StatisticCountUp）
            └── ContentPresenter#ValueSuffixAddOn   ← 后缀展示器（单位/文本）
```

### TimerStatistic 模板

TimerStatistic 使用了独立的模板（基于 `AbstractStatisticTheme`），结构与 Statistic 类似，区别在于 `ContentPresenter` 绑定的是 `RemainingTimeText` 而非 `Content`。

### StatisticCountUp 模板

```
TextBlock (Text = FormattedValue)
```

StatisticCountUp 的模板非常简洁，仅包含一个绑定 `FormattedValue` 的 `TextBlock`。动画效果完全通过 `AnimatingValue` 属性的 `DoubleTransition` 实现。
