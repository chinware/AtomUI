# Semantic Part 第二批集合与容器实施计划

> **供智能体执行者使用：** 使用 `superpowers:executing-plans` 在当前会话中执行，不得使用 subagent。每个控件在 Gate A 后停止等待用户批准，Gate B 改动保持未提交。

**目标：** 为 15 个具有 Ant Design 6.6.0 公开 Semantic DOM 对应 API 的集合、容器、布局和导航控件家族建立 Semantic Part 契约，同时保持容器、虚拟化和运行时节点性能。

**架构：** 父级 owner 只公开自身稳定区域；public item/container 控件在适用时拥有自己的 Descriptor。运行时生成节点只在现有创建路径中使用生成常量添加 marker，并提供 prepare/clear/recycle 证据。

**技术栈：** .NET 10、Avalonia 12、AtomUI Desktop Controls、AXAML、xUnit v3、Avalonia Headless、AtomUI Gallery。

## 全局约束

- 遵循[全量改造总计划](2026-08-12-semantic-part-control-rollout.md)和[全量改造设计](../specs/2026-08-12-semantic-part-control-rollout-design.md)。
- 修改代码前必须通过 Gate A 文档审核；Gate B 改动保持未提交，直到用户批准。
- 不得将任意用户子元素标记为 `.semantic-item`；只有 owner 创建的稳定容器才能形成 item Part 契约。
- 容器 marker 必须为静态声明，或在构造时通过生成常量一次性添加；不得在 prepare、选择或状态变化期间切换。
- 适用时，测试必须证明集合 replace/reset、prepare/clear/recycle、嵌套 semantic owner 隔离，并且旧容器不会被保留。
- 共享源码或 Gallery 页面中如果同时承载被排除控件，只允许修改准入 owner 的路径；不得给 `TabStrip`、`ListBox` 或其他
  被排除 owner 添加 Descriptor、marker 或 Semantic Preview。

---

### 任务 1：Calendar

**控件文档：** `docs/controls/desktop/data-display/calendar/overview.md`, `docs/controls/desktop/data-display/calendar/implementation.md`

**证据范围：** `src/AtomUI.Desktop.Controls/Calendar/**/*.cs`, `src/AtomUI.Desktop.Controls/Calendar/Themes/*Theme.axaml`；测试 `tests/AtomUI.Desktop.Controls.Tests/Calendar`；Gallery `controlgallery/AtomUIGallery/ShowCases/DataDisplay/Calendar`.

**风险类型：** 运行时日历 cell、派生 LunarCalendar、导航 header、高密度重复节点。

- [ ] **Gate A 设计审核：** 审计 `Calendar`、`LunarCalendar`、internal CalendarView/Header/Cell 与 range bar 的 owner，确认 header/navigation、cell content、range/status 等职责能否跨 month/year/decade 和 lunar templates 保持；定义 cell builder 创建、mode 切换与可见日期范围的 cardinality。
- [ ] 更新两份控件文档，写明准确的 Descriptor、真实模板/运行时节点、owner 边界、排除的 internal wrapper、生命周期/性能不变量和验证矩阵；运行 LLMS verify 和 `git diff --check`；随后停止并等待用户批准。
- [ ] **Gate B 实现与验证：** 新增 `tests/AtomUI.Desktop.Controls.Tests/Calendar/CalendarSemanticPartTests.cs`，覆盖 Calendar/LunarCalendar、各 view mode、cell rebuild、range、header navigation、集合刷新和 marker owner；记录可见 cell marker 数与 mode 切换前后实例释放。
- [ ] 运行 Generator Semantic 测试、目标 Desktop 测试、GalleryBase 测试、目标 Gallery 测试、LLMS verify 和 `git diff --check`；涉及 Popup/运行时宿主路径时增加 NativeAOT 验证。
- [ ] **强制停止：** 保持 Calendar 的所有实现改动未提交，直到用户明确完成验证并授权提交。

