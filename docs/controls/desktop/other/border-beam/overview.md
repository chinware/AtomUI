# BorderBeam 桌面版架构设计

本文档定义 `AtomUI.Desktop.Controls.BorderBeam` 桌面版的最新架构设计、装饰边界模型、API 模型、模板结构、主题边界和验证策略。通用控件研发约束见 [控件研发标准](../../../../engineering/control-development-guidelines.md)，BorderBeam Token 的专项设计见 [BorderBeam Token 设计](token.md)，设计和契约变化记录见 [BorderBeam Changelog](changelog.md)。

Ant Design `BorderBeam` 的核心语义是为容器边框提供持续流动的装饰性高亮效果。AtomUI 的 BorderBeam 以该设计语义为基准，但按 Avalonia 控件模型实现为独立包装控件，并通过显式边界感知接口获取被装饰控件的有效边框和圆角。

## 1. 控件定位

BorderBeam 是 AtomUI 桌面其他类控件中的装饰性包装控件，用于在容器边界上绘制持续流动的高光效果。它强化某个容器的视觉关注度，但不表达焦点态、校验态、选中态、错误态、警告态或任何业务状态。

BorderBeam 的职责是围绕一个内容控件绘制流光边界，并在不拦截内容交互的前提下提供颜色、渐变、外扩、圆角、线宽和动效控制。它不拥有内容控件的布局语义，不改变内容控件的输入、焦点、命中测试、状态同步、数据绑定或模板结构。

BorderBeam 适用于登录面板、推荐卡片、AI 模块、重点 CTA 区域、工作台概览卡片等需要强调但不需要业务状态语义的场景。需要表达选择、校验、焦点、警告或错误时，应使用对应控件自身状态、Form 状态或反馈类控件，不应以 BorderBeam 替代。

## 2. 设计语言

BorderBeam 的设计语言是“非业务状态的动态强调”。它通过沿容器边界运行的高光段形成持续注意力提示，但保持内容本身的控件语义不变。

| 维度 | 含义 | 典型表达 |
| --- | --- | --- |
| 装饰性 | 流光只是视觉强调，不参与业务状态判断。 | `IsHitTestVisible=false` 的渲染层。 |
| 容器边界 | 流光贴合容器有效边框和圆角。 | `IBorderBeamAwareControl` 或显式 `BorderThickness` / `CornerRadius`。 |
| 品牌强调 | 默认颜色来自主题主色。 | `ColorPrimary`、`ColorPrimaryHover`。 |
| 渐变尾迹 | 用户停靠点映射到可见段，尾部保留透明衰减。 | `ColorStops.Percent` 映射到 `0~70`。 |
| 动效可控 | 装饰效果遵守全局动效开关。 | `IsMotionEnabled`、`EnableMotion`。 |

BorderBeam 不应绘制成一个新的实体边框，也不应让被装饰控件看起来拥有新的可交互状态。流光层应贴合容器边界，透明尾迹应保持连续，圆角转弯处不应出现断裂。

## 3. 架构分层

BorderBeam 架构按装饰职责分层，避免公共 API、被装饰控件状态、渲染路径和主题 Token 相互耦合。

| 层 | 责任 | 主要载体 |
| --- | --- | --- |
| Public API | 暴露内容、颜色、渐变、外扩、圆角、边框厚度、动效和持续时间。 | `BorderBeam` |
| Geometry Awareness | 从实现感知接口的内容控件读取有效边框和圆角。 | `IBorderBeamAwareControl`、`BorderBeamGeometry` |
| Effective State | 归一颜色、渐变停靠点、边界几何、外扩、动效状态和动画进度。 | `BorderBeam` / `BorderBeamPresenter` |
| Rendering | 在边框环区域绘制流动高光段，不拦截输入。 | internal `BorderBeamPresenter` |
| Template Contract | 装配内容 presenter 和流光 presenter。 | `BorderBeamTheme.axaml` |
| Theme Mapping | 将 SharedToken 与 BorderBeamToken 映射为默认颜色、线宽、圆角和动效参数。 | `BorderBeamTheme.axaml`、`BorderBeamToken` |
| Integration | 与 Card、Button、GroupBox、输入壳体等可感知控件协同。 | `IBorderBeamAwareControl` 实现 |

