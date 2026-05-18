# PopupConfirm 性能优化记录

## 范围

- 控件：`PopupConfirm`、`PopupConfirmFlyout`、`PopupConfirmContainer`。
- Gallery：真实 `PopupConfirmShowCase.axaml`，源码形态为 `PopupConfirm=15`、`Button=15`、`ShowCaseItem=4`、`AntDesignIconProvider=1`。
- 工具：`tools/performances/AtomUI.Performance --suite popupconfirm`，`tools/performances/AtomUI.GalleryPerformance --showcase popupconfirm`。

本轮重点不是重复优化 closed popup shell。这个成本已经在 Flyouts 轮次完成：关闭态不再创建 `Popup` shell 和 popup child。本轮继续处理首次打开后的 `PopupConfirmContainer` 隐性成本和生命周期风险。

## 主要瓶颈与风险

1. `PopupConfirmContainer` 模板固定创建 `PART_CancelButton`，`IsShowCancelButton=false` 时仍然承担一个 `Button`、`WaveSpiritDecorator` 和相关模板成本。
2. 模板固定创建 `PART_Content`，`ConfirmContent=null` 时仍有隐藏 `ContentPresenter` 和模板绑定成本。
3. `PART_OkButton`、`PART_CancelButton` 的点击事件需要在 template replacement 和 optional slot 移除时有明确解绑路径。
4. Popup 内容会在关闭后 detach，并可能在下一次打开时复用。不能把普通 detach 当作销毁，否则二次打开会丢按钮、内容或事件。
5. 同生命周期的懒创建子控件属性同步应优先使用 Avalonia `[!]` 绑定，避免不必要的 disposable plumbing 和手动同步复杂度。

## 实施内容

| Phase | 内容 | 状态 |
| --- | --- | --- |
| Phase 0 | 建立 `popupconfirm` 专用 suite、真实 `PopupConfirmShowCase` 基线和状态验证 | Done |
| Phase 1 | `PART_CancelButton` 按 `IsShowCancelButton` 懒创建/释放 | Done |
| Phase 2 | `PART_Content` 按 `ConfirmContent` 懒创建/释放 | Done |
| Phase 3 | 子控件同步改为 `[!]` 绑定，减少手动同步代码 | Done |
| Phase 4 | 生命周期验证覆盖 slot toggle、detach/reattach、事件解绑和 Flyout lazy shell | Done |
| Phase 5 | 控件级/Gallery 实测、文档和清理 | Done |

## 控件级结果

口径：Debug / Avalonia Headless / `--suite popupconfirm --count 80`。Before 为本轮 PopupConfirm 专项代码修改前的同 suite 基线；After 为当前工作区最终结果。

| Scenario | Before | After | Improvement |
| --- | ---: | ---: | ---: |
| `PopupConfirm.Container.Basic` | `5.573 ms/item`, `625.6 KB/item`, `27 visuals` | `3.338 ms/item`, `618.3 KB/item`, `27 visuals` | 快 `40.10%`，内存少 `1.17%` |
| `PopupConfirm.Container.TitleOnly` | `4.368 ms/item`, `603.4 KB/item`, `26 visuals`, `CP=3` | `2.341 ms/item`, `587.4 KB/item`, `25 visuals`, `CP=2` | 快 `46.41%`，少 `1` ContentPresenter |
| `PopupConfirm.Container.NoCancel` | `2.865 ms/item`, `528.8 KB/item`, `20 visuals`, `Button=2` | `1.634 ms/item`, `391.3 KB/item`, `19 visuals`, `Button=1` | 快 `42.97%`，内存少 `26.00%`，少 `1` Button |
| `PopupConfirm.GalleryShape.PopupConfirmShowCase` | `14.690 ms/item`, `3999.6 KB/item`, `152 visuals` | `13.922 ms/item`, `4002.1 KB/item`, `152 visuals` | 快 `5.23%`，结构不变 |

