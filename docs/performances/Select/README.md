# Select 性能优化

> 状态：已完成主线优化；本页记录后续低风险结构性补充。

---

## 1. 追加结构优化：异步选项加载完成后的空判断

`SelectOptionsLoadResult.Data` 的类型是 `IReadOnlyList<ISelectOption>?`，异步加载完成后只需要判断是否有数据来决定是否打开下拉。本轮把 `loadResult?.Data?.Any() == true` 改为 `loadResult?.Data?.Count > 0`，保持空/非空语义不变。

| 指标 | 优化前 | 优化后 | 公式 | 提升 | 结论 |
| --- | ---: | ---: | --- | ---: | --- |
| Async options empty-check LINQ calls / load completion | 1 call | 0 calls | `(1 - 0) / 1` | 100.00% | 结构收益；异步加载完成路径直接读 `IReadOnlyList.Count` |

说明：这是异步数据加载完成时的结构性收益；未新增 Gallery timing 对比，不声明页面加载速度提升。

## 2. 追加结构优化：单选默认值首项读取

`Select.DefaultValues` 是 `IList<object>?`。单选默认值配置路径只需要在 `DefaultValues.Count > 0` 后取首项，旧实现使用 `DefaultValues.First()`，会额外创建 LINQ enumerator；现在改为 `DefaultValues[0]`，默认选中语义不变。

| 指标 | 优化前 | 优化后 | 公式 | 提升 | 结论 |
| --- | ---: | ---: | --- | ---: | --- |
| Single-select default value first lookup LINQ calls / configure defaults | 1 call | 0 calls | `(1 - 0) / 1` | 100.00% | 结构收益；直接读 `IList[0]` |
| Default value semantics | first item | first item | n/a | 0.00% | 正确性保持不变 |

说明：这是默认值初始化路径 structural-only 收益；未新增 Gallery timing 对比，不声明页面加载速度提升。

## 3. 追加结构优化：只读集合容量快路径

`BuildSelectedOptionsList()` 的输入类型是 `IEnumerable?`。旧逻辑只识别非泛型 `ICollection`，遇到只实现 `IReadOnlyCollection<ISelectOption>` 的选中集合会退回动态增长。本轮补上只读集合 Count 快路径，枚举顺序和类型转换不变。

| 指标 | 优化前 | 优化后 | 公式 | 提升 | 结论 |
| --- | ---: | ---: | --- | ---: | --- |
| Select selected-options list growth / read-only source copy | dynamic growth | exact read-only count | structural | 结构收益 | 只读选中集合复制时按 Count 预分配 |
| selected option copy semantics | unchanged | unchanged | n/a | 0.00% | 行为保持 |

说明：这是 Select 选择同步路径 structural-only 收益；没有新增页面 timing 对比，不声明页面加载速度提升。

## 4. 追加结构优化：多选/Tags 选择同步热路径

本轮继续收敛 Select 运行期集合操作，覆盖 Backspace/Delete 删除最后一个 tag、候选列表选择变更、tag close 和动态 tag 清理。所有改动只调整临时集合构造方式，不改变模板、padding、视觉结构或选中语义。

| 指标 | 优化前 | 优化后 | 公式 | 提升 | 结论 |
| --- | ---: | ---: | --- | ---: | --- |
| Delete-key selected item copy writes / delete | `N` | `N - 1` | `(N - (N - 1)) / N` | `1 / N` | 结构收益；删除最后一项时不再先复制被删除项 |
| Delete-key post-copy remove operations / delete | 1 call | 0 calls | `(1 - 0) / 1` | 100.00% | 结构收益；移除动作合并进复制过程 |
| Empty no-op candidate selection HashSet allocations / change | 2 sets | 0 sets | `(2 - 0) / 2` | 100.00% | 结构收益；当前无选中且无新增项时直接返回 |
| Empty-current add selection HashSet allocations / change | 2 sets | 1 set | `(2 - 1) / 2` | 50.00% | 结构收益；从空选中变为新增项时不再构造空 current set |
| Tag close selected item copy writes / close | `N` | `N - 1` | `(N - (N - 1)) / N` | `1 / N` | 结构收益；关闭 tag 时不复制被删除项 |
| Tag close extra remove pass / close | up to `N` scan | 0 extra scan | structural | 结构收益 | 旧的 `List.Remove` 二次扫描被合并到复制循环 |
| Dynamic tag cleanup selected-set allocations / empty selected | 1 set | 0 sets | `(1 - 0) / 1` | 100.00% | 结构收益；没有选中项时不再构造空 HashSet |
| Dynamic tag cleanup removal-list allocations / no removal | 1 list | 0 lists | `(1 - 0) / 1` | 100.00% | 结构收益；没有可清理动态项时不再构造临时 List |

