# DatePicker 语义结构

> 生成产物：由源文档生成，不要手工编辑。修改内容请回到控件文档、源码 public surface、Token 类型或生成数据、Gallery ShowCase 或源码结构。

## Semantic Parts

| Part | AtomUI 节点 | 职责 | 相关 API | 相关 Token | 稳定性 |
| --- | --- | --- | --- | --- | --- |
| `root` | `DatePicker` | 数据录入控件根语义区域，承载 public API、值状态、验证状态和主题入口。 | 见 API 与契约模型 | 见视觉与主题模型 | stable |
| `input` | `输入或编辑区域` | 承载用户输入、当前值、占位、格式化或只读状态。 | 见 API 与契约模型 | 见视觉与主题模型 | stable |
| `trigger` | `触发区域` | 承载清除、展开、提交、步进、上传或辅助操作。 | 见 API 与契约模型 | 见视觉与主题模型 | stable |
| `popup` | `弹层或候选区域` | 承载下拉、候选项、日历、颜色面板或异步内容。 | 见 API 与契约模型 | 见视觉与主题模型 | stable |
| `validation` | `校验反馈区域` | 承载 Form、status、错误、警告、help 或 loading 状态。 | 见 API 与契约模型 | 见视觉与主题模型 | stable |

## Abstract AXAML Structure

未定位到可生成抽象 AXAML 结构的 ControlTheme 模板。生成器不会根据 semantic parts 发明 AXAML 节点；请以 Template Parts、主题文件和源码索引为准。

## Composition Model

该章节由控件 `Themes/` 文件夹中的真实主题文件生成，用于说明 public 控件与内部协作对象之间的运行时结构。内部节点只用于理解和维护，不应指导用户代码直接依赖。

### 控件角色图

```text
DatePicker
  -> DatePickerPresenter (presenter control theme, DatePickerPresenterTheme.axaml)
     -> DockPanel#RootLayout (template-stable)
        -> PixelAlignedBorder#ButtonsFrame (template-stable)
           -> Panel#ButtonsLayout (template-stable)
              -> Button#PART_NowButton (template-stable)
              -> Button#PART_TodayButton (template-stable)
              -> Button#PART_ConfirmButton (template-stable)
        -> StackPanel (template-stable)
           -> Calendar#PART_CalendarView (template-stable)
           -> TimeView#PART_TimeView (template-stable)
  -> DatePicker (control theme, DatePickerTheme.axaml)
  -> DualMonthRangeDatePickerPresenter (presenter control theme, DualMonthRangeDatePickerPresenterTheme.axaml)
     -> DockPanel#RootLayout (template-stable)
        -> PixelAlignedBorder#ButtonsFrame (template-stable)
           -> Panel#ButtonsLayout (template-stable)
              -> Button#PART_NowButton (template-stable)
              -> Button#PART_TodayButton (template-stable)
              -> Button#PART_ConfirmButton (template-stable)
        -> StackPanel (template-stable)
           -> DualMonthRangeCalendar#PART_CalendarView (template-stable)
  -> RangeDatePickerPresenter (presenter control theme, RangeDatePickerPresenterTheme.axaml)
  -> TimedRangeDatePickerPresenter (presenter control theme, TimedRangeDatePickerPresenterTheme.axaml)
     -> DockPanel#RootLayout (template-stable)
        -> PixelAlignedBorder#ButtonsFrame (template-stable)
           -> Panel#ButtonsLayout (template-stable)
              -> Button#PART_NowButton (template-stable)
              -> Button#PART_TodayButton (template-stable)
              -> Button#PART_ConfirmButton (template-stable)
        -> StackPanel (template-stable)
           -> RangeCalendar#PART_CalendarView (template-stable)
           -> TimeView#PART_TimeView (template-stable)
```

### 协作节点

