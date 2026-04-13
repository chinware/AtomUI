# NavMenu Design Token

NavMenu 控件使用 `NavMenuToken`（Token ID: `"NavMenu"`）作为组件级 Design Token，所有视觉属性均从全局 `SharedToken` 派生，不硬编码任何具体值。

---

## Token 资源访问方式

在 AXAML 中使用组件级 Token：

```xml
{atom:NavMenuTokenResource ItemColor}
{atom:NavMenuTokenResource ItemSelectedBg}
{atom:NavMenuTokenResource DarkMenuBg}
```

在 AXAML 中使用全局共享 Token：

```xml
{atom:SharedTokenResource ColorPrimary}
{atom:SharedTokenResource ControlHeight}
{atom:SharedTokenResource ColorText}
```

---

## 组件级 Token 一览

以下是 `NavMenuToken` 定义的全部组件级 Token，按功能分组说明。

### 菜单项颜色 Token

| Token 名 | 类型 | 来源 | 说明 |
|---|---|---|---|
| `ItemColor` | `Color` | `SharedToken.ColorText` | 菜单项文字颜色 |
| `ItemHoverColor` | `Color` | `SharedToken.ColorText` | 菜单项悬浮文字颜色 |
| `ItemSelectedColor` | `Color` | `SharedToken.ColorPrimary` | 菜单项选中文字颜色 |
| `ItemDisabledColor` | `Color` | `SharedToken.ColorTextDisabled` | 菜单项禁用文字颜色 |
| `HorizontalItemHoverColor` | `Color` | `SharedToken.ColorPrimary` | 水平菜单项悬浮文字颜色 |
| `HorizontalItemSelectedColor` | `Color` | `SharedToken.ColorPrimary` | 水平菜单项选中文字颜色 |
| `GroupTitleColor` | `Color` | `SharedToken.ColorTextDescription` | 分组标题文字颜色 |
| `KeyGestureColor` | `Color` | `SharedToken.ColorTextSecondary` | 快捷键文字颜色 |

### 菜单项背景 Token

| Token 名 | 类型 | 来源 | 说明 |
|---|---|---|---|
| `ItemBg` | `Color` | `SharedToken.ColorTransparent` | 菜单项默认背景色（透明） |
| `ItemHoverBg` | `Color` | `SharedToken.ColorBgTextHover` | 菜单项悬浮背景色 |
| `ItemActiveBg` | `Color` | `SharedToken.ColorFillContent` | 菜单项激活背景色 |
| `ItemSelectedBg` | `Color` | `SharedToken.ControlItemBgActive` | 菜单项选中背景色 |
| `SubMenuItemBg` | `Color` | `SharedToken.ColorFillAlter` | 子菜单项背景色 |
| `HorizontalItemSelectedBg` | `Color` | `Colors.Transparent` | 水平菜单项选中背景色 |
| `HorizontalItemHoverBg` | `Color` | `Colors.Transparent` | 水平菜单项悬浮背景色 |

### 危险菜单项 Token

| Token 名 | 类型 | 来源 | 说明 |
|---|---|---|---|
| `DangerItemColor` | `Color` | `SharedToken.ColorError` | 危险菜单项文字颜色 |
| `DangerItemHoverColor` | `Color` | `SharedToken.ColorError` | 危险菜单项悬浮文字颜色 |
| `DangerItemSelectedColor` | `Color` | `SharedToken.ColorError` | 危险菜单项选中文字颜色 |
| `DangerItemActiveBg` | `Color` | `SharedToken.ColorError` | 危险菜单项激活背景色 |
| `DangerItemSelectedBg` | `Color` | `SharedToken.ColorError` | 危险菜单项选中背景色 |

### 圆角 Token

| Token 名 | 类型 | 来源 | 说明 |
|---|---|---|---|
| `ItemBorderRadius` | `CornerRadius` | `SharedToken.BorderRadiusLG` | 菜单项圆角 |
| `SubMenuItemBorderRadius` | `CornerRadius` | `SharedToken.BorderRadiusSM` | 子菜单项圆角 |
| `HorizontalItemBorderRadius` | `CornerRadius` | `new CornerRadius(0)` | 水平菜单项圆角（无圆角） |

### 尺寸与间距 Token