### 任务 2：Collapse

**控件文档：** `docs/controls/desktop/data-display/collapse/overview.md`, `docs/controls/desktop/data-display/collapse/implementation.md`

**证据范围：** `src/AtomUI.Desktop.Controls/Collapse/*.cs`, `src/AtomUI.Desktop.Controls/Collapse/Themes/*Theme.axaml`；测试 `tests/AtomUI.Desktop.Controls.Tests/Collapse`；Gallery `controlgallery/AtomUIGallery/ShowCases/DataDisplay/Collapse`.

**风险类型：** item 容器、展开/收起 motion、accordion 模式。

- [ ] **Gate A 设计审核：** 审计 `Collapse` 与 `CollapseItem` 的 owner，确认 item header/content/expand indicator 与 collection host 的职责；记录 data item 到 container 的创建、accordion selection、expand motion 和 clear/recycle 路径。
- [ ] 更新两份控件文档，写明准确的 Descriptor、真实模板/运行时节点、owner 边界、排除的 internal wrapper、生命周期/性能不变量和验证矩阵；运行 LLMS verify 和 `git diff --check`；随后停止并等待用户批准。
- [ ] **Gate B 实现与验证：** 新增 `tests/AtomUI.Desktop.Controls.Tests/Collapse/CollapseSemanticPartTests.cs`，覆盖 multiple/accordion、item add/remove/reset、header/content templates、expand/collapse/reopen 和 container clear；验证 repeated Part 数量与 owner scope。
- [ ] 运行 Generator Semantic 测试、目标 Desktop 测试、GalleryBase 测试、目标 Gallery 测试、LLMS verify 和 `git diff --check`；涉及 Popup/运行时宿主路径时增加 NativeAOT 验证。
- [ ] **强制停止：** 保持 Collapse 的所有实现改动未提交，直到用户明确完成验证并授权提交。

### 任务 3：ListView

**控件文档：** `docs/controls/desktop/data-display/list-view/overview.md`, `docs/controls/desktop/data-display/list-view/implementation.md`

**证据范围：** `src/AtomUI.Desktop.Controls/ListView/*.cs`, `src/AtomUI.Desktop.Controls/ListView/Themes/*Theme.axaml`；测试 `tests/AtomUI.Desktop.Controls.Tests/ListView`；共享 Gallery `controlgallery/AtomUIGallery/ShowCases/DataDisplay/List`.

**风险类型：** 虚拟化、pagination、selection model、共享 Gallery、公开 `List.Item` 边界有限。

- [ ] **Gate A 设计审核：** 以 Ant Design 公开 `List.Item` 的 `actions` / `extra` API 为上限，审计 `ListViewItem` 中职责直接
  对应的稳定区域；不得据此把 `ListView` root、selection、pagination、empty/loading 或普通 item content 扩大为公共 Part。
  同时记录 virtualized 容器生命周期、selection model、filter/pagination changes 和 nested content isolation。
- [ ] 更新两份控件文档，写明准确的 Descriptor、真实模板/运行时节点、owner 边界、排除的 internal wrapper、生命周期/性能不变量和验证矩阵；运行 LLMS verify 和 `git diff --check`；随后停止并等待用户批准。
- [ ] **Gate B 实现与验证：** 新增 `tests/AtomUI.Desktop.Controls.Tests/ListView/ListViewSemanticPartTests.cs`，覆盖 virtualized/nonvirtualized、pagination、empty/loading、selection、filter、replace/reset 和 recycle；记录 marker/container 数量与滚动前后 retained instance。
- [ ] 运行 Generator Semantic 测试、目标 Desktop 测试、GalleryBase 测试、目标 Gallery 测试、LLMS verify 和 `git diff --check`；涉及 Popup/运行时宿主路径时增加 NativeAOT 验证。
- [ ] **强制停止：** 保持 ListView 的所有实现改动未提交，直到用户明确完成验证并授权提交。

### 任务 4：Segmented

