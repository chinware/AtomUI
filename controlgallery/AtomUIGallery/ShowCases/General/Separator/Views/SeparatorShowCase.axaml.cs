using ReactiveUI;
using ReactiveUI.Avalonia;

namespace AtomUIGallery.ShowCases.Separator;

public partial class SeparatorShowCase : ReactiveUserControl<SeparatorViewModel>
{
    public const string LanguageId = nameof(SeparatorShowCase);

    public SeparatorShowCase()
    {
        this.WhenActivated(disposables => { });
        InitializeComponent();
    }
}
