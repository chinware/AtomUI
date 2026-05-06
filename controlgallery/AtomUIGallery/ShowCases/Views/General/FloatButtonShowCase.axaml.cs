using AtomUIGallery.ShowCases.ViewModels;
using ReactiveUI;
using ReactiveUI.Avalonia;

namespace AtomUIGallery.ShowCases.Views;

public partial class FloatButtonShowCase : ReactiveUserControl<FloatButtonViewModel>
{
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
