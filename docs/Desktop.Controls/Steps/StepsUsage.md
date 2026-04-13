# Steps 使用文档

本文档介绍 AtomUI Steps 控件的各种使用方式，示例代码摘自 Gallery 演示程序。

> 📖 Gallery 源码位置：`controlgallery/AtomUIGallery/ShowCases/Views/Navigation/StepsShowCase.axaml`

---

## 前置准备

在 AXAML 中使用 Steps，需要引入以下命名空间：

```xml
xmlns:atom="https://atomui.net"
xmlns:antdicons="https://atomui.net/icons/antdesign"
```

在 C# 代码中引入：

```csharp
using AtomUI.Desktop.Controls;   // Steps, StepsItem, 枚举类型
```

---

## 1. 基础用法

最简单的步骤条——展示三个步骤的流程：

```xml
<atom:Steps CurrentStep="0">
    <atom:StepsItem Header="Finished" Description="This is a description." />
    <atom:StepsItem Header="In Progress" Description="This is a description."
                    SubHeader="Left 00:00:08" />
    <atom:StepsItem Header="Waiting" Description="This is a description." />
</atom:Steps>
```

`CurrentStep="0"` 表示第一个步骤为当前步骤。Steps 会自动为索引 0 之前的步骤设为 `Finish`，索引 0 设为 `Process`，之后设为 `Wait`。

> 📖 参考：Gallery ShowCase 中 "Basic" 示例。

---

## 2. 小尺寸版本

通过 `SizeType="Small"` 获得紧凑版本的步骤条：

```xml
<atom:Steps CurrentStep="0" SizeType="Small">
    <atom:StepsItem Header="Finished" Description="This is a description." />
    <atom:StepsItem Header="In Progress" Description="This is a description."
                    SubHeader="Left 00:00:08" />
    <atom:StepsItem Header="Waiting" Description="This is a description." />
</atom:Steps>
```

> 📖 参考：Gallery ShowCase 中 "Mini version" 示例。

---

## 3. 自定义图标

通过 `Icon` 属性为每个步骤设置自定义图标，替换默认的数字指示器：

```xml
<atom:Steps CurrentStep="1">
    <atom:StepsItem Header="Login" Status="Finish"
                    Icon="{antdicons:AntDesignIconProvider Kind=UserOutlined}" />
    <atom:StepsItem Header="Verification" Status="Finish"
                    Icon="{antdicons:AntDesignIconProvider Kind=SolutionOutlined}" />
    <atom:StepsItem Header="Pay" Status="Process"
                    Icon="{antdicons:AntDesignIconProvider Kind=LoadingOutlined, Animation=Spin}" />
    <atom:StepsItem Header="Done" Status="Wait"
                    Icon="{antdicons:AntDesignIconProvider Kind=SmileOutlined}" />
</atom:Steps>
```

> 📖 参考：Gallery ShowCase 中 "With icon" 示例。

---

## 4. 步骤内容切换

为每个步骤关联内容，配合 `CurrentContent` 实现内容区域的动态切换：

```xml
<StackPanel Orientation="Vertical" Spacing="20">
    <atom:Steps Name="ContentSteps" CurrentStep="{Binding CurrentStep}">
        <atom:StepsItem Header="First" Content="First-content" />
        <atom:StepsItem Header="Second" Content="Second-content" />
        <atom:StepsItem Header="Third" Content="Last-content" />
    </atom:Steps>

    <atom:DashedBorder BorderThickness="1"
                       BorderBrush="{atom:SharedTokenResource ColorBorder}"
                       CornerRadius="{atom:SharedTokenResource BorderRadiusLG}"
                       Background="{atom:SharedTokenResource ColorFillAlter}"
                       Height="260">
        <ContentPresenter Content="{Binding #ContentSteps.CurrentContent}"
                          ContentTemplate="{Binding #ContentSteps.CurrentContentTemplate}"
                          HorizontalAlignment="Center"
                          VerticalAlignment="Center" />
    </atom:DashedBorder>

    <StackPanel Orientation="Horizontal" Spacing="10">
        <atom:Button ButtonType="Primary" Name="NextBtn">Next</atom:Button>
        <atom:Button Name="PrevBtn">Previous</atom:Button>
    </StackPanel>
</StackPanel>
```

