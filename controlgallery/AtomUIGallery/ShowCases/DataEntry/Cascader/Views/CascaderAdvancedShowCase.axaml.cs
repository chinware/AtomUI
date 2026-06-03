using AtomUI.Controls;
using AtomUI.Desktop.Controls;
using ReactiveUI.Avalonia;

namespace AtomUIGallery.ShowCases.Cascader;

public partial class CascaderAdvancedShowCase : ReactiveUserControl<CascaderViewModel>
{
    public CascaderAdvancedShowCase()
    {
        InitializeComponent();
    }

    private void HandlePlacementOptionCheckedChanged(object? sender, OptionCheckedChangedEventArgs args)
    {
        if (DataContext is not CascaderViewModel viewModel)
        {
            return;
        }

        viewModel.Placement = args.Index switch
        {
            0 => SelectPopupPlacement.TopEdgeAlignedLeft,
            1 => SelectPopupPlacement.TopEdgeAlignedRight,
            2 => SelectPopupPlacement.BottomEdgeAlignedLeft,
            _ => SelectPopupPlacement.BottomEdgeAlignedRight
        };
    }
}
