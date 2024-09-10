using AtomUI.Controls;
using Avalonia;
using Avalonia.Controls;

namespace AtomUI.Demo.Desktop.ShowCase;

public partial class CollapseShowCase : UserControl
{
    public static readonly StyledProperty<CollapseExpandIconPosition> CollapseExpandIconPositionProperty =
        AvaloniaProperty.Register<CollapseShowCase, CollapseExpandIconPosition>(nameof(CollapseExpandIconPosition));

    public CollapseExpandIconPosition CollapseExpandIconPosition
    {
        get => GetValue(CollapseExpandIconPositionProperty);
        set => SetValue(CollapseExpandIconPositionProperty, value);
    }

    public CollapseShowCase()
    {
        InitializeComponent();
        DataContext = this;
        //ExpandButtonPosGroup.OptionCheckedChanged += HandleExpandButtonPosOptionCheckedChanged;
    }

    private void HandleExpandButtonPosOptionCheckedChanged(object? sender, OptionCheckedChangedEventArgs args)
    {
        if (args.Index == 0)
        {
            CollapseExpandIconPosition = CollapseExpandIconPosition.Start;
        }
        else if (args.Index == 1)
        {
            CollapseExpandIconPosition = CollapseExpandIconPosition.End;
        }
    }
}