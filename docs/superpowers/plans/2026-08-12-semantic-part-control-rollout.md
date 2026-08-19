# Semantic Part 控件全量改造实施计划

> **供智能体执行者使用：** 必须使用 `superpowers:executing-plans` 在当前会话中逐个控件家族执行本计划，不得分派 subagent。每个控件在修改源码前和创建提交前都必须经过用户审核。

**目标：** 为所有适用的 AtomUI Desktop 控件家族建立基于代码事实的 Semantic Part 契约，同时保持性能、NativeAOT、主题、布局、Popup、容器和兼容性边界。

**架构：** 全局 Semantic Part 系统继续采用 selector-first 和生成器驱动方案。每个控件在自己的 `overview.md` 与 `implementation.md` 中拥有公共 Part 契约；只有这两份文档通过审核后才能开始实现。Gallery Preview 保持产品无关、Descriptor 驱动并真正延迟创建。

**技术栈：** .NET 10、Avalonia 12、AtomUI Desktop Controls、AtomUI Generator、AXAML ControlTheme、xUnit v3、Shouldly、Avalonia Headless、AtomUI Gallery、NativeAOT。

## 全局约束

- 只允许在 `/Users/chinboy/Projects/dotnet/AtomUIV6/.worktrees/semantic` 的 `feature/semantic` 分支中工作。
- 遵循[全量改造设计](../specs/2026-08-12-semantic-part-control-rollout-design.md)和正式的 [Semantic Part 系统架构](../../architecture/systems/theming/semantic-parts.md)。
- 准入只以 Ant Design 最新稳定发布源码中公开且实际消费的 Semantic DOM API 为准；当前基线为 2026-08-12 的 6.6.0。
- 官网展示、普通 `className` / `style`、ConfigProvider、internal schema、Props 间接继承或嵌套子组件透传不得作为准入证据。
- 每次只处理一个控件家族。当前控件等待文档或实现审核时，不得开始下一个控件，除非用户明确调整顺序。
- 修改源码、主题、测试、Gallery 或 changelog 前，必须先更新该控件的 `overview.md` 和 `implementation.md` 并获得批准。
- 只有主题符合仓库独立维护标准时，才能创建控件专项设计文档。
- 不得手工编辑 `docs/AI/generated`。
- 不得向生产控件引入运行时 VisualTree 搜索、反射、运行时 AXAML 解析、动态 marker binding 或永久监听器。
- AtomUI 默认主题不得使用 Semantic selector 驱动内置样式。
- 单个控件实现完成后必须保持所有变更未提交；只有用户完成验证并明确授权后才能创建提交。

---

## 1. 计划文件与职责

| 文件 | 职责 |
| --- | --- |
| `docs/superpowers/specs/2026-08-12-semantic-part-control-rollout-design.md` | 稳定的改造范围、门禁、风险模型、性能和兼容性规则。 |
| `docs/superpowers/plans/2026-08-12-semantic-part-control-rollout.md` | 总体顺序、状态、通用执行循环和跨批次收尾。 |
| `docs/superpowers/plans/2026-08-12-semantic-part-batch-1-basic-controls.md` | 16 个基础视觉与状态控件家族。 |
| `docs/superpowers/plans/2026-08-12-semantic-part-batch-2-collections-containers.md` | 15 个集合、容器和导航控件家族。 |
| `docs/superpowers/plans/2026-08-12-semantic-part-batch-3-input-selection.md` | 15 个输入和选择控件家族。 |
| `docs/superpowers/plans/2026-08-12-semantic-part-batch-4-hosts-windows.md` | 9 个 Popup、Overlay 和服务宿主控件家族。 |
| `docs/superpowers/plans/2026-08-12-semantic-part-batch-5-high-density.md` | `NavMenu` 和 `DataGrid` 两个性能敏感控件家族。 |

正式控件文档不得将这些计划作为唯一设计来源。它们应链接系统架构，并描述控件自身的当前契约。

## 2. 基线清单

### 已完成基线

- [x] `Button`：已具备 Descriptor、Desktop/Browser 静态 marker、selector/布局测试、现行设计文档和延迟创建的 Gallery Preview。

### 不适用

- [x] `Avatar`、`Carousel`、`Expander`、`GroupBox`、`Rate`、`Watermark`。
- [x] `Icon`、`SplitButton`、`FlexPanel`、`Grid / Row / Col`、`TabStrip`。
- [x] `ButtonSpinner`、`ComboBox`、`DropdownButton`、`BorderBeam`、`Splash`。
- [x] `Menu`、`WindowTitleBar`、`Window`。

逐项公开 API 证据、产品职责映射和重新评估条件以全量改造设计的“排除映射”为准。不得因 AtomUI 模板内部存在
可定制节点而绕过准入 Gate。

