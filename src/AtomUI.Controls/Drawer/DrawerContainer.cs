using AtomUI.Controls.Primitives;
using Avalonia;
using Avalonia.Animation;
using Avalonia.Animation.Easings;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Layout;
using Avalonia.Media;
using Avalonia.Media.Transformation;

namespace AtomUI.Controls;

internal class DrawerContainer : Border
{
    private readonly TimeSpan    _duration = TimeSpan.FromMilliseconds(500);
    private readonly Easing      _easing1   = new SplineEasing(0.3, 0.7, 0.3, 0.7);
    private readonly Easing      _easing2   = new SplineEasing(0.7, 0.3, 0.7, 0.3);
    private readonly Border      _child;
    private readonly Border      _mask;
    private readonly IBrush      _maskBrush = new SolidColorBrush(Colors.Black, 0.45);
    private readonly Transitions _transitions1;
    private readonly Transitions _transitions2;
    private readonly Transitions _transitions3;

    #region Properties

    public Drawer Drawer { get; }
    
    internal bool IsClosing { get; set; }

    #endregion
    
    #region Ctor

    internal DrawerContainer(Drawer drawer, Border child)
    {
        _transitions1 =
        [
            new DoubleTransition
            {
                Property = OpacityProperty,
                Duration = _duration,
                Easing   = _easing1
            }
        ];
        _transitions2 =
        [
            new TransformOperationsTransition
            {
                Property = RenderTransformProperty,
                Duration = _duration,
                Easing   = _easing1
            }
        ];
        _transitions3 =
        [
            new TransformOperationsTransition
            {
                Property = RenderTransformProperty,
                Duration = _duration,
                Easing   = _easing2
            }
        ];
        _child                     = child;
        _child[!MinWidthProperty]  = drawer[!Drawer.DrawerMinWidthProperty];
        _child[!MinHeightProperty] = drawer[!Drawer.DrawerMinHeightProperty];
        _child[!MaxWidthProperty]  = drawer[!Drawer.DrawerMaxWidthProperty];
        _child[!MaxHeightProperty] = drawer[!Drawer.DrawerMaxHeightProperty];
        _child[!WidthProperty]     = drawer[!Drawer.DrawerWidthProperty];
        _child[!HeightProperty]    = drawer[!Drawer.DrawerHeightProperty];

        _mask = new Border
        {
            Background           = _maskBrush,
            [!IsVisibleProperty] = drawer[!Drawer.ShowMaskProperty]
        };

        Drawer       = drawer;
        ClipToBounds = true;
        Child        = new Panel { Children = { _mask, _child } };

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

    internal void SetIsClosing(bool isClosing)
    {
        if (isClosing)
        {
            IsHitTestVisible = false;
            IsClosing        = true;
        }
        else
        {
            IsHitTestVisible = true;
            IsClosing        = false;
        }
    }
    
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

        _child.Transitions     = null;
        _child.RenderTransform = TransformOperations.Parse($"translate({fromX}px,{fromY}px)");
        _child.Transitions     = _transitions2;
        _child.RenderTransform = TransformOperations.Parse("translate(0px,0px)");
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

        _child.Transitions     = null;
        _child.RenderTransform = TransformOperations.Parse("translate(0px,0px)");
        _child.Transitions     = _transitions3;
        _child.RenderTransform = TransformOperations.Parse($"translate({fromX}px,{fromY}px)");
        
        PullPreviousDrawers();
    }
    
    protected override void OnLoaded(RoutedEventArgs e)
    {
        base.OnLoaded(e);
        PushPreviousDrawers();
    }
    
    private void PushPreviousDrawers()
    {
        if (string.IsNullOrEmpty(this.Drawer.Group))
        {
            return;
        }
        
        var layer = this.Drawer.GetLayer();
        if (layer == null)
        {
            return;
        }
        
        var w        = 0d;
        var h        = 0d;
        var adorners = layer.Children
            .OfType<DrawerContainer>()
            .Where(dc => dc.IsClosing == false && dc.Drawer.Group == this.Drawer.Group)
            .Reverse()
            .ToList();
        foreach (var adorner in adorners)
        {
            adorner.PushOffset(ref w, ref h);
        }
    }
    
    private void PullPreviousDrawers()
    {
        if (string.IsNullOrEmpty(this.Drawer.Group))
        {
            return;
        }

        var layer = this.Drawer.GetLayer();
        if (layer == null)
        {
            return;
        }

        var w        = 0d;
        var h        = 0d;
        var adorners = layer.Children
            .OfType<DrawerContainer>()
            .Where(dc => dc.IsClosing == false && dc.Drawer.Group == this.Drawer.Group)
            .Reverse()
            .ToList();
        foreach (var adorner in adorners)
        {
            adorner.PushOffset(ref w, ref h);
        }
    }
    
    private void PushOffset(ref double x, ref double y)
    {
        _child.RenderTransform = Drawer.Placement switch
        {
            DrawerPlacement.Left   => TransformOperations.Parse($"translate({x}px,{0}px)"),
            DrawerPlacement.Top    => TransformOperations.Parse($"translate({0}px,{y}px)"),
            DrawerPlacement.Right  => TransformOperations.Parse($"translate({-x}px,{0}px)"),
            DrawerPlacement.Bottom => TransformOperations.Parse($"translate({0}px,{-y}px)"),
            _                      => throw new ArgumentOutOfRangeException()
        };

        x += this._child.Bounds.Width;
        y += this._child.Bounds.Height;
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
                _child.BoxShadow = new BoxShadows(box1, [
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
                _child.BoxShadow = new BoxShadows(box1, [
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
                _child.BoxShadow = new BoxShadows(box1, [
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
                _child.BoxShadow = new BoxShadows(box1, [
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
                _child.VerticalAlignment   = VerticalAlignment.Stretch;
                _child.HorizontalAlignment = HorizontalAlignment.Left;
                break;
            case DrawerPlacement.Right:
                _child.VerticalAlignment   = VerticalAlignment.Stretch;
                _child.HorizontalAlignment = HorizontalAlignment.Right;
                break;
            case DrawerPlacement.Top:
                _child.VerticalAlignment   = VerticalAlignment.Top;
                _child.HorizontalAlignment = HorizontalAlignment.Stretch;
                break;
            case DrawerPlacement.Bottom:
                _child.VerticalAlignment   = VerticalAlignment.Bottom;
                _child.HorizontalAlignment = HorizontalAlignment.Stretch;
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }

        _isPlacementValid = true;
    }

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
}