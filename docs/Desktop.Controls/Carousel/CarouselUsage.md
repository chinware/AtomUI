# Carousel 使用文档

本文档介绍 AtomUI Carousel 控件的各种使用方式，示例代码摘自 Gallery 演示程序。

> 📖 Gallery 源码位置：`controlgallery/AtomUIGallery/ShowCases/Views/DataDisplay/CarouselShowCase.axaml`

---

## 前置准备

在 AXAML 中使用 Carousel，需要引入以下命名空间：

```xml
xmlns:atom="https://atomui.net"
xmlns:antdicons="https://atomui.net/icons/antdesign"
```

在 C# 代码中引入：

```csharp
using AtomUI.Desktop.Controls;   // Carousel, CarouselPage 等
```

---

## 1. 基本用法

最简单的走马灯，包含多个页面和分页指示器：

```xml
<atom:Carousel Height="160" Background="#364d79" Foreground="#fff" SelectedIndex="2">
    <atom:CarouselPage HorizontalContentAlignment="Center"
                       VerticalContentAlignment="Center"
                       FontWeight="Bold">1</atom:CarouselPage>
    <atom:CarouselPage HorizontalContentAlignment="Center"
                       VerticalContentAlignment="Center"
                       FontWeight="Bold">2</atom:CarouselPage>
    <atom:CarouselPage HorizontalContentAlignment="Center"
                       VerticalContentAlignment="Center"
                       FontWeight="Bold">3</atom:CarouselPage>
    <atom:CarouselPage HorizontalContentAlignment="Center"
                       VerticalContentAlignment="Center"
                       FontWeight="Bold">4</atom:CarouselPage>
</atom:Carousel>
```

**要点**：
- 通过 `SelectedIndex` 设置初始显示页面
- `CarouselPage` 是页面容器，继承自 `ContentControl`
- 分页指示器默认显示在底部

> 📖 参考：`controlgallery/AtomUIGallery/ShowCases/Views/DataDisplay/CarouselShowCase.axaml` 中 "Basic" 示例。

---

## 2. 分页位置

通过 `PaginationPosition` 控制指示器和导航箭头的位置：

```xml
<atom:Carousel PaginationPosition="Bottom" Height="160" Background="#364d79">
    <atom:CarouselPage>1</atom:CarouselPage>
    <atom:CarouselPage>2</atom:CarouselPage>
    <atom:CarouselPage>3</atom:CarouselPage>
    <atom:CarouselPage>4</atom:CarouselPage>
</atom:Carousel>
```

四种位置效果：

| 值 | 说明 | 滑动方向 |
|---|---|---|
| `Top` | 指示器在顶部，导航箭头在左右 | 水平 |
| `Bottom` | 指示器在底部，导航箭头在左右（默认） | 水平 |
| `Left` | 指示器在左侧（旋转 90°），导航箭头在上下 | 垂直 |
| `Right` | 指示器在右侧（旋转 90°），导航箭头在上下 | 垂直 |

### 配合数据绑定动态切换

```xml
<StackPanel Orientation="Vertical" Spacing="20">
    <StackPanel Orientation="Horizontal" Spacing="5">
        <atom:TextBlock VerticalAlignment="Center">Pagination Position:</atom:TextBlock>
        <atom:OptionButtonGroup ButtonStyle="Outline" Name="PositionOptionGroup">
            <atom:OptionButton>Top</atom:OptionButton>
            <atom:OptionButton IsChecked="True">Bottom</atom:OptionButton>
            <atom:OptionButton>Left</atom:OptionButton>
            <atom:OptionButton>Right</atom:OptionButton>
        </atom:OptionButtonGroup>
    </StackPanel>

    <atom:Carousel PaginationPosition="{Binding PaginationPosition}" Height="160">
        <atom:CarouselPage>1</atom:CarouselPage>
        <atom:CarouselPage>2</atom:CarouselPage>
        <atom:CarouselPage>3</atom:CarouselPage>
        <atom:CarouselPage>4</atom:CarouselPage>
    </atom:Carousel>
</StackPanel>
```

> 📖 参考：`controlgallery/AtomUIGallery/ShowCases/Views/DataDisplay/CarouselShowCase.axaml` 中 "Card Shape Position" 示例。

---

## 3. 自动播放

通过 `IsAutoPlay` 和 `AutoPlaySpeed` 控制自动播放：

```xml
<atom:Carousel IsAutoPlay="True" Height="160" Background="#364d79">
    <atom:CarouselPage>1</atom:CarouselPage>
    <atom:CarouselPage>2</atom:CarouselPage>
    <atom:CarouselPage>3</atom:CarouselPage>
    <atom:CarouselPage>4</atom:CarouselPage>
</atom:Carousel>
```

