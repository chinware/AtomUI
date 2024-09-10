using System.Globalization;
using AtomUI.Theme.Styling;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Converters;
using Avalonia.Controls.Metadata;
using Avalonia.Controls.Presenters;
using Avalonia.Controls.Primitives;

namespace AtomUI.Controls;

using AvaloniaScrollViewer = ScrollViewer;

[TemplatePart(MenuScrollViewerTheme.ScrollDownButtonPart, typeof(IconButton))]
[TemplatePart(MenuScrollViewerTheme.ScrollUpButtonPart, typeof(IconButton))]
[TemplatePart(MenuScrollViewerTheme.ScrollViewContentPart, typeof(ScrollContentPresenter))]
public class MenuScrollViewer : AvaloniaScrollViewer, IControlCustomStyle
{
    private readonly IControlCustomStyle _customStyle;
    private IconButton? _scrollDownButton;
    private IconButton? _scrollUpButton;
    private ScrollContentPresenter? _scrollViewContent;

    public MenuScrollViewer()
    {
        _customStyle = this;
    }

    #region IControlCustomStyle 实现

    void IControlCustomStyle.HandleTemplateApplied(INameScope scope)
    {
        _scrollUpButton    = scope.Find<IconButton>(MenuScrollViewerTheme.ScrollUpButtonPart);
        _scrollDownButton  = scope.Find<IconButton>(MenuScrollViewerTheme.ScrollDownButtonPart);
        _scrollViewContent = scope.Find<ScrollContentPresenter>(MenuScrollViewerTheme.ScrollViewContentPart);

        SetupScrollButtonVisibility();
    }

    #endregion

    protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
    {
        base.OnApplyTemplate(e);
        _customStyle.HandleTemplateApplied(e.NameScope);
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