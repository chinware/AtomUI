using AtomUI.Controls;
using AtomUI.Icons.AntDesign;
using AtomUI.MotionScene;
using AtomUI.Reflection;
using AtomUI.Theme;
using Avalonia;
using Avalonia.Animation.Easings;
using Avalonia.Controls;
using Avalonia.Controls.Metadata;
using Avalonia.Controls.Primitives;
using Avalonia.Data;
using Avalonia.Threading;
using Avalonia.Input;
using Avalonia.Interactivity;

namespace AtomUI.Desktop.Controls;

[PseudoClasses(StdPseudoClass.Error, StdPseudoClass.Information, StdPseudoClass.Success, StdPseudoClass.Warning)]
public class NotificationCard : ContentControl, IMotionAwareControl
{
    internal const double AnimationMaxOffsetY = 150d;
    internal const double AnimationMaxOffsetX = 500d;

    #region 公共属性定义
    
    public static readonly DirectProperty<NotificationCard, bool> IsClosingProperty =
        AvaloniaProperty.RegisterDirect<NotificationCard, bool>(nameof(IsClosing), o => o.IsClosing);
    
    public static readonly StyledProperty<bool> IsClosedProperty =
        AvaloniaProperty.Register<NotificationCard, bool>(nameof(IsClosed));

    public static readonly StyledProperty<bool> IsShowProgressProperty =
        AvaloniaProperty.Register<NotificationCard, bool>(nameof(IsShowProgress));
    
    public static readonly StyledProperty<NotificationType> NotificationTypeProperty =
        AvaloniaProperty.Register<NotificationCard, NotificationType>(nameof(NotificationType));

    public static readonly StyledProperty<bool> IsMotionEnabledProperty =
        MotionAwareControlProperty.IsMotionEnabledProperty.AddOwner<NotificationCard>();
    
    public static readonly RoutedEvent<RoutedEventArgs> NotificationClosedEvent =
        RoutedEvent.Register<NotificationCard, RoutedEventArgs>(nameof(NotificationClosed), RoutingStrategies.Bubble);

    public static readonly StyledProperty<string> TitleProperty =
        AvaloniaProperty.Register<NotificationCard, string>(nameof(Title));

    public static readonly StyledProperty<PathIcon?> IconProperty =
        AvaloniaProperty.Register<NotificationCard, PathIcon?>(nameof(Icon));
    
    public static readonly StyledProperty<TimeSpan?> ExpirationProperty =
        AvaloniaProperty.Register<NotificationCard, TimeSpan?>(nameof(Expiration));
    
    public bool IsClosing
    {
        get => _isClosing;
        private set => SetAndRaise(IsClosingProperty, ref _isClosing, value);
    }
    
    public bool IsClosed
    {
        get => GetValue(IsClosedProperty);
        set => SetValue(IsClosedProperty, value);
    }

    public bool IsShowProgress
    {
        get => GetValue(IsShowProgressProperty);
        set => SetValue(IsShowProgressProperty, value);
    }

    public NotificationType NotificationType
    {
        get => GetValue(NotificationTypeProperty);
        set => SetValue(NotificationTypeProperty, value);
    }

    public string Title
    {
        get => GetValue(TitleProperty);
        set => SetValue(TitleProperty, value);
    }

    public PathIcon? Icon
    {
        get => GetValue(IconProperty);
        set => SetValue(IconProperty, value);
    }
    
    /// <summary>
    /// Gets the expiration time of the notification after which it will automatically close.
    /// If the value is null then the notification will remain open until the user closes it.
    /// </summary>
    public TimeSpan? Expiration
    {
        get => GetValue(ExpirationProperty);
        set => SetValue(ExpirationProperty, value);
    }

    public event EventHandler<RoutedEventArgs>? NotificationClosed
    {
        add => AddHandler(NotificationClosedEvent, value);
        remove => RemoveHandler(NotificationClosedEvent, value);
    }

    public bool IsMotionEnabled
    {
        get => GetValue(IsMotionEnabledProperty);
        set => SetValue(IsMotionEnabledProperty, value);
    }
    
    #endregion

    #region 内部属性定义

