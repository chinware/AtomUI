﻿using System.Reactive.Disposables;
using AtomUI.Data;
using AtomUI.Theme;
using AtomUI.Theme.Data;
using AtomUI.Theme.Styling;
using AtomUI.Theme.Utils;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Controls.Templates;
using Avalonia.Data;
using Avalonia.LogicalTree;
using Avalonia.Media;
using Avalonia.VisualTree;

namespace AtomUI.Controls;

using AvaloniaTabControl = Avalonia.Controls.TabControl;

public class BaseTabControl : AvaloniaTabControl,
                              IAnimationAwareControl,
                              IControlSharedTokenResourcesHost,
                              ITokenResourceConsumer
{
    public const string TopPC = ":top";
    public const string RightPC = ":right";
    public const string BottomPC = ":bottom";
    public const string LeftPC = ":left";

    private static readonly FuncTemplate<Panel?> DefaultPanel =
        new(() => new StackPanel());

    #region 公共属性定义

    public static readonly StyledProperty<SizeType> SizeTypeProperty =
        SizeTypeAwareControlProperty.SizeTypeProperty.AddOwner<BaseTabControl>();

    public static readonly StyledProperty<bool> TabAlignmentCenterProperty =
        AvaloniaProperty.Register<BaseTabControl, bool>(nameof(TabAlignmentCenter));

    public static readonly StyledProperty<bool> IsMotionEnabledProperty
        = AnimationAwareControlProperty.IsMotionEnabledProperty.AddOwner<BaseTabControl>();

    public static readonly StyledProperty<bool> IsWaveAnimationEnabledProperty
        = AnimationAwareControlProperty.IsWaveAnimationEnabledProperty.AddOwner<BaseTabControl>();

    public SizeType SizeType
    {
        get => GetValue(SizeTypeProperty);
        set => SetValue(SizeTypeProperty, value);
    }

    public bool TabAlignmentCenter
    {
        get => GetValue(TabAlignmentCenterProperty);
        set => SetValue(TabAlignmentCenterProperty, value);
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

    #region 内部属性实现

    internal static readonly DirectProperty<BaseTabControl, double> TabAndContentGutterProperty =
        AvaloniaProperty.RegisterDirect<BaseTabControl, double>(nameof(TabAndContentGutter),
            o => o.TabAndContentGutter,
            (o, v) => o.TabAndContentGutter = v);

    private double _tabAndContentGutter;

    internal double TabAndContentGutter
    {
        get => _tabAndContentGutter;
        set => SetAndRaise(TabAndContentGutterProperty, ref _tabAndContentGutter, value);
    }

    internal static readonly DirectProperty<BaseTabControl, Thickness> TabStripMarginProperty =
        AvaloniaProperty.RegisterDirect<BaseTabControl, Thickness>(nameof(TabStripMargin),
            o => o.TabStripMargin,
            (o, v) => o.TabStripMargin = v);

    private Thickness _tabStripMargin;

    internal Thickness TabStripMargin
    {
        get => _tabStripMargin;
        set => SetAndRaise(TabStripMarginProperty, ref _tabStripMargin, value);
    }

    Control IAnimationAwareControl.PropertyBindTarget => this;
    Control IControlSharedTokenResourcesHost.HostControl => this;
    string IControlSharedTokenResourcesHost.TokenId => TabControlToken.ID;
    
    #endregion

    private Border? _frame;
    private Panel? _alignWrapper;
    private Point _tabStripBorderStartPoint;
    private Point _tabStripBorderEndPoint;
    private CompositeDisposable? _tokenBindingsDisposable;
    CompositeDisposable? ITokenResourceConsumer.TokenBindingsDisposable => _tokenBindingsDisposable;

    static BaseTabControl()
    {
        ItemsPanelProperty.OverrideDefaultValue<BaseTabControl>(DefaultPanel);
    }

    public BaseTabControl()
    {
        this.RegisterResources();
        this.BindAnimationProperties(IsMotionEnabledProperty, IsWaveAnimationEnabledProperty);
    }

    protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
    {
        base.OnApplyTemplate(e);
        _frame = e.NameScope.Find<Border>(BaseTabControlTheme.FramePart);
        _alignWrapper   = e.NameScope.Find<Panel>(BaseTabControlTheme.AlignWrapperPart);
    }
    
    protected override void PrepareContainerForItemOverride(Control container, object? item, int index)
    {
        base.PrepareContainerForItemOverride(container, item, index);
        if (container is TabItem tabItem)
        {
            BindUtils.RelayBind(this, SizeTypeProperty, tabItem, TabItem.SizeTypeProperty);
        }
    }
    
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
        if (_frame is not null)
        {
            this.AddTokenBindingDisposable(TokenResourceBinder.CreateTokenBinding(this, BorderThicknessProperty,
                SharedTokenKey.BorderThickness, BindingPriority.Template,
                new RenderScaleAwareThicknessConfigure(this)));
        }
        this.AddTokenBindingDisposable(TokenResourceBinder.CreateTokenBinding(this, TabAndContentGutterProperty,
            TabControlTokenKey.TabAndContentGutter));
        UpdatePseudoClasses();
        HandlePlacementChanged();
    }

    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        base.OnPropertyChanged(change);
        if (this.IsAttachedToVisualTree())
        {
            if (change.Property == TabStripPlacementProperty)
            {
                UpdatePseudoClasses();
                HandlePlacementChanged();
            }
        }
    }

    private void UpdatePseudoClasses()
    {
        PseudoClasses.Set(TopPC, TabStripPlacement == Dock.Top);
        PseudoClasses.Set(RightPC, TabStripPlacement == Dock.Right);
        PseudoClasses.Set(BottomPC, TabStripPlacement == Dock.Bottom);
        PseudoClasses.Set(LeftPC, TabStripPlacement == Dock.Left);
    }

    private void HandlePlacementChanged()
    {
        if (TabStripPlacement == Dock.Top)
        {
            TabStripMargin = new Thickness(0, 0, 0, _tabAndContentGutter);
        }
        else if (TabStripPlacement == Dock.Right)
        {
            TabStripMargin = new Thickness(_tabAndContentGutter, 0, 0, 0);
        }
        else if (TabStripPlacement == Dock.Bottom)
        {
            TabStripMargin = new Thickness(0, _tabAndContentGutter, 0, 0);
        }
        else
        {
            TabStripMargin = new Thickness(0, 0, _tabAndContentGutter, 0);
        }
    }

    private void SetupTabStripBorderPoints()
    {
        if (_alignWrapper is not null)
        {
            var offset          = _alignWrapper.TranslatePoint(new Point(0, 0), this) ?? default;
            var size            = _alignWrapper.Bounds.Size;
            var borderThickness = BorderThickness.Left;
            var offsetDelta     = borderThickness / 2;
            if (TabStripPlacement == Dock.Top)
            {
                _tabStripBorderStartPoint = new Point(0, size.Height - offsetDelta);
                _tabStripBorderEndPoint   = new Point(size.Width, size.Height - offsetDelta);
            }
            else if (TabStripPlacement == Dock.Right)
            {
                _tabStripBorderStartPoint = new Point(offsetDelta, 0);
                _tabStripBorderEndPoint   = new Point(offsetDelta, size.Height);
            }
            else if (TabStripPlacement == Dock.Bottom)
            {
                _tabStripBorderStartPoint = new Point(0, offsetDelta);
                _tabStripBorderEndPoint   = new Point(size.Width, offsetDelta);
            }
            else
            {
                _tabStripBorderStartPoint = new Point(size.Width - offsetDelta, 0);
                _tabStripBorderEndPoint   = new Point(size.Width - offsetDelta, size.Height);
            }

            _tabStripBorderStartPoint += offset;
            _tabStripBorderEndPoint   += offset;
        }
    }

    public override void Render(DrawingContext context)
    {
        SetupTabStripBorderPoints();
        var borderThickness = BorderThickness.Left;
        using var optionState = context.PushRenderOptions(new RenderOptions
        {
            EdgeMode = EdgeMode.Aliased
        });
        context.DrawLine(new Pen(BorderBrush, borderThickness), _tabStripBorderStartPoint, _tabStripBorderEndPoint);
    }
    
}