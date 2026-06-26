# AtomUI 控件文档规范

本文档定义 AtomUI 控件级研发文档的结构、内容边界、写作规则和审查标准，适用于 `AtomUI.Controls`、`AtomUI.Desktop.Controls`、DataGrid、ColorPicker 以及其他控件包。

控件文档用于描述控件的最新设计状态、公共契约、交互模型、状态模型、主题架构、Token 边界、内部实现原理和维护规则。它不替代面向最终用户的站点文档、API reference、正式发布 `CHANGELOG.md` 或代码注释。

控件文档也是 AtomUI LLMS 文档的唯一人工维护源。面向 AI 编程工具的 `llms.txt`、`llms-full-cn.txt`、`llms-semantic-cn.md`、单控件 `controls/<control>/index-<lang>.md` 和单控件 `controls/<control>/semantic-<lang>.md` 必须从控件文档、Gallery API / Token 表、Gallery 示例源码片段和源码结构生成，不维护第二套手写控件文档。

## 文档目录结构

每个控件必须使用独立目录承载文档：

```text
docs/controls/<platform>/<category>/<control>/
├── overview.md
├── implementation.md
├── token.md
└── changelog.md
```

要求：

- `overview.md` 必须存在，用于描述控件设计定位、公共契约、状态模型、视觉主题关系和维护入口。
- `implementation.md` 必须存在，用于描述特定控件的内部实现原理、源码职责边界、关键状态流和维护规则，以提升控件可维护性。
- `changelog.md` 必须存在，用于记录控件级设计、API、主题契约、Token 和实现结构变化。
- `token.md` 仅当控件存在专属 Token 或复杂主题变量关系时存在。
- 内容过多时可以继续拆分专题文档，但 `overview.md` 只保留设计与契约主线，`implementation.md` 只保留实现原理主线，并链接专题文档。
- 分类入口文档必须链接控件目录中的主要文档。

## 写作原则

控件文档必须采用严谨、稳定、工程化的语言。

必须遵守：

- 描述最新设计状态，不写历史过程。
- 不使用“未来”“后续”“演进方向”等路线图表述。
- 不堆砌属性清单；属性必须服务于 API 模型、状态模型或兼容边界。
- 不重复全局规范；通用规则应链接全局规范文档。
- 不记录临时讨论、实现流水账或无长期维护价值的细节。
- API、主题契约、Token、template part、伪类等必须表述为稳定契约。
- `overview.md` 不展开具体代码实现，具体实现原理进入 `implementation.md`。
- `implementation.md` 不重复公共 API 清单、Token 全局规则或用户文档内容，只解释当前控件内部实现为什么这样组织、状态如何流动、维护时必须守住哪些边界。

历史变化统一进入控件级 `changelog.md`。

## LLMS 支持原则

AtomUI 控件文档必须支持生成中文 LLMS 产物。LLMS 产物是面向 AI 编程工具的消费层，用于稳定提供控件用途、公共 API、AXAML 结构、Template Part、伪类、状态模型、Token 边界、Gallery 示例和 AOT 约束。

LLMS 生成器的多控件项目、商业控件项目、配置、可见性和验证设计见 [AtomUI LLMS 文档生成器设计](llms-generator-design.md)。

中文 LLMS 第一阶段必须生成：

```text
llms.txt
llms-full-cn.txt
llms-semantic-cn.md
```

每个纳入 LLMS 覆盖范围的控件必须能生成：

```text
controls/<control>/index-cn.md
controls/<control>/semantic-cn.md
```

文件职责：

- `llms.txt` 是导航索引，链接聚合文档、单控件文档、语义文档、工程规范和 Gallery 相关入口。
- `llms-full-cn.txt` 由所有单控件完整中文文档聚合生成。
- `llms-semantic-cn.md` 由所有单控件中文语义文档聚合生成。
- `controls/<control>/index-cn.md` 是单控件完整中文文档，面向 AI 理解控件如何使用。
- `controls/<control>/semantic-cn.md` 是单控件中文语义文档，面向 AI 理解控件的 AXAML、ControlTemplate、运行时组合结构、Template Part、伪类、状态和 Token 结构。

LLMS 单控件目录规则：

```text
docs/AI/llms/
├── llms.txt
├── llms-full-cn.txt
├── llms-semantic-cn.md
└── controls/
    └── <control>/
        ├── index-cn.md
        └── semantic-cn.md
```

要求：

