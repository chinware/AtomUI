using System.Reactive.Disposables;
using AtomUI.Controls.Utils;
using AtomUI.Data;
using AtomUI.IconPkg;
using AtomUI.Media;
using AtomUI.Theme;
using AtomUI.Theme.Data;
using AtomUI.Theme.Styling;
using AtomUI.Theme.Utils;
using Avalonia;
using Avalonia.Animation;
using Avalonia.Controls;
using Avalonia.Controls.Metadata;
using Avalonia.Controls.Primitives;
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

[PseudoClasses(IconOnlyPC, LoadingPC)]
public class Button : AvaloniaButton,
                      ISizeTypeAware,
                      IWaveAdornerInfoProvider,
                      IAnimationAwareControl,
                      IControlSharedTokenResourcesHost,
                      ITokenResourceConsumer
{
    public const string IconOnlyPC = ":icononly";
    public const string LoadingPC = ":loading";

    #region 公共属性定义

    public static readonly StyledProperty<ButtonType> ButtonTypeProperty =
        AvaloniaProperty.Register<Button, ButtonType>(nameof(ButtonType));

    public static readonly StyledProperty<ButtonShape> ButtonShapeProperty =
        AvaloniaProperty.Register<Button, ButtonShape>(nameof(Shape));

    public static readonly StyledProperty<bool> IsDangerProperty =
        AvaloniaProperty.Register<Button, bool>(nameof(IsDanger));

    public static readonly StyledProperty<bool> IsGhostProperty =
        AvaloniaProperty.Register<Button, bool>(nameof(IsGhost));

    public static readonly StyledProperty<bool> IsLoadingProperty =
        AvaloniaProperty.Register<Button, bool>(nameof(IsLoading));

    public static readonly StyledProperty<ButtonSizeType> SizeTypeProperty =
        SizeTypeAwareControlProperty.SizeTypeProperty.AddOwner<Button>();

    public static readonly StyledProperty<Icon?> IconProperty
        = AvaloniaProperty.Register<Button, Icon?>(nameof(Icon));

    public static readonly StyledProperty<string?> TextProperty
        = AvaloniaProperty.Register<Button, string?>(nameof(Text));

    public static readonly StyledProperty<bool> IsIconVisibleProperty
        = AvaloniaProperty.Register<Button, bool>(nameof(IsIconVisible), true);

    public static readonly StyledProperty<bool> IsMotionEnabledProperty
        = AnimationAwareControlProperty.IsMotionEnabledProperty.AddOwner<Button>();

    public static readonly StyledProperty<bool> IsWaveAnimationEnabledProperty
        = AnimationAwareControlProperty.IsWaveAnimationEnabledProperty.AddOwner<Button>();

    public ButtonType ButtonType
    {
        get => GetValue(ButtonTypeProperty);
        set => SetValue(ButtonTypeProperty, value);
    }

    public ButtonShape Shape
    {
        get => GetValue(ButtonShapeProperty);
        set => SetValue(ButtonShapeProperty, value);
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

    public string? Text
    {
        get => GetValue(TextProperty);
        set => SetValue(TextProperty, value);
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

    public bool IsWaveAnimationEnabled
    {
        get => GetValue(IsWaveAnimationEnabledProperty);
        set => SetValue(IsWaveAnimationEnabledProperty, value);
    }

    #endregion

    #region 内部属性定义

    internal static readonly StyledProperty<double> ControlHeightTokenProperty =
        AvaloniaProperty.Register<Button, double>(
            nameof(ControlHeight));

    internal static readonly StyledProperty<Thickness> IconMarginProperty =
        AvaloniaProperty.Register<Button, Thickness>(
            nameof(IconMargin));

    internal static readonly StyledProperty<double> IconSizeProperty =
        AvaloniaProperty.Register<Button, double>(
            nameof(IconSize));

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

    internal static readonly StyledProperty<Thickness> EffectiveBorderThicknessProperty =
        AvaloniaProperty.Register<Button, Thickness>(
            nameof(EffectiveBorderThickness));
    
    internal static readonly DirectProperty<Button, IBrush?> IconNormalColorProperty =
        AvaloniaProperty.RegisterDirect<Button, IBrush?>(nameof(IconNormalColor),
            o => o.IconNormalColor,
            (o, v) => o.IconNormalColor = v);
    
    internal static readonly DirectProperty<Button, IBrush?> IconHoverColorProperty =
        AvaloniaProperty.RegisterDirect<Button, IBrush?>(nameof(IconHoverColor),
            o => o.IconHoverColor,
            (o, v) => o.IconHoverColor = v);
    
    internal static readonly DirectProperty<Button, IBrush?> IconPressedColorProperty =
        AvaloniaProperty.RegisterDirect<Button, IBrush?>(nameof(IconPressedColor),
            o => o.IconPressedColor,
            (o, v) => o.IconPressedColor = v);
    
    internal static readonly DirectProperty<Button, IBrush?> IconDisabledColorProperty =
        AvaloniaProperty.RegisterDirect<Button, IBrush?>(nameof(IconDisabledColor),
            o => o.IconDisabledColor,
            (o, v) => o.IconDisabledColor = v);
    
    internal static readonly DirectProperty<Button, bool> ExtraContainerVisibleProperty =
        AvaloniaProperty.RegisterDirect<Button, bool>(nameof(ExtraContainerVisible),
            o => o.ExtraContainerVisible,
            (o, v) => o.ExtraContainerVisible = v);

    internal double ControlHeight
    {
        get => GetValue(ControlHeightTokenProperty);
        set => SetValue(ControlHeightTokenProperty, value);
    }

    internal Thickness IconMargin
    {
        get => GetValue(IconMarginProperty);
        set => SetValue(IconMarginProperty, value);
    }

    internal double IconSize
    {
        get => GetValue(IconSizeProperty);
        set => SetValue(IconSizeProperty, value);
    }

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

    public Thickness EffectiveBorderThickness
    {
        get => GetValue(EffectiveBorderThicknessProperty);
        set => SetValue(EffectiveBorderThicknessProperty, value);
    }
    
    private IBrush? _iconNormalColor;

    internal IBrush? IconNormalColor
    {
        get => _iconNormalColor;
        set => SetAndRaise(IconNormalColorProperty, ref _iconNormalColor, value);
    }

    private IBrush? _iconHoverColor;

    internal IBrush? IconHoverColor
    {
        get => _iconHoverColor;
        set => SetAndRaise(IconHoverColorProperty, ref _iconHoverColor, value);
    }
    
    private IBrush? _iconPressedColor;

    internal IBrush? IconPressedColor
    {
        get => _iconPressedColor;
        set => SetAndRaise(IconPressedColorProperty, ref _iconPressedColor, value);
    }
    
    private IBrush? _iconDisabledColor;

    internal IBrush? IconDisabledColor
    {
        get => _iconDisabledColor;
        set => SetAndRaise(IconDisabledColorProperty, ref _iconDisabledColor, value);
    }
    
    private bool _extraContainerVisible;

    internal bool ExtraContainerVisible
    {
        get => _extraContainerVisible;
        set => SetAndRaise(ExtraContainerVisibleProperty, ref _extraContainerVisible, value);
    }
    
    Control IAnimationAwareControl.PropertyBindTarget => this;
    Control IControlSharedTokenResourcesHost.HostControl => this;
    string IControlSharedTokenResourcesHost.TokenId => ButtonToken.ID;
    CompositeDisposable? ITokenResourceConsumer.TokenBindingsDisposable => _tokenBindingsDisposable;

    #endregion
    
    private CompositeDisposable? _tokenBindingsDisposable;
    private Border? _frame;

    static Button()
    {
        AffectsMeasure<Button>(SizeTypeProperty,
            ButtonShapeProperty,
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
        this.BindAnimationProperties(IsMotionEnabledProperty, IsWaveAnimationEnabledProperty);
    }

    protected override Size MeasureOverride(Size availableSize)
    {
        var size         = base.MeasureOverride(availableSize); 
        var targetWidth  = size.Width;
        var targetHeight = size.Height;

        targetHeight = Math.Max(targetHeight, ControlHeight);

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
        _tokenBindingsDisposable = new CompositeDisposable();
        SetupControlThemeBindings();
        base.OnAttachedToLogicalTree(e);
      
        if (Text is null && Content is string content)
        {
            Text    = content;
            Content = null;
        }
        
        SetupShapeStyleBindings();
        SetupIconBrush();
        UpdatePseudoClasses();
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
                IsWaveAnimationEnabled &&
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
            e.Property == TextProperty ||
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
        else if (e.Property == RightExtraContentProperty)
        {
            ExtraContainerVisible = SetupExtraContainerVisible();
        }

        if (this.IsAttachedToVisualTree())
        {
            if (e.Property == IconProperty)
            {
                if (e.OldValue is Icon oldIcon)
                {
                    oldIcon.SetTemplatedParent(null);
                }
                SetupIcon();
                SetupIconBrush();
            }
            else if (e.Property == IsDangerProperty ||
                  e.Property == IsGhostProperty ||
                  e.Property == ButtonTypeProperty)
            {
                SetupShadows();
                SetupIconBrush();
            }
            else if (e.Property == ButtonTypeProperty)
            {
                SetupControlThemeBindings();
            }
            else if (e.Property == ButtonShapeProperty)
            {
                SetupShapeStyleBindings();
            }
        }

        if (this.IsAttachedToVisualTree())
        {
            if (e.Property == IsMotionEnabledProperty ||
                e.Property == IsWaveAnimationEnabledProperty)
            {
                SetupTransitions();
            }
        }
    }

    protected virtual bool SetupExtraContainerVisible()
    {
        return RightExtraContent != null;
    }

    protected virtual void SetupControlThemeBindings()
    {
        if (ButtonType == ButtonType.Default)
        {
            this.AddTokenBindingDisposable(
                TokenResourceBinder.CreateTokenBinding(this, ThemeProperty, DefaultButtonTheme.ID));
        }
        else if (ButtonType == ButtonType.Primary)
        {
            this.AddTokenBindingDisposable(
                TokenResourceBinder.CreateTokenBinding(this, ThemeProperty, PrimaryButtonTheme.ID));
        }
        else if (ButtonType == ButtonType.Text)
        {
            this.AddTokenBindingDisposable(
                TokenResourceBinder.CreateTokenBinding(this, ThemeProperty, TextButtonTheme.ID));
        }
        else if (ButtonType == ButtonType.Link)
        {
            this.AddTokenBindingDisposable(
                TokenResourceBinder.CreateTokenBinding(this, ThemeProperty, LinkButtonTheme.ID));
        }
    }

    private void SetupShapeStyleBindings()
    {
        if (Shape == ButtonShape.Circle)
        {
            this.AddTokenBindingDisposable(
                TokenResourceBinder.CreateTokenBinding(this, PaddingProperty, ButtonTokenKey.CirclePadding));
        }
    }

    private void SetupTransitions()
    {
        if (IsMotionEnabled)
        {
            if (Transitions is null)
            {
                var transitions = new Transitions();
                if (ButtonType == ButtonType.Primary)
                {
                    if (IsGhost)
                    {
                        transitions.Add(
                            AnimationUtils.CreateTransition<SolidColorBrushTransition>(BorderBrushProperty));
                        transitions.Add(AnimationUtils.CreateTransition<SolidColorBrushTransition>(ForegroundProperty));
                    }
                }
                else if (ButtonType == ButtonType.Default)
                {
                    transitions.Add(AnimationUtils.CreateTransition<SolidColorBrushTransition>(BorderBrushProperty));
                    transitions.Add(AnimationUtils.CreateTransition<SolidColorBrushTransition>(ForegroundProperty));
                }
                else if (ButtonType == ButtonType.Link)
                {
                    transitions.Add(AnimationUtils.CreateTransition<SolidColorBrushTransition>(ForegroundProperty));
                }

                Transitions = transitions;
            }
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
        // 为了防止意外被用户改变背景，做了一个 frame
        _frame = e.NameScope.Find<Border>(BaseButtonTheme.FramePart);
        SetupShadows();
        UpdatePseudoClasses();
        SetupIcon();
        SetupTransitions();
    }

    protected override void OnAttachedToVisualTree(VisualTreeAttachmentEventArgs e)
    {
        base.OnAttachedToVisualTree(e);
        this.AddTokenBindingDisposable(TokenResourceBinder.CreateTokenBinding(this, BorderThicknessProperty,
            SharedTokenKey.BorderThickness,
            BindingPriority.Template,
            new RenderScaleAwareThicknessConfigure(this)));
        SetupEffectiveBorderThickness();
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

    private void SetupIcon()
    {
        if (Icon is not null)
        {
            BindUtils.RelayBind(this, IconSizeProperty, Icon, WidthProperty);
            BindUtils.RelayBind(this, IconSizeProperty, Icon, HeightProperty);
            BindUtils.RelayBind(this, IconMarginProperty, Icon, MarginProperty);
            BindUtils.RelayBind(this, IconNormalColorProperty, Icon, Icon.NormalFilledBrushProperty);
            BindUtils.RelayBind(this, IconHoverColorProperty, Icon, Icon.ActiveFilledBrushProperty);
            BindUtils.RelayBind(this, IconPressedColorProperty, Icon, Icon.SelectedFilledBrushProperty);
            BindUtils.RelayBind(this, IconDisabledColorProperty, Icon, Icon.DisabledFilledBrushProperty);
            Icon.SetTemplatedParent(this);
        }
    }

    private void SetupIconBrush()
    {
        var normalFilledBrushKey   = ButtonTokenKey.DefaultColor;
        var selectedFilledBrushKey = ButtonTokenKey.DefaultActiveColor;
        var activeFilledBrushKey   = ButtonTokenKey.DefaultHoverColor;
        var disabledFilledBrushKey = SharedTokenKey.ColorTextDisabled;
        if (ButtonType == ButtonType.Default)
        {
            if (IsGhost)
            {
                normalFilledBrushKey   = SharedTokenKey.ColorTextLightSolid;
                selectedFilledBrushKey = SharedTokenKey.ColorPrimaryActive;
                activeFilledBrushKey   = SharedTokenKey.ColorPrimaryHover;
            }

            if (IsDanger)
            {
                normalFilledBrushKey   = SharedTokenKey.ColorError;
                selectedFilledBrushKey = SharedTokenKey.ColorErrorActive;
                activeFilledBrushKey   = SharedTokenKey.ColorErrorBorderHover;
            }
        }
        else if (ButtonType == ButtonType.Primary)
        {
            normalFilledBrushKey   = ButtonTokenKey.PrimaryColor;
            selectedFilledBrushKey = ButtonTokenKey.PrimaryColor;
            activeFilledBrushKey   = ButtonTokenKey.PrimaryColor;
            if (IsGhost)
            {
                normalFilledBrushKey   = SharedTokenKey.ColorPrimary;
                selectedFilledBrushKey = SharedTokenKey.ColorPrimaryActive;
                activeFilledBrushKey   = SharedTokenKey.ColorPrimaryHover;
                if (IsDanger)
                {
                    normalFilledBrushKey   = SharedTokenKey.ColorError;
                    selectedFilledBrushKey = SharedTokenKey.ColorErrorActive;
                    activeFilledBrushKey   = SharedTokenKey.ColorErrorBorderHover;
                }
            }
        }
        else if (ButtonType == ButtonType.Text)
        {
            normalFilledBrushKey   = ButtonTokenKey.DefaultColor;
            selectedFilledBrushKey = ButtonTokenKey.DefaultColor;
            activeFilledBrushKey   = ButtonTokenKey.DefaultColor;
            if (IsDanger)
            {
                normalFilledBrushKey   = SharedTokenKey.ColorError;
                selectedFilledBrushKey = SharedTokenKey.ColorErrorActive;
                activeFilledBrushKey   = SharedTokenKey.ColorErrorBorderHover;
            }
        }
        else if (ButtonType == ButtonType.Link)
        {
            normalFilledBrushKey   = SharedTokenKey.ColorLink;
            selectedFilledBrushKey = SharedTokenKey.ColorLinkActive;
            activeFilledBrushKey   = SharedTokenKey.ColorLinkHover;

            if (IsDanger)
            {
                normalFilledBrushKey   = SharedTokenKey.ColorError;
                selectedFilledBrushKey = SharedTokenKey.ColorErrorActive;
                activeFilledBrushKey   = SharedTokenKey.ColorErrorHover;
            }
        }
        
        this.AddTokenBindingDisposable(TokenResourceBinder.CreateTokenBinding(this, IconNormalColorProperty,
            normalFilledBrushKey));
        this.AddTokenBindingDisposable(TokenResourceBinder.CreateTokenBinding(this, IconHoverColorProperty,
            activeFilledBrushKey));
        this.AddTokenBindingDisposable(TokenResourceBinder.CreateTokenBinding(this, IconPressedColorProperty,
            selectedFilledBrushKey));
        this.AddTokenBindingDisposable(TokenResourceBinder.CreateTokenBinding(this,
            IconDisabledColorProperty,
            disabledFilledBrushKey));
        
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
        PseudoClasses.Set(IconOnlyPC, Icon is not null && Text is null);
        PseudoClasses.Set(LoadingPC, IsLoading);
    }
}