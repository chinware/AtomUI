# Calendar 桌面版架构设计

本文档定义 `Calendar` 家族桌面版的稳定定位、公共契约、状态模型、视觉主题关系和兼容边界。内部实现见 [Calendar 桌面版实现原理](implementation.md)，行为规则见 [Calendar 行为设计](behavior-design.md)，农历能力见 [LunarCalendar 农历能力设计](lunar-calendar-design.md)，范围条见 [Calendar 范围条设计](range-bar-design.md)，Token 见 [Calendar Token 设计](token.md)，变化记录见 [Calendar Changelog](changelog.md)。

## 1. 控件定位

| 项 | 值 |
| --- | --- |
| NuGet 包 | `AtomUI.Desktop.Controls` |
| .NET 命名空间 | `AtomUI.Desktop.Controls` |
| AXAML 命名空间 | `https://atomui.net` |
| Gallery 页面 | `controlgallery/AtomUIGallery/ShowCases/DataDisplay/Calendar` |
| 控件状态 | Stable |

`Calendar` 是按日期组织业务展示内容的桌面日历控件，遵循本专题定义的月面板、年面板、Header、范围限制、禁用规则、周序号和单元格定制语义。它同时保留桌面端可用的焦点与方向键导航。`LunarCalendar` 继承这些稳定语义，并在同一公历状态模型上增加中国农历、二十四节气、传统节日、周末以及应用提供的节假日/调休投影。

Calendar 只负责“查看并选择一个日期或月份”的面板体验，不负责日期输入弹层、范围选择、多日期选择、时间编辑或复杂日程排布。日期输入由 DatePicker 等控件承担，范围和日程数据由业务层承担；Calendar 只提供轻量 `RangeBars` 标记能力，用于在日期网格中表达连续日期业务条。新 Calendar 的内部 `CalendarView` 与 DatePicker 的旧 CalendarView 子系统完全隔离。

主要源码入口：

- `src/AtomUI.Desktop.Controls/Calendar`
- `controlgallery/AtomUIGallery/ShowCases/DataDisplay/Calendar`

## 2. 设计语言

Calendar 的设计语言围绕日期面板的产品语义、可观察状态和主题契约组织，而不是围绕某个模板节点组织。

| 维度 | 含义 | Calendar 中的表达 |
| --- | --- | --- |
| 产品语义 | 控件在界面中承担的稳定职责。 | 以 Month/Year 两种面板展示日期或月份，并提交单一选中值。 |
| 内容承载 | 业务数据和模板如何进入控件。 | `Value`、`ValidRange`、`DisabledDate`、`CellTemplate`、`FullCellTemplate` 与 `HeaderTemplate`。 |
| 状态反馈 | API、内部状态与伪类如何形成反馈。 | `today`、`selected`、`outside`、`disabled`、`focused`、`fullscreen`、`mini`、`show-week`。 |
| 主题语义 | SharedToken、`CalendarToken`、`LunarCalendarToken`、ControlTheme 与模板如何表达视觉。 | Calendar 基础主题消费九个 Calendar 专属 Token；LunarCalendar 只增加农历内容所需的增量 Token。 |

设计上的首要不变量是：Cell 定制不能夺走日期值、选中、禁用、焦点和命中测试语义；这些语义由 Cell 容器保留，模板只改变内容呈现方式。

## 3. API 与契约模型

Calendar 的公共契约由 Avalonia 属性、事件、模板、上下文类型、枚举、伪类和 ControlTheme 共同组成。

### 3.1 属性

