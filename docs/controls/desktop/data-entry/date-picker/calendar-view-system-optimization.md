# CalendarView 系统性优化设计

本文定义 `DatePicker` 内部 `CalendarView` 的系统性优化目标架构、状态模型、生命周期规则和验证边界。它服务于 `src/AtomUI.Desktop.Controls/DatePicker/CalendarView` 的根因重构，不替代 [DatePicker 桌面版架构设计](overview.md)、[DatePicker 桌面版实现原理](implementation.md) 或 [DatePicker Token 设计](token.md)。具体研发任务见 [CalendarView 系统性优化开发计划](../../../../superpowers/plans/2026-07-01-calendar-view-system-optimization.md)。

## 1. 设计目标

CalendarView 是 DatePicker 弹层内的日历面板实现，负责单日期、范围日期和双月范围日期选择。优化目标是保持 DatePicker、RangeDatePicker 和双月 RangeDatePicker 的用户可见功能、交互顺序和主题表现不变，同时重做 CalendarView 内部状态、渲染和生命周期结构。

本次设计不把内部兼容性作为约束。以下内部契约允许重做：

- `CalendarView` 命名空间下的 internal 类型、internal 属性、internal 方法和内部事件。
- Calendar、RangeCalendar、DualMonthRangeCalendar 与 CalendarItem 之间的协作方式。
- CalendarDayButton 和 CalendarButton 的内部状态承载方式。
- 动态生成按钮的 owner、事件、binding 和清理流程。
- CalendarView 内部 template part 的接入方式，前提是主题文件同步更新并保持 public DatePicker 视觉语义。

以下用户可见能力必须保持不变：

- 单日期选择、范围日期选择、双月范围选择。
- 月、年、十年视图切换和导航。
- today、blackout、inactive、selected、range start/end/middle、focus、hover 的视觉语义。
- `DisplayDate`、`DisplayDateStart`、`DisplayDateEnd`、`SelectedDate`、`SecondarySelectedDate`、`FirstDayOfWeek`、`IsTodayHighlighted` 等外部可观察行为。
- DatePicker、RangeDatePicker 与 TimeView、确认按钮、Today/Now 按钮之间的协作行为。

## 2. 当前根因

CalendarView 当前维护成本高的根因不是单个文件过长，而是状态所有权和渲染职责交叉。

| 问题 | 当前表现 | 优化方向 |
| --- | --- | --- |
| 状态 owner 分裂 | `Calendar`、`CalendarItem`、`CalendarDayButton`、`RangeCalendar` 都会写 selected/range/focus 状态。 | 建立单一状态 owner，所有视觉状态从状态快照推导。 |
| 属性回调互相修改 | `DisplayDate`、`DisplayDateStart`、`DisplayDateEnd`、`SelectedDate` 互相 `SetCurrentValue`，并依赖 `_displayDateIsChanging` 压制递归。 | 引入一次性状态归一化流程，删除回调抑制 flag。 |
| Range 算法重复 | `RangeCalendarItem.CheckButtonSelectedState`、`RangeCalendar.UpdateHighlightDays`、`DualMonthRangeCalendar.UpdateHighlightDays` 重复计算范围态。 | 抽出统一 range visual state 计算。 |
| 动态容器生命周期分散 | CalendarItem 生成按钮、挂事件、设置 owner、绑定 motion，并在多个入口清理。 | 建立 generated container manager，统一 acquire/release。 |
| 文化信息读取时机不稳定 | 静态属性默认值和按钮构造器读取 `ThemeManager.Current`。 | 运行态维护 culture context，语言变化时刷新模型。 |
| Pointer 判断依赖模板细节 | `Children[7]` 和 null-forgiving 判断真实 month view 区域。 | 使用 realized panel bounds，避免硬编码 children index。 |

## 3. 目标架构

CalendarView 优化后采用单向数据流：

```text
Avalonia property / presenter event / pointer / keyboard
  -> CalendarViewStateController
  -> CalendarViewState normalized snapshot
  -> CalendarPanelBuilder
  -> CalendarPanelModel / CalendarCellState
  -> CalendarItemRenderer
  -> CalendarDayButton / CalendarButton pseudo-class
```

