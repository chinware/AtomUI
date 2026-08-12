# Semantic Part 第五批高密度控件实施计划

> **供智能体执行者使用：** 使用 `superpowers:executing-plans` 在当前会话中执行，不得使用 subagent。Gate A 源码工作前先记录性能基线，文档完成后停止并等待用户批准；实现完成后保持未提交，再次接受用户审核。

**目标：** 为对应 Ant Design 6.6.0 稳定发布源码公开 `Menu` 与 `Table` Semantic DOM API 的 `NavMenu` 和 `DataGrid` 建立 Semantic Part 契约，不得造成高密度实例化、虚拟化、Popup 或容器生命周期行为退化。

**架构：** public owner 及其 public item/container 控件只获得真实模板能够证明的最小稳定 Descriptor。静态 marker 不参与 AtomUI 默认主题样式，也不随选择或状态切换。使用现有 `tools/performances/AtomUI.Performance` 检查提供改造前后的实例化、布局、分配和状态证据。

**技术栈：** .NET 10、Avalonia 12、AtomUI Desktop Controls/DataGrid、AXAML、xUnit v3、Avalonia Headless、AtomUI Performance、AtomUI Gallery、NativeAOT。

## 全局约束

- 遵循[全量改造总计划](2026-08-12-semantic-part-control-rollout.md)和[全量改造设计](../specs/2026-08-12-semantic-part-control-rollout-design.md)。
- 编辑每个控件的文档前，使用 `--count 60` 和该控件现有的状态验证参数记录性能基线。
- 改造前后报告使用相同的 SDK、配置、count 和宿主；报告保存在临时路径，不得写入正式控件文档或被跟踪的生成输出。
- Gate A 必须定义每个 owner/container 的 marker 数量、Popup root、容器创建/回收路径和预期热路径成本。
- Gate B 不得添加 VisualTree 扫描、动态 class 变更、逐容器 registry 查询、以 Visual 为 key 的 Descriptor cache 或永久监听器。
- AtomUI 默认主题不得选择 `.semantic-*`。
- Gallery Preview 保持延迟创建，最多高亮 32 个可见目标。
- Gate B 改动保持未提交，直到用户明确完成验证并授权提交。

---

### 任务 1：NavMenu

**控件文档：** `docs/controls/desktop/navigation/nav-menu/overview.md`、`docs/controls/desktop/navigation/nav-menu/implementation.md`。

**证据范围：** `src/AtomUI.Desktop.Controls/NavMenu/**/*.cs`、全部 `src/AtomUI.Desktop.Controls/NavMenu/Themes/*Theme.axaml`；测试 `tests/AtomUI.Desktop.Controls.Tests/NavMenu`；共享 Gallery `controlgallery/AtomUIGallery/ShowCases/Navigation/Menu` 和真实 Gallery sidebar `controlgallery/AtomUIGallery/Workspace/Views/CaseNavigation*`；性能检查 `tools/performances/AtomUI.Performance/Suites/NavMenu`。

**风险类型：** entry 到容器的图关系、分层 group/item/divider 控件、Inline/Vertical/Horizontal 模板、Popup 和 collapse motion。

- [ ] **记录改造前基线：** 运行：

```bash
dotnet run --project tools/performances/AtomUI.Performance/AtomUI.Performance.csproj --framework net10.0 -- --suite navmenu --count 60 --markdown /tmp/atomui-semantic-navmenu-before.md
dotnet run --project tools/performances/AtomUI.Performance/AtomUI.Performance.csproj --framework net10.0 -- --verify-navmenu-states
```

记录改造后将再次采集的同一组耗时、分配和 Visual/Logical tree 指标。

- [ ] **Gate A 设计审核：** 审计 `NavMenu`、`NavMenuItem`、`NavMenuGroupItem`、`NavMenuDividerItem`、header 变体和 `NavMenuPopupFrame` 的 owner。明确 header/footer/items、item icon/text/indicator/active indicator、group header/items、inline child frame 和 popup frame/items 的职责。记录 entry graph ownership、container binder prepare/clear、Inline collapse、mode switch、selection、Popup root 和资源生命周期。
- [ ] 更新两份控件文档，写明准确的 Descriptor、逐模式模板映射、每个容器的 marker 数量、runtime/cross-root 标志、生命周期/性能和验证矩阵；运行 LLMS verify 和 `git diff --check`；随后停止并等待用户批准。
- [ ] **Gate B 实现：** 新增 `tests/AtomUI.Desktop.Controls.Tests/NavMenu/NavMenuSemanticPartTests.cs`，覆盖 structured entry、explicit node、group/divider transparency、全部模式、Inline collapse/expand、Popup reopen、selection/keyboard、entry source replace/reset、资源生命周期，并证明重新绑定容器不会产生重复 marker。
- [ ] 将 NavMenu 集成到共享 Menu ShowCase 的 Semantic Tab；不得给同页的 `Menu`、`ContextMenu` 或真实应用 sidebar 附加 Preview 行为，也不得因此改造这些被排除的控件。
- [ ] **记录改造后基线：** 重新运行与改造前完全相同的命令，输出到 `/tmp/atomui-semantic-navmenu-after.md`；比较 ms/item、KB/item、Visual/root、Logical/root 和状态验证结果。
- [ ] 运行 Generator Semantic 测试、全部 NavMenu 测试、GalleryBase 测试、`MenuShowCasePageTests`、LLMS verify、NativeAOT publish 和 `git diff --check`。
- [ ] **强制停止：** 保持 NavMenu 改动未提交，直到用户明确完成性能与行为验证并授权提交。

