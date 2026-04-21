using AtomUI.Animations;
using AtomUI.Controls;
using AtomUI.Desktop.Controls.Primitives.Themes;
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

    internal static readonly DirectProperty<AddOnDecoratedBox, Thickness> InnerBoxBorderThicknessProperty =
        AvaloniaProperty.RegisterDirect<AddOnDecoratedBox, Thickness>(nameof(InnerBoxBorderThickness),
            o => o.InnerBoxBorderThickness,
            (o, v) => o.InnerBoxBorderThickness = v);

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

    internal Border? ContentFrame;
    
    static AddOnDecoratedBox()
    {
        AffectsRender<AddOnDecoratedBox>(BorderBrushProperty, BackgroundProperty);
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

        if (change.Property == StyleVariantProperty)
        {
            UpdatePseudoClasses();
        }

        if (this.IsAttachedToVisualTree())
        {
            if (change.Property == StyleVariantProperty)
            {
                ConfigureInnerBoxBorderThickness();
            }

            if (change.Property == LeftAddOnProperty ||
                change.Property == RightAddOnProperty ||
                change.Property == CornerRadiusProperty ||
                change.Property == StyleVariantProperty ||
                change.Property == CompactSpaceItemPositionProperty ||
                change.Property == CompactSpaceOrientationProperty)
            {
                ConfigureInnerBoxCornerRadius();
            }

            if (change.Property == BorderThicknessProperty)
            {
                ConfigureInnerBoxBorderThickness();
            }

            if (change.Property == CornerRadiusProperty ||
                change.Property == BorderThicknessProperty ||
                change.Property == StyleVariantProperty ||
                change.Property == CompactSpaceItemPositionProperty ||
                change.Property == CompactSpaceOrientationProperty)
            {
                ConfigureAddOnBorderInfo();
            }

            if (change.Property == StatusProperty ||
                change.Property == IsEnabledProperty ||
                change.Property == ContentLeftAddOnProperty ||
                change.Property == ContentRightAddOnProperty ||
                change.Property == AddOnStatusForegroundProperty ||
                change.Property == AddOnStatusIconBrushProperty)
            {
                UpdateIconStatusColors();
            }
        }
    }
    
    private void ConfigureAddOnBorderInfo()
    {
        var topLeftRadius     = CornerRadius.TopLeft;
        var topRightRadius    = CornerRadius.TopRight;
        var bottomLeftRadius  = CornerRadius.BottomLeft;
        var bottomRightRadius = CornerRadius.BottomRight;
        
        if (IsUsedInCompactSpace && CompactSpaceItemPosition.HasValue &&
            (!CompactSpaceItemPosition.Value.HasFlag(SpaceItemPosition.First) || 
             !CompactSpaceItemPosition.Value.HasFlag(SpaceItemPosition.Last)))
        {
            if (CompactSpaceItemPosition.Value.HasFlag(SpaceItemPosition.First))
            {
                if (CompactSpaceOrientation == Orientation.Horizontal)
                {
                    LeftAddOnCornerRadius = new CornerRadius(topLeft:topLeftRadius,
                        topRight:0,
                        bottomLeft: bottomLeftRadius,
                        bottomRight: 0);
                    RightAddOnCornerRadius = new CornerRadius(topLeft:0,
                        topRight: 0,
                        bottomLeft: 0,
                        bottomRight: 0);
                }
                else
                {
                    LeftAddOnCornerRadius = new CornerRadius(topLeft:topLeftRadius,
                        topRight:0,
                        bottomLeft: 0,
                        bottomRight: 0);
                    RightAddOnCornerRadius = new CornerRadius(topLeft:0,
                        topRight:topRightRadius,
                        bottomLeft: 0,
                        bottomRight: 0);
                }
            }
            else if (CompactSpaceItemPosition.Value.HasFlag(SpaceItemPosition.Middle))
            {
                LeftAddOnCornerRadius = new CornerRadius(topLeft:0,
                    topRight:0,
                    bottomLeft: 0,
                    bottomRight: 0);
                RightAddOnCornerRadius = new CornerRadius(topLeft:0,
                    topRight:0,
                    bottomLeft: 0,
                    bottomRight: 0);
            }
            else if (CompactSpaceItemPosition.Value.HasFlag(SpaceItemPosition.Last))
            {
                if (CompactSpaceOrientation == Orientation.Horizontal)
                {
                    LeftAddOnCornerRadius = new CornerRadius(topLeft:0,
                        topRight:0,
                        bottomLeft: 0,
                        bottomRight: 0);
                    RightAddOnCornerRadius = new CornerRadius(topLeft:0,
                        topRight:topRightRadius,
                        bottomLeft: 0,
                        bottomRight: bottomRightRadius);
                }
                else
                {
                    LeftAddOnCornerRadius = new CornerRadius(topLeft:0,
                        topRight:0,
                        bottomLeft: bottomLeftRadius,
                        bottomRight: 0);
                    RightAddOnCornerRadius = new CornerRadius(topLeft:0,
                        topRight:0,
                        bottomLeft: 0,
                        bottomRight: bottomRightRadius);
                }
            }
        }
        else
        {
            LeftAddOnCornerRadius = new CornerRadius(topLeft:topLeftRadius,
                topRight:0,
                bottomLeft: bottomLeftRadius,
                bottomRight: 0);
            RightAddOnCornerRadius = new CornerRadius(topLeft:0,
                topRight:topRightRadius,
                bottomLeft: 0,
                bottomRight: bottomRightRadius);
        }
        
        if (StyleVariant == InputControlStyleVariant.Outlined ||
            StyleVariant == InputControlStyleVariant.Filled)
        {
            var topThickness    = BorderThickness.Top;
            var rightThickness  = BorderThickness.Right;
            var bottomThickness = BorderThickness.Bottom;
            var leftThickness   = BorderThickness.Left;
            
            LeftAddOnBorderThickness =
                new Thickness(top: topThickness, right: 0, bottom: bottomThickness, left: leftThickness);
            RightAddOnBorderThickness =
                new Thickness(top: topThickness, right: rightThickness, bottom: bottomThickness, left: 0);
        }
        else if (StyleVariant == InputControlStyleVariant.Underlined)
        {
            LeftAddOnBorderThickness  = new Thickness(0);
            RightAddOnBorderThickness = new Thickness(0);
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

        _leftAddOn   = e.NameScope.Find<Control>(AddOnDecoratedBoxThemeConstants.LeftAddOnPart);
        _rightAddOn  = e.NameScope.Find<Control>(AddOnDecoratedBoxThemeConstants.RightAddOnPart);
        _contentLeftAddOn  = e.NameScope.Find<ContentPresenter>(AddOnDecoratedBoxThemeConstants.ContentLeftAddOnPart);
        _contentRightAddOn = e.NameScope.Find<ContentPresenter>(AddOnDecoratedBoxThemeConstants.ContentRightAddOnPart);

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
        
        ContentFrame = e.NameScope.Find<Border>(AddOnDecoratedBoxThemeConstants.ContentFramePart);
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
        UpdateIconStatusColors();
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
            if (e.OldValue is Control oldChild)
            {
                oldChild.AttachedToVisualTree -= HandleAddOnChildAttachedToVisualTree;
            }

            if (e.NewValue is Control newChild)
            {
                if (newChild.IsAttachedToVisualTree())
                {
                    UpdateIconStatusColors();
                }
                else
                {
                    newChild.AttachedToVisualTree += HandleAddOnChildAttachedToVisualTree;
                }
            }
        }
    }

    private void HandleAddOnChildAttachedToVisualTree(object? sender, VisualTreeAttachmentEventArgs e)
    {
        if (sender is Control child)
        {
            child.AttachedToVisualTree -= HandleAddOnChildAttachedToVisualTree;
        }
        UpdateIconStatusColors();
    }

    private void UpdateIconStatusColors()
    {
        var foreground = AddOnStatusForeground;
        var iconBrush = AddOnStatusIconBrush;

        // 应用 Foreground 到 addon 区域的 ContentPresenter
        ApplyAddOnForeground(_contentLeftAddOn, foreground);
        ApplyAddOnForeground(_contentRightAddOn, foreground);
        ApplyAddOnForeground(_leftAddOn as ContentPresenter, foreground);
        ApplyAddOnForeground(_rightAddOn as ContentPresenter, foreground);

        // 应用 Icon 染色
        ApplyIconBrush(_contentLeftAddOn, iconBrush);
        ApplyIconBrush(_contentRightAddOn, iconBrush);
        ApplyIconBrush(_leftAddOn, iconBrush);
        ApplyIconBrush(_rightAddOn, iconBrush);
    }

    private static void ApplyAddOnForeground(ContentPresenter? presenter, IBrush? brush)
    {
        if (presenter == null) return;
        if (brush != null)
        {
            presenter.SetCurrentValue(ForegroundProperty, brush);
        }
        else
        {
            presenter.ClearValue(ForegroundProperty);
        }
    }

    private static void ApplyIconBrush(Control? container, IBrush? brush)
    {
        if (container == null)
        {
            return;
        }
        foreach (var icon in container.GetVisualDescendants().OfType<Icon>())
        {
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
        this.EnableTransitions();
    }
}