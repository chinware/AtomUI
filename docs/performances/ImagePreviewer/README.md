# ImagePreviewer 性能优化记录

日期：2026-05-17

## 结论

`ImagePreviewer` 的主要问题不是 visual tree 过重，而是关闭态提前解码了弹窗才需要的图片列表。真实 `ImagePreviewerShowCase` 只有 5 个例子，但包含大图、WebP、SVG 和自定义封面；旧实现会在 `ItemsSource` 设置时把完整预览列表加载到 `EffectiveSources`。

本轮完成低风险优化：

- 单个 `ImagePreviewer` 关闭态只加载当前可见封面，完整弹窗图片列表延迟到首次 `OpenDialog()` 前创建。
- `ImageGroupPreviewer` 保持关闭态加载全部封面，因为它本身需要显示多张缩略图。
- `ItemsSource`、`FallbackImageSrc`、`CoverImageSrc` 替换/清空时释放旧 `PreviewImageSource`。
- 关闭、detach、re-template 路径补齐资源释放和事件解绑，避免性能优化引入泄露。
- `ImagePreviewRenderer.Stretch` 修正为自身属性，并把内部 `Image/Svg.Stretch` 改为同生命周期 `[!]` 绑定。
- `ImagePreviewerTitleBar` 内部 toolbar 绑定改为 `[!]`，去掉不必要的 disposable binding。
- 修复 Gallery `ImagePreviewerShowCase` 激活清理未注册到 `disposables` 的问题。

未实施项：

- 未减少封面 visual tree。当前 Gallery 运行时仍是 129 个 visual，视觉结构没有变化。
- 未把弹窗窗口本身缓存复用。弹窗是交互路径成本，缓存会增加生命周期复杂度，本轮只做首次打开前的图片源按需加载。
- 未改变图片解码策略和显示质量，避免影响 UI 行为。

## 测试口径

控件级：

```bash
dotnet run --no-build --framework net10.0 \
  --project tools/performances/AtomUI.Performance/AtomUI.Performance.csproj \
  -- --suite imagepreviewer --count 60 \
  --markdown /tmp/atomui-imagepreviewer-control-current.md
```

状态与生命周期验证：

```bash
dotnet run --no-build --framework net10.0 \
  --project tools/performances/AtomUI.Performance/AtomUI.Performance.csproj \
  -- --verify-imagepreviewer-states
```

真实 Gallery：

```bash
dotnet run --no-build --framework net10.0 \
  --project tools/performances/AtomUI.GalleryPerformance/AtomUI.GalleryPerformance.csproj \
  -- --showcase imagepreviewer --warmup 6 --iterations 40 --cold-iterations 10 \
  --timeout-ms 45000 --label imagepreviewer-current \
  --markdown /tmp/atomui-imagepreviewer-gallery-current.md
```

环境：Debug / net10.0 / Avalonia Headless / Gallery 1300x900。

## 控件级结果

| 场景 | Before | After | 变化 |
| --- | ---: | ---: | ---: |
| `ImagePreviewer.Basic` ms/item | 0.848ms | 0.734ms | 快 13.44% |
| `ImagePreviewer.Basic` KB/item | 148.7KB | 147.6KB | 少 0.74% |
| `ImagePreviewer.Fallback` ms/item | 0.917ms | 0.732ms | 快 20.17% |
| `ImagePreviewer.Fallback` KB/item | 150.7KB | 146.5KB | 少 2.79% |
| `ImagePreviewer.MultiSource` ms/item | 0.885ms | 0.730ms | 快 17.51% |
| `ImagePreviewer.MultiSource` KB/item | 149.0KB | 146.3KB | 少 1.81% |
| `ImagePreviewer.CustomCover` ms/item | 1.144ms | 0.960ms | 快 16.08% |
| `ImageGroupPreviewer.TwoSvg` ms/item | 3.305ms | 3.096ms | 快 6.32% |
| `ImagePreviewer.GalleryShape` ms/item | 7.950ms | 6.544ms | 快 17.69% |
| `ImagePreviewer.GalleryShape` KB/item | 1414.6KB | 1403.0KB | 少 0.82% |
| `ImagePreviewer.GalleryShape` visual/root | 75 | 75 | 持平 |

解释：控件级收益主要来自单个 `ImagePreviewer` 不再关闭态创建完整 dialog source list。`ImageGroupPreviewer` 仍要显示两张 SVG 封面，所以保留 eager cover materialization，但 renderer 和生命周期修复仍带来改善。

## ImagePreviewerShowCase 真实加载结果

`ImagePreviewerShowCase.axaml` 源形态：5 个 `ShowCaseItem`，4 个 `ImagePreviewer`，1 个 `ImageGroupPreviewer`。运行时封面形态保持不变：4 个 bitmap image、2 个 svg、14 个 text block。

