# Tooltip Design Token

ToolTip 控件使用 `ToolTipToken`（Token ID: `"ToolTip"`）作为组件级 Design Token，所有视觉属性均从全局 `SharedToken` 派生，不硬编码任何具体值。

---

## Token 资源访问方式

在 AXAML 中使用组件级 Token：

```xml
{atom:ToolTipTokenResource ToolTipBackground}
{atom:ToolTipTokenResource ToolTipColor}
{atom:ToolTipTokenResource ToolTipMaxWidth}
{atom:ToolTipTokenResource ToolTipPadding}
```

在 AXAML 中使用全局共享 Token：

```xml
{atom:SharedTokenResource ColorBgSpotlight}
{atom:SharedTokenResource ColorTextLightSolid}
{atom:SharedTokenResource FontSize}
{atom:SharedTokenResource ControlHeight}
{atom:SharedTokenResource EnableMotion}
```

---

## 组件级 Token 一览

以下是 `ToolTipToken` 定义的全部组件级 Token，按功能分组说明。

### 尺寸与布局 Token

| Token 名 | 类型 | 来源 | 说明 |
|---|---|---|---|
| `ToolTipMaxWidth` | `double` | 固定值 `250` | ToolTip 的最大宽度（像素），超过此宽度文本自动换行 |
| `ToolTipPadding` | `Thickness` | 基于 `SharedToken.UniformlyPaddingSM` 计算 | ToolTip 内容区域的内间距。水平 = `UniformlyPaddingSM`，垂直 = `UniformlyPaddingSM / 2 + 2` |

### 颜色 Token

| Token 名 | 类型 | 来源 | 说明 |
|---|---|---|---|
| `ToolTipColor` | `Color` | `SharedToken.ColorTextLightSolid` | ToolTip 默认前景色（文字颜色），通常为白色 |
| `ToolTipBackground` | `Color` | `SharedToken.ColorBgSpotlight` | ToolTip 默认背景色，通常为深灰/黑色半透明 |

### 圆角 Token

| Token 名 | 类型 | 来源 | 说明 |
|---|---|---|---|
| `BorderRadiusOuter` | `CornerRadius` | 基于 `SharedToken.BorderRadiusSM` 计算，最小值 `4` | ToolTip 气泡框的外圆角。每个角的值取 `max(原值, 4)`，确保始终有足够的圆角 |

### 阴影 Token

| Token 名 | 类型 | 来源 | 说明 |
|---|---|---|---|
| `ToolTipShadows` | `BoxShadows` | `SharedToken.BoxShadowsSecondary` | ToolTip 气泡框的内置阴影效果，使用全局二级阴影 |

### 动画 Token

| Token 名 | 类型 | 来源 | 说明 |
|---|---|---|---|
| `ToolTipMotionDuration` | `TimeSpan` | `SharedToken.MotionDurationMid` | ToolTip 弹出/关闭动画的持续时间 |

---

## 主题中使用的全局 SharedToken

除组件级 Token 外，ToolTip 的 ControlTheme 还直接引用了以下全局 `SharedToken`：

| Token 资源键 | 使用位置 | 说明 |
|---|---|---|
| `FontSize` | `ArrowDecoratedBox` 字体 | ToolTip 内容文字的字号 |
| `ControlHeight` | `ArrowDecoratedBox` 最小高度 | ToolTip 气泡框的最小高度 |
| `EnableMotion` | `IsMotionEnabled` 属性 | 全局动画开关，控制 ToolTip 是否使用弹出/关闭动画 |
| `MarginToAnchor` | `MarginToAnchor` 属性 | 全局弹出层边距（从 PopupHostToken 获取） |
| `ColorBgSpotlight` | `ToolTipBackground` 来源 | 深色聚焦背景色，作为 ToolTip 默认背景 |
| `ColorTextLightSolid` | `ToolTipColor` 来源 | 浅色实心文字色，作为 ToolTip 默认前景 |
| `UniformlyPaddingSM` | `ToolTipPadding` 来源 | 统一小号内间距，作为 ToolTip 内间距的计算基准 |
| `BoxShadowsSecondary` | `ToolTipShadows` 来源 | 二级阴影效果 |
| `MotionDurationMid` | `ToolTipMotionDuration` 来源 | 中等动画时长 |

