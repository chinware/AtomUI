# ComboBox 使用文档

本文档介绍 AtomUI ComboBox 控件的各种使用方式，示例代码摘自 Gallery 演示程序。

> 📖 Gallery 源码位置：`controlgallery/AtomUIGallery/ShowCases/Views/Navigation/ComboBoxShowCase.axaml`

---

## 前置准备

在 AXAML 中使用 ComboBox，需要引入以下命名空间：

```xml
xmlns:atom="https://atomui.net"
xmlns:antdicons="https://atomui.net/icons/antdesign"
```

在 C# 代码中引入：

```csharp
using AtomUI.Desktop.Controls;   // ComboBox, ComboBoxItem 控件
using AtomUI.Controls;            // SizeType, InputControlStyleVariant 等共享类型
```

---

## 1. 基本用法

最简单的 ComboBox 使用方式——直接在 AXAML 中声明 `ComboBoxItem` 子项：

```xml
<atom:ComboBox PlaceholderText="Please select" Width="300">
    <atom:ComboBoxItem>床前明月光</atom:ComboBoxItem>
    <atom:ComboBoxItem>疑是地上霜</atom:ComboBoxItem>
    <atom:ComboBoxItem>举头望明月</atom:ComboBoxItem>
    <atom:ComboBoxItem>低头思故乡</atom:ComboBoxItem>
</atom:ComboBox>
```

---

## 2. 通过 ItemsSource + ItemTemplate 绑定数据

当选项来自 ViewModel 数据源时，使用 `ItemsSource` 绑定集合，`ItemTemplate` 定义选项模板：

```xml
<atom:ComboBox Name="TplComboBox"
               PlaceholderText="Please select" Width="300"
               ItemsSource="{Binding ComboBoxItems}">
    <atom:ComboBox.ItemTemplate>
        <DataTemplate>
            <atom:TextBlock Text="{Binding Text}" VerticalAlignment="Center"/>
        </DataTemplate>
    </atom:ComboBox.ItemTemplate>
</atom:ComboBox>
```

```csharp
// ViewModel
public class ComboBoxViewModel : ReactiveObject
{
    private List<ComboBoxItemData>? _comboBoxItems = [];

    public List<ComboBoxItemData>? ComboBoxItems
    {
        get => _comboBoxItems;
        set => this.RaiseAndSetIfChanged(ref _comboBoxItems, value);
    }
}

public class ComboBoxItemData
{
    public string Text { get; set; } = string.Empty;
}
```

```csharp
// Code-behind：初始化数据
private void InitComboBoxItems(ComboBoxViewModel viewModel)
{
    var items = new List<ComboBoxItemData>
    {
        new() { Text = "床前明月光" },
        new() { Text = "疑是地上霜" },
        new() { Text = "举头望明月" },
        new() { Text = "低头思故乡" },
    };
    viewModel.ComboBoxItems = items;
}
```

> 📖 参考：`controlgallery/AtomUIGallery/ShowCases/Views/Navigation/ComboBoxShowCase.axaml` 中 "Generate ComboBoxItem by ItemsSource" 示例。

---

## 3. 禁用状态

通过 `IsEnabled="False"` 禁用 ComboBox：

```xml
<atom:ComboBox PlaceholderText="Please select" Width="300" IsEnabled="False">
    <atom:ComboBoxItem>床前明月光</atom:ComboBoxItem>
    <atom:ComboBoxItem>疑是地上霜</atom:ComboBoxItem>
    <atom:ComboBoxItem>举头望明月</atom:ComboBoxItem>
    <atom:ComboBoxItem>低头思故乡</atom:ComboBoxItem>
</atom:ComboBox>
```

---

## 4. 三种尺寸

通过 `SizeType` 属性设置尺寸，支持 `Large`（40px）、`Middle`（32px，默认）、`Small`（24px）三种。

```xml
<StackPanel Orientation="Vertical" Spacing="10">
    <atom:ComboBox SizeType="Large" PlaceholderText="Please select">
        <atom:ComboBoxItem>床前明月光</atom:ComboBoxItem>
        <atom:ComboBoxItem>疑是地上霜</atom:ComboBoxItem>
        <atom:ComboBoxItem>举头望明月</atom:ComboBoxItem>
        <atom:ComboBoxItem>低头思故乡</atom:ComboBoxItem>
    </atom:ComboBox>
    <atom:ComboBox SizeType="Middle" PlaceholderText="Please select">
        <atom:ComboBoxItem>床前明月光</atom:ComboBoxItem>
        <atom:ComboBoxItem>疑是地上霜</atom:ComboBoxItem>
        <atom:ComboBoxItem>举头望明月</atom:ComboBoxItem>
        <atom:ComboBoxItem>低头思故乡</atom:ComboBoxItem>
    </atom:ComboBox>
    <atom:ComboBox SizeType="Small" PlaceholderText="Please select">
        <atom:ComboBoxItem>床前明月光</atom:ComboBoxItem>
        <atom:ComboBoxItem>疑是地上霜</atom:ComboBoxItem>
        <atom:ComboBoxItem>举头望明月</atom:ComboBoxItem>
        <atom:ComboBoxItem>低头思故乡</atom:ComboBoxItem>
    </atom:ComboBox>
</StackPanel>
```

