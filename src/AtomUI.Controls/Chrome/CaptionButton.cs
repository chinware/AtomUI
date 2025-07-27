using System.Diagnostics;
using AtomUI.Animations;
using AtomUI.Controls.Utils;
using AtomUI.IconPkg;
using AtomUI.Theme;
using AtomUI.Theme.Utils;
using Avalonia;
using Avalonia.Animation;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Data;
using Avalonia.Utilities;
using Avalonia.VisualTree;

namespace AtomUI.Controls;

using AvaloniaButton = Avalonia.Controls.Button;

internal class CaptionButton : AvaloniaButton, IControlSharedTokenResourcesHost
{
    #region 公共属性定义

    public static readonly StyledProperty<Icon?> NormalIconProperty =
        AvaloniaProperty.Register<CaptionButton, Icon?>(nameof(NormalIcon));
    
    public static readonly StyledProperty<Icon?> CheckedIconProperty =
        AvaloniaProperty.Register<CaptionButton, Icon?>(nameof(CheckedIcon));
    
    public static readonly StyledProperty<double> IconWidthProperty =
        AvaloniaProperty.Register<CaptionButton, double>(nameof(IconWidth));

    public static readonly StyledProperty<double> IconHeightProperty = 
        AvaloniaProperty.Register<CaptionButton, double>(nameof(IconHeight));
    
    public static readonly StyledProperty<bool> IsCheckedProperty = 
        AvaloniaProperty.Register<CaptionButton, bool>(nameof(IsChecked), defaultBindingMode: BindingMode.TwoWay, defaultValue:false);
    
    public Icon? NormalIcon
    {
        get => GetValue(NormalIconProperty);
        set => SetValue(NormalIconProperty, value);
    }
    
    public Icon? CheckedIcon
    {
        get => GetValue(CheckedIconProperty);
        set => SetValue(CheckedIconProperty, value);
    }

    public double IconWidth
    {
        get => GetValue(IconWidthProperty);
        set => SetValue(IconWidthProperty, value);
    }

    public double IconHeight
    {
        get => GetValue(IconHeightProperty);
        set => SetValue(IconHeightProperty, value);
    }
    
    public bool IsChecked
    {
        get => GetValue(IsCheckedProperty);
        set => SetValue(IsCheckedProperty, value);
    }

    #endregion
    
    #region 内部属性定义
    
    internal static readonly StyledProperty<bool> IsMotionEnabledProperty = 
        MotionAwareControlProperty.IsMotionEnabledProperty.AddOwner<CaptionButton>();
    
    internal static readonly StyledProperty<bool> IsWindowActiveProperty = 
        TitleBar.IsWindowActiveProperty.AddOwner<CaptionButton>();
    
    internal static readonly DirectProperty<CaptionButton, CornerRadius> EffectiveCornerRadiusProperty =
        AvaloniaProperty.RegisterDirect<CaptionButton, CornerRadius>(
            nameof(EffectiveCornerRadius),
            o => o.EffectiveCornerRadius,
            (o, v) => o.EffectiveCornerRadius = v);
    
    internal bool IsMotionEnabled
    {
        get => GetValue(IsMotionEnabledProperty);
        set => SetValue(IsMotionEnabledProperty, value);
    }
    
    internal bool IsWindowActive
    {
        get => GetValue(IsWindowActiveProperty);
        set => SetValue(IsWindowActiveProperty, value);
    }

    private CornerRadius _effectiveCornerRadius;

    internal CornerRadius EffectiveCornerRadius
    {
        get => _effectiveCornerRadius;
        set => SetAndRaise(EffectiveCornerRadiusProperty, ref _effectiveCornerRadius, value);
    }
    
    Control IControlSharedTokenResourcesHost.HostControl => this;
    string IControlSharedTokenResourcesHost.TokenId => ChromeToken.ID;

    #endregion

    static CaptionButton()
    {
        AffectsRender<CaptionButton>(EffectiveCornerRadiusProperty);
        AffectsMeasure<CaptionButton>(IsCheckedProperty);
    }
    
    public CaptionButton()
    {
        this.RegisterResources();
    }

    protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
    {
        base.OnApplyTemplate(e);
        ConfigureTransitions();
    }

    protected override void OnSizeChanged(SizeChangedEventArgs e)
    {
        base.OnSizeChanged(e);
        Debug.Assert(MathUtilities.AreClose(DesiredSize.Width, DesiredSize.Height));
        EffectiveCornerRadius = new CornerRadius(DesiredSize.Width / 2);
    }

    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        base.OnPropertyChanged(change);
        if (this.IsAttachedToVisualTree())
        {
            if (change.Property == IsMotionEnabledProperty)
            {
                ConfigureTransitions();
            }
        }
    }

    private void ConfigureTransitions()
    {
        if (IsMotionEnabled)
        {
            Transitions = new Transitions()
            {
                TransitionUtils.CreateTransition<SolidColorBrushTransition>(BackgroundProperty),
            };
        }
        else
        {
            Transitions?.Clear();
            Transitions = null;
        }
    }
}