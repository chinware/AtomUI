# Rate Design Token

Rate 控件使用 `RateToken`（Token ID: `"Rate"`）作为组件级 Design Token，所有视觉属性均从全局 `SharedToken` 派生，不硬编码任何具体值。

---

## Token 资源访问方式

在 AXAML 中使用组件级 Token：

```xml
{atom:RateTokenResource StarColor}
{atom:RateTokenResource StarSize}
{atom:RateTokenResource StarSizeSM}
{atom:RateTokenResource StarSizeLG}
{atom:RateTokenResource StarHoverScale}
{atom:RateTokenResource StarBg}
```

在 AXAML 中使用全局共享 Token：

```xml
{atom:SharedTokenResource EnableMotion}
{atom:SharedTokenResource SpacingXS}
{atom:SharedTokenResource ControlHeight}
```

---

## 组件级 Token 一览

以下是 `RateToken` 定义的全部组件级 Token，按功能分组说明。

### 颜色 Token

| Token 名 | 类型 | 来源 | 说明 |
|---|---|---|---|
| `StarColor` | `Color` | `SharedToken.ColorPalettes[PresetPrimaryColor.Yellow].Color6` | 星星选中颜色（黄色色板 6 号色，即 Ant Design 标准的评分黄色） |
| `StarBg` | `Color` | `SharedToken.ColorFillContent` | 星星未选中背景色（内容填充色，通常为浅灰色） |

### 尺寸 Token

| Token 名 | 类型 | 来源 | 说明 |
|---|---|---|---|
| `StarSize` | `double` | `SharedToken.ControlHeight × 0.625` | 中号星星尺寸（宽 = 高 = StarSize） |
| `StarSizeSM` | `double` | `SharedToken.ControlHeightSM × 0.625` | 小号星星尺寸 |
| `StarSizeLG` | `double` | `SharedToken.ControlHeightLG × 0.625` | 大号星星尺寸 |

### 动画 Token

| Token 名 | 类型 | 来源 | 说明 |
|---|---|---|---|
| `StarHoverScale` | `double` | 固定 `1.2` | 悬浮时星星缩放比例（120%） |

---

## 主题中使用的全局 SharedToken

除组件级 Token 外，Rate 的 ControlTheme 还直接引用了以下全局 `SharedToken`：

### 动画相关

| Token 资源键 | 使用场景 |
|---|---|
| `EnableMotion` | 全局动画开关，控制悬浮缩放过渡是否启用 |

### 间距相关

| Token 资源键 | 使用场景 |
|---|---|
| `SpacingXS` | 星星之间的间距（RateItemsControl 内 StackPanel 的 `Spacing`） |

### 尺寸相关（Token 派生来源）

| Token 资源键 | 使用场景 |
|---|---|
| `ControlHeight` | 中号星星尺寸的计算基准（`StarSize = ControlHeight × 0.625`） |
| `ControlHeightSM` | 小号星星尺寸的计算基准 |
| `ControlHeightLG` | 大号星星尺寸的计算基准 |

### 颜色相关（Token 派生来源）

| Token 资源键 | 使用场景 |
|---|---|
| `ColorPalettes[Yellow].Color6` | `StarColor` 的来源，Ant Design 标准评分黄色 |
| `ColorFillContent` | `StarBg` 的来源，未选中态填充色 |

---

## Token 对外观的具体影响

### 尺寸与 Token 映射

| SizeType | 星星尺寸 Token | 计算公式 |
|---|---|---|
| `Large` | `StarSizeLG` | `ControlHeightLG × 0.625` |
| `Middle` | `StarSize` | `ControlHeight × 0.625` |
| `Small` | `StarSizeSM` | `ControlHeightSM × 0.625` |

### 选中状态与颜色映射

| 状态 | 前景层颜色 | 背景层颜色 |
|---|---|---|
| **未选中** | — | `StarBg`（浅灰色） |
| **整星选中** | `StarColor`（黄色） | `StarBg`（被前景完全覆盖） |
| **半星选中** | `StarColor`（左半），`StarBg`（右半） | `StarBg` |
| **禁用** | 同上，但无交互响应 | 同上 |

### 悬浮缩放效果

| 属性 | 值 | 说明 |
|---|---|---|
| `StarHoverScale` | `1.2` | 鼠标悬浮时以中心点缩放至 120% |
| `RenderTransformOrigin` | `Center` | 缩放原点为星星中心 |
| 过渡动画 | `TransformOperationsTransition` | 由 `IsMotionEnabled` 控制是否启用 |

### 键盘焦点视觉

键盘操作时，当前焦点星显示虚线边框：
- 边框颜色：`StarColor`（与选中色一致）
- 边框粗细：`0.75`
- 虚线样式：`StrokeDashArray = 4, 2`
- 焦点星同时放大至 `StarHoverScale`

### 暗色模式适配

Token 值通过 `CalculateTokenValues(bool isDarkMode)` 在暗色模式下自动适配：
- `SharedToken.ColorPalettes[Yellow].Color6` 在暗色模式下为适配暗色背景的黄色调
- `SharedToken.ColorFillContent` 在暗色模式下为深色背景的内容填充色
- 无需额外配置，切换主题时自动响应
