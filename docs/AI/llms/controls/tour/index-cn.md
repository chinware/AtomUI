# Tour

> 生成产物：由源文档生成，不要手工编辑。修改内容请回到控件文档、Gallery API / Token 表、Gallery ShowCase 或源码结构。

## 概述

Tour 是 AtomUI 桌面控件体系中的漫游引导控件，用于按步骤高亮界面目标并展示说明。

Tour 不负责路由系统、教学内容管理或通用 Dialog。这些职责应由业务层、组合控件或更专用的 AtomUI 控件承担。

主要源码入口：

- `src/AtomUI.Desktop.Controls/Tour`

## 包与命名空间

| 项 | 值 |
| --- | --- |
| NuGet 包 | `AtomUI.Desktop.Controls` |
| .NET 命名空间 | `AtomUI.Desktop.Controls` |
| AXAML 命名空间 | `https://atomui.net` |
| Gallery 页面 | `controlgallery/AtomUIGallery/ShowCases/DataDisplay/Tour` |
| 状态 | Stable |

## 何时使用

Tour 的设计语言围绕控件职责、可观察状态和主题契约组织，而不是围绕模板节点组织。

| 维度 | 含义 | Tour 中的表达 |
| --- | --- | --- |
| 产品语义 | 控件在界面中承担的稳定职责。 | Tour 是 AtomUI 桌面控件体系中的漫游引导控件，用于按步骤高亮界面目标并展示说明。 |
| 内容承载 | 用户数据、展示内容、集合项或操作入口如何进入控件。 | `CloseIcon`、`CoverTemplate`、`Description`、`DescriptionTemplate`、`ItemSpacing`、`ItemTemplate`、`Title`、`TitleTemplate`。 |
| 状态反馈 | public API、内部状态和伪类如何形成用户可感知反馈。 | selection/checked/active、open/close、motion、visual option。 |
| 主题语义 | ControlTheme、SharedToken、组件 Token 和模板绑定如何表达视觉。 | Tour Token + ControlTheme。 |

## 公共 API

Tour 的公共契约由 public/protected 类型成员、Avalonia 属性、事件、命令、template part、伪类、ControlTheme key 和资源 key 共同组成。维护时应先确认这些契约是否已经被源码、Gallery 示例或文档暴露。

核心 public surface 按语义分组维护：

| 契约组 | 代表成员 | 维护含义 |
| --- | --- | --- |
| 内容与数据 | `CloseIcon`、`CoverTemplate`、`Description`、`DescriptionTemplate`、`ItemSpacing`、`ItemTemplate`、`Title`、`TitleTemplate` | 定义控件展示内容、输入数据、模板或业务对象入口。 |
| 选择与集合 | `ActiveIndex`、`CurrentIndex`、`IndicatorActiveColor`、`StepCount` | 维护选择、展开、过滤、分页、分组或集合状态；`CurrentIndex` 默认双向绑定。 |
| 交互与状态 | `IsArrowVisible`、`IsDisabledInteraction`、`IsMotionEnabled`、`IsOpen`、`IsPointAtCenter`、`IsScrollIntoView`、`IsShowMask` | 表达用户可观察状态、可用性、清除、加载或反馈语义；`IsOpen` 默认双向绑定。 |
| 视觉与布局 | `Background`、`GapOffsetX`、`GapOffsetY`、`GapRadius`、`IndicatorColor`、`IndicatorSize`、`MaskColor`、`Placement`、`StyleType`、`TargetRegionCornerRadius` | 影响尺寸、位置、颜色、形状、密度和模板视觉变量。 |
| 其他稳定入口 | `Cover`、`Indicator`、`Target`、`TargetRegion` | 保留为 public surface，变更前需确认 Gallery 和用户 XAML 依赖。 |

当前没有抽取到控件专属 public 事件；交互通知主要来自继承事件、命令或 Gallery 可观察状态。

`IsOpen` 与 `CurrentIndex` 是用户可拥有的受控状态，Avalonia Binding 默认使用 `TwoWay`；开始引导、关闭引导和步骤切换都必须回写同一 public 属性路径。它们不是 Form value，不写入 `DataValidationErrors`。

主要公开类型与枚举：

