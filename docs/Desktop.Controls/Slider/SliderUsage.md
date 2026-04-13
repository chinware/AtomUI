# Slider 使用文档

本文档介绍 AtomUI Slider 控件的各种使用方式和常见场景。

> 📖 Gallery 源码位置：`controlgallery/AtomUIGallery/ShowCases/Views/DataEntry/SliderShowCase.axaml`

---

## 前置准备

### AXAML 命名空间

```xml
xmlns:atom="https://atomui.net"
```

### C# 命名空间

```csharp
using AtomUI.Desktop.Controls;
```

---

## 1. 基本用法

最简单的滑动条，通过拖拽选择一个值：

```xml
<atom:Slider Maximum="100" Minimum="0" Value="50" TickFrequency="5" />
```

通过 `IsEnabled` 控制启用/禁用状态：

```xml
<atom:Slider Maximum="100" Minimum="0" Value="50" IsEnabled="False" />
```

配合 `ToggleSwitch` 动态切换启用状态（参考 Gallery `Basic` 示例）：

```xml
<StackPanel Orientation="Vertical" Spacing="20">
    <atom:Slider
        Maximum="100" Minimum="0"
        TickFrequency="5"
        IsEnabled="{Binding NormalEnabled}"
        Value="50" />
    <StackPanel Orientation="Horizontal" Spacing="2">
        <atom:TextBlock VerticalAlignment="Center">Enabled:</atom:TextBlock>
        <atom:ToggleSwitch SizeType="Small" IsChecked="{Binding NormalEnabled, Mode=TwoWay}" />
    </StackPanel>
</StackPanel>
```

---

## 2. 范围选择（双滑块）

通过 `IsRangeMode="True"` 启用双滑块模式，使用 `RangeValue` 设置范围。拖拽时自动判断离哪个滑块更近：

```xml
<atom:Slider
    Maximum="100" Minimum="0"
    IsRangeMode="True"
    TickFrequency="5"
    RangeValue="20, 80" />
```

在 C# 中设置范围值：

```csharp
slider.RangeValue = new SliderRangeValue { StartValue = 20, EndValue = 80 };
```

**约束**：`StartValue` 必须 ≤ `EndValue`，否则解析时抛出 `FormatException`。

---

## 3. 自定义 Tooltip

通过 `ValueFormatTemplate` 自定义 Tooltip 显示格式，使用 `string.Format` 语法：

```xml
<atom:Slider
    Maximum="100" Minimum="0"
    TickFrequency="1"
    IsSnapToTickEnabled="True"
    ValueFormatTemplate="\{0\}%"
    Value="20" />
```

> ⚠️ 注意：AXAML 中的 `{` 和 `}` 需要用 `\{` 和 `\}` 转义，避免被识别为标记扩展。

常用格式示例：

| 格式模板 | 效果 | 适用场景 |
|---|---|---|
| `\{0\}%` | `50%` | 百分比 |
| `\{0:F1\}°C` | `26.0°C` | 温度 |
| `$\{0:N0\}` | `$1,000` | 金额 |
| `\{0:0\}` | `50`（默认） | 整数 |

---

## 4. 垂直方向

通过 `Orientation="Vertical"` 切换为垂直滑动条。垂直模式下需要为容器设置固定高度：

```xml
<StackPanel Orientation="Horizontal" Spacing="20" Height="300">
    <!-- 单值垂直 -->
    <atom:Slider
        Maximum="100" Minimum="0"
        Orientation="Vertical"
        TickFrequency="1"
        Value="20" />

    <!-- 范围垂直 + 刻度吸附 -->
    <atom:Slider
        Maximum="100" Minimum="0"
        Orientation="Vertical"
        IsRangeMode="True"
        TickFrequency="5"
        IsSnapToTickEnabled="True"
        RangeValue="20, 80" />
</StackPanel>
```

---

## 5. 刻度标记（Marks）

