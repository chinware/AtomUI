# Input Design Token

Input 控件族使用两个组件级 Design Token：

- `LineEditToken`（Token ID: `"LineEdit"`）— 供 TextBox、LineEdit、SearchEdit 使用
- `TextAreaToken`（Token ID: `"TextArea"`）— 供 TextArea 使用

所有视觉属性均从全局 `SharedToken` 派生，不硬编码任何具体值。

---

## Token 资源访问方式

在 AXAML 中使用 LineEdit 组件级 Token：

```xml
{atom:LineEditTokenResource InputFontSize}
{atom:LineEditTokenResource InputFontSizeLG}
{atom:LineEditTokenResource InputFontSizeSM}
```

在 AXAML 中使用 TextArea 组件级 Token：

```xml
{atom:TextAreaTokenResource FontSize}
{atom:TextAreaTokenResource FontSizeLG}
{atom:TextAreaTokenResource ResizeHandleSize}
```

在 AXAML 中使用全局共享 Token：

```xml
{atom:SharedTokenResource ColorPrimary}
{atom:SharedTokenResource ControlHeight}
{atom:SharedTokenResource BorderRadius}
```

---

## LineEditToken 组件级 Token 一览

`LineEditToken`（Token ID: `"LineEdit"`）定义了 LineEdit / TextBox / SearchEdit 共用的组件级 Token。

### 字体 Token

| Token 名 | 类型 | 来源 | 说明 |
|---|---|---|---|
| `InputFontSize` | `double` | `SharedToken.FontSize` | 中号输入框字体大小 |
| `InputFontSizeLG` | `double` | `SharedToken.FontSizeLG` | 大号输入框字体大小 |
| `InputFontSizeSM` | `double` | `SharedToken.FontSizeSM` | 小号输入框字体大小 |

> `LineEditToken` 支持继承扩展：其构造函数为 `protected`，允许子 Token 类（如 `SearchEditToken`）复用字体计算逻辑。

---

## TextAreaToken 组件级 Token 一览

`TextAreaToken`（Token ID: `"TextArea"`）定义了 TextArea 专用的组件级 Token。

### 字体 Token

| Token 名 | 类型 | 来源 | 说明 |
|---|---|---|---|
| `FontSize` | `double` | `SharedToken.FontSize` | 中号文本域字体大小 |
| `FontSizeLG` | `double` | `SharedToken.FontSizeLG` | 大号文本域字体大小 |
| `FontSizeSM` | `double` | `SharedToken.FontSizeSM` | 小号文本域字体大小 |

### Resize 指示器 Token

| Token 名 | 类型 | 来源 | 说明 |
|---|---|---|---|
| `ResizeIndicatorLineColor` | `Color` | `SharedToken.ColorTextDescription` | 拖拽调整大小指示器线条颜色 |
| `ResizeHandleSize` | `double` | `SharedToken.SizeXS` | 拖拽手柄大小 |

### 内边距 Token

| Token 名 | 类型 | 来源 | 说明 |
|---|---|---|---|
| `RightAddOnPadding` | `Thickness` | `SharedToken.UniformlyPaddingSM - LineWidth` | 中号右侧装饰区域内边距 |
| `RightAddOnPaddingSM` | `Thickness` | `SharedToken.ControlPaddingHorizontalSM - LineWidth` | 小号右侧装饰区域内边距 |
| `RightAddOnPaddingLG` | `Thickness` | `SharedToken.ControlPaddingHorizontal - LineWidth` | 大号右侧装饰区域内边距 |

---

## 主题中使用的全局 SharedToken

除组件级 Token 外，Input 控件族的 ControlTheme 还直接引用了以下全局 `SharedToken`：

### 尺寸与布局

