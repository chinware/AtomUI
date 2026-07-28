# ButtonSpinner 语义结构

> 生成产物：由源文档生成，不要手工编辑。修改内容请回到控件文档、源码 public surface、Token 类型或生成数据、Gallery ShowCase 或源码结构。

## Semantic Parts

| Part | AtomUI 节点 | 职责 | 相关 API | 相关 Token | 稳定性 |
| --- | --- | --- | --- | --- | --- |
| `root` | `ButtonSpinner` | 导航控件根语义区域，承载 public API、状态归一和主题入口。 | 见 API 与契约模型 | 见视觉与主题模型 | stable |
| `trigger` | `触发区域` | 承载点击、键盘、打开关闭、跳转或提交入口。 | 见 API 与契约模型 | 见视觉与主题模型 | stable |
| `item` | `导航项区域` | 承载当前项、选中项、禁用项、层级项或分页项状态。 | 见 API 与契约模型 | 见视觉与主题模型 | stable |
| `popup` | `弹层或内容区域` | 承载 flyout、dropdown、tab content、submenu 或候选内容。 | 见 API 与契约模型 | 见视觉与主题模型 | stable |
| `motion` | `动效区域` | 表达打开关闭、选中指示、切换和过渡反馈。 | 见 API 与契约模型 | 见视觉与主题模型 | stable |

## Abstract AXAML Structure

来源：`src/AtomUI.Desktop.Controls/ButtonSpinner/Themes/ButtonSpinnerTheme.axaml`

```xml
<ButtonSpinnerDecoratedBox Name="PART_DecoratedBox" />
```

## Composition Model

该章节由控件 `Themes/` 文件夹中的真实主题文件生成，用于说明 public 控件与内部协作对象之间的运行时结构。内部节点只用于理解和维护，不应指导用户代码直接依赖。

### 控件角色图

```text
ButtonSpinner
  -> ButtonSpinnerDecoratedBox (control theme, ButtonSpinnerDecoratedBoxTheme.axaml)
     -> DockPanel#RootLayout (template-stable)
        -> PixelAlignedBorder#{x:Static atom:AddOnDecoratedBoxThemeConstants.LeftAddOnPart} (template-stable)
           -> AddOnContentPresenter#PART_LeftAddOnPresenter (template-stable)
        -> PixelAlignedBorder#{x:Static atom:AddOnDecoratedBoxThemeConstants.RightAddOnPart} (template-stable)
           -> AddOnContentPresenter#PART_RightAddOnPresenter (template-stable)
        -> Panel (template-stable)
           -> AddOnDecoratedBoxContentFrame#{x:Static atom:AddOnDecoratedBoxThemeConstants.ContentFramePart} (template-stable)
              -> ButtonSpinnerContentPanel#ContentLayout (internal-observable)
                 -> AddOnContentPresenter#{x:Static atom:AddOnDecoratedBoxThemeConstants.ContentLeftAddOnPart} (internal-observable)
                 -> AddOnContentPresenter#{x:Static atom:AddOnDecoratedBoxThemeConstants.ContentRightAddOnPart} (internal-observable)
                 -> ContentPresenter#{x:Static atom:AddOnDecoratedBoxThemeConstants.ContentPresenterPart} (internal-observable)
           -> ContentPresenter#PART_SpinnerHandle (template-stable)
  -> ButtonSpinnerHandle (control theme, ButtonSpinnerHandleTheme.axaml)
     -> UniformGrid (template-stable)
        -> IconButton#PART_IncreaseButton (template-stable)
        -> IconButton#PART_DecreaseButton (template-stable)
  -> ButtonSpinner (control theme, ButtonSpinnerTheme.axaml)
     -> ButtonSpinnerDecoratedBox#PART_DecoratedBox (template-stable)
```

### 协作节点

