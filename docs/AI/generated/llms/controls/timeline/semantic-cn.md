# Timeline 语义结构

> 生成产物：由源文档生成，不要手工编辑。修改内容请回到控件文档、源码 public surface、Token 类型或生成数据、Gallery ShowCase 或源码结构。

## Semantic Parts

Timeline 以单一 owner `Timeline` 公开全部九个 Semantic Part，与上游稳定 Semantic DOM 一一对齐。上游
`TimelineSemanticType` 是 `StepsSemanticType` 去掉 `itemSubtitle` 后的九个语义键（`classNames` / `styles` 均为
`{ root?, item?, itemWrapper?, itemIcon?, itemSection?, itemHeader?, itemTitle?, itemContent?, itemRail? }`），
Timeline 委托 `Steps`（`type="dot"`）渲染：`root` 消费于 `<ol>` 根节点，`item` 消费于每个 `<li>` 容器，
`itemWrapper` 消费于容器内裹节点，`itemIcon` 消费于图标节点，`itemSection` 消费于包含 header 与 content 的
区域容器，`itemHeader` 消费于包含 title 与 rail 的头部容器，`itemTitle` 消费于标题节点，`itemContent` 消费于
内容节点，`itemRail` 消费于节点连接线元素。上游 DOM 嵌套为
`ol > li > wrapper > [icon, section > [header > [title, rail], content]]`（rc-steps `Step.tsx`，
npm `@rc-component/steps@1.2.2`，上游 6.4.5 依赖 `~1.2.2`）。

AtomUI 原 item 模板是扁平结构，且连接线由 `TimelineIndicator.Render` 直接绘制。为支持全部九个 Part，本次
改造把控件自身的视觉结构补齐为真实节点：

```text
Timeline (root)
  └─ TimelineItem (item, .semantic-item)
       └─ TimelineItemPanel#RootLayout (itemWrapper, .semantic-item-wrapper)
            └─ TimelineSectionPanel#Section (itemSection, .semantic-item-section)
                 ├─ StackPanel#Header (itemHeader, .semantic-item-header)
                 │    └─ TextBlock#Label (itemTitle, .semantic-item-title)
                 ├─ TimelineIndicator#Indicator (.semantic-indicator 跳点)
                 │    ├─ Border#PART_Rail (itemRail, .semantic-item-rail)
                 │    ├─ Border#PART_Dot (itemIcon, .semantic-item-icon，无 IndicatorIcon 时可见)
                 │    └─ Border#PART_IconHost (itemIcon, .semantic-item-icon，有 IndicatorIcon 时可见)
                 │         └─ IconPresenter#PART_IconPresenter（图标内容，非 Part）
                 └─ ContentPresenter#ContentPresenter (itemContent, .semantic-item-content)
```

与上游 DOM 的两处结构差异（Part 名称、数量与样式语义不变）：

- 上游 `rail` 元素 DOM 上位于 `header` 内（CSS absolute 定位到轴线）；AtomUI 的 rail 是
  `TimelineIndicator` 模板内的真实 `Border` 元素，与圆点/图标同属轴线列。Avalonia Panel 只能排列自己的直接
  子级，把 rail 放在 header 内无法由面板定位到轴线，因此 rail 归属轴线列是布局事实的忠实表达。
- 上游 `icon` 是 `wrapper` 的直接子级；AtomUI 的图标宿主位于 `TimelineIndicator`（section 内）。该层级差异
  不影响 Part 的寻址与样式语义。

`itemIcon` 的双元素承载与上游对齐：上游 icon 元素在无自定义 icon 时本身就是圆点（dot 尺寸 + 边框即圆环），
有自定义 icon 时是图标盒。AtomUI 对应地把 `.semantic-item-icon` 同时标记在 `Border#PART_Dot`（内置圆点，
无 `IndicatorIcon` 时可见）与 `Border#PART_IconHost`（图标宿主，有 `IndicatorIcon` 时可见）两个互斥可见的
Border 上，`itemIcon` 样式（如 `BorderBrush`）会作用到当前可见的那个 —— 与上游
`styles.itemIcon.borderColor` 同时能改圆点环色与图标盒边框的行为一致。