    /// <summary>回调：用户点击通知卡片时触发，由 WindowNotificationManager 设置以避免 lambda 闭包。</summary>
    internal Action? OnClick { get; set; }

    /// <summary>回调：通知卡片关闭时触发，由 WindowNotificationManager 设置以避免 lambda 闭包。</summary>
    internal Action? OnClose { get; set; }

    internal static readonly DirectProperty<NotificationCard, NotificationPosition> PositionProperty =
        AvaloniaProperty.RegisterDirect<NotificationCard, NotificationPosition>(
            nameof(Position),
            o => o.Position,
            (o, v) => o.Position = v);

    internal static readonly DirectProperty<NotificationCard, TimeSpan> OpenCloseMotionDurationProperty =
        AvaloniaProperty.RegisterDirect<NotificationCard, TimeSpan>(nameof(OpenCloseMotionDuration),
            o => o.OpenCloseMotionDuration,
            (o, v) => o.OpenCloseMotionDuration = v);

    private NotificationPosition _position;

    internal NotificationPosition Position
    {
        get => _position;
        set => SetAndRaise(PositionProperty, ref _position, value);
    }

    private TimeSpan _openCloseMotionDuration;

    internal TimeSpan OpenCloseMotionDuration
    {
        get => _openCloseMotionDuration;
        set => SetAndRaise(OpenCloseMotionDurationProperty, ref _openCloseMotionDuration, value);
    }
    
    #endregion

    private bool _isClosing;
    private readonly WindowNotificationManager _notificationManager;
    private IconButton? _closeButton;
    private BaseMotionActor? _motionActor;
    private Grid? _contentLayout;
    private NotificationProgressBar? _progressBar;
    private PathIcon? _generatedDefaultIcon;
    private NotificationType? _generatedDefaultIconType;
    private TimeSpan? _initialExpiration;
    private bool _isTemplateApplied;
    private bool _isUpdatingDefaultIcon;
    private NotificationType? _appliedNotificationType;
    private NotificationPosition? _appliedPosition;

    /// <summary>
    /// Initializes a new instance of the <see cref="NotificationCard" /> class.
    /// </summary>
    public NotificationCard(WindowNotificationManager manager)
    {
        this.RegisterTokenResourceScope(NotificationToken.ScopeProvider);
        _notificationManager = manager;
    }
    
    public void Close()
    {
        if (IsClosing)
        {
            return;
        }

        IsClosing = true;
    }

    protected override void OnAttachedToVisualTree(VisualTreeAttachmentEventArgs e)
    {
        base.OnAttachedToVisualTree(e);
        SetupPositionPseudoClasses(Position);
        SetupNotificationTypePseudoClasses();
        SetupDefaultNotificationIcon();
        if (_closeButton is not null)
        {
            _closeButton.Click -= HandleCloseButtonClose;
            _closeButton.Click += HandleCloseButtonClose;
        }
        EnsureProgressBarState();
    }

    protected override void OnDetachedFromVisualTree(VisualTreeAttachmentEventArgs e)
    {
        base.OnDetachedFromVisualTree(e);
        if (_closeButton is not null)
        {
            _closeButton.Click -= HandleCloseButtonClose;
        }
        ReleaseProgressBar();
    }

    protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
    {
        base.OnApplyTemplate(e);

        if (_closeButton is not null)
        {
            _closeButton.Click -= HandleCloseButtonClose;
        }
        ReleaseProgressBar();
        _isTemplateApplied = true;
        _initialExpiration ??= Expiration;
        _closeButton       = e.NameScope.Find<IconButton>("PART_CloseButton");
        _motionActor       = e.NameScope.Find<BaseMotionActor>(BaseMotionActor.MotionActorPart);
        _contentLayout     = e.NameScope.Find<Grid>("PART_ContentLayout");

        if (_closeButton is not null)
        {
            _closeButton.Click += HandleCloseButtonClose;
        }

        if (_motionActor is not null && IsMotionEnabled)
        {
            _motionActor.Opacity = 0;
            Dispatcher.InvokeAsync(ApplyShowMotionAsync, DispatcherPriority.Loaded);
        }

        SetupPositionPseudoClasses(Position);
        SetupNotificationTypePseudoClasses();
        SetupDefaultNotificationIcon();
        EnsureProgressBarState();
    }

