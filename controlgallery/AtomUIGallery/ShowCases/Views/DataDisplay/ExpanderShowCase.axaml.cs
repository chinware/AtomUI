using AtomUIGallery.ShowCases.ViewModels;
using ReactiveUI;
using ReactiveUI.Avalonia;

namespace AtomUIGallery.ShowCases.Views;

public partial class ExpanderShowCase : ReactiveUserControl<ExpanderViewModel>
{
    public ExpanderShowCase()
    {
        this.WhenActivated(disposables =>
        {
            if (DataContext is ExpanderViewModel viewModel)
            {
                ExpandButtonPosGroup.OptionCheckedChanged       += viewModel.HandleExpandButtonPosOptionCheckedChanged;
                ExpandDirectionOptionGroup.OptionCheckedChanged += viewModel.HandleExpandDirectionOptionCheckedChanged;
            }
        });
        InitializeComponent();
    }
}