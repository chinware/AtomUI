# Calendar 桌面版架构设计

本文档定义 `Calendar` 桌面版的设计定位、公共契约、状态模型与主题关系。内部实现原理见 [Calendar 桌面版实现原理](implementation.md)，Token 专项设计见 [Calendar Token 设计](token.md)，变更记录见 [Calendar Changelog](changelog.md)。完整设计规格见 `docs/superpowers/specs/2026-07-30-ant-design-calendar-view-design.md`。

## 1. 控件定位

| 项 | 值 |
| --- | --- |
| NuGet 包 | `AtomUI.Desktop.Controls` |
| .NET 命名空间 | `AtomUI.Desktop.Controls` |
| AXAML 命名空间 | `https://atomui.net` |
| Gallery 页面 | `controlgallery/AtomUIGallery/ShowCases/DataDisplay/Calendar` |
| 控件状态 | Stable |

`Calendar` 是按日期组织业务展示内容的桌面日历控件，遵循 Ant Design 6 Calendar 语义。它提供月日期面板、年月份面板、默认年月头部、有效范围、禁用日期、单元格定制与选择事件。

Calendar **不**承担日期输入弹层、范围选择、多日期选择或日程排布职责——这些由 DatePicker 或业务层承担。Calendar 也不复用旧 WPF/Avalonia Calendar 移植体系，其内部 `CalendarView` 面板独立实现，与 DatePicker 的 CalendarView 子系统无耦合。

## 2. 公共 API

```csharp
public class Calendar : TemplatedControl
{
    public DateTime Value { get; set; }                 // 选中日期与面板锚点，默认 DateTime.Today
    public CalendarMode Mode { get; set; }              // Month | Year，默认 Month
    public bool Fullscreen { get; set; }                // 默认 true
    public bool ShowWeek { get; set; }                  // 默认 false
    public CalendarDateRange? ValidRange { get; set; }  // 有效范围（首尾包含）
    public Func<DateTime, bool>? DisabledDate { get; set; }
    public IDataTemplate? CellTemplate { get; set; }        // 对应 Ant Design cellRender
    public IDataTemplate? FullCellTemplate { get; set; }    // 对应 fullCellRender，优先于 CellTemplate
    public IDataTemplate? HeaderTemplate { get; set; }      // 自定义头部

    public event EventHandler<CalendarValueChangedEventArgs>? ValueChanged;
    public event EventHandler<CalendarSelectedEventArgs>? Selected;
    public event EventHandler<CalendarPanelChangedEventArgs>? PanelChanged;
}
```

- `Value` 规范化到 `.Date`；Calendar 不提供时间编辑。
- `CalendarMode.Month` 使用 6×7 日期网格，`CalendarMode.Year` 使用 3×4 月份网格。
- 三个事件表达用户交互回调。程序直接设置 `Value`/`Mode` 只更新属性和渲染，不触发这三个事件。

### 值类型

- `CalendarDateRange(DateTime start, DateTime end)`：首尾包含，`Start`/`End` 规范化到 `.Date`，`end < start` 抛 `ArgumentOutOfRangeException`。仅服务 `ValidRange`。
- `CalendarCellContext`：Cell 模板上下文（`Value`/`Today`/`CellType`/`DisplayValue`/`IsToday`/`IsInView`/`IsSelected`/`IsDisabled`）。
- `CalendarHeaderContext`：自定义 Header 上下文（`Value`/`Mode` + `ChangeValueCommand`/`ChangeModeCommand`）。

## 3. 事件顺序

一次有效用户选择按固定顺序触发：`PanelChanged -> ValueChanged -> Selected`，不存在的事件从序列中省略。触发时 `Value` 与渲染输入已提交新状态。

| 操作 | PanelChanged | ValueChanged | Selected |
| --- | --- | --- | --- |
| 选择当前日期 | 否 | 否 | Date |
| 选择同月其他日期 | 否 | 是 | Date |
| 选择相邻月份补位日期 | 是 | 是 | Date |
| 默认 Header 改月份 | 跨月时是 | 日期变化时是 | Month |
| 默认 Header 改年份 | 跨面板时是 | 日期变化时是 | Year |
| Year 模式选月份 | 跨年时是 | 日期变化时是 | Month |
| 自定义 Header 改日期 | 按面板边界 | 日期变化时是 | Customize |

用户切换 Mode 触发一次 `PanelChanged(Value, newMode)`，不触发 ValueChanged/Selected。

## 4. 组合结构

```text
Calendar
├── CalendarHeader 或 HeaderTemplate
└── CalendarView（内部，AtomUI.Desktop.Controls.Internal.Calendar）
    ├── WeekHeader
    └── CellHost
        └── CalendarViewCell × N
```

Calendar 是唯一业务状态 owner；CalendarView 是纯面板，只报告用户意图；CalendarViewCell 是有界数据容器。单向数据流：Calendar 向 Header 和 CalendarView 投影不可变状态。

## 5. Gallery 示例

Gallery 覆盖：基础 Fullscreen、Mini、Year 模式、ShowWeek、ValidRange+DisabledDate、CellTemplate、FullCellTemplate、自定义 HeaderTemplate、事件来源展示。

## 6. 兼容边界

本设计不兼容旧 Calendar 的 Public API、Template Part、ControlTheme key 或伪类。旧 `SelectedDate`/`SelectedDates`/`SelectionMode`/`DisplayDate*`/`BlackoutDates`/Decade 面板等语义不提供迁移映射。
