using System.Collections;
using System.Reactive.Linq;
using AtomUI.Controls;
using AtomUI.Theme;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Metadata;
using Avalonia.Controls.Primitives;
using Avalonia.Interactivity;
using Avalonia.Layout;
using Avalonia.Threading;

namespace AtomUI.Desktop.Controls;

[TemplatePart("PART_Items", typeof(Panel))]
public class WindowMessageManager : TemplatedControl, IMessageManager, IMotionAwareControl, IDisposable
{
    #region 公共属性定义

    /// <summary>
    /// Defines the <see cref="Position" /> property.
    /// </summary>
    public static readonly StyledProperty<NotificationPosition> PositionProperty =
        AvaloniaProperty.Register<WindowMessageManager, NotificationPosition>(
            nameof(Position), NotificationPosition.TopRight);

    public static readonly StyledProperty<bool> IsMotionEnabledProperty =
        MotionAwareControlProperty.IsMotionEnabledProperty.AddOwner<WindowMessageManager>();

    /// <summary>
    /// Defines which corner of the screen notifications can be displayed in.
    /// </summary>
    /// <seealso cref="NotificationPosition" />
    public NotificationPosition Position
    {
        get => GetValue(PositionProperty);
        set => SetValue(PositionProperty, value);
    }

    /// <summary>
    /// Defines the <see cref="MaxItems" /> property.
    /// </summary>
    public static readonly StyledProperty<int> MaxItemsProperty =
        AvaloniaProperty.Register<WindowMessageManager, int>(nameof(MaxItems), 5);

    /// <summary>
    /// Defines the maximum number of notifications visible at once.
    /// </summary>
    public int MaxItems
    {
        get => GetValue(MaxItemsProperty);
        set => SetValue(MaxItemsProperty, value);
    }

    public bool IsMotionEnabled
    {
        get => GetValue(IsMotionEnabledProperty);
        set => SetValue(IsMotionEnabledProperty, value);
    }

    #endregion

    private IList? _items;
    private TopLevel? _topLevel;
    private bool _isDisposed;
    private AdornerLayer? _adornerLayer;
    private IDisposable? _safeAreaMarginSubscription;
    private readonly Queue<PendingMessage> _pendingMessages = new();
    private readonly Dictionary<MessageCard, IDisposable> _messageCloseTimers = new();
    private const int MaxAdornerLayerRetryCount = 30;
    private bool _adornerLayerRetryScheduled;
    private int _adornerLayerRetryCount;

    /// <summary>
    /// Initializes a new instance of the <see cref="WindowNotificationManager" /> class.
    /// </summary>
    /// <param name="host">The TopLevel that will host the control.</param>
    public WindowMessageManager(TopLevel? host)
    {
        this.RegisterTokenResourceScope(MessageToken.ScopeProvider);

        if (host is not null)
        {
            _topLevel = host;
            InstallFromTopLevel(host);
        }
    }

    static WindowMessageManager()
    {
        HorizontalAlignmentProperty.OverrideDefaultValue<WindowMessageManager>(HorizontalAlignment.Stretch);
        VerticalAlignmentProperty.OverrideDefaultValue<WindowMessageManager>(VerticalAlignment.Stretch);
    }

    protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
    {
        base.OnApplyTemplate(e);

        var itemsControl = e.NameScope.Find<Panel>("PART_Items");
        _items = itemsControl?.Children;
        FlushPendingMessages();
    }

    protected override void OnDetachedFromVisualTree(VisualTreeAttachmentEventArgs e)
    {
        base.OnDetachedFromVisualTree(e);
        _items?.Clear();
    }

    /// <summary>
    /// Shows a Notification
    /// </summary>
    /// <param name="message">the content of the message</param>
    /// <param name="classes">style classes to apply</param>
    public void Show(IMessage message, string[]? classes = null)
    {
        Dispatcher.VerifyAccess();

        if (_isDisposed)
        {
            return;
        }

        if (_items is null)
        {
            _pendingMessages.Enqueue(new PendingMessage(message, classes));
            ApplyTemplate();
            return;
        }

        ShowCore(message, classes);
    }