- 目录名使用 `controls`，与 AtomUI 控件文档和源码术语保持一致，不使用 `components`。
- `<control>` 目录只表达控件身份，例如 `button`、`date-picker`、`data-grid`。
- 语言维度只能体现在文件名后缀中，例如 `index-cn.md`、`semantic-cn.md`、`index-en.md`、`semantic-en.md`。
- 不使用 `<control>-<lang>` 这类把控件名和语言混在一起的目录名。
- `index-<lang>.md` 是该控件在指定语言下的 LLMS 主入口；`semantic-<lang>.md` 是该控件在指定语言下的语义结构文档。

生成产物必须带有“由源文档生成，不要手工编辑”的标记。人工修改应回到以下来源：

- `docs/controls/<platform>/<category>/<control>/overview.md`
- `docs/controls/<platform>/<category>/<control>/implementation.md`
- `docs/controls/<platform>/<category>/<control>/token.md`
- Gallery `*ApiDataGrid`
- Gallery `*DesignTokenDataGrid`
- Gallery ShowCase 示例和源码片段 catalog
- 控件源码、主题文件和 Token 类型

不得新增一套人工维护的 LLMS 专用控件文档。若生成内容不足，必须补强上述源文档或结构化数据，而不是直接修改生成结果。

### Gallery 示例与 LLMS

LLMS 的“使用示例”必须来自 Gallery 源码查看功能使用的同一组 `ShowCaseItem` 示例。Gallery 示例既是用户可运行的演示，也是 LLMS 示例代码的来源。

控件维护 Gallery 示例时必须遵守：

- 每个适合进入文档的稳定示例必须是独立 `ShowCaseItem`。
- 文档级示例必须设置稳定 `SourceKey`，例如 `button-type`、`button-loading`、`date-picker-range`。`SourceKey` 不应随示例顺序、标题文案或布局调整变化。
- 示例代码必须优先展示 public API，不依赖 internal/private 控件、临时测试辅助类型或只服务 Gallery shell 的实现细节。
- 示例内容应尽量保持可复制。展示布局可以存在，但不能让 `ShowCasePanel`、`ShowCaseItem`、Gallery scenario shell 成为 LLMS 输出的主要内容。
- 示例标题、描述和展示文案继续使用 Gallery 本地化资源。LLMS 生成器负责按目标语言解析资源文本。
- 如果某个示例只服务调试、压力测试或内部行为展示，不应标记为文档级稳定示例。

LLMS 生成器可以从现有 `ShowCaseItem` 自动抽取候选示例，但控件文档长期要求是：稳定示例必须有 `SourceKey`，并能通过 Gallery 源码查看获得 AXAML / code-behind / ViewModel 片段。

LLMS 生成必须遵守：

- 当前状态优先，不输出历史过程。
- 公共契约优先，不输出无维护价值的私有细节。
- 生成内容必须保持稳定顺序，便于 diff 和 review。
- 链接必须使用稳定相对路径或发布路径。
- 单控件完整文档和语义文档的覆盖范围必须一致。
- 控件文档、Gallery 和源码发生冲突时，生成流程应失败或报告差异，不得静默选择其中一方。
- 不从运行时扫描程序集生成 LLMS 内容；生成应在构建期或文档生成期完成。
- 不引入 NativeAOT 不友好的运行时反射路径。

## `overview.md` 结构

`overview.md` 必须按以下结构书写：

```text
# <Control> <平台>架构设计

1. 控件定位
2. 设计语言
3. API 与契约模型
4. 行为与状态模型
5. 视觉与主题模型
6. 控件家族或集成关系
7. 兼容性不变量
8. 专项模型
9. 文档导航、LLMS 导出与验证策略
```

章节要求：

- 控件定位：定义控件职责边界，说明控件是什么、不是什么。
- 设计语言：描述控件表达的产品语义和视觉语义。
- API 与契约模型：说明公共属性、事件、方法、template part、伪类和主题入口的契约边界，不机械罗列成员。
- 行为与状态模型：说明 hover、pressed、disabled、loading、focus、keyboard、motion 等交互语义，以及 public API 如何归一为 effective state。
- 视觉与主题模型：说明模板、Theme、Token、SharedToken 的总体关系和不可破坏的视觉契约，不展开具体 AXAML 结构。
- 控件家族或集成关系：说明与派生控件、组合控件、Form、Compact、Browser theme 等关系。
- 兼容性不变量：列出优化和扩展时必须保持不变的 API、行为和渲染。
- 专项模型：记录控件特有模型，例如 Button 的 `Color / Variant`。
- 文档导航、LLMS 导出与验证策略：链接 `implementation.md`、`token.md`、`changelog.md`，说明 LLMS 生成来源，并按 Public API、状态、AXAML、Token、文档分层列出验证要求。

