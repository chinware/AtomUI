using System.Globalization;
using AtomUI.Controls.Themes;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Converters;
using Avalonia.Controls.Metadata;
using Avalonia.Controls.Presenters;
using Avalonia.Controls.Primitives;

namespace AtomUI.Controls;

using AvaloniaScrollViewer = ScrollViewer;

[TemplatePart(MenuScrollViewerThemeConstants.ScrollDownButtonPart, typeof(IconButton))]
[TemplatePart(MenuScrollViewerThemeConstants.ScrollUpButtonPart, typeof(IconButton))]
[TemplatePart(MenuScrollViewerThemeConstants.ScrollViewContentPart, typeof(ScrollContentPresenter))]
internal class MenuScrollViewer : AvaloniaScrollViewer
{
    private IconButton? _scrollUpButton;
    private IconButton? _scrollDownButton;

    #region 内部属性定义
    
    public static readonly StyledProperty<bool> IsMotionEnabledProperty
        = MotionAwareControlProperty.IsMotionEnabledProperty.AddOwner<MenuScrollViewer>();

    public bool IsMotionEnabled
    {
        get => GetValue(IsMotionEnabledProperty);
        set => SetValue(IsMotionEnabledProperty, value);
    }
    
    #endregion
    
    protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
    {
        base.OnApplyTemplate(e);
        _scrollUpButton   = e.NameScope.Find<IconButton>(MenuScrollViewerThemeConstants.ScrollUpButtonPart);
        _scrollDownButton = e.NameScope.Find<IconButton>(MenuScrollViewerThemeConstants.ScrollDownButtonPart);

        SetupScrollButtonVisibility();
    }
    
    private void SetupScrollButtonVisibility()
    {
        var args = new List<object?>();
        args.Add(VerticalScrollBarVisibility);
        args.Add(Offset.Y);
        args.Add(Extent.Height);
        args.Add(Viewport.Height);
        var scrollUpVisibility =
            MenuScrollingVisibilityConverter.Instance.Convert(args, typeof(bool), 0d, CultureInfo.CurrentCulture);
        var scrollDownVisibility =
            MenuScrollingVisibilityConverter.Instance.Convert(args, typeof(bool), 100d, CultureInfo.CurrentCulture);
        if (_scrollUpButton is not null &&
            scrollUpVisibility is not null &&
            scrollUpVisibility != AvaloniaProperty.UnsetValue)
        {
            _scrollUpButton.IsVisible = (bool)scrollUpVisibility;
        }

        if (_scrollDownButton is not null &&
            scrollDownVisibility is not null &&
            scrollDownVisibility != AvaloniaProperty.UnsetValue)
        {
            _scrollDownButton.IsVisible = (bool)scrollDownVisibility;
        }
    }

    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        base.OnPropertyChanged(change);
        if (change.Property == VerticalScrollBarVisibilityProperty ||
            change.Property == OffsetProperty ||
            change.Property == ExtentProperty ||
            change.Property == ViewportProperty)
        {
            SetupScrollButtonVisibility();
        }
    }
}