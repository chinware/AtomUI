# AtomUI Desktop Controls Semantic Part 全量改造设计

本文档定义 AtomUI Desktop Controls 全量接入 Semantic Part 的范围、设计方法、审核关卡、性能边界和交付纪律。
Semantic Part 的公共模型、Selector 契约和生成器规则分别由
[Semantic Part 系统设计](../../architecture/systems/theming/semantic-parts.md) 与
[Semantic Part Generator](../../modules/generator/semantic-part-generator.md) 维护；本文档只负责把这些系统级能力稳定地
落到每一个控件家族。

## 1. 目标与完成定义

本轮改造的目标不是让每个控件都拥有相同数量或相同名称的 Part，而是逐一审计控件的真实视觉职责，只公开能够跨主题、
状态、平台和后续模板重构长期保持的 Semantic Part 契约。

一个控件家族只有同时满足以下条件，才算完成：

1. `overview.md` 与 `implementation.md` 已根据真实源码、叶子主题、测试和 Gallery 定义 Semantic Part 契约。
2. 控件设计文档已经由用户审核通过，之后才开始实现。
3. 所有声明均由 `[SemanticPart]` 和生成式 descriptor 表达，所有静态节点均使用 `Classes.semantic-*="True"`。
4. Desktop、Browser、派生主题、运行时节点、Popup、Overlay、独立 TopLevel 和 ItemContainer 路径按实际适用范围完成。
5. descriptor、marker、selector、布局、容器回收、Popup、性能和 Gallery 延迟创建验证覆盖实际风险。
6. `changelog.md` 与 LLMS 人工源文档同步，`docs/AI/generated` 未被手工修改。
7. 实现保持未提交状态，用户验收通过并明确要求后，才为该控件家族创建一个独立 commit。

## 2. 范围基线

截至 2026-08-12，`docs/controls/desktop` 下共有 78 个正式控件文档叶子。

| 状态 | 数量 | 范围 |
| --- | ---: | --- |
| 已完成基线 | 1 | `Button` |
| 本轮待改造 | 73 | 五个批次中的控件家族 |
| 不适用 | 4 | `Icon`、`FlexPanel`、`Grid / Row / Col`、`Watermark` |

`Button` 是首个完整样例，用于校验 descriptor、静态 marker、Selector、尺寸协调和 Gallery Preview 的全链路；它不作为
其他控件 Part 命名的机械模板。

### 2.1 不适用判定

以下控件当前不新增 Semantic Part：

| 控件 | 判定依据 | 重新评估触发条件 |
| --- | --- | --- |
| `Icon` | 视觉职责就是控件 root，没有独立且可长期承诺的内部区域。 | 出现独立 public 子 Control 或稳定的多区域视觉契约。 |
| `FlexPanel` | 布局 Panel 只管理 children 排列，没有 owner 自有的非 root 视觉职责。 | 新增稳定装饰、控制柄或 public presenter。 |
| `Grid / Row / Col` | 栅格职责由 owner 与 children 布局共同完成，虚构 `content/item` 不形成有价值契约。 | 新增 owner 自有、跨模板稳定的视觉区域。 |
| `Watermark` | 当前可定制职责由 root API 和渲染参数表达，不存在适合 Selector 定制的独立视觉节点。 | 水印内容成为稳定独立 Visual 或 public 子 Control。 |

不适用不是永久豁免。触发条件出现时，必须重新执行本设计的文档审核关卡，不能直接补 marker。

## 3. 最小交付单位

任务以正式控件文档叶子代表的“控件家族”而不是单个 `.cs` 文件为单位。一个家族包含共同维护同一用户能力的 public
owner、public child control、internal presenter、item container、runtime-created visual、Popup/Overlay host、所有叶子主题、
测试和 Gallery。两个文档叶子即使共享源码目录，也必须分别完成 Gate A；共享基础文件只有在相关设计均获批后才能修改。

典型家族边界：

- `LineEdit` 包含 `LineEdit`、`TextBox`、`TextArea` 及其内部 clear/reveal/resize 节点；`SearchEdit` 使用同一源码目录，
  但作为独立文档叶子另行审核其搜索按钮和 decorated box 契约。
- `Modal / Dialog` 包含 `Dialog`、`DialogSurface`、Overlay host、Window host、header、resizer 和 button box。
- `DataGrid` 包含 grid、row、cell、header、presenter、filter flyout 和虚拟化/回收路径。
- `SplitButton` 与 `DropdownButton` 分别作为独立用户控件家族审核，但必须核对其复用的 Button 主题与语义边界。

