# AtomUI LLMS 文档生成器设计

本文档定义 AtomUI LLMS 文档生成器的设计定位、配置模型、输入契约、输出契约、生成流程、验证规则和扩展边界。控件文档规范见 [AtomUI 控件文档规范](control-documentation-guidelines.md)，AI 协作和生成文件维护规则见 [AtomUI AI 协作规范](agent-guidelines.md)。

## 1. 工具定位

LLMS 文档生成器是 `tools/AtomUI.Docs.LLMsGenerator` 下的确定性文档工具。它不是运行时库、不是 Roslyn source generator，也不是 AI 内容总结器。

生成器的职责是把控件项目中已经维护好的源文档、Gallery 元数据、示例源码和源码索引转换为面向 AI 编程工具消费的 LLMS 文档。源文档仍然是唯一人工维护点；生成产物只作为消费层存在。

工具必须支持多个控件项目共用同一套生成规则。AtomUI Desktop 控件是第一个接入项目，商业控件项目也通过配置文件接入，而不是复制一套生成器或在工具中写死项目路径。

## 2. 设计目标

- 保持 `docs/controls/**` 作为控件文档的唯一人工维护源。
- 支持开源控件包和商业控件包使用同一生成器。
- 通过配置声明控件文档根目录、输出目录、Gallery 根目录、源码根目录、分类顺序、语言和可见性策略。
- 生成稳定、可 diff、可 review 的 Markdown 输出。
- 在源文档不足、输出过期、路径结构错误、可见性越界或生成产物被手工修改时失败。
- 不从运行时程序集扫描类型，不依赖运行时反射，不访问网络。
- 不把商业控件的内部 API、内部示例或内部控件泄漏到公开 LLMS 输出。

## 3. 非目标

- 不根据源码或 AXAML 自动推断缺失的公共 API、Token 语义或 semantic parts。
- 不根据 semantic parts、控件名称或通用分类发明抽象 AXAML 结构。
- 不把私有实现细节完整复制到 LLMS 输出。
- 不替代面向最终用户的站点文档或 API reference。
- 不在生成器中维护控件专属知识库。
- 不合并不同项目的公开和内部输出，除非配置明确声明同一输出目标和同一可见性策略。

## 4. 工具位置与命令

工具项目位置：

```text
tools/AtomUI.Docs.LLMsGenerator/
```

测试项目位置：

```text
tests/AtomUI.Docs.LLMsGenerator.Tests/
```

基础命令：

```bash
dotnet run --project tools/AtomUI.Docs.LLMsGenerator/AtomUI.Docs.LLMsGenerator.csproj -- generate --config docs/AI/llms.config.json
dotnet run --project tools/AtomUI.Docs.LLMsGenerator/AtomUI.Docs.LLMsGenerator.csproj -- verify --config docs/AI/llms.config.json
```

`generate` 写入生成产物。`verify` 在内存或临时目录中重新生成产物，并与工作区文件和源文档契约比对；发现差异时以非零退出码失败。

## 5. 配置模型

每个控件项目必须提供一个 LLMS 配置文件。AtomUI Desktop 的默认位置为：

```text
docs/AI/llms.config.json
```

配置示例：

```json
{
  "schemaVersion": 1,
  "projectId": "AtomUI.Desktop",
  "displayName": "AtomUI Desktop Controls",
  "defaultLanguage": "cn",
  "languages": ["cn"],
  "outputRoot": "docs/AI/llms",
  "visibility": {
    "default": "public",
    "included": ["public"],
    "excluded": ["internal", "private"]
  },
  "controlSets": [
    {
      "id": "desktop",
      "platform": "desktop",
      "docsRoot": "docs/controls/desktop",
      "galleryRoot": "controlgallery/AtomUIGallery/ShowCases",
      "sourceRoots": [
        "src/AtomUI.Desktop.Controls",
        "src/AtomUI.Desktop.Controls.DataGrid",
        "src/AtomUI.Desktop.Controls.ColorPicker",
        "src/AtomUI.Desktop.Controls.Extras"
      ],
      "categoryOrder": [
        "general",
        "layout",
        "navigation",
        "data-entry",
        "data-display",
        "feedback",
        "window",
        "other"
      ]
    }
  ]
}
```

