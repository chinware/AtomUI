using System.Reactive.Disposables;
using AtomUI.Animations;
using AtomUI.Controls.Utils;
using AtomUI.Data;
using AtomUI.Theme;
using AtomUI.Theme.Data;
using AtomUI.Theme.Styling;
using AtomUI.Theme.Utils;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Converters;
using Avalonia.Controls.Diagnostics;
using Avalonia.Controls.Primitives;
using Avalonia.Controls.Primitives.PopupPositioning;
using Avalonia.Data;
using Avalonia.Input.Raw;
using Avalonia.Interactivity;
using Avalonia.LogicalTree;
using Avalonia.Media;
using Avalonia.VisualTree;

namespace AtomUI.Controls;

using AvaloniaButton = Avalonia.Controls.Button;
using AtomUIFlyout = AtomUI.Controls.Flyout;

public enum ColorPickerValueSyncMode
{
    Immediate,
    OnCompleted
}

public abstract class AbstractColorPicker : AvaloniaButton, 
                                            ISizeTypeAware,
                                            IMotionAwareControl,
                                            IControlSharedTokenResourcesHost,
                                            IResourceBindingManager
{
    #region 公共属性定义
    public static readonly StyledProperty<ColorFormat> FormatProperty =
        AbstractColorPickerView.FormatProperty.AddOwner<AbstractColorPicker>();

    public static readonly StyledProperty<FlyoutTriggerType> TriggerTypeProperty =
        FlyoutStateHelper.TriggerTypeProperty.AddOwner<AbstractColorPicker>();
    
    public static readonly StyledProperty<bool> IsShowArrowProperty =
        ArrowDecoratedBox.IsShowArrowProperty.AddOwner<AbstractColorPicker>();

    public static readonly StyledProperty<bool> IsPointAtCenterProperty =
        AtomUIFlyout.IsPointAtCenterProperty.AddOwner<AbstractColorPicker>();
    
    public static readonly StyledProperty<PlacementMode> PlacementProperty =
        Popup.PlacementProperty.AddOwner<AbstractColorPicker>();
    
    public static readonly StyledProperty<PopupGravity> PlacementGravityProperty =
        Popup.PlacementGravityProperty.AddOwner<AbstractColorPicker>();

    public static readonly StyledProperty<double> MarginToAnchorProperty =
        Popup.MarginToAnchorProperty.AddOwner<AbstractColorPicker>();

    public static readonly StyledProperty<int> MouseEnterDelayProperty =
        FlyoutStateHelper.MouseEnterDelayProperty.AddOwner<AbstractColorPicker>();

    public static readonly StyledProperty<int> MouseLeaveDelayProperty =
        FlyoutStateHelper.MouseLeaveDelayProperty.AddOwner<AbstractColorPicker>();
    
    public static readonly StyledProperty<bool> IsAlphaEnabledProperty =
        AbstractColorPickerView.IsAlphaEnabledProperty.AddOwner<AbstractColorPicker>();
    
    public static readonly StyledProperty<bool> IsFormatEnabledProperty =
        AbstractColorPickerView.IsFormatEnabledProperty.AddOwner<AbstractColorPicker>();
    
    public static readonly StyledProperty<bool> IsShowTextProperty =
        AvaloniaProperty.Register<AbstractColorPicker, bool>(nameof(IsShowText));
    
    public static readonly StyledProperty<bool> IsClearEnabledProperty =
        AbstractColorPickerView.IsClearEnabledProperty.AddOwner<AbstractColorPicker>();
    
    public static readonly StyledProperty<SizeType> SizeTypeProperty =
        SizeTypeAwareControlProperty.SizeTypeProperty.AddOwner<AbstractColorPicker>();
    
    public static readonly StyledProperty<bool> IsMotionEnabledProperty =
        MotionAwareControlProperty.IsMotionEnabledProperty.AddOwner<AbstractColorPicker>();
    
    public static readonly StyledProperty<string> EmptyColorTextProperty =
        AvaloniaProperty.Register<AbstractColorPicker, string>(nameof(EmptyColorText), string.Empty);

    public static readonly StyledProperty<List<ColorPickerPalette>?> PaletteGroupProperty =
        AbstractColorPickerView.PaletteGroupProperty.AddOwner<AbstractColorPicker>();

    public ColorFormat Format
    {
        get => GetValue(FormatProperty);
        set => SetValue(FormatProperty, value);
    }
    
    public FlyoutTriggerType TriggerType
    {
        get => GetValue(TriggerTypeProperty);
        set => SetValue(TriggerTypeProperty, value);
    }

    public bool IsShowArrow
    {
        get => GetValue(IsShowArrowProperty);
        set => SetValue(IsShowArrowProperty, value);
    }
    
    public bool IsPointAtCenter
    {
        get => GetValue(IsPointAtCenterProperty);
        set => SetValue(IsPointAtCenterProperty, value);
    }
    
    public PlacementMode Placement
    {
        get => GetValue(PlacementProperty);
        set => SetValue(PlacementProperty, value);
    }
    
    public PopupGravity PlacementGravity
    {
        get => GetValue(PlacementGravityProperty);
        set => SetValue(PlacementGravityProperty, value);
    }
    
    public double MarginToAnchor
    {
        get => GetValue(MarginToAnchorProperty);
        set => SetValue(MarginToAnchorProperty, value);
    }
    
    public int MouseEnterDelay
    {
        get => GetValue(MouseEnterDelayProperty);
        set => SetValue(MouseEnterDelayProperty, value);
    }

    public int MouseLeaveDelay
    {
        get => GetValue(MouseLeaveDelayProperty);
        set => SetValue(MouseLeaveDelayProperty, value);
    }
    
    public bool IsAlphaEnabled
    {
        get => GetValue(IsAlphaEnabledProperty);
        set => SetValue(IsAlphaEnabledProperty, value);
    }
    
    public bool IsFormatEnabled
    {
        get => GetValue(IsFormatEnabledProperty);
        set => SetValue(IsFormatEnabledProperty, value);
    }
    
    public bool IsShowText
    {
        get => GetValue(IsShowTextProperty);
        set => SetValue(IsShowTextProperty, value);
    }
    
    public bool IsClearEnabled
    {
        get => GetValue(IsClearEnabledProperty);
        set => SetValue(IsClearEnabledProperty, value);
    }
    
    public SizeType SizeType
    {
        get => GetValue(SizeTypeProperty);
        set => SetValue(SizeTypeProperty, value);
    }
    
    public bool IsMotionEnabled
    {
        get => GetValue(IsMotionEnabledProperty);
        set => SetValue(IsMotionEnabledProperty, value);
    }
    
    public string EmptyColorText
    {
        get => GetValue(EmptyColorTextProperty);
        set => SetValue(EmptyColorTextProperty, value);
    }
    
    public List<ColorPickerPalette>? PaletteGroup
    {
        get => GetValue(PaletteGroupProperty);
        set => SetValue(PaletteGroupProperty, value);
    }
    
    #endregion

    #region 内部属性定义
    
    internal static readonly DirectProperty<AbstractColorPicker, IBrush?> ColorBlockBackgroundProperty =
        AvaloniaProperty.RegisterDirect<AbstractColorPicker, IBrush?>(
            nameof(ColorBlockBackground),
            o => o.ColorBlockBackground,
            (o, v) => o.ColorBlockBackground = v);
    
    private IBrush? _colorBlockBackground;

    internal IBrush? ColorBlockBackground
    {
        get => _colorBlockBackground;
        set => SetAndRaise(ColorBlockBackgroundProperty, ref _colorBlockBackground, value);
    }

    Control IMotionAwareControl.PropertyBindTarget => this;
    Control IControlSharedTokenResourcesHost.HostControl => this;
    string IControlSharedTokenResourcesHost.TokenId => ColorPickerToken.ID;
    
    CompositeDisposable? IResourceBindingManager.ResourceBindingsDisposable { get; set; }
    
    #endregion
    
    private protected readonly FlyoutStateHelper FlyoutStateHelper;
    private protected Flyout? PickerFlyout;
    private protected bool IsFlyoutOpen;
    private CompositeDisposable? _flyoutBindingDisposables;
    private CompositeDisposable? _flyoutHelperBindingDisposables;

    static AbstractColorPicker()
    {
        AffectsMeasure<AbstractColorPicker>(IsShowTextProperty, 
            FormatProperty,
            ColorBlockBackgroundProperty);
    }
    
    public AbstractColorPicker()
    {
        this.RegisterResources();
        FlyoutStateHelper = new FlyoutStateHelper();
        FlyoutStateHelper.FlyoutAboutToShow        += HandleFlyoutAboutToShow;
        FlyoutStateHelper.FlyoutAboutToClose       += HandleFlyoutAboutToClose;
        FlyoutStateHelper.FlyoutOpened             += HandleFlyoutOpened;
        FlyoutStateHelper.FlyoutClosed             += HandleFlyoutClosed;
        FlyoutStateHelper.OpenFlyoutPredicate      =  FlyoutOpenPredicate;
        FlyoutStateHelper.ClickHideFlyoutPredicate =  ClickHideFlyoutPredicate;
    }

    protected override void OnAttachedToLogicalTree(LogicalTreeAttachmentEventArgs e)
    {
        base.OnAttachedToLogicalTree(e);
        this.AddResourceBindingDisposable(TokenResourceBinder.CreateTokenBinding(this, BorderThicknessProperty,
            SharedTokenKey.BorderThickness,
            BindingPriority.Template,
            new RenderScaleAwareThicknessConfigure(this)));
        _flyoutHelperBindingDisposables?.Dispose();
        _flyoutHelperBindingDisposables = new CompositeDisposable(3);
        _flyoutHelperBindingDisposables.Add(BindUtils.RelayBind(this, TriggerTypeProperty, FlyoutStateHelper,
            FlyoutStateHelper.TriggerTypeProperty));
        _flyoutHelperBindingDisposables.Add(BindUtils.RelayBind(this, MouseEnterDelayProperty, FlyoutStateHelper,
            FlyoutStateHelper.MouseEnterDelayProperty));
        _flyoutHelperBindingDisposables.Add(BindUtils.RelayBind(this, MouseLeaveDelayProperty, FlyoutStateHelper,
            FlyoutStateHelper.MouseLeaveDelayProperty));
    }

    protected override void OnDetachedFromLogicalTree(LogicalTreeAttachmentEventArgs e)
    {
        base.OnDetachedFromLogicalTree(e);
        this.DisposeTokenBindings();
        _flyoutHelperBindingDisposables?.Dispose();
    }

    protected override void OnAttachedToVisualTree(VisualTreeAttachmentEventArgs e)
    {
        base.OnAttachedToVisualTree(e);
        FlyoutStateHelper.NotifyAttachedToVisualTree();
    }

    protected override void OnDetachedFromVisualTree(VisualTreeAttachmentEventArgs e)
    {
        base.OnDetachedFromVisualTree(e);
        FlyoutStateHelper.NotifyDetachedFromVisualTree();
    }

    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        base.OnPropertyChanged(change);
        if (IsLoaded)
        {
            if (change.Property == IsMotionEnabledProperty)
            {
                ConfigureTransitions(true);
            }
        }

        if (change.Property == IsShowTextProperty || change.Property == FormatProperty)
        {
            GenerateValueText();
        }
    }

    protected abstract void GenerateValueText();
    protected abstract void GenerateColorBlockBackground();
    
    private void ConfigureTransitions(bool force)
    {
        if (IsMotionEnabled)
        {
            if (force || Transitions == null)
            {
                Transitions =
                [
                    TransitionUtils.CreateTransition<SolidColorBrushTransition>(BorderBrushProperty)
                ];
            }
        }
        else
        {
            Transitions = null;
        }
    }

    protected override void OnLoaded(RoutedEventArgs e)
    {
        base.OnLoaded(e);
        ConfigureTransitions(false);
    }

    protected override void OnUnloaded(RoutedEventArgs e)
    {
        base.OnUnloaded(e);
        Transitions = null;
    }

    protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
    {
        base.OnApplyTemplate(e);
        if (PickerFlyout is null)
        {
            PickerFlyout = CreatePickerFlyout();
            PickerFlyout.Opened += (sender, args) =>
            {
                IsFlyoutOpen = true;
                UpdatePseudoClasses();
            };
            PickerFlyout.Closed += (sender, args) =>
            {
                IsFlyoutOpen = false;
                UpdatePseudoClasses();
            };
            PickerFlyout.PresenterCreated += (sender, args) => { NotifyFlyoutPresenterCreated(args.Presenter); };
            FlyoutStateHelper.Flyout      =  PickerFlyout;
        }
        FlyoutStateHelper.AnchorTarget = this;
        SetupFlyoutProperties();
    }

    protected virtual void SetupFlyoutProperties()
    {
        if (PickerFlyout is not null)
        {
            _flyoutBindingDisposables?.Dispose();
            _flyoutBindingDisposables = new CompositeDisposable(5);
            _flyoutBindingDisposables.Add(BindUtils.RelayBind(this, PlacementProperty, PickerFlyout, PopupFlyoutBase.PlacementProperty));
            _flyoutBindingDisposables.Add(BindUtils.RelayBind(this, IsShowArrowProperty, PickerFlyout));
            _flyoutBindingDisposables.Add(BindUtils.RelayBind(this, IsPointAtCenterProperty, PickerFlyout));
            _flyoutBindingDisposables.Add(BindUtils.RelayBind(this, MarginToAnchorProperty, PickerFlyout));
            _flyoutBindingDisposables.Add(BindUtils.RelayBind(this, IsMotionEnabledProperty, PickerFlyout, AtomUIFlyout.IsMotionEnabledProperty));
        }
    }
    
    protected abstract Flyout CreatePickerFlyout();
    
    protected virtual void NotifyFlyoutPresenterCreated(Control flyoutPresenter)
    {
    }
    
    protected virtual void NotifyFlyoutClosed()
    {
    }
    
    private void HandleFlyoutClosed(object? sender, EventArgs args)
    {
        NotifyFlyoutClosed();
    }
    
    private void HandleFlyoutOpened(object? sender, EventArgs args)
    {
        NotifyFlyoutOpened();
    }

    protected virtual void NotifyFlyoutOpened()
    {
    }
    
    private void HandleFlyoutAboutToClose(object? sender, EventArgs args)
    {
        NotifyFlyoutAboutToClose();
    }

    protected virtual void NotifyFlyoutAboutToClose()
    {
    }
    
    private void HandleFlyoutAboutToShow(object? sender, EventArgs args)
    {
        NotifyFlyoutAboutToShow();
    }

    protected virtual void NotifyFlyoutAboutToShow()
    {
    }
    
    public void ClosePickerFlyout()
    {
        FlyoutStateHelper.HideFlyout(true);
    }
    
    protected virtual bool FlyoutOpenPredicate(Point position)
    {
        if (!IsEnabled)
        {
            return false;
        }
        var pos = this.TranslatePoint(new Point(0, 0), TopLevel.GetTopLevel(this)!);
        if (!pos.HasValue)
        {
            return false;
        }

        var region = new Rect(pos.Value, Bounds.Size);
        return region.Contains(position);
    }
    
    protected virtual bool ClickHideFlyoutPredicate(IPopupHostProvider hostProvider, RawPointerEventArgs args)
    {
        if (hostProvider.PopupHost != args.Root)
        {
            if (args.Root is PopupRoot root)
            {
                var parent = root.Parent;
                while (parent != null)
                {
                    if (parent == hostProvider.PopupHost)
                    {
                        return false;
                    }

                    if (parent is Control parentControl && parentControl.GetVisualRoot() == hostProvider.PopupHost)
                    {
                        return false;
                    }
                    parent = parent.Parent;
                }
            }
  
            return true;
        }
        return false;
    }
    
    protected virtual void UpdatePseudoClasses()
    {
        PseudoClasses.Set(StdPseudoClass.FlyoutOpen, IsFlyoutOpen);
    }

    protected string FormatColor(Color color)
    {
        if (Format == ColorFormat.Hex)
        {
            return ColorToHexConverter.ToHexString(color, AlphaComponentPosition.Leading, false, true);
        }
        if (Format == ColorFormat.Rgba)
        {
            return $"rgb({(int)color.R},{(int)color.G},{(int)color.B})";
        }
        return color.ToHsv().ToString();
    }
}