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
    ///     <b>Windows</b>：Windows 10 和 Windows 11 按 Avalonia 的合成回退顺序使用 WinUIComposition、
    ///     DirectComposition、LowLatencyDxgiSwapChain 和 RedirectionSurface；CSD 窗口保留主题背景配置。
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
        // 按目标平台分支设置平台选项。NativeAOT 在 RID 发布时会把 OperatingSystem.IsXxx()
        // 折叠为常量，非目标平台的分支及其引用的平台选项类型会被整体裁剪。
        if (OperatingSystem.IsMacOS())
        {
            appBuilder = appBuilder.With(new AvaloniaNativePlatformOptions
            {
                RenderingMode =
                [
                    AvaloniaNativeRenderingMode.OpenGl,
                    AvaloniaNativeRenderingMode.Metal,
                    AvaloniaNativeRenderingMode.Software
                ]
            });
        }
        if (OperatingSystem.IsWindows())
        {
            appBuilder = appBuilder.With(new Win32PlatformOptions
            {
                RenderingMode =
                [
                    Win32RenderingMode.AngleEgl,
                    Win32RenderingMode.Software
                ],
                CompositionMode =
                [
                    Win32CompositionMode.WinUIComposition,
                    Win32CompositionMode.DirectComposition,
                    Win32CompositionMode.LowLatencyDxgiSwapChain,
                    Win32CompositionMode.RedirectionSurface
                ]
            });
        }
        if (OperatingSystem.IsLinux())
        {
            appBuilder = appBuilder.With(new X11PlatformOptions
            {
                EnableDrawnDecorations = true
            });
        }
        return appBuilder
            .With(new FontManagerOptions
            {
                FontFallbacks = [new FontFallback
                {
                    FontFamily = new FontFamily("Microsoft YaHei")
                }]
            });
    }
}
