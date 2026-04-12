# ComboBox Design Token

ComboBox 控件使用 `ComboBoxToken`（Token ID: `"ComboBox"`）作为组件级 Design Token，所有视觉属性均从全局 `SharedToken` 派生，不硬编码任何具体值。

`ComboBoxToken` 继承自 `ButtonSpinnerToken`，后者又继承自 `LineEditToken`，形成三层 Token 继承链：

```
LineEditToken (字体尺寸)
  └── ButtonSpinnerToken (操作按钮/手柄样式)
        └── ComboBoxToken (下拉列表项样式)
```

---

## Token 资源访问方式

在 AXAML 中使用组件级 Token：

```xml
{atom:ComboBoxTokenResource PopupContentPadding}
{atom:ComboBoxTokenResource ItemColor}
{atom:ComboBoxTokenResource ItemSelectedBgColor}
```

在 AXAML 中使用继承自父类的 Token（通过 ButtonSpinner 作用域访问）：

```xml
{atom:ButtonSpinnerTokenResource HandleHoverColor}
{atom:ButtonSpinnerTokenResource HandleBg}
```

在 AXAML 中使用全局共享 Token：

```xml
{atom:SharedTokenResource ColorPrimary}
{atom:SharedTokenResource ControlHeight}
{atom:SharedTokenResource FontSize}
```

---

## 组件级 Token 一览

以下是 `ComboBoxToken` 及其父类定义的全部组件级 Token，按继承层级和功能分组说明。

### ComboBox 专有 Token

| Token 名 | 类型 | 来源 | 说明 |
|---|---|---|---|
| `PopupContentPadding` | `Thickness` | `SharedToken.UniformlyPaddingXXS`, `SharedToken.BorderRadiusLG.TopLeft / 2` | 下拉面板内容区域内边距 |
| `ItemColor` | `Color` | `SharedToken.ColorTextSecondary` | 列表项正常态文字颜色 |
| `ItemHoverColor` | `Color` | `SharedToken.ColorTextSecondary` | 列表项悬浮态文字颜色 |
| `ItemSelectedColor` | `Color` | `SharedToken.ColorText` | 列表项选中态文字颜色 |
| `ItemDisabledColor` | `Color` | `SharedToken.ColorTextDisabled` | 列表项禁用态文字颜色 |
| `ItemBgColor` | `Color` | `SharedToken.ColorBgElevated` | 列表项正常态背景色 |
| `ItemHoverBgColor` | `Color` | `SharedToken.ColorBgTextHover` | 列表项悬浮态背景色 |
| `ItemSelectedBgColor` | `Color` | `SharedToken.ControlItemBgActive` | 列表项选中态背景色 |
| `ItemPadding` | `Thickness` | `SharedToken.UniformlyPaddingSM` | 列表项内间距 |
| `ItemMargin` | `Thickness` | 固定 `(0, 0.5)` | 列表项外边距（上下间距） |

### 继承自 ButtonSpinnerToken（操作手柄相关）

| Token 名 | 类型 | 来源 | 说明 |
|---|---|---|---|
| `ControlWidth` | `double` | 固定 `90` | 控件默认宽度 |
| `HandleWidth` | `double` | `SharedToken.ControlHeightSM` | 操作按钮宽度 |
| `HandleIconSize` | `double` | `SharedToken.FontSize / 2` | 操作按钮图标大小 |
| `HandleBg` | `Color` | `SharedToken.ColorBgContainer` | 操作按钮背景色 |
| `HandleActiveBg` | `Color` | `SharedToken.ColorFillAlter` | 操作按钮激活背景色 |
| `HandleHoverColor` | `Color` | `SharedToken.ColorPrimary` | 操作按钮悬浮文字/图标颜色 |
| `HandleBorderColor` | `Color` | `SharedToken.ColorBorder` | 操作按钮边框颜色 |
| `FilledHandleBg` | `Color` | 基于 `SharedToken.ColorFillSecondary` + `HandleBg` 混合 | 面性变体操作按钮背景色 |