**控件文档：** `docs/controls/desktop/data-display/segmented/overview.md`, `docs/controls/desktop/data-display/segmented/implementation.md`

**证据范围：** `src/AtomUI.Desktop.Controls/Segmented/*.cs`, `src/AtomUI.Desktop.Controls/Segmented/Themes/*Theme.axaml`；测试 `tests/AtomUI.Desktop.Controls.Tests/Segmented`；Gallery `controlgallery/AtomUIGallery/ShowCases/DataDisplay/Segmented`.

**风险类型：** item 容器、selected indicator geometry、SizeType。

- [ ] **Gate A 设计审核：** 审计 `Segmented` 与 `SegmentedItem` owner、item icon/label 和 selected indicator/track 职责；记录 ItemsSource/容器生命周期、selection motion、block mode 和 SizeType。
- [ ] 更新两份控件文档，写明准确的 Descriptor、真实模板/运行时节点、owner 边界、排除的 internal wrapper、生命周期/性能不变量和验证矩阵；运行 LLMS verify 和 `git diff --check`；随后停止并等待用户批准。
- [ ] **Gate B 实现与验证：** 新增 `tests/AtomUI.Desktop.Controls.Tests/Segmented/SegmentedSemanticPartTests.cs`，覆盖 explicit/generated items、icon/text variants、selection changes、collection reset、block mode、所有尺寸 和 layout Setter。
- [ ] 运行 Generator Semantic 测试、目标 Desktop 测试、GalleryBase 测试、目标 Gallery 测试、LLMS verify 和 `git diff --check`；涉及 Popup/运行时宿主路径时增加 NativeAOT 验证。
- [ ] **强制停止：** 保持 Segmented 的所有实现改动未提交，直到用户明确完成验证并授权提交。

### 任务 5：Tag

**控件文档：** `docs/controls/desktop/data-display/tag/overview.md`, `docs/controls/desktop/data-display/tag/implementation.md`

**证据范围：** `src/AtomUI.Desktop.Controls/Tag/*.cs`, `src/AtomUI.Desktop.Controls/Tag/Themes/*Theme.axaml`；测试 `tests/AtomUI.Desktop.Controls.Tests/Tag`；Gallery `controlgallery/AtomUIGallery/ShowCases/DataDisplay/Tag`.

**风险类型：** Tag 变体、close icon、checkable group/容器生命周期。

- [ ] **Gate A 设计审核：** 分别审计 `Tag`、`CheckableTag`、group/items control 的 owner，确认 icon/content/close 与 checked indicator 是否共享或分离职责；记录 close lifecycle、group selection、container prepare/clear 和 SizeType/颜色 variants。
- [ ] 更新两份控件文档，写明准确的 Descriptor、真实模板/运行时节点、owner 边界、排除的 internal wrapper、生命周期/性能不变量和验证矩阵；运行 LLMS verify 和 `git diff --check`；随后停止并等待用户批准。
- [ ] **Gate B 实现与验证：** 新增 `tests/AtomUI.Desktop.Controls.Tests/Tag/TagSemanticPartTests.cs`，覆盖 basic/closable/checkable/group、集合变更、close/checked states、颜色和尺寸；验证 marker 不随状态切换增删。
- [ ] 运行 Generator Semantic 测试、目标 Desktop 测试、GalleryBase 测试、目标 Gallery 测试、LLMS verify 和 `git diff --check`；涉及 Popup/运行时宿主路径时增加 NativeAOT 验证。
- [ ] **强制停止：** 保持 Tag 的所有实现改动未提交，直到用户明确完成验证并授权提交。

### 任务 6：Timeline

**控件文档：** `docs/controls/desktop/data-display/timeline/overview.md`, `docs/controls/desktop/data-display/timeline/implementation.md`

