# Window Design Token

Window 控件使用 `WindowToken`（Token ID: `"Window"`）作为组件级 Design Token，所有视觉属性均从全局 `SharedToken` 派生，不硬编码任何具体值。

Window 的标题栏（`WindowTitleBar`）和标题按钮（`CaptionButton` / `CaptionButtonGroup`）使用独立的 `ChromeToken`（Token ID: `"Chrome"`）作为组件级 Design Token。

---

## Token 资源访问方式

在 AXAML 中使用 Window 组件级 Token：

```xml
{atom:WindowTokenResource DefaultBackground}
{atom:WindowTokenResource DefaultForeground}
{atom:WindowTokenResource CornerRadius}
```

在 AXAML 中使用 Chrome 组件级 Token（标题栏相关）：

```xml
{atom:ChromeTokenResource ForegroundColor}
{atom:ChromeTokenResource TitleBarPadding}
{atom:ChromeTokenResource CaptionButtonIconSize}
```

在 AXAML 中使用全局共享 Token：

```xml
{atom:SharedTokenResource ColorBgContainer}
{atom:SharedTokenResource ColorText}
{atom:SharedTokenResource FontSize}
{atom:SharedTokenResource FontFamily}
{atom:SharedTokenResource ColorBorder}
```

---

## WindowToken 组件级 Token 一览

`WindowToken` 定义了 Window 控件自身的 4 个组件级 Token：

| Token 名 | 类型 | 来源 | 说明 |
|---|---|---|---|
| `DefaultBackground` | `Color` | `SharedToken.ColorBgContainer` | 窗口默认背景色 |
| `DefaultForeground` | `Color` | `SharedToken.ColorText` | 窗口默认前景色（文本颜色） |
| `CornerRadius` | `CornerRadius` | 固定 `12`（仅 Linux 生效） | 窗口圆角半径 |
| `SystemBarColor` | `SolidColorBrush?` | `SharedToken.ColorBgContainer` | 系统状态栏颜色 |

### Token 派生逻辑

```csharp
public override void CalculateTokenValues(bool isDarkMode)
{
    DefaultBackground = SharedToken.ColorBgContainer;
    DefaultForeground = SharedToken.ColorText;
    CornerRadius      = new CornerRadius(12);
    SystemBarColor    = new SolidColorBrush(SharedToken.ColorBgContainer);
}
```

> 注意：`CornerRadius` 在主题样式中仅对 Linux 平台生效，Windows 和 macOS 通过 `:not(^[OsType=Linux])` 选择器将圆角重设为 `0`。最大化/全屏时也被重设为 `0`。

---

## ChromeToken 组件级 Token 一览

`ChromeToken` 定义了标题栏和标题按钮相关的 Token，按功能分组说明。

### 标题栏布局 Token

| Token 名 | 类型 | 来源 | 说明 |
|---|---|---|---|
| `TitleBarPadding` | `Thickness` | 基于 `SharedToken.SizeUnit` 计算 | 标题栏内间距 |
| `LogoAndTitleSpacing` | `double` | `SharedToken.SizeUnit * 2` | Logo 与标题文本之间的间距 |
| `LogoSize` | `double` | `SharedToken.SizeUnit * 4` | 应用程序 Logo 图标大小 |
| `MinHeight` | `double` | 固定 `40` | 标题栏最小高度 |

### 标题字体 Token

| Token 名 | 类型 | 来源 | 说明 |
|---|---|---|---|
| `TitleFontSize` | `double` | 固定 `13` | 窗口标题默认字号 |
| `TitleFontWeight` | `FontWeight` | `FontWeight.Bold` | 窗口标题默认字重 |

### 标题按钮 Token

| Token 名 | 类型 | 来源 | 说明 |
|---|---|---|---|
| `CaptionButtonIconSize` | `double` | `SharedToken.IconSizeSM` | 标题按钮图标大小 |
| `CaptionButtonPadding` | `Thickness` | `SharedToken.SizeUnit * 2` | 标题按钮内间距 |
| `CaptionGroupSpacing` | `double` | `SharedToken.SizeUnit * 2` | 标题按钮组内按钮间距 |
| `ForegroundColor` | `Color` | `SharedToken.ColorTextSecondary` | 按钮前景色（图标颜色） |
| `HoverBackgroundColor` | `Color` | `SharedToken.ColorBgTextHover` | 按钮 Hover 背景色 |
| `PressedBackgroundColor` | `Color` | `SharedToken.ColorBgTextActive` | 按钮按下背景色 |
| `CloseHoverBackgroundColor` | `Color` | `SharedToken.ColorErrorTextActive` | 关闭按钮 Hover 背景色（红色） |
| `ClosePressedBackgroundColor` | `Color` | `SharedToken.ColorErrorTextHover` | 关闭按钮按下背景色 |

