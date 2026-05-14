using System.Diagnostics;
using AtomUI.Animations;
using AtomUI.Controls;
using AtomUI.Controls.Primitives;
using AtomUI.Icons.AntDesign;
using AtomUI.Reflection;
using AtomUI.Theme;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Metadata;
using Avalonia.Controls.Primitives;
using Avalonia.Controls.Templates;
using Avalonia.Interactivity;
using Avalonia.Layout;
using Avalonia.Media;
using Avalonia.VisualTree;

namespace AtomUI.Desktop.Controls;

using AvaloniaButton = Avalonia.Controls.Button;

public enum ButtonType
{
    Default,
    Dashed,
    Primary,
    Link,
    Text
}

public enum ButtonShape
{
    Default,
    Circle,
    Round
}

[PseudoClasses(ButtonPseudoClass.IconOnly,
    ButtonPseudoClass.Loading,
    ButtonPseudoClass.IsDanger,
    ButtonPseudoClass.DefaultType,
    ButtonPseudoClass.DashedType,
    ButtonPseudoClass.PrimaryType,
    ButtonPseudoClass.LinkType,
    ButtonPseudoClass.TextType)]
public class Button : AvaloniaButton,
                      ISizeTypeAware,
                      IWaveSpiritAwareControl,
                      ICompactSpaceAware,
                      IFormItemAware
{
    #region 公共属性定义

    public static readonly StyledProperty<ButtonType> ButtonTypeProperty =
        AvaloniaProperty.Register<Button, ButtonType>(nameof(ButtonType));

    public static readonly StyledProperty<ButtonShape> ShapeProperty =
        AvaloniaProperty.Register<Button, ButtonShape>(nameof(Shape));

    public static readonly StyledProperty<bool> IsDangerProperty =
        AvaloniaProperty.Register<Button, bool>(nameof(IsDanger));

    public static readonly StyledProperty<bool> IsGhostProperty =
        AvaloniaProperty.Register<Button, bool>(nameof(IsGhost));

    public static readonly StyledProperty<bool> IsLoadingProperty =
        AvaloniaProperty.Register<Button, bool>(nameof(IsLoading));

    public static readonly StyledProperty<SizeType> SizeTypeProperty =
        SizeTypeControlProperty.SizeTypeProperty.AddOwner<Button>();

    public static readonly StyledProperty<PathIcon?> IconProperty =
        AvaloniaProperty.Register<Button, PathIcon?>(nameof(Icon));

    public static readonly StyledProperty<bool> IsMotionEnabledProperty =
        MotionAwareControlProperty.IsMotionEnabledProperty.AddOwner<Button>();

    public static readonly StyledProperty<bool> IsWaveSpiritEnabledProperty =
        WaveSpiritAwareControlProperty.IsWaveSpiritEnabledProperty.AddOwner<Button>();
    
    public ButtonType ButtonType
    {
        get => GetValue(ButtonTypeProperty);
        set => SetValue(ButtonTypeProperty, value);
    }

    public ButtonShape Shape
    {
        get => GetValue(ShapeProperty);
        set => SetValue(ShapeProperty, value);
    }

    public bool IsDanger
    {
        get => GetValue(IsDangerProperty);
        set => SetValue(IsDangerProperty, value);
    }

    public bool IsGhost
    {
        get => GetValue(IsGhostProperty);
        set => SetValue(IsGhostProperty, value);
    }

    public bool IsLoading
    {
        get => GetValue(IsLoadingProperty);
        set => SetValue(IsLoadingProperty, value);
    }

    public SizeType SizeType
    {
        get => GetValue(SizeTypeProperty);
        set => SetValue(SizeTypeProperty, value);
    }

    public PathIcon? Icon
    {
        get => GetValue(IconProperty);
        set => SetValue(IconProperty, value);
    }

    public bool IsMotionEnabled
    {
        get => GetValue(IsMotionEnabledProperty);
        set => SetValue(IsMotionEnabledProperty, value);
    }

    public bool IsWaveSpiritEnabled
    {
        get => GetValue(IsWaveSpiritEnabledProperty);
        set => SetValue(IsWaveSpiritEnabledProperty, value);
    }

    #endregion

    #region 内部属性定义

    internal static readonly StyledProperty<bool> IsIconVisibleProperty =
        AvaloniaProperty.Register<Button, bool>(nameof(IsIconVisible), true);

    internal static readonly StyledProperty<object?> RightExtraContentProperty =
        AvaloniaProperty.Register<Button, object?>(nameof(RightExtraContent));

    internal static readonly StyledProperty<IDataTemplate?> RightExtraContentTemplateProperty =
        AvaloniaProperty.Register<ContentControl, IDataTemplate?>(nameof(RightExtraContentTemplate));

    internal static readonly StyledProperty<Thickness> EffectiveBorderThicknessProperty =
        AvaloniaProperty.Register<Button, Thickness>(
            nameof(EffectiveBorderThickness));
    
    internal static readonly StyledProperty<WaveSpiritType> WaveSpiritTypeProperty =
        WaveSpiritAwareControlProperty.WaveSpiritTypeProperty.AddOwner<Button>();
    
    internal static readonly DirectProperty<Button, CornerRadius> EffectiveCornerRadiusProperty =
        AvaloniaProperty.RegisterDirect<Button, CornerRadius>(nameof(EffectiveCornerRadius),
            o => o.EffectiveCornerRadius,
            (o, v) => o.EffectiveCornerRadius = v);
    
    internal static readonly StyledProperty<SpaceItemPosition?> CompactSpaceItemPositionProperty = 
        CompactSpaceAwareControlProperty.CompactSpaceItemPositionProperty.AddOwner<Button>();
    
    internal static readonly StyledProperty<Orientation> CompactSpaceOrientationProperty = 
        CompactSpaceAwareControlProperty.CompactSpaceOrientationProperty.AddOwner<Button>();
    
    internal static readonly StyledProperty<bool> IsUsedInCompactSpaceProperty = 
        CompactSpaceAwareControlProperty.IsUsedInCompactSpaceProperty.AddOwner<Button>();
    
    internal bool IsIconVisible
    {
        get => GetValue(IsIconVisibleProperty);
        set => SetValue(IsIconVisibleProperty, value);
    }

    internal object? RightExtraContent
    {
        get => GetValue(RightExtraContentProperty);
        set => SetValue(RightExtraContentProperty, value);
    }

    internal IDataTemplate? RightExtraContentTemplate
    {
        get => GetValue(RightExtraContentTemplateProperty);
        set => SetValue(RightExtraContentTemplateProperty, value);
    }

    internal Thickness EffectiveBorderThickness
    {
        get => GetValue(EffectiveBorderThicknessProperty);
        set => SetValue(EffectiveBorderThicknessProperty, value);
    }
    
    internal WaveSpiritType WaveSpiritType
    {
        get => GetValue(WaveSpiritTypeProperty);
        set => SetValue(WaveSpiritTypeProperty, value);
    }
    
    private CornerRadius _effectiveCornerRadius;

    internal CornerRadius EffectiveCornerRadius
    {
        get => _effectiveCornerRadius;
        set => SetAndRaise(EffectiveCornerRadiusProperty, ref _effectiveCornerRadius, value);
    }
    
    internal SpaceItemPosition? CompactSpaceItemPosition
    {
        get => GetValue(CompactSpaceItemPositionProperty);
        set => SetValue(CompactSpaceItemPositionProperty, value);
    }
    
    internal Orientation CompactSpaceOrientation
    {
        get => GetValue(CompactSpaceOrientationProperty);
        set => SetValue(CompactSpaceOrientationProperty, value);
    }
    
    internal bool IsUsedInCompactSpace
    {
        get => GetValue(IsUsedInCompactSpaceProperty);
        set => SetValue(IsUsedInCompactSpaceProperty, value);
    }
    
    #endregion
    
    private const string LoadingIconName = "PART_LoadingIcon";
    private const string ButtonIconPresenterName = "PART_ButtonIcon";

    private Panel? _frameLayout;
    private WaveSpiritDecorator? _waveSpiritDecorator;
    private DockPanel? _contentLayout;
    private LoadingOutlined? _loadingIcon;
    private IconPresenter? _buttonIconPresenter;

    static Button()
    {
        AffectsMeasure<Button>(SizeTypeProperty,
            ShapeProperty,
            IconProperty,
            CompactSpaceItemPositionProperty,
            CompactSpaceOrientationProperty);
        AffectsRender<Button>(ButtonTypeProperty,
            IsDangerProperty,
            IsGhostProperty);
    }

    public Button()
    {
        this.RegisterTokenResourceScope(ButtonToken.ScopeProvider);
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

    protected override Size MeasureOverride(Size availableSize)
    {
        var size         = base.MeasureOverride(availableSize);
        var targetWidth  = size.Width;
        var targetHeight = size.Height;

        targetWidth = Math.Max(targetWidth, targetHeight);

        if (Shape == ButtonShape.Circle)
        {
            targetWidth  = targetHeight;
            CornerRadius = new CornerRadius(targetHeight);
        }
        else if (Shape == ButtonShape.Round)
        {
            CornerRadius = new CornerRadius(targetHeight);
            targetWidth  = Math.Max(targetWidth, targetHeight + targetHeight / 2);
        }

        return new Size(targetWidth, targetHeight);
    }
    
    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        base.OnPropertyChanged(change);
        if (change.Property == IsPressedProperty)
        {
            if (ShouldUseWaveSpirit() &&
                change.OldValue as bool? == true)
            {
                UpdateWaveSpiritDecorator();
                if (_waveSpiritDecorator is null)
                {
                    return;
                }
                
                IBrush? waveBrush = null;
                if (IsDanger)
                {
                    if (ButtonType == ButtonType.Primary && !IsGhost)
                    {
                        waveBrush = Background;
                    }
                    else
                    {
                        waveBrush = Foreground;
                    }
                }

                if (waveBrush != null)
                {
                    _waveSpiritDecorator.WaveBrush = waveBrush;
                }
     
                Dispatcher.Post(() =>
                {
                    _waveSpiritDecorator?.Play();
                });
            }
        }

        if (change.Property == ButtonTypeProperty ||
            change.Property == ShapeProperty)
        {
            ConfigureWaveSpiritType();
            UpdateWaveSpiritDecorator();
        }

        if (change.Property == ContentProperty ||
            change.Property == IconProperty ||
            change.Property == IsLoadingProperty ||
            change.Property == ButtonTypeProperty ||
            change.Property == IsDangerProperty)
        {
            UpdatePseudoClasses();
            if (change.Property == IsLoadingProperty)
            {
                UpdateLoadingIcon();
                UpdateWaveSpiritDecorator();
            }
        }
        if (change.Property == IconProperty ||
            change.Property == IsIconVisibleProperty ||
            change.Property == IsLoadingProperty)
        {
            UpdateButtonIconPresenter();
        }
        if (change.Property == IsWaveSpiritEnabledProperty)
        {
            UpdateWaveSpiritDecorator();
        }
        if (change.Property == BorderBrushProperty ||
            change.Property == ButtonTypeProperty ||
            change.Property == IsEnabledProperty ||
            change.Property == BorderThicknessProperty)
        {
            ConfigureEffectiveBorderThickness();
        }

        if (change.Property == CornerRadiusProperty ||
            change.Property == CompactSpaceItemPositionProperty ||
            change.Property == CompactSpaceOrientationProperty)
        {
            ConfigureEffectiveCornerRadius();
            SyncWaveSpiritDecorator();
        }
    }
    
    private void ConfigureWaveSpiritType()
    {
        WaveSpiritType waveType = default;
        if (Shape == ButtonShape.Default)
        {
            waveType = WaveSpiritType.RoundRectWave;
        }
        else if (Shape == ButtonShape.Round)
        {
            waveType = WaveSpiritType.PillWave;
        }
        else if (Shape == ButtonShape.Circle)
        {
            waveType = WaveSpiritType.CircleWave;
        }

        WaveSpiritType = waveType;
    }

    protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
    {
        DetachWaveSpiritDecorator();
        DetachLoadingIcon();
        DetachButtonIconPresenter();
        base.OnApplyTemplate(e);
        _frameLayout         = e.NameScope.Find<Panel>("PART_FrameLayout");
        _contentLayout       = e.NameScope.Find<DockPanel>("PART_ContentLayout");
        UpdatePseudoClasses();
        ConfigureWaveSpiritType();
        ConfigureEffectiveCornerRadius();
        UpdateWaveSpiritDecorator();
        UpdateLoadingIcon();
        UpdateButtonIconPresenter();
        ConfigureEffectiveBorderThickness();
    }

    private void ConfigureEffectiveBorderThickness()
    {
        if (ButtonType == ButtonType.Default ||
            ButtonType == ButtonType.Dashed ||
            ButtonType == ButtonType.Primary)
        {
            EffectiveBorderThickness = BorderThickness;
        }
        else
        {
            EffectiveBorderThickness = new Thickness(0);
        }
    }

    private void ConfigureEffectiveCornerRadius()
    {
        EffectiveCornerRadius = CompactSpace.CalculateEffectiveCornerRadius(
            CornerRadius, 
            IsUsedInCompactSpace, 
            CompactSpaceItemPosition,
            CompactSpaceOrientation);
    }

    private bool ShouldUseWaveSpirit()
    {
        return IsWaveSpiritEnabled &&
               !IsLoading &&
               (ButtonType == ButtonType.Primary ||
                ButtonType == ButtonType.Default ||
                ButtonType == ButtonType.Dashed);
    }

    private void UpdateWaveSpiritDecorator()
    {
        if (!ShouldUseWaveSpirit())
        {
            DetachWaveSpiritDecorator();
            return;
        }

        if (_frameLayout is null)
        {
            return;
        }

        if (_waveSpiritDecorator is null)
        {
            _waveSpiritDecorator = new WaveSpiritDecorator
            {
                Name = WaveSpiritDecorator.WaveSpiritPart
            };
            _waveSpiritDecorator.SetTemplatedParent(this);
            _frameLayout.Children.Insert(0, _waveSpiritDecorator);
        }

        SyncWaveSpiritDecorator();
    }

    private void SyncWaveSpiritDecorator()
    {
        if (_waveSpiritDecorator is null)
        {
            return;
        }

        _waveSpiritDecorator.SetCurrentValue(WaveSpiritDecorator.CornerRadiusProperty, EffectiveCornerRadius);
        _waveSpiritDecorator.SetCurrentValue(WaveSpiritDecorator.WaveTypeProperty, WaveSpiritType);
    }

    private void DetachWaveSpiritDecorator()
    {
        if (_waveSpiritDecorator is null)
        {
            return;
        }

        if (_waveSpiritDecorator.GetVisualParent() is Panel parent)
        {
            parent.Children.Remove(_waveSpiritDecorator);
        }
        else
        {
            _frameLayout?.Children.Remove(_waveSpiritDecorator);
        }

        _waveSpiritDecorator.SetTemplatedParent(null);
        _waveSpiritDecorator = null;
    }

    private void UpdatePseudoClasses()
    {
        PseudoClasses.Set(ButtonPseudoClass.IconOnly, Icon is not null && Content is null);
        PseudoClasses.Set(ButtonPseudoClass.Loading, IsLoading);
        PseudoClasses.Set(ButtonPseudoClass.DefaultType, ButtonType == ButtonType.Default);
        PseudoClasses.Set(ButtonPseudoClass.DashedType, ButtonType == ButtonType.Dashed);
        PseudoClasses.Set(ButtonPseudoClass.PrimaryType, ButtonType == ButtonType.Primary);
        PseudoClasses.Set(ButtonPseudoClass.LinkType, ButtonType == ButtonType.Link);
        PseudoClasses.Set(ButtonPseudoClass.TextType, ButtonType == ButtonType.Text);
        PseudoClasses.Set(ButtonPseudoClass.IsDanger, IsDanger);
    }

    private void UpdateLoadingIcon()
    {
        if (!IsLoading)
        {
            DetachLoadingIcon();
            return;
        }

        if (_contentLayout is null)
        {
            return;
        }

        if (_loadingIcon is not null)
        {
            return;
        }

        _loadingIcon = new LoadingOutlined
        {
            Name             = LoadingIconName,
            LoadingAnimation = IconAnimation.Spin
        };
        DockPanel.SetDock(_loadingIcon, Dock.Left);
        _loadingIcon.SetTemplatedParent(this);
        _contentLayout.Children.Insert(GetLoadingIconInsertIndex(), _loadingIcon);
    }

    private void DetachLoadingIcon()
    {
        if (_loadingIcon is null)
        {
            return;
        }

        if (_loadingIcon.GetVisualParent() is Panel parent)
        {
            parent.Children.Remove(_loadingIcon);
        }
        else
        {
            _contentLayout?.Children.Remove(_loadingIcon);
        }
        _loadingIcon.SetTemplatedParent(null);
        _loadingIcon = null;
    }

    private bool ShouldShowButtonIcon() => Icon is not null && IsIconVisible && !IsLoading;

    private void UpdateButtonIconPresenter()
    {
        if (!ShouldShowButtonIcon())
        {
            DetachButtonIconPresenter();
            return;
        }

        if (_contentLayout is null)
        {
            return;
        }

        if (_buttonIconPresenter is null)
        {
            _buttonIconPresenter = new IconPresenter
            {
                Name = ButtonIconPresenterName
            };
            DockPanel.SetDock(_buttonIconPresenter, Dock.Left);
            _buttonIconPresenter.SetTemplatedParent(this);
            _contentLayout.Children.Insert(GetButtonIconInsertIndex(), _buttonIconPresenter);
        }

        _buttonIconPresenter.SetCurrentValue(IconPresenter.IconProperty, Icon);
    }

    private void DetachButtonIconPresenter()
    {
        if (_buttonIconPresenter is null)
        {
            return;
        }

        if (_buttonIconPresenter.GetVisualParent() is Panel parent)
        {
            parent.Children.Remove(_buttonIconPresenter);
        }
        else
        {
            _contentLayout?.Children.Remove(_buttonIconPresenter);
        }

        _buttonIconPresenter.SetCurrentValue(IconPresenter.IconProperty, null);
        _buttonIconPresenter.SetTemplatedParent(null);
        _buttonIconPresenter = null;
    }

    private int GetLoadingIconInsertIndex()
    {
        Debug.Assert(_contentLayout is not null);
        for (var i = 0; i < _contentLayout.Children.Count; i++)
        {
            var child = _contentLayout.Children[i];
            if (child.Name == ButtonIconPresenterName ||
                child.Name == "PART_ContentPresenter")
            {
                return i;
            }
        }

        return _contentLayout.Children.Count;
    }

    private int GetButtonIconInsertIndex()
    {
        Debug.Assert(_contentLayout is not null);
        for (var i = 0; i < _contentLayout.Children.Count; i++)
        {
            if (_contentLayout.Children[i].Name == "PART_ContentPresenter")
            {
                return i;
            }
        }

        return _contentLayout.Children.Count;
    }

    void ICompactSpaceAware.NotifyPositionChange(SpaceItemPosition? position)
    {
        IsUsedInCompactSpace     = position != null;
        CompactSpaceItemPosition = position;
    }

    void ICompactSpaceAware.NotifyOrientationChange(Orientation orientation)
    {
        CompactSpaceOrientation = orientation;
    }
    
    bool ICompactSpaceAware.IsAlwaysActiveZIndex()
    {
        return ButtonType == ButtonType.Primary;
    }

    double ICompactSpaceAware.GetBorderThickness() => GetBorderThicknessForCompactSpace();

    protected virtual double GetBorderThicknessForCompactSpace()
    {
        if (!IsUsedInCompactSpace)
        {
            return 0.0;
        }

        return CompactSpaceOrientation == Orientation.Horizontal ? BorderThickness.Left : BorderThickness.Top;
    }
    
    #region 实现 FormItem 接口
    private EventHandler? _formValueChanged;
    event EventHandler? IFormItemAware.ValueChanged
    {
        add => _formValueChanged += value;
        remove => _formValueChanged -= value;
    }

    void IFormItemAware.SetFormValue(object? value) => NotifySetFormValue(value);

    object? IFormItemAware.GetFormValue() => NotifyGetFormValue();
    void IFormItemAware.ClearFormValue() => NotifyClearFormValue();
    void IFormItemAware.NotifyValidateStatus(FormValidateStatus status) => NotifyValidateStatus(status);
    
    protected virtual void NotifySetFormValue(object? value)
    {
    }

    protected virtual object? NotifyGetFormValue()
    {
        return null;
    }

    protected virtual void NotifyClearFormValue()
    {
    }

    protected virtual void NotifyValidateStatus(FormValidateStatus status)
    {
    }
    #endregion
}
