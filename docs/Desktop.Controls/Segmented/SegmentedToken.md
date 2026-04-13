# Segmented Design Token

Segmented 控件使用 `SegmentedToken`（Token ID: `"Segmented"`）作为组件级 Design Token，所有视觉属性均从全局 `SharedToken` 派生，不硬编码任何具体值。

---

## Token 资源访问方式

在 AXAML 中使用组件级 Token：

```xml
{atom:SegmentedTokenResource ItemColor}
{atom:SegmentedTokenResource TrackBg}
{atom:SegmentedTokenResource ItemSelectedBg}
```

在 AXAML 中使用全局共享 Token：

```xml
{atom:SharedTokenResource BorderRadius}
{atom:SharedTokenResource FontSize}
{atom:SharedTokenResource ControlHeight}
```

---

## 组件级 Token 一览

以下是 `SegmentedToken` 定义的全部组件级 Token，按功能分组说明。

### 颜色 Token

| Token 名 | 类型 | 来源 | 说明 |
|---|---|---|---|
| `ItemColor` | `Color` | `SharedToken.ColorTextLabel` | 选项文本颜色（未选中、非悬浮态） |
| `ItemHoverColor` | `Color` | `SharedToken.ColorText` | 选项悬浮态文本颜色 |
| `ItemHoverBg` | `Color` | `SharedToken.ColorFillSecondary` | 选项悬浮态背景颜色 |
| `ItemActiveBg` | `Color` | `SharedToken.ColorFill` | 选项激活态（按下）背景颜色 |
| `ItemSelectedBg` | `Color` | `SharedToken.ColorBgElevated` | 选中选项的背景颜色（色块 Thumb 的背景色） |
| `ItemSelectedColor` | `Color` | `SharedToken.ColorText` | 选中选项的文本颜色 |
| `TrackBg` | `Color` | `SharedToken.ColorBgLayout` | 容器（轨道）背景色 |

### 尺寸 Token

| Token 名 | 类型 | 来源 | 说明 |
|---|---|---|---|
| `ItemMinHeightLG` | `double` | `SharedToken.ControlHeightLG - TrackPadding * 2` | 大号选项最小高度 |
| `ItemMinHeight` | `double` | `SharedToken.ControlHeight - TrackPadding * 2` | 中号选项最小高度 |
| `ItemMinHeightSM` | `double` | `SharedToken.ControlHeightSM - TrackPadding * 2` | 小号选项最小高度 |

### 间距 Token

| Token 名 | 类型 | 来源 | 说明 |
|---|---|---|---|
| `TrackPadding` | `Thickness` | `SharedToken.LineWidthBold`（均匀四边） | 容器内间距（色块与容器边缘的间隙） |
| `SegmentedItemPadding` | `Thickness` | `SharedToken.ControlPaddingHorizontal - LineWidth`（水平方向） | 中号/大号选项的水平内间距 |
| `SegmentedItemPaddingSM` | `Thickness` | `SharedToken.ControlPaddingHorizontalSM - LineWidth`（水平方向） | 小号选项的水平内间距 |
| `SegmentedItemContentMargin` | `Thickness` | `SharedToken.UniformlyPaddingXXS`（仅左侧） | 带图标时内容文字的左边距（图标与文字之间的间距） |

---

## 主题中使用的全局 SharedToken

除组件级 Token 外，Segmented 的 ControlTheme 还直接引用了以下全局 `SharedToken`：

| Token 资源键 | 使用场景 |
|---|---|
| `EnableMotion` | 全局动画开关（控制 `IsMotionEnabled` 的默认值） |
| `BoxShadowsTertiary` | 选中色块的阴影效果 |
| `BorderRadiusLG` / `BorderRadius` / `BorderRadiusSM` / `BorderRadiusXS` | 不同尺寸下容器和选项的圆角 |
| `FontSizeLG` / `FontSize` | 大号和中/小号选项的字体大小 |
| `IconSizeLG` / `IconSize` / `IconSizeSM` | 不同尺寸下图标的宽高 |
| `ColorTextDisabled` | 禁用选项的文本颜色 |
| `ControlHeightLG` / `ControlHeight` / `ControlHeightSM` | 用于计算选项最小高度 |
| `ControlPaddingHorizontal` / `ControlPaddingHorizontalSM` | 用于计算选项水平内间距 |
| `LineWidth` / `LineWidthBold` | 用于计算容器内间距和选项内间距偏移 |

---

## Token 对外观的具体影响

### 容器外观

| 属性 | Token 来源 |
|---|---|
| 背景色 | `TrackBg`（`ColorBgLayout`） |
| 内间距 | `TrackPadding`（`LineWidthBold`） |
| 圆角（Large） | `BorderRadiusLG`（全局 SharedToken） |
| 圆角（Middle） | `BorderRadius`（全局 SharedToken） |
| 圆角（Small） | `BorderRadiusSM`（全局 SharedToken） |

### 选中色块（Thumb）外观

| 属性 | Token 来源 |
|---|---|
| 背景色 | `ItemSelectedBg`（`ColorBgElevated`） |
| 阴影 | `BoxShadowsTertiary`（全局 SharedToken） |
| 圆角（Large） | `BorderRadius`（全局 SharedToken） |
| 圆角（Middle） | `BorderRadiusSM`（全局 SharedToken） |
| 圆角（Small） | `BorderRadiusXS`（全局 SharedToken） |

### 选项状态与 Token 映射

| 状态 | 文字颜色 Token | 背景色 Token | 图标颜色 Token |
|---|---|---|---|
| **正常（未选中）** | `ItemColor` | 透明 | `ItemColor` |
| **悬浮（未选中）** | `ItemHoverColor` | `ItemHoverBg` | `ItemHoverColor` |
| **按下（未选中）** | — | `ItemActiveBg` | `ItemSelectedColor` |
| **选中** | `ItemSelectedColor` | `ItemSelectedBg` | `ItemSelectedColor` |
| **禁用** | `ColorTextDisabled` | — | — |

### 尺寸与 Token 映射

| 尺寸 | 选项最小高度 | 选项内间距 | 字体大小 | 图标大小 | 选项圆角 |
|---|---|---|---|---|---|
| `Large` | `ItemMinHeightLG` | `SegmentedItemPadding` | `FontSizeLG` | `IconSizeLG` | `BorderRadius` |
| `Middle` | `ItemMinHeight` | `SegmentedItemPadding` | `FontSize` | `IconSize` | `BorderRadiusSM` |
| `Small` | `ItemMinHeightSM` | `SegmentedItemPaddingSM` | `FontSize` | `IconSizeSM` | `BorderRadiusXS` |
