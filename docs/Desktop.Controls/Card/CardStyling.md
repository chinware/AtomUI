# Card 自定义样式指南

Card 的视觉表现通过 `ControlTheme` + Design Token 系统控制。以下介绍几种常见的自定义方式。

---

## 1. 使用属性直接控制

最简单的方式是通过 Card 的公共属性来控制外观：

```xml
<!-- 不同风格 -->
<atom:Card Header="Card Title">
    <atom:TextBlock>Default card with border</atom:TextBlock>
</atom:Card>

<atom:Card Header="Card Title" StyleVariant="Borderless">
    <atom:TextBlock>Borderless card with shadow</atom:TextBlock>
</atom:Card>

<!-- 不同尺寸 -->
<atom:Card Header="Large Card" SizeType="Large" Width="300">
    <atom:TextBlock>Large card content</atom:TextBlock>
</atom:Card>
<atom:Card Header="Default Card" SizeType="Middle" Width="300">
    <atom:TextBlock>Default card content</atom:TextBlock>
</atom:Card>
<atom:Card Header="Small Card" SizeType="Small" Width="300">
    <atom:TextBlock>Small card content</atom:TextBlock>
</atom:Card>

<!-- 悬浮阴影 -->
<atom:Card Header="Hoverable" IsHoverable="True" Width="300">
    <atom:TextBlock>Hover me to see shadow effect!</atom:TextBlock>
</atom:Card>

<!-- 内嵌模式 -->
<atom:Card Header="Outer Card" SizeType="Large">
    <StackPanel Spacing="20">
        <atom:Card Header="Inner Card 1" IsInnerMode="True">
            <atom:TextBlock>Inner card content</atom:TextBlock>
        </atom:Card>
        <atom:Card Header="Inner Card 2" IsInnerMode="True">
            <atom:TextBlock>Inner card content</atom:TextBlock>
        </atom:Card>
    </StackPanel>
</atom:Card>

<!-- 带封面和操作区 -->
<atom:Card Width="300" IsHoverable="True">
    <atom:Card.Cover>
        <Image Source="/Assets/CardShowCase/Cover1.png" />
    </atom:Card.Cover>
    <atom:CardMetaContent Header="Europe Street beat"
                          Content="www.instagram.com" />
    <atom:Card.Actions>
        <atom:CardActionButton Icon="{antdicons:AntDesignIconProvider Kind=EditOutlined}" />
        <atom:CardActionButton Icon="{antdicons:AntDesignIconProvider Kind=SettingOutlined}" />
    </atom:Card.Actions>
</atom:Card>

<!-- 加载状态 -->
<atom:Card Header="Loading Card" IsLoading="True" Width="300">
    <atom:TextBlock>This content is hidden during loading</atom:TextBlock>
</atom:Card>
```

> 📖 完整示例参考：`controlgallery/AtomUIGallery/ShowCases/Views/DataDisplay/CardShowCase.axaml`

---

## 2. 通过 Style 覆盖样式

可以在 AXAML 中使用 `Style` 对 Card 进行全局或局部样式覆盖：

### 全局统一间距

```xml
<Window.Styles>
    <Style Selector="atom|Card">
        <Setter Property="Margin" Value="10" />
    </Style>
</Window.Styles>
```

### 按风格定制

```xml
<!-- 所有无边框卡片添加自定义阴影 -->
<Style Selector="atom|Card[StyleVariant=Borderless]">
    <Setter Property="BoxShadow" Value="0 2 8 0 #33000000" />
</Style>
```

### 使用伪类选择器

```xml
<!-- 无标题卡片的自定义边框 -->
<Style Selector="atom|Card:headerless">
    <Setter Property="BorderBrush" Value="LightGray" />
</Style>

<!-- 禁用态的自定义透明度 -->
<Style Selector="atom|Card:disabled">
    <Setter Property="Opacity" Value="0.5" />
</Style>
```

### 按属性选择器定制

```xml
<!-- 所有小号卡片使用紧凑内边距 -->
<Style Selector="atom|Card[SizeType=Small]">
    <Setter Property="Margin" Value="4" />
</Style>

<!-- 所有悬浮卡片添加手形光标 -->
<Style Selector="atom|Card[IsHoverable=True]">
    <Setter Property="Cursor" Value="Hand" />
</Style>

<!-- 内嵌模式卡片的额外样式 -->
<Style Selector="atom|Card[IsInnerMode=True]">
    <Setter Property="Margin" Value="0 0 0 16" />
</Style>
```

### 访问模板内部部件

```xml
<!-- 自定义头部区域边框颜色 -->
<Style Selector="atom|Card /template/ Border#HeaderFrame">
    <Setter Property="BorderBrush" Value="#e8e8e8" />
</Style>

<!-- 自定义内容区域内边距 -->
<Style Selector="atom|Card /template/ Border#CardContent">
    <Setter Property="Padding" Value="24" />
</Style>
```

