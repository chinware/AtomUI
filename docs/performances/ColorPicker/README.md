# ColorPicker 性能优化

> 状态：closed-state `Window.Deactivated` lifecycle、打开态 `GradientColorPickerView` binding、`ColorSpectrum` brush 热路径复用、`ColorPickerPaletteGroup` 选择事件隔离、`HsvValue` 预览 / slider thumb brush 复用、透明棋盘背景 brush token 缓存均已完成。

---

## 0. 结论

T3.1 已完成低风险生命周期优化：`AbstractColorPicker` 不再在 attach 时为关闭态 picker 订阅 `Window.Deactivated`，改为 picker 打开时订阅，关闭或 detach 时释放。

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

T3.2/T3.3 继续处理打开态子控件：

- `GradientColorPickerView` 的 3 个同 owner、同生命周期 `RelativeSource={RelativeSource TemplatedParent}` binding 改为 `TemplateBinding`。
- `ColorSpectrum` 仍按原来的 bitmap 生命周期生成图像；只把对应 `ImageBrush` 缓存在 bitmap 重建点，HSV 高频变化时复用 brush 并只更新 `Opacity` / `Fill` 引用。

| 指标 | baseline | optimized | 改善 | 结论 |
| --- | ---: | ---: | ---: | --- |
| Gradient view full TemplatedParent bindings | 3 | 0 | 100.00% replaced | 有效，结构性收益 |
| ColorSpectrum `UpdateBitmapSources` base/overlay brush refs / 1000 updates | source-derived 1000 / 1000 | 1 / 1 | ~99.90% fewer refs | 有效，交互热路径收益 |
| ColorSpectrum `UpdateBitmapSources` allocation | n/a | 0.3 bytes/update | n/a | 复测确认近零分配 |
| ColorSpectrum full `HsvColor` update allocation | 24386.6 bytes/update | 21434.6 bytes/update | 12.11% fewer bytes | 有效，打开态交互热路径收益 |
| ColorPickerView.Default materialization | 17.311 ms/item | 13.066-21.383 ms/item | +24.52% to -23.52% | 波动过大，不作为收益 |
| ColorPickerView.NoAlpha materialization | 11.233 ms/item | 6.223-6.333 ms/item | +43.62% to +44.60% | 正向，仍需谨慎 |
| GradientColorPickerView.Default materialization | 15.002 ms/item | 11.009-11.875 ms/item | +20.84% to +26.62% | 正向，仍需谨慎 |
| ColorPicker.GalleryShape materialization | 14.248 ms/item | 10.202-12.252 ms/item | +14.01% to +28.40% | 正向但有抖动 |
| Opened view visual/logical count | unchanged | unchanged | 0.00% | 符合预期 |

本轮判断：优化有效，主收益是打开态 ColorPicker 面板在颜色拖动/滑条联动时不再反复创建 `ImageBrush`，以及 Gradient view 模板绑定走更轻的 TemplateBinding 路径。页面闭合态 Gallery 导航不是这轮主路径；最新顺序复测期间 load averages 仍约 `8.81` 到 `11.76`，物化 timing 只能作为参考，不作为唯一收益依据。

T3.3 本轮补充处理 `ColorPickerPaletteGroup`：旧实现每个 palette group attach 时都注册一次 `PaletteColorItem.IsCheckedChangedEvent.AddClassHandler<ColorPickerPaletteGroup>`。这是类级订阅，不是实例级订阅；多个 palette group 同时存在时，后一个 group 的颜色选择可能被先 attach 的 group 截获，且会让每次路由事件多走全局 `Raised` subscriber。现在改为 palette group 自己的实例级 `AddDisposableHandler`，detach 时释放。

| metric | baseline | optimized | formula | improvement | conclusion |
| --- | ---: | ---: | --- | ---: | --- |
| Global class handlers per attached `ColorPickerPaletteGroup` | 1 / group | 0 / group | `(1 - 0) / 1` | 100.00% removed | 有效，结构性收益 |
| Correct owner notifications when selecting second palette group | 0 / selection | 1 / selection | `(1 - 0) / 1` | 100.00% restored | 正确性修复 |
| Wrong first-group notifications when selecting second palette group | 1 / selection | 0 / selection | `(1 - 0) / 1` | 100.00% removed | 正确性修复 |
| ColorPicker materialization timing | smoke-only | smoke-only | n/a | n/a | 单轮 smoke 只用于排查回退，不作为收益证明 |

