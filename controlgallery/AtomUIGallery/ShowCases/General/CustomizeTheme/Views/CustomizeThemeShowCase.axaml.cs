using ReactiveUI;
using ReactiveUI.Avalonia;

namespace AtomUIGallery.ShowCases.CustomizeTheme;

public partial class CustomizeThemeShowCase : ReactiveUserControl<CustomizeThemeViewModel>
{
    public const string LanguageId = nameof(CustomizeThemeShowCase);

    public CustomizeThemeShowCase()
    {
        this.WhenActivated(disposables => { });
        InitializeComponent();
    }
}
