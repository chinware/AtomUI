using System.Reactive.Disposables;
using AtomUI.Animations;
using AtomUI.Controls.Utils;
using AtomUI.Theme;
using AtomUI.Theme.Data;
using AtomUI.Theme.Styling;
using AtomUI.Theme.Utils;
using Avalonia;
using Avalonia.Animation;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Controls.Primitives.PopupPositioning;
using Avalonia.Data;

namespace AtomUI.Controls;

using AvaloniaButton = Avalonia.Controls.Button;

public abstract class AbstractColorPicker : AvaloniaButton, 
                                   ISizeTypeAware,
                                   IMotionAwareControl,
                                   IControlSharedTokenResourcesHost,
                                   IResourceBindingManager
{
    #region 公共属性定义
    
    public static readonly StyledProperty<ColorFormat> FormatProperty =
        AvaloniaProperty.Register<AbstractColorPicker, ColorFormat>(nameof(Format), ColorFormat.Hex);

    public static readonly StyledProperty<FlyoutTriggerType> TriggerTypeProperty =
        FlyoutStateHelper.TriggerTypeProperty.AddOwner<AbstractColorPicker>();
    
    public static readonly StyledProperty<bool> IsShowArrowProperty =
        ArrowDecoratedBox.IsShowArrowProperty.AddOwner<AbstractColorPicker>();

    public static readonly StyledProperty<bool> IsPointAtCenterProperty =
        AtomUI.Controls.Flyout.IsPointAtCenterProperty.AddOwner<AbstractColorPicker>();
    
    public static readonly StyledProperty<PlacementMode> PlacementProperty =
        Avalonia.Controls.Primitives.Popup.PlacementProperty.AddOwner<AbstractColorPicker>();
    
    public static readonly StyledProperty<PopupGravity> PlacementGravityProperty =
        Avalonia.Controls.Primitives.Popup.PlacementGravityProperty.AddOwner<AbstractColorPicker>();

    public static readonly StyledProperty<double> MarginToAnchorProperty =
        Popup.MarginToAnchorProperty.AddOwner<AbstractColorPicker>();

    public static readonly StyledProperty<int> MouseEnterDelayProperty =
        FlyoutStateHelper.MouseEnterDelayProperty.AddOwner<AbstractColorPicker>();

    public static readonly StyledProperty<int> MouseLeaveDelayProperty =
        FlyoutStateHelper.MouseLeaveDelayProperty.AddOwner<AbstractColorPicker>();
    
    public static readonly StyledProperty<bool> DisabledAlphaProperty =
        AvaloniaProperty.Register<AbstractColorPicker, bool>(nameof(DisabledAlpha));
    
    public static readonly StyledProperty<bool> DisabledFormatProperty =
        AvaloniaProperty.Register<AbstractColorPicker, bool>(nameof(DisabledFormat));
    
    public static readonly StyledProperty<bool> IsShowTextProperty =
        AvaloniaProperty.Register<AbstractColorPicker, bool>(nameof(IsShowText));
    
    public static readonly StyledProperty<SizeType> SizeTypeProperty =
        SizeTypeAwareControlProperty.SizeTypeProperty.AddOwner<AbstractColorPicker>();
    
    public static readonly StyledProperty<bool> IsMotionEnabledProperty =
        MotionAwareControlProperty.IsMotionEnabledProperty.AddOwner<AbstractColorPicker>();

    public ColorFormat Format
    {
        get => GetValue(FormatProperty);
        set => SetValue(FormatProperty, value);
    }
    
    public FlyoutTriggerType TriggerType
    {
        get => GetValue(TriggerTypeProperty);
        set => SetValue(TriggerTypeProperty, value);
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
    
    public bool DisabledAlpha
    {
        get => GetValue(DisabledAlphaProperty);
        set => SetValue(DisabledAlphaProperty, value);
    }
    
    public bool DisabledFormat
    {
        get => GetValue(DisabledFormatProperty);
        set => SetValue(DisabledFormatProperty, value);
    }
    
    public bool IsShowText
    {
        get => GetValue(IsShowTextProperty);
        set => SetValue(IsShowTextProperty, value);
    }
    
    public SizeType SizeType
    {
        get => GetValue(SizeTypeProperty);
        set => SetValue(SizeTypeProperty, value);
    }
    
    public bool IsMotionEnabled
    {
        get => GetValue(IsMotionEnabledProperty);
        set => SetValue(IsMotionEnabledProperty, value);
    }
    
    #endregion

    #region 内部属性定义

    Control IMotionAwareControl.PropertyBindTarget => this;
    Control IControlSharedTokenResourcesHost.HostControl => this;
    string IControlSharedTokenResourcesHost.TokenId => ColorPickerToken.ID;
    
    CompositeDisposable? IResourceBindingManager.ResourceBindingsDisposable
    {
        get => _resourceBindingsDisposable;
        set => _resourceBindingsDisposable = value;
    }
    #endregion
    
    private CompositeDisposable? _resourceBindingsDisposable;

    static AbstractColorPicker()
    {
        AffectsMeasure<AbstractColorPicker>(IsShowTextProperty, 
            FormatProperty);
    }
    
    public AbstractColorPicker()
    {
        this.RegisterResources();
        _resourceBindingsDisposable = new CompositeDisposable();
    }
    
    protected override void OnAttachedToVisualTree(VisualTreeAttachmentEventArgs e)
    {
        base.OnAttachedToVisualTree(e);
        _resourceBindingsDisposable = new CompositeDisposable();
        this.AddResourceBindingDisposable(TokenResourceBinder.CreateTokenBinding(this, BorderThicknessProperty,
            SharedTokenKey.BorderThickness,
            BindingPriority.Template,
            new RenderScaleAwareThicknessConfigure(this)));
    }

    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        base.OnPropertyChanged(change);
        if (change.Property == IsMotionEnabledProperty)
        {
            ConfigureTransitions();
        }
        else if (change.Property == IsShowTextProperty ||
                 change.Property == FormatProperty)
        {
            GenerateValueText();
        }
    }

    protected abstract void GenerateValueText();

    protected override void OnDetachedFromVisualTree(VisualTreeAttachmentEventArgs e)
    {
        base.OnDetachedFromVisualTree(e);
        this.DisposeTokenBindings();
    }

    private void ConfigureTransitions()
    {
        if (IsMotionEnabled)
        {
            Transitions = new Transitions()
            {
                TransitionUtils.CreateTransition<SolidColorBrushTransition>(BorderBrushProperty)
            };
        }
        else
        {
            Transitions?.Clear();
            Transitions = null;
        }
    }
    
    protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
    {
        base.OnApplyTemplate(e);
        ConfigureTransitions();
    }
}