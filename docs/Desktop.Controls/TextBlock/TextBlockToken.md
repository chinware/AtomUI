# TextBlock Design Token

TextBlock 是 Avalonia `TextBlock` 的轻量包装，**不使用组件级 Design Token**。它没有 `ControlDesignToken` 子类，没有 `TokenResourceScopeProvider`，也不通过 `RegisterTokenResourceScope` 注册 Token 作用域。

---

## 样式来源

TextBlock 的视觉属性来自以下几个层次：

### 1. Avalonia 全局 TextBlock 样式

Avalonia 框架为 `TextBlock` 提供了默认的字体族、字体大小、前景色等全局样式。AtomUI 的主题系统会通过全局样式覆盖这些默认值。

### 2. 父控件主题上下文

当 TextBlock 被嵌入其他控件的模板中时（如 Separator 标题、Tag 文本、FormItem 标签等），其样式由父控件的 ControlTheme 通过模板选择器（`/template/`）控制。例如：

```xml
<!-- 在 Separator 主题中，标题 TextBlock 的字号由 Separator Token 控制 -->
<Style Selector="^:is(atom|Separator) /template/ TextBlock#PART_Title">
    <Setter Property="FontSize" Value="{atom:SharedTokenResource FontSizeLG}" />
</Style>
```

### 3. 使用处直接设置

在 AXAML 中直接使用 `atom:TextBlock` 时，可通过属性或 Style 自由设置视觉属性：

```xml
<atom:TextBlock Text="Hello"
                FontSize="16"
                Foreground="{atom:SharedTokenResource ColorText}" />
```

---

## 可用的全局 SharedToken

虽然 TextBlock 没有组件级 Token，但可以在使用处通过 `{atom:SharedTokenResource}` 引用全局 Token 来保持主题一致性：

| Token 资源键 | 典型用途 |
|---|---|
| `ColorText` | 主要文本颜色 |
| `ColorTextSecondary` | 次要文本颜色 |
| `ColorTextTertiary` | 第三级文本颜色 |
| `ColorTextDisabled` | 禁用态文本颜色 |
| `ColorTextHeading` | 标题文本颜色 |
| `ColorLink` | 链接文本颜色 |
| `ColorError` | 错误文本颜色 |
| `ColorWarning` | 警告文本颜色 |
| `ColorSuccess` | 成功文本颜色 |
| `FontSize` | 标准字号（14px） |
| `FontSizeSM` | 小号字号（12px） |
| `FontSizeLG` | 大号字号（16px） |
| `FontSizeXL` | 特大号字号（20px） |
| `LineHeight` | 标准行高 |

---

## 示例：在自定义样式中使用 Token

```xml
<!-- 通过 Token 保持主题一致性 -->
<atom:TextBlock Text="Secondary text"
                Foreground="{atom:SharedTokenResource ColorTextSecondary}"
                FontSize="{atom:SharedTokenResource FontSizeSM}" />

<!-- 在 Style 中引用 Token -->
<Style Selector="atom|TextBlock.description">
    <Setter Property="Foreground" Value="{atom:SharedTokenResource ColorTextTertiary}" />
    <Setter Property="FontSize" Value="{atom:SharedTokenResource FontSizeSM}" />
</Style>
```
