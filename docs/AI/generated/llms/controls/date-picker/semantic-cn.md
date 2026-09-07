# DatePicker 语义结构

> 生成产物：由源文档生成，不要手工编辑。修改内容请回到控件文档、源码 public surface、Token 类型或生成数据、Gallery ShowCase 或源码结构。

## Semantic Parts

`DatePicker` 公开 12 个 Semantic Part，`RangeDatePicker` 公开 13 个 Semantic Part（后者多出范围双输入框的
`secondaryInput`）。声明分别位于 `DatePicker.SemanticParts.cs` 与 `RangeDatePicker.SemanticParts.cs` partial 文件。

触发区部件的 marker 分布：

- 单值 `DatePicker` 的宿主模板是共享 `InfoPickerInputTheme.axaml`（`DatePickerTheme.axaml` 纯 `BasedOn` 继承，
  无自有模板）；`RangeDatePicker` 的宿主模板是自有 `RangeDatePickerTheme.axaml`。两个宿主模板按同一模式标注
  `semantic-scope-input`（AddOnDecoratedBox 节点）、`semantic-input`（`PART_InfoInputBox`）、`semantic-suffix`
  （右侧内容 StackPanel）、`semantic-scope-handle`（PickerClearUpButton 节点）与 `semantic-popup-root`
  （`PART_Popup` 内的 `ArrowDecoratedBox` / `DualMonthArrowDecoratedBox`）。
- `prefix` 借用共享 `AddOnDecoratedBoxTheme` 的 `.semantic-scope-prefix` scope 锚点路由到宿主模板新增的
  `AddOnContentPresenter` 投影节点（与 Select 家族同构；投影节点以
  `CompiledBinding $parent[atom:InfoPickerInput].ContentLeftAddOn` 呈现公共 API 值）。
- `clear` 的物理按钮在共享 `PickerClearUpButtonTheme.axaml` 模板内（`PART_ClearButton`），声明
  `CrossNestedOwners=true`，生成器沿 PickerClearUpButton 主题链校验；该共享主题同时服务于未来 TimePicker
  家族的同名 Part（inert marker，未声明契约的控件零影响）。
- `secondaryInput` 仅 `RangeDatePicker` 声明，标注自有模板的 `PART_SecondaryInfoInputBox`。

弹层部件的 marker 分布（弹层内容全部由 owner `CreatePickerPresenter` 在首次打开时运行时创建）：

- `popup.container` / `popup.footer` 标注 presenter 主题模板节点：`popup.container` 在模板根 `DockPanel #RootLayout`，
  `popup.footer` 在 `PixelAlignedBorder #ButtonsFrame`；三个带模板的 presenter 主题
  （`DatePickerPresenterTheme.axaml`、`DualMonthRangeDatePickerPresenterTheme.axaml`、
  `TimedRangeDatePickerPresenterTheme.axaml`）均标注。`RangeDatePickerPresenterTheme.axaml` 纯继承无模板。
- `popup.header` / `popup.body` / `popup.content` 标注 `CalendarItemTheme.axaml`（单月：
  `PART_HeaderFrame` / `PART_MonthViewLayout` / `PART_MonthView`）与 `DualMonthCalendarItemTheme.axaml`
  （双月：同名节点加 `PART_SecondaryMonthView`）。
- `popup.cell` 为运行时注入：`CalendarDayButton` 构造函数追加生成 selector class 常量（共享 CalendarView
  基础设施，两个 owner 的常量值一致），覆盖月网格重建与容器回收。
- `popup.*` 全部声明 `RuntimeCreated=true`（presenter 子树在运行时组装，生成器豁免宿主模板 marker 校验，
  由控件行为测试兜底），其中 `popup.root` 为宿主模板静态节点、`RuntimeCreated=false`。

### `root`