改造的视觉保真保证：圆点与连接线的绘制从 `TimelineIndicator.Render` 迁移为真实模板元素（`PART_Dot` 与
`PART_Rail`），rail 覆盖整条轴线、由不透明圆点/图标宿主掩膜出与原先完全一致的线段缺口；几何公式与
IsFirst/IsLast 裁剪边界逐项保留，默认主题下渲染结果像素级不变（见 2.5 节与
[Timeline 桌面版实现原理](implementation.md)）。

上游 Semantic DOM 文档还有第二个预览区块 "Timeline Items"（`items[].classNames` 逐项注入，
`root` / `wrapper` / `icon` / `section` / `header` / `title` / `content` / `rail` 八个键）。它是每项数据上的
classNames 注入机制，不是独立 owner；AtomUI Semantic Part 系统不提供逐项 classNames 注入 API，且 item 级
Part 的 cardinality 已经是 `Multiple`（天然覆盖每一项）。Gallery Semantic Parts 页签用两个
`SemanticPartPreview` 复刻上游的两个预览区块：`Timeline`（九卡，对应组件级 Part）与 `Timeline Items`
（两 item 预览 + 九卡短描述，对应上游逐项视图的 item 级 Part 呈现）；hover 高亮均为 owner 作用域
（覆盖预览内全部 item），逐项注入式高亮不适用。

AtomUI 九个 Part 随本次 Semantic Part 改造同时公开，descriptor 的 `Since` 统一为 `6.0`。

以下类型不持有独立 Semantic descriptor：

- `TimelineItem` 是公开 item 容器，上游 `TimelineSemanticType` 声明在 Timeline 组件上（`Timeline.Item` 只是
  兼容入口，无独立 Semantic DOM Props），容器职责由 Timeline 的 `item` 等 Part 表达（与 `SegmentedItem`、
  `ListBoxItem`、`CollapseItem` 同一决策）。
- `AbstractTimeline`、`AbstractTimelineItem` 是跨平台共享基类，不是对应用公开的独立 owner。
- `TimelineIndicator`、`TimelineItemPanel`、`TimelineSectionPanel`、`TimelineStackPanel` 是 internal 协作
  类型；其中 `TimelineIndicator` 模板节点只作为 `itemIcon` / `itemRail` route 的中间跳点
  （`.semantic-indicator`），不是 Part。

### 1.1 `Timeline`

#### `root`

| 字段 | 值 |
| --- | --- |
| Owner | `Timeline` |
| Part | `root` |
| Selector | Timeline 本身 |
| SelectorRoute | 不适用 |
| Style Type | 不适用（root 不生成 Style） |
| ContractType | `Timeline` |
| Cardinality | `Single` |
| Customization | `Root` |
| CrossVisualRoot | `false` |
| RuntimeCreated | `false` |
| AtomUI 节点 | Timeline owner（表面投影到 `Border#Frame`） |
| 职责 | Timeline root 是 Items、Orientation、Mode、IsReverse、Pending 与可见项视觉顺序的统一 owner；根表面（背景、边框、圆角、内边距）投影到 `Frame`。 |
| 相关 API | `Items`、`ItemsSource`、`Orientation`、`Mode`、`IsReverse`、`Pending`、`PendingIcon` |
| 相关 Token | SharedToken（`ColorBorder`、`ColorBgContainer`） |
| 稳定性 | stable since 6.0 |

#### `item`

