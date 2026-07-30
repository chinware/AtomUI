# AtomUI Ant Design 6 Calendar 与 CalendarView 设计

## 1. 设计定位

AtomUI `Calendar` 是按日期组织业务展示内容的桌面日历控件。它提供月日期面板、年月份面板、默认年月头部、有效范围、禁用日期、单元格定制以及选择事件，不承担日期输入弹层、范围选择、多日期选择或日程排布职责。

本设计以 Ant Design 6 Calendar 的公开语义为基线。公开 `Calendar` 对应 Ant Design Calendar 组合层；全新的内部 `CalendarView` 对应被 Calendar 隐藏默认头部后使用的日期/月面板。`CalendarView` 在 Calendar 控件目录内独立实现，不复用或包装现有 WPF/Avalonia Calendar 移植体系，也不依赖 DatePicker 的 CalendarView 子系统。

设计覆盖：

- `Calendar` 的 Public API、默认值和事件语义。
- `CalendarHeader` 与 `CalendarView` 的组合关系。
- Month/Year 两种公开模式及其内部 Date/Month 面板映射。
- Cell、FullCell 和 Header 三类定制入口。
- ValidRange、DisabledDate、语言、主题、键盘和生命周期规则。
- Gallery、文档和测试验收边界。

## 2. 设计原则

1. **Ant Design 6 语义优先。** 公开行为以 Ant Design 6 Calendar 为准，不保留旧 Calendar 的 WPF 兼容语义。
2. **Calendar 是唯一业务状态 owner。** `Value`、`Mode`、公开事件和用户选择提交均由 `Calendar` 管理。
3. **CalendarView 是纯面板。** 它负责网格、单元格、焦点和输入投影，只向 Calendar 报告用户意图。
4. **单向数据流。** Calendar 向 Header 和 CalendarView 投影不可变状态；内部控件不得反向维护第二份 Value 或 Mode。
5. **公开模式与内部面板分离。** `CalendarMode.Month` 使用日期网格，`CalendarMode.Year` 使用月份网格。
6. **模板定制不替换交互外壳。** 应用可以替换 Cell 内容，但选中、禁用、焦点、命中测试和 Automation 仍由 CalendarView 管理。
7. **AXAML 承担固定视觉。** 根、Header Host、View Host 和稳定状态视觉由 ControlTheme 表达；只有有界数据单元格由 CalendarView 生成和复用。
8. **不提前抽取共享核心。** 本设计不创建 CalendarPanelCore、公共日期面板接口、泛型日期适配器或 DatePicker 共享基类。

## 3. 范围与非目标

### 3.1 包含范围

- Month 模式下的 6×7 日期网格。
- Year 模式下的 3×4 月份网格。
- Fullscreen 和 Mini 两种视觉密度。
- 可选周序号列，并与 Ant Design 一致支持通过周序号选择该周首日。
- Year Select、Month Select 和 Month/Year 模式切换。
- 有效日期范围与业务禁用规则。
- 日期、月份、完整单元格和 Header 定制。
- Pointer、Keyboard、Focus 和 Automation 基础能力。

### 3.2 非目标

- DatePicker、RangeDatePicker、WeekPicker、QuarterPicker 或 Popup 行为。
- 单日期以外的选择模型。
- `ItemsSource`、Appointment、Scheduler 或跨日期事件布局。
- Decade 面板、Previous/Next 导航头部或 WPF Calendar 导航语义。
- React `generateCalendar<DateType>` 的泛型日期适配能力。
- Calendar 与 DatePicker 的内部代码抽取。

## 4. 术语与模式模型

### 4.1 公开模式

```csharp
public enum CalendarMode
{
    Month,
    Year
}
```

- `Month`：显示一个月的日期网格。
- `Year`：显示一个年份的月份网格。

### 4.2 内部面板模式

```csharp
internal enum CalendarViewMode
{
    Date,
    Month
}
```

映射固定为：

| Calendar.Mode | CalendarView.ViewMode | 网格 |
| --- | --- | --- |
| `Month` | `Date` | 6×7 日期 Cell |
| `Year` | `Month` | 3×4 月份 Cell |

这层映射属于 Calendar 的内部不变量。`CalendarViewMode` 不进入 Public API。

### 4.3 选择来源

```csharp
public enum CalendarSelectSource
{
    Year,
    Month,
    Date,
    Customize
}
```

| 操作入口 | Source |
| --- | --- |
| 默认 Header 的 Year Select | `Year` |
| 默认 Header 的 Month Select | `Month` |
| Date 面板选择日期 | `Date` |
| Month 面板选择月份 | `Month` |
| 自定义 Header 提交 Value | `Customize` |

## 5. Public API

### 5.1 Calendar 属性

```csharp
public class Calendar : TemplatedControl
{
    public DateTime Value { get; set; }

    public CalendarMode Mode { get; set; }

    public bool Fullscreen { get; set; }

    public bool ShowWeek { get; set; }

    public CalendarDateRange? ValidRange { get; set; }

    public Func<DateTime, bool>? DisabledDate { get; set; }

    public IDataTemplate? CellTemplate { get; set; }

    public IDataTemplate? FullCellTemplate { get; set; }

    public IDataTemplate? HeaderTemplate { get; set; }
}
```