配置字段语义：

| 字段 | 说明 |
| --- | --- |
| `schemaVersion` | 配置格式版本，生成器只接受明确支持的版本。 |
| `projectId` | 控件项目稳定 ID，用于诊断、聚合标题和缓存键。 |
| `displayName` | 输出文档中展示的项目名称。 |
| `defaultLanguage` | 当前默认输出语言，中文第一阶段为 `cn`。 |
| `languages` | 要生成的语言列表。第一阶段只启用 `cn`。 |
| `outputRoot` | LLMS 输出根目录。 |
| `visibility` | 输出可见性策略，控制公开和内部控件的边界。 |
| `controlSets` | 一个项目内的控件集合。每个集合有独立文档根、Gallery 根和源码根。 |
| `docsRoot` | 控件源文档目录，形如 `docs/controls/<platform>`。 |
| `galleryRoot` | Gallery 示例和源码片段的根目录。 |
| `sourceRoots` | 控件源码根目录，用于源码索引和链接校验。 |
| `categoryOrder` | 聚合输出中的分类顺序。 |

商业控件项目使用同一配置结构。商业项目可以把配置放在自己的项目目录中，例如：

```text
commercial/AtomUI.Charts/docs/AI/llms.config.json
```

工具通过 `--config` 显式选择目标项目，不隐式扫描所有商业项目。

## 6. 可见性模型

可见性是商业控件接入的强制边界。生成器只生成配置允许的控件和内容。

控件级可见性来源按以下优先级解析：

```text
overview.md 元数据中的 LLMS 可见性
> control manifest 中的 visibility
> 配置文件 visibility.default
```

控件文档中的元数据表可以包含：

```md
| LLMS 可见性 | public |
```

可见性取值：

| 值 | 说明 |
| --- | --- |
| `public` | 可以进入公开 LLMS 输出。 |
| `internal` | 只允许进入内部输出。 |
| `private` | 不进入 LLMS 输出。 |

当配置 `visibility.included` 不包含某个控件的可见性时，生成器跳过该控件。若被跳过控件仍出现在聚合输出或链接中，`verify` 必须失败。

公开输出不得包含 internal/private 控件名称、源码路径、示例、API 或 Token 信息。内部输出必须使用单独的 `outputRoot` 或单独的配置文件，避免与公开输出混合。

## 7. 输入契约

每个控件目录遵循控件文档规范：

```text
docs/controls/<platform>/<category>/<control>/
├── overview.md
├── implementation.md
├── token.md
└── changelog.md
```

`token.md` 只在控件存在专属 Token 或复杂主题变量关系时存在。没有 `token.md` 的控件必须在 `overview.md` 说明它没有专属 Token、复用家族 Token，或直接使用 SharedToken / 主题资源。

### 7.1 overview.md

生成器从 `overview.md` 读取：

- 控件定位和典型使用场景。
- 包名、.NET 命名空间、AXAML 命名空间、Gallery 页面、控件状态、LLMS 可见性。
- API 与契约模型。
- 行为与状态模型。
- 视觉与主题模型。
- LLMS semantic parts 表格。
- LLMS 导出来源表。
- 验证策略。

`overview.md` 必须显式维护 semantic parts 表格。生成器不从 AXAML 自动发明 semantic parts。

`Abstract AXAML Structure` 不由 `overview.md` 手写维护，也不从 semantic parts 反推。生成器只能从 `implementation.md` 源码索引定位到的 `*Theme.axaml` 中读取真实 `ControlTheme` / `ControlTemplate`，并把模板中的稳定视觉节点摘要为抽象结构。无法定位 ControlTheme 或无法解析 ControlTemplate 的控件，不输出伪 XML，只输出“未定位到可生成抽象 AXAML 结构的 ControlTheme 模板”的说明。

复杂控件还必须提供 `Composition Model`。`Composition Model` 描述 public 控件、内部协作控件、item container、adorner、popup host、motion actor、presenter、数据对象和模板 part 之间的运行时组合关系。它不是 AXAML 模板结构，不要求每个节点都出现在同一个 `ControlTemplate` 中；它用于帮助 AI 编程工具理解控件如何由多个内部对象协同完成 public API 行为。

