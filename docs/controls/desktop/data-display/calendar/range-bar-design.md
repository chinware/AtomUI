# Calendar 范围条设计

本文档定义 `Calendar` 内置范围条的公共模型、overlay 渲染层、网格坐标算法、资源生命周期和验证边界。控件总体设计见 [Calendar 桌面版架构设计](overview.md)，源码职责与维护入口见 [Calendar 桌面版实现原理](implementation.md)，日期面板行为见 [Calendar 行为设计](behavior-design.md)，视觉变量见 [Calendar Token 设计](token.md)。

## 1. 设计定位

范围条是 Calendar 在日期面板中表达连续日期业务标记的基础能力。应用只声明日期区间、颜色、高度和标签，Calendar 在日期网格上方的独立 overlay 层统一绘制横向条段，并保持 Cell 的选择、禁用、焦点、Automation 和模板语义。

该设计覆盖：

- 单日、跨日、跨周、跨月但仍落入当前 6 行日期网格的范围标记。
- 多个范围在同一日期网格中的稳定排序、分段、叠放和行边界截断。
- 范围条 overlay 与 `CellTemplate`、日期值、周序号列和自动隐藏式滚动条之间的层级关系。
- `Background` 等 Avalonia 属性承载 DynamicResource、TokenResource 和普通 binding 时的资源宿主生命周期。

范围条不是通用日程排布控件。Calendar 不负责拖拽排期、重叠冲突编辑、全天/时间段混排、虚拟化日程列表或甘特图能力；这类业务仍由应用或更专门的控件承担。

## 2. 设计原则

1. `Calendar` 是 `RangeBars` 集合和资源宿主 attach/release 的 owner。
2. `CalendarRangeBar` 只描述一个连续日期区间，不持有生成出来的视觉元素、Cell 或 Gallery 对象。
3. 范围条只投影到 Month 模式的 Fullscreen 日期网格；Year 模式、周序号 Cell 和 Mini 密度不渲染范围条。
4. 范围条使用独立 overlay panel 统一绘制，不进入每个 `CalendarViewCell` 的模板树。
5. `CalendarView` 继续拥有日期/月/周序号 Cell 的布局、选择、焦点、键盘和 Automation。
6. `CellTemplate` 仍只负责业务内容区域；范围条 overlay 与 Cell 内容并列叠放。
7. `FullCellTemplate` 只接管 Cell 内部内容，不替换根 body overlay 层；是否显示范围条由 `RangeBars` 和模式策略决定。
8. 范围条不得改变 Cell 网格的行列数、Cell 外边距、选择状态、禁用状态或事件顺序。
9. 横向连续由 overlay 坐标计算和行内分段完成，所有几何逻辑集中在范围条 overlay 层。
10. 范围条 overlay 不参与命中测试；Cell 仍是鼠标指针、Pointer、键盘和 Automation 的唯一交互容器。

## 3. 专项模型与 Public API

### 3.1 Calendar.RangeBars

`Calendar.RangeBars` 是 Calendar 持有的范围条集合，默认为空。集合使用可观察列表语义，支持 XAML property element 写法：

```xml
<atom:Calendar Value="{Binding CrossDateEventsSampleDate}">
    <atom:Calendar.RangeBars>
        <atom:CalendarRangeBar StartDate="2026-01-08"
                               EndDate="2026-01-10"
                               Label="{gallery:CalendarShowCaseLangResource CrossDateEventsReleaseText}"
                               Background="{atom:SharedTokenResource ColorPrimary}" />
    </atom:Calendar.RangeBars>
</atom:Calendar>
```

集合顺序是稳定输入顺序，用于 lane 分配的 tie-breaker 和同层绘制顺序。集合替换、Add、Remove、Move、Reset 或条目属性变化都会使 overlay 失效；日期 Cell model 不因为纯视觉范围条变化重新计算。

### 3.2 CalendarRangeBar

`CalendarRangeBar` 是 owner-managed 的非 Visual `AvaloniaObject`，声明为 `partial` 并使用 `[GenerateScopedResourceHost]`。它的业务主文件只保留属性注册和 CLR wrapper；资源宿主、主题变体转发和 `AttachResourceHost(...)` 由 source generator 生成。

