# Collapse Design Token

Collapse 控件使用 `CollapseToken`（Token ID: `"Collapse"`）作为组件级 Design Token，所有视觉属性均从全局 `SharedToken` 派生，不硬编码任何具体值。

---

## Token 资源访问方式

在 AXAML 中使用组件级 Token：

```xml
{atom:CollapseTokenResource HeaderPadding}
{atom:CollapseTokenResource HeaderBg}
{atom:CollapseTokenResource ContentPadding}
{atom:CollapseTokenResource ContentBg}
```

在 AXAML 中使用全局共享 Token：

```xml
{atom:SharedTokenResource ColorBorder}
{atom:SharedTokenResource ColorFillAlter}
{atom:SharedTokenResource BorderThickness}
```

---

## 组件级 Token 一览

以下是 `CollapseToken` 定义的全部组件级 Token，按功能分组说明。

### 间距 Token — 头部内间距

| Token 名 | 类型 | 来源 | 说明 |
|---|---|---|---|
| `HeaderPadding` | `Thickness` | `(SharedToken.UniformlyPadding, SharedToken.UniformlyPaddingSM)` | 中号头部内间距 |
| `CollapseHeaderPaddingSM` | `Thickness` | `(SharedToken.UniformlyPaddingSM, SharedToken.UniformlyPaddingXS)` | 小号头部内间距 |
| `CollapseHeaderPaddingLG` | `Thickness` | `(SharedToken.UniformlyPaddingLG, SharedToken.UniformlyPadding)` | 大号头部内间距 |

### 间距 Token — 内容内间距

| Token 名 | 类型 | 来源 | 说明 |
|---|---|---|---|
| `ContentPadding` | `Thickness` | `(16, SharedToken.UniformlyPadding)` | 中号内容内间距 |
| `CollapseContentPaddingSM` | `Thickness` | `SharedToken.PaddingSM` | 小号内容内间距 |
| `CollapseContentPaddingLG` | `Thickness` | `SharedToken.PaddingLG` | 大号内容内间距 |

### 颜色 Token

| Token 名 | 类型 | 来源 | 说明 |
|---|---|---|---|
| `HeaderBg` | `Color` | `SharedToken.ColorFillAlter` | 头部背景色（默认模式下） |
| `ContentBg` | `Color` | `SharedToken.ColorBgContainer` | 内容区域背景色 |

### 展开按钮间距 Token

展开按钮在不同尺寸和位置下使用不同的间距值：

| Token 名 | 类型 | 来源 | 说明 |
|---|---|---|---|
| `LeftExpandButtonMarginSM` | `Thickness` | `(0, 0, SharedToken.UniformlyMarginXXS, 0)` | 小号-左侧展开按钮右间距 |
| `LeftExpandButtonMargin` | `Thickness` | `(0, 0, SharedToken.UniformlyMarginXS, 0)` | 中号-左侧展开按钮右间距 |
| `LeftExpandButtonMarginLG` | `Thickness` | `(0, 0, SharedToken.UniformlyMarginSM, 0)` | 大号-左侧展开按钮右间距 |
| `RightExpandButtonMarginSM` | `Thickness` | `(SharedToken.UniformlyMarginXXS, 0, 0, 0)` | 小号-右侧展开按钮左间距 |
| `RightExpandButtonMargin` | `Thickness` | `(SharedToken.UniformlyMarginXS, 0, 0, 0)` | 中号-右侧展开按钮左间距 |
| `RightExpandButtonMarginLG` | `Thickness` | `(SharedToken.UniformlyMarginSM, 0, 0, 0)` | 大号-右侧展开按钮左间距 |

### 圆角 Token

| Token 名 | 类型 | 来源 | 说明 |
|---|---|---|---|
| `CollapsePanelBorderRadius` | `CornerRadius` | `SharedToken.BorderRadiusLG` | 折叠面板外框圆角 |

---

## 主题中使用的全局 SharedToken

除组件级 Token 外，Collapse 的 ControlTheme 还直接引用了以下全局 `SharedToken`：

| Token 资源键 | 使用场景 |
|---|---|
| `BorderThickness` | 面板默认边框厚度 |
| `ColorBorder` | 面板边框颜色、头部分隔线颜色、内容分隔线颜色 |
| `EnableMotion` | 全局动画开关 |
| `MotionDurationSlow` | 折叠/展开动画时长 |
| `ColorTextHeading` | 头部标题文本颜色 |
| `ColorTextDisabled` | 禁用态文本颜色 |
| `FontSize` | 中号/小号头部字体大小 |
| `FontSizeLG` | 大号头部字体大小 |
| `FontHeight` | 中号/小号头部行高 |
| `FontHeightLG` | 大号头部行高 |
| `IconSizeSM` | 展开图标尺寸 |

---

## Token 对外观的具体影响

### 尺寸与 Token 映射

| 尺寸 | 头部内间距 | 内容内间距 | 头部字号 | 头部行高 | 展开按钮左侧间距 | 展开按钮右侧间距 |
|---|---|---|---|---|---|---|
| `Large` | `CollapseHeaderPaddingLG` | `CollapseContentPaddingLG` | `FontSizeLG` | `FontHeightLG` | `LeftExpandButtonMarginLG` | `RightExpandButtonMarginLG` |
| `Middle` | `HeaderPadding` | `ContentPadding` | `FontSize` | `FontHeight` | `LeftExpandButtonMargin` | `RightExpandButtonMargin` |
| `Small` | `CollapseHeaderPaddingSM` | `CollapseContentPaddingSM` | `FontSize` | `FontHeight` | `LeftExpandButtonMarginSM` | `RightExpandButtonMarginSM` |

### 样式模式与 Token 覆盖

| 样式模式 | 属性设置 | Token 变化 |
|---|---|---|
| **默认** | — | 头部背景 = `HeaderBg`（`ColorFillAlter`），内容背景 = 无（透明），外框边框 = `BorderThickness` + `ColorBorder` |
| **无边框** `IsBorderless=True` | 外框 `EffectiveBorderThickness = 0` | 内容区域背景变为 `HeaderBg`（`ColorFillAlter`） |
| **幽灵** `IsGhostStyle=True` | 外框 `EffectiveBorderThickness = 0` | 头部背景变为 `ContentBg`（`ColorBgContainer`），边框全部为 0 |

### 禁用态

当 `IsEnabled = false` 时：
- 头部标题前景色变为 `ColorTextDisabled`
- 所有子内容的前景色也变为 `ColorTextDisabled`

### 展开图标位置与间距

展开图标按钮的间距随 `ExpandIconPosition` 和 `SizeType` 的组合变化：

| 图标位置 | Grid 列 | 间距 Token（以中号为例） |
|---|---|---|
| `Start`（左侧） | Column 0 | `LeftExpandButtonMargin` = `(0, 0, UniformlyMarginXS, 0)` — 右侧有间距 |
| `End`（右侧） | Column 3 | `RightExpandButtonMargin` = `(UniformlyMarginXS, 0, 0, 0)` — 左侧有间距 |

### 自定义间距优先级

`ItemHeaderPadding` / `ItemContentPadding`（Collapse 级）和 `HeaderPadding` / `ContentPadding`（CollapseItem 级）会触发 `:custom-header-padding` / `:custom-content-padding` 伪类，此时 Token 定义的尺寸相关间距（如 `HeaderPadding`、`CollapseHeaderPaddingSM` 等）不再生效。

优先级关系：

```
CollapseItem.HeaderPadding（直接设置） > Collapse.ItemHeaderPadding（批量设置） > Token 默认值（按 SizeType）
```
