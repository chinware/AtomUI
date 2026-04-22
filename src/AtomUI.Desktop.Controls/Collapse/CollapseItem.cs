using System.Diagnostics;
using AtomUI.Animations;
using AtomUI.Controls;
using AtomUI.Desktop.Controls.Themes;
using AtomUI.Icons.AntDesign;
using AtomUI.MotionScene;
using Avalonia;
using Avalonia.Animation.Easings;
using Avalonia.Automation.Peers;
using Avalonia.Controls;
using Avalonia.Controls.Metadata;
using Avalonia.Controls.Mixins;
using Avalonia.Threading;
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
    
    public static readonly StyledProperty<Thickness?> HeaderPaddingProperty =
        AvaloniaProperty.Register<CollapseItem, Thickness?>(nameof(HeaderPadding));
    
    public static readonly StyledProperty<Thickness?> ContentPaddingProperty =
        AvaloniaProperty.Register<CollapseItem, Thickness?>(nameof(ContentPadding));

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
    #endregion

    #region 内部属性定义

    internal static readonly StyledProperty<SizeType> SizeTypeProperty =
        SizeTypeControlProperty.SizeTypeProperty.AddOwner<CollapseItem>();

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
    
    internal SizeType SizeType
    {
        get => GetValue(SizeTypeProperty);
        set => SetValue(SizeTypeProperty, value);
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

    static CollapseItem()
    {
        SelectableMixin.Attach<CollapseItem>(IsSelectedProperty);
        PressedMixin.Attach<CollapseItem>();
        FocusableProperty.OverrideDefaultValue(typeof(CollapseItem), true);
        DataContextProperty.Changed.AddClassHandler<CollapseItem>((x, e) => x.UpdateHeader(e));
        AffectsRender<CollapseItem>(HeaderBorderThicknessProperty, ContentBorderThicknessProperty);
    }
    
    private BaseMotionActor? _motionActor;
    private Border? _headerDecorator;
    private IconButton? _expandButton;

    internal bool InAnimating { get; private set; }

    protected override AutomationPeer OnCreateAutomationPeer()
    {
        return new ListItemAutomationPeer(this);
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

    protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
    {
        base.OnApplyTemplate(e);

        if (_expandButton is not null)
        {
            _expandButton.Click -= HandleExpandButtonClick;
        }

        _motionActor           = e.NameScope.Find<BaseMotionActor>(CollapseItemThemeConstants.ContentMotionActorPart);
        _headerDecorator       = e.NameScope.Find<Border>(CollapseItemThemeConstants.HeaderDecoratorPart);
        _expandButton          = e.NameScope.Find<IconButton>(CollapseItemThemeConstants.ExpandButtonPart);

        HandleSelectedChanged(true);
        if (_expandButton is not null)
        {
            _expandButton.Click += HandleExpandButtonClick;
        }

        UpdatePseudoClasses();
    }

    private void HandleExpandButtonClick(object? sender, RoutedEventArgs args)
    {
        IsSelected = !IsSelected;
    }

    protected override void OnAttachedToVisualTree(VisualTreeAttachmentEventArgs e)
    {
        base.OnAttachedToVisualTree(e);
        SetupDefaultExpandIcon();
    }

    protected override void OnDetachedFromVisualTree(VisualTreeAttachmentEventArgs e)
    {
        base.OnDetachedFromVisualTree(e);
        InAnimating = false;
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

        if (change.Property == ContentPaddingProperty ||
            change.Property == HeaderPaddingProperty)
        {
            UpdatePseudoClasses();
        }
    }

    private void HandleSelectedChanged(bool forceDisabledMotion = false)
    {
        if (Presenter is not null)
        {
            if (IsSelected)
            {
                ExpandItemContentAsync(forceDisabledMotion);
            }
            else
            {
                CollapseItemContentAsync(forceDisabledMotion);
            }
        }
    }

    private void ExpandItemContentAsync(bool forceDisabledMotion = false)
    {
        if (_motionActor is null || InAnimating)
        {
            return;
        }

        if (!IsMotionEnabled || forceDisabledMotion)
        {
            _motionActor.IsVisible = true;
            return;
        }

        InAnimating = true;
        var motion = new SlideUpInMotion(MotionDuration, new CubicEaseOut());
        Dispatcher.UIThread.InvokeAsync(async () =>
        {
            await motion.RunAsync(_motionActor, () => { _motionActor.SetCurrentValue(IsVisibleProperty, true); });
            InAnimating = false;
        });
    }

    private void CollapseItemContentAsync(bool forceDisabledMotion = false)
    {
        if (_motionActor is null || InAnimating)
        {
            return;
        }

        if (!IsMotionEnabled || forceDisabledMotion)
        {
            _motionActor.IsVisible = false;
            return;
        }

        InAnimating = true;
        var motion = new SlideUpOutMotion(MotionDuration, new CubicEaseIn());
        Dispatcher.UIThread.InvokeAsync(async () =>
        {
            await motion.RunAsync(_motionActor);
            _motionActor.SetCurrentValue(IsVisibleProperty, false);
            InAnimating = false;
        });
    }

    internal bool IsPointInHeaderBounds(Point position)
    {
        if (_headerDecorator is not null && TriggerType != CollapseTriggerType.Icon)
        {
            return _headerDecorator.Bounds.Contains(position);
        }

        return false;
    }
    
    private void UpdatePseudoClasses()
    {
        PseudoClasses.Set(CollapseItemPseudoClass.CustomHeaderPadding, HeaderPadding != null);
        PseudoClasses.Set(CollapseItemPseudoClass.CustomContentPadding, ContentPadding != null);
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