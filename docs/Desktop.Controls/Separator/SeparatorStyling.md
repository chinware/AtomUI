# Separator 自定义样式指南

Separator 的视觉表现通过 `ControlTheme` + Design Token 系统控制，同时支持通过属性和 `Style` 进行灵活的自定义。由于 Separator 使用自绘渲染（`OnRender`），其线条样式通过属性而非模板内控件来控制。

---

## 1. 使用属性直接控制

最简单的方式是通过 Separator 的公共属性来控制外观：

```xml
<!-- 基本水平分割线 -->
<atom:Separator />

<!-- 带标题（居中） -->
<atom:Separator Title="Section Title" />

<!-- 标题靠左 -->
<atom:Separator Title="Left Title" TitlePosition="Left" />

<!-- 标题靠右 -->
<atom:Separator Title="Right Title" TitlePosition="Right" />

<!-- 自定义标题边距 -->
<atom:Separator Title="Left Text with 0 orientationMargin"
                 TitlePosition="Left" OrientationMargin="0" />
<atom:Separator Title="Right Text with 50px orientationMargin"
                 TitlePosition="Right" OrientationMargin="50" />

<!-- 线条变体 -->
<atom:Separator Variant="Dashed" />
<atom:Separator Variant="Dotted" />

<!-- 普通文字样式（非标题加粗） -->
<atom:Separator Title="Plain Text" IsPlain="True" />

<!-- 自定义线条颜色 -->
<atom:Separator Title="Green Line" LineColor="#7cb305" />

<!-- 自定义标题颜色 -->
<atom:Separator Title="Coral Title" TitleColor="Coral" />

<!-- 同时自定义线条和标题颜色 -->
<atom:Separator Title="Custom Colors" LineColor="#7cb305" TitleColor="Coral" />

<!-- 不同尺寸 -->
<atom:Separator SizeType="Small" />
<atom:Separator SizeType="Large" />

<!-- 垂直分割线 -->
<atom:VerticalSeparator />
<atom:Separator Orientation="Vertical" />

<!-- 自定义字体样式 -->
<atom:Separator Title="Italic Title" FontStyle="Italic" />
<atom:Separator Title="Bold Title" FontWeight="Bold" />
```

> 📖 完整示例参考：`controlgallery/AtomUIGallery/ShowCases/Views/General/SeparatorShowCase.axaml`

---

## 2. 通过 Style 覆盖样式

可以在 AXAML 中使用 `Style` 对 Separator 进行全局或局部样式覆盖：

### 统一所有分割线为虚线

```xml
<Window.Styles>
    <Style Selector="atom|Separator">
        <Setter Property="Variant" Value="Dashed" />
    </Style>
</Window.Styles>
```

### 全局统一分割线颜色

```xml
<Window.Styles>
    <Style Selector="atom|Separator">
        <Setter Property="LineColor" Value="#d9d9d9" />
    </Style>
</Window.Styles>
```

### 自定义带标题分割线的字体

```xml
<Window.Styles>
    <Style Selector="atom|Separator:has-title">
        <Setter Property="FontStyle" Value="Italic" />
        <Setter Property="FontWeight" Value="Bold" />
    </Style>
</Window.Styles>
```

### 自定义无标题分割线的间距

```xml
<Window.Styles>
    <Style Selector="atom|Separator:not(:has-title)">
        <Setter Property="Margin" Value="0,16" />
    </Style>
</Window.Styles>
```

### 按方向区分样式

```xml
<Window.Styles>
    <!-- 水平分割线加大间距 -->
    <Style Selector="atom|Separator[Orientation=Horizontal]">
        <Setter Property="Margin" Value="0,20" />
    </Style>
    
    <!-- 垂直分割线自定义颜色 -->
    <Style Selector="atom|Separator[Orientation=Vertical]">
        <Setter Property="LineColor" Value="#bfbfbf" />
    </Style>
</Window.Styles>
```

### 使用伪类选择器

```xml
<!-- 有标题的分割线使用自定义标题颜色 -->
<Style Selector="atom|Separator:has-title">
    <Setter Property="TitleColor" Value="#1677ff" />
</Style>

<!-- 小号分割线使用点线 -->
<Style Selector="atom|Separator[SizeType=Small]">
    <Setter Property="Variant" Value="Dotted" />
</Style>
```

### 通过属性选择器定制变体

```xml
<!-- 虚线分割线使用红色 -->
<Style Selector="atom|Separator[Variant=Dashed]">
    <Setter Property="LineColor" Value="#ff4d4f" />
</Style>

<!-- 普通文字模式自定义样式 -->
<Style Selector="atom|Separator[IsPlain=True]">
    <Setter Property="TitleColor" Value="#8c8c8c" />
</Style>
```

---

## 3. 通过 ControlTheme 完全替换主题

如果需要彻底替换 Separator 的模板和样式，可以定义自定义 `ControlTheme`：

```xml
<ControlTheme x:Key="MyCustomSeparator" TargetType="atom:Separator">
    <Setter Property="Template">
        <ControlTemplate>
            <atom:TextBlock Name="PART_Title"
                   HorizontalAlignment="Left"
                   VerticalAlignment="Center"
                   Text="{TemplateBinding Title}"
                   FontSize="{TemplateBinding FontSize}"
                   Foreground="{TemplateBinding TitleColor}" />
        </ControlTemplate>
    </Setter>
    <Setter Property="LineColor" Value="#d9d9d9" />
    <Setter Property="TitleColor" Value="#262626" />
</ControlTheme>

<!-- 使用 -->
<atom:Separator Theme="{StaticResource MyCustomSeparator}" Title="Custom Theme" />
```

