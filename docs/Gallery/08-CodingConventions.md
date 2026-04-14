# 编码规范摘要

## 1. 命名约定

### 1.1 通用命名

| 类型 | 约定 | 示例 |
|------|------|------|
| 类名 | PascalCase | `ButtonShowCase`, `CaseNavigationViewModel` |
| 接口名 | PascalCase + I 前缀 | `IScreen`, `IRoutableViewModel` |
| 枚举 | PascalCase | `DesktopIconKind`, `WindowMenuItemKind` |
| 枚举值 | PascalCase | `DarkMode`, `LanguageZhCN` |
| 命名空间 | PascalCase（与目录结构对应） | `AtomUIGallery.ShowCases.ViewModels` |
| 常量 | PascalCase（`const string` 字段） | `public const string General = "通用"` |
| 静态只读字段 | PascalCase | `public static EntityKey ID = "Button"` |

### 1.2 私有字段

| 类型 | 约定 | 示例 |
|------|------|------|
| 私有字段 | `_` 前缀 + camelCase | `_viewModel`, `_buttonSizeType` |
| 响应式属性后备字段 | `_` 前缀 + camelCase | `_buttonSizeType` → `ButtonSizeType` |

### 1.3 方法命名

| 类型 | 约定 | 示例 |
|------|------|------|
| 事件处理器 | `Handle` 前缀 | `HandleNavMenuItemClick`, `HandleMenuItemCheckChanged` |
| 生命周期方法 | Avalonia 标准覆写 | `OnAttachedToLogicalTree`, `OnAttachedToVisualTree` |
| 导航方法 | 动词开头 | `NavigateTo`, `TestNavigatePages`, `StopTestNavigatePages` |
| 通知方法 | `Notify` 前缀 | `NotifyAboutToActive`, `NotifyActivated` |

### 1.4 属性命名

| 类型 | 约定 | 示例 |
|------|------|------|
| 公共属性 | PascalCase | `HostScreen`, `UrlPathSegment`, `Router` |
| Avalonia StyledProperty | PascalCase | `TitleProperty`, `IsOccupyEntireRowProperty` |
| CLR 属性包装器 | PascalCase | `Title`, `IsOccupyEntireRow` |

### 1.5 XAML 命名

| 类型 | 约定 | 示例 |
|------|------|------|
| x:Name | PascalCase | `ShowCaseNavMenu`, `DarkModeMenuItem` |
| XML 命名空间前缀 | 小写缩写 | `rxui:`, `vm:`, `icons:` |

## 2. 代码组织方式

### 2.1 文件结构

- **一个类一个文件**：每个 ViewModel / View 对应独立文件
- **目录即命名空间**：目录结构与命名空间严格对应
- **分类组织**：ShowCase 按功能分类（DataDisplay, DataEntry, Feedback, General, Layout, Navigation）

### 2.2 ViewModel 文件结构

```csharp
// 1. using 语句（按字母序排列）
using AtomUI;
using ReactiveUI;

// 2. 命名空间（与目录结构对应）
namespace AtomUIGallery.ShowCases.ViewModels;

// 3. 类定义
public class XxxViewModel : ReactiveObject, IRoutableViewModel, IActivatableViewModel
{
    // 3.1 静态字段
    public static EntityKey ID = "Xxx";
    
    // 3.2 接口实现属性
    public IScreen HostScreen { get; }
    public ViewModelActivator Activator { get; }
    public string UrlPathSegment { get; } = ID.ToString();
    
    // 3.3 响应式属性（后备字段 + 公共属性）
    private SizeType _sizeType;
    public SizeType SizeType
    {
        get => _sizeType;
        set => this.RaiseAndSetIfChanged(ref _sizeType, value);
    }
    
    // 3.4 构造函数
    public XxxViewModel(IScreen screen)
    {
        Activator  = new ViewModelActivator();
        HostScreen = screen;
    }
}
```

