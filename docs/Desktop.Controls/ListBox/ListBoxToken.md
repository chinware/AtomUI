# ListBox Design Token

ListBox 使用 `ListBoxToken`（Token ID: `"ListBox"`）作为组件级 Design Token。所有视觉属性均从全局 `SharedToken` 派生，不硬编码任何具体值。

---

## Token 资源访问方式

在 AXAML 中使用 ListBox 组件级 Token：

```xml
{atom:ListBoxTokenResource ItemColor}
{atom:ListBoxTokenResource ItemHoverBgColor}
{atom:ListBoxTokenResource ItemPadding}
```

在 AXAML 中使用全局共享 Token：

```xml
{atom:SharedTokenResource ColorPrimary}
{atom:SharedTokenResource ControlHeight}
{atom:SharedTokenResource BorderRadius}
```

---

## ListBoxToken 组件级 Token 一览

### 颜色 Token

| Token 名 | 类型 | 来源 | 说明 |
|---|---|---|---|
| `ItemColor` | `Color` | `SharedToken.ColorTextSecondary` | 列表项默认文字颜色 |
| `ItemHoverColor` | `Color` | `SharedToken.ColorTextSecondary` | 列表项悬浮态文字颜色 |
| `ItemSelectedColor` | `Color` | `SharedToken.ColorText` | 列表项选中态文字颜色 |
| `ItemDisabledColor` | `Color` | `SharedToken.ColorTextDisabled` | 列表项禁用态文字颜色 |
| `ItemBgColor` | `Color` | `SharedToken.ColorTransparent` | 列表项默认背景色（透明） |
| `ItemHoverBgColor` | `Color` | `SharedToken.ColorBgTextHover` | 列表项悬浮态背景色 |
| `ItemSelectedBgColor` | `Color` | `SharedToken.ControlItemBgActive` | 列表项选中态背景色 |
| `FilterHighlightColor` | `Color` | `SharedToken.ColorError` | 过滤匹配文本高亮颜色（红色） |

### 内边距 Token

| Token 名 | 类型 | 来源 | 说明 |
|---|---|---|---|
| `ContentPadding` | `Thickness` | `SharedToken.UniformlyPaddingXXS / 2` | ListBox 容器内边距 |
| `ItemPaddingLG` | `Thickness` | `SharedToken.UniformlyPadding, 0` | 大号列表项水平内边距 |
| `ItemPadding` | `Thickness` | `SharedToken.UniformlyPaddingSM, 0` | 中号列表项水平内边距 |
| `ItemPaddingSM` | `Thickness` | `SharedToken.UniformlyPaddingXS, 0` | 小号列表项水平内边距 |
| `ItemMargin` | `Thickness` | `0, 0.5` | 列表项外边距（上下 0.5px 间距） |

### 布局 Token

| Token 名 | 类型 | 来源 | 说明 |
|---|---|---|---|
| `SelectedIndicatorMargin` | `Thickness` | `SharedToken.UniformlyMarginXXS, 0, 0, 0` | 选中指示器与内容的间距 |

---

## 主题中使用的全局 SharedToken

除组件级 Token 外，ListBox 的 ControlTheme 还直接引用了以下全局 `SharedToken`：

### 尺寸与布局

| Token 资源键 | 使用场景 |
|---|---|
| `ControlHeightLG` | 大号列表项最小高度 |
| `ControlHeight` | 中号列表项最小高度 |
| `ControlHeightSM` | 小号列表项最小高度 |
| `BorderRadiusLG` | 大号 ListBox 圆角 |
| `BorderRadius` | 中号 ListBox 圆角、大号 ListBoxItem 圆角 |
| `BorderRadiusSM` | 小号 ListBox 圆角、中号 ListBoxItem 圆角 |
| `BorderRadiusXS` | 小号 ListBoxItem 圆角 |
| `BorderThickness` | 边框粗细 |
| `IconSize` | 选中指示器图标大小 |
| `PaddingXL` | 大号空状态指示器内边距 |
| `Padding` | 中号/小号空状态指示器内边距 |

### 颜色

| Token 资源键 | 使用场景 |
|---|---|
| `ColorBorder` | 边框颜色 |
| `ColorPrimary` | 选中指示器图标颜色 |
| `ColorTextDisabled` | 禁用态文字颜色 |
| `EnableMotion` | 全局动画开关 |

---

## Token 对外观的具体影响

### 尺寸与 Token 映射

| 尺寸 | ListBox 圆角 | Item 最小高度 | Item 圆角 | Item 水平内边距 | 空状态内边距 |
|---|---|---|---|---|---|
| `Large` | `BorderRadiusLG` | `ControlHeightLG` | `BorderRadius` | `UniformlyPadding` | `PaddingXL` |
| `Middle` | `BorderRadius` | `ControlHeight` | `BorderRadiusSM` | `UniformlyPaddingSM` | `Padding` |
| `Small` | `BorderRadiusSM` | `ControlHeightSM` | `BorderRadiusXS` | `UniformlyPaddingXS` | `Padding` |

### ListBoxItem 交互态

| 状态 | 文字颜色 Token | 背景色 Token |
|---|---|---|
| **默认** | `ItemColor`（ColorTextSecondary） | `ItemBgColor`（透明） |
| **悬浮** | `ItemHoverColor`（ColorTextSecondary） | `ItemHoverBgColor`（ColorBgTextHover） |
| **选中** | `ItemSelectedColor`（ColorText） | `ItemSelectedBgColor`（ControlItemBgActive） |
| **禁用** | `ColorTextDisabled` | — |

> `ItemHoverBg` 和 `ItemSelectedBg` 属性可覆盖 Token 默认值，允许在不修改全局 Token 的情况下自定义单个 ListBox 实例的交互态颜色。

### 过滤高亮

- `FilterHighlightColor`：默认使用 `ColorError`（红色）高亮匹配文本。
- 可通过 `FilterHighlightForeground` 属性覆盖。

### 选中指示器

- 图标颜色：`ColorPrimary`（主色调）
- 图标大小：`IconSize`
- 图标间距：`SelectedIndicatorMargin`
