# Dialog / MessageBox 性能优化

本轮目标是收敛 `Dialog` / `MessageBox` 的隐藏成本和生命周期风险。原则不变：未打开的弹层不承担打开态重型结构成本，所有新增 binding、事件订阅、lazy visual 都必须有释放路径。

## Phase 0 基线

- 日期：2026-05-16
- 配置：Debug / Avalonia Headless / net10.0
- 控件级工具：`tools/performances/AtomUI.Performance --suite dialog`
- Gallery 工具：`tools/performances/AtomUI.GalleryPerformance --showcase modal`

### 真实 Gallery 基线

`ModalShowCase.axaml` 的源形态包含 9 个 `ShowCaseItem`、22 个 `Button`、7 个运行时 `MessageBox`，闭合状态稳定后有 14 个 `Dialog` visual，其中 7 个来自 `MessageBox` 模板提前创建的内部 `Dialog`。

| 指标 | Before |
| --- | ---: |
| Cold first navigation | 209.34ms |
| Repeated mean | 55.02ms |
| Repeated median | 52.75ms |
| Repeated P95 | 64.25ms |
| Repeated alloc mean | 7648.64KB |
| Visuals | 324 |
| Dialog runtime count | 14 |
| MessageBox runtime count | 7 |

### 关键瓶颈与风险

1. `MessageBox` 闭合状态通过 `MessageBoxTheme.axaml` 固定创建内部 `Dialog`，ShowCase 里 7 个 `MessageBox` 直接多出 7 个 `Dialog` hidden cost。
2. `MessageBox.OnApplyTemplate()` 创建 `MessageBoxContent` 后的 4 个 `BindUtils.RelayBind(...)` 没保存 `IDisposable`，re-template/detach 后无法释放。
3. `OverlayDialogHost` 构造函数固定创建 modal mask，非 modal host 也承担成本；mask 绑定窗口圆角时未保存 disposable。
4. `OverlayDialogHostTheme` 固定创建 `OverlayDialogResizer`，即使 `IsResizable=false` 也会创建 8 个 resize handle。
5. `DialogButtonBox` 同步按钮分组时手动清 `VisualParent/LogicalParent`，并且在重新添加前才清 panel，headless 下会触发 visual parent 异常；剩余按钮循环还有重复添加风险。
6. `DialogStandardButtons.Count` 每次遍历 `Enum.GetValues()`，在 footer 可见性计算中属于不必要的热路径成本。

## 实施内容

| Phase | 内容 | 状态 |
| --- | --- | --- |
| Phase 1 | `MessageBox` RelayBind 生命周期修复 | Done |
| Phase 2 | `MessageBox` 内部 `Dialog` 改为首次打开按需创建 | Done |
| Phase 3 | `OverlayDialogMask` 按 modal 打开态创建，圆角 binding 保存并关闭释放 | Done |
| Phase 4 | `OverlayDialogResizer` 改为 `IsResizable && Normal` 时按需创建/释放 | Done |
| Phase 5 | `DialogButtonBox` 父级同步修复，去掉手动 visual/logical parent 清理 | Done |
| Phase 6 | `DialogStandardButtons.Count` 改为 bit count | Done |
| Phase 7 | 增加 Dialog 专用性能套件、ModalShowCase Gallery 统计、状态验证和文档 | Done |

## 最终结果

### 控件级结构结果

`MessageBox` 闭合态已经不再创建内部 `Dialog`、`DialogButtonBox`、`MessageBoxContent`：

| 场景 | ms/item | KB/item | Visual/root | Dialog/root | MessageBox/root | MessageBoxContent/root |
| --- | ---: | ---: | ---: | ---: | ---: | ---: |
| MessageBox.Closed.Information | 0.032 | 8.0 | 1 | 0 | 1 | 0 |
| MessageBox.Closed.Confirm | 0.024 | 7.9 | 1 | 0 | 1 | 0 |
| MessageBox.Closed.Loading | 0.030 | 8.1 | 1 | 0 | 1 | 0 |