默认值：

| 属性 | 默认值 | 语义 |
| --- | --- | --- |
| `Value` | 每个实例创建时的 `DateTime.Today` | 当前选中日期和默认面板锚点 |
| `Mode` | `CalendarMode.Month` | 日期网格 |
| `Fullscreen` | `true` | 完整数据展示布局 |
| `ShowWeek` | `false` | 不显示周序号列 |
| `ValidRange` | `null` | 不附加范围限制 |
| `DisabledDate` | `null` | 不附加业务禁用规则 |
| 三个 Template | `null` | 使用默认 Header 和 Cell 内容 |

Calendar 使用 `DateTime` 与 AtomUI 日期控件体系保持一致，不引入 `DateOnly` 或泛型 DateType。Calendar 的比较、选择和范围判断均采用日期部分；用户选择写回的 Value 规范化到 `.Date`。

外部为 `Value` 提供带时间值时，Calendar 在属性入口将其规范化到 `.Date`，使公开属性、Header Context、Cell Model 与事件参数始终观察到同一个日期值。该规范化属于属性约束，不触发 `ValueChanged`、`Selected` 或 `PanelChanged`，也不得以二次 `SetCurrentValue` 破坏调用方绑定。Calendar 不提供时间编辑能力。

### 5.2 ValidRange

`CalendarDateRange` 表示包含首尾的有效范围：

```csharp
public sealed class CalendarDateRange
{
    public CalendarDateRange(DateTime start, DateTime end);

    public DateTime Start { get; }

    public DateTime End { get; }
}
```

- `Start` 和 `End` 规范化到 `.Date`。
- `end < start` 时构造函数抛出 `ArgumentOutOfRangeException`，不静默折叠范围。
- 该类型不再表示选中范围或 BlackoutDates 集合项，只服务 `ValidRange`。

### 5.3 Cell 模板上下文

```csharp
public enum CalendarCellType
{
    Date,
    Month
}

public sealed record CalendarCellContext(
    DateTime Value,
    DateTime Today,
    CalendarCellType CellType,
    string DisplayValue,
    bool IsToday,
    bool IsInView,
    bool IsSelected,
    bool IsDisabled);
```

- `CellTemplate` 对应 Ant Design `cellRender`，只填充默认日期/月值下方的业务内容区域；日期/月值始终保留。
- `FullCellTemplate` 对应 Ant Design `fullCellRender`，替换 Cell 的完整 inner 内容。
- 两者同时存在时，`FullCellTemplate` 优先。
- 周标题和周序号不进入这两个模板。
- 最外层 `CalendarViewCell` 始终由 AtomUI 保留，因此模板不能绕过禁用、焦点、选择、命中测试和 Automation 语义。

### 5.4 Header 模板上下文

```csharp
public sealed class CalendarHeaderContext
{
    public DateTime Value { get; }

    public CalendarMode Mode { get; }

    public ICommand ChangeValueCommand { get; }

    public ICommand ChangeModeCommand { get; }
}
```

- `HeaderTemplate` 为 `null` 时使用默认 `CalendarHeader`。
- 自定义 Header 通过 `ChangeValueCommand` 提交 `DateTime`，其选择来源为 `Customize`。
- `ChangeModeCommand` 接收 `CalendarMode`。
- 参数类型不匹配或模式枚举无效时，命令 `CanExecute` 返回 `false`。
- 与 Ant Design 一致，`ChangeValueCommand` 不自动应用 ValidRange 或 DisabledDate；自定义 Header 负责约束自己提交的日期。
- Header Context 不暴露 Calendar 实例，不允许模板直接调用内部状态方法。

### 5.5 公开事件

```csharp
public event EventHandler<CalendarValueChangedEventArgs>? ValueChanged;

public event EventHandler<CalendarSelectedEventArgs>? Selected;

public event EventHandler<CalendarPanelChangedEventArgs>? PanelChanged;
```

事件参数：

```csharp
public sealed class CalendarValueChangedEventArgs : EventArgs
{
    public DateTime OldValue { get; }
    public DateTime NewValue { get; }
}

public sealed class CalendarSelectedEventArgs : EventArgs
{
    public DateTime Value { get; }
    public CalendarSelectSource Source { get; }
}

public sealed class CalendarPanelChangedEventArgs : EventArgs
{
    public DateTime Value { get; }
    public CalendarMode Mode { get; }
}
```

这些事件表达 Ant Design Calendar 的用户回调语义。程序直接设置 `Value` 或 `Mode` 只更新属性和渲染，不触发三个用户交互事件；需要观察所有属性变化的调用方使用 Avalonia Property Observable。

## 6. 事件与状态转换

### 6.1 日期或月份选择

一次有效用户选择按以下原子流程处理：

```text
CalendarView.CellSelected / CalendarHeader change
  -> Calendar 按输入来源处理目标日期
  -> Calendar 写入 Value
  -> 跨面板时触发 PanelChanged
  -> 日期实际变化时触发 ValueChanged
  -> 每次有效选择触发 Selected
```

