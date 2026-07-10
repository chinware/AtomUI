using System.Diagnostics;
using AtomUI.Animations;
using AtomUI.Controls;
using AtomUI.Icons.AntDesign;
using AtomUI.MotionScene;
using Avalonia;
using Avalonia.Animation.Easings;
using Avalonia.Automation.Peers;
using Avalonia.Controls;
using Avalonia.Controls.Metadata;
using Avalonia.Controls.Mixins;
using Avalonia.Controls.Primitives;
using Avalonia.Controls.Templates;
using Avalonia.Data;
using Avalonia.Interactivity;
using Avalonia.VisualTree;

namespace AtomUI.Desktop.Controls;

[PseudoClasses(StdPseudoClass.Pressed, StdPseudoClass.Selected)]
public class CollapseItem : HeaderedContentControl, ISelectable
{
    #region 公共属性定义

    public static readonly StyledProperty<bool> IsSelectedProperty =
        SelectingItemsControl.IsSelectedProperty.AddOwner<CollapseItem>();

    public static readonly StyledProperty<bool> IsShowExpandIconProperty =
        AvaloniaProperty.Register<CollapseItem, bool>(nameof(IsShowExpandIcon), true);

    public static readonly StyledProperty<PathIcon?> ExpandIconProperty =
        AvaloniaProperty.Register<CollapseItem, PathIcon?>(nameof(ExpandIcon));

    public static readonly StyledProperty<object?> AddOnContentProperty =
        AvaloniaProperty.Register<CollapseItem, object?>(nameof(AddOnContent));

    public static readonly StyledProperty<IDataTemplate?> AddOnContentTemplateProperty =
        AvaloniaProperty.Register<CollapseItem, IDataTemplate?>(nameof(AddOnContentTemplate));
    
    public static readonly StyledProperty<Thickness> HeaderPaddingProperty =
        AvaloniaProperty.Register<CollapseItem, Thickness>(nameof(HeaderPadding));

    public static readonly StyledProperty<Thickness> ContentPaddingProperty =
        AvaloniaProperty.Register<CollapseItem, Thickness>(nameof(ContentPadding));

