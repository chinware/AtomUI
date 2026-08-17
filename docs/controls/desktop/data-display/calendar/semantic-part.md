# Calendar Semantic Part 契约

本文档定义 `Calendar` / `LunarCalendar` 对应用公开的 Semantic Part、选择器、类型约束、数量语义和定制边界。Calendar 的整体设计见
[Calendar 桌面版架构设计](overview.md)，真实模板与生命周期见 [Calendar 桌面版实现原理](implementation.md)，系统级规则见
[AtomUI Semantic Part 系统设计](../../../../architecture/systems/theming/semantic-parts.md)。

## 1. Semantic Parts

`Calendar` 主控件公开 `root`、`header`、`body`、`content`、`item` 与 `itemContent` 六个职责区域，与上游稳定 Semantic DOM
对齐。上游基线为 6.6.0 稳定发布的 `CalendarSemanticType` 与 Semantic DOM 演示：

- `root`、`header`、`body`、`content`、`item` 自上游 6.0.0 公开；
- `itemContent` 自上游 6.4.0 公开（Semantic DOM 演示中 `itemContent` 的 version 为 `6.4.0`）。

AtomUI 全部六个 Part 随本次 Semantic Part 改造同时公开，descriptor 的 `Since` 统一为 `6.0`。

内部 `CalendarHeader`、`CalendarView`、`CalendarViewCell` 与 `LunarCalendarViewCell` 均不持有独立 Semantic descriptor：

- 上游 Calendar 只提供一个 owner 的 Semantic DOM；这四个类型是 internal 模板协作类型，不是对应用公开的独立 owner。
- `CalendarHeader` 的职责通过 `Calendar` 的 `header` Part 对外公开，并以 `TemplatedControl` 作为最低 ContractType。
- `CalendarView` 的职责通过 `Calendar` 的 `body` 与 `content` Part 对外公开。
- `CalendarViewCell` / `LunarCalendarViewCell` 是运行时生成的网格单元，职责通过 `item` 与 `itemContent` Part 对外公开。

`LunarCalendar` 是 `Calendar` 的公开派生控件，按批次既有约定声明自己的 descriptor（与 `FloatButton` /
`BackTopFloatButton` 一致）。它复用同一套 Part 名称、selector class 与 ContractType，marker 由继承的根模板与派生
Cell 模板承载。

### 1.1 `Calendar`

#### `root`

| 字段 | 值 |
| --- | --- |
| Owner | `Calendar` |
| Part | `root` |
| Selector | Calendar 本身 |
| SelectorRoute | 不适用 |
| Style Type | 不适用（root 不生成 Style） |
| ContractType | `Calendar` |
| Cardinality | `Single` |
| Customization | `Root` |
| CrossVisualRoot | `false` |
| RuntimeCreated | `false` |
| AtomUI 节点 | Calendar owner |
| 职责 | Calendar root 是日期值、显示模式、面板状态与根视觉样式的统一 owner。 |
| 相关 API | `Value`、`Mode`、`Fullscreen`、`ShowWeek`、`ValidRange`、`DisabledDate`、`CellTemplate`、`FullCellTemplate`、`HeaderTemplate`、`RangeBars` |
| 相关 Token | CalendarToken、SharedToken |
| 稳定性 | stable since 6.0 |

#### `header`

| 字段 | 值 |
| --- | --- |
| Owner | `Calendar` |
| Part | `header` |
| Selector | `.semantic-header` |
| SelectorRoute | `/template/ .semantic-header` |
| Style Type | `CalendarHeaderStyle` |
| ContractType | `TemplatedControl` |
| Cardinality | `Single` |
| Customization | `Selector` |
| CrossVisualRoot | `false` |
| RuntimeCreated | `false` |
| AtomUI 节点 | 默认 Header `CalendarHeader`（`PART_DefaultHeader`） |
| 职责 | 统一表示年份选择、月份选择与 Month/Year 模式切换的 Header 区域布局与样式；年/月 Select 与模式切换组默认带白色容器背景（`ColorBgContainer`），选中态仅以主色边框/文字标识。 |
| 相关 API | `Value`、`Mode`、`Fullscreen`、`ValidRange`、`HeaderTemplate` |
| 相关 Token | `YearControlWidth`、`MonthControlWidth`、SharedToken |
| 稳定性 | stable since 6.0 |

