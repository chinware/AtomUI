# ListBox 自定义样式指南

ListBox 控件族的视觉表现通过 `ControlTheme` + Design Token 系统控制。以下介绍各种常见的自定义方式。

---

## 1. 使用属性直接控制

### 尺寸

```xml
<atom:ListBox SizeType="Large" />
<atom:ListBox SizeType="Middle" />
<atom:ListBox SizeType="Small" />
```

### 无边框模式

```xml
<atom:ListBox IsBorderless="True" />
```

### 自定义悬浮和选中背景色

```xml
<atom:ListBox ItemHoverBg="#E6F4FF" ItemSelectedBg="#BAE0FF" />
```

### 选中指示器

```xml
<!-- 启用勾选图标 -->
<atom:ListBox IsShowSelectedIndicator="True" />

<!-- 自定义选中图标 -->
<atom:ListBox IsShowSelectedIndicator="True">
    <atom:ListBox.SelectedIndicator>
        <atom:IconTemplate>
            <antdicons:HeartFilled />
        </atom:IconTemplate>
    </atom:ListBox.SelectedIndicator>
</atom:ListBox>
```

### 禁用选择

```xml
<!-- 纯展示模式，不可选择 -->
<atom:ListBox IsSelectable="False" />
```

### 过滤与高亮

```xml
<atom:ListBox ItemFilterValue="{Binding SearchText}"
              ItemFilterHighlightStrategy="All" />
```

### 自定义空状态

```xml
<atom:ListBox>
    <atom:ListBox.EmptyIndicatorTemplate>
        <DataTemplate>
            <atom:TextBlock Text="暂无数据" HorizontalAlignment="Center" />
        </DataTemplate>
    </atom:ListBox.EmptyIndicatorTemplate>
</atom:ListBox>
```

---

## 2. 通过 Style 覆盖样式

### 全局 ListBox 样式

```xml
<Window.Styles>
    <Style Selector="atom|ListBox">
        <Setter Property="Margin" Value="0 5" />
        <Setter Property="Background" Value="Transparent" />
    </Style>
</Window.Styles>
```

### 自定义 ListBoxItem 样式

```xml
<!-- 自定义悬浮态背景色 -->
<Style Selector="atom|ListBoxItem:pointerover">
    <Setter Property="Background" Value="#F0F5FF" />
</Style>

<!-- 自定义选中态背景色 -->
<Style Selector="atom|ListBoxItem:selected:not(:disabled)">
    <Setter Property="Background" Value="#D6E4FF" />
    <Setter Property="Foreground" Value="#1677FF" />
</Style>

<!-- 禁用态 -->
<Style Selector="atom|ListBoxItem:disabled">
    <Setter Property="Opacity" Value="0.5" />
</Style>
```

### 按尺寸定制

```xml
<Style Selector="atom|ListBox[SizeType=Large]">
    <Setter Property="FontSize" Value="16" />
</Style>

<Style Selector="atom|ListBox[SizeType=Small]">
    <Setter Property="FontSize" Value="12" />
</Style>
```

---

## 3. 通过 ControlTheme 完全替换主题

如需彻底替换 ListBox 或 ListBoxItem 的模板：

```xml
<ControlTheme x:Key="MyCustomListBoxItem" TargetType="atom:ListBoxItem">
    <Setter Property="Template">
        <ControlTemplate TargetType="atom:ListBoxItem">
            <Border Background="{TemplateBinding Background}"
                    Padding="{TemplateBinding Padding}"
                    CornerRadius="{TemplateBinding CornerRadius}">
                <ContentPresenter Content="{TemplateBinding Content}"
                                  ContentTemplate="{TemplateBinding ContentTemplate}" />
            </Border>
        </ControlTemplate>
    </Setter>
</ControlTheme>
```

> ⚠️ 注意：完全替换 ControlTheme 将丢失内置的选中指示器和过滤高亮功能。建议优先使用 Style 覆盖。

---

## 4. 自定义 ItemTemplate

通过 `ItemTemplate` 自定义列表项的内容布局：

```xml
<atom:ListBox>
    <atom:ListBox.ItemTemplate>
        <DataTemplate>
            <StackPanel Orientation="Horizontal" Spacing="8">
                <antdicons:UserOutlined Width="16" Height="16" />
                <atom:TextBlock Text="{Binding Name}" />
                <atom:TextBlock Text="{Binding Description}" Opacity="0.5" />
            </StackPanel>
        </DataTemplate>
    </atom:ListBox.ItemTemplate>
</atom:ListBox>
```

---

## 样式选择器速查

### ListBox 选择器

| 选择器 | 说明 |
|---|---|
| `atom\|ListBox` | 匹配所有 ListBox 实例 |
| `atom\|ListBox[SizeType=Large]` | 匹配大号 ListBox |
| `atom\|ListBox[SizeType=Middle]` | 匹配中号 ListBox |
| `atom\|ListBox[SizeType=Small]` | 匹配小号 ListBox |
| `atom\|ListBox[IsBorderless=True]` | 匹配无边框 ListBox |

### ListBoxItem 选择器

| 选择器 | 说明 |
|---|---|
| `atom\|ListBoxItem` | 匹配所有 ListBoxItem |
| `atom\|ListBoxItem:selected` | 匹配选中状态的 ListBoxItem |
| `atom\|ListBoxItem:pointerover` | 匹配鼠标悬浮的 ListBoxItem |
| `atom\|ListBoxItem:disabled` | 匹配禁用的 ListBoxItem |
| `atom\|ListBoxItem:selected:not(:disabled)` | 匹配非禁用的选中项 |

### 模板内部元素选择器

| 选择器 | 说明 |
|---|---|
| `atom\|ListBoxItem /template/ Border#Frame` | 列表项外框 |
| `atom\|ListBoxItem /template/ ContentPresenter#ContentPresenter` | 列表项内容区 |
| `atom\|ListBoxItem /template/ atom\|IconPresenter#SelectedIndicator` | 选中指示器图标 |
| `atom\|ListBox /template/ ContentPresenter#EmptyIndicator` | 空状态指示器 |
