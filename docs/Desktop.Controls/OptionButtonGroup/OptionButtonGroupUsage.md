# OptionButtonGroup 使用文档

本文档介绍 AtomUI OptionButtonGroup 控件的各种使用方式，示例代码摘自 Gallery 演示程序。

> 📖 Gallery 源码位置：`controlgallery/AtomUIGallery/ShowCases/Views/DataEntry/RadioButtonShowCase.axaml`

---

## 前置准备

在 AXAML 中使用 OptionButtonGroup，需要引入以下命名空间：

```xml
xmlns:atom="https://atomui.net"
xmlns:antdicons="https://atomui.net/icons/antdesign"
```

在 C# 代码中引入：

```csharp
using AtomUI.Desktop.Controls;   // OptionButton, OptionButtonGroup
using AtomUI.Controls;            // SizeType, OptionButtonStyle 等共享类型
```

---

## 1. 基本用法（Outline 样式）

默认 Outline 样式的选项按钮组，选中项以边框颜色和文字颜色区分：

```xml
<atom:OptionButtonGroup>
    <atom:OptionButton IsChecked="True">Hangzhou</atom:OptionButton>
    <atom:OptionButton>Shanghai</atom:OptionButton>
    <atom:OptionButton>Beijing</atom:OptionButton>
    <atom:OptionButton>Chengdu</atom:OptionButton>
</atom:OptionButtonGroup>
```

通过 `IsChecked="True"` 设置默认选中项。

---

## 2. Solid 样式

通过 `ButtonStyle="Solid"` 启用实色填充样式，选中项使用主色背景 + 白色文字：

```xml
<atom:OptionButtonGroup ButtonStyle="Solid">
    <atom:OptionButton IsChecked="True">Hangzhou</atom:OptionButton>
    <atom:OptionButton>Shanghai</atom:OptionButton>
    <atom:OptionButton>Beijing</atom:OptionButton>
    <atom:OptionButton>Chengdu</atom:OptionButton>
</atom:OptionButtonGroup>
```

**使用场景指引**：
- **Outline**：适合常规选项切换场景，视觉权重较低
- **Solid**：适合需要强调选中状态的场景（如工具栏模式切换）

---

## 3. 三种尺寸

通过 `SizeType` 属性设置尺寸，支持 `Large`、`Middle`（默认）、`Small` 三种：

```xml
<StackPanel Orientation="Vertical" Spacing="10">
    <atom:OptionButtonGroup SizeType="Large">
        <atom:OptionButton IsChecked="True">Hangzhou</atom:OptionButton>
        <atom:OptionButton>Shanghai</atom:OptionButton>
        <atom:OptionButton>Beijing</atom:OptionButton>
        <atom:OptionButton>Chengdu</atom:OptionButton>
    </atom:OptionButtonGroup>

    <atom:OptionButtonGroup SizeType="Middle">
        <atom:OptionButton IsChecked="True">Hangzhou</atom:OptionButton>
        <atom:OptionButton>Shanghai</atom:OptionButton>
        <atom:OptionButton>Beijing</atom:OptionButton>
        <atom:OptionButton>Chengdu</atom:OptionButton>
    </atom:OptionButtonGroup>

    <atom:OptionButtonGroup SizeType="Small">
        <atom:OptionButton IsChecked="True">Hangzhou</atom:OptionButton>
        <atom:OptionButton>Shanghai</atom:OptionButton>
        <atom:OptionButton>Beijing</atom:OptionButton>
        <atom:OptionButton>Chengdu</atom:OptionButton>
    </atom:OptionButtonGroup>
</StackPanel>
```

---

## 4. 带图标的选项按钮

通过 `Icon` 属性为按钮添加图标，支持图标 + 文字组合：

