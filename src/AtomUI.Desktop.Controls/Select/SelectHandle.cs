using AtomUI.Controls;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Interactivity;
using Avalonia.LogicalTree;

namespace AtomUI.Desktop.Controls;

internal class SelectHandle : TemplatedControl
{
    public static readonly StyledProperty<bool> IsInputHoverProperty =
        AvaloniaProperty.Register<SelectHandle, bool>(nameof(IsInputHover));

    public static readonly StyledProperty<bool> IsInputPressedProperty =
        AvaloniaProperty.Register<SelectHandle, bool>(nameof(IsInputPressed));

    public static readonly StyledProperty<bool> IsFilterEnabledProperty =
        AvaloniaProperty.Register<SelectHandle, bool>(nameof(IsFilterEnabled));

    public static readonly StyledProperty<bool> IsAllowClearProperty =
        AvaloniaProperty.Register<SelectHandle, bool>(nameof(IsAllowClear));

    public static readonly StyledProperty<bool> IsMotionEnabledProperty =
        MotionAwareControlProperty.IsMotionEnabledProperty.AddOwner<SelectHandle>();

    public static readonly StyledProperty<bool> IsLoadingProperty =
        AvaloniaProperty.Register<SelectHandle, bool>(nameof(IsLoading));

    public static readonly StyledProperty<bool> IsSelectionEmptyProperty =
        AvaloniaProperty.Register<SelectHandle, bool>(nameof(IsSelectionEmpty), true);

    public static readonly StyledProperty<bool> IsDropDownOpenProperty =
        AvaloniaProperty.Register<SelectHandle, bool>(nameof(IsDropDownOpen), false);

    public static readonly StyledProperty<PathIcon?> OpenIndicatorProperty =
        AvaloniaProperty.Register<SelectHandle, PathIcon?>(nameof (OpenIndicator));

    public static readonly StyledProperty<PathIcon?> LoadingIconProperty =
        AvaloniaProperty.Register<SelectHandle, PathIcon?>(nameof (LoadingIcon));

    public static readonly StyledProperty<FormValidateFeedback?> FormFeedbackProperty =
        AvaloniaProperty.Register<SelectHandle, FormValidateFeedback?>(nameof (FormFeedback));

    public static readonly DirectProperty<SelectHandle, bool> IsFormFeedbackVisibleProperty =
        AvaloniaProperty.RegisterDirect<SelectHandle, bool>(
            nameof(IsFormFeedbackVisible),
            o => o.IsFormFeedbackVisible);

    public bool IsInputHover
    {
        get => GetValue(IsInputHoverProperty);
        set => SetValue(IsInputHoverProperty, value);
    }

    public bool IsInputPressed
    {
        get => GetValue(IsInputPressedProperty);
        set => SetValue(IsInputPressedProperty, value);
    }

    public bool IsFilterEnabled
    {
        get => GetValue(IsFilterEnabledProperty);
        set => SetValue(IsFilterEnabledProperty, value);
    }

    public bool IsAllowClear
    {
        get => GetValue(IsAllowClearProperty);
        set => SetValue(IsAllowClearProperty, value);
    }

    public bool IsMotionEnabled
    {
        get => GetValue(IsMotionEnabledProperty);
        set => SetValue(IsMotionEnabledProperty, value);
    }

    public bool IsLoading
    {
        get => GetValue(IsLoadingProperty);
        set => SetValue(IsLoadingProperty, value);
    }

    public bool IsSelectionEmpty
    {
        get => GetValue(IsSelectionEmptyProperty);
        set => SetValue(IsSelectionEmptyProperty, value);
    }

    public bool IsDropDownOpen
    {
        get => GetValue(IsDropDownOpenProperty);
        set => SetValue(IsDropDownOpenProperty, value);
    }

    public PathIcon? OpenIndicator
    {
        get => GetValue(OpenIndicatorProperty);
        set => SetValue(OpenIndicatorProperty, value);
    }

    public PathIcon? LoadingIcon
    {
        get => GetValue(LoadingIconProperty);
        set => SetValue(LoadingIconProperty, value);
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

    public static readonly RoutedEvent<RoutedEventArgs> ClearRequestedEvent =
        RoutedEvent.Register<Button, RoutedEventArgs>(nameof(ClearRequested), RoutingStrategies.Bubble);

    public event EventHandler<RoutedEventArgs>? ClearRequested
    {
        add => AddHandler(ClearRequestedEvent, value);
        remove => RemoveHandler(ClearRequestedEvent, value);
    }

    private IconButton? _clearButton;
    private IDisposable? _feedbackStatusSubscription;

    protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
    {
        base.OnApplyTemplate(e);
        if (_clearButton != null)
        {
            _clearButton.Click -= HandleClearButtonClicked;
        }
        _clearButton = e.NameScope.Find<IconButton>("PART_ClearButton");
        if (_clearButton != null)
        {
            _clearButton.Click += HandleClearButtonClicked;
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

    private void HandleClearButtonClicked(object? sender, RoutedEventArgs e)
    {
        RaiseEvent(new RoutedEventArgs(ClearRequestedEvent, this));
    }
}