| API | 默认值 | 语义 |
| --- | --- | --- |
| `StartDate` | `null` | 区间起始日期，投影时规范化到 `.Date`。 |
| `EndDate` | `null` | 区间结束日期，投影时规范化到 `.Date`。 |
| `Label` | `null` | 显示在范围起点条段内的内容；为空时只绘制色条。 |
| `Background` | `null` | 范围条背景画刷；支持普通 brush、binding、DynamicResource 和 TokenResource。 |
| `Height` | `double.NaN` | 显式条高；非有限正数时使用 `CalendarControlToken.RangeBarHeight`。 |

`StartDate` 或 `EndDate` 缺失时，该范围条不参与投影。`EndDate < StartDate` 时，该范围条按无效输入跳过渲染，不在 XAML 解析或 binding 过渡状态下抛出异常。范围条不改变 `ValidRange`、`DisabledDate` 或 Cell 是否可选；业务需要隐藏无效日期上的标记时，应从数据源侧移除对应范围。

### 3.3 CalendarRangeBarPanel

`CalendarRangeBarPanel` 是 Calendar 内部 overlay panel，不是 public API。它接收 Calendar 的 `Value`、`Mode`、`Fullscreen`、`ShowWeek`、Culture、`RangeBars`、`RangeBarHeight` 和周标题高度，按当前可见月份网格计算所有范围条的绝对位置。

Panel 内部可以复用轻量视觉子元素或自绘几何，但它必须保持以下语义：

- `IsHitTestVisible=false`。
- 不读取或修改 `CalendarViewCell` 的 margin、bounds、DataContext 或模板内容。
- 不通过 Gallery converter、CellTemplate 或字符串 binding 参与布局计算。
- 不保存业务数据 owner；所有输入来自 Calendar 的强类型属性投影。

## 4. 状态与模式策略

| Calendar 状态 | 范围条策略 |
| --- | --- |
| `Mode=Month` 且 `Fullscreen=true` | 在日期网格 overlay 中渲染范围条。 |
| `Mode=Month` 且 `Fullscreen=false` | 不渲染范围条，保持 Mini 的紧凑日期选择语义。 |
| `Mode=Year` | 不渲染范围条。 |
| `ShowWeek=true` | 周序号列不承载范围条；日期列按一列视觉偏移计算。 |
| `CellTemplate != null` | 范围条 overlay 与业务内容共存，默认日期值仍保留。 |
| `FullCellTemplate != null` | FullCell 接管单个 Cell 内部内容；范围条 overlay 仍由 Calendar 根 body 管理。 |
| disabled / outside 日期 Cell | 范围条仍按日期覆盖关系渲染；禁用只影响 Cell 交互和选择。 |

范围条的 label 只显示在实际 `StartDate` 所在的可见条段上。跨周或跨月导致可见行重新起段时，后续行段不重复显示 label，避免同一业务标记在一个月面板中重复命名。

## 5. 架构、文件结构与职责

```text
Calendar
├── RangeBars: CalendarRangeBarCollection
│   └── CalendarRangeBar × N
└── PART_BodyPresenter
    ├── PART_CalendarView
    │   ├── PART_WeekHeader
    │   └── PART_CellHost
    │       └── CalendarViewCell × 42/48/12
    └── PART_RangeBarPanel
        └── CalendarRangeBar visual segment × M
```

| 类型或职责 | owner 与输入 | 输出 |
| --- | --- | --- |
| `Calendar` | `RangeBars` 集合、条目资源宿主、条目属性订阅、Culture 和模板 part 接线。 | 将稳定的范围条输入投影给 `CalendarRangeBarPanel`。 |
| `CalendarRangeBar` | 单个范围条的公开描述属性。 | 不直接生成视觉，只作为 overlay 投影输入。 |
| `CalendarView` | 当前日期网格、周标题、CellHost、容器池、焦点、键盘和 Automation。 | 可交互日期/月/周序号 Cell。 |
| `CalendarRangeBarPanel` | 当前月份锚点、Culture、ShowWeek、Fullscreen、RangeBars、Token metrics 和自身 bounds。 | 所有范围条的行内 segment 位置、尺寸、圆角、label 和绘制顺序。 |
| `CalendarViewCell` | 当前 Cell model、默认值、业务模板、伪类和输入。 | Cell 交互与内容呈现；不计算也不承载范围条。 |

范围条投影应集中在 `CalendarRangeBarPanel` 或其内部纯逻辑 helper 中，不能把分段判断、坐标公式或圆角判断复制到 Gallery converter、主题 converter 或每个 Cell 的模板里。

## 6. Template、组合与集成契约

默认根模板的 body 使用叠放容器组合 `CalendarView` 和范围条 overlay：

