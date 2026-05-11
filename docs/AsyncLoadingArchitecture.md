# AtomUI 异步加载体系架构与使用指南

> 适用于 `AtomUI.Desktop.Controls` 下所有需要异步拉取数据的控件：AutoComplete / Mentions / Select（搜索类），Cascader / TreeView / TreeSelect（展开类）。

## 1. 设计目标

异步加载面对的几个常见工程问题：

- **搜索类场景**用户高频输入，需要 debounce 避免每敲一键发一个请求
- **展开类场景**同一节点可能被多次触发加载，需要去重避免并发重入
- 无论哪种场景，请求都可能卡在慢后端，需要硬超时兜底
- 取消、超时、被覆盖这几种"非成功终态"需要统一表达，调用方能分辨

AtomUI 把这些需求抽成两种场景专用的协调器，放在 `AtomUI.Controls.Shared/AsyncLoad/`。控件只负责"发出请求 + 处理结果"，一切时序和去重细节交给协调器。

## 2. 核心契约

### 2.1 `AsyncLoadStatus`

异步加载的终态枚举。

```csharp
public enum AsyncLoadStatus
{
    Success,    // 加载成功
    Cancelled,  // 外部 CancellationToken 触发
    TimedOut,   // 达到 Timeout
    Faulted,    // loader 抛异常
    Skipped     // 搜索类专用：被后续调用覆盖，本次结果丢弃
}
```

### 2.2 `AsyncLoadOutcome<TResult>`

协调器返回的结果包装。

```csharp
public sealed class AsyncLoadOutcome<TResult> where TResult : class
{
    public AsyncLoadStatus Status { get; init; }
    public TResult?        Result { get; init; }  // Status == Success 时非空
    public Exception?      Error  { get; init; }  // Status == Faulted 时非空

    public bool IsSuccess   { get; }
    public bool IsTimedOut  { get; }
    public bool IsCancelled { get; }
    public bool IsFaulted   { get; }
    public bool IsSkipped   { get; }
}
```

**调用方处理模式**：

```csharp
var outcome = await coordinator.LoadAsync(context, (ctx, token) => loader.LoadAsync(ctx, token));

if (outcome.IsSkipped) return;  // 被后续请求覆盖，不触发任何回调
if (outcome.IsSuccess) { /* 用 outcome.Result */ }
else                   { /* 上报超时 / 取消 / 错误 */ }
```

### 2.3 `AsyncSearchLoadCoordinator<TContext, TResult>`

**搜索类专用**。特性：

- **取消前一次**：每次 `LoadAsync` 调用会取消前一个进行中的请求
- **Debounce**：`DebounceInterval > Zero` 时，请求被延迟启动；延迟期间来新请求直接替换（旧请求返回 `Skipped`）
- **Timeout**：`Timeout` 控制总超时（含 debounce 时间）

```csharp
public sealed class AsyncSearchLoadCoordinator<TContext, TResult> where TResult : class
{
    public TimeSpan DebounceInterval { get; set; } = TimeSpan.Zero;
    public TimeSpan Timeout          { get; set; } = System.Threading.Timeout.InfiniteTimeSpan;

    public Task<AsyncLoadOutcome<TResult>> LoadAsync(
        TContext context,
        Func<TContext, CancellationToken, Task<TResult>> loader,
        CancellationToken external = default);

    public void Cancel();
}
```

`TContext` 可以是引用类型也可以是值类型，**无 `notnull` 约束**，允许 `null` 作为"空搜索"上下文。

### 2.4 `AsyncExpandLoadCoordinator<TContext, TResult>`

**展开类专用**。特性：

- **in-flight 去重**：同一 `TContext` key 正在加载中时，新调用复用同一个 Task
- **Timeout**：每个请求独立计时
- **CancelAll**：控件 detach 时一次性取消所有在飞请求

```csharp
public sealed class AsyncExpandLoadCoordinator<TContext, TResult>
    where TContext : notnull
    where TResult : class
{
    public AsyncExpandLoadCoordinator(IEqualityComparer<TContext>? dedupComparer = null);

    public TimeSpan Timeout { get; set; } = System.Threading.Timeout.InfiniteTimeSpan;

    public Task<AsyncLoadOutcome<TResult>> LoadOrJoinAsync(
        TContext context,
        Func<TContext, CancellationToken, Task<TResult>> loader,
        CancellationToken external = default);

    public void CancelAll();
}
```