| 字段 | 值 |
| --- | --- |
| Owner | `Timeline` |
| Part | `item` |
| Selector | `.semantic-item` |
| SelectorRoute | `> .semantic-item` |
| Style Type | `TimelineItemStyle` |
| ContractType | `TimelineItem` |
| Cardinality | `Multiple` |
| Customization | `Selector` |
| CrossVisualRoot | `false` |
| RuntimeCreated | `true` |
| AtomUI 节点 | 每个 `TimelineItem` 容器 |
| 职责 | 统一表示时间轴单个节点容器：单项 Label、Content、Indicator 的承载入口与视觉顺序派生状态的接收方；对应上游 `<li>`。 |
| 相关 API | `Label`、`Content`、`ContentTemplate`、`IndicatorIcon`、`IndicatorColor` |
| 相关 Token | `ItemPaddingBottom`、`ItemPaddingBottomLG`、`IndicatorStartModeMargin`、`IndicatorEndModeMargin`、`IndicatorMiddleModeMargin` |
| 稳定性 | stable since 6.0 |

#### `itemWrapper`

| 字段 | 值 |
| --- | --- |
| Owner | `Timeline` |
| Part | `itemWrapper` |
| Selector | `.semantic-item-wrapper` |
| SelectorRoute | `> .semantic-item /template/ .semantic-item-wrapper` |
| Style Type | `TimelineItemWrapperStyle` |
| ContractType | `Panel` |
| Cardinality | `Multiple` |
| Customization | `Selector` |
| CrossVisualRoot | `false` |
| RuntimeCreated | `true` |
| AtomUI 节点 | TimelineItem 模板中的 `TimelineItemPanel#RootLayout` |
| 职责 | 统一表示节点内容包装容器：把 section 铺满自身并承载 IndicatorSpacing 等包装级布局状态；对应上游 item wrapper 节点。 |
| 相关 API | `Orientation`、`Mode`、`IsLabelLayout`、`IsOdd`（internal 投影） |
| 相关 Token | SharedToken（`UniformlyPaddingXS`） |
| 稳定性 | stable since 6.0 |

#### `itemIcon`

| 字段 | 值 |
| --- | --- |
| Owner | `Timeline` |
| Part | `itemIcon` |
| Selector | `.semantic-item-icon` |
| SelectorRoute | `> .semantic-item /template/ .semantic-indicator /template/ .semantic-item-icon` |
| Style Type | `TimelineItemIconStyle` |
| ContractType | `Border` |
| Cardinality | `Multiple` |
| Customization | `Selector` |
| CrossVisualRoot | `false` |
| RuntimeCreated | `true` |
| AtomUI 节点 | TimelineIndicator 模板中的 `Border#PART_Dot` 与 `Border#PART_IconHost`（互斥可见） |
| 职责 | 统一表示节点图标区域：无 `IndicatorIcon` 时是内置圆点（`BorderBrush` 即圆环色），有 `IndicatorIcon` 时是图标宿主；对应上游 item icon 节点（上游同一元素两种形态）。 |
| 相关 API | `IndicatorIcon`、`IndicatorColor` |
| 相关 Token | `IndicatorSize`、`IndicatorDotSize`、`IndicatorDotBorderWidth`、SharedToken（`ColorPrimary`、`ColorBgContainer`） |
| 稳定性 | stable since 6.0 |

#### `itemSection`

| 字段 | 值 |
| --- | --- |
| Owner | `Timeline` |
| Part | `itemSection` |
| Selector | `.semantic-item-section` |
| SelectorRoute | `> .semantic-item /template/ .semantic-item-section` |
| Style Type | `TimelineItemSectionStyle` |
| ContractType | `Panel` |
| Cardinality | `Multiple` |
| Customization | `Selector` |
| CrossVisualRoot | `false` |
| RuntimeCreated | `true` |
| AtomUI 节点 | TimelineItem 模板中的 `TimelineSectionPanel#Section` |
| 职责 | 统一表示节点区域容器：承载 header、Indicator 与 content 的方向化 Measure/Arrange（Alternate 双侧、同侧紧凑与水平 Label 堆叠模型）；对应上游 item section 节点。 |
| 相关 API | `Orientation`、`Mode`、`IsLabelLayout`、`IsOdd`（internal 投影） |
| 相关 Token | SharedToken（`UniformlyPaddingXS`） |
| 稳定性 | stable since 6.0 |

