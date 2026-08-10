# Skeleton

> 生成产物：由源文档生成，不要手工编辑。修改内容请回到控件文档、源码 public surface、Token 类型或生成数据、Gallery ShowCase 或源码结构。

## 概述

Skeleton 是 AtomUI 桌面控件体系中的骨架屏控件，用于在内容加载前展示结构占位。

Skeleton 不负责 Spin 加载器、真实数据列表或动画演示控件。这些职责应由业务层、组合控件或更专用的 AtomUI 控件承担。

主要源码入口：

- `src/AtomUI.Desktop.Controls/Skeleton`

## 包与命名空间

| 项 | 值 |
| --- | --- |
| NuGet 包 | `AtomUI.Desktop.Controls` |
| .NET 命名空间 | `AtomUI.Desktop.Controls` |
| AXAML 命名空间 | `https://atomui.net` |
| Gallery 页面 | `controlgallery/AtomUIGallery/ShowCases/Feedback/Skeleton` |
| 状态 | Stable |

## 何时使用

Skeleton 的设计语言围绕控件职责、可观察状态和主题契约组织，而不是围绕模板节点组织。

| 维度 | 含义 | Skeleton 中的表达 |
| --- | --- | --- |
| 产品语义 | 控件在界面中承担的稳定职责。 | Skeleton 是 AtomUI 桌面控件体系中的骨架屏控件，用于在内容加载前展示结构占位。 |
| 内容承载 | 用户数据、展示内容、集合项或操作入口如何进入控件。 | `AvatarShape`、`AvatarSize`、`AvatarSizeType`、`Content`、`ContentTemplate`、`HorizontalContentAlignment`、`IsShowAvatar`、`IsShowTitle` 等 12 项。 |
| 状态反馈 | public API、内部状态和伪类如何形成用户可感知反馈。 | selection/checked/active、loading/async、motion、visual option。 |
| 主题语义 | ControlTheme、SharedToken、控件 Token 和模板绑定如何表达视觉。 | Skeleton Token + ControlTheme。 |

## 公共 API

Skeleton 的公共契约由 public/protected 类型成员、Avalonia 属性、事件、命令、template part、伪类、ControlTheme key 和资源 key 共同组成。维护时应先确认这些契约是否已经被源码、Gallery 示例或文档暴露。

核心 public surface 按语义分组维护：

| 契约组 | 代表成员 | 维护含义 |
| --- | --- | --- |
| 内容与数据 | `AvatarShape`、`AvatarSize`、`AvatarSizeType`、`Content`、`ContentTemplate`、`HorizontalContentAlignment`、`IsShowAvatar`、`IsShowTitle`、`ParagraphRows`、`Rows` 等 12 项 | 定义控件展示内容、输入数据、模板或业务对象入口。 |
| 选择与集合 | `IsActive` | 维护选择、展开、过滤、分页、分组或集合状态。 |
| 交互与状态 | `IsBlock`、`IsLoading`、`IsRound`、`IsShowParagraph` | 表达用户可观察状态、可用性、清除、加载或反馈语义。 |
| 视觉与布局 | `LastLineWidth`、`LineWidth`、`ParagraphLastLineWidth`、`Shape`、`Size`、`SizeType` | 影响尺寸、位置、颜色、形状、密度和模板视觉变量。 |
| 动效与异步 | `MotionDuration`、`MotionEasingCurve` | 约束动效开关、异步加载、播放速度、超时和任务边界。 |

当前没有抽取到控件专属 public 事件；交互通知主要来自继承事件、命令或 Gallery 可观察状态。

主要公开类型与枚举：

- 类型：`AbstractSkeleton`、`Skeleton`、`SkeletonAvatar`、`SkeletonButton`、`SkeletonElement`、`SkeletonImage`、`SkeletonInput`、`SkeletonLine`、`SkeletonNode`、`SkeletonParagraph`、`SkeletonTitle`。
- 枚举：`SkeletonButtonShape`。

稳定 template part：

| Template Part | 类型 | 职责 |
| --- | --- | --- |
| `PART_ActiveAnimationLayer` | `?` | 稳定模板协作入口，重命名前必须同步主题和实现。 |
| `PART_Avatar` | `?` | 稳定模板协作入口，重命名前必须同步主题和实现。 |
| `PART_Content` | `?` | 稳定模板协作入口，重命名前必须同步主题和实现。 |
| `PART_ContentLayer` | `?` | 稳定模板协作入口，重命名前必须同步主题和实现。 |
| `PART_LineLayout` | `?` | 稳定模板协作入口，重命名前必须同步主题和实现。 |
| `PART_Paragraph` | `?` | 稳定模板协作入口，重命名前必须同步主题和实现。 |
| `PART_RootLayout` | `?` | 承载根视觉、边框、背景或尺寸基线。 |
| `PART_Title` | `?` | 稳定模板协作入口，重命名前必须同步主题和实现。 |

当前未抽取到控件专属伪类；主题主要依赖 Avalonia 标准伪类、模板绑定和内部 StyledProperty。

## 事件与命令

Skeleton 的公共契约由 public/protected 类型成员、Avalonia 属性、事件、命令、template part、伪类、ControlTheme key 和资源 key 共同组成。维护时应先确认这些契约是否已经被源码、Gallery 示例或文档暴露。
当前没有抽取到控件专属 public 事件；交互通知主要来自继承事件、命令或 Gallery 可观察状态。

## 使用示例

稳定示例来源于 Gallery ShowCase 和源码查看片段。生成器只输出可从 `ShowCaseItem` 追溯的示例，不维护第二套手写示例。

以下示例来自 Gallery 源码查看使用的 `ShowCaseItem` 片段，并已按中文资源规范化。

