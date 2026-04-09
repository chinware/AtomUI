using System.Collections;
using AtomUI.Controls;
using AtomUI.Theme;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Metadata;
using Avalonia.Controls.Primitives;
using Avalonia.Threading;
using Avalonia.VisualTree;

namespace AtomUI.Desktop.Controls;

[TemplatePart("PART_Items", typeof(Panel))]
[PseudoClasses(NotificationPseudoClass.TopLeft, 
    NotificationPseudoClass.TopRight,
    NotificationPseudoClass.BottomLeft,
    NotificationPseudoClass.BottomRight, 
    NotificationPseudoClass.TopCenter,
    NotificationPseudoClass.BottomCenter)]
public class WindowNotificationManager : TemplatedControl, INotificationManager, IMotionAwareControl, IDisposable
{
    #region 公共属性定义
    public static readonly StyledProperty<NotificationPosition> PositionProperty =
        AvaloniaProperty.Register<WindowNotificationManager, NotificationPosition>(
            nameof(Position), NotificationPosition.TopRight);
    
    public static readonly StyledProperty<bool> IsMotionEnabledProperty =
        MotionAwareControlProperty.IsMotionEnabledProperty.AddOwner<WindowNotificationManager>();

    public static readonly StyledProperty<int> MaxItemsProperty =
        AvaloniaProperty.Register<WindowNotificationManager, int>(nameof(MaxItems), 5);

    public static readonly StyledProperty<bool> IsPauseOnHoverProperty =
        AvaloniaProperty.Register<WindowNotificationManager, bool>(nameof(IsPauseOnHover), true);

    public NotificationPosition Position
    {
        get => GetValue(PositionProperty);
        set => SetValue(PositionProperty, value);
    }

    public int MaxItems
    {
        get => GetValue(MaxItemsProperty);
        set => SetValue(MaxItemsProperty, value);
    }

    public bool IsPauseOnHover
    {
        get => GetValue(IsPauseOnHoverProperty);
        set => SetValue(IsPauseOnHoverProperty, value);
    }
    
    public bool IsMotionEnabled
    {
        get => GetValue(IsMotionEnabledProperty);
        set => SetValue(IsMotionEnabledProperty, value);
    }
    
    #endregion
    
    private TopLevel? _topLevel;
    private bool _isDisposed;
    private AdornerLayer? _adornerLayer;
    private IList? _items;
    private readonly Queue<NotificationCard> _cleanupQueue;
    private readonly DispatcherTimer _cardExpiredTimer;
    private readonly DispatcherTimer _cleanupTimer;

    public WindowNotificationManager(TopLevel? host) : this()
    {
        if (host is not null)
        {
            _topLevel = host;
            InstallFromTopLevel(host);
        }
    }

    public WindowNotificationManager()
    {
        this.RegisterTokenResourceScope(NotificationToken.ScopeProvider);
        _cardExpiredTimer      =  new DispatcherTimer { Interval = TimeSpan.FromMilliseconds(50), Tag = this };
        _cardExpiredTimer.Tick += HandleCardExpiredTimer;
        _cleanupTimer          =  new DispatcherTimer { Interval = TimeSpan.FromMilliseconds(50), Tag = this };
        _cleanupTimer.Tick     += HandleCleanupTimerTick;
        _cleanupQueue          =  new Queue<NotificationCard>();
    }

    protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
    {
        base.OnApplyTemplate(e);
        var itemsControl = e.NameScope.Find<Panel>("PART_Items");
        _items = itemsControl?.Children;
        UpdatePseudoClasses(Position);
    }

    private void HandleCardExpiredTimer(object? sender, EventArgs eventArgs)
    {
        if (_items != null)
        {
            foreach (var item in _items)
            {
                if (item is NotificationCard card)
                {
                    if (card.NotifyCloseTick(_cardExpiredTimer.Interval))
                    {
                        if (!_cleanupQueue.Contains(card))
                        {
                            _cleanupQueue.Enqueue(card);
                            if (!_cleanupTimer.IsEnabled)
                            {
                                _cleanupTimer.Start();
                            }
                        }
                    }
                }
            }
        }
    }

    private void HandleCleanupTimerTick(object? sender, EventArgs eventArgs)
    {
        if (_cleanupQueue.Count > 0)
        {
            var card = _cleanupQueue.Peek();
            if (!card.IsClosing)
            {
                card.Close();
            }
            else if (card.IsClosed)
            {
                _cleanupQueue.Dequeue();
                if (_cleanupQueue.Count == 0)
                {
                    _cleanupTimer.Stop();
                }
            }
        }
    }

    private void ConfigureExpiredTimer()
    {
        if (_items?.Count > 0)
        {
            _cardExpiredTimer.Start();
        }
        else
        {
            _cardExpiredTimer.Stop();
        }
    }

