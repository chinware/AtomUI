# DropdownButton 使用文档

本文档介绍 AtomUI DropdownButton 控件的各种使用方式，示例代码摘自 Gallery 演示程序。

> 📖 Gallery 源码位置：`controlgallery/AtomUIGallery/ShowCases/Views/Navigation/DropdownButtonShowCase.axaml`

---

## 前置准备

在 AXAML 中使用 DropdownButton，需要引入以下命名空间：

```xml
xmlns:atom="https://atomui.net"
xmlns:antdicons="https://atomui.net/icons/antdesign"
```

在 C# 代码中引入：

```csharp
using AtomUI.Desktop.Controls;   // DropdownButton 控件
using AtomUI.Controls;            // SizeType 等共享类型
```

---

## 1. 基本用法

最基本的下拉菜单按钮，悬浮触发弹出菜单：

```xml
<atom:DropdownButton ButtonType="Link" TriggerType="Hover">
    Hover me
    <atom:DropdownButton.DropdownFlyout>
        <atom:MenuFlyout>
            <atom:MenuItem Header="Cut" InputGesture="Ctrl+X"
                           Icon="{antdicons:AntDesignIconProvider Kind=ScissorOutlined}" />
            <atom:MenuItem Header="Copy" InputGesture="Ctrl+C"
                           Icon="{antdicons:AntDesignIconProvider Kind=CopyOutlined}" />
            <atom:MenuItem Header="Delete" InputGesture="Ctrl+D"
                           Icon="{antdicons:AntDesignIconProvider Kind=DeleteOutlined}" />
            <atom:MenuItem Header="Paste">
                <atom:MenuItem Header="Paste" InputGesture="Ctrl+P"
                               Icon="{antdicons:AntDesignIconProvider Kind=FileDoneOutlined}" />
                <atom:MenuItem Header="Paste from History" InputGesture="Ctrl+Shift+V" />
            </atom:MenuItem>
        </atom:MenuFlyout>
    </atom:DropdownButton.DropdownFlyout>
</atom:DropdownButton>
```

**要点**：
- `DropdownFlyout` 属性接受 `MenuFlyout`，其中包含 `MenuItem` 菜单项
- `MenuItem` 支持 `Header`（文本）、`InputGesture`（快捷键提示）、`Icon`（图标）
- 支持嵌套子菜单（MenuItem 中嵌套 MenuItem）

---

## 2. 按钮类型

DropdownButton 继承 Button 的所有类型，通过 `ButtonType` 切换外观风格。点击触发弹出：

```xml
<StackPanel Orientation="Horizontal" Spacing="10">
    <!-- 主按钮 -->
    <atom:DropdownButton ButtonType="Primary" TriggerType="Click">
        Edit File
        <atom:DropdownButton.DropdownFlyout>
            <atom:MenuFlyout>
                <atom:MenuItem Header="Cut" InputGesture="Ctrl+X"
                               Icon="{antdicons:AntDesignIconProvider Kind=ScissorOutlined}" />
                <atom:MenuItem Header="Copy" InputGesture="Ctrl+C"
                               Icon="{antdicons:AntDesignIconProvider Kind=CopyOutlined}" />
                <atom:MenuItem Header="Delete" InputGesture="Ctrl+D"
                               Icon="{antdicons:AntDesignIconProvider Kind=DeleteOutlined}" />
            </atom:MenuFlyout>
        </atom:DropdownButton.DropdownFlyout>
    </atom:DropdownButton>

    <!-- 主按钮 + 胶囊形 -->
    <atom:DropdownButton ButtonType="Primary" Shape="Round" TriggerType="Click">
        Edit File
        <atom:DropdownButton.DropdownFlyout>
            <atom:MenuFlyout>
                <atom:MenuItem Header="Cut" />
                <atom:MenuItem Header="Copy" />
                <atom:MenuItem Header="Delete" />
            </atom:MenuFlyout>
        </atom:DropdownButton.DropdownFlyout>
    </atom:DropdownButton>

    <!-- 默认按钮 -->
    <atom:DropdownButton ButtonType="Default" TriggerType="Click">
        Edit File
        <atom:DropdownButton.DropdownFlyout>
            <atom:MenuFlyout>
                <atom:MenuItem Header="Cut" />
                <atom:MenuItem Header="Copy" />
                <atom:MenuItem Header="Delete" />
            </atom:MenuFlyout>
        </atom:DropdownButton.DropdownFlyout>
    </atom:DropdownButton>

    <!-- 文本按钮 -->
    <atom:DropdownButton ButtonType="Text" TriggerType="Click">
        Edit File
        <atom:DropdownButton.DropdownFlyout>
            <atom:MenuFlyout>
                <atom:MenuItem Header="Cut" />
                <atom:MenuItem Header="Copy" />
                <atom:MenuItem Header="Delete" />
            </atom:MenuFlyout>
        </atom:DropdownButton.DropdownFlyout>
    </atom:DropdownButton>
</StackPanel>
```

