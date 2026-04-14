# ColorPicker API 参考

## 命名空间

```csharp
namespace AtomUI.Desktop.Controls;
```

AXAML 命名空间前缀：`atom`（`xmlns:atom="https://atomui.net"`）

---

## 枚举类型

### ColorFormat

颜色格式枚举，定义颜色值的编码格式。

| 值 | 说明 |
|---|---|
| `Hex` | 十六进制格式，如 `#1677ff`（默认） |
| `Hsva` | 色相-饱和度-明度-透明度格式 |
| `Rgba` | 红-绿-蓝-透明度格式 |

### ColorPickerValueSyncMode

颜色值同步模式枚举。

| 值 | 说明 |
|---|---|
| `Immediate` | 实时同步，拖动滑块时立即更新 `Value`（默认） |
| `OnCompleted` | 完成后同步，仅在关闭 Flyout 面板时更新 `Value` |

### FlyoutTriggerType

Flyout 触发方式枚举。

| 值 | 说明 |
|---|---|
| `Click` | 点击触发（默认） |
| `Hover` | 悬浮触发 |

### SizeType（来自 `AtomUI.Controls`）

| 值 | 说明 |
|---|---|
| `Small` | 小号 |
| `Middle` | 中号（默认） |
| `Large` | 大号 |

### InputControlStyleVariant（来自 `AtomUI.Controls`）

| 值 | 说明 |
|---|---|
| `Outlined` | 描边样式（默认） |
| `Filled` | 填充样式 |
| `Borderless` | 无边框样式 |

### InputControlStatus（来自 `AtomUI.Controls`）

| 值 | 说明 |
|---|---|
| `Default` | 默认状态 |
| `Warning` | 警告状态 |
| `Error` | 错误状态 |

---

## AbstractColorPicker 公共属性

以下属性定义在抽象基类 `AbstractColorPicker` 上，`ColorPicker` 和 `GradientColorPicker` 均继承。

| 属性名 | 类型 | 默认值 | 说明 |
|---|---|---|---|
| `Format` | `ColorFormat` | `ColorFormat.Hex` | 颜色格式 |
| `TriggerType` | `FlyoutTriggerType` | `FlyoutTriggerType.Click` | Flyout 触发方式 |
| `IsShowArrow` | `bool` | `false` | Flyout 是否显示箭头 |
| `IsPointAtCenter` | `bool` | `false` | Flyout 是否指向触发器中心 |
| `Placement` | `PlacementMode` | `PlacementMode.Bottom` | Flyout 弹出位置 |
| `PlacementGravity` | `PopupGravity` | — | Flyout 弹出对齐重力方向 |
| `MarginToAnchor` | `double` | — | Flyout 与锚点的间距 |
| `MouseEnterDelay` | `int` | — | 鼠标进入延迟（毫秒，Hover 模式） |
| `MouseLeaveDelay` | `int` | — | 鼠标离开延迟（毫秒，Hover 模式） |
| `IsAlphaEnabled` | `bool` | `true` | 是否启用 Alpha 通道 |
| `IsFormatEnabled` | `bool` | `true` | 是否启用颜色格式切换 |
| `IsShowText` | `bool` | `false` | 是否在触发器上显示颜色文本 |
| `IsClearEnabled` | `bool` | `false` | 是否允许清除颜色 |
| `SizeType` | `SizeType` | `SizeType.Middle` | 触发器尺寸 |
| `IsMotionEnabled` | `bool` | 跟随全局 Token | 是否启用过渡动画 |
| `EmptyColorText` | `string` | `string.Empty` | 颜色为空时显示的文本 |
| `IsPaletteGroupEnabled` | `bool` | `false` | 是否启用预设色板 |
| `PaletteGroup` | `List<ColorPickerPalette>?` | `null` | 自定义预设色板列表 |
| `StyleVariant` | `InputControlStyleVariant` | `InputControlStyleVariant.Outlined` | 输入控件样式变体 |
| `Status` | `InputControlStatus` | `InputControlStatus.Default` | 输入控件状态 |

### 继承自 Avalonia.Controls.Button 的常用属性