| Token 名 | 类型 | 来源 | 说明 |
|---|---|---|---|
| `ItemHeight` | `double` | `SharedToken.ControlHeight` | 菜单项高度 |
| `ItemContentMargin` | `Thickness` | `SharedToken.UniformlyMarginXXS` | 菜单项内容外间距 |
| `ItemContentPadding` | `Thickness` | 基于 `SharedToken.UniformlyPadding` | 菜单项内容内间距 |
| `ItemMargin` | `Thickness` | `(0, 0, SharedToken.UniformlyMarginXS, 0)` | 菜单项内部元素边距 |
| `HorizontalItemMargin` | `Thickness` | `(SharedToken.UniformlyPadding, 0)` | 水平菜单项外间距 |
| `InlineItemIndentUnit` | `double` | `ItemHeight / 2` | 内联菜单项缩进单位 |
| `VerticalItemsPanelSpacing` | `double` | `SharedToken.UniformlyMarginXXS` | 垂直面板元素间距 |
| `VerticalChildItemsMargin` | `Thickness` | `(0, SharedToken.UniformlyMarginXXS, 0, 0)` | 垂直面板子项外间距 |
| `VerticalMenuContentPadding` | `Thickness` | `SharedToken.PaddingXXS` | 垂直面板内容内间距 |

### 图标 Token

| Token 名 | 类型 | 来源 | 说明 |
|---|---|---|---|
| `IconSize` | `double` | `SharedToken.FontSize` | 菜单项图标尺寸 |
| `IconMargin` | `Thickness` | 基于 `SharedToken.ControlHeightSM - FontSize` | 图标与文字间距 |
| `ItemIconSize` | `double` | `SharedToken.IconSize` | 菜单项图标展示尺寸 |
| `CollapsedIconSize` | `double` | `SharedToken.FontSizeLG` | 收起时图标尺寸 |

### 字体 Token

| Token 名 | 类型 | 来源 | 说明 |
|---|---|---|---|
| `GroupTitleFontSize` | `double` | `SharedToken.FontSize` | 分组标题字体大小 |
| `GroupTitleLineHeight` | `double` | `SharedToken.ControlHeight` | 分组标题行高 |

### 水平菜单 Token

| Token 名 | 类型 | 来源 | 说明 |
|---|---|---|---|
| `MenuHorizontalHeight` | `double` | `SharedToken.ControlHeightLG * 1.15` | 水平菜单整体高度 |
| `HorizontalLineHeight` | `double` | `SharedToken.ControlHeightLG * 1.15` | 水平菜单行高 |
| `ActiveBarScaleX` | `double` | `1.0` | 选中指示条宽度比例 |
| `ActiveBarHeight` | `double` | `SharedToken.LineWidthBold` | 选中指示条高度 |

### Popup Token

| Token 名 | 类型 | 来源 | 说明 |
|---|---|---|---|
| `MenuPopupBg` | `Color` | `SharedToken.ColorBgElevated` | Popup 背景色 |
| `MenuPopupContentPadding` | `Thickness` | 基于 `SharedToken.UniformlyPaddingXXS` | Popup 内容内间距 |
| `MenuPopupMinWidth` | `double` | `160` | Popup 最小宽度 |
| `MenuPopupMaxWidth` | `double` | `800` | Popup 最大宽度 |
| `MenuPopupMaxHeight` | `double` | `ItemHeight * 30` | Popup 最大高度 |
| `TopLevelItemPopupMarginToAnchor` | `double` | `SharedToken.UniformlyMarginXS` | 顶层菜单项 Popup 边距 |
| `MenuSubMenuBg` | `Color` | `SharedToken.ColorBgElevated` | 子菜单背景色 |
| `MenuArrowSize` | `double` | `SharedToken.FontSize / 7 * 5` | 菜单箭头尺寸 |

### 其他 Token

| Token 名 | 类型 | 来源 | 说明 |
|---|---|---|---|
| `CollapsedWidth` | `double` | `SharedToken.ControlHeight * 2` | 收起后菜单宽度 |

---

## 暗色主题 Token

暗色主题使用一套完全独立的 Token，当 `IsDarkStyle=True` 时生效：

