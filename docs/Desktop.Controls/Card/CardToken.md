# Card Design Token

Card 控件使用 `CardToken`（Token ID: `"Card"`）作为组件级 Design Token，所有视觉属性均从全局 `SharedToken` 派生，不硬编码任何具体值。

---

## Token 资源访问方式

在 AXAML 中使用组件级 Token：

```xml
{atom:CardTokenResource HeaderBg}
{atom:CardTokenResource BodyPadding}
{atom:CardTokenResource CardShadows}
```

在 AXAML 中使用全局共享 Token：

```xml
{atom:SharedTokenResource ColorBorderSecondary}
{atom:SharedTokenResource BorderRadiusLG}
{atom:SharedTokenResource BorderThickness}
```

---

## 组件级 Token 一览

以下是 `CardToken` 定义的全部组件级 Token，按功能分组说明。

### 颜色 Token

| Token 名 | 类型 | 来源 | 说明 |
|---|---|---|---|
| `HeaderBg` | `Color` | `Colors.Transparent` | 卡片头部背景色（内嵌模式时由主题切换为 `ColorFillAlter`） |
| `ActionsBg` | `Color` | `SharedToken.ColorBgContainer` | 操作区背景色 |
| `ExtraColor` | `Color` | `SharedToken.ColorText` | 额外区域文字颜色 |

### 字号 Token

| Token 名 | 类型 | 来源 | 说明 |
|---|---|---|---|
| `HeaderFontSizeLG` | `double` | `SharedToken.FontSizeLG` | 大号卡片标题字号 |
| `HeaderFontSize` | `double` | `SharedToken.FontSize` | 默认卡片标题字号 |
| `HeaderFontSizeSM` | `double` | `SharedToken.FontSize` | 小号卡片标题字号 |

### 头部高度 Token

| Token 名 | 类型 | 来源 | 说明 |
|---|---|---|---|
| `HeaderHeightLG` | `double` | `SharedToken.FontHeightLG + SharedToken.UniformlyPadding * 2` | 大号卡片头部高度 |
| `HeaderHeight` | `double` | `SharedToken.FontHeight + SharedToken.UniformlyPaddingXS * 2` | 默认卡片头部高度 |
| `HeaderHeightSM` | `double` | `SharedToken.FontHeightSM` | 小号卡片头部高度 |

### 内容区内边距 Token

| Token 名 | 类型 | 来源 | 说明 |
|---|---|---|---|
| `BodyPaddingLG` | `Thickness` | `SharedToken.PaddingLG` | 大号卡片内容区内边距 |
| `BodyPadding` | `Thickness` | `SharedToken.PaddingSM` | 默认卡片内容区内边距 |
| `BodyPaddingSM` | `Thickness` | `SharedToken.PaddingXS` | 小号卡片内容区内边距 |

### 头部内边距 Token

| Token 名 | 类型 | 来源 | 说明 |
|---|---|---|---|
| `HeaderPaddingLG` | `Thickness` | `new Thickness(SharedToken.UniformlyPaddingLG, 0)` | 大号卡片头部水平内边距 |
| `HeaderPadding` | `Thickness` | `new Thickness(SharedToken.UniformlyPaddingSM, 0)` | 默认卡片头部水平内边距 |
| `HeaderPaddingSM` | `Thickness` | `new Thickness(SharedToken.UniformlyPaddingXS, 0)` | 小号卡片头部水平内边距 |

### 阴影 Token

| Token 名 | 类型 | 来源 | 说明 |
|---|---|---|---|
| `CardShadows` | `BoxShadows` | 三层阴影叠加（`0 1 2 -1`, `0 3 6 0`, `0 5 12 4`） | 卡片悬浮时的深阴影效果 |
| `CardGridItemShadows` | `BoxShadows` | 基于 `SharedToken.LineWidth` 和 `SharedToken.ColorBorderSecondary` | Grid 单元格边框阴影（模拟分割线） |

### 操作区 Token

| Token 名 | 类型 | 来源 | 说明 |
|---|---|---|---|
| `CardActionsIconSize` | `double` | `SharedToken.FontSize` | 操作区图标大小 |
| `ActionsSpacing` | `double` | `SharedToken.SpacingSM` | 操作项之间的间距 |

