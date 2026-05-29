# TabControl 性能优化

> 路线图位置：[`../desktop-controls-optimization-roadmap.md`](../desktop-controls-optimization-roadmap.md) Tier 1 #7  
> 状态：本轮已完成控件级与 Gallery 实测。

---

## 0. 结论

本轮优化 `TabItem` / `TabStripItem` 的默认模板成本，并补齐 overflow flyout 生命周期清理：

- 默认无 icon / 非 closable tab 不再创建隐藏 `ItemIconPresenter` 与 `PART_ItemCloseButton`。
- icon、closable、icon+closable 三种实际需要的结构仍保留在 axaml 模板分支中，没有把主题视觉搬到 C# 动态创建。
- `TabControlScrollViewer` / `TabStripScrollViewer` 的 overflow `MenuFlyout` 关闭、detach、无 overflow items 时都会释放 binding、items 和事件订阅。
- re-template 时释放旧 close button 事件，并从旧模板 `StackPanel` 中移除旧 part，避免测试保留旧引用时仍看到 visual parent。
- `BaseTabControl` / `BaseTabStrip` 的 tab strip 边线渲染不再每次 `Render()` 创建 `Pen`，改为按 brush / thickness 缓存。

| 指标 | baseline | optimized | 改善 |
| --- | ---: | ---: | ---: |
| Gallery cold mean | 502.81 ms | 486.28 ms | 3.3% |
| Gallery cold median | 497.52 ms | 485.52 ms | 2.4% |
| Gallery cold P95 | 545.49 ms | 516.11 ms | 5.4% |
| Gallery cold alloc mean | 31895.95 KB | 28454.13 KB | 10.8% |
| Gallery repeated mean | 170.75 ms | 157.35 ms | 7.8% |
| Gallery repeated median | 168.15 ms | 152.67 ms | 9.2% |
| Gallery repeated P95 | 193.23 ms | 186.57 ms | 3.4% |
| Gallery repeated alloc mean | 28552.12 KB | 25178.99 KB | 11.8% |
| Gallery runtime visuals | 1583 | 1405 | 178 |
| Gallery `IconButton` | 134 | 31 | 103 |
| Gallery `IconPresenter` | 130 | 55 | 75 |

控件级 14 个 scenario 平均：`ms/item -14.8%`，`KB/item -12.9%`。收益主要来自无 icon / 非 closable 默认路径的模板树裁剪；真实 Gallery 页面也有稳定的分配与 visual 降幅。

| 追加指标 | baseline | optimized | 公式 | 改善 | 结论 |
| --- | ---: | ---: | --- | ---: | --- |
| BaseTabControl tab strip border `Pen` allocations / repeated render | 1 pen | 0 pens after first render | `(1 - 0) / 1` | 100.00% | structural-only；每个 TabControl 后续 render 不再为边线分配 `Pen` |
| BaseTabStrip tab strip border `Pen` allocations / repeated render | 1 pen | 0 pens after first render | `(1 - 0) / 1` | 100.00% | structural-only；每个 TabStrip 后续 render 不再为边线分配 `Pen` |

---

## 1. 资格门槛

Gallery source：`controlgallery/AtomUIGallery/ShowCases/Views/Navigation/TabControlShowCase.axaml`

| 项 | 数值 |
| --- | ---: |
| `<atom:TabControl>` | 14 |
| `<atom:CardTabControl>` | 8 |
| `<atom:TabStrip>` | 10 |
| `<atom:CardTabStrip>` | 7 |
| `<atom:TabItem>` | 105 |
| `<atom:TabStripItem>` | 81 |
| Runtime visuals baseline | 1583 |

真实会话操作频率：

- 页面导航：至少 1 次。
- tab 切换：基础、Card、position、size、editable demo 都会触发。
- overflow menu：slide / position 场景会触发滚动和 overflow menu。
- closable tab：close button 点击会触发 item 移除、selection 调整和 flyout item 清理。

实例数远高于 5，且操作频率 > 1/session，满足 SKILL Tier 1 §13。

---

## 2. 根因

原 `TabItem` / `TabStripItem` base 和 card 模板固定创建 `ItemIconPresenter` 与 `PART_ItemCloseButton`，再通过 `Icon` / `IsClosable` 相关状态隐藏或控制行为。`TabControlShowCase` 中大多数 tab 没有 icon，也不是 closable，因此这些隐藏节点会在页面加载时被大量创建。

Avalonia source reference：