`Composition Model` 的第一信息源是控件源码目录下的 `Themes/` 文件夹。生成器应优先扫描同一控件家族的 `*Theme.axaml` 和 `*Themes.axaml`，识别 public control、internal control、item container、adorner、presenter、popup host、motion actor、template part 和主题聚合关系。`implementation.md`、源码索引、`new Xxx()`、`CreateContainerForItemOverride()`、`OnApplyTemplate`、`PART_` 和 item container 类型用于解释与静态校验；不得仅凭控件分类或名称发明内部协作结构。若控件没有额外内部组合结构，该章节应明确说明“该控件主要由 public 控件和 ControlTheme 模板直接表达，没有额外运行时组合层”。

### 7.2 implementation.md

生成器从 `implementation.md` 读取：

- 源码文件结构。
- 核心类职责。
- 状态与数据流。
- 组合结构模型。
- 生命周期与模板接入。
- 资源、性能与 AOT 边界。
- 维护不变量。
- 测试与验证入口。

生成器只抽取维护者和 AI 编程工具需要理解的稳定结构，不把私有方法说明或局部实现流水账完整写入 LLMS。

### 7.3 token.md

当 `token.md` 存在时，生成器读取：

- Token 定位。
- Token 分类。
- 控件专项模型中的 Token 使用。
- 控件家族影响。
- 验证策略。

Token 表格优先来自 `token.md`、Token 类型或生成数据；`token.md` 解释语义边界。

### 7.4 Gallery 示例元数据

Gallery 元数据只用于生成稳定示例和源码片段。默认 AtomUI Desktop 适配器读取以下来源：

- `*ShowCase.axaml`
- ShowCase source snippet catalog

公共 API 表来自 `overview.md` 语义摘要和源码 public surface；Design Token 表来自 `token.md`、Token 类型或生成数据。Gallery 不再收集 API / Token DataGrid sidecar 或 ShowCase ViewModel rows。

如果 Gallery 示例元数据、源码 public surface、Token 数据和控件文档冲突，生成器不得静默选择一方，必须报告差异。

### 7.5 Gallery 源码示例

LLMS 使用示例复用 Gallery 源码查看功能的数据契约。Gallery 源码查看以 `ShowCasePanel` + `ShowCaseItem` 为示例边界，为每个示例生成 `ShowCaseCodeSnippetGroup`，其中可以包含 AXAML、code-behind 和 ViewModel 片段。LLMS 生成器必须使用同一组源文件和同一组示例边界，不维护第二套示例源码。

LLMS 不读取 `ShowCaseCodeSnippetCatalog.g.cs` 这类生成产物作为长期数据源。该文件是 Gallery 运行时源码查看的消费结果，格式会随 generator 实现调整。LLMS 生成器应复用源码查看抽取逻辑或等价的共享抽取服务，从 `*ShowCase.axaml`、对应 `.axaml.cs` 和 `ViewModels/*.cs` 构建示例模型。

文档级示例必须满足以下要求：

- 示例来自真实 `ShowCaseItem`，并可在 Gallery 中渲染。
- 示例必须有稳定 `SourceKey`；禁止只依赖 `itemIndex` 作为公开文档引用。
- 示例只输出 public 可见内容，不输出 internal/private 控件、API、路径或数据。
- 示例代码应优先展示控件公共 API，不把 Gallery shell、ShowCase 容器、测试辅助代码或临时演示数据输出为用户示例。
- 示例标题、描述和代码中的 Gallery 本地化资源必须按目标语言解析。中文第一阶段解析 `zh_CN` 资源；无法解析时保留原资源表达并发出诊断。
- 示例过长时可以按配置限制数量和长度，但不得截断为无法理解或无法复用的代码片段。

LLMS 示例模型至少包含：

