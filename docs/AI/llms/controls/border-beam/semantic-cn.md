# BorderBeam 语义结构

> 生成产物：由源文档生成，不要手工编辑。修改内容请回到控件文档、Gallery API / Token 表、Gallery ShowCase 或源码结构。

## Semantic Parts

| Part | AtomUI 节点 | 职责 | 相关 API | 相关 Token | 稳定性 |
| --- | --- | --- | --- | --- | --- |
| `root` | `BorderBeam` | 控件根语义区域，承载 public API、状态归一、主题入口和 Gallery 可观察行为。 | 见 API 与契约模型 | 见视觉与主题模型 | stable |
| `surface` | `装饰表面` | 承载背景、边框、圆角、遮罩或装饰性效果。 | 见 API 与契约模型 | 见视觉与主题模型 | stable |
| `content` | `内容区域` | 承载被装饰内容或用户内容。 | 见 API 与契约模型 | 见视觉与主题模型 | stable |
| `motion` | `动效区域` | 表达视觉动效、过渡和刷新边界。 | 见 API 与契约模型 | 见视觉与主题模型 | stable |

## Abstract AXAML Structure

来源：`src/AtomUI.Desktop.Controls/BorderBeam/Themes/BorderBeamTheme.axaml`

```xml
<Grid>
    <ContentPresenter Name="PART_ContentPresenter" />
    <BorderBeamPresenter Name="PART_BeamPresenter" />
</Grid>
```

## Composition Model

该章节由控件 `Themes/` 文件夹中的真实主题文件生成，用于说明 public 控件与内部协作对象之间的运行时结构。内部节点只用于理解和维护，不应指导用户代码直接依赖。

### 控件角色图

```text
BorderBeam
  -> BorderBeam (control theme, BorderBeamTheme.axaml)
     -> Grid (template-stable)
        -> ContentPresenter#PART_ContentPresenter (template-stable)
        -> BorderBeamPresenter#PART_BeamPresenter (template-stable)
```

### 协作节点

| 节点 | 类型 | 来源 | 生命周期 owner | 影响的 public API | 稳定性 | Agent 使用边界 |
| --- | --- | --- | --- | --- | --- | --- |
| `BorderBeam` | public control | `源文档 + public API` | 用户代码 / 控件宿主 | public API | public | 用户可直接使用 public 控件；可作为示例和 API 入口。 |
| `BorderBeam` | control theme | `BorderBeamTheme.axaml` | 用户代码 / 控件宿主 | `BeamOpacity`, `BeamSize`, `Color`, `ColorStops`, `Content`, `ContentTemplate` | public | 用户可直接使用 public 控件；可作为示例和 API 入口。 |
| `PART_ContentPresenter` | template node (ContentPresenter) | `BorderBeamTheme.axaml` | BorderBeam | `Content`, `ContentTemplate`, `HorizontalContentAlignment`, `VerticalContentAlignment` | template-stable | 用于主题维护；变更需同步主题、实现和 LLMS。 |
| `PART_BeamPresenter` | template node (BorderBeamPresenter) | `BorderBeamTheme.axaml` | BorderBeam | `BeamOpacity`, `BeamSize`, `Color`, `ColorStops`, `DefaultEndColor`, `DefaultStartColor` | template-stable | 用于主题维护；变更需同步主题、实现和 LLMS。 |

## Template Parts

| Template Part | 类型 | 职责 |
| --- | --- | --- |
| `PART_ContentPresenter` | `ContentPresenter` | 承载被装饰内容。 |
| `PART_BeamPresenter` | internal `BorderBeamPresenter` | 绘制边框流光，不参与命中测试。 |

## Pseudo Classes

BorderBeam 不拦截鼠标、触控、键盘或焦点。流光 presenter 必须 `IsHitTestVisible=false`，内容控件继续承担自身交互。BorderBeam 不改变内容控件的 `IsEnabled`、`:pointerover`、`:pressed`、`:focus`、`:disabled` 或任何伪类。

effective state 由几何状态、颜色状态和动效状态组成：

- 几何状态：优先读取 `IBorderBeamAwareControl`，未命中时使用 BorderBeam 自身 `BorderThickness` 与 `CornerRadius`。
- 颜色状态：`ColorStops` 优先，其次 `Color`，最后使用主题默认渐变。
- 动效状态：`IsMotionEnabled`、可见性和有效尺寸共同决定动画是否运行。

