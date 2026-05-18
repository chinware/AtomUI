using System.Reactive.Disposables;
using AtomUI.Data;
using AtomUI.Reflection;
using AtomUI.Theme;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.VisualTree;

namespace AtomUI.Desktop.Controls;

public class SimplePagination : AbstractPagination
{
    #region 公共属性定义

    public static readonly StyledProperty<bool> IsReadOnlyProperty =
        AvaloniaProperty.Register<AbstractPagination, bool>(nameof(IsReadOnly), defaultValue:true);

    public bool IsReadOnly
    {
        get => GetValue(IsReadOnlyProperty);
        set => SetValue(IsReadOnlyProperty, value);
    }

    #endregion
    
    private PaginationNavItem? _previousPageItem;
    private PaginationNavItem? _nextPageItem;
    private TextBlock? _infoIndicator;
    private QuickJumpEdit? _quickJumper;
    private Panel? _rootLayout;
    private CompositeDisposable? _quickJumperBindings;

    static SimplePagination()
    {
        AffectsMeasure<SimplePagination>(IsReadOnlyProperty);
    }

    public SimplePagination()
    {
        this.RegisterTokenResourceScope(PaginationToken.ScopeProvider);
    }

    protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
    {
        ReleaseTemplateParts();
        base.OnApplyTemplate(e);
        _rootLayout       = e.NameScope.Find<Panel>("PART_RootLayout");
        _previousPageItem = e.NameScope.Find<PaginationNavItem>("PART_PreviousNavItem");
        _nextPageItem     = e.NameScope.Find<PaginationNavItem>("PART_NextNavItem");
        _infoIndicator    = e.NameScope.Find<TextBlock>("PART_InfoIndicator");

        if (_previousPageItem is not null)
        {
            _previousPageItem.Click += HandleNavItemClicked;
        }
        if (_nextPageItem is not null)
        {
            _nextPageItem.Click += HandleNavItemClicked;
        }

        TemplateConfigured = _rootLayout is not null &&
                             _previousPageItem is not null &&
                             _nextPageItem is not null &&
                             _infoIndicator is not null;
        HandlePageConditionChanged();
        ConfigureQuickJumper();
    }

    protected override void OnDetachedFromVisualTree(VisualTreeAttachmentEventArgs e)
    {
        ReleaseTemplateParts();
        base.OnDetachedFromVisualTree(e);
    }

    protected override void NotifyPageConditionChanged(int currentPage, int pageCount, int pageSize, long total)
    {
        if (TemplateConfigured)
        {
            if (IsReadOnly)
            {
                if (_infoIndicator != null)
                {
                    _infoIndicator.Text = $"{currentPage} / {pageCount}";
                }
            }
            else
            {
                if (_infoIndicator != null)
                {
                    _infoIndicator.Text = $" / {pageCount}";
                }

                if (_quickJumper != null)
                {
                    _quickJumper.Text = $"{currentPage}";
                }
            }
            if (_previousPageItem is not null && _nextPageItem is not null)
            {
                _previousPageItem.IsEnabled  = currentPage > 1;
                _previousPageItem.PageNumber = Math.Max(1, CurrentPage - 1);
                _nextPageItem.IsEnabled      = currentPage < pageCount;
                _nextPageItem.PageNumber     = Math.Min(pageCount, CurrentPage + 1);
            }
        }
        if (_quickJumper != null)
        {
            _quickJumper.Maximum = PageCount;
        }
        base.NotifyPageConditionChanged(currentPage, pageCount, pageSize, total);
    }

    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        base.OnPropertyChanged(change);
        if (change.Property == PageCountProperty)
        {
            if (_quickJumper != null)
            {
                _quickJumper.Maximum = PageCount;
            }
        }
        else if (change.Property == IsReadOnlyProperty)
        {
            ConfigureQuickJumper();
            HandlePageConditionChanged();
        }
    }

    private void HandleNavItemClicked(object? sender, RoutedEventArgs args)
    {
        if (sender is PaginationNavItem navItemSender)
        {
            CurrentPage = navItemSender.PageNumber;
        }
    }
    
    private void HandleLineEditKeyUp(object? sender, KeyEventArgs e)
    {
        if (sender is QuickJumpEdit lineEdit)
        {
            if (e.Key == Key.Enter)
            {
                if (int.TryParse(lineEdit.Text?.Trim(), out var pageNumber))
                {
                    var pageCount   = (int)Math.Ceiling(Total / (double)PageSize);
                    CurrentPage = Math.Max(1, Math.Min(pageNumber, pageCount));
                }
            }
        }
    }

    private void ConfigureQuickJumper()
    {
        if (IsReadOnly)
        {
            ReleaseQuickJumper();
            return;
        }

        EnsureQuickJumper();
    }

    private void EnsureQuickJumper()
    {
        if (_quickJumper is not null || _rootLayout is null)
        {
            return;
        }

        _quickJumper = new QuickJumpEdit
        {
            Name    = "PART_QuickJumper",
            Minimum = 1,
            Maximum = PageCount,
            Text    = CurrentPage.ToString()
        };
        _quickJumper.SetTemplatedParent(this);
        _quickJumper.KeyUp += HandleLineEditKeyUp;
        _quickJumperBindings = new CompositeDisposable
        {
            BindUtils.RelayBind(this, SizeTypeProperty, _quickJumper, TextBox.SizeTypeProperty),
            BindUtils.RelayBind(this, IsEnabledProperty, _quickJumper, InputElement.IsEnabledProperty),
            BindUtils.RelayBind(this, IsMotionEnabledProperty, _quickJumper, TextBox.IsMotionEnabledProperty)
        };

        var insertIndex = _infoIndicator is null ? _rootLayout.Children.Count : _rootLayout.Children.IndexOf(_infoIndicator);
        if (insertIndex < 0)
        {
            insertIndex = Math.Min(1, _rootLayout.Children.Count);
        }
        _rootLayout.Children.Insert(insertIndex, _quickJumper);
    }

    private void ReleaseTemplateParts()
    {
        if (_previousPageItem is not null)
        {
            _previousPageItem.Click -= HandleNavItemClicked;
        }
        if (_nextPageItem is not null)
        {
            _nextPageItem.Click -= HandleNavItemClicked;
        }

        ReleaseQuickJumper();
        _rootLayout        = null;
        _previousPageItem  = null;
        _nextPageItem      = null;
        _infoIndicator     = null;
        TemplateConfigured = false;
    }

    private void ReleaseQuickJumper()
    {
        if (_quickJumper is null)
        {
            return;
        }

        _quickJumperBindings?.Dispose();
        _quickJumperBindings = null;
        _quickJumper.KeyUp  -= HandleLineEditKeyUp;
        if (_quickJumper.GetVisualParent() is Panel parent)
        {
            parent.Children.Remove(_quickJumper);
        }
        else
        {
            _rootLayout?.Children.Remove(_quickJumper);
        }
        _quickJumper.SetTemplatedParent(null);
        _quickJumper = null;
    }
}
