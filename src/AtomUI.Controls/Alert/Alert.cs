using System.Reactive.Disposables;
using AtomUI.IconPkg;
using AtomUI.IconPkg.AntDesign;
using AtomUI.Theme;
using AtomUI.Theme.Data;
using AtomUI.Theme.Styling;
using AtomUI.Theme.Utils;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Metadata;
using Avalonia.Controls.Primitives;
using Avalonia.Data;
using Avalonia.LogicalTree;
using Avalonia.Metadata;
using Avalonia.VisualTree;

namespace AtomUI.Controls;

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
public class Alert : TemplatedControl,
                     IControlSharedTokenResourcesHost,
                     IResourceBindingManager
{
    #region 公共属性定义

    public static readonly StyledProperty<AlertType> TypeProperty =
        AvaloniaProperty.Register<Alert, AlertType>(nameof(Type));

    public static readonly StyledProperty<bool> IsShowIconProperty =
        AvaloniaProperty.Register<Alert, bool>(nameof(IsShowIcon));

    public static readonly StyledProperty<bool> IsMessageMarqueEnabledProperty =
        AvaloniaProperty.Register<Alert, bool>(nameof(IsMessageMarqueEnabled));

    public static readonly StyledProperty<bool> IsClosableProperty =
        AvaloniaProperty.Register<Alert, bool>(nameof(IsClosable));

    public static readonly StyledProperty<Icon?> CloseIconProperty =
        AvaloniaProperty.Register<Alert, Icon?>(nameof(CloseIcon));

    public static readonly StyledProperty<string> MessageProperty =
        AvaloniaProperty.Register<Alert, string>(nameof(Message));

    public static readonly StyledProperty<string?> DescriptionProperty =
        AvaloniaProperty.Register<Alert, string?>(nameof(Description));

    public static readonly StyledProperty<Control?> ExtraActionProperty =
        AvaloniaProperty.Register<Alert, Control?>(nameof(Description));

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

    public bool IsMessageMarqueEnabled
    {
        get => GetValue(IsMessageMarqueEnabledProperty);
        set => SetValue(IsMessageMarqueEnabledProperty, value);
    }

    public bool IsClosable
    {
        get => GetValue(IsClosableProperty);
        set => SetValue(IsClosableProperty, value);
    }

    public Icon? CloseIcon
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

    #region 内部属性定义

    Control IControlSharedTokenResourcesHost.HostControl => this;
    string IControlSharedTokenResourcesHost.TokenId => AlertToken.ID;
    CompositeDisposable? IResourceBindingManager.ResourceBindingsDisposable { get; set; }

    #endregion

    static Alert()
    {
        AffectsMeasure<Segmented>(IsClosableProperty,
            IsShowIconProperty,
            MessageProperty,
            DescriptionProperty,
            IsMessageMarqueEnabledProperty,
            PaddingProperty,
            ExtraActionProperty,
            IsMessageMarqueEnabledProperty);
        AffectsRender<Segmented>(TypeProperty);
    }

    public Alert()
    {
        this.RegisterResources();
    }

    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs e)
    {
        base.OnPropertyChanged(e);
        if (this.IsAttachedToVisualTree())
        {
            if (e.Property == IsClosableProperty)
            {
                SetupCloseButton();
            }
        }
        
        if (e.Property == DescriptionProperty)
        {
            UpdatePseudoClasses();
        }
    }

    protected override void OnAttachedToLogicalTree(LogicalTreeAttachmentEventArgs e)
    {
        base.OnAttachedToLogicalTree(e);
        this.AddResourceBindingDisposable(TokenResourceBinder.CreateTokenBinding(this, BorderThicknessProperty,
            SharedTokenKey.BorderThickness,
            BindingPriority.Template,
            new RenderScaleAwareThicknessConfigure(this)));
    }

    protected override void OnDetachedFromLogicalTree(LogicalTreeAttachmentEventArgs e)
    {
        base.OnDetachedFromLogicalTree(e);
        this.DisposeTokenBindings();
    }

    protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
    {
        base.OnApplyTemplate(e);
        UpdatePseudoClasses();
        SetupCloseButton();
    }

    private void SetupCloseButton()
    {
        if (CloseIcon == null)
        {
            ClearValue(CloseIconProperty);
            SetValue(CloseIconProperty, AntDesignIconPackage.CloseOutlined(), BindingPriority.Template);
        }
    }
    
    private void UpdatePseudoClasses()
    {
        PseudoClasses.Set(AlertPseudoClass.HasDescription, !string.IsNullOrEmpty(Description));
        PseudoClasses.Set(AlertPseudoClass.HasExtraAction, ExtraAction != null);
    }
}