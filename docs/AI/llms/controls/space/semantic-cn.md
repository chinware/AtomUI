# Space 语义结构

> 生成产物：由源文档生成，不要手工编辑。修改内容请回到控件文档、Gallery API / Token 表、Gallery ShowCase 或源码结构。

## Semantic Parts

| Part | AtomUI 节点 | 职责 | 相关 API | 相关 Token | 稳定性 |
| --- | --- | --- | --- | --- | --- |
| `root` | `Space` | 布局控件根语义区域，承载布局 public API、尺寸和主题入口。 | 见 API 与契约模型 | 见视觉与主题模型 | stable |
| `container` | `布局容器` | 组织子元素、间距、断点、对齐或分割状态。 | 见 API 与契约模型 | 见视觉与主题模型 | stable |
| `item` | `布局项` | 承载子内容、占位、跨度、排序或尺寸约束。 | 见 API 与契约模型 | 见视觉与主题模型 | stable |
| `theme` | `主题区域` | 连接 SharedToken、布局主题资源和 Gallery 可观察样式。 | 见 API 与契约模型 | 见视觉与主题模型 | stable |

## Abstract AXAML Structure

未定位到可生成抽象 AXAML 结构的 ControlTheme 模板。生成器不会根据 semantic parts 发明 AXAML 节点；请以 Template Parts、主题文件和源码索引为准。

## Composition Model

该控件主要由 public 控件和 ControlTheme 模板直接表达，没有额外运行时组合层。

## Template Parts

| 契约组 | 代表成员 | 维护含义 |
| --- | --- | --- |
| 内容与数据 | `CompactSpaceItemPosition`、`Content`、`ContentTemplate`、`ItemHeight`、`ItemSpacing`、`ItemWidth`、`ItemsAlignment` | 定义控件展示内容、输入数据、模板或业务对象入口。 |
| 选择与集合 | `PositionIndex` | 维护选择、展开、过滤、分页、分组或集合状态。 |
| 交互与状态 | `IsUsedInCompactSpace`、`Status` | 表达用户可观察状态、可用性、清除、加载或反馈语义。 |
| 视觉与布局 | `CompactSpaceOrientation`、`LineSpacing`、`Orientation`、`SizeType`、`StyleVariant` | 影响尺寸、位置、颜色、形状、密度和模板视觉变量。 |

## Pseudo Classes

| 状态反馈 | public API、内部状态和伪类如何形成用户可感知反馈。 | collection/filter、visual option。 |
| 主题语义 | ControlTheme、SharedToken、组件 Token 和模板绑定如何表达视觉。 | Space Token + ControlTheme。 |

## State Flow

Space 的状态流按以下路径收敛：

```text
Public API / inherited command / item source / user input
  -> 控件实例状态
  -> effective state / pseudo-class / template property
  -> ControlTheme selector / presenter / renderer
  -> Gallery 可观察行为
```

状态维护规则：

- Disabled 或不可交互状态优先屏蔽 pointer、keyboard、motion 和提交类反馈。
- collection/filter、visual option 状态由控件实例或明确的数据 owner 推导，不能在 template part 之间双向竞争。
- 模板重套用时必须把 public API 对应状态回放到新的 part、伪类和主题变量。
- 集合、弹层、异步、动效或窗口相关状态必须能处理 reset、close、cancel、detach 和 owner 释放。

## Theme and Token Boundaries

Space 的视觉模型由控件模板、ControlTheme、SharedToken 和必要的组件 Token 共同构成。

| 主题文件 | 职责 |
| --- | --- |
| `CompactSpaceAddOnTheme.axaml` | 提供控件模板、selector、资源绑定和状态视觉。 |
| `CompactSpaceTheme.axaml` | 提供控件模板、selector、资源绑定和状态视觉。 |
| `SpaceThemes.axaml` | 聚合控件家族主题资源，保证包级引入顺序稳定。 |

Space 使用 `SpaceToken` 作为组件 Token scope。Token 只表达组件视觉语义，不承载 collection/filter、visual option 运行时状态。

主题维护规则：

- 不删除或重命名已经稳定的 ControlTheme key、template part、伪类和资源 key。
- 不把可由 AXAML 表达的模板状态迁移为 C# 动态创建视觉。
- 不把 hover、pressed、selected、expanded、loading、filter、popup open 等运行时状态写入 Token。
- Browser 或平台特化主题必须保持同一 API 的语义一致。

Token 边界：

Space Token 只表达组件级视觉变量，例如尺寸、间距、颜色、圆角、阴影、图标尺寸和弹层边界。Token 不承载运行时选择、展开、加载、错误、上传任务、过滤条件或业务状态。

当前 Token scope：

- `SpaceToken`，scope id 为 `Space`，源码位于 `src/AtomUI.Desktop.Controls/Space/SpaceToken.cs`。

## Customization Boundaries

维护 Space 时必须保持以下不变量：

- 不擅自新增、删除、重命名或改变 public/protected API、Avalonia 属性、事件和默认值。
- 不破坏 template part、伪类、ControlTheme key、Token 名称和资源 key。
- 不改变 Gallery 已展示的 XAML 用法、默认外观、交互顺序和状态优先级。
- Template part 重新应用、集合替换、弹层关闭、窗口失活和控件 detach 时必须释放旧订阅和资源宿主。
- 不通过隐藏延迟、强制刷新或吞异常掩盖状态同步问题。
- 不引入运行时反射扫描作为 API、Token 或数据路径发现机制。
- 文档只描述当前稳定设计；历史变化记录在 `changelog.md`。

维护不变量：

维护 Space 时不得破坏：

- Public API、默认值、事件顺序和 Gallery 可观察行为。
- Template part 名称、ControlTheme key、伪类和资源 key。
- 旧 template part、事件订阅、Popup/Flyout/Window host 和 collection view 的释放路径。
- Light/Dark、Browser/Desktop 和不同 SizeType 下的主题一致性。
- 文档、Gallery API 表、Token 表与源码契约的一致性。
