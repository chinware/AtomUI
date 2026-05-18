# Notification 性能优化记录

## 范围

- 控件：`Notification`、`NotificationCard`、`NotificationProgressBar`、`WindowNotificationManager`。
- Gallery：真实 `NotificationShowCase.axaml`，源码形态为 `ShowCaseItem=6`、`Button=14`、`OptionButtonGroup=1`。
- 工具：`tools/performances/AtomUI.Performance --suite notification`，`tools/performances/AtomUI.GalleryPerformance --showcase notification`。

## 当前风险与瓶颈

1. `NotificationShowCase.OnAttachedToVisualTree()` 默认创建 7 个 `WindowNotificationManager`。用户不点击任何按钮时，也会提前安装 7 个 adorner、订阅 7 组窗口 safe-area observable、创建 14 个 `DispatcherTimer`。
2. `WindowNotificationManager.Dispose()` 在 `_topLevel is null` 时直接返回；控件级、测试 host、未来非 TopLevel 挂载路径下，timer Tick、队列和卡片事件存在无法释放的风险。
3. `NotificationCard` 关闭后，manager 只移除 visual，未清 `PointerPressed`、`NotificationClosed`、`OnClick`、`OnClose`；如果 timer/cleanup 队列或外部闭包仍持有 card，会延长对象生命周期。
4. `NotificationCard` 默认 icon 只在 `Icon is null` 时创建，但没有区分“生成的默认 icon”和“调用方自定义 icon”；动态切换 type 时可能无法正确同步默认 icon。
5. `NotificationCard` 的 `NotificationType` 伪类只 Add 不清旧状态；`Position` 和 manager position 每次全量 Set 6 个伪类。
6. `NotificationCardTheme.axaml` 和 `WindowNotificationManagerTheme.axaml` 仍使用 `[NotificationType=...]`、`[Position=...]` 属性 selector；这会让每个状态变化承担属性 selector 成本。
7. `NotificationProgressBar` 默认存在于模板中，普通不显示进度的 notification 也会创建 progress bar、MultiBinding 和 converter。

## Phase 计划

| Phase | 内容 | 目标 |
| --- | --- | --- |
| Phase 0 | 建立 `notification` 控件级 suite 和真实 `NotificationShowCase` 加载基线 | 先量化，不凭感觉优化 |
| Phase 1 | 修复 manager/card 生命周期：timer、队列、事件、回调、`TopLevel=null` dispose | 泄露风险优先清零 |
| Phase 2 | Gallery manager 按需创建，7 个 manager 只在对应 placement 首次 show 时创建 | 未点击功能不承担 adorner/timer/subscription 成本 |
| Phase 3 | `NotificationCard` 默认 icon、type/position 伪类改为前后状态切换 | 保持视觉，降低状态计算和错误状态风险 |
| Phase 4 | 评估属性 selector 切伪类 selector | 保留样式表达，同时减少热路径 selector 成本 |
| Phase 5 | `NotificationProgressBar` 按需创建或移出默认模板 | 普通 notification 不承担 progress 成本 |
| Phase 6 | `Show()` 早于模板应用的 pending 队列处理 | 避免按需 manager 后第一条 notification 丢失 |
| Phase 7 | 状态/生命周期验证、Gallery 实测、清理文档 | 验证功能、无泄露、无性能回退 |

## Phase 0 基线

已运行口径：Debug / Avalonia Headless / `1300x900`，Gallery 使用 `--cold-iterations 5 --warmup 3 --iterations 20 --timeout-ms 15000`。

### Gallery 加载时间

| Metric | Baseline |
| --- | ---: |
| `NotificationShowCase` cold first navigation mean | `162.56 ms` |
| `NotificationShowCase` cold first navigation median | `162.50 ms` |
| `NotificationShowCase` cold first navigation P95 | `166.52 ms` |
| `NotificationShowCase` cold alloc mean | `6449.99 KB` |
| `NotificationShowCase` repeated mean | `58.21 ms` |
| `NotificationShowCase` repeated median | `58.80 ms` |
| `NotificationShowCase` repeated P95 | `66.90 ms` |
| `NotificationShowCase` repeated alloc mean | `5008.67 KB` |
| Runtime visual/logical shape | `200 visuals / 47 logical` |

页面源码 shape：`ShowCaseItem=6`、`Button=14`。当前 Gallery 统计只覆盖 route 子树，`WindowNotificationManager` 被添加到 `AdornerLayer`，其提前创建成本主要体现在 allocation 和 navigation timing，不直接反映在 route visual count 中。

### 控件级基线

| Scenario | ms/item | KB/item | Visual/root |
| --- | ---: | ---: | ---: |
| `NotificationCard.Information.NoMotion` | `2.007` | `269.8` | `18` |
| `NotificationCard.Success.NoMotion` | `2.436` | `267.0` | `18` |
| `NotificationCard.Warning.NoMotion` | `1.706` | `266.5` | `18` |
| `NotificationCard.Error.NoMotion` | `1.332` | `266.9` | `18` |
| `NotificationCard.Progress.NoMotion` | `1.068` | `266.5` | `18` |
| `WindowNotificationManager.Empty.Closed` | `0.037` | `17.8` | `2` |
| `WindowNotificationManager.Show.Single.NoMotion` | `1.692` | `339.8` | `21` |
| `WindowNotificationManager.Show.MaxItems.NoMotion` | `18.125` | `4018.5` | `219` |

`Show.MaxItems.NoMotion` 使用 12 条 notification、`MaxItems=5`。当前结果仍保留 `219` 个 visual，说明批量 show 后没有在测量稳定点收敛到 5 条可见卡片；这需要在 Phase 1/6 作为正确性与生命周期问题优先处理，不能只当性能问题。

## 复现命令

```bash
dotnet build tools/performances/AtomUI.Performance/AtomUI.Performance.csproj --framework net10.0 --no-restore
dotnet run --project tools/performances/AtomUI.Performance/AtomUI.Performance.csproj --framework net10.0 --no-build -- --suite notification --count 80
```

```bash
dotnet build tools/performances/AtomUI.GalleryPerformance/AtomUI.GalleryPerformance.csproj --framework net10.0 --no-restore
dotnet run --project tools/performances/AtomUI.GalleryPerformance/AtomUI.GalleryPerformance.csproj --framework net10.0 --no-build -- --showcase notification --cold-iterations 5 --warmup 3 --iterations 20 --timeout-ms 15000
```

`GalleryPerformance` 构建仍有既有 warning：`DataGridColumn._clipboardContentBinding` 未使用，和本轮 Notification Phase 0 无关。
