using System.Globalization;
using AtomUI.Controls.Primitives.Themes;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Converters;
using Avalonia.Controls.Primitives;

namespace AtomUI.Controls.Primitives;

using AvaloniaScrollViewer = ScrollViewer;

internal class UpDownScrollerViewer : AvaloniaScrollViewer
{
    #region 内部属性定义
    
    public static readonly StyledProperty<bool> IsMotionEnabledProperty = 
        MotionAwareControlProperty.IsMotionEnabledProperty.AddOwner<UpDownScrollerViewer>();

    public bool IsMotionEnabled
    {
        get => GetValue(IsMotionEnabledProperty);
        set => SetValue(IsMotionEnabledProperty, value);
    }
    
    #endregion
    
    private IconButton? _scrollUpButton;
    private IconButton? _scrollDownButton;
    
    protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
    {
        base.OnApplyTemplate(e);
        _scrollUpButton   = e.NameScope.Find<IconButton>(UpDownScrollerViewerThemeConstants.ScrollUpButtonPart);
        _scrollDownButton = e.NameScope.Find<IconButton>(UpDownScrollerViewerThemeConstants.ScrollDownButtonPart);

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