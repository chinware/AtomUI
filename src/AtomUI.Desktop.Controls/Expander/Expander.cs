using System.Diagnostics;
using AtomUI.Controls;
using AtomUI.Icons.AntDesign;
using AtomUI.MotionScene;
using AtomUI.Theme;
using Avalonia;
using Avalonia.Animation.Easings;
using Avalonia.Controls;
using Avalonia.Controls.Metadata;
using Avalonia.Controls.Primitives;
using Avalonia.Controls.Templates;
using Avalonia.Data;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.VisualTree;

namespace AtomUI.Desktop.Controls;

using AvaloniaExpander = Avalonia.Controls.Expander;

public enum ExpanderTriggerType
{
    Header,
    Icon
}

public enum ExpanderIconPosition
{
    Start,
    End
}

[PseudoClasses(
    ExpanderPseudoClass.Expanded,
    ExpanderPseudoClass.ExpandUp, 
    ExpanderPseudoClass.ExpandDown,
    ExpanderPseudoClass.ExpandLeft,
    ExpanderPseudoClass.ExpandRight)]
public class Expander : AvaloniaExpander, IMotionAwareControl
{
    #region 公共属性定义

    public static readonly StyledProperty<CustomizableSizeType> SizeTypeProperty =
        CustomizableSizeTypeControlProperty.SizeTypeProperty.AddOwner<Expander>();

    public static readonly StyledProperty<bool> IsShowExpandIconProperty =
        AvaloniaProperty.Register<Expander, bool>(nameof(IsShowExpandIcon), true);

    public static readonly StyledProperty<PathIcon?> ExpandIconProperty =
        AvaloniaProperty.Register<Expander, PathIcon?>(nameof(ExpandIcon));

    public static readonly StyledProperty<object?> AddOnContentProperty =
        AvaloniaProperty.Register<Expander, object?>(nameof(AddOnContent));

    public static readonly StyledProperty<IDataTemplate?> AddOnContentTemplateProperty =
        AvaloniaProperty.Register<Expander, IDataTemplate?>(nameof(AddOnContentTemplate));

    public static readonly StyledProperty<bool> IsGhostStyleProperty =
        AvaloniaProperty.Register<Expander, bool>(nameof(IsGhostStyle));

    public static readonly StyledProperty<bool> IsBorderlessProperty =
        AvaloniaProperty.Register<Expander, bool>(nameof(IsBorderless));

    public static readonly StyledProperty<ExpanderTriggerType> TriggerTypeProperty =
        AvaloniaProperty.Register<Expander, ExpanderTriggerType>(nameof(TriggerType));

    public static readonly StyledProperty<ExpanderIconPosition> ExpandIconPositionProperty =
        AvaloniaProperty.Register<Expander, ExpanderIconPosition>(nameof(ExpandIconPosition));
    
    public static readonly StyledProperty<Thickness?> HeaderPaddingProperty =
        AvaloniaProperty.Register<Expander, Thickness?>(nameof(HeaderPadding));
    
    public static readonly StyledProperty<Thickness?> ContentPaddingProperty =
        AvaloniaProperty.Register<Expander, Thickness?>(nameof(ContentPadding));

    public static readonly StyledProperty<bool> IsMotionEnabledProperty =
        MotionAwareControlProperty.IsMotionEnabledProperty.AddOwner<Expander>();
    
    public CustomizableSizeType SizeType
    {
        get => GetValue(SizeTypeProperty);
        set => SetValue(SizeTypeProperty, value);
    }

    public bool IsShowExpandIcon
    {
        get => GetValue(IsShowExpandIconProperty);
        set => SetValue(IsShowExpandIconProperty, value);
    }

    public PathIcon? ExpandIcon
    {
        get => GetValue(ExpandIconProperty);
        set => SetValue(ExpandIconProperty, value);
    }

    public object? AddOnContent
    {
        get => GetValue(AddOnContentProperty);
        set => SetValue(AddOnContentProperty, value);
    }

    public IDataTemplate? AddOnContentTemplate
    {
        get => GetValue(AddOnContentTemplateProperty);
        set => SetValue(AddOnContentTemplateProperty, value);
    }

    public bool IsGhostStyle
    {
        get => GetValue(IsGhostStyleProperty);
        set => SetValue(IsGhostStyleProperty, value);
    }

