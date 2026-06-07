# AtomUI Gallery Browser 改造问题记录

本文档记录 2026-06-07 进行 `AtomUIGallery.Browser` 初步改造时遇到的问题、根因和处理结论。当前目标是让 Browser Gallery 以薄封装方式复用共享 `AtomUIGallery` 的 ShowCase，并允许 Browser 项目引用 `AtomUI.Desktop.Controls`，由控件库内部自动降级。

## 本轮目标

- 新增 Browser Gallery 项目并跑通空壳应用。
- Browser 启动阶段使用 AtomUI 红色 SVG logo。
- Browser Gallery 不复制 `AboutUsPage` / `ButtonShowCase` / `FloatButtonShowCase` / `PaletteShowCase` / `IconShowCase`，只做一层薄的宿主封装。
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
| Icons 迁移需要输入控件链路 | `IconGallery` 使用 `SearchEdit`、`SearchButton`、`InputClearIconButton`、`RevealButton` 和 Desktop `ScrollViewer` | Browser desktop provider 原先只覆盖 AboutUs / Palette 所需主题，缺少 SearchEdit 相关主题和输入 token | 新增 `Input/Themes/BrowserInputThemes.axaml`，只纳入 Icons 实际需要的 SearchEdit 链路；Browser token 白名单补 `AddOnDecoratedBoxToken` 和 `LineEditToken` |
| Button 迁移后 Browser 白屏 | 临时把 Browser 默认页切到 `ButtonShowCase` 时，页面无 `.splash-error` 但 canvas 持续白屏 | `ButtonShowCase` 首次引入 `ShowCasePanel` / `ShowCaseItem` / `Separator` / `OptionButtonGroup`，并且完整 Desktop `ButtonTheme` 在 Browser 首帧下过重且不稳定 | Browser provider 补 `Separator`、`OptionButtonGroup` 主题和 token；`BrowserButtonThemes.axaml` 使用轻量 `atom:Button` 降级主题，但保留 hover/pressed/loading/wave/icon/dashed/ghost 等交互表达，Desktop 仍走完整 `ButtonTheme` |
| Browser Button 视觉降级过头 | Button ShowCase 中 dashed、icon-only、Search、Ghost 等区域与 Desktop 正常效果差异明显 | 首版 Browser Button 主题只保留基础状态，缺少 Desktop 的 Frame 高度、icon-only padding/尺寸、DashedBorder 和 Ghost/Danger 组合样式 | 在不恢复完整 Desktop `ButtonTheme` 的前提下，补齐 Browser Button 的关键可见样式；Release host 中验证 Button 和 Ghost 区域正常渲染 |
| Link Button 在 Ghost 区域出现白底 | Desktop Gallery 的 Ghost Button 示例里，Link 类型按钮在灰色背景上出现白色背景块 | Desktop `ButtonTheme` 的 `ButtonType=Link` 分支直接给 template `Border#Frame` 设置了 `DefaultBg`，覆盖了按钮自身的透明背景 | 删除 Link 类型对 `Frame.Background` 的白底设置，改为 Link 按钮自身 `Background=Transparent`，让 template binding 正常透传 |
| FloatButton 迁移需要 OverlayLayer | Browser 加入 `FloatButtonShowCase` 后需要让悬浮按钮挂载到正确的局部 overlay | `FloatButtonHost` / `FloatButtonGroupHost` 不依赖 Window，依赖 `ScopeAwareOverlayLayer`；ShowCase 内的 `atom:ScrollViewer` 模板已经提供 `ScopeAwareOverlayLayerPanel` | Browser provider 补 `FloatButton`、`Badge`、`ToggleSwitch` 主题和 token，继续复用共享 ShowCase，不新增 Browser 专用页面 |
| FloatButtonGroup 存在重复父级风险 | 页面缓存、隐藏/显示或重新套模板后，同一批子按钮可能被再次加入内部 `FloatButtonItemsControl` | `FloatButtonGroupHost` 把 Host `Children` 转移到 overlay 内的 `FloatButtonGroup`，旧 group / items control 没有在 detach 或 re-template 时清理子项；click trigger 在 detach 时也可能重新订阅输入事件 | 内部补齐 group children、items control children 和 items layout children 的成对清理；click trigger 只在已附着视觉树时订阅，detach 时释放 |
| Browser FloatButton Placement 展开缺子按钮 | Placement 示例中点击左侧展开组后，trigger 变为 X，但 X 左侧两个子按钮没有显示 | Browser 分支此前完全跳过 Desktop initialized handler，导致 `MotionTransformOptionsAnimator` 没有注册；`MoveRightInMotion` / `MoveLeftInMotion` 的 `TransformOperations` keyframe 动画停留在隐藏起始态 | initialized handler 所有平台都注册 `TransformOperations` animator；`ToolTipService` 和 `MediaBreakPointThemeBootstrapper` 继续只在 native window 下启用 |
| FloatButton Tooltip 暂未打开 Browser 完整链路 | Tooltip 属性在 Browser 下不会导致页面崩溃，但本轮不验证 tooltip 弹层 | AtomUI Tooltip 会继续牵出 `Popup`、`PopupRoot`、`OverlayPopupHost`、`ArrowDecoratedBox`、`PopupHostToken`、`ToolTipService` 等一组资源，超出 FloatButton ShowCase 首轮接入范围 | 本轮先保证 FloatButton 主体、Badge、BackTop、Group 可用；Tooltip/Popup Browser 降级后续单独拆分验证 |
| Browser shell 字体未继承 AtomUI 字体 | Browser Gallery 外壳文字看起来没有使用 AtomUI 的 AlibabaSans | Browser 根是 `UserControl`，没有 Desktop `atom:Window` theme 里的 `FontFamily="{atom:SharedTokenResource FontFamily}"` 继承入口 | `BrowserGalleryView` 根设置 `fonts:AlibabaSans#Alibaba Sans, $Default`，由 Avalonia 可继承 `FontFamily` 向导航外壳和共享 ShowCase 传递 |
| Browser 导航切页卡顿明显 | AboutUs / Button / Palette / Icons 之间切换时，新页面出来有明显停顿 | Browser 宿主原先每次点击都 `new` 页面和 ViewModel，并替换 `ContentControl.Content`，导致 ShowCase XAML、控件模板、主题匹配、首次布局和绘制全部冷启动 | `BrowserGalleryView` 改为 lazy page cache，首次进入页面后保留在同一个 `Grid` 中，切换只调整 `IsVisible`；首屏加载后再按 idle 顺序预热 Button / Palette / Icons，降低用户首次点击重页面时的冷启动体感 |

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

