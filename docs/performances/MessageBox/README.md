# MessageBox 性能优化记录

## 范围

- 控件：`MessageBox`、`MessageBoxContent`，以及 `MessageBox` 打开后承载内容的 `Dialog` loading content wrapper。
- Gallery：真实 `ModalShowCase.axaml`，源码形态为 `ShowCaseItem=9`、`Button=22`、运行时 `MessageBox=7`。
- 工具：`tools/performances/AtomUI.Performance --suite messagebox`，`tools/performances/AtomUI.GalleryPerformance --showcase modal`。

本轮目标不是改变 Modal 的 UI 行为，而是继续压低 `MessageBox` 的隐形成本和生命周期风险：未打开不创建打开态重对象；同生命周期绑定不引入 disposable plumbing；loading skeleton 只在真正 loading 时承担成本。

## 主要瓶颈与风险

1. `MessageBox.AttachDialog()` 中多数 `BindUtils.RelayBind(...)` 是同生命周期子对象绑定，原写法需要额外 `IDisposable` 管理，增加 re-template/detach 生命周期复杂度。
2. `MessageBoxContent` 的 4 个 relay binding 生命周期和内部 content 完全一致，可以用 Avalonia `[!]` 绑定表达，不需要单独 `CompositeDisposable`。
3. `MessageBoxStyle.Normal` 从其他 style 切换回来时，style-provided icon 没有被清掉，属于正确性问题；这种问题优先级高于性能。
4. `Dialog`/`MessageBox` 打开后内容外层原来固定创建 `Skeleton`，非 loading 状态也承担 skeleton 控件结构成本，不符合“未使用功能不承担成本”。
5. lazy skeleton 必须有释放路径，覆盖 `IsLoading=true -> false -> true`、close/detach、re-template，避免为了性能引入资源泄露。

## 实施内容

| Phase | 内容 | 状态 |
| --- | --- | --- |
| Phase 0 | 建立 MessageBox suite、状态验证、ModalShowCase 长样本基线 | Done |
| Phase 1 | 同生命周期 relay binding 改为 Avalonia `[!]` 绑定，仅保留真正 two-way relay | Done |
| Phase 2 | 修复 `MessageBoxStyle.Normal` 不清理 style icon 的正确性问题 | Done |
| Phase 3 | `Dialog` loading skeleton 改为 `DialogLoadingContentPresenter` 按需创建 | Done |
| Phase 4 | lazy skeleton 增加 toggle 和 detach 释放验证 | Done |
| Phase 5 | 控件级/Gallery 实测、文档与清理 | Done |

本轮曾评估 `DialogButtonBox` 列表分配优化，但短样本出现可重复变慢风险，已撤回。性能优化不能用复杂度或回归换取不稳定收益。

## Gallery 加载对比

口径：Debug / Avalonia Headless / `1300x900`，`--cold-iterations 10 --warmup 30 --iterations 60 --timeout-ms 30000`。Before 使用上一轮 Dialog 优化完成后的同口径长样本，After 为本轮 MessageBox 完成后复测。

| Metric | Before | After | Improvement |
| --- | ---: | ---: | ---: |
| `ModalShowCase` cold first navigation mean | `211.09 ms` | `186.51 ms` | `11.64%` faster |
| `ModalShowCase` cold first navigation median | `206.47 ms` | `185.88 ms` | `9.97%` faster |
| `ModalShowCase` cold first navigation P95 | `236.84 ms` | `192.42 ms` | `18.76%` faster |
| `ModalShowCase` cold alloc mean | `8691.30 KB` | `8685.90 KB` | `0.06%` less KB |
| `ModalShowCase` repeated mean | `34.53 ms` | `32.11 ms` | `7.01%` faster |
| `ModalShowCase` repeated median | `34.11 ms` | `31.60 ms` | `7.36%` faster |
| `ModalShowCase` repeated P95 | `37.18 ms` | `34.42 ms` | `7.42%` faster |
| `ModalShowCase` repeated alloc mean | `7303.77 KB` | `7299.06 KB` | `0.06%` less KB |

运行时结构保持 `317 visuals / 100 logical / Dialog=7 / MessageBox=7`。这轮收益主要来自生命周期和打开态 loading skeleton 的按需化，Gallery 页面闭合态结构已经在上一轮 Dialog 优化中降过一次，所以本轮页面级耗时提升是小幅但没有回归。

## 控件级结果

最终 `messagebox` suite：

| Scenario | ms/item | KB/item | Visual/root |
| --- | ---: | ---: | ---: |
| `MessageBox.Closed.Information` | `0.033` | `7.1` | `1` |
| `MessageBox.Closed.Confirm` | `0.029` | `7.1` | `1` |
| `MessageBox.Closed.Normal` | `0.027` | `7.0` | `1` |
| `MessageBox.OpenWindow.Information.NoMotion` | `10.353` | `1110.0` | `10` |
| `MessageBox.OpenWindow.Confirm.NoMotion` | `8.047` | `1309.9` | `10` |
| `MessageBox.OpenWindow.Loading.NoMotion` | `5.360` | `1132.5` | `10` |

`OpenWindow.*` 场景用于验证首次打开链路和分配量；visual/root 只统计 suite root，不包含外部 `Dialog` window 内容，因此不用于结构结论。

`dialog` 回归套件也已复跑，`DialogButtonBox`、closed `MessageBox` 场景均可稳定执行，没有保留前面被撤回的 ButtonBox 分配优化。

## 正确性与泄露验证

新增并通过：

```bash
dotnet run --project tools/performances/AtomUI.Performance/AtomUI.Performance.csproj \
  --framework net10.0 --no-build -- --verify-messagebox-states --verify-dialog-states
```

覆盖内容：

- `MessageBoxStyle.Information -> Normal -> Warning` 会正确创建、清理和恢复 style icon。
- 本地自定义 `Icon` 不会被 style icon 更新覆盖。
- closed `MessageBox` 不提前 materialize 内部 `Dialog`。
- `OpenAsync()` 首次打开才创建内部 `Dialog`，`Cancel()` 后状态恢复，detach 后释放 lazy dialog。
- 非 loading `Dialog` 不创建 `Skeleton`。
- `IsLoading=false -> true -> false -> true` 会按需创建和释放 `Skeleton`。
- loading 状态 detach 后释放 lazy skeleton visual。

## 复现命令

```bash
dotnet build tools/performances/AtomUI.Performance/AtomUI.Performance.csproj --framework net10.0 --no-restore
dotnet run --project tools/performances/AtomUI.Performance/AtomUI.Performance.csproj --framework net10.0 --no-build -- --suite messagebox --count 80
dotnet run --project tools/performances/AtomUI.Performance/AtomUI.Performance.csproj --framework net10.0 --no-build -- --suite dialog --count 80
dotnet run --project tools/performances/AtomUI.Performance/AtomUI.Performance.csproj --framework net10.0 --no-build -- --verify-messagebox-states --verify-dialog-states
```

```bash
dotnet build tools/performances/AtomUI.GalleryPerformance/AtomUI.GalleryPerformance.csproj --framework net10.0 --no-restore
dotnet run --project tools/performances/AtomUI.GalleryPerformance/AtomUI.GalleryPerformance.csproj --framework net10.0 --no-build -- --showcase modal --cold-iterations 10 --warmup 30 --iterations 60 --timeout-ms 30000
```
