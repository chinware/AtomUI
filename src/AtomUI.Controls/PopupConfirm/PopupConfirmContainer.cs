using System.Diagnostics;
using AtomUI.Controls.Utils;
using AtomUI.IconPkg;
using AtomUI.IconPkg.AntDesign;
using AtomUI.Theme;
using AtomUI.Theme.Utils;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Controls.Templates;
using Avalonia.Data;
using Avalonia.Interactivity;
using Avalonia.LogicalTree;
using Avalonia.VisualTree;

namespace AtomUI.Controls;

internal class PopupConfirmContainer : TemplatedControl,
                                       IControlSharedTokenResourcesHost
{
    #region 内部属性定义

    internal static readonly StyledProperty<string> OkTextProperty =
        PopupConfirm.OkTextProperty.AddOwner<PopupConfirmContainer>();

    internal static readonly StyledProperty<string> CancelTextProperty =
        PopupConfirm.CancelTextProperty.AddOwner<PopupConfirmContainer>();

    internal static readonly StyledProperty<ButtonType> OkButtonTypeProperty =
        PopupConfirm.OkButtonTypeProperty.AddOwner<PopupConfirmContainer>();

    internal static readonly StyledProperty<bool> IsShowCancelButtonProperty =
        PopupConfirm.IsShowCancelButtonProperty.AddOwner<PopupConfirmContainer>();

    internal static readonly StyledProperty<string> TitleProperty =
        PopupConfirm.TitleProperty.AddOwner<PopupConfirmContainer>();

    internal static readonly StyledProperty<object?> ConfirmContentProperty =
        PopupConfirm.ConfirmContentProperty.AddOwner<PopupConfirmContainer>();

    internal static readonly StyledProperty<IDataTemplate?> ConfirmContentTemplateProperty =
        PopupConfirm.ConfirmContentTemplateProperty.AddOwner<PopupConfirmContainer>();

    internal static readonly StyledProperty<Icon?> IconProperty =
        PopupConfirm.IconProperty.AddOwner<PopupConfirmContainer>();

    internal static readonly StyledProperty<PopupConfirmStatus> ConfirmStatusProperty
        = PopupConfirm.ConfirmStatusProperty.AddOwner<PopupConfirmContainer>();

    internal string OkText
    {
        get => GetValue(OkTextProperty);
        set => SetValue(OkTextProperty, value);
    }

    internal string CancelText
    {
        get => GetValue(CancelTextProperty);
        set => SetValue(CancelTextProperty, value);
    }

    internal ButtonType OkButtonType
    {
        get => GetValue(OkButtonTypeProperty);
        set => SetValue(OkButtonTypeProperty, value);
    }

    internal bool IsShowCancelButton
    {
        get => GetValue(IsShowCancelButtonProperty);
        set => SetValue(IsShowCancelButtonProperty, value);
    }

    internal string Title
    {
        get => GetValue(TitleProperty);
        set => SetValue(TitleProperty, value);
    }

    internal object? ConfirmContent
    {
        get => GetValue(ConfirmContentProperty);
        set => SetValue(ConfirmContentProperty, value);
    }

    internal IDataTemplate? ConfirmContentTemplate
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

        _okButton     = e.NameScope.Find<Button>(PopupConfirmContainerTheme.OkButtonPart);
        _cancelButton = e.NameScope.Find<Button>(PopupConfirmContainerTheme.CancelButtonPart);
        if (_okButton is not null)
        {
            _okButton.Click += HandleButtonClicked;
        }

        if (_cancelButton is not null)
        {
            _cancelButton.Click += HandleButtonClicked;
        }
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
                if (change.OldValue is Icon oldIcon)
                {
                    oldIcon.SetTemplatedParent(null);
                }
                if (change.NewValue is Icon newIcon)
                {
                    newIcon.SetTemplatedParent(this);
                }
                SetupDefaultIcon();
            }
        }
    }

    private void SetupDefaultIcon()
    {
        if (Icon == null)
        {
            ClearValue(IconProperty);
            SetValue(IconProperty, AntDesignIconPackage.ExclamationCircleFilled(), BindingPriority.Template);
        }
        Debug.Assert(Icon != null);
        Icon.SetTemplatedParent(this);
    }
    
    protected override void OnAttachedToLogicalTree(LogicalTreeAttachmentEventArgs e)
    {
        base.OnAttachedToLogicalTree(e);
        SetupDefaultIcon();
    }
}