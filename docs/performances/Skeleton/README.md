# Skeleton 性能优化

> 路线图位置：[`../desktop-controls-optimization-roadmap.md`](../desktop-controls-optimization-roadmap.md) Tier 0 #7
> 状态：本轮完成动画生命周期收敛，并修复多处状态正确性问题。

---

## 0. 结论

本轮没有把 `Skeleton` 模板视觉搬到 C# 动态创建；`AbstractSkeletonTheme`、`SkeletonTheme` 的功能视觉继续保留在 axaml。

实际收益来自四类根因修复：

- `Skeleton.ContentProperty.Changed` 从实例构造器注册改为静态注册，避免每创建一个 `Skeleton` 就多注册一次 class handler；
- inactive `SkeletonLine` / `SkeletonButton` / `SkeletonAvatar` 等不再在 template apply 时提前构造 `Animation`；
- `Skeleton` 模板子控件不再先通过 `TemplateBinding IsActive` 启动自己的动画，再被 `Follow(parent)` 接管；
- `SkeletonParagraph` 行重建时释放旧 `SkeletonLine` 的 follow 绑定，并修复 `LineWidths` 更新总是写最后一行的问题。

主收益不是页面 timing，而是正确性和结构成本：`Skeleton.Content.NotLoading` 的 logical/root 从异常的 `902.5` 回到 `4.0`，分配下降约 `24.6%`；inactive / elements 场景分配下降约 `1.2% ~ 3.6%`。

---

## 1. 根因

### 1.1 class handler 重复注册

`Skeleton` 原来在实例构造器里执行：

```csharp
ContentProperty.Changed.AddClassHandler<Skeleton>((x, e) => x.HandleContentChanged(e));
```

这会导致每创建一个 `Skeleton` 实例，全局 class handler 就多一份。结果是后续 `Content` 变化被重复处理，logical children 被重复添加，`Skeleton.Content.NotLoading` 基线出现 `902.5 logical/root` 的异常。

### 1.2 inactive animation 提前创建

`AbstractSkeleton.OnApplyTemplate()` 原来对非 follow mode 直接 `BuildActiveAnimation()`。因此 inactive 的独立 `SkeletonLine`、`SkeletonButton`、`SkeletonAvatar` 等也会创建 `Animation + 3 KeyFrame + 3 Setter`。

### 1.3 parent follow 前重复启动

`SkeletonTheme.axaml` 给 `PART_Avatar` / `PART_Title` / `PART_Paragraph` 直接绑定了 `IsActive="{TemplateBinding IsActive}"`。这些子控件会先按自身 active 状态启动动画，随后 `Skeleton.OnApplyTemplate()` 再调用 `Follow(this)` 让它们跟随父控件，形成不必要的启动/停止和对象 materialization。

### 1.4 paragraph 行状态未清理

`SkeletonParagraph.BuildLines()` 清空旧 `SkeletonLine` 前没有解除 `Follow()` 绑定；`ConfigureLastLineWidths()` 还一直取 `Children.Last()`，导致更新多行宽度时实际只反复写最后一行。

---

## 2. 改动

### 2.1 生产代码

- `Skeleton`：`ContentProperty.Changed` class handler 移到 static constructor；
- `Skeleton.OnApplyTemplate()`：re-template 前解除旧 template part 的 follow 绑定；
- `AbstractSkeleton`：动画改为首次 active/start 时 lazy build；
- `AbstractSkeleton`：`MotionDuration` / `MotionEasingCurve` / loading background 变化时，只重建已 materialized 的动画，并保持运行态；
- `AbstractSkeleton.Follow()`：进入 follow mode 时停止并释放自身动画；
- `AbstractSkeleton.UnFollow(false)`：用于 template part / dynamic line 清理；
- `SkeletonParagraph`：重建行前解除旧行 follow；`LineWidths` 按行索引更新，并在缺省行上 `ClearValue(LineWidthProperty)`；
- `SkeletonTheme.axaml`：移除三个内部 part 上冗余的 `IsActive="{TemplateBinding IsActive}"`。

### 2.2 状态验证

`--verify-skeleton-states` 覆盖：

- 多个 `Skeleton` 实例创建后，`Content` logical child 仍只保留一个；
- `LineWidths` 更新按每一行生效；
- `Rows` 重建后旧 `SkeletonLine` 从 visual tree 和 follow target 中释放；
- inactive `SkeletonLine` 不提前构造 animation；
- active 后 lazy build/start animation；
- `MotionDuration` 变化后 animation 重建并重启；
- inactive 后 animation token 释放。