状态流：

```text
Public API
  Content / Color / ColorStops / Outset
  BorderThickness / CornerRadius
  IsMotionEnabled / Duration / BeamSize
        ↓
Geometry Awareness
  Content implements IBorderBeamAwareControl
    → BorderBeamGeometry
  otherwise
    → BorderBeam explicit geometry
        ↓
Effective State
  effective border thickness
  effective corner radius
  effective gradient
  effective outset
  animation progress
        ↓
BorderBeamPresenter
  border ring geometry
  beam path position
  gradient tail
        ↓
Template Visual
  content presenter + non-hit-test beam layer
```

## 4. API 设计

BorderBeam 的公共 API 采用 Avalonia 属性模型。凡是需要 XAML 设置、Style 设置、绑定、主题参与或动画参与的标量状态，都定义为 `StyledProperty`。可变集合类状态使用 `DirectProperty` 加实例级集合，避免共享默认集合和构造函数 local value 覆盖外部绑定。

### 4.1 BorderBeam 控件 API

```csharp
public class BorderBeam : ContentControl, IMotionAwareControl
{
    public static readonly StyledProperty<Color?> ColorProperty;
    public static readonly DirectProperty<BorderBeam, AvaloniaList<BorderBeamColorStop>> ColorStopsProperty;
    public static readonly StyledProperty<Thickness?> OutsetProperty;
    public static readonly StyledProperty<Thickness> BorderThicknessProperty;
    public static readonly StyledProperty<CornerRadius> CornerRadiusProperty;
    public static readonly StyledProperty<bool> IsMotionEnabledProperty;
    public static readonly StyledProperty<TimeSpan> DurationProperty;
    public static readonly StyledProperty<double> BeamSizeProperty;
}
```

| API | 类型 | 语义 |
| --- | --- | --- |
| `Content` | `object?` | 被装饰内容，继承自 `ContentControl`。 |
| `Color` | `Color?` | 单色流光配置。 |
| `ColorStops` | `AvaloniaList<BorderBeamColorStop>` | 渐变流光停靠点集合，使用 `DirectProperty` 暴露实例级可变集合。 |
| `Outset` | `Thickness?` | 流光层相对有效边界的外扩距离；`null` 时按有效边框厚度计算。 |
| `BorderThickness` | `Thickness` | 未命中感知接口时使用的边框厚度。 |
| `CornerRadius` | `CornerRadius` | 未命中感知接口时使用的圆角。 |
| `IsMotionEnabled` | `bool` | 控制流光动画是否启用，默认绑定 `SharedToken.EnableMotion`。 |
| `Duration` | `TimeSpan` | 流光运行一周的时长，默认 `6s`。 |
| `BeamSize` | `double` | 流光高光段基准尺寸，默认 `100`。 |

`ColorStops` 非空时优先于 `Color`。`ColorStops` 为空且 `Color` 不为空时，使用单色流光。两者均为空时，使用主题默认渐变。

### 4.2 BorderBeamColorStop

```csharp
public class BorderBeamColorStop : AvaloniaObject
{
    public static readonly StyledProperty<Color> ColorProperty;
    public static readonly StyledProperty<double> PercentProperty;
}
```

`Percent` 是用户输入区间，取值按 `0~100` 解释。BorderBeam 内部将该值映射到可见流光段的 `0~70` 区间，为尾部透明渐隐保留空间。该模型对齐 Ant Design `BorderBeam` 的 percent 语义。

### 4.3 边界感知接口

```csharp
public interface IBorderBeamAwareControl
{
    BorderBeamGeometry GetBorderBeamGeometry();
    event EventHandler? BorderBeamGeometryChanged;
}

public readonly record struct BorderBeamGeometry(
    Thickness BorderThickness,
    CornerRadius CornerRadius);
```