T3.3 本轮继续处理 `HsvValue` 预览 / slider thumb brush：旧模板里同一个 `HsvValue` 分别通过 converter 生成 `ColorPreview.Background`、第三分量 slider thumb brush、alpha slider thumb brush；拖动取色时这三处每次更新都会创建新的 `SolidColorBrush`。现在 `AbstractColorPickerView` 持有两个实例级 brush（带 alpha / 不带 alpha），模板通过 `TemplateBinding` 复用，`HsvValue` 变化只更新 brush color。

| metric | baseline | optimized | formula | improvement | conclusion |
| --- | ---: | ---: | --- | ---: | --- |
| Preview/thumb `SolidColorBrush` allocations per `HsvValue` update / view | 3 | 0 | `(3 - 0) / 3` | 100.00% removed | 有效，结构性收益 |
| `ColorSpectrum.HsvColorUpdate` allocation | 24386.6 bytes/update | 21434.6 bytes/update | `(24386.6 - 21434.6) / 24386.6` | 12.11% fewer bytes | 有效，交互热路径收益 |
| `ColorSpectrum.HsvColorUpdate` time | 105.34 us/update | 62.82 us/update | `(105.34 - 62.82) / 105.34` | 40.36% faster | 单轮 microbench，辅助参考，不作为页面导航收益 |
| Opened view visual/logical count | 162 / 1 | 162 / 1 | `(162 - 162) / 162` | 0.00% | 符合预期 |
| ColorPicker materialization timing | smoke-only | smoke-only | n/a | n/a | 单轮 smoke 只用于排查回退，不作为收益证明 |

T3.3 本轮收口 `ColorBlock` / `ColorSlider` 透明棋盘背景：旧实现每次 `TransparentBgBrushUtils.Build(size, fillColor)` 都重新创建完整 `DrawingBrush` 图（`DrawingBrush` + `GeometryDrawing` + `ConicGradientBrush` + `GradientStops` + `RectangleGeometry`）。同一个 token 在 `ColorPreview`、第三分量 slider、alpha slider、gradient slider 中重复出现，应该共享同一份 token 派生 brush。现在按 `(size, fillColor)` 做 32 项有上限缓存；自定义颜色/尺寸仍按 key 隔离，避免跨主题误复用。

Avalonia source reference：

- `.referenceprojects/Avalonia/src/Avalonia.Base/Media/DrawingBrush.cs:15-43`：`DrawingBrush` 是持有 `Drawing` 的 brush 对象。
- `.referenceprojects/Avalonia/src/Avalonia.Base/Media/GeometryDrawing.cs:10-53`：`GeometryDrawing` 持有 `Geometry` / `Brush` / `Pen`。
- `.referenceprojects/Avalonia/src/Avalonia.Base/Media/ConicGradientBrush.cs:12-51`：`ConicGradientBrush` 持有 center / angle / gradient stops。

| metric | baseline | optimized | formula | improvement | conclusion |
| --- | ---: | ---: | --- | ---: | --- |
| Same-token transparent background brush refs / 1000 builds | 1000 | 1 | `(1000 - 1) / 1000` | 99.90% fewer refs | 有效，结构性收益 |
| `TransparentBgBrush.BuildSameToken` allocation | 10569.1 bytes/update | 0.2 bytes/update | `(10569.1 - 0.2) / 10569.1` | 99.998% fewer bytes | 有效，打开态背景热路径收益 |
| `TransparentBgBrush.BuildSameToken` time | 14.33 us/update | 0.17 us/update | `(14.33 - 0.17) / 14.33` | 98.81% faster | 单轮 microbench，辅助参考，不作为页面导航收益 |
| Realized `ColorPickerView` same-token transparent brush refs (`ColorPreview` + 2 visible sliders) | 3 | 1 | `(3 - 1) / 3` | 66.67% fewer refs | 状态验证覆盖 |
| ColorPicker materialization timing | smoke-only | smoke-only | n/a | n/a | 单轮 smoke 只用于排查回退，不作为收益证明 |

