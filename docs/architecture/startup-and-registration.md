# 启动与注册链路

AtomUI 的启动链路分为平台默认配置和主题控件注册两部分。

## AppBuilder 默认配置

入口位于 `src/AtomUI.Core/AppBuilderExtensions.cs`：

```csharp
AppBuilder.Configure<App>()
          .UseReactiveUI()
          .UsePlatformDetect()
          .WithAtomUIDefaultOptions()
          .StartWithClassicDesktopLifetime(args);
```

`WithAtomUIDefaultOptions()` 当前设置：

- macOS Avalonia Native 渲染优先级：OpenGL、Metal、Software。
- X11 平台选项：`EnableDrawnDecorations = false`。
- 字体 fallback：`Microsoft YaHei`。

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

`UseAtomUI()` 会创建 `ThemeManagerBuilder`，设置默认语言和主题，执行用户传入的注册动作，然后构建 `ThemeManager`。

## ThemeManagerBuilder 收集内容

`ThemeManagerBuilder` 在构建前收集以下内容：

- `ControlDesignTokens`：控件 Token 类型。
- `ControlThemesProviders`：AXAML 主题 Provider。
- `ThemeAssetPathProviders`：自定义主题资源路径 Provider。
- `LanguageProviders`：本地化资源 Provider。
- `InitializedHandlers`：`ThemeManager.NotifyInitialized()` 后执行的初始化回调。
- `ThemeVariantCalculatorFactory`：可选主题算法工厂。

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

- `ControlTokenTypePool.GetTokenTypes()`：返回当前项目内的控件 Token 类型。
- `LanguageProviderPool.GetLanguageProviders()`：返回当前项目内的语言 Provider。
- Token 资源键常量：供 AXAML 和 C# 使用。

因此新增控件 Token 或语言 Provider 时，需要确认对应 Attribute 正确，生成文件才会进入注册链路。