---

## 3. 通过 ControlTheme 完全替换主题

如果需要彻底替换 Card 的模板和样式，可以定义自定义 `ControlTheme`：

```xml
<ControlTheme x:Key="MyCustomCard" TargetType="atom:Card">
    <Setter Property="Template">
        <ControlTemplate>
            <Border Background="{TemplateBinding Background}"
                    CornerRadius="{TemplateBinding CornerRadius}"
                    BorderBrush="{TemplateBinding BorderBrush}"
                    BorderThickness="{TemplateBinding BorderThickness}"
                    Padding="{TemplateBinding Padding}">
                <DockPanel LastChildFill="True">
                    <ContentPresenter DockPanel.Dock="Top"
                                      Content="{TemplateBinding Header}"
                                      ContentTemplate="{TemplateBinding HeaderTemplate}" />
                    <ContentPresenter Content="{TemplateBinding Content}"
                                      ContentTemplate="{TemplateBinding ContentTemplate}" />
                </DockPanel>
            </Border>
        </ControlTemplate>
    </Setter>
</ControlTheme>

<!-- 使用 -->
<atom:Card Theme="{StaticResource MyCustomCard}" Header="Custom Card">
    <atom:TextBlock>Custom card content</atom:TextBlock>
</atom:Card>
```

> ⚠️ 注意：完全替换 ControlTheme 将丢失内置的骨架屏加载、操作区面板、封面圆角裁剪等功能。建议优先使用 Style 覆盖。

---

## 4. 控制动画行为

```xml
<!-- 禁用过渡动画（BoxShadow 不再渐变过渡） -->
<atom:Card Header="No Animation" IsMotionEnabled="False" IsHoverable="True">
    <atom:TextBlock>Shadow changes instantly</atom:TextBlock>
</atom:Card>
```

---

## 样式选择器速查

> 说明：Avalonia 样式选择器中使用 `atom|Card` 语法引用 `atom` XML 命名空间下的 `Card` 类型，其中 `|` 是命名空间分隔符。

### 按风格选择

| 选择器 | 说明 |
|---|---|
| `atom\|Card` | 匹配所有 AtomUI Card 实例 |
| `atom\|Card[StyleVariant=Outline]` | 匹配有边框的卡片（默认风格） |
| `atom\|Card[StyleVariant=Borderless]` | 匹配无边框的卡片（使用阴影替代边框） |

### 按尺寸选择

| 选择器 | 说明 |
|---|---|
| `atom\|Card[SizeType=Large]` | 匹配大号卡片 |
| `atom\|Card[SizeType=Middle]` | 匹配中号卡片（默认） |
| `atom\|Card[SizeType=Small]` | 匹配小号卡片 |

### 按状态属性选择

| 选择器 | 说明 |
|---|---|
| `atom\|Card[IsHoverable=True]` | 匹配启用悬浮阴影的卡片 |
| `atom\|Card[IsInnerMode=True]` | 匹配内嵌模式的卡片 |
| `atom\|Card[IsLoading=True]` | 匹配加载中状态的卡片 |

### 按伪类选择

| 选择器 | 说明 |
|---|---|
| `atom\|Card:headerless` | 匹配无标题区域的卡片（Header / Extra 均为 null） |
| `atom\|Card:pointerover` | 匹配鼠标悬浮的卡片 |
| `atom\|Card:disabled` | 匹配禁用状态的卡片 |

### 组合选择器示例

| 选择器 | 说明 |
|---|---|
| `atom\|Card[IsHoverable=True]:pointerover` | 悬浮态的可交互卡片（阴影加深状态） |
| `atom\|Card[SizeType=Large]:headerless` | 大号无标题卡片 |
| `atom\|Card[StyleVariant=Borderless][IsInnerMode=True]` | 无边框的内嵌卡片 |
| `atom\|Card /template/ Border#Frame` | 访问卡片模板内的主框架 Border |
| `atom\|Card /template/ Border#HeaderFrame` | 访问卡片模板内的头部区域 |
| `atom\|Card /template/ Border#CardContent` | 访问卡片模板内的内容区域 |

### 相关控件选择器

| 选择器 | 说明 |
|---|---|
| `atom\|CardMetaContent` | 匹配所有 CardMetaContent 实例 |
| `atom\|CardGridItem` | 匹配所有网格单元格 |
| `atom\|CardGridItem[IsHoverable=True]:pointerover` | 匹配悬浮态的网格单元格 |
| `atom\|CardActionButton` | 匹配所有操作区按钮 |
| `atom\|CardActionButton:pointerover` | 匹配悬浮态的操作区按钮（图标变主色） |
