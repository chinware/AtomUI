using ReactiveUI;
using ReactiveUI.Avalonia;

namespace AtomUIGallery.ShowCases.Alert;

public partial class AlertShowCase : ReactiveUserControl<AlertViewModel>
{
    public const string LanguageId = nameof(AlertShowCase);

    public AlertShowCase()
    {
        this.WhenActivated(disposables => { });
        InitializeComponent();
    }
}
