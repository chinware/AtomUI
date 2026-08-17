# Collapse 语义结构

> 生成产物：由源文档生成，不要手工编辑。修改内容请回到控件文档、源码 public surface、Token 类型或生成数据、Gallery ShowCase 或源码结构。

## Semantic Parts

`Collapse` 主控件公开 `root`、`header`、`icon`、`title` 与 `body` 五个职责区域，与上游稳定 Semantic DOM 对齐。上游基线为
6.6.0 稳定发布的 `CollapseSemanticType` 与 Semantic DOM 演示：

- `header`、`body` 自上游 5.21.0 公开；
- `root`、`icon`、`title` 自上游 6.0.0 公开。

上游 Semantic DOM 以 `itemsAPI="items"` 组织：`header`、`icon`、`title`、`body` 按 item 出现（每个面板各一个），面板容器
本身（`.ant-collapse-item`）不是 Semantic key。AtomUI 五个 Part 随本次 Semantic Part 改造同时公开，descriptor 的 `Since`
统一为 `6.0`。

`CollapseItem` 不持有独立 Semantic descriptor：

- 上游 `Collapse` 只提供一个 owner 的 Semantic DOM；`Collapse.Panel` 没有独立公开 Semantic DOM Props。
- `CollapseItem` 是 Collapse 的公开子控件与运行时容器，其职责通过 `Collapse` 的 `header`、`icon`、`title`、`body` Part
  对外公开；容器本身与 item shell 边框不属于任何 Part。

### 1.1 `Collapse`

#### `root`

| 字段 | 值 |
| --- | --- |
| Owner | `Collapse` |
| Part | `root` |
| Selector | Collapse 本身 |
| SelectorRoute | 不适用 |
| Style Type | 不适用（root 不生成 Style） |
| ContractType | `Collapse` |
| Cardinality | `Single` |
| Customization | `Root` |
| CrossVisualRoot | `false` |
| RuntimeCreated | `false` |
| AtomUI 节点 | Collapse owner（表面投影到 `PART_Frame`） |
| 职责 | Collapse root 是面板集合状态、视觉模式与根表面样式（背景、边框、圆角、内边距）的统一 owner。 |
| 相关 API | `IsAccordion`、`IsBorderless`、`IsGhostStyle`、`TriggerType`、`ExpandIconPosition`、`SizeType`、`IsMotionEnabled`、`ItemHeaderPadding`、`ItemContentPadding`、`Items`、`SelectedItems` |
| 相关 Token | CollapseToken、SharedToken |
| 稳定性 | stable since 6.0 |

#### `header`

| 字段 | 值 |
| --- | --- |
| Owner | `Collapse` |
| Part | `header` |
| Selector | `.semantic-header` |
| SelectorRoute | `> .semantic-scope-item /template/ .semantic-header` |
| Style Type | `CollapseHeaderStyle` |
| ContractType | `PixelAlignedBorder` |
| Cardinality | `Multiple` |
| Customization | `Selector` |
| CrossVisualRoot | `false` |
| RuntimeCreated | `true` |
| AtomUI 节点 | 每个 `CollapseItem` 模板中的 `PixelAlignedBorder#PART_HeaderDecorator` |
| 职责 | 统一表示每个面板头部的背景、内边距、字体/行高、光标与交互视觉；对应上游 `.ant-collapse-header` 的 flex 布局、内边距、颜色、行高、光标与过渡动画职责。 |
| 相关 API | `SizeType`、`ItemHeaderPadding`、`TriggerType`、`IsGhostStyle`、`IsEnabled` |
| 相关 Token | `HeaderBg`、`HeaderPadding`、`CollapseHeaderPaddingSM`、`CollapseHeaderPaddingLG`、SharedToken |
| 稳定性 | stable since 6.0 |

#### `icon`

