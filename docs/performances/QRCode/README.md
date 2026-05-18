# QRCode 性能优化记录

## 范围

- 控件：`QRCode`、`AbstractQRCode`。
- Gallery：真实 `QRCodeShowCase.axaml`，源码形态为 `ShowCaseItem=8`、`QRCode=12`、`Button=4`、`AntDesignIconProvider=3`、`LineEdit=1`。
- 工具：`tools/performances/AtomUI.Performance --suite qrcode`，`tools/performances/AtomUI.GalleryPerformance --showcase qrcode`。

## 主要瓶颈与风险

1. `AbstractQRCode` 原来每次生成二维码都走 `SKSurface -> SKImage -> PNG encode -> Bitmap decode`，大量实例同屏时 CPU 和分配都被 PNG 编解码放大。
2. `Bitmap` 原来参与 `AffectsMeasure`，二维码内容、颜色、纠错等级等变化会把本应局部更新的成本推到 measure 路径。
3. 模板默认创建 icon overlay、loading、expired、scanned 和 custom content presenter。普通 Active 且无 icon 的二维码也承担隐藏状态层成本。
4. `QRCodeShowCase` 同屏有 12 个二维码，其中 5 个是 Active 且无状态层，2 个无 icon。未使用功能提前创建不符合“不使用的功能不承担成本”。
5. status/icon 层从 XAML 移到按需创建后，必须保持旧模板名称、主题 selector 行为、刷新事件语义，并在 status 切换、re-template、detach 时释放 visual parent、templated parent、事件和 binding。

## 实施内容

| Phase | 内容 | 状态 |
| --- | --- | --- |
| Phase 0 | 建立 `qrcode` 控件级 suite、真实 `QRCodeShowCase` route 和状态/生命周期验证 | Done |
| Phase 1 | 位图生成改为 Skia 像素直接创建 Avalonia `Bitmap`，移除 PNG encode/decode | Done |
| Phase 2 | 增加 render key 缓存，`Value` 为空时清空位图，detach 时释放 bitmap | Done |
| Phase 3 | 移除 `BitmapProperty` 的 measure invalidation，模板尺寸继续由 `Size` 控制 | Done |
| Phase 4 | icon overlay 按需创建，`Icon=null` 不创建 `ImageFrame` | Done |
| Phase 5 | loading/expired/scanned/default custom status 层按需创建，Active 不创建隐藏状态树 | Done |
| Phase 6 | 释放路径覆盖 visual parent、templated parent、custom content、语言 binding、refresh button 事件和 icon 引用 | Done |
| Phase 7 | 控件级/Gallery 复测、文档和清理 | Done |

## 控件级结果

口径：Debug / Avalonia Headless / `--suite qrcode --count 80`。Before 为本轮 QRCode 修改前基线；After 为当前工作区优化后结果。

| Scenario | Before | After | Improvement |
| --- | ---: | ---: | ---: |
| `QRCode.Basic` | `4.107 ms/item`, `309.6 KB/item`, `19 visuals` | `0.567 ms/item`, `63.9 KB/item`, `7 visuals` | 快 `86.20%`，内存少 `79.36%`，少 `12` visuals |
| `QRCode.WithIcon` | `4.332 ms/item`, `311.8 KB/item`, `19 visuals` | `0.722 ms/item`, `76.0 KB/item`, `9 visuals` | 快 `83.33%`，内存少 `75.63%`，少 `10` visuals |
| `QRCode.Status.Loading` | `4.810 ms/item`, `415.3 KB/item`, `27 visuals` | `1.649 ms/item`, `180.0 KB/item`, `17 visuals` | 快 `65.72%`，内存少 `56.66%`，少 `10` visuals |
| `QRCode.Status.Expired` | `4.979 ms/item`, `489.9 KB/item`, `28 visuals` | `2.958 ms/item`, `387.2 KB/item`, `20 visuals` | 快 `40.59%`，内存少 `20.96%`，少 `8` visuals |
| `QRCode.Status.Scanned` | `2.773 ms/item`, `313.5 KB/item`, `19 visuals` | `0.582 ms/item`, `80.2 KB/item`, `9 visuals` | 快 `79.01%`，内存少 `74.42%`，少 `10` visuals |
| `QRCode.Status.CustomContent` | `16.117 ms/item`, `1321.2 KB/item`, `79 visuals` | `3.587 ms/item`, `570.4 KB/item`, `46 visuals` | 快 `77.74%`，内存少 `56.83%`，少 `33` visuals |
| `QRCode.EmptyValue` | `0.532 ms/item`, `302.7 KB/item`, `19 visuals` | `0.173 ms/item`, `62.4 KB/item`, `7 visuals` | 快 `67.48%`，内存少 `79.38%`，少 `12` visuals |
| `QRCode.GalleryShape.QRCodeShowCase` | `55.457 ms/item`, `5679.8 KB/item`, `335 visuals` | `16.868 ms/item`, `2870.2 KB/item`, `206 visuals` | 快 `69.58%`，内存少 `49.47%`，少 `129` visuals |

