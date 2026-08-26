using Avalonia;
using Avalonia.Interactivity;
using Avalonia.Threading;
using Avalonia.VisualTree;

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
    private TreeViewFlyout? _registeredFlyout;
    private int _pinnedOpenGeneration;

    public TreeViewFlyout? TreeViewFlyout
    {
        get => _flyout;
        set
        {
            if (ReferenceEquals(_flyout, value))
            {
                return;
            }

            _flyout = value;
            NotifyFlyoutAssigned(value);
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
        UnregisterFlyout(_registeredFlyout);
        if (flyout is TreeViewFlyout treeViewFlyout && this.IsAttachedToVisualTree())
        {
            RegisterFlyout(treeViewFlyout);
        }
    }

    protected override void OnAttachedToVisualTree(VisualTreeAttachmentEventArgs e)
    {
        base.OnAttachedToVisualTree(e);
        RegisterFlyout(TreeViewFlyout);
    }

    protected override void OnDetachedFromVisualTree(VisualTreeAttachmentEventArgs e)
    {
        UnregisterFlyout(_registeredFlyout);
        base.OnDetachedFromVisualTree(e);
    }

    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        base.OnPropertyChanged(change);
        if (change.Property == IsPopupPinnedOpenProperty)
        {
            if (_registeredFlyout is { } flyout)
            {
                flyout.SetCurrentValue(Flyout.IsPopupPinnedOpenProperty, change.GetNewValue<bool>());
            }

            if (change.GetNewValue<bool>() && this.IsAttachedToVisualTree())
            {
                QueuePinnedOpen();
            }
            else
            {
                ++_pinnedOpenGeneration;
            }
        }
    }

    private void RegisterFlyout(TreeViewFlyout? flyout)
    {
        if (flyout is null || ReferenceEquals(flyout, _registeredFlyout))
        {
            return;
        }

        UnregisterFlyout(_registeredFlyout);
        _registeredFlyout = flyout;
        flyout.Opened += HandleFlyoutOpened;
        flyout.Closed += HandleFlyoutClosed;
        flyout.SetCurrentValue(Flyout.IsPopupPinnedOpenProperty, IsPopupPinnedOpen);
        IsOpen = flyout.IsOpen;
        if (IsPopupPinnedOpen)
        {
            QueuePinnedOpen();
        }
    }

    private void UnregisterFlyout(TreeViewFlyout? flyout)
    {
        if (flyout is null || !ReferenceEquals(flyout, _registeredFlyout))
        {
            return;
        }

        ++_pinnedOpenGeneration;
        flyout.CloseForLifecycle();
        flyout.Opened -= HandleFlyoutOpened;
        flyout.Closed -= HandleFlyoutClosed;
        flyout.SetCurrentValue(Flyout.IsPopupPinnedOpenProperty, false);
        _registeredFlyout = null;
        IsOpen = false;
    }

    private void QueuePinnedOpen()
    {
        var generation = ++_pinnedOpenGeneration;
        var flyout     = _registeredFlyout;
        Dispatcher.UIThread.Post(() =>
        {
            if (generation == _pinnedOpenGeneration &&
                IsPopupPinnedOpen &&
                this.IsAttachedToVisualTree() &&
                IsEffectivelyEnabled &&
                IsVisible &&
                ReferenceEquals(flyout, _registeredFlyout) &&
                flyout is { IsOpen: false })
            {
                flyout.ShowAt(this);
            }
        }, DispatcherPriority.Loaded);
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
