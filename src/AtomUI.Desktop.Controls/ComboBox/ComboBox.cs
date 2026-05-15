using System.Reactive.Disposables;
using AtomUI.Controls;
using AtomUI.Data;
using AtomUI.Reflection;
using AtomUI.Theme;
using AtomUI.Theme.Styling;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Presenters;
using Avalonia.Controls.Primitives;
using Avalonia.Controls.Templates;
using Avalonia.Data;
using Avalonia.Data.Converters;
using Avalonia.Input;
using Avalonia.LogicalTree;

namespace AtomUI.Desktop.Controls;

using AvaloniaComboBox = Avalonia.Controls.ComboBox;

public class ComboBox : AvaloniaComboBox,
                        IMotionAwareControl,
                        ISizeTypeAware,
                        IInputControlStatusAware,
                        IInputControlStyleVariantAware,
                        IFormItemAware,
                        IFormItemFeedbackAware
{
    #region 公共属性定义

    public static readonly StyledProperty<object?> LeftAddOnProperty =
        AddOnDecoratedBox.LeftAddOnProperty.AddOwner<ComboBox>();
    
    public static readonly StyledProperty<IDataTemplate?> LeftAddOnTemplateProperty =
        AddOnDecoratedBox.LeftAddOnTemplateProperty.AddOwner<ComboBox>();

    public static readonly StyledProperty<object?> RightAddOnProperty =
        AddOnDecoratedBox.RightAddOnProperty.AddOwner<ComboBox>();
    
    public static readonly StyledProperty<IDataTemplate?> RightAddOnTemplateProperty =
        AddOnDecoratedBox.RightAddOnTemplateProperty.AddOwner<ComboBox>();
    
    public static readonly StyledProperty<object?> ContentLeftAddOnProperty =
        AddOnDecoratedBox.ContentLeftAddOnProperty.AddOwner<ComboBox>();
    
    public static readonly StyledProperty<IDataTemplate?> ContentLeftAddOnTemplateProperty =
        AddOnDecoratedBox.ContentLeftAddOnTemplateProperty.AddOwner<ComboBox>();

    public static readonly StyledProperty<object?> ContentRightAddOnProperty =
        AddOnDecoratedBox.ContentRightAddOnProperty.AddOwner<ComboBox>();
    
    public static readonly StyledProperty<IDataTemplate?> ContentRightAddOnTemplateProperty =
        AddOnDecoratedBox.ContentRightAddOnTemplateProperty.AddOwner<ComboBox>();

    public static readonly StyledProperty<SizeType> SizeTypeProperty =
        SizeTypeControlProperty.SizeTypeProperty.AddOwner<ComboBox>();

    public static readonly StyledProperty<InputControlStyleVariant> StyleVariantProperty =
        InputControlStyleVariantProperty.StyleVariantProperty.AddOwner<ComboBox>();

    public static readonly StyledProperty<InputControlStatus> StatusProperty =
        InputControlStatusProperty.StatusProperty.AddOwner<ComboBox>();

    public static readonly StyledProperty<bool> IsAllowClearProperty =
        TextBox.IsAllowClearProperty.AddOwner<ComboBox>();
    
    public static readonly StyledProperty<double> OptionFontSizeProperty =
        AvaloniaProperty.Register<ComboBox, double>(nameof(OptionFontSize));

    public static readonly StyledProperty<bool> IsMotionEnabledProperty =
        MotionAwareControlProperty.IsMotionEnabledProperty.AddOwner<ComboBox>();
    
    public static readonly StyledProperty<int> DropDownDisplayPageSizeProperty =
        AvaloniaProperty.Register<ComboBox, int>(nameof (DropDownDisplayPageSize), 10);

    public static readonly StyledProperty<bool> ShouldUseOverlayPopupProperty =
        AvaloniaProperty.Register<ComboBox, bool>(nameof(ShouldUseOverlayPopup), true);

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

    public bool IsAllowClear
    {
        get => GetValue(IsAllowClearProperty);
        set => SetValue(IsAllowClearProperty, value);
    }
    
    public double OptionFontSize
    {
        get => GetValue(OptionFontSizeProperty);
        set => SetValue(OptionFontSizeProperty, value);
    }

    public bool IsMotionEnabled
    {
        get => GetValue(IsMotionEnabledProperty);
        set => SetValue(IsMotionEnabledProperty, value);
    }
    
    public int DropDownDisplayPageSize
    {
        get => GetValue(DropDownDisplayPageSizeProperty);
        set => SetValue(DropDownDisplayPageSizeProperty, value);
    }

    public bool ShouldUseOverlayPopup
    {
        get => GetValue(ShouldUseOverlayPopupProperty);
        set => SetValue(ShouldUseOverlayPopupProperty, value);
    }

    #endregion

    #region 内部属性定义
    
    internal static readonly DirectProperty<ComboBox, double> EffectivePopupWidthProperty =
        AvaloniaProperty.RegisterDirect<ComboBox, double>(
            nameof(EffectivePopupWidth),
            o => o.EffectivePopupWidth,
            (o, v) => o.EffectivePopupWidth = v);
    
    internal static readonly StyledProperty<double> ItemHeightProperty =
        AvaloniaProperty.Register<ComboBox, double>(nameof(ItemHeight));
    
    internal static readonly StyledProperty<Thickness> PopupContentPaddingProperty =
        AvaloniaProperty.Register<ComboBox, Thickness>(nameof(PopupContentPadding));
    
    internal static readonly StyledProperty<FormValidateFeedback?> FormFeedbackProperty =
        AvaloniaProperty.Register<ComboBox, FormValidateFeedback?>(nameof(FormFeedback));

    internal static readonly DirectProperty<ComboBox, bool> IsFormFeedbackVisibleProperty =
        AvaloniaProperty.RegisterDirect<ComboBox, bool>(
            nameof(IsFormFeedbackVisible),
            o => o.IsFormFeedbackVisible);
    
    private double _effectivePopupWidth;

    internal double EffectivePopupWidth
    {
        get => _effectivePopupWidth;
        set => SetAndRaise(EffectivePopupWidthProperty, ref _effectivePopupWidth, value);
    }
    
    internal double ItemHeight
    {
        get => GetValue(ItemHeightProperty);
        set => SetValue(ItemHeightProperty, value);
    }
    
    internal Thickness PopupContentPadding
    {
        get => GetValue(PopupContentPaddingProperty);
        set => SetValue(PopupContentPaddingProperty, value);
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

    private Popup? _popup;
    private Border? _popupFrame;
    private ScrollViewer? _popupScrollViewer;
    private ItemsPresenter? _popupItemsPresenter;
    private Avalonia.Controls.Window? _attachedWindow;
    private AddOnDecoratedBox? _addOnDecoratedBox;
    private ComboBoxHandle? _templateComboBoxHandle;
    private ComboBoxHandle? _lightweightComboBoxHandle;
    private ComboBoxAccessoryHost? _accessoryHost;
    private IDisposable? _accessoryHostSpacingBinding;
    private CompositeDisposable? _contentRightAddOnBindings;
    private IDisposable? _feedbackStatusSubscription;
    private bool _isUsingLegacyAccessoryTemplate;

    public ComboBox()
    {
        this.RegisterTokenResourceScope(ComboBoxToken.ScopeProvider);
        SelectionBoxItemProperty.Changed.AddClassHandler<ComboBox>((box, args) => box.NotifyFormValueChanged(args.NewValue));

        // 阻止下拉框打开时的 BringIntoView 行为，防止触发页面滚动
        AddHandler(RequestBringIntoViewEvent, (sender, e) =>
        {
            // 如果是下拉框内的元素触发的 BringIntoView，则取消事件
            if (IsDropDownOpen && e.TargetObject != this)
            {
                e.Handled = true;
            }
        }, handledEventsToo: true);
    }
    
    protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
    {
        _contentRightAddOnBindings?.Dispose();
        _contentRightAddOnBindings = null;
        ClearOwnerDrivenAccessoryHost();
        ClearPopupContent();
        ClearWindowDeactivatedSubscription();

        base.OnApplyTemplate(e);
        this.SetPopup(null); // 清空父类，防止鼠标点击的错误处理

        _popup                  = e.NameScope.Find<Popup>("PART_Popup");
        _addOnDecoratedBox      = e.NameScope.Find<AddOnDecoratedBox>(AddOnDecoratedBox.AddOnDecoratedBoxPart);
        _templateComboBoxHandle = e.NameScope.Find<ComboBoxHandle>("PART_ComboBoxHandle");

        UpdatePseudoClasses();
        ConfigureMaxDropdownHeight();
        _isUsingLegacyAccessoryTemplate = SetupContentRightAddOnBindings(e);
        if (!_isUsingLegacyAccessoryTemplate)
        {
            ConfigureOwnerDrivenAccessoryHost();
        }
        ConfigureWindowDeactivatedSubscription();
    }

    private bool SetupContentRightAddOnBindings(TemplateAppliedEventArgs e)
    {
        _contentRightAddOnBindings?.Dispose();
        _contentRightAddOnBindings = null;
        var bindings    = new CompositeDisposable();
        var hasBindings = false;

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

        if (e.NameScope.Find<ContentPresenter>("PART_FormFeedBack") is { } formFeedback)
        {
            hasBindings = true;
            bindings.Add(formFeedback.Bind(Visual.IsVisibleProperty,
                new Binding(nameof(IsFormFeedbackVisible)) { Source = this }));
            bindings.Add(formFeedback.Bind(ContentPresenter.ContentProperty,
                new Binding(nameof(FormFeedback)) { Source = this }));
        }

        if (_templateComboBoxHandle != null)
        {
            hasBindings = true;
            var handle = _templateComboBoxHandle;
            handle.HandleClick += HandleOpenPopupClicked;
            bindings.Add(Disposable.Create(() => handle.HandleClick -= HandleOpenPopupClicked));
            bindings.Add(handle.Bind(InputElement.IsEnabledProperty,
                new Binding(nameof(IsEnabled)) { Source = this }));
            bindings.Add(handle.Bind(ComboBoxHandle.IsMotionEnabledProperty,
                new Binding(nameof(IsMotionEnabled)) { Source = this }));
        }

        if (hasBindings)
        {
            _contentRightAddOnBindings = bindings;
            return true;
        }

        bindings.Dispose();
        return _addOnDecoratedBox?.ContentRightAddOn != null;
    }
    
    private void HandleOpenPopupClicked(object? sender, EventArgs e)
    {
        SetCurrentValue(IsDropDownOpenProperty, !IsDropDownOpen);
    }

    internal void NotifyAccessoryHandleClicked()
    {
        SetCurrentValue(IsDropDownOpenProperty, !IsDropDownOpen);
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
            ClearLightweightComboBoxHandle();

            if (_accessoryHost == null)
            {
                _accessoryHost = new ComboBoxAccessoryHost();
                _accessoryHostSpacingBinding = TokenResourceBinder.CreateTokenBinding(
                    _accessoryHost,
                    StackPanel.SpacingProperty,
                    SharedTokenKind.SpacingXS);
                _accessoryHost.AttachOwner(this);
            }

            if (!ReferenceEquals(_addOnDecoratedBox.ContentRightAddOn, _accessoryHost))
            {
                _addOnDecoratedBox.SetCurrentValue(AddOnDecoratedBox.ContentRightAddOnProperty, _accessoryHost);
            }
            return;
        }

        ClearCompositeAccessoryHost();
        if (_lightweightComboBoxHandle == null)
        {
            _lightweightComboBoxHandle = new ComboBoxHandle();
            _lightweightComboBoxHandle.SetTemplatedParent(this);
            _lightweightComboBoxHandle.HandleClick += HandleOpenPopupClicked;
        }
        UpdateLightweightComboBoxHandleState();

        if (!ReferenceEquals(_addOnDecoratedBox.ContentRightAddOn, _lightweightComboBoxHandle))
        {
            _addOnDecoratedBox.SetCurrentValue(AddOnDecoratedBox.ContentRightAddOnProperty, _lightweightComboBoxHandle);
        }
    }

    private bool NeedsCompositeAccessoryHost()
    {
        return ContentRightAddOn != null ||
               IsFormFeedbackVisible && FormFeedback != null;
    }

    private void ClearOwnerDrivenAccessoryHost()
    {
        ClearLightweightComboBoxHandle();
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

        if (_accessoryHost == null)
        {
            return;
        }

        _accessoryHost.DetachOwner();
        _accessoryHost = null;
    }

    private void ClearLightweightComboBoxHandle()
    {
        if (_addOnDecoratedBox != null &&
            _lightweightComboBoxHandle != null &&
            ReferenceEquals(_addOnDecoratedBox.ContentRightAddOn, _lightweightComboBoxHandle))
        {
            _addOnDecoratedBox.ClearValue(AddOnDecoratedBox.ContentRightAddOnProperty);
        }

        if (_lightweightComboBoxHandle == null)
        {
            return;
        }

        _lightweightComboBoxHandle.HandleClick -= HandleOpenPopupClicked;
        _lightweightComboBoxHandle.SetTemplatedParent(null);
        _lightweightComboBoxHandle = null;
    }

    private void UpdateOwnerDrivenAccessoryState()
    {
        if (_isUsingLegacyAccessoryTemplate)
        {
            return;
        }

        if (_accessoryHost != null && NeedsCompositeAccessoryHost())
        {
            _accessoryHost.AttachOwner(this);
        }

        ConfigureOwnerDrivenAccessoryHost();
        UpdateLightweightComboBoxHandleState();
    }

    private void UpdateLightweightComboBoxHandleState()
    {
        if (_lightweightComboBoxHandle == null)
        {
            return;
        }

        _lightweightComboBoxHandle.SetCurrentValue(InputElement.IsEnabledProperty, IsEnabled);
        _lightweightComboBoxHandle.SetCurrentValue(ComboBoxHandle.IsMotionEnabledProperty, IsMotionEnabled);
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
        if (topLevel is Avalonia.Controls.Window window)
        {
            _attachedWindow    = window;
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
    
    private void HandleWindowDeactivated(object? sender, EventArgs e)
    {
        SetCurrentValue(IsDropDownOpenProperty, false);
    }
    
    protected override Control CreateContainerForItemOverride(object? item, int index, object? recycleKey)
    {
        return new ComboBoxItem();
    }
    
    protected override bool NeedsContainerOverride(object? item, int index, out object? recycleKey)
    {
        if (item is ComboBoxItem)
        {
            recycleKey = null;
            return false;
        }

        recycleKey = DefaultRecycleKey;
        return true;
    }

    protected override void PrepareContainerForItemOverride(Control container, object? item, int index)
    {
        base.PrepareContainerForItemOverride(container, item, index);
        if (container is ComboBoxItem comboBoxItem)
        {
            if (item != null && item is not Visual)
            {
                if (!comboBoxItem.IsSet(ComboBoxItem.ContentProperty))
                {
                    comboBoxItem.SetCurrentValue(ComboBoxItem.ContentProperty, item);
                }
            }
            
            if (ItemTemplate != null)
            {
                comboBoxItem[!ComboBoxItem.ContentTemplateProperty] = this[!ItemTemplateProperty];
            }
            
            comboBoxItem[!ComboBoxItem.SizeTypeProperty]        = this[!SizeTypeProperty];
            comboBoxItem[!ComboBoxItem.FontSizeProperty]        = this[!OptionFontSizeProperty];
            comboBoxItem[!ComboBoxItem.IsMotionEnabledProperty] = this[!IsMotionEnabledProperty];
            comboBoxItem[!ComboBoxItem.HeightProperty]          = this[!ItemHeightProperty];
        }
    }

    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        if (change.Property == IsDropDownOpenProperty &&
            change.GetNewValue<bool>())
        {
            EnsurePopupContent();
            ConfigureWindowDeactivatedSubscription();
        }

        base.OnPropertyChanged(change);

        if (change.Property == StatusProperty)
        {
            UpdatePseudoClasses();
        }
        else if (change.Property == DropDownDisplayPageSizeProperty ||
                 change.Property == ItemHeightProperty ||
                 change.Property == PopupContentPaddingProperty)
        {
            ConfigureMaxDropdownHeight();
        }
        else if (change.Property == ItemsPanelProperty ||
                 change.Property == EffectivePopupWidthProperty ||
                 change.Property == IsMotionEnabledProperty ||
                 change.Property == ScrollViewer.IsLiteModeProperty ||
                 change.Property == Avalonia.Controls.ScrollViewer.AllowAutoHideProperty)
        {
            ConfigurePopupContent();
        }
        else if (change.Property == IsDropDownOpenProperty)
        {
            if (change.GetNewValue<bool>())
            {
                ConfigureWindowDeactivatedSubscription();
            }
            else
            {
                ClearWindowDeactivatedSubscription();
            }
        }
        else if (change.Property == SelectedItemProperty && change.NewValue != null)
        {
            // Close dropdown when an item is selected
            if (IsDropDownOpen)
            {
                SetCurrentValue(IsDropDownOpenProperty, false);
            }
        }
        else if (change.Property == FormFeedbackProperty)
        {
            ConfigureFormFeedbackSubscription();
            UpdateOwnerDrivenAccessoryState();
        }
        else if (change.Property == ContentRightAddOnProperty ||
                 change.Property == ContentRightAddOnTemplateProperty ||
                 change.Property == IsFormFeedbackVisibleProperty ||
                 change.Property == InputElement.IsEnabledProperty)
        {
            UpdateOwnerDrivenAccessoryState();
        }

        if (change.Property == IsMotionEnabledProperty)
        {
            UpdateOwnerDrivenAccessoryState();
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
    
    protected override void OnPointerPressed(PointerPressedEventArgs e)
    {
        if (!IsDropDownOpen)
        {
            PseudoClasses.Set(StdPseudoClass.Pressed, true);
        }
    }
    
    protected override void OnPointerReleased(PointerReleasedEventArgs e)
    {
        if (e.Source == this
            && !e.Handled
            && e.InitialPressMouseButton == MouseButton.Right)
        {
            var args = new ContextRequestedEventArgs(e);
            RaiseEvent(args);
            e.Handled = args.Handled;
        }
        if (!e.Handled && e.Source is Visual source)
        {
            // Check if the click is inside the popup
            if (_popup?.IsInsidePopup(source) != true && PseudoClasses.Contains(StdPseudoClass.Pressed))
            {
                SetCurrentValue(IsDropDownOpenProperty, !IsDropDownOpen);
            }
        }
        PseudoClasses.Set(StdPseudoClass.Pressed, false);
    }

    private void UpdatePseudoClasses()
    {
        PseudoClasses.Set(StdPseudoClass.Error, Status == InputControlStatus.Error);
        PseudoClasses.Set(StdPseudoClass.Warning, Status == InputControlStatus.Warning);
    }

    protected override void OnSizeChanged(SizeChangedEventArgs e)
    {
        base.OnSizeChanged(e);
        EffectivePopupWidth = e.NewSize.Width;
    }

    private void ConfigureMaxDropdownHeight()
    {
        SetCurrentValue(MaxDropDownHeightProperty, DropDownDisplayPageSize * ItemHeight + PopupContentPadding.Top + PopupContentPadding.Bottom);
        ConfigurePopupContent();
    }

    private void EnsurePopupContent()
    {
        if (_popupFrame != null)
        {
            ConfigurePopupContent();
            return;
        }

        _popupItemsPresenter = new ItemsPresenter
        {
            Name       = "PART_ItemsPresenter",
            ItemsPanel = ItemsPanel
        };
        Grid.SetIsSharedSizeScope(_popupItemsPresenter, true);
        _popupItemsPresenter.SetTemplatedParent(this);

        _popupScrollViewer = new ScrollViewer
        {
            IsScrollChainingEnabled = false,
            Content                 = _popupItemsPresenter
        };
        _popupScrollViewer.SetTemplatedParent(this);

        _popupFrame = new Border
        {
            Name  = "PopupFrame",
            Child = _popupScrollViewer
        };
        _popupFrame.SetTemplatedParent(this);
        _popup?.SetCurrentValue(Popup.ChildProperty, _popupFrame);

        ConfigurePopupContent();
    }

    private void ConfigurePopupContent()
    {
        if (_popupFrame != null)
        {
            _popupFrame.SetCurrentValue(Border.MaxHeightProperty, MaxDropDownHeight);
            _popupFrame.SetCurrentValue(Border.MinWidthProperty, EffectivePopupWidth);
            _popupFrame.SetCurrentValue(Border.PaddingProperty, PopupContentPadding);
        }

        if (_popupScrollViewer != null)
        {
            _popupScrollViewer.SetCurrentValue(ScrollViewer.IsMotionEnabledProperty, IsMotionEnabled);
            ScrollViewer.SetIsLiteMode(_popupScrollViewer, ScrollViewer.GetIsLiteMode(this));
            _popupScrollViewer.SetCurrentValue(Avalonia.Controls.ScrollViewer.AllowAutoHideProperty,
                Avalonia.Controls.ScrollViewer.GetAllowAutoHide(this));
        }

        _popupItemsPresenter?.SetCurrentValue(ItemsPresenter.ItemsPanelProperty, ItemsPanel);
    }

    private void ClearPopupContent()
    {
        if (_popup != null)
        {
            _popup.SetCurrentValue(Popup.ChildProperty, null);
        }

        if (_popupFrame != null)
        {
            _popupFrame.Child = null;
            _popupFrame.SetTemplatedParent(null);
            _popupFrame = null;
        }

        if (_popupScrollViewer != null)
        {
            _popupScrollViewer.Content = null;
            _popupScrollViewer.SetTemplatedParent(null);
            _popupScrollViewer = null;
        }

        if (_popupItemsPresenter != null)
        {
            _popupItemsPresenter.SetTemplatedParent(null);
            _popupItemsPresenter = null;
        }
    }

    internal static ComboBoxItem? GetComboBoxItemCore(StyledElement? item)
    {
        while (true)
        {
            if (item == null)
            {
                return null;
            }

            if (item is ComboBoxItem menuItem)
            {
                return menuItem;
            }
            item = item.Parent;
        }
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