`Progress` 是 internal animation state。它不形成公共 API，不参与样式选择器，不允许外部绑定。

## State Flow

BorderBeam 的行为优先级：

```text
IsVisible=false
> IsMotionEnabled=false
> Content geometry changed
> Progress animation
> Normal render
```

BorderBeam 不拦截鼠标、触控、键盘或焦点。流光 presenter 必须 `IsHitTestVisible=false`，内容控件继续承担自身交互。BorderBeam 不改变内容控件的 `IsEnabled`、`:pointerover`、`:pressed`、`:focus`、`:disabled` 或任何伪类。

effective state 由几何状态、颜色状态和动效状态组成：

- 几何状态：优先读取 `IBorderBeamAwareControl`，未命中时使用 BorderBeam 自身 `BorderThickness` 与 `CornerRadius`。
- 颜色状态：`ColorStops` 优先，其次 `Color`，最后使用主题默认渐变。
- 动效状态：`IsMotionEnabled`、可见性和有效尺寸共同决定动画是否运行。

`Progress` 是 internal animation state。它不形成公共 API，不参与样式选择器，不允许外部绑定。

## Theme and Token Boundaries

BorderBeam 的模板由内容层和装饰层组成，视觉树保持必要最小层级。

```text
BorderBeam
└─ Grid
   ├─ ContentPresenter#PART_ContentPresenter
   └─ BorderBeamPresenter#PART_BeamPresenter
```

视觉层级要求：

- `BorderBeamPresenter` 覆盖内容层边界，但不得改变内容层测量和排列结果。
- `BorderBeamPresenter` 必须 `IsHitTestVisible=false`。
- 不把 beam 层插入被装饰控件模板内部，不修改 Card、Button、GroupBox 等控件的模板结构。
- 不使用全局 `ScopeAwareAdornerLayer` 作为默认实现。
- 不在内容控件未实现感知接口时尝试读取其 internal 属性或模板 part。

BorderBeam Theme 只负责装配内容层和流光 presenter，并设置默认 token 绑定。几何解析、颜色归一和动画生命周期属于 C# 状态模型。

Token 边界：

BorderBeamToken 是 BorderBeam 的组件级设计变量层。它只承载流光装饰自身需要的默认动效、尺寸和渐变映射参数。颜色、线宽、圆角和 motion 开关优先复用 SharedToken，不在 BorderBeamToken 中重复定义全局语义。

BorderBeamToken 服务以下主题和控件：

- `BorderBeamTheme.axaml`
- internal `BorderBeamPresenter`
- BorderBeam 渐变归一和动画默认值

BorderBeamToken 不承载 `Content`、`Color`、`ColorStops`、`Outset`、`Progress`、`EffectiveBorderThickness`、`EffectiveCornerRadius` 等实例状态。这些状态由 BorderBeam 状态模型和边界感知接口处理。

## Customization Boundaries

BorderBeam 设计和实现必须保持以下不变量：

- BorderBeam 是装饰控件，不改变内容控件的公共 API、主题契约、伪类、事件或方法。
- BorderBeam 不替代焦点态、校验态、选中态、错误态或警告态。
- BorderBeam 不拦截内容控件输入事件。
- BorderBeam 不通过反射读取内容控件 internal 属性。
- BorderBeam 不修改内容控件模板，不依赖内容控件 template part 名称。
- `IBorderBeamAwareControl` 只暴露边界几何，不暴露业务状态。
- `ColorStops.Percent` 的 public 输入范围固定为 `0~100`。
- 动效禁用时不应持续产生 UI 线程动画或 render invalidation。
- 颜色和线宽默认值必须跟随主题 token，支持 light / dark 主题切换。

如果实现某项能力时无法保持这些不变量，应先停止实现，说明原因、影响范围、替代方案和迁移方式，并获得授权。

维护不变量：

内部重构必须保持以下不变量：

- BorderBeam 包装内容，不修改内容模板。
- beam presenter 不参与命中测试。
- 感知接口只暴露边框厚度和圆角。
- content 替换时旧事件订阅必须释放。
- motion 关闭或 detached 后不持续 invalidation。
- 渐变尾迹连续，圆角转弯处不分段卡顿。
- 非统一圆角只影响边框环裁剪，不直接拆分运动路径。
- public `ColorStops.Percent` 仍按 `0~100` 解释。
