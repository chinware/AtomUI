# AtomUI Gallery Browser 改造问题记录

本文档记录 2026-06-07 进行 `AtomUIGallery.Browser` 初步改造时遇到的问题、根因和处理结论。当前目标是先让 Browser Gallery 以薄封装方式复用共享 `AtomUIGallery` 的 `AboutUsPage`，并允许 Browser 项目引用 `AtomUI.Desktop.Controls`，由控件库内部自动降级。

## 本轮目标

- 新增 Browser Gallery 项目并跑通空壳应用。
- Browser 启动阶段使用 AtomUI 红色 SVG logo。
- Browser Gallery 不复制 `AboutUsPage`，只做一层薄的宿主封装。
- Browser 可以引用 `AtomUI.Desktop.Controls`，用户侧继续调用 `UseDesktopControls()`，控件库内部按平台自动降级。
- 不改变公开 API，不影响 Desktop Gallery 原有行为。

## 遇到的问题

| 问题 | 现象 | 根因 | 处理结论 |
| --- | --- | --- | --- |
| Browser 直接引用 `AtomUI.Desktop.Controls` 后启动失败 | 页面 splash 显示 `AggregateException_ctor_DefaultMessage (Arg_InvalidCastException)` | `UseDesktopControls()` 注册完整 Desktop 主题资源时包含 Browser 不支持的 Window/TopLevel 相关资源和初始化逻辑 | `UseDesktopControls()` 保持公开 API 不变，内部根据 `RuntimePlatform.Features.SupportsNativeWindow` 切换 Browser 降级 provider |
| 缩小 Desktop provider 后仍启动失败 | 只保留 `HyperLinkButton`、`GroupBox`、`ScrollViewer` 后仍报同类异常 | `UseDesktopControls()` 会先调用 `UseCommonControls()`；失败点不在 Desktop provider，而在 Common provider | 先二分跳过 `UseCommonControls()` 验证，确认 root cause 后再改造 Common provider |
| Common provider 在 Browser 下不兼容 | 跳过 `UseCommonControls()` 后 Browser 能启动；恢复后失败 | `CommonControlThemesProvider` 包含 `Embedding/Themes/EmbeddableControlRootTheme.axaml`，该资源对 Browser 单视图场景不安全 | 新增 `BrowserCommonControlThemesProvider`，Browser 下排除 `EmbeddableControlRootTheme.axaml`；Desktop 仍使用原 `CommonControlThemesProvider` |
| `AboutUsPage` 复用后再次启动失败 | splash error: `Don't know how to detect when ... AboutUsPage is activated/deactivated` | Browser `AppBuilder` 没有注册 ReactiveUI Avalonia 集成，`AboutUsPage.WhenActivated(...)` 无法找到 activation fetcher | Browser `Program` 和 Desktop 对齐，调用 `UseReactiveUI(...)` 并注册 `ShowCaseViewModule` |
| 页面 DOM 看不到 AboutUs 文本 | `document.body.innerText` 为空，但没有启动错误 | Avalonia Browser 主要渲染在 canvas 中，DOM 文本不是可靠验证信号 | Browser 验证要看 splash error、canvas 是否存在，并用截图确认实际渲染 |
| 控制台日志容易混入旧端口错误 | Console 中出现历史端口的 `libSkiaSharp` 或旧 wasm 异常 | 多次运行 Browser dev server 后，浏览器日志会保留旧页面/旧端口记录 | 每次验证前停止旧宿主，重新 build/run，并以当前端口页面的 splash error 和截图为准 |
| Browser static web assets 有重复项风险 | Browser 构建/运行阶段可能遇到重复 wasm static asset | Browser 项目引用共享 Gallery 和控件库后，静态资源解析链路更复杂 | Browser 项目保留 `DeduplicateBrowserWasmStaticWebAssets` target，去重 `WasmStaticWebAsset` |

## 当前实现要点

### Browser Gallery 宿主

Browser Gallery 项目位于：

```text
controlgallery/AtomUIGallery.Browser/
```

关键入口：

```csharp
AppBuilder.Configure<BrowserGalleryApplication>()
    .UseReactiveUI(build =>
        build.ConfigureViewLocator(locator => new ShowCaseViewModule().RegisterViews(locator)));
```

`BrowserGalleryApplication.Initialize()` 中继续使用和 Desktop 相同的 AtomUI 配置主线：

```csharp
this.UseAtomUI(builder =>
{
    builder.WithDefaultCultureInfo(CultureInfo.CurrentUICulture);
    builder.WithDefaultTheme(IThemeManager.DEFAULT_THEME_ID);
    builder.UseAlibabaSansFont();
    builder.UseDesktopControls();
    builder.UseGalleryControls();
});
```

`BrowserGalleryView` 只负责 Browser 单视图宿主和导航外壳，内容直接创建共享页面：

