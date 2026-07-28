# Skeleton 语义结构

> 生成产物：由源文档生成，不要手工编辑。修改内容请回到控件文档、源码 public surface、Token 类型或生成数据、Gallery ShowCase 或源码结构。

## Semantic Parts

| Part | AtomUI 节点 | 职责 | 相关 API | 相关 Token | 稳定性 |
| --- | --- | --- | --- | --- | --- |
| `root` | `Skeleton` | 反馈控件根语义区域，承载 public API、反馈状态和主题入口。 | 见 API 与契约模型 | 见视觉与主题模型 | stable |
| `host` | `宿主或弹层区域` | 承载 overlay、popup、portal、message host、drawer 或 modal 容器。 | 见 API 与契约模型 | 见视觉与主题模型 | stable |
| `surface` | `反馈表面` | 承载背景、边框、阴影、尺寸、placement 和视觉状态。 | 见 API 与契约模型 | 见视觉与主题模型 | stable |
| `content` | `内容区域` | 承载标题、正文、图标、进度、结果、操作或关闭入口。 | 见 API 与契约模型 | 见视觉与主题模型 | stable |
| `motion` | `动效区域` | 表达进入退出、loading、progress、skeleton 或水印刷新反馈。 | 见 API 与契约模型 | 见视觉与主题模型 | stable |

## Abstract AXAML Structure

来源：`src/AtomUI.Desktop.Controls/Skeleton/Themes/SkeletonTheme.axaml`

```xml
<Panel>
    <DockPanel>
        <SkeletonAvatar Name="PART_Avatar" />
        <StackPanel Name="PART_Content">
            <SkeletonTitle Name="PART_Title" />
            <SkeletonParagraph Name="PART_Paragraph" />
        </StackPanel>
    </DockPanel>
    <ContentPresenter />
</Panel>
```

## Composition Model

该章节由控件 `Themes/` 文件夹中的真实主题文件生成，用于说明 public 控件与内部协作对象之间的运行时结构。内部节点只用于理解和维护，不应指导用户代码直接依赖。

### 控件角色图

```text
Skeleton
  -> SkeletonAvatar (control theme, SkeletonAvatarTheme.axaml)
  -> SkeletonButton (control theme, SkeletonButtonTheme.axaml)
  -> SkeletonElement (control theme, SkeletonElementTheme.axaml)
  -> SkeletonImage (control theme, SkeletonImageTheme.axaml)
     -> Panel#PART_RootLayout (template-stable)
        -> Border#PART_ContentLayer (template-stable)
        -> Border#PART_ActiveAnimationLayer (template-stable)
        -> ImageFilled#Image (template-stable)
  -> SkeletonInput (control theme, SkeletonInputTheme.axaml)
  -> SkeletonLine (control theme, SkeletonLineTheme.axaml)
  -> SkeletonNode (control theme, SkeletonNodeTheme.axaml)
     -> Panel#PART_RootLayout (template-stable)
        -> Border#PART_ContentLayer (template-stable)
        -> Border#PART_ActiveAnimationLayer (template-stable)
        -> ContentPresenter#ContentPresenter (internal-observable)
  -> SkeletonParagraph (control theme, SkeletonParagraphTheme.axaml)
     -> StackPanel#PART_LineLayout (template-stable)
  -> Skeleton (control theme, SkeletonTheme.axaml)
     -> Panel (template-stable)
        -> DockPanel (template-stable)
           -> SkeletonAvatar#PART_Avatar (template-stable)
           -> StackPanel#PART_Content (template-stable)
              -> SkeletonTitle#PART_Title (template-stable)
              -> SkeletonParagraph#PART_Paragraph (template-stable)
        -> ContentPresenter (internal-observable)
```

### 协作节点

