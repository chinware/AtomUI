# AdornerLayer 性能优化

> 状态：本轮完成 `ScopeAwareAdornerLayer` attach/detach 结构与生命周期优化；不声明页面级加载耗时提升。

---

## 0. 结论

本轮没有改 Avalonia 内置 `AdornerLayer` 的主题，也没有新增动态模板元素；优化点在 AtomUI 的 `ScopeAwareAdornerLayer` 运行期路径。`Drawer`、`Watermark` 等 scope-aware adorner 挂载 / 卸载时，不再用 `layer.Children.Contains(adorner)` 做线性成员扫描，而是直接看 adorner 当前 visual parent；同时在卸载时清掉 adorner 上的 adorned target attached property，避免 detach 后继续保留目标引用。

| 指标 | baseline | optimized | 计算 | 改善 | 结论 |
| --- | ---: | ---: | --- | ---: | --- |
| Scope-aware adorner attach membership scan / attach | 1 | 0 | `(1 - 0) / 1` | 100.00% removed | 主收益 |
| Scope-aware adorner detach membership scan / detach | 1 | 0 | `(1 - 0) / 1` | 100.00% removed | 主收益 |
| Detached adorner retained target refs / detach | 1 | 0 | `(1 - 0) / 1` | 100.00% removed | 正确性 + 生命周期收益 |
| `AdornerAttach8` attach membership scans / realization | 8 | 0 | `(8 - 0) / 8` | 100.00% removed | 主收益 |
| `AdornerLayer.ScopeAware.DrawerOpen` ms/item | 3.145 ms | 2.523 ms | `(3.145 - 2.523) / 3.145` | 19.78% faster | smoke-only，不作为收益证明 |

---

## 1. 资格门槛

`AdornerLayer` 在路线图里是 Tier 2 #9，备注为“与 Drawer/Dialog/Tooltip overlay 协同”。它自身不是大量实例化控件，单独页面基线意义有限；但 `ScopeAwareAdornerLayer` 是 `Drawer` 和 `Watermark` 的运行期挂载层，open/close、show/hide 属于用户操作路径。

本轮只处理可局部证明的 attach/detach 成本和引用释放，不把单次 headless timing 当成页面收益。

---

## 2. 根因

`ScopeAwareAdornerLayer.AddVisualAdorner()` 和 `RemoveVisualAdorner()` 原来都用：

```csharp
layer.Children.Contains(adorner)
```

这个检查发生在每次 adorner attach/detach 上；对于同一 layer 下多个 adorner，membership 判断会扫描 children。更直接的状态源其实是 adorner 的当前 visual parent，Avalonia `VisualExtensions.GetVisualParent()` 直接返回当前 visual parent（`.referenceprojects/Avalonia/src/Avalonia.Base/VisualTree/VisualExtensions.cs:435-437`）。

另外，`RemoveVisualAdorner()` 删除 visual child 和 logical parent 后，没有清掉 adorner 自身的 `AdornedElement` attached property。这样 adorner 对象如果被复用或被外部字段继续持有，会继续保留旧 target 引用。状态验证已覆盖显式 detach 与 target visual detach 两条路径。

---

## 3. 改动

### 3.1 attach/detach membership 判断

`AddVisualAdorner()`：

- 旧：`layer.Children.Contains(adorner)`
- 新：`ReferenceEquals(adorner.GetVisualParent(), layer)`

`RemoveVisualAdorner()`：

- 旧：`!layer.Children.Contains(adorner)`
- 新：`!ReferenceEquals(adorner.GetVisualParent(), layer)`

### 3.2 detach 时释放 adorned target 引用

卸载 adorner 时，如果 `ScopeAwareAdornerLayer.GetAdornedElement(adorner)` 仍指向当前 visual，则先清空：

```csharp
SetAdornedElement(adorner, null);
```

随后再从 layer 移除并清 logical parent。这样 `HandleAdornedElementChanged()` 可以在 adorner 仍挂在 layer 下时释放 `AdornedElementInfo` subscription。

---

## 4. 实测结果

命令：

