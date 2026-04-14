# ToggleIconButton API 参考

## 命名空间

```csharp
namespace AtomUI.Desktop.Controls;
```

AXAML 命名空间前缀：`atom`（`xmlns:atom="https://atomui.net"`）

---

## 公共属性（StyledProperty）

| 属性名 | 类型 | 默认值 | 说明 |
|---|---|---|---|
| `CheckedIcon` | `PathIcon?` | `null` | 选中状态（`IsChecked = true`）时显示的图标 |
| `UnCheckedIcon` | `PathIcon?` | `null` | 未选中状态（`IsChecked = false`）时显示的图标 |
| `IconWidth` | `double` | 由 Token 控制（`IconSizeSM`） | 图标渲染宽度 |
| `IconHeight` | `double` | 由 Token 控制（`IconSizeSM`） | 图标渲染高度 |
| `IconBrush` | `IBrush?` | 由主题控制（`ColorIcon`） | 当前生效的图标颜色，由主题根据状态自动切换 |
| `NormalIconBrush` | `IBrush?` | `null` | 未选中正常态图标颜色（设置后覆盖主题默认值） |
| `ActiveIconBrush` | `IBrush?` | `null` | 悬浮/按下态图标颜色（设置后覆盖主题默认值） |
| `SelectedIconBrush` | `IBrush?` | `null` | 选中态图标颜色（设置后覆盖主题默认值） |
| `DisabledIconBrush` | `IBrush?` | `null` | 禁用态图标颜色（设置后覆盖主题默认值） |
| `IsMotionEnabled` | `bool` | 跟随全局 Token（`EnableMotion`） | 是否启用过渡动画（共享属性，通过 `AddOwner` 注册） |

### 继承自 Avalonia.Controls.Primitives.ToggleButton 的常用属性

| 属性名 | 类型 | 默认值 | 说明 |
|---|---|---|---|
| `IsChecked` | `bool?` | `false` | 切换状态：`true`（选中）、`false`（未选中）、`null`（不确定，当 `IsThreeState=true` 时） |
| `IsThreeState` | `bool` | `false` | 是否支持三态切换（`true` → `null` → `false` → `true`） |
| `Command` | `ICommand?` | `null` | 点击命令（MVVM 绑定） |
| `CommandParameter` | `object?` | `null` | 命令参数 |
| `ClickMode` | `ClickMode` | `Release` | 点击触发时机：`Release`（指针释放时）或 `Press`（指针按下时） |
| `IsEnabled` | `bool` | `true` | 是否启用 |
| `IsPressed` | `bool` | `false`（只读） | 是否处于按压状态 |
| `Background` | `IBrush?` | `Transparent` | 背景色（默认透明） |
| `BorderBrush` | `IBrush?` | 由主题控制 | 边框颜色 |
| `BorderThickness` | `Thickness` | 由主题控制 | 边框粗细 |
| `CornerRadius` | `CornerRadius` | 由 Token 控制（`BorderRadiusSM`） | 圆角半径 |
| `Cursor` | `Cursor` | `Hand` | 鼠标光标 |

---

## 事件

| 事件名 | 类型 | 说明 |
|---|---|---|
| `Click` | `EventHandler<RoutedEventArgs>` | 按钮点击事件（继承自 `Button`） |
| `IsCheckedChanged` | `EventHandler<RoutedEventArgs>` | `IsChecked` 状态变更事件（继承自 `ToggleButton`） |

---

## 伪类（Pseudo-Classes）

ToggleIconButton 支持以下伪类，可在样式选择器中使用：

### 继承自 ToggleButton 的伪类

| 伪类 | 触发条件 |
|---|---|
| `:checked` | `IsChecked == true`（选中状态） |
| `:unchecked` | `IsChecked == false`（未选中状态） |
| `:indeterminate` | `IsChecked == null`（三态模式下的不确定状态） |

### 继承的标准伪类

| 伪类 | 触发条件 |
|---|---|
| `:pointerover` | 鼠标悬浮 |
| `:pressed` | 按钮按下 |
| `:disabled` | `IsEnabled == false` |
| `:focus` | 获得焦点 |
| `:focus-visible` | 通过键盘（Tab 键）获得焦点 |

---

## 实现的接口

### IMotionAwareControl

```csharp
public bool IsMotionEnabled { get; set; }
```

控制是否启用过渡动画。启用时，`Background` 和 `RenderTransform` 属性变化会使用平滑过渡效果。

---

## 键盘交互

| 按键 | 行为 |
|---|---|
| `Space` | 切换 `IsChecked` 状态（继承自 ToggleButton） |
| `Enter` | 触发 Click 事件（继承自 Button） |

---

## 静态构造器影响

以下属性变更会触发重新测量（`AffectsMeasure`）：

- `CheckedIcon`、`UnCheckedIcon`、`IsChecked`、`IconWidth`、`IconHeight`

以下属性变更会触发重新渲染（`AffectsRender`）：

- `IconBrush`
