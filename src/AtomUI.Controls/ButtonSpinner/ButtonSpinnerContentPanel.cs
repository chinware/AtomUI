using Avalonia;
using Avalonia.Controls;

namespace AtomUI.Controls;

internal class ButtonSpinnerContentPanel : DockPanel
{
    #region 公共属性定义

    internal static readonly DirectProperty<ButtonSpinnerContentPanel, bool> IsHandleFloatableProperty =
        AvaloniaProperty.RegisterDirect<ButtonSpinnerContentPanel, bool>(
            nameof(IsHandleFloatable),
            o => o.IsHandleFloatable,
            (o, v) => o.IsHandleFloatable = v);

    internal static readonly DirectProperty<ButtonSpinnerContentPanel, ButtonSpinnerLocation> HandleLocationProperty =
        AvaloniaProperty.RegisterDirect<ButtonSpinnerContentPanel, ButtonSpinnerLocation>(
            nameof(HandleLocation),
            o => o.HandleLocation,
            (o, v) => o.HandleLocation = v);

    internal static readonly DirectProperty<ButtonSpinnerContentPanel, bool> IsHoverProperty =
        AvaloniaProperty.RegisterDirect<ButtonSpinnerContentPanel, bool>(
            nameof(IsHover),
            o => o.IsHover,
            (o, v) => o.IsHover = v);

    internal static readonly DirectProperty<ButtonSpinnerContentPanel, double> HandleWidthProperty =
        AvaloniaProperty.RegisterDirect<ButtonSpinnerContentPanel, double>(nameof(HandleWidth),
            o => o.HandleWidth,
            (o, v) => o.HandleWidth = v);

    internal static readonly DirectProperty<ButtonSpinnerContentPanel, Thickness> ContentPaddingProperty =
        AvaloniaProperty.RegisterDirect<ButtonSpinnerContentPanel, Thickness>(nameof(ContentPadding),
            o => o.ContentPadding,
            (o, v) => o.ContentPadding = v);

    private bool _isHandleFloatable;

    internal bool IsHandleFloatable
    {
        get => _isHandleFloatable;
        set => SetAndRaise(IsHandleFloatableProperty, ref _isHandleFloatable, value);
    }

    private ButtonSpinnerLocation _handleLocation;

    internal ButtonSpinnerLocation HandleLocation
    {
        get => _handleLocation;
        set => SetAndRaise(HandleLocationProperty, ref _handleLocation, value);
    }

    private bool _isHover;

    internal bool IsHover
    {
        get => _isHover;
        set => SetAndRaise(IsHoverProperty, ref _isHover, value);
    }

    private double _handleWidth;

    internal double HandleWidth
    {
        get => _handleWidth;
        set => SetAndRaise(HandleWidthProperty, ref _handleWidth, value);
    }

    private Thickness _contentPadding;

    internal Thickness ContentPadding
    {
        get => _contentPadding;
        set => SetAndRaise(ContentPaddingProperty, ref _contentPadding, value);
    }

    #endregion

    static ButtonSpinnerContentPanel()
    {
        AffectsMeasure<ButtonSpinnerContentPanel>(HandleWidthProperty);
        AffectsArrange<ButtonSpinnerContentPanel>(IsHandleFloatableProperty, HandleLocationProperty, IsHoverProperty,
            ContentPaddingProperty);
    }

    protected override Size ArrangeOverride(Size finalSize)
    {
        var size = base.ArrangeOverride(finalSize);
        if (IsHandleFloatable && IsHover)
        {
            Control? leftAlignmentControl  = null;
            Control? rightAlignmentControl = null;
            foreach (var item in Children)
            {
                if (item != null)
                {
                    if (GetDock(item) == Dock.Left)
                    {
                        leftAlignmentControl = item;
                    }

                    if (GetDock(item) == Dock.Right)
                    {
                        rightAlignmentControl = item;
                    }
                }
            }

            if (HandleLocation == ButtonSpinnerLocation.Right)
            {
                if (rightAlignmentControl != null)
                {
                    var bounds = rightAlignmentControl.Bounds;
                    rightAlignmentControl.Arrange(new Rect(bounds.X - ContentPadding.Right * 1.5, bounds.Y, bounds.Width,
                        bounds.Height));
                }
            }
            else if (HandleLocation == ButtonSpinnerLocation.Left)
            {
                if (leftAlignmentControl != null)
                {
                    var bounds = leftAlignmentControl.Bounds;
                    leftAlignmentControl.Arrange(new Rect(bounds.X + ContentPadding.Left * 1.5, bounds.Y, bounds.Width,
                        bounds.Height));
                }
            }
        }

        return size;
    }
}