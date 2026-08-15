# 启动与注册链路

AtomUI 的启动链路分为平台默认配置和主题控件注册两部分。

## AppBuilder 默认配置

入口位于 `src/AtomUI.Core/AppBuilderExtensions.cs`：

```csharp
AppBuilder.Configure<App>()
          .UseReactiveUI()
          .UseAtomUIPlatformDetect()
          .WithAtomUIDefaultOptions()
          .StartWithClassicDesktopLifetime(args);
```

`UseAtomUIPlatformDetect()` 位于 `AtomUI.Desktop.Controls`。Linux 下选择顺序为：显式
`AtomUIWindowingPlatform`、`ATOMUI_WINDOWING_PLATFORM`、非空 `WAYLAND_DISPLAY`、X11/XWayland。
不要只根据 `XDG_SESSION_TYPE` 选择 Wayland；headless/framebuffer 应用应直接配置自己的后端。
选择 Wayland 时，该入口会在创建首个 toplevel 前设置 `ForceDrawnDecorations = true` 并禁用服务端装饰协商，确保
AtomUI Window 从首帧开始使用 CSD，不能先显示 compositor 标题栏再切换为 AtomUI 标题栏。

`WithAtomUIDefaultOptions()` 当前设置：

- Windows 10：`AngleEgl/Software` 渲染回退与 `RedirectionSurface` 合成。
- Windows 11+：`AngleEgl/Software` 渲染回退与 `RedirectionSurface` 合成。
- macOS Avalonia Native 渲染优先级：OpenGL、Metal、Software。
- X11 平台选项：`EnableDrawnDecorations = true`。
- 字体 fallback：`Microsoft YaHei`。

Windows 选项通过公开 `Win32PlatformOptions` 强类型配置，不使用运行时反射。详细边界见
[Windows live resize 与窗口装饰架构](../systems/windowing/windows-live-resize.md)。

这一步只配置 Avalonia 平台选项，不注册 AtomUI 控件主题。

## Application 注册 AtomUI

入口位于 `src/AtomUI.Core/ApplicationExtensions.cs`：

```csharp
public override void Initialize()
{
    AvaloniaXamlLoader.Load(this);

    this.UseAtomUI(builder =>
    {
        builder.UseLanguages(
            LanguageTags.EnUS,
            [LanguageTags.EnUS, LanguageTags.ZhCN, LanguageTags.ZhTW]);
        builder.WithInitialTheme(IThemeManager.DEFAULT_THEME_ID);
        builder.UseAlibabaSansFont();
        builder.UseDesktopControls();
        builder.UseDesktopColorPicker();
        builder.UseDesktopDataGrid();
    });
}
```

`UseAtomUI()` 会创建根 `IAtomUIBuilder` 及相互独立的 `LocalizationBuilder`、`ThemeManagerBuilder`。应用生成的
本地化 bootstrap 先注册应用 Catalog 和外部语言包 Bundle，随后执行用户注册动作；最后分别构建全部支持语言
Snapshot 与主题 schema、ControlTheme asset manifest、首个 ThemeSnapshot、Root ThemeContext 和唯一 ThemeManager。

构建后的主题运行流见 [AtomUI 主题系统](../systems/theming/runtime.md)。简化顺序是：

1. 本地化 Builder 冻结 Catalog/Bundle Registry，为全部支持语言生成完整 Snapshot，并以默认语言初始化稳定
   `LanguageResourceProvider`、`ILanguageManager` 与 `ILocalizer`。
2. 主题 Builder 解析 Application Id，收集生成式 Control descriptor、ControlTheme asset manifest、
   `IThemeDefinitionResolver`、算法 descriptor 和不可变初始 `ThemeRequest` 模板。
3. `ThemeSchemaRegistry` 构建并冻结 exact CLR type/identity、完整 Global Token schema、Control Own Token schema、
   资产 owner/引用 identity 和 Semantic Part 契约。无效 descriptor、重复 type/identity、未知 Token、资产
   URI/identity 或 Semantic Part Theme 冲突在此失败。
