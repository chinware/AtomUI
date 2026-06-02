using ReactiveUI.Avalonia;

namespace AtomUIGallery.ShowCases.NumberUpDown;

public partial class NumberUpDownShowCase : ReactiveUserControl<NumberUpDownViewModel>
{
    public const string LanguageId = nameof(NumberUpDownShowCase);

    public NumberUpDownShowCase()
    {
        InitializeComponent();
    }
}