### 待执行批次

- [x] 第一批：基础控件，共 16 个家族。
- [ ] 第二批：集合与容器，共 16 个家族。
- [ ] 第三批：输入与选择，共 15 个家族。
- [ ] 第四批：Popup 与独立宿主，共 9 个家族。
- [ ] 第五批：高密度控件，共 2 个家族。

合计待改造：58 个控件家族。

## 3. 单控件强制执行循环

以下流程适用于各批次清单中的每一个控件。批次任务会给出准确的文档、源码/主题范围、风险和附加验证要求。

### 单控件 Gate A：文档设计

**文件：**

- 修改：批次任务列出的准确 `overview.md`。
- 修改：批次任务列出的准确 `implementation.md`。
- 仅在符合创建条件时新增：与上述文件同目录的 `semantic-part-design.md`。

**输入证据：** 控件任务列出的 public/protected API、全部叶子主题、运行时 Visual 创建路径、子控件、Popup/Overlay 路径、item 生命周期、测试和 Gallery 证据。

**产出：** 经过批准的现行设计契约，其中包含准确的 Part/path、selector、ContractType、cardinality、customization、cross-root/runtime 标志、适用时的 ThemePropertyName、节点映射、兼容性边界和验证矩阵。

- [ ] 阅读控件文档、每个 public owner/子类型、每个叶子 `*Theme.axaml`、现有测试和 Gallery 页面。
- [ ] 建立 owner 类型、模板变体、`PART_*`、伪类、运行时 Visual、Popup/Overlay root、SizeType 接口和容器生命周期的事实表。
- [ ] 排除会暴露偶然 wrapper、internal 类型名或不稳定模板层级的候选区域。
- [ ] 在 `overview.md` 中写入最终 Semantic Parts 表以及定制与兼容性摘要。
- [ ] 在 `implementation.md` 中写入准确的节点 ownership、marker 放置位置、运行时创建路径、生命周期、布局、性能/AOT 和验证不变量。
- [ ] 对文档改动运行 LLMS verify 和 `git diff --check`。
- [ ] 停止并向用户展示文档差异。在获得明确批准前，不得编辑源码、主题、测试、Gallery 或 changelog。

### 单控件 Gate B：实现

**前置条件：** 用户已经明确批准该控件家族的 Gate A 文档设计。

**文件：**

- 修改：Gate A 已批准的准确 public owner 源码和主题家族。
- 测试：该控件批次任务列出的现有测试目录/工程和聚焦的 Semantic Part 测试文件。
- 修改：控件现有的 Gallery 页面与本地化资源。
- 修改：控件的 `changelog.md`。

**产出：** 与已批准文档一致的生成式 Descriptor 声明、静态/运行时 marker、聚焦测试和真正延迟创建的 Gallery Semantic Parts 示例。

- [ ] 先编写失败的 Descriptor 测试，验证准确的 Part 数量/顺序、ContractType、cardinality、标志和 `root` 行为。
- [ ] 为每个已批准 marker 和模板变体编写失败的模板/运行时测试。
- [ ] 向 public owner 类型添加 `[SemanticPart]` 声明，并依赖生成的常量和 Descriptor 注册。
- [ ] 为静态模板节点添加 `Classes.semantic-*="True"`，为运行时创建节点使用生成常量添加 marker。
- [ ] 当契约开放布局属性时，验证布局 Setter 与 SizeType、Min/Max、shape 和 owner Measure/Arrange 的关系。
- [ ] 当批次风险要求时，验证 Popup/Overlay 的打开-关闭-重新打开流程，或容器的 prepare-clear-recycle 流程。
- [ ] 向 Gallery 页面添加 `SemanticPartsContentTemplate`，首次选择 Semantic Parts Tab 前不得实例化任何 Semantic 内容。
- [ ] 更新 `changelog.md`；运行目标控件测试、Gallery 测试、LLMS verify 和 `git diff --check`。
- [ ] 当批次任务要求验证 Popup、Window、可选包或运行时注册时，执行 NativeAOT publish。
- [ ] 保持全部实现改动未提交并停止，向用户展示测试和视觉证据。

### 单控件 Gate C：提交

**前置条件：** 用户已经明确批准 Gate B 的未提交实现，并要求创建提交。

- [ ] 检查 `git status`，识别并隔离用户的无关改动。
- [ ] 只暂存已批准控件家族的文档、源码、主题、测试、Gallery，以及仓库有意跟踪的生成构建产物。
- [ ] 审查已暂存差异，并生成一条范围明确且符合 AtomUI 风格的提交信息。
- [ ] 只为该控件家族创建一个提交。
- [ ] 报告 commit hash，不触碰无关改动。

## 4. 批次顺序

