# Slider API 参考

## 命名空间

```csharp
namespace AtomUI.Desktop.Controls;
```

AXAML 命名空间前缀：`atom`（`xmlns:atom="https://atomui.net"`）

---

## Slider 类

### 继承关系

```
System.Object
  └── Avalonia.AvaloniaObject
        └── Avalonia.Animatable
              └── Avalonia.StyledElement
                    └── Avalonia.Visual
                          └── Avalonia.Layout.Layoutable
                                └── Avalonia.Controls.Control
                                      └── Avalonia.Controls.Primitives.TemplatedControl
                                            └── Avalonia.Controls.Primitives.RangeBase
                                                  └── AtomUI.Desktop.Controls.Slider
```

### 实现的接口

| 接口 | 定义位置 | 作用 |
|---|---|---|
| `IMotionAwareControl` | `AtomUI.Controls.Shared` | 控制过渡动画开关（`IsMotionEnabled`） |
| `IFormItemAware` | `AtomUI.Controls.Shared` | 可参与表单验证；单值模式表单值为 `double`，范围模式为 `SliderRangeValue` |

---

## Slider 公共属性（StyledProperty）

### 值属性

| 属性名 | 类型 | 默认值 | 说明 |
|---|---|---|---|
| `Value` | `double` | `0` | 当前值（单值模式），继承自 `RangeBase` |
| `Minimum` | `double` | `0` | 最小值，继承自 `RangeBase` |
| `Maximum` | `double` | `100` | 最大值，继承自 `RangeBase` |
| `SmallChange` | `double` | `1` | 小步进（方向键），继承自 `RangeBase` |
| `LargeChange` | `double` | `10` | 大步进（PageUp/PageDown），继承自 `RangeBase` |
| `RangeValue` | `SliderRangeValue` | `default` | 范围值（范围模式），AXAML 中用逗号分隔的两个数字，通过 `AddOwner` 共享自 `SliderTrack` |
| `IsRangeMode` | `bool` | `false` | 是否启用范围选择模式（双滑块），通过 `AddOwner` 共享自 `SliderTrack` |

### 布局属性

| 属性名 | 类型 | 默认值 | 说明 |
|---|---|---|---|
| `Orientation` | `Orientation` | `Horizontal` | 滑动条方向，通过 `AddOwner` 共享自 `StackPanel` |
| `IsDirectionReversed` | `bool` | `false` | 是否反转方向，通过 `AddOwner` 共享自 `SliderTrack` |

### 刻度属性

| 属性名 | 类型 | 默认值 | 说明 |
|---|---|---|---|
| `TickFrequency` | `double` | `0` | 刻度间隔；为 `0` 时无刻度效果 |
| `IsSnapToTickEnabled` | `bool` | `false` | 是否吸附到最近刻度 |
| `Marks` | `List<SliderMark>?` | `null` | 刻度标记列表，通过 `AddOwner` 共享自 `SliderTrack` |
| `Included` | `bool` | `true` | 是否高亮轨道覆盖部分，通过 `AddOwner` 共享自 `SliderTrack` |

### 显示属性

| 属性名 | 类型 | 默认值 | 说明 |
|---|---|---|---|
| `ValueFormatTemplate` | `string` | `"{0:0}"` | Tooltip 值格式化模板（`string.Format` 语法） |

### 行为属性

| 属性名 | 类型 | 默认值 | 说明 |
|---|---|---|---|
| `IsMotionEnabled` | `bool` | 跟随全局 Token `EnableMotion` | 是否启用过渡动画，通过 `AddOwner` 共享自 `MotionAwareControlProperty` |
| `IsWaveAnimationEnabled` | `bool` | 跟随全局 Token | 是否启用聚焦波纹效果，通过 `AddOwner` 共享自 `WaveSpiritAwareControlProperty` |

---

## SliderRangeValue 结构体

```csharp
public record struct SliderRangeValue
{
    public double StartValue { get; set; }
    public double EndValue { get; set; }
    public static SliderRangeValue Parse(string expr);
}
```

