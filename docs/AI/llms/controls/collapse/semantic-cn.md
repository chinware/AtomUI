# Collapse 语义结构

> 生成产物：由源文档生成，不要手工编辑。修改内容请回到控件文档、Gallery API / Token 表、Gallery ShowCase 或源码结构。

## Semantic Parts

| Part | AtomUI 节点 | 职责 | 相关 API | 相关 Token | 稳定性 |
| --- | --- | --- | --- | --- | --- |
| `root` | `Collapse` | 数据展示控件根语义区域，承载 public API、数据状态和主题入口。 | 见 API 与契约模型 | 见视觉与主题模型 | stable |
| `item` | `条目或容器区域` | 承载集合项、单元格、标签、时间节点、卡片或展示单元。 | 见 API 与契约模型 | 见视觉与主题模型 | stable |
| `header` | `标题或头部区域` | 承载标题、字段名、列头、操作入口或摘要信息。 | 见 API 与契约模型 | 见视觉与主题模型 | stable |
| `content` | `内容区域` | 承载主体内容、媒体、文本、空状态、加载状态或详情区域。 | 见 API 与契约模型 | 见视觉与主题模型 | stable |
| `motion` | `动效或浮层区域` | 表达展开收起、轮播、tooltip、tour、预览或虚拟化反馈。 | 见 API 与契约模型 | 见视觉与主题模型 | stable |

## Abstract AXAML Structure

来源：`src/AtomUI.Desktop.Controls/Collapse/Themes/CollapseTheme.axaml`

```xml
<PixelAlignedBorder Name="PART_Frame">
    <ItemsPresenter Name="PART_ItemsPresenter" />
</PixelAlignedBorder>
```

## Composition Model

该章节由控件 `Themes/` 文件夹中的真实主题文件生成，用于说明 public 控件与内部协作对象之间的运行时结构。内部节点只用于理解和维护，不应指导用户代码直接依赖。

### 控件角色图

```text
Collapse
  -> CollapseItem (item container control theme, CollapseItemTheme.axaml)
     -> PixelAlignedBorder (template-stable)
        -> DockPanel#PART_MainLayout (template-stable)
           -> PixelAlignedBorder#PART_HeaderDecorator (template-stable)
              -> Grid (template-stable)
                 -> IconButton#PART_ExpandButton (template-stable)
                 -> ContentPresenter#PART_HeaderPresenter (template-stable)
                 -> ContentPresenter#PART_AddOnContentPresenter (template-stable)
           -> LayoutAwareMotionActor#PART_ContentMotionActor (template-stable)
              -> PixelAlignedBorder#PART_ContentFrame (template-stable)
                 -> ContentPresenter#PART_ContentPresenter (template-stable)
  -> Collapse (control theme, CollapseTheme.axaml)
     -> PixelAlignedBorder#PART_Frame (template-stable)
        -> ItemsPresenter#PART_ItemsPresenter (template-stable)
```

### 协作节点

