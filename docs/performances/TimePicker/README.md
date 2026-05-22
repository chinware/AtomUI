# TimePicker 性能优化

> 路线图位置：[`../desktop-controls-optimization-roadmap.md`](../desktop-controls-optimization-roadmap.md) Tier 1 #4
> 状态：本轮已完成 Gallery 实测。

---

## 0. 结论

本轮优化 `InfoPickerInput` 的 closed-state presenter 创建：`TimePicker` / `RangeTimePicker` 默认关闭时不再在 `OnApplyTemplate()` 中创建 `TimePickerPresenter`，而是在第一次打开 picker 前创建，并在 detach 时释放自己创建的 presenter。

这不是把模板视觉搬到 C# 动态创建。`Popup` shell、`ArrowDecoratedBox`、clear/icon accessory 仍保留在 axaml；延迟的是 popup 的重内容 presenter，符合 SKILL 的 Popup Lazy Content exception。

| 指标 | baseline | optimized | 改善 |
| --- | ---: | ---: | ---: |
| Cold first navigation mean | 464.82 ms | 393.11 ms | 15.4% |
| Cold median | 435.50 ms | 389.74 ms | 10.5% |
| Cold P95 | 623.34 ms | 418.20 ms | 32.9% |
| Cold alloc mean | 25430.67 KB | 25062.11 KB | 1.5% |
| Repeated navigation mean | 148.79 ms | 138.48 ms | 6.9% |
| Repeated median | 142.46 ms | 137.02 ms | 3.8% |
| Repeated P95 | 180.65 ms | 151.63 ms | 16.1% |
| Repeated alloc mean | 23617.32 KB | 23430.26 KB | 0.8% |
| Runtime visuals | 1230 | 1230 | 0.0% |
| `TimeView` / `DateTimePanel` | 0 / 0 | 0 / 0 | preserved |

---

## 1. 资格门槛

Gallery source：`controlgallery/AtomUIGallery/ShowCases/Views/DataEntry/TimePickerShowCase.axaml`

| 项 | 数值 |
| --- | ---: |
| `ShowCaseItem` | 9 |
| `<atom:TimePicker>` | 15 |
| `<atom:RangeTimePicker>` | 7 |
| Runtime `InfoPickerInput` | 22 |
| Runtime `AddOnDecoratedBox` | 22 |

真实会话操作频率：

- 页面导航：至少 1 次。
- 打开 picker：basic、confirm、interval、12/24 hour、variants、status、range 示例都会触发。
- 选择/hover/confirm：打开后的 `TimePickerPresenter` 事件链会参与状态同步。

结论：实例数 > 5，操作 > 1/session，满足 SKILL Tier 1 §13。

---

## 2. 根因

`InfoPickerInput.OnApplyTemplate()` 原来对所有 picker 都执行：

```csharp
if (PickerPresenter is null)
{
    PickerPresenter = CreatePickerPresenter();
    NotifyPickerPresenterCreated(PickerPresenter);
}
```

这意味着 `TimePickerShowCase` 的 22 个 closed picker 会在页面加载时创建 22 个 presenter 对象和对应 direct bindings。由于 popup 关闭，presenter 模板不会展开成 `TimeView`，所以 visual count 没有下降空间；主收益来自跳过 closed-state presenter 对象构造、绑定建立和派生控件缓存同步。

Avalonia source reference：

- `.referenceprojects/Avalonia/src/Avalonia.Base/Data/TemplateBindingExpression.cs:37-43`：`TemplateBinding` 订阅 templated parent 的 `PropertyChanged`，因此首次打开时设置 `PickerPresenter` 后，axaml 中的 `Content="{TemplateBinding PickerPresenter}"` 会继续更新。
- `.referenceprojects/Avalonia/src/Avalonia.Base/AvaloniaObject.cs:407-422`：内部 lazy 值使用 `SetCurrentValue`，避免破坏外部绑定/优先级。
- `.referenceprojects/Avalonia/src/Avalonia.Base/Layout/Layoutable.cs:546`：隐藏元素跳过 measure；本轮没有为了省隐藏节点而移动模板视觉。

---

## 3. 改动

### 3.1 `InfoPickerInput` 延迟 presenter

新增：

- `EnsurePickerPresenter()`：仅在 `IsPickerOpen=true` 或模板应用时已经处于打开态时创建 presenter。
- `_ownedPickerPresenter`：只跟踪本控件内部创建的 presenter。
- `ClearOwnedPickerPresenter()`：detach 时先关闭 picker，再通知派生类清理缓存，并仅在当前 `PickerPresenter` 仍是内部创建对象时清空属性。