### 非无限循环 + 自动播放

```xml
<atom:Carousel IsAutoPlay="True" IsInfinite="False" Height="160">
    <atom:CarouselPage>1</atom:CarouselPage>
    <atom:CarouselPage>2</atom:CarouselPage>
    <atom:CarouselPage>3</atom:CarouselPage>
    <atom:CarouselPage>4</atom:CarouselPage>
</atom:Carousel>
```

### 自定义播放速度

```xml
<!-- 每 5 秒切换一次 -->
<atom:Carousel IsAutoPlay="True" AutoPlaySpeed="0:0:5" Height="160">
    <atom:CarouselPage>1</atom:CarouselPage>
    <atom:CarouselPage>2</atom:CarouselPage>
</atom:Carousel>
```

**行为说明**：
- 手动切换页面时，定时器自动重新计时
- 控件从可视树分离时自动停止播放
- 非无限循环模式下，播放到最后一页时停止

> 📖 参考：`controlgallery/AtomUIGallery/ShowCases/Views/DataDisplay/CarouselShowCase.axaml` 中 "Scroll automatically" 示例。

---

## 4. 淡入淡出效果

通过 `TransitionEffect="Fade"` 启用淡入淡出切换：

```xml
<atom:Carousel TransitionEffect="Fade" Height="160">
    <atom:CarouselPage Background="#B3001B">1</atom:CarouselPage>
    <atom:CarouselPage Background="#255C99">2</atom:CarouselPage>
    <atom:CarouselPage Background="#262626">3</atom:CarouselPage>
    <atom:CarouselPage Background="#CCAD8F">4</atom:CarouselPage>
</atom:Carousel>
```

**要点**：
- 淡入淡出效果不受 `PaginationPosition` 的方向影响
- 可以为每个 `CarouselPage` 设置不同的 `Background` 以增强视觉效果

> 📖 参考：`controlgallery/AtomUIGallery/ShowCases/Views/DataDisplay/CarouselShowCase.axaml` 中 "Fade in" 示例。

---

## 5. 导航箭头

通过 `IsShowNavButtons` 显示前进/后退导航按钮：

```xml
<!-- 无限循环 + 导航箭头 -->
<atom:Carousel IsShowNavButtons="True" Height="160">
    <atom:CarouselPage>1</atom:CarouselPage>
    <atom:CarouselPage>2</atom:CarouselPage>
    <atom:CarouselPage>3</atom:CarouselPage>
    <atom:CarouselPage>4</atom:CarouselPage>
</atom:Carousel>
```

### 纵向模式 + 非无限循环

```xml
<atom:Carousel PaginationPosition="Left" IsShowNavButtons="True" IsInfinite="False" Height="160">
    <atom:CarouselPage>1</atom:CarouselPage>
    <atom:CarouselPage>2</atom:CarouselPage>
    <atom:CarouselPage>3</atom:CarouselPage>
    <atom:CarouselPage>4</atom:CarouselPage>
</atom:Carousel>
```

**行为说明**：
- 无限循环模式：两个按钮始终可见
- 非无限循环模式：首页时隐藏「前一页」按钮，末页时隐藏「下一页」按钮
- 纵向模式下按钮旋转 90° 适配上下布局

> 📖 参考：`controlgallery/AtomUIGallery/ShowCases/Views/DataDisplay/CarouselShowCase.axaml` 中 "Arrows for switching" 示例。

---

## 6. 进度指示

通过 `IsShowTransitionProgress` 在指示点上显示自动播放进度：

```xml
<atom:Carousel IsShowTransitionProgress="True" IsAutoPlay="True" Height="160">
    <atom:CarouselPage>1</atom:CarouselPage>
    <atom:CarouselPage>2</atom:CarouselPage>
    <atom:CarouselPage>3</atom:CarouselPage>
    <atom:CarouselPage>4</atom:CarouselPage>
</atom:Carousel>
```

**行为说明**：
- 仅当 `IsAutoPlay = true` 时进度条生效
- 进度条从左到右填充，周期与 `AutoPlaySpeed` 一致
- 鼠标悬浮在指示点上时，进度条隐藏

> 📖 参考：`controlgallery/AtomUIGallery/ShowCases/Views/DataDisplay/CarouselShowCase.axaml` 中 "Progress of dots" 示例。

---

## 7. 滑动手势

通过 `IsSwipeEnabled` 启用鼠标/触摸滑动切换：

```xml
<atom:Carousel IsSwipeEnabled="True" Height="160">
    <atom:CarouselPage>1</atom:CarouselPage>
    <atom:CarouselPage>2</atom:CarouselPage>
    <atom:CarouselPage>3</atom:CarouselPage>
</atom:Carousel>
```