- 类型：`DefaultTourIndicator`、`TextTourIndicator`、`Tour`、`TourIndicator`、`TourLayer`、`TourStep`、`TourStepNavRequestEventArgs`、`TourStepOption`、`TourStepsView`、`en_US`、`zh_CN`、`zh_TW`。
- 枚举：`TourPlacementMode`、`TourStyleType`。

稳定 template part：

| Template Part | 类型 | 职责 |
| --- | --- | --- |
| `PART_ArrowDecorator` | `?` | 稳定模板协作入口，重命名前必须同步主题和实现。 |
| `PART_Popup` | `?` | 承载弹层宿主、打开关闭或候选内容。 |

当前未抽取到控件专属伪类；主题主要依赖 Avalonia 标准伪类、模板绑定和内部 StyledProperty。

## 事件与命令

Tour 的公共契约由 public/protected 类型成员、Avalonia 属性、事件、命令、template part、伪类、ControlTheme key 和资源 key 共同组成。维护时应先确认这些契约是否已经被源码、Gallery 示例或文档暴露。
当前没有抽取到控件专属 public 事件；交互通知主要来自继承事件、命令或 Gallery 可观察状态。
- 类型：`DefaultTourIndicator`、`TextTourIndicator`、`Tour`、`TourIndicator`、`TourLayer`、`TourStep`、`TourStepNavRequestEventArgs`、`TourStepOption`、`TourStepsView`、`en_US`、`zh_CN`、`zh_TW`。

## 使用示例

稳定示例来源于 Gallery ShowCase 和源码查看片段。生成器只输出可从 `ShowCaseItem` 追溯的示例，不维护第二套手写示例。

以下示例来自 Gallery 源码查看使用的 `ShowCaseItem` 片段，并已按中文资源规范化。

### 位置

来源：`controlgallery/AtomUIGallery/ShowCases/DataDisplay/Tour/Views/TourShowCase.axaml:110`

Gallery key：`ExamplesContent` / item `2`

```axaml
<StackPanel Orientation="Vertical" Spacing="10" Loaded="HandleTourExampleLoaded">
    <atom:Button ButtonType="Primary" Click="HandlePlacementBeginTour" Name="PlacementBeginTour" Content="开始引导" />
    <atom:Tour IsOpen="{Binding PlacementTourOpened, Mode=TwoWay}" IsShowMask="True">
        <atom:TourStep Title="居中"
                       Description="显示在屏幕中央。" />
        <atom:TourStep Name="PlacementRightStep"
                       Title="右侧"
                       Description="位于目标右侧。"
                       Placement="Right" />
        <atom:TourStep Name="PlacementTopStep"
                       Title="顶部"
                       Description="位于目标上方。"
                       Placement="Top" />
        <atom:TourStep Name="PlacementLeftStep"
                       Title="左侧"
                       Description="位于目标左侧。"
                       Placement="Left" />
    </atom:Tour>
</StackPanel>
```

## 状态模型

Tour 的状态流按以下路径收敛：

```text
Public API / inherited command / item source / user input
  -> 控件实例状态
  -> effective state / pseudo-class / template property
  -> ControlTheme selector / presenter / renderer
  -> Gallery 可观察行为
```

状态维护规则：

- Disabled 或不可交互状态优先屏蔽 pointer、keyboard、motion 和提交类反馈。
- selection/checked/active、open/close、motion、visual option 状态由控件实例或明确的数据 owner 推导，不能在 template part 之间双向竞争。
- `IsOpen` 与 `CurrentIndex` 是默认 `TwoWay` 的受控状态；popup、indicator 和步骤视图只能消费或回写 public 状态，不能形成局部当前步骤。
- 模板重套用时必须把 public API 对应状态回放到新的 part、伪类和主题变量。
- 集合、弹层、异步、动效或窗口相关状态必须能处理 reset、close、cancel、detach 和 owner 释放。

## 主题与 Design Token

Tour 的视觉模型由控件模板、ControlTheme、SharedToken 和必要的组件 Token 共同构成。

