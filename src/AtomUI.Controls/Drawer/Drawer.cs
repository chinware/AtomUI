using AtomUI.Controls.Primitives;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Data;
using Avalonia.Layout;
using Avalonia.Metadata;
using Avalonia.VisualTree;

namespace AtomUI.Controls;

public class Drawer : TemplatedControl
{
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


    #region Ctor

    static Drawer()
    {
        IsOpenProperty.Changed.AddClassHandler<Drawer>(OnIsOpenChanged);
        PlacementProperty.Changed.AddClassHandler<Drawer>((drawer, args) => drawer.UpdatePlacement(drawer._container));
    }

    private void UpdatePlacement(DrawerContainer? container)
    {
        if (container?.Child == null)
        {
            return;
        }

        switch (Placement)
        {
            case DrawerPlacement.Left:
                container.Child.VerticalAlignment   = VerticalAlignment.Stretch;
                container.Child.HorizontalAlignment = HorizontalAlignment.Left;
                break;
            case DrawerPlacement.Right:
                container.Child.VerticalAlignment   = VerticalAlignment.Stretch;
                container.Child.HorizontalAlignment = HorizontalAlignment.Right;
                break;
            case DrawerPlacement.Top:
                container.Child.VerticalAlignment   = VerticalAlignment.Top;
                container.Child.HorizontalAlignment = HorizontalAlignment.Stretch;
                break;
            case DrawerPlacement.Bottom:
                container.Child.VerticalAlignment   = VerticalAlignment.Bottom;
                container.Child.HorizontalAlignment = HorizontalAlignment.Stretch;
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }
    
    public Drawer()
    {
        this[!AtomLayer.BoundsAnchorProperty] = this[!OpenOnProperty];
    }

    #endregion


    #region Open & Close

    private DrawerContainer? _container;

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
        _container ??= new DrawerContainer(this)
        {
            Child = new DrawerElementBorder(this)
            {
                Child = Content,
            },
            ShowMask = ShowMask,
        };
        UpdatePlacement(_container);
        
        var layer = this.GetLayer();
        layer?.AddAdorner(this, _container);
    }
    
    private void Close()
    {
        if (_container == null)
        {
            return;
        }

        this.RemoveAdorner(_container);
    }

    #endregion


    public static Drawer? GetDrawer(Visual element)
    {
        var container = element.FindAncestorOfType<DrawerContainer>();
        return container?.Drawer;
    }
}