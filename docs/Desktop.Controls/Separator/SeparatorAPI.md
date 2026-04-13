# Separator API 参考

## 命名空间

```csharp
namespace AtomUI.Desktop.Controls;       // Separator, VerticalSeparator
namespace AtomUI.Controls.Commons;        // AbstractSeparator（设备无关基类）
namespace AtomUI.Controls;                // SeparatorTitlePosition, SeparatorVariant, SeparatorPseudoClass, SizeType
```

AXAML 命名空间前缀：`atom`（`xmlns:atom="https://atomui.net"`）

---

## 枚举类型

### SeparatorTitlePosition

标题位置枚举，定义标题在水平分割线中的位置。

| 值 | 说明 |
|---|---|
| `Left` | 标题靠左，左侧为短线条（或无线条，取决于 `OrientationMargin`） |
| `Right` | 标题靠右，右侧为短线条 |
| `Center` | 标题居中（默认），两侧等长线条 |

### SeparatorVariant

线条样式变体枚举。

| 值 | 说明 |
|---|---|
| `Solid` | 实线（默认），连续直线 |
| `Dotted` | 点线，`[1, 1]` 间隔 |
| `Dashed` | 虚线，`[4, 2]` 间隔 |

### SizeType（来自 `AtomUI.Controls`）

尺寸类型枚举。

| 值 | 说明 |
|---|---|
| `Small` | 小号 |
| `Middle` | 中号（默认） |
| `Large` | 大号 |

---

## 公共属性（StyledProperty）

以下属性定义在 `AbstractSeparator`（设备无关基类），`Separator` 和 `VerticalSeparator` 完整继承。

| 属性名 | 类型 | 默认值 | 说明 |
|---|---|---|---|
| `Title` | `string?` | `null` | 分割线标题文本（内容属性 `[Content]`，仅水平方向有效）。设置后自动激活 `:has-title` 伪类 |
| `TitlePosition` | `SeparatorTitlePosition` | `Center` | 标题在线条中的位置：`Left`（靠左）、`Center`（居中）、`Right`（靠右） |
| `TitleColor` | `IBrush?` | `ColorTextHeading` | 标题文本颜色。未设置时由主题通过 `SharedToken.ColorTextHeading` 控制 |
| `LineColor` | `IBrush?` | `ColorSplit` | 分割线颜色。未设置时由主题通过 `SharedToken.ColorSplit` 控制 |
| `Orientation` | `Orientation` | `Horizontal` | 分割线方向。`Horizontal`：水平全宽线条；`Vertical`：垂直短线条 |
| `Variant` | `SeparatorVariant` | `Solid` | 线条样式变体：`Solid`（实线）、`Dashed`（虚线）、`Dotted`（点线） |
| `OrientationMargin` | `double` | `NaN` | 标题距左/右边缘的精确像素值。设为 `NaN`（默认）时使用 `OrientationMarginPercent` 百分比计算 |
| `LineWidth` | `double` | `1` | 线条宽度（像素），由主题通过 `SharedToken.LineWidth` 控制 |
| `IsPlain` | `bool` | `false` | 标题是否使用普通正文样式。`false` 时使用大字号加粗，`true` 时使用正常字号字重 |
| `SizeType` | `SizeType` | `Middle` | 分割线垂直间距尺寸（共享属性，通过 `AddOwner` 注册）。仅影响水平分割线的上下 Margin |

### 继承自 Avalonia.Controls.Separator 的常用属性

| 属性名 | 类型 | 默认值 | 说明 |
|---|---|---|---|
| `Margin` | `Thickness` | 由主题控制 | 外间距，水平分割线的上下间距由 `SizeType` 对应的 Token 自动设置 |
| `IsEnabled` | `bool` | `true` | 是否启用 |
| `Background` | `IBrush?` | 由主题控制 | 背景色 |
| `Foreground` | `IBrush?` | 由主题控制 | 前景色 |
| `FontSize` | `double` | 由主题控制 | 标题文字大小，`IsPlain=False` 时为 `FontSizeLG`，`IsPlain=True` 时为 `FontSize` |
| `FontWeight` | `FontWeight` | 由主题控制 | 标题文字粗细，`IsPlain=False` 时为 500（Medium），`IsPlain=True` 时为 Normal |
| `FontStyle` | `FontStyle` | `Normal` | 标题文字样式（Normal / Italic / Oblique） |
| `HorizontalAlignment` | `HorizontalAlignment` | 由主题控制 | 水平对齐。水平分割线主题设为 `Stretch`，垂直分割线设为 `Center` |
| `VerticalAlignment` | `VerticalAlignment` | 由主题控制 | 垂直对齐。两种方向均由主题设为 `Center` |