| 属性 | 类型 | 说明 |
|---|---|---|
| `StartValue` | `double` | 范围起始值 |
| `EndValue` | `double` | 范围结束值 |

在 AXAML 中可用逗号分隔的数字字符串赋值：

```xml
RangeValue="20, 80"
```

**约束**：`StartValue` ≤ `EndValue`，否则解析抛出 `FormatException`。

---

## SliderMark 数据类

```csharp
public record SliderMark(string Label, double Value)
{
    public IBrush? LabelBrush { get; set; }
    public FontStyle LabelFontStyle { get; set; } = FontStyle.Normal;
    public FontWeight LabelFontWeight { get; set; } = FontWeight.Normal;
}
```

| 属性 | 类型 | 默认值 | 说明 |
|---|---|---|---|
| `Label` | `string` | （构造参数） | 刻度标注文字 |
| `Value` | `double` | （构造参数） | 刻度对应的值 |
| `LabelBrush` | `IBrush?` | `null` | 标注颜色（可选，默认使用 Token 颜色） |
| `LabelFontStyle` | `FontStyle` | `Normal` | 标注字体样式（可选） |
| `LabelFontWeight` | `FontWeight` | `Normal` | 标注字体粗细（可选） |

---

## SliderThumb 类

### 继承关系

```
Avalonia.Controls.Primitives.TemplatedControl
  └── AtomUI.Desktop.Controls.SliderThumb
```

### 公共事件（RoutedEvent）

| 事件名 | 事件参数类型 | 路由策略 | 说明 |
|---|---|---|---|
| `DragStarted` | `VectorEventArgs` | `Bubble` | 开始拖拽时触发 |
| `DragDelta` | `VectorEventArgs` | `Bubble` | 拖拽过程中持续触发 |
| `DragCompleted` | `VectorEventArgs` | `Bubble` | 拖拽完成时触发 |

### 公共属性（StyledProperty）

| 属性名 | 类型 | 说明 |
|---|---|---|
| `OutlineBrushProperty` | `IBrush?` | outline 环颜色（内部使用） |
| `OutlineThicknessProperty` | `Thickness` | outline 环厚度（内部使用） |
| `ThumbCircleSizeProperty` | `double` | 滑块圆形尺寸（内部使用） |

### 渲染机制

`SliderThumb` 重写 `Render()` 方法进行自绘：

1. 绘制底圆：白色背景 + 主色边框（`BorderBrush` / `BorderThickness`）
2. 绘制 outline 环：主色半透明环（`OutlineBrush` / `OutlineThickness`）

动画过渡通过 `Transitions` 配置，包含 `OutlineBrush`、`OutlineThickness`、`BorderBrush` 三个属性的过渡。

---

## SliderTrack 类

### 继承关系

```
Avalonia.Controls.Primitives.TemplatedControl
  └── AtomUI.Desktop.Controls.SliderTrack
```

### 公共属性（StyledProperty）

| 属性名 | 类型 | 默认值 | 说明 |
|---|---|---|---|
| `Minimum` | `double` | `0` | 最小值，通过 `AddOwner` 共享自 `RangeBase` |
| `Maximum` | `double` | `100` | 最大值，通过 `AddOwner` 共享自 `RangeBase` |
| `Value` | `double` | `0` | 当前值，通过 `AddOwner` 共享自 `RangeBase` |
| `RangeValue` | `SliderRangeValue` | `default` | 范围值 |
| `IsRangeMode` | `bool` | `false` | 范围模式开关 |
| `Orientation` | `Orientation` | `Horizontal` | 方向 |
| `StartSliderThumb` | `SliderThumb?` | `null` | 起始滑块 |
| `EndSliderThumb` | `SliderThumb?` | `null` | 结束滑块 |
| `IsDirectionReversed` | `bool` | `false` | 是否反转方向 |
| `IgnoreThumbDrag` | `bool` | `false` | 是否忽略滑块拖拽（由 Slider 接管交互） |
| `DeferThumbDrag` | `bool` | `false` | 是否延迟拖拽 |
| `Included` | `bool` | `true` | 是否高亮轨道覆盖部分 |
| `TrackBarBrush` | `IBrush?` | — | 已覆盖部分画刷 |
| `TrackGrooveBrush` | `IBrush?` | — | 轨道底层画刷 |
| `Marks` | `List<SliderMark>?` | `null` | 刻度标记列表 |
| `MarkLabelFontSize` | `double` | — | 刻度标签字号 |
| `MarkLabelFontFamily` | `FontFamily` | — | 刻度标签字体 |
| `MarkLabelBrush` | `IBrush?` | — | 刻度标签颜色 |

