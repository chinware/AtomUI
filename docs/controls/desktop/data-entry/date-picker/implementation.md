# DatePicker 桌面版实现原理

本文档描述 DatePicker 桌面版的内部实现范围、源码职责、状态流、生命周期、资源边界和维护规则。公共设计与 API 契约见 [DatePicker 桌面版架构设计](overview.md)，CalendarView 的系统性优化目标见 [CalendarView 系统性优化设计](calendar-view-system-optimization.md)，变化记录见 [DatePicker Changelog](changelog.md)。涉及组件 Token 的实现应同时阅读 [DatePicker Token 设计](token.md)。

## 1. 实现定位

本文档覆盖 DatePicker 的控件实现、主题接入、状态同步和 Gallery 可见维护边界。具体属性注册、默认值、绘制细节和 AXAML selector 仍应直接阅读源码；本文只记录维护者必须理解的稳定结构和不变量。

## 2. 源码文件结构

主要源码文件：

- `src/AtomUI.Desktop.Controls/DatePicker`：DatePicker 控件家族根目录，代表文件 `DatePicker.cs`、`RangeDatePicker.cs`、`DatePickerPresenter.cs`、`DatePickerFormattingHelper.cs`、`DatePickerToken.cs`、`DualMonthRangeDatePickerPresenter.cs` 等。
- `src/AtomUI.Desktop.Controls/DatePicker/CalendarView`：CalendarView runtime。`State` 保存归一化状态和 action，`Models` 保存纯 panel model，`Rendering` 将 model 应用到 generated buttons，`Infrastructure` 封装 culture 和 pointer tracking。
- `src/AtomUI.Desktop.Controls/DatePicker/Localization`：3 个文件，代表文件 `en_US.cs`、`zh_CN.cs`、`zh_TW.cs`。
- `src/AtomUI.Desktop.Controls/DatePicker/Themes`：19 个文件，代表文件 `CalendarButtonTheme.axaml`、`CalendarButtonTheme.cs`、`CalendarDayButtonTheme.axaml`、`CalendarItemTheme.axaml`、`CalendarItemTheme.cs` 等。

职责边界：

- 控件主文件保留 public/protected API、Avalonia 属性注册、事件和主要生命周期入口。
- Theme 文件负责静态视觉结构、template part、selector 和资源绑定。
- Token 文件只提供组件视觉变量，不保存实例状态。
- Gallery 文件只展示用法、API 表和 Token 表，不作为运行时逻辑 owner。
- CalendarView 的深度重构必须以 [CalendarView 系统性优化设计](calendar-view-system-optimization.md) 中定义的单向状态模型、panel model、renderer 和生命周期规则为边界。

## 3. 核心类职责

- `Calendar`：控件核心或内部协作类型，维护 public surface 与主题可观察行为。
- `CalendarButtonTheme`：ControlTheme 类型入口，连接主题资源和控件类型。
- `CalendarItem`：集合项、节点或容器类型，承载单项状态和模板协作。
- `CalendarItemTheme`：ControlTheme 类型入口，连接主题资源和控件类型。
- `CalendarTheme`：ControlTheme 类型入口，连接主题资源和控件类型。
- `DatePicker`：控件核心或内部协作类型，维护 public surface 与主题可观察行为。
- `DatePickerPresenter`：模板协作类型，承载内容展示、宿主或视觉边界。
- `DatePickerPresenterTheme`：ControlTheme 类型入口，连接主题资源和控件类型。
- `DatePickerToken`：组件 Token scope，负责从全局 token 派生控件语义变量。
- `DualMonthArrowDecoratedBox`：模板协作类型，承载内容展示、宿主或视觉边界。
- `DualMonthCalendarItem`：集合项、节点或容器类型，承载单项状态和模板协作。
- `DualMonthRangeCalendar`：控件核心或内部协作类型，维护 public surface 与主题可观察行为。
- `DualMonthRangeDatePickerPresenter`：模板协作类型，承载内容展示、宿主或视觉边界。
- `RangeCalendar`：控件核心或内部协作类型，维护 public surface 与主题可观察行为。
- `RangeCalendarItem`：集合项、节点或容器类型，承载单项状态和模板协作。
- `RangeDatePicker`：控件核心或内部协作类型，维护 public surface 与主题可观察行为。
- `RangeDatePickerPresenter`：模板协作类型，承载内容展示、宿主或视觉边界。
- `RangeDatePickerPresenterTheme`：ControlTheme 类型入口，连接主题资源和控件类型。
- `TimedRangeDatePickerPresenter`：模板协作类型，承载内容展示、宿主或视觉边界。
- `en_US`：控件核心或内部协作类型，维护 public surface 与主题可观察行为。
- `zh_CN`：控件核心或内部协作类型，维护 public surface 与主题可观察行为。
- `zh_TW`：控件核心或内部协作类型，维护 public surface 与主题可观察行为。