### {gallery:SkeletonShowCaseLangResource BasicTitle}

来源：`controlgallery/AtomUIGallery/ShowCases/Feedback/Skeleton/Views/SkeletonShowCase.axaml:37`

Gallery key：`ExamplesContent` / item `0`

```axaml
<atom:Skeleton IsLoading="True"/>
```

### {gallery:SkeletonShowCaseLangResource ComplexCombinationTitle}

来源：`controlgallery/AtomUIGallery/ShowCases/Feedback/Skeleton/Views/SkeletonShowCase.axaml:49`

Gallery key：`ExamplesContent` / item `1`

```axaml
<atom:Skeleton IsShowAvatar="True" ParagraphRows="4" IsLoading="True"/>
```

### {gallery:SkeletonShowCaseLangResource ActiveAnimationTitle}

来源：`controlgallery/AtomUIGallery/ShowCases/Feedback/Skeleton/Views/SkeletonShowCase.axaml:61`

Gallery key：`ExamplesContent` / item `2`

```axaml
<atom:Skeleton IsActive="True" IsLoading="True"/>
```

## 状态模型

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

## 主题与 Design Token

Skeleton 的视觉模型由控件模板、ControlTheme、SharedToken 和必要的控件 Token 共同构成。

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

Skeleton 使用 `SkeletonToken` 作为控件 Token scope。Token 只表达组件视觉语义，不承载 selection/checked/active、loading/async、motion、visual option 运行时状态。

主题维护规则：

- 不删除或重命名已经稳定的 ControlTheme key、template part、伪类和资源 key。
- 不把可由 AXAML 表达的模板状态迁移为 C# 动态创建视觉。
- 不把 hover、pressed、selected、expanded、loading、filter、popup open 等运行时状态写入 Token。
- Browser 或平台特化主题必须保持同一 API 的语义一致。

Token 来源：

Skeleton Token 只表达组件级视觉变量，例如尺寸、间距、颜色、圆角、阴影、图标尺寸和弹层边界。Token 不承载运行时选择、展开、加载、错误、上传任务、过滤条件或业务状态。

当前 Token scope：

- `SkeletonToken`，scope id 为 `Skeleton`，源码位于 `src/AtomUI.Desktop.Controls/Skeleton/SkeletonToken.cs`。

## AOT 与裁剪注意事项

资源和 AOT 约束：

- 不通过运行时反射扫描 public API、Token 或 Gallery 示例数据。
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

- `src/AtomUI.Desktop.Controls/Skeleton/AbstractSkeleton.cs`
- `src/AtomUI.Desktop.Controls/Skeleton/Skeleton.cs`
- `src/AtomUI.Desktop.Controls/Skeleton/SkeletonAvatar.cs`
- `src/AtomUI.Desktop.Controls/Skeleton/SkeletonButton.cs`
- `src/AtomUI.Desktop.Controls/Skeleton/SkeletonElement.cs`
- `src/AtomUI.Desktop.Controls/Skeleton/SkeletonImage.cs`
- `src/AtomUI.Desktop.Controls/Skeleton/SkeletonInput.cs`
- `src/AtomUI.Desktop.Controls/Skeleton/SkeletonLine.cs`
- `src/AtomUI.Desktop.Controls/Skeleton/SkeletonNode.cs`
- `src/AtomUI.Desktop.Controls/Skeleton/SkeletonParagraph.cs`
- `src/AtomUI.Desktop.Controls/Skeleton/SkeletonTitle.cs`
- `src/AtomUI.Desktop.Controls/Skeleton/SkeletonToken.cs`
- `src/AtomUI.Desktop.Controls/Skeleton/Themes/AbstractSkeletonTheme.axaml`
- `src/AtomUI.Desktop.Controls/Skeleton/Themes/AbstractSkeletonTheme.cs`
- `src/AtomUI.Desktop.Controls/Skeleton/Themes/SkeletonAvatarTheme.axaml`
- `src/AtomUI.Desktop.Controls/Skeleton/Themes/SkeletonButtonTheme.axaml`
- `src/AtomUI.Desktop.Controls/Skeleton/Themes/SkeletonElementTheme.axaml`
- `src/AtomUI.Desktop.Controls/Skeleton/Themes/SkeletonElementTheme.cs`
- `src/AtomUI.Desktop.Controls/Skeleton/Themes/SkeletonImageTheme.axaml`
- `src/AtomUI.Desktop.Controls/Skeleton/Themes/SkeletonImageTheme.cs`
- `src/AtomUI.Desktop.Controls/Skeleton/Themes/SkeletonInputTheme.axaml`
- `src/AtomUI.Desktop.Controls/Skeleton/Themes/SkeletonLineTheme.axaml`
- `src/AtomUI.Desktop.Controls/Skeleton/Themes/SkeletonNodeTheme.axaml`
- `src/AtomUI.Desktop.Controls/Skeleton/Themes/SkeletonParagraphTheme.axaml`
- `src/AtomUI.Desktop.Controls/Skeleton/Themes/SkeletonTheme.axaml`

职责边界：

- 控件主文件保留 public/protected API、Avalonia 属性注册、事件和主要生命周期入口。
- Theme 文件负责静态视觉结构、template part、selector 和资源绑定。
- Token 文件只提供组件视觉变量，不保存实例状态。
- Gallery 文件只展示用法和示例，不作为运行时逻辑 owner。

## 相关文档

- 源设计文档：`docs/controls/desktop/feedback/skeleton/overview.md`
- 实现文档：`docs/controls/desktop/feedback/skeleton/implementation.md`
- Token 文档：`docs/controls/desktop/feedback/skeleton/token.md`
- 变更记录：`docs/controls/desktop/feedback/skeleton/changelog.md`
- 语义结构：`./semantic-cn.md`
