using AtomUI.Controls;
using AtomUI.Desktop.Controls.Primitives.Themes;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.LogicalTree;

namespace AtomUI.Desktop.Controls;

internal class PickerClearUpButton : TemplatedControl
{
    public event EventHandler? ClearRequest;

    public static readonly StyledProperty<bool> IsInClearModeProperty =
        AvaloniaProperty.Register<PickerClearUpButton, bool>(nameof(IsInClearMode));

    public static readonly StyledProperty<PathIcon?> IconProperty =
        AvaloniaProperty.Register<PickerClearUpButton, PathIcon?>(nameof(Icon));

    public static readonly StyledProperty<FormValidateFeedback?> FormFeedbackProperty =
        AvaloniaProperty.Register<PickerClearUpButton, FormValidateFeedback?>(nameof(FormFeedback));

    public static readonly DirectProperty<PickerClearUpButton, bool> IsFormFeedbackVisibleProperty =
        AvaloniaProperty.RegisterDirect<PickerClearUpButton, bool>(
            nameof(IsFormFeedbackVisible),
            o => o.IsFormFeedbackVisible);

    public bool IsInClearMode
    {
        get => GetValue(IsInClearModeProperty);
        set => SetValue(IsInClearModeProperty, value);
    }

    public PathIcon? Icon
    {
        get => GetValue(IconProperty);
        set => SetValue(IconProperty, value);
    }

    public FormValidateFeedback? FormFeedback
    {
        get => GetValue(FormFeedbackProperty);
        set => SetValue(FormFeedbackProperty, value);
    }

    private bool _isFormFeedbackVisible;

    public bool IsFormFeedbackVisible
    {
        get => _isFormFeedbackVisible;
        private set => SetAndRaise(IsFormFeedbackVisibleProperty, ref _isFormFeedbackVisible, value);
    }

    private IconButton? _clearButton;
    private IDisposable? _feedbackStatusSubscription;

    protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
    {
        base.OnApplyTemplate(e);
        _clearButton = e.NameScope.Get<IconButton>(PickerClearUpButtonThemeConstants.ClearButtonPart);
        if (_clearButton is not null)
        {
            _clearButton.Click += (sender, args) => { ClearRequest?.Invoke(this, EventArgs.Empty); };
        }
    }

    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        base.OnPropertyChanged(change);
        if (change.Property == FormFeedbackProperty)
        {
            ConfigureFormFeedbackSubscription();
        }
    }

    private void ConfigureFormFeedbackSubscription()
    {
        _feedbackStatusSubscription?.Dispose();
        _feedbackStatusSubscription = null;
        if (FormFeedback is { } feedback)
        {
            _feedbackStatusSubscription = feedback.GetObservable(FormValidateFeedback.ValidateStatusProperty)
                                                  .Subscribe(status => IsFormFeedbackVisible = status != FormValidateStatus.Default);
        }
        else
        {
            IsFormFeedbackVisible = false;
        }
    }

    protected override void OnDetachedFromLogicalTree(LogicalTreeAttachmentEventArgs e)
    {
        base.OnDetachedFromLogicalTree(e);
        _feedbackStatusSubscription?.Dispose();
        _feedbackStatusSubscription = null;
    }
}