```csharp
private void HandleNextClick(object? sender, RoutedEventArgs e)
{
    if (viewModel.CurrentStep < ContentSteps.ItemCount - 1)
        viewModel.CurrentStep++;
}

private void HandlePreviousClick(object? sender, RoutedEventArgs e)
{
    if (viewModel.CurrentStep > 0)
        viewModel.CurrentStep--;
}
```

> 📖 参考：Gallery ShowCase 中 "Switch Step" 示例。

---

## 5. 垂直方向

```xml
<atom:Steps CurrentStep="1" Orientation="Vertical">
    <atom:StepsItem Header="Finished" Description="This is a description." />
    <atom:StepsItem Header="In Progress" Description="This is a description."
                    SubHeader="Left 00:00:08" />
    <atom:StepsItem Header="Waiting" Description="This is a description." />
</atom:Steps>
```

垂直 + 小尺寸：

```xml
<atom:Steps CurrentStep="1" Orientation="Vertical" SizeType="Small">
    <atom:StepsItem Header="Finished" Description="This is a description." />
    <atom:StepsItem Header="In Progress" Description="This is a description." />
    <atom:StepsItem Header="Waiting" Description="This is a description." />
</atom:Steps>
```

> 📖 参考：Gallery ShowCase 中 "Vertical" 和 "Vertical mini version" 示例。

---

## 6. 错误状态

通过 `CurrentStepStatus="Error"` 将当前步骤标记为错误状态：

```xml
<atom:Steps CurrentStep="1" CurrentStepStatus="Error">
    <atom:StepsItem Header="Finished" Description="This is a description." />
    <atom:StepsItem Header="In Progress" Description="This is a description."
                    SubHeader="Left 00:00:08" />
    <atom:StepsItem Header="Waiting" Description="This is a description." />
</atom:Steps>
```

> 📖 参考：Gallery ShowCase 中 "Error status" 示例。

---

## 7. 点状样式

通过 `ItemIndicatorType="Dot"` 使用点状指示器代替数字圆圈：

### 水平点状

```xml
<atom:Steps CurrentStep="1" ItemIndicatorType="Dot">
    <atom:StepsItem Header="Finished" Description="This is a description." />
    <atom:StepsItem Header="In Progress" Description="This is a description." />
    <atom:StepsItem Header="Waiting" Description="This is a description." />
</atom:Steps>
```

### 垂直点状

```xml
<atom:Steps CurrentStep="1" ItemIndicatorType="Dot" Orientation="Vertical">
    <atom:StepsItem Header="Finished" Description="This is a description." />
    <atom:StepsItem Header="In Progress" Description="This is a description." />
    <atom:StepsItem Header="Waiting" Description="This is a description." />
</atom:Steps>
```

> 📖 参考：Gallery ShowCase 中 "Dot Style" 和 "Dot Style Vertical" 示例。

---

## 8. 可点击步骤

通过 `IsItemClickable="True"` 允许用户点击任意步骤进行跳转：

```xml
<atom:Steps CurrentStep="0" IsItemClickable="True">
    <atom:StepsItem Header="Step 1" Description="This is a description." />
    <atom:StepsItem Header="Step 2" Description="This is a description." />
    <atom:StepsItem Header="Step 3" Description="This is a description." />
</atom:Steps>
```

可点击模式下支持所有风格和方向的组合：

```xml
<!-- 可点击 + 垂直 -->
<atom:Steps CurrentStep="0" IsItemClickable="True" Orientation="Vertical">
    <!-- ... -->
</atom:Steps>

<!-- 可点击 + 点状 -->
<atom:Steps CurrentStep="0" IsItemClickable="True" ItemIndicatorType="Dot">
    <!-- ... -->
</atom:Steps>
```

