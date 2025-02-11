using Avalonia;
using Avalonia.Controls;
using Avalonia.Layout;

namespace AtomUI.Controls;

public class BoxPanel : Panel
{
    #region 公共属性定义

    public static readonly StyledProperty<Orientation> OrientationProperty =
        AvaloniaProperty.Register<BoxPanel, Orientation>(nameof(Orientation), Orientation.Vertical);

    public static readonly StyledProperty<double> SpacingProperty =
        AvaloniaProperty.Register<BoxPanel, double>(nameof(Spacing), 0.0);

    public static readonly AttachedProperty<int> FlexProperty =
        AvaloniaProperty.RegisterAttached<BoxPanel, Control, int>("Flex", 0);

    #endregion

    public Orientation Orientation
    {
        get => GetValue(OrientationProperty);
        set => SetValue(OrientationProperty, value);
    }

    public double Spacing
    {
        get => GetValue(SpacingProperty);
        set => SetValue(SpacingProperty, value);
    }

    public static void SetFlex(Control element, int value)
    {
        element.SetValue(FlexProperty, value);
    }

    public static int GetFlex(Control element)
    {
        return element.GetValue(FlexProperty);
    }

    static BoxPanel()
    {
        AffectsMeasure<BoxPanel>(OrientationProperty, SpacingProperty, FlexProperty);
        AffectsArrange<BoxPanel>(OrientationProperty, SpacingProperty, FlexProperty);

        FlexProperty.Changed
            .AddClassHandler<Control>(OnFlexPropertyChanged);
    }

    private static void OnFlexPropertyChanged(Control control, AvaloniaPropertyChangedEventArgs e)
    {
        if (control.Parent is BoxPanel parentBoxPanel)
        {
            parentBoxPanel.InvalidateMeasure();
        }
    }

    public void AddSpacing(double size)
    {
        var spacer = new Border
        {
            Width  = Orientation == Orientation.Horizontal ? size : 0,
            Height = Orientation == Orientation.Vertical ? size : 0
        };
        Children.Add(spacer);
    }

    public void AddFlex(int flex)
    {
        var spacer = new Border();
        SetFlex(spacer, flex);
        Children.Add(spacer);
    }

    protected override Size MeasureOverride(Size availableSize)
    {
        double totalWidth      = 0;
        double totalHeight     = 0;
        double maxWidth        = 0;
        double maxHeight       = 0;
        double spacing         = Spacing;
        int    visibleChildren = 0;

        foreach (var child in Children)
        {
            if (!child.IsVisible)
            {
                continue;
            }
            
            child.Measure(availableSize);
            var desired = child.DesiredSize;

            if (Orientation == Orientation.Vertical)
            {
                totalHeight += desired.Height;
                maxWidth    =  Math.Max(maxWidth, desired.Width);
            }
            else
            {
                totalWidth += desired.Width;
                maxHeight  =  Math.Max(maxHeight, desired.Height);
            }

            visibleChildren++;
        }

        // Add spacing
        if (visibleChildren > 1)
        {
            if (Orientation == Orientation.Vertical)
            {
                totalHeight += spacing * (visibleChildren - 1);
            }
            else
            {
                totalWidth += spacing * (visibleChildren - 1);
            }
        }

        if (Orientation == Orientation.Vertical)
        {
            double desiredWidth = double.IsInfinity(availableSize.Width) ? maxWidth : availableSize.Width;
            return new Size(desiredWidth, totalHeight);
        }
        double desiredHeight = double.IsInfinity(availableSize.Height) ? maxHeight : availableSize.Height;
        return new Size(totalWidth, desiredHeight);
    }

    protected override Size ArrangeOverride(Size finalSize)
    {
        double offset            = 0;
        double spacing           = Spacing;
        double totalFlex         = 0;
        double originTotalWidth  = 0;
        double originTotalHeight = 0;
        int    visibleChildren   = 0;


        foreach (var child in Children)
        {
            if (!child.IsVisible)
            {
                continue;
            }
            visibleChildren++;
            int childFlex = GetFlex(child);
            totalFlex += childFlex;
            if (childFlex == 0)
            {
                if (Orientation == Orientation.Horizontal)
                {
                    originTotalWidth += child.DesiredSize.Width;
                }
                else
                {
                    originTotalHeight += child.DesiredSize.Height;
                }
            }
        }

        if (Orientation == Orientation.Horizontal)
        {
            originTotalWidth += spacing * (visibleChildren - 1);
        }
        else
        {
            originTotalHeight += spacing * (visibleChildren - 1);
        }


        foreach (var child in Children)
        {
            if (!child.IsVisible)
            {
                continue;
            }
            
            int flex = GetFlex(child);
            double allocatedSize = (totalFlex > 0)
                ? (Orientation == Orientation.Vertical
                      ? finalSize.Height - originTotalHeight
                      : finalSize.Width - originTotalWidth) *
                  (flex / (double)totalFlex)
                : 0;

            if (Orientation == Orientation.Vertical)
            {
                double height = allocatedSize > 0 ? allocatedSize : child.DesiredSize.Height;
                double width  = finalSize.Width;

                var rect = new Rect(0, offset, width, height);

                child.Arrange(rect);
                offset += height + spacing;
            }
            else
            {
                double width  = allocatedSize > 0 ? allocatedSize : child.DesiredSize.Width;
                double height = finalSize.Height;

                var rect = new Rect(offset, 0, width, height);
                child.Arrange(rect);
                offset += width + spacing;
            }
        }

        return finalSize;
    }
}