using ReactiveUI;
using ReactiveUI.Avalonia;

namespace AtomUIGallery.ShowCases.Spin;

public partial class SpinShowCase : ReactiveUserControl<SpinViewModel>
{
    public const string LanguageId = nameof(SpinShowCase);

    public SpinShowCase()
    {
        this.WhenActivated(disposables => { });
        InitializeComponent();
    }
}
