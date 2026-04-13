# Dialog Design Token

Dialog 控件使用 `DialogToken`（Token ID: `"Dialog"`）作为组件级 Design Token，所有视觉属性均从全局 `SharedToken` 派生，不硬编码任何具体值。

---

## Token 资源访问方式

在 AXAML 中使用组件级 Token：

```xml
{atom:TokenResource DialogHeaderBg}
{atom:TokenResource DialogContentBg}
{atom:TokenResource DialogHeaderFontSize}
```

在 AXAML 中使用全局共享 Token：

```xml
{atom:SharedTokenResource ColorBgElevated}
{atom:SharedTokenResource FontSizeHeading5}
{atom:SharedTokenResource PaddingContentHorizontalLG}
```

---

## 组件级 Token 一览

以下是 `DialogToken` 定义的全部组件级 Token，按功能分组说明。

### 颜色 Token

| Token 名 | 类型 | 来源 | 说明 |
|---|---|---|---|
| `HeaderBg` | `Color` | `Colors.Transparent` | 标题区域背景色 |
| `HeaderColor` | `Color` | `SharedToken.ColorTextHeading` | 标题字体颜色 |
| `ContentBg` | `Color` | `SharedToken.ColorBgElevated` | 内容区域背景色 |
| `FooterBg` | `Color` | `Colors.Transparent` | 底部区域背景色 |

### 字体 Token

| Token 名 | 类型 | 来源 | 说明 |
|---|---|---|---|
| `HeaderFontSize` | `double` | `SharedToken.FontSizeHeading5` | 标题字体大小 |

### 图标 Token

| Token 名 | 类型 | 来源 | 说明 |
|---|---|---|---|
| `LogoSize` | `double` | `SharedToken.SizeLG` | Overlay 对话框标题图标大小 |
| `CloseBtnSize` | `double` | `SharedToken.ControlHeight` | 关闭按钮大小 |

### 间距 Token

| Token 名 | 类型 | 来源 | 说明 |
|---|---|---|---|
| `HeaderPadding` | `Thickness` | 基于 `SharedToken.PaddingContentHorizontalLG` 和 `SharedToken.UniformlyPaddingSM` | 标题区域内间距 |
| `HeaderMarginBottom` | `Thickness` | 基于 `SharedToken.UniformlyMarginXS` | 标题区域下外间距 |
| `ContentPadding` | `Thickness` | 基于 `SharedToken.PaddingContentHorizontalLG` 和 `SharedToken.UniformlyPaddingMD` | 内容区域内间距 |
| `FooterPadding` | `Thickness` | 基于 `SharedToken.PaddingContentHorizontalLG` 和 `SharedToken.UniformlyPaddingMD` | 底部区域内间距 |
| `FooterMarginTop` | `Thickness` | 基于 `SharedToken.UniformlyMarginXS` | 底部区域上外间距 |
| `ButtonGroupSpacing` | `double` | `SharedToken.SpacingXS` | 底部按钮之间的间距 |

### 尺寸 Token

| Token 名 | 类型 | 来源 | 说明 |
|---|---|---|---|
| `MinHeight` | `double` | 基于 `SharedToken.ControlHeightLG` + `HeaderPadding` 计算 | 对话框默认最小高度 |
| `MinWidth` | `double` | 固定 `200` | 对话框默认最小宽度 |

---

## 主题中使用的全局 SharedToken

除组件级 Token 外，Dialog 的 ControlTheme 还直接引用了以下全局 `SharedToken`：

| Token 资源键 | 使用场景 |
|---|---|
| `ColorBgElevated` | 对话框整体背景色 |
| `ColorTextHeading` | 标题文字颜色 |
| `FontSizeHeading5` | 标题字号 |
| `PaddingContentHorizontalLG` | 水平内间距基准 |
| `PaddingContentHorizontalSM` | 标题右侧内间距 |
| `UniformlyPaddingSM` / `UniformlyPaddingMD` | 垂直内间距 |
| `UniformlyMarginXS` | 标题/底部间距 |
| `SizeLG` | 图标大小 |
| `ControlHeight` / `ControlHeightLG` | 控件高度基准 |
| `SpacingXS` | 按钮间距 |
| `ColorBgMask` | Overlay 模式遮罩层背景色 |
| `BorderRadiusLG` | 对话框圆角 |
| `BoxShadowsSecondary` | 对话框阴影 |

---

## Token 对外观的具体影响

### 标题区域

- 背景色由 `HeaderBg`（默认透明）控制
- 字体大小由 `HeaderFontSize` 控制
- 字体颜色由 `HeaderColor` 控制
- 图标大小由 `LogoSize` 控制
- 内间距由 `HeaderPadding` 控制
- 与内容区域间隔由 `HeaderMarginBottom` 控制

### 内容区域

- 背景色由 `ContentBg` 控制
- 内间距由 `ContentPadding` 控制

### 底部按钮区域

- 背景色由 `FooterBg`（默认透明）控制
- 内间距由 `FooterPadding` 控制
- 与内容区域间隔由 `FooterMarginTop` 控制
- 按钮间距由 `ButtonGroupSpacing` 控制

### 整体尺寸约束

- 最小宽度由 `MinWidth`（200px）控制
- 最小高度由 `MinHeight`（从 HeaderPadding + ControlHeightLG 派生）控制
