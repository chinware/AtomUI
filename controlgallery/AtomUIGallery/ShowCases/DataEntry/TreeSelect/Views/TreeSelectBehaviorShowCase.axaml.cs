using AtomUI.Controls;
using AtomUI.Desktop.Controls;

namespace AtomUIGallery.ShowCases.TreeSelect;

public partial class TreeSelectBehaviorShowCase : GalleryReactiveUserControl<TreeSelectViewModel>
{
    public TreeSelectBehaviorShowCase()
    {
        InitializeComponent();
    }

    private void HandlePlacementOptionCheckedChanged(object? sender, OptionCheckedChangedEventArgs args)
    {
        if (DataContext is not TreeSelectViewModel viewModel)
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
