# Timeline

> 生成产物：由源文档生成，不要手工编辑。修改内容请回到控件文档、Gallery API / Token 表、Gallery ShowCase 或源码结构。

## 概述

Timeline 是 AtomUI 桌面控件体系中的时间轴控件，用于按顺序展示事件节点、状态和时间信息。

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
| 内容承载 | 用户数据、展示内容、集合项或操作入口如何进入控件。 | `IndicatorIcon`、`PendingIcon`。 |
| 状态反馈 | public API、内部状态和伪类如何形成用户可感知反馈。 | visual option。 |
| 主题语义 | ControlTheme、SharedToken、组件 Token 和模板绑定如何表达视觉。 | Timeline Token + ControlTheme。 |

## 公共 API

Timeline 的公共契约由 public/protected 类型成员、Avalonia 属性、事件、命令、template part、伪类、ControlTheme key 和资源 key 共同组成。维护时应先确认这些契约是否已经被源码、Gallery 示例或文档暴露。

核心 public surface 按语义分组维护：

| 契约组 | 代表成员 | 维护含义 |
| --- | --- | --- |
| 内容与数据 | `IndicatorIcon`、`PendingIcon` | 定义控件展示内容、输入数据、模板或业务对象入口。 |
| 选择与集合 | `Mode` | 维护选择、展开、过滤、分页、分组或集合状态。 |
| 交互与状态 | `IsLabelLayout`、`IsOdd`、`IsReverse` | 表达用户可观察状态、可用性、清除、加载或反馈语义。 |
| 视觉与布局 | `IndicatorColor` | 影响尺寸、位置、颜色、形状、密度和模板视觉变量。 |
| 其他稳定入口 | `Label`、`Pending` | 保留为 public surface，变更前需确认 Gallery 和用户 XAML 依赖。 |

当前没有抽取到控件专属 public 事件；交互通知主要来自继承事件、命令或 Gallery 可观察状态。

主要公开类型与枚举：

- 类型：`AbstractTimeline`、`AbstractTimelineItem`、`Timeline`、`TimelineIndicator`、`TimelineItem`、`TimelineItemPanel`、`TimelineStackPanel`。
- 枚举：`TimelineMode`。

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

来源：`controlgallery/AtomUIGallery/ShowCases/DataDisplay/Timeline/Views/TimelineShowCase.axaml:140`

Gallery key：`ExamplesContent` / item `0`

```axaml
<atom:Timeline>
    <atom:TimelineItem Content="2024-01-01 AtomUI 正式启动" />
    <atom:TimelineItem IndicatorColor="green" Content="2024-08-12 经过 7 个多月的开发，AtomUI 正式开源。欢迎大家关注我们。" />
    <atom:TimelineItem IndicatorColor="red" Content="2024-10-01 发布 0.0.1 预览版" />
</atom:Timeline>
```

### 颜色

来源：`controlgallery/AtomUIGallery/ShowCases/DataDisplay/Timeline/Views/TimelineShowCase.axaml:153`

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

来源：`controlgallery/AtomUIGallery/ShowCases/DataDisplay/Timeline/Views/TimelineShowCase.axaml:168`

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

### 标签

来源：`controlgallery/AtomUIGallery/ShowCases/DataDisplay/Timeline/Views/TimelineShowCase.axaml:208`

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
                          x:Name="ModeLeft"
                          Tag="{x:Static atom:TimelineMode.Left}"
                          IsCheckedChanged="ModeChecked"
                          Content="左侧" />
        <atom:RadioButton x:Name="ModeRight"
                          Tag="{x:Static atom:TimelineMode.Right}"
                          IsCheckedChanged="ModeChecked"
                          Content="右侧" />
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
Public API / inherited command / item source / user input
  -> 控件实例状态
  -> effective state / pseudo-class / template property
  -> ControlTheme selector / presenter / renderer
  -> Gallery 可观察行为
```

状态维护规则：

- Disabled 或不可交互状态优先屏蔽 pointer、keyboard、motion 和提交类反馈。
- visual option 状态由控件实例或明确的数据 owner 推导，不能在 template part 之间双向竞争。
- 模板重套用时必须把 public API 对应状态回放到新的 part、伪类和主题变量。
- 集合、弹层、异步、动效或窗口相关状态必须能处理 reset、close、cancel、detach 和 owner 释放。

## 主题与 Design Token

Timeline 的视觉模型由控件模板、ControlTheme、SharedToken 和必要的组件 Token 共同构成。

| 主题文件 | 职责 |
| --- | --- |
| `TimelineIndicatorTheme.axaml` | 提供控件模板、selector、资源绑定和状态视觉。 |
| `TimelineItemTheme.axaml` | 定义集合项、容器项或局部单元的状态视觉。 |
| `TimelineTheme.axaml` | 提供控件模板、selector、资源绑定和状态视觉。 |
| `TimelineThemes.axaml` | 聚合控件家族主题资源，保证包级引入顺序稳定。 |

Timeline 使用 `TimelineToken` 作为组件 Token scope。Token 只表达组件视觉语义，不承载 visual option 运行时状态。

主题维护规则：

- 不删除或重命名已经稳定的 ControlTheme key、template part、伪类和资源 key。
- 不把可由 AXAML 表达的模板状态迁移为 C# 动态创建视觉。
- 不把 hover、pressed、selected、expanded、loading、filter、popup open 等运行时状态写入 Token。
- Browser 或平台特化主题必须保持同一 API 的语义一致。

Token 来源：

Timeline Token 只表达组件级视觉变量，例如尺寸、间距、颜色、圆角、阴影、图标尺寸和弹层边界。Token 不承载运行时选择、展开、加载、错误、上传任务、过滤条件或业务状态。

当前 Token scope：

- `TimelineToken`，scope id 为 `Timeline`，源码位于 `src/AtomUI.Desktop.Controls/Timeline/TimelineToken.cs`。

## AOT 与裁剪注意事项

资源和 AOT 约束：

- 不通过运行时反射扫描 public API、Token 或 Gallery 表格数据。
- 不把可静态声明的模板结构迁移到 C# 动态创建。
- 异步加载、上传、弹层和窗口生命周期必须能取消或释放。
- 缓存对象必须与控件、窗口、弹层或数据 owner 生命周期一致。
- Source generator 生成文件不手工编辑；需要修改时改输入源或 generator。

性能边界：

- 控件应优先复用 Avalonia 原生虚拟化、模板绑定和资源系统。
- 避免为每次状态变化创建不必要的视觉对象、订阅或动画对象。
- 大集合控件必须保证 container recycle 后不会泄漏旧 item 状态。

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
- Gallery 文件只展示用法、API 表和 Token 表，不作为运行时逻辑 owner。

## 相关文档

- 源设计文档：`docs/controls/desktop/data-display/timeline/overview.md`
- 实现文档：`docs/controls/desktop/data-display/timeline/implementation.md`
- Token 文档：`docs/controls/desktop/data-display/timeline/token.md`
- 变更记录：`docs/controls/desktop/data-display/timeline/changelog.md`
- 语义结构：`./semantic-cn.md`
