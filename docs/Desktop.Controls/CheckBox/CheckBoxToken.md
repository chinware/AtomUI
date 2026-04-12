# CheckBox Design Token

CheckBox 控件使用 `CheckBoxToken`（Token ID: `"CheckBox"`）作为组件级 Design Token，所有视觉属性均从全局 `SharedToken` 派生，不硬编码任何具体值。

---

## Token 资源访问方式

在 AXAML 中使用组件级 Token：

```xml
{atom:CheckBoxTokenResource CheckIndicatorSize}
{atom:CheckBoxTokenResource CheckedMarkSize}
{atom:CheckBoxTokenResource TextMargin}
```

在 AXAML 中使用全局共享 Token：

```xml
{atom:SharedTokenResource ColorPrimary}
{atom:SharedTokenResource ColorBorder}
{atom:SharedTokenResource BorderRadiusSM}
```

---

## 组件级 Token 一览

以下是 `CheckBoxToken` 定义的全部组件级 Token。

### 指示器 Token

| Token 名 | 类型 | 来源 | 说明 |
|---|---|---|---|
| `CheckIndicatorSize` | `double` | `SharedToken.ControlInteractiveSize` | 复选框指示器（方框）的大小 |
| `CheckedMarkSize` | `double` | `CheckIndicatorSize × 0.6` | 选中对勾图标的大小 |
| `IndicatorTristateMarkSize` | `double` | `SharedToken.FontSizeLG / 2` | 不确定态横杠的大小 |

### 间距 Token

| Token 名 | 类型 | 来源 | 说明 |
|---|---|---|---|
| `TextMargin` | `Thickness` | `SharedToken.UniformlyMarginXS, 0` | 文本与指示器之间的左右间距 |

---

## 主题中使用的全局 SharedToken

除组件级 Token 外，CheckBox 的 ControlTheme 还直接引用了以下全局 `SharedToken`：

| Token 资源键 | 使用场景 |
|---|---|
| `ColorText` | 复选框文本颜色 |
| `ColorTextDisabled` | 禁用态文本颜色 |
| `ColorPrimary` | 选中态指示器背景色、半选态横杠颜色 |
| `ColorPrimaryHover` | 悬浮态指示器边框色（选中时为背景色） |
| `ColorBgContainer` | 未选中态指示器背景色 |
| `ColorBgContainerDisabled` | 禁用态指示器背景色 |
| `ColorBorder` | 未选中态指示器边框色、禁用态边框色 |
| `BorderThickness` | 指示器边框粗细（应用于 `CheckBoxIndicator`，而非 `CheckBox` 自身） |
| `BorderRadiusSM` | 指示器圆角半径 |
| `EnableMotion` | 全局动画开关 |
| `EnableWaveSpirit` | 全局波纹开关 |
| `SpacingXS` | CheckBoxGroup 子项间距（`ItemSpacing` / `LineSpacing` 默认值） |

> ⚠️ **已知偏差**：选中态对勾颜色（`CheckedMarkBrush`）在当前主题中使用了硬编码的 `White`，而非通过 Token 资源引用（如 `ColorTextLightSolid`）。这意味着在深色主题下对勾仍为白色，通常视觉效果正确，但不完全遵循 Token 系统。

---

## Token 对外观的具体影响

### 选中状态与 Token 映射

| 状态 | 指示器背景 | 指示器边框 | 对勾/横杠颜色 | 文本颜色 |
|---|---|---|---|---|
| **未选中** | `ColorBgContainer` | `ColorBorder` | — | `ColorText` |
| **未选中 + 悬浮** | `ColorBgContainer` | `ColorPrimaryHover` | — | `ColorText` |
| **选中** | `ColorPrimary` | `ColorPrimary` | `White` | `ColorText` |
| **选中 + 悬浮** | `ColorPrimaryHover` | `ColorPrimaryHover` | `White` | `ColorText` |
| **不确定** | `ColorBgContainer` | `ColorBorder` | `ColorPrimary` | `ColorText` |
| **不确定 + 悬浮** | `ColorBgContainer` | `ColorPrimaryHover` | `ColorPrimary` | `ColorText` |
| **禁用（未选中）** | `ColorBgContainerDisabled` | `ColorBorder` | — | `ColorTextDisabled` |
| **禁用（选中）** | `ColorBgContainerDisabled` | `ColorBorder` | `ColorTextDisabled` | `ColorTextDisabled` |
| **禁用（不确定）** | `ColorBgContainerDisabled` | `ColorBorder` | `ColorTextDisabled` | `ColorTextDisabled` |
