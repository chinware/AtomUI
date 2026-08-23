# Steps 语义结构

> 生成产物：由源文档生成，不要手工编辑。修改内容请回到控件文档、源码 public surface、Token 类型或生成数据、Gallery ShowCase 或源码结构。

## Semantic Parts

| Part | Selector | ContractType | Cardinality | Customization | CrossVisualRoot | RuntimeCreated |
| --- | --- | --- | --- | --- | --- | --- |
| `root` | owner | `Steps` | `Single` | `Root` | `false` | `false` |
| `item` | `.semantic-item` | `StepsItem` | `Multiple` | `Selector` | `false` | `true` |
| `itemWrapper` | `.semantic-item-wrapper` | `Border` | `Multiple` | `Selector` | `false` | `true` |
| `itemIcon` | `.semantic-item-icon` | `TemplatedControl` | `Multiple` | `Selector` | `false` | `true` |
| `itemTitle` | `.semantic-item-title` | `ContentPresenter` | `Multiple` | `Selector` | `false` | `true` |
| `itemSubtitle` | `.semantic-item-subtitle` | `ContentPresenter` | `Multiple` | `Selector` | `false` | `true` |
| `itemSection` | `.semantic-item-section` | `Panel` | `Multiple` | `Selector` | `false` | `true` |
| `itemContent` | `.semantic-item-content` | `ContentPresenter` | `Multiple` | `Selector` | `false` | `true` |
| `itemRail` | `.semantic-item-rail` | `DashedBorder` | `Multiple` | `Selector` | `false` | `true` |

`root` 是控件自身，承载 `Current`、`Initial`、`Status`、`Percent`、`Type`、`Orientation`、
`TitlePlacement`、`SizeType`、`IsItemClickable` 等 public API、主题入口和状态投影，不声明 `.semantic-root`
marker。

`item` 的 marker 挂在 `StepsItem` 实例上，是运行时创建的语义标记：`Steps` 在容器准备时对每个容器写入
`semantic-item`。直接声明的 `StepsItem`、普通数据项生成的容器以及回收复用后重新准备的容器遵循同一规则。

七个子 Part 是 item 模板内的静态标记，声明在 `StepsItemTheme.axaml` 的对应节点上。它们的
`RuntimeCreated = true` 表示这些 Part 只随 item 容器的存在而存在：item 被创建时 marker 随模板出现，
item 被移除时随模板销毁；`Steps` 自身不包含任何子 Part marker。

## Abstract AXAML Structure

来源：`src/AtomUI.Desktop.Controls/Steps/Themes/StepsTheme.axaml`

```xml
<DashedBorder>
    <ItemsPresenter Name="PART_ItemsPresenter" />
</DashedBorder>
```

## Composition Model

该章节由控件 `Themes/` 文件夹中的真实主题文件生成，用于说明 public 控件与内部协作对象之间的运行时结构。内部节点只用于理解和维护，不应指导用户代码直接依赖。

### 控件角色图

```text
Steps
  -> StepsItemIndicator (control theme, StepsItemIndicatorTheme.axaml)
     -> Panel (template-stable)
        -> WaveSpiritDecorator#{x:Static atom:WaveSpiritDecorator.WaveSpiritPart} (template-stable)
        -> PixelAlignedBorder#Frame (template-stable)
           -> Panel (template-stable)
              -> TextBlock#StepNumberText (template-stable)
              -> CheckOutlined#FinishedMark (template-stable)
              -> CloseOutlined#ErrorMark (template-stable)
        -> IconPresenter#CustomIconPresenter (internal-observable)
  -> StepsItem (item container control theme, StepsItemTheme.axaml)
     -> StepsItemLayoutPanel (internal-observable)
        -> StepsPanelItemFrame#ItemWrapper (internal-observable)
        -> StepsItemIndicator#PART_Indicator (template-stable)
        -> StepsItemSectionPanel#Section (internal-observable)
           -> ContentPresenter#HeaderPresenter (internal-observable)
           -> ContentPresenter#SubHeaderPresenter (internal-observable)
           -> ContentPresenter#ContentPresenter (internal-observable)
        -> PixelAlignedBorder#Connector (template-stable)
        -> StepsNavigationArrow#NavigationArrow (internal-observable)
        -> StepsPanelArrow#PanelArrow (internal-observable)
        -> PixelAlignedBorder#NavigationActiveIndicator (template-stable)
  -> Steps (control theme, StepsTheme.axaml)
     -> DashedBorder (template-stable)
        -> ItemsPresenter#PART_ItemsPresenter (template-stable)
```

