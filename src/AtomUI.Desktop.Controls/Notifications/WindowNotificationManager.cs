using System.Collections;
using System.Reactive.Linq;
using AtomUI.Controls;
using AtomUI.Theme;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Metadata;
using Avalonia.Controls.Primitives;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Threading;

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

    /// <summary>
    /// 通知卡片过期检测的轮询间隔，默认 200ms。
    /// </summary>
    public static readonly StyledProperty<TimeSpan> CardExpiredPollingIntervalProperty =
        AvaloniaProperty.Register<WindowNotificationManager, TimeSpan>(
            nameof(CardExpiredPollingInterval), TimeSpan.FromMilliseconds(80));

    /// <summary>
    /// 通知卡片关闭清理的轮询间隔，默认 200ms。
    /// </summary>
    public static readonly StyledProperty<TimeSpan> CleanupPollingIntervalProperty =
        AvaloniaProperty.Register<WindowNotificationManager, TimeSpan>(
            nameof(CleanupPollingInterval), TimeSpan.FromMilliseconds(150));

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

    /// <summary>
    /// 获取或设置通知卡片过期检测的轮询间隔。
    /// </summary>
    public TimeSpan CardExpiredPollingInterval
    {
        get => GetValue(CardExpiredPollingIntervalProperty);
        set => SetValue(CardExpiredPollingIntervalProperty, value);
    }

    /// <summary>
    /// 获取或设置通知卡片关闭清理的轮询间隔。
    /// </summary>
    public TimeSpan CleanupPollingInterval
    {
        get => GetValue(CleanupPollingIntervalProperty);
        set => SetValue(CleanupPollingIntervalProperty, value);
    }
    
    #endregion
    
    private TopLevel? _topLevel;
    private bool _isDisposed;
    private AdornerLayer? _adornerLayer;
    private IDisposable? _safeAreaMarginSubscription;
    private IList? _items;
    private Queue<NotificationCard>? _cleanupQueue;
    private HashSet<NotificationCard>? _cleanupSet;
    private DispatcherTimer? _cardExpiredTimer;
    private DispatcherTimer? _cleanupTimer;
    private List<PendingNotification>? _pendingNotifications;
    private NotificationPosition? _appliedPosition;

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
    }

    protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
    {
        base.OnApplyTemplate(e);
        var itemsControl = e.NameScope.Find<Panel>("PART_Items");
        _items = itemsControl?.Children;
        UpdatePseudoClasses(Position);
        FlushPendingNotifications();
    }

    protected override void OnDetachedFromVisualTree(VisualTreeAttachmentEventArgs e)
    {
        base.OnDetachedFromVisualTree(e);
        _pendingNotifications?.Clear();
        CleanupNotificationCards();
        _items?.Clear();
    }

    private void HandleCardExpiredTimer(object? sender, EventArgs eventArgs)
    {
        if (_items != null && _cardExpiredTimer is not null)
        {
            foreach (var item in _items)
            {
                if (item is NotificationCard card)
                {
                    if (card.NotifyCloseTick(_cardExpiredTimer.Interval))
                    {
                        if (EnsureCleanupSet().Add(card))
                        {
                            EnsureCleanupQueue().Enqueue(card);
                            var cleanupTimer = EnsureCleanupTimer();
                            if (!cleanupTimer.IsEnabled)
                            {
                                cleanupTimer.Start();
                            }
                        }
                    }
                }
            }
        }
    }

    private void HandleCleanupTimerTick(object? sender, EventArgs eventArgs)
    {
        if (_cleanupQueue is null)
        {
            _cleanupTimer?.Stop();
            return;
        }

        while (_cleanupQueue.Count > 0)
        {
            var card = _cleanupQueue.Peek();
            if (card.IsClosed)
            {
                _cleanupQueue.Dequeue();
                _cleanupSet?.Remove(card);
                continue;
            }

            if (!card.IsClosing)
            {
                card.Close();
                break;
            }

            break;
        }

        if (_cleanupQueue.Count == 0)
        {
            _cleanupTimer?.Stop();
        }
    }

    private void ConfigureExpiredTimer()
    {
        if (HasActiveExpiringCards())
        {
            EnsureCardExpiredTimer().Start();
        }
        else
        {
            _cardExpiredTimer?.Stop();
        }
    }

    public void Show(INotification notification, string[]? classes = null)
    {
        if (_isDisposed)
        {
            return;
        }

        Dispatcher.VerifyAccess();
        if (_items is null)
        {
            ApplyTemplate();
            if (_items is null)
            {
                (_pendingNotifications ??= new List<PendingNotification>()).Add(new PendingNotification(notification, classes));
                return;
            }
        }

        var expiration = notification.Expiration;
        var onClick    = notification.OnClick;
        var onClose    = notification.OnClose;

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
        notificationControl[!NotificationCard.IsMotionEnabledProperty] = this[!IsMotionEnabledProperty];

        // Add style classes if any
        if (classes?.Length > 0)
        {
            foreach (var cls in classes)
            {
                notificationControl.Classes.Add(cls);
            }
        }

        notificationControl.OnClick = onClick;
        notificationControl.OnClose = onClose;
        notificationControl.PointerPressed     += OnNotificationPointerPressed;
        notificationControl.NotificationClosed += OnNotificationClosed;

        Dispatcher.Post(() =>
        {
            if (_isDisposed || _items is null)
            {
                CleanupNotificationCard(notificationControl);
                return;
            }

            _items.Add(notificationControl);
            ConfigureExpiredTimer();
            RemoveExcessNotifications();
        });
    }

    private void FlushPendingNotifications()
    {
        if (_items is null || _pendingNotifications is null)
        {
            return;
        }

        var pendingNotifications = _pendingNotifications;
        _pendingNotifications = null;
        foreach (var pendingNotification in pendingNotifications)
        {
            Show(pendingNotification.Notification, pendingNotification.Classes);
        }
    }
    
    private static void OnNotificationPointerPressed(object? sender, PointerPressedEventArgs e)
    {
        if (sender is NotificationCard card)
        {
            card.OnClick?.Invoke();
        }
    }

    private void OnNotificationClosed(object? sender, RoutedEventArgs e)
    {
        if (sender is NotificationCard card)
        {
            var onClose = card.OnClose;
            CleanupNotificationCard(card);
            try
            {
                onClose?.Invoke();
            }
            finally
            {
                _items?.Remove(card);
                ConfigureExpiredTimer();
            }
        }
    }

    /// <summary>
    /// Removes excess notifications when the count exceeds MaxItems.
    /// </summary>
    private void RemoveExcessNotifications()
    {
        int visibleCount = 0;
        foreach (var item in _items!)
        {
            if (item is NotificationCard { IsClosing: false })
            {
                visibleCount++;
            }
        }

        var closeNeed = visibleCount - MaxItems;
        if (closeNeed <= 0)
        {
            return;
        }

        List<NotificationCard>? cardsToClose = null;
        foreach (var item in _items!)
        {
            if (item is NotificationCard { IsClosing: false } card)
            {
                (cardsToClose ??= new List<NotificationCard>()).Add(card);
                if (--closeNeed == 0)
                {
                    break;
                }
            }
        }

        if (cardsToClose is null)
        {
            return;
        }

        foreach (var card in cardsToClose)
        {
            card.Close();
        }
    }

    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        base.OnPropertyChanged(change);

        if (change.Property == PositionProperty)
        {
            UpdatePseudoClasses(change.GetNewValue<NotificationPosition>());
        }
        else if (change.Property == CardExpiredPollingIntervalProperty)
        {
            if (_cardExpiredTimer is not null)
            {
                _cardExpiredTimer.Interval = change.GetNewValue<TimeSpan>();
            }
        }
        else if (change.Property == CleanupPollingIntervalProperty)
        {
            if (_cleanupTimer is not null)
            {
                _cleanupTimer.Interval = change.GetNewValue<TimeSpan>();
            }
        }
    }
    
    private void InstallFromTopLevel(TopLevel topLevel)
    {
        topLevel.TemplateApplied -= TopLevelOnTemplateApplied;
        topLevel.TemplateApplied += TopLevelOnTemplateApplied;
        _adornerLayer            =  AdornerLayer.GetAdornerLayer(topLevel);
        if (_adornerLayer is not null)
        {
            _adornerLayer.Children.Add(this);
            AdornerLayer.SetAdornedElement(this, _adornerLayer);
        }

        // AdornerLayer 实测覆盖整个 TopLevel（含 CSD 装饰阴影 / 非 CSD 自绘阴影那一圈），
        // 不会跟着 VisualLayerManager.Margin 内缩。这里给 manager 自己加 Margin 把内容收到可见客户区，
        // 不然 Top/Right/Bottom/Left 的对齐都会贴到 AdornerLayer 边沿（即装饰外沿），卡片就漏到窗外了。
        _safeAreaMarginSubscription?.Dispose();
        if (topLevel is Window window)
        {
            _safeAreaMarginSubscription = window.GetObservable(Window.IsCsdEnabledProperty)
                                                .CombineLatest(
                                                    window.GetObservable(Avalonia.Controls.Window.WindowDecorationMarginProperty),
                                                    window.GetObservable(Window.FrameShadowThicknessProperty),
                                                    static (isCsd, wdm, fst) => isCsd ? wdm : fst)
                                                .Subscribe(margin => Margin = margin);
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
        _safeAreaMarginSubscription?.Dispose();
        _safeAreaMarginSubscription = null;
        if (_adornerLayer is not null)
        {
            _adornerLayer.Children.Remove(this);
            AdornerLayer.SetAdornedElement(this, null);
            _adornerLayer = null;
        }
    }
    
    public void Dispose()
    {
        if (_isDisposed)
        {
            return;
        }

        try
        {
            // 卸载事件订阅
            if (_topLevel is not null)
            {
                _topLevel.TemplateApplied -= TopLevelOnTemplateApplied;
            }

            CleanupNotificationCards();
            _pendingNotifications?.Clear();
            _pendingNotifications = null;
            _items?.Clear();

            _safeAreaMarginSubscription?.Dispose();
            _safeAreaMarginSubscription = null;

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
        if (_appliedPosition == position)
        {
            return;
        }

        if (_appliedPosition is { } previousPosition)
        {
            PseudoClasses.Set(GetPositionPseudoClass(previousPosition), false);
        }
        PseudoClasses.Set(GetPositionPseudoClass(position), true);
        _appliedPosition = position;
    }

    internal void StopExpiredTimer()
    {
        _cardExpiredTimer?.Stop();
    }

    internal void StartExpiredTimer()
    {
        ConfigureExpiredTimer();
    }

    private DispatcherTimer EnsureCardExpiredTimer()
    {
        if (_cardExpiredTimer is null)
        {
            _cardExpiredTimer = new DispatcherTimer
            {
                Interval = CardExpiredPollingInterval,
                Tag      = this
            };
            _cardExpiredTimer.Tick += HandleCardExpiredTimer;
        }

        return _cardExpiredTimer;
    }

    private DispatcherTimer EnsureCleanupTimer()
    {
        if (_cleanupTimer is null)
        {
            _cleanupTimer = new DispatcherTimer
            {
                Interval = CleanupPollingInterval,
                Tag      = this
            };
            _cleanupTimer.Tick += HandleCleanupTimerTick;
        }

        return _cleanupTimer;
    }

    private Queue<NotificationCard> EnsureCleanupQueue()
    {
        return _cleanupQueue ??= new Queue<NotificationCard>();
    }

    private HashSet<NotificationCard> EnsureCleanupSet()
    {
        return _cleanupSet ??= new HashSet<NotificationCard>();
    }

    private bool HasActiveExpiringCards()
    {
        if (_items is null)
        {
            return false;
        }

        foreach (var item in _items)
        {
            if (item is NotificationCard { IsClosing: false, Expiration: not null })
            {
                return true;
            }
        }

        return false;
    }

    private void CleanupNotificationCards()
    {
        if (_items is not null)
        {
            foreach (var item in _items)
            {
                if (item is NotificationCard card)
                {
                    CleanupNotificationCard(card);
                }
            }
        }

        _cleanupTimer?.Stop();
        if (_cleanupTimer is not null)
        {
            _cleanupTimer.Tick -= HandleCleanupTimerTick;
            _cleanupTimer = null;
        }

        _cardExpiredTimer?.Stop();
        if (_cardExpiredTimer is not null)
        {
            _cardExpiredTimer.Tick -= HandleCardExpiredTimer;
            _cardExpiredTimer = null;
        }

        _cleanupQueue?.Clear();
        _cleanupSet?.Clear();
    }

    private void CleanupNotificationCard(NotificationCard card)
    {
        card.PointerPressed -= OnNotificationPointerPressed;
        card.NotificationClosed -= OnNotificationClosed;
        card.OnClick = null;
        card.OnClose = null;
        _cleanupSet?.Remove(card);
    }

    private static string GetPositionPseudoClass(NotificationPosition position)
    {
        return position switch
        {
            NotificationPosition.TopLeft => NotificationPseudoClass.TopLeft,
            NotificationPosition.TopRight => NotificationPseudoClass.TopRight,
            NotificationPosition.BottomLeft => NotificationPseudoClass.BottomLeft,
            NotificationPosition.BottomRight => NotificationPseudoClass.BottomRight,
            NotificationPosition.TopCenter => NotificationPseudoClass.TopCenter,
            NotificationPosition.BottomCenter => NotificationPseudoClass.BottomCenter,
            _ => NotificationPseudoClass.TopRight
        };
    }

    private readonly record struct PendingNotification(INotification Notification, string[]? Classes);
}
