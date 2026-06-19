# GalleryBase 导航与路由设计

本文档细化 GalleryBase 的导航树、路由注册、ReactiveUI ViewLocator 适配和导航生命周期。该部分是产品复用的核心：产品只描述“有哪些页面”和“如何创建页面”，GalleryBase 负责展示导航并完成路由。

## 设计目标

- 彻底移除当前 `CaseNavigation.axaml` 和 `CaseNavigationViewModel` 中的产品硬编码。
- 导航树由产品配置生成，支持分组、页面、默认展开路径和默认页面。
- 路由注册显式、可校验、兼容 AOT 和 Browser。
- Desktop 和 Browser 使用同一套导航与路由。
- 导航节点的显示文案和路由身份分离，语言切换不影响路由。

## 导航节点模型

```csharp
public sealed class GalleryNavigationNode
{
    public string Key { get; }
    public object Header { get; }
    public object? Icon { get; }
    public bool IsRoute { get; }
    public IReadOnlyList<GalleryNavigationNode> Children { get; }
}
```

字段说明：

| 字段 | 说明 |
|---|---|
| `Key` | 稳定身份；页面节点必须等于 route key，分组节点是导航路径身份 |
| `Header` | 显示内容；可以是字符串、语言资源引用对象或轻量 ViewModel |
| `Icon` | 图标内容；默认适配 AtomUI icon provider 和 Avalonia `IconElement` |
| `IsRoute` | `true` 表示点击后导航；`false` 表示分组 |
| `Children` | 子节点；页面节点第一阶段不允许有子节点 |

## 导航构建器

产品侧使用 builder 构造导航：

```csharp
options.Navigation.DefaultRoute = "Overview";
options.Navigation.DefaultOpenKeys.Add("Components");

options.Navigation
    .AddPage("Overview", "Overview", GalleryIcon.Home)
    .AddPage("Community", "Community", GalleryIcon.Team);

options.Navigation
    .AddGroup("Components", "Components", GalleryIcon.Apps)
    .AddGroup("General", "General", GalleryIcon.Control)
    .AddPage("Button", "Button")
    .AddPage("Icon", "Icon");
```

Builder 规则：

- `AddPage(key, header, icon)` 创建 route 节点。
- `AddGroup(key, header, icon)` 创建分组节点。
- 同一棵树内 `Key` 全局唯一。
- 分组 key 可以没有 route。
- 页面 key 必须有 route。
- Header 不参与唯一性判断。

## 本地化 Header

第一阶段允许 `Header` 为字符串。为了支持 Shell 级语言切换，预留 `IGalleryLocalizedText`：

```csharp
public interface IGalleryLocalizedText
{
    string Resolve();
}
```

产品可在后续提供：

```csharp
options.Navigation.AddPage(
    "Button",
    GalleryLang.Text(ButtonNavigationLangResourceKind.Button));
```

第一阶段如果产品仍用 `{gallery:...LangResource}` 绑定页面内部文案，不受导航模型影响。

## NavMenu 适配

GalleryBase 提供内部适配器：

```csharp
internal sealed class GalleryNavigationMenuAdapter
{
    public IReadOnlyList<INavMenuNode> BuildNodes(
        IReadOnlyList<GalleryNavigationNode> nodes);
}
```

职责：

- 把 `GalleryNavigationNode` 转为 AtomUI `NavMenuNode`。
- 设置 `ItemKey`。
- 应用图标。
- 设置默认展开路径。
- 处理 Header 内容和语言刷新。

该适配器是 UI 实现细节，不暴露为产品 API。未来如果 GalleryBase 支持其他 UI 实现，可以替换该适配层。

## 路由注册模型

```csharp
public sealed class GalleryRouteRegistry
{
    public void Map<TViewModel, TView>(
        string routeKey,
        Func<IScreen, TViewModel> viewModelFactory,
        Func<TView>? viewFactory = null)
        where TViewModel : class, IRoutableViewModel
        where TView : class, IViewFor<TViewModel>;
}
```

注册后生成：