### 协作节点

| 节点 | 类型 | 来源 | 生命周期 owner | 影响的 public API | 稳定性 | Agent 使用边界 |
| --- | --- | --- | --- | --- | --- | --- |
| `Steps` | public control | `源文档 + public API` | 用户代码 / 控件宿主 | public API | public | 用户可直接使用 public 控件；可作为示例和 API 入口。 |
| `StepsItemIndicator` | control theme | `StepsItemIndicatorTheme.axaml` | Steps | `Background`, `BorderBrush`, `CornerRadius`, `DisplayStepNumber`, `FontSize`, `Foreground` | internal-observable | 用于理解结构和状态流，不应指导用户代码直接依赖。 |
| `Panel` | template node (Panel) | `StepsItemIndicatorTheme.axaml` | StepsItemIndicator | `Background`, `BorderBrush`, `CornerRadius`, `DisplayStepNumber`, `FontSize`, `Foreground` | template-stable | 用于主题维护；变更需同步主题、实现和 LLMS。 |
| `{x:Static atom:WaveSpiritDecorator.WaveSpiritPart}` | template node (WaveSpiritDecorator) | `StepsItemIndicatorTheme.axaml` | StepsItemIndicator | `IsMotionEnabled`, `IsWaveSpiritEnabled` | template-stable | 用于主题维护；变更需同步主题、实现和 LLMS。 |
| `Frame` | template node (PixelAlignedBorder) | `StepsItemIndicatorTheme.axaml` | StepsItemIndicator | `Background`, `BorderBrush`, `CornerRadius`, `DisplayStepNumber`, `FontSize`, `Foreground` | template-stable | 用于主题维护；变更需同步主题、实现和 LLMS。 |
| `StepNumberText` | template node (TextBlock) | `StepsItemIndicatorTheme.axaml` | StepsItemIndicator | `DisplayStepNumber`, `FontSize`, `Foreground` | template-stable | 用于主题维护；变更需同步主题、实现和 LLMS。 |
| `FinishedMark` | template node (CheckOutlined) | `StepsItemIndicatorTheme.axaml` | StepsItemIndicator | `FontSize` | template-stable | 用于主题维护；变更需同步主题、实现和 LLMS。 |
| `ErrorMark` | template node (CloseOutlined) | `StepsItemIndicatorTheme.axaml` | StepsItemIndicator | `FontSize` | template-stable | 用于主题维护；变更需同步主题、实现和 LLMS。 |
| `CustomIconPresenter` | template node (IconPresenter) | `StepsItemIndicatorTheme.axaml` | StepsItemIndicator | `Icon`, `IsCustom` | internal-observable | 用于理解结构和状态流，不应指导用户代码直接依赖。 |
| `StepsItem` | item container control theme | `StepsItemTheme.axaml` | 用户代码 / 控件宿主 | `Background`, `BorderBrush`, `BorderThickness`, `CanInvoke`, `Content`, `ContentTemplate` | public | 用户可直接使用 public 控件；可作为示例和 API 入口。 |
| `StepsItemLayoutPanel` | template node (StepsItemLayoutPanel) | `StepsItemTheme.axaml` | StepsItem | `Background`, `BorderBrush`, `BorderThickness`, `CanInvoke`, `Content`, `ContentTemplate` | internal-observable | 用于理解结构和状态流，不应指导用户代码直接依赖。 |
| `ItemWrapper` | template node (StepsPanelItemFrame) | `StepsItemTheme.axaml` | StepsItem | `Background`, `BorderBrush`, `BorderThickness`, `CornerRadius`, `IsFirst`, `PanelVariant` | internal-observable | 用于理解结构和状态流，不应指导用户代码直接依赖。 |
| `PART_Indicator` | template node (StepsItemIndicator) | `StepsItemTheme.axaml` | StepsItem | `CanInvoke`, `EffectiveStatus`, `Icon`, `IsCurrent`, `IsMotionEnabled`, `IsProgressFrameReserved` | template-stable | 用于主题维护；变更需同步主题、实现和 LLMS。 |
| `Section` | template node (StepsItemSectionPanel) | `StepsItemTheme.axaml` | StepsItem | `Content`, `ContentTemplate`, `Foreground`, `Header`, `HeaderTemplate`, `Orientation` | internal-observable | 用于理解结构和状态流，不应指导用户代码直接依赖。 |
| `HeaderPresenter` | template node (ContentPresenter) | `StepsItemTheme.axaml` | StepsItem | `Foreground`, `Header`, `HeaderTemplate` | internal-observable | 用于理解结构和状态流，不应指导用户代码直接依赖。 |
| `SubHeaderPresenter` | template node (ContentPresenter) | `StepsItemTheme.axaml` | StepsItem | `SubHeader`, `SubHeaderTemplate` | internal-observable | 用于理解结构和状态流，不应指导用户代码直接依赖。 |
| `ContentPresenter` | template node (ContentPresenter) | `StepsItemTheme.axaml` | StepsItem | `Content`, `ContentTemplate` | internal-observable | 用于理解结构和状态流，不应指导用户代码直接依赖。 |
| `Connector` | template node (PixelAlignedBorder) | `StepsItemTheme.axaml` | StepsItem | 主题状态 / visual state | template-stable | 用于主题维护；变更需同步主题、实现和 LLMS。 |
| `NavigationArrow` | template node (StepsNavigationArrow) | `StepsItemTheme.axaml` | StepsItem | 主题状态 / visual state | internal-observable | 用于理解结构和状态流，不应指导用户代码直接依赖。 |
| `PanelArrow` | template node (StepsPanelArrow) | `StepsItemTheme.axaml` | StepsItem | 主题状态 / visual state | internal-observable | 用于理解结构和状态流，不应指导用户代码直接依赖。 |
| `NavigationActiveIndicator` | template node (PixelAlignedBorder) | `StepsItemTheme.axaml` | StepsItem | 主题状态 / visual state | template-stable | 用于主题维护；变更需同步主题、实现和 LLMS。 |
| `Steps` | control theme | `StepsTheme.axaml` | 用户代码 / 控件宿主 | `Background`, `BackgroundSizing`, `BorderBrush`, `BorderDashArray`, `BorderDashOffset`, `BorderThickness` | public | 用户可直接使用 public 控件；可作为示例和 API 入口。 |
| `PART_ItemsPresenter` | template node (ItemsPresenter) | `StepsTheme.axaml` | Steps | 主题状态 / visual state | template-stable | 用于主题维护；变更需同步主题、实现和 LLMS。 |