| 节点 | 类型 | 来源 | 生命周期 owner | 影响的 public API | 稳定性 | Agent 使用边界 |
| --- | --- | --- | --- | --- | --- | --- |
| `Skeleton` | public control | `源文档 + public API` | 用户代码 / 控件宿主 | public API | public | 用户可直接使用 public 控件；可作为示例和 API 入口。 |
| `SkeletonAvatar` | control theme | `SkeletonAvatarTheme.axaml` | 用户代码 / 控件宿主 | 主题状态 / visual state | public | 用户可直接使用 public 控件；可作为示例和 API 入口。 |
| `SkeletonButton` | control theme | `SkeletonButtonTheme.axaml` | 用户代码 / 控件宿主 | 主题状态 / visual state | public | 用户可直接使用 public 控件；可作为示例和 API 入口。 |
| `SkeletonElement` | control theme | `SkeletonElementTheme.axaml` | Skeleton | 主题状态 / visual state | template-stable | 用于主题维护；变更需同步主题、实现和 LLMS。 |
| `SkeletonImage` | control theme | `SkeletonImageTheme.axaml` | 用户代码 / 控件宿主 | `AnimationLayerFill`, `Background`, `CornerRadius`, `IsActive` | public | 用户可直接使用 public 控件；可作为示例和 API 入口。 |
| `PART_RootLayout` | template node (Panel) | `SkeletonImageTheme.axaml` | SkeletonImage | `AnimationLayerFill`, `Background`, `CornerRadius`, `IsActive` | template-stable | 用于主题维护；变更需同步主题、实现和 LLMS。 |
| `PART_ContentLayer` | template node (Border) | `SkeletonImageTheme.axaml` | SkeletonImage | `Background`, `CornerRadius`, `IsActive` | template-stable | 用于主题维护；变更需同步主题、实现和 LLMS。 |
| `PART_ActiveAnimationLayer` | template node (Border) | `SkeletonImageTheme.axaml` | SkeletonImage | `AnimationLayerFill`, `CornerRadius`, `IsActive` | template-stable | 用于主题维护；变更需同步主题、实现和 LLMS。 |
| `Image` | template node (ImageFilled) | `SkeletonImageTheme.axaml` | SkeletonImage | 主题状态 / visual state | template-stable | 用于主题维护；变更需同步主题、实现和 LLMS。 |
| `SkeletonInput` | control theme | `SkeletonInputTheme.axaml` | 用户代码 / 控件宿主 | 主题状态 / visual state | public | 用户可直接使用 public 控件；可作为示例和 API 入口。 |
| `SkeletonLine` | control theme | `SkeletonLineTheme.axaml` | 用户代码 / 控件宿主 | 主题状态 / visual state | public | 用户可直接使用 public 控件；可作为示例和 API 入口。 |
| `SkeletonNode` | control theme | `SkeletonNodeTheme.axaml` | 用户代码 / 控件宿主 | `AnimationLayerFill`, `Background`, `Content`, `ContentTemplate`, `CornerRadius`, `IsActive` | public | 用户可直接使用 public 控件；可作为示例和 API 入口。 |
| `PART_RootLayout` | template node (Panel) | `SkeletonNodeTheme.axaml` | SkeletonNode | `AnimationLayerFill`, `Background`, `Content`, `ContentTemplate`, `CornerRadius`, `IsActive` | template-stable | 用于主题维护；变更需同步主题、实现和 LLMS。 |
| `PART_ContentLayer` | template node (Border) | `SkeletonNodeTheme.axaml` | SkeletonNode | `Background`, `CornerRadius`, `IsActive` | template-stable | 用于主题维护；变更需同步主题、实现和 LLMS。 |
| `PART_ActiveAnimationLayer` | template node (Border) | `SkeletonNodeTheme.axaml` | SkeletonNode | `AnimationLayerFill`, `CornerRadius`, `IsActive` | template-stable | 用于主题维护；变更需同步主题、实现和 LLMS。 |
| `ContentPresenter` | template node (ContentPresenter) | `SkeletonNodeTheme.axaml` | SkeletonNode | `Content`, `ContentTemplate` | internal-observable | 用于理解结构和状态流，不应指导用户代码直接依赖。 |
| `SkeletonParagraph` | control theme | `SkeletonParagraphTheme.axaml` | 用户代码 / 控件宿主 | 主题状态 / visual state | public | 用户可直接使用 public 控件；可作为示例和 API 入口。 |
| `PART_LineLayout` | template node (StackPanel) | `SkeletonParagraphTheme.axaml` | SkeletonParagraph | 主题状态 / visual state | template-stable | 用于主题维护；变更需同步主题、实现和 LLMS。 |
| `Skeleton` | control theme | `SkeletonTheme.axaml` | 用户代码 / 控件宿主 | `AvatarShape`, `AvatarSize`, `AvatarSizeType`, `Content`, `ContentTemplate`, `HorizontalContentAlignment` | public | 用户可直接使用 public 控件；可作为示例和 API 入口。 |
| `Panel` | template node (Panel) | `SkeletonTheme.axaml` | Skeleton | `AvatarShape`, `AvatarSize`, `AvatarSizeType`, `Content`, `ContentTemplate`, `HorizontalContentAlignment` | template-stable | 用于主题维护；变更需同步主题、实现和 LLMS。 |
| `DockPanel` | template node (DockPanel) | `SkeletonTheme.axaml` | Skeleton | `AvatarShape`, `AvatarSize`, `AvatarSizeType`, `IsActive`, `IsContentVisible`, `IsRound` | template-stable | 用于主题维护；变更需同步主题、实现和 LLMS。 |
| `PART_Avatar` | template node (SkeletonAvatar) | `SkeletonTheme.axaml` | Skeleton | `AvatarShape`, `AvatarSize`, `AvatarSizeType`, `IsActive`, `IsShowAvatar` | template-stable | 用于主题维护；变更需同步主题、实现和 LLMS。 |
| `PART_Content` | template node (StackPanel) | `SkeletonTheme.axaml` | Skeleton | `IsActive`, `IsRound`, `IsShowParagraph`, `IsShowTitle`, `ParagraphLastLineWidth`, `ParagraphLineWidths` | template-stable | 用于主题维护；变更需同步主题、实现和 LLMS。 |
| `PART_Title` | template node (SkeletonTitle) | `SkeletonTheme.axaml` | Skeleton | `IsActive`, `IsRound`, `IsShowTitle`, `TitleWidth` | template-stable | 用于主题维护；变更需同步主题、实现和 LLMS。 |
| `PART_Paragraph` | template node (SkeletonParagraph) | `SkeletonTheme.axaml` | Skeleton | `IsActive`, `IsRound`, `IsShowParagraph`, `ParagraphLastLineWidth`, `ParagraphLineWidths`, `ParagraphRows` | template-stable | 用于主题维护；变更需同步主题、实现和 LLMS。 |
| `ContentPresenter` | template node (ContentPresenter) | `SkeletonTheme.axaml` | Skeleton | `Content`, `ContentTemplate`, `HorizontalContentAlignment`, `IsContentVisible`, `VerticalContentAlignment` | internal-observable | 用于理解结构和状态流，不应指导用户代码直接依赖。 |