事件顺序固定为 `PanelChanged -> ValueChanged -> Selected`，不存在的事件从序列中省略。所有事件触发时，`Value` 和渲染输入已经提交到新状态。

| 操作 | PanelChanged | ValueChanged | Selected |
| --- | --- | --- | --- |
| 选择当前日期 | 否 | 否 | `Date` |
| 选择同月其他日期 | 否 | 是 | `Date` |
| 选择相邻月份补位日期 | 是，当前 Mode | 是 | `Date` |
| 默认 Header 修改月份 | 跨月时是 | 日期变化时是 | `Month` |
| 默认 Header 修改年份 | Month 模式跨月或 Year 模式跨年时是 | 日期变化时是 | `Year` |
| Year 模式选择当前年份月份 | 否 | 日期变化时是 | `Month` |
| 自定义 Header 修改日期 | 按面板边界判断 | 日期变化时是 | `Customize` |

CalendarView 的禁用 Cell 不会写入 Value，也不会触发任何事件。默认 Header 通过 ValidRange 约束年份和月份选项，但不应用 DisabledDate。自定义 Header 的 ChangeValueCommand 与 Ant Design `headerRender.onChange` 一致，可以提交范围外或 DisabledDate 命中的日期。

### 6.2 Mode 切换

用户通过默认或自定义 Header 切换 Mode 时：

1. Calendar 写入 Mode。
2. Calendar 将 Mode 映射为 CalendarViewMode。
3. CalendarView 使用现有 Value 重建对应网格。
4. Calendar 触发一次 `PanelChanged(Value, newMode)`。

Mode 切换不触发 `ValueChanged` 或 `Selected`。程序直接设置 Mode 不触发 `PanelChanged`。

### 6.3 PanelChanged 边界

- Date 面板仅在新旧 Value 不属于同一自然月时触发。
- Month 面板仅在新旧 Value 不属于同一自然年时触发。
- Mode 用户切换始终触发一次。
- Fullscreen、ShowWeek、Culture、Template、ValidRange 和 DisabledDate 变化不触发。

## 7. 架构与职责

### 7.1 组合结构

```text
Calendar
├── CalendarHeader 或 HeaderTemplate
└── CalendarView
    ├── WeekHeader
    └── CellHost
        └── CalendarViewCell × N
```

### 7.2 Calendar

`Calendar` 是公开契约和业务状态 owner：

- 注册 Public Avalonia Properties 和事件。
- 归一 Value、Mode、ValidRange 和语言输入。
- 创建 Header Context。
- 向默认 Header 和 CalendarView 投影状态。
- 接收内部用户意图并执行第 6 节状态转换。
- 管理 `OnApplyTemplate`、旧 part 解绑和新 part 状态回放。

Calendar 不生成日期网格，不维护 Cell 容器，也不在 Pointer handler 中计算日期。

### 7.3 CalendarHeader

`CalendarHeader` 是 Calendar 专用内部控件：

- 展示 Year Select、条件 Month Select 和 Month/Year 模式切换。
- 接收 Value、Mode、Fullscreen、ValidRange 和本地化文本。
- 只报告 Year、Month 或 Mode 用户操作。
- 不拥有 Value、Mode 或选择状态。

默认 Header 规则：

- 无 ValidRange 时，Year Select 提供 `[currentYear - 10, currentYear + 9]` 共 20 年。
- 有 ValidRange 时，提供 Start.Year 到 End.Year，首尾包含。
- Month Select 只在 `Mode=Month` 时显示。
- 当前年份处于 ValidRange 边界年份时，Month Select 只提供与范围相交的月份。
- 切换到边界年份时，当前月份超出范围则收敛到边界月份。
- Fullscreen 使用默认 ComboBox/Segmented 尺寸；Mini 使用 Small 尺寸。
- Month/Year 模式标签、周标题的可访问名称以及中文年份后缀来自 Calendar 语言资源，不在 C# 或 AXAML 中硬编码英文文本。

### 7.4 CalendarView

```csharp
internal sealed class CalendarView : TemplatedControl
```

CalendarView 位于 Calendar 所有的 internal namespace 中，例如：

```csharp
namespace AtomUI.Desktop.Controls.Internal.Calendar;
```

该命名避免与 DatePicker 当前使用的 `AtomUI.Desktop.Controls.CalendarView` namespace 冲突。

CalendarView 输入：

- Value、Today、ViewMode。
- ValidRange、DisabledDate。
- ShowWeek、Culture、FlowDirection。
- CellTemplate、FullCellTemplate。

CalendarView 输出只有内部 `CellSelected` 用户意图。它只维护 `FocusedValue`、当前 Cell Model 和已生成容器，不持有第二份 SelectedValue 或公开 Mode。

### 7.5 CalendarViewCell

`CalendarViewCell` 是内部有界数据容器：

- 应用一个不可变 Cell Model。
- 投影 CellTemplate 或 FullCellTemplate。
- 管理 Pointer、Focus、Keyboard、Automation 和伪类。
- 将有效激活报告给 CalendarView。

Cell 不判断 PanelChanged，不写 Calendar.Value，不缓存业务对象，也不订阅全局服务。

## 8. 源码与主题边界

