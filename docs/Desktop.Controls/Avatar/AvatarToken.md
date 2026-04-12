# Avatar Design Token

Avatar 控件使用 `AvatarToken`（Token ID: `"Avatar"`）作为组件级 Design Token，所有视觉属性均从全局 `SharedToken` 派生，不硬编码任何具体值。`AvatarToken` 同时为 `Avatar` 和 `AvatarGroup` 两个控件提供 Token 资源。

---

## Token 资源访问方式

在 AXAML 中使用组件级 Token：

```xml
{atom:AvatarTokenResource ContainerSize}
{atom:AvatarTokenResource TextFontSize}
{atom:AvatarTokenResource GroupOverlapping}
{atom:AvatarTokenResource AvatarBg}
{atom:AvatarTokenResource AvatarColor}
```

在 AXAML 中使用全局共享 Token：

```xml
{atom:SharedTokenResource BorderRadius}
{atom:SharedTokenResource BorderThickness}
{atom:SharedTokenResource EnableMotion}
```

---

## 组件级 Token 一览

以下是 `AvatarToken` 定义的全部组件级 Token，按功能分组说明。

### 尺寸 Token

| Token 名 | 类型 | 来源 | 说明 |
|---|---|---|---|
| `ContainerSize` | `double` | `SharedToken.ControlHeight` | 中号头像容器尺寸（Width = Height） |
| `ContainerSizeLG` | `double` | `SharedToken.ControlHeightLG` | 大号头像容器尺寸 |
| `ContainerSizeSM` | `double` | `SharedToken.ControlHeightSM` | 小号头像容器尺寸 |

### 字体 Token

| Token 名 | 类型 | 来源 | 说明 |
|---|---|---|---|
| `TextFontSize` | `double` | `Math.Round((SharedToken.FontSizeLG + SharedToken.FontSizeXL) / 2)` | 中号头像文字/图标大小。取大号和超大号字体的平均值并取整 |
| `TextFontSizeLG` | `double` | `SharedToken.FontSizeHeading3` | 大号头像文字/图标大小。使用三级标题字号 |
| `TextFontSizeSM` | `double` | `SharedToken.FontSize` | 小号头像文字/图标大小。使用基础字号 |

### 头像组 Token

| Token 名 | 类型 | 来源 | 说明 |
|---|---|---|---|
| `GroupSpace` | `double` | `SharedToken.UniformlyMarginXXS` | 头像组内弹出面板的子项水平间距 |
| `GroupOverlapping` | `double` | `SharedToken.UniformlyMarginXS` | 头像组中相邻头像的重叠宽度（像素），值越大重叠越多 |
| `GroupBorderColor` | `Color` | `SharedToken.ColorBorderBg` | 头像组中每个头像的边框颜色（与背景融合的浅色边框） |

### 颜色 Token

| Token 名 | 类型 | 来源 | 说明 |
|---|---|---|---|
| `AvatarBg` | `Color` | `SharedToken.ColorTextPlaceholder` | 默认头像背景色（Icon 和 Text 类型使用），灰色占位色 |
| `AvatarColor` | `Color` | `SharedToken.ColorTextLightSolid` | 默认头像前景色（图标和文字的颜色），通常为白色 |

---

## 主题中使用的全局 SharedToken

除组件级 Token 外，Avatar 的 ControlTheme 还直接引用了以下全局 `SharedToken`：

### Avatar ControlTheme 使用的 SharedToken

| Token 资源键 | 使用场景 |
|---|---|
| `EnableMotion` | 全局动画开关，绑定到 `IsMotionEnabled` 属性 |
| `BorderRadiusLG` | 方形 + 大号头像的圆角半径 |
| `BorderRadius` | 方形 + 中号/自定义头像的圆角半径 |
| `BorderRadiusSM` | 方形 + 小号头像的圆角半径 |

### AvatarGroup ControlTheme 使用的 SharedToken

| Token 资源键 | 使用场景 |
|---|---|
| `BorderThickness` | 头像组中每个子 Avatar 的统一边框粗细 |

---

## Token 对外观的具体影响

### 内容类型与 Token 映射

| 内容类型 | Background | Foreground | 图标/文字尺寸 |
|---|---|---|---|
| **Icon** | `AvatarBg`（灰色占位背景） | `AvatarColor`（白色前景） | `EffectiveIconSize`（由 SizeType 或 Custom 计算） |
| **Text** | `AvatarBg`（灰色占位背景） | `AvatarColor`（白色前景） | 由 FontSize 和 TextRenderTransform 控制 |
| **BitmapImage** | `Transparent`（透明） | — | 图片自适应 |
| **SvgImage** | `Transparent`（透明） | — | 图片自适应 |

### 尺寸与 Token 映射

| 尺寸 | 容器大小 (Width/Height) | 图标/文字大小 (EffectiveIconSize) | 方形圆角 |
|---|---|---|---|
| `Large` | `ContainerSizeLG` | `TextFontSizeLG` | `BorderRadiusLG` |
| `Middle` | `ContainerSize` | `TextFontSize` | `BorderRadius` |
| `Small` | `ContainerSizeSM` | `TextFontSizeSM` | `BorderRadiusSM` |
| `Custom` | `Size`（用户指定） | `Size / 2`（有 Icon/Src）或 `18`（文字） | `BorderRadius` |

### AvatarGroup Token 映射

| 属性 | Token | 说明 |
|---|---|---|
| 子项间距（Flyout 内） | `GroupSpace` | Flyout 弹出面板中被折叠头像之间的水平间距 |
| 重叠宽度 | `GroupOverlapping` | 相邻头像的水平重叠像素数 |
| 子 Avatar 边框颜色 | `GroupBorderColor` | 每个子头像的 `BorderBrush` |
| 子 Avatar 边框粗细 | `SharedToken.BorderThickness` | 每个子头像的 `BorderThickness` |
| 折叠计数头像背景色 | `AvatarBg` | `+N` 头像的默认背景色 |
| 折叠计数头像前景色 | `AvatarColor` | `+N` 头像的默认文字颜色 |

---

## Token 计算公式参考

以下展示 `AvatarToken.CalculateTokenValues()` 中各 Token 的完整计算逻辑：

```csharp
// 尺寸 Token — 直接映射全局控件高度
ContainerSize   = SharedToken.ControlHeight;       // 中号
ContainerSizeLG = SharedToken.ControlHeightLG;     // 大号
ContainerSizeSM = SharedToken.ControlHeightSM;     // 小号

// 字体 Token — 基于全局字体尺度
TextFontSize    = Math.Round((SharedToken.FontSizeLG + SharedToken.FontSizeXL) / 2);
TextFontSizeLG  = SharedToken.FontSizeHeading3;
TextFontSizeSM  = SharedToken.FontSize;

// 头像组 Token — 基于全局间距
GroupSpace       = SharedToken.UniformlyMarginXXS;
GroupOverlapping = SharedToken.UniformlyMarginXS;
GroupBorderColor = SharedToken.ColorBorderBg;

// 颜色 Token — 基于全局色彩
AvatarBg    = SharedToken.ColorTextPlaceholder;    // 灰色占位背景
AvatarColor = SharedToken.ColorTextLightSolid;     // 白色前景
```