`DialogButtonBox` 父级异常已修复，独立 suite 可稳定运行：

| 场景 | ms/item | KB/item | Visual/root | DialogButton/root |
| --- | ---: | ---: | ---: | ---: |
| DialogButtonBox.OkCancel | 3.732 | 504.1 | 25 | 2 |
| DialogButtonBox.AllStandard | 19.073 | 2467.5 | 97 | 11 |
| DialogButtonBox.Custom | 2.740 | 718.1 | 33 | 3 |

### Gallery ShowCase 加载对比

短样本 `warmup 3 / iterations 10` 曾出现 after repeated mean 变慢，这个结论不合格，不能作为优化验收口径。后续统一改用相同稳态口径：warmup 20，iterations 30，`1300x900` headless。

| 指标 | Before | After | 变化 |
| --- | ---: | ---: | ---: |
| Repeated mean | 37.03ms | 34.67ms | +6.37% |
| Repeated median | 34.95ms | 33.96ms | +2.83% |
| Repeated P95 | 47.42ms | 39.80ms | +16.07% |
| Repeated alloc mean | 7639.10KB | 7307.00KB | +4.35% |
| Visuals | 324 | 317 | +2.16% |
| Dialog runtime count | 14 | 7 | +50.00% |

`Cold first navigation` 仍是单样本，当前工具不适合用一次进程内 cold 值做严格结论；后续需要增加多进程 cold-run 统计后再作为硬指标。当前可确认的 Gallery 收益是稳态 repeated、结构和分配：闭合 `MessageBox` 不再付内部 `Dialog` 成本，页面少 7 个 visual，分配下降约 332KB。

## 正确性与泄露验证

已增加并通过：

```bash
dotnet run --no-build --framework net10.0 --project tools/performances/AtomUI.Performance/AtomUI.Performance.csproj -- --verify-dialog-states
```

覆盖内容：

- closed `MessageBox` 不创建内部 `Dialog`，重复 `ApplyTemplate()` 也不物化。
- `MessageBox.OpenAsync()` 首次打开才创建内部 `Dialog`，关闭后状态恢复，detach 后释放 lazy `Dialog`。
- `Dialog.OpenAsync()` / `Reject()` 后 host 创建、关闭和 `IsOpen` 状态恢复。
- `DialogButtonBox` `Ok/Cancel -> NoButton -> Ok/Cancel` 不重复按钮，不遗留父级异常。
- modal overlay 打开时按需创建 mask；关闭后释放 mask 字段和圆角 binding。
- `IsResizable=false -> true -> false` 时按需创建并移除 `OverlayDialogResizer`。

## 结论

本轮符合“未使用功能不承担成本”和“不能引入资源泄露”的目标，但 Gallery 页面 timing 收益没有稳定体现。`ModalShowCase` 的可见收益主要是结构减重和分配下降；如果后续要继续压 repeated timing，应该优先分析 `ShowCaseItem`、Gallery route 稳定流程、Button/TextBlock 固定成本，而不是继续在闭合 `MessageBox` 上做小改。

## 复现命令

```bash
dotnet build tools/performances/AtomUI.Performance/AtomUI.Performance.csproj --framework net10.0 --no-restore
dotnet run --no-build --framework net10.0 --project tools/performances/AtomUI.Performance/AtomUI.Performance.csproj -- --suite dialog --count 30
dotnet run --no-build --framework net10.0 --project tools/performances/AtomUI.Performance/AtomUI.Performance.csproj -- --verify-dialog-states
```

```bash
dotnet build tools/performances/AtomUI.GalleryPerformance/AtomUI.GalleryPerformance.csproj --framework net10.0 --no-restore
dotnet run --no-build --framework net10.0 --project tools/performances/AtomUI.GalleryPerformance/AtomUI.GalleryPerformance.csproj -- --showcase modal --warmup 3 --iterations 10 --timeout-ms 30000
```