| 属性名 | 类型 | 默认值 | 说明 |
|---|---|---|---|
| `IsEnabled` | `bool` | `true` | 是否启用 |
| `Foreground` | `IBrush?` | 由主题控制 | 前景色 |
| `Background` | `IBrush?` | 由主题控制 | 背景色 |
| `BorderBrush` | `IBrush?` | 由主题控制 | 边框颜色 |
| `BorderThickness` | `Thickness` | 由 Token 控制 | 边框粗细 |
| `CornerRadius` | `CornerRadius` | 由 Token 控制 | 圆角半径 |
| `Padding` | `Thickness` | 由 Token 控制 | 内间距 |

---

## ColorPicker 专有属性

| 属性名 | 类型 | 默认值 | 说明 |
|---|---|---|---|
| `DefaultValue` | `Color?` | `null` | 默认颜色值 |
| `Value` | `Color?` | `null` | 当前颜色值 |
| `ValueSyncStrategy` | `ColorPickerValueSyncMode` | `Immediate` | 值同步模式 |

## ColorPicker 附加属性

| 属性名 | 类型 | 说明 |
|---|---|---|
| `ColorTextFormatter` | `Func<Color, ColorFormat, string>?` | 自定义颜色文本格式化函数 |

## ColorPicker 事件

| 事件名 | 类型 | 说明 |
|---|---|---|
| `ValueChanged` | `EventHandler<ColorChangedEventArgs>` | 颜色值变化时触发 |
| `ValueSelected` | `EventHandler<ColorSelectedEventArgs>` | Flyout 关闭时触发，确认选择 |

---

## GradientColorPicker 专有属性

| 属性名 | 类型 | 默认值 | 说明 |
|---|---|---|---|
| `DefaultValue` | `LinearGradientBrush?` | `null` | 默认渐变色值 |
| `Value` | `LinearGradientBrush?` | `null` | 当前渐变色值 |

## GradientColorPicker 附加属性

| 属性名 | 类型 | 说明 |
|---|---|---|
| `ColorTextFormatter` | `Action<LinearGradientBrush?, ColorFormat, Controls>?` | 自定义渐变色文本格式化函数 |

## GradientColorPicker 事件

| 事件名 | 类型 | 说明 |
|---|---|---|
| `GradientValueChanged` | `EventHandler<GradientColorChangedEventArgs>` | 渐变色值变化时触发 |
| `ValueSelected` | `EventHandler<GradientColorSelectedEventArgs>` | Flyout 关闭时触发，确认选择 |

---

## 公共方法

| 方法 | 返回类型 | 说明 |
|---|---|---|
| `ClosePickerFlyout()` | `void` | 关闭颜色选择器 Flyout |
| `FormatColor(Color, ColorFormat)` | `string` | `static` — 格式化颜色为字符串 |

---

## 伪类（Pseudo-Classes）

| 伪类 | 触发条件 |
|---|---|
| `:flyout-open` | Flyout 面板打开 |
| `:disabled` | `IsEnabled == false` |
| `:pointerover` | 鼠标悬浮 |
| `:pressed` | 触发器被按下 |
| `:focus` | 获得焦点 |
| `:focus-visible` | 键盘获得焦点 |

---

## 事件参数类型

### ColorChangedEventArgs

| 属性 | 类型 | 说明 |
|---|---|---|
| `OldColor` | `Color?` | 变化前的颜色 |
| `NewColor` | `Color?` | 变化后的颜色 |

### ColorSelectedEventArgs

| 属性 | 类型 | 说明 |
|---|---|---|
| `SelectedColor` | `Color` | 选中的颜色 |

### GradientColorChangedEventArgs

| 属性 | 类型 | 说明 |
|---|---|---|
| `OldColor` | `LinearGradientBrush?` | 变化前的渐变色 |
| `NewColor` | `LinearGradientBrush?` | 变化后的渐变色 |

### GradientColorSelectedEventArgs

| 属性 | 类型 | 说明 |
|---|---|---|
| — | `LinearGradientBrush` | 选中的渐变色值 |