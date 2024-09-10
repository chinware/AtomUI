using AtomUI.Controls;
using Avalonia;
using Avalonia.Controls;

namespace AtomUI.Demo.Desktop.ShowCase;

public partial class ExpanderShowCase : UserControl
{
    public static readonly StyledProperty<ExpanderIconPosition> ToggleIconPositionProperty =
        AvaloniaProperty.Register<ExpanderShowCase, ExpanderIconPosition>(nameof(ToggleIconPosition));

    public ExpanderIconPosition ToggleIconPosition
    {
        get => GetValue(ToggleIconPositionProperty);
        set => SetValue(ToggleIconPositionProperty, value);
    }

    public static readonly StyledProperty<ExpandDirection> ExpandDirectionProperty =
        AvaloniaProperty.Register<ExpanderShowCase, ExpandDirection>(nameof(ExpandDirection));

    public ExpandDirection ExpandDirection
    {
        get => GetValue(ExpandDirectionProperty);
        set => SetValue(ExpandDirectionProperty, value);
    }

    public ExpanderShowCase()
    {
        InitializeComponent();
        DataContext                                     =  this;
        ExpandButtonPosGroup.OptionCheckedChanged       += HandleExpandButtonPosOptionCheckedChanged;
        ExpandDirectionOptionGroup.OptionCheckedChanged += HandleExpandDirectionOptionCheckedChanged;
    }

    private void HandleExpandButtonPosOptionCheckedChanged(object? sender, OptionCheckedChangedEventArgs args)
    {
        if (args.Index == 0)
        {
            ToggleIconPosition = ExpanderIconPosition.Start;
        }
        else if (args.Index == 1)
        {
            ToggleIconPosition = ExpanderIconPosition.End;
        }
    }

    private void HandleExpandDirectionOptionCheckedChanged(object? sender, OptionCheckedChangedEventArgs args)
    {
        if (args.Index == 0)
        {
            ExpandDirection = ExpandDirection.Down;
        }
        else if (args.Index == 1)
        {
            ExpandDirection = ExpandDirection.Up;
        }
        else if (args.Index == 2)
        {
            ExpandDirection = ExpandDirection.Left;
        }
        else if (args.Index == 3)
        {
            ExpandDirection = ExpandDirection.Right;
        }
    }
}