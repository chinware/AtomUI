using System.Globalization;
using System.Reactive.Disposables;
using AtomUI.Theme;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Converters;
using Avalonia.Controls.Metadata;
using Avalonia.Controls.Presenters;
using Avalonia.Controls.Primitives;
using Avalonia.LogicalTree;

namespace AtomUI.Controls;

using AvaloniaScrollViewer = ScrollViewer;

[TemplatePart(MenuScrollViewerTheme.ScrollDownButtonPart, typeof(IconButton))]
[TemplatePart(MenuScrollViewerTheme.ScrollUpButtonPart, typeof(IconButton))]
[TemplatePart(MenuScrollViewerTheme.ScrollViewContentPart, typeof(ScrollContentPresenter))]
public class MenuScrollViewer : AvaloniaScrollViewer,
                                ITokenResourceConsumer
{
    private IconButton? _scrollUpButton;
    private IconButton? _scrollDownButton;

    #region 内部属性定义

    internal static readonly DirectProperty<MenuScrollViewer, bool> IsMotionEnabledProperty
        = AvaloniaProperty.RegisterDirect<MenuScrollViewer, bool>(nameof(IsMotionEnabled),
            o => o.IsMotionEnabled,
            (o, v) => o.IsMotionEnabled = v);

    private bool _isMotionEnabled;

    internal bool IsMotionEnabled
    {
        get => _isMotionEnabled;
        set => SetAndRaise(IsMotionEnabledProperty, ref _isMotionEnabled, value);
    }

    CompositeDisposable? ITokenResourceConsumer.TokenBindingsDisposable => _tokenBindingsDisposable;
    private CompositeDisposable? _tokenBindingsDisposable;
    
    #endregion
    
    protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
    {
        base.OnApplyTemplate(e);
        HandleTemplateApplied(e.NameScope);
    }

    private void HandleTemplateApplied(INameScope scope)
    {
        _scrollUpButton    = scope.Find<IconButton>(MenuScrollViewerTheme.ScrollUpButtonPart);
        _scrollDownButton  = scope.Find<IconButton>(MenuScrollViewerTheme.ScrollDownButtonPart);

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
}