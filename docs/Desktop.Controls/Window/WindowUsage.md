# Window 使用文档

本文档介绍 AtomUI Window 控件的各种使用方式，示例代码参考自 Gallery 演示程序。

> 📖 Gallery 主窗口源码位置：`controlgallery/AtomUIGallery/Workspace/Views/WorkspaceWindow.axaml`

---

## 前置准备

在 AXAML 中使用 Window，需要引入以下命名空间：

```xml
xmlns:atom="https://atomui.net"
```

或使用 `using` 语法：

```xml
xmlns:atom="using:AtomUI.Desktop.Controls"
```

在 C# 代码中引入：

```csharp
using AtomUI.Desktop.Controls;   // Window, ReactiveWindow<T>
using AtomUI.Controls;            // MediaBreakPoint 等共享类型
```

---

## 1. 基本窗口

最基本的 Window 用法，设置标题、大小和启动位置：

```xml
<atom:Window xmlns="https://github.com/avaloniaui"
             xmlns:atom="https://atomui.net"
             Title="我的应用程序"
             Width="1200" Height="800"
             WindowStartupLocation="CenterScreen">
    <TextBlock Text="Hello, AtomUI!" />
</atom:Window>
```

---

## 2. 设置窗口 Logo

通过 `Logo` 属性在标题栏显示应用程序图标：

```xml
<atom:Window Title="我的应用程序"
             WindowStartupLocation="CenterScreen"
             Width="1200" Height="800">
    <atom:Window.Logo>
        <Image Source="/Assets/app-logo.png" />
    </atom:Window.Logo>
    <!-- 窗口内容 -->
</atom:Window>
```

> 📖 参考：`controlgallery/AtomUIGallery/Workspace/Views/WorkspaceWindow.axaml` 中 Logo 设置。

---

## 3. 配置标题按钮

Window 提供多个标题按钮配置属性，可灵活控制标题栏功能按钮的显示/隐藏：

```xml
<!-- 显示置顶（Pin）按钮和全屏按钮 -->
<atom:Window IsPinCaptionButtonEnabled="True"
             IsFullScreenCaptionButtonEnabled="True"
             Title="全功能窗口">
    <!-- 内容 -->
</atom:Window>

<!-- 禁用关闭按钮（macOS 下红绿灯中的关闭按钮变灰） -->
<atom:Window IsCloseCaptionButtonEnabled="False"
             Title="不可关闭的窗口">
    <!-- 内容 -->
</atom:Window>
```

在代码中动态控制标题按钮（参考 Gallery 示例）：

```csharp
// 动态切换全屏按钮显示
IsFullScreenCaptionButtonEnabled = true;

// 动态切换置顶按钮显示
IsPinCaptionButtonEnabled = true;

// 动态切换关闭按钮可用性（macOS 下控制原生红绿灯按钮）
IsCloseCaptionButtonEnabled = false;
```

> 📖 参考：`controlgallery/AtomUIGallery/Workspace/Views/WorkspaceWindow.axaml.cs` 中 `HandleMenuItemCheckChanged` 方法。

---

## 4. 隐藏标题栏

如果需要完全自定义窗口外观，可以隐藏默认标题栏：

```xml
<atom:Window IsTitleBarVisible="False"
             Title="无标题栏窗口">
    <!-- 需要自行实现关闭/最小化等功能 -->
    <DockPanel>
        <StackPanel DockPanel.Dock="Top" Orientation="Horizontal"
                    HorizontalAlignment="Right">
            <atom:Button ButtonType="Text" Content="—" />
            <atom:Button ButtonType="Text" Content="✕" />
        </StackPanel>
        <!-- 主内容 -->
    </DockPanel>
</atom:Window>
```

---

## 5. 控制窗口大小和拖拽

### 限制最大尺寸为屏幕比例

```xml
<!-- 窗口最大宽度不超过屏幕的 80%，最大高度不超过 90% -->
<atom:Window MaxWidthScreenRatio="0.8"
             MaxHeightScreenRatio="0.9"
             Title="受限窗口">
    <!-- 内容 -->
</atom:Window>
```

### 禁用窗口拖拽

```xml
<!-- 禁用标题栏拖拽移动 -->
<atom:Window IsMoveEnabled="False" Title="不可移动的窗口">
    <!-- 内容 -->
</atom:Window>
```

### 禁用窗口缩放

```xml
<atom:Window CanResize="False" Title="固定大小窗口"
             Width="600" Height="400">
    <!-- 内容 -->
</atom:Window>
```

### 使用扩展方法居中窗口

