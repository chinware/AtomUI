using Avalonia;
using Avalonia.Animation;
using Avalonia.Animation.Easings;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Layout;
using Avalonia.Media;
using Avalonia.Media.Transformation;

namespace AtomUI.Controls;

internal class DrawerContainer : Border
{
    private readonly TimeSpan _duration = TimeSpan.FromMilliseconds(500);
    private readonly Easing _easing = new SplineEasing(0.3, 0.7, 0.3, 0.7);
    private readonly DrawerElementBorder _elementBorder;
    private readonly Border _mask;
    private readonly IBrush _maskBrush = new SolidColorBrush(Colors.Black, 0.45);
    private readonly Transitions _transitions1;
    private readonly Transitions _transitions2;

    #region Properties

    public Drawer Drawer { get; }

    #endregion

    #region Close On Click

    protected override void OnPointerPressed(PointerPressedEventArgs e)
    {
        base.OnPointerPressed(e);

        if (e.Handled || Equals(e.Source, _mask) == false)
        {
            return;
        }

        if (e.Pointer.IsPrimary && Drawer.CloseWhenClickOnMask)
        {
            Drawer.IsOpen = false;
        }
    }

    #endregion

    #region Ctor

    internal DrawerContainer(Drawer drawer, DrawerElementBorder elementBorder)
    {
        _transitions1 =
        [
            new DoubleTransition
            {
                Property = OpacityProperty,
                Duration = _duration,
                Easing   = _easing
            }
        ];
        _transitions2 =
        [
            new TransformOperationsTransition
            {
                Property = RenderTransformProperty,
                Duration = _duration,
                Easing   = _easing
            }
        ];
        _elementBorder                     = elementBorder;
        _elementBorder[!MinWidthProperty]  = drawer[!Drawer.DrawerMinWidthProperty];
        _elementBorder[!MinHeightProperty] = drawer[!Drawer.DrawerMinHeightProperty];
        _elementBorder[!MaxWidthProperty]  = drawer[!Drawer.DrawerMaxWidthProperty];
        _elementBorder[!MaxHeightProperty] = drawer[!Drawer.DrawerMaxHeightProperty];
        _elementBorder[!WidthProperty]     = drawer[!Drawer.DrawerWidthProperty];
        _elementBorder[!HeightProperty]    = drawer[!Drawer.DrawerHeightProperty];

        _mask = new Border
        {
            Background           = _maskBrush,
            [!IsVisibleProperty] = drawer[!Drawer.ShowMaskProperty]
        };

        Drawer       = drawer;
        ClipToBounds = true;
        Child        = new Panel { Children = { _mask, _elementBorder } };

        Drawer.PropertyChanged += DrawerOnPropertyChanged;
    }

    private void DrawerOnPropertyChanged(object? sender, AvaloniaPropertyChangedEventArgs e)
    {
        if (e.Property == Drawer.PlacementProperty)
        {
            _isBoxShadowValid = false;
            _isPlacementValid = false;
            UpdateDropShadow();
            UpdatePlacement();
        }

        if (e.Property == Drawer.IsOpenProperty)
        {
            UpdateDropShadow();
            UpdatePlacement();

            if (Drawer.IsOpen)
            {
                OnOpening();
            }
            else
            {
                OnClosing();
            }
        }
    }

    #endregion

    #region On Opening & Closing

    private void OnOpening()
    {
        _mask.Transitions = null;
        _mask.Opacity     = 0;
        _mask.Transitions = _transitions1;
        _mask.Opacity     = 1;

        var fromX = 0d;
        var fromY = 0d;

        switch (Drawer.Placement)
        {
            case DrawerPlacement.Left:
                fromX = -Bounds.Width;
                break;
            case DrawerPlacement.Top:
                fromY = -Bounds.Height;
                break;
            case DrawerPlacement.Right:
                fromX = Bounds.Width;
                break;
            case DrawerPlacement.Bottom:
                fromY = Bounds.Height;
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }

        _elementBorder.Transitions     = null;
        _elementBorder.RenderTransform = TransformOperations.Parse($"translate({fromX}px,{fromY}px)");
        _elementBorder.Transitions     = _transitions2;
        _elementBorder.RenderTransform = TransformOperations.Parse("translate(0px,0px)");
    }