| 属性 | 默认值 | 语义 |
| --- | --- | --- |
| `Value` | 构造时的 `DateTime.Today` | 当前选中日期和面板锚点；写入和提交均规范化到 `.Date`。 |
| `Mode` | `CalendarMode.Month` | `Month` 显示 6×7 日期网格；`Year` 显示 3×4 月份网格。 |
| `Fullscreen` | `true` | 完整布局或紧凑 Mini 布局；不改变日期算法和选择语义。 |
| `ShowWeek` | `false` | Month 日期面板是否显示周序号列。 |
| `ValidRange` | `null` | 首尾包含的日期范围；范围外日期禁用。 |
| `DisabledDate` | `null` | 业务禁用谓词；与 `ValidRange` 共同决定 Cell 是否可用。 |
| `CellTemplate` | `null` | 替换默认值下方的业务内容区域，但保留默认日期/月值和 Cell 状态。 |
| `FullCellTemplate` | `null` | 替换 Cell 的完整内部内容；优先于 `CellTemplate`。 |
| `HeaderTemplate` | `null` | 自定义 Header；为空时使用默认 Year/Month/Mode Header。 |
| `RangeBars` | empty | 声明连续日期范围条；仅在 Fullscreen Month 日期网格 overlay 中渲染。 |

`ValidRange` 使用 `CalendarDateRange(start, end)`，两端均包含，构造函数把时间规范化到日期并拒绝 `end < start`。`CalendarCellContext` 提供 `Value`、`Today`、`CellType`、`DisplayValue`、`IsToday`、`IsInView`、`IsSelected` 和 `IsDisabled`。`CalendarHeaderContext` 提供当前 `Value`/`Mode` 以及提交值和模式的命令。

`CalendarRangeBar` 提供 `StartDate`、`EndDate`、`Label`、`Background` 和 `Height`。日期端点按 `.Date` 投影，首尾包含；`Background` 支持普通 brush、binding、DynamicResource 和 TokenResource。范围条模型、分段算法和资源生命周期见 [Calendar 范围条设计](range-bar-design.md)。

`Value` 与 `Mode` 的默认 Avalonia binding mode 均为 `TwoWay`，用户通过 Cell 或 Header 提交的新状态可以写回绑定源。

### 3.2 事件

| 事件 | 语义 |
| --- | --- |
| `PanelChanged` | 用户选择跨自然月/自然年，或用户切换 Month/Year 模式时触发。 |
| `ValueChanged` | 用户提交后日期值实际变化时触发；程序直接设置 `Value` 不触发。 |
| `Selected` | 每次有效用户选择触发，即使选择的日期与当前 `Value` 相同。 |

用户选择的提交顺序固定为 `PanelChanged` → `ValueChanged` → `Selected`，不存在的事件从序列中省略。事件发生时新值已经写入 `Value`。程序直接设置 `Value` 或 `Mode` 只更新属性和渲染，不模拟用户事件。

`CalendarSelectSource` 用于区分 `Year`、`Month`、`Date` 和 `Customize` 来源。周序号 Cell 可选择；激活后以该行周首日作为日期值，并使用 `Date` 来源。

### 3.3 模板、伪类与主题契约

稳定的根模板协作入口为 `PART_HeaderPresenter`、`PART_BodyPresenter`、`PART_CalendarView`、`PART_RangeBarPanel`、`PART_DefaultHeader` 和 `PART_CustomHeader`；默认 Header 内部使用 `PART_YearSelect`、`PART_MonthSelect`、`PART_ModeSwitch`，View 内部使用 `PART_WeekHeader` 和 `PART_CellHost`，Cell 内部使用 `PART_Item`、`PART_CellInner`、`PART_ItemContent` 和 `PART_Value`。

根伪类包括 `:fullscreen`、`:mini`、`:month`、`:year`、`:show-week`；Cell 伪类包括 `:date`、`:month`、`:week`、`:today`、`:selected`、`:outside`、`:disabled`、`:focused`。四个内部 ControlTheme 的 key 与伪类是主题兼容契约，变更必须同步源码、Gallery 和文档。

### 3.4 LunarCalendar 扩展契约

`LunarCalendar` 是 `Calendar` 的公开派生控件，继续使用 `Value`、`Mode`、`Fullscreen`、`ShowWeek`、`ValidRange`、`DisabledDate`、三个模板入口、`RangeBars` 和三个选择事件。它增加 `ShowSolarTerms`、`ShowTraditionalFestivals`、`ShowHolidays`、`HighlightWeekends`、`HolidayProvider`、只读 `SelectedLunarDateInfo` 和 `RefreshHolidayData()`。

