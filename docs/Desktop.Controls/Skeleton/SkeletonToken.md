# Skeleton Design Token

Skeleton 控件族使用 `SkeletonToken`（Token ID: `"Skeleton"`）作为组件级 Design Token，所有视觉属性均从全局 `SharedToken` 派生，不硬编码任何具体值。所有 Skeleton 系列控件共享同一套 Token。

---

## Token 资源访问方式

在 AXAML 中使用组件级 Token：

```xml
{atom:SkeletonTokenResource GradientFromColor}
{atom:SkeletonTokenResource TitleHeight}
{atom:SkeletonTokenResource BlockRadius}
{atom:SkeletonTokenResource ParagraphLineHeight}
{atom:SkeletonTokenResource ImageContainerSize}
```

在 AXAML 中使用全局共享 Token：

```xml
{atom:SharedTokenResource ControlHeight}
{atom:SharedTokenResource ControlHeightLG}
{atom:SharedTokenResource ControlHeightSM}
{atom:SharedTokenResource BorderRadius}
{atom:SharedTokenResource BorderRadiusSM}
{atom:SharedTokenResource BorderRadiusLG}
```

---

## 组件级 Token 一览

以下是 `SkeletonToken` 定义的全部组件级 Token，按功能分组说明。

### 颜色 Token

| Token 名 | 类型 | 来源 | 说明 |
|---|---|---|---|
| `GradientFromColor` | `Color` | `SharedToken.ColorFillContent` | 骨架块的基底颜色（静态状态和渐变起点色） |
| `GradientToColor` | `Color` | `SharedToken.ColorFill` | 渐变终点颜色（用于构建流光动画的渐变画刷） |

### 流光动画 Token

| Token 名 | 类型 | 来源 | 说明 |
|---|---|---|---|
| `LoadingBackgroundStart` | `IBrush` | 基于 `GradientFromColor` + `GradientToColor` 构建 | 流光动画起始帧画刷（`LinearGradientBrush`，起始位置为左） |
| `LoadingBackgroundMiddle` | `IBrush` | 基于 `GradientFromColor` + `GradientToColor` 构建 | 流光动画中间帧画刷（渐变已扫过一半） |
| `LoadingBackgroundEnd` | `IBrush` | 基于 `GradientFromColor` 构建 | 流光动画结束帧画刷（回到纯色状态） |
| `LoadingMotionDuration` | `TimeSpan` | 固定 `1.4s` | 流光动画一次循环的时长 |

### 尺寸 Token

| Token 名 | 类型 | 来源 | 说明 |
|---|---|---|---|
| `TitleHeight` | `double` | `SharedToken.ControlHeight / 2` | 标题骨架行高度 |
| `ParagraphLineHeight` | `double` | `SharedToken.ControlHeight / 2` | 段落骨架单行高度 |
| `BlockRadius` | `CornerRadius` | `SharedToken.BorderRadiusSM` | 骨架块默认圆角（`IsRound=False` 时使用） |
| `ParagraphLineRoundCornerRadius` | `CornerRadius` | `ParagraphLineHeight / 2` | 段落行胶囊形圆角（`IsRound=True` 时使用） |

### 间距 Token

| Token 名 | 类型 | 来源 | 说明 |
|---|---|---|---|
| `ParagraphMarginTop` | `Thickness` | `new Thickness(0, SharedToken.UniformlyMarginLG + SharedToken.UniformlyMarginXXS, 0, 0)` | 段落区域顶部间距（标题与段落之间的间隔） |
| `AvatarMarginRight` | `Thickness` | `new Thickness(0, 0, SharedToken.UniformlyMargin, 0)` | 头像区域右侧间距（头像与内容区域之间的间隔） |

### 图片骨架 Token

| Token 名 | 类型 | 来源 | 说明 |
|---|---|---|---|
| `ImageSize` | `double` | `SharedToken.ControlHeight × 1.5` | 图片骨架内嵌图标的尺寸 |
| `ImageContainerSize` | `double` | `SharedToken.ControlHeight × 1.5 × 2` | 图片骨架容器的默认宽高 |
| `ImageContainerMaxSize` | `double` | `SharedToken.ControlHeight × 1.5 × 4` | 图片骨架容器的最大宽高 |

---

## 主题中使用的全局 SharedToken

除组件级 Token 外，Skeleton 的各 ControlTheme 还直接引用了以下全局 `SharedToken`：

