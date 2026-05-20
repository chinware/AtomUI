# ScrollViewer 性能优化记录

## 范围

- 控件：`AtomUI.Desktop.Controls.ScrollViewer`、`AtomUI.Desktop.Controls.ScrollBar`、`ScrollBarThumb`，以及 AtomUI 主题覆盖的 Avalonia `ScrollViewer`。
- Gallery：真实 `FloatButtonShowCase.axaml`，该页面包含 11 个 `atom:ScrollViewer`，是当前 Gallery 中 ScrollViewer 成本最集中的页面之一。
- 工具：`tools/performances/AtomUI.Performance --suite scrollviewer`，`tools/performances/AtomUI.GalleryPerformance --showcase floatbutton`。

## 主要瓶颈与风险

1. ScrollViewer 模板固定创建两个 scrollbar、两个 thumb 和 repeat button。即使 no-overflow 或禁用方向，也会承担基础模板成本；但这是 Avalonia `ScrollViewer` 模板契约和滚动状态更新路径的一部分，本轮没有直接移除。
2. `ScopeAwareOverlayLayerPanel` 是轻量 overlay host，真正的 `ScopeAwareOverlayLayer` 应该只在子控件请求 overlay 时创建。这里符合“不使用的功能不承担成本”，需要用验证锁住。
3. 主题 selector 原来错误地指向 `ScrollContentPresenter#PART_ContentPresenter` 或不存在的 `#ContentPresenter`。当前模板里真正参与 Grid span 的 direct child 是 `ScopeAwareOverlayLayerPanel`，旧 selector 对 overlay/auto-hide 布局没有生效。
4. `AbstractScrollBar.IsMotionEnabledProperty` 原来 `AddOwner<AbstractScrollViewer>()`，导致 ScrollViewer 到 ScrollBar/ScrollBarThumb 的 motion 状态同步存在错误 owner。这个是正确性问题，优先级高于性能。
5. transition 减重看起来有机会减少首帧成本，但会直接碰 UI/动画行为。实测结果混杂，且用户已经明确要求性能优化不能改变动画/视觉行为，所以本轮不落地。
6. ScrollViewer 是基础控件，改动 blast radius 很大。任何 lazy scrollbar、模板重排或 selector 迁移都必须同时覆盖 no-overflow、overflow、lite、auto-hide、标准 Avalonia ScrollViewer 主题和 Gallery 真实页面。

## 实施内容

| Phase | 内容 | 状态 |
| --- | --- | --- |
| Phase 0 | 拆出 `scrollviewer` 控件级 suite，覆盖 no-overflow、overflow、lite、auto-hide、disabled、batch 和 overlay request | Done |
| Phase 1 | 扫描泄露和生命周期风险；确认本轮不新增事件订阅、disposable binding、缓存或长生命周期对象 | Done |
| Phase 2 | 修复 `AbstractScrollBar.IsMotionEnabledProperty` owner，保证 motion 状态能同步到 ScrollBar/Thumb | Done |
| Phase 3 | 修复 Atom/Avalonia ScrollViewer 主题 selector，改为命中实际 Grid direct child `ScopeAwareOverlayLayerPanel` | Done |
| Phase 4 | 评估 transition/template 减重；因动画行为风险和重复跑分未证明稳定收益，已撤回不落地 | Evaluated |
| Phase 5 | 补齐 `--verify-scrollviewer-states`，覆盖 template part、auto-hide span、lite opacity、overlay layer lazy、motion binding | Done |
| Phase 6 | 用真实 `FloatButtonShowCase` 做 Gallery 加载时间对比 | Done |
| Phase 7 | 文档、工具索引、清理中间结果 | Done |

## 控件级结果

口径：Debug / Avalonia Headless / `--suite scrollviewer --count 220`，Before/After 各 10 轮汇总均值。After 为当前工作区，Before 为同一性能 suite 在本轮 ScrollViewer 修改前代码上的结果。

| Scenario | Before | After | Improvement |
| --- | ---: | ---: | ---: |
| `ScrollViewer.VerticalAuto.Overflow` | `2.140 ms/item`, `244.0 KB/item`, `18 visuals` | `1.975 ms/item`, `243.6 KB/item`, `18 visuals` | 快 `7.70%` |
| `ScrollViewer.LiteAutoHide.Overflow` | `1.231 ms/item`, `351.9 KB/item`, `27 visuals` | `1.128 ms/item`, `350.3 KB/item`, `27 visuals` | 快 `8.38%` |
| `ScrollViewer.Batch.PopupListLike` | `4.432 ms/item`, `1406.6 KB/item`, `109 visuals` | `4.112 ms/item`, `1399.8 KB/item`, `109 visuals` | 快 `7.22%` |
| `ScrollViewer.Batch.TextBoxLike` | `9.016 ms/item`, `2875.0 KB/item`, `217 visuals` | `8.991 ms/item`, `2861.2 KB/item`, `217 visuals` | 基本持平，快 `0.28%` |
| `ScrollViewer.BothAuto.Overflow` | `1.898 ms/item`, `356.8 KB/item`, `27 visuals` | `1.821 ms/item`, `355.2 KB/item`, `27 visuals` | 快 `4.04%` |
| `ScrollViewer.NormalNoAutoHide.GalleryPanel` | `1.768 ms/item`, `464.5 KB/item`, `41 visuals` | `1.786 ms/item`, `463.3 KB/item`, `41 visuals` | 基本持平，慢 `0.97%` |
| `ScrollViewer.BothDisabled` | `0.231 ms/item`, `132.1 KB/item`, `9 visuals` | `0.237 ms/item`, `132.7 KB/item`, `9 visuals` | 极小场景噪声，慢 `2.43%` |
| `ScrollViewer.HorizontalDisabled.VerticalAuto` | `0.576 ms/item`, `240.1 KB/item`, `18 visuals` | `0.602 ms/item`, `239.7 KB/item`, `18 visuals` | 极小场景噪声，慢 `4.48%` |
| `ScrollViewer.Default.NoOverflow` | `0.850 ms/item`, `134.9 KB/item`, `9 visuals` | `0.928 ms/item`, `135.4 KB/item`, `9 visuals` | 极小场景噪声，慢 `9.18%` |
| `ScrollViewer.OverlayScope.Requested` | `0.701 ms/item`, `257.4 KB/item`, `21 visuals` | `0.763 ms/item`, `255.6 KB/item`, `21 visuals` | 极小场景噪声，慢 `8.92%` |

