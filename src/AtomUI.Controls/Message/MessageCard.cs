using AtomUI.IconPkg;
using AtomUI.IconPkg.AntDesign;
using AtomUI.MotionScene;
using AtomUI.Theme.Data;
using AtomUI.Theme.Styling;
using Avalonia;
using Avalonia.Animation.Easings;
using Avalonia.Controls;
using Avalonia.Controls.Metadata;
using Avalonia.Controls.Primitives;
using Avalonia.Interactivity;

namespace AtomUI.Controls;

[PseudoClasses(ErrorPC, InformationPC, SuccessPC, WarningPC, LoadingPC)]
public class MessageCard : TemplatedControl
{
    public const string ErrorPC = ":error";
    public const string InformationPC = ":information";
    public const string SuccessPC = ":success";
    public const string WarningPC = ":warning";
    public const string LoadingPC = ":loading";
    
    internal const double AnimationMaxOffsetY = 100d;

    #region 公共属性定义

    /// <summary>
    /// Defines the <see cref="IsClosing" /> property.
    /// </summary>
    public static readonly DirectProperty<MessageCard, bool> IsClosingProperty =
        AvaloniaProperty.RegisterDirect<MessageCard, bool>(nameof(IsClosing), o => o.IsClosing);

    /// <summary>
    /// Defines the <see cref="IsClosed" /> property.
    /// </summary>
    public static readonly StyledProperty<bool> IsClosedProperty =
        AvaloniaProperty.Register<MessageCard, bool>(nameof(IsClosed));

    /// <summary>
    /// Defines the <see cref="NotificationType" /> property
    /// </summary>
    public static readonly StyledProperty<MessageType> MessageTypeProperty =
        AvaloniaProperty.Register<MessageCard, MessageType>(nameof(NotificationType));

    /// <summary>
    /// Defines the <see cref="MessageClosed" /> event.
    /// </summary>
    public static readonly RoutedEvent<RoutedEventArgs> MessageClosedEvent =
        RoutedEvent.Register<MessageCard, RoutedEventArgs>(nameof(MessageClosed), RoutingStrategies.Bubble);

    public static readonly StyledProperty<Icon?> IconProperty
        = AvaloniaProperty.Register<MessageCard, Icon?>(nameof(Icon));

    public static readonly StyledProperty<string> MessageProperty =
        AvaloniaProperty.Register<NotificationCard, string>(nameof(Message));

    /// <summary>
    /// Determines if the notification is already closing.
    /// </summary>
    public bool IsClosing
    {
        get => _isClosing;
        private set => SetAndRaise(IsClosingProperty, ref _isClosing, value);
    }

    /// <summary>
    /// Determines if the notification is closed.
    /// </summary>
    public bool IsClosed
    {
        get => GetValue(IsClosedProperty);
        set => SetValue(IsClosedProperty, value);
    }

    /// <summary>
    /// Gets or sets the type of the notification
    /// </summary>
    public MessageType MessageType
    {
        get => GetValue(MessageTypeProperty);
        set => SetValue(MessageTypeProperty, value);
    }

    /// <summary>
    /// Raised when the <see cref="MessageCard" /> has closed.
    /// </summary>
    public event EventHandler<RoutedEventArgs>? MessageClosed
    {
        add => AddHandler(MessageClosedEvent, value);
        remove => RemoveHandler(MessageClosedEvent, value);
    }

    public Icon? Icon
    {
        get => GetValue(IconProperty);
        set => SetValue(IconProperty, value);
    }

    public string Message
    {
        get => GetValue(MessageProperty);
        set => SetValue(MessageProperty, value);
    }

    #endregion

    #region 内部属性定义

    internal static readonly DirectProperty<MessageCard, TimeSpan> OpenCloseMotionDurationProperty =
        AvaloniaProperty.RegisterDirect<MessageCard, TimeSpan>(nameof(OpenCloseMotionDuration),
            o => o.OpenCloseMotionDuration, 
            (o, v) => o.OpenCloseMotionDuration = v);
    
    private TimeSpan _openCloseMotionDuration;
    internal TimeSpan OpenCloseMotionDuration
    {
        get => _openCloseMotionDuration;
        set => SetAndRaise(OpenCloseMotionDurationProperty, ref _openCloseMotionDuration, value);
    }

    #endregion

    private bool _isClosing;
    private MotionActorControl? _motionActor;

    /// <summary>
    /// Initializes a new instance of the <see cref="MessageCard" /> class.
    /// </summary>
    public MessageCard()
    {
        UpdateMessageType();
        ClipToBounds = false;
    }

