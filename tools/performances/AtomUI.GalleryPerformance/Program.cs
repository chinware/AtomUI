using System.Diagnostics;
using System.Globalization;
using System.Reflection;
using System.Reactive.Linq;
using System.Text;
using System.Xml.Linq;
using AtomUI;
using AtomUI.Controls;
using AtomUI.Desktop.Controls;
using AtomUIGallery.Desktop;
using AtomUIGallery.Controls;
using AtomUIGallery.ShowCases;
using AtomUIGallery.ShowCases.ViewModels;
using AtomUIGallery.ShowCases.Views;
using AtomUIGallery.Workspace.Views;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Headless;
using Avalonia.Layout;
using Avalonia.LogicalTree;
using Avalonia.Threading;
using Avalonia.VisualTree;
using ReactiveUI;
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
                stats => stats.IconCount > 0),
            ["button"] = new(
                "ButtonShowCase",
                ButtonViewModel.ID,
                "AtomUIGallery.ShowCases.Views.ButtonShowCase",
                "controlgallery/AtomUIGallery/ShowCases/Views/General/ButtonShowCase.axaml",
                stats => stats.ButtonCount > 0),
            ["dropdownbutton"] = new(
                "DropdownButtonShowCase",
                DropdownButtonViewModel.ID,
                "AtomUIGallery.ShowCases.Views.DropdownButtonShowCase",
                "controlgallery/AtomUIGallery/ShowCases/Views/Navigation/DropdownButtonShowCase.axaml",
                stats => stats.ButtonCount > 0),
            ["splitbutton"] = new(
                "SplitButtonShowCase",
                SplitButtonViewModel.ID,
                "AtomUIGallery.ShowCases.Views.SplitButtonShowCase",
                "controlgallery/AtomUIGallery/ShowCases/Views/General/SplitButtonShowCase.axaml",
                stats => stats.ButtonCount > 0),
            ["space"] = new(
                "SpaceShowCase",
                SpaceViewModel.ID,
                "AtomUIGallery.ShowCases.Views.SpaceShowCase",
                "controlgallery/AtomUIGallery/ShowCases/Views/Layout/SpaceShowCase.axaml",
                stats => stats.SpaceCount > 0 || stats.CompactSpaceCount > 0),
            ["select"] = new(
                "SelectShowCase",
                SelectViewModel.ID,
                "AtomUIGallery.ShowCases.Views.SelectShowCase",
                "controlgallery/AtomUIGallery/ShowCases/Views/DataEntry/SelectShowCase.axaml",
                stats => stats.SelectCount > 0),
            ["autocomplete"] = new(
                "AutoCompleteShowCase",
                AutoCompleteViewModel.ID,
                "AtomUIGallery.ShowCases.Views.AutoCompleteShowCase",
                "controlgallery/AtomUIGallery/ShowCases/Views/DataEntry/AutoCompleteShowCase.axaml",
                stats => stats.AutoCompleteCount > 0),
            ["treeselect"] = new(
                "TreeSelectShowCase",
                TreeSelectViewModel.ID,
                "AtomUIGallery.ShowCases.Views.TreeSelectShowCase",
                "controlgallery/AtomUIGallery/ShowCases/Views/DataEntry/TreeSelectShowCase.axaml",
                stats => stats.TreeSelectCount > 0),
            ["cascader"] = new(
                "CascaderShowCase",
                CascaderViewModel.ID,
                "AtomUIGallery.ShowCases.Views.CascaderShowCase",
                "controlgallery/AtomUIGallery/ShowCases/Views/DataEntry/CascaderShowCase.axaml",
                stats => stats.CascaderCount > 0),
            ["menu"] = new(
                "MenuShowCase",
                MenuViewModel.ID,
                "AtomUIGallery.ShowCases.Views.MenuShowCase",
                "controlgallery/AtomUIGallery/ShowCases/Views/Navigation/MenuShowCase.axaml",
                stats => stats.MenuItemCount > 0)
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
            if (options.SpaceItems)
            {
                var itemOutput = RunSpaceShowCaseItemBreakdown(options);
                Console.WriteLine(itemOutput);
                WriteMarkdownOutput(itemOutput, options);
                return 0;
            }

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

            if (options.TraceNavigation)
            {
                var traceOutput = RunNavigationTrace(window, options, showCase);
                Console.WriteLine(traceOutput);
                WriteMarkdownOutput(traceOutput, options);
                window.Close();
                Dispatcher.UIThread.RunJobs();
                return 0;
            }

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
                WriteMarkdownOutput(output, options);
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

    private static string RunNavigationTrace(WorkspaceWindow window, PerfOptions options, ShowCaseSpec showCase)
    {
        var samples = new List<NavigationTraceSample>
        {
            MeasureNavigationTrace(window, 0, "Cold", options, showCase)
        };
        NavigateToAboutUs(window, options);
        samples.Add(MeasureNavigationTrace(window, 1, "Second", options, showCase));

        var builder = new StringBuilder();
        builder.AppendLine($"# {showCase.Label} navigation trace - {options.Label}");
        builder.AppendLine();
        builder.AppendLine($"- Timestamp: {DateTimeOffset.Now:yyyy-MM-dd HH:mm:ss zzz}");
        builder.AppendLine($"- Configuration: Debug, headless, {WindowSize.Width:0}x{WindowSize.Height:0} window");
        builder.AppendLine("- Measurement: AboutUs route settled -> trigger navigation -> route visual tree and layout stable");
        builder.AppendLine();
        builder.AppendLine("| Phase | Trigger | Total ms | Trigger ms | First found ms | First ready ms | Stable ms | Pump count | Pump total ms | Max pump ms | Stats count | Stats total ms | Scan total ms | Alloc KB | Visuals | AddOnDecoratedBox | CompactSpace | CompactSpaceItem | LineEdit | Button | Select | AutoComplete | AC popup fields | AC candidate fields | CandidateList visuals | TreeSelect | Cascader |");
        builder.AppendLine("| --- | --- | ---: | ---: | ---: | ---: | ---: | ---: | ---: | ---: | ---: | ---: | ---: | ---: | ---: | ---: | ---: | ---: | ---: | ---: | ---: | ---: | ---: | ---: | ---: | ---: | ---: |");
        foreach (var sample in samples)
        {
            builder.AppendLine(
                $"| {sample.Phase} | {sample.Trigger} | {FormatMs(sample.Total)} | {FormatMs(sample.TriggerElapsed)} | {FormatOptionalMs(sample.FirstFoundElapsed)} | {FormatOptionalMs(sample.FirstReadyElapsed)} | {FormatMs(sample.StableElapsed)} | {sample.PumpCount} | {FormatMs(sample.PumpTotal)} | {FormatMs(sample.MaxPump)} | {sample.StatsCount} | {FormatMs(sample.StatsTotal)} | {FormatMs(sample.ScanTotal)} | {FormatKb(sample.AllocatedBytes)} | {sample.Stats.VisualCount} | {sample.Stats.AddOnDecoratedBoxCount} | {sample.Stats.CompactSpaceCount} | {sample.Stats.CompactSpaceItemCount} | {sample.Stats.LineEditCount} | {sample.Stats.ButtonCount} | {sample.Stats.SelectCount} | {sample.Stats.AutoCompleteCount} | {sample.Stats.AutoCompletePopupFieldCount} | {sample.Stats.AutoCompleteCandidateListFieldCount} | {sample.Stats.CandidateListCount} | {sample.Stats.TreeSelectCount} | {sample.Stats.CascaderCount} |");
        }

        builder.AppendLine();
        builder.AppendLine("## Shape Events");
        builder.AppendLine();
        foreach (var sample in samples)
        {
            builder.AppendLine($"### {sample.Phase}");
            builder.AppendLine();
            foreach (var item in sample.Events)
            {
                builder.AppendLine($"- {item}");
            }
            builder.AppendLine();
        }
        return builder.ToString();
    }

    private static NavigationTraceSample MeasureNavigationTrace(WorkspaceWindow window,
                                                               int iteration,
                                                               string phase,
                                                               PerfOptions options,
                                                               ShowCaseSpec showCase)
    {
        GC.Collect();
        GC.WaitForPendingFinalizers();
        GC.Collect();

        var allocatedBefore = GC.GetAllocatedBytesForCurrentThread();
        var totalStopwatch  = Stopwatch.StartNew();
        var trigger         = TriggerNavigation(window, showCase);
        ApplyNavigationVariant(window, options);
        var triggerElapsed  = totalStopwatch.Elapsed;
        var trace           = WaitForRouteTrace(window, showCase, options.Timeout, totalStopwatch);
        totalStopwatch.Stop();

        var allocatedBytes = GC.GetAllocatedBytesForCurrentThread() - allocatedBefore;
        return new NavigationTraceSample(
            iteration,
            phase,
            trigger,
            totalStopwatch.Elapsed,
            triggerElapsed,
            trace.FirstFoundElapsed,
            trace.FirstReadyElapsed,
            trace.StableElapsed,
            trace.PumpCount,
            trace.PumpTotal,
            trace.MaxPump,
            trace.StatsCount,
            trace.StatsTotal,
            trace.ScanTotal,
            allocatedBytes,
            trace.Stats,
            trace.Events);
    }

    private static void WriteMarkdownOutput(string output, PerfOptions options)
    {
        if (string.IsNullOrWhiteSpace(options.MarkdownOutputPath))
        {
            return;
        }

        var fullPath = Path.GetFullPath(options.MarkdownOutputPath);
        Directory.CreateDirectory(Path.GetDirectoryName(fullPath)!);
        File.WriteAllText(fullPath, output, new UTF8Encoding(encoderShouldEmitUTF8Identifier: false));
        Console.WriteLine();
        Console.WriteLine($"Wrote markdown result: {fullPath}");
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
        ApplyNavigationVariant(window, options);
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
                            .Concat(window.GetSelfAndLogicalDescendants().OfType<INavMenuItem>())
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

    private static void ApplyNavigationVariant(WorkspaceWindow window, PerfOptions options)
    {
        if (options.SpaceRemoveItem is not { } itemNumber)
        {
            return;
        }

        var route = window.GetSelfAndVisualDescendants()
                          .OfType<SpaceShowCase>()
                          .FirstOrDefault() ??
                    window.GetSelfAndLogicalDescendants()
                          .OfType<SpaceShowCase>()
                          .FirstOrDefault();
        if (route is null)
        {
            return;
        }

        var panel = GetSpaceShowCasePanel(route);
        var items = panel.Children.OfType<ShowCaseItem>().Where(item => !item.IsFake).ToList();
        var index = itemNumber - 1;
        if (index < 0 || index >= items.Count)
        {
            throw new ArgumentOutOfRangeException(nameof(options.SpaceRemoveItem),
                $"SpaceShowCase item {itemNumber} is out of range. Count={items.Count}.");
        }
        panel.Children.Remove(items[index]);
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

    private static NavigationTraceData WaitForRouteTrace(WorkspaceWindow window,
                                                         ShowCaseSpec showCase,
                                                         TimeSpan timeout,
                                                         Stopwatch totalStopwatch)
    {
        var stableLayoutPasses = 0;
        var previousStats      = default(RouteStats);
        var events             = new List<string>();
        var pumpCount          = 0;
        var statsCount         = 0;
        var pumpTotal          = TimeSpan.Zero;
        var maxPump            = TimeSpan.Zero;
        var statsTotal         = TimeSpan.Zero;
        var scanTotal          = TimeSpan.Zero;
        var firstFoundElapsed  = default(TimeSpan?);
        var firstReadyElapsed  = default(TimeSpan?);
        var latestStats        = default(RouteStats);

        while (totalStopwatch.Elapsed < timeout)
        {
            var pumpStopwatch = Stopwatch.StartNew();
            PumpLayout(window);
            pumpStopwatch.Stop();
            pumpCount++;
            pumpTotal += pumpStopwatch.Elapsed;
            if (pumpStopwatch.Elapsed > maxPump)
            {
                maxPump = pumpStopwatch.Elapsed;
            }

            var scanStopwatch = Stopwatch.StartNew();
            var route = window.GetSelfAndVisualDescendants()
                              .OfType<Control>()
                              .FirstOrDefault(control => control.GetType().FullName == showCase.RouteTypeName);
            scanStopwatch.Stop();
            scanTotal += scanStopwatch.Elapsed;

            if (route is null)
            {
                continue;
            }

            if (firstFoundElapsed is null)
            {
                firstFoundElapsed = totalStopwatch.Elapsed;
                events.Add($"+{FormatMs(firstFoundElapsed.Value)} first route found");
            }

            if (!route.IsVisible || route.Bounds.Width <= 0 || route.Bounds.Height <= 0)
            {
                continue;
            }

            var statsStopwatch = Stopwatch.StartNew();
            var currentStats   = RouteStats.Collect(route);
            statsStopwatch.Stop();
            statsCount++;
            statsTotal += statsStopwatch.Elapsed;
            latestStats = currentStats;

            if (currentStats.IsDisplayReady(showCase) && firstReadyElapsed is null)
            {
                firstReadyElapsed = totalStopwatch.Elapsed;
                events.Add($"+{FormatMs(firstReadyElapsed.Value)} first display-ready shape: {DescribeStats(currentStats)}");
            }

            if (previousStats is null || !currentStats.HasSameShape(previousStats))
            {
                events.Add($"+{FormatMs(totalStopwatch.Elapsed)} shape changed: {DescribeStats(currentStats)}");
                stableLayoutPasses = 0;
                previousStats      = currentStats;
                continue;
            }

            if (currentStats.IsDisplayReady(showCase))
            {
                stableLayoutPasses++;
                events.Add($"+{FormatMs(totalStopwatch.Elapsed)} stable pass {stableLayoutPasses}: {DescribeStats(currentStats)}");
                if (stableLayoutPasses >= 2)
                {
                    return new NavigationTraceData(
                        firstFoundElapsed,
                        firstReadyElapsed,
                        totalStopwatch.Elapsed,
                        pumpCount,
                        pumpTotal,
                        maxPump,
                        statsCount,
                        statsTotal,
                        scanTotal,
                        currentStats,
                        events);
                }
            }
        }

        throw new TimeoutException(
            $"Timed out waiting for {showCase.RouteTypeName}: latest={DescribeStats(latestStats)}.");
    }

    private static void PumpLayout(Avalonia.Controls.Window window)
    {
        Dispatcher.UIThread.RunJobs();
        window.Measure(WindowSize);
        window.Arrange(WindowBounds);
        window.UpdateLayout();
        Dispatcher.UIThread.RunJobs();
    }

    private static string RunSpaceShowCaseItemBreakdown(PerfOptions options)
    {
        var itemInfos = GetSpaceShowCaseItemInfos();
        var samples = new List<SpaceShowCaseItemSample>();
        foreach (var itemInfo in itemInfos)
        {
            for (var i = 0; i < options.Warmup; i++)
            {
                _ = MeasureSpaceShowCaseItem(itemInfo, i + 1, "Warmup", options);
            }

            for (var i = 0; i < options.Iterations; i++)
            {
                samples.Add(MeasureSpaceShowCaseItem(itemInfo, i + 1, "Measured", options));
            }
        }

        return RenderSpaceShowCaseItemBreakdown(samples, options);
    }

    private static IReadOnlyList<SpaceShowCaseItemInfo> GetSpaceShowCaseItemInfos()
    {
        var view  = CreateSpaceShowCase();
        var panel = GetSpaceShowCasePanel(view);
        return panel.Children
                    .OfType<ShowCaseItem>()
                    .Where(item => !item.IsFake)
                    .Select((item, index) => new SpaceShowCaseItemInfo(
                        index,
                        string.IsNullOrWhiteSpace(item.Title) ? $"Item {index + 1}" : item.Title,
                        string.IsNullOrWhiteSpace(item.Description) ? string.Empty : item.Description))
                    .ToList();
    }

    private static SpaceShowCaseItemSample MeasureSpaceShowCaseItem(SpaceShowCaseItemInfo itemInfo,
                                                                    int iteration,
                                                                    string phase,
                                                                    PerfOptions options)
    {
        GC.Collect();
        GC.WaitForPendingFinalizers();
        GC.Collect();

        var view   = CreateSpaceShowCase();
        var panel  = GetSpaceShowCasePanel(view);
        var items  = panel.Children.OfType<ShowCaseItem>().Where(item => !item.IsFake).ToList();
        var target = items[itemInfo.Index];
        for (var i = panel.Children.Count - 1; i >= 0; i--)
        {
            if (!ReferenceEquals(panel.Children[i], target))
            {
                panel.Children.RemoveAt(i);
            }
        }
        if (options.SpaceItemsWithoutTreeCascader ||
            options.SpaceItemsWithoutTreeCascaderSelect)
        {
            RemoveSpaceItemVariantControls(target, options.SpaceItemsWithoutTreeCascaderSelect);
        }

        var window = new Avalonia.Controls.Window
        {
            Width         = WindowSize.Width,
            Height        = WindowSize.Height,
            Content       = view,
            ShowInTaskbar = false
        };

        var allocatedBefore = GC.GetAllocatedBytesForCurrentThread();
        var stopwatch       = Stopwatch.StartNew();
        window.Show();
        WaitForStableControl(window, target, options.Timeout);
        stopwatch.Stop();

        var allocatedBytes = GC.GetAllocatedBytesForCurrentThread() - allocatedBefore;
        var stats          = RouteStats.Collect(target);
        window.Close();
        Dispatcher.UIThread.RunJobs();

        return new SpaceShowCaseItemSample(itemInfo, iteration, phase, stopwatch.Elapsed, allocatedBytes, stats);
    }

    private static void RemoveSpaceItemVariantControls(Control root, bool includeSelect)
    {
        RemoveMatchingChildren(root, control =>
        {
            var typeName = control.GetType().FullName;
            return typeName is "AtomUI.Desktop.Controls.TreeSelect" or
                               "AtomUI.Desktop.Controls.Cascader" ||
                   includeSelect && typeName == "AtomUI.Desktop.Controls.Select";
        });
    }

    private static void RemoveMatchingChildren(Control control, Func<Control, bool> shouldRemove)
    {
        if (control is Panel panel)
        {
            for (var i = panel.Children.Count - 1; i >= 0; i--)
            {
                if (panel.Children[i] is Control child)
                {
                    if (shouldRemove(child))
                    {
                        panel.Children.RemoveAt(i);
                    }
                    else
                    {
                        RemoveMatchingChildren(child, shouldRemove);
                    }
                }
            }
        }

        if (control is Space space)
        {
            for (var i = space.Children.Count - 1; i >= 0; i--)
            {
                var child = space.Children[i];
                if (shouldRemove(child))
                {
                    space.Children.RemoveAt(i);
                }
                else
                {
                    RemoveMatchingChildren(child, shouldRemove);
                }
            }
        }

        if (control is CompactSpace compactSpace)
        {
            for (var i = compactSpace.Children.Count - 1; i >= 0; i--)
            {
                var child = compactSpace.Children[i];
                if (shouldRemove(child))
                {
                    compactSpace.Children.RemoveAt(i);
                }
                else
                {
                    RemoveMatchingChildren(child, shouldRemove);
                }
            }
        }

        if (control is ContentControl contentControl &&
            contentControl.Content is Control content)
        {
            if (shouldRemove(content))
            {
                contentControl.Content = null;
            }
            else
            {
                RemoveMatchingChildren(content, shouldRemove);
            }
        }
    }

    private static SpaceShowCase CreateSpaceShowCase()
    {
        var viewModel = new SpaceViewModel(new ProbeScreen())
        {
            SizeType           = CustomizableSizeType.Small,
            CustomSpacingValue = 24
        };
        return new SpaceShowCase
        {
            DataContext = viewModel
        };
    }

    private static ShowCasePanel GetSpaceShowCasePanel(SpaceShowCase view)
    {
        if (view.Content is ShowCasePanel panel)
        {
            return panel;
        }
        throw new InvalidOperationException("SpaceShowCase root content is not ShowCasePanel.");
    }

    private static void WaitForStableControl(Avalonia.Controls.Window window, Control target, TimeSpan timeout)
    {
        var stopwatch          = Stopwatch.StartNew();
        var stableLayoutPasses = 0;
        var previousStats      = default(RouteStats);

        while (stopwatch.Elapsed < timeout)
        {
            PumpLayout(window);

            if (target.IsVisible && target.Bounds.Width > 0 && target.Bounds.Height > 0)
            {
                var currentStats = RouteStats.Collect(target);
                if (previousStats is not null &&
                    currentStats.HasSameShape(previousStats))
                {
                    stableLayoutPasses++;
                    if (stableLayoutPasses >= 2)
                    {
                        return;
                    }
                }
                else
                {
                    stableLayoutPasses = 0;
                    previousStats      = currentStats;
                }
            }
        }

        throw new TimeoutException($"Timed out waiting for SpaceShowCase item layout: {target}.");
    }

    private static string RenderSpaceShowCaseItemBreakdown(IReadOnlyList<SpaceShowCaseItemSample> samples,
                                                           PerfOptions options)
    {
        var builder = new StringBuilder();
        builder.AppendLine("# SpaceShowCase item performance breakdown");
        builder.AppendLine();
        builder.AppendLine($"- Timestamp: {DateTimeOffset.Now:yyyy-MM-dd HH:mm:ss zzz}");
        builder.AppendLine($"- Configuration: Debug, headless, {WindowSize.Width.ToString(CultureInfo.InvariantCulture)}x{WindowSize.Height.ToString(CultureInfo.InvariantCulture)} window");
        builder.AppendLine("- Measurement: construct the real SpaceShowCase, keep one real ShowCaseItem before ShowCasePanel template/layout, then time attach/template/layout until stable.");
        if (options.SpaceItemsWithoutTreeCascaderSelect)
        {
            builder.AppendLine("- Variant: `TreeSelect`, `Cascader`, and exact `Select` controls are removed from each item content tree before attach/layout.");
        }
        else if (options.SpaceItemsWithoutTreeCascader)
        {
            builder.AppendLine("- Variant: `TreeSelect` and `Cascader` controls are removed from each item content tree before attach/layout.");
        }
        builder.AppendLine($"- Warmup per item: {options.Warmup}, measured iterations per item: {options.Iterations}, timeout: {options.Timeout.TotalSeconds.ToString("0.#", CultureInfo.InvariantCulture)}s");
        builder.AppendLine();
        builder.AppendLine("| # | Title | Description | Mean ms | Median ms | P95 ms | Min ms | Max ms | Alloc KB mean | Visuals | Logical | Space | CompactSpace | CompactSpaceItem | LineEdit total | LineEdit direct | SearchEdit | TextArea | Button | Select | TreeSelect | Cascader | Menu | MenuItem | AddOnDecoratedBox |");
        builder.AppendLine("| ---: | --- | --- | ---: | ---: | ---: | ---: | ---: | ---: | ---: | ---: | ---: | ---: | ---: | ---: | ---: | ---: | ---: | ---: | ---: | ---: | ---: | ---: | ---: | ---: |");
        foreach (var group in samples.GroupBy(sample => sample.Item).OrderBy(group => group.Key.Index))
        {
            var ordered = group.Select(sample => sample.Elapsed.TotalMilliseconds).Order().ToArray();
            var stats   = group.Last().Stats;
            builder.AppendLine(string.Join(" | ",
                "| " + (group.Key.Index + 1).ToString(CultureInfo.InvariantCulture),
                EscapeCell(group.Key.Title),
                EscapeCell(group.Key.Description),
                Format(ordered.Average()),
                Format(Percentile(ordered, 0.50)),
                Format(Percentile(ordered, 0.95)),
                Format(ordered.First()),
                Format(ordered.Last()),
                Format(group.Average(sample => sample.AllocatedBytes / 1024.0)),
                stats.VisualCount.ToString(CultureInfo.InvariantCulture),
                stats.LogicalCount.ToString(CultureInfo.InvariantCulture),
                stats.SpaceCount.ToString(CultureInfo.InvariantCulture),
                stats.CompactSpaceCount.ToString(CultureInfo.InvariantCulture),
                stats.CompactSpaceItemCount.ToString(CultureInfo.InvariantCulture),
                stats.LineEditCount.ToString(CultureInfo.InvariantCulture),
                stats.LineEditDirectCount.ToString(CultureInfo.InvariantCulture),
                stats.SearchEditCount.ToString(CultureInfo.InvariantCulture),
                stats.TextAreaCount.ToString(CultureInfo.InvariantCulture),
                stats.ButtonCount.ToString(CultureInfo.InvariantCulture),
                stats.SelectCount.ToString(CultureInfo.InvariantCulture),
                stats.TreeSelectCount.ToString(CultureInfo.InvariantCulture),
                stats.CascaderCount.ToString(CultureInfo.InvariantCulture),
                stats.MenuCount.ToString(CultureInfo.InvariantCulture),
                stats.MenuItemCount.ToString(CultureInfo.InvariantCulture),
                stats.AddOnDecoratedBoxCount.ToString(CultureInfo.InvariantCulture) + " |"));
        }

        builder.AppendLine();
        builder.AppendLine("## Samples");
        builder.AppendLine();
        builder.AppendLine("| # | Title | Iteration | Elapsed ms | Alloc KB | Visuals | Logical | Space | CompactSpace | CompactSpaceItem | LineEdit total | SearchEdit | Button | Select | TreeSelect | Cascader | AddOnDecoratedBox |");
        builder.AppendLine("| ---: | --- | ---: | ---: | ---: | ---: | ---: | ---: | ---: | ---: | ---: | ---: | ---: | ---: | ---: | ---: | ---: |");
        foreach (var sample in samples.OrderBy(sample => sample.Item.Index).ThenBy(sample => sample.Iteration))
        {
            builder.AppendLine(string.Join(" | ",
                "| " + (sample.Item.Index + 1).ToString(CultureInfo.InvariantCulture),
                EscapeCell(sample.Item.Title),
                sample.Iteration.ToString(CultureInfo.InvariantCulture),
                Format(sample.Elapsed.TotalMilliseconds),
                Format(sample.AllocatedBytes / 1024.0),
                sample.Stats.VisualCount.ToString(CultureInfo.InvariantCulture),
                sample.Stats.LogicalCount.ToString(CultureInfo.InvariantCulture),
                sample.Stats.SpaceCount.ToString(CultureInfo.InvariantCulture),
                sample.Stats.CompactSpaceCount.ToString(CultureInfo.InvariantCulture),
                sample.Stats.CompactSpaceItemCount.ToString(CultureInfo.InvariantCulture),
                sample.Stats.LineEditCount.ToString(CultureInfo.InvariantCulture),
                sample.Stats.SearchEditCount.ToString(CultureInfo.InvariantCulture),
                sample.Stats.ButtonCount.ToString(CultureInfo.InvariantCulture),
                sample.Stats.SelectCount.ToString(CultureInfo.InvariantCulture),
                sample.Stats.TreeSelectCount.ToString(CultureInfo.InvariantCulture),
                sample.Stats.CascaderCount.ToString(CultureInfo.InvariantCulture),
                sample.Stats.AddOnDecoratedBoxCount.ToString(CultureInfo.InvariantCulture) + " |"));
        }
        return builder.ToString();
    }

    private static string EscapeCell(string value)
    {
        return value.Replace("|", "\\|", StringComparison.Ordinal)
                    .ReplaceLineEndings(" ");
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
        builder.AppendLine("| Set | Trigger | Mean ms | Median ms | P95 ms | Min ms | Max ms | Alloc KB mean | Visuals | Logical | Space | CompactSpace | CompactSpaceItem | Icon | IconPresenter | PathIcon | LineEdit total | LineEdit direct | SearchEdit | TextArea | Button | ToggleIconButton | Select | AutoComplete | AC popup fields | AC candidate fields | CandidateList visuals | TreeSelect | Cascader | Menu | MenuItem | NavMenuHeader | ShowCaseItem | IconGallery | IconInfoItem | AddOnDecoratedBox |");
        builder.AppendLine("| --- | --- | ---: | ---: | ---: | ---: | ---: | ---: | ---: | ---: | ---: | ---: | ---: | ---: | ---: | ---: | ---: | ---: | ---: | ---: | ---: | ---: | ---: | ---: | ---: | ---: | ---: | ---: | ---: | ---: | ---: | ---: | ---: | ---: | ---: | ---: |");
        builder.AppendLine(RenderSampleRow("Cold first navigation", [result.ColdRun]));
        builder.AppendLine(RenderSampleRow("Repeated navigation", result.Samples));
        builder.AppendLine();
        builder.AppendLine("## Samples");
        builder.AppendLine();
        builder.AppendLine("| Iteration | Phase | Trigger | Elapsed ms | Alloc KB | Visuals | Logical | Space | CompactSpace | CompactSpaceItem | Icon | IconPresenter | PathIcon | LineEdit total | LineEdit direct | SearchEdit | TextArea | Button | ToggleIconButton | Select | AutoComplete | AC popup fields | AC candidate fields | CandidateList visuals | TreeSelect | Cascader | Menu | MenuItem | NavMenuHeader | ShowCaseItem | IconGallery | IconInfoItem | AddOnDecoratedBox |");
        builder.AppendLine("| ---: | --- | --- | ---: | ---: | ---: | ---: | ---: | ---: | ---: | ---: | ---: | ---: | ---: | ---: | ---: | ---: | ---: | ---: | ---: | ---: | ---: | ---: | ---: | ---: | ---: | ---: | ---: | ---: | ---: | ---: | ---: | ---: |");
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
            stats.SpaceCount.ToString(CultureInfo.InvariantCulture),
            stats.CompactSpaceCount.ToString(CultureInfo.InvariantCulture),
            stats.CompactSpaceItemCount.ToString(CultureInfo.InvariantCulture),
            stats.IconCount.ToString(CultureInfo.InvariantCulture),
            stats.IconPresenterCount.ToString(CultureInfo.InvariantCulture),
            stats.PathIconCount.ToString(CultureInfo.InvariantCulture),
            stats.LineEditCount.ToString(CultureInfo.InvariantCulture),
            stats.LineEditDirectCount.ToString(CultureInfo.InvariantCulture),
            stats.SearchEditCount.ToString(CultureInfo.InvariantCulture),
            stats.TextAreaCount.ToString(CultureInfo.InvariantCulture),
            stats.ButtonCount.ToString(CultureInfo.InvariantCulture),
            stats.ToggleIconButtonCount.ToString(CultureInfo.InvariantCulture),
            stats.SelectCount.ToString(CultureInfo.InvariantCulture),
            stats.AutoCompleteCount.ToString(CultureInfo.InvariantCulture),
            stats.AutoCompletePopupFieldCount.ToString(CultureInfo.InvariantCulture),
            stats.AutoCompleteCandidateListFieldCount.ToString(CultureInfo.InvariantCulture),
            stats.CandidateListCount.ToString(CultureInfo.InvariantCulture),
            stats.TreeSelectCount.ToString(CultureInfo.InvariantCulture),
            stats.CascaderCount.ToString(CultureInfo.InvariantCulture),
            stats.MenuCount.ToString(CultureInfo.InvariantCulture),
            stats.MenuItemCount.ToString(CultureInfo.InvariantCulture),
            stats.NavMenuItemHeaderCount.ToString(CultureInfo.InvariantCulture),
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
            sample.Stats.SpaceCount.ToString(CultureInfo.InvariantCulture),
            sample.Stats.CompactSpaceCount.ToString(CultureInfo.InvariantCulture),
            sample.Stats.CompactSpaceItemCount.ToString(CultureInfo.InvariantCulture),
            sample.Stats.IconCount.ToString(CultureInfo.InvariantCulture),
            sample.Stats.IconPresenterCount.ToString(CultureInfo.InvariantCulture),
            sample.Stats.PathIconCount.ToString(CultureInfo.InvariantCulture),
            sample.Stats.LineEditCount.ToString(CultureInfo.InvariantCulture),
            sample.Stats.LineEditDirectCount.ToString(CultureInfo.InvariantCulture),
            sample.Stats.SearchEditCount.ToString(CultureInfo.InvariantCulture),
            sample.Stats.TextAreaCount.ToString(CultureInfo.InvariantCulture),
            sample.Stats.ButtonCount.ToString(CultureInfo.InvariantCulture),
            sample.Stats.ToggleIconButtonCount.ToString(CultureInfo.InvariantCulture),
            sample.Stats.SelectCount.ToString(CultureInfo.InvariantCulture),
            sample.Stats.AutoCompleteCount.ToString(CultureInfo.InvariantCulture),
            sample.Stats.AutoCompletePopupFieldCount.ToString(CultureInfo.InvariantCulture),
            sample.Stats.AutoCompleteCandidateListFieldCount.ToString(CultureInfo.InvariantCulture),
            sample.Stats.CandidateListCount.ToString(CultureInfo.InvariantCulture),
            sample.Stats.TreeSelectCount.ToString(CultureInfo.InvariantCulture),
            sample.Stats.CascaderCount.ToString(CultureInfo.InvariantCulture),
            sample.Stats.MenuCount.ToString(CultureInfo.InvariantCulture),
            sample.Stats.MenuItemCount.ToString(CultureInfo.InvariantCulture),
            sample.Stats.NavMenuItemHeaderCount.ToString(CultureInfo.InvariantCulture),
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

    private static string FormatMs(TimeSpan value)
    {
        return value.TotalMilliseconds.ToString("0.00", CultureInfo.InvariantCulture);
    }

    private static string FormatOptionalMs(TimeSpan? value)
    {
        return value.HasValue ? FormatMs(value.Value) : string.Empty;
    }

    private static string FormatKb(long bytes)
    {
        return (bytes / 1024.0).ToString("0.00", CultureInfo.InvariantCulture);
    }

    private static string DescribeStats(RouteStats? stats)
    {
        if (stats is null)
        {
            return "none";
        }

        return $"visuals={stats.VisualCount}, logical={stats.LogicalCount}, space={stats.SpaceCount}, compactSpace={stats.CompactSpaceCount}, compactItems={stats.CompactSpaceItemCount}, lineEdit={stats.LineEditCount}, button={stats.ButtonCount}, select={stats.SelectCount}, autoComplete={stats.AutoCompleteCount}, autoCompletePopupFields={stats.AutoCompletePopupFieldCount}, autoCompleteCandidateFields={stats.AutoCompleteCandidateListFieldCount}, candidateListVisuals={stats.CandidateListCount}, treeSelect={stats.TreeSelectCount}, cascader={stats.CascaderCount}, addOnDecoratedBox={stats.AddOnDecoratedBoxCount}";
    }
}

internal sealed record PerfOptions(
    int Iterations,
    int Warmup,
    string Label,
    string ShowCase,
    string? MarkdownOutputPath,
    TimeSpan Timeout,
    bool SpaceItems,
    bool SpaceItemsWithoutTreeCascader,
    bool SpaceItemsWithoutTreeCascaderSelect,
    bool TraceNavigation,
    int? SpaceRemoveItem)
{
    public static PerfOptions Parse(string[] args)
    {
        var iterations = 20;
        var warmup     = 3;
        var label      = "current";
        var showCase   = "lineedit";
        var markdown   = default(string);
        var timeout    = TimeSpan.FromSeconds(10);
        var spaceItems = false;
        var spaceItemsWithoutTreeCascader = false;
        var spaceItemsWithoutTreeCascaderSelect = false;
        var traceNavigation = false;
        var spaceRemoveItem = default(int?);

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
                case "--space-items":
                    showCase   = "space";
                    spaceItems = true;
                    break;
                case "--space-items-without-tree-cascader":
                    showCase                      = "space";
                    spaceItems                    = true;
                    spaceItemsWithoutTreeCascader = true;
                    break;
                case "--space-items-without-tree-cascader-select":
                    showCase                            = "space";
                    spaceItems                          = true;
                    spaceItemsWithoutTreeCascaderSelect = true;
                    break;
                case "--trace-navigation":
                    traceNavigation = true;
                    break;
                case "--space-remove-item" when i + 1 < args.Length &&
                                                int.TryParse(args[i + 1], NumberStyles.Integer, CultureInfo.InvariantCulture, out var parsedItem):
                    showCase        = "space";
                    spaceRemoveItem = parsedItem;
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
            timeout,
            spaceItems,
            spaceItemsWithoutTreeCascader,
            spaceItemsWithoutTreeCascaderSelect,
            traceNavigation,
            spaceRemoveItem);
    }
}

internal sealed class ProbeScreen : IScreen
{
    public RoutingState Router { get; } = new();
}

internal sealed record SpaceShowCaseItemInfo(int Index, string Title, string Description);

internal sealed record SpaceShowCaseItemSample(
    SpaceShowCaseItemInfo Item,
    int Iteration,
    string Phase,
    TimeSpan Elapsed,
    long AllocatedBytes,
    RouteStats Stats);

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

internal sealed record NavigationTraceData(
    TimeSpan? FirstFoundElapsed,
    TimeSpan? FirstReadyElapsed,
    TimeSpan StableElapsed,
    int PumpCount,
    TimeSpan PumpTotal,
    TimeSpan MaxPump,
    int StatsCount,
    TimeSpan StatsTotal,
    TimeSpan ScanTotal,
    RouteStats Stats,
    IReadOnlyList<string> Events);

internal sealed record NavigationTraceSample(
    int Iteration,
    string Phase,
    string Trigger,
    TimeSpan Total,
    TimeSpan TriggerElapsed,
    TimeSpan? FirstFoundElapsed,
    TimeSpan? FirstReadyElapsed,
    TimeSpan StableElapsed,
    int PumpCount,
    TimeSpan PumpTotal,
    TimeSpan MaxPump,
    int StatsCount,
    TimeSpan StatsTotal,
    TimeSpan ScanTotal,
    long AllocatedBytes,
    RouteStats Stats,
    IReadOnlyList<string> Events);

internal sealed record RouteStats(
    int VisualCount,
    int LogicalCount,
    int SpaceCount,
    int CompactSpaceCount,
    int CompactSpaceItemCount,
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
    int AddOnDecoratedBoxCount,
    int ButtonCount,
    int ToggleIconButtonCount,
    int SelectCount,
    int AutoCompleteCount,
    int AutoCompleteSearchEditCount,
    int AutoCompleteTextAreaCount,
    int AutoCompletePopupFieldCount,
    int AutoCompleteCandidateListFieldCount,
    int CandidateListCount,
    int TreeSelectCount,
    int CascaderCount,
    int MenuCount,
    int MenuItemCount,
    int NavMenuItemHeaderCount)
{
    public bool IsDisplayReady(ShowCaseSpec showCase)
    {
        return VisualCount > 0 && showCase.IsReady(this);
    }

    public static RouteStats Collect(Control root)
    {
        var visuals                 = root.GetSelfAndVisualDescendants().ToList();
        var spaceCount              = 0;
        var compactSpaceCount       = 0;
        var compactSpaceItemCount   = 0;
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
        var buttonCount             = 0;
        var toggleIconButtonCount   = 0;
        var selectCount             = 0;
        var autoCompleteCount       = 0;
        var autoCompleteSearchEditCount = 0;
        var autoCompleteTextAreaCount   = 0;
        var autoCompletePopupFieldCount = 0;
        var autoCompleteCandidateListFieldCount = 0;
        var candidateListCount      = 0;
        var treeSelectCount         = 0;
        var cascaderCount           = 0;
        var menuCount               = 0;
        var menuItemCount           = 0;
        var navMenuItemHeaderCount  = 0;

        foreach (var visual in visuals)
        {
            var type = visual.GetType();
            if (IsTypeOrDerived(type, "AtomUI.Desktop.Controls.Space"))
            {
                spaceCount++;
            }
            if (IsTypeOrDerived(type, "AtomUI.Desktop.Controls.CompactSpace"))
            {
                compactSpaceCount++;
            }
            if (IsTypeOrDerived(type, "AtomUI.Desktop.Controls.CompactSpaceItem"))
            {
                compactSpaceItemCount++;
            }
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
            if (IsTypeOrDerived(type, "AtomUI.Desktop.Controls.Button"))
            {
                buttonCount++;
            }
            if (IsTypeOrDerived(type, "AtomUI.Desktop.Controls.ToggleIconButton"))
            {
                toggleIconButtonCount++;
            }
            if (IsTypeOrDerived(type, "AtomUI.Desktop.Controls.Select"))
            {
                selectCount++;
            }
            if (IsTypeOrDerived(type, "AtomUI.Desktop.Controls.AbstractAutoComplete"))
            {
                autoCompleteCount++;
                if (HasFieldValue(visual, "AtomUI.Desktop.Controls.AbstractAutoComplete", "_popup"))
                {
                    autoCompletePopupFieldCount++;
                }
                if (HasFieldValue(visual, "AtomUI.Desktop.Controls.AbstractAutoComplete", "_candidateList"))
                {
                    autoCompleteCandidateListFieldCount++;
                }
            }
            if (IsTypeOrDerived(type, "AtomUI.Desktop.Controls.AutoCompleteSearchEdit"))
            {
                autoCompleteSearchEditCount++;
            }
            if (IsTypeOrDerived(type, "AtomUI.Desktop.Controls.AutoCompleteTextArea"))
            {
                autoCompleteTextAreaCount++;
            }
            if (IsTypeOrDerived(type, "AtomUI.Desktop.Controls.Primitives.CandidateList"))
            {
                candidateListCount++;
            }
            if (IsTypeOrDerived(type, "AtomUI.Desktop.Controls.TreeSelect"))
            {
                treeSelectCount++;
            }
            if (IsTypeOrDerived(type, "AtomUI.Desktop.Controls.Cascader"))
            {
                cascaderCount++;
            }
            if (IsTypeOrDerived(type, "AtomUI.Desktop.Controls.Menu"))
            {
                menuCount++;
            }
            if (IsTypeOrDerived(type, "AtomUI.Desktop.Controls.MenuItem"))
            {
                menuItemCount++;
            }
            if (IsTypeOrDerived(type, "AtomUI.Desktop.Controls.BaseNavMenuItemHeader"))
            {
                navMenuItemHeaderCount++;
            }
        }

        return new RouteStats(
            visuals.Count,
            root.GetSelfAndLogicalDescendants().Count(),
            spaceCount,
            compactSpaceCount,
            compactSpaceItemCount,
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
            addOnDecoratedBoxCount,
            buttonCount,
            toggleIconButtonCount,
            selectCount,
            autoCompleteCount,
            autoCompleteSearchEditCount,
            autoCompleteTextAreaCount,
            autoCompletePopupFieldCount,
            autoCompleteCandidateListFieldCount,
            candidateListCount,
            treeSelectCount,
            cascaderCount,
            menuCount,
            menuItemCount,
            navMenuItemHeaderCount);
    }

    public bool HasSameShape(RouteStats other)
    {
        return VisualCount == other.VisualCount &&
               LogicalCount == other.LogicalCount &&
               SpaceCount == other.SpaceCount &&
               CompactSpaceCount == other.CompactSpaceCount &&
               CompactSpaceItemCount == other.CompactSpaceItemCount &&
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
               AddOnDecoratedBoxCount == other.AddOnDecoratedBoxCount &&
               ButtonCount == other.ButtonCount &&
               ToggleIconButtonCount == other.ToggleIconButtonCount &&
               SelectCount == other.SelectCount &&
               AutoCompleteCount == other.AutoCompleteCount &&
               AutoCompleteSearchEditCount == other.AutoCompleteSearchEditCount &&
               AutoCompleteTextAreaCount == other.AutoCompleteTextAreaCount &&
               AutoCompletePopupFieldCount == other.AutoCompletePopupFieldCount &&
               AutoCompleteCandidateListFieldCount == other.AutoCompleteCandidateListFieldCount &&
               CandidateListCount == other.CandidateListCount &&
               TreeSelectCount == other.TreeSelectCount &&
               CascaderCount == other.CascaderCount &&
               MenuCount == other.MenuCount &&
               MenuItemCount == other.MenuItemCount &&
               NavMenuItemHeaderCount == other.NavMenuItemHeaderCount;
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

    private static bool HasFieldValue(object target, string declaringTypeName, string fieldName)
    {
        var type = target.GetType();
        while (type is not null)
        {
            if (type.FullName == declaringTypeName)
            {
                var field = type.GetField(fieldName, BindingFlags.Instance | BindingFlags.NonPublic);
                return field?.GetValue(target) is not null;
            }
            type = type.BaseType;
        }
        return false;
    }
}

internal sealed record SourceXamlStats(
    string SourcePath,
    bool IsAvailable,
    int AntDesignIconProviderCount,
    int SpaceCount,
    int CompactSpaceCount,
    int CompactSpaceFillerCount,
    int CompactSpaceAddOnCount,
    int IconPresenterCount,
    int IconGalleryCount,
    int LineEditDirectCount,
    int SearchEditCount,
    int TextAreaCount,
    int ButtonCount,
    int ToggleIconButtonCount,
    int SelectCount,
    int TreeSelectCount,
    int CascaderCount,
    int MenuCount,
    int MenuItemCount,
    int ShowCaseItemCount)
{
    private const string AtomNamespace = "https://atomui.net";
    private const string GalleryNamespace = "https://atomui.net/oss-controls/gallery";

    public static SourceXamlStats Read(string relativePath)
    {
        var sourcePath = Path.GetFullPath(relativePath);
        if (!File.Exists(sourcePath))
        {
            return new SourceXamlStats(sourcePath, false, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0);
        }

        var text     = File.ReadAllText(sourcePath);
        var document = XDocument.Load(sourcePath, LoadOptions.None);
        return new SourceXamlStats(
            sourcePath,
            true,
            CountText(text, "AntDesignIconProvider"),
            CountElements(document, AtomNamespace, "Space"),
            CountElements(document, AtomNamespace, "CompactSpace"),
            CountElements(document, AtomNamespace, "CompactSpaceFiller"),
            CountElements(document, AtomNamespace, "CompactSpaceAddOn"),
            CountElements(document, AtomNamespace, "IconPresenter"),
            CountElements(document, GalleryNamespace, "IconGallery"),
            CountElements(document, AtomNamespace, "LineEdit"),
            CountElements(document, AtomNamespace, "SearchEdit"),
            CountElements(document, AtomNamespace, "TextArea"),
            CountElements(document, AtomNamespace, "Button"),
            CountElements(document, AtomNamespace, "ToggleIconButton"),
            CountElements(document, AtomNamespace, "Select"),
            CountElements(document, AtomNamespace, "TreeSelect"),
            CountElements(document, AtomNamespace, "Cascader"),
            CountElements(document, AtomNamespace, "Menu"),
            CountElements(document, AtomNamespace, "MenuItem"),
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
        builder.AppendLine("| Source | AntDesignIconProvider | Space | CompactSpace | CompactSpaceFiller | CompactSpaceAddOn | IconPresenter | IconGallery | LineEdit direct | SearchEdit | LineEdit total | TextArea | Button | ToggleIconButton | Select | TreeSelect | Cascader | Menu | MenuItem | ShowCaseItem |");
        builder.AppendLine("| --- | ---: | ---: | ---: | ---: | ---: | ---: | ---: | ---: | ---: | ---: | ---: | ---: | ---: | ---: | ---: | ---: | ---: | ---: | ---: |");
        builder.Append("| `");
        builder.Append(SourcePath);
        builder.Append("` | ");
        builder.Append(AntDesignIconProviderCount.ToString(CultureInfo.InvariantCulture));
        builder.Append(" | ");
        builder.Append(SpaceCount.ToString(CultureInfo.InvariantCulture));
        builder.Append(" | ");
        builder.Append(CompactSpaceCount.ToString(CultureInfo.InvariantCulture));
        builder.Append(" | ");
        builder.Append(CompactSpaceFillerCount.ToString(CultureInfo.InvariantCulture));
        builder.Append(" | ");
        builder.Append(CompactSpaceAddOnCount.ToString(CultureInfo.InvariantCulture));
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
        builder.Append(ButtonCount.ToString(CultureInfo.InvariantCulture));
        builder.Append(" | ");
        builder.Append(ToggleIconButtonCount.ToString(CultureInfo.InvariantCulture));
        builder.Append(" | ");
        builder.Append(SelectCount.ToString(CultureInfo.InvariantCulture));
        builder.Append(" | ");
        builder.Append(TreeSelectCount.ToString(CultureInfo.InvariantCulture));
        builder.Append(" | ");
        builder.Append(CascaderCount.ToString(CultureInfo.InvariantCulture));
        builder.Append(" | ");
        builder.Append(MenuCount.ToString(CultureInfo.InvariantCulture));
        builder.Append(" | ");
        builder.Append(MenuItemCount.ToString(CultureInfo.InvariantCulture));
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