核心协作规则：

- 控件实例是 public API 和运行时状态 owner。
- Template part 是视觉协作对象，生命周期必须受 `OnApplyTemplate` 或模板加载流程管理。
- 数据对象、选项对象、任务对象或节点对象只保存业务数据，不应反向持有不可释放的视觉对象。
- 弹层、窗口、计时器、异步 loader 和全局管理器必须有明确关闭、解绑或释放路径。

## 4. 状态与数据流

DatePicker 的状态流遵循下面路径：

```text
Public API / ItemsSource / Command / Event
  -> 控件实例状态
  -> internal state / effective state / pseudo-class
  -> template part property / AXAML selector
  -> renderer / popup / adorner / Gallery observable behavior
```

源码中的状态入口按以下语义维护：

- 内容与数据：`HeaderBackground`。
- 选择与集合：`PickerMode`、`RangeEndSelectedDate`、`RangeStartSelectedDate`、`SelectedDateTime`。其中 `SelectedDateTime` 是单值 DatePicker 的受控提交值，默认 `TwoWay` 绑定并启用 Avalonia data validation。
- 弹层显示游标：`PickerDisplayDate`，以及 presenter 内部转发到 CalendarView 的 `DisplayDate`、`SelectedMonth`、`SelectedYear`、`LastSelectedDate`。
- 交互与状态：`IsFloatingArrowPosition`、`IsHorizontalFlipped`、`IsNeedConfirm`、`IsShowNow`、`IsShowTime`、`IsTodayHighlighted`。
- 视觉与布局：`RangePickerIndicatorOffsetEnd`、`RangePickerIndicatorOffsetStart`。
- 其他稳定入口：`ClockIdentifier`、`DefaultDateTime`、`Format`。

维护要求：

- 外部设置的 Avalonia 属性必须在模板应用前后保持一致。
- 集合、选择、展开、过滤、分页、上传任务或异步 loader 必须能处理 reset、replace 和 clear。
- 伪类和 internal state 必须从单一 owner 推导，避免双向同步导致循环更新。
- Gallery API 表中的状态说明应与源码实际状态流一致。

## 5. 生命周期与模板接入

生命周期规则：

- 构造阶段只注册必要状态，不依赖 template part。
- 模板应用时获取 part、建立事件订阅和绑定，并先释放旧 part 订阅。
- 控件卸载、弹层关闭、窗口关闭、集合替换或 container recycle 时释放事件订阅和资源宿主。
- DynamicResource、TokenResourceBinder 或 C# binding 必须有明确 owner 和释放点。
- Browser 和 Desktop 宿主下的主题加载顺序不得影响 public API 语义。

稳定 template part 接入点：

