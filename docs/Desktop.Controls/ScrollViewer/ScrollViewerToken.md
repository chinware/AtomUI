# ScrollViewer Design Token

ScrollViewer 控件使用 `ScrollViewerToken`（Token ID: `"ScrollViewer"`）作为组件级 Design Token，所有视觉属性均从全局 `SharedToken` 派生，不硬编码任何具体值。

---

## Token 资源访问方式

在 AXAML 中使用组件级 Token：

```xml
{atom:ScrollViewerTokenResource ThumbBg}
{atom:ScrollViewerTokenResource NormalModeThumbThickness}
{atom:ScrollViewerTokenResource LiteModeThumbThickness}
```

在 AXAML 中使用全局共享 Token：

```xml
{atom:SharedTokenResource ColorBgContainer}
{atom:SharedTokenResource EnableMotion}
```

---

## 组件级 Token 一览

以下是 `ScrollViewerToken` 定义的全部组件级 Token，按功能分组说明。

### 滑块粗细 Token

| Token 名 | 类型 | 来源 | 说明 |
|---|---|---|---|
| `LiteModeThumbThickness` | `double` | `SharedToken.LineWidthBold` | 极简模式下滑块的粗细（约 2-3px 细线） |
| `NormalModeThumbThickness` | `double` | `SharedToken.SizeXS` | 标准模式下滑块的粗细（约 8px） |

### 滑块外观 Token

| Token 名 | 类型 | 来源 | 说明 |
|---|---|---|---|
| `ThumbCornerRadius` | `CornerRadius` | `NormalModeThumbThickness / 2` | 滑块圆角半径（完全圆角，呈药丸形） |
| `ThumbBg` | `Color` | `SharedToken.ColorBorder` | 滑块正常态背景色 |
| `ThumbHoverBg` | `Color` | 亮色主题：`ThumbBg.Darken()`；暗色主题：`ThumbBg.Lighten()` | 滑块鼠标悬浮背景色 |
| `ThumbActiveBg` | `Color` | 亮色主题：`ThumbHoverBg.Darken()`；暗色主题：`ThumbHoverBg.Lighten()` | 滑块鼠标按下/拖拽背景色 |

### 内间距 Token

| Token 名 | 类型 | 来源 | 说明 |
|---|---|---|---|
| `ScrollBarContentHPadding` | `Thickness` | `SharedToken.UniformlyPaddingXXS` (水平方向) | 水平滚动条的内间距（左右留白，使滑块不紧贴边缘） |
| `ScrollBarContentVPadding` | `Thickness` | `SharedToken.UniformlyPaddingXXS` (垂直方向) | 垂直滚动条的内间距（上下留白，使滑块不紧贴边缘） |

---

## 暗色主题适配

`ScrollViewerToken` 在 `CalculateTokenValues(bool isDarkMode)` 中根据主题模式调整滑块颜色：

| 主题 | ThumbBg | ThumbHoverBg | ThumbActiveBg |
|---|---|---|---|
| **亮色（Light）** | `ColorBorder` | `ColorBorder` 加深（`.Darken()`） | 再次加深 |
| **暗色（Dark）** | `ColorBorder` | `ColorBorder` 提亮（`.Lighten()`） | 再次提亮 |

这确保了滑块在亮色/暗色背景下都有足够的对比度和交互反馈。

---

## 主题中使用的全局 SharedToken

除组件级 Token 外，ScrollViewer 的 ControlTheme 还直接引用了以下全局 `SharedToken`：

| Token 资源键 | 使用场景 |
|---|---|
| `ColorBgContainer` | 滚动条分隔面板（`ScrollBarsSeparator`）的背景色 |
| `EnableMotion` | 全局动画开关（控制 `IsMotionEnabled` 的默认值） |

---

## Token 对外观的具体影响

### 滑块粗细与模式映射

| 模式 | 正常粗细 | 悬浮/拖拽粗细 |
|---|---|---|
| **标准模式**（`IsLiteMode = false`） | `NormalModeThumbThickness`（`SizeXS`，约 8px） | 同正常粗细（无变化） |
| **极简模式**（`IsLiteMode = true`） | `LiteModeThumbThickness`（`LineWidthBold`，约 2-3px） | `NormalModeThumbThickness`（膨胀到标准粗细） |

### 滑块颜色状态映射

| 状态 | 背景色 Token |
|---|---|
| 正常 | `ThumbBg` |
| 鼠标悬浮（`:pointerover`） | `ThumbHoverBg` |
| 鼠标按下/拖拽（`:pressed`） | `ThumbActiveBg` |

### 方向与属性映射

| 滑块方向 | 粗细属性 | 内间距 Token |
|---|---|---|
| 垂直（`Orientation = Vertical`） | `Width` | `ScrollBarContentVPadding` |
| 水平（`Orientation = Horizontal`） | `Height` | `ScrollBarContentHPadding` |

### 圆角

滑块始终使用 `ThumbCornerRadius`（= `NormalModeThumbThickness / 2`），使其呈现为药丸形/胶囊形，与 macOS 系统滚动条风格一致。