| 字段 | 说明 |
| --- | --- |
| `controlId` | 控件文档 ID，例如 `button`。 |
| `viewTypeName` | Gallery ShowCase 视图类型。 |
| `panelKey` | 示例所在 `ShowCasePanel` 名称，默认示例区域通常为 `ExamplesContent`。 |
| `sourceKey` | 稳定示例 ID，例如 `button-type`。 |
| `title` | 目标语言下的示例标题。 |
| `description` | 目标语言下的示例说明，可为空。 |
| `kind` | 示例类型，例如 `basic`、`state`、`variant`、`integration`。 |
| `priority` | 输出排序权重。 |
| `snippets` | AXAML / C# 片段集合。 |
| `sourceFilePath` | 源文件相对路径和行号，用于追溯。 |

AtomUI Desktop 第一阶段可从现有 `ShowCaseItem` 自动抽取候选示例，并按 `SourceKey`、`Title`、源码路径和片段数量生成输出。后续控件补齐文档级示例时，应为稳定示例补充 `SourceKey` 和文档示例元数据，而不是修改 LLMS 生成产物。

### 7.6 组合结构模型

`Composition Model` 面向结构复杂但 `ControlTheme` 节点不一定丰富的控件，例如 Drawer、Badge、Breadcrumb、Modal、Message、Notification、Watermark、Menu、Steps、ListView、Select、Cascader、DatePicker、TimePicker、ComboBox、AutoComplete、TabControl、ProgressBar、Skeleton 等。

这类控件常见特征：

- public 控件自身模板较薄，主要作为 API 和状态 owner。
- 功能由内部容器、presenter、adorner、popup host、motion actor、item container 或数据对象完成。
- 关键结构通常由 `Themes/` 下多个 `ControlTheme` 共同定义，也可能由 C# 创建、item container 生成或 adorner 挂载补充，而不是集中在一个 `ControlTemplate` 中。
- Agent 只看 `Semantic Parts` 和 `Abstract AXAML Structure` 会误以为控件结构简单，从而无法正确理解状态流、生命周期和可定制边界。

`Composition Model` 输出必须包含两个层次：

1. 控件角色图：用稳定文本树展示 public 控件到内部协作节点和模板视觉节点的真实父子关系。
2. 协作节点表：列出节点类型、来源、生命周期 owner、影响的 public API、稳定性和 Agent 使用边界。

示例形态：

````md
## Composition Model

### 控件角色图

```text
Drawer
  -> DrawerContainer (internal container control theme, DrawerContainerTheme.axaml)
     -> Panel#RootLayout (template-stable)
        -> Border#PART_Mask (template-stable)
        -> MotionActor#PART_InfoContainerMotionActor (template-stable)
           -> DrawerInfoContainer#PART_InfoContainer (template-stable)
  -> DrawerInfoContainer (internal container control theme, DrawerInfoContainerTheme.axaml)
     -> Panel#RootLayout (template-stable)
        -> Grid#InfoLayout (template-stable)
           -> Grid#InfoHeader (template-stable)
              -> IconButton#PART_CloseButton (template-stable)
```

### 协作节点

| 节点 | 类型 | 来源 | 生命周期 owner | 影响的 public API | 稳定性 | Agent 使用边界 |
| --- | --- | --- | --- | --- | --- | --- |
| Drawer | public control | `Drawer.cs` | 用户持有 | `IsOpen`、`Placement`、`Content` | public | 用户只直接使用 public 控件。 |
| DrawerContainer | internal control | `DrawerContainerTheme.axaml` + `Drawer.cs` 创建 | Drawer | mask、motion、placement | internal-observable | 可用于理解行为，不应在用户代码中直接依赖。 |
````

`控件角色图` 不是扁平节点索引。生成器必须按真实 `ControlTemplate` 的 XML 父子结构输出模板层级树：`ControlTheme` target 作为角色入口，模板视觉节点使用 `ElementType#Name` 形式；没有 `Name` 的节点只输出 `ElementType`。多个 `ControlTemplate` 或 selector 变体可以形成多个根树，但每棵树内部必须保持真实包含关系。协作节点表可以继续按节点去重和补充 owner/API/稳定性信息，但不得替代层级树。

稳定性取值：

| 值 | 说明 |
| --- | --- |
| `public` | 用户可直接使用和依赖的 public 控件、数据对象或 API。 |
| `template-stable` | 稳定 template part、ControlTheme key 或主题节点，可用于主题维护但不能随意改名。 |
| `internal-observable` | 内部协作对象，影响用户可观察行为；Agent 可以用它理解结构，但不能指导用户直接依赖。 |
| `private` | 私有实现细节，默认不进入公开 LLMS 输出；如必须出现，只能用于说明不可依赖边界。 |

