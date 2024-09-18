using AtomUI.Controls;
using Avalonia;
using Avalonia.Controls;

namespace AtomUI.Demo.Desktop.ShowCase;

public partial class DatePickerShowCase : UserControl
{
    public static readonly StyledProperty<SizeType> PickerSizeTypeProperty =
        AvaloniaProperty.Register<ButtonShowCase, SizeType>(nameof(PickerSizeType), SizeType.Middle);
    
    public static readonly StyledProperty<PlacementMode> PickerPlacementProperty =
        AvaloniaProperty.Register<ButtonShowCase, PlacementMode>(nameof(PickerPlacement), PlacementMode.BottomEdgeAlignedLeft);

    public SizeType PickerSizeType
    {
        get => GetValue(PickerSizeTypeProperty);
        set => SetValue(PickerSizeTypeProperty, value);
    }
    
    public PlacementMode PickerPlacement
    {
        get => GetValue(PickerPlacementProperty);
        set => SetValue(PickerPlacementProperty, value);
    }
    
    public DatePickerShowCase()
    {
        InitializeComponent();
        DataContext = this;

        PickerSizeTypeOptionGroup.OptionCheckedChanged  += HandlePickerSizeTypeOptionCheckedChanged;
        PickerPlacementOptionGroup.OptionCheckedChanged += HandlePickerPlacementCheckedChanged;
    }
    
    private void HandlePickerSizeTypeOptionCheckedChanged(object? sender, OptionCheckedChangedEventArgs args)
    {
        if (args.Index == 0)
        {
            PickerSizeType = SizeType.Large;
        }
        else if (args.Index == 1)
        {
            PickerSizeType = SizeType.Middle;
        }
        else
        {
            PickerSizeType = SizeType.Small;
        }
    }
    
    private void HandlePickerPlacementCheckedChanged(object? sender, OptionCheckedChangedEventArgs args)
    {
        if (args.Index == 0)
        {
            PickerPlacement = PlacementMode.TopEdgeAlignedLeft;
        }
        else if (args.Index == 1)
        {
            PickerPlacement = PlacementMode.TopEdgeAlignedRight;
        }
        else if (args.Index == 2)
        {
            PickerPlacement = PlacementMode.BottomEdgeAlignedLeft;
        }
        else
        {
            PickerPlacement = PlacementMode.BottomEdgeAlignedRight;
        }
    }
}