## Template Parts

| 布局模式 | item 份额 | 收缩下限 |
| --- | --- | --- |
| 水平 + 水平标题（`Default` 等） | 非末项 `1 1 auto`，末项 `0 1 auto` | `IconContainerSize` |
| 水平 + 垂直标题（`Dot`、`OutlineDot`、`Inline`、`TitlePlacement=Vertical`） | 全部 item `1 1 0%` 等分 | `IconContainerSize` |
| 水平 `Navigation` | 等分 | `IconContainerSize` |
| 垂直 `Orientation` | 不参与横向份额 | 不适用 |

## Pseudo Classes

Steps 不提供控件专属的完成态或选择态伪类。状态主题读取 `EffectiveStatus`、`IsCurrent`、`CanInvoke` 以及 Avalonia 标准 `:pointerover`、`:focus-visible`、`:disabled` 伪类。

`itemWrapper`、`itemIcon`、`itemTitle`、`itemSubtitle`、`itemSection`、`itemContent` 和 `itemRail` 是 `StepsItem` 自身模板内的静态语义 marker，由 `StepsItemTheme.axaml` 声明；应用通过生成的子 Part Style 经 `> .semantic-item /template/ .semantic-item-x` 路由进入 item 模板，不得依赖节点名称以外的模板结构或手写 `/template/` selector。实例级 Header、SubHeader 和 Connector 覆盖由根控件三项 nullable 语义 API 投影，与 Semantic Part 定制互不取代。