- `PART_CalendarItem`：稳定模板协作入口，重命名前必须同步主题和实现。
- `PART_CalendarView`：稳定模板协作入口，重命名前必须同步主题和实现。
- `PART_ClearUpButton`：承载用户触发入口、导航或关闭动作。
- `PART_ConfirmButton`：承载用户触发入口、导航或关闭动作。
- `PART_ContentRightAddOnPresenter`：展示用户内容、文本、图标或模板化数据。
- `PART_HeaderButton`：承载用户触发入口、导航或关闭动作。
- `PART_HeaderFrame`：承载根视觉、边框、背景或尺寸基线。
- `PART_HeaderLayout`：稳定模板协作入口，重命名前必须同步主题和实现。
- `PART_InfoInputBox`：承载文本输入、过滤、显示或编辑入口。
- `PART_ItemFrame`：承载根视觉、边框、背景或尺寸基线。
- `PART_ItemRootLayout`：承载根视觉、边框、背景或尺寸基线。
- `PART_MonthView`：稳定模板协作入口，重命名前必须同步主题和实现。
- `PART_MonthViewLayout`：稳定模板协作入口，重命名前必须同步主题和实现。
- `PART_NextButton`：承载用户触发入口、导航或关闭动作。
- `PART_NextMonthButton`：承载用户触发入口、导航或关闭动作。
- `PART_NowButton`：承载用户触发入口、导航或关闭动作。
- `PART_Popup`：承载弹层宿主、打开关闭或候选内容。
- `PART_PreviousButton`：承载用户触发入口、导航或关闭动作。
- `PART_PreviousMonthButton`：承载用户触发入口、导航或关闭动作。
- `PART_RangePickerArrow`：稳定模板协作入口，重命名前必须同步主题和实现。
- `PART_RangePickerIndicator`：展示指示器、进度、分页或状态反馈。
- `PART_SecondaryHeaderButton`：承载用户触发入口、导航或关闭动作。
- `PART_SecondaryInfoInputBox`：承载文本输入、过滤、显示或编辑入口。
- `PART_SecondaryMonthView`：稳定模板协作入口，重命名前必须同步主题和实现。
- 其他 7 个 part 按相同生命周期规则维护。

## 6. 交互与事件处理

DatePicker 的交互事件应从输入源收敛到控件级语义事件：

- Pointer、keyboard、focus 和 command 事件不应绕过 Avalonia 基础控件语义。
- 没有弹层职责的路径不应引入额外 popup 或全局输入捕获。
- 非集合控件不应通过隐藏集合状态模拟业务数据。
- 值提交或命令触发必须保持继承控件的事件顺序。

当前没有抽取到控件专属 public 事件；交互语义主要通过继承事件、命令、属性变化和 Gallery 可观察行为体现。

## 7. 内部算法与关键流程

维护者需要重点关注以下流程：

- API 默认值到 effective state 的归一。
- Template part 重新应用时的状态回放。
- 主题资源、Token 和 SharedToken 计算后的视觉更新。
- 内容、命令和视觉状态在模板节点之间的同步。
- 状态变化时避免创建不必要的视觉对象、订阅或动画对象。

输入宽度维护规则：

- `DatePicker` 和 `RangeDatePicker` 的输入预留宽度以当前有效格式对应的最宽格式化日期时间为基线；未显式设置 `Format` 时，还需要与 Ant Design 默认原生 input 风格的输入基线取最大值。Ant Design 本地参考源码位于 `../ReferenceProjects/ant-design/components/date-picker/style/index.ts` 和 `../ReferenceProjects/ant-design/components/date-picker/demo/basic.tsx`：basic demo 不设置 `width`，样式只让内部 input `width: 100%`，默认宽度来自 input intrinsic width，而不是日期格式最小宽度。
- `PlaceholderText` 和 `SecondaryPlaceholderText` 不参与 `PreferredInputWidth` / `PreferredWidth` 计算；placeholder 只能在已预留的输入内容区域内显示，超出时由文本呈现层使用 ellipsis 省略，不能反向撑大控件默认宽度。
- `Text` 和 `SecondaryText` 只表达当前显示值或 hover preview，不作为 `PreferredInputWidth` / `PreferredWidth` 的计算来源。
- `IsShowTime`、`Format`、`ClockIdentifier`、AM/PM 文本和字体变化会重新计算格式预留宽度；选中值、hover 值和范围端点切换不得改变预留宽度。
- `Width` 显式设置或 `HorizontalAlignment=Stretch` 时，控件应交给外部布局系统决定实际宽度，不再强制内部预留宽度。
- 范围选择的两端输入使用同一个格式预留宽度，`RangePickerIndicator` 和 popup placement 只跟随稳定输入框 bounds，不反向驱动输入框测量。
- 范围输入模板的内部 `AddOnDecoratedBox` 和 content presenter 必须在控件内部 stretch；范围整体测量以 `base.MeasureOverride` 的完整宽度为基础，只替换两端输入框宽度为 `PreferredWidth`，不得重新手算 padding、spacing、icon 或 add-on 宽度。