稳定 ownership 结构：

```text
src/AtomUI.Desktop.Controls/Calendar/
├── Calendar.cs                         # Public API、状态转换、生命周期
├── CalendarDateRange.cs                # ValidRange 值对象
├── CalendarCellContext.cs              # Public Cell 模板上下文
├── CalendarHeaderContext.cs            # Public Header 模板上下文
├── CalendarEventArgs.cs                # Public 事件模型
├── CalendarControlToken.cs             # 新 Calendar 的六语义组件 Token
├── CalendarToken.cs                    # DatePicker CalendarView 继续使用的旧 Token
├── Localization/                       # Month/Year/YearSuffix/Week 语言资源
│   ├── en_US.cs
│   ├── zh_CN.cs
│   └── zh_TW.cs
├── Internal/
│   ├── CalendarHeader.cs               # 默认 Header
│   ├── CalendarView.cs                 # 面板与容器 owner
│   ├── CalendarViewCell.cs             # Cell 交互外壳
│   ├── CalendarViewCellModel.cs        # 不可变内部模型
│   └── CalendarViewCellBuilder.cs      # 纯日期/月网格构建
└── Themes/
    ├── CalendarTheme.axaml
    ├── CalendarHeaderTheme.axaml
    ├── CalendarViewTheme.axaml
    ├── CalendarViewCellTheme.axaml
    └── CalendarThemes.axaml
```

`CalendarViewCellBuilder` 只接收纯值输入并返回不可变 Cell Model；它不引用 Control、不触发事件，也不是共享核心或可扩展接口。

旧 WPF/Avalonia 移植体系不属于新 ownership：

- BaseCalendarButton、BaseCalendarDayButton。
- CalendarButton、CalendarDayButton、CalendarItem、HeadTextButton。
- SelectedDatesCollection、CalendarBlackoutDatesCollection。
- 旧 CalendarExtensions 和选择模型辅助代码。
- 对应旧 ControlTheme 与 Template Part。

## 9. Template 与语义区域

### 9.1 Calendar Template

```text
Calendar / PART_Root
├── PART_HeaderPresenter
└── PART_CalendarView
```

- `PART_HeaderPresenter` 承载默认 CalendarHeader 或 HeaderTemplate。
- `PART_CalendarView` 必须是新的内部 CalendarView。
- 固定视觉关系使用 TemplateBinding 或 AXAML selector，不使用 C# relay binding。

### 9.2 CalendarView Template

```text
CalendarView / PART_Body
├── PART_WeekHeader
└── PART_CellHost
```

- Date 模式显示 WeekHeader；Month 模式隐藏。
- CellHost 根据 ViewMode 使用 7/8 列日期布局或 4 列月份布局。
- Fullscreen/Mini 只改变布局和视觉密度，不改变业务状态或 Cell Model 语义。

### 9.3 Cell Template

```text
CalendarViewCell / PART_Item
└── PART_CellInner
    ├── PART_Value
    └── PART_ItemContent
```

- FullCellTemplate 替换 `PART_CellInner` 的默认内容。
- CellTemplate 只进入 `PART_ItemContent`。
- `PART_Item` 保留状态、焦点、Automation 和命中测试。
- `CellTemplate` 存在时 `PART_Value` 仍然显示，模板内容只进入 `PART_ItemContent`。
- `FullCellTemplate` 存在时隐藏默认 inner 结构并只显示完整模板；它与 `CellTemplate` 同时存在时具有确定的优先级。
- 周序号 Cell 始终使用默认周序号内容，不接收 `CellTemplate` 或 `FullCellTemplate`。

### 9.4 Ant Design 6 Semantic 映射

| Ant Semantic | AtomUI 区域 |
| --- | --- |
| `root` | Calendar / `PART_Root` |
| `header` | `PART_HeaderPresenter` / CalendarHeader |
| `body` | CalendarView / `PART_Body` |
| `content` | CalendarView / `PART_CellHost` |
| `item` | CalendarViewCell / `PART_Item` |
| `itemContent` | `PART_ItemContent` |

应用应通过 Public Template、ControlTheme 和稳定伪类定制这些区域，不暴露 React classNames/styles API。

## 10. 网格算法

### 10.1 Date 网格

输入：Value.Date、Today.Date、Culture、ValidRange、DisabledDate。

计算：

1. 取 Value 所在月第一天 `monthStart`。
2. 从 Culture 获取 `FirstDayOfWeek`。
3. 计算 `monthStart` 相对周首日的偏移。
4. 得到网格起点 `gridStart = monthStart - offset days`。
5. 从 gridStart 连续生成 42 个日期。
6. 为每个日期计算 IsInView、IsToday、IsSelected 和 IsDisabled。

输出固定 42 个 Date Cell Model。前后月份补位日期保留完整交互语义；选择补位日期通过 Calendar 触发跨月 PanelChanged。

默认日期值使用两位显示文本，例如 `01`、`09`、`31`。CellTemplate 不改变这个默认值格式；FullCellTemplate 可以完全替换 inner 内容。

### 10.2 Week Header 与周序号

