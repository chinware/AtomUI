# Semantic Part Generator 实现

Semantic Part Generator 为 AtomUI Control 的公开视觉区域生成静态 descriptor、名称常量、包级注册和构建期诊断。
公共语义、Selector、Popup、Theme 和兼容性契约由
[AtomUI Semantic Part 系统设计](../../architecture/systems/theming/semantic-parts.md) 定义；本文档只定义生成器职责。

## 1. 设计定位

生成器把 Control 源码中的显式 Semantic Part 声明与构建系统提供的 `Themes/**/*.axaml` 组合为确定性输出。它不从
节点名称、控件分类或目录名称推测公共 Part，也不在运行时扫描 AXAML 或程序集。

生成器负责：

- 读取 public Control 上的 Semantic Part 声明。
- 验证 Part 名称、selector class、ContractType、cardinality 和 customization。
- 分析内置 AXAML 模板中的 `.semantic-*` marker。
- 关联已有 Semantic Part Theme 资产元数据。
- 生成 `ControlSemanticDescriptor`、Part 常量和包级注册。
- 向文档和 Gallery 工具提供稳定静态输入。

生成器保持 `SelectorClass` 与 `ContractType` 正交：前者生成 `.semantic-*` Part 身份，后者验证 marker 节点类型并供文档
工具读取为 `x:SetterTargetType`。生成器不得把二者拼接成类型限定 Semantic selector。

生成器不负责：

- 发明未声明的 Part。
- 修改 AXAML 或自动为模板节点添加 class。
- 运行时查找 VisualTree 节点。
- 验证应用或第三方包未作为当前 compilation 输入的自定义 ControlTheme。
- 把 Semantic Part 转换为 Token identity。

### 1.1 源码归属

| 文件 | 职责 |
| --- | --- |
| `SemanticParts/SemanticPartDeclaration.cs` | 从 Roslyn `AttributeData` 构造声明模型，按 Control symbol 合并 partial 声明并按 path 排序。 |
| `SemanticParts/SemanticPartModelBuilder.cs` | 校验声明、强类型 Theme 契约和每个 ControlTemplate 的 marker。 |
| `SemanticParts/SemanticPartManifestWriter.cs` | 生成 package manifest 与 per-Control 名称/class 常量。 |
| `ThemeAssets/ThemeAssetInfo.cs` | 使用 `XDocument` 提取 ControlTheme、ControlTemplate、XML namespace 和 `Classes` marker。 |
| `Registration/ControlPackageRegistrationWriter.cs` | 把生成的 Semantic descriptor 传入包级注册。 |
| `TokenResourceKeyGenerator.cs` | 组合 Attribute、Compilation、Theme AdditionalFiles 和 catalog 输入。 |

## 2. 输入模型

### 2.1 Control 声明

Control 使用可重复 `SemanticPartAttribute` 声明除 `root` 外的公开 Part。生成器通过固定 metadata name 识别
Attribute，不依赖 Attribute 实例反射。同一 non-generic public Control 的多个 partial 声明按 CLR symbol 合并，只生成
一个 descriptor 和一份常量文件；泛型 Control 在声明校验阶段拒绝。

声明字段包括：

```text
Name
Path
SelectorClass
ContractType
Cardinality
Customization
ThemePropertyName optional; SelectorAndTheme 时指向 public get/set ControlTheme 属性
CrossVisualRoot
Since
RuntimeCreated
```

`root` 由生成器为每个具有 Semantic Part 声明的 Control 隐式生成。

### 2.2 AXAML 资产

构建集成继续把 `Themes/**/*.axaml` 作为 `AdditionalFiles` 提供给 Generator。生成器使用结构化 AXAML 分析读取：

- ControlTheme `TargetType`。
- typed `BasedOn="{StaticResource {x:Type ...}}"` 继承关系。
- ControlTemplate variant。
- `Classes` 中的 `.semantic-*` marker。
- marker 所在节点的公开类型 identity。
- Browser 或其他平台主题资产。
- 叶子 Theme 资产与 owner Control 的既有映射。

生成器不得使用正则表达式替代 AXAML 结构分析，也不得依赖只用于聚合的 `*Themes.axaml` 推断模板完整性。

### 2.3 Semantic Part Theme 资产

现有 Theme asset generator 产生的 `ControlThemeSemanticPartDescriptor` 继续表达：

```text
Theme property name
Theme TargetType
Theme asset owner
Referenced Control identity
```

Semantic Part Generator 将该信息关联到 `Customization=SelectorAndTheme` 的 Part。存在同名 Semantic Theme 资产时，
解析其真实 `TargetType`，要求它可赋值给 `ContractType`，并要求平台变体使用同一个目标类型；descriptor 保存该真实类型。
没有对应资产时，Theme target metadata 回退到 `ContractType`。Selector-only Part 不创建
`ControlThemeSemanticPartDescriptor`。

### 2.4 Runtime-created Part

