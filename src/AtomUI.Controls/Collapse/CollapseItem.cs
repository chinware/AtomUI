using System.Diagnostics;
using AtomUI.Controls.Themes;
using AtomUI.Controls.Utils;
using AtomUI.IconPkg;
using AtomUI.IconPkg.AntDesign;
using AtomUI.MotionScene;
using AtomUI.Reflection;
using Avalonia;
using Avalonia.Animation;
using Avalonia.Animation.Easings;
using Avalonia.Automation.Peers;
using Avalonia.Controls;
using Avalonia.Controls.Metadata;
using Avalonia.Controls.Mixins;
using Avalonia.Controls.Primitives;
using Avalonia.Controls.Templates;
using Avalonia.Data;
using Avalonia.VisualTree;

namespace AtomUI.Controls;

[PseudoClasses(StdPseudoClass.Pressed, StdPseudoClass.Selected)]
public class CollapseItem : HeaderedContentControl,
                            ISelectable
{
    #region 公共属性定义

    public static readonly StyledProperty<bool> IsSelectedProperty =
        SelectingItemsControl.IsSelectedProperty.AddOwner<CollapseItem>();

    public static readonly StyledProperty<bool> IsShowExpandIconProperty =
        AvaloniaProperty.Register<CollapseItem, bool>(nameof(IsShowExpandIcon), true);

    public static readonly StyledProperty<Icon?> ExpandIconProperty =
        AvaloniaProperty.Register<CollapseItem, Icon?>(nameof(ExpandIcon));

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

    public Icon? ExpandIcon
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

    internal static readonly StyledProperty<SizeType> SizeTypeProperty =
        SizeTypeAwareControlProperty.SizeTypeProperty.AddOwner<CollapseItem>();

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

    internal static readonly StyledProperty<bool> IsMotionEnabledProperty
        = MotionAwareControlProperty.IsMotionEnabledProperty.AddOwner<CollapseItem>();

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
    }

    private bool _tempAnimationDisabled = false;
    private MotionActorControl? _motionActor;
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
        _motionActor           = e.NameScope.Find<MotionActorControl>(CollapseItemThemeConstants.ContentMotionActorPart);
        _headerDecorator       = e.NameScope.Find<Border>(CollapseItemThemeConstants.HeaderDecoratorPart);
        _expandButton          = e.NameScope.Find<IconButton>(CollapseItemThemeConstants.ExpandButtonPart);
        
        // 必须放在这里，因为依赖 _motionActor 是否设置
        _tempAnimationDisabled = true;
        HandleSelectedChanged();
        _tempAnimationDisabled = false;
        if (_expandButton is not null)
        {
            _expandButton.Click += (sender, args) => { IsSelected = !IsSelected; };
        }
        ConfigureTransitions();
    }

    protected override void OnAttachedToVisualTree(VisualTreeAttachmentEventArgs e)
    {
        base.OnAttachedToVisualTree(e);
        SetupDefaultExpandIcon();
    }

    private void SetupDefaultExpandIcon()
    {
        if (ExpandIcon is null)
        {
            ClearValue(ExpandIconProperty);
            SetValue(ExpandIconProperty, AntDesignIconPackage.RightOutlined(), BindingPriority.Template);
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
            else if (change.Property == IsMotionEnabledProperty)
            {
                ConfigureTransitions();
            }
        }

        if (change.Property == AddOnContentProperty ||
            change.Property == ExpandIconProperty)
        {
            if (change.OldValue is Control oldControl)
            {
                oldControl.SetTemplatedParent(null);
            }

            if (change.NewValue is Control newControl)
            {
                newControl.SetTemplatedParent(this);
            }
            if (change.Property == ExpandIconProperty)
            {
                SetupDefaultExpandIcon();
            }
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
        if (_motionActor is null || InAnimating)
        {
            return;
        }

        if (!IsMotionEnabled || _tempAnimationDisabled)
        {
            _motionActor.IsVisible = true;
            return;
        }

        InAnimating = true;
        var motion = new SlideUpInMotion(MotionDuration, new CubicEaseOut());
        MotionInvoker.Invoke(_motionActor, motion, () => { _motionActor.SetCurrentValue(IsVisibleProperty, true); },
            () => { InAnimating = false; });
    }

    private void CollapseItemContent()
    {
        if (_motionActor is null || InAnimating)
        {
            return;
        }

        if (!IsMotionEnabled || _tempAnimationDisabled)
        {
            _motionActor.IsVisible = false;
            return;
        }

        InAnimating = true;
        var motion = new SlideUpOutMotion(MotionDuration, new CubicEaseIn());
        MotionInvoker.Invoke(_motionActor, motion, null, () =>
        {
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

    private void ConfigureTransitions()
    {
        if (IsMotionEnabled)
        {
            if (_headerDecorator != null)
            {
                _headerDecorator.Transitions = new Transitions()
                {
                    TransitionUtils.CreateTransition<ThicknessTransition>(Border.BorderThicknessProperty)
                };
            }

            if (_expandButton != null)
            {
                _expandButton.Transitions = new Transitions()
                {
                    TransitionUtils.CreateTransition<TransformOperationsTransition>(Visual.RenderTransformProperty)
                };
            }
        }
        else
        {
            if (_headerDecorator != null)
            {
                _headerDecorator.Transitions?.Clear();
            }

            if (_expandButton != null)
            {
                _expandButton.Transitions?.Clear();
            }
        }
    }
}