| 字段 | 值 |
| --- | --- |
| Owner | `DatePicker` / `RangeDatePicker` |
| Part | `root` |
| Selector | owner 本身 |
| SelectorRoute | 不适用 |
| Style Type | 不适用（root 不生成 Style） |
| ContractType | `DatePicker` / `RangeDatePicker` |
| Cardinality | `Single` |
| Customization | `Root` |
| CrossVisualRoot | `false` |
| RuntimeCreated | `false` |
| AtomUI 节点 | DatePicker / RangeDatePicker owner |
| 职责 | owner 是日期值、格式化、弹层状态、Form 值与验证状态的组织边界；owner 级 `BorderBrush` 经控件中继为输入框边框颜色（root 级定制入口，未设置时恢复共享状态机）。 |
| 相关 API | 全部 DatePicker / RangeDatePicker public API |
| 相关 Token | DatePickerToken、SharedToken |
| 稳定性 | stable since 6.0 |

### `prefix`

| 字段 | 值 |
| --- | --- |
| Owner | `DatePicker` / `RangeDatePicker` |
| Part | `prefix` |
| Selector | `.semantic-prefix` |
| SelectorRoute | `/template/ .semantic-scope-input /template/ .semantic-scope-prefix > .semantic-prefix` |
| Style Type | `DatePickerPrefixStyle` / `RangeDatePickerPrefixStyle` |
| ContractType | `ContentPresenter` |
| Cardinality | `Single` |
| Customization | `Selector` |
| CrossVisualRoot | `false` |
| RuntimeCreated | `false` |
| AtomUI 节点 | 宿主模板中承载 `ContentLeftAddOn` 的 `AddOnContentPresenter` 投影节点 |
| 职责 | 输入区内容前缀区域，承载 `ContentLeftAddOn` 用户内容，在内容框内联展示。 |
| 相关 API | `ContentLeftAddOn`、`ContentLeftAddOnTemplate` |
| 相关 Token | SharedToken |
| 稳定性 | stable since 6.0 |

### `input`

| 字段 | 值 |
| --- | --- |
| Owner | `DatePicker` / `RangeDatePicker` |
| Part | `input` |
| Selector | `.semantic-input` |
| SelectorRoute | `/template/ .semantic-input` |
| Style Type | `DatePickerInputStyle` / `RangeDatePickerInputStyle` |
| ContractType | `TextBox` |
| Cardinality | `Single` |
| Customization | `Selector` |
| CrossVisualRoot | `false` |
| RuntimeCreated | `false` |
| AtomUI 节点 | 宿主模板的 `InfoPickerTextBox #PART_InfoInputBox`（起始端输入框） |
| 职责 | 日期文本输入框，承载格式化显示值、占位符与只读/校验状态。 |
| 相关 API | `Text`、`PlaceholderText`、`Format`、`IsReadOnly`、`PreferredInputWidth` |
| 相关 Token | SharedToken |
| 稳定性 | stable since 6.0 |

### `secondaryInput`（仅 RangeDatePicker）

| 字段 | 值 |
| --- | --- |
| Owner | `RangeDatePicker` |
| Part | `secondaryInput` |
| Selector | `.semantic-secondary-input` |
| SelectorRoute | `/template/ .semantic-secondary-input` |
| Style Type | `RangeDatePickerSecondaryInputStyle` |
| ContractType | `TextBox` |
| Cardinality | `Single` |
| Customization | `Selector` |
| CrossVisualRoot | `false` |
| RuntimeCreated | `false` |
| AtomUI 节点 | `RangeDatePickerTheme.axaml` 的 `InfoPickerTextBox #PART_SecondaryInfoInputBox`（结束端输入框） |
| 职责 | 范围选择的结束端日期文本输入框，与 `input` 共用格式与宽度基线。 |
| 相关 API | `SecondaryText`、`SecondaryPlaceholderText` |
| 相关 Token | SharedToken |
| 稳定性 | stable since 6.0 |

### `suffix`

