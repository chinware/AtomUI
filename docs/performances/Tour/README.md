# Tour 性能优化

> 路线图位置：[`../desktop-controls-optimization-roadmap.md`](../desktop-controls-optimization-roadmap.md) Tier 1 #6
> 状态：本轮完成绑定结构收敛，并补齐 Gallery baseline。

---

## 0. 结论

`TourShowCase` 的 closed route 本身较轻：7 个 Tour 不会把 popup 内容挂进页面视觉树，页面稳定形态为 `349` visuals / `82` logical。真正高频成本更偏首次打开 Tour；当前 GalleryPerformance 只覆盖页面导航，因此本轮不做主题动态创建或 popup 内容重构。

保留的运行时代码改动只收敛 `TourStepsView.PrepareContainerForItemOverride` 中 7 条同生命周期 binding：

- 旧实现：`BindUtils.RelayBind(...)` 创建完整 `Binding + Path`，并且在 container prepare 路径不保存 returned disposable。
- 新实现：用 Avalonia indexer direct binding，左侧 `~Property` 保持 `BindingPriority.Template`，右侧 `[!Property]` 作为源 observable。
- 保留 per-step local override 语义，例如 `TourStep Placement="Right"`、`MaskColor`、`IsShowMask="False"` 仍高于 Template priority。

Avalonia 依据：

- `.referenceprojects/Avalonia/src/Avalonia.Base/AvaloniaProperty.cs:172-191`：`!Property` 是 local binding descriptor，`~Property` 是 template binding descriptor。
- `.referenceprojects/Avalonia/src/Avalonia.Base/AvaloniaObjectExtensions.cs:188-204` 与 `PropertyStore/DirectBindingObserver.cs:7-84`：direct observable binding 是轻量 observer 路径。

| 指标 | baseline | template direct binding | 改善 |
| --- | ---: | ---: | ---: |
| Cold first navigation mean | 265.80 ms | 262.72 ms | 1.16% |
| Cold first navigation median | 262.14 ms | 260.73 ms | 0.54% |
| Cold first navigation P95 | 287.59 ms | 274.70 ms | 4.48% |
| Cold alloc mean | 14209.09 KB | 14203.77 KB | 0.04% |
| Repeated navigation mean | 65.81 ms | 61.91 ms | 5.93% |
| Repeated navigation median | 56.83 ms | 54.70 ms | 3.75% |
| Repeated navigation P95 | 104.22 ms | 102.82 ms | 1.34% |
| Repeated alloc mean | 12525.50 KB | 12526.14 KB | -0.01% |
| Runtime visuals | 349 | 349 | 0 |
| Runtime logical | 82 | 82 | 0 |

页面级 timing 有小幅改善，但应按结构收敛看待，不按大收益优化宣传。

---

## 1. 改动

`src/AtomUI.Desktop.Controls/Tour/TourStepsView.cs`

原实现：

```csharp
BindUtils.RelayBind(this, StyleTypeProperty, tourStep, TourStep.StyleTypeProperty, priority: BindingPriority.Template);
```

新实现：

```csharp
tourStep[~TourStep.StyleTypeProperty] = this[!StyleTypeProperty];
```

同样替换的属性：

- `StyleType`
- `IsArrowVisible`
- `IsPointAtCenter`
- `Placement`
- `IsShowMask`
- `IsScrollIntoView`
- `MaskColor`

移除 `AtomUI.Data` / `Avalonia.Data` using，因为不再直接使用 `BindUtils` / `BindingPriority`。

---

## 2. 复现命令

```bash
dotnet run --project tools/performances/AtomUI.GalleryPerformance/AtomUI.GalleryPerformance.csproj \
  -c Debug --framework net10.0 --no-build -- \
  --showcase tour --label baseline \
  --cold-iterations 10 --iterations 30 --warmup 5 --timeout-ms 30000 \
  --markdown /tmp/tour-showcase-baseline.md
```

```bash
dotnet run --project tools/performances/AtomUI.GalleryPerformance/AtomUI.GalleryPerformance.csproj \
  -c Debug --framework net10.0 --no-build -- \
  --showcase tour --label template-direct-binding \
  --cold-iterations 10 --iterations 30 --warmup 5 --timeout-ms 30000 \
  --markdown /tmp/tour-showcase-template-direct-binding.md
```

---

## 3. 复杂度自评

| 指标 | 数值 |
| --- | --- |
| 新增 `Ensure*/Clear*/Sync*` 方法 | 0 |
| 新增 `_ignoreXxx` 标志位 | 0 |
| 新增 disposable / event / timer | 0 |
| axaml 节点搬到 C# 动态创建 | 否 |
| 生产文件范围 | 1 个文件 |

---

## 4. 后续

如需继续优化 Tour，下一步应补一个 Gallery interaction harness：导航到 `TourShowCase` 后触发第一个 `Begin Tour`，测首次打开、下一步切换、关闭。仅用页面导航数据不足以评估 popup 内容和 `TourLayer` 热路径。

---

## 5. 追加结构优化：CustomActions 快照

`Tour.HandleCustomActionsChanged()` 和 `OnApplyTemplate()` 过去各自执行 `CustomActions.Cast<Control>().ToList()`，并重复一段 `ITourAction.NotifyAttached()` 逻辑。现在统一走 `PrepareCustomActions()`，用 `Control[]` 快照并复用 attach 通知逻辑。

| 指标 | 优化前 | 优化后 | 公式 | 提升 | 结论 |
| --- | ---: | ---: | --- | ---: | --- |
| CustomActions LINQ iterator / sync | 1 | 0 | `(1 - 0) / 1` | 100.00% | 同步自定义按钮时不再创建 `Cast` iterator |
| CustomActions list wrapper / sync | 1 list | 0 list | `(1 - 0) / 1` | 100.00% | 改为数组快照，避免 List 增长逻辑和 LINQ materialize |
| 重复 attach 同步代码块 | 2 处 | 1 处 | `(2 - 1) / 2` | 50.00% | 降低后续行为漂移风险 |

说明：这是 popup/template 同步路径的结构性收益；没有页面加载 timing 百分比声明。