同一家族的 descriptor 与模板 marker 必须一起审核和实现，不能让父控件与其容器、Popup 或派生模板在不同 commit 中短暂
形成不完整公共契约。

## 4. 三阶段审核状态机

每个控件家族严格按以下状态推进：

```text
已规划
  -> 文档设计中
  -> 文档审核中
  -> 文档已批准
  -> 实现中
  -> 实现审核中
  -> 实现已批准
  -> 已授权提交
  -> 已提交
```

### 4.1 Gate A：控件设计文档

进入 Gate A 前必须读取：

- public/protected API、默认值、继承关系和 `[TemplatePart]`。
- 所有叶子 `*Theme.axaml`，包括 typed `BasedOn`、派生模板和 Browser 变体。
- public child control、internal presenter、runtime-created visual 和 adorner。
- Popup、Flyout、Overlay、独立 Window/TopLevel 与 resource bridge。
- ItemsControl 的 container 创建、prepare、clear、recycle 和虚拟化路径。
- 既有测试、Gallery API/Token/ShowCase 和当前控件文档。

现有控件文档中的 `LLMS 语义区域` 或 `Semantic Parts` 表只能作为待核对的旧文档内容，不能证明运行时 descriptor 已存在。
只有 public owner 的 `[SemanticPart]`、生成 descriptor、真实模板 marker 和对应测试共同成立时，才表示控件已经支持 Semantic
Part。旧分类模型推导出的 `item/header/content/motion` 等通用区域必须重新从源码事实审计，不能直接转写为公共契约。

先更新：

先更新对应批次任务中列出的该控件 `overview.md` 与 `implementation.md` 精确路径。

只有 Semantic Part 模型同时跨越公共区域模型、多个 owner、多个视觉根、复杂生命周期和独立验证矩阵时，才创建
`semantic-part-design.md`。正式控件文档只描述最终设计，不写任务状态、候选方案、排期或 checklist。

`overview.md` 的 Semantic Parts 表必须逐项定义：

| 字段 | 要求 |
| --- | --- |
| `Part` / `Path` | camelCase 语义名；描述职责，不泄露节点层级。 |
| `Selector` | `root` 使用 owner；其余使用唯一 `.semantic-*`。 |
| `ContractType` | Setter 可稳定依赖的最低 public 类型。 |
| `Cardinality` | `Single`、`Optional` 或 `Multiple`，与所有模板/状态一致。 |
| `Customization` | `Root`、`Selector` 或经论证的 `SelectorAndTheme`。 |
| `CrossVisualRoot` | 是否需要穿过 owner 可达的 Popup/Overlay/TopLevel。 |
| `RuntimeCreated` | 是否由 C# 或容器生命周期创建。 |
| `ThemePropertyName` | 仅真实 public 子 Control 支持完整替换时存在。 |
| 实现节点 | 每个叶子模板或运行时路径中的真实节点及 owner。 |
| 兼容边界 | 哪些内部节点明确不属于公共 Part。 |

`implementation.md` 必须定义 marker 所有权、模板映射、运行时创建点、生命周期、Popup 路径、尺寸协调、性能/AOT 和验证
不变量。文档完成后立即停止，等待用户审核；未获批准不得修改源码、主题、测试或 Gallery。

### 4.2 Gate B：实现与验收

文档获批后，按测试先行完成：

1. 为 descriptor、cardinality、ContractType 和 selector 命中增加失败测试。
2. 在 public owner 上增加 `[SemanticPart]`；不为 `root` 显式声明 marker。
3. 在所有静态叶子模板上使用 `Classes.semantic-*="True"`，禁止动态 Binding marker。
4. 运行时节点只使用生成的 selector class 常量，不写重复字符串，不扫描 VisualTree 维护 Part。
5. 覆盖 Desktop、Browser、派生模板、状态替代节点、Popup 和 container 生命周期。
6. 为布局型 Setter 验证 owner 与 Part 的 Measure/Arrange、Min/Max、Padding、shape 和 SizeType 协调。
7. 为 Gallery 增加真正延迟创建的 Semantic Parts Tab 内容，未打开 Tab 时不创建 demo、descriptor item 或 Preview。
8. 更新该控件 `changelog.md`，运行 LLMS verify，但不手改生成文档。

实现完成后必须保持未提交，向用户报告文件、测试和已知风险，等待实际运行与视觉验收。

### 4.3 Gate C：用户授权提交

只有用户明确确认该控件实现没有问题并要求创建 commit 后，才能：

- 只 stage 当前控件家族的文档、源码、主题、测试和 Gallery 文件。
- 检查工作树中用户或其他任务的改动，不混入无关文件。
- 创建一个 scoped commit。

