using System.Runtime.CompilerServices;
using AtomUI.Desktop.Controls;
using AtomUI.Toolkits.GalleryBase.Controls;
using AtomUIGallery.ShowCases.DataGrid;
using Avalonia.Headless;
using Avalonia.Layout;
using Avalonia.Threading;
using Avalonia.VisualTree;
using ReactiveUI;
using Shouldly;
using Xunit;

namespace AtomUIGallery.Tests.DataGrid;

public class DataGridQueryRangeShowCaseTests
{
    [Fact]
    public void Fake_Remote_Source_Generates_Only_The_Requested_Range()
    {
        using var source = new GalleryRemoteDataGridSource(
            totalDataCount: 1_000_000,
            latency: TimeSpan.Zero);

        var first = AwaitSynchronously(source.FetchAsync(
            Request(start: 640, count: 32),
            TestContext.Current.CancellationToken));
        var second = AwaitSynchronously(source.FetchAsync(
            Request(start: 672, count: 32, expectedSnapshot: first.Snapshot),
            TestContext.Current.CancellationToken));

        first.StartIndex.ShouldBe(640);
        first.Entries.Length.ShouldBe(32);
        first.TotalEntryCount.ShouldBe(1_000_000);
        first.WindowDataCount.ShouldBe(1_000_000);
        first.TotalDataCount.ShouldBe(1_000_000);
        second.Snapshot.ShouldBe(first.Snapshot);
        source.GeneratedRowCount.ShouldBe(64);
        source.MaximumGeneratedRangeCount.ShouldBe(32);
    }

    [Fact]
    public void Fake_Remote_Source_Models_Cancellation_Failure_And_Snapshot_Expiry()
    {
        using var source = new GalleryRemoteDataGridSource(
            totalDataCount: 1_000_000,
            latency: TimeSpan.FromSeconds(5));
        using var cancellation = CancellationTokenSource.CreateLinkedTokenSource(
            TestContext.Current.CancellationToken);
        var cancelledFetch = source.FetchAsync(Request(), cancellation.Token).AsTask();

        cancellation.Cancel();

        Should.Throw<OperationCanceledException>(() => AwaitSynchronously(cancelledFetch));
        source.CancellationCount.ShouldBe(1);

        source.Latency = TimeSpan.Zero;
        source.FailNextRequest();
        Should.Throw<InvalidOperationException>(() =>
            AwaitSynchronously(source.FetchAsync(Request(), TestContext.Current.CancellationToken)));

        var stable = AwaitSynchronously(source.FetchAsync(
            Request(),
            TestContext.Current.CancellationToken));
        source.ExpireSnapshot();
        Should.Throw<DataGridSnapshotExpiredException>(() =>
            AwaitSynchronously(source.FetchAsync(
                Request(expectedSnapshot: stable.Snapshot),
                TestContext.Current.CancellationToken)));
    }

    [Fact]
    public void Repeated_Gallery_DataGrid_Ownership_Releases_All_Sources()
    {
        for (var cycle = 0; cycle < 3; cycle++)
        {
            var references = CreateDisposeAndRelease();

            CollectGarbage();

            references.ViewModel.IsAlive.ShouldBeFalse();
            references.LocalSource.IsAlive.ShouldBeFalse();
            references.RemoteSource.IsAlive.ShouldBeFalse();
        }
    }

    [Fact]
    public void Basic_Paging_ShowCase_Exposes_Natural_Horizontal_Overflow_From_The_First_Page()
    {
        AvaloniaTestApp.EnsureInitialized();
        var oldDeferredLoadingDisabled = GalleryShowCaseRuntimeOptions.IsDeferredLoadingDisabled;
        DataGridViewModel? viewModel = null;
        Avalonia.Controls.Window? window = null;
        try
        {
            GalleryShowCaseRuntimeOptions.IsDeferredLoadingDisabled = true;
            viewModel = new DataGridViewModel(new TestScreen());
            var showCase = new DataGridShowCase
            {
                DataContext = viewModel
            };
            window = new Avalonia.Controls.Window
            {
                Width = 1_100,
                Height = 720,
                Content = showCase,
                ShowInTaskbar = false
            };
            window.Show();
            PumpLayout(window);

            var dataGrid = showCase.GetVisualDescendants()
                                   .OfType<AtomUI.Desktop.Controls.DataGrid>()
                                   .First(grid => grid.Name == "BasicPagingCaseGrid");
            dataGrid.Width = 1_000;
            PumpLayout(window);

            var horizontalScrollBar = dataGrid.GetVisualDescendants()
                                              .OfType<Avalonia.Controls.Primitives.ScrollBar>()
                                              .Single(scrollBar => scrollBar.Orientation == Orientation.Horizontal);

            horizontalScrollBar.IsVisible.ShouldBeTrue();
            horizontalScrollBar.Maximum.ShouldBeGreaterThan(0);

            foreach (var pageStart in new long[] { 10, 20, 30, 40, 0 })
            {
                dataGrid.PageRequest = new DataGridPageRequest(pageStart, 10);
                PumpLayout(window);

                horizontalScrollBar.IsVisible.ShouldBeTrue();
                horizontalScrollBar.Maximum.ShouldBeGreaterThan(0);
            }
        }
        finally
        {
            if (window is not null)
            {
                window.Content = null;
                Dispatcher.UIThread.RunJobs();
                window.Close();
            }
            viewModel?.Dispose();
            GalleryShowCaseRuntimeOptions.IsDeferredLoadingDisabled = oldDeferredLoadingDisabled;
        }
    }

