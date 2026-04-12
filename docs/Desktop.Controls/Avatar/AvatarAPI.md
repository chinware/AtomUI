# Avatar API 参考

## 命名空间

```csharp
namespace AtomUI.Desktop.Controls;          // Avatar, AvatarGroup
namespace AtomUI.Controls.Commons;           // AbstractAvatar, AvatarPseudoClass
namespace AtomUI.Controls;                   // AvatarShape, CustomizableSizeType
```

AXAML 命名空间前缀：`atom`（`xmlns:atom="https://atomui.net"`）

---

## 枚举类型

### AvatarShape

头像形状枚举，定义头像的外轮廓形状。

| 值 | 说明 |
|---|---|
| `Circle` | 圆形（默认），`CornerRadius = Width / 2` 实现完全圆角 |
| `Square` | 方形，带 Token 定义的圆角（`BorderRadius` / `BorderRadiusSM` / `BorderRadiusLG`） |

### CustomizableSizeType（来自 `AtomUI.Controls`）

可自定义的尺寸类型枚举，比标准 `SizeType` 多一个 `Custom` 值。

| 值 | 说明 |
|---|---|
| `Large` | 大号 |
| `Middle` | 中号（默认） |
| `Small` | 小号 |
| `Custom` | 自定义尺寸（设置 `Size` 属性后自动切换） |

### FlyoutTriggerType（来自 `AtomUI.Controls`）

弹出触发类型枚举，用于 AvatarGroup 的折叠头像弹出。

| 值 | 说明 |
|---|---|
| `Hover` | 鼠标悬浮触发（默认） |
| `Click` | 鼠标点击触发 |

---

## Avatar 公共属性（StyledProperty）

| 属性名 | 类型 | 默认值 | 说明 |
|---|---|---|---|
| `Gap` | `double` | `4.0` | 文字距离左右两侧的像素间距，用于控制文字自适应缩放的留白空间 |
| `Icon` | `PathIcon?` | `null` | 图标内容，使用 Ant Design 图标集提供 |
| `BitmapSrc` | `IImage?` | `null` | 位图图片源，支持 Bitmap/CroppedBitmap 等 `IImage` 实现 |
| `Src` | `string?` | `null` | SVG 图片路径（`avares://` URI 或文件路径） |
| `Text` | `string?` | `null` | 文字内容（标记了 `[Content]` 属性，支持 AXAML 直接嵌套文字） |
| `SizeType` | `CustomizableSizeType` | `Middle` | 尺寸类型，支持预设三种尺寸及自定义。设置 `Size` 后自动切换为 `Custom` |
| `Size` | `double` | `NaN` | 自定义尺寸像素值。设置为非 NaN 值时，`SizeType` 自动切换为 `Custom` |
| `Shape` | `AvatarShape` | `Circle` | 头像形状（圆形或方形） |
| `IsMotionEnabled` | `bool` | 跟随全局 Token `EnableMotion` | 是否启用过渡动画（共享属性，通过 `AddOwner` 注册） |

### 继承自 TemplatedControl 的常用属性

| 属性名 | 类型 | 默认值 | 说明 |
|---|---|---|---|
| `Background` | `IBrush?` | 由主题控制 | 背景色（Icon/Text 模式使用 `AvatarBg`，图片模式透明） |
| `Foreground` | `IBrush?` | 由主题控制 | 前景色（图标和文字颜色，默认 `AvatarColor`） |
| `BorderBrush` | `IBrush?` | 由主题控制 | 边框颜色（默认 `GroupBorderColor`） |
| `BorderThickness` | `Thickness` | 由 Token 控制 | 边框粗细 |
| `CornerRadius` | `CornerRadius` | 由形状和尺寸自动计算 | 圆角半径（Circle 模式 = Width/2，Square 模式由 Token 控制） |
| `Width` / `Height` | `double` | 由 SizeType/Size 控制 | 控件尺寸 |
| `FontSize` | `double` | 由 Token 控制 | 字体大小（影响文字头像的文字渲染） |
| `FontFamily` | `FontFamily` | 继承 | 字体（影响文字缩放计算） |
| `HorizontalAlignment` | `HorizontalAlignment` | `Left` | 水平对齐 |
| `VerticalAlignment` | `VerticalAlignment` | `Top` | 垂直对齐 |
| `IsEnabled` | `bool` | `true` | 是否启用 |

---

## AvatarGroup 公共属性（StyledProperty）