不同控件族的组合模型重点：

| 控件族 | 代表控件 | 组合模型重点 |
| --- | --- | --- |
| Shell / Overlay | Drawer、Modal、Notification、Message、PopupConfirm | public control、container、mask、surface、motion、close/action 区。 |
| Adorner / Decorator | Badge、Watermark | decorated target、adorner、indicator、motion actor、offset、placement。 |
| Items / Container | Breadcrumb、Steps、Menu、ListView、TabControl | owner control、item data、item container、separator、trigger、selection/navigation flow。 |
| Popup Input | Select、Cascader、DatePicker、TimePicker、ComboBox、AutoComplete | input shell、popup host、panel、item presenter、selection/value sync。 |
| Presenter / Family | ProgressBar、Skeleton、Card、Descriptions | public API 到内部 presenter、visual unit 或家族子控件的映射。 |

公开 LLMS 输出中的 `Composition Model` 必须区分“可直接使用”和“只用于理解”。例如 Agent 可以学习 Drawer 由 `DrawerContainer` 和 `DrawerInfoContainer` 协同完成，但不得建议用户直接创建或依赖 internal 类型。

## 8. 输出契约

每个项目的输出根目录由 `outputRoot` 决定。中文第一阶段输出结构为：

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

每个生成文件必须包含生成标记：

```md
> 生成产物：由源文档生成，不要手工编辑。修改内容请回到控件文档、源码 public surface、Token 类型或生成数据、Gallery ShowCase 或源码结构。
```

### 8.1 controls/<control>/index-cn.md

单控件完整中文文档结构：

```text
# <Control>
概述
包与命名空间
何时使用
公共 API
事件与命令
使用示例
状态模型
主题与 Design Token
AOT 与裁剪注意事项
源码索引
相关文档
```

### 8.2 controls/<control>/semantic-cn.md

单控件中文语义文档结构：

```text
# <Control> 语义结构
Semantic Parts
Abstract AXAML Structure
Composition Model
Template Parts
Pseudo Classes
State Flow
Theme and Token Boundaries
Customization Boundaries
```

### 8.3 聚合文件

`llms.txt` 是导航索引，链接聚合文档、单控件文档、语义文档、工程规范和源文档。

`llms-full-cn.txt` 按配置中的分类顺序聚合所有可见控件的 `index-cn.md` 内容。

`llms-semantic-cn.md` 按同样顺序聚合所有可见控件的 `semantic-cn.md` 内容。

聚合文件必须包含每个片段的来源路径：

```text
Source: ./controls/<control>/index-cn.md
Source: ./controls/<control>/semantic-cn.md
```

## 9. 内部模块设计

生成器按职责拆分为以下模块：

| 模块 | 职责 |
| --- | --- |
| `Config` | 读取和校验 `llms.config.json`。 |
| `Catalog` | 根据配置发现控件、分类、输出路径和可见性。 |
| `Markdown` | 解析 Markdown 标题、表格、代码块和稳定章节。 |
| `Extractors` | 从控件文档、Gallery 元数据和源码索引构建控件模型。 |
| `Model` | 承载项目、控件、API、Token、semantic parts、示例和输出路径。 |
| `ControlThemeStructureReader` | 从源码索引和源码根定位真实 ControlTheme 模板，抽取 `Abstract AXAML Structure`。 |
| `CompositionModelReader` | 优先从控件 `Themes/` 文件夹读取运行时组合结构，并用 `implementation.md`、源码索引和静态源码线索补充校验；不得仅凭控件分类发明协作节点。 |
| `GalleryExampleReader` | 复用 Gallery 源码查看示例边界，读取 `ShowCaseItem` 示例片段、标题、描述、本地化文本和源码位置。 |
| `Writers` | 写出单控件文档、语义文档和聚合文档；不得从 semantic parts 发明 AXAML 结构。 |
| `Verification` | 校验输入契约、输出契约、可见性、路径和生成结果一致性。 |
| `Diagnostics` | 统一诊断码、文件路径、控件 ID 和修复建议。 |

