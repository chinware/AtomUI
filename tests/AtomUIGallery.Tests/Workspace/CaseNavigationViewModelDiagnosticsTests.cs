using System;
using AtomUIGallery.Controls;
using AtomUIGallery.Workspace.ViewModels;
using ReactiveUI;
using Shouldly;
using Xunit;

namespace AtomUIGallery.Tests.Workspace;

public class CaseNavigationViewModelDiagnosticsTests
{
    [Fact]
    public void Test_Navigate_Pages_Command_Disables_ShowCase_Deferred_Loading_Until_Stopped()
    {
        AvaloniaTestApp.EnsureInitialized();
        var oldValue = Environment.GetEnvironmentVariable(
            GalleryShowCaseRuntimeOptions.DisableDeferredLoadingEnvironmentVariable);
        Environment.SetEnvironmentVariable(
            GalleryShowCaseRuntimeOptions.DisableDeferredLoadingEnvironmentVariable,
            null);
        GalleryShowCaseRuntimeOptions.ResetDeferredLoadingDisabledOverride();
        var viewModel = new CaseNavigationViewModel(new TestScreen());

        try
        {
            viewModel.TestNavigatePagesCommand.Execute(TimeSpan.FromMilliseconds(300))
                     .Subscribe();

            GalleryShowCaseRuntimeOptions.IsDeferredLoadingDisabled.ShouldBeTrue();

            viewModel.StopTestNavigatePagesCommand.Execute()
                     .Subscribe();

            GalleryShowCaseRuntimeOptions.IsDeferredLoadingDisabled.ShouldBeFalse();
        }
        finally
        {
            Environment.SetEnvironmentVariable(
                GalleryShowCaseRuntimeOptions.DisableDeferredLoadingEnvironmentVariable,
                oldValue);
            GalleryShowCaseRuntimeOptions.ResetDeferredLoadingDisabledOverride();
        }
    }

    private sealed class TestScreen : IScreen
    {
        public RoutingState Router { get; } = new();
    }
}
