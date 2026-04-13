# TextBlock 使用文档

本文档介绍 AtomUI TextBlock 控件的各种使用方式。

---

## 前置准备

在 AXAML 中使用 TextBlock，需要引入以下命名空间：

```xml
xmlns:atom="https://atomui.net"
```

在 C# 代码中引入：

```csharp
using AtomUI.Desktop.Controls;   // TextBlock 控件
```

---

## 1. 基本文本显示

最基本的用法，显示一段文本：

```xml
<atom:TextBlock Text="Hello, AtomUI!" />

<!-- 或使用内容语法 -->
<atom:TextBlock>Hello, AtomUI!</atom:TextBlock>
```

---

## 2. 文本换行

通过 `TextWrapping` 属性控制换行行为：

```xml
<!-- 不换行（默认） -->
<atom:TextBlock Text="This long text will not wrap and may be clipped." />

<!-- 自动换行 -->
<atom:TextBlock Text="This long text will automatically wrap to the next line when it exceeds the available width."
                TextWrapping="Wrap"
                MaxWidth="300" />
```

---

## 3. 省略号裁剪

当文本超出可用空间时，显示省略号（`…`）：

```xml
<!-- 字符级省略号 -->
<atom:TextBlock Text="This is a very long text that will be trimmed with character ellipsis"
                TextTrimming="CharacterEllipsis"
                MaxWidth="200" />

<!-- 单词级省略号 -->
<atom:TextBlock Text="This is a very long text that will be trimmed at word boundary"
                TextTrimming="WordEllipsis"
                MaxWidth="200" />
```

---

## 4. 多行限制

通过 `MaxLines` 限制最大显示行数，超出部分裁剪：

```xml
<atom:TextBlock TextWrapping="Wrap" MaxLines="3"
                TextTrimming="CharacterEllipsis"
                MaxWidth="300">
    Lorem ipsum dolor sit amet, consectetur adipiscing elit.
    Sed do eiusmod tempor incididunt ut labore et dolore magna aliqua.
    Ut enim ad minim veniam, quis nostrud exercitation ullamco laboris.
    Duis aute irure dolor in reprehenderit in voluptate velit esse cillum dolore.
</atom:TextBlock>
```

---

## 5. 文本装饰

通过 `TextDecorations` 添加下划线、删除线等装饰：

```xml
<StackPanel Spacing="8">
    <atom:TextBlock Text="Normal text" />
    <atom:TextBlock Text="Underlined text" TextDecorations="Underline" />
    <atom:TextBlock Text="Strikethrough text" TextDecorations="Strikethrough" />
</StackPanel>
```

---

## 6. 富文本（Inlines）

通过 `Inlines` 实现段内混合格式文本：

```xml
<atom:TextBlock>
    <Run Text="This is " />
    <Run Text="bold" FontWeight="Bold" />
    <Run Text=" and this is " />
    <Run Text="italic" FontStyle="Italic" />
    <Run Text=" and this is " />
    <Run Text="colored" Foreground="{atom:SharedTokenResource ColorPrimary}" />
    <Run Text="." />
</atom:TextBlock>
```

---

## 7. 使用 Token 保持主题一致性

在 AtomUI 中推荐通过 `SharedTokenResource` 引用全局 Token，而非硬编码颜色和字号：

```xml
<!-- 标题文本 -->
<atom:TextBlock Text="Section Title"
                FontSize="{atom:SharedTokenResource FontSizeLG}"
                FontWeight="Bold"
                Foreground="{atom:SharedTokenResource ColorTextHeading}" />

<!-- 次要文本 -->
<atom:TextBlock Text="Supplementary information"
                FontSize="{atom:SharedTokenResource FontSizeSM}"
                Foreground="{atom:SharedTokenResource ColorTextSecondary}" />

<!-- 错误文本 -->
<atom:TextBlock Text="Something went wrong"
                Foreground="{atom:SharedTokenResource ColorError}" />

<!-- 禁用文本 -->
<atom:TextBlock Text="Not available"
                Foreground="{atom:SharedTokenResource ColorTextDisabled}" />
```

---

## 8. 在 Form 中使用

TextBlock 实现了 `IFormItemAware` 接口，可作为只读展示字段参与 Form 表单：

```xml
<atom:Form>
    <atom:FormItem Label="Username">
        <atom:TextBlock Text="{Binding Username}" />
    </atom:FormItem>
    <atom:FormItem Label="Email">
        <atom:TextBlock Text="{Binding Email}" />
    </atom:FormItem>
</atom:Form>
```

---

## 9. 数据绑定

TextBlock 的 `Text` 属性支持标准 Avalonia 数据绑定：

```xml
<!-- 绑定 ViewModel 属性 -->
<atom:TextBlock Text="{Binding DisplayName}" />

<!-- 字符串格式化 -->
<atom:TextBlock Text="{Binding Price, StringFormat='¥{0:N2}'}" />

<!-- 多值绑定 -->
<atom:TextBlock>
    <atom:TextBlock.Text>
        <MultiBinding StringFormat="{}{0} - {1}">
            <Binding Path="FirstName" />
            <Binding Path="LastName" />
        </MultiBinding>
    </atom:TextBlock.Text>
</atom:TextBlock>
```

---

## 常见组合模式

### 标题 + 描述

```xml
<StackPanel Spacing="4">
    <atom:TextBlock Text="Card Title"
                    FontSize="{atom:SharedTokenResource FontSizeLG}"
                    FontWeight="Bold" />
    <atom:TextBlock Text="This is a brief description of the card content."
                    Foreground="{atom:SharedTokenResource ColorTextSecondary}" />
</StackPanel>
```

### 操作链接分隔

```xml
<StackPanel Orientation="Horizontal" Spacing="8">
    <atom:HyperLinkButton>Edit</atom:HyperLinkButton>
    <atom:VerticalSeparator />
    <atom:HyperLinkButton IsDanger="True">Delete</atom:HyperLinkButton>
    <atom:VerticalSeparator />
    <atom:TextBlock Text="Created 2026-01-01"
                    Foreground="{atom:SharedTokenResource ColorTextTertiary}"
                    VerticalAlignment="Center" />
</StackPanel>
```

### 状态文本

```xml
<StackPanel Spacing="4">
    <atom:TextBlock Classes="type-success" Text="✓ Operation completed" />
    <atom:TextBlock Classes="type-warning" Text="⚠ Pending review" />
    <atom:TextBlock Classes="type-danger" Text="✕ Failed to save" />
</StackPanel>
```
