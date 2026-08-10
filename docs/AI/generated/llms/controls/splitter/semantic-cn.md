# Splitter 语义结构

> 生成产物：由源文档生成，不要手工编辑。修改内容请回到控件文档、源码 public surface、Token 类型或生成数据、Gallery ShowCase 或源码结构。

## Semantic Parts

| Part | AtomUI 节点 | 职责 | 相关 API | 相关 Token | 稳定性 |
| --- | --- | --- | --- | --- | --- |
| `root` | `Splitter` | 控件根语义区域，承载 public API、Children、事件和 Token scope。 | `Orientation`、`IsLazy`、`HandleSize`、`Children` | `SplitBarHandleSize` | stable |
| `frame` | `SplitterTheme.axaml` / `Border#Frame` | Splitter 整体背景、边框、圆角和裁剪入口。 | `Background`、`BorderBrush`、`BorderThickness`、`CornerRadius` | SharedToken / SplitterToken | stable |
| `panel-host` | `PART_SplitterPanel` | 组织用户面板并生成 handle。 | attached panel properties | `SplitBarHandleSize` | stable |
| `handle` | `SplitterHandle` | 相邻面板之间的交互边界。 | `HandleSize`、collapse API | `HandleLineColor`、`HandleLineHoverColor`、`HandleLineDragColor` | internal stable |
| `drag-bar` | `SplitterDragBar` | 拖拽命中区和 grip 展示。 | `IsLazy`、`HandleSize` | `SplitTriggerSize`、`SplitBarDraggableSize`、`HandleLineThickness` | internal stable |
| `collapse-actions` | collapse `IconButton` | 折叠和恢复相邻面板。 | `CollapsePreviousIcon`、`CollapseNextIcon`、`Splitter.Collapsible` | `HandleIconSize`、`HandleIconColor` | internal stable |

## Abstract AXAML Structure

来源：`src/AtomUI.Desktop.Controls/Splitter/Themes/SplitterTheme.axaml`

```xml
<PixelAlignedBorder Name="Frame">
    <SplitterPanel Name="PART_SplitterPanel" />
</PixelAlignedBorder>
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
     -> PixelAlignedBorder#Frame (template-stable)
        -> SplitterPanel#PART_SplitterPanel (template-stable)
```

### 协作节点

| 节点 | 类型 | 来源 | 生命周期 owner | 影响的 public API | 稳定性 | Agent 使用边界 |
| --- | --- | --- | --- | --- | --- | --- |
| `Splitter` | public control | `源文档 + public API` | 用户代码 / 控件宿主 | public API | public | 用户可直接使用 public 控件；可作为示例和 API 入口。 |
| `SplitterDragBar` | control theme | `SplitterDragBarTheme.axaml` | Splitter | `Background`, `LineBrush`, `LineCornerRadius` | internal-observable | 用于理解结构和状态流，不应指导用户代码直接依赖。 |
| `PART_Grip` | template node (Border) | `SplitterDragBarTheme.axaml` | SplitterDragBar | `LineBrush`, `LineCornerRadius` | template-stable | 用于主题维护；变更需同步主题、实现和 LLMS。 |
| `SplitterHandle` | control theme | `SplitterHandleTheme.axaml` | Splitter | `IsDragEnabled`, `LineBrush`, `LineCornerRadius`, `LineThickness`, `Orientation` | internal-observable | 用于理解结构和状态流，不应指导用户代码直接依赖。 |
| `PART_HandleLine` | template node (Border) | `SplitterHandleTheme.axaml` | SplitterHandle | `LineBrush`, `LineCornerRadius` | template-stable | 用于主题维护；变更需同步主题、实现和 LLMS。 |
| `PART_DragBar` | template node (SplitterDragBar) | `SplitterHandleTheme.axaml` | SplitterHandle | `IsDragEnabled`, `LineCornerRadius`, `LineThickness`, `Orientation` | template-stable | 用于主题维护；变更需同步主题、实现和 LLMS。 |
| `PART_CollapseIconsHost` | template node (Canvas) | `SplitterHandleTheme.axaml` | SplitterHandle | 主题状态 / visual state | template-stable | 用于主题维护；变更需同步主题、实现和 LLMS。 |
| `PART_CollapsePrevButton` | template node (IconButton) | `SplitterHandleTheme.axaml` | SplitterHandle | 主题状态 / visual state | template-stable | 用于主题维护；变更需同步主题、实现和 LLMS。 |
| `PART_CollapseNextButton` | template node (IconButton) | `SplitterHandleTheme.axaml` | SplitterHandle | 主题状态 / visual state | template-stable | 用于主题维护；变更需同步主题、实现和 LLMS。 |
| `Splitter` | control theme | `SplitterTheme.axaml` | 用户代码 / 控件宿主 | `Background`, `BorderBrush`, `BorderThickness`, `CollapseNextIcon`, `CollapsePreviousIcon`, `CornerRadius` | public | 用户可直接使用 public 控件；可作为示例和 API 入口。 |
| `Frame` | template node (PixelAlignedBorder) | `SplitterTheme.axaml` | Splitter | `Background`, `BorderBrush`, `BorderThickness`, `CollapseNextIcon`, `CollapsePreviousIcon`, `CornerRadius` | template-stable | 用于主题维护；变更需同步主题、实现和 LLMS。 |
| `PART_SplitterPanel` | template node (SplitterPanel) | `SplitterTheme.axaml` | Splitter | `CollapseNextIcon`, `CollapsePreviousIcon`, `HandleSize`, `IsLazy`, `LineCornerRadius`, `LineThickness` | template-stable | 用于主题维护；变更需同步主题、实现和 LLMS。 |

