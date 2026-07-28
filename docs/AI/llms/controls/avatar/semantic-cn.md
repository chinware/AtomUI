# Avatar 语义结构

> 生成产物：由源文档生成，不要手工编辑。修改内容请回到控件文档、源码 public surface、Token 类型或生成数据、Gallery ShowCase 或源码结构。

## Semantic Parts

| Part | AtomUI 节点 | 职责 | 相关 API | 相关 Token | 稳定性 |
| --- | --- | --- | --- | --- | --- |
| `root` | `Avatar` | 数据展示控件根语义区域，承载 public API、数据状态和主题入口。 | 见 API 与契约模型 | 见视觉与主题模型 | stable |
| `item` | `条目或容器区域` | 承载集合项、单元格、标签、时间节点、卡片或展示单元。 | 见 API 与契约模型 | 见视觉与主题模型 | stable |
| `header` | `标题或头部区域` | 承载标题、字段名、列头、操作入口或摘要信息。 | 见 API 与契约模型 | 见视觉与主题模型 | stable |
| `content` | `内容区域` | 承载主体内容、媒体、文本、空状态、加载状态或详情区域。 | 见 API 与契约模型 | 见视觉与主题模型 | stable |
| `motion` | `动效或浮层区域` | 表达展开收起、轮播、tooltip、tour、预览或虚拟化反馈。 | 见 API 与契约模型 | 见视觉与主题模型 | stable |

## Abstract AXAML Structure

来源：`src/AtomUI.Desktop.Controls/Avatar/Themes/AvatarTheme.axaml`

```xml
<Panel Name="RootLayout">
    <PixelAlignedBorder Name="Frame" />
    <IconPresenter Name="IconPresenter" />
    <Border>
        <Image Name="ImagePresenter" />
    </Border>
    <Border>
        <Svg Name="SvgPresenter" />
    </Border>
    <TextBlock Name="PART_TextPresenter" />
</Panel>
```

## Composition Model

该章节由控件 `Themes/` 文件夹中的真实主题文件生成，用于说明 public 控件与内部协作对象之间的运行时结构。内部节点只用于理解和维护，不应指导用户代码直接依赖。

### 控件角色图

```text
Avatar
  -> AvatarGroup (control theme, AvatarGroupTheme.axaml)
  -> Avatar (control theme, AvatarTheme.axaml)
     -> Panel#RootLayout (template-stable)
        -> PixelAlignedBorder#Frame (template-stable)
        -> IconPresenter#IconPresenter (internal-observable)
        -> Border (template-stable)
           -> Image#ImagePresenter (template-stable)
        -> Border (template-stable)
           -> Svg#SvgPresenter (template-stable)
        -> TextBlock#PART_TextPresenter (template-stable)
```

### 协作节点

| 节点 | 类型 | 来源 | 生命周期 owner | 影响的 public API | 稳定性 | Agent 使用边界 |
| --- | --- | --- | --- | --- | --- | --- |
| `Avatar` | public control | `源文档 + public API` | 用户代码 / 控件宿主 | public API | public | 用户可直接使用 public 控件；可作为示例和 API 入口。 |
| `AvatarGroup` | control theme | `AvatarGroupTheme.axaml` | 用户代码 / 控件宿主 | 主题状态 / visual state | public | 用户可直接使用 public 控件；可作为示例和 API 入口。 |
| `Avatar` | control theme | `AvatarTheme.axaml` | 用户代码 / 控件宿主 | `Background`, `BitmapSrc`, `BorderBrush`, `BorderThickness`, `CornerRadius`, `EffectiveIconSize` | public | 用户可直接使用 public 控件；可作为示例和 API 入口。 |
| `RootLayout` | template node (Panel) | `AvatarTheme.axaml` | Avatar | `Background`, `BitmapSrc`, `BorderBrush`, `BorderThickness`, `CornerRadius`, `EffectiveIconSize` | template-stable | 用于主题维护；变更需同步主题、实现和 LLMS。 |
| `Frame` | template node (PixelAlignedBorder) | `AvatarTheme.axaml` | Avatar | `Background`, `BorderBrush`, `BorderThickness`, `CornerRadius` | template-stable | 用于主题维护；变更需同步主题、实现和 LLMS。 |
| `IconPresenter` | template node (IconPresenter) | `AvatarTheme.axaml` | Avatar | `EffectiveIconSize`, `Foreground`, `Icon` | internal-observable | 用于理解结构和状态流，不应指导用户代码直接依赖。 |
| `ImagePresenter` | template node (Image) | `AvatarTheme.axaml` | Avatar | `BitmapSrc`, `Height`, `Width` | template-stable | 用于主题维护；变更需同步主题、实现和 LLMS。 |
| `SvgPresenter` | template node (Svg) | `AvatarTheme.axaml` | Avatar | `Height`, `Src`, `Width` | template-stable | 用于主题维护；变更需同步主题、实现和 LLMS。 |
| `PART_TextPresenter` | template node (TextBlock) | `AvatarTheme.axaml` | Avatar | `Text`, `TextRenderTransform` | template-stable | 用于主题维护；变更需同步主题、实现和 LLMS。 |

