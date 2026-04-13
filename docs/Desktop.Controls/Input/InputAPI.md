# Input API 参考

## 命名空间

```csharp
namespace AtomUI.Desktop.Controls;
```

AXAML 命名空间前缀：`atom`（`xmlns:atom="https://atomui.net"`）

---

## 枚举类型

### InputControlStyleVariant（来自 `AtomUI.Controls`）

输入框样式变体枚举。

| 值 | 说明 |
|---|---|
| `Outline` | 带边框的标准样式（默认） |
| `Filled` | 填充背景的样式 |
| `Borderless` | 无边框样式 |
| `Underlined` | 下划线样式 |

### InputControlStatus（来自 `AtomUI.Controls`）

输入框验证状态枚举。

| 值 | 说明 |
|---|---|
| `Default` | 默认状态 |
| `Warning` | 警告状态（橙色边框） |
| `Error` | 错误状态（红色边框） |

### SearchEditButtonStyle

搜索按钮样式枚举。

| 值 | 说明 |
|---|---|
| `Default` | 默认搜索按钮（仅图标） |
| `Primary` | 主色调搜索按钮 |

### SizeType（来自 `AtomUI.Controls`）

尺寸类型枚举。

| 值 | 说明 |
|---|---|
| `Small` | 小号 |
| `Middle` | 中号（默认） |
| `Large` | 大号 |

---

## TextBox（基类）

`AtomUI.Desktop.Controls.TextBox` 是 Input 控件族的底层基类，继承自 `Avalonia.Controls.TextBox`，通过 `using AvaloniaTextBox = Avalonia.Controls.TextBox;` 别名引用避免类名冲突。

### 公共属性（StyledProperty）

| 属性名 | 类型 | 默认值 | 说明 |
|---|---|---|---|
| `SizeType` | `SizeType` | `SizeType.Middle` | 输入框尺寸（共享属性，通过 `AddOwner` 注册） |
| `IsAllowClear` | `bool` | `false` | 是否允许清除内容（显示清除按钮） |
| `IsEnableRevealButton` | `bool` | `false` | 是否启用密码可见性切换按钮 |
| `IsCustomFontSize` | `bool` | `false` | 是否使用自定义字体大小（为 `true` 时不自动设置 Token 字号） |
| `IsShowCount` | `bool` | `false` | 是否显示字符计数（格式：`已输入 / 最大`） |
| `ClearIcon` | `PathIcon?` | `CloseCircleFilled` | 清除按钮图标 |
| `PlaceholderText` | `string?` | `null` | 占位提示文本（替代 Avalonia 的 `Watermark`） |
| `PlaceholderForeground` | `IBrush?` | 由主题控制 | 占位文本前景色 |
| `IsMotionEnabled` | `bool` | 跟随全局 Token | 是否启用过渡动画（共享属性） |

### 继承自 Avalonia.Controls.TextBox 的常用属性

| 属性名 | 类型 | 默认值 | 说明 |
|---|---|---|---|
| `Text` | `string?` | `null` | 输入框文本内容（支持双向绑定） |
| `MaxLength` | `int` | `0`（无限制） | 最大输入字符数 |
| `IsReadOnly` | `bool` | `false` | 是否只读 |
| `PasswordChar` | `char` | `'\0'` | 密码掩码字符（设置后启用密码模式） |
| `RevealPassword` | `bool` | `false` | 是否显示密码明文 |
| `AcceptsReturn` | `bool` | `false` | 是否接受回车换行 |
| `AcceptsTab` | `bool` | `false` | 是否接受 Tab 字符 |
| `CaretIndex` | `int` | `0` | 光标位置 |
| `SelectionStart` | `int` | `0` | 选区起始位置 |
| `SelectionEnd` | `int` | `0` | 选区结束位置 |
| `SelectionBrush` | `IBrush?` | 由主题控制 | 选区背景色 |
| `SelectionForegroundBrush` | `IBrush?` | 由主题控制 | 选区文本色 |
| `CaretBrush` | `IBrush?` | 由主题控制 | 光标颜色 |
| `HorizontalContentAlignment` | `HorizontalAlignment` | — | 内容水平对齐 |
| `VerticalContentAlignment` | `VerticalAlignment` | `Center` | 内容垂直对齐 |
| `IsEnabled` | `bool` | `true` | 是否启用 |
| `Foreground` | `IBrush?` | 由主题控制 | 前景色（文本颜色） |
| `Background` | `IBrush?` | `Transparent` | 背景色 |
| `BorderBrush` | `IBrush?` | 由主题控制 | 边框颜色 |
| `BorderThickness` | `Thickness` | 由 Token 控制 | 边框粗细 |
| `CornerRadius` | `CornerRadius` | 由 Token 控制 | 圆角半径 |
| `FontSize` | `double` | 由 Token 控制 | 字体大小 |
| `Padding` | `Thickness` | 由 Token 控制 | 内间距 |

