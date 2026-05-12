using AtomUI.Controls;
using AtomUI.Theme;
using Avalonia.Styling;

namespace AtomUI.Desktop.Controls;

/// <summary>
/// 在 Window 的 ControlTheme 里程序化注入媒体断点 ContainerQuery。
///
/// 断点数值来自当前 Theme 的 Alias Token (ScreenSMMin/ScreenSMMax 等),使得开发者
/// 通过主题配置文件覆盖 Alias Token 就能改断点,不需要动 axaml 里的字面量。
/// 订阅 ThemeManager.ThemeLoaded,每次 theme 加载后重建 CQ。
///
/// 注意:CQ 必须按从大到小的顺序注入 (ExtraExtraLarge → ExtraSmall)。
/// Avalonia 在跨断点 resize 过程中 activator 的评估顺序与声明顺序相关,
/// 经实测只有这个顺序下所有方向的断点切换都能正确 fire。
/// </summary>
internal static class MediaBreakPointThemeBootstrapper
{
    private static readonly List<ContainerQuery> InjectedQueries = new();
    private static ControlTheme? _windowTheme;
    private static ThemeManager? _themeManager;

    public static void Attach(ThemeManager themeManager)
    {
        if (_themeManager != null)
        {
            return;
        }

        _themeManager              =  themeManager;
        themeManager.ThemeLoaded   += HandleThemeLoaded;
    }

    private static void HandleThemeLoaded(object? sender, ThemeOperateEventArgs args)
    {
        if (sender is not ThemeManager tm || args.Theme == null)
        {
            return;
        }

        Rebuild(tm, args.Theme);
    }

    private static void Rebuild(ThemeManager themeManager, ITheme theme)
    {
        if (_windowTheme == null)
        {
            if (!themeManager.TryGetResource(typeof(Window), null, out var resource) ||
                resource is not ControlTheme controlTheme)
            {
                return;
            }
            _windowTheme = controlTheme;
        }

        var children = _windowTheme.Children;
        foreach (var cq in InjectedQueries)
        {
            children.Remove(cq);
        }
        InjectedQueries.Clear();

        var token = theme.SharedToken;

        var rows = new (double? Min, double? Max, MediaBreakPoint Bp)[]
        {
            (token.ScreenXXLMin,    (double?)null,         MediaBreakPoint.ExtraExtraLarge),
            (token.ScreenXLMin,     token.ScreenXLMax,     MediaBreakPoint.ExtraLarge),
            (token.ScreenLGMin,     token.ScreenLGMax,     MediaBreakPoint.Large),
            (token.ScreenMDMin,     token.ScreenMDMax,     MediaBreakPoint.Medium),
            (token.ScreenSMMin,     token.ScreenSMMax,     MediaBreakPoint.Small),
            (null,                  token.ScreenSMMin - 1, MediaBreakPoint.ExtraSmall),
        };

        foreach (var (min, max, bp) in rows)
        {
            var cq = BuildContainerQuery(min, max, bp);
            children.Add(cq);
            InjectedQueries.Add(cq);
        }
    }

    private static ContainerQuery BuildContainerQuery(double? min, double? max, MediaBreakPoint bp)
    {
        StyleQuery? query = null;
        if (min.HasValue)
        {
            query = query.Width(StyleQueryComparisonOperator.GreaterThanOrEquals, min.Value);
        }
        if (max.HasValue)
        {
            query = query.Width(StyleQueryComparisonOperator.LessThanOrEquals, max.Value);
        }

        var cq = new ContainerQuery
        {
            Name  = IMediaBreakAwareControl.GlobalQueryContainerName,
            Query = query
        };

        var style = new Style(s => s.OfType<WindowMediaQueryIndicator>().Name(WindowMediaQueryIndicator.MediaQueryIndicatorName));
        style.Setters.Add(new Setter(WindowMediaQueryIndicator.MediaBreakPointProperty, bp));
        cq.Children.Add(style);

        return cq;
    }
}
