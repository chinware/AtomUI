# SplitButton Design Token

SplitButton 复用 `ButtonToken`（Token ID: `"Button"`）作为组件级 Design Token 作用域。SplitButton 在构造函数中通过 `this.RegisterTokenResourceScope(ButtonToken.ScopeProvider)` 注册到 Button 的 Token 作用域，因此内部的两个按钮（PrimaryButton 和 SecondaryButton）均为 AtomUI Button 实例，所有按钮相关的颜色、间距、字体 Token 均与 Button 共享。

---

## Token 资源访问方式

在 AXAML 中使用组件级 Token（来自 ButtonToken）：

```xml
{atom:ButtonTokenResource PrimaryColor}
{atom:ButtonTokenResource DefaultBg}
{atom:ButtonTokenResource Padding}
```

在 AXAML 中使用全局共享 Token：

```xml
{atom:SharedTokenResource ColorPrimary}
{atom:SharedTokenResource ColorBorder}
{atom:SharedTokenResource ControlHeight}
```

---

## SplitButton 使用的关键 Token

SplitButton 主要消费 ButtonToken 和 SharedToken 两个层级的 Token。

### 来自 ButtonToken（组件级）

由于 SplitButton 注册了 `ButtonToken.ScopeProvider`，内部的两个 Button 可以访问完整的 Button 组件级 Token。以下列出 SplitButton 场景下最常用的 Token（完整列表请参见 [ButtonToken.md](../Button/ButtonToken.md)）：

#### 颜色 Token

| Token 名 | 类型 | 来源 | 说明 |
|---|---|---|---|
| `PrimaryColor` | `Color` | `SharedToken.ColorTextLightSolid` | Primary 类型按钮文本颜色（通常为白色） |
| `DefaultColor` | `Color` | `SharedToken.ColorText` | Default 类型按钮文本颜色 |
| `DefaultBg` | `Color` | `SharedToken.ColorBgContainer` | Default 类型按钮背景色 |
| `DefaultBorderColor` | `Color` | `SharedToken.ColorBorder` | Default 类型按钮边框颜色 |
| `DangerColor` | `Color` | `SharedToken.ColorTextLightSolid` | 危险按钮文本颜色 |
| `BorderColorDisabled` | `Color` | `SharedToken.ColorBorder` | 禁用态边框颜色 |

#### 悬浮/激活态颜色 Token

| Token 名 | 类型 | 来源 | 说明 |
|---|---|---|---|
| `DefaultHoverBg` | `Color` | `SharedToken.ColorBgContainer` | Default 按钮悬浮背景色 |
| `DefaultHoverColor` | `Color` | `SharedToken.ColorPrimaryHover` | Default 按钮悬浮文本颜色 |
| `DefaultHoverBorderColor` | `Color` | `SharedToken.ColorPrimaryHover` | Default 按钮悬浮边框颜色 |
| `DefaultActiveBg` | `Color` | `SharedToken.ColorBgContainer` | Default 按钮激活背景色 |
| `DefaultActiveColor` | `Color` | `SharedToken.ColorPrimaryActive` | Default 按钮激活文本颜色 |
| `DefaultActiveBorderColor` | `Color` | `SharedToken.ColorPrimaryActive` | Default 按钮激活边框颜色 |

#### 阴影 Token

| Token 名 | 类型 | 来源 | 说明 |
|---|---|---|---|
| `DefaultShadow` | `BoxShadows` | 基于 `SharedToken.ColorControlOutline` | Default 按钮底部微阴影 |
| `PrimaryShadow` | `BoxShadows` | 基于 `SharedToken.ColorControlOutline` | Primary 按钮底部微阴影 |
| `DangerShadow` | `BoxShadows` | 基于 `SharedToken.ColorErrorOutline` | 危险按钮底部微阴影 |

#### 内间距 Token

| Token 名 | 类型 | 来源 | 说明 |
|---|---|---|---|
| `Padding` | `Thickness` | 基于 `SharedToken.PaddingContentHorizontal`、`SharedToken.ControlHeight` | 中号按钮内间距 |
| `PaddingLG` | `Thickness` | 基于 `SharedToken.PaddingContentHorizontal`、`SharedToken.ControlHeightLG` | 大号按钮内间距 |
| `PaddingSM` | `Thickness` | 基于 `SharedToken.ControlHeightSM` | 小号按钮内间距 |

#### 图标 Token

| Token 名 | 类型 | 来源 | 说明 |
|---|---|---|---|
| `IconSize` | `double` | `SharedToken.IconSize` | 中号按钮图标尺寸 |
| `IconSizeLG` | `double` | `SharedToken.IconSize` | 大号按钮图标尺寸 |
| `IconSizeSM` | `double` | `SharedToken.IconSizeSM` | 小号按钮图标尺寸 |
| `IconMargin` | `Thickness` | `SharedToken.UniformlyPaddingXXS` | 图标与文本之间的间距 |
| `OnlyIconSize` | `double` | `SharedToken.IconSizeLG` | 仅图标中号按钮图标尺寸（SecondaryButton 使用） |
| `OnlyIconSizeLG` | `double` | `SharedToken.IconSizeLG` | 仅图标大号按钮图标尺寸 |
| `OnlyIconSizeSM` | `double` | `SharedToken.IconSize` | 仅图标小号按钮图标尺寸 |

