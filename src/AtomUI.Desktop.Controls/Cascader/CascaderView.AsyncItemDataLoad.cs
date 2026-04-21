using System.Diagnostics;
using Avalonia;
using Avalonia.Threading;
using DynamicData;

namespace AtomUI.Desktop.Controls;

public partial class CascaderView
{
    #region 公开属性定义
    public static readonly StyledProperty<TimeSpan> AsyncLoadTimeoutProperty =
        AvaloniaProperty.Register<CascaderView, TimeSpan>(nameof(AsyncLoadTimeout),
            TimeSpan.FromSeconds(30));

    public TimeSpan AsyncLoadTimeout
    {
        get => GetValue(AsyncLoadTimeoutProperty);
        set => SetValue(AsyncLoadTimeoutProperty, value);
    }
    #endregion
    #region 内部属性定义
    internal static readonly DirectProperty<CascaderView, bool> HasItemAsyncDataLoaderProperty =
        AvaloniaProperty.RegisterDirect<CascaderView, bool>(nameof(HasItemAsyncDataLoader),
            o => o.HasItemAsyncDataLoader,
            (o, v) => o.HasItemAsyncDataLoader = v);
    
    private bool _hasItemAsyncDataLoader;

    internal bool HasItemAsyncDataLoader
    {
        get => _hasItemAsyncDataLoader;
        set => SetAndRaise(HasItemAsyncDataLoaderProperty, ref _hasItemAsyncDataLoader, value);
    }
    #endregion

    #region 私有字段
    private List<CancellationTokenSource>? _loadingTokens;
    #endregion

    private async Task LoadItemDataAsync(CascaderViewItem item)
    {
        if (DataLoader == null)
        {
            return;
        }

        var option = item.AttachedOption;
        if (option != null)
        {
            var cts = new CancellationTokenSource(AsyncLoadTimeout);
            _loadingTokens ??= new();
            _loadingTokens.Add(cts);
            
            item.IsLoading = true;
            await Dispatcher.UIThread.InvokeAsync(async () =>
            {
                try
                {
                    Debug.Assert(DataLoader != null);
                    var result = await DataLoader.LoadAsync(option, cts.Token);
                    if (!cts.Token.IsCancellationRequested)
                    {
                        item.IsLoading   = false;
                        item.AsyncLoaded = true;
                        ItemAsyncLoaded?.Invoke(this, new CascaderViewItemLoadedEventArgs(item, result));
                        if (result.IsSuccess)
                        {
                            if (result.Data?.Count > 0)
                            {
                                foreach (var child in result.Data)
                                {
                                    child.UpdateParentNode(option);
                                }
                                ((IList<ICascaderOption>)option.Children).AddRange(result.Data);
                            }
                        }
                    }
                }
                catch (OperationCanceledException)
                {
                    item.IsLoading = false;
                }
                finally
                {
                    cts.Dispose();
                    _loadingTokens?.Remove(cts);
                }
            });
        }
    }
}
