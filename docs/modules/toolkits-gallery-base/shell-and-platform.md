# GalleryBase Shell 与平台宿主设计

本文档细化 GalleryBase 的 Desktop Window、Browser View、共享 Shell、品牌区域、标题栏菜单和平台差异。当前已抽出共享 Workspace ViewModel、导航运行时和产品配置；Desktop Window 与 Browser View 的视图层抽取仍是后续工作。

## 设计目标

- Desktop 和 Browser 共用同一套品牌、导航、路由和 Workspace ViewModel 基础。
- 产品侧只提供配置，不重写 Shell 布局。
- Shell 视觉使用 AtomUI 控件和 Token，但不写入任何产品品牌默认值。
- Desktop 差异和 Browser 差异封装在平台宿主边界。
- 支持未来扩展搜索、面包屑、页面元信息和响应式布局。

## Shell 组成目标

通用 Shell 结构：

```text
GalleryShell
  Sidebar
    BrandArea
    Navigation
    FooterLinks / Version
  ContentHost
    RoutedViewHost
  OptionalTopSeparator
  PlatformTitleBarMenu
```

Shell 不负责 Demo 页面内部布局。ShowCase 页面继续使用 `GalleryStickyTabsHost`、`ShowCasePanel` 和 `ShowCaseItem`。

## 共享 ViewModel

```csharp
public class GalleryWorkspaceViewModel : ReactiveObject, IScreen, IDisposable
{
    public RoutingState Router { get; }
    public GalleryNavigationViewModel Navigation { get; }

    public ReactiveCommand<bool, Unit> ToggleDarkModeCommand { get; }
    public ReactiveCommand<bool, Unit> ToggleCompactModeCommand { get; }
    public ReactiveCommand<bool, Unit> ToggleMotionCommand { get; }
    public ReactiveCommand<bool, Unit> ToggleWaveSpiritCommand { get; }
    public ReactiveCommand<Unit, Unit> SwitchToZhCNCommand { get; }
    public ReactiveCommand<Unit, Unit> SwitchToZhTWCommand { get; }
    public ReactiveCommand<Unit, Unit> SwitchToEnUSCommand { get; }
}
```

职责：

- 持有 ReactiveUI Router。
- 持有导航 ViewModel。
- 转发主题、紧凑、动效、语言切换命令。
- 监听 ThemeManager 语言变化并更新菜单状态。
- 释放时解绑 ThemeManager 语言事件，并释放 `GalleryNavigationViewModel`。

`GalleryWorkspaceViewModel` 不能知道具体产品页面类型。产品可以通过继承或组合方式提供自己的导航 ViewModel 类型别名，例如 AtomUI Gallery 的 `WorkspaceWindowViewModel` 继承 `GalleryWorkspaceViewModel`，并把 `CaseNavigation` 暴露为产品侧兼容属性。

宿主 View/Window 关闭或 Browser 根视图卸载时，如果其生命周期不是进程级单例，应调用 `Dispose()`。当前 AtomUI Gallery 的 `WorkspaceWindowViewModel` 继承该基类，因此同样获得导航诊断 timer 和语言事件的释放边界。

## Desktop 宿主目标

```csharp
public class GalleryWorkspaceWindow : ReactiveWindow<GalleryWorkspaceViewModel>
{
}
```

职责：

- 创建并绑定 `GalleryWorkspaceViewModel`。
- 配置 AtomUI Window title bar。
- 应用 `GalleryShellOptions.InitialDesktopWindowSize` 和 `MinDesktopWindowSize`。
- 根据配置显示或隐藏窗口选项菜单。
- 处理 caption button 可见性、移动、缩放、置顶等窗口行为。

Desktop 不负责：

- 注册产品页面。
- 创建产品导航树。
- 写死产品 logo 或链接。

## Browser 宿主目标

```csharp
public sealed class GalleryBrowserView : UserControl, IScreen, IMediaBreakAwareControl
{
}
```

职责：

