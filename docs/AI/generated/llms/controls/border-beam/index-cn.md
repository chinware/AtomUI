# BorderBeam

> 生成产物：由源文档生成，不要手工编辑。修改内容请回到控件文档、源码 public surface、Token 类型或生成数据、Gallery ShowCase 或源码结构。

## 概述

BorderBeam 是 AtomUI 桌面其他类控件中的装饰性包装控件，用于在容器边界上绘制持续流动的高光效果。它强化某个容器的视觉关注度，但不表达焦点态、校验态、选中态、错误态、警告态或任何业务状态。

BorderBeam 的职责是围绕一个内容控件绘制一个或多个流光边界，并在不拦截内容交互的前提下提供数量、颜色、渐变、外扩、圆角、线宽和动效控制。它不拥有内容控件的布局语义，不改变内容控件的输入、焦点、命中测试、状态同步、数据绑定或模板结构。

## 包与命名空间

| 项 | 值 |
| --- | --- |
| NuGet 包 | `AtomUI.Desktop.Controls` |
| .NET 命名空间 | `AtomUI.Desktop.Controls` |
| AXAML 命名空间 | `https://atomui.net` |
| Gallery 页面 | `controlgallery/AtomUIGallery/ShowCases/Other/BorderBeam` |
| 状态 | Stable |

## 何时使用

BorderBeam 的设计语言是“非业务状态的动态强调”。它通过沿容器边界运行的高光段形成持续注意力提示，但保持内容本身的控件语义不变。

| 维度 | 含义 | 典型表达 |
| --- | --- | --- |
| 装饰性 | 流光只是视觉强调，不参与业务状态判断。 | `IsHitTestVisible=false` 的渲染层。 |
| 容器边界 | 流光贴合容器有效边框和圆角。 | `IBorderBeamAwareControl` 或显式 `BorderThickness` / `CornerRadius`。 |
| 品牌强调 | 默认颜色来自主题主色。 | `ColorPrimary`、`ColorPrimaryHover`。 |
| 渐变尾迹 | 用户停靠点映射到可见段，尾部保留透明衰减。 | `ColorStops.Percent` 映射到可见段。 |
| 持续流动 | 默认流光不跟随全局动效开关关闭，保持装饰强调一致可见。 | `IsMotionEnabled` 实例开关。 |
| 均匀分布 | 多个光束共享一个周期，并沿同一边界保持等距。 | `Count` 与共享 `Progress`。 |

BorderBeam 不应绘制成一个新的实体边框，也不应让被装饰控件看起来拥有新的可交互状态。流光层应贴合容器边界，透明尾迹应保持连续，圆角转弯处不应出现断裂。

## 公共 API

BorderBeam 的公共 API 采用 Avalonia 属性模型。需要 XAML 设置、Style 设置、绑定、主题参与或动画参与的标量状态定义为 `StyledProperty`。可变集合类状态使用 `DirectProperty` 加实例级集合，避免共享默认集合和构造函数 local value 覆盖外部绑定。

BorderBeam 控件 API：

| API | 类型 | 语义 |
| --- | --- | --- |
| `Content` | `object?` | 被装饰内容，继承自 `ContentControl`。 |
| `Color` | `Color?` | 单色流光配置。 |
| `ColorStops` | `AvaloniaList<BorderBeamColorStop>` | 渐变流光停靠点集合。 |
| `Outset` | `Thickness?` | 流光层相对有效边界的渲染偏移；`null` 时贴合有效边界，不额外改变绘制范围。 |
| `BorderThickness` | `Thickness` | 未命中感知接口时使用的边框厚度，同时决定流光线宽；默认值来自共享边框厚度 Token。 |
| `CornerRadius` | `CornerRadius` | 未命中感知接口时使用的圆角。 |
| `IsMotionEnabled` | `bool` | 控制当前实例的流光动画是否启用；默认值不绑定全局 motion 设置。 |
| `Duration` | `TimeSpan` | 流光运行一周的时长；默认值为 `6s`。 |
| `BeamSize` | `double` | 流光高光段的设备无关像素基准尺寸；默认值为 `100`。 |
| `Count` | `int` | 沿同一边界均匀分布的光束数量；默认值为 `1`，小于 `1` 时按 `1` 渲染。 |