“完成代码”“测试通过”“文档审核通过”均不等于提交授权。批次完成也不自动创建聚合 commit。

## 5. Part 设计方法

### 5.1 从职责而不是节点名称出发

候选 Part 必须同时满足：

1. 用户能够理解并有实际定制价值。
2. 对应职责能在所有受支持主题、状态和平台上保持。
3. AtomUI 愿意把名称、ContractType 和 cardinality 作为版本兼容契约。
4. 不要求用户知道 `PART_*`、internal 类型或偶然的 Grid/Border 层级。

如果控件只有 root 职责，Gate A 必须用源码和模板事实证明，并把该控件重新归类为“不适用”。当前生成器只为至少声明
一个非 root Part 的 owner 隐式加入 `root`，不生成独立的 root-only descriptor。不得为了覆盖率虚构 `content`、`item`、
`wrapper` 或 `container`，也不得只为获得 root descriptor 创建无价值 Part。

### 5.2 Owner 边界

父控件只承诺自己拥有的区域。嵌套 public 子 Control 具有独立行为、主题或替换价值时，应由子 Control 声明自己的
descriptor；父控件不能用多层 `/template/` 穿透其内部实现。

同一用户控件家族可以包含多个 descriptor owner。控件文档必须明确：

- 哪个 public type 拥有哪个 Part。
- Part 是父 owner 的 region，还是 public child control 的 root/region。
- runtime-created container 如何获得生成 marker。
- container recycle 后 marker、状态和订阅是否保持正确。

### 5.3 Popup、Overlay 与独立宿主

Popup 不因跨视觉根而自动需要额外 Theme 属性。默认仍使用 selector 模型；`CrossVisualRoot=true` 只记录 owner 可达的
实际视觉根和 Gallery/工具解析方式。

设计必须区分：

- owner 模板内可达的 `Popup`：从 owner 的模板作用域定位 Popup，再从公开 `Popup.Child` 进入目标根。
- Flyout presenter：确认 presenter 是 owner 的实际 public/内部视觉职责，不能仅按全局 TopLevel 搜索。
- Overlay layer：确认 overlay visual 的 owner、attach/detach 和 marker 创建位置。
- 服务创建的 Message、Notification、Dialog、Splash Window：由具体 host/session 明确 ownership；Gallery 通过显式
  `AdditionalRoots` 演示，不给生产控件增加 Preview 专用接口。

只有真实 public 子 Control 需要完整 `ControlTheme` 替换时，才使用 `SelectorAndTheme` 和 `ThemePropertyName`。

## 6. 布局与 SizeType 协调

Semantic Style 使用 Avalonia 原生优先级。Part Setter 已命中不代表父级布局边界会自动让出空间；布局型 Part 必须把
owner 根、尺寸档和中间节点作为一个完整测量系统审计。

每个涉及 `Padding`、`Margin`、`Width`、`Height`、Min/Max 或字体/图标尺寸的 Part，文档必须覆盖：

- owner 是否实现 `ISizeTypeAware` 或 `ICustomizableSizeTypeAware`。
- 预设尺寸是否以 `MinHeight` 建立基线，还是存在有意不可扩展的固定 `Height`。
- `SizeType=Custom` 的自然测量和本地属性优先级。
- 中间 wrapper 的固定尺寸、clip、padding 和自定义 Measure/Arrange。
- Circle、Round、icon-only、loading、空内容和多行内容。
- Desktop、Browser 与派生主题的尺寸一致性。

默认方向是使用 Min/Max 建立设计基线，让合法 Semantic Setter 参与自然测量。若控件的公共几何必须固定，文档必须明确
其不可扩展理由和允许定制的属性边界，不能让用户误以为布局型 Setter 能改变最终外形。

## 7. 性能与 NativeAOT 边界

生产控件的 Semantic Part 增量成本只允许来自 Avalonia 已有 class 匹配和静态 descriptor 注册：

- 不在控件实例构造时读取 registry。
- 不为 Semantic Part 增加 VisualTree 扫描、反射、运行时 AXAML 解析或程序集扫描。
- 不增加常驻 layout、scroll、pointer、timer 或 TopLevel 监听器。
- marker 必须静态存在，不能在状态变化时反复增删。
- runtime-created visual 在既有创建路径一次性添加生成常量，不建立额外索引。
- 默认 AtomUI ControlTheme 不使用 `.semantic-*` 驱动内置样式，避免把公共 marker 变成主题内部热路径依赖。

高密度或虚拟化控件还必须提供以下证据：

- 单 container 的 marker 数量和新增 class 匹配范围。
- prepare/clear/recycle 不产生重复 class、重复订阅或 retained owner。
- 大数据、滚动和展开/收起前后的实例数量与布局性能无明显回退。
- Gallery Preview 只处理当前可见实例，并遵守 32 个 Adorner 预算。