## Template Parts

| 契约组 | 代表成员 | 维护含义 |
| --- | --- | --- |
| 内容与数据 | `AvatarShape`、`AvatarSize`、`AvatarSizeType`、`Content`、`ContentTemplate`、`HorizontalContentAlignment`、`IsShowAvatar`、`IsShowTitle`、`ParagraphRows`、`Rows` 等 12 项 | 定义控件展示内容、输入数据、模板或业务对象入口。 |
| 选择与集合 | `IsActive` | 维护选择、展开、过滤、分页、分组或集合状态。 |
| 交互与状态 | `IsBlock`、`IsLoading`、`IsRound`、`IsShowParagraph` | 表达用户可观察状态、可用性、清除、加载或反馈语义。 |
| 视觉与布局 | `LastLineWidth`、`LineWidth`、`ParagraphLastLineWidth`、`Shape`、`Size`、`SizeType` | 影响尺寸、位置、颜色、形状、密度和模板视觉变量。 |
| 动效与异步 | `MotionDuration`、`MotionEasingCurve` | 约束动效开关、异步加载、播放速度、超时和任务边界。 |

## Pseudo Classes

| 状态反馈 | public API、内部状态和伪类如何形成用户可感知反馈。 | selection/checked/active、loading/async、motion、visual option。 |
| 主题语义 | ControlTheme、SharedToken、组件 Token 和模板绑定如何表达视觉。 | Skeleton Token + ControlTheme。 |

## State Flow

Skeleton 的状态流按以下路径收敛：

```text
Public API / inherited command / item source / user input
  -> 控件实例状态
  -> effective state / pseudo-class / template property
  -> ControlTheme selector / presenter / renderer
  -> Gallery 可观察行为
```

