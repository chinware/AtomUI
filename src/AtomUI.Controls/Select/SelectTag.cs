using Avalonia;

namespace AtomUI.Controls;

internal class SelectTag : Tag
{
    public static readonly StyledProperty<SizeType> SizeTypeProperty =
        SizeTypeAwareControlProperty.SizeTypeProperty.AddOwner<SelectTag>();
    
    public SizeType SizeType
    {
        get => GetValue(SizeTypeProperty);
        set => SetValue(SizeTypeProperty, value);
    }
    
    public SelectOption? Option { get; set; }
    
}