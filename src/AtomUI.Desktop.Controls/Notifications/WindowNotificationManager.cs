using System.Collections;
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
    private readonly Queue<NotificationCard> _cleanupQueue;
    private readonly HashSet<NotificationCard> _cleanupSet;
    private DispatcherTimer? _cardExpiredTimer;
    private DispatcherTimer? _cleanupTimer;
    private readonly Queue<PendingNotification> _pendingNotifications = new();
    private const int MaxAdornerLayerRetryCount = 30;
    private bool _adornerLayerRetryScheduled;
    private int _adornerLayerRetryCount;

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
        _cardExpiredTimer      =  new DispatcherTimer { Interval = CardExpiredPollingIntervalProperty.GetDefaultValue(typeof(WindowNotificationManager)), Tag = this };
        _cardExpiredTimer.Tick += HandleCardExpiredTimer;
        _cleanupTimer          =  new DispatcherTimer { Interval = CleanupPollingIntervalProperty.GetDefaultValue(typeof(WindowNotificationManager)), Tag = this };
        _cleanupTimer.Tick     += HandleCleanupTimerTick;
        var maxItems = MaxItemsProperty.GetDefaultValue(typeof(WindowNotificationManager));
        _cleanupQueue          =  new Queue<NotificationCard>(maxItems);
        _cleanupSet            =  new HashSet<NotificationCard>(maxItems);
    }

    protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
    {
        base.OnApplyTemplate(e);
        var itemsControl = e.NameScope.Find<Panel>("PART_Items");
        _items = itemsControl?.Children;
        UpdatePseudoClasses(Position);
        FlushPendingNotifications();
    }

    private void HandleCardExpiredTimer(object? sender, EventArgs eventArgs)
    {
        var cardExpiredTimer = _cardExpiredTimer;
        if (_items != null && cardExpiredTimer != null)
        {
            foreach (var item in _items)
            {
                if (item is NotificationCard card)
                {
                    if (card.NotifyCloseTick(cardExpiredTimer.Interval))
                    {
                        if (_cleanupSet.Add(card))
                        {
                            _cleanupQueue.Enqueue(card);
                            if (_cleanupTimer is { IsEnabled: false } cleanupTimer)
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
        var cleanupTimer = _cleanupTimer;
        if (cleanupTimer is null)
        {
            return;
        }

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
                _cleanupSet.Remove(card);
                if (_cleanupQueue.Count == 0)
                {
                    cleanupTimer.Stop();
                }
            }
        }
    }

    private void ConfigureExpiredTimer()
    {
        var cardExpiredTimer = _cardExpiredTimer;
        if (cardExpiredTimer is null)
        {
            return;
        }

        var hasExpiringCard = false;
        if (_items is not null)
        {
            foreach (var item in _items)
            {
                if (item is NotificationCard { Expiration: not null, IsClosing: false, IsClosed: false })
                {
                    hasExpiringCard = true;
                    break;
                }
            }
        }

        if (hasExpiringCard)
        {
            cardExpiredTimer.Start();
        }
        else
        {
            cardExpiredTimer.Stop();
        }
    }

    public void Show(INotification notification, string[]? classes = null)
    {
        Dispatcher.VerifyAccess();

        if (_isDisposed)
        {
            return;
        }

        if (_items is null)
        {
            _pendingNotifications.Enqueue(new PendingNotification(notification, classes));
            ApplyTemplate();
            return;
        }

        ShowCore(notification, classes);
    }

    private void ShowCore(INotification notification, string[]? classes)
    {
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
            if (_items is null || _isDisposed)
            {
                return;
            }

            _items?.Add(notificationControl);
            ConfigureExpiredTimer();
            RemoveExcessNotifications();
        });
    }

    private void FlushPendingNotifications()
    {
        if (_items is null || _isDisposed)
        {
            return;
        }

        while (_pendingNotifications.Count > 0)
        {
            var pendingNotification = _pendingNotifications.Dequeue();
            ShowCore(pendingNotification.Notification, pendingNotification.Classes);
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
            card.OnClose?.Invoke();
            _items?.Remove(card);
            ConfigureExpiredTimer();
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

        foreach (var item in _items!)
        {
            if (item is NotificationCard { IsClosing: false } card)
            {
                card.Close();
                if (--closeNeed == 0)
                {
                    break;
                }
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
        TryInstallAdornerLayer(topLevel);

        // AdornerLayer 实测覆盖整个 TopLevel（含 CSD 装饰阴影 / 非 CSD 自绘阴影那一圈），
        // 不会跟着 VisualLayerManager.Margin 内缩。这里给 manager 自己加 Margin 把内容收到可见客户区，
        // 不然 Top/Right/Bottom/Left 的对齐都会贴到 AdornerLayer 边沿（即装饰外沿），卡片就漏到窗外了。
        _safeAreaMarginSubscription?.Dispose();
        _safeAreaMarginSubscription = TopLevelMarginBinder.BindHostMargin(topLevel, margin => Margin = margin);
    }

    private void TryInstallAdornerLayer(TopLevel topLevel)
    {
        _adornerLayer = AdornerLayer.GetAdornerLayer(topLevel);
        if (_adornerLayer is null)
        {
            ScheduleAdornerLayerRetry(topLevel);
            return;
        }

        _adornerLayerRetryCount     = 0;
        _adornerLayerRetryScheduled = false;
        if (!_adornerLayer.Children.Contains(this))
        {
            _adornerLayer.Children.Add(this);
        }
        AdornerLayer.SetAdornedElement(this, _adornerLayer);
    }

    private void ScheduleAdornerLayerRetry(TopLevel topLevel)
    {
        if (_adornerLayerRetryScheduled ||
            _isDisposed ||
            _adornerLayerRetryCount >= MaxAdornerLayerRetryCount)
        {
            return;
        }

        _adornerLayerRetryScheduled = true;
        if (_adornerLayerRetryCount == 0)
        {
            Dispatcher.UIThread.Post(() => RetryInstallFromTopLevel(topLevel), DispatcherPriority.Loaded);
        }
        else
        {
            DispatcherTimer.RunOnce(() => RetryInstallFromTopLevel(topLevel), TimeSpan.FromMilliseconds(16));
        }
    }

    private void RetryInstallFromTopLevel(TopLevel topLevel)
    {
        if (!_adornerLayerRetryScheduled)
        {
            return;
        }

        _adornerLayerRetryScheduled = false;
        if (_isDisposed ||
            _adornerLayer is not null ||
            !ReferenceEquals(_topLevel, topLevel))
        {
            return;
        }

        _adornerLayerRetryCount++;
        TryInstallAdornerLayer(topLevel);
    }

    private void TopLevelOnTemplateApplied(object? sender, TemplateAppliedEventArgs e)
    {
        RemoveFromAdornerLayer();
        _adornerLayerRetryScheduled = false;
        _adornerLayerRetryCount     = 0;

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
            // 停止定时器并取消 Tick 事件订阅
            if (_cleanupTimer is not null)
            {
                _cleanupTimer.Stop();
                _cleanupTimer.Tick -= HandleCleanupTimerTick;
                _cleanupTimer = null;
            }
            if (_cardExpiredTimer is not null)
            {
                _cardExpiredTimer.Stop();
                _cardExpiredTimer.Tick -= HandleCardExpiredTimer;
                _cardExpiredTimer = null;
            }

            _items?.Clear();
            _cleanupQueue.Clear();
            _cleanupSet.Clear();
            _pendingNotifications.Clear();
            _adornerLayerRetryScheduled = false;
            _adornerLayerRetryCount = 0;
            // 卸载事件订阅
            if (_topLevel is not null)
            {
                _topLevel.TemplateApplied -= TopLevelOnTemplateApplied;
            }

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
        PseudoClasses.Set(NotificationPseudoClass.TopLeft, position == NotificationPosition.TopLeft);
        PseudoClasses.Set(NotificationPseudoClass.TopRight, position == NotificationPosition.TopRight);
        PseudoClasses.Set(NotificationPseudoClass.BottomLeft, position == NotificationPosition.BottomLeft);
        PseudoClasses.Set(NotificationPseudoClass.BottomRight, position == NotificationPosition.BottomRight);
        PseudoClasses.Set(NotificationPseudoClass.TopCenter, position == NotificationPosition.TopCenter);
        PseudoClasses.Set(NotificationPseudoClass.BottomCenter, position == NotificationPosition.BottomCenter);
    }

    internal void StopExpiredTimer()
    {
        _cardExpiredTimer?.Stop();
    }

    internal void StartExpiredTimer()
    {
        ConfigureExpiredTimer();
    }

    private readonly record struct PendingNotification(INotification Notification, string[]? Classes);
}
