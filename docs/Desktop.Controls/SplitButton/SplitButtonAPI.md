# SplitButton API 参考

## 命名空间

```csharp
namespace AtomUI.Desktop.Controls;
```

AXAML 命名空间前缀：`atom`（`xmlns:atom="https://atomui.net"`）

---

## 枚举类型

### SizeType（来自 `AtomUI.Controls`）

尺寸类型枚举。

| 值 | 说明 |
|---|---|
| `Small` | 小号 |
| `Middle` | 中号（默认） |
| `Large` | 大号 |

### FlyoutTriggerType（来自 `AtomUI.Controls`）

弹出菜单触发方式枚举。

| 值 | 说明 |
|---|---|
| `Click` | 点击触发（默认） |
| `Hover` | 鼠标悬浮触发 |

### PlacementMode（来自 `Avalonia.Controls.Primitives.PopupPositioning`）

弹出层位置模式枚举。SplitButton 默认使用 `BottomEdgeAlignedRight`。

| 常用值 | 说明 |
|---|---|
| `Bottom` | 下方居中 |
| `BottomEdgeAlignedLeft` | 下方左对齐 |
| `BottomEdgeAlignedRight` | 下方右对齐（SplitButton 默认值） |
| `Top` | 上方居中 |
| `TopEdgeAlignedLeft` | 上方左对齐 |
| `TopEdgeAlignedRight` | 上方右对齐 |

---

## 公共属性（StyledProperty）

### SplitButton 自身属性

| 属性名 | 类型 | 默认值 | 说明 |
|---|---|---|---|
| `Flyout` | `Flyout?` | `null` | 下拉菜单弹出层（支持 `MenuFlyout`），由右侧按钮触发 |
| `TriggerType` | `FlyoutTriggerType` | `Click` | Flyout 触发方式（`Click` / `Hover`） |
| `IsShowArrow` | `bool` | `false` | 弹出菜单是否显示指示箭头 |
| `IsPointAtCenter` | `bool` | `false` | 箭头是否指向触发元素中心 |
| `Placement` | `PlacementMode` | `BottomEdgeAlignedRight` | Flyout 弹出位置 |
| `PlacementAnchor` | `PopupAnchor` | 默认值 | Flyout 弹出锚点 |
| `PlacementGravity` | `PopupGravity` | 默认值 | Flyout 弹出重力方向 |
| `GutterToFlyout` | `double` | 由 Token 控制 | 弹出层与按钮之间的间距 |
| `MouseEnterDelay` | `int` | 默认值 | 悬浮触发延迟（毫秒），仅 `TriggerType=Hover` 时生效 |
| `MouseLeaveDelay` | `int` | 默认值 | 离开关闭延迟（毫秒），仅 `TriggerType=Hover` 时生效 |
| `IsPrimaryButtonType` | `bool` | `false` | 是否使用 Primary 按钮样式（蓝色背景），`false` 时使用 Default 样式（白色背景 + 边框） |
| `IsDanger` | `bool` | `false` | 危险按钮样式，启用后两个内部按钮使用红色系 |
| `Icon` | `PathIcon?` | `null` | 主按钮图标，使用 Ant Design 图标集 |
| `OpenIndicator` | `PathIcon?` | `EllipsisOutlined` | 下拉按钮图标，默认为省略号图标 |
| `SizeType` | `SizeType` | `SizeType.Middle` | 按钮尺寸（共享属性，通过 `AddOwner` 注册） |
| `IsMotionEnabled` | `bool` | 跟随全局 Token | 是否启用过渡动画（共享属性） |
| `IsWaveSpiritEnabled` | `bool` | 跟随全局 Token | 是否启用点击波纹效果（共享属性） |

### ICommandSource 接口属性

| 属性名 | 类型 | 默认值 | 说明 |
|---|---|---|---|
| `Command` | `ICommand?` | `null` | 主按钮点击命令，`CanExecute` 为 `false` 时自动禁用整个 SplitButton |
| `CommandParameter` | `object?` | `null` | 命令参数 |
| `HotKey` | `KeyGesture?` | `null` | 键盘快捷键，触发主按钮点击 |

### 继承自 Avalonia.Controls.ContentControl 的常用属性

