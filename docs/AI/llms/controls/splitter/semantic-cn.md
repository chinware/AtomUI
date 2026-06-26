# Splitter 语义结构

> 生成产物：由源文档生成，不要手工编辑。修改内容请回到控件文档、Gallery API / Token 表、Gallery ShowCase 或源码结构。

## Semantic Parts

| Part | AtomUI 节点 | 职责 | 相关 API | 相关 Token | 稳定性 |
| --- | --- | --- | --- | --- | --- |
| `root` | `Splitter` | 布局控件根语义区域，承载布局 public API、尺寸和主题入口。 | 见 API 与契约模型 | 见视觉与主题模型 | stable |
| `container` | `布局容器` | 组织子元素、间距、断点、对齐或分割状态。 | 见 API 与契约模型 | 见视觉与主题模型 | stable |
| `item` | `布局项` | 承载子内容、占位、跨度、排序或尺寸约束。 | 见 API 与契约模型 | 见视觉与主题模型 | stable |
| `theme` | `主题区域` | 连接 SharedToken、布局主题资源和 Gallery 可观察样式。 | 见 API 与契约模型 | 见视觉与主题模型 | stable |

## Abstract AXAML Structure

来源：`src/AtomUI.Desktop.Controls/Splitter/Themes/SplitterTheme.axaml`

```xml
<Border Name="Frame">
    <SplitterPanel Name="PART_SplitterPanel" />
</Border>
```

## Composition Model

该章节由控件 `Themes/` 文件夹中的真实主题文件生成，用于说明 public 控件与内部协作对象之间的运行时结构。内部节点只用于理解和维护，不应指导用户代码直接依赖。

### 控件角色图

```text
Splitter
  -> SplitterDragBar (control theme, SplitterDragBarTheme.axaml)
     -> Border (template-stable)
        -> Border#PART_Grip (template-stable)
  -> SplitterHandle (control theme, SplitterHandleTheme.axaml)
     -> Border (template-stable)
        -> Grid (template-stable)
           -> Border#PART_HandleLine (template-stable)
           -> SplitterDragBar#PART_DragBar (template-stable)
           -> Canvas#PART_CollapseIconsHost (template-stable)
              -> IconButton#PART_CollapsePrevButton (template-stable)
              -> IconButton#PART_CollapseNextButton (template-stable)
  -> Splitter (control theme, SplitterTheme.axaml)
     -> Border#Frame (template-stable)
        -> SplitterPanel#PART_SplitterPanel (template-stable)
```

### 协作节点

| 节点 | 类型 | 来源 | 生命周期 owner | 影响的 public API | 稳定性 | Agent 使用边界 |
| --- | --- | --- | --- | --- | --- | --- |
| `Splitter` | public control | `源文档 + public API` | 用户代码 / 控件宿主 | public API | public | 用户可直接使用 public 控件；可作为示例和 API 入口。 |
| `SplitterDragBar` | control theme | `SplitterDragBarTheme.axaml` | Splitter | `Background`, `LineBrush` | internal-observable | 用于理解结构和状态流，不应指导用户代码直接依赖。 |
| `PART_Grip` | template node (Border) | `SplitterDragBarTheme.axaml` | SplitterDragBar | `LineBrush` | template-stable | 用于主题维护；变更需同步主题、实现和 LLMS。 |
| `SplitterHandle` | control theme | `SplitterHandleTheme.axaml` | Splitter | `IsDragEnabled`, `LineBrush`, `LineThickness`, `Orientation` | internal-observable | 用于理解结构和状态流，不应指导用户代码直接依赖。 |
| `PART_HandleLine` | template node (Border) | `SplitterHandleTheme.axaml` | SplitterHandle | `LineBrush` | template-stable | 用于主题维护；变更需同步主题、实现和 LLMS。 |
| `PART_DragBar` | template node (SplitterDragBar) | `SplitterHandleTheme.axaml` | SplitterHandle | `IsDragEnabled`, `LineThickness`, `Orientation` | template-stable | 用于主题维护；变更需同步主题、实现和 LLMS。 |
| `PART_CollapseIconsHost` | template node (Canvas) | `SplitterHandleTheme.axaml` | SplitterHandle | 主题状态 / visual state | template-stable | 用于主题维护；变更需同步主题、实现和 LLMS。 |
| `PART_CollapsePrevButton` | template node (IconButton) | `SplitterHandleTheme.axaml` | SplitterHandle | 主题状态 / visual state | template-stable | 用于主题维护；变更需同步主题、实现和 LLMS。 |
| `PART_CollapseNextButton` | template node (IconButton) | `SplitterHandleTheme.axaml` | SplitterHandle | 主题状态 / visual state | template-stable | 用于主题维护；变更需同步主题、实现和 LLMS。 |
| `Splitter` | control theme | `SplitterTheme.axaml` | 用户代码 / 控件宿主 | `CollapseNextIcon`, `CollapsePreviousIcon`, `HandleSize`, `IsLazy`, `Orientation` | public | 用户可直接使用 public 控件；可作为示例和 API 入口。 |
| `Frame` | template node (Border) | `SplitterTheme.axaml` | Splitter | `CollapseNextIcon`, `CollapsePreviousIcon`, `HandleSize`, `IsLazy`, `Orientation` | template-stable | 用于主题维护；变更需同步主题、实现和 LLMS。 |
| `PART_SplitterPanel` | template node (SplitterPanel) | `SplitterTheme.axaml` | Splitter | `CollapseNextIcon`, `CollapsePreviousIcon`, `HandleSize`, `IsLazy`, `Orientation` | template-stable | 用于主题维护；变更需同步主题、实现和 LLMS。 |

