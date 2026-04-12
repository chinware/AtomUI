# Button API 参考

## 命名空间

```csharp
namespace AtomUI.Desktop.Controls;
```

AXAML 命名空间前缀：`atom`（`xmlns:atom="https://atomui.net"`）

---

## 枚举类型

### ButtonType

按钮类型枚举，定义按钮的视觉风格。

| 值 | 说明 |
|---|---|
| `Default` | 默认按钮，白色背景 + 边框 |
| `Dashed` | 虚线按钮，虚线边框 |
| `Primary` | 主按钮，主色调填充背景 |
| `Link` | 链接按钮，无边框无背景，文字呈链接色 |
| `Text` | 文本按钮，无边框无背景，文字为默认色 |

### ButtonShape

按钮形状枚举。

| 值 | 说明 |
|---|---|
| `Default` | 默认矩形（带圆角） |
| `Circle` | 圆形按钮 |
| `Round` | 胶囊形（圆角等于高度一半） |

### SizeType（来自 `AtomUI.Controls`）

尺寸类型枚举。

| 值 | 说明 |
|---|---|
| `Small` | 小号 |
| `Middle` | 中号（默认） |
| `Large` | 大号 |

---

## 公共属性（StyledProperty）

| 属性名 | 类型 | 默认值 | 说明 |
|---|---|---|---|
| `ButtonType` | `ButtonType` | `ButtonType.Default` | 按钮类型，控制视觉风格 |
| `Shape` | `ButtonShape` | `ButtonShape.Default` | 按钮形状 |
| `IsDanger` | `bool` | `false` | 是否为危险按钮，启用后按钮使用红色系 |
| `IsGhost` | `bool` | `false` | 幽灵模式，背景透明，文字和边框使用容器色/主题色 |
| `IsLoading` | `bool` | `false` | 加载中状态，显示旋转加载图标并降低不透明度 |
| `SizeType` | `SizeType` | `SizeType.Middle` | 按钮尺寸（共享属性，通过 `AddOwner` 注册） |
| `Icon` | `PathIcon?` | `null` | 按钮图标，使用 Ant Design 图标集 |
| `IsMotionEnabled` | `bool` | 跟随全局 Token | 是否启用过渡动画（共享属性） |
| `IsWaveSpiritEnabled` | `bool` | 跟随全局 Token | 是否启用点击波纹效果（共享属性） |

### 继承自 Avalonia.Controls.Button 的常用属性

| 属性名 | 类型 | 默认值 | 说明 |
|---|---|---|---|
| `Content` | `object?` | `null` | 按钮文本内容 |
| `ContentTemplate` | `IDataTemplate?` | `null` | 内容模板 |
| `Command` | `ICommand?` | `null` | 点击命令 |
| `CommandParameter` | `object?` | `null` | 命令参数 |
| `HorizontalAlignment` | `HorizontalAlignment` | `Left` | 水平对齐（设为 `Stretch` 实现 Block 模式） |
| `HorizontalContentAlignment` | `HorizontalAlignment` | `Center` | 内容水平对齐 |
| `VerticalContentAlignment` | `VerticalAlignment` | `Center` | 内容垂直对齐 |
| `IsEnabled` | `bool` | `true` | 是否启用 |
| `Foreground` | `IBrush?` | 由主题控制 | 前景色（文本颜色） |
| `Background` | `IBrush?` | 由主题控制 | 背景色 |
| `BorderBrush` | `IBrush?` | 由主题控制 | 边框颜色 |
| `BorderThickness` | `Thickness` | 由 Token 控制 | 边框粗细 |
| `CornerRadius` | `CornerRadius` | 由 Token 控制 | 圆角半径 |
| `FontSize` | `double` | 由 Token 控制 | 字体大小 |
| `Padding` | `Thickness` | 由 Token 控制 | 内间距 |
| `Cursor` | `Cursor` | `Hand` | 鼠标光标 |

---

## 事件

| 事件名 | 类型 | 说明 |
|---|---|---|
| `Click` | `EventHandler<RoutedEventArgs>` | 按钮点击事件（继承自 `Avalonia.Controls.Button`） |

---

## 伪类（Pseudo-Classes）

Button 支持以下伪类，可在样式选择器中使用：

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

### 继承的标准伪类

| 伪类 | 触发条件 |
|---|---|
| `:pointerover` | 鼠标悬浮 |
| `:pressed` | 按钮按下 |
| `:disabled` | `IsEnabled == false` |
| `:focus` | 获得焦点 |
| `:focus-visible` | 通过键盘获得焦点（显示焦点框） |

---

## 实现的接口

### ISizeTypeAware

```csharp
public SizeType SizeType { get; set; }
```

### IWaveSpiritAwareControl

```csharp
public bool IsWaveSpiritEnabled { get; set; }
```

### ICompactSpaceAware

在 `Space.Compact` 容器中使用时自动调整圆角。Button 提供以下行为：

- 根据在紧凑空间中的位置自动裁剪圆角
- `ButtonType == Primary` 时始终处于活跃 ZIndex 层

### IFormItemAware

可作为 `FormItem` 的子控件参与表单验证流程。