**证据范围：** `src/AtomUI.Desktop.Controls/Timeline/*.cs`, `src/AtomUI.Desktop.Controls/Timeline/Themes/*Theme.axaml`；测试 `tests/AtomUI.Desktop.Controls.Tests/Timeline`；Gallery `controlgallery/AtomUIGallery/ShowCases/DataDisplay/Timeline`.

**风险类型：** 重复 item、indicator/tail geometry、orientation 模式。

- [ ] **Gate A 设计审核：** 审计 `Timeline` 与 `TimelineItem` owner，确认 item indicator、tail、label/content 的职责与 alternate/right/left/horizontal orientation 一致性；记录 pending item、reverse 和 runtime item generation。
- [ ] 更新两份控件文档，写明准确的 Descriptor、真实模板/运行时节点、owner 边界、排除的 internal wrapper、生命周期/性能不变量和验证矩阵；运行 LLMS verify 和 `git diff --check`；随后停止并等待用户批准。
- [ ] **Gate B 实现与验证：** 新增 `tests/AtomUI.Desktop.Controls.Tests/Timeline/TimelineSemanticPartTests.cs`，覆盖 orientations/modes、pending/reverse、自定义 dot、item add/remove/reset 和 重复 marker cardinality。
- [ ] 运行 Generator Semantic 测试、目标 Desktop 测试、GalleryBase 测试、目标 Gallery 测试、LLMS verify 和 `git diff --check`；涉及 Popup/运行时宿主路径时增加 NativeAOT 验证。
- [ ] **强制停止：** 保持 Timeline 的所有实现改动未提交，直到用户明确完成验证并授权提交。

### 任务 7：TreeView

**控件文档：** `docs/controls/desktop/data-display/tree-view/overview.md`, `docs/controls/desktop/data-display/tree-view/implementation.md`

**证据范围：** `src/AtomUI.Desktop.Controls/TreeView/**/*.cs`, `src/AtomUI.Desktop.Controls/TreeView/Themes/*Theme.axaml`；测试 `tests/AtomUI.Desktop.Controls.Tests/TreeView`；Gallery `controlgallery/AtomUIGallery/ShowCases/DataDisplay/TreeView`.

**风险类型：** 分层虚拟化、异步加载、item/header owner、drag adorner。

- [ ] **Gate A 设计审核：** 审计 `TreeView`、`TreeViewItem`、`TreeViewItemHeader`、switcher 与 drag preview owner；确认 hierarchy item header/content/indent/check/switcher 的职责，记录 async children load、expand/collapse、filter、state replay、drag adorner 和 container recycle。
- [ ] 更新两份控件文档，写明准确的 Descriptor、真实模板/运行时节点、owner 边界、排除的 internal wrapper、生命周期/性能不变量和验证矩阵；运行 LLMS verify 和 `git diff --check`；随后停止并等待用户批准。
- [ ] **Gate B 实现与验证：** 新增 `tests/AtomUI.Desktop.Controls.Tests/TreeView/TreeViewSemanticPartTests.cs`，覆盖 generated hierarchy、async load、expand/collapse、check/select/filter、drag preview、replace/reset 和嵌套 owner 隔离；记录 marker 数量、回收行为，并证明不会保留旧节点。
- [ ] 运行 Generator Semantic 测试、目标 Desktop 测试、GalleryBase 测试、目标 Gallery 测试、LLMS verify 和 `git diff --check`；涉及 Popup/运行时宿主路径时增加 NativeAOT 验证。
- [ ] **强制停止：** 保持 TreeView 的所有实现改动未提交，直到用户明确完成验证并授权提交。

### 任务 8：Slider

**控件文档：** `docs/controls/desktop/data-entry/slider/overview.md`, `docs/controls/desktop/data-entry/slider/implementation.md`

**证据范围：** `src/AtomUI.Desktop.Controls/Slider/*.cs`, `src/AtomUI.Desktop.Controls/Slider/Themes/*Theme.axaml`；测试 `tests/AtomUI.Desktop.Controls.Tests/Slider`；Gallery `controlgallery/AtomUIGallery/ShowCases/DataEntry/Slider`.

