# Tag API 参考

## 命名空间

```csharp
namespace AtomUI.Desktop.Controls;
```

AXAML 命名空间前缀：`atom`（`xmlns:atom="https://atomui.net"`）

---

## 枚举类型

### TagStatus

标签状态枚举，用于通过 `TagColor` 属性设置语义化状态颜色。

| 值 | 说明 |
|---|---|
| `Success` | 成功（绿色系） |
| `Info` | 信息（蓝色系） |
| `Warning` | 警告（橙色系） |
| `Error` | 错误（红色系） |

### 预设颜色名称

以下字符串可传入 `TagColor` 属性，触发预设颜色模式：

`magenta`、`red`、`volcano`、`orange`、`gold`、`lime`、`green`、`cyan`、`blue`、`geekblue`、`purple`、`pink`、`yellow`

---

## 公共属性（StyledProperty）

| 属性名 | 类型 | 默认值 | 说明 |
|---|---|---|---|
| `TagColor` | `string?` | `null` | 标签颜色，支持预设色名称（`"blue"`）、状态名称（`"success"`）、CSS 颜色值（`"#f50"`）。传入 `"default"` 或其他无法识别的值时，标签保持默认样式 |
| `TagText` | `string?` | `null` | 标签文字内容（`[Content]` 属性，可直接写在标签内） |
| `IsClosable` | `bool` | `false` | 是否可关闭，启用后显示关闭图标 |
| `Bordered` | `bool` | `true` | 是否显示边框（自定义颜色模式下自动设为 `false`） |
| `Icon` | `PathIcon?` | `null` | 标签图标，显示在文字左侧 |
| `CloseIcon` | `PathIcon?` | `null`（默认 `CloseOutlined`） | 关闭图标，可自定义 |

### 继承自 TemplatedControl 的常用属性

| 属性名 | 类型 | 默认值 | 说明 |
|---|---|---|---|
| `Background` | `IBrush?` | 由 Token 控制（`DefaultBg`） | 背景色（预设/状态/自定义颜色会覆盖） |
| `Foreground` | `IBrush?` | 由 Token 控制（`DefaultColor`） | 前景色（文字颜色） |
| `BorderBrush` | `IBrush?` | `SharedToken.ColorBorder` | 边框颜色 |
| `BorderThickness` | `Thickness` | `SharedToken.BorderThickness` | 边框粗细 |
| `CornerRadius` | `CornerRadius` | `SharedToken.BorderRadiusSM` | 圆角半径 |
| `Padding` | `Thickness` | 由 Token 控制（`TagPadding`） | 内间距 |
| `FontSize` | `double` | 由 Token 控制（`TagFontSize`） | 字号 |
| `IsEnabled` | `bool` | `true` | 是否启用 |

---

## 事件

| 事件名 | 类型 | 路由策略 | 说明 |
|---|---|---|---|
| `Closed` | `EventHandler<RoutedEventArgs>` | Bubble | 关闭按钮点击时触发（仅 `IsClosable=true` 时有效） |

---

## 伪类（Pseudo-Classes）

### Tag 特有伪类

| 伪类 | 常量 | 触发条件 |
|---|---|---|
| `:preset-color` | `TagPseudoClass.PresetColor` | `TagColor` 匹配 13 种预设颜色名 |
| `:status-color` | `TagPseudoClass.StatusColor` | `TagColor` 匹配 4 种状态名 |
| `:custom-color` | `TagPseudoClass.CustomColor` | `TagColor` 为自定义 CSS 颜色值 |

### 继承的标准伪类

| 伪类 | 触发条件 |
|---|---|
| `:pointerover` | 鼠标悬浮 |
| `:disabled` | `IsEnabled == false` |

---

## 静态构造器影响

以下属性变更会触发重新测量（`AffectsMeasure`）：

- `Bordered`、`Icon`、`IsClosable`、`TagText`

以下属性变更会触发重新渲染（`AffectsRender`）：

- `TagColor`、`Foreground`、`Background`、`BorderBrush`
