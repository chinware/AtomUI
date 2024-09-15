using AtomUI.Controls.Primitives;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Data;
using Avalonia.Media;
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

    public DrawerOpenMode OpenMode
    {
        get => GetValue(OpenModeProperty);
        set => SetValue(OpenModeProperty, value);
    }
    public static readonly StyledProperty<DrawerOpenMode> OpenModeProperty = AvaloniaProperty
        .Register<Drawer, DrawerOpenMode>(nameof(OpenMode), DrawerOpenMode.Overlay);

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

    private DrawerContainer? _container;
    private Border?          _element;

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
        var layer = this.GetLayer();
        if (layer == null)
        {
            return;
        }
        
        _element ??= new Border()
        {
            Child      = Content,
            Background = Brushes.White,
        };
        _container ??= new DrawerContainer(this, _element);
        _container.SetIsClosing(false);
        layer.AddAdorner(this, _container);
    }

    private void Close()
    {
        if (_container == null)
        {
            return;
        }
        
        var layer = this.GetLayer();
        layer?.BeginRemovingAdorner(_container, 1000, () => IsOpen == false);
        _container.SetIsClosing(true);
    }

    #endregion
}