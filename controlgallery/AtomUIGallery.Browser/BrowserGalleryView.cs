using AtomUI.Fonts.AlibabaPuHuiTi;
using AtomUI.Toolkits.GalleryBase.Shell;
using AtomUIGallery.Workspace.ViewModels;
using AtomUIGallery.Workspace.Views;
using Avalonia.Controls;
using Avalonia.Media;

namespace AtomUIGallery.Browser;

internal sealed class BrowserGalleryView : GalleryBrowserShellView
{
    private static readonly FontFamily s_appFontFamily =
        FontFamily.Parse($"fonts:AlibabaSans#Alibaba Sans, {AlibabaPuHuiTiFontConstants.FontFamily}, $Default");

    public BrowserGalleryView()
        : base(AtomUIGalleryModule.GetConfiguration(),
               _ => new WorkspaceWindowViewModel(),
               CreateNavigationView)
    {
        FontFamily = s_appFontFamily;
    }

    private static Control CreateNavigationView(GalleryWorkspaceViewModel workspaceViewModel)
    {
        var viewModel = (WorkspaceWindowViewModel)workspaceViewModel;
        return new CaseNavigation
        {
            Name      = "ShowCaseNavigation",
            ViewModel = viewModel.CaseNavigation
        };
    }
}
