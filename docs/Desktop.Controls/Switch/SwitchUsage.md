# Switch 使用文档

本文档介绍 AtomUI ToggleSwitch 控件的各种使用方式。

> 📖 ToggleSwitch 在 Gallery 中没有独立的 ShowCase 页面，但在多个演示中作为辅助控件使用，例如：
> - `controlgallery/AtomUIGallery/ShowCases/Views/DataDisplay/TreeViewShowCase.axaml`
> - `controlgallery/AtomUIGallery/ShowCases/Views/DataDisplay/BadgeShowCase.axaml`
> - `controlgallery/AtomUIGallery/ShowCases/Views/Navigation/MenuShowCase.axaml`
> - `controlgallery/AtomUIGallery/ShowCases/Views/Feedback/DrawerShowCase.axaml`

---

## 前置准备

在 AXAML 中使用 ToggleSwitch，需要引入以下命名空间：

```xml
xmlns:atom="https://atomui.net"
xmlns:antdicons="https://atomui.net/icons/antdesign"
```

在 C# 代码中引入：

```csharp
using AtomUI.Desktop.Controls;   // ToggleSwitch 控件
using AtomUI.Controls;            // SizeType 等共享类型
```

---

## 1. 基本用法

最简单的开关，点击切换开/关状态：

```xml
<atom:ToggleSwitch />
<atom:ToggleSwitch IsChecked="True" />
```

---

## 2. 带文字

在开关轨道内显示开/关文字：

```xml
<atom:ToggleSwitch OnContent="开" OffContent="关" />
<atom:ToggleSwitch OnContent="ON" OffContent="OFF" />
<atom:ToggleSwitch OnContent="1" OffContent="0" IsChecked="True" />
```

---

## 3. 带图标

使用 Ant Design 图标作为开/关内容：

```xml
<atom:ToggleSwitch
    OnContent="{antdicons:AntDesignIconProvider Kind=CheckOutlined}"
    OffContent="{antdicons:AntDesignIconProvider Kind=CloseOutlined}" />
```

---

## 4. 两种尺寸

通过 `SizeType` 属性设置开关尺寸：

```xml
<!-- 默认尺寸 -->
<atom:ToggleSwitch />
<atom:ToggleSwitch OnContent="开" OffContent="关" />

<!-- 小号尺寸 -->
<atom:ToggleSwitch SizeType="Small" />
<atom:ToggleSwitch SizeType="Small" OnContent="开" OffContent="关" />
```

---

## 5. 禁用状态

通过 `IsEnabled="False"` 禁用开关：

```xml
<!-- 禁用的关闭态 -->
<atom:ToggleSwitch IsEnabled="False" />

<!-- 禁用的开启态 -->
<atom:ToggleSwitch IsChecked="True" IsEnabled="False" />
```

---

## 6. 加载状态

通过 `IsLoading="True"` 显示加载指示器，加载中禁止切换：

```xml
<atom:ToggleSwitch IsLoading="True" />
<atom:ToggleSwitch IsLoading="True" IsChecked="True" />

<!-- 小号加载 -->
<atom:ToggleSwitch IsLoading="True" SizeType="Small" />
```

### 异步切换场景

```xml
<atom:ToggleSwitch IsChecked="{Binding IsFeatureEnabled}"
                   Click="HandleSwitchClick" />
```

```csharp
private async void HandleSwitchClick(object? sender, RoutedEventArgs e)
{
    if (sender is ToggleSwitch toggleSwitch)
    {
        toggleSwitch.IsLoading = true;
        try
        {
            await _settingsService.ToggleFeatureAsync();
        }
        finally
        {
            toggleSwitch.IsLoading = false;
        }
    }
}
```

---

## 7. 数据绑定

### 绑定 IsChecked

```xml
<atom:ToggleSwitch IsChecked="{Binding IsDarkMode, Mode=TwoWay}" />
```

```csharp
// ViewModel
private bool _isDarkMode;
public bool IsDarkMode
{
    get => _isDarkMode;
    set => this.RaiseAndSetIfChanged(ref _isDarkMode, value);
}
```

### MVVM 命令绑定

```xml
<atom:ToggleSwitch Command="{Binding ToggleNotificationCommand}" />
```

```csharp
public ReactiveCommand<Unit, Unit> ToggleNotificationCommand { get; }

public MyViewModel()
{
    ToggleNotificationCommand = ReactiveCommand.CreateFromTask(async () =>
    {
        await _notificationService.ToggleAsync();
    });
}
```

---

## 8. 控制动画行为

```xml
<!-- 禁用滑块过渡动画 -->
<atom:ToggleSwitch IsMotionEnabled="False" />

<!-- 禁用切换波纹效果 -->
<atom:ToggleSwitch IsWaveSpiritEnabled="False" />
```

---

## 常见组合模式

### 设置项列表

```xml
<StackPanel Spacing="16">
    <DockPanel>
        <atom:ToggleSwitch DockPanel.Dock="Right"
                           IsChecked="{Binding IsNotificationEnabled, Mode=TwoWay}" />
        <TextBlock Text="Enable Notifications" VerticalAlignment="Center" />
    </DockPanel>
    <DockPanel>
        <atom:ToggleSwitch DockPanel.Dock="Right"
                           IsChecked="{Binding IsDarkMode, Mode=TwoWay}" />
        <TextBlock Text="Dark Mode" VerticalAlignment="Center" />
    </DockPanel>
    <DockPanel>
        <atom:ToggleSwitch DockPanel.Dock="Right"
                           IsChecked="{Binding IsAutoSave, Mode=TwoWay}" />
        <TextBlock Text="Auto Save" VerticalAlignment="Center" />
    </DockPanel>
</StackPanel>
```

### 表单中使用

```xml
<atom:FormItem Label="Enable Feature">
    <atom:ToggleSwitch IsChecked="{Binding IsFeatureEnabled, Mode=TwoWay}" />
</atom:FormItem>
```

### 带状态描述

```xml
<StackPanel Orientation="Horizontal" Spacing="8">
    <atom:ToggleSwitch IsChecked="{Binding IsOnline, Mode=TwoWay}" />
    <TextBlock Text="{Binding IsOnline, Converter={StaticResource BoolToOnlineConverter}}"
               VerticalAlignment="Center" />
</StackPanel>
```
