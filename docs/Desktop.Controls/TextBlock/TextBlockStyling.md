# TextBlock 自定义样式指南

TextBlock 的视觉表现通过 Avalonia 原生属性和 AtomUI 全局 Token 资源控制。由于 TextBlock 没有 ControlTheme，样式定制主要通过属性设置和 Style 选择器实现。

---

## 1. 使用属性直接控制

最简单的方式是通过 TextBlock 的公共属性来控制外观：

```xml
<!-- 基本文本 -->
<atom:TextBlock Text="Hello, World!" />

<!-- 换行文本 -->
<atom:TextBlock Text="This is a very long text that will wrap to the next line."
                TextWrapping="Wrap" />

<!-- 省略号裁剪 -->
<atom:TextBlock Text="This text will be trimmed with ellipsis..."
                TextTrimming="CharacterEllipsis"
                MaxWidth="200" />

<!-- 多行限制 -->
<atom:TextBlock Text="Line 1&#x0a;Line 2&#x0a;Line 3&#x0a;Line 4"
                TextWrapping="Wrap" MaxLines="2"
                TextTrimming="CharacterEllipsis" />

<!-- 自定义字体 -->
<atom:TextBlock Text="Bold Heading"
                FontSize="20" FontWeight="Bold"
                Foreground="{atom:SharedTokenResource ColorTextHeading}" />

<!-- 文本装饰 -->
<atom:TextBlock Text="Strikethrough"
                TextDecorations="Strikethrough" />
<atom:TextBlock Text="Underline"
                TextDecorations="Underline" />
```

---

## 2. 通过 Style 覆盖样式

### 全局文本样式

```xml
<Window.Styles>
    <Style Selector="atom|TextBlock">
        <Setter Property="FontSize" Value="{atom:SharedTokenResource FontSize}" />
        <Setter Property="Foreground" Value="{atom:SharedTokenResource ColorText}" />
    </Style>
</Window.Styles>
```

### 使用 CSS-like 类选择器

```xml
<!-- 定义样式类 -->
<Window.Styles>
    <Style Selector="atom|TextBlock.heading">
        <Setter Property="FontSize" Value="{atom:SharedTokenResource FontSizeLG}" />
        <Setter Property="FontWeight" Value="Bold" />
        <Setter Property="Foreground" Value="{atom:SharedTokenResource ColorTextHeading}" />
    </Style>
    <Style Selector="atom|TextBlock.secondary">
        <Setter Property="FontSize" Value="{atom:SharedTokenResource FontSizeSM}" />
        <Setter Property="Foreground" Value="{atom:SharedTokenResource ColorTextSecondary}" />
    </Style>
    <Style Selector="atom|TextBlock.danger">
        <Setter Property="Foreground" Value="{atom:SharedTokenResource ColorError}" />
    </Style>
</Window.Styles>

<!-- 使用样式类 -->
<atom:TextBlock Classes="heading" Text="Section Title" />
<atom:TextBlock Classes="secondary" Text="Supplementary info" />
<atom:TextBlock Classes="danger" Text="Error message" />
```

### 上下文样式（在特定容器内）

```xml
<!-- 在 StackPanel 内的所有 TextBlock 使用小号字体 -->
<Style Selector="StackPanel.compact atom|TextBlock">
    <Setter Property="FontSize" Value="{atom:SharedTokenResource FontSizeSM}" />
    <Setter Property="Margin" Value="0,2" />
</Style>
```

---

## 3. 富文本（Inlines）

通过 `Inlines` 实现段内混合格式：

```xml
<atom:TextBlock>
    <Run Text="Normal text, " />
    <Run Text="bold text, " FontWeight="Bold" />
    <Run Text="colored text, " Foreground="{atom:SharedTokenResource ColorPrimary}" />
    <Run Text="small text." FontSize="12" />
</atom:TextBlock>
```

---

## 4. 模拟 Ant Design Typography 变体

Ant Design 的 `Typography.Text` 支持多种变体。在 AtomUI 中可通过样式类模拟：

```xml
<Window.Styles>
    <!-- type="secondary" -->
    <Style Selector="atom|TextBlock.type-secondary">
        <Setter Property="Foreground" Value="{atom:SharedTokenResource ColorTextSecondary}" />
    </Style>
    <!-- type="success" -->
    <Style Selector="atom|TextBlock.type-success">
        <Setter Property="Foreground" Value="{atom:SharedTokenResource ColorSuccess}" />
    </Style>
    <!-- type="warning" -->
    <Style Selector="atom|TextBlock.type-warning">
        <Setter Property="Foreground" Value="{atom:SharedTokenResource ColorWarning}" />
    </Style>
    <!-- type="danger" -->
    <Style Selector="atom|TextBlock.type-danger">
        <Setter Property="Foreground" Value="{atom:SharedTokenResource ColorError}" />
    </Style>
    <!-- disabled -->
    <Style Selector="atom|TextBlock:disabled">
        <Setter Property="Foreground" Value="{atom:SharedTokenResource ColorTextDisabled}" />
    </Style>
</Window.Styles>
```

---

## 样式选择器速查

> 说明：Avalonia 样式选择器中使用 `atom|TextBlock` 语法引用 `atom` XML 命名空间下的 `TextBlock` 类型，其中 `|` 是命名空间分隔符。

### 基本选择

| 选择器 | 说明 |
|---|---|
| `atom\|TextBlock` | 匹配所有 AtomUI TextBlock 实例 |

### 按属性选择

| 选择器 | 说明 |
|---|---|
| `atom\|TextBlock[TextWrapping=Wrap]` | 匹配设置了换行的 TextBlock |
| `atom\|TextBlock[FontWeight=Bold]` | 匹配粗体 TextBlock |

### 按样式类选择

| 选择器 | 说明 |
|---|---|
| `atom\|TextBlock.heading` | 匹配带 `heading` 样式类的 TextBlock |
| `atom\|TextBlock.secondary` | 匹配带 `secondary` 样式类的 TextBlock |

### 按伪类选择

| 选择器 | 说明 |
|---|---|
| `atom\|TextBlock:pointerover` | 鼠标悬浮状态 |
| `atom\|TextBlock:disabled` | 禁用状态 |

### 上下文选择

| 选择器 | 说明 |
|---|---|
| `atom\|FormItem atom\|TextBlock` | FormItem 内的 TextBlock |
| `StackPanel atom\|TextBlock` | StackPanel 内的 TextBlock |