> 📖 参考：Gallery ShowCase 中 "Clickable" 示例。

---

## 9. 导航风格

导航式步骤条适用于顶部导航场景：

### 小尺寸导航 + 描述

```xml
<atom:Steps CurrentStep="0" Style="Navigation" IsItemClickable="True" SizeType="Small">
    <atom:StepsItem Header="Step 1" Description="This is a description."
                    SubHeader="00:00:05" Status="Finish" />
    <atom:StepsItem Header="Step 2" Description="This is a description."
                    SubHeader="00:01:02" Status="Process" />
    <atom:StepsItem Header="Step 3" Description="This is a description."
                    SubHeader="waiting for longlong time" Status="Wait" />
</atom:Steps>
```

### 标准导航

```xml
<atom:Steps CurrentStep="0" Style="Navigation" IsItemClickable="True">
    <atom:StepsItem Header="Step 1" Status="Finish" />
    <atom:StepsItem Header="Step 2" Status="Process" />
    <atom:StepsItem Header="Step 3" Status="Wait" />
    <atom:StepsItem Header="Step 4" Status="Wait" />
</atom:Steps>
```

### 垂直导航

```xml
<atom:Steps CurrentStep="0" Style="Navigation" IsItemClickable="True" Orientation="Vertical">
    <atom:StepsItem Header="Step 1" Description="This is a description."
                    SubHeader="00:00:05" Status="Finish" />
    <atom:StepsItem Header="Step 2" Description="This is a description."
                    SubHeader="10:00:05" Status="Process" />
    <atom:StepsItem Header="Step 3" Description="This is a description."
                    SubHeader="00:30:05" Status="Wait" />
</atom:Steps>
```

### 导航 + 点状

```xml
<atom:Steps CurrentStep="0" Style="Navigation" IsItemClickable="True" ItemIndicatorType="Dot">
    <atom:StepsItem Header="Step 1" Description="This is a description." SubHeader="00:00:05" />
    <atom:StepsItem Header="Step 2" Description="This is a description." SubHeader="00:01:02" />
    <atom:StepsItem Header="Step 3" Description="This is a description." SubHeader="03:01:02" />
</atom:Steps>
```

> 📖 参考：Gallery ShowCase 中 "Navigation Steps" 示例（包含 10+ 种组合变体）。

---

## 10. 带进度环

在当前步骤显示进度环（仅在 Default 指示器类型和非 Inline 风格下生效）：

```xml
<atom:Steps CurrentStep="1" ProgressValue="60" IsShowItemProgress="True">
    <atom:StepsItem Header="Finished" Description="This is a description." />
    <atom:StepsItem Header="In Progress" Description="This is a description." />
    <atom:StepsItem Header="Waiting" Description="This is a description." />
</atom:Steps>
```

小尺寸进度环：

```xml
<atom:Steps CurrentStep="1" ProgressValue="30" IsShowItemProgress="True" SizeType="Small">
    <atom:StepsItem Header="Finished" Description="This is a description." />
    <atom:StepsItem Header="In Progress" Description="This is a description." />
    <atom:StepsItem Header="Waiting" Description="This is a description." />
</atom:Steps>
```

> 📖 参考：Gallery ShowCase 中 "Steps with progress" 示例。

---

## 11. 标签垂直排列

通过 `LabelPlacement="Vertical"` 将标题和描述放在指示器下方：

```xml
<atom:Steps CurrentStep="1" LabelPlacement="Vertical">
    <atom:StepsItem Header="Finished" Description="This is a description." />
    <atom:StepsItem Header="In Progress" Description="This is a description." />
    <atom:StepsItem Header="Waiting" Description="This is a description." />
</atom:Steps>
```

标签垂直 + 进度环：

```xml
<atom:Steps CurrentStep="1" LabelPlacement="Vertical" ProgressValue="45" IsShowItemProgress="True">
    <atom:StepsItem Header="Finished" Description="This is a description." />
    <atom:StepsItem Header="In Progress" Description="This is a description." />
    <atom:StepsItem Header="Waiting" Description="This is a description." />
</atom:Steps>
```