```csharp
internal sealed class GalleryRouteDescriptor
{
    public string RouteKey { get; }
    public Type ViewModelType { get; }
    public Type ViewType { get; }
    public Func<IScreen, IRoutableViewModel> CreateViewModel { get; }
    public Func<IViewFor> CreateView { get; }
}
```

`viewFactory` 允许为空。为空时第一阶段可以要求 `TView` 有公开无参构造；否则配置校验失败。这样避免运行时通过反射找不到构造器。

## ViewLocator 适配

现有 Gallery 使用 ReactiveUI `DefaultViewLocator.Map<TViewModel,TView>()`。GalleryBase 应继续适配这个机制：

```csharp
public void RegisterViews(DefaultViewLocator locator)
{
    foreach (var route in _routes)
    {
        locator.Map(route.ViewModelType, route.CreateView);
    }
}
```

如果 `DefaultViewLocator` 没有非泛型 `Map`，GalleryBase 内部提供一个小型 adapter：

```csharp
public interface IGalleryViewLocatorRegistrar
{
    void Register<TViewModel, TView>(Func<TView> viewFactory)
        where TViewModel : class
        where TView : IViewFor<TViewModel>;
}
```

产品启动项目不直接创建 `ShowCaseViewModule`，而是调用产品模块暴露的 registry：

```csharp
AppBuilder.Configure<GalleryApplication>()
    .UseReactiveUI(build =>
        build.ConfigureViewLocator(locator =>
            AtomUIGalleryModule.RegisterViews(locator)));
```

## 导航 ViewModel

`GalleryNavigationViewModel` 替代当前产品硬编码的 `CaseNavigationViewModel`：

```csharp
public sealed class GalleryNavigationViewModel : ReactiveObject, IActivatableViewModel
{
    public ReactiveCommand<string, Unit> NavigateToCommand { get; }
    public ReactiveCommand<TimeSpan, Unit> StartNavigationDiagnosticsCommand { get; }
    public ReactiveCommand<Unit, Unit> StopNavigationDiagnosticsCommand { get; }

    public bool CanNavigateTo(string routeKey);
}
```

职责：

- 保存当前 route key。
- 根据 route key 从 `GalleryRouteRegistry` 创建 ViewModel。
- 调用 `HostScreen.Router.NavigateAndReset`。
- 避免重复导航到当前页面。
- 提供诊断命令循环切换页面，用于内存和延迟加载压测。

## 导航流程

```text
User clicks NavMenu item
  -> GalleryNavigationView receives item key
  -> GalleryNavigationViewModel.CanNavigateTo(key)
  -> GalleryRouteRegistry.CreateViewModel(key, hostScreen)
  -> HostScreen.Router.NavigateAndReset(viewModel)
  -> ReactiveUI ViewLocator creates View
  -> RoutedViewHost displays View
```

失败处理：

| 失败点 | 处理 |
|---|---|
| 点击分组节点 | 不导航 |
| route key 不存在 | 忽略点击并记录诊断，配置校验应提前防止 |
| ViewModel factory 抛异常 | 让异常向上冒泡，测试和启动阶段暴露问题 |
| ViewLocator 未注册 View | 抛出带 ViewModel type 的错误 |

## 默认路由

`GalleryNavigationViewModel` 激活时导航到 `DefaultRoute`：

```csharp
Activator.Activated.Subscribe(_ =>
{
    NavigateTo(DefaultRoute);
});
```

规则：

- `DefaultRoute` 必须是页面节点。
- 默认路由不允许是分组节点。
- Browser 和 Desktop 默认路由一致。

## 测试要求

- 导航树重复 key 报错。
- 页面节点没有 route 报错。
- 默认路由不存在报错。
- 点击分组节点不触发 route factory。
- 点击页面节点只创建目标 ViewModel。
- 重复点击当前页面不重复创建 ViewModel。
- 诊断自动切页会暂时关闭 ShowCase 延迟创建，并在停止后恢复。
- Browser 和 Desktop 使用同一份 `GalleryRouteRegistry`。