无对应内容的章节不能删除，应写明“不适用”及原因。

第 9 节必须包含 LLMS 导出来源表：

```md
| LLMS 内容 | 来源 | 说明 |
| --- | --- | --- |
| 单控件完整文档 | overview.md + Gallery API / Token / ShowCase | 生成 `controls/<control>/index-cn.md` |
| 单控件语义文档 | overview.md + implementation.md + Themes 文件夹 + theme/template 信息 | 生成 `controls/<control>/semantic-cn.md` |
| API 表 | Gallery ApiDataGrid 或源码 public surface | 不在 overview.md 中手工复制完整表 |
| Design Token 表 | Gallery DesignTokenDataGrid 或 Token 类型 | 不在 token.md 中手工复制生成表 |
| 示例 | Gallery ShowCase + source snippet catalog | 只引用稳定示例 |
| 源码索引 | implementation.md | 用于定位控件源码、主题和测试 |
```

`overview.md` 中的 API 描述仍应按语义分组说明公共契约，不要求机械列出全部属性。完整 API 表应由 Gallery 表格或源码结构生成。

生成的 `controls/<control>/index-cn.md` 必须使用以下结构：

```md
# <Control>

## 概述
## 包与命名空间
## 何时使用
## 公共 API
## 事件与命令
## 使用示例
## 状态模型
## 主题与 Design Token
## AOT 与裁剪注意事项
## 源码索引
## 相关文档
```

内容来源要求：

- “概述”“何时使用”“状态模型”主要来自 `overview.md`。
- “公共 API”“事件与命令”来自 Gallery API 表或源码 public surface。
- “使用示例”来自 Gallery ShowCase 和源码片段 catalog。
- “主题与 Design Token”来自 `token.md`、Gallery Token 表和 Token 类型。
- “AOT 与裁剪注意事项”来自 `implementation.md` 和全局 AOT 规范。
- “源码索引”来自 `implementation.md` 的源码文件结构。

生成的 `controls/<control>/semantic-cn.md` 必须使用以下结构：

```md
# <Control> 语义结构

## Semantic Parts
## Abstract AXAML Structure
## Composition Model
## Template Parts
## Pseudo Classes
## State Flow
## Theme and Token Boundaries
## Customization Boundaries
```

AtomUI 的 semantic 文档描述 AXAML、ControlTemplate、运行时组合结构、Template Part、伪类、状态流和 Token 语义，不描述 Web DOM。

`Semantic Parts` 表格必须按控件语义区域组织：

```md
| Part | AtomUI 节点 | 职责 | 相关 API | 相关 Token | 稳定性 |
| --- | --- | --- | --- | --- | --- |
| root | <root control> | 控件根语义区域 | ... | ... | stable |
| content | PART_ContentPresenter | 内容展示区域 | Content | ... | stable |
```

要求：

- `Part` 是语义名，不一定等于 Template Part 名。
- `AtomUI 节点` 可以是 Template Part、主题节点、控件类或抽象区域。
- 只记录稳定语义区域，不暴露用户不应依赖的内部临时节点。
- 如果某个节点是内部实现细节，必须在“稳定性”或“Customization Boundaries”中说明不可依赖。

`Abstract AXAML Structure` 必须遵守：

- 只能由生成器从真实 `ControlTheme` / `ControlTemplate` 抽取。
- 生成器通过 `implementation.md` 的源码索引定位 `*Theme.axaml`，再读取模板节点。
- 不允许根据 `Semantic Parts` 表、控件分类或控件名称生成伪 XML。
- 不允许输出 `SemanticPart` 这类并不存在于 AXAML 模板中的占位节点。
- 控件没有可定位的 ControlTheme 模板时，该章节只说明未生成结构，并链接 Template Parts、主题文件和源码索引。
- 如果需要稳定抽象结构，必须先在控件源文档和源码索引中明确对应 ControlTheme / ControlTemplate 来源，而不是手工修改生成产物。

`Composition Model` 必须描述 public 控件与内部协作对象之间的运行时组合关系。它的第一信息源是控件源码目录下的 `Themes/` 文件夹：

