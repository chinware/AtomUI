﻿using AtomUI.IconPkg;
using AtomUI.IconPkg.AntDesign;
using Avalonia.Layout;
using Avalonia.Markup.Xaml;
using Avalonia.Media;

namespace AtomUI.Controls.Utils;

public class IconProvider : MarkupExtension
{
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
        icon.SetCurrentValue(Icon.NormalFilledBrushProperty, NormalFilledColor);
        icon.SetCurrentValue(Icon.ActiveFilledBrushProperty, ActiveFilledColor);
        icon.SetCurrentValue(Icon.SelectedFilledBrushProperty, SelectedFilledColor);
        icon.SetCurrentValue(Icon.DisabledFilledBrushProperty, DisabledFilledColor);
        icon.SetCurrentValue(Icon.PrimaryFilledBrushProperty, SelectedFilledColor);
        icon.SetCurrentValue(Icon.DisabledFilledBrushProperty, PrimaryFilledColor);
        icon.SetCurrentValue(Icon.SecondaryFilledBrushProperty, SecondaryFilledColor);

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