#### `itemHeader`

| 字段 | 值 |
| --- | --- |
| Owner | `Timeline` |
| Part | `itemHeader` |
| Selector | `.semantic-item-header` |
| SelectorRoute | `> .semantic-item /template/ .semantic-item-header` |
| Style Type | `TimelineItemHeaderStyle` |
| ContractType | `StackPanel` |
| Cardinality | `Multiple` |
| Customization | `Selector` |
| CrossVisualRoot | `false` |
| RuntimeCreated | `true` |
| AtomUI 节点 | TimelineItem 模板中的 `StackPanel#Header` |
| 职责 | 统一表示节点头部容器：承载 title 文本与对齐方式；对应上游 item header 节点。 |
| 相关 API | `Label`、`Mode`、`Orientation` |
| 相关 Token | `ItemPaddingBottom`、`ItemPaddingBottomLG` |
| 稳定性 | stable since 6.0 |

#### `itemTitle`

| 字段 | 值 |
| --- | --- |
| Owner | `Timeline` |
| Part | `itemTitle` |
| Selector | `.semantic-item-title` |
| SelectorRoute | `> .semantic-item /template/ .semantic-item-title` |
| Style Type | `TimelineItemTitleStyle` |
| ContractType | `TextBlock` |
| Cardinality | `Multiple` |
| Customization | `Selector` |
| CrossVisualRoot | `false` |
| RuntimeCreated | `true` |
| AtomUI 节点 | header 内的 `TextBlock#Label` |
| 职责 | 统一表示节点标题/时间标签区域：文本呈现、换行与下内边距；对应上游 item title 节点。 |
| 相关 API | `Label`、`Mode`、`Orientation` |
| 相关 Token | `ItemPaddingBottom`、`ItemPaddingBottomLG` |
| 稳定性 | stable since 6.0 |

#### `itemContent`

| 字段 | 值 |
| --- | --- |
| Owner | `Timeline` |
| Part | `itemContent` |
| Selector | `.semantic-item-content` |
| SelectorRoute | `> .semantic-item /template/ .semantic-item-content` |
| Style Type | `TimelineItemContentStyle` |
| ContractType | `ContentPresenter` |
| Cardinality | `Multiple` |
| Customization | `Selector` |
| CrossVisualRoot | `false` |
| RuntimeCreated | `true` |
| AtomUI 节点 | section 内的 `ContentPresenter#ContentPresenter` |
| 职责 | 统一表示节点详细内容区域：`Content` / `ContentTemplate` 的呈现、受限宽度换行与下内边距；对应上游 item content 节点。 |
| 相关 API | `Content`、`ContentTemplate`、`Mode`、`Orientation` |
| 相关 Token | `ItemPaddingBottom`、`ItemPaddingBottomLG`、`LastItemContentMinHeight` |
| 稳定性 | stable since 6.0 |

#### `itemRail`

| 字段 | 值 |
| --- | --- |
| Owner | `Timeline` |
| Part | `itemRail` |
| Selector | `.semantic-item-rail` |
| SelectorRoute | `> .semantic-item /template/ .semantic-indicator /template/ .semantic-item-rail` |
| Style Type | `TimelineItemRailStyle` |
| ContractType | `Border` |
| Cardinality | `Multiple` |
| Customization | `Selector` |
| CrossVisualRoot | `false` |
| RuntimeCreated | `true` |
| AtomUI 节点 | TimelineIndicator 模板中的 `Border#PART_Rail` |
| 职责 | 统一表示节点连接线（轴线轨道）：承载 IsFirst/IsLast 裁剪后的轨道条，厚度与颜色来自 `IndicatorTailWidth` / `IndicatorTailColor`；对应上游 item rail 节点。 |
| 相关 API | `IndicatorTailColor`、`IndicatorTailWidth`、`IndicatorColor` |
| 相关 Token | `IndicatorTailWidth`、`IndicatorTailColor`、`IndicatorDotSize` |
| 稳定性 | stable since 6.0 |

