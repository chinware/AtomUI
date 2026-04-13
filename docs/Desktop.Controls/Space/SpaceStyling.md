# Space 自定义样式指南

Space 和 CompactSpace 的视觉表现主要通过属性控制和 Design Token 系统驱动。以下介绍各种自定义方式。

---

## 1. 使用属性直接控制

### 基本间距

最简单的方式是通过 Space 的公共属性控制布局行为：

```xml
<!-- 水平间距（默认方向 + 默认 Small 尺寸） -->
<atom:Space>
    <atom:Button ButtonType="Primary">Primary</atom:Button>
    <atom:Button>Default</atom:Button>
    <atom:Button ButtonType="Dashed">Dashed</atom:Button>
</atom:Space>

<!-- 垂直间距 -->
<atom:Space Orientation="Vertical" Width="200">
    <atom:Button ButtonType="Primary" HorizontalAlignment="Stretch">Primary</atom:Button>
    <atom:Button HorizontalAlignment="Stretch">Default</atom:Button>
    <atom:Button ButtonType="Dashed" HorizontalAlignment="Stretch">Dashed</atom:Button>
</atom:Space>
```

### 预设尺寸切换

```xml
<!-- 三种预设尺寸 -->
<atom:Space SizeType="Small">...</atom:Space>   <!-- 8px 间距 -->
<atom:Space SizeType="Middle">...</atom:Space>  <!-- 16px 间距 -->
<atom:Space SizeType="Large">...</atom:Space>   <!-- 24px 间距 -->
```

### 自定义间距

当预设尺寸不满足需求时，可以直接设置具体数值：

```xml
<!-- 自定义项目间距 -->
<atom:Space ItemSpacing="24">
    <atom:Button>Button 1</atom:Button>
    <atom:Button>Button 2</atom:Button>
</atom:Space>

<!-- 分别控制项目间距和行间距 -->
<atom:Space ItemSpacing="8" LineSpacing="16">
    <atom:Button>Button 1</atom:Button>
    <atom:Button>Button 2</atom:Button>
    <!-- 更多按钮... -->
</atom:Space>
```

### 对齐方式

```xml
<!-- 不同对齐方式对比 -->
<atom:Space ItemsAlignment="Start">
    <TextBlock>Text</TextBlock>
    <atom:Button SizeType="Large">Large Button</atom:Button>
</atom:Space>

<atom:Space ItemsAlignment="Center">
    <TextBlock>Text</TextBlock>
    <atom:Button SizeType="Large">Large Button</atom:Button>
</atom:Space>

<atom:Space ItemsAlignment="End">
    <TextBlock>Text</TextBlock>
    <atom:Button SizeType="Large">Large Button</atom:Button>
</atom:Space>
```

### 带分割线

```xml
<atom:Space Orientation="Horizontal" ItemsAlignment="Center">
    <atom:Space.SplitTemplate>
        <Template>
            <atom:Separator Orientation="Vertical" />
        </Template>
    </atom:Space.SplitTemplate>
    <atom:HyperLinkTextBlock>Link 1</atom:HyperLinkTextBlock>
    <atom:HyperLinkTextBlock>Link 2</atom:HyperLinkTextBlock>
    <atom:HyperLinkTextBlock>Link 3</atom:HyperLinkTextBlock>
</atom:Space>
```

> 📖 完整示例参考：`controlgallery/AtomUIGallery/ShowCases/Views/Layout/SpaceShowCase.axaml`

---

## 2. CompactSpace 使用方式

### 基本紧凑组合

```xml
<!-- 搜索栏：输入框 + 按钮 -->
<atom:CompactSpace Orientation="Horizontal" HorizontalAlignment="Stretch">
    <atom:LineEdit Text="https://atomui.net" atom:CompactSpace.ItemSize="4*" />
    <atom:Button ButtonType="Primary">Submit</atom:Button>
    <atom:CompactSpaceFiller atom:CompactSpace.ItemSize="5*" />
</atom:CompactSpace>
```

### 按钮紧凑组合

```xml
<!-- 工具栏按钮组 -->
<atom:CompactSpace Orientation="Horizontal">
    <atom:Button Icon="{antdicons:AntDesignIconProvider LikeOutlined}" atom:ToolTip.Tip="Like" />
    <atom:Button Icon="{antdicons:AntDesignIconProvider CommentOutlined}" atom:ToolTip.Tip="Comment" />
    <atom:Button Icon="{antdicons:AntDesignIconProvider StarOutlined}" atom:ToolTip.Tip="Star" />
</atom:CompactSpace>

<!-- Primary 按钮组 -->
<atom:CompactSpace Orientation="Horizontal">
    <atom:Button ButtonType="Primary">Button 1</atom:Button>
    <atom:Button ButtonType="Primary">Button 2</atom:Button>
    <atom:Button ButtonType="Primary">Button 3</atom:Button>
</atom:CompactSpace>
```