农历算法保证范围为 `1900-01-01` 至 `2100-12-31`。`Value` 在 LunarCalendar 上收敛到该范围；内部有效选择范围为支持范围与 `ValidRange` 的交集。法定节假日和调休不内置，由 `ILunarCalendarHolidayProvider` 以同步面板数据提供。完整模型、默认值、枚举、Provider 规则和显示优先级见 [LunarCalendar 农历能力设计](lunar-calendar-design.md)。

## 4. 行为与状态模型

状态流按单一 owner 收敛：

```text
Public API / Header / Cell input
  -> Calendar（Value、Mode、事件顺序）
  -> CalendarHeader + CalendarView（不可变投影）
  -> CalendarViewCell（状态、模板、命中测试）
  -> ControlTheme selector / Gallery 可观察行为
```

- `Calendar` 是唯一业务状态 owner；`CalendarView` 不保存第二份公开选中值。
- Month 模式按当前月生成 42 个日期 Cell；Year 模式按当前年生成 12 个月 Cell。
- `ShowWeek` 只作用于 Month 日期网格，增加每行一个周序号 Cell。周序号 Cell 可被点击并提交该行周首日，但不作为 `CalendarCellContext` 的日期/月模板项。
- 日期禁用由 `ValidRange` 与 `DisabledDate` 的并集决定；月份禁用按“月份首日和末日都在范围外”或业务规则判定，不能只检查当前日。
- 方向键只移动面板内的 roving focus；应跳过禁用 Cell 和周序号 Cell，无合法目标时保留当前焦点。Enter/Space 才提交选择。
- `Fullscreen`/Mini 只改变布局密度和 Header 控件尺寸，不改变值、事件顺序、禁用和模板优先级。
- `RangeBars` 只改变日期网格上方的 overlay 业务标记层，不改变 Cell 外间距、选择状态、禁用状态、鼠标指针、事件顺序或 Automation。
- AtomUI 语言服务改变会同步更新日期格式、周标题、月份名称、Header 的 Month/Year 文本与年份后缀。
- LunarCalendar 不建立第二份农历选中值；`SelectedLunarDateInfo`、Cell 农历内容和 Header 农历标签都从当前公历 Value/面板数据单向投影。
- Fullscreen/Card × Month/Year 四种组合共享 CalendarView 的网格拓扑、焦点、容器池和 Automation，农历 adapter 只改变专用 Cell、Header 文案和布局 metrics。

## 5. 视觉与主题模型

Calendar 使用源码中的 `CalendarToken` 以及 SharedToken。专属 Token 只表达九个组件视觉语义：`FullBg`、`FullPanelBg`、`ItemActiveBg`、`YearControlWidth`、`MonthControlWidth`、`YearMonthCellWidth`、`MiniContentHeight`、`FullCellMinHeight`、`RangeBarHeight`。运行时状态通过伪类和 selector 表达，不写入 Token。

LunarCalendar 使用独立 exact Control identity 和 `LunarCalendarToken`，只补充双行 Cell、农历次级文本、卡片内容高度、周末/节假日标记、Fullscreen Cell 高度和范围条避让所需语义。LunarCalendar root 不通过深层 selector 修改 CalendarHeader、CalendarView、ComboBox、OptionButtonGroup 或普通 CalendarViewCell 的模板内部。

| Theme 文件 | 稳定职责 |
| --- | --- |
| `CalendarTheme.axaml` | 根背景、Header/CustomHeader 选择、CalendarView 与范围条 overlay 接线；Fullscreen 拉伸，Mini 提供无外框的卡片内容布局，外部容器负责边框与宽度。 |
| `CalendarHeaderTheme.axaml` | Year Select、Month Select、Month/Year `OptionButtonGroup` 模式切换。Mini 时 Header 交互控件应使用 Small 尺寸。 |
| `CalendarViewTheme.axaml` | WeekHeader、CellHost，以及 Fullscreen/Mini 的布局差异。 |
| `CalendarViewCellTheme.axaml` | 默认日期值、Cell/FullCell 模板消费、状态 selector 和命中测试视觉。 |

