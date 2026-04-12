# Carousel 自定义样式指南

Carousel 的视觉表现通过 `ControlTheme` + Design Token 系统控制。以下介绍几种常见的自定义方式。

---

## 1. 使用属性直接控制

最简单的方式是通过 Carousel 的公共属性来控制行为和外观：

```xml
<!-- 基本走马灯 -->
<atom:Carousel Height="160" Background="#364d79">
    <atom:CarouselPage>1</atom:CarouselPage>
    <atom:CarouselPage>2</atom:CarouselPage>
    <atom:CarouselPage>3</atom:CarouselPage>
    <atom:CarouselPage>4</atom:CarouselPage>
</atom:Carousel>

<!-- 自动播放 -->
<atom:Carousel IsAutoPlay="True" AutoPlaySpeed="0:0:5" Height="160">
    <atom:CarouselPage>Slide 1</atom:CarouselPage>
    <atom:CarouselPage>Slide 2</atom:CarouselPage>
</atom:Carousel>

<!-- 淡入淡出效果 -->
<atom:Carousel TransitionEffect="Fade" Height="160">
    <atom:CarouselPage Background="#B3001B">1</atom:CarouselPage>
    <atom:CarouselPage Background="#255C99">2</atom:CarouselPage>
</atom:Carousel>

<!-- 显示导航箭头 -->
<atom:Carousel IsShowNavButtons="True" Height="160">
    <atom:CarouselPage>1</atom:CarouselPage>
    <atom:CarouselPage>2</atom:CarouselPage>
</atom:Carousel>

<!-- 分页位置在左侧 -->
<atom:Carousel PaginationPosition="Left" Height="160">
    <atom:CarouselPage>1</atom:CarouselPage>
    <atom:CarouselPage>2</atom:CarouselPage>
</atom:Carousel>

<!-- 进度指示 + 自动播放 -->
<atom:Carousel IsAutoPlay="True" IsShowTransitionProgress="True" Height="160">
    <atom:CarouselPage>1</atom:CarouselPage>
    <atom:CarouselPage>2</atom:CarouselPage>
</atom:Carousel>
```

> 📖 完整示例参考：`controlgallery/AtomUIGallery/ShowCases/Views/DataDisplay/CarouselShowCase.axaml`

---

## 2. 通过 Style 覆盖样式

可以在 AXAML 中使用 `Style` 对 Carousel 及其子控件进行统一样式定制：

### 统一背景和文字

```xml
<Window.Styles>
    <Style Selector="atom|Carousel">
        <Setter Property="Background" Value="#364d79" />
        <Setter Property="Foreground" Value="#fff" />
        <Setter Property="Height" Value="160" />
    </Style>
    <Style Selector="atom|CarouselPage">
        <Setter Property="HorizontalContentAlignment" Value="Center" />
        <Setter Property="VerticalContentAlignment" Value="Center" />
        <Setter Property="FontWeight" Value="Bold" />
    </Style>
</Window.Styles>
```

### 自定义导航箭头样式

```xml
<!-- 增大导航箭头图标，并调整不透明度 -->
<Style Selector="atom|Carousel /template/ atom|CarouselNavButton">
    <Setter Property="IconWidth" Value="24" />
    <Setter Property="IconHeight" Value="24" />
    <Setter Property="Opacity" Value="0.5" />
</Style>

<Style Selector="atom|Carousel /template/ atom|CarouselNavButton:pointerover">
    <Setter Property="Opacity" Value="1.0" />
</Style>
```

### 自定义指示点颜色

```xml
<!-- 使用蓝色指示点 -->
<Style Selector="atom|CarouselPageIndicator">
    <Setter Property="Background" Value="#1677ff" />
</Style>
```

### 按分页位置定制

```xml
<!-- 纵向模式下增加导航按钮边距 -->
<Style Selector="atom|Carousel[PaginationPosition=Left]">
    <Setter Property="Padding" Value="30 0 0 0" />
</Style>
```

---

## 3. 通过 ControlTheme 完全替换主题

如果需要彻底替换 Carousel 的模板，可以定义自定义 `ControlTheme`：

