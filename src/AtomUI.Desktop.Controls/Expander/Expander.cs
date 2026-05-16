using System.Reactive.Disposables;
using AtomUI.Controls;
using AtomUI.Controls.Commons;
using AtomUI.Controls.Converters;
using AtomUI.Controls.Primitives;
using AtomUI.Data;
using AtomUI.Icons.AntDesign;
using AtomUI.MotionScene;
using AtomUI.Reflection;
using AtomUI.Theme;
using Avalonia;
using Avalonia.Animation.Easings;
using Avalonia.Controls;
using Avalonia.Controls.Metadata;
using Avalonia.Controls.Presenters;
using Avalonia.Controls.Primitives;
using Avalonia.Controls.Templates;
using Avalonia.Data;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Layout;
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

    public static readonly StyledProperty<SizeType> SizeTypeProperty =
        SizeTypeControlProperty.SizeTypeProperty.AddOwner<Expander>();

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

    internal static readonly DirectProperty<Expander, Thickness> HeaderBorderThicknessProperty =
        AvaloniaProperty.RegisterDirect<Expander, Thickness>(nameof(HeaderBorderThickness),
            o => o.HeaderBorderThickness,
            (o, v) => o.HeaderBorderThickness = v);
    
    internal static readonly StyledProperty<TimeSpan> MotionDurationProperty =
        MotionAwareControlProperty.MotionDurationProperty.AddOwner<Expander>();
    
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
    
    #endregion

    public Expander()
    {
        this.RegisterTokenResourceScope(ExpanderToken.ScopeProvider);
    }

    private static readonly StringToTextBlockConverter ContentTextConverter = new()
    {
        VerticalAlignment = VerticalAlignment.Center
    };

    private DockPanel? _mainLayout;
    private Grid? _headerLayout;
    private BaseMotionActor? _motionActor;
    private ContentPresenter? _contentPresenter;
    private Border? _headerDecorator;
    private IconButton? _expandButton;
    private ContentPresenter? _addOnContentPresenter;
    private PathIcon? _defaultExpandIcon;
    private CompositeDisposable? _contentBindings;
    private bool _animating;
    
    protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
    {
        DetachExpandButton();
        DetachAddOnContentPresenter();
        DetachContentMotionActor();
        base.OnApplyTemplate(e);

        _mainLayout       = e.NameScope.Find<DockPanel>("PART_MainLayout");
        _headerLayout     = e.NameScope.Find<Grid>("PART_HeaderLayout");
        _headerDecorator  = e.NameScope.Find<Border>("PART_HeaderDecorator");

        SetupEffectiveBorderThickness();
        SetupExpanderBorderThickness();
        UpdatePseudoClasses();
        UpdateExpandButton();
        UpdateAddOnContentPresenter();
        HandleExpandedChanged(forceDisabledMotion: true);
    }

    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        base.OnPropertyChanged(change);

        if (change.Property == IsShowExpandIconProperty)
        {
            UpdateExpandButton();
        }
        else if (change.Property == ExpandIconProperty && IsShowExpandIcon && _expandButton is not null && ExpandIcon is null)
        {
            SetupDefaultExpandIcon();
        }

        if (change.Property == ExpandIconProperty ||
            change.Property == IsMotionEnabledProperty ||
            change.Property == IsEnabledProperty ||
            change.Property == ExpandIconPositionProperty)
        {
            SyncExpandButtonProperties();
        }

        if (change.Property == AddOnContentProperty ||
            change.Property == AddOnContentTemplateProperty)
        {
            UpdateAddOnContentPresenter();
        }

        if (change.Property == IsExpandedProperty)
        {
            HandleExpandedChanged();
        }

        if (change.Property == BorderThicknessProperty ||
            change.Property == IsGhostStyleProperty ||
            change.Property == IsBorderlessProperty)
        {
            SetupEffectiveBorderThickness();
        }

        if (change.Property == BorderThicknessProperty ||
            change.Property == IsGhostStyleProperty ||
            change.Property == IsBorderlessProperty ||
            change.Property == IsExpandedProperty ||
            change.Property == ExpandDirectionProperty)
        {
            SetupExpanderBorderThickness();
        }

        if (change.Property == ContentPaddingProperty ||
            change.Property == HeaderPaddingProperty)
        {
            UpdatePseudoClasses();
        }

        if (change.Property == ContentPaddingProperty)
        {
            SyncContentPresenterPadding();
        }
    }

    private void HandleExpandButtonClick(object? sender, RoutedEventArgs args)
    {
        if (_animating || !IsEnabled)
        {
            return;
        }

        IsExpanded = !IsExpanded;
    }

    private void UpdateExpandButton()
    {
        if (!IsShowExpandIcon)
        {
            DetachExpandButton();
            return;
        }

        if (_headerLayout is null)
        {
            return;
        }

        SetupDefaultExpandIcon();
        if (_expandButton is null)
        {
            _expandButton = new IconButton
            {
                Name                = "PART_ExpandButton",
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment   = VerticalAlignment.Center
            };
            _expandButton.SetTemplatedParent(this);
            _expandButton.Click += HandleExpandButtonClick;
        }

        SyncExpandButtonProperties();
        if (!_headerLayout.Children.Contains(_expandButton))
        {
            _headerLayout.Children.Insert(0, _expandButton);
        }
    }

    private void SyncExpandButtonProperties()
    {
        if (_expandButton is null)
        {
            return;
        }

        _expandButton.SetValue(AbstractIconButton.IsMotionEnabledProperty, IsMotionEnabled, BindingPriority.Template);
        _expandButton.SetValue(AbstractIconButton.IconProperty, ExpandIcon, BindingPriority.Template);
        _expandButton.SetValue(IsEnabledProperty, IsEnabled, BindingPriority.Template);
        Grid.SetColumn(_expandButton, ExpandIconPosition == ExpanderIconPosition.Start ? 0 : 3);
    }

    private void DetachExpandButton()
    {
        if (_expandButton is null)
        {
            ReleaseDefaultExpandIconIfUnused();
            return;
        }

        _expandButton.Click -= HandleExpandButtonClick;
        if (_expandButton.GetVisualParent() is Panel parent)
        {
            parent.Children.Remove(_expandButton);
        }
        else
        {
            _headerLayout?.Children.Remove(_expandButton);
        }
        _expandButton.ClearValue(AbstractIconButton.IconProperty);
        _expandButton.ClearValue(AbstractIconButton.IsMotionEnabledProperty);
        _expandButton.ClearValue(IsEnabledProperty);
        _expandButton.SetTemplatedParent(null);
        _expandButton = null;
        ReleaseDefaultExpandIconIfUnused();
    }

    private void SetupDefaultExpandIcon()
    {
        if (ExpandIcon is not null)
        {
            return;
        }

        ClearValue(ExpandIconProperty);
        _defaultExpandIcon = new RightOutlined();
        SetValue(ExpandIconProperty, _defaultExpandIcon, BindingPriority.Template);
    }

    private void ReleaseDefaultExpandIconIfUnused()
    {
        if (_defaultExpandIcon is not null && ReferenceEquals(ExpandIcon, _defaultExpandIcon))
        {
            SetValue(ExpandIconProperty, null, BindingPriority.Template);
        }
        _defaultExpandIcon = null;
    }

    private bool HasAddOnContent()
    {
        return AddOnContent is not null || AddOnContentTemplate is not null;
    }

    private void UpdateAddOnContentPresenter()
    {
        if (!HasAddOnContent())
        {
            DetachAddOnContentPresenter();
            return;
        }

        if (_headerLayout is null)
        {
            return;
        }

        if (_addOnContentPresenter is null)
        {
            _addOnContentPresenter = new ContentPresenter
            {
                Name                       = "PART_AddOnContentPresenter",
                HorizontalAlignment        = HorizontalAlignment.Left,
                VerticalAlignment          = VerticalAlignment.Center,
                HorizontalContentAlignment = HorizontalAlignment.Left,
                VerticalContentAlignment   = VerticalAlignment.Center
            };
            Grid.SetColumn(_addOnContentPresenter, 2);
            _addOnContentPresenter.SetTemplatedParent(this);
        }

        SyncAddOnContentPresenter();
        if (!_headerLayout.Children.Contains(_addOnContentPresenter))
        {
            _headerLayout.Children.Add(_addOnContentPresenter);
        }
    }

    private void SyncAddOnContentPresenter()
    {
        if (_addOnContentPresenter is null)
        {
            return;
        }

        _addOnContentPresenter.SetValue(ContentPresenter.ContentProperty, AddOnContent, BindingPriority.Template);
        _addOnContentPresenter.SetValue(ContentPresenter.ContentTemplateProperty, AddOnContentTemplate, BindingPriority.Template);
    }

    private void DetachAddOnContentPresenter()
    {
        if (_addOnContentPresenter is null)
        {
            return;
        }

        if (_addOnContentPresenter.GetVisualParent() is Panel parent)
        {
            parent.Children.Remove(_addOnContentPresenter);
        }
        else
        {
            _headerLayout?.Children.Remove(_addOnContentPresenter);
        }
        _addOnContentPresenter.SetCurrentValue(ContentPresenter.ContentProperty, null);
        _addOnContentPresenter.SetCurrentValue(ContentPresenter.ContentTemplateProperty, null);
        _addOnContentPresenter.SetTemplatedParent(null);
        _addOnContentPresenter = null;
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
        if (!IsEnabled ||
            TriggerType != ExpanderTriggerType.Header ||
            !e.GetCurrentPoint(this).Properties.IsLeftButtonPressed ||
            e.Pointer.Type != PointerType.Mouse ||
            _headerDecorator is null ||
            IsPointerOverExpandButton(e.Source as Visual))
        {
            return;
        }

        var position   = e.GetPosition(_headerDecorator);
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

    private bool IsPointerOverExpandButton(Visual? source)
    {
        return _expandButton is not null &&
               source is not null &&
               (ReferenceEquals(source, _expandButton) ||
                source.GetVisualAncestors().Contains(_expandButton));
    }

    private void HandleExpandedChanged(bool forceDisabledMotion = false)
    {
        if (IsExpanded)
        {
            EnsureContentMotionActor(forceDisabledMotion || !IsMotionEnabled);
            ExpandItemContent(forceDisabledMotion);
        }
        else
        {
            CollapseItemContent(forceDisabledMotion);
        }
    }

    private void ExpandItemContent(bool forceDisabledMotion = false)
    {
        var motionActor = _motionActor;
        if (motionActor is null || _animating)
        {
            return;
        }

        if (!IsMotionEnabled || forceDisabledMotion)
        {
            motionActor.IsVisible = true;
            return;
        }

        _animating = true;
        var motion = new ExpandMotion(DirectionFromExpandDirection(ExpandDirection),
            MotionDuration,
            new CubicEaseOut());
        Dispatcher.InvokeAsync(async () =>
        {
            try
            {
                if (motionActor.GetVisualParent() is null)
                {
                    return;
                }
                await motion.RunAsync(motionActor, () => { motionActor.SetCurrentValue(IsVisibleProperty, true); });
            }
            finally
            {
                _animating = false;
            }
        });
    }

    private void CollapseItemContent(bool forceDisabledMotion = false)
    {
        var motionActor = _motionActor;
        if (motionActor is null || _animating)
        {
            return;
        }

        if (!IsMotionEnabled || forceDisabledMotion)
        {
            motionActor.IsVisible = false;
            return;
        }

        _animating = true;
        var motion = new CollapseMotion(DirectionFromExpandDirection(ExpandDirection),
            MotionDuration,
            new CubicEaseIn());
        Dispatcher.InvokeAsync(async () =>
        {
            try
            {
                if (motionActor.GetVisualParent() is null)
                {
                    return;
                }
                await motion.RunAsync(motionActor);
                motionActor.SetCurrentValue(IsVisibleProperty, false);
            }
            finally
            {
                _animating = false;
            }
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
    
    private void UpdatePseudoClasses()
    {
        PseudoClasses.Set(ExpanderPseudoClass.CustomHeaderPadding, HeaderPadding != null);
        PseudoClasses.Set(ExpanderPseudoClass.CustomContentPadding, ContentPadding != null);
    }

    private void EnsureContentMotionActor(bool initiallyVisible)
    {
        if (_motionActor is not null || _mainLayout is null)
        {
            return;
        }

        _contentPresenter = new ContentPresenter
        {
            Name = "PART_ContentPresenter"
        };
        _contentPresenter.SetTemplatedParent(this);
        _contentBindings = new CompositeDisposable
        {
            _contentPresenter.Bind(ContentPresenter.ContentProperty, new Binding
            {
                Source    = this,
                Path      = nameof(Content),
                Converter = ContentTextConverter,
                Priority  = BindingPriority.Template
            }),
            BindUtils.RelayBind(this, ContentTemplateProperty, _contentPresenter, ContentPresenter.ContentTemplateProperty, priority: BindingPriority.Template)
        };
        SyncContentPresenterPadding();

        _motionActor = new LayoutAwareMotionActor
        {
            Name         = "PART_ContentMotionActor",
            ClipToBounds = true,
            Content      = _contentPresenter,
            IsVisible    = initiallyVisible
        };
        _motionActor.SetTemplatedParent(this);
        _mainLayout.Children.Add(_motionActor);
    }

    private void DetachContentMotionActor()
    {
        _contentBindings?.Dispose();
        _contentBindings = null;

        if (_motionActor is not null)
        {
            if (_motionActor.GetVisualParent() is Panel parent)
            {
                parent.Children.Remove(_motionActor);
            }
            else
            {
                _mainLayout?.Children.Remove(_motionActor);
            }
            _motionActor.SetCurrentValue(ContentControl.ContentProperty, null);
            _motionActor.SetTemplatedParent(null);
            _motionActor = null;
        }

        if (_contentPresenter is not null)
        {
            _contentPresenter.SetCurrentValue(ContentPresenter.ContentProperty, null);
            _contentPresenter.SetCurrentValue(ContentPresenter.ContentTemplateProperty, null);
            _contentPresenter.SetCurrentValue(ContentPresenter.PaddingProperty, default(Thickness));
            _contentPresenter.SetTemplatedParent(null);
            _contentPresenter = null;
        }

        _animating = false;
    }

    private void SyncContentPresenterPadding()
    {
        if (_contentPresenter is null)
        {
            return;
        }

        if (ContentPadding is { } padding)
        {
            _contentPresenter.SetValue(ContentPresenter.PaddingProperty, padding);
        }
        else
        {
            _contentPresenter.ClearValue(ContentPresenter.PaddingProperty);
        }
    }

    protected override void OnDetachedFromVisualTree(VisualTreeAttachmentEventArgs e)
    {
        DetachExpandButton();
        DetachAddOnContentPresenter();
        DetachContentMotionActor();
        _headerDecorator = null;
        _headerLayout    = null;
        _mainLayout      = null;
        base.OnDetachedFromVisualTree(e);
    }
}