| 属性名 | 类型 | 默认值 | 说明 |
|---|---|---|---|
| `Content` | `object?` | `null` | 主按钮文本内容 |
| `ContentTemplate` | `IDataTemplate?` | `null` | 内容模板 |
| `HorizontalAlignment` | `HorizontalAlignment` | `Left` | 水平对齐 |
| `VerticalAlignment` | `VerticalAlignment` | `Top` | 垂直对齐 |
| `IsEnabled` | `bool` | `true` | 是否启用（同时受 `Command.CanExecute` 控制） |
| `Foreground` | `IBrush?` | 由主题控制 | 前景色 |
| `Background` | `IBrush?` | 由主题控制 | 背景色 |
| `BorderBrush` | `IBrush?` | 由 Token 控制 | 边框颜色（`SharedToken.ColorBorder`） |
| `BorderThickness` | `Thickness` | 由 Token 控制 | 边框粗细（`SharedToken.BorderThickness`） |
| `CornerRadius` | `CornerRadius` | 由 Token 控制 | 圆角半径（根据 `SizeType` 自动设置） |

---

## 事件

| 事件名 | 类型 | 说明 |
|---|---|---|
| `Click` | `EventHandler<RoutedEventArgs>` | 主按钮（左侧）被点击时触发，路由策略为 `Bubble` |

> **重要提示**：`Click` 事件仅在点击左侧主按钮时触发。点击右侧下拉按钮会打开 Flyout，不会触发 `Click` 事件。内部主按钮的 `Click` 事件会被标记为 `Handled`，防止冒泡到外部。

---

## 伪类（Pseudo-Classes）

SplitButton 支持以下伪类，可在样式选择器中使用：

| 伪类 | 触发条件 |
|---|---|
| `:flyout-open` | Flyout 处于打开状态 |
| `:pressed` | 主按钮被按下（键盘 Space/Enter 触发） |
| `:checked` | 选中状态（用于 `ToggleSplitButton` 派生类） |
| `:disabled` | `IsEnabled == false` 或 `Command.CanExecute == false` |
| `:pointerover` | 鼠标悬浮在控件上 |
| `:focus` | 获得焦点 |
| `:focus-visible` | 通过键盘（Tab 键）获得焦点 |

---

## 实现的接口

### ICommandSource

```csharp
public ICommand? Command { get; set; }
public object? CommandParameter { get; set; }
```

支持 MVVM 命令绑定。当 `Command.CanExecute` 返回 `false` 时，SplitButton 自动禁用（灰色调 + 不可点击）。

### ISizeTypeAware

```csharp
public SizeType SizeType { get; set; }
```

支持 Large / Middle / Small 三种尺寸，两个内部 Button 同步尺寸。

### IWaveSpiritAwareControl

```csharp
public bool IsWaveSpiritEnabled { get; set; }
```

控制内部 Button 的点击波纹效果开关。

### ICompactSpaceAware

在 `Space.Compact` 容器中使用时自动调整圆角。SplitButton 提供以下行为：

- 根据在紧凑空间中的位置自动裁剪外部圆角
- 将裁剪后的圆角重新分配给左右两个内部 Button
- 返回边框厚度供容器计算相邻控件间距

---

## 键盘交互

| 按键 | 行为 |
|---|---|
| `Space` / `Enter` | 触发主按钮点击（`Click` 事件 + `Command` 执行） |
| `Alt+Down` | 打开 Flyout（需非 XY 导航模式） |
| `F4` | 打开 Flyout |
| `Escape` | 关闭 Flyout（当 Flyout 无可聚焦内容时） |

---

## 内部属性（Internal）

以下属性为内部使用，不作为公共 API：

| 属性名 | 类型 | 说明 |
|---|---|---|
| `SplitSeparatorBrush` | `IBrush?` | Primary 模式下分隔线画刷 |
| `EffectiveButtonType` | `ButtonType` | 内部 Button 的实际类型（由 `IsPrimaryButtonType` 驱动） |
| `CompactSpaceItemPosition` | `SpaceItemPosition?` | 紧凑空间中的位置 |
| `CompactSpaceOrientation` | `Orientation` | 紧凑空间方向 |
| `IsUsedInCompactSpace` | `bool` | 是否处于紧凑空间中 |