核心原则：

- public 或 styled property 只作为输入入口，不直接驱动多个 template part。
- 所有日期单元视觉状态由 `CalendarCellState` 表达。
- CalendarItem 只负责把模型应用到已生成按钮，不再重复计算业务状态。
- RangeCalendar 和 DualMonthRangeCalendar 只表达模式差异，不拥有另一套 range 算法。
- 模板、全局订阅、动态容器和语言资源都有成对释放路径。

## 4. 核心类型职责

### 4.1 CalendarViewState

`CalendarViewState` 是 CalendarView 的运行态快照，保存已经归一化的日期选择状态。

建议字段：

| 字段 | 职责 |
| --- | --- |
| `DisplayMode` | 当前显示层级：Month、Year、Decade。 |
| `DisplayDate` | 当前主面板显示月份，归一到月份首日。 |
| `SecondaryDisplayDate` | 双月面板的次面板显示月份，由 `DisplayDate` 推导。 |
| `DisplayDateStart` / `DisplayDateEnd` | 可显示和可选择边界。 |
| `SelectedDate` | 单选日期或范围开始日期。 |
| `SecondarySelectedDate` | 范围结束日期。 |
| `HoverDate` | pointer hover 产生的临时范围端点。 |
| `ActiveRangePart` | 当前正在选择 range start 或 range end。 |
| `FocusedDate` | 键盘和焦点视觉使用的日期。 |
| `SelectedMonth` / `SelectedYear` | 年视图、十年视图当前项。 |
| `BlackoutDates` | 不可选日期范围集合。 |
| `FirstDayOfWeek` | 周起始日，来自显式属性或 culture context。 |
| `IsTodayHighlighted` | 是否显示 today 视觉状态。 |
| `Culture` | 日历文本格式化使用的 `DateTimeFormatInfo`。 |

状态对象只描述数据，不持有 Visual、Button、Grid、TemplatePart、Dispatcher 或订阅。

### 4.2 CalendarViewStateController

`CalendarViewStateController` 是唯一的状态归一化入口。所有 property changed、pointer、keyboard 和 presenter 事件都转换为明确 action，再由 controller 生成新的状态快照。

建议 action：

| Action | 来源 |
| --- | --- |
| `SetDisplayDate(DateTime)` | 外部设置、导航按钮、键盘导航。 |
| `SetDisplayRange(DateTime? start, DateTime? end)` | `DisplayDateStart/End` 变化。 |
| `SelectDate(DateTime)` | 单日期点击、Today/Now 按钮。 |
| `SelectRangeEndpoint(DateTime)` | Range start/end 点击。 |
| `SetHoverDate(DateTime?)` | pointer enter/out month panel。 |
| `SetDisplayMode(CalendarMode)` | Header、年/月/十年切换。 |
| `MoveFocus(CalendarNavigationDirection)` | keyboard。 |
| `SetCulture(DateTimeFormatInfo)` | language variant 变化。 |
| `SetBlackoutDates(CalendarBlackoutDatesCollection)` | blackout collection mutation。 |

归一化规则：

- `DisplayDate` 始终 clamp 到 `DisplayDateStart/End` 范围内。
- `DisplayDateStart > DisplayDateEnd` 时以 start 为准，将 end 归一到 start。
- `SelectedDate` 或 `SecondarySelectedDate` 超出边界时由 controller 按当前行为归一，不在 property validator 中反向写边界。
- blackout 日期不能成为有效选中值。
- `SecondaryDisplayDate` 由 `DisplayDate + 1 month` 推导，不能由外部独立写入。
- Range 视觉范围由 `SelectedDate`、`SecondarySelectedDate`、`HoverDate` 和 `ActiveRangePart` 统一计算，是否交换起止点由 range policy 控制。
- 状态更新完成后只触发一次 panel model rebuild 和 render。

### 4.3 CalendarPanelBuilder

`CalendarPanelBuilder` 输入 `CalendarViewState`，输出与视觉无关的 panel model。