| 节点 | 类型 | 来源 | 生命周期 owner | 影响的 public API | 稳定性 | Agent 使用边界 |
| --- | --- | --- | --- | --- | --- | --- |
| `Collapse` | public control | `源文档 + public API` | 用户代码 / 控件宿主 | public API | public | 用户可直接使用 public 控件；可作为示例和 API 入口。 |
| `CollapseItem` | item container control theme | `CollapseItemTheme.axaml` | 用户代码 / 控件宿主 | `AddOnContent`, `AddOnContentTemplate`, `Content`, `ContentBorderThickness`, `ContentTemplate`, `EffectiveContentPadding` | public | 用户可直接使用 public 控件；可作为示例和 API 入口。 |
| `PART_MainLayout` | template node (DockPanel) | `CollapseItemTheme.axaml` | CollapseItem | `AddOnContent`, `AddOnContentTemplate`, `Content`, `ContentBorderThickness`, `ContentTemplate`, `EffectiveContentPadding` | template-stable | 用于主题维护；变更需同步主题、实现和 LLMS。 |
| `PART_HeaderDecorator` | template node (PixelAlignedBorder) | `CollapseItemTheme.axaml` | CollapseItem | `AddOnContent`, `AddOnContentTemplate`, `EffectiveHeaderPadding`, `ExpandIcon`, `Header`, `HeaderTemplate` | template-stable | 用于主题维护；变更需同步主题、实现和 LLMS。 |
| `PART_ExpandButton` | template node (IconButton) | `CollapseItemTheme.axaml` | CollapseItem | `ExpandIcon`, `IsEnabled`, `IsMotionEnabled`, `IsShowExpandIcon` | template-stable | 用于主题维护；变更需同步主题、实现和 LLMS。 |
| `PART_HeaderPresenter` | template node (ContentPresenter) | `CollapseItemTheme.axaml` | CollapseItem | `Header`, `HeaderTemplate` | template-stable | 用于主题维护；变更需同步主题、实现和 LLMS。 |
| `PART_AddOnContentPresenter` | template node (ContentPresenter) | `CollapseItemTheme.axaml` | CollapseItem | `AddOnContent`, `AddOnContentTemplate` | template-stable | 用于主题维护；变更需同步主题、实现和 LLMS。 |
| `PART_ContentMotionActor` | template node (LayoutAwareMotionActor) | `CollapseItemTheme.axaml` | CollapseItem | `Content`, `ContentBorderThickness`, `ContentTemplate`, `EffectiveContentPadding` | template-stable | 用于主题维护；变更需同步主题、实现和 LLMS。 |
| `PART_ContentFrame` | template node (PixelAlignedBorder) | `CollapseItemTheme.axaml` | CollapseItem | `Content`, `ContentBorderThickness`, `ContentTemplate`, `EffectiveContentPadding` | template-stable | 用于主题维护；变更需同步主题、实现和 LLMS。 |
| `PART_ContentPresenter` | template node (ContentPresenter) | `CollapseItemTheme.axaml` | CollapseItem | `Content`, `ContentTemplate` | template-stable | 用于主题维护；变更需同步主题、实现和 LLMS。 |
| `Collapse` | control theme | `CollapseTheme.axaml` | 用户代码 / 控件宿主 | `BorderBrush`, `CornerRadius`, `EffectiveBorderThickness`, `ItemsPanel` | public | 用户可直接使用 public 控件；可作为示例和 API 入口。 |
| `PART_Frame` | template node (PixelAlignedBorder) | `CollapseTheme.axaml` | Collapse | `BorderBrush`, `CornerRadius`, `EffectiveBorderThickness`, `ItemsPanel` | template-stable | 用于主题维护；变更需同步主题、实现和 LLMS。 |
| `PART_ItemsPresenter` | template node (ItemsPresenter) | `CollapseTheme.axaml` | Collapse | `ItemsPanel` | template-stable | 用于主题维护；变更需同步主题、实现和 LLMS。 |

## Template Parts

| 契约组 | 代表成员 | 维护含义 |
| --- | --- | --- |
| 内容与数据 | `AddOnContent`、`AddOnContentTemplate`、`ContentPadding`、`ExpandIcon`、`ExpandIconPosition`、`HeaderPadding`、`IsShowExpandIcon`、`ItemContentPadding`、`ItemHeaderPadding` | 定义控件展示内容、输入数据、模板或业务对象入口。 |
| 选择与集合 | `IsSelected` | 维护选择、展开、过滤、分页、分组或集合状态。 |
| 交互与状态 | `IsAccordion`、`IsBorderless`、`IsGhostStyle`、`IsMotionEnabled` | 表达用户可观察状态、可用性、清除、加载或反馈语义。 |
| 视觉与布局 | `SizeType` | 影响尺寸、位置、颜色、形状、密度和模板视觉变量。 |
| 其他稳定入口 | `TriggerType` | 保留为 public surface，变更前需确认 Gallery 和用户 XAML 依赖。 |

## Pseudo Classes

| 状态反馈 | public API、内部状态和伪类如何形成用户可感知反馈。 | selection/checked/active、motion、visual option。 |
| 主题语义 | ControlTheme、SharedToken、组件 Token 和模板绑定如何表达视觉。 | Collapse Token + ControlTheme。 |

## State Flow

Collapse 的状态流按以下路径收敛：

```text
Public API / inherited command / item source / user input
  -> 控件实例状态
  -> effective state / pseudo-class / template property
  -> ControlTheme selector / presenter / renderer
  -> Gallery 可观察行为
```

`Collapse` 继续使用 Avalonia `SelectingItemsControl` 的 selection model 作为唯一展开状态 owner，`CollapseItem.IsSelected` 是该状态投影到容器后的公开绑定入口。控件不得维护 active-key 集合、当前展开项缓存或另一套展开状态。

状态维护规则：