`IBorderBeamAwareControl` 是被装饰控件向 BorderBeam 暴露有效边界几何的显式契约。它只描述可用于流光贴合的边框厚度和圆角，不暴露控件状态、不暴露模板节点、不要求 BorderBeam 理解被装饰控件的内部实现。

边界来源优先级：

1. `Content` 实现 `IBorderBeamAwareControl` 时，使用 `GetBorderBeamGeometry()` 返回值。
2. `Content` 未实现接口时，使用 `BorderBeam.BorderThickness` 和 `BorderBeam.CornerRadius`。
3. 未显式设置时，使用 Theme 默认值：`SharedToken.BorderThickness` 和 `SharedToken.BorderRadiusLG`。

实现 `IBorderBeamAwareControl` 的控件应在有效边框或有效圆角实际变化后触发 `BorderBeamGeometryChanged`。BorderBeam 不通过反射读取 `EffectiveBorderThickness`、`EffectiveCornerRadius` 或模板 part。

## 5. 行为交互模型

BorderBeam 的行为优先级：

```text
IsVisible=false
> IsMotionEnabled=false
> Content geometry changed
> Progress animation
> Normal render
```

BorderBeam 不应拦截鼠标、触控、键盘或焦点。流光 presenter 必须 `IsHitTestVisible=false`，内容控件继续承担自身交互。BorderBeam 不改变内容控件的 `IsEnabled`、`:pointerover`、`:pressed`、`:focus`、`:disabled` 或任何伪类。

`IsMotionEnabled=false` 时，BorderBeam 不启动循环动画，不持续触发 `InvalidateVisual`。装饰层可以保持静态首帧，也可以隐藏流光效果；桌面版设计采用隐藏流光效果，以对齐 Ant Design 在 reduced motion 下隐藏 beam 的语义。

当内容控件通过 `IBorderBeamAwareControl.BorderBeamGeometryChanged` 通知几何变化时，BorderBeam 重新计算有效边界并刷新渲染。该刷新只影响 BorderBeam 自身视觉，不要求内容控件重新模板化。

BorderBeam 不提供点击、命令、选择、关闭、展开或业务事件。它的状态变化仅影响装饰视觉。

## 6. 状态模型

BorderBeam 的 effective state 由几何状态、颜色状态和动效状态组成。

几何状态：

```text
Content is IBorderBeamAwareControl
  → effective border thickness / corner radius from content
else
  → effective border thickness / corner radius from BorderBeam properties

Outset != null
  → effective outset = Outset
else
  → effective outset = effective border thickness outward
```

颜色状态：

```text
ColorStops.Count > 0
  → normalize stops
else if Color != null
  → [Color 0%, Color 100%]
else
  → [ColorPrimary 0%, ColorPrimaryHover 100%]

normalized percent = clamp(percent, 0, 100) / 100 * MaxVisibleStopPercent
tail = transparent fade-out
```

动效状态：

```text
IsMotionEnabled && IsVisible && Bounds has positive size
  → run progress animation 0..1 repeatedly
otherwise
  → stop animation and release cancellation source
```

`Progress` 是 internal animation state。它不形成公共 API，不参与样式选择器，不允许外部绑定。`Progress` 变化只触发 `BorderBeamPresenter` 渲染。

## 7. 模板与视觉架构

BorderBeam 的模板由内容层和装饰层组成，视觉树保持必要最小层级。

稳定模板节点：

| Template Part | 类型 | 职责 |
| --- | --- | --- |
| `PART_ContentPresenter` | `ContentPresenter` | 承载被装饰内容。 |
| `PART_BeamPresenter` | internal `BorderBeamPresenter` | 绘制边框流光，不参与命中测试。 |

默认视觉树：

```text
BorderBeam
└─ Grid
   ├─ ContentPresenter#PART_ContentPresenter
   └─ BorderBeamPresenter#PART_BeamPresenter
```