展开类推荐使用 `ReferenceEqualityComparer<T>.Instance` 作为去重比较器（按节点引用判等）：

```csharp
new AsyncExpandLoadCoordinator<ITreeItemNode, TreeItemLoadResult>(
    ReferenceEqualityComparer<ITreeItemNode>.Instance);
```

## 3. 控件 API 总览

### 搜索类（AutoComplete / Mentions / Select）

| 属性 | 类型 | 默认值 | 作用 |
|---|---|---|---|
| `AsyncLoadDebounce` | `TimeSpan` | `Zero`（AutoComplete / Mentions） | 输入后等待多久才发请求；为 `Zero` 即不 debounce |
| `AsyncLoadTimeout` | `TimeSpan` | `10s` | 单次请求超时 |
| `OptionsAsyncLoader` / `OptionsLoader` | `I*OptionsAsyncLoader?` | `null` | 实际 loader，`null` 时不触发异步加载 |

Select 没有 `AsyncLoadDebounce` —— 它的异步加载触发点是 Popup 打开，不是输入变化。

### 展开类（Cascader / TreeView / TreeSelect）

| 属性 | 类型 | 默认值 | 作用 |
|---|---|---|---|
| `AsyncLoadTimeout` | `TimeSpan` | `30s` | 单次节点加载超时 |
| `DataLoader` | `I*NodeLoader?` / `I*ItemDataLoader?` | `null` | 节点加载器 |

展开类没有 `AsyncLoadDebounce`，因为展开不是高频操作；真正的并发风险由协调器的 in-flight 去重解决。

### 共同暴露的加载状态

| 属性/事件 | 含义 |
|---|---|
| `IsLoading` | 当前是否有请求在飞 |
| `AsyncLoaded`（展开类 item 上） | 该节点是否已经加载过（防止已展开的节点重复请求） |
| `OptionsLoaded` / `ItemAsyncLoaded` / `TreeItemLoaded` | 加载终态事件（含成功/超时/取消/错误） |

加载结果类型（`CompleteOptionsLoadResult` / `MentionOptionsLoadResult` / `SelectOptionsLoadResult` / `CascaderItemLoadResult` / `TreeItemLoadResult`）共享同一组字段：

```csharp
public bool           IsSuccess           { get; }  // => StatusCode == Success
public RpcStatusCode  StatusCode          { get; }  // Success / Timeout / Cancelled / ...
public string?        UserFriendlyMessage { get; }
public IReadOnlyList<T>? Data             { get; }
```

## 4. 数据流

### 搜索类

```
  [用户输入]
        │
        ▼
  FilterValue / SearchText 变化
        │
        ▼
  TryPopulateAsync(text)
        │
        ▼
  _coordinator.LoadAsync(text, loader)
        │
        ├── DebounceInterval 延迟（若 > Zero）
        │      └── 延迟中来了新请求？旧请求返回 Skipped
        │
        ├── 启动 Timeout 倒计时
        │
        ├── loader.LoadAsync(ctx, linkedToken)
        │
        ▼
  AsyncLoadOutcome
        │
        ├── Success   → 更新 OptionsSource + fire *Loaded(Success)
        ├── Skipped   → return, 不触发事件
        ├── TimedOut  → fire *Loaded(StatusCode=Timeout)
        ├── Cancelled → fire *Loaded(StatusCode=Cancelled)
        └── Faulted   → fire *Loaded(StatusCode=Unknown, Error)
```

### 展开类

