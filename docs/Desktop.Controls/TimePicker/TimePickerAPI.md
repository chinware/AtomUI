# TimePicker API 参考

## 命名空间

```csharp
namespace AtomUI.Desktop.Controls;
```

AXAML 命名空间前缀：`atom`（`xmlns:atom="https://atomui.net"`）

---

## 枚举类型

### ClockIdentifierType

时钟格式枚举，定义 12 小时制或 24 小时制。

| 值 | 说明 |
|---|---|
| `HourClock12` | 12 小时制，面板额外显示 AM/PM 列 |
| `HourClock24` | 24 小时制（默认） |

### InputControlStyleVariant（来自 `AtomUI.Controls`）

输入控件样式变体枚举。

| 值 | 说明 |
|---|---|
| `Outline` | 带边框的标准样式（默认） |
| `Filled` | 填充背景样式 |
| `Borderless` | 无边框样式 |

### InputControlStatus（来自 `AtomUI.Controls`）

输入控件验证状态枚举。

| 值 | 说明 |
|---|---|
| `Default` | 默认状态 |
| `Warning` | 警告状态（黄色系） |
| `Error` | 错误状态（红色系） |

### SizeType（来自 `AtomUI.Controls`）

尺寸类型枚举。

| 值 | 说明 |
|---|---|
| `Small` | 小号 |
| `Middle` | 中号（默认） |
| `Large` | 大号 |

---

## TimePicker 公共属性（StyledProperty）

### TimePicker 特有属性

| 属性名 | 类型 | 默认值 | 说明 |
|---|---|---|---|
| `SelectedTime` | `TimeSpan?` | `null` | 当前选中的时间值，支持双向绑定和数据验证 |
| `DefaultTime` | `TimeSpan?` | `null` | 默认时间值。控件加载时若 `SelectedTime` 为 null 则自动设置为该值 |
| `ClockIdentifier` | `ClockIdentifierType` | `HourClock24` | 时钟格式，12 小时制或 24 小时制 |
| `MinuteIncrement` | `int` | `1` | 分钟列步进值，有效范围 1–59 |
| `SecondIncrement` | `int` | `1` | 秒钟列步进值，有效范围 1–59 |
| `IsNeedConfirm` | `bool` | `false` | 是否启用确认模式（面板底部显示"确定"按钮） |
| `IsShowNow` | `bool` | `true` | 是否显示"此刻"快捷按钮 |

### 继承自 InfoPickerInput 的属性

| 属性名 | 类型 | 默认值 | 说明 |
|---|---|---|---|
| `SizeType` | `SizeType` | `SizeType.Middle` | 控件尺寸（共享属性） |
| `StyleVariant` | `InputControlStyleVariant` | `Outline` | 样式变体（Outline / Filled / Borderless） |
| `Status` | `InputControlStatus` | `Default` | 验证状态（Default / Warning / Error） |
| `PlaceholderText` | `string?` | `null` | 占位提示文本 |
| `InfoIcon` | `PathIcon?` | `ClockCircleOutlined` | 右侧图标（默认为时钟图标） |
| `PickerPlacement` | `PlacementMode` | `BottomEdgeAlignedLeft` | 弹出面板的放置位置 |
| `IsShowArrow` | `bool` | `false` | 弹出面板是否显示箭头 |
| `IsPointAtCenter` | `bool` | `false` | 弹出面板箭头是否指向中心 |
| `MarginToAnchor` | `double` | 由系统决定 | 弹出面板与锚点的间距 |
| `MouseEnterDelay` | `int` | 由系统决定 | 鼠标进入延迟（毫秒） |
| `MouseLeaveDelay` | `int` | 由系统决定 | 鼠标离开延迟（毫秒） |
| `IsReadOnly` | `bool` | `false` | 输入框是否只读 |
| `IsMotionEnabled` | `bool` | 跟随全局 Token | 是否启用过渡动画（共享属性） |
| `LeftAddOn` | `object?` | `null` | 输入框左侧附加内容 |
| `RightAddOn` | `object?` | `null` | 输入框右侧附加内容 |
| `ContentLeftAddOn` | `object?` | `null` | 内容区左侧附加内容 |
| `ContentRightAddOn` | `object?` | `null` | 内容区右侧附加内容 |

### 继承自 TemplatedControl 的常用属性

| 属性名 | 类型 | 默认值 | 说明 |
|---|---|---|---|
| `IsEnabled` | `bool` | `true` | 是否启用 |
| `FontSize` | `double` | 由 Token 控制 | 字体大小 |
| `FontFamily` | `FontFamily` | 由 Token 控制 | 字体族 |
| `Width` | `double` | `NaN`（自动） | 控件宽度 |
| `MinWidth` | `double` | `NaN` | 最小宽度 |
| `MaxWidth` | `double` | `NaN` | 最大宽度 |

