# Steps

> 生成产物：由源文档生成，不要手工编辑。修改内容请回到控件文档、源码 public surface、Token 类型或生成数据、Gallery ShowCase 或源码结构。

## 概述

Steps 是线性流程的状态导航条，用于表达当前步骤、已完成步骤、等待步骤、错误步骤和当前步骤的局部进度。它适合安装向导、分步表单、审批流、支付流程和任务流程。

Steps 负责：

- 展示有序步骤及其标题、副标题和详情内容。
- 根据唯一的 `Current` 输入计算每个 item 的有效状态。
- 展示 Indicator、Connector、Dot、Navigation、Inline、Panel 和 Progress 视觉。
- 在启用交互时发出当前步骤变更请求。

Steps 不负责：

- 保存或切换步骤对应的页面内容。
- 业务流程校验、路由、异步任务编排、表单提交或权限控制。
- 维护 Avalonia Selection 状态。
- 在用户激活 item 后自行修改 `Current`。

步骤页面由调用方根据 `Current` 在 Steps 外部切换。`StepsItem.Content` 表示步骤详情，不表示步骤页面。

Steps 的公开语义区域使用 `root`、`item`、`itemWrapper`、`itemIcon`、`itemTitle`、`itemSubtitle`、
`itemSection`、`itemContent` 和 `itemRail` 九个 Semantic Part，与上游步骤语义结构 `StepsSemanticType`
对齐（since 6.0.0）。`root` 是隐式 owner，`item` 覆盖 `StepsItem` 容器，七个 item 子 Part 覆盖 item 模板内
的包裹层、图标、标题、副标题、内容区、详情和连接线。完整契约见 [Steps Semantic Part 契约](semantic-part.md)。

## 包与命名空间

| 项 | 值 |
| --- | --- |
| NuGet 包 | `AtomUI.Desktop.Controls` |
| .NET 命名空间 | `AtomUI.Desktop.Controls` |
| AXAML 命名空间 | `https://atomui.net` |
| Gallery 页面 | `controlgallery/AtomUIGallery/ShowCases/Navigation/Steps` |
| 状态 | Stable |

## 何时使用

Steps 表达“有序流程 + 当前进度 + 可选导航请求”。

| 维度 | 含义 | 典型表达 |
| --- | --- | --- |
| 顺序 | item 按 `Initial + index` 获得稳定步骤编号。 | Indicator number、Connector。 |
| 当前 | `Current` 是唯一当前步骤输入。 | `IsCurrent`、当前 Indicator。 |
| 历史 | 当前步骤之前且未显式覆盖的 item 为完成。 | `StepsStatus.Finish`。 |
| 等待 | 当前步骤之后且未显式覆盖的 item 为等待。 | `StepsStatus.Wait`。 |
| 异常 | 根当前状态或 item 显式状态可以为错误。 | `StepsStatus.Error`。 |
| 导航 | 激活 item 只发出变更请求。 | `CurrentChangeRequested`。 |
| 反馈 | Wave 表达真实 pointer click，不表达状态变化。 | Indicator Wave。 |
| 密度 | 尺寸控制 Indicator、文字和间距。 | `SizeType`。 |
| 弹性 | 可用宽度不足时 item 连续收缩、标题/副标题/描述文本连续换行。 | flex 份额、`IconContainerSize` 收缩下限、Wrap 文本。 |

`StepsType.Default` 表达标准流程，`Dot` 表达实心点状流程，`OutlineDot` 表达空心点状流程，`Navigation` 强调导航入口，`Inline` 表达紧凑内联流程，`Panel` 表达面板式分段步骤。

## 公共 API

### 3.1 根控件契约

`Steps` 继承 `ItemsControl`。它不公开或继承 Selection API。

