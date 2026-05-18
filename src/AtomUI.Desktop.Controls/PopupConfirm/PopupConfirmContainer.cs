using AtomUI.Icons.AntDesign;
using AtomUI.Reflection;
using AtomUI.Theme;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Presenters;
using Avalonia.Controls.Primitives;
using Avalonia.Controls.Templates;
using Avalonia.Data;
using Avalonia.Interactivity;
using Avalonia.Metadata;
using Avalonia.VisualTree;

namespace AtomUI.Desktop.Controls;

internal class PopupConfirmContainer : TemplatedControl
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

    public static readonly StyledProperty<PathIcon?> IconProperty =
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

    public PathIcon? Icon
    {
        get => GetValue(IconProperty);
        set => SetValue(IconProperty, value);
    }

    public PopupConfirmStatus ConfirmStatus
    {
        get => GetValue(ConfirmStatusProperty);
        set => SetValue(ConfirmStatusProperty, value);
    }

    #endregion
    
    internal WeakReference<PopupConfirm> PopupConfirmRef { get; set; }
    private StackPanel? _buttonLayout;
    private StackPanel? _contentLayout;
    private Button? _okButton;
    private Button? _cancelButton;
    private ContentPresenter? _contentPresenter;

    public PopupConfirmContainer(PopupConfirm popupConfirm)
    {
        this.RegisterTokenResourceScope(PopupConfirmToken.ScopeProvider);
        PopupConfirmRef = new WeakReference<PopupConfirm>(popupConfirm);
    }

    protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
    {
        ReleaseTemplateParts();
        base.OnApplyTemplate(e);
        SetupDefaultIcon();
        _buttonLayout = e.NameScope.Find<StackPanel>("PART_ButtonLayout");
        _contentLayout = e.NameScope.Find<StackPanel>("PART_ContentLayout");
        _okButton     = e.NameScope.Find<Button>("PART_OkButton");
        if (_okButton is not null)
        {
            _okButton.Click  += HandleButtonClicked;
            _okButton.Width  =  double.NaN;
            _okButton.Height =  double.NaN;
        }

        ConfigureCancelButton();
        ConfigureContentPresenter();
        UpdatePseudoClasses();
    }

    private void ReleaseTemplateParts()
    {
        if (_okButton is not null)
        {
            _okButton.Click -= HandleButtonClicked;
            _okButton = null;
        }

        ReleaseCancelButton();
        ReleaseContentPresenter();
        _buttonLayout  = null;
        _contentLayout = null;
    }

    private void ReleaseCancelButton()
    {
        if (_cancelButton is not null)
        {
            _cancelButton.Click -= HandleButtonClicked;
            if (_cancelButton.GetVisualParent() is Panel parent)
            {
                parent.Children.Remove(_cancelButton);
            }
            else
            {
                _buttonLayout?.Children.Remove(_cancelButton);
            }
            _cancelButton.ClearValue(ContentControl.ContentProperty);
            _cancelButton.SetTemplatedParent(null);
            _cancelButton = null;
        }
    }

    private void ReleaseContentPresenter()
    {
        if (_contentPresenter is not null)
        {
            if (_contentPresenter.GetVisualParent() is Panel parent)
            {
                parent.Children.Remove(_contentPresenter);
            }
            else
            {
                _contentLayout?.Children.Remove(_contentPresenter);
            }
            _contentPresenter.ClearValue(ContentPresenter.ContentProperty);
            _contentPresenter.ClearValue(ContentPresenter.ContentTemplateProperty);
            _contentPresenter.SetTemplatedParent(null);
            _contentPresenter = null;
        }
    }

    private void ConfigureCancelButton()
    {
        if (!IsShowCancelButton)
        {
            ReleaseCancelButton();
            return;
        }

        EnsureCancelButton();
    }

    private void EnsureCancelButton()
    {
        if (_buttonLayout is null || _cancelButton is not null)
        {
            return;
        }

        _cancelButton = new Button
        {
            Name     = "PART_CancelButton",
            SizeType = SizeType.Small,
            Margin   = new Thickness(0),
            Width    = double.NaN,
            Height   = double.NaN
        };
        _cancelButton.SetTemplatedParent(this);
        _cancelButton[!ContentControl.ContentProperty] = this[!CancelTextProperty];
        _cancelButton.Click += HandleButtonClicked;

        var insertIndex = _okButton is null ? _buttonLayout.Children.Count : _buttonLayout.Children.IndexOf(_okButton);
        if (insertIndex < 0)
        {
            insertIndex = 0;
        }
        _buttonLayout.Children.Insert(insertIndex, _cancelButton);
    }

    private void ConfigureContentPresenter()
    {
        if (ConfirmContent is null)
        {
            ReleaseContentPresenter();
            return;
        }

        EnsureContentPresenter();
    }

    private void EnsureContentPresenter()
    {
        if (_contentLayout is null || _contentPresenter is not null)
        {
            return;
        }

        _contentPresenter = new ContentPresenter
        {
            Name = "PART_Content"
        };
        _contentPresenter.SetTemplatedParent(this);
        _contentPresenter[!ContentPresenter.ContentProperty] = this[!ConfirmContentProperty];
        _contentPresenter[!ContentPresenter.ContentTemplateProperty] = this[!ConfirmContentTemplateProperty];
        _contentLayout.Children.Add(_contentPresenter);
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
            ConfigureContentPresenter();
            UpdatePseudoClasses();
        }
        else if (change.Property == IsShowCancelButtonProperty)
        {
            ConfigureCancelButton();
        }
    }

    private void SetupDefaultIcon()
    {
        if (Icon == null)
        {
            ClearValue(IconProperty);
            SetValue(IconProperty, new ExclamationCircleFilled(), BindingPriority.Template);
        }
    }
    
    private void UpdatePseudoClasses()
    {
        PseudoClasses.Set(PopupConfirmPseudoClass.EmptyContent, ConfirmContent == null);
    }
}