```csharp
var aboutUsPage = new AboutUsPage
{
    DataContext = new AboutUsViewModel(this)
};
```

### Common 控件降级

`UseCommonControls()` 不改变调用方式。内部按平台选择 provider：

```csharp
themeManagerBuilder.AddControlThemesProvider(RuntimePlatform.Features.SupportsNativeWindow
    ? new CommonControlThemesProvider()
    : new BrowserCommonControlThemesProvider());
```

Browser common provider 当前包含：

```text
Icon/Themes/IconThemes.axaml
ItemsControl/Themes/ItemsControlTheme.axaml
TextSelectionHandle/Themes/TextSelectionHandleTheme.axaml
TransitioningContentControl/Themes/TransitioningContentControlTheme.axaml
```

明确排除：

```text
Embedding/Themes/EmbeddableControlRootTheme.axaml
```

### Desktop 控件降级

`UseDesktopControls()` 不改变调用方式。内部按平台选择 token 和 provider：

```csharp
var controlTokenTypes = RuntimePlatform.Features.SupportsNativeWindow
    ? ControlTokenTypePool.GetTokenTypes()
    : GetBrowserControlTokenTypes();

themeManagerBuilder.AddControlThemesProvider(RuntimePlatform.Features.SupportsNativeWindow
    ? new DesktopControlThemesProvider()
    : new BrowserDesktopControlThemesProvider());
```

Browser desktop provider 当前只承载 `AboutUsPage` 需要的最小集合：

```text
Buttons/Themes/HyperLinkButtonTheme.axaml
GroupBox/Themes/GroupBoxTheme.axaml
ScrollViewer/Themes/ScrollViewerThemes.axaml
```

Browser token 当前只注册：

```text
ButtonToken
GroupBoxToken
ScrollViewerToken
```

Desktop 下仍然使用完整 `ControlTokenTypePool` 和 `DesktopControlThemesProvider`，并继续注册原有 initialized handler。Browser 下跳过该 handler，避免注册 Window/Tooltip/媒体断点等 native window 相关逻辑。

## 调试过程记录

本轮采用二分定位：

1. 验证 bare Browser app 可以启动。
2. 验证 `UseAtomUI()` 可以启动。
3. 验证 culture/theme/font 可以启动。
4. 加入 `UseDesktopControls()` 后复现 `Arg_InvalidCastException`。
5. 缩小 `DesktopControlThemesProvider` 和 Desktop token 后仍失败。
6. 临时跳过 `UseCommonControls()` 后启动成功，定位到 Common provider。
7. Browser common provider 排除 `EmbeddableControlRootTheme` 后启动成功。
8. 切回共享 `AboutUsPage` 后出现 ReactiveUI activation 错误。
9. Browser `Program` 补齐 `UseReactiveUI(...)` 后 AboutUs 正常渲染。

## 验证结果

已执行：

```bash
dotnet build controlgallery/AtomUIGallery.Browser/AtomUIGallery.Browser.csproj -c Debug
dotnet build controlgallery/AtomUIGallery.Desktop/AtomUIGallery.Desktop.csproj -c Debug
```

结果：

- Browser Debug build 通过。
- Desktop Gallery Debug build 通过。
- Browser 实跑 `AboutUsPage` 渲染成功，无 `.splash-error`。

Browser 运行验证时不要只依赖 DOM 文本，因为 Avalonia Browser 页面内容主要在 canvas 中绘制；需要结合 splash error 和截图。

## 后续扩展规则

1. Browser 支持继续以“自动降级、用户无感知”为目标，不新增公开 API。
2. 先让用户继续调用现有 `UseDesktopControls()`，由内部 provider/token 根据平台选择。
3. 每新增一个 Browser 可用控件，优先只把该控件需要的 token/theme 加到 Browser provider，不直接放开完整 Desktop provider。
4. 涉及 Window、WindowTitleBar、Dialog window host、Notifications native host、ImagePreviewer window host、文件系统枚举等能力时，必须走 `RuntimePlatform.Features` 能力判断。
5. Browser 验证必须同时跑 Browser build、Browser 实跑 smoke 和 Desktop build，防止降级分支影响 Desktop。
6. 如果 Browser 页面启动失败，先看当前端口 splash error；不要被旧端口 console 日志误导。

## 待继续处理

- `UseDesktopColorPicker()` 和 `UseDesktopDataGrid()` 尚未接入 Browser 降级路径。
- Browser Gallery 当前只接入 `AboutUsPage`，完整 Workspace / AppView / AppShell 还未实现。
- `BrowserDesktopControlThemesProvider` 的范围后续应随着 ShowCase 迁移逐步扩大，并保持最小可用集合。
- `EmbeddableControlRootTheme` 在 Browser 下的精确不兼容点后续可以进一步拆解；当前先通过 Browser provider 排除该资源，保证启动路径稳定。