关闭态 `PopupConfirm.Closed.*` 结构保持 `10 visuals / 3 logical`。本轮没有再改 closed shell 路径；关闭态微基准耗时波动较大，不作为本轮主结论。

## Gallery 加载对比

口径：Debug / Avalonia Headless / `1300x900` / `--cold-iterations 10 --warmup 6 --iterations 20 --timeout-ms 30000`。Before 来自临时 detached worktree `HEAD`，After 来自当前工作区最终代码。

| Metric | Before | After | Improvement |
| --- | ---: | ---: | ---: |
| `PopupConfirmShowCase` cold first navigation mean | `174.79 ms` | `152.66 ms` | 快 `22.13 ms`，`12.66%` |
| `PopupConfirmShowCase` cold first navigation median | `174.15 ms` | `148.27 ms` | 快 `25.88 ms`，`14.86%` |
| `PopupConfirmShowCase` cold first navigation P95 | `200.20 ms` | `174.99 ms` | 快 `25.21 ms`，`12.59%` |
| `PopupConfirmShowCase` cold alloc mean | `6258.91 KB` | `6259.18 KB` | 基本持平 |
| `PopupConfirmShowCase` repeated mean | `45.21 ms` | `40.42 ms` | 快 `4.79 ms`，`10.60%` |
| `PopupConfirmShowCase` repeated median | `44.84 ms` | `34.03 ms` | 快 `10.81 ms`，`24.11%` |
| `PopupConfirmShowCase` repeated P95 | `65.01 ms` | `61.02 ms` | 快 `3.99 ms`，`6.14%` |
| `PopupConfirmShowCase` repeated alloc mean | `5023.31 KB` | `5029.30 KB` | 基本持平 |
| Runtime shape | `190 visuals / 52 logical` | `190 visuals / 52 logical` | 页面闭合态结构不变 |

用户可见效果：`PopupConfirmShowCase` 页面打开时间在同口径样本下有约 `5-22ms` 改善。页面闭合态 visual tree 不变，本轮真正的结构收益在用户首次打开 `PopupConfirm` 之后，尤其是不带 cancel 或不带 content 的 popup 内容。

## 正确性与泄露验证

新增并通过：

```bash
dotnet run --project tools/performances/AtomUI.Performance/AtomUI.Performance.csproj \
  --framework net10.0 --no-build -- --verify-popupconfirm-states --verify-flyout-states
```

覆盖内容：

- 关闭态 `PopupConfirm` 不创建 `Popup` shell 和 popup child。
- `ConfirmContent=null` 时不创建 `PART_Content`，恢复 content 后能重新创建。
- `IsShowCancelButton=false` 时不创建 `PART_CancelButton`，恢复后能重新创建。
- 被移除的 cancel button 不保留 click handler。
- popup content 普通 detach/reattach 后保留按钮、内容和事件，避免二次打开 UI 丢失。
- OK/Cancel 点击仍然触发 `Confirmed`、`Cancelled` 和 `PopupClick`。

## 复现命令

```bash
dotnet build tools/performances/AtomUI.Performance/AtomUI.Performance.csproj --framework net10.0 --no-restore
dotnet run --project tools/performances/AtomUI.Performance/AtomUI.Performance.csproj --framework net10.0 --no-build -- --verify-popupconfirm-states --verify-flyout-states
dotnet run --project tools/performances/AtomUI.Performance/AtomUI.Performance.csproj --framework net10.0 --no-build -- --suite popupconfirm --count 80
```

```bash
dotnet build tools/performances/AtomUI.GalleryPerformance/AtomUI.GalleryPerformance.csproj --framework net10.0 --no-restore
dotnet run --project tools/performances/AtomUI.GalleryPerformance/AtomUI.GalleryPerformance.csproj --framework net10.0 --no-build -- --showcase popupconfirm --cold-iterations 10 --warmup 6 --iterations 20 --timeout-ms 30000
```

`GalleryPerformance` 构建仍有既有 warning：`DataGridColumn._clipboardContentBinding` 未使用，和本轮 PopupConfirm 优化无关。
