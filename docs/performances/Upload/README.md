# Upload 性能评估

> 路线图位置：[`../desktop-controls-optimization-roadmap.md`](../desktop-controls-optimization-roadmap.md) Tier 1 #5
> 状态：已建立 Gallery baseline；本轮 3 个页面级候选均未通过收益门槛；后续仅保留单图预览路径的 structural-only 分配收敛。

---

## 0. 结论

本轮页面级候选只保留 `AtomUI.GalleryPerformance` 的 `upload` showcase 映射，方便后续复测。后续追加的单图预览路径只减少临时集合，不声明页面级 timing 收益。

| 指标 | baseline |
| --- | ---: |
| Cold first navigation mean | 351.77 ms |
| Cold first navigation median | 347.01 ms |
| Cold first navigation P95 | 379.12 ms |
| Cold alloc mean | 17557.31 KB |
| Repeated navigation mean | 87.48 ms |
| Repeated navigation median | 84.67 ms |
| Repeated navigation P95 | 106.70 ms |
| Repeated alloc mean | 14650.15 KB |
| Runtime visuals | 687 |
| Runtime logical | 55 |

---

## 1. 已回滚候选

| 候选 | 结果 | 处理 |
| --- | --- | --- |
| `UploadTriggerContent` raw input 改 routed pointer handler | timing 退化 | 已回滚 |
| `CurrentTaskList` 使用稳定 `AvaloniaList` 代替 `ToArray()` | allocation 下降但 cold/repeated timing 更差 | 已回滚 |
| scheduler lazy creation | cold/repeated timing 更差 | 已回滚 |

按 SKILL 3-round budget，同一 scoped target 连续 3 轮无 primary speed metric 改善后停止，不继续堆叠候选。

---

## 2. 保留改动

`tools/performances/AtomUI.GalleryPerformance/Program.cs` 新增：

```csharp
["upload"] = new(
    "UploadShowCase",
    UploadViewModel.ID,
    "AtomUIGallery.ShowCases.Views.UploadShowCase",
    "controlgallery/AtomUIGallery/ShowCases/Views/DataEntry/UploadShowCase.axaml",
    stats => stats.ShowCaseItemCount >= 10 && stats.VisualCount > 0),
```

这是测量入口，不改变产品行为。

追加 structural-only runtime 改动：

| 指标 | 优化前 | 优化后 | 公式 | 提升 | 结论 |
| --- | ---: | ---: | --- | ---: | --- |
| PictureList 单图预览 Sources wrapper / FilePath change | 1 list | 1 fixed array | 结构替换 | 分配更紧 | 单元素 sources 不再创建可增长 List |
| PictureShapeList 单图预览 Sources wrapper / FilePath change | 1 list | 1 fixed array | 结构替换 | 分配更紧 | 单元素 sources 不再创建可增长 List |
| Directory upload file enumeration LINQ iterator / selected directory | 1 iterator | 0 iterators | `(1 - 0) / 1` | 100.00% | 结构收益；目录上传枚举文件时不再经 `.Select(...)` 包装 |
| Drop files list capacity / drop event | dynamic growth | exact capacity | structural | 分配更紧 | 按 `DataTransfer.Items.Count` 预分配 drop 文件列表 |

说明：这是单图预览路径的结构性收益；不作为 `UploadShowCase` 页面导航收益证明。

---

## 3. 复现命令

```bash
dotnet run --project tools/performances/AtomUI.GalleryPerformance/AtomUI.GalleryPerformance.csproj \
  -c Debug --framework net10.0 --no-build -- \
  --showcase upload --label baseline \
  --cold-iterations 10 --iterations 30 --warmup 5 --timeout-ms 30000 \
  --markdown /tmp/upload-showcase-baseline.md
```

---

## 4. 后续

`Upload` 暂不标记为性能 Done。下一轮只有在发现更明确的高频路径时再回来，例如实际上传进度刷新、预览弹窗首次打开、或大列表上传任务渲染。
