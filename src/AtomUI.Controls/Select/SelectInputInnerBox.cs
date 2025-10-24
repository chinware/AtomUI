using Avalonia;

namespace AtomUI.Controls;

internal class SelectInputInnerBox : AddOnDecoratedInnerBox
{
    public static readonly StyledProperty<SelectMode> ModeProperty =
        AvaloniaProperty.Register<SelectInputInnerBox, SelectMode>(nameof(Mode));
    
    public SelectMode Mode
    {
        get => GetValue(ModeProperty);
        set => SetValue(ModeProperty, value);
    }
    
    internal SelectInput? OwnerInput;

    protected override void NotifyClearButtonClicked()
    {
        OwnerInput?.Clear();
    }

    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        base.OnPropertyChanged(change);
        if (change.Property == ModeProperty)
        {
            ConfigureEffectiveInnerBoxPadding();
        }
    }
    
    protected override void ConfigureEffectiveInnerBoxPadding()
    {
        if (Mode == SelectMode.Multiple)
        {
            SetCurrentValue(EffectiveInnerBoxPaddingProperty, new Thickness(InnerBoxPadding.Left / 2, InnerBoxPadding.Top, InnerBoxPadding.Right, InnerBoxPadding.Bottom));
        }
        else
        {
            SetCurrentValue(EffectiveInnerBoxPaddingProperty, InnerBoxPadding);
        }
    }
}