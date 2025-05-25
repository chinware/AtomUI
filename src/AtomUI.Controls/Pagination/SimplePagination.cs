using System.Diagnostics;
using AtomUI.Theme;
using AtomUI.Theme.Utils;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Input;
using Avalonia.Interactivity;

namespace AtomUI.Controls;

public class SimplePagination : AbstractPagination,
                                IControlSharedTokenResourcesHost
{
    #region 公共属性定义

    public static readonly StyledProperty<bool> IsReadOnlyProperty =
        AvaloniaProperty.Register<AbstractPagination, bool>(nameof(PaginationAlign), defaultValue:true);

    public bool IsReadOnly
    {
        get => GetValue(IsReadOnlyProperty);
        set => SetValue(IsReadOnlyProperty, value);
    }

    #endregion

    #region 内部属性定义

    Control IControlSharedTokenResourcesHost.HostControl => this;
    string IControlSharedTokenResourcesHost.TokenId => PaginationToken.ID;

    #endregion
    
    private PaginationNavItem? _previousPageItem;
    private PaginationNavItem? _nextPageItem;
    private TextBlock? _infoIndicator;
    private LineEdit? _quickJumper;

    static SimplePagination()
    {
        AffectsMeasure<SimplePagination>(IsReadOnlyProperty);
    }

    public SimplePagination()
    {
        this.RegisterResources();
    }

    protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
    {
        base.OnApplyTemplate(e);
        _previousPageItem = e.NameScope.Find<PaginationNavItem>(SimplePaginationTheme.PreviousNavItemPart);
        _nextPageItem     = e.NameScope.Find<PaginationNavItem>(SimplePaginationTheme.NextNavItemPart);
        _infoIndicator    = e.NameScope.Find<TextBlock>(SimplePaginationTheme.InfoIndicatorPart);
        _quickJumper      = e.NameScope.Find<LineEdit>(SimplePaginationTheme.QuickJumperPart);

        Debug.Assert(_nextPageItem != null);
        Debug.Assert(_previousPageItem != null);
        Debug.Assert(_quickJumper != null);
        _previousPageItem.Click += HandleNavItemClicked;
        _nextPageItem.Click     += HandleNavItemClicked;
        _quickJumper.KeyUp      += HandleLineEditKeyUp;
        
        HandlePageConditionChanged();
    }

    protected override void NotifyPageConditionChanged(int currentPage, int pageCount, int pageSize, long total)
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
        Debug.Assert(_previousPageItem != null);
        Debug.Assert(_nextPageItem != null);
        
        _previousPageItem.IsEnabled  = currentPage > 1;
        _previousPageItem.PageNumber = Math.Max(1, CurrentPage - 1);
        _nextPageItem.IsEnabled      = currentPage < pageCount;
        _nextPageItem.PageNumber     = Math.Min(pageCount, CurrentPage + 1);
        EmitCurrentPageChanged(CurrentPage, pageCount, pageSize);
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
        if (sender is LineEdit lineEdit)
        {
            if (e.Key == Key.Enter)
            {
                if (int.TryParse(lineEdit.Text?.Trim(), out var pageNumber))
                {
                    var pageCount   = (int)Math.Ceiling(Total / (double)PageSize);
                    CurrentPage = Math.Max(1, Math.Min(pageNumber, pageCount));
                }
                else
                {
                    lineEdit.Text = $"{CurrentPage}";
                }
            }
        }
    }
}