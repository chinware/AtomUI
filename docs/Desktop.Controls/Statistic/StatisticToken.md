# Statistic Design Token

Statistic 控件家族使用 `StatisticToken`（Token ID: `"Statistic"`）作为组件级 Design Token。Token 较为精简，主要控制标题和内容的字体大小。所有颜色值均直接引用全局 `SharedToken`。

---

## Token 资源访问方式

在 AXAML 中使用组件级 Token：

```xml
{atom:StatisticTokenResource TitleFontSize}
{atom:StatisticTokenResource ContentFontSize}
```

在 AXAML 中使用全局共享 Token：

```xml
{atom:SharedTokenResource ColorTextDescription}
{atom:SharedTokenResource ColorTextHeading}
{atom:SharedTokenResource SpacingXXS}
```

---

## 组件级 Token 一览

以下是 `StatisticToken` 定义的全部组件级 Token。

### 字体 Token

| Token 名 | 类型 | 来源 | 说明 |
|---|---|---|---|
| `TitleFontSize` | `double` | `SharedToken.FontSize` | 标题（Header）字体大小，默认等于全局基础字号（14px） |
| `ContentFontSize` | `double` | `SharedToken.FontSizeHeading3` | 数值内容字体大小，默认等于 Heading3 字号（24px），使数值成为视觉焦点 |

---

## 主题中使用的全局 SharedToken

除组件级 Token 外，Statistic 的 ControlTheme 还直接引用了以下全局 `SharedToken`：

### 颜色 Token

| Token 资源键 | 使用场景 |
|---|---|
| `ColorTextDescription` | 标题（`HeaderPresenter`）的前景色，使用描述性文本色（较浅，突出数值） |
| `ColorTextHeading` | 数值内容（`ContentForeground`）的默认前景色，使用标题文本色（较深，作为视觉焦点） |

### 间距 Token

| Token 资源键 | 使用场景 |
|---|---|
| `SpacingXXS` | `RootLayout`（标题与内容之间）的默认间距 |
| `Spacing` | 加载态下（`IsLoading=True`）`RootLayout` 的间距（增大间距给骨架屏更多空间） |
| `SpacingXXS` | `ContentLayout`（前缀、数值、后缀之间）的间距 |

### 遮罩/背景 Token

| Token 资源键 | 使用场景 |
|---|---|
| `ColorBgMask` | 加载骨架屏（Skeleton）的遮罩背景色 |

### 动画 Token

| Token 资源键 | 使用场景 |
|---|---|
| `MotionDurationVerySlow` | StatisticCountUp 数值递增动画的时长 |

---

## Token 对外观的具体影响

### 字体层次结构

Statistic 通过字体大小的对比建立视觉层次：

| 区域 | Token | 默认值 | 视觉效果 |
|---|---|---|---|
| 标题 | `TitleFontSize` | 14px（`FontSize`） | 较小字号，描述性文字 |
| 数值 | `ContentFontSize` | 24px（`FontSizeHeading3`） | 较大字号，视觉焦点 |

### 颜色层次结构

| 区域 | SharedToken | 视觉效果 |
|---|---|---|
| 标题 | `ColorTextDescription` | 较浅的灰色调，提供语义上下文但不抢夺注意力 |
| 数值 | `ColorTextHeading` | 较深的标题色，作为视觉焦点突出显示 |

### 前缀/后缀中图标的尺寸

模板中前缀/后缀区域内的 `Icon` 控件，其 `Width` 和 `Height` 均绑定为 `StatisticTokenResource ContentFontSize`（即与数值字号一致），确保图标与数值文字视觉对齐。同时图标的 `FillBrush` 和 `StrokeBrush` 继承自 `ContentForeground`，确保颜色一致性。

### 各控件的 Token 继承关系

```
StatisticToken (组件级 Token 定义)
├── AbstractStatisticTheme.axaml (基类主题，引用 TitleFontSize / ContentFontSize / SharedToken 颜色)
│   ├── StatisticTheme.axaml (Statistic 主题，基于 AbstractStatisticTheme，增加间距)
│   └── TimerStatisticTheme.axaml (TimerStatistic 主题，基于 AbstractStatisticTheme，使用独立模板)
└── StatisticCountUpTheme.axaml (StatisticCountUp 主题，直接引用 ContentFontSize / ColorTextHeading)
```

所有三个控件（Statistic、TimerStatistic、StatisticCountUp）共享同一个 `StatisticToken` 作用域，通过 `this.RegisterTokenResourceScope(StatisticToken.ScopeProvider)` 注册。这意味着修改 `StatisticToken` 中的 `ContentFontSize` 会同时影响所有三个控件的数值显示大小。
