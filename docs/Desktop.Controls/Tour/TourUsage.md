# Tour 使用文档

本文档介绍 AtomUI Tour 控件的各种使用方式，示例代码摘自 Gallery 演示程序。

> 📖 Gallery 源码位置：`controlgallery/AtomUIGallery/ShowCases/Views/DataDisplay/TourShowCase.axaml`

---

## 前置准备

在 AXAML 中使用 Tour，需要引入以下命名空间：

```xml
xmlns:atom="https://atomui.net"
xmlns:antdicons="https://atomui.net/icons/antdesign"
```

在 C# 代码中引入：

```csharp
using AtomUI.Desktop.Controls;   // Tour, TourStep, TourStyleType 等
using AtomUI.Controls;            // IMotionAwareControl 等共享类型
```

---

## 1. 基本用法

最基本的 Tour 使用方式：定义目标控件 → 声明 TourStep 步骤 → 通过 `IsOpen` 控制显隐。

```xml
<StackPanel Orientation="Vertical" Spacing="10">
    <!-- 触发按钮 -->
    <atom:Button ButtonType="Primary" Click="HandleBeginTour">Begin Tour</atom:Button>
    <atom:Separator />

    <!-- 目标控件 -->
    <StackPanel Orientation="Horizontal" Spacing="10">
        <atom:Button Name="Upload">Upload</atom:Button>
        <atom:Button ButtonType="Primary" Name="Save">Save</atom:Button>
        <atom:Button Name="Ellipsis"
                     Icon="{antdicons:AntDesignIconProvider EllipsisOutlined}" />
    </StackPanel>

    <!-- Tour 定义 -->
    <atom:Tour IsOpen="{Binding TourOpened, Mode=TwoWay}">
        <atom:TourStep Target="{Binding ElementName=Upload}"
                       Title="Upload File"
                       Description="Put your files here.">
            <atom:TourStep.Cover>
                <Image Source="/Assets/TourShowCase/Cover.png" Width="400" />
            </atom:TourStep.Cover>
        </atom:TourStep>
        <atom:TourStep Target="{Binding ElementName=Save}"
                       Title="Save"
                       Description="Save your changes." />
        <atom:TourStep Target="{Binding ElementName=Ellipsis}"
                       Title="Other Actions"
                       Description="Click to see other actions." />
    </atom:Tour>
</StackPanel>
```

```csharp
// Code-behind
private void HandleBeginTour(object? sender, RoutedEventArgs args)
{
    if (DataContext is MyViewModel vm)
    {
        vm.TourOpened = !vm.TourOpened;
    }
}
```

**要点**：
- `Tour.Steps` 是 Content 属性，`TourStep` 子元素直接写在 `<atom:Tour>` 内
- `Target` 通过 `{Binding ElementName=...}` 引用页面上的控件
- `IsOpen` 支持双向绑定，Tour 关闭时自动设回 `false`
- 每个步骤可附带 `Cover`（封面图）、`Title`（标题）和 `Description`（描述）

---

## 2. 非模态引导（无遮罩）

设置 `IsShowMask="False"` 可以去除遮罩层，适合不打断用户操作的轻量引导。建议配合 `StyleType="Primary"` 增强引导卡片的醒目度：

```xml
<atom:Tour IsOpen="{Binding NonMaskTourOpened, Mode=TwoWay}"
           IsShowMask="False"
           StyleType="Primary">
    <atom:TourStep Target="{Binding ElementName=Upload}"
                   Title="Upload File"
                   Description="Put your files here.">
        <atom:TourStep.Cover>
            <Image Source="/Assets/TourShowCase/Cover.png" Width="400" />
        </atom:TourStep.Cover>
    </atom:TourStep>
    <atom:TourStep Target="{Binding ElementName=Save}"
                   Title="Save"
                   Description="Save your changes." />
    <atom:TourStep Target="{Binding ElementName=Ellipsis}"
                   Title="Other Actions"
                   Description="Click to see other actions." />
</atom:Tour>
```

---

## 3. 弹出位置

通过 `Placement` 属性控制气泡卡片的弹出方向，支持 13 种位置。当 `Target` 为空时，无论 Placement 设置如何，卡片都会居中显示：