| 节点 | 类型 | 来源 | 生命周期 owner | 影响的 public API | 稳定性 | Agent 使用边界 |
| --- | --- | --- | --- | --- | --- | --- |
| `DatePicker` | public control | `源文档 + public API` | 用户代码 / 控件宿主 | public API | public | 用户可直接使用 public 控件；可作为示例和 API 入口。 |
| `DatePickerPresenter` | presenter control theme | `DatePickerPresenterTheme.axaml` | DatePicker | `BorderThickness`, `ClockIdentifier`, `IsButtonsPanelVisible`, `IsMotionEnabled`, `IsTimeSelectionVisible`, `PickerMode` | internal-observable | 用于理解结构和状态流，不应指导用户代码直接依赖。 |
| `RootLayout` | template node (DockPanel) | `DatePickerPresenterTheme.axaml` | DatePickerPresenter | `BorderThickness`, `ClockIdentifier`, `IsButtonsPanelVisible`, `IsMotionEnabled`, `IsTimeSelectionVisible`, `PickerMode` | template-stable | 用于主题维护；变更需同步主题、实现和 LLMS。 |
| `ButtonsFrame` | template node (PixelAlignedBorder) | `DatePickerPresenterTheme.axaml` | DatePickerPresenter | `BorderThickness`, `IsButtonsPanelVisible` | template-stable | 用于主题维护；变更需同步主题、实现和 LLMS。 |
| `ButtonsLayout` | template node (Panel) | `DatePickerPresenterTheme.axaml` | DatePickerPresenter | 主题状态 / visual state | template-stable | 用于主题维护；变更需同步主题、实现和 LLMS。 |
| `PART_NowButton` | template node (Button) | `DatePickerPresenterTheme.axaml` | DatePickerPresenter | 主题状态 / visual state | template-stable | 用于主题维护；变更需同步主题、实现和 LLMS。 |
| `PART_TodayButton` | template node (Button) | `DatePickerPresenterTheme.axaml` | DatePickerPresenter | 主题状态 / visual state | template-stable | 用于主题维护；变更需同步主题、实现和 LLMS。 |
| `PART_ConfirmButton` | template node (Button) | `DatePickerPresenterTheme.axaml` | DatePickerPresenter | 主题状态 / visual state | template-stable | 用于主题维护；变更需同步主题、实现和 LLMS。 |
| `StackPanel` | template node (StackPanel) | `DatePickerPresenterTheme.axaml` | DatePickerPresenter | `ClockIdentifier`, `IsMotionEnabled`, `IsTimeSelectionVisible`, `PickerMode` | template-stable | 用于主题维护；变更需同步主题、实现和 LLMS。 |
| `PART_CalendarView` | template node (Calendar) | `DatePickerPresenterTheme.axaml` | DatePickerPresenter | `IsMotionEnabled`, `PickerMode` | template-stable | 用于主题维护；变更需同步主题、实现和 LLMS。 |
| `PART_TimeView` | template node (TimeView) | `DatePickerPresenterTheme.axaml` | DatePickerPresenter | `ClockIdentifier`, `IsMotionEnabled`, `IsTimeSelectionVisible` | template-stable | 用于主题维护；变更需同步主题、实现和 LLMS。 |
| `DatePicker` | control theme | `DatePickerTheme.axaml` | 用户代码 / 控件宿主 | 主题状态 / visual state | public | 用户可直接使用 public 控件；可作为示例和 API 入口。 |
| `DualMonthRangeDatePickerPresenter` | presenter control theme | `DualMonthRangeDatePickerPresenterTheme.axaml` | DatePicker | `BorderThickness`, `IsButtonsPanelVisible`, `IsMotionEnabled`, `IsRangeStartActive`, `PickerMode` | internal-observable | 用于理解结构和状态流，不应指导用户代码直接依赖。 |
| `RootLayout` | template node (DockPanel) | `DualMonthRangeDatePickerPresenterTheme.axaml` | DualMonthRangeDatePickerPresenter | `BorderThickness`, `IsButtonsPanelVisible`, `IsMotionEnabled`, `IsRangeStartActive`, `PickerMode` | template-stable | 用于主题维护；变更需同步主题、实现和 LLMS。 |
| `ButtonsFrame` | template node (PixelAlignedBorder) | `DualMonthRangeDatePickerPresenterTheme.axaml` | DualMonthRangeDatePickerPresenter | `BorderThickness`, `IsButtonsPanelVisible` | template-stable | 用于主题维护；变更需同步主题、实现和 LLMS。 |
| `ButtonsLayout` | template node (Panel) | `DualMonthRangeDatePickerPresenterTheme.axaml` | DualMonthRangeDatePickerPresenter | 主题状态 / visual state | template-stable | 用于主题维护；变更需同步主题、实现和 LLMS。 |
| `PART_NowButton` | template node (Button) | `DualMonthRangeDatePickerPresenterTheme.axaml` | DualMonthRangeDatePickerPresenter | 主题状态 / visual state | template-stable | 用于主题维护；变更需同步主题、实现和 LLMS。 |
| `PART_TodayButton` | template node (Button) | `DualMonthRangeDatePickerPresenterTheme.axaml` | DualMonthRangeDatePickerPresenter | 主题状态 / visual state | template-stable | 用于主题维护；变更需同步主题、实现和 LLMS。 |
| `PART_ConfirmButton` | template node (Button) | `DualMonthRangeDatePickerPresenterTheme.axaml` | DualMonthRangeDatePickerPresenter | 主题状态 / visual state | template-stable | 用于主题维护；变更需同步主题、实现和 LLMS。 |
| `StackPanel` | template node (StackPanel) | `DualMonthRangeDatePickerPresenterTheme.axaml` | DualMonthRangeDatePickerPresenter | `IsMotionEnabled`, `IsRangeStartActive`, `PickerMode` | template-stable | 用于主题维护；变更需同步主题、实现和 LLMS。 |
| `PART_CalendarView` | template node (DualMonthRangeCalendar) | `DualMonthRangeDatePickerPresenterTheme.axaml` | DualMonthRangeDatePickerPresenter | `IsMotionEnabled`, `IsRangeStartActive`, `PickerMode` | template-stable | 用于主题维护；变更需同步主题、实现和 LLMS。 |
| `RangeDatePickerPresenter` | presenter control theme | `RangeDatePickerPresenterTheme.axaml` | DatePicker | 主题状态 / visual state | internal-observable | 用于理解结构和状态流，不应指导用户代码直接依赖。 |
| `TimedRangeDatePickerPresenter` | presenter control theme | `TimedRangeDatePickerPresenterTheme.axaml` | DatePicker | `BorderThickness`, `ClockIdentifier`, `IsButtonsPanelVisible`, `IsMotionEnabled`, `IsRangeStartActive`, `IsTimeSelectionVisible` | internal-observable | 用于理解结构和状态流，不应指导用户代码直接依赖。 |
| `RootLayout` | template node (DockPanel) | `TimedRangeDatePickerPresenterTheme.axaml` | TimedRangeDatePickerPresenter | `BorderThickness`, `ClockIdentifier`, `IsButtonsPanelVisible`, `IsMotionEnabled`, `IsRangeStartActive`, `IsTimeSelectionVisible` | template-stable | 用于主题维护；变更需同步主题、实现和 LLMS。 |
| `ButtonsFrame` | template node (PixelAlignedBorder) | `TimedRangeDatePickerPresenterTheme.axaml` | TimedRangeDatePickerPresenter | `BorderThickness`, `IsButtonsPanelVisible` | template-stable | 用于主题维护；变更需同步主题、实现和 LLMS。 |
| `ButtonsLayout` | template node (Panel) | `TimedRangeDatePickerPresenterTheme.axaml` | TimedRangeDatePickerPresenter | 主题状态 / visual state | template-stable | 用于主题维护；变更需同步主题、实现和 LLMS。 |
| `PART_NowButton` | template node (Button) | `TimedRangeDatePickerPresenterTheme.axaml` | TimedRangeDatePickerPresenter | 主题状态 / visual state | template-stable | 用于主题维护；变更需同步主题、实现和 LLMS。 |
| `PART_TodayButton` | template node (Button) | `TimedRangeDatePickerPresenterTheme.axaml` | TimedRangeDatePickerPresenter | 主题状态 / visual state | template-stable | 用于主题维护；变更需同步主题、实现和 LLMS。 |
| `PART_ConfirmButton` | template node (Button) | `TimedRangeDatePickerPresenterTheme.axaml` | TimedRangeDatePickerPresenter | 主题状态 / visual state | template-stable | 用于主题维护；变更需同步主题、实现和 LLMS。 |
| `StackPanel` | template node (StackPanel) | `TimedRangeDatePickerPresenterTheme.axaml` | TimedRangeDatePickerPresenter | `ClockIdentifier`, `IsMotionEnabled`, `IsRangeStartActive`, `IsTimeSelectionVisible`, `PickerMode` | template-stable | 用于主题维护；变更需同步主题、实现和 LLMS。 |
| `PART_CalendarView` | template node (RangeCalendar) | `TimedRangeDatePickerPresenterTheme.axaml` | TimedRangeDatePickerPresenter | `IsMotionEnabled`, `IsRangeStartActive`, `PickerMode` | template-stable | 用于主题维护；变更需同步主题、实现和 LLMS。 |
| `PART_TimeView` | template node (TimeView) | `TimedRangeDatePickerPresenterTheme.axaml` | TimedRangeDatePickerPresenter | `ClockIdentifier`, `IsMotionEnabled`, `IsTimeSelectionVisible` | template-stable | 用于主题维护；变更需同步主题、实现和 LLMS。 |

