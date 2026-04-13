# IconButton 使用文档

IconButton 是 AtomUI 中最轻量的按钮控件，仅展示一个图标。它既可以独立使用（工具栏、操作栏），也被广泛用作其他复合控件的内部构件。

> 📖 IconButton 没有专属的 Gallery ShowCase，但它被大量使用在其他控件的主题模板中：
> - Tag 的关闭按钮（`src/AtomUI.Desktop.Controls/Tag/Themes/TagTheme.axaml`）
> - TabItem 的关闭按钮（`src/AtomUI.Desktop.Controls/TabControl/Themes/BaseTabItemTheme.axaml`）
> - NotificationCard 的关闭按钮（`src/AtomUI.Desktop.Controls/Notifications/Themes/NotificationCardTheme.axaml`）
> - Upload 的删除/预览按钮（`src/AtomUI.Desktop.Controls/Upload/Themes/`）

---

## 前置准备

在 AXAML 中使用 IconButton，需要引入以下命名空间：

```xml
xmlns:atom="https://atomui.net"
xmlns:antdicons="https://atomui.net/icons/antdesign"
```

在 C# 代码中引入：

```csharp
using AtomUI.Desktop.Controls;   // IconButton 控件
```

---

## 1. 基本使用

IconButton 最常见的用法是展示一个 Ant Design 图标：

```xml
<atom:IconButton Icon="{antdicons:AntDesignIconProvider Kind=SearchOutlined}" />
<atom:IconButton Icon="{antdicons:AntDesignIconProvider Kind=EditOutlined}" />
<atom:IconButton Icon="{antdicons:AntDesignIconProvider Kind=DeleteOutlined}" />
<atom:IconButton Icon="{antdicons:AntDesignIconProvider Kind=SettingOutlined}" />
```

---

## 2. 自定义图标尺寸

通过 `IconWidth` 和 `IconHeight` 控制图标大小（默认为 `SharedToken.IconSizeSM`）：

```xml
<!-- 小号 -->
<atom:IconButton Icon="{antdicons:AntDesignIconProvider Kind=SearchOutlined}"
                 IconWidth="12" IconHeight="12" />
<!-- 默认 -->
<atom:IconButton Icon="{antdicons:AntDesignIconProvider Kind=SearchOutlined}" />
<!-- 中号 -->
<atom:IconButton Icon="{antdicons:AntDesignIconProvider Kind=SearchOutlined}"
                 IconWidth="16" IconHeight="16" />
<!-- 大号 -->
<atom:IconButton Icon="{antdicons:AntDesignIconProvider Kind=SearchOutlined}"
                 IconWidth="24" IconHeight="24" />
```

---

## 3. 自定义图标颜色

通过 `IconBrush` 覆盖默认的主题颜色：

```xml
<atom:IconButton Icon="{antdicons:AntDesignIconProvider Kind=HeartFilled}"
                 IconBrush="Red" />
<atom:IconButton Icon="{antdicons:AntDesignIconProvider Kind=StarFilled}"
                 IconBrush="Gold" />
<atom:IconButton Icon="{antdicons:AntDesignIconProvider Kind=CheckCircleFilled}"
                 IconBrush="{atom:SharedTokenResource ColorSuccess}" />
```

> ⚠️ 注意：直接设置 `IconBrush` 会覆盖主题的伪类驱动颜色（悬浮/按下颜色切换将失效）。如果需要保留交互色变化，请通过 Style 覆盖各状态的 `IconBrush`。

---

## 4. 禁用状态

```xml
<atom:IconButton Icon="{antdicons:AntDesignIconProvider Kind=DeleteOutlined}"
                 IsEnabled="False" />
```

禁用时图标颜色自动切换为 `ColorTextDisabled`。

---

## 5. MVVM 命令绑定

IconButton 继承自 Avalonia Button，完整支持 `Command` / `CommandParameter` 绑定：

```xml
<atom:IconButton Icon="{antdicons:AntDesignIconProvider Kind=DeleteOutlined}"
                 Command="{Binding DeleteCommand}"
                 CommandParameter="{Binding SelectedItem}" />
```

当 `CanExecute` 返回 `false` 时，IconButton 自动禁用（灰色图标 + 不可点击）。

---

## 6. Click 事件

```xml
<atom:IconButton Icon="{antdicons:AntDesignIconProvider Kind=CopyOutlined}"
                 Click="HandleCopyClick" />
```

```csharp
public void HandleCopyClick(object? sender, RoutedEventArgs args)
{
    // 复制操作
    var clipboard = TopLevel.GetTopLevel(this)?.Clipboard;
    if (clipboard is not null)
    {
        _ = clipboard.SetTextAsync("Copied text");
    }
}
```