```xml
<atom:Tour IsOpen="{Binding PlacementTourOpened, Mode=TwoWay}">
    <!-- 无 Target → 自动居中 -->
    <atom:TourStep Title="Center"
                   Description="Displayed in the center of screen." />
    <!-- 目标右侧 -->
    <atom:TourStep Title="Right"
                   Description="On the right of target."
                   Placement="Right"
                   Target="{Binding ElementName=PlacementButton}" />
    <!-- 目标上方 -->
    <atom:TourStep Title="Top"
                   Description="On the top of target."
                   Placement="Top"
                   Target="{Binding ElementName=PlacementButton}" />
    <!-- 目标左侧 -->
    <atom:TourStep Title="Left"
                   Description="On the left of target."
                   Placement="Left"
                   Target="{Binding ElementName=PlacementButton}" />
</atom:Tour>
```

---

## 4. 自定义指示器

### 文字指示器

```xml
<atom:Tour IsOpen="{Binding TourOpened, Mode=TwoWay}">
    <atom:Tour.Indicator>
        <atom:TextTourIndicator />
    </atom:Tour.Indicator>
    <atom:TourStep Target="{Binding ElementName=Upload}"
                   Title="Upload File"
                   Description="Put your files here.">
        <atom:TourStep.Cover>
            <Image Source="/Assets/TourShowCase/Cover.png" Width="400" />
        </atom:TourStep.Cover>
    </atom:TourStep>
    <atom:TourStep Target="{Binding ElementName=Save}"
                   Title="Save"
                   Description="Save your changes." />
    <atom:TourStep Target="{Binding ElementName=Ellipsis}"
                   Title="Other Actions"
                   Description="Click to see other actions." />
</atom:Tour>
```

指示器将显示 "1 / 3"、"2 / 3"、"3 / 3" 格式文本，而非默认的圆点。

---

## 5. 自定义遮罩颜色

Tour 全局遮罩颜色和每个步骤的遮罩颜色都可以单独自定义：

```xml
<atom:Tour IsOpen="{Binding CustomMaskTourOpened, Mode=TwoWay}"
           MaskColor="#6650FFFF">
    <!-- 第一步：使用全局遮罩颜色 -->
    <atom:TourStep Target="{Binding ElementName=Upload}"
                   Title="Upload File"
                   Description="Put your files here." />
    <!-- 第二步：覆盖为紫色遮罩 -->
    <atom:TourStep Target="{Binding ElementName=Save}"
                   Title="Save"
                   Description="Save your changes."
                   MaskColor="#662800FF" />
    <!-- 第三步：关闭遮罩 -->
    <atom:TourStep Target="{Binding ElementName=Ellipsis}"
                   Title="Other Actions"
                   Description="Click to see other actions."
                   IsShowMask="False" />
</atom:Tour>
```

---

## 6. 自定义操作按钮

通过 `Tour.CustomActions` 可以添加自定义按钮（如"跳过"），按钮需实现 `ITourAction` 接口：

```xml
<atom:Tour IsOpen="{Binding CustomActionTourOpened, Mode=TwoWay}">
    <atom:Tour.CustomActions>
        <local:SkipTourActionButton>Skip</local:SkipTourActionButton>
    </atom:Tour.CustomActions>
    <atom:TourStep Target="{Binding ElementName=Upload}"
                   Title="Upload File"
                   Description="Put your files here." />
    <atom:TourStep Target="{Binding ElementName=Save}"
                   Title="Save"
                   Description="Save your changes." />
    <atom:TourStep Target="{Binding ElementName=Ellipsis}"
                   Title="Other Actions"
                   Description="Click to see other actions." />
</atom:Tour>
```

`SkipTourActionButton` 的实现参见 [TourStyling.md](TourStyling.md) 中的自定义操作按钮章节。

> 📖 参考：`controlgallery/AtomUIGallery/ShowCases/Views/DataDisplay/TourShowCase.axmal.cs` 中的 `SkipTourActionButton` 实现。

---

## 7. 自定义高亮区域