    public bool IsSelected
    {
        get => GetValue(IsSelectedProperty);
        set => SetValue(IsSelectedProperty, value);
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
    
    public Thickness HeaderPadding
    {
        get => GetValue(HeaderPaddingProperty);
        set => SetValue(HeaderPaddingProperty, value);
    }

    public Thickness ContentPadding
    {
        get => GetValue(ContentPaddingProperty);
        set => SetValue(ContentPaddingProperty, value);
    }
    #endregion

    #region 内部属性定义

    internal static readonly StyledProperty<CustomizableSizeType> SizeTypeProperty =
        CustomizableSizeTypeControlProperty.SizeTypeProperty.AddOwner<CollapseItem>();

    internal static readonly StyledProperty<Thickness> DefaultHeaderPaddingProperty =
        AvaloniaProperty.Register<CollapseItem, Thickness>(nameof(DefaultHeaderPadding));

    internal static readonly StyledProperty<Thickness> DefaultContentPaddingProperty =
        AvaloniaProperty.Register<CollapseItem, Thickness>(nameof(DefaultContentPadding));

    internal static readonly DirectProperty<CollapseItem, Thickness?> OwnerHeaderPaddingProperty =
        AvaloniaProperty.RegisterDirect<CollapseItem, Thickness?>(nameof(OwnerHeaderPadding),
            o => o.OwnerHeaderPadding,
            (o, v) => o.OwnerHeaderPadding = v);

    internal static readonly DirectProperty<CollapseItem, Thickness?> OwnerContentPaddingProperty =
        AvaloniaProperty.RegisterDirect<CollapseItem, Thickness?>(nameof(OwnerContentPadding),
            o => o.OwnerContentPadding,
            (o, v) => o.OwnerContentPadding = v);

    internal static readonly DirectProperty<CollapseItem, Thickness> EffectiveHeaderPaddingProperty =
        AvaloniaProperty.RegisterDirect<CollapseItem, Thickness>(nameof(EffectiveHeaderPadding),
            o => o.EffectiveHeaderPadding,
            (o, v) => o.EffectiveHeaderPadding = v);

    internal static readonly DirectProperty<CollapseItem, Thickness> EffectiveContentPaddingProperty =
        AvaloniaProperty.RegisterDirect<CollapseItem, Thickness>(nameof(EffectiveContentPadding),
            o => o.EffectiveContentPadding,
            (o, v) => o.EffectiveContentPadding = v);

    internal static readonly DirectProperty<CollapseItem, bool> IsGhostStyleProperty =
        AvaloniaProperty.RegisterDirect<CollapseItem, bool>(nameof(IsGhostStyle),
            o => o.IsGhostStyle,
            (o, v) => o.IsGhostStyle = v);

    internal static readonly DirectProperty<CollapseItem, bool> IsBorderlessProperty =
        AvaloniaProperty.RegisterDirect<CollapseItem, bool>(nameof(IsBorderless),
            o => o.IsBorderless,
            (o, v) => o.IsBorderless = v);

    internal static readonly DirectProperty<CollapseItem, CollapseTriggerType> TriggerTypeProperty =
        AvaloniaProperty.RegisterDirect<CollapseItem, CollapseTriggerType>(nameof(TriggerType),
            o => o.TriggerType,
            (o, v) => o.TriggerType = v);

    internal static readonly DirectProperty<CollapseItem, CollapseExpandIconPosition> ExpandIconPositionProperty =
        AvaloniaProperty.RegisterDirect<CollapseItem, CollapseExpandIconPosition>(nameof(ExpandIconPosition),
            o => o.ExpandIconPosition,
            (o, v) => o.ExpandIconPosition = v);
    
    internal static readonly StyledProperty<Thickness> HeaderBorderThicknessProperty =
        AvaloniaProperty.Register<CollapseItem, Thickness>(nameof(HeaderBorderThickness));

    internal static readonly StyledProperty<Thickness> ContentBorderThicknessProperty =
        AvaloniaProperty.Register<CollapseItem, Thickness>(nameof(ContentBorderThickness));

    internal static readonly StyledProperty<TimeSpan> MotionDurationProperty =
        MotionAwareControlProperty.MotionDurationProperty.AddOwner<CollapseItem>();

    internal static readonly StyledProperty<bool> IsMotionEnabledProperty =
        MotionAwareControlProperty.IsMotionEnabledProperty.AddOwner<CollapseItem>();
    
    internal CustomizableSizeType SizeType
    {
        get => GetValue(SizeTypeProperty);
        set => SetValue(SizeTypeProperty, value);
    }

    internal Thickness DefaultHeaderPadding
    {
        get => GetValue(DefaultHeaderPaddingProperty);
        set => SetValue(DefaultHeaderPaddingProperty, value);
    }

    internal Thickness DefaultContentPadding
    {
        get => GetValue(DefaultContentPaddingProperty);
        set => SetValue(DefaultContentPaddingProperty, value);
    }

    private Thickness? _ownerHeaderPadding;

    internal Thickness? OwnerHeaderPadding
    {
        get => _ownerHeaderPadding;
        set => SetAndRaise(OwnerHeaderPaddingProperty, ref _ownerHeaderPadding, value);
    }

    private Thickness? _ownerContentPadding;

    internal Thickness? OwnerContentPadding
    {
        get => _ownerContentPadding;
        set => SetAndRaise(OwnerContentPaddingProperty, ref _ownerContentPadding, value);
    }

    private Thickness _effectiveHeaderPadding;

    internal Thickness EffectiveHeaderPadding
    {
        get => _effectiveHeaderPadding;
        set => SetAndRaise(EffectiveHeaderPaddingProperty, ref _effectiveHeaderPadding, value);
    }

    private Thickness _effectiveContentPadding;

    internal Thickness EffectiveContentPadding
    {
        get => _effectiveContentPadding;
        set => SetAndRaise(EffectiveContentPaddingProperty, ref _effectiveContentPadding, value);
    }

    private bool _isGhostStyle;

    internal bool IsGhostStyle
    {
        get => _isGhostStyle;
        set => SetAndRaise(IsGhostStyleProperty, ref _isGhostStyle, value);
    }

    private bool _isBorderless;

    internal bool IsBorderless
    {
        get => _isBorderless;
        set => SetAndRaise(IsBorderlessProperty, ref _isBorderless, value);
    }

    private CollapseTriggerType _triggerType = CollapseTriggerType.Header;

    internal CollapseTriggerType TriggerType
    {
        get => _triggerType;
        set => SetAndRaise(TriggerTypeProperty, ref _triggerType, value);
    }

    private CollapseExpandIconPosition _expandIconPosition = CollapseExpandIconPosition.Start;

    internal CollapseExpandIconPosition ExpandIconPosition
    {
        get => _expandIconPosition;
        set => SetAndRaise(ExpandIconPositionProperty, ref _expandIconPosition, value);
    }
    
    internal Thickness HeaderBorderThickness
    {
        get => GetValue(HeaderBorderThicknessProperty);
        set => SetValue(HeaderBorderThicknessProperty, value);
    }

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

    internal bool IsMotionEnabled
    {
        get => GetValue(IsMotionEnabledProperty);
        set => SetValue(IsMotionEnabledProperty, value);
    }
    
    #endregion

    private static readonly CubicEaseOut DefaultExpandMotionEasing = new();
    private static readonly CubicEaseIn DefaultCollapseMotionEasing = new();

    private BaseMotionActor? _motionActor;
    private Control? _headerDecorator;
    private IconButton? _expandButton;
    private CancellationTokenSource? _contentMotionCancellation;

    internal event EventHandler? MotionStateChanged;

    internal bool InAnimating { get; private set; }

    static CollapseItem()
    {
        SelectableMixin.Attach<CollapseItem>(IsSelectedProperty);
        PressedMixin.Attach<CollapseItem>();
        FocusableProperty.OverrideDefaultValue(typeof(CollapseItem), true);
        DataContextProperty.Changed.AddClassHandler<CollapseItem>((x, e) => x.UpdateHeader(e));
        AffectsRender<CollapseItem>(HeaderBorderThicknessProperty, ContentBorderThicknessProperty);
    }

    protected override AutomationPeer OnCreateAutomationPeer()
    {
        return new ListItemAutomationPeer(this);
    }

    protected override void OnInitialized()
    {
        base.OnInitialized();
        this.DisableTransitions();
    }

    protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
    {
        base.OnApplyTemplate(e);

        if (_expandButton is not null)
        {
            _expandButton.Click -= HandleExpandButtonClick;
        }
        CancelContentMotionAndClearValues();

        _motionActor           = e.NameScope.Find<BaseMotionActor>("PART_ContentMotionActor");
        _headerDecorator       = e.NameScope.Find<Control>("PART_HeaderDecorator");
        _expandButton          = e.NameScope.Find<IconButton>("PART_ExpandButton");

        UpdateEffectivePaddings();
        HandleSelectedChanged(true);
        if (_expandButton is not null)
        {
            _expandButton.Click += HandleExpandButtonClick;
        }
    }

    protected override void OnAttachedToVisualTree(VisualTreeAttachmentEventArgs e)
    {
        base.OnAttachedToVisualTree(e);
        SetupDefaultExpandIcon();
    }

    protected override void OnLoaded(RoutedEventArgs e)
    {
        base.OnLoaded(e);
        Dispatcher.Post(this.EnableTransitions);
    }

    protected override void OnDetachedFromVisualTree(VisualTreeAttachmentEventArgs e)
    {
        base.OnDetachedFromVisualTree(e);
        CancelContentMotionAndClearValues();
    }

    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        base.OnPropertyChanged(change);

        if (this.IsAttachedToVisualTree())
        {
            if (change.Property == IsSelectedProperty)
            {
                HandleSelectedChanged();
            }
        }

        if (change.Property == HeaderPaddingProperty ||
            change.Property == ContentPaddingProperty ||
            change.Property == DefaultHeaderPaddingProperty ||
            change.Property == DefaultContentPaddingProperty ||
            change.Property == OwnerHeaderPaddingProperty ||
            change.Property == OwnerContentPaddingProperty)
        {
            UpdateEffectivePaddings();
        }
    }