状态维护规则：

- Disabled 或不可交互状态优先屏蔽 pointer、keyboard、motion 和提交类反馈。
- selection/checked/active、loading/async、motion、visual option 状态由控件实例或明确的数据 owner 推导，不能在 template part 之间双向竞争。
- 模板重套用时必须把 public API 对应状态回放到新的 part、伪类和主题变量。
- 集合、弹层、异步、动效或窗口相关状态必须能处理 reset、close、cancel、detach 和 owner 释放。

## Theme and Token Boundaries

Skeleton 的视觉模型由控件模板、ControlTheme、SharedToken 和必要的组件 Token 共同构成。

| 主题文件 | 职责 |
| --- | --- |
| `AbstractSkeletonTheme.axaml` | 提供控件模板、selector、资源绑定和状态视觉。 |
| `SkeletonAvatarTheme.axaml` | 提供控件模板、selector、资源绑定和状态视觉。 |
| `SkeletonButtonTheme.axaml` | 定义局部操作入口、按钮或 handle 的状态视觉。 |
| `SkeletonElementTheme.axaml` | 提供控件模板、selector、资源绑定和状态视觉。 |
| `SkeletonImageTheme.axaml` | 提供控件模板、selector、资源绑定和状态视觉。 |
| `SkeletonInputTheme.axaml` | 提供控件模板、selector、资源绑定和状态视觉。 |
| `SkeletonLineTheme.axaml` | 提供控件模板、selector、资源绑定和状态视觉。 |
| `SkeletonNodeTheme.axaml` | 提供控件模板、selector、资源绑定和状态视觉。 |
| `SkeletonParagraphTheme.axaml` | 提供控件模板、selector、资源绑定和状态视觉。 |
| `SkeletonTheme.axaml` | 提供控件模板、selector、资源绑定和状态视觉。 |
| `SkeletonThemes.axaml` | 聚合控件家族主题资源，保证包级引入顺序稳定。 |

Skeleton 使用 `SkeletonToken` 作为组件 Token scope。Token 只表达组件视觉语义，不承载 selection/checked/active、loading/async、motion、visual option 运行时状态。

主题维护规则：

- 不删除或重命名已经稳定的 ControlTheme key、template part、伪类和资源 key。
- 不把可由 AXAML 表达的模板状态迁移为 C# 动态创建视觉。
- 不把 hover、pressed、selected、expanded、loading、filter、popup open 等运行时状态写入 Token。
- Browser 或平台特化主题必须保持同一 API 的语义一致。

Token 边界：

Skeleton Token 只表达组件级视觉变量，例如尺寸、间距、颜色、圆角、阴影、图标尺寸和弹层边界。Token 不承载运行时选择、展开、加载、错误、上传任务、过滤条件或业务状态。

当前 Token scope：

- `SkeletonToken`，scope id 为 `Skeleton`，源码位于 `src/AtomUI.Desktop.Controls/Skeleton/SkeletonToken.cs`。

## Customization Boundaries

维护 Skeleton 时必须保持以下不变量：

- 不擅自新增、删除、重命名或改变 public/protected API、Avalonia 属性、事件和默认值。
- 不破坏 template part、伪类、ControlTheme key、Token 名称和资源 key。
- 不改变 Gallery 已展示的 XAML 用法、默认外观、交互顺序和状态优先级。
- Template part 重新应用、集合替换、弹层关闭、窗口失活和控件 detach 时必须释放旧订阅和资源宿主。
- 不通过隐藏延迟、强制刷新或吞异常掩盖状态同步问题。
- 不引入运行时反射扫描作为 API、Token 或数据路径发现机制。
- 文档只描述当前稳定设计；历史变化记录在 `changelog.md`。

维护不变量：

维护 Skeleton 时不得破坏：

- Public API、默认值、事件顺序和 Gallery 可观察行为。
- Template part 名称、ControlTheme key、伪类和资源 key。
- 旧 template part、事件订阅、Popup/Flyout/Window host 和 collection view 的释放路径。
- Light/Dark、Browser/Desktop 和不同 SizeType 下的主题一致性。
- 控件文档、源码 public surface、Token 类型或生成数据与源码契约的一致性。