- 星期标题按 Culture 的 FirstDayOfWeek 循环排列。
- ShowWeek=false 时只显示 7 个星期标题和 42 个日期 Cell。
- ShowWeek=true 时增加一个周序号标题槽和每行一个周序号 Cell。
- 周序号使用 Culture.Calendar、CalendarWeekRule 和 FirstDayOfWeek 计算。
- 周序号 Cell 不触发 CellTemplate 或 FullCellTemplate。
- 与 Ant Design 面板一致，激活周序号 Cell 选择该行的周首日，以 `CalendarSelectSource.Date` 进入标准提交和事件流程。
- 周序号 Cell 的禁用状态使用该行周首日执行 Calendar 合并后的禁用谓词；禁用时不提交选择。
- ShowWeek 在 Month 面板中忽略，不改变 3×4 月份网格。

### 10.3 Month 网格

输入：Value.Date、Today.Date、ValidRange、DisabledDate 和 Culture。

从一月至十二月生成 12 个 Month Cell Model。每个候选值保留 Value 的日，并截断到目标月份最后一天：

```text
2028-01-31 -> February -> 2028-02-29
2027-01-31 -> February -> 2027-02-28
```

月份显示使用本地化短月份名称。月份禁用逻辑与 Ant Design MonthPanel 一致：分别对月首和月末执行 Calendar 合并后的禁用谓词，只有两端都禁用时才禁用整月。该规则使 ValidRange 与月份部分相交时仍可选择该月，也避免仅因保留日命中 DisabledDate 就错误禁用整月。选择月份后保持 CalendarMode.Year。

### 10.4 禁用组合

Date Cell 的禁用规则：

```text
IsDisabled = OutsideInclusiveValidRange || DisabledDate(Value)
```

- ValidRange 的 Start 和 End 均可选择。
- DisabledDate 不得覆盖范围限制，只能增加禁用日期。
- DisabledDate 的异常不被吞掉或转换为可用状态。
- ValidRange 或 DisabledDate 变化只重新构建 Cell Model，不隐式修改 Value，也不触发用户事件。
- 当前外部 Value 位于有效范围外时仍作为面板锚点展示；相关 Cell 按禁用规则投影，Calendar 不擅自修改调用方状态。

## 11. 焦点、键盘与 Automation

CalendarView 使用 roving focus：

- 进入面板时优先聚焦选中且可用的 Cell，否则聚焦第一个可用 Cell。
- Date 模式左右方向键移动一天，上下方向键移动一周。
- Month 模式左右方向键移动一个月，上下方向键移动四个月。
- 焦点移动只改变 `FocusedValue`，不选择、不触发公开事件。
- Enter 和 Space 激活当前 Focused Cell，并进入与 Pointer 相同的选择流程。
- 焦点移动到当前已生成网格之外时停止，不隐式引入 Previous/Next 导航语义。
- Disabled Cell 不可成为提交目标；方向键跳过不可聚焦 Cell。
- 周序号不进入日期/月 roving focus 序列；它通过 Pointer 和 Automation 激活周首日。

CalendarView 的方向键导航属于 AtomUI 桌面增强，不以降低 Ant Design 的公开功能为代价。目标 Cell 禁用时，导航沿相同步长继续搜索当前已生成网格中的下一个可聚焦 Cell，直到找到目标或越出网格。

Avalonia 12 的跨平台 Automation Provider 未公开 `IGridProvider`/`IGridItemProvider`。因此 CalendarView 使用平台可实现的契约：

- CalendarView 暴露 `AutomationControlType.Table` 和单选 `ISelectionProvider`。
- 日期/月 Cell 暴露 `AutomationControlType.ListItem` 和 `ISelectionItemProvider`。
- `SelectionContainer` 返回所属 CalendarView 的 Selection Provider，`GetSelection` 返回当前选中 Cell。
- 周序号保留可访问名称和激活能力，但不冒充当前选中日期。

选中、禁用、名称和激活结果必须来自同一 Cell Model。FullCellTemplate 不得移除外层 Automation Peer。文档不得再宣称当前 Avalonia 未提供的 GridItem Provider 能力。

## 12. 语言与方向

- Calendar 使用 AtomUI 语言服务和当前 Culture，不引入 React Locale 对象。
- 语言变化使 Header 选项、月份名称、星期标题和周序号规则失效并重建。
- Calendar 继承 FlowDirection；RTL 只改变布局方向和视觉对齐，不改变 Value、事件顺序或日期计算。
- 默认 Header 的 Month/Year 模式标签、周序号可访问名称和中文年份后缀遵循 Calendar 语言资源，不在 CalendarHeader 或 AXAML 中硬编码。
- Calendar 新增 `Month`、`Year`、`YearSuffix`、`Week` 四个语言资源键；这是完整对齐所需的唯一增量公开资源面，不增加 Calendar 属性、事件或模板入口。

## 13. 主题与 Token

Calendar 组件 Token 收敛为 Ant Design 6 Calendar 的六个公开视觉语义：

