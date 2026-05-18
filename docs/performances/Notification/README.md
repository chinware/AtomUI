# Notification 性能优化记录

## 范围

- 控件：`Notification`、`NotificationCard`、`NotificationProgressBar`、`WindowNotificationManager`。
- Gallery：真实 `NotificationShowCase.axaml`，源码形态为 `ShowCaseItem=6`、`Button=14`。
- 工具：`tools/performances/AtomUI.Performance --suite notification`，`tools/performances/AtomUI.GalleryPerformance --showcase notification`。

## 优化前问题

1. `NotificationShowCase` attach 时默认创建 7 个 `WindowNotificationManager`。用户不点击任何按钮时，也会提前安装 adorner、订阅窗口 safe-area observable。
2. `WindowNotificationManager` 构造时提前创建过期检测 timer、清理 timer、cleanup queue/set。没有 active notification 时也承担 timer 与集合成本。
3. `WindowNotificationManager.Dispose()` 在 `_topLevel is null` 时直接返回，控件级或非 TopLevel 使用路径下 timer、队列、card 事件存在无法释放风险。
4. `NotificationCard` 关闭后只移除 visual，未清 `PointerPressed`、`NotificationClosed`、`OnClick`、`OnClose`，可能延长 card 与外部闭包生命周期。
5. `NotificationCard` 默认 icon 没有区分“生成的默认 icon”和“调用方自定义 icon”，动态切换 type 时存在状态同步风险。
6. `NotificationType`、`Position` 状态依赖属性 selector 和多次全量伪类设置，状态变化成本偏高。
7. `NotificationProgressBar` 默认存在于 card 模板中，普通不显示进度的 notification 也会创建 progress bar、MultiBinding 和 converter。

## 实施内容

| Phase | 内容 | 结果 |
| --- | --- | --- |
| Phase 0 | 建立控件级 suite 和真实 `NotificationShowCase` 基线 | 已完成，使用临时 detached worktree 对照优化前 HEAD |
| Phase 1 | 修复 manager/card 生命周期 | timer、queue、pending notification、card 事件与回调均有释放路径 |
| Phase 2 | Gallery manager 按需创建 | 7 个 manager 改为对应 placement 首次 show 时创建 |
| Phase 3 | 默认 icon 与状态伪类收敛 | 默认 icon 按 type 复用；自定义 icon 不被覆盖；伪类只切 previous/current |
| Phase 4 | 属性 selector 改为伪类 selector | `NotificationCardTheme`、`WindowNotificationManagerTheme` 已迁移 |
| Phase 5 | progress bar 按需创建 | 普通 notification 不再创建隐藏 progress bar；显示进度时才 materialize |
| Phase 6 | pre-template `Show()` 队列 | manager 尚未 apply template 时先入 pending 队列，模板就绪后 flush |
| Phase 7 | 状态/生命周期验证与清理 | 新增 `--verify-notification-states`；删除无用 converter |

## 控件级结果

口径：Debug / Avalonia Headless / `--suite notification --count 500`。Before 来自同一仓库 HEAD 的临时 detached worktree，After 来自本轮优化后工作区。

| Scenario | Before | After | Improvement |
| --- | ---: | ---: | ---: |
| `NotificationCard.Information.NoMotion` | `1.972 ms/item`, `278.7 KB/item`, `18 visuals` | `1.698 ms/item`, `252.1 KB/item`, `17 visuals` | 快 `13.89%`，内存少 `9.54%`，少 `1` visual |
| `NotificationCard.Success.NoMotion` | `0.969 ms/item`, `268.8 KB/item`, `18 visuals` | `0.888 ms/item`, `242.8 KB/item`, `17 visuals` | 快 `8.36%`，内存少 `9.67%`，少 `1` visual |
| `NotificationCard.Warning.NoMotion` | `0.812 ms/item`, `267.9 KB/item`, `18 visuals` | `0.760 ms/item`, `242.1 KB/item`, `17 visuals` | 快 `6.40%`，内存少 `9.63%`，少 `1` visual |
| `NotificationCard.Error.NoMotion` | `0.740 ms/item`, `267.7 KB/item`, `18 visuals` | `0.658 ms/item`, `241.7 KB/item`, `17 visuals` | 快 `11.08%`，内存少 `9.71%`，少 `1` visual |
| `NotificationCard.Progress.NoMotion` | `0.751 ms/item`, `267.9 KB/item`, `18 visuals` | `0.734 ms/item`, `250.6 KB/item`, `18 visuals` | 快 `2.26%`，内存少 `6.46%` |
| `WindowNotificationManager.Empty.Closed` | `0.028 ms/item`, `17.4 KB/item`, `2 visuals` | `0.022 ms/item`, `14.7 KB/item`, `2 visuals` | 快 `21.43%`，内存少 `15.52%` |
| `WindowNotificationManager.Show.Single.NoMotion` | `1.557 ms/item`, `398.6 KB/item`, `21 visuals` | `0.854 ms/item`, `291.6 KB/item`, `20 visuals` | 快 `45.15%`，内存少 `26.84%`，少 `1` visual |
| `WindowNotificationManager.Show.MaxItems.NoMotion` | `22.160 ms/item`, `5322.1 KB/item`, `93 visuals` | `11.466 ms/item`, `3183.5 KB/item`, `88 visuals` | 快 `48.26%`，内存少 `40.18%`，少 `5` visuals |

