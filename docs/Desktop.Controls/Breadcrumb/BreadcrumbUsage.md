# Breadcrumb 使用文档

本文档介绍 AtomUI Breadcrumb 控件的各种使用方式，示例代码摘自 Gallery 演示程序。

> 📖 Gallery 源码位置：`controlgallery/AtomUIGallery/ShowCases/Views/Navigation/BreadcrumbShowCase.axaml`

---

## 前置准备

在 AXAML 中使用 Breadcrumb，需要引入以下命名空间：

```xml
xmlns:atom="https://atomui.net"
xmlns:antdicons="https://atomui.net/icons/antdesign"
```

在 C# 代码中引入：

```csharp
using AtomUI.Desktop.Controls;   // Breadcrumb, BreadcrumbItem, BreadcrumbItemData
```

---

## 1. 基本用法

最简单的面包屑导航，直接在 AXAML 中声明 `BreadcrumbItem` 子项：

```xml
<atom:Breadcrumb>
    <atom:BreadcrumbItem>Home</atom:BreadcrumbItem>
    <atom:BreadcrumbItem NavigateContext="#">Application Center</atom:BreadcrumbItem>
    <atom:BreadcrumbItem NavigateContext="#">Application List</atom:BreadcrumbItem>
    <atom:BreadcrumbItem>An Application</atom:BreadcrumbItem>
</atom:Breadcrumb>
```

**说明**：
- 设置了 `NavigateContext` 的面包屑项可点击（鼠标变为手形，文字显示链接色）
- 最后一项自动标记为当前页面（文字颜色加深，不可点击）
- 分隔符默认为 `/`，自动显示在相邻面包屑项之间（最后一项不显示分隔符）

---

## 2. 带图标的面包屑

通过 `Icon` 属性为面包屑项添加前置图标，图标使用 Ant Design 图标集：

```xml
<atom:Breadcrumb>
    <atom:BreadcrumbItem
        Icon="{antdicons:AntDesignIconProvider Kind=HomeOutlined}" />
    <atom:BreadcrumbItem
        Icon="{antdicons:AntDesignIconProvider Kind=UserOutlined}"
        NavigateContext="#">
        Application List
    </atom:BreadcrumbItem>
    <atom:BreadcrumbItem>Application</atom:BreadcrumbItem>
</atom:Breadcrumb>
```

**使用场景**：
- 首页图标（HomeOutlined）通常只显示图标不显示文字
- 其他层级可以同时显示图标和文字，增强视觉辨识度

---

## 3. 自定义分隔符

### 全局分隔符

通过 `Breadcrumb.Separator` 属性设置全局分隔符，所有面包屑项共享：

```xml
<atom:Breadcrumb Separator=">">
    <atom:BreadcrumbItem>Home</atom:BreadcrumbItem>
    <atom:BreadcrumbItem NavigateContext="#">Application Center</atom:BreadcrumbItem>
    <atom:BreadcrumbItem NavigateContext="#">Application List</atom:BreadcrumbItem>
    <atom:BreadcrumbItem>An Application</atom:BreadcrumbItem>
</atom:Breadcrumb>
```

### 面包屑项独立分隔符

通过 `BreadcrumbItem.Separator` 属性为单个面包屑项设置独立分隔符，优先级高于全局设置：

```xml
<atom:Breadcrumb>
    <atom:BreadcrumbItem Separator=":">Location</atom:BreadcrumbItem>
    <atom:BreadcrumbItem NavigateContext="#">Application Center</atom:BreadcrumbItem>
    <atom:BreadcrumbItem NavigateContext="#">Application List</atom:BreadcrumbItem>
    <atom:BreadcrumbItem>An Application</atom:BreadcrumbItem>
</atom:Breadcrumb>
```

**说明**：第一项使用 `:` 作为分隔符，其余项继承全局默认的 `/`。

### 使用分隔符模板

通过 `SeparatorTemplate` 使用 `DataTemplate` 实现更复杂的分隔符样式（如图标）：

```xml
<atom:Breadcrumb>
    <atom:Breadcrumb.SeparatorTemplate>
        <DataTemplate>
            <antdicons:RightOutlined Width="12" Height="12" />
        </DataTemplate>
    </atom:Breadcrumb.SeparatorTemplate>
    <atom:BreadcrumbItem>Home</atom:BreadcrumbItem>
    <atom:BreadcrumbItem NavigateContext="#">Application Center</atom:BreadcrumbItem>
    <atom:BreadcrumbItem>An Application</atom:BreadcrumbItem>
</atom:Breadcrumb>
```

---

## 4. 导航功能

### 应用内导航（NavigateContext）

使用 `NavigateContext` 属性携带导航上下文数据，点击面包屑项时触发父级 `Breadcrumb` 的 `NavigateRequest` 事件：

```xml
<atom:Breadcrumb NavigateRequest="HandleNavigateRequest">
    <atom:BreadcrumbItem>Users</atom:BreadcrumbItem>
    <atom:BreadcrumbItem NavigateContext="Param(1)">Param</atom:BreadcrumbItem>
</atom:Breadcrumb>
```

```csharp
// Code-behind
private void HandleNavigateRequest(object? sender, BreadcrumbNavigateEventArgs eventArgs)
{
    // eventArgs.BreadcrumbItem 是触发导航的面包屑项
    var context = eventArgs.BreadcrumbItem.NavigateContext;
    // 执行应用内路由跳转...
    _messageManager?.Show(new Message(
        $"Navigate context: {context}"
    ));
}
```

### 外部链接导航（NavigateUri）

使用 `NavigateUri` 属性设置外部链接，点击时自动通过系统启动器打开：

