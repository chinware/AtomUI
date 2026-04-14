# TreeSelect Design Token

TreeSelect 控件使用 `TreeSelectToken`（Token ID: `"TreeSelect"`）作为组件级 Design Token。由于 TreeSelect 是一个复合控件，它还依赖 `SelectToken`（Select 共享 Token）和 `AddOnDecoratedBoxToken`、`PopupHostToken` 等共享 Token 来控制视觉表现。

---

## Token 资源访问方式

在 AXAML 中使用 TreeSelect 组件级 Token：

```xml
{atom:TreeSelectTokenResource MinPopupWidth}
```

在 AXAML 中使用 Select 共享 Token：

```xml
{atom:SelectTokenResource PopupContentPadding}
{atom:SelectTokenResource MultiModePadding}
{atom:SelectTokenResource Padding}
```

在 AXAML 中使用全局共享 Token：

```xml
{atom:SharedTokenResource ColorPrimary}
{atom:SharedTokenResource ColorBgElevated}
{atom:SharedTokenResource ControlHeight}
```

---

## 组件级 Token 一览

### TreeSelectToken

TreeSelect 自身定义的组件级 Token 较少，大部分视觉属性通过 `SelectToken` 和全局 `SharedToken` 控制。

| Token 名 | 类型 | 来源 | 说明 |
|---|---|---|---|
| `MinPopupWidth` | `double` | 固定 `300` | 弹窗的最小宽度，防止弹窗在数据较少时过窄 |

### SelectToken（继承使用）

TreeSelect 的装饰框（`TreeSelectAddOnDecoratedBox`）大量引用了 `SelectToken` 中的 Token，用于控制内边距和多选模式下的间距：

| Token 名 | 类型 | 来源 | 说明 |
|---|---|---|---|
| `PopupContentPadding` | `Thickness` | 基于 `SharedToken.UniformlyPaddingXXS` | 弹窗内容区域内边距 |
| `Padding` | `Thickness` | 基于 `SharedToken.ControlPaddingHorizontal` | 中号选择框内边距 |
| `PaddingSM` | `Thickness` | 基于 `SharedToken.ControlPaddingHorizontalSM` | 小号选择框内边距 |
| `PaddingLG` | `Thickness` | 基于 `SharedToken.ControlPaddingHorizontal` | 大号选择框内边距 |
| `MultiModePadding` | `Thickness` | 基于 `SharedToken.UniformlyPaddingSM` 等 | 多选模式下中号选择框内边距 |
| `MultiModePaddingSM` | `Thickness` | 基于 `SharedToken.ControlPaddingHorizontalSM` 等 | 多选模式下小号选择框内边距 |
| `MultiModePaddingLG` | `Thickness` | 基于 `SharedToken.ControlPaddingHorizontal` 等 | 多选模式下大号选择框内边距 |
| `MultipleItemBg` | `Color` | `SharedToken.ColorFillSecondary` | 多选标签背景色 |
| `MultipleItemHeight` | `double` | 基于 `SharedToken.ControlHeight` | 多选标签高度 |
| `MultipleItemHeightSM` | `double` | 基于 `SharedToken.ControlHeightSM` | 小号多选标签高度 |
| `MultipleItemHeightLG` | `double` | 基于 `SharedToken.ControlHeightLG` | 大号多选标签高度 |
| `MultipleSelectorBgDisabled` | `Color` | `SharedToken.ColorBgContainerDisabled` | 多选框禁用背景色 |
| `MultipleItemColorDisabled` | `Color` | `SharedToken.ColorTextDisabled` | 多选标签禁用文本颜色 |
| `OptionSelectedColor` | `Color` | `SharedToken.ColorText` | 选项选中时文本颜色 |
| `OptionSelectedFontWeight` | `FontWeight` | `SharedToken.FontWeightStrong` | 选项选中时字重 |
| `OptionSelectedBg` | `Color` | `SharedToken.ControlItemBgActive` | 选项选中时背景色 |
| `OptionActiveBg` | `Color` | `SharedToken.ControlItemBgHover` | 选项激活态背景色 |
| `OptionPadding` | `Thickness` | 基于 `SharedToken.ControlPaddingHorizontal` | 选项内间距 |
| `OptionFontSize` | `double` | `SharedToken.FontSize` | 选项字体大小 |
| `OptionHeight` | `double` | `SharedToken.ControlHeight` | 选项高度 |

---

## 主题中使用的全局 SharedToken

除组件级 Token 外，TreeSelect 的 ControlTheme 还直接引用了以下全局 `SharedToken`：

