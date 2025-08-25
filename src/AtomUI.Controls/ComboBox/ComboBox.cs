using AtomUI.Controls.Themes;
using AtomUI.Data;
using AtomUI.Theme;
using AtomUI.Theme.Utils;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Diagnostics;
using Avalonia.Controls.Primitives;
using Avalonia.Controls.Templates;
using Avalonia.Input;

namespace AtomUI.Controls;

using AvaloniaComboBox = Avalonia.Controls.ComboBox;

public class ComboBox : AvaloniaComboBox,
                        IMotionAwareControl,
                        IControlSharedTokenResourcesHost
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
    
    public static readonly StyledProperty<object?> InnerLeftContentProperty = 
        AvaloniaProperty.Register<ComboBox, object?>(nameof(InnerLeftContent));
    
    public static readonly StyledProperty<IDataTemplate?> InnerLeftContentTemplateProperty =
        AvaloniaProperty.Register<ComboBox, IDataTemplate?>(nameof(InnerLeftContentTemplate));

    public static readonly StyledProperty<object?> InnerRightContentProperty =
        AvaloniaProperty.Register<ComboBox, object?>(nameof(InnerRightContent));
    
    public static readonly StyledProperty<IDataTemplate?> InnerRightContentTemplateProperty =
        AvaloniaProperty.Register<ComboBox, IDataTemplate?>(nameof(InnerRightContentTemplate));

    public static readonly StyledProperty<SizeType> SizeTypeProperty =
        SizeTypeAwareControlProperty.SizeTypeProperty.AddOwner<ComboBox>();

    public static readonly StyledProperty<AddOnDecoratedVariant> StyleVariantProperty =
        AddOnDecoratedBox.StyleVariantProperty.AddOwner<ComboBox>();

    public static readonly StyledProperty<AddOnDecoratedStatus> StatusProperty =
        AddOnDecoratedBox.StatusProperty.AddOwner<ComboBox>();

    public static readonly StyledProperty<bool> IsEnableClearButtonProperty =
        TextBox.IsEnableClearButtonProperty.AddOwner<ComboBox>();

    public static readonly StyledProperty<bool> IsMotionEnabledProperty =
        MotionAwareControlProperty.IsMotionEnabledProperty.AddOwner<ComboBox>();

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
    
    public object? InnerLeftContent
    {
        get => GetValue(InnerLeftContentProperty);
        set => SetValue(InnerLeftContentProperty, value);
    }
    
    public IDataTemplate? InnerLeftContentTemplate
    {
        get => GetValue(InnerLeftContentTemplateProperty);
        set => SetValue(InnerLeftContentTemplateProperty, value);
    }

    public object? InnerRightContent
    {
        get => GetValue(InnerRightContentProperty);
        set => SetValue(InnerRightContentProperty, value);
    }
    
    public IDataTemplate? InnerRightContentTemplate
    {
        get => GetValue(InnerRightContentTemplateProperty);
        set => SetValue(InnerRightContentTemplateProperty, value);
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

    #endregion

    #region 内部属性定义
    
    internal static readonly DirectProperty<ComboBox, double> EffectivePopupWidthProperty =
        AvaloniaProperty.RegisterDirect<ComboBox, double>(
            nameof(EffectivePopupWidth),
            o => o.EffectivePopupWidth,
            (o, v) => o.EffectivePopupWidth = v);
    
    private double _effectivePopupWidth;

    internal double EffectivePopupWidth
    {
        get => _effectivePopupWidth;
        set => SetAndRaise(EffectivePopupWidthProperty, ref _effectivePopupWidth, value);
    }

    Control IMotionAwareControl.PropertyBindTarget => this;
    Control IControlSharedTokenResourcesHost.HostControl => this;
    string IControlSharedTokenResourcesHost.TokenId => ComboBoxToken.ID;

    #endregion
    
    private Popup? _popup;

    public ComboBox()
    {
        this.RegisterResources();
    }
    
    protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
    {
        base.OnApplyTemplate(e);
        this.SetPopup(null); // 情况父类，方式鼠标点击的错误处理
        _popup               = e.NameScope.Find<Popup>(ComboBoxThemeConstants.PopupPart);
        var spinnerHandle = e.NameScope.Find<ComboBoxSpinnerInnerBox>(ComboBoxThemeConstants.SpinnerInnerBoxPart);
        if (spinnerHandle?.SpinnerContent is ComboBoxHandle handle)
        {
            handle.HandleClick += HandleOpenPopupClicked;
        }
        if (_popup is IPopupHostProvider popupHostProvider)
        {
            popupHostProvider.PopupHostChanged += HandlePopupHostChanged;
        }
        UpdatePseudoClasses();
    }

    private void HandleOpenPopupClicked(object? sender, EventArgs e)
    {
        SetCurrentValue(IsDropDownOpenProperty, !IsDropDownOpen);
    }
    
    private void HandlePopupHostChanged(IPopupHost? host)
    {
        if (host is PopupRoot popupRoot)
        {
            if (popupRoot.ParentTopLevel is WindowBase window)
            {
                window.Deactivated += (sender, args) =>
                {
                    IsDropDownOpen = false;
                };
            }
        }
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
    
    protected override void OnPointerReleased(PointerReleasedEventArgs e)
    {
        if (!e.Handled && e.Source is Visual source)
        {
            if (_popup?.IsInsidePopup(source) == true)
            {
                if (UpdateSelectionFromEventSource(e.Source))
                {
                    e.Handled = true;
                }
            }
        }
        base.OnPointerReleased(e);
    }

    private void UpdatePseudoClasses()
    {
        PseudoClasses.Set(StdPseudoClass.Error, Status == AddOnDecoratedStatus.Error);
        PseudoClasses.Set(StdPseudoClass.Warning, Status == AddOnDecoratedStatus.Warning);
    }

    protected override void OnSizeChanged(SizeChangedEventArgs e)
    {
        base.OnSizeChanged(e);
        EffectivePopupWidth = e.NewSize.Width;
    }
}