| 属性名 | 类型 | 默认值 | 说明 |
|---|---|---|---|
| `MaxDisplayCount` | `int?` | `null` | 最大显示数量。超出部分折叠为 `+N` 计数头像，`null` 表示不限制 |
| `SizeType` | `CustomizableSizeType` | `Middle` | 统一尺寸类型，自动传递给所有子 Avatar（通过 `AddOwner` 共享） |
| `Size` | `double` | `NaN` | 统一自定义尺寸像素值（通过 `AddOwner` 共享） |
| `Shape` | `AvatarShape` | `Circle` | 统一形状，自动传递给所有子 Avatar（通过 `AddOwner` 共享） |
| `FoldInfoAvatarBackground` | `IBrush?` | 由主题控制（`AvatarBg`） | 折叠计数头像的背景色 |
| `FoldInfoAvatarForeground` | `IBrush?` | 由主题控制（`AvatarColor`） | 折叠计数头像的前景色（文字颜色） |
| `FoldAvatarFlyoutTriggerType` | `FlyoutTriggerType` | `Hover` | 折叠头像弹出触发方式（悬浮或点击） |
| `IsMotionEnabled` | `bool` | 跟随全局 Token `EnableMotion` | 是否启用过渡动画 |
| `Children` | `Controls` | 空集合 | 子 Avatar 控件集合（标记了 `[Content]`，支持 AXAML 直接嵌套子元素） |

### AvatarGroup 继承自 TemplatedControl 的常用属性

| 属性名 | 类型 | 默认值 | 说明 |
|---|---|---|---|
| `BorderThickness` | `Thickness` | 由 Token 控制（`SharedToken.BorderThickness`） | 统一边框粗细，自动传递给所有子 Avatar |
| `HorizontalAlignment` | `HorizontalAlignment` | `Left` | 水平对齐 |
| `VerticalAlignment` | `VerticalAlignment` | `Top` | 垂直对齐 |

---

## 伪类（Pseudo-Classes）

Avatar 支持以下伪类（定义在 `AvatarPseudoClass` 中），可在样式选择器中使用：

| 伪类 | 常量 | 触发条件 |
|---|---|---|
| `:large` | `AvatarPseudoClass.Large` | `SizeType == Large` |
| `:middle` | `AvatarPseudoClass.Middle` | `SizeType == Middle` |
| `:small` | `AvatarPseudoClass.Small` | `SizeType == Small` |
| `:custom-size` | `AvatarPseudoClass.Custom` | `SizeType == Custom`（设置了 `Size` 属性） |

### 注意

Avatar 的 ControlTheme 实际使用属性选择器（如 `[SizeType=Large]`、`[ContentType=Icon]`、`[Shape=Square]`）而非伪类来驱动样式切换。伪类常量在 `AvatarPseudoClass` 中定义但当前主要用于扩展场景。

### 继承的标准伪类

| 伪类 | 触发条件 |
|---|---|
| `:pointerover` | 鼠标悬浮 |
| `:disabled` | `IsEnabled == false` |
| `:focus` | 获得焦点 |
| `:focus-visible` | 通过键盘获得焦点 |

---

## 实现的接口

### IMotionAwareControl（Avatar / AvatarGroup）

```csharp
public bool IsMotionEnabled { get; set; }
```

控制是否启用过渡动画。默认跟随全局 Token `EnableMotion` 的值。

---

## 内部属性（仅供主题/扩展使用）

以下属性为 `internal` 可见性，主题 AXAML 中可通过 `TemplateBinding` 引用：

### Avatar 内部属性

| 属性名 | 类型 | 说明 |
|---|---|---|
| `EffectiveIconSize` | `double` | 运行时计算的实际图标/文字图标尺寸 |
| `TextRenderTransform` | `ITransform?` | 文字自适应缩放变换矩阵 |
| `ContentType` | `AvatarContentType` | 当前内容类型（`Icon`/`BitmapImage`/`SvgImage`/`Text`），驱动模板内容切换 |

### AvatarGroup 内部属性

| 属性名 | 类型 | 说明 |
|---|---|---|
| `GroupSpace` | `double` | 头像组子项间距（由 `AvatarToken.GroupSpace` 控制） |
| `GroupOverlapping` | `double` | 头像重叠宽度（由 `AvatarToken.GroupOverlapping` 控制） |

---

## 内容优先级

当同时设置多个内容属性时，按以下优先级显示：

| 优先级 | 属性 | 内容类型 |
|---|---|---|
| 1（最高） | `Src` | SVG 图片 |
| 2 | `BitmapSrc` | 位图图片 |
| 3 | `Text` | 文字 |
| 4（最低/默认） | `Icon` | 图标 |