结论：overflow 和批量场景有小幅改善，结构和分配基本不变；no-overflow 等绝对值低于 1ms 的场景波动较大。本轮不能声明 ScrollViewer 有明显整体性能提升，只能声明修复了两个低风险正确性问题，并补齐了后续优化的观测能力。

## Gallery ShowCase 加载时间

口径：真实 Gallery route `FloatButtonShowCase`，`warmup=3`，`iterations=15`，`cold-iterations=5`，Debug / headless / `1300x900`。选择该页面是因为源 XAML 有 `11` 个 `atom:ScrollViewer`。

| Metric | Before | After | Improvement |
| --- | ---: | ---: | ---: |
| `FloatButtonShowCase` cold first navigation mean | `325.55 ms` | `323.62 ms` | 快 `1.93 ms`，`0.59%` |
| `FloatButtonShowCase` cold first navigation median | `322.45 ms` | `327.95 ms` | 慢 `5.50 ms`，`1.71%` |
| `FloatButtonShowCase` cold first navigation P95 | `344.98 ms` | `330.21 ms` | 快 `14.77 ms`，`4.28%` |
| `FloatButtonShowCase` repeated mean | `127.97 ms` | `129.56 ms` | 慢 `1.59 ms`，`1.24%` |
| `FloatButtonShowCase` repeated median | `111.24 ms` | `108.08 ms` | 快 `3.16 ms`，`2.84%` |
| `FloatButtonShowCase` repeated P95 | `196.06 ms` | `208.89 ms` | 慢 `12.83 ms`，`6.54%` |
| Runtime visuals | `752` | `752` | 持平 |
| Runtime logical | `81` | `81` | 持平 |

用户可见结论：真实 Gallery 页面基本中性，没有可声明的页面级性能提升。这个结果符合本轮实际改动：selector 和 motion owner 修复不减少 visual tree，也不减少 ScrollViewer 模板固定节点。

## 正确性与泄露验证

新增并通过：

```bash
dotnet run --project tools/performances/AtomUI.Performance/AtomUI.Performance.csproj \
  --framework net10.0 --no-build -- --verify-scrollviewer-states
```

覆盖内容：

- Atom ScrollViewer 的 `PART_ContentPresenter`、水平/垂直 scrollbar、thumb 和 scoped overlay host 存在。
- `AllowAutoHide=True` 和 `IsLiteMode=True` 会把实际内容 host `ScopeAwareOverlayLayerPanel` span 到 scrollbar 下方。
- Avalonia `ScrollViewer` 主题的 auto-hide selector 同样命中实际内容 host。
- lite mode 初始 scrollbar opacity 为 `0`，normal mode 保持可见。
- 没有 overlay requester 时不创建 `ScopeAwareOverlayLayer`；子控件请求 overlay 后只创建一个 layer。
- `IsMotionEnabled` 从 ScrollViewer 同步到 ScrollBar 和 ScrollBarThumb，避免 motion owner 错误造成状态不同步。

本轮没有新增事件订阅、timer、manual disposable binding 或缓存。代码创建的 overlay layer 仍由现有 `ScopeAwareOverlayLayerPanel` 按请求管理，验证覆盖了默认不创建和请求后唯一创建。

## 后续不直接落地的方向

- 懒创建 scrollbar：理论上能让 no-overflow / disabled 场景少承担隐藏滚动条成本，但会碰 Avalonia ScrollViewer 模板契约、滚动范围更新、pointer/keyboard 滚动和 scrollbar visibility 切换，复杂度高，当前收益不足以承担风险。
- transition 细分：本轮试过减少默认 transition 绑定成本，但结果不稳定且可能改变动画边界。除非后续有明确动画一致性验证和无回退数据，否则不继续。
- selector 进一步收敛：可以给 host 增加稳定 `Name` 再用 `#PART_...` 精确匹配，但新增 Name 本身也有成本，当前只有一个 host，收益不明确。

## 复现命令

```bash
dotnet build tools/performances/AtomUI.Performance/AtomUI.Performance.csproj --framework net10.0 --no-restore
dotnet run --project tools/performances/AtomUI.Performance/AtomUI.Performance.csproj --framework net10.0 --no-build -- --verify-scrollviewer-states
dotnet run --project tools/performances/AtomUI.Performance/AtomUI.Performance.csproj --framework net10.0 --no-build -- --suite scrollviewer --count 220
```

```bash
dotnet build tools/performances/AtomUI.GalleryPerformance/AtomUI.GalleryPerformance.csproj --framework net10.0 --no-restore
dotnet run --project tools/performances/AtomUI.GalleryPerformance/AtomUI.GalleryPerformance.csproj --framework net10.0 --no-build -- --showcase floatbutton --warmup 3 --iterations 15 --cold-iterations 5 --timeout-ms 20000
```