    private async Task ApplyShowMotionAsync()
    {
        if (_motionActor is null || !IsMotionEnabled)
        {
            return;
        }

        AbstractMotion? motion;
        if (Position == NotificationPosition.TopLeft || Position == NotificationPosition.BottomLeft)
        {
            motion = new NotificationMoveLeftInMotion(AnimationMaxOffsetX,
                _openCloseMotionDuration, new CubicEaseOut());
        }
        else if (Position == NotificationPosition.TopRight || Position == NotificationPosition.BottomRight)
        {
            motion = new NotificationMoveRightInMotion(AnimationMaxOffsetX, _openCloseMotionDuration, new CubicEaseOut());
        }
        else if (Position == NotificationPosition.TopCenter)
        {
            motion = new NotificationMoveUpInMotion(AnimationMaxOffsetY, _openCloseMotionDuration,
                new CubicEaseOut());
        }
        else
        {
            motion = new NotificationMoveDownInMotion(AnimationMaxOffsetY, _openCloseMotionDuration,
                new CubicEaseOut());
        }
        await motion.RunAsync(_motionActor);
        _motionActor.Opacity = 1;
    }

    private async Task ApplyHideMotionAsync()
    {
        if (_motionActor is null || !IsMotionEnabled)
        {
            IsClosed = true;
            return;
        }

        AbstractMotion? motion;
        if (Position == NotificationPosition.TopLeft || Position == NotificationPosition.BottomLeft)
        {
            motion = new NotificationMoveLeftOutMotion(AnimationMaxOffsetX, _openCloseMotionDuration,
                new CubicEaseIn());
        }
        else if (Position == NotificationPosition.TopRight || Position == NotificationPosition.BottomRight)
        {
            motion = new NotificationMoveRightOutMotion(AnimationMaxOffsetX, _openCloseMotionDuration,
                new CubicEaseIn());
        }
        else if (Position == NotificationPosition.TopCenter)
        {
            motion = new NotificationMoveUpOutMotion(AnimationMaxOffsetY, _openCloseMotionDuration,
                new CubicEaseIn());
        }
        else
        {
            motion = new NotificationMoveDownOutMotion(AnimationMaxOffsetY, _openCloseMotionDuration,
                new CubicEaseIn());
        }

        await motion.RunAsync(_motionActor);
        IsClosed = true;
    }

    private void HandleCloseButtonClose(object? sender, EventArgs args)
    {
        Close();
    }

    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        base.OnPropertyChanged(change);

        if (change.Property == NotificationTypeProperty)
        {
            if (_isTemplateApplied)
            {
                SetupNotificationTypePseudoClasses();
                SetupDefaultNotificationIcon();
            }
        }
        else if (change.Property == IconProperty)
        {
            if (_isUpdatingDefaultIcon)
            {
                return;
            }

            if (Icon is null && _isTemplateApplied)
            {
                SetupDefaultNotificationIcon();
            }
            else if (!ReferenceEquals(Icon, _generatedDefaultIcon))
            {
                _generatedDefaultIcon     = null;
                _generatedDefaultIconType = null;
            }
        }
        else if (change.Property == IsShowProgressProperty || change.Property == ExpirationProperty)
        {
            EnsureProgressBarState();
            if (change.Property == ExpirationProperty && _progressBar is not null)
            {
                _progressBar.CurrentExpiration = Expiration;
            }
        }

