using System.Diagnostics;
using Avalonia.Layout;
using Avalonia.Markup.Xaml;
using Avalonia.Media;

namespace AtomUI.Controls;

public abstract class IconProvider<TIconKind> : MarkupExtension
    where TIconKind : Enum
{
    public static IBrush? DefaultFilledColor { get; set; }
    public static IBrush? DefaultPrimaryFilledColor { get; set; }
    public static IBrush? DefaultSecondaryFilledColor { get; set; }
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
    public IconAnimation Animation { get; set; } = IconAnimation.None;

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
        icon.SetCurrentValue(Icon.LoadingAnimationProperty, Animation);
        icon.SetCurrentValue(Icon.NormalFilledBrushProperty, NormalFilledColor ?? DefaultFilledColor);
        icon.SetCurrentValue(Icon.ActiveFilledBrushProperty, ActiveFilledColor ?? DefaultFilledColor);
        icon.SetCurrentValue(Icon.SelectedFilledBrushProperty, SelectedFilledColor ?? DefaultFilledColor);
        icon.SetCurrentValue(Icon.DisabledFilledBrushProperty, DisabledFilledColor ?? DefaultFilledColor);
        icon.SetCurrentValue(Icon.PrimaryFilledBrushProperty, PrimaryFilledColor ?? DefaultPrimaryFilledColor);
        icon.SetCurrentValue(Icon.SecondaryFilledBrushProperty, SecondaryFilledColor ?? DefaultSecondaryFilledColor);

        if (!double.IsNaN(Width))
        {
            icon.SetCurrentValue(Layoutable.WidthProperty, Width);
        }

        if (!double.IsNaN(Height))
        {
            icon.SetCurrentValue(Layoutable.HeightProperty, Height);
        }

        return icon;
    }

    protected abstract Icon GetIcon(TIconKind kind);
}