| 节点 | 类型 | 来源 | 生命周期 owner | 影响的 public API | 稳定性 | Agent 使用边界 |
| --- | --- | --- | --- | --- | --- | --- |
| `ButtonSpinner` | public control | `源文档 + public API` | 用户代码 / 控件宿主 | public API | public | 用户可直接使用 public 控件；可作为示例和 API 入口。 |
| `ButtonSpinnerDecoratedBox` | control theme | `ButtonSpinnerDecoratedBoxTheme.axaml` | ButtonSpinner | `Background`, `BorderBrush`, `ButtonSpinnerLocation`, `Content`, `ContentLeftAddOn`, `ContentLeftAddOnTemplate` | internal-observable | 用于理解结构和状态流，不应指导用户代码直接依赖。 |
| `RootLayout` | template node (DockPanel) | `ButtonSpinnerDecoratedBoxTheme.axaml` | ButtonSpinnerDecoratedBox | `Background`, `BorderBrush`, `ButtonSpinnerLocation`, `Content`, `ContentLeftAddOn`, `ContentLeftAddOnTemplate` | template-stable | 用于主题维护；变更需同步主题、实现和 LLMS。 |
| `{x:Static atom:AddOnDecoratedBoxThemeConstants.LeftAddOnPart}` | template node (PixelAlignedBorder) | `ButtonSpinnerDecoratedBoxTheme.axaml` | ButtonSpinnerDecoratedBox | `LeftAddOn`, `LeftAddOnBorderThickness`, `LeftAddOnCornerRadius`, `LeftAddOnTemplate` | template-stable | 用于主题维护；变更需同步主题、实现和 LLMS。 |
| `PART_LeftAddOnPresenter` | template node (AddOnContentPresenter) | `ButtonSpinnerDecoratedBoxTheme.axaml` | ButtonSpinnerDecoratedBox | `LeftAddOn`, `LeftAddOnTemplate` | template-stable | 用于主题维护；变更需同步主题、实现和 LLMS。 |
| `{x:Static atom:AddOnDecoratedBoxThemeConstants.RightAddOnPart}` | template node (PixelAlignedBorder) | `ButtonSpinnerDecoratedBoxTheme.axaml` | ButtonSpinnerDecoratedBox | `RightAddOn`, `RightAddOnBorderThickness`, `RightAddOnCornerRadius`, `RightAddOnTemplate` | template-stable | 用于主题维护；变更需同步主题、实现和 LLMS。 |
| `PART_RightAddOnPresenter` | template node (AddOnContentPresenter) | `ButtonSpinnerDecoratedBoxTheme.axaml` | ButtonSpinnerDecoratedBox | `RightAddOn`, `RightAddOnTemplate` | template-stable | 用于主题维护；变更需同步主题、实现和 LLMS。 |
| `Panel` | template node (Panel) | `ButtonSpinnerDecoratedBoxTheme.axaml` | ButtonSpinnerDecoratedBox | `Background`, `BorderBrush`, `ButtonSpinnerLocation`, `Content`, `ContentLeftAddOn`, `ContentLeftAddOnTemplate` | template-stable | 用于主题维护；变更需同步主题、实现和 LLMS。 |
| `{x:Static atom:AddOnDecoratedBoxThemeConstants.ContentFramePart}` | template node (AddOnDecoratedBoxContentFrame) | `ButtonSpinnerDecoratedBoxTheme.axaml` | ButtonSpinnerDecoratedBox | `Background`, `BorderBrush`, `ButtonSpinnerLocation`, `Content`, `ContentLeftAddOn`, `ContentLeftAddOnTemplate` | template-stable | 用于主题维护；变更需同步主题、实现和 LLMS。 |
| `ContentLayout` | template node (ButtonSpinnerContentPanel) | `ButtonSpinnerDecoratedBoxTheme.axaml` | ButtonSpinnerDecoratedBox | `ButtonSpinnerLocation`, `Content`, `ContentLeftAddOn`, `ContentLeftAddOnTemplate`, `ContentRightAddOn`, `ContentRightAddOnTemplate` | internal-observable | 用于理解结构和状态流，不应指导用户代码直接依赖。 |
| `{x:Static atom:AddOnDecoratedBoxThemeConstants.ContentLeftAddOnPart}` | template node (AddOnContentPresenter) | `ButtonSpinnerDecoratedBoxTheme.axaml` | ButtonSpinnerDecoratedBox | `ContentLeftAddOn`, `ContentLeftAddOnTemplate` | internal-observable | 用于理解结构和状态流，不应指导用户代码直接依赖。 |
| `{x:Static atom:AddOnDecoratedBoxThemeConstants.ContentRightAddOnPart}` | template node (AddOnContentPresenter) | `ButtonSpinnerDecoratedBoxTheme.axaml` | ButtonSpinnerDecoratedBox | `ContentRightAddOn`, `ContentRightAddOnTemplate` | internal-observable | 用于理解结构和状态流，不应指导用户代码直接依赖。 |
| `{x:Static atom:AddOnDecoratedBoxThemeConstants.ContentPresenterPart}` | template node (ContentPresenter) | `ButtonSpinnerDecoratedBoxTheme.axaml` | ButtonSpinnerDecoratedBox | `Content`, `ContentTemplate` | internal-observable | 用于理解结构和状态流，不应指导用户代码直接依赖。 |
| `PART_SpinnerHandle` | template node (ContentPresenter) | `ButtonSpinnerDecoratedBoxTheme.axaml` | ButtonSpinnerDecoratedBox | `HandleOpacity`, `SpinnerContent` | template-stable | 用于主题维护；变更需同步主题、实现和 LLMS。 |
| `ButtonSpinnerHandle` | control theme | `ButtonSpinnerHandleTheme.axaml` | ButtonSpinner | 主题状态 / visual state | internal-observable | 用于理解结构和状态流，不应指导用户代码直接依赖。 |
| `PART_IncreaseButton` | template node (IconButton) | `ButtonSpinnerHandleTheme.axaml` | ButtonSpinnerHandle | 主题状态 / visual state | template-stable | 用于主题维护；变更需同步主题、实现和 LLMS。 |
| `PART_DecreaseButton` | template node (IconButton) | `ButtonSpinnerHandleTheme.axaml` | ButtonSpinnerHandle | 主题状态 / visual state | template-stable | 用于主题维护；变更需同步主题、实现和 LLMS。 |
| `ButtonSpinner` | control theme | `ButtonSpinnerTheme.axaml` | 用户代码 / 控件宿主 | `ButtonSpinnerLocation`, `CompactSpaceItemPosition`, `CompactSpaceOrientation`, `Content`, `ContentTemplate`, `DataValidationErrors` | public | 用户可直接使用 public 控件；可作为示例和 API 入口。 |
| `PART_DecoratedBox` | template node (ButtonSpinnerDecoratedBox) | `ButtonSpinnerTheme.axaml` | ButtonSpinner | `ButtonSpinnerLocation`, `CompactSpaceItemPosition`, `CompactSpaceOrientation`, `Content`, `ContentTemplate`, `DataValidationErrors` | template-stable | 用于主题维护；变更需同步主题、实现和 LLMS。 |