    public void Show(INotification notification, string[]? classes = null)
    {
        if (_items is null)
        {
            return;
        }
        var expiration = notification.Expiration;
        var onClick    = notification.OnClick;
        var onClose    = notification.OnClose;
        Dispatcher.UIThread.VerifyAccess();

        var notificationControl = new NotificationCard(this)
        {
            Title            = notification.Title,
            Content          = notification.Content,
            Icon             = notification.Icon,
            NotificationType = notification.Type,
            Expiration       = expiration == TimeSpan.Zero ? null : expiration,
            IsShowProgress   = notification.ShowProgress
        };
        notificationControl[!NotificationCard.PositionProperty] = this[!PositionProperty];
        
        // Add style classes if any
        if (classes?.Length > 0)
        {
            foreach (var cls in classes)
            {
                notificationControl.Classes.Add(cls);
            }
        }

        notificationControl.PointerPressed += (_, _) => { onClick?.Invoke(); };
        notificationControl.NotificationClosed += (sender, _) =>
        {
            onClose?.Invoke();
            if (sender is NotificationCard card)
            {
                _items?.Remove(card);
                ConfigureExpiredTimer();
            }
        };

        Dispatcher.UIThread.Post(() =>
        {
            _items?.Add(notificationControl);
            ConfigureExpiredTimer();
            RemoveExcessNotifications();
        });
    }
    
    /// <summary>
    /// Removes excess notifications when the count exceeds MaxItems.
    /// </summary>
    private void RemoveExcessNotifications()
    {
        var visibleNotifications = _items!.OfType<NotificationCard>().Where(n => !n.IsClosing).ToList();
        var excessCount          = visibleNotifications.Count - MaxItems;

        if (excessCount > 0)
        {
            // 支持关闭多个超限的通知，而不仅仅是一个
            for (int i = 0; i < excessCount; i++)
            {
                visibleNotifications[i].Close();
            }
        }
    }

    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        base.OnPropertyChanged(change);

        if (change.Property == PositionProperty)
        {
            UpdatePseudoClasses(change.GetNewValue<NotificationPosition>());
        }
    }
    
    private void InstallFromTopLevel(TopLevel topLevel)
    {
        topLevel.TemplateApplied += TopLevelOnTemplateApplied;
        _adornerLayer            =  topLevel.FindDescendantOfType<VisualLayerManager>()?.AdornerLayer;
        if (_adornerLayer is not null)
        {
            _adornerLayer.Children.Add(this);
            AdornerLayer.SetAdornedElement(this, _adornerLayer);
        }
    }

    private void TopLevelOnTemplateApplied(object? sender, TemplateAppliedEventArgs e)
    {
        RemoveFromAdornerLayer();

        // Reinstall notification manager on template reapplied.
        var topLevel = (TopLevel)sender!;
        topLevel.TemplateApplied -= TopLevelOnTemplateApplied;
        InstallFromTopLevel(topLevel);
    }
    
    private void RemoveFromAdornerLayer()
    {
        if (_adornerLayer is not null)
        {
            _adornerLayer.Children.Remove(this);
            AdornerLayer.SetAdornedElement(this, null);
            _adornerLayer = null;
        }
    }
    
    public void Dispose()
    {
        if (_topLevel is null || _isDisposed)
        {
            return;
        }

        try
        {
            _cleanupTimer.Stop();
            _cardExpiredTimer.Stop();
            _items?.Clear();
            _cleanupQueue.Clear();
            // 卸载事件订阅
            _topLevel.TemplateApplied -= TopLevelOnTemplateApplied;

            // 从 AdornerLayer 中移除
            if (_adornerLayer is not null)
            {
                _adornerLayer.Children.Remove(this);
                AdornerLayer.SetAdornedElement(this, null);
                _adornerLayer = null;
            }
            
            _topLevel     = null;
            _items        = null;
            _isDisposed   = true;
          
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error uninstalling from TopLevel: {ex.Message}");
        }
    }

    private void UpdatePseudoClasses(NotificationPosition position)
    {
        PseudoClasses.Set(NotificationPseudoClass.TopLeft, position == NotificationPosition.TopLeft);
        PseudoClasses.Set(NotificationPseudoClass.TopRight, position == NotificationPosition.TopRight);
        PseudoClasses.Set(NotificationPseudoClass.BottomLeft, position == NotificationPosition.BottomLeft);
        PseudoClasses.Set(NotificationPseudoClass.BottomRight, position == NotificationPosition.BottomRight);
        PseudoClasses.Set(NotificationPseudoClass.TopCenter, position == NotificationPosition.TopCenter);
        PseudoClasses.Set(NotificationPseudoClass.BottomCenter, position == NotificationPosition.BottomCenter);
    }

    internal void StopExpiredTimer()
    {
        _cardExpiredTimer.Stop();
    }

    internal void StartExpiredTimer()
    {
        _cardExpiredTimer.Start();
    }
    
}