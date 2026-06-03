using ReactiveUI;
using ReactiveUI.Avalonia;

namespace AtomUIGallery.ShowCases.Icon;

public partial class IconShowCase : ReactiveUserControl<IconViewModel>
{
    public const string LanguageId = nameof(IconShowCase);

    public IconShowCase()
    {
        this.WhenActivated(disposables => { });
        InitializeComponent();
    }
}