### 任务 1：第一批 - 基础控件

**计划：** [第一批任务清单](2026-08-12-semantic-part-batch-1-basic-controls.md)

- [x] 完成 `Badge` 的 Gate A 至 Gate C 全流程。
- [x] 严格按照第一批计划中的顺序逐个处理控件家族。
- [x] 16 个家族全部提交后，运行完整 Desktop Controls、Gallery、Generator 和 LLMS 检查，再将该批次标记为完成。

### 任务 2：第二批 - 集合与容器

**计划：** [第二批任务清单](2026-08-12-semantic-part-batch-2-collections-containers.md)

- [ ] 只有第一批形成稳定审核节奏后才能开始，除非用户明确调整优先级。
- [ ] 每个适用家族都必须提供容器和运行时创建 marker 的生命周期证据。
- [ ] 15 个家族全部提交后，运行集合/虚拟化回归测试和完整通用检查。

### 任务 3：第三批 - 输入与选择

**计划：** [第三批任务清单](2026-08-12-semantic-part-batch-3-input-selection.md)

- [ ] 开放 input frame/content/icon 区域前，必须分析 SizeType 和布局 Setter。
- [ ] candidate、option、calendar 和 time panel 必须提供 Popup 打开-关闭-重新打开的证据。
- [ ] 15 个家族全部提交后，运行输入、选择、本地化和 Gallery NativeAOT 验证。

### 任务 4：第四批 - Popup 与独立宿主

**计划：** [第四批任务清单](2026-08-12-semantic-part-batch-4-hosts-windows.md)

- [ ] 每项设计获批前，必须明确 Visual root ownership 和释放路径。
- [ ] 测试多宿主隔离、关闭/detach 清理和 Gallery `AdditionalRoots`，不得引入生产 Preview API。
- [ ] 9 个家族全部提交后，运行 Popup/Overlay 检查和 Gallery NativeAOT publish。

### 任务 5：第五批 - 高密度控件

**计划：** [第五批任务清单](2026-08-12-semantic-part-batch-5-high-density.md)

- [ ] 每个控件进入 Gate B 前，记录改造前 marker、容器和性能基线。
- [ ] 必须证明可见实例数量受限、回收正确，并且没有永久监听器或索引。
- [ ] 2 个家族全部提交后，运行各自完整测试工程、性能检查、Gallery 和 NativeAOT 验证。

## 5. 通用验证命令

文档门禁：

```bash
dotnet run --project tools/AtomUI.Docs.LLMsGenerator/AtomUI.Docs.LLMsGenerator.csproj -- verify --config docs/AI/generated/llms.config.json
git diff --check
```

Desktop 控件实现：

```bash
dotnet test tests/AtomUI.Generator.Tests/AtomUI.Generator.Tests.csproj --framework net10.0 --no-restore --filter FullyQualifiedName~SemanticPart
dotnet test tests/AtomUI.Desktop.Controls.Tests/AtomUI.Desktop.Controls.Tests.csproj --framework net10.0 --no-restore
dotnet test tests/AtomUI.Toolkits.GalleryBase.Tests/AtomUI.Toolkits.GalleryBase.Tests.csproj --framework net10.0 --no-restore
dotnet test tests/AtomUIGallery.Tests/AtomUIGallery.Tests.csproj --framework net10.0 --no-restore
```

DataGrid 实现：

```bash
dotnet test tests/AtomUI.Desktop.Controls.DataGrid.Tests/AtomUI.Desktop.Controls.DataGrid.Tests.csproj --framework net10.0 --no-restore
```

Popup、Window、可选包或运行时敏感改动的 NativeAOT 验证：

```bash
pwsh -NoLogo -NoProfile -File controlgallery/AtomUIGallery.Desktop/scripts/PublishToLocal.ps1 -publishRootPath /tmp/atomui-gallery-aot-semantic -runtime osx-arm64 -buildType Release -publishAot true
```

## 6. 全量改造收尾

- [ ] 确认 57 个家族都通过各自经用户授权的提交达到 `Committed` 状态。
- [ ] 重新扫描 public 控件和全部叶子主题，检查未声明的 `.semantic-*`、缺少的已批准 marker，以及 Descriptor 与文档不一致。
- [ ] 确认 20 个排除控件仍然没有通过最新稳定发布源码公开 API 准入 Gate。
- [ ] 运行全部通用测试、DataGrid 测试、LLMS verify、NativeAOT publish 和 `git diff --check`。
- [ ] 审核每个 Gallery 页面，确认保持 Examples-first 行为，并且选择 Tab 前不会实例化 Semantic Preview。
- [ ] 输出最终兼容性与性能摘要；除非用户明确要求，否则不得额外创建 squash 或批次提交。
