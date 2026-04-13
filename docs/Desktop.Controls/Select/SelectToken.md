# Select Design Token

Select 控件使用 `SelectToken`（Token ID: `"Select"`）作为组件级 Design Token，所有视觉属性均从全局 `SharedToken` 派生，不硬编码任何具体值。

---

## Token 资源访问方式

在 AXAML 中使用组件级 Token：

```xml
{atom:SelectTokenResource MultipleItemBg}
{atom:SelectTokenResource OptionSelectedBg}
{atom:SelectTokenResource Padding}
```

在 AXAML 中使用全局共享 Token：

```xml
{atom:SharedTokenResource ColorPrimary}
{atom:SharedTokenResource ControlHeight}
{atom:SharedTokenResource ColorTextPlaceholder}
```

---

## 组件级 Token 一览

以下是 `SelectToken` 定义的全部组件级 Token，按功能分组说明。

### 多选标签 Token

| Token 名 | 类型 | 来源 | 说明 |
|---|---|---|---|
| `MultipleItemBg` | `Color` | `SharedToken.ColorFillSecondary` | 多选标签背景色 |
| `MultipleItemHeight` | `double` | 基于 `SharedToken.ControlHeight` 和 `PaddingXXS` 计算 | 多选标签高度（中号） |
| `MultipleItemHeightSM` | `double` | 基于 `SharedToken.ControlHeightSM` 和 `PaddingXXS` 计算 | 多选标签高度（小号） |
| `MultipleItemHeightLG` | `double` | 基于 `SharedToken.ControlHeightLG` 和 `PaddingXXS` 计算 | 多选标签高度（大号） |
| `MultipleSelectorBgDisabled` | `Color` | `SharedToken.ColorBgContainerDisabled` | 多选框禁用背景色 |
| `MultipleItemColorDisabled` | `Color` | `SharedToken.ColorTextDisabled` | 多选标签禁用文本颜色 |

### 选项样式 Token

| Token 名 | 类型 | 来源 | 说明 |
|---|---|---|---|
| `OptionSelectedColor` | `Color` | `SharedToken.ColorText` | 选项选中时文本颜色 |
| `OptionSelectedFontWeight` | `FontWeight` | `SharedToken.FontWeightStrong` | 选项选中时文本字重 |
| `OptionSelectedBg` | `Color` | `SharedToken.ControlItemBgActive` | 选项选中时背景色 |
| `OptionActiveBg` | `Color` | `SharedToken.ControlItemBgHover` | 选项激活（悬浮）时背景色 |
| `OptionPadding` | `Thickness` | 基于 `SharedToken.ControlPaddingHorizontal` 和 `ControlHeight` 计算 | 选项内间距 |
| `OptionFontSize` | `double` | `SharedToken.FontSize` | 选项字体大小 |
| `OptionHeight` | `double` | `SharedToken.ControlHeight` | 选项高度 |

### 内间距 Token

| Token 名 | 类型 | 来源 | 说明 |
|---|---|---|---|
| `Padding` | `Thickness` | 基于 `SharedToken.UniformlyPaddingSM` 和 `ControlHeight` 计算 | 输入框内边距（中号） |
| `PaddingSM` | `Thickness` | 基于 `SharedToken.ControlPaddingHorizontalSM` 和 `ControlHeightSM` 计算 | 输入框内边距（小号） |
| `PaddingLG` | `Thickness` | 基于 `SharedToken.ControlPaddingHorizontal` 和 `ControlHeightLG` 计算 | 输入框内边距（大号） |
| `MultiModePadding` | `Thickness` | 基于 `SharedToken.ControlHeight` 和 `UniformlyPaddingSM` 计算 | 多选模式下的输入框内边距（中号） |
| `MultiModePaddingSM` | `Thickness` | 基于 `SharedToken.ControlHeightSM` 和 `ControlPaddingHorizontalSM` 计算 | 多选模式下的输入框内边距（小号） |
| `MultiModePaddingLG` | `Thickness` | 基于 `SharedToken.ControlHeightLG` 和 `ControlPaddingHorizontal` 计算 | 多选模式下的输入框内边距（大号） |