### 2.3 View 文件结构

```csharp
// 1. using 语句
using AtomUIGallery.ShowCases.ViewModels;
using ReactiveUI;

// 2. 命名空间
namespace AtomUIGallery.ShowCases.Views;

// 3. 类定义
public partial class XxxShowCase : ReactiveUserControl<XxxViewModel>
{
    // 3.1 私有字段
    private XxxViewModel? _viewModel;
    
    // 3.2 构造函数
    public XxxShowCase()
    {
        // 3.3 WhenActivated 回调
        this.WhenActivated(disposables =>
        {
            _viewModel = DataContext as XxxViewModel;
        });
        
        // 3.4 InitializeComponent（在 WhenActivated 之后）
        InitializeComponent();
    }
    
    // 3.5 事件处理器
    public void HandleXxxClick(object? sender, RoutedEventArgs args)
    {
        // ...
    }
}
```

### 2.4 XAML 文件结构

```xml
<!-- 1. 根元素（含命名空间声明） -->
<rxui:ReactiveUserControl
    xmlns="https://github.com/avaloniaui"
    xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
    xmlns:rxui="clr-namespace:ReactiveUI.Avalonia;assembly=ReactiveUI.Avalonia"
    xmlns:vm="clr-namespace:AtomUIGallery.ShowCases.ViewModels"
    xmlns:controls="clr-namespace:AtomUIGallery.Controls"
    x:TypeArguments="vm:XxxViewModel"
    x:Class="AtomUIGallery.ShowCases.Views.XxxShowCase">

    <!-- 2. 控件内容 -->
    <controls:ShowCasePanel>
        <controls:ShowCaseItem Title="基本用法">
            <!-- 内容 -->
        </controls:ShowCaseItem>
    </controls:ShowCasePanel>
</rxui:ReactiveUserControl>
```

## 3. 注释与文档规范

### 3.1 当前状态

- **XML 文档注释**：较少使用，大部分公共 API 缺少 `/// <summary>` 注释
- **行内注释**：少量使用，主要用于解释特殊逻辑
- **TODO 注释**：存在少量 `// TODO` 标记

### 3.2 建议规范

```csharp
/// <summary>
/// Button 控件的演示 ViewModel
/// </summary>
public class ButtonViewModel : ReactiveObject, IRoutableViewModel, IActivatableViewModel
{
    // ...
}
```

## 4. 设计模式使用情况

### 4.1 MVVM 模式

严格遵循 ReactiveUI MVVM 模式：

| 层 | 职责 | 基类 |
|----|------|------|
| **Model** | 数据和业务逻辑 | 无特定基类 |
| **ViewModel** | 状态管理、交互逻辑 | `ReactiveObject` |
| **View** | UI 展示、用户交互 | `ReactiveUserControl<T>` / `ReactiveWindow<T>` |

### 4.2 Service Locator 模式

使用 ReactiveUI 的 `Locator` 进行 ViewModel 注册和解析：

```csharp
// 注册
Locator.CurrentMutable.Register(() => new XxxViewModel(screen), 
    typeof(IRoutableViewModel), XxxViewModel.ID.ToString());

// 解析
var vm = Locator.Current.GetService<IRoutableViewModel>(id);
```

> **注意**：Service Locator 被一些人视为反模式，但在 ReactiveUI 生态中是标准做法。

### 4.3 Observer 模式

通过 `RaiseAndSetIfChanged` 实现属性变更观察：

```csharp
private string _searchText = string.Empty;
public string SearchText
{
    get => _searchText;
    set => this.RaiseAndSetIfChanged(ref _searchText, value);
}
```

### 4.4 Provider 模式

AtomUI 大量使用 Provider 模式：

- `LanguageProvider` — 国际化文本提供
- `IconProvider` — 图标数据提供
- `ControlThemesProvider` — 控件主题提供

### 4.5 Factory 模式

ShowCase ViewModel 通过工厂 Lambda 延迟创建：