```
  [用户点开节点]
        │
        ▼
  HandleNodeLoadRequest(item)
        │
        ▼
  _coordinator.LoadOrJoinAsync(nodeRef, loader)
        │
        ├── 同 nodeRef 已有 Task 在飞？复用它
        ├── 否则新建 linkedCts + CancelAfter(Timeout)
        ├── loader.LoadAsync(nodeRef, linkedToken)
        │
        ▼
  AsyncLoadOutcome（在 UI 线程 Dispatcher.InvokeAsync 中处理）
        │
        ├── Success   → item.AsyncLoaded = true; 追加子节点; fire *Loaded
        ├── TimedOut  → fire *Loaded(StatusCode=Timeout)
        ├── Cancelled → fire *Loaded(StatusCode=Cancelled)
        └── Faulted   → fire *Loaded(StatusCode=Unknown, Error)
```

## 5. 使用示例

### 5.1 最简：给 Select 配一个异步 loader

```csharp
myselect.OptionsLoader = new MyAsyncLoader();
myselect.AsyncLoadTimeout = TimeSpan.FromSeconds(5);  // 默认 10s
```

```csharp
public sealed class MyAsyncLoader : ISelectOptionsAsyncLoader
{
    public async Task<SelectOptionsLoadResult> LoadAsync(object? context, CancellationToken token)
    {
        var items = await _api.FetchUsersAsync(token);
        return new SelectOptionsLoadResult { Data = items };
    }
}
```

无需手写取消 / 超时 / 线程调度，控件内部用 coordinator 处理好。

### 5.2 AutoComplete 开启 debounce

```xml
<atom:AutoComplete OptionsAsyncLoader="{Binding Loader}"
                   AsyncLoadDebounce="0:0:0.3"
                   AsyncLoadTimeout="0:0:8" />
```

用户每次敲键都会重置 debounce 计时器；停止敲键 300ms 后才真正发请求。被后续请求覆盖的旧请求返回 `Skipped`，不触发 `OptionsLoaded` 事件。

### 5.3 处理加载失败

```csharp
mycascader.ItemAsyncLoaded += (sender, args) =>
{
    if (!args.Result?.IsSuccess == true)
    {
        switch (args.Result?.StatusCode)
        {
            case RpcStatusCode.Timeout:
                ShowToast("加载超时，请重试");
                break;
            case RpcStatusCode.NetworkFailure:
                ShowToast("网络异常");
                break;
            default:
                ShowToast($"加载失败: {args.Result?.UserFriendlyMessage}");
                break;
        }
    }
};
```

timeout / cancel / faulted 都会触发事件，便于统一处理用户提示。

### 5.4 TreeView 展开时加载子节点

```csharp
mytree.DataLoader = new MyNodeLoader();
mytree.AsyncLoadTimeout = TimeSpan.FromSeconds(30);  // 默认值
```

```csharp
public sealed class MyNodeLoader : ITreeItemNodeLoader
{
    public async Task<TreeItemLoadResult> LoadAsync(ITreeItemNode parent, CancellationToken token)
    {
        var children = await _api.FetchChildrenAsync(parent.Id, token);
        return new TreeItemLoadResult
        {
            Data      = children,
            StatusCode = RpcStatusCode.Success
        };
    }
}
```

同一父节点即使被快速连续点开两次，也只会触发一次实际请求（第二次 await 第一次的 Task）。首次成功后 `item.AsyncLoaded = true`，后续不再触发请求。

## 6. 新控件接入指引

如果你在开发新控件，需要异步数据加载，按场景选一个协调器即可。

### 6.1 搜索类控件模板