### 其他 Token

| Token 名 | 类型 | 来源 | 说明 |
|---|---|---|---|
| `SelectAffixPadding` | `Thickness` | `SharedToken.PaddingXXS` | 前缀/后缀区域间距 |
| `FixedItemMargin` | `Thickness` | 基于 `SharedToken.UniformlyPaddingXXS / 2` 计算 | 固定项外边距 |
| `PopupContentPadding` | `Thickness` | 基于 `SharedToken.UniformlyPaddingXXS / 2` 计算 | 弹出面板内容边距 |

---

## 主题中使用的全局 SharedToken

除组件级 Token 外，Select 的 ControlTheme 还直接引用了以下全局 `SharedToken`：

| Token 资源键 | 使用场景 |
|---|---|
| `FontSize` | 中号/小号 Select 字体大小 |
| `FontSizeLG` | 大号 Select 字体大小 |
| `ControlHeight` | 选项高度基准 |
| `ColorBgElevated` | 弹出面板背景色 |
| `ColorTextPlaceholder` | 占位文本颜色 |
| `ColorText` | 搜索输入框占位文本颜色（非搜索状态） |
| `ColorTextDisabled` | 禁用状态前景色 |
| `SpacingXS` | ContentRightAddOn 区域内子元素间距 |
| `EnableMotion` | 全局动画开关 |

### 弹出面板 Token

Select 的弹出面板还引用了 `PopupHostToken` 中的以下 Token：

| Token 资源键 | 使用场景 |
|---|---|
| `PopupHostTokenResource MarginToAnchor` | 弹出面板与锚点的间距 |
| `PopupHostTokenResource BoxShadows` | 弹出面板阴影效果 |
| `PopupHostTokenResource BorderRadius` | 弹出面板圆角 |

---

## Token 对外观的具体影响

### 选择模式与模板可见性

| 模式 | `SingleSelectResultPresenter` | `SingleFilterInput` | `SelectedOptionsBox` | `SelectCandidateList` |
|---|---|---|---|---|
| **Single（无搜索）** | ✅ 可见 | ❌ 隐藏 | ❌ 隐藏 | `SelectionMode=Single`，无选中指示器 |
| **Single（有搜索）** | ❌ 隐藏 | ✅ 可见 | ❌ 隐藏 | `SelectionMode=Single`，无选中指示器 |
| **Multiple** | ❌ 隐藏 | ❌ 隐藏 | ✅ 可见 | `SelectionMode=Multiple`，有选中指示器 |
| **Tags** | ❌ 隐藏 | ❌ 隐藏 | ✅ 可见 | `SelectionMode=Multiple`，有选中指示器 |

### 尺寸与 Token 映射

| 尺寸 | 字号 | 输入框内边距 | 多选模式内边距 | 多选标签高度 |
|---|---|---|---|---|
| `Large` | `FontSizeLG` | `PaddingLG` | `MultiModePaddingLG` | `MultipleItemHeightLG` |
| `Middle` | `FontSize` | `Padding` | `MultiModePadding` | `MultipleItemHeight` |
| `Small` | `FontSize` | `PaddingSM` | `MultiModePaddingSM` | `MultipleItemHeightSM` |

### 选项交互状态颜色

| 状态 | 背景色 | 文本颜色 | 字重 |
|---|---|---|---|
| **默认** | 透明 | 继承 | 正常 |
| **悬浮** | `OptionActiveBg` | 继承 | 正常 |
| **选中** | `OptionSelectedBg` | `OptionSelectedColor` | `OptionSelectedFontWeight` |
| **禁用** | 透明 | `ColorTextDisabled` | 正常 |