页面加载时间使用同一策略：`cold-iterations=10`、`warmup=6`、`iterations=40`。

| 指标 | Before | After | 变化 |
| --- | ---: | ---: | ---: |
| Cold first navigation mean | 180.28ms | 147.48ms | 快 18.19% |
| Cold first navigation median | 172.52ms | 147.40ms | 快 14.56% |
| Cold first navigation P95 | 223.28ms | 151.65ms | 快 32.08% |
| Repeated mean | 33.56ms | 30.61ms | 快 8.79% |
| Repeated median | 33.76ms | 30.37ms | 快 10.04% |
| Repeated P95 | 42.02ms | 35.49ms | 快 15.54% |
| Repeated alloc mean | 2797.28KB | 2787.61KB | 少 0.35% |
| Visual nodes | 129 | 129 | 持平 |
| Logical nodes | 12 | 12 | 持平 |

解释：

- 视觉树不变，收益来自关闭态少做图片源列表解码和少保留不用的 `PreviewImageSource`。
- Gallery 页面本身已经比较轻，repeated mean 从 33.56ms 到 30.61ms，实际少约 2.95ms。
- 冷启动多样本 P95 改善明显，说明去掉提前图片列表加载后尾部波动更低。
- 没有保留可重复性能回退；控件级和真实 Gallery 主指标均改善。

## 已实施 Phase

### Phase 0：基线与观测

已完成：

- 新增 `tools/performances/AtomUI.Performance/Suites/ImagePreviewer/ImagePreviewerScenarios.cs`。
- 新增真实 Gallery 路由：`AtomUI.GalleryPerformance --showcase imagepreviewer`。
- 基线覆盖 Basic、Fallback、MultiSource、CustomCover、ImageGroupPreviewer 和 GalleryShape。

### Phase 1：资源生命周期修复

已完成：

- `ItemsSource=null` 或替换时释放旧 `EffectiveSources`。
- `FallbackImageSrc` 替换时释放旧 fallback source。
- `CoverImageSrc` 替换、清空、切回 `ItemsSource` 封面时释放旧 owned cover source。
- `OnDetachedFromVisualTree` 关闭弹窗并释放 effective sources 和 owned cover。
- 弹窗打开期间替换 `ItemsSource` 时继续保持完整 dialog source list，避免按关闭态策略误清空弹窗内容。
- Gallery `ImagePreviewerShowCase` 的 `Disposable.Create(...)` 已注册到 `disposables`。

### Phase 2：单个 ImagePreviewer 关闭态按需加载

已完成：

- `ImagePreviewer` 覆盖 `ShouldEagerLoadSourcesOnItemsSourceChanged=false`。
- 关闭态只 materialize `EffectiveCoverImage`。
- `OpenDialog()` 前调用 `EnsureDialogSources()`，首次打开时补齐完整 `EffectiveSources`。
- 完整源列表创建后，非自定义封面会切换复用 `EffectiveSources[0]`，并释放独立封面 source。

### Phase 3：模板事件与绑定收敛

已完成：

- `ImagePreviewBaseToolbar.OnApplyTemplate()` 重新套模板前解绑旧按钮事件。
- `ImageViewer.OnApplyTemplate()` 重新套模板前解绑旧上一张/下一张按钮事件。
- `ImagePreviewerTitleBar` 内部 toolbar 绑定改为 `[!]`。
- `ImagePreviewRenderer` 内部 `Image/Svg.Stretch` 改为 `[!]`，移除不必要的 disposable binding。

### Phase 4：Renderer 正确性修复

已完成：

- `ImagePreviewRenderer.Stretch` getter/setter 从错误的 `Image.StretchProperty` 改为自身 `StretchProperty`。
- source 切换时移除旧 child visual/logical parent，避免子控件残留。

### Phase 5：状态与泄露验证

已完成：

- 单个 `ImagePreviewer` 关闭态不创建 dialog source list。
- 打开弹窗后完整 source list 创建，关闭后状态正常回落。
- 打开弹窗期间替换 `ItemsSource` 仍保留完整 dialog source list。
- `ImageGroupPreviewer` 关闭态保留全部封面 source。
- `ItemsSource` 清空释放 sources 与 cover。
- `CoverImageSrc` 替换、清空、回退到 `ItemsSource` 封面路径验证通过。

### Phase 6：最终对比

本轮符合预期：

- 控件级 GalleryShape 快 17.69%，真实 Gallery repeated mean 快 8.79%。
- 由于 visual tree 不变，分配改善有限；这符合本轮“少解码/少保留不用图片源”的优化边界。
- 未增加弹窗缓存等复杂生命周期逻辑，避免为低收益引入更难维护的状态机。