| Model | 内容 |
| --- | --- |
| `CalendarMonthPanelModel` | 一个 7x7 月面板，包含星期标题和 42 个日期单元。 |
| `CalendarYearPanelModel` | 一个 3x4 月份面板，包含月份项状态。 |
| `CalendarDecadePanelModel` | 一个 3x4 年份面板，包含年份项状态。 |
| 当前 display mode 对应的 panel model | 单月、双月、年或十年视图分别按需构建具体 model；不保留无状态聚合层。 |

Month cell 使用 `CalendarCellState`：

| 字段 | 含义 |
| --- | --- |
| `Date` | 单元格对应日期。 |
| `Text` | 当前 culture 下的展示文本。 |
| `IsToday` | 是否今天。 |
| `IsBlackout` | 是否不可选择。 |
| `IsDisabled` | 是否超出边界或控件不可用。 |
| `IsInactive` | 是否不属于当前显示月份。 |
| `IsSelected` | 是否处于选中视觉状态。 |
| `IsRangeStart` | 是否 range start。 |
| `IsRangeEnd` | 是否 range end。 |
| `IsRangeMiddle` | 是否 range middle。 |
| `IsFocused` | 是否 keyboard/focus 当前项。 |
| `IsHidden` | 是否为了边界占位但不可见。 |

`CalendarPanelBuilder` 不读取 VisualTree，不创建控件，不触发 Avalonia property changed。

### 4.4 CalendarItemRenderer

`CalendarItemRenderer` 负责把 panel model 应用到 CalendarItem 已生成的按钮。

职责：

- 生成和复用 month/year/decade 按钮。
- 将 `CalendarCellState` 映射为 `CalendarDayButton` 的 content、data context、enabled、opacity、pseudo-class。
- 将 year/month item state 映射为 `CalendarButton` 的 content、enabled、selected、focused、inactive。
- 维护 focus visual 指向，但不决定 focused date。
- 不直接修改 `SelectedDate`、`DisplayDate`、`SecondarySelectedDate` 等业务状态。

## 5. Range 与双月模型

Range 模型不再由 subclass 分别计算视觉。统一使用 `CalendarRangeSelectionState`：

| 字段 | 含义 |
| --- | --- |
| `Start` | 当前 range start 候选值。 |
| `End` | 当前 range end 候选值。 |
| `ActivePart` | 正在选择 start 或 end。 |
| `HoverDate` | 临时 hover 日期。 |
| `RepairReverseRange` | start > end 时是否交换输出。 |

计算规则：

```text
ActivePart = Start:
  visualStart = HoverDate ?? SelectedDate
  visualEnd   = SecondarySelectedDate

ActivePart = End:
  visualStart = SelectedDate
  visualEnd   = HoverDate ?? SecondarySelectedDate

visualStart > visualEnd:
  range visual 始终按小到大渲染
  confirmed value 是否交换由 RepairReverseRange 决定
```

单月 RangeCalendar 和 DualMonthRangeCalendar 的差异只在 panel 数量：

- RangeCalendar 渲染主 `CalendarMonthPanelModel`。
- DualMonthRangeCalendar 渲染主 panel 和 secondary panel。
- 两个 panel 共享同一个 `CalendarRangeSelectionState`。
- `UpdateHighlightDays` 这类按 Grid 重扫按钮的逻辑被 renderer 替代。

## 6. Template 与生命周期设计

### 6.1 Template part 接入

CalendarView 模板接入必须遵循：

```text
OnApplyTemplate
  -> ReleaseTemplateParts()
  -> base.OnApplyTemplate(e)
  -> ResolveTemplateParts(e)
  -> AttachTemplatePartHandlers()
  -> EnsureGeneratedContainers()
  -> RenderCurrentState()
```

`ReleaseTemplateParts` 释放旧 part 的事件、owner、motion binding、generated container、focus 引用和 pointer/culture 订阅。它可以拆成私有方法，也可以内联，但 acquire/release 对必须在代码结构上相邻可读。

### 6.2 Generated container 管理

动态生成容器由 dedicated manager 维护：

| 操作 | 规则 |
| --- | --- |
| 创建 | 根据 panel 类型生成固定数量按钮，设置 Grid row/column。 |
| 绑定 | 只绑定必要的 motion property 和事件。 |
| 渲染 | 使用 model 更新按钮状态。 |
| 清理 | 移除事件、清除 binding、清除 owner、清除 focus 引用、从 panel children 移除。 |
| 重套模板 | 先清理旧 panel，再接入新 panel。 |
| detach | 释放全局订阅和 Visual 引用。 |