- `.referenceprojects/Avalonia/src/Avalonia.Controls/Primitives/TemplatedControl.cs:316-346`：`Template` 变化后重新 build 模板树，旧模板根从 `VisualChildren` 移除，新模板根加入 visual tree。
- `.referenceprojects/Avalonia/src/Avalonia.Base/Layout/Layoutable.cs:546` / `:671` 与 `.referenceprojects/Avalonia/src/Avalonia.Base/Rendering/ImmediateRenderer.cs:34`：`IsVisible=false` 会跳过 layout/render，但已经创建的模板节点仍承担实例化和 binding 成本。

这次没有采用 C# `EnsureIconPresenter()` / `EnsureCloseButton()`，因为这会把主题视觉和 `/template/` 样式语义搬进手写生命周期。更低风险的方案是在 axaml 内提供结构化模板分支，用 `HasIcon` / `IsClosable` 选择实际需要的模板结构。

---

## 3. 改动

### 3.1 Tab item 模板分支

涉及文件：

- `src/AtomUI.Desktop.Controls/TabControl/TabItem.cs`
- `src/AtomUI.Desktop.Controls/TabControl/TabStrip/TabStripItem.cs`
- `src/AtomUI.Desktop.Controls/TabControl/Themes/BaseTabItemTheme.axaml`
- `src/AtomUI.Desktop.Controls/TabControl/Themes/CardTabItemTheme.axaml`
- `src/AtomUI.Desktop.Controls/TabControl/Themes/TabStrip/BaseTabStripItemTheme.axaml`
- `src/AtomUI.Desktop.Controls/TabControl/Themes/TabStrip/CardTabStripItemTheme.axaml`

新增 internal `HasIcon` styled property，随 `Icon` 更新。模板分支：

| 状态 | 模板结构 |
| --- | --- |
| `HasIcon=False`, `IsClosable=False` | content only |
| `HasIcon=True`, `IsClosable=False` | icon + content |
| `HasIcon=False`, `IsClosable=True` | content + close button |
| `HasIcon=True`, `IsClosable=True` | icon + content + close button |

`CloseIcon` 默认值只在 `IsClosable=true` 时创建，避免非 closable tab 提前创建 close icon。

### 3.2 Close button re-template 清理

`OnApplyTemplate` 重新应用时：

- 先移除旧 `_closeButton.Click`。
- 如果旧 close button 仍挂在旧模板 `Panel` 下，从旧 parent 中移除。
- 再绑定新模板中的 `PART_ItemCloseButton`。

这个清理是为了覆盖 Avalonia re-template 后旧模板子树内部 parent 链仍存在的测试场景；生产路径中旧模板根已经从当前控件 visual tree 分离。

### 3.3 Overflow flyout 生命周期

`TabControlScrollViewer` / `TabStripScrollViewer`：

- `MenuFlyout.Closed` 改为 method group，关闭时统一释放。
- detach 时主动 `Hide()` 并清理 flyout。
- no overflow items 时直接清理新建 flyout。
- 清理 menu item `Click` / `CloseTab` 事件、`Items` 和 `_flyoutBindingDisposable`。
- re-template 前移除旧 `MenuIndicator.Click`，避免重复订阅。

### 3.4 Tab strip border Pen 缓存

`BaseTabControl.Render()` 和 `BaseTabStrip.Render()` 原先每次绘制 tab strip 边线都会执行 `new Pen(BorderBrush, borderThickness)`。

现在各自缓存一支 `_tabStripBorderPen`，同一实例的后续 render 直接复用；当 `BorderBrush` 引用或 `BorderThickness.Left` 变化时重建。`AffectsRender` 同步补充 `BorderThicknessProperty`，确保厚度变化后仍会触发重绘。

Avalonia source reference：

- `.referenceprojects/Avalonia/src/Avalonia.Base/Media/Pen.cs:17-40`：`Pen` 是 mutable `AvaloniaObject`，`Brush` / `Thickness` 是 styled properties；render 热路径不应按帧创建。
- `.referenceprojects/Avalonia/src/Avalonia.Base/Visual.cs:446-500`：`AffectsRender` 会在相关属性变化时 invalidate render。

---

## 4. 验证

### 4.1 状态验证

```bash
dotnet run --project tools/performances/AtomUI.Performance/AtomUI.Performance.csproj \
  -c Debug --framework net10.0 -- \
  --verify-tabcontrol-states
```

结果：`TabControl state verification passed.`

覆盖：

- 默认 `TabItem` / `TabStripItem` 不创建 icon presenter 和 close button。
- `Icon` / `IsClosable` 动态开关会创建并移除对应 slot。
- close button 行为仍会触发 close event 和 item removal。
- Card add button re-template 不重复订阅。
- overflow flyout close / detach / reattach 后能清理并重新打开。

追加 border pen 缓存验证：

