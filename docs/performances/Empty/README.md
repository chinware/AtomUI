# Empty 性能优化记录

## 目标

`Empty` 是 Data Display 控件，也会被 `ListView`、`TreeView`、`CascaderView`、`DataGrid` 等空态复用。本轮优化的边界是：先修正确性，再做低风险热路径收敛；不改变默认 UI、模板结构和公开 API，不引入订阅、binding、dispatcher、缓存或 visual parent 泄露。

## 已落地

- `IsDescriptionVisible=False` 之前没有被模板消费，`No description` 示例仍会显示描述文本；现在模板应用后同步描述文本可见性，并覆盖运行时开关。
- `PresetImage`、`ImagePath`、`ImageSource` 的互斥检查从仅 `OnApplyTemplate()` 扩展到模板已应用后的运行时切换。
- 颜色 token 属性不再放在 `AffectsMeasure` 中；颜色只影响 SVG 内容，不应触发 Empty 自身 measure。
- built-in SVG 生成前会安全判断 solid brush，非 solid brush 不再强转崩溃。
- `Svg.Source` / `Svg.Path` 按当前图片来源互斥同步，并跳过相同值重复赋值，避免切换图片来源后留下旧值。
- `IsDescriptionVisible=True` 默认路径不主动扫描描述 `TextBlock`；只有隐藏描述时才缓存模板内的描述节点。

## 生命周期与泄露检查

本轮没有新增事件订阅、外部 observable、dispatcher 回调、全局缓存或 popup/visual 重挂载逻辑。新增的 `_descriptionTextBlock` 和 `_svg` 都是控件模板内子节点引用，并且在每次 `OnApplyTemplate()` 时重新取值；没有跨控件或跨窗口持有路径。

## 未保留的方案

- `Svg` / `Description` 按需创建：会改变模板结构，且收益只覆盖少数无图/无描述场景，本轮不做。
- built-in SVG 字符串缓存：会引入缓存生命周期和主题色失效问题，本轮没有保留。
- native renderer 替代 SVG：需要完全复刻现有 SVG 视觉细节，UI 风险高，本轮不做。
- 下游空态延迟创建：`ListView`、`DataGrid` 等是否默认创建 Empty 应在对应控件优化中单独处理。

## 控件级结果

命令：

```bash
dotnet run --no-build --framework net10.0 \
  --project tools/performances/AtomUI.Performance/AtomUI.Performance.csproj -- \
  --suite empty --count 1000
```

口径：Debug + Avalonia headless，单控件/小组合实例化、模板应用和 layout；这不是 Gallery 页面打开时间。

| Scenario | Before | After | Result |
| --- | ---: | ---: | ---: |
| `Empty.DefaultPreset` time | `0.992 ms/item` | `0.995 ms/item` | `0.30% slower` |
| `Empty.Simple.Small` time | `0.626 ms/item` | `0.543 ms/item` | `13.26% faster` |
| `Empty.Simple.Middle` time | `0.371 ms/item` | `0.328 ms/item` | `11.59% faster` |
| `Empty.Simple.Large` time | `0.273 ms/item` | `0.261 ms/item` | `4.40% faster` |
| `Empty.ImagePath` time | `0.497 ms/item` | `0.463 ms/item` | `6.84% faster` |
| `Empty.ImageSource` time | `0.152 ms/item` | `0.138 ms/item` | `9.21% faster` |
| `Empty.NoDescription` time | `0.272 ms/item` | `0.263 ms/item` | `3.31% faster` |
| `Empty.DescriptionOnly` time | `0.079 ms/item` | `0.081 ms/item` | `2.53% slower` |
| `Empty.GalleryShape` time | `2.552 ms/item` | `2.591 ms/item` | `1.53% slower` |
| `Empty.GalleryShape` alloc | `1242.6 KB/item` | `1236.1 KB/item` | `0.52% less` |

控件级创建套件没有证明结构性大幅提速：`Empty` 默认模板仍是 `StackPanel + Svg + TextBlock`，visual/root 保持 `4`。收益主要来自正确性修复、颜色属性不再触发 measure、运行时重复赋值减少，以及真实 Gallery 下的分配下降。

## EmptyShowCase 加载结果

命令：

```bash
dotnet run --no-build --framework net10.0 \
  --project tools/performances/AtomUI.GalleryPerformance/AtomUI.GalleryPerformance.csproj -- \
  --showcase empty --iterations 50 --warmup 5 --cold-iterations 10
```

口径：Debug + Avalonia headless，`1300x900`，从 AboutUs route 稳定后导航到真实 `EmptyShowCase`，等待 visual tree 和 layout 稳定。冷启动为 10 个独立进程样本，重复导航为 warmup 5 + measured 50。

| Metric | Before | After | Result |
| --- | ---: | ---: | ---: |
| Cold first navigation mean | `180.13 ms` | `180.43 ms` | `0.17% slower` |
| Cold first navigation median | `180.44 ms` | `181.24 ms` | `0.44% slower` |
| Cold first navigation P95 | `185.55 ms` | `184.75 ms` | `0.43% faster` |
| Cold alloc mean | `3770.76 KB` | `3640.27 KB` | `3.46% less` |
| Repeated navigation mean | `21.90 ms` | `21.32 ms` | `2.65% faster` |
| Repeated navigation median | `22.09 ms` | `22.20 ms` | `0.50% slower` |
| Repeated navigation P95 | `25.35 ms` | `24.79 ms` | `2.21% faster` |
| Repeated alloc mean | `2269.73 KB` | `2253.90 KB` | `0.70% less` |
| Visual nodes | `74` | `74` | no change |
| Svg nodes | `6` | `6` | no change |
| TextBlock nodes | `15` | `15` | no change |

结论：这次 Empty 本体优化符合“低风险修复”的预期，但不是一次大幅提速。真实 `EmptyShowCase` 的 repeated mean/P95 和分配下降，cold timing 基本持平；没有 visual tree 减重。

## 验证

```bash
dotnet run --framework net10.0 \
  --project tools/performances/AtomUI.Performance/AtomUI.Performance.csproj -- \
  --verify-empty-states

dotnet run --framework net10.0 \
  --project tools/performances/AtomUI.GalleryPerformance/AtomUI.GalleryPerformance.csproj -- \
  --showcase empty --iterations 50 --warmup 5 --cold-iterations 10
```

`--verify-empty-states` 覆盖描述可见性、图片来源互斥、运行时图片来源切换、`Svg.Source` / `Svg.Path` 清理和模板 `Svg` 复用。
