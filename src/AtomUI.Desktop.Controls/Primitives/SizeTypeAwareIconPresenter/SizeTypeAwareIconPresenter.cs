using AtomUI.Controls;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Media;
using Avalonia.Metadata;

namespace AtomUI.Desktop.Controls.Primitives;

using IconControl = Icon;

internal class SizeTypeAwareIconPresenter : TemplatedControl, ISizeTypeAware
{
    #region 公共属性定义
    
    public static readonly StyledProperty<SizeType> SizeTypeProperty =
        SizeTypeControlProperty.SizeTypeProperty.AddOwner<SizeTypeAwareIconPresenter>();
    
    public static readonly StyledProperty<PathIcon?> IconProperty =
        AvaloniaProperty.Register<SizeTypeAwareIconPresenter, PathIcon?>(nameof(Icon));

    public static readonly StyledProperty<IconAnimation> LoadingAnimationProperty =
        IconControl.LoadingAnimationProperty.AddOwner<SizeTypeAwareIconPresenter>();

    public static readonly StyledProperty<TimeSpan> LoadingAnimationDurationProperty =
        IconControl.LoadingAnimationDurationProperty.AddOwner<SizeTypeAwareIconPresenter>();
    
    public static readonly StyledProperty<IBrush?> StrokeBrushProperty =
        IconControl.StrokeBrushProperty.AddOwner<IconPresenter>();
    
    public static readonly StyledProperty<IBrush?> FillBrushProperty =
        IconControl.FillBrushProperty.AddOwner<IconPresenter>();
    
    public static readonly StyledProperty<IBrush?> SecondaryStrokeBrushProperty =
        IconControl.SecondaryStrokeBrushProperty.AddOwner<IconPresenter>();
    
    public static readonly StyledProperty<IBrush?> SecondaryFillBrushProperty =
        IconControl.SecondaryFillBrushProperty.AddOwner<IconPresenter>();

    public static readonly StyledProperty<double> IconWidthProperty =
        AvaloniaProperty.Register<SizeTypeAwareIconPresenter, double>(nameof(IconWidth), double.NaN);

    public static readonly StyledProperty<double> IconHeightProperty =
        AvaloniaProperty.Register<SizeTypeAwareIconPresenter, double>(nameof(IconHeight), double.NaN);
    
    public SizeType SizeType
    {
        get => GetValue(SizeTypeProperty);
        set => SetValue(SizeTypeProperty, value);
    }
    
    [Content]
    public PathIcon? Icon
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
    
    public IBrush? StrokeBrush
    {
        get => GetValue(StrokeBrushProperty);
        set => SetValue(StrokeBrushProperty, value);
    }
    
    public IBrush? FillBrush
    {
        get => GetValue(FillBrushProperty);
        set => SetValue(FillBrushProperty, value);
    }
    
    public IBrush? SecondaryStrokeBrush
    {
        get => GetValue(SecondaryStrokeBrushProperty);
        set => SetValue(SecondaryStrokeBrushProperty, value);
    }
    
    public IBrush? SecondaryFillBrush
    {
        get => GetValue(SecondaryFillBrushProperty);
        set => SetValue(SecondaryFillBrushProperty, value);
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

    #endregion
    
    static SizeTypeAwareIconPresenter()
    {
        AffectsMeasure<SizeTypeAwareIconPresenter>(IconProperty, PaddingProperty);
    }

    protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
    {
        base.OnApplyTemplate(e);
        UpdatePseudoClasses();
    }

    private void UpdatePseudoClasses()
    {
        PseudoClasses.Set(StdPseudoClass.Empty, Icon is null);
    }
}