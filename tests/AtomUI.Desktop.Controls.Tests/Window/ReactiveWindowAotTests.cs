using System;
using System.IO;
using System.Reactive.Disposables;
using Avalonia.Interactivity;
using ReactiveUI;
using Shouldly;
using Xunit;

namespace AtomUI.Desktop.Controls.Tests.Window;

public class ReactiveWindowAotTests
{
    static ReactiveWindowAotTests()
    {
        AvaloniaTestApp.EnsureInitialized();
    }

    [Fact]
    public void ReactiveWindow_Does_Not_Call_ReactiveUI_WhenActivated()
    {
        var source = File.ReadAllText(GetRepoFile("src/AtomUI.Desktop.Controls/Window/ReactiveWindow.cs"));

        source.ShouldNotContain("RequiresUnreferencedCode");
        source.ShouldNotContain("WhenActivated");
    }

    [Fact]
    public void ReactiveWindow_Activates_Current_ViewModel_Only_While_Loaded()
    {
        var window    = new TestReactiveWindow();
        var viewModel = new ActivatableViewModel();

        window.ViewModel = viewModel;

        window.RaiseLoaded();
        window.RaiseLoaded();

        viewModel.ActivatedCount.ShouldBe(1);
        viewModel.DeactivatedCount.ShouldBe(0);

        window.RaiseUnloaded();
        window.RaiseUnloaded();

        viewModel.ActivatedCount.ShouldBe(1);
        viewModel.DeactivatedCount.ShouldBe(1);
    }

    [Fact]
    public void ReactiveWindow_Switches_Active_ViewModel_While_Loaded()
    {
        var window = new TestReactiveWindow();
        var first  = new ActivatableViewModel();
        var second = new ActivatableViewModel();

        window.ViewModel = first;
        window.RaiseLoaded();

        window.ViewModel = second;

        first.ActivatedCount.ShouldBe(1);
        first.DeactivatedCount.ShouldBe(1);
        second.ActivatedCount.ShouldBe(1);
        second.DeactivatedCount.ShouldBe(0);

        window.RaiseUnloaded();

        second.ActivatedCount.ShouldBe(1);
        second.DeactivatedCount.ShouldBe(1);
    }

    [Fact]
    public void ReactiveWindow_Does_Not_Activate_New_ViewModel_During_Unload_Reentrancy()
    {
        var window = new TestReactiveWindow();
        var first  = new ActivatableViewModel();
        var second = new ActivatableViewModel();

        first.Deactivating = () => window.ViewModel = second;
        window.ViewModel  = first;

        window.RaiseLoaded();
        window.RaiseUnloaded();

        first.ActivatedCount.ShouldBe(1);
        first.DeactivatedCount.ShouldBe(1);
        second.ActivatedCount.ShouldBe(0);
        second.DeactivatedCount.ShouldBe(0);

        window.RaiseLoaded();

        second.ActivatedCount.ShouldBe(1);
        second.DeactivatedCount.ShouldBe(0);
    }

    private static string GetRepoFile(string relativePath)
    {
        var directory = new DirectoryInfo(AppContext.BaseDirectory);
        while (directory is not null)
        {
            var candidate = Path.Combine(directory.FullName, relativePath);
            if (File.Exists(candidate))
            {
                return candidate;
            }

            directory = directory.Parent;
        }

        throw new FileNotFoundException($"Could not find repository file: {relativePath}");
    }

    private sealed class TestReactiveWindow : AtomUI.Desktop.Controls.ReactiveWindow<ActivatableViewModel>
    {
        public void RaiseLoaded()
        {
            OnLoaded(new RoutedEventArgs(LoadedEvent));
        }

        public void RaiseUnloaded()
        {
            OnUnloaded(new RoutedEventArgs(UnloadedEvent));
        }
    }

    private sealed class ActivatableViewModel : IActivatableViewModel
    {
        public ViewModelActivator Activator { get; } = new();

        public int ActivatedCount { get; private set; }

        public int DeactivatedCount { get; private set; }

        public Action? Deactivating { get; set; }

        public ActivatableViewModel()
        {
            this.WhenActivated(disposables =>
            {
                ActivatedCount++;
                disposables.Add(Disposable.Create(() =>
                {
                    DeactivatedCount++;
                    Deactivating?.Invoke();
                }));
            });
        }
    }
}