`root` 是隐式 Part，不声明 `.semantic-root` marker。`itemWrapper`、`itemSection`、`itemHeader`、
`itemTitle`、`itemContent` 的 marker 静态声明在 `TimelineItemTheme.axaml` 模板内；`itemIcon`、`itemRail` 的
marker 声明在 `TimelineIndicatorTheme.axaml` 模板内。这八个 Part 都位于运行时生成的 `TimelineItem` 容器
内部，descriptor 统一声明 `RuntimeCreated=true`，生成器不按 owner 主题资产做静态校验。

`item` 是 `RuntimeCreated` Part：`.semantic-item` marker 在容器创建与 prepare 路径幂等添加，Pending item
创建路径同样幂等补齐。Timeline 是 ItemsControl，`TimelineItem` 容器的逻辑父级就是 Timeline 本身（与
`ListView` / `ListBox` 的 `> .semantic-item` 同一模式），因此 route 不需要先经 `/template/` 进入内部 items
host，`>` 一步即可到达容器。

`itemIcon` 与 `itemRail` 的 route 需要两次 `/template/` 跳点：先进入 TimelineItem 模板命中
`TimelineIndicator#Indicator` 节点上的 `.semantic-indicator` 跳点（不是 Part），再进入 TimelineIndicator
模板命中对应 marker。

`ContractType` 只定义 Setter 可以稳定依赖的最低 public 类型，并通过 `x:SetterTargetType` 提供 AXAML 编译期
类型上下文；它不参与 `.semantic-*` 的身份匹配。`itemWrapper` / `itemSection` 的真实节点
`TimelineItemPanel` / `TimelineSectionPanel` 是 internal 类型，因此 ContractType 使用其最低公开基类
`Panel`（与 Calendar 对 internal cell 使用 `TemplatedControl` 同一决策）。

## Abstract AXAML Structure

来源：`src/AtomUI.Desktop.Controls/Timeline/Themes/TimelineTheme.axaml`

```xml
<Border Name="Frame">
    <ScrollViewer Name="ScrollViewer">
        <ItemsPresenter Name="ItemsPresenter" />
    </ScrollViewer>
</Border>
```

## Composition Model

该章节由控件 `Themes/` 文件夹中的真实主题文件生成，用于说明 public 控件与内部协作对象之间的运行时结构。内部节点只用于理解和维护，不应指导用户代码直接依赖。

### 控件角色图

```text
Timeline
  -> TimelineIndicator (control theme, TimelineIndicatorTheme.axaml)
     -> Panel (template-stable)
        -> Border#PART_Rail (template-stable)
        -> Border#PART_Dot (template-stable)
        -> Border#PART_IconHost (template-stable)
           -> IconPresenter#PART_IconPresenter (template-stable)
  -> TimelineItem (item container control theme, TimelineItemTheme.axaml)
     -> TimelineItemPanel#RootLayout (template-stable)
        -> TimelineSectionPanel#Section (template-stable)
           -> StackPanel#Header (template-stable)
              -> TextBlock#Label (template-stable)
           -> TimelineIndicator#Indicator (internal-observable)
           -> ContentPresenter#ContentPresenter (internal-observable)
  -> Timeline (control theme, TimelineTheme.axaml)
     -> Border#Frame (template-stable)
        -> ScrollViewer#ScrollViewer (template-stable)
           -> ItemsPresenter#ItemsPresenter (internal-observable)
```

### 协作节点

