# Grid 自定义样式指南

Grid 栅格系统（`Row` / `Col`）是纯布局控件，自身不渲染任何可见样式。因此自定义样式主要有两个层面：

1. **调整布局行为** — 通过 `Row` / `Col` 的属性控制间距、对齐、栅格分配
2. **调整子元素样式** — 通过 Avalonia `Style` 控制放置在 `Col` 内的子控件外观

---

## 1. 通过属性直接控制布局

`Row` 和 `Col` 的布局行为完全由属性控制，不需要覆盖 ControlTheme。

### Row 布局属性

```xml
<!-- 间距 + 对齐 + 换行 -->
<atom:Row Gutter="16,24"
          Justify="SpaceBetween"
          Align="Middle"
          Wrap="True">
    <atom:Col Span="6">
        <Border Height="48" Background="#1677FF" />
    </atom:Col>
    <atom:Col Span="6">
        <Border Height="48" Background="#4096FF" />
    </atom:Col>
</atom:Row>
```

### Col 布局属性

```xml
<!-- 列宽 + 偏移 + 顺序 + 推拉 -->
<atom:Col Span="12"
          Offset="2"
          Order="1"
          Push="4"
          Pull="0">
    <Border Height="48" Background="#1677FF" />
</atom:Col>
```

### 响应式布局属性

```xml
<!-- 不同断点使用不同布局 -->
<atom:Col Span="24"
          Xs="24"
          Sm="12"
          Md="8"
          Lg="6"
          Xl="4"
          Xxl="3">
    <Border Height="48" Background="#1677FF" />
</atom:Col>
```

---

## 2. 通过 Style 设置批量属性

虽然 `Row` 和 `Col` 不使用 ControlTheme，但可以通过 Avalonia 的 `Style` 系统批量设置属性默认值。

### 为所有 Row 设置默认间距

```xml
<Window.Styles>
    <!-- 所有 Row 默认使用 16,16 间距 -->
    <Style Selector="atom|Row">
        <Setter Property="Gutter" Value="16,16" />
    </Style>
</Window.Styles>
```

### 为特定容器内的 Row 设置对齐方式

```xml
<Window.Styles>
    <!-- form-row 类中的 Row 使用特定对齐 -->
    <Style Selector="StackPanel.form-row atom|Row">
        <Setter Property="Gutter" Value="8,8" />
        <Setter Property="Align" Value="Middle" />
    </Style>
</Window.Styles>
```

### 为 Col 设置默认属性

```xml
<Window.Styles>
    <!-- 所有 Col 默认禁用内容裁剪 -->
    <Style Selector="atom|Col">
        <Setter Property="ClipToBounds" Value="False" />
    </Style>
</Window.Styles>
```

---

## 3. 为栅格子元素定义样式

Gallery 中使用的样式模式：在容器级别定义子元素样式类，然后在 `Col` 的子元素上应用。

> 📖 参考：`controlgallery/AtomUIGallery/ShowCases/Views/Layout/GridShowCase.axaml`

### 典型网格演示样式

```xml
<UserControl.Styles>
    <!-- 基础网格单元格 -->
    <Style Selector="Border.grid-cell">
        <Setter Property="Height" Value="48" />
        <Setter Property="Background" Value="#1677FF" />
    </Style>
    
    <!-- 交替颜色的单元格 -->
    <Style Selector="Border.grid-cell.alt">
        <Setter Property="Background" Value="#4096FF" />
    </Style>
    
    <!-- 行背景 -->
    <Style Selector="Border.grid-row-surface">
        <Setter Property="Background" Value="#F5F5F5" />
        <Setter Property="Padding" Value="8" />
    </Style>
    
    <!-- 单元格内文字 -->
    <Style Selector="TextBlock.grid-cell-text">
        <Setter Property="HorizontalAlignment" Value="Center" />
        <Setter Property="VerticalAlignment" Value="Center" />
        <Setter Property="Foreground" Value="White" />
        <Setter Property="FontWeight" Value="SemiBold" />
    </Style>
</UserControl.Styles>

<!-- 使用样式 -->
<atom:Row Gutter="16,16">
    <atom:Col Span="6">
        <Border Classes="grid-cell">
            <TextBlock Classes="grid-cell-text">col-6</TextBlock>
        </Border>
    </atom:Col>
    <atom:Col Span="6">
        <Border Classes="grid-cell alt">
            <TextBlock Classes="grid-cell-text">col-6</TextBlock>
        </Border>
    </atom:Col>
</atom:Row>
```

---

## 4. 卡片网格布局

一个常见的实际应用场景——使用栅格创建卡片网格：

```xml
<UserControl.Styles>
    <Style Selector="Border.card">
        <Setter Property="Background" Value="White" />
        <Setter Property="CornerRadius" Value="8" />
        <Setter Property="Padding" Value="16" />
        <Setter Property="BoxShadow" Value="0 2 8 0 #0000001A" />
    </Style>
</UserControl.Styles>

<atom:Row Gutter="16,16">
    <atom:Col Xs="24" Sm="12" Md="8" Lg="6">
        <Border Classes="card">
            <StackPanel Spacing="8">
                <TextBlock Text="Card Title 1" FontWeight="Bold" />
                <TextBlock Text="Card content goes here..." />
            </StackPanel>
        </Border>
    </atom:Col>
    <atom:Col Xs="24" Sm="12" Md="8" Lg="6">
        <Border Classes="card">
            <StackPanel Spacing="8">
                <TextBlock Text="Card Title 2" FontWeight="Bold" />
                <TextBlock Text="Card content goes here..." />
            </StackPanel>
        </Border>
    </atom:Col>
    <atom:Col Xs="24" Sm="12" Md="8" Lg="6">
        <Border Classes="card">
            <StackPanel Spacing="8">
                <TextBlock Text="Card Title 3" FontWeight="Bold" />
                <TextBlock Text="Card content goes here..." />
            </StackPanel>
        </Border>
    </atom:Col>
    <atom:Col Xs="24" Sm="12" Md="8" Lg="6">
        <Border Classes="card">
            <StackPanel Spacing="8">
                <TextBlock Text="Card Title 4" FontWeight="Bold" />
                <TextBlock Text="Card content goes here..." />
            </StackPanel>
        </Border>
    </atom:Col>
</atom:Row>
```

---

## 样式选择器速查

| 选择器 | 说明 |
|---|---|
| `atom\|Row` | 匹配所有 Row |
| `atom\|Col` | 匹配所有 Col |
| `atom\|Row > atom\|Col` | 匹配 Row 直接子元素 Col |
| `StackPanel.form-layout atom\|Row` | 匹配特定容器内的 Row |
| `atom\|Col > Border` | 匹配 Col 直接子元素 Border |

---

## 注意事项

1. **Grid 没有 ControlTheme** — `Row` 继承自 `Panel`，`Col` 继承自 `ContentControl`，它们不定义独立的 ControlTheme。无法通过 `ControlTheme` 覆盖来改变栅格布局行为。

2. **间距由 Gutter 实现** — 不要通过 `Style` 设置 `Col` 的 `Margin` 或 `Padding` 来模拟间距，这会与 Gutter 计算冲突。

3. **背景色在子元素上设置** — `Row` 和 `Col` 自身不应设置背景色（除非有特殊需求）。应该在放置在 `Col` 内的 `Border` 或其他容器上设置视觉样式。

4. **响应式配置优先级** — `Xs` / `Sm` / `Md` / `Lg` / `Xl` / `Xxl` 属性会覆盖 `Span` / `Offset` / `Order` / `Push` / `Pull` 的基础值。断点属性中未指定的字段回退到基础值。