### 6.3 Pointer tracking

Pointer hover 只产生 `SetHoverDate` action。CalendarItem 不直接写 range highlight。

如果仍需全局 pointer out month panel 检测，应使用明确 tracker：

- tracker attach 时订阅 `IInputManager.Process`。
- tracker detach 和 template release 时释放订阅。
- month bounds 来自已实现 panel 的 bounds，不读取 `Children[7]`。
- bounds 不可用时输出 pointer out，不抛异常。

### 6.4 Culture context

CalendarView 使用运行态 `CalendarCultureContext`：

- 静态属性默认值不读取 `ThemeManager.Current`。
- attach 后读取当前 language variant。
- language variant 变化时更新 culture context，并触发 panel rebuild。
- 按钮构造器不读取 culture；按钮文本由 renderer 写入。

## 7. 文件组织

CalendarView 目录按稳定职责组织。公共 DatePicker API 仍保留在 DatePicker/RangeDatePicker/Presenter 主文件中。

建议结构：

```text
src/AtomUI.Desktop.Controls/DatePicker/CalendarView/
├── Calendar.cs
├── RangeCalendar.cs
├── DualMonthRangeCalendar.cs
├── CalendarItem.cs
├── DualMonthCalendarItem.cs
├── CalendarButton.cs
├── CalendarDayButton.cs
├── CalendarBlackoutDatesCollection.cs
├── State/
│   ├── CalendarViewState.cs
│   ├── CalendarViewStateController.cs
│   ├── CalendarViewAction.cs
│   └── CalendarRangeSelectionState.cs
├── Models/
│   ├── CalendarCellState.cs
│   ├── CalendarMonthPanelModel.cs
│   ├── CalendarYearPanelModel.cs
│   └── CalendarDecadePanelModel.cs
├── Rendering/
│   ├── CalendarPanelBuilder.cs
│   ├── CalendarItemRenderer.cs
│   └── CalendarGeneratedContainerManager.cs
└── Infrastructure/
    ├── CalendarCultureContext.cs
    └── CalendarPointerTracker.cs
```

拆分边界：

- `State` 不引用 Avalonia Visual 类型。
- `Models` 不引用控件类型。
- `Rendering` 可以引用 CalendarItem、CalendarDayButton、CalendarButton 和 Avalonia panel。
- `Infrastructure` 只封装运行时依赖，不保存业务状态。
- Calendar、RangeCalendar、DualMonthRangeCalendar 保留 Avalonia property surface、事件和 mode-specific action 转发。

## 8. Public 行为映射

| 外部入口 | 优化后流向 | 验收要求 |
| --- | --- | --- |
| `SelectedDate` changed | `SelectDate` action -> state normalize -> render | 当前月份按钮选中态刷新；跨月份时 display date 同步。 |
| `SecondarySelectedDate` changed | `SelectRangeEndpoint` action -> range state -> render | range end 和 middle 视觉刷新。 |
| `DisplayDate` changed | `SetDisplayDate` action -> panel rebuild | Header、month cells、nav enable 状态刷新。 |
| `DisplayDateStart/End` changed | `SetDisplayRange` action -> normalize -> render | 超界日期禁用或隐藏，选中值按既有语义修正。 |
| `FirstDayOfWeek` changed | state culture/week start update -> month panel rebuild | 星期标题和日期排列刷新。 |
| `IsTodayHighlighted` changed | state update -> render | today 伪类刷新。 |
| blackout collection changed | state update -> render | blackout 伪类、enabled 状态和选择合法性刷新。 |
| pointer enter day | `SetHoverDate` action | range hover 只由 model 计算。 |
| pointer out month panel | `SetHoverDate(null)` action | hover range 清除。 |
| keyboard navigation | `MoveFocus` action | focused date 和 visual focus 一致。 |
| language variant changed | `SetCulture` action -> panel rebuild | header、weekday、month/year 文本刷新。 |

## 9. 测试矩阵

### 9.1 纯状态测试

