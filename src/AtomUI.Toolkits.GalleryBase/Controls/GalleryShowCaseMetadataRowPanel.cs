using Avalonia;
using Avalonia.Controls;

namespace AtomUI.Toolkits.GalleryBase.Controls;

internal sealed class GalleryShowCaseMetadataRowPanel : Panel
{
    public static readonly StyledProperty<double> LabelWidthProperty =
        AvaloniaProperty.Register<GalleryShowCaseMetadataRowPanel, double>(nameof(LabelWidth));

    public static readonly StyledProperty<double> ValueWidthProperty =
        AvaloniaProperty.Register<GalleryShowCaseMetadataRowPanel, double>(nameof(ValueWidth));

    public static readonly StyledProperty<double> PairSpacingProperty =
        AvaloniaProperty.Register<GalleryShowCaseMetadataRowPanel, double>(nameof(PairSpacing));

    public double LabelWidth
    {
        get => GetValue(LabelWidthProperty);
        set => SetValue(LabelWidthProperty, value);
    }

    public double ValueWidth
    {
        get => GetValue(ValueWidthProperty);
        set => SetValue(ValueWidthProperty, value);
    }

    public double PairSpacing
    {
        get => GetValue(PairSpacingProperty);
        set => SetValue(PairSpacingProperty, value);
    }

    static GalleryShowCaseMetadataRowPanel()
    {
        AffectsMeasure<GalleryShowCaseMetadataRowPanel>(
            LabelWidthProperty,
            ValueWidthProperty,
            PairSpacingProperty);
        AffectsArrange<GalleryShowCaseMetadataRowPanel>(
            LabelWidthProperty,
            ValueWidthProperty,
            PairSpacingProperty);
    }

    protected override Size MeasureOverride(Size availableSize)
    {
        var label = GetVisibleChild(0);
        var value = GetVisibleChild(1);
        var heightConstraint = double.IsFinite(availableSize.Height)
            ? Math.Max(0, availableSize.Height)
            : double.PositiveInfinity;
        var widthConstraint = double.IsFinite(availableSize.Width)
            ? Math.Max(0, availableSize.Width)
            : double.PositiveInfinity;
        var spacing = ResolveSpacing(label, value, widthConstraint);

        label?.Measure(new Size(double.PositiveInfinity, heightConstraint));
        var labelDesiredWidth = ResolveLabelDesiredWidth(label);
        if (double.IsFinite(widthConstraint))
        {
            var allocation = Allocate(widthConstraint, labelDesiredWidth, spacing, label, value);
            value?.Measure(new Size(allocation.ValueWidth, heightConstraint));
        }
        else
        {
            value?.Measure(new Size(double.PositiveInfinity, heightConstraint));
        }

        var desiredWidth = labelDesiredWidth + spacing + Math.Max(GetDesiredWidth(value), NormalizeWidth(ValueWidth));
        if (double.IsFinite(widthConstraint))
        {
            desiredWidth = Math.Min(widthConstraint, desiredWidth);
        }

        return new Size(desiredWidth, Math.Max(GetDesiredHeight(label), GetDesiredHeight(value)));
    }

    protected override Size ArrangeOverride(Size finalSize)
    {
        var label = GetVisibleChild(0);
        var value = GetVisibleChild(1);
        var width = Math.Max(0, finalSize.Width);
        var height = Math.Max(0, finalSize.Height);
        var spacing = ResolveSpacing(label, value, width);
        var allocation = Allocate(width, ResolveLabelDesiredWidth(label), spacing, label, value);

        label?.Arrange(new Rect(0, 0, allocation.LabelWidth, height));
        value?.Arrange(new Rect(allocation.LabelWidth + spacing, 0, allocation.ValueWidth, height));

        return finalSize;
    }

    private (double LabelWidth, double ValueWidth) Allocate(
        double width,
        double labelDesiredWidth,
        double spacing,
        Control? label,
        Control? value)
    {
        if (label is null && value is null)
        {
            return default;
        }

        var contentWidth = Math.Max(0, width - spacing);
        if (label is null)
        {
            return (0, contentWidth);
        }

        if (value is null)
        {
            return (Math.Min(contentWidth, labelDesiredWidth), 0);
        }

        var labelMinWidth = Math.Min(NormalizeWidth(LabelWidth), contentWidth);
        var valuePreferredWidth = Math.Min(NormalizeWidth(ValueWidth), contentWidth);
        var labelWidth = Math.Clamp(labelDesiredWidth, labelMinWidth, contentWidth);

        if (valuePreferredWidth > 0 && contentWidth - labelWidth < valuePreferredWidth)
        {
            labelWidth = Math.Max(labelMinWidth, contentWidth - valuePreferredWidth);
        }

        labelWidth = Math.Min(labelWidth, contentWidth);
        return (labelWidth, Math.Max(0, contentWidth - labelWidth));
    }

    private Control? GetVisibleChild(int index)
    {
        return Children.Count > index && Children[index].IsVisible
            ? Children[index]
            : null;
    }

    private double ResolveLabelDesiredWidth(Control? label)
    {
        return Math.Max(GetDesiredWidth(label), NormalizeWidth(LabelWidth));
    }

    private double ResolveValueDesiredWidth(Control? value)
    {
        return Math.Max(GetDesiredWidth(value), NormalizeWidth(ValueWidth));
    }

    private double ResolveSpacing(Control? label, Control? value, double width)
    {
        if (label is null || value is null)
        {
            return 0;
        }

        return Math.Min(NormalizeWidth(PairSpacing), Math.Max(0, width));
    }

    private static double GetDesiredWidth(Control? control)
    {
        return control is null ? 0 : NormalizeWidth(control.DesiredSize.Width);
    }

    private static double GetDesiredHeight(Control? control)
    {
        return control is null ? 0 : NormalizeWidth(control.DesiredSize.Height);
    }

    private static double NormalizeWidth(double value)
    {
        return double.IsFinite(value) && value > 0 ? value : 0;
    }
}