- 普通模式使用 `Multiple | Toggle`：每个 item 可独立展开和收起。
- 手风琴模式使用 `Single | Toggle`：打开目标项时关闭旧项，点击当前项时允许全部收起。
- 普通模式切换到手风琴模式时，按视觉索引保留第一个已展开项，与 Ant Design `activeKey[0]` 语义一致。
- Header、Icon、keyboard 和 pointer 输入最终进入同一个 selection 操作，不在输入处理器中直接维护展开状态。
- Disabled 或不可交互状态优先屏蔽 pointer、keyboard 和 motion，不改变 selection。
- 内容可见性、箭头方向和动效目标只从 `IsSelected` 派生；模板节点之间不得双向同步展开状态。
- 模板重套用、items reset/replace/clear 和模式切换后必须保持 selection model、容器与内容视觉一致。

## Theme and Token Boundaries

Collapse 的视觉模型由控件模板、ControlTheme、SharedToken 和必要的组件 Token 共同构成。

| 主题文件 | 职责 |
| --- | --- |
| `CollapseItemTheme.axaml` | 定义集合项、容器项或局部单元的状态视觉。 |
| `CollapseTheme.axaml` | 提供控件模板、selector、资源绑定和状态视觉。 |
| `CollapseThemes.axaml` | 聚合控件家族主题资源，保证包级引入顺序稳定。 |

Collapse 使用 `CollapseToken` 作为组件 Token scope。Token 只表达组件视觉语义，不承载 selection/checked/active、motion、visual option 运行时状态。

Collapse 的分隔线采用结构化所有权：

- `PART_Frame` 绘制外框、圆角并裁剪整体内容。
- 非末 `CollapseItem` 的 item shell 固定绘制底部分隔线。
- 默认 bordered 模式下，`PART_ContentFrame` 固定绘制内容顶部边线。
- Borderless 模式保留 item 间分隔线，但不绘制外框和内容顶部边线。
- Ghost 模式不绘制外框、item 分隔线和内容顶部边线。
- 分隔线厚度不得依赖 `IsSelected`、动效进行状态或动效完成时机。

主题维护规则：

- 不删除或重命名已经稳定的 ControlTheme key、template part、伪类和资源 key。
- 不把可由 AXAML 表达的模板状态迁移为 C# 动态创建视觉。
- 不把 hover、pressed、selected、expanded、loading、filter、popup open 等运行时状态写入 Token。
- Browser 或平台特化主题必须保持同一 API 的语义一致。

Token 边界：

Collapse Token 只表达组件级视觉变量，例如尺寸、间距、颜色、圆角、阴影、图标尺寸和弹层边界。Token 不承载运行时选择、展开、加载、错误、上传任务、过滤条件或业务状态。

当前 Token scope：

- `CollapseToken`，scope id 为 `Collapse`，源码位于 `src/AtomUI.Desktop.Controls/Collapse/CollapseToken.cs`。

## Customization Boundaries

维护 Collapse 时必须保持以下不变量：

- 不擅自新增、删除、重命名或改变 public/protected API、Avalonia 属性、事件和默认值。
- 不破坏 template part、伪类、ControlTheme key、Token 名称和资源 key。
- 不改变 Gallery 已展示的 XAML 用法、默认外观、交互顺序和状态优先级。
- Template part 重新应用、集合替换、弹层关闭、窗口失活和控件 detach 时必须释放旧订阅和资源宿主。
- 不通过隐藏延迟、强制刷新或吞异常掩盖状态同步问题。
- 不引入运行时反射扫描作为 API、Token 或数据路径发现机制。
- 文档只描述当前稳定设计；历史变化记录在 `changelog.md`。

维护不变量：

维护 Collapse 时不得破坏：

- Public API、默认值、事件顺序和 Gallery 可观察行为。
- Template part 名称、ControlTheme key、伪类和资源 key。
- Selection model 是唯一展开状态 owner，不能增加 active-key 镜像或 `SelectionChanged` 回写循环。
- 手风琴模式最多展开一项，并允许点击当前项后全部收起。
- 分隔线只由 item 位置、视觉模式和固定模板结构决定，不能依赖 selection 或 motion 时序。
- 旧 template part、事件订阅和 content motion cancellation 的释放路径。
- Light/Dark、Browser/Desktop 和不同 SizeType 下的主题一致性。
- 文档、Gallery API 表、Token 表与源码契约的一致性。