    private void OnClosing()
    {
        _mask.Transitions = null;
        _mask.Opacity     = 1;
        _mask.Transitions = _transitions1;
        _mask.Opacity     = 0;

        var fromX = 0d;
        var fromY = 0d;

        switch (Drawer.Placement)
        {
            case DrawerPlacement.Left:
                fromX = -Bounds.Width;
                break;
            case DrawerPlacement.Top:
                fromY = -Bounds.Height;
                break;
            case DrawerPlacement.Right:
                fromX = Bounds.Width;
                break;
            case DrawerPlacement.Bottom:
                fromY = Bounds.Height;
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }

        _elementBorder.Transitions     = null;
        _elementBorder.RenderTransform = TransformOperations.Parse("translate(0px,0px)");
        _elementBorder.Transitions     = _transitions2;
        _elementBorder.RenderTransform = TransformOperations.Parse($"translate({fromX}px,{fromY}px)");
    }

    #endregion

    #region BoxShadow

    private bool _isBoxShadowValid;
    private bool _isPlacementValid;

    private void UpdateDropShadow()
    {
        if (_isBoxShadowValid)
        {
            return;
        }

        switch (Drawer.Placement)
        {
            case DrawerPlacement.Left:
            {
                var box1 = Avalonia.Media.BoxShadow.Parse("6 0 16 0 rgba(0,0,0,0.08)");
                var box2 = Avalonia.Media.BoxShadow.Parse("3 0 6 -4 rgba(0,0,0,0.12)");
                var box3 = Avalonia.Media.BoxShadow.Parse("9 0 28 8 rgba(0,0,0,0.05)");
                _elementBorder.BoxShadow = new BoxShadows(box1, [
                    box2,
                    box3
                ]);
                break;
            }
            case DrawerPlacement.Top:
            {
                var box1 = Avalonia.Media.BoxShadow.Parse("0 6 16 0 rgba(0,0,0,0.08)");
                var box2 = Avalonia.Media.BoxShadow.Parse("0 3 6 -4 rgba(0,0,0,0.12)");
                var box3 = Avalonia.Media.BoxShadow.Parse("0 9 28 8 rgba(0,0,0,0.05)");
                _elementBorder.BoxShadow = new BoxShadows(box1, [
                    box2,
                    box3
                ]);
                break;
            }
            case DrawerPlacement.Right:
            {
                var box1 = Avalonia.Media.BoxShadow.Parse("-6 0 16 0 rgba(0,0,0,0.08)");
                var box2 = Avalonia.Media.BoxShadow.Parse("-3 0 6 -4 rgba(0,0,0,0.12)");
                var box3 = Avalonia.Media.BoxShadow.Parse("-9 0 28 8 rgba(0,0,0,0.05)");
                _elementBorder.BoxShadow = new BoxShadows(box1, [
                    box2,
                    box3
                ]);
                break;
            }
            case DrawerPlacement.Bottom:
            {
                var box1 = Avalonia.Media.BoxShadow.Parse("0 -6 16 0 rgba(0,0,0,0.08)");
                var box2 = Avalonia.Media.BoxShadow.Parse("0 -3 6 -4 rgba(0,0,0,0.12)");
                var box3 = Avalonia.Media.BoxShadow.Parse("0 -9 28 8 rgba(0,0,0,0.05)");
                _elementBorder.BoxShadow = new BoxShadows(box1, [
                    box2,
                    box3
                ]);
                break;
            }
            default:
                throw new ArgumentOutOfRangeException();
        }

        _isBoxShadowValid = true;
    }

    private void UpdatePlacement()
    {
        if (_isPlacementValid)
        {
            return;
        }

        switch (Drawer.Placement)
        {
            case DrawerPlacement.Left:
                _elementBorder.VerticalAlignment   = VerticalAlignment.Stretch;
                _elementBorder.HorizontalAlignment = HorizontalAlignment.Left;
                break;
            case DrawerPlacement.Right:
                _elementBorder.VerticalAlignment   = VerticalAlignment.Stretch;
                _elementBorder.HorizontalAlignment = HorizontalAlignment.Right;
                break;
            case DrawerPlacement.Top:
                _elementBorder.VerticalAlignment   = VerticalAlignment.Top;
                _elementBorder.HorizontalAlignment = HorizontalAlignment.Stretch;
                break;
            case DrawerPlacement.Bottom:
                _elementBorder.VerticalAlignment   = VerticalAlignment.Bottom;
                _elementBorder.HorizontalAlignment = HorizontalAlignment.Stretch;
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }

        _isPlacementValid = true;
    }

    #endregion
}