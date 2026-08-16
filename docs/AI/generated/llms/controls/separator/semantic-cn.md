# Separator 语义结构

> 生成产物：由源文档生成，不要手工编辑。修改内容请回到控件文档、源码 public surface、Token 类型或生成数据、Gallery ShowCase 或源码结构。

## Semantic Parts

Separator 主控件公开 `root`、`rail` 与 `content` 三个职责区域，与上游稳定 Semantic DOM（`root` / `rail` /
`content`）对齐。`rail` 对应连接线（背景条）区域，由模板中的 `SeparatorRail` 节点承载；`content` 对应带标题分隔线中的文本区域。

`VerticalSeparator` 是 `Separator` 的便捷子类，仅覆盖 `Orientation` 默认值并通过 `StyleKeyOverride` 复用
`SeparatorTheme`，不持有独立 descriptor，其 Semantic Part 契约与 `Separator` 完全一致。

| Part | Selector | ContractType | Cardinality | Customization | CrossVisualRoot | RuntimeCreated |
| --- | --- | --- | --- | --- | --- | --- |
| `root` | owner | `Separator` | `Single` | `Root` | `false` | `false` |
| `rail` | `.semantic-rail` | `SeparatorRail` | `Multiple` | `Selector` | `false` | `false` |
| `content` | `.semantic-content` | `TextBlock` | `Single` | `Selector` | `false` | `false` |

`root` 是控件自身，承载方向、尺寸、线型、标题位置等 public API、主题入口和状态归一，不声明 `.semantic-root`
marker。`rail` 是模板中的连接线节点 `PART_RailStart` / `PART_RailEnd`，以 `Multiple` 基数表示带标题水平分隔线的
左右两段；无标题水平分隔线只保留一段可见 rail，垂直分隔线只保留一段可见 rail，另一段被归零宽/高而不参与
Semantic Part 命中。`content` 是模板文本节点 `PART_Title`，承载 `Title` 文本内容，对应公共 API
`Title` / `TitleColor` / `TitlePosition` / `IsPlain`。

## Abstract AXAML Structure

来源：`src/AtomUI.Desktop.Controls/Separator/Themes/SeparatorTheme.axaml`

```xml
<Panel>
    <SeparatorRail Name="PART_RailStart" />
    <TextBlock Name="PART_Title" />
    <SeparatorRail Name="PART_RailEnd" />
</Panel>
```

## Composition Model

该章节由控件 `Themes/` 文件夹中的真实主题文件生成，用于说明 public 控件与内部协作对象之间的运行时结构。内部节点只用于理解和维护，不应指导用户代码直接依赖。

### 控件角色图

```text
Separator
  -> Separator (control theme, SeparatorTheme.axaml)
     -> Panel (template-stable)
        -> SeparatorRail#PART_RailStart (template-stable)
        -> TextBlock#PART_Title (template-stable)
        -> SeparatorRail#PART_RailEnd (template-stable)
```

### 协作节点

| 节点 | 类型 | 来源 | 生命周期 owner | 影响的 public API | 稳定性 | Agent 使用边界 |
| --- | --- | --- | --- | --- | --- | --- |
| `Separator` | public control | `源文档 + public API` | 用户代码 / 控件宿主 | public API | public | 用户可直接使用 public 控件；可作为示例和 API 入口。 |
| `Separator` | control theme | `SeparatorTheme.axaml` | 用户代码 / 控件宿主 | `FontSize`, `LineColor`, `LineWidth`, `Orientation`, `Title`, `TitleColor` | public | 用户可直接使用 public 控件；可作为示例和 API 入口。 |
| `Panel` | template node (Panel) | `SeparatorTheme.axaml` | Separator | `FontSize`, `LineColor`, `LineWidth`, `Orientation`, `Title`, `TitleColor` | template-stable | 用于主题维护；变更需同步主题、实现和 LLMS。 |
| `PART_RailStart` | template node (SeparatorRail) | `SeparatorTheme.axaml` | Separator | `LineColor`, `LineWidth`, `Orientation`, `Variant` | template-stable | 用于主题维护；变更需同步主题、实现和 LLMS。 |
| `PART_Title` | template node (TextBlock) | `SeparatorTheme.axaml` | Separator | `FontSize`, `Title`, `TitleColor` | template-stable | 用于主题维护；变更需同步主题、实现和 LLMS。 |
| `PART_RailEnd` | template node (SeparatorRail) | `SeparatorTheme.axaml` | Separator | `LineColor`, `LineWidth`, `Orientation`, `Variant` | template-stable | 用于主题维护；变更需同步主题、实现和 LLMS。 |

