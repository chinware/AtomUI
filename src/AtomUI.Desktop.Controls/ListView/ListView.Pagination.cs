using System.ComponentModel;
using System.Reactive.Disposables;
using AtomUI.Controls;
using AtomUI.Controls.Data;
using AtomUI.Data;
using Avalonia;

namespace AtomUI.Desktop.Controls;

public partial class ListView
{
    #region 公共属性定义
    
    public static readonly StyledProperty<int> PageIndexProperty =
        AvaloniaProperty.Register<ListView, int>(
            nameof(PageIndex), AbstractPagination.DefaultCurrentPage);
    
    public static readonly StyledProperty<int> PageSizeProperty =
        AvaloniaProperty.Register<ListView, int>(
            nameof(PageSize), 0);
    
    public int PageIndex
    {
        get => GetValue(PageIndexProperty);
        set => SetValue(PageIndexProperty, value);
    }
    
    public int PageSize
    {
        get => GetValue(PageSizeProperty);
        set => SetValue(PageSizeProperty, value);
    }
    
    #endregion
    
    private AbstractPagination? _topPagination;
    private AbstractPagination? _bottomPagination;
    
    private CompositeDisposable? _topPaginationDisposables;
    private CompositeDisposable? _bottomPaginationDisposables;
    private bool _isSyncingPagination;

    private void HandlePropertyChangedForPagination(AvaloniaPropertyChangedEventArgs change)
    {
        if (change.Property == PageSizeProperty)
        {
            if (_collectionView != null && _collectionView.PageSize != PageSize)
            {
                _collectionView.PageSize = PageSize;
            }
            SyncPagination(_topPagination);
            SyncPagination(_bottomPagination);
        }
        else if (change.Property == TopPaginationProperty)
        {
            HandleTopPaginationChanged(change);
        }
        else if (change.Property == BottomPaginationProperty)
        {
            HandleBottomPaginationChanged(change);
        }
        else if (change.Property == PaginationVisibilityProperty)
        {
            HandlePaginationVisibility();
        }
    }
    
    private void HandleTopPaginationChanged(AvaloniaPropertyChangedEventArgs args)
    {
        if (args.OldValue is AbstractPagination oldPagination)
        {
            oldPagination.CurrentPageChanged -= HandlePageChangeRequest;
            _topPaginationDisposables?.Dispose();
            _topPaginationDisposables = null;
            if (ReferenceEquals(_topPagination, oldPagination))
            {
                _topPagination = null;
            }
        }

        if (args.NewValue is AbstractPagination newPagination)
        {
            newPagination.CurrentPageChanged += HandlePageChangeRequest;
            _topPaginationDisposables        =  new CompositeDisposable();
            _topPaginationDisposables.Add(BindUtils.RelayBind(this, TopPaginationAlignProperty, newPagination, AbstractPagination.AlignProperty));
            _topPaginationDisposables.Add(BindUtils.RelayBind(this, IsHideOnSinglePageProperty, newPagination, AbstractPagination.IsHideOnSinglePageProperty));
            _topPaginationDisposables.Add(BindUtils.RelayBind(this, IsEnabledProperty, newPagination, AbstractPagination.IsEnabledProperty));
            _topPaginationDisposables.Add(BindUtils.RelayBind(this, IsMotionEnabledProperty, newPagination, AbstractPagination.IsMotionEnabledProperty));
            _topPagination = newPagination;
            SyncPagination(newPagination);
        }
        HandlePaginationVisibility();
    }
    
    private void HandleBottomPaginationChanged(AvaloniaPropertyChangedEventArgs args)
    {
        if (args.OldValue is AbstractPagination oldPagination)
        {
            oldPagination.CurrentPageChanged -= HandlePageChangeRequest;
            _bottomPaginationDisposables?.Dispose();
            _bottomPaginationDisposables = null;
            if (ReferenceEquals(_bottomPagination, oldPagination))
            {
                _bottomPagination = null;
            }
        }

        if (args.NewValue is AbstractPagination newPagination)
        {
            newPagination.CurrentPageChanged += HandlePageChangeRequest;
            _bottomPaginationDisposables     =  new CompositeDisposable();
            _bottomPaginationDisposables.Add(BindUtils.RelayBind(this, BottomPaginationAlignProperty, newPagination, AbstractPagination.AlignProperty));
            _bottomPaginationDisposables.Add(BindUtils.RelayBind(this, IsHideOnSinglePageProperty, newPagination, AbstractPagination.IsHideOnSinglePageProperty));
            _bottomPaginationDisposables.Add(BindUtils.RelayBind(this, IsEnabledProperty, newPagination, AbstractPagination.IsEnabledProperty));
            _bottomPaginationDisposables.Add(BindUtils.RelayBind(this, IsMotionEnabledProperty, newPagination, AbstractPagination.IsMotionEnabledProperty));
            _bottomPagination = newPagination;
            SyncPagination(newPagination);
        }
        HandlePaginationVisibility();
    }

    private void HandlePaginationVisibility()
    {
        if (PaginationVisibility == ListPaginationVisibility.None)
        {
            _topPagination?.IsVisible    = false;
            _bottomPagination?.IsVisible = false;
        }
        else if (PaginationVisibility == ListPaginationVisibility.Both)
        {
            _topPagination?.IsVisible    = true;
            _bottomPagination?.IsVisible = true;
        }
        else if (PaginationVisibility == ListPaginationVisibility.Top)
        {
            _topPagination?.IsVisible    = true;
            _bottomPagination?.IsVisible = false;
        }
        else
        {
            _topPagination?.IsVisible    = false;
            _bottomPagination?.IsVisible = true;
        }
    }
    
    private void HandlePageChangeRequest(object? sender, PageChangedEventArgs args)
    {
        if (_isSyncingPagination)
        {
            return;
        }

        if (_collectionView != null)
        {
            _collectionView.MoveToPage(args.PageIndex - 1);
        }
    }

    private void HandleCollectionPropertyChanged(object? sender, PropertyChangedEventArgs change)
    {
        if (sender is IListCollectionView listCollectionView)
        {
            if (change.PropertyName == nameof(ListCollectionView.PageSize))
            {
                SetCurrentValue(PageSizeProperty, listCollectionView.PageSize);
            }
        }
    }
    
    private void HandlePageChanging(object? sender, PageChangingEventArgs args)
    {
    }

    private void HandlePageChanged(object? sender, EventArgs args)
    {
        if (_collectionView != null)
        {
            SetCurrentValue(PageIndexProperty, _collectionView.PageIndex);
            SyncPagination(_topPagination);
            SyncPagination(_bottomPagination);
        }
    }
    
    private void ReConfigurePagination()
    {
        if (_collectionView != null)
        {
            if (_collectionView.PageSize != PageSize)
            {
                _collectionView.PageSize = PageSize;
            }

            SyncPagination(_topPagination);
            SyncPagination(_bottomPagination);
        }
    }

    private void SyncPagination(AbstractPagination? pagination)
    {
        if (pagination == null || _collectionView == null)
        {
            return;
        }

        _isSyncingPagination = true;
        try
        {
            pagination.Total       = _collectionView.TotalItemCount;
            pagination.PageSize    = PageSize;
            pagination.CurrentPage = PageIndex + 1;
        }
        finally
        {
            _isSyncingPagination = false;
        }
    }
}