### 实现的接口

| 接口 | 说明 |
|---|---|
| `ISizeTypeAware` | 支持 `SizeType`（Small / Middle / Large）三种尺寸切换 |
| `IMotionAwareControl` | 支持动画开关 |
| `ICompactSpaceAware` | 在 `Space.Compact` 中使用时自动调整圆角和边框 |
| `IFormItemAware` | 可作为 `FormItem` 的子控件参与表单验证流程 |
| `IFormItemFeedbackAware` | 可接收并显示表单验证反馈控件 |

---

## LineEdit

`AtomUI.Desktop.Controls.LineEdit` 继承自 `AtomUI.Desktop.Controls.TextBox`，是 Input 控件族对标 Ant Design `Input` 的核心控件。

### 公共属性（StyledProperty）

| 属性名 | 类型 | 默认值 | 说明 |
|---|---|---|---|
| `StyleVariant` | `InputControlStyleVariant` | `Outline` | 样式变体（共享属性，通过 `AddOwner` 注册） |
| `Status` | `InputControlStatus` | `Default` | 验证状态（共享属性，通过 `AddOwner` 注册） |
| `LeftAddOn` | `object?` | `null` | 左侧附加组件内容（如 `"http://"`） |
| `LeftAddOnTemplate` | `IDataTemplate?` | `null` | 左侧附加组件数据模板 |
| `RightAddOn` | `object?` | `null` | 右侧附加组件内容（如 `".com"`） |
| `RightAddOnTemplate` | `IDataTemplate?` | `null` | 右侧附加组件数据模板 |
| `InnerLeftContentTemplate` | `IDataTemplate?` | `null` | 输入框内侧左装饰元素模板（如用户图标） |
| `InnerRightContentTemplate` | `IDataTemplate?` | `null` | 输入框内侧右装饰元素模板 |

> 💡 `LeftAddOn` 和 `RightAddOn` 属性使用 `[DependsOn]` 特性标记对相应模板属性的依赖关系。

### 额外实现的接口

| 接口 | 说明 |
|---|---|
| `IInputControlStatusAware` | 支持验证状态反馈（Error / Warning） |
| `IInputControlStyleVariantAware` | 支持样式变体切换（Outline / Filled / Borderless / Underlined） |

### 伪类（Pseudo-Classes）

| 伪类 | 常量 | 触发条件 |
|---|---|---|
| `:error` | `StdPseudoClass.Error` | `Status == InputControlStatus.Error` |
| `:warning` | `StdPseudoClass.Warning` | `Status == InputControlStatus.Warning` |
| `:outline` | `AddOnDecoratedBoxPseudoClass.Outline` | `StyleVariant == Outline` |
| `:filled` | `AddOnDecoratedBoxPseudoClass.Filled` | `StyleVariant == Filled` |
| `:borderless` | `AddOnDecoratedBoxPseudoClass.Borderless` | `StyleVariant == Borderless` |

### 继承的标准伪类

| 伪类 | 触发条件 |
|---|---|
| `:pointerover` | 鼠标悬浮 |
| `:focus` | 获得焦点 |
| `:focus-visible` | 通过键盘获得焦点 |
| `:disabled` | `IsEnabled == false` |
| `:empty` | 文本为空 |

---

## SearchEdit

`AtomUI.Desktop.Controls.SearchEdit` 继承自 `LineEdit`，对标 Ant Design `Input.Search`。