---

## 3. 带箭头指示

通过 `IsShowArrow="True"` 在弹出菜单上显示指向按钮的箭头。配合不同的 `Placement` 使用效果最佳：

```xml
<atom:DropdownButton ButtonType="Default" TriggerType="Hover"
                     IsShowArrow="True"
                     Placement="BottomEdgeAlignedLeft">
    BottomLeft
    <atom:DropdownButton.DropdownFlyout>
        <atom:MenuFlyout>
            <atom:MenuItem Header="Cut" InputGesture="Ctrl+X"
                           Icon="{antdicons:AntDesignIconProvider Kind=ScissorOutlined}" />
            <atom:MenuItem Header="Copy" InputGesture="Ctrl+C"
                           Icon="{antdicons:AntDesignIconProvider Kind=CopyOutlined}" />
            <atom:MenuItem Header="Delete" InputGesture="Ctrl+D"
                           Icon="{antdicons:AntDesignIconProvider Kind=DeleteOutlined}" />
        </atom:MenuFlyout>
    </atom:DropdownButton.DropdownFlyout>
</atom:DropdownButton>

<!-- 箭头 + 不同弹出位置 -->
<atom:DropdownButton ButtonType="Default" TriggerType="Hover"
                     IsShowArrow="True" Placement="Bottom">
    Bottom
    <!-- ... DropdownFlyout ... -->
</atom:DropdownButton>

<atom:DropdownButton ButtonType="Default" TriggerType="Hover"
                     IsShowArrow="True" Placement="TopEdgeAlignedLeft">
    TopLeft
    <!-- ... DropdownFlyout ... -->
</atom:DropdownButton>
```

可以使用 `IsPointAtCenter="True"` 让箭头指向按钮中心：

```xml
<atom:DropdownButton IsShowArrow="True" IsPointAtCenter="True"
                     TriggerType="Hover" Placement="Bottom">
    Centered Arrow
    <!-- ... DropdownFlyout ... -->
</atom:DropdownButton>
```

---

## 4. 弹出位置

支持 6 种弹出位置，通过 `Placement` 属性设置：

```xml
<!-- 底部左对齐（默认） -->
<atom:DropdownButton Placement="BottomEdgeAlignedLeft" TriggerType="Hover">
    BottomLeft
    <!-- ... DropdownFlyout ... -->
</atom:DropdownButton>

<!-- 底部居中 -->
<atom:DropdownButton Placement="Bottom" TriggerType="Hover">
    Bottom
    <!-- ... DropdownFlyout ... -->
</atom:DropdownButton>

<!-- 底部右对齐 -->
<atom:DropdownButton Placement="BottomEdgeAlignedRight" TriggerType="Hover">
    BottomRight
    <!-- ... DropdownFlyout ... -->
</atom:DropdownButton>

<!-- 顶部左对齐 -->
<atom:DropdownButton Placement="TopEdgeAlignedLeft" TriggerType="Hover">
    TopLeft
    <!-- ... DropdownFlyout ... -->
</atom:DropdownButton>

<!-- 顶部居中 -->
<atom:DropdownButton Placement="Top" TriggerType="Hover">
    Top
    <!-- ... DropdownFlyout ... -->
</atom:DropdownButton>

<!-- 顶部右对齐 -->
<atom:DropdownButton Placement="TopEdgeAlignedRight" TriggerType="Hover">
    TopRight
    <!-- ... DropdownFlyout ... -->
</atom:DropdownButton>
```

---

## 5. 菜单项点击事件

通过 `MenuItemClicked` 事件统一监听弹出菜单中所有菜单项的点击：

```xml
<atom:DropdownButton MenuItemClicked="HandleMenuItemClicked">
    Actions
    <atom:DropdownButton.DropdownFlyout>
        <atom:MenuFlyout>
            <atom:MenuItem Header="Edit" />
            <atom:MenuItem Header="Delete" />
            <atom:MenuItem Header="Export" />
        </atom:MenuFlyout>
    </atom:DropdownButton.DropdownFlyout>
</atom:DropdownButton>
```

```csharp
private void HandleMenuItemClicked(object? sender, FlyoutMenuItemClickedEventArgs args)
{
    var menuItem = args.Item;
    // 获取被点击的菜单项，处理相应业务逻辑
    var header = menuItem.Header?.ToString();
    Debug.WriteLine($"Clicked: {header}");
}
```

---

## 6. 自定义下拉指示器

### 隐藏下拉指示器

```xml
<atom:DropdownButton IsShowOpenIndicator="False" TriggerType="Hover">
    No Arrow Icon
    <!-- ... DropdownFlyout ... -->
</atom:DropdownButton>
```

### 自定义指示器图标

