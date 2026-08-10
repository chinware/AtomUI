# Timeline 语义结构

> 生成产物：由源文档生成，不要手工编辑。修改内容请回到控件文档、源码 public surface、Token 类型或生成数据、Gallery ShowCase 或源码结构。

## Semantic Parts

| Part | AtomUI 节点 | 职责 | 相关 API | 相关 Token | 稳定性 |
| --- | --- | --- | --- | --- | --- |
| `root` | `Timeline` | 承载 Items、Orientation、Mode、IsReverse、Pending 和主题入口。 | `Orientation`、`Mode`、`IsReverse`、`Pending` | 见视觉与主题模型 | stable |
| `item` | `TimelineItem` | 承载单项 Label、Content、Indicator，并接收视觉顺序派生状态。 | `Label`、`Content`、`IndicatorIcon`、`IndicatorColor` | 见视觉与主题模型 | stable |
| `axis` | `TimelineIndicator` | 绘制垂直或水平轴线、节点和自定义图标。 | `Orientation`、`IndicatorIcon`、`IndicatorColor` | `IndicatorTailColor`、`IndicatorTailWidth`、`IndicatorSize` | internal-observable |
| `label` | `TextBlock#Label` | 承载可选时间标签，并参与双侧布局。 | `Label`、`Mode`、`Orientation` | 间距类 Token / SharedToken | template-stable |
| `content` | `ContentPresenter#ContentPresenter` | 承载事件内容并在水平等宽槽位内换行。 | `Content`、`ContentTemplate`、`Mode`、`Orientation` | 间距类 Token / SharedToken | template-stable |

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
     -> IconPresenter#PART_IconPresenter (template-stable)
  -> TimelineItem (item container control theme, TimelineItemTheme.axaml)
     -> TimelineItemPanel#RootLayout (template-stable)
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
| `TimelineIndicator` | control theme | `TimelineIndicatorTheme.axaml` | Timeline | `IndicatorColor`, `IndicatorIcon` | internal-observable | 用于理解结构和状态流，不应指导用户代码直接依赖。 |
| `PART_IconPresenter` | template node (IconPresenter) | `TimelineIndicatorTheme.axaml` | TimelineIndicator | `IndicatorIcon` | template-stable | 用于主题维护；变更需同步主题、实现和 LLMS。 |
| `TimelineItem` | item container control theme | `TimelineItemTheme.axaml` | 用户代码 / 控件宿主 | `Content`, `ContentTemplate`, `IndicatorColor`, `IndicatorIcon`, `IsFirst`, `IsLabelLayout` | public | 用户可直接使用 public 控件；可作为示例和 API 入口。 |
| `RootLayout` | template node (TimelineItemPanel) | `TimelineItemTheme.axaml` | TimelineItem | `Content`, `ContentTemplate`, `IndicatorColor`, `IndicatorIcon`, `IsFirst`, `IsLabelLayout` | template-stable | 用于主题维护；变更需同步主题、实现和 LLMS。 |
| `Label` | template node (TextBlock) | `TimelineItemTheme.axaml` | TimelineItem | `Label` | template-stable | 用于主题维护；变更需同步主题、实现和 LLMS。 |
| `Indicator` | template node (TimelineIndicator) | `TimelineItemTheme.axaml` | TimelineItem | `IndicatorColor`, `IndicatorIcon`, `IsFirst`, `IsLast`, `NextIsPending`, `Orientation` | internal-observable | 用于理解结构和状态流，不应指导用户代码直接依赖。 |
| `ContentPresenter` | template node (ContentPresenter) | `TimelineItemTheme.axaml` | TimelineItem | `Content`, `ContentTemplate` | internal-observable | 用于理解结构和状态流，不应指导用户代码直接依赖。 |
| `Timeline` | control theme | `TimelineTheme.axaml` | 用户代码 / 控件宿主 | `Background`, `BorderBrush`, `CornerRadius`, `Padding`, `atom` | public | 用户可直接使用 public 控件；可作为示例和 API 入口。 |
| `Frame` | template node (Border) | `TimelineTheme.axaml` | Timeline | `Background`, `BorderBrush`, `CornerRadius`, `Padding`, `atom` | template-stable | 用于主题维护；变更需同步主题、实现和 LLMS。 |
| `ScrollViewer` | template node (ScrollViewer) | `TimelineTheme.axaml` | Timeline | `atom` | template-stable | 用于主题维护；变更需同步主题、实现和 LLMS。 |
| `ItemsPresenter` | template node (ItemsPresenter) | `TimelineTheme.axaml` | Timeline | 主题状态 / visual state | internal-observable | 用于理解结构和状态流，不应指导用户代码直接依赖。 |

## Template Parts

| 契约组 | 代表成员 | 维护含义 |
| --- | --- | --- |
| 内容与数据 | `Content`、`Label`、`IndicatorIcon`、`Pending`、`PendingIcon` | 定义单项内容、时间标签、节点图标和待处理节点入口。 |
| 集合顺序 | `Items`、`ItemsSource`、`IsReverse` | 维护源顺序、最终视觉顺序和 Pending 项的相邻关系。 |
| 方向与模式 | `Orientation`、`Mode` | 决定主轴方向以及内容位于轴线的 Start、End 或交替侧。 |
| 内部派生状态 | `IsLabelLayout`、`IsOdd`、`IsFirst`、`IsLast`、`NextIsPending` | 由 Timeline 根据可见项视觉顺序单向投影到 Item 和模板。 |
| 视觉与布局 | `IndicatorColor`、`IndicatorIcon` | 影响节点颜色、形状和轴线渲染。 |
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
| `Horizontal` | `Start` | 轴线在上，Content 在下。 | Label 在上，Content 在下。 |
| `Horizontal` | `End` | Content 在上，轴线在下。 | Content 在上，Label 在下。 |
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
