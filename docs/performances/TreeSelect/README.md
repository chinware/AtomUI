# TreeSelect 性能优化

> 状态：本轮已完成 Gallery 实测。

---

## 0. 结论

本轮没有继续改 `TreeSelect` 的 popup / binding 结构；这些尝试已被实测否定。最终收益来自 `TreeSelect` 依赖的共享 `SelectHandle`：原模板为每个 handle 固定创建 `OpenIndicator`、`LoadingIndicator`、`SearchIndicator` 三套 indicator visual，实际同一时刻只应显示一个。现在模板保留一个 `IconPresenter#OpenIndicator`，由 `CurrentIndicatorIcon` / `IsCurrentIndicatorVisible` 选择当前图标。

搜索图标仍由 `SelectHandleTheme.axaml` 通过 `IconTemplate` 声明；C# 不硬编码 `SearchOutlined`，也不新增 `Ensure*/Clear*` 动态 visual 管线。

| 指标 | baseline | optimized | 改善 |
| --- | ---: | ---: | ---: |
| Cold first navigation mean | 474.44 ms | 409.20 ms | 13.7% |
| Cold median | 456.79 ms | 405.25 ms | 11.3% |
| Cold P95 | 571.90 ms | 443.28 ms | 22.5% |
| Repeated navigation mean | 124.72 ms | 108.95 ms | 12.6% |
| Repeated median | 119.78 ms | 101.99 ms | 14.9% |
| Repeated P95 | 164.29 ms | 147.20 ms | 10.4% |
| Repeated alloc mean | 19996.64 KB | 18118.79 KB | 9.4% |
| Runtime visuals | 909 | 858 | 5.6% |
| `IconPresenter` | 38 | 21 | 44.7% |
| `Icon` / `PathIcon` | 51 | 17 | 66.7% |

---

## 1. 资格门槛

Gallery source：`controlgallery/AtomUIGallery/ShowCases/Views/DataEntry/TreeSelectShowCase.axaml`

| 项 | 数值 |
| --- | ---: |
| `<atom:TreeSelect>` | 17 |
| `ShowCaseItem` | 11 |
| Runtime `TreeSelect` | 17 |
| Runtime `AddOnDecoratedBox` | 17 |
| Runtime `CheckBox` | 12 |

真实会话操作频率：

- 页面导航：至少 1 次。
- 下拉打开：基础、多选、checkable、filter、async load 等示例都会触发。
- filter / search：filter 示例会触发 `SelectHandle` 的搜索 indicator 状态。
- clear / loading：allow-clear、async load 路径会触发 handle 右侧状态切换。

结论：实例数 > 5，操作 > 1/session，满足 SKILL Tier 1 §13。

---

## 2. 根因

`SelectHandleTheme.axaml` 原模板固定创建三套 indicator：

- `IconPresenter#OpenIndicator`
- `IconPresenter#LoadingIndicator`
- `SearchOutlined#SearchIndicator`

TreeSelectShowCase 有 17 个 `TreeSelect`，这三套 indicator 在页面加载时放大为 51 个 icon / path icon 节点，其中大多数只是隐藏状态。

Avalonia source reference：

- `.referenceprojects/Avalonia/src/Avalonia.Controls/Primitives/TemplatedControl.cs:333`：`template.Build(this)` 创建模板树。
- `.referenceprojects/Avalonia/src/Avalonia.Controls/Primitives/TemplatedControl.cs:338`：模板根加入 visual children。
- `.referenceprojects/Avalonia/src/Avalonia.Base/Layout/Layoutable.cs:546` / `:624`：`IsVisible=false` 会跳过 measure，但模板创建成本已经发生。
- `src/AtomUI.Controls/Icon/IconPresenter.cs:50`：`IconProperty` 影响 measure。
- `src/AtomUI.Controls/Icon/IconPresenter.cs:63-66`、`:95-96`：`IconPresenter` 切换 icon 时释放旧 visual/logical child 并挂载新 child。

