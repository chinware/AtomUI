# AtomUI 边框渲染架构

本文档定义 AtomUI 在不同 DPI、不同 render scale 和不同显示器下绘制边框的架构原则。目标是在普通屏、3.5K、4K 和跨屏窗口移动场景中，让控件边框保持稳定、自然并符合 AtomUI 视觉语义，同时不向使用者暴露额外配置。

## 1. 设计目标

- 用户无感知：业务代码、AXAML 使用方式、控件 API 和 Design Token 语义不改变。
- 保留设计语义：`LineWidth = 1` 仍表示设计系统中的 1 DIP 基础线宽，不被改写成屏幕相关值。
- 稳定物理观感：普通边框在整数 scale 下保持设计 DIP，在非整数 scale 下避免被取整放大成更粗的物理像素线。
- 复用成熟圆角算法：圆角绘制必须继续使用 AtomUI 已引入的 Avalonia / WinUI 派生算法，不手写新的圆角合成逻辑。
- 架构集中：屏幕 scale、layout rounding、hairline 等规则必须集中在共享 helper 或明确语义中，不能散落在各控件渲染代码里。

## 2. 背景结论

AtomUI 把基础线宽定义为普通设计 Token：Seed 层声明 `LineWidth = 1`，Alias 和 Control Token
在需要时继续消费该语义值。Token 层不按分辨率、显示器或 render scale 动态改写线宽。

AtomUI 在设计层保持稳定的 1 DIP 语义，并在绘制层完成物理像素适配。设计层不承担显示器差异，渲染层负责把
设计厚度转换为当前显示器 scale 下稳定的实际绘制厚度。

## 3. 分层模型

边框渲染分为三层。

### 3.1 设计语义层

设计语义层只关心设计系统值：

```text
DesignToken.LineWidth = 1
DesignToken.BorderThickness = new Thickness(LineWidth)
ControlToken / SharedTokenResource
```

这一层不读取显示器 DPI，不读取 `TopLevel.RenderScaling`，不做 `1 / scale` 计算。Token 是主题级语义，而 DPI scale 是 visual root / monitor 级运行时状态。窗口跨屏移动时，scale 可能变化；把运行时 scale 写进 token 会破坏主题语义，也会让不同窗口和不同显示器之间出现错误共享。

### 3.2 渲染线宽层

渲染线宽层负责把设计厚度转换为当前 render scale 下适合绘制的厚度。它不能简单调用 `LayoutHelper.RoundLayoutThickness()`，因为在 Windows 1.5、1.75 等非整数 scale 下，1 DIP 会被取整成 2 个物理像素，使基础边框明显变粗。

AtomUI 普通控件边框采用以下规则：

```text
if UseLayoutRounding and LayoutScale is non-integer:
    RenderThickness = BorderThickness / LayoutScale
else:
    RenderThickness = BorderThickness
```

这样 1 DIP 边框在 1.5 scale 下绘制为 2/3 DIP，最终落到 1 个物理像素；在 macOS Retina 2x 这类整数 scale 下仍保持 1 DIP，因此与网页 1px 边框的观感一致。

该逻辑应集中在内部 helper 中，例如：

```text
BorderUtils.BuildRenderScaleAwareThickness(Layoutable owner, Thickness thickness)
```

helper 的职责只限于 render scale 下的视觉线宽换算，不承担 token 读取、不承担业务控件状态判断。

### 3.3 绘制层

绘制层只接收已经确定的渲染输入：

```text
Bounds.Size
RenderThickness
CornerRadius
BackgroundSizing
Background
BorderBrush
DashStyle / BoxShadow
```

AtomUI 自绘边框控件必须在调用 `BorderRenderHelper` 或构建几何之前，把 `BorderThickness` 转换为 `RenderThickness`。

### 3.4 自绘边框的样式类型

`PixelAlignedBorder` 和 `DashedBorder` 是 AtomUI 自绘控件，不是 Avalonia `Border`。模板样式必须按真实类型选择：

```xml
<Style Selector="^ /template/ atom|PixelAlignedBorder#Frame">
    <Setter Property="BorderBrush" Value="..." />
</Style>
```