`RuntimeCreated=true` 表示 marker 由 C# 创建路径添加。生成器产生稳定 class 常量供 Control 使用，但不通过源码文本
搜索证明调用已经发生。该契约由控件行为测试验证。

Runtime-created Part 仍需在 descriptor 中声明 `ContractType`、cardinality、owner 和跨视觉根信息。

## 3. 输出模型

### 3.1 ControlSemanticDescriptor

每个 Control 生成一个不可变静态 descriptor，内容包括：

```text
Control CLR identity
Control catalog identity
SemanticPartDescriptor[]
```

每个 `SemanticPartDescriptor` 包含：

```text
Name
Path
SelectorClass
ContractType CLR identity
Cardinality
Customization
Theme property metadata optional
CrossVisualRoot
Since
RuntimeCreated
```

Descriptor 不保存 `Type` 的动态发现逻辑、Style、Setter、ControlTheme 实例或 VisualTree 引用。

### 3.2 Part 常量

生成器为 Control 产生 `<Control>SemanticParts.g.cs`，供 C# runtime-created 节点和测试使用：

```text
Part name constants
Selector class constants
```

AXAML 仍直接使用 `.semantic-*` class。生成常量不是第二套命名来源，其值必须与 descriptor 完全一致。

消费 descriptor 的文档或示例工具在输出包含 Setter 的示例时，必须使用 `SelectorClass` 生成 class-only selector，
并把 `ContractType` 输出为 `x:SetterTargetType`；不得输出 `ContractType.semantic-*` 或
`:is(ContractType).semantic-*`。

### 3.3 包级注册

包级输出包括：

```text
GeneratedSemanticPartManifest.g.cs
<Control>SemanticParts.g.cs
GeneratedControlPackageRegistration.g.cs
```

Control 包现有生成式 registration helper 同时注册 `ControlSemanticDescriptor`，并由真实 `UseXxxControls()` 入口调用。
当引用的 Core 版本尚未提供 `AtomUI.Theme.Schema.ControlSemanticDescriptor` 时，生成器保持四参数 package registration，
不输出 Semantic manifest，从而维持旧 Core 编译兼容。采用五参数注册时，Semantic descriptor 与 Control descriptor 使用同一个
`includeIdentity`谓词筛选。运行时 registry冻结后不扫描程序集或 AXAML。

Semantic descriptor 注册失败必须和 Control identity、Token descriptor、Theme asset manifest 冲突一样在启动构建边界
明确失败，不能静默覆盖。

## 4. 模板分析规则

### 4.1 Marker 完整性

对于 `RuntimeCreated=false` 的 Part，生成器验证每个适用 ControlTemplate variant：

- `Single` 必须存在且只能映射一个有效职责节点。
- `Optional` 可以不存在，但存在时必须类型兼容。
- `Multiple` 必须映射一个或多个节点，所有节点必须表示同一公开职责。
- marker 的节点类型必须可赋值给 `ContractType`。
- 同一节点只能携带一个已声明 Semantic Part marker。

### 4.2 Variant 覆盖

分析范围包括：

- 同一 ControlTheme 中由 selector 分支设置的不同 ControlTemplate。
- 同一 Control 家族的多个叶子 Theme 资产。
- Desktop 与 Browser 主题。
- 派生 Control 自有模板，或通过 typed `BasedOn` 明确复用的基类模板。

typed `BasedOn` 按 Avalonia 12 的应用顺序展开：派生 Theme 只有 selector 条件模板时，基类默认模板仍属于适用
variant；派生 Theme 通过直接 `Setter Property="Template"` 替换默认模板时，不再校验被覆盖的基类模板。循环引用使用
访问集合终止，并最终报告没有适用模板。

生成器必须区分“该 variant 不适用”和“适用但漏标”。无法静态确定适用关系时，要求声明方提供明确资产归属，不能
默认为通过。

### 4.3 结构化解析边界

当前静态校验只读取模板节点的 `Classes` marker，不分析或重写 `Style.Selector` 文本。Selector 的 owner scope、
`/template/` 边界和状态组合由架构规范、控件主题 review 与运行时 selector 测试保证。

`BasedOn` 只解析显式 `{StaticResource {x:Type ...}}`。字符串 key、运行时资源选择或自定义 markup extension 无法静态解析，
声明方必须提供可分析的叶子模板或 typed `BasedOn`。

类型解析支持：

- `using:` XML namespace；
- `clr-namespace:` XML namespace；
- compilation 与引用程序集上的 `XmlnsDefinitionAttribute`；
- 当前 compilation 内唯一同名 public Control fallback。

只用于聚合资源的 `*Themes.axaml` 不产生可分析模板；验证以包含真实 `ControlTemplate` 的叶子 Theme 资产为准。

### 4.4 Popup 与 Overlay

`CrossVisualRoot=true` 不触发 Theme 属性生成。生成器只记录该事实，并要求测试清单覆盖 PopupRoot 与
OverlayPopupHost。