## Template Parts

| 契约组 | 代表成员 | 维护含义 |
| --- | --- | --- |
| 内容与数据 | `HeaderBackground` | 定义控件展示内容、输入数据、模板或业务对象入口。 |
| 选择与集合 | `PickerMode`、`RangeEndSelectedDate`、`RangeStartSelectedDate`、`SelectedDateTime` | 维护提交值、范围端点和选择颗粒度；`SelectedDateTime`、`RangeStartSelectedDate`、`RangeEndSelectedDate` 默认 `TwoWay` 绑定并启用 Avalonia data validation。 |
| 日期边界 | `MinDate`、`MaxDate` | 以包含边界限制可选 picker unit 和面板导航范围；默认值均为 `null`，表示对应方向无边界。 |
| 弹层显示游标 | `PickerDisplayDate`；内部 `Calendar.DisplayDate`、`DisplayDateStart`、`DisplayDateEnd` | 维护弹出面板打开时显示到哪个日期区域，不代表已选值。 |
| 交互与状态 | `IsFloatingArrowPosition`、`IsHorizontalFlipped`、`IsNeedConfirm`、`IsShowNow`、`IsShowTime`、`IsTodayHighlighted` | 表达用户可观察状态、可用性、清除、加载或反馈语义。 |
| 视觉与布局 | `RangePickerIndicatorOffsetEnd`、`RangePickerIndicatorOffsetStart` | 影响尺寸、位置、颜色、形状、密度和模板视觉变量。 |
| 其他稳定入口 | `ClockIdentifier`、`DefaultDateTime`、`Format` | 保留为 public surface，变更前需确认 Gallery 和用户 XAML 依赖。 |