`ColorStops` 非空时优先于 `Color`。`ColorStops` 为空且 `Color` 不为空时，使用单色流光。两者均为空时，使用主题默认渐变。

边界感知接口：

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

稳定 template part：

| Template Part | 类型 | 职责 |
| --- | --- | --- |
| `PART_ContentPresenter` | `ContentPresenter` | 承载被装饰内容。 |
| `PART_BeamPresenter` | internal `BorderBeamPresenter` | 绘制边框流光，不参与命中测试。 |

## 事件与命令

event EventHandler? BorderBeamGeometryChanged;

## 使用示例

稳定示例来源于 Gallery ShowCase 和源码查看片段。生成器只输出可从 `ShowCaseItem` 追溯的示例，不维护第二套手写示例。

以下示例来自 Gallery 源码查看使用的 `ShowCaseItem` 片段，并已按中文资源规范化。

### 基础

来源：`controlgallery/AtomUIGallery/ShowCases/Other/BorderBeam/Views/BorderBeamShowCase.axaml:46`

SourceKey：`border-beam-basic`

```axaml
<atom:BorderBeam Width="360"
                 HorizontalAlignment="Left">
    <atom:Card Header="Workspace overview"
               BorderThickness="1"
               CornerRadius="8">
        <Grid ColumnDefinitions="*,*,*"
              RowDefinitions="Auto,Auto"
              MinHeight="96">
            <atom:TextBlock Grid.Column="0"
                            Text="Users"
                            Foreground="{atom:SharedTokenResource ColorTextTertiary}" />
            <atom:TextBlock Grid.Column="1"
                            Text="Projects"
                            Foreground="{atom:SharedTokenResource ColorTextTertiary}" />
            <atom:TextBlock Grid.Column="2"
                            Text="Tasks"
                            Foreground="{atom:SharedTokenResource ColorTextTertiary}" />
            <atom:TextBlock Grid.Row="1"
                            Grid.Column="0"
                            Text="128"
                            FontSize="22"
                            FontWeight="Bold" />
            <atom:TextBlock Grid.Row="1"
                            Grid.Column="1"
                            Text="24"
                            FontSize="22"
                            FontWeight="Bold" />
            <atom:TextBlock Grid.Row="1"
                            Grid.Column="2"
                            Text="86"
                            FontSize="22"
                            FontWeight="Bold" />
        </Grid>
    </atom:Card>
</atom:BorderBeam>
```

### 悬停显示

来源：`controlgallery/AtomUIGallery/ShowCases/Other/BorderBeam/Views/BorderBeamShowCase.axaml:93`

SourceKey：`border-beam-show-on-hover`

```axaml
<atom:BorderBeam Width="360"
                 Classes="show-on-hover"
                 HorizontalAlignment="Left">
    <atom:Card Header="将指针移到卡片上"
               BorderThickness="1"
               CornerRadius="8">
        <atom:TextBlock Text="当指针移到此卡片上时，边框流光会显示。"
                        TextWrapping="Wrap"
                        Foreground="{atom:SharedTokenResource ColorTextSecondary}" />
    </atom:Card>
</atom:BorderBeam>
```

### 多光束

来源：`controlgallery/AtomUIGallery/ShowCases/Other/BorderBeam/Views/BorderBeamShowCase.axaml:116`

SourceKey：`border-beam-multiple-beams`

