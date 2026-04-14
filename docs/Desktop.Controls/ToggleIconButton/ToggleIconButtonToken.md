# ToggleIconButton Design Token

ToggleIconButton 复用 `ButtonToken`（Token ID: `"Button"`）作为组件级 Design Token 作用域。它本身不定义独立的组件级 Token，而是直接使用 `SharedToken` 中的图标色系和尺寸 Token 来控制视觉表现。

---

## Token 作用域

ToggleIconButton 在构造函数中注册了 `ButtonToken.ScopeProvider`：

```csharp
public class ToggleIconButton : AbstractToggleIconButton
{
    public ToggleIconButton()
    {
        this.RegisterTokenResourceScope(ButtonToken.ScopeProvider);
    }
}
```

这意味着 ToggleIconButton 可以访问 `ButtonToken` 中定义的所有组件级 Token，但其主题模板（`ToggleIconButtonTheme.axaml`）实际上主要使用的是全局 `SharedToken`。

---

## Token 资源访问方式

在 AXAML 中使用全局共享 Token：

```xml
{atom:SharedTokenResource ColorIcon}
{atom:SharedTokenResource ColorIconHover}
{atom:SharedTokenResource ColorText}
{atom:SharedTokenResource ColorTextDisabled}
{atom:SharedTokenResource IconSizeSM}
{atom:SharedTokenResource BorderRadiusSM}
{atom:SharedTokenResource EnableMotion}
```

在 AXAML 中使用 ButtonToken 组件级 Token（可选）：

```xml
{atom:TokenResource PrimaryColor}
{atom:TokenResource DefaultColor}
```

---

## 主题中使用的全局 SharedToken

以下是 ToggleIconButton 的 ControlTheme 直接引用的全部 `SharedToken`：

### 图标颜色 Token

| Token 资源键 | 类型 | 使用场景 | 说明 |
|---|---|---|---|
| `ColorIcon` | `Color` | 正常态（默认）图标颜色 | 未选中且无交互时的图标色 |
| `ColorIconHover` | `Color` | 悬浮态图标颜色 | 鼠标悬浮时的图标色，比正常态略深 |
| `ColorText` | `Color` | 按下态图标颜色 | 鼠标按下时的图标色，视觉反馈最强 |
| `ColorTextDisabled` | `Color` | 禁用态图标颜色 | 灰色调，表示不可交互 |

### 尺寸 Token

| Token 资源键 | 类型 | 使用场景 | 说明 |
|---|---|---|---|
| `IconSizeSM` | `double` | 默认图标宽度/高度 | 小号图标尺寸，适合纯图标按钮的紧凑场景 |
| `BorderRadiusSM` | `CornerRadius` | 圆角半径 | 小号圆角，与小号图标尺寸视觉协调 |

### 动画 Token

| Token 资源键 | 类型 | 使用场景 | 说明 |
|---|---|---|---|
| `EnableMotion` | `bool` | 全局动画开关 | 控制 `IsMotionEnabled` 的默认值 |

---

## Token 对外观的具体影响

### 状态与 Token 映射

ToggleIconButton 的主题通过伪类选择器将不同状态映射到不同的 Token 颜色：

| 状态 | 伪类 | `IconBrush` 来源 | 视觉效果 |
|---|---|---|---|
| **正常态** | 无特殊伪类 | `ColorIcon` | 中灰色图标，低调不抢眼 |
| **悬浮态** | `:pointerover` | `ColorIconHover` | 稍深的灰色，提示可交互 |
| **按下态** | `:pressed` | `ColorText` | 最深色（通常接近黑色），强反馈 |
| **禁用态** | `:disabled` | `ColorTextDisabled` | 浅灰色，明确表示不可用 |

### 颜色属性覆盖机制

当用户通过属性显式设置颜色时，这些属性值优先于主题默认的 Token 颜色：

| 用户属性 | 覆盖的状态 | 对应的默认 Token |
|---|---|---|
| `NormalIconBrush` | 未选中正常态 | `ColorIcon` |
| `ActiveIconBrush` | 悬浮态 + 按下态 | `ColorIconHover` / `ColorText` |
| `SelectedIconBrush` | 选中态 | `ColorIcon` |
| `DisabledIconBrush` | 禁用态 | `ColorTextDisabled` |

### 尺寸与 Token 映射

| 维度 | Token | 默认值（Default 主题） | 说明 |
|---|---|---|---|
| 图标宽度 | `IconSizeSM` | 12px | 通过 `IconWidth` 属性覆盖 |
| 图标高度 | `IconSizeSM` | 12px | 通过 `IconHeight` 属性覆盖 |
| 圆角 | `BorderRadiusSM` | 4px | 背景框圆角 |

### 与 Button Token 的关系

虽然 ToggleIconButton 注册了 `ButtonToken.ScopeProvider`，但其主题实际未使用 ButtonToken 中的专有 Token（如 `PrimaryColor`、`DefaultBg`、`Padding` 等）。这是因为 ToggleIconButton 是纯图标控件，不需要 Button 复杂的颜色方案和内间距体系。注册 ButtonToken 作用域主要是为了保持架构一致性，并为未来可能的扩展预留空间。