视觉层级要求：

- `BorderBeamPresenter` 必须覆盖内容层边界，但不得改变内容层测量和排列结果。
- `BorderBeamPresenter` 必须 `IsHitTestVisible=false`。
- 不把 beam 层插入被装饰控件模板内部，不修改 Card、Button、GroupBox 等控件的模板结构。
- 不使用全局 `ScopeAwareAdornerLayer` 作为默认实现，避免与 Badge、Watermark、Drawer 等全局装饰层竞争。
- 不在内容控件未实现感知接口时尝试读取其 internal 属性或模板 part。

BorderBeam 使用包装控件模型而不是 attached property 模型。attached property 容易引入装饰层归属、生命周期和与其他 adorner 互斥的问题；包装模型使装饰的存在、范围和生命周期在 XAML 中明确可见。

## 8. Theme 架构

BorderBeam Theme 位于 `src/AtomUI.Desktop.Controls/BorderBeam/Themes/BorderBeamTheme.axaml`。主题只负责装配内容层和流光 presenter，并设置默认 token 绑定。

Theme 职责：

- 装配 `PART_ContentPresenter` 和 `PART_BeamPresenter`。
- 将 `BorderThickness` 默认绑定到 `SharedToken.BorderThickness`。
- 将 `CornerRadius` 默认绑定到 `SharedToken.BorderRadiusLG`。
- 将 `IsMotionEnabled` 默认绑定到 `SharedToken.EnableMotion`。
- 将 `Duration`、`BeamSize`、`MaxVisibleStopPercent` 等默认值绑定到 BorderBeamToken。
- 保证 beam presenter 不参与命中测试。

Theme 不负责推断内容控件几何，不负责生成渐变停靠点，不负责启动或停止动画。几何解析、颜色归一和动画生命周期属于 C# 状态模型。

## 9. 控件家族或集成关系

BorderBeam 与 AtomUI 控件家族的集成通过 `IBorderBeamAwareControl` 完成。

适配对象应满足两个条件：

- 控件已经拥有稳定的有效边框和有效圆角状态。
- 控件边界是用户自然认为的视觉容器边界。

典型适配对象包括：

| 控件 | 适配理由 |
| --- | --- |
| `Card` | Card 是 BorderBeam 最常见的容器场景，拥有明确边框和圆角。 |
| `Button` | Button 已维护 `EffectiveBorderThickness` 与 `EffectiveCornerRadius`，适合用于强调 CTA。 |
| `GroupBox` | GroupBox 有标题分段边框，需要由自身明确返回可用于流光的有效边界。 |
| 输入壳体类控件 | TextBox、LineEdit、NumericUpDown 等拥有输入边框和圆角语义。 |

未实现 `IBorderBeamAwareControl` 的控件仍可被 BorderBeam 包裹，但需要用户通过 `BorderThickness` 和 `CornerRadius` 显式对齐。BorderBeam 不对任意控件做类型判断。

## 10. 兼容性不变量

BorderBeam 设计和实现必须保持以下不变量：

- BorderBeam 是装饰控件，不改变内容控件的公共 API、主题契约、伪类、事件或方法。
- BorderBeam 不替代焦点态、校验态、选中态、错误态或警告态。
- BorderBeam 不拦截内容控件输入事件。
- BorderBeam 不通过反射读取内容控件 internal 属性。
- BorderBeam 不修改内容控件模板，不依赖内容控件 template part 名称。
- `IBorderBeamAwareControl` 只暴露边界几何，不暴露业务状态。
- `ColorStops.Percent` 的 public 输入范围固定为 `0~100`，内部映射保留尾部透明区。
- 动效禁用时不应持续产生 UI 线程动画或 render invalidation。
- 颜色和线宽默认值必须跟随主题 token，支持 light / dark 主题切换。

## 11. 专项模型

### 11.1 边界感知模型

BorderBeam 的核心专项模型是边界感知。该模型将“控件自己的有效边框和圆角”从控件内部状态提升为外部装饰器可读取的窄契约。