---

## 7. 带 Flyout 弹出

```xml
<atom:IconButton Icon="{antdicons:AntDesignIconProvider Kind=MoreOutlined}">
    <atom:IconButton.Flyout>
        <MenuFlyout>
            <MenuItem Header="Edit" />
            <MenuItem Header="Delete" />
        </MenuFlyout>
    </atom:IconButton.Flyout>
</atom:IconButton>
```

---

## 8. 加载动画

通过 `LoadingAnimation` 属性为图标添加旋转等加载动画：

```xml
<atom:IconButton Icon="{antdicons:AntDesignIconProvider Kind=LoadingOutlined}"
                 LoadingAnimation="Spin" />
<atom:IconButton Icon="{antdicons:AntDesignIconProvider Kind=SyncOutlined}"
                 LoadingAnimation="Spin" />
```

---

## 9. C# 代码操作

### 动态创建

```csharp
using AtomUI.Desktop.Controls;
using AtomUI.Icons.AntDesign;
using Avalonia.Media;

var iconButton = new IconButton
{
    Icon = AntDesignIconPackage.Current.GetPathIcon(AntDesignIconKind.SearchOutlined),
    IconWidth = 16,
    IconHeight = 16,
    IconBrush = new SolidColorBrush(Colors.DodgerBlue)
};

iconButton.Click += (sender, e) =>
{
    // 处理点击
};

parentPanel.Children.Add(iconButton);
```

### 动态修改属性

```csharp
// 切换图标
iconButton.Icon = AntDesignIconPackage.Current.GetPathIcon(AntDesignIconKind.CheckOutlined);

// 修改颜色
iconButton.IconBrush = new SolidColorBrush(Colors.Green);

// 开启加载动画
iconButton.LoadingAnimation = IconAnimation.Spin;
```

---

## 常见组合模式

### 工具栏

```xml
<StackPanel Orientation="Horizontal" Spacing="4">
    <atom:IconButton Icon="{antdicons:AntDesignIconProvider Kind=BoldOutlined}" />
    <atom:IconButton Icon="{antdicons:AntDesignIconProvider Kind=ItalicOutlined}" />
    <atom:IconButton Icon="{antdicons:AntDesignIconProvider Kind=UnderlineOutlined}" />
    <atom:IconButton Icon="{antdicons:AntDesignIconProvider Kind=StrikethroughOutlined}" />
</StackPanel>
```

### 表格行操作

```xml
<StackPanel Orientation="Horizontal" Spacing="8">
    <atom:IconButton Icon="{antdicons:AntDesignIconProvider Kind=EditOutlined}"
                     Command="{Binding EditCommand}" />
    <atom:IconButton Icon="{antdicons:AntDesignIconProvider Kind=DeleteOutlined}"
                     IconBrush="{atom:SharedTokenResource ColorError}"
                     Command="{Binding DeleteCommand}" />
</StackPanel>
```

### 自定义悬浮背景

当需要 IconButton 有悬浮背景反馈时（如独立使用场景），可通过 Style 增加背景：

```xml
<StackPanel Orientation="Horizontal" Spacing="4">
    <StackPanel.Styles>
        <Style Selector="atom|IconButton">
            <Setter Property="Padding" Value="6" />
            <Setter Property="CornerRadius" Value="4" />
        </Style>
        <Style Selector="atom|IconButton:pointerover">
            <Setter Property="Background" Value="{atom:SharedTokenResource ColorBgTextHover}" />
        </Style>
    </StackPanel.Styles>
    <atom:IconButton Icon="{antdicons:AntDesignIconProvider Kind=AlignLeftOutlined}" />
    <atom:IconButton Icon="{antdicons:AntDesignIconProvider Kind=AlignCenterOutlined}" />
    <atom:IconButton Icon="{antdicons:AntDesignIconProvider Kind=AlignRightOutlined}" />
</StackPanel>
```

### 控件内部构件使用

IconButton 在 AtomUI 其他控件的主题模板中被广泛使用。以 Tag 的关闭按钮为例：

```xml
<!-- 来自 TagTheme.axaml -->
<atom:IconButton Name="PART_CloseButton"
                 Icon="{antdicons:AntDesignIconProvider CloseOutlined}"
                 IsPassthroughMouseEvent="True" />
```

关键点：`IsPassthroughMouseEvent="True"` 确保关闭按钮不会阻断父控件（Tag）的鼠标事件冒泡。