    private void ShowCore(IMessage message, string[]? classes)
    {
        var messageControl = new MessageCard
        {
            Icon        = message.Icon,
            Message     = message.Content,
            MessageType = message.Type
        };
        messageControl[!MessageCard.IsMotionEnabledProperty] = this[!IsMotionEnabledProperty];
        
        // Add style classes if any
        if (classes?.Length > 0)
        {
            foreach (var cls in classes)
            {
                messageControl.Classes.Add(cls);
            }
        }

        messageControl.OnClose = message.OnClose;
        messageControl.MessageClosed += OnMessageClosed;

        Dispatcher.Post(() =>
        {
            if (_items is null || _isDisposed)
            {
                return;
            }

            _items.Add(messageControl);
            RemoveExcessMessages();
        });

        // Auto-close after expiration time
        if (message.Expiration != TimeSpan.Zero)
        {
            _messageCloseTimers[messageControl] = DispatcherTimer.RunOnce(messageControl.Close, message.Expiration);
        }
    }

    private void FlushPendingMessages()
    {
        if (_items is null || _isDisposed)
        {
            return;
        }

        while (_pendingMessages.Count > 0)
        {
            var pendingMessage = _pendingMessages.Dequeue();
            ShowCore(pendingMessage.Message, pendingMessage.Classes);
        }
    }

    private void OnMessageClosed(object? sender, RoutedEventArgs e)
    {
        if (sender is MessageCard card)
        {
            if (_messageCloseTimers.Remove(card, out var closeTimer))
            {
                closeTimer.Dispose();
            }
            card.OnClose?.Invoke();
            _items?.Remove(card);
        }
    }

    /// <summary>
    /// Removes excess messages when the count exceeds MaxItems.
    /// </summary>
    private void RemoveExcessMessages()
    {
        var visibleCount = 0;
        foreach (var item in _items!)
        {
            if (item is MessageCard { IsClosing: false })
            {
                visibleCount++;
            }
        }

        var excessCount = visibleCount - MaxItems;

        if (excessCount > 0)
        {
            foreach (var item in _items)
            {
                if (item is not MessageCard { IsClosing: false } messageCard)
                {
                    continue;
                }

                messageCard.Close();
                excessCount--;
                if (excessCount == 0)
                {
                    break;
                }
            }
        }
    }
    
    private void InstallFromTopLevel(TopLevel topLevel)
    {
        topLevel.TemplateApplied -= TopLevelOnTemplateApplied;
        topLevel.TemplateApplied += TopLevelOnTemplateApplied;
        TryInstallAdornerLayer(topLevel);

        // AdornerLayer 覆盖整个 TopLevel（含 CSD 装饰阴影 / 非 CSD 自绘阴影那一圈）不会跟着
        // VisualLayerManager.Margin 内缩。给 manager 自己加 Margin 把内容收到可见客户区，
        // 否则 Top/Center 等对齐会贴到装饰外沿、卡片漏到窗外。
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
            _items?.Clear();
            _pendingMessages.Clear();
            foreach (var closeTimer in _messageCloseTimers.Values)
            {
                closeTimer.Dispose();
            }
            _messageCloseTimers.Clear();
            _adornerLayerRetryScheduled = false;
            _adornerLayerRetryCount = 0;
            _safeAreaMarginSubscription?.Dispose();
            _safeAreaMarginSubscription = null;
            // 从 AdornerLayer 中移除
            if (_adornerLayer is not null)
            {
                _adornerLayer.Children.Remove(this);
                AdornerLayer.SetAdornedElement(this, null);
                _adornerLayer = null;
            }
            _topLevel   = null;
            _items      = null;
            _isDisposed = true;
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error uninstalling from TopLevel: {ex.Message}");
        }
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

    private void TopLevelOnTemplateApplied(object? sender, TemplateAppliedEventArgs _)
    {
        RemoveFromAdornerLayer();
        _adornerLayerRetryScheduled = false;
        _adornerLayerRetryCount     = 0;
        
        // Reinstall notification manager on template reapplied.
        var topLevel = (TopLevel)sender!;
        topLevel.TemplateApplied -= TopLevelOnTemplateApplied;
        InstallFromTopLevel(topLevel);
    }

    private readonly record struct PendingMessage(IMessage Message, string[]? Classes);
}
