# MessageBox Design Token

MessageBox 使用 `MessageBoxToken`（Token ID: `"MessageBox"`）作为组件级 Design Token。MessageBox 的 Token 非常精简，大部分视觉属性继承自 Dialog 系统的 Token（`DialogToken`）。

> 📖 Token 源码位置：`src/AtomUI.Desktop.Controls/MessageBox/MessageBoxToken.cs`

---

## Token 资源访问方式

### AXAML 中访问

```xml
<!-- 组件级 Token（MessageBox 专属） -->
{atom:MessageBoxTokenResource StyleIconSize}

<!-- 全局共享 Token（图标颜色） -->
{atom:SharedTokenResource ColorInfo}
{atom:SharedTokenResource ColorSuccess}
{atom:SharedTokenResource ColorWarning}
{atom:SharedTokenResource ColorError}
{atom:SharedTokenResource SpacingSM}
```

---

## Token 类定义

```csharp
[ControlDesignToken]
internal class MessageBoxToken : AbstractControlDesignToken
{
    public const string ID = "MessageBox";
    // ...
}
```

> ⚠️ 注意：MessageBox 本身注册的 Token Scope 是 `DialogToken.ScopeProvider`（而非 `MessageBoxToken.ScopeProvider`），因为 MessageBox 复用了 Dialog 的大部分主题基础设施。`MessageBoxToken` 仅定义 MessageBox 独有的扩展 Token。

---

## 完整 Token 属性列表

### MessageBox 专属 Token

| Token 属性 | 类型 | 派生自 SharedToken | 说明 |
|---|---|---|---|
| `StyleIconSize` | `double` | `SizeLG × 1.2` | 语义样式图标的尺寸（宽度和高度） |

> `StyleIconSize` 的计算公式：`SharedToken.SizeLG * 1.2`。默认情况下 `SizeLG` 为 24，因此 `StyleIconSize` 约为 28.8px。

---

## 主题中使用的全局 SharedToken

除组件级 Token 外，MessageBox 的宿主主题还直接引用了以下全局 `SharedToken`：

| Token 资源键 | 使用场景 |
|---|---|
| `EnableMotion` | 全局动画开关 |
| `ColorInfo` | Information 样式图标颜色（蓝色） |
| `ColorSuccess` | Success 样式图标颜色（绿色） |
| `ColorWarning` | Warning / Confirm 样式图标颜色（黄色） |
| `ColorError` | Error 样式图标颜色（红色） |
| `SpacingSM` | 图标与内容之间的水平间距 |

### 继承自 Dialog 系统的 Token

MessageBox 复用 Dialog 的主题基础设施，因此还间接使用了 `DialogToken` 中定义的以下 Token（参见 Dialog 文档）：

| Token 来源 | 影响区域 |
|---|---|
| Dialog 背景色、边框、圆角 | 弹窗容器外观 |
| Dialog 标题栏样式 | 标题文本、关闭按钮 |
| Dialog 按钮区样式 | 底部按钮区的内边距和布局 |
| Dialog 内容区内边距 | 正文内容的内间距 |

---

## Token 在主题中的使用示例

### MessageBoxDialogHostTheme.axaml 中的 Token 引用

```xml
<!-- 语义图标尺寸 -->
<Style Selector="^ /template/ atom|IconPresenter#StyleIconPresenter">
    <Setter Property="Width" Value="{atom:MessageBoxTokenResource StyleIconSize}" />
    <Setter Property="Height" Value="{atom:MessageBoxTokenResource StyleIconSize}" />
</Style>

<!-- 图标与内容间距 -->
<DockPanel LastChildFill="True"
           HorizontalSpacing="{atom:SharedTokenResource SpacingSM}">
    ...
</DockPanel>

<!-- 各样式的图标颜色 -->
<Style Selector="^[Style=Confirm] /template/ atom|IconPresenter#StyleIconPresenter">
    <Setter Property="IconBrush" Value="{atom:SharedTokenResource ColorWarning}" />
</Style>

<Style Selector="^[Style=Information] /template/ atom|IconPresenter#StyleIconPresenter">
    <Setter Property="IconBrush" Value="{atom:SharedTokenResource ColorInfo}" />
</Style>

<Style Selector="^[Style=Success] /template/ atom|IconPresenter#StyleIconPresenter">
    <Setter Property="IconBrush" Value="{atom:SharedTokenResource ColorSuccess}" />
</Style>

<Style Selector="^[Style=Error] /template/ atom|IconPresenter#StyleIconPresenter">
    <Setter Property="IconBrush" Value="{atom:SharedTokenResource ColorError}" />
</Style>

<Style Selector="^[Style=Warning] /template/ atom|IconPresenter#StyleIconPresenter">
    <Setter Property="IconBrush" Value="{atom:SharedTokenResource ColorWarning}" />
</Style>
```

---

## Token 对外观的具体影响

### 消息样式与颜色 Token 映射

| 消息样式 | 图标颜色 Token | 默认颜色 |
|---|---|---|
| `Normal` | —（无图标） | — |
| `Confirm` | `SharedToken.ColorWarning` | 黄色（`#faad14`） |
| `Information` | `SharedToken.ColorInfo` | 蓝色（`#1677ff`） |
| `Success` | `SharedToken.ColorSuccess` | 绿色（`#52c41a`） |
| `Warning` | `SharedToken.ColorWarning` | 黄色（`#faad14`） |
| `Error` | `SharedToken.ColorError` | 红色（`#ff4d4f`） |

### 视觉布局 Token 映射

| 视觉属性 | Token | 说明 |
|---|---|---|
| 语义图标大小 | `MessageBoxToken.StyleIconSize` ← `SizeLG × 1.2` | 约 28.8px |
| 图标与内容间距 | `SharedToken.SpacingSM` | 小号间距（8px） |
| 弹窗背景/边框/圆角 | 继承自 `DialogToken` | 由 Dialog 主题控制 |
| 标题栏样式 | 继承自 `DialogToken` | 由 Dialog 主题控制 |
| 按钮区样式 | 继承自 `DialogToken` | 由 Dialog 主题控制 |

---

## Token 派生链总结

```
SharedToken (DesignToken)
├── SizeLG              → StyleIconSize（语义图标大小，× 1.2）
├── ColorInfo           → Information 图标颜色
├── ColorSuccess        → Success 图标颜色
├── ColorWarning        → Warning / Confirm 图标颜色
├── ColorError          → Error 图标颜色
├── SpacingSM           → 图标与内容间距
├── EnableMotion        → 动画开关
└── (DialogToken 继承)  → 弹窗容器、标题栏、按钮区样式
```
