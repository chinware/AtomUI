# Badge API 参考

## 命名空间

```csharp
namespace AtomUI.Desktop.Controls;   // CountBadge, DotBadge, RibbonBadge
namespace AtomUI.Controls.Commons;    // 枚举类型、抽象基类
```

AXAML 命名空间前缀：`atom`（`xmlns:atom="https://atomui.net"`）

---

## 枚举类型

### CountBadgeSize

数字徽标尺寸枚举。

| 值 | 说明 |
|---|---|
| `Default` | 默认尺寸 |
| `Small` | 小号尺寸 |

### DotBadgeStatus

圆点徽标状态枚举。

| 值 | 说明 |
|---|---|
| `Default` | 默认状态（灰色） |
| `Success` | 成功状态（绿色） |
| `Processing` | 处理中状态（蓝色） |
| `Error` | 错误状态（红色） |
| `Warning` | 警告状态（黄色） |

### RibbonBadgePlacement

缎带徽标放置位置枚举。

| 值 | 说明 |
|---|---|
| `Start` | 左上角 |
| `End` | 右上角（默认） |

---

## CountBadge 公共属性（StyledProperty）

| 属性名 | 类型 | 默认值 | 说明 |
|---|---|---|---|
| `Count` | `int` | `0` | 显示的数字（最小值为 0，负值被 coerce 为 0） |
| `OverflowCount` | `int` | `99` | 封顶值，`Count > OverflowCount` 时显示 `{OverflowCount}+` |
| `ShowZero` | `bool` | `false` | `Count == 0` 时是否仍然显示徽标 |
| `Size` | `CountBadgeSize` | `CountBadgeSize.Default` | 徽标尺寸 |
| `DecoratedTarget` | `Control?` | `null` | 被装饰的目标控件（[Content] 属性，支持直接包裹子元素） |
| `BadgeColor` | `string?` | `null` | 自定义颜色（支持预设色名如 `"Blue"` 或 CSS 色值如 `"#faad14"`） |
| `Offset` | `Point` | `(0, 0)` | 标记相对于默认位置的偏移量 |
| `BadgeIsVisible` | `bool` | `true` | 控制徽标的显隐（配合动画） |
| `IsMotionEnabled` | `bool` | 跟随全局 Token | 是否启用显隐动画（共享属性） |

---

## DotBadge 公共属性（StyledProperty）

| 属性名 | 类型 | 默认值 | 说明 |
|---|---|---|---|
| `Status` | `DotBadgeStatus?` | `null` | 圆点状态，设置后自动应用对应语义色 |
| `DotColor` | `string?` | `null` | 自定义圆点颜色（支持预设色名或 CSS 色值） |
| `Text` | `string?` | `null` | 状态文本（独立模式下显示在圆点旁边） |
| `DecoratedTarget` | `Control?` | `null` | 被装饰的目标控件（[Content] 属性） |
| `Offset` | `Point` | `(0, 0)` | 标记相对于默认位置的偏移量 |
| `BadgeIsVisible` | `bool` | `false` | 控制徽标的显隐 |
| `IsMotionEnabled` | `bool` | 跟随全局 Token | 是否启用显隐动画（共享属性） |

---

## RibbonBadge 公共属性（StyledProperty）

| 属性名 | 类型 | 默认值 | 说明 |
|---|---|---|---|
| `Text` | `string?` | `null` | 缎带上显示的文字 |
| `RibbonColor` | `string?` | `null` | 缎带颜色（支持预设色名或 CSS 色值，默认使用 `ColorPrimary`） |
| `Placement` | `RibbonBadgePlacement` | `RibbonBadgePlacement.End` | 缎带放置位置（左上角 / 右上角） |
| `DecoratedTarget` | `Control?` | `null` | 被装饰的目标控件（[Content] 属性） |
| `Offset` | `Point` | `(0, 0)` | 缎带相对于默认位置的偏移量 |
| `BadgeIsVisible` | `bool` | `false` | 控制缎带的显隐 |

---

## 颜色属性说明

`BadgeColor`（CountBadge）、`DotColor`（DotBadge）、`RibbonColor`（RibbonBadge）均为 `string?` 类型，支持以下输入格式：

### 预设色名

不区分大小写的预设 Ant Design 调色板名：

| 预设色名 | 示例值 |
|---|---|
| `Pink` | `DotColor="Pink"` |
| `Red` | `BadgeColor="Red"` |
| `Yellow` | `DotColor="Yellow"` |
| `Orange` | `DotColor="Orange"` |
| `Cyan` | `RibbonColor="Cyan"` |
| `Green` | `RibbonColor="Green"` |
| `Blue` | `BadgeColor="Blue"` |
| `Purple` | `RibbonColor="purple"` |
| `GeekBlue` | `DotColor="GeekBlue"` |
| `Magenta` | `RibbonColor="magenta"` |
| `Volcano` | `RibbonColor="volcano"` |
| `Gold` | `DotColor="Gold"` |
| `Lime` | `DotColor="Lime"` |

### CSS 颜色值

任意 Avalonia 可解析的颜色字符串：

```xml
BadgeColor="#faad14"
BadgeColor="#f50"
DotColor="rgb(45, 183, 245)"
DotColor="hsl(102, 53%, 61%)"
RibbonColor="#52c41a"
```

---

## [Content] 属性

三种 Badge 的 `DecoratedTarget` 属性均标记了 `[Content]` 特性，因此可以直接将被装饰控件写在 Badge 标签内：

```xml
<!-- 简写（推荐） -->
<atom:CountBadge Count="5">
    <Border Width="40" Height="40" Background="rgb(191,191,191)" CornerRadius="8" />
</atom:CountBadge>

<!-- 等价的完整写法 -->
<atom:CountBadge Count="5">
    <atom:CountBadge.DecoratedTarget>
        <Border Width="40" Height="40" Background="rgb(191,191,191)" CornerRadius="8" />
    </atom:CountBadge.DecoratedTarget>
</atom:CountBadge>
```

---

## 实现的接口

### IMotionAwareControl（CountBadge、DotBadge）

```csharp
public bool IsMotionEnabled { get; set; }
```

控制 Badge 显示/隐藏时的缩放动画效果。当 `BadgeIsVisible` 变化时，若 `IsMotionEnabled == true`，则播放缩放动画。

> 注意：`RibbonBadge` 不实现此接口，缎带徽标无显隐动画。