```csharp
// 将窗口居中到当前所在屏幕
this.CenterOnScreen();

// 居中到指定屏幕
var screen = this.GetHostScreen();
this.CenterOnScreen(screen);
```

---

## 6. 框架层背景

### 窗口级框架背景

覆盖整个窗口区域（标题栏 + 内容区）的背景装饰：

```xml
<atom:Window Title="渐变背景窗口">
    <atom:Window.WindowFrameLayer>
        <Border>
            <Border.Background>
                <LinearGradientBrush StartPoint="0%,0%" EndPoint="100%,100%">
                    <GradientStop Color="#667eea" Offset="0" />
                    <GradientStop Color="#764ba2" Offset="1" />
                </LinearGradientBrush>
            </Border.Background>
        </Border>
    </atom:Window.WindowFrameLayer>
    <!-- 内容 -->
</atom:Window>
```

### 内容区域独立背景

仅为内容区域设置背景，不影响标题栏：

```xml
<atom:Window ContentFrameBackground="#f0f2f5" Title="灰色内容区">
    <!-- 内容区域有独立背景色 -->
</atom:Window>
```

### 标题栏独立背景

仅为标题栏设置背景色：

```xml
<atom:Window TitleBarFrameBackground="DarkBlue" Title="蓝色标题栏">
    <!-- 内容 -->
</atom:Window>
```

### 带不透明度的背景图层

```xml
<atom:Window WindowFrameLayerOpacity="0.15" Title="半透明背景">
    <atom:Window.WindowFrameLayer>
        <Image Source="/Assets/background-pattern.png" Stretch="UniformToFill" />
    </atom:Window.WindowFrameLayer>
    <!-- 内容 -->
</atom:Window>
```

---

## 7. ReactiveWindow（MVVM 模式）

### 基本 MVVM 模式

`ReactiveWindow<TViewModel>` 是推荐的 MVVM 窗口基类：

**AXAML：**

```xml
<atom:Window xmlns="https://github.com/avaloniaui"
             xmlns:atom="using:AtomUI.Desktop.Controls"
             xmlns:vm="using:MyApp.ViewModels"
             x:Class="MyApp.Views.MainWindow"
             x:DataType="vm:MainViewModel"
             Title="MVVM 窗口">
    <TextBlock Text="{Binding Greeting}" />
</atom:Window>
```

**C# Code-behind：**

```csharp
using AtomUI.Desktop.Controls;
using MyApp.ViewModels;

namespace MyApp.Views;

public partial class MainWindow : ReactiveWindow<MainViewModel>
{
    public MainWindow()
    {
        DataContext = new MainViewModel();
        InitializeComponent();
    }
}
```

**ViewModel：**

```csharp
using ReactiveUI;

namespace MyApp.ViewModels;

public class MainViewModel : ReactiveObject, IActivatableViewModel
{
    public ViewModelActivator Activator { get; } = new();

    private string _greeting = "Hello, AtomUI!";
    public string Greeting
    {
        get => _greeting;
        set => this.RaiseAndSetIfChanged(ref _greeting, value);
    }
}
```

### ViewModel 自动激活

如果 ViewModel 实现了 `IActivatableViewModel`，窗口显示时自动激活 ViewModel：

```csharp
public class MainViewModel : ReactiveObject, IActivatableViewModel
{
    public ViewModelActivator Activator { get; } = new();

    public MainViewModel()
    {
        this.WhenActivated(disposables =>
        {
            // 窗口显示时执行
            Debug.WriteLine("ViewModel activated");

            // 订阅自动清理
            Observable.Interval(TimeSpan.FromSeconds(1))
                .Subscribe(_ => { /* 定时任务 */ })
                .DisposeWith(disposables);
        });
    }
}
```

> 📖 参考：`controlgallery/AtomUIGallery/Workspace/Views/WorkspaceWindow.axaml.cs` 使用了 `ReactiveWindow<WorkspaceWindowViewModel>`。

---

## 8. 响应媒体断点变化

Window 内置的媒体查询系统基于内容区域宽度自动计算断点：

### 通过事件响应

```csharp
public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
        MediaBreakPointChanged += OnMediaBreakPointChanged;
    }

    private void OnMediaBreakPointChanged(object? sender, MediaBreakPointChangedEventArgs e)
    {
        switch (e.MediaBreakPoint)
        {
            case MediaBreakPoint.ExtraSmall:
                // ≤ 575px：切换到紧凑布局
                break;
            case MediaBreakPoint.Small:
                // ≥ 576px：显示简化侧边栏
                break;
            case MediaBreakPoint.Medium:
                // ≥ 768px：显示完整侧边栏
                break;
            case MediaBreakPoint.Large:
                // ≥ 992px：标准桌面布局
                break;
            case MediaBreakPoint.ExtraLarge:
                // ≥ 1200px：宽屏布局
                break;
            case MediaBreakPoint.ExtraExtraLarge:
                // ≥ 1600px：超宽屏布局
                break;
        }
    }
}
```

