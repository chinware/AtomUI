using System.Reactive.Disposables;
using AtomUI.Controls;
using AtomUI.Desktop.Controls.Primitives.Themes;
using AtomUI.Theme;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Metadata;
using Avalonia.Controls.Presenters;
using Avalonia.Controls.Primitives;
using Avalonia.Controls.Templates;
using Avalonia.Data;
using Avalonia.Data.Converters;
using Avalonia.Input;
using Avalonia.Layout;

namespace AtomUI.Desktop.Controls.Primitives;

[PseudoClasses(InfoPickerPseudoClass.Choosing, InfoPickerPseudoClass.FlyoutOpen)]
public abstract class InfoPickerInput : TemplatedControl,
                                        IMotionAwareControl,
                                        ICompactSpaceAware,
                                        IFormItemAware,
                                        IInputControlStatusAware,
                                        IInputControlStyleVariantAware,
                                        ISizeTypeAware,
                                        IFormItemFeedbackAware
{
    #region 公共属性定义
    public static readonly StyledProperty<object?> LeftAddOnProperty =
        AddOnDecoratedBox.LeftAddOnProperty.AddOwner<InfoPickerInput>();
    
    public static readonly StyledProperty<IDataTemplate?> LeftAddOnTemplateProperty =
        AddOnDecoratedBox.LeftAddOnTemplateProperty.AddOwner<InfoPickerInput>();

    public static readonly StyledProperty<object?> RightAddOnProperty =
        AddOnDecoratedBox.RightAddOnProperty.AddOwner<InfoPickerInput>();
    
    public static readonly StyledProperty<IDataTemplate?> RightAddOnTemplateProperty =
        AddOnDecoratedBox.RightAddOnTemplateProperty.AddOwner<InfoPickerInput>();
    
    public static readonly StyledProperty<object?> ContentLeftAddOnProperty =
        AddOnDecoratedBox.ContentLeftAddOnProperty.AddOwner<InfoPickerInput>();
    
    public static readonly StyledProperty<IDataTemplate?> ContentLeftAddOnTemplateProperty =
        AddOnDecoratedBox.ContentLeftAddOnTemplateProperty.AddOwner<InfoPickerInput>();

    public static readonly StyledProperty<object?> ContentRightAddOnProperty =
        AddOnDecoratedBox.ContentRightAddOnProperty.AddOwner<InfoPickerInput>();
    
    public static readonly StyledProperty<IDataTemplate?> ContentRightAddOnTemplateProperty =
        AddOnDecoratedBox.ContentRightAddOnTemplateProperty.AddOwner<InfoPickerInput>();

    public static readonly StyledProperty<SizeType> SizeTypeProperty =
        SizeTypeControlProperty.SizeTypeProperty.AddOwner<InfoPickerInput>();

    public static readonly StyledProperty<InputControlStyleVariant> StyleVariantProperty =
        InputControlStyleVariantProperty.StyleVariantProperty.AddOwner<InfoPickerInput>();

    public static readonly StyledProperty<InputControlStatus> StatusProperty =
        InputControlStatusProperty.StatusProperty.AddOwner<InfoPickerInput>();

    public static readonly StyledProperty<string?> PlaceholderTextProperty =
        AvaloniaProperty.Register<InfoPickerInput, string?>(nameof(PlaceholderText));
    
    public static readonly StyledProperty<PathIcon?> InfoIconProperty =
        AvaloniaProperty.Register<InfoPickerInput, PathIcon?>(nameof(InfoIcon));

    public static readonly StyledProperty<PlacementMode> PickerPlacementProperty =
        AvaloniaProperty.Register<InfoPickerInput, PlacementMode>(nameof(PickerPlacement),
            PlacementMode.BottomEdgeAlignedLeft,
            coerce:CoercePickerPlacement);

    public static readonly StyledProperty<bool> IsShowArrowProperty =
        ArrowDecoratedBox.IsShowArrowProperty.AddOwner<InfoPickerInput>();

    public static readonly StyledProperty<bool> IsPointAtCenterProperty =
        Flyout.IsPointAtCenterProperty.AddOwner<InfoPickerInput>();

    public static readonly StyledProperty<double> MarginToAnchorProperty =
        Popup.MarginToAnchorProperty.AddOwner<InfoPickerInput>();

    public static readonly StyledProperty<int> MouseEnterDelayProperty =
        FlyoutStateHelper.MouseEnterDelayProperty.AddOwner<InfoPickerInput>();

    public static readonly StyledProperty<int> MouseLeaveDelayProperty =
        FlyoutStateHelper.MouseLeaveDelayProperty.AddOwner<InfoPickerInput>();

    public static readonly StyledProperty<bool> IsReadOnlyProperty =
        Avalonia.Controls.TextBox.IsReadOnlyProperty.AddOwner<InfoPickerInput>();

    public static readonly StyledProperty<bool> IsMotionEnabledProperty =
        MotionAwareControlProperty.IsMotionEnabledProperty.AddOwner<InfoPickerInput>();

    public object? LeftAddOn
    {
        get => GetValue(LeftAddOnProperty);
        set => SetValue(LeftAddOnProperty, value);
    }
    
    public IDataTemplate? LeftAddOnTemplate
    {
        get => GetValue(LeftAddOnTemplateProperty);
        set => SetValue(LeftAddOnTemplateProperty, value);
    }

    public object? RightAddOn
    {
        get => GetValue(RightAddOnProperty);
        set => SetValue(RightAddOnProperty, value);
    }
    
    public IDataTemplate? RightAddOnTemplate
    {
        get => GetValue(RightAddOnTemplateProperty);
        set => SetValue(RightAddOnTemplateProperty, value);
    }
    
    public object? ContentLeftAddOn
    {
        get => GetValue(ContentLeftAddOnProperty);
        set => SetValue(ContentLeftAddOnProperty, value);
    }
    
    public IDataTemplate? ContentLeftAddOnTemplate
    {
        get => GetValue(ContentLeftAddOnTemplateProperty);
        set => SetValue(ContentLeftAddOnTemplateProperty, value);
    }

    public object? ContentRightAddOn
    {
        get => GetValue(ContentRightAddOnProperty);
        set => SetValue(ContentRightAddOnProperty, value);
    }
    
    public IDataTemplate? ContentRightAddOnTemplate
    {
        get => GetValue(ContentRightAddOnTemplateProperty);
        set => SetValue(ContentRightAddOnTemplateProperty, value);
    }

    public SizeType SizeType
    {
        get => GetValue(SizeTypeProperty);
        set => SetValue(SizeTypeProperty, value);
    }

    public InputControlStyleVariant StyleVariant
    {
        get => GetValue(StyleVariantProperty);
        set => SetValue(StyleVariantProperty, value);
    }

    public InputControlStatus Status
    {
        get => GetValue(StatusProperty);
        set => SetValue(StatusProperty, value);
    }

    public string? PlaceholderText
    {
        get => GetValue(PlaceholderTextProperty);
        set => SetValue(PlaceholderTextProperty, value);
    }
    
    public PathIcon? InfoIcon
    {
        get => GetValue(InfoIconProperty);
        set => SetValue(InfoIconProperty, value);
    }

    public PlacementMode PickerPlacement
    {
        get => GetValue(PickerPlacementProperty);
        set => SetValue(PickerPlacementProperty, value);
    }

    public bool IsShowArrow
    {
        get => GetValue(IsShowArrowProperty);
        set => SetValue(IsShowArrowProperty, value);
    }

    public bool IsPointAtCenter
    {
        get => GetValue(IsPointAtCenterProperty);
        set => SetValue(IsPointAtCenterProperty, value);
    }

    public double MarginToAnchor
    {
        get => GetValue(MarginToAnchorProperty);
        set => SetValue(MarginToAnchorProperty, value);
    }

    public int MouseEnterDelay
    {
        get => GetValue(MouseEnterDelayProperty);
        set => SetValue(MouseEnterDelayProperty, value);
    }

    public int MouseLeaveDelay
    {
        get => GetValue(MouseLeaveDelayProperty);
        set => SetValue(MouseLeaveDelayProperty, value);
    }

    public bool IsReadOnly
    {
        get => GetValue(IsReadOnlyProperty);
        set => SetValue(IsReadOnlyProperty, value);
    }

    public bool IsMotionEnabled
    {
        get => GetValue(IsMotionEnabledProperty);
        set => SetValue(IsMotionEnabledProperty, value);
    }
    
    #endregion

    #region 内部属性定义

    internal static readonly StyledProperty<string?> TextProperty =
        AvaloniaProperty.Register<InfoPickerInput, string?>(nameof(Text));
    
    internal static readonly DirectProperty<InfoPickerInput, double> PreferredInputWidthProperty =
        AvaloniaProperty.RegisterDirect<InfoPickerInput, double>(nameof(PreferredInputWidth),
            o => o.PreferredInputWidth,
            (o, v) => o.PreferredInputWidth = v);
    
    internal static readonly DirectProperty<InfoPickerInput, bool> IsClearButtonVisibleProperty =
        AvaloniaProperty.RegisterDirect<InfoPickerInput, bool>(nameof(IsClearButtonVisible),
            o => o.IsClearButtonVisible,
            (o, v) => o.IsClearButtonVisible = v);
    
    internal static readonly StyledProperty<SpaceItemPosition?> CompactSpaceItemPositionProperty = 
        CompactSpaceAwareControlProperty.CompactSpaceItemPositionProperty.AddOwner<InfoPickerInput>();
    
    internal static readonly StyledProperty<Orientation> CompactSpaceOrientationProperty = 
        CompactSpaceAwareControlProperty.CompactSpaceOrientationProperty.AddOwner<InfoPickerInput>();
    
    internal static readonly StyledProperty<bool> IsUsedInCompactSpaceProperty = 
        CompactSpaceAwareControlProperty.IsUsedInCompactSpaceProperty.AddOwner<InfoPickerInput>();
    
    public static readonly StyledProperty<IFormValidateFeedback?> FormFeedbackProperty =
        AvaloniaProperty.Register<InfoPickerInput, IFormValidateFeedback?>(nameof (FormFeedback));
    
    public static readonly StyledProperty<bool> ShouldUseOverlayPopupProperty =
        AvaloniaProperty.Register<InfoPickerInput, bool>(nameof(ShouldUseOverlayPopup), true);
    
    protected string? Text
    {
        get => GetValue(TextProperty);
        set => SetValue(TextProperty, value);
    }

    private double _preferredInputWidth = double.NaN;

    internal double PreferredInputWidth
    {
        get => _preferredInputWidth;
        set => SetAndRaise(PreferredInputWidthProperty, ref _preferredInputWidth, value);
    }

    private bool _isClearButtonVisible;

    internal bool IsClearButtonVisible
    {
        get => _isClearButtonVisible;
        set => SetAndRaise(IsClearButtonVisibleProperty, ref _isClearButtonVisible, value);
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
    
    public IFormValidateFeedback? FormFeedback
    {
        get => GetValue(FormFeedbackProperty);
        set => SetValue(FormFeedbackProperty, value);
    }
    
    public bool ShouldUseOverlayPopup
    {
        get => GetValue(ShouldUseOverlayPopupProperty);
        set => SetValue(ShouldUseOverlayPopupProperty, value);
    }

    #endregion

    private protected AddOnDecoratedBox? DecoratedBox;
    private protected PickerClearUpButton? PickerClearUpButton;
    private CompositeDisposable? _contentRightAddOnBindings;
    private protected readonly FlyoutStateHelper FlyoutStateHelper;
    private protected Flyout? PickerFlyout;
    protected bool CurrentValidSelected;
    protected TextBox? InfoInputBox;
    protected Border? PickerInnerBox;

    private protected bool IsFlyoutOpen;
    private protected bool IsChoosing;
    private AddOnDecoratedBox? _addOnDecoratedBox;
    

    static InfoPickerInput()
    {
        AffectsMeasure<InfoPickerInput>(PreferredInputWidthProperty, SizeTypeProperty);
    }

    public InfoPickerInput()
    {
        this.RegisterTokenResourceScope(InfoPickerInputToken.ScopeProvider);
        FlyoutStateHelper = new FlyoutStateHelper
        {
            TriggerType = FlyoutTriggerType.Click
        };
        FlyoutStateHelper.FlyoutAboutToShow                           += HandleFlyoutAboutToShow;
        FlyoutStateHelper.FlyoutAboutToClose                          += HandleFlyoutAboutToClose;
        FlyoutStateHelper.FlyoutOpened                                += HandleFlyoutOpened;
        FlyoutStateHelper.FlyoutClosed                                += HandleFlyoutClosed;
        FlyoutStateHelper[!FlyoutStateHelper.MouseEnterDelayProperty] =  this[!MouseEnterDelayProperty];
        FlyoutStateHelper[!FlyoutStateHelper.MouseLeaveDelayProperty] =  this[!MouseLeaveDelayProperty];
    }

    public virtual void Clear()
    {
        InfoInputBox?.Clear();
    }

    public void ClosePickerFlyout()
    {
        FlyoutStateHelper.HideFlyout(true);
    }

    private void HandleFlyoutAboutToShow(object? sender, EventArgs args)
    {
        CurrentValidSelected = false;
        NotifyFlyoutAboutToShow();
    }

    protected virtual void NotifyFlyoutAboutToShow()
    {
    }

    private void HandleFlyoutAboutToClose(object? sender, EventArgs args)
    {
        NotifyFlyoutAboutToClose(CurrentValidSelected);
    }

    protected virtual void NotifyFlyoutAboutToClose(bool selectedIsValid)
    {
    }

    private void HandleFlyoutOpened(object? sender, EventArgs args)
    {
        NotifyFlyoutOpened();
    }

    protected virtual void NotifyFlyoutOpened()
    {
    }

    private void HandleFlyoutClosed(object? sender, EventArgs args)
    {
        NotifyFlyoutClosed();
    }

    protected virtual void NotifyFlyoutClosed()
    {
    }

    protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
    {
        if (DecoratedBox != null)
        {
            DecoratedBox.TemplateApplied -= HandleDecoratedBoxTemplateApplied;
            DecoratedBox.PropertyChanged -= HandleDecoratedBoxPropertyChanged;
        }
        
        if (PickerClearUpButton is not null)
        {
            PickerClearUpButton.ClearRequest -= HandleClearRequest;
        }
        
        base.OnApplyTemplate(e);
        if (PickerFlyout is null)
        {
            PickerFlyout                  =  CreatePickerFlyout();
            PickerFlyout.Opened           += HandlePickerFlyoutOpened;
            PickerFlyout.Closed           += HandlePickerFlyoutClosed;
            FlyoutStateHelper.Flyout      =  PickerFlyout;
        }

        DecoratedBox = e.NameScope.Get<AddOnDecoratedBox>(AddOnDecoratedBox.AddOnDecoratedBoxPart);
        InfoInputBox = e.NameScope.Get<TextBox>(InfoPickerInputThemeConstants.InfoInputBoxPart);
        PickerClearUpButton = e.NameScope.Find<PickerClearUpButton>(InfoPickerInputThemeConstants.ClearUpButtonPart);

        if (DecoratedBox != null)
        {
            KeyboardNavigation.SetTabOnceActiveElement(this, DecoratedBox);
            DecoratedBox.TemplateApplied += HandleDecoratedBoxTemplateApplied;
            DecoratedBox.PropertyChanged += HandleDecoratedBoxPropertyChanged;
        }

        if (PickerClearUpButton is not null)
        {
            PickerClearUpButton.ClearRequest += HandleClearRequest;
        }
        
        _addOnDecoratedBox = e.NameScope.Find<AddOnDecoratedBox>(AddOnDecoratedBox.AddOnDecoratedBoxPart);

        SetupFlyoutProperties();
        SetupContentRightAddOnBindings(e);
    }

    private void SetupContentRightAddOnBindings(TemplateAppliedEventArgs e)
    {
        _contentRightAddOnBindings?.Dispose();
        _contentRightAddOnBindings = new CompositeDisposable();

        if (PickerClearUpButton is { } clearUpButton)
        {
            _contentRightAddOnBindings.Add(clearUpButton.Bind(PickerClearUpButton.IsInClearModeProperty,
                new Binding(nameof(IsClearButtonVisible)) { Source = this }));
            _contentRightAddOnBindings.Add(clearUpButton.Bind(PickerClearUpButton.IconProperty,
                new Binding(nameof(InfoIcon)) { Source = this }));
            _contentRightAddOnBindings.Add(clearUpButton.Bind(PickerClearUpButton.FormFeedbackProperty,
                new Binding(nameof(FormFeedback)) { Source = this }));
            _contentRightAddOnBindings.Add(clearUpButton.Bind(Visual.IsVisibleProperty,
                new Binding(nameof(InfoIcon)) { Source = this, Converter = ObjectConverters.IsNotNull }));
        }

        if (e.NameScope.Find<ContentPresenter>("PART_ContentRightAddOnPresenter") is { } contentPresenter)
        {
            _contentRightAddOnBindings.Add(contentPresenter.Bind(ContentPresenter.ContentProperty,
                new Binding(nameof(ContentRightAddOn)) { Source = this }));
            _contentRightAddOnBindings.Add(contentPresenter.Bind(ContentPresenter.ContentTemplateProperty,
                new Binding(nameof(ContentRightAddOnTemplate)) { Source = this }));
            _contentRightAddOnBindings.Add(contentPresenter.Bind(Visual.IsVisibleProperty,
                new Binding(nameof(ContentRightAddOn)) { Source = this, Converter = ObjectConverters.IsNotNull }));
        }
    }

    protected virtual void ConfigureIsClearButtonVisible()
    {
        if (DecoratedBox is not null)
        {
            SetCurrentValue(IsClearButtonVisibleProperty, DecoratedBox.IsInnerBoxHover && InfoInputBox?.IsReadOnly == false && InfoInputBox.Text?.Length > 0);
        }
    }

    protected virtual void NotifyClearButtonClicked()
    {
        Clear();
        SetCurrentValue(IsClearButtonVisibleProperty, false);
    }

    protected virtual void NotifyFlyoutPresenterCreated(Control flyoutPresenter)
    {
    }

    protected virtual void SetupFlyoutProperties()
    {
        if (PickerFlyout is not null)
        {
            PickerFlyout[!Flyout.PlacementProperty]       = this[!PickerPlacementProperty];
            PickerFlyout[!Flyout.IsShowArrowProperty]     = this[!IsShowArrowProperty];
            PickerFlyout[!Flyout.IsPointAtCenterProperty] = this[!IsPointAtCenterProperty];
            PickerFlyout[!Flyout.MarginToAnchorProperty]  = this[!MarginToAnchorProperty];
            PickerFlyout[!Flyout.IsMotionEnabledProperty] = this[!IsMotionEnabledProperty];
        }
    }

    protected abstract Flyout CreatePickerFlyout();

    protected virtual void UpdatePseudoClasses()
    {
        PseudoClasses.Set(InfoPickerPseudoClass.FlyoutOpen, IsFlyoutOpen);
        PseudoClasses.Set(InfoPickerPseudoClass.Choosing, IsChoosing);
    }

    protected override void OnAttachedToVisualTree(VisualTreeAttachmentEventArgs e)
    {
        base.OnAttachedToVisualTree(e);
        FlyoutStateHelper.NotifyAttachedToVisualTree();
    }

    private void HandleDecoratedBoxTemplateApplied(object? sender, TemplateAppliedEventArgs args)
    {
        PickerInnerBox                 = DecoratedBox!.ContentFrame;
        FlyoutStateHelper.AnchorTarget = PickerInnerBox;
    }

    private void HandleDecoratedBoxPropertyChanged(object? sender, AvaloniaPropertyChangedEventArgs args)
    {
        if (args.Property == AddOnDecoratedBox.IsInnerBoxHoverProperty)
        {
            ConfigureIsClearButtonVisible();
        }
    }

    private void HandleClearRequest(object? sender, EventArgs args)
    {
        NotifyClearButtonClicked();
    }

    private void HandlePickerFlyoutOpened(object? sender, EventArgs args)
    {
        IsFlyoutOpen = true;
        UpdatePseudoClasses();
    }

    private void HandlePickerFlyoutClosed(object? sender, EventArgs args)
    {
        IsFlyoutOpen = false;
        UpdatePseudoClasses();
    }

    protected override void OnDetachedFromVisualTree(VisualTreeAttachmentEventArgs e)
    {
        base.OnDetachedFromVisualTree(e);
        FlyoutStateHelper.NotifyDetachedFromVisualTree();
        
        if (DecoratedBox != null)
        {
            DecoratedBox.TemplateApplied -= HandleDecoratedBoxTemplateApplied;
            DecoratedBox.PropertyChanged -= HandleDecoratedBoxPropertyChanged;
        }
        
        if (PickerClearUpButton is not null)
        {
            PickerClearUpButton.ClearRequest -= HandleClearRequest;
        }
    }

    protected virtual bool ShowClearButtonPredicate()
    {
        return false;
    }
    
    private static PlacementMode CoercePickerPlacement(object o, PlacementMode value)
    {
        if (value == PlacementMode.Pointer ||
            value == PlacementMode.Custom ||
            value == PlacementMode.AnchorAndGravity ||
            value == PlacementMode.Center) 
        {
            return PlacementMode.BottomEdgeAlignedLeft;
        }

        return value;
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
        if (!IsUsedInCompactSpace)
        {
            return 0.0;
        }

        if (_addOnDecoratedBox == null || _addOnDecoratedBox.StyleVariant != InputControlStyleVariant.Outlined)
        {
            return 0.0;
        }

        // 都一样宽
        return _addOnDecoratedBox.InnerBoxBorderThickness.Left;
    }
    
    #region 实现 FormItem 接口
    
    private EventHandler? _formValueChanged;
    event EventHandler? IFormItemAware.ValueChanged
    {
        add => _formValueChanged += value;
        remove => _formValueChanged -= value;
    }

    void IFormItemAware.SetFormValue(object? value) => NotifySetFormValue(value);

    object? IFormItemAware.GetFormValue() => NotifyGetFormValue();
    void IFormItemAware.ClearFormValue() => NotifyClearFormValue();
    void IFormItemAware.NotifyValidateStatus(FormValidateStatus status) => NotifyValidateStatus(status);
    void IFormItemFeedbackAware.SetFeedbackControl(FormValidateFeedback? value) => NotifySetFeedBackControl(value);
    
    protected virtual void NotifyFormValueChanged(object? value)
    {
        _formValueChanged?.Invoke(this, EventArgs.Empty);
    }

    protected virtual void NotifySetFormValue(object? value)
    {
    }

    protected virtual object? NotifyGetFormValue()
    {
        return null;
    }

    protected virtual void NotifyClearFormValue()
    {
    }

    protected virtual void NotifyValidateStatus(FormValidateStatus status)
    {
        if (status == FormValidateStatus.Error)
        {
            SetCurrentValue(StatusProperty, InputControlStatus.Error);
        }
        else if (status == FormValidateStatus.Warning)
        {
            SetCurrentValue(StatusProperty, InputControlStatus.Warning);
        }
        else
        {
            SetCurrentValue(StatusProperty, InputControlStatus.Default);
        }
    }
    
    protected virtual void NotifySetFeedBackControl(FormValidateFeedback? value)
    {
        FormFeedback = value;
    }
    #endregion
}