> 📖 参考：`controlgallery/AtomUIGallery/ShowCases/Views/Navigation/ComboBoxShowCase.axaml` 中 "Three sizes of Input" 示例。

---

## 5. 样式变体

通过 `StyleVariant` 属性切换视觉风格，支持 `Outline`（默认）、`Filled`、`Borderless` 三种。

```xml
<StackPanel Orientation="Vertical" Spacing="10">
    <atom:ComboBox StyleVariant="Outline" PlaceholderText="Please select" Width="300">
        <atom:ComboBoxItem>床前明月光</atom:ComboBoxItem>
        <atom:ComboBoxItem>疑是地上霜</atom:ComboBoxItem>
        <atom:ComboBoxItem>举头望明月</atom:ComboBoxItem>
        <atom:ComboBoxItem>低头思故乡</atom:ComboBoxItem>
    </atom:ComboBox>

    <atom:ComboBox StyleVariant="Filled" PlaceholderText="Please select" Width="300">
        <atom:ComboBoxItem>床前明月光</atom:ComboBoxItem>
        <atom:ComboBoxItem>疑是地上霜</atom:ComboBoxItem>
        <atom:ComboBoxItem>举头望明月</atom:ComboBoxItem>
        <atom:ComboBoxItem>低头思故乡</atom:ComboBoxItem>
    </atom:ComboBox>

    <atom:ComboBox StyleVariant="Borderless" PlaceholderText="Please select" Width="300">
        <atom:ComboBoxItem>床前明月光</atom:ComboBoxItem>
        <atom:ComboBoxItem>疑是地上霜</atom:ComboBoxItem>
        <atom:ComboBoxItem>举头望明月</atom:ComboBoxItem>
        <atom:ComboBoxItem>低头思故乡</atom:ComboBoxItem>
    </atom:ComboBox>
</StackPanel>
```

> 📖 参考：`controlgallery/AtomUIGallery/ShowCases/Views/Navigation/ComboBoxShowCase.axaml` 中 "Variants" 示例。

---

## 6. 前置/后置标签（AddOn）

通过 `LeftAddOn` 和 `RightAddOn` 在 ComboBox 外部添加附加区域，常用于协议前缀或域名后缀：

```xml
<StackPanel Orientation="Vertical" Spacing="10">
    <!-- 文本前后置 -->
    <atom:ComboBox PlaceholderText="Please select" Width="300"
                   LeftAddOn="http://"
                   RightAddOn=".com">
        <atom:ComboBoxItem>床前明月光</atom:ComboBoxItem>
        <atom:ComboBoxItem>疑是地上霜</atom:ComboBoxItem>
        <atom:ComboBoxItem>举头望明月</atom:ComboBoxItem>
        <atom:ComboBoxItem>低头思故乡</atom:ComboBoxItem>
    </atom:ComboBox>

    <!-- 图标后置 -->
    <atom:ComboBox PlaceholderText="Please select" Width="300"
                   RightAddOn="{antdicons:AntDesignIconProvider Kind=SettingOutlined}">
        <atom:ComboBoxItem>床前明月光</atom:ComboBoxItem>
        <atom:ComboBoxItem>疑是地上霜</atom:ComboBoxItem>
        <atom:ComboBoxItem>举头望明月</atom:ComboBoxItem>
        <atom:ComboBoxItem>低头思故乡</atom:ComboBoxItem>
    </atom:ComboBox>

    <!-- 混合：外部前置 + 内部后缀 -->
    <atom:ComboBox PlaceholderText="Please select" Width="300"
                   LeftAddOn="http://"
                   ContentRightAddOn=".com">
        <atom:ComboBoxItem>床前明月光</atom:ComboBoxItem>
        <atom:ComboBoxItem>疑是地上霜</atom:ComboBoxItem>
        <atom:ComboBoxItem>举头望明月</atom:ComboBoxItem>
        <atom:ComboBoxItem>低头思故乡</atom:ComboBoxItem>
    </atom:ComboBox>
</StackPanel>
```

> 📖 参考：`controlgallery/AtomUIGallery/ShowCases/Views/Navigation/ComboBoxShowCase.axaml` 中 "Pre / Post tab" 示例。

---

## 7. 内部前缀/后缀（Content AddOn）

通过 `ContentLeftAddOn` 和 `ContentRightAddOn` 在内容区域内部添加图标或辅助文本：

