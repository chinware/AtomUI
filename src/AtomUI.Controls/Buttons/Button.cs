using AtomUI.Controls.Utils;
using AtomUI.Data;
using AtomUI.IconPkg;
using AtomUI.Media;
using AtomUI.Theme;
using AtomUI.Theme.Data;
using AtomUI.Theme.Styling;
using AtomUI.Theme.TokenSystem;
using Avalonia;
using Avalonia.Animation;
using Avalonia.Controls;
using Avalonia.Controls.Metadata;
using Avalonia.Controls.Primitives;
using Avalonia.Data;
using Avalonia.Layout;
using Avalonia.LogicalTree;
using Avalonia.Media;

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
// TODO 目前不能动态切换 ButtonType

[PseudoClasses(IconOnlyPC, LoadingPC)]
public class Button : AvaloniaButton, ISizeTypeAware, IWaveAdornerInfoProvider
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
        AvaloniaProperty.Register<Button, ButtonSizeType>(nameof(SizeType), ButtonSizeType.Middle);

    public static readonly StyledProperty<Icon?> IconProperty
        = AvaloniaProperty.Register<Button, Icon?>(nameof(Icon));

    public static readonly StyledProperty<string?> TextProperty
        = AvaloniaProperty.Register<Button, string?>(nameof(Text));

    public static readonly StyledProperty<bool> IsIconVisibleProperty
        = AvaloniaProperty.Register<Button, bool>(nameof(IsIconVisible), true);

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

    #endregion

    protected ControlStyleState _styleState;
    private bool _initialized;
    private Icon? _loadingIcon;
    private ControlTokenResourceRegister _controlTokenResourceRegister;
    
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
        _controlTokenResourceRegister = new ControlTokenResourceRegister(this, ButtonToken.ID);
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
        base.OnAttachedToLogicalTree(e);
        if (!_initialized)
        {
            _controlTokenResourceRegister.RegisterResources();
            SetupControlTheme();
            if (Text is null && Content is string content)
            {
                Text    = content;
                Content = null;
            }

            PseudoClasses.Set(IconOnlyPC, Icon is not null && Text is null);
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
            _initialized = true;
        }
    }

    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs e)
    {
        base.OnPropertyChanged(e);
        if (e.Property == IsPointerOverProperty ||
            e.Property == IsPressedProperty ||
            e.Property == IsEnabledProperty)
        {
            CollectStyleState();
            ApplyIconModeStyleConfig();
            if (e.Property == IsPressedProperty)
            {
                if (!IsLoading && _styleState.HasFlag(ControlStyleState.Raised) &&
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
                            waveColor = Color.Parse(Background?.ToString()!);
                        }
                        else
                        {
                            waveColor = Color.Parse(Foreground?.ToString()!);
                        }
                    }

                    WaveSpiritAdorner.ShowWaveAdorner(this, waveType, waveColor);
                }
            }
        }

        if (e.Property == ButtonTypeProperty)
        {
            if (VisualRoot is not null)
            {
                SetupControlTheme();
            }
        }
        else if (e.Property == ContentProperty ||
                 e.Property == TextProperty ||
                 e.Property == IsLoadingProperty)
        {
            UpdatePseudoClasses();
        }

        if (e.Property == BorderBrushProperty ||
            e.Property == ButtonTypeProperty ||
            e.Property == IsEnabledProperty)
        {
            SetupEffectiveBorderThickness();
        }

        if (VisualRoot is not null)
        {
            if (e.Property == IconProperty)
            {
                SetupIcon();
                SetupIconBrush();
            }

            if (e.Property == IsDangerProperty ||
                e.Property == IsGhostProperty ||
                e.Property == ButtonTypeProperty)
            {
                SetupIconBrush();
            }
        }
    }

    private void SetupControlTheme()
    {
        if (ButtonType == ButtonType.Default)
        {
            TokenResourceBinder.CreateTokenBinding(this, ThemeProperty, DefaultButtonTheme.ID);
        }
        else if (ButtonType == ButtonType.Primary)
        {
            TokenResourceBinder.CreateTokenBinding(this, ThemeProperty, PrimaryButtonTheme.ID);
        }
        else if (ButtonType == ButtonType.Text)
        {
            TokenResourceBinder.CreateTokenBinding(this, ThemeProperty, TextButtonTheme.ID);
        }
        else if (ButtonType == ButtonType.Link)
        {
            TokenResourceBinder.CreateTokenBinding(this, ThemeProperty, LinkButtonTheme.ID);
        }
    }

    private void ApplyShapeStyleConfig()
    {
        if (Shape == ButtonShape.Circle)
        {
            TokenResourceBinder.CreateTokenBinding(this, PaddingProperty, ButtonTokenKey.CirclePadding);
        }
    }

    private void CollectStyleState()
    {
        ControlStateUtils.InitCommonState(this, ref _styleState);
        if (IsPressed)
        {
            _styleState |= ControlStyleState.Sunken;
        }
        else
        {
            _styleState |= ControlStyleState.Raised;
        }
    }

    private void SetupTransitions()
    {
        if (Transitions is null)
        {
            var transitions = new Transitions();
            if (ButtonType == ButtonType.Primary)
            {
                transitions.Add(AnimationUtils.CreateTransition<SolidColorBrushTransition>(BackgroundProperty));
                if (IsGhost)
                {
                    transitions.Add(AnimationUtils.CreateTransition<SolidColorBrushTransition>(BorderBrushProperty));
                    transitions.Add(AnimationUtils.CreateTransition<SolidColorBrushTransition>(ForegroundProperty));
                }
            }
            else if (ButtonType == ButtonType.Default)
            {
                transitions.Add(AnimationUtils.CreateTransition<SolidColorBrushTransition>(BorderBrushProperty));
                transitions.Add(AnimationUtils.CreateTransition<SolidColorBrushTransition>(ForegroundProperty));
            }
            else if (ButtonType == ButtonType.Text)
            {
                transitions.Add(AnimationUtils.CreateTransition<SolidColorBrushTransition>(BackgroundProperty));
            }
            else if (ButtonType == ButtonType.Link)
            {
                transitions.Add(AnimationUtils.CreateTransition<SolidColorBrushTransition>(ForegroundProperty));
            }

            Transitions = transitions;
        }
    }

    protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
    {
        base.OnApplyTemplate(e);
        _loadingIcon = e.NameScope.Find<Icon>(BaseButtonTheme.LoadingIconPart);
        HandleTemplateApplied(e.NameScope);
        SetupTransitions();
    }

    private void HandleTemplateApplied(INameScope scope)
    {
        CollectStyleState();
        ApplyShapeStyleConfig();
        ApplyIconModeStyleConfig();
        UpdatePseudoClasses();
        SetupIcon();
        SetupIconBrush();
    }

    protected virtual void ApplyIconModeStyleConfig()
    {
        if (Icon is null)
        {
            return;
        }

        if (_styleState.HasFlag(ControlStyleState.Enabled))
        {
            if (_styleState.HasFlag(ControlStyleState.Sunken))
            {
                Icon.IconMode = IconMode.Selected;
            }
            else if (_styleState.HasFlag(ControlStyleState.MouseOver))
            {
                Icon.IconMode = IconMode.Active;
            }
            else
            {
                Icon.IconMode = IconMode.Normal;
            }
        }
        else
        {
            Icon.IconMode = IconMode.Disabled;
        }
    }

    protected override void OnAttachedToVisualTree(VisualTreeAttachmentEventArgs e)
    {
        base.OnAttachedToVisualTree(e);
        TokenResourceBinder.CreateSharedTokenBinding(this, BorderThicknessProperty,
            SharedTokenKey.BorderThickness,
            BindingPriority.Template,
            new RenderScaleAwareThicknessConfigure(this));
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

        if (Icon is not null)
        {
            TokenResourceBinder.CreateTokenBinding(Icon, Icon.NormalFilledBrushProperty,
                normalFilledBrushKey);
            TokenResourceBinder.CreateTokenBinding(Icon, Icon.SelectedFilledBrushProperty,
                selectedFilledBrushKey);
            TokenResourceBinder.CreateTokenBinding(Icon, Icon.ActiveFilledBrushProperty,
                activeFilledBrushKey);
            TokenResourceBinder.CreateTokenBinding(Icon, Icon.DisabledFilledBrushProperty,
                disabledFilledBrushKey);
        }

        if (_loadingIcon is not null)
        {
            TokenResourceBinder.CreateTokenBinding(_loadingIcon, Icon.NormalFilledBrushProperty,
                normalFilledBrushKey);
            TokenResourceBinder.CreateTokenBinding(_loadingIcon, Icon.SelectedFilledBrushProperty,
                selectedFilledBrushKey);
            TokenResourceBinder.CreateTokenBinding(_loadingIcon, Icon.ActiveFilledBrushProperty,
                activeFilledBrushKey);
            TokenResourceBinder.CreateTokenBinding(_loadingIcon, Icon.DisabledFilledBrushProperty,
                disabledFilledBrushKey);
        }

        NotifyIconBrushCalculated(in normalFilledBrushKey, in selectedFilledBrushKey, in activeFilledBrushKey,
            in disabledFilledBrushKey);
    }

    protected virtual void NotifyIconBrushCalculated(in TokenResourceKey normalFilledBrushKey,
                                                     in TokenResourceKey selectedFilledBrushKey,
                                                     in TokenResourceKey activeFilledBrushKey,
                                                     in TokenResourceKey disabledFilledBrushKey)
    {
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