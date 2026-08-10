# 异步加载使用指南

本文说明应用和 Control 作者如何使用 AtomUI 的异步加载能力。状态模型、协调器时序、生命周期和兼容性由
[异步加载架构](../../architecture/systems/control-infrastructure/async-loading.md) 定义。

## 选择加载模式

| 场景 | 协调器 | 典型 Control |
| --- | --- | --- |
| 输入变化触发搜索，后一次请求覆盖前一次 | `AsyncSearchLoadCoordinator<TContext, TResult>` | AutoComplete、Mentions、Select |
| 展开节点触发加载，同一节点复用在飞任务 | `AsyncExpandLoadCoordinator<TContext, TResult>` | Cascader、TreeView、TreeSelect |

搜索类可以配置 debounce；展开类不使用 debounce，通过 in-flight 去重避免同一节点并发重入。

## 配置现有 Control

搜索类 Control 通常提供：

| 属性 | 作用 |
| --- | --- |
| `AsyncLoadDebounce` | 输入停止后等待多久再发送请求；`Zero` 表示立即发送 |
| `AsyncLoadTimeout` | 单次调用的总超时，包括 debounce 等待 |
| `OptionsAsyncLoader` / `OptionsLoader` | 实际加载器；`null` 表示不启用异步加载 |

Select 的加载触发点是 Popup 打开，因此没有 `AsyncLoadDebounce`。展开类 Control 通常提供
`AsyncLoadTimeout` 和 `DataLoader`。

### Select

```csharp
mySelect.OptionsLoader = new MyAsyncLoader();
mySelect.AsyncLoadTimeout = TimeSpan.FromSeconds(5);
```

```csharp
public sealed class MyAsyncLoader : ISelectOptionsAsyncLoader
{
    public async Task<SelectOptionsLoadResult> LoadAsync(
        object? context,
        CancellationToken cancellationToken)
    {
        var items = await _api.FetchUsersAsync(cancellationToken);
        return new SelectOptionsLoadResult { Data = items };
    }
}
```

### AutoComplete debounce

```xml
<atom:AutoComplete OptionsAsyncLoader="{Binding Loader}"
                   AsyncLoadDebounce="0:0:0.3"
                   AsyncLoadTimeout="0:0:8" />
```

新的输入会接管 loading 状态。被覆盖的旧请求返回 `Skipped`，不会触发终态事件，也不能用旧结果更新 UI。

### TreeView 节点加载

```csharp
myTree.DataLoader = new MyNodeLoader();
myTree.AsyncLoadTimeout = TimeSpan.FromSeconds(30);
```

```csharp
public sealed class MyNodeLoader : ITreeItemNodeLoader
{
    public async Task<TreeItemLoadResult> LoadAsync(
        ITreeItemNode parent,
        CancellationToken cancellationToken)
    {
        var children = await _api.FetchChildrenAsync(parent.Id, cancellationToken);
        return new TreeItemLoadResult
        {
            Data = children,
            StatusCode = RpcStatusCode.Success
        };
    }
}
```

同一节点的连续展开调用复用一个在飞 Task。首次成功后 item 标记为 `AsyncLoaded`，由 Control 决定后续是否需要再次加载。

## 处理终态

Control 的加载结果事件统一暴露 `IsSuccess`、`StatusCode`、`UserFriendlyMessage` 和可选 `Data`。应用应基于公开结果
类型处理提示，不依赖协调器的内部 CancellationToken 组合方式。

```csharp
myCascader.ItemAsyncLoaded += (_, args) =>
{
    if (args.Result?.IsSuccess == true)
    {
        return;
    }

    switch (args.Result?.StatusCode)
    {
        case RpcStatusCode.Timeout:
            ShowToast("加载超时，请重试");
            break;
        case RpcStatusCode.NetworkFailure:
            ShowToast("网络异常");
            break;
        default:
            ShowToast(args.Result?.UserFriendlyMessage ?? "加载失败");
            break;
    }
};
```

Loader 必须尊重传入的 `CancellationToken`。不要捕获并吞掉 `OperationCanceledException`；协调器需要根据 token 来源
区分 timeout、外部取消和请求覆盖。

## 新搜索类 Control 接入

1. Control 拥有一个长期复用的 `AsyncSearchLoadCoordinator<TContext, TResult>`。
2. 调用前把公开的 debounce 和 timeout 属性同步给协调器。
3. `Skipped` 直接返回，不清理由新请求接管的 loading 状态，不触发事件。
4. Success 更新数据；TimedOut、Cancelled 和 Faulted 映射到 Control 的公开结果类型。
5. template reapply、detach 或 owner 变化不能遗留旧事件、binding 或请求状态。

```csharp
private readonly AsyncSearchLoadCoordinator<string?, MyLoadResult> _coordinator = new();

private async Task LoadAsync(string? query)
{
    if (Loader is null)
    {
        return;
    }

    _coordinator.DebounceInterval = AsyncLoadDebounce;
    _coordinator.Timeout = AsyncLoadTimeout;

    var outcome = await _coordinator.LoadAsync(
        query,
        (context, token) => Loader.LoadAsync(context, token));

    if (outcome.IsSkipped)
    {
        return;
    }

    ApplyOutcome(outcome);
}
```

## 新展开类 Control 接入

1. 使用 `AsyncExpandLoadCoordinator<TContext, TResult>`。
2. 节点对象默认使用 `ReferenceEqualityComparer<T>.Instance`；只有稳定业务 ID 明确构成节点身份时才使用自定义 comparer。
3. 多个不同节点可以并发加载，同一节点复用在飞 Task。
4. 集合和 item 状态更新回到 UI 线程。
5. Control detach 时调用 `CancelAll()`。

```csharp
private readonly AsyncExpandLoadCoordinator<IMyNode, MyLoadResult> _coordinator =
    new(ReferenceEqualityComparer<IMyNode>.Instance);

protected override void OnDetachedFromVisualTree(VisualTreeAttachmentEventArgs e)
{
    _coordinator.CancelAll();
    base.OnDetachedFromVisualTree(e);
}
```

## 常见问题

**Debounce 是否计入 timeout？**

计入。Timeout 覆盖从 `LoadAsync` 调用开始到产生终态的总时间。

**超时后 loader 是否继续运行？**

协调器会取消传给 loader 的 token。实际底层操作必须正确消费该 token 才能及时停止。

**为什么 `Skipped` 不触发错误或完成事件？**

它表示旧搜索已经被新搜索取代。对外发布旧终态可能覆盖新结果或产生错误提示。

## 源码入口

- `src/AtomUI.Controls.Shared/AsyncLoad/`
- `src/AtomUI.Desktop.Controls/AutoComplete/AbstractAutoComplete.cs`
- `src/AtomUI.Desktop.Controls/Select/Select.AsyncOptionsLoad.cs`
- `src/AtomUI.Desktop.Controls/Cascader/CascaderView.AsyncItemDataLoad.cs`
- `src/AtomUI.Desktop.Controls/TreeView/TreeView.AsyncItemDataLoad.cs`
