using System.Diagnostics;
using AtomUI.Animations;
using AtomUI.Controls;
using AtomUI.Controls.Primitives;
using AtomUI.Media;
using AtomUI.Theme;
using AtomUI.Theme.Palette;
using AtomUI.Theme.TokenSystem;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Metadata;
using Avalonia.Controls.Primitives;
using Avalonia.Controls.Templates;
using Avalonia.Interactivity;
using Avalonia.Layout;
using Avalonia.Media;
using Avalonia.Media.Immutable;

namespace AtomUI.Desktop.Controls;

using AvaloniaButton = Avalonia.Controls.Button;

public enum ButtonType
{
    Default,
    Dashed,
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

public enum ButtonColor
{
    Default,
    Primary,
    Danger,
    Red,
    Volcano,
    Orange,
    Gold,
    Yellow,
    Lime,
    Green,
    Cyan,
    Blue,
    GeekBlue,
    Purple,
    Pink,
    Magenta,
    Grey
}

public enum ButtonVariant
{
    Outlined,
    Dashed,
    Solid,
    Filled,
    Text,
    Link
}

[PseudoClasses(ButtonPseudoClass.IconOnly,
    ButtonPseudoClass.Loading,
    ButtonPseudoClass.IsDanger,
    ButtonPseudoClass.DefaultType,
    ButtonPseudoClass.DashedType,
    ButtonPseudoClass.PrimaryType,
    ButtonPseudoClass.LinkType,
    ButtonPseudoClass.TextType)]
public class Button : AvaloniaButton,
                      ISizeTypeAware,
                      IWaveSpiritAwareControl,
                      ICompactSpaceAware,
                      IFormItemAware
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

    public static readonly StyledProperty<SizeType> SizeTypeProperty =
        SizeTypeControlProperty.SizeTypeProperty.AddOwner<Button>();

    public static readonly StyledProperty<PathIcon?> IconProperty =
        AvaloniaProperty.Register<Button, PathIcon?>(nameof(Icon));

    public static readonly StyledProperty<bool> IsMotionEnabledProperty =
        MotionAwareControlProperty.IsMotionEnabledProperty.AddOwner<Button>();

    public static readonly StyledProperty<bool> IsWaveSpiritEnabledProperty =
        WaveSpiritAwareControlProperty.IsWaveSpiritEnabledProperty.AddOwner<Button>();

    public static readonly StyledProperty<ButtonColor?> ColorProperty =
        AvaloniaProperty.Register<Button, ButtonColor?>(nameof(Color));

    public static readonly StyledProperty<ButtonVariant?> VariantProperty =
        AvaloniaProperty.Register<Button, ButtonVariant?>(nameof(Variant));
    
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

    public SizeType SizeType
    {
        get => GetValue(SizeTypeProperty);
        set => SetValue(SizeTypeProperty, value);
    }

