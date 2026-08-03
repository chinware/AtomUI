# Calendar Token 设计

本文档定义 Calendar 家族 Token 的专属语义、分类与兼容边界。控件 Token 的通用规则见 [AtomUI 控件 Token 设计规范](../../../../engineering/control-token-guidelines.md)。Calendar 整体架构见 [Calendar 桌面版架构设计](overview.md)，实现原理见 [Calendar 桌面版实现原理](implementation.md)，行为规则见 [Calendar 行为设计](behavior-design.md)，农历能力见 [LunarCalendar 农历能力设计](lunar-calendar-design.md)，范围条见 [Calendar 范围条设计](range-bar-design.md)，变更记录见 [Calendar Changelog](changelog.md)。

## 1. 定位

Calendar 的 Token 收敛为九个组件视觉语义。日期值、周标题、范围条间距、范围条圆角、Padding、Border、Typography 与 Motion 均从 SharedToken 派生；Fullscreen 单元最小高度通过 `FullCellMinHeight` 固化 Calendar 完整单元的测量规则，Year 模式月份单元宽度通过 `YearMonthCellWidth` 固化面板单元测量规则，范围条默认高度通过 `RangeBarHeight` 固化 Calendar overlay 的默认条高。

当前 Calendar Token 源：

- `CalendarToken`，源码位于 `src/AtomUI.Desktop.Controls/Calendar/CalendarToken.cs`。
- AXAML 通过生成的 `CalendarTokenResource` 访问这些值。

LunarCalendar 使用独立 exact Control identity 和 `LunarCalendarToken`。它只补充农历双行内容、卡片内容高度、周末/节假日状态以及 Fullscreen RangeBars 避让所需语义，不复制 Calendar 的根背景、Header、普通 Cell 选中态或 RangeBars 默认条高。

## 2. Token 清单

| Token | 类型 | 默认语义 |
| --- | --- | --- |
| `FullBg` | Color | 完整 Calendar 背景，派生自容器背景（`ColorBgContainer`） |
| `FullPanelBg` | Color | 完整 Calendar Panel 背景，派生自容器背景（`ColorBgContainer`） |
| `ItemActiveBg` | Color | 完整模式选中日期/月单元背景，派生自 active item 背景（`ControlItemBgActive`） |
| `YearControlWidth` | double | Year Select 最小宽度，默认 80 |
| `MonthControlWidth` | double | Month Select 最小宽度，默认 70 |
| `YearMonthCellWidth` | double | Year 模式月份单元内容宽度，按 `ControlHeightLG * 1.5` 派生 |
| `MiniContentHeight` | double | Mini 内容高度，默认 256；约束包含 WeekHeader 与六行 CellHost 的完整 CalendarView，不包含 Header、body 分隔线或 body Padding |
| `FullCellMinHeight` | double | Fullscreen 日期/月单元最小高度，按 `ControlHeightSM + ((FontHeightSM + MarginXS) * 3 + LineWidth * 2) + PaddingXS / 2 + LineWidthBold` 派生 |
| `RangeBarHeight` | double | Fullscreen 日期范围条默认高度，默认 `ControlHeightSM - MarginXXS` |

## 3. LunarCalendar Token 清单

| Token | 类型 | 默认语义 |
| --- | --- | --- |
| `MiniContentHeight` | double | 卡片内容高度，按 `FontHeightSM + (MiniDateCellSize + MarginXS) * 6` 派生；包含 WeekHeader 与六行 CellHost，并为相邻双行 Cell 保留稳定纵向间隔。 |
| `MiniDateCellSize` | double | 卡片 Month 模式双行日期 Cell 的宽高，默认从 `ControlHeightLG` 派生。 |
| `MiniMonthCellWidth` | double | 卡片 Year 模式双行月份 Cell 内容宽度，保证公历月和农历月范围可读。 |
| `SecondaryTextFontSize` | double | 农历日期、节气和节日次级文本字号，默认从 `FontSizeSM` 派生。 |
| `SecondaryTextLineHeight` | double | 次级文本稳定行高，参与 Cell 测量和 RangeBars 避让。 |
| `SecondaryTextColor` | Color | 普通农历日期次级文本颜色，默认从 `ColorTextTertiary` 派生。 |
| `WeekendTextColor` | Color | 普通周末的公历日期文本色，默认从 `ColorError` 派生。 |
| `HolidayMarkerColor` | Color | Provider `Holiday` 标记色，默认从状态/错误色语义派生。 |
| `WorkdayMarkerColor` | Color | Provider `Workday` 标记色，默认从次级文本色语义派生。 |
| `FullCellMinHeight` | double | LunarCalendar Fullscreen Cell 最小高度，包含公历值、农历次级行和 RangeBars 可用区。 |
| `RangeBarTopOffset` | double | LunarCalendar Fullscreen Month 中范围条相对日期 Cell 顶部的有效偏移，必须位于农历次级行之后。 |

