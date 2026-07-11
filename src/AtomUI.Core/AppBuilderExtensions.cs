using System.Runtime.Versioning;
using Avalonia;
using Avalonia.Media;

namespace AtomUI;

/// <summary>
/// AtomUI 对 <see cref="AppBuilder"/> 的扩展。
/// </summary>
public static class AppBuilderExtensions
{
    /// <summary>
    /// 应用 AtomUI 推荐的平台默认配置。主要用途是规避当前 Avalonia 上游尚未修复、
    /// 对 AtomUI 体验影响较大的平台问题，让框架使用者默认就拿到最佳表现，无需自行配置。
    /// </summary>
    /// <remarks>
    /// 目前包含：
    /// <list type="bullet">
    ///   <item>
    ///     <description>
    ///     <b>Windows</b>：<c>Win32PlatformOptions.CompositionMode</c> 保持
    ///     WinUIComposition / DirectComposition 优先，确保 Tooltip、Popup 等透明 PopupRoot
    ///     可以使用独立 Window 模式；普通 AtomUI 窗口由 Windows 专用模板完整绘制 opaque 背景，
    ///     避免 resize 时露出未绘制区域。
    ///     </description>
    ///   </item>
    ///   <item>
    ///     <description>
    ///     <b>macOS</b>：<see cref="AvaloniaNativePlatformOptions.RenderingMode"/> 把
    ///     <see cref="AvaloniaNativeRenderingMode.OpenGl"/> 排在 <see cref="AvaloniaNativeRenderingMode.Metal"/>
    ///     之前。Avalonia 12 的 Metal 后端在 live resize 时会让窗体内容可见抖动，
    ///     切换到 OpenGL 渲染路径可完全规避。上游修复后可移除此默认。
    ///     </description>
    ///   </item>
    ///   <item>
    ///     <description>
    ///     <b>Linux</b>：<see cref="X11PlatformOptions.EnableDrawnDecorations"/> = <c>true</c>，
    ///     AtomUI 自绘窗体装饰 (CSD) 需要此开关支撑。
    ///     </description>
    ///   </item>
    /// </list>
    /// 调用者在此基础上仍可继续 <c>.With(...)</c> 覆盖或扩展选项。
    /// </remarks>
    /// <example>
    /// <code>
    /// BuildAvaloniaApp()
    ///     .UseAtomUIPlatformDetect()
    ///     .WithAtomUIDefaultOptions()
    ///     .StartWithClassicDesktopLifetime(args);
    /// </code>
    /// </example>
    public static AppBuilder WithAtomUIDefaultOptions(this AppBuilder appBuilder)
    {
        if (OperatingSystem.IsWindows())
        {
            appBuilder = appBuilder.WithWin32TransparentPopupCompositionOptions();
        }

        return appBuilder
            .With(new AvaloniaNativePlatformOptions
            {
                RenderingMode =
                [
                    AvaloniaNativeRenderingMode.OpenGl,
                    AvaloniaNativeRenderingMode.Metal,
                    AvaloniaNativeRenderingMode.Software
                ]
            })
            .With(new X11PlatformOptions
            {
                EnableDrawnDecorations = true
            })
            .With(new FontManagerOptions
            {
                FontFallbacks = [new FontFallback
                {
                    FontFamily = new FontFamily("Microsoft YaHei")
                }]
            });
    }

    [SupportedOSPlatform("windows")]
    private static AppBuilder WithWin32TransparentPopupCompositionOptions(this AppBuilder appBuilder)
    {
        var win32OptionsType = Type.GetType("Avalonia.Win32PlatformOptions, Avalonia.Win32");
        var renderingModeType = Type.GetType("Avalonia.Win32RenderingMode, Avalonia.Win32");
        var compositionModeType = Type.GetType("Avalonia.Win32CompositionMode, Avalonia.Win32");
        if (win32OptionsType is null || renderingModeType is null || compositionModeType is null)
        {
            return appBuilder;
        }

        var options = Activator.CreateInstance(win32OptionsType);
        if (options is null)
        {
            return appBuilder;
        }

        win32OptionsType.GetProperty("RenderingMode")?.SetValue(options,
            CreateEnumArray(renderingModeType, "AngleEgl", "Software"));
        win32OptionsType.GetProperty("CompositionMode")?.SetValue(options,
            CreateEnumArray(
                compositionModeType,
                "WinUIComposition",
                "DirectComposition",
                "RedirectionSurface"));

        var withMethod = typeof(AppBuilder).GetMethods()
                                           .Single(method =>
                                           {
                                               if (method.Name != nameof(AppBuilder.With) ||
                                                   !method.IsGenericMethodDefinition)
                                               {
                                                   return false;
                                               }

                                               var parameters = method.GetParameters();
                                               return parameters.Length == 1 &&
                                                      parameters[0].ParameterType.IsGenericParameter;
                                           });
        withMethod.MakeGenericMethod(win32OptionsType).Invoke(appBuilder, [options]);
        return appBuilder;
    }

    private static Array CreateEnumArray(Type enumType, params string[] names)
    {
        var values = Array.CreateInstance(enumType, names.Length);
        for (var i = 0; i < names.Length; i++)
        {
            values.SetValue(Enum.Parse(enumType, names[i]), i);
        }
        return values;
    }
}