---

## 3. 控件级结果

命令：

```bash
dotnet run -c Release -f net10.0 --no-build \
  --project tools/performances/AtomUI.Performance/AtomUI.Performance.csproj -- \
  --suite skeleton --count 80 \
  --markdown /tmp/skeleton-after-rerun3.md
```

当前机器负载较高（`load averages: 8.78 7.84 7.81`），ms 只作参考；主看 KB、logical/root 和状态验证。

| Scenario | ms/item baseline | ms/item after | ms 变化 | KB/item baseline | KB/item after | KB 变化 | Logical/root baseline | Logical/root after |
| --- | ---: | ---: | ---: | ---: | ---: | ---: | ---: | ---: |
| `Skeleton.Basic.Loading` | 1.329 | 1.305 | +1.81% | 240.5 | 236.5 | +1.66% | 1.0 | 1.0 |
| `Skeleton.Avatar.Paragraph4` | 1.654 | 1.566 | +5.32% | 298.8 | 294.8 | +1.34% | 1.0 | 1.0 |
| `Skeleton.Active.Basic` | 1.499 | 1.559 | -4.00% | 247.2 | 244.8 | +0.97% | 1.0 | 1.0 |
| `Skeleton.Content.NotLoading` | 1.216 | 1.068 | +12.17% | 172.3 | 129.9 | +24.61% | 902.5 | 4.0 |
| `Skeleton.Paragraph.Rows4` | 0.884 | 0.905 | -2.38% | 154.4 | 152.6 | +1.17% | 1.0 | 1.0 |
| `Skeleton.Elements` | 1.628 | 1.555 | +4.48% | 243.0 | 234.2 | +3.62% | 6.0 | 6.0 |

说明：

- visual/root 不变，符合本轮不改模板结构的预期；
- `Content.NotLoading` 的 logical/root 修复是确定性正确性收益；
- active 场景 timing 受异步 animation 和机器负载影响，不声明稳定速度提升；
- 持续 shimmer render 仍需 Gallery 级 active 场景专项，不在本轮过度声明。

---

## 4. 正确性验证

命令：

```bash
dotnet build -c Release -f net10.0 tools/performances/AtomUI.Performance/AtomUI.Performance.csproj
dotnet run -c Release -f net10.0 --no-build \
  --project tools/performances/AtomUI.Performance/AtomUI.Performance.csproj -- \
  --verify-skeleton-states
```

结果：

| 验证 | 结果 |
| --- | --- |
| Release build | passed, 0 warnings, 0 errors |
| `--verify-skeleton-states` | passed |
| `git diff --check` | passed |

---

## 5. 复杂度自评

| 指标 | 数值 |
| --- | --- |
| 生产文件改动 | `AbstractSkeleton.cs`, `Skeleton.cs`, `SkeletonParagraph.cs`, `SkeletonTheme.axaml` |
| 新增 `_ignoreXxx` 标志位 | 0 |
| axaml 节点搬到 C# 动态创建 | 否 |
| 新增全局订阅 | 0；反而修复重复 class handler 注册 |
| lifecycle release path | `UnFollow(false)` 覆盖 template part 和 dynamic line 清理 |
| 正确性修复 | logical child 重复、line width 更新、old line follow 泄漏、motion animation 重建 |

本轮是正确性优先的结构优化：消除重复 class handler 和 inactive animation materialization，同时修复动态行的生命周期释放。

---

## 6. 追加结构优化：paragraph last line indexer lookup

`SkeletonParagraph.ConfigureLastLineWidth()` 在已有 `Children.Count > 0` 保护后，只需要读取最后一个 child。旧实现使用 LINQ `Last()`；本轮改为直接读取 `Children[Count - 1]`，保留最后一行宽度更新语义。

| 指标 | 优化前 | 优化后 | 公式 | 提升 | 结论 |
| --- | ---: | ---: | --- | ---: | --- |
| SkeletonParagraph last-line LINQ calls / width update | 1 `Last()` | 0 | `(1 - 0) / 1` | 100.00% | structural-only；最后一行直接按 index 读取 |
| Last line width semantics | unchanged | unchanged | n/a | 0.00% | 行为保持；仍按 `LineWidths` 或 `LastLineWidth` 更新 |
| Page-load timing claim | none | none | n/a | n/a | 本轮没有有效前后 timing，不声明页面级速度收益 |