---

## Token 对外观的具体影响

### 颜色映射

| 模式 | 背景色 | 文字色 | 箭头颜色 |
|---|---|---|---|
| **默认** | `ToolTipBackground`（`ColorBgSpotlight`，深灰/黑色） | `ToolTipColor`（`ColorTextLightSolid`，白色） | 与背景色一致 |
| **预设颜色**（`PresetColor`） | 对应调色板主色 | `ToolTipColor`（白色） | 与背景色一致 |
| **自定义颜色**（`Color`） | 用户指定色 | `ToolTipColor`（白色） | 与背景色一致 |

> **说明**：背景色通过 `TemplateBinding Background` 传递给 `ArrowDecoratedBox`，箭头颜色自动跟随背景色变化。当设置 `PresetColor` 或 `Color` 时，ToolTip 会更新 `Background` 属性，覆盖默认的 `ToolTipBackground` Token 值。

### 尺寸映射

| 属性 | Token 来源 | 默认值（近似） | 说明 |
|---|---|---|---|
| 最大宽度 | `ToolTipMaxWidth` | `250px` | 超过此宽度文本自动换行 |
| 最小高度 | `SharedToken.ControlHeight` | `32px` | 确保 ToolTip 不会太扁 |
| 内间距 | `ToolTipPadding` | 约 `8px 6px` | 内容与气泡框边缘的间距 |
| 圆角 | `BorderRadiusOuter` | 至少 `4px` | 气泡框的圆角半径 |

### 阴影映射

| 属性 | Token 来源 | 说明 |
|---|---|---|
| 阴影 | `ToolTipShadows` | 使用 `BoxShadowsSecondary`，提供轻微的阴影效果使 ToolTip 与背景分离 |

### 动画映射

| 属性 | Token 来源 | 说明 |
|---|---|---|
| 弹出动画时长 | `ToolTipMotionDuration` | 使用 `MotionDurationMid`（约 200ms），控制淡入淡出速度 |
| 动画开关 | `SharedToken.EnableMotion` | 全局控制是否使用动画。设为 `false` 时 ToolTip 立即显示/隐藏 |

---

## 深色模式行为

`ToolTipToken.CalculateTokenValues(bool isDarkMode)` 在深色模式和浅色模式下的行为：

| Token | 浅色模式 | 深色模式 |
|---|---|---|
| `ToolTipBackground` | `ColorBgSpotlight`（深色背景） | `ColorBgSpotlight`（深色背景） |
| `ToolTipColor` | `ColorTextLightSolid`（白色文字） | `ColorTextLightSolid`（白色文字） |
| `ToolTipShadows` | `BoxShadowsSecondary` | `BoxShadowsSecondary` |

> **注意**：ToolTip 在浅色和深色模式下的默认外观非常相似（都是深色背景 + 白色文字），这是因为 Ant Design 的设计规范就是如此——ToolTip 始终使用高对比度的深色气泡，以确保在任何主题下都具有良好的可读性。

---

## Token 注册

ToolTip 控件在构造函数中注册 Token 作用域：

```csharp
public ToolTip()
{
    this.RegisterTokenResourceScope(ToolTipToken.ScopeProvider);
}
```

`ToolTipToken` 的 Token 类定义：

```csharp
[ControlDesignToken]
internal class ToolTipToken : AbstractControlDesignToken
{
    public const string ID = "ToolTip";
    public static readonly ControlTokenResourceScopeProvider ScopeProvider = new(ID);

    // Token 属性...

    protected override Type GetTokenKindType() => typeof(ToolTipTokenKind);
}
```