| 字段 | 值 |
| --- | --- |
| Owner | `DatePicker` / `RangeDatePicker` |
| Part | `suffix` |
| Selector | `.semantic-suffix` |
| SelectorRoute | `/template/ .semantic-scope-input /template/ .semantic-scope-suffix > .semantic-suffix` |
| Style Type | `DatePickerSuffixStyle` / `RangeDatePickerSuffixStyle` |
| ContractType | `StackPanel` |
| Cardinality | `Single` |
| Customization | `Selector` |
| CrossVisualRoot | `false` |
| RuntimeCreated | `false` |
| AtomUI 节点 | 宿主模板中投影给 `ContentRightAddOn` 的水平 StackPanel（含清除按钮与 `PART_ContentRightAddOnPresenter`） |
| 职责 | 输入区后缀区域，承载清除按钮、Form 反馈与用户后缀内容。 |
| 相关 API | `ContentRightAddOn`、`ContentRightAddOnTemplate` |
| 相关 Token | SharedToken |
| 稳定性 | stable since 6.0 |

### `clear`

| 字段 | 值 |
| --- | --- |
| Owner | `DatePicker` / `RangeDatePicker` |
| Part | `clear` |
| Selector | `.semantic-clear` |
| SelectorRoute | `>> .semantic-scope-handle /template/ .semantic-clear` |
| Style Type | `DatePickerClearStyle` / `RangeDatePickerClearStyle` |
| ContractType | `IconButton` |
| Cardinality | `Single` |
| Customization | `Selector` |
| CrossNestedOwners | `true` |
| CrossVisualRoot | `false` |
| RuntimeCreated | `false` |
| AtomUI 节点 | 共享 `PickerClearUpButtonTheme.axaml` 模板内的 `InputClearIconButton #PART_ClearButton` |
| 职责 | 后缀区清除按钮，进入清除模式（hover / focus）时渲染。 |
| 相关 API | `ShowClearButtonPredicate`（DatePicker）/ 范围清除行为（RangeDatePicker） |
| 相关 Token | SharedToken |
| 稳定性 | stable since 6.0 |

### `popup.root`

| 字段 | 值 |
| --- | --- |
| Owner | `DatePicker` / `RangeDatePicker` |
| Part | `popup.root` |
| Selector | `.semantic-popup-root` |
| SelectorRoute | `/template/ .semantic-popup-root` |
| Style Type | `DatePickerPopupRootStyle` / `RangeDatePickerPopupRootStyle` |
| ContractType | `ArrowDecoratedBox` |
| Cardinality | `Single` |
| Customization | `Selector` |
| CrossVisualRoot | `true` |
| RuntimeCreated | `false` |
| AtomUI 节点 | 单选：`InfoPickerInputTheme.axaml` 中 `PART_Popup` 的 `ArrowDecoratedBox`；范围：`RangeDatePickerTheme.axaml` 中的 `DualMonthArrowDecoratedBox` |
| 职责 | 弹层内容根视觉盒子，承载背景、边框、阴影与浮动箭头；`BorderThickness` 定制为非零时盒子进入 `:bordered` 状态，内置主题自动隐藏浮动箭头（内置视觉不支持箭头与边框的融合呈现）。 |
| 相关 API | `IsArrowVisible`（经 `IsArrowVisibleEffective`）、`ArrowPosition`、`IsMotionEnabled` |
| 相关 Token | PopupToken |
| 稳定性 | stable since 6.0 |

### `popup.container`

| 字段 | 值 |
| --- | --- |
| Owner | `DatePicker` / `RangeDatePicker` |
| Part | `popup.container` |
| Selector | `.semantic-popup-container` |
| SelectorRoute | `/template/ .semantic-popup-root >> .semantic-popup-container` |
| Style Type | `DatePickerPopupContainerStyle` / `RangeDatePickerPopupContainerStyle` |
| ContractType | `DockPanel` |
| Cardinality | `Single` |
| Customization | `Selector` |
| CrossVisualRoot | `true` |
| RuntimeCreated | `true` |
| AtomUI 节点 | presenter 主题模板根 `DockPanel #RootLayout`（三个 presenter 主题模板均标注） |
| 职责 | 日历面板内容容器，组织主体区与底部按钮区的布局。 |
| 相关 API | 无（面板内容布局容器） |
| 相关 Token | DatePickerToken |
| 稳定性 | stable since 6.0 |

### `popup.header`