| API | 类型 | 语义 |
| --- | --- | --- |
| `Current` | `int` | 唯一当前步骤编号，默认 `0`；控件不在激活路径中自行写入。 |
| `Initial` | `int` | 第一个 item 的编号偏移，默认 `0`；不用于初始化或重置 `Current`。 |
| `Status` | `StepsStatus` | 当前步骤的默认状态，默认 `Process`。 |
| `Percent` | `double?` | 当前 Process item 的局部进度；`null` 表示不显示。 |
| `Type` | `StepsType` | `Default`、`Dot`、`OutlineDot`、`Navigation`、`Inline` 或 `Panel`。Panel 强制采用水平等宽布局。 |
| `PanelVariant` | `StepsPanelVariant` | Panel 的视觉变体：`Filled`（默认）或 `Outlined`。其他类型忽略。 |
| `Orientation` | `Orientation` | 步骤排列方向，默认 `Horizontal`。 |
| `TitlePlacement` | `Orientation` | 标题相对 Indicator 的请求布局，默认 `Horizontal`。 |
| `SizeType` | `SizeType` | Indicator、文字和间距尺寸，默认 `Middle`。 |
| `IsItemClickable` | `bool` | 是否允许 item 发出导航请求，默认 `false`。 |
| `Offset` | `int` | Inline 类型前方保留的空 item 单元数量，默认 `0`；其他类型忽略。 |
| `HorizontalContentAlignment` | `HorizontalAlignment` | 垂直 Navigation item 列的水平对齐，默认 `Center`；普通 Steps 和水平 Navigation 不使用。 |
| `IsMotionEnabled` | `bool` | 是否启用 transition 和 Wave，默认来自 SharedToken。 |
| `ItemHeaderForeground` | `IBrush?` | 所有 item 主标题的实例级语义前景色覆盖；默认 `null`，保留当前类型和状态的 Token 视觉。 |
| `ItemSubHeaderForeground` | `IBrush?` | 所有 item 辅助标题的实例级语义前景色覆盖；默认 `null`，保留当前类型和状态的 Token 视觉。 |
| `ItemRailBackground` | `IBrush?` | 所有 item Connector 的实例级语义背景覆盖；默认 `null`，保留当前类型和状态的 Token 视觉。 |
| `Items` / `ItemsSource` / `ItemTemplate` | inherited | 步骤集合和数据模板入口。 |

`Current` 允许小于 `Initial` 或大于所有 item 的编号。Steps 不裁剪、不归一也不回写越界值。

三个 `Item*` 属性只覆盖实例的语义视觉，不参与 `EffectiveStatus`、`ConnectorStatus`、布局或交互计算。它们作用于 `Steps` 拥有的全部直接或生成容器；属性为 `null` 时，主题继续使用 Wait、Process、Finish、Error 以及 Inline 对应的 Token。

### 3.2 事件契约

```csharp
public event EventHandler<StepsCurrentChangeRequestedEventArgs>?
    CurrentChangeRequested;
```

事件仅由可交互 item 的 pointer 或 keyboard 激活路径触发，参数提供目标步骤编号。调用方决定是否更新 `Current`。当前 item 被重复激活时不发出请求。

### 3.3 Item 契约

`StepsItem` 继承 `HeaderedContentControl`。

| API | 类型 | 语义 |
| --- | --- | --- |
| `Header` / `HeaderTemplate` | inherited | 步骤标题。 |
| `SubHeader` / `SubHeaderTemplate` | `object?` / `IDataTemplate?` | 标题旁的辅助信息。 |
| `Content` / `ContentTemplate` | inherited | 步骤详情内容。 |
| `Icon` | `PathIcon?` | 自定义 Indicator 图标。 |
| `Status` | `StepsStatus?` | item 显式状态；`null` 使用根控件自动状态。 |
| `IsEnabled` | inherited | 是否允许该 item 参与交互。 |

`StepsItem` 不实现选择协议，不公开选择态、额外完成态或页面内容 API。

### 3.4 枚举契约

