# Badge 语义结构

> 生成产物：由源文档生成，不要手工编辑。修改内容请回到控件文档、源码 public surface、Token 类型或生成数据、Gallery ShowCase 或源码结构。

## Semantic Parts

| Owner | Part | Selector | ContractType | Cardinality | Customization | CrossVisualRoot | RuntimeCreated | 职责 | 稳定性 |
| --- | --- | --- | --- | --- | --- | --- | --- | --- | --- |
| `CountBadge` | `root` | owner 本身 | `CountBadge` | `Single` | `Root` | `false` | `false` | 数量、可见性、颜色、尺寸、定位和目标组合的状态 owner。 | stable since 6.0 |
| `CountBadge` | `indicator` | `.semantic-indicator` | `Control` | `Optional` | `Selector` | `true` | `true` | 完整数量徽标视觉，包括背景、数量文本和统一动效边界。 | stable since 6.0 |
| `DotBadge` | `root` | owner 本身 | `DotBadge` | `Single` | `Root` | `false` | `false` | 状态、文本、颜色、可见性、定位和目标组合的状态 owner。 | stable since 6.0 |
| `DotBadge` | `indicator` | `.semantic-indicator` | `Control` | `Optional` | `Selector` | `true` | `true` | 状态点视觉和统一动效边界，不包含独立模式的说明文本。 | stable since 6.0 |
| `RibbonBadge` | `root` | owner 本身 | `RibbonBadge` | `Single` | `Root` | `false` | `false` | 文本、颜色、位置、可见性和目标组合的状态 owner。 | stable since 6.0 |
| `RibbonBadge` | `indicator` | `.semantic-indicator` | `Control` | `Optional` | `Selector` | `false` | `true` | 完整 Ribbon 视觉、定位和绘制边界。 | stable since 6.0 |
| `RibbonBadge` | `content` | `.semantic-content` | `Avalonia.Controls.TextBlock` | `Optional` | `Selector` | `false` | `true` | Ribbon 文本展示区域。 | stable since 6.0 |

## Abstract AXAML Structure

未定位到可生成抽象 AXAML 结构的 ControlTheme 模板。生成器不会根据 semantic parts 发明 AXAML 节点；请以 Template Parts、主题文件和源码索引为准。

## Composition Model

该章节由控件 `Themes/` 文件夹中的真实主题文件生成，用于说明 public 控件与内部协作对象之间的运行时结构。内部节点只用于理解和维护，不应指导用户代码直接依赖。

### 控件角色图

```text
Badge
  -> CountBadgeAdorner (internal adorner control theme, CountBadgeAdornerTheme.axaml)
     -> MotionActor#PART_MotionActor (template-stable)
        -> Panel#RootLayout (template-stable)
           -> Border#BadgeIndicator (template-stable)
           -> TextBlock#BadgeText (template-stable)
  -> DotBadgeAdorner (internal adorner control theme, DotBadgeAdornerTheme.axaml)
     -> DockPanel#RootLayout (template-stable)
        -> MotionActor#PART_MotionActor (template-stable)
           -> DotBadgeIndicator#Indicator (internal-observable)
        -> Label#Label (template-stable)
     -> DockPanel#RootLayout (template-stable)
        -> MotionActor#PART_MotionActor (template-stable)
           -> DotBadgeIndicator#Indicator (internal-observable)
  -> DotBadgeIndicator (control theme, DotBadgeIndicatorTheme.axaml)
  -> RibbonBadgeAdorner (internal adorner control theme, RibbonBadgeAdornerTheme.axaml)
     -> Panel (template-stable)
        -> TextBlock#PART_LabelPart (template-stable)
```

### 协作节点