- 优先阅读同一控件家族的 `*Theme.axaml` 和 `*Themes.axaml`。
- 从多个 `ControlTheme` 中识别 public control、internal control、item container、adorner、presenter、popup host、motion actor、template part 和主题聚合关系。
- `implementation.md`、源码索引、C# 创建逻辑、`CreateContainerForItemOverride()`、`OnApplyTemplate` 和 `PART_` 只作为解释与校验补充。
- 不允许仅凭控件名称、控件分类或通用模式发明内部协作节点。

`Composition Model` 解决以下场景：

- public 控件本身 `ControlTheme` 很薄，但行为由内部容器、presenter、adorner、popup host、motion actor 或 item container 完成。
- 控件结构分散在多个主题文件、C# 创建逻辑、item container 生成、adorner 挂载或数据对象中。
- Agent 只阅读 `Semantic Parts` 和 `Abstract AXAML Structure` 时，会低估控件结构复杂度，进而误导用户直接依赖内部类型或错误理解状态流。

`Composition Model` 至少包含：

````md
## Composition Model

### 控件角色图

```text
<PublicControl>
  -> <InternalControlOrPresenter> (<theme source>)
     -> <ElementType>#<TemplatePartName> (<stability>)
        -> <ChildElementType>#<ChildName> (<stability>)
```

### 协作节点

| 节点 | 类型 | 来源 | 生命周期 owner | 影响的 public API | 稳定性 | Agent 使用边界 |
| --- | --- | --- | --- | --- | --- | --- |
| ... | ... | ... | ... | ... | public / template-stable / internal-observable / private | ... |
````

`控件角色图` 必须是模板层级树，不是节点清单。生成器应按真实 `ControlTemplate` XML 父子关系输出结构：`ControlTheme` target 是角色入口，模板视觉节点以 `ElementType#Name` 表示；没有 `Name` 的节点只输出 `ElementType`。同一个 `ControlTheme` 中的 template part、presenter、motion actor、adorner indicator 和布局节点不得被打平成同级节点。存在多个模板变体时可以输出多个根树，但必须保持每个变体内部的父子关系。

稳定性要求：

- `public` 表示用户可直接使用和依赖的 public 控件、数据对象或 API。
- `template-stable` 表示稳定 template part、ControlTheme key 或主题节点，可用于主题维护但不能随意改名。
- `internal-observable` 表示内部协作对象，影响用户可观察行为；Agent 可以用它理解结构，但不能指导用户直接依赖。
- `private` 表示私有实现细节，默认不进入公开 LLMS 输出；如必须出现，只能用于说明不可依赖边界。

不同控件族的组合结构重点：

| 控件族 | 代表控件 | 文档重点 |
| --- | --- | --- |
| Shell / Overlay | Drawer、Modal、Notification、Message、PopupConfirm | public control、container、mask、surface、motion、close/action 区。 |
| Adorner / Decorator | Badge、Watermark | decorated target、adorner、indicator、motion actor、offset、placement。 |
| Items / Container | Breadcrumb、Steps、Menu、ListView、TabControl | owner control、item data、item container、separator、trigger、selection/navigation flow。 |
| Popup Input | Select、Cascader、DatePicker、TimePicker、ComboBox、AutoComplete | input shell、popup host、panel、item presenter、selection/value sync。 |
| Presenter / Family | ProgressBar、Skeleton、Card、Descriptions | public API 到内部 presenter、visual unit 或家族子控件的映射。 |

没有额外运行时组合层的控件也应明确说明：该控件主要由 public 控件和 ControlTheme 模板直接表达，没有额外内部协作对象。生成器不得因缺少复杂结构而发明组合节点。

## `implementation.md` 结构

`implementation.md` 描述特定控件的内部实现原理，用于帮助维护者理解源码结构、状态流转、生命周期和关键算法。它不替代代码注释，不记录临时实现过程，也不作为用户 API 文档。

`implementation.md` 必须按以下结构书写：

```text
# <Control> <平台>实现原理

1. 实现定位
2. 源码文件结构
3. 核心类职责
4. 状态与数据流
5. 组合结构模型
6. 生命周期与模板接入
7. 交互与事件处理
8. 内部算法与关键流程
9. 资源、性能与 AOT 边界
10. 维护不变量
11. 测试与验证
```

章节要求：

