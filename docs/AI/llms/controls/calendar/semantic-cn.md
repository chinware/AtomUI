# Calendar 语义结构

> 生成产物：由源文档生成，不要手工编辑。修改内容请回到控件文档、源码 public surface、Token 类型或生成数据、Gallery ShowCase 或源码结构。

## Semantic Parts

| Part | AtomUI 节点 | 职责 | 相关 API | 相关 Token | 稳定性 |
| --- | --- | --- | --- | --- | --- |
| `root` | `Calendar` | 桌面日历控件根语义区域，承载 public API、状态投影和主题入口。 | `Value`、`Mode`、`Fullscreen`、`ShowWeek`、`ValidRange`、`DisabledDate`、`CellTemplate`、`FullCellTemplate`、`HeaderTemplate` | `CalendarControlToken` | stable |
| `header` | `PART_HeaderPresenter` / `CalendarHeader` | 年份、月份和模式切换区域。 | `Value`、`Mode`、`Fullscreen`、`ValidRange` | `YearControlWidth`、`MonthControlWidth` | stable |
| `body` | `PART_BodyPresenter` / `PART_CalendarView` / `CalendarView` | 日期/月网格、周标题与 overlay 承载区域。 | `Value`、`Mode`、`Fullscreen`、`ShowWeek`、`ValidRange`、`DisabledDate` | `FullBg`、`FullPanelBg`、`MiniContentHeight`、`FullCellMinHeight` | stable |
| `content` | `PART_CellHost` | 日期、月份和周序号容器区域。 | `Value`、`Mode`、`ShowWeek`、`ValidRange`、`DisabledDate` | `ItemActiveBg` | stable |
| `item` | `CalendarViewCell` / `PART_Item` | 单个日期、月份或周序号单元。 | `CellTemplate`、`FullCellTemplate`、`Value`、`Mode`、`ShowWeek` | `ItemActiveBg` | stable |
| `rangeBar` | `PART_RangeBarPanel` / `CalendarRangeBarPanel` | Fullscreen Month 日期网格上方的连续日期范围条 overlay。 | `RangeBars`、`CalendarRangeBar` | `RangeBarHeight` | stable |
| `itemContent` | `PART_ItemContent` | CellTemplate / FullCellTemplate 的业务内容区域。 | `CellTemplate`、`FullCellTemplate`、`CalendarCellContext` | `ItemActiveBg` | stable |

## Abstract AXAML Structure

来源：`src/AtomUI.Desktop.Controls/Calendar/Themes/CalendarTheme.axaml`

```xml
<Border Name="PART_Root">
    <DockPanel>
        <Panel Name="PART_HeaderPresenter">
            <CalendarHeader Name="PART_DefaultHeader" />
            <ContentControl Name="PART_CustomHeader" />
        </Panel>
        <Panel Name="PART_BodyPresenter">
            <CalendarView Name="PART_CalendarView" />
            <CalendarRangeBarPanel Name="PART_RangeBarPanel" />
        </Panel>
    </DockPanel>
</Border>
```

## Composition Model

该章节由控件 `Themes/` 文件夹中的真实主题文件生成，用于说明 public 控件与内部协作对象之间的运行时结构。内部节点只用于理解和维护，不应指导用户代码直接依赖。

### 控件角色图

```text
Calendar
  -> CalendarHeader (control theme, CalendarHeaderTheme.axaml)
     -> Border (template-stable)
        -> StackPanel (template-stable)
           -> ComboBox#PART_YearSelect (template-stable)
           -> ComboBox#PART_MonthSelect (template-stable)
           -> Segmented#PART_ModeSwitch (template-stable)
              -> SegmentedItem (template-stable)
              -> SegmentedItem (template-stable)
  -> Calendar (control theme, CalendarTheme.axaml)
     -> Border#PART_Root (template-stable)
        -> DockPanel (template-stable)
           -> Panel#PART_HeaderPresenter (template-stable)
              -> CalendarHeader#PART_DefaultHeader (template-stable)
              -> ContentControl#PART_CustomHeader (template-stable)
           -> Panel#PART_BodyPresenter (template-stable)
              -> CalendarView#PART_CalendarView (template-stable)
              -> CalendarRangeBarPanel#PART_RangeBarPanel (template-stable)
  -> CalendarViewCell (control theme, CalendarViewCellTheme.axaml)
     -> Border#PART_Item (template-stable)
        -> Border#PART_CellInner (template-stable)
           -> Grid (template-stable)
              -> ContentControl#PART_ItemContent (template-stable)
              -> TextBlock#PART_Value (template-stable)
  -> CalendarView (control theme, CalendarViewTheme.axaml)
     -> DockPanel#PART_Body (template-stable)
        -> Grid#PART_WeekHeader (template-stable)
        -> Grid#PART_CellHost (template-stable)
```

