using AtomUI.Controls;
using AtomUI.Desktop.Controls.Themes;
using AtomUI.Icons.AntDesign;
using AtomUI.Theme;
using AtomUI.Utils;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Controls.Templates;
using Avalonia.Data;
using Avalonia.Interactivity;
using Avalonia.Metadata;
using Avalonia.VisualTree;

namespace AtomUI.Desktop.Controls;

internal class PopupConfirmContainer : TemplatedControl,
                                       IControlSharedTokenResourcesHost
{
    #region 内部属性定义

    public static readonly StyledProperty<string> OkTextProperty =
        PopupConfirm.OkTextProperty.AddOwner<PopupConfirmContainer>();

    public static readonly StyledProperty<string> CancelTextProperty =
        PopupConfirm.CancelTextProperty.AddOwner<PopupConfirmContainer>();

    public static readonly StyledProperty<ButtonType> OkButtonTypeProperty =
        PopupConfirm.OkButtonTypeProperty.AddOwner<PopupConfirmContainer>();

    public static readonly StyledProperty<bool> IsShowCancelButtonProperty =
        PopupConfirm.IsShowCancelButtonProperty.AddOwner<PopupConfirmContainer>();

    public static readonly StyledProperty<string> TitleProperty =
        PopupConfirm.TitleProperty.AddOwner<PopupConfirmContainer>();
    
    public static readonly StyledProperty<object?> ConfirmContentProperty =
        PopupConfirm.ConfirmContentProperty.AddOwner<PopupConfirmContainer>();

    public static readonly StyledProperty<IDataTemplate?> ConfirmContentTemplateProperty =
        PopupConfirm.ConfirmContentTemplateProperty.AddOwner<PopupConfirmContainer>();

    public static readonly StyledProperty<Icon?> IconProperty =
        PopupConfirm.IconProperty.AddOwner<PopupConfirmContainer>();

    public static readonly StyledProperty<PopupConfirmStatus> ConfirmStatusProperty =
        PopupConfirm.ConfirmStatusProperty.AddOwner<PopupConfirmContainer>();

    public string OkText
    {
        get => GetValue(OkTextProperty);
        set => SetValue(OkTextProperty, value);
    }

    public string CancelText
    {
        get => GetValue(CancelTextProperty);
        set => SetValue(CancelTextProperty, value);
    }

    public ButtonType OkButtonType
    {
        get => GetValue(OkButtonTypeProperty);
        set => SetValue(OkButtonTypeProperty, value);
    }

    public bool IsShowCancelButton
    {
        get => GetValue(IsShowCancelButtonProperty);
        set => SetValue(IsShowCancelButtonProperty, value);
    }

    public string Title
    {
        get => GetValue(TitleProperty);
        set => SetValue(TitleProperty, value);
    }

    [DependsOn(nameof(ConfirmContentTemplate))]
    public object? ConfirmContent
    {
        get => GetValue(ConfirmContentProperty);
        set => SetValue(ConfirmContentProperty, value);
    }

    public IDataTemplate? ConfirmContentTemplate
    {
        get => GetValue(ConfirmContentTemplateProperty);
        set => SetValue(ConfirmContentTemplateProperty, value);
    }

    public Icon? Icon
    {
        get => GetValue(IconProperty);
        set => SetValue(IconProperty, value);
    }

    public PopupConfirmStatus ConfirmStatus
    {
        get => GetValue(ConfirmStatusProperty);
        set => SetValue(ConfirmStatusProperty, value);
    }
    
    Control IControlSharedTokenResourcesHost.HostControl => this;
    string IControlSharedTokenResourcesHost.TokenId => PopupConfirmToken.ID;

    #endregion
    
    internal WeakReference<PopupConfirm> PopupConfirmRef { get; set; }
    private Button? _okButton;
    private Button? _cancelButton;

    public PopupConfirmContainer(PopupConfirm popupConfirm)
    {
        this.RegisterResources();
        PopupConfirmRef = new WeakReference<PopupConfirm>(popupConfirm);
    }

    protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
    {
        base.OnApplyTemplate(e);
        SetupDefaultIcon();
        _okButton     = e.NameScope.Find<Button>(PopupConfirmContainerThemeConstants.OkButtonPart);
        _cancelButton = e.NameScope.Find<Button>(PopupConfirmContainerThemeConstants.CancelButtonPart);
        if (_okButton is not null)
        {
            _okButton.Click  += HandleButtonClicked;
            _okButton.Width  =  double.NaN;
            _okButton.Height =  double.NaN;
        }

        if (_cancelButton is not null)
        {
            _cancelButton.Click  += HandleButtonClicked;
            _cancelButton.Width  =  double.NaN;
            _cancelButton.Height =  double.NaN;
        }
        UpdatePseudoClasses();
    }

    private void HandleButtonClicked(object? sender, RoutedEventArgs args)
    {
        if (PopupConfirmRef.TryGetTarget(out var popupConfirm))
        {
            var isConfirmed = false;
            if (sender == _okButton)
            {
                isConfirmed = true;
                var eventArgs = new RoutedEventArgs(PopupConfirm.ConfirmedEvent);
                popupConfirm.RaiseEvent(eventArgs);
            }
            else
            {
                var eventArgs = new RoutedEventArgs(PopupConfirm.CancelledEvent);
                popupConfirm.RaiseEvent(eventArgs);
            }

            var popupEventArgs = new PopupConfirmClickEventArgs(PopupConfirm.PopupClickEvent, isConfirmed);
            popupConfirm.RaiseEvent(popupEventArgs);
            popupConfirm.HideFlyout(true);
        }
    }
    
    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        base.OnPropertyChanged(change);
        if (this.IsAttachedToVisualTree())
        {
            if (change.Property == IconProperty)
            {
                SetupDefaultIcon();
            }
        }

        if (change.Property == ConfirmContentProperty ||
            change.Property == ConfirmContentTemplateProperty)
        {
            UpdatePseudoClasses();
        }
    }

    private void SetupDefaultIcon()
    {
        if (Icon == null)
        {
            ClearValue(IconProperty);
            SetValue(IconProperty, AntDesignIconPackage.ExclamationCircleFilled(), BindingPriority.Template);
        }
    }
    
    private void UpdatePseudoClasses()
    {
        PseudoClasses.Set(PopupConfirmPseudoClass.EmptyContent, ConfirmContent == null);
    }
}