> ⚠️ 注意：完全替换 ControlTheme 时，请确保保留 `PART_Title` 模板部件名称，否则标题功能将无法正常工作。Separator 的线条绘制通过 `OnRender` 自绘完成，不依赖模板中的控件。

---

## 4. 尺寸控制

通过 `SizeType` 控制水平分割线的垂直间距（上下 Margin）：

```xml
<StackPanel Orientation="Vertical">
    <atom:TextBlock>Content...</atom:TextBlock>
    <atom:Separator SizeType="Small" />   <!-- 紧凑间距 -->
    <atom:TextBlock>Content...</atom:TextBlock>
    <atom:Separator SizeType="Middle" />  <!-- 标准间距（默认） -->
    <atom:TextBlock>Content...</atom:TextBlock>
    <atom:Separator SizeType="Large" />   <!-- 宽松间距 -->
    <atom:TextBlock>Content...</atom:TextBlock>
</StackPanel>
```

尺寸也支持数据绑定，可在运行时动态切换：

```xml
<atom:Separator SizeType="{Binding SeparatorSize}" />
```

---

## 5. 标题样式控制

### IsPlain 模式对比

```xml
<!-- 标题模式（默认）：大字号 + 加粗 -->
<atom:Separator Title="Heading Style" />

<!-- 普通文字模式：正常字号 + 正常字重 -->
<atom:Separator Title="Plain Style" IsPlain="True" />
```

### 字体属性覆盖

即使不使用 `IsPlain`，也可以直接覆盖字体属性：

```xml
<atom:Separator Title="Custom Font"
                 FontStyle="Italic"
                 FontWeight="Medium" />
```

---

## 样式选择器速查

> 说明：Avalonia 样式选择器中使用 `atom|Separator` 语法引用 `atom` XML 命名空间下的 `Separator` 类型，其中 `|` 是命名空间分隔符。

### 按控件类型选择

| 选择器 | 说明 |
|---|---|
| `atom\|Separator` | 匹配所有 AtomUI Separator 实例（包括 VerticalSeparator），用于设置全局通用样式 |
| `atom\|VerticalSeparator` | 仅匹配 VerticalSeparator 实例（Separator 的便捷子类） |

### 按方向选择

| 选择器 | 说明 |
|---|---|
| `atom\|Separator[Orientation=Horizontal]` | 匹配水平分割线，可定制水平方向特有的间距、标题样式 |
| `atom\|Separator[Orientation=Vertical]` | 匹配垂直分割线，可定制垂直方向的颜色、宽度 |

### 按线条变体选择

| 选择器 | 说明 |
|---|---|
| `atom\|Separator[Variant=Solid]` | 匹配实线分割线（默认） |
| `atom\|Separator[Variant=Dashed]` | 匹配虚线分割线 |
| `atom\|Separator[Variant=Dotted]` | 匹配点线分割线 |

### 按尺寸选择

| 选择器 | 说明 |
|---|---|
| `atom\|Separator[SizeType=Small]` | 匹配小号分割线（紧凑间距） |
| `atom\|Separator[SizeType=Middle]` | 匹配中号分割线（标准间距，默认） |
| `atom\|Separator[SizeType=Large]` | 匹配大号分割线（宽松间距） |

### 按属性选择

| 选择器 | 说明 |
|---|---|
| `atom\|Separator[IsPlain=True]` | 匹配普通文字样式的分割线 |
| `atom\|Separator[IsPlain=False]` | 匹配标题样式的分割线（默认） |
| `atom\|Separator[TitlePosition=Left]` | 匹配标题靠左的分割线 |
| `atom\|Separator[TitlePosition=Right]` | 匹配标题靠右的分割线 |
| `atom\|Separator[TitlePosition=Center]` | 匹配标题居中的分割线（默认） |

### 按伪类选择

| 选择器 | 说明 |
|---|---|
| `atom\|Separator:has-title` | 匹配设置了标题文本的分割线，可用于定制有标题时的专属样式 |
| `atom\|Separator:not(:has-title)` | 匹配无标题的分割线，可用于定制纯线条模式的样式 |
| `atom\|Separator:disabled` | 匹配禁用状态的分割线 |

### 模板部件选择器

| 选择器 | 说明 |
|---|---|
| `atom\|Separator /template/ atom\|TextBlock#PART_Title` | 访问模板内的标题 TextBlock 部件，可精细控制标题样式 |

### 组合选择器示例

| 选择器 | 说明 |
|---|---|
| `atom\|Separator[Orientation=Horizontal]:has-title` | 水平方向且有标题的分割线 |
| `atom\|Separator[Orientation=Horizontal]:not(:has-title)[SizeType=Small]` | 无标题的小号水平分割线 |
| `atom\|Separator:has-title[TitlePosition=Left]` | 标题靠左的带标题分割线 |
| `atom\|Separator[Variant=Dashed]:has-title` | 虚线样式的带标题分割线 |
| `atom\|Separator:has-title[IsPlain=True]` | 使用普通文字样式的带标题分割线 |
