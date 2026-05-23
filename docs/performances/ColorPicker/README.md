# ColorPicker 性能优化

> 路线图位置：[`../desktop-controls-optimization-roadmap.md`](../desktop-controls-optimization-roadmap.md) Phase E / Tier 3
> 状态：本轮完成 closed-state `Window.Deactivated` lifecycle 收敛；ColorPicker 包仍保留后续打开态子控件优化空间。

---

## 0. 结论

本轮只做低风险生命周期优化：`AbstractColorPicker` 不再在 attach 时为关闭态 picker 订阅 `Window.Deactivated`，改为 picker 打开时订阅，关闭或 detach 时释放。

这是结构收益，不是 visual tree 收益。`ColorPickerShowCase` 当前有 23 个 `ColorPicker` / `GradientColorPicker` 实例，页面加载后默认全是关闭态；优化前这些实例会在 attach 阶段全部挂到宿主 `Window.Deactivated`，优化后关闭态为 0 个订阅。打开态仍保留窗口失活自动关闭行为。

| 指标 | baseline | optimized | 改善 | 结论 |
| --- | ---: | ---: | ---: | --- |
| Closed `Window.Deactivated` subscriptions | 23 | 0 | 100.00% removed | 有效，结构性收益 |
| Open picker `Window.Deactivated` subscription | 1 / open picker | 1 / open picker | preserved | 行为不变 |
| Runtime visuals | 370 | 370 | 0.00% | 符合预期 |
| Runtime logical | 49 | 49 | 0.00% | 符合预期 |
| Repeated alloc mean | 6787.93 KB | 6772.82 KB | 0.22% | 小幅下降 |
| Repeated mean | 73.19 ms | 72.25 ms | 1.28% | 噪声内，不作为主收益 |
| Repeated median | 66.10 ms | 68.01 ms | -2.89% | 噪声内，不作为回退结论 |
| Repeated P95 | 110.93 ms | 104.11 ms | 6.15% | 噪声内，谨慎看待 |

本轮判断：优化有效，但收益主要体现在减少 idle 事件订阅和降低长期生命周期负担；页面导航 timing 在当前机器负载下不能作为确定性收益声明。

---

## 1. 资格门槛

Gallery source：`controlgallery/AtomUIGallery/ShowCases/Views/DataEntry/ColorPickerShowCase.axaml`

| 项 | 数值 |
| --- | ---: |
| `<atom:ColorPicker>` | 20 |
| `<atom:GradientColorPicker>` | 3 |
| `AbstractColorPicker` total | 23 |
| 默认打开态 picker | 0 |

真实会话操作频率：

- 页面导航：至少 1 次。
- 打开 picker：示例页通常会逐项试用基础、hover、format、clear、gradient、palette group。
- 窗口失活：低频，但必须保留打开态自动关闭语义。

结论：实例数明显 > 5，页面加载默认关闭态占多数，满足 SKILL Tier 1 §13。优化重点是让 closed controls 不承担 popup-open-only 的全局/window 订阅。

---

## 2. 根因

旧逻辑在 `OnAttachedToVisualTree()` 中执行：

```csharp
var topLevel = TopLevel.GetTopLevel(this);
if (topLevel is Window window)
{
    _attachedWindow = window;
    window.Deactivated += HandleWindowDeactivated;
}
```

这把“打开态需要在窗口失活时关闭 picker”的行为成本提前到了页面加载阶段。`ColorPickerShowCase` 23 个 picker 默认关闭，仍会保留 23 条窗口事件订阅。每次窗口失活时，宿主窗口会遍历这些 handler，即使大部分 picker 没有打开。

Avalonia source reference：

- `.referenceprojects/Avalonia/src/Avalonia.Controls/WindowBase.cs:74`：`WindowBase` 暴露 `Deactivated` 事件。
- `.referenceprojects/Avalonia/src/Avalonia.Controls/WindowBase.cs:347-351`：平台 deactivated 通知触发 `Deactivated?.Invoke(...)`。

