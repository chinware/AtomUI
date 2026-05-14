using System.Diagnostics;
using System.Globalization;
using System.Reactive.Linq;
using System.Text;
using System.Xml.Linq;
using AtomUI;
using AtomUI.Controls;
using AtomUI.Desktop.Controls;
using AtomUIGallery.Desktop;
using AtomUIGallery.ShowCases;
using AtomUIGallery.ShowCases.ViewModels;
using AtomUIGallery.Workspace.Views;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Headless;
using Avalonia.Layout;
using Avalonia.LogicalTree;
using Avalonia.Threading;
using Avalonia.VisualTree;
using ReactiveUI.Avalonia;

namespace AtomUI.GalleryPerformance;

internal static class Program
{
    private static readonly Size WindowSize = new(1300, 900);
    private static readonly Rect WindowBounds = new(0, 0, WindowSize.Width, WindowSize.Height);
    private static readonly ShowCaseSpec AboutUs = new(
        "AboutUsPage",
        AboutUsViewModel.ID,
        "AtomUIGallery.ShowCases.Views.AboutUsPage",
        "controlgallery/AtomUIGallery/ShowCases/Views/General/AboutUsPage.axaml",
        stats => stats.VisualCount > 0);
    private static readonly IReadOnlyDictionary<string, ShowCaseSpec> ShowCases =
        new Dictionary<string, ShowCaseSpec>(StringComparer.OrdinalIgnoreCase)
        {
            ["lineedit"] = new(
                "LineEditShowCase",
                LineEditViewModel.ID,
                "AtomUIGallery.ShowCases.Views.LineEditShowCase",
                "controlgallery/AtomUIGallery/ShowCases/Views/DataEntry/LineEditShowCase.axaml",
                stats => stats.LineEditCount > 0),
            ["icon"] = new(
                "IconShowCase",
                IconViewModel.ID,
                "AtomUIGallery.ShowCases.Views.IconShowCase",
                "controlgallery/AtomUIGallery/ShowCases/Views/General/IconShowCase.axaml",
                stats => stats.IconCount > 0)
        };

    [STAThread]
    public static int Main(string[] args)
    {
        var options = PerfOptions.Parse(args);
        if (!ShowCases.TryGetValue(options.ShowCase, out var showCase))
        {
            Console.Error.WriteLine($"Unknown showcase '{options.ShowCase}'. Available: {string.Join(", ", ShowCases.Keys)}.");
            return 1;
        }

        try
        {
            SetupAvalonia(out var lifetime);
            if (lifetime.MainWindow is not WorkspaceWindow window)
            {
                Console.Error.WriteLine("Gallery workspace window was not created.");
                return 1;
            }

            window.ShowInTaskbar = false;
            window.Width         = WindowSize.Width;
            window.Height        = WindowSize.Height;
            window.Show();

            WaitForRoute(window, AboutUs, options.Timeout);

            var coldRun = MeasureNavigation(window, 0, "Cold", options, showCase);
            NavigateToAboutUs(window, options);

            for (var i = 0; i < options.Warmup; i++)
            {
                _ = MeasureNavigation(window, i + 1, "Warmup", options, showCase);
                NavigateToAboutUs(window, options);
            }

            var samples = new List<NavigationSample>(options.Iterations);
            for (var i = 0; i < options.Iterations; i++)
            {
                samples.Add(MeasureNavigation(window, i + 1, "Measured", options, showCase));
                NavigateToAboutUs(window, options);
            }

            var result = NavigationResult.Create(options.Label, coldRun, samples);
            var output = RenderResult(result, options, showCase);
            Console.WriteLine(output);

            if (!string.IsNullOrWhiteSpace(options.MarkdownOutputPath))
            {
                var fullPath = Path.GetFullPath(options.MarkdownOutputPath);
                Directory.CreateDirectory(Path.GetDirectoryName(fullPath)!);
                File.WriteAllText(fullPath, output, new UTF8Encoding(encoderShouldEmitUTF8Identifier: false));
                Console.WriteLine();
                Console.WriteLine($"Wrote markdown result: {fullPath}");
            }

            window.Close();
            Dispatcher.UIThread.RunJobs();
            return 0;
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine(ex);
            return 1;
        }
    }

