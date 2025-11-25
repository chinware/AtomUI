using System.Diagnostics;
using Avalonia.Markup.Xaml;
using Avalonia.Media;

namespace AtomUI.Controls;

public abstract class IconProvider<TIconKind> : MarkupExtension
    where TIconKind : Enum
{
    public TIconKind? Kind { get; set; }

    // Filled 和 Outlined
    public IBrush? NormalFilledColor { get; set; }
    public IBrush? ActiveFilledColor { get; set; }
    public IBrush? SelectedFilledColor { get; set; }
    public IBrush? DisabledFilledColor { get; set; }

    // TwoTone
    public IBrush? PrimaryFilledColor { get; set; }
    public IBrush? SecondaryFilledColor { get; set; }

    public double Width { get; set; } = double.NaN;
    public double Height { get; set; } = double.NaN;
    public IconAnimation? Animation { get; set; }

    public IconProvider()
    {
    }

    public IconProvider(TIconKind kind)
    {
        Kind = kind;
    }

    public override object ProvideValue(IServiceProvider serviceProvider)
    {
        Debug.Assert(Kind != null);
        var icon = GetIcon(Kind);

        if (Animation != null)
        {
            icon.SetCurrentValue(Icon.LoadingAnimationProperty, Animation);
        }
        
        if (NormalFilledColor != null)
        {
            icon.NormalFilledBrush = NormalFilledColor;
        }
        if (ActiveFilledColor != null)
        {
            icon.ActiveFilledBrush = ActiveFilledColor;
        }
        if (SelectedFilledColor != null)
        {
            icon.SelectedFilledBrush = SelectedFilledColor;
        }
        
        if (DisabledFilledColor != null)
        {
            icon.DisabledFilledBrush = DisabledFilledColor;
        }
        
        if (PrimaryFilledColor != null)
        {
            icon.PrimaryFilledBrush = PrimaryFilledColor;
        }
        if (SecondaryFilledColor != null)
        {
            icon.SecondaryFilledBrush = SecondaryFilledColor;
        }
        
        if (!double.IsNaN(Width))
        {
            icon.Width = Width;
        }

        if (!double.IsNaN(Height))
        {
            icon.Height = Height;
        }

        return icon;
    }

    protected abstract Icon GetIcon(TIconKind kind);
}