新增补充场景：

| Scenario | After |
| --- | ---: |
| `WindowNotificationManager.Show.Single.Expiring.NoProgress.NoMotion` | `0.826 ms/item`, `292.5 KB/item`, `20 visuals` |
| `WindowNotificationManager.Show.Single.Progress.NoMotion` | `0.846 ms/item`, `301.0 KB/item`, `21 visuals` |

## Gallery 结果

口径：Debug / Avalonia Headless / `1300x900` / cold independent process samples `10` / warmup `15` / measured iterations `30` / timeout `15s`。这是本轮最终采用口径；warmup `5` 的 repeated 样本仍有明显热身阶梯，不作为最终结论。

| Metric | Before | After | Improvement |
| --- | ---: | ---: | ---: |
| `NotificationShowCase` cold mean | `176.82 ms` | `155.20 ms` | 快 `21.62 ms`，`12.23%` |
| `NotificationShowCase` cold median | `175.73 ms` | `154.76 ms` | 快 `20.97 ms`，`11.93%` |
| `NotificationShowCase` cold P95 | `183.53 ms` | `162.35 ms` | 快 `21.18 ms`，`11.54%` |
| `NotificationShowCase` cold alloc mean | `6482.23 KB` | `6173.89 KB` | 少 `308.34 KB`，`4.76%` |
| `NotificationShowCase` repeated mean | `37.46 ms` | `29.13 ms` | 快 `8.33 ms`，`22.24%` |
| `NotificationShowCase` repeated median | `34.31 ms` | `24.41 ms` | 快 `9.90 ms`，`28.85%` |
| `NotificationShowCase` repeated P95 | `58.60 ms` | `49.60 ms` | 快 `9.00 ms`，`15.36%` |
| `NotificationShowCase` repeated alloc mean | `5012.72 KB` | `4815.47 KB` | 少 `197.25 KB`，`3.94%` |
| Runtime shape | `200 visuals / 47 logical` | `200 visuals / 47 logical` | route 子树不变；收益来自 route 外 manager 懒创建和控件内部成本 |

用户可见效果：打开 `NotificationShowCase` 冷启动约少 `22ms`，重复打开约少 `8-10ms`；未点击 notification 按钮时，不再提前承担 7 个 manager 的窗口挂载和订阅成本。

## 生命周期验证

新增 `--verify-notification-states` 覆盖：

- `NotificationType` 和 `Position` 伪类切换只保留当前状态。
- 自定义 icon 不被默认 icon 覆盖，默认 icon 随 type 正确更新并复用。
- progress bar 在开启、关闭、重新开启、`Expiration=null` 时创建/释放正确。
- `Show()` 早于 template apply 时 pending queue 能 flush，dispose 后清空。
- timer 只在 active expiring card 存在时创建/启动，并在 dispose 后停止和退订。
- `MaxItems` 批量 show 能收敛到上限。
- `TopLevel=null` 的 manager 也能完整 dispose。

## 复现命令

```bash
dotnet build tools/performances/AtomUI.Performance/AtomUI.Performance.csproj --framework net10.0 --no-restore
dotnet run --project tools/performances/AtomUI.Performance/AtomUI.Performance.csproj --framework net10.0 --no-build -- --verify-notification-states
dotnet run --project tools/performances/AtomUI.Performance/AtomUI.Performance.csproj --framework net10.0 --no-build -- --suite notification --count 500
```

```bash
dotnet build tools/performances/AtomUI.GalleryPerformance/AtomUI.GalleryPerformance.csproj --framework net10.0 --no-restore
dotnet run --project tools/performances/AtomUI.GalleryPerformance/AtomUI.GalleryPerformance.csproj --framework net10.0 --no-build -- --showcase notification --cold-iterations 10 --warmup 15 --iterations 30 --timeout-ms 15000
```

`GalleryPerformance` 构建仍有既有 warning：`DataGridColumn._clipboardContentBinding` 未使用，和本轮 Notification 优化无关。