建议目录结构：

```text
tools/AtomUI.Docs.LLMsGenerator/
├── Config/
├── Catalog/
├── Markdown/
├── Extractors/
├── Model/
├── Writers/
├── Verification/
├── Diagnostics/
└── Program.cs
```

## 10. 生成流程

`generate` 流程：

```text
读取配置
校验配置路径和可见性策略
发现控件目录
读取 overview / implementation / token / changelog
读取源码 public surface、Token 类型或生成数据、Gallery ShowCase 元数据
从 Gallery 源码查看示例边界读取稳定示例片段
从源码索引定位 ControlTheme 模板并抽取 Abstract AXAML Structure
从控件 Themes 文件夹读取 Composition Model，并用 implementation.md 和源码索引补充说明
构建 ControlDocModel
运行输入契约校验
写出 controls/<control>/index-cn.md
写出 controls/<control>/semantic-cn.md
写出 llms.txt
写出 llms-full-cn.txt
写出 llms-semantic-cn.md
运行输出契约校验
```

`verify` 流程：

```text
读取配置
生成到内存或临时目录
校验工作区输出文件存在
比较工作区输出和重新生成内容
校验无旧路径、无未完成标记、无越权内容
校验聚合覆盖范围和单控件输出一致
```

`verify` 不修改工作区文件。

## 11. 诊断模型

诊断必须包含：

- 诊断码。
- 严重级别。
- 控件 ID。
- 文件路径。
- 失败原因。
- 修复入口。

诊断码示例：

| 诊断码 | 含义 |
| --- | --- |
| `LLMS001` | 配置文件无效。 |
| `LLMS010` | 控件目录缺少必需源文档。 |
| `LLMS020` | `overview.md` 缺少 LLMS 元数据。 |
| `LLMS021` | `overview.md` 缺少 semantic parts。 |
| `LLMS022` | `overview.md` 缺少 LLMS 导出来源表。 |
| `LLMS030` | 源码 public surface、Token 数据、Gallery 示例元数据与控件文档冲突。 |
| `LLMS031` | 文档级 Gallery 示例缺少稳定 `SourceKey`。 |
| `LLMS032` | Gallery 示例源码片段无法生成或无法解析目标语言资源。 |
| `LLMS033` | 结构复杂控件缺少 `Composition Model` 或组合节点稳定性边界。 |
| `LLMS040` | 输出路径不符合 `controls/<control>/index-<lang>.md` 规则。 |
| `LLMS041` | 生成文件缺少生成标记。 |
| `LLMS050` | 公开输出包含 internal/private 内容。 |
| `LLMS060` | 聚合文件覆盖范围与单控件输出不一致。 |
| `LLMS070` | 工作区输出与重新生成内容不一致。 |

## 12. 验证规则

生成器至少执行以下验证：

- 配置文件 schema version 受支持。
- `outputRoot`、`docsRoot`、`galleryRoot`、`sourceRoots` 在配置上下文中可解析。
- 每个可见控件有 `overview.md`、`implementation.md`、`changelog.md`。
- 没有 `token.md` 的控件在 `overview.md` 说明 Token 来源。
- `overview.md` 包含包名、命名空间、AXAML 命名空间、Gallery 页面、控件状态和 LLMS 可见性。
- `overview.md` 包含 LLMS semantic parts。
- `overview.md` 包含 LLMS 导出来源表。
- 文档级 Gallery 示例来自真实 `ShowCaseItem`，并优先使用稳定 `SourceKey`。
- 生成的“使用示例”包含真实代码片段，不只输出 Gallery 文件路径。
- 示例代码不包含 Gallery shell、ShowCase 容器或 internal/private 内容。
- 示例中的本地化资源按目标语言解析；无法解析时必须报告诊断或在输出中保留可追溯资源表达。
- 结构复杂控件的 `semantic-cn.md` 包含 `Composition Model`；组合节点必须声明稳定性和 Agent 使用边界。
- `Composition Model` 不把 internal/private 节点描述成用户可直接依赖的 public API。
- 单控件完整文档和语义文档都存在且必需章节非空。
- 聚合文件中的控件列表和单控件输出完全一致。
- 生成产物不包含旧目录结构路径。
- 公开输出不包含 internal/private 控件。
- 生成产物没有未完成标记。
- 生成产物没有超过配置允许范围的源码路径。