### 任务 2：DataGrid

**控件文档：** `docs/controls/desktop/data-display/data-grid/overview.md`、`docs/controls/desktop/data-display/data-grid/implementation.md`。只有 Gate A 确认跨 owner 的 grid/row/cell/header/filter 模型无法在两份主文档中保持可维护时，才创建 `semantic-part-design.md`。

**证据范围：** 可选包 `src/AtomUI.Desktop.Controls.DataGrid/**/*.cs`、全部 `src/AtomUI.Desktop.Controls.DataGrid/Themes/**/*.axaml`；独立测试工程 `tests/AtomUI.Desktop.Controls.DataGrid.Tests`；Gallery `controlgallery/AtomUIGallery/ShowCases/DataDisplay/DataGrid`；性能检查 `tools/performances/AtomUI.Performance/Suites/DataGrid`。

**风险类型：** 虚拟化 row/cell、多个 public owner 控件、分组 header/row、filter Flyout、pagination、editing、drag/reorder 和可选包注册。

- [ ] **记录改造前基线：** 运行：

```bash
dotnet run --project tools/performances/AtomUI.Performance/AtomUI.Performance.csproj --framework net10.0 -- --suite datagrid --count 60 --markdown /tmp/atomui-semantic-datagrid-before.md
dotnet run --project tools/performances/AtomUI.Performance/AtomUI.Performance.csproj --framework net10.0 -- --verify-datagrid-states
```

保留 Basic、关闭状态的 Menu/Tree filter、row header、collapsed details、column group、row group 和 Gallery-shape 场景指标。

- [ ] **Gate A 设计审核：** 选择 Part 前盘点每个 public Visual owner：`DataGrid`、`DataGridRow`、`DataGridCell`、`DataGridRowHeader`、`DataGridRowGroupHeader`、column/group header、details/presenter、filter indicator/presenter、sort indicator、row expander/reorder handle 和 operation control。明确哪些 owner 应拥有 Descriptor，哪些 public panel 仅保留结构职责。
- [ ] 映射完整控件家族中的稳定职责：title/footer/pagination/header/rows/empty/scroll 区域；row header/cells/details；cell content/validation/grid line；column header content/sort/filter/separator；grouped header/row；filter popup content/actions/items；operation/edit/reorder/expand affordance。不得把每个命名节点都转换成 Part。
- [ ] 记录 virtualization ownership、row/cell/header 创建与回收、editing template、row details 重建、group expand/collapse、filter Flyout 延迟实例化、pagination template reapply、drag/reorder adorner、SizeType 和嵌套 semantic-control 隔离。
- [ ] Table 对应的 DataGrid owner 可以公开自身 filter/pagination 区域，但嵌套的 `Menu`、`ComboBox` 等被排除控件不得
  因组合关系获得 Descriptor、marker 或 Preview；已有独立准入的 `Pagination` 仍遵守自身 owner 边界。
- [ ] 为每个已实例化 grid/row/cell/header/container 定义明确的 marker 预算，并证明 scroll、pointer hover、selection、edit、sort、filter 或 row reorder 期间不会发生 Descriptor 查询或 marker 变更。
- [ ] 更新两份主控件文档；只有符合创建条件时才更新专项设计文档。写明准确的 Descriptor、节点、cardinality、cross-root/runtime 标志、兼容性、性能/AOT 和完整验证矩阵；运行 LLMS verify 和 `git diff --check`；随后停止并等待用户批准。
- [ ] **Gate B 实现：** 新增 `tests/AtomUI.Desktop.Controls.DataGrid.Tests/Theme/DataGridSemanticPartTests.cs`，验证 Descriptor/可选包注册和静态 marker；扩展生命周期/交互测试，覆盖虚拟化 row/cell/details 回收、grouped row/header、editing、filter Popup reopen、pagination reapply、drag/reorder 和嵌套 owner 隔离。
- [ ] 添加延迟创建的 DataGrid Semantic Parts 界面，使用受限 viewport 和具有代表性的 title/header/row/cell/filter 场景。首次选择 Semantic Tab 前不得创建 DataGrid、row、column、filter model 或 Preview，高亮目标仍限制为最多 32 个可见实例。
- [ ] **记录改造后基线：** 重新运行与改造前完全相同的两条命令，输出到 `/tmp/atomui-semantic-datagrid-after.md`。比较每个命名场景的 ms/item、KB/item、Visual/root 和 Logical/root；解释全部变化，用户审核前拒绝未解释的性能回归。
- [ ] 运行完整 `AtomUI.Desktop.Controls.DataGrid.Tests` 工程、Generator Semantic 测试、GalleryBase 测试、DataGrid Gallery 测试、LLMS verify、Gallery NativeAOT publish 和 `git diff --check`。
- [ ] **强制停止：** 保持所有 DataGrid 改动未提交，直到用户审核设计、实际 Gallery 行为、虚拟化/回收测试和改造前后性能报告，并明确授权提交。

## 批次收尾

- [ ] 确认 NavMenu 和 DataGrid 分别拥有用户授权的独立提交。
- [ ] 重新运行两套控件的性能/状态命令，将最终对比归档到全量改造收尾报告，不写入正式控件文档。
- [ ] 运行完整 Desktop Controls、DataGrid、Generator、GalleryBase 和 Gallery 测试，以及 LLMS verify、NativeAOT publish 和 `git diff --check`。
- [ ] 将第五批和总计划标记为完成，不额外创建批次提交。