```bash
dotnet build -c Release -f net10.0 --no-restore tools/performances/AtomUI.Performance/AtomUI.Performance.csproj
dotnet run -c Release -f net10.0 --no-build --project tools/performances/AtomUI.Performance/AtomUI.Performance.csproj -- --verify-tabcontrol-states
```

说明：该追加项是 render-path structural-only；当前 harness 没有逐帧 render allocation 计数器，因此不使用单次 timing 证明速度收益。

### 4.2 控件级基准

命令：

```bash
dotnet run --project tools/performances/AtomUI.Performance/AtomUI.Performance.csproj \
  -c Debug --framework net10.0 -- \
  --suite tabcontrol --count 40 --iterations 8 --warmup 2 \
  --markdown /tmp/atomui-tabcontrol-baseline.md
```

```bash
dotnet run --project tools/performances/AtomUI.Performance/AtomUI.Performance.csproj \
  -c Debug --framework net10.0 -- \
  --suite tabcontrol --count 40 --iterations 8 --warmup 2 \
  --markdown /tmp/atomui-tabcontrol-optimized.md
```

| Scenario | ms/item baseline | ms/item optimized | KB/item baseline | KB/item optimized | Visual baseline | Visual optimized |
| --- | ---: | ---: | ---: | ---: | ---: | ---: |
| `TabControl.Line.Basic3` | 4.065 | 3.031 | 609.3 | 493.8 | 42 | 36 |
| `TabControl.Line.Overflow20` | 9.472 | 7.979 | 2688.2 | 1944.3 | 165 | 125 |
| `TabControl.Card.Basic3` | 1.737 | 1.524 | 744.5 | 630.2 | 48 | 42 |
| `TabControl.Card.Overflow20` | 11.625 | 8.154 | 3474.2 | 2716.8 | 205 | 165 |
| `TabStrip.Line.Basic3` | 1.578 | 0.971 | 576.0 | 460.9 | 39 | 33 |
| `TabControl.GalleryShape.FirstSection` | 48.346 | 42.673 | 15437.1 | 12830.1 | 907 | 770 |

### 4.3 Gallery 导航基准

baseline 使用临时 clean worktree：`/tmp/AtomUIV6-tabcontrol-baseline` at `5771513d6`，跑完已删除。

```bash
dotnet run --project tools/performances/AtomUI.GalleryPerformance/AtomUI.GalleryPerformance.csproj \
  -c Debug --framework net10.0 -- \
  --showcase tabcontrol --label tabcontrol-baseline \
  --cold-iterations 10 --iterations 30 --warmup 5 --timeout-ms 30000 \
  --markdown /tmp/tabcontrol-showcase-navigation-baseline.md
```

```bash
dotnet run --project tools/performances/AtomUI.GalleryPerformance/AtomUI.GalleryPerformance.csproj \
  -c Debug --framework net10.0 -- \
  --showcase tabcontrol --label tabcontrol-optimized \
  --cold-iterations 10 --iterations 30 --warmup 5 --timeout-ms 30000 \
  --markdown /tmp/tabcontrol-showcase-navigation-optimized.md
```

| Set | Mean | Median | P95 | Alloc mean | Visuals | IconButton | IconPresenter |
| --- | ---: | ---: | ---: | ---: | ---: | ---: | ---: |
| Cold baseline | 502.81 ms | 497.52 ms | 545.49 ms | 31895.95 KB | 1583 | 134 | 130 |
| Cold optimized | 486.28 ms | 485.52 ms | 516.11 ms | 28454.13 KB | 1405 | 31 | 55 |
| Repeated baseline | 170.75 ms | 168.15 ms | 193.23 ms | 28552.12 KB | 1583 | 134 | 130 |
| Repeated optimized | 157.35 ms | 152.67 ms | 186.57 ms | 25178.99 KB | 1405 | 31 | 55 |

构建过程中仅出现既有 warning：`DataGridColumn._clipboardContentBinding is never used`，非本轮新增。

---

## 5. 复杂度自评

| 指标 | 数值 |
| --- | --- |
| 新增 `Ensure*/Clear*/Sync*` 方法 | 0 |
| 新增 `_ignoreXxx` 标志位 | 0 |
| 新增 disposable 字段 | 0 |
| 新增事件订阅 | 0；只补齐既有 click/flyout 订阅释放 |
| axaml 节点搬到 C# 动态创建 | 否 |
| 新增模板分支 | 4 个主题各 3 个分支 |
| 生产文件范围 | 8 个文件，均在 `TabControl` |

模板分支有一定重复，但它把视觉结构留在 ControlTheme 内，避免了 C# 动态创建带来的生命周期和主题选择器风险。
