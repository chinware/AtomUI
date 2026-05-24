# Switch 性能优化

> 路线图位置：[`../desktop-controls-optimization-roadmap.md`](../desktop-controls-optimization-roadmap.md) Tier 2 #7
> 状态：本轮完成交互路径结构优化；不声明页面加载耗时稳定提升。

---

## 0. 结论

本轮优化 `ToggleSwitch.IsChecked` 切换路径：`IsChecked` 不再触发 `InvalidateMeasure()`，只保留位置变化所需的 arrange。收益适用于用户点击开关、表单值回填、ViewModel 批量切换开关状态等交互路径。

`ToggleSwitchShowCase.axaml` 当前包含 13 个 `ToggleSwitch`、2 个 `Button`、5 个 showcase 分组、4 个 `AntDesignIconProvider`。页面加载时视觉树没有变化，因此页面导航耗时不是本轮主收益指标。

| 指标 | 旧实现 | 新实现 | 变化 | 结论 |
| --- | ---: | ---: | ---: | --- |
| `IsChecked` 1000 次切换的 measure invalidations | 1000 | 0 | 100% removed | 主收益 |
| `IsChecked` 1000 次切换的 arrange invalidations | 1000 | 1000 | 0 | 必须保留，开关位置需要重新 arrange |
| `DesiredSize` changes | 0 | 0 | 0 | 尺寸确实与 checked 状态无关 |
| `Switch.Default` visual/root | 7 | 7 | 0 | 初始树不变 |
| `Switch.GalleryShape` visual/root | 119 | 119 | 0 | 初始树不变 |

耗时类数据在本轮机器负载较高时采集，仅作为参考：交互基准 `60.21us/update -> 55.09us/update`；控件实例化和 Gallery 导航指标混合波动，不作为本轮性能提升声明。

---

## 1. 根因

原静态构造器把 `IsCheckedProperty` 注册为 measure 影响项：

```csharp
AffectsMeasure<AbstractToggleSwitch>(SizeTypeProperty, IsCheckedProperty);
```

Avalonia 12 中 `AffectsMeasure<T>` 会订阅属性变化并调用 `InvalidateMeasure()`：`.referenceprojects/Avalonia/src/Avalonia.Base/Layout/Layoutable.cs:502-512`。同文件 `AffectsArrange<T>` 对应 `InvalidateArrange()`：`.referenceprojects/Avalonia/src/Avalonia.Base/Layout/Layoutable.cs:523-533`。

但 `AbstractToggleSwitch.MeasureOverride()` 的尺寸只来自：

- `OnContent` / `OffContent` 两个 content presenter 的最大宽度；
- `TrackHeight`、`TrackMinWidth`、`InnerMinMargin`、`InnerMaxMargin` token；
- `KnobSize` 的 measure。

`IsChecked` 只影响 `CalculateElementsOffset(...)` 算出的 knob/content 位置，以及 wave 播放。`OnPropertyChanged(IsChecked)` 仍会调用 `CalculateElementsOffset(GrooveRect().Size)`，`KnobMovingRect`、`OnContentOffset`、`OffContentOffset` 仍在 `AffectsArrange` 中。因此去掉 measure 失效不会丢失 checked/unchecked 的视觉切换。

---

## 2. 改动

### 2.1 生产代码

`src/AtomUI.Controls/Switch/AbstractToggleSwitch.cs`

```csharp
static AbstractToggleSwitch()
{
    AffectsMeasure<AbstractToggleSwitch>(SizeTypeProperty);
    AffectsArrange<AbstractToggleSwitch>(
        IsPressedProperty,
        KnobRectProperty,
        KnobMovingRectProperty,
        OnContentOffsetProperty,
        OffContentOffsetProperty);
    AffectsRender<AbstractToggleSwitch>(GrooveBackgroundProperty, SwitchOpacityProperty);
}
```

保留项：

- `IsChecked` 变化时仍计算目标位置；
- `_isCheckedChanged` 仍驱动 checked 目标 `KnobMovingRect` arrange；
- wave、loading、disabled、content/icon 状态不改；
- 没有把主题视觉搬到 C# 动态创建。

### 2.2 验证与基准工具

- `--verify-switch-states` 增加 checked 切换不失效 measure、`DesiredSize` 不变、knob 目标位置正确的断言。
- 新增 `--measure-switch-interactions`，专门量化已实现 `ToggleSwitch` 上重复切换 `IsChecked` 后的 measure/arrange invalidation。

---

## 3. 实测结果

### 3.1 交互路径基准

命令：

```bash
dotnet run -c Release -f net10.0 --no-build \
  --project tools/performances/AtomUI.Performance/AtomUI.Performance.csproj -- \
  --measure-switch-interactions --count 1000 \
  --markdown /tmp/switch-interactions-after.md
```