**风险类型：** Track/thumb public 子控件、range thumb、pointer 热路径、ToolTip。

- [ ] **Gate A 设计审核：** 审计 `Slider`、`SliderTrack`、`SliderThumb` owner，确认 rail/filled track/handle/marks/tooltip 的职责以及 single/range 与 horizontal/vertical templates；记录 drag hot path、tooltip Popup 和 multiple thumbs cardinality。
- [ ] 更新两份控件文档，写明准确的 Descriptor、真实模板/运行时节点、owner 边界、排除的 internal wrapper、生命周期/性能不变量和验证矩阵；运行 LLMS verify 和 `git diff --check`；随后停止并等待用户批准。
- [ ] **Gate B 实现与验证：** 新增 `tests/AtomUI.Desktop.Controls.Tests/Slider/SliderSemanticPartTests.cs`，覆盖 single/range、orientation、marks、tooltip open-close、min/max/value updates 和 drag；验证 marker 静态、thumb cardinality 与 selector owner。
- [ ] 运行 Generator Semantic 测试、目标 Desktop 测试、GalleryBase 测试、目标 Gallery 测试、LLMS verify 和 `git diff --check`；涉及 Popup/运行时宿主路径时增加 NativeAOT 验证。
- [ ] **强制停止：** 保持 Slider 的所有实现改动未提交，直到用户明确完成验证并授权提交。

### 任务 9：Masonry

**控件文档：** `docs/controls/desktop/layout/masonry/overview.md`, `docs/controls/desktop/layout/masonry/implementation.md`

**证据范围：** `src/AtomUI.Desktop.Controls/Masonry/*.cs`, `src/AtomUI.Desktop.Controls/Masonry/Themes/MasonryTheme.axaml`；测试 `tests/AtomUI.Desktop.Controls.Tests/Masonry`；Gallery `controlgallery/AtomUIGallery/ShowCases/Layout/Masonry`.

**风险类型：** 仅布局倾向、item 生成、可能重分类为不适用。

- [ ] **Gate A 设计审核：** 审计 Masonry owner 与 `MasonryPanel`，判断是否存在 owner 自有的非 root 视觉职责；不能把任意用户 child 虚构为 `.semantic-item`，除非控件实际拥有稳定生成 container。记录 span/layout changes 的真实 ownership。
- [ ] 更新两份控件文档，写明准确的 Descriptor、真实模板/运行时节点、owner 边界、排除的 internal wrapper、生命周期/性能不变量和验证矩阵；运行 LLMS verify 和 `git diff --check`；随后停止并等待用户批准。
- [ ] **Gate B 实现与验证：** 若 Gate A 批准 owner-created 非 root Part，新增 `tests/AtomUI.Desktop.Controls.Tests/Masonry/MasonrySemanticPartTests.cs` 覆盖 add/remove/reset、span changes 与 layout pass；若只有 root 职责，则将 Masonry 重分类为不适用并跳过实现。
- [ ] 运行 Generator Semantic 测试、目标 Desktop 测试、GalleryBase 测试、目标 Gallery 测试、LLMS verify 和 `git diff --check`；涉及 Popup/运行时宿主路径时增加 NativeAOT 验证。
- [ ] **强制停止：** 保持 Masonry 的所有实现改动未提交，直到用户明确完成验证并授权提交。

### 任务 10：Space

**控件文档：** `docs/controls/desktop/layout/space/overview.md`, `docs/controls/desktop/layout/space/implementation.md`

**证据范围：** `src/AtomUI.Desktop.Controls/Space/*.cs`, `src/AtomUI.Desktop.Controls/Space/Themes/*Theme.axaml`；测试 `tests/AtomUI.Desktop.Controls.Tests/Space`；Gallery `controlgallery/AtomUIGallery/ShowCases/Layout/Space`.

**风险类型：** 布局容器、CompactSpace wrapper/add-on、共享 child ownership。

