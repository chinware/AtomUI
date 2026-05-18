using System.Diagnostics;
using AtomUI.Animations;
using AtomUI.Controls.Primitives;
using AtomUI.Controls.Utils;
using AtomUI.Reflection;
using AtomUI.Utils;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Interactivity;
using Avalonia.LogicalTree;
using Avalonia.Media;
using Avalonia.VisualTree;

namespace AtomUI.Controls.Commons;

using AvaloniaRadioButton = Avalonia.Controls.RadioButton;

public abstract class AbstractOptionButton : AvaloniaRadioButton
{
    public static readonly StyledProperty<PathIcon?> IconProperty = AvaloniaProperty.Register<AbstractOptionButton, PathIcon?>(nameof (Icon));
    
    public PathIcon? Icon
    {
        get => GetValue(IconProperty);
        set => SetValue(IconProperty, value);
    }
    
    #region 内部属性定义

    internal static readonly StyledProperty<SizeType> SizeTypeProperty =
        SizeTypeControlProperty.SizeTypeProperty.AddOwner<AbstractOptionButton>();

    internal static readonly StyledProperty<OptionButtonStyle> ButtonStyleProperty =
        AvaloniaProperty.Register<AbstractOptionButton, OptionButtonStyle>(nameof(ButtonStyle));
    
    internal static readonly DirectProperty<AbstractOptionButton, OptionButtonPositionTrait> GroupPositionTraitProperty =
        AvaloniaProperty.RegisterDirect<AbstractOptionButton, OptionButtonPositionTrait>(
            nameof(GroupPositionTrait),
            o => o.GroupPositionTrait,
            (o, v) => o.GroupPositionTrait = v,
            OptionButtonPositionTrait.OnlyOne);

    internal static readonly StyledProperty<bool> IsMotionEnabledProperty =
        MotionAwareControlProperty.IsMotionEnabledProperty.AddOwner<AbstractOptionButton>();

    internal static readonly StyledProperty<bool> IsWaveSpiritEnabledProperty =
        WaveSpiritAwareControlProperty.IsWaveSpiritEnabledProperty.AddOwner<AbstractOptionButton>();

    internal SizeType SizeType
    {
        get => GetValue(SizeTypeProperty);
        set => SetValue(SizeTypeProperty, value);
    }

    internal OptionButtonStyle ButtonStyle
    {
        get => GetValue(ButtonStyleProperty);
        set => SetValue(ButtonStyleProperty, value);
    }
    
    internal bool IsMotionEnabled
    {
        get => GetValue(IsMotionEnabledProperty);
        set => SetValue(IsMotionEnabledProperty, value);
    }

    internal bool IsWaveSpiritEnabled
    {
        get => GetValue(IsWaveSpiritEnabledProperty);
        set => SetValue(IsWaveSpiritEnabledProperty, value);
    }

    private OptionButtonPositionTrait _groupPositionTrait = OptionButtonPositionTrait.OnlyOne;

    internal OptionButtonPositionTrait GroupPositionTrait
    {
        get => _groupPositionTrait;
        set => SetAndRaise(GroupPositionTraitProperty, ref _groupPositionTrait, value);
    }

    #endregion

    private const string RootLayoutName = "PART_RootLayout";
    private const string ContentLayoutName = "ContentLayout";
    private const string IconPresenterName = "IconPresenter";

    private CornerRadius? _originCornerRadius;
    private readonly BorderRenderHelper _borderRenderHelper;
    private Panel? _rootLayout;
    private DockPanel? _contentLayout;
    private IconPresenter? _iconPresenter;
    private WaveSpiritDecorator? _waveSpiritDecorator;

    static AbstractOptionButton()
    {
        AffectsMeasure<AbstractOptionButton>(SizeTypeProperty, ButtonStyleProperty);
        AffectsRender<AbstractOptionButton>(IsCheckedProperty, CornerRadiusProperty, ForegroundProperty, BackgroundProperty);
    }

    public AbstractOptionButton()
    {
        _borderRenderHelper = new BorderRenderHelper();
    }

    protected override Size MeasureOverride(Size availableSize)
    {
        var size         = base.MeasureOverride(availableSize);
        var targetWidth  = size.Width;
        var targetHeight = size.Height;
        targetHeight += Padding.Top + Padding.Bottom;
        targetWidth  += Padding.Left + Padding.Right;
        return new Size(targetWidth, targetHeight);
    }

    protected override void OnAttachedToLogicalTree(LogicalTreeAttachmentEventArgs e)
    {
        base.OnAttachedToLogicalTree(e);
        Debug.Assert(Parent is AbstractOptionButtonGroup, "AbstractOptionButton parent must be type of OptionButtonGroup");
    }

    protected override void OnAttachedToVisualTree(VisualTreeAttachmentEventArgs e)
    {
        base.OnAttachedToVisualTree(e);
        HandleSizeTypeChanged();
        UpdateIconPresenter();
    }

    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        base.OnPropertyChanged(change);
        if (change.Property == IsPointerOverProperty ||
            change.Property == IsPressedProperty ||
            change.Property == IsCheckedProperty)
        {
            if (change.Property == IsPressedProperty && change.OldValue as bool? == true && IsWaveSpiritEnabled)
            {
                UpdateWaveSpiritDecorator(createIfNeeded: true);
                Dispatcher.Post(PlayWaveSpiritDecorator);
            }
        }

