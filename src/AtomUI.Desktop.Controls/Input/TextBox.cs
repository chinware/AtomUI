using AtomUI.Controls;
using AtomUI.Controls.Commons;
using AtomUI.Desktop.Controls.Primitives.Themes;
using AtomUI.Reflection;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Presenters;
using Avalonia.Controls.Primitives;
using Avalonia.Interactivity;
using Avalonia.Layout;
using Avalonia.LogicalTree;

namespace AtomUI.Desktop.Controls;

using AvaloniaTextBox = Avalonia.Controls.TextBox;

public class TextBox : AvaloniaTextBox,
                       IMotionAwareControl,
                       ISizeTypeAware,
                       ICompactSpaceAware,
                       IFormItemAware,
                       IFormItemFeedbackAware
{
    #region 公共属性定义

    public static readonly StyledProperty<SizeType> SizeTypeProperty =
        SizeTypeControlProperty.SizeTypeProperty.AddOwner<TextBox>();

    public static readonly StyledProperty<bool> IsAllowClearProperty =
        AvaloniaProperty.Register<TextBox, bool>(nameof(IsAllowClear));

    public static readonly StyledProperty<bool> IsEnableRevealButtonProperty =
        AvaloniaProperty.Register<TextBox, bool>(nameof(IsEnableRevealButton));
    
    public static readonly StyledProperty<bool> IsCustomFontSizeProperty =
        AvaloniaProperty.Register<TextBox, bool>(nameof(IsCustomFontSize));
    
    public static readonly StyledProperty<bool> IsShowCountProperty =
        AvaloniaProperty.Register<TextBox, bool>(nameof(IsShowCount));
    
    public static readonly StyledProperty<PathIcon?> ClearIconProperty =
        AvaloniaProperty.Register<TextBox, PathIcon?>(nameof(ClearIcon));
    
    public static readonly StyledProperty<bool> IsMotionEnabledProperty = 
        MotionAwareControlProperty.IsMotionEnabledProperty.AddOwner<TextBox>();
    
    public SizeType SizeType
    {
        get => GetValue(SizeTypeProperty);
        set => SetValue(SizeTypeProperty, value);
    }
    
    public bool IsAllowClear
    {
        get => GetValue(IsAllowClearProperty);
        set => SetValue(IsAllowClearProperty, value);
    }

    public bool IsEnableRevealButton
    {
        get => GetValue(IsEnableRevealButtonProperty);
        set => SetValue(IsEnableRevealButtonProperty, value);
    }
    
    public bool IsCustomFontSize
    {
        get => GetValue(IsCustomFontSizeProperty);
        set => SetValue(IsCustomFontSizeProperty, value);
    }
    
    public bool IsShowCount
    {
        get => GetValue(IsShowCountProperty);
        set => SetValue(IsShowCountProperty, value);
    }
    
    public PathIcon? ClearIcon
    {
        get => GetValue(ClearIconProperty);
        set => SetValue(ClearIconProperty, value);
    }
    
    public bool IsMotionEnabled
    {
        get => GetValue(IsMotionEnabledProperty);
        set => SetValue(IsMotionEnabledProperty, value);
    }
    #endregion

    #region 内部属性定义

    internal static readonly DirectProperty<TextBox, bool> IsEffectiveShowClearButtonProperty =
        AvaloniaProperty.RegisterDirect<TextBox, bool>(nameof(IsEffectiveShowClearButton),
            o => o.IsEffectiveShowClearButton,
            (o, v) => o.IsEffectiveShowClearButton = v);
    
    internal static readonly DirectProperty<TextBox, string?> CountTextProperty =
        AvaloniaProperty.RegisterDirect<TextBox, string?>(nameof(CountText),
            o => o.CountText,
            (o, v) => o.CountText = v);
    
    internal static readonly DirectProperty<TextBox, CornerRadius> EffectiveCornerRadiusProperty =
        AvaloniaProperty.RegisterDirect<TextBox, CornerRadius>(nameof(EffectiveCornerRadius),
            o => o.EffectiveCornerRadius,
            (o, v) => o.EffectiveCornerRadius = v);
    
    internal static readonly StyledProperty<SpaceItemPosition?> CompactSpaceItemPositionProperty = 
        CompactSpaceAwareControlProperty.CompactSpaceItemPositionProperty.AddOwner<TextBox>();
    
    internal static readonly StyledProperty<Orientation> CompactSpaceOrientationProperty = 
        CompactSpaceAwareControlProperty.CompactSpaceOrientationProperty.AddOwner<TextBox>();
    
    internal static readonly StyledProperty<bool> IsUsedInCompactSpaceProperty = 
        CompactSpaceAwareControlProperty.IsUsedInCompactSpaceProperty.AddOwner<TextBox>();
    
    internal static readonly StyledProperty<FormValidateFeedback?> FormFeedbackProperty =
        AvaloniaProperty.Register<TextBox, FormValidateFeedback?>(nameof(FormFeedback));

    internal static readonly DirectProperty<TextBox, bool> IsFormFeedbackVisibleProperty =
        AvaloniaProperty.RegisterDirect<TextBox, bool>(
            nameof(IsFormFeedbackVisible),
            o => o.IsFormFeedbackVisible);

    private bool _isEffectiveShowClearButton;

    internal bool IsEffectiveShowClearButton
    {
        get => _isEffectiveShowClearButton;
        set => SetAndRaise(IsEffectiveShowClearButtonProperty, ref _isEffectiveShowClearButton, value);
    }

    private string? _countText;

    internal string? CountText
    {
        get => _countText;
        set => SetAndRaise(CountTextProperty, ref _countText, value);
    }
    
    private CornerRadius _effectiveCornerRadius;

    internal CornerRadius EffectiveCornerRadius
    {
        get => _effectiveCornerRadius;
        set => SetAndRaise(EffectiveCornerRadiusProperty, ref _effectiveCornerRadius, value);
    }
    
    internal SpaceItemPosition? CompactSpaceItemPosition
    {
        get => GetValue(CompactSpaceItemPositionProperty);
        set => SetValue(CompactSpaceItemPositionProperty, value);
    }
    
    internal Orientation CompactSpaceOrientation
    {
        get => GetValue(CompactSpaceOrientationProperty);
        set => SetValue(CompactSpaceOrientationProperty, value);
    }
    
    internal bool IsUsedInCompactSpace
    {
        get => GetValue(IsUsedInCompactSpaceProperty);
        set => SetValue(IsUsedInCompactSpaceProperty, value);
    }
    
    internal FormValidateFeedback? FormFeedback
    {
        get => GetValue(FormFeedbackProperty);
        set => SetValue(FormFeedbackProperty, value);
    }

    private bool _isFormFeedbackVisible;

    internal bool IsFormFeedbackVisible
    {
        get => _isFormFeedbackVisible;
        private set => SetAndRaise(IsFormFeedbackVisibleProperty, ref _isFormFeedbackVisible, value);
    }
    
    #endregion

    private DockPanel? _contentLayout;
    private Button? _templateClearButton;
    private StackPanel? _leftAddOnLayout;
    private StackPanel? _rightAddOnLayout;
    private ContentPresenter? _leftAddOnPresenter;
    private InputClearIconButton? _clearButton;
    private RevealButton? _revealButton;
    private IDisposable? _revealButtonSubscription;
    private ContentPresenter? _formFeedbackPresenter;
    private ContentPresenter? _innerRightContentPresenter;
    private TextBlock? _countTextIndicator;
    private bool _isUsingTemplateAccessorySlots;
    private IDisposable? _feedbackStatusSubscription;

    static TextBox()
    {
        AffectsArrange<TextBox>(CompactSpaceItemPositionProperty, CompactSpaceOrientationProperty);
        TextChangedEvent.AddClassHandler<TextBox>((textBox, args) => textBox.HandleTextChanged());
    }

    public TextBox()
    {
        // this.RegisterTokenResourceScope(LineEditToken.ScopeProvider);
    }

    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        base.OnPropertyChanged(change);
    
        if (change.Property == AcceptsReturnProperty ||
            change.Property == IsReadOnlyProperty ||
            change.Property == TextProperty ||
            change.Property == IsAllowClearProperty)
        {
            ConfigureEffectiveShowClearButton();
        }
        else if (change.Property == IsShowCountProperty ||
                 change.Property == MaxLengthProperty)
        {
            HandleInputChanged(Text);
        }
        else if (change.Property == CornerRadiusProperty ||
                 change.Property == CompactSpaceItemPositionProperty ||
                 change.Property == CompactSpaceOrientationProperty)
        {
            ConfigureCornerRadius();
        }
        else if (change.Property == FormFeedbackProperty)
        {
            ConfigureFormFeedbackSubscription();
        }

        if (IsRuntimeAccessoryStateProperty(change.Property))
        {
            ConfigureRuntimeAccessories();
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

    private void ConfigureCornerRadius()
    {
        EffectiveCornerRadius = CompactSpace.CalculateEffectiveCornerRadius(
            CornerRadius, 
            IsUsedInCompactSpace, 
            CompactSpaceItemPosition,
            CompactSpaceOrientation);
    }

    private void ConfigureEffectiveShowClearButton()
    {
        if (!IsAllowClear)
        {
            SetCurrentValue(IsEffectiveShowClearButtonProperty, false);
            return;
        }
        
        SetCurrentValue(IsEffectiveShowClearButtonProperty, !IsReadOnly && !AcceptsReturn && !string.IsNullOrEmpty(Text));
    }

    protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
    {
        ClearRuntimeAccessories();
        if (_templateClearButton != null)
        {
            _templateClearButton.Click -= HandleClearButtonClicked;
            _templateClearButton = null;
        }

        base.OnApplyTemplate(e);

        _contentLayout = e.NameScope.Find<DockPanel>("ContentLayout");
        _templateClearButton = e.NameScope.Find<Button>("PART_ClearButton");
        _isUsingTemplateAccessorySlots = _templateClearButton != null;
        if (_templateClearButton != null)
        {
            _templateClearButton.Click += HandleClearButtonClicked;
        }
        ConfigureEffectiveShowClearButton();
        HandleInputChanged(Text);
        ConfigureRuntimeAccessories();
    }

    private static bool IsRuntimeAccessoryStateProperty(AvaloniaProperty property)
    {
        return property == InnerLeftContentProperty ||
               property == InnerRightContentProperty ||
               property == IsEffectiveShowClearButtonProperty ||
               property == ClearIconProperty ||
               property == IsMotionEnabledProperty ||
               property == IsEnableRevealButtonProperty ||
               property == RevealPasswordProperty ||
               property == FormFeedbackProperty ||
               property == IsFormFeedbackVisibleProperty ||
               property == IsShowCountProperty ||
               property == CountTextProperty;
    }

    private void ConfigureRuntimeAccessories()
    {
        if (_contentLayout == null || _isUsingTemplateAccessorySlots)
        {
            return;
        }

        UpdateLeftAddOnPresenter();
        UpdateClearButton();
        UpdateRevealButton();
        UpdateFormFeedbackPresenter();
        UpdateInnerRightContentPresenter();
        UpdateCountTextIndicator();
        EnsureAccessoryLayouts();
    }

    private void UpdateLeftAddOnPresenter()
    {
        if (InnerLeftContent == null)
        {
            DestroyLeftAddOnPresenter();
            return;
        }

        EnsureLeftAddOnLayout();
        if (_leftAddOnPresenter == null)
        {
            _leftAddOnPresenter = new ContentPresenter
            {
                Name                     = AddOnDecoratedBoxThemeConstants.LeftAddOnPart,
                VerticalAlignment        = VerticalAlignment.Stretch,
                VerticalContentAlignment = VerticalAlignment.Center,
                HorizontalAlignment      = HorizontalAlignment.Left,
                Focusable                = false
            };
            _leftAddOnPresenter.SetTemplatedParent(this);
            _leftAddOnLayout!.Children.Add(_leftAddOnPresenter);
        }

        _leftAddOnPresenter.SetCurrentValue(ContentPresenter.ContentProperty, InnerLeftContent);
    }

    private void DestroyLeftAddOnPresenter()
    {
        if (_leftAddOnPresenter == null)
        {
            DestroyLeftAddOnLayoutIfEmpty();
            return;
        }

        _leftAddOnPresenter.ClearValue(ContentPresenter.ContentProperty);
        _leftAddOnLayout?.Children.Remove(_leftAddOnPresenter);
        _leftAddOnPresenter.SetTemplatedParent(null);
        _leftAddOnPresenter = null;
        DestroyLeftAddOnLayoutIfEmpty();
    }

    private void UpdateClearButton()
    {
        if (!IsEffectiveShowClearButton)
        {
            DestroyClearButton();
            return;
        }

        EnsureRightAddOnLayout();
        if (_clearButton == null)
        {
            _clearButton = new InputClearIconButton
            {
                Name              = "PART_ClearButton",
                VerticalAlignment = VerticalAlignment.Center
            };
            _clearButton.SetTemplatedParent(this);
            _clearButton.Click += HandleClearButtonClicked;
            _rightAddOnLayout!.Children.Add(_clearButton);
        }

        _clearButton.SyncIcon(ClearIcon);
        _clearButton.SetCurrentValue(AbstractIconButton.IsMotionEnabledProperty, IsMotionEnabled);
    }

    private void DestroyClearButton()
    {
        if (_clearButton == null)
        {
            DestroyRightAddOnLayoutIfEmpty();
            return;
        }

        _clearButton.Click -= HandleClearButtonClicked;
        _clearButton.ClearValue(AbstractIconButton.IconProperty);
        _clearButton.ClearValue(AbstractIconButton.IsMotionEnabledProperty);
        _rightAddOnLayout?.Children.Remove(_clearButton);
        _clearButton.SetTemplatedParent(null);
        _clearButton = null;
        DestroyRightAddOnLayoutIfEmpty();
    }

    private void UpdateRevealButton()
    {
        if (!IsEnableRevealButton)
        {
            DestroyRevealButton();
            return;
        }

        EnsureRightAddOnLayout();
        if (_revealButton == null)
        {
            _revealButton = new RevealButton
            {
                Name              = "PART_RevealButton",
                VerticalAlignment = VerticalAlignment.Center
            };
            _revealButton.SetTemplatedParent(this);
            _revealButton.SetCurrentValue(ToggleButton.IsCheckedProperty, RevealPassword);
            _revealButtonSubscription = _revealButton.GetObservable(ToggleButton.IsCheckedProperty)
                                                     .Subscribe(HandleRevealButtonCheckedChanged);
            _rightAddOnLayout!.Children.Add(_revealButton);
        }

        _revealButton.SetCurrentValue(ToggleButton.IsCheckedProperty, RevealPassword);
        _revealButton.SetCurrentValue(AbstractIconButton.IsMotionEnabledProperty, IsMotionEnabled);
    }

    private void DestroyRevealButton()
    {
        if (_revealButton == null)
        {
            DestroyRightAddOnLayoutIfEmpty();
            return;
        }

        _revealButtonSubscription?.Dispose();
        _revealButtonSubscription = null;
        _revealButton.ClearValue(ToggleButton.IsCheckedProperty);
        _revealButton.ClearValue(AbstractIconButton.IsMotionEnabledProperty);
        _rightAddOnLayout?.Children.Remove(_revealButton);
        _revealButton.SetTemplatedParent(null);
        _revealButton = null;
        DestroyRightAddOnLayoutIfEmpty();
    }

    private void HandleRevealButtonCheckedChanged(bool? isChecked)
    {
        var revealPassword = isChecked == true;
        if (RevealPassword != revealPassword)
        {
            SetCurrentValue(RevealPasswordProperty, revealPassword);
        }
    }

    private void UpdateFormFeedbackPresenter()
    {
        if (!IsFormFeedbackVisible || FormFeedback == null)
        {
            DestroyFormFeedbackPresenter();
            return;
        }

        EnsureRightAddOnLayout();
        if (_formFeedbackPresenter == null)
        {
            _formFeedbackPresenter = new ContentPresenter
            {
                Name                     = "FormFeedBack",
                VerticalAlignment        = VerticalAlignment.Stretch,
                VerticalContentAlignment = VerticalAlignment.Center,
                HorizontalAlignment      = HorizontalAlignment.Right
            };
            _formFeedbackPresenter.SetTemplatedParent(this);
            _rightAddOnLayout!.Children.Add(_formFeedbackPresenter);
        }

        _formFeedbackPresenter.SetCurrentValue(ContentPresenter.ContentProperty, FormFeedback);
    }

    private void DestroyFormFeedbackPresenter()
    {
        if (_formFeedbackPresenter == null)
        {
            DestroyRightAddOnLayoutIfEmpty();
            return;
        }

        _formFeedbackPresenter.ClearValue(ContentPresenter.ContentProperty);
        _rightAddOnLayout?.Children.Remove(_formFeedbackPresenter);
        _formFeedbackPresenter.SetTemplatedParent(null);
        _formFeedbackPresenter = null;
        DestroyRightAddOnLayoutIfEmpty();
    }

    private void UpdateInnerRightContentPresenter()
    {
        if (InnerRightContent == null)
        {
            DestroyInnerRightContentPresenter();
            return;
        }

        EnsureRightAddOnLayout();
        if (_innerRightContentPresenter == null)
        {
            _innerRightContentPresenter = new ContentPresenter
            {
                Name                     = AddOnDecoratedBoxThemeConstants.RightAddOnPart,
                VerticalAlignment        = VerticalAlignment.Stretch,
                VerticalContentAlignment = VerticalAlignment.Center,
                HorizontalAlignment      = HorizontalAlignment.Right,
                Focusable                = false
            };
            _innerRightContentPresenter.SetTemplatedParent(this);
            _rightAddOnLayout!.Children.Add(_innerRightContentPresenter);
        }

        _innerRightContentPresenter.SetCurrentValue(ContentPresenter.ContentProperty, InnerRightContent);
    }

    private void DestroyInnerRightContentPresenter()
    {
        if (_innerRightContentPresenter == null)
        {
            DestroyRightAddOnLayoutIfEmpty();
            return;
        }

        _innerRightContentPresenter.ClearValue(ContentPresenter.ContentProperty);
        _rightAddOnLayout?.Children.Remove(_innerRightContentPresenter);
        _innerRightContentPresenter.SetTemplatedParent(null);
        _innerRightContentPresenter = null;
        DestroyRightAddOnLayoutIfEmpty();
    }

    private void UpdateCountTextIndicator()
    {
        if (!IsShowCount)
        {
            DestroyCountTextIndicator();
            return;
        }

        EnsureRightAddOnLayout();
        if (_countTextIndicator == null)
        {
            _countTextIndicator = new TextBlock
            {
                Name              = "TextCountIndicator",
                VerticalAlignment = VerticalAlignment.Center
            };
            _countTextIndicator.SetTemplatedParent(this);
            _rightAddOnLayout!.Children.Add(_countTextIndicator);
        }

        _countTextIndicator.SetCurrentValue(Avalonia.Controls.TextBlock.TextProperty, CountText);
    }

    private void DestroyCountTextIndicator()
    {
        if (_countTextIndicator == null)
        {
            DestroyRightAddOnLayoutIfEmpty();
            return;
        }

        _countTextIndicator.ClearValue(Avalonia.Controls.TextBlock.TextProperty);
        _rightAddOnLayout?.Children.Remove(_countTextIndicator);
        _countTextIndicator.SetTemplatedParent(null);
        _countTextIndicator = null;
        DestroyRightAddOnLayoutIfEmpty();
    }

    private void EnsureLeftAddOnLayout()
    {
        if (_leftAddOnLayout != null || _contentLayout == null)
        {
            return;
        }

        _leftAddOnLayout = new StackPanel
        {
            Name        = AddOnDecoratedBoxThemeConstants.LeftAddOnLayoutPart,
            Orientation = Orientation.Horizontal
        };
        DockPanel.SetDock(_leftAddOnLayout, Dock.Left);
        _leftAddOnLayout.SetTemplatedParent(this);
        _contentLayout.Children.Insert(0, _leftAddOnLayout);
    }

    private void EnsureRightAddOnLayout()
    {
        if (_rightAddOnLayout != null || _contentLayout == null)
        {
            return;
        }

        _rightAddOnLayout = new StackPanel
        {
            Name        = AddOnDecoratedBoxThemeConstants.RightAddOnLayoutPart,
            Orientation = Orientation.Horizontal
        };
        DockPanel.SetDock(_rightAddOnLayout, Dock.Right);
        _rightAddOnLayout.SetTemplatedParent(this);
        _contentLayout.Children.Insert(0, _rightAddOnLayout);
    }

    private void EnsureAccessoryLayouts()
    {
        if (_contentLayout == null)
        {
            return;
        }

        var index = 0;
        EnsureAccessoryLayoutAt(_leftAddOnLayout, ref index);
        EnsureAccessoryLayoutAt(_rightAddOnLayout, ref index);
    }

    private void EnsureAccessoryLayoutAt(StackPanel? layout, ref int index)
    {
        if (layout == null || _contentLayout == null)
        {
            return;
        }

        var currentIndex = _contentLayout.Children.IndexOf(layout);
        if (currentIndex == index)
        {
            index++;
            return;
        }

        if (currentIndex >= 0)
        {
            _contentLayout.Children.RemoveAt(currentIndex);
        }

        _contentLayout.Children.Insert(Math.Min(index, _contentLayout.Children.Count), layout);
        index++;
    }

    private void DestroyLeftAddOnLayoutIfEmpty()
    {
        if (_leftAddOnLayout == null || _leftAddOnLayout.Children.Count > 0)
        {
            return;
        }

        _contentLayout?.Children.Remove(_leftAddOnLayout);
        _leftAddOnLayout.SetTemplatedParent(null);
        _leftAddOnLayout = null;
    }

    private void DestroyRightAddOnLayoutIfEmpty()
    {
        if (_rightAddOnLayout == null || _rightAddOnLayout.Children.Count > 0)
        {
            return;
        }

        _contentLayout?.Children.Remove(_rightAddOnLayout);
        _rightAddOnLayout.SetTemplatedParent(null);
        _rightAddOnLayout = null;
    }

    private void ClearRuntimeAccessories()
    {
        DestroyLeftAddOnPresenter();
        DestroyClearButton();
        DestroyRevealButton();
        DestroyFormFeedbackPresenter();
        DestroyInnerRightContentPresenter();
        DestroyCountTextIndicator();
        DestroyLeftAddOnLayoutIfEmpty();
        DestroyRightAddOnLayoutIfEmpty();
        _contentLayout = null;
    }

    private void HandleClearButtonClicked(object? sender, RoutedEventArgs args)
    {
        NotifyClearButtonClicked();
    }
    
    protected virtual void NotifyClearButtonClicked() => Clear();

    private void HandleInputChanged(string? text)
    {
        if (IsShowCount)
        {
            SetCurrentValue(CountTextProperty, $"{text?.Length ?? 0} / {MaxLength}");
        }
    }
    
    void ICompactSpaceAware.NotifyPositionChange(SpaceItemPosition? position)
    {
        IsUsedInCompactSpace     = position != null;
        CompactSpaceItemPosition = position;
    }
    
    void ICompactSpaceAware.NotifyOrientationChange(Orientation orientation)
    {
        CompactSpaceOrientation = orientation;
    }

    double ICompactSpaceAware.GetBorderThickness()
    {
        return GetBorderThicknessForCompactSpace();
    }
    
    protected virtual double GetBorderThicknessForCompactSpace()
    {
        return CompactSpaceOrientation == Orientation.Horizontal ? BorderThickness.Left : BorderThickness.Top;
    }

    #region 实现 FormItem 接口
    private EventHandler? _formValueChanged;
    event EventHandler? IFormItemAware.ValueChanged
    {
        add => _formValueChanged += value;
        remove => _formValueChanged -= value;
    }

    void IFormItemAware.SetFormValue(object? value) => NotifySetFormValue(value?.ToString());

    object? IFormItemAware.GetFormValue() => NotifyGetFormValue();
    void IFormItemAware.ClearFormValue() => NotifyClearFormValue();
    void IFormItemAware.NotifyValidateStatus(FormValidateStatus status) => NotifyValidateStatus(status);
    void IFormItemFeedbackAware.SetFeedbackControl(FormValidateFeedback? value) => NotifySetFeedBackControl(value);
    
    private void HandleTextChanged()
    {
        HandleInputChanged(Text);
        _formValueChanged?.Invoke(this, EventArgs.Empty);
    }

    protected virtual void NotifySetFormValue(string? value)
    {
        SetCurrentValue(TextProperty, value);
    }

    protected virtual string? NotifyGetFormValue()
    {
        return Text;
    }

    protected virtual void NotifyClearFormValue()
    {
        SetCurrentValue(TextProperty, null);
    }

    protected virtual void NotifyValidateStatus(FormValidateStatus status)
    {
    }

    protected virtual void NotifySetFeedBackControl(FormValidateFeedback? value)
    {
        FormFeedback = value;
    }
    #endregion
}