`CellTemplate` 必须保留默认值显示，并与内置范围条 overlay 共存；`FullCellTemplate` 覆盖完整 Cell 内部内容且优先级最高，但不替换 Calendar body overlay。两者都不能删除禁用、选中、焦点和 outside 的容器状态。

普通 Calendar 在 `Fullscreen=false` 时使用 256 高内容区，包含 WeekHeader 与六行 CellHost；LunarCalendar 由自身 `MiniContentHeight` 按双行 Cell 尺寸和共享间距派生有效内容高度。body 的顶部分隔线和纵向 Padding 位于该内容区之外。Mini 的 selected/today/disabled 状态分别使用主色实心、主色单线描边和禁用背景；Fullscreen selected 保持 `ItemActiveBg` 与主色日期值，不复用 Mini 的实心主色规则。

## 6. 控件家族或集成关系

Calendar 与 DatePicker、ThemeManager、LanguageManager、Gallery 和 Token 生成系统协作，但不共享 DatePicker 的旧 CalendarView 类型。DatePicker 负责输入弹层和范围输入；Calendar 家族负责独立的面板展示与单值选择。LunarCalendar 的全部运行时实现仍位于 `src/AtomUI.Desktop.Controls/Calendar`，不进入 Extras 或独立包。

Gallery 示例覆盖基础 Fullscreen、Notice Calendar、跨日期 `RangeBars`、Mini Card，以及可选择日历的面板值与最后选择值分离。可选择日历示例通过 `Value` 的默认 TwoWay 绑定接收 Header 面板变化，并只在 `Selected` 事件中更新 Alert 的最后选择值；该示例是页面最后一个 ShowcaseItem。API/Token 表必须来自源码和本目录文档，不能由生成 LLMS 文件反向维护。

## 7. 兼容性不变量

- 不擅自新增、删除或重命名 public 属性、事件、上下文类型、枚举成员、模板 part、伪类、ControlTheme key 或 Token。
- `Value` 的日期规范化、用户事件顺序、`FullCellTemplate` 优先级和月份两端禁用规则属于行为兼容契约。
- `RangeBars` 不改变 Cell 外间距、网格行列、选择/禁用语义、事件顺序和 Automation；`CalendarRangeBar.Background` 的资源绑定必须跟随 Calendar owner 生命周期释放。
- 模板重应用、模式切换、语言切换和控件 detach 必须释放旧事件订阅、清理旧容器 owner，并把当前状态回放到新模板。
- 不把 DatePicker 的旧 Calendar API（`SelectedDate`、`BlackoutDates`、范围选择、Decade 等）映射进新 Calendar。
- 不用运行时反射发现 API、Token 或模板；AXAML 绑定、静态注册和生成资源必须保持 NativeAOT 友好。
- Automation 的跨平台契约以 `CalendarView` 的 `Table` + `ISelectionProvider` 和 Cell 的 `ListItem` + `ISelectionItemProvider` 为准；Cell 的 `SelectionContainer` 返回 View provider，不宣称 Avalonia 当前未公开的跨平台 GridItem provider。
- LunarCalendar 不改变 Calendar 的事件顺序、模板优先级、键盘拓扑、RangeBars 选择隔离或 Automation owner；普通 Calendar 的默认呈现 adapter 必须保持现有视觉和行为。
- LunarCalendar 的算法支持范围只有在农历年数据、二十四节气数据和全范围验证同时扩展后才能调整；法定节假日政策数据始终由应用 Provider 负责。

## 8. LLMS 语义区域