| 枚举 | 成员 | 语义 |
| --- | --- | --- |
| `StepsStatus` | `Wait`、`Process`、`Finish`、`Error` | 根当前状态、item 显式状态和有效状态。 |
| `StepsType` | `Default`、`Dot`、`OutlineDot`、`Navigation`、`Inline`、`Panel` | Steps 的完整视觉类型。Panel 隐藏 Indicator/Connector，使用面板背景、边框和外溢箭头。 |
| `StepsPanelVariant` | `Filled`、`Outlined` | Panel 的填充或描边变体。 |

`StepsType` 同时表达原 Style 和 Indicator 类型，禁止形成 `Navigation + Dot` 等没有明确 Steps 语义的组合。
`OutlineDot` 与 `Dot` 共享布局语义和 Dot 尺寸 Token，但 Indicator 使用透明背景和状态色边框，并且不播放 Indicator Wave。

### 3.5 主题契约

LLMS 语义区域：

| Part | Owner | AtomUI 节点 | 职责 | 稳定性 |
| --- | --- | --- | --- | --- |
| `root` | `Steps` | 控件自身 | 步骤条根语义区域，承载流程状态、布局入口和主题视觉。 | stable since 6.0 |
| `item` | `Steps` | `StepsItem` | 单项状态、内容和交互语义。 | stable since 6.0 |
| `itemWrapper` | `Steps` | `ItemWrapper`（`StepsPanelItemFrame`） | item 包裹层，承载单项整体视觉。 | stable since 6.0 |
| `itemIcon` | `Steps` | `PART_Indicator`（`StepsItemIndicator`） | 数字、状态图标、Dot、自定义 Icon、Progress 和 Wave 目标。 | stable since 6.0 |
| `itemTitle` | `Steps` | `HeaderPresenter` | 标题。 | stable since 6.0 |
| `itemSubtitle` | `Steps` | `SubHeaderPresenter` | 副标题。 | stable since 6.0 |
| `itemSection` | `Steps` | `Section`（`StepsItemSectionPanel`） | item 内容区，承载标题行与详情内容的分组布局容器。 | stable since 6.0 |
| `itemContent` | `Steps` | `ContentPresenter` | 步骤详情。 | stable since 6.0 |
| `itemRail` | `Steps` | `Connector` | 当前 item 与下一个 item 的连接线。 | stable since 6.0 |

Part 的 Selector、ContractType、数量语义与定制边界以 [Steps Semantic Part 契约](semantic-part.md) 为唯一完整来源。

内部模板节点（不属于 Semantic Part）：

| 节点 | 类型 | 职责 |
| --- | --- | --- |
| `items` | `PART_ItemsPresenter` | 承载 StepsPanel 和 item 容器。 |
| `navigation-arrow` | `NavigationArrow` | Navigation 类型的步骤方向提示。 |
| `navigation-active-indicator` | `NavigationActiveIndicator` | Navigation 当前项的水平底线或垂直右侧线。 |

Steps 不提供控件专属的完成态或选择态伪类。状态主题读取 `EffectiveStatus`、`IsCurrent`、`CanInvoke` 以及 Avalonia 标准 `:pointerover`、`:focus-visible`、`:disabled` 伪类。

`itemWrapper`、`itemIcon`、`itemTitle`、`itemSubtitle`、`itemSection`、`itemContent` 和 `itemRail` 是 `StepsItem` 自身模板内的静态语义 marker，由 `StepsItemTheme.axaml` 声明；应用通过生成的子 Part Style 经 `> .semantic-item /template/ .semantic-item-x` 路由进入 item 模板，不得依赖节点名称以外的模板结构或手写 `/template/` selector。实例级 Header、SubHeader 和 Connector 覆盖由根控件三项 nullable 语义 API 投影，与 Semantic Part 定制互不取代。

## 事件与命令