### 其他 Token

| Token 名 | 类型 | 来源 | 说明 |
|---|---|---|---|
| `TabsMarginBottom` | `Thickness` | `new Thickness(0, 0, 0, -SharedToken.UniformlyPadding - SharedToken.LineWidth)` | Tabs 内容下间距（负值使标签页与内容紧贴） |
| `CardHeadPadding` | `Thickness` | `SharedToken.Padding` | 卡片头部通用内边距 |
| `CardPaddingBase` | `Thickness` | `SharedToken.PaddingLG` | 卡片基础内边距 |

---

## 主题中使用的全局 SharedToken

除组件级 Token 外，Card 的 ControlTheme 还直接引用了以下全局 `SharedToken`：

| Token 资源键 | 使用场景 |
|---|---|
| `BorderRadiusLG` | 卡片默认圆角半径 |
| `BorderThickness` | 卡片默认边框厚度 |
| `ColorBorderSecondary` | 卡片边框颜色和头部底部分割线颜色 |
| `ColorBgContainer` | 卡片默认背景色 |
| `ColorFillAlter` | 内嵌模式（`IsInnerMode=True`）时的头部背景色 |
| `BoxShadowsTertiary` | 无边框风格（`Borderless`）时的默认阴影 |
| `EnableMotion` | 全局动画开关 |
| `FontWeightStrong` | 标题文字字重 |
| `Spacing` | CardMetaContent 中头像与文字的间距 |
| `SpacingXS` | CardMetaContent 中标题与描述的间距 |
| `FontSizeLG` | CardMetaContent 标题字号 |
| `ColorTextHeading` | CardMetaContent 标题前景色 |
| `ColorTextDescription` | CardMetaContent 描述前景色 |
| `ColorIcon` | CardActionButton 正常态图标颜色 |
| `ColorPrimary` | CardActionButton 悬浮态图标颜色 |

---

## Token 对外观的具体影响

### 风格变体与 Token 映射

| 风格 | 边框 | 阴影 | 背景 |
|---|---|---|---|
| **Outline** | `ColorBorderSecondary` / `BorderThickness` | 无阴影（透明） | `ColorBgContainer` |
| **Borderless** | 无边框（`Thickness=0`） | `BoxShadowsTertiary` | `ColorBgContainer` |

### 悬浮态 Token 覆盖

当 `IsHoverable=True` 且鼠标悬浮（`:pointerover`）时：
- `BoxShadow` → `CardShadows`（三层深阴影）
- `BorderThickness` → `0`（隐藏边框，以阴影替代）

### 内嵌模式 Token 覆盖

当 `IsInnerMode=True` 时：
- 头部背景 `HeaderBg` → `ColorFillAlter`（区分内外卡片层次）

### 尺寸与 Token 映射

| 尺寸 | 头部高度 | 头部字号 | 头部内边距 | 内容内边距 |
|---|---|---|---|---|
| `Large` | `HeaderHeightLG` | `HeaderFontSizeLG` | `HeaderPaddingLG` | `BodyPaddingLG` |
| `Middle` | `HeaderHeight` | `HeaderFontSize` | `HeaderPadding` | `BodyPadding` |
| `Small` | `HeaderHeightSM` | `HeaderFontSizeSM` | `HeaderPaddingSM` | `BodyPaddingSM` |

### 内容类型与 Token 影响

| 内容类型 | 内容内边距 | 头部底部边框 | 主框架圆角 |
|---|---|---|---|
| `Default` / `Meta` | 由尺寸决定（`BodyPadding*`） | 正常显示 | 完整圆角 |
| `Tabs` | 无内边距（Tab 控件自行管理） | 正常显示 | 完整圆角 |
| `Grid` | 无内边距 | 隐藏 | 仅保留顶部圆角 |

### CardGridItem 的 Token 使用

| 尺寸 | 单元格内边距 |
|---|---|
| `Large` | `BodyPaddingLG` |
| `Middle` | `BodyPadding` |
| `Small` | `BodyPaddingSM` |

悬浮时（`IsHoverable=True` + `:pointerover`）：`BoxShadow` → `CardShadows`