主导子系统：Templates / visual tree structure，其次是 IconPresenter 子图标结构。

---

## 3. 被否定的尝试

本轮先验证了 `TreeSelect` 自身 binding 方向：

| 尝试 | 结果 |
| --- | --- |
| 将模板绑定改为 `TemplateBinding` | 破坏运行时视觉形态，回退 |
| 改回 ancestor binding | 视觉恢复，但 repeated mean 回退 |
| 使用 direct observable binding | 分配小幅下降，但耗时明显回退 |

结论：TreeSelect 当前主要问题不在这条 binding 链路；继续叠加 binding 改动不符合 3-round budget，已回退。

---

## 4. 最终改动

### 4.1 `SelectHandle` 增加运行态 indicator

新增：

- `CurrentIndicatorIcon`：当前交给模板 presenter 显示的 `PathIcon?`。
- `IsCurrentIndicatorVisible`：当前没有可显示 icon 时隐藏 presenter。
- `FilterIndicatorTemplate`：主题声明的 filter icon 模板。

状态映射：

| 状态 | 当前图标 |
| --- | --- |
| `IsFilterEnabled && IsDropDownOpen` | `FilterIndicatorTemplate.Build()` |
| `IsLoading` | `LoadingIcon` |
| 默认 | `OpenIndicator` |

clear 按钮仍在 XAML 中保留，hover / pressed clear 状态只隐藏共享 indicator presenter，不新增动态按钮创建。

### 4.2 模板保留单个 presenter

`SelectHandleTheme.axaml` 将三套 indicator 收敛为：

```xml
<atom:IconPresenter Name="OpenIndicator"
                    Icon="{TemplateBinding CurrentIndicatorIcon}"
                    IsVisible="{TemplateBinding IsCurrentIndicatorVisible}"
                    IsMotionEnabled="{TemplateBinding IsMotionEnabled}" />
```

`SearchOutlined` 通过 theme-level `FilterIndicatorTemplate` 保留：

```xml
<Setter Property="FilterIndicatorTemplate">
    <atom:IconTemplate>
        <antdicons:SearchOutlined />
    </atom:IconTemplate>
</Setter>
```

---

## 5. 验证

### 5.1 构建

```bash
dotnet build src/AtomUI.Desktop.Controls/AtomUI.Desktop.Controls.csproj -c Debug --framework net10.0
```

结果：0 Warning(s), 0 Error(s)。

```bash
dotnet build tools/performances/AtomUI.Performance/AtomUI.Performance.csproj -c Debug --framework net10.0
```

结果：0 Warning(s), 0 Error(s)。

```bash
dotnet build tools/performances/AtomUI.GalleryPerformance/AtomUI.GalleryPerformance.csproj -c Debug --framework net10.0
```

结果：构建通过；保留既有 warning `DataGridColumn._clipboardContentBinding is never used`，非本轮新增。

### 5.2 Gallery 基线与优化后对比

命令：

```bash
dotnet run --project tools/performances/AtomUI.GalleryPerformance/AtomUI.GalleryPerformance.csproj \
  -c Debug --framework net10.0 --no-build -- \
  --showcase treeselect --label baseline \
  --cold-iterations 10 --iterations 30 --warmup 5 --timeout-ms 30000 \
  --markdown /tmp/treeselect-showcase-baseline.md
```

```bash
dotnet run --project tools/performances/AtomUI.GalleryPerformance/AtomUI.GalleryPerformance.csproj \
  -c Debug --framework net10.0 --no-build -- \
  --showcase treeselect --label optimized-selecthandle-rerun \
  --cold-iterations 10 --iterations 30 --warmup 5 --timeout-ms 30000 \
  --markdown /tmp/treeselect-showcase-optimized-selecthandle-rerun.md
```