| Token 资源键 | 使用场景 |
|---|---|
| `ControlHeight` / `ControlHeightSM` / `ControlHeightLG` | 输入框高度（中/小/大） |
| `BorderRadius` / `BorderRadiusSM` / `BorderRadiusLG` | 圆角半径（中/小/大） |
| `FontHeight` / `FontHeightSM` / `FontHeightLG` | 行高（中/小/大） |
| `BorderThickness` | 默认边框厚度 |
| `LineWidth` | 线宽（用于计算内边距偏移） |
| `UniformlyPaddingXXS` | 内部元素间距（清除按钮、密码按钮之间） |
| `UniformlyPaddingSM` | 中号内边距 |
| `ControlPaddingHorizontal` | 控件水平内边距 |
| `ControlPaddingHorizontalSM` | 小号控件水平内边距 |

### 颜色

| Token 资源键 | 使用场景 |
|---|---|
| `ColorPrimary` | 主色调（搜索按钮 Primary 样式、图标高亮） |
| `ColorBorder` | 默认边框颜色 |
| `ColorText` | 默认文本颜色（光标颜色） |
| `ColorTextPlaceholder` | 占位文本颜色、字符计数颜色 |
| `ColorTextDisabled` | 禁用态文本颜色 |
| `ColorTextDescription` | Resize 指示器线条颜色 |
| `ColorErrorText` | 错误状态文本颜色（Borderless / Filled 变体） |
| `ColorWarningText` | 警告状态文本颜色（Borderless / Filled 变体） |
| `SelectionBackground` | 文本选区背景色 |
| `SelectionForeground` | 文本选区前景色 |

### 动画与交互

| Token 资源键 | 使用场景 |
|---|---|
| `EnableMotion` | 全局动画开关（控制 `IsMotionEnabled` 默认值） |

---

## Token 对外观的具体影响

### 尺寸与 Token 映射

| 尺寸 | 高度 | 字号 (LineEdit) | 字号 (TextArea) | 圆角 | 行高 |
|---|---|---|---|---|---|
| `Large` | `ControlHeightLG` | `InputFontSizeLG` | `FontSizeLG` | `BorderRadiusLG` | `FontHeightLG` |
| `Middle` | `ControlHeight` | `InputFontSize` | `FontSize` | `BorderRadius` | `FontHeight` |
| `Small` | `ControlHeightSM` | `InputFontSizeSM` | `FontSizeSM` | `BorderRadiusSM` | `FontHeightSM` |

### 样式变体与 Token 映射

样式变体的颜色映射主要由内部 `AddOnDecoratedBox` 控件的主题控制，各变体使用以下 SharedToken：

| 样式变体 | 边框颜色 | 背景色 | 焦点态 |
|---|---|---|---|
| **Outline** | `ColorBorder` | `ColorBgContainer` | 主色调边框 + 外发光 |
| **Filled** | 透明 | `ColorFillTertiary` | 主色调边框 + 外发光 |
| **Borderless** | 透明 | 透明 | 无边框无外发光 |
| **Underlined** | 底部 `ColorBorder` | 透明 | 主色调底部边框 |

### 验证状态与 Token 映射

| 验证状态 | 边框颜色 | 文本颜色（Borderless/Filled） |
|---|---|---|
| **Default** | `ColorBorder` | `ColorText` |
| **Error** | `ColorError` | `ColorErrorText` |
| **Warning** | `ColorWarning` | `ColorWarningText` |

### 禁用态

禁用时：
- 文本颜色变为 `ColorTextDisabled`
- 背景色变为 `ColorBgContainerDisabled`（Outline 变体）
- 占位文本颜色保持 `ColorTextPlaceholder`

### TextArea 特有 Token 影响

- `ResizeIndicatorLineColor`：控制右下角拖拽手柄的两条指示线颜色
- `ResizeHandleSize`：控制拖拽手柄的热区大小
- `RightAddOnPadding` / `RightAddOnPaddingSM` / `RightAddOnPaddingLG`：控制清除按钮、反馈图标等右侧装饰元素与文本域边缘的距离