    private static void SetupAvalonia(out ClassicDesktopStyleApplicationLifetime lifetime)
    {
        lifetime = new ClassicDesktopStyleApplicationLifetime
        {
            Args = []
        };

        AppBuilder.Configure<GalleryApplication>()
                  .UseReactiveUI(build =>
                      build.ConfigureViewLocator(locator => new ShowCaseViewModule().RegisterViews(locator)))
                  .UseHeadless(new AvaloniaHeadlessPlatformOptions())
                  .WithAtomUIDefaultOptions()
                  .SetupWithLifetime(lifetime);
    }

    private static NavigationSample MeasureNavigation(WorkspaceWindow window,
                                                       int iteration,
                                                       string phase,
                                                       PerfOptions options,
                                                       ShowCaseSpec showCase)
    {
        GC.Collect();
        GC.WaitForPendingFinalizers();
        GC.Collect();

        var allocatedBefore = GC.GetAllocatedBytesForCurrentThread();
        var stopwatch       = Stopwatch.StartNew();
        var trigger         = TriggerNavigation(window, showCase);
        var route           = WaitForRoute(window, showCase, options.Timeout);
        stopwatch.Stop();

        var allocatedBytes = GC.GetAllocatedBytesForCurrentThread() - allocatedBefore;
        var stats          = RouteStats.Collect(route);
        return new NavigationSample(iteration, phase, trigger, stopwatch.Elapsed, allocatedBytes, stats);
    }

    private static string TriggerNavigation(WorkspaceWindow window, ShowCaseSpec showCase)
    {
        var navMenu = window.GetSelfAndVisualDescendants().OfType<NavMenu>().FirstOrDefault();
        var navItem = window.GetSelfAndVisualDescendants()
                            .OfType<INavMenuItem>()
                            .FirstOrDefault(item => item.ItemKey.HasValue &&
                                                    item.ItemKey.Value == showCase.Key);

        if (navMenu is not null && navItem is not null)
        {
            navMenu.RaiseEvent(new NavMenuItemClickEventArgs(NavMenu.NavMenuItemClickEvent, navItem));
            return "NavMenuItemClick";
        }

        ExecuteNavigateCommand(window, showCase.Key);
        return "NavigateToCommand";
    }

    private static void NavigateToAboutUs(WorkspaceWindow window, PerfOptions options)
    {
        ExecuteNavigateCommand(window, AboutUsViewModel.ID);
        WaitForRoute(window, AboutUs, options.Timeout);
    }

    private static void ExecuteNavigateCommand(WorkspaceWindow window, EntityKey key)
    {
        var error = default(Exception);
        using var subscription = window.ViewModel!.CaseNavigation.NavigateToCommand
                                      .Execute(key)
                                      .Subscribe(_ => { }, ex => error = ex);
        if (error is not null)
        {
            throw error;
        }
    }

    private static Control WaitForRoute(WorkspaceWindow window, ShowCaseSpec showCase, TimeSpan timeout)
    {
        var stopwatch          = Stopwatch.StartNew();
        var stableLayoutPasses = 0;
        var previousStats      = default(RouteStats);
        Control? route         = null;

        while (stopwatch.Elapsed < timeout)
        {
            PumpLayout(window);

            route = window.GetSelfAndVisualDescendants()
                          .OfType<Control>()
                          .FirstOrDefault(control => control.GetType().FullName == showCase.RouteTypeName);

            if (route is not null && route.IsVisible && route.Bounds.Width > 0 && route.Bounds.Height > 0)
            {
                var currentStats = RouteStats.Collect(route);
                if (currentStats.IsDisplayReady(showCase) &&
                    previousStats is not null &&
                    currentStats.HasSameShape(previousStats))
                {
                    stableLayoutPasses++;
                    if (stableLayoutPasses >= 2)
                    {
                        return route;
                    }
                }
                else
                {
                    stableLayoutPasses = 0;
                    previousStats      = currentStats;
                }
            }
        }

        var routeLabel = route is null
            ? "route was not found"
            : $"route was found but did not stabilize, bounds={route.Bounds}";
        throw new TimeoutException($"Timed out waiting for {showCase.RouteTypeName}: {routeLabel}.");
    }

    private static void PumpLayout(Avalonia.Controls.Window window)
    {
        Dispatcher.UIThread.RunJobs();
        window.Measure(WindowSize);
        window.Arrange(WindowBounds);
        window.UpdateLayout();
        Dispatcher.UIThread.RunJobs();
    }