| 字段 | 值 |
| --- | --- |
| Owner | `DatePicker` / `RangeDatePicker` |
| Part | `popup.header` |
| Selector | `.semantic-popup-header` |
| SelectorRoute | `/template/ .semantic-popup-root >> .semantic-popup-header` |
| Style Type | `DatePickerPopupHeaderStyle` / `RangeDatePickerPopupHeaderStyle` |
| ContractType | `Border` |
| Cardinality | `Single` |
| Customization | `Selector` |
| CrossVisualRoot | `true` |
| RuntimeCreated | `true` |
| AtomUI 节点 | `CalendarItemTheme.axaml` / `DualMonthCalendarItemTheme.axaml` 的 `PixelAlignedBorder #PART_HeaderFrame`（双月布局内含左右两月导航按钮组） |
| 职责 | 日历年月导航头部，承载年月标题与前进/后退/翻年按钮。 |
| 相关 API | 无（导航按钮交互由 CalendarView 内部承担） |
| 相关 Token | DatePickerToken |
| 稳定性 | stable since 6.0 |

### `popup.body`

| 字段 | 值 |
| --- | --- |
| Owner | `DatePicker` / `RangeDatePicker` |
| Part | `popup.body` |
| Selector | `.semantic-popup-body` |
| SelectorRoute | `/template/ .semantic-popup-root >> .semantic-popup-body` |
| Style Type | `DatePickerPopupBodyStyle` / `RangeDatePickerPopupBodyStyle` |
| ContractType | `UniformGrid` |
| Cardinality | `Single` |
| Customization | `Selector` |
| CrossVisualRoot | `true` |
| RuntimeCreated | `true` |
| AtomUI 节点 | `CalendarItemTheme.axaml` 的 `UniformGrid #PART_MonthViewLayout`；双月为 `DualMonthCalendarItemTheme.axaml` 的同名节点（Columns=2，包住两张月表） |
| 职责 | 日期面板表格容器，按月视图/年视图模式承载表格布局。 |
| 相关 API | 无 |
| 相关 Token | DatePickerToken |
| 稳定性 | stable since 6.0 |

### `popup.content`

| 字段 | 值 |
| --- | --- |
| Owner | `DatePicker` / `RangeDatePicker` |
| Part | `popup.content` |
| Selector | `.semantic-popup-content` |
| SelectorRoute | `/template/ .semantic-popup-root >> .semantic-popup-content` |
| Style Type | `DatePickerPopupContentStyle` / `RangeDatePickerPopupContentStyle` |
| ContractType | `Grid` |
| Cardinality | `DatePicker`: `Single`；`RangeDatePicker`: `Multiple`（双月两张表，带时间单月一张） |
| Customization | `Selector` |
| CrossVisualRoot | `true` |
| RuntimeCreated | `true` |
| AtomUI 节点 | 单月：`CalendarItemTheme.axaml` 的 `Grid #PART_MonthView`；双月：`DualMonthCalendarItemTheme.axaml` 的 `PART_MonthView` 与 `PART_SecondaryMonthView` |
| 职责 | 单个月份的 7×7 日期表格本体（含周序号列变体），承载日期格子与周头标题。 |
| 相关 API | 无（随 `popup.body` 呈现） |
| 相关 Token | DatePickerToken |
| 稳定性 | stable since 6.0 |

### `popup.cell`

| 字段 | 值 |
| --- | --- |
| Owner | `DatePicker` / `RangeDatePicker` |
| Part | `popup.cell` |
| Selector | `.semantic-cell` |
| SelectorRoute | `/template/ .semantic-popup-root >> .semantic-cell` |
| Style Type | `DatePickerPopupCellStyle` / `RangeDatePickerPopupCellStyle` |
| ContractType | `Button` |
| Cardinality | `Multiple` |
| Customization | `Selector` |
| CrossVisualRoot | `true` |
| RuntimeCreated | `true` |
| AtomUI 节点 | 月网格运行时创建的 `CalendarDayButton`（`CalendarItem.PopulateMonthViewGrid` 与 `DualMonthCalendarItem.PopulateMonthViewsGrid` 创建路径，构造时注入 marker） |
| 职责 | 日期格子按钮，承载可选日期、选中/范围/今天/禁用等状态视觉（伪类见 overview）。 |
| 相关 API | 无（随 `popup.content` 呈现） |
| 相关 Token | DatePickerToken |
| 稳定性 | stable since 6.0 |