| Set | Mean | Median | P95 | Alloc mean | Visuals | IconPresenter | PathIcon |
| --- | ---: | ---: | ---: | ---: | ---: | ---: | ---: |
| Cold baseline | 474.44 ms | 456.79 ms | 571.90 ms | n/a | 909 | 38 | 51 |
| Cold optimized | 409.20 ms | 405.25 ms | 443.28 ms | 20407.88 KB | 858 | 21 | 17 |
| Repeated baseline | 124.72 ms | 119.78 ms | 164.29 ms | 19996.64 KB | 909 | 38 | 51 |
| Repeated optimized | 108.95 ms | 101.99 ms | 147.20 ms | 18118.79 KB | 858 | 21 | 17 |

### 5.3 Shared surface smoke

`SelectHandle` 同时被 Select / Cascader 使用，本轮只对它们做 shared surface smoke，不声明页面级收益：

```bash
dotnet run --project tools/performances/AtomUI.GalleryPerformance/AtomUI.GalleryPerformance.csproj \
  -c Debug --framework net10.0 --no-build -- \
  --showcase select --label selecthandle-smoke \
  --cold-iterations 3 --iterations 10 --warmup 2 --timeout-ms 30000 \
  --markdown /tmp/select-showcase-selecthandle-smoke.md
```

结果：`SelectShowCase` 完成，runtime `Select=33`、`IconPresenter=59`、`Icon/PathIcon=46`。

```bash
dotnet run --project tools/performances/AtomUI.GalleryPerformance/AtomUI.GalleryPerformance.csproj \
  -c Debug --framework net10.0 --no-build -- \
  --showcase cascader --label selecthandle-smoke \
  --cold-iterations 3 --iterations 10 --warmup 2 --timeout-ms 30000 \
  --markdown /tmp/cascader-showcase-selecthandle-smoke.md
```

结果：`CascaderShowCase` 完成，runtime `Cascader=23`、`IconPresenter=42`、`Icon/PathIcon=56`。

### 5.4 状态验证现状

已更新 `VerifySelectHandleIndicatorSlots` 和 `VerifySelectLoadingLifecycle` 的期望：`SelectHandle` 现在验证共享 presenter，而不是旧的 `LoadingIndicator` / `SearchIndicator` 独立 visual。

历史宽验证中的 icon hidden-slot 断言仍有非本轮失败：

```bash
dotnet run --project tools/performances/AtomUI.Performance/AtomUI.Performance.csproj \
  -c Debug --framework net10.0 --no-build -- \
  --verify-icon-hidden-slots
```

剩余失败：MenuItem、ToggleIconButton、NavMenu indicator、Button loading icon 等既有 hidden-slot 断言。

`--verify-select-states` 中的 Select / TreeSelect closed popup lazy 和 DropDown event count 断言已在第 10 节修复并通过。

---

## 6. 复杂度自评

| 指标 | 数值 |
| --- | --- |
| 新增 `Ensure*/Clear*/Sync*` 方法 | 0 |
| 新增 `_ignoreXxx` / try-finally 标志位 | 0 |
| 新增 disposable 字段 | 0 |
| 新增订阅 / timer | 0 |
| 同生命周期 `RelayBind` | 0 |
| 生产文件范围 | 2 个文件，均在共享 `SelectHandle` |
| 状态验证文件 | 2 个文件，匹配新单 presenter 语义 |

本轮保留 `TemplateBinding`，没有引入同优先级 binding 碰撞。搜索图标由 theme `IconTemplate` 声明，避免把具体 icon 类型硬编码到 C#。

---

## 7. 本次增量：选择同步集合构造收敛

本次增量只处理 `TreeSelect` 运行期选择同步和有效标签计算中的 LINQ materialization；不改模板、padding、视觉层级、popup 生命周期或绑定优先级。因此本节只声明结构性收益，不声明新的页面导航耗时提升。

