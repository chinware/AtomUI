using System.Diagnostics;
using AtomUI.IconPkg;
using AtomUI.IconPkg.AntDesign;
using AtomUI.MotionScene;
using AtomUI.Reflection;
using AtomUI.Theme;
using AtomUI.Theme.Utils;
using Avalonia;
using Avalonia.Animation.Easings;
using Avalonia.Controls;
using Avalonia.Controls.Metadata;
using Avalonia.Controls.Primitives;
using Avalonia.Data;
using Avalonia.Interactivity;
using Avalonia.VisualTree;

namespace AtomUI.Controls;

[PseudoClasses(ErrorPC, InformationPC, SuccessPC, WarningPC, LoadingPC)]
public class MessageCard : TemplatedControl,
                           IMotionAwareControl,
                           IControlSharedTokenResourcesHost
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
        AvaloniaProperty.Register<MessageCard, string>(nameof(Message));

    public static readonly StyledProperty<bool> IsMotionEnabledProperty
        = MotionAwareControlProperty.IsMotionEnabledProperty.AddOwner<MessageCard>();

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

    public bool IsMotionEnabled
    {
        get => GetValue(IsMotionEnabledProperty);
        set => SetValue(IsMotionEnabledProperty, value);
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

    Control IMotionAwareControl.PropertyBindTarget => this;
    Control IControlSharedTokenResourcesHost.HostControl => this;
    string IControlSharedTokenResourcesHost.TokenId => MessageToken.ID;

    #endregion

    private bool _isClosing;
    private MotionActorControl? _motionActor;

    /// <summary>
    /// Initializes a new instance of the <see cref="MessageCard" /> class.
    /// </summary>
    public MessageCard()
    {
        this.RegisterResources();
        this.BindMotionProperties();
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

        if (this.IsAttachedToVisualTree())
        {
            if (e.Property == MessageTypeProperty)
            {
                SetupDefaultMessageIcon();
                UpdatePseudoClasses();
            }
        }

        if (e.Property == IconProperty)
        {
            if (e.OldValue is Icon oldIcon)
            {
                oldIcon.SetTemplatedParent(null);
            }

            if (e.NewValue is Icon newIcon)
            {
                newIcon.SetTemplatedParent(this);
            }

            if (Icon is null)
            {
                SetupDefaultMessageIcon();
            }
        }

        if (e.Property == IsClosedProperty)
        {
            if (!IsClosing && !IsClosed)
            {
                return;
            }

            RaiseEvent(new RoutedEventArgs(MessageClosedEvent));
        } 
        else if (e.Property == IsClosingProperty)
        {
            if (IsClosing)
            {
                ApplyHideMotion();
            }
        }
    }

    protected override void OnAttachedToVisualTree(VisualTreeAttachmentEventArgs e)
    {
        base.OnAttachedToVisualTree(e);
        UpdatePseudoClasses();
        SetupDefaultMessageIcon();
    }

    protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
    {
        base.OnApplyTemplate(e);
        _motionActor = e.NameScope.Find<MotionActorControl>(MessageCardTheme.MotionActorPart);
        ApplyShowMotion();
    }

    private void ApplyShowMotion()
    {
        if (_motionActor is not null)
        {
            if (IsMotionEnabled)
            {
                _motionActor.IsVisible = false;
                var motion = new MoveUpInMotion(AnimationMaxOffsetY, _openCloseMotionDuration, new CubicEaseOut());
                MotionInvoker.Invoke(_motionActor, motion, () => { _motionActor.IsVisible = true; });
            }
            else
            {
                _motionActor.IsVisible = true;
            }
        }
    }

    private void ApplyHideMotion()
    {
        if (_motionActor is not null)
        {
            if (IsMotionEnabled)
            {
                var motion =
                    new MoveUpOutMotion(AnimationMaxOffsetY, _openCloseMotionDuration, new CubicEaseIn());
                MotionInvoker.Invoke(_motionActor, motion, null, () => { IsClosed = true; });
            }
            else
            {
                IsClosed = true;
            }
        }
    }

    private void UpdatePseudoClasses()
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
    }

    private void SetupDefaultMessageIcon()
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

        if (Icon is null)
        {
            ClearValue(IconProperty);
            SetValue(IconProperty, icon, BindingPriority.Template);
        }
        Debug.Assert(Icon != null);
        Icon.SetTemplatedParent(this);
    }
    
}