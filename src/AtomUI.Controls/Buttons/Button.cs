using System.Reactive.Disposables;
using AtomUI.Animations;
using AtomUI.Controls.Themes;
using AtomUI.Controls.Utils;
using AtomUI.IconPkg;
using AtomUI.Theme;
using AtomUI.Theme.Data;
using AtomUI.Theme.Styling;
using AtomUI.Theme.Utils;
using Avalonia;
using Avalonia.Animation;
using Avalonia.Controls;
using Avalonia.Controls.Metadata;
using Avalonia.Controls.Primitives;
using Avalonia.Controls.Templates;
using Avalonia.Data;
using Avalonia.Layout;
using Avalonia.LogicalTree;
using Avalonia.Media;
using Avalonia.VisualTree;

namespace AtomUI.Controls;

using AvaloniaButton = Avalonia.Controls.Button;
using ButtonSizeType = SizeType;

public enum ButtonType
{
    Default,
    Primary,
    Link,
    Text
}

public enum ButtonShape
{
    Default,
    Circle,
    Round
}

[PseudoClasses(
    ButtonPseudoClass.IconOnly,
    ButtonPseudoClass.Loading, 
    ButtonPseudoClass.IsDanger,
    ButtonPseudoClass.DefaultType,
    ButtonPseudoClass.PrimaryType,
    ButtonPseudoClass.LinkType,
    ButtonPseudoClass.TextType)]
