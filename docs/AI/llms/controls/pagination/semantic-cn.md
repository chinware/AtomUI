# Pagination 语义结构

> 生成产物：由源文档生成，不要手工编辑。修改内容请回到控件文档、Gallery API / Token 表、Gallery ShowCase 或源码结构。

## Semantic Parts

| Part | AtomUI 节点 | 职责 | 相关 API | 相关 Token | 稳定性 |
| --- | --- | --- | --- | --- | --- |
| `root` | `Pagination` | 导航控件根语义区域，承载 public API、状态归一和主题入口。 | 见 API 与契约模型 | 见视觉与主题模型 | stable |
| `trigger` | `触发区域` | 承载点击、键盘、打开关闭、跳转或提交入口。 | 见 API 与契约模型 | 见视觉与主题模型 | stable |
| `item` | `导航项区域` | 承载当前项、选中项、禁用项、层级项或分页项状态。 | 见 API 与契约模型 | 见视觉与主题模型 | stable |
| `popup` | `弹层或内容区域` | 承载 flyout、dropdown、tab content、submenu 或候选内容。 | 见 API 与契约模型 | 见视觉与主题模型 | stable |
| `motion` | `动效区域` | 表达打开关闭、选中指示、切换和过渡反馈。 | 见 API 与契约模型 | 见视觉与主题模型 | stable |

## Abstract AXAML Structure

来源：`src/AtomUI.Desktop.Controls/Pagination/Themes/PaginationTheme.axaml`

```xml
<StackPanel Name="PART_RootLayout">
    <ContentPresenter Name="PART_TotalInfoPresenter" />
    <PaginationNav Name="PART_Nav" />
    <ContentPresenter Name="PART_SizeChangerPresenter" />
    <ContentPresenter Name="PART_QuickJumperBarPresenter" />
</StackPanel>
```

## Composition Model

该章节由控件 `Themes/` 文件夹中的真实主题文件生成，用于说明 public 控件与内部协作对象之间的运行时结构。内部节点只用于理解和维护，不应指导用户代码直接依赖。

### 控件角色图

```text
Pagination
  -> PaginationNavItem (item container control theme, PaginationNavItemTheme.axaml)
     -> Panel (template-stable)
        -> PixelAlignedBorder (template-stable)
        -> IconPresenter#IconPresenter (internal-observable)
        -> ContentPresenter#ContentPresenter (internal-observable)
  -> PaginationNav (control theme, PaginationNavTheme.axaml)
     -> Border#PART_Frame (template-stable)
        -> ItemsPresenter (internal-observable)
  -> Pagination (control theme, PaginationTheme.axaml)
     -> StackPanel#PART_RootLayout (template-stable)
        -> ContentPresenter#PART_TotalInfoPresenter (template-stable)
        -> PaginationNav#PART_Nav (template-stable)
        -> ContentPresenter#PART_SizeChangerPresenter (template-stable)
        -> ContentPresenter#PART_QuickJumperBarPresenter (template-stable)
  -> QuickJumperBar (control theme, QuickJumperBarTheme.axaml)
     -> StackPanel#PART_RootLayout (template-stable)
        -> ContentPresenter#PART_JumpToContentPresenter (template-stable)
        -> LineEdit#PART_PageLineEdit (template-stable)
        -> ContentPresenter#PART_PageContentPresenter (template-stable)
```

### 协作节点

