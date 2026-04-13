# ListView Design Token

ListView 控件使用 `ListViewToken`（Token ID: `"ListView"`）作为组件级 Design Token，所有视觉属性均从全局 `SharedToken` 派生，不硬编码任何具体值。

---

## Token 资源访问方式

在 AXAML 中使用组件级 Token：

```xml
{atom:ListViewTokenResource ContentPadding}
{atom:ListViewTokenResource ItemColor}
{atom:ListViewTokenResource ItemPadding}
```

在 AXAML 中使用全局共享 Token：

```xml
{atom:SharedTokenResource ColorPrimary}
{atom:SharedTokenResource ControlHeight}
{atom:SharedTokenResource BorderThickness}
```

---

## 组件级 Token 一览

以下是 `ListViewToken` 定义的全部组件级 Token，按功能分组说明。

### 颜色 Token

| Token 名 | 类型 | 来源 | 说明 |
|---|---|---|---|
| `ItemColor` | `Color` | `SharedToken.ColorTextSecondary` | 列表项文字颜色 |
| `ItemHoverColor` | `Color` | `SharedToken.ColorTextSecondary` | 列表项文字悬浮颜色 |
| `ItemSelectedColor` | `Color` | `SharedToken.ColorText` | 列表项文字选中颜色 |
| `ItemDisabledColor` | `Color` | `SharedToken.ColorTextDisabled` | 列表项文字禁用颜色 |
| `GroupHeaderColor` | `Color` | `SharedToken.ColorTextDescription` | 分组标题颜色 |

### 背景色 Token

| Token 名 | 类型 | 来源 | 说明 |
|---|---|---|---|
| `ItemBgColor` | `Color` | `SharedToken.ColorTransparent` | 列表项背景色（默认透明） |
| `ItemHoverBgColor` | `Color` | `SharedToken.ColorBgTextHover` | 列表项悬浮态背景色 |
| `ItemSelectedBgColor` | `Color` | `SharedToken.ControlItemBgActive` | 列表项选中态背景色 |

### 内间距 Token

| Token 名 | 类型 | 来源 | 说明 |
|---|---|---|---|
| `ContentPadding` | `Thickness` | `SharedToken.UniformlyPaddingXXS / 2` | ListView 容器内边距 |
| `ItemPaddingSM` | `Thickness` | `SharedToken.UniformlyPaddingXS, 0` | 小号列表项内间距 |
| `ItemPadding` | `Thickness` | `SharedToken.UniformlyPaddingSM, 0` | 中号列表项内间距 |
| `ItemPaddingLG` | `Thickness` | `SharedToken.UniformlyPadding, 0` | 大号列表项内间距 |

### 间距 Token

| Token 名 | 类型 | 来源 | 说明 |
|---|---|---|---|
| `ItemMargin` | `Thickness` | `0, 0.5` | 列表项之间的外边距 |
| `PaginationMargin` | `Thickness` | `0, SharedToken.UniformlyMarginXS` | 分页器的外边距 |
| `SelectedIndicatorMargin` | `Thickness` | `SharedToken.UniformlyMarginXXS, 0, 0, 0` | 选中指示器的外边距 |

---

## 主题中使用的全局 SharedToken

除组件级 Token 外，ListView 的 ControlTheme 还直接引用了以下全局 `SharedToken`：

### ListView 主题引用

| Token 资源键 | 使用场景 |
|---|---|
| `ColorBorder` | ListView 默认边框颜色 |
| `BorderThickness` | ListView 默认边框粗细 |
| `EnableMotion` | 全局动画开关 |
| `BorderRadiusLG` | 大号 ListView 圆角半径 |
| `BorderRadius` | 中号 ListView 圆角半径 |
| `BorderRadiusSM` | 小号 ListView 圆角半径 |
| `PaddingXL` | 大号空状态指示器内边距 |
| `Padding` | 中号/小号空状态指示器内边距 |

### ListViewItem 主题引用

| Token 资源键 | 使用场景 |
|---|---|
| `ColorPrimary` | 选中指示器图标颜色 |
| `IconSize` | 选中指示器图标尺寸 |
| `ColorTextDisabled` | 禁用态文本颜色 |
| `BorderRadius` / `BorderRadiusSM` / `BorderRadiusXS` | 列表项圆角（大/中/小） |
| `ControlHeightLG` / `ControlHeight` / `ControlHeightSM` | 列表项最小高度（大/中/小） |
| `FontSize` / `FontSizeSM` | 分组标题字号（大号 / 中小号） |

---

## Token 对外观的具体影响

### 列表项状态与 Token 映射

| 状态 | 文字颜色 | 背景色 |
|---|---|---|
| **正常** | `ItemColor` | `ItemBgColor`（透明） |
| **悬浮** | `ItemHoverColor` | `ItemHoverBgColor`（或 `ItemHoverBg` 属性覆盖） |
| **选中** | `ItemSelectedColor` | `ItemSelectedBgColor`（或 `ItemSelectedBg` 属性覆盖） |
| **禁用** | `ColorTextDisabled` | — |
| **分组标题** | `GroupHeaderColor` | `ItemBgColor`（透明） |

> **注意**：悬浮和选中背景色有两层机制——默认由 Token 控制，但可通过 `ItemHoverBg` / `ItemSelectedBg` 属性在实例级别覆盖。属性值优先级高于 Token。

### 尺寸与 Token 映射

| 尺寸 | 最小高度 | 内间距 | 圆角 |
|---|---|---|---|
| `Large` | `ControlHeightLG` | `ItemPaddingLG` | `BorderRadius` |
| `Middle` | `ControlHeight` | `ItemPadding` | `BorderRadiusSM` |
| `Small` | `ControlHeightSM` | `ItemPaddingSM` | `BorderRadiusXS` |

### ListView 容器尺寸映射

| 尺寸 | 圆角 | 空状态内边距 |
|---|---|---|
| `Large` | `BorderRadiusLG` | `PaddingXL` |
| `Middle` | `BorderRadius` | `Padding` |
| `Small` | `BorderRadiusSM` | `Padding` |