        if (change.Property == GroupPositionTraitProperty)
        {
            if (_originCornerRadius.HasValue)
            {
                CornerRadius = BuildCornerRadius(GroupPositionTrait, _originCornerRadius!.Value);
                SyncWaveSpiritDecorator();
            }
        }

        if (change.Property == IconProperty)
        {
            UpdateIconPresenter();
        }

        if (change.Property == IsWaveSpiritEnabledProperty)
        {
            UpdateWaveSpiritDecorator(createIfNeeded: false);
        }

        if (change.Property == CornerRadiusProperty)
        {
            SyncWaveSpiritDecorator();
        }
    }

    private void HandleSizeTypeChanged()
    {
        _originCornerRadius = CornerRadius;
        CornerRadius        = BuildCornerRadius(GroupPositionTrait, _originCornerRadius!.Value);
    }

    private CornerRadius BuildCornerRadius(OptionButtonPositionTrait positionTrait, CornerRadius cornerRadius)
    {
        if (positionTrait == OptionButtonPositionTrait.First)
        {
            return new CornerRadius(cornerRadius.TopLeft,
                0,
                0,
                cornerRadius.BottomLeft);
        }

        if (positionTrait == OptionButtonPositionTrait.Last)
        {
            return new CornerRadius(0,
                cornerRadius.TopRight,
                cornerRadius.BottomRight,
                0);
        }

        if (positionTrait == OptionButtonPositionTrait.Middle)
        {
            return new CornerRadius(0);
        }

        return cornerRadius;
    }

    public override void Render(DrawingContext context)
    {
        _borderRenderHelper.Render(context,
            Bounds.Size,
            BorderUtils.BuildRenderScaleAwareThickness(BorderThickness, TopLevel.GetTopLevel(this)?.RenderScaling ?? 1.0),
            CornerRadius,
            BackgroundSizing.InnerBorderEdge,
            Background,
            BorderBrush);
    }

    protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
    {
        DetachIconPresenter();
        DetachWaveSpiritDecorator();
        base.OnApplyTemplate(e);
        _rootLayout   = e.NameScope.Find<Panel>(RootLayoutName);
        _contentLayout = e.NameScope.Find<DockPanel>(ContentLayoutName);
        UpdateIconPresenter();
    }

    protected override void OnDetachedFromVisualTree(VisualTreeAttachmentEventArgs e)
    {
        DetachIconPresenter();
        DetachWaveSpiritDecorator();
        base.OnDetachedFromVisualTree(e);
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

    private void UpdateIconPresenter()
    {
        if (Icon is null)
        {
            DetachIconPresenter();
            return;
        }

        if (_contentLayout is null)
        {
            return;
        }

        if (_iconPresenter is null)
        {
            _iconPresenter = new IconPresenter
            {
                Name            = IconPresenterName,
                IsMotionEnabled = false
            };
            DockPanel.SetDock(_iconPresenter, Dock.Left);
            _iconPresenter.SetTemplatedParent(this);
            _iconPresenter[!IconPresenter.IconBrushProperty] = this[!ForegroundProperty];
            _contentLayout.Children.Insert(0, _iconPresenter);
        }

        _iconPresenter.SetCurrentValue(IconPresenter.IconProperty, Icon);
    }

    private void DetachIconPresenter()
    {
        if (_iconPresenter is null)
        {
            return;
        }

        if (_iconPresenter.GetVisualParent() is Panel parent)
        {
            parent.Children.Remove(_iconPresenter);
        }
        else
        {
            _contentLayout?.Children.Remove(_iconPresenter);
        }

        _iconPresenter.ClearValue(IconPresenter.IconProperty);
        _iconPresenter.ClearValue(IconPresenter.IconBrushProperty);
        _iconPresenter.ClearValue(IconPresenter.IsMotionEnabledProperty);
        _iconPresenter.SetTemplatedParent(null);
        _iconPresenter = null;
    }

    private void UpdateWaveSpiritDecorator(bool createIfNeeded)
    {
        if (!IsWaveSpiritEnabled)
        {
            DetachWaveSpiritDecorator();
            return;
        }

        if (!createIfNeeded || _rootLayout is null)
        {
            return;
        }

        if (_waveSpiritDecorator is null)
        {
            _waveSpiritDecorator = new WaveSpiritDecorator
            {
                Name     = WaveSpiritDecorator.WaveSpiritPart,
                WaveType = WaveSpiritType.RoundRectWave
            };
            _waveSpiritDecorator.SetTemplatedParent(this);
            _rootLayout.Children.Insert(0, _waveSpiritDecorator);
        }

        SyncWaveSpiritDecorator();
    }

    private void SyncWaveSpiritDecorator()
    {
        if (_waveSpiritDecorator is null)
        {
            return;
        }

        _waveSpiritDecorator.SetCurrentValue(WaveSpiritDecorator.CornerRadiusProperty, CornerRadius);
        _waveSpiritDecorator.SetCurrentValue(WaveSpiritDecorator.WaveTypeProperty, WaveSpiritType.RoundRectWave);
    }

    private void PlayWaveSpiritDecorator()
    {
        _waveSpiritDecorator?.Play();
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
            _rootLayout?.Children.Remove(_waveSpiritDecorator);
        }

        _waveSpiritDecorator.SetTemplatedParent(null);
        _waveSpiritDecorator = null;
    }
}