    public PathIcon? Icon
    {
        get => GetValue(IconProperty);
        set => SetValue(IconProperty, value);
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

    public ButtonColor? Color
    {
        get => GetValue(ColorProperty);
        set => SetValue(ColorProperty, value);
    }

    public ButtonVariant? Variant
    {
        get => GetValue(VariantProperty);
        set => SetValue(VariantProperty, value);
    }

    #endregion

    #region 内部属性定义

    internal static readonly StyledProperty<bool> IsIconVisibleProperty =
        AvaloniaProperty.Register<Button, bool>(nameof(IsIconVisible), true);

    internal static readonly StyledProperty<object?> RightExtraContentProperty =
        AvaloniaProperty.Register<Button, object?>(nameof(RightExtraContent));

    internal static readonly StyledProperty<IDataTemplate?> RightExtraContentTemplateProperty =
        AvaloniaProperty.Register<ContentControl, IDataTemplate?>(nameof(RightExtraContentTemplate));

    internal static readonly StyledProperty<Thickness> EffectiveBorderThicknessProperty =
        AvaloniaProperty.Register<Button, Thickness>(
            nameof(EffectiveBorderThickness));

    internal static readonly StyledProperty<ButtonColor> EffectiveColorProperty =
        AvaloniaProperty.Register<Button, ButtonColor>(
            nameof(EffectiveColor),
            ButtonColor.Default);

    internal static readonly StyledProperty<ButtonVariant> EffectiveVariantProperty =
        AvaloniaProperty.Register<Button, ButtonVariant>(
            nameof(EffectiveVariant),
            ButtonVariant.Outlined);

    internal static readonly StyledProperty<bool> EffectiveIsDangerProperty =
        AvaloniaProperty.Register<Button, bool>(nameof(EffectiveIsDanger));

    internal static readonly StyledProperty<bool> EffectiveIsGhostProperty =
        AvaloniaProperty.Register<Button, bool>(nameof(EffectiveIsGhost));

    internal static readonly StyledProperty<bool> EffectiveIsBorderedProperty =
        AvaloniaProperty.Register<Button, bool>(nameof(EffectiveIsBordered), true);

    internal static readonly StyledProperty<IBrush?> VariantTextBrushProperty =
        AvaloniaProperty.Register<Button, IBrush?>(nameof(VariantTextBrush));

    internal static readonly StyledProperty<IBrush?> VariantTextHoverBrushProperty =
        AvaloniaProperty.Register<Button, IBrush?>(nameof(VariantTextHoverBrush));

    internal static readonly StyledProperty<IBrush?> VariantTextPressedBrushProperty =
        AvaloniaProperty.Register<Button, IBrush?>(nameof(VariantTextPressedBrush));

    internal static readonly StyledProperty<IBrush?> VariantBackgroundBrushProperty =
        AvaloniaProperty.Register<Button, IBrush?>(nameof(VariantBackgroundBrush));

    internal static readonly StyledProperty<IBrush?> VariantBackgroundHoverBrushProperty =
        AvaloniaProperty.Register<Button, IBrush?>(nameof(VariantBackgroundHoverBrush));

    internal static readonly StyledProperty<IBrush?> VariantBackgroundPressedBrushProperty =
        AvaloniaProperty.Register<Button, IBrush?>(nameof(VariantBackgroundPressedBrush));

    internal static readonly StyledProperty<IBrush?> VariantBorderBrushProperty =
        AvaloniaProperty.Register<Button, IBrush?>(nameof(VariantBorderBrush));

    internal static readonly StyledProperty<IBrush?> VariantBorderHoverBrushProperty =
        AvaloniaProperty.Register<Button, IBrush?>(nameof(VariantBorderHoverBrush));

    internal static readonly StyledProperty<IBrush?> VariantBorderPressedBrushProperty =
        AvaloniaProperty.Register<Button, IBrush?>(nameof(VariantBorderPressedBrush));

    internal static readonly StyledProperty<BoxShadows> VariantShadowProperty =
        AvaloniaProperty.Register<Button, BoxShadows>(nameof(VariantShadow));
    
    internal static readonly StyledProperty<WaveSpiritType> WaveSpiritTypeProperty =
        WaveSpiritAwareControlProperty.WaveSpiritTypeProperty.AddOwner<Button>();
    
    internal static readonly DirectProperty<Button, CornerRadius> EffectiveCornerRadiusProperty =
        AvaloniaProperty.RegisterDirect<Button, CornerRadius>(nameof(EffectiveCornerRadius),
            o => o.EffectiveCornerRadius,
            (o, v) => o.EffectiveCornerRadius = v);
    
    internal static readonly StyledProperty<SpaceItemPosition?> CompactSpaceItemPositionProperty = 
        CompactSpaceAwareControlProperty.CompactSpaceItemPositionProperty.AddOwner<Button>();
    
    internal static readonly StyledProperty<Orientation> CompactSpaceOrientationProperty = 
        CompactSpaceAwareControlProperty.CompactSpaceOrientationProperty.AddOwner<Button>();
    
    internal static readonly StyledProperty<bool> IsUsedInCompactSpaceProperty = 
        CompactSpaceAwareControlProperty.IsUsedInCompactSpaceProperty.AddOwner<Button>();
    
    internal bool IsIconVisible
    {
        get => GetValue(IsIconVisibleProperty);
        set => SetValue(IsIconVisibleProperty, value);
    }

    internal object? RightExtraContent
    {
        get => GetValue(RightExtraContentProperty);
        set => SetValue(RightExtraContentProperty, value);
    }

    internal IDataTemplate? RightExtraContentTemplate
    {
        get => GetValue(RightExtraContentTemplateProperty);
        set => SetValue(RightExtraContentTemplateProperty, value);
    }

    internal Thickness EffectiveBorderThickness
    {
        get => GetValue(EffectiveBorderThicknessProperty);
        set => SetValue(EffectiveBorderThicknessProperty, value);
    }

    internal ButtonColor EffectiveColor
    {
        get => GetValue(EffectiveColorProperty);
        set => SetValue(EffectiveColorProperty, value);
    }

    internal ButtonVariant EffectiveVariant
    {
        get => GetValue(EffectiveVariantProperty);
        set => SetValue(EffectiveVariantProperty, value);
    }

    internal bool EffectiveIsDanger
    {
        get => GetValue(EffectiveIsDangerProperty);
        set => SetValue(EffectiveIsDangerProperty, value);
    }

    internal bool EffectiveIsGhost
    {
        get => GetValue(EffectiveIsGhostProperty);
        set => SetValue(EffectiveIsGhostProperty, value);
    }

    internal bool EffectiveIsBordered
    {
        get => GetValue(EffectiveIsBorderedProperty);
        set => SetValue(EffectiveIsBorderedProperty, value);
    }

    internal IBrush? VariantTextBrush
    {
        get => GetValue(VariantTextBrushProperty);
        set => SetValue(VariantTextBrushProperty, value);
    }

    internal IBrush? VariantTextHoverBrush
    {
        get => GetValue(VariantTextHoverBrushProperty);
        set => SetValue(VariantTextHoverBrushProperty, value);
    }

    internal IBrush? VariantTextPressedBrush
    {
        get => GetValue(VariantTextPressedBrushProperty);
        set => SetValue(VariantTextPressedBrushProperty, value);
    }

    internal IBrush? VariantBackgroundBrush
    {
        get => GetValue(VariantBackgroundBrushProperty);
        set => SetValue(VariantBackgroundBrushProperty, value);
    }

    internal IBrush? VariantBackgroundHoverBrush
    {
        get => GetValue(VariantBackgroundHoverBrushProperty);
        set => SetValue(VariantBackgroundHoverBrushProperty, value);
    }

    internal IBrush? VariantBackgroundPressedBrush
    {
        get => GetValue(VariantBackgroundPressedBrushProperty);
        set => SetValue(VariantBackgroundPressedBrushProperty, value);
    }

    internal IBrush? VariantBorderBrush
    {
        get => GetValue(VariantBorderBrushProperty);
        set => SetValue(VariantBorderBrushProperty, value);
    }

    internal IBrush? VariantBorderHoverBrush
    {
        get => GetValue(VariantBorderHoverBrushProperty);
        set => SetValue(VariantBorderHoverBrushProperty, value);
    }

    internal IBrush? VariantBorderPressedBrush
    {
        get => GetValue(VariantBorderPressedBrushProperty);
        set => SetValue(VariantBorderPressedBrushProperty, value);
    }

    internal BoxShadows VariantShadow
    {
        get => GetValue(VariantShadowProperty);
        set => SetValue(VariantShadowProperty, value);
    }
    
    internal WaveSpiritType WaveSpiritType
    {
        get => GetValue(WaveSpiritTypeProperty);
        set => SetValue(WaveSpiritTypeProperty, value);
    }
    
    private CornerRadius _effectiveCornerRadius;

    internal CornerRadius EffectiveCornerRadius
    {
        get => _effectiveCornerRadius;
        set => SetAndRaise(EffectiveCornerRadiusProperty, ref _effectiveCornerRadius, value);
    }
    
    internal SpaceItemPosition? CompactSpaceItemPosition
    {
        get => GetValue(CompactSpaceItemPositionProperty);
        set => SetValue(CompactSpaceItemPositionProperty, value);
    }
    
    internal Orientation CompactSpaceOrientation
    {
        get => GetValue(CompactSpaceOrientationProperty);
        set => SetValue(CompactSpaceOrientationProperty, value);
    }
    
    internal bool IsUsedInCompactSpace
    {
        get => GetValue(IsUsedInCompactSpaceProperty);
        set => SetValue(IsUsedInCompactSpaceProperty, value);
    }
    
    #endregion
    
    private static readonly IBrush TransparentBrush = new ImmutableSolidColorBrush(Colors.Transparent);
    private WaveSpiritDecorator? _waveSpiritDecorator;

    static Button()
    {
        AffectsMeasure<Button>(SizeTypeProperty,
            ShapeProperty,
            IconProperty,
            CompactSpaceItemPositionProperty,
            CompactSpaceOrientationProperty);
        AffectsRender<Button>(ButtonTypeProperty,
            IsDangerProperty,
            IsGhostProperty,
            ColorProperty,
            VariantProperty);
    }

    public Button()
    {
        this.RegisterTokenResourceScope(ButtonToken.ScopeProvider);
    }

    #region 实现 CompactSpace 接口

    void ICompactSpaceAware.NotifyPositionChange(SpaceItemPosition? position)
    {
        IsUsedInCompactSpace     = position != null;
        CompactSpaceItemPosition = position;
    }

    void ICompactSpaceAware.NotifyOrientationChange(Orientation orientation)
    {
        CompactSpaceOrientation = orientation;
    }

    bool ICompactSpaceAware.IsAlwaysActiveZIndex()
    {
        return ButtonType == ButtonType.Primary;
    }

    double ICompactSpaceAware.GetBorderThickness() => GetBorderThicknessForCompactSpace();

    protected virtual double GetBorderThicknessForCompactSpace()
    {
        if (!IsUsedInCompactSpace)
        {
            return 0.0;
        }

        return CompactSpaceOrientation == Orientation.Horizontal ? BorderThickness.Left : BorderThickness.Top;
    }

    #endregion

    #region 实现 FormItem 接口

    private EventHandler? _formValueChanged;

    event EventHandler? IFormItemAware.ValueChanged
    {
        add => _formValueChanged += value;
        remove => _formValueChanged -= value;
    }

    void IFormItemAware.SetFormValue(object? value) => NotifySetFormValue(value);

    object? IFormItemAware.GetFormValue() => NotifyGetFormValue();
    void IFormItemAware.ClearFormValue() => NotifyClearFormValue();
    void IFormItemAware.NotifyValidateStatus(FormValidateStatus status) => NotifyValidateStatus(status);

    protected virtual void NotifySetFormValue(object? value)
    {
    }

    protected virtual object? NotifyGetFormValue()
    {
        return null;
    }

    protected virtual void NotifyClearFormValue()
    {
    }

    protected virtual void NotifyValidateStatus(FormValidateStatus status)
    {
    }

    #endregion

    protected override void OnInitialized()
    {
        base.OnInitialized();
        this.DisableTransitions();
    }

    protected override void OnLoaded(RoutedEventArgs e)
    {
        base.OnLoaded(e);
        Dispatcher.Post(this.EnableTransitions);
    }

    protected override void OnAttachedToVisualTree(VisualTreeAttachmentEventArgs e)
    {
        base.OnAttachedToVisualTree(e);
        if (ThemeManager.Current is not null)
        {
            ThemeManager.Current.ThemeChanged += HandleThemeChanged;
        }
        ConfigureVariantThemeVariables();
    }

    protected override void OnDetachedFromVisualTree(VisualTreeAttachmentEventArgs e)
    {
        base.OnDetachedFromVisualTree(e);
        if (ThemeManager.Current is not null)
        {
            ThemeManager.Current.ThemeChanged -= HandleThemeChanged;
        }
    }

    protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
    {
        base.OnApplyTemplate(e);
        _waveSpiritDecorator = e.NameScope.Find<WaveSpiritDecorator>("PART_WaveSpirit");
        ConfigureEffectiveButtonState();
        UpdatePseudoClasses();
        ConfigureWaveSpiritType();
        ConfigureEffectiveCornerRadius();
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
    
    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        base.OnPropertyChanged(change);
        if (change.Property == IsPressedProperty)
        {
            HandlePressedChanged(change);
        }

        if (change.Property == ButtonTypeProperty ||
            change.Property == ShapeProperty)
        {
            ConfigureWaveSpiritType();
        }

        if (ShouldConfigureEffectiveButtonState(change.Property))
        {
            ConfigureEffectiveButtonState();
        }

        if (ShouldUpdatePseudoClasses(change.Property))
        {
            UpdatePseudoClasses();
        }

        if (ShouldConfigureEffectiveBorderThickness(change.Property))
        {
            ConfigureEffectiveBorderThickness();
        }

        if (ShouldConfigureEffectiveCornerRadius(change.Property))
        {
            ConfigureEffectiveCornerRadius();
        }
    }

    private void HandleThemeChanged(object? sender, ThemeChangedEventArgs e)
    {
        ConfigureVariantThemeVariables();
    }

    private void HandlePressedChanged(AvaloniaPropertyChangedEventArgs change)
    {
        if (!CanPlayWaveSpirit(change))
        {
            return;
        }

        Debug.Assert(_waveSpiritDecorator != null);
        ConfigureWaveSpiritBrush();
        Dispatcher.Post(() =>
        {
            _waveSpiritDecorator?.Play();
        });
    }

    private bool CanPlayWaveSpirit(AvaloniaPropertyChangedEventArgs change)
    {
        return !IsLoading &&
               IsWaveSpiritEnabled &&
               (change.OldValue as bool? == true) &&
               IsWaveSpiritSupportedButtonType();
    }

    private bool IsWaveSpiritSupportedButtonType()
    {
        return EffectiveVariant == ButtonVariant.Solid ||
               EffectiveVariant == ButtonVariant.Outlined ||
               EffectiveVariant == ButtonVariant.Dashed;
    }

    private void ConfigureWaveSpiritBrush()
    {
        if (_waveSpiritDecorator is null)
        {
            return;
        }

        var waveBrush = ResolveWaveSpiritBrush();
        if (waveBrush is not null)
        {
            _waveSpiritDecorator.WaveBrush = waveBrush;
        }
    }

    private IBrush? ResolveWaveSpiritBrush()
    {
        if (EffectiveColor == ButtonColor.Default &&
            Color is null &&
            Variant is null &&
            !IsDanger)
        {
            return null;
        }

        return EffectiveVariant switch
        {
            ButtonVariant.Solid    => VariantBackgroundBrush,
            ButtonVariant.Outlined => VariantBorderBrush,
            ButtonVariant.Dashed   => VariantBorderBrush,
            _                      => null
        };
    }

    private void ConfigureWaveSpiritType()
    {
        WaveSpiritType = Shape switch
        {
            ButtonShape.Default => WaveSpiritType.RoundRectWave,
            ButtonShape.Round   => WaveSpiritType.PillWave,
            ButtonShape.Circle  => WaveSpiritType.CircleWave,
            _                   => default
        };
    }

    private void ConfigureEffectiveBorderThickness()
    {
        if (EffectiveIsBordered)
        {
            EffectiveBorderThickness = BorderThickness;
        }
        else
        {
            EffectiveBorderThickness = new Thickness(0);
        }
    }

    private void ConfigureEffectiveButtonState()
    {
        var (color, variant) = ResolveEffectiveColorAndVariant();

        if (IsGhost && variant == ButtonVariant.Solid)
        {
            variant = ButtonVariant.Outlined;
        }

        EffectiveColor      = color;
        EffectiveVariant    = variant;
        EffectiveIsDanger   = color == ButtonColor.Danger;
        EffectiveIsGhost    = IsGhost;
        EffectiveIsBordered = IsBorderedVariant(variant);

        ConfigureEffectiveBorderThickness();
        ConfigureVariantThemeVariables();
    }

    private (ButtonColor Color, ButtonVariant Variant) ResolveEffectiveColorAndVariant()
    {
        if (Color is not null && Variant is not null)
        {
            return (Color.Value, Variant.Value);
        }

        if (ButtonType != ButtonType.Default || IsDanger)
        {
            var variant = ButtonType switch
            {
                ButtonType.Primary => ButtonVariant.Solid,
                ButtonType.Dashed  => ButtonVariant.Dashed,
                ButtonType.Link    => ButtonVariant.Link,
                ButtonType.Text    => ButtonVariant.Text,
                _                  => ButtonVariant.Outlined
            };
            return (IsDanger ? ButtonColor.Danger : ResolveCompatibilityColor(ButtonType), variant);
        }

        if (Variant == ButtonVariant.Solid)
        {
            return (ButtonColor.Primary, ButtonVariant.Solid);
        }

        return (ButtonColor.Default, ButtonVariant.Outlined);
    }

    private static ButtonColor ResolveCompatibilityColor(ButtonType buttonType)
    {
        return buttonType switch
        {
            ButtonType.Primary => ButtonColor.Primary,
            _                  => ButtonColor.Default
        };
    }

    private static bool IsBorderedVariant(ButtonVariant variant)
    {
        return variant == ButtonVariant.Outlined ||
               variant == ButtonVariant.Dashed ||
               variant == ButtonVariant.Solid;
    }

    private void ConfigureVariantThemeVariables()
    {
        var activatedTheme = ThemeManager.Current?.ActivatedTheme;
        var sharedToken    = activatedTheme?.SharedToken;
        var buttonToken    = activatedTheme?.GetControlToken(ButtonToken.ID) as ButtonToken;
        if (sharedToken is null || buttonToken is null)
        {
            return;
        }

        if (EffectiveColor == ButtonColor.Default)
        {
            ConfigureDefaultVariantThemeVariables(buttonToken, sharedToken);
        }
        else if (EffectiveColor == ButtonColor.Primary)
        {
            ConfigureSemanticVariantThemeVariables(
                sharedToken.ColorPrimary,
                sharedToken.ColorPrimaryHover,
                sharedToken.ColorPrimaryActive,
                sharedToken.ColorPrimaryBg,
                sharedToken.ColorPrimaryBgHover,
                sharedToken.ColorPrimaryBorder,
                buttonToken.PrimaryColor,
                buttonToken.PrimaryShadow);
        }
        else if (EffectiveColor == ButtonColor.Danger)
        {
            ConfigureDangerVariantThemeVariables(buttonToken, sharedToken);
        }
        else if (TryGetPresetPrimaryColor(EffectiveColor, out var presetColor))
        {
            var colorMap = sharedToken.GetColorPalette(presetColor);
            if (colorMap is null)
            {
                return;
            }
            ConfigureSemanticVariantThemeVariables(
                colorMap.Color6,
                colorMap.Color5,
                colorMap.Color7,
                colorMap.Color1,
                colorMap.Color2,
                colorMap.Color3,
                buttonToken.SolidTextColor,
                CreatePresetShadow(sharedToken, colorMap.Color1));
        }
    }

    private void ConfigureDangerVariantThemeVariables(ButtonToken buttonToken, DesignToken sharedToken)
    {
        switch (EffectiveVariant)
        {
            case ButtonVariant.Solid:
                SetVariantThemeVariables(
                    buttonToken.DangerColor,
                    buttonToken.DangerColor,
                    buttonToken.DangerColor,
                    sharedToken.ColorError,
                    sharedToken.ColorErrorHover,
                    sharedToken.ColorErrorActive,
                    sharedToken.ColorError,
                    sharedToken.ColorErrorHover,
                    sharedToken.ColorErrorActive,
                    EffectiveIsGhost ? new BoxShadows() : buttonToken.DangerShadow);
                break;
            case ButtonVariant.Outlined:
            case ButtonVariant.Dashed:
                SetVariantThemeVariables(
                    sharedToken.ColorError,
                    sharedToken.ColorErrorBorderHover,
                    sharedToken.ColorErrorActive,
                    EffectiveIsGhost ? Colors.Transparent : buttonToken.DefaultBg,
                    EffectiveIsGhost ? Colors.Transparent : buttonToken.DefaultHoverBg,
                    EffectiveIsGhost ? Colors.Transparent : buttonToken.DefaultActiveBg,
                    sharedToken.ColorError,
                    sharedToken.ColorErrorBorderHover,
                    sharedToken.ColorErrorActive,
                    EffectiveIsGhost ? new BoxShadows() : buttonToken.DangerShadow);
                break;
            case ButtonVariant.Filled:
                SetVariantThemeVariables(
                    sharedToken.ColorError,
                    sharedToken.ColorError,
                    sharedToken.ColorError,
                    sharedToken.ColorErrorBg,
                    sharedToken.ColorErrorBgHover,
                    sharedToken.ColorErrorBgActive,
                    Colors.Transparent,
                    Colors.Transparent,
                    Colors.Transparent,
                    new BoxShadows());
                break;
            case ButtonVariant.Text:
                SetVariantThemeVariables(
                    sharedToken.ColorError,
                    sharedToken.ColorErrorHover,
                    sharedToken.ColorErrorActive,
                    Colors.Transparent,
                    sharedToken.ColorErrorBgHover,
                    sharedToken.ColorErrorBgActive,
                    Colors.Transparent,
                    Colors.Transparent,
                    Colors.Transparent,
                    new BoxShadows());
                break;
            case ButtonVariant.Link:
                SetVariantThemeVariables(
                    sharedToken.ColorError,
                    sharedToken.ColorErrorHover,
                    sharedToken.ColorErrorActive,
                    Colors.Transparent,
                    Colors.Transparent,
                    Colors.Transparent,
                    Colors.Transparent,
                    Colors.Transparent,
                    Colors.Transparent,
                    new BoxShadows());
                break;
        }
    }

    private void ConfigureDefaultVariantThemeVariables(ButtonToken buttonToken, DesignToken sharedToken)
    {
        var text = EffectiveVariant == ButtonVariant.Text
            ? buttonToken.TextTextColor
            : buttonToken.DefaultColor;
        var textHover = EffectiveVariant == ButtonVariant.Text
            ? buttonToken.TextTextHoverColor
            : buttonToken.DefaultHoverColor;
        var textPressed = EffectiveVariant == ButtonVariant.Text
            ? buttonToken.TextTextActiveColor
            : buttonToken.DefaultActiveColor;

        switch (EffectiveVariant)
        {
            case ButtonVariant.Solid:
                SetVariantThemeVariables(
                    buttonToken.SolidTextColor,
                    buttonToken.SolidTextColor,
                    buttonToken.SolidTextColor,
                    sharedToken.ColorBgSolid,
                    sharedToken.ColorBgSolidHover,
                    sharedToken.ColorBgSolidActive,
                    sharedToken.ColorBgSolid,
                    sharedToken.ColorBgSolidHover,
                    sharedToken.ColorBgSolidActive,
                    EffectiveIsGhost ? new BoxShadows() : buttonToken.DefaultShadow);
                break;
            case ButtonVariant.Outlined:
            case ButtonVariant.Dashed:
                if (EffectiveIsGhost)
                {
                    SetVariantThemeVariables(
                        buttonToken.DefaultGhostColor,
                        buttonToken.DefaultHoverColor,
                        buttonToken.DefaultActiveColor,
                        buttonToken.GhostBg,
                        buttonToken.GhostBg,
                        buttonToken.GhostBg,
                        buttonToken.DefaultGhostBorderColor,
                        buttonToken.DefaultHoverBorderColor,
                        buttonToken.DefaultActiveBorderColor,
                        new BoxShadows());
                }
                else
                {
                    SetVariantThemeVariables(
                        buttonToken.DefaultColor,
                        buttonToken.DefaultHoverColor,
                        buttonToken.DefaultActiveColor,
                        buttonToken.DefaultBg,
                        buttonToken.DefaultHoverBg,
                        buttonToken.DefaultActiveBg,
                        buttonToken.DefaultBorderColor,
                        buttonToken.DefaultHoverBorderColor,
                        buttonToken.DefaultActiveBorderColor,
                        buttonToken.DefaultShadow);
                }
                break;
            case ButtonVariant.Filled:
                SetVariantThemeVariables(
                    buttonToken.DefaultColor,
                    buttonToken.DefaultColor,
                    buttonToken.DefaultColor,
                    sharedToken.ColorFillTertiary,
                    sharedToken.ColorFillSecondary,
                    sharedToken.ColorFill,
                    Colors.Transparent,
                    Colors.Transparent,
                    Colors.Transparent,
                    new BoxShadows());
                break;
            case ButtonVariant.Text:
                SetVariantThemeVariables(
                    text,
                    textHover,
                    textPressed,
                    Colors.Transparent,
                    buttonToken.TextHoverBg,
                    sharedToken.ColorBgTextActive,
                    Colors.Transparent,
                    Colors.Transparent,
                    Colors.Transparent,
                    new BoxShadows());
                break;
            case ButtonVariant.Link:
                SetVariantThemeVariables(
                    sharedToken.ColorLink ?? sharedToken.ColorPrimary,
                    sharedToken.ColorLinkHover,
                    sharedToken.ColorLinkActive,
                    Colors.Transparent,
                    buttonToken.LinkHoverBg,
                    Colors.Transparent,
                    Colors.Transparent,
                    Colors.Transparent,
                    Colors.Transparent,
                    new BoxShadows());
                break;
        }
    }

    private void ConfigureSemanticVariantThemeVariables(
        Color baseColor,
        Color hoverColor,
        Color activeColor,
        Color lightColor,
        Color lightHoverColor,
        Color lightActiveColor,
        Color solidTextColor,
        BoxShadows shadow)
    {
        switch (EffectiveVariant)
        {
            case ButtonVariant.Solid:
                SetVariantThemeVariables(
                    solidTextColor,
                    solidTextColor,
                    solidTextColor,
                    baseColor,
                    hoverColor,
                    activeColor,
                    baseColor,
                    hoverColor,
                    activeColor,
                    EffectiveIsGhost ? new BoxShadows() : shadow);
                break;
            case ButtonVariant.Outlined:
            case ButtonVariant.Dashed:
                SetVariantThemeVariables(
                    baseColor,
                    hoverColor,
                    activeColor,
                    EffectiveIsGhost ? Colors.Transparent : GetDefaultBackgroundColor(),
                    EffectiveIsGhost ? Colors.Transparent : GetDefaultBackgroundColor(),
                    EffectiveIsGhost ? Colors.Transparent : GetDefaultBackgroundColor(),
                    baseColor,
                    hoverColor,
                    activeColor,
                    EffectiveIsGhost ? new BoxShadows() : shadow);
                break;
            case ButtonVariant.Filled:
                SetVariantThemeVariables(
                    baseColor,
                    baseColor,
                    baseColor,
                    lightColor,
                    lightHoverColor,
                    lightActiveColor,
                    Colors.Transparent,
                    Colors.Transparent,
                    Colors.Transparent,
                    new BoxShadows());
                break;
            case ButtonVariant.Text:
                SetVariantThemeVariables(
                    baseColor,
                    hoverColor,
                    activeColor,
                    Colors.Transparent,
                    lightColor,
                    lightActiveColor,
                    Colors.Transparent,
                    Colors.Transparent,
                    Colors.Transparent,
                    new BoxShadows());
                break;
            case ButtonVariant.Link:
                SetVariantThemeVariables(
                    baseColor,
                    hoverColor,
                    activeColor,
                    Colors.Transparent,
                    Colors.Transparent,
                    Colors.Transparent,
                    Colors.Transparent,
                    Colors.Transparent,
                    Colors.Transparent,
                    new BoxShadows());
                break;
        }
    }

    private Color GetDefaultBackgroundColor()
    {
        var activatedTheme = ThemeManager.Current?.ActivatedTheme;
        var buttonToken    = activatedTheme?.GetControlToken(ButtonToken.ID) as ButtonToken;
        return buttonToken?.DefaultBg ?? Colors.Transparent;
    }

    private void SetVariantThemeVariables(
        Color text,
        Color textHover,
        Color textPressed,
        Color background,
        Color backgroundHover,
        Color backgroundPressed,
        Color border,
        Color borderHover,
        Color borderPressed,
        BoxShadows shadow)
    {
        VariantTextBrush              = ToBrush(text);
        VariantTextHoverBrush         = ToBrush(textHover);
        VariantTextPressedBrush       = ToBrush(textPressed);
        VariantBackgroundBrush        = ToBrush(background);
        VariantBackgroundHoverBrush   = ToBrush(backgroundHover);
        VariantBackgroundPressedBrush = ToBrush(backgroundPressed);
        VariantBorderBrush            = ToBrush(border);
        VariantBorderHoverBrush       = ToBrush(borderHover);
        VariantBorderPressedBrush     = ToBrush(borderPressed);
        VariantShadow                 = shadow;
        ConfigureWaveSpiritBrush();
    }

    private static IBrush ToBrush(Color color)
    {
        return color == Colors.Transparent
            ? TransparentBrush
            : new ImmutableSolidColorBrush(color);
    }

    private static BoxShadows CreatePresetShadow(DesignToken sharedToken, Color lightColor)
    {
        return new BoxShadows(new BoxShadow
        {
            OffsetX = 0,
            OffsetY = sharedToken.ControlOutlineWidth,
            Blur    = 3,
            Spread  = 0,
            Color   = ColorUtils.CalculateAlphaColor(lightColor, sharedToken.ColorBgContainer)
        });
    }

    private static bool TryGetPresetPrimaryColor(ButtonColor color, out PresetPrimaryColor presetColor)
    {
        switch (color)
        {
            case ButtonColor.Red:
                presetColor = PresetPrimaryColor.Red;
                return true;
            case ButtonColor.Volcano:
                presetColor = PresetPrimaryColor.Volcano;
                return true;
            case ButtonColor.Orange:
                presetColor = PresetPrimaryColor.Orange;
                return true;
            case ButtonColor.Gold:
                presetColor = PresetPrimaryColor.Gold;
                return true;
            case ButtonColor.Yellow:
                presetColor = PresetPrimaryColor.Yellow;
                return true;
            case ButtonColor.Lime:
                presetColor = PresetPrimaryColor.Lime;
                return true;
            case ButtonColor.Green:
                presetColor = PresetPrimaryColor.Green;
                return true;
            case ButtonColor.Cyan:
                presetColor = PresetPrimaryColor.Cyan;
                return true;
            case ButtonColor.Blue:
                presetColor = PresetPrimaryColor.Blue;
                return true;
            case ButtonColor.GeekBlue:
                presetColor = PresetPrimaryColor.GeekBlue;
                return true;
            case ButtonColor.Purple:
                presetColor = PresetPrimaryColor.Purple;
                return true;
            case ButtonColor.Pink:
                presetColor = PresetPrimaryColor.Pink;
                return true;
            case ButtonColor.Magenta:
                presetColor = PresetPrimaryColor.Magenta;
                return true;
            case ButtonColor.Grey:
                presetColor = PresetPrimaryColor.Grey;
                return true;
            default:
                presetColor = PresetPrimaryColor.Grey;
                return false;
        }
    }

    private void ConfigureEffectiveCornerRadius()
    {
        EffectiveCornerRadius = CompactSpace.CalculateEffectiveCornerRadius(
            CornerRadius, 
            IsUsedInCompactSpace, 
            CompactSpaceItemPosition,
            CompactSpaceOrientation);
    }

    private bool ShouldUpdatePseudoClasses(AvaloniaProperty property)
    {
        return property == ContentProperty ||
               property == IsLoadingProperty ||
               property == ButtonTypeProperty ||
               property == IsDangerProperty;
    }

    private bool ShouldConfigureEffectiveButtonState(AvaloniaProperty property)
    {
        return property == ButtonTypeProperty ||
               property == IsDangerProperty ||
               property == IsGhostProperty ||
               property == ColorProperty ||
               property == VariantProperty;
    }

    private bool ShouldConfigureEffectiveBorderThickness(AvaloniaProperty property)
    {
        return property == BorderBrushProperty ||
               property == IsEnabledProperty ||
               property == BorderThicknessProperty;
    }

    private bool ShouldConfigureEffectiveCornerRadius(AvaloniaProperty property)
    {
        return property == CornerRadiusProperty ||
               property == CompactSpaceItemPositionProperty ||
               property == CompactSpaceOrientationProperty;
    }

    private void UpdatePseudoClasses()
    {
        PseudoClasses.Set(ButtonPseudoClass.IconOnly, Icon is not null && Content is null);
        PseudoClasses.Set(ButtonPseudoClass.Loading, IsLoading);
        PseudoClasses.Set(ButtonPseudoClass.DefaultType, ButtonType == ButtonType.Default);
        PseudoClasses.Set(ButtonPseudoClass.DashedType, ButtonType == ButtonType.Dashed);
        PseudoClasses.Set(ButtonPseudoClass.PrimaryType, ButtonType == ButtonType.Primary);
        PseudoClasses.Set(ButtonPseudoClass.LinkType, ButtonType == ButtonType.Link);
        PseudoClasses.Set(ButtonPseudoClass.TextType, ButtonType == ButtonType.Text);
        PseudoClasses.Set(ButtonPseudoClass.IsDanger, IsDanger);
    }

}