弹层显示锚点维护规则：

- `SelectedDateTime`、`DefaultDateTime` 和 `PickerDisplayDate` 是三个不同状态：已提交值、默认选中/reset 值、弹层显示锚点。实现中不得复用 `DefaultDateTime` 表达弹层显示锚点。
- `PickerDisplayDate` 只在打开弹层时参与面板定位，不是受控面板游标；面板导航过程中不应反向写回 `PickerDisplayDate`。
- 单值 `DatePicker` 仅在设置 `PickerDisplayDate` 后介入打开定位；面板锚点优先级为 `SelectedDateTime ?? PickerDisplayDate`，结果为空时保持现有 CalendarView 默认行为。
- 范围选择打开弹层时，面板锚点优先级为当前 active endpoint 的选中值、`PickerDisplayDate`、现有默认行为；inactive endpoint 不得把面板拉回自身日期。
- 应用锚点前必须按当前 `PickerMode` 通过 `DatePickerFormattingHelper.NormalizeDateTime` 归一化。周为 ISO 周起始日，月份为当月 1 日，季度为季度首月 1 日，年份为当年 1 月 1 日。
- 应用锚点时只同步内部 CalendarView 的 `DisplayDate`、`SelectedMonth`、`SelectedYear`、`LastSelectedDate` 和高亮刷新；不得设置 `SelectedDate`，不得修改 `SelectedDateTime`、输入框文本、Form value 或清除按钮状态。

范围日历视觉状态维护规则：

- `CalendarRangeSelectionState` 同时保存真实端点和 hover 日期，但必须通过 committed range 与 preview range 两条路径输出。
- `CalendarPanelBuilder` 只用真实 `SelectedDate` / `SecondarySelectedDate` 设置 `IsSelected`，不能因为日期处于区间中或 hover 预览中而设置 selected。
- `IsRangeStart`、`IsRangeEnd`、`IsRangeMiddle` 只描述两个真实端点形成的区间；`IsRangePreviewStart`、`IsRangePreviewEnd`、`IsRangePreviewMiddle` 只描述 hover 形成的预览区间。
- `Date`、`Month`、`Quarter`、`Year` 的 range 状态必须在 `CalendarPanelBuilder` 按当前 picker unit 统一归一化后输出到 `CalendarCellState`；renderer 只负责把状态映射到 `CalendarDayButton` 或 `CalendarButton`，不能在按钮事件或主题层重新推断范围。
- `CalendarDayButtonTheme.axaml` 必须让 committed endpoint 和 preview endpoint 共用半边 range indicator；半边 indicator 的宽度来自实际 cell slot 的 50%，不能固定为 `CellWidth`，否则双月弹层或宽布局下范围背景会断裂。
- `CalendarDayButtonTheme.axaml` 与 `CalendarButtonTheme.axaml` 必须让 `:range-preview-start` / `:range-preview-end` 使用与 selected endpoint 相同的主色背景和白色前景；这是 hover range 的临时视觉端点，不等价于提交 `:selected`。
- `CalendarDayButton.EffectiveCornerRadius` 与 `CalendarButton.EffectiveCornerRadius` 必须在 committed endpoint 和 preview endpoint 上压平连接侧圆角，让浅色 range indicator 与主色端点连续。
- `CalendarDayButton` 与 `CalendarButton` 的 hover 入口都必须通知 `Calendar.NotifyHoverDateChanged`；Month、Quarter、Year 的范围选择依赖这个入口同步输入框临时结束值和面板 preview range，不能只在 day button 上维护 hover。
- 范围选择的 active part 是面板显示月份的前置状态，`RangeDatePickerPresenter.IsRangeStartActive` 必须先通过模板传给 `RangeCalendar.IsSelectRangeStart`，再同步 `SelectedDate` / `SecondarySelectedDate`。
- `Calendar.SelectedDate` 在 range 模式中只代表开始端点；只有 Start 端激活时它才允许驱动 `DisplayDate`。End 端激活时，开始端点只能参与范围计算和高亮，不能把双月面板回滚到开始月份。

PickerMode 颗粒度维护规则：

