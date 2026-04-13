# Menu Design Token

Menu 使用 `MenuToken`（Token ID: `"Menu"`）作为组件级 Design Token。所有视觉属性均从全局 `SharedToken` 派生，不使用硬编码值。

> 📖 Token 源码位置：`src/AtomUI.Desktop.Controls/Menu/MenuToken.cs`

---

## Token 资源访问方式

### AXAML 中访问

```xml
<!-- 组件级 Token（Menu 专属） -->
{atom:MenuTokenResource ItemColor}
{atom:MenuTokenResource ItemHoverBg}
{atom:MenuTokenResource MenuPopupBgColor}

<!-- 全局共享 Token -->
{atom:SharedTokenResource ColorPrimary}
{atom:SharedTokenResource ColorBgElevated}
```

---

## Token 类定义

```csharp
[ControlDesignToken]
internal class MenuToken : AbstractControlDesignToken
{
    public const string ID = "Menu";
    public static readonly ControlTokenResourceScopeProvider ScopeProvider = new(ID);
    // ...
}
```

---

## 完整 Token 属性列表

### 弹窗相关

| Token 属性 | 类型 | 派生自 SharedToken | 说明 |
|---|---|---|---|
| `MenuPopupContentPadding` | `Thickness` | `UniformlyPaddingXXS`, `BorderRadiusLG` | 菜单弹窗内容内间距 |
| `MenuPopupMinWidth` | `double` | 固定值 `120` | 菜单弹窗最小宽度 |
| `MenuPopupMaxWidth` | `double` | 固定值 `800` | 菜单弹窗最大宽度 |
| `MenuPopupBgColor` | `Color` | `ColorBgElevated` | 菜单弹窗背景色 |

### 菜单项基础

| Token 属性 | 类型 | 派生自 SharedToken | 说明 |
|---|---|---|---|
| `ItemColor` | `Color` | `ColorText` | 菜单项文字颜色 |
| `ItemHoverColor` | `Color` | 同 `ItemColor` | 菜单项悬浮文字颜色 |
| `ItemDisabledColor` | `Color` | `ColorTextDisabled` | 菜单项禁用文字颜色 |
| `ItemBg` | `Color` | `ColorBgElevated` | 菜单项背景色 |
| `ItemHoverBg` | `Color` | `ColorBgTextHover` | 菜单项悬浮背景色 |
| `ItemHeight` | `double` | `ControlHeight` | 菜单项高度 |
| `ItemBorderRadius` | `CornerRadius` | `BorderRadius` | 菜单项圆角 |
| `ItemPaddingInline` | `Thickness` | `UniformlyPadding`, `UniformlyPaddingXXS` | 菜单项横向内间距 |
| `ItemMargin` | `Thickness` | `UniformlyMarginXXS` | 菜单项间距（右边距） |

### 图标

| Token 属性 | 类型 | 派生自 SharedToken | 说明 |
|---|---|---|---|
| `ItemIconSize` | `double` | `IconSize` | 图标尺寸 |
| `ItemIconMarginInlineEnd` | `double` | `ControlHeight - FontSize` | 图标与文字间距 |

### 快捷键

| Token 属性 | 类型 | 派生自 SharedToken | 说明 |
|---|---|---|---|
| `KeyGestureColor` | `Color` | `ColorTextQuaternary` | 快捷键提示文字颜色 |

### 危险菜单项

| Token 属性 | 类型 | 派生自 SharedToken | 说明 |
|---|---|---|---|
| `DangerItemColor` | `Color` | `ColorError` | 危险菜单项文字颜色 |
| `DangerItemHoverColor` | `Color` | `ColorError` | 危险菜单项悬浮文字颜色 |

### 顶层菜单项（TopLevel）

| Token 属性 | 类型 | 派生自 SharedToken | 说明 |
|---|---|---|---|
| `TopLevelItemColor` | `Color` | `ColorText` | 顶层菜单项文字颜色 |
| `TopLevelItemSelectedColor` | `Color` | `ColorTextSecondary` | 顶层菜单项选中文字颜色 |
| `TopLevelItemHoverColor` | `Color` | `ColorTextSecondary` | 顶层菜单项悬浮文字颜色 |
| `TopLevelItemBg` | `Color` | `ColorBgContainer` | 顶层菜单项背景色 |
| `TopLevelItemSelectedBg` | `Color` | `ColorBgTextHover` | 顶层菜单项选中背景色 |
| `TopLevelItemHoverBg` | `Color` | `ColorBgTextHover` | 顶层菜单项悬浮背景色 |