所有实现禁止依赖反射发现 Part，确保 trimming 和 NativeAOT 下 descriptor、常量和注册路径确定。

## 8. Gallery 契约

每个适用控件最终都应提供一个 Semantic Parts Tab，遵循
[Semantic Part Preview](../../gallery/authoring/semantic-part-preview.md)：

- Examples 仍是默认 Tab。
- Semantic 内容由 `IDataTemplate` 或显式 factory 延迟创建。
- 未首次进入 Semantic Parts Tab 前，不创建 demo control、registry query、Part item、snippet 或 Adorner。
- Part 说明来自该控件 descriptor 和本地化职责描述，不复制 selector/cardinality 元数据。
- Popup、Overlay 或独立 host 示例只显式提供该 demo 自己创建的 additional roots。
- Info 打开后才创建技术元数据和代码片段。

Gallery 接入是控件交付的一部分，因为它同时验证用户可理解性、真实 marker、跨视觉根和延迟创建；Gallery 不参与生产
控件的运行时依赖闭包。

## 9. 验证矩阵

每个控件至少执行：

```bash
dotnet test tests/AtomUI.Generator.Tests/AtomUI.Generator.Tests.csproj --framework net10.0 --no-restore --filter FullyQualifiedName~SemanticPart
dotnet test tests/AtomUI.Toolkits.GalleryBase.Tests/AtomUI.Toolkits.GalleryBase.Tests.csproj --framework net10.0 --no-restore
dotnet run --project tools/AtomUI.Docs.LLMsGenerator/AtomUI.Docs.LLMsGenerator.csproj -- verify --config docs/AI/generated/llms.config.json
git diff --check
```

控件与 Gallery 的定向测试命令由对应批次任务给出的真实测试目录、项目和 namespace 确定；执行前先用 `rg` 定位现有
测试类，不能运行一个示意 filter 并把零测试误报为通过。风险附加验证：

| 风险 | 附加验证 |
| --- | --- |
| Browser theme | Desktop 与 Browser 每个叶子模板 marker/cardinality 一致。 |
| Layout/SizeType | Large/Middle/Small/Custom、Min/Max、shape 和布局型 Setter。 |
| ItemContainer | 多 item、空集合、replace/reset、虚拟化滚动、prepare/clear/recycle。 |
| Popup/Flyout | closed/open/reopen、placement、Popup.Child、关闭后无订阅/引用。 |
| Overlay/Window | host attach/detach、session close、additional roots 和多窗口隔离。 |
| High density | 大数据滚动、marker/class 数量、容器回收和性能基线。 |
| Optional package | 对应项目测试、主题注册、Gallery 引用和 NativeAOT publish。 |

涉及新的 AXAML 控件、运行时注册、Popup/Window 或 optional package 时，实施阶段还必须执行 Gallery NativeAOT publish。

## 10. 批次策略

| 批次 | 数量 | 目标 | 主要风险 |
| --- | ---: | --- | --- |
| Batch 1 | 20 | 基础视觉与状态控件，建立可复用审核节奏。 | 派生主题、尺寸、adorner、复合 Button。 |
| Batch 2 | 20 | 集合、容器与导航结构。 | container、runtime-created、虚拟化、多重 cardinality。 |
| Batch 3 | 18 | 输入、选择与日期/时间类控件。 | SizeType、Popup、内部 editor、候选项容器。 |
| Batch 4 | 12 | 独立宿主、Overlay 与 Window。 | 跨视觉根、session 生命周期、多 TopLevel。 |
| Batch 5 | 3 | 高密度复合控件。 | 大量 container、Popup、虚拟化和性能。 |

批次表达审核顺序，不构成批量提交边界。始终一次只推进一个控件家族，并在 Gate A 与 Gate B 后等待用户确认。

## 11. 兼容性纪律

Semantic Part 是新的公共主题契约。以下变化默认属于破坏性变更：

- 删除或重命名 Part/path/selector class。
- 收窄或更换不兼容的 `ContractType`。
- 把 `Single` 变成 `Optional`/`Multiple`，或反向改变可观察数量语义。
- 默认模板、Browser 模板、派生模板或 Popup 路径丢失 marker。
- 把原本 owner 可达的 Part 移入不可达独立 host，或改变 `CrossVisualRoot`。
- 取消 typed theme property 或改变 public child control 的替换边界。

因此每个 Part 在文档审核时必须按长期 API 的标准评估。没有足够事实支持的候选区域应留在 Composition Model，而不是先
公开再修正。