模板内 Popup 的 marker 按正常模板节点分析。Popup 内容由运行时创建时使用 `RuntimeCreated=true`，并由测试证明：

- `TemplatedParent` 或 StyleHost 链连接到正确 owner。
- owner-scoped Selector 可以命中。
- Popup 关闭和重新打开后 marker 保持一致。

### 4.5 ItemContainer

虚拟化或回收容器通常使用 `RuntimeCreated=true` 和 `Multiple`。生成器验证 descriptor，控件测试验证 prepare、clear、
recycle 和 owner 切换后的实际 marker。

## 5. 诊断

当前诊断为：

| ID | Severity | 触发条件 |
| --- | --- | --- |
| `ATOMUIGEN020` | Error | Control/Part 声明形态、名称、path、class、cardinality 或 customization 非法。 |
| `ATOMUIGEN021` | Error | 同一 Control 中 Part name、path 或 selector class 重复。 |
| `ATOMUIGEN022` | Error | `ContractType` 不是 public `StyledElement`。 |
| `ATOMUIGEN023` | Error | `SelectorAndTheme` 的 public get/set `ControlTheme` 属性契约非法。 |
| `ATOMUIGEN024` | Warning | Part 未声明 `Since`。 |
| `ATOMUIGEN025` | Error | 某个模板的 marker 数量不满足 `Single`、`Optional` 或 `Multiple`。 |
| `ATOMUIGEN026` | Error | marker 节点类型不能赋值给 `ContractType`。 |
| `ATOMUIGEN027` | Error | Control 声明了静态 Part，但没有适用的可分析 ControlTemplate。 |
| `ATOMUIGEN028` | Error | 同一模板节点同时声明了多个 Semantic Part marker。 |

同一 Control identity 的包级冲突由 Core `ControlPackageRegistration` 和 `ThemeManagerBuilder` 在启动注册边界拒绝。
控件文档与 descriptor 的一致性由文档 review 和文档验证流程负责，当前 Generator 不解析 Markdown。

Diagnostic ID、默认严重级别、消息格式和帮助链接统一遵循
[编译器诊断规范](../../engineering/development/compiler-diagnostics-guidelines.md)。Runtime-created Part 和
`CrossVisualRoot=true` 的行为覆盖属于测试验证要求，不伪装成 Generator 可以静态证明的诊断。

## 6. 增量生成

生成器输入必须按职责拆分：

```text
Compilation public Control declarations
+ SemanticPartAttribute symbols
+ Theme AdditionalFiles
+ AnalyzerConfig control catalog
+ Existing Theme asset semantic metadata
```

Semantic 声明和 Theme AdditionalFiles 在增量管线中分别收集，再按 compilation 生成确定性 package 输出。实现不承诺
单个 Control 修改时只执行该 Control 的 writer；正确性契约是输入相同则生成文本和诊断顺序相同。

排序规则必须稳定：

1. Control catalog identity。
2. Control identity。
3. Part path。
4. Theme asset `AssetPath` 与资产内模板顺序。

生成代码、diagnostic 顺序和 manifest 顺序不能依赖文件系统枚举顺序。

## 7. AOT 与裁剪

生成器和生成物必须满足：

- 不使用 `Assembly.GetTypes()`。
- 不使用 `Activator.CreateInstance()` 创建 descriptor 或 Theme。
- 不通过 `PropertyInfo` 查找 Theme property。
- 不在运行时解析 selector class 或 Part path。
- ContractType identity 由编译期 symbol 产生。
- 包级入口直接注册静态 descriptor。
- 第三方 Control 包使用同一 Generator，不提供反射 fallback。

Generator 项目仍以 Analyzer 方式引用，不参与应用 NativeAOT publish。

## 8. 文档与 Gallery 集成

Control 文档以生成 descriptor 和真实 ControlTemplate 为事实源维护 Semantic Parts 表，不得根据 descriptor 发明
Abstract AXAML Structure。当前 Semantic Part Generator 不读取 Markdown 或生成 LLMS 文档。

Gallery Semantic Preview 使用 descriptor 展示 Part 名称、selector、ContractType 和 cardinality。预览工具可以使用
public VisualTree API 查找已实例化 `.semantic-*` marker；该查找只属于开发工具，不进入 Control 运行时。

## 9. 验证要求

生成器测试至少覆盖：

1. 合法声明和确定性输出。
2. 重复名称、非法 class 和 ContractType 错误。
3. Single、Optional、Multiple marker 数量。
4. 多 ControlTemplate 和 Desktop/Browser variant。
5. Selector-only 与 SelectorAndTheme 的差异。
6. 现有 `ControlThemeSemanticPartDescriptor` 关联。
7. Popup、runtime-created Part 和 item container 元数据。
8. 旧 Core 引用下保持四参数 package registration 的兼容路径。
9. 包级注册、生成顺序与 NativeAOT 友好输出。
