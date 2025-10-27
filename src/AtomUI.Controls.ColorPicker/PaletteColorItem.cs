using AtomUI.Animations;
using AtomUI.Controls.Themes;
using AtomUI.Controls.Utils;
using AtomUI.IconPkg;
using AtomUI.Theme.Data;
using AtomUI.Theme.Styling;
using Avalonia;
using Avalonia.Animation;
using Avalonia.Animation.Easings;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Data;
using Avalonia.Interactivity;
using Avalonia.Media;

namespace AtomUI.Controls;

using AvaloniaRadioButton = Avalonia.Controls.RadioButton;

internal class PaletteColorItem : AvaloniaRadioButton
{
    #region 公共属性定义

    public static readonly StyledProperty<Color?> ColorProperty =
        AvaloniaProperty.Register<PaletteColorItem, Color?>(nameof(Color));
    
    public Color? Color
    {
        get => GetValue(ColorProperty);
        set => SetValue(ColorProperty, value);
    }

    #endregion

    #region 内部属性定义

    internal static readonly StyledProperty<bool> IsMotionEnabledProperty =
        MotionAwareControlProperty.IsMotionEnabledProperty.AddOwner<PaletteColorItem>();
    
    internal static readonly StyledProperty<ITransform?> CheckedMarkRenderTransformProperty = 
        AvaloniaProperty.Register<PaletteColorItem, ITransform?>(nameof (CheckedMarkRenderTransform));
    
    internal static readonly DirectProperty<PaletteColorItem, bool> IsLightColorProperty =
        AvaloniaProperty.RegisterDirect<PaletteColorItem, bool>(
            nameof(IsLightColor),
            o => o.IsLightColor,
            (o, v) => o.IsLightColor = v);
    
    internal bool IsMotionEnabled
    {
        get => GetValue(IsMotionEnabledProperty);
        set => SetValue(IsMotionEnabledProperty, value);
    }
    
    internal ITransform? CheckedMarkRenderTransform
    {
        get => GetValue(CheckedMarkRenderTransformProperty);
        set => SetValue(CheckedMarkRenderTransformProperty, value);
    }
        
    private bool _isLightColor;

    internal bool IsLightColor
    {
        get => _isLightColor;
        set => SetAndRaise(IsLightColorProperty, ref _isLightColor, value);
    }
    #endregion
    
    private IDisposable? _borderThicknessDisposable;
    
    static PaletteColorItem()
    {
        AffectsRender<PaletteColorItem>(ColorProperty, IsLightColorProperty);
    }

    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        base.OnPropertyChanged(change);
        if (change.Property == ColorProperty)
        {
            if (Color != null)
            {
                SetCurrentValue(BackgroundProperty, new SolidColorBrush(Color.Value));
                var luminance = ColorHelper.GetRelativeLuminance(Color.Value);
                SetCurrentValue(IsLightColorProperty, luminance > 0.5);
            }
        }
        
        if (IsLoaded)
        {
            if (change.Property == IsMotionEnabledProperty)
            {
                ConfigureTransitions(true);
            }
        }
    }
    
    protected override void OnAttachedToVisualTree(VisualTreeAttachmentEventArgs e)
    {
        base.OnAttachedToVisualTree(e);
        _borderThicknessDisposable = TokenResourceBinder.CreateTokenBinding(this, BorderThicknessProperty,
            SharedTokenKey.BorderThickness,
            BindingPriority.Template,
            new RenderScaleAwareThicknessConfigure(this));
    }

    protected override void OnDetachedFromVisualTree(VisualTreeAttachmentEventArgs e)
    {
        base.OnDetachedFromVisualTree(e);
        _borderThicknessDisposable?.Dispose();
    }
    
    private void ConfigureTransitions(bool force)
    {
        if (IsMotionEnabled)
        {
            if (force || Transitions == null)
            {
                Transitions = [
                    TransitionUtils.CreateTransition<SolidColorBrushTransition>(BorderBrushProperty),
                    TransitionUtils.CreateTransition<TransformOperationsTransition>(CheckedMarkRenderTransformProperty, SharedTokenKey.MotionDurationMid,
                        new BackEaseOut())
                ];
            }
        }
        else
        {
            Transitions = null;
        }
    }
    
    protected override void OnLoaded(RoutedEventArgs e)
    {
        base.OnLoaded(e);
        ConfigureTransitions(false);
    }

    protected override void OnUnloaded(RoutedEventArgs e)
    {
        base.OnUnloaded(e);
        Transitions = null;
    }
}