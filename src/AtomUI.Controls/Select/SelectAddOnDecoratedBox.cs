using Avalonia;

namespace AtomUI.Controls;

internal class SelectAddOnDecoratedBox : AddOnDecoratedBox
{
    public static readonly StyledProperty<SelectMode> ModeProperty =
       Select.ModeProperty.AddOwner<SelectAddOnDecoratedBox>();
    
    internal static readonly DirectProperty<SelectAddOnDecoratedBox, bool> IsSelectionEmptyProperty =
        AvaloniaProperty.RegisterDirect<SelectAddOnDecoratedBox, bool>(
            nameof(IsSelectionEmpty),
            o => o.IsSelectionEmpty,
            (o, v) => o.IsSelectionEmpty = v);
    
    public SelectMode Mode
    {
        get => GetValue(ModeProperty);
        set => SetValue(ModeProperty, value);
    }
    
    private bool _isSelectionEmpty = true;

    internal bool IsSelectionEmpty
    {
        get => _isSelectionEmpty;
        set => SetAndRaise(IsSelectionEmptyProperty, ref _isSelectionEmpty, value);
    }
}