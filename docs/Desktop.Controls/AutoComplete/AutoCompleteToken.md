# AutoComplete Design Token

AutoComplete 控件使用 `AutoCompleteToken`（Token ID: `"AutoComplete"`）作为组件级 Design Token，所有视觉属性均从全局 `SharedToken` 派生，不硬编码任何具体值。

---

## Token 资源访问方式

在 AXAML 中使用组件级 Token：

```xml
{atom:AutoCompleteTokenResource OptionHeight}
{atom:AutoCompleteTokenResource PopupContentPadding}
{atom:AutoCompleteTokenResource MinPopupWidth}
{atom:AutoCompleteTokenResource MaxPopupWidth}
```

在 AXAML 中使用全局共享 Token：

```xml
{atom:SharedTokenResource ColorPrimary}
{atom:SharedTokenResource ControlHeight}
{atom:SharedTokenResource ColorBgElevated}
```

---

## 组件级 Token 一览

以下是 `AutoCompleteToken` 定义的全部组件级 Token。

### 弹出层 Token

| Token 名 | 类型 | 来源 | 说明 |
|---|---|---|---|
| `PopupContentPadding` | `Thickness` | `SharedToken.UniformlyPaddingXXS / 2` | 弹出层候选列表的内边距 |
| `OptionHeight` | `double` | `SharedToken.ControlHeight` | 每个候选选项的高度 |
| `MinPopupWidth` | `double` | 固定 `120` | 候选列表弹出层的最小宽度 |
| `MaxPopupWidth` | `double` | 固定 `200` | 在不跟随输入框宽度（`IsPopupMatchSelectWidth=False`）时的弹出层最大宽度 |

---

## 主题中使用的全局 SharedToken

AutoComplete 的 ControlTheme（`AbstractAutoCompleteTheme.axaml`）直接引用了以下全局 `SharedToken`：

| Token 资源键 | 使用场景 |
|---|---|
| `EnableMotion` | 全局动画开关，控制弹出/关闭动画是否启用 |
| `ColorTextPlaceholder` | 占位文本颜色 |
| `ColorBgElevated` | 弹出层背景色（浮层提升背景） |

### 弹出层样式 Token

弹出层框架引用了 `PopupHostToken` 中的全局 Token：

| Token 资源键 | 使用场景 |
|---|---|
| `PopupHostToken.MarginToAnchor` | 弹出层与锚点（输入框）之间的间距 |
| `PopupHostToken.BoxShadows` | 弹出层阴影效果 |
| `PopupHostToken.BorderRadius` | 弹出层圆角 |

### 内嵌输入框使用的 Token

AutoComplete 的输入框部分由内嵌的 `LineEdit`、`SearchEdit` 或 `TextArea` 控件渲染，这些控件各自拥有独立的 Token 系统。AutoComplete 通过 `TemplateBinding` 将以下属性透传给内嵌输入框：

| 透传属性 | 影响 |
|---|---|
| `SizeType` | 内嵌输入框的尺寸，决定高度、字号、圆角等 |
| `StyleVariant` | 内嵌输入框的样式变体（Outline / Filled / Borderless / Underlined） |
| `Status` | 验证状态视觉反馈（Error → 红色边框，Warning → 橙色边框） |
| `IsAllowClear` | 清除按钮显示 |
| `ClearIcon` | 自定义清除图标 |
| `PlaceholderText` | 占位文本 |
| `IsReadOnly` | 只读模式 |
| `IsMotionEnabled` | 动画开关 |
| `CaretIndex` | 光标位置双向绑定 |

因此，AutoComplete 的输入框外观实际由对应输入框控件的 Token（如 `LineEditToken`、`SearchEditToken`、`TextAreaToken`）控制，具体包括：

- **边框颜色**：正常态 / 悬浮态 / 焦点态 / 错误态 / 警告态
- **背景色**：各变体（Outline / Filled / Borderless / Underlined）的背景
- **字体大小**：根据 `SizeType` 由 `SharedToken.FontSize` / `FontSizeSM` / `FontSizeLG` 决定
- **圆角**：根据 `SizeType` 由 `SharedToken.BorderRadius` / `BorderRadiusSM` / `BorderRadiusLG` 决定
- **高度**：根据 `SizeType` 由 `SharedToken.ControlHeight` / `ControlHeightSM` / `ControlHeightLG` 决定

---

## Token 对外观的具体影响

### 弹出层外观

| 视觉特征 | Token 来源 | 说明 |
|---|---|---|
| 弹出层背景 | `SharedToken.ColorBgElevated` | 浮层使用提升背景色，通常比容器背景稍亮 |
| 弹出层圆角 | `PopupHostToken.BorderRadius` | 统一的弹出层圆角 |
| 弹出层阴影 | `PopupHostToken.BoxShadows` | 浮层投影效果 |
| 弹出层内边距 | `AutoCompleteToken.PopupContentPadding` | 候选列表与弹出层边框之间的间距 |
| 弹出层与输入框间距 | `PopupHostToken.MarginToAnchor` | 候选列表与输入框之间的垂直间距 |
| 弹出层最小宽度 | `AutoCompleteToken.MinPopupWidth` | 确保弹出层有最小可读宽度 |
| 弹出层最大宽度 | `AutoCompleteToken.MaxPopupWidth` | 非等宽模式下限制最大宽度 |

### 候选选项外观

| 视觉特征 | Token 来源 | 说明 |
|---|---|---|
| 选项高度 | `AutoCompleteToken.OptionHeight` = `SharedToken.ControlHeight` | 每个候选选项的行高 |
| 选项悬浮背景 | 由 `CandidateList` 控件的 Token 控制 | 鼠标悬浮时的高亮背景 |
| 选项选中背景 | 由 `CandidateList` 控件的 Token 控制 | 键盘选中时的高亮背景 |

### 尺寸与 Token 映射

候选列表的 `MaxPopupHeight` 由以下公式计算：

```
MaxPopupHeight = OptionHeight × DisplayCandidateCount + PopupContentPadding.Top + PopupContentPadding.Bottom
```

其中 `OptionHeight` 默认等于 `SharedToken.ControlHeight`，`DisplayCandidateCount` 默认为 `10`。

| 尺寸 | 输入框高度 | 选项高度 | 可见候选数 | 弹出层最大高度 |
|---|---|---|---|---|
| `Large` | `ControlHeightLG` | `ControlHeight` | 10（默认） | 约 `ControlHeight × 10 + padding` |
| `Middle` | `ControlHeight` | `ControlHeight` | 10（默认） | 约 `ControlHeight × 10 + padding` |
| `Small` | `ControlHeightSM` | `ControlHeight` | 10（默认） | 约 `ControlHeight × 10 + padding` |

> 注意：`SizeType` 仅影响内嵌输入框的尺寸，候选选项的高度始终为 `ControlHeight`，不随 `SizeType` 变化。