结构收益最核心：默认 Active QRCode 从 `19 visuals` 降到 `7 visuals`，状态层只在 `Status != Active` 时创建，icon 层只在 `Icon != null` 时创建。

## Gallery 加载对比

口径：Debug / Avalonia Headless / `1300x900` / `--cold-iterations 10 --warmup 6 --iterations 20 --timeout-ms 30000`。Before 来自本轮修改前 detached worktree，After 来自当前工作区优化后代码。

| Metric | Before | After | Improvement |
| --- | ---: | ---: | ---: |
| `QRCodeShowCase` cold first navigation mean | `449.72 ms` | `423.92 ms` | 快 `25.80 ms`，`5.74%` |
| `QRCodeShowCase` cold first navigation median | `449.18 ms` | `409.13 ms` | 快 `40.05 ms`，`8.92%` |
| `QRCodeShowCase` cold first navigation P95 | `483.03 ms` | `513.17 ms` | 慢 `30.14 ms`，`6.24%` |
| `QRCodeShowCase` cold alloc mean | `12409.25 KB` | `9386.80 KB` | 少 `3022.45 KB`，`24.36%` |
| `QRCodeShowCase` repeated mean | `125.12 ms` | `91.58 ms` | 快 `33.54 ms`，`26.81%` |
| `QRCodeShowCase` repeated median | `114.84 ms` | `65.13 ms` | 快 `49.71 ms`，`43.29%` |
| `QRCodeShowCase` repeated P95 | `168.39 ms` | `160.25 ms` | 快 `8.14 ms`，`4.83%` |
| `QRCodeShowCase` repeated alloc mean | `9831.66 KB` | `6943.57 KB` | 少 `2888.09 KB`，`29.37%` |
| Runtime visuals | `436` | `307` | 少 `129`，`29.59%` |
| Runtime Image/TextBlock/Button | `24 / 55 / 16` | `14 / 33 / 5` | 少 `10 / 22 / 11` |

用户可见效果：真实 `QRCodeShowCase` repeated open 从约 `125ms` 降到约 `92ms`，少约 `34ms`；冷启动均值少约 `26ms`，但 cold P95 在最终复测里仍有一次高尾样本，暂不把 cold P95 作为已改善结论。页面剩余成本主要来自 8 个 ShowCaseItem、LineEdit、按钮、真实 Gallery 布局以及仍然需要实际生成的二维码位图。

## 正确性与泄露验证

新增并通过：

```bash
dotnet run --project tools/performances/AtomUI.Performance/AtomUI.Performance.csproj \
  --framework net10.0 --no-build -- --verify-qrcode-states
```

覆盖内容：

- `Value` 非空时创建 bitmap，重复 `ApplyTemplate` 不重复生成同 render key 位图。
- `Value` 变空时清空 bitmap，重新变为非空后恢复生成。
- Active 状态不创建 default loading/expired/scanned layer。
- status 切换会释放旧状态层并清理 visual parent。
- custom status content 只在对应 status 激活时 parent 到 `ContentPresenter`，恢复 Active 后释放。
- `Icon=null` 不创建 `ImageFrame`，设置 icon 后创建，清空 icon 后释放。
- expired refresh button 在 re-template 后不会重复订阅，移除 status 后释放事件和 visual parent。

## 复现命令

```bash
dotnet build tools/performances/AtomUI.Performance/AtomUI.Performance.csproj --framework net10.0 --no-restore
dotnet run --project tools/performances/AtomUI.Performance/AtomUI.Performance.csproj --framework net10.0 --no-build -- --verify-qrcode-states
dotnet run --project tools/performances/AtomUI.Performance/AtomUI.Performance.csproj --framework net10.0 --no-build -- --suite qrcode --count 80
```

```bash
dotnet build tools/performances/AtomUI.GalleryPerformance/AtomUI.GalleryPerformance.csproj --framework net10.0 --no-restore
dotnet run --project tools/performances/AtomUI.GalleryPerformance/AtomUI.GalleryPerformance.csproj --framework net10.0 --no-build -- --showcase qrcode --cold-iterations 10 --warmup 6 --iterations 20 --timeout-ms 30000
```

`GalleryPerformance` 构建仍有既有 warning：`DataGridColumn._clipboardContentBinding` 未使用，和本轮 QRCode 优化无关。
