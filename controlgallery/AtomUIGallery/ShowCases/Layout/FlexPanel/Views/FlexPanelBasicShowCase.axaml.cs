using AtomUI.Controls;
using Avalonia.Layout;

namespace AtomUIGallery.ShowCases.FlexPanel;

public partial class FlexPanelBasicShowCase : GalleryReactiveUserControl<FlexPanelViewModel>
{
    public FlexPanelBasicShowCase()
    {
        InitializeComponent();
        SetupBasicItems();
        SetupEventHandlers();
        SetBasicDirection(FlexDirection.Row);
    }

    private void SetupBasicItems()
    {
        ConfigureBasicItem(BasicItem1);
        ConfigureBasicItem(BasicItem2);
        ConfigureBasicItem(BasicItem3);
        ConfigureBasicItem(BasicItem4);
    }

    private static void ConfigureBasicItem(Layoutable item)
    {
        Flex.SetGrow(item, 1);
    }

    private void SetupEventHandlers()
    {
        DirectionHorizontal.IsCheckedChanged += (_, _) =>
        {
            if (DirectionHorizontal.IsChecked == true)
            {
                SetBasicDirection(FlexDirection.Row);
            }
        };

        DirectionVertical.IsCheckedChanged += (_, _) =>
        {
            if (DirectionVertical.IsChecked == true)
            {
                SetBasicDirection(FlexDirection.Column);
            }
        };
    }

    private void SetBasicDirection(FlexDirection direction)
    {
        BasicFlexPanel.Direction = direction;
        BasicFlexPanel.AlignItems = direction == FlexDirection.Column
            ? AlignItems.FlexStart
            : AlignItems.Stretch;
        var grow = direction == FlexDirection.Row ? 1.0 : 0.0;
        Flex.SetGrow(BasicItem1, grow);
        Flex.SetGrow(BasicItem2, grow);
        Flex.SetGrow(BasicItem3, grow);
        Flex.SetGrow(BasicItem4, grow);
    }
}