这些 Token 只决定布局和视觉，不决定节日优先级、Provider 结果、周末判定、节气类型或农历算法。Holiday/Workday 的显示文本来自 Provider 和语言格式化，不进入 Token。

## 4. Token 消费

- C# 控件负责状态归一与伪类同步。
- AXAML/ControlTheme 通过 `{atom:CalendarTokenResource Xxx}` 和 `{atom:LunarCalendarTokenResource Xxx}` 消费 Token。
- Token 默认值从 SharedToken 派生，不读取控件实例状态。
- Fullscreen 与 Mini 共用同一 CalendarView 与 Cell Model，只由 selector 和布局资源改变视觉。
- RangeBars 只新增默认高度 Token：`RangeBarHeight`。单条业务颜色和高度分别由 `CalendarRangeBar.Background` 与 `CalendarRangeBar.Height` 控制；条间距、圆角和 label padding 作为内部 metrics 从 SharedToken 派生。
- LunarCalendar 的专用 Cell 通过自身 ControlTheme 消费农历 Token；LunarCalendar root 不通过深层 selector 修改 CalendarHeader、CalendarView、ComboBox、OptionButtonGroup 或普通 CalendarViewCell 的模板内部。
- Calendar 家族 presentation adapter 选择有效的 `MiniContentHeight` 资源；普通 Calendar 继续使用 256，LunarCalendar 使用双行 Cell 派生高度，并通过 Calendar 根模板强属性传给 `CalendarView`。
- Fullscreen Month 中，普通 Calendar 使用 `CalendarToken.FullCellMinHeight` 和现有范围条顶部偏移；LunarCalendar 使用自己的 `FullCellMinHeight` 与 `RangeBarTopOffset` 为农历次级行预留空间。

## 5. 伪类

根伪类：`:fullscreen`、`:mini`、`:month`、`:year`、`:show-week`。

Cell 伪类：`:date`、`:month`、`:week`、`:today`、`:selected`、`:outside`、`:disabled`、`:focused`。

LunarCalendar 的 Holiday、Workday 和 weekend 是专用 Cell 的呈现状态。若使用伪类表达，伪类只由 `LunarCalendarViewCell` 自己声明和消费，不能要求 Calendar root 通过模板穿透 selector 设置内部状态。

## 6. 兼容性要求

- 不把实例状态或交互状态写成 Token。
- 不为了单个范围条样式新增控件 Token；范围条公共样式使用 `RangeBarHeight`、SharedToken 内部 metrics 和实例属性。
- LunarCalendarToken 不复制 CalendarToken；Calendar 结构语义和农历增量语义分别由各自 token owner 管理。
- LunarCalendar 卡片纵向间隔由 `MiniContentHeight` 与网格行分配共同形成，不能给选中 Cell、某一周或某个 Gallery 示例单独增加 Margin。
- `MiniDateCellSize` 和 `MiniMonthCellWidth` 不能改变 Month 6×7、Year 3×4 的拓扑。
- `RangeBarTopOffset` 必须与 `SecondaryTextLineHeight` 和 Cell Padding 一致，不能通过 Cell 外 margin 制造行间间距。
- 如需引入新 Token，必须同步 Token 类型、生成资源、主题引用和本文档。

## 7. 验证策略

| 改动类型 | 验证要求 |
| --- | --- |
| Token 文档 | `git diff --check`，检查相对链接存在。 |
| Token 默认值 | 运行 Calendar 控件测试，走查 Light/Dark 主题。 |
| Token 名称或数量 | 检查 generated `CalendarTokenResource` / `LunarCalendarTokenResource` key、AXAML 引用和 token.md。 |
| 主题映射 | 走查 today、selected、outside、disabled、focused、weekend、Holiday、Workday 状态视觉，并确认 CellTemplate/FullCellTemplate 不改变容器状态。 |
| 布局 metrics | 验证卡片双行 Cell 不溢出、相邻周行间距至少为 `MarginXS`，普通 Calendar Mini 高度仍为 256，Fullscreen 农历次级行不与 RangeBars 重叠，ShowWeek 不改变日期列 metrics。 |
