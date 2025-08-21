using System.Globalization;
using AtomUI.Controls.Themes;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Converters;
using Avalonia.Controls.Diagnostics;
using Avalonia.Controls.Metadata;
using Avalonia.Controls.Presenters;
using Avalonia.Controls.Primitives;
using Avalonia.Input.Raw;
using Avalonia.Media;
using Colors = Avalonia.Media.Colors;
using GradientStop = Avalonia.Media.GradientStop;

namespace AtomUI.Controls;

[TemplatePart(TabScrollViewerThemeConstants.ScrollStartEdgeIndicatorPart, typeof(Control))]
[TemplatePart(TabScrollViewerThemeConstants.ScrollEndEdgeIndicatorPart, typeof(Control))]
[TemplatePart(TabScrollViewerThemeConstants.ScrollMenuIndicatorPart, typeof(IconButton))]
[TemplatePart(TabScrollViewerThemeConstants.ScrollViewContentPart, typeof(ScrollContentPresenter))]
internal abstract class BaseTabScrollViewer : ScrollViewer
{
    private const int EdgeIndicatorZIndex = 1000;

    #region 内部属性定义

    internal static readonly DirectProperty<BaseTabScrollViewer, Dock> TabStripPlacementProperty =
        AvaloniaProperty.RegisterDirect<BaseTabScrollViewer, Dock>(nameof(TabStripPlacement),
            o => o.TabStripPlacement,
            (o, v) => o.TabStripPlacement = v);

    internal static readonly DirectProperty<BaseTabScrollViewer, IBrush?> EdgeShadowStartColorProperty =
        AvaloniaProperty.RegisterDirect<BaseTabScrollViewer, IBrush?>(nameof(EdgeShadowStartColor),
            o => o.EdgeShadowStartColor,
            (o, v) => o.EdgeShadowStartColor = v);

    internal static readonly DirectProperty<BaseTabScrollViewer, double> MenuEdgeThicknessProperty =
        AvaloniaProperty.RegisterDirect<BaseTabScrollViewer, double>(nameof(MenuEdgeThickness),
            o => o.MenuEdgeThickness,
            (o, v) => o.MenuEdgeThickness = v);
    
    internal static readonly StyledProperty<bool> IsMotionEnabledProperty =
        MotionAwareControlProperty.IsMotionEnabledProperty.AddOwner<BaseTabScrollViewer>();

    private Dock _tabStripPlacement;

    internal Dock TabStripPlacement
    {
        get => _tabStripPlacement;
        set => SetAndRaise(TabStripPlacementProperty, ref _tabStripPlacement, value);
    }

    private IBrush? _edgeShadowStartColor;

    internal IBrush? EdgeShadowStartColor
    {
        get => _edgeShadowStartColor;
        set => SetAndRaise(EdgeShadowStartColorProperty, ref _edgeShadowStartColor, value);
    }

    private double _menuEdgeThickness;

    internal double MenuEdgeThickness
    {
        get => _menuEdgeThickness;
        set => SetAndRaise(MenuEdgeThicknessProperty, ref _menuEdgeThickness, value);
    }
    
    internal bool IsMotionEnabled
    {
        get => GetValue(IsMotionEnabledProperty);
        set => SetValue(IsMotionEnabledProperty, value);
    }
    
    #endregion

    private protected IconButton? _menuIndicator;
    private protected Border? _startEdgeIndicator;
    private protected Border? _endEdgeIndicator;
    private protected MenuFlyout? _menuFlyout;

