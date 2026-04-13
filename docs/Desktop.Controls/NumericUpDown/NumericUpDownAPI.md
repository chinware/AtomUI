# NumericUpDown API 参考

## 命名空间

```csharp
namespace AtomUI.Desktop.Controls;
```

AXAML 命名空间前缀：`atom`（`xmlns:atom="https://atomui.net"`）

---

## 枚举类型

### InputControlStyleVariant（来自 `AtomUI.Controls`）

样式变体枚举，控制输入框的视觉风格。

| 值 | 说明 |
|---|---|
| `Outline` | 轮廓样式，标准边框（默认） |
| `Filled` | 填充样式，灰色背景无边框 |
| `Borderless` | 无边框样式 |

### InputControlStatus（来自 `AtomUI.Controls`）

输入控件验证状态枚举。

| 值 | 说明 |
|---|---|
| `Default` | 默认状态 |
| `Warning` | 警告状态，黄色系 |
| `Error` | 错误状态，红色系 |

### SizeType（来自 `AtomUI.Controls`）

尺寸类型枚举。

| 值 | 说明 |
|---|---|
| `Small` | 小号（24px） |
| `Middle` | 中号（32px，默认） |
| `Large` | 大号（40px） |

---

## 公共属性（StyledProperty）

### AtomUI 扩展属性

| 属性名 | 类型 | 默认值 | 说明 |
|---|---|---|---|
| `SizeType` | `SizeType` | `SizeType.Middle` | 控件尺寸（共享属性，通过 `AddOwner` 注册） |
| `StyleVariant` | `InputControlStyleVariant` | `InputControlStyleVariant.Outline` | 样式变体（共享属性） |
| `Status` | `InputControlStatus` | `InputControlStatus.Default` | 验证状态（共享属性） |
| `StringMode` | `bool` | `false` | 高精度字符串模式，启用后通过 `StringValue` 操作数值 |
| `StringValue` | `string?` | `null` | StringMode 下的原始数值字符串 |
| `Keyboard` | `bool` | `true` | 是否允许键盘方向键步进 |
| `MouseWheel` | `bool` | `true` | 是否允许鼠标滚轮步进 |
| `IsAllowClear` | `bool` | `false` | 是否显示清除按钮（共享属性，来自 `TextBox`） |
| `ClearIcon` | `PathIcon?` | `CloseCircleFilled` | 自定义清除按钮图标 |
| `LeftAddOn` | `object?` | `null` | 外部左侧附加内容（如 `http://`） |
| `LeftAddOnTemplate` | `IDataTemplate?` | `null` | 外部左侧附加内容模板 |
| `RightAddOn` | `object?` | `null` | 外部右侧附加内容（如 `.com`） |
| `RightAddOnTemplate` | `IDataTemplate?` | `null` | 外部右侧附加内容模板 |
| `InnerLeftContentTemplate` | `IDataTemplate?` | `null` | 内部左侧内容模板 |
| `InnerRightContentTemplate` | `IDataTemplate?` | `null` | 内部右侧内容模板 |
| `PlaceholderText` | `string?` | `null` | 占位提示文本（共享属性，来自 `TextBox`） |
| `PlaceholderForeground` | `IBrush?` | 由 Token 控制 | 占位文本颜色（共享属性，来自 `TextBox`） |
| `IsMotionEnabled` | `bool` | 跟随全局 Token | 是否启用过渡动画（共享属性） |

> **注意**：`InnerLeftContent` 和 `InnerRightContent` 属性通过 `ButtonSpinner` 中转，在 NumericUpDown 的模板中通过 `Binding` 传递，而非直接注册为 `StyledProperty`。

### 继承自 Avalonia.Controls.NumericUpDown 的常用属性

| 属性名 | 类型 | 默认值 | 说明 |
|---|---|---|---|
| `Value` | `decimal?` | `null` | 当前数值，null 表示无值 |
| `Minimum` | `decimal` | `decimal.MinValue` | 允许的最小值 |
| `Maximum` | `decimal` | `decimal.MaxValue` | 允许的最大值 |
| `Increment` | `decimal` | `1` | 每次步进的增量 |
| `FormatString` | `string?` | `null` | 显示格式化字符串（如 `"N2"` 表示两位小数） |
| `NumberFormat` | `NumberFormatInfo?` | `null` | 自定义数字格式化对象 |
| `ParsingNumberStyle` | `NumberStyles` | `NumberStyles.Any` | 控制允许的数字输入格式 |
| `TextConverter` | `IValueConverter?` | `null` | 自定义文本到值的转换器 |
| `IsReadOnly` | `bool` | `false` | 只读模式，禁止编辑但允许步进 |
| `Text` | `string?` | `null` | 显示的文本内容 |
| `Watermark` | `string?` | `null` | 水印文本（设置后自动同步到 `PlaceholderText`） |
| `IsEnabled` | `bool` | `true` | 是否启用 |
| `HorizontalAlignment` | `HorizontalAlignment` | `Left` | 水平对齐 |
| `VerticalAlignment` | `VerticalAlignment` | `Top` | 垂直对齐 |

---

## 事件

| 事件名 | 类型 | 说明 |
|---|---|---|
| `ValueChanged` | `EventHandler<NumericUpDownValueChangedEventArgs>` | 数值变更时触发（继承自 `Avalonia.Controls.NumericUpDown`） |
| `Spin` | `EventHandler<SpinEventArgs>` | 步进操作时触发（继承自 `Spinner`） |

---

## 伪类（Pseudo-Classes）

NumericUpDown 本身不定义自定义伪类，但通过继承和内部控件（ButtonSpinner、TextBox）获得以下标准伪类支持：

### 继承的标准伪类

| 伪类 | 触发条件 |
|---|---|
| `:pointerover` | 鼠标悬浮 |
| `:disabled` | `IsEnabled == false` |
| `:focus` | 获得焦点 |
| `:focus-within` | 控件或其子控件获得焦点 |

---

## 实现的接口

### ISizeTypeAware

```csharp
public SizeType SizeType { get; set; }
```

支持 Large / Middle / Small 三种尺寸。

### IInputControlStyleVariantAware

```csharp
public InputControlStyleVariant StyleVariant { get; set; }
```

支持 Outline / Filled / Borderless 三种样式变体。

### IInputControlStatusAware

```csharp
public InputControlStatus Status { get; set; }
```

支持 Error / Warning 验证状态视觉反馈。

### IMotionAwareControl

```csharp
public bool IsMotionEnabled { get; set; }
```

控制过渡动画开关。

### ICompactSpaceAware

在 `Space.Compact` 容器中使用时自动调整圆角和边框。NumericUpDown 通过内部 `ButtonSpinner` 的 `AddOnDecoratedBox` 获取实际边框厚度。

### IFormItemAware

可作为 `FormItem` 的子控件参与表单验证流程：
- `SetFormValue(object?)` → 设置 `Value`
- `GetFormValue()` → 返回 `Value`
- `ClearFormValue()` → 将 `Value` 设为 `null`
- `NotifyValidateStatus(FormValidateStatus)` → 映射到 `Status` 属性
