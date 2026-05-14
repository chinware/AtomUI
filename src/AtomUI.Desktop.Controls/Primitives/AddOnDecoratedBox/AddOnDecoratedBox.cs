using AtomUI.Animations;
using AtomUI.Controls;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Presenters;
using Avalonia.Controls.Primitives;
using Avalonia.Controls.Templates;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Layout;
using Avalonia.Media;
using Avalonia.Metadata;
using Avalonia.Threading;
using Avalonia.VisualTree;

namespace AtomUI.Desktop.Controls;

internal class AddOnDecoratedBox : ContentControl, 
                                   ISizeTypeAware,
                                   IMotionAwareControl,
                                   IInputControlStatusAware,
                                   IInputControlStyleVariantAware
{
    public const string AddOnDecoratedBoxPart = "PART_AddOnDecoratedBox";
    
    #region 公共属性定义

    public static readonly StyledProperty<object?> LeftAddOnProperty =
        AvaloniaProperty.Register<AddOnDecoratedBox, object?>(nameof(LeftAddOn));
    
    public static readonly StyledProperty<IDataTemplate?> LeftAddOnTemplateProperty =
        AvaloniaProperty.Register<AddOnDecoratedBox, IDataTemplate?>(nameof(LeftAddOnTemplate));

    public static readonly StyledProperty<object?> RightAddOnProperty =
        AvaloniaProperty.Register<AddOnDecoratedBox, object?>(nameof(RightAddOn));
    
    public static readonly StyledProperty<IDataTemplate?> RightAddOnTemplateProperty =
        AvaloniaProperty.Register<AddOnDecoratedBox, IDataTemplate?>(nameof(RightAddOnTemplate));
    
    public static readonly StyledProperty<object?> ContentLeftAddOnProperty =
        AvaloniaProperty.Register<AddOnDecoratedBox, object?>(nameof(ContentLeftAddOn));
    
    public static readonly StyledProperty<IDataTemplate?> ContentLeftAddOnTemplateProperty =
        AvaloniaProperty.Register<AddOnDecoratedBox, IDataTemplate?>(nameof(ContentLeftAddOnTemplate));

    public static readonly StyledProperty<object?> ContentRightAddOnProperty =
        AvaloniaProperty.Register<AddOnDecoratedBox, object?>(nameof(ContentRightAddOn));
    
    public static readonly StyledProperty<IDataTemplate?> ContentRightAddOnTemplateProperty =
        AvaloniaProperty.Register<AddOnDecoratedBox, IDataTemplate?>(nameof(ContentRightAddOnTemplate));

    public static readonly StyledProperty<SizeType> SizeTypeProperty =
        SizeTypeControlProperty.SizeTypeProperty.AddOwner<AddOnDecoratedBox>();

    public static readonly StyledProperty<InputControlStyleVariant> StyleVariantProperty =
        InputControlStyleVariantProperty.StyleVariantProperty.AddOwner<AddOnDecoratedBox>();

    public static readonly StyledProperty<InputControlStatus> StatusProperty =
        InputControlStatusProperty.StatusProperty.AddOwner<AddOnDecoratedBox>();
    
    public static readonly StyledProperty<bool> IsMotionEnabledProperty =
        MotionAwareControlProperty.IsMotionEnabledProperty.AddOwner<AddOnDecoratedBox>();

    [DependsOn(nameof(LeftAddOnTemplate))]
    public object? LeftAddOn
    {
        get => GetValue(LeftAddOnProperty);
        set => SetValue(LeftAddOnProperty, value);
    }
    
    public IDataTemplate? LeftAddOnTemplate
    {
        get => GetValue(LeftAddOnTemplateProperty);
        set => SetValue(LeftAddOnTemplateProperty, value);
    }

    [DependsOn(nameof(RightAddOnTemplate))]
    public object? RightAddOn
    {
        get => GetValue(RightAddOnProperty);
        set => SetValue(RightAddOnProperty, value);
    }
    
    public IDataTemplate? RightAddOnTemplate
    {
        get => GetValue(RightAddOnTemplateProperty);
        set => SetValue(RightAddOnTemplateProperty, value);
    }
    
    [DependsOn(nameof(ContentLeftAddOnTemplate))]
    public object? ContentLeftAddOn
    {
        get => GetValue(ContentLeftAddOnProperty);
        set => SetValue(ContentLeftAddOnProperty, value);
    }
    
    public IDataTemplate? ContentLeftAddOnTemplate
    {
        get => GetValue(ContentLeftAddOnTemplateProperty);
        set => SetValue(ContentLeftAddOnTemplateProperty, value);
    }

    [DependsOn(nameof(ContentRightAddOnTemplate))]
    public object? ContentRightAddOn
    {
        get => GetValue(ContentRightAddOnProperty);
        set => SetValue(ContentRightAddOnProperty, value);
    }
    
    public IDataTemplate? ContentRightAddOnTemplate
    {
        get => GetValue(ContentRightAddOnTemplateProperty);
        set => SetValue(ContentRightAddOnTemplateProperty, value);
    }

    public SizeType SizeType
    {
        get => GetValue(SizeTypeProperty);
        set => SetValue(SizeTypeProperty, value);
    }

    public InputControlStyleVariant StyleVariant
    {
        get => GetValue(StyleVariantProperty);
        set => SetValue(StyleVariantProperty, value);
    }

    public InputControlStatus Status
    {
        get => GetValue(StatusProperty);
        set => SetValue(StatusProperty, value);
    }
    
    public bool IsMotionEnabled
    {
        get => GetValue(IsMotionEnabledProperty);
        set => SetValue(IsMotionEnabledProperty, value);
    }

    #endregion
    
    #region 内部属性定义
    internal static readonly StyledProperty<IBrush?> AddOnStatusForegroundProperty =
        AvaloniaProperty.Register<AddOnDecoratedBox, IBrush?>(nameof(AddOnStatusForeground));

    internal static readonly StyledProperty<IBrush?> AddOnStatusIconBrushProperty =
        AvaloniaProperty.Register<AddOnDecoratedBox, IBrush?>(nameof(AddOnStatusIconBrush));

    internal static readonly StyledProperty<IBrush?> InnerBoxDefaultBorderBrushProperty =
        AvaloniaProperty.Register<AddOnDecoratedBox, IBrush?>(nameof(InnerBoxDefaultBorderBrush));

    internal static readonly StyledProperty<IBrush?> InnerBoxHoverBorderBrushProperty =
        AvaloniaProperty.Register<AddOnDecoratedBox, IBrush?>(nameof(InnerBoxHoverBorderBrush));

    internal static readonly StyledProperty<IBrush?> InnerBoxActiveBorderBrushProperty =
        AvaloniaProperty.Register<AddOnDecoratedBox, IBrush?>(nameof(InnerBoxActiveBorderBrush));

    internal static readonly StyledProperty<IBrush?> InnerBoxFilledBackgroundProperty =
        AvaloniaProperty.Register<AddOnDecoratedBox, IBrush?>(nameof(InnerBoxFilledBackground));

    internal static readonly StyledProperty<IBrush?> InnerBoxFilledBorderBrushProperty =
        AvaloniaProperty.Register<AddOnDecoratedBox, IBrush?>(nameof(InnerBoxFilledBorderBrush));

    internal static readonly StyledProperty<IBrush?> InnerBoxFilledHoverBackgroundProperty =
        AvaloniaProperty.Register<AddOnDecoratedBox, IBrush?>(nameof(InnerBoxFilledHoverBackground));

    internal static readonly StyledProperty<IBrush?> InnerBoxActiveBackgroundProperty =
        AvaloniaProperty.Register<AddOnDecoratedBox, IBrush?>(nameof(InnerBoxActiveBackground));

    internal static readonly StyledProperty<IBrush?> InnerBoxDisabledBackgroundProperty =
        AvaloniaProperty.Register<AddOnDecoratedBox, IBrush?>(nameof(InnerBoxDisabledBackground));

    internal static readonly StyledProperty<IBrush?> InnerBoxErrorBorderBrushProperty =
        AvaloniaProperty.Register<AddOnDecoratedBox, IBrush?>(nameof(InnerBoxErrorBorderBrush));

    internal static readonly StyledProperty<IBrush?> InnerBoxErrorHoverBorderBrushProperty =
        AvaloniaProperty.Register<AddOnDecoratedBox, IBrush?>(nameof(InnerBoxErrorHoverBorderBrush));

    internal static readonly StyledProperty<IBrush?> InnerBoxErrorBackgroundProperty =
        AvaloniaProperty.Register<AddOnDecoratedBox, IBrush?>(nameof(InnerBoxErrorBackground));

    internal static readonly StyledProperty<IBrush?> InnerBoxErrorFilledBorderBrushProperty =
        AvaloniaProperty.Register<AddOnDecoratedBox, IBrush?>(nameof(InnerBoxErrorFilledBorderBrush));

    internal static readonly StyledProperty<IBrush?> InnerBoxErrorHoverBackgroundProperty =
        AvaloniaProperty.Register<AddOnDecoratedBox, IBrush?>(nameof(InnerBoxErrorHoverBackground));

    internal static readonly StyledProperty<IBrush?> InnerBoxWarningBorderBrushProperty =
        AvaloniaProperty.Register<AddOnDecoratedBox, IBrush?>(nameof(InnerBoxWarningBorderBrush));

    internal static readonly StyledProperty<IBrush?> InnerBoxWarningHoverBorderBrushProperty =
        AvaloniaProperty.Register<AddOnDecoratedBox, IBrush?>(nameof(InnerBoxWarningHoverBorderBrush));

    internal static readonly StyledProperty<IBrush?> InnerBoxWarningBackgroundProperty =
        AvaloniaProperty.Register<AddOnDecoratedBox, IBrush?>(nameof(InnerBoxWarningBackground));

    internal static readonly StyledProperty<IBrush?> InnerBoxWarningFilledBorderBrushProperty =
        AvaloniaProperty.Register<AddOnDecoratedBox, IBrush?>(nameof(InnerBoxWarningFilledBorderBrush));

    internal static readonly StyledProperty<IBrush?> InnerBoxWarningHoverBackgroundProperty =
        AvaloniaProperty.Register<AddOnDecoratedBox, IBrush?>(nameof(InnerBoxWarningHoverBackground));

    internal IBrush? AddOnStatusForeground
    {
        get => GetValue(AddOnStatusForegroundProperty);
        set => SetValue(AddOnStatusForegroundProperty, value);
    }

    internal IBrush? AddOnStatusIconBrush
    {
        get => GetValue(AddOnStatusIconBrushProperty);
        set => SetValue(AddOnStatusIconBrushProperty, value);
    }

    internal IBrush? InnerBoxDefaultBorderBrush
    {
        get => GetValue(InnerBoxDefaultBorderBrushProperty);
        set => SetValue(InnerBoxDefaultBorderBrushProperty, value);
    }

    internal IBrush? InnerBoxHoverBorderBrush
    {
        get => GetValue(InnerBoxHoverBorderBrushProperty);
        set => SetValue(InnerBoxHoverBorderBrushProperty, value);
    }

    internal IBrush? InnerBoxActiveBorderBrush
    {
        get => GetValue(InnerBoxActiveBorderBrushProperty);
        set => SetValue(InnerBoxActiveBorderBrushProperty, value);
    }

    internal IBrush? InnerBoxFilledBackground
    {
        get => GetValue(InnerBoxFilledBackgroundProperty);
        set => SetValue(InnerBoxFilledBackgroundProperty, value);
    }

    internal IBrush? InnerBoxFilledBorderBrush
    {
        get => GetValue(InnerBoxFilledBorderBrushProperty);
        set => SetValue(InnerBoxFilledBorderBrushProperty, value);
    }

    internal IBrush? InnerBoxFilledHoverBackground
    {
        get => GetValue(InnerBoxFilledHoverBackgroundProperty);
        set => SetValue(InnerBoxFilledHoverBackgroundProperty, value);
    }

    internal IBrush? InnerBoxActiveBackground
    {
        get => GetValue(InnerBoxActiveBackgroundProperty);
        set => SetValue(InnerBoxActiveBackgroundProperty, value);
    }

    internal IBrush? InnerBoxDisabledBackground
    {
        get => GetValue(InnerBoxDisabledBackgroundProperty);
        set => SetValue(InnerBoxDisabledBackgroundProperty, value);
    }

    internal IBrush? InnerBoxErrorBorderBrush
    {
        get => GetValue(InnerBoxErrorBorderBrushProperty);
        set => SetValue(InnerBoxErrorBorderBrushProperty, value);
    }

    internal IBrush? InnerBoxErrorHoverBorderBrush
    {
        get => GetValue(InnerBoxErrorHoverBorderBrushProperty);
        set => SetValue(InnerBoxErrorHoverBorderBrushProperty, value);
    }

    internal IBrush? InnerBoxErrorBackground
    {
        get => GetValue(InnerBoxErrorBackgroundProperty);
        set => SetValue(InnerBoxErrorBackgroundProperty, value);
    }

    internal IBrush? InnerBoxErrorFilledBorderBrush
    {
        get => GetValue(InnerBoxErrorFilledBorderBrushProperty);
        set => SetValue(InnerBoxErrorFilledBorderBrushProperty, value);
    }

    internal IBrush? InnerBoxErrorHoverBackground
    {
        get => GetValue(InnerBoxErrorHoverBackgroundProperty);
        set => SetValue(InnerBoxErrorHoverBackgroundProperty, value);
    }

    internal IBrush? InnerBoxWarningBorderBrush
    {
        get => GetValue(InnerBoxWarningBorderBrushProperty);
        set => SetValue(InnerBoxWarningBorderBrushProperty, value);
    }

    internal IBrush? InnerBoxWarningHoverBorderBrush
    {
        get => GetValue(InnerBoxWarningHoverBorderBrushProperty);
        set => SetValue(InnerBoxWarningHoverBorderBrushProperty, value);
    }

    internal IBrush? InnerBoxWarningBackground
    {
        get => GetValue(InnerBoxWarningBackgroundProperty);
        set => SetValue(InnerBoxWarningBackgroundProperty, value);
    }

    internal IBrush? InnerBoxWarningFilledBorderBrush
    {
        get => GetValue(InnerBoxWarningFilledBorderBrushProperty);
        set => SetValue(InnerBoxWarningFilledBorderBrushProperty, value);
    }

    internal IBrush? InnerBoxWarningHoverBackground
    {
        get => GetValue(InnerBoxWarningHoverBackgroundProperty);
        set => SetValue(InnerBoxWarningHoverBackgroundProperty, value);
    }

    internal static readonly DirectProperty<AddOnDecoratedBox, Thickness> InnerBoxBorderThicknessProperty =
        AvaloniaProperty.RegisterDirect<AddOnDecoratedBox, Thickness>(nameof(InnerBoxBorderThickness),
            o => o.InnerBoxBorderThickness,
            (o, v) => o.InnerBoxBorderThickness = v);

    internal static readonly StyledProperty<IBrush?> EffectiveInnerBoxBorderBrushProperty =
        AvaloniaProperty.Register<AddOnDecoratedBox, IBrush?>(nameof(EffectiveInnerBoxBorderBrush));

    internal static readonly StyledProperty<IBrush?> EffectiveInnerBoxBackgroundProperty =
        AvaloniaProperty.Register<AddOnDecoratedBox, IBrush?>(nameof(EffectiveInnerBoxBackground));

    internal static readonly DirectProperty<AddOnDecoratedBox, CornerRadius> InnerBoxCornerRadiusProperty =
        AvaloniaProperty.RegisterDirect<AddOnDecoratedBox, CornerRadius>(nameof(InnerBoxCornerRadius),
            o => o.InnerBoxCornerRadius,
            (o, v) => o.InnerBoxCornerRadius = v);

    internal static readonly DirectProperty<AddOnDecoratedBox, CornerRadius> LeftAddOnCornerRadiusProperty =
        AvaloniaProperty.RegisterDirect<AddOnDecoratedBox, CornerRadius>(nameof(LeftAddOnCornerRadius),
            o => o.LeftAddOnCornerRadius,
            (o, v) => o.LeftAddOnCornerRadius = v);

    internal static readonly DirectProperty<AddOnDecoratedBox, CornerRadius> RightAddOnCornerRadiusProperty =
        AvaloniaProperty.RegisterDirect<AddOnDecoratedBox, CornerRadius>(nameof(RightAddOnCornerRadius),
            o => o.RightAddOnCornerRadius,
            (o, v) => o.RightAddOnCornerRadius = v);

    internal static readonly DirectProperty<AddOnDecoratedBox, Thickness> LeftAddOnBorderThicknessProperty =
        AvaloniaProperty.RegisterDirect<AddOnDecoratedBox, Thickness>(nameof(LeftAddOnBorderThickness),
            o => o.LeftAddOnBorderThickness,
            (o, v) => o.LeftAddOnBorderThickness = v);

    internal static readonly DirectProperty<AddOnDecoratedBox, Thickness> RightAddOnBorderThicknessProperty =
        AvaloniaProperty.RegisterDirect<AddOnDecoratedBox, Thickness>(nameof(RightAddOnBorderThickness),
            o => o.RightAddOnBorderThickness,
            (o, v) => o.RightAddOnBorderThickness = v);
    
    internal static readonly DirectProperty<AddOnDecoratedBox, bool> IsInnerBoxHoverProperty =
        AvaloniaProperty.RegisterDirect<AddOnDecoratedBox, bool>(nameof(IsInnerBoxHover),
            o => o.IsInnerBoxHover,
            (o, v) => o.IsInnerBoxHover = v);
    
    internal static readonly DirectProperty<AddOnDecoratedBox, bool> IsInnerBoxPressedProperty =
        AvaloniaProperty.RegisterDirect<AddOnDecoratedBox, bool>(nameof(IsInnerBoxPressed),
            o => o.IsInnerBoxPressed,
            (o, v) => o.IsInnerBoxPressed = v);
    
    internal static readonly StyledProperty<SpaceItemPosition?> CompactSpaceItemPositionProperty = 
        CompactSpaceAwareControlProperty.CompactSpaceItemPositionProperty.AddOwner<AddOnDecoratedBox>();
    
    internal static readonly StyledProperty<Orientation> CompactSpaceOrientationProperty = 
        CompactSpaceAwareControlProperty.CompactSpaceOrientationProperty.AddOwner<AddOnDecoratedBox>();
    
    internal static readonly StyledProperty<bool> IsUsedInCompactSpaceProperty = 
        CompactSpaceAwareControlProperty.IsUsedInCompactSpaceProperty.AddOwner<AddOnDecoratedBox>();
    
    private Thickness _innerBoxBorderThickness;

    internal Thickness InnerBoxBorderThickness
    {
        get => _innerBoxBorderThickness;
        set => SetAndRaise(InnerBoxBorderThicknessProperty, ref _innerBoxBorderThickness, value);
    }

    internal IBrush? EffectiveInnerBoxBorderBrush
    {
        get => GetValue(EffectiveInnerBoxBorderBrushProperty);
        set => SetValue(EffectiveInnerBoxBorderBrushProperty, value);
    }

    internal IBrush? EffectiveInnerBoxBackground
    {
        get => GetValue(EffectiveInnerBoxBackgroundProperty);
        set => SetValue(EffectiveInnerBoxBackgroundProperty, value);
    }

    private CornerRadius _innerBoxCornerRadius;

    internal CornerRadius InnerBoxCornerRadius
    {
        get => _innerBoxCornerRadius;
        set => SetAndRaise(InnerBoxCornerRadiusProperty, ref _innerBoxCornerRadius, value);
    }

    private CornerRadius _leftAddOnCornerRadius;

    internal CornerRadius LeftAddOnCornerRadius
    {
        get => _leftAddOnCornerRadius;
        set => SetAndRaise(LeftAddOnCornerRadiusProperty, ref _leftAddOnCornerRadius, value);
    }

    private CornerRadius _rightAddOnCornerRadius;

    internal CornerRadius RightAddOnCornerRadius
    {
        get => _rightAddOnCornerRadius;
        set => SetAndRaise(RightAddOnCornerRadiusProperty, ref _rightAddOnCornerRadius, value);
    }

    private Thickness _leftAddOnBorderThickness;

    internal Thickness LeftAddOnBorderThickness
    {
        get => _leftAddOnBorderThickness;
        set => SetAndRaise(LeftAddOnBorderThicknessProperty, ref _leftAddOnBorderThickness, value);
    }

    private Thickness _rightAddOnBorderThickness;

    internal Thickness RightAddOnBorderThickness
    {
        get => _rightAddOnBorderThickness;
        set => SetAndRaise(RightAddOnBorderThicknessProperty, ref _rightAddOnBorderThickness, value);
    }
    
    private bool _isInnerBoxHover;

    internal bool IsInnerBoxHover
    {
        get => _isInnerBoxHover;
        set => SetAndRaise(IsInnerBoxHoverProperty, ref _isInnerBoxHover, value);
    }
    
    private bool _isInnerBoxPressed;

    internal bool IsInnerBoxPressed
    {
        get => _isInnerBoxPressed;
        set => SetAndRaise(IsInnerBoxPressedProperty, ref _isInnerBoxPressed, value);
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
    
    private protected Control? _leftAddOn;
    private protected Control? _rightAddOn;
    private ContentPresenter? _contentLeftAddOn;
    private ContentPresenter? _contentRightAddOn;
    private bool _borderInfoDirty;
    private bool _cornerRadiusDirty;
    private bool _borderThicknessDirty;
    private bool _layoutUpdatePosted;
    private List<WeakReference<Icon>>? _leftAddOnIcons;
    private List<WeakReference<Icon>>? _rightAddOnIcons;
    private List<WeakReference<Icon>>? _contentLeftAddOnIcons;
    private List<WeakReference<Icon>>? _contentRightAddOnIcons;
    private HashSet<Control>? _pendingAddOnChildAttachListeners;
    private bool _hasAppliedAddOnStatusColors;
    private IBrush? _appliedAddOnStatusForeground;
    private IBrush? _appliedAddOnStatusIconBrush;
    private int _addOnStatusContentVersion;
    private int _appliedAddOnStatusContentVersion = -1;

    internal Border? ContentFrame;
    
    static AddOnDecoratedBox()
    {
        AffectsRender<AddOnDecoratedBox>(
            BorderBrushProperty,
            BackgroundProperty,
            EffectiveInnerBoxBorderBrushProperty,
            EffectiveInnerBoxBackgroundProperty);
        AffectsMeasure<AddOnDecoratedBox>(LeftAddOnProperty,
            LeftAddOnTemplateProperty,
            RightAddOnProperty,
            RightAddOnTemplateProperty,
            ContentLeftAddOnProperty,
            ContentLeftAddOnTemplateProperty,
            ContentRightAddOnProperty,
            ContentRightAddOnTemplateProperty);
    }

    public AddOnDecoratedBox()
    {
    }
    
    protected virtual void UpdatePseudoClasses()
    {
        PseudoClasses.Set(AddOnDecoratedBoxPseudoClass.Outline, StyleVariant == InputControlStyleVariant.Outlined);
        PseudoClasses.Set(AddOnDecoratedBoxPseudoClass.Filled, StyleVariant == InputControlStyleVariant.Filled);
        PseudoClasses.Set(AddOnDecoratedBoxPseudoClass.Borderless, StyleVariant == InputControlStyleVariant.Borderless);
        PseudoClasses.Set(AddOnDecoratedBoxPseudoClass.Underlined, StyleVariant == InputControlStyleVariant.Underlined);
    }
    
    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        base.OnPropertyChanged(change);

        InvalidateIconCache(change.Property);

        if (change.Property == StyleVariantProperty)
        {
            UpdatePseudoClasses();
        }

        if (change.Property == StyleVariantProperty ||
            change.Property == BorderThicknessProperty)
        {
            _borderThicknessDirty = true;
        }

        if (change.Property == LeftAddOnProperty ||
            change.Property == RightAddOnProperty ||
            change.Property == CornerRadiusProperty ||
            change.Property == StyleVariantProperty ||
            change.Property == CompactSpaceItemPositionProperty ||
            change.Property == CompactSpaceOrientationProperty)
        {
            _cornerRadiusDirty = true;
        }

        if (change.Property == CornerRadiusProperty ||
            change.Property == BorderThicknessProperty ||
            change.Property == StyleVariantProperty ||
            change.Property == CompactSpaceItemPositionProperty ||
            change.Property == CompactSpaceOrientationProperty)
        {
            _borderInfoDirty = true;
        }

        if (change.Property == StatusProperty ||
            change.Property == IsEffectivelyEnabledProperty ||
            change.Property == ContentLeftAddOnProperty ||
            change.Property == ContentRightAddOnProperty ||
            change.Property == AddOnStatusForegroundProperty ||
            change.Property == AddOnStatusIconBrushProperty)
        {
            UpdateIconStatusColors();
        }

        if (IsEffectiveInnerBoxBrushStateProperty(change.Property) ||
            IsInnerBoxBrushSourceProperty(change.Property))
        {
            ConfigureEffectiveInnerBoxBrushes();
        }

        if (_cornerRadiusDirty || _borderInfoDirty || _borderThicknessDirty)
        {
            ScheduleLayoutUpdate();
        }
    }

    private protected virtual bool IsEffectiveInnerBoxBrushStateProperty(AvaloniaProperty property)
    {
        return property == StyleVariantProperty ||
               property == StatusProperty ||
               property == IsEffectivelyEnabledProperty ||
               property == IsInnerBoxHoverProperty ||
               property == IsInnerBoxPressedProperty ||
               property == IsKeyboardFocusWithinProperty;
    }

    private static bool IsInnerBoxBrushSourceProperty(AvaloniaProperty property)
    {
        return property == InnerBoxDefaultBorderBrushProperty ||
               property == InnerBoxHoverBorderBrushProperty ||
               property == InnerBoxActiveBorderBrushProperty ||
               property == InnerBoxFilledBackgroundProperty ||
               property == InnerBoxFilledBorderBrushProperty ||
               property == InnerBoxFilledHoverBackgroundProperty ||
               property == InnerBoxActiveBackgroundProperty ||
               property == InnerBoxDisabledBackgroundProperty ||
               property == InnerBoxErrorBorderBrushProperty ||
               property == InnerBoxErrorHoverBorderBrushProperty ||
               property == InnerBoxErrorBackgroundProperty ||
               property == InnerBoxErrorFilledBorderBrushProperty ||
               property == InnerBoxErrorHoverBackgroundProperty ||
               property == InnerBoxWarningBorderBrushProperty ||
               property == InnerBoxWarningHoverBorderBrushProperty ||
               property == InnerBoxWarningBackgroundProperty ||
               property == InnerBoxWarningFilledBorderBrushProperty ||
               property == InnerBoxWarningHoverBackgroundProperty;
    }

    private protected virtual bool IsInnerBoxActive => IsInnerBoxPressed || IsKeyboardFocusWithin;

    private void ConfigureEffectiveInnerBoxBrushes()
    {
        EffectiveInnerBoxBorderBrush = CalculateEffectiveInnerBoxBorderBrush();
        EffectiveInnerBoxBackground  = CalculateEffectiveInnerBoxBackground();
    }

    private IBrush? CalculateEffectiveInnerBoxBorderBrush()
    {
        if (!IsEffectivelyEnabled)
        {
            return StyleVariant == InputControlStyleVariant.Borderless ? Brushes.Transparent : InnerBoxDefaultBorderBrush;
        }

        return StyleVariant switch
        {
            InputControlStyleVariant.Filled     => CalculateFilledInnerBoxBorderBrush(),
            InputControlStyleVariant.Borderless => Brushes.Transparent,
            _                                  => CalculateOutlineInnerBoxBorderBrush()
        };
    }

    private IBrush? CalculateOutlineInnerBoxBorderBrush()
    {
        if (IsInnerBoxActive)
        {
            return GetStatusActiveBorderBrush();
        }

        if (IsInnerBoxHover)
        {
            return GetStatusBorderBrush(hover: true);
        }

        return GetStatusBorderBrush(hover: false);
    }

    private IBrush? CalculateFilledInnerBoxBorderBrush()
    {
        if (IsInnerBoxActive)
        {
            return GetStatusActiveBorderBrush();
        }

        return Status switch
        {
            InputControlStatus.Error   => InnerBoxErrorFilledBorderBrush,
            InputControlStatus.Warning => InnerBoxWarningFilledBorderBrush,
            _                          => InnerBoxFilledBorderBrush
        };
    }

    private IBrush? GetStatusActiveBorderBrush()
    {
        return Status switch
        {
            InputControlStatus.Error   => InnerBoxErrorBorderBrush,
            InputControlStatus.Warning => InnerBoxWarningBorderBrush,
            _                          => InnerBoxActiveBorderBrush
        };
    }

    private IBrush? GetStatusBorderBrush(bool hover)
    {
        return Status switch
        {
            InputControlStatus.Error   => hover ? InnerBoxErrorHoverBorderBrush : InnerBoxErrorBorderBrush,
            InputControlStatus.Warning => hover ? InnerBoxWarningHoverBorderBrush : InnerBoxWarningBorderBrush,
            _                          => hover ? InnerBoxHoverBorderBrush : InnerBoxDefaultBorderBrush
        };
    }

    private IBrush? CalculateEffectiveInnerBoxBackground()
    {
        if (!IsEffectivelyEnabled)
        {
            return StyleVariant is InputControlStyleVariant.Outlined or InputControlStyleVariant.Filled
                ? InnerBoxDisabledBackground
                : Brushes.Transparent;
        }

        return StyleVariant == InputControlStyleVariant.Filled
            ? CalculateFilledInnerBoxBackground()
            : Brushes.Transparent;
    }

    private IBrush? CalculateFilledInnerBoxBackground()
    {
        if (IsKeyboardFocusWithin)
        {
            return InnerBoxActiveBackground;
        }

        if (IsInnerBoxHover)
        {
            return Status switch
            {
                InputControlStatus.Error   => InnerBoxErrorHoverBackground,
                InputControlStatus.Warning => InnerBoxWarningHoverBackground,
                _                          => InnerBoxFilledHoverBackground
            };
        }

        return Status switch
        {
            InputControlStatus.Error   => InnerBoxErrorBackground,
            InputControlStatus.Warning => InnerBoxWarningBackground,
            _                          => InnerBoxFilledBackground
        };
    }

    private void ScheduleLayoutUpdate()
    {
        if (_layoutUpdatePosted)
        {
            return;
        }
        if (!_borderInfoDirty && !_cornerRadiusDirty && !_borderThicknessDirty)
        {
            return;
        }
        _layoutUpdatePosted = true;
        Dispatcher.Post(ApplyDirtyLayoutUpdates, DispatcherPriority.Render);
    }

    private void ApplyDirtyLayoutUpdates()
    {
        _layoutUpdatePosted = false;
        if (!_cornerRadiusDirty && !_borderInfoDirty && !_borderThicknessDirty)
        {
            return;
        }

        if (_cornerRadiusDirty)
        {
            ConfigureInnerBoxCornerRadius();
            _cornerRadiusDirty = false;
        }
        if (_borderInfoDirty)
        {
            ConfigureAddOnBorderInfo();
            _borderInfoDirty = false;
        }
        if (_borderThicknessDirty)
        {
            ConfigureInnerBoxBorderThickness();
            _borderThicknessDirty = false;
        }
    }
    
    private void ConfigureAddOnBorderInfo()
    {
        var topLeftRadius     = CornerRadius.TopLeft;
        var topRightRadius    = CornerRadius.TopRight;
        var bottomLeftRadius  = CornerRadius.BottomLeft;
        var bottomRightRadius = CornerRadius.BottomRight;

        CornerRadius newLeftRadius;
        CornerRadius newRightRadius;

        if (IsUsedInCompactSpace && CompactSpaceItemPosition.HasValue &&
            (!CompactSpaceItemPosition.Value.HasFlag(SpaceItemPosition.First) ||
             !CompactSpaceItemPosition.Value.HasFlag(SpaceItemPosition.Last)))
        {
            if (CompactSpaceItemPosition.Value.HasFlag(SpaceItemPosition.First))
            {
                if (CompactSpaceOrientation == Orientation.Horizontal)
                {
                    newLeftRadius = new CornerRadius(topLeft:topLeftRadius,
                        topRight:0,
                        bottomLeft: bottomLeftRadius,
                        bottomRight: 0);
                    newRightRadius = new CornerRadius(topLeft:0,
                        topRight: 0,
                        bottomLeft: 0,
                        bottomRight: 0);
                }
                else
                {
                    newLeftRadius = new CornerRadius(topLeft:topLeftRadius,
                        topRight:0,
                        bottomLeft: 0,
                        bottomRight: 0);
                    newRightRadius = new CornerRadius(topLeft:0,
                        topRight:topRightRadius,
                        bottomLeft: 0,
                        bottomRight: 0);
                }
            }
            else if (CompactSpaceItemPosition.Value.HasFlag(SpaceItemPosition.Middle))
            {
                newLeftRadius = new CornerRadius(topLeft:0,
                    topRight:0,
                    bottomLeft: 0,
                    bottomRight: 0);
                newRightRadius = new CornerRadius(topLeft:0,
                    topRight:0,
                    bottomLeft: 0,
                    bottomRight: 0);
            }
            else // Last
            {
                if (CompactSpaceOrientation == Orientation.Horizontal)
                {
                    newLeftRadius = new CornerRadius(topLeft:0,
                        topRight:0,
                        bottomLeft: 0,
                        bottomRight: 0);
                    newRightRadius = new CornerRadius(topLeft:0,
                        topRight:topRightRadius,
                        bottomLeft: 0,
                        bottomRight: bottomRightRadius);
                }
                else
                {
                    newLeftRadius = new CornerRadius(topLeft:0,
                        topRight:0,
                        bottomLeft: bottomLeftRadius,
                        bottomRight: 0);
                    newRightRadius = new CornerRadius(topLeft:0,
                        topRight:0,
                        bottomLeft: 0,
                        bottomRight: bottomRightRadius);
                }
            }
        }
        else
        {
            newLeftRadius = new CornerRadius(topLeft:topLeftRadius,
                topRight:0,
                bottomLeft: bottomLeftRadius,
                bottomRight: 0);
            newRightRadius = new CornerRadius(topLeft:0,
                topRight:topRightRadius,
                bottomLeft: 0,
                bottomRight: bottomRightRadius);
        }

        if (LeftAddOnCornerRadius != newLeftRadius)
        {
            LeftAddOnCornerRadius = newLeftRadius;
        }
        if (RightAddOnCornerRadius != newRightRadius)
        {
            RightAddOnCornerRadius = newRightRadius;
        }

        Thickness newLeftThickness;
        Thickness newRightThickness;

        if (StyleVariant == InputControlStyleVariant.Outlined ||
            StyleVariant == InputControlStyleVariant.Filled)
        {
            var topThickness    = BorderThickness.Top;
            var rightThickness  = BorderThickness.Right;
            var bottomThickness = BorderThickness.Bottom;
            var leftThickness   = BorderThickness.Left;

            newLeftThickness =
                new Thickness(top: topThickness, right: 0, bottom: bottomThickness, left: leftThickness);
            newRightThickness =
                new Thickness(top: topThickness, right: rightThickness, bottom: bottomThickness, left: 0);
        }
        else if (StyleVariant == InputControlStyleVariant.Underlined)
        {
            newLeftThickness  = new Thickness(0);
            newRightThickness = new Thickness(0);
        }
        else
        {
            newLeftThickness  = LeftAddOnBorderThickness;
            newRightThickness = RightAddOnBorderThickness;
        }

        if (LeftAddOnBorderThickness != newLeftThickness)
        {
            LeftAddOnBorderThickness = newLeftThickness;
        }
        if (RightAddOnBorderThickness != newRightThickness)
        {
            RightAddOnBorderThickness = newRightThickness;
        }

        NotifyAddOnBorderInfoCalculated();
    }

    protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
    {
        base.OnApplyTemplate(e);
        UpdatePseudoClasses();

        // 取消旧的 ContentPresenter 订阅
        if (_contentLeftAddOn != null)
        {
            _contentLeftAddOn.PropertyChanged -= HandleContentPresenterChildChanged;
        }

        if (_contentRightAddOn != null)
        {
            _contentRightAddOn.PropertyChanged -= HandleContentPresenterChildChanged;
        }

        if (_leftAddOn is ContentPresenter oldLeftAddOn)
        {
            oldLeftAddOn.PropertyChanged -= HandleContentPresenterChildChanged;
        }

        if (_rightAddOn is ContentPresenter oldRightAddOn)
        {
            oldRightAddOn.PropertyChanged -= HandleContentPresenterChildChanged;
        }
        ClearPendingAddOnChildAttachListeners();

        _leftAddOn   = e.NameScope.Find<Control>("PART_LeftAddOn");
        _rightAddOn  = e.NameScope.Find<Control>("PART_RightAddOn");
        _contentLeftAddOn  = e.NameScope.Find<ContentPresenter>("PART_ContentLeftAddOn");
        _contentRightAddOn = e.NameScope.Find<ContentPresenter>("PART_ContentRightAddOn");
        InvalidateAllIconCaches();

        // 订阅新的 ContentPresenter Child 变化
        if (_contentLeftAddOn != null)
        {
            _contentLeftAddOn.PropertyChanged += HandleContentPresenterChildChanged;
        }

        if (_contentRightAddOn != null)
        {
            _contentRightAddOn.PropertyChanged += HandleContentPresenterChildChanged;
        }

        if (_leftAddOn is ContentPresenter newLeftAddOn)
        {
            newLeftAddOn.PropertyChanged += HandleContentPresenterChildChanged;
        }

        if (_rightAddOn is ContentPresenter newRightAddOn)
        {
            newRightAddOn.PropertyChanged += HandleContentPresenterChildChanged;
        }
        
        if (ContentFrame != null)
        {
            ContentFrame.PointerEntered  -= HandleContentFramePointerEnter;
            ContentFrame.PointerExited   -= HandleContentFramePointerExited;
            ContentFrame.PointerPressed  -= HandleContentFramePointerPressed;
            ContentFrame.PointerReleased -= HandleContentFramePointerReleased;
        }
        
        ContentFrame = e.NameScope.Find<Border>("PART_ContentFrame");
        if (ContentFrame != null)
        {
            ContentFrame.PointerEntered  += HandleContentFramePointerEnter;
            ContentFrame.PointerExited   += HandleContentFramePointerExited;
            ContentFrame.PointerPressed  += HandleContentFramePointerPressed;
            ContentFrame.PointerReleased += HandleContentFramePointerReleased;
        }

        ConfigureInnerBoxCornerRadius();
        ConfigureAddOnBorderInfo();
        ConfigureInnerBoxBorderThickness();
        ConfigureEffectiveInnerBoxBrushes();
        _cornerRadiusDirty    = false;
        _borderInfoDirty      = false;
        _borderThicknessDirty = false;
        UpdateIconStatusColors();
    }

    protected override void OnDetachedFromVisualTree(VisualTreeAttachmentEventArgs e)
    {
        base.OnDetachedFromVisualTree(e);
        ClearPendingAddOnChildAttachListeners();
    }

    private void HandleContentFramePointerEnter(object? sender, PointerEventArgs args)
    {
        IsInnerBoxHover = true;
    }
    
    private void HandleContentFramePointerExited(object? sender, PointerEventArgs args)
    {
        IsInnerBoxHover = false;
    }
    
    private void HandleContentFramePointerPressed(object? sender, PointerEventArgs args)
    {
        IsInnerBoxHover   = true;
        IsInnerBoxPressed = true;
    }
    
    private void HandleContentFramePointerReleased(object? sender, PointerEventArgs args)
    {
        IsInnerBoxPressed = false;
        IsInnerBoxHover   = true;
    }

    private void HandleContentPresenterChildChanged(object? sender, AvaloniaPropertyChangedEventArgs e)
    {
        if (e.Property == ContentPresenter.ChildProperty)
        {
            InvalidateIconCacheForPresenter(sender);

            if (e.OldValue is Control oldChild)
            {
                RemovePendingAddOnChildAttachListener(oldChild);
            }

            if (e.NewValue is Control newChild)
            {
                if (newChild.IsAttachedToVisualTree())
                {
                    UpdateIconStatusColors();
                }
                else
                {
                    AddPendingAddOnChildAttachListener(newChild);
                }
            }
            else
            {
                UpdateIconStatusColors();
            }
        }
    }

    private void HandleAddOnChildAttachedToVisualTree(object? sender, VisualTreeAttachmentEventArgs e)
    {
        if (sender is Control child)
        {
            RemovePendingAddOnChildAttachListener(child);
        }
        InvalidateAllIconCaches();
        UpdateIconStatusColors();
    }

    private void AddPendingAddOnChildAttachListener(Control child)
    {
        _pendingAddOnChildAttachListeners ??= new HashSet<Control>();
        if (_pendingAddOnChildAttachListeners.Add(child))
        {
            child.AttachedToVisualTree += HandleAddOnChildAttachedToVisualTree;
        }
    }

    private void RemovePendingAddOnChildAttachListener(Control child)
    {
        if (_pendingAddOnChildAttachListeners?.Remove(child) == true)
        {
            child.AttachedToVisualTree -= HandleAddOnChildAttachedToVisualTree;
        }
    }

    private void ClearPendingAddOnChildAttachListeners()
    {
        if (_pendingAddOnChildAttachListeners == null)
        {
            return;
        }

        foreach (var child in _pendingAddOnChildAttachListeners)
        {
            child.AttachedToVisualTree -= HandleAddOnChildAttachedToVisualTree;
        }
        _pendingAddOnChildAttachListeners = null;
    }

    private void UpdateIconStatusColors()
    {
#if DEBUG
        AddOnDecoratedBoxPerfProbe.RecordUpdateIconStatusColors();
#endif
        var foreground = AddOnStatusForeground;
        var iconBrush = AddOnStatusIconBrush;

        if (_hasAppliedAddOnStatusColors &&
            _appliedAddOnStatusContentVersion == _addOnStatusContentVersion &&
            Equals(_appliedAddOnStatusForeground, foreground) &&
            Equals(_appliedAddOnStatusIconBrush, iconBrush))
        {
            return;
        }

        // 应用 Foreground 到 addon 区域的 ContentPresenter
        ApplyAddOnForeground(_contentLeftAddOn, foreground);
        ApplyAddOnForeground(_contentRightAddOn, foreground);
        ApplyAddOnForeground(_leftAddOn as ContentPresenter, foreground);
        ApplyAddOnForeground(_rightAddOn as ContentPresenter, foreground);

        // 应用 Icon 染色
        ApplyIconBrush(_contentLeftAddOn, iconBrush, ref _contentLeftAddOnIcons);
        ApplyIconBrush(_contentRightAddOn, iconBrush, ref _contentRightAddOnIcons);
        ApplyIconBrush(_leftAddOn, iconBrush, ref _leftAddOnIcons);
        ApplyIconBrush(_rightAddOn, iconBrush, ref _rightAddOnIcons);

        _hasAppliedAddOnStatusColors        = true;
        _appliedAddOnStatusForeground       = foreground;
        _appliedAddOnStatusIconBrush        = iconBrush;
        _appliedAddOnStatusContentVersion   = _addOnStatusContentVersion;
    }

    private static void ApplyAddOnForeground(ContentPresenter? presenter, IBrush? brush)
    {
        if (presenter == null || (presenter.Content == null && presenter.Child == null))
        {
            return;
        }

        if (brush != null)
        {
            presenter.SetCurrentValue(ForegroundProperty, brush);
        }
        else
        {
            presenter.ClearValue(ForegroundProperty);
        }
    }

    private static bool HasAddOnContent(Control? container)
    {
        if (container == null)
        {
            return false;
        }

        return container is not ContentPresenter presenter || HasPresenterContent(presenter);
    }

    private static bool HasPresenterContent(ContentPresenter? presenter)
    {
        return presenter != null && (presenter.Content != null || presenter.Child != null);
    }

    private void InvalidateAllIconCaches()
    {
        _leftAddOnIcons         = null;
        _rightAddOnIcons        = null;
        _contentLeftAddOnIcons  = null;
        _contentRightAddOnIcons = null;
        _addOnStatusContentVersion++;
    }

    private bool InvalidateIconCache(AvaloniaProperty property)
    {
        var invalidated = false;

        if (property == LeftAddOnProperty ||
            property == LeftAddOnTemplateProperty)
        {
            _leftAddOnIcons = null;
            invalidated     = true;
        }

        if (property == RightAddOnProperty ||
            property == RightAddOnTemplateProperty)
        {
            _rightAddOnIcons = null;
            invalidated      = true;
        }

        if (property == ContentLeftAddOnProperty ||
            property == ContentLeftAddOnTemplateProperty)
        {
            _contentLeftAddOnIcons = null;
            invalidated            = true;
        }

        if (property == ContentRightAddOnProperty ||
            property == ContentRightAddOnTemplateProperty)
        {
            _contentRightAddOnIcons = null;
            invalidated             = true;
        }

        if (invalidated)
        {
            _addOnStatusContentVersion++;
        }

        return invalidated;
    }

    private void InvalidateIconCacheForPresenter(object? presenter)
    {
        var invalidated = false;

        if (ReferenceEquals(presenter, _contentLeftAddOn))
        {
            _contentLeftAddOnIcons = null;
            invalidated            = true;
        }
        else if (ReferenceEquals(presenter, _contentRightAddOn))
        {
            _contentRightAddOnIcons = null;
            invalidated             = true;
        }
        else if (ReferenceEquals(presenter, _leftAddOn))
        {
            _leftAddOnIcons = null;
            invalidated     = true;
        }
        else if (ReferenceEquals(presenter, _rightAddOn))
        {
            _rightAddOnIcons = null;
            invalidated      = true;
        }

        if (invalidated)
        {
            _addOnStatusContentVersion++;
        }
    }

    internal void NotifyContentRightAddOnVisualsChanged()
    {
        _contentRightAddOnIcons = null;
        _addOnStatusContentVersion++;
        UpdateIconStatusColors();
    }

    private static void ApplyIconBrush(Control? container, IBrush? brush, ref List<WeakReference<Icon>>? iconCache)
    {
        if (!HasAddOnContent(container))
        {
            return;
        }

        var target = container!;
#if DEBUG
        var isProbeEnabled = AddOnDecoratedBoxPerfProbe.IsEnabled;
        var scannedVisuals = 0;
        var matchedIcons   = 0;
#endif
        var iconRefs = iconCache;
        var rebuiltCache = false;
        if (iconRefs == null)
        {
            rebuiltCache = true;
            iconRefs     = new List<WeakReference<Icon>>();
            foreach (var descendant in target.GetVisualDescendants())
            {
                if (descendant is not Icon icon)
                {
#if DEBUG
                    if (isProbeEnabled)
                    {
                        scannedVisuals++;
                    }
#endif
                    continue;
                }
#if DEBUG
                if (isProbeEnabled)
                {
                    scannedVisuals++;
                    matchedIcons++;
                }
#endif
                iconRefs.Add(new WeakReference<Icon>(icon));
            }
            iconCache = iconRefs;
        }

        var hasStaleIcon = false;
        for (var i = iconRefs.Count - 1; i >= 0; i--)
        {
            if (!iconRefs[i].TryGetTarget(out var icon) ||
                (!rebuiltCache && !IsVisualDescendantOf(icon, target)))
            {
                iconRefs.RemoveAt(i);
                hasStaleIcon = true;
                continue;
            }

#if DEBUG
            if (!rebuiltCache && isProbeEnabled)
            {
                matchedIcons++;
            }
#endif
            if (icon.Classes.Contains("skip-status")) continue;
            if (brush != null)
            {
                icon.SetCurrentValue(Icon.FillBrushProperty, brush);
                icon.SetCurrentValue(Icon.StrokeBrushProperty, brush);
                icon.SetCurrentValue(Icon.ForegroundProperty, brush);
            }
            else
            {
                icon.ClearValue(Icon.FillBrushProperty);
                icon.ClearValue(Icon.StrokeBrushProperty);
                icon.ClearValue(Icon.ForegroundProperty);
            }
        }

        if (hasStaleIcon && iconRefs.Count == 0)
        {
            iconCache = null;
        }
#if DEBUG
        if (isProbeEnabled)
        {
            AddOnDecoratedBoxPerfProbe.RecordApplyIconBrush(scannedVisuals, matchedIcons);
        }
#endif
    }

    private static bool IsVisualDescendantOf(Visual visual, Visual ancestor)
    {
        foreach (var current in visual.GetVisualAncestors())
        {
            if (ReferenceEquals(current, ancestor))
            {
                return true;
            }
        }

        return false;
    }
    
    protected virtual void NotifyAddOnBorderInfoCalculated()
    {
    }
    
    private void ConfigureInnerBoxCornerRadius()
    {
        if (StyleVariant != InputControlStyleVariant.Underlined)
        {
            var topLeftRadius     = CornerRadius.TopLeft;
            var topRightRadius    = CornerRadius.TopRight;
            var bottomLeftRadius  = CornerRadius.BottomLeft;
            var bottomRightRadius = CornerRadius.BottomRight;

            if (_leftAddOn is not null && _leftAddOn.IsVisible)
            {
                topLeftRadius    = 0;
                bottomLeftRadius = 0;
            }

            if (_rightAddOn is not null && _rightAddOn.IsVisible)
            {
                topRightRadius    = 0;
                bottomRightRadius = 0;
            }

            if (IsUsedInCompactSpace && CompactSpaceItemPosition.HasValue &&
                (!CompactSpaceItemPosition.Value.HasFlag(SpaceItemPosition.First) || !CompactSpaceItemPosition.Value.HasFlag(SpaceItemPosition.Last)))
            {
                if (CompactSpaceItemPosition.Value.HasFlag(SpaceItemPosition.First))
                {
                    if (CompactSpaceOrientation == Orientation.Horizontal)
                    {
                        topRightRadius    = 0;
                        bottomRightRadius = 0;
                    }
                    else
                    {
                        bottomLeftRadius  = 0;
                        bottomRightRadius = 0;
                    }
                }
                else if (CompactSpaceItemPosition.Value.HasFlag(SpaceItemPosition.Middle))
                {
                     topLeftRadius     = 0;
                     topRightRadius    = 0;
                     bottomLeftRadius  = 0;
                     bottomRightRadius = 0;
                }
                else if (CompactSpaceItemPosition.Value.HasFlag(SpaceItemPosition.Last))
                {
                    if (CompactSpaceOrientation == Orientation.Horizontal)
                    {
                        topLeftRadius    = 0;
                        bottomLeftRadius = 0;
                    }
                    else
                    {
                        topLeftRadius = 0;
                        topRightRadius = 0;
                    }
                }
            }
            
            SetCurrentValue(InnerBoxCornerRadiusProperty, new CornerRadius(topLeftRadius,
                topRightRadius,
                bottomLeft: bottomLeftRadius,
                bottomRight: bottomRightRadius));
        }
        else
        {
            SetCurrentValue(InnerBoxCornerRadiusProperty, new CornerRadius(0));
        }
    }

    private void ConfigureInnerBoxBorderThickness()
    {
        if (StyleVariant == InputControlStyleVariant.Borderless)
        {
            SetCurrentValue(InnerBoxBorderThicknessProperty, new Thickness(0));
        }
        else if (StyleVariant == InputControlStyleVariant.Underlined)
        {
            SetCurrentValue(InnerBoxBorderThicknessProperty, new Thickness(0, 0, 0, BorderThickness.Bottom));
        }
        else
        {
            SetCurrentValue(InnerBoxBorderThicknessProperty, BorderThickness);
        }
    }
    

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
}
