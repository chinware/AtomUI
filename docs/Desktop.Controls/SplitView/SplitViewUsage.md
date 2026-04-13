# SplitView 使用文档

本文档介绍 AtomUI SplitView 控件的各种使用方式。

> 📖 注意：当前 Gallery 演示程序中尚未包含 SplitView 的独立 ShowCase。以下示例基于控件源码和主题模板编写。

---

## 前置准备

在 AXAML 中使用 SplitView，需要引入以下命名空间：

```xml
xmlns:atom="https://atomui.net"
```

在 C# 代码中引入：

```csharp
using AtomUI.Desktop.Controls;   // SplitView 控件
using Avalonia.Controls;          // SplitViewDisplayMode, SplitViewPanePlacement 等枚举
```

---

## 1. 基础用法

最简单的 SplitView 用法——左侧面板 + 主内容区域：

```xml
<atom:SplitView DisplayMode="Inline"
                IsPaneOpen="True"
                OpenPaneLength="250">
    <atom:SplitView.Pane>
        <StackPanel Padding="10" Spacing="8">
            <TextBlock Text="导航菜单" FontWeight="Bold" />
            <TextBlock Text="首页" />
            <TextBlock Text="设置" />
            <TextBlock Text="关于" />
        </StackPanel>
    </atom:SplitView.Pane>
    <TextBlock Text="这是主内容区域" Padding="20" />
</atom:SplitView>
```

---

## 2. 显示模式

### Overlay 模式

面板覆盖在主内容之上，不影响主内容布局。适用于窄屏或临时弹出的导航面板：

```xml
<atom:SplitView DisplayMode="Overlay"
                IsPaneOpen="{Binding IsPaneOpen}"
                OpenPaneLength="300">
    <atom:SplitView.Pane>
        <Border Padding="16">
            <TextBlock Text="覆盖面板内容" />
        </Border>
    </atom:SplitView.Pane>
    <StackPanel Padding="20">
        <atom:Button Click="TogglePane">切换面板</atom:Button>
        <TextBlock Text="主内容不会被推开" />
    </StackPanel>
</atom:SplitView>
```

### Inline 模式

面板与主内容并排，展开时主内容被压缩：

```xml
<atom:SplitView DisplayMode="Inline"
                IsPaneOpen="True"
                OpenPaneLength="200">
    <atom:SplitView.Pane>
        <TextBlock Text="并排面板" Padding="10" />
    </atom:SplitView.Pane>
    <TextBlock Text="主内容会被压缩" Padding="10" />
</atom:SplitView>
```

### CompactInline 模式

收起时保留紧凑条（通常只显示图标），展开时压缩主内容显示完整导航：

```xml
<atom:SplitView DisplayMode="CompactInline"
                IsPaneOpen="{Binding IsPaneOpen}"
                CompactPaneLength="48"
                OpenPaneLength="250">
    <atom:SplitView.Pane>
        <StackPanel Padding="4" Spacing="4">
            <!-- 紧凑态只显示图标，展开态显示图标+文字 -->
            <StackPanel Orientation="Horizontal" Spacing="8">
                <antdicons:HomeOutlined Width="24" Height="24" />
                <TextBlock Text="首页" VerticalAlignment="Center" />
            </StackPanel>
            <StackPanel Orientation="Horizontal" Spacing="8">
                <antdicons:SettingOutlined Width="24" Height="24" />
                <TextBlock Text="设置" VerticalAlignment="Center" />
            </StackPanel>
        </StackPanel>
    </atom:SplitView.Pane>
    <TextBlock Text="主内容区域" Padding="20" />
</atom:SplitView>
```

### CompactOverlay 模式

收起时保留紧凑条，展开时覆盖主内容：

```xml
<atom:SplitView DisplayMode="CompactOverlay"
                IsPaneOpen="{Binding IsPaneOpen}"
                CompactPaneLength="48"
                OpenPaneLength="280">
    <atom:SplitView.Pane>
        <Border Padding="10">
            <TextBlock Text="紧凑覆盖面板" />
        </Border>
    </atom:SplitView.Pane>
    <TextBlock Text="主内容不被压缩" Padding="20" />
</atom:SplitView>
```

---

## 3. 面板位置

### 右侧面板

```xml
<atom:SplitView PanePlacement="Right"
                DisplayMode="Inline"
                IsPaneOpen="True"
                OpenPaneLength="300">
    <atom:SplitView.Pane>
        <Border Padding="16">
            <TextBlock Text="属性面板 / 详情面板" />
        </Border>
    </atom:SplitView.Pane>
    <TextBlock Text="主内容区域" Padding="20" />
</atom:SplitView>
```

### 顶部面板

```xml
<atom:SplitView PanePlacement="Top"
                DisplayMode="Inline"
                IsPaneOpen="True"
                OpenPaneLength="120">
    <atom:SplitView.Pane>
        <Border Padding="16">
            <TextBlock Text="顶部工具栏区域" />
        </Border>
    </atom:SplitView.Pane>
    <TextBlock Text="主内容区域" Padding="20" />
</atom:SplitView>
```

### 底部面板

```xml
<atom:SplitView PanePlacement="Bottom"
                DisplayMode="Overlay"
                IsPaneOpen="{Binding IsBottomPanelOpen}"
                OpenPaneLength="200">
    <atom:SplitView.Pane>
        <Border Padding="16">
            <TextBlock Text="底部弹出面板" />
        </Border>
    </atom:SplitView.Pane>
    <TextBlock Text="主内容区域" Padding="20" />
</atom:SplitView>
```

---

## 4. 展开/收起切换

### 通过按钮切换