T3.3 本轮补充处理 Gradient stop/thumb 排序：旧实现用 `OrderBy(...).ToList()` 在取色、thumb 同步、active stop index 计算三条路径上创建 LINQ iterator 和 materialized list。现在保留稳定排序语义，改为 Count/indexer 复制 + 稳定插入排序，避免 LINQ 链。

| metric | baseline | optimized | formula | improvement | conclusion |
| --- | ---: | ---: | --- | ---: | --- |
| Gradient stop sort LINQ chains / color lookup | 1 `OrderBy().ToList()` | 0 LINQ chains | `(1 - 0) / 1` | 100.00% | 结构收益；相同 offset 保持原顺序 |
| Gradient thumb sort LINQ chains / sync + active index | 2 `OrderBy().ToList()` | 0 LINQ chains | `(2 - 0) / 2` | 100.00% | 结构收益；相同 value 保持原顺序 |
| Stable equal-key order behavior | stable | stable | n/a | 0.00% | 行为保持 |
| Page-load timing claim | none | none | n/a | n/a | 本轮没有有效前后 timing，不声明页面级速度收益 |

T3.3 本轮补充处理 ColorBlock preview 背景：`ColorPicker` 空值背景从每次 `new SolidColorBrush(Colors.Transparent)` 改为 Avalonia 已缓存的 `Brushes.Transparent`，普通颜色在颜色没变时不再重建 `SolidColorBrush` / 写入 `ColorBlockBackgroundProperty`；`GradientColorPicker` 的空值/同一 brush 引用也做短路。

Avalonia source reference：

- `.referenceprojects/Avalonia/src/Avalonia.Base/Media/Brushes.cs:674-676`：`Brushes.Transparent` 返回 known-color immutable brush。
- `.referenceprojects/Avalonia/src/Avalonia.Base/Media/KnownColors.cs:118-136`：known-color brush 会缓存复用。

| metric | baseline | optimized | formula | improvement | conclusion |
| --- | ---: | ---: | --- | ---: | --- |
| ColorPicker empty ColorBlock background brush allocations / update | 1 | 0 | `(1 - 0) / 1` | 100.00% | 结构收益；空值使用 cached `Brushes.Transparent` |
| ColorPicker same-color ColorBlock brush allocations / update | 1 | 0 | `(1 - 0) / 1` | 100.00% | 结构收益；颜色没变不再创建新 brush |
| ColorPicker same-color ColorBlock property writes / update | 1 | 0 | `(1 - 0) / 1` | 100.00% | 结构收益；避免重复 DirectProperty 写入和 AffectsMeasure |
| GradientColorPicker same-reference background writes / update | 1 | 0 | `(1 - 0) / 1` | 100.00% | 结构收益；同一 gradient brush 引用不重复写入 |

说明：这是 preview 背景更新路径 structural-only 收益；没有新增 Gallery timing 对比，不声明页面加载速度提升。

T3.3 本轮最后补齐 `GradientColorPickerTrack` active thumb 兜底路径：缩减 gradient stop 后，如果当前 `ActivatedThumb` 已不在 `Thumbs` 中，旧实现用 `Thumbs.First()` 取第一个 thumb。`Thumbs` 是 `List<GradientColorSliderThumb>`，且已有 `Thumbs.Count > 0` 保护；现在直接用 `Thumbs[0]`，语义不变，去掉一次 LINQ enumerator。

| metric | baseline | optimized | formula | improvement | conclusion |
| --- | ---: | ---: | --- | ---: | --- |
| Active thumb fallback LINQ enumerators / gradient stop shrink | 1 | 0 | `(1 - 0) / 1` | 100.00% | 结构收益；直接走 `List<T>` indexer |
| Removed thumb temp list growth reallocations / gradient stop shrink | dynamic | exact `delta` capacity | structural | 结构收益 | 缩减数量已知，按 `delta` 预分配 |
| Default palette group list growth reallocations / picker view default palette setup | dynamic | exact 4 capacity | structural | 结构收益 | 默认 palette 数固定为 4 |
| Active thumb fallback semantics | first thumb | first thumb | n/a | 0.00% | 行为保持 |
| Page-load timing claim | none | none | n/a | n/a | 本轮没有有效前后 timing，不声明页面级速度收益 |

