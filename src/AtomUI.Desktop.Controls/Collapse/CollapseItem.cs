using System.Reactive.Disposables;
using AtomUI.Animations;
using AtomUI.Controls;
using AtomUI.Controls.Commons;
using AtomUI.Controls.Converters;
using AtomUI.Controls.Primitives;
using AtomUI.Data;
using AtomUI.Icons.AntDesign;
using AtomUI.MotionScene;
using AtomUI.Reflection;
using Avalonia;
using Avalonia.Animation.Easings;
using Avalonia.Automation.Peers;
using Avalonia.Controls;
using Avalonia.Controls.Metadata;
using Avalonia.Controls.Mixins;
using Avalonia.Controls.Presenters;
using Avalonia.Controls.Primitives;
using Avalonia.Controls.Templates;
using Avalonia.Data;
using Avalonia.Interactivity;
using Avalonia.Layout;
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
        DetachExpandButton();
        DetachAddOnContentPresenter();
        DetachContentMotionActor();
        base.OnApplyTemplate(e);

        _mainLayout            = e.NameScope.Find<DockPanel>("PART_MainLayout");
        _headerLayout          = e.NameScope.Find<Grid>("PART_HeaderLayout");
        _headerDecorator       = e.NameScope.Find<Border>("PART_HeaderDecorator");

        UpdateExpandButton();
        UpdateAddOnContentPresenter();
        HandleSelectedChanged(true);
    }

    private void HandleExpandButtonClick(object? sender, RoutedEventArgs args)
    {
        IsSelected = !IsSelected;
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
            Grid.SetColumn(_expandButton, 0);
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
        _expandButton.SetTemplatedParent(null);
        _expandButton = null;
        ReleaseDefaultExpandIconIfUnused();
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

    protected override void OnAttachedToVisualTree(VisualTreeAttachmentEventArgs e)
    {
        base.OnAttachedToVisualTree(e);
        if (IsShowExpandIcon)
        {
            SetupDefaultExpandIcon();
        }
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
            _defaultExpandIcon = new RightOutlined();
            SetValue(ExpandIconProperty, _defaultExpandIcon, BindingPriority.Template);
        }
    }

    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        base.OnPropertyChanged(change);

        if (change.Property == IsShowExpandIconProperty)
        {
            UpdateExpandButton();
        }
        else if (change.Property == ExpandIconProperty && IsShowExpandIcon && ExpandIcon is null)
        {
            SetupDefaultExpandIcon();
        }

        if (change.Property == ExpandIconProperty ||
            change.Property == IsMotionEnabledProperty ||
            change.Property == IsEnabledProperty)
        {
            SyncExpandButtonProperties();
        }

        if (change.Property == AddOnContentProperty ||
            change.Property == AddOnContentTemplateProperty)
        {
            UpdateAddOnContentPresenter();
        }

        if (this.IsAttachedToVisualTree())
        {
            if (change.Property == IsSelectedProperty)
            {
                HandleSelectedChanged();
            }
        }
    }

    private void HandleSelectedChanged(bool forceDisabledMotion = false)
    {
        if (Presenter is not null || _mainLayout is not null)
        {
            if (IsSelected)
            {
                EnsureContentMotionActor(forceDisabledMotion || !IsMotionEnabled);
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
        var motionActor = _motionActor;
        if (motionActor is null || InAnimating)
        {
            return;
        }

        if (!IsMotionEnabled || forceDisabledMotion)
        {
            motionActor.IsVisible = true;
            return;
        }

        InAnimating = true;
        var motion = new SlideUpInMotion(MotionDuration, new CubicEaseOut());
        Dispatcher.InvokeAsync(async () =>
        {
            await motion.RunAsync(motionActor, () => { motionActor.SetCurrentValue(IsVisibleProperty, true); });
            InAnimating = false;
        });
    }

    private void CollapseItemContent(bool forceDisabledMotion = false)
    {
        var motionActor = _motionActor;
        if (motionActor is null || InAnimating)
        {
            return;
        }

        if (!IsMotionEnabled || forceDisabledMotion)
        {
            motionActor.IsVisible = false;
            return;
        }

        InAnimating = true;
        var motion = new SlideUpOutMotion(MotionDuration, new CubicEaseIn());
        Dispatcher.InvokeAsync(async () =>
        {
            await motion.RunAsync(motionActor);
            motionActor.SetCurrentValue(IsVisibleProperty, false);
            InAnimating = false;
        });
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
            BindUtils.RelayBind(this, ContentTemplateProperty, _contentPresenter, ContentPresenter.ContentTemplateProperty, priority: BindingPriority.Template),
            BindUtils.RelayBind(this, ContentBorderThicknessProperty, _contentPresenter, ContentPresenter.BorderThicknessProperty, priority: BindingPriority.Template),
            BindUtils.RelayBind(this, ContentPaddingProperty, _contentPresenter, ContentPresenter.PaddingProperty, priority: BindingPriority.Template)
        };

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
            _contentPresenter.SetTemplatedParent(null);
            _contentPresenter = null;
        }

        InAnimating = false;
    }

    internal bool IsPointInHeaderBounds(Point position)
    {
        if (_headerDecorator is not null && TriggerType != CollapseTriggerType.Icon)
        {
            return _headerDecorator.Bounds.Contains(position);
        }

        return false;
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
