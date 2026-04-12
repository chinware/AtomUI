# DatePicker API 参考

## 命名空间

```csharp
namespace AtomUI.Desktop.Controls;
```

AXAML 命名空间前缀：`atom`（`xmlns:atom="https://atomui.net"`）

---

## 枚举类型

### ClockIdentifierType

时钟标识类型枚举，控制时间显示的小时制。

| 值 | 说明 |
|---|---|
| `HourClock12` | 12 小时制（AM/PM） |
| `HourClock24` | 24 小时制（默认） |

### InputControlStyleVariant

输入控件样式变体枚举。

| 值 | 说明 |
|---|---|
| `Outline` | 轮廓样式（默认），白色背景 + 实线边框 |
| `Filled` | 填充样式，灰色背景 + 无边框 |
| `Borderless` | 无边框样式，透明背景 + 无边框 |

### InputControlStatus

输入控件状态枚举。

| 值 | 说明 |
|---|---|
| `Default` | 默认状态 |
| `Warning` | 警告状态（橙色边框） |
| `Error` | 错误状态（红色边框） |

### SizeType（来自 `AtomUI.Controls`）

尺寸类型枚举。

| 值 | 说明 |
|---|---|
| `Small` | 小号 |
| `Middle` | 中号（默认） |
| `Large` | 大号 |

---

## DatePicker 公共属性（StyledProperty）

| 属性名 | 类型 | 默认值 | 说明 |
|---|---|---|---|
| `SelectedDateTime` | `DateTime?` | `null` | 当前选中的日期时间值，支持双向绑定和数据验证 |
| `DefaultDateTime` | `DateTime?` | `null` | 默认日期时间值，组件初始化时若 `SelectedDateTime` 为空则自动设置为此值 |
| `Format` | `string?` | `null` | 自定义日期显示格式，为 null 时自动推断（`yyyy-MM-dd` 或 `yyyy-MM-dd HH:mm:ss`） |
| `IsShowTime` | `bool` | `false` | 是否显示时间选择器，开启后日历面板右侧附加 TimeView |
| `IsNeedConfirm` | `bool` | `false` | 是否需要手动确认，开启后底部显示确认按钮 |
| `IsShowNow` | `bool` | `true` | 是否显示今天/现在快捷按钮 |
| `ClockIdentifier` | `ClockIdentifierType` | `HourClock24` | 时钟标识，控制 12/24 小时制显示 |

### 继承自 InfoPickerInput 的常用属性

| 属性名 | 类型 | 默认值 | 说明 |
|---|---|---|---|
| `PlaceholderText` | `string?` | `null` | 占位文本，未选择日期时显示 |
| `InfoIcon` | `PathIcon?` | `CalendarOutlined` | 输入框图标，默认为日历图标 |
| `PickerPlacement` | `PlacementMode` | `BottomEdgeAlignedLeft` | Flyout 弹出方向 |
| `SizeType` | `SizeType` | `SizeType.Middle` | 控件尺寸（共享属性，通过 `AddOwner` 注册） |
| `StyleVariant` | `InputControlStyleVariant` | `Outline` | 样式变体（Outline / Filled / Borderless） |
| `Status` | `InputControlStatus` | `Default` | 验证状态（Default / Warning / Error） |
| `IsMotionEnabled` | `bool` | 跟随全局 Token | 是否启用过渡动画 |
| `IsEnabled` | `bool` | `true` | 是否启用 |
| `IsReadOnly` | `bool` | `false` | 是否只读 |
| `LeftAddOn` | `object?` | `null` | 左侧附加内容（外部） |
| `RightAddOn` | `object?` | `null` | 右侧附加内容（外部） |
| `ContentLeftAddOn` | `object?` | `null` | 左侧附加内容（内部） |
| `ContentRightAddOn` | `object?` | `null` | 右侧附加内容（内部） |
| `IsShowArrow` | `bool` | `false` | Flyout 是否显示箭头 |
| `MarginToAnchor` | `double` | 由 Token 控制 | Flyout 与锚点的距离 |

