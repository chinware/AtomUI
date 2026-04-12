# ComboBox API 参考

## 命名空间

```csharp
namespace AtomUI.Desktop.Controls;
```

AXAML 命名空间前缀：`atom`（`xmlns:atom="https://atomui.net"`）

---

## 枚举类型

### InputControlStyleVariant

样式变体枚举，定义控件的视觉风格。

| 值 | 说明 |
|---|---|
| `Outline` | 描边样式（默认），白色背景 + 实线边框 |
| `Filled` | 填充样式，灰色填充背景 |
| `Borderless` | 无边框样式，无背景无边框 |

### InputControlStatus

输入控件验证状态枚举。

| 值 | 说明 |
|---|---|
| `Default` | 默认状态（无验证反馈） |
| `Error` | 错误状态，红色系视觉反馈 |
| `Warning` | 警告状态，橙色系视觉反馈 |

### SizeType（来自 `AtomUI.Controls`）

尺寸类型枚举。

| 值 | 说明 |
|---|---|
| `Small` | 小号（24px） |
| `Middle` | 中号（默认，32px） |
| `Large` | 大号（40px） |

---

## 公共属性（StyledProperty）

### AddOn 扩展区域属性

| 属性名 | 类型 | 默认值 | 说明 |
|---|---|---|---|
| `LeftAddOn` | `object?` | `null` | 前置标签内容（选择器外部左侧），支持文本或控件 |
| `LeftAddOnTemplate` | `IDataTemplate?` | `null` | 前置标签的数据模板 |
| `RightAddOn` | `object?` | `null` | 后置标签内容（选择器外部右侧），支持文本或控件 |
| `RightAddOnTemplate` | `IDataTemplate?` | `null` | 后置标签的数据模板 |
| `ContentLeftAddOn` | `object?` | `null` | 内部前缀内容（内容区域左侧），常用于图标 |
| `ContentLeftAddOnTemplate` | `IDataTemplate?` | `null` | 内部前缀的数据模板 |
| `ContentRightAddOn` | `object?` | `null` | 内部后缀内容（内容区域右侧，手柄左侧） |
| `ContentRightAddOnTemplate` | `IDataTemplate?` | `null` | 内部后缀的数据模板 |

### 外观与行为属性

| 属性名 | 类型 | 默认值 | 说明 |
|---|---|---|---|
| `SizeType` | `SizeType` | `SizeType.Middle` | 控件尺寸（共享属性，通过 `AddOwner` 注册） |
| `StyleVariant` | `InputControlStyleVariant` | `InputControlStyleVariant.Outline` | 样式变体（共享属性） |
| `Status` | `InputControlStatus` | `InputControlStatus.Default` | 验证状态（共享属性） |
| `IsAllowClear` | `bool` | `false` | 是否允许清除已选值 |
| `OptionFontSize` | `double` | 由 Token 控制 | 下拉选项列表的字体大小 |
| `IsMotionEnabled` | `bool` | 跟随全局 Token | 是否启用过渡动画（共享属性） |
| `DropDownDisplayPageSize` | `int` | `10` | 下拉面板最多可见选项数量，超出部分滚动显示 |

### 继承自 Avalonia.Controls.ComboBox 的常用属性

| 属性名 | 类型 | 默认值 | 说明 |
|---|---|---|---|
| `ItemsSource` | `IEnumerable?` | `null` | 数据源集合，支持绑定任意集合 |
| `ItemTemplate` | `IDataTemplate?` | `null` | 项目数据模板，定义每个选项的呈现方式 |
| `SelectedItem` | `object?` | `null` | 当前选中项，支持双向绑定 |
| `SelectedIndex` | `int` | `-1` | 当前选中项索引 |
| `PlaceholderText` | `string?` | `null` | 未选中时显示的占位提示文本 |
| `IsDropDownOpen` | `bool` | `false` | 下拉面板是否打开 |
| `MaxDropDownHeight` | `double` | 自动计算 | 下拉面板最大高度（由 `DropDownDisplayPageSize` 自动派生） |
| `ItemsPanel` | `ItemsPanelTemplate` | `VirtualizingStackPanel` | 项目面板模板 |
| `IsEnabled` | `bool` | `true` | 是否启用 |
| `Width` | `double` | `NaN` | 控件宽度 |

---

## 事件

| 事件名 | 类型 | 说明 |
|---|---|---|
| `SelectionChanged` | `EventHandler<SelectionChangedEventArgs>` | 选中项变更事件（继承自 `SelectingItemsControl`） |

---

## 伪类（Pseudo-Classes）

ComboBox 支持以下伪类，可在样式选择器中使用：

| 伪类 | 触发条件 |
|---|---|
| `:error` | `Status == InputControlStatus.Error` |
| `:warning` | `Status == InputControlStatus.Warning` |

### 继承的标准伪类

| 伪类 | 触发条件 |
|---|---|
| `:pointerover` | 鼠标悬浮 |
| `:pressed` | 控件被按下（非下拉打开状态下） |
| `:disabled` | `IsEnabled == false` |
| `:focus` | 获得焦点 |
| `:focus-visible` | 通过键盘获得焦点（显示焦点框） |

---

## 实现的接口

### IMotionAwareControl

```csharp
public bool IsMotionEnabled { get; set; }
```

控制下拉面板展开/收起动画、ComboBoxItem 背景色过渡动画的开关。

### ISizeTypeAware

```csharp
public SizeType SizeType { get; set; }
```

支持 `Small`（24px）、`Middle`（32px，默认）、`Large`（40px）三种尺寸。尺寸会同步传递给内部的 `AddOnDecoratedBox` 和 `ComboBoxItem`。

### IInputControlStatusAware

```csharp
public InputControlStatus Status { get; set; }
```

支持 `Default`、`Error`、`Warning` 三种验证状态。状态变更时自动更新对应伪类（`:error`、`:warning`）。

### IInputControlStyleVariantAware

```csharp
public InputControlStyleVariant StyleVariant { get; set; }
```

支持 `Outline`、`Filled`、`Borderless` 三种样式变体。变体会传递给内部的 `AddOnDecoratedBox`。

### IFormItemAware

可作为 `FormItem` 的子控件参与表单验证流程。ComboBox 提供以下行为：

- 选中项变更时通过 `ValueChanged` 事件通知 FormItem
- FormItem 验证状态（`FormValidateStatus`）自动映射为 `Status` 属性

### IFormItemFeedbackAware

可接收并显示表单验证反馈控件（`FormValidateFeedback`），反馈图标显示在下拉手柄左侧。

---

## 辅助控件

### ComboBoxItem

下拉选项容器，继承自 `Avalonia.Controls.ComboBoxItem`。

| 伪类 | 说明 |
|---|---|
| `:pointerover` | 鼠标悬浮在选项上 |
| `:selected` | 选项被选中 |
| `:disabled` | 选项被禁用 |

### ComboBoxHandle（内部控件）

下拉手柄组件，包含一个 `IconButton`（`DownOutlined` 图标）。

| 属性名 | 类型 | 说明 |
|---|---|---|
| `IsMotionEnabled` | `bool` | 是否启用动画 |

| 事件名 | 类型 | 说明 |
|---|---|---|
| `HandleClick` | `EventHandler<RoutedEventArgs>` | 手柄被点击 |