### 职责

`SliderTrack` 是 Slider 的核心渲染控件，负责：
- 轨道底层（Rail）和已覆盖部分（Track Bar）的自绘
- 刻度标记圆点和标签文字的自绘
- 滑块（SliderThumb）的位置计算和布局
- 指针事件的处理和转发

---

## 伪类

### Slider 伪类

| 伪类 | 来源 | 说明 |
|---|---|---|
| `:horizontal` | `StdPseudoClass.Horizontal` | 水平方向 |
| `:vertical` | `StdPseudoClass.Vertical` | 垂直方向 |
| `:pressed` | `StdPseudoClass.Pressed`（PressedMixin） | 正在拖拽中 |
| `:disabled` | Avalonia 内置 | 禁用态 |
| `:pointerover` | Avalonia 内置 | 鼠标悬浮 |

### SliderThumb 伪类

| 伪类 | 来源 | 说明 |
|---|---|---|
| `:pressed` | `StdPseudoClass.Pressed` | 正在拖拽中 |
| `:pointerover` | Avalonia 内置 | 鼠标悬浮 |
| `:focus` | Avalonia 内置 | 获得焦点 |

### SliderTrack 伪类

| 伪类 | 来源 | 说明 |
|---|---|---|
| `:horizontal` | `StdPseudoClass.Horizontal` | 水平方向 |
| `:vertical` | `StdPseudoClass.Vertical` | 垂直方向 |
| `:pointerover` | Avalonia 内置 | 鼠标悬浮 |
| `:disabled` | Avalonia 内置 | 禁用态 |

---

## 键盘操作

| 按键 | 说明 |
|---|---|
| `←` / `↓` | 减少 `SmallChange`（方向反转时增加） |
| `→` / `↑` | 增加 `SmallChange`（方向反转时减少） |
| `PageUp` | 增加 `LargeChange`（方向反转时减少） |
| `PageDown` | 减少 `LargeChange`（方向反转时增加） |
| `Home` | 跳转到 `Minimum` |
| `End` | 跳转到 `Maximum` |
| `Enter` | 切换 XY 导航焦点锁定（游戏手柄场景） |
| `Escape` | 退出焦点锁定 |

---

## 表单集成（IFormItemAware）

Slider 实现了 `IFormItemAware` 接口，可嵌入 `FormItem` 参与表单验证：

| 方法 | 行为 |
|---|---|
| `GetFormValue()` | 单值模式返回 `double`，范围模式返回 `SliderRangeValue` |
| `SetFormValue(object?)` | 单值模式设置 `Value`，范围模式设置 `RangeValue` |
| `ClearFormValue()` | 重置 `RangeValue` 为 `default` |
| `ValueChanged` 事件 | `Value` 或 `RangeValue` 变化时触发 |

---

## 数据验证

Slider 支持 Avalonia 数据验证机制：

```csharp
ValueProperty.OverrideMetadata<Slider>(new StyledPropertyMetadata<double>(enableDataValidation: true));
```

当绑定的 `Value` 属性验证失败时，会通过 `DataValidationErrors.SetError()` 设置错误状态。

---

## 无障碍支持

- `AutomationControlType` 默认为 `Slider`
- 通过 `SliderAutomationPeer` 提供自动化支持
- `SliderThumb` 通过 `SliderThumbAutomationPeer` 提供自动化支持
