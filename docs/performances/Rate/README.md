# Rate 性能优化记录

## 背景

`RateShowCase` 当前有 6 个 `ShowCaseItem`、9 个 `Rate`、45 个 `RateItem`。优化前每个 `RateItem` 都包含一个隐藏的焦点 `DashedBorder`，默认星形字符通过 `VisualBrush` 克隆 `StarFilled` icon 视觉，`AbstractRate` 还会在每个实例 attached 后订阅全局 `IInputManager.Process`。

本轮目标是降低默认不使用功能的成本，同时不改变选中、半星、清除、disabled、tooltip、键盘焦点和动画行为。

## 优化内容

- 移除 `AbstractRate` 的全局 `IInputManager.Process` 订阅，改为控件本地 pointer enter/move/press/release/capture-lost 处理。
- pointer press 使用控件本地 capture，并在 release、capture lost、detach 时释放或清空状态，避免订阅或捕获生命周期泄露。
- `RateItem` 默认 `StarFilled` 使用 `DrawingBrush + GeometryDrawing`，避免默认星形路径为每个 item 克隆隐藏 icon visual；自定义 icon/text 仍保留原 `VisualBrush` 兼容路径。
- 键盘焦点虚线框改到 `RateItem.Render()` 绘制，删除模板内每个 item 都创建的隐藏 `DashedBorder`。
- `Count` 变化时复用已有 `RateItem`，避免整组清空重建。
- 跳过未变化的 `SelectedState` 和 `EffectiveValue` 写入。
- 修复 `ToolTips` 变短或置空后旧 tooltip 残留的问题。
- 修复 `RateTheme.axaml` 中 `BorderThickness="{TemplateBinding BorderBrush}"` 的错误绑定。
- 修复 `RateCharacter.BackgroundProperty` owner 错误。

## 生命周期与泄露边界

- 本轮不再创建全局 input 订阅。
- pointer capture 有 release、capture-lost、detach 三条释放路径。
- 默认星形 `Geometry` 是静态只读绘制元数据；没有引入持有控件实例或用户 brush 的全局缓存。
- 曾评估过按 `IBrush` 做弱表缓存默认星形 brush，分配更低但 Gallery timing 变差且复杂度更高，已撤回。

## 控件级结果

命令：

```bash
dotnet run --project tools/performances/AtomUI.Performance/AtomUI.Performance.csproj --framework net10.0 --no-build -- --suite rate --count 300
```

| Scenario | Before | After | Improvement |
| --- | ---: | ---: | ---: |
| Rate.Default.Empty | 2.397ms/item, 428.8KB/item, visual 35, DashedBorder 5 | 1.967ms/item, 364.7KB/item, visual 30, DashedBorder 0 | 时间快 17.94%，分配少 14.95%，visual 少 14.29% |
| Rate.Half.Value3_5 | 1.217ms/item, 431.6KB/item, visual 35, DashedBorder 5 | 1.020ms/item, 360.2KB/item, visual 30, DashedBorder 0 | 时间快 16.19%，分配少 16.54%，visual 少 14.29% |
| Rate.ToolTips5 | 1.081ms/item, 417.8KB/item, visual 35, DashedBorder 5 | 0.827ms/item, 352.1KB/item, visual 30, DashedBorder 0 | 时间快 23.50%，分配少 15.72%，visual 少 14.29% |
| Rate.Batch20.Default | 36.610ms/item, 8820.2KB/item, visual 701, DashedBorder 100 | 26.334ms/item, 7428.0KB/item, visual 601, DashedBorder 0 | 时间快 28.07%，分配少 15.78%，visual 少 14.27% |
| Rate.GalleryShape | 15.846ms/item, 3921.6KB/item, visual 324, DashedBorder 45 | 10.354ms/item, 3367.3KB/item, visual 279, DashedBorder 0 | 时间快 34.66%，分配少 14.13%，visual 少 13.89% |

单个 `Rate` 的微基准 timing 会有首项抖动，所以判断重点放在批量、Gallery shape、visual count 和 allocation。结构指标稳定为每个 `Rate` 少 5 个隐藏 `DashedBorder`。

## RateShowCase 加载时间

同环境对比方式：

- Baseline：临时 worktree `/tmp/AtomUIV6-rate-baseline` 指向优化前 HEAD，仅补同一个 `rate` 测试 route。
- After：当前工作区。
- 口径：Debug、headless、1300x900、`--cold-iterations 10 --warmup 3 --iterations 20 --timeout-ms 20000`。

| Metric | Before | After | Improvement |
| --- | ---: | ---: | ---: |
| Cold first navigation mean | 154.19ms | 146.69ms | 快 4.86% |
| Cold first navigation median | 144.88ms | 142.80ms | 快 1.44% |
| Cold first navigation P95 | 194.16ms | 166.70ms | 快 14.14% |
| Repeated navigation mean | 62.75ms | 55.87ms | 快 10.96% |
| Repeated navigation median | 63.24ms | 58.01ms | 快 8.27% |
| Repeated navigation P95 | 84.04ms | 74.04ms | 快 11.90% |
| Repeated allocation mean | 5315.90KB | 4764.59KB | 少 10.37% |
| Runtime visuals | 376 | 331 | 少 45 个，约 11.97% |

结论：Rate 自身的结构成本明显下降，真实 `RateShowCase` 页面打开也有约 5%-11% 的加载时间收益。页面级收益小于控件级收益，因为 `ShowCasePanel`、`ShowCaseItem`、ReactiveUI activation、route/layout 稳定等固定成本仍然占比较高。

## 验证

```bash
dotnet build tools/performances/AtomUI.Performance/AtomUI.Performance.csproj --no-restore
dotnet run --project tools/performances/AtomUI.Performance/AtomUI.Performance.csproj --framework net10.0 --no-build -- --verify-rate-states
dotnet build tools/performances/AtomUI.GalleryPerformance/AtomUI.GalleryPerformance.csproj --no-restore
dotnet run --project tools/performances/AtomUI.GalleryPerformance/AtomUI.GalleryPerformance.csproj --framework net10.0 --no-build -- --showcase rate --label after-final --cold-iterations 10 --warmup 3 --iterations 20 --timeout-ms 20000
```

`--verify-rate-states` 覆盖点击选择、再次点击清除、半星、disabled、pointer 释放到控件外不提交、tooltip 变短/置空清理、Count 收缩/扩展复用、隐藏 `DashedBorder` 移除。