| 字段 | 值 |
| --- | --- |
| Owner | `Collapse` |
| Part | `icon` |
| Selector | `.semantic-icon` |
| SelectorRoute | `> .semantic-scope-item /template/ .semantic-icon` |
| Style Type | `CollapseIconStyle` |
| ContractType | `IconButton` |
| Cardinality | `Multiple` |
| Customization | `Selector` |
| CrossVisualRoot | `false` |
| RuntimeCreated | `true` |
| AtomUI 节点 | 每个 `CollapseItem` 模板中的 `IconButton#PART_ExpandButton` |
| 职责 | 统一表示展开/收起箭头的大小、对齐、边距与动效视觉；对应上游 `.ant-collapse-expand-icon` 的字体大小、过渡动画与旋转变换职责。 |
| 相关 API | `ExpandIcon`、`ExpandIconPosition`、`IsShowExpandIcon`、`IsSelected` |
| 相关 Token | `IconSizeSM`、`LeftExpandButtonMargin*`、`RightExpandButtonMargin*`、SharedToken |
| 稳定性 | stable since 6.0 |

#### `title`

| 字段 | 值 |
| --- | --- |
| Owner | `Collapse` |
| Part | `title` |
| Selector | `.semantic-title` |
| SelectorRoute | `> .semantic-scope-item /template/ .semantic-title` |
| Style Type | `CollapseTitleStyle` |
| ContractType | `ContentPresenter` |
| Cardinality | `Multiple` |
| Customization | `Selector` |
| CrossVisualRoot | `false` |
| RuntimeCreated | `true` |
| AtomUI 节点 | 每个 `CollapseItem` 模板中的 `ContentPresenter#PART_HeaderPresenter` |
| 职责 | 统一表示每个面板标题文字的布局、颜色、字体与对齐；对应上游 `.ant-collapse-title` 的 flex 自适应布局与边距职责。 |
| 相关 API | `Header`、`HeaderTemplate` |
| 相关 Token | `ColorTextHeading`、`ColorTextDisabled`、SharedToken |
| 稳定性 | stable since 6.0 |

#### `body`

| 字段 | 值 |
| --- | --- |
| Owner | `Collapse` |
| Part | `body` |
| Selector | `.semantic-body` |
| SelectorRoute | `> .semantic-scope-item /template/ .semantic-body` |
| Style Type | `CollapseBodyStyle` |
| ContractType | `PixelAlignedBorder` |
| Cardinality | `Multiple` |
| Customization | `Selector` |
| CrossVisualRoot | `false` |
| RuntimeCreated | `true` |
| AtomUI 节点 | 每个 `CollapseItem` 模板中的 `PixelAlignedBorder#PART_ContentFrame` |
| 职责 | 统一表示每个面板内容区域的内边距、颜色、背景与内容顶部分隔线；对应上游 `.ant-collapse-body` 的内边距、颜色与背景职责。 |
| 相关 API | `Content`、`ContentTemplate`、`ItemContentPadding`、`IsBorderless`、`IsGhostStyle` |
| 相关 Token | `ContentPadding`、`ContentBg`、`HeaderBg`、SharedToken |
| 稳定性 | stable since 6.0 |

`root` 是隐式 Part，不添加 `.semantic-root`。`ContractType` 只定义 Setter 可以稳定依赖的最低 public 类型，并通过
`x:SetterTargetType` 提供 AXAML 编译期类型上下文；它不参与 `.semantic-*` 的身份匹配。`header` 与 `body` 的承载节点是
公开的 `PixelAlignedBorder`，`icon` 是公开的 `IconButton`，`title` 是公开的 `ContentPresenter`，均取节点真实 public 类型
作为最低依赖类型。