4. `ThemeCatalog` 执行内置、应用资源及可选用户目录 Resolver，并通过统一 Reader 和 Binder 生成 typed theme
   definition。静态来源失败终止启动；用户来源失败时使用静态 Catalog 启动并保留 diagnostics。
5. FollowSystem 在编译前解析初始系统 appearance，并选择完整的 Light/Dark request 模板。
6. `ThemeCompiler` 在 ThemeManager 挂载前同步生成首个不可变 `ThemeSnapshot`。
7. Root ThemeContext 和唯一的 `ThemeTokenResourceProvider` 使用该 snapshot 初始化；ThemeManager 同时准备向
   每个 TopLevel 注入 Root ThemeContext 的全局 style。
8. ThemeManager 与本地化 Provider 以完整资源状态挂载到 Application，显式设置匹配 snapshot 的 Avalonia Light/Dark variant，
   并且只发布一次初始 Token 资源通知。
9. 后续主题更新由 ThemeManager 的五阶段 `ThemeTransaction` 提交；语言更新由 `LanguageManager` 独立交换预构建 Snapshot。

如果应用需要首帧就是暗色或紧凑主题，应在 builder 阶段通过 `ThemeConfig` 配置初始主题算法，而不是在
`UseAtomUI()` 之后调用运行期切换 API：

```csharp
this.UseAtomUI(builder =>
{
    builder.WithInitialTheme(
        IThemeManager.DEFAULT_THEME_ID,
        new ThemeConfigBuilder()
            .WithAlgorithms(ThemeAlgorithm.Default, ThemeAlgorithm.Dark)
            .Build());
    builder.UseDesktopControls();
});
```

`ThemeConfigBuilder` 只用于一次性构造并在 `Build()` 时防御性复制；传入 Manager 或
`ThemeConfigProvider.Config` 的 `ThemeConfig` 及其集合均不可变。运行时更新必须替换完整 Config，不能修改已
提交对象中的 Algorithms、Tokens 或 Controls 集合。

应用启动后的主题变化使用 `IThemeManager.ApplyThemeAsync(ThemeRequest)`。AtomUI 的主题 id 和 Compact
算法不编码进 Avalonia `ThemeVariant`；运行时只根据已提交 snapshot 设置 Avalonia Light 或 Dark。

应用可以显式启用用户主题目录：

```csharp
this.UseAtomUI(builder =>
{
    builder.WithApplicationId("AtomUIGallery");
    builder.UseUserThemeDirectory();
});
```

未显式设置 Application Id 时，默认使用具体 Application 类型所在程序集的简单名称；`Application.Name` 不作为
目录身份。默认目录为 `Environment.SpecialFolder.ApplicationData/{ApplicationId}/Themes`。运行时调用
`IThemeManager.ReloadThemesAsync()` 手动刷新；主题系统不使用 `FileSystemWatcher` 自动监听。

局部主题由继承 `ThemeVariantScope` 的 `ThemeConfigProvider` 建立。Provider 首次 attach 在内容可见前同步创建
稳定 ThemeContext 和唯一 ResourceProvider；后续 Config 替换由同一个 ThemeManager 事务化处理。普通
Popup/Flyout 通过逻辑树自然继承，独立 Window/Dialog/Notification TopLevel 必须从显式 owner 获得
`ThemeContextLease` 和宿主私有 ResourceBridge；无 owner 静态 API 使用根主题。

## ThemeManagerBuilder 收集内容

`ThemeManagerBuilder` 在构建前收集以下内容：

- `ControlTokenDescriptors`：每个对外可主题化 Control 的 exact CLR type、生成式 identity、可选 Own Token schema、
  强类型构造和资源投影；没有 Own Token 的 Control 也必须注册 descriptor。
- `ControlThemeAssetManifests`：生成式资产 URI、owner identity、引用的 Control identities、Semantic Part Theme
  契约和构建期校验结果。
