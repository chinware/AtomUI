# Expander Design Token

Expander 使用 `ExpanderToken`（Token ID: `"Expander"`）作为组件级 Design Token。所有 Token 值均从全局 `SharedToken` 派生，不硬编码任何具体值。

---

## Token 资源访问方式

在 AXAML 中使用组件级 Token：

```xml
{atom:ExpanderTokenResource HeaderPadding}
{atom:ExpanderTokenResource ContentPadding}
{atom:ExpanderTokenResource HeaderBg}
{atom:ExpanderTokenResource ContentBg}
```

在 AXAML 中使用全局共享 Token：

```xml
{atom:SharedTokenResource ColorBorder}
{atom:SharedTokenResource ColorFillAlter}
{atom:SharedTokenResource ColorTextHeading}
```

---

## 组件级 Token 一览

以下是 `ExpanderToken` 定义的全部组件级 Token。

### 头部间距 Token

| Token 名 | 类型 | 来源 | 说明 |
|---|---|---|---|
| `HeaderPadding` | `Thickness` | `(UniformlyPadding, UniformlyPaddingSM)` | Middle 尺寸头部内边距 |
| `HeaderPaddingSM` | `Thickness` | `(UniformlyPaddingSM, UniformlyPaddingXS)` | Small 尺寸头部内边距 |
| `HeaderPaddingLG` | `Thickness` | `(UniformlyPaddingLG, UniformlyPadding)` | Large 尺寸头部内边距 |

### 内容间距 Token

| Token 名 | 类型 | 来源 | 说明 |
|---|---|---|---|
| `ContentPadding` | `Thickness` | `(16, UniformlyPadding)` | Middle 尺寸内容内边距 |
| `ContentPaddingSM` | `Thickness` | `SharedToken.PaddingSM` | Small 尺寸内容内边距 |
| `ContentPaddingLG` | `Thickness` | `SharedToken.PaddingLG` | Large 尺寸内容内边距 |

### 背景色 Token

| Token 名 | 类型 | 来源 | 说明 |
|---|---|---|---|
| `HeaderBg` | `Color` | `SharedToken.ColorFillAlter` | 头部背景色（淡灰色） |
| `ContentBg` | `Color` | `SharedToken.ColorBgContainer` | 内容区域背景色（白色） |

### 布局 Token

| Token 名 | 类型 | 来源 | 说明 |
|---|---|---|---|
| `ExpanderBorderRadius` | `CornerRadius` | `SharedToken.BorderRadiusLG` | 外层边框圆角 |
| `LeftExpandButtonHMargin` | `Thickness` | `(0, 0, UniformlyMarginSM, 0)` | 图标在左侧时的右边距（水平方向） |
| `RightExpandButtonHMargin` | `Thickness` | `(UniformlyMarginSM, 0, 0, 0)` | 图标在右侧时的左边距（水平方向） |
| `LeftExpandButtonVMargin` | `Thickness` | `(0, 0, 0, UniformlyMarginSM)` | 图标在左侧时的下边距（垂直方向） |
| `RightExpandButtonVMargin` | `Thickness` | `(0, UniformlyMarginSM, 0, 0)` | 图标在右侧时的上边距（垂直方向） |

---

## 主题中使用的全局 SharedToken

除组件级 Token 外，Expander 的 ControlTheme 还直接引用了以下全局 `SharedToken`：

### 颜色 Token

| Token 资源键 | 使用场景 |
|---|---|
| `ColorBorder` | 外层边框颜色、展开时头部分隔线颜色 |
| `ColorFillAlter` | 头部默认背景色（通过 `HeaderBg` Token 间接引用） |
| `ColorBgContainer` | 内容区域背景色（通过 `ContentBg` Token 间接引用） |
| `ColorTextHeading` | 头部标题文字颜色 |
| `ColorTextDisabled` | 禁用状态文字颜色 |

### 字号 Token

| Token 资源键 | 使用场景 |
|---|---|
| `FontSizeLG` | Large 尺寸头部字号 |
| `FontSize` | Middle / Small 尺寸头部字号 |
| `FontHeightLG` | Large 尺寸行高 |
| `FontHeight` | Middle / Small 尺寸行高 |
| `IconSizeSM` | 展开图标尺寸 |

### 其他 Token

| Token 资源键 | 使用场景 |
|---|---|
| `BorderThickness` | 外层边框粗细 |
| `MotionDurationSlow` | 展开/收起动画时长 |
| `EnableMotion` | 全局动画开关 |

---

## Token 对外观的具体影响

### 尺寸与 Token 映射

| 尺寸 | 头部间距 | 内容间距 | 字号 / 行高 |
|---|---|---|---|
| `Large` | `HeaderPaddingLG` | `ContentPaddingLG` | `FontSizeLG` / `FontHeightLG` |
| `Middle` | `HeaderPadding` | `ContentPadding` | `FontSize` / `FontHeight` |
| `Small` | `HeaderPaddingSM` | `ContentPaddingSM` | `FontSize` / `FontHeight` |

### 风格与 Token 映射

| 风格 | 外边框 | 头部背景 | 内容背景 | 头部分隔线 |
|---|---|---|---|---|
| 默认 | `BorderThickness` + `ColorBorder` | `HeaderBg` | `ContentBg` | `ColorBorder` |
| 无边框 | 无 | `HeaderBg` | `HeaderBg`（同头部色） | 无 |
| 幽灵 | 无 | `ContentBg`（透明效果） | `ContentBg` | 无 |

### 自定义间距优先级

当用户设置 `HeaderPadding` 或 `ContentPadding` 属性时，对应的伬类（`:custom-header-padding` / `:custom-content-padding`）会被激活，此时用户指定的值优先于 Token 控制的尺寸映射。
