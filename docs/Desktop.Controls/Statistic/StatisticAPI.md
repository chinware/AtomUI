# Statistic API 参考

## 命名空间

```csharp
namespace AtomUI.Desktop.Controls;
```

AXAML 命名空间前缀：`atom`（`xmlns:atom="https://atomui.net"`）

---

## AbstractStatistic（抽象基类）

所有统计数值控件（`Statistic`、`TimerStatistic`）的公共基类，继承自 `HeaderedContentControl`。

### 公共属性（StyledProperty）

| 属性名 | 类型 | 默认值 | 说明 |
|---|---|---|---|
| `DecimalSeparator` | `string` | `"."` | 小数点分隔符 |
| `GroupSeparator` | `string` | `","` | 千分位分隔符 |
| `Precision` | `int` | `0` | 小数精度（保留几位小数，0 表示整数） |
| `IsLoading` | `bool` | `false` | 是否显示加载骨架屏（Skeleton 占位） |
| `ValuePrefixAddOn` | `object?` | `null` | 数值前缀装饰（可以是文本、图标等任意内容） |
| `ValuePrefixAddOnTemplate` | `IDataTemplate?` | `null` | 前缀内容的数据模板 |
| `ValueSuffixAddOn` | `object?` | `null` | 数值后缀装饰（可以是单位文本、图标等） |
| `ValueSuffixAddOnTemplate` | `IDataTemplate?` | `null` | 后缀内容的数据模板 |
| `ContentForeground` | `IBrush?` | 由 Token 控制（`ColorTextHeading`） | 数值区域前景色 |
| `ContentFontSize` | `double` | 由 Token 控制（`FontSizeHeading3`） | 数值区域字体大小 |

### 继承自 HeaderedContentControl 的常用属性

| 属性名 | 类型 | 默认值 | 说明 |
|---|---|---|---|
| `Header` | `object?` | `null` | 标题内容（统计项描述文本） |
| `HeaderTemplate` | `IDataTemplate?` | `null` | 标题数据模板 |
| `Content` | `object?` | `null` | 主内容区域（格式化后的数值文本或嵌入的 StatisticCountUp） |
| `ContentTemplate` | `IDataTemplate?` | `null` | 内容数据模板 |
| `Background` | `IBrush?` | `null` | 背景色 |
| `CornerRadius` | `CornerRadius` | `0` | 圆角半径 |

---

## Statistic（基础统计数值）

继承自 `AbstractStatistic`。

### 公共属性

| 属性名 | 类型 | 默认值 | 说明 |
|---|---|---|---|
| `Value` | `object?` | `null` | 原始数值（支持 `int`、`long`、`float`、`double`、`decimal` 等数值类型，也可以是字符串） |

### CLR 属性

| 属性名 | 类型 | 说明 |
|---|---|---|
| `Formatter` | `Func<Statistic, object?, string?>?` | 自定义格式化回调。设置后将优先使用此回调格式化 `Value`，忽略 `Precision` / `GroupSeparator` 等参数 |

### 内部属性（Internal）

| 属性名 | 类型 | 说明 |
|---|---|---|
| `EffectiveValue` | `string?` | 经过格式化后的最终显示值，由 `GenerateEffectiveValue` 计算 |

### 格式化优先级

1. 如果设置了 `Content`（如嵌入 `StatisticCountUp`）→ 直接显示 `Content`
2. 如果设置了 `Formatter` → 调用 `Formatter(this, Value)` 获取显示文本
3. 否则 → 使用 `StatisticUtils.FormatNumber(Value, GroupSeparator, DecimalSeparator, Precision)` 自动格式化

---

## TimerStatistic（计时器/倒计时）

继承自 `AbstractStatistic`。

### 公共属性（StyledProperty）

