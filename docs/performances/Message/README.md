# Message 性能优化记录

## 范围

- 控件：`Message`、`MessageCard`、`WindowMessageManager`。
- Gallery：真实 `MessageShowCase.axaml`，源码形态为 `ShowCaseItem=4`、`Button=7`。
- 工具：`tools/performances/AtomUI.Performance --suite message`，`tools/performances/AtomUI.GalleryPerformance --showcase message`。

## 主要瓶颈与风险

1. `MessageShowCase`、`BreadcrumbShowCase`、`FormShowCase`、`UploadShowCase` 在 `OnAttachedToVisualTree` 里提前创建 `WindowMessageManager`。用户不触发 Message 时，也会安装 adorner、订阅窗口 margin observable、应用 manager 模板。
2. `WindowMessageManager.Show()` 调用 `DispatcherTimer.RunOnce(...)` 后丢弃返回的 `IDisposable`，卡片提前关闭、ShowCase detach 或 manager dispose 后无法取消自动关闭 timer。
3. `MessageClosed += OnMessageClosed` 没有对称解绑；和 timer、`OnClose` 闭包组合后会延长对象生命周期。
4. `MessageCard` 在 `_motionActor == null` 时关闭不会设置 `IsClosed=true`，极短生命周期、re-template 或 detach 边界下卡片可能不触发关闭事件。
5. `MessageCard` 默认 icon 与 type 伪类更新逻辑不完整：自定义 icon 下仍可能多创建默认 icon，`MessageType` 动态切换时旧伪类不会清掉。

## 实施内容

| Phase | 内容 | 状态 |
| --- | --- | --- |
| Phase 0 | 新增 `Message` 控件级 suite、`--verify-message-states` 和真实 `MessageShowCase` Gallery 测量入口 | Done |
| Phase 1 | 修复 timer、事件、`OnClose`、detach/dispose 清理路径 | Done |
| Phase 2 | Gallery 中使用 Message 的 ShowCase 改为首次实际展示 Message 时才创建 `WindowMessageManager` | Done |
| Phase 3 | `MessageCard` 默认 icon 按模板应用后创建；自定义 icon 不被覆盖；关闭无 motion actor 时也能完成状态 | Done |
| Phase 4 | `MessageCard` / `WindowMessageManager` 伪类状态改为只切换前后状态，`MessageCard` type 样式从属性 selector 切到伪类 selector，避免全量 selector 状态计算 | Done |
| Phase 5 | `RemoveExcessMessages()` 去掉 `ToList()` 热路径分配 | Done |
| Phase 6 | 补状态/生命周期验证 | Done |
| Phase 7 | Gallery 实测、文档和清理 | Done |

## Gallery 加载对比

口径：Debug / Avalonia Headless / `1300x900`，`--cold-iterations 5 --warmup 3 --iterations 20 --timeout-ms 15000`。After 冷启动首轮有明显波动，按相同参数复跑后采用稳定样本；两轮 after 都显示 repeated 和 alloc 下降。

| Metric | Before | After | Improvement |
| --- | ---: | ---: | ---: |
| `MessageShowCase` cold first navigation mean | `109.07 ms` | `98.86 ms` | `9.36%` faster |
| `MessageShowCase` cold first navigation median | `110.63 ms` | `100.82 ms` | `8.87%` faster |
| `MessageShowCase` cold first navigation P95 | `114.52 ms` | `101.93 ms` | `10.99%` faster |
| `MessageShowCase` cold alloc mean | `3581.86 KB` | `3500.47 KB` | `2.27%` less KB |
| `MessageShowCase` repeated mean | `30.97 ms` | `28.52 ms` | `7.91%` faster |
| `MessageShowCase` repeated median | `30.04 ms` | `28.18 ms` | `6.19%` faster |
| `MessageShowCase` repeated P95 | `36.69 ms` | `30.33 ms` | `17.34%` faster |
| `MessageShowCase` repeated alloc mean | `2475.54 KB` | `2455.45 KB` | `0.81%` less KB |

运行时 route visual shape 仍是 `96 visuals / 21 logical / 7 Button / 4 ShowCaseItem`。本轮主要收益来自不再默认创建和安装 `WindowMessageManager`，这部分在 route 子树统计里不直接显示，但体现在 allocation 和 navigation timing 上。

## 控件级结果

最终 `Message` suite 使用 no-motion 场景做稳定热路径基准。Motion 场景会调度动画任务，批量实例化时波动很大，不作为回归门禁；动画行为通过控件功能和状态验证覆盖。

当前结果：

| Scenario | ms/item | KB/item | Visual/root |
| --- | ---: | ---: | ---: |
| `MessageCard.Information.NoMotion` | `0.807` | `115.2` | `9` |
| `MessageCard.Success.NoMotion` | `0.712` | `112.1` | `9` |
| `MessageCard.Warning.NoMotion` | `0.768` | `112.1` | `9` |
| `MessageCard.Error.NoMotion` | `1.019` | `112.1` | `9` |
| `MessageCard.Loading.NoMotion` | `0.950` | `121.5` | `9` |
| `WindowMessageManager.Empty.Closed` | `0.073` | `12.6` | `2` |
| `WindowMessageManager.Show.Single.NoMotion` | `1.062` | `134.8` | `12` |
| `WindowMessageManager.Show.MaxItems.NoMotion` | `10.868` | `1578.0` | `48` |

`Show.MaxItems.NoMotion` 会创建 12 条消息并按 `MaxItems=5` 立即收敛到 5 条可见卡片，主要用于观察批量 show 和 excess cleanup 的结构，不用于和单卡场景比较。

## 正确性与泄露验证

新增并通过：

```bash
dotnet run --project tools/performances/AtomUI.Performance/AtomUI.Performance.csproj \
  --framework net10.0 --no-build -- --verify-message-states
```

覆盖内容：

- `MessageType` 切换会清理旧伪类并设置新伪类。
- 自定义 icon 不会被默认 type icon 覆盖。
- `Show()` 早于 `PART_Items` 模板应用时，第一条消息会在模板应用后显示，不会被丢弃。
- 模板前 pending message 会在 `Dispose()` 时清空，不保留 `Message` / `OnClose` 闭包。
- 自动关闭 timer 会被 manager 追踪，并在消息关闭后释放。
- `WindowMessageManager.Dispose()` 即使没有安装到 `TopLevel`，也会清理 timer、事件和卡片回调。

## 复现命令

```bash
dotnet build tools/performances/AtomUI.Performance/AtomUI.Performance.csproj --framework net10.0 --no-restore
dotnet run --project tools/performances/AtomUI.Performance/AtomUI.Performance.csproj --framework net10.0 --no-build -- --suite message --count 80
dotnet run --project tools/performances/AtomUI.Performance/AtomUI.Performance.csproj --framework net10.0 --no-build -- --verify-message-states
```

```bash
dotnet build tools/performances/AtomUI.GalleryPerformance/AtomUI.GalleryPerformance.csproj --framework net10.0 --no-restore
dotnet run --project tools/performances/AtomUI.GalleryPerformance/AtomUI.GalleryPerformance.csproj --framework net10.0 --no-build -- --showcase message --cold-iterations 5 --warmup 3 --iterations 20 --timeout-ms 15000
```

`GalleryPerformance` 构建仍有既有 warning：`DataGridColumn._clipboardContentBinding` 未使用，和本轮 Message 改动无关。