| Token 资源键 | 使用场景 | 说明 |
|---|---|---|
| `ControlHeight` | 中号高度 | `SkeletonElement`（Middle）、`SkeletonAvatar`（Middle）的高度 |
| `ControlHeightLG` | 大号高度 | `SkeletonElement`（Large）、`SkeletonAvatar`（Large）的高度 |
| `ControlHeightSM` | 小号高度 | `SkeletonElement`（Small）、`SkeletonAvatar`（Small）的高度 |
| `ControlHeightXS` | 段落行间距 | `SkeletonParagraph` 内部 StackPanel 的 `Spacing` |
| `BorderRadius` | 中号圆角 | `SkeletonAvatar`（Square + Middle）的圆角 |
| `BorderRadiusSM` | 小号圆角 | `SkeletonElement` 默认圆角、`SkeletonButton`（Square）圆角、`SkeletonImage` 圆角 |
| `BorderRadiusLG` | 大号圆角 | `SkeletonAvatar`（Square + Large）的圆角 |

---

## Token 对外观的具体影响

### 颜色与动画

| 状态 | 视觉效果 | 相关 Token |
|---|---|---|
| 静态（`IsActive=False`） | 纯色灰色块 | `GradientFromColor` → `Background` |
| 流光动画（`IsActive=True`） | 从左到右的渐变流光，三帧循环 | `LoadingBackgroundStart` → `LoadingBackgroundMiddle` → `LoadingBackgroundEnd`，时长 `LoadingMotionDuration` |

流光动画通过 Avalonia `Animation` + `KeyFrame` 实现，动画驱动 `AnimationLayerFill` 属性在三个 `LinearGradientBrush` 之间过渡：

```
帧 0（0%）: LoadingBackgroundStart  ── 渐变在最左侧
帧 1（80%）: LoadingBackgroundMiddle ── 渐变扫过右侧
帧 2（100%）: LoadingBackgroundEnd   ── 回到纯色
```

### SizeType 与高度 Token 映射

| SizeType | SkeletonElement/SkeletonButton/SkeletonInput 高度 | SkeletonAvatar 宽高 |
|---|---|---|
| `Large` | `ControlHeightLG` | `ControlHeightLG` |
| `Middle` | `ControlHeight` | `ControlHeight` |
| `Small` | `ControlHeightSM` | `ControlHeightSM` |

### IsRound 与圆角 Token 映射

| IsRound | 段落行圆角 |
|---|---|
| `false`（默认） | `BlockRadius`（`BorderRadiusSM`） |
| `true` | `ParagraphLineRoundCornerRadius`（`ParagraphLineHeight / 2`，形成胶囊形） |

### 组合骨架屏内间距

| 区域 | 相关 Token | 效果 |
|---|---|---|
| 头像 → 内容 | `AvatarMarginRight` | 头像右侧留白 |
| 标题 → 段落 | `ParagraphMarginTop` | 段落区域顶部留白（仅 `IsShowTitle=True` 时生效） |
| 段落行间 | `ControlHeightXS`（共享 Token） | 段落内行与行之间的间距 |

---

## Token 值参考（默认主题）

以下是默认（Light）主题下各 Token 的典型计算值，实际值取决于全局 `SharedToken` 配置：

| Token | 默认值（约） | 计算逻辑 |
|---|---|---|
| `GradientFromColor` | `rgba(0,0,0,0.06)` | `SharedToken.ColorFillContent` |
| `GradientToColor` | `rgba(0,0,0,0.15)` | `SharedToken.ColorFill` |
| `TitleHeight` | `16` px | `ControlHeight / 2` |
| `ParagraphLineHeight` | `16` px | `ControlHeight / 2` |
| `BlockRadius` | `4` px | `SharedToken.BorderRadiusSM` |
| `ParagraphLineRoundCornerRadius` | `8` px | `ParagraphLineHeight / 2` |
| `ParagraphMarginTop` | `0,28,0,0` | `UniformlyMarginLG + UniformlyMarginXXS` |
| `AvatarMarginRight` | `0,0,16,0` | `UniformlyMargin` |
| `LoadingMotionDuration` | `1.4s` | 固定值 |
| `ImageSize` | `48` px | `ControlHeight × 1.5` |
| `ImageContainerSize` | `96` px | `ImageSize × 2` |
| `ImageContainerMaxSize` | `192` px | `ImageSize × 4` |
| `ControlHeight`（共享） | `32` px | 全局控件高度 |
| `ControlHeightLG`（共享） | `40` px | 全局大号控件高度 |
| `ControlHeightSM`（共享） | `24` px | 全局小号控件高度 |
| `ControlHeightXS`（共享） | `16` px | 全局超小号控件高度 |
