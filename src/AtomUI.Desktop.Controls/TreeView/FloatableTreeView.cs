using Avalonia;
using Avalonia.Interactivity;

namespace AtomUI.Desktop.Controls;

public class FloatableTreeView : TreeView
{
    #region 公共属性定义

    public static readonly DirectProperty<FloatableTreeView, bool> IsOpenProperty =
        AvaloniaProperty.RegisterDirect<FloatableTreeView, bool>(
            nameof(IsOpen),
            o => o.IsOpen);

    private bool _isOpen;

    public bool IsOpen
    {
        get => _isOpen;
        protected set => SetAndRaise(IsOpenProperty, ref _isOpen, value);
    }

    #endregion

    #region 公共事件定义

    public static readonly RoutedEvent<RoutedEventArgs> OpenedEvent =
        RoutedEvent.Register<FloatableTreeView, RoutedEventArgs>(nameof(Opened), RoutingStrategies.Bubble);

    public static readonly RoutedEvent<RoutedEventArgs> ClosedEvent =
        RoutedEvent.Register<FloatableTreeView, RoutedEventArgs>(nameof(Closed), RoutingStrategies.Bubble);

    public event EventHandler<RoutedEventArgs>? Opened
    {
        add => AddHandler(OpenedEvent, value);
        remove => RemoveHandler(OpenedEvent, value);
    }

    public event EventHandler<RoutedEventArgs>? Closed
    {
        add => AddHandler(ClosedEvent, value);
        remove => RemoveHandler(ClosedEvent, value);
    }

    #endregion

    private TreeViewFlyout? _flyout;

    public TreeViewFlyout? TreeViewFlyout
    {
        get => _flyout;
        set
        {
            NotifyFlyoutAssigned(_flyout);
            _flyout = value;
        }
    }

    public FloatableTreeView()
    {
    }

    protected FloatableTreeView(ITreeViewInteractionHandler interactionHandler)
        : base(interactionHandler)
    {
    }

    public void Close()
    {
        if (TreeViewFlyout is not null)
        {
            TreeViewFlyout.Hide();
        }
    }

    protected virtual void NotifyFlyoutAssigned(Flyout? flyout)
    {
        if (_flyout != null)
        {
            _flyout.Opened -= HandleFlyoutOpened;
            _flyout.Closed -= HandleFlyoutClosed;
        }

        if (flyout != null)
        {
            flyout.Opened += HandleFlyoutOpened;
            flyout.Closed += HandleFlyoutClosed;
        }
    }

    private void HandleFlyoutOpened(object? sender, EventArgs e)
    {
        IsOpen = true;
        RaiseEvent(new RoutedEventArgs
        {
            RoutedEvent = OpenedEvent,
            Source      = this,
        });
    }

    private void HandleFlyoutClosed(object? sender, EventArgs e)
    {
        IsOpen = false;
        RaiseEvent(new RoutedEventArgs
        {
            RoutedEvent = ClosedEvent,
            Source      = this,
        });
    }
}