### `popup.footer`

| 字段 | 值 |
| --- | --- |
| Owner | `DatePicker` / `RangeDatePicker` |
| Part | `popup.footer` |
| Selector | `.semantic-popup-footer` |
| SelectorRoute | `/template/ .semantic-popup-root >> .semantic-popup-footer` |
| Style Type | `DatePickerPopupFooterStyle` / `RangeDatePickerPopupFooterStyle` |
| ContractType | `PixelAlignedBorder` |
| Cardinality | `Single` |
| Customization | `Selector` |
| CrossVisualRoot | `true` |
| RuntimeCreated | `true` |
| AtomUI 节点 | presenter 主题模板的 `PixelAlignedBorder #ButtonsFrame`（内含 `PART_NowButton` / `PART_TodayButton` / `PART_ConfirmButton`） |
| 职责 | 面板底部操作区，承载此刻/今天/确认按钮。 |
| 相关 API | `IsNeedConfirm`、`IsShowNow` |
| 相关 Token | DatePickerToken |
| 稳定性 | stable since 6.0 |

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
  -> InfoPickerTextBox (control theme, InfoPickerTextBoxTheme.axaml)
  -> PickerClearUpButton (control theme, PickerClearUpButtonTheme.axaml)
     -> Panel (template-stable)
        -> InputClearIconButton#PART_ClearButton (template-stable)
        -> StackPanel#IconLayout (template-stable)
           -> IconPresenter#PART_InfoIconPresenter (template-stable)
           -> ContentPresenter#FormFeedBack (internal-observable)
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
| `InfoPickerTextBox` | control theme | `InfoPickerTextBoxTheme.axaml` | DatePicker | 主题状态 / visual state | internal-observable | 用于理解结构和状态流，不应指导用户代码直接依赖。 |
| `PickerClearUpButton` | control theme | `PickerClearUpButtonTheme.axaml` | DatePicker | `FormFeedback`, `Icon`, `IsFormFeedbackVisible`, `IsInClearMode` | internal-observable | 用于理解结构和状态流，不应指导用户代码直接依赖。 |
| `Panel` | template node (Panel) | `PickerClearUpButtonTheme.axaml` | PickerClearUpButton | `FormFeedback`, `Icon`, `IsFormFeedbackVisible`, `IsInClearMode` | template-stable | 用于主题维护；变更需同步主题、实现和 LLMS。 |
| `PART_ClearButton` | template node (InputClearIconButton) | `PickerClearUpButtonTheme.axaml` | PickerClearUpButton | `IsInClearMode` | template-stable | 用于主题维护；变更需同步主题、实现和 LLMS。 |

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
| 主题语义 | ControlTheme、SharedToken、控件 Token 和模板绑定如何表达视觉。 | DatePicker Token + ControlTheme。 |

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
- DatePicker / RangeDatePicker 必须把 `DataValidationErrors`、FormStatus 和显式 Status 投射到 shared `InputControlFrame`；range indicator 等附属视觉读取 `EffectiveStatus`，native error 优先于 Form/显式 warning/error 状态。Calendar 和 picker panel 只拥有日期选择、范围预览和面板交互状态。
- `PickerMode=Week` 的月视图是带周序号列的 8 列 week panel，不是普通日期面板的 7 个日期按钮逐个选中；选中视觉和 hover 视觉都必须按整周连续行渲染，不能退回单个日期按钮的普通 pointerover 背景。
- 非 `Date` 颗粒度仍使用 `DateTime?` 保存提交值：`Week` 保存 ISO 周起始日，`Month` 保存当月 1 日，`Quarter` 保存季度首月 1 日，`Year` 保存当年 1 月 1 日。
- `IsShowTime` 只在 `PickerMode=Date` 时形成有效时间选择；其他颗粒度忽略时间面板和时间拼接。
- 范围选择的 committed 状态和 hover preview 状态必须分开：`:selected`、`:range-start`、`:range-end`、`:range-middle` 只来自真实端点；hover 只写入 `:range-preview-start`、`:range-preview-end`、`:range-preview-middle`，其中 preview start/end 在视觉上按临时端点显示，但不能污染真实提交状态。
- 模板重套用时必须把 public API 对应状态回放到新的 part、伪类和主题变量。
- 集合、弹层、异步、动效或窗口相关状态必须能处理 reset、close、cancel、detach 和 owner 释放。

