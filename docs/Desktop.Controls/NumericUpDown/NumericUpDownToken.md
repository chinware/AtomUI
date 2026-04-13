# NumericUpDown Design Token

NumericUpDown 控件使用 `NumericUpDownToken`（Token ID: `"NumericUpDown"`）作为组件级 Design Token。`NumericUpDownToken` 本身不定义任何新的 Token 属性，而是继承自 `ButtonSpinnerToken`（继承自 `LineEditToken`），复用步进按钮和输入框的全部 Token。所有视觉属性均从全局 `SharedToken` 派生，不硬编码任何具体值。

---

## Token 继承链

```
AbstractControlDesignToken
  └── LineEditToken (ID: "LineEdit")
        └── ButtonSpinnerToken (ID: "ButtonSpinner")
              └── NumericUpDownToken (ID: "NumericUpDown")
```

NumericUpDown 的视觉表现由三层 Token 共同决定：
- **LineEditToken**：控制输入框字体大小
- **ButtonSpinnerToken**：控制步进按钮的尺寸、颜色和背景
- **NumericUpDownToken**：继承上述所有 Token，不增加额外定义

---

## Token 资源访问方式

在 AXAML 中使用组件级 Token（通过 ButtonSpinner 的 Token 资源前缀访问）：

```xml
{atom:ButtonSpinnerTokenResource HandleWidth}
{atom:ButtonSpinnerTokenResource HandleBg}
{atom:ButtonSpinnerTokenResource ControlWidth}
```

在 AXAML 中使用全局共享 Token：

```xml
{atom:SharedTokenResource ColorPrimary}
{atom:SharedTokenResource ControlHeight}
{atom:SharedTokenResource EnableMotion}
```

---

## 继承的 Token 一览

### 输入框字体 Token（来自 LineEditToken）

| Token 名 | 类型 | 来源 | 说明 |
|---|---|---|---|
| `InputFontSize` | `double` | `SharedToken.FontSize` | 中号输入框字体大小 |
| `InputFontSizeLG` | `double` | `SharedToken.FontSizeLG` | 大号输入框字体大小 |
| `InputFontSizeSM` | `double` | `SharedToken.FontSizeSM` | 小号输入框字体大小 |

### 步进按钮 Token（来自 ButtonSpinnerToken）

| Token 名 | 类型 | 来源 | 说明 |
|---|---|---|---|
| `ControlWidth` | `double` | 固定值 `90` | 输入框默认宽度 |
| `HandleWidth` | `double` | `SharedToken.ControlHeightSM` | 步进按钮区域宽度 |
| `HandleIconSize` | `double` | `SharedToken.FontSize / 2` | 步进按钮图标大小（上下箭头） |
| `HandleBg` | `Color` | `SharedToken.ColorBgContainer` | 步进按钮背景色 |
| `HandleActiveBg` | `Color` | `SharedToken.ColorFillAlter` | 步进按钮激活（按下）背景色 |
| `HandleHoverColor` | `Color` | `SharedToken.ColorPrimary` | 步进按钮悬浮时图标颜色 |
| `HandleBorderColor` | `Color` | `SharedToken.ColorBorder` | 步进按钮边框颜色 |
| `FilledHandleBg` | `Color` | 基于 `SharedToken.ColorFillSecondary` 混合 | Filled 变体下步进按钮背景色 |

---

## 主题中使用的全局 SharedToken

除组件级 Token 外，NumericUpDown 的 ControlTheme 还直接引用了以下全局 `SharedToken`：

| Token 资源键 | 使用场景 |
|---|---|
| `EnableMotion` | 全局动画开关，控制 `IsMotionEnabled` 默认值 |
| `UniformlyPaddingXXS` | 清除按钮与右侧内容之间的间距（StackPanel.Spacing） |
| `ColorTextPlaceholder` | 占位文本的默认前景色 |
| `ControlHeight` / `ControlHeightSM` / `ControlHeightLG` | 三种尺寸的控件高度 |
| `BorderRadius` / `BorderRadiusSM` / `BorderRadiusLG` | 三种尺寸的圆角半径 |
| `BorderThickness` | 输入框默认边框厚度 |
| `ColorPrimary` / `ColorPrimaryHover` / `ColorPrimaryActive` | 聚焦/悬浮时的边框和步进按钮颜色 |
| `ColorError` / `ColorErrorBorderHover` | Error 状态的边框颜色 |
| `ColorWarning` / `ColorWarningBorderHover` | Warning 状态的边框颜色 |
| `ColorBgContainer` / `ColorBgContainerDisabled` | 容器背景色（正常/禁用） |
| `ColorTextDisabled` | 禁用态文本颜色 |
| `ColorBorder` | 默认边框颜色 |

---

## Token 对外观的具体影响

### 样式变体与 Token 映射

| 样式变体 | 正常态 | 悬浮态 | 聚焦态 | 禁用态 |
|---|---|---|---|---|
| **Outline** | `ColorBorder` 边框 + `ColorBgContainer` 背景 | `ColorPrimaryHover` 边框 | `ColorPrimary` 边框 | `ColorBgContainerDisabled` 背景 |
| **Filled** | 无边框 + `ColorFillAlter` 背景 | `ColorFillSecondary` 背景 | `ColorPrimary` 边框 | `ColorBgContainerDisabled` 背景 |
| **Borderless** | 无边框 + 透明背景 | 透明背景 | `ColorPrimary` 底部边框 | 透明背景 |

### 验证状态与 Token 映射

| 验证状态 | 边框颜色 | 悬浮边框 |
|---|---|---|
| **Default** | `ColorBorder` | `ColorPrimaryHover` |
| **Error** | `ColorError` | `ColorErrorBorderHover` |
| **Warning** | `ColorWarning` | `ColorWarningBorderHover` |

### 尺寸与 Token 映射

| 尺寸 | 高度 | 字号 | 圆角 | 步进按钮宽度 |
|---|---|---|---|---|
| `Large` | `ControlHeightLG` | `InputFontSizeLG` | `BorderRadiusLG` | `HandleWidth` |
| `Middle` | `ControlHeight` | `InputFontSize` | `BorderRadius` | `HandleWidth` |
| `Small` | `ControlHeightSM` | `InputFontSizeSM` | `BorderRadiusSM` | `HandleWidth` |

### 步进按钮交互状态

| 状态 | 背景色 | 图标颜色 |
|---|---|---|
| 正常 | `HandleBg` | 默认文本色 |
| 悬浮 | `HandleBg` | `HandleHoverColor`（主色） |
| 按下 | `HandleActiveBg` | `HandleHoverColor` |
| 禁用（到达边界） | `HandleBg` | `ColorTextDisabled` |

---

## 自定义 Token

由于 `NumericUpDownToken` 继承自 `ButtonSpinnerToken`，自定义 NumericUpDown 的视觉表现需要同时考虑 ButtonSpinner 级别的 Token。如果需要全局修改所有数字输入框的步进按钮样式，修改 `ButtonSpinnerToken` 即可同时影响 `NumericUpDown` 和 `ButtonSpinner`。
