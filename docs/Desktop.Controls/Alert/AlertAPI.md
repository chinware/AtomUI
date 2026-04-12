# Alert API 参考

## 命名空间

```csharp
namespace AtomUI.Desktop.Controls;
```

AXAML 命名空间前缀：`atom`（`xmlns:atom="https://atomui.net"`）

---

## 枚举类型

### AlertType

警告类型枚举，定义警告的视觉风格和语义。

| 值 | 说明 |
|---|---|
| `Success` | 成功提示（绿色系），表示操作已成功完成 |
| `Info` | 信息提示（蓝色系），表示一般性提示信息 |
| `Warning` | 警告提示（橙色系），提醒用户需要注意 |
| `Error` | 错误提示（红色系），表示操作失败或存在风险 |

---

## 公共属性（StyledProperty）

| 属性名 | 类型 | 默认值 | 说明 |
|---|---|---|---|
| `Type` | `AlertType` | `AlertType.Success` | 警告类型，控制背景色、边框色和图标 |
| `Message` | `string` | `""` | 主消息内容（标记为 `[Content]`，可直接写在 AXAML 标签内部） |
| `Description` | `string?` | `null` | 描述信息，显示在消息下方，设置后触发 `:has-description` 伪类 |
| `IsShowIcon` | `bool` | `false` | 是否显示类型图标，图标自动匹配 `Type` |
| `IsClosable` | `bool` | `false` | 是否可关闭，启用后右侧显示关闭按钮 |
| `CloseIcon` | `PathIcon?` | `null`（默认使用 `CloseOutlined`） | 自定义关闭图标，仅在 `IsClosable=True` 时生效 |
| `ExtraAction` | `Control?` | `null` | 额外操作区域，放置在消息行右侧（如操作按钮） |
| `IsMessageMarqueEnabled` | `bool` | `false` | 是否启用消息跑马灯效果（长文本自动水平滚动） |

### 继承自 TemplatedControl 的常用属性

| 属性名 | 类型 | 默认值 | 说明 |
|---|---|---|---|
| `Background` | `IBrush?` | 由主题根据 `Type` 控制 | 背景色 |
| `Foreground` | `IBrush?` | 由主题控制 | 前景色（文本颜色） |
| `BorderBrush` | `IBrush?` | 由主题根据 `Type` 控制 | 边框颜色 |
| `BorderThickness` | `Thickness` | `{atom:SharedTokenResource BorderThickness}` | 边框粗细 |
| `CornerRadius` | `CornerRadius` | `{atom:SharedTokenResource BorderRadiusLG}` | 圆角半径 |
| `Padding` | `Thickness` | 由 Token 根据有无 Description 控制 | 内间距 |
| `FontSize` | `double` | 由 Token 控制 | 字体大小 |
| `HorizontalAlignment` | `HorizontalAlignment` | `Stretch` | 水平对齐（默认撑满父容器） |
| `VerticalAlignment` | `VerticalAlignment` | `Top` | 垂直对齐（默认顶部对齐） |
| `IsEnabled` | `bool` | `true` | 是否启用 |

---

## 事件

| 事件名 | 类型 | 说明 |
|---|---|---|
| `CloseRequest` | `EventHandler?` | 关闭按钮点击时触发。注意：事件触发后 Alert 不会自动移除，需在事件处理中手动移除 |

---

## 伪类（Pseudo-Classes）

Alert 支持以下伪类，可在样式选择器中使用：

| 伪类 | 常量 | 触发条件 |
|---|---|---|
| `:has-description` | `AlertPseudoClass.HasDescription` | `Description` 不为 null 且非空字符串 |
| `:has-extra-action` | `AlertPseudoClass.HasExtraAction` | `ExtraAction` 不为 null |

### 继承的标准伪类

| 伪类 | 触发条件 |
|---|---|
| `:disabled` | `IsEnabled == false` |
| `:pointerover` | 鼠标悬浮 |
| `:focus` | 获得焦点 |

---

## 静态构造器影响

以下属性变更会触发重新测量（`AffectsMeasure`）：

- `IsClosable`、`IsShowIcon`、`Message`、`Description`、`IsMessageMarqueEnabled`、`Padding`、`ExtraAction`

以下属性变更会触发重新渲染（`AffectsRender`）：

- `Type`

---

## [Content] 属性说明

`Message` 属性标记了 `[Content]` 特性，因此 AXAML 中可以直接在标签内部写消息文本：

```xml
<!-- 以下两种写法等价 -->
<atom:Alert Type="Info">This is a message</atom:Alert>
<atom:Alert Type="Info" Message="This is a message" />
```

当同时使用 `Description` 时，需要显式使用 `Message` 属性：

```xml
<atom:Alert Type="Info"
            Message="Title Text"
            Description="Detailed description here" />
```