## Pseudo Classes

| 状态反馈 | public API、内部状态和伪类如何形成用户可感知反馈。 | selection/checked/active、visual option。 |
| 主题语义 | ControlTheme、SharedToken、组件 Token 和模板绑定如何表达视觉。 | DatePicker Token + ControlTheme。 |

## State Flow

DatePicker 的状态流按以下路径收敛：

```text
Public API / inherited command / item source / user input
  -> 控件实例状态
  -> effective state / pseudo-class / template property
  -> ControlTheme selector / presenter / renderer
  -> Gallery 可观察行为
```

状态维护规则：

- Disabled 或不可交互状态优先屏蔽 pointer、keyboard、motion 和提交类反馈。
- selection/checked/active、visual option 状态由控件实例或明确的数据 owner 推导，不能在 template part 之间双向竞争。
- `PickerMode` 决定选择颗粒度和初始面板：`Date`、`Week` 使用月视图，`Month`、`Quarter` 使用年视图，`Year` 使用十年视图。目标颗粒度不能继续降级到更细面板。
- `SelectedDateTime`、`DefaultDateTime` 和 `PickerDisplayDate` 必须保持语义分离：`SelectedDateTime` 是已提交值，`DefaultDateTime` 是默认选中值/reset 值，`PickerDisplayDate` 只作为弹出面板打开时的显示锚点。
- `SelectedDateTime` 是单值 DatePicker 的受控 Form 值入口，默认 `BindingMode.TwoWay`，并通过 Avalonia `DataValidationErrors` 参与原生数据校验。
- `MinDate` 和 `MaxDate` 是包含式 picker unit 边界。两者先忽略时间部分，再按当前 `PickerMode` 归一化；`null` 表示对应方向不受限制。
- 当归一化后的 `MinDate` 晚于 `MaxDate` 时，有效范围收敛为 `MinDate` 所在的一个 picker unit，但控件不得修改或回写调用方设置的原始属性值。
- 外部受控值越界时，`SelectedDateTime`、`RangeStartSelectedDate` 和 `RangeEndSelectedDate` 保持不变，输入框继续显示外部值；Calendar 不标记越界值为选中，确认操作不可提交该值。用户选择有效日期后，才按既有 TwoWay 契约更新受控值。
- 设置 `PickerDisplayDate` 后不得写入 `SelectedDateTime`，不得改变输入框文本、Form value 或清除按钮状态；当已有已选值时，弹出面板仍优先围绕已选值展示。
- DatePicker / RangeDatePicker 输入壳体必须把 `DataValidationErrors` 同步转发到外层 AddOn 和内部文本框；range indicator 等附属视觉读取 effective status，native error 优先于手动 warning/error 状态。
- `PickerMode=Week` 的月视图是带周序号列的 8 列 week panel，不是普通日期面板的 7 个日期按钮逐个选中；选中视觉和 hover 视觉都必须按整周连续行渲染，不能退回单个日期按钮的普通 pointerover 背景。
- 非 `Date` 颗粒度仍使用 `DateTime?` 保存提交值：`Week` 保存 ISO 周起始日，`Month` 保存当月 1 日，`Quarter` 保存季度首月 1 日，`Year` 保存当年 1 月 1 日。
- `IsShowTime` 只在 `PickerMode=Date` 时形成有效时间选择；其他颗粒度忽略时间面板和时间拼接。
- 范围选择的 committed 状态和 hover preview 状态必须分开：`:selected`、`:range-start`、`:range-end`、`:range-middle` 只来自真实端点；hover 只写入 `:range-preview-start`、`:range-preview-end`、`:range-preview-middle`，其中 preview start/end 在视觉上按临时端点显示，但不能污染真实提交状态。
- 模板重套用时必须把 public API 对应状态回放到新的 part、伪类和主题变量。
- 集合、弹层、异步、动效或窗口相关状态必须能处理 reset、close、cancel、detach 和 owner 释放。