通过 `Marks` 属性添加刻度标注。刻度标记在轨道上显示为小圆点，标签文字渲染在刻度下方（水平）或右侧（垂直）。点击标记可直接跳转到对应值：

```csharp
slider.Marks = new List<SliderMark>
{
    new("0°C", 0),
    new("26°C", 26),
    new("37°C", 37),
    new("100°C", 100) { LabelBrush = Brushes.Red, LabelFontWeight = FontWeight.Bold }
};
```

每个 `SliderMark` 支持自定义样式：

| 属性 | 说明 |
|---|---|
| `Label` | 标注文字 |
| `Value` | 对应值 |
| `LabelBrush` | 文字颜色（可选） |
| `LabelFontStyle` | 字体样式（可选） |
| `LabelFontWeight` | 字体粗细（可选） |

在 Gallery 中，`Slider4`~`Slider7` 演示了带刻度标记的滑动条（通过 code-behind 设置 Marks）。

---

## 6. 刻度吸附

设置 `TickFrequency` 和 `IsSnapToTickEnabled` 使滑块只能停留在刻度位置：

```xml
<atom:Slider
    Maximum="100" Minimum="0"
    TickFrequency="10"
    IsSnapToTickEnabled="True"
    Value="30" />
```

键盘操作时也遵循刻度吸附规则。

---

## 7. 不包含模式

设置 `Included="False"` 时，轨道不高亮已覆盖部分，只显示刻度标记和滑块位置：

```xml
<!-- 单值不包含 -->
<atom:Slider Maximum="100" Minimum="0" Included="False" Value="20" />

<!-- 范围不包含 -->
<atom:Slider
    Maximum="100" Minimum="0"
    IsRangeMode="True"
    TickFrequency="5"
    Included="False"
    RangeValue="20, 80" />
```

---

## 8. 方向反转

设置 `IsDirectionReversed="True"` 反转滑动方向：

```xml
<atom:Slider Maximum="100" Minimum="0" IsDirectionReversed="True" Value="70" />
```

- 水平模式：值从右到左递增
- 垂直模式：值从上到下递增

---

## 9. 禁用动画

通过 `IsMotionEnabled="False"` 禁用滑块的过渡动画：

```xml
<atom:Slider Maximum="100" Value="50" IsMotionEnabled="False" />
```

---

## 常见组合模式

### 表单中的滑动条

Slider 实现了 `IFormItemAware` 接口，可直接嵌入 `FormItem`：

```xml
<atom:FormItem Label="Volume">
    <atom:Slider Maximum="100" Minimum="0" Value="60" />
</atom:FormItem>
```

范围模式下表单值类型为 `SliderRangeValue`：

```xml
<atom:FormItem Label="Price Range">
    <atom:Slider Maximum="1000" Minimum="0" IsRangeMode="True" RangeValue="200, 800" />
</atom:FormItem>
```

### 与数值输入联动

```xml
<StackPanel Orientation="Horizontal" Spacing="16">
    <atom:Slider Name="LinkedSlider" Maximum="100" Value="50" Width="200" />
    <atom:NumericUpDown Value="{Binding #LinkedSlider.Value}" Maximum="100" Minimum="0" />
</StackPanel>
```

### 数据绑定

```xml
<atom:Slider Maximum="100" Minimum="0" Value="{Binding Volume, Mode=TwoWay}" />
```

范围模式数据绑定：

```xml
<atom:Slider
    Maximum="100" Minimum="0"
    IsRangeMode="True"
    RangeValue="{Binding PriceRange, Mode=TwoWay}" />
```

---

## 键盘操作速查

| 按键 | 说明 |
|---|---|
| `←` / `↓` | 减少 `SmallChange` |
| `→` / `↑` | 增加 `SmallChange` |
| `PageUp` | 增加 `LargeChange` |
| `PageDown` | 减少 `LargeChange` |
| `Home` | 跳转到 `Minimum` |
| `End` | 跳转到 `Maximum` |

> 当 `IsDirectionReversed="True"` 时，方向键的增减方向会反转。
