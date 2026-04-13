# DropdownButton API 参考

## 命名空间

```csharp
namespace AtomUI.Desktop.Controls;
```

AXAML 命名空间前缀：`atom`（`xmlns:atom="https://atomui.net"`）

---

## 枚举类型

### FlyoutTriggerType

下拉菜单触发方式枚举。

| 值 | 说明 |
|---|---|
| `Click` | 点击触发弹出 |
| `Hover` | 悬浮触发弹出 |

### PlacementMode（常用值）

弹出层位置枚举，DropdownButton 默认使用 `BottomEdgeAlignedLeft`。

| 值 | 说明 |
|---|---|
| `BottomEdgeAlignedLeft` | 底部左对齐（默认） |
| `BottomEdgeAlignedRight` | 底部右对齐 |
| `Bottom` | 底部居中 |
| `TopEdgeAlignedLeft` | 顶部左对齐 |
| `TopEdgeAlignedRight` | 顶部右对齐 |
| `Top` | 顶部居中 |

---

## 公共属性（StyledProperty）

### DropdownButton 自身定义

| 属性名 | 类型 | 默认值 | 说明 |
|---|---|---|---|
| `DropdownFlyout` | `MenuFlyout?` | `null` | 下拉菜单弹出层，包含 `MenuItem` 菜单项 |
| `TriggerType` | `FlyoutTriggerType` | `Click` | 触发方式（`Click` / `Hover`） |
| `IsShowArrow` | `bool` | `false` | 是否在弹出菜单上显示指向触发元素的箭头 |
| `IsPointAtCenter` | `bool` | `false` | 箭头是否指向触发元素中心（需配合 `IsShowArrow=True`） |
| `Placement` | `PlacementMode` | `BottomEdgeAlignedLeft` | 弹出层位置 |
| `PlacementAnchor` | `PopupAnchor` | 默认值 | 弹出层锚点（精细控制） |
| `PlacementGravity` | `PopupGravity` | 默认值 | 弹出层重力方向（精细控制） |
| `MarginToAnchor` | `double` | 由 Token 控制 | 弹出层与锚点（按钮）的间距 |
| `MouseEnterDelay` | `int` | 默认值 | 悬浮触发延迟（毫秒），仅 `TriggerType=Hover` 时有效 |
| `MouseLeaveDelay` | `int` | 默认值 | 离开关闭延迟（毫秒），仅 `TriggerType=Hover` 时有效 |
| `IsShowOpenIndicator` | `bool` | `true` | 是否显示下拉指示器图标（按钮右侧的箭头） |
| `OpenIndicator` | `PathIcon?` | `DownOutlined` | 下拉指示器图标，默认为向下箭头（`DownOutlined`） |

### 继承自 Button 的常用属性

| 属性名 | 类型 | 默认值 | 说明 |
|---|---|---|---|
| `ButtonType` | `ButtonType` | `Default` | 按钮类型（Primary / Default / Dashed / Text / Link） |
| `SizeType` | `SizeType` | `Middle` | 按钮尺寸（Large / Middle / Small） |
| `Shape` | `ButtonShape` | `Default` | 按钮形状（Default / Round / Circle） |
| `IsDanger` | `bool` | `false` | 是否危险按钮样式 |
| `IsGhost` | `bool` | `false` | 幽灵按钮（背景透明） |
| `IsLoading` | `bool` | `false` | 加载中状态 |
| `Icon` | `PathIcon?` | `null` | 按钮图标 |
| `Content` | `object?` | `null` | 按钮文本内容 |
| `Command` | `ICommand?` | `null` | 点击命令（MVVM） |
| `CommandParameter` | `object?` | `null` | 命令参数 |
| `IsEnabled` | `bool` | `true` | 是否启用 |
| `IsMotionEnabled` | `bool` | 跟随全局 Token | 是否启用过渡动画 |
| `IsWaveSpiritEnabled` | `bool` | 跟随全局 Token | 是否启用点击波纹效果 |

---

## 事件

| 事件名 | 类型 | 路由策略 | 说明 |
|---|---|---|---|
| `MenuItemClicked` | `EventHandler<FlyoutMenuItemClickedEventArgs>` | Bubble | 弹出菜单中的菜单项被点击时触发 |
| `Click` | `EventHandler<RoutedEventArgs>` | Bubble | 按钮本身被点击时触发（继承自 Button） |

### FlyoutMenuItemClickedEventArgs

| 属性 | 类型 | 说明 |
|---|---|---|
| `Item` | `MenuItem` | 被点击的菜单项引用 |

---

## 伪类（Pseudo-Classes）

### 继承自 Button 的伪类

| 伪类 | 常量 | 触发条件 |
|---|---|---|
| `:icononly` | `ButtonPseudoClass.IconOnly` | Icon 不为 null 且 Content 为 null |
| `:loading` | `ButtonPseudoClass.Loading` | `IsLoading == true` |
| `:danger` | `ButtonPseudoClass.IsDanger` | `IsDanger == true` |
| `:default` | `ButtonPseudoClass.DefaultType` | `ButtonType == Default` |
| `:dashed` | `ButtonPseudoClass.DashedType` | `ButtonType == Dashed` |
| `:primary` | `ButtonPseudoClass.PrimaryType` | `ButtonType == Primary` |
| `:link` | `ButtonPseudoClass.LinkType` | `ButtonType == Link` |
| `:text` | `ButtonPseudoClass.TextType` | `ButtonType == Text` |

### 标准伪类

| 伪类 | 触发条件 |
|---|---|
| `:pointerover` | 鼠标悬浮 |
| `:pressed` | 按钮按下 |
| `:disabled` | `IsEnabled == false` |
| `:focus` | 获得焦点 |
| `:focus-visible` | 通过键盘获得焦点 |
| `:flyout-open` | 下拉菜单处于打开状态 |

---

## 实现的接口

继承自 `Button` 的所有接口：

| 接口 | 定义位置 | 作用 |
|---|---|---|
| `ISizeTypeAware` | `AtomUI.Controls.Shared` | 支持 `SizeType`（Small / Middle / Large）三种尺寸切换 |
| `IWaveSpiritAwareControl` | `AtomUI.Controls.Shared` | 支持点击涟漪（Wave）动画效果 |
| `ICompactSpaceAware` | `AtomUI.Controls.Shared` | 在 `Space.Compact` 中使用时自动调整圆角 |
| `IFormItemAware` | `AtomUI.Controls.Shared` | 可作为 `FormItem` 的子控件参与表单验证 |
| `IMotionAwareControl` | `AtomUI.Controls.Shared` | 支持过渡动画开关 |