### 通过属性绑定

```xml
<!-- 基于断点控制侧边栏可见性 -->
<Panel>
    <DockPanel>
        <Border DockPanel.Dock="Left" Width="250"
                IsVisible="{Binding MediaBreakPoint, 
                    RelativeSource={RelativeSource AncestorType=atom:Window},
                    Converter={StaticResource BreakPointToVisibilityConverter}}">
            <!-- 侧边栏 -->
        </Border>
        <!-- 主内容 -->
    </DockPanel>
</Panel>
```

---

## 9. macOS 特有配置

在 macOS 上精确控制原生红绿灯按钮位置：

```xml
<atom:Window MacOSCaptionGroupOffset="10, 2"
             MacOSCaptionGroupSpacing="5"
             Title="macOS 窗口">
    <!-- 内容 -->
</atom:Window>
```

> 注意：macOS 红绿灯按钮的位置和间距由 `MacOSCaptionGroupOffset` 和 `MacOSCaptionGroupSpacing` 控制，这些属性仅在 macOS 平台生效。在主题中默认值为 `(10, 2)` 和 `5`。

---

## 10. 获取主窗口引用

在应用程序的任何位置获取主窗口引用：

```csharp
// 静态方法获取主窗口
var mainWindow = Window.GetMainWindow();
if (mainWindow != null)
{
    mainWindow.Title = "Updated Title";
}
```

---

## 11. 窗口图标自动继承

子窗口（对话框等）如果没有设置 `Icon`，会自动从主窗口继承图标：

```csharp
// 子窗口自动使用主窗口的图标
var dialog = new Window
{
    Title = "Dialog",
    Width = 400,
    Height = 300,
    // Icon 不设置，会自动从主窗口继承
};
dialog.ShowDialog(this);
```

---

## 常见组合模式

### 标准桌面应用窗口

```xml
<atom:Window xmlns="https://github.com/avaloniaui"
             xmlns:atom="https://atomui.net"
             Title="我的应用"
             Icon="/Assets/app.ico"
             IsPinCaptionButtonEnabled="True"
             WindowStartupLocation="CenterScreen"
             Width="1300" Height="900">
    <atom:Window.Logo>
        <Image Source="/Assets/logo.png" />
    </atom:Window.Logo>
    <DockPanel LastChildFill="True">
        <atom:Menu DockPanel.Dock="Top">
            <atom:MenuItem Header="文件" />
            <atom:MenuItem Header="编辑" />
            <atom:MenuItem Header="帮助" />
        </atom:Menu>
        <!-- 主内容 -->
        <TextBlock Text="主内容区域"
                   HorizontalAlignment="Center"
                   VerticalAlignment="Center" />
    </DockPanel>
</atom:Window>
```

> 📖 这正是 Gallery 主窗口 `controlgallery/AtomUIGallery/Workspace/Views/WorkspaceWindow.axaml` 的结构模式。

### 固定尺寸对话框

```xml
<atom:Window Title="设置"
             CanResize="False"
             CanMinimize="False"
             CanMaximize="False"
             IsPinCaptionButtonEnabled="False"
             IsFullScreenCaptionButtonEnabled="False"
             Width="500" Height="400"
             WindowStartupLocation="CenterOwner">
    <!-- 对话框内容 -->
    <StackPanel Margin="20">
        <TextBlock Text="设置项..." />
        <StackPanel Orientation="Horizontal"
                    HorizontalAlignment="Right" Margin="0,20,0,0">
            <atom:Button Margin="0,0,8,0">取消</atom:Button>
            <atom:Button ButtonType="Primary">确定</atom:Button>
        </StackPanel>
    </StackPanel>
</atom:Window>
```

### 带主题切换的窗口

```csharp
// 切换暗色主题
Application.Current?.SetDarkThemeMode(true);

// 切换紧凑模式
Application.Current?.SetCompactThemeMode(true);

// 启用/禁用动画
Application.Current?.SetMotionEnabled(false);
```

> 📖 参考：`controlgallery/AtomUIGallery/Workspace/Views/WorkspaceWindow.axaml.cs` 中完整的主题切换实现。