## Template Parts

| 契约组 | 代表成员 | 维护含义 |
| --- | --- | --- |
| 内容与数据 | `ContentLeftShift`、`ContentPadding`、`ContentRightShift`、`InnerLeftContent`、`InnerLeftContentTemplate`、`InnerRightContent`、`InnerRightContentTemplate`、`LeftAddOnTemplate`、`RightAddOnTemplate`、`SpinnerContent` | 定义控件展示内容、输入数据、模板或业务对象入口。 |
| 交互与状态 | `IsButtonSpinnerFloatable`、`IsButtonSpinnerVisible`、`IsHandleFloatable`、`IsMotionEnabled`、`IsShowHandle`、`IsSpinEnabled`、`Status` | 表达用户可观察状态、可用性、清除、加载或反馈语义。 |
| 视觉与布局 | `HandleOffset`、`SizeType`、`SpinnerBorderThickness`、`SpinnerHandleWidth`、`StyleVariant` | 影响尺寸、位置、颜色、形状、密度和模板视觉变量。 |
| 其他稳定入口 | `ButtonSpinnerLocation`、`HandleOpacity`、`LeftAddOn`、`RightAddOn` | 保留为 public surface，变更前需确认 Gallery 和用户 XAML 依赖。 |

## Pseudo Classes

