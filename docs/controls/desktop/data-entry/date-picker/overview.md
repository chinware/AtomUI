# DatePicker 桌面版架构设计

本文档定义 `DatePicker` 桌面版的最新设计定位、公共契约、状态模型、视觉主题关系和兼容边界。通用控件研发约束见 [控件研发标准](../../../../engineering/control-development-guidelines.md)，内部实现原理见 [DatePicker 桌面版实现原理](implementation.md)，DatePicker Token 的专项设计见 [DatePicker Token 设计](token.md)，设计和契约变化记录见 [DatePicker Changelog](changelog.md)。

## 1. 控件定位

| 项 | 值 |
| --- | --- |
| NuGet 包 | `AtomUI.Desktop.Controls` |
| .NET 命名空间 | `AtomUI.Desktop.Controls` |
| AXAML 命名空间 | `https://atomui.net` |
| Gallery 页面 | `controlgallery/AtomUIGallery/ShowCases/DataEntry/DatePicker` |
| 控件状态 | Stable |

DatePicker 是 AtomUI 桌面控件体系中的日期选择控件，用于单日期、日期范围和日历面板选择。

DatePicker 不负责业务日程系统、时间选择或完整日期时间解析服务。这些职责应由业务层、组合控件或更专用的 AtomUI 控件承担。

主要源码入口：

- `src/AtomUI.Desktop.Controls/DatePicker`

## 2. 设计语言

DatePicker 的设计语言围绕控件职责、可观察状态和主题契约组织，而不是围绕模板节点组织。

| 维度 | 含义 | DatePicker 中的表达 |
| --- | --- | --- |
| 产品语义 | 控件在界面中承担的稳定职责。 | DatePicker 是 AtomUI 桌面控件体系中的日期选择控件，用于单日期、日期范围和日历面板选择。 |
| 内容承载 | 用户数据、展示内容、集合项或操作入口如何进入控件。 | `HeaderBackground`。 |
| 状态反馈 | public API、内部状态和伪类如何形成用户可感知反馈。 | selection/checked/active、visual option。 |
| 主题语义 | ControlTheme、SharedToken、组件 Token 和模板绑定如何表达视觉。 | DatePicker Token + ControlTheme。 |

## 3. API 与契约模型

DatePicker 的公共契约由 public/protected 类型成员、Avalonia 属性、事件、命令、template part、伪类、ControlTheme key 和资源 key 共同组成。维护时应先确认这些契约是否已经被源码、Gallery 示例或文档暴露。

核心 public surface 按语义分组维护：

| 契约组 | 代表成员 | 维护含义 |
| --- | --- | --- |
| 内容与数据 | `HeaderBackground` | 定义控件展示内容、输入数据、模板或业务对象入口。 |
| 选择与集合 | `DisplayMode`、`RangeEndSelectedDate`、`RangeStartSelectedDate`、`SecondarySelectedDate`、`SecondarySelectedDateTime`、`SelectedDate`、`SelectedDateTime`、`TempSelectedTime` | 维护选择、展开、过滤、分页、分组或集合状态。 |
| 交互与状态 | `IsFloatingArrowPosition`、`IsHorizontalFlipped`、`IsNeedConfirm`、`IsShowNow`、`IsShowTime`、`IsTodayHighlighted` | 表达用户可观察状态、可用性、清除、加载或反馈语义。 |
| 视觉与布局 | `RangePickerIndicatorOffsetEnd`、`RangePickerIndicatorOffsetStart` | 影响尺寸、位置、颜色、形状、密度和模板视觉变量。 |
| 其他稳定入口 | `ClockIdentifier`、`DefaultDateTime`、`DisplayDate`、`DisplayDateEnd`、`DisplayDateStart`、`FirstDayOfWeek`、`Format` | 保留为 public surface，变更前需确认 Gallery 和用户 XAML 依赖。 |

当前没有抽取到控件专属 public 事件；交互通知主要来自继承事件、命令或 Gallery 可观察状态。

主要公开类型与枚举：

- 类型：`Calendar`、`CalendarItem`、`ChoosingStatusEventArgs`、`DatePicker`、`DatePickerPresenter`、`DateSelectedEventArgs`、`DualMonthArrowDecoratedBox`、`DualMonthCalendarItem`、`DualMonthRangeCalendar`、`DualMonthRangeDatePickerPresenter`、`RangeCalendar`、`RangeCalendarItem`、`RangeDatePicker`、`RangeDatePickerPresenter` 等 19 项。
- 枚举：无。