---

## 内部属性（由主题/Token 驱动）

以下属性为 `internal`，由 ControlTheme 通过 Token 资源绑定设置，开发者无需手动操作：

| 属性名 | 类型 | 来源 | 说明 |
|---|---|---|---|
| `TextPaddingInline` | `double` | `SeparatorToken.TextPaddingInline` | 标题文本两侧的横向内间距（单位 em），默认 1.0 |
| `OrientationMarginPercent` | `double` | `SeparatorToken.OrientationMarginPercent` | 标题与边缘距离的百分比（0～1），默认 0.05 |
| `VerticalMarginInline` | `double` | `SeparatorToken.VerticalMarginInline` | 垂直分割线的横向外间距 |

---

## 伪类（Pseudo-Classes）

Separator 支持以下伪类，可在样式选择器中使用：

| 伪类 | 常量 | 触发条件 |
|---|---|---|
| `:has-title` | `SeparatorPseudoClass.HasTitleText` | `Title` 属性不为 `null` 且不为空字符串 |

### 继承的标准伪类

| 伪类 | 触发条件 |
|---|---|
| `:disabled` | `IsEnabled == false` |

---

## 实现的接口

### ISizeTypeAware

```csharp
public SizeType SizeType { get; set; }
```

支持 `SizeType`（Small / Middle / Large）三种尺寸，控制水平分割线的垂直间距。通过共享属性 `SizeTypeControlProperty.SizeTypeProperty.AddOwner<AbstractSeparator>()` 注册。

---

## 类层次结构

### AbstractSeparator（设备无关基类）

```csharp
namespace AtomUI.Controls.Commons;

[PseudoClasses(SeparatorPseudoClass.HasTitleText)]
public abstract class AbstractSeparator : Avalonia.Controls.Separator, ISizeTypeAware
```

**职责**：定义所有平台共享的属性、布局逻辑（`MeasureOverride` / `ArrangeOverride`）和自绘渲染（`Render`）。包含标题定位算法、线条绘制、DashStyle/DotStyle 静态实例等核心逻辑。

**关键常量**：
- `SEPARATOR_LINE_MIN_PROPORTION = 0.25` — 标题区域保证至少 25% 的线条比例

### Separator（桌面实现）

```csharp
namespace AtomUI.Desktop.Controls;

public class Separator : AbstractSeparator
{
    public Separator()
    {
        this.RegisterTokenResourceScope(SeparatorToken.ScopeProvider);
    }
}
```

**职责**：注册桌面端 `SeparatorToken` 的 Token 作用域，使主题能通过 Token 资源键控制视觉属性。

### VerticalSeparator（垂直分割线便捷类）

```csharp
namespace AtomUI.Desktop.Controls;

public class VerticalSeparator : Separator
{
    static VerticalSeparator()
    {
        OrientationProperty.OverrideDefaultValue<VerticalSeparator>(Orientation.Vertical);
    }
    protected override Type StyleKeyOverride => typeof(Separator);
}
```

**职责**：将 `Orientation` 默认值覆盖为 `Vertical`，使用时无需手动设置方向。通过 `StyleKeyOverride` 共享 `Separator` 的 ControlTheme。

```xml
<!-- 两种写法等价 -->
<atom:Separator Orientation="Vertical" />
<atom:VerticalSeparator />
```

---

## 模板部件

| 部件名 | 控件类型 | 说明 |
|---|---|---|
| `PART_Title` | `TextBlock` | 标题文本，仅在 `Orientation=Horizontal` 且 `Title` 不为空时可见。通过 `ArrangeOverride` 手动定位 |

---

## 静态成员

| 成员 | 类型 | 说明 |
|---|---|---|
| `AbstractSeparator.DashStyle` | `IDashStyle` | 虚线样式（`[4, 2]`，offset=0），`Variant=Dashed` 时使用 |
| `AbstractSeparator.DotStyle` | `IDashStyle` | 点线样式（`[1, 1]`，offset=0），`Variant=Dotted` 时使用 |