| Token | 默认语义 |
| --- | --- |
| `FullBg` | 完整 Calendar 背景，派生自容器背景 |
| `FullPanelBg` | 完整 Calendar Panel 背景，派生自容器背景 |
| `ItemActiveBg` | 完整模式选中日期/月单元背景，派生自 active item 背景 |
| `YearControlWidth` | Year Select 最小宽度，默认 80 |
| `MonthControlWidth` | Month Select 最小宽度，默认 70 |
| `MiniContentHeight` | Mini 内容高度，默认 256 |

日期值高度、周标题高度、内容高度、Padding、Border、Typography 和 Motion 从 SharedToken 派生，不扩大 Calendar 专属 Token surface。旧范围选择、固定 Cell 尺寸和旧 Header 导航 Token 不属于新 Calendar。

根伪类：

```text
:fullscreen
:mini
:month
:year
:show-week
```

Cell 伪类：

```text
:date
:month
:week
:today
:selected
:outside
:disabled
:focused
```

Fullscreen 和 Mini 使用同一 CalendarView 与 Cell Model。Fullscreen/Mini 切换只由 selector 和布局资源改变视觉，不创建第二套控件逻辑。

视觉结构遵循 Ant Design 的两种密度语义：

- Fullscreen Cell 将日期/月值与业务内容纵向分区，值区靠面板末端对齐，内容区独立布局并允许溢出滚动；Cell 顶部分隔线、Today、Selected、Hover 和 Disabled 状态由外层主题表达。
- Mini Cell 使用紧凑居中值布局，内容区和 FullCellTemplate 仍遵循相同的渲染优先级，不改变用户模板语义。
- WeekHeader 使用与 CellHost 相同的 Grid 列定义，ShowWeek 开关前后标题与 Cell 始终对齐。
- Mini Header 的 Year Select、Month Select 和模式 Segmented 全部使用 Small；Fullscreen 使用 Middle/default。

## 14. 生命周期、性能与 AOT

### 14.1 生命周期

- Calendar 每次 OnApplyTemplate 前解绑旧 CalendarHeader、CalendarView 和 Header Context command 协作。
- 新 part 接入后立即回放当前 Value、Mode、Range、Template、Culture 和 FlowDirection。
- CalendarView 在 template reapply、detach 和 owner 替换时先清空旧 WeekHeader/CellHost，再对所有池化 Cell 执行显式 Unbind，释放 owner、model、context、CellTemplate、FullCellTemplate 与焦点状态。
- CalendarViewCell 只订阅 owner 范围内事件；容器回收时恢复完整伪类和 DataContext 状态。
- Calendar 的提交流不使用全局事件、timer、Dispatcher 延迟刷新或 commit 抑制标记维持一致性。CalendarHeader 仅可在向模板 part 回放状态的同步窗口内保护 SelectionChanged，并必须用确定的进入/退出边界保证真实用户输入不被吞掉。

### 14.2 性能边界

- Date 模式最多实现 42 个日期容器；ShowWeek 增加 6 个周序号容器。
- Month 模式只激活 12 个容器。
- CalendarView 使用一个有界容器池复用 CalendarViewCell，Mode、Value、Range 和语言变化不无限增加容器。
- Fullscreen/Mini 切换只更新样式，不重建 Cell Model 或模板内容。
- Cell Model 只在 Value、ViewMode、ShowWeek、ValidRange、DisabledDate、Culture 或 Today 失效时重建，不在 Measure/Arrange 热路径计算。
- Date Cell 和周序号 Cell 对各自日期每次重建最多调用一次 DisabledDate；Month Cell 按月首、月末短路判断，每月最多调用两次。
- 没有 CellTemplate/FullCellTemplate 时不创建业务 ContentPresenter 内容。

性能验收必须记录默认实例的 Visual 数量、实例化分配、Mode 重复切换后的容器数量和可回收性；设计不预设未经测量的性能提升结论。

### 14.3 AOT

- 不引入反射日期适配、运行时类型扫描或字符串属性发现。
- Public 和 internal Avalonia Properties 使用静态注册。
- 固定模板关系使用 TemplateBinding、AXAML selector 或强类型 binding。
- Gallery DataTemplate 使用 `x:DataType`。
- Header/Cell context 是显式强类型，不依赖动态字典或 ExpandoObject。

## 15. 兼容性与定制边界

本设计不兼容旧 Calendar 的 Public API、Template Part、ControlTheme key、伪类或行为。旧体系中的以下语义不提供迁移映射：

- SelectedDate、SelectedDates、SelectionMode。
- DisplayDate、DisplayDateStart、DisplayDateEnd。
- BlackoutDates、Decade 和范围选择。
- Previous/Next Header、CalendarItem、CalendarButton 和 CalendarDayButton。

新的稳定定制边界是：

- Value、Mode、Fullscreen、ShowWeek、ValidRange、DisabledDate。
- CellTemplate、FullCellTemplate、HeaderTemplate 及其强类型上下文。
- 三个公开事件及其触发顺序。
- `Month`、`Year`、`YearSuffix`、`Week` 四个 Calendar 语言资源键。
- 第 9 节 Template Part、第 13 节 Token 和伪类。

应用负责模板内部业务内容的视觉与业务数据；AtomUI 负责外层交互、选择、禁用、焦点、Automation、主题和生命周期。

## 16. Gallery 与文档契约