- [ ] **Gate A 设计审核：** 分别审计 `Space` 与 `CompactSpace`，判断普通 Space 是否应重分类为不适用；对 `CompactSpaceItem`、add-on/filler 和 generated wrapper 明确 owner、runtime creation、corner/border coordination，禁止把任意 child 视为 owner Part。
- [ ] 更新两份控件文档，写明准确的 Descriptor、真实模板/运行时节点、owner 边界、排除的 internal wrapper、生命周期/性能不变量和验证矩阵；运行 LLMS verify 和 `git diff --check`；随后停止并等待用户批准。
- [ ] **Gate B 实现与验证：** 对 Gate A 批准的 owner 新增 `tests/AtomUI.Desktop.Controls.Tests/Space/SpaceSemanticPartTests.cs`，覆盖 CompactSpace generated wrappers、add/remove/reset、orientation、filler/add-on、child 替换 与布局/圆角协调；任何只有 root 职责的 owner 重分类为不适用，不生成 descriptor。
- [ ] 运行 Generator Semantic 测试、目标 Desktop 测试、GalleryBase 测试、目标 Gallery 测试、LLMS verify 和 `git diff --check`；涉及 Popup/运行时宿主路径时增加 NativeAOT 验证。
- [ ] **强制停止：** 保持 Space 的所有实现改动未提交，直到用户明确完成验证并授权提交。

### 任务 11：Splitter

**控件文档：** `docs/controls/desktop/layout/splitter/overview.md`, `docs/controls/desktop/layout/splitter/implementation.md`

**证据范围：** `src/AtomUI.Desktop.Controls/Splitter/*.cs`, `src/AtomUI.Desktop.Controls/Splitter/Themes/*Theme.axaml`；测试 `tests/AtomUI.Desktop.Controls.Tests/Splitter`；Gallery `controlgallery/AtomUIGallery/ShowCases/Layout/Splitter`.

**风险类型：** 生成的 panel、drag handle/bar、pointer 热路径。

- [ ] **Gate A 设计审核：** 审计 `Splitter`、`SplitterPanel`、`SplitterHandle`、`SplitterDragBar` owner，确认 pane content 与 generated handle/drag bar 的职责；记录 panel add/remove、collapsible state、orientation 和 resize drag lifecycle。
- [ ] 更新两份控件文档，写明准确的 Descriptor、真实模板/运行时节点、owner 边界、排除的 internal wrapper、生命周期/性能不变量和验证矩阵；运行 LLMS verify 和 `git diff --check`；随后停止并等待用户批准。
- [ ] **Gate B 实现与验证：** 新增 `tests/AtomUI.Desktop.Controls.Tests/Splitter/SplitterSemanticPartTests.cs`，覆盖 generated handle、多 pane、collapse、orientation、drag start/cancel/release、集合变更和 marker 数量；验证 drag 热路径不发生 class 变更。
- [ ] 运行 Generator Semantic 测试、目标 Desktop 测试、GalleryBase 测试、目标 Gallery 测试、LLMS verify 和 `git diff --check`；涉及 Popup/运行时宿主路径时增加 NativeAOT 验证。
- [ ] **强制停止：** 保持 Splitter 的所有实现改动未提交，直到用户明确完成验证并授权提交。

### 任务 12：Breadcrumb

**控件文档：** `docs/controls/desktop/navigation/breadcrumb/overview.md`, `docs/controls/desktop/navigation/breadcrumb/implementation.md`

**证据范围：** `src/AtomUI.Desktop.Controls/Breadcrumb/*.cs`, `src/AtomUI.Desktop.Controls/Breadcrumb/Themes/*Theme.axaml`；测试 `tests/AtomUI.Desktop.Controls.Tests/Breadcrumb`；Gallery `controlgallery/AtomUIGallery/ShowCases/Navigation/Breadcrumb`.

**风险类型：** 生成的 item 容器、separator、collapsed menu。

