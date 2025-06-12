using AtomUI.Controls.Primitives;
using AtomUI.Controls.Themes;
using AtomUI.Data;
using AtomUI.IconPkg;
using AtomUI.Theme;
using AtomUI.Theme.Utils;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Controls.Templates;
using Avalonia.VisualTree;

namespace AtomUI.Controls;

using AvaloniaButtonSpinner = Avalonia.Controls.ButtonSpinner;

public class ButtonSpinner : AvaloniaButtonSpinner,
                             IMotionAwareControl,
                             IControlSharedTokenResourcesHost
{
    #region 公共属性定义

    public static readonly StyledProperty<object?> LeftAddOnProperty =
        AvaloniaProperty.Register<ButtonSpinner, object?>(nameof(LeftAddOn));
    
    public static readonly StyledProperty<IDataTemplate?> LeftAddOnTemplateProperty =
        ContentTemplateProperty.AddOwner<ButtonSpinner>();

    public static readonly StyledProperty<object?> RightAddOnProperty =
        AvaloniaProperty.Register<ButtonSpinner, object?>(nameof(RightAddOn));
    
    public static readonly StyledProperty<IDataTemplate?> RightAddOnTemplateProperty =
        ContentTemplateProperty.AddOwner<ButtonSpinner>();

    public static readonly StyledProperty<object?> InnerLeftContentProperty
        = AvaloniaProperty.Register<ButtonSpinner, object?>(nameof(InnerLeftContent));
    
    public static readonly StyledProperty<IDataTemplate?> InnerLeftContentTemplateProperty =
        ContentTemplateProperty.AddOwner<ButtonSpinner>();

    public static readonly StyledProperty<object?> InnerRightContentProperty
        = AvaloniaProperty.Register<ButtonSpinner, object?>(nameof(InnerRightContent));
    
    public static readonly StyledProperty<IDataTemplate?> InnerRightContentTemplateProperty =
        ContentTemplateProperty.AddOwner<ButtonSpinner>();

    public static readonly StyledProperty<SizeType> SizeTypeProperty =
        SizeTypeAwareControlProperty.SizeTypeProperty.AddOwner<ButtonSpinner>();

    public static readonly StyledProperty<AddOnDecoratedVariant> StyleVariantProperty =
        AddOnDecoratedBox.StyleVariantProperty.AddOwner<ButtonSpinner>();

    public static readonly StyledProperty<AddOnDecoratedStatus> StatusProperty =
        AddOnDecoratedBox.StatusProperty.AddOwner<ButtonSpinner>();
    
    public static readonly StyledProperty<bool> IsMotionEnabledProperty = 
        MotionAwareControlProperty.IsMotionEnabledProperty.AddOwner<ButtonSpinner>();

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
    
    public bool IsMotionEnabled
    {
        get => GetValue(IsMotionEnabledProperty);
        set => SetValue(IsMotionEnabledProperty, value);
    }
    
    #endregion

    #region 内部属性定义

    Control IControlSharedTokenResourcesHost.HostControl => this;
    string IControlSharedTokenResourcesHost.TokenId => ButtonSpinnerToken.ID;
    Control IMotionAwareControl.PropertyBindTarget => this;

    #endregion
    
    private ButtonSpinnerDecoratedBox? _decoratedBox;
    private ButtonSpinnerInnerBox? _buttonSpinnerInnerBox;

    public ButtonSpinner()
    {
        this.RegisterResources();
        this.BindMotionProperties();
    }

    protected override Size ArrangeOverride(Size finalSize)
    {
        var borderThickness = _decoratedBox?.BorderThickness ?? default;
        return base.ArrangeOverride(finalSize).Inflate(borderThickness);
    }

    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        base.OnPropertyChanged(change);
        if (this.IsAttachedToVisualTree())
        {
            if (change.Property == LeftAddOnProperty ||
                change.Property == RightAddOnProperty ||
                change.Property == InnerLeftContentProperty ||
                change.Property == InnerRightContentProperty)
            {
                ConfigureAddOns();
            }
        }
    }

    protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
    {
        _decoratedBox          = e.NameScope.Find<ButtonSpinnerDecoratedBox>(ButtonSpinnerThemeConstants.DecoratedBoxPart);
        _buttonSpinnerInnerBox = e.NameScope.Find<ButtonSpinnerInnerBox>(ButtonSpinnerThemeConstants.SpinnerInnerBoxPart);
        base.OnApplyTemplate(e);
        if (_buttonSpinnerInnerBox?.SpinnerContent is ButtonSpinnerHandle spinnerHandle)
        {
            spinnerHandle.ButtonsCreated += (sender, args) =>
            {
                this.SetIncreaseButton(spinnerHandle.IncreaseButton);
                this.SetDecreaseButton(spinnerHandle.DecreaseButton);
            };
        }

        ConfigureAddOns();
    }

    private void ConfigureAddOns()
    {
        if (LeftAddOn is Icon leftAddOnIcon)
        {
            var iconPresenter = new SizeTypeAwareIconPresenter()
            {
                Icon = leftAddOnIcon
            };
            BindUtils.RelayBind(this, SizeTypeProperty, iconPresenter, SizeTypeProperty);
            LeftAddOn = iconPresenter;
        }
        if (InnerLeftContent is Icon innerLeftContent)
        {
            var iconPresenter = new SizeTypeAwareIconPresenter()
            {
                Icon = innerLeftContent
            };
            BindUtils.RelayBind(this, SizeTypeProperty, iconPresenter, SizeTypeProperty);
            InnerLeftContent = iconPresenter;
        }
        if (RightAddOn is Icon rightAddOnIcon)
        {
            var iconPresenter = new SizeTypeAwareIconPresenter()
            {
                Icon = rightAddOnIcon
            };
            BindUtils.RelayBind(this, SizeTypeProperty, iconPresenter, SizeTypeProperty);
            RightAddOn = iconPresenter;
        }
        if (InnerRightContent is Icon innerRightContent)
        {
            var iconPresenter = new SizeTypeAwareIconPresenter()
            {
                Icon = innerRightContent
            };
            BindUtils.RelayBind(this, SizeTypeProperty, iconPresenter, SizeTypeProperty);
            InnerRightContent = iconPresenter;
        }
    }
}