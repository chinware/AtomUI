# Breadcrumb 语义结构

> 生成产物：由源文档生成，不要手工编辑。修改内容请回到控件文档、源码 public surface、Token 类型或生成数据、Gallery ShowCase 或源码结构。

## Semantic Parts

| Part | AtomUI 节点 | Selector | ContractType | Cardinality | Customization | CrossVisualRoot | RuntimeCreated |
| --- | --- | --- | --- | --- | --- | --- | --- |
| `root` | `Breadcrumb` owner | owner | `Breadcrumb` | `Single` | `Root` | `false` | `false` |
| `item` | 条目容器 | `> .semantic-item` | `BreadcrumbItem` | `Multiple` | `Selector` | `false` | `true` |
| `separator` | 条目容器之间的兄弟分隔元素 | `> .semantic-separator` | `ContentPresenter` | `Multiple` | `Selector` | `false` | `true` |

`root` 不声明 `.semantic-root` marker，也不生成独立 Style type。`item` 和 `separator` 都是运行时创建的语义标记：
`item` 由 `Breadcrumb` 在 `CreateContainerForItemOverride` 与 `PrepareContainerForItemOverride` 中通过生成的
`BreadcrumbSemanticParts.ItemClass` 挂到每个 `BreadcrumbItem` 容器上；`separator` 由 `Breadcrumb` 在分隔符创建路径
中通过 `BreadcrumbSemanticParts.SeparatorClass` 挂到每个运行时创建的兄弟分隔 `ContentPresenter` 上。分隔符是
`Breadcrumb` 的逻辑子级（供 `> .semantic-separator` 路由匹配）、items panel 的视觉子级（参与布局），不进入 panel 的
`Children` 集合，避免污染条目生成器基于面板位置的容器索引。`separator` 的标记数量为条目数量减一，每个分隔符尾随
其前一条目，最后一条目没有分隔符。

内置主题不依赖静态 `.semantic-*` marker：`BreadcrumbTheme.axaml` 与 `BreadcrumbItemTheme.axaml` 都不包含静态
semantic class，语义契约完全由 runtime marker 与生成的 `Style` 类型表达。分隔符是运行时创建的兄弟节点，无法被
`^ /template/` 主题 selector 覆盖，因此默认前景色与间距由 `Breadcrumb` 通过 `TokenResourceBinder` 控件 Token 绑定
提供（`SeparatorColor` / `SeparatorMargin`），而不是模板字面值，从而允许应用 Semantic Style 覆盖该颜色。

带导航能力的条目（`IsNavigateResponsive=True`）的链接前景色由主题 selector `^ /template/ ContentPresenter#Content`
提供（绑定 `LinkColor` Token），直接落在内容呈现器上——对齐参考实现的链接锚点着色规则（`.ant-breadcrumb-item a`）。因此
item 级 Semantic Style（如 `Foreground`）不会改变链接条目的文字颜色：链接条目始终使用 `LinkColor`，只有普通条目的
文字颜色可被 item 样式定制。悬浮时的 `LinkHoverColor` 同样作用在内容呈现器上。

## Abstract AXAML Structure

来源：`src/AtomUI.Desktop.Controls/Breadcrumb/Themes/BreadcrumbTheme.axaml`

```xml
<ItemsPresenter />
```

## Composition Model

该章节由控件 `Themes/` 文件夹中的真实主题文件生成，用于说明 public 控件与内部协作对象之间的运行时结构。内部节点只用于理解和维护，不应指导用户代码直接依赖。

### 控件角色图

```text
Breadcrumb
  -> BreadcrumbItem (item container control theme, BreadcrumbItemTheme.axaml)
     -> Border#ContentInfoFrame (template-stable)
        -> StackPanel (template-stable)
           -> IconPresenter#IconPresenter (internal-observable)
           -> ContentPresenter#Content (internal-observable)
  -> Breadcrumb (control theme, BreadcrumbTheme.axaml)
     -> ItemsPresenter (internal-observable)
```

