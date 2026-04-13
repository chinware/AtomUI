# Tag 自定义样式指南

Tag 的视觉表现通过 `ControlTheme` + Design Token 系统控制。以下介绍几种常见的自定义方式。

---

## 1. 通过属性直接控制

最简单的方式是通过 Tag 的公共属性来控制外观：

```xml
<!-- 默认标签 -->
<atom:Tag>Tag 1</atom:Tag>

<!-- 预设颜色 -->
<atom:Tag TagColor="blue">Blue</atom:Tag>
<atom:Tag TagColor="green">Green</atom:Tag>
<atom:Tag TagColor="red">Red</atom:Tag>

<!-- 状态颜色 -->
<atom:Tag TagColor="success">Success</atom:Tag>
<atom:Tag TagColor="warning">Warning</atom:Tag>
<atom:Tag TagColor="error">Error</atom:Tag>

<!-- 自定义颜色 -->
<atom:Tag TagColor="#f50">#f50</atom:Tag>
<atom:Tag TagColor="#2db7f5">#2db7f5</atom:Tag>
<atom:Tag TagColor="#87d068">#87d068</atom:Tag>

<!-- 可关闭标签 -->
<atom:Tag IsClosable="True">Closable</atom:Tag>

<!-- 带图标标签 -->
<atom:Tag Icon="{antdicons:AntDesignIconProvider Kind=CheckCircleOutlined}"
         TagColor="success">
    Success
</atom:Tag>

<!-- 无边框标签 -->
<atom:Tag Bordered="False" TagColor="blue">No Border</atom:Tag>
```

---

## 2. 通过 Style 覆盖样式

### 全局统一间距

```xml
<Window.Styles>
    <Style Selector="atom|Tag">
        <Setter Property="Margin" Value="0,0,8,8" />
    </Style>
</Window.Styles>
```

### 按颜色模式定制

```xml
<!-- 预设颜色标签加粗 -->
<Style Selector="atom|Tag:preset-color">
    <Setter Property="FontWeight" Value="SemiBold" />
</Style>

<!-- 状态颜色标签加大字号 -->
<Style Selector="atom|Tag:status-color">
    <Setter Property="FontSize" Value="14" />
</Style>

<!-- 自定义颜色标签加圆角 -->
<Style Selector="atom|Tag:custom-color">
    <Setter Property="CornerRadius" Value="12" />
</Style>
```

### 关闭按钮样式

```xml
<!-- 可关闭标签的关闭按钮额外间距 -->
<Style Selector="atom|Tag[IsClosable=True] /template/ atom|IconButton#PART_CloseButton">
    <Setter Property="Margin" Value="4,0,0,0" />
</Style>
```

---

## 3. 通过 ControlTheme 完全替换主题

```xml
<ControlTheme x:Key="MyCustomTag" TargetType="atom:Tag">
    <Setter Property="Template">
        <ControlTemplate>
            <Border Background="{TemplateBinding Background}"
                    CornerRadius="{TemplateBinding CornerRadius}"
                    BorderBrush="{TemplateBinding BorderBrush}"
                    BorderThickness="{TemplateBinding BorderThickness}"
                    Padding="{TemplateBinding Padding}">
                <TextBlock Text="{TemplateBinding TagText}"
                           VerticalAlignment="Center" />
            </Border>
        </ControlTemplate>
    </Setter>
</ControlTheme>

<atom:Tag Theme="{StaticResource MyCustomTag}">Custom</atom:Tag>
```

> ⚠️ 注意：完全替换 ControlTheme 将丢失内置的图标、关闭按钮等功能。建议优先使用 Style 覆盖。

---

## 4. 处理关闭事件

```xml
<atom:Tag IsClosable="True" Closed="HandleTagClosed">Closable Tag</atom:Tag>
```

```csharp
private void HandleTagClosed(object? sender, RoutedEventArgs e)
{
    if (sender is Tag tag)
    {
        // 从父容器中移除标签
        if (tag.Parent is Panel panel)
        {
            panel.Children.Remove(tag);
        }
    }
}
```

---

## 样式选择器速查

> 说明：Avalonia 样式选择器中使用 `atom|Tag` 语法引用 `atom` XML 命名空间下的 `Tag` 类型，其中 `|` 是命名空间分隔符。

### 基础选择器

| 选择器 | 说明 |
|---|---|
| `atom\|Tag` | 匹配所有 Tag 实例 |

### 按颜色模式选择

| 选择器 | 说明 |
|---|---|
| `atom\|Tag:preset-color` | 匹配使用预设颜色的标签（13 种调色板颜色） |
| `atom\|Tag:status-color` | 匹配使用状态颜色的标签（success/info/warning/error） |
| `atom\|Tag:custom-color` | 匹配使用自定义 CSS 颜色的标签 |

### 按属性选择

| 选择器 | 说明 |
|---|---|
| `atom\|Tag[IsClosable=True]` | 匹配可关闭标签 |
| `atom\|Tag[Bordered=False]` | 匹配无边框标签 |

### 按状态伪类选择

| 选择器 | 说明 |
|---|---|
| `atom\|Tag:pointerover` | 鼠标悬浮 |
| `atom\|Tag:disabled` | 禁用 |

### 组合选择器示例

| 选择器 | 说明 |
|---|---|
| `atom\|Tag:preset-color:not(:disabled)` | 非禁用的预设颜色标签 |
| `atom\|Tag[IsClosable=True] /template/ atom\|IconButton#PART_CloseButton` | 关闭按钮部件 |
| `atom\|Tag /template/ atom\|IconPresenter#IconPresenter` | 图标展示器部件 |
| `atom\|Tag /template/ atom\|TextBlock#TagTextLabel` | 文字标签部件 |
| `atom\|Tag /template/ Border#Frame` | 背景边框部件 |
