using Avalonia;
using Avalonia.Interactivity;
using Avalonia.LogicalTree;
using Avalonia.VisualTree;

namespace AtomUI.Desktop.Controls;

public class SubmitButton : Button
{
    #region 公共属性定义

    public static readonly StyledProperty<bool> IsWatchValidateResultProperty =
        AvaloniaProperty.Register<SubmitButton, bool>(nameof(IsWatchValidateResult), false);
    
    public bool IsWatchValidateResult
    {
        get => GetValue(IsWatchValidateResultProperty);
        set => SetValue(IsWatchValidateResultProperty, value);
    }
    #endregion
    
    #region 公共事件定义

    public static readonly RoutedEvent<RoutedEventArgs> SubmitEvent =
        RoutedEvent.Register<SubmitButton, RoutedEventArgs>(nameof(Submit), RoutingStrategies.Bubble);

    public event EventHandler<RoutedEventArgs>? Submit
    {
        add => AddHandler(SubmitEvent, value);
        remove => RemoveHandler(SubmitEvent, value);
    }
    #endregion

    private Form? _ownerForm;
    private IDisposable? _formValidSubscription;

    protected override void OnClick()
    {
        base.OnClick();
        RaiseEvent(new RoutedEventArgs(SubmitEvent));
    }

    protected override void OnAttachedToLogicalTree(LogicalTreeAttachmentEventArgs e)
    {
        base.OnAttachedToLogicalTree(e);
        ConfigureOwnerForm();
    }

    protected override void OnAttachedToVisualTree(VisualTreeAttachmentEventArgs e)
    {
        base.OnAttachedToVisualTree(e);
        ConfigureOwnerForm();
    }

    protected override void OnDetachedFromLogicalTree(LogicalTreeAttachmentEventArgs e)
    {
        ReleaseOwnerForm();
        base.OnDetachedFromLogicalTree(e);
    }

    protected override void OnDetachedFromVisualTree(VisualTreeAttachmentEventArgs e)
    {
        ReleaseOwnerForm();
        base.OnDetachedFromVisualTree(e);
    }

    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        base.OnPropertyChanged(change);
        if (change.Property == IsWatchValidateResultProperty)
        {
            ConfigureOwnerForm();
        }
    }

    private void ConfigureOwnerForm()
    {
        if (!IsWatchValidateResult)
        {
            ReleaseOwnerForm();
            return;
        }

        var ownerForm = this.FindLogicalAncestorOfType<Form>() ?? this.FindAncestorOfType<Form>();
        if (ReferenceEquals(_ownerForm, ownerForm))
        {
            ApplyFormValidState(ownerForm?.IsFormValid);
            return;
        }

        ReleaseOwnerForm();
        _ownerForm = ownerForm;
        if (_ownerForm != null)
        {
            _formValidSubscription = _ownerForm.GetObservable(Form.IsFormValidProperty).Subscribe(ApplyFormValidState);
            ApplyFormValidState(_ownerForm.IsFormValid);
        }
    }

    private void ReleaseOwnerForm()
    {
        _formValidSubscription?.Dispose();
        _formValidSubscription = null;
        _ownerForm             = null;
    }

    private void ApplyFormValidState(bool? isFormValid)
    {
        if (IsWatchValidateResult)
        {
            SetCurrentValue(IsEnabledProperty, isFormValid == true);
        }
    }
}