```csharp
using AtomUI.Controls.AsyncLoad;

public class MySearchControl : TemplatedControl
{
    public static readonly StyledProperty<TimeSpan> AsyncLoadDebounceProperty =
        AvaloniaProperty.Register<MySearchControl, TimeSpan>(
            nameof(AsyncLoadDebounce),
            TimeSpan.Zero,
            validate: v => v.TotalMilliseconds >= 0);

    public static readonly StyledProperty<TimeSpan> AsyncLoadTimeoutProperty =
        AvaloniaProperty.Register<MySearchControl, TimeSpan>(
            nameof(AsyncLoadTimeout),
            TimeSpan.FromSeconds(10));

    public static readonly StyledProperty<IMyAsyncLoader?> LoaderProperty =
        AvaloniaProperty.Register<MySearchControl, IMyAsyncLoader?>(nameof(Loader));

    public TimeSpan AsyncLoadDebounce { ... }
    public TimeSpan AsyncLoadTimeout  { ... }
    public IMyAsyncLoader? Loader     { ... }

    public event EventHandler<MyLoadedEventArgs>? DataLoaded;

    private readonly AsyncSearchLoadCoordinator<string?, MyLoadResult> _coordinator = new();

    private async Task LoadAsync(string? query)
    {
        if (Loader == null) return;

        _coordinator.DebounceInterval = AsyncLoadDebounce;
        _coordinator.Timeout          = AsyncLoadTimeout;

        IsLoading = true;
        var outcome = await _coordinator.LoadAsync(
            query,
            (ctx, token) => Loader.LoadAsync(ctx, token));

        if (outcome.IsSkipped) return;  // 被后续请求覆盖
        IsLoading = false;

        if (outcome.IsSuccess && outcome.Result != null)
        {
            ApplyResult(outcome.Result);
            DataLoaded?.Invoke(this, new MyLoadedEventArgs(query, outcome.Result));
        }
        else
        {
            var statusCode = outcome.Status switch
            {
                AsyncLoadStatus.TimedOut  => RpcStatusCode.Timeout,
                AsyncLoadStatus.Cancelled => RpcStatusCode.Cancelled,
                _                         => RpcStatusCode.Unknown
            };
            DataLoaded?.Invoke(this, new MyLoadedEventArgs(query, new MyLoadResult
            {
                StatusCode          = statusCode,
                UserFriendlyMessage = outcome.Error?.Message
            }));
        }
    }
}
```

**要点**：

1. `_coordinator` 作为实例字段，整个控件生命周期共用一个
2. 每次调用 `LoadAsync` 前把 `DebounceInterval` / `Timeout` 从控件属性同步过去
3. `outcome.IsSkipped` 直接 return，不清 `IsLoading`（因为新请求接管了），不 fire 事件
4. 所有非成功终态统一转成 `StatusCode` fire 事件，调用方不需要分辨是 coordinator 内部哪一步失败

### 6.2 展开类控件模板

```csharp
using AtomUI.Controls.AsyncLoad;

public class MyTreeControl : TemplatedControl
{
    public static readonly StyledProperty<TimeSpan> AsyncLoadTimeoutProperty =
        AvaloniaProperty.Register<MyTreeControl, TimeSpan>(
            nameof(AsyncLoadTimeout),
            TimeSpan.FromSeconds(30));

    public static readonly StyledProperty<IMyNodeLoader?> NodeLoaderProperty =
        AvaloniaProperty.Register<MyTreeControl, IMyNodeLoader?>(nameof(NodeLoader));

    public TimeSpan AsyncLoadTimeout  { ... }
    public IMyNodeLoader? NodeLoader  { ... }

    public event EventHandler<MyNodeLoadedEventArgs>? NodeLoaded;

    private readonly AsyncExpandLoadCoordinator<IMyNode, MyLoadResult> _coordinator
        = new(ReferenceEqualityComparer<IMyNode>.Instance);

    private async Task LoadNodeAsync(MyTreeItem item, IMyNode node)
    {
        if (NodeLoader == null) return;

        _coordinator.Timeout = AsyncLoadTimeout;
        item.IsLoading       = true;

        var outcome = await _coordinator.LoadOrJoinAsync(
            node,
            (ctx, token) => NodeLoader.LoadAsync(ctx, token));

        await Dispatcher.InvokeAsync(() =>
        {
            item.IsLoading = false;

            if (outcome.IsSuccess && outcome.Result != null)
            {
                item.AsyncLoaded = true;
                ApplyChildren(node, outcome.Result);
                NodeLoaded?.Invoke(this, new MyNodeLoadedEventArgs(item, outcome.Result));
                return;
            }

            var statusCode = outcome.Status switch
            {
                AsyncLoadStatus.TimedOut  => RpcStatusCode.Timeout,
                AsyncLoadStatus.Cancelled => RpcStatusCode.Cancelled,
                _                         => RpcStatusCode.Unknown
            };
            NodeLoaded?.Invoke(this, new MyNodeLoadedEventArgs(item, new MyLoadResult
            {
                StatusCode          = statusCode,
                UserFriendlyMessage = outcome.Error?.Message
            }));
        });
    }

    protected override void OnDetachedFromVisualTree(VisualTreeAttachmentEventArgs e)
    {
        base.OnDetachedFromVisualTree(e);
        _coordinator.CancelAll();
    }
}
```