- 实现定位：说明本文档覆盖的内部实现范围，以及哪些细节仍应直接阅读源码。
- 源码文件结构：说明控件相关 `.cs`、`.axaml`、Token、helper、handler、内部 view 的职责边界。
- 核心类职责：描述主要类、内部接口、handler、presenter、decorator、数据节点等协作关系。
- 状态与数据流：描述 public API、internal state、effective state、ItemsSource、Children、selection、checked、expanded、filter 等状态如何流动。
- 组合结构模型：优先基于控件 `Themes/` 文件夹描述 public 控件、内部协作控件、item container、adorner、popup host、motion actor、presenter、数据对象和 template part 的运行时关系。
- 生命周期与模板接入：描述构造、初始化、加载、卸载、template part 获取、事件订阅释放、container 准备和资源绑定规则。
- 交互与事件处理：描述 pointer、keyboard、focus、context menu、drag/drop、motion、popup 等内部事件路径。
- 内部算法与关键流程：描述维护者必须理解的遍历、布局、状态回放、同步、缓存、渲染等关键流程，不逐行复述代码。
- 资源、性能与 AOT 边界：描述动态资源、事件订阅、binding、缓存、异步任务、反射或生成器相关约束。
- 维护不变量：列出重构、优化和修 bug 时不能破坏的内部行为顺序、状态一致性、生命周期释放和主题契约。
- 测试与验证：列出对应测试、Gallery 走查点、AOT 或发布验证要求。

要求：

- 只写当前实现原理，不写历史过程；历史变化进入 `changelog.md`。
- 不把私有方法逐个改写成说明书；只记录维护者必须知道的稳定结构、关键路径和不变量。
- 不重复 `overview.md` 的设计语言、API 模型和公共契约；必要时只引用它们作为实现入口。
- 不重复 `token.md` 的 Token 分类和语义；实现中使用 Token 的路径可以链接 `token.md`。
- 当控件实现较简单时，也应保留该文档，并用简短章节说明实现没有额外内部模型，避免维护者误判文档缺失。

LLMS 生成 `llms-full-cn.txt` 时，只从 `implementation.md` 抽取以下内容：

- 源码文件结构。
- 核心类职责。
- 状态与数据流。
- 组合结构模型。
- 生命周期与模板接入。
- 资源、性能与 AOT 边界。
- 维护不变量。
- 测试与验证入口。

生成文档不得把 `implementation.md` 中所有内部细节无差别塞入 LLMS。私有方法说明、局部实现细节、临时维护记录不应进入 LLMS 产物。

`implementation.md` 的“组合结构模型”章节应使用和 LLMS semantic 文档一致的角色图与协作节点表。维护要求：

- 组合节点必须优先来自控件 `Themes/` 文件夹中的 `ControlTheme`、template part、内部主题控件和主题聚合关系。
- C# 源码中的动态创建、item container 生成、adorner 挂载和 popup host 管理用于补充主题文件无法表达的运行时关系。
- 不把 internal 类型写成用户可直接使用的 API。
- 对于 internal 但用户可观察的结构，使用 `internal-observable` 并说明 Agent 只能用于理解行为和维护边界。
- 对于 template part 或 ControlTheme key，使用 `template-stable` 并说明变更需要同步主题、实现和文档。
- 对于简单控件，明确说明没有额外组合层，不允许为了补齐章节发明内部节点。

## `token.md` 结构

`token.md` 只记录控件专属 Token 内容，不重复全局 Token 系统规则。通用 Token 分层、命名、计算、Theme Variables、预设色规则统一链接 [AtomUI 控件 Token 设计规范](control-token-guidelines.md)。

`token.md` 必须按以下结构书写：

```text
# <Control> Token 设计

1. 定位
2. Token 分类
3. 控件专项模型中的 Token 使用
4. 控件家族影响
5. 兼容性要求
6. 验证策略
```

要求：

- Token 分类必须按控件语义组织，而不是按代码顺序机械罗列。
- Token 文档必须说明哪些主题和控件家族引用这些 Token。
- 不允许把实例状态、交互状态或 `EffectiveXxx` 状态写成 Token。
- 不允许在控件 Token 中展开颜色、variant、状态的组合 Token。
- 没有专属 Token 的控件不需要创建 `token.md`，但 `overview.md` 中仍应说明其 Theme 是否直接使用 SharedToken。
- LLMS 中的 Token 表格应从 Gallery Token 表、Token 类型或生成数据中抽取；`token.md` 只解释这些 Token 的语义边界。

如果控件没有专属 `token.md`，`overview.md` 必须明确说明：

- 该控件没有专属 Token；或
- 该控件复用家族 Token；或
- 该控件直接使用 SharedToken / 主题资源。

LLMS 生成时不得把缺失 `token.md` 解释为文档缺失，除非 `overview.md` 没有给出上述说明。

## `changelog.md` 结构

每个控件必须维护控件级 changelog。