    public bool IsBorderless
    {
        get => GetValue(IsBorderlessProperty);
        set => SetValue(IsBorderlessProperty, value);
    }

    public ExpanderTriggerType TriggerType
    {
        get => GetValue(TriggerTypeProperty);
        set => SetValue(TriggerTypeProperty, value);
    }

    public ExpanderIconPosition ExpandIconPosition
    {
        get => GetValue(ExpandIconPositionProperty);
        set => SetValue(ExpandIconPositionProperty, value);
    }
    
    public Thickness? HeaderPadding
    {
        get => GetValue(HeaderPaddingProperty);
        set => SetValue(HeaderPaddingProperty, value);
    }

    public Thickness? ContentPadding
    {
        get => GetValue(ContentPaddingProperty);
        set => SetValue(ContentPaddingProperty, value);
    }
    
    public bool IsMotionEnabled
    {
        get => GetValue(IsMotionEnabledProperty);
        set => SetValue(IsMotionEnabledProperty, value);
    }
    
    #endregion

    #region 内部属性定义

    internal static readonly StyledProperty<Thickness> ContentBorderThicknessProperty =
        AvaloniaProperty.Register<Expander, Thickness>(nameof(ContentBorderThickness));
    
    internal static readonly StyledProperty<TimeSpan> MotionDurationProperty =
        MotionAwareControlProperty.MotionDurationProperty.AddOwner<Expander>();
    
    internal static readonly DirectProperty<Expander, Thickness> EffectiveBorderThicknessProperty =
        AvaloniaProperty.RegisterDirect<Expander, Thickness>(nameof(EffectiveBorderThickness),
            o => o.EffectiveBorderThickness,
            (o, v) => o.EffectiveBorderThickness = v);

    internal static readonly DirectProperty<Expander, Thickness> EffectiveExpandButtonMarginProperty =
        AvaloniaProperty.RegisterDirect<Expander, Thickness>(nameof(EffectiveExpandButtonMargin),
            o => o.EffectiveExpandButtonMargin,
            (o, v) => o.EffectiveExpandButtonMargin = v);

    internal Thickness ContentBorderThickness
    {
        get => GetValue(ContentBorderThicknessProperty);
        set => SetValue(ContentBorderThicknessProperty, value);
    }

    internal TimeSpan MotionDuration
    {
        get => GetValue(MotionDurationProperty);
        set => SetValue(MotionDurationProperty, value);
    }

    private Thickness _effectiveBorderThickness;

    internal Thickness EffectiveBorderThickness
    {
        get => _effectiveBorderThickness;
        set => SetAndRaise(EffectiveBorderThicknessProperty, ref _effectiveBorderThickness, value);
    }

    private Thickness _effectiveExpandButtonMargin;

    internal Thickness EffectiveExpandButtonMargin
    {
        get => _effectiveExpandButtonMargin;
        set => SetAndRaise(EffectiveExpandButtonMarginProperty, ref _effectiveExpandButtonMargin, value);
    }

    #endregion

    private static readonly CubicEaseOut DefaultExpandMotionEasing = new();
    private static readonly CubicEaseIn DefaultCollapseMotionEasing = new();

    private BaseMotionActor? _motionActor;
    private Control? _headerDecorator;
    private IconButton? _expandButton;
    private CancellationTokenSource? _contentMotionCancellation;

    public Expander()
    {
    }
    
    protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
    {
        base.OnApplyTemplate(e);

        if (_expandButton is not null)
        {
            _expandButton.Click -= HandleExpandButtonClicked;
        }

        CancelContentMotionAndClearValues();
        _motionActor     = e.NameScope.Find<BaseMotionActor>("PART_ContentMotionActor");
        _headerDecorator = e.NameScope.Find<Control>("PART_HeaderDecorator");
        _expandButton    = e.NameScope.Find<IconButton>("PART_ExpandButton");

        if (_motionActor is not null)
        {
            ApplyContentStableState(_motionActor, IsExpanded);
        }

        if (_expandButton is not null)
        {
            _expandButton.Click += HandleExpandButtonClicked;
        }

        SetupEffectiveBorderThickness();
        ConfigureContentBorderThickness();
        UpdateEffectiveExpandButtonMargin();
        SetupDefaultIcon();
        UpdatePseudoClasses();
    }

