# OverlayDialog Modal 模式关闭后无法再次打开

## 现象

以 `IsModal = true` 打开 OverlayDialog，关闭后再次调用 `OpenAsync()` 或设置 `IsOpen = true` 无效，Dialog 不再弹出。

## 根因分析

### 核心问题：`_opening` 标志在 await 期间保持 `true`

`Dialog.OpenAsync()` 的结构：

```csharp
public async Task OpenAsync(CancellationToken cancellationToken = default)
{
    if (_openState != null || _opening) return;  // 守卫条件

    _opening = true;
    try
    {
        // ... 创建 host, 绑定, 显示 ...
        dialogHost.Show();

        if (IsModal)
        {
            await openState.ClosedTask.WaitAsync(cancellationToken);  // 阻塞在这里
        }
    }
    finally
    {
        _opening = false;  // 只有 await 完成后才会执行
    }
}
```

### 关闭时序问题

`NotifyClose()` 中的执行顺序：

```csharp
protected virtual void NotifyClose()
{
    _closing = true;
    try
    {
        var openState = _openState;
        _openState = null;
        openState?.Dispose();                          // 1. 关闭 DialogHost

        SetCurrentValue(IsOpenProperty, false);        // 2. 设置 IsOpen = false（带 ignore 标志）

        Closed?.Invoke(this, EventArgs.Empty);         // 3. 触发 Closed 事件 ← 用户可能在这里尝试重新打开
        _frameCancellationTokenSource?.Cancel();        // 4. 取消 frame token
        openState?.SetClosed(Result);                  // 5. 完成 ClosedTask → 释放 await
    }
    finally
    {
        _closing = false;
    }
}
```

### 竞态窗口

关键时序：

1. `NotifyClose()` 在 UI 线程同步执行
2. 步骤 5 `SetClosed(Result)` 调用 `TrySetResult`，完成 TaskCompletionSource
3. `OpenAsync` 中 `await` 的 continuation 被 **post 到 Dispatcher 队列**（不是同步执行）
4. `NotifyClose()` 返回，`_closing = false`
5. **此时 `_opening` 仍然是 `true`**（因为 continuation 还没执行）

如果用户在步骤 3 的 `Closed` 事件中尝试重新打开（或在 `NotifyClose` 返回后立即设置 `IsOpen = true`），会触发：

```csharp
HandleIsOpenChanged → Dispatcher.InvokeAsync(() => OpenAsync())
```

这个新的 `OpenAsync()` 被 post 到 Dispatcher 队列。问题在于：
- 如果 continuation（设置 `_opening = false`）先执行 → 重新打开成功
- 如果新的 `OpenAsync()` 先执行 → 命中 `_opening == true` 守卫 → 静默返回

### 同步 `Open()` 方法的额外问题

```csharp
public object? Open()
{
    _frameCancellationTokenSource = new CancellationTokenSource();
    var frame = new DispatcherFrame();
    _frameCancellationTokenSource.Token.Register(() => frame.Continue = false);
    Dispatcher.InvokeAsync(async () => await OpenAsync(_frameCancellationTokenSource.Token));
    Dispatcher.PushFrame(frame);
    return Result;
}
```

`NotifyClose()` 中 `_frameCancellationTokenSource?.Cancel()` 在 `SetClosed()` **之前**执行。这意味着：
- Token 被取消 → `WaitAsync(cancellationToken)` 抛出 `TaskCanceledException`
- 异常在 `Dispatcher.InvokeAsync` 中变成 faulted task（被吞掉）
- `finally` 中 `_opening = false` 仍然会执行（异常路径也走 finally）
- 但 `frame.Continue = false` 已经通过 token registration 设置了

这条路径理论上能正常恢复，但如果 `_frameCancellationTokenSource` 没有被重置为 `null`，下次 `Open()` 调用时会先 Cancel 旧的（已经 cancelled 的）source，这不会有问题。

### Popup 动画干扰（次要因素）

Popup 构造函数中：
```csharp
this.ConfigureMotionBindingStyle();  // 可能绑定 IsMotionEnabled = true
```

如果 Popup 通过样式获得了 `CloseMotion` 且 `_motionActor` 不为 null，`HandlePopupClosing` 会：
1. `e.Cancel = true` — 取消关闭
2. 播放关闭动画
3. 动画完成后再次设置 `IsOpen = false`

这会导致 `DialogHost.Close()` 中 `_popup.IsOpen = false` 不会立即关闭 popup，而是延迟关闭。在动画期间 popup 处于中间状态。

## 修复方案

### 方案 A：将 `_opening` 标志的重置移到 await 之前

```csharp
public async Task OpenAsync(CancellationToken cancellationToken = default)
{
    if (_openState != null || _opening) return;

    _opening = true;
    try
    {
        // ... 创建 host, 绑定, 显示 ...
        dialogHost.Show();
        Opened?.Invoke(this, EventArgs.Empty);
    }
    finally
    {
        _opening = false;  // 打开完成后立即重置
    }

    if (IsModal)
    {
        await openState.ClosedTask.WaitAsync(cancellationToken);
    }
}
```

优点：`_opening` 只保护创建/显示阶段，不会在 modal 等待期间阻塞重新打开。
缺点：需要用 `_openState != null` 作为"已打开"的守卫（已经有这个检查）。

### 方案 B：在 `NotifyClose` 中确保 `SetClosed` 在 `Closed` 事件之前

```csharp
// 调整顺序：先完成 Task，再触发事件
openState?.SetClosed(Result);                  // 先释放 await
_frameCancellationTokenSource?.Cancel();
Closed?.Invoke(this, EventArgs.Empty);         // 后触发事件
```

这样 `OpenAsync` 的 continuation 会在 `Closed` 事件之前被 post 到队列，保证执行顺序正确。

### 方案 C（推荐）：组合修复

1. 将 `_opening` 的语义改为仅保护"正在创建 host"阶段（方案 A）
2. 用 `_openState != null` 作为"dialog 已打开"的守卫
3. 确保 `NotifyClose` 中 `SetClosed` 在事件触发之前执行（方案 B）

## 验证步骤

1. 以 modal 模式打开 Dialog
2. 关闭 Dialog
3. 立即再次打开 — 应该成功
4. 在 `Closed` 事件处理器中重新打开 — 应该成功
5. 快速连续开关 — 不应死锁或异常