关键边界：

- 外部提前设置 `PickerPresenter` 时不创建内部 presenter。
- 外部后续替换 `PickerPresenter` 时，detach 不会误清外部对象。
- `Popup` shell 和 `ArrowDecoratedBox` 保持在 ControlTheme 中。

### 3.2 派生类释放 presenter cache

`TimePicker`、`RangeTimePicker`、`DatePicker`、`RangeDatePicker` 覆写 `NotifyPickerPresenterCleared()`，在 owned presenter 被释放时把 `_pickerPresenter` 置空，避免 detach 后保留 stale presenter 引用。

### 3.3 性能工具补齐

新增：

- `GalleryPerformance --showcase timepicker`
- `AtomUI.Performance --verify-timepicker-states`

TimePicker state verification 覆盖：

- closed `TimePicker` / `RangeTimePicker` 不创建 `PickerPresenter`。
- closed input icon 仍存在。
- 第一次 materialize 创建正确 presenter。
- 第二次 materialize 复用同一个 presenter。
- detach 后清理 owned presenter。

---

## 4. 验证

### 4.1 构建

```bash
dotnet build tools/performances/AtomUI.GalleryPerformance/AtomUI.GalleryPerformance.csproj -c Debug --framework net10.0 --no-restore
```

结果：构建通过；保留既有 warning `DataGridColumn._clipboardContentBinding is never used`，非本轮新增。

```bash
dotnet build tools/performances/AtomUI.Performance/AtomUI.Performance.csproj -c Debug --framework net10.0 --no-restore
```

结果：0 Warning(s), 0 Error(s)。

### 4.2 状态验证

```bash
dotnet run --project tools/performances/AtomUI.Performance/AtomUI.Performance.csproj \
  -c Debug --framework net10.0 --no-build -- \
  --verify-timepicker-states
```

结果：`TimePicker state verification passed.`

### 4.3 Gallery 基线与优化后对比

命令：

```bash
dotnet run --project tools/performances/AtomUI.GalleryPerformance/AtomUI.GalleryPerformance.csproj \
  -c Debug --framework net10.0 --no-build -- \
  --showcase timepicker --label baseline \
  --cold-iterations 10 --iterations 30 --warmup 5 --timeout-ms 30000 \
  --markdown /tmp/timepicker-showcase-baseline.md
```

```bash
dotnet run --project tools/performances/AtomUI.GalleryPerformance/AtomUI.GalleryPerformance.csproj \
  -c Debug --framework net10.0 --no-build -- \
  --showcase timepicker --label lazy-presenter \
  --cold-iterations 10 --iterations 30 --warmup 5 --timeout-ms 30000 \
  --markdown /tmp/timepicker-showcase-lazy-presenter.md
```

| Set | Mean | Median | P95 | Alloc mean | Visuals | `InfoPicker` | `TimeView` | `DateTimePanel` |
| --- | ---: | ---: | ---: | ---: | ---: | ---: | ---: | ---: |
| Cold baseline | 464.82 ms | 435.50 ms | 623.34 ms | 25430.67 KB | 1230 | 22 | 0 | 0 |
| Cold optimized | 393.11 ms | 389.74 ms | 418.20 ms | 25062.11 KB | 1230 | 22 | 0 | 0 |
| Repeated baseline | 148.79 ms | 142.46 ms | 180.65 ms | 23617.32 KB | 1230 | 22 | 0 | 0 |
| Repeated optimized | 138.48 ms | 137.02 ms | 151.63 ms | 23430.26 KB | 1230 | 22 | 0 | 0 |

### 4.4 Shared surface smoke

`InfoPickerInput` 同时服务 DatePicker。本轮额外跑了 DatePicker showcase，确认关闭态仍没有创建 presenter visual：

```bash
dotnet run --project tools/performances/AtomUI.GalleryPerformance/AtomUI.GalleryPerformance.csproj \
  -c Debug --framework net10.0 --no-build -- \
  --showcase datepicker --label lazy-presenter \
  --cold-iterations 10 --iterations 30 --warmup 5 --timeout-ms 30000 \
  --markdown /tmp/datepicker-showcase-lazy-presenter.md
```

结果：`DatePickerPresenter=0`、`RangePickerPresenter=0`、`TimeView=0`、`DateTimePanel=0`，runtime visuals `1840`。