    protected override void OnDetachedFromVisualTree(VisualTreeAttachmentEventArgs e)
    {
        base.OnDetachedFromVisualTree(e);
        if (_motionActor is { } motionActor)
        {
            ApplyContentStableState(motionActor, IsExpanded);
        }
    }

    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        base.OnPropertyChanged(change);
        if (this.IsAttachedToVisualTree())
        {
            if (change.Property == ExpandIconProperty)
            {
                SetupDefaultIcon();
            }
        }

        if (change.Property == IsExpandedProperty)
        {
            UpdateContentVisibility(IsExpanded);
        }

        if (change.Property == BorderThicknessProperty ||
            change.Property == IsGhostStyleProperty ||
            change.Property == IsBorderlessProperty ||
            change.Property == ExpandDirectionProperty)
        {
            SetupEffectiveBorderThickness();
            ConfigureContentBorderThickness();
        }

        if (change.Property == HeaderPaddingProperty ||
            change.Property == ExpandIconPositionProperty)
        {
            UpdateEffectiveExpandButtonMargin();
        }

        if (change.Property == ContentPaddingProperty ||
            change.Property == HeaderPaddingProperty)
        {
            UpdatePseudoClasses();
        }
    }

    protected override void OnPointerPressed(PointerPressedEventArgs e)
    {
        base.OnPointerPressed(e);

        if (!CanToggleFromHeaderPointer(e))
        {
            return;
        }

        ToggleExpanded();
        e.Handled = true;
    }

    private void HandleExpandButtonClicked(object? sender, RoutedEventArgs args)
    {
        ToggleExpanded();
        args.Handled = true;
    }

    private bool CanToggleFromHeaderPointer(PointerPressedEventArgs e)
    {
        if (TriggerType != ExpanderTriggerType.Header ||
            e.Pointer.Type != PointerType.Mouse ||
            !e.GetCurrentPoint(this).Properties.IsLeftButtonPressed ||
            _headerDecorator is null)
        {
            return false;
        }

        var position = e.GetPosition(_headerDecorator);
        var bounds   = new Rect(_headerDecorator.Bounds.Size);
        return bounds.Contains(position);
    }

    private void ToggleExpanded()
    {
        IsExpanded = !IsExpanded;
    }

    private void SetupDefaultIcon()
    {
        if (ExpandIcon is null)
        {
            ClearValue(ExpandIconProperty);
            SetValue(ExpandIconProperty, new RightOutlined(), BindingPriority.Template);
        }

        Debug.Assert(ExpandIcon is not null);
    }

    private void ConfigureContentBorderThickness()
    {
        if (IsBorderless || IsGhostStyle)
        {
            ContentBorderThickness = default;
            return;
        }

        var line = BorderThickness.Bottom;
        ContentBorderThickness = ExpandDirection switch
        {
            ExpandDirection.Down => new Thickness(0, line, 0, 0),
            ExpandDirection.Up => new Thickness(0, 0, 0, line),
            ExpandDirection.Left => new Thickness(0, 0, line, 0),
            ExpandDirection.Right => new Thickness(line, 0, 0, 0),
            _ => throw new ArgumentOutOfRangeException(nameof(ExpandDirection), ExpandDirection,
                "Invalid value for ExpandDirection")
        };
    }

    private void UpdateContentVisibility(bool isVisible)
    {
        var motionActor = _motionActor;
        if (motionActor is null)
        {
            return;
        }

        if (!IsMotionEnabled)
        {
            ApplyContentStableState(motionActor, isVisible);
            return;
        }

        if (!isVisible && !motionActor.IsVisible && _contentMotionCancellation is null)
        {
            ApplyContentStableState(motionActor, false);
            return;
        }

        var cancellation = BeginContentMotion();
        Dispatcher.InvokeAsync(async () => await RunContentMotionAsync(motionActor, isVisible, cancellation));
    }

    private async Task RunContentMotionAsync(BaseMotionActor motionActor,
                                             bool targetVisible,
                                             CancellationTokenSource cancellation)
    {
        try
        {
            await RunContentLayoutMotionAsync(motionActor, targetVisible, cancellation.Token);
        }
        catch (OperationCanceledException) when (cancellation.IsCancellationRequested)
        {
        }
        finally
        {
            CompleteContentMotion(motionActor, targetVisible, cancellation);
        }
    }

    private async Task RunContentLayoutMotionAsync(BaseMotionActor motionActor,
                                                   bool targetVisible,
                                                   CancellationToken cancellationToken)
    {
        ClearContentMotionValues(motionActor);
        AbstractMotion motion = targetVisible
            ? new ExpandMotion(DirectionFromExpandDirection(ExpandDirection), MotionDuration, DefaultExpandMotionEasing)
            : new CollapseMotion(DirectionFromExpandDirection(ExpandDirection), MotionDuration, DefaultCollapseMotionEasing);
        await motion.RunAsync(motionActor,
            targetVisible ? () => motionActor.SetCurrentValue(IsVisibleProperty, true) : null,
            cancellationToken);
    }

    private CancellationTokenSource BeginContentMotion()
    {
        CancelContentMotion();
        var cancellation = new CancellationTokenSource();
        _contentMotionCancellation = cancellation;
        return cancellation;
    }

    private void CompleteContentMotion(BaseMotionActor motionActor,
                                       bool targetVisible,
                                       CancellationTokenSource cancellation)
    {
        if (!IsCurrentContentMotion(cancellation))
        {
            cancellation.Dispose();
            return;
        }

        _contentMotionCancellation = null;
        if (!ReferenceEquals(_motionActor, motionActor) || cancellation.IsCancellationRequested)
        {
            cancellation.Dispose();
            return;
        }

        cancellation.Dispose();
        if (IsExpanded == targetVisible)
        {
            ApplyContentStableState(motionActor, targetVisible);
        }
        else
        {
            UpdateContentVisibility(IsExpanded);
        }
    }

    private void CancelContentMotion()
    {
        var cancellation = _contentMotionCancellation;
        if (cancellation is not null)
        {
            _contentMotionCancellation = null;
            cancellation.Cancel();
        }
    }

    private void CancelContentMotionAndClearValues()
    {
        CancelContentMotion();
        if (_motionActor is { } motionActor)
        {
            ClearContentMotionValues(motionActor);
        }
    }

    private bool IsCurrentContentMotion(CancellationTokenSource cancellation)
    {
        return ReferenceEquals(_contentMotionCancellation, cancellation);
    }

    private void ApplyContentStableState(BaseMotionActor motionActor, bool isVisible)
    {
        CancelContentMotion();
        ClearContentMotionValues(motionActor);
        motionActor.Opacity   = isVisible ? 1.0 : 0.0;
        motionActor.IsVisible = isVisible;
    }

    private static void ClearContentMotionValues(BaseMotionActor motionActor)
    {
        motionActor.Transitions               = null;
        motionActor.MotionTransform           = null;
        motionActor.MotionTransformOperations = null;
        motionActor.ClearValue(HeightProperty);
    }

    private static Direction DirectionFromExpandDirection(ExpandDirection expandDirection)
    {
        return expandDirection switch
        {
            ExpandDirection.Left => Direction.Left,
            ExpandDirection.Up => Direction.Top,
            ExpandDirection.Right => Direction.Right,
            ExpandDirection.Down => Direction.Bottom,
            _ => throw new ArgumentOutOfRangeException(nameof(expandDirection), expandDirection,
                "Invalid value for ExpandDirection")
        };
    }

    private void SetupEffectiveBorderThickness()
    {
        if (IsBorderless || IsGhostStyle)
        {
            EffectiveBorderThickness = default;
        }
        else
        {
            EffectiveBorderThickness = BorderThickness;
        }
    }

    private void UpdateEffectiveExpandButtonMargin()
    {
        if (HeaderPadding is not { } headerPadding)
        {
            EffectiveExpandButtonMargin = default;
            return;
        }

        EffectiveExpandButtonMargin = ExpandIconPosition == ExpanderIconPosition.Start
            ? new Thickness(0, 0, headerPadding.Left, 0)
            : new Thickness(headerPadding.Right, 0, 0, 0);
    }
    
    private void UpdatePseudoClasses()
    {
        PseudoClasses.Set(ExpanderPseudoClass.CustomHeaderPadding, HeaderPadding != null);
        PseudoClasses.Set(ExpanderPseudoClass.CustomContentPadding, ContentPadding != null);
    }
}