---

## RangeTimePicker 公共属性（StyledProperty）

### RangeTimePicker 特有属性

| 属性名 | 类型 | 默认值 | 说明 |
|---|---|---|---|
| `RangeStartSelectedTime` | `TimeSpan?` | `null` | 起始时间值，支持双向绑定和数据验证 |
| `RangeEndSelectedTime` | `TimeSpan?` | `null` | 结束时间值，支持双向绑定和数据验证 |
| `RangeStartDefaultTime` | `TimeSpan?` | `null` | 起始时间默认值 |
| `RangeEndDefaultTime` | `TimeSpan?` | `null` | 结束时间默认值 |
| `MinuteIncrement` | `int` | `1` | 分钟列步进值，有效范围 1–59 |
| `SecondIncrement` | `int` | `1` | 秒钟列步进值，有效范围 1–59 |
| `ClockIdentifier` | `ClockIdentifierType` | `HourClock24` | 时钟格式 |
| `IsNeedConfirm` | `bool` | `false` | 是否启用确认模式 |
| `IsShowNow` | `bool` | `true` | 是否显示"此刻"快捷按钮 |

### 继承自 RangeInfoPickerInput 的属性

| 属性名 | 类型 | 默认值 | 说明 |
|---|---|---|---|
| `SecondaryPlaceholderText` | `string?` | `null` | 结束时间输入框的占位文本 |

> 其余属性与 TimePicker 继承自 `InfoPickerInput` 的属性一致。

---

## 公共方法

### TimePicker

| 方法名 | 返回类型 | 说明 |
|---|---|---|
| `Clear()` | `void` | 将 `SelectedTime` 设为 `null`，不考虑默认值 |
| `Reset()` | `void` | 将 `SelectedTime` 重置为 `DefaultTime` |
| `ClosePickerFlyout()` | `void` | 关闭弹出面板（继承自 `InfoPickerInput`） |

### RangeTimePicker

| 方法名 | 返回类型 | 说明 |
|---|---|---|
| `Clear()` | `void` | 将 `RangeStartSelectedTime` 和 `RangeEndSelectedTime` 都设为 `null` |
| `Reset()` | `void` | 将起止时间重置为各自的默认值 |
| `ClosePickerFlyout()` | `void` | 关闭弹出面板（继承自 `InfoPickerInput`） |

---

## 伪类（Pseudo-Classes）

TimePicker 和 RangeTimePicker 继承自 `InfoPickerInput`，支持以下伪类：

| 伪类 | 常量 | 触发条件 |
|---|---|---|
| `:flyout-open` | `InfoPickerPseudoClass.FlyoutOpen` | 弹出面板处于打开状态 |
| `:choosing` | `InfoPickerPseudoClass.Choosing` | 用户正在弹出面板中选择时间 |

### 继承的标准伪类

| 伪类 | 触发条件 |
|---|---|
| `:pointerover` | 鼠标悬浮在控件上 |
| `:disabled` | `IsEnabled == false` |
| `:focus` | 获得焦点 |
| `:focus-visible` | 通过键盘（Tab）获得焦点 |

---

## 实现的接口

### ISizeTypeAware

```csharp
public SizeType SizeType { get; set; }
```

### IInputControlStyleVariantAware

```csharp
public InputControlStyleVariant StyleVariant { get; set; }
```

### IInputControlStatusAware

```csharp
public InputControlStatus Status { get; set; }
```

### IMotionAwareControl

```csharp
public bool IsMotionEnabled { get; set; }
```

### ICompactSpaceAware

在 `Space.Compact` 容器中使用时自动调整圆角和边框。

### IFormItemAware

可作为 `FormItem` 的子控件参与表单验证流程。TimePicker 的表单值为 `TimeSpan?`，RangeTimePicker 的表单值为 `(TimeSpan?, TimeSpan?)`。

---

## 国际化资源

TimePicker 通过 `LanguageProvider` 系统提供以下可本地化字符串：

| 资源键 | 英文（en_US） | 中文（zh_CN） | 用途 |
|---|---|---|---|
| `AMText` | `"AM"` | `"上午"` | 12 小时制上午标识 |
| `PMText` | `"PM"` | `"下午"` | 12 小时制下午标识 |
| `Now` | `"Now"` | `"现在"` | "此刻"快捷按钮文案 |

在 AXAML 中引用：`{atom:TimePickerLangResource AMText}`

---

## 键盘交互

弹出面板（TimePickerPresenter）支持以下键盘操作：

| 按键 | 行为 |
|---|---|
| `Escape` | 关闭面板（不应用选择） |
| `Enter` | 确认选择并关闭面板 |
| `Tab` | 在面板内的时/分/秒列之间切换焦点 |
