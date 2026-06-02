# SplitView 性能优化

> 状态：本轮完成结构性 transition 延迟创建；无独立 Gallery ShowCase，页面级耗时不声明。

---

## 0. 结论

本轮继续收敛 `SplitView` 的 pane transition 生命周期：初始 closed/open 状态不再创建 pane transition，第一次运行时打开或关闭时才创建对应方向的 transition。

`SplitView` 当前没有独立 Gallery showcase 映射，因此主验证口径是控件级 suite 和 `--verify-splitview-states`。耗时数据在本机负载较高时波动明显，不作为主收益；稳定收益是初始 transition 对象不再 materialize，closed/open 初始路径分配约下降 `~1 KB/item`。

| 指标 | 旧实现 | 新实现 | 变化 | 结论 |
| --- | ---: | ---: | ---: | --- |
| Loaded closed 初始 `PaneOpenTransitions` | 0 | 0 | 0 | 不需要 open transition |
| Loaded closed 初始 `PaneCloseTransitions` | 1 | 0 | -1 | 主收益 |
| Loaded initially-open 初始 `PaneOpenTransitions` | 1 | 0 | -1 | 主收益 |
| Loaded initially-open 初始 `PaneCloseTransitions` | 0 | 0 | 0 | 不需要 close transition |
| Runtime first open | creates Width/Height open transition | creates Width/Height open transition | 0 | 行为保留 |
| Runtime first close | creates Width/Height close transition | creates Width/Height close transition | 0 | 行为保留 |

---

## 1. 根因

上一轮 SplitView 已经把 open/close transition 从默认双创建收敛到按 pane state 创建。但 `OnInitialized()` 仍会调用：

```csharp
EnsureTransitionsForPaneState(IsPaneOpen);
```

这意味着：

- 初始 closed：加载时会创建 `PaneCloseTransitions`，但用户还没有发生 close 操作；
- 初始 open：加载时会创建 `PaneOpenTransitions`，但初始 open 不应动画，真正需要的是后续 close；
- `this.DisableTransitions()` 已经防止初始动画，初始 transition 对象本身没有必要存在。

`CreatePaneTransitions(...)` 每次会构造一个 `Transitions` 集合和一个 `DoubleTransition`，并通过 `TransitionUtils.CreateTransition<DoubleTransition>(...)` 设置 target property、duration、easing。对于只作为初始布局显示的 SplitView，这批对象可以完全延迟到第一次运行时切换。

---

## 2. 改动

### 2.1 生产代码

`src/AtomUI.Desktop.Controls/SplitView/SplitView.cs`

```csharp
protected override void OnInitialized()
{
    base.OnInitialized();
    this.DisableTransitions();
}
```

保留项：

- `OnPropertyChanged(IsPaneOpen)` 仍在 `base.OnPropertyChanged(change)` 前调用 `EnsureTransitionsForPaneState(...)`，保证 runtime open/close 前 transition 已准备好；
- `IsMotionEnabled=false` 仍清空 transitions；
- `PanePlacement`、duration、easing 变化仍刷新已 materialized 的 transitions；
- 主题模板和 pane/content/light-dismiss 视觉树不变。

### 2.2 状态验证

`--verify-splitview-states` 增加/调整断言：

- constructed closed：open/close transitions 都为 `null`；
- loaded closed：open/close transitions 仍都为 `null`；
- first open：创建 Width transition；
- first close：创建 Width close transition；
- loaded initially-open：open/close transitions 都为 `null`；
- initially-open 后 first close：创建 Width close transition。

---

## 3. 控件级结果

命令：

```bash
dotnet run -c Release -f net10.0 --no-build \
  --project tools/performances/AtomUI.Performance/AtomUI.Performance.csproj -- \
  --suite splitview --count 80 \
  --markdown /tmp/splitview-after.md
```

| Scenario | KB/item baseline | KB/item optimized | 变化 | Visual | Logical |
| --- | ---: | ---: | ---: | ---: | ---: |
| `SplitView.Closed.Overlay.Left` | 163.7 | 162.6 | +0.67% | 15 | 8 |
| `SplitView.Open.Overlay.Left` | 167.1 | 166.0 | +0.66% | 15 | 8 |
| `SplitView.Closed.CompactInline.Left` | 165.2 | 164.2 | +0.61% | 15 | 8 |
| `SplitView.Open.Inline.Left` | 163.5 | 162.5 | +0.61% | 15 | 8 |
| `SplitView.Open.CompactOverlay.Right` | 166.3 | 165.2 | +0.66% | 15 | 8 |
| `SplitView.Open.Overlay.Top` | 165.6 | 164.6 | +0.60% | 15 | 8 |
| `SplitView.Open.Overlay.Bottom` | 165.9 | 165.0 | +0.54% | 15 | 8 |
| `SplitView.Batch.ClosedOverlay8` | 1377.7 | 1369.0 | +0.63% | 121 | 65 |

说明：

- `MotionDisabled.Open` 不作为主对比；旧实现在 `IsMotionEnabled=false` 下本来就不会保留 transitions。
- visual/logical 不变，符合本轮只延迟 transition 对象创建的预期。
- ms/item 在两次 after run 中波动很大，当前环境不能用耗时证明收益或回归。

---

## 4. 正确性验证

命令：

```bash
dotnet build -c Release -f net10.0 tools/performances/AtomUI.Performance/AtomUI.Performance.csproj
dotnet run -c Release -f net10.0 --no-build \
  --project tools/performances/AtomUI.Performance/AtomUI.Performance.csproj -- \
  --verify-splitview-states
```

结果：

| 验证 | 结果 |
| --- | --- |
| Release build | passed, 0 warnings, 0 errors |
| `--verify-splitview-states` | passed |
| constructed/loaded initial transition state | passed |
| runtime open/close transition axis | passed |
| motion disabled transition clearing | passed |
| event order / template settings / placement parts | passed |

---

## 5. 复杂度自评

| 指标 | 数值 |
| --- | --- |
| 生产文件改动 | 删除 1 行 |
| 新增 `Ensure*/Clear*/Sync*` 方法 | 0 |
| 新增 `_ignoreXxx` 标志位 | 0 |
| 新增 disposable / event / timer | 0 |
| axaml 节点搬到 C# 动态创建 | 否 |
| 正确性测试 | 调整并扩展 deferred transition state verification |

本轮是低风险生命周期收敛：不改变视觉树、不改变模板、不改变 runtime open/close 动画，只把初始布局不需要的 transition 对象延迟到实际状态切换。