## State Flow

### 4.1 唯一状态流

```text
Current / Initial / Steps.Status / StepsItem.Status / item index
    -> StepNumber / AutomaticStatus
    -> IsCurrent / EffectiveStatus / ConnectorStatus
    -> StepsItem theme / StepsItemIndicator theme
```

模板和容器生命周期只消费状态，不建立或修正状态。

### 4.2 状态算法

对索引为 `index` 的 item：

```text
StepNumber = Initial + index

AutomaticStatus =
    StepNumber == Current ? Steps.Status :
    StepNumber < Current  ? Finish :
                            Wait

EffectiveStatus = StepsItem.Status ?? AutomaticStatus
IsCurrent       = StepNumber == Current
```

`EffectiveStatus` 是 Indicator、Connector、Progress、文字和颜色唯一允许读取的状态。`IsCurrent` 与 `EffectiveStatus` 独立，因此当前 item 可以显式为 Wait、Finish 或 Error。

### 4.3 越界语义

- `Current < Initial`：没有当前 item，未覆盖 item 自动为 Wait。
- `Current >= Initial + ItemCount`：没有当前 item，未覆盖 item 自动为 Finish。
- Items 为空：保留 `Current`，不产生 item 状态。
- Items 动态变化：使用保留的输入重新计算，不执行选择恢复或 Current 归一。

### 4.4 Connector 语义

连接 item `i` 和 `i + 1` 的 Connector 使用下一个 item 的 `EffectiveStatus`，让指向当前错误或当前进行中步骤的线段跟随目标步骤状态：

```text
Connector[i].Status = Item[i + 1].EffectiveStatus
```

最后一个 item 不显示 Connector。

### 4.5 Pointer、键盘与 Wave

可交互条件：

```text
Steps.IsItemClickable
&& Steps.IsEnabled
&& StepsItem.IsEnabled
```

- Pointer 只有在同一 item 内完成 press/release 才视为 click。
- 点击非当前 item：播放目标 Indicator Wave，并发出 `CurrentChangeRequested`。
- 点击当前 item：播放 Wave，不发出请求。
- `Type=OutlineDot` 点击仍按可交互规则发出请求，但不播放 Indicator Wave。
- Enter/Space：非当前 item 发出请求，不播放 Wave。
- 程序化修改 `Current`、Items 变化、模板重套和状态重算都不播放 Wave。
- 不可交互 item 不显示 hand cursor、hover 激活视觉，也不进入 Tab 焦点序列。

## Theme and Token Boundaries

Steps 使用统一语义模板，而不是按 Type、Orientation 和 TitlePlacement 复制 ControlTemplate。

| 主题文件 | 职责 |
| --- | --- |
| `StepsTheme.axaml` | 根模板、ItemsPresenter、StepsPanel 和根展示输入映射。 |
| `StepsItemTheme.axaml` | 统一 item 语义模板、Panel item frame、状态颜色、Connector、内容和交互视觉。 |
| `StepsItemIndicatorTheme.axaml` | 统一 Indicator 模板、Dot、Icon、状态图标、Progress 和 Wave。 |

运行时组合：

```text
Steps
└── ItemsPresenter#PART_ItemsPresenter
    └── StepsPanel
        └── StepsItem
            └── StepsItemLayoutPanel
                ├── StepsPanelItemFrame#ItemWrapper
                ├── StepsItemIndicator#PART_Indicator
                │   └── WaveSpiritDecorator#PART_WaveSpirit
                ├── StepsItemSectionPanel#Section
                │   ├── ContentPresenter#HeaderPresenter
                │   ├── ContentPresenter#SubHeaderPresenter
                │   └── ContentPresenter#ContentPresenter
                ├── PixelAlignedBorder#Connector
                ├── PathIcon#NavigationArrow
                ├── StepsPanelArrow#PanelArrow
                └── PixelAlignedBorder#NavigationActiveIndicator
```

