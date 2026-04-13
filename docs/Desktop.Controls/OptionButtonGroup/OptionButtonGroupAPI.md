# OptionButtonGroup API 参考

## 命名空间

```csharp
// 基础层（设备无关）
namespace AtomUI.Controls.Commons;    // AbstractOptionButton, AbstractOptionButtonGroup
namespace AtomUI.Controls;            // 枚举、数据模型、事件

// 平台层（桌面端）
namespace AtomUI.Desktop.Controls;    // OptionButton, OptionButtonGroup
```

AXAML 命名空间前缀：`atom`（`xmlns:atom="https://atomui.net"`）

---

## 枚举类型

### OptionButtonStyle（来自 `AtomUI.Controls`）

按钮样式枚举，控制选中项的视觉风格。

| 值 | 说明 |
|---|---|
| `Outline` | 轮廓样式（默认），选中项通过边框和文字颜色区分 |
| `Solid` | 实色样式，选中项使用主色填充背景 |

### OptionButtonPositionTrait（来自 `AtomUI.Controls`）

按钮在组中的位置标识（内部使用，自动分配）。

| 值 | 说明 |
|---|---|
| `First` | 第一个按钮 |
| `Last` | 最后一个按钮 |
| `Middle` | 中间按钮 |
| `OnlyOne` | 唯一按钮 |

### SizeType（来自 `AtomUI.Controls`）

尺寸类型枚举。

| 值 | 说明 |
|---|---|
| `Small` | 小号（24px） |
| `Middle` | 中号（32px，默认） |
| `Large` | 大号（40px） |

---

## OptionButtonGroup 公共属性

| 属性名 | 类型 | 默认值 | 说明 |
|---|---|---|---|
| `SizeType` | `SizeType` | `SizeType.Middle` | 控件尺寸，自动传播给所有子 OptionButton |
| `ButtonStyle` | `OptionButtonStyle` | `OptionButtonStyle.Outline` | 按钮样式（Outline / Solid） |
| `IsMotionEnabled` | `bool` | 跟随全局 Token | 是否启用过渡动画 |
| `IsWaveSpiritEnabled` | `bool` | 跟随全局 Token | 是否启用点击波纹效果 |

### 继承自 SelectingItemsControl 的常用属性

| 属性名 | 类型 | 默认值 | 说明 |
|---|---|---|---|
| `ItemsSource` | `IEnumerable?` | `null` | 数据源（可绑定 `OptionButtonData` 集合） |
| `ItemTemplate` | `IDataTemplate?` | `null` | 子项模板 |
| `SelectedItem` | `object?` | `null` | 当前选中项 |
| `SelectedIndex` | `int` | `-1` | 当前选中项索引 |
| `SelectionMode` | `SelectionMode` | `Single \| AlwaysSelected` | 选择模式（默认单选且始终有选中项） |
| `IsEnabled` | `bool` | `true` | 是否启用（禁用时整个组不可交互） |
| `BorderBrush` | `IBrush?` | 由 Token 控制 | 边框颜色 |
| `BorderThickness` | `Thickness` | 由 Token 控制 | 边框厚度 |
| `CornerRadius` | `CornerRadius` | 由 Token 控制 | 圆角半径 |

---

## OptionButton 公共属性

| 属性名 | 类型 | 默认值 | 说明 |
|---|---|---|---|
| `Icon` | `PathIcon?` | `null` | 按钮图标，使用 Ant Design 图标集 |

### 继承自 RadioButton 的常用属性

| 属性名 | 类型 | 默认值 | 说明 |
|---|---|---|---|
| `Content` | `object?` | `null` | 按钮文本内容 |
| `IsChecked` | `bool?` | `false` | 是否选中 |
| `IsEnabled` | `bool` | `true` | 是否启用（可单独禁用某个选项） |

---

## 数据模型

### OptionButtonData

用于 `ItemsSource` 数据驱动模式的数据模型。

```csharp
public class OptionButtonData : IOptionButtonData
{
    public object? Header { get; init; }    // 按钮文本内容
    public PathIcon? Icon { get; init; }    // 按钮图标
    public bool IsEnabled { get; init; } = true;  // 是否启用
}
```

---

## 事件

### OptionButtonGroup 事件

| 事件名 | 类型 | 说明 |
|---|---|---|
| `OptionCheckedChanged` | `EventHandler<OptionCheckedChangedEventArgs>` | 选中项变更时触发（路由事件，冒泡策略） |

### OptionCheckedChangedEventArgs

| 属性 | 类型 | 说明 |
|---|---|---|
| `CheckedOption` | `AbstractOptionButton` | 新选中的按钮实例 |
| `Index` | `int` | 新选中项的索引 |

### OptionButton 事件（继承自 RadioButton）

| 事件名 | 类型 | 说明 |
|---|---|---|
| `IsCheckedChanged` | `EventHandler<RoutedEventArgs>` | 选中状态变更时触发 |
| `Click` | `EventHandler<RoutedEventArgs>` | 按钮点击时触发 |

---

## 伪类（Pseudo-Classes）

### OptionButton 伪类

| 伪类 | 触发条件 |
|---|---|
| `:checked` | `IsChecked == true`（选中状态） |
| `:unchecked` | `IsChecked == false`（未选中状态） |
| `:pointerover` | 鼠标悬浮 |
| `:pressed` | 按钮按下 |
| `:disabled` | `IsEnabled == false` |

---

## 实现的接口

### ISizeTypeAware（OptionButtonGroup）

```csharp
public SizeType SizeType { get; set; }
```

### IWaveSpiritAwareControl（OptionButtonGroup）

```csharp
public bool IsWaveSpiritEnabled { get; set; }
```

### IFormItemAware（OptionButtonGroup）

可作为 `FormItem` 的子控件参与表单验证流程：
- `SetFormValue(object?)` → 设置 `SelectedItem`
- `GetFormValue()` → 返回 `SelectedItem`
- `ClearFormValue()` → 清除选中项