| 场景 | 断言 |
| --- | --- |
| `DisplayDateStart > DisplayDateEnd` | end 被归一到 start，render 不循环。 |
| selected date 小于 start | selected date 或 display range 按既有语义归一，只有一次 render。 |
| selected date 大于 end | selected date 或 display range 按既有语义归一，只有一次 render。 |
| blackout selected date | selection 被拒绝或清理，按钮不进入 selected。 |
| range start > end | visual range 始终按小到大渲染。 |
| hover start/end | active part 不同，visual range 端点不同。 |
| first day of week | weekday header 和 42 格日期排列正确。 |
| culture changed | header、weekday、month label 使用新 culture。 |

### 9.2 控件模板测试

| 场景 | 断言 |
| --- | --- |
| template apply 前设置 selected date | apply 后按钮状态正确。 |
| template reapply | 旧按钮 owner、事件、focus 引用被清理。 |
| detached from visual tree | pointer subscription 和 language subscription 被释放。 |
| dual month secondary panel | secondary panel 使用同一 range model。 |
| month/year/decade 切换 | header、panel visibility、focused item 一致。 |

### 9.3 DatePicker 集成测试

| 场景 | 断言 |
| --- | --- |
| DatePicker 选择日期 | 输入显示、SelectedDateTime、popup 状态保持既有行为。 |
| RangeDatePicker 选择起止日期 | start/end 输入、确认按钮、range visual 正确。 |
| TimeView 与 RangeCalendar 联动 | hover/confirm 使用正确日期时间组合。 |
| Today/Now 按钮 | selected date 和 display date 同步。 |
| Gallery showcase | 单选、范围、双月、时间、尺寸、状态示例外观不变。 |

## 10. 实施顺序

实施必须按可验证的小阶段推进，避免把文件拆分、行为修复和视觉改动混在一个不可审查 diff 中。

1. 增加纯状态和现有行为回归测试，锁住当前用户可见行为。
2. 引入 `CalendarCellState`、panel model 和 builder，先由测试验证，不接入 UI。
3. 引入 state controller，替换 `_displayDateIsChanging` 路径。
4. CalendarItem 改为消费 panel model 渲染按钮。
5. 合并 RangeCalendar、RangeCalendarItem、DualMonthRangeCalendar 的 range highlight 分叉。
6. 引入 generated container manager，集中清理事件、binding、owner 和 focus 引用。
7. 移除静态 culture 读取，接入 runtime culture context。
8. 替换 pointer bounds 判断，删除 `Children[7]` 依赖。
9. 删除旧补丁路径、重复 helper、空事件 handler 和过时注释。
10. 运行 DatePicker 定向测试、Desktop Controls 相关测试、Gallery 走查和 `git diff --check`。

## 11. 维护不变量

优化后 CalendarView 必须保持以下不变量：

- 状态只从 `CalendarViewState` 推导，CalendarDayButton 不拥有业务选择状态。
- 同一次外部输入只进入一次 state normalize 和 render。
- Range visual state 只有一个算法入口。
- Culture、pointer、generated containers 和 template parts 都有明确释放路径。
- Token 和主题资源只表达视觉语义，不保存运行时状态。
- AXAML 能表达的 template 关系优先留在 AXAML；C# 动态逻辑只用于动态生成的日期和月份容器。
- 新增 helper 或类型必须对应稳定职责，不能只服务单个补丁路径。

## 12. 验证命令

实现阶段推荐验证：

```bash
dotnet test tests/AtomUI.Desktop.Controls.Tests/AtomUI.Desktop.Controls.Tests.csproj --framework net10.0 --filter FullyQualifiedName~CalendarViewStateTests
dotnet test tests/AtomUI.Desktop.Controls.Tests/AtomUI.Desktop.Controls.Tests.csproj --framework net10.0 --filter DatePicker
dotnet build src/AtomUI.Desktop.Controls/AtomUI.Desktop.Controls.csproj --framework net10.0 --no-restore
git diff --check
```

如果改动影响 Gallery 示例、API 表或 Token 表，还需要运行：

```bash
dotnet test tests/AtomUIGallery.Tests/AtomUIGallery.Tests.csproj --framework net10.0 --no-restore
```