### 协作节点

| 节点 | 类型 | 来源 | 生命周期 owner | 影响的 public API | 稳定性 | Agent 使用边界 |
| --- | --- | --- | --- | --- | --- | --- |
| `Calendar` | public control | `源文档 + public API` | 用户代码 / 控件宿主 | public API | public | 用户可直接使用 public 控件；可作为示例和 API 入口。 |
| `CalendarHeader` | control theme | `CalendarHeaderTheme.axaml` | Calendar | 主题状态 / visual state | template-stable | 用于主题维护；变更需同步主题、实现和 LLMS。 |
| `StackPanel` | template node (StackPanel) | `CalendarHeaderTheme.axaml` | CalendarHeader | 主题状态 / visual state | template-stable | 用于主题维护；变更需同步主题、实现和 LLMS。 |
| `PART_YearSelect` | template node (ComboBox) | `CalendarHeaderTheme.axaml` | CalendarHeader | 主题状态 / visual state | template-stable | 用于主题维护；变更需同步主题、实现和 LLMS。 |
| `PART_MonthSelect` | template node (ComboBox) | `CalendarHeaderTheme.axaml` | CalendarHeader | 主题状态 / visual state | template-stable | 用于主题维护；变更需同步主题、实现和 LLMS。 |
| `PART_ModeSwitch` | template node (Segmented) | `CalendarHeaderTheme.axaml` | CalendarHeader | 主题状态 / visual state | template-stable | 用于主题维护；变更需同步主题、实现和 LLMS。 |
| `SegmentedItem` | template node (SegmentedItem) | `CalendarHeaderTheme.axaml` | CalendarHeader | 主题状态 / visual state | template-stable | 用于主题维护；变更需同步主题、实现和 LLMS。 |
| `Calendar` | control theme | `CalendarTheme.axaml` | 用户代码 / 控件宿主 | `CellTemplate`, `DisabledDate`, `FullCellTemplate`, `Fullscreen`, `HeaderTemplate`, `Mode` | public | 用户可直接使用 public 控件；可作为示例和 API 入口。 |
| `PART_Root` | template node (Border) | `CalendarTheme.axaml` | Calendar | `CellTemplate`, `DisabledDate`, `FullCellTemplate`, `Fullscreen`, `HeaderTemplate`, `Mode` | template-stable | 用于主题维护；变更需同步主题、实现和 LLMS。 |
| `DockPanel` | template node (DockPanel) | `CalendarTheme.axaml` | Calendar | `CellTemplate`, `DisabledDate`, `FullCellTemplate`, `Fullscreen`, `HeaderTemplate`, `Mode` | template-stable | 用于主题维护；变更需同步主题、实现和 LLMS。 |
| `PART_HeaderPresenter` | template node (Panel) | `CalendarTheme.axaml` | Calendar | `Fullscreen`, `HeaderTemplate`, `Mode`, `ValidRange`, `Value` | template-stable | 用于主题维护；变更需同步主题、实现和 LLMS。 |
| `PART_DefaultHeader` | template node (CalendarHeader) | `CalendarTheme.axaml` | Calendar | `Fullscreen`, `Mode`, `ValidRange`, `Value` | template-stable | 用于主题维护；变更需同步主题、实现和 LLMS。 |
| `PART_CustomHeader` | template node (ContentControl) | `CalendarTheme.axaml` | Calendar | `HeaderTemplate` | template-stable | 用于主题维护；变更需同步主题、实现和 LLMS。 |
| `PART_BodyPresenter` | template node (Panel) | `CalendarTheme.axaml` | Calendar | `CellTemplate`, `DisabledDate`, `FullCellTemplate`, `Fullscreen`, `Mode`, `RangeBars` | template-stable | 用于主题维护；变更需同步主题、实现和 LLMS。 |
| `PART_CalendarView` | template node (CalendarView) | `CalendarTheme.axaml` | Calendar | `CellTemplate`, `DisabledDate`, `FullCellTemplate`, `Fullscreen`, `ShowWeek`, `ValidRange` | template-stable | 用于主题维护；变更需同步主题、实现和 LLMS。 |
| `PART_RangeBarPanel` | template node (CalendarRangeBarPanel) | `CalendarTheme.axaml` | Calendar | `Fullscreen`, `Mode`, `RangeBars`, `ShowWeek`, `Value` | template-stable | 用于主题维护；变更需同步主题、实现和 LLMS。 |
| `CalendarViewCell` | control theme | `CalendarViewCellTheme.axaml` | Calendar | `DisplayText` | template-stable | 用于主题维护；变更需同步主题、实现和 LLMS。 |
| `PART_Item` | template node (Border) | `CalendarViewCellTheme.axaml` | CalendarViewCell | `DisplayText` | template-stable | 用于主题维护；变更需同步主题、实现和 LLMS。 |
| `PART_CellInner` | template node (Border) | `CalendarViewCellTheme.axaml` | CalendarViewCell | `DisplayText` | template-stable | 用于主题维护；变更需同步主题、实现和 LLMS。 |
| `PART_ItemContent` | template node (ContentControl) | `CalendarViewCellTheme.axaml` | CalendarViewCell | 主题状态 / visual state | template-stable | 用于主题维护；变更需同步主题、实现和 LLMS。 |
| `PART_Value` | template node (TextBlock) | `CalendarViewCellTheme.axaml` | CalendarViewCell | `DisplayText` | template-stable | 用于主题维护；变更需同步主题、实现和 LLMS。 |
| `CalendarView` | control theme | `CalendarViewTheme.axaml` | Calendar | 主题状态 / visual state | template-stable | 用于主题维护；变更需同步主题、实现和 LLMS。 |
| `PART_Body` | template node (DockPanel) | `CalendarViewTheme.axaml` | CalendarView | 主题状态 / visual state | template-stable | 用于主题维护；变更需同步主题、实现和 LLMS。 |
| `PART_WeekHeader` | template node (Grid) | `CalendarViewTheme.axaml` | CalendarView | 主题状态 / visual state | template-stable | 用于主题维护；变更需同步主题、实现和 LLMS。 |
| `PART_CellHost` | template node (Grid) | `CalendarViewTheme.axaml` | CalendarView | 主题状态 / visual state | template-stable | 用于主题维护；变更需同步主题、实现和 LLMS。 |