## Template Parts

| 契约组 | 代表成员 | 维护含义 |
| --- | --- | --- |
| 内容与数据 | `Title`、`TitleColor`、`TitlePosition` | 定义控件展示内容、输入数据、模板或业务对象入口。 |
| 交互与状态 | `IsPlain` | 表达用户可观察状态、可用性、清除、加载或反馈语义。 |
| 视觉与布局 | `LineColor`、`LineWidth`、`Orientation`、`OrientationMargin`、`SizeType`、`Variant` | 影响尺寸、位置、颜色、形状、密度和模板视觉变量。 |

## Pseudo Classes

| 状态反馈 | public API、内部状态和伪类如何形成用户可感知反馈。 | visual option。 |
| 主题语义 | ControlTheme、SharedToken、控件 Token 和模板绑定如何表达视觉。 | Separator Token + ControlTheme。 |

## State Flow

Separator 的状态流按以下路径收敛：

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

### 4.1 可自定义间距尺寸

`AbstractSeparator` 实现 `ICustomizableSizeTypeAware`，`SizeType` 使用 `CustomizableSizeType`，默认值为
`Middle`。水平 Separator 的预设尺寸控制上下外间距：`Small`、`Middle`、`Large` 分别映射到控件 Token 的
小、中、大 block margin；该规则对带标题和无标题的水平 Separator 一致，垂直 Separator 不应用这组间距。

`SizeType=Custom` 表示调用方接管间距。Theme 保留 Middle block margin 作为未指定 `Margin` 时的基础值，但不为
`Custom` 声明专属 selector；调用方可以通过实例 `Margin` 或 owner-scoped Style 覆盖。组合控件模板中的结构性
Separator 应使用 `Custom`，并由组合控件自身的 Theme 明确设置间距。

## Theme and Token Boundaries

Separator 的视觉模型由控件模板、ControlTheme、SharedToken 和必要的控件 Token 共同构成。

| 主题文件 | 职责 |
| --- | --- |
| `SeparatorTheme.axaml` | 提供控件模板、selector、资源绑定和状态视觉。 |

Separator 使用 `SeparatorToken` 作为控件 Token scope。Token 只表达组件视觉语义，不承载 visual option 运行时状态。

主题维护规则：

- 不删除或重命名已经稳定的 ControlTheme key、template part、伪类和资源 key。
- 不把可由 AXAML 表达的模板状态迁移为 C# 动态创建视觉。
- 不把 hover、pressed、selected、expanded、loading、filter、popup open 等运行时状态写入 Token。
- Browser 或平台特化主题必须保持同一 API 的语义一致。

Token 边界：

Separator Token 只表达组件级视觉变量，例如尺寸、间距、颜色、圆角、阴影、图标尺寸和弹层边界。Token 不承载运行时选择、展开、加载、错误、上传任务、过滤条件或业务状态。

当前 Token scope：

- `SeparatorToken`，scope id 为 `Separator`，源码位于 `src/AtomUI.Desktop.Controls/Separator/SeparatorToken.cs`。

## Customization Boundaries

维护 Separator 时必须保持以下不变量：

- 不擅自新增、删除、重命名或改变 public/protected API、Avalonia 属性、事件和默认值。
- 不破坏 template part、伪类、ControlTheme key、Token 名称和资源 key。
- 不改变 Gallery 已展示的 XAML 用法、默认外观、交互顺序和状态优先级。
- Template part 重新应用、集合替换、弹层关闭、窗口失活和控件 detach 时必须释放旧订阅和资源宿主。
- 不通过隐藏延迟、强制刷新或吞异常掩盖状态同步问题。
- 不引入运行时反射扫描作为 API、Token 或数据路径发现机制。
- 文档只描述当前稳定设计；历史变化记录在 `changelog.md`。

维护不变量：

维护 Separator 时不得破坏：

- Public API、默认值、事件顺序和 Gallery 可观察行为。
- Template part 名称、ControlTheme key、伪类和资源 key。
- 旧 template part、事件订阅、Popup/Flyout/Window host 和 collection view 的释放路径。
- Light/Dark、Browser/Desktop 和不同 SizeType 下的主题一致性。
- 控件文档、源码 public surface、Token 类型或生成数据与源码契约的一致性。
