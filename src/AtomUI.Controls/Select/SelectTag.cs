using Avalonia;
using Avalonia.Controls.Primitives;

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

    protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
    {
        base.OnApplyTemplate(e);
        if (CloseButton != null)
        {
            CloseButton.IsPassthroughMouseEvent = true;
        }
    }
}