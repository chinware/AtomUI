# Tooltip 自定义样式指南

ToolTip 的视觉表现通过 `ControlTheme` + Design Token 系统控制。以下介绍几种常见的自定义方式。

---

## 1. 通过附加属性直接控制

最简单的方式是通过 ToolTip 的附加属性来控制外观：

### 基本用法

```xml
<!-- 默认样式（深色背景、白色文字、带箭头） -->
<atom:Button atom:ToolTip.Tip="This is a tooltip">
    Hover me
</atom:Button>
```

### 设置弹出方向

```xml
<!-- 12 种方向 -->
<atom:Button atom:ToolTip.Tip="Top tooltip"
             atom:ToolTip.Placement="Top">
    Top
</atom:Button>

<atom:Button atom:ToolTip.Tip="Bottom tooltip"
             atom:ToolTip.Placement="Bottom">
    Bottom
</atom:Button>

<atom:Button atom:ToolTip.Tip="Left tooltip"
             atom:ToolTip.Placement="LeftEdgeAlignedTop">
    LT
</atom:Button>
```

### 箭头控制

```xml
<!-- 显示箭头（默认） -->
<atom:Button atom:ToolTip.Tip="With arrow"
             atom:ToolTip.IsShowArrow="True">
    Arrow
</atom:Button>

<!-- 隐藏箭头 -->
<atom:Button atom:ToolTip.Tip="No arrow"
             atom:ToolTip.IsShowArrow="False">
    No Arrow
</atom:Button>

<!-- 箭头指向目标中心 -->
<atom:Button atom:ToolTip.Tip="Point at center"
             atom:ToolTip.IsShowArrow="True"
             atom:ToolTip.IsPointAtCenter="True"
             atom:ToolTip.Placement="TopEdgeAlignedLeft">
    Point at Center
</atom:Button>
```

### 预设颜色

```xml
<!-- 使用 Ant Design 调色板颜色 -->
<atom:Button atom:ToolTip.Tip="Blue tooltip"
             atom:ToolTip.PresetColor="Blue">
    Blue
</atom:Button>

<atom:Button atom:ToolTip.Tip="Red tooltip"
             atom:ToolTip.PresetColor="Red">
    Red
</atom:Button>

<atom:Button atom:ToolTip.Tip="Green tooltip"
             atom:ToolTip.PresetColor="Green">
    Green
</atom:Button>
```

### 自定义颜色

```xml
<!-- 使用任意颜色值 -->
<atom:Button atom:ToolTip.Tip="Custom color"
             atom:ToolTip.Color="#f50">
    Custom
</atom:Button>

<atom:Button atom:ToolTip.Tip="Custom blue"
             atom:ToolTip.Color="#2db7f5">
    Blue
</atom:Button>
```

### 延迟设置

```xml
<!-- 立即显示 -->
<atom:Button atom:ToolTip.Tip="Instant tooltip"
             atom:ToolTip.ShowDelay="0">
    Instant
</atom:Button>

<!-- 延迟 1 秒显示 -->
<atom:Button atom:ToolTip.Tip="Delayed tooltip"
             atom:ToolTip.ShowDelay="1000">
    Delayed
</atom:Button>
```

> 📖 完整示例参考：`controlgallery/AtomUIGallery/ShowCases/Views/DataDisplay/TooltipShowCase.axaml`

---

## 2. 通过 Style 覆盖样式

可以在 AXAML 中使用 `Style` 对 ToolTip 进行全局或局部样式覆盖：

### 全局字体大小

```xml
<Window.Styles>
    <Style Selector="atom|ToolTip">
        <Setter Property="FontSize" Value="12" />
    </Style>
</Window.Styles>
```

### 覆盖内部 ArrowDecoratedBox 样式

```xml
<Window.Styles>
    <!-- 修改所有 ToolTip 的最大宽度 -->
    <Style Selector="atom|ToolTip /template/ atom|ArrowDecoratedBox#PART_ToolTipContainer">
        <Setter Property="MaxWidth" Value="400" />
    </Style>

    <!-- 修改所有 ToolTip 的内间距 -->
    <Style Selector="atom|ToolTip /template/ atom|ArrowDecoratedBox#PART_ToolTipContainer">
        <Setter Property="Padding" Value="12 8" />
    </Style>
</Window.Styles>
```

### ToolTip 打开时的样式

```xml
<Style Selector="atom|ToolTip:open">
    <!-- ToolTip 打开时的额外样式 -->
</Style>
```

---

## 3. 通过 ControlTheme 完全替换主题

如果需要彻底替换 ToolTip 的模板和样式，可以定义自定义 `ControlTheme`：

