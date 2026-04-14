# TimePicker Design Token

TimePicker 控件使用 `TimePickerToken`（Token ID: `"TimePicker"`）作为组件级 Design Token，所有视觉属性均从全局 `SharedToken` 派生，不硬编码任何具体值。

---

## Token 资源访问方式

在 AXAML 中使用组件级 Token：

```xml
{atom:TimePickerTokenResource ItemHeight}
{atom:TimePickerTokenResource ItemWidth}
{atom:TimePickerTokenResource ItemPadding}
```

在 AXAML 中使用全局共享 Token：

```xml
{atom:SharedTokenResource ColorPrimary}
{atom:SharedTokenResource ControlHeight}
{atom:SharedTokenResource BorderThickness}
```

---

## 组件级 Token 一览

以下是 `TimePickerToken` 定义的全部组件级 Token，按功能分组说明。

### 面板项尺寸 Token

| Token 名 | 类型 | 来源 | 说明 |
|---|---|---|---|
| `ItemHeight` | `double` | `SharedToken.ControlHeight - 4` | 时间选择项的高度 |
| `ItemWidth` | `double` | 固定 `40` | 时间选择项的宽度（时/分/秒列） |
| `PeriodHostWidth` | `double` | 固定 `50` | AM/PM 选择列的宽度（12小时制时可见） |

### 间距 Token

| Token 名 | 类型 | 来源 | 说明 |
|---|---|---|---|
| `ItemPadding` | `Thickness` | `(0, SharedToken.UniformlyPaddingXXS)` | 时间选择项的内间距（上下） |
| `ButtonsMargin` | `Thickness` | `(0, SharedToken.UniformlyMarginXS, 0, 0)` | 按钮区域对上的外边距 |
| `HeaderMargin` | `Thickness` | `(0, 0, 0, 3)` | 头部文本区域的外边距 |

### 范围选择器 Token

| Token 名 | 类型 | 来源 | 说明 |
|---|---|---|---|
| `RangePickerArrowMargin` | `Thickness` | `(SharedToken.UniformlyMarginXS, 0)` | 范围选择箭头（→）的左右外间距 |
| `RangePickerIndicatorThickness` | `double` | `SharedToken.LineWidthFocus` | 范围选中指示器（下划线）的厚度 |

---

## 主题中使用的全局 SharedToken

除组件级 Token 外，TimePicker 的 ControlTheme 还直接引用了以下全局 `SharedToken`：

| Token 资源键 | 使用场景 |
|---|---|
| `BorderThickness` | 面板边框厚度 |
| `ColorBorderSecondary` | 面板内按钮区域上方的分隔线颜色 |
| `ControlHeight` / `ControlHeightSM` / `ControlHeightLG` | 控件高度（中/小/大） |
| `BorderRadius` / `BorderRadiusSM` / `BorderRadiusLG` | 圆角半径（中/小/大） |
| `ColorPrimary` / `ColorPrimaryHover` / `ColorPrimaryActive` | 主色调及其交互态（焦点边框、选中高亮） |
| `ColorError` / `ColorWarning` | 错误/警告色（Status 状态反馈） |
| `ColorText` / `ColorTextSecondary` / `ColorTextDisabled` | 文本色系 |
| `ColorBgContainer` / `ColorBgContainerDisabled` | 容器背景色 |
| `ColorBorder` | 边框颜色 |
| `ColorFocusBorder` | 焦点边框颜色 |
| `LineWidth` | 分隔线宽度 |
| `LineWidthFocus` | 焦点指示器宽度 |
| `UniformlyPaddingXXS` | 极小统一间距 |
| `UniformlyMarginXS` | 小号统一外边距 |
| `EnableMotion` | 全局动画开关 |
| `FontSize` | 字体大小 |

---

## Token 对外观的具体影响

### 面板尺寸

`ItemHeight` 和 `ItemWidth` 共同决定了弹出面板中每个时间单元格的大小。面板总高度由 `ItemHeight × SelectorRowCount`（默认 7 行）计算得出。`PeriodHostWidth` 仅在 12 小时制（`ClockIdentifier == HourClock12`）时影响 AM/PM 列的宽度。

### 按钮区域

`ButtonsMargin` 控制面板底部按钮区域与上方时间滚轮之间的间距。按钮区域的可见性由 `IsNeedConfirm` 和 `IsShowNow` 属性共同决定：

| IsNeedConfirm | IsShowNow | 按钮区域可见性 | 显示的按钮 |
|---|---|---|---|
| `false` | `true` | ✅ 可见 | 仅"此刻"按钮（居中） |
| `false` | `false` | ❌ 隐藏 | 无 |
| `true` | `true` | ✅ 可见 | "此刻"按钮（左对齐）+ "确定"按钮（右对齐） |
| `true` | `false` | ✅ 可见 | 仅"确定"按钮 |

### 范围选择器

`RangePickerArrowMargin` 控制起始/结束输入框之间箭头图标的左右间距。`RangePickerIndicatorThickness` 控制激活输入框下方彩色指示线的粗细。

### 样式变体与 Token 映射

| 样式变体 | 正常态 | 悬浮态 | 焦点态 | 禁用态 |
|---|---|---|---|---|
| **Outline** | `ColorBorder` 边框 + `ColorBgContainer` 背景 | `ColorPrimaryHover` 边框 | `ColorPrimary` 边框 | `ColorBgContainerDisabled` 背景 |
| **Filled** | 无边框 + `ColorFillTertiary` 背景 | `ColorFillSecondary` 背景 | `ColorPrimary` 边框 | `ColorBgContainerDisabled` 背景 |
| **Borderless** | 无边框 + 透明背景 | 轻微背景色变化 | `ColorPrimary` 底部边框 | `ColorTextDisabled` 文本 |

### 验证状态与 Token 映射

| 验证状态 | 边框颜色 | 焦点边框 | 图标颜色 |
|---|---|---|---|
| `Default` | `ColorBorder` | `ColorPrimary` | `ColorTextSecondary` |
| `Warning` | `ColorWarning` | `ColorWarning` | `ColorWarning` |
| `Error` | `ColorError` | `ColorError` | `ColorError` |

### 尺寸与 Token 映射

| 尺寸 | 高度 | 字号 | 圆角 |
|---|---|---|---|
| `Large` | `ControlHeightLG` | `FontSizeLG` | `BorderRadiusLG` |
| `Middle` | `ControlHeight` | `FontSize` | `BorderRadius` |
| `Small` | `ControlHeightSM` | `FontSize` | `BorderRadiusSM` |

> 注意：TimePicker 的输入框主题继承自 `InfoPickerInputTheme`，因此输入框相关的 Token（背景色、边框色、文本色等）由 `InfoPickerInputToken` 提供。`TimePickerToken` 仅定义弹出面板内部的额外 Token。
