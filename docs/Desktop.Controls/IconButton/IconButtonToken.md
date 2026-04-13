# IconButton Design Token

IconButton 控件复用 `ButtonToken`（Token ID: `"Button"`）作为组件级 Design Token 作用域。由于 IconButton 的模板非常轻量，它主要消费全局 `SharedToken` 中的值，不直接使用 `ButtonToken` 中的大部分颜色、间距和字体 Token。

---

## Token 架构说明

### 为什么复用 ButtonToken？

IconButton 在构造函数中注册了 `ButtonToken.ScopeProvider`：

```csharp
public class IconButton : AbstractIconButton
{
    public IconButton()
    {
        this.RegisterTokenResourceScope(ButtonToken.ScopeProvider);
    }
}
```

这意味着 IconButton 模板中可以使用 `{atom:TokenResource ...}` 引用 `ButtonToken` 的所有 Token 属性。但实际上，IconButton 的极简主题模板仅使用全局 `SharedToken`，并未直接引用 `ButtonToken` 的组件级 Token。这种设计保留了扩展灵活性——如果未来 IconButton 需要更丰富的样式变体，可以直接引用 `ButtonToken` 的 Token 而无需修改 Token 作用域注册。

---

## Token 资源访问方式

在 AXAML 中，IconButton 主题使用全局共享 Token：

```xml
<!-- IconButton 主题中实际使用的 Token -->
{atom:SharedTokenResource ColorIcon}
{atom:SharedTokenResource ColorIconHover}
{atom:SharedTokenResource ColorText}
{atom:SharedTokenResource ColorTextDisabled}
{atom:SharedTokenResource IconSizeSM}
{atom:SharedTokenResource BorderRadiusSM}
{atom:SharedTokenResource EnableMotion}
```

---

## 消费的全局 SharedToken

以下是 IconButton 主题模板（`IconButtonTheme.axaml`）中直接引用的全部 SharedToken：

| Token 资源键 | 类型 | 使用场景 |
|---|---|---|
| `ColorIcon` | `Color` | 正常态图标颜色 |
| `ColorIconHover` | `Color` | 悬浮态图标颜色 |
| `ColorText` | `Color` | 按下态图标颜色 |
| `ColorTextDisabled` | `Color` | 禁用态图标颜色 |
| `IconSizeSM` | `double` | 默认图标宽度和高度 |
| `BorderRadiusSM` | `CornerRadius` | 默认圆角半径 |
| `EnableMotion` | `bool` | 全局动画开关，控制 `IsMotionEnabled` 的默认值 |

---

## Token 对外观的影响

### 交互状态与 Token 映射

IconButton 通过 `IconBrush` 属性在不同状态下自动切换图标颜色：

| 状态 | IconBrush 来源 | Background | 说明 |
|---|---|---|---|
| 正常 | `ColorIcon` | 透明 | 默认状态，使用主题图标色 |
| 悬浮（`:pointerover`） | `ColorIconHover` | 透明 | 鼠标悬浮时图标颜色加深 |
| 按下（`:pressed`） | `ColorText` | 透明 | 按压时使用正文色，反馈更强 |
| 禁用（`:disabled`） | `ColorTextDisabled` | 透明 | 灰色调，表示不可交互 |

### 尺寸与 Token 映射

| 属性 | Token 来源 | 说明 |
|---|---|---|
| `IconWidth` | `SharedToken.IconSizeSM` | 默认图标宽度（通常为 12px） |
| `IconHeight` | `SharedToken.IconSizeSM` | 默认图标高度（通常为 12px） |
| `CornerRadius` | `SharedToken.BorderRadiusSM` | 默认圆角半径 |

### 暗色主题适配

IconButton 的颜色完全由 SharedToken 驱动。当主题切换到暗色模式时：
- `ColorIcon`、`ColorIconHover`、`ColorText`、`ColorTextDisabled` 等 Token 会由暗色主题算法自动重新计算
- IconButton 无需额外适配，自动跟随主题变化

---

## 与 ButtonToken 的关系

> **注意**：虽然 IconButton 注册了 `ButtonToken` 作用域，但 `ButtonToken` 中的大部分 Token（如 `PrimaryColor`、`DefaultBg`、`DefaultBorderColor`、`Padding`、`ContentFontSize` 等）对 IconButton **没有直接影响**。IconButton 的视觉表现完全由 `IconBrush` 和全局 SharedToken 的图标色系驱动。
>
> 如果需要为 IconButton 添加更丰富的视觉变体（如带背景色的 IconButton），建议通过 Style 覆盖 `Background` 属性并引用 `SharedToken`，而非直接引用 `ButtonToken` 的颜色 Token。