## 13. AOT 与运行时边界

LLMS 生成器是离线工具，不进入 AtomUI 运行时包，也不作为控件库依赖发布。

工具不从运行时程序集扫描类型，不要求加载控件程序集，不依赖 Avalonia 运行时初始化。API、Token 和 semantic 信息来自源文档、Gallery 源文件和显式配置。这样可以避免把 LLMS 生成和 NativeAOT、trimming、运行时反射绑定在一起。

如果需要从源码提取 public surface，应优先使用静态文本或 Roslyn 语法树分析项目源码，而不是加载编译后的程序集。

## 14. 商业控件项目接入

商业控件项目接入时只提供配置和符合规范的源文档：

```text
commercial/<project>/docs/AI/llms.config.json
commercial/<project>/docs/controls/<platform>/<category>/<control>/
```

商业项目可以选择独立输出根目录：

```text
commercial/<project>/docs/AI/llms/
```

也可以在主仓库统一输出，但必须使用不同 `projectId` 和明确的可见性策略。

接入要求：

- 控件目录结构与公共控件一致。
- 每个控件声明 LLMS 可见性。
- 内部控件和私有 API 不进入公开配置。
- Gallery 元数据可以通过项目自己的 `galleryRoot` 提供。
- 商业项目不修改生成器核心逻辑，只通过配置和必要的 extractor adapter 扩展。

## 15. 扩展点

生成器允许以下扩展点：

| 扩展点 | 用途 |
| --- | --- |
| `IProjectConfigReader` | 支持配置文件版本演进。 |
| `IControlCatalogProvider` | 支持不同控件项目的目录发现方式。 |
| `ISourceDocReader` | 支持控件文档结构的兼容读取。 |
| `IGalleryMetadataProvider` | 支持不同 Gallery 目录和示例组织方式。 |
| `ISourceIndexProvider` | 支持不同源码根目录和源码索引策略。 |
| `ILLMsWriter` | 支持新增语言或输出形态。 |
| `IVerificationRule` | 支持项目专属校验规则。 |

扩展点必须通过配置或显式注册启用。默认生成路径只启用 AtomUI 标准控件文档和 Gallery 约定。

## 16. 测试策略

测试项目位于：

```text
tests/AtomUI.Docs.LLMsGenerator.Tests/
```

测试覆盖：

- 配置读取和 schema 校验。
- 控件发现和分类排序。
- Markdown 章节、表格、代码块解析。
- Button 参考输出。
- Gallery ShowCase 示例片段抽取和本地化文本解析。
- Drawer、Badge、Breadcrumb 等组合结构参考输出。
- 可见性过滤。
- 缺少 semantic parts 的失败诊断。
- 缺少 Token 来源说明的失败诊断。
- 公开输出包含 internal/private 内容的失败诊断。
- 聚合文件覆盖范围不一致的失败诊断。
- 工作区输出和重新生成内容不一致的失败诊断。

基础验证命令：

```bash
dotnet test tests/AtomUI.Docs.LLMsGenerator.Tests/AtomUI.Docs.LLMsGenerator.Tests.csproj --framework net10.0 --no-restore
dotnet run --project tools/AtomUI.Docs.LLMsGenerator/AtomUI.Docs.LLMsGenerator.csproj -- verify --config docs/AI/llms.config.json
git diff --check
```

## 17. 维护不变量

- 源文档是唯一人工维护源。
- 生成产物不手工编辑。
- 输出路径使用 `controls/<control>/index-<lang>.md` 和 `controls/<control>/semantic-<lang>.md`。
- 控件名目录不包含语言后缀。
- 公开输出不包含 internal/private 控件内容。
- 生成器不访问网络。
- 生成器不加载控件运行时程序集。
- 控件项目通过配置接入，不通过硬编码路径接入。
- Gallery 元数据和控件文档冲突时生成失败。
- 结构复杂控件必须通过 `Composition Model` 说明 public 控件与内部协作节点的关系。
- 生成顺序稳定，由配置的分类顺序和控件目录名决定。