#### `body`

| 字段 | 值 |
| --- | --- |
| Owner | `Calendar` |
| Part | `body` |
| Selector | `.semantic-body` |
| SelectorRoute | `/template/ .semantic-body` |
| Style Type | `CalendarBodyStyle` |
| ContractType | `DockPanel` |
| Cardinality | `Single` |
| Customization | `Selector` |
| CrossVisualRoot | `false` |
| RuntimeCreated | `false` |
| AtomUI 节点 | `DockPanel#PART_BodyPresenter` |
| 职责 | 统一表示 Header 下方容纳日历网格与范围条 overlay 的主体区域的内边距、背景与布局。 |
| 相关 API | `Fullscreen`、`Mode`、`ShowWeek`、`RangeBars` |
| 相关 Token | `FullBg`、`FullPanelBg`、SharedToken |
| 稳定性 | stable since 6.0 |

#### `content`

| 字段 | 值 |
| --- | --- |
| Owner | `Calendar` |
| Part | `content` |
| Selector | `.semantic-content` |
| SelectorRoute | `/template/ .semantic-content` |
| Style Type | `CalendarContentStyle` |
| ContractType | `TemplatedControl` |
| Cardinality | `Single` |
| Customization | `Selector` |
| CrossVisualRoot | `false` |
| RuntimeCreated | `false` |
| AtomUI 节点 | 日历面板 `CalendarView`（`PART_CalendarView`） |
| 职责 | 统一表示日历表格（周标题行 + 日期/月网格）区域的宽度、高度与表格级样式。面板默认自带 `FullPanelBg`（`ColorBgContainer`）背景，Fullscreen 模式面板背景为 `FullBg`；root 表面的背景定制只落在面板外圈，不渗入面板内部。 |
| 相关 API | `Value`、`Mode`、`Fullscreen`、`ShowWeek`、`ValidRange`、`DisabledDate`、`CellTemplate`、`FullCellTemplate` |
| 相关 Token | `FullPanelBg`、`MiniContentHeight`、`FullCellMinHeight`、SharedToken |
| 稳定性 | stable since 6.0 |

#### `item`

| 字段 | 值 |
| --- | --- |
| Owner | `Calendar` |
| Part | `item` |
| Selector | `.semantic-item` |
| SelectorRoute | `/template/ .semantic-content > .semantic-scope-body > .semantic-scope-cells > .semantic-item` |
| Style Type | `CalendarItemStyle` |
| ContractType | `TemplatedControl` |
| Cardinality | `Multiple` |
| Customization | `Selector` |
| CrossVisualRoot | `false` |
| RuntimeCreated | `true` |
| AtomUI 节点 | 运行时生成的 `CalendarViewCell` 网格单元（含周序号 Cell） |
| 职责 | 统一表示单个日期、月份或周序号单元的背景、边框、悬停与选中等交互样式。 |
| 相关 API | `Value`、`Mode`、`ShowWeek`、`ValidRange`、`DisabledDate` |
| 相关 Token | `ItemActiveBg`、SharedToken |
| 稳定性 | stable since 6.0 |

#### `itemContent`

| 字段 | 值 |
| --- | --- |
| Owner | `Calendar` |
| Part | `itemContent` |
| Selector | `.semantic-item-content` |
| SelectorRoute | `/template/ .semantic-content > .semantic-scope-body > .semantic-scope-cells > .semantic-item /template/ .semantic-item-content` |
| Style Type | `CalendarItemContentStyle` |
| ContractType | `ContentControl` |
| Cardinality | `Multiple` |
| Customization | `Selector` |
| CrossVisualRoot | `false` |
| RuntimeCreated | `true` |
| AtomUI 节点 | 每个 Cell 模板中的 `ContentControl#PART_ItemContent` |
| 职责 | 统一表示单元格内自定义内容区域（`CellTemplate` / `FullCellTemplate`）的高度、溢出与布局样式。 |
| 相关 API | `CellTemplate`、`FullCellTemplate`、`CalendarCellContext` |
| 相关 Token | `ItemActiveBg`、SharedToken |
| 稳定性 | stable since 6.0 |