### 公共属性（StyledProperty）

| 属性名 | 类型 | 默认值 | 说明 |
|---|---|---|---|
| `SearchButtonStyle` | `SearchEditButtonStyle` | `Default` | 搜索按钮样式（Default 为仅图标，Primary 为主色调按钮） |
| `SearchButtonText` | `string` | `""` | 搜索按钮文字（为空时仅显示搜索图标） |
| `IsOperating` | `bool` | `false` | 是否处于加载状态（显示加载动画，阻止搜索按钮点击） |

### 事件

| 事件名 | 类型 | 路由策略 | 说明 |
|---|---|---|---|
| `SearchButtonClick` | `EventHandler<RoutedEventArgs>` | `Bubble` | 搜索按钮点击事件 |

### 使用说明

- SearchEdit 继承了 LineEdit 和 TextBox 的所有属性、伪类和接口。
- 当 `IsOperating == true` 时，`SearchButtonClick` 事件不会触发（防止重复操作）。
- SearchEdit 初始化时会自动设置 `ClearIcon` 为 `CloseCircleFilled`。

---

## TextArea

`AtomUI.Desktop.Controls.TextArea` 独立继承自 `Avalonia.Controls.TextBox`（不经过 AtomUI TextBox），对标 Ant Design `Input.TextArea`。

### 公共属性（StyledProperty）

| 属性名 | 类型 | 默认值 | 说明 |
|---|---|---|---|
| `ClearIcon` | `PathIcon?` | `null` | 清除按钮图标 |
| `Lines` | `int` | `2` | 显示行数（自动计算高度） |
| `IsAutoSize` | `bool` | `false` | 是否根据内容自动调整高度 |
| `IsShowCount` | `bool` | `false` | 是否显示字符计数 |
| `IsResizable` | `bool` | `false` | 是否可拖拽调整高度（右下角显示拖拽手柄） |
| `StyleVariant` | `InputControlStyleVariant` | `Outline` | 样式变体 |
| `Status` | `InputControlStatus` | `Default` | 验证状态 |
| `SizeType` | `SizeType` | `Middle` | 尺寸类型 |
| `IsAllowClear` | `bool` | `false` | 是否允许清除内容 |
| `PlaceholderText` | `string?` | `null` | 占位提示文本 |
| `PlaceholderForeground` | `IBrush?` | 由主题控制 | 占位文本前景色 |
| `IsMotionEnabled` | `bool` | 跟随全局 Token | 是否启用过渡动画 |
| `InnerLeftContentTemplate` | `IDataTemplate?` | `null` | 输入框内侧左装饰模板 |
| `InnerRightContentTemplate` | `IDataTemplate?` | `null` | 输入框内侧右装饰模板 |

### 继承自 Avalonia TextBox 的重要属性

| 属性名 | 类型 | TextArea 默认值 | 说明 |
|---|---|---|---|
| `MinLines` | `int` | `1` | 最少显示行数 |
| `MaxLines` | `int` | `0`（无限制） | 最多显示行数 |
| `MaxLength` | `int` | `0`（无限制） | 最大输入字符数 |
| `TextWrapping` | `TextWrapping` | `Wrap` | 文本换行模式（TextArea 默认开启换行） |
| `AcceptsReturn` | `bool` | `true` | 是否接受回车换行（TextArea 默认开启） |

### 伪类

TextArea 支持与 LineEdit 相同的伪类：`:error`、`:warning`、`:outline`、`:filled`、`:borderless`、`:disabled`。

### 实现的接口

| 接口 | 说明 |
|---|---|
| `ISizeTypeAware` | 尺寸切换 |
| `IMotionAwareControl` | 动画开关 |
| `IInputControlStatusAware` | 验证状态反馈 |
| `IInputControlStyleVariantAware` | 样式变体切换 |
| `IFormItemAware` | 表单集成 |
| `IFormItemFeedbackAware` | 表单反馈 |

### Lines 属性约束

`Lines` 属性的值必须在 `MinLines` 和 `MaxLines`（如果已设置）之间，否则会抛出 `ArgumentOutOfRangeException`。