因此，这类订阅应按真实打开生命周期配对，而不是按 visual attach 生命周期配对。

---

## 3. 改动

`AbstractColorPicker`：

- `_attachedWindow` 改为 `_deactivatedWindow`，语义变为“当前已订阅 deactivated 的窗口”。
- `OnAttachedToVisualTree()` 只配置 trigger handler；如果控件 attach 时已经打开，才补订阅。
- `NotifyPickerOpened()` 注册 `Window.Deactivated`。
- `NotifyPickerClosed()` 和 `OnDetachedFromVisualTree()` 释放订阅。
- `RegisterWindowDeactivatedHandler(Window? window)` 先释放旧窗口再订阅新窗口，避免 reattach 或窗口变化时残留旧 handler。

状态验证：

- closed ColorPicker 不持有 `_deactivatedWindow`。
- open 路径会注册。
- close 路径释放。
- detach 路径释放。

---

## 4. 验证

### 4.1 构建

```bash
dotnet build -c Release -f net10.0 tools/performances/AtomUI.Performance/AtomUI.Performance.csproj
```

结果：0 Warning(s), 0 Error(s)。

```bash
dotnet build -c Debug tools/performances/AtomUI.GalleryPerformance/AtomUI.GalleryPerformance.csproj
```

结果：构建通过；保留既有 warning `DataGridColumn._clipboardContentBinding is never used`，非本轮新增。

### 4.2 状态验证

```bash
dotnet run -c Release -f net10.0 --no-build \
  --project tools/performances/AtomUI.Performance/AtomUI.Performance.csproj -- \
  --verify-colorpicker-states
```

结果：`ColorPicker state verification passed.`

### 4.3 控件级复测

命令：

```bash
dotnet run -c Release -f net10.0 --no-build \
  --project tools/performances/AtomUI.Performance/AtomUI.Performance.csproj -- \
  --suite colorpicker --count 80
```

| Scenario | baseline ms/item | optimized ms/item | baseline KB/item | optimized KB/item | Visual/root | Logical/root |
| --- | ---: | ---: | ---: | ---: | ---: | ---: |
| ColorPicker.Default | 0.784 | 0.768 | 151.4 | 151.4 | 11 | 1 |
| GradientColorPicker.Text | 1.013 | 0.613 | 174.6 | 174.6 | 14 | 1 |
| ColorPicker.GalleryShape | 14.580 | 12.902 | 3808.0 | 3806.9 | 274 | 37 |

说明：控件级 runner 使用 `Avalonia.Controls.Window`，而生产代码这里订阅的是 AtomUI `Window`，所以这组 ms/item 只能用于排查结构/分配回退，不能证明本轮订阅优化的速度收益。

### 4.4 Gallery 复测

命令：

```bash
dotnet run -c Debug -f net10.0 --no-build \
  --project tools/performances/AtomUI.GalleryPerformance/AtomUI.GalleryPerformance.csproj -- \
  --showcase colorpicker --cold-iterations 5 --iterations 20 --warmup 5
```

机器状态：复测期间 `load averages` 约 `9.18 13.25 13.93`，timing 噪声偏大。

| Set | Mean | Median | P95 | Alloc mean | Visuals | Logical |
| --- | ---: | ---: | ---: | ---: | ---: | ---: |
| Cold baseline | 177.11 ms | 178.37 ms | 190.72 ms | 7470.18 KB | 370 | 49 |
| Cold optimized | 173.07 ms | 170.03 ms | 182.46 ms | 7466.55 KB | 370 | 49 |
| Repeated baseline | 73.19 ms | 66.10 ms | 110.93 ms | 6787.93 KB | 370 | 49 |
| Repeated optimized | 72.25 ms | 68.01 ms | 104.11 ms | 6772.82 KB | 370 | 49 |

结论：visual/logical 不变，分配小幅下降；timing 混合且负载较高，不把导航耗时当成本轮主收益。