| 主题文件 | 职责 |
| --- | --- |
| `DefaultTourIndicatorTheme.axaml` | 提供控件模板、selector、资源绑定和状态视觉。 |
| `TextTourIndicatorTheme.axaml` | 提供控件模板、selector、资源绑定和状态视觉。 |
| `TourStepTheme.axaml` | 提供控件模板、selector、资源绑定和状态视觉。 |
| `TourStepsViewTheme.axaml` | 提供控件模板、selector、资源绑定和状态视觉。 |
| `TourTheme.axaml` | 提供控件模板、selector、资源绑定和状态视觉。 |
| `TourThemes.axaml` | 聚合控件家族主题资源，保证包级引入顺序稳定。 |

Tour 使用 `TourToken` 作为组件 Token scope。Token 只表达组件视觉语义，不承载 selection/checked/active、open/close、motion、visual option 运行时状态。

主题维护规则：

- 不删除或重命名已经稳定的 ControlTheme key、template part、伪类和资源 key。
- 不把可由 AXAML 表达的模板状态迁移为 C# 动态创建视觉。
- 不把 hover、pressed、selected、expanded、loading、filter、popup open 等运行时状态写入 Token。
- Browser 或平台特化主题必须保持同一 API 的语义一致。

Token 来源：

Tour Token 只表达组件级视觉变量，例如尺寸、间距、颜色、圆角、阴影、图标尺寸和弹层边界。Token 不承载运行时选择、展开、加载、错误、上传任务、过滤条件或业务状态。

当前 Token scope：

- `TourToken`，scope id 为 `Tour`，源码位于 `src/AtomUI.Desktop.Controls/Tour/TourToken.cs`。

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

- `src/AtomUI.Desktop.Controls/Tour/DefaultTourIndicator.cs`
- `src/AtomUI.Desktop.Controls/Tour/ITourAction.cs`
- `src/AtomUI.Desktop.Controls/Tour/Localization/en_US.cs`
- `src/AtomUI.Desktop.Controls/Tour/Localization/zh_CN.cs`
- `src/AtomUI.Desktop.Controls/Tour/Localization/zh_TW.cs`
- `src/AtomUI.Desktop.Controls/Tour/TextTourIndicator.cs`
- `src/AtomUI.Desktop.Controls/Tour/Themes/DefaultTourIndicatorTheme.axaml`
- `src/AtomUI.Desktop.Controls/Tour/Themes/TextTourIndicatorTheme.axaml`
- `src/AtomUI.Desktop.Controls/Tour/Themes/TourStepTheme.axaml`
- `src/AtomUI.Desktop.Controls/Tour/Themes/TourStepsViewTheme.axaml`
- `src/AtomUI.Desktop.Controls/Tour/Themes/TourTheme.axaml`
- `src/AtomUI.Desktop.Controls/Tour/Themes/TourThemes.axaml`
- `src/AtomUI.Desktop.Controls/Tour/Tour.cs`
- `src/AtomUI.Desktop.Controls/Tour/TourIndicator.cs`
- `src/AtomUI.Desktop.Controls/Tour/TourLayer.cs`
- `src/AtomUI.Desktop.Controls/Tour/TourPlacementMode.cs`
- `src/AtomUI.Desktop.Controls/Tour/TourStep.cs`
- `src/AtomUI.Desktop.Controls/Tour/TourStepNavRequestEventArgs.cs`
- `src/AtomUI.Desktop.Controls/Tour/TourStepOption.cs`
- `src/AtomUI.Desktop.Controls/Tour/TourStepsView.cs`
- `src/AtomUI.Desktop.Controls/Tour/TourToken.cs`

职责边界：

- 控件主文件保留 public/protected API、Avalonia 属性注册、事件和主要生命周期入口。
- Theme 文件负责静态视觉结构、template part、selector 和资源绑定。
- Token 文件只提供组件视觉变量，不保存实例状态。
- Gallery 文件只展示用法、API 表和 Token 表，不作为运行时逻辑 owner。

## 相关文档

- 源设计文档：`docs/controls/desktop/data-display/tour/overview.md`
- 实现文档：`docs/controls/desktop/data-display/tour/implementation.md`
- Token 文档：`docs/controls/desktop/data-display/tour/token.md`
- 变更记录：`docs/controls/desktop/data-display/tour/changelog.md`
- 语义结构：`./semantic-cn.md`