| 节点 | 类型 | 来源 | 生命周期 owner | 影响的 public API | 稳定性 | Agent 使用边界 |
| --- | --- | --- | --- | --- | --- | --- |
| `Timeline` | public control | `源文档 + public API` | 用户代码 / 控件宿主 | public API | public | 用户可直接使用 public 控件；可作为示例和 API 入口。 |
| `TimelineIndicator` | control theme | `TimelineIndicatorTheme.axaml` | Timeline | `IndicatorColor`, `IndicatorIcon`, `IndicatorTailColor` | internal-observable | 用于理解结构和状态流，不应指导用户代码直接依赖。 |
| `Panel` | template node (Panel) | `TimelineIndicatorTheme.axaml` | TimelineIndicator | `IndicatorColor`, `IndicatorIcon`, `IndicatorTailColor` | template-stable | 用于主题维护；变更需同步主题、实现和 LLMS。 |
| `PART_Rail` | template node (Border) | `TimelineIndicatorTheme.axaml` | TimelineIndicator | `IndicatorTailColor` | template-stable | 用于主题维护；变更需同步主题、实现和 LLMS。 |
| `PART_Dot` | template node (Border) | `TimelineIndicatorTheme.axaml` | TimelineIndicator | `IndicatorColor` | template-stable | 用于主题维护；变更需同步主题、实现和 LLMS。 |
| `PART_IconHost` | template node (Border) | `TimelineIndicatorTheme.axaml` | TimelineIndicator | `IndicatorIcon` | template-stable | 用于主题维护；变更需同步主题、实现和 LLMS。 |
| `PART_IconPresenter` | template node (IconPresenter) | `TimelineIndicatorTheme.axaml` | TimelineIndicator | `IndicatorIcon` | template-stable | 用于主题维护；变更需同步主题、实现和 LLMS。 |
| `TimelineItem` | item container control theme | `TimelineItemTheme.axaml` | 用户代码 / 控件宿主 | `Content`, `ContentTemplate`, `IndicatorColor`, `IndicatorIcon`, `IsFirst`, `IsLabelLayout` | public | 用户可直接使用 public 控件；可作为示例和 API 入口。 |
| `RootLayout` | template node (TimelineItemPanel) | `TimelineItemTheme.axaml` | TimelineItem | `Content`, `ContentTemplate`, `IndicatorColor`, `IndicatorIcon`, `IsFirst`, `IsLabelLayout` | template-stable | 用于主题维护；变更需同步主题、实现和 LLMS。 |
| `Section` | template node (TimelineSectionPanel) | `TimelineItemTheme.axaml` | TimelineItem | `Content`, `ContentTemplate`, `IndicatorColor`, `IndicatorIcon`, `IsFirst`, `IsLabelLayout` | template-stable | 用于主题维护；变更需同步主题、实现和 LLMS。 |
| `Header` | template node (StackPanel) | `TimelineItemTheme.axaml` | TimelineItem | `Label` | template-stable | 用于主题维护；变更需同步主题、实现和 LLMS。 |
| `Label` | template node (TextBlock) | `TimelineItemTheme.axaml` | TimelineItem | `Label` | template-stable | 用于主题维护；变更需同步主题、实现和 LLMS。 |
| `Indicator` | template node (TimelineIndicator) | `TimelineItemTheme.axaml` | TimelineItem | `IndicatorColor`, `IndicatorIcon`, `IsFirst`, `IsLast`, `NextIsPending`, `Orientation` | internal-observable | 用于理解结构和状态流，不应指导用户代码直接依赖。 |
| `ContentPresenter` | template node (ContentPresenter) | `TimelineItemTheme.axaml` | TimelineItem | `Content`, `ContentTemplate` | internal-observable | 用于理解结构和状态流，不应指导用户代码直接依赖。 |
| `Timeline` | control theme | `TimelineTheme.axaml` | 用户代码 / 控件宿主 | `Background`, `BorderBrush`, `BorderThickness`, `CornerRadius`, `Padding`, `atom` | public | 用户可直接使用 public 控件；可作为示例和 API 入口。 |
| `Frame` | template node (Border) | `TimelineTheme.axaml` | Timeline | `Background`, `BorderBrush`, `BorderThickness`, `CornerRadius`, `Padding`, `atom` | template-stable | 用于主题维护；变更需同步主题、实现和 LLMS。 |
| `ScrollViewer` | template node (ScrollViewer) | `TimelineTheme.axaml` | Timeline | `atom` | template-stable | 用于主题维护；变更需同步主题、实现和 LLMS。 |
| `ItemsPresenter` | template node (ItemsPresenter) | `TimelineTheme.axaml` | Timeline | 主题状态 / visual state | internal-observable | 用于理解结构和状态流，不应指导用户代码直接依赖。 |