- [ ] **Gate A 设计审核：** 审计 `Breadcrumb` 与 `BreadcrumbItem` owner，确认 item content/icon/separator、collapsed ellipsis/menu 等职责；记录 data items/容器生命周期、maximum display count、navigation 和 Popup/Flyout 如存在。
- [ ] 更新两份控件文档，写明准确的 Descriptor、真实模板/运行时节点、owner 边界、排除的 internal wrapper、生命周期/性能不变量和验证矩阵；运行 LLMS verify 和 `git diff --check`；随后停止并等待用户批准。
- [ ] **Gate B 实现与验证：** 新增 `tests/AtomUI.Desktop.Controls.Tests/Breadcrumb/BreadcrumbSemanticPartTests.cs`，覆盖 explicit/data items、separator templates、collapsed state、add/remove/reset、navigation 和 popup open-close 适用时。
- [ ] 运行 Generator Semantic 测试、目标 Desktop 测试、GalleryBase 测试、目标 Gallery 测试、LLMS verify 和 `git diff --check`；涉及 Popup/运行时宿主路径时增加 NativeAOT 验证。
- [ ] **强制停止：** 保持 Breadcrumb 的所有实现改动未提交，直到用户明确完成验证并授权提交。

### 任务 13：Pagination

**控件文档：** `docs/controls/desktop/navigation/pagination/overview.md`, `docs/controls/desktop/navigation/pagination/implementation.md`

**证据范围：** `src/AtomUI.Desktop.Controls/Pagination/*.cs`, `src/AtomUI.Desktop.Controls/Pagination/Themes/*Theme.axaml`；测试 `tests/AtomUI.Desktop.Controls.Tests/Pagination`；Gallery `controlgallery/AtomUIGallery/ShowCases/Navigation/Pagination`.

**风险类型：** 生成的 nav item、多个 public 变体、内部 ComboBox Popup、SizeType。

- [ ] **Gate A 设计审核：** 审计 `Pagination`、`SimplePagination`、nav/nav item、quick jumper 和 page-size ComboBox item 的 owner，
  确认 previous/next/page/ellipsis/size changer/quick jump regions；记录 page 数量 rebuild、simple/default templates、Popup 和
  SizeType。ComboBox 作为被排除的嵌套控件保持独立，不得获得 Descriptor 或内部 marker。
- [ ] 更新两份控件文档，写明准确的 Descriptor、真实模板/运行时节点、owner 边界、排除的 internal wrapper、生命周期/性能不变量和验证矩阵；运行 LLMS verify 和 `git diff --check`；随后停止并等待用户批准。
- [ ] **Gate B 实现与验证：** 新增 `tests/AtomUI.Desktop.Controls.Tests/Pagination/PaginationSemanticPartTests.cs`，覆盖 default/simple、page 数量与 value 变化、生成的 nav item 生命周期、size changer Popup、quick jumper、所有尺寸和 marker 数量。
- [ ] 运行 Generator Semantic 测试、目标 Desktop 测试、GalleryBase 测试、目标 Gallery 测试、LLMS verify 和 `git diff --check`；涉及 Popup/运行时宿主路径时增加 NativeAOT 验证。
- [ ] **强制停止：** 保持 Pagination 的所有实现改动未提交，直到用户明确完成验证并授权提交。

### 任务 14：Steps

**控件文档：** `docs/controls/desktop/navigation/steps/overview.md`, `docs/controls/desktop/navigation/steps/implementation.md`

**证据范围：** `src/AtomUI.Desktop.Controls/Steps/*.cs`, `src/AtomUI.Desktop.Controls/Steps/Themes/*Theme.axaml`；测试 `tests/AtomUI.Desktop.Controls.Tests/Steps`；Gallery `controlgallery/AtomUIGallery/ShowCases/Navigation/Steps`.

**风险类型：** 生成的 item、indicator public 子控件、navigation 模式、orientation。

