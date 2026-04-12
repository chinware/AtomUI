# CheckBox API 参考

## 命名空间

```csharp
namespace AtomUI.Desktop.Controls;
```

AXAML 命名空间前缀：`atom`（`xmlns:atom="https://atomui.net"`）

---

## 数据类型

### ICheckBoxOption（接口）

复选框选项数据接口，用于 `CheckBoxGroup` 数据驱动模式。

| 属性 | 类型 | 说明 |
|---|---|---|
| `IsEnabled` | `bool` | 是否启用 |
| `Content` | `object?` | 选项内容（文本） |
| `IsChecked` | `bool` | 是否选中 |

### CheckBoxOption（类）

`ICheckBoxOption` 的默认实现类。

```csharp
public class CheckBoxOption : ICheckBoxOption
{
    public bool IsEnabled { get; set; } = true;
    public object? Content { get; set; }
    public bool IsChecked { get; set; } = false;
}
```

---

## CheckBox 公共属性（StyledProperty）

| 属性名 | 类型 | 默认值 | 说明 |
|---|---|---|---|
| `IsMotionEnabled` | `bool` | 跟随全局 Token | 是否启用过渡动画 |
| `IsWaveSpiritEnabled` | `bool` | 跟随全局 Token | 是否启用点击波纹效果 |

### 继承自 Avalonia.Controls.CheckBox 的常用属性

| 属性名 | 类型 | 默认值 | 说明 |
|---|---|---|---|
| `Content` | `object?` | `null` | 复选框文本内容 |
| `ContentTemplate` | `IDataTemplate?` | `null` | 内容模板 |
| `IsChecked` | `bool?` | `false` | 选中状态：`true`/`false`/`null`（不确定态） |
| `IsThreeState` | `bool` | `false` | 是否启用三态模式 |
| `Command` | `ICommand?` | `null` | 点击命令 |
| `CommandParameter` | `object?` | `null` | 命令参数 |
| `IsEnabled` | `bool` | `true` | 是否启用 |
| `Foreground` | `IBrush?` | 由主题控制 | 文本颜色 |
| `Background` | `IBrush?` | `Transparent` | 背景色 |
| `Cursor` | `Cursor` | `Hand` | 鼠标光标 |

---

## CheckBox 事件

| 事件名 | 类型 | 说明 |
|---|---|---|
| `IsCheckedChanged` | `EventHandler<RoutedEventArgs>` | 选中状态变化事件（继承自 `ToggleButton`） |
| `Click` | `EventHandler<RoutedEventArgs>` | 点击事件（继承自 `Button`） |

---

## CheckBox 伪类（Pseudo-Classes）

CheckBox 使用 Avalonia 内置伪类，无自定义伪类。

| 伪类 | 触发条件 |
|---|---|
| `:checked` | `IsChecked == true` |
| `:unchecked` | `IsChecked == false` |
| `:indeterminate` | `IsChecked == null` |
| `:pointerover` | 鼠标悬浮 |
| `:pressed` | 按下 |
| `:disabled` | `IsEnabled == false` |
| `:focus` | 获得焦点 |
| `:focus-visible` | 通过键盘获得焦点 |

---

## CheckBoxGroup 公共属性（StyledProperty）

| 属性名 | 类型 | 默认值 | 说明 |
|---|---|---|---|
| `ItemSpacing` | `double` | `SharedToken.SpacingXS` | 复选框之间的水平间距 |
| `LineSpacing` | `double` | `SharedToken.SpacingXS` | 复选框之间的换行间距 |
| `Orientation` | `Orientation` | `Horizontal` | 排列方向 |
| `IsMotionEnabled` | `bool` | 跟随全局 Token | 是否启用过渡动画 |
| `ItemsSource` | `IEnumerable?` | `null` | 数据源绑定，绑定 `ICheckBoxOption` 集合 |
| `ItemTemplate` | `IDataTemplate?` | 默认文本模板 | 子项数据模板 |
| `CheckedItems` | `IList?` | `null` | 选中项集合（双向绑定），可获取/设置选中的数据项 |
| `Items` | `ItemCollection` | 空集合 | 直接添加 `CheckBox` 子控件（`[Content]` 属性） |

---

## CheckBoxGroup 事件

| 事件名 | 类型 | 说明 |
|---|---|---|
| `CheckedChanged` | `EventHandler<CheckBoxGroupCheckedChangedEventArgs>` | 选中状态变化路由事件，包含添加/移除的选项信息 |

### CheckBoxGroupCheckedChangedEventArgs

继承自 `RoutedEventArgs`，定义于 `AtomUI.Controls` 命名空间。

| 属性名 | 类型 | 说明 |
|---|---|---|
| `AddedItems` | `IList` | 本次操作新增的选中项集合 |
| `RemovedItems` | `IList` | 本次操作取消选中的项集合 |

**使用示例**：

```csharp
checkBoxGroup.CheckedChanged += (sender, args) =>
{
    foreach (var item in args.AddedItems)
    {
        Console.WriteLine($"新增选中: {item}");
    }
    foreach (var item in args.RemovedItems)
    {
        Console.WriteLine($"取消选中: {item}");
    }
};
```

---

## 实现的接口

### IWaveSpiritAwareControl（CheckBox）

```csharp
public bool IsWaveSpiritEnabled { get; set; }
```

### IFormItemAware（CheckBox / CheckBoxGroup）

可作为 `FormItem` 的子控件参与表单验证流程。CheckBox 的表单值为 `bool?`，CheckBoxGroup 的表单值为 `IList?`。

### IMotionAwareControl（CheckBoxGroup）

```csharp
public bool IsMotionEnabled { get; set; }
```