        if (change.Property == IsClosedProperty)
        {
            if (!IsClosing && !IsClosed)
            {
                return;
            }

            RaiseEvent(new RoutedEventArgs(NotificationClosedEvent));
        }
        else if (change.Property == PositionProperty)
        {
            SetupPositionPseudoClasses(change.GetNewValue<NotificationPosition>());
        } 
        else if (change.Property == IsClosingProperty)
        {
            if (IsClosing)
            {
                if (_motionActor is null || !IsMotionEnabled)
                {
                    IsClosed = true;
                }
                else
                {
                    Dispatcher.InvokeAsync(ApplyHideMotionAsync);
                }
            }
        }
    }

    private void SetupNotificationTypePseudoClasses()
    {
        if (_appliedNotificationType == NotificationType)
        {
            return;
        }

        if (_appliedNotificationType is { } previousNotificationType)
        {
            PseudoClasses.Set(GetNotificationTypePseudoClass(previousNotificationType), false);
        }
        PseudoClasses.Set(GetNotificationTypePseudoClass(NotificationType), true);
        _appliedNotificationType = NotificationType;
    }

    private void SetupDefaultNotificationIcon()
    {
        if (Icon is not null)
        {
            if (!ReferenceEquals(Icon, _generatedDefaultIcon))
            {
                return;
            }

            if (_generatedDefaultIconType == NotificationType)
            {
                return;
            }
        }

        var icon = CreateDefaultNotificationIcon();
        _generatedDefaultIcon     = icon;
        _generatedDefaultIconType = NotificationType;
        _isUpdatingDefaultIcon    = true;
        try
        {
            ClearValue(IconProperty);
            SetValue(IconProperty, icon, BindingPriority.Template);
        }
        finally
        {
            _isUpdatingDefaultIcon = false;
        }
    }

    private PathIcon? CreateDefaultNotificationIcon()
    {
        if (NotificationType == NotificationType.Information)
        {
            return new InfoCircleFilled();
        }
        if (NotificationType == NotificationType.Success)
        {
            return new CheckCircleFilled();
        }
        if (NotificationType == NotificationType.Error)
        {
            return new CloseCircleFilled();
        }
        if (NotificationType == NotificationType.Warning)
        {
            return new ExclamationCircleFilled();
        }

        return null;
    }

    internal bool NotifyCloseTick(TimeSpan cycleDuration)
    {
        if (Expiration is null)
        {
            return false;
        }

        Expiration -= cycleDuration;

        if (Expiration.Value.TotalMilliseconds < 0)
        {
            return true;
        }

        return false;
    }

    protected override void OnPointerEntered(PointerEventArgs e)
    {
        base.OnPointerEntered(e);
        if (_notificationManager.IsPauseOnHover)
        {
            _notificationManager.StopExpiredTimer();
        }
    }
    
    protected override void OnPointerMoved(PointerEventArgs e)
    {
        base.OnPointerMoved(e);
        if (_notificationManager.IsPauseOnHover)
        {
            _notificationManager.StopExpiredTimer();
        }
    }
    
    protected override void OnPointerExited(PointerEventArgs e)
    {
        base.OnPointerExited(e);
        if (_notificationManager.IsPauseOnHover)
        {
            _notificationManager.StartExpiredTimer();
        }
    }

    private void SetupPositionPseudoClasses(NotificationPosition position)
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

    private void EnsureProgressBarState()
    {
        if (!_isTemplateApplied || _contentLayout is null)
        {
            return;
        }

        if (!ShouldShowProgressBar())
        {
            ReleaseProgressBar();
            return;
        }

        if (_progressBar is not null)
        {
            return;
        }

        _progressBar = new NotificationProgressBar
        {
            Name              = "ProgressBar",
            Expiration        = _initialExpiration ?? Expiration.GetValueOrDefault(),
            CurrentExpiration = Expiration
        };
        _progressBar.SetTemplatedParent(this);
        Grid.SetRow(_progressBar, 1);
        Grid.SetColumn(_progressBar, 0);
        Grid.SetColumnSpan(_progressBar, 2);
        _contentLayout.Children.Add(_progressBar);
    }

    private bool ShouldShowProgressBar()
    {
        return IsShowProgress && Expiration is { } expiration && expiration > TimeSpan.Zero;
    }

    private void ReleaseProgressBar()
    {
        if (_progressBar is null)
        {
            return;
        }

        _contentLayout?.Children.Remove(_progressBar);
        _progressBar.SetTemplatedParent(null);
        _progressBar = null;
    }

    private static string GetNotificationTypePseudoClass(NotificationType notificationType)
    {
        return notificationType switch
        {
            NotificationType.Error => StdPseudoClass.Error,
            NotificationType.Information => StdPseudoClass.Information,
            NotificationType.Success => StdPseudoClass.Success,
            NotificationType.Warning => StdPseudoClass.Warning,
            _ => StdPseudoClass.Information
        };
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
}
