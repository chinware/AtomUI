# Message Design Token

Message 控件使用 `MessageToken`（Token ID: `"Message"`）作为组件级 Design Token。所有视觉属性均从全局 `SharedToken` 派生，不使用硬编码值。

> 📖 Token 源码位置：`src/AtomUI.Desktop.Controls/Message/MessageToken.cs`

---

## Token 资源访问方式

### AXAML 中访问

```xml
<!-- 组件级 Token（Message 专属） -->
{atom:MessageTokenResource ContentBg}
{atom:MessageTokenResource ContentPadding}
{atom:MessageTokenResource MessageIconSize}
{atom:MessageTokenResource MessageIconMargin}
{atom:MessageTokenResource MessageTopMargin}

<!-- 全局共享 Token -->
{atom:SharedTokenResource ColorPrimary}
{atom:SharedTokenResource ColorSuccess}
{atom:SharedTokenResource ColorWarning}
{atom:SharedTokenResource ColorError}
```

---

## Token 类定义

```csharp
[ControlDesignToken]
internal class MessageToken : AbstractControlDesignToken
{
    public const string ID = "Message";
    public static readonly ControlTokenResourceScopeProvider ScopeProvider = new(ID);
    // ...
}
```

---

## 完整 Token 属性列表

### 背景与容器

| Token 属性 | 类型 | 派生自 SharedToken | 说明 |
|---|---|---|---|
| `ContentBg` | `Color` | `ColorBgElevated` | 消息卡片背景色 |
| `ContentPadding` | `Thickness` | `ControlHeightLG`、`FontSize`、`RelativeLineHeight`、`UniformlyPaddingXS` | 消息卡片内边距 |

> `ContentPadding` 的计算公式：垂直方向 = `(ControlHeightLG - FontSize × RelativeLineHeight) / 2`；水平方向 = `UniformlyPaddingXS`。

### 图标

| Token 属性 | 类型 | 派生自 SharedToken | 说明 |
|---|---|---|---|
| `MessageIconSize` | `double` | `FontSizeSM × RelativeLineHeightSM` | 消息图标尺寸 |
| `MessageIconMargin` | `Thickness` | `UniformlyMarginXS` | 消息图标右侧外边距（`0, 0, MarginXS, 0`） |

### 间距

| Token 属性 | 类型 | 派生自 SharedToken | 说明 |
|---|---|---|---|
| `MessageTopMargin` | `Thickness` | `UniformlyMargin` | 消息卡片外边距（上、左、右各 `UniformlyMargin`，下为 0） |

---

## 主题中使用的全局 SharedToken

除组件级 Token 外，MessageCard 的 ControlTheme 还直接引用了以下全局 `SharedToken`：

| Token 资源键 | 使用场景 |
|---|---|
| `EnableMotion` | 全局动画开关，控制消息入场/出场动画 |
| `MotionDurationMid` | 入场/出场动画时长 |
| `BoxShadows` | 消息卡片阴影效果 |
| `BorderRadiusLG` | 消息卡片大圆角 |
| `FontSize` | 消息文本字体大小 |
| `FontHeight` | 消息文本行高 |
| `ColorText` | 消息文本颜色 |
| `SelectionBackground` | 文本选中背景色 |
| `SelectionForeground` | 文本选中前景色 |
| `ColorPrimary` | Information / Loading 消息图标颜色（蓝色） |
| `ColorSuccess` | Success 消息图标颜色（绿色） |
| `ColorWarning` | Warning 消息图标颜色（黄色） |
| `ColorError` | Error 消息图标颜色（红色） |

---

## Token 在主题中的使用示例

### MessageCardTheme.axaml 中的 Token 引用

```xml
<!-- 消息卡片主框架 -->
<Style Selector="^ /template/ Border#PART_Frame">
    <Setter Property="Margin" Value="{atom:MessageTokenResource MessageTopMargin}" />
    <Setter Property="Padding" Value="{atom:MessageTokenResource ContentPadding}" />
    <Setter Property="BoxShadow" Value="{atom:SharedTokenResource BoxShadows}" />
    <Setter Property="Background" Value="{atom:MessageTokenResource ContentBg}" />
    <Setter Property="CornerRadius" Value="{atom:SharedTokenResource BorderRadiusLG}" />
</Style>

<!-- 图标尺寸 -->
<Style Selector="^ /template/ atom|IconPresenter">
    <Setter Property="Width" Value="{atom:MessageTokenResource MessageIconSize}" />
    <Setter Property="Height" Value="{atom:MessageTokenResource MessageIconSize}" />
</Style>

<!-- 图标外边距 -->
<Style Selector="^ /template/ atom|IconPresenter#PART_IconContent">
    <Setter Property="Margin" Value="{atom:MessageTokenResource MessageIconMargin}" />
</Style>

<!-- 消息文本 -->
<Style Selector="^ /template/ SelectableTextBlock#PART_Message">
    <Setter Property="LineHeight" Value="{atom:SharedTokenResource FontHeight}" />
    <Setter Property="FontSize" Value="{atom:SharedTokenResource FontSize}" />
    <Setter Property="Foreground" Value="{atom:SharedTokenResource ColorText}" />
    <Setter Property="SelectionBrush" Value="{atom:SharedTokenResource SelectionBackground}" />
    <Setter Property="SelectionForegroundBrush" Value="{atom:SharedTokenResource SelectionForeground}" />
</Style>

<!-- 消息类型图标颜色 -->
<Style Selector="^[MessageType=Information] /template/ atom|IconPresenter">
    <Setter Property="IconBrush" Value="{atom:SharedTokenResource ColorPrimary}" />
</Style>
<Style Selector="^[MessageType=Success] /template/ atom|IconPresenter">
    <Setter Property="IconBrush" Value="{atom:SharedTokenResource ColorSuccess}" />
</Style>
<Style Selector="^[MessageType=Warning] /template/ atom|IconPresenter">
    <Setter Property="IconBrush" Value="{atom:SharedTokenResource ColorWarning}" />
</Style>
<Style Selector="^[MessageType=Error] /template/ atom|IconPresenter">
    <Setter Property="IconBrush" Value="{atom:SharedTokenResource ColorError}" />
</Style>
<Style Selector="^[MessageType=Loading] /template/ atom|IconPresenter">
    <Setter Property="IconBrush" Value="{atom:SharedTokenResource ColorPrimary}" />
</Style>
```

