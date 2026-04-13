# DropdownButton Design Token

DropdownButton 继承自 Button，复用 `ButtonToken`（Token ID: `"Button"`）作为组件级 Design Token 作用域。DropdownButton 自身没有定义独立的 Token 类——所有按钮相关的颜色、间距、字体 Token 均与 Button 共享。

---

## Token 资源访问方式

在 AXAML 中使用组件级 Token（来自 ButtonToken）：

```xml
{atom:ButtonTokenResource PrimaryColor}
{atom:ButtonTokenResource DefaultBg}
{atom:ButtonTokenResource Padding}
{atom:ButtonTokenResource ExtraContentMargin}
```

在 AXAML 中使用全局共享 Token：

```xml
{atom:SharedTokenResource ColorPrimary}
{atom:SharedTokenResource IconSizeSM}
{atom:SharedTokenResource ControlHeight}
```

在 AXAML 中使用弹出层 Token：

```xml
{atom:PopupHostTokenResource MarginToAnchor}
```

---

## DropdownButton 使用的关键 Token

DropdownButton 主要消费 ButtonToken、SharedToken 和 PopupHostToken。

### 来自 ButtonToken

所有按钮外观相关 Token（详见 [ButtonToken.md](../Button/ButtonToken.md)）均适用。以下列出与 DropdownButton 最相关的 Token：

#### 颜色 Token

| Token 名 | 类型 | 来源 | 说明 |
|---|---|---|---|
| `PrimaryColor` | `Color` | `SharedToken.ColorTextLightSolid` | Primary 类型按钮文本颜色（通常为白色） |
| `DefaultColor` | `Color` | `SharedToken.ColorText` | Default 类型按钮文本颜色 |
| `DefaultBg` | `Color` | `SharedToken.ColorBgContainer` | Default 类型按钮背景色 |
| `DefaultBorderColor` | `Color` | `SharedToken.ColorBorder` | Default 类型按钮边框颜色 |
| `DefaultHoverColor` | `Color` | `SharedToken.ColorPrimaryHover` | Default 按钮悬浮态文本颜色 |
| `DefaultHoverBorderColor` | `Color` | `SharedToken.ColorPrimaryHover` | Default 按钮悬浮态边框颜色 |
| `DefaultActiveColor` | `Color` | `SharedToken.ColorPrimaryActive` | Default 按钮激活态文本颜色 |
| `DefaultActiveBorderColor` | `Color` | `SharedToken.ColorPrimaryActive` | Default 按钮激活态边框颜色 |
| `LinkHoverBg` | `Color` | `Colors.Transparent` | Link 按钮悬浮态背景色 |
| `TextHoverBg` | `Color` | `SharedToken.ColorBgTextHover` | Text 按钮悬浮态背景色 |

#### 阴影 Token

| Token 名 | 类型 | 来源 | 说明 |
|---|---|---|---|
| `DefaultShadow` | `BoxShadows` | 基于 `SharedToken.ColorControlOutline` | Default 按钮底部微阴影 |
| `PrimaryShadow` | `BoxShadows` | 基于 `SharedToken.ColorControlOutline` | Primary 按钮底部微阴影 |
| `DangerShadow` | `BoxShadows` | 基于 `SharedToken.ColorErrorOutline` | 危险按钮阴影 |

#### 内间距 Token

| Token 名 | 类型 | 来源 | 说明 |
|---|---|---|---|
| `Padding` | `Thickness` | 基于 `SharedToken.PaddingContentHorizontal`、`SharedToken.ControlHeight` | 中号按钮内间距 |
| `PaddingLG` | `Thickness` | 基于 `SharedToken.PaddingContentHorizontal`、`SharedToken.ControlHeightLG` | 大号按钮内间距 |
| `PaddingSM` | `Thickness` | 基于 `SharedToken.ControlHeightSM` | 小号按钮内间距 |

#### 额外内容区域 Token（DropdownButton 下拉指示器使用）

| Token 名 | 类型 | 来源 | 说明 |
|---|---|---|---|
| `ExtraContentMargin` | `Thickness` | `Padding.Left / 2` | 中号下拉指示器左侧外边距 |
| `ExtraContentMarginLG` | `Thickness` | `PaddingLG.Left / 2` | 大号下拉指示器左侧外边距 |
| `ExtraContentMarginSM` | `Thickness` | `PaddingSM.Left / 2` | 小号下拉指示器左侧外边距 |

