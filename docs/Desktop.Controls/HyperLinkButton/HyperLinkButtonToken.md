# HyperLinkButton Design Token

HyperLinkButton 复用 `ButtonToken`（Token ID: `"Button"`）作为组件级 Design Token 作用域。间距、字体等 Token 与 Button 共享，颜色使用 `SharedToken` 中的链接色系。

> **架构说明**：HyperLinkButton 没有定义自己的 `ControlDesignToken`，而是在构造函数中通过 `this.RegisterTokenResourceScope(ButtonToken.ScopeProvider)` 直接复用 Button 的 Token 作用域。这意味着所有 `ButtonTokenResource` 标记扩展在 HyperLinkButton 的主题中同样可用。

---

## Token 资源访问方式

在 AXAML 中使用组件级 Token（来自 ButtonToken）：

```xml
{atom:ButtonTokenResource Padding}
{atom:ButtonTokenResource PaddingLG}
{atom:ButtonTokenResource PaddingSM}
{atom:ButtonTokenResource ContentFontSize}
{atom:ButtonTokenResource ContentFontSizeLG}
{atom:ButtonTokenResource ContentFontSizeSM}
{atom:ButtonTokenResource IconMargin}
{atom:ButtonTokenResource IconOnyPadding}
{atom:ButtonTokenResource OnlyIconSize}
```

在 AXAML 中使用全局共享 Token：

```xml
{atom:SharedTokenResource ColorLink}
{atom:SharedTokenResource ColorLinkHover}
{atom:SharedTokenResource ColorLinkActive}
{atom:SharedTokenResource ColorError}
{atom:SharedTokenResource ControlHeight}
{atom:SharedTokenResource IconSize}
```

---

## 来自 ButtonToken 的组件级 Token

HyperLinkButton 从 `ButtonToken` 作用域使用以下 Token：

### 内间距 Token

| Token 名 | 类型 | 说明 |
|---|---|---|
| `Padding` | `Thickness` | 中号按钮内间距 |
| `PaddingLG` | `Thickness` | 大号按钮内间距 |
| `PaddingSM` | `Thickness` | 小号按钮内间距 |
| `IconOnyPadding` | `Thickness` | 仅图标中号按钮内间距 |
| `IconOnyPaddingLG` | `Thickness` | 仅图标大号按钮内间距 |
| `IconOnyPaddingSM` | `Thickness` | 仅图标小号按钮内间距 |

### 字体 Token

| Token 名 | 类型 | 说明 |
|---|---|---|
| `ContentFontSize` | `double` | 中号按钮字体大小 |
| `ContentFontSizeLG` | `double` | 大号按钮字体大小 |
| `ContentFontSizeSM` | `double` | 小号按钮字体大小 |

### 图标 Token

| Token 名 | 类型 | 说明 |
|---|---|---|
| `OnlyIconSize` | `double` | 仅图标中号按钮图标尺寸 |
| `OnlyIconSizeLG` | `double` | 仅图标大号按钮图标尺寸 |
| `OnlyIconSizeSM` | `double` | 仅图标小号按钮图标尺寸 |
| `IconMargin` | `Thickness` | 图标与文本之间的间距（图标 + 文本共存时使用） |

### 其他 Token

| Token 名 | 类型 | 说明 |
|---|---|---|
| `BorderColorDisabled` | `Color` | 禁用态边框颜色 |

---

## 来自 SharedToken 的全局 Token

HyperLinkButton 的 ControlTheme 直接引用以下全局 `SharedToken`：

### 颜色 Token（链接色系）

| Token 资源键 | 使用场景 | 说明 |
|---|---|---|
| `ColorLink` | 正常态前景色 + 图标色 | 链接文本的默认颜色（通常为蓝色） |
| `ColorLinkHover` | 悬浮态前景色 + 图标色 | 鼠标悬浮时的链接颜色 |
| `ColorLinkActive` | 按下态前景色 + 图标色 | 鼠标按下时的链接颜色 |

### 颜色 Token（危险色系）

| Token 资源键 | 使用场景 | 说明 |
|---|---|---|
| `ColorError` | `:danger` 正常态前景色 + 图标色 | 危险链接的默认红色 |
| `ColorErrorHover` | `:danger` 悬浮态前景色 + 图标色 | 危险链接悬浮时的颜色 |
| `ColorErrorActive` | `:danger` 按下态前景色 + 图标色 | 危险链接按下时的颜色 |

### 颜色 Token（禁用态）

| Token 资源键 | 使用场景 | 说明 |
|---|---|---|
| `ColorTextDisabled` | `:disabled` 前景色 + 图标色 | 禁用状态的灰色文本 |

### 尺寸 Token

| Token 资源键 | 使用场景 | 说明 |
|---|---|---|
| `ControlHeight` | `SizeType=Middle` 时的高度 | 中号链接按钮高度 |
| `ControlHeightSM` | `SizeType=Small` 时的高度 | 小号链接按钮高度 |
| `ControlHeightLG` | `SizeType=Large` 时的高度 | 大号链接按钮高度 |
| `IconSize` | `SizeType=Middle` 时的图标大小 | 中号图标尺寸 |
| `IconSizeSM` | `SizeType=Small` 时的图标大小 | 小号图标尺寸 |
| `IconSizeLG` | `SizeType=Large` 时的图标大小 | 大号图标尺寸 |

### 动画 Token

| Token 资源键 | 使用场景 | 说明 |
|---|---|---|
| `EnableMotion` | `IsMotionEnabled` 默认值 | 全局动画开关 |
| `OpacityLoading` | `:loading` 时的不透明度 | 加载态透明度（通常为 0.65） |

---

## Token 对外观的具体影响

### 状态与颜色 Token 映射

| 状态组合 | 前景色 | 图标色（IconBrush/FillBrush） |
|---|---|---|
| 正常 | `ColorLink` | `ColorLink` |
| 正常 + 悬浮 | `ColorLinkHover` | `ColorLinkHover` |
| 正常 + 按下 | `ColorLinkActive` | `ColorLinkActive` |
| 危险 | `ColorError` | `ColorError` |
| 危险 + 悬浮 | `ColorErrorHover` | `ColorErrorHover` |
| 危险 + 按下 | `ColorErrorActive` | `ColorErrorActive` |
| 禁用（任何类型） | `ColorTextDisabled` | `ColorTextDisabled` |
| 加载中 | 继承当前前景色 | 继承当前图标色，并添加旋转动画 |

### 尺寸与 Token 映射

| 尺寸 | 高度 | 内间距 | 字号 | 图标大小 | 仅图标尺寸 |
|---|---|---|---|---|---|
| `Large` | `ControlHeightLG` | `PaddingLG` | `ContentFontSizeLG` | `IconSizeLG` | `OnlyIconSizeLG` |
| `Middle` | `ControlHeight` | `Padding` | `ContentFontSize` | `IconSize` | `OnlyIconSize` |
| `Small` | `ControlHeightSM` | `PaddingSM` | `ContentFontSizeSM` | `IconSizeSM` | `OnlyIconSizeSM` |

### 仅图标模式的 Token 差异

当 `:icononly` 伪类激活时（仅有图标无文本）：
- 图标间距 `Margin` 重置为 `0`（不再使用 `IconMargin`）
- 图标尺寸切换为 `OnlyIconSize` / `OnlyIconSizeLG` / `OnlyIconSizeSM`（比带文本模式更大）
- 内间距切换为 `IconOnyPadding` / `IconOnyPaddingLG` / `IconOnyPaddingSM`
