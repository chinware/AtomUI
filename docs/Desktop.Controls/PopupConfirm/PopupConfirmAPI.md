# PopupConfirm API 参考

## 命名空间

```csharp
namespace AtomUI.Desktop.Controls;
```

AXAML 命名空间前缀：`atom`（`xmlns:atom="https://atomui.net"`）

---

## 枚举类型

### PopupConfirmStatus

确认状态枚举，控制图标颜色以传达操作风险等级。

| 值 | 说明 |
|---|---|
| `Info` | 信息性确认，图标使用主色（`ColorPrimary`，蓝色） |
| `Warning` | 警告性确认（默认），图标使用警告色（`ColorWarning`，橙色） |
| `Error` | 高风险/破坏性确认，图标使用错误色（`ColorError`，红色） |

### FlyoutTriggerType（来自 `FlyoutHost`）

弹出触发方式枚举。

| 值 | 说明 |
|---|---|
| `Hover` | 鼠标悬浮触发 |
| `Click` | 鼠标点击触发（默认） |
| `Focus` | 获得焦点时触发 |

### ButtonType（来自 `AtomUI.Desktop.Controls`）

按钮类型枚举，用于控制确认按钮的视觉风格。

| 值 | 说明 |
|---|---|
| `Default` | 默认按钮 |
| `Primary` | 主按钮（默认用于确认按钮） |
| `Dashed` | 虚线按钮 |
| `Text` | 文本按钮 |
| `Link` | 链接按钮 |

---

## PopupConfirm 公共属性（StyledProperty）

| 属性名 | 类型 | 默认值 | 说明 |
|---|---|---|---|
| `Title` | `string` | — | 确认框标题文本 |
| `ConfirmContent` | `object?` | `null` | 确认框描述内容，支持任意类型（字符串、控件等） |
| `ConfirmContentTemplate` | `IDataTemplate?` | `null` | 确认内容的数据模板 |
| `OkText` | `string` | 由本地化资源控制 | 确认按钮文本（英文默认 `"Ok"`，中文默认 `"确定"`） |
| `CancelText` | `string` | 由本地化资源控制 | 取消按钮文本（英文默认 `"Cancel"`，中文默认 `"取消"`） |
| `OkButtonType` | `ButtonType` | `ButtonType.Primary` | 确认按钮的视觉类型 |
| `IsShowCancelButton` | `bool` | `true` | 是否显示取消按钮 |
| `Icon` | `PathIcon?` | `null`（自动使用 `ExclamationCircleFilled`） | 自定义图标，未设置时使用默认警告图标 |
| `ConfirmStatus` | `PopupConfirmStatus` | `PopupConfirmStatus.Warning` | 确认状态，控制图标颜色 |

### 继承自 FlyoutHost 的属性

| 属性名 | 类型 | 默认值 | 说明 |
|---|---|---|---|
| `Flyout` | `PopupFlyoutBase?` | 自动创建 `PopupConfirmFlyout` | 内部弹出层对象（通常无需手动设置） |
| `Trigger` | `FlyoutTriggerType` | `FlyoutTriggerType.Click` | 弹出触发方式 |
| `Placement` | `PlacementMode` | `PlacementMode.Top` | 弹出方位 |
| `IsShowArrow` | `bool` | `false` | 是否显示箭头指向触发元素 |
| `IsPointAtCenter` | `bool` | `false` | 箭头是否指向触发元素中心 |
| `MarginToAnchor` | `double` | 由 `PopupHostToken` 控制 | 弹出层与触发元素之间的间距 |
| `MouseEnterDelay` | `int` | 由 `FlyoutStateHelper` 控制 | 悬浮触发模式下的显示延迟（毫秒） |
| `MouseLeaveDelay` | `int` | 由 `FlyoutStateHelper` 控制 | 悬浮触发模式下的隐藏延迟（毫秒） |
| `IsMotionEnabled` | `bool` | 跟随全局 Token `EnableMotion` | 是否启用弹出/收起动画 |
| `MotionDuration` | `TimeSpan` | 跟随全局 Token `MotionDurationMid` | 动画持续时间 |
| `OpenMotion` | `AbstractMotion?` | `null` | 自定义打开动画 |
| `CloseMotion` | `AbstractMotion?` | `null` | 自定义关闭动画 |
| `PlacementAnchor` | `PopupAnchor` | — | 锚点位置（高级定位） |
| `PlacementGravity` | `PopupGravity` | — | 弹出重力方向（高级定位） |

### 继承自 ContentControl 的常用属性

| 属性名 | 类型 | 默认值 | 说明 |
|---|---|---|---|
| `Content` | `object?` | `null` | 触发元素（如 Button），PopupConfirm 包裹该内容 |
| `ContentTemplate` | `IDataTemplate?` | `null` | 触发元素的数据模板 |
| `IsEnabled` | `bool` | `true` | 是否启用 |

---

## 事件

