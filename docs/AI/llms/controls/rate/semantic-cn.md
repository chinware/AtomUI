# Rate 语义结构

> 生成产物：由源文档生成，不要手工编辑。修改内容请回到控件文档、源码 public surface、Token 类型或生成数据、Gallery ShowCase 或源码结构。

## Semantic Parts

| Part | AtomUI 节点 | 职责 | 相关 API | 相关 Token | 稳定性 |
| --- | --- | --- | --- | --- | --- |
| `root` | `Rate` | 数据录入控件根语义区域，承载 public API、值状态、验证状态和主题入口。 | 见 API 与契约模型 | 见视觉与主题模型 | stable |
| `input` | `输入或编辑区域` | 承载用户输入、当前值、占位、格式化或只读状态。 | 见 API 与契约模型 | 见视觉与主题模型 | stable |
| `trigger` | `触发区域` | 承载清除、展开、提交、步进、上传或辅助操作。 | 见 API 与契约模型 | 见视觉与主题模型 | stable |
| `popup` | `弹层或候选区域` | 承载下拉、候选项、日历、颜色面板或异步内容。 | 见 API 与契约模型 | 见视觉与主题模型 | stable |
| `validation` | `校验反馈区域` | 承载 Form、status、错误、警告、help 或 loading 状态。 | 见 API 与契约模型 | 见视觉与主题模型 | stable |

## Abstract AXAML Structure

来源：`src/AtomUI.Desktop.Controls/Rate/Themes/RateTheme.axaml`

```xml
<PixelAlignedBorder Name="Frame">
    <RateItemsControl Name="PART_RateItems" />
</PixelAlignedBorder>
```

## Composition Model

该章节由控件 `Themes/` 文件夹中的真实主题文件生成，用于说明 public 控件与内部协作对象之间的运行时结构。内部节点只用于理解和维护，不应指导用户代码直接依赖。

### 控件角色图

```text
Rate
  -> RateItem (item container control theme, RateItemTheme.axaml)
     -> Border (template-stable)
        -> Panel (template-stable)
           -> DashedBorder (template-stable)
           -> Rectangle (template-stable)
           -> Rectangle#ActiveItem (template-stable)
  -> RateItemsControl (control theme, RateItemsControlTheme.axaml)
     -> ItemsPresenter#PART_ItemsPresenter (template-stable)
  -> Rate (control theme, RateTheme.axaml)
     -> PixelAlignedBorder#Frame (template-stable)
        -> RateItemsControl#PART_RateItems (template-stable)
```

### 协作节点

| 节点 | 类型 | 来源 | 生命周期 owner | 影响的 public API | 稳定性 | Agent 使用边界 |
| --- | --- | --- | --- | --- | --- | --- |
| `Rate` | public control | `源文档 + public API` | 用户代码 / 控件宿主 | public API | public | 用户可直接使用 public 控件；可作为示例和 API 入口。 |
| `RateItem` | item container control theme | `RateItemTheme.axaml` | Rate | `Background`, `CharacterBgBrush`, `CharacterBrush`, `IsFocusStartItem`, `StarClip`, `StarColor` | template-stable | 用于主题维护；变更需同步主题、实现和 LLMS。 |
| `Panel` | template node (Panel) | `RateItemTheme.axaml` | RateItem | `CharacterBgBrush`, `IsFocusStartItem`, `StarClip`, `StarColor` | template-stable | 用于主题维护；变更需同步主题、实现和 LLMS。 |
| `ActiveItem` | template node (Rectangle) | `RateItemTheme.axaml` | RateItem | `StarClip` | template-stable | 用于主题维护；变更需同步主题、实现和 LLMS。 |
| `RateItemsControl` | control theme | `RateItemsControlTheme.axaml` | Rate | 主题状态 / visual state | template-stable | 用于主题维护；变更需同步主题、实现和 LLMS。 |
| `PART_ItemsPresenter` | template node (ItemsPresenter) | `RateItemsControlTheme.axaml` | RateItemsControl | 主题状态 / visual state | template-stable | 用于主题维护；变更需同步主题、实现和 LLMS。 |
| `Rate` | control theme | `RateTheme.axaml` | 用户代码 / 控件宿主 | `Background`, `BorderBrush`, `BorderThickness`, `Character`, `CornerRadius`, `FontSize` | public | 用户可直接使用 public 控件；可作为示例和 API 入口。 |
| `Frame` | template node (PixelAlignedBorder) | `RateTheme.axaml` | Rate | `Background`, `BorderBrush`, `BorderThickness`, `Character`, `CornerRadius`, `FontSize` | template-stable | 用于主题维护；变更需同步主题、实现和 LLMS。 |
| `PART_RateItems` | template node (RateItemsControl) | `RateTheme.axaml` | Rate | `Character`, `FontSize`, `IsAllowClear`, `IsAllowHalf`, `IsMotionEnabled`, `StarBgColor` | template-stable | 用于主题维护；变更需同步主题、实现和 LLMS。 |

## Template Parts