| 节点 | 类型 | 来源 | 生命周期 owner | 影响的 public API | 稳定性 | Agent 使用边界 |
| --- | --- | --- | --- | --- | --- | --- |
| `Badge` | public control | `源文档 + public API` | 用户代码 / 控件宿主 | public API | public | 用户可直接使用 public 控件；可作为示例和 API 入口。 |
| `CountBadgeAdorner` | internal adorner control theme | `CountBadgeAdornerTheme.axaml` | Badge | `BadgeColor`, `BoxShadow`, `CountText`, `Foreground` | internal-observable | 用于理解结构和状态流，不应指导用户代码直接依赖。 |
| `PART_MotionActor` | template node (MotionActor) | `CountBadgeAdornerTheme.axaml` | CountBadgeAdorner | `BadgeColor`, `BoxShadow`, `CountText`, `Foreground` | template-stable | 用于主题维护；变更需同步主题、实现和 LLMS。 |
| `RootLayout` | template node (Panel) | `CountBadgeAdornerTheme.axaml` | CountBadgeAdorner | `BadgeColor`, `BoxShadow`, `CountText`, `Foreground` | template-stable | 用于主题维护；变更需同步主题、实现和 LLMS。 |
| `BadgeIndicator` | template node (Border) | `CountBadgeAdornerTheme.axaml` | CountBadgeAdorner | `BadgeColor`, `BoxShadow` | template-stable | 用于主题维护；变更需同步主题、实现和 LLMS。 |
| `BadgeText` | template node (TextBlock) | `CountBadgeAdornerTheme.axaml` | CountBadgeAdorner | `CountText`, `Foreground` | template-stable | 用于主题维护；变更需同步主题、实现和 LLMS。 |
| `DotBadgeAdorner` | internal adorner control theme | `DotBadgeAdornerTheme.axaml` | Badge | `BadgeDotColor`, `IsAdornerMode`, `Text` | internal-observable | 用于理解结构和状态流，不应指导用户代码直接依赖。 |
| `RootLayout` | template node (DockPanel) | `DotBadgeAdornerTheme.axaml` | DotBadgeAdorner | `BadgeDotColor`, `IsAdornerMode`, `Text` | template-stable | 用于主题维护；变更需同步主题、实现和 LLMS。 |
| `PART_MotionActor` | template node (MotionActor) | `DotBadgeAdornerTheme.axaml` | DotBadgeAdorner | `BadgeDotColor` | template-stable | 用于主题维护；变更需同步主题、实现和 LLMS。 |
| `Indicator` | template node (DotBadgeIndicator) | `DotBadgeAdornerTheme.axaml` | DotBadgeAdorner | `BadgeDotColor` | internal-observable | 用于理解结构和状态流，不应指导用户代码直接依赖。 |
| `Label` | template node (Label) | `DotBadgeAdornerTheme.axaml` | DotBadgeAdorner | `IsAdornerMode`, `Text` | template-stable | 用于主题维护；变更需同步主题、实现和 LLMS。 |
| `DotBadgeIndicator` | control theme | `DotBadgeIndicatorTheme.axaml` | Badge | 主题状态 / visual state | internal-observable | 用于理解结构和状态流，不应指导用户代码直接依赖。 |
| `RibbonBadgeAdorner` | internal adorner control theme | `RibbonBadgeAdornerTheme.axaml` | Badge | `Text` | internal-observable | 用于理解结构和状态流，不应指导用户代码直接依赖。 |
| `Panel` | template node (Panel) | `RibbonBadgeAdornerTheme.axaml` | RibbonBadgeAdorner | `Text` | template-stable | 用于主题维护；变更需同步主题、实现和 LLMS。 |
| `PART_LabelPart` | template node (TextBlock) | `RibbonBadgeAdornerTheme.axaml` | RibbonBadgeAdorner | `Text` | template-stable | 用于主题维护；变更需同步主题、实现和 LLMS。 |

## Template Parts

源文档未声明稳定 Template Part。维护模板时应以源码和主题文件中的实际声明为准。

## Pseudo Classes

源文档未声明控件专属伪类。控件仍可能消费 Avalonia 标准状态，例如 `:pointerover`、`:pressed`、`:disabled` 和 focus 相关状态。

## State Flow

Badge 状态从 public owner 单向投影到运行时视觉：

```text
Public API
  -> owner visibility / mode / effective text and color
  -> runtime Adorner properties
  -> ControlTheme / Measure / Arrange / Render
```

主要状态规则：

- `Count=0 && IsZeroVisible=false` 时，CountBadge 将徽标归一为隐藏；`Count > OverflowCount` 时显示 `<OverflowCount>+`。
- DotBadge 有 `DecoratedTarget` 时只显示状态点；独立模式可以同时显示状态点和 `Text`。
- RibbonBadge 隐藏时只移除 Ribbon 视觉，不隐藏 `DecoratedTarget`。
- Count/Dot 启用退出动效时，indicator 可以在隐藏请求后短暂保留；动效完成后才从宿主移除。
- Dot 在 standalone 与 target mode 间切换时会重建内部 Adorner，但公开 `indicator` 身份不变。
- Semantic marker 不表达 visible、status、placement 或 motion phase；节点存在时 marker 保持不变。

| 场景 | root | indicator | content | 说明 |
| --- | --- | --- | --- | --- |
| owner 未附加 | 存在 | 不保证存在 | 不保证存在 | descriptor 可查询，但运行时视觉可以尚未创建。 |
| standalone 且可见 | 存在 | 存在 | Ribbon 存在；Count/Dot 不公开 content | 运行时宿主属于 owner 普通子树。 |
| target mode 且可见 | 存在 | 存在 | Ribbon 存在；Count/Dot 不公开 content | Count/Dot indicator 跨 VisualRoot；Ribbon 保持 inline。 |
| `BadgeIsVisible=false` | 存在 | 最终不存在 | 最终不存在 | 启用动效时 indicator 可以在退出阶段短暂保留。 |
| Count 零值且不显示零 | 存在 | 最终不存在 | 不适用 | `Count` 与 `IsZeroVisible` 共同归一可见性。 |
| Dot standalone/target 切换 | 存在 | 重新建立 | 不适用 | 两种模式使用不同内部模板。 |

## Theme and Token Boundaries

