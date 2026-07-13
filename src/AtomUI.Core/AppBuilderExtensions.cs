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
    ///     <b>Windows</b>：Windows 10 使用 RedirectionSurface，避免 WinUIComposition 在
    ///     live resize 期间提交与 HWND 尺寸不同步的表面；Windows 11 保持 WinUIComposition /
    ///     DirectComposition 优先，确保透明 PopupRoot 可以使用独立 Window 模式。
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
            appBuilder = WindowsAppBuilderDefaults.Apply(appBuilder);
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
}
