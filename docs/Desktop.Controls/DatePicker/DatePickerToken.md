# DatePicker Design Token

DatePicker 控件使用 `DatePickerToken`（Token ID: `"DatePicker"`）作为组件级 Design Token，所有视觉属性均从全局 `SharedToken` 派生，不硬编码任何具体值。DatePicker 和 RangeDatePicker 共享同一套 Token。

---

## Token 资源访问方式

在 AXAML 中使用组件级 Token：

```xml
{atom:DatePickerTokenResource CellHoverBg}
{atom:DatePickerTokenResource CellHeight}
{atom:DatePickerTokenResource PanelContentPadding}
```

在 AXAML 中使用全局共享 Token：

```xml
{atom:SharedTokenResource ColorPrimary}
{atom:SharedTokenResource ControlHeightSM}
{atom:SharedTokenResource BorderThickness}
```

---

## 组件级 Token 一览

以下是 `DatePickerToken` 定义的全部组件级 Token，按功能分组说明。

### 单元格颜色 Token

| Token 名 | 类型 | 来源 | 说明 |
|---|---|---|---|
| `CellHoverBg` | `Color` | `SharedToken.ControlItemBgHover` | 单元格悬浮态背景色 |
| `CellActiveWithRangeBg` | `Color` | `SharedToken.ControlItemBgActive` | 选取范围内的单元格背景色 |
| `CellHoverWithRangeBg` | `Color` | `SharedToken.ColorPrimary.Lighten(35)` | 选取范围内的单元格悬浮态背景色 |
| `CellBgDisabled` | `Color` | `SharedToken.ColorBgContainerDisabled` | 单元格禁用态背景色 |
| `CellRangeBorderColor` | `Color` | `SharedToken.ColorPrimary.Lighten(20)` | 选取范围时单元格边框色 |

### 尺寸 Token

| Token 名 | 类型 | 来源 | 说明 |
|---|---|---|---|
| `CellHeight` | `double` | `SharedToken.ControlHeightSM` | 单元格高度 |
| `CellWidth` | `double` | `SharedToken.ControlHeightSM` | 单元格宽度 |
| `TextHeight` | `double` | `SharedToken.ControlHeightLG` | 单元格文本高度 |
| `WithoutTimeCellHeight` | `double` | `SharedToken.ControlHeightLG * 1.65` | 十年/年/季/月/周单元格高度 |
| `ItemPanelMinWidth` | `double` | 固定 `225` | 日历项最小宽度 |
| `ItemPanelMinHeight` | `double` | 固定 `270` | 日历项最小高度 |
| `MonthViewMinWidth` | `double` | 固定 `260` | 月历最小宽度 |
| `DayTitleHeight` | `double` | `SharedToken.ControlHeightSM` | 星期标题行高度 |
| `RangeCalendarSpacing` | `double` | 固定 `20` | 范围日历面板间距 |

### 间距 Token

| Token 名 | 类型 | 来源 | 说明 |
|---|---|---|---|
| `CellMargin` | `Thickness` | `SharedToken.MarginXXS` | 单元格外边距 |
| `PanelContentPadding` | `Thickness` | `SharedToken.PaddingSM` | 面板内容内边距 |
| `HeaderMargin` | `Thickness` | `(0, 0, 0, SharedToken.UniformlyMarginSM)` | Header 头外间距 |
| `HeaderPadding` | `Thickness` | `(0, 0, 0, SharedToken.UniformlyPaddingSM)` | Header 头内间距 |
| `ButtonsPanelMargin` | `Thickness` | `(0, SharedToken.UniformlyMarginXS, 0, 0)` | 按钮区域面板外间距 |

---

## 主题中使用的全局 SharedToken

除组件级 Token 外，DatePicker 的 ControlTheme 和 Presenter 主题还直接引用了以下全局 `SharedToken`：

| Token 资源键 | 使用场景 |
|---|---|
| `BorderThickness` | Presenter 底部操作栏上方的分隔线厚度 |
| `ColorBorderSecondary` | Presenter 底部操作栏上方的分隔线颜色 |
| `ColorPrimary` | 主色调，用于选中日期的高亮背景、范围选择边框色派生 |
| `ControlHeightSM` | 单元格及星期行高度的派生基准 |
| `ControlHeightLG` | 文本高度和非时间单元格高度的派生基准 |
| `ControlItemBgHover` | 单元格悬浮背景色的派生来源 |
| `ControlItemBgActive` | 选中范围内单元格背景色的派生来源 |
| `ColorBgContainerDisabled` | 禁用态单元格背景色 |
| `MarginXXS` | 单元格外边距 |
| `PaddingSM` | 面板内容内边距 |
| `UniformlyMarginSM` | Header 外间距 |
| `UniformlyPaddingSM` | Header 内间距 |
| `UniformlyMarginXS` | 按钮面板外间距 |
| `EnableMotion` | 全局动画开关 |

---

## InfoPickerInput 共享 Token

DatePicker 和 RangeDatePicker 的输入框外观由 `InfoPickerInputToken`（Token ID: `"InfoPickerInput"`）控制，以下为其关键 Token：

| Token 名 | 类型 | 说明 |
|---|---|---|
| `RangePickerArrowMargin` | `Thickness` | 范围选择箭头外间距（基于 `SharedToken.UniformlyMarginXS`） |
| `RangePickerIndicatorThickness` | `double` | 选择指示器厚度（基于 `SharedToken.LineWidthFocus`） |

> 输入框的边框颜色、背景色、焦点色等视觉效果由 `InfoPickerInputTheme` 的基础样式通过全局 SharedToken 直接控制（如 `ColorBorder`、`ColorBgContainer`、`ColorPrimary` 等），无需额外的组件级 Token。

---

## Token 对外观的具体影响

### 日历面板单元格

| 状态 | 背景色 Token | 边框色 Token |
|---|---|---|
| **正常** | 透明 | — |
| **悬浮** | `CellHoverBg` | — |
| **选中** | `ColorPrimary`（SharedToken） | — |
| **范围内** | `CellActiveWithRangeBg` | — |
| **范围内悬浮** | `CellHoverWithRangeBg` | — |
| **范围边界** | — | `CellRangeBorderColor` |
| **禁用** | `CellBgDisabled` | — |

### 输入框外观与 SharedToken 映射

| 状态 | 边框色 | 背景色 |
|---|---|---|
| **正常（Outline）** | `ColorBorder` | `ColorBgContainer` |
| **悬浮** | `ColorPrimaryHover` | `ColorBgContainer` |
| **聚焦** | `ColorPrimary` | `ColorBgContainer` |
| **警告** | `ColorWarning` | `ColorBgContainer` |
| **错误** | `ColorError` | `ColorBgContainer` |
| **禁用** | `ColorBorder` | `ColorBgContainerDisabled` |

### 尺寸与 Token 映射

| 尺寸 | 输入框高度 | 字号 | 圆角 |
|---|---|---|---|
| `Large` | `ControlHeightLG` | `FontSizeLG` | `BorderRadiusLG` |
| `Middle` | `ControlHeight` | `FontSize` | `BorderRadius` |
| `Small` | `ControlHeightSM` | `FontSize` | `BorderRadiusSM` |

### 国际化文本

| 语言 | 「今天」按钮文本 | 「现在」按钮文本 |
|---|---|---|
| `en_US` | `"Today"` | `"Now"` |
| `zh_CN` | `"今天"` | `"现在"` |