Calendar Gallery 必须覆盖以下稳定示例：

1. 基础 Fullscreen Month Calendar。
2. Mini Calendar。
3. Year 模式。
4. ShowWeek。
5. ValidRange 与 DisabledDate 组合。
6. CellTemplate 业务内容。
7. FullCellTemplate，并证明其优先于 CellTemplate。
8. 使用 `CalendarHeaderContext` 强类型绑定和命令的自定义 HeaderTemplate。
9. ValueChanged、Selected 和 PanelChanged 事件来源展示。

Gallery API 表应只暴露新 Public API；Token 表只展示第 13 节六个 Calendar Token。控件实现落地时同步重写 Calendar 的 overview.md、implementation.md、token.md 和 changelog.md，并使 LLMS 输入来源与 Gallery 示例一致。`overview.md` 必须满足控制文档标准章节要求；LLMS 文件只能通过生成/同步流程更新，不手工维护过期的旧 Calendar 契约。

## 17. 验证要求

### 17.1 纯逻辑测试

- 42 个 Date Cell 的起点、顺序和 IsInView。
- 12 个 Month Cell 的候选值和月末截断。
- 闰年、跨年、不同 FirstDayOfWeek 和 CalendarWeekRule。
- ValidRange 首尾包含与非法 Range 构造。
- ValidRange 与 DisabledDate 合并。
- Month Cell 使用月首/月末合并禁用谓词，而不是候选保留日。
- ShowWeek 周序号使用周首日的禁用与选择语义。

### 17.2 控件行为测试

- 每个 Public 属性的默认值和模板应用前后状态回放。
- 程序设置带时间的 Value 在属性入口规范化为 `.Date`，且不触发用户事件、不破坏绑定。
- Month/Year 与 Date/Month 内部模式映射。
- 四种 CalendarSelectSource。
- PanelChanged、ValueChanged、Selected 的条件和顺序。
- 程序设置 Value/Mode 不触发用户事件。
- 禁用 Cell、相邻月份 Cell 和重复选择当前值。
- Header 边界年份/月选项和自定义 Header command。
- Pointer、Keyboard、Focus 和 Automation 状态一致。
- 方向键连续跳过禁用 Cell，直到当前网格内的下一可用 Cell。
- CalendarView Selection Provider、Cell SelectionContainer 与当前选中 Cell 一致。

### 17.3 Template 与主题测试

- 稳定 Template Part 类型和组合关系。
- CellTemplate 保留默认日期/月值，FullCellTemplate 完整替换 inner 且优先，周序号不消费两类模板。
- Fullscreen/Mini、Month/Year、ShowWeek 和 Cell 伪类。
- WeekHeader 与 CellHost 列对齐，Mini Header 使用 Small 控件尺寸，本地化模式标签和中文年份后缀正确。
- Light/Dark 和运行时主题切换。
- 六个 Calendar Token 的默认派生与资源消费。

### 17.4 生命周期与性能测试

- Template reapply 不保留旧 Header、View 或 Cell handler。
- Detach 后 Calendar、Header Context 和 Cell 可被回收。
- 重复切换 Mode、Language、Theme 和 Template 后容器数量有界。
- 默认实例与当前基线的 Visual 数量和分配对比有记录。

### 17.5 Gallery 与 AOT

- Calendar Gallery API、Token 和 ShowCase snapshot 与新契约一致。
- Calendar 定向控件测试与 Gallery 测试通过。
- `git diff --check` 通过。
- NativeAOT Gallery publish 在包含强类型 Cell/Header 模板示例时通过。

## 18. 抽取边界

本设计的实现边界止于 Calendar 目录。CalendarView、Cell Model、网格构建和容器生命周期均归 Calendar 所有，DatePicker 不参与该状态流。

任何跨 Calendar 与 DatePicker 的共享抽取必须由独立设计证明已有实现具有相同输入、输出、日期语义、失效条件和生命周期。该抽取不得作为本设计实施的前置步骤，也不得改变这里定义的 Calendar Public API、事件顺序、Template 或主题契约。

## 19. 旧代码清理与共享类型迁移

本设计要求彻底删除旧 `Calendar/` 目录的 WPF/Avalonia 移植体系并重写。但当前部分定义在旧 `Calendar/` 目录、被 `DatePicker/CalendarView/` 复用的类型不能直接删除，否则 DatePicker 无法编译。本次重构不改动 DatePicker 的 CalendarView 行为，因此这些被复用类型迁移而非删除。

### 19.1 迁移原则

- DatePicker 仍需要、当前定义在旧 `Calendar/` 目录的类型，迁移到 `DatePicker/CalendarView/` 目录并归 DatePicker 所有（保持或收敛为 `internal`）。
- 迁移只改变类型归属和 `namespace`/可见性，不改变 DatePicker 的运行时行为、日期语义或视觉。
- 迁移完成后，旧 `Calendar/` 目录整体删除，由新 Calendar 实现重建。
- 新 Calendar 按第 5.2 节重新定义自己的 `CalendarDateRange`（只服务 ValidRange，`end < start` 抛异常，不再承载 BlackoutDates）；该类型与 DatePicker 迁移走的同名类型彼此独立，不共享定义。