    private static string RenderResult(NavigationResult result, PerfOptions options, ShowCaseSpec showCase)
    {
        var builder = new StringBuilder();
        builder.AppendLine($"# {showCase.Label} navigation performance - {result.Label}");
        builder.AppendLine();
        builder.AppendLine($"- Timestamp: {DateTimeOffset.Now:yyyy-MM-dd HH:mm:ss zzz}");
        builder.AppendLine($"- Configuration: Debug, headless, {WindowSize.Width.ToString(CultureInfo.InvariantCulture)}x{WindowSize.Height.ToString(CultureInfo.InvariantCulture)} window");
        builder.AppendLine($"- Measurement: AboutUs route settled -> trigger {showCase.Label} navigation -> visual tree and layout stable");
        builder.AppendLine($"- Route type: `{showCase.RouteTypeName}`");
        builder.AppendLine($"- XAML source: `{Path.GetFullPath(showCase.XamlPath)}`");
        builder.AppendLine($"- Warmup: {options.Warmup}, measured iterations: {options.Iterations}, timeout: {options.Timeout.TotalSeconds.ToString("0.#", CultureInfo.InvariantCulture)}s");
        builder.AppendLine();
        builder.AppendLine("## Gallery source shape");
        builder.AppendLine();
        builder.AppendLine(SourceXamlStats.Read(showCase.XamlPath).RenderMarkdown());
        builder.AppendLine();
        builder.AppendLine("| Set | Trigger | Mean ms | Median ms | P95 ms | Min ms | Max ms | Alloc KB mean | Visuals | Logical | Icon | IconPresenter | PathIcon | LineEdit total | LineEdit direct | SearchEdit | TextArea | ShowCaseItem | IconGallery | IconInfoItem | AddOnDecoratedBox |");
        builder.AppendLine("| --- | --- | ---: | ---: | ---: | ---: | ---: | ---: | ---: | ---: | ---: | ---: | ---: | ---: | ---: | ---: | ---: | ---: | ---: | ---: | ---: |");
        builder.AppendLine(RenderSampleRow("Cold first navigation", [result.ColdRun]));
        builder.AppendLine(RenderSampleRow("Repeated navigation", result.Samples));
        builder.AppendLine();
        builder.AppendLine("## Samples");
        builder.AppendLine();
        builder.AppendLine("| Iteration | Phase | Trigger | Elapsed ms | Alloc KB | Visuals | Logical | Icon | IconPresenter | PathIcon | LineEdit total | LineEdit direct | SearchEdit | TextArea | ShowCaseItem | IconGallery | IconInfoItem | AddOnDecoratedBox |");
        builder.AppendLine("| ---: | --- | --- | ---: | ---: | ---: | ---: | ---: | ---: | ---: | ---: | ---: | ---: | ---: | ---: | ---: | ---: | ---: |");
        builder.AppendLine(RenderSample(result.ColdRun));
        foreach (var sample in result.Samples)
        {
            builder.AppendLine(RenderSample(sample));
        }
        return builder.ToString();
    }

    private static string RenderSampleRow(string label, IReadOnlyList<NavigationSample> samples)
    {
        var ordered = samples.Select(sample => sample.Elapsed.TotalMilliseconds).Order().ToArray();
        var mean    = ordered.Average();
        var median  = Percentile(ordered, 0.50);
        var p95     = Percentile(ordered, 0.95);
        var min     = ordered.First();
        var max     = ordered.Last();
        var allocKb = samples.Average(sample => sample.AllocatedBytes / 1024.0);
        var stats   = samples.Last().Stats;
        var trigger = string.Join(", ", samples.Select(sample => sample.Trigger).Distinct());

        return string.Join(" | ",
            "| " + label,
            trigger,
            Format(mean),
            Format(median),
            Format(p95),
            Format(min),
            Format(max),
            Format(allocKb),
            stats.VisualCount.ToString(CultureInfo.InvariantCulture),
            stats.LogicalCount.ToString(CultureInfo.InvariantCulture),
            stats.IconCount.ToString(CultureInfo.InvariantCulture),
            stats.IconPresenterCount.ToString(CultureInfo.InvariantCulture),
            stats.PathIconCount.ToString(CultureInfo.InvariantCulture),
            stats.LineEditCount.ToString(CultureInfo.InvariantCulture),
            stats.LineEditDirectCount.ToString(CultureInfo.InvariantCulture),
            stats.SearchEditCount.ToString(CultureInfo.InvariantCulture),
            stats.TextAreaCount.ToString(CultureInfo.InvariantCulture),
            stats.ShowCaseItemCount.ToString(CultureInfo.InvariantCulture),
            stats.IconGalleryCount.ToString(CultureInfo.InvariantCulture),
            stats.IconInfoItemCount.ToString(CultureInfo.InvariantCulture),
            stats.AddOnDecoratedBoxCount.ToString(CultureInfo.InvariantCulture) + " |");
    }

    private static string RenderSample(NavigationSample sample)
    {
        return string.Join(" | ",
            "| " + sample.Iteration.ToString(CultureInfo.InvariantCulture),
            sample.Phase,
            sample.Trigger,
            Format(sample.Elapsed.TotalMilliseconds),
            Format(sample.AllocatedBytes / 1024.0),
            sample.Stats.VisualCount.ToString(CultureInfo.InvariantCulture),
            sample.Stats.LogicalCount.ToString(CultureInfo.InvariantCulture),
            sample.Stats.IconCount.ToString(CultureInfo.InvariantCulture),
            sample.Stats.IconPresenterCount.ToString(CultureInfo.InvariantCulture),
            sample.Stats.PathIconCount.ToString(CultureInfo.InvariantCulture),
            sample.Stats.LineEditCount.ToString(CultureInfo.InvariantCulture),
            sample.Stats.LineEditDirectCount.ToString(CultureInfo.InvariantCulture),
            sample.Stats.SearchEditCount.ToString(CultureInfo.InvariantCulture),
            sample.Stats.TextAreaCount.ToString(CultureInfo.InvariantCulture),
            sample.Stats.ShowCaseItemCount.ToString(CultureInfo.InvariantCulture),
            sample.Stats.IconGalleryCount.ToString(CultureInfo.InvariantCulture),
            sample.Stats.IconInfoItemCount.ToString(CultureInfo.InvariantCulture),
            sample.Stats.AddOnDecoratedBoxCount.ToString(CultureInfo.InvariantCulture) + " |");
    }

    private static double Percentile(IReadOnlyList<double> ordered, double percentile)
    {
        if (ordered.Count == 0)
        {
            return 0;
        }
        if (ordered.Count == 1)
        {
            return ordered[0];
        }

        var index = (ordered.Count - 1) * percentile;
        var lower = (int)Math.Floor(index);
        var upper = (int)Math.Ceiling(index);
        if (lower == upper)
        {
            return ordered[lower];
        }

        var weight = index - lower;
        return ordered[lower] * (1 - weight) + ordered[upper] * weight;
    }

    private static string Format(double value)
    {
        return value.ToString("0.00", CultureInfo.InvariantCulture);
    }
}

internal sealed record PerfOptions(
    int Iterations,
    int Warmup,
    string Label,
    string ShowCase,
    string? MarkdownOutputPath,
    TimeSpan Timeout)
{
    public static PerfOptions Parse(string[] args)
    {
        var iterations = 20;
        var warmup     = 3;
        var label      = "current";
        var showCase   = "lineedit";
        var markdown   = default(string);
        var timeout    = TimeSpan.FromSeconds(10);

        for (var i = 0; i < args.Length; i++)
        {
            switch (args[i])
            {
                case "--iterations" when i + 1 < args.Length &&
                                         int.TryParse(args[i + 1], NumberStyles.Integer, CultureInfo.InvariantCulture, out var parsedIterations):
                    iterations = parsedIterations;
                    i++;
                    break;
                case "--warmup" when i + 1 < args.Length &&
                                     int.TryParse(args[i + 1], NumberStyles.Integer, CultureInfo.InvariantCulture, out var parsedWarmup):
                    warmup = parsedWarmup;
                    i++;
                    break;
                case "--label" when i + 1 < args.Length:
                    label = args[i + 1];
                    i++;
                    break;
                case "--showcase" when i + 1 < args.Length:
                    showCase = args[i + 1];
                    i++;
                    break;
                case "--markdown" when i + 1 < args.Length:
                    markdown = args[i + 1];
                    i++;
                    break;
                case "--timeout-ms" when i + 1 < args.Length &&
                                         int.TryParse(args[i + 1], NumberStyles.Integer, CultureInfo.InvariantCulture, out var parsedTimeout):
                    timeout = TimeSpan.FromMilliseconds(parsedTimeout);
                    i++;
                    break;
            }
        }

        return new PerfOptions(
            Math.Max(1, iterations),
            Math.Max(0, warmup),
            label,
            showCase,
            markdown,
            timeout);
    }
}

internal sealed record ShowCaseSpec(
    string Label,
    EntityKey Key,
    string RouteTypeName,
    string XamlPath,
    Func<RouteStats, bool> IsReady);

internal sealed record NavigationResult(
    string Label,
    NavigationSample ColdRun,
    IReadOnlyList<NavigationSample> Samples)
{
    public static NavigationResult Create(string label,
                                          NavigationSample coldRun,
                                          IReadOnlyList<NavigationSample> samples)
    {
        return new NavigationResult(label, coldRun, samples);
    }
}

internal sealed record NavigationSample(
    int Iteration,
    string Phase,
    string Trigger,
    TimeSpan Elapsed,
    long AllocatedBytes,
    RouteStats Stats);

internal sealed record RouteStats(
    int VisualCount,
    int LogicalCount,
    int IconCount,
    int IconPresenterCount,
    int PathIconCount,
    int LineEditCount,
    int LineEditDirectCount,
    int SearchEditCount,
    int TextAreaCount,
    int ShowCaseItemCount,
    int IconGalleryCount,
    int IconInfoItemCount,
    int AddOnDecoratedBoxCount)
{
    public bool IsDisplayReady(ShowCaseSpec showCase)
    {
        return VisualCount > 0 && showCase.IsReady(this);
    }

    public static RouteStats Collect(Control root)
    {
        var visuals                 = root.GetSelfAndVisualDescendants().ToList();
        var iconCount               = 0;
        var iconPresenterCount      = 0;
        var pathIconCount           = 0;
        var lineEditCount           = 0;
        var lineEditDirectCount     = 0;
        var searchEditCount         = 0;
        var textAreaCount           = 0;
        var showCaseItemCount       = 0;
        var iconGalleryCount        = 0;
        var iconInfoItemCount       = 0;
        var addOnDecoratedBoxCount  = 0;

        foreach (var visual in visuals)
        {
            var type = visual.GetType();
            if (IsTypeOrDerived(type, "AtomUI.Controls.Icon"))
            {
                iconCount++;
            }
            if (IsTypeOrDerived(type, "AtomUI.Controls.IconPresenter"))
            {
                iconPresenterCount++;
            }
            if (visual is PathIcon)
            {
                pathIconCount++;
            }
            if (type.FullName == "AtomUI.Desktop.Controls.LineEdit")
            {
                lineEditDirectCount++;
            }
            if (IsTypeOrDerived(type, "AtomUI.Desktop.Controls.SearchEdit"))
            {
                searchEditCount++;
            }
            if (IsTypeOrDerived(type, "AtomUI.Desktop.Controls.LineEdit"))
            {
                lineEditCount++;
            }
            if (IsTypeOrDerived(type, "AtomUI.Desktop.Controls.TextArea"))
            {
                textAreaCount++;
            }
            if (IsTypeOrDerived(type, "AtomUIGallery.Controls.ShowCaseItem"))
            {
                showCaseItemCount++;
            }
            if (IsTypeOrDerived(type, "AtomUIGallery.Controls.IconGallery"))
            {
                iconGalleryCount++;
            }
            if (IsTypeOrDerived(type, "AtomUIGallery.Controls.IconInfoItem"))
            {
                iconInfoItemCount++;
            }
            if (IsTypeOrDerived(type, "AtomUI.Desktop.Controls.AddOnDecoratedBox"))
            {
                addOnDecoratedBoxCount++;
            }
        }

        return new RouteStats(
            visuals.Count,
            root.GetSelfAndLogicalDescendants().Count(),
            iconCount,
            iconPresenterCount,
            pathIconCount,
            lineEditCount,
            lineEditDirectCount,
            searchEditCount,
            textAreaCount,
            showCaseItemCount,
            iconGalleryCount,
            iconInfoItemCount,
            addOnDecoratedBoxCount);
    }

    public bool HasSameShape(RouteStats other)
    {
        return VisualCount == other.VisualCount &&
               LogicalCount == other.LogicalCount &&
               IconCount == other.IconCount &&
               IconPresenterCount == other.IconPresenterCount &&
               PathIconCount == other.PathIconCount &&
               LineEditCount == other.LineEditCount &&
               LineEditDirectCount == other.LineEditDirectCount &&
               SearchEditCount == other.SearchEditCount &&
               TextAreaCount == other.TextAreaCount &&
               ShowCaseItemCount == other.ShowCaseItemCount &&
               IconGalleryCount == other.IconGalleryCount &&
               IconInfoItemCount == other.IconInfoItemCount &&
               AddOnDecoratedBoxCount == other.AddOnDecoratedBoxCount;
    }

    private static bool IsTypeOrDerived(Type type, string fullName)
    {
        while (true)
        {
            if (type.FullName == fullName)
            {
                return true;
            }

            if (type.BaseType is null)
            {
                return false;
            }

            type = type.BaseType;
        }
    }
}

internal sealed record SourceXamlStats(
    string SourcePath,
    bool IsAvailable,
    int AntDesignIconProviderCount,
    int IconPresenterCount,
    int IconGalleryCount,
    int LineEditDirectCount,
    int SearchEditCount,
    int TextAreaCount,
    int ShowCaseItemCount)
{
    private const string AtomNamespace = "https://atomui.net";
    private const string GalleryNamespace = "https://atomui.net/oss-controls/gallery";

    public static SourceXamlStats Read(string relativePath)
    {
        var sourcePath = Path.GetFullPath(relativePath);
        if (!File.Exists(sourcePath))
        {
            return new SourceXamlStats(sourcePath, false, 0, 0, 0, 0, 0, 0, 0);
        }

        var text     = File.ReadAllText(sourcePath);
        var document = XDocument.Load(sourcePath, LoadOptions.None);
        return new SourceXamlStats(
            sourcePath,
            true,
            CountText(text, "AntDesignIconProvider"),
            CountElements(document, AtomNamespace, "IconPresenter"),
            CountElements(document, GalleryNamespace, "IconGallery"),
            CountElements(document, AtomNamespace, "LineEdit"),
            CountElements(document, AtomNamespace, "SearchEdit"),
            CountElements(document, AtomNamespace, "TextArea"),
            CountElements(document, GalleryNamespace, "ShowCaseItem"));
    }

    public string RenderMarkdown()
    {
        if (!IsAvailable)
        {
            return $"`{SourcePath}` was not found.";
        }

        var lineEditTotal = LineEditDirectCount + SearchEditCount;
        var builder       = new StringBuilder();
        builder.AppendLine("| Source | AntDesignIconProvider | IconPresenter | IconGallery | LineEdit direct | SearchEdit | LineEdit total | TextArea | ShowCaseItem |");
        builder.AppendLine("| --- | ---: | ---: | ---: | ---: | ---: | ---: | ---: | ---: |");
        builder.Append("| `");
        builder.Append(SourcePath);
        builder.Append("` | ");
        builder.Append(AntDesignIconProviderCount.ToString(CultureInfo.InvariantCulture));
        builder.Append(" | ");
        builder.Append(IconPresenterCount.ToString(CultureInfo.InvariantCulture));
        builder.Append(" | ");
        builder.Append(IconGalleryCount.ToString(CultureInfo.InvariantCulture));
        builder.Append(" | ");
        builder.Append(LineEditDirectCount.ToString(CultureInfo.InvariantCulture));
        builder.Append(" | ");
        builder.Append(SearchEditCount.ToString(CultureInfo.InvariantCulture));
        builder.Append(" | ");
        builder.Append(lineEditTotal.ToString(CultureInfo.InvariantCulture));
        builder.Append(" | ");
        builder.Append(TextAreaCount.ToString(CultureInfo.InvariantCulture));
        builder.Append(" | ");
        builder.Append(ShowCaseItemCount.ToString(CultureInfo.InvariantCulture));
        builder.Append(" |");
        return builder.ToString();
    }

    private static int CountElements(XDocument document, string ns, string localName)
    {
        return document.Descendants()
                       .Count(element => element.Name.NamespaceName == ns &&
                                         element.Name.LocalName == localName);
    }

    private static int CountText(string source, string pattern)
    {
        var count = 0;
        var index = 0;
        while ((index = source.IndexOf(pattern, index, StringComparison.Ordinal)) >= 0)
        {
            count++;
            index += pattern.Length;
        }

        return count;
    }
}