### 顶层菜单项 — 圆角（按尺寸）

| Token 属性 | 类型 | 派生自 SharedToken | 说明 |
|---|---|---|---|
| `TopLevelItemBorderRadiusSM` | `CornerRadius` | `BorderRadiusSM` | 小号圆角 |
| `TopLevelItemBorderRadius` | `CornerRadius` | `BorderRadius` | 中号圆角 |
| `TopLevelItemBorderRadiusLG` | `CornerRadius` | `BorderRadiusLG` | 大号圆角 |

### 顶层菜单项 — 内间距（按尺寸）

| Token 属性 | 类型 | 派生自 SharedToken | 说明 |
|---|---|---|---|
| `TopLevelItemPaddingSM` | `Thickness` | `PaddingContentHorizontalXS × 0.7`, `ControlHeightSM` | 小号内间距 |
| `TopLevelItemPadding` | `Thickness` | `PaddingContentHorizontalXS`, `ControlHeight` | 中号内间距 |
| `TopLevelItemPaddingLG` | `Thickness` | `PaddingContentHorizontalSM`, `ControlHeightLG` | 大号内间距 |

### 顶层菜单项 — 字体（按尺寸）

| Token 属性 | 类型 | 默认值 | 说明 |
|---|---|---|---|
| `TopLevelItemFontSizeSM` | `double` | `SharedToken.FontSize` | 小号字体大小 |
| `TopLevelItemFontSize` | `double` | `SharedToken.FontSize` | 中号字体大小 |
| `TopLevelItemFontSizeLG` | `double` | `SharedToken.FontSizeLG` | 大号字体大小 |
| `TopLevelItemLineHeightSM` | `double` | 自动计算 | 小号行高 |
| `TopLevelItemLineHeight` | `double` | 自动计算 | 中号行高 |
| `TopLevelItemLineHeightLG` | `double` | 自动计算 | 大号行高 |

> 行高计算公式：`CalculatorUtils.CalculateLineHeight(fontSize) × fontSize`

### 顶层弹出菜单

| Token 属性 | 类型 | 派生自 SharedToken | 说明 |
|---|---|---|---|
| `TopLevelItemPopupMarginToAnchor` | `double` | `UniformlyMarginXXS` | 顶层弹出菜单距离锚点的间距 |

### 分隔线

| Token 属性 | 类型 | 派生自 SharedToken | 说明 |
|---|---|---|---|
| `SeparatorItemHeight` | `double` | `LineWidth × 5` | 分隔线总高度（含上下间距） |

### 上下文菜单偏移

| Token 属性 | 类型 | 派生自 SharedToken | 说明 |
|---|---|---|---|
| `ContextMenuOffsetX` | `double` | `SpacingXXS` | 上下文菜单水平偏移 |
| `ContextMenuOffsetY` | `double` | `SpacingXXS` | 上下文菜单垂直偏移 |

### 其他

| Token 属性 | 类型 | 说明 |
|---|---|---|
| `MenuTearOffHeight` | `double` | 分离菜单项高度（`ItemHeight × 1.2`，暂未使用） |

---

## Token 在主题中的使用示例

### MenuItemTheme.axaml 中的 Token 引用