| 状态反馈 | public API、内部状态和伪类如何形成用户可感知反馈。 | motion、visual option。 |
| 主题语义 | ControlTheme、SharedToken、组件 Token 和模板绑定如何表达视觉。 | ButtonSpinner Token + ControlTheme。 |

## State Flow

ButtonSpinner 的状态流按以下路径收敛：

```text
Public API / inherited command / item source / user input
  -> 控件实例状态
  -> effective state / pseudo-class / template property
  -> ControlTheme selector / presenter / renderer
  -> Gallery 可观察行为
```

状态维护规则：

- Disabled 或不可交互状态优先屏蔽 pointer、keyboard、motion 和提交类反馈。
- motion、visual option 状态由控件实例或明确的数据 owner 推导，不能在 template part 之间双向竞争。
- 模板重套用时必须把 public API 对应状态回放到新的 part、伪类和主题变量。
- 集合、弹层、异步、动效或窗口相关状态必须能处理 reset、close、cancel、detach 和 owner 释放。

## Theme and Token Boundaries

ButtonSpinner 的视觉模型由控件模板、ControlTheme、SharedToken 和必要的组件 Token 共同构成。

| 主题文件 | 职责 |
| --- | --- |
| `ButtonSpinnerDecoratedBoxTheme.axaml` | 定义局部操作入口、按钮或 handle 的状态视觉。 |
| `ButtonSpinnerHandleTheme.axaml` | 定义局部操作入口、按钮或 handle 的状态视觉。 |
| `ButtonSpinnerTheme.axaml` | 定义局部操作入口、按钮或 handle 的状态视觉。 |
| `ButtonSpinnerThemes.axaml` | 聚合控件家族主题资源，保证包级引入顺序稳定。 |

ButtonSpinner 使用 `ButtonSpinnerToken` 作为组件 Token scope。Token 只表达组件视觉语义，不承载 motion、visual option 运行时状态。

主题维护规则：

- 不删除或重命名已经稳定的 ControlTheme key、template part、伪类和资源 key。
- 不把可由 AXAML 表达的模板状态迁移为 C# 动态创建视觉。
- 不把 hover、pressed、selected、expanded、loading、filter、popup open 等运行时状态写入 Token。
- Browser 或平台特化主题必须保持同一 API 的语义一致。

Token 边界：

ButtonSpinner Token 只表达组件级视觉变量，例如尺寸、间距、颜色、圆角、阴影、图标尺寸和弹层边界。Token 不承载运行时选择、展开、加载、错误、上传任务、过滤条件或业务状态。

当前 Token scope：

- `ButtonSpinnerToken`，scope id 为 `ButtonSpinner`，源码位于 `src/AtomUI.Desktop.Controls/ButtonSpinner/ButtonSpinnerToken.cs`。

## Customization Boundaries

维护 ButtonSpinner 时必须保持以下不变量：

- 不擅自新增、删除、重命名或改变 public/protected API、Avalonia 属性、事件和默认值。
- 不破坏 template part、伪类、ControlTheme key、Token 名称和资源 key。
- 不改变 Gallery 已展示的 XAML 用法、默认外观、交互顺序和状态优先级。
- Template part 重新应用、集合替换、弹层关闭、窗口失活和控件 detach 时必须释放旧订阅和资源宿主。
- 不通过隐藏延迟、强制刷新或吞异常掩盖状态同步问题。
- 不引入运行时反射扫描作为 API、Token 或数据路径发现机制。
- 文档只描述当前稳定设计；历史变化记录在 `changelog.md`。

维护不变量：

维护 ButtonSpinner 时不得破坏：

- Public API、默认值、事件顺序和 Gallery 可观察行为。
- Template part 名称、ControlTheme key、伪类和资源 key。
- 旧 template part、事件订阅、Popup/Flyout/Window host 和 collection view 的释放路径。
- Light/Dark、Browser/Desktop 和不同 SizeType 下的主题一致性。
- 控件文档、源码 public surface、Token 类型或生成数据与源码契约的一致性。