`header`、`icon`、`title`、`body` 的节点位于 `CollapseItem` 自己的模板内，而 `CollapseItem` 容器由 `Collapse` 的
ItemsControl 生命周期运行时创建（`TemplatedParent` 为 null），因此这四个 Part 声明 `RuntimeCreated=true` 并显式携带
SelectorRoute。Avalonia 的 `>` 步骤沿逻辑树（`LogicalParent`）行走，而 ItemsControl 生成的容器逻辑父级是 Collapse
owner 本身（并非运行时 ItemsPanel），所以路由从 owner 出发经一步 `>` 直达 `.semantic-scope-item` 容器，再以
`/template/` 进入容器模板到达 Part 节点。三个 scope marker（`.semantic-scope-items` / `.semantic-scope-panel` /
`.semantic-scope-item`）中只有 `.semantic-scope-item` 参与路由，前两者标识 items host 链、不单独发布为 Part，详见
[§3 Selector 用法](#3-selector-用法)。

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
| `CollapseItem` | item container control theme | `CollapseItemTheme.axaml` | 用户代码 / 控件宿主 | `AddOnContent`, `AddOnContentTemplate`, `Content`, `ContentBorderThickness`, `ContentCornerRadius`, `ContentTemplate` | public | 用户可直接使用 public 控件；可作为示例和 API 入口。 |
| `PART_MainLayout` | template node (DockPanel) | `CollapseItemTheme.axaml` | CollapseItem | `AddOnContent`, `AddOnContentTemplate`, `Content`, `ContentBorderThickness`, `ContentCornerRadius`, `ContentTemplate` | template-stable | 用于主题维护；变更需同步主题、实现和 LLMS。 |
| `PART_HeaderDecorator` | template node (PixelAlignedBorder) | `CollapseItemTheme.axaml` | CollapseItem | `AddOnContent`, `AddOnContentTemplate`, `EffectiveHeaderPadding`, `ExpandIcon`, `Header`, `HeaderCornerRadius` | template-stable | 用于主题维护；变更需同步主题、实现和 LLMS。 |
| `PART_ExpandButton` | template node (IconButton) | `CollapseItemTheme.axaml` | CollapseItem | `ExpandIcon`, `IsEnabled`, `IsMotionEnabled`, `IsShowExpandIcon` | template-stable | 用于主题维护；变更需同步主题、实现和 LLMS。 |
| `PART_HeaderPresenter` | template node (ContentPresenter) | `CollapseItemTheme.axaml` | CollapseItem | `Header`, `HeaderTemplate` | template-stable | 用于主题维护；变更需同步主题、实现和 LLMS。 |
| `PART_AddOnContentPresenter` | template node (ContentPresenter) | `CollapseItemTheme.axaml` | CollapseItem | `AddOnContent`, `AddOnContentTemplate` | template-stable | 用于主题维护；变更需同步主题、实现和 LLMS。 |
| `PART_ContentMotionActor` | template node (LayoutAwareMotionActor) | `CollapseItemTheme.axaml` | CollapseItem | `Content`, `ContentBorderThickness`, `ContentCornerRadius`, `ContentTemplate`, `EffectiveContentPadding` | template-stable | 用于主题维护；变更需同步主题、实现和 LLMS。 |
| `PART_ContentFrame` | template node (PixelAlignedBorder) | `CollapseItemTheme.axaml` | CollapseItem | `Content`, `ContentBorderThickness`, `ContentCornerRadius`, `ContentTemplate`, `EffectiveContentPadding` | template-stable | 用于主题维护；变更需同步主题、实现和 LLMS。 |
| `PART_ContentPresenter` | template node (ContentPresenter) | `CollapseItemTheme.axaml` | CollapseItem | `Content`, `ContentTemplate` | template-stable | 用于主题维护；变更需同步主题、实现和 LLMS。 |
| `Collapse` | control theme | `CollapseTheme.axaml` | 用户代码 / 控件宿主 | `Background`, `BorderBrush`, `CornerRadius`, `EffectiveBorderThickness`, `ItemsPanel`, `Padding` | public | 用户可直接使用 public 控件；可作为示例和 API 入口。 |
| `PART_Frame` | template node (PixelAlignedBorder) | `CollapseTheme.axaml` | Collapse | `Background`, `BorderBrush`, `CornerRadius`, `EffectiveBorderThickness`, `ItemsPanel`, `Padding` | template-stable | 用于主题维护；变更需同步主题、实现和 LLMS。 |
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
| 主题语义 | ControlTheme、SharedToken、控件 Token 和模板绑定如何表达视觉。 | Collapse Token + ControlTheme。 |

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
- 普通模式切换到手风琴模式时，按视觉索引保留第一个已展开项，保证切换后的单一展开状态确定且稳定。
- Header、Icon、keyboard 和 pointer 输入最终进入同一个 selection 操作，不在输入处理器中直接维护展开状态。
- Disabled 或不可交互状态优先屏蔽 pointer、keyboard 和 motion，不改变 selection。
- 内容可见性、箭头方向和动效目标只从 `IsSelected` 派生；模板节点之间不得双向同步展开状态。
- 模板重套用、items reset/replace/clear 和模式切换后必须保持 selection model、容器与内容视觉一致。

普通模式与手风琴模式使用 Core 共用内容展开机制：
内容按正常尺寸排版，通过高度和透明度呈现收放，反转从当前帧接续。手风琴在 selection 提交时同步产生旧项收起和
新项展开目标，两项使用同一进度交换空间，互斥不依赖动画完成事件。关闭动效及模板生命周期边界直接投影当前状态，
释放仅限该机制拥有的动画和内部布局控制，保留自定义尺寸与变换。
共享设计来源：`docs/architecture/systems/control-infrastructure/content-expansion.md`。

## Theme and Token Boundaries

Collapse 的视觉模型由控件模板、ControlTheme、SharedToken 和必要的控件 Token 共同构成。

| 主题文件 | 职责 |
| --- | --- |
| `CollapseItemTheme.axaml` | 定义集合项、容器项或局部单元的状态视觉。 |
| `CollapseTheme.axaml` | 提供控件模板、selector、资源绑定和状态视觉。 |

Collapse 使用 `CollapseToken` 作为控件 Token scope。Token 只表达组件视觉语义，不承载 selection/checked/active、motion、visual option 运行时状态。

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
- Semantic Part 的五个区域（`root`、`header`、`icon`、`title`、`body`）、selector class、ContractType、cardinality 与
  marker 放置属于主题兼容契约；删除、重命名、收窄类型或让内置模板缺少 marker 都是破坏性变更。
- `header`/`icon`/`title`/`body` 的 marker 静态声明于 `CollapseItemTheme.axaml`，scope marker 在默认 ItemsPanel 与容器
  创建路径一次性建立；任何状态切换、容器回收、items 集合变化与模板重应用都不得增删 marker；默认主题不得消费
  `.semantic-*` selector。
- root 表面投影（`Background`/`BorderBrush`/`BorderThickness`/`CornerRadius`/`Padding` → `PART_Frame`）属于公共契约；
  运行时 marker 通过静态 AXAML class 与既有创建路径添加，不引入 VisualTree 搜索、反射或运行时 AXAML 解析，保持
  NativeAOT 友好。

维护不变量：

维护 Collapse 时不得破坏：

- Public API、默认值、事件顺序和 Gallery 可观察行为。
- Template part 名称、ControlTheme key、伪类和资源 key。
- Selection model 是唯一展开状态 owner，不能增加 active-key 镜像或 `SelectionChanged` 回写循环。
- 手风琴模式最多展开一项，并允许点击当前项后全部收起。
- 分隔线只由 item 位置、视觉模式和固定模板结构决定，不能依赖 selection 或 motion 时序。
- 旧 template part、事件订阅和 content motion cancellation 的释放路径。
- Light/Dark、Browser/Desktop 和不同 SizeType 下的主题一致性。
- Semantic Part 的 marker 放置（`CollapseTheme.axaml` 的 `semantic-scope-items` 静态节点、默认 ItemsPanel 的
  `.semantic-scope-panel`、容器创建/prepare 路径的 `.semantic-scope-item`、`CollapseItemTheme.axaml` 的四个静态 Part
  marker）属于维护不变量：状态切换、容器复用/回收、items
  集合变化与模板重应用不得增删 marker，默认主题不得消费 `.semantic-*` selector，root 表面投影不得丢失。
- 控件文档、源码 public surface、Token 类型或生成数据与源码契约的一致性。