    internal bool IsPointInHeaderBounds(Point position)
    {
        if (_headerDecorator is not null && TriggerType != CollapseTriggerType.Icon)
        {
            return _headerDecorator.Bounds.Contains(position);
        }

        return false;
    }

    private void UpdateHeader(AvaloniaPropertyChangedEventArgs obj)
    {
        if (Header == null)
        {
            if (obj.NewValue is IHeadered headered)
            {
                if (Header != headered.Header)
                {
                    SetCurrentValue(HeaderProperty, headered.Header);
                }
            }
            else if (obj.NewValue is IReadOnlyHeadered readOnlyHeadered)
            {
                if (Header != readOnlyHeadered.Header)
                {
                    SetCurrentValue(HeaderProperty, readOnlyHeadered.Header);
                }
            }
            else
            {
                if (!(obj.NewValue is Control))
                {
                    SetCurrentValue(HeaderProperty, obj.NewValue);
                }
            }
        }
        else
        {
            if (Header == obj.OldValue)
            {
                SetCurrentValue(HeaderProperty, obj.NewValue);
            }
        }
    }

    private void HandleExpandButtonClick(object? sender, RoutedEventArgs args)
    {
        if (SelectingItemsControl.ItemsControlFromItemContainer(this) is Collapse collapse)
        {
            args.Handled = collapse.UpdateSelectionFromEvent(this, args);
        }
        else
        {
            IsSelected = !IsSelected;
        }
    }