#### 字体 Token

| Token 名 | 类型 | 来源 | 说明 |
|---|---|---|---|
| `ContentFontSize` | `double` | `SharedToken.FontSize` | 中号按钮字体大小 |
| `ContentFontSizeLG` | `double` | `SharedToken.FontSizeLG` | 大号按钮字体大小 |
| `ContentFontSizeSM` | `double` | `SharedToken.FontSize` | 小号按钮字体大小 |
| `FontWeight` | `double` | 固定 `400` | 按钮文字字重 |

### 来自 SharedToken

| Token 资源键 | 使用场景 |
|---|---|
| `IconSizeSM` | 下拉指示器图标的宽高尺寸 |
| `ColorPrimary` / `ColorPrimaryHover` / `ColorPrimaryActive` | Primary 类型按钮主色调及其交互态 |
| `ColorError` / `ColorErrorHover` / `ColorErrorActive` | 危险样式色系 |
| `ColorLink` / `ColorLinkHover` / `ColorLinkActive` | Link 类型按钮色系 |
| `ColorText` / `ColorTextDisabled` | 文本色 / 禁用文本色 |
| `ColorBgContainer` / `ColorBgContainerDisabled` | 容器背景色 |
| `ColorBorder` | 边框颜色 |
| `ControlHeight` / `ControlHeightSM` / `ControlHeightLG` | 按钮高度（中 / 小 / 大） |
| `BorderRadius` / `BorderRadiusSM` / `BorderRadiusLG` | 圆角半径（中 / 小 / 大） |
| `BorderThickness` | 默认边框厚度 |
| `EnableMotion` | 全局动画开关 |

### 来自 PopupHostToken

| Token 资源键 | 使用场景 |
|---|---|
| `MarginToAnchor` | 弹出菜单与按钮之间的默认间距 |

---

## Token 对外观的具体影响

### 按钮类型与 Token 映射

DropdownButton 的颜色方案与 Button 完全一致：

| 按钮类型 | 正常态 | 悬浮态 | 按下态 | 禁用态 |
|---|---|---|---|---|
| **Default** | `DefaultColor` / `DefaultBg` / `DefaultBorderColor` | `DefaultHoverColor` / `DefaultHoverBorderColor` | `DefaultActiveColor` / `DefaultActiveBorderColor` | `ColorTextDisabled` / `ColorBgContainerDisabled` |
| **Primary** | `PrimaryColor` / `ColorPrimary` | - / `ColorPrimaryHover` | - / `ColorPrimaryActive` | `ColorTextDisabled` / `ColorBgContainerDisabled` |
| **Link** | `ColorLink` / 透明 | `ColorLinkHover` | `ColorLinkActive` | `ColorTextDisabled` / 透明 |
| **Text** | `DefaultColor` / 透明 | - / `TextHoverBg` | - / `ColorBgTextActive` | `ColorTextDisabled` / 透明 |

### 下拉指示器 Token 映射

| 尺寸 | 图标大小 | 指示器左边距 |
|---|---|---|
| `Large` | `SharedToken.IconSizeSM` | `ButtonToken.ExtraContentMarginLG` |
| `Middle` | `SharedToken.IconSizeSM` | `ButtonToken.ExtraContentMargin` |
| `Small` | `SharedToken.IconSizeSM` | `ButtonToken.ExtraContentMarginSM` |

下拉指示器的 `IconBrush` 绑定按钮的 `Foreground`，因此颜色跟随按钮类型和状态自动变化。

### 危险态下的 Token 覆盖

当 `IsDanger=True` 时，色系替换为 Error 色系（`ColorError` / `ColorErrorHover` / `ColorErrorActive`），阴影替换为 `DangerShadow`。

### 尺寸与 Token 映射

| 尺寸 | 高度 | 内间距 | 字号 | 圆角 |
|---|---|---|---|---|
| `Large` | `ControlHeightLG` | `PaddingLG` | `ContentFontSizeLG` | `BorderRadiusLG` |
| `Middle` | `ControlHeight` | `Padding` | `ContentFontSize` | `BorderRadius` |
| `Small` | `ControlHeightSM` | `PaddingSM` | `ContentFontSizeSM` | `BorderRadiusSM` |