```xml
<atom:Breadcrumb>
    <atom:BreadcrumbItem>Home</atom:BreadcrumbItem>
    <atom:BreadcrumbItem NavigateUri="https://ant.design">
        Ant Design
    </atom:BreadcrumbItem>
    <atom:BreadcrumbItem>Current Page</atom:BreadcrumbItem>
</atom:Breadcrumb>
```

**说明**：`NavigateUri` 会调用 `TopLevel.Launcher.LaunchUriAsync` 打开 URI，适用于需要跳转到外部网页的场景。

---

## 5. 数据驱动模式（MVVM）

通过 `ItemsSource` 绑定数据集合，配合 `BreadcrumbItemData` 实现数据驱动：

### AXAML

```xml
<atom:Breadcrumb Name="TplBreadcrumb">
    <atom:Breadcrumb.ItemTemplate>
        <DataTemplate x:DataType="atom:BreadcrumbItemData">
            <TextBlock Text="{Binding Content}" />
        </DataTemplate>
    </atom:Breadcrumb.ItemTemplate>
</atom:Breadcrumb>
```

### ViewModel

```csharp
public class BreadcrumbViewModel : ReactiveObject
{
    private List<BreadcrumbItemData>? _breadcrumbItems;
    public List<BreadcrumbItemData>? BreadcrumbItems
    {
        get => _breadcrumbItems;
        set => this.RaiseAndSetIfChanged(ref _breadcrumbItems, value);
    }
}
```

### Code-behind（使用 ReactiveUI 绑定）

```csharp
this.WhenActivated(disposables =>
{
    if (DataContext is BreadcrumbViewModel viewModel)
    {
        viewModel.BreadcrumbItems = [
            new BreadcrumbItemData()
            {
                Separator = ":",
                Content = "Location"
            },
            new BreadcrumbItemData()
            {
                NavigateContext = "#",
                Content = "Application Center"
            },
            new BreadcrumbItemData()
            {
                NavigateContext = "#",
                Content = "Application List"
            },
            new BreadcrumbItemData()
            {
                Content = "An Application"
            }
        ];
        
        this.OneWayBind(viewModel,
                vm => vm.BreadcrumbItems,
                v => v.TplBreadcrumb.ItemsSource)
            .DisposeWith(disposables);
    }
});
```

**数据映射规则**：当 `ItemsSource` 中的数据对象实现 `IBreadcrumbItemData` 接口时，Breadcrumb 会自动将以下属性映射到生成的 `BreadcrumbItem`：
- `Content` → `BreadcrumbItem.Content`
- `Icon` → `BreadcrumbItem.Icon`
- `Separator` → `BreadcrumbItem.Separator`（仅当数据中不为 null）
- `SeparatorTemplate` → `BreadcrumbItem.SeparatorTemplate`（仅当数据中不为 null）
- `NavigateContext` → `BreadcrumbItem.NavigateContext`（仅当数据中不为 null）
- `NavigateUri` → `BreadcrumbItem.NavigateUri`（仅当数据中不为 null）

---

## 6. 控制动画行为

```xml
<!-- 禁用过渡动画（悬浮时前景色、背景色不再渐变过渡） -->
<atom:Breadcrumb IsMotionEnabled="False">
    <atom:BreadcrumbItem NavigateContext="#">Home</atom:BreadcrumbItem>
    <atom:BreadcrumbItem>Current Page</atom:BreadcrumbItem>
</atom:Breadcrumb>
```

---

## 常见组合模式

### 页面顶部导航路径

```xml
<StackPanel Spacing="8">
    <!-- 面包屑导航 -->
    <atom:Breadcrumb>
        <atom:BreadcrumbItem
            Icon="{antdicons:AntDesignIconProvider Kind=HomeOutlined}"
            NavigateContext="home" />
        <atom:BreadcrumbItem NavigateContext="users">User Management</atom:BreadcrumbItem>
        <atom:BreadcrumbItem>User List</atom:BreadcrumbItem>
    </atom:Breadcrumb>
    
    <!-- 页面标题 -->
    <TextBlock Text="User List" FontSize="24" FontWeight="Bold" />
    
    <!-- 页面内容 -->
    <!-- ... -->
</StackPanel>
```

### 文件路径展示

```xml
<atom:Breadcrumb Separator=">">
    <atom:BreadcrumbItem
        Icon="{antdicons:AntDesignIconProvider Kind=FolderOutlined}"
        NavigateContext="root">
        Root
    </atom:BreadcrumbItem>
    <atom:BreadcrumbItem
        Icon="{antdicons:AntDesignIconProvider Kind=FolderOutlined}"
        NavigateContext="documents">
        Documents
    </atom:BreadcrumbItem>
    <atom:BreadcrumbItem
        Icon="{antdicons:AntDesignIconProvider Kind=FileOutlined}">
        Report.pdf
    </atom:BreadcrumbItem>
</atom:Breadcrumb>
```

### 带消息反馈的导航

```csharp
// 在 OnAttachedToVisualTree 中初始化消息管理器
protected override void OnAttachedToVisualTree(VisualTreeAttachmentEventArgs e)
{
    base.OnAttachedToVisualTree(e);
    var topLevel = TopLevel.GetTopLevel(this);
    _messageManager = new WindowMessageManager(topLevel) { MaxItems = 10 };
}

// 导航事件处理
private void HandleNavigateRequest(object? sender, BreadcrumbNavigateEventArgs eventArgs)
{
    var item = eventArgs.BreadcrumbItem;
    _messageManager?.Show(new Message(
        $"Navigate to: {item.Content} (context: {item.NavigateContext})"
    ));
}
```