### 垂直紧凑模式

```xml
<atom:CompactSpace Orientation="Vertical">
    <atom:Button ButtonType="Default">Button 1</atom:Button>
    <atom:Button ButtonType="Default">Button 2</atom:Button>
    <atom:Button ButtonType="Default">Button 3</atom:Button>
</atom:CompactSpace>
```

### 使用 CompactSpaceAddOn

```xml
<atom:CompactSpace Orientation="Horizontal" HorizontalAlignment="Stretch">
    <atom:LineEdit PlaceholderText="input here" />
    <atom:CompactSpaceAddOn Content="$" />
    <atom:NumericUpDown PlaceholderText="amount" atom:CompactSpace.ItemSize="1*" />
    <atom:CompactSpaceFiller atom:CompactSpace.ItemSize="*" />
</atom:CompactSpace>
```

### 使用 ItemSize 控制宽度分配

```xml
<!-- 区号 + 电话号码组合（2:3 比例） -->
<atom:CompactSpace Orientation="Horizontal" HorizontalAlignment="Stretch">
    <atom:LineEdit Text="0571" atom:CompactSpace.ItemSize="2*" />
    <atom:LineEdit Text="26888888" atom:CompactSpace.ItemSize="3*" />
    <atom:CompactSpaceFiller atom:CompactSpace.ItemSize="5*" />
</atom:CompactSpace>

<!-- 固定像素宽度 -->
<atom:CompactSpace Orientation="Horizontal" HorizontalAlignment="Stretch">
    <atom:LineEdit atom:CompactSpace.ItemSize="100" PlaceholderText="Minimum" />
    <atom:LineEdit PlaceholderText="~" IsEnabled="False" />
    <atom:LineEdit atom:CompactSpace.ItemSize="100" PlaceholderText="Maximum" />
    <atom:CompactSpaceFiller atom:CompactSpace.ItemSize="5*" />
</atom:CompactSpace>
```

---

## 3. 通过 Style 覆盖样式

### 全局统一间距

```xml
<Window.Styles>
    <!-- 统一所有 Space 的默认间距 -->
    <Style Selector="atom|Space">
        <Setter Property="ItemSpacing" Value="12" />
    </Style>
</Window.Styles>
```

### 按方向定制

```xml
<Style Selector="atom|Space[Orientation=Vertical]">
    <Setter Property="ItemSpacing" Value="16" />
    <Setter Property="LineSpacing" Value="16" />
</Style>
```

### CompactSpace 样式覆盖

```xml
<!-- CompactSpace 内子项的统一尺寸 -->
<Style Selector="atom|CompactSpace[SizeType=Small]">
    <Setter Property="MinHeight" Value="24" />
</Style>
```

### CompactSpaceAddOn 样式覆盖

```xml
<!-- 所有 AddOn 使用自定义背景色 -->
<Style Selector="atom|CompactSpaceAddOn[StyleVariant=Outline]">
    <Setter Property="Background" Value="#f0f0f0" />
</Style>
```

---

## 4. 通过 ControlTheme 替换主题

CompactSpace 和 CompactSpaceAddOn 支持完全替换 ControlTheme：

```xml
<ControlTheme x:Key="MyCompactSpaceAddOn" TargetType="atom:CompactSpaceAddOn">
    <Setter Property="Template">
        <ControlTemplate>
            <ContentPresenter Content="{TemplateBinding Content}"
                              Background="{TemplateBinding Background}"
                              Padding="{TemplateBinding Padding}" />
        </ControlTemplate>
    </Setter>
    <!-- 自定义样式... -->
</ControlTheme>
```

> ⚠️ 注意：Space 本身不使用 ControlTemplate，因此不支持替换 ControlTheme。Space 的视觉表现完全由子控件和间距属性决定。

---

## 5. 动态切换尺寸

通过数据绑定实现运行时动态切换间距尺寸（参考 Gallery 中的 "Space Size" 示例）：