```xml
<DockPanel>
    <atom:Button DockPanel.Dock="Top"
                 Click="TogglePane"
                 ButtonType="Primary"
                 Margin="10">
        切换侧边栏
    </atom:Button>
    <atom:SplitView x:Name="MySplitView"
                    DisplayMode="CompactInline"
                    CompactPaneLength="48"
                    OpenPaneLength="250">
        <atom:SplitView.Pane>
            <TextBlock Text="侧边栏" Padding="10" />
        </atom:SplitView.Pane>
        <TextBlock Text="主内容" Padding="10" />
    </atom:SplitView>
</DockPanel>
```

```csharp
private void TogglePane(object? sender, RoutedEventArgs e)
{
    MySplitView.IsPaneOpen = !MySplitView.IsPaneOpen;
}
```

### 通过 MVVM 数据绑定

```xml
<atom:SplitView IsPaneOpen="{Binding IsSidebarOpen}"
                DisplayMode="CompactInline">
    <!-- ... -->
</atom:SplitView>

<atom:ToggleButton IsChecked="{Binding IsSidebarOpen}">
    Toggle Sidebar
</atom:ToggleButton>
```

```csharp
public class MainViewModel : ReactiveObject
{
    private bool _isSidebarOpen = true;
    public bool IsSidebarOpen
    {
        get => _isSidebarOpen;
        set => this.RaiseAndSetIfChanged(ref _isSidebarOpen, value);
    }
}
```

---

## 5. 轻量关闭遮罩

在 Overlay 模式下启用轻量关闭遮罩，点击遮罩自动关闭面板：

```xml
<atom:SplitView DisplayMode="Overlay"
                UseLightDismissOverlayMode="True"
                IsPaneOpen="{Binding IsPaneOpen}"
                OpenPaneLength="300">
    <atom:SplitView.Pane>
        <Border Padding="16">
            <TextBlock Text="点击遮罩区域可关闭此面板" />
        </Border>
    </atom:SplitView.Pane>
    <TextBlock Text="点击此处的半透明遮罩可关闭面板" Padding="20" />
</atom:SplitView>
```

---

## 6. 自定义动画参数

### 自定义展开/收起时长

```xml
<!-- 慢速展开，快速收起 -->
<atom:SplitView PaneOpenMotionDuration="0:0:0.5"
                PaneCloseMotionDuration="0:0:0.1"
                DisplayMode="Inline"
                IsPaneOpen="True">
    <!-- ... -->
</atom:SplitView>
```

### 禁用动画

```xml
<!-- 面板展开/收起瞬间完成 -->
<atom:SplitView IsMotionEnabled="False"
                DisplayMode="CompactInline"
                IsPaneOpen="True">
    <!-- ... -->
</atom:SplitView>
```

---

## 7. 响应面板关闭事件

```xml
<atom:SplitView PaneClosing="OnPaneClosing"
                PaneClosed="OnPaneClosed"
                DisplayMode="Overlay">
    <!-- ... -->
</atom:SplitView>
```

```csharp
private void OnPaneClosing(object? sender, CancelRoutedEventArgs e)
{
    // 可以条件性地阻止面板关闭
    if (HasUnsavedChanges)
    {
        e.Cancel = true; // 取消关闭
    }
}

private void OnPaneClosed(object? sender, RoutedEventArgs e)
{
    // 面板已关闭后的清理逻辑
}
```

---

## 常见组合模式

### 应用主框架布局

```xml
<DockPanel>
    <!-- 顶部标题栏 -->
    <Border DockPanel.Dock="Top" Padding="10" Background="{atom:SharedTokenResource ColorBgContainer}">
        <DockPanel>
            <atom:Button DockPanel.Dock="Left" ButtonType="Text"
                         Click="ToggleNav"
                         Icon="{antdicons:AntDesignIconProvider Kind=MenuOutlined}" />
            <TextBlock Text="我的应用" VerticalAlignment="Center" FontWeight="Bold" />
        </DockPanel>
    </Border>

    <!-- 导航 + 内容 -->
    <atom:SplitView x:Name="NavSplitView"
                    DisplayMode="CompactInline"
                    CompactPaneLength="48"
                    OpenPaneLength="220"
                    IsPaneOpen="True">
        <atom:SplitView.Pane>
            <StackPanel Spacing="4" Padding="4">
                <!-- 导航项 -->
            </StackPanel>
        </atom:SplitView.Pane>

        <!-- 主内容页面 -->
        <ContentControl Content="{Binding CurrentPage}" Padding="16" />
    </atom:SplitView>
</DockPanel>
```

### 右侧属性面板

```xml
<atom:SplitView PanePlacement="Right"
                DisplayMode="Overlay"
                IsPaneOpen="{Binding IsPropertyPanelOpen}"
                OpenPaneLength="350"
                UseLightDismissOverlayMode="True">
    <atom:SplitView.Pane>
        <ScrollViewer Padding="16">
            <!-- 属性编辑器 -->
        </ScrollViewer>
    </atom:SplitView.Pane>

    <!-- 画布/编辑区域 -->
    <ContentControl Content="{Binding EditorContent}" />
</atom:SplitView>
```

### 底部信息面板（类似 IDE 输出面板）

```xml
<atom:SplitView PanePlacement="Bottom"
                DisplayMode="Inline"
                IsPaneOpen="{Binding IsOutputPanelOpen}"
                OpenPaneLength="200">
    <atom:SplitView.Pane>
        <DockPanel>
            <Border DockPanel.Dock="Top" Padding="4,2">
                <TextBlock Text="输出" FontWeight="Bold" />
            </Border>
            <ScrollViewer>
                <TextBlock Text="{Binding OutputText}" FontFamily="Consolas" Padding="8" />
            </ScrollViewer>
        </DockPanel>
    </atom:SplitView.Pane>

    <!-- 主编辑区域 -->
    <ContentControl Content="{Binding EditorContent}" />
</atom:SplitView>
```