- 创建并绑定 `GalleryWorkspaceViewModel`。
- 提供 Browser 单页面 `MainView`。
- 配置 Browser 需要的 overlay layers。
- 根据内容区域宽度维护 media breakpoint。
- 使用同一个 Gallery Shell 渲染侧边栏和内容路由区。

Browser 不再手写一份独立 sidebar/footer/routing host。它只能处理 Browser 平台差异。

## Branding 渲染

Branding 区域由 `GalleryBrandingOptions` 驱动：

```text
BrandArea
  LogoPresenter
  Optional AppName
```

规则：

- 有 Logo 时优先展示 Logo。
- 无 Logo 但有 AppName 时展示文本。
- Logo 不强制固定宽高，默认约束由 Shell Token 提供。
- 产品可通过 `GalleryBrandingOptions.Logo` 提供自定义 Control。

Footer 区域：

```text
Footer
  LinkButtons
  VersionTag
```

规则：

- `Links` 为空且 `VersionText` 为空时 footer 默认隐藏。
- `Links` 不为空时渲染超链接按钮。
- `VersionText` 不为空时渲染 Tag。
- 链接图标由产品配置提供；GalleryBase 不默认显示官网、GitHub 或 Gitee。

## 标题栏菜单

标题栏菜单分三组：

| 菜单 | 配置开关 | 职责 |
|---|---|---|
| Window Options | `IsWindowOptionsMenuEnabled` | 控制 caption button、移动、缩放 |
| Theme | `IsThemeMenuEnabled` | 暗色、紧凑、动效、WaveSpirit |
| Language | `IsLanguageMenuEnabled` | 切换 AtomUI 语言变体 |

语言菜单第一阶段提供 AtomUI 已支持的语言：

- `zh_CN`
- `zh_TW`
- `en_US`

后续如果 AtomUI 语言系统支持动态枚举，菜单应改成根据 `ThemeManager` 可用语言生成。

## 内容宿主

内容区使用 ReactiveUI `RoutedViewHost`：

```xml
<rxui:RoutedViewHost Router="{Binding Router}"
                     PageTransition="{x:Null}"
                     ClipToBounds="True" />
```

规则：

- Shell 不设置页面 DataContext。
- 页面 ViewModel 由路由创建。
- View 由 ViewLocator 创建。
- Shell 不缓存页面；缓存策略交给 ReactiveUI 和页面内部 controller。

## 媒体断点

Browser 宿主实现 `IMediaBreakAwareControl`，用于让 AtomUI 控件获得内容区域断点。

断点来源：

- Desktop 第一阶段不由 GalleryBase 强制提供，继续依赖 Window/AtomUI 现有机制。
- Browser 根据 ContentHost 宽度计算。

规则：

- 断点变化只由 Shell 内容区尺寸驱动，不由整个浏览器窗口直接驱动。
- 侧边栏宽度变化会自然影响内容断点。

## OverlayLayer

Browser 宿主需要初始化 `VisualLayerManager`：

- OverlayLayer
- PopupOverlayLayer
- LightDismissOverlayLayer

这是 AtomUI popup/flyout/tour/badge 等控件在 Browser Gallery 中正常工作的必要条件。GalleryBase 封装这段逻辑，产品不再复制反射访问 `VisualLayerManager` 私有属性的代码。

## 崩溃日志

Desktop 崩溃日志由启动项目决定是否使用 GalleryBase helper：

```csharp
GalleryDesktopCrashLogger.Log(ex, configuration.Platform.CrashLogDirectoryName);
```

规则：

- GalleryBase 提供 helper，但不强制包裹 `Main`。
- 产品可以替换为自己的日志系统。
- helper 不吞异常，只写日志后重新抛出。

## 测试要求

- Desktop Shell 不包含具体产品 logo URI。
- Browser Shell 不包含具体产品链接。
- Desktop 和 Browser 都使用 `GalleryWorkspaceViewModel`。
- Workspace ViewModel 可释放并释放导航运行时。
- Footer 在 links/version 为空时隐藏。
- 标题栏菜单按配置开关显示或隐藏。
- Browser OverlayLayer 初始化方法存在且只在 Browser View 使用。
- Browser 不再手写重复导航树。