`StepsPanel` 负责 item 间的 flex/stack 布局；Panel 类型强制水平排列并将每个 item 等宽。`StepsItemLayoutPanel` 负责 item 内固定语义区域、Connector 线宽、Panel 外溢箭头和 Navigation active 线的排列，正文区域（标题、副标题、详情）的分组与对齐由 `StepsItemSectionPanel` 完成。这些面板不创建视觉、不计算状态。item 间的弹性压缩与文本换行契约见 [8.7 弹性压缩与文本换行模型](#87-弹性压缩与文本换行模型)。

Panel 类型的几何规则：

- Indicator 和普通 Connector 不参与可见布局。
- ItemWrapper 覆盖完整 item 单元，PanelArrow 在非末项的外侧拉伸为楔形箭头。
- LTR 箭头向右外溢，RTL 箭头向左外溢；末项不创建可见箭头。
- `Filled` 使用状态背景作为面板表面，并在非首项裁出左侧 notch；`Outlined` 保留共享接缝的箭头边框，非当前 Error 项保持容器背景并使用红色文字和边框，当前 Error 项才使用浅红 active 背景。
`OutlineDot` 复用 `Dot` 的布局路径，只改变 Indicator 的填充、边框和 Wave 语义。

### 5.1 Item 语义样式覆盖

`ItemHeaderForeground`、`ItemSubHeaderForeground` 和 `ItemRailBackground` 是 `Steps` 实例级的语义槽，不是对模板内部节点的公开暴露。`Steps` 将显式值投影到每个 `StepsItem`，再由 `StepsItemTheme.axaml` 在自己的模板边界内分别应用到 Header、SubHeader 和 Connector。

三项 API 分别对应步骤条标题、子标题和 rail 的语义能力，但保持 Avalonia 的强类型 `IBrush?` 契约，不公开任一模板节点。

优先级固定为：

```text
非 null 的 Steps 实例语义样式
    > 当前 Type / EffectiveStatus 对应的 StepsToken
null
    -> 完整回退到当前 Type / EffectiveStatus 对应的 StepsToken
```

运行时修改或清空任一属性必须立即更新所有已实现容器；直接声明的 `StepsItem`、由普通数据项生成的容器以及回收后重新准备的容器遵循同一投影规则。该覆盖不改变 Indicator、Content、Navigation active indicator 或其他未命名语义区域。

有效标题布局：

```text
Orientation == Vertical -> Horizontal
Type == Dot             -> Vertical
Type == OutlineDot      -> Vertical
Type == Inline          -> Vertical
Type == Navigation      -> Horizontal
Type == Panel           -> Horizontal
其他                    -> TitlePlacement
```

Token 边界：

StepsToken 描述步骤标题、详情内容、Indicator、Dot、OutlineDot、Connector、Navigation、Inline、Panel 和 Progress ring 的组件级视觉语义。

StepsToken 不承载：

- Current、Initial、item index 或 StepNumber。
- public Status、AutomaticStatus、EffectiveStatus、IsCurrent 或 ConnectorStatus。
- item 数量、layout bounds、pointer、keyboard、focus 或 Wave 播放状态。
- Percent 当前值、CanInvoke、IsItemClickable 或 IsMotionEnabled。
- `ItemHeaderForeground`、`ItemSubHeaderForeground` 或 `ItemRailBackground` 的实例值。

## Customization Boundaries

完成本次不兼容重构后，以下契约构成新的稳定边界：

- `Steps` 继承 `ItemsControl`，不得重新引入 Selection 作为第二状态源。
- `Current` 是唯一当前步骤输入，交互路径只发出请求。
- `Initial` 只表示编号偏移，不能在模板生命周期中写入 `Current`。
- `StepsItem.Status` 保持 nullable，并优先于自动状态。
- 主题只能读取 `EffectiveStatus`，不得恢复多套并行的状态输入。
- `Content` 只表示步骤详情，Steps 不保存或投影当前页面内容。
- `Type` 是视觉类型唯一入口，不得恢复独立 Style/IndicatorType 组合。
- `Percent=null` 是 Progress 的唯一关闭语义。
- Wave 只能由真实 pointer click 触发，不得监听 `Current` 或 `IsCurrent`。
- `OutlineDot` 必须保持 Dot 布局、空心状态色边框和无 Wave 语义。
- `PART_ItemsPresenter` 和 `PART_Indicator` 是稳定 template part。
- 外部样式不得依赖 `HeaderPresenter`、`SubHeaderPresenter`、`Connector` 等内部节点名称，也不得手写 `/template/` selector 穿透 `StepsItem`；item 子节点的实例级定制必须通过生成的 Semantic Part Style 表达，Header、SubHeader 和 Connector 的实例级覆盖由 `Steps` 三项 nullable 语义 API 表达。
- 三项 item 语义样式保持 nullable；`null` 必须恢复完整的状态和类型 Token 视觉。
- 每个根、item 和 indicator 主题各保留一套语义模板。
- 水平布局的 item 收缩与文本换行遵循 8.7 弹性模型的份额算法与 `IconContainerSize` 收缩下限。
- 测量与排列必须共用同一份额算法，排列宽度等于测量宽度；任何布局路径不得以裁剪代替换行。

维护不变量：

- Current 是唯一当前步骤输入；不得引入第二套选择状态或双向同步。
- 每次状态协调必须完整覆盖派生状态，不依赖旧值。
- item public Status 不被根控件写入或覆盖。
- EffectiveStatus 是所有状态视觉的唯一输入。
- Connector 使用下一个 item EffectiveStatus。
- 垂直 Steps 的 item 间距属于 item 内部测量空间，最后一个 item 必须清零；不得用 Content padding 或 StepsPanel 外部 spacing 代替。
- Initial 不在 OnApplyTemplate 或 attach 中写入 Current。
- Offset 不参与状态编号、Current 归一或 item 状态计算；它只改变 Inline 布局前置占位。
- 根级步骤页面内容投影和内容订阅不得重新引入。
- pointer click 是 Wave 的唯一触发源；Current 变化不能播放 Wave。
- OutlineDot click 不能播放 Wave；该例外必须在 Indicator 层兜住，避免 pointer、keyboard 或未来激活入口绕过。
- 每个主题只维护一套语义模板。
- 外部代码不得通过深层 selector 修改 StepsItem 内部节点；实例级 Header、SubHeader 和 Connector 定制由根控件三项 nullable 语义 API 进入。
- 语义样式的 `null` 值必须完整回退 Token；容器清理和重新准备不得残留旧 owner 的显式值。
- Semantic Part descriptor、`semantic-item` 运行时 marker 与七个静态 `semantic-item-*` marker 的同步规则、`> .semantic-item /template/ .semantic-item-x` 容器边界路由形状以及生成的 Steps*Style 类型保持稳定；`itemIcon` 默认圆角只能由主题 style 优先级提供，代码不得再以 local value 写入。
- StepsPanel、StepsItemLayoutPanel 和 StepsItemSectionPanel 只负责布局。
- 水平布局的 item 收缩与文本换行遵循 overview.md 8.7 弹性模型的份额算法与 `IconContainerSize` 收缩下限；测量与排列必须共用同一份额算法，排列宽度等于测量宽度，任何布局路径不得以裁剪代替换行。
- 水平标题 heading 行的同行/换行决策由测量与排列共用同一判定条件；Header 与 SubHeader 并排放不下时，SubHeader 必须换到 Header 下方独占一行并保持测量宽度，不得裁成剩余宽度。
- 水平标题路径的 body 子项（Header / SubHeader / Content）必须按排列时的 body 可用宽度（item 宽度 − indicator − spacing）测量，不得按完整 item 宽度测量；否则份额落在文本自然宽度的邻近区间时会以裁剪代替换行。
- 宽容器的既有伸展语义（非末 item 等额伸展、末 item 内容宽、单 item 内容宽）不得随压缩能力回归。
- 容器清理必须释放 Owner，模板重套必须释放旧 part 引用。
- Percent、Icon、Type 和 EffectiveStatus 运行时变化必须立即更新 Progress。