| 契约组 | 代表成员 | 维护含义 |
| --- | --- | --- |
| 内容与数据 | `DefaultValue`、`Value` | 定义控件展示内容、输入数据、模板或业务对象入口。 |
| 选择与集合 | `Count`、`SelectedState` | 维护选择、展开、过滤、分页、分组或集合状态。 |
| 交互与状态 | `IsAllowClear`、`IsAllowHalf`、`IsKeyboardEnabled`、`IsMotionEnabled` | 表达用户可观察状态、可用性、清除、加载或反馈语义。 |
| 视觉与布局 | `Background`、`FontSize`、`FontStyle`、`Foreground`、`SizeType`、`StarBgColor`、`StarColor` | 影响尺寸、位置、颜色、形状、密度和模板视觉变量。 |
| 其他稳定入口 | `Character`、`FontFamily`、`FontWeight` | 保留为 public surface，变更前需确认 Gallery 和用户 XAML 依赖。 |

## Pseudo Classes

| 状态反馈 | public API、内部状态和伪类如何形成用户可感知反馈。 | selection/checked/active、input/value、motion、visual option。 |
| 主题语义 | ControlTheme、SharedToken、组件 Token 和模板绑定如何表达视觉。 | Rate Token + ControlTheme。 |

## State Flow

Rate 的状态流按以下路径收敛：

```text
Public API / inherited command / item source / user input
  -> 控件实例状态
  -> effective state / pseudo-class / template property
  -> ControlTheme selector / presenter / renderer
  -> Gallery 可观察行为
```

状态维护规则：

- Disabled 或不可交互状态优先屏蔽 pointer、keyboard、motion 和提交类反馈。
- selection/checked/active、input/value、motion、visual option 状态由控件实例或明确的数据 owner 推导，不能在 template part 之间双向竞争。
- `Rate.Value` 是评分控件的外部值 owner，默认 `BindingMode.TwoWay` 并启用 Avalonia data validation；用户评分、键盘调整、Form value 和 ViewModel 更新必须收敛到同一份数值状态。
- 模板重套用时必须把 public API 对应状态回放到新的 part、伪类和主题变量。
- 集合、弹层、异步、动效或窗口相关状态必须能处理 reset、close、cancel、detach 和 owner 释放。

## Theme and Token Boundaries

Rate 的视觉模型由控件模板、ControlTheme、SharedToken 和必要的组件 Token 共同构成。

| 主题文件 | 职责 |
| --- | --- |
| `RateItemTheme.axaml` | 定义集合项、容器项或局部单元的状态视觉。 |
| `RateItemsControlTheme.axaml` | 定义集合项、容器项或局部单元的状态视觉。 |
| `RateTheme.axaml` | 提供控件模板、selector、资源绑定和状态视觉。 |
| `RateThemes.axaml` | 聚合控件家族主题资源，保证包级引入顺序稳定。 |

Rate 使用 `RateToken` 作为组件 Token scope。Token 只表达组件视觉语义，不承载 selection/checked/active、input/value、motion、visual option 运行时状态。

主题维护规则：

- 不删除或重命名已经稳定的 ControlTheme key、template part、伪类和资源 key。
- 不把可由 AXAML 表达的模板状态迁移为 C# 动态创建视觉。
- 不把 hover、pressed、selected、expanded、loading、filter、popup open 等运行时状态写入 Token。
- Browser 或平台特化主题必须保持同一 API 的语义一致。

Token 边界：

Rate Token 只表达组件级视觉变量，例如尺寸、间距、颜色、圆角、阴影、图标尺寸和弹层边界。Token 不承载运行时选择、展开、加载、错误、上传任务、过滤条件或业务状态。

当前 Token scope：

- `RateToken`，scope id 为 `Rate`，源码位于 `src/AtomUI.Desktop.Controls/Rate/RateToken.cs`。

## Customization Boundaries

维护 Rate 时必须保持以下不变量：

- 不擅自新增、删除、重命名或改变 public/protected API、Avalonia 属性、事件和默认值。
- 不破坏 template part、伪类、ControlTheme key、Token 名称和资源 key。
- 不改变 Gallery 已展示的 XAML 用法、默认外观、交互顺序和状态优先级。
- Template part 重新应用、集合替换、弹层关闭、窗口失活和控件 detach 时必须释放旧订阅和资源宿主。
- 不通过隐藏延迟、强制刷新或吞异常掩盖状态同步问题。
- 不引入运行时反射扫描作为 API、Token 或数据路径发现机制。
- 文档只描述当前稳定设计；历史变化记录在 `changelog.md`。

维护不变量：

维护 Rate 时不得破坏：

- Public API、默认值、事件顺序和 Gallery 可观察行为。
- Template part 名称、ControlTheme key、伪类和资源 key。
- 旧 template part、事件订阅、Popup/Flyout/Window host 和 collection view 的释放路径。
- Light/Dark、Browser/Desktop 和不同 SizeType 下的主题一致性。
- 文档、Gallery API 表、Token 表与源码契约的一致性。