## Template Parts

| 契约组 | 代表成员 | 维护含义 |
| --- | --- | --- |
| 内容与数据 | `Content`、`Label`、`IndicatorIcon`、`Pending`、`PendingIcon` | 定义单项内容、时间标签、节点图标和待处理节点入口。 |
| 集合顺序 | `Items`、`ItemsSource`、`IsReverse` | 维护源顺序、最终视觉顺序和 Pending 项的相邻关系。 |
| 方向与模式 | `Orientation`、`Mode` | 决定主轴方向以及内容位于轴线的 Start、End 或交替侧。 |
| 内部派生状态 | `IsLabelLayout`、`IsOdd`、`IsFirst`、`IsLast`、`NextIsPending` | 由 Timeline 根据可见项视觉顺序单向投影到 Item 和模板。 |
| 视觉与布局 | `IndicatorColor`、`IndicatorIcon`、`IndicatorTailColor`、`IndicatorTailWidth` | 影响节点颜色、形状和轴线渲染；连接线颜色与宽度由 Indicator 属性与 Token 定制。 |
| Semantic Part | `root`、`item`、`itemWrapper`、`itemIcon`、`itemTitle`、`itemContent` | 单一 owner `Timeline` 公开的语义区域，见 [Timeline Semantic Part 契约](semantic-part.md)。 |
| 其他稳定入口 | `Label`、`Pending` | 保留为 public surface，变更前需确认 Gallery 和用户 XAML 依赖。 |

## Pseudo Classes

| 状态反馈 | public API、内部状态和伪类如何形成用户可感知反馈。 | `IsReverse`、Pending 状态、可见项顺序和首尾节点状态。 |
| 布局语义 | 主轴方向和内容相对轴线的位置如何组合。 | `Orientation` 决定主轴，`Mode` 决定交叉轴上的 `Start`、`End` 或交替布局。 |
| 主题语义 | ControlTheme、SharedToken、控件 Token 和模板绑定如何表达视觉。 | Timeline Token、方向 selector、Item 模板和 Indicator renderer。 |

## State Flow

Timeline 的状态流按以下路径收敛：

```text
Orientation / Mode / IsReverse / Items / item visibility
  -> Timeline 计算可见项视觉顺序
  -> item effective mode / order / first / last / pending adjacency
  -> internal property / pseudo-class / template property
  -> ControlTheme selector / presenter / renderer
  -> Gallery 可观察行为
```

方向与模式的稳定语义：

| Orientation | Mode | 无 Label | 存在 Label |
| --- | --- | --- | --- |
| `Vertical` | `Start` | 轴线位于逻辑起始侧，Content 位于结束侧。 | Label 位于起始侧，Content 位于结束侧。 |
| `Vertical` | `End` | Content 位于逻辑起始侧，轴线位于结束侧。 | Content 位于起始侧，Label 位于结束侧。 |
| `Horizontal` | `Start` | 轴线在上，Content 在下。 | 轴线在上，Label 与 Content 依次在下并对轴线居中。 |
| `Horizontal` | `End` | Content 在上，轴线在下。 | Label 与 Content 依次在上，轴线在下。 |
| 任意方向 | `Alternate` | 第一可见项为 Start，后续按 End、Start 交替。 | 使用同一交替规则，并保持所有节点共用同一轴线。 |

`FlowDirection` 只影响垂直 Timeline 的逻辑起始侧和结束侧；水平 Timeline 的 Start/End 分别映射到下方和上方。`IsReverse` 只反转主轴视觉顺序，不交换 Start/End。隐藏项不占用布局槽位，也不参与交替奇偶、首尾和 Pending 相邻关系计算。