```xml
<!-- Solid 样式 + 图标 -->
<atom:OptionButtonGroup ButtonStyle="Solid">
    <atom:OptionButton IsChecked="True"
        Icon="{antdicons:AntDesignIconProvider Kind=AppleOutlined}">macOS</atom:OptionButton>
    <atom:OptionButton
        Icon="{antdicons:AntDesignIconProvider Kind=LinuxOutlined}">Linux</atom:OptionButton>
    <atom:OptionButton
        Icon="{antdicons:AntDesignIconProvider Kind=WindowsOutlined}">Windows</atom:OptionButton>
</atom:OptionButtonGroup>

<!-- Outline 样式 + 图标 -->
<atom:OptionButtonGroup ButtonStyle="Outline">
    <atom:OptionButton
        Icon="{antdicons:AntDesignIconProvider Kind=AppleOutlined}">macOS</atom:OptionButton>
    <atom:OptionButton IsChecked="True"
        Icon="{antdicons:AntDesignIconProvider Kind=LinuxOutlined}">Linux</atom:OptionButton>
    <atom:OptionButton
        Icon="{antdicons:AntDesignIconProvider Kind=WindowsOutlined}">Windows</atom:OptionButton>
</atom:OptionButtonGroup>
```

图标与文本之间的间距由 `SharedToken.SpacingXXS` 控制。

---

## 5. 不同尺寸的图标按钮

```xml
<StackPanel Orientation="Vertical" Spacing="10">
    <atom:OptionButtonGroup ButtonStyle="Outline" SizeType="Large">
        <atom:OptionButton
            Icon="{antdicons:AntDesignIconProvider Kind=AppleOutlined}">macOS</atom:OptionButton>
        <atom:OptionButton IsChecked="True"
            Icon="{antdicons:AntDesignIconProvider Kind=LinuxOutlined}">Linux</atom:OptionButton>
        <atom:OptionButton
            Icon="{antdicons:AntDesignIconProvider Kind=WindowsOutlined}">Windows</atom:OptionButton>
    </atom:OptionButtonGroup>

    <atom:OptionButtonGroup ButtonStyle="Outline">
        <atom:OptionButton
            Icon="{antdicons:AntDesignIconProvider Kind=AppleOutlined}">macOS</atom:OptionButton>
        <atom:OptionButton IsChecked="True"
            Icon="{antdicons:AntDesignIconProvider Kind=LinuxOutlined}">Linux</atom:OptionButton>
        <atom:OptionButton
            Icon="{antdicons:AntDesignIconProvider Kind=WindowsOutlined}">Windows</atom:OptionButton>
    </atom:OptionButtonGroup>

    <atom:OptionButtonGroup ButtonStyle="Outline" SizeType="Small">
        <atom:OptionButton
            Icon="{antdicons:AntDesignIconProvider Kind=AppleOutlined}">macOS</atom:OptionButton>
        <atom:OptionButton IsChecked="True"
            Icon="{antdicons:AntDesignIconProvider Kind=LinuxOutlined}">Linux</atom:OptionButton>
        <atom:OptionButton
            Icon="{antdicons:AntDesignIconProvider Kind=WindowsOutlined}">Windows</atom:OptionButton>
    </atom:OptionButtonGroup>
</StackPanel>
```

---

## 6. 禁用状态

### 禁用单个选项

```xml
<atom:OptionButtonGroup ButtonStyle="Outline">
    <atom:OptionButton>Apple</atom:OptionButton>
    <atom:OptionButton IsChecked="True">Pear</atom:OptionButton>
    <atom:OptionButton IsEnabled="False">Orange</atom:OptionButton>
</atom:OptionButtonGroup>
```

禁用的选项显示为灰色调，不可点击。已选中的选项也可以被禁用。

### 禁用整个组

```xml
<!-- Solid 样式全组禁用 -->
<atom:OptionButtonGroup ButtonStyle="Solid" IsEnabled="False">
    <atom:OptionButton IsChecked="True"
        Icon="{antdicons:AntDesignIconProvider Kind=AppleOutlined}">macOS</atom:OptionButton>
    <atom:OptionButton
        Icon="{antdicons:AntDesignIconProvider Kind=LinuxOutlined}">Linux</atom:OptionButton>
    <atom:OptionButton
        Icon="{antdicons:AntDesignIconProvider Kind=WindowsOutlined}">Windows</atom:OptionButton>
</atom:OptionButtonGroup>

<!-- Outline 样式全组禁用 -->
<atom:OptionButtonGroup ButtonStyle="Outline" IsEnabled="False">
    <atom:OptionButton
        Icon="{antdicons:AntDesignIconProvider Kind=AppleOutlined}">macOS</atom:OptionButton>
    <atom:OptionButton IsChecked="True"
        Icon="{antdicons:AntDesignIconProvider Kind=LinuxOutlined}">Linux</atom:OptionButton>
    <atom:OptionButton
        Icon="{antdicons:AntDesignIconProvider Kind=WindowsOutlined}">Windows</atom:OptionButton>
</atom:OptionButtonGroup>
```