说明：这是运行期交互路径 structural-only 收益；没有新增 Gallery timing 对比，不声明页面加载速度提升。

## 5. 追加结构优化：关闭态 popup 重内容延迟创建

`Select` 的 `Popup` shell 仍保留在 theme 中，保证 placement、light dismiss、overlay 和样式入口不变；只把 `PopupFrame + SelectCandidateList` 这类重型 popup 内容延迟到首次打开前创建。非 popup 的 `SelectFilterTextBox` / `SelectResultOptionsBox` 继续留在 theme 中用 `IsVisible` 控制，避免违反 Theme Static Rule。

本轮同时修复 `AbstractSelect.OpenDropDown()` / `CloseDropDown()` 直接发出 opened / closed 的重复通知问题，事件只由 `Popup.Opened` / `Popup.Closed` 对应回调发出。

| 指标 | 优化前 | 优化后 | 公式 | 提升 | 结论 |
| --- | ---: | ---: | --- | ---: | --- |
| Closed Select popup frame visuals / instance | 1 | 0 | `(1 - 0) / 1` | 100.00% | 关闭态不再创建 popup frame |
| Closed Select candidate list visuals / instance | 1 | 0 | `(1 - 0) / 1` | 100.00% | 关闭态不再创建候选列表 / ItemsControl |
| DropDownOpened notifications / open | 2 | 1 | `(2 - 1) / 2` | 50.00% | 修复重复事件；只在 Popup.Opened 后通知 |
| DropDownClosed notifications / close | 2 | 1 | `(2 - 1) / 2` | 50.00% | 修复重复事件；只在 Popup.Closed 后通知 |
| Non-popup mode template elements | static hidden | static hidden | n/a | 0.00% | 正确性约束；不把普通模板元素搬到 C# |

说明：这是关闭态结构和事件正确性收益；本轮没有有效前后页面 timing，不声明页面导航速度提升。

## 6. 验证

- `git diff --check` passed.
- `dotnet build tools/performances/AtomUI.Performance/AtomUI.Performance.csproj --framework net10.0 --no-restore` passed with 0 warning / 0 error.
- `dotnet build tools/performances/AtomUI.Performance/AtomUI.Performance.csproj -c Release --framework net10.0 --no-restore` passed with 0 warning / 0 error.
- `dotnet build tools/performances/AtomUI.Performance/AtomUI.Performance.csproj -c Release --framework net8.0 --no-restore` passed with 0 warning / 0 error.
- `dotnet build tools/performances/AtomUI.Performance/AtomUI.Performance.csproj -c Release --framework net10.0 --no-restore` passed with 0 warning / 0 error.
- `--verify-select-states` passed.

## 7. 追加结构优化：候选列表单选判断

`SelectCandidateList.IsSingleMode()` 原先用 `SelectionMode.HasFlag(Single) && !HasFlag(Multiple)`。Avalonia 的 `SelectionMode.Single = 0`，因此本轮改为直接判断“不包含 Multiple”，与旧语义等价，同时去掉 zero-value flag 的 `HasFlag()` 调用。

| 指标 | 优化前 | 优化后 | 公式 | 提升 | 结论 |
| --- | ---: | ---: | --- | ---: | --- |
| Select candidate single-mode `HasFlag` callsites / selection check | 2 | 0 | `(2 - 0) / 2` | 100.00% | structural-only；单选判断直接检查 Multiple bit |
| Single / Multiple candidate behavior | unchanged | unchanged | n/a | 0.00% | 行为保持 |
| Page-load timing claim | none | none | n/a | n/a | 本轮没有有效前后 timing，不声明页面级速度收益 |

验证：`--verify-select-states` 通过。