| 节点 | 类型 | 来源 | 生命周期 owner | 影响的 public API | 稳定性 | Agent 使用边界 |
| --- | --- | --- | --- | --- | --- | --- |
| `Pagination` | public control | `源文档 + public API` | 用户代码 / 控件宿主 | public API | public | 用户可直接使用 public 控件；可作为示例和 API 入口。 |
| `PaginationNavItem` | item container control theme | `PaginationNavItemTheme.axaml` | Pagination | `Background`, `BorderBrush`, `BorderThickness`, `Content`, `CornerRadius`, `Foreground` | internal-observable | 用于理解结构和状态流，不应指导用户代码直接依赖。 |
| `Panel` | template node (Panel) | `PaginationNavItemTheme.axaml` | PaginationNavItem | `Background`, `BorderBrush`, `BorderThickness`, `Content`, `CornerRadius`, `Icon` | template-stable | 用于主题维护；变更需同步主题、实现和 LLMS。 |
| `IconPresenter` | template node (IconPresenter) | `PaginationNavItemTheme.axaml` | PaginationNavItem | `Icon`, `IsEnabled` | internal-observable | 用于理解结构和状态流，不应指导用户代码直接依赖。 |
| `ContentPresenter` | template node (ContentPresenter) | `PaginationNavItemTheme.axaml` | PaginationNavItem | `Content` | internal-observable | 用于理解结构和状态流，不应指导用户代码直接依赖。 |
| `PaginationNav` | control theme | `PaginationNavTheme.axaml` | Pagination | `CornerRadius`, `Padding` | internal-observable | 用于理解结构和状态流，不应指导用户代码直接依赖。 |
| `PART_Frame` | template node (Border) | `PaginationNavTheme.axaml` | PaginationNav | `CornerRadius`, `Padding` | template-stable | 用于主题维护；变更需同步主题、实现和 LLMS。 |
| `ItemsPresenter` | template node (ItemsPresenter) | `PaginationNavTheme.axaml` | PaginationNav | 主题状态 / visual state | internal-observable | 用于理解结构和状态流，不应指导用户代码直接依赖。 |
| `Pagination` | control theme | `PaginationTheme.axaml` | 用户代码 / 控件宿主 | `IsEffectiveVisible`, `IsMotionEnabled`, `IsShowQuickJumper`, `IsShowSizeChanger`, `IsShowTotalInfo`, `QuickJumperBar` | public | 用户可直接使用 public 控件；可作为示例和 API 入口。 |
| `PART_RootLayout` | template node (StackPanel) | `PaginationTheme.axaml` | Pagination | `IsEffectiveVisible`, `IsMotionEnabled`, `IsShowQuickJumper`, `IsShowSizeChanger`, `IsShowTotalInfo`, `QuickJumperBar` | template-stable | 用于主题维护；变更需同步主题、实现和 LLMS。 |
| `PART_TotalInfoPresenter` | template node (ContentPresenter) | `PaginationTheme.axaml` | Pagination | `IsShowTotalInfo`, `TotalInfoText` | template-stable | 用于主题维护；变更需同步主题、实现和 LLMS。 |
| `PART_Nav` | template node (PaginationNav) | `PaginationTheme.axaml` | Pagination | `IsMotionEnabled`, `SizeType` | template-stable | 用于主题维护；变更需同步主题、实现和 LLMS。 |
| `PART_SizeChangerPresenter` | template node (ContentPresenter) | `PaginationTheme.axaml` | Pagination | `IsShowSizeChanger`, `SizeChanger` | template-stable | 用于主题维护；变更需同步主题、实现和 LLMS。 |
| `PART_QuickJumperBarPresenter` | template node (ContentPresenter) | `PaginationTheme.axaml` | Pagination | `IsShowQuickJumper`, `QuickJumperBar` | template-stable | 用于主题维护；变更需同步主题、实现和 LLMS。 |
| `QuickJumperBar` | control theme | `QuickJumperBarTheme.axaml` | Pagination | `JumpToText`, `PageText`, `SizeType` | internal-observable | 用于理解结构和状态流，不应指导用户代码直接依赖。 |
| `PART_RootLayout` | template node (StackPanel) | `QuickJumperBarTheme.axaml` | QuickJumperBar | `JumpToText`, `PageText`, `SizeType` | template-stable | 用于主题维护；变更需同步主题、实现和 LLMS。 |
| `PART_JumpToContentPresenter` | template node (ContentPresenter) | `QuickJumperBarTheme.axaml` | QuickJumperBar | `JumpToText` | template-stable | 用于主题维护；变更需同步主题、实现和 LLMS。 |
| `PART_PageLineEdit` | template node (LineEdit) | `QuickJumperBarTheme.axaml` | QuickJumperBar | `SizeType` | template-stable | 用于主题维护；变更需同步主题、实现和 LLMS。 |
| `PART_PageContentPresenter` | template node (ContentPresenter) | `QuickJumperBarTheme.axaml` | QuickJumperBar | `PageText` | template-stable | 用于主题维护；变更需同步主题、实现和 LLMS。 |

## Template Parts

| 契约组 | 代表成员 | 维护含义 |
| --- | --- | --- |
| 内容与数据 | `Icon`、`JumpToText`、`PageText`、`PaginationItemType`、`TotalInfoTemplate` | 定义控件展示内容、输入数据、模板或业务对象入口。 |
| 选择与集合 | `CurrentPage`、`IsHideOnSinglePage`、`IsSelected`、`PageCount`、`PageSize` | 维护选择、展开、过滤、分页、分组或集合状态；`CurrentPage` 和 `PageSize` 默认 `TwoWay`。 |
| 交互与状态 | `IsMotionEnabled`、`IsPressed`、`IsReadOnly`、`IsShowQuickJumper`、`IsShowSizeChanger`、`IsShowTotalInfo` | 表达用户可观察状态、可用性、清除、加载或反馈语义。 |
| 视觉与布局 | `Align`、`SizeType` | 影响尺寸、位置、颜色、形状、密度和模板视觉变量。 |
| 其他稳定入口 | `Maximum`、`Minimum`、`Total` | 保留为 public surface，变更前需确认 Gallery 和用户 XAML 依赖。 |

