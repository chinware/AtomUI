using System.ComponentModel;
using System.Reactive.Disposables;
using System.Reactive.Disposables.Fluent;
using AtomUI.Controls;
using AtomUI.Data;
using AtomUI.Icons.AntDesign;
using AtomUI.Reflection;
using AtomUI.Theme.Styling;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Presenters;
using Avalonia.Controls.Primitives;
using Avalonia.Controls.Templates;
using Avalonia.Data;
using Avalonia.Data.Converters;
using Avalonia.Input;
using Avalonia.Layout;
using Avalonia.Media;
using Avalonia.Metadata;

namespace AtomUI.Desktop.Controls;

public abstract class AbstractSelect : TemplatedControl,
                                       IMotionAwareControl,
                                       ISizeTypeAware,
                                       ICompactSpaceAware,
                                       IInputControlStatusAware,
                                       IInputControlStyleVariantAware,
                                       IFormItemAware,
                                       IFormItemFeedbackAware
{
    #region 公共属性定义
    public static readonly StyledProperty<bool> IsAllowClearProperty =
        AvaloniaProperty.Register<AbstractSelect, bool>(nameof(IsAllowClear));

    public static readonly StyledProperty<bool> IsAutoClearSearchValueProperty =
        AvaloniaProperty.Register<AbstractSelect, bool>(nameof(IsAutoClearSearchValue));

    public static readonly StyledProperty<bool> IsDefaultOpenProperty =
        AvaloniaProperty.Register<AbstractSelect, bool>(nameof(IsDefaultOpen));

    public static readonly StyledProperty<bool> IsDropDownOpenProperty =
        AvaloniaProperty.Register<AbstractSelect, bool>(nameof(IsDropDownOpen));

    public static readonly StyledProperty<string?> PlaceholderTextProperty =
        AvaloniaProperty.Register<AbstractSelect, string?>(nameof(PlaceholderText));

    public static readonly StyledProperty<IBrush?> PlaceholderForegroundProperty =
        AvaloniaProperty.Register<AbstractSelect, IBrush?>(nameof(PlaceholderForeground));

    public static readonly StyledProperty<bool> IsPopupMatchSelectWidthProperty =
        AvaloniaProperty.Register<AbstractSelect, bool>(nameof(IsPopupMatchSelectWidth), true);

    public static readonly StyledProperty<bool> ShouldUseOverlayPopupProperty =
        AvaloniaProperty.Register<AbstractSelect, bool>(nameof(ShouldUseOverlayPopup), true);

    public static readonly StyledProperty<bool> IsFilterEnabledProperty =
        AvaloniaProperty.Register<AbstractSelect, bool>(nameof(IsFilterEnabled));

    public static readonly StyledProperty<int> DisplayPageSizeProperty =
        AvaloniaProperty.Register<AbstractSelect, int>(nameof (DisplayPageSize), 10);

    public static readonly StyledProperty<int> MaxCountProperty =
        AvaloniaProperty.Register<AbstractSelect, int>(nameof(MaxCount), int.MaxValue);

    public static readonly StyledProperty<bool> IsShowMaxCountIndicatorProperty =
        AvaloniaProperty.Register<AbstractSelect, bool>(nameof(IsShowMaxCountIndicator));

    public static readonly StyledProperty<int?> MaxTagCountProperty =
        AvaloniaProperty.Register<AbstractSelect, int?>(nameof(MaxTagCount));

    public static readonly StyledProperty<bool> IsResponsiveTagModeProperty =
        AvaloniaProperty.Register<AbstractSelect, bool>(nameof(IsResponsiveTagMode));

    public static readonly StyledProperty<string?> MaxTagPlaceholderProperty =
        AvaloniaProperty.Register<AbstractSelect, string?>(nameof(MaxTagPlaceholder));

    public static readonly StyledProperty<object?> LeftAddOnProperty =
        AddOnDecoratedBox.LeftAddOnProperty.AddOwner<AbstractSelect>();

    public static readonly StyledProperty<IDataTemplate?> LeftAddOnTemplateProperty =
        AddOnDecoratedBox.LeftAddOnTemplateProperty.AddOwner<AbstractSelect>();

    public static readonly StyledProperty<object?> RightAddOnProperty =
        AddOnDecoratedBox.RightAddOnProperty.AddOwner<AbstractSelect>();

    public static readonly StyledProperty<IDataTemplate?> RightAddOnTemplateProperty =
        AddOnDecoratedBox.RightAddOnTemplateProperty.AddOwner<AbstractSelect>();

    public static readonly StyledProperty<object?> ContentLeftAddOnProperty =
        AddOnDecoratedBox.ContentLeftAddOnProperty.AddOwner<AbstractSelect>();

    public static readonly StyledProperty<IDataTemplate?> ContentLeftAddOnTemplateProperty =
        AddOnDecoratedBox.ContentLeftAddOnTemplateProperty.AddOwner<AbstractSelect>();

    public static readonly StyledProperty<object?> ContentRightAddOnProperty =
        AddOnDecoratedBox.ContentRightAddOnProperty.AddOwner<AbstractSelect>();

    public static readonly StyledProperty<IDataTemplate?> ContentRightAddOnTemplateProperty =
        AddOnDecoratedBox.ContentRightAddOnTemplateProperty.AddOwner<AbstractSelect>();

    public static readonly StyledProperty<InputControlStyleVariant> StyleVariantProperty =
        InputControlStyleVariantProperty.StyleVariantProperty.AddOwner<AbstractSelect>();

    public static readonly StyledProperty<InputControlStatus> StatusProperty =
        InputControlStatusProperty.StatusProperty.AddOwner<AbstractSelect>();

    public static readonly StyledProperty<SelectPopupPlacement> PlacementProperty =
        AvaloniaProperty.Register<AbstractSelect, SelectPopupPlacement>(nameof(Placement), SelectPopupPlacement.BottomEdgeAlignedLeft);

    public static readonly StyledProperty<object?> FilterValueProperty =
        AvaloniaProperty.Register<AbstractSelect, object?>(nameof(FilterValue));

    public static readonly StyledProperty<PathIcon?> ClearIconProperty =
        AvaloniaProperty.Register<AbstractSelect, PathIcon?>(nameof(ClearIcon));

    public static readonly StyledProperty<PathIcon?> SuffixIconProperty =
        AvaloniaProperty.Register<AbstractSelect, PathIcon?>(nameof(SuffixIcon));

    public static readonly StyledProperty<PathIcon?> SuffixLoadingIconProperty =
        AvaloniaProperty.Register<AbstractSelect, PathIcon?>(nameof(SuffixLoadingIcon));

    public static readonly StyledProperty<bool> IsMotionEnabledProperty =
        MotionAwareControlProperty.IsMotionEnabledProperty.AddOwner<AbstractSelect>();

    public static readonly StyledProperty<SizeType> SizeTypeProperty =
        SizeTypeControlProperty.SizeTypeProperty.AddOwner<AbstractSelect>();

    public static readonly StyledProperty<object?> EmptyIndicatorProperty =
        AvaloniaProperty.Register<AbstractSelect, object?>(nameof(EmptyIndicator));

    public static readonly StyledProperty<IDataTemplate?> EmptyIndicatorTemplateProperty =
        AvaloniaProperty.Register<AbstractSelect, IDataTemplate?>(nameof(EmptyIndicatorTemplate));

    public static readonly StyledProperty<bool> IsShowEmptyIndicatorProperty =
        AvaloniaProperty.Register<AbstractSelect, bool>(nameof(IsShowEmptyIndicator), true);

    public static readonly StyledProperty<Thickness> EmptyIndicatorPaddingProperty =
        AvaloniaProperty.Register<AbstractSelect, Thickness>(nameof(EmptyIndicatorPadding));

    public static readonly DirectProperty<AbstractSelect, bool> IsLoadingProperty =
        AvaloniaProperty.RegisterDirect<AbstractSelect, bool>(
            nameof(IsLoading),
            o => o.IsLoading);

    public bool IsAllowClear
    {
        get => GetValue(IsAllowClearProperty);
        set => SetValue(IsAllowClearProperty, value);
    }

    public bool IsAutoClearSearchValue
    {
        get => GetValue(IsAutoClearSearchValueProperty);
        set => SetValue(IsAutoClearSearchValueProperty, value);
    }

    public bool IsDefaultOpen
    {
        get => GetValue(IsDefaultOpenProperty);
        set => SetValue(IsDefaultOpenProperty, value);
    }

    public bool IsDropDownOpen
    {
        get => GetValue(IsDropDownOpenProperty);
        set => SetValue(IsDropDownOpenProperty, value);
    }

    public string? PlaceholderText
    {
        get => GetValue(PlaceholderTextProperty);
        set => SetValue(PlaceholderTextProperty, value);
    }

    public IBrush? PlaceholderForeground
    {
        get => GetValue(PlaceholderForegroundProperty);
        set => SetValue(PlaceholderForegroundProperty, value);
    }

    public bool IsPopupMatchSelectWidth
    {
        get => GetValue(IsPopupMatchSelectWidthProperty);
        set => SetValue(IsPopupMatchSelectWidthProperty, value);
    }

    public bool ShouldUseOverlayPopup
    {
        get => GetValue(ShouldUseOverlayPopupProperty);
        set => SetValue(ShouldUseOverlayPopupProperty, value);
    }

    public bool IsFilterEnabled
    {
        get => GetValue(IsFilterEnabledProperty);
        set => SetValue(IsFilterEnabledProperty, value);
    }

    public int DisplayPageSize
    {
        get => GetValue(DisplayPageSizeProperty);
        set => SetValue(DisplayPageSizeProperty, value);
    }

    public int MaxCount
    {
        get => GetValue(MaxCountProperty);
        set => SetValue(MaxCountProperty, value);
    }

    public bool IsShowMaxCountIndicator
    {
        get => GetValue(IsShowMaxCountIndicatorProperty);
        set => SetValue(IsShowMaxCountIndicatorProperty, value);
    }

    /// <summary>
    /// 响应式最大显示的 Tags 数量
    /// </summary>
    public int? MaxTagCount
    {
        get => GetValue(MaxTagCountProperty);
        set => SetValue(MaxTagCountProperty, value);
    }

    public bool IsResponsiveTagMode
    {
        get => GetValue(IsResponsiveTagModeProperty);
        set => SetValue(IsResponsiveTagModeProperty, value);
    }

    /// <summary>
    /// 在响应式情况下，Tags 因为宽度不够被隐藏之后显示的内容
    /// </summary>
    public string? MaxTagPlaceholder
    {
        get => GetValue(MaxTagPlaceholderProperty);
        set => SetValue(MaxTagPlaceholderProperty, value);
    }

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

    public SelectPopupPlacement Placement
    {
        get => GetValue(PlacementProperty);
        set => SetValue(PlacementProperty, value);
    }

    public object? FilterValue
    {
        get => GetValue(FilterValueProperty);
        set => SetValue(FilterValueProperty, value);
    }

    public PathIcon? ClearIcon
    {
        get => GetValue(ClearIconProperty);
        set => SetValue(ClearIconProperty, value);
    }

    public PathIcon? SuffixIcon
    {
        get => GetValue(SuffixIconProperty);
        set => SetValue(SuffixIconProperty, value);
    }

    public PathIcon? SuffixLoadingIcon
    {
        get => GetValue(SuffixLoadingIconProperty);
        set => SetValue(SuffixLoadingIconProperty, value);
    }

    public bool IsMotionEnabled
    {
        get => GetValue(IsMotionEnabledProperty);
        set => SetValue(IsMotionEnabledProperty, value);
    }

    public SizeType SizeType
    {
        get => GetValue(SizeTypeProperty);
        set => SetValue(SizeTypeProperty, value);
    }

    [DependsOn(nameof(EmptyIndicatorTemplate))]
    public object? EmptyIndicator
    {
        get => GetValue(EmptyIndicatorProperty);
        set => SetValue(EmptyIndicatorProperty, value);
    }

    public IDataTemplate? EmptyIndicatorTemplate
    {
        get => GetValue(EmptyIndicatorTemplateProperty);
        set => SetValue(EmptyIndicatorTemplateProperty, value);
    }

    public bool IsShowEmptyIndicator
    {
        get => GetValue(IsShowEmptyIndicatorProperty);
        set => SetValue(IsShowEmptyIndicatorProperty, value);
    }

    public Thickness EmptyIndicatorPadding
    {
        get => GetValue(EmptyIndicatorPaddingProperty);
        set => SetValue(EmptyIndicatorPaddingProperty, value);
    }

    private bool _isLoading;

    public bool IsLoading
    {
        get => _isLoading;
        protected set => SetAndRaise(IsLoadingProperty, ref _isLoading, value);
    }
    #endregion

    #region 公共事件定义

    public event EventHandler<CancelEventArgs>? DropDownOpening;
    public event EventHandler? DropDownOpened;
    public event EventHandler<CancelEventArgs>? DropDownClosing;
    public event EventHandler? DropDownClosed;

    #endregion

    #region 内部属性定义

    internal static readonly DirectProperty<AbstractSelect, double> ItemHeightProperty =
        AvaloniaProperty.RegisterDirect<AbstractSelect, double>(
            nameof(ItemHeight),
            o => o.ItemHeight,
            (o, v) => o.ItemHeight = v);

    internal static readonly DirectProperty<AbstractSelect, double> MaxPopupHeightProperty =
        AvaloniaProperty.RegisterDirect<AbstractSelect, double>(
            nameof(MaxPopupHeight),
            o => o.MaxPopupHeight,
            (o, v) => o.MaxPopupHeight = v);

    internal static readonly DirectProperty<AbstractSelect, Thickness> PopupContentPaddingProperty =
        AvaloniaProperty.RegisterDirect<AbstractSelect, Thickness>(nameof(PopupContentPadding),
            o => o.PopupContentPadding,
            (o, v) => o.PopupContentPadding = v);

    internal static readonly DirectProperty<AbstractSelect, bool> IsEffectiveShowClearButtonProperty =
        AvaloniaProperty.RegisterDirect<AbstractSelect, bool>(nameof(IsEffectiveShowClearButton),
            o => o.IsEffectiveShowClearButton,
            (o, v) => o.IsEffectiveShowClearButton = v);

    internal static readonly DirectProperty<AbstractSelect, double> EffectivePopupWidthProperty =
        AvaloniaProperty.RegisterDirect<AbstractSelect, double>(
            nameof(EffectivePopupWidth),
            o => o.EffectivePopupWidth,
            (o, v) => o.EffectivePopupWidth = v);

    internal static readonly DirectProperty<AbstractSelect, bool> IsPlaceholderTextVisibleProperty =
        AvaloniaProperty.RegisterDirect<AbstractSelect, bool>(
            nameof(IsPlaceholderTextVisible),
            o => o.IsPlaceholderTextVisible,
            (o, v) => o.IsPlaceholderTextVisible = v);

    internal static readonly DirectProperty<AbstractSelect, bool> IsSelectionEmptyProperty =
        AvaloniaProperty.RegisterDirect<AbstractSelect, bool>(
            nameof(IsSelectionEmpty),
            o => o.IsSelectionEmpty,
            (o, v) => o.IsSelectionEmpty = v);

    internal static readonly DirectProperty<AbstractSelect, int> SelectedCountProperty =
        AvaloniaProperty.RegisterDirect<AbstractSelect, int>(nameof(SelectedCount),
            o => o.SelectedCount,
            (o, v) => o.SelectedCount = v);

    internal static readonly DirectProperty<AbstractSelect, PlacementMode> PopupPlacementProperty =
        AvaloniaProperty.RegisterDirect<AbstractSelect, PlacementMode>(
            nameof(PopupPlacement),
            o => o.PopupPlacement,
            (o, v) => o.PopupPlacement = v);

    internal static readonly StyledProperty<SpaceItemPosition?> CompactSpaceItemPositionProperty =
        CompactSpaceAwareControlProperty.CompactSpaceItemPositionProperty.AddOwner<AbstractSelect>();

    internal static readonly StyledProperty<Orientation> CompactSpaceOrientationProperty =
        CompactSpaceAwareControlProperty.CompactSpaceOrientationProperty.AddOwner<AbstractSelect>();

    internal static readonly StyledProperty<bool> IsUsedInCompactSpaceProperty =
        CompactSpaceAwareControlProperty.IsUsedInCompactSpaceProperty.AddOwner<AbstractSelect>();

    internal static readonly StyledProperty<FormValidateFeedback?> FormFeedbackProperty =
        AvaloniaProperty.Register<AbstractSelect, FormValidateFeedback?>(nameof(FormFeedback));

    private double _itemHeight;

    internal double ItemHeight
    {
        get => _itemHeight;
        set => SetAndRaise(ItemHeightProperty, ref _itemHeight, value);
    }

    private double _maxPopupHeight;

    internal double MaxPopupHeight
    {
        get => _maxPopupHeight;
        set => SetAndRaise(MaxPopupHeightProperty, ref _maxPopupHeight, value);
    }

    private Thickness _popupContentPadding;

    internal Thickness PopupContentPadding
    {
        get => _popupContentPadding;
        set => SetAndRaise(PopupContentPaddingProperty, ref _popupContentPadding, value);
    }

    private bool _isEffectiveShowClearButton;

    internal bool IsEffectiveShowClearButton
    {
        get => _isEffectiveShowClearButton;
        set => SetAndRaise(IsEffectiveShowClearButtonProperty, ref _isEffectiveShowClearButton, value);
    }

    private double _effectivePopupWidth;

    internal double EffectivePopupWidth
    {
        get => _effectivePopupWidth;
        set => SetAndRaise(EffectivePopupWidthProperty, ref _effectivePopupWidth, value);
    }

    private bool _isPlaceholderTextVisible;

    internal bool IsPlaceholderTextVisible
    {
        get => _isPlaceholderTextVisible;
        set => SetAndRaise(IsPlaceholderTextVisibleProperty, ref _isPlaceholderTextVisible, value);
    }

    private bool _isSelectionEmpty = true;

    internal bool IsSelectionEmpty
    {
        get => _isSelectionEmpty;
        set => SetAndRaise(IsSelectionEmptyProperty, ref _isSelectionEmpty, value);
    }

    private int _selectedCount;

    internal int SelectedCount
    {
        get => _selectedCount;
        set => SetAndRaise(SelectedCountProperty, ref _selectedCount, value);
    }

    private PlacementMode _popupPlacement = PlacementMode.BottomEdgeAlignedLeft;

    internal PlacementMode PopupPlacement
    {
        get => _popupPlacement;
        set => SetAndRaise(PopupPlacementProperty, ref _popupPlacement, value);
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
    #endregion

    private protected readonly CompositeDisposable SubscriptionsOnOpen = new ();
    private protected Popup? Popup;
    private protected bool PopupHasOpened;
    private protected bool IgnorePropertyChange;
    private protected Border? PopupFrame;
    private AddOnDecoratedBox? _addOnDecoratedBox;
    private SelectAccessoryHost? _accessoryHost;
    private IDisposable? _accessoryHostSpacingBinding;
    private SelectHandle? _lightweightSelectHandle;
    private CompositeDisposable? _lightweightHandleInputSubscriptions;
    private CompositeDisposable? _contentRightAddOnBindings;
    private bool _isUsingLegacyAccessoryTemplate;

    private Window? _attachedWindow;

    static AbstractSelect()
    {
        AffectsArrange<AbstractSelect>(CompactSpaceItemPositionProperty, CompactSpaceOrientationProperty);
        IsDropDownOpenProperty.Changed.AddClassHandler<AbstractSelect>((select, args) => select.HandleIsDropDownOpenChanged(args));
    }

    private void HandleIsDropDownOpenChanged(AvaloniaPropertyChangedEventArgs e)
    {
        // Ignore the change if requested
        if (IgnorePropertyChange)
        {
            IgnorePropertyChange = false;
            return;
        }

        bool oldValue = (bool)e.OldValue!;
        bool newValue = (bool)e.NewValue!;

        if (!newValue)
        {
            ClosingDropDown(oldValue);
        }
        else
        {
            OpeningDropDown(oldValue);
        }

        UpdatePseudoClasses();
    }

    protected override void OnInitialized()
    {
        base.OnInitialized();
        ConfigurePopupPlacement();
        if (SuffixIcon == null)
        {
            SetCurrentValue(SuffixIconProperty, new DownOutlined());
        }
    }

    protected virtual void NotifyPopupClosed()
    {
        DropDownClosed?.Invoke(this, EventArgs.Empty);
    }

    protected virtual void NotifyPopupOpened()
    {
        DropDownOpened?.Invoke(this, EventArgs.Empty);
    }

    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        base.OnPropertyChanged(change);
        if (change.Property == PlacementProperty)
        {
            ConfigurePopupPlacement();
        }
        else if (change.Property == DisplayPageSizeProperty ||
                 change.Property == ItemHeightProperty ||
                 change.Property == PopupContentPaddingProperty)
        {
            ConfigureMaxDropdownHeight();
        }
        else if (change.Property == EffectivePopupWidthProperty ||
                 change.Property == MaxPopupHeightProperty)
        {
            ConfigurePopupFrame();
        }

        if (change.Property == IsDropDownOpenProperty)
        {
            ConfigureWindowDeactivatedSubscription();
        }

        if (change.Property == IsShowMaxCountIndicatorProperty ||
            change.Property == MaxCountProperty ||
            change.Property == SelectedCountProperty ||
            change.Property == ContentRightAddOnProperty ||
            change.Property == ContentRightAddOnTemplateProperty ||
            change.Property == FormFeedbackProperty ||
            change.Property == SuffixLoadingIconProperty ||
            change.Property == SuffixIconProperty ||
            change.Property == IsFilterEnabledProperty ||
            change.Property == IsMotionEnabledProperty ||
            change.Property == IsLoadingProperty ||
            change.Property == IsAllowClearProperty ||
            change.Property == IsSelectionEmptyProperty ||
            change.Property == IsDropDownOpenProperty ||
            change.Property == InputElement.IsEnabledProperty)
        {
            UpdateOwnerDrivenAccessoryState();
        }
    }

    protected override void OnAttachedToVisualTree(VisualTreeAttachmentEventArgs e)
    {
        base.OnAttachedToVisualTree(e);
        ConfigureWindowDeactivatedSubscription();
    }

    protected override void OnDetachedFromVisualTree(VisualTreeAttachmentEventArgs e)
    {
        base.OnDetachedFromVisualTree(e);

        ClearWindowDeactivatedSubscription();
        ClearPopupContent();
        ClearPopupFrame();
    }

    private void HandleWindowDeactivated(object? sender, EventArgs e)
    {
        SetCurrentValue(IsDropDownOpenProperty, false);
    }

    protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
    {
        _contentRightAddOnBindings?.Dispose();
        _contentRightAddOnBindings = null;
        ClearOwnerDrivenAccessoryHost();
        ClearPopupContent();
        ClearPopupFrame();

        base.OnApplyTemplate(e);
        ConfigureMaxDropdownHeight();
        if (Popup != null)
        {
            Popup.Opened -= PopupOpened;
            Popup.Closed -= PopupClosed;
        }

        Popup = e.NameScope.Find<Popup>("PART_Popup");

        if (Popup != null)
        {
            Popup.Opened += PopupOpened;
            Popup.Closed += PopupClosed;
        }
        _addOnDecoratedBox = e.NameScope.Find<AddOnDecoratedBox>(AddOnDecoratedBox.AddOnDecoratedBoxPart);
        ConfigureContentRightAddOn(e);
        ConfigureWindowDeactivatedSubscription();
    }

    private void ConfigureContentRightAddOn(TemplateAppliedEventArgs e)
    {
        _isUsingLegacyAccessoryTemplate = SetupContentRightAddOnBindings(e);
        if (!_isUsingLegacyAccessoryTemplate)
        {
            ConfigureOwnerDrivenAccessoryHost();
        }
    }

    private bool SetupContentRightAddOnBindings(TemplateAppliedEventArgs e)
    {
        _contentRightAddOnBindings?.Dispose();
        _contentRightAddOnBindings = null;
        var bindings    = new CompositeDisposable();
        var hasBindings = false;

        if (e.NameScope.Find<SelectMaxCountIndicator>("PART_SelectMaxCountIndicator") is { } indicator)
        {
            hasBindings = true;
            bindings.Add(indicator.Bind(SelectMaxCountIndicator.MaxCountProperty,
                new Binding(nameof(MaxCount)) { Source = this }));
            bindings.Add(indicator.Bind(SelectMaxCountIndicator.SelectedCountProperty,
                new Binding(nameof(SelectedCount)) { Source = this }));
            bindings.Add(indicator.Bind(Visual.IsVisibleProperty,
                new Binding(nameof(IsShowMaxCountIndicator)) { Source = this }));
        }

        if (e.NameScope.Find<ContentPresenter>("PART_ContentRightAddOnPresenter") is { } contentPresenter)
        {
            hasBindings = true;
            bindings.Add(contentPresenter.Bind(ContentPresenter.ContentProperty,
                new Binding(nameof(ContentRightAddOn)) { Source = this }));
            bindings.Add(contentPresenter.Bind(ContentPresenter.ContentTemplateProperty,
                new Binding(nameof(ContentRightAddOnTemplate)) { Source = this }));
            bindings.Add(contentPresenter.Bind(Visual.IsVisibleProperty,
                new Binding(nameof(ContentRightAddOn)) { Source = this, Converter = ObjectConverters.IsNotNull }));
        }

        if (e.NameScope.Find<SelectHandle>("PART_SelectHandle") is { } handle)
        {
            hasBindings = true;
            bindings.Add(handle.Bind(SelectHandle.FormFeedbackProperty,
                new Binding(nameof(FormFeedback)) { Source = this }));
            bindings.Add(handle.Bind(SelectHandle.LoadingIconProperty,
                new Binding(nameof(SuffixLoadingIcon)) { Source = this }));
            bindings.Add(handle.Bind(SelectHandle.OpenIndicatorProperty,
                new Binding(nameof(SuffixIcon)) { Source = this }));
            bindings.Add(handle.Bind(SelectHandle.IsFilterEnabledProperty,
                new Binding(GetFilterEnabledBindingPath()) { Source = this }));
            bindings.Add(handle.Bind(InputElement.IsEnabledProperty,
                new Binding(nameof(IsEnabled)) { Source = this }));
            bindings.Add(handle.Bind(SelectHandle.IsMotionEnabledProperty,
                new Binding(nameof(IsMotionEnabled)) { Source = this }));
            bindings.Add(handle.Bind(SelectHandle.IsLoadingProperty,
                new Binding(nameof(IsLoading)) { Source = this }));
            bindings.Add(handle.Bind(SelectHandle.IsAllowClearProperty,
                new Binding(nameof(IsAllowClear)) { Source = this }));
            bindings.Add(handle.Bind(SelectHandle.IsSelectionEmptyProperty,
                new Binding(nameof(IsSelectionEmpty)) { Source = this }));
            bindings.Add(handle.Bind(SelectHandle.IsDropDownOpenProperty,
                new Binding(nameof(IsDropDownOpen)) { Source = this }));

            if (_addOnDecoratedBox != null)
            {
                bindings.Add(handle.Bind(SelectHandle.IsInputHoverProperty,
                    new Binding(nameof(AddOnDecoratedBox.IsInnerBoxHover)) { Source = _addOnDecoratedBox }));
                bindings.Add(handle.Bind(SelectHandle.IsInputPressedProperty,
                    new Binding(nameof(AddOnDecoratedBox.IsInnerBoxPressed)) { Source = _addOnDecoratedBox }));
            }
        }

        if (hasBindings)
        {
            _contentRightAddOnBindings = bindings;
            return true;
        }

        bindings.Dispose();
        return _addOnDecoratedBox?.ContentRightAddOn != null;
    }

    private string GetFilterEnabledBindingPath()
    {
        return this is Select ? nameof(Select.IsEffectiveFilterEnabled) : nameof(IsFilterEnabled);
    }

    private bool GetEffectiveFilterEnabled()
    {
        return this is Select select ? select.IsEffectiveFilterEnabled : IsFilterEnabled;
    }

    private void ConfigureOwnerDrivenAccessoryHost()
    {
        if (_addOnDecoratedBox == null ||
            _isUsingLegacyAccessoryTemplate)
        {
            return;
        }

        if (NeedsCompositeAccessoryHost())
        {
            ClearLightweightSelectHandle();

            if (_accessoryHost == null)
            {
                _accessoryHost = new SelectAccessoryHost();
                _accessoryHostSpacingBinding = TokenResourceBinder.CreateTokenBinding(
                    _accessoryHost,
                    StackPanel.SpacingProperty,
                    SharedTokenKind.SpacingXS);
                _accessoryHost.AttachOwner(this, _addOnDecoratedBox);
            }

            if (!ReferenceEquals(_addOnDecoratedBox.ContentRightAddOn, _accessoryHost))
            {
                _addOnDecoratedBox.SetCurrentValue(AddOnDecoratedBox.ContentRightAddOnProperty, _accessoryHost);
            }
            return;
        }

        ClearCompositeAccessoryHost();
        if (_lightweightSelectHandle == null)
        {
            _lightweightSelectHandle = new SelectHandle();
            _lightweightSelectHandle.SetTemplatedParent(this);
        }
        UpdateLightweightSelectHandleState();

        if (!ReferenceEquals(_addOnDecoratedBox.ContentRightAddOn, _lightweightSelectHandle))
        {
            _addOnDecoratedBox.SetCurrentValue(AddOnDecoratedBox.ContentRightAddOnProperty, _lightweightSelectHandle);
        }
    }

    private void ClearOwnerDrivenAccessoryHost()
    {
        ClearLightweightSelectHandle();
        ClearCompositeAccessoryHost();
    }

    private void ClearCompositeAccessoryHost()
    {
        if (_addOnDecoratedBox != null &&
            _accessoryHost != null &&
            ReferenceEquals(_addOnDecoratedBox.ContentRightAddOn, _accessoryHost))
        {
            _addOnDecoratedBox.ClearValue(AddOnDecoratedBox.ContentRightAddOnProperty);
        }

        _accessoryHostSpacingBinding?.Dispose();
        _accessoryHostSpacingBinding = null;

        if (_accessoryHost != null)
        {
            _accessoryHost.DetachOwner();
            _accessoryHost = null;
        }
    }

    private bool NeedsCompositeAccessoryHost()
    {
        return IsShowMaxCountIndicator || ContentRightAddOn != null;
    }

    private void ClearLightweightSelectHandle()
    {
        _lightweightHandleInputSubscriptions?.Dispose();
        _lightweightHandleInputSubscriptions = null;

        if (_addOnDecoratedBox != null &&
            _lightweightSelectHandle != null &&
            ReferenceEquals(_addOnDecoratedBox.ContentRightAddOn, _lightweightSelectHandle))
        {
            _addOnDecoratedBox.ClearValue(AddOnDecoratedBox.ContentRightAddOnProperty);
        }

        if (_lightweightSelectHandle != null)
        {
            _lightweightSelectHandle.ClearValue(SelectHandle.FormFeedbackProperty);
            _lightweightSelectHandle.ClearValue(SelectHandle.LoadingIconProperty);
            _lightweightSelectHandle.ClearValue(SelectHandle.OpenIndicatorProperty);
            _lightweightSelectHandle.SetTemplatedParent(null);
            _lightweightSelectHandle = null;
        }
    }

    private protected void UpdateOwnerDrivenAccessoryState()
    {
        if (_isUsingLegacyAccessoryTemplate)
        {
            return;
        }

        if (_accessoryHost != null && NeedsCompositeAccessoryHost())
        {
            _accessoryHost.AttachOwner(this, _addOnDecoratedBox);
        }

        ConfigureOwnerDrivenAccessoryHost();
        UpdateLightweightSelectHandleState();
    }

    private void UpdateLightweightSelectHandleState()
    {
        if (_lightweightSelectHandle == null)
        {
            return;
        }

        _lightweightSelectHandle.SetCurrentValue(SelectHandle.FormFeedbackProperty, FormFeedback);
        _lightweightSelectHandle.SetCurrentValue(SelectHandle.LoadingIconProperty, SuffixLoadingIcon);
        _lightweightSelectHandle.SetCurrentValue(SelectHandle.OpenIndicatorProperty, SuffixIcon);
        _lightweightSelectHandle.SetCurrentValue(SelectHandle.IsFilterEnabledProperty, GetEffectiveFilterEnabled());
        _lightweightSelectHandle.SetCurrentValue(InputElement.IsEnabledProperty, IsEnabled);
        _lightweightSelectHandle.SetCurrentValue(SelectHandle.IsMotionEnabledProperty, IsMotionEnabled);
        _lightweightSelectHandle.SetCurrentValue(SelectHandle.IsLoadingProperty, IsLoading);
        _lightweightSelectHandle.SetCurrentValue(SelectHandle.IsAllowClearProperty, IsAllowClear);
        _lightweightSelectHandle.SetCurrentValue(SelectHandle.IsSelectionEmptyProperty, IsSelectionEmpty);
        _lightweightSelectHandle.SetCurrentValue(SelectHandle.IsDropDownOpenProperty, IsDropDownOpen);
        ConfigureLightweightHandleInputSubscriptions();
        UpdateLightweightHandleInputState();
    }

    private void ConfigureLightweightHandleInputSubscriptions()
    {
        var shouldTrackInputState = _addOnDecoratedBox != null && IsAllowClear && !IsSelectionEmpty;
        if (!shouldTrackInputState)
        {
            _lightweightHandleInputSubscriptions?.Dispose();
            _lightweightHandleInputSubscriptions = null;
            return;
        }

        if (_lightweightHandleInputSubscriptions != null || _addOnDecoratedBox == null)
        {
            return;
        }

        _lightweightHandleInputSubscriptions = new CompositeDisposable
        {
            _addOnDecoratedBox.GetObservable(AddOnDecoratedBox.IsInnerBoxHoverProperty)
                              .Subscribe(_ => UpdateLightweightHandleInputState()),
            _addOnDecoratedBox.GetObservable(AddOnDecoratedBox.IsInnerBoxPressedProperty)
                              .Subscribe(_ => UpdateLightweightHandleInputState())
        };
    }

    private void UpdateLightweightHandleInputState()
    {
        if (_lightweightSelectHandle == null)
        {
            return;
        }

        _lightweightSelectHandle.SetCurrentValue(SelectHandle.IsInputHoverProperty,
            _addOnDecoratedBox?.IsInnerBoxHover ?? false);
        _lightweightSelectHandle.SetCurrentValue(SelectHandle.IsInputPressedProperty,
            _addOnDecoratedBox?.IsInnerBoxPressed ?? false);
    }

    protected virtual void PopupClosed(object? sender, EventArgs e)
    {
        SubscriptionsOnOpen.Clear();
        ClearWindowDeactivatedSubscription();
        NotifyPopupClosed();
    }

    protected virtual void PopupOpened(object? sender, EventArgs e)
    {
        SubscriptionsOnOpen.Clear();
        this.GetObservable(IsVisibleProperty).Subscribe(IsVisibleChanged).DisposeWith(SubscriptionsOnOpen);
        this.SubscribeAncestorIsVisible(IsVisibleChanged, SubscriptionsOnOpen);

        ConfigurePopupMinWith(Bounds.Width);
        NotifyPopupOpened();
    }

    private void ConfigureWindowDeactivatedSubscription()
    {
        if (!IsDropDownOpen)
        {
            ClearWindowDeactivatedSubscription();
            return;
        }

        var topLevel = TopLevel.GetTopLevel(this);
        if (ReferenceEquals(_attachedWindow, topLevel))
        {
            return;
        }

        ClearWindowDeactivatedSubscription();
        if (topLevel is Window window)
        {
            _attachedWindow    =  window;
            window.Deactivated += HandleWindowDeactivated;
        }
    }

    private void ClearWindowDeactivatedSubscription()
    {
        if (_attachedWindow != null)
        {
            _attachedWindow.Deactivated -= HandleWindowDeactivated;
            _attachedWindow = null;
        }
    }

    private void IsVisibleChanged(bool isVisible)
    {
        if (!isVisible && IsDropDownOpen)
        {
            SetCurrentValue(IsDropDownOpenProperty, false);
        }
    }

    private void ConfigurePopupPlacement()
    {
        if (Placement == SelectPopupPlacement.BottomEdgeAlignedLeft)
        {
            PopupPlacement = PlacementMode.BottomEdgeAlignedLeft;
        }
        else if (Placement == SelectPopupPlacement.BottomEdgeAlignedRight)
        {
            PopupPlacement = PlacementMode.BottomEdgeAlignedRight;
        }
        else if (Placement == SelectPopupPlacement.TopEdgeAlignedLeft)
        {
            PopupPlacement = PlacementMode.TopEdgeAlignedLeft;
        }
        else if (Placement == SelectPopupPlacement.TopEdgeAlignedRight)
        {
            PopupPlacement = PlacementMode.TopEdgeAlignedRight;
        }
    }

    protected virtual void ConfigurePopupMinWith(double selectWidth)
    {
        if (IsPopupMatchSelectWidth)
        {
            SetCurrentValue(EffectivePopupWidthProperty, selectWidth);
        }
        else
        {
            SetCurrentValue(EffectivePopupWidthProperty, 0.0);
        }
    }

    protected virtual void ConfigureMaxDropdownHeight()
    {
        MaxPopupHeight = ItemHeight * DisplayPageSize + PopupContentPadding.Top + PopupContentPadding.Bottom;
    }

    protected void ClosingDropDown(bool oldValue)
    {
        var args = new CancelEventArgs();
        NotifyDropDownClosing(args);

        if (args.Cancel)
        {
            IgnorePropertyChange = true;
            SetCurrentValue(IsDropDownOpenProperty, oldValue);
        }
        else
        {
            CloseDropDown();
        }

        UpdatePseudoClasses();
    }

    protected void CloseDropDown()
    {
        if (PopupHasOpened)
        {
            if (Popup != null)
            {
                Popup.IsOpen = false;
            }
            NotifyDropDownClosed(EventArgs.Empty);
        }
    }

    protected void OpenDropDown()
    {
        EnsurePopupContent();
        ConfigureWindowDeactivatedSubscription();
        if (Popup != null)
        {
            Popup.IsOpen = true;
        }
        PopupHasOpened = true;
        NotifyDropDownOpened(EventArgs.Empty);
    }

    protected void OpeningDropDown(bool oldValue)
    {
        var args = new CancelEventArgs();

        // Opening
        NotifyDropDownOpening(args);

        if (args.Cancel)
        {
            IgnorePropertyChange = true;
            SetCurrentValue(IsDropDownOpenProperty, oldValue);
        }
        else
        {
            OpenDropDown();
        }

        UpdatePseudoClasses();
    }

    protected virtual void NotifyDropDownOpening(CancelEventArgs eventArgs)
    {
        DropDownOpening?.Invoke(this, eventArgs);
    }

    protected virtual void NotifyDropDownOpened(EventArgs eventArgs)
    {
        DropDownOpened?.Invoke(this, eventArgs);
    }

    protected virtual void NotifyDropDownClosing(CancelEventArgs eventArgs)
    {
        DropDownClosing?.Invoke(this, eventArgs);
    }

    protected virtual void NotifyDropDownClosed(EventArgs eventArgs)
    {
        DropDownClosed?.Invoke(this, eventArgs);
    }

    protected void UpdatePseudoClasses()
    {
        PseudoClasses.Set(SelectPseudoClass.DropdownOpen, IsDropDownOpen);
        PseudoClasses.Set(StdPseudoClass.Error, Status == InputControlStatus.Error);
        PseudoClasses.Set(StdPseudoClass.Warning, Status == InputControlStatus.Warning);
        PseudoClasses.Set(AddOnDecoratedBoxPseudoClass.Outline, StyleVariant == InputControlStyleVariant.Outlined);
        PseudoClasses.Set(AddOnDecoratedBoxPseudoClass.Filled, StyleVariant == InputControlStyleVariant.Filled);
        PseudoClasses.Set(AddOnDecoratedBoxPseudoClass.Borderless, StyleVariant == InputControlStyleVariant.Borderless);
    }

    private protected virtual void EnsurePopupContent()
    {
    }

    private protected Border EnsurePopupFrame(Control child)
    {
        if (PopupFrame == null)
        {
            PopupFrame = new Border
            {
                Name  = "PopupFrame",
                Child = child
            };
            PopupFrame.SetTemplatedParent(this);
            Popup?.SetCurrentValue(Avalonia.Controls.Primitives.Popup.ChildProperty, PopupFrame);
        }
        else if (!ReferenceEquals(PopupFrame.Child, child))
        {
            PopupFrame.Child = child;
        }

        ConfigurePopupFrame();
        return PopupFrame;
    }

    private protected virtual void ClearPopupContent()
    {
    }

    private protected void ClearPopupFrame()
    {
        if (PopupFrame != null)
        {
            PopupFrame.Child = null;
            PopupFrame.SetTemplatedParent(null);
            PopupFrame = null;
        }

        if (Popup != null)
        {
            Popup.SetCurrentValue(Avalonia.Controls.Primitives.Popup.ChildProperty, null);
        }
    }

    private void ConfigurePopupFrame()
    {
        if (PopupFrame == null)
        {
            return;
        }

        PopupFrame.SetCurrentValue(Border.MaxHeightProperty, MaxPopupHeight);
        PopupFrame.SetCurrentValue(Border.MinWidthProperty, EffectivePopupWidth);
        PopupFrame.SetCurrentValue(Border.PaddingProperty, PopupContentPadding);
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

        if (_addOnDecoratedBox == null || _addOnDecoratedBox.StyleVariant == InputControlStyleVariant.Borderless)
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
