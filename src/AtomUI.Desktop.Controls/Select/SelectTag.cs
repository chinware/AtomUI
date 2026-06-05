using AtomUI.Controls;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.VisualTree;

namespace AtomUI.Desktop.Controls;

internal class SelectTag : Tag
{
    public static readonly StyledProperty<SizeType> SizeTypeProperty =
        SizeTypeControlProperty.SizeTypeProperty.AddOwner<SelectTag>();

    public SizeType SizeType
    {
        get => GetValue(SizeTypeProperty);
        set => SetValue(SizeTypeProperty, value);
    }

    public object? Item { get; set; }

    internal static bool IsCloseButtonSource(Control sourceControl)
    {
        var closeButton = sourceControl.FindAncestorOfType<IconButton>(includeSelf: true);
        return closeButton?.FindAncestorOfType<SelectTag>() != null;
    }

    protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
    {
        base.OnApplyTemplate(e);
        if (CloseButton != null)
        {
            CloseButton.IsPassthroughMouseEvent = true;
        }
    }
}

internal class SelectRemainInfoTag : SelectTag
{
    protected override Type StyleKeyOverride { get; } = typeof(SelectTag);

    public void SetRemainText(int remainCount)
    {
        Text = $"+ {remainCount} ...";
    }
}