临时恢复旧 `AffectsMeasure(SizeTypeProperty, IsCheckedProperty)` 后跑同一基准，再恢复优化代码复跑。

| 指标 | 旧实现 | 新实现 | 变化 |
| --- | ---: | ---: | ---: |
| Updates | 1000 | 1000 | 0 |
| Total ms | 60.21 | 55.09 | +8.50% |
| us/update | 60.21 | 55.09 | +8.50% |
| KB total | 8304.1 | 8306.8 | -0.03% |
| bytes/update | 8503.4 | 8506.2 | -0.03% |
| Measure invalidations | 1000 | 0 | 100% removed |
| Arrange invalidations | 1000 | 1000 | 0 |
| DesiredSize changes | 0 | 0 | 0 |
| Visual/root | 9 | 9 | 0 |
| Logical/root | 1 | 1 | 0 |

`Total ms` 在当前机器负载下仅作为参考；本轮可信主指标是 measure invalidation 从每次切换 1 次降为 0 次。

### 3.2 控件实例化基准

命令：

```bash
dotnet run -c Release -f net10.0 --no-build \
  --project tools/performances/AtomUI.Performance/AtomUI.Performance.csproj -- \
  --suite switch --count 80 \
  --markdown /tmp/switch-after.md
```

| Scenario | ms/item baseline | ms/item optimized | KB/item baseline | KB/item optimized | Visual |
| --- | ---: | ---: | ---: | ---: | ---: |
| `Switch.Default` | 0.398 | 0.386 | 78.4 | 78.4 | 7 |
| `Switch.Checked` | 0.399 | 0.389 | 79.2 | 79.2 | 7 |
| `Switch.Text` | 0.657 | 0.727 | 106.1 | 106.1 | 9 |
| `Switch.Icon` | 0.878 | 0.919 | 183.9 | 183.9 | 11 |
| `Switch.Loading` | 0.417 | 0.462 | 90.1 | 90.1 | 7 |
| `Switch.GalleryShape` | 11.125 | 10.507 | 2113.2 | 2113.2 | 119 |

实例化路径不是本轮目标，visual/logical/KB 基本不变；ms/item 在高负载下混合波动。

### 3.3 Gallery 页面基准

命令：

```bash
dotnet run -c Debug -f net10.0 --no-build \
  --project tools/performances/AtomUI.GalleryPerformance/AtomUI.GalleryPerformance.csproj -- \
  --showcase toggleswitch \
  --cold-iterations 5 --iterations 20 --warmup 5 \
  --markdown /tmp/toggleswitch-gallery-after.md
```

| 指标 | baseline | optimized | 变化 |
| --- | ---: | ---: | ---: |
| Cold mean | 159.65 ms | 161.81 ms | -1.35% |
| Cold median | 159.72 ms | 162.39 ms | -1.67% |
| Cold P95 | 170.43 ms | 166.74 ms | +2.17% |
| Cold alloc | 5011.07 KB | 5020.52 KB | -0.19% |
| Repeated mean | 43.38 ms | 41.95 ms | +3.30% |
| Repeated median | 43.57 ms | 44.38 ms | -1.86% |
| Repeated P95 | 48.25 ms | 49.90 ms | -3.42% |
| Repeated alloc | 3621.45 KB | 3621.52 KB | 0.00% |
| Runtime visuals | 170 | 170 | 0 |
| Runtime logical | 28 | 28 | 0 |

结论：Gallery 页面加载指标中性/噪声内，不作为本轮收益。

---

## 4. 正确性验证

命令：

```bash
dotnet build -c Release -f net10.0 tools/performances/AtomUI.Performance/AtomUI.Performance.csproj
dotnet run -c Release -f net10.0 --no-build \
  --project tools/performances/AtomUI.Performance/AtomUI.Performance.csproj -- \
  --verify-switch-states
```

结果：

| 验证 | 结果 |
| --- | --- |
| Release build | passed, 0 warnings, 0 errors |
| `--verify-switch-states` | passed |
| checked 切换后 `IsMeasureValid` | true |
| checked 切换后 `DesiredSize` | unchanged |
| checked knob 目标位置 | matches `KnobMovingRect.X` |

---

## 5. 复杂度自评

| 指标 | 数值 |
| --- | --- |
| 生产文件改动 | 1 行 |
| 新增 `Ensure*/Clear*/Sync*` 方法 | 0 |
| 新增 `_ignoreXxx` 标志位 | 0 |
| 新增 disposable / event / timer | 0 |
| axaml 节点搬到 C# 动态创建 | 否 |
| 正确性测试 | 增加 checked measure/knob 状态验证 |
| 性能工具 | 增加 Switch interaction benchmark |

本轮是低风险结构优化：删除错误的 measure 依赖，保留 arrange/render/状态路径。
