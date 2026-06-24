# DatePicker 桌面版实现原理

本文档描述 DatePicker 桌面版的内部实现范围、源码职责、状态流、生命周期、资源边界和维护规则。公共设计与 API 契约见 [DatePicker 桌面版架构设计](overview.md)，变化记录见 [DatePicker Changelog](changelog.md)。涉及组件 Token 的实现应同时阅读 [DatePicker Token 设计](token.md)。

## 1. 实现定位

本文档覆盖 DatePicker 的控件实现、主题接入、状态同步和 Gallery 可见维护边界。具体属性注册、默认值、绘制细节和 AXAML selector 仍应直接阅读源码；本文只记录维护者必须理解的稳定结构和不变量。

## 2. 源码文件结构

主要源码文件：

- `src/AtomUI.Desktop.Controls/DatePicker`：8 个文件，代表文件 `DatePicker.cs`、`DatePickerPresenter.cs`、`DatePickerToken.cs`、`DualMonthArrowDecoratedBox.cs`、`DualMonthRangeDatePickerPresenter.cs` 等。
- `src/AtomUI.Desktop.Controls/DatePicker/CalendarView`：10 个文件，代表文件 `Calendar.cs`、`CalendarBlackoutDatesCollection.cs`、`CalendarButton.cs`、`CalendarDayButton.cs`、`CalendarDayButtonPseudoClass.cs` 等。
- `src/AtomUI.Desktop.Controls/DatePicker/Localization`：3 个文件，代表文件 `en_US.cs`、`zh_CN.cs`、`zh_TW.cs`。
- `src/AtomUI.Desktop.Controls/DatePicker/Themes`：19 个文件，代表文件 `CalendarButtonTheme.axaml`、`CalendarButtonTheme.cs`、`CalendarDayButtonTheme.axaml`、`CalendarItemTheme.axaml`、`CalendarItemTheme.cs` 等。

职责边界：

- 控件主文件保留 public/protected API、Avalonia 属性注册、事件和主要生命周期入口。
- Theme 文件负责静态视觉结构、template part、selector 和资源绑定。
- Token 文件只提供组件视觉变量，不保存实例状态。
- Gallery 文件只展示用法、API 表和 Token 表，不作为运行时逻辑 owner。

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
- 选择与集合：`DisplayMode`、`RangeEndSelectedDate`、`RangeStartSelectedDate`、`SecondarySelectedDate`、`SecondarySelectedDateTime`、`SelectedDate`、`SelectedDateTime`、`TempSelectedTime`。
- 交互与状态：`IsFloatingArrowPosition`、`IsHorizontalFlipped`、`IsNeedConfirm`、`IsShowNow`、`IsShowTime`、`IsTodayHighlighted`。
- 视觉与布局：`RangePickerIndicatorOffsetEnd`、`RangePickerIndicatorOffsetStart`。
- 其他稳定入口：`ClockIdentifier`、`DefaultDateTime`、`DisplayDate`、`DisplayDateEnd`、`DisplayDateStart`、`FirstDayOfWeek`、`Format`。

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
