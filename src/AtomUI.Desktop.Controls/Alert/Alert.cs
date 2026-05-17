using AtomUI.Data;
using AtomUI.Icons.AntDesign;
using AtomUI.Reflection;
using AtomUI.Theme;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Metadata;
using Avalonia.Controls.Primitives;
using Avalonia.Data;
using Avalonia.Interactivity;
using Avalonia.Layout;
using Avalonia.Metadata;
using Avalonia.VisualTree;

namespace AtomUI.Desktop.Controls;

public enum AlertType
{
    Success,
    Info,
    Warning,
    Error
}

public static class AlertPseudoClass
{
    public const string HasDescription = ":has-description";
    public const string HasExtraAction = ":has-extra-action";
}

[PseudoClasses(AlertPseudoClass.HasDescription, AlertPseudoClass.HasExtraAction)]
public class Alert : TemplatedControl
{
    #region 公共属性定义

    public static readonly StyledProperty<AlertType> TypeProperty =
        AvaloniaProperty.Register<Alert, AlertType>(nameof(Type));

    public static readonly StyledProperty<bool> IsShowIconProperty =
        AvaloniaProperty.Register<Alert, bool>(nameof(IsShowIcon));

    public static readonly StyledProperty<bool> IsMessageMarqueeEnabledProperty =
        AvaloniaProperty.Register<Alert, bool>(nameof(IsMessageMarqueeEnabled));

    public static readonly StyledProperty<bool> IsClosableProperty =
        AvaloniaProperty.Register<Alert, bool>(nameof(IsClosable));

    public static readonly StyledProperty<PathIcon?> CloseIconProperty =
        AvaloniaProperty.Register<Alert, PathIcon?>(nameof(CloseIcon));

    public static readonly StyledProperty<string> MessageProperty =
        AvaloniaProperty.Register<Alert, string>(nameof(Message));

    public static readonly StyledProperty<string?> DescriptionProperty =
        AvaloniaProperty.Register<Alert, string?>(nameof(Description));

    public static readonly StyledProperty<Control?> ExtraActionProperty =
        AvaloniaProperty.Register<Alert, Control?>(nameof(ExtraAction));

    public AlertType Type
    {
        get => GetValue(TypeProperty);
        set => SetValue(TypeProperty, value);
    }

    public bool IsShowIcon
    {
        get => GetValue(IsShowIconProperty);
        set => SetValue(IsShowIconProperty, value);
    }

    public bool IsMessageMarqueeEnabled
    {
        get => GetValue(IsMessageMarqueeEnabledProperty);
        set => SetValue(IsMessageMarqueeEnabledProperty, value);
    }

    public bool IsClosable
    {
        get => GetValue(IsClosableProperty);
        set => SetValue(IsClosableProperty, value);
    }

    public PathIcon? CloseIcon
    {
        get => GetValue(CloseIconProperty);
        set => SetValue(CloseIconProperty, value);
    }

    [Content]
    public string Message
    {
        get => GetValue(MessageProperty);
        set => SetValue(MessageProperty, value);
    }

    public string? Description
    {
        get => GetValue(DescriptionProperty);
        set => SetValue(DescriptionProperty, value);
    }

    public Control? ExtraAction
    {
        get => GetValue(ExtraActionProperty);
        set => SetValue(ExtraActionProperty, value);
    }

    #endregion

    #region 公共事件定义

    public event EventHandler? CloseRequest;

    #endregion
    
    private IconButton? _closeButton;
    private StackPanel? _messageLayout;
    private Label? _messageLabel;
    private MarqueeLabel? _marqueeLabel;
    private IDisposable? _marqueeLabelTextBinding;

    static Alert()
    {
        AffectsMeasure<Alert>(IsClosableProperty,
            IsShowIconProperty,
            MessageProperty,
            DescriptionProperty,
            IsMessageMarqueeEnabledProperty,
            PaddingProperty,
            ExtraActionProperty);
        AffectsRender<Alert>(TypeProperty);
    }

    public Alert()
    {
        this.RegisterTokenResourceScope(AlertToken.ScopeProvider);
    }

    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        base.OnPropertyChanged(change);
        if (this.IsAttachedToVisualTree())
        {
            if (change.Property == IsClosableProperty)
            {
                SetupCloseButton();
            }
        }
        