## Pseudo Classes

| 状态反馈 | public API、内部状态和伪类如何形成用户可感知反馈。 | selection/checked/active、collection/filter、input/value、motion、visual option。 |
| 主题语义 | ControlTheme、SharedToken、组件 Token 和模板绑定如何表达视觉。 | Pagination Token + ControlTheme。 |

## State Flow

Pagination 的状态流按以下路径收敛：

```text
Public API / inherited command / item source / user input
  -> 控件实例状态
  -> effective state / pseudo-class / template property
  -> ControlTheme selector / presenter / renderer
  -> Gallery 可观察行为
```

状态维护规则：

- Disabled 或不可交互状态优先屏蔽 pointer、keyboard、motion 和提交类反馈。
- 用户点击页码、快速跳转或切换页大小时，通过 `CurrentPage` / `PageSize` 写回同一个受控状态；绑定方不需要显式设置 `Mode=TwoWay`。
- selection/checked/active、collection/filter、input/value、motion、visual option 状态由控件实例或明确的数据 owner 推导，不能在 template part 之间双向竞争。
- 模板重套用时必须把 public API 对应状态回放到新的 part、伪类和主题变量。
- 集合、弹层、异步、动效或窗口相关状态必须能处理 reset、close、cancel、detach 和 owner 释放。

## Theme and Token Boundaries

Pagination 的视觉模型由控件模板、ControlTheme、SharedToken 和必要的组件 Token 共同构成。

| 主题文件 | 职责 |
| --- | --- |
| `PaginationNavItemTheme.axaml` | 定义集合项、容器项或局部单元的状态视觉。 |
| `PaginationNavTheme.axaml` | 提供控件模板、selector、资源绑定和状态视觉。 |
| `PaginationTheme.axaml` | 提供控件模板、selector、资源绑定和状态视觉。 |
| `PaginationThemes.axaml` | 聚合控件家族主题资源，保证包级引入顺序稳定。 |
| `QuickJumperBarTheme.axaml` | 提供控件模板、selector、资源绑定和状态视觉。 |
| `SimplePaginationTheme.axaml` | 提供控件模板、selector、资源绑定和状态视觉。 |

Pagination 使用 `PaginationToken` 作为组件 Token scope。Token 只表达组件视觉语义，不承载 selection/checked/active、collection/filter、input/value、motion、visual option 运行时状态。

主题维护规则：

- 不删除或重命名已经稳定的 ControlTheme key、template part、伪类和资源 key。
- 不把可由 AXAML 表达的模板状态迁移为 C# 动态创建视觉。
- 不把 hover、pressed、selected、expanded、loading、filter、popup open 等运行时状态写入 Token。
- Browser 或平台特化主题必须保持同一 API 的语义一致。

Token 边界：

Pagination Token 只表达组件级视觉变量，例如尺寸、间距、颜色、圆角、阴影、图标尺寸和弹层边界。Token 不承载运行时选择、展开、加载、错误、上传任务、过滤条件或业务状态。

当前 Token scope：

- `PaginationToken`，scope id 为 `Pagination`，源码位于 `src/AtomUI.Desktop.Controls/Pagination/PaginationToken.cs`。

## Customization Boundaries

维护 Pagination 时必须保持以下不变量：

- 不擅自新增、删除、重命名或改变 public/protected API、Avalonia 属性、事件和默认值。
- 不破坏 template part、伪类、ControlTheme key、Token 名称和资源 key。
- 不改变 Gallery 已展示的 XAML 用法、默认外观、交互顺序和状态优先级。
- Template part 重新应用、集合替换、弹层关闭、窗口失活和控件 detach 时必须释放旧订阅和资源宿主。
- 不通过隐藏延迟、强制刷新或吞异常掩盖状态同步问题。
- 不引入运行时反射扫描作为 API、Token 或数据路径发现机制。
- 文档只描述当前稳定设计；历史变化记录在 `changelog.md`。

维护不变量：

维护 Pagination 时不得破坏：

- Public API、默认值、事件顺序和 Gallery 可观察行为。
- `CurrentPage` / `PageSize` 的默认 `TwoWay` binding metadata，以及内部写入不破坏外部 binding 的 `SetCurrentValue` 路径。
- Template part 名称、ControlTheme key、伪类和资源 key。
- 旧 template part、事件订阅、Popup/Flyout/Window host 和 collection view 的释放路径。
- Light/Dark、Browser/Desktop 和不同 SizeType 下的主题一致性。
- 文档、Gallery API 表、Token 表与源码契约的一致性。