| 事件名 | 类型 | 路由策略 | 说明 |
|---|---|---|---|
| `Confirmed` | `EventHandler<RoutedEventArgs>` | Bubble | 用户点击确认按钮时触发 |
| `Cancelled` | `EventHandler<RoutedEventArgs>` | Bubble | 用户点击取消按钮时触发 |
| `PopupClick` | `EventHandler<PopupConfirmClickEventArgs>` | Bubble | 用户点击任意按钮（确认或取消）时触发，通过 `IsConfirmed` 区分 |

### PopupConfirmClickEventArgs

| 属性 | 类型 | 说明 |
|---|---|---|
| `IsConfirmed` | `bool` | `true` 表示用户点击了确认按钮，`false` 表示点击了取消按钮 |

**事件触发顺序**：当用户点击确认按钮时，先触发 `Confirmed`，再触发 `PopupClick`（`IsConfirmed = true`）。当用户点击取消按钮时，先触发 `Cancelled`，再触发 `PopupClick`（`IsConfirmed = false`）。点击任意按钮后，弹出层自动关闭。

---

## 公共方法

| 方法名 | 返回值 | 说明 |
|---|---|---|
| `ShowFlyout(bool immediately)` | `void` | 显示弹出层（继承自 `FlyoutHost`）。`immediately=true` 时跳过动画 |
| `HideFlyout(bool immediately)` | `void` | 隐藏弹出层（继承自 `FlyoutHost`）。`immediately=true` 时跳过动画 |

---

## 伪类（Pseudo-Classes）

### PopupConfirmContainer 支持的伪类

| 伪类 | 常量 | 触发条件 |
|---|---|---|
| `:empty-content` | `PopupConfirmPseudoClass.EmptyContent` | `ConfirmContent == null`，即未设置描述内容 |

当 `:empty-content` 伪类激活时，按钮区域的上边距（`ButtonContainerMargin`）被移除，使仅有标题的气泡更加紧凑。

### 继承的标准伪类

| 伪类 | 触发条件 |
|---|---|
| `:disabled` | `IsEnabled == false` |

---

## 实现的接口

### IMotionAwareControl（继承自 FlyoutHost）

```csharp
public bool IsMotionEnabled { get; set; }
public TimeSpan MotionDuration { get; set; }
```

控制弹出层的显示/隐藏动画是否启用及其持续时间。

---

## 内部控件类型（Internal）

以下控件为 `internal` 类型，开发者不能直接实例化使用，但了解它们有助于理解模板结构和样式选择器。

### PopupConfirmFlyout

继承自 `Flyout`，负责创建 `FlyoutPresenter` 并在其中放置 `PopupConfirmContainer`。通过 `RelayBind` 将 `PopupConfirm` 的所有属性中继到 `PopupConfirmContainer`。

### PopupConfirmContainer

继承自 `TemplatedControl`，承载弹出气泡内部的实际 UI（图标 + 标题 + 内容 + 按钮）。通过弱引用持有 `PopupConfirm` 实例，避免内存泄漏。

**中继的属性（通过 AddOwner）**：

| 属性名 | 类型 | 说明 |
|---|---|---|
| `OkText` | `string` | 确认按钮文本 |
| `CancelText` | `string` | 取消按钮文本 |
| `OkButtonType` | `ButtonType` | 确认按钮类型 |
| `IsShowCancelButton` | `bool` | 是否显示取消按钮 |
| `Title` | `string` | 标题文本 |
| `ConfirmContent` | `object?` | 描述内容 |
| `ConfirmContentTemplate` | `IDataTemplate?` | 内容模板 |
| `Icon` | `PathIcon?` | 自定义图标 |
| `ConfirmStatus` | `PopupConfirmStatus` | 确认状态 |

---

## 本地化资源

PopupConfirm 的按钮文本通过 `CommonLangResource` 本地化系统提供默认值。

### 英文 (en_US)

| 资源键 | 值 | 说明 |
|---|---|---|
| `Ok` | `"Ok"` | 确认按钮默认文本 |
| `Cancel` | `"Cancel"` | 取消按钮默认文本 |

### 中文 (zh_CN)

| 资源键 | 值 | 说明 |
|---|---|---|
| `Ok` | `"确定"` | 确认按钮默认文本 |
| `Cancel` | `"取消"` | 取消按钮默认文本 |

---

## 模板部件常量

### PopupConfirmContainerThemeConstants

| 常量 | 值 | 说明 |
|---|---|---|
| `MainLayoutPart` | `"PART_MainLayout"` | 主布局 DockPanel |
| `ButtonLayoutPart` | `"PART_ButtonLayout"` | 按钮区域 StackPanel |
| `OkButtonPart` | `"PART_OkButton"` | 确认按钮 |
| `CancelButtonPart` | `"PART_CancelButton"` | 取消按钮 |
| `TitlePart` | `"PART_Title"` | 标题 TextBlock |
| `ContentPart` | `"PART_Content"` | 内容 ContentPresenter |
| `IconPresenterPart` | `"PART_IconPresenter"` | 图标 IconPresenter |

### PopupConfirmThemeConstants

| 常量 | 值 | 说明 |
|---|---|---|
| `ContentPresenterPart` | `"PART_ContentPresenter"` | 外层内容展示器 |