```text
Calendar
└── PART_Root
    └── DockPanel
        ├── PART_HeaderPresenter (DockPanel.Dock=Top)
        └── PART_BodyPresenter
            ├── PART_CalendarView
            └── PART_RangeBarPanel
```

`PART_HeaderPresenter` 继续只负责默认 Header 与自定义 Header。`PART_BodyPresenter` 是剩余空间中的叠放层，`PART_CalendarView` 位于底层并负责实际网格；`PART_RangeBarPanel` 位于上层并设置为不消费 pointer。

`CalendarViewCell` 的稳定内部结构不增加范围条 host：

```text
CalendarViewCell
└── PART_Item
    └── PART_CellInner
        ├── PART_Value
        └── PART_ItemContent
```

范围条 overlay 与 `CellTemplate` 的关系：

- 默认日期值在 Cell 顶部保留。
- 范围条 overlay 位于 `CalendarView` 之上，按日期网格坐标绘制，不属于任何单个 Cell 的 DataContext。
- `CellTemplate` 仍使用 `CalendarCellContext` 作为 DataContext，并可以为业务内容提供自动隐藏式滚动条。
- `FullCellTemplate` 替换 Cell 内部内容，但不替换 `PART_BodyPresenter` 或范围条 overlay。

主题必须保证 `PART_BodyPresenter` 与 `PART_CalendarView` 使用相同的可用区域。范围条 panel 根据周标题高度跳过 `PART_WeekHeader` 区域，只覆盖日期 CellHost 区域；它不需要 Cell 横向 overflow，也不允许通过 Cell 外 margin 制造行间视觉缝隙。

## 7. 核心算法、数据流与生命周期

### 7.1 输入与坐标系

输入：

- `Value.Date` 所在自然月。
- 当前 Culture 的 `FirstDayOfWeek`。
- `Mode`、`Fullscreen` 和 `ShowWeek`。
- `PART_BodyPresenter` / `CalendarRangeBarPanel` 的可用尺寸。
- 周标题高度，默认来自 SharedToken 的 `ControlHeightSM`。
- `CalendarControlToken.RangeBarHeight` 和单条 `CalendarRangeBar.Height`。
- 有效 `CalendarRangeBar` 集合快照。

输出：

- overlay 内每个可见行段的 `Rect`、lane、corner radius、background、label 和 label visibility。
- 所有视觉 segment 的 z-order 与集合顺序保持稳定。

坐标系使用 `PART_RangeBarPanel` 的本地坐标。横向按完整可见列计算：`ShowWeek=false` 时共有 7 列日期，`ShowWeek=true` 时共有 8 列且日期列从第 2 列开始。纵向先扣除周标题高度，再把日期区分成 6 行。周序号列和周标题区不绘制范围条。

### 7.2 可见日期网格

overlay 必须复用与 `CalendarViewCellBuilder` 等价的日期网格算法，避免视觉条段与 Cell 日期错位：

1. 取 `Value.Date` 所在月的月首。
2. 根据 Culture 的 `FirstDayOfWeek` 计算月首前需要回退的天数。
3. 得到 42 天日期网格起点；靠近 `DateTime.MinValue/MaxValue` 时按 CalendarView 的边界策略夹紧，避免日期运算溢出。
4. 可见网格为起点起连续 42 天，按 6 行 7 日期列排列。

`ShowWeek` 只改变视觉列偏移，不改变这 42 个日期的顺序。

### 7.3 分段与 lane 分配

1. 规范化每个范围条的 `StartDate`、`EndDate` 和 `Height`，跳过缺失、反向或与当前可见日期网格完全不相交的条目。
2. 按集合顺序处理有效范围条，并用日期区间重叠关系分配 lane。日期区间相交的范围条不能共享 lane；不相交的范围条可以复用较小 lane。
3. 对每个有效范围按周行拆分：每行最多生成一个横向 segment。
4. segment 起止日期夹紧到当前行的 7 天日期范围和可见 42 天范围内。
5. 只在 `segmentStart == range.StartDate.Date` 的可见 segment 上显示 label。
6. segment 的左/右圆角只由真实范围起点/终点决定。跨周换行处属于可见行截断边界，不生成圆角。

该算法按可见行截断范围条。跨周范围会在每一行形成独立横向条段，行尾和下一行行首不通过垂直或斜向视觉连接。

### 7.4 视觉公式

基础几何：