| 指标 | baseline | optimized | 公式 | 改善 | 结论 |
| --- | ---: | ---: | --- | ---: | --- |
| ItemsSource copy LINQ chain / reset | 2 chains | 0 chains | `(2 - 0) / 2` | 100.00% | `Items.Cast<object?>().ToList()` 改为显式列表构造 |
| selected/checked comparison hashset materialization / sync | 4 chains | 0 chains | `(4 - 0) / 4` | 100.00% | `ToHashSet()` / `Cast().ToHashSet()` 改为按 Count 构造 |
| TreeView selected/checked snapshot list / sync from view | 2 chains | 0 chains | `(2 - 0) / 2` | 100.00% | `Cast<ITreeItemNode>().ToList()` 改为显式列表构造 |
| selected/checked diff LINQ chain / sync to view | 8 chains | 0 chains | `(8 - 0) / 8` | 100.00% | 两条路径的 `Cast().ToList()`、`SelectedItems.ToList()`、`Except()` 全部改为 set lookup |
| effective selected item LINQ calls / tag rebuild | 4 calls | 0 calls | `(4 - 0) / 4` | 100.00% | `ToHashSet()`、`Where().ToHashSet()`、祖先 `Any()`、children `Any()` 全部改为显式循环 |
| tag close selected list capacity / close | dynamic growth | exact `SelectedItems.Count` | structural | 结构收益 | 关闭 tag 时的临时列表按已知选中数预分配 |

验证：

```bash
dotnet build -c Release -f net10.0 --no-restore tools/performances/AtomUI.Performance/AtomUI.Performance.csproj
dotnet run -c Release -f net10.0 --no-build --project tools/performances/AtomUI.Performance/AtomUI.Performance.csproj -- --verify-addon-states --verify-datepicker-states --verify-listbox-states
dotnet run -c Release -f net10.0 --no-build --project tools/performances/AtomUI.Performance/AtomUI.Performance.csproj -- --verify-timepicker-states
dotnet run -c Release -f net10.0 --no-build --project tools/performances/AtomUI.Performance/AtomUI.Performance.csproj -- --verify-transfer-states
dotnet run -c Release -f net10.0 --no-build --project tools/performances/AtomUI.Performance/AtomUI.Performance.csproj -- --verify-treeview-states
```

结果：上述构建和状态验证通过。`--verify-select-states` 的 closed lazy-slot / dropdown event count 已在第 10 节修复并通过。

## 8. 追加结构优化：只读集合容量快路径

`TreeSelect` 的 ItemsSource / selected / checked 同步 helper 原先只识别非泛型 `ICollection`。当调用方传入只实现 `IReadOnlyCollection<object?>` 或 `IReadOnlyCollection<ITreeItemNode>` 的集合时，列表 / HashSet 会从默认容量动态增长。本轮补上只读集合 Count 快路径；复制顺序、类型转换、diff 语义不变。

| 指标 | baseline | optimized | 公式 | 改善 | 结论 |
| --- | ---: | ---: | --- | ---: | --- |
| TreeSelect ItemsSource copy growth / read-only source reset | dynamic growth | exact read-only count | structural | 结构收益 | 只读 ItemsSource 复制时按 Count 预分配 |
| TreeSelect tree node list growth / read-only selection sync | dynamic growth | exact read-only count | structural | 结构收益 | 只读 selected/checked snapshot 列表按 Count 预分配 |
| TreeSelect tree node HashSet growth / read-only selection sync | dynamic growth | exact read-only count | structural | 结构收益 | 只读 selected/checked set 按 Count 预分配 |
| TreeSelect selection semantics | unchanged | unchanged | n/a | 0.00% | 行为保持 |

说明：这是 TreeSelect 数据同步路径 structural-only 收益；没有新增页面 timing 对比，不声明页面加载速度提升。

## 9. 追加结构优化：effective tag 祖先判断

