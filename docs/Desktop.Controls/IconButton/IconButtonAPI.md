# IconButton API 参考

## 命名空间

- **C#**：`AtomUI.Desktop.Controls`（控件类）/ `AtomUI.Controls.Commons`（基类）
- **AXAML**：`xmlns:atom="https://atomui.net"`

---

## 枚举类型

### IconAnimation（来自 `AtomUI.Controls`）

图标加载动画枚举。

| 值 | 说明 |
|---|---|
| `None` | 无动画（默认） |
| `Spin` | 旋转动画 |

---

## 公共属性（StyledProperty）

### AbstractIconButton 基类层定义（设备无关）

| 属性名 | 类型 | 默认值 | 说明 |
|---|---|---|---|
| `Icon` | `PathIcon?` | `null` | 要展示的图标，使用 Ant Design 图标集 |
| `IconBrush` | `IBrush?` | `null`（由主题设置为 `ColorIcon`） | 图标画刷颜色，主题根据伪类自动切换；手动设置后将覆盖主题驱动的颜色变化 |
| `IconWidth` | `double` | `0`（由主题设置为 `SharedToken.IconSizeSM`） | 图标渲染宽度 |
| `IconHeight` | `double` | `0`（由主题设置为 `SharedToken.IconSizeSM`） | 图标渲染高度 |
| `LoadingAnimation` | `IconAnimation` | `IconAnimation.None` | 图标加载动画类型（如 `Spin` 旋转） |
| `LoadingAnimationDuration` | `TimeSpan` | 默认值 | 加载动画一次循环的持续时间 |
| `IsMotionEnabled` | `bool` | 由 `SharedToken.EnableMotion` 控制 | 是否启用过渡动画（`IMotionAwareControl`） |
| `IsPassthroughMouseEvent` | `bool` | `false` | 鼠标事件穿透开关。设为 `true` 时，`PointerPressed` / `PointerReleased` / `PointerMoved` 事件不会被标记为已处理，继续向父控件冒泡。用于嵌入其他控件内部时不阻断父控件交互 |

> ⚠️ `IsPassthroughMouseEvent` 的 `StyledProperty` 字段声明为 `internal`（仅在 AtomUI 程序集内可通过 AXAML 设置），但 CLR 属性访问器为 `public`（C# 代码中可从任何位置访问）。

### 主题控制的可覆盖属性

以下属性继承自 Avalonia 基类，由主题默认赋值，可通过 Style 或直接设置覆盖：

| 属性名 | 类型 | 主题默认值 | 说明 |
|---|---|---|---|
| `Background` | `IBrush?` | `Transparent` | 背景色 |
| `CornerRadius` | `CornerRadius` | `SharedToken.BorderRadiusSM` | 圆角半径 |
| `Cursor` | `Cursor` | `Hand` | 鼠标光标 |
| `Padding` | `Thickness` | 未设置 | 内间距 |
| `Foreground` | `IBrush?` | 由主题控制 | 前景色（IconButton 通常不使用，图标颜色通过 `IconBrush` 控制） |

### 继承自 Avalonia Button 的常用属性

| 属性名 | 类型 | 默认值 | 说明 |
|---|---|---|---|
| `Command` | `ICommand?` | `null` | MVVM 命令绑定，点击时调用 `ICommand.Execute`；`CanExecute` 返回 `false` 时自动禁用 |
| `CommandParameter` | `object?` | `null` | 命令参数 |
| `HotKey` | `KeyGesture?` | `null` | 键盘快捷键 |
| `ClickMode` | `ClickMode` | `Release` | 点击触发时机：`Release`（默认）或 `Press` |
| `Flyout` | `FlyoutBase?` | `null` | 附加弹出层，点击时自动展开 |
| `IsPressed` | `bool` | `false`（只读） | 是否处于按压状态 |
| `IsEnabled` | `bool` | `true` | 是否启用 |
| `Content` | `object?` | `null` | 按钮内容（继承自 `ContentControl`，但 **IconButton 模板中不使用** `ContentPresenter`，设置此属性无可见效果） |

---

## 事件

| 事件名 | 类型 | 说明 |
|---|---|---|
| `Click` | `EventHandler<RoutedEventArgs>` | 按钮被点击时触发（继承自 Avalonia Button） |

---

## 伪类（Pseudo-Classes）

IconButton 不定义自定义伪类，仅使用继承自 Avalonia Button 的标准伪类：

| 伪类 | 触发条件 |
|---|---|
| `:pointerover` | 鼠标悬浮在按钮上 |
| `:pressed` | 按钮被按下 |
| `:disabled` | `IsEnabled == false` |
| `:focus` | 按钮获得焦点 |
| `:focus-visible` | 通过键盘（Tab）获得焦点 |
| `:flyout-open` | 附加的 Flyout 处于打开状态 |

---

## 实现的接口

| 接口 | 定义位置 | 说明 |
|---|---|---|
| `IMotionAwareControl` | `AtomUI.Controls` | 支持 `IsMotionEnabled` 过渡动画开关 |

---

## 在其他控件中作为内部构件使用

IconButton 被 AtomUI 体系中的多个复合控件作为内部构件使用，以下是主要使用位置：

| 控件 | 模板部件名 | 用途 |
|---|---|---|
| `Tag` | `PART_CloseButton` | 标签的关闭按钮 |
| `Alert` | `PART_CloseBtn` | 警告提示的关闭按钮 |
| `NotificationCard` | 关闭按钮 | 通知卡片的关闭按钮 |
| `TabItem` / `TabStripItem` | `PART_ItemCloseButton` | 选项卡的关闭按钮 |
| `TabControl` | 添加/滚动按钮 | 选项卡栏的添加和滚动指示按钮 |
| `CalendarItem` | 导航按钮 | 日历的前/后月份、年份导航按钮 |
| `Drawer` | `PART_CloseButton` | 抽屉的关闭按钮 |
| `OverlayDialog` | `PART_CloseButton` | 对话框的关闭按钮 |
| `Upload` | 删除/预览按钮 | 上传列表项的删除和预览操作按钮 |
| `NumericUpDown` | 步进按钮 | 数字输入框的增减按钮 |
| `TextBox` | `PART_ClearButton` | 输入框的清除按钮 |