`BrowserGalleryView` 只负责 Browser 单视图宿主、导航外壳和当前 ShowCase 切换，内容直接创建共享页面，并按页面类型 lazy cache：

```csharp
var aboutUsPage = new AboutUsPage
{
    DataContext = new AboutUsViewModel(this)
};

var buttonShowCase = new ButtonShowCase
{
    DataContext = new ButtonViewModel(this)
};

var floatButtonShowCase = new FloatButtonShowCase
{
    DataContext = new FloatButtonViewModel(this)
};

var paletteShowCase = new PaletteShowCase
{
    DataContext = new PaletteViewModel(this)
};

var iconShowCase = new IconShowCase
{
    DataContext = new IconViewModel(this)
};
```

页面首次打开后会保留在 Browser 宿主的内容 `Grid` 中，非当前页通过 `IsVisible = false` 隐藏。这样可以保留 ViewModel、控件树、滚动位置和 Icons 已加载内容，避免切回时再次触发 XAML inflate / template / layout 冷启动。

首屏 `AboutUsPage` 加载完成后，Browser 宿主会用 `DispatcherPriority.ApplicationIdle` 依次预热 `ButtonShowCase`、`FloatButtonShowCase`、`PaletteShowCase` 和 `IconShowCase`。预热页面先以 `Opacity = 0`、`IsHitTestVisible = false` 放入内容 `Grid`，让模板和布局在用户空闲时完成一轮，再隐藏回缓存；如果用户在预热期间点击该页面，正常导航会接管可见性。

当前 Browser 已接入页面没有后台定时任务；后续迁移有持续订阅或异步任务的 ShowCase 时，需要单独评估隐藏页保持 attached 的成本。

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

Browser desktop provider 当前只承载 Browser Gallery 已接入 ShowCase 需要的最小集合：

```text
Badge/Themes/BadgeThemes.axaml
Buttons/Themes/BrowserButtonThemes.axaml
FloatButton/Themes/FloatButtonThemes.axaml
GroupBox/Themes/GroupBoxTheme.axaml
Input/Themes/BrowserInputThemes.axaml
OptionButtonGroup/Themes/OptionButtonBoxThemes.axaml
ScrollViewer/Themes/ScrollViewerThemes.axaml
Separator/Themes/SeparatorTheme.axaml
Switch/Themes/ToggleSwitchThemes.axaml
TabControl/Themes/TabControlThemes.axaml
```