稳定 template part：

| Template Part | 类型 | 职责 |
| --- | --- | --- |
| `PART_CalendarItem` | `CalendarItem` | 稳定模板协作入口，重命名前必须同步主题和实现。 |
| `PART_CalendarView` | `?` | 稳定模板协作入口，重命名前必须同步主题和实现。 |
| `PART_ClearUpButton` | `?` | 承载用户触发入口、导航或关闭动作。 |
| `PART_ConfirmButton` | `?` | 承载用户触发入口、导航或关闭动作。 |
| `PART_ContentRightAddOnPresenter` | `?` | 展示用户内容、文本、图标或模板化数据。 |
| `PART_HeaderButton` | `HeadTextButton` | 承载用户触发入口、导航或关闭动作。 |
| `PART_HeaderFrame` | `?` | 承载根视觉、边框、背景或尺寸基线。 |
| `PART_HeaderLayout` | `?` | 稳定模板协作入口，重命名前必须同步主题和实现。 |
| `PART_InfoInputBox` | `?` | 承载文本输入、过滤、显示或编辑入口。 |
| `PART_ItemFrame` | `?` | 承载根视觉、边框、背景或尺寸基线。 |
| `PART_ItemRootLayout` | `?` | 承载根视觉、边框、背景或尺寸基线。 |
| `PART_MonthView` | `Grid` | 稳定模板协作入口，重命名前必须同步主题和实现。 |
| `PART_MonthViewLayout` | `?` | 稳定模板协作入口，重命名前必须同步主题和实现。 |
| `PART_NextButton` | `IconButton` | 承载用户触发入口、导航或关闭动作。 |
| `PART_NextMonthButton` | `IconButton` | 承载用户触发入口、导航或关闭动作。 |
| `PART_NowButton` | `?` | 承载用户触发入口、导航或关闭动作。 |
| `PART_Popup` | `?` | 承载弹层宿主、打开关闭或候选内容。 |
| `PART_PreviousButton` | `IconButton` | 承载用户触发入口、导航或关闭动作。 |
| `PART_PreviousMonthButton` | `IconButton` | 承载用户触发入口、导航或关闭动作。 |
| `PART_RangePickerArrow` | `?` | 稳定模板协作入口，重命名前必须同步主题和实现。 |
| `PART_RangePickerIndicator` | `?` | 展示指示器、进度、分页或状态反馈。 |
| `PART_SecondaryHeaderButton` | `?` | 承载用户触发入口、导航或关闭动作。 |
| `PART_SecondaryInfoInputBox` | `?` | 承载文本输入、过滤、显示或编辑入口。 |
| `PART_SecondaryMonthView` | `?` | 稳定模板协作入口，重命名前必须同步主题和实现。 |
| 其他 part | 7 项 | 参见源码和主题文件；维护时按同一生命周期规则检查。 |

控件专属或内部伪类包括 `Blackout=:blackout`、`BtnFocusedPC`、`CalendarDayButtonPseudoClass.Blackout`、`CalendarDayButtonPseudoClass.DayFocused`、`CalendarDayButtonPseudoClass.RangeEnd`、`CalendarDayButtonPseudoClass.RangeMiddle`、`CalendarDayButtonPseudoClass.RangeStart`、`CalendarDayButtonPseudoClass.Today`、`CalendarDisabledPC`、`DayFocused=:dayfocused`、`RangeEnd=:range-end`、`RangeMiddle=:range-middle` 等 14 项。这些伪类属于主题 selector 可观察契约，不能在未同步主题和 Gallery 的情况下重命名或删除。

## 4. 行为与状态模型

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
- 模板重套用时必须把 public API 对应状态回放到新的 part、伪类和主题变量。
- 集合、弹层、异步、动效或窗口相关状态必须能处理 reset、close、cancel、detach 和 owner 释放。

## 5. 视觉与主题模型

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
- Browser 或平台特化主题必须保持同一 API 的语义一致。

## 6. 控件家族或集成关系

DatePicker 与同分类控件共享尺寸、状态、Token、Gallery 展示和验证规则。组合或派生控件应显式说明哪些 API 被继承、覆盖或不支持。

主要协作类型：

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

集成关系：