## Theme and Token Boundaries

DatePicker 的视觉模型由控件模板、ControlTheme、SharedToken 和必要的组件 Token 共同构成。

| 主题文件 | 职责 |
| --- | --- |
| `CalendarButtonTheme.axaml` | 定义局部操作入口、按钮或 handle 的状态视觉。 |
| `CalendarDayButtonTheme.axaml` | 定义局部操作入口、按钮或 handle 的状态视觉。 |
| `CalendarItemTheme.axaml` | 定义集合项、容器项或局部单元的状态视觉。 |
| `CalendarTheme.axaml` | 提供控件模板、selector、资源绑定和状态视觉。 |
| `DualMonthCalendarItemTheme.axaml` | 定义集合项、容器项或局部单元的状态视觉。 |
| `DualMonthRangeCalendarTheme.axaml` | 提供控件模板、selector、资源绑定和状态视觉。 |
| `RangeCalendarTheme.axaml` | 提供控件模板、selector、资源绑定和状态视觉。 |
| `DatePickerPresenterTheme.axaml` | 定义布局、内容承载或框架节点视觉。 |
| `DatePickerTheme.axaml` | 提供控件模板、selector、资源绑定和状态视觉。 |
| `DatePickerThemes.axaml` | 聚合控件家族主题资源，保证包级引入顺序稳定。 |
| `DualMonthRangeDatePickerPresenterTheme.axaml` | 定义布局、内容承载或框架节点视觉。 |
| `RangeDatePickerPresenterTheme.axaml` | 定义布局、内容承载或框架节点视觉。 |
| `RangeDatePickerTheme.axaml` | 提供控件模板、selector、资源绑定和状态视觉。 |
| `TimedRangeDatePickerPresenterTheme.axaml` | 定义布局、内容承载或框架节点视觉。 |