    private void SetupDefaultExpandIcon()
    {
        if (ExpandIcon is null)
        {
            ClearValue(ExpandIconProperty);
            SetValue(ExpandIconProperty, new RightOutlined(), BindingPriority.Template);
        }
        Debug.Assert(ExpandIcon != null);
    }

    private void UpdateEffectivePaddings()
    {
        EffectiveHeaderPadding = IsSet(HeaderPaddingProperty)
            ? HeaderPadding
            : OwnerHeaderPadding ?? DefaultHeaderPadding;
        EffectiveContentPadding = IsSet(ContentPaddingProperty)
            ? ContentPadding
            : OwnerContentPadding ?? DefaultContentPadding;
    }

    private void HandleSelectedChanged(bool forceDisabledMotion = false)
    {
        if (Presenter is not null)
        {
            if (IsSelected)
            {
                ExpandItemContent(forceDisabledMotion);
            }
            else
            {
                CollapseItemContent(forceDisabledMotion);
            }
        }
    }

    private void ExpandItemContent(bool forceDisabledMotion = false)
    {
        UpdateContentVisibility(isVisible: true, forceDisabledMotion);
    }

    private void CollapseItemContent(bool forceDisabledMotion = false)
    {
        UpdateContentVisibility(isVisible: false, forceDisabledMotion);
    }

    private void UpdateContentVisibility(bool isVisible, bool forceDisabledMotion)
    {
        var motionActor = _motionActor;
        if (motionActor is null)
        {
            return;
        }

        if (!IsMotionEnabled || forceDisabledMotion)
        {
            ApplyContentStableState(motionActor, isVisible);
            return;
        }

        if (!isVisible && !motionActor.IsVisible && !InAnimating)
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
            ? new ExpandMotion(Direction.Bottom, MotionDuration, DefaultExpandMotionEasing)
            : new CollapseMotion(Direction.Bottom, MotionDuration, DefaultCollapseMotionEasing);
        await motion.RunAsync(motionActor,
            targetVisible ? () => motionActor.SetCurrentValue(IsVisibleProperty, true) : null,
            cancellationToken);
    }

    private CancellationTokenSource BeginContentMotion()
    {
        CancelContentMotion();
        var cancellation = new CancellationTokenSource();
        _contentMotionCancellation = cancellation;
        SetInAnimating(true);
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
        SetInAnimating(false);

        if (!ReferenceEquals(_motionActor, motionActor) || cancellation.IsCancellationRequested)
        {
            cancellation.Dispose();
            return;
        }

        cancellation.Dispose();
        if (IsSelected == targetVisible)
        {
            ApplyContentStableState(motionActor, targetVisible);
        }
        else
        {
            UpdateContentVisibility(IsSelected, forceDisabledMotion: false);
        }
    }

    private void CancelContentMotion()
    {
        var cancellation = _contentMotionCancellation;
        if (cancellation is not null)
        {
            _contentMotionCancellation = null;
            cancellation.Cancel();
            SetInAnimating(false);
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

    private void SetInAnimating(bool value)
    {
        if (InAnimating == value)
        {
            return;
        }

        InAnimating = value;
        MotionStateChanged?.Invoke(this, EventArgs.Empty);
    }
}