`root` 是隐式 Part，不添加 `.semantic-root`。`ContractType` 只定义 Setter 可以稳定依赖的最低 public 类型，并通过
`x:SetterTargetType` 提供 AXAML 编译期类型上下文；它不参与 `.semantic-*` 的身份匹配。`header`、`content`、`item` 的
`ContractType` 为 `TemplatedControl` 而非 internal 的 `CalendarHeader` / `CalendarView` / `CalendarViewCell`，因为
internal 类型不能作为公共 Setter 依赖的最低类型；`body` 的 `ContractType` 为 `DockPanel`，`itemContent` 为
`ContentControl`，二者都是承载节点的公开具体类型。

### 1.2 `LunarCalendar`

`LunarCalendar` 声明与 `Calendar` 完全相同的六个 Part（`root`、`header`、`body`、`content`、`item`、`itemContent`），
字段值与 §1.1 一致，仅 Owner 与生成 Style 类型名不同（`LunarCalendarHeaderStyle` 等）。节点映射差异：

- `root` 为 `LunarCalendar` owner。
- `header`、`body`、`content` 的 marker 继承自 `LunarCalendarControlTheme` BasedOn 的 `CalendarControlTheme` 模板，
  节点与普通 Calendar 相同。
- `item` 由 `LunarCalendarPresentationAdapter.CreateCell()` 创建的 `LunarCalendarViewCell` 承载；marker 从
  `CalendarViewCell` 构造逻辑继承，不因农历适配而重复添加。
