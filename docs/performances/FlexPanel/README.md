# FlexPanel 性能评估

> 路线图位置：[`../README.md`](../README.md) Layout / FlexPanel
> 状态：已建立控件级基线和状态校验；本轮生产优化候选因 timing 回归已撤回，未保留运行时代码改动。

---

## 0. 结论

本轮只保留 `AtomUI.Performance` 的 FlexPanel 独立场景和状态校验。生产代码恢复到 baseline 状态，不声明运行时速度提升。

| 指标 | baseline | optimized | formula | improvement | 结论 |
| --- | ---: | ---: | --- | ---: | --- |
| 控件级性能场景覆盖 | 0 | 5 | `(5 - 0) / 5` | 100.00% | 新增 Row / Wrap / Ordered / GrowShrink / ColumnWrap 五类场景 |
| 状态正确性校验覆盖 | 0 | 4 | `(4 - 0) / 4` | 100.00% | 新增 order 稳定、隐藏子项、wrap、grow 分配校验 |
| 保留的生产运行时代码改动 | 0 | 0 | `0 - 0` | 0.00% | 候选有回归，已撤回，避免把风险带入产品 |
| 可证明页面级速度提升 | 无 | 无 | n/a | 不声明 | 当前没有 Gallery 级 before/after 证据 |

---

## 1. 新增基准场景

`tools/performances/AtomUI.Performance` 新增 `flexpanel` suite：

| Scenario | 覆盖点 |
| --- | --- |
| `FlexPanel.Row.Border8` | 单行 Row、spacing、stretch |
| `FlexPanel.Wrap.Border24` | Row wrap 多行 |
| `FlexPanel.Ordered.Border24` | `Flex.Order` 排序与稳定顺序 |
| `FlexPanel.GrowShrink.Border18` | grow / shrink / basis 分配 |
| `FlexPanel.ColumnWrap.Border24` | Column + wrap |

新增参数：

```bash
--suite flexpanel
--verify-flexpanel-states
```

---

## 2. 状态校验

新增 `RunFlexPanelStateVerification()`，覆盖：

| 校验 | 目标 |
| --- | --- |
| order 稳定排序 | `Flex.Order` 小的子项先排，同 order 保持 source order |
| 隐藏子项跳过 | `IsVisible=false` 子项不占主轴空间 |
| wrap 换行 | 超出主轴尺寸时进入下一行 |
| grow 分配 | grow factor 更大的子项获得更大宽度 |

这些校验不是性能收益，但能防止后续优化破坏布局语义。

---

## 3. 已撤回候选

候选方向：尝试减少 `Where/OrderBy/ToArray`、line item `ToArray()`、LINQ `Sum/Max`、flex item 临时列表等分配。

状态校验通过，但单轮控件级 smoke 数据显示多个主耗时场景明显回归，allocation 降幅很小。按性能规范“无 measurable speed regression”和“3-round budget”，不保留这批生产代码。

| 指标 | baseline | candidate | formula | improvement | 结论 |
| --- | ---: | ---: | --- | ---: | --- |
| `FlexPanel.Row.Border8` KB/item | 55.0 KB | 54.2 KB | `(55.0 - 54.2) / 55.0` | 1.45% | 分配小幅下降 |
| `FlexPanel.Row.Border8` ms/item | 0.276 ms | 0.274 ms | `(0.276 - 0.274) / 0.276` | 0.72% | smoke-only，不作为速度收益证明 |
| `FlexPanel.Wrap.Border24` KB/item | 157.9 KB | 156.3 KB | `(157.9 - 156.3) / 157.9` | 1.01% | 分配小幅下降 |
| `FlexPanel.Wrap.Border24` ms/item | 0.882 ms | 1.129 ms | `(0.882 - 1.129) / 0.882` | -28.00% | smoke-only，主耗时回归，候选撤回 |
| `FlexPanel.GrowShrink.Border18` KB/item | 137.8 KB | 134.5 KB | `(137.8 - 134.5) / 137.8` | 2.39% | 分配小幅下降 |
| `FlexPanel.GrowShrink.Border18` ms/item | 0.359 ms | 0.467 ms | `(0.359 - 0.467) / 0.359` | -30.08% | smoke-only，主耗时回归，候选撤回 |
| `FlexPanel.ColumnWrap.Border24` KB/item | 150.9 KB | 149.7 KB | `(150.9 - 149.7) / 150.9` | 0.80% | 分配小幅下降 |
| `FlexPanel.ColumnWrap.Border24` ms/item | 0.398 ms | 0.436 ms | `(0.398 - 0.436) / 0.398` | -9.55% | smoke-only，主耗时回归，候选撤回 |

---

## 4. 复现命令

```bash
dotnet build -c Release -f net10.0 --no-restore tools/performances/AtomUI.Performance/AtomUI.Performance.csproj
dotnet run -c Release -f net10.0 --no-build --project tools/performances/AtomUI.Performance/AtomUI.Performance.csproj -- --verify-flexpanel-states
dotnet run -c Release -f net10.0 --no-build --project tools/performances/AtomUI.Performance/AtomUI.Performance.csproj -- --suite flexpanel --count 80
```

---

## 5. 后续

FlexPanel 当前标记为 baseline only。下一轮只有在定位到更明确的真实高频路径时再进入生产代码优化，例如具体 Gallery 页面中的 FlexPanel 大量实例化、动态 children 变更、或频繁 layout invalidation。
