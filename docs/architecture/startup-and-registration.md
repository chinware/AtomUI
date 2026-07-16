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

`WithAtomUIDefaultOptions()` 当前设置：

- Windows 10：`AngleEgl/Software` 渲染回退与 `RedirectionSurface` 合成。
- Windows 11+：`AngleEgl/Software` 渲染回退与 `RedirectionSurface` 合成。
- macOS Avalonia Native 渲染优先级：OpenGL、Metal、Software。
- X11 平台选项：`EnableDrawnDecorations = true`。
- 字体 fallback：`Microsoft YaHei`。

Windows 选项通过公开 `Win32PlatformOptions` 强类型配置，不使用运行时反射。详细边界见
[Windows live resize 与窗口装饰架构](../modules/native/windows-live-resize-scheme.md)。

这一步只配置 Avalonia 平台选项，不注册 AtomUI 控件主题。

## Application 注册 AtomUI

入口位于 `src/AtomUI.Core/ApplicationExtensions.cs`：

```csharp
public override void Initialize()
{
    AvaloniaXamlLoader.Load(this);

    this.UseAtomUI(builder =>
    {
        builder.WithDefaultCultureInfo(CultureInfo.CurrentUICulture);
        builder.WithDefaultTheme(IThemeManager.DEFAULT_THEME_ID);
        builder.UseAlibabaSansFont();
        builder.UseDesktopControls();
        builder.UseDesktopColorPicker();
        builder.UseDesktopDataGrid();
    });
}
```

`UseAtomUI()` 会创建 `ThemeManagerBuilder`，设置默认语言和初始主题请求，执行用户传入的注册动作，然后
构建主题 schema、首个 snapshot、ThemeEngine 和 ThemeManager。

构建后的主题运行流见 [AtomUI.Core 主题系统](../modules/core/theme-system.md)。简化顺序是：

1. Builder 收集生成式 Control descriptor、主题 Provider、算法 descriptor、语言和初始 `ThemeConfig`。
2. `ThemeSchemaRegistry` 在读取主题文件前完成构建并拒绝无效注册。
3. `ThemeCatalog` 通过 Reader 和 Binder 生成 typed theme definition。
4. `ThemeCompiler` 在 ThemeManager 挂载前同步生成首个不可变 `ThemeSnapshot`。
5. Root ThemeContext 和唯一的 `ThemeTokenResourceProvider` 使用该 snapshot 初始化。
6. ThemeManager 挂载到 Application 后设置 Avalonia Light/Dark variant，并且只发布一次资源通知。
7. 后续全局和局部更新统一由 ThemeEngine 准备和事务提交。

如果应用需要首帧就是暗色或紧凑主题，应在 builder 阶段通过 `ThemeConfig` 配置初始主题算法，而不是在
`UseAtomUI()` 之后调用运行期切换 API：

```csharp
this.UseAtomUI(builder =>
{
    builder.WithInitialTheme(
        IThemeManager.DEFAULT_THEME_ID,
        new ThemeConfig
        {
            Algorithms = { ThemeAlgorithms.Default, ThemeAlgorithms.Dark }
        });
    builder.UseDesktopControls();
});
```

应用启动后的主题变化使用 `IThemeManager.ApplyThemeAsync(ThemeRequest)`。AtomUI 的主题 id 和 Compact
算法不编码进 Avalonia `ThemeVariant`；运行时只根据已提交 snapshot 设置 Avalonia Light 或 Dark。

## ThemeManagerBuilder 收集内容

`ThemeManagerBuilder` 在构建前收集以下内容：

- `ControlTokenDescriptors`：生成式 Control identity、Token schema、强类型构造和资源投影。
- `ControlThemesProviders`：AXAML 主题 Provider。
- `ThemeAssetPathProviders`：自定义主题资源路径 Provider。
- `LanguageProviders`：本地化资源 Provider。
- `ThemeAlgorithmDescriptors`：默认、暗色、紧凑和自定义算法。
- `InitialThemeRequest`：首帧使用的主题 id 和 `ThemeConfig`。
- `ModuleInitializers`：与 ThemeLoaded 等主题生命周期无关的模块初始化回调。

构建时会把这些内容注册到 `ThemeManager`。

## 控件包注册顺序

`UseDesktopControls()` 的注册顺序很关键：

1. 先调用 `UseCommonControls()`，注册 `AtomUI.Controls` 的公共 Token、公共主题和语言资源。
2. 再注册 `AtomUI.Desktop.Controls` 的 Token。
3. 根据是否支持 Native Window 选择 `DesktopControlThemesProvider` 或 `BrowserDesktopControlThemesProvider`。
4. 注册桌面控件包语言资源。
5. 注册初始化回调，包括自定义动画器、桌面 Tooltip 服务、媒体断点主题引导。

DataGrid 和 ColorPicker 独立包通过 `UseDesktopDataGrid()`、`UseDesktopColorPicker()` 追加自己的 Token、主题 Provider 和语言资源。

## 源生成池

控件包不手工维护完整 Token/Language 列表，而是依赖 `AtomUI.Generator` 生成：

- `ControlTokenDescriptorPool.GetDescriptors()`：返回当前项目内完整的 Control Token descriptor。
- `LanguageProviderPool.GetLanguageProviders()`：返回当前项目内的语言 Provider。
- Token 资源键常量：供 AXAML 和 C# 使用。

Builder 必须原样注册 descriptor，不能退化成只传递 `Type`。因此新增控件 Token 或语言 Provider 时，
需要确认对应 Attribute 正确，并检查生成 descriptor、资源键和注册池是否一致。
