# TreeView 性能优化

> 路线图位置：[`../desktop-controls-optimization-roadmap.md`](../desktop-controls-optimization-roadmap.md) Tier 1 #1
> 状态：本轮已完成 Gallery 实测。

---

## 0. 结论

本轮优化 `NodeSwitcherButton` 的模板结构：每个树节点的 switcher 原来固定创建 5 个 `IconPresenter`（expand/collapse/rotation/loading/leaf），实际同一时刻只显示一个。现在模板只保留 1 个 `IconPresenter`，由 `NodeSwitcherButton.CurrentIcon` 和 `IsCurrentIconVisible` 选择当前图标。

这不是 axaml 节点搬到 C# 动态创建；功能视觉仍在 `ControlTheme` 中，C# 只维护运行态属性。

| 指标 | baseline | optimized | 改善 |
| --- | ---: | ---: | ---: |
| Cold first navigation mean | 1049.62 ms | 776.51 ms | 26.0% |
| Cold alloc mean | 54136.18 KB | 37558.07 KB | 30.6% |
| Repeated navigation mean | 332.44 ms | 213.41 ms | 35.8% |
| Repeated median | 323.32 ms | 206.68 ms | 36.1% |
| Repeated P95 | 403.63 ms | 243.17 ms | 39.8% |
| Repeated alloc mean | 49964.64 KB | 33732.33 KB | 32.5% |
| Runtime visuals | 2108 | 1646 | 21.9% |
| `IconPresenter` | 332 | 112 | 66.3% |
| `Icon` / `PathIcon` | 302 | 60 | 80.1% |

---

## 1. 资格门槛

Gallery source：`controlgallery/AtomUIGallery/ShowCases/Views/DataDisplay/TreeViewShowCase.axaml`

| 项 | 数值 |
| --- | ---: |
| `<atom:TreeView>` | 15 |
| declared `<atom:TreeViewItem>` | 87 |
| `ShowCaseItem` | 10 |
| Runtime `MotionActor` / tree rows | 55 |

真实会话操作频率：

- 页面导航：至少 1 次。
- 展开/折叠：基础示例、line 示例、自定义 switcher、搜索结果、右键菜单示例都会触发。
- 过滤搜索：2 个 `SearchEdit` 示例。
- 拖拽：`IsDraggable=True` 示例。
- 右键菜单：`ItemContextMenuRequest` 示例。

结论：实例数 > 5，操作 > 1/session，并已有 Gallery 数字，满足 SKILL Tier 1 §13。

---

## 2. 根因

`NodeSwitcherButtonTheme.axaml` 原模板为每个 switcher 固定创建 5 个图标 presenter：

- `ExpandIconPresenter`
- `CollapseIconPresenter`
- `RotationIconPresenter`
- `LoadingIconPresenter`
- `LeafIconPresenter`

Gallery baseline 显示 `IconPresenter=332`、`Icon/PathIcon=302`，远高于页面直接声明的图标数量。TreeView 当前真实稳定形态有 55 个节点 header，5 个 switcher presenter 会放大成约 275 个额外 icon presenter 结构。

Avalonia source reference：

- `.referenceprojects/Avalonia/src/Avalonia.Controls/Primitives/TemplatedControl.cs:333`：`template.Build(this)` 创建模板树。
- `.referenceprojects/Avalonia/src/Avalonia.Controls/Primitives/TemplatedControl.cs:338`：模板根加入 visual children。
- `.referenceprojects/Avalonia/src/Avalonia.Base/Layout/Layoutable.cs:546` / `:624`：`IsVisible=false` 的元素会跳过 measure，但前面的模板创建成本已经发生。
- `src/AtomUI.Controls/Icon/IconPresenter.cs:50`：`IconProperty` 影响 measure。
- `src/AtomUI.Controls/Icon/IconPresenter.cs:95` / `:96`：`IconPresenter` 会把当前 `PathIcon` 加入 visual/logical children。

主导子系统：Templates / visual tree structure，其次是 IconPresenter 绑定与子图标结构。

可证伪假设：如果把 `NodeSwitcherButton` 的 5 个固定 `IconPresenter` 收敛到 1 个，同时保持所有图标状态和 selector 行为，`TreeViewShowCase` 的 `IconPresenter` 与 visual count 应显著下降，并带来 repeated navigation >= 5% 的时间或分配改善；否则回退。

---

## 3. 改动

### 3.1 `NodeSwitcherButton` 增加运行态只读属性

新增：

- `CurrentIcon`：当前应该交给模板 presenter 显示的 `PathIcon?`。
- `IsCurrentIconVisible`：Leaf 模式且 `IsLeafIconVisible=false` 时隐藏 presenter。

状态映射：