## Template Parts

| Template Part | 类型 | 稳定性 | 职责 |
| --- | --- | --- | --- |
| `PART_SplitterPanel` | `SplitterPanel` | public control template part | 承载子面板并生成 internal handle。 |
| `PART_HandleLine` | `Border` | internal handle template part | 展示可见分割线。 |
| `PART_DragBar` | `SplitterDragBar` | internal handle template part | 提供拖拽命中区和拖拽事件源。 |
| `PART_Grip` | `Border` | internal drag-bar template part | 展示拖拽区域内的 grip。 |
| `PART_CollapsePrevButton` | `IconButton` | internal handle template part | 触发前侧面板折叠或展开。 |
| `PART_CollapseNextButton` | `IconButton` | internal handle template part | 触发后侧面板折叠或展开。 |

## Pseudo Classes

伪类模型：

- `Splitter` 根控件当前不定义专属伪类。
- `SplitterHandle` 使用 `:pointerover` 和 `:dragging` 驱动分割线颜色。
- `SplitterDragBar` 使用 `:dragging` 表达拖拽状态。
- `IconButton` 折叠按钮继续使用标准 pointer/pressed/disabled 视觉。

## State Flow

Splitter 的状态流按以下路径收敛：

```text
Public API / attached panel properties / pointer drag / collapse button
  -> SplitterPanel layout state
  -> SplitterHandle drag and collapse state
  -> SplitterDragBar pointer state
  -> ControlTheme selector / template binding / resize events
  -> Gallery 可观察行为
```

行为规则：

- `Orientation=Vertical` 表示左右分割，handle 横向拖动；`Orientation=Horizontal` 表示上下分割，handle 纵向拖动。
- `HandleSize` 控制 handle 在布局中占用的命中区域，不能被可见线条厚度替代。
- `IsLazy=False` 时拖拽过程中实时调整面板尺寸；`IsLazy=True` 时拖拽过程中移动 drag bar，完成后提交尺寸。
- `Splitter.Size` 是用户可双向绑定的实际尺寸入口；`DefaultSize` 是未提供实际尺寸时的初始化入口。
- `MinSize`、`MaxSize`、`IsResizable` 和折叠状态共同决定某个 handle 是否可拖拽。
- 折叠按钮只在相邻面板支持折叠或存在可恢复折叠面板时显示。
- resize 事件以 handle index 和当前尺寸快照暴露，不让用户直接依赖 internal handle 实例。

