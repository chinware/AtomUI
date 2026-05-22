# Breadcrumb 性能评估

> 路线图位置：[`../desktop-controls-optimization-roadmap.md`](../desktop-controls-optimization-roadmap.md) Tier 2 #5
> 状态：本轮建立基线并保留 separator 传播正确性修复；Gallery 同口径复测下 cold / P95 / alloc 有收益，repeated median 基本持平。

---

## 0. 结论

`Breadcrumb` 不适合作为隐藏视觉树按需化目标：Gallery 真实页面只有 6 个 `Breadcrumb` 示例、21 个 `BreadcrumbItem`，视觉树主体来自 `Button` / `TextBlock` / `IconPresenter` 模板。Theme Static Rule 下不移动这些模板节点到 C#。

同参数复测使用 `warmup=15`、`iterations=50` 后，原先 `warmup=5` 下的 repeated mean/median 回退没有复现。最终口径是：cold 与 P95 有明确收益，repeated mean 小幅收益，repeated median 基本持平，不计为收益。

本轮保留的运行时代码修复了一个已存在的 separator 传播问题：

- 旧实现用 `SetCurrentValue` 把父级 `Separator` 写入 item，导致 item 之后被视为已设置；父级 `Separator` 运行时变化不会继续同步到 direct/generated inherited items。
- 新实现将父级 separator 写到 `BindingPriority.Style` frame，item local/data separator 仍以 `LocalValue` 胜出。
- 只更新 `GetRealizedContainers()`，覆盖 `ItemsSource` 生成的 `BreadcrumbItem`，避免旧实现只扫 `Items` 数据项导致 generated containers 不更新。

相关 Avalonia 成本依据：

- `BindingPriority.LocalValue` 高于 `Style`：`.referenceprojects/Avalonia/src/Avalonia.Base/Data/BindingPriority.cs:9-50`
- realized container API：`.referenceprojects/Avalonia/src/Avalonia.Controls/ItemsControl.cs:304`

---

## 1. 控件级基线

命令：

```bash
dotnet run --project tools/performances/AtomUI.Performance/AtomUI.Performance.csproj \
  -c Debug --framework net10.0 --no-build -- \
  --suite breadcrumb --count 60 \
  --markdown /tmp/breadcrumb-current-default-skip.md
```

| Scenario | baseline ms/item | current ms/item | baseline KB/item | current KB/item | 结论 |
| --- | ---: | ---: | ---: | ---: | --- |
| `Breadcrumb.Basic.Items4` | 2.716 | 2.910 | 417.8 | 415.7 | timing 退化，alloc 小降 |
| `Breadcrumb.Icon.Items3` | 3.563 | 4.027 | 430.3 | 429.1 | timing 退化，alloc 小降 |
| `Breadcrumb.Navigate.Items2` | 1.103 | 1.786 | 210.9 | 210.0 | timing 退化，alloc 小降 |
| `Breadcrumb.CustomSeparator.Items4` | 2.229 | 2.727 | 411.2 | 412.4 | timing 退化 |
| `Breadcrumb.ItemSeparator.Items4` | 1.700 | 2.731 | 411.1 | 408.9 | timing 退化，alloc 小降 |
| `Breadcrumb.DataTemplate.Items4` | 1.399 | 2.357 | 415.7 | 412.2 | timing 退化，alloc 小降 |
| `Breadcrumb.GalleryShape` | 7.839 | 8.174 | 2362.8 | 2353.7 | timing 退化约 4.27%，alloc 小降 |

控件级结论：separator 正确性修复增加了 property frame 写入成本，创建路径未获得速度收益。

---

## 2. Gallery 真实场景

命令：

```bash
dotnet run --project tools/performances/AtomUI.GalleryPerformance/AtomUI.GalleryPerformance.csproj \
  -c Debug --framework net10.0 --no-build -- \
  --showcase breadcrumb --label current-rerun-w15-i50 \
  --cold-iterations 10 --iterations 50 --warmup 15 --timeout-ms 30000 \
  --markdown /tmp/breadcrumb-gallery-current-rerun-w15-i50.md
```

baseline 临时 worktree 只补 `--showcase breadcrumb` 测试入口，不包含 Breadcrumb 运行时代码改动。

| 指标 | baseline | current | 变化 | 结论 |
| --- | ---: | ---: | ---: | --- |
| Cold first navigation mean | 92.98 ms | 87.53 ms | +5.86% | 有效 |
| Cold first navigation median | 90.16 ms | 87.85 ms | +2.56% | 有效 |
| Cold first navigation P95 | 108.99 ms | 90.01 ms | +17.41% | 有效 |
| Cold alloc mean | 4124.48 KB | 4096.10 KB | +0.69% | 小幅有效 |
| Repeated navigation mean | 27.41 ms | 27.04 ms | +1.35% | 小幅有效 |
| Repeated navigation median | 25.16 ms | 25.35 ms | -0.76% | 基本持平 |
| Repeated navigation P95 | 36.72 ms | 34.00 ms | +7.41% | 有效 |
| Repeated alloc mean | 3707.87 KB | 3684.19 KB | +0.64% | 小幅有效 |
| Runtime visuals | 263 | 263 | 0 | 结构无变化 |
| Runtime logical | 41 | 41 | 0 | 结构无变化 |

Gallery 结论：页面级数据在严格 warmup 后整体小幅有效；`Repeated navigation median` 不写为收益。

---

## 3. 验证

```bash
dotnet build tools/performances/AtomUI.Performance/AtomUI.Performance.csproj -c Debug --framework net10.0

dotnet run --project tools/performances/AtomUI.Performance/AtomUI.Performance.csproj \
  -c Debug --framework net10.0 --no-build -- \
  --verify-breadcrumb-states
```

结果：

- `AtomUI.Performance` build passed，0 warning / 0 error。
- `Breadcrumb state verification passed.`
- baseline 旧运行时代码在同一 state verification 下失败：父级 `Separator` 变化后 direct/generated inherited items 仍停留在旧值。

---

## 4. 复杂度自评

| 指标 | 数值 |
| --- | --- |
| 新增 `Ensure*/Clear*/Sync*` 方法 | 0 |
| 新增 `_ignoreXxx` 标志位 | 0 |
| 新增 disposable / event / timer | 0 |
| axaml 节点搬到 C# 动态创建 | 否 |
| 生产文件范围 | 1 个文件 |

---

## 5. 改动文件清单

| 路径 | 改动 |
| --- | --- |
| `src/AtomUI.Desktop.Controls/Breadcrumb/Breadcrumb.cs` | `Separator` 默认值移入 property metadata；data separator 写 local value；父级 separator 用 style-priority 写入 realized item，保留 local/data 覆盖 |
| `tools/performances/AtomUI.Performance/Suites/Breadcrumb/*` | 新增控件级 scenarios、state verification、regression matrix |
| `tools/performances/AtomUI.GalleryPerformance/Program.cs` | 新增 `--showcase breadcrumb` 映射 |
