using AtomUI.IconPkg;
using AtomUI.IconPkg.AntDesign;
using Avalonia.Layout;
using Avalonia.Markup.Xaml;
using Avalonia.Media;

namespace AtomUI.Desktop.Controls.Utils;

public class IconProvider : MarkupExtension
{
    public static IBrush? DefaultFilledColor { get; set; }
    public static IBrush? DefaultPrimaryFilledColor { get; set; }
    public static IBrush? DefaultSecondaryFilledColor { get; set; }
    public string Kind { get; set; }

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
        Kind = string.Empty;
    }

    public IconProvider(string kind)
    {
        Kind = kind;
    }

    public override object ProvideValue(IServiceProvider serviceProvider)
    {
        var icon = AntDesignIconPackage.Current.BuildIcon(Kind) ?? new Icon();
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
}