## Theme and Token Boundaries

DatePicker 的视觉模型由 `InputControlFrame` 输入表面、InfoPicker 输入子控件、控件模板、ControlTheme、SharedToken 和必要的控件 Token 共同构成。输入表面状态由 shared frame 统一表达，DatePicker 主题只扩展日期、范围和弹层内容。

| 主题文件 | 职责 |
| --- | --- |
| `CalendarButtonTheme.axaml` | 定义局部操作入口、按钮或 handle 的状态视觉。 |
| `CalendarDayButtonTheme.axaml` | 定义局部操作入口、按钮或 handle 的状态视觉。 |
| `CalendarItemTheme.axaml` | 定义集合项、容器项或局部单元的状态视觉。 |
| `CalendarTheme.axaml` | 提供控件模板、selector、资源绑定和状态视觉。 |
| `InfoPickerTextBoxTheme.axaml` | 通过 `StyleVariant=Borderless` 提供内部日期文本输入的无 chrome 布局。 |
| `InputControlFrameTheme.axaml` | 提供输入表面 variant、effective status、focus、disabled、error、warning、CompactSpace 和 motion。 |
| `DualMonthCalendarItemTheme.axaml` | 定义集合项、容器项或局部单元的状态视觉。 |
| `DualMonthRangeCalendarTheme.axaml` | 提供控件模板、selector、资源绑定和状态视觉。 |
| `RangeCalendarTheme.axaml` | 提供控件模板、selector、资源绑定和状态视觉。 |
| `DatePickerPresenterTheme.axaml` | 定义布局、内容承载或框架节点视觉。 |
| `DatePickerTheme.axaml` | 提供控件模板、selector、资源绑定和状态视觉。 |
| `DualMonthRangeDatePickerPresenterTheme.axaml` | 定义布局、内容承载或框架节点视觉。 |
| `RangeDatePickerPresenterTheme.axaml` | 定义布局、内容承载或框架节点视觉。 |
| `RangeDatePickerTheme.axaml` | 提供控件模板、selector、资源绑定和状态视觉。 |
| `TimedRangeDatePickerPresenterTheme.axaml` | 定义布局、内容承载或框架节点视觉。 |

DatePicker 使用 `DatePickerToken` 作为控件 Token scope。Token 只表达组件视觉语义，不承载 selection/checked/active、visual option 运行时状态。

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
- Semantic Part marker 的维护边界：共享 `InfoPickerInputTheme.axaml` 承载单选触发区静态 marker（`semantic-scope-input`、`semantic-prefix`、`semantic-input`、`semantic-suffix`、`semantic-scope-handle`、`semantic-popup-root`）；`RangeDatePickerTheme.axaml` 承载范围触发区同名 marker 与 `semantic-secondary-input`；共享 `PickerClearUpButtonTheme.axaml` 承载 `semantic-clear` marker（`clear` Part 声明 `CrossNestedOwners=true`，生成器沿 PickerClearUpButton 主题链校验）；`DatePickerPresenterTheme.axaml` / `DualMonthRangeDatePickerPresenterTheme.axaml` / `TimedRangeDatePickerPresenterTheme.axaml` 承载 `semantic-popup-container` / `semantic-popup-footer`；`CalendarItemTheme.axaml` / `DualMonthCalendarItemTheme.axaml` 承载 `semantic-popup-header` / `semantic-popup-body` / `semantic-popup-content`（双月含 secondary 月表）。运行时注入点：`CalendarDayButton` 构造函数追加 `popup.cell` 的生成 selector class 常量（CalendarView 为家族内共享基础设施，两个 owner 常量值一致）。marker 随实例创建一次，月网格 rebuild、弹层重开和容器回收路径不得增删；共享主题 marker 对 TimePicker / RangeTimePicker 保持 inert。
