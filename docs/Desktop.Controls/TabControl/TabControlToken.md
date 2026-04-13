# TabControl Design Token

TabControl 控件家族使用 `TabControlToken`（Token ID: `"TabControl"`）作为组件级 Design Token，所有视觉属性均从全局 `SharedToken` 派生。TabControl 和 TabStrip 系列共享同一套 Token。

---

## Token 资源访问方式

在 AXAML 中使用组件级 Token：

```xml
{atom:TabControlTokenResource InkBarColor}
{atom:TabControlTokenResource CardBg}
{atom:TabControlTokenResource ItemColor}
```

在 AXAML 中使用全局共享 Token：

```xml
{atom:SharedTokenResource ColorPrimary}
{atom:SharedTokenResource ColorBorderSecondary}
```

---

## 组件级 Token 一览

### 卡片式 Token

| Token 名 | 类型 | 来源 | 说明 |
|---|---|---|---|
| `CardBg` | `Color` | `SharedToken.ColorFillAlter` | 卡片标签页背景色 |
| `CardSize` | `double` | `SharedToken.ControlHeightLG` | 卡片标签页大小 |
| `CardPadding` | `Thickness` | 基于 `UniformlyPadding` 和 `FontSize` | 卡片标签页中号内间距 |
| `CardPaddingSM` | `Thickness` | 基于 `UniformlyPaddingXXS` | 卡片标签页小号内间距 |
| `CardPaddingLG` | `Thickness` | 基于 `UniformlyPaddingXS` | 卡片标签页大号内间距 |
| `CardGutter` | `double` | `SharedToken.UniformlyMarginXXS / 2` | 卡片标签间距 |

### 字体 Token

| Token 名 | 类型 | 来源 | 说明 |
|---|---|---|---|
| `TitleFontSize` | `double` | `SharedToken.FontSize` | 标签标题字号（中号） |
| `TitleFontSizeLG` | `double` | `SharedToken.FontSizeLG` | 标签标题字号（大号） |
| `TitleFontSizeSM` | `double` | `SharedToken.FontSize` | 标签标题字号（小号） |

### 指示器 Token

| Token 名 | 类型 | 来源 | 说明 |
|---|---|---|---|
| `InkBarColor` | `Color` | `SharedToken.ColorPrimary` | 线条式选中指示器颜色（主色调） |

### 间距与布局 Token

| Token 名 | 类型 | 来源 | 说明 |
|---|---|---|---|
| `HorizontalItemGutter` | `double` | 固定 `32` | 横向标签间距 |
| `HorizontalItemMargin` | `Thickness` | `Thickness(0)` | 横向标签外间距 |
| `HorizontalItemPadding` | `Thickness` | 基于 `UniformlyPaddingSM` | 横向标签中号内间距 |
| `HorizontalItemPaddingSM` | `Thickness` | 基于 `UniformlyPaddingXS` | 横向标签小号内间距 |
| `HorizontalItemPaddingLG` | `Thickness` | 基于 `UniformlyPadding` | 横向标签大号内间距 |
| `HorizontalMargin` | `Thickness` | 基于 `UniformlyMargin` | 横向标签页外间距 |
| `VerticalItemGutter` | `double` | `SharedToken.UniformlyMargin` | 纵向标签间距 |
| `VerticalItemPadding` | `Thickness` | 基于 `UniformlyPaddingXS` | 纵向标签内间距 |
| `TabAndContentGutter` | `double` | `SharedToken.UniformlyMarginSM` | 标签栏与内容区域间距 |

### 颜色 Token

| Token 名 | 类型 | 来源 | 说明 |
|---|---|---|---|
| `ItemColor` | `Color` | `SharedToken.ColorText` | 标签默认文本颜色 |
| `ItemHoverColor` | `Color` | `SharedToken.ColorPrimaryHover` | 标签悬浮态文本颜色 |
| `ItemSelectedColor` | `Color` | `SharedToken.ColorPrimary` | 标签选中态文本颜色 |

### 图标与按钮 Token

| Token 名 | 类型 | 来源 | 说明 |
|---|---|---|---|
| `ItemIconMargin` | `Thickness` | 基于 `UniformlyMarginSM` | 标签图标外边距 |
| `CloseIconMargin` | `Thickness` | 基于 `UniformlyMarginXXS` | 关闭按钮外边距 |
| `AddTabButtonMarginHorizontal` | `Thickness` | 基于 `UniformlyMarginXXS` | 水平添加按钮外边距 |
| `AddTabButtonMarginVertical` | `Thickness` | 基于 `UniformlyMarginXXS` | 垂直添加按钮外边距 |

### 滚动相关 Token

| Token 名 | 类型 | 来源 | 说明 |
|---|---|---|---|
| `MenuIndicatorPaddingHorizontal` | `Thickness` | 基于 `UniformlyPaddingXS` | 水平溢出菜单指示器间距 |
| `MenuIndicatorPaddingVertical` | `Thickness` | 基于 `UniformlyPaddingXS` | 垂直溢出菜单指示器间距 |
| `MenuEdgeThickness` | `double` | 固定 `20` | 滚动边缘检测厚度 |

---

## Token 对外观的具体影响

### 状态与颜色映射

| 状态 | 标签文本色 | 图标色 |
|---|---|---|
| **默认** | `ItemColor`（`ColorText`） | `ItemColor` |
| **悬浮** | `ItemHoverColor`（`ColorPrimaryHover`） | `ItemHoverColor` |
| **选中** | `ItemSelectedColor`（`ColorPrimary`） | `ItemSelectedColor` |
| **禁用** | `ColorTextDisabled` | `ColorTextDisabled` |

### 尺寸与 Token 映射

| 尺寸 | 标题字号 | 横向标签内间距 | 卡片内间距 |
|---|---|---|---|
| `Small` | `TitleFontSizeSM` | `HorizontalItemPaddingSM` | `CardPaddingSM` |
| `Middle` | `TitleFontSize` | `HorizontalItemPadding` | `CardPadding` |
| `Large` | `TitleFontSizeLG` | `HorizontalItemPaddingLG` | `CardPaddingLG` |

---

## 主题中使用的全局 SharedToken

| Token 资源键 | 使用场景 |
|---|---|
| `ColorPrimary` | 选中态文本色、指示器颜色 |
| `ColorPrimaryHover` | 悬浮态文本色 |
| `ColorText` | 默认态文本色 |
| `ColorTextDisabled` | 禁用态文本色 |
| `ColorFillAlter` | 卡片标签背景色 |
| `ColorBorderSecondary` | 标签栏分割线颜色 |
| `BorderThickness` | 分割线厚度 |
| `EnableMotion` | 全局动画开关 |
| `IconSizeSM` | 关闭按钮图标尺寸 |
