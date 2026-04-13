# RadioButton API 参考

## 命名空间

```csharp
// 桌面端控件
namespace AtomUI.Desktop.Controls;

// 跨平台基类（AbstractRadioButton, AbstractRadioButtonGroup）
namespace AtomUI.Controls.Commons;

// 数据类型（IRadioButtonOption, RadioButtonOption, RadioButtonGroupCheckedChangedEventArgs）
namespace AtomUI.Controls;
```

AXAML 命名空间前缀：`atom`（`xmlns:atom="https://atomui.net"`）

---

## 数据类型

### IRadioButtonOption（接口）

单选框选项数据接口，用于 `RadioButtonGroup` 数据驱动模式。

| 属性 | 类型 | 说明 |
|---|---|---|
| `IsEnabled` | `bool` | 是否启用 |
| `Content` | `object?` | 选项内容（文本或其他控件） |
| `IsChecked` | `bool` | 是否选中 |

### RadioButtonOption（类）

`IRadioButtonOption` 的默认实现类，所有属性均有默认值：

```csharp
public class RadioButtonOption : IRadioButtonOption
{
    public bool IsEnabled { get; set; } = true;
    public object? Content { get; set; }
    public bool IsChecked { get; set; } = false;
}
```

### RadioButtonGroupCheckedChangedEventArgs（类）

选中变化事件参数，继承自 `EventArgs`。

| 属性 | 类型 | 说明 |
|---|---|---|
| `OldCheckedItem` | `object?` | 之前的选中项 |
| `NewCheckedItem` | `object?` | 新的选中项 |

---

## RadioButton 公共属性（StyledProperty）

以下属性定义在 `AbstractRadioButton` 基类中，`RadioButton` 直接继承：

| 属性名 | 类型 | 默认值 | 来源 | 说明 |
|---|---|---|---|---|
| `IsMotionEnabled` | `bool` | 跟随全局 Token `EnableMotion` | `AbstractRadioButton` | 是否启用过渡动画（边框色、圆点大小过渡） |
| `IsWaveSpiritEnabled` | `bool` | 跟随全局 Token `EnableWaveSpirit` | `AbstractRadioButton` | 是否启用选中时的圆形波纹效果 |

### 继承自 Avalonia.Controls.RadioButton 的常用属性

| 属性名 | 类型 | 默认值 | 说明 |
|---|---|---|---|
| `Content` | `object?` | `null` | 单选框文本内容，可以是字符串或任意控件 |
| `ContentTemplate` | `IDataTemplate?` | `null` | 内容数据模板 |
| `IsChecked` | `bool?` | `false` | 是否选中（支持三态，单选框通常仅使用 `true`/`false`） |
| `GroupName` | `string?` | `null` | 分组名称，同名的 RadioButton 互斥 |
| `Command` | `ICommand?` | `null` | 点击命令（MVVM 绑定） |
| `CommandParameter` | `object?` | `null` | 命令参数 |
| `IsEnabled` | `bool` | `true` | 是否启用 |
| `Foreground` | `IBrush?` | 由主题控制（`ColorText`） | 文本颜色 |
| `Background` | `IBrush?` | `Transparent` | 背景色 |
| `Cursor` | `Cursor` | `Hand` | 鼠标光标 |
| `ClipToBounds` | `bool` | `false` | 裁剪超出边界的内容（设为 false 以支持波纹溢出） |
| `HorizontalAlignment` | `HorizontalAlignment` | `Left` | 水平对齐方式 |
| `VerticalAlignment` | `VerticalAlignment` | `Top` | 垂直对齐方式 |

---

## RadioButton 伪类（Pseudo-Classes）

| 伪类 | 触发条件 | 说明 |
|---|---|---|
| `:checked` | `IsChecked == true` | 选中状态，指示器变为主色填充 + 白色圆点 |
| `:unchecked` | `IsChecked == false` | 未选中状态，指示器为空心圆环 |
| `:pointerover` | 鼠标悬浮 | 未选中时边框色变为 `ColorPrimary` |
| `:pressed` | 按下 | — |
| `:disabled` | `IsEnabled == false` | 灰色调，不可交互 |
| `:focus` | 获得焦点 | — |
| `:focus-visible` | 通过键盘获得焦点 | 显示焦点指示框 |

