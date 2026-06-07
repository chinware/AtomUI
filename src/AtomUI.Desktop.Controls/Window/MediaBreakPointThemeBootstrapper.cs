using AtomUI.Controls;
using AtomUI.Theme;
using AtomUI.Theme.TokenSystem;
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
    // 注意:ContainerQuery.SetParent 只允许 ControlTheme 作为 parent,不接受 null,
    // 意味着一旦加入 ControlTheme.Children 就无法 Remove。因此首次加载后缓存 CQ,
    // 后续主题切换只就地更新 Query (断点阈值),不再增删节点。
    private static readonly List<ContainerQuery> InjectedQueries = new();
    private static ControlTheme? _windowTheme;
    private static ThemeManager? _themeManager;

    private static readonly (Func<DesignToken, double?> Min, Func<DesignToken, double?> Max, MediaBreakPoint Bp)[] Rows =
    {
        (t => t.ScreenXXLMin, _ => null,                 MediaBreakPoint.ExtraExtraLarge),
        (t => t.ScreenXLMin,  t => t.ScreenXLMax,        MediaBreakPoint.ExtraLarge),
        (t => t.ScreenLGMin,  t => t.ScreenLGMax,        MediaBreakPoint.Large),
        (t => t.ScreenMDMin,  t => t.ScreenMDMax,        MediaBreakPoint.Medium),
        (t => t.ScreenSMMin,  t => t.ScreenSMMax,        MediaBreakPoint.Small),
        (_ => null,           t => t.ScreenSMMin - 1,    MediaBreakPoint.ExtraSmall),
    };

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

        var token = theme.SharedToken;

        if (InjectedQueries.Count == 0)
        {
            var children = _windowTheme.Children;
            foreach (var (minFn, maxFn, bp) in Rows)
            {
                var cq = BuildContainerQuery(minFn(token), maxFn(token), bp);
                children.Add(cq);
                InjectedQueries.Add(cq);
            }
        }
        else
        {
            for (var i = 0; i < Rows.Length; i++)
            {
                var (minFn, maxFn, _) = Rows[i];
                InjectedQueries[i].Query = BuildStyleQuery(minFn(token), maxFn(token));
            }
        }
    }

    private static StyleQuery? BuildStyleQuery(double? min, double? max)
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
        return query;
    }

    private static ContainerQuery BuildContainerQuery(double? min, double? max, MediaBreakPoint bp)
    {
        var cq = new ContainerQuery
        {
            Name  = IMediaBreakAwareControl.GlobalQueryContainerName,
            Query = BuildStyleQuery(min, max)
        };

        var style = new Style(s => s.OfType<MediaBreakPointIndicator>().Name(MediaBreakPointIndicator.MediaQueryIndicatorName));
        style.Setters.Add(new Setter(MediaBreakPointIndicator.MediaBreakPointProperty, bp));
        cq.Children.Add(style);

        return cq;
    }
}
