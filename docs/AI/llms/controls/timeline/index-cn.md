# Timeline

> 生成产物：由源文档生成，不要手工编辑。修改内容请回到控件文档、源码 public surface、Token 类型或生成数据、Gallery ShowCase 或源码结构。

## 概述

Timeline 是 AtomUI 桌面控件体系中的时间轴控件，用于沿垂直或水平方向按顺序展示事件节点、状态和时间信息。控件使用同一套 Item、Indicator、Panel 和主题结构表达两种方向，不创建方向专用的平行控件家族。

Timeline 不负责日历、列表排序或流程引擎。这些职责应由业务层、组合控件或更专用的 AtomUI 控件承担。

主要源码入口：

- `src/AtomUI.Controls/Timeline`
- `src/AtomUI.Desktop.Controls/Timeline`

## 包与命名空间

| 项 | 值 |
| --- | --- |
| NuGet 包 | `AtomUI.Desktop.Controls` |
| .NET 命名空间 | `AtomUI.Desktop.Controls` |
| AXAML 命名空间 | `https://atomui.net` |
| Gallery 页面 | `controlgallery/AtomUIGallery/ShowCases/DataDisplay/Timeline` |
| 状态 | Stable |

## 何时使用

Timeline 的设计语言围绕控件职责、可观察状态和主题契约组织，而不是围绕模板节点组织。

| 维度 | 含义 | Timeline 中的表达 |
| --- | --- | --- |
| 产品语义 | 控件在界面中承担的稳定职责。 | Timeline 是 AtomUI 桌面控件体系中的时间轴控件，用于按顺序展示事件节点、状态和时间信息。 |
| 内容承载 | 用户数据、展示内容、集合项或操作入口如何进入控件。 | `Content`、`Label`、`IndicatorIcon`、`Pending`、`PendingIcon`。 |
| 状态反馈 | public API、内部状态和伪类如何形成用户可感知反馈。 | `IsReverse`、Pending 状态、可见项顺序和首尾节点状态。 |
| 布局语义 | 主轴方向和内容相对轴线的位置如何组合。 | `Orientation` 决定主轴，`Mode` 决定交叉轴上的 `Start`、`End` 或交替布局。 |
| 主题语义 | ControlTheme、SharedToken、组件 Token 和模板绑定如何表达视觉。 | Timeline Token、方向 selector、Item 模板和 Indicator renderer。 |

## 公共 API

Timeline 的公共契约由 public/protected 类型成员、Avalonia 属性、事件、命令、template part、伪类、ControlTheme key 和资源 key 共同组成。维护时应先确认这些契约是否已经被源码、Gallery 示例或文档暴露。

核心 public surface 按语义分组维护：

| 契约组 | 代表成员 | 维护含义 |
| --- | --- | --- |
| 内容与数据 | `Content`、`Label`、`IndicatorIcon`、`Pending`、`PendingIcon` | 定义单项内容、时间标签、节点图标和待处理节点入口。 |
| 集合顺序 | `Items`、`ItemsSource`、`IsReverse` | 维护源顺序、最终视觉顺序和 Pending 项的相邻关系。 |
| 方向与模式 | `Orientation`、`Mode` | 决定主轴方向以及内容位于轴线的 Start、End 或交替侧。 |
| 内部派生状态 | `IsLabelLayout`、`IsOdd`、`IsFirst`、`IsLast`、`NextIsPending` | 由 Timeline 根据可见项视觉顺序单向投影到 Item 和模板。 |
| 视觉与布局 | `IndicatorColor`、`IndicatorIcon` | 影响节点颜色、形状和轴线渲染。 |
| 其他稳定入口 | `Label`、`Pending` | 保留为 public surface，变更前需确认 Gallery 和用户 XAML 依赖。 |

当前没有抽取到控件专属 public 事件；交互通知主要来自继承事件、命令或 Gallery 可观察状态。

主要公开类型与枚举：

- 类型：`AbstractTimeline`、`AbstractTimelineItem`、`Timeline`、`TimelineItem`。
- 枚举：`TimelineMode`。

`TimelineIndicator`、`TimelineItemPanel` 和 `TimelineStackPanel` 是影响可观察布局与渲染的 internal 协作类型，不属于用户可直接依赖的 public surface。

方向与模式的公共契约为：

```csharp
public enum TimelineMode
{
    Start,
    End,
    Alternate
}
```

- `Orientation` 使用 `Avalonia.Layout.Orientation`，默认值为 `Vertical`。
- `Mode` 默认值为 `TimelineMode.Start`。
- `Start` 和 `End` 是相对 Timeline 轴线的逻辑位置，不是固定的物理 Left/Right。
- `Alternate` 从最终视觉顺序中的第一个可见项开始按 `Start`、`End` 交替。
- Timeline 不提供单个 `TimelineItem` 的 placement 覆盖属性；位置策略由 Timeline 统一拥有。

稳定 template part：

| Template Part | 类型 | 职责 |
| --- | --- | --- |
| `PART_IconPresenter` | `IconPresenter` | 展示用户内容、文本、图标或模板化数据。 |