控件实现 `IBorderBeamAwareControl` 时，应返回已经考虑自身状态后的最终几何。例如 Button 应返回已经考虑 `ButtonType`、`Variant`、`Shape`、`CompactSpace` 后的有效边框和圆角。BorderBeam 不再重复这些判断。

接口事件只在返回值实际变化时触发，避免每次普通属性变化都刷新流光层。

### 11.2 渐变停靠点模型

BorderBeam 渐变停靠点以用户友好的 `0~100` percent 表达。内部渲染时将这些停靠点映射到可见高光段的前 `70%`，剩余区域用于透明尾迹。

该模型保证用户描述的是完整高光段上的颜色分布，而不是直接控制最终 brush 的绝对百分比。`Percent=30` 表示约三成位置的颜色节点，映射后仍处于可见段前部，而不是在保留尾迹后被硬截断。

### 11.3 动效与 reduced motion 模型

BorderBeam 是装饰性动效。`IsMotionEnabled=false` 时，流光效果隐藏，内容保持完全可用。全局主题关闭 motion 时，默认 `IsMotionEnabled` 也应随 `SharedToken.EnableMotion` 关闭。

动画使用 internal `Progress` 从 `0` 到 `1` 循环，周期由 `Duration` 控制。动画停止时必须取消并释放内部 `CancellationTokenSource` 或等价资源。

### 11.4 渲染连续性模型

BorderBeamPresenter 应以一个连续圆角矩形运动路径驱动流光。边框环裁剪必须跟随被装饰容器的有效边框和圆角；运动路径的转角半径则按 `BeamSize` 建立平滑圆角，避免高光段在容器真实小圆角处发生可见面积突变。

高光段本身按路径切线旋转，并使用与 Ant Design `offsetAnchor: 90% 50%` 等价的锚点模型：流光矩形局部坐标的 `90% / 50%` 位置落在运动路径上。该模型让光束提前进入转角并保持尾迹连续，不应分成四条互不衔接的直线分别绘制。

非统一圆角只影响边框环裁剪，不直接决定运动路径半径。圆角半径应按 Avalonia 圆角矩形规则夹取，避免半径超过控件尺寸时产生自交路径。

## 12. 验证策略

文档验证：

- `overview.md`、`token.md`、`changelog.md` 互相链接有效。
- 分类入口 `docs/controls/desktop/other/overview.md` 链接 BorderBeam 文档。
- 桌面控件入口包含 `other` 分类。
- 文档不包含路线图式措辞。

C# 状态验证：

- `BorderBeam` 暴露的 StyledProperty 与 CLR wrapper 顺序符合控件研发标准。
- `IBorderBeamAwareControl` 不依赖反射和 template part。
- `BorderBeamGeometryChanged` 只在有效边界变化时触发。
- `IsMotionEnabled=false` 时不启动循环动画。
- detach / re-template 时取消动画并解除对内容控件事件订阅。

渲染验证：

- 默认渐变颜色来自当前主题主色。
- 单色 `Color` 映射为可见段单色 + 透明尾迹。
- 多 stop 渐变按 `0~100` 输入区间映射到 `0~70` 可见区间。
- 统一圆角和非统一圆角下流光转角连续。
- `Outset=0` 时流光不外扩，适用于裁剪容器。
- beam presenter 不拦截内容点击、焦点和键盘事件。

Token 验证：

- Light / dark 主题下默认颜色、线宽、圆角和 motion 默认值正确。
- 修改 BorderBeamToken 后生成的 `BorderBeamTokenKind` 和 AXAML 引用同步。
- 不把实例状态写入 Token。

Public API 验证：

- `Content` 为普通控件时使用 BorderBeam 自身几何。
- `Content` 实现 `IBorderBeamAwareControl` 时使用控件返回几何。
- `ColorStops` 优先级高于 `Color`。
- 无内容、零尺寸、禁用 motion、不可见状态不抛异常。