| Token 名 | 类型 | 来源 | 说明 |
|---|---|---|---|
| `DarkMenuBg` | `Color` | `#001529` | 暗色菜单背景色 |
| `DarkMenuPopupBg` | `Color` | `#001529` | 暗色 Popup 背景色 |
| `DarkItemColor` | `Color` | 基于 `ColorTextLightSolid` 65% 透明度 | 暗色菜单项文字颜色 |
| `DarkItemBg` | `Color` | `Colors.Transparent` | 暗色菜单项背景色 |
| `DarkSubMenuItemBg` | `Color` | `#000c17` | 暗色子菜单项背景色 |
| `DarkItemSelectedColor` | `Color` | `SharedToken.ColorTextLightSolid` | 暗色选中文字颜色 |
| `DarkItemSelectedBg` | `Color` | `SharedToken.ColorPrimary` | 暗色选中背景色 |
| `DarkItemHoverBg` | `Color` | `Colors.Transparent` | 暗色悬浮背景色 |
| `DarkItemHoverColor` | `Color` | `SharedToken.ColorTextLightSolid` | 暗色悬浮文字颜色 |
| `DarkItemDisabledColor` | `Color` | 基于 `ColorTextLightSolid` 25% 透明度 | 暗色禁用文字颜色 |
| `DarkGroupTitleColor` | `Color` | 基于 `ColorTextLightSolid` 65% 透明度 | 暗色分组标题颜色 |
| `DarkDangerItemColor` | `Color` | `SharedToken.ColorError` | 暗色危险菜单项文字颜色 |
| `DarkDangerItemHoverColor` | `Color` | `SharedToken.ColorErrorHover` | 暗色危险菜单项悬浮颜色 |
| `DarkDangerItemSelectedColor` | `Color` | `SharedToken.ColorTextLightSolid` | 暗色危险菜单项选中颜色 |
| `DarkDangerItemSelectedBg` | `Color` | `SharedToken.ColorError` | 暗色危险菜单项选中背景 |
| `DarkDangerItemActiveBg` | `Color` | `SharedToken.ColorError` | 暗色危险菜单项激活背景 |

---

## 主题中使用的全局 SharedToken

除组件级 Token 外，NavMenu 的 ControlTheme 还直接引用了以下全局 `SharedToken`：

| Token 资源键 | 使用场景 |
|---|---|
| `ColorPrimary` / `ColorPrimaryHover` / `ColorPrimaryActive` | 选中项、悬浮项主色调 |
| `ColorText` / `ColorTextDescription` / `ColorTextDisabled` | 文字颜色系列 |
| `ColorTextLightSolid` | 亮色文字（用于暗色主题推导） |
| `ColorTextSecondary` | 次要文字（快捷键颜色） |
| `ColorBgTextHover` | 文本悬浮背景色 |
| `ColorBgElevated` | 弹出层背景色 |
| `ColorBgContainer` | 容器背景色 |
| `ColorFillContent` / `ColorFillAlter` | 填充颜色 |
| `ColorTransparent` | 透明色 |
| `ColorError` / `ColorErrorHover` | 错误/危险色系 |
| `ControlHeight` / `ControlHeightSM` / `ControlHeightLG` | 控件高度 |
| `ControlItemBgActive` | 活跃项背景色 |
| `BorderRadius` / `BorderRadiusSM` / `BorderRadiusLG` | 圆角系列 |
| `FontSize` / `FontSizeLG` | 字体大小 |
| `IconSize` / `IconSizeSM` | 全局图标尺寸 |
| `LineWidthBold` | 粗线宽度（指示条高度） |
| `EnableMotion` | 全局动画开关 |

---

## Token 对外观的具体影响

### 亮色主题下菜单项状态与 Token 映射

| 状态 | 文字色 | 背景色 |
|---|---|---|
| **正常** | `ItemColor` | `ItemBg`（透明） |
| **悬浮** | `ItemHoverColor` | `ItemHoverBg` |
| **激活** | — | `ItemActiveBg` |
| **选中** | `ItemSelectedColor` | `ItemSelectedBg` |
| **禁用** | `ItemDisabledColor` | — |

### 暗色主题下菜单项状态与 Token 映射

| 状态 | 文字色 | 背景色 |
|---|---|---|
| **正常** | `DarkItemColor` | `DarkItemBg`（透明） |
| **悬浮** | `DarkItemHoverColor` | `DarkItemHoverBg` |
| **选中** | `DarkItemSelectedColor` | `DarkItemSelectedBg` |
| **禁用** | `DarkItemDisabledColor` | — |

### 水平模式特殊 Token

水平模式下，选中项不使用背景色高亮，而是通过底部指示条（`ActiveBarHeight` × `ActiveBarScaleX`）标识选中状态。选中文字使用 `HorizontalItemSelectedColor`，悬浮文字使用 `HorizontalItemHoverColor`。
