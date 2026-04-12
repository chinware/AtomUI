# ButtonSpinner API 参考

## 命名空间

```csharp
namespace AtomUI.Desktop.Controls;   // ButtonSpinner, ButtonSpinnerLocation
namespace AtomUI.Controls;            // SizeType, InputControlStyleVariant, InputControlStatus
```

AXAML 命名空间前缀：`atom`（`xmlns:atom="https://atomui.net"`）

---

## 枚举类型

### ButtonSpinnerLocation

操作按钮放置位置枚举。

| 值 | 说明 |
|---|---|
| `Left` | 操作按钮放在控件左侧 |
| `Right` | 操作按钮放在控件右侧（默认） |

### SpinDirection（Avalonia 内建）

增减方向枚举，由 `SpinEventArgs` 携带。

| 值 | 说明 |
|---|---|
| `Increase` | 增加方向 |
| `Decrease` | 减少方向 |

### ValidSpinDirections（Avalonia 内建）

可用方向位标志枚举，控制增加/减少按钮的可用性。

| 值 | 说明 |
|---|---|
| `None` | 两个方向都不可用 |
| `Increase` | 仅增加可用 |
| `Decrease` | 仅减少可用 |
| `Increase \| Decrease` | 两个方向都可用（默认） |

---

## 公共属性（StyledProperty）

### ButtonSpinner 自身属性

| 属性名 | 类型 | 默认值 | 说明 |
|---|---|---|---|
| `AllowSpin` | `bool` | `true` | 是否允许通过按钮、键盘、滚轮触发 Spin |
| `ShowButtonSpinner` | `bool` | `true` | 是否显示操作按钮 |
| `ButtonSpinnerLocation` | `ButtonSpinnerLocation` | `Right` | 操作按钮放置位置 |
| `IsButtonSpinnerFloatable` | `bool` | `false` | 操作按钮是否浮动（悬浮时滑入） |

### 尺寸与样式

| 属性名 | 类型 | 默认值 | 说明 |
|---|---|---|---|
| `SizeType` | `SizeType` | `SizeType.Middle` | 控件尺寸（Large/Middle/Small），共享属性 |
| `StyleVariant` | `InputControlStyleVariant` | `InputControlStyleVariant.Outline` | 样式变体（Outline/Filled/Borderless），共享属性 |
| `Status` | `InputControlStatus` | `InputControlStatus.Default` | 验证状态（Default/Error/Warning），共享属性 |
| `IsMotionEnabled` | `bool` | 跟随全局 Token | 是否启用过渡动画，共享属性 |

### 附加组件（AddOn）

| 属性名 | 类型 | 默认值 | 说明 |
|---|---|---|---|
| `LeftAddOn` | `object?` | `null` | 左侧外部附加组件（支持文本、图标等） |
| `LeftAddOnTemplate` | `IDataTemplate?` | `null` | 左侧附加组件的数据模板 |
| `RightAddOn` | `object?` | `null` | 右侧外部附加组件 |
| `RightAddOnTemplate` | `IDataTemplate?` | `null` | 右侧附加组件的数据模板 |

### 内部前后缀（Inner Content）

| 属性名 | 类型 | 默认值 | 说明 |
|---|---|---|---|
| `InnerLeftContent` | `object?` | `null` | 内部左侧前缀（支持文本、图标等） |
| `InnerLeftContentTemplate` | `IDataTemplate?` | `null` | 前缀的数据模板 |
| `InnerRightContent` | `object?` | `null` | 内部右侧后缀 |
| `InnerRightContentTemplate` | `IDataTemplate?` | `null` | 后缀的数据模板 |

### 继承自 Spinner 的属性

| 属性名 | 类型 | 默认值 | 说明 |
|---|---|---|---|
| `Content` | `object?` | `null` | 主内容区域 |
| `ContentTemplate` | `IDataTemplate?` | `null` | 主内容的数据模板 |
| `ValidSpinDirection` | `ValidSpinDirections` | `Increase \| Decrease` | 可用的 Spin 方向 |

---

## 事件

### Spin（继承自 Spinner）

```csharp
public event EventHandler<SpinEventArgs>? Spin;
```

当用户通过按钮点击、键盘方向键或鼠标滚轮触发增减操作时引发。

**SpinEventArgs 属性：**

| 属性 | 类型 | 说明 |
|---|---|---|
| `Direction` | `SpinDirection` | 增减方向（`Increase` / `Decrease`） |

---

## 伪类

### ButtonSpinner 自定义伪类

| 伪类 | 触发条件 | 说明 |
|---|---|---|
| `:left` | `ButtonSpinnerLocation == Left` | 操作按钮在左侧时激活 |
| `:right` | `ButtonSpinnerLocation == Right` | 操作按钮在右侧时激活 |

### 继承的标准伪类

| 伪类 | 说明 |
|---|---|
| `:disabled` | 控件禁用时 |
| `:pointerover` | 鼠标悬浮时 |
| `:focus` | 获得焦点时 |
| `:focus-within` | 子控件获得焦点时 |

---

## 实现的接口

| 接口 | 提供的属性/方法 |
|---|---|
| `ISizeTypeAware` | `SizeType` 属性 |
| `IInputControlStyleVariantAware` | `StyleVariant` 属性 |
| `IInputControlStatusAware` | `Status` 属性 |
| `IMotionAwareControl` | `IsMotionEnabled` 属性 |
| `ICompactSpaceAware` | `NotifyPositionChange()`、`NotifyOrientationChange()`、`GetBorderThickness()` |

---

## PathIcon 自动适配

当 `LeftAddOn`、`RightAddOn`、`InnerLeftContent`、`InnerRightContent` 被设置为 `PathIcon` 类型时，ButtonSpinner 会自动将其包装为 `SizeTypeAwareIconPresenter`，确保图标尺寸跟随 `SizeType` 属性自动调整。无需手动设置图标大小。