## Template Parts

源文档未声明稳定 Template Part。维护模板时应以源码和主题文件中的实际声明为准。

## Pseudo Classes

| 状态反馈 | API、内部状态与伪类如何形成反馈。 | `today`、`selected`、`outside`、`disabled`、`focused`、`fullscreen`、`mini`、`show-week`。 |
| 主题语义 | SharedToken、CalendarControlToken、ControlTheme 与模板如何表达视觉。 | 四个 Calendar ControlTheme 消费共享 Token 与八个 Calendar 专属 Token。 |

设计上的首要不变量是：Cell 定制不能夺走日期值、选中、禁用、焦点和命中测试语义；这些语义由 Cell 容器保留，模板只改变内容呈现方式。

## State Flow

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

## Theme and Token Boundaries

Calendar 使用 `CalendarControlToken`（scope id `CalendarControl`）以及 SharedToken。专属 Token 只表达八个组件视觉语义：`FullBg`、`FullPanelBg`、`ItemActiveBg`、`YearControlWidth`、`MonthControlWidth`、`MiniContentHeight`、`FullCellMinHeight`、`RangeBarHeight`。运行时状态通过伪类和 selector 表达，不写入 Token。

| Theme 文件 | 稳定职责 |
| --- | --- |
| `CalendarTheme.axaml` | 根背景、Header/CustomHeader 选择、CalendarView 与范围条 overlay 接线；Fullscreen 拉伸且无紧凑边框，Mini 使用圆角边框。 |
| `CalendarHeaderTheme.axaml` | Year Select、Month Select、Month/Year 模式切换。Mini 时 Header 交互控件应使用 Small 尺寸。 |
| `CalendarViewTheme.axaml` | WeekHeader、CellHost，以及 Fullscreen/Mini 的布局差异。 |
| `CalendarViewCellTheme.axaml` | 默认日期值、Cell/FullCell 模板消费、状态 selector 和命中测试视觉。 |

