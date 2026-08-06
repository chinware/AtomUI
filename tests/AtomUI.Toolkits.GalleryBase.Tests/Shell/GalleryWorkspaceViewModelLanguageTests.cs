using AtomUI.Localization;
using AtomUI.Controls;
using AtomUI.Toolkits.GalleryBase.Configuration;
using AtomUI.Toolkits.GalleryBase.Shell;
using Avalonia;
using Avalonia.Controls;
using ReactiveUI;
using Shouldly;
using Xunit;

namespace AtomUI.Toolkits.GalleryBase.Tests.Shell;

public class GalleryWorkspaceViewModelLanguageTests
{
    static GalleryWorkspaceViewModelLanguageTests()
    {
        AvaloniaTestApp.EnsureInitialized();
    }

    [Fact]
    public void Language_Commands_Use_New_Language_Manager_And_Refresh_State()
    {
        var application = Application.Current.ShouldNotBeNull();
        var languageManager = application.GetLanguageManager().ShouldNotBeNull();
        languageManager.ChangeLanguage(LanguageTags.EnUS);
        var viewModel = new GalleryWorkspaceViewModel(CreateConfiguration());
        try
        {
            viewModel.IsEnUS.ShouldBeTrue();
            viewModel.SwitchToZhCNCommand.Execute().Subscribe();
            languageManager.Current.CurrentLanguage.ShouldBe(LanguageTags.ZhCN);
            viewModel.IsZhCN.ShouldBeTrue();
            viewModel.IsZhTW.ShouldBeFalse();

            viewModel.SwitchToZhTWCommand.Execute().Subscribe();
            languageManager.Current.CurrentLanguage.ShouldBe(LanguageTags.ZhTW);
            viewModel.IsZhTW.ShouldBeTrue();
            viewModel.IsZhCN.ShouldBeFalse();
        }
        finally
        {
            viewModel.Dispose();
            languageManager.ChangeLanguage(LanguageTags.EnUS);
        }
    }

    [Fact]
    public void Dispose_Unsubscribes_From_Language_Changes()
    {
        var application = Application.Current.ShouldNotBeNull();
        var languageManager = application.GetLanguageManager().ShouldNotBeNull();
        languageManager.ChangeLanguage(LanguageTags.ZhTW);
        var viewModel = new GalleryWorkspaceViewModel(CreateConfiguration());
        viewModel.IsZhTW.ShouldBeTrue();

        viewModel.Dispose();
        languageManager.ChangeLanguage(LanguageTags.EnUS);

        viewModel.IsZhTW.ShouldBeTrue();
        viewModel.IsEnUS.ShouldBeFalse();
    }

    private static GalleryBaseConfiguration CreateConfiguration()
    {
        var options = new GalleryBaseOptions();
        options.Navigation.DefaultRoute = "Overview";
        options.Navigation.AddPage("Overview", "Overview");
        options.Routes.Map(
            "Overview",
            screen => new TestRouteViewModel(screen),
            () => new TestRouteView());
        return options.BuildConfiguration();
    }

    private sealed class TestRouteViewModel(IScreen hostScreen) : ReactiveObject, IRoutableViewModel
    {
        public string? UrlPathSegment => "Overview";

        public IScreen HostScreen { get; } = hostScreen;
    }

    private sealed class TestRouteView : UserControl, IViewFor<TestRouteViewModel>
    {
        public static readonly StyledProperty<TestRouteViewModel?> ViewModelProperty =
            AvaloniaProperty.Register<TestRouteView, TestRouteViewModel?>(nameof(ViewModel));

        public TestRouteViewModel? ViewModel
        {
            get => GetValue(ViewModelProperty);
            set => SetValue(ViewModelProperty, value);
        }

        object? IViewFor.ViewModel
        {
            get => ViewModel;
            set => ViewModel = (TestRouteViewModel?)value;
        }
    }
}