- `ControlThemesProviders`：AXAML 主题 Provider。
- `ThemeDefinitionResolvers`：内置资源、应用 `avares://` 资源和可选用户配置目录的统一主题来源解析器。
- `ThemeAlgorithmDescriptors`：`ThemeAlgorithm.Default`、`Dark`、`Compact` 的生成式 descriptor。
- `InitialThemeRequests`：固定主题或 FollowSystem 的 Light/Dark 不可变 root request 模板。
- `ModuleInitializers`：与 ThemeLoaded 等主题生命周期无关的模块初始化回调。

构建时会把这些内容注册到 `ThemeManager`。

## 控件包注册顺序

当前实现中，`UseDesktopControls()` 使用全量兼容注册，顺序很关键：

1. 先调用 `UseCommonControls()`，分别向 Theme Builder 注册公共 Token/主题，向 Localization Builder 注册公共 Catalog 和内置 Bundle。
2. 再注册 `AtomUI.Desktop.Controls` 的 Token。
3. 根据是否支持 Native Window 选择 `DesktopControlThemesProvider` 或 `BrowserDesktopControlThemesProvider`。
4. 调用生成的 `GeneratedLanguageModuleRegistration` 注册桌面控件包 Catalog 和内置 Bundle。
5. 注册初始化回调，包括自定义动画器、桌面 Tooltip 服务、媒体断点主题引导。

DataGrid 和 ColorPicker 独立包通过 `UseDesktopDataGrid()`、`UseDesktopColorPicker()` 追加自己的 Token、主题 Provider 和语言资源。

linked publish 中，应用仍调用 `UseDesktopControls()`，但 `PublishTrimmed=true`、`PublishAot=true` 或 WebAssembly
`RunAOTCompilation=true` 会在编译期切换到生成式 Registration Unit 计划。Desktop 仍完整注册 Common，并保持
Dialog input capture、Provider、完整 Language Module 和 Theme initializer 的既有顺序；只有 Desktop 的 Control
descriptor、Own Token schema 和控件族专属 AXAML Theme 按 Unit 保留。无法可靠确定 Unit 时，仅把对应 Package
扩大为 full fallback。普通非裁剪构建继续执行上述全量顺序。完整契约见
[AOT 与裁剪架构](aot-and-trimming.md)。

## 源生成池

当前 Control 包不手工维护完整 Token、主题资产或 Language 列表，而是依赖 `AtomUI.Generator` 生成：

- `ControlTokenDescriptorPool.GetDescriptors()`：返回当前项目内全部对外可主题化 Control 的 descriptor，包括零
  Own Token 的 Control。
- `ControlThemeAssetManifest.GetDescriptors()`：返回通过构建校验的 ControlTheme asset、owner 和引用 identity descriptor。
- `GeneratedLanguageModuleRegistration.Register()`：显式注册当前项目的 Catalog descriptor 和内置 Translation Bundle。
- `XxxTokens.Identity`、强类型 `XxxTokenKey` 和 `XxxTokenResourceExtension`：供 AXAML 和 C# 使用。
- 包级 `UseXxxControls()` 注册入口：分别向 `builder.Theme` 和 `builder.Localization` 注册当前包的主题资产与本地化模块。

Builder 必须原样注册包含 exact CLR type 与 identity 的 descriptor 和 manifest，不能退化成只传递其中一项或
运行时扫描 AXAML。因此新增控件
时只需遵循 Control、无参数 `[ControlDesignToken]` 标记的可选 Own Token 类型和 `Themes/**/*.axaml` 约定；不编写
泛型 Token Attribute、Theme Asset glob、
手工 manifest 或逐 Theme 注册代码。生成器负责检查 descriptor、Own/Global 名称冲突、强类型资源键、owner
identity 和注册池是否一致；Global Token 消费关系不进入注册数据。

linked registration 会把同一控件族的 Control descriptor、Own Token schema、内部控件和专属 Theme Asset factory
聚合成 linker 可独立删除的 Registration Unit；Language Catalog、内置 Bundle、Package 初始化逻辑和显式共享资源
不拆分。全量池只由普通兼容路径或该 Package 的 full fallback 调用。Theme Builder 最终仍按包接收一个完整且自洽的
`ControlPackageRegistration`，不会改成控件实例化时追加注册。