```csharp
Locator.CurrentMutable.Register(() => new XxxViewModel(screen), ...);
```

## 5. ReactiveUI 使用规范

### 5.1 属性变更通知

使用 `RaiseAndSetIfChanged` 而非 `INotifyPropertyChanged`：

```csharp
// ✅ 正确
private SizeType _sizeType;
public SizeType SizeType
{
    get => _sizeType;
    set => this.RaiseAndSetIfChanged(ref _sizeType, value);
}

// ❌ 错误
public event PropertyChangedEventHandler? PropertyChanged;
```

### 5.2 WhenActivated 使用

在 View 构造函数中注册 `WhenActivated`，在 `InitializeComponent` 之前：

```csharp
public XxxShowCase()
{
    this.WhenActivated(disposables =>
    {
        _viewModel = DataContext as XxxViewModel;
        // 设置订阅，disposables 管理生命周期
    });
    InitializeComponent();
}
```

### 5.3 IDisposable 管理

`WhenActivated` 的 `disposables` 参数用于管理订阅生命周期，View 停用时自动释放：

```csharp
this.WhenActivated(disposables =>
{
    // 使用 DisposeWith 确保订阅随 View 停用而释放
    this.WhenAnyValue(x => x.ViewModel!.SearchText)
        .Subscribe(text => FilterIcons(text))
        .DisposeWith(disposables);
});
```

### 5.4 路由规范

- 每个 ViewModel 必须实现 `IRoutableViewModel`
- `HostScreen` 引用传入的 `IScreen` 实例
- `UrlPathSegment` 使用 `ID.ToString()` 作为路由标识

## 6. 异常处理与资源管理

### 6.1 异常处理

- **事件处理器**：通常不包含 try-catch，依赖框架级异常处理
- **异步操作**：使用 `async/await`，较少使用 `ConfigureAwait(false)`
- **空值检查**：启用 `Nullable enable`，使用 `?` 和 `as` 模式匹配

### 6.2 资源管理

- **Dispatcher.UIThread**：确保 UI 操作在主线程执行
- **WhenActivated/DisposeWith**：管理订阅生命周期
- **Source Generator**：自动生成代码，减少手动资源管理

### 6.3 空值安全

项目启用 `Nullable enable`：

```csharp
<Nullable>enable</Nullable>
```

常见模式：

```csharp
// 可空字段
private XxxViewModel? _viewModel;

// as 模式匹配
if (DataContext is XxxViewModel vm) { }

// 空条件访问
if (sender is Button button) { }
```

## 7. Avalonia 特定规范

### 7.1 控件主题

自定义控件使用 `ControlTheme` 定义样式，通过 `ControlThemesProvider` 注册：

```csharp
// 控件主题文件命名
ShowCasePanelTheme.axaml    // 主题定义
ShowCasePanel.axaml.cs      // 控件逻辑
```

### 7.2 StyledProperty 定义

```csharp
public static readonly StyledProperty<string> TitleProperty =
    AvaloniaProperty.Register<ShowCaseItem, string>(nameof(Title));

public string Title
{
    get => GetValue(TitleProperty);
    set => SetValue(TitleProperty, value);
}
```

### 7.3 事件处理

优先使用 Code-Behind 事件处理而非 ViewModel Command 绑定：

```csharp
// XAML
<Button Click="HandleButtonClick"/>

// Code-Behind
public void HandleButtonClick(object? sender, RoutedEventArgs args)
{
    // 直接操作控件
}
```

## 8. 遵循的编码标准

| 标准 | 遵循程度 |
|------|---------|
| Microsoft C# 编码约定 | ✅ 高度遵循 |
| ReactiveUI 最佳实践 | ✅ 高度遵循 |
| Avalonia XAML 规范 | ✅ 高度遵循 |
| XML 文档注释 | ⚠️ 较少使用 |
| 单元测试覆盖 | ⚠️ 较低 |