## Template Parts

| 契约组 | 代表成员 | 维护含义 |
| --- | --- | --- |
| 内容与数据 | `BitmapSrc`、`FoldAvatarFlyoutTriggerType`、`FoldInfoAvatarBackground`、`FoldInfoAvatarForeground`、`Icon`、`Src`、`Text` | 定义控件展示内容、输入数据、模板或业务对象入口。 |
| 选择与集合 | `MaxDisplayCount` | 维护选择、展开、过滤、分页、分组或集合状态。 |
| 交互与状态 | `IsMotionEnabled` | 表达用户可观察状态、可用性、清除、加载或反馈语义。 |
| 视觉与布局 | `Gap`、`Shape`、`Size`、`SizeType` | 影响尺寸、位置、颜色、形状、密度和模板视觉变量。 |

## Pseudo Classes

| 状态反馈 | public API、内部状态和伪类如何形成用户可感知反馈。 | open/close、input/value、motion、visual option。 |
| 主题语义 | ControlTheme、SharedToken、组件 Token 和模板绑定如何表达视觉。 | Avatar Token + ControlTheme。 |

## State Flow

Avatar 的状态流按以下路径收敛：

```text
Public API / inherited command / item source / user input
  -> 控件实例状态
  -> effective state / pseudo-class / template property
  -> ControlTheme selector / presenter / renderer
  -> Gallery 可观察行为
```

状态维护规则：

- Disabled 或不可交互状态优先屏蔽 pointer、keyboard、motion 和提交类反馈。
- open/close、input/value、motion、visual option 状态由控件实例或明确的数据 owner 推导，不能在 template part 之间双向竞争。
- 模板重套用时必须把 public API 对应状态回放到新的 part、伪类和主题变量。
- 集合、弹层、异步、动效或窗口相关状态必须能处理 reset、close、cancel、detach 和 owner 释放。

## Theme and Token Boundaries

Avatar 的视觉模型由控件模板、ControlTheme、SharedToken 和必要的组件 Token 共同构成。

| 主题文件 | 职责 |
| --- | --- |
| `AvatarGroupTheme.axaml` | 提供控件模板、selector、资源绑定和状态视觉。 |
| `AvatarTheme.axaml` | 提供控件模板、selector、资源绑定和状态视觉。 |
| `AvatarThemes.axaml` | 聚合控件家族主题资源，保证包级引入顺序稳定。 |

Avatar 使用 `AvatarToken` 作为组件 Token scope。Token 只表达组件视觉语义，不承载 open/close、input/value、motion、visual option 运行时状态。

主题维护规则：

- 不删除或重命名已经稳定的 ControlTheme key、template part、伪类和资源 key。
- 不把可由 AXAML 表达的模板状态迁移为 C# 动态创建视觉。
- 不把 hover、pressed、selected、expanded、loading、filter、popup open 等运行时状态写入 Token。
- Browser 或平台特化主题必须保持同一 API 的语义一致。

Token 边界：

Avatar Token 只表达组件级视觉变量，例如尺寸、间距、颜色、圆角、阴影、图标尺寸和弹层边界。Token 不承载运行时选择、展开、加载、错误、上传任务、过滤条件或业务状态。

当前 Token scope：

- `AvatarToken`，scope id 为 `Avatar`，源码位于 `src/AtomUI.Desktop.Controls/Avatar/AvatarToken.cs`。

## Customization Boundaries

维护 Avatar 时必须保持以下不变量：

- 不擅自新增、删除、重命名或改变 public/protected API、Avalonia 属性、事件和默认值。
- 不破坏 template part、伪类、ControlTheme key、Token 名称和资源 key。
- 不改变 Gallery 已展示的 XAML 用法、默认外观、交互顺序和状态优先级。
- Template part 重新应用、集合替换、弹层关闭、窗口失活和控件 detach 时必须释放旧订阅和资源宿主。
- 不通过隐藏延迟、强制刷新或吞异常掩盖状态同步问题。
- 不引入运行时反射扫描作为 API、Token 或数据路径发现机制。
- 文档只描述当前稳定设计；历史变化记录在 `changelog.md`。

维护不变量：

维护 Avatar 时不得破坏：

- Public API、默认值、事件顺序和 Gallery 可观察行为。
- Template part 名称、ControlTheme key、伪类和资源 key。
- 旧 template part、事件订阅、Popup/Flyout/Window host 和 collection view 的释放路径。
- Light/Dark、Browser/Desktop 和不同 SizeType 下的主题一致性。
- 控件文档、源码 public surface、Token 类型或生成数据与源码契约的一致性。