**行为说明**：
- 启用后光标变为移动样式（`DragMove`）
- 水平模式：左滑 = Next，右滑 = Previous
- 垂直模式：上滑 = Next，下滑 = Previous
- 拖动距离需超过 30px 阈值才触发切换
- 导航按钮和分页指示器区域不触发滑动

---

## 8. 编程式控制

通过 C# 代码控制走马灯：

```csharp
// 获取 Carousel 引用
var carousel = this.FindControl<Carousel>("MyCarousel");

// 切换到下一页
carousel.Next();

// 切换到上一页
carousel.Previous();

// 跳转到指定页
carousel.SelectedIndex = 2;

// 动态启用/禁用自动播放
carousel.IsAutoPlay = true;
carousel.AutoPlaySpeed = TimeSpan.FromSeconds(5);
```

---

## 9. 数据绑定

使用 `ItemsSource` 和 `ItemTemplate` 进行数据绑定：

```xml
<atom:Carousel ItemsSource="{Binding Slides}" Height="200">
    <atom:Carousel.ItemTemplate>
        <DataTemplate>
            <atom:CarouselPage Background="{Binding Color}"
                               HorizontalContentAlignment="Center"
                               VerticalContentAlignment="Center">
                <atom:TextBlock Text="{Binding Title}" FontSize="24" FontWeight="Bold" />
            </atom:CarouselPage>
        </DataTemplate>
    </atom:Carousel.ItemTemplate>
</atom:Carousel>
```

```csharp
// ViewModel
public class SlideItem
{
    public string Title { get; set; } = "";
    public IBrush Color { get; set; } = Brushes.Gray;
}

public ObservableCollection<SlideItem> Slides { get; } = new()
{
    new SlideItem { Title = "Slide 1", Color = new SolidColorBrush(Color.FromRgb(54, 77, 121)) },
    new SlideItem { Title = "Slide 2", Color = new SolidColorBrush(Color.FromRgb(179, 0, 27)) },
    new SlideItem { Title = "Slide 3", Color = new SolidColorBrush(Color.FromRgb(37, 92, 153)) },
};
```

---

## 常见组合模式

### Banner 轮播

```xml
<atom:Carousel IsAutoPlay="True" IsShowNavButtons="True" IsInfinite="True"
               Height="300" IsSwipeEnabled="True">
    <atom:CarouselPage>
        <Image Source="/Assets/banner1.png" Stretch="UniformToFill" />
    </atom:CarouselPage>
    <atom:CarouselPage>
        <Image Source="/Assets/banner2.png" Stretch="UniformToFill" />
    </atom:CarouselPage>
    <atom:CarouselPage>
        <Image Source="/Assets/banner3.png" Stretch="UniformToFill" />
    </atom:CarouselPage>
</atom:Carousel>
```

### 新手引导

```xml
<atom:Carousel IsInfinite="False" IsShowNavButtons="True"
               PaginationPosition="Bottom" Height="400">
    <atom:CarouselPage>
        <StackPanel HorizontalAlignment="Center" VerticalAlignment="Center" Spacing="16">
            <atom:TextBlock FontSize="24" FontWeight="Bold">欢迎使用</atom:TextBlock>
            <atom:TextBlock>这是第一步引导说明</atom:TextBlock>
        </StackPanel>
    </atom:CarouselPage>
    <atom:CarouselPage>
        <StackPanel HorizontalAlignment="Center" VerticalAlignment="Center" Spacing="16">
            <atom:TextBlock FontSize="24" FontWeight="Bold">功能介绍</atom:TextBlock>
            <atom:TextBlock>这是第二步引导说明</atom:TextBlock>
        </StackPanel>
    </atom:CarouselPage>
    <atom:CarouselPage>
        <StackPanel HorizontalAlignment="Center" VerticalAlignment="Center" Spacing="16">
            <atom:TextBlock FontSize="24" FontWeight="Bold">开始使用</atom:TextBlock>
            <atom:TextBlock>点击开始体验全部功能</atom:TextBlock>
        </StackPanel>
    </atom:CarouselPage>
</atom:Carousel>
```

### 自动播放 + 进度显示

```xml
<atom:Carousel IsAutoPlay="True" IsShowTransitionProgress="True"
               AutoPlaySpeed="0:0:4" IsInfinite="True" Height="200">
    <atom:CarouselPage Background="#364d79">公告一</atom:CarouselPage>
    <atom:CarouselPage Background="#5c7099">公告二</atom:CarouselPage>
    <atom:CarouselPage Background="#7b90b3">公告三</atom:CarouselPage>
</atom:Carousel>
```