### 协作节点

| 节点 | 类型 | 来源 | 生命周期 owner | 影响的 public API | 稳定性 | Agent 使用边界 |
| --- | --- | --- | --- | --- | --- | --- |
| `Breadcrumb` | public control | `源文档 + public API` | 用户代码 / 控件宿主 | public API | public | 用户可直接使用 public 控件；可作为示例和 API 入口。 |
| `BreadcrumbItem` | item container control theme | `BreadcrumbItemTheme.axaml` | 用户代码 / 控件宿主 | `Background`, `Content`, `ContentTemplate`, `CornerRadius`, `Icon`, `Padding` | public | 用户可直接使用 public 控件；可作为示例和 API 入口。 |
| `ContentInfoFrame` | template node (Border) | `BreadcrumbItemTheme.axaml` | BreadcrumbItem | `Background`, `Content`, `ContentTemplate`, `CornerRadius`, `Icon`, `Padding` | template-stable | 用于主题维护；变更需同步主题、实现和 LLMS。 |
| `StackPanel` | template node (StackPanel) | `BreadcrumbItemTheme.axaml` | BreadcrumbItem | `Content`, `ContentTemplate`, `Icon` | template-stable | 用于主题维护；变更需同步主题、实现和 LLMS。 |
| `IconPresenter` | template node (IconPresenter) | `BreadcrumbItemTheme.axaml` | BreadcrumbItem | `Icon` | internal-observable | 用于理解结构和状态流，不应指导用户代码直接依赖。 |
| `Content` | template node (ContentPresenter) | `BreadcrumbItemTheme.axaml` | BreadcrumbItem | `Content`, `ContentTemplate` | internal-observable | 用于理解结构和状态流，不应指导用户代码直接依赖。 |
| `Breadcrumb` | control theme | `BreadcrumbTheme.axaml` | 用户代码 / 控件宿主 | 主题状态 / visual state | public | 用户可直接使用 public 控件；可作为示例和 API 入口。 |
| `ItemsPresenter` | template node (ItemsPresenter) | `BreadcrumbTheme.axaml` | Breadcrumb | 主题状态 / visual state | internal-observable | 用于理解结构和状态流，不应指导用户代码直接依赖。 |

## Template Parts

| 契约组 | 代表成员 | 维护含义 |
| --- | --- | --- |
| 内容与数据 | `Icon`、`SeparatorTemplate` | 定义控件展示内容、输入数据、模板或业务对象入口。 |
| 交互与状态 | `IsMotionEnabled` | 表达用户可观察状态、可用性、清除、加载或反馈语义。 |
| 视觉与布局 | `Background`、`BorderBrush`、`BorderThickness`、`CornerRadius`、`Padding` | 表达 root frame 的边框、背景、圆角与内边距（继承自 `TemplatedControl`），配合 Semantic Part 根定制边界使用。 |
| 其他稳定入口 | `NavigateContext`、`NavigateUri`、`Separator` | 保留为 public surface，变更前需确认 Gallery 和用户 XAML 依赖。 |

## Pseudo Classes

| 状态反馈 | public API、内部状态和伪类如何形成用户可感知反馈。 | motion。 |
| 主题语义 | ControlTheme、SharedToken、控件 Token 和模板绑定如何表达视觉。 | Breadcrumb Token + ControlTheme。 |

## State Flow

Breadcrumb 的状态流按以下路径收敛：

```text
Public API / inherited command / item source / user input
  -> 控件实例状态
  -> effective state / pseudo-class / template property
  -> ControlTheme selector / presenter / renderer
  -> Gallery 可观察行为
```

状态维护规则：

- Disabled 或不可交互状态优先屏蔽 pointer、keyboard、motion 和提交类反馈。
- motion 状态由控件实例或明确的数据 owner 推导，不能在 template part 之间双向竞争。
- 模板重套用时必须把 public API 对应状态回放到新的 part、伪类和主题变量。
- 集合、弹层、异步、动效或窗口相关状态必须能处理 reset、close、cancel、detach 和 owner 释放。