控件专属或内部伪类包括 `OrderEvenPC`、`OrderFirstPC`、`OrderLastPC`、`OrderOddPC`、`PendingItemPC`。这些伪类属于主题 selector 可观察契约，不能在未同步主题和 Gallery 的情况下重命名或删除。

## 事件与命令

Timeline 的公共契约由 public/protected 类型成员、Avalonia 属性、事件、命令、template part、伪类、ControlTheme key 和资源 key 共同组成。维护时应先确认这些契约是否已经被源码、Gallery 示例或文档暴露。
当前没有抽取到控件专属 public 事件；交互通知主要来自继承事件、命令或 Gallery 可观察状态。

## 使用示例

稳定示例来源于 Gallery ShowCase 和源码查看片段。生成器只输出可从 `ShowCaseItem` 追溯的示例，不维护第二套手写示例。

以下示例来自 Gallery 源码查看使用的 `ShowCaseItem` 片段，并已按中文资源规范化。

### 基础用法

来源：`controlgallery/AtomUIGallery/ShowCases/DataDisplay/Timeline/Views/TimelineShowCase.axaml:36`

Gallery key：`ExamplesContent` / item `0`

```axaml
<atom:Timeline>
    <atom:TimelineItem Content="2024-01-01 AtomUI 正式启动" />
    <atom:TimelineItem IndicatorColor="green" Content="2024-08-12 经过 7 个多月的开发，AtomUI 正式开源。欢迎大家关注我们。" />
    <atom:TimelineItem IndicatorColor="red" Content="2024-10-01 发布 0.0.1 预览版" />
</atom:Timeline>
```

### 颜色

来源：`controlgallery/AtomUIGallery/ShowCases/DataDisplay/Timeline/Views/TimelineShowCase.axaml:49`

Gallery key：`ExamplesContent` / item `1`

```axaml
<atom:Timeline>
    <atom:TimelineItem IndicatorColor="green" Content="2024-01-01 AtomUI 正式启动" />
    <atom:TimelineItem IndicatorColor="blue" Content="2024-01-01 AtomUI 正式启动" />
    <atom:TimelineItem IndicatorColor="Red" Content="2024-01-01 AtomUI 正式启动" />
    <atom:TimelineItem IndicatorColor="gray" Content="2024-01-01 AtomUI 正式启动" />
    <atom:TimelineItem IndicatorColor="#00CCFF" Content="2024-01-01 AtomUI 正式启动" />
</atom:Timeline>
```

### 最后节点和反转

来源：`controlgallery/AtomUIGallery/ShowCases/DataDisplay/Timeline/Views/TimelineShowCase.axaml:64`

Gallery key：`ExamplesContent` / item `2`

```axaml
<StackPanel>
    <atom:Timeline
        Pending="记录中..."
        IsReverse="{Binding ReverseTimelineIsReverse}"
        x:Name="ReverseTimeline">
        <atom:TimelineItem Label="2024-01-01" Content="2024-01-01 AtomUI 正式启动。1" />
        <atom:TimelineItem Label="2024-08-12" Content="2024-01-01 AtomUI 正式启动。2" />
        <atom:TimelineItem Label="2024-10-01" Content="2024-01-01 AtomUI 正式启动。3" />
    </atom:Timeline>
    <DockPanel>
        <atom:Button ButtonType="Primary"
                     x:Name="ReverseButton"
                     Click="ReverseButtonClick"
                     Content="切换反转" />
    </DockPanel>
</StackPanel>
```

### 动态模式

来源：`controlgallery/AtomUIGallery/ShowCases/DataDisplay/Timeline/Views/TimelineShowCase.axaml:104`

Gallery key：`ExamplesContent` / item `4`

```axaml
<StackPanel>
    <WrapPanel Margin="0,0,0,20" Orientation="Horizontal">
        <WrapPanel.Styles>
            <Style Selector="atom|RadioButton">
                <Setter Property="Margin" Value="5" />
            </Style>
        </WrapPanel.Styles>
        <atom:RadioButton IsChecked="True"
                          x:Name="ModeStart"
                          Tag="{x:Static atom:TimelineMode.Start}"
                          IsCheckedChanged="ModeChecked"
                          Content="起始" />
        <atom:RadioButton x:Name="ModeEnd"
                          Tag="{x:Static atom:TimelineMode.End}"
                          IsCheckedChanged="ModeChecked"
                          Content="结束" />
        <atom:RadioButton x:Name="ModeAlternate"
                          Tag="{x:Static atom:TimelineMode.Alternate}"
                          IsCheckedChanged="ModeChecked"
                          Content="交替" />
    </WrapPanel>
    <atom:Timeline Mode="{Binding SelectedTimelineMode}" x:Name="LabelTimeline">
        <atom:TimelineItem Label="2024-01-01" Content="AtomUI 正式启动" />
        <atom:TimelineItem Label="2015-09-01 09:12:11" Content="创建服务站点" />
        <atom:TimelineItem Content="Qinware 网站上线" />
        <atom:TimelineItem Label="2029-09-01" Content="网络问题正在解决" />
    </atom:Timeline>
</StackPanel>
```