```text
totalColumns = ShowWeek ? 8 : 7
dateColumnOffset = ShowWeek ? 1 : 0
weekHeaderHeight = SharedToken.ControlHeightSM
dateAreaHeight = panelHeight - weekHeaderHeight
cellWidth = panelWidth / totalColumns
cellHeight = dateAreaHeight / 6
effectiveHeight = finite Height > 0 ? Height : CalendarControlToken.RangeBarHeight
```

行段位置：

```text
rowStartDate = gridStart + row * 7 days
rowEndDate = rowStartDate + 6 days
segmentStart = max(rangeStart, rowStartDate, gridStart)
segmentEnd = min(rangeEnd, rowEndDate, gridStart + 41 days)
startColumn = (segmentStart - rowStartDate).Days
endColumn = (segmentEnd - rowStartDate).Days
x = (dateColumnOffset + startColumn) * cellWidth
width = (endColumn - startColumn + 1) * cellWidth
y = weekHeaderHeight + row * cellHeight + laneOffset
```

`laneOffset` 由日期值下方的范围条可用区域决定，必须保持同一 row 内各 lane 垂直连续且不制造 Cell 外间距。默认条高来自 `CalendarControlToken.RangeBarHeight`；单条 `Height` 只覆盖自身高度。条间距、圆角和 label padding 从 SharedToken 派生，作为内部 metrics，不新增额外 public API。

FlowDirection 为 RTL 时，视觉 x 坐标按 panel 宽度镜像；日期顺序、日期比较和事件语义不变。圆角也按视觉 inline-start/inline-end 镜像。

## 8. 资源、性能与 AOT 边界

- `CalendarRangeBar` 使用 `[GenerateScopedResourceHost]`，避免手写 `IResourceHost` / `IThemeVariantHost` 样板代码。
- `Background` 等资源绑定先走 Calendar owner resource host，再 fallback 到 `Application.Current`。
- Calendar 负责 attach/release；条目不能持有生成视觉、Cell、ViewModel 或 Gallery 引用。
- `CalendarRangeBarPanel` 只在输入、尺寸或主题 metrics 失效时重新排布，不在 pointer move 热路径中计算。
- overlay 复杂度以当前可见 42 个日期和有效范围条数量为界；不需要遍历 Cell visual tree。
- 设计使用静态 Avalonia 属性、强类型 model 和显式订阅；不引入字符串 binding、运行时反射、assembly scan 或动态类型发现。

## 9. 兼容性与定制边界

稳定契约包括 `Calendar.RangeBars`、`CalendarRangeBar` 的五个公开属性、`CalendarControlToken.RangeBarHeight`、范围条在 Fullscreen Month 日期网格 overlay 中的渲染语义、Cell 外层交互保留和资源宿主生命周期。

应用可以通过 `RangeBars` 数据控制日期、颜色、高度和 label；可以继续使用 `CellTemplate` 添加日期内业务内容；也可以使用 `FullCellTemplate` 完整替换 Cell 内部视觉。范围条 overlay 不读取业务模板内部结构，因此应用不需要在模板中手写跨日期连接算法。

范围条只新增默认高度 Token。单条业务颜色和高度由 `CalendarRangeBar.Background` 与 `CalendarRangeBar.Height` 控制；条间距、圆角、label padding 和 motion 保持内部 SharedToken 派生 metrics，不作为独立 Calendar Token 暴露。

## 10. 验证要求

- 纯逻辑：单日、跨日、跨周、跨月可见区间、反向区间、空端点、ShowWeek 列偏移、lane 分配、DateTime 边界和 FlowDirection 镜像。
- 控件行为：RangeBars 集合 Add/Remove/Move/Reset、单个 RangeBar 属性变化、Value/Mode/Fullscreen/ShowWeek/Culture 切换后的 overlay 更新。
- Template/主题：overlay 与 CalendarView 完全重叠，周标题区不绘制范围条，日期 Cell 横向连续、无可见垂直 Cell 外间距、hover/selected/today/focused/disabled 状态不被范围条覆盖。
- 资源生命周期：`Background` 使用 DynamicResource/TokenResource 时，RangeBar remove/reset/detach 后释放 owner resource host，不保留 Calendar 或 Gallery。
- 交互与 Automation：范围条不改变 Cursor、不截获 Pointer、不改变 Cell 选择事件顺序、不改变 SelectionProvider/SelectionItemProvider 语义。
- Gallery/LLMS/AOT：跨日期事件示例使用 `RangeBars` 展示 public API；LLMS 从 overview、implementation、本文和 Gallery 示例生成；NativeAOT 路径不出现反射或字符串 binding 新风险。
