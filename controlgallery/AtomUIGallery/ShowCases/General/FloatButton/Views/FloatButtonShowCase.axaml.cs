using ReactiveUI;
using ReactiveUI.Avalonia;

namespace AtomUIGallery.ShowCases.FloatButton;

public partial class FloatButtonShowCase : ReactiveUserControl<FloatButtonViewModel>
{
    public const string LanguageId = nameof(FloatButtonShowCase);

    public FloatButtonShowCase()
    {
        this.WhenActivated(disposables =>
        {
            if (DataContext is FloatButtonViewModel vm)
            {
                vm.IsOpened = true;
            }
        });
        InitializeComponent();
    }
}
