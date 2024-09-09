using AtomUI.Controls.Primitives;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Data;
using Avalonia.Metadata;
using Avalonia.VisualTree;

namespace AtomUI.Controls;

public class Drawer : TemplatedControl
{
    public static Drawer? GetDrawer(Visual element)
    {
        var container = element.FindAncestorOfType<DrawerContainer>();
        return container?.Drawer;
    }
    
    
    #region Properties

    [Content]
    public Control? Content
    {
        get => GetValue(ContentProperty);
        set => SetValue(ContentProperty, value);
    }
    public static readonly StyledProperty<Control?> ContentProperty = AvaloniaProperty
        .Register<Drawer, Control?>(nameof(Content));

    public bool IsOpen
    {
        get => GetValue(IsOpenProperty);
        set => SetValue(IsOpenProperty, value);
    }
    public static readonly StyledProperty<bool> IsOpenProperty = AvaloniaProperty
        .Register<Drawer, bool>(nameof(IsOpen), false, false, BindingMode.TwoWay);

    public DrawerPlacement Placement
    {
        get => GetValue(PlacementProperty);
        set => SetValue(PlacementProperty, value);
    }
    public static readonly StyledProperty<DrawerPlacement> PlacementProperty = AvaloniaProperty
        .Register<Drawer, DrawerPlacement>(nameof(Placement), DrawerPlacement.Right);

    public Visual? OpenOn
    {
        get => GetValue(OpenOnProperty);
        set => SetValue(OpenOnProperty, value);
    }
    public static readonly StyledProperty<Visual?> OpenOnProperty = AvaloniaProperty
        .Register<Drawer, Visual?>(nameof(OpenOn));

    public bool ShowMask
    {
        get => GetValue(ShowMaskProperty);
        set => SetValue(ShowMaskProperty, value);
    }
    public static readonly StyledProperty<bool> ShowMaskProperty = AvaloniaProperty
        .Register<Drawer, bool>(nameof(ShowMask), true);

    public bool CloseWhenClickOnMask
    {
        get => GetValue(CloseWhenClickOnMaskProperty);
        set => SetValue(CloseWhenClickOnMaskProperty, value);
    }
    public static readonly StyledProperty<bool> CloseWhenClickOnMaskProperty = AvaloniaProperty
        .Register<Drawer, bool>(nameof(CloseWhenClickOnMask), true);

    public double DrawerMinWidth
    {
        get => GetValue(DrawerMinWidthProperty);
        set => SetValue(DrawerMinWidthProperty, value);
    }
    public static readonly StyledProperty<double> DrawerMinWidthProperty = AvaloniaProperty
        .Register<Drawer, double>(nameof(DrawerMinWidth), 0);

    public double DrawerMinHeight
    {
        get => GetValue(DrawerMinHeightProperty);
        set => SetValue(DrawerMinHeightProperty, value);
    }
    public static readonly StyledProperty<double> DrawerMinHeightProperty = AvaloniaProperty
        .Register<Drawer, double>(nameof(DrawerMinHeight), 0);

    public double DrawerMaxWidth
    {
        get => GetValue(DrawerMaxWidthProperty);
        set => SetValue(DrawerMaxWidthProperty, value);
    }
    public static readonly StyledProperty<double> DrawerMaxWidthProperty = AvaloniaProperty
        .Register<Drawer, double>(nameof(DrawerMaxWidth), double.MaxValue);

    public double DrawerMaxHeight
    {
        get => GetValue(DrawerMaxHeightProperty);
        set => SetValue(DrawerMaxHeightProperty, value);
    }
    public static readonly StyledProperty<double> DrawerMaxHeightProperty = AvaloniaProperty
        .Register<Drawer, double>(nameof(DrawerMaxHeight), double.MaxValue);

    public double DrawerWidth
    {
        get => GetValue(DrawerWidthProperty);
        set => SetValue(DrawerWidthProperty, value);
    }
    public static readonly StyledProperty<double> DrawerWidthProperty = AvaloniaProperty
        .Register<Drawer, double>(nameof(DrawerWidth), double.NaN);

    public double DrawerHeight
    {
        get => GetValue(DrawerHeightProperty);
        set => SetValue(DrawerHeightProperty, value);
    }
    public static readonly StyledProperty<double> DrawerHeightProperty = AvaloniaProperty
        .Register<Drawer, double>(nameof(DrawerHeight), double.NaN);
    
    #endregion
    
    
    #region Ctor

    static Drawer()
    {
        IsOpenProperty.Changed.AddClassHandler<Drawer>(OnIsOpenChanged);
    }
    
    public Drawer()
    {
        this[!AtomLayer.BoundsAnchorProperty] = this[!OpenOnProperty];
    }

    #endregion


    #region Open & Close

    private DrawerContainer?     _container;
    private DrawerElementBorder? _element;

    private static void OnIsOpenChanged(Drawer drawer, AvaloniaPropertyChangedEventArgs arg)
    {
        if (drawer.IsOpen)
        {
            drawer.Open();
        }
        else
        {
            drawer.Close();
        }
    }

    private void Open()
    {
        _element   ??= new DrawerElementBorder(this);
        _container ??= new DrawerContainer(this, _element);
        
        var layer = this.GetLayer();
        layer?.AddAdorner(this, _container);
        this._container.IsHitTestVisible = true;
    }
    
    private void Close()
    {
        if (_container == null)
        {
            return;
        }
        
        this.BeginRemovingAdorner(_container, 1000, () => this.IsOpen == false);
        this._container.IsHitTestVisible = false;
    }

    #endregion
}