    static BaseTabScrollViewer()
    {
        AffectsMeasure<BaseTabScrollViewer>(TabStripPlacementProperty);
    }

    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        base.OnPropertyChanged(change);
        if (change.Property == VerticalScrollBarVisibilityProperty ||
            change.Property == OffsetProperty ||
            change.Property == ExtentProperty ||
            change.Property == ViewportProperty)
        {
            SetupIndicatorsVisibility();
        }
    }

    private void SetupEdgeIndicatorSize()
    {
        if (Presenter is not null)
        {
            if (TabStripPlacement == Dock.Top || TabStripPlacement == Dock.Bottom)
            {
                if (_startEdgeIndicator is not null)
                {
                    _startEdgeIndicator.Height     = Presenter.DesiredSize.Height;
                    _startEdgeIndicator.Width      = _menuEdgeThickness;
                    _startEdgeIndicator.ZIndex     = EdgeIndicatorZIndex;
                    _startEdgeIndicator.Background = BuildEdgeIndicatorBrush(TabStripPlacement, true);
                }

                if (_endEdgeIndicator is not null)
                {
                    _endEdgeIndicator.Height     = Presenter.DesiredSize.Height;
                    _endEdgeIndicator.Width      = _menuEdgeThickness;
                    _endEdgeIndicator.Margin     = new Thickness(0, 0, _menuEdgeThickness, 0);
                    _endEdgeIndicator.ZIndex     = EdgeIndicatorZIndex;
                    _endEdgeIndicator.Background = BuildEdgeIndicatorBrush(TabStripPlacement, false);
                }
            }
            else
            {
                if (_startEdgeIndicator is not null)
                {
                    _startEdgeIndicator.Width      = Presenter.DesiredSize.Width;
                    _startEdgeIndicator.Height     = _menuEdgeThickness;
                    _startEdgeIndicator.ZIndex     = EdgeIndicatorZIndex;
                    _startEdgeIndicator.Background = BuildEdgeIndicatorBrush(TabStripPlacement, true);
                }

                if (_endEdgeIndicator is not null)
                {
                    _endEdgeIndicator.Width      = Presenter.DesiredSize.Width;
                    _endEdgeIndicator.Height     = _menuEdgeThickness;
                    _endEdgeIndicator.Margin     = new Thickness(0, 0, 0, _menuEdgeThickness);
                    _endEdgeIndicator.ZIndex     = EdgeIndicatorZIndex;
                    _endEdgeIndicator.Background = BuildEdgeIndicatorBrush(TabStripPlacement, false);
                }
            }
        }
    }

    private LinearGradientBrush BuildEdgeIndicatorBrush(Dock placement, bool isStart)
    {
        var linearGradientBrush = new LinearGradientBrush
        {
            GradientStops =
            {
                new GradientStop((_edgeShadowStartColor as ISolidColorBrush)!.Color, 0),
                new GradientStop(Colors.Transparent, 1)
            }
        };
        if (placement == Dock.Top || placement == Dock.Bottom)
        {
            if (isStart)
            {
                linearGradientBrush.StartPoint = new RelativePoint(0, 0.5, RelativeUnit.Relative);
                linearGradientBrush.EndPoint   = new RelativePoint(1, 0.5, RelativeUnit.Relative);
            }
            else
            {
                linearGradientBrush.StartPoint = new RelativePoint(1, 0.5, RelativeUnit.Relative);
                linearGradientBrush.EndPoint   = new RelativePoint(0, 0.5, RelativeUnit.Relative);
            }
        }
        else
        {
            if (isStart)
            {
                linearGradientBrush.StartPoint = new RelativePoint(0.5, 0, RelativeUnit.Relative);
                linearGradientBrush.EndPoint   = new RelativePoint(0.5, 1, RelativeUnit.Relative);
            }
            else
            {
                linearGradientBrush.StartPoint = new RelativePoint(0.5, 1, RelativeUnit.Relative);
                linearGradientBrush.EndPoint   = new RelativePoint(0.5, 0, RelativeUnit.Relative);
            }
        }

        return linearGradientBrush;
    }

    protected override bool RegisterContentPresenter(ContentPresenter presenter)
    {
        if (presenter is TabScrollContentPresenter tabStripScrollContentPresenter)
        {
            tabStripScrollContentPresenter.TabStripPlacement = TabStripPlacement;
        }

        return base.RegisterContentPresenter(presenter);
    }

    protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
    {
        base.OnApplyTemplate(e);
        _menuIndicator      = e.NameScope.Find<IconButton>(TabScrollViewerThemeConstants.ScrollMenuIndicatorPart);
        _startEdgeIndicator = e.NameScope.Find<Border>(TabScrollViewerThemeConstants.ScrollStartEdgeIndicatorPart);
        _endEdgeIndicator   = e.NameScope.Find<Border>(TabScrollViewerThemeConstants.ScrollEndEdgeIndicatorPart);

        SetupIndicatorsVisibility();
    }

    protected override Size MeasureOverride(Size availableSize)
    {
        var size = base.MeasureOverride(availableSize);
        SetupEdgeIndicatorSize();
        return size;
    }

    private void SetupIndicatorsVisibility()
    {
        var args = new List<object?>();

        object? scrollUpVisibility   = default;
        object? scrollDownVisibility = default;
        if (TabStripPlacement == Dock.Top || TabStripPlacement == Dock.Bottom)
        {
            args.Add(ScrollBarVisibility.Auto);
            args.Add(Offset.X);
            args.Add(Extent.Width);
            args.Add(Viewport.Width);
            scrollUpVisibility =
                MenuScrollingVisibilityConverter.Instance.Convert(args, typeof(bool), 0d, CultureInfo.CurrentCulture);
            scrollDownVisibility =
                MenuScrollingVisibilityConverter.Instance.Convert(args, typeof(bool), 100d, CultureInfo.CurrentCulture);
        }
        else
        {
            args.Add(ScrollBarVisibility.Auto);
            args.Add(Offset.Y);
            args.Add(Extent.Height);
            args.Add(Viewport.Height);
            scrollUpVisibility =
                MenuScrollingVisibilityConverter.Instance.Convert(args, typeof(bool), 0d, CultureInfo.CurrentCulture);
            scrollDownVisibility =
                MenuScrollingVisibilityConverter.Instance.Convert(args, typeof(bool), 100d, CultureInfo.CurrentCulture);
        }

        if (_startEdgeIndicator is not null &&
            scrollUpVisibility is not null &&
            scrollUpVisibility != AvaloniaProperty.UnsetValue)
        {
            _startEdgeIndicator.IsVisible = (bool)scrollUpVisibility;
        }

        if (_endEdgeIndicator is not null &&
            scrollDownVisibility is not null &&
            scrollDownVisibility != AvaloniaProperty.UnsetValue)
        {
            _endEdgeIndicator.IsVisible = (bool)scrollDownVisibility;
        }

        if (_menuIndicator is not null)
        {
            var startEdgeVisible = _startEdgeIndicator?.IsVisible ?? false;
            var endEdgeVisible   = _endEdgeIndicator?.IsVisible ?? false;
            _menuIndicator.IsVisible = startEdgeVisible || endEdgeVisible;
        }
    }
    
    protected bool ClickHideFlyoutPredicate(IPopupHostProvider hostProvider, RawPointerEventArgs args)
    {
        if (hostProvider.PopupHost != args.Root)
        {
            // 只有 TriggerType 为 Hover 的时候会判断
            var secondaryButtonOrigin = this.TranslatePoint(new Point(0, 0), TopLevel.GetTopLevel(_menuIndicator)!);
            var secondaryBounds = secondaryButtonOrigin.HasValue ? new Rect(secondaryButtonOrigin.Value, Bounds.Size) : new Rect();
            if (!secondaryBounds.Contains(args.Position))
            {
                return true;
            }
        }
        return false;
    }
}