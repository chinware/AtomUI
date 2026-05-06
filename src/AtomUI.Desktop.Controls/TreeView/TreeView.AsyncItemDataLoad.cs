using System.Diagnostics;
using Avalonia;
using Avalonia.Threading;
using DynamicData;

namespace AtomUI.Desktop.Controls;

public partial class TreeView
{
    #region 内部属性定义
    internal static readonly DirectProperty<TreeView, bool> HasTreeItemDataLoaderProperty =
        AvaloniaProperty.RegisterDirect<TreeView, bool>(nameof(HasTreeItemDataLoader),
            o => o.HasTreeItemDataLoader,
            (o, v) => o.HasTreeItemDataLoader = v);
    
    private bool _hasTreeItemDataLoader;

    internal bool HasTreeItemDataLoader
    {
        get => _hasTreeItemDataLoader;
        set => SetAndRaise(HasTreeItemDataLoaderProperty, ref _hasTreeItemDataLoader, value);
    }
    #endregion

    #region 公开属性定义
    public static readonly StyledProperty<TimeSpan> AsyncLoadTimeoutProperty =
        AvaloniaProperty.Register<TreeView, TimeSpan>(nameof(AsyncLoadTimeout),
            TimeSpan.FromSeconds(30));

    public TimeSpan AsyncLoadTimeout
    {
        get => GetValue(AsyncLoadTimeoutProperty);
        set => SetValue(AsyncLoadTimeoutProperty, value);
    }
    #endregion

    #region 私有字段
    private List<CancellationTokenSource>? _loadingTokens;
    #endregion

    private void HandleNodeLoadRequest(TreeViewItem viewItem)
    {
        if (DataLoader == null)
        {
            return;
        }
        if (DataLoader != null && ItemsSource == null)
        {
            throw new InvalidOperationException("ITreeNodeDataLoader is set, but the tree nodes are not initially set via ItemsSource.");
        }
        var data = TreeItemFromContainer(viewItem);
        if (data is ITreeItemNode treeItemData)
        {
            var cts = new CancellationTokenSource(AsyncLoadTimeout);
            _loadingTokens ??= new();
            _loadingTokens.Add(cts);
            
            viewItem.IsLoading = true;
            Dispatcher.UIThread.InvokeAsync(async () =>
            {
                try
                {
                    Debug.Assert(DataLoader != null);
                    var result = await DataLoader.LoadAsync(treeItemData, cts.Token);
                    if (!cts.Token.IsCancellationRequested)
                    {
                        viewItem.IsLoading   = false;
                        viewItem.AsyncLoaded = true; // TODO 是不是应该多给几次机会？
                        TreeItemLoaded?.Invoke(this, new TreeViewItemLoadedEventArgs(viewItem, result));
                        if (result.IsSuccess)
                        {
                            if (result.Data?.Count > 0)
                            {
                                foreach (var child in result.Data)
                                {
                                    child.UpdateParentNode(treeItemData);
                                }
                                ((IList<ITreeItemNode>)treeItemData.Children).AddRange(result.Data);
                                viewItem.IsExpanded = true;
                            }
                        }
                    }
                }
                catch (OperationCanceledException)
                {
                    viewItem.IsLoading = false;
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
