# Calendar Design Token

Calendar 控件使用 `CalendarToken`（Token ID: `"Calendar"`）作为组件级 Design Token，所有视觉属性均从全局 `SharedToken` 派生，不硬编码任何具体值。

---

## Token 资源访问方式

在 AXAML 中使用组件级 Token：

```xml
{atom:CalendarTokenResource CellHoverBg}
{atom:CalendarTokenResource CellWidth}
{atom:CalendarTokenResource PanelContentPadding}
```

在 AXAML 中使用全局共享 Token：

```xml
{atom:SharedTokenResource ColorPrimary}
{atom:SharedTokenResource ColorBgContainer}
{atom:SharedTokenResource BorderThickness}
```

---

## 组件级 Token 一览

以下是 `CalendarToken` 定义的全部组件级 Token，按功能分组说明。

### 颜色 Token

| Token 名 | 类型 | 来源 | 说明 |
|---|---|---|---|
| `CellHoverBg` | `Color` | `SharedToken.ControlItemBgHover` | 单元格（日期/月份/年份按钮）鼠标悬浮时的背景色 |
| `CellActiveWithRangeBg` | `Color` | `SharedToken.ControlItemBgActive` | 选取范围内的单元格背景色 |
| `CellHoverWithRangeBg` | `Color` | `SharedToken.ColorPrimary.Lighten(35)` | 选取范围内单元格悬浮态的背景色（主色调亮化 35%） |
| `CellBgDisabled` | `Color` | `SharedToken.ColorBgContainerDisabled` | 单元格禁用态背景色 |
| `CellRangeBorderColor` | `Color` | `SharedToken.ColorPrimary.Lighten(20)` | 选取范围时单元格的边框色（主色调亮化 20%） |

### 尺寸 Token

| Token 名 | 类型 | 来源 | 说明 |
|---|---|---|---|
| `CellWidth` | `double` | `SharedToken.ControlHeightSM` | 日期按钮的宽度 |
| `CellHeight` | `double` | `SharedToken.ControlHeightSM` | 日期按钮的高度（与宽度相同，正方形） |
| `CellLineHeight` | `double` | `CellHeight - 2` | 日期按钮内文本的行高 |
| `TextHeight` | `double` | `SharedToken.ControlHeightLG` | 单元格文本高度 |
| `WithoutTimeCellHeight` | `double` | `SharedToken.ControlHeightLG × 1.65` | 十年/年/月单元格高度（年/十年视图中的按钮高度） |
| `DayTitleHeight` | `double` | `SharedToken.ControlHeightSM` | 月视图中星期标题行的高度（如「日 一 二 …」） |
| `ItemPanelMinWidth` | `double` | 固定 `260` | 日历面板的最小宽度 |
| `ItemPanelMinHeight` | `double` | 固定 `290` | 日历面板的最小高度 |
| `RangeCalendarSpacing` | `double` | 固定 `20` | 范围日历（双面板）之间的间距 |

### 间距 Token

| Token 名 | 类型 | 来源 | 说明 |
|---|---|---|---|
| `CellMargin` | `Thickness` | `SharedToken.MarginXXS` | 日期/月份/年份按钮之间的外边距 |
| `PanelContentPadding` | `Thickness` | `SharedToken.PaddingSM` | 日历面板整体的内间距 |
| `HeaderMargin` | `Thickness` | `new Thickness(0, 0, 0, SharedToken.UniformlyMarginXS)` | 头部导航区域的外边距（底部留间距） |

---

## 主题中使用的全局 SharedToken

除组件级 Token 外，Calendar 的 ControlTheme 还直接引用了以下全局 `SharedToken`：

### Calendar 主框架

| Token 资源键 | 使用场景 |
|---|---|
| `ColorBorder` | 日历外边框颜色 |
| `BorderThickness` | 日历外边框厚度 |
| `BorderRadius` | 日历外边框圆角 |
| `ColorBgContainer` | 日历面板背景色 |
| `EnableMotion` | 全局动画开关，控制 `IsMotionEnabled` 默认值 |

### 日期按钮（BaseCalendarDayButton）

| Token 资源键 | 使用场景 |
|---|---|
| `ColorTextLabel` | 日期按钮默认文本颜色 |
| `ColorTextDisabled` | 非当前月份日期（溢出日期）的文本颜色 |
| `BorderRadiusSM` | 日期按钮的圆角半径 |
| `BorderThickness` | 日期按钮的边框厚度 |
| `FontSize` | 日期按钮的字体大小 |
| `ColorPrimary` | 今日日期的边框色；选中日期的背景色 |
| `ColorWhite` | 选中日期的文本颜色 |

### 月/年按钮（BaseCalendarButton）

| Token 资源键 | 使用场景 |
|---|---|
| `ColorTextLabel` | 月/年按钮默认文本颜色 |
| `ColorTextDisabled` | 非当前范围月/年的文本颜色 |
| `BorderRadiusSM` | 月/年按钮的圆角半径 |
| `ColorTransparent` | 月/年按钮的默认背景色和边框色（透明） |
| `FontSize` | 月/年按钮的字体大小 |
| `ColorPrimary` | 选中月/年的背景色 |
| `ColorWhite` | 选中月/年的文本颜色 |

### 头部标题按钮（HeadTextButton）

| Token 资源键 | 使用场景 |
|---|---|
| `FontSize` | 标题文字字号 |
| `ColorPrimary` | 悬浮时标题文字颜色 |

### 导航图标按钮（CalendarItem 内的 IconButton）

| Token 资源键 | 使用场景 |
|---|---|
| `IconSizeSM` | 导航图标的宽度和高度 |
| `ColorTextDescription` | 导航图标的默认颜色（浅灰描述色） |
| `ColorText` | 导航图标在悬浮/按下时的颜色 |

---

## Token 对外观的具体影响

### 日期按钮状态与 Token 映射

| 状态 | 背景色 | 文本色 | 边框色 |
|---|---|---|---|
| **正常** | 透明 | `ColorTextLabel` | 透明 |
| **悬浮** | `CellHoverBg` | `ColorTextLabel` | 透明 |
| **今日** | 透明 | `ColorTextLabel` | `ColorPrimary` |
| **选中** | `ColorPrimary` | `ColorWhite` | — (BorderThickness=0) |
| **非当前月（inactive）** | 透明 | `ColorTextDisabled` | 透明 |
| **禁用（blackout）** | — | — | — (不可交互) |

### 月/年按钮状态与 Token 映射

| 状态 | 背景色 | 文本色 |
|---|---|---|
| **正常** | `ColorTransparent` | `ColorTextLabel` |
| **悬浮** | `CellHoverBg` | `ColorTextLabel` |
| **选中** | `ColorPrimary` | `ColorWhite` |
| **非当前范围（inactive）** | `ColorTransparent` | `ColorTextDisabled` |

### 面板整体与 Token 映射

| 视觉元素 | Token |
|---|---|
| 日历外边框 | `ColorBorder` / `BorderThickness` / `BorderRadius` |
| 日历背景 | `ColorBgContainer` |
| 日历内间距 | `PanelContentPadding`（CalendarToken） |
| 日历最小尺寸 | `ItemPanelMinWidth` / `ItemPanelMinHeight`（CalendarToken） |
| 头部区域下边距 | `HeaderMargin`（CalendarToken） |
| 单元格间距 | `CellMargin`（CalendarToken） |
| 单元格尺寸 | `CellWidth` / `CellHeight`（CalendarToken） |
| 星期标题行高度 | `DayTitleHeight`（CalendarToken） |
