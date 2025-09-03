using System.Reactive.Disposables;
using AtomUI.Animations;
using AtomUI.Controls.Themes;
using AtomUI.Controls.Utils;
using AtomUI.IconPkg;
using AtomUI.Theme;
using AtomUI.Theme.Data;
using AtomUI.Theme.Styling;
using Avalonia;
using Avalonia.Animation;
using Avalonia.Animation.Easings;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Data;
using Avalonia.Interactivity;
using Avalonia.LogicalTree;
using Avalonia.Media;

namespace AtomUI.Controls;

using AvaloniaRadioButton = Avalonia.Controls.RadioButton;

internal class PaletteColorItem : AvaloniaRadioButton, IResourceBindingManager
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
        
    private bool _isLightColor;

    internal bool IsLightColor
    {
        get => _isLightColor;
        set => SetAndRaise(IsLightColorProperty, ref _isLightColor, value);
    }
    #endregion
    
    CompositeDisposable? IResourceBindingManager.ResourceBindingsDisposable { get; set; }
    private Icon? _checkedMark;
    
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
                ConfigureCheckedMarkTransitions(true);
            }
        }
    }
    
    protected override void OnAttachedToLogicalTree(LogicalTreeAttachmentEventArgs e)
    {
        base.OnAttachedToLogicalTree(e);
        this.AddResourceBindingDisposable(TokenResourceBinder.CreateTokenBinding(this, BorderThicknessProperty,
            SharedTokenKey.BorderThickness,
            BindingPriority.Template,
            new RenderScaleAwareThicknessConfigure(this)));
    }

    protected override void OnDetachedFromLogicalTree(LogicalTreeAttachmentEventArgs e)
    {
        base.OnDetachedFromLogicalTree(e);
        this.DisposeTokenBindings();
    }
    
    protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
    {
        base.OnApplyTemplate(e);
        _checkedMark = e.NameScope.Find<Icon>(PaletteColorItemThemeConstants.CheckedMarkPart);
        if (_checkedMark != null)
        {
            _checkedMark.Loaded   += HandleCheckedMarkLoaded;
            _checkedMark.Unloaded += HandleCheckedMarkUnLoaded;
        }
    }
    
    private void HandleCheckedMarkLoaded(object? sender, RoutedEventArgs e)
    {
        ConfigureCheckedMarkTransitions(false);
    }

    private void HandleCheckedMarkUnLoaded(object? sender, RoutedEventArgs e)
    {
        if (_checkedMark != null)
        {
            _checkedMark.Transitions = null;
        }
    }
    
    private void ConfigureTransitions(bool force)
    {
        if (IsMotionEnabled)
        {
            if (force || Transitions == null)
            {
                Transitions = [
                    TransitionUtils.CreateTransition<SolidColorBrushTransition>(BorderBrushProperty),
                ];
            }
        }
        else
        {
            Transitions = null;
        }
    }
    
    private void ConfigureCheckedMarkTransitions(bool force)
    {
        if (IsMotionEnabled)
        {
            if (_checkedMark != null)
            {
                if (force || _checkedMark.Transitions == null)
                {
                    _checkedMark.Transitions = [
                        TransitionUtils.CreateTransition<TransformOperationsTransition>(RenderTransformProperty, SharedTokenKey.MotionDurationMid,
                            new BackEaseOut()),
                    ];
                }
            }
        }
        else
        {
            if (_checkedMark != null)
            {
                _checkedMark.Transitions = null;
            }
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