通过 `GapRadius`、`GapOffsetX`、`GapOffsetY` 控制遮罩层镂空区域的形状和偏移：

```xml
<atom:Tour IsOpen="{Binding CustomGapTourOpened, Mode=TwoWay}"
           Placement="Top"
           GapRadius="{Binding CustomGapRadius}"
           GapOffsetX="{Binding CustomGapOffsetX}"
           GapOffsetY="{Binding CustomGapOffsetY}">
    <atom:TourStep Target="{Binding ElementName=CustomGapControl}"
                   Title="Upload File"
                   Description="Put your files here.">
        <atom:TourStep.Cover>
            <Image Source="/Assets/TourShowCase/Cover.png" Width="400" />
        </atom:TourStep.Cover>
    </atom:TourStep>
</atom:Tour>
```

可以配合 Slider 实时调整参数：

```xml
<DockPanel LastChildFill="True">
    <TextBlock Width="200" DockPanel.Dock="Left">Radius:</TextBlock>
    <atom:Slider Minimum="0" Maximum="100" Value="{Binding CustomGapRadius, Mode=TwoWay}" />
</DockPanel>
<DockPanel LastChildFill="True">
    <TextBlock Width="200" DockPanel.Dock="Left">Horizontal offset:</TextBlock>
    <atom:Slider Minimum="1" Maximum="50" Value="{Binding CustomGapOffsetX, Mode=TwoWay}" />
</DockPanel>
<DockPanel LastChildFill="True">
    <TextBlock Width="200" DockPanel.Dock="Left">Vertical offset:</TextBlock>
    <atom:Slider Minimum="1" Maximum="50" Value="{Binding CustomGapOffsetY, Mode=TwoWay}" />
</DockPanel>
```

---

## 8. 封面图（Cover）

每个步骤都支持附带封面图，通过 `Cover` 属性设置（可以是 `Image` 或任意控件），也可通过 `CoverTemplate` 使用数据模板：

```xml
<atom:TourStep Target="{Binding ElementName=Upload}"
               Title="Upload File"
               Description="Put your files here.">
    <atom:TourStep.Cover>
        <Image Source="/Assets/TourShowCase/Cover.png" Width="400" />
    </atom:TourStep.Cover>
</atom:TourStep>
```

---

## 9. MVVM 绑定模式

Tour 的 `IsOpen` 和 `CurrentIndex` 均支持双向绑定，推荐使用 MVVM 模式管理 Tour 状态：

```csharp
// ViewModel（使用 ReactiveUI）
public class TourViewModel : ReactiveObject
{
    [Reactive] public bool TourOpened { get; set; }
}
```

```xml
<atom:Button ButtonType="Primary"
             Command="{Binding OpenTourCommand}">Begin Tour</atom:Button>

<atom:Tour IsOpen="{Binding TourOpened, Mode=TwoWay}">
    ...
</atom:Tour>
```

当用户完成所有步骤或点击关闭按钮时，Tour 内部会自动将 `IsOpen` 设为 `false`，ViewModel 中的属性也会同步更新。

---

## 常见组合模式

### 新手引导（多步骤 + 遮罩）

```xml
<atom:Tour IsOpen="{Binding ShowOnboarding, Mode=TwoWay}">
    <atom:TourStep Target="{Binding ElementName=SearchBar}"
                   Title="Search" Description="Search for anything here."
                   Placement="Bottom" />
    <atom:TourStep Target="{Binding ElementName=NavMenu}"
                   Title="Navigation" Description="Browse sections."
                   Placement="Right" />
    <atom:TourStep Target="{Binding ElementName=ProfileBtn}"
                   Title="Profile" Description="View and edit your profile."
                   Placement="BottomRight" />
</atom:Tour>
```

### 功能高亮（单步骤 + 无遮罩 + Primary）

```xml
<atom:Tour IsOpen="{Binding HighlightFeature, Mode=TwoWay}"
           IsShowMask="False" StyleType="Primary">
    <atom:TourStep Target="{Binding ElementName=NewFeatureBtn}"
                   Title="New Feature!"
                   Description="Try our brand new export function."
                   Placement="Bottom" />
</atom:Tour>
```