## 状态模型

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

## 主题与 Design Token

Timeline 的视觉模型由控件模板、ControlTheme、SharedToken 和必要的组件 Token 共同构成。

| 主题文件 | 职责 |
| --- | --- |
| `TimelineIndicatorTheme.axaml` | 提供控件模板、selector、资源绑定和状态视觉。 |
| `TimelineItemTheme.axaml` | 定义集合项、容器项或局部单元的状态视觉。 |
| `TimelineTheme.axaml` | 提供控件模板、selector、资源绑定和状态视觉。 |
| `TimelineThemes.axaml` | 聚合控件家族主题资源，保证包级引入顺序稳定。 |

Timeline 使用 `TimelineToken` 作为组件 Token scope。Token 只表达组件视觉语义，不承载方向、Mode、视觉索引或 Pending 相邻状态。水平布局的内容间距优先使用 SharedToken；方向差异由 ControlTheme selector 和布局 Panel 表达。

主题维护规则：

- 不删除或重命名已经稳定的 ControlTheme key、template part、伪类和资源 key。
- 不把可由 AXAML 表达的模板状态迁移为 C# 动态创建视觉。
- 不把 hover、pressed、selected、expanded、loading、filter、popup open 等运行时状态写入 Token。
- Browser 或平台特化主题必须保持同一 API 的语义一致。

Token 来源：

Timeline Token 只表达节点、连接线和 Item 的组件级尺寸、间距与颜色。Token 不承载 Orientation、Mode、视觉索引、首尾、Reverse、Pending 邻接或其他实例运行时状态。

当前 Token scope：

- `TimelineToken`，scope id 为 `Timeline`，源码位于 `src/AtomUI.Desktop.Controls/Timeline/TimelineToken.cs`。

## AOT 与裁剪注意事项

资源和 AOT 约束：

- 不通过运行时反射扫描 public API、Orientation、Mode、Token 或 Gallery 示例数据。
- 不把可静态声明的 TimelineItem 模板迁移到 C# 动态创建，也不为 Horizontal 创建第二套视觉树。
- 视觉顺序重算为 O(N)，只在结构状态变化时执行，不进入 Render 热路径。
- 水平 Measure/Arrange 为 O(N)，只使用已有容器和局部尺寸值。
- TimelineIndicator 的 dot Pen 和 line Pen 与 Brush/Width 缓存键保持一致，属性变化时精准失效。
- 新增属性使用静态 AvaloniaProperty 注册和 AXAML 绑定，不引入反射、动态发现或 trimming 风险。
- Source generator 和 LLMS 生成文件不手工编辑；需要修改时更新源码、主题、Gallery 和人工维护文档源。

性能边界：

- 控件优先复用 Avalonia 属性失效、模板绑定和资源系统。
- 运行时切换 Orientation 或 Mode 不能创建新 Item、Panel、Indicator、订阅或动画对象。
- 容器重用后必须覆盖 Orientation、Mode、顺序和 Pending 派生状态，不能保留旧 Item 状态。

## 源码索引

主要源码文件：

- `src/AtomUI.Controls/Timeline/AbstractTimeline.cs`
- `src/AtomUI.Controls/Timeline/AbstractTimelineItem.cs`
- `src/AtomUI.Controls/Timeline/TimeLineEnums.cs`
- `src/AtomUI.Controls/Timeline/TimelineIndicator.cs`
- `src/AtomUI.Controls/Timeline/TimelineItemPanel.cs`
- `src/AtomUI.Controls/Timeline/TimelineStackPanel.cs`
- `src/AtomUI.Desktop.Controls/Timeline/Themes/TimelineIndicatorTheme.axaml`
- `src/AtomUI.Desktop.Controls/Timeline/Themes/TimelineItemTheme.axaml`
- `src/AtomUI.Desktop.Controls/Timeline/Themes/TimelineTheme.axaml`
- `src/AtomUI.Desktop.Controls/Timeline/Themes/TimelineThemes.axaml`
- `src/AtomUI.Desktop.Controls/Timeline/Timeline.cs`
- `src/AtomUI.Desktop.Controls/Timeline/TimelineItem.cs`
- `src/AtomUI.Desktop.Controls/Timeline/TimelineToken.cs`

职责边界：

- 控件主文件保留 public/protected API、Avalonia 属性注册、事件和主要生命周期入口。
- Theme 文件负责静态视觉结构、template part、selector 和资源绑定。
- Token 文件只提供组件视觉变量，不保存实例状态。
- Gallery 文件只展示用法和示例，不作为运行时逻辑 owner。

## 相关文档

- 源设计文档：`docs/controls/desktop/data-display/timeline/overview.md`
- 实现文档：`docs/controls/desktop/data-display/timeline/implementation.md`
- Token 文档：`docs/controls/desktop/data-display/timeline/token.md`
- 变更记录：`docs/controls/desktop/data-display/timeline/changelog.md`
- 语义结构：`./semantic-cn.md`