| 属性名 | 类型 | 默认值 | 说明 |
|---|---|---|---|
| `Value` | `DateTime` | `default` | 目标时间。如果在未来则为倒计时模式，如果在过去则为计时模式 |
| `Format` | `string?` | `null` | 时间显示格式（.NET `TimeSpan` 格式字符串），默认为 `hh:mm:ss` |
| `RefreshDuration` | `TimeSpan` | `10ms` | 计时器刷新间隔。减小此值可提高显示精度（如毫秒级显示需设为较小值） |

### CLR 属性

| 属性名 | 类型 | 说明 |
|---|---|---|
| `Formatter` | `Func<TimeSpan, string>?` | 自定义时间格式化回调。设置后完全自定义剩余时间的文本表示 |

### 事件

| 事件名 | 类型 | 说明 |
|---|---|---|
| `CountdownFinished` | `EventHandler?` | 倒计时完成时触发（剩余时间 ≤ 0），同时自动停止计时器 |

### 内部属性（Internal）

| 属性名 | 类型 | 说明 |
|---|---|---|
| `RemainingTime` | `TimeSpan` | 当前剩余/经过时间 |
| `RemainingTimeText` | `string?` | 格式化后的剩余时间文本 |

### 计时行为说明

- **倒计时**：当 `Value > DateTime.Now` 时，`RemainingTime = Value - DateTime.Now`
- **计时**：当 `Value ≤ DateTime.Now` 时，`RemainingTime = DateTime.Now - Value`
- **自动停止**：当 `RemainingTime ≤ TimeSpan.Zero` 时，停止计时器并触发 `CountdownFinished`
- **生命周期管理**：附加到视觉树时自动启动计时器，分离时自动停止

### 常用 Format 格式

| 格式字符串 | 输出示例 | 说明 |
|---|---|---|
| `hh\:mm\:ss` (默认) | `02:30:45` | 时:分:秒 |
| `hh\:mm\:ss\.fff` | `02:30:45.123` | 时:分:秒.毫秒 |
| `d\ \天\ h\ \时\ m\ \分\ s\ \秒` | `2 天 6 时 30 分 45 秒` | 天级中文格式 |

---

## StatisticCountUp（数值递增动画）

继承自 `TemplatedControl`。设计为嵌入 `Statistic.Content` 中使用。

### 公共属性（StyledProperty）

| 属性名 | 类型 | 默认值 | 说明 |
|---|---|---|---|
| `EndValue` | `double` | `0` | 目标数值，动画将从 0 递增到此值 |
| `DecimalSeparator` | `string` | `"."` | 小数点分隔符 |
| `GroupSeparator` | `string` | `","` | 千分位分隔符 |
| `Precision` | `int` | `0` | 小数精度 |
| `AnimatingValue` | `double` | `0` | 当前动画值（由 `DoubleTransition` 驱动从 0 到 `EndValue`） |

### 内部属性（Internal）

| 属性名 | 类型 | 说明 |
|---|---|---|
| `FormattedValue` | `string?` | 当前动画值经过格式化后的显示文本 |

### 动画说明

- 动画使用 `DoubleTransition` 过渡 `AnimatingValue` 属性
- 缓动曲线：`ExponentialEaseOut`（快速启动，缓慢到达终值）
- 动画时长：`SharedToken.MotionDurationVerySlow`
- 控件加载（`OnLoaded`）时自动将 `AnimatingValue` 设为 `EndValue` 触发动画
- 卸载（`OnUnloaded`）时清除 `Transitions`

---

## 伪类（Pseudo-Classes）

Statistic 家族控件的伪类较少，主要继承自 Avalonia 基础控件。

### 通用伪类

| 伪类 | 触发条件 |
|---|---|
| `:disabled` | `IsEnabled == false` |

### 属性选择器

| 选择器 | 说明 |
|---|---|
| `atom\|Statistic[IsLoading=True]` | 加载中状态 |
| `atom\|TimerStatistic[IsLoading=True]` | TimerStatistic 加载中状态 |
