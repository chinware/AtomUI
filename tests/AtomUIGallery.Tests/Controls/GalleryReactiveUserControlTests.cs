using System;
using System.Collections.Generic;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using Avalonia.Interactivity;
using AtomUIGallery.Controls;
using ReactiveUI;
using Shouldly;
using Xunit;

namespace AtomUIGallery.Tests.Controls;

public class GalleryReactiveUserControlTests
{
    static GalleryReactiveUserControlTests()
    {
        AvaloniaTestApp.EnsureInitialized();
    }

    [Fact]
    public void WhenActivated_Runs_Callback_Once_Per_Load_And_Disposes_On_Unload()
    {
        var control = new TestGalleryReactiveUserControl();

        control.WhenActivated(disposables =>
        {
            control.ActivatedCount++;
            disposables.Add(Disposable.Create(() => control.DeactivatedCount++));
        });

        control.RaiseLoaded();
        control.RaiseLoaded();

        control.ActivatedCount.ShouldBe(1);
        control.DeactivatedCount.ShouldBe(0);

        control.RaiseUnloaded();
        control.RaiseUnloaded();

        control.ActivatedCount.ShouldBe(1);
        control.DeactivatedCount.ShouldBe(1);

        control.RaiseLoaded();
        control.ActivatedCount.ShouldBe(2);
        control.DeactivatedCount.ShouldBe(1);
    }

    [Fact]
    public void Activates_Current_ViewModel_Only_While_Loaded()
    {
        var control   = new TestGalleryReactiveUserControl();
        var viewModel = new ActivatableViewModel();

        control.ViewModel = viewModel;

        control.RaiseLoaded();
        control.RaiseLoaded();

        viewModel.ActivatedCount.ShouldBe(1);
        viewModel.DeactivatedCount.ShouldBe(0);

        control.RaiseUnloaded();
        control.RaiseUnloaded();

        viewModel.ActivatedCount.ShouldBe(1);
        viewModel.DeactivatedCount.ShouldBe(1);
    }

    [Fact]
    public void Switches_Active_ViewModel_While_Loaded()
    {
        var control = new TestGalleryReactiveUserControl();
        var first   = new ActivatableViewModel();
        var second  = new ActivatableViewModel();

        control.ViewModel = first;
        control.RaiseLoaded();

        control.ViewModel = second;

        first.ActivatedCount.ShouldBe(1);
        first.DeactivatedCount.ShouldBe(1);
        second.ActivatedCount.ShouldBe(1);
        second.DeactivatedCount.ShouldBe(0);

        control.RaiseUnloaded();

        second.ActivatedCount.ShouldBe(1);
        second.DeactivatedCount.ShouldBe(1);
    }

    [Fact]
    public void Does_Not_Activate_New_ViewModel_During_Unload_Reentrancy()
    {
        var control = new TestGalleryReactiveUserControl();
        var first   = new ActivatableViewModel();
        var second  = new ActivatableViewModel();

        first.Deactivating = () => control.ViewModel = second;
        control.ViewModel  = first;

        control.RaiseLoaded();
        control.RaiseUnloaded();

        first.ActivatedCount.ShouldBe(1);
        first.DeactivatedCount.ShouldBe(1);
        second.ActivatedCount.ShouldBe(0);
        second.DeactivatedCount.ShouldBe(0);

        control.RaiseLoaded();

        second.ActivatedCount.ShouldBe(1);
        second.DeactivatedCount.ShouldBe(0);
    }

    [Fact]
    public void Syncs_ViewModel_And_DataContext_When_Gallery_Owns_Current_Value()
    {
        var control   = new TestGalleryReactiveUserControl();
        var viewModel = new object();

        control.ViewModel.ShouldBeNull();
        control.DataContext.ShouldBeNull();

        control.ViewModel = viewModel;

        control.DataContext.ShouldBeSameAs(viewModel);

        control.DataContext = null;

        control.ViewModel.ShouldBeNull();
    }

    [Fact]
    public void Deactivates_ViewModel_Before_View_Disposables_On_Unload()
    {
        var control   = new TestGalleryReactiveUserControl();
        var viewModel = new ActivatableViewModel();
        var order     = new List<string>();

        control.ViewModel = viewModel;
        control.WhenActivated(disposables =>
        {
            order.Add("view-activate");
            disposables.Add(Disposable.Create(() => order.Add("view-dispose")));
        });
        viewModel.Activator.Deactivated.Subscribe(_ => order.Add("vm-deactivate"));

        control.RaiseLoaded();
        control.RaiseUnloaded();

        order.ShouldContain("view-activate");
        order.ShouldContain("vm-deactivate");
        order.ShouldContain("view-dispose");
        order.IndexOf("vm-deactivate").ShouldBeLessThan(order.IndexOf("view-dispose"));
    }

    [Fact]
    public void Disposing_Active_WhenActivated_Registration_Disposes_Its_Current_Activation_Scope()
    {
        var control       = new TestGalleryReactiveUserControl();
        var disposedCount = 0;

        control.RaiseLoaded();

        var registration = control.WhenActivated(disposables =>
        {
            disposables.Add(Disposable.Create(() => disposedCount++));
        });

        disposedCount.ShouldBe(0);

        registration.Dispose();

        disposedCount.ShouldBe(1);

        control.RaiseUnloaded();

        disposedCount.ShouldBe(1);
    }

    private sealed class TestGalleryReactiveUserControl : GalleryReactiveUserControl<object>
    {
        public int ActivatedCount { get; set; }

        public int DeactivatedCount { get; set; }

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
            Activator.Activated.Subscribe(_ => ActivatedCount++);
            Activator.Deactivated.Subscribe(_ =>
            {
                DeactivatedCount++;
                Deactivating?.Invoke();
            });
        }
    }
}