```bash
dotnet run -c Release -f net10.0 --no-build \
  --project tools/performances/AtomUI.Performance/AtomUI.Performance.csproj -- \
  --suite adornerlayer --count 80
```

| Scenario | ms/item baseline | ms/item optimized | KB/item baseline | KB/item optimized | Visual baseline | Visual optimized | 结论 |
| --- | ---: | ---: | ---: | ---: | ---: | ---: | --- |
| `AdornerLayer.ScopeAware.LayerLookup16` | 0.117 | 0.113 | 20.1 | 20.1 | 4 | 4 | timing smoke-only |
| `AdornerLayer.ScopeAware.AdornerAttach8` | 0.805 | 0.797 | 139.1 | 139.1 | 19 | 19 | timing smoke-only |
| `AdornerLayer.ScopeAware.DrawerOpen` | 3.145 | 2.523 | 381.9 | 381.9 | 30 | 30 | timing smoke-only |

结构收益不改变 visual/logical/KB；它减少的是 attach/detach 路径里的 membership scan 和 detach 后引用保留。

---

## 5. 正确性验证

命令：

```bash
dotnet build -c Release -f net10.0 --no-restore \
  tools/performances/AtomUI.Performance/AtomUI.Performance.csproj

dotnet run -c Release -f net10.0 --no-build \
  --project tools/performances/AtomUI.Performance/AtomUI.Performance.csproj -- \
  --verify-adornerlayer-states

dotnet run -c Release -f net10.0 --no-build \
  --project tools/performances/AtomUI.Performance/AtomUI.Performance.csproj -- \
  --verify-drawer-states
```

结果：

| 验证 | 结果 |
| --- | --- |
| Release build | passed, 0 warnings, 0 errors |
| `--verify-adornerlayer-states` | passed |
| explicit detach | adorner visual parent 清空，`AdornedElement` 清空 |
| visual detach | target 从 visual tree 移除时 adorner 同步卸载并清 target 引用 |
| reattach | target 回到 visual tree 后 adorner 可重新挂载 |
| `--verify-drawer-states` | passed |

---

## 6. 复杂度自评

| 指标 | 数值 |
| --- | --- |
| 新增 `Ensure*/Clear*` 方法 | 0 |
| 新增 `_ignoreXxx` / suppression flag | 0 |
| 新增 disposable 字段 | 0 |
| 新增 timer / global subscription | 0 |
| axaml 节点搬到 C# 动态创建 | 否 |
| 生产文件范围 | `ScopeAwareAdornerLayer.cs` |

结论：本轮是 attach/detach 路径收敛和引用释放修复，复杂度低，行为由状态验证覆盖。

---

## 7. 追加结构优化：Layer 查找路径去 LINQ

本轮补充优化 `ScopeAwareAdornerLayer.GetLayer()` 的 layer 查找路径。`TopLevel` fallback 和已有 layer 查找保持 first-match 语义，但不再通过 `OfType().FirstOrDefault()` / `FirstOrDefault(predicate)` 构造 LINQ 链；首次注入时也不再在 `InjectLayer()` 里重复执行一次同类查找。

| 指标 | 优化前 | 优化后 | 公式 | 提升 | 结论 |
| --- | ---: | ---: | --- | ---: | --- |
| TopLevel fallback manager lookup LINQ operators / lookup | 2 operators | 0 operators | `(2 - 0) / 2` | 100.00% | 结构收益；查找 `VisualLayerManager` 时改为显式 first-match |
| existing adorner layer lookup LINQ predicate / lookup | 1 predicate callsite | 0 predicate callsites | `(1 - 0) / 1` | 100.00% | 结构收益；复用 `FindAdornerLayer()` 的显式扫描 |
| first inject adorner-layer scans / lookup miss | 2 scans | 1 scan | `(2 - 1) / 2` | 50.00% | 结构收益；`GetLayer()` 已查过，私有 `InjectLayer()` 不再复查 |

说明：这是 `Drawer` / `Watermark` 等 scope-aware adorner 首次挂载路径的结构性收益；未新增页面 timing 对比，不声明页面加载速度提升。
