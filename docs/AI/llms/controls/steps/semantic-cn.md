# Steps 语义结构

> 生成产物：由源文档生成，不要手工编辑。修改内容请回到控件文档、源码 public surface、Token 类型或生成数据、Gallery ShowCase 或源码结构。

## Semantic Parts

| Part | AtomUI 节点 | 职责 | 稳定性 |
| --- | --- | --- | --- |
| `root` | `Steps` | Items、根展示输入和导航请求入口。 | public |
| `items` | `PART_ItemsPresenter` | 承载 StepsPanel 和 item 容器。 | template-stable |
| `item` | `StepsItem` | 单项状态、内容和交互语义。 | public |
| `indicator` | `PART_Indicator` | 数字、状态图标、Dot、自定义 Icon、Progress 和 Wave 目标。 | template-stable |
| `title` | `HeaderPresenter` | 标题。 | internal-observable |
| `subtitle` | `SubHeaderPresenter` | 副标题。 | internal-observable |
| `content` | `ContentPresenter` | 步骤详情。 | internal-observable |
| `rail` | `Connector` | 当前 item 与下一个 item 的连接线。 | internal-observable |
| `navigation-arrow` | `NavigationArrow` | Navigation 类型的步骤方向提示。 | internal-observable |
| `navigation-active-indicator` | `NavigationActiveIndicator` | Navigation 当前项的水平底线或垂直右侧线。 | internal-observable |

## Abstract AXAML Structure

来源：`src/AtomUI.Desktop.Controls/Steps/Themes/StepsTheme.axaml`