    [Fact]
    public void Repeated_Gallery_Navigation_Releases_ShowCase_Grid_Column_And_Source()
    {
        AvaloniaTestApp.EnsureInitialized();

        for (var cycle = 0; cycle < 3; cycle++)
        {
            var references = CreateAttachDetachAndRelease();

            CollectVisualGarbage();

            references.ShowCase.IsAlive.ShouldBeFalse();
            references.ViewModel.IsAlive.ShouldBeFalse();
            references.DataGrid.IsAlive.ShouldBeFalse();
            references.Column.IsAlive.ShouldBeFalse();
            references.Source.IsAlive.ShouldBeFalse();
        }
    }

    [MethodImpl(MethodImplOptions.NoInlining)]
    private static LifecycleReferences CreateDisposeAndRelease()
    {
        var viewModel = new DataGridViewModel(new TestScreen());
        DataGridShowCaseDataSources.EnsureBasicDataSource(viewModel);
        DataGridShowCaseDataSources.EnsureRemoteDataSource(viewModel);
        var localSource = viewModel.BasicCaseDataSource!.Source;
        var remoteSource = viewModel.RemoteDataSource!;

        viewModel.Dispose();
        remoteSource.IsDisposed.ShouldBeTrue();

        return new LifecycleReferences(
            new WeakReference(viewModel),
            new WeakReference(localSource),
            new WeakReference(remoteSource));
    }

    [MethodImpl(MethodImplOptions.NoInlining)]
    private static VisualLifecycleReferences CreateAttachDetachAndRelease()
    {
        var viewModel = new DataGridViewModel(new TestScreen());
        var showCase = new DataGridShowCase
        {
            DataContext = viewModel
        };
        var window = new Avalonia.Controls.Window
        {
            Width = 1_000,
            Height = 720,
            Content = showCase,
            ShowInTaskbar = false
        };
        window.Show();
        Dispatcher.UIThread.RunJobs();
        window.UpdateLayout();
        Dispatcher.UIThread.RunJobs();

        var dataGrid = showCase.GetVisualDescendants()
                               .OfType<AtomUI.Desktop.Controls.DataGrid>()
                               .First(grid => grid.Name == "BasicCaseGrid");
        var column = dataGrid.Columns[0];
        var source = dataGrid.ItemsSource;
        source.ShouldNotBeNull();

        window.Content = null;
        Dispatcher.UIThread.RunJobs();
        window.Close();
        AvaloniaHeadlessPlatform.ForceRenderTimerTick(1);
        Dispatcher.UIThread.RunJobs();
        viewModel.BasicCaseDataSource.ShouldBeNull();

        return new VisualLifecycleReferences(
            new WeakReference(showCase),
            new WeakReference(viewModel),
            new WeakReference(dataGrid),
            new WeakReference(column),
            new WeakReference(source));
    }

    private static DataGridFetchRequest Request(
        int start = 0,
        int count = 32,
        DataGridSnapshotId? expectedSnapshot = null) =>
        new(
            DataGridQuery.Empty,
            pageRequest: null,
            DataGridGroupExpansion.AllExpanded,
            new DataGridRange(start, count),
            expectedSnapshot,
            queryRevision: 0,
            dataGeneration: 0);

    private static T AwaitSynchronously<T>(ValueTask<T> operation) =>
        operation.GetAwaiter().GetResult();

    private static T AwaitSynchronously<T>(Task<T> operation) =>
        operation.GetAwaiter().GetResult();

    private static void PumpLayout(Avalonia.Controls.Window window)
    {
        for (var pass = 0; pass < 4; pass++)
        {
            Dispatcher.UIThread.RunJobs();
            window.UpdateLayout();
            AvaloniaHeadlessPlatform.ForceRenderTimerTick(1);
        }
        Dispatcher.UIThread.RunJobs();
    }

    private static void CollectGarbage()
    {
        for (var attempt = 0; attempt < 3; attempt++)
        {
            GC.Collect();
            GC.WaitForPendingFinalizers();
            GC.Collect();
        }
    }

    private static void CollectVisualGarbage()
    {
        for (var attempt = 0; attempt < 3; attempt++)
        {
            AvaloniaHeadlessPlatform.ForceRenderTimerTick(1);
            Thread.Sleep(TimeSpan.FromMilliseconds(10));
            Dispatcher.UIThread.RunJobs();
            GC.Collect();
            GC.WaitForPendingFinalizers();
            GC.Collect();
        }
    }

    private sealed record LifecycleReferences(
        WeakReference ViewModel,
        WeakReference LocalSource,
        WeakReference RemoteSource);

    private sealed record VisualLifecycleReferences(
        WeakReference ShowCase,
        WeakReference ViewModel,
        WeakReference DataGrid,
        WeakReference Column,
        WeakReference Source);

    private sealed class TestScreen : IScreen
    {
        public RoutingState Router { get; } = new();
    }
}