- [ ] **Gate A 设计审核：** 审计 `Steps`、`StepsItem`、`StepsItemIndicator`、navigation arrow 和 panel owners；确认 title/subtitle/description/icon/tail/indicator regions 覆盖 default/navigation/inline 和 horizontal/vertical modes。
- [ ] 更新两份控件文档，写明准确的 Descriptor、真实模板/运行时节点、owner 边界、排除的 internal wrapper、生命周期/性能不变量和验证矩阵；运行 LLMS verify 和 `git diff --check`；随后停止并等待用户批准。
- [ ] **Gate B 实现与验证：** 新增 `tests/AtomUI.Desktop.Controls.Tests/Steps/StepsSemanticPartTests.cs`，覆盖 所有模式/orientations、status/current changes、item 集合变更、custom indicator 和 重复 marker/cardinality。
- [ ] 运行 Generator Semantic 测试、目标 Desktop 测试、GalleryBase 测试、目标 Gallery 测试、LLMS verify 和 `git diff --check`；涉及 Popup/运行时宿主路径时增加 NativeAOT 验证。
- [ ] **强制停止：** 保持 Steps 的所有实现改动未提交，直到用户明确完成验证并授权提交。

### 任务 15：TabControl

**控件文档：** `docs/controls/desktop/navigation/tab-control/overview.md`, `docs/controls/desktop/navigation/tab-control/implementation.md`

**证据范围：** `src/AtomUI.Desktop.Controls/TabControl/*.cs`, `src/AtomUI.Desktop.Controls/TabControl/Themes/BaseTabControlTheme.axaml`, `CardTabControlTheme.axaml`, `TabControlTheme.axaml`, `BaseTabItemTheme.axaml`, `CardTabItemTheme.axaml`, `TabItemTheme.axaml` 和共享 scroll/overflow themes；测试 `tests/AtomUI.Desktop.Controls.Tests/TabControl`；Gallery `controlgallery/AtomUIGallery/ShowCases/Navigation/TabControl`.

**风险类型：** 生成的容器、content presenter、overflow Popup、reorder、与被排除 TabStrip 共享源码。

- [ ] **Gate A 设计审核：** 审计 `TabControl`/`CardTabControl`、`TabItem`、scroll viewer/overflow item owners，确认 tab header/icon/close/content/ink/overflow regions；记录 selection、overflow Popup、reorder 和容器生命周期。共享 BaseTab 主题中的改动必须只命中
  `TabControl` / `TabItem` owner，不得给 `TabStrip` 添加 Descriptor、marker 或通过共享模板间接形成 Semantic Part。
- [ ] 更新两份控件文档，写明准确的 Descriptor、真实模板/运行时节点、owner 边界、排除的 internal wrapper、生命周期/性能不变量和验证矩阵；运行 LLMS verify 和 `git diff --check`；随后停止并等待用户批准。
- [ ] **Gate B 实现与验证：** 新增 `tests/AtomUI.Desktop.Controls.Tests/TabControl/TabControlSemanticPartTests.cs`，覆盖 line/card、generated/explicit items、content 切换、overflow popup、close/reorder、collection reset 和 nested owner 隔离。
- [ ] 运行 Generator Semantic 测试、目标 Desktop 测试、GalleryBase 测试、目标 Gallery 测试、LLMS verify 和 `git diff --check`；涉及 Popup/运行时宿主路径时增加 NativeAOT 验证。
- [ ] **强制停止：** 保持 TabControl 的所有实现改动未提交，直到用户明确完成验证并授权提交。

## 批次收尾

- [ ] 确认 15 个控件家族分别拥有用户授权的独立提交。
- [ ] 运行完整 Desktop Controls、Generator、GalleryBase 和 Gallery 测试工程，并执行集合/虚拟化回归筛选。
- [ ] 运行 LLMS verify、NativeAOT publish 和 `git diff --check`。
- [ ] 更新总计划清单，不创建批次提交。