### 3.2 事件契约
public event EventHandler<StepsCurrentChangeRequestedEventArgs>?
事件仅由可交互 item 的 pointer 或 keyboard 激活路径触发，参数提供目标步骤编号。调用方决定是否更新 `Current`。当前 item 被重复激活时不发出请求。

## 使用示例

稳定示例来源于 Gallery ShowCase 和源码查看片段。生成器只输出可从 `ShowCaseItem` 追溯的示例，不维护第二套手写示例。

以下示例来自 Gallery 源码查看使用的 `ShowCaseItem` 片段，并已按中文资源规范化。

### 基础用法

来源：`controlgallery/AtomUIGallery/ShowCases/Navigation/Steps/Views/StepsShowCase.axaml:92`

SourceKey：`steps-basic`

```axaml
<atom:Steps Current="0">
    <atom:StepsItem Header="已完成" Content="这是一段描述。" />
    <atom:StepsItem Header="进行中" Content="这是一段描述。" SubHeader="剩余 00:00:08" />
    <atom:StepsItem Header="等待中" Content="这是一段描述。" />
</atom:Steps>
```

### 迷你版本

来源：`controlgallery/AtomUIGallery/ShowCases/Navigation/Steps/Views/StepsShowCase.axaml:110`

SourceKey：`steps-small`

```axaml
<atom:Steps Current="0" SizeType="Small">
    <atom:StepsItem Header="已完成" Content="这是一段描述。" />
    <atom:StepsItem Header="进行中" Content="这是一段描述。" SubHeader="剩余 00:00:08" />
    <atom:StepsItem Header="等待中" Content="这是一段描述。" />
</atom:Steps>
```

### 垂直方向

来源：`controlgallery/AtomUIGallery/ShowCases/Navigation/Steps/Views/StepsShowCase.axaml:193`

SourceKey：`steps-vertical`

```axaml
<atom:Steps Current="1" Orientation="Vertical">
    <atom:StepsItem Header="已完成" Content="这是一段描述。" />
    <atom:StepsItem Header="进行中" Content="这是一段描述。" SubHeader="剩余 00:00:08" />
    <atom:StepsItem Header="等待中" Content="这是一段描述。" />
</atom:Steps>
```

### 垂直迷你版本

来源：`controlgallery/AtomUIGallery/ShowCases/Navigation/Steps/Views/StepsShowCase.axaml:211`

SourceKey：`steps-vertical-small`

```axaml
<atom:Steps Current="1" Orientation="Vertical" SizeType="Small">
    <atom:StepsItem Header="已完成" Content="这是一段描述。" />
    <atom:StepsItem Header="进行中" Content="这是一段描述。" SubHeader="剩余 00:00:08" />
    <atom:StepsItem Header="等待中" Content="这是一段描述。" />
</atom:Steps>
```

## 状态模型

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

## 主题与 Design Token

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

Token 来源：

StepsToken 描述步骤标题、详情内容、Indicator、Dot、OutlineDot、Connector、Navigation、Inline、Panel 和 Progress ring 的组件级视觉语义。

StepsToken 不承载：

- Current、Initial、item index 或 StepNumber。
- public Status、AutomaticStatus、EffectiveStatus、IsCurrent 或 ConnectorStatus。
- item 数量、layout bounds、pointer、keyboard、focus 或 Wave 播放状态。
- Percent 当前值、CanInvoke、IsItemClickable 或 IsMotionEnabled。
- `ItemHeaderForeground`、`ItemSubHeaderForeground` 或 `ItemRailBackground` 的实例值。

## AOT 与裁剪注意事项

资源边界：

- TokenResource 和 SharedToken 只提供视觉值，不保存实例状态。
- 根展示属性使用 AXAML Ancestor Binding 投影到 item 和 internal panel。
- 固定模板关系使用 TemplateBinding、Ancestor Binding 和 selector，不使用字符串路径反射。
- item 语义样式使用静态注册的 StyledProperty 和普通 Avalonia binding 投影，不生成运行时 selector，也不依赖内部节点名称从控件外穿透模板。