```xml
<StackPanel Orientation="Vertical" Spacing="10">
    <!-- 图标前缀 + 图标后缀 -->
    <atom:ComboBox PlaceholderText="Please select" Width="300"
                   ContentLeftAddOn="{antdicons:AntDesignIconProvider Kind=UserOutlined, FillBrush=#D7D7D7}"
                   ContentRightAddOn="{antdicons:AntDesignIconProvider Kind=InfoCircleOutlined, FillBrush=#8C8C8C}">
        <atom:ComboBoxItem>床前明月光</atom:ComboBoxItem>
        <atom:ComboBoxItem>疑是地上霜</atom:ComboBoxItem>
        <atom:ComboBoxItem>举头望明月</atom:ComboBoxItem>
        <atom:ComboBoxItem>低头思故乡</atom:ComboBoxItem>
    </atom:ComboBox>

    <!-- 文本前缀 + 文本后缀（货币场景） -->
    <atom:ComboBox PlaceholderText="Please select" Width="300"
                   ContentLeftAddOn="￥"
                   ContentRightAddOn="RMB">
        <atom:ComboBoxItem>床前明月光</atom:ComboBoxItem>
        <atom:ComboBoxItem>疑是地上霜</atom:ComboBoxItem>
        <atom:ComboBoxItem>举头望明月</atom:ComboBoxItem>
        <atom:ComboBoxItem>低头思故乡</atom:ComboBoxItem>
    </atom:ComboBox>

    <!-- 带前后缀的禁用状态 -->
    <atom:ComboBox PlaceholderText="Please select" Width="300"
                   ContentLeftAddOn="￥"
                   ContentRightAddOn="RMB" IsEnabled="False">
        <atom:ComboBoxItem>床前明月光</atom:ComboBoxItem>
        <atom:ComboBoxItem>疑是地上霜</atom:ComboBoxItem>
        <atom:ComboBoxItem>举头望明月</atom:ComboBoxItem>
        <atom:ComboBoxItem>低头思故乡</atom:ComboBoxItem>
    </atom:ComboBox>
</StackPanel>
```

> 📖 参考：`controlgallery/AtomUIGallery/ShowCases/Views/Navigation/ComboBoxShowCase.axaml` 中 "prefix and suffix" 示例。

---

## 8. 验证状态（Status）

通过 `Status` 属性设置验证反馈，配合不同样式变体展示各种效果：

```xml
<StackPanel Orientation="Vertical" Spacing="10">
    <!-- Outline 变体（默认） -->
    <atom:ComboBox PlaceholderText="Please select" Width="300" Status="Error">
        <atom:ComboBoxItem>床前明月光</atom:ComboBoxItem>
        <atom:ComboBoxItem>疑是地上霜</atom:ComboBoxItem>
    </atom:ComboBox>
    <atom:ComboBox PlaceholderText="Please select" Width="300" Status="Warning">
        <atom:ComboBoxItem>床前明月光</atom:ComboBoxItem>
        <atom:ComboBoxItem>疑是地上霜</atom:ComboBoxItem>
    </atom:ComboBox>

    <!-- 带图标前缀的错误/警告 -->
    <atom:ComboBox PlaceholderText="Please select" Width="300"
                   Status="Error"
                   ContentLeftAddOn="{antdicons:AntDesignIconProvider Kind=ClockCircleOutlined}">
        <atom:ComboBoxItem>床前明月光</atom:ComboBoxItem>
        <atom:ComboBoxItem>疑是地上霜</atom:ComboBoxItem>
    </atom:ComboBox>
    <atom:ComboBox PlaceholderText="Please select" Width="300"
                   Status="Warning"
                   ContentLeftAddOn="{antdicons:AntDesignIconProvider Kind=ClockCircleOutlined}">
        <atom:ComboBoxItem>床前明月光</atom:ComboBoxItem>
        <atom:ComboBoxItem>疑是地上霜</atom:ComboBoxItem>
    </atom:ComboBox>

    <!-- Filled 变体 + 错误/警告 -->
    <atom:ComboBox PlaceholderText="Please select" Width="300"
                   Status="Error" StyleVariant="Filled"
                   ContentLeftAddOn="{antdicons:AntDesignIconProvider Kind=ClockCircleOutlined}">
        <atom:ComboBoxItem>床前明月光</atom:ComboBoxItem>
        <atom:ComboBoxItem>疑是地上霜</atom:ComboBoxItem>
    </atom:ComboBox>
    <atom:ComboBox PlaceholderText="Please select" Width="300"
                   Status="Warning" StyleVariant="Filled"
                   ContentLeftAddOn="{antdicons:AntDesignIconProvider Kind=ClockCircleOutlined}">
        <atom:ComboBoxItem>床前明月光</atom:ComboBoxItem>
        <atom:ComboBoxItem>疑是地上霜</atom:ComboBoxItem>
    </atom:ComboBox>

    <!-- Borderless 变体 + 错误/警告 -->
    <atom:ComboBox PlaceholderText="Please select" Width="300"
                   Status="Error" StyleVariant="Borderless"
                   ContentLeftAddOn="{antdicons:AntDesignIconProvider Kind=ClockCircleOutlined}">
        <atom:ComboBoxItem>床前明月光</atom:ComboBoxItem>
        <atom:ComboBoxItem>疑是地上霜</atom:ComboBoxItem>
    </atom:ComboBox>
    <atom:ComboBox PlaceholderText="Please select" Width="300"
                   Status="Warning" StyleVariant="Borderless"
                   ContentLeftAddOn="{antdicons:AntDesignIconProvider Kind=ClockCircleOutlined}">
        <atom:ComboBoxItem>床前明月光</atom:ComboBoxItem>
        <atom:ComboBoxItem>疑是地上霜</atom:ComboBoxItem>
    </atom:ComboBox>
</StackPanel>
```