#### 字体 Token

| Token 名 | 类型 | 来源 | 说明 |
|---|---|---|---|
| `ContentFontSize` | `double` | `SharedToken.FontSize` | 中号按钮字体大小 |
| `ContentFontSizeLG` | `double` | `SharedToken.FontSizeLG` | 大号按钮字体大小 |
| `ContentFontSizeSM` | `double` | `SharedToken.FontSize` | 小号按钮字体大小 |
| `ContentLineHeight` | `double` | 基于 `ContentFontSize` 计算 | 中号按钮行高 |
| `ContentLineHeightLG` | `double` | 基于 `ContentFontSizeLG` 计算 | 大号按钮行高 |
| `ContentLineHeightSM` | `double` | 基于 `ContentFontSizeSM` 计算 | 小号按钮行高 |

#### 其他 Token

| Token 名 | 类型 | 来源 | 说明 |
|---|---|---|---|
| `GutterToFlyout` | `double` | `SharedToken.UniformlyMarginXXS` | 下拉菜单与按钮之间的间距 |

### 来自 SharedToken（在 SplitButton 主题中直接引用）

除组件级 Token 外，SplitButton 的 ControlTheme 还直接引用了以下全局 `SharedToken`：

| Token 资源键 | 使用场景 |
|---|---|
| `ColorBorder` | SplitButton 的 `BorderBrush` 和 `SplitSeparatorBrush`（分隔线颜色） |
| `BorderThickness` | SplitButton 的 `BorderThickness`（边框粗细） |
| `BorderRadius` | 中号 SplitButton 的 `CornerRadius` |
| `BorderRadiusSM` | 小号 SplitButton 的 `CornerRadius` |
| `BorderRadiusLG` | 大号 SplitButton 的 `CornerRadius` |
| `EnableMotion` | 全局动画开关，控制 `IsMotionEnabled` 默认值 |
| `EnableWaveSpirit` | 全局波纹开关，控制 `IsWaveSpiritEnabled` 默认值 |
| `ControlHeight` / `ControlHeightSM` / `ControlHeightLG` | 内部 Button 高度（中/小/大） |
| `ColorPrimary` / `ColorPrimaryHover` / `ColorPrimaryActive` | Primary 按钮主色调及其交互态 |
| `ColorError` / `ColorErrorHover` / `ColorErrorActive` | 错误/危险色系 |
| `ColorText` / `ColorTextDisabled` | 文本色系 |
| `ColorBgContainer` / `ColorBgContainerDisabled` | 容器背景色系 |

---

## 分隔线渲染

SplitButton 在 `IsPrimaryButtonType=True` 时，通过 `Render` 方法使用 `SplitSeparatorBrush` 绘制两个按钮之间的分隔线背景。该画刷默认绑定到 `SharedToken.ColorBorder`，确保分隔线颜色跟随主题变化。

在 Default 模式下（`IsPrimaryButtonType=False`），两个按钮的边框在 `ArrangeOverride` 中重叠，自然形成分隔线效果，无需额外绘制。

---

## Token 对外观的具体影响

### 按钮类型与 Token 映射

SplitButton 内部的两个 Button 消费与独立 Button 完全相同的 Token。通过 `EffectiveButtonType` 属性（由 `IsPrimaryButtonType` 驱动），内部 Button 使用 Default 或 Primary 样式：

| 按钮类型 | 正常态 | 悬浮态 | 按下态 | 禁用态 |
|---|---|---|---|---|
| **Default** | `DefaultColor` / `DefaultBg` / `DefaultBorderColor` | `DefaultHoverColor` / `DefaultHoverBorderColor` | `DefaultActiveColor` / `DefaultActiveBorderColor` | `ColorTextDisabled` / `ColorBgContainerDisabled` |
| **Primary** | `PrimaryColor` / `ColorPrimary` | - / `ColorPrimaryHover` | - / `ColorPrimaryActive` | `ColorTextDisabled` / `ColorBgContainerDisabled` |

### 危险态下的 Token 覆盖

当 `IsDanger=True` 时，上述色系会被替换为 Error 色系（`ColorError` / `ColorErrorHover` / `ColorErrorActive`），阴影替换为 `DangerShadow`。

### 尺寸与 Token 映射

SplitButton 的尺寸由 `SizeType` 控制。圆角由 SplitButton 自身的 ControlTheme 设置（然后分配给左右内部 Button），按钮内部的高度/间距/字号通过内部 Button 消费 ButtonToken：

| 尺寸 | 外部圆角 | 内部按钮高度 | 内部按钮间距 | 内部按钮字号 | 内部按钮图标 |
|---|---|---|---|---|---|
| `Large` | `BorderRadiusLG` | `ControlHeightLG` | `PaddingLG` | `ContentFontSizeLG` | `IconSizeLG` |
| `Middle` | `BorderRadius` | `ControlHeight` | `Padding` | `ContentFontSize` | `IconSize` |
| `Small` | `BorderRadiusSM` | `ControlHeightSM` | `PaddingSM` | `ContentFontSizeSM` | `IconSizeSM` |
