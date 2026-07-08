using Avalonia;
using Avalonia.Controls;

namespace AtomUI.Desktop.Controls;

internal class NavMenuPopupFrame : Border
{
    public static readonly StyledProperty<double> PopupMaxWidthProperty =
        AvaloniaProperty.Register<NavMenuPopupFrame, double>(
            nameof(PopupMaxWidth),
            double.PositiveInfinity);

    public static readonly StyledProperty<double> PopupMaxHeightProperty =
        AvaloniaProperty.Register<NavMenuPopupFrame, double>(
            nameof(PopupMaxHeight),
            double.PositiveInfinity);

    public double PopupMaxWidth
    {
        get => GetValue(PopupMaxWidthProperty);
        set => SetValue(PopupMaxWidthProperty, value);
    }

    public double PopupMaxHeight
    {
        get => GetValue(PopupMaxHeightProperty);
        set => SetValue(PopupMaxHeightProperty, value);
    }

    static NavMenuPopupFrame()
    {
        AffectsMeasure<NavMenuPopupFrame>(
            PopupMaxWidthProperty,
            PopupMaxHeightProperty,
            PaddingProperty,
            BorderThicknessProperty);
    }

    protected override Size MeasureOverride(Size availableSize)
    {
        if (Child is not { } child)
        {
            return ResolveFrameSize(default, availableSize);
        }

        var chrome = GetChromeThickness();
        child.Measure(Deflate(new Size(double.PositiveInfinity, double.PositiveInfinity), chrome));

        var naturalSize = Inflate(child.DesiredSize, chrome);
        var frameSize   = ResolveFrameSize(naturalSize, availableSize);
        var childSize   = Deflate(frameSize, chrome);

        if (ShouldRemeasureChild(naturalSize, frameSize))
        {
            child.Measure(childSize);
            frameSize = ResolveFrameSize(Inflate(child.DesiredSize, chrome), availableSize);
        }

        return frameSize;
    }

    private Size ResolveFrameSize(Size desiredSize, Size availableSize)
    {
        var width = ResolveLength(
            desiredSize.Width,
            MinWidth,
            PopupMaxWidth,
            availableSize.Width);
        var height = ResolveLength(
            desiredSize.Height,
            MinHeight,
            PopupMaxHeight,
            availableSize.Height);

        return new Size(width, height);
    }

    private Thickness GetChromeThickness()
    {
        var padding         = Padding;
        var borderThickness = BorderThickness;

        return new Thickness(
            padding.Left + borderThickness.Left,
            padding.Top + borderThickness.Top,
            padding.Right + borderThickness.Right,
            padding.Bottom + borderThickness.Bottom);
    }

    private static bool ShouldRemeasureChild(Size naturalSize, Size frameSize)
    {
        return frameSize.Width < naturalSize.Width ||
               frameSize.Height < naturalSize.Height ||
               frameSize.Width > naturalSize.Width ||
               frameSize.Height > naturalSize.Height;
    }

    private static double ResolveLength(double desired, double min, double max, double available)
    {
        var value = NormalizeLength(desired, 0);
        value = Math.Max(value, NormalizeLength(min, 0));

        var effectiveMax = NormalizeLength(max, double.PositiveInfinity);
        if (double.IsFinite(available))
        {
            effectiveMax = Math.Min(effectiveMax, Math.Max(available, 0));
        }

        return Math.Min(value, effectiveMax);
    }

    private static Size Deflate(Size size, Thickness thickness)
    {
        return new Size(
            DeflateLength(size.Width, thickness.Left + thickness.Right),
            DeflateLength(size.Height, thickness.Top + thickness.Bottom));
    }

    private static Size Inflate(Size size, Thickness thickness)
    {
        return new Size(
            InflateLength(size.Width, thickness.Left + thickness.Right),
            InflateLength(size.Height, thickness.Top + thickness.Bottom));
    }

    private static double DeflateLength(double length, double amount)
    {
        return double.IsFinite(length) ? Math.Max(0, length - amount) : double.PositiveInfinity;
    }

    private static double InflateLength(double length, double amount)
    {
        return double.IsFinite(length) ? length + amount : double.PositiveInfinity;
    }

    private static double NormalizeLength(double value, double fallback)
    {
        return double.IsNaN(value) ? fallback : value;
    }
}