> 📖 参考：`controlgallery/AtomUIGallery/ShowCases/Views/Navigation/ComboBoxShowCase.axaml` 中 "Status" 示例。

---

## 9. MVVM 选中项绑定

ComboBox 继承自 Avalonia 的 `SelectingItemsControl`，完整支持 `SelectedItem` / `SelectedIndex` 双向绑定：

```xml
<atom:ComboBox PlaceholderText="请选择城市" Width="300"
               ItemsSource="{Binding Cities}"
               SelectedItem="{Binding SelectedCity}">
    <atom:ComboBox.ItemTemplate>
        <DataTemplate>
            <atom:TextBlock Text="{Binding Name}" VerticalAlignment="Center"/>
        </DataTemplate>
    </atom:ComboBox.ItemTemplate>
</atom:ComboBox>
```

```csharp
// ViewModel（使用 ReactiveUI）
public class MyViewModel : ReactiveObject
{
    private CityData? _selectedCity;
    public CityData? SelectedCity
    {
        get => _selectedCity;
        set => this.RaiseAndSetIfChanged(ref _selectedCity, value);
    }

    public List<CityData> Cities { get; } =
    [
        new() { Name = "北京" },
        new() { Name = "上海" },
        new() { Name = "广州" },
        new() { Name = "深圳" },
    ];
}
```

---

## 10. 控制动画行为

```xml
<!-- 禁用过渡动画（下拉面板不再有展开/收起动画） -->
<atom:ComboBox IsMotionEnabled="False" PlaceholderText="无动画" Width="300">
    <atom:ComboBoxItem>选项一</atom:ComboBoxItem>
    <atom:ComboBoxItem>选项二</atom:ComboBoxItem>
</atom:ComboBox>
```

---

## 常见组合模式

### 表单选择器

```xml
<StackPanel Orientation="Vertical" Spacing="8">
    <atom:TextBlock Text="城市" />
    <atom:ComboBox PlaceholderText="请选择城市" Width="300"
                   StyleVariant="Outline">
        <atom:ComboBoxItem>北京</atom:ComboBoxItem>
        <atom:ComboBoxItem>上海</atom:ComboBoxItem>
        <atom:ComboBoxItem>广州</atom:ComboBoxItem>
    </atom:ComboBox>
</StackPanel>
```

### 带前缀图标的选择器

```xml
<atom:ComboBox PlaceholderText="选择用户" Width="300"
               ContentLeftAddOn="{antdicons:AntDesignIconProvider Kind=UserOutlined, FillBrush=#D7D7D7}">
    <atom:ComboBoxItem>张三</atom:ComboBoxItem>
    <atom:ComboBoxItem>李四</atom:ComboBoxItem>
    <atom:ComboBoxItem>王五</atom:ComboBoxItem>
</atom:ComboBox>
```

### 协议/域名选择器

```xml
<atom:ComboBox PlaceholderText="mysite" Width="400"
               LeftAddOn="http://"
               RightAddOn=".com">
    <atom:ComboBoxItem>mysite</atom:ComboBoxItem>
    <atom:ComboBoxItem>example</atom:ComboBoxItem>
    <atom:ComboBoxItem>demo</atom:ComboBoxItem>
</atom:ComboBox>
```

### 货币金额选择器

```xml
<atom:ComboBox PlaceholderText="请选择金额" Width="300"
               ContentLeftAddOn="￥"
               ContentRightAddOn="RMB">
    <atom:ComboBoxItem>100</atom:ComboBoxItem>
    <atom:ComboBoxItem>500</atom:ComboBoxItem>
    <atom:ComboBoxItem>1000</atom:ComboBoxItem>
    <atom:ComboBoxItem>5000</atom:ComboBoxItem>
</atom:ComboBox>
```