性能边界：

- 单容器准备 O(1)。
- 单 item Status 变化 O(1)。
- 根状态变化和 Reset O(n)。
- 不维护第二份 item 列表、状态字典或延迟更新队列。
- 三个 Panel 在 Measure/Arrange 中不创建视觉，不修改 public 状态。

AOT 边界：

- 不新增运行时反射扫描、动态类型注册或编译期不可分析的 binding 路径。
- 不通过反射访问 Wave 播放状态；测试使用 internal 可观察入口或渲染结果。
- 新 internal panel 由静态 AXAML 和显式类型引用创建。
- 三项语义样式投影不使用反射、动态属性发现或运行时类型扫描。

## 源码索引

主要源码：

- `src/AtomUI.Desktop.Controls/Steps/Steps.cs`：public API、事件、容器生成、根输入分发和 item 状态协调。
- `src/AtomUI.Desktop.Controls/Steps/Steps.SemanticParts.cs`：`Steps` 的 Semantic Part descriptor（`root` + `item` + 七个 item 子 Part）。
- `src/AtomUI.Desktop.Controls/Steps/StepsItem.cs`：public item 契约、internal 派生状态、owner 生命周期和激活入口。
- `src/AtomUI.Desktop.Controls/Steps/StepsItemIndicator.cs`：Indicator 状态、Wave part、Progress 绘制和渲染失效。
- `src/AtomUI.Desktop.Controls/Steps/StepsPanel.cs`：item 间水平 flex、Navigation/Panel 等宽、Inline 和垂直 stack 布局。
- `src/AtomUI.Desktop.Controls/Steps/StepsItemLayoutPanel.cs`：Indicator、Section、Connector、NavigationArrow、PanelArrow 和 NavigationActiveIndicator 的 item 内布局。
- `src/AtomUI.Desktop.Controls/Steps/StepsItemSectionPanel.cs`：Header、SubHeader 与 Content 正文节点的分组测量与排列。
- `src/AtomUI.Desktop.Controls/Steps/StepsPanelArrow.cs`：internal 可拉伸 Panel 楔形箭头绘制控件，负责 LTR/RTL 三角形和边框。
- `src/AtomUI.Desktop.Controls/Steps/StepsPanelItemFrame.cs`：Panel item 的视觉外框；Filled 非首项使用左侧 notch 几何裁剪，Outlined 保持完整边框。
- `src/AtomUI.Desktop.Controls/Steps/StepsToken.cs`：Steps 控件 Token。
- `src/AtomUI.Desktop.Controls/Steps/Themes/StepsTheme.axaml`：根模板和 StepsPanel。
- `src/AtomUI.Desktop.Controls/Steps/Themes/StepsItemTheme.axaml`：统一 item 语义模板和状态样式。
- `src/AtomUI.Desktop.Controls/Steps/Themes/StepsItemIndicatorTheme.axaml`：统一 Indicator、Dot、Icon、Progress 和 Wave 模板。

测试目录：

- `tests/AtomUI.Desktop.Controls.Tests/Steps`：状态、Items、交互、Wave、Progress、布局、生命周期和 Semantic Part 回归测试。

Gallery 目录：

- `controlgallery/AtomUIGallery/ShowCases/Navigation/Steps`：示例、源码片段和本地化资源。

## 相关文档

- 源设计文档：`docs/controls/desktop/navigation/steps/overview.md`
- 实现文档：`docs/controls/desktop/navigation/steps/implementation.md`
- Semantic Part 文档：`docs/controls/desktop/navigation/steps/semantic-part.md`
- Token 文档：`docs/controls/desktop/navigation/steps/token.md`
- 变更记录：`docs/controls/desktop/navigation/steps/changelog.md`
- 语义结构：`./semantic-cn.md`