## Theme and Token Boundaries

Splitter 的视觉模型由根控件模板、internal 面板和 handle 模板、SharedToken、控件 Token 共同构成。

| 主题文件 | 职责 |
| --- | --- |
| `SplitterTheme.axaml` | 定义 `Splitter` 根模板、外层 Frame 和 `PART_SplitterPanel`。 |
| `SplitterHandleTheme.axaml` | 定义可见分割线、拖拽命中区和折叠按钮的组合结构。 |
| `SplitterDragBarTheme.axaml` | 定义拖拽命中区、grip 尺寸、grip 圆角和方向 cursor。 |

视觉语义拆分：

- 根框架：承载 Splitter 整体背景、边框、圆角和裁剪。
- 命中区域：由 `HandleSize` 和 `SplitBarHandleSize` 定义，保证拖拽易用性。
- 可见分割线：由 line thickness、line corner radius 和 line color 定义，不能反向改变命中区域。
- grip：由 drag bar 模板展示，使用较短尺寸提示可拖拽。
- 折叠按钮：依赖 `SplitterPanelCollapsible` 和 hover/press 状态显示，不参与面板尺寸计算。

主题维护规则：

- 不删除或重命名已经稳定的 ControlTheme key、template part、伪类和资源 key。
- 不把可由 AXAML 表达的模板状态迁移为 C# 动态创建视觉。
- 不把 hover、dragging、collapsed 等运行时状态写入 Token。
- 调整线条厚度时必须同时检查 `PART_HandleLine` 和 `PART_Grip`，避免可见线条与拖拽提示尺寸不一致。

Token 边界：

Splitter Token 只表达组件级视觉变量，包括分割线尺寸、拖拽命中区、折叠按钮定位、handle 颜色和图标尺寸。Token 不承载运行时拖拽状态、折叠状态、面板尺寸、业务布局数据或用户内容背景。

当前 Token scope：

- `SplitterToken`，scope id 为 `Splitter`，源码位于 `src/AtomUI.Desktop.Controls/Splitter/SplitterToken.cs`。

## Customization Boundaries

维护 Splitter 时必须保持以下不变量：

- 不改变 `Orientation`、`IsLazy`、`HandleSize`、附加尺寸属性、折叠属性和 resize 事件的默认语义。
- 不把 `HandleSize` 重新定义为可见分割线厚度。
- 不让 internal `SplitterPanel`、`SplitterHandle`、`SplitterDragBar` 成为用户必须引用的样式 API。
- 不删除或重命名 `PART_SplitterPanel`，也不随意重命名 internal handle template part。
- 不改变 Gallery 已展示的 XAML 用法、默认外观、交互顺序和状态优先级。
- 不通过延迟刷新、吞异常或特殊 Gallery 判断掩盖布局状态问题。
- 不引入运行时反射扫描作为 API、Token 或 Gallery 示例发现机制。
- 文档只描述当前稳定设计和维护规则；历史变化记录在 `changelog.md`。

维护不变量：

维护 Splitter 时不得破坏：

- Public API、默认值、事件顺序和 Gallery 可观察行为。
- `HandleSize` 作为 hit area 的语义。
- `SplitterPanel` 作为尺寸与折叠状态 owner 的语义。
- internal handle template part 的绑定关系和事件释放路径。
- Light/Dark、Browser/Desktop 和不同方向下的主题一致性。
- API 契约摘要、Token 语义、ShowCase 示例和控件文档的一致性。

新增分割线样式能力时必须遵守：

- API 定义在 `Splitter`，不定义在 internal handle 上作为用户入口。
- `LineThickness` 不替代 `HandleSize`。
- `LineCornerRadius` 同时作用于 `PART_HandleLine` 和 `PART_Grip`。
- 默认值来自 Splitter Token 或 SharedToken，保证现有视觉不变。
- API 契约摘要、Token 语义、ShowCase 示例和回归测试同步更新。
