using AtomUIGallery.ShowCases.ViewModels;
using Avalonia.Controls;
using ReactiveUI;
using ReactiveUI.Avalonia;

namespace AtomUIGallery.ShowCases.Views;

public partial class DatePickerShowCase : ReactiveUserControl<DatePickerViewModel>
{
    public DatePickerShowCase()
    {
        this.WhenActivated(disposables =>
        {
            if (DataContext is DatePickerViewModel viewModel)
            {
                // PickerSizeTypeOptionGroup.OptionCheckedChanged  += viewModel.HandlePickerSizeTypeOptionCheckedChanged;
                // PickerPlacementOptionGroup.OptionCheckedChanged += viewModel.HandlePickerPlacementCheckedChanged;
                viewModel.PickerPlacement                       =  PlacementMode.BottomEdgeAlignedLeft;
            }
        });
        InitializeComponent();
    }
}