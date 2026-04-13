# MarqueeLabel API 参考

## 命名空间

```csharp
// 桌面端具体实现
namespace AtomUI.Desktop.Controls;

// 设备无关基类
namespace AtomUI.Controls.Commons;
```

AXAML 命名空间前缀：`atom`（`xmlns:atom="https://atomui.net"`）

---

## 基类信息

### AbstractMarqueeLabel（设备无关基类）

| 项目 | 说明 |
|---|---|
| 所在项目 | `AtomUI.Controls` |
| 命名空间 | `AtomUI.Controls.Commons` |
| 继承自 | `Avalonia.Controls.TextBlock` |
| 文件位置 | `src/AtomUI.Controls/MarqueeLabel/AbstractMarqueeLabel.cs` |

`AbstractMarqueeLabel` 定义了跑马灯的所有核心行为，包括滚动动画、布局测量、悬浮暂停/恢复逻辑。桌面端 `MarqueeLabel` 继承此基类并注册 Token 资源作用域。

---

## 公共属性（StyledProperty）

以下属性定义在基类 `AbstractMarqueeLabel` 中，`MarqueeLabel` 继承并使用：

| 属性名 | 类型 | 默认值 | 定义层 | 说明 |
|---|---|---|---|---|
| `CycleSpace` | `double` | `0`（由 Token 覆盖为 `200`） | `AbstractMarqueeLabel` | 循环间隔——两份文本之间的视觉间距（像素） |
| `MoveSpeed` | `double` | `150`（由 Token 覆盖为 `150`） | `AbstractMarqueeLabel` | 滚动速度（像素/秒） |

### 继承自 Avalonia.Controls.TextBlock 的常用属性

| 属性名 | 类型 | 默认值 | 说明 |
|---|---|---|---|
| `Text` | `string?` | `null` | 显示的文本内容 |
| `FontSize` | `double` | 由主题控制 | 字体大小 |
| `FontFamily` | `FontFamily` | 由主题控制 | 字体族 |
| `FontWeight` | `FontWeight` | `Normal` | 字重 |
| `FontStyle` | `FontStyle` | `Normal` | 字体样式（正常/斜体） |
| `Foreground` | `IBrush?` | 由主题控制 | 文本前景色 |
| `TextWrapping` | `TextWrapping` | `NoWrap` | 文本换行模式（跑马灯场景下应保持 `NoWrap`） |
| `TextTrimming` | `TextTrimming` | `None` | 文本截断模式 |
| `TextAlignment` | `TextAlignment` | `Left` | 文本对齐方式 |
| `LineHeight` | `double` | `NaN` | 行高 |
| `Padding` | `Thickness` | `0` | 内间距 |
| `HorizontalAlignment` | `HorizontalAlignment` | `Stretch`（已覆盖默认值） | 水平对齐 |
| `VerticalAlignment` | `VerticalAlignment` | `Top`（已覆盖默认值） | 垂直对齐 |
| `IsEnabled` | `bool` | `true` | 是否启用 |

> **注意**：`AbstractMarqueeLabel` 将 `HorizontalAlignment` 默认值覆盖为 `Stretch`，将 `VerticalAlignment` 默认值覆盖为 `Top`，确保控件在布局中默认水平撑满父容器。

---

## 内部属性（Internal）

以下属性仅供内部动画系统使用，不对外公开：

| 属性名 | 类型 | 说明 |
|---|---|---|
| `PivotOffset` | `double` | 文本绘制的水平偏移量，由动画驱动，控制滚动位置 |

---

## 伪类（Pseudo-Classes）

MarqueeLabel 未定义自定义伪类，支持从 `TextBlock` 继承的标准伪类：

| 伪类 | 触发条件 | 对 MarqueeLabel 的影响 |
|---|---|---|
| `:pointerover` | 鼠标悬浮在控件上 | 暂停滚动动画，保持当前位置 |
| `:disabled` | `IsEnabled == false` | 标准禁用样式 |

---

## 事件

MarqueeLabel 未定义自定义事件。可使用 `TextBlock` / `Control` 继承的标准事件：

| 事件名 | 类型 | 说明 |
|---|---|---|
| `PointerEntered` | `EventHandler<PointerEventArgs>` | 指针进入控件区域（此时动画暂停） |
| `PointerExited` | `EventHandler<PointerEventArgs>` | 指针离开控件区域（此时动画恢复） |
| `PropertyChanged` | `EventHandler<AvaloniaPropertyChangedEventArgs>` | 属性变化通知 |

---

## 重写的方法

以下是 `AbstractMarqueeLabel` 中重写的关键方法（仅列出对理解控件行为有帮助的部分）：

| 方法 | 来源 | 说明 |
|---|---|---|
| `MeasureOverride(Size)` | `TextBlock` | 布局测量时判断文本是否溢出，决定是否启动滚动动画 |
| `RenderTextLayout(DrawingContext, Point)` | `TextBlock` | 核心渲染方法——绘制双份文本实现无缝循环效果 |
| `OnAttachedToVisualTree(...)` | `Visual` | 控件挂载到可视树时启动动画 |
| `OnDetachedFromVisualTree(...)` | `Visual` | 控件从可视树移除时清理动画资源 |
| `OnPropertyChanged(...)` | `AvaloniaObject` | 监听 `IsPointerOver`、`CycleSpace`、`MoveSpeed` 变化 |
| `ApplyTemplate()` | `Control` | 密封方法，确保模板应用行为一致 |