### 19.2 迁移候选

以下类型在实现阶段逐一甄别：凡 DatePicker 真实依赖且当前仅由旧 `Calendar/` 目录提供的，迁移到 DatePicker；DatePicker 自身已有的同名 `internal` 类型不受影响。

- `DateTimeHelper`（DatePicker 大量依赖，迁移到 DatePicker 内部）。
- `CalendarDateRange`（DatePicker 用作范围/黑名单区间值对象，迁移；新 Calendar 另立同名类型）。
- `CalendarMode`、`CalendarSelectionMode`、`CalendarExtensions`、`CalendarDateChangedEventArgs`、`CalendarModeChangedEventArgs` 等若被 DatePicker 引用且无 DatePicker 自有定义，一并迁移。

甄别标准：对每个候选类型确认 (1) DatePicker 是否引用它，(2) DatePicker 是否已自带同名定义。只有「被引用且无自有定义」的类型需要迁移。

### 19.3 消费点更新

旧 Calendar 只有三处外部消费点，全部在删除旧实现后指向新 Calendar：

- 主题注册 `DesktopControlThemesProvider.axaml` 与 `BrowserDesktopControlThemesProvider.axaml` 的 `Calendar/Themes/CalendarThemes.axaml`。
- Gallery `CalendarShowCase`（axaml、ViewModel、本地化资源）按第 16 节九个示例重写。
- 测试 `CalendarShowCaseExamples.snapshot` 与 `CalendarShowCasePageTests` 按新契约更新。

生成文件（`GeneratedControlThemeAssetManifest.g.cs`、`GeneratedThemeSchema.g.cs`）由代码生成器随源码重建，不手工编辑。

### 19.4 命名空间隔离（彻底重构，不兼容）

阶段一迁移时，DatePicker 接管的共享类型暂时保留在 `AtomUI.Desktop.Controls` 顶层命名空间。这与新 Calendar 公开类型（`CalendarDateRange`、`CalendarMode` 等，spec §5，同样要求 `AtomUI.Desktop.Controls`）直接同名冲突。

本设计彻底重构、不考虑兼容，因此：

- **顶层公开命名空间 `AtomUI.Desktop.Controls` 归新 Calendar 独占。** 新 Calendar 的公开类型（`CalendarMode{Month,Year}`、`CalendarSelectSource`、`CalendarCellType`、`CalendarDateRange`、`CalendarCellContext`、`CalendarHeaderContext`、三个事件参数）在此命名空间定义。
- **DatePicker 接管的类型全部迁入 `AtomUI.Desktop.Controls.CalendarView` 命名空间**（与 DatePicker CalendarView 现有代码一致）。这些类型（`DateTimeHelper`、`CalendarExtensions`、`HeadTextButton`、`CalendarDateRange`（DatePicker 版，含 `ContainsAny`/单日构造/折叠语义）、`CalendarMode`（含 `Decade`）、`CalendarSelectionMode`、`CalendarDateChangedEventArgs`、`CalendarModeChangedEventArgs`）是 DatePicker 内部实现细节，其 `public` 仅为程序集内可见性，不构成对外库 API，迁移命名空间不破坏任何真正对外的公开契约。
- 引用这些类型但不在 `AtomUI.Desktop.Controls.CalendarView` 命名空间的 DatePicker 文件（如 `DatePickerFormattingHelper.cs`、`RangeDatePicker.cs`、`DualMonthRangeDatePickerPresenter.cs`）添加 `using AtomUI.Desktop.Controls.CalendarView;`。
- **新 Calendar 不得为 DatePicker 兼容而在公开类型上增加成员**（例如公开 `CalendarMode` 不得含 `Decade`，公开 `CalendarDateRange` 不得含 `ContainsAny`）。两套类型彻底隔离，各自独立演进。

### 19.5 Design Token 隔离

旧 `CalendarToken`（类名 `CalendarToken`，ID `"Calendar"`）当前被 DatePicker 的 CalendarView 主题（`CalendarButtonTheme.axaml`、`CalendarDayButtonTheme.axaml`、`CalendarItemTheme.axaml` 等）通过 `CalendarTokenResource` 引用（`CellHeight`/`CellHoverBg`/`CellActiveWithRangeBg`/`CellWidth`/`CellMargin`/`CellLineHeight`/`CellBgDisabled`/`WithoutTimeCellHeight` 等）。源生成器按 **token 类名** 生成对应的 `XxxTokenResource` / `XxxTokenKind`。

本次重构**不改动 DatePicker 实现**（含其主题与 token 引用），因此：

- **旧 `CalendarToken` 类原封不动保留**，继续归 DatePicker 的 CalendarView 使用。
- **新 Calendar 使用独立的 Design Token 类 `CalendarControlToken`（ID `"CalendarControl"`）**，只含 spec §13 的六个语义（`FullBg`/`FullPanelBg`/`ItemActiveBg`/`YearControlWidth`/`MonthControlWidth`/`MiniContentHeight`）。新 Calendar 主题通过 `CalendarControlTokenResource` 引用。
- 两个 token 类彼此独立，互不影响；DatePicker 视觉与行为零变化。
