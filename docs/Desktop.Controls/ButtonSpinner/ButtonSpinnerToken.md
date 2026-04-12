# ButtonSpinner Design Token

ButtonSpinner 使用 `ButtonSpinnerToken`（Token ID: `"ButtonSpinner"`）作为组件级 Design Token。`ButtonSpinnerToken` 继承自 `LineEditToken`，后者又继承自 `AbstractControlDesignToken`，因此 ButtonSpinner 可以复用所有输入控件的基础 Token 值。

---

## Token 继承关系

```
AbstractControlDesignToken (SharedToken 访问)
└── LineEditToken (Token ID: "LineEdit")
    ├── InputFontSize     → SharedToken.FontSize
    ├── InputFontSizeLG   → SharedToken.FontSizeLG
    └── InputFontSizeSM   → SharedToken.FontSizeSM
        └── ButtonSpinnerToken (Token ID: "ButtonSpinner")
            ├── ControlWidth, HandleWidth, HandleIconSize
            ├── HandleBg, HandleActiveBg, HandleHoverColor
            ├── HandleBorderColor, FilledHandleBg
            └── 继承所有 LineEditToken 属性
```

---

## Token 资源访问方式

在 AXAML 中使用组件级 Token：

```xml
{atom:ButtonSpinnerTokenResource HandleWidth}
{atom:ButtonSpinnerTokenResource HandleIconSize}
{atom:ButtonSpinnerTokenResource HandleHoverColor}
```

在 AXAML 中使用全局共享 Token：

```xml
{atom:SharedTokenResource ColorBorder}
{atom:SharedTokenResource ColorBgContainer}
{atom:SharedTokenResource ColorPrimary}
{atom:SharedTokenResource BorderRadius}
```

---

## 组件级 Token 一览

以下是 `ButtonSpinnerToken` 自身定义的 Token（不含继承自 `LineEditToken` 的部分）。

### 尺寸 Token

| Token 名 | 类型 | 来源 | 说明 |
|---|---|---|---|
| `ControlWidth` | `double` | 固定 `90` | 默认控件宽度 |
| `HandleWidth` | `double` | `SharedToken.ControlHeightSM` | 操作按钮区域宽度 |
| `HandleIconSize` | `double` | `SharedToken.FontSize / 2` | 操作按钮图标（↑↓）大小 |

### 颜色 Token

| Token 名 | 类型 | 来源 | 说明 |
|---|---|---|---|
| `HandleBg` | `Color` | `SharedToken.ColorBgContainer` | 操作按钮默认背景色 |
| `HandleActiveBg` | `Color` | `SharedToken.ColorFillAlter` | 操作按钮按下态背景色 |
| `HandleHoverColor` | `Color` | `SharedToken.ColorPrimary` | 操作按钮悬浮态图标颜色 |
| `HandleBorderColor` | `Color` | `SharedToken.ColorBorder` | 操作按钮边框颜色 |
| `FilledHandleBg` | `Color` | `ColorUtils.OnBackground(SharedToken.ColorFillSecondary, HandleBg)` | Filled 变体下操作按钮背景色 |

### 继承自 LineEditToken 的 Token

| Token 名 | 类型 | 来源 | 说明 |
|---|---|---|---|
| `InputFontSize` | `double` | `SharedToken.FontSize` | 默认字体大小 |
| `InputFontSizeLG` | `double` | `SharedToken.FontSizeLG` | 大号字体大小 |
| `InputFontSizeSM` | `double` | `SharedToken.FontSizeSM` | 小号字体大小 |

---

## 主题中使用的全局 SharedToken

除组件级 Token 外，ButtonSpinner 的 ControlTheme 还直接引用了以下全局 `SharedToken`：

### ButtonSpinnerTheme 使用的 SharedToken

| Token 资源键 | 使用场景 |
|---|---|
| `EnableMotion` | 控制是否启用动画过渡 |
| `BorderRadius` | Middle 尺寸圆角 |
| `BorderRadiusSM` | Small 尺寸圆角 |
| `BorderRadiusLG` | Large 尺寸圆角 |

### ButtonSpinnerHandleTheme 使用的 SharedToken

| Token 资源键 | 使用场景 |
|---|---|
| `ColorBorder` | 操作按钮容器边框颜色 |
| `BorderThickness` | 操作按钮容器边框粗细 |
| `ColorBgContainer` | 操作按钮容器背景色 |
| `ColorPrimaryActive` | 操作按钮按下态图标颜色 |

---

## Token 对外观的具体影响

### 操作按钮区域

| 视觉元素 | 默认态 Token | 悬浮态 Token | 按下态 Token |
|---|---|---|---|
| 背景色 | `HandleBg` | — | `HandleActiveBg` |
| 图标颜色 | 继承主题默认色 | `HandleHoverColor`（主色） | `ColorPrimaryActive`（主色激活态） |
| 边框颜色 | `HandleBorderColor` | — | — |
| 图标大小 | `HandleIconSize` | — | — |
| 区域宽度 | `HandleWidth` | — | — |

### 整体控件

| 视觉元素 | Token | 说明 |
|---|---|---|
| Large 尺寸圆角 | `SharedToken.BorderRadiusLG` | `SizeType="Large"` 时的圆角 |
| Middle 尺寸圆角 | `SharedToken.BorderRadius` | `SizeType="Middle"` 时的圆角 |
| Small 尺寸圆角 | `SharedToken.BorderRadiusSM` | `SizeType="Small"` 时的圆角 |
| 浮动动画时长 | `SharedToken.MotionDurationMid` | 操作按钮滑入/滑出的动画时长 |

### Filled 变体

| 视觉元素 | Token | 说明 |
|---|---|---|
| 操作按钮背景色 | `FilledHandleBg` | Filled 变体下操作按钮背景区分于默认 `HandleBg` |

### 注意事项

- `ButtonSpinnerToken` 继承自 `LineEditToken`，因此当修改 `LineEditToken` 的 Token 值时，ButtonSpinner 也会受到影响。
- 操作按钮的悬浮/按下态使用主题主色（`ColorPrimary` / `ColorPrimaryActive`），确保与 AtomUI 整体主题一致。
- `ControlWidth` 固定为 `90`，是少数未从 `SharedToken` 派生的 Token 值之一——这是因为 ButtonSpinner 通常需要精确的默认宽度。