```axaml
<StackPanel Orientation="Vertical"
            Spacing="12"
            HorizontalAlignment="Left">
    <atom:BorderBeam Width="360"
                     Count="3"
                     HorizontalAlignment="Left">
        <atom:Card Header="多光束"
                   BorderThickness="1"
                   CornerRadius="8">
            <atom:TextBlock Text="设置 Count 可沿容器边界均匀分布多个光束。"
                            TextWrapping="Wrap"
                            Foreground="{atom:SharedTokenResource ColorTextSecondary}" />
        </atom:Card>
    </atom:BorderBeam>
    <atom:BorderBeam Width="360"
                     Count="2"
                     HorizontalAlignment="Left">
        <atom:Card Header="多光束"
                   BorderThickness="1"
                   CornerRadius="8">
            <atom:TextBlock Text="设置 Count 可沿容器边界均匀分布多个光束。"
                            TextWrapping="Wrap"
                            Foreground="{atom:SharedTokenResource ColorTextSecondary}" />
        </atom:Card>
    </atom:BorderBeam>
</StackPanel>
```

### 自定义容器

来源：`controlgallery/AtomUIGallery/ShowCases/Other/BorderBeam/Views/BorderBeamShowCase.axaml:154`

SourceKey：`border-beam-custom-container`

```axaml
<atom:BorderBeam Width="420"
                 BorderThickness="1"
                 CornerRadius="8"
                 HorizontalAlignment="Left">
    <Border MinHeight="160"
            Padding="24"
            BorderThickness="1"
            CornerRadius="8"
            BorderBrush="{atom:SharedTokenResource ColorBorderSecondary}"
            Background="{atom:SharedTokenResource ColorBgContainer}">
        <atom:TextBlock Text="在一个自定义容器中查看任务状态、部署健康度和最近的自动化活动。"
                        TextWrapping="Wrap"
                        Foreground="{atom:SharedTokenResource ColorTextSecondary}" />
    </Border>
</atom:BorderBeam>
```

## 状态模型

BorderBeam 的行为优先级：

```text
IsVisible=false
> IsMotionEnabled=false
> Content geometry changed
> Progress animation
> Normal render
```

BorderBeam 不拦截鼠标、触控、键盘或焦点。流光 presenter 必须 `IsHitTestVisible=false`，内容控件继续承担自身交互。BorderBeam 不改变内容控件的 `IsEnabled`、`:pointerover`、`:pressed`、`:focus`、`:disabled` 或任何伪类。

effective state 由几何状态、颜色状态、数量状态和动效状态组成：

- 几何状态：优先读取 `IBorderBeamAwareControl`，未命中时使用 BorderBeam 自身 `BorderThickness` 与 `CornerRadius`。
- 颜色状态：`ColorStops` 优先，其次 `Color`，最后使用主题默认渐变。
- 数量状态：`effectiveCount = Math.Max(1, Count)`；`Count` 变化只触发 presenter 重绘，不替换模板或重启动画。
- 动效状态：实例级 `IsMotionEnabled`、可见性和有效尺寸共同决定动画是否运行；默认主题不从全局 `EnableMotion` 覆盖该属性。

`Progress` 是所有光束共享的 internal animation state。第 `i` 个光束使用
`NormalizeProgress(Progress + i / effectiveCount)` 计算相位。`Progress` 不形成公共 API，不参与样式选择器，
不允许外部绑定。

“仅悬停时显示”不是 BorderBeam 的内置状态或公共属性。调用方可以用 Style 在默认状态设置
`IsMotionEnabled=false`，并在 BorderBeam 根控件的 `:pointerover` 状态设置为 `true`；该组合会复用现有动画
启动与停止路径，内容控件仍保留完整交互。

## 主题与 Design Token

BorderBeam 的模板由内容层和装饰层组成，视觉树保持必要最小层级。

```text
BorderBeam
└─ Grid
   ├─ ContentPresenter#PART_ContentPresenter
   └─ BorderBeamPresenter#PART_BeamPresenter
```

视觉层级要求：

