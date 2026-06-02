using ReactiveUI;
using ReactiveUI.Avalonia;

namespace AtomUIGallery.ShowCases.CustomizeTheme;

public partial class CustomizeThemeShowCase : ReactiveUserControl<CustomizeThemeViewModel>
{
    public CustomizeThemeShowCase()
    {
        this.WhenActivated(disposables => { });
        InitializeComponent();
    }
}
