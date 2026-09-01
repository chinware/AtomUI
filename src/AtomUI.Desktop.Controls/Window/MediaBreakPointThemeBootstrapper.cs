using AtomUI.Controls;
using AtomUI.Theme;
using AtomUI.Theme.Compilation;
using AtomUI.Theme.Resources;
using Avalonia.Styling;

namespace AtomUI.Desktop.Controls;

/// <summary>
/// 在 Window 的 ControlTheme 里程序化注入媒体断点 ContainerQuery。
///
/// 断点数值来自当前 Theme 的 Alias Token (ScreenSMMin/ScreenSMMax 等),使得开发者
/// 通过主题配置文件覆盖 Alias Token 就能改断点,不需要动 axaml 里的字面量。
/// 订阅 ThemeManager.ThemeChanged,每次 committed theme 切换后重建 CQ。
///
/// 注意:CQ 必须按从大到小的顺序注入 (ExtraExtraExtraLarge → ExtraSmall)。
/// Avalonia 在跨断点 resize 过程中 activator 的评估顺序与声明顺序相关,
/// 经实测只有这个顺序下所有方向的断点切换都能正确 fire。
/// </summary>
internal static class MediaBreakPointThemeBootstrapper
{
    // 注意:ContainerQuery.SetParent 只允许 ControlTheme 作为 parent,不接受 null,
    // 意味着一旦加入 ControlTheme.Children 就无法 Remove。因此首次加载后缓存 CQ,
    // 后续主题切换只就地更新 Query (断点阈值),不再增删节点。
    private static readonly List<ContainerQuery> InjectedQueries = new();
    private static ControlTheme? s_windowTheme;
    private static ThemeManager? s_themeManager;

    private static readonly (Func<ThemeSnapshot, double?> Min, Func<ThemeSnapshot, double?> Max, MediaBreakPoint Bp)[] Rows =
    {
        (t => Global(t, SharedTokenKind.ScreenXXXLMin), _ => null, MediaBreakPoint.ExtraExtraExtraLarge),
        (t => Global(t, SharedTokenKind.ScreenXXLMin), t => Global(t, SharedTokenKind.ScreenXXLMax), MediaBreakPoint.ExtraExtraLarge),
        (t => Global(t, SharedTokenKind.ScreenXLMin), t => Global(t, SharedTokenKind.ScreenXLMax), MediaBreakPoint.ExtraLarge),
        (t => Global(t, SharedTokenKind.ScreenLGMin), t => Global(t, SharedTokenKind.ScreenLGMax), MediaBreakPoint.Large),
        (t => Global(t, SharedTokenKind.ScreenMDMin), t => Global(t, SharedTokenKind.ScreenMDMax), MediaBreakPoint.Medium),
        (t => Global(t, SharedTokenKind.ScreenSMMin), t => Global(t, SharedTokenKind.ScreenSMMax), MediaBreakPoint.Small),
        (_ => null, t => Global(t, SharedTokenKind.ScreenSMMin) - 1, MediaBreakPoint.ExtraSmall),
    };

    public static void Attach(ThemeManager themeManager)
    {
        if (s_themeManager != null)
        {
            return;
        }

        s_themeManager              =  themeManager;
        themeManager.ThemeChanged  += HandleThemeChanged;
        if (themeManager.CurrentSnapshot is { } snapshot)
        {
            Rebuild(themeManager, snapshot);
        }
    }

    private static void HandleThemeChanged(object? sender, ThemeChangedEventArgs args)
    {
        if (sender is not ThemeManager tm)
        {
            return;
        }

        if (tm.CurrentSnapshot is { } snapshot)
        {
            Rebuild(tm, snapshot);
        }
    }

    private static void Rebuild(ThemeManager themeManager, ThemeSnapshot snapshot)
    {
        if (s_windowTheme == null)
        {
            if (!themeManager.TryGetResource(typeof(Window), null, out var resource) ||
                resource is not ControlTheme controlTheme)
            {
                return;
            }
            s_windowTheme = controlTheme;
        }

        if (InjectedQueries.Count == 0)
        {
            var children = s_windowTheme.Children;
            foreach (var (minFn, maxFn, bp) in Rows)
            {
                var cq = BuildContainerQuery(minFn(snapshot), maxFn(snapshot), bp);
                children.Add(cq);
                InjectedQueries.Add(cq);
            }
        }
        else
        {
            for (var i = 0; i < Rows.Length; i++)
            {
                var (minFn, maxFn, _) = Rows[i];
                InjectedQueries[i].Query = BuildStyleQuery(minFn(snapshot), maxFn(snapshot));
            }
        }
    }

    private static int Global(ThemeSnapshot snapshot, SharedTokenKind token)
    {
        return snapshot.GlobalTokenValues.Get<int>((int)token);
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