禁止通过 `StyleKeyOverride => typeof(Border)` 让自绘控件伪装成 Avalonia `Border`。运行时 selector 匹配使用控件的 `StyleKey`，AXAML 编译器则根据 selector 的目标类型解析未限定的 Setter 属性；如果自绘控件拥有独立注册的 `Background`、`BorderBrush` 等属性，这种伪装会让 `Border#...` selector 匹配成功，但 Setter 写入 Avalonia `Border` 的属性槽，而渲染器读取 AtomUI 自绘边框的属性槽，最终出现样式已匹配但边框或背景没有生效的问题。

派生自绘边框应使用自己的真实类型 selector。不得为了保留旧 `Border#...` selector 添加类型伪装；替换模板节点类型时，必须同时迁移对应 selector 和查找该节点的运行时测试。

## 4. Button 的绘制链路

Button 是边框策略的基准控件。它必须保持用户无感知。

### 4.1 普通 Button

普通 Button 模板使用 AtomUI 自绘边框 primitive。该 primitive 在未设置 `StrokeDashArray` 时绘制实线边框，内部通过 render-scale-aware thickness 保持不同 DPI 下的稳定视觉线宽：

```text
SharedToken.BorderThickness
    ↓
Button.BorderThickness
    ↓
Button.EffectiveBorderThickness
    ↓
ButtonTheme.axaml: DashedBorder#Frame.BorderThickness
    ↓
DashedBorder.RenderThickness
    ↓
BorderRenderHelper.Render(... RenderThickness ...)
```

普通 Button 不在 `Button` 类里手动计算 scale。它的合理边框来自 AtomUI 边框 primitive 内部的 `RenderThickness`，该值由 `BorderUtils.BuildRenderScaleAwareThickness()` 统一计算。

维护要求：

- 不在 `Button.ConfigureEffectiveBorderThickness()` 中做 DPI 或 scale 计算。
- `EffectiveBorderThickness` 继续表达控件状态后的设计厚度，例如 bordered / borderless。
- 普通 Button、DropdownButton 以及 Browser Button 备用主题中的普通 Frame 都应使用同一自绘边框 primitive，避免不同主题下边框厚度策略分裂。

### 4.2 Dashed Button

虚线 Button 模板使用同一个 AtomUI 边框 primitive，并设置 `StrokeDashArray`：

```text
SharedToken.BorderThickness
    ↓
Button.BorderThickness
    ↓
Button.EffectiveBorderThickness
    ↓
ButtonTheme.axaml: DashedBorder#Frame.BorderThickness
    ↓
DashedBorder.RenderThickness
    ↓
BorderRenderHelper.Render(... RenderThickness, StrokeDashArray ...)
```

该 primitive 应具备内部 render thickness 缓存：

- `BorderThickness` 或 `UseLayoutRounding` 改变时使缓存失效。
- 当前 `LayoutHelper.GetLayoutScale(this)` 改变时使缓存失效。
- `Render()` 使用 `RenderThickness`。
- `MeasureOverride()` / `ArrangeOverride()` 继续使用原始 `BorderThickness`，避免渲染线宽换算改变布局尺寸。

这样普通 Button 和 Dashed Button 在同一 scale 下得到一致的视觉厚度，只在 stroke dash 样式上不同。

## 5. 圆角算法边界

圆角是边框绘制中最容易出错的部分。AtomUI 已有成熟实现：

- `src/AtomUI.Controls.Shared/Utils/BorderRenderHelper.cs`
- `src/AtomUI.Controls.Shared/Utils/RoundRectGeometryBuilder.cs`

这些代码已经按 Avalonia / WinUI 思路处理圆角：

- `CornerRadius` 定义在 border stroke 中线。
- `BackgroundSizing.InnerBorderEdge` 使用内边界。
- `BackgroundSizing.OuterBorderEdge` 使用外边界。
- `BackgroundSizing.CenterBorder` 使用 stroke 中线。
- 复杂边框通过 outer rounded geometry 排除 inner rounded geometry 得到真实边框区域。

新的边框一致性机制不得替换该算法。正确做法是只改变传入算法的 `borderThickness`：

```text
旧输入：RoundRectGeometryBuilder(..., BorderThickness, CornerRadius, ...)
新输入：RoundRectGeometryBuilder(..., RenderThickness, CornerRadius, ...)
```

禁止做法：

