using AtomUI.Controls;
using AtomUI.Icons.AntDesign;
using AtomUI.Reflection;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Metadata;
using Avalonia.Controls.Primitives;
using Avalonia.Interactivity;
using Avalonia.Layout;
using Avalonia.LogicalTree;

namespace AtomUI.Desktop.Controls;

[TemplatePart("PART_IndicatorHost", typeof(Panel))]
internal class SelectHandle : TemplatedControl
{
    private const string OpenIndicatorName = "OpenIndicator";
    private const string LoadingIndicatorName = "LoadingIndicator";
    private const string SearchIndicatorName = "SearchIndicator";
    private const string ClearButtonName = "PART_ClearButton";

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

    private Panel? _indicatorHost;
    private IconPresenter? _openIndicatorPresenter;
    private IconPresenter? _loadingIndicatorPresenter;
    private SearchOutlined? _searchIndicator;
    private InputClearIconButton? _clearButton;
    private IDisposable? _feedbackStatusSubscription;

    protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
    {
        DetachIndicators();
        base.OnApplyTemplate(e);
        _indicatorHost = e.NameScope.Find<Panel>("PART_IndicatorHost");
        UpdateIndicators();
    }

    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        base.OnPropertyChanged(change);
        if (change.Property == FormFeedbackProperty)
        {
            ConfigureFormFeedbackSubscription();
        }
        else if (change.Property == OpenIndicatorProperty ||
                 change.Property == LoadingIconProperty ||
                 change.Property == IsMotionEnabledProperty ||
                 change.Property == IsLoadingProperty ||
                 change.Property == IsFilterEnabledProperty ||
                 change.Property == IsDropDownOpenProperty ||
                 change.Property == IsAllowClearProperty ||
                 change.Property == IsSelectionEmptyProperty ||
                 change.Property == IsInputHoverProperty ||
                 change.Property == IsInputPressedProperty)
        {
            UpdateIndicators();
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

    private void UpdateIndicators()
    {
        if (_indicatorHost is null)
        {
            return;
        }

        var isClearVisible  = IsAllowClear && !IsSelectionEmpty && (IsInputHover || IsInputPressed);
        var isSearchVisible = IsFilterEnabled && IsDropDownOpen && !isClearVisible;
        var isOpenVisible   = !IsLoading && !isSearchVisible && !isClearVisible && OpenIndicator is not null;

        UpdateOpenIndicator(isOpenVisible);
        UpdateLoadingIndicator(IsLoading && LoadingIcon is not null);
        UpdateSearchIndicator(isSearchVisible);
        UpdateClearButton(isClearVisible);
        EnsureIndicatorOrder();
    }

    private void UpdateOpenIndicator(bool isVisible)
    {
        if (!isVisible)
        {
            DetachOpenIndicator();
            return;
        }

        _openIndicatorPresenter ??= CreateIconPresenter(OpenIndicatorName);
        _openIndicatorPresenter.SetCurrentValue(IconPresenter.IconProperty, OpenIndicator);
        _openIndicatorPresenter.SetCurrentValue(IconPresenter.IsMotionEnabledProperty, IsMotionEnabled);
        EnsureIndicatorAttached(_openIndicatorPresenter);
    }

    private void UpdateLoadingIndicator(bool isVisible)
    {
        if (!isVisible)
        {
            DetachLoadingIndicator();
            return;
        }

        _loadingIndicatorPresenter ??= CreateIconPresenter(LoadingIndicatorName);
        _loadingIndicatorPresenter.SetCurrentValue(IconPresenter.IconProperty, LoadingIcon);
        _loadingIndicatorPresenter.SetCurrentValue(IconPresenter.IsMotionEnabledProperty, IsMotionEnabled);
        EnsureIndicatorAttached(_loadingIndicatorPresenter);
    }

    private void UpdateSearchIndicator(bool isVisible)
    {
        if (!isVisible)
        {
            DetachSearchIndicator();
            return;
        }

        _searchIndicator ??= CreateSearchIndicator();
        _searchIndicator.SetCurrentValue(Icon.IsMotionEnabledProperty, IsMotionEnabled);
        EnsureIndicatorAttached(_searchIndicator);
    }

    private void UpdateClearButton(bool isVisible)
    {
        if (!isVisible)
        {
            DetachClearButton();
            return;
        }

        if (_clearButton is null)
        {
            _clearButton = new InputClearIconButton
            {
                Name = ClearButtonName
            };
            _clearButton.SetTemplatedParent(this);
            _clearButton.Click += HandleClearButtonClicked;
        }
        _clearButton.SetCurrentValue(IconButton.IsMotionEnabledProperty, IsMotionEnabled);
        EnsureIndicatorAttached(_clearButton);
    }

    private IconPresenter CreateIconPresenter(string name)
    {
        var presenter = new IconPresenter
        {
            Name                = name,
            HorizontalAlignment = HorizontalAlignment.Center,
            VerticalAlignment   = VerticalAlignment.Center
        };
        presenter.SetTemplatedParent(this);
        return presenter;
    }

    private SearchOutlined CreateSearchIndicator()
    {
        var indicator = new SearchOutlined
        {
            Name                = SearchIndicatorName,
            HorizontalAlignment = HorizontalAlignment.Center,
            VerticalAlignment   = VerticalAlignment.Center
        };
        indicator.SetTemplatedParent(this);
        return indicator;
    }

    private void EnsureIndicatorAttached(Control indicator)
    {
        if (_indicatorHost != null && !_indicatorHost.Children.Contains(indicator))
        {
            _indicatorHost.Children.Add(indicator);
        }
    }

    private void EnsureIndicatorOrder()
    {
        var index = 0;
        EnsureIndicatorAt(_openIndicatorPresenter, ref index);
        EnsureIndicatorAt(_loadingIndicatorPresenter, ref index);
        EnsureIndicatorAt(_searchIndicator, ref index);
        EnsureIndicatorAt(_clearButton, ref index);
    }

    private void EnsureIndicatorAt(Control? indicator, ref int index)
    {
        if (_indicatorHost is null || indicator is null)
        {
            return;
        }

        var currentIndex = _indicatorHost.Children.IndexOf(indicator);
        if (currentIndex == index)
        {
            index++;
            return;
        }

        if (currentIndex >= 0)
        {
            _indicatorHost.Children.RemoveAt(currentIndex);
        }

        _indicatorHost.Children.Insert(index, indicator);
        index++;
    }

    private void DetachIndicators()
    {
        DetachOpenIndicator();
        DetachLoadingIndicator();
        DetachSearchIndicator();
        DetachClearButton();
    }

    private void DetachOpenIndicator()
    {
        if (_openIndicatorPresenter is null)
        {
            return;
        }

        _openIndicatorPresenter.SetCurrentValue(IconPresenter.IconProperty, null);
        _indicatorHost?.Children.Remove(_openIndicatorPresenter);
        _openIndicatorPresenter.SetTemplatedParent(null);
        _openIndicatorPresenter = null;
    }

    private void DetachLoadingIndicator()
    {
        if (_loadingIndicatorPresenter is null)
        {
            return;
        }

        _loadingIndicatorPresenter.SetCurrentValue(IconPresenter.IconProperty, null);
        _indicatorHost?.Children.Remove(_loadingIndicatorPresenter);
        _loadingIndicatorPresenter.SetTemplatedParent(null);
        _loadingIndicatorPresenter = null;
    }

    private void DetachSearchIndicator()
    {
        if (_searchIndicator is null)
        {
            return;
        }

        _indicatorHost?.Children.Remove(_searchIndicator);
        _searchIndicator.SetTemplatedParent(null);
        _searchIndicator = null;
    }

    private void DetachClearButton()
    {
        if (_clearButton is null)
        {
            return;
        }

        _clearButton.Click -= HandleClearButtonClicked;
        _indicatorHost?.Children.Remove(_clearButton);
        _clearButton.SetTemplatedParent(null);
        _clearButton = null;
    }
}