| Part | AtomUI 节点 | 职责 | 相关 API | 相关 Token | 稳定性 |
| --- | --- | --- | --- | --- | --- |
| `root` | `Calendar` / `LunarCalendar` | 桌面日历控件根语义区域，承载 public API、状态投影和主题入口。 | `Value`、`Mode`、`Fullscreen`、`ShowWeek`、`ValidRange`、`DisabledDate`、`CellTemplate`、`FullCellTemplate`、`HeaderTemplate` | `CalendarToken`、`LunarCalendarToken` | stable |
| `header` | `PART_HeaderPresenter` / `CalendarHeader` | 年份、月份和模式切换区域。 | `Value`、`Mode`、`Fullscreen`、`ValidRange` | `YearControlWidth`、`MonthControlWidth` | stable |
| `body` | `PART_BodyPresenter` / `PART_CalendarView` / `CalendarView` | 日期/月网格、周标题与 overlay 承载区域。 | `Value`、`Mode`、`Fullscreen`、`ShowWeek`、`ValidRange`、`DisabledDate` | `FullBg`、`FullPanelBg`、`MiniContentHeight`、`FullCellMinHeight` | stable |
| `content` | `PART_CellHost` | 日期、月份和周序号容器区域。 | `Value`、`Mode`、`ShowWeek`、`ValidRange`、`DisabledDate` | `ItemActiveBg` | stable |
| `item` | `CalendarViewCell` / `PART_Item` | 单个日期、月份或周序号单元。 | `CellTemplate`、`FullCellTemplate`、`Value`、`Mode`、`ShowWeek` | `ItemActiveBg` | stable |
| `rangeBar` | `PART_RangeBarPanel` / `CalendarRangeBarPanel` | Fullscreen Month 日期网格上方的连续日期范围条 overlay。 | `RangeBars`、`CalendarRangeBar` | `RangeBarHeight` | stable |
| `itemContent` | `PART_ItemContent` | CellTemplate / FullCellTemplate 的业务内容区域。 | `CellTemplate`、`FullCellTemplate`、`CalendarCellContext` | `ItemActiveBg` | stable |
| `lunarContent` | `LunarCalendarViewCell` | 农历日期、节气、传统节日、节假日/调休标记和月份相交信息。 | `LunarCalendarCellContext`、`HolidayProvider`、四个显示开关 | LunarCalendar 增量 Token | stable |

## 9. 文档导航、LLMS 导出与验证策略

日期/月/周算法、Cell 模板优先级、键盘导航、Automation、资源生命周期与性能边界集中记录在 [Calendar 行为设计](behavior-design.md)。农历模型、算法、Provider、大日历/卡片模式和专用主题集中记录在 [LunarCalendar 农历能力设计](lunar-calendar-design.md)。范围条的公共模型、overlay 坐标算法、模板层和非 Visual 资源宿主生命周期集中记录在 [Calendar 范围条设计](range-bar-design.md)。这些专题文档不能替代本 overview 的公共契约摘要。

关联文档：

- [Calendar 桌面版实现原理](implementation.md)
- [Calendar 行为设计](behavior-design.md)
- [LunarCalendar 农历能力设计](lunar-calendar-design.md)
- [Calendar 范围条设计](range-bar-design.md)
- [Calendar Token 设计](token.md)
- [Calendar Changelog](changelog.md)

LLMS 导出来源：

| LLMS 内容 | 来源 | 说明 |
| --- | --- | --- |
| 单控件完整文档 | `overview.md` + `implementation.md` + `behavior-design.md` + `lunar-calendar-design.md` + `range-bar-design.md` + `token.md` + Gallery ShowCase | 生成 `controls/calendar/index-cn.md`。 |
| 单控件语义文档 | `overview.md` + `implementation.md` + `lunar-calendar-design.md` + `range-bar-design.md` + `Themes/` | 生成 `controls/calendar/semantic-cn.md`。 |
| API 表 | overview 的 API 摘要 + 源码 public surface | 不在 `docs/AI/llms` 中手工维护第二份契约。 |
| Token 表 | `token.md` + `CalendarToken` + `LunarCalendarToken` + AXAML 引用 | 以 Token 源码和主题消费点为准。 |
| 示例 | Gallery API/Token/ShowCase | 只引用稳定的 Gallery 用法。 |

验证要求：

- 文档改动运行 `git diff --check`，并检查本目录及新增专题文档的相对链接。
- API/行为改动覆盖默认值、事件顺序、范围和禁用、模板优先级、周序号选择、键盘导航与语言切换。
- Theme 改动检查四个 ControlTheme、伪类、Token 资源以及 Light/Dark 和 Fullscreen/Mini。
- 不手工编辑 `docs/AI/llms` 生成产物；LLMS 源文件变化后运行仓库提供的生成/verify 命令。