- 手动 snap 圆角 arc 的点。
- 为每个控件单独手写 `ArcTo` 圆角。
- 把圆角半径按 scale 简单除法处理。
- 对不同控件采用不同的内外圆角合成规则。

## 6. 自绘控件接入规则

凡是控件自己绘制边框、自己创建 `Pen`、自己调用 `BorderRenderHelper` 或自己构建圆角几何，都必须先判断它绘制的是哪一种线。

### 6.1 普通控件边框

普通控件边框包括 Button、Input、GroupBox、OptionButton、TreeView item hover 背景边框、Badge ribbon 背景边框等。这类边框表达控件轮廓，应使用 `RenderScaleAware` 语义：

```text
BorderThickness
    ↓
BorderUtils.BuildRenderScaleAwareThickness
    ↓
BorderRenderHelper / RoundRectGeometryBuilder / Pen
```

### 6.2 发丝线和分割线

Separator、MenuSeparator、DataGrid grid line、TreeView node line、Card action separator 等需要逐个分类。它们不一定是控件轮廓，有些更接近 CSS hairline 或视觉分割线。

这类线不能混用普通边框 helper。应显式选择一种语义：

| 语义 | 使用场景 | 厚度规则 |
|---|---|---|
| `RenderScaleAware` | 控件轮廓、需要保持基础物理像素观感的线 | 非整数 scale 下除以 render scale；整数 scale 下保持设计 DIP |
| `Hairline` | 视觉分割线，希望无论设计 token 如何都保持 1 个物理像素 | 内部 helper 明确计算，不伪装成 token `BorderThickness` |

普通控件边框必须通过 `BorderUtils.BuildRenderScaleAwareThickness()` 进入共享策略，不能在各控件中重复手写 render scale 除法或 `RoundLayoutThickness` 取整逻辑。

## 7. 缓存与失效

自绘边框控件如果缓存 render thickness 或 geometry，必须包含以下失效条件：

- `BorderThickness` 改变。
- `UseLayoutRounding` 改变。
- `LayoutHelper.GetLayoutScale(owner)` 改变。
- `CornerRadius` 改变。
- `BackgroundSizing` 改变。
- `Bounds.Size` 或参与几何的 bounds 改变。

推荐缓存结构：

```text
_renderThickness
_layoutScale
_geometryCacheInitialized
_cachedSize
_cachedRenderThickness
_cachedCornerRadius
_cachedBackgroundSizing
```

控件在 `Render()` 中读取 layout scale 时，必须检测 scale 是否变化。窗口跨显示器移动后，即使 `BorderThickness` 未改变，缓存也应失效。

## 8. 测试与验证

验证 scale 至少覆盖：

```text
1.0
1.25
1.5
1.75
2.0
```

重点场景：

- 普通 Button 和 Dashed Button 在同一 scale 下边框粗细一致。
- 普通屏、3.5K、4K 下同一控件没有明显忽粗忽细。
- 圆角处无断裂、无角部厚薄突变、无背景漏线。
- 窗口跨屏移动后，自绘边框缓存失效并按新 scale 绘制。
- `UseLayoutRounding = false` 时，自绘控件尊重原始 `BorderThickness`。
- GroupBox 这类复杂边框在 Header gap、透明背景和不同圆角下仍稳定。

自动化测试优先级：

- helper 单元测试：验证非整数 scale 下 1 DIP 会转换为 1 个物理像素，整数 scale 下保持设计 DIP。
- `DashedBorder` 渲染输入测试：确认传入 helper 的 thickness 与共享策略一致。
- 控件视觉回归：Button、Dashed Button、GroupBox、OptionButton。
- `git diff --check` 作为文档和代码改动的收尾检查。

## 9. 维护不变量

- Design Token 不读取 DPI / scale。
- `LineWidth = 1` 在 token 层表示 1 DIP 设计线宽；在非整数 scale 的渲染层会转换为 1 个物理像素。
- 普通 Button 使用 AtomUI 自绘边框 primitive，不做额外 DPI 特判。
- 自绘普通边框必须使用 `BorderUtils.BuildRenderScaleAwareThickness()`。
- 圆角绘制继续使用 `RoundRectGeometryBuilder`，不新增平行算法。
- 普通边框与 hairline 必须语义分离。
- scale 变化必须使自绘边框缓存失效。
- 新增自绘边框控件时，必须在实现文档或代码结构中说明其边框语义。
