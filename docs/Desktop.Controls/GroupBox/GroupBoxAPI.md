# GroupBox API 参考

## 命名空间

```csharp
namespace AtomUI.Desktop.Controls;
```

AXAML 命名空间前缀：`atom`（`xmlns:atom="https://atomui.net"`）

---

## 枚举类型

### GroupBoxTitlePosition

标题位置枚举，控制标题在分组框顶部的水平对齐方式。

| 值 | 说明 |
|---|---|
| `Left` | 标题左对齐（默认） |
| `Right` | 标题右对齐 |
| `Center` | 标题居中对齐 |

---

## 公共属性（StyledProperty）

| 属性名 | 类型 | 默认值 | 说明 |
|---|---|---|---|
| `HeaderTitle` | `string?` | `null` | 标题文本内容 |
| `HeaderTitleColor` | `IBrush?` | 由主题控制（`ColorText`） | 标题文本颜色 |
| `HeaderIcon` | `PathIcon?` | `null` | 标题图标，使用 Ant Design 图标集 |
| `HeaderTitlePosition` | `GroupBoxTitlePosition` | `GroupBoxTitlePosition.Left` | 标题水平对齐位置 |
| `HeaderFontSize` | `double` | 由主题控制（`FontSize`） | 标题字体大小 |
| `HeaderFontStyle` | `FontStyle` | `FontStyle.Normal` | 标题字体风格（Normal / Italic / Oblique） |
| `HeaderFontWeight` | `FontWeight` | `FontWeight.Normal` | 标题字重 |

### 继承自 Avalonia.Controls.ContentControl 的常用属性

| 属性名 | 类型 | 默认值 | 说明 |
|---|---|---|---|
| `Content` | `object?` | `null` | 分组框内容，可以是任意控件或布局面板 |
| `ContentTemplate` | `IDataTemplate?` | `null` | 内容数据模板 |
| `Background` | `IBrush?` | 由主题控制（`ColorBgContainer`） | 背景色（同时用于标题遮挡区域的填充色） |
| `BorderBrush` | `IBrush?` | 由主题控制（`ColorBorder`） | 边框颜色 |
| `BorderThickness` | `Thickness` | 由 Token 控制（`BorderThickness`） | 边框粗细 |
| `CornerRadius` | `CornerRadius` | 由 Token 控制（`BorderRadius`） | 圆角半径 |
| `Padding` | `Thickness` | 由 Token 控制（`ContentPadding`） | 内容区域内间距 |
| `IsEnabled` | `bool` | `true` | 是否启用 |
| `Margin` | `Thickness` | `0` | 外间距 |

---

## 事件

GroupBox 没有定义自定义事件。可使用继承自 `Control` 的标准事件：

| 事件名 | 类型 | 说明 |
|---|---|---|
| `PointerEntered` | `EventHandler<PointerEventArgs>` | 指针进入控件区域 |
| `PointerExited` | `EventHandler<PointerEventArgs>` | 指针离开控件区域 |
| `PointerPressed` | `EventHandler<PointerPressedEventArgs>` | 指针按下 |
| `PointerReleased` | `EventHandler<PointerReleasedEventArgs>` | 指针释放 |

---

## 伪类（Pseudo-Classes）

GroupBox 没有定义自定义伪类。仅支持继承自基类的标准伪类：

| 伪类 | 触发条件 |
|---|---|
| `:pointerover` | 鼠标悬浮在 GroupBox 上 |
| `:disabled` | `IsEnabled == false` |
| `:focus` | 获得焦点 |
| `:focus-visible` | 通过键盘（Tab）获得焦点 |

---

## 实现的接口

GroupBox 没有实现 AtomUI 共享接口（如 `ISizeTypeAware`、`IWaveSpiritAwareControl` 等），因为它是一个静态容器控件，不需要尺寸切换、波纹效果等交互特性。

---

## 关键内部成员

以下成员为 `private`，不对外暴露，但有助于理解控件的工作原理：

| 成员 | 类型 | 说明 |
|---|---|---|
| `_borderRenderHelper` | `BorderRenderHelper` | 边框渲染辅助器，负责绘制带圆角的边框和背景 |
| `_headerContentContainer` | `Control?` | 标题内容容器引用（`PART_HeaderContent`），用于计算遮挡区域 |
| `_frame` | `Border?` | 主框架引用（`PART_Frame`），用于布局计算 |
| `_borderBounds` | `Rect` | 计算后的边框区域（从标题垂直中心到控件底部） |
