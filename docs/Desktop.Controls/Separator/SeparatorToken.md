# Separator Design Token

Separator 控件使用 `SeparatorToken`（Token ID: `"Separator"`）作为组件级 Design Token，所有视觉属性均从全局 `SharedToken` 派生，不硬编码任何具体值。

---

## Token 资源访问方式

在 AXAML 中使用组件级 Token：

```xml
{atom:SeparatorTokenResource TextPaddingInline}
{atom:SeparatorTokenResource OrientationMarginPercent}
{atom:SeparatorTokenResource HorizontalMarginBlock}
{atom:SeparatorTokenResource VerticalMarginInline}
```

在 AXAML 中使用全局共享 Token：

```xml
{atom:SharedTokenResource ColorSplit}
{atom:SharedTokenResource ColorTextHeading}
{atom:SharedTokenResource LineWidth}
{atom:SharedTokenResource FontSize}
{atom:SharedTokenResource FontSizeLG}
```

---

## 组件级 Token 一览

以下是 `SeparatorToken` 定义的全部组件级 Token，按功能分组说明。

### 标题间距 Token

| Token 名 | 类型 | 来源 | 说明 |
|---|---|---|---|
| `TextPaddingInline` | `double` | 固定 `1.0` | 标题文本两侧的横向内间距（单位 em）。实际像素值 = `FontSize × TextPaddingInline`，确保间距与字号成比例 |
| `OrientationMarginPercent` | `double` | 固定 `0.05` | 标题与边缘距离的比例（0～1），当 `OrientationMargin` 未设置（`NaN`）时生效。默认 0.05 = 5%，即标题距边缘为分割线总宽度的 5% |

### 水平分割线间距 Token

| Token 名 | 类型 | 来源 | 说明 |
|---|---|---|---|
| `HorizontalMarginBlockSM` | `Thickness` | `new Thickness(0, SharedToken.UniformlyMarginXS)` | 小号水平分割线的垂直外间距，对应 `SizeType=Small` |
| `HorizontalMarginBlock` | `Thickness` | `new Thickness(0, SharedToken.UniformlyMargin)` | 标准水平分割线的垂直外间距，对应 `SizeType=Middle`（默认） |
| `HorizontalMarginBlockLG` | `Thickness` | `new Thickness(0, SharedToken.UniformlyMarginLG)` | 大号水平分割线的垂直外间距，对应 `SizeType=Large` |
| `HorizontalWithTextGutterMargin` | `Thickness` | `new Thickness(0, SharedToken.UniformlyMargin)` | 带标题文本的水平分割线外边距。当有标题且 `SizeType=Small` 时使用此值代替 `HorizontalMarginBlockSM`，确保带标题时间距不会过于紧凑 |

### 垂直分割线 Token

| Token 名 | 类型 | 来源 | 说明 |
|---|---|---|---|
| `VerticalMarginInline` | `double` | `SharedToken.UniformlyMarginXS` | 垂直分割线的横向外间距，控制垂直线条左右两侧的留白 |

---

## 主题中使用的全局 SharedToken

除组件级 Token 外，Separator 的 ControlTheme 还直接引用了以下全局 `SharedToken`：

| Token 资源键 | 使用场景 | 说明 |
|---|---|---|
| `ColorSplit` | 分割线默认颜色 | 通过 `LineColor` 属性 Setter 绑定，作为线条的默认颜色 |
| `ColorTextHeading` | 标题文本默认颜色 | 通过 `TitleColor` 属性 Setter 绑定，作为标题的默认颜色 |
| `LineWidth` | 线条宽度 | 通过 `LineWidth` 属性 Setter 绑定，控制分割线的粗细 |
| `FontSize` | 普通文字模式字号 | `IsPlain=True` 时标题 TextBlock 使用此字号 |
| `FontSizeLG` | 标题文字字号 | `IsPlain=False` 时标题 TextBlock 使用此字号（比正文大） |

---

## Token 对外观的具体影响

### 尺寸与 Token 映射

通过 `SizeType` 属性选择不同尺寸时，主题通过以下 Token 控制水平分割线的垂直间距：

| 尺寸 | 无标题时使用的 Token | 有标题（Small）时使用的 Token |
|---|---|---|
| `Small` | `HorizontalMarginBlockSM` (`UniformlyMarginXS`) | `HorizontalWithTextGutterMargin` (`UniformlyMargin`) |
| `Middle` | `HorizontalMarginBlock` (`UniformlyMargin`) | `HorizontalMarginBlock` (`UniformlyMargin`) |
| `Large` | `HorizontalMarginBlockLG` (`UniformlyMarginLG`) | `HorizontalMarginBlockLG` (`UniformlyMarginLG`) |

> 注意：当有标题且 `SizeType=Small` 时，主题会使用 `HorizontalWithTextGutterMargin` 代替 `HorizontalMarginBlockSM`，这是为了确保带标题的分割线即使在小号模式下也保持足够的视觉间距。

### IsPlain 与标题样式 Token 映射

| IsPlain | 标题字号 Token | 标题字重 |
|---|---|---|
| `false`（默认） | `SharedToken.FontSizeLG` | 500（Medium） |
| `true` | `SharedToken.FontSize` | Normal |

### 方向与 Token 映射

| 方向 | 相关 Token | 影响 |
|---|---|---|
| `Horizontal` | `HorizontalMarginBlock*` 系列 + `TextPaddingInline` + `OrientationMarginPercent` | 控制上下间距、标题内间距、标题位置偏移比例 |
| `Vertical` | `VerticalMarginInline` | 控制垂直分割线左右的横向间距，影响 `MeasureOverride` 中的宽度计算 |

### 颜色 Token 覆盖

当开发者显式设置 `LineColor` 或 `TitleColor` 属性时，属性值会覆盖主题通过 `SharedToken.ColorSplit` / `SharedToken.ColorTextHeading` 设置的默认值：

```xml
<!-- 使用默认 Token 颜色 -->
<atom:Separator />

<!-- 覆盖线条颜色 -->
<atom:Separator LineColor="#7cb305" />

<!-- 覆盖标题颜色 -->
<atom:Separator Title="Custom" TitleColor="Coral" />
```

---

## Token 值参考（默认主题）

以下是默认（Light）主题下各 Token 的典型计算值，实际值取决于全局 `SharedToken` 配置：

| Token | 默认值（约） | 计算逻辑 |
|---|---|---|
| `TextPaddingInline` | `1.0` em | 固定值 |
| `OrientationMarginPercent` | `0.05` (5%) | 固定值 |
| `VerticalMarginInline` | `8` px | `SharedToken.UniformlyMarginXS` |
| `HorizontalMarginBlockSM` | `0,8,0,8` | `SharedToken.UniformlyMarginXS` |
| `HorizontalMarginBlock` | `0,16,0,16` | `SharedToken.UniformlyMargin` |
| `HorizontalMarginBlockLG` | `0,24,0,24` | `SharedToken.UniformlyMarginLG` |
| `HorizontalWithTextGutterMargin` | `0,16,0,16` | `SharedToken.UniformlyMargin` |
| `ColorSplit`（共享） | `rgba(5,5,5,0.06)` | 全局分隔线颜色 |
| `ColorTextHeading`（共享） | `rgba(0,0,0,0.88)` | 全局标题文本颜色 |
| `LineWidth`（共享） | `1` px | 全局线宽 |
| `FontSize`（共享） | `14` px | 全局基准字号 |
| `FontSizeLG`（共享） | `16` px | 全局大号字号 |