- `itemContent` 位于 `LunarCalendarViewCellTheme` 自身重写的 Cell 模板中的 `ContentControl#PART_ItemContent`；
  农历次级内容（`PART_SecondaryPresenter` 及内部 marker/文本）不属于 Semantic Part，见 [§6 定制边界](#6-定制边界)。

## 2. Part 说明

### 2.1 root

`root` 是 Calendar / LunarCalendar owner 本身，在实例整个生命周期内始终存在，每个实例恰好一个。

它负责：

- 承载 `Value`、`Mode`、`Fullscreen`、`ShowWeek`、`ValidRange`、`DisabledDate`、三个模板入口与 `RangeBars` 等公共状态。
- 承载 `:fullscreen`、`:mini`、`:month`、`:year`、`:show-week` 根伪类。
- 提供背景、边框、圆角、整体前景色与整体布局；Fullscreen 时水平/垂直拉伸，Mini 时根只提供背景与圆角，外部容器负责
  边框与宽度。
- 作为 `header`、`body`、`content`、`item`、`itemContent` owner-scoped Selector 的作用域边界。

适合通过 root 定制整体前景色、背景、对齐与面板级几何。需要按 `:fullscreen` / `:mini` 或 `:month` / `:year` 分支改变
根样式时，应在 owner Selector 上组合伪类；默认主题已按这些状态设置根伪类，Semantic Style 覆盖时作用于同一属性。

root 不表示模板中的 `Border#PART_Root`、`DockPanel` 等任何内部节点；这些节点的名称、数量和层级不属于 root 契约。
Mini 卡片模式的边框与固定宽度由外部承载容器提供，不属于 Calendar root 契约。

### 2.2 header

`header` 表示默认 Header 区域，每个内置 Calendar 模板恰好一个 `CalendarHeader` 节点（`PART_DefaultHeader`），cardinality
为 `Single`。`HeaderTemplate` 非空时该节点通过 `IsVisible=false` 隐藏并由 `ContentControl#PART_CustomHeader` 显示自定义
内容，但节点与 marker 仍属于模板稳定结构，Part 身份与数量不变。这一行为与上游一致：antd 在提供 `headerRender` 时
不再把 `header` class 应用到自定义节点。

它负责：

- 统一承载年份选择、月份选择与 Month/Year 模式切换的 Header 布局与局部视觉。
- 在 Fullscreen 与 Mini 之间切换内部交互控件的尺寸（Mini 使用 Small 尺寸）。

适合定制 Header 区域的对齐与间距。Header 模板的根节点是 `DockPanel`，不投影 `Background` / `Padding`，因此这两个属性
不作为 `header` Part 的公共定制路径；需要改变 Header 区域背景时通过 root 的整体背景或主题分支实现。Header 内部的
Year/Month 下拉 `ComboBox` 与模式切换 `OptionButtonGroup` 是嵌套组件：`ComboBox` 属于全局排除清单，`OptionButtonGroup`
不是本家族公开的 Part；它们不通过本控件的 Semantic Part 定制，如需样式化应使用各自控件的公开入口。Header 的选项生成
（`CalendarHeaderOptions`）、ValidRange 收敛与本地化文案不属于 `header` Part 的样式契约。

### 2.3 body

`body` 表示根模板中 Header 下方的主体区域容器，即 `DockPanel#PART_BodyPresenter`，每个内置模板恰好一个，cardinality
为 `Single`。它是 `content`（日历表格）与 `PART_RangeBarPanel` 范围条 overlay 的共同容器，包含顶部分隔线
（`ColorSplit` 细线）与纵向 Padding。

它负责：

- 承载主体区域的内边距、背景与整体布局，对应上游 `.ant-picker-body` 的“容纳日历网格的容器”职责。
- 为 `content`、`item`、`itemContent` 的 Selector 路由提供作用域锚点（见 [§3 Selector 用法](#3-selector-用法)）。

适合定制主体区域的 `Background` 与局部对齐。`DockPanel` 不提供 `Padding` 属性，body 的纵向间距由主题内的分隔线 Border
承载，不作为 Semantic Setter 路径。body 不承诺其内部子节点顺序（分隔线、网格与 overlay 的层级），
不公开 `PART_RangeBarPanel`、范围条 segment 或 overlay 坐标算法；范围条样式通过 [Calendar 范围条设计](range-bar-design.md)
的 `CalendarRangeBar` 属性定制。

### 2.4 content

`content` 表示日历表格区域，即 `CalendarView`（`PART_CalendarView`），每个内置模板恰好一个，cardinality 为 `Single`。
它同时承载周标题行（`Grid#PART_WeekHeader`）与日期/月网格宿主（`Grid#PART_CellHost`），对应上游 `.ant-picker-content`
的“日历表格宽度、高度与表格样式”职责（上游 `<table>` 的 thead 与 tbody 在 AtomUI 中由同一 `CalendarView` 内的两个
Grid 表达）。

它负责：

- 承载表格级样式：背景、宽度与高度。
- 在 Month 模式提供 6×7（ShowWeek 时 8 列）日期网格，在 Year 模式提供 3×4 月份网格；网格行列由 `CalendarView` 代码
  生成，不通过 AXAML 维护。

适合定制表格区域的 `Background` 与横向/纵向对齐。`content` 的 `Background` 经主题 TemplateBinding 投影到模板根
`DockPanel#PART_Body`；`DockPanel` 不提供 `Padding`，content 的 `Padding` 不作为公共定制路径。content 的固定高度约束
（Mini 的 `MiniContentHeight`、Fullscreen Cell 的 `FullCellMinHeight`）由 root 有效属性经主题绑定驱动，见
[§5 尺寸基线](#5-尺寸基线)；不承诺 `PART_WeekHeader` / `PART_CellHost` 的内部结构或周标题文本节点为公共区域。周标题
行的生成逻辑（Culture 的 `FirstDayOfWeek`、缩写星期名、周序号列）不属于 content 的样式契约。

### 2.5 item

`item` 表示日历网格中的单个单元，即运行时创建的 `CalendarViewCell`，cardinality 为 `Multiple`。Month 日期模式生成
42 个日期 Cell，`ShowWeek` 开启时追加 6 个周序号 Cell（共 48），Year 模式生成 12 个月份 Cell；周序号 Cell 与日期/月
Cell 同为网格单元，携带同一 `.semantic-item` marker。Cell 由 `CalendarView` 的有限容器池复用，marker 在 Cell 构造时
一次性添加，`Bind` / `Unbind` / 回收 / 模式切换不增删 marker。

它负责：

- 承载单个单元的局部视觉入口：背景、边框、悬停与选中视觉。
- 保留选中、禁用、命中测试与键盘焦点语义（由 Cell 容器的伪类表达：`:today`、`:selected`、`:outside`、`:disabled`、
  `:focused`、`:date`、`:month`、`:week`）。

适合定制单元的背景与边框；需要按状态分支时在生成 Style 内继续组合伪类（例如 `.semantic-item:selected`）。Mini 选中态
使用主色背景与浅色文本、today 使用主色单线描边；Fullscreen 选中态使用 `ItemActiveBg`。item 不公开
`CalendarViewCell` 具体控件类型、`PART_Item` / `PART_CellInner` 模板层级、`DisplayText` 或 Cell 模型字段。

### 2.6 itemContent

`itemContent` 表示单元格内的自定义内容区域，即每个 Cell 模板中的 `ContentControl#PART_ItemContent`，cardinality 为
`Multiple`（每个网格单元各一个）。marker 静态声明于 `CalendarViewCellTheme.axaml` 与
`LunarCalendarViewCellTheme.axaml` 两个 Cell 模板中，不随模板切换增删。Fullscreen 单元格中该节点即使未设置
`CellTemplate` / `FullCellTemplate` 也保持可见——与上游一致，antd 默认 Cell render 始终渲染
`.ant-picker-calendar-date-content` 空内容区域，高度取自 `dateContentHeight`；Mini 与 Week 单元格没有模板时隐藏。
节点与 marker 始终属于模板稳定结构，该区域是 Cell 内自定义内容的高度、溢出控制点。

它负责：

- 承载 `CellTemplate` / `FullCellTemplate` 生成的业务内容，并暴露高度、溢出、对齐等局部样式。
- 在 `CellTemplate` 与 `FullCellTemplate` 两种模板模式间保持同一 Part 身份。

适合定制自定义内容区域的高度、对齐与溢出行为。itemContent 不公开用户模板生成的子树、`CalendarCellContext` /
`LunarCalendarCellContext` 的字段级契约或农历次级内容（`PART_SecondaryPresenter`）。

## 3. Selector 用法

应用级样式先限定 Calendar owner，再通过生成的 Semantic Style 进入 Part。生成类型已经封装 owner 类型保护和
`SelectorRoute`，用户不需要复制模板路径：

```xml
<Application.Styles>
    <Style Selector="atom|Calendar">
        <atom:CalendarBodyStyle x:SetterTargetType="DockPanel">
            <Setter Property="Background" Value="#FAFAFA" />
        </atom:CalendarBodyStyle>

        <atom:CalendarContentStyle x:SetterTargetType="TemplatedControl">
            <Setter Property="Background" Value="#F5F7FA" />
        </atom:CalendarContentStyle>

        <atom:CalendarItemStyle x:SetterTargetType="TemplatedControl">
            <Setter Property="Background" Value="#F5D2D2" />
        </atom:CalendarItemStyle>
    </Style>
</Application.Styles>
```

对特定 Calendar class 或状态定制时，把 class、属性或伪类放在 owner 一侧：

```xml
<Style Selector="atom|Calendar.semantic-card:mini">
    <Setter Property="Fullscreen" Value="False" />
    <atom:CalendarContentStyle x:SetterTargetType="TemplatedControl">
        <Setter Property="Background" Value="#BDE3C3" />
    </atom:CalendarContentStyle>
</Style>
```

`item` 与 `itemContent` 是运行时 Part。运行时 Cell 不是任何模板的静态内容（`TemplatedParent` 为 null），因此它们的
SelectorRoute 不能再用第二个 `/template/` 跨越 `CalendarView` 模板，而是从 `.semantic-content` 出发，经
`.semantic-scope-body`（`CalendarView` 模板根）与 `.semantic-scope-cells`（Cell 宿主）两个中间 scope marker 的 `>` 步骤
到达运行时 Cell，再以 `/template/` 进入 Cell 自身模板到达 `itemContent`。两个 scope marker 只用于路由、不单独发布为
Part。生成 Style 已封装完整路由，用户样式不得复制这些 route，也不得依赖 `PART_*` 名称或 internal 类型。

不得把 `ContractType` 写入 Part Selector。以下写法不属于公共契约：

- `TemplatedControl.semantic-header` 或 `:is(TemplatedControl).semantic-header`。
- `DockPanel.semantic-body` 或 `ContentControl.semantic-item-content`。
- 直接复制 `/template/ .semantic-*` route 作为用户主路径；route 只属于 descriptor 与生成 Style 的实现元数据。
- 连续穿过子控件模板的多个 `/template/`。
- 依赖 `PART_*`、internal 类型、Name 或视觉祖先顺序。

## 4. 状态与数量语义

marker 身份不随任何运行时状态切换增删；marker 数量只随网格拓扑（`Mode` 与 `ShowWeek`）变化：

| 状态 | root | header | body | content | item | itemContent | 说明 |
| --- | --- | --- | --- | --- | --- | --- | --- |
| Month 日期网格 | 1 | 1 | 1 | 1 | 42 | 42 | 6×7 日期 Cell，marker 数量固定。 |
| Month + `ShowWeek` | 1 | 1 | 1 | 1 | 48 | 48 | 追加 6 个周序号 Cell，周序号 Cell 同样携带 marker。 |
| Year 月份网格 | 1 | 1 | 1 | 1 | 12 | 12 | 3×4 月份 Cell。 |
| 同月内 `Value` 变化 | 1 | 1 | 1 | 1 | 42/48 | 42/48 | 只更新选中伪类，不重建容器。 |
| `Fullscreen` ↔ Mini | 1 | 1 | 1 | 1 | 42/48/12 | 42/48/12 | 只改变布局密度与 Header 控件尺寸。 |
| `HeaderTemplate` 非空 | 1 | 1 | 1 | 1 | 42/48/12 | 42/48/12 | 默认 Header 隐藏但节点与 marker 仍存在。 |
| `CellTemplate` 未设置 | 1 | 1 | 1 | 1 | 42/48/12 | 42/48/12 | `PART_ItemContent` 隐藏但节点与 marker 仍存在。 |
| 语言 / Culture 变化 | 1 | 1 | 1 | 1 | 42/48/12 | 42/48/12 | `FirstDayOfWeek` 变化重建网格，数量不变。 |
| LunarCalendar | 1 | 1 | 1 | 1 | 42/48/12 | 42/48/12 | Cell 类型变为 `LunarCalendarViewCell`，marker 继承自基类构造。 |

容器池上限由当前 `Mode` 与 `ShowWeek` 决定；`Unbind`、模板重应用与 detach 只解除业务绑定，不清除 `.semantic-item`
marker，保证回收后的 Cell 重新 `Bind` 时 marker 身份不变。

## 5. 尺寸基线

Calendar 的尺寸链与 Semantic Setter 的关系：

| 项目 | 内容 |
| --- | --- |
| 网格拓扑 | Month 6×7（ShowWeek 时 8 列）、Year 3×4，由 `CalendarView` 代码生成列/行定义；Semantic Setter 不改变网格拓扑。 |
| Mini 内容高度 | `CalendarView` 在 `:mini` 下由主题绑定 `Height` / `MaxHeight` 到 `MiniContentHeight`（普通 Calendar 固定 256；LunarCalendar 按双行 Cell 尺寸派生），有效值经 root 的 `EffectiveMiniContentHeight` DynamicResource 链传入。 |
| Fullscreen Cell 高度 | `PART_CellInner` 的 `MinHeight` 由 `TemplateBinding FullCellMinHeight` 驱动，有效值经 root 的 `EffectiveFullCellMinHeight` 链传入；LunarCalendar 用增大的有效值避免次级文案与 RangeBars 重叠。 |
| 头部与 body 间距 | 默认 Header 使用 SharedToken 纵向 `PaddingSM`，body 在顶部分隔线之后使用纵向 `PaddingXS`；这些是主题 Token 契约，不经 Semantic Part 覆盖。 |

`body` / `content` 的 `Background` 等样式 Setter 是公共定制路径（`body` 直接命中 `DockPanel`，`content` 经主题
TemplateBinding 投影）；`Padding` 与 Header/body 的纵向间距由主题 Token（`PaddingSM` / `PaddingXS` 分隔线）承载，不作为
Semantic Setter 路径。固定 `Height` / `MaxHeight` / `MinHeight` 类布局 Setter 会与主题内的尺寸绑定竞争 Avalonia 属性
优先级，且无法跟随 Fullscreen/Mini 与 LunarCalendar 的 metrics 链，因此不作为公共定制路径。需要改变 Mini 内容高度或
Fullscreen Cell 高度时应通过 Token（`MiniContentHeight` / `FullCellMinHeight`）或主题分支实现。

失败回归：Mini 的 `CalendarView` 高度、周标题行与六行 Cell 的高度由同一 metrics 链驱动。若 Semantic Setter 直接固定
`content` 的 `Height`，Mini 高度链被旁路，六行网格会与主题设置的 `MaxHeight` 冲突；LunarCalendar 场景下固定高度还会
破坏 `FullCellMinHeight` 与 `RangeBarTopOffset` 的避让关系。最小复现是给 `:mini` 的 `CalendarContentStyle` 设置固定
`Height`，此时内容区不再跟随 `MiniContentHeight`；移除该 Setter 后恢复。

## 6. 定制边界

以下区域明确不属于 Calendar Semantic Part：

- 默认 Header 内部的 Year/Month `ComboBox` 下拉、`OptionButtonGroup` 模式切换与其弹层内容；`ComboBox` 属于全局排除
  清单，不通过本控件的 Part 获得 marker。
- 周标题行（`Grid#PART_WeekHeader`）与其文本节点；上游 `<table>` 的 thead 没有独立 Semantic key。
- `PART_RangeBarPanel` 范围条 overlay、segment 分段与 lane 分配；`RangeBars` 是 AtomUI 桌面扩展，上游 Calendar 没有
  对应 Semantic key（LLMS 区域 `rangeBar` 不属于 Semantic Part）。
- `LunarCalendarViewCell` 的次级内容（`PART_SecondaryPresenter`、节气/节日/节假日 marker 与次级文本）；LLMS 区域
  `lunarContent` 不属于 Semantic Part。
- 用户 `CellTemplate` / `FullCellTemplate` 模板生成的子树。
- `CalendarViewCell` 的 `DisplayText`、Cell 模型字段、键盘 roving focus 与 Automation provider。
- `PART_*` 名称、internal 类型、状态转换器与 motion phase。

Semantic Style 服从 Avalonia 原生属性优先级。Part Setter 命中只证明目标属性已生效；如果最终布局仍被 owner 或中间
节点的固定测量、主题尺寸绑定或网格代码生成约束，应按跨节点布局约束排查，不能把它解释为 Semantic Style 优先级失效。

## 7. 兼容性与验证

删除或重命名 Part、修改 selector class、收窄 `ContractType`、改变 cardinality，或者让任一内置模板缺少 marker，均
属于公共主题契约变更。

验证至少覆盖：

- descriptor 中只有 `root`、`header`、`body`、`content`、`item`、`itemContent`，字段值与本文表格一致（`header` /
  `body` / `content` 为 `Single`，`item` / `itemContent` 为 `Multiple`，后两者 `RuntimeCreated=true` 且
  `ContractType` 为公开类型）。
- 内置 Calendar 模板的 marker 数量与类型一致：`semantic-header`、`semantic-body`、`semantic-content` 各一个，root
  隐式；Cell 模板中的 `semantic-item-content` 静态存在。
- `LunarCalendar` 拥有自己的 descriptor，marker 从继承的根模板与派生 Cell 模板获得，普通 Calendar 与 LunarCalendar
  的 marker 数量一致。
- Month 日期网格 42 个 `semantic-item`（ShowWeek 48）、Year 12 个；周序号 Cell 同样携带 marker；`Value` 同月变化不
  重建容器，marker 实例不丢失。
- `Mode` / `ShowWeek` 切换后容器池收敛到 42/48/12，回收的 Cell 重新 `Bind` 后 marker 身份不变；模板重应用与 detach
  不残留旧容器。
- `HeaderTemplate` 非空时 `semantic-header` 节点隐藏但存在；`CellTemplate` 未设置时 `semantic-item-content` 节点隐藏
  但存在。
- `body` / `content` 的 `Padding` / `Background`、`item` 的背景与选中态在 Fullscreen/Mini、Light/Dark 下保持有效；
  Semantic Style 覆盖不与默认主题（不消费 `.semantic-*`）产生 selector activator 开销。
- 默认主题不消费 `.semantic-*`；未声明用户 Semantic Style 时不增加 selector activator。
- Generator 静态输出和 NativeAOT 路径不依赖反射或运行时扫描；`item` marker 通过生成常量在 Cell 构造路径一次性添加。
