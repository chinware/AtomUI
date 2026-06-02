# Upload 性能评估

> 状态：已完成 structural-only 收口；3 个页面级候选均未通过收益门槛并已按 3-round budget 关闭；保留单图预览和运行期任务路径的分配 / 扫描收敛。

---

## 0. 结论

本轮页面级候选只保留 `AtomUI.GalleryPerformance` 的 `upload` showcase 映射，方便后续复测。后续追加的单图预览和运行期任务路径只减少临时集合 / 重复扫描 / 重复写入，不声明页面级 timing 收益；该项按 structural-only Done 收口。

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
| Directory upload file list initial capacity / folder picker result | dynamic growth | selected directory count | structural | 结构收益 | 目录上传临时文件列表按已选目录数预留下限容量 |
| Drop files list capacity / drop event | dynamic growth | exact capacity | structural | 分配更紧 | 按 `DataTransfer.Items.Count` 预分配 drop 文件列表 |
| Upload task lookup LINQ calls / progress-complete-fail-cancel-remove | 5 `FirstOrDefault(predicate)` | 0 LINQ calls | `(5 - 0) / 5` | 100.00% | 上传回调按 `TaskId` 查任务改为显式扫描，避免每次回调创建 predicate |
| Picture trigger lookup LINQ calls / load-listtype change | 2 `FirstOrDefault(predicate)` | 0 LINQ calls | `(2 - 0) / 2` | 100.00% | PictureCard / PictureCircle trigger 查找改为显式扫描 |
| CurrentTaskList snapshot `ToArray()` callsites / task list change | 1 | 0 | `(1 - 0) / 1` | 100.00% | 结构收益；任务列表快照改为 Count/indexer 复制，数组快照语义不变 |

说明：这是单图预览路径的结构性收益；不作为 `UploadShowCase` 页面导航收益证明。

### 2.1 追加结构优化：运行期任务路径

本轮继续收敛 Upload 实际上传中的小热路径，不改上传调度、列表显示、默认任务导入或 trigger 逻辑：

- 图片扩展名判断保留 `RegexOptions.IgnoreCase`，去掉每个文件的 `ToLowerInvariant()` 临时字符串，并补上 `RegexOptions.CultureInvariant` 维持 invariant 口径。
- `CurrentTaskList` 数组快照保留原语义，但复制前缓存 `TaskInfoList.Count`，避免 N 项快照过程中反复读取 Count。
- 上传进度回调只有第一次从非运行态进入运行态时写 `IsTaskRunning=true`；后续 progress 不再重复写同一个 direct property。
- 完成 / 失败 / 取消后检查是否仍有任务运行时，找到第一个 uploading 后立即结束扫描，并且状态不变时不再重复写 `IsTaskRunning`。

| 指标 | 优化前 | 优化后 | 公式 | 提升 | 结论 |
| --- | ---: | ---: | --- | ---: | --- |
| Image extension lowercase temp strings / uploaded file | 1 string | 0 strings | `(1 - 0) / 1` | 100.00% | 有效；大小写匹配交给 compiled regex |
| CurrentTaskList Count reads / 10-task snapshot | 12 reads | 1 read | `(12 - 1) / 12` | 91.67% | 结构收益；N 项通式为 `N + 2 -> 1` |
| Repeated IsTaskRunning writes / progress callback after first progress | 1 write | 0 writes | `(1 - 0) / 1` | 100.00% | 有效；运行态不变时跳过 property write |
| Task-running scan items / completion with 10 tasks and first task still uploading | 10 items | 1 item | `(10 - 1) / 10` | 90.00% | 数据相关收益；找到 uploading 后提前结束 |
| Unchanged IsTaskRunning writes / completion-fail-cancel check | 1 write | 0 writes | `(1 - 0) / 1` | 100.00% | 有效；状态不变时不触发 direct property write |

说明：这是运行期上传交互 structural-only 收益；没有新增 UploadShowCase 页面 timing 对比，不声明页面加载速度提升。

本次追加验证：

```bash
dotnet build -c Release -f net10.0 --no-restore tools/performances/AtomUI.Performance/AtomUI.Performance.csproj
```

结果：0 warning / 0 error。当前 performance harness 没有独立 `--verify-upload-states`。

### 2.2 追加结构优化：无文件 drop 路径

`UploadDefaultDropArea.HandleDrop()` 旧实现进入 drop 事件后会立即创建 `List<IStorageFile>`，即使拖入的数据里没有文件也会分配一个空列表。本轮改成发现第一个 `IStorageFile` 时再创建列表；无文件 drop 使用 `Array.Empty<IStorageFile>()` 作为事件参数，保持 `Files` 为非 null 只读列表。

| 指标 | 优化前 | 优化后 | 公式 | 提升 | 结论 |
| --- | ---: | ---: | --- | ---: | --- |
| Empty drop file-list allocations / drop event | 1 list | 0 lists | `(1 - 0) / 1` | 100.00% | 结构收益；无文件 drop 不再分配可增长 List |
| File drop list capacity / drop event | exact DataTransfer item count | exact DataTransfer item count | n/a | 0.00% | 有文件路径保留原容量策略和事件语义 |

说明：这是拖拽交互路径 structural-only 收益；没有新增 UploadShowCase 页面 timing 对比，不声明页面加载速度提升。

本轮追加验证：`git diff --check` passed；Debug net10、Release net10、Release net8 的 `AtomUI.Performance` build 均 0 warning / 0 error。当前 performance harness 没有独立 `--verify-upload-states`。

### 2.3 追加结构优化：空目录上传路径

目录上传旧实现会在 folder picker 返回后立即创建 `List<UploadFileInfo>`，即使用户取消选择、选择空目录，或所有目录顶层没有文件，也会分配一个空列表并调用空 enqueue。本轮改成发现第一个文件时再创建列表；没有实际文件时直接结束，保留有文件路径的预分配容量和上传顺序。

| 指标 | 优化前 | 优化后 | 公式 | 提升 | 结论 |
| --- | ---: | ---: | --- | ---: | --- |
| Empty directory upload list allocations / folder picker result | 1 list | 0 lists | `(1 - 0) / 1` | 100.00% | 结构收益；取消/空目录不再创建临时上传列表 |
| Empty directory upload enqueue calls / folder picker result | 1 no-op call | 0 calls | `(1 - 0) / 1` | 100.00% | 结构收益；无文件时不再进入空 enqueue |
| Non-empty directory upload order | directory/file enumeration order | unchanged | n/a | 0.00% | 有文件路径保持原上传顺序 |

说明：这是目录上传交互路径 structural-only 收益；没有新增 UploadShowCase 页面 timing 对比，不声明页面加载速度提升。

本轮追加验证同上：`git diff --check` passed；Debug net10、Release net10、Release net8 的 `AtomUI.Performance` build 均 0 warning / 0 error。

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

`Upload` 标记为 structural-only Done。后续只有在出现新的真实高频路径证据时再重开，例如实际上传进度刷新、预览弹窗首次打开、或大列表上传任务渲染。