Browser token 当前只注册：

```text
AddOnDecoratedBoxToken
BadgeToken
ButtonToken
FloatButtonToken
GroupBoxToken
LineEditToken
MenuToken
OptionButtonToken
ScrollViewerToken
SeparatorToken
TabControlToken
ToggleSwitchToken
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
dotnet build controlgallery/AtomUIGallery.Browser/AtomUIGallery.Browser.csproj -c Release
dotnet build controlgallery/AtomUIGallery.Desktop/AtomUIGallery.Desktop.csproj -c Debug
```

结果：

- Browser Debug build 通过。
- Browser Release build 通过。
- Desktop Gallery Debug build 通过。
- Browser 实跑 `AboutUsPage` / `ButtonShowCase` / `FloatButtonShowCase` / `PaletteShowCase` / `IconShowCase` 渲染成功，无 `.splash-error`。
- `ButtonShowCase` 在 Release host 中验证首屏渲染成功，Button hover 状态可见，点击 smoke test 无当前端口错误日志；最终代码仍默认进入 `AboutUsPage`。
- `FloatButtonShowCase` 在 Release host 中验证首屏渲染成功；切到 Button 后再切回 FloatButton 仍正常渲染；向下滚动到 Placement / Badge / BackTop 区域后渲染正常；点击 Placement click group 后无当前端口错误日志。
- Browser 修复 `MotionTransformOptionsAnimator` 注册后，Release host 中复测 Placement 左侧展开组，X 左侧两个子按钮正常显示，无当前端口错误日志。
- `IconShowCase` 的 `Outlined` / `Filled` / `Two Tone` tab 均已在 Browser 中切换并渲染图标网格。
- Browser 导航 lazy cache 后，Release host 中执行 Button / Icons / Button / Icons 切换 smoke test，Icons 二次切回仍正常渲染，无当前端口错误日志。
- Browser 导航 idle 预热后，Release host 中等待首屏预热并执行 Button / Palette / Icons 切换 smoke test，最终 Icons 正常渲染，无当前端口错误日志。

Browser 运行验证时不要只依赖 DOM 文本，因为 Avalonia Browser 页面内容主要在 canvas 中绘制；需要结合 splash error 和截图。

本轮使用 Browser 自动化尝试向 `SearchEdit` 输入过滤文本时，键盘事件没有可靠送入 Avalonia canvas；因此搜索过滤不作为本轮自动化验证结论。当前已确认 `SearchEdit` 视觉渲染、焦点态和搜索按钮存在，后续如需严格验证文本输入，应结合手工操作或专门的 Avalonia Browser 输入测试能力。

## 后续扩展规则

1. Browser 支持继续以“自动降级、用户无感知”为目标，不新增公开 API。
2. 先让用户继续调用现有 `UseDesktopControls()`，由内部 provider/token 根据平台选择。
3. 每新增一个 Browser 可用控件，优先只把该控件需要的 token/theme 加到 Browser provider，不直接放开完整 Desktop provider。
4. 涉及 Window、WindowTitleBar、Dialog window host、Notifications native host、ImagePreviewer window host、文件系统枚举等能力时，必须走 `RuntimePlatform.Features` 能力判断。
5. Browser 验证必须同时跑 Browser build、Browser 实跑 smoke 和 Desktop build，防止降级分支影响 Desktop。
6. 如果 Browser 页面启动失败，先看当前端口 splash error；不要被旧端口 console 日志误导。

## 待继续处理

- `UseDesktopColorPicker()` 和 `UseDesktopDataGrid()` 尚未接入 Browser 降级路径。
- Browser Gallery 当前只接入 `AboutUsPage`、`ButtonShowCase`、`FloatButtonShowCase`、`PaletteShowCase` 和 `IconShowCase`，完整 Workspace / AppView / AppShell 还未实现。
- Tooltip / Popup 的 Browser 降级链路尚未打开；`FloatButtonShowCase` 中 Tooltip 属性本轮只保证不阻塞页面渲染。
- `BrowserDesktopControlThemesProvider` 的范围后续应随着 ShowCase 迁移逐步扩大，并保持最小可用集合。
- `EmbeddableControlRootTheme` 在 Browser 下的精确不兼容点后续可以进一步拆解；当前先通过 Browser provider 排除该资源，保证启动路径稳定。