## Template Parts

| 契约组 | 代表成员 | 维护含义 |
| --- | --- | --- |
| 内容与数据 | `CollapseNextIcon`、`CollapsePreviousIcon` | 定义控件展示内容、输入数据、模板或业务对象入口。 |
| 交互与状态 | `IsDragEnabled`、`IsLazy` | 表达用户可观察状态、可用性、清除、加载或反馈语义。 |
| 视觉与布局 | `HandleSize`、`LineBrush`、`LineThickness`、`Orientation` | 影响尺寸、位置、颜色、形状、密度和模板视觉变量。 |

## Pseudo Classes

| 状态反馈 | public API、内部状态和伪类如何形成用户可感知反馈。 | visual option。 |
| 主题语义 | ControlTheme、SharedToken、组件 Token 和模板绑定如何表达视觉。 | Splitter Token + ControlTheme。 |

## State Flow

Splitter 的状态流按以下路径收敛：

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

## Theme and Token Boundaries

Splitter 的视觉模型由控件模板、ControlTheme、SharedToken 和必要的组件 Token 共同构成。

| 主题文件 | 职责 |
| --- | --- |
| `SplitterDragBarTheme.axaml` | 提供控件模板、selector、资源绑定和状态视觉。 |
| `SplitterHandleTheme.axaml` | 定义局部操作入口、按钮或 handle 的状态视觉。 |
| `SplitterTheme.axaml` | 提供控件模板、selector、资源绑定和状态视觉。 |
| `SplitterThemes.axaml` | 聚合控件家族主题资源，保证包级引入顺序稳定。 |

Splitter 使用 `SplitterToken` 作为组件 Token scope。Token 只表达组件视觉语义，不承载 visual option 运行时状态。

主题维护规则：

- 不删除或重命名已经稳定的 ControlTheme key、template part、伪类和资源 key。
- 不把可由 AXAML 表达的模板状态迁移为 C# 动态创建视觉。
- 不把 hover、pressed、selected、expanded、loading、filter、popup open 等运行时状态写入 Token。
- Browser 或平台特化主题必须保持同一 API 的语义一致。

Token 边界：

Splitter Token 只表达组件级视觉变量，例如尺寸、间距、颜色、圆角、阴影、图标尺寸和弹层边界。Token 不承载运行时选择、展开、加载、错误、上传任务、过滤条件或业务状态。

当前 Token scope：

- `SplitterToken`，scope id 为 `Splitter`，源码位于 `src/AtomUI.Desktop.Controls/Splitter/SplitterToken.cs`。

## Customization Boundaries

维护 Splitter 时必须保持以下不变量：

- 不擅自新增、删除、重命名或改变 public/protected API、Avalonia 属性、事件和默认值。
- 不破坏 template part、伪类、ControlTheme key、Token 名称和资源 key。
- 不改变 Gallery 已展示的 XAML 用法、默认外观、交互顺序和状态优先级。
- Template part 重新应用、集合替换、弹层关闭、窗口失活和控件 detach 时必须释放旧订阅和资源宿主。
- 不通过隐藏延迟、强制刷新或吞异常掩盖状态同步问题。
- 不引入运行时反射扫描作为 API、Token 或数据路径发现机制。
- 文档只描述当前稳定设计；历史变化记录在 `changelog.md`。

维护不变量：

维护 Splitter 时不得破坏：

- Public API、默认值、事件顺序和 Gallery 可观察行为。
- Template part 名称、ControlTheme key、伪类和资源 key。
- 旧 template part、事件订阅、Popup/Flyout/Window host 和 collection view 的释放路径。
- Light/Dark、Browser/Desktop 和不同 SizeType 下的主题一致性。
- 文档、Gallery API 表、Token 表与源码契约的一致性。
