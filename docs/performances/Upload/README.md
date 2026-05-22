# Upload 性能评估

> 路线图位置：[`../desktop-controls-optimization-roadmap.md`](../desktop-controls-optimization-roadmap.md) Tier 1 #5
> 状态：已建立 Gallery baseline；本轮 3 个候选均未通过收益门槛，未保留运行时代码改动。

---

## 0. 结论

本轮只保留 `AtomUI.GalleryPerformance` 的 `upload` showcase 映射，方便后续复测。`Upload` 运行时代码恢复到 baseline 状态。

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