public class Button : AvaloniaButton,
                      ISizeTypeAware,
                      IWaveAdornerInfoProvider,
                      IWaveSpiritAwareControl,
                      IControlSharedTokenResourcesHost,
                      IResourceBindingManager
{
    #region 公共属性定义

    public static readonly StyledProperty<ButtonType> ButtonTypeProperty =
        AvaloniaProperty.Register<Button, ButtonType>(nameof(ButtonType));

    public static readonly StyledProperty<ButtonShape> ShapeProperty =
        AvaloniaProperty.Register<Button, ButtonShape>(nameof(Shape));

    public static readonly StyledProperty<bool> IsDangerProperty =
        AvaloniaProperty.Register<Button, bool>(nameof(IsDanger));

    public static readonly StyledProperty<bool> IsGhostProperty =
        AvaloniaProperty.Register<Button, bool>(nameof(IsGhost));

    public static readonly StyledProperty<bool> IsLoadingProperty =
        AvaloniaProperty.Register<Button, bool>(nameof(IsLoading));

    public static readonly StyledProperty<ButtonSizeType> SizeTypeProperty =
        SizeTypeAwareControlProperty.SizeTypeProperty.AddOwner<Button>();

    public static readonly StyledProperty<Icon?> IconProperty = 
        AvaloniaProperty.Register<Button, Icon?>(nameof(Icon));

    public static readonly StyledProperty<bool> IsIconVisibleProperty = 
        AvaloniaProperty.Register<Button, bool>(nameof(IsIconVisible), true);

    public static readonly StyledProperty<bool> IsMotionEnabledProperty = 
        MotionAwareControlProperty.IsMotionEnabledProperty.AddOwner<Button>();

    public static readonly StyledProperty<bool> IsWaveSpiritEnabledProperty = 
        WaveSpiritAwareControlProperty.IsWaveSpiritEnabledProperty.AddOwner<Button>();

    public ButtonType ButtonType
    {
        get => GetValue(ButtonTypeProperty);
        set => SetValue(ButtonTypeProperty, value);
    }

    public ButtonShape Shape
    {
        get => GetValue(ShapeProperty);
        set => SetValue(ShapeProperty, value);
    }

    public bool IsDanger
    {
        get => GetValue(IsDangerProperty);
        set => SetValue(IsDangerProperty, value);
    }

    public bool IsGhost
    {
        get => GetValue(IsGhostProperty);
        set => SetValue(IsGhostProperty, value);
    }

    public bool IsLoading
    {
        get => GetValue(IsLoadingProperty);
        set => SetValue(IsLoadingProperty, value);
    }

    public ButtonSizeType SizeType
    {
        get => GetValue(SizeTypeProperty);
        set => SetValue(SizeTypeProperty, value);
    }

    public Icon? Icon
    {
        get => GetValue(IconProperty);
        set => SetValue(IconProperty, value);
    }

    public bool IsIconVisible
    {
        get => GetValue(IsIconVisibleProperty);
        set => SetValue(IsIconVisibleProperty, value);
    }

    public bool IsMotionEnabled
    {
        get => GetValue(IsMotionEnabledProperty);
        set => SetValue(IsMotionEnabledProperty, value);
    }

    public bool IsWaveSpiritEnabled
    {
        get => GetValue(IsWaveSpiritEnabledProperty);
        set => SetValue(IsWaveSpiritEnabledProperty, value);
    }

    #endregion

    #region 内部属性定义

    internal static readonly StyledProperty<BoxShadow> DefaultShadowProperty =
        AvaloniaProperty.Register<Button, BoxShadow>(
            nameof(DefaultShadow));

    internal static readonly StyledProperty<BoxShadow> PrimaryShadowProperty =
        AvaloniaProperty.Register<Button, BoxShadow>(
            nameof(PrimaryShadow));

    internal static readonly StyledProperty<BoxShadow> DangerShadowProperty =
        AvaloniaProperty.Register<Button, BoxShadow>(
            nameof(DangerShadow));

    internal static readonly StyledProperty<object?> RightExtraContentProperty =
        AvaloniaProperty.Register<Button, object?>(nameof(RightExtraContent));
    
    internal static readonly StyledProperty<IDataTemplate?> RightExtraContentTemplateProperty =
        AvaloniaProperty.Register<ContentControl, IDataTemplate?>(nameof(RightExtraContentTemplate));

    internal static readonly StyledProperty<Thickness> EffectiveBorderThicknessProperty =
        AvaloniaProperty.Register<Button, Thickness>(
            nameof(EffectiveBorderThickness));
    
    internal BoxShadow DefaultShadow
    {
        get => GetValue(DefaultShadowProperty);
        set => SetValue(DefaultShadowProperty, value);
    }

    internal BoxShadow PrimaryShadow
    {
        get => GetValue(PrimaryShadowProperty);
        set => SetValue(PrimaryShadowProperty, value);
    }

    internal BoxShadow DangerShadow
    {
        get => GetValue(DangerShadowProperty);
        set => SetValue(DangerShadowProperty, value);
    }

    public object? RightExtraContent
    {
        get => GetValue(RightExtraContentProperty);
        set => SetValue(RightExtraContentProperty, value);
    }

    public IDataTemplate? RightExtraContentTemplate
    {
        get => GetValue(RightExtraContentTemplateProperty);
        set => SetValue(RightExtraContentTemplateProperty, value);
    }
    
    public Thickness EffectiveBorderThickness
    {
        get => GetValue(EffectiveBorderThicknessProperty);
        set => SetValue(EffectiveBorderThicknessProperty, value);
    }
    
    Control IMotionAwareControl.PropertyBindTarget => this;
    Control IControlSharedTokenResourcesHost.HostControl => this;
    string IControlSharedTokenResourcesHost.TokenId => ButtonToken.ID;
    CompositeDisposable? IResourceBindingManager.ResourceBindingsDisposable => _resourceBindingsDisposable;

    #endregion
    
    private CompositeDisposable? _resourceBindingsDisposable;
    private Border? _frame;

    static Button()
    {
        AffectsMeasure<Button>(SizeTypeProperty,
            ShapeProperty,
            IconProperty,
            WidthProperty,
            HeightProperty,
            PaddingProperty);
        AffectsRender<Button>(ButtonTypeProperty,
            IsDangerProperty,
            IsGhostProperty,
            BackgroundProperty,
            ForegroundProperty);
        HorizontalAlignmentProperty.OverrideDefaultValue<Button>(HorizontalAlignment.Left);
        VerticalAlignmentProperty.OverrideDefaultValue<Button>(VerticalAlignment.Center);
    }

    public Button()
    {
        this.RegisterResources();
        this.BindWaveSpiritProperties();
    }

    protected override Size MeasureOverride(Size availableSize)
    {
        var size         = base.MeasureOverride(availableSize); 
        var targetWidth  = size.Width;
        var targetHeight = size.Height;

        targetWidth = Math.Max(targetWidth, targetHeight);

        if (Shape == ButtonShape.Circle)
        {
            targetWidth  = targetHeight;
            CornerRadius = new CornerRadius(targetHeight);
        }
        else if (Shape == ButtonShape.Round)
        {
            CornerRadius = new CornerRadius(targetHeight);
            targetWidth  = Math.Max(targetWidth, targetHeight + targetHeight / 2);
        }
      
        return new Size(targetWidth, targetHeight);
    }

    protected override void OnAttachedToLogicalTree(LogicalTreeAttachmentEventArgs e)
    {
        _resourceBindingsDisposable = new CompositeDisposable();
        SetupControlThemeBindings();
        base.OnAttachedToLogicalTree(e);
    }

    private void SetupShadows()
    {
        if (ButtonType == ButtonType.Default)
        {
            if (IsDanger)
            {
                Effect = new DropShadowEffect
                {
                    OffsetX    = DangerShadow.OffsetX,
                    OffsetY    = DangerShadow.OffsetY,
                    Color      = DangerShadow.Color,
                    BlurRadius = DangerShadow.Blur
                };
            }
            else
            {
                Effect = new DropShadowEffect
                {
                    OffsetX    = DefaultShadow.OffsetX,
                    OffsetY    = DefaultShadow.OffsetY,
                    Color      = DefaultShadow.Color,
                    BlurRadius = DefaultShadow.Blur
                };
            }
        }
        else if (ButtonType == ButtonType.Primary)
        {
            if (IsDanger)
            {
                Effect = new DropShadowEffect
                {
                    OffsetX    = DangerShadow.OffsetX,
                    OffsetY    = DangerShadow.OffsetY,
                    Color      = DangerShadow.Color,
                    BlurRadius = DangerShadow.Blur
                };
            }
            else
            {
                Effect = new DropShadowEffect
                {
                    OffsetX    = PrimaryShadow.OffsetX,
                    OffsetY    = PrimaryShadow.OffsetY,
                    Color      = PrimaryShadow.Color,
                    BlurRadius = PrimaryShadow.Blur
                };
            }
        }
    }

    protected override void OnDetachedFromLogicalTree(LogicalTreeAttachmentEventArgs e)
    {
        base.OnDetachedFromLogicalTree(e);
        this.DisposeTokenBindings();
    }

    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs e)
    {
        base.OnPropertyChanged(e);
        if (e.Property == IsPressedProperty)
        {
            if (!IsLoading &&
                IsWaveSpiritEnabled &&
                (e.OldValue as bool? == true) &&
                (ButtonType == ButtonType.Primary || ButtonType == ButtonType.Default))
            {
                WaveType waveType = default;
                if (Shape == ButtonShape.Default)
                {
                    waveType = WaveType.RoundRectWave;
                }
                else if (Shape == ButtonShape.Round)
                {
                    waveType = WaveType.PillWave;
                }
                else if (Shape == ButtonShape.Circle)
                {
                    waveType = WaveType.CircleWave;
                }

                Color? waveColor = null;
                if (IsDanger)
                {
                    if (ButtonType == ButtonType.Primary && !IsGhost)
                    {
                        waveColor = Color.Parse(_frame?.Background?.ToString()!);
                    }
                    else
                    {
                        waveColor = Color.Parse(Foreground?.ToString()!);
                    }
                }

                WaveSpiritAdorner.ShowWaveAdorner(this, waveType, waveColor);
            }
        }

        if (e.Property == ContentProperty ||
            e.Property == IsLoadingProperty)
        {
            UpdatePseudoClasses();
        }
        else if (e.Property == BorderBrushProperty ||
                 e.Property == ButtonTypeProperty ||
                 e.Property == IsEnabledProperty)
        {
            SetupEffectiveBorderThickness();
        }

        if (this.IsAttachedToVisualTree())
        { 
            if (e.Property == IsDangerProperty ||
              e.Property == IsGhostProperty ||
              e.Property == ButtonTypeProperty)
            {
                SetupShadows();
            }
            else if (e.Property == ButtonTypeProperty)
            {
                SetupControlThemeBindings();
            }
            else if (e.Property == IsMotionEnabledProperty ||
                     e.Property == IsWaveSpiritEnabledProperty)
            {
                ConfigureTransitions();
            }
        }
        
    }

    protected virtual void SetupControlThemeBindings()
    {
        if (ButtonType == ButtonType.Default)
        {
            this.AddResourceBindingDisposable(
                TokenResourceBinder.CreateTokenBinding(this, ThemeProperty, DefaultButtonTheme.ID));
        }
        else if (ButtonType == ButtonType.Primary)
        {
            this.AddResourceBindingDisposable(
                TokenResourceBinder.CreateTokenBinding(this, ThemeProperty, PrimaryButtonTheme.ID));
        }
        else if (ButtonType == ButtonType.Text)
        {
            this.AddResourceBindingDisposable(
                TokenResourceBinder.CreateTokenBinding(this, ThemeProperty, TextButtonTheme.ID));
        }
        else if (ButtonType == ButtonType.Link)
        {
            this.AddResourceBindingDisposable(
                TokenResourceBinder.CreateTokenBinding(this, ThemeProperty, LinkButtonTheme.ID));
        }
    }

    private void ConfigureTransitions()
    {
        if (IsMotionEnabled)
        {
            if (_frame != null)
            {
                _frame.Transitions ??= new Transitions();
                _frame.Transitions.Add(TransitionUtils.CreateTransition<SolidColorBrushTransition>(BackgroundProperty));
            }
            
            if (Transitions is null)
            {
                var transitions = new Transitions();
                if (ButtonType == ButtonType.Primary)
                {
                    if (IsGhost)
                    {
                        transitions.Add(
                            TransitionUtils.CreateTransition<SolidColorBrushTransition>(BorderBrushProperty));
                        transitions.Add(TransitionUtils.CreateTransition<SolidColorBrushTransition>(ForegroundProperty));
                    }
                }
                else if (ButtonType == ButtonType.Default)
                {
                    transitions.Add(TransitionUtils.CreateTransition<SolidColorBrushTransition>(BorderBrushProperty));
                    transitions.Add(TransitionUtils.CreateTransition<SolidColorBrushTransition>(ForegroundProperty));
                }
                else if (ButtonType == ButtonType.Link)
                {
                    transitions.Add(TransitionUtils.CreateTransition<SolidColorBrushTransition>(ForegroundProperty));
                }

                Transitions = transitions;
            }
        }
        else
        {
            Transitions?.Clear();
            Transitions = null;
            if (_frame != null)
            {
                _frame.Transitions?.Clear();
                _frame.Transitions = null;
            }
        }
    }

    protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
    {
        base.OnApplyTemplate(e);
        // 为了防止意外被用户改变背景，做了一个 frame
        _frame = e.NameScope.Find<Border>(ButtonThemeConstants.FramePart);
        ConfigureTransitions();
    }

    protected override void OnAttachedToVisualTree(VisualTreeAttachmentEventArgs e)
    {
        base.OnAttachedToVisualTree(e);
        this.AddResourceBindingDisposable(TokenResourceBinder.CreateTokenBinding(this, BorderThicknessProperty,
            SharedTokenKey.BorderThickness,
            BindingPriority.Template,
            new RenderScaleAwareThicknessConfigure(this)));
        SetupEffectiveBorderThickness();
        SetupShadows();
        UpdatePseudoClasses();
    }

    private void SetupEffectiveBorderThickness()
    {
        if (ButtonType == ButtonType.Default)
        {
            EffectiveBorderThickness = BorderThickness;
        }
        else if (ButtonType == ButtonType.Primary)
        {
            if (IsGhost || !IsEnabled)
            {
                EffectiveBorderThickness = BorderThickness;
            }
            else
            {
                EffectiveBorderThickness = new Thickness(0);
            }
        }
        else
        {
            EffectiveBorderThickness = new Thickness(0);
        }
    }

    public Rect WaveGeometry()
    {
        return new Rect(0, 0, Bounds.Width, Bounds.Height);
    }

    public CornerRadius WaveBorderRadius()
    {
        return CornerRadius;
    }

    private void UpdatePseudoClasses()
    {
        PseudoClasses.Set(ButtonPseudoClass.IconOnly, Icon is not null && Content is null);
        PseudoClasses.Set(ButtonPseudoClass.Loading, IsLoading);
        PseudoClasses.Set(ButtonPseudoClass.DefaultType, ButtonType == ButtonType.Default);
        PseudoClasses.Set(ButtonPseudoClass.PrimaryType, ButtonType == ButtonType.Primary);
        PseudoClasses.Set(ButtonPseudoClass.LinkType, ButtonType == ButtonType.Link);
        PseudoClasses.Set(ButtonPseudoClass.TextType, ButtonType == ButtonType.Text);
        PseudoClasses.Set(ButtonPseudoClass.IsDanger, IsDanger);
    }
}