Badge owner 本身没有 ControlTemplate。三个 owner 在运行时创建内部视觉宿主，并把 public 属性单向投影给该宿主。

| Owner | 无 `DecoratedTarget` | 有 `DecoratedTarget` |
| --- | --- | --- |
| `CountBadge` | 数量视觉作为 owner 的普通视觉和逻辑子树。 | 目标作为 owner 子节点；数量视觉显示在 Avalonia `AdornerLayer`。 |
| `DotBadge` | 状态点与可选文本作为 owner 的普通视觉和逻辑子树。 | 目标作为 owner 子节点；状态点显示在 Avalonia `AdornerLayer`。 |
| `RibbonBadge` | Ribbon 视觉作为 owner 的普通视觉和逻辑子树。 | 目标与 Ribbon 都由 owner 在同一 inline visual tree 中排列，不进入原生 `AdornerLayer`。 |

CountBadge 和 DotBadge 的跨 VisualRoot 模式只改变 indicator 的 visual parent。其 logical/style owner 仍必须是对应 Badge owner，使资源、实例 `Styles` 和 owner-scoped selector 保持可达。

Badge 的默认视觉由三个内部 Token scope 与四个 ControlTheme 共同提供：

| 资源 | 职责 |
| --- | --- |
| `CountBadgeToken` | 数量徽标高度、字体、颜色、Padding、圆角和阴影。 |
| `DotBadgeToken` | 状态点尺寸、颜色、阴影和独立文本间距。 |
| `RibbonBadgeToken` | Ribbon 偏移、折角、文本 Padding 和行高。 |
| `CountBadgeAdornerTheme.axaml` | 数量 indicator 的模板、尺寸变体和默认视觉。 |
| `DotBadgeAdornerTheme.axaml` | 状态点、独立文本和 target mode 模板。 |
| `DotBadgeIndicatorTheme.axaml` | 状态点绘制所需的默认属性。 |
| `RibbonBadgeAdornerTheme.axaml` | Ribbon content 模板及绘制参数。 |

Token 只表达组件视觉语义，不保存数量、状态、可见性、目标引用或 motion phase。AtomUI 内置主题不得使用 `.semantic-*` 实现默认视觉。

Token 边界：

Badge Token 只表达组件级视觉变量，例如尺寸、间距、颜色、圆角、阴影、图标尺寸和弹层边界。Token 不承载运行时选择、展开、加载、错误、上传任务、过滤条件或业务状态。

当前 Token scope：

- `BadgeToken`，scope id 为 `Badge`，源码位于 `src/AtomUI.Desktop.Controls/Badge/BadgeToken.cs`。

## Customization Boundaries

- `CountBadge`、`DotBadge`、`RibbonBadge` 的 public API、默认值和 XAML content property 语义不变。
- 三个 owner 分别拥有自己的 descriptor；内部 Adorner 不成为 descriptor owner。
- `root` 不添加 `.semantic-root`；非 root Part 使用唯一 `.semantic-indicator` 或 `.semantic-content`。
- Count/Dot 跨根 indicator 的 visual parent 可以是 `AdornerLayer`，logical/style owner 必须保持对应 Badge owner。
- Ribbon target mode 保持 inline visual tree；隐藏 Ribbon 时必须保留目标内容。
- `DecoratedTarget`、内部文本拆分、动效节点名称和绘制几何不得升级为隐式公共契约。
- 删除、重命名 Part、修改 selector class、收窄 ContractType 或改变 cardinality 按公共主题破坏性变更处理。
- Semantic Part 不引入运行时反射、VisualTree 全局扫描、额外常驻监听或默认路径视觉对象。

维护不变量：

- descriptor owner 只能是 `CountBadge`、`DotBadge`、`RibbonBadge`，不能是 shared abstract base 或 internal Adorner。
- Count/Dot 只有 `root/indicator`；Ribbon 只有 `root/indicator/content`。
- Count/Dot `indicator` marker 位于所有适用 Adorner 模板的 `PART_MotionActor`；Ribbon indicator 位于 runtime Adorner，content 位于 `PART_LabelPart`。
- 所有非 root Part 保持 `Optional + Selector + RuntimeCreated`；Count/Dot indicator 保持 `CrossVisualRoot=true`。
- Badge public owner selector 使用 logical descendant，不使用 `/template/`、类型前缀 class 或 internal 类型。
- Count/Dot target mode 的 visual parent 与 logical/style owner 必须分离，detach 时对称清理。
- Dot standalone 与 target 两套模板必须实现同一个 indicator marker 契约。
- Ribbon 背景与折角继续由 Render 绘制，不为了 Semantic Part 新增视觉节点。
- marker 在节点生命周期内静态存在，不表达 visible、status、placement 或 motion phase。
- `DecoratedTarget`、内部 Label、Count 文本拆分、折角和 motion actor identity 保持非公开。
- 默认 Theme 不消费 semantic class；实现不引入反射、扫描、额外常驻监听或新的默认视觉对象。
