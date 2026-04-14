# ColorPicker Design Token

ColorPicker 控件使用 `ColorPickerToken`（Token ID: `"ColorPicker"`）作为组件级 Design Token，所有视觉属性均从全局 `SharedToken` 派生，不硬编码任何具体值。

---

## Token 资源访问方式

在 AXAML 中使用组件级 Token：

```xml
{atom:ColorPickerTokenResource ColorBlockBorderRadius}
{atom:ColorPickerTokenResource ColorBlockInnerShadow}
{atom:ColorPickerTokenResource ColorBlockSize}
```

在 AXAML 中使用全局共享 Token：

```xml
{atom:SharedTokenResource ColorPrimary}
{atom:SharedTokenResource ControlHeight}
{atom:SharedTokenResource BorderRadius}
```

---

## 组件级 Token 一览

以下是 `ColorPickerToken` 定义的全部组件级 Token，按功能分组说明。

### 触发器尺寸 Token

| Token 名 | 类型 | 来源 | 说明 |
|---|---|---|---|
| `ColorBlockBorderRadius` | `CornerRadius` | 基于 `SharedToken.BorderRadiusSM` | 触发器色块圆角半径 |
| `ColorBlockInnerShadow` | `BoxShadows` | 基于 `SharedToken.ColorBgContainer` | 触发器色块内阴影 |
| `ColorBlockSize` | `double` | 基于 `SharedToken.ControlHeight` | 中号触发器色块尺寸 |
| `ColorBlockSizeLG` | `double` | 基于 `SharedToken.ControlHeightLG` | 大号触发器色块尺寸 |
| `ColorBlockSizeSM` | `double` | 基于 `SharedToken.ControlHeightSM` | 小号触发器色块尺寸 |

### 颜色面板尺寸 Token

| Token 名 | 类型 | 来源 | 说明 |
|---|---|---|---|
| `ColorPickerPanelWidth` | `double` | 固定值 | 颜色面板宽度 |
| `ColorPickerPanelHeight` | `double` | 固定值 | 颜色面板高度 |
| `ColorPickerPanelBorderRadius` | `CornerRadius` | 基于 `SharedToken.BorderRadiusLG` | 颜色面板圆角半径 |
| `ColorPickerPanelPadding` | `Thickness` | 基于 `SharedToken.PaddingSM` | 颜色面板内间距 |

### 色谱区域 Token

| Token 名 | 类型 | 来源 | 说明 |
|---|---|---|---|
| `SpectrumHeight` | `double` | 固定值 | 色谱区域高度 |
| `SpectrumBorderRadius` | `CornerRadius` | 基于 `SharedToken.BorderRadiusSM` | 色谱区域圆角半径 |
| `SpectrumBorderThickness` | `Thickness` | 固定值 | 色谱区域边框粗细 |
| `SpectrumBorderColor` | `Color` | 基于 `SharedToken.ColorBorder` | 色谱区域边框颜色 |
| `SpectrumThumbSize` | `double` | 固定值 | 色谱指示器（Thumb）尺寸 |
| `SpectrumThumbBorderThickness` | `double` | 固定值 | 色谱指示器边框粗细 |
| `SpectrumThumbBorderColor` | `Color` | 基于 `SharedToken.ColorText` | 色谱指示器边框颜色 |

### 滑块 Token

| Token 名 | 类型 | 来源 | 说明 |
|---|---|---|---|
| `SliderHeight` | `double` | 固定值 | 滑块高度 |
| `SliderBorderRadius` | `CornerRadius` | 基于 `SharedToken.BorderRadiusSM` | 滑块圆角半径 |
| `SliderBorderThickness` | `Thickness` | 固定值 | 滑块边框粗细 |
| `SliderBorderColor` | `Color` | 基于 `SharedToken.ColorBorder` | 滑块边框颜色 |
| `SliderThumbSize` | `double` | 固定值 | 滑块指示器（Thumb）尺寸 |
| `SliderThumbBorderThickness` | `double` | 固定值 | 滑块指示器边框粗细 |
| `SliderThumbBorderColor` | `Color` | 基于 `SharedToken.ColorText` | 滑块指示器边框颜色 |

### 输入区域 Token

| Token 名 | 类型 | 来源 | 说明 |
|---|---|---|---|
| `InputBorderRadius` | `CornerRadius` | 基于 `SharedToken.BorderRadiusSM` | 输入框圆角半径 |
| `InputBorderThickness` | `Thickness` | 固定值 | 输入框边框粗细 |
| `InputBorderColor` | `Color` | 基于 `SharedToken.ColorBorder` | 输入框边框颜色 |
| `InputHeight` | `double` | 基于 `SharedToken.ControlHeightSM` | 输入框高度 |
| `InputPadding` | `Thickness` | 基于 `SharedToken.PaddingXXS` | 输入框内间距 |