`CellTemplate` 必须保留默认值显示，并与内置范围条 overlay 共存；`FullCellTemplate` 覆盖完整 Cell 内部内容且优先级最高，但不替换 Calendar body overlay。两者都不能删除禁用、选中、焦点和 outside 的容器状态。

Token 边界：

新 Calendar 的 Token 收敛为八个公开视觉语义。日期值、周标题、范围条间距、范围条圆角、Padding、Border、Typography 与 Motion 均从 SharedToken 派生；Fullscreen 单元最小高度通过 `FullCellMinHeight` 固化 Calendar 完整单元的测量规则，范围条默认高度通过 `RangeBarHeight` 固化 Calendar overlay 的默认条高。

当前 Token scope：

- `CalendarControlToken`，scope id 为 `CalendarControl`，源码位于 `src/AtomUI.Desktop.Controls/Calendar/CalendarControlToken.cs`。

> 注意：旧 `CalendarToken`（scope id `Calendar`）现归 DatePicker 的 CalendarView 使用，与新 Calendar 无关。新 Calendar 通过 `CalendarControlTokenResource` 引用自己的 Token，两者完全独立。

## Customization Boundaries

- 不擅自新增、删除或重命名 public 属性、事件、上下文类型、枚举成员、模板 part、伪类、ControlTheme key 或 Token。
- `Value` 的日期规范化、用户事件顺序、`FullCellTemplate` 优先级和月份两端禁用规则属于行为兼容契约。
- `RangeBars` 不改变 Cell 外间距、网格行列、选择/禁用语义、事件顺序和 Automation；`CalendarRangeBar.Background` 的资源绑定必须跟随 Calendar owner 生命周期释放。
- 模板重应用、模式切换、语言切换和控件 detach 必须释放旧事件订阅、清理旧容器 owner，并把当前状态回放到新模板。
- 不把 DatePicker 的旧 Calendar API（`SelectedDate`、`BlackoutDates`、范围选择、Decade 等）映射进新 Calendar。
- 不用运行时反射发现 API、Token 或模板；AXAML 绑定、静态注册和生成资源必须保持 NativeAOT 友好。
- Automation 的跨平台契约以 `CalendarView` 的 `Table` + `ISelectionProvider` 和 Cell 的 `ListItem` + `ISelectionItemProvider` 为准；Cell 的 `SelectionContainer` 返回 View provider，不宣称 Avalonia 当前未公开的跨平台 GridItem provider。

维护不变量：

- Calendar 是唯一 public 状态 owner；View/Cell 不得引入第二份可写 Value。
- `Value` 永远是日期值；所有提交和上下文值均不携带时间部分。
- `FullCellTemplate` 优先于 `CellTemplate`，但两者都保留 Cell 状态和交互语义。
- `RangeBars` 只进入 Fullscreen Month 日期网格 overlay 层，不改变 Cell 外间距、Pointer、键盘、Automation 或选择事件顺序。
- Month 禁用使用月首/月末范围判断；方向键跳过禁用和周序号 Cell。
- 默认 Header、自定义 Header、Cell Pointer、键盘和 Automation 使用同一提交与事件顺序。
- 新 Calendar 与 DatePicker 旧 CalendarView 的类型、Token、Theme key 和生命周期互不越界。
- 改动 ControlTheme、伪类、Token、Template part 或 Automation 时，必须同步 Gallery、测试和本目录文档。