> 📖 参考：Gallery ShowCase 中 "Label Placement" 示例。

---

## 12. 内联步骤条

内联风格适用于列表项中嵌入步骤状态：

```xml
<DockPanel LastChildFill="True">
    <atom:Steps CurrentStep="1" DockPanel.Dock="Right" Style="Inline">
        <atom:StepsItem Header="Step 1" Description="This is a description." />
        <atom:StepsItem Header="Step 2" Description="This is a description." />
        <atom:StepsItem Header="Step 3" Description="This is a description." />
    </atom:Steps>
    <StackPanel Orientation="Vertical" Spacing="10">
        <TextBlock FontWeight="Bold">Ant Design Title</TextBlock>
        <TextBlock Foreground="{atom:SharedTokenResource ColorTextTertiary}">
            Ant Design, a design language for background applications
        </TextBlock>
    </StackPanel>
</DockPanel>
```

内联 + 错误状态：

```xml
<atom:Steps CurrentStep="1" Style="Inline" CurrentStepStatus="Error">
    <atom:StepsItem Header="Step 1" Description="This is a description." />
    <atom:StepsItem Header="Step 2" Description="This is a description." />
    <atom:StepsItem Header="Step 3" Description="This is a description." />
</atom:Steps>
```

> 📖 参考：Gallery ShowCase 中 "Inline Steps" 示例。

---

## 常见组合模式

### 表单向导

```xml
<StackPanel Spacing="20">
    <atom:Steps CurrentStep="{Binding WizardStep}">
        <atom:StepsItem Header="Basic Info" />
        <atom:StepsItem Header="Address" />
        <atom:StepsItem Header="Payment" />
        <atom:StepsItem Header="Confirm" />
    </atom:Steps>

    <!-- 表单内容区域 -->
    <ContentControl Content="{Binding CurrentWizardPage}" />

    <!-- 导航按钮 -->
    <StackPanel Orientation="Horizontal" Spacing="10" HorizontalAlignment="Right">
        <atom:Button Click="HandleBack" IsVisible="{Binding CanGoBack}">Back</atom:Button>
        <atom:Button ButtonType="Primary" Click="HandleNext">
            Next
        </atom:Button>
    </StackPanel>
</StackPanel>
```

### 订单进度追踪

```xml
<atom:Steps CurrentStep="2" Orientation="Vertical" SizeType="Small">
    <atom:StepsItem Header="Order Placed" Description="2024-01-15 10:30"
                    Icon="{antdicons:AntDesignIconProvider Kind=ShoppingCartOutlined}" />
    <atom:StepsItem Header="Payment Confirmed" Description="2024-01-15 10:35"
                    Icon="{antdicons:AntDesignIconProvider Kind=CreditCardOutlined}" />
    <atom:StepsItem Header="Shipping" Description="Estimated: 2024-01-18"
                    Icon="{antdicons:AntDesignIconProvider Kind=CarOutlined}" />
    <atom:StepsItem Header="Delivered" Description="Pending"
                    Icon="{antdicons:AntDesignIconProvider Kind=CheckCircleOutlined}" />
</atom:Steps>
```

### 列表中嵌入进度

```xml
<ItemsControl ItemsSource="{Binding Tasks}">
    <ItemsControl.ItemTemplate>
        <DataTemplate>
            <DockPanel LastChildFill="True" Margin="0,4">
                <atom:Steps CurrentStep="{Binding ProgressStep}"
                            DockPanel.Dock="Right" Style="Inline">
                    <atom:StepsItem Header="Draft" />
                    <atom:StepsItem Header="Review" />
                    <atom:StepsItem Header="Published" />
                </atom:Steps>
                <TextBlock Text="{Binding Title}" FontWeight="Bold" />
            </DockPanel>
        </DataTemplate>
    </ItemsControl.ItemTemplate>
</ItemsControl>
```