```xml
<StackPanel Orientation="Vertical" Spacing="15">
    <!-- 尺寸切换 RadioButton 组 -->
    <StackPanel Orientation="Horizontal" Spacing="10">
        <atom:RadioButton Tag="{x:Static atom:CustomizableSizeType.Small}"
                          IsCheckedChanged="HandleSizeTypeChanged"
                          IsChecked="True">Small</atom:RadioButton>
        <atom:RadioButton Tag="{x:Static atom:CustomizableSizeType.Middle}"
                          IsCheckedChanged="HandleSizeTypeChanged">Middle</atom:RadioButton>
        <atom:RadioButton Tag="{x:Static atom:CustomizableSizeType.Large}"
                          IsCheckedChanged="HandleSizeTypeChanged">Large</atom:RadioButton>
        <atom:RadioButton Tag="{x:Static atom:CustomizableSizeType.Custom}"
                          IsCheckedChanged="HandleSizeTypeChanged">Custom</atom:RadioButton>
    </StackPanel>

    <!-- 自定义间距滑块（仅 Custom 模式下可见） -->
    <atom:Slider Name="CustomSizeSlider"
                 Minimum="0" Maximum="100"
                 Value="{Binding CustomSpacingValue, Mode=TwoWay}"
                 IsVisible="False" />

    <!-- 间距效果预览 -->
    <atom:Space Orientation="Horizontal"
                SizeType="{Binding SizeType}"
                LineSpacing="{Binding #CustomSizeSlider.Value, Priority=Template}"
                ItemSpacing="{Binding #CustomSizeSlider.Value, Priority=Template}">
        <atom:Button ButtonType="Primary">Primary</atom:Button>
        <atom:Button>Default</atom:Button>
        <atom:Button ButtonType="Dashed">Dashed</atom:Button>
        <atom:Button ButtonType="Link">Link</atom:Button>
    </atom:Space>
</StackPanel>
```

```csharp
private void HandleSizeTypeChanged(object sender, RoutedEventArgs e)
{
    if (sender is RadioButton radioButton && radioButton.IsChecked == true)
    {
        var sizeType = (CustomizableSizeType)radioButton.Tag!;
        if (DataContext is SpaceViewModel vm)
        {
            vm.SizeType = sizeType;
        }
        if (CustomSizeSlider != null)
        {
            CustomSizeSlider.IsVisible = sizeType == CustomizableSizeType.Custom;
        }
    }
}
```

> 📖 参考：`controlgallery/AtomUIGallery/ShowCases/Views/Layout/SpaceShowCase.axaml` 中 "Space Size" 示例。

---

## 样式选择器速查

### Space 选择器

| 选择器 | 说明 |
|---|---|
| `atom\|Space` | 匹配所有 Space 实例 |
| `atom\|Space[Orientation=Horizontal]` | 匹配水平方向的 Space |
| `atom\|Space[Orientation=Vertical]` | 匹配垂直方向的 Space |
| `atom\|Space[SizeType=Small]` | 匹配小尺寸 Space |
| `atom\|Space[SizeType=Middle]` | 匹配中尺寸 Space |
| `atom\|Space[SizeType=Large]` | 匹配大尺寸 Space |
| `atom\|Space:disabled` | 匹配禁用状态的 Space |

### CompactSpace 选择器

| 选择器 | 说明 |
|---|---|
| `atom\|CompactSpace` | 匹配所有 CompactSpace 实例 |
| `atom\|CompactSpace[Orientation=Horizontal]` | 匹配水平方向的 CompactSpace |
| `atom\|CompactSpace[Orientation=Vertical]` | 匹配垂直方向的 CompactSpace |
| `atom\|CompactSpace[SizeType=Small]` | 匹配小尺寸 CompactSpace |
| `atom\|CompactSpace[SizeType=Large]` | 匹配大尺寸 CompactSpace |

### CompactSpaceAddOn 选择器

| 选择器 | 说明 |
|---|---|
| `atom\|CompactSpaceAddOn` | 匹配所有 CompactSpaceAddOn 实例 |
| `atom\|CompactSpaceAddOn[StyleVariant=Outline]` | 匹配 Outline 样式变体（默认带边框 + 背景色） |
| `atom\|CompactSpaceAddOn[StyleVariant=Filled]` | 匹配 Filled 样式变体（有背景无边框） |
| `atom\|CompactSpaceAddOn[Status=Error]` | 匹配错误状态 |
| `atom\|CompactSpaceAddOn[Status=Warning]` | 匹配警告状态 |
| `atom\|CompactSpaceAddOn[SizeType=Small]` | 匹配小尺寸 |
| `atom\|CompactSpaceAddOn[SizeType=Large]` | 匹配大尺寸 |
| `atom\|CompactSpaceAddOn:disabled` | 匹配禁用状态 |

### 组合选择器示例

| 选择器 | 说明 |
|---|---|
| `atom\|CompactSpaceAddOn[StyleVariant=Outline][Status=Error]` | Outline 样式 + 错误状态 |
| `atom\|CompactSpaceAddOn[StyleVariant=Filled][Status=Warning]` | Filled 样式 + 警告状态 |
| `atom\|CompactSpace /template/ Grid#PART_ContentLayout` | 访问 CompactSpace 模板内的 Grid 部件 |