- `BorderBeamPresenter` 覆盖内容层边界，在一个 presenter 内绘制 `effectiveCount` 个等距光束，但不得改变内容层测量和排列结果。
- `BorderBeamPresenter` 必须 `IsHitTestVisible=false`。
- 不把 beam 层插入被装饰控件模板内部，不修改 Card、Button、GroupBox 等控件的模板结构。
- 不使用全局 `ScopeAwareAdornerLayer` 作为默认实现。
- 不在内容控件未实现感知接口时尝试读取其 internal 属性或模板 part。

BorderBeam Theme 只负责装配内容层和流光 presenter，并设置默认 token 绑定。几何解析、颜色归一和动画生命周期属于 C# 状态模型。

Token 来源：

BorderBeamToken 是 BorderBeam 的组件级设计变量层。它只承载流光装饰自身需要的默认动效、尺寸和渐变映射参数。颜色、线宽和圆角优先复用 SharedToken；motion 开关与光束数量保留为实例行为，不由 BorderBeamToken 或 `SharedToken.EnableMotion` 决定。

BorderBeamToken 服务以下主题和控件：

- `BorderBeamTheme.axaml`
- internal `BorderBeamPresenter`
- BorderBeam 渐变归一和动画默认值

BorderBeamToken 不承载 `Content`、`Color`、`ColorStops`、`Outset`、`Count`、`Progress`、`EffectiveBorderThickness`、`EffectiveCornerRadius` 等实例状态。这些状态由 BorderBeam 状态模型和边界感知接口处理。

## AOT 与裁剪注意事项

BorderBeam 不使用反射，不访问内容控件 internal 属性或 template part。集成通过 `IBorderBeamAwareControl` 完成。

实例级 motion 未启用时不应启动循环动画。隐藏祖先下的动画必须暂停；动画取消资源必须在 detached、模板替换、
实例 motion 关闭或动画重启时释放。Content 替换只更新 effective geometry，不重启动画。

ColorStops 使用实例级集合，避免共享默认集合。集合变更应触发渐变重建和 presenter 重绘，不应重建整个模板。

`Count=N` 的结构成本为：1 个 presenter、1 个 Animation、1 个 CancellationTokenSource、每帧 1 次路径指标构建和
N 次光束 draw call。额外持久内存保持 `O(1)`，单帧绘制成本为 `O(N)`；实现不创建 phase 集合、每光束 Control、
每光束 animation 或跨帧路径缓存。

## 源码索引

主要源码：

- `src/AtomUI.Desktop.Controls/BorderBeam/BorderBeam.cs`：公共 API、Content 几何感知、effective geometry 和
  `ColorStops` 集合变更通知。
- `src/AtomUI.Desktop.Controls/BorderBeam/BorderBeamPresenter.cs`：internal 渲染层，负责颜色归一、动画生命周期、
  单时钟多相位路径采样以及边框环内的流动高光绘制。
- `src/AtomUI.Desktop.Controls/BorderBeam/IBorderBeamAwareControl.cs`：被装饰控件边界感知接口。
- `src/AtomUI.Desktop.Controls/BorderBeam/BorderBeamGeometry.cs`：边框厚度和圆角几何值。
- `src/AtomUI.Desktop.Controls/BorderBeam/BorderBeamColorStop.cs`：单个渐变停靠点。
- `src/AtomUI.Desktop.Controls/BorderBeam/BorderBeamColorStops.cs`：颜色停靠点集合支持。
- `src/AtomUI.Desktop.Controls/BorderBeam/BorderBeamToken.cs`：BorderBeam 专属 Token。
- `src/AtomUI.Desktop.Controls/BorderBeam/Themes/BorderBeamTheme.axaml`：内容层和 presenter 层装配。

## 相关文档

- 源设计文档：`docs/controls/desktop/other/border-beam/overview.md`
- 实现文档：`docs/controls/desktop/other/border-beam/implementation.md`
- Token 文档：`docs/controls/desktop/other/border-beam/token.md`
- 变更记录：`docs/controls/desktop/other/border-beam/changelog.md`
- 语义结构：`./semantic-cn.md`