### WindowMessageManagerTheme.axaml 中的 Token 引用

```xml
<!-- 动画开关 -->
<Setter Property="IsMotionEnabled" Value="{atom:SharedTokenResource EnableMotion}" />

<!-- 顶部居中定位 -->
<Style Selector="^:topcenter /template/ ReversibleStackPanel#PART_Items">
    <Setter Property="ReverseOrder" Value="True" />
    <Setter Property="HorizontalAlignment" Value="Center" />
    <Setter Property="VerticalAlignment" Value="Top" />
</Style>
```

---

## Token 对外观的具体影响

### 消息类型与颜色 Token 映射

| 消息类型 | 图标颜色 Token | 默认颜色 |
|---|---|---|
| `Information` | `SharedToken.ColorPrimary` | 蓝色（`#1677ff`） |
| `Success` | `SharedToken.ColorSuccess` | 绿色（`#52c41a`） |
| `Warning` | `SharedToken.ColorWarning` | 黄色（`#faad14`） |
| `Error` | `SharedToken.ColorError` | 红色（`#ff4d4f`） |
| `Loading` | `SharedToken.ColorPrimary` | 蓝色（`#1677ff`） |

### 视觉布局 Token 映射

| 视觉属性 | Token | 说明 |
|---|---|---|
| 卡片背景 | `ContentBg` ← `ColorBgElevated` | 浮层背景色，浅色模式下为白色 |
| 卡片阴影 | `SharedToken.BoxShadows` | 全局标准阴影 |
| 卡片圆角 | `SharedToken.BorderRadiusLG` | 大号圆角（8px） |
| 卡片内边距 | `ContentPadding` | 垂直方向根据控件高度和字号自适应 |
| 卡片外边距 | `MessageTopMargin` ← `UniformlyMargin` | 消息之间的堆叠间距 |
| 图标尺寸 | `MessageIconSize` ← `FontSizeSM × RelativeLineHeightSM` | 与小号字体行高对齐 |
| 图标间距 | `MessageIconMargin` ← `UniformlyMarginXS` | 图标与文本的间隔 |
| 文本字号 | `SharedToken.FontSize` | 全局默认字号（14px） |
| 文本行高 | `SharedToken.FontHeight` | 全局默认行高 |
| 文本颜色 | `SharedToken.ColorText` | 全局文本颜色 |
| 动画时长 | `SharedToken.MotionDurationMid` | 中等动画时长 |

---

## Token 派生链总结

```
SharedToken (DesignToken)
├── ColorBgElevated       → ContentBg（卡片背景色）
├── ControlHeightLG       → ContentPadding（垂直内边距计算）
├── FontSize              → ContentPadding（垂直内边距计算）
├── RelativeLineHeight    → ContentPadding（垂直内边距计算）
├── UniformlyPaddingXS    → ContentPadding（水平内边距）
├── FontSizeSM            → MessageIconSize（图标尺寸）
├── RelativeLineHeightSM  → MessageIconSize（图标尺寸）
├── UniformlyMarginXS     → MessageIconMargin（图标右侧间距）
├── UniformlyMargin       → MessageTopMargin（卡片外边距）
├── BoxShadows            → 卡片阴影（直接引用）
├── BorderRadiusLG        → 卡片圆角（直接引用）
├── FontHeight            → 文本行高（直接引用）
├── ColorText             → 文本颜色（直接引用）
├── ColorPrimary          → Information / Loading 图标颜色
├── ColorSuccess          → Success 图标颜色
├── ColorWarning          → Warning 图标颜色
├── ColorError            → Error 图标颜色
├── MotionDurationMid     → 入场/出场动画时长
└── EnableMotion          → 动画开关
```