---

## RadioButton 事件

| 事件名 | 类型 | 说明 |
|---|---|---|
| `Click` | `EventHandler<RoutedEventArgs>` | 点击事件（继承自 `Button`） |
| `IsCheckedChanged` | `RoutedEvent` | 选中状态变化路由事件（继承自 `ToggleButton`） |

---

## RadioButtonGroup 公共属性（StyledProperty）

以下属性定义在 `AbstractRadioButtonGroup` 基类中，`RadioButtonGroup` 直接继承：

| 属性名 | 类型 | 默认值 | 来源 | 说明 |
|---|---|---|---|---|
| `CheckedItem` | `object?` | `null` | `AbstractRadioButtonGroup` | 当前选中项。声明式用法中为 `RadioButton` 实例；数据驱动用法中为数据对象 |
| `ItemSpacing` | `double` | `0`（主题覆盖为 `SpacingXS`） | `AbstractRadioButtonGroup` | 单选框之间的间距 |
| `LineSpacing` | `double` | `0`（主题覆盖为 `SpacingXS`） | `AbstractRadioButtonGroup` | 换行间距 |
| `Orientation` | `Orientation` | `Horizontal` | `AbstractRadioButtonGroup` | 排列方向（水平/垂直） |
| `IsMotionEnabled` | `bool` | 跟随全局 Token `EnableMotion` | `AbstractRadioButtonGroup` | 是否启用过渡动画（向下传递给子项） |

### 继承自 Avalonia.Controls.ItemsControl 的常用属性

| 属性名 | 类型 | 默认值 | 说明 |
|---|---|---|---|
| `ItemsSource` | `IEnumerable?` | `null` | 数据源绑定，需实现 `IRadioButtonOption` 接口 |
| `ItemTemplate` | `IDataTemplate?` | 默认绑定 `Content` | 子项数据模板 |
| `ItemsPanel` | `ITemplate<Panel>` | `WrapPanel` | 子项布局面板 |
| `IsEnabled` | `bool` | `true` | 是否启用（影响整组） |

---

## RadioButtonGroup 事件

| 事件名 | 类型 | 说明 |
|---|---|---|
| `CheckedChanged` | `EventHandler<RadioButtonGroupCheckedChangedEventArgs>` | 选中项变化事件，提供 `OldCheckedItem` 和 `NewCheckedItem` |

---

## 实现的接口

### RadioButton 实现的接口

| 接口 | 定义位置 | 作用 |
|---|---|---|
| `IWaveSpiritAwareControl` | `AtomUI.Controls.Shared` | 支持点击涟漪动画，通过 `IsWaveSpiritEnabled` 属性控制 |
| `IFormItemAware` | `AtomUI.Controls.Shared` | 可作为 `FormItem` 的子控件参与表单验证 |

**IFormItemAware 在 RadioButton 中的行为：**
- `GetFormValue()` → 返回 `IsChecked` 值
- `SetFormValue(value)` → 设置 `IsChecked`（接受 `bool?`）
- `ClearFormValue()` → 将 `IsChecked` 设为 `null`
- `ValueChanged` → 当 `IsChecked` 变化时触发

### RadioButtonGroup 实现的接口

| 接口 | 定义位置 | 作用 |
|---|---|---|
| `IMotionAwareControl` | `AtomUI.Controls.Shared` | 动画开关，`IsMotionEnabled` 自动传递给所有子 RadioButton |
| `IFormItemAware` | `AtomUI.Controls.Shared` | 可作为 `FormItem` 的子控件参与表单验证 |

**IFormItemAware 在 RadioButtonGroup 中的行为：**
- `GetFormValue()` → 返回 `CheckedItem` 值
- `SetFormValue(value)` → 设置 `CheckedItem`
- `ClearFormValue()` → 将 `CheckedItem` 设为 `null`
- `ValueChanged` → 当 `CheckedItem` 变化时触发