## Theme and Token Boundaries

Breadcrumb 的视觉模型由控件模板、ControlTheme、SharedToken 和必要的控件 Token 共同构成。

| 主题文件 | 职责 |
| --- | --- |
| `BreadcrumbItemTheme.axaml` | 定义集合项、容器项或局部单元的状态视觉。 |
| `BreadcrumbTheme.axaml` | 提供控件模板、selector、资源绑定和状态视觉。 |

Breadcrumb 使用 `BreadcrumbToken` 作为控件 Token scope。Token 只表达组件视觉语义，不承载 motion 运行时状态。

root frame 由 `Breadcrumb` 自身在 `Render` 中绘制（复用 `BorderRenderHelper`），边框、背景、圆角与内边距由继承自
`TemplatedControl` 的 root frame 样式属性表达，定制边界见 [Breadcrumb Semantic Part 契约](semantic-part.md)第 3 节。
分隔符是条目容器之间的兄弟元素（N-1 个、尾随其前一条目），由 `Breadcrumb` 运行时创建并交给 `BreadcrumbItemsPanel`
交错布局；其默认前景色与间距经 `TokenResourceBinder` 绑定 `SeparatorColor` / `SeparatorMargin`，保持 Template 绑定
优先级以便 Semantic Part 样式覆盖。带导航能力的条目（`IsNavigateResponsive=True`）的链接前景色绑定 `LinkColor`，
作用在条目内容呈现器（`^ /template/ ContentPresenter#Content`）上，对齐参考实现的链接锚点着色规则
（`.ant-breadcrumb-item a`），item 级 Semantic Part 样式不改变链接文字颜色。主题默认 `HorizontalAlignment=Stretch`，
root 作为块级元素铺满可用宽度。

主题维护规则：

- 不删除或重命名已经稳定的 ControlTheme key、template part、伪类和资源 key。
- 不把可由 AXAML 表达的模板状态迁移为 C# 动态创建视觉。
- 不把 hover、pressed、selected、expanded、loading、filter、popup open 等运行时状态写入 Token。
- Browser 或平台特化主题必须保持同一 API 的语义一致。

Token 边界：

Breadcrumb Token 只表达组件级视觉变量，例如尺寸、间距、颜色、圆角、阴影、图标尺寸和弹层边界。Token 不承载运行时选择、展开、加载、错误、上传任务、过滤条件或业务状态。

当前 Token scope：

- `BreadcrumbToken`，scope id 为 `Breadcrumb`，源码位于 `src/AtomUI.Desktop.Controls/Breadcrumb/BreadcrumbToken.cs`。

## Customization Boundaries

维护 Breadcrumb 时必须保持以下不变量：

- 不擅自新增、删除、重命名或改变 public/protected API、Avalonia 属性、事件和默认值。
- 不破坏 template part、伪类、ControlTheme key、Token 名称和资源 key。
- 不改变 Gallery 已展示的 XAML 用法、默认外观、交互顺序和状态优先级。
- Template part 重新应用、集合替换、弹层关闭、窗口失活和控件 detach 时必须释放旧订阅和资源宿主。
- 不通过隐藏延迟、强制刷新或吞异常掩盖状态同步问题。
- 不引入运行时反射扫描作为 API、Token 或数据路径发现机制。
- 文档只描述当前稳定设计；历史变化记录在 `changelog.md`。

维护不变量：

维护 Breadcrumb 时不得破坏：

- Public API、默认值、事件顺序和 Gallery 可观察行为。
- Template part 名称、ControlTheme key、伪类和资源 key。
- 旧 template part、事件订阅、Popup/Flyout/Window host 和 collection view 的释放路径。
- Light/Dark、Browser/Desktop 和不同 SizeType 下的主题一致性。
- 控件文档、源码 public surface、Token 类型或生成数据与源码契约的一致性。