```xml
<ControlTheme x:Key="MyCustomCarousel" TargetType="atom:Carousel">
    <Setter Property="Template">
        <ControlTemplate>
            <Panel>
                <atom:ScrollViewer Name="PART_ScrollViewer"
                                   Background="{TemplateBinding Background}"
                                   HorizontalScrollBarVisibility="Hidden"
                                   VerticalScrollBarVisibility="Hidden">
                    <ItemsPresenter Name="PART_ItemsPresenter"
                                    ItemsPanel="{TemplateBinding ItemsPanel}" />
                </atom:ScrollViewer>
                <!-- 自定义分页指示器或导航按钮 -->
            </Panel>
        </ControlTemplate>
    </Setter>
</ControlTheme>
```

> ⚠️ 注意：完全替换 ControlTheme 将丢失内置的分页指示器、导航按钮、进度动画等功能。建议优先使用 Style 覆盖。

---

## 4. 控制动画行为

```xml
<!-- 禁用过渡动画（页面切换无动画） -->
<atom:Carousel IsMotionEnabled="False" Height="160">
    <atom:CarouselPage>1</atom:CarouselPage>
    <atom:CarouselPage>2</atom:CarouselPage>
</atom:Carousel>

<!-- 自定义过渡时长和缓动 -->
<atom:Carousel PageTransitionDuration="0:0:1"
               PageInEasing="QuadraticEaseInOut"
               PageOutEasing="QuadraticEaseInOut"
               Height="160">
    <atom:CarouselPage>1</atom:CarouselPage>
    <atom:CarouselPage>2</atom:CarouselPage>
</atom:Carousel>
```

---

## 样式选择器速查

> 说明：Avalonia 样式选择器中使用 `atom|Carousel` 语法引用 `atom` XML 命名空间下的类型。

### 按属性选择

| 选择器 | 说明 |
|---|---|
| `atom\|Carousel` | 匹配所有 Carousel 实例 |
| `atom\|Carousel[IsAutoPlay=True]` | 匹配自动播放的走马灯 |
| `atom\|Carousel[IsShowNavButtons=True]` | 匹配显示导航箭头的走马灯 |
| `atom\|Carousel[TransitionEffect=Fade]` | 匹配淡入淡出效果的走马灯 |

### 按分页位置选择

| 选择器 | 说明 |
|---|---|
| `atom\|Carousel[PaginationPosition=Top]` | 分页在顶部 |
| `atom\|Carousel[PaginationPosition=Bottom]` | 分页在底部（默认） |
| `atom\|Carousel[PaginationPosition=Left]` | 分页在左侧 |
| `atom\|Carousel[PaginationPosition=Right]` | 分页在右侧 |

### 按伪类选择

| 选择器 | 说明 |
|---|---|
| `atom\|Carousel:pointerover` | 鼠标悬浮 |
| `atom\|Carousel:disabled` | 禁用状态 |

### 子控件选择器

| 选择器 | 说明 |
|---|---|
| `atom\|CarouselPage` | 匹配所有页面容器 |
| `atom\|CarouselNavButton` | 匹配所有导航箭头按钮 |
| `atom\|CarouselNavButton:pointerover` | 悬浮态导航按钮（不透明度 1.0） |
| `atom\|CarouselPageIndicator` | 匹配所有分页指示点 |
| `atom\|CarouselPageIndicator[IsSelected=True]` | 选中态指示点（宽度扩展） |
| `atom\|CarouselPageIndicator:pointerover` | 悬浮态指示点（不透明度 0.75） |

### 模板内部部件选择器

| 选择器 | 说明 |
|---|---|
| `atom\|Carousel /template/ atom\|CarouselNavButton#PART_PreviousButton` | 前一页按钮 |
| `atom\|Carousel /template/ atom\|CarouselNavButton#PART_NextButton` | 下一页按钮 |
| `atom\|Carousel /template/ LayoutTransformControl#PaginationLayoutTransform` | 分页指示器布局容器 |
| `atom\|CarouselPageIndicator /template/ Border#PART_Frame` | 指示点主框架 |
| `atom\|CarouselPageIndicator /template/ Border#Progress` | 指示点进度条 |