T3.4 本轮补齐 HEX 输入解析路径：`ColorPickerInput` 原先在确认输入时对 `_hexValueInput.Text` 创建 trimmed string 后再 `Color.TryParse`；现在使用 span trim + span `Color.TryParse`，保持 HEX 解析和 alpha 合成语义。

| metric | baseline | optimized | formula | improvement | conclusion |
| --- | ---: | ---: | --- | ---: | --- |
| HEX input trim temp strings / confirm input | 1 string risk | 0 strings | `(1 - 0) / 1` | 100.00% | structural-only；span trim 后直接 parse |
| HEX parse behavior | `Color.TryParse(string)` | `Color.TryParse(ReadOnlySpan<char>)` | n/a | 0.00% | 行为保持 |
| Page-load timing claim | none | none | n/a | n/a | 本轮没有有效前后 timing，不声明页面级速度收益 |

T3.4 本轮追加 `ColorSpectrum` 默认 HSV 缓存空集合收敛：实例创建时 `_hsvValues` 只需要支持 `Count` 和 indexer，真正 bitmap 生成后会替换为有容量的 `List<Hsv>`。因此默认空态改用 `Array.Empty<Hsv>()`，避免每个 `ColorSpectrum` 初始化时分配空 `List<Hsv>`，颜色取样和键盘移动语义不变。

| metric | baseline | optimized | formula | improvement | conclusion |
| --- | ---: | ---: | --- | ---: | --- |
| ColorSpectrum empty HSV cache list allocations / instance | 1 | 0 | `(1 - 0) / 1` | 100.00% | structural-only；默认空态复用 `Array.Empty<Hsv>()` |
| Generated HSV map capacity / bitmap rebuild | exact pixel count | exact pixel count | n/a | 0.00% | 行为保持；生成后仍使用 `new List<Hsv>(pixelCount)` |
| ColorSpectrum arrow-key modifier enum `HasFlag` calls / keydown | 1 | 0 | `(1 - 0) / 1` | 100.00% | structural-only；Control modifier 改为 bitwise check |
| GradientColorPicker excess text-run removal LINQ calls / sync | 1 per removed run | 0 | `(1 - 0) / 1` | 100.00% | structural-only；直接 `RemoveAt(lastIndex)` |
| Page-load timing claim | none | none | n/a | n/a | 本轮没有有效前后 timing，不声明页面级速度收益 |

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
- palette group 选择：打开 palette 示例后每次点击色块触发。
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

`ColorPickerPaletteGroup` 的旧逻辑在实例 `OnAttachedToVisualTree()` 内调用 `AddClassHandler`：

```csharp
PaletteColorItem.IsCheckedChangedEvent.AddClassHandler<ColorPickerPaletteGroup>(...);
```

Avalonia source reference：

- `.referenceprojects/Avalonia/src/Avalonia.Base/Interactivity/RoutedEvent.cs:78-94`：`AddClassHandler` 订阅 routed event 的全局 `Raised` observable。
- `.referenceprojects/Avalonia/src/Avalonia.Base/Interactivity/RoutedEvent.cs:121-134`：泛型 `AddClassHandler<TTarget>` 最终仍调用类级订阅。
- `.referenceprojects/Avalonia/src/Avalonia.Base/Interactivity/Interactive.cs:30-42`：`AddHandler` 写入当前实例事件表。
- `.referenceprojects/Avalonia/src/Avalonia.Base/Interactivity/InteractiveExtensions.cs:21-30`：`AddDisposableHandler` 是实例 `AddHandler` + dispose/remove 的封装。

因此 palette group 的色块选择应该用实例级 handler；类级 handler 只能放在 static constructor 这类每类型一次的注册点。

---

## 3. 改动

`AbstractColorPicker`：

