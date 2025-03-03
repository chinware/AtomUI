using System.Reactive.Disposables;
using AtomUI.Data;
using AtomUI.Theme;
using AtomUI.Theme.Utils;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Layout;
using Avalonia.LogicalTree;

namespace AtomUI.Controls;

using AvaloniaComboBox = Avalonia.Controls.ComboBox;

public class ComboBox : AvaloniaComboBox,
                        IAnimationAwareControl,
                        IControlSharedTokenResourcesHost,
                        ITokenResourceConsumer
{
    public const string ErrorPC = ":error";
    public const string WarningPC = ":warning";

    #region 公共属性定义

    public static readonly StyledProperty<object?> LeftAddOnProperty =
        AddOnDecoratedBox.LeftAddOnProperty.AddOwner<ComboBox>();

    public static readonly StyledProperty<object?> RightAddOnProperty =
        AddOnDecoratedBox.RightAddOnProperty.AddOwner<ComboBox>();

    public static readonly StyledProperty<SizeType> SizeTypeProperty =
        AddOnDecoratedBox.SizeTypeProperty.AddOwner<ComboBox>();

    public static readonly StyledProperty<AddOnDecoratedVariant> StyleVariantProperty =
        AddOnDecoratedBox.StyleVariantProperty.AddOwner<ComboBox>();

    public static readonly StyledProperty<AddOnDecoratedStatus> StatusProperty =
        AddOnDecoratedBox.StatusProperty.AddOwner<ComboBox>();

    public static readonly StyledProperty<object?> InnerLeftContentProperty
        = TextBox.InnerLeftContentProperty.AddOwner<ComboBox>();

    public static readonly StyledProperty<object?> InnerRightContentProperty
        = TextBox.InnerRightContentProperty.AddOwner<ComboBox>();

    public static readonly StyledProperty<bool> IsEnableClearButtonProperty =
        TextBox.IsEnableClearButtonProperty.AddOwner<ComboBox>();

    public static readonly StyledProperty<bool> IsMotionEnabledProperty
        = AnimationAwareControlProperty.IsMotionEnabledProperty.AddOwner<ComboBox>();

    public static readonly StyledProperty<bool> IsWaveAnimationEnabledProperty
        = AnimationAwareControlProperty.IsWaveAnimationEnabledProperty.AddOwner<ComboBox>();

    public object? LeftAddOn
    {
        get => GetValue(LeftAddOnProperty);
        set => SetValue(LeftAddOnProperty, value);
    }

    public object? RightAddOn
    {
        get => GetValue(RightAddOnProperty);
        set => SetValue(RightAddOnProperty, value);
    }

    public SizeType SizeType
    {
        get => GetValue(SizeTypeProperty);
        set => SetValue(SizeTypeProperty, value);
    }

    public AddOnDecoratedVariant StyleVariant
    {
        get => GetValue(StyleVariantProperty);
        set => SetValue(StyleVariantProperty, value);
    }

    public AddOnDecoratedStatus Status
    {
        get => GetValue(StatusProperty);
        set => SetValue(StatusProperty, value);
    }

    public object? InnerLeftContent
    {
        get => GetValue(InnerLeftContentProperty);
        set => SetValue(InnerLeftContentProperty, value);
    }

    public object? InnerRightContent
    {
        get => GetValue(InnerRightContentProperty);
        set => SetValue(InnerRightContentProperty, value);
    }

    public bool IsEnableClearButton
    {
        get => GetValue(IsEnableClearButtonProperty);
        set => SetValue(IsEnableClearButtonProperty, value);
    }

    public bool IsMotionEnabled
    {
        get => GetValue(IsMotionEnabledProperty);
        set => SetValue(IsMotionEnabledProperty, value);
    }

    public bool IsWaveAnimationEnabled
    {
        get => GetValue(IsWaveAnimationEnabledProperty);
        set => SetValue(IsWaveAnimationEnabledProperty, value);
    }

    #endregion

    #region 内部属性定义

    Control IAnimationAwareControl.PropertyBindTarget => this;
    Control IControlSharedTokenResourcesHost.HostControl => this;
    string IControlSharedTokenResourcesHost.TokenId => ComboBoxToken.ID;
    CompositeDisposable? ITokenResourceConsumer.TokenBindingsDisposable => _tokenBindingsDisposable;

    #endregion
    
    private CompositeDisposable? _tokenBindingsDisposable;

    static ComboBox()
    {
        HorizontalAlignmentProperty.OverrideDefaultValue<ComboBox>(HorizontalAlignment.Left);
        VerticalAlignmentProperty.OverrideDefaultValue<ComboBox>(VerticalAlignment.Top);
    }

    public ComboBox()
    {
        this.RegisterResources();
        this.BindAnimationProperties(IsMotionEnabledProperty, IsWaveAnimationEnabledProperty);
    }

    private IconButton? _openIndicatorButton;
    private ComboBoxSpinnerInnerBox? _spinnerInnerBox;

    protected override Size ArrangeOverride(Size finalSize)
    {
        var size = base.ArrangeOverride(finalSize);
        if (_popup is not null)
        {
            _popup.MinWidth = size.Width;
        }

        return size;
    }

    private Popup? _popup;

    protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
    {
        base.OnApplyTemplate(e);
        this.RunThemeTokenBindingActions();
        _popup               = e.NameScope.Find<Popup>(ComboBoxTheme.PopupPart);
        _openIndicatorButton = e.NameScope.Find<IconButton>(ComboBoxTheme.OpenIndicatorButtonPart);
        _spinnerInnerBox     = e.NameScope.Find<ComboBoxSpinnerInnerBox>(ComboBoxTheme.SpinnerInnerBoxPart);

        if (_openIndicatorButton is not null)
        {
            _openIndicatorButton.Click += (sender, args) => { SetCurrentValue(IsDropDownOpenProperty, true); };
        }

        UpdatePseudoClasses();
    }

    protected override void PrepareContainerForItemOverride(Control container, object? item, int index)
    {
        base.PrepareContainerForItemOverride(container, item, index);
        if (container is ComboBoxItem comboBoxItem)
        {
            BindUtils.RelayBind(this, SizeTypeProperty, comboBoxItem, ComboBoxItem.SizeTypeProperty);
            BindUtils.RelayBind(this, IsMotionEnabledProperty, comboBoxItem, ComboBoxItem.IsMotionEnabledProperty);
        }
    }

    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        base.OnPropertyChanged(change);

        if (change.Property == StatusProperty)
        {
            UpdatePseudoClasses();
        }
    }

    private void UpdatePseudoClasses()
    {
        PseudoClasses.Set(ErrorPC, Status == AddOnDecoratedStatus.Error);
        PseudoClasses.Set(WarningPC, Status == AddOnDecoratedStatus.Warning);
    }

    protected override void OnAttachedToLogicalTree(LogicalTreeAttachmentEventArgs e)
    {
        base.OnAttachedToLogicalTree(e);
        _tokenBindingsDisposable = new CompositeDisposable();
    }

    protected override void OnDetachedFromLogicalTree(LogicalTreeAttachmentEventArgs e)
    {
        base.OnDetachedFromLogicalTree(e);
        this.DisposeTokenBindings();
    }
}