| Token 资源键 | 使用场景 |
|---|---|
| `EnableMotion` | 全局动画开关，控制弹窗展开/收起动画 |
| `ColorPrimary` | 默认的筛选高亮颜色（`FilterHighlightForeground` 默认值） |
| `ColorText` | 单选模式下选中项文本颜色 |
| `ColorTextPlaceholder` | 占位文本颜色，单选搜索时选中项降级显示颜色 |
| `ColorTextDisabled` | 禁用状态下文本颜色 |
| `ColorBgElevated` | 弹窗背景色 |
| `ColorBorder` | Outline 变体默认边框颜色 |
| `ColorError` | 错误状态边框颜色（Outline 变体） |
| `ColorErrorBorderHover` | 错误状态悬浮边框颜色 |
| `ColorErrorBg` | 错误状态背景色（Filled 变体） |
| `ColorErrorBgHover` | 错误状态悬浮背景色（Filled 变体） |
| `ColorWarning` | 警告状态边框颜色（Outline 变体） |
| `ColorWarningBorderHover` | 警告状态悬浮边框颜色 |
| `ColorWarningBg` | 警告状态背景色（Filled 变体） |
| `ColorWarningBgHover` | 警告状态悬浮背景色（Filled 变体） |
| `ColorFillTertiary` | Filled 变体默认背景色 |
| `ColorFillSecondary` | Filled 变体悬浮背景色 |
| `ControlHeight` | 选项高度（`ItemHeight`） |
| `FontSize` | 中号/小号字体大小 |
| `FontSizeLG` | 大号字体大小 |
| `SpacingXS` | 右侧附加内容区域元素间距 |

### AddOnDecoratedBoxToken（通过装饰框引用）

| Token 资源键 | 使用场景 |
|---|---|
| `HoverBorderColor` | 装饰框悬浮时边框颜色 |
| `ActiveBorderColor` | 装饰框激活/聚焦/下拉打开时边框颜色 |
| `ActiveBg` | Filled 变体聚焦时背景色 |

### PopupHostToken（弹窗层引用）

| Token 资源键 | 使用场景 |
|---|---|
| `MarginToAnchor` | 弹窗与锚点控件之间的间距 |
| `BoxShadows` | 弹窗阴影效果 |
| `BorderRadius` | 弹窗圆角半径 |

---

## Token 对外观的具体影响

### 样式变体与 Token 映射

| 样式变体 | 正常态 | 悬浮态 | 聚焦/激活态 | 禁用态 |
|---|---|---|---|---|
| **Outline** | `ColorBorder` 边框 | `HoverBorderColor` 边框 | `ActiveBorderColor` 边框 | 灰色调 |
| **Filled** | `ColorFillTertiary` 背景，透明边框 | `ColorFillSecondary` 背景 | `ActiveBorderColor` 边框，透明背景 | 灰色调 |
| **Borderless** | 无边框无背景 | — | — | — |
| **Underlined** | 仅底部边框 | — | — | — |

### 状态与 Token 映射

| 状态 | Outline 变体 | Filled 变体 |
|---|---|---|
| **Error** | `ColorError` 边框；悬浮 `ColorErrorBorderHover` | `ColorErrorBg` 背景；悬浮 `ColorErrorBgHover`；聚焦 `ColorError` 边框 |
| **Warning** | `ColorWarning` 边框；悬浮 `ColorWarningBorderHover` | `ColorWarningBg` 背景；悬浮 `ColorWarningBgHover`；聚焦 `ColorWarning` 边框 |

### 尺寸与 Token 映射

| 尺寸 | 字号 | 单选内边距 | 多选内边距 |
|---|---|---|---|
| `Large` | `FontSizeLG` | `SelectToken.PaddingLG` | `SelectToken.MultiModePaddingLG` |
| `Middle` | `FontSize` | `SelectToken.Padding` | `SelectToken.MultiModePadding` |
| `Small` | `FontSize` | `SelectToken.Padding` | `SelectToken.MultiModePaddingSM` |

### 弹窗相关 Token

| Token | 影响 |
|---|---|
| `TreeSelectToken.MinPopupWidth` | 弹窗最小宽度（默认 300px） |
| `SelectToken.PopupContentPadding` | 弹窗内部 TreeView 周围的内边距 |
| `PopupHostToken.BoxShadows` | 弹窗投影效果 |
| `PopupHostToken.BorderRadius` | 弹窗圆角 |
| `PopupHostToken.MarginToAnchor` | 弹窗与选择框之间的间距 |
| `SharedToken.ColorBgElevated` | 弹窗背景色 |