- `_attachedWindow` 改为 `_deactivatedWindow`，语义变为“当前已订阅 deactivated 的窗口”。
- `OnAttachedToVisualTree()` 只配置 trigger handler；如果控件 attach 时已经打开，才补订阅。
- `NotifyPickerOpened()` 注册 `Window.Deactivated`。
- `NotifyPickerClosed()` 和 `OnDetachedFromVisualTree()` 释放订阅。
- `RegisterWindowDeactivatedHandler(Window? window)` 先释放旧窗口再订阅新窗口，避免 reattach 或窗口变化时残留旧 handler。

`ColorPickerPaletteGroup`：

- `PaletteColorItem.IsCheckedChangedEvent.AddClassHandler<ColorPickerPaletteGroup>` 改为 `this.AddDisposableHandler(...)`。
- handler 不再捕获 attach 时的 `this` 闭包，而是只处理当前 palette group 路由里的事件。
- `OnDetachedFromVisualTree()` 释放实例 handler 并清空 disposable。

`AbstractColorPickerView`：

- 增加带 alpha / 不带 alpha 两个实例级 `SolidColorBrush`。
- `ColorPickerViewTheme` / `GradientColorPickerViewTheme` 中 `ColorPreview.Background`、`ColorSpectrumThirdComponentSlider.ThumbColorValueBrush`、`ColorSpectrumAlphaSlider.ThumbColorValueBrush` 从 converter binding 改为 `TemplateBinding` 复用 brush。
- `ColorPickerView` / `GradientColorPickerView` 在 `HsvValue` 变化时同步更新实例 brush 的 `Color`。

状态验证：

- closed ColorPicker 不持有 `_deactivatedWindow`。
- open 路径会注册。
- close 路径释放。
- detach 路径释放。
- 两个 palette-enabled `ColorPickerView` 同时存在时，第二个 group 的色块选择只更新第二个 view，不会触发第一个 view。
- `HsvValue` 连续变化时，`ColorPreview`、第三分量 slider、alpha slider 复用原 brush 引用，同时颜色值更新正确。

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

本轮新增覆盖：

- 第二个 palette group 的色块选择只触发第二个 `ColorPickerView.ValueChanged`。
- 第一个 palette group 不收到跨 group 的错误通知。
- 被选中的色值原样转发。
- `HsvValue` 连续变化时，预览块和 slider thumb brush 引用复用且颜色值更新正确。
- `ColorPreview` 和两个可见 `ColorSlider` 共享同 token 透明背景 brush；不同 size 使用不同 key，恢复 size 后复用原 brush。

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

### 4.4 本轮 smoke 复测

单轮，只用于排查明显回退，不作为收益证明：

| Scenario | ms/item | KB/item | Visual/root | Logical/root |
| --- | ---: | ---: | ---: | ---: |
| ColorPicker.Default | 0.929 | 139.8 | 11 | 1 |
| ColorPickerView.Default | 15.073 | 4586.9 | 162 | 1 |
| GradientColorPickerView.Default | 9.248 | 3787.8 | 173 | 1 |
| ColorPicker.GalleryShape | 10.378 | 3464.9 | 274 | 37 |

### 4.5 交互热路径复测

命令：

```bash
dotnet run -c Release -f net10.0 --no-build \
  --project tools/performances/AtomUI.Performance/AtomUI.Performance.csproj -- \
  --measure-colorpicker-interactions --count 1000
```

| Scenario | baseline bytes/update | optimized bytes/update | baseline us/update | optimized us/update | baseline refs | optimized refs |
| --- | ---: | ---: | ---: | ---: | ---: | ---: |
| ColorSpectrum.UpdateBitmapSources | 0.3 | 0.3 | 0.66 | 0.64 | 1000 / 1000 source-derived | 1 / 1 |
| ColorSpectrum.HsvColorUpdate | 24386.6 | 21434.6 | 105.34 | 62.82 | 6 / 6 | 6 / 6 |
| TransparentBgBrush.BuildSameToken | 10569.1 | 0.2 | 14.33 | 0.17 | 1000 | 1 |

### 4.6 Gallery 复测

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