- `DatePickerMode` 是 public 颗粒度契约，外层 `DatePicker` / `RangeDatePicker` 通过 `PickerMode` relay 到 presenter，再由 AXAML `TemplateBinding` 传给 `Calendar` / `RangeCalendar` / `DualMonthRangeCalendar`。
- `Calendar.PickerMode` 必须进入 `CalendarViewState`，panel model 构建和 selected/focused 判断都从同一个 state 读取颗粒度，不能在 renderer 或按钮事件里重新推断。
- 目标面板映射固定为 `Date` / `Week` -> `CalendarMode.Month`，`Month` / `Quarter` -> `CalendarMode.Year`，`Year` -> `CalendarMode.Decade`。
- `CalendarItem` 点击年面板按钮时按目标颗粒度决定行为：`Date` / `Week` 继续进入月视图，`Month` / `Quarter` 直接提交，十年面板中的 `Year` 直接提交。
- `PickerMode=Week` 仍复用月视图容器，但实际结构必须切换为 8 列：首列为 ISO 周序号，后 7 列为日期；`CalendarPanelBuilder.BuildMonthPanel` 通过 `WeekCells` 输出周序号列，renderer 负责按行把周序号和 7 个日期 cell 对齐。
- Week 模式的选中周必须使用 `:week-selection-start` / `:week-selection-middle` / `:week-selection-end` 这组内部伪类和 `WeekSelectionIndicator` 渲染连续主色行；不能通过给 7 个日期 cell 分别设置普通 `:selected` 背景来模拟。
- Week 模式的 hover 周必须从 `Calendar.HoverDate` 同步到 `CalendarViewState`，再由 `CalendarPanelBuilder` 输出 `:week-hover-start` / `:week-hover-middle` / `:week-hover-end`；hover 触发范围是整行，`CalendarItem` 必须基于 `PART_MonthView` pointer 位置推导当前周，不能只依赖日期按钮自身 `PointerEntered`。
- Week 模式的 hover 状态入口必须统一经过 `Calendar.NotifyHoverDateChanged` 归一化为周起始日；`RangeCalendar.HoverDateTime` 与 `Calendar.HoverDate` 必须保存同一个归一化值，不能让输入框 preview 使用归一化值而面板模型继续持有原始日期 cell。
- 双月 Week 面板的 pointer hit-test 必须同时覆盖主 `PART_MonthView` 和 `PART_SecondaryMonthView`；`DualMonthCalendarItem.IsPointerInMonthView` 与周起点推导逻辑必须保持同一覆盖范围，不能出现“右侧面板判定在月视图内，但周起点只从左侧面板查找”的状态断裂。
- `CalendarDayButtonTheme.axaml` 使用 `WeekHoverIndicator` 渲染连续浅色行，并排除普通单格 `:pointerover` 背景。
- Week 模式中 selected 周优先于 hover 周；当 hover 命中已选周时，builder 不输出 week hover 伪类，避免浅色 hover 覆盖主色选中行。
- Week 范围选择中，已提交范围的端点周继续使用 `:week-selection-*` 主色整行；两个端点之间的中间周使用 `:week-range-start` / `:week-range-middle` / `:week-range-end` 和 `WeekRangeIndicator` 渲染 `CellActiveWithRangeBg` 浅色整行，周号列也必须参与范围背景。
- Week 范围选择预览中，`CalendarRangeSelectionState.HoverDate` 形成的临时端点周也使用 `:week-selection-*` 主色整行，但 `CalendarCellState.IsSelected` 必须保持 `false`；预览端点之间的中间周复用 `:week-range-*` 浅色整行，且 preview range 优先于 committed range 输出。
- Week 模式不得输出普通范围伪类 `:range-start` / `:range-end` / `:range-middle` / `:range-preview-start` / `:range-preview-end` / `:range-preview-middle`；这些伪类服务 Date、Month、Quarter、Year 的 picker unit cell，否则会与周行 indicator 叠加，导致端点周和中间周背景断裂。
- `CalendarPanelBuilder.BuildQuarterPanel` 输出 Q1-Q4 四个季度 cell，季度选择不能复用 12 个月面板伪装；`CalendarItem` 在 `PickerMode=Quarter` 且 `DisplayMode=Year` 时必须把 `PART_YearView` 配置为 1 行 4 列紧凑面板，并取消通用年/月/十年面板最小高度，避免未使用的 8 个 slot 撑出空白。
- `RangeDatePicker` 的弹层始终保持双面板语义：`Date` / `Week` 使用左右双月面板，`Month` 使用左右双年面板，`Quarter` 使用左右双年紧凑季度面板，`Year` 使用左右双十年面板；`DualMonthCalendarItem` 的 secondary 面板必须有独立 `PART_SecondaryYearView`，不能在非月视图时退化成单面板。
- `CalendarItem.SetupHeaderForDisplayModeChanged()` 是 `CalendarMode` 可见布局的唯一源头：运行时切换 `PickerMode` 或 `DisplayMode` 时，必须同步维护 `PART_MonthViewLayout`、`PART_MonthView` 与 `PART_YearView` 的互斥可见性；`DualMonthCalendarItem` 只在此基础上补齐 `PART_SecondaryMonthView`、`PART_YearViewLayout`、`PART_SecondaryYearView` 和左右导航按钮，不能只依赖 `OnApplyTemplate` 初始化分支。
- `CalendarItem.OnAttachedToVisualTree` 只能走 `RefreshLocalizedContent()` 的 mode 分发刷新，不能无条件调用 day cell 渲染；否则 `RangeDatePicker` 在 `Month` / `Quarter` / `Year` 颗粒度下会让未初始化或不参与当前 DisplayMode 的 secondary month state 进入月视图构建。
- 提交值统一由 `DatePickerFormattingHelper.NormalizeDateTime` 归一化：周为 ISO 周起始日，月份为当月 1 日，季度为季度首月 1 日，年份为当年 1 月 1 日。
- `DatePickerFormattingHelper` 同时负责默认格式、显示文本和预留宽度。显示文本按目标颗粒度格式化；默认宽度预留在非 Date 颗粒度下仍使用稳定输入基线，显式 `Format` 才按用户格式重新计算，不能因为用户传入的 placeholder、hover 或选中值变化而改变宽度。
- `IsShowTime` 只在 `PickerMode=Date` 时形成有效时间选择；presenter 使用 `IsTimeSelectionVisible` 控制 TimeView 显示和时间拼接，其他颗粒度忽略时间面板。