**要点**：

1. 构造时显式传入 `ReferenceEqualityComparer<T>.Instance`，按引用判等而不是按 `Equals` —— 避免不同实例但 `Equals` 相等的节点被错误去重
2. `outcome` 处理在 `Dispatcher.InvokeAsync` 里统一走 UI 线程，避免子节点集合在非 UI 线程被写
3. `OnDetachedFromVisualTree` 时调用 `CancelAll()`，所有在飞请求立即取消
4. 不写"取消前一次"逻辑 —— 展开类每个节点独立，多节点同时展开是正常场景

### 6.3 Loader 接口规范

不管搜索类还是展开类，loader 接口都长这样：

```csharp
public interface IMyAsyncLoader
{
    Task<MyLoadResult> LoadAsync(TContext context, CancellationToken token);
}
```

- **永远返回非空 `Result`**；用 `StatusCode` 表达失败，而不是抛异常或返回 null
- **尊重 `CancellationToken`**：长操作中要 `token.ThrowIfCancellationRequested()` 或把 token 传给下游（HttpClient、DB 查询等）
- **不要自己捕获 `OperationCanceledException`**：让它抛出去，coordinator 会识别是 timeout 还是外部取消

## 7. 常见问题

### Q: Debounce 和 Timeout 的关系？

Timeout 覆盖的是"从 `LoadAsync` 调用开始到拿到结果"的总时间（含 debounce 等待）。比如 `DebounceInterval=300ms`、`Timeout=5s`，那 loader 实际最多跑 `5s - 300ms = 4.7s`。

### Q: 超时的请求会继续在后台跑吗？

不会。coordinator 通过 linked CTS 把 token 传给 loader；`CancelAfter(Timeout)` 触发时 token 进入 cancelled 状态，loader 里的 HttpClient / DB 操作应该立即响应取消。

### Q: 展开类为什么不按 `Equals` 去重？

`Equals` 可能把不同实例视作相等（比如两个从 API 返回的值对象）。按引用去重能保证"同一个节点对象"才复用 Task，这是展开语义下最安全的默认。如果你的节点有稳定 Id 且希望按 Id 去重，传入自定义 `IEqualityComparer<TContext>`。

### Q: `Skipped` 为什么要静默？

搜索类 debounce 场景下，被覆盖的旧请求对用户来说"根本没发生"—— UI 不会看到它的结果，也不应该看到它的超时/错误。如果 fire 事件调用方很可能误更新 UI（比如把旧结果渲染覆盖新结果）。`Skipped` 是 coordinator 给调用方的"安全返回"信号，直接 return 即可。

### Q: Loader 抛异常会怎样？

coordinator 捕获任何 `Exception`（除了 `OperationCanceledException`）转成 `Faulted` 状态，`outcome.Error` 保留原始异常。调用方应当把 `Faulted` 也 fire 成事件，让应用层决定怎么显示。

## 8. 参考实现

源码位置（便于按需对照）：

- 核心契约：`src/AtomUI.Controls.Shared/AsyncLoad/`
  - `AsyncLoadStatus.cs` / `AsyncLoadOutcome.cs`
  - `AsyncSearchLoadCoordinator.cs` / `AsyncExpandLoadCoordinator.cs`
  - `ReferenceEqualityComparer.cs`
- 搜索类参考：
  - `src/AtomUI.Desktop.Controls/AutoComplete/AbstractAutoComplete.cs`（debounce + timeout）
  - `src/AtomUI.Desktop.Controls/Select/Select.AsyncOptionsLoad.cs`（只 timeout，无 debounce）
- 展开类参考：
  - `src/AtomUI.Desktop.Controls/Cascader/CascaderView.AsyncItemDataLoad.cs`
  - `src/AtomUI.Desktop.Controls/TreeView/TreeView.AsyncItemDataLoad.cs`