### 预设色板 Token

| Token 名 | 类型 | 来源 | 说明 |
|---|---|---|---|
| `PaletteBorderRadius` | `CornerRadius` | 基于 `SharedToken.BorderRadiusSM` | 预设色块圆角半径 |
| `PaletteSize` | `double` | 固定值 | 预设色块尺寸 |
| `PaletteGap` | `double` | 基于 `SharedToken.UniformlyPaddingXXS` | 预设色块间距 |
| `PaletteGroupMargin` | `Thickness` | 固定值 | 预设色板组外边距 |

### 渐变色编辑器 Token

| Token 名 | 类型 | 来源 | 说明 |
|---|---|---|---|
| `GradientSliderHeight` | `double` | 固定值 | 渐变色滑块高度 |
| `GradientSliderBorderRadius` | `CornerRadius` | 基于 `SharedToken.BorderRadiusSM` | 渐变色滑块圆角半径 |
| `GradientSliderBorderThickness` | `Thickness` | 固定值 | 渐变色滑块边框粗细 |
| `GradientSliderBorderColor` | `Color` | 基于 `SharedToken.ColorBorder` | 渐变色滑块边框颜色 |
| `GradientStopThumbSize` | `double` | 固定值 | 渐变色标指示器尺寸 |
| `GradientStopThumbBorderThickness` | `double` | 固定值 | 渐变色标指示器边框粗细 |
| `GradientStopThumbBorderColor` | `Color` | 基于 `SharedToken.ColorText` | 渐变色标指示器边框颜色 |

---

## Token 对控件外观的影响

### 触发器外观

触发器的外观由 `ColorBlockSize` / `ColorBlockBorderRadius` / `ColorBlockInnerShadow` 三个 Token 控制：

- **`ColorBlockSize`**：决定色块的宽高，根据 `SizeType` 自动选择 `ColorBlockSize` / `ColorBlockSizeSM` / `ColorBlockSizeLG`
- **`ColorBlockBorderRadius`**：色块的圆角，使色块呈现圆角矩形
- **`ColorBlockInnerShadow`**：色块的内阴影，增强色块的立体感和层次感

### 颜色面板外观

颜色面板的整体尺寸和间距由 `ColorPickerPanelWidth` / `ColorPickerPanelHeight` / `ColorPickerPanelPadding` 控制：

- **`ColorPickerPanelWidth`** / **`ColorPickerPanelHeight`**：面板的固定宽高，确保面板在不同主题下保持一致的布局
- **`ColorPickerPanelBorderRadius`**：面板的圆角，通常与 Flyout 的圆角保持一致
- **`ColorPickerPanelPadding`**：面板内容与边框的间距

### 色谱区域外观

色谱区域是面板中最大的交互区域，由 `SpectrumHeight` / `SpectrumBorderRadius` / `SpectrumBorderColor` 等控制：

- **`SpectrumHeight`**：色谱区域的高度，影响用户拖动选取颜色的精度
- **`SpectrumBorderRadius`**：色谱区域的圆角
- **`SpectrumBorderColor`** / **`SpectrumBorderThickness`**：色谱区域的边框，提供视觉边界
- **`SpectrumThumbSize`** / **`SpectrumThumbBorderColor`**：色谱指示器的尺寸和边框，指示当前选中位置

### 滑块外观

色相滑块和透明度滑块共享相同的 Token：

- **`SliderHeight`**：滑块的高度
- **`SliderBorderRadius`**：滑块的圆角
- **`SliderThumbSize`** / **`SliderThumbBorderColor`**：滑块指示器的尺寸和边框

### 预设色板外观

- **`PaletteSize`**：每个预设色块的尺寸
- **`PaletteGap`**：色块之间的间距
- **`PaletteBorderRadius`**：色块的圆角

---

## 全局共享 Token 依赖

ColorPicker 依赖以下全局 `SharedToken`，修改这些共享 Token 会间接影响 ColorPicker 的外观：

| 共享 Token | 影响范围 |
|---|---|
| `ControlHeight` / `ControlHeightSM` / `ControlHeightLG` | 触发器色块尺寸、输入框高度 |
| `BorderRadius` / `BorderRadiusSM` / `BorderRadiusLG` | 面板、色谱、滑块、输入框、色块的圆角 |
| `ColorBorder` | 色谱边框、滑块边框、输入框边框 |
| `ColorText` | 色谱指示器边框、滑块指示器边框 |
| `ColorBgContainer` | 色块内阴影颜色 |
| `PaddingSM` / `PaddingXXS` / `UniformlyPaddingXXS` | 面板内间距、输入框内间距、色块间距 |