---

## RangeDatePicker 公共属性（StyledProperty）

| 属性名 | 类型 | 默认值 | 说明 |
|---|---|---|---|
| `RangeStartSelectedDate` | `DateTime?` | `null` | 范围起始日期，支持双向绑定和数据验证 |
| `RangeEndSelectedDate` | `DateTime?` | `null` | 范围结束日期，支持双向绑定和数据验证 |
| `RangeStartDefaultDate` | `DateTime?` | `null` | 范围起始默认日期（CLR 属性） |
| `RangeEndDefaultDate` | `DateTime?` | `null` | 范围结束默认日期（CLR 属性） |
| `IsNeedConfirm` | `bool` | `false` | 是否需要手动确认 |
| `IsShowNow` | `bool` | `true` | 是否显示今天/现在快捷按钮 |
| `IsShowTime` | `bool` | `false` | 是否显示时间选择器 |
| `ClockIdentifier` | `ClockIdentifierType` | `HourClock24` | 时钟标识 |
| `Format` | `string?` | `null` | 自定义日期显示格式 |

### 继承自 RangeInfoPickerInput 的附加属性

| 属性名 | 类型 | 默认值 | 说明 |
|---|---|---|---|
| `SecondaryPlaceholderText` | `string?` | `null` | 结束日期输入框的占位文本 |

> RangeDatePicker 同时继承了 InfoPickerInput 的所有公共属性（`SizeType`、`StyleVariant`、`Status`、`PickerPlacement` 等）。

---

## 公共方法

### DatePicker

| 方法名 | 返回类型 | 说明 |
|---|---|---|
| `Clear()` | `void` | 清除选中的日期值（设为 `null`），不考虑默认值 |
| `Reset()` | `void` | 重置为默认值，当 `DefaultDateTime` 不为 null 时，将 `SelectedDateTime` 设为 `DefaultDateTime` |

### RangeDatePicker

| 方法名 | 返回类型 | 说明 |
|---|---|---|
| `Clear()` | `void` | 同时清除起始和结束日期 |
| `Reset()` | `void` | 将起始和结束日期重置为各自的默认值 |

---

## 伪类（Pseudo-Classes）

DatePicker 和 RangeDatePicker 继承自 InfoPickerInput 的伪类：

| 伪类 | 常量 | 触发条件 |
|---|---|---|
| `:choosing` | `InfoPickerPseudoClass.Choosing` | 用户正在面板中选择（悬浮在日历或时间选择器上） |
| `:flyout-open` | `InfoPickerPseudoClass.FlyoutOpen` | Flyout 弹出面板处于打开状态 |

### 继承的标准伪类

| 伪类 | 触发条件 |
|---|---|
| `:pointerover` | 鼠标悬浮 |
| `:disabled` | `IsEnabled == false` |
| `:focus` | 获得焦点 |
| `:focus-visible` | 通过键盘获得焦点 |

---

## 实现的接口

### ISizeTypeAware

```csharp
public SizeType SizeType { get; set; }
```

### IMotionAwareControl

```csharp
public bool IsMotionEnabled { get; set; }
```

### IInputControlStatusAware

```csharp
public InputControlStatus Status { get; set; }
```

### IInputControlStyleVariantAware

```csharp
public InputControlStyleVariant StyleVariant { get; set; }
```

### ICompactSpaceAware

在 `Space.Compact` 容器中使用时自动调整圆角和边框。

### IFormItemAware

可作为 `FormItem` 的子控件参与表单验证流程。

- **DatePicker** 表单值类型：`DateTime?`
- **RangeDatePicker** 表单值类型：`(DateTime?, DateTime?)?`

### IFormItemFeedbackAware

```csharp
public IFormValidateFeedback? FormFeedback { get; set; }
```