`ShowCheckedStrategy.ShowParent` 构建多选有效标签时，旧实现对每个 selected item 遍历所有 `fullySelectedParents`，再从当前节点向上判断是否属于该 parent。现在改为沿当前 node 的父链直接查 `HashSet`，语义仍然跳过自身，只判断祖先。

| 指标 | baseline | optimized | 公式 | 改善 | 结论 |
| --- | ---: | ---: | --- | ---: | --- |
| TreeSelect effective tag ancestor candidate scans / selected item | `P` parent candidates | 0 parent-candidate loop | `(P - 0) / P` | P>0 时 100.00% | structural-only；改为父链 HashSet lookup |
| TreeSelect effective tag ancestor path walks / selected item | up to `P * depth` | up to `depth` | `(P*D - D) / (P*D)` | P>1 时随 P 增大 | 行为保持；自身不算祖先 |
| TreeSelect empty selected effective tag list allocations / rebuild | 1 list | 0 lists | `(1 - 0) / 1` | 100.00% | structural-only；空多选直接复用当前空 selected list |
| Page-load timing claim | none | none | n/a | n/a | 本轮没有有效前后 timing，不声明页面级速度收益 |

## 10. 追加结构优化：关闭态 TreeView 延迟创建

`TreeSelect` 的 `Popup` shell 继续留在 theme 中；只把 `PopupFrame + TreeSelectTreeView` 延迟到首次打开前创建，并在 re-template / detach 时清理事件、`ItemsSource` 和 visual parent。输入区的 `SelectFilterTextBox` / `SelectTagAwareTextBox` 继续保持 theme 静态隐藏，避免把普通模板元素改成 C# 动态创建。

| 指标 | baseline | optimized | 公式 | 改善 | 结论 |
| --- | ---: | ---: | --- | ---: | --- |
| Closed TreeSelect popup frame visuals / instance | 1 | 0 | `(1 - 0) / 1` | 100.00% | 关闭态不再创建 popup frame |
| Closed TreeSelect tree view visuals / instance | 1 | 0 | `(1 - 0) / 1` | 100.00% | 关闭态不再创建 `TreeSelectTreeView` |
| Detached lazy TreeView visual parent risk | possible | cleared | structural | 100.00% risk removed | detach 后清理 child / template parent / ItemsSource |
| Page-load timing claim | none | none | n/a | n/a | 本轮没有有效前后 timing，不声明页面级速度收益 |

验证：`dotnet build tools/performances/AtomUI.Performance/AtomUI.Performance.csproj -c Release --framework net10.0 --no-restore` 通过；`--verify-select-states` 通过，覆盖 TreeSelect popup 首次 materialize、复用和 detach 清理。

## 11. 后续

TreeSelect 当前独立热点更可能在 popup 打开后的 `TreeSelectTreeView`、filter 交互和 checkable tree item 路径。下一轮需要先建立交互动作级基线，而不是继续改页面首次导航的 handle 结构。

## 12. 追加结构优化：checked strategy flag 判断

`BuildEffectiveSelectedItems()` 需要判断 `ShowParent` / `ShowChild` 两个 flag。本轮把 `HasFlag()` 改为等价 bitwise check，并在一次 rebuild 内复用 `ShowCheckedStrategy` 快照。ShowParent / ShowChild / All 的 effective tags 语义不变。

| 指标 | baseline | optimized | 公式 | 改善 | 结论 |
| --- | ---: | ---: | --- | ---: | --- |
| TreeSelect checked strategy `HasFlag` callsites / effective tag rebuild | 2 | 0 | `(2 - 0) / 2` | 100.00% | structural-only；flag 判断不再走 enum helper |
| Effective tag semantics | unchanged | unchanged | n/a | 0.00% | ShowParent / ShowChild / All 行为保持 |
| Page-load timing claim | none | none | n/a | n/a | 本轮没有有效前后 timing，不声明页面级速度收益 |

验证：`--verify-select-states` 通过。