### 继承自 LineEditToken（字体尺寸相关）

| Token 名 | 类型 | 来源 | 说明 |
|---|---|---|---|
| `InputFontSize` | `double` | `SharedToken.FontSize` | 输入区域中号字体大小 |
| `InputFontSizeLG` | `double` | `SharedToken.FontSizeLG` | 输入区域大号字体大小 |
| `InputFontSizeSM` | `double` | `SharedToken.FontSizeSM` | 输入区域小号字体大小 |

---

## 主题中使用的全局 SharedToken

除组件级 Token 外，ComboBox 的 ControlTheme 还直接引用了以下全局 `SharedToken`：

| Token 资源键 | 使用场景 |
|---|---|
| `EnableMotion` | 全局动画开关，控制下拉面板展开/收起动画 |
| `FontSize` / `FontSizeSM` / `FontSizeLG` | 选中内容字体大小（按 `SizeType` 分档） |
| `ControlHeight` | 列表项高度（中号） |
| `IconSizeSM` | 下拉手柄箭头图标尺寸 |
| `SpacingXS` | 内部后缀区域元素间距 |
| `ColorBgElevated` | 下拉面板弹出框背景色 |
| `ColorPrimaryActive` | 下拉手柄按下时的图标颜色 |
| `ColorTextDisabled` | 禁用态选项的文字颜色 |
| `ColorErrorText` | 错误状态下 Borderless/Filled 变体的选中内容前景色 |
| `ColorWarningText` | 警告状态下 Borderless/Filled 变体的选中内容前景色 |
| `BorderRadiusSM` | ComboBoxItem 选项圆角 |

### PopupHost Token（下拉面板外观）

| Token 资源键 | 使用场景 |
|---|---|
| `PopupHostTokenResource MarginToAnchor` | 下拉面板与触发控件之间的间距 |
| `PopupHostTokenResource BoxShadows` | 下拉面板阴影 |
| `PopupHostTokenResource BorderRadius` | 下拉面板圆角 |

---

## Token 对外观的具体影响

### 样式变体与 Token 映射

样式变体（Outlined / Filled / Borderless）的边框/背景样式由 `AddOnDecoratedBox` 统一管理，ComboBox 本身不直接定义这些 Token。但在错误/警告状态下，不同变体的前景色由 ComboBox 主题覆盖：

| 变体 | 错误态前景色 | 警告态前景色 |
|---|---|---|
| `Outlined` | 由 `AddOnDecoratedBox` 管理（边框变红） | 由 `AddOnDecoratedBox` 管理（边框变橙） |
| `Filled` | `ColorErrorText`（选中内容变红） | `ColorWarningText`（选中内容变橙） |
| `Borderless` | `ColorErrorText`（选中内容变红） | `ColorWarningText`（选中内容变橙） |

### ComboBoxItem 状态与 Token 映射

| 选项状态 | 前景色 Token | 背景色 Token | 附加效果 |
|---|---|---|---|
| **正常态** | `ItemColor` | `ItemBgColor` | — |
| **悬浮态** | `ItemHoverColor` | `ItemHoverBgColor` | 背景色带过渡动画 |
| **选中态** | `ItemSelectedColor` | `ItemSelectedBgColor` | `FontWeight = SemiBold` |
| **禁用态** | `ColorTextDisabled` | — | — |

### 尺寸与 Token 映射

| 尺寸 | 控件字体大小 | 选项字体大小 | 控件高度 |
|---|---|---|---|
| `Large` | `FontSizeLG` | `FontSize`（OptionFontSize 默认） | `ControlHeightLG`（由 AddOnDecoratedBox 控制） |
| `Middle` | `FontSize` | `FontSize` | `ControlHeight` |
| `Small` | `FontSizeSM` | `FontSize` | `ControlHeightSM` |

### 下拉手柄与 Token 映射

| 手柄状态 | 图标颜色 |
|---|---|
| **正常态** | 默认图标色 |
| **悬浮态** | `HandleHoverColor`（= `ColorPrimary`） |
| **按下态** | `ColorPrimaryActive` |
