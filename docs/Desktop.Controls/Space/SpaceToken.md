# Space Design Token

Space 控件使用 `SpaceToken`（Token ID: `"Space"`）作为组件级 Design Token，所有视觉属性均从全局 `SharedToken` 派生，不硬编码任何具体值。

---

## Token 资源访问方式

在 AXAML 中使用组件级 Token：

```xml
{atom:SpaceTokenResource GapSmallSize}
{atom:SpaceTokenResource GapMiddleSize}
{atom:SpaceTokenResource GapLargeSize}
{atom:SpaceTokenResource AddonBg}
{atom:SpaceTokenResource AddOnPadding}
```

在 AXAML 中使用全局共享 Token：

```xml
{atom:SharedTokenResource SpacingXS}
{atom:SharedTokenResource Spacing}
{atom:SharedTokenResource SpacingLG}
{atom:SharedTokenResource ColorFillAlter}
```

---

## 组件级 Token 一览

以下是 `SpaceToken` 定义的全部组件级 Token，按功能分组说明。

### 间距 Token

| Token 名 | 类型 | 来源 | 说明 |
|---|---|---|---|
| `GapSmallSize` | `double` | `SharedToken.SpacingXS` | 小间距尺寸（对应 `SizeType.Small`，默认 8） |
| `GapMiddleSize` | `double` | `SharedToken.Spacing` | 中间距尺寸（对应 `SizeType.Middle`，默认 16） |
| `GapLargeSize` | `double` | `SharedToken.SpacingLG` | 大间距尺寸（对应 `SizeType.Large`，默认 24） |

### CompactSpaceAddOn Token

| Token 名 | 类型 | 来源 | 说明 |
|---|---|---|---|
| `AddonBg` | `Color` | `SharedToken.ColorFillAlter` | AddOn 区域背景色（Outline / Filled 样式变体使用） |
| `AddOnPadding` | `Thickness` | `SharedToken.UniformlyPaddingSM` | AddOn 默认内边距（对应 `SizeType.Middle`） |
| `AddOnPaddingSM` | `Thickness` | `SharedToken.ControlPaddingHorizontalSM` | AddOn 小号内边距（对应 `SizeType.Small`） |
| `AddOnPaddingLG` | `Thickness` | `SharedToken.ControlPaddingHorizontal` | AddOn 大号内边距（对应 `SizeType.Large`） |

---

## 主题中使用的全局 SharedToken

除组件级 Token 外，Space 相关组件的 ControlTheme 还直接引用了以下全局 `SharedToken`：

### CompactSpaceAddOn 使用的 SharedToken

| Token 资源键 | 使用场景 |
|---|---|
| `BorderThickness` | AddOn 默认边框厚度 |
| `FontSize` / `FontSizeSM` / `FontSizeLG` | AddOn 字体大小（中/小/大） |
| `BorderRadius` / `BorderRadiusSM` / `BorderRadiusLG` | AddOn 圆角半径（中/小/大） |
| `ControlHeight` / `ControlHeightSM` / `ControlHeightLG` | AddOn 最小高度（中/小/大） |
| `ColorBorder` | 边框颜色（Outline 样式变体 + 禁用态） |
| `ColorError` / `ColorErrorBg` / `ColorErrorText` | 错误状态颜色系列 |
| `ColorWarning` / `ColorWarningBg` / `ColorWarningText` | 警告状态颜色系列 |
| `ColorTextDisabled` | 禁用态文字颜色 |
| `ColorBgContainerDisabled` | 禁用态背景颜色 |

### Space 间距值对应的 SharedToken

| Token 资源键 | 使用场景 |
|---|---|
| `SpacingXS` | Small 间距值来源（默认 8） |
| `Spacing` | Middle 间距值来源（默认 16） |
| `SpacingLG` | Large 间距值来源（默认 24） |

---

## Token 对外观的具体影响

### Space 间距与 Token 映射

Space 的间距值完全由 Token 驱动，通过实例级样式根据 `SizeType` 属性自动切换：

| SizeType | `ItemSpacing` | `LineSpacing` | Token 来源 |
|---|---|---|---|
| `Small`（默认） | `GapSmallSize` | `GapSmallSize` | `SharedToken.SpacingXS`（默认 8） |
| `Middle` | `GapMiddleSize` | `GapMiddleSize` | `SharedToken.Spacing`（默认 16） |
| `Large` | `GapLargeSize` | `GapLargeSize` | `SharedToken.SpacingLG`（默认 24） |
| `Custom` | 用户设置 | 用户设置 | 不使用 Token |

### CompactSpaceAddOn 尺寸与 Token 映射

| SizeType | 字号 | 圆角 | 内间距 | 最小高度 |
|---|---|---|---|---|
| `Large` | `FontSizeLG` | `BorderRadiusLG` | `AddOnPaddingLG` | `ControlHeightLG` |
| `Middle` | `FontSize` | `BorderRadius` | `AddOnPadding` | `ControlHeight` |
| `Small` | `FontSizeSM` | `BorderRadiusSM` | `AddOnPaddingSM` | `ControlHeightSM` |

### CompactSpaceAddOn 样式变体与 Token 映射

| 样式变体 | 正常态 | Error 状态 | Warning 状态 | 禁用态 |
|---|---|---|---|---|
| **Outline** | `AddonBg` 背景 + `ColorBorder` 边框 | `ColorError` 边框 + `ColorErrorText` 文字 | `ColorWarning` 边框 + `ColorWarningText` 文字 | `ColorBgContainerDisabled` 背景 + `ColorBorder` 边框 + `ColorTextDisabled` 文字 |
| **Filled** | `AddonBg` 背景 | `ColorErrorBg` 背景 + `ColorErrorText` 文字 | `ColorWarningBg` 背景 + `ColorWarningText` 文字 | `ColorBgContainerDisabled` 背景 + `ColorTextDisabled` 文字 |
| **Underlined** | 透明背景 | `ColorErrorText` 文字 | `ColorWarningText` 文字 | `ColorTextDisabled` 文字 |

### 紧凑空间圆角裁剪规则

CompactSpace 中子项的圆角根据位置自动裁剪，以消除组合控件之间的圆角间隙：

| 位置 | 水平方向 | 垂直方向 |
|---|---|---|
| **First**（首项） | 保留左侧圆角，右侧为 0 | 保留顶部圆角，底部为 0 |
| **Middle**（中间项） | 四角均为 0 | 四角均为 0 |
| **Last**（末项） | 左侧为 0，保留右侧圆角 | 顶部为 0，保留底部圆角 |
| **First + Last**（唯一项） | 保留全部圆角 | 保留全部圆角 |