```xml
<!-- 菜单项基础样式 -->
<Setter Property="Foreground" Value="{atom:MenuTokenResource ItemColor}" />
<Setter Property="Padding" Value="{atom:MenuTokenResource ItemPaddingInline}" />
<Setter Property="Background" Value="{atom:MenuTokenResource ItemBg}" />
<Setter Property="CornerRadius" Value="{atom:MenuTokenResource ItemBorderRadius}" />
<Setter Property="Height" Value="{atom:MenuTokenResource ItemHeight}" />

<!-- 悬浮状态 -->
<Style Selector="^:pointerover">
    <Setter Property="Foreground" Value="{atom:MenuTokenResource ItemHoverColor}" />
    <Setter Property="Background" Value="{atom:MenuTokenResource ItemHoverBg}" />
</Style>

<!-- 禁用状态 -->
<Style Selector="^:disabled">
    <Setter Property="Foreground" Value="{atom:MenuTokenResource ItemDisabledColor}" />
</Style>

<!-- 图标 -->
<Style Selector="^ /template/ atom|IconPresenter#ItemIconPresenter">
    <Setter Property="Width" Value="{atom:MenuTokenResource ItemIconSize}" />
    <Setter Property="Height" Value="{atom:MenuTokenResource ItemIconSize}" />
    <Setter Property="IconBrush" Value="{atom:MenuTokenResource ItemColor}" />
</Style>

<!-- 快捷键文字 -->
<Style Selector="^ /template/ atom|TextBlock#InputGestureText">
    <Setter Property="Foreground" Value="{atom:MenuTokenResource KeyGestureColor}" />
</Style>
```

### TopLevelMenuItemTheme.axaml 中的 Token 引用

```xml
<!-- 悬浮状态 -->
<Style Selector="^:pointerover">
    <Setter Property="Background" Value="{atom:MenuTokenResource TopLevelItemHoverBg}" />
    <Setter Property="Foreground" Value="{atom:MenuTokenResource TopLevelItemHoverColor}" />
</Style>

<!-- 尺寸变体 -->
<Style Selector="^[SizeType=Large]">
    <Setter Property="CornerRadius" Value="{atom:MenuTokenResource TopLevelItemBorderRadiusLG}" />
    <Setter Property="Padding" Value="{atom:MenuTokenResource TopLevelItemPaddingLG}" />
    <Setter Property="FontSize" Value="{atom:MenuTokenResource TopLevelItemFontSizeLG}" />
</Style>
```

### ContextMenuTheme.axaml 中的 Token 引用

```xml
<Setter Property="HorizontalOffset" Value="{atom:MenuTokenResource ContextMenuOffsetX}" />
<Setter Property="VerticalOffset" Value="{atom:MenuTokenResource ContextMenuOffsetY}" />
<Setter Property="Padding" Value="{atom:MenuTokenResource MenuPopupContentPadding}" />
<Setter Property="Background" Value="{atom:MenuTokenResource MenuPopupBgColor}" />
<Setter Property="MinWidth" Value="{atom:MenuTokenResource MenuPopupMinWidth}" />
<Setter Property="MaxWidth" Value="{atom:MenuTokenResource MenuPopupMaxWidth}" />
<Setter Property="ItemHeight" Value="{atom:MenuTokenResource ItemHeight}" />
```

---

## Token 派生链总结

```
SharedToken (DesignToken)
├── ColorText           → ItemColor, TopLevelItemColor
├── ColorTextSecondary  → TopLevelItemSelectedColor, TopLevelItemHoverColor
├── ColorTextQuaternary → KeyGestureColor
├── ColorTextDisabled   → ItemDisabledColor
├── ColorError          → DangerItemColor, DangerItemHoverColor
├── ColorBgElevated     → ItemBg, MenuPopupBgColor
├── ColorBgTextHover    → ItemHoverBg, TopLevelItemHoverBg, TopLevelItemSelectedBg
├── ColorBgContainer    → TopLevelItemBg
├── ControlHeight       → ItemHeight
├── BorderRadius        → ItemBorderRadius, TopLevelItemBorderRadius
├── BorderRadiusSM      → TopLevelItemBorderRadiusSM
├── BorderRadiusLG      → TopLevelItemBorderRadiusLG
├── IconSize            → ItemIconSize
├── FontSize            → TopLevelItemFontSize, TopLevelItemFontSizeSM
├── FontSizeLG          → TopLevelItemFontSizeLG
├── LineWidth           → SeparatorItemHeight (× 5)
├── UniformlyPadding    → ItemPaddingInline (水平)
├── UniformlyPaddingXXS → ItemPaddingInline (垂直), MenuPopupContentPadding
├── UniformlyMarginXXS  → ItemMargin, TopLevelItemPopupMarginToAnchor
├── SpacingXXS          → ContextMenuOffsetX, ContextMenuOffsetY
├── PaddingContentHorizontalXS → TopLevelItemPadding, TopLevelItemPaddingSM
└── PaddingContentHorizontalSM → TopLevelItemPaddingLG
```