DatePicker 使用 `DatePickerToken` 作为组件 Token scope。Token 只表达组件视觉语义，不承载 selection/checked/active、visual option 运行时状态。

主题维护规则：

- 不删除或重命名已经稳定的 ControlTheme key、template part、伪类和资源 key。
- 不把可由 AXAML 表达的模板状态迁移为 C# 动态创建视觉。
- 不把 hover、pressed、selected、expanded、loading、filter、popup open 等运行时状态写入 Token。
- 日期边界外的 Calendar cell 保持可见并使用 disabled 状态视觉，不通过隐藏 cell 表达不可选择状态。
- Browser 或平台特化主题必须保持同一 API 的语义一致。

Token 边界：

DatePicker Token 只表达组件级视觉变量，例如尺寸、间距、颜色、圆角、阴影、图标尺寸和弹层边界。Token 不承载运行时选择、展开、加载、错误、上传任务、过滤条件或业务状态。

当前 Token scope：

- `DatePickerToken`，scope id 为 `DatePicker`，源码位于 `src/AtomUI.Desktop.Controls/DatePicker/DatePickerToken.cs`。

## Customization Boundaries

维护 DatePicker 时必须保持以下不变量：

- 不擅自新增、删除、重命名或改变 public/protected API、Avalonia 属性、事件和默认值。
- 不破坏 template part、伪类、ControlTheme key、Token 名称和资源 key。
- 不改变 Gallery 已展示的 XAML 用法、默认外观、交互顺序和状态优先级。
- Template part 重新应用、集合替换、弹层关闭、窗口失活和控件 detach 时必须释放旧订阅和资源宿主。
- 不通过隐藏延迟、强制刷新或吞异常掩盖状态同步问题。
- 不引入运行时反射扫描作为 API、Token 或数据路径发现机制。
- 文档只描述当前稳定设计；历史变化记录在 `changelog.md`。

维护不变量：

维护 DatePicker 时不得破坏：

- Public API、默认值、事件顺序和 Gallery 可观察行为。
- Template part 名称、ControlTheme key、伪类和资源 key。
- 旧 template part、事件订阅、Popup/Flyout/Window host 和 collection view 的释放路径。
- Light/Dark、Browser/Desktop 和不同 SizeType 下的主题一致性。
- 控件文档、源码 public surface、Token 类型或生成数据与源码契约的一致性。
- `MinDate` / `MaxDate` 的包含边界、PickerMode 归一化、越界受控值不回写以及可见 disabled cell 语义。