### 窗口激活/未激活状态 Token

| Token 名 | 类型 | 来源 | 说明 |
|---|---|---|---|
| `ActiveColor` | `Color` | `SharedToken.ColorTextSecondary` | 窗口激活状态下的标题按钮前景色 |
| `InactiveColor` | `Color` | `SharedToken.ColorTextQuaternary` | 窗口未激活状态下的标题按钮前景色 |
| `ActiveBgColor` | `Color` | `SharedToken.ColorFillTertiary` | 激活状态按钮默认背景色 |
| `ActiveHoverBgColor` | `Color` | `SharedToken.ColorFillSecondary` | 激活状态按钮 Hover 背景色 |
| `ActivePressedBgColor` | `Color` | `SharedToken.ColorFill` | 激活状态按钮按下背景色 |
| `InactiveBgColor` | `Color` | `SharedToken.ColorFillQuaternary` | 未激活状态按钮默认背景色 |
| `InactiveHoverBgColor` | `Color` | `SharedToken.ColorFillTertiary` | 未激活状态按钮 Hover 背景色 |

### Windows 平台关闭按钮 Token

| Token 名 | 类型 | 来源 | 说明 |
|---|---|---|---|
| `WindowsCloseButtonHoverColor` | `Color` | 基于白色和红色背景计算 | Windows 关闭按钮 Hover 前景色 |
| `WindowsCloseButtonHoverBgColor` | `Color` | `#F44336`（Material Red） | Windows 关闭按钮 Hover 背景色 |
| `WindowsCloseButtonPressedBgColor` | `Color` | `rgba(244, 67, 54, 0.75)` | Windows 关闭按钮按下背景色 |

---

## 主题中使用的全局 SharedToken

除组件级 Token 外，Window 的 ControlTheme 还直接引用了以下全局 `SharedToken`：

| Token 资源键 | 使用场景 |
|---|---|
| `FontSize` | 窗口默认字体大小 |
| `FontFamily` | 窗口默认字体族 |
| `ColorBorder` | Linux 平台窗口边框颜色 |
| `ColorBgContainer` | 窗口背景色来源（通过 `WindowToken.DefaultBackground` 间接引用） |
| `ColorText` | 窗口前景色来源（通过 `WindowToken.DefaultForeground` 间接引用） |
| `SizeUnit` | 标题栏间距计算基础单位 |
| `IconSizeSM` | 标题按钮图标大小 |
| `ColorTextSecondary` | 标题按钮前景色 |
| `ColorTextQuaternary` | 未激活窗口标题按钮前景色 |
| `ColorBgTextHover` / `ColorBgTextActive` | 标题按钮 Hover/Active 背景色 |
| `ColorFill` / `ColorFillSecondary` / `ColorFillTertiary` / `ColorFillQuaternary` | 标题按钮激活/未激活状态背景色梯度 |
| `ColorErrorTextActive` / `ColorErrorTextHover` | 关闭按钮红色系背景色 |

---

## Token 对外观的具体影响

### 主题切换

| 场景 | 受影响的 Token | 效果 |
|---|---|---|
| **亮色 → 暗色** | `DefaultBackground`、`DefaultForeground`、`SystemBarColor` | 窗口背景色自动从浅色切换为深色，前景色反转 |
| **窗口激活/未激活** | `ActiveColor` ↔ `InactiveColor`、`ActiveBgColor` ↔ `InactiveBgColor` | 标题按钮前景色和背景色随窗口焦点状态变化 |

### 平台差异

| 平台 | Token 应用差异 |
|---|---|
| **Windows** | 关闭按钮使用 `WindowsCloseButtonHoverBgColor`（红色）和 `WindowsCloseButtonHoverColor`（白色） |
| **macOS** | 标题按钮由系统原生红绿灯实现，Chrome Token 中的按钮色不影响原生按钮 |
| **Linux** | `CornerRadius = 12` 生效，窗口带圆角；`ColorBorder` 用于 1px 边框 |

### 圆角控制

| 状态 | 圆角值 | 说明 |
|---|---|---|
| Linux 正常状态 | `WindowToken.CornerRadius`（12px） | Linux 无系统装饰，由 AtomUI 自行绘制圆角边框 |
| Windows / macOS 正常状态 | `0` | 系统自行管理窗口圆角 |
| 最大化 / 全屏 | `0` | 任何平台下最大化/全屏都清除圆角 |