- 与 ThemeManager、SharedToken、ControlTheme 和 Gallery ShowCase 的示例/API/Token 表保持一致。
- 涉及 ItemsSource、Popup、Flyout、Window、Form 或 CompactSpace 的路径必须保持生命周期释放和数据状态同步。
- 源码目录中的共享基类和内部协作类型形成维护边界，不能只修改桌面包装类而忽略共享状态 owner。

## 7. 兼容性不变量

维护 DatePicker 时必须保持以下不变量：

- 不擅自新增、删除、重命名或改变 public/protected API、Avalonia 属性、事件和默认值。
- 不破坏 template part、伪类、ControlTheme key、Token 名称和资源 key。
- 不改变 Gallery 已展示的 XAML 用法、默认外观、交互顺序和状态优先级。
- Template part 重新应用、集合替换、弹层关闭、窗口失活和控件 detach 时必须释放旧订阅和资源宿主。
- 不通过隐藏延迟、强制刷新或吞异常掩盖状态同步问题。
- 不引入运行时反射扫描作为 API、Token 或数据路径发现机制。
- 文档只描述当前稳定设计；历史变化记录在 `changelog.md`。

## 8. 专项模型

### 8.1 选择与当前项模型

DatePicker 的当前项状态必须由单一 owner 推导。public 选择属性、集合项容器和伪类之间只能做单向同步，集合替换、清空和模板重套用时必须回放当前状态。

### 8.2 视觉选项模型

DatePicker 的视觉选项通过 public API 归一为 theme variables、伪类或模板绑定。Token 保存组件语义值，不能保存实例运行时状态或业务色值。

## 9. 文档导航、LLMS 导出与验证策略

关联文档：

- [DatePicker 桌面版实现原理](implementation.md)
- [DatePicker Token 设计](token.md)
- [DatePicker Changelog](changelog.md)

LLMS 语义区域：

| Part | AtomUI 节点 | 职责 | 相关 API | 相关 Token | 稳定性 |
| --- | --- | --- | --- | --- | --- |
| `root` | `DatePicker` | 数据录入控件根语义区域，承载 public API、值状态、验证状态和主题入口。 | 见 API 与契约模型 | 见视觉与主题模型 | stable |
| `input` | `输入或编辑区域` | 承载用户输入、当前值、占位、格式化或只读状态。 | 见 API 与契约模型 | 见视觉与主题模型 | stable |
| `trigger` | `触发区域` | 承载清除、展开、提交、步进、上传或辅助操作。 | 见 API 与契约模型 | 见视觉与主题模型 | stable |
| `popup` | `弹层或候选区域` | 承载下拉、候选项、日历、颜色面板或异步内容。 | 见 API 与契约模型 | 见视觉与主题模型 | stable |
| `validation` | `校验反馈区域` | 承载 Form、status、错误、警告、help 或 loading 状态。 | 见 API 与契约模型 | 见视觉与主题模型 | stable |

LLMS 导出来源：

| LLMS 内容 | 来源 | 说明 |
| --- | --- | --- |
| 单控件完整文档 | `overview.md` + Gallery API / Token / ShowCase | 生成 `controls/date-picker/index-cn.md` |
| 单控件语义文档 | `overview.md` + `implementation.md` + theme/template 信息 | 生成 `controls/date-picker/semantic-cn.md` |
| API 表 | Gallery ApiDataGrid 或源码 public surface | 不在 `overview.md` 中复制完整 API 表 |
| Design Token 表 | Gallery DesignTokenDataGrid、Token 类型或第 5 节主题模型 | 不在生成产物中手工维护第二份 Token 表 |
| 示例 | Gallery ShowCase + source snippet catalog | 只引用稳定示例 |
| 源码索引 | `implementation.md` | 用于定位控件源码、主题和测试 |

验证策略：

| 改动类型 | 验证要求 |
| --- | --- |
| 文档改动 | 运行 `git diff --check`，检查相对链接存在。 |
| Public API | 覆盖属性默认值、事件触发、命令和继承语义。 |
| 状态模型 | 覆盖 selection/checked/active、visual option、disabled、hover、pressed、focus 以及控件特有状态。 |
| AXAML/Theme | 检查 template part、伪类、资源 key、Light/Dark 主题和 Browser 主题。 |
| Token | 检查 TokenKind、AXAML token resource、Gallery Token 表和文档同步。 |
| Gallery | 走查对应 ShowCase 示例、API 表和 Token 表入口。 |
