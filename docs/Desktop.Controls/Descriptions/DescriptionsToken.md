# Descriptions Design Token

Descriptions 使用 `DescriptionsToken`（Token ID: `"Descriptions"`）作为组件级 Design Token。所有视觉属性均从全局 `SharedToken` 派生，不使用任何硬编码值。

> 源码位置：`src/AtomUI.Desktop.Controls/Descriptions/DescriptionsToken.cs`

---

## Token 资源访问方式

在 AXAML 中通过标记扩展引用 Token：

```xml
<!-- 组件级 Token（Descriptions 专属） -->
{atom:DescriptionsTokenResource LabelBg}
{atom:DescriptionsTokenResource LabelColor}

<!-- 全局共享 Token -->
{atom:SharedTokenResource ColorPrimary}
{atom:SharedTokenResource FontSize}
```

---

## 组件级 Token 列表

以下 Token 定义在 `DescriptionsToken` 类中，通过 `CalculateTokenValues()` 从全局 `SharedToken` 派生：

| Token 名称 | 类型 | 派生自 | 说明 |
|---|---|---|---|
| `LabelBg` | `Color` | `SharedToken.ColorFillAlter` | 标签背景色（有边框模式下标签单元格的背景色） |
| `LabelColor` | `Color` | `SharedToken.ColorTextTertiary` | 标签文字颜色（无边框模式下的标签色） |
| `TitleColor` | `Color` | `SharedToken.ColorText` | 标题（Header）文字颜色 |
| `HeaderMargin` | `Thickness` | `(0, 0, 0, FontSizeSM * RelativeLineHeightSM)` | 标题栏底部间距 |
| `ItemPaddingLG` | `Thickness` | `(UniformlyPaddingLG, UniformlyPadding)` | 大号尺寸的子项内间距 |
| `ItemPadding` | `Thickness` | `(UniformlyPaddingLG, UniformlyPaddingSM)` | 中号尺寸的子项内间距 |
| `ItemPaddingSM` | `Thickness` | `(UniformlyPadding, UniformlyPaddingXS)` | 小号尺寸的子项内间距 |
| `ColonMargin` | `Thickness` | `(MarginXXS/2, 0, MarginXS, 0)` | 冒号的边距 |
| `ContentColor` | `Color` | `SharedToken.ColorText` | 内容区域文字颜色 |
| `ExtraColor` | `Color` | `SharedToken.ColorText` | 额外操作区域文字颜色 |

---

## Token 在不同模式下的使用场景

### 按边框模式

| Token | 无边框模式 | 有边框模式 |
|---|---|---|
| `LabelBg` | ❌ 不使用 | ✅ 标签单元格背景色 |
| `LabelColor` | ✅ 标签文字色 | ❌ 使用 `SharedToken.ColorTextSecondary` |
| `ContentColor` | ✅ 内容文字色 | ✅ 内容文字色 |
| `ColonMargin` | ✅ 冒号间距 | ❌ 不使用 |

> 注意：有边框模式下，标签的文字颜色使用的是 `SharedToken.ColorTextSecondary` 而非组件级的 `LabelColor`。这与 Ant Design 的实现保持一致。

### 按尺寸

| Token | SizeType=Large | SizeType=Middle | SizeType=Small |
|---|---|---|---|
| `ItemPaddingLG` | ✅ | ❌ | ❌ |
| `ItemPadding` | ❌ | ✅ | ❌ |
| `ItemPaddingSM` | ❌ | ❌ | ✅ |

---

## 引用的全局共享 Token

除组件级 Token 外，Descriptions 的主题还直接引用以下全局 `SharedToken`：

| SharedToken | AXAML 引用方式 | 使用场景 |
|---|---|---|
| `ColorSplit` | `{atom:SharedTokenResource ColorSplit}` | 有边框模式下的边框颜色、垂直布局有边框的分隔线颜色 |
| `BorderThickness` | `{atom:SharedTokenResource BorderThickness}` | 有边框模式的边框粗细 |
| `BorderRadiusLG` | `{atom:SharedTokenResource BorderRadiusLG}` | 有边框模式内容框的圆角半径 |
| `FontSizeLG` | `{atom:SharedTokenResource FontSizeLG}` | 标题字号 |
| `FontWeightStrong` | `{atom:SharedTokenResource FontWeightStrong}` | 标题字重 |
| `FontHeightLG` | `{atom:SharedTokenResource FontHeightLG}` | 标题行高 |
| `RelativeLineHeight` | `{atom:SharedTokenResource RelativeLineHeight}` | 内容区域相对行高 |
| `FontSize` | `{atom:SharedTokenResource FontSize}` | 默认字号 |
| `LineWidth` | `{atom:SharedTokenResource LineWidth}` | 垂直有边框模式下分隔线高度 |
| `Spacing` | `{atom:SharedTokenResource Spacing}` | 无边框 Large 尺寸的行间距 |
| `SpacingSM` | `{atom:SharedTokenResource SpacingSM}` | 无边框 Middle 尺寸的行间距 |
| `SpacingXS` | `{atom:SharedTokenResource SpacingXS}` | 无边框 Small 尺寸的行间距 |
| `ColorTextSecondary` | `{atom:SharedTokenResource ColorTextSecondary}` | 有边框模式下标签文字色 |

---

## Token 值计算逻辑

```csharp
public override void CalculateTokenValues(bool isDarkMode)
{
    base.CalculateTokenValues(isDarkMode);
    LabelBg       = SharedToken.ColorFillAlter;
    LabelColor    = SharedToken.ColorTextTertiary;
    TitleColor    = SharedToken.ColorText;
    HeaderMargin  = new Thickness(0, 0, 0, SharedToken.FontSizeSM * SharedToken.RelativeLineHeightSM);
    ItemPaddingLG = new Thickness(SharedToken.UniformlyPaddingLG, SharedToken.UniformlyPadding);
    ItemPadding   = new Thickness(SharedToken.UniformlyPaddingLG, SharedToken.UniformlyPaddingSM);
    ItemPaddingSM = new Thickness(SharedToken.UniformlyPadding, SharedToken.UniformlyPaddingXS);
    ColonMargin   = new Thickness(SharedToken.UniformlyMarginXXS / 2, 0, SharedToken.UniformlyMarginXS, 0);
    ContentColor  = SharedToken.ColorText;
    ExtraColor    = SharedToken.ColorText;
}
```

所有值均从 `SharedToken` 派生，当全局主题变更（如切换 Dark 模式或 Compact 模式）时，Token 会自动重新计算。

---

## 暗色模式

`CalculateTokenValues` 接收 `isDarkMode` 参数。当前 Descriptions 的 Token 计算逻辑不区分亮色/暗色模式，但由于所有值均从 `SharedToken` 派生，而 `SharedToken` 在暗色模式下会使用暗色主题算法重新计算，因此 Descriptions 的视觉表现会自动跟随全局主题切换。