格式：

```md
# <Control> Changelog

本文档记录 <Control> 控件级设计、API、主题契约、Token 和实现结构的变化。
它不替代仓库根目录 CHANGELOG.md，也不作为正式版本发布说明。

## YYYY-MM-DD

- Docs
  - ...
- API
  - ...
- Theme
  - ...
- Token
  - ...
```

记录范围：

- API 模型变化。
- 主题契约变化。
- Token 分类、语义或边界变化。
- template part、伪类、状态模型变化。
- 控件家族协同规则变化。
- 文档结构、实现文档边界和重要设计决策变化。

不记录：

- 临时讨论。
- 纯格式化。
- 无长期维护价值的实现细节。
- 正式 release changelog 条目。

`changelog.md` 不进入 `llms-full-cn.txt` 主体。LLMS 文档描述当前稳定状态，不描述历史过程。`llms.txt` 可以链接控件级 `changelog.md`，但聚合正文默认不包含 changelog 内容。

## 链接规则

控件目录内文档必须互相链接：

- `overview.md` 链接 `implementation.md`、`token.md` 和 `changelog.md`。没有 `token.md` 时只链接 `implementation.md` 和 `changelog.md`。
- `implementation.md` 链接 `overview.md`、`changelog.md`，并在涉及控件专属 Token 时链接 `token.md`。
- `token.md` 链接 `overview.md`、`implementation.md`、`changelog.md` 和 [AtomUI 控件 Token 设计规范](control-token-guidelines.md)。
- `changelog.md` 不解释当前设计，只记录变化。

分类入口文档必须链接控件目录：

```md
- [Button 桌面版架构设计](button/overview.md)
- [Button 桌面版实现原理](button/implementation.md)
- [Button Token 设计](button/token.md)
- [Button Changelog](button/changelog.md)
```

分类入口文档还必须能被 LLMS 索引用作分类导航来源。分类入口应保持控件列表完整、顺序稳定，并按控件目录链接主要维护文档。LLMS 生成器可以基于分类入口或文件系统目录生成分类索引，但两者不一致时应报告错误。

## 审查标准

控件文档 review 时按以下标准检查：

- 是否描述最新状态，而不是历史过程。
- 是否明确公共 API、事件、方法和兼容边界。
- 是否有清晰的状态模型。
- 是否说明模板职责和不可破坏的 template part。
- 是否有独立 `implementation.md` 描述控件内部实现原理、源码职责边界、生命周期和维护不变量。
- `overview.md` 是否避免承载过重实现细节。
- 是否把通用规则放到全局规范，而不是控件文档重复。
- 是否维护控件级 `changelog.md`。
- 是否包含分层验证策略。
- 是否存在“未来”“后续”“演进方向”等不适合架构文档的表述。
- `overview.md` 是否能支撑单控件完整文档生成。
- `overview.md` 是否包含 LLMS 导出来源表。
- `implementation.md` 是否提供稳定源码索引、状态流、生命周期和 AOT 边界。
- 结构复杂控件的 `implementation.md` 是否基于 `Themes/` 文件夹提供 Composition Model，并明确 internal 协作对象的稳定性和 Agent 使用边界。
- `token.md` 是否能解释 Token 语义；没有 `token.md` 的控件是否在 `overview.md` 明确说明原因。
- Template Part、伪类、主题资源和 Token 是否足够支撑 semantic 文档生成。
- Gallery API 表、Token 表和 ShowCase 示例是否能与控件文档对应。
- 是否避免维护第二套 LLMS 专用手写控件文档。

## 验证要求

控件文档改动至少验证：

```bash
git diff --check
```

同时检查：

- 新增文档没有尾随空白。
- 所有相对链接存在。
- 分类入口文档已更新。
- 只改文档时，不应误改控件实现代码或主题文件。

涉及 LLMS 支持的文档改动，除 `git diff --check` 外，还应运行 LLMS 生成器的验证命令。命令名称由实际生成器确定：

```bash
dotnet run --project <LLMS generator project> -- verify
```

验证内容至少包括：

- `llms.txt` 可生成。
- `llms-full-cn.txt` 可生成。
- `llms-semantic-cn.md` 可生成。
- 每个覆盖控件都能生成 `controls/<control>/index-cn.md`。
- 每个覆盖控件都能生成 `controls/<control>/semantic-cn.md`。
- 单控件完整文档和语义文档覆盖范围一致。
- 链接有效。
- 必填章节非空。
- 无人工编辑生成产物造成的未同步差异。
