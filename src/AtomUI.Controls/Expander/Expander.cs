using System.Diagnostics;
using System.Reactive.Disposables;
using AtomUI.Controls.Utils;
using AtomUI.IconPkg;
using AtomUI.IconPkg.AntDesign;
using AtomUI.MotionScene;
using AtomUI.Theme;
using AtomUI.Theme.Data;
using AtomUI.Theme.Styling;
using AtomUI.Theme.Utils;
using Avalonia;
using Avalonia.Animation.Easings;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Controls.Templates;
using Avalonia.Data;
using Avalonia.Input;
using Avalonia.LogicalTree;
using Avalonia.VisualTree;

namespace AtomUI.Controls;

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

public class Expander : AvaloniaExpander,
                        IAnimationAwareControl,
                        IControlSharedTokenResourcesHost,
                        ITokenResourceConsumer
{
    public const string ExpandedPC = ":expanded";
    public const string ExpandUpPC = ":up";
    public const string ExpandDownPC = ":down";
    public const string ExpandLeftPC = ":left";
    public const string ExpandRightPC = ":right";

    #region 公共属性定义

    public static readonly StyledProperty<SizeType> SizeTypeProperty =
        SizeTypeAwareControlProperty.SizeTypeProperty.AddOwner<Expander>();

    public static readonly StyledProperty<bool> IsShowExpandIconProperty =
        AvaloniaProperty.Register<Expander, bool>(nameof(IsShowExpandIcon), true);

    public static readonly StyledProperty<Icon?> ExpandIconProperty =
        AvaloniaProperty.Register<Expander, Icon?>(nameof(ExpandIcon));

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

    public static readonly StyledProperty<bool> IsMotionEnabledProperty
        = AnimationAwareControlProperty.IsMotionEnabledProperty.AddOwner<Expander>();

    public static readonly StyledProperty<bool> IsWaveAnimationEnabledProperty
        = AnimationAwareControlProperty.IsWaveAnimationEnabledProperty.AddOwner<Expander>();
    
    public SizeType SizeType
    {
        get => GetValue(SizeTypeProperty);
        set => SetValue(SizeTypeProperty, value);
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

    internal static readonly DirectProperty<Expander, Thickness> HeaderBorderThicknessProperty =
        AvaloniaProperty.RegisterDirect<Expander, Thickness>(nameof(HeaderBorderThickness),
            o => o.HeaderBorderThickness,
            (o, v) => o.HeaderBorderThickness = v);

    internal static readonly DirectProperty<Expander, TimeSpan> MotionDurationProperty =
        AvaloniaProperty.RegisterDirect<Expander, TimeSpan>(nameof(MotionDuration),
            o => o.MotionDuration,
            (o, v) => o.MotionDuration = v);

    internal static readonly DirectProperty<Expander, Thickness> EffectiveBorderThicknessProperty =
        AvaloniaProperty.RegisterDirect<Expander, Thickness>(nameof(EffectiveBorderThickness),
            o => o.EffectiveBorderThickness,
            (o, v) => o.EffectiveBorderThickness = v);

    private Thickness _headerBorderThickness;

    internal Thickness HeaderBorderThickness
    {
        get => _headerBorderThickness;
        set => SetAndRaise(HeaderBorderThicknessProperty, ref _headerBorderThickness, value);
    }

    private TimeSpan _motionDuration;

    internal TimeSpan MotionDuration
    {
        get => _motionDuration;
        set => SetAndRaise(MotionDurationProperty, ref _motionDuration, value);
    }

    private Thickness _effectiveBorderThickness;

    internal Thickness EffectiveBorderThickness
    {
        get => _effectiveBorderThickness;
        set => SetAndRaise(EffectiveBorderThicknessProperty, ref _effectiveBorderThickness, value);
    }
    
    Control IAnimationAwareControl.PropertyBindTarget => this;
    Control IControlSharedTokenResourcesHost.HostControl => this;
    string IControlSharedTokenResourcesHost.TokenId => ExpanderToken.ID;
    CompositeDisposable? ITokenResourceConsumer.TokenBindingsDisposable => _tokenBindingsDisposable;
    
    #endregion
    
    private CompositeDisposable? _tokenBindingsDisposable;

    public Expander()
    {
        this.RegisterResources();
        this.BindAnimationProperties(IsMotionEnabledProperty, IsWaveAnimationEnabledProperty);
    }

    private MotionActorControl? _motionActor;
    private Border? _headerDecorator;
    private IconButton? _expandButton;
    private bool _animating;
    private bool _tempAnimationDisabled = false;

    protected override void OnAttachedToLogicalTree(LogicalTreeAttachmentEventArgs e)
    {
        base.OnAttachedToLogicalTree(e);
        _tokenBindingsDisposable = new CompositeDisposable();
    }

    protected override void OnDetachedFromLogicalTree(LogicalTreeAttachmentEventArgs e)
    {
        base.OnDetachedFromLogicalTree(e);
        this.DisposeTokenBindings();
    }

    protected override void OnAttachedToVisualTree(VisualTreeAttachmentEventArgs e)
    {
        base.OnAttachedToVisualTree(e);
        this.AddTokenBindingDisposable(TokenResourceBinder.CreateTokenBinding(this, BorderThicknessProperty,
            SharedTokenKey.BorderThickness,
            BindingPriority.Template, new RenderScaleAwareThicknessConfigure(this)));
    }

    protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
    {
        SetupDefaultIcon();
        base.OnApplyTemplate(e);
        this.RunThemeTokenBindingActions();
        _motionActor     = e.NameScope.Find<MotionActorControl>(ExpanderTheme.ContentMotionActorPart);
        _headerDecorator = e.NameScope.Find<Border>(ExpanderTheme.HeaderDecoratorPart);
        _expandButton    = e.NameScope.Find<IconButton>(ExpanderTheme.ExpandButtonPart);
        SetupEffectiveBorderThickness();
        SetupExpanderBorderThickness();
        _tempAnimationDisabled = true;
        HandleExpandedChanged();
        _tempAnimationDisabled = false;
        if (_expandButton is not null)
        {
            _expandButton.Click += (sender, args) =>
            {
                if (_animating)
                {
                    return;
                }

                IsExpanded = !IsExpanded;
            };
        }
    }

    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        base.OnPropertyChanged(change);
        if (this.IsAttachedToVisualTree())
        {
            if (change.Property == AddOnContentProperty)
            {
                if (change.OldValue is Control oldControl)
                {
                    oldControl.SetTemplatedParent(null);
                }

                if (change.NewValue is Control newControl)
                {
                    newControl.SetTemplatedParent(this);
                }

                SetupDefaultIcon();
            }
        }

        if (change.Property == ExpandIconProperty)
        {
            if (change.OldValue is Icon oldIcon)
            {
                oldIcon.SetTemplatedParent(null);
            }

            if (change.NewValue is Icon newIcon)
            {
                newIcon.SetTemplatedParent(this);
            }
        }

        if (this.IsAttachedToVisualTree())
        {
            if (change.Property == IsBorderlessProperty)
            {
                SetupEffectiveBorderThickness();
            }
        }

        if (change.Property == IsExpandedProperty)
        {
            HandleExpandedChanged();
        }
        else if (change.Property == IsGhostStyleProperty ||
                 change.Property == IsBorderlessProperty ||
                 change.Property == IsExpandedProperty ||
                 change.Property == ExpandDirectionProperty)
        {
            SetupExpanderBorderThickness();
        }
    }

    private void SetupDefaultIcon()
    {
        if (ExpandIcon is null)
        {
            ClearValue(ExpandIconProperty);
            SetValue(ExpandIconProperty, AntDesignIconPackage.RightOutlined(), BindingPriority.Template);
        }
        Debug.Assert(ExpandIcon is not null);
        ExpandIcon.SetTemplatedParent(this);
    }

    private void SetupExpanderBorderThickness()
    {
        var headerBorderThickness = BorderThickness.Bottom;
        if (IsGhostStyle || IsBorderless)
        {
            headerBorderThickness = 0d;
        }

        if (ExpandDirection == ExpandDirection.Down || ExpandDirection == ExpandDirection.Left)
        {
            HeaderBorderThickness = new Thickness(0, 0, 0, headerBorderThickness);
        }
        else if (ExpandDirection == ExpandDirection.Up || ExpandDirection == ExpandDirection.Right)
        {
            HeaderBorderThickness = new Thickness(0, headerBorderThickness, 0, 0);
        }
    }
    
    protected override void OnPointerPressed(PointerPressedEventArgs e)
    {
        base.OnPointerPressed(e);
        if (e.GetCurrentPoint(this).Properties.IsLeftButtonPressed && e.Pointer.Type == PointerType.Mouse)
        {
            var position = e.GetPosition(_headerDecorator);
            if (_headerDecorator is not null)
            {
                var targetRect = new Rect(_headerDecorator.Bounds.Size);
                if (targetRect.Contains(position))
                {
                    if (_animating)
                    {
                        return;
                    }

                    IsExpanded = !IsExpanded;
                }
            }
        }
    }

    private void HandleExpandedChanged()
    {
        if (IsExpanded)
        {
            ExpandItemContent();
        }
        else
        {
            CollapseItemContent();
        }
    }

    private void ExpandItemContent()
    {
        if (_motionActor is null || _animating)
        {
            return;
        }

        if (!IsMotionEnabled || _tempAnimationDisabled)
        {
            _motionActor.IsVisible = true;
            return;
        }

        _animating = true;
        var motion = new ExpandMotion(DirectionFromExpandDirection(ExpandDirection),
            MotionDuration,
            new CubicEaseOut());
        MotionInvoker.Invoke(_motionActor, motion, () => { _motionActor.SetCurrentValue(IsVisibleProperty, true); },
            () => { _animating = false; });
    }

    private void CollapseItemContent()
    {
        if (_motionActor is null || _animating)
        {
            return;
        }

        if (!IsMotionEnabled || _tempAnimationDisabled)
        {
            _motionActor.IsVisible = false;
            return;
        }

        _animating = true;
        var motion = new CollapseMotion(DirectionFromExpandDirection(ExpandDirection),
            MotionDuration,
            new CubicEaseIn());
        MotionInvoker.Invoke(_motionActor, motion, null, () =>
        {
            _motionActor.SetCurrentValue(IsVisibleProperty, false);
            _animating = false;
        });
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
}