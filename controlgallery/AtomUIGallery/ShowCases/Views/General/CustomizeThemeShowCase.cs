using AtomUIGallery.ShowCases.ViewModels;
using ReactiveUI;
using ReactiveUI.Avalonia;

namespace AtomUIGallery.ShowCases.Views;

public partial class CustomizeThemeShowCase : ReactiveUserControl<CustomizeThemeViewModel>
{
    public CustomizeThemeShowCase()
    {
        this.WhenActivated(disposables =>
        {
            if (DataContext is CustomizeThemeViewModel customizeThemeModel)
            {
            }
        });
        InitializeComponent();
    }
}