```xml
<ControlTheme x:Key="MyCustomToolTip" TargetType="atom:ToolTip">
    <Setter Property="Template">
        <ControlTemplate>
            <Border Background="{TemplateBinding Background}"
                    CornerRadius="8"
                    Padding="12 8">
                <ContentPresenter Content="{TemplateBinding Content}"
                                  ContentTemplate="{TemplateBinding ContentTemplate}" />
            </Border>
        </ControlTemplate>
    </Setter>
    <Setter Property="Background" Value="#333" />
    <Setter Property="Foreground" Value="White" />
</ControlTheme>
```

> ⚠️ 注意：完全替换 ControlTheme 将丢失内置的箭头指示器、阴影效果和动画功能。建议优先使用 Style 覆盖。

---

## 4. 数据绑定箭头控制

ToolTip 的箭头模式可以通过数据绑定动态切换，Gallery 中的 Arrow 示例展示了这种用法：

```xml
<!-- 通过 Segmented 切换箭头模式 -->
<atom:Segmented x:Name="ArrowSegmented">
    <atom:SegmentedItem>Show</atom:SegmentedItem>
    <atom:SegmentedItem>Hide</atom:SegmentedItem>
    <atom:SegmentedItem>Center</atom:SegmentedItem>
</atom:Segmented>

<!-- 绑定 ViewModel 控制箭头 -->
<atom:Button atom:ToolTip.Tip="prompt text"
             atom:ToolTip.Placement="Top"
             atom:ToolTip.IsShowArrow="{Binding ShowArrow}"
             atom:ToolTip.IsPointAtCenter="{Binding IsPointAtCenter}">
    Top
</atom:Button>
```

```csharp
// ViewModel
public class TooltipViewModel : ReactiveObject
{
    private bool _showArrow = true;
    public bool ShowArrow
    {
        get => _showArrow;
        set => this.RaiseAndSetIfChanged(ref _showArrow, value);
    }

    private bool _isPointAtCenter;
    public bool IsPointAtCenter
    {
        get => _isPointAtCenter;
        set => this.RaiseAndSetIfChanged(ref _isPointAtCenter, value);
    }
}
```

> 📖 参考：`controlgallery/AtomUIGallery/ShowCases/Views/DataDisplay/TooltipShowCase.axaml` 中 "Arrow" 示例。

---

## 5. 控制动画行为

```xml
<!-- 禁用弹出/关闭动画 -->
<!-- 注意：IsMotionEnabled 是 ToolTip 实例属性，非附加属性 -->
<!-- 通过 Style 覆盖全局设置 -->
<Window.Styles>
    <Style Selector="atom|ToolTip">
        <Setter Property="IsMotionEnabled" Value="False" />
    </Style>
</Window.Styles>
```

---

## 6. 程序化控制显示/隐藏

通过 `IsOpen` + `IsCustomShowAndHide` 组合实现程序化控制：

```xml
<atom:Button atom:ToolTip.Tip="Controlled tooltip"
             atom:ToolTip.IsOpen="{Binding IsTooltipOpen}"
             atom:ToolTip.IsCustomShowAndHide="True">
    Controlled
</atom:Button>
```

> ⚠️ 使用程序化控制时**必须**设置 `IsCustomShowAndHide="True"`，否则 `ToolTipService` 会自动管理显示/隐藏，导致 `IsOpen` 绑定被覆盖。

---

## 样式选择器速查

> 说明：Avalonia 样式选择器中使用 `atom|ToolTip` 语法引用 `atom` XML 命名空间下的 `ToolTip` 类型，其中 `|` 是命名空间分隔符。

### 基本选择器

| 选择器 | 说明 |
|---|---|
| `atom\|ToolTip` | 匹配所有 AtomUI ToolTip 实例，用于设置全局通用样式 |
| `atom\|ToolTip:open` | 匹配已打开的 ToolTip |

### 模板部件选择器

| 选择器 | 说明 |
|---|---|
| `atom\|ToolTip /template/ atom\|ArrowDecoratedBox#PART_ToolTipContainer` | 访问模板内的箭头装饰盒子，可修改最大宽度、内间距、圆角等 |
| `atom\|ToolTip /template/ ContentPresenter#PART_ToolTipContentPresenter` | 访问模板内的内容展示器 |

### 附加属性选择器

由于 ToolTip 通过附加属性使用，要对特定条件下的 ToolTip 进行样式定制，通常需要在目标控件上设置样式：

```xml
<!-- 为特定控件的 ToolTip 设置弹出方向 -->
<Style Selector="atom|Button.my-class">
    <Setter Property="atom:ToolTip.Placement" Value="Bottom" />
    <Setter Property="atom:ToolTip.ShowDelay" Value="200" />
</Style>
```
