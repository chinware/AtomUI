using AtomUI.Controls.MotionScene;
using AtomUI.Controls.Utils;
using AtomUI.MotionScene;
using AtomUI.Theme.Data;
using AtomUI.Theme.Styling;
using Avalonia;
using Avalonia.Automation.Peers;
using Avalonia.Controls;
using Avalonia.Controls.Metadata;
using Avalonia.Controls.Mixins;
using Avalonia.Controls.Primitives;
using Avalonia.Controls.Templates;
using Avalonia.Layout;

namespace AtomUI.Controls;

[PseudoClasses(StdPseudoClass.Pressed, StdPseudoClass.Selected)]
public class CollapseItem : HeaderedContentControl, ISelectable
{
    private AnimationTargetPanel? _animationTarget;
    private bool _enableAnimation = true;
    private IconButton? _expandButton;
    private Border? _headerDecorator;

    static CollapseItem()
    {
        SelectableMixin.Attach<CollapseItem>(IsSelectedProperty);
        PressedMixin.Attach<CollapseItem>();
        FocusableProperty.OverrideDefaultValue(typeof(CollapseItem), true);
        DataContextProperty.Changed.AddClassHandler<CollapseItem>((x, e) => x.UpdateHeader(e));
    }

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
        _animationTarget = e.NameScope.Find<AnimationTargetPanel>(CollapseItemTheme.ContentAnimationTargetPart);
        _headerDecorator = e.NameScope.Find<Border>(CollapseItemTheme.HeaderDecoratorPart);
        _expandButton    = e.NameScope.Find<IconButton>(CollapseItemTheme.ExpandButtonPart);
        TokenResourceBinder.CreateTokenBinding(this, MotionDurationProperty, GlobalTokenResourceKey.MotionDurationSlow);
        SetupIconButton();
        _enableAnimation = false;
        HandleSelectedChanged();
        _enableAnimation = true;
        if (_expandButton is not null)
        {
            _expandButton.Click += (sender, args) => { IsSelected = !IsSelected; };
        }
    }

    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        base.OnPropertyChanged(change);
        if (change.Property == ExpandIconProperty)
        {
            SetupIconButton();
        }

        if (VisualRoot is not null)
        {
            if (change.Property == IsSelectedProperty)
            {
                HandleSelectedChanged();
            }
        }

        if (change.Property == AddOnContentProperty)
        {
            if (change.OldValue is Control oldControl)
            {
                UIStructureUtils.SetTemplateParent(oldControl, null);
            }

            if (change.NewValue is Control newControl)
            {
                UIStructureUtils.SetTemplateParent(newControl, this);
            }
        }
        else if (change.Property == ExpandIconProperty)
        {
            var oldExpandIcon = change.GetOldValue<PathIcon?>();
            if (oldExpandIcon is not null)
            {
                UIStructureUtils.SetTemplateParent(oldExpandIcon, null);
            }

            SetupIconButton();
        }
    }

    private void HandleSelectedChanged()
    {
        if (Presenter is not null)
        {
            if (IsSelected)
            {
                ExpandItemContent();
            }
            else
            {
                CollapseItemContent();
            }
        }
    }

    private void ExpandItemContent()
    {
        if (_animationTarget is null || InAnimating)
        {
            return;
        }

        if (!_enableAnimation)
        {
            _animationTarget.IsVisible = true;
            return;
        }

        _animationTarget.IsVisible = true;
        LayoutHelper.MeasureChild(_animationTarget, new Size(Bounds.Width, double.PositiveInfinity), new Thickness());
        InAnimating = true;
        var director = Director.Instance;
        var motion   = new ExpandMotion();
        motion.ConfigureOpacity(MotionDuration);
        motion.ConfigureHeight(MotionDuration);
        var motionActor = new MotionActor(_animationTarget, motion);
        motionActor.DispatchInSceneLayer = false;
        _animationTarget.InAnimation     = true;
        motionActor.Completed += (sender, args) =>
        {
            _animationTarget.InAnimation = false;
            InAnimating                  = false;
        };
        director?.Schedule(motionActor);
    }

    private void CollapseItemContent()
    {
        if (_animationTarget is null || InAnimating)
        {
            return;
        }

        if (!_enableAnimation)
        {
            _animationTarget.IsVisible = false;
            return;
        }

        InAnimating = true;

        LayoutHelper.MeasureChild(_animationTarget, new Size(Bounds.Width, double.PositiveInfinity), new Thickness());
        var director = Director.Instance;
        var motion   = new CollapseMotion();
        motion.ConfigureOpacity(MotionDuration);
        motion.ConfigureHeight(MotionDuration);
        var motionActor = new MotionActor(_animationTarget!, motion);
        motionActor.DispatchInSceneLayer = false;
        _animationTarget.InAnimation     = true;
        motionActor.Completed += (sender, args) =>
        {
            InAnimating                  = false;
            _animationTarget.InAnimation = false;
            _animationTarget.IsVisible   = false;
        };
        director?.Schedule(motionActor);
    }

    private void SetupIconButton()
    {
        if (ExpandIcon is null)
        {
            ExpandIcon = new PathIcon
            {
                Kind = "RightOutlined"
            };
            TokenResourceBinder.CreateGlobalTokenBinding(ExpandIcon, PathIcon.DisabledFilledBrushProperty,
                GlobalTokenResourceKey.ColorTextDisabled);
        }

        UIStructureUtils.SetTemplateParent(ExpandIcon, this);
    }

    internal bool IsPointInHeaderBounds(Point position)
    {
        if (_headerDecorator is not null && TriggerType != CollapseTriggerType.Icon)
        {
            return _headerDecorator.Bounds.Contains(position);
        }

        return false;
    }

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

    #endregion

    #region 内部属性定义

    internal static readonly DirectProperty<CollapseItem, SizeType> SizeTypeProperty =
        AvaloniaProperty.RegisterDirect<CollapseItem, SizeType>(nameof(SizeType),
            o => o.SizeType,
            (o, v) => o.SizeType = v);

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

    internal static readonly DirectProperty<CollapseItem, Thickness> HeaderBorderThicknessProperty =
        AvaloniaProperty.RegisterDirect<CollapseItem, Thickness>(nameof(HeaderBorderThickness),
            o => o.HeaderBorderThickness,
            (o, v) => o.HeaderBorderThickness = v);

    internal static readonly DirectProperty<CollapseItem, Thickness> ContentBorderThicknessProperty =
        AvaloniaProperty.RegisterDirect<CollapseItem, Thickness>(nameof(ContentBorderThickness),
            o => o.ContentBorderThickness,
            (o, v) => o.ContentBorderThickness = v);

    internal static readonly DirectProperty<CollapseItem, TimeSpan> MotionDurationProperty =
        AvaloniaProperty.RegisterDirect<CollapseItem, TimeSpan>(nameof(MotionDuration),
            o => o.MotionDuration,
            (o, v) => o.MotionDuration = v);

    private SizeType _sizeType;

    internal SizeType SizeType
    {
        get => _sizeType;
        set => SetAndRaise(SizeTypeProperty, ref _sizeType, value);
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

    private Thickness _headerBorderThickness;

    internal Thickness HeaderBorderThickness
    {
        get => _headerBorderThickness;
        set => SetAndRaise(HeaderBorderThicknessProperty, ref _headerBorderThickness, value);
    }

    private Thickness _contentBorderThickness;

    internal Thickness ContentBorderThickness
    {
        get => _contentBorderThickness;
        set => SetAndRaise(ContentBorderThicknessProperty, ref _contentBorderThickness, value);
    }

    private TimeSpan _motionDuration;

    internal TimeSpan MotionDuration
    {
        get => _motionDuration;
        set => SetAndRaise(MotionDurationProperty, ref _motionDuration, value);
    }

    #endregion
}