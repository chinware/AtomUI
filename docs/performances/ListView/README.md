# ListView 性能优化

`ListView` 是 Data Display 分类下的高频列表控件。本轮优化边界是：不改变选择、分组、过滤、排序、分页、空状态、动画和模板视觉语义；所有新增或调整的事件、绑定、枚举器和 lazy visual 都必须有释放路径。

真实 Gallery 场景 `ListShowCase.axaml` 当前包含：

- `8` 个 `ListView` 示例：基础、选择、分组、禁用项、空状态、过滤、排序、分页。
- `3` 个 `ListBox` 示例。
- `1` 个 `SearchEdit`。
- `11` 个 `ShowCaseItem`。

## 本轮执行

| Phase | 状态 | 结果 |
| --- | --- | --- |
| Phase 0 基线与观测 | Done | 建立 `ListView` 控件级基线与真实 `ListShowCase` 加载基线 |
| Phase 1 拆出专用套件 | Done | 新增 `tools/performances/AtomUI.Performance/Suites/ListView`，从 `ListBox` 套件移出 ListView 场景 |
| Phase 2 Pagination 生命周期与同步 | Done | 修正 `PaginationVisibility` 错绑到 `IsMotionEnabled`；替换 pagination 时释放旧事件和 binding |
| Phase 3 Selection/TextSearch 生命周期 | Done | detach 时停止 text search timer；替换 selection model 时释放 `LostSelection` 订阅 |
| Phase 4 Sort/Filter/Group 刷新收敛 | Done | ItemsSource 替换时用 `DeferRefresh()` 合并刷新；默认无排序时不触碰 `SortDescriptions` |
| Phase 5 ListCollectionView 枚举与分配 | Done | 分页枚举不再复制临时 List；source 复制预分配容量；tracking enumerator 替换和 dispose 路径补齐 |
| Phase 6 状态与泄露验证 | Done | 新增 `--verify-listview-states`，覆盖 selected indicator、filter、empty、pagination、selection model lifecycle |
| Phase 7 Gallery 真实场景验证 | Done | 使用真实 `ListShowCase` 跑 cold 多样本和 repeated 导航对比 |

## 控件级结果

口径：Debug / net10.0 / Avalonia Headless，`--suite listview --count 60`。Before 来自 Phase 0 baseline，After 为本轮最终结果 `/private/tmp/atomui-listview-final.md`。

| 场景 | Before | After | 提升 |
| --- | ---: | ---: | ---: |
| `ListView.Default.Items20` | `4.113ms/item` | `3.103ms/item` | 快 `24.56%` |
| `ListView.SelectedIndicator.Items20` | `3.722ms/item` | `3.649ms/item` | 快 `1.96%` |
| `ListView.Disabled.Items20` | `2.817ms/item` | `2.188ms/item` | 快 `22.33%` |
| `ListView.Empty` | `1.361ms/item` | `1.120ms/item` | 快 `17.71%` |
| `ListView.Grouped.Items20` | `3.619ms/item` | `1.759ms/item` | 快 `51.40%` |
| `ListView.FilterActive.Items20` | `3.421ms/item` | `1.841ms/item` | 快 `46.19%` |
| `ListView.SortActive.Items20` | `1.732ms/item` | `1.711ms/item` | 快 `1.21%` |
| `ListView.Pagination.Items2000` | `8.422ms/item` | `7.881ms/item` | 快 `6.42%` |
| `ListView.GalleryListViewShape` | `25.593ms/item` | `16.982ms/item` | 快 `33.65%` |

分配变化：

| 场景 | Before | After | 提升 |
| --- | ---: | ---: | ---: |
| `ListView.Pagination.Items2000` | `2213.6KB/item` | `2203.4KB/item` | 少 `10.2KB/item` |
| `ListView.GalleryListViewShape` | `5193.2KB/item` | `5177.7KB/item` | 少 `15.5KB/item` |

解释：

- 分组和过滤场景提升最大，主要来自 `ItemsSource` 替换期间合并 refresh，以及默认路径不再无意义创建/清空排序描述。
- 分页场景 timing 提升较小，但移除了每次分页枚举都复制当前页临时列表的成本。
- visual/root 基本不变，本轮不是模板树减重，而是内部 collection refresh、枚举和生命周期成本收敛。

## ListShowCase 加载时间

口径：真实 `ListShowCase.axaml`，Debug / headless / 1300x900，`--cold-iterations 10 --warmup 10 --iterations 30`。Before 为 Phase 0 同口径多轮均值，After 为最终 `/private/tmp/atomui-listshowcase-listview-final-robust.md`。

| 指标 | Before | After | 提升 |
| --- | ---: | ---: | ---: |
| Cold first navigation mean | `456.89ms` | `443.27ms` | 快 `13.62ms / 2.98%` |
| Cold first navigation median | `451.94ms` | `448.26ms` | 快 `3.68ms / 0.81%` |
| Cold first navigation P95 | `497.27ms` | `461.58ms` | 快 `35.69ms / 7.18%` |
| Cold alloc mean | `16283.72KB` | `16237.59KB` | 少 `46.13KB / 0.28%` |
| Repeated mean | `80.72ms` | `77.34ms` | 快 `3.38ms / 4.19%` |
| Repeated median | `75.76ms` | `72.00ms` | 快 `3.76ms / 4.96%` |
| Repeated P95 | `111.77ms` | `108.82ms` | 快 `2.95ms / 2.64%` |
| Repeated alloc mean | `12408.34KB` | `12386.25KB` | 少 `22.09KB / 0.18%` |

结构数据：

| 指标 | Before | After | 变化 |
| --- | ---: | ---: | ---: |
| Visual nodes | `664` | `664` | 持平 |
| Logical nodes | `104` | `104` | 持平 |
| ShowCaseItem | `11` | `11` | 持平 |

结论：本轮符合预期。控件级 grouped/filter/pagination 路径有明确收益；真实 `ListShowCase` 页面因为视觉树没有减少，页面级收益不会很大，但 cold/repeated 主要指标没有回退，重复导航约快 `3.38ms`。

## 验证命令

```bash
dotnet build tools/performances/AtomUI.Performance/AtomUI.Performance.csproj -c Debug -f net10.0
dotnet run --no-build --project tools/performances/AtomUI.Performance/AtomUI.Performance.csproj -c Debug -f net10.0 -- --verify-listview-states
dotnet run --no-build --project tools/performances/AtomUI.Performance/AtomUI.Performance.csproj -c Debug -f net10.0 -- --verify-listbox-states
dotnet run --no-build --project tools/performances/AtomUI.Performance/AtomUI.Performance.csproj -c Debug -f net10.0 -- --suite listview --count 60 --markdown /private/tmp/atomui-listview-final.md
dotnet build tools/performances/AtomUI.GalleryPerformance/AtomUI.GalleryPerformance.csproj -c Debug -f net10.0
dotnet run --no-build --project tools/performances/AtomUI.GalleryPerformance/AtomUI.GalleryPerformance.csproj -c Debug -f net10.0 -- --showcase list --label listview-final-robust --warmup 10 --iterations 30 --cold-iterations 10 --markdown /private/tmp/atomui-listshowcase-listview-final-robust.md
dotnet build controlgallery/AtomUIGallery.Desktop/AtomUIGallery.Desktop.csproj -c Debug -f net10.0
git diff --check
```