        if (change.Property == DescriptionProperty)
        {
            UpdatePseudoClasses();
        }
        else if (change.Property == IsMessageMarqueeEnabledProperty)
        {
            ConfigureMarqueeLabel();
        }
    }

    protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
    {
        base.OnApplyTemplate(e);
        RemoveMarqueeLabel();
        if (_closeButton != null)
        {
            _closeButton.Click -= HandleCloseBtnClick;
        }
        _closeButton = e.NameScope.Find<IconButton>("PART_CloseBtn");
        _messageLayout = e.NameScope.Find<StackPanel>("PART_MessageLayout");
        _messageLabel  = e.NameScope.Find<Label>("MessageLabel");
        _messageLayout ??= _messageLabel?.GetVisualParent() as StackPanel;
        if (_closeButton != null)
        {
            _closeButton.Click += HandleCloseBtnClick;
        }
        UpdatePseudoClasses();
        SetupCloseButton();
        if (IsMessageMarqueeEnabled)
        {
            ConfigureMarqueeLabel();
        }
    }

    protected override void OnAttachedToVisualTree(VisualTreeAttachmentEventArgs e)
    {
        base.OnAttachedToVisualTree(e);
        if (IsMessageMarqueeEnabled)
        {
            ConfigureMarqueeLabel();
        }
    }

    protected override void OnDetachedFromVisualTree(VisualTreeAttachmentEventArgs e)
    {
        RemoveMarqueeLabel();
        base.OnDetachedFromVisualTree(e);
    }

    private void HandleCloseBtnClick(object? sender, RoutedEventArgs e)
    {
        CloseRequest?.Invoke(this, EventArgs.Empty);
    }

    private void SetupCloseButton()
    {
        if (CloseIcon == null)
        {
            ClearValue(CloseIconProperty);
            SetValue(CloseIconProperty, new CloseOutlined(), BindingPriority.Template);
        }
    }

    private void ConfigureMarqueeLabel()
    {
        if (!IsMessageMarqueeEnabled)
        {
            RemoveMarqueeLabel();
            return;
        }

        if (_messageLayout is null || _marqueeLabel is not null)
        {
            return;
        }

        _marqueeLabel = new MarqueeLabel
        {
            Name                = "MarqueeLabel",
            HorizontalAlignment = HorizontalAlignment.Stretch,
            Padding             = new Thickness(0)
        };
        _marqueeLabel.SetTemplatedParent(this);
        _marqueeLabelTextBinding = BindUtils.RelayBind(this,
            MessageProperty,
            _marqueeLabel,
            TextBlock.TextProperty,
            BindingMode.Default,
            BindingPriority.Template);

        var insertIndex = 0;
        if (_messageLabel is not null)
        {
            var messageLabelIndex = _messageLayout.Children.IndexOf(_messageLabel);
            if (messageLabelIndex >= 0)
            {
                insertIndex = messageLabelIndex + 1;
            }
        }
        if (insertIndex > _messageLayout.Children.Count)
        {
            insertIndex = _messageLayout.Children.Count;
        }
        _messageLayout.Children.Insert(insertIndex, _marqueeLabel);
    }

    private void RemoveMarqueeLabel()
    {
        if (_marqueeLabel is null)
        {
            return;
        }

        _marqueeLabelTextBinding?.Dispose();
        _marqueeLabelTextBinding = null;
        _marqueeLabel.ClearValue(TextBlock.TextProperty);
        if (_messageLayout?.Children.Contains(_marqueeLabel) == true)
        {
            _messageLayout.Children.Remove(_marqueeLabel);
        }
        else if (_marqueeLabel.GetVisualParent() is Panel parentPanel)
        {
            parentPanel.Children.Remove(_marqueeLabel);
        }
        _marqueeLabel.SetTemplatedParent(null);
        _marqueeLabel = null;
    }
    
    private void UpdatePseudoClasses()
    {
        PseudoClasses.Set(AlertPseudoClass.HasDescription, !string.IsNullOrEmpty(Description));
        PseudoClasses.Set(AlertPseudoClass.HasExtraAction, ExtraAction != null);
    }
}