实现文档不逐行解释私有方法。若某个私有算法成为稳定维护入口，应在本节补充算法不变量，而不是把代码复述为说明书。

## 8. 资源、性能与 AOT 边界

资源和 AOT 约束：

- 不通过运行时反射扫描 public API、Token 或 Gallery 表格数据。
- 不把可静态声明的模板结构迁移到 C# 动态创建。
- 异步加载、上传、弹层和窗口生命周期必须能取消或释放。
- 缓存对象必须与控件、窗口、弹层或数据 owner 生命周期一致。
- Source generator 生成文件不手工编辑；需要修改时改输入源或 generator。

性能边界：

- 控件应优先复用 Avalonia 原生虚拟化、模板绑定和资源系统。
- 避免为每次状态变化创建不必要的视觉对象、订阅或动画对象。
- 大集合控件必须保证 container recycle 后不会泄漏旧 item 状态。

## 9. 维护不变量

维护 DatePicker 时不得破坏：

- Public API、默认值、事件顺序和 Gallery 可观察行为。
- Template part 名称、ControlTheme key、伪类和资源 key。
- 旧 template part、事件订阅、Popup/Flyout/Window host 和 collection view 的释放路径。
- Light/Dark、Browser/Desktop 和不同 SizeType 下的主题一致性。
- 文档、Gallery API 表、Token 表与源码契约的一致性。

## 10. 测试与验证

推荐验证：

- 纯文档改动运行 `git diff --check` 并检查相对链接。
- 控件 API 或行为变更运行对应 `tests/AtomUI.Desktop.Controls.Tests` 或专用包测试。
- DataGrid 相关变更运行 `tests/AtomUI.Desktop.Controls.DataGrid.Tests`。
- Gallery 示例、API 表或 Token 表变更运行 `tests/AtomUIGallery.Tests`。
- AOT、生成器或动态数据路径变更按 Gallery NativeAOT 发布流程验证。
