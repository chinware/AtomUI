using AtomUI.IconPkg;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Media;
using Avalonia.Metadata;

namespace AtomUI.Controls.Primitives;

internal class SizeTypeAwareIconPresenter : TemplatedControl, ISizeTypeAware
{
    #region 公共属性定义
    
    public static readonly StyledProperty<SizeType> SizeTypeProperty =
        SizeTypeControlProperty.SizeTypeProperty.AddOwner<SizeTypeAwareIconPresenter>();
    
    public static readonly StyledProperty<Icon?> IconProperty =
        AvaloniaProperty.Register<SizeTypeAwareIconPresenter, Icon?>(nameof(Icon));

    public static readonly StyledProperty<IconAnimation> LoadingAnimationProperty =
        Icon.LoadingAnimationProperty.AddOwner<SizeTypeAwareIconPresenter>();

    public static readonly StyledProperty<TimeSpan> LoadingAnimationDurationProperty =
        Icon.LoadingAnimationDurationProperty.AddOwner<SizeTypeAwareIconPresenter>();
    
    public static readonly StyledProperty<IBrush?> NormalFilledBrushProperty =
        Icon.NormalFilledBrushProperty.AddOwner<SizeTypeAwareIconPresenter>();
    
    public static readonly StyledProperty<IBrush?> ActiveFilledBrushProperty =
        Icon.ActiveFilledBrushProperty.AddOwner<SizeTypeAwareIconPresenter>();
    
    public static readonly StyledProperty<IBrush?> SelectedFilledBrushProperty =
        Icon.SelectedFilledBrushProperty.AddOwner<SizeTypeAwareIconPresenter>();
    
    public static readonly StyledProperty<IBrush?> DisabledFilledBrushProperty =
        Icon.DisabledFilledBrushProperty.AddOwner<SizeTypeAwareIconPresenter>();

    public static readonly StyledProperty<double> IconWidthProperty =
        AvaloniaProperty.Register<SizeTypeAwareIconPresenter, double>(nameof(IconWidth), double.NaN);

    public static readonly StyledProperty<double> IconHeightProperty =
        AvaloniaProperty.Register<SizeTypeAwareIconPresenter, double>(nameof(IconHeight), double.NaN);
    
    public static readonly StyledProperty<IconMode> IconModeProperty =
        Icon.IconModeProperty.AddOwner<SizeTypeAwareIconPresenter>();
    
    public SizeType SizeType
    {
        get => GetValue(SizeTypeProperty);
        set => SetValue(SizeTypeProperty, value);
    }
    
    [Content]
    public Icon? Icon
    {
        get => GetValue(IconProperty);
        set => SetValue(IconProperty, value);
    }
    
    public IconAnimation LoadingAnimation
    {
        get => GetValue(LoadingAnimationProperty);
        set => SetValue(LoadingAnimationProperty, value);
    }

    public TimeSpan LoadingAnimationDuration
    {
        get => GetValue(LoadingAnimationDurationProperty);
        set => SetValue(LoadingAnimationDurationProperty, value);
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
    
    public IBrush? NormalFilledBrush
    {
        get => GetValue(NormalFilledBrushProperty);
        set => SetValue(NormalFilledBrushProperty, value);
    }

    public IBrush? ActiveFilledBrush
    {
        get => GetValue(ActiveFilledBrushProperty);
        set => SetValue(ActiveFilledBrushProperty, value);
    }

    public IBrush? SelectedFilledBrush
    {
        get => GetValue(SelectedFilledBrushProperty);
        set => SetValue(SelectedFilledBrushProperty, value);
    }

    public IBrush? DisabledFilledBrush
    {
        get => GetValue(DisabledFilledBrushProperty);
        set => SetValue(DisabledFilledBrushProperty, value);
    }
    
    public IconMode IconMode
    {
        get => GetValue(IconModeProperty);
        set => SetValue(IconModeProperty, value);
    }

    #endregion
    
    static SizeTypeAwareIconPresenter()
    {
        AffectsMeasure<SizeTypeAwareIconPresenter>(IconProperty, PaddingProperty);
    }
    
    private void UpdatePseudoClasses()
    {
        PseudoClasses.Set(StdPseudoClass.Empty, Icon is null);
    }
}