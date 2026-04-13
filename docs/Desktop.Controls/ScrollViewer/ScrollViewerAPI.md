# ScrollViewer API 参考

## 命名空间

```csharp
namespace AtomUI.Desktop.Controls;      // ScrollViewer, ScrollBar, ScrollBarThumb
namespace AtomUI.Controls.Commons;       // AbstractScrollViewer, AbstractScrollBar, AbstractScrollBarThumb
```

AXAML 命名空间前缀：`atom`（`xmlns:atom="https://atomui.net"`）

---

## ScrollViewer

### 公共属性（StyledProperty / AttachedProperty）

| 属性名 | 类型 | 默认值 | 属性类别 | 说明 |
|---|---|---|---|---|
| `IsLiteMode` | `bool` | `false` | `AttachedProperty` | 极简模式：滑块初始为细线，悬浮时膨胀，内容覆盖滚动条区域 |
| `IsMotionEnabled` | `bool` | 跟随全局 Token `EnableMotion` | `StyledProperty`（继承自 `AbstractScrollViewer`） | 是否启用过渡动画（滚动条显隐、分隔符透明度等） |

### 继承自 Avalonia.Controls.ScrollViewer 的常用属性

| 属性名 | 类型 | 默认值 | 说明 |
|---|---|---|---|
| `Content` | `object?` | `null` | 滚动内容（单个子控件） |
| `HorizontalScrollBarVisibility` | `ScrollBarVisibility` | `Disabled` | 水平滚动条可见性策略 |
| `VerticalScrollBarVisibility` | `ScrollBarVisibility` | `Auto` | 垂直滚动条可见性策略 |
| `AllowAutoHide` | `bool` | `true` | 是否允许滚动条在无交互时自动隐藏 |
| `IsScrollInertiaEnabled` | `bool` | `true` | 是否启用滚动惯性效果 |
| `IsDeferredScrollingEnabled` | `bool` | `false` | 延迟滚动模式（拖拽滑块时不实时更新内容位置） |
| `Offset` | `Vector` | `(0, 0)` | 当前滚动偏移量（可读写） |
| `Extent` | `Size` | — | 内容总尺寸（只读） |
| `Viewport` | `Size` | — | 可视区域尺寸（只读） |
| `IsExpanded` | `bool` | — | 滚动条是否展开（鼠标悬浮/拖拽时为 `true`） |
| `BringIntoViewOnFocusChange` | `bool` | `true` | 焦点变化时是否自动滚动到焦点元素 |
| `Background` | `IBrush?` | `Transparent` | 背景色 |
| `Padding` | `Thickness` | — | 内容区域内间距 |
| `HorizontalSnapPointsType` | `SnapPointsType` | — | 水平 SnapPoints 类型 |
| `VerticalSnapPointsType` | `SnapPointsType` | — | 垂直 SnapPoints 类型 |
| `HorizontalSnapPointsAlignment` | `SnapPointsAlignment` | — | 水平 SnapPoints 对齐方式 |
| `VerticalSnapPointsAlignment` | `SnapPointsAlignment` | — | 垂直 SnapPoints 对齐方式 |

### 附加属性静态方法

```csharp
// 获取控件的极简模式设置
public static bool GetIsLiteMode(Control control);

// 设置控件的极简模式
public static void SetIsLiteMode(Control control, bool value);
```

### ScrollBarVisibility 枚举

| 值 | 说明 |
|---|---|
| `Disabled` | 禁用该方向的滚动（不显示滚动条，内容不可滚动） |
| `Auto` | 按需显示（内容超出时显示，未超出时隐藏） |
| `Visible` | 始终显示滚动条 |
| `Hidden` | 隐藏滚动条但内容仍可滚动（通过触控/滚轮） |

---

## ScrollBar

### 公共属性（StyledProperty）

| 属性名 | 类型 | 默认值 | 说明 |
|---|---|---|---|
| `IsLiteMode` | `bool` | `false` | 极简模式（通过 `ScrollViewer.IsLiteModeProperty.AddOwner` 注册） |
| `IsMotionEnabled` | `bool` | 跟随全局 Token `EnableMotion` | 是否启用过渡动画 |

### 继承自 Avalonia.Controls.Primitives.ScrollBar 的常用属性

| 属性名 | 类型 | 说明 |
|---|---|---|
| `Orientation` | `Orientation` | 滚动条方向：`Horizontal` 或 `Vertical` |
| `Value` | `double` | 当前滑块位置值 |
| `Minimum` | `double` | 最小值 |
| `Maximum` | `double` | 最大值 |
| `ViewportSize` | `double` | 视口大小（决定滑块相对于轨道的长度比例） |
| `AllowAutoHide` | `bool` | 是否允许自动隐藏 |

---

## ScrollBarThumb

### 公共属性（StyledProperty）

| 属性名 | 类型 | 默认值 | 说明 |
|---|---|---|---|
| `IsLiteMode` | `bool` | `false` | 极简模式（通过 `ScrollViewer.IsLiteModeProperty.AddOwner` 注册） |
| `Orientation` | `Orientation` | — | 滑块方向（继承自 `AbstractScrollBarThumb`） |
| `IsMotionEnabled` | `bool` | 跟随全局 Token | 是否启用过渡动画（背景色、宽度/高度过渡） |

### 继承自 Avalonia.Controls.Primitives.Thumb 的常用属性

| 属性名 | 类型 | 说明 |
|---|---|---|
| `Background` | `IBrush?` | 滑块背景色（由 Token 控制） |
| `CornerRadius` | `CornerRadius` | 滑块圆角（由 Token 控制） |
| `Width` | `double` | 垂直滑块的宽度（由 Token 和 LiteMode 状态控制） |
| `Height` | `double` | 水平滑块的高度（由 Token 和 LiteMode 状态控制） |

---

## 伪类（Pseudo-Classes）

### ScrollBar 伪类

| 伪类 | 常量 | 触发条件 |
|---|---|---|
| `:vertical` | `StdPseudoClass.Vertical` | `Orientation == Vertical` |
| `:horizontal` | `StdPseudoClass.Horizontal` | `Orientation == Horizontal` |

### ScrollBarThumb 伪类

| 伪类 | 触发条件 |
|---|---|
| `:pointerover` | 鼠标悬浮在滑块上 |
| `:pressed` | 滑块被按下（拖拽中） |

---

## 实现的接口

### IMotionAwareControl

所有三个控件（`ScrollViewer`、`ScrollBar`、`ScrollBarThumb`）均实现此接口：

```csharp
public bool IsMotionEnabled { get; set; }
```

用于全局控制过渡动画的开启/关闭。当 `IsMotionEnabled = false` 时，所有过渡动画被移除，属性变化立即生效。