    /// <summary>
    /// Closes the <see cref="MessageCard" />.
    /// </summary>
    public void Close()
    {
        if (IsClosing)
        {
            return;
        }

        IsClosing = true;
    }

    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs e)
    {
        base.OnPropertyChanged(e);

        if (e.Property == MessageTypeProperty)
        {
            SetupMessageIcon();
            UpdateMessageType();
        }

        if (e.Property == MessageTypeProperty)
        {
            UpdateMessageType();
        }

        if (e.Property == IsClosedProperty)
        {
            if (!IsClosing && !IsClosed)
            {
                return;
            }

            RaiseEvent(new RoutedEventArgs(MessageClosedEvent));
        }

        if (e.Property == IsClosingProperty)
        {
            if (IsClosing)
            {
                ApplyHideMotion();
            }
        }
    }

    protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
    {
        base.OnApplyTemplate(e);
        if (Icon is null)
        {
            SetupMessageIcon();
            UpdateMessageType();
        }
        TokenResourceBinder.CreateTokenBinding(this, OpenCloseMotionDurationProperty, SharedTokenKey.MotionDurationMid);
        _motionActor           = e.NameScope.Find<MotionActorControl>(MessageCardTheme.MotionActorPart);
        ApplyShowMotion();
    }

    private void ApplyShowMotion()
    {
        if (_motionActor is not null)
        {
            _motionActor.IsVisible = false;
            var motion = new MoveUpInMotion(AnimationMaxOffsetY, _openCloseMotionDuration, new CubicEaseOut());
            MotionInvoker.Invoke(_motionActor, motion, () =>
            {
                _motionActor.IsVisible = true;
            });
        }
    }

    private void ApplyHideMotion()
    {
        if (_motionActor is not null)
        {
            var motion = new MoveUpOutMotion(AnimationMaxOffsetY, _openCloseMotionDuration, new CubicEaseIn());
            MotionInvoker.Invoke(_motionActor, motion, null, () =>
            {
                IsClosed = true;
            });
        }
    }
    
    private void UpdateMessageType()
    {
        switch (MessageType)
        {
            case MessageType.Error:
                PseudoClasses.Add(":error");
                break;

            case MessageType.Information:
                PseudoClasses.Add(":information");
                break;

            case MessageType.Success:
                PseudoClasses.Add(":success");
                break;

            case MessageType.Warning:
                PseudoClasses.Add(":warning");
                break;

            case MessageType.Loading:
                PseudoClasses.Add(":loading");
                break;
        }

        if (Icon is not null)
        {
            SetupMessageIconColor(Icon);
        }
    }

    private void SetupMessageIconColor(Icon Icon)
    {
        if (MessageType == MessageType.Error)
        {
            TokenResourceBinder.CreateTokenBinding(Icon, Icon.NormalFilledBrushProperty,
                SharedTokenKey.ColorError);
        }
        else if (MessageType == MessageType.Information ||
                 MessageType == MessageType.Loading)
        {
            TokenResourceBinder.CreateTokenBinding(Icon, Icon.NormalFilledBrushProperty,
                SharedTokenKey.ColorPrimary);
        }
        else if (MessageType == MessageType.Success)
        {
            TokenResourceBinder.CreateTokenBinding(Icon, Icon.NormalFilledBrushProperty,
                SharedTokenKey.ColorSuccess);
        }
        else if (MessageType == MessageType.Warning)
        {
            TokenResourceBinder.CreateTokenBinding(Icon, Icon.NormalFilledBrushProperty,
                SharedTokenKey.ColorWarning);
        }
    }

    private void SetupMessageIcon()
    {
        Icon? icon = null;
        if (MessageType == MessageType.Information)
        {
            icon = AntDesignIconPackage.InfoCircleFilled();
        }
        else if (MessageType == MessageType.Success)
        {
            icon = AntDesignIconPackage.CheckCircleFilled();
        }
        else if (MessageType == MessageType.Error)
        {
            icon = AntDesignIconPackage.CloseCircleFilled();
        }
        else if (MessageType == MessageType.Warning)
        {
            icon = AntDesignIconPackage.ExclamationCircleFilled();
        }
        else if (MessageType == MessageType.Loading)
        {
            icon                  = AntDesignIconPackage.LoadingOutlined();
            icon.LoadingAnimation = IconAnimation.Spin;
        }

        if (icon is not null)
        {
            SetupMessageIconColor(icon);
        }

        SetCurrentValue(IconProperty, icon);
    }
}