| `IconMode` | 当前图标 |
| --- | --- |
| `Default` + unchecked | `ExpandIcon` |
| `Default` + checked | `CollapseIcon` |
| `Rotation` | `RotationIcon` |
| `Loading` | `LoadingIcon` |
| `Leaf` + visible | `LeafIcon` |
| `Leaf` + hidden | `null` |

触发更新的属性：`IconMode`、`IsChecked`、`IsLeafIconVisible`、`ExpandIcon`、`CollapseIcon`、`RotationIcon`、`LoadingIcon`、`LeafIcon`。

### 3.2 模板保留单个 presenter

`NodeSwitcherButtonTheme.axaml` 将 5 个 presenter 替换成：

```xml
<atom:IconPresenter Name="CurrentIconPresenter"
                    RenderTransform="{TemplateBinding RotationIconRenderTransform}"
                    Icon="{TemplateBinding CurrentIcon}"
                    IsVisible="{TemplateBinding IsCurrentIconVisible}" />
```

保留的样式行为：

- 普通尺寸：`IconSize`。
- Rotation 模式：`IconSizeXS`。
- Loading 模式：`ColorPrimary` icon brush。
- checked + Rotation：`RotationIconRenderTransform=rotate(90deg)`。
- motion：`Background` 与 `RotationIconRenderTransform` transition 不变。

---

## 4. 验证

### 4.1 构建

```bash
dotnet build src/AtomUI.Desktop.Controls/AtomUI.Desktop.Controls.csproj -c Debug --framework net10.0
```

结果：0 Warning(s), 0 Error(s)。

```bash
dotnet build tools/performances/AtomUI.GalleryPerformance/AtomUI.GalleryPerformance.csproj -c Debug --framework net10.0
```

结果：构建通过；保留既有 warning `DataGridColumn._clipboardContentBinding is never used`，非本轮新增。

### 4.2 Gallery 基线与优化后对比

命令：

```bash
dotnet run --project tools/performances/AtomUI.GalleryPerformance/AtomUI.GalleryPerformance.csproj \
  -c Debug --framework net10.0 --no-build -- \
  --showcase treeview --label baseline \
  --cold-iterations 10 --iterations 30 --warmup 5 --timeout-ms 30000 \
  --markdown /tmp/treeview-showcase-baseline.md
```

```bash
dotnet run --project tools/performances/AtomUI.GalleryPerformance/AtomUI.GalleryPerformance.csproj \
  -c Debug --framework net10.0 --no-build -- \
  --showcase treeview --label optimized \
  --cold-iterations 10 --iterations 30 --warmup 5 --timeout-ms 30000 \
  --markdown /tmp/treeview-showcase-optimized.md
```

| Set | Mean | Median | P95 | Alloc mean | Visuals | IconPresenter | PathIcon |
| --- | ---: | ---: | ---: | ---: | ---: | ---: | ---: |
| Cold baseline | 1049.62 ms | 1056.51 ms | 1189.91 ms | 54136.18 KB | 2108 | 332 | 302 |
| Cold optimized | 776.51 ms | 770.78 ms | 830.95 ms | 37558.07 KB | 1646 | 112 | 60 |
| Repeated baseline | 332.44 ms | 323.32 ms | 403.63 ms | 49964.64 KB | 2108 | 332 | 302 |
| Repeated optimized | 213.41 ms | 206.68 ms | 243.17 ms | 33732.33 KB | 1646 | 112 | 60 |

### 4.3 Regression matrix

Matrix：`tools/performances/AtomUI.Performance/Suites/TreeView/Regression.md`

自动状态验证：

```bash
dotnet run --project tools/performances/AtomUI.Performance/AtomUI.Performance.csproj \
  -c Debug --framework net10.0 --no-build -- \
  --verify-treeview-states
```

结果：`TreeView state verification passed.`

本轮覆盖：

- 默认 expand/collapse 图标切换。
- rotation switcher checked transform。
- loading async node icon。
- leaf icon visible / hidden。
- search/filter page load path。
- context menu page load path。

---

## 5. 复杂度自评

| 指标 | 数值 |
| --- | --- |
| 新增 `Ensure*/Clear*/Sync*` 方法 | 0 |
| 新增 try/finally 标志位 | 0 |
| 新增 disposable 字段 | 0 |
| 新增订阅 / timer / reparented element | 0 |
| axaml 节点搬到 C# 动态创建 | 否 |
| Theme Static Rule | 未触发 |
| 生产文件范围 | 2 个文件，均在 `TreeView` |

新增 `CurrentIcon` 是 direct runtime state，模板仍使用 `{TemplateBinding}`，没有引入 `RelayBind` 或同生命周期 disposable plumbing。

---

## 6. 后续

TreeView 仍有第二类热点：`TreeViewItemHeader.BuildFilterHighlightRuns()` 在搜索高亮中存在 per-character `Run` 模式。该问题只在 filter 交互触发，不属于本轮页面导航主因；下一轮应先建立 filter action 的专项基线，再考虑段级 Run 合并。