### 选中项被禁用

```xml
<atom:OptionButtonGroup>
    <atom:OptionButton IsChecked="True" IsEnabled="False">Hangzhou</atom:OptionButton>
    <atom:OptionButton IsEnabled="False">Shanghai</atom:OptionButton>
    <atom:OptionButton IsEnabled="False">Beijing</atom:OptionButton>
    <atom:OptionButton IsEnabled="False">Chengdu</atom:OptionButton>
</atom:OptionButtonGroup>
```

---

## 7. 选中变更事件

通过 `OptionCheckedChanged` 事件监听选中项变更：

```xml
<atom:OptionButtonGroup ButtonStyle="Outline"
                         OptionCheckedChanged="HandleSizeTypeChanged">
    <atom:OptionButton IsChecked="True">Default</atom:OptionButton>
    <atom:OptionButton>Large</atom:OptionButton>
    <atom:OptionButton>Small</atom:OptionButton>
</atom:OptionButtonGroup>
```

```csharp
private void HandleSizeTypeChanged(object? sender, OptionCheckedChangedEventArgs e)
{
    // e.CheckedOption — 新选中的按钮实例
    // e.Index — 新选中项的索引
    var selectedText = e.CheckedOption.Content?.ToString();
    // 根据选中项执行相应逻辑...
}
```

> 📖 参考：`controlgallery/AtomUIGallery/ShowCases/Views/Feedback/SkeletonShowCase.axaml` 中使用 OptionButtonGroup 切换 Skeleton 尺寸。

---

## 8. 数据驱动模式

通过 `ItemsSource` 绑定数据集合自动生成按钮：

```csharp
// ViewModel 或 Code-behind
var items = new List<OptionButtonData>
{
    new() { Header = "Apple", Icon = new AppleOutlined() },
    new() { Header = "Linux", Icon = new LinuxOutlined() },
    new() { Header = "Windows", Icon = new WindowsOutlined(), IsEnabled = false }
};
optionButtonGroup.ItemsSource = items;
```

```xml
<atom:OptionButtonGroup Name="optionButtonGroup"
                         ButtonStyle="Solid" />
```

---

## 常见组合模式

### 作为视图模式切换器

```xml
<atom:OptionButtonGroup ButtonStyle="Solid" SizeType="Small">
    <atom:OptionButton IsChecked="True"
        Icon="{antdicons:AntDesignIconProvider Kind=AppstoreOutlined}" />
    <atom:OptionButton
        Icon="{antdicons:AntDesignIconProvider Kind=UnorderedListOutlined}" />
</atom:OptionButtonGroup>
```

### 作为 Skeleton 尺寸切换器

```xml
<atom:OptionButtonGroup ButtonStyle="Outline"
                         OptionCheckedChanged="HandleSizeTypeChanged">
    <atom:OptionButton IsChecked="True">Default</atom:OptionButton>
    <atom:OptionButton>Large</atom:OptionButton>
    <atom:OptionButton>Small</atom:OptionButton>
</atom:OptionButtonGroup>
```

### 作为折叠面板展开按钮位置切换器

```xml
<atom:OptionButtonGroup ButtonStyle="Outline" Name="ExpandButtonPosGroup">
    <atom:OptionButton IsChecked="True">Start</atom:OptionButton>
    <atom:OptionButton>End</atom:OptionButton>
</atom:OptionButtonGroup>
```

### 与表单集成

```xml
<atom:FormItem Label="Region">
    <atom:OptionButtonGroup>
        <atom:OptionButton IsChecked="True">China</atom:OptionButton>
        <atom:OptionButton>USA</atom:OptionButton>
        <atom:OptionButton>EU</atom:OptionButton>
    </atom:OptionButtonGroup>
</atom:FormItem>
```

### MVVM 绑定选中项

```xml
<atom:OptionButtonGroup SelectedItem="{Binding SelectedRegion}">
    <atom:OptionButton>China</atom:OptionButton>
    <atom:OptionButton>USA</atom:OptionButton>
    <atom:OptionButton>EU</atom:OptionButton>
</atom:OptionButtonGroup>
```

```csharp
// ViewModel
[Reactive]
public object? SelectedRegion { get; set; }
```