```xml
<atom:DropdownButton OpenIndicator="{antdicons:AntDesignIconProvider Kind=MoreOutlined}"
                     TriggerType="Click">
    Custom Icon
    <!-- ... DropdownFlyout ... -->
</atom:DropdownButton>

<atom:DropdownButton OpenIndicator="{antdicons:AntDesignIconProvider Kind=EllipsisOutlined}"
                     TriggerType="Click">
    Ellipsis
    <!-- ... DropdownFlyout ... -->
</atom:DropdownButton>
```

---

## 7. 继承自 Button 的能力

DropdownButton 继承 Button 的全部属性，以下展示一些常用组合：

### 危险样式

```xml
<atom:DropdownButton ButtonType="Primary" IsDanger="True" TriggerType="Click">
    Danger Actions
    <atom:DropdownButton.DropdownFlyout>
        <atom:MenuFlyout>
            <atom:MenuItem Header="Delete All" />
            <atom:MenuItem Header="Reset" />
        </atom:MenuFlyout>
    </atom:DropdownButton.DropdownFlyout>
</atom:DropdownButton>
```

### 不同尺寸

```xml
<atom:DropdownButton SizeType="Large" TriggerType="Click">
    Large
    <!-- ... DropdownFlyout ... -->
</atom:DropdownButton>

<atom:DropdownButton SizeType="Middle" TriggerType="Click">
    Default
    <!-- ... DropdownFlyout ... -->
</atom:DropdownButton>

<atom:DropdownButton SizeType="Small" TriggerType="Click">
    Small
    <!-- ... DropdownFlyout ... -->
</atom:DropdownButton>
```

### 带图标

```xml
<atom:DropdownButton ButtonType="Primary" TriggerType="Click"
                     Icon="{antdicons:AntDesignIconProvider Kind=SettingOutlined}">
    Settings
    <!-- ... DropdownFlyout ... -->
</atom:DropdownButton>
```

### 禁用状态

```xml
<atom:DropdownButton IsEnabled="False">
    Disabled
    <!-- ... DropdownFlyout ... -->
</atom:DropdownButton>
```

---

## 常见组合模式

### 操作更多菜单

主操作使用独立按钮，更多操作使用 DropdownButton：

```xml
<StackPanel Orientation="Horizontal" Spacing="8">
    <atom:Button ButtonType="Primary">Submit</atom:Button>
    <atom:DropdownButton TriggerType="Click">
        More
        <atom:DropdownButton.DropdownFlyout>
            <atom:MenuFlyout>
                <atom:MenuItem Header="Save as Draft" />
                <atom:MenuItem Header="Export" />
                <atom:MenuItem Header="Print" />
            </atom:MenuFlyout>
        </atom:DropdownButton.DropdownFlyout>
    </atom:DropdownButton>
</StackPanel>
```

### 文件操作工具栏

```xml
<StackPanel Orientation="Horizontal" Spacing="8">
    <atom:DropdownButton ButtonType="Primary" TriggerType="Click"
                         Icon="{antdicons:AntDesignIconProvider Kind=FileOutlined}">
        File
        <atom:DropdownButton.DropdownFlyout>
            <atom:MenuFlyout>
                <atom:MenuItem Header="New" InputGesture="Ctrl+N" />
                <atom:MenuItem Header="Open" InputGesture="Ctrl+O" />
                <atom:MenuItem Header="Save" InputGesture="Ctrl+S" />
                <atom:MenuItem Header="Save As" InputGesture="Ctrl+Shift+S" />
            </atom:MenuFlyout>
        </atom:DropdownButton.DropdownFlyout>
    </atom:DropdownButton>

    <atom:DropdownButton ButtonType="Default" TriggerType="Click"
                         Icon="{antdicons:AntDesignIconProvider Kind=EditOutlined}">
        Edit
        <atom:DropdownButton.DropdownFlyout>
            <atom:MenuFlyout>
                <atom:MenuItem Header="Cut" InputGesture="Ctrl+X"
                               Icon="{antdicons:AntDesignIconProvider Kind=ScissorOutlined}" />
                <atom:MenuItem Header="Copy" InputGesture="Ctrl+C"
                               Icon="{antdicons:AntDesignIconProvider Kind=CopyOutlined}" />
                <atom:MenuItem Header="Paste" InputGesture="Ctrl+V" />
            </atom:MenuFlyout>
        </atom:DropdownButton.DropdownFlyout>
    </atom:DropdownButton>
</StackPanel>
```

### 悬浮触发 + 自定义延迟

```xml
<atom:DropdownButton TriggerType="Hover"
                     MouseEnterDelay="300"
                     MouseLeaveDelay="200">
    Slow Hover
    <atom:DropdownButton.DropdownFlyout>
        <atom:MenuFlyout>
            <atom:MenuItem Header="Option A" />
            <atom:MenuItem Header="Option B" />
        </atom:MenuFlyout>
    </atom:DropdownButton.DropdownFlyout>
</atom:DropdownButton>
```

