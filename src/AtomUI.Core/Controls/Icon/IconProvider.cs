using System.Diagnostics;
using Avalonia.Data;
using Avalonia.Layout;
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
        
        icon.SetValue(Icon.NormalFilledBrushProperty, NormalFilledColor, NormalFilledColor != null ? BindingPriority.LocalValue : BindingPriority.Template);
        icon.SetValue(Icon.ActiveFilledBrushProperty, ActiveFilledColor, ActiveFilledColor != null ? BindingPriority.LocalValue : BindingPriority.Template);
        icon.SetValue(Icon.SelectedFilledBrushProperty, SelectedFilledColor, SelectedFilledColor != null ? BindingPriority.LocalValue : BindingPriority.Template);
        icon.SetValue(Icon.DisabledFilledBrushProperty, DisabledFilledColor, DisabledFilledColor != null ? BindingPriority.LocalValue : BindingPriority.Template);
        icon.SetValue(Icon.PrimaryFilledBrushProperty, PrimaryFilledColor, PrimaryFilledColor != null ? BindingPriority.LocalValue : BindingPriority.Template);
        icon.SetValue(Icon.SecondaryFilledBrushProperty, SecondaryFilledColor, SecondaryFilledColor != null ? BindingPriority.LocalValue : BindingPriority.Template);

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