```xml
<ItemsPresenter Name="PART_ItemsPresenter" />
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
        -> Border#ItemWrapper (template-stable)
        -> StepsItemIndicator#PART_Indicator (template-stable)
        -> ContentPresenter#HeaderPresenter (internal-observable)
        -> ContentPresenter#SubHeaderPresenter (internal-observable)
        -> PixelAlignedBorder#Connector (template-stable)
        -> ContentPresenter#ContentPresenter (internal-observable)
        -> StepsNavigationArrow#NavigationArrow (internal-observable)
        -> PixelAlignedBorder#NavigationActiveIndicator (template-stable)
  -> Steps (control theme, StepsTheme.axaml)
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
| `StepsItem` | item container control theme | `StepsItemTheme.axaml` | 用户代码 / 控件宿主 | `Background`, `CanInvoke`, `Content`, `ContentTemplate`, `EffectiveStatus`, `Foreground` | public | 用户可直接使用 public 控件；可作为示例和 API 入口。 |
| `StepsItemLayoutPanel` | template node (StepsItemLayoutPanel) | `StepsItemTheme.axaml` | StepsItem | `Background`, `CanInvoke`, `Content`, `ContentTemplate`, `EffectiveStatus`, `Foreground` | internal-observable | 用于理解结构和状态流，不应指导用户代码直接依赖。 |
| `ItemWrapper` | template node (Border) | `StepsItemTheme.axaml` | StepsItem | `Background` | template-stable | 用于主题维护；变更需同步主题、实现和 LLMS。 |
| `PART_Indicator` | template node (StepsItemIndicator) | `StepsItemTheme.axaml` | StepsItem | `CanInvoke`, `EffectiveStatus`, `Icon`, `IsCurrent`, `IsMotionEnabled`, `IsProgressFrameReserved` | template-stable | 用于主题维护；变更需同步主题、实现和 LLMS。 |
| `HeaderPresenter` | template node (ContentPresenter) | `StepsItemTheme.axaml` | StepsItem | `Foreground`, `Header`, `HeaderTemplate` | internal-observable | 用于理解结构和状态流，不应指导用户代码直接依赖。 |
| `SubHeaderPresenter` | template node (ContentPresenter) | `StepsItemTheme.axaml` | StepsItem | `SubHeader`, `SubHeaderTemplate` | internal-observable | 用于理解结构和状态流，不应指导用户代码直接依赖。 |
| `Connector` | template node (PixelAlignedBorder) | `StepsItemTheme.axaml` | StepsItem | 主题状态 / visual state | template-stable | 用于主题维护；变更需同步主题、实现和 LLMS。 |
| `ContentPresenter` | template node (ContentPresenter) | `StepsItemTheme.axaml` | StepsItem | `Content`, `ContentTemplate` | internal-observable | 用于理解结构和状态流，不应指导用户代码直接依赖。 |
| `NavigationArrow` | template node (StepsNavigationArrow) | `StepsItemTheme.axaml` | StepsItem | 主题状态 / visual state | internal-observable | 用于理解结构和状态流，不应指导用户代码直接依赖。 |
| `NavigationActiveIndicator` | template node (PixelAlignedBorder) | `StepsItemTheme.axaml` | StepsItem | 主题状态 / visual state | template-stable | 用于主题维护；变更需同步主题、实现和 LLMS。 |
| `Steps` | control theme | `StepsTheme.axaml` | 用户代码 / 控件宿主 | 主题状态 / visual state | public | 用户可直接使用 public 控件；可作为示例和 API 入口。 |
| `PART_ItemsPresenter` | template node (ItemsPresenter) | `StepsTheme.axaml` | Steps | 主题状态 / visual state | template-stable | 用于主题维护；变更需同步主题、实现和 LLMS。 |

## Template Parts

| LLMS 内容 | 来源 | 说明 |
| --- | --- | --- |
| 单控件完整文档 | `overview.md` + `implementation.md` + `token.md` + Gallery ShowCase | 生成 `controls/steps/index-cn.md`。 |
| 单控件语义文档 | `overview.md` + `implementation.md` + Themes 文件夹 | 生成 `controls/steps/semantic-cn.md`。 |
| API 表 | overview.md 语义摘要 + 源码 public surface | 不在 overview 中维护第二份机械列表。 |
| Design Token 表 | token.md 或 Token 类型 | Token 文档只解释语义边界。 |
| 示例 | Gallery ShowCase + source snippet catalog | 只使用稳定示例。 |
| 源码索引 | `implementation.md` | 用于定位源码、主题和测试。 |

## Pseudo Classes

Steps 不提供控件专属的完成态或选择态伪类。状态主题读取 `EffectiveStatus`、`IsCurrent`、`CanInvoke` 以及 Avalonia 标准 `:pointerover`、`:focus-visible`、`:disabled` 伪类。

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
| `StepsItemTheme.axaml` | 统一 item 语义模板、状态颜色、Connector、内容和交互视觉。 |
| `StepsItemIndicatorTheme.axaml` | 统一 Indicator 模板、Dot、Icon、状态图标、Progress 和 Wave。 |
| `StepsThemes.axaml` | Steps 主题资源聚合入口。 |

运行时组合：

```text
Steps
└── ItemsPresenter#PART_ItemsPresenter
    └── StepsPanel
        └── StepsItem
            └── StepsItemLayoutPanel
                ├── StepsItemIndicator#PART_Indicator
                │   └── WaveSpiritDecorator#PART_WaveSpirit
                ├── ContentPresenter#HeaderPresenter
                ├── ContentPresenter#SubHeaderPresenter
                ├── PixelAlignedBorder#Connector
                ├── ContentPresenter#ContentPresenter
                ├── PathIcon#NavigationArrow
                └── PixelAlignedBorder#NavigationActiveIndicator
```

`StepsPanel` 负责 item 间的 flex/stack 布局；`StepsItemLayoutPanel` 负责 item 内固定语义区域、Connector 线宽和 Navigation active 线的排列。二者不创建视觉、不计算状态。
`OutlineDot` 复用 `Dot` 的布局路径，只改变 Indicator 的填充、边框和 Wave 语义。

有效标题布局：

```text
Orientation == Vertical -> Horizontal
Type == Dot             -> Vertical
Type == OutlineDot      -> Vertical
Type == Inline          -> Vertical
Type == Navigation      -> Horizontal
其他                    -> TitlePlacement
```

Token 边界：

StepsToken 描述步骤标题、详情内容、Indicator、Dot、OutlineDot、Connector、Navigation、Inline 和 Progress ring 的组件级视觉语义。

StepsToken 不承载：

- Current、Initial、item index 或 StepNumber。
- public Status、AutomaticStatus、EffectiveStatus、IsCurrent 或 ConnectorStatus。
- item 数量、layout bounds、pointer、keyboard、focus 或 Wave 播放状态。
- Percent 当前值、CanInvoke、IsItemClickable 或 IsMotionEnabled。

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
- 每个根、item 和 indicator 主题各保留一套语义模板。

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
- StepsPanel 和 StepsItemLayoutPanel 只负责布局。
- 容器清理必须释放 Owner，模板重套必须释放旧 part 引用。
- Percent、Icon、Type 和 EffectiveStatus 运行时变化必须立即更新 Progress。