## Theme and Token Boundaries

Timeline 的视觉模型由控件模板、ControlTheme、SharedToken 和必要的控件 Token 共同构成。

| 主题文件 | 职责 |
| --- | --- |
| `TimelineIndicatorTheme.axaml` | 提供控件模板、selector、资源绑定和状态视觉。 |
| `TimelineItemTheme.axaml` | 定义集合项、容器项或局部单元的状态视觉。 |
| `TimelineTheme.axaml` | 提供控件模板、selector、资源绑定和状态视觉。 |

Timeline 使用 `TimelineToken` 作为控件 Token scope。Token 只表达组件视觉语义，不承载方向、Mode、视觉索引或 Pending 相邻状态。水平布局的内容间距优先使用 SharedToken；方向差异由 ControlTheme selector 和布局 Panel 表达。

主题维护规则：

- 不删除或重命名已经稳定的 ControlTheme key、template part、伪类和资源 key。
- 不把可由 AXAML 表达的模板状态迁移为 C# 动态创建视觉。
- 不把 hover、pressed、selected、expanded、loading、filter、popup open 等运行时状态写入 Token。
- Browser 或平台特化主题必须保持同一 API 的语义一致。

Token 边界：

Timeline Token 只表达节点、连接线和 Item 的组件级尺寸、间距与颜色。Token 不承载 Orientation、Mode、视觉索引、首尾、Reverse、Pending 邻接或其他实例运行时状态。

当前 Token scope：

- `TimelineToken`，scope id 为 `Timeline`，源码位于 `src/AtomUI.Desktop.Controls/Timeline/TimelineToken.cs`。

## Customization Boundaries

维护 Timeline 时必须保持以下不变量：

- 不擅自新增、删除、重命名或改变 public/protected API、Avalonia 属性、事件和默认值。
- `TimelineMode` 的稳定值为 `Start`、`End`、`Alternate`；不得重新引入 `Left`、`Right` 或重复值兼容别名。
- `Orientation` 默认值保持 `Vertical`，`Mode` 默认值保持 `Start`。
- 不破坏 template part、伪类、ControlTheme key、Token 名称和资源 key。
- 不改变 Start/End、Alternate 首项、RTL、Reverse、隐藏项和 Pending 的既定组合语义。
- 不为单个 TimelineItem 增加与 Timeline 全局 Mode 竞争的位置 owner。
- 不改变 Gallery 已展示的 XAML 用法、默认外观、交互顺序和状态优先级。
- Template part 重新应用、集合替换、弹层关闭、窗口失活和控件 detach 时必须释放旧订阅和资源宿主。
- 不通过隐藏延迟、强制刷新或吞异常掩盖状态同步问题。
- 不引入运行时反射扫描作为 API、Token 或数据路径发现机制。
- 文档只描述当前稳定设计；历史变化记录在 `changelog.md`。

维护不变量：

维护 Timeline 时不得破坏：

- `TimelineMode` 精确包含 Start、End、Alternate，不保留 Left/Right 或重复值别名。
- Orientation 默认 Vertical，Mode 默认 Start。
- 第一可见 Alternate Item 为 Start，Reverse 后仍按最终视觉顺序重新从 Start 计算。
- 隐藏项不占水平槽位，不参与奇偶、首尾、Label 布局或 Pending 邻接计算。
- 水平可见 Item 等宽、节点同轴、长文本项内换行且不创建内部水平滚动。
- Vertical Start/End 遵循 FlowDirection；Horizontal Start/End 的下方/上方语义不受 RTL 交换。
- Template part 名称、ControlTheme key、伪类和资源 key。
- Item、Panel 和 Indicator 只能消费 AbstractTimeline 投影的状态，不能成为第二状态 owner。
- Light/Dark、Browser/Desktop 和运行时方向切换下的主题一致性。
- 控件文档、源码 public surface、Token 类型或生成数据与源码契约的一致性。
