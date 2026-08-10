# Semantic Part Generator 设计

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

生成器不负责：

- 发明未声明的 Part。
- 修改 AXAML 或自动为模板节点添加 class。
- 运行时查找 VisualTree 节点。
- 验证应用或第三方包未作为当前 compilation 输入的自定义 ControlTheme。
- 把 Semantic Part 转换为 Token identity。

## 2. 输入模型

### 2.1 Control 声明

Control 使用可重复 `SemanticPartAttribute` 声明除 `root` 外的公开 Part。生成器通过固定 metadata name 识别
Attribute，不依赖 Attribute 实例反射。

声明字段包括：

```text
Name
Path
SelectorClass
ContractType
Cardinality
Customization
ThemePropertyName optional
CrossVisualRoot
Since
RuntimeCreated
```

`root` 由生成器为每个具有 Semantic Part 声明的 Control 隐式生成。

### 2.2 AXAML 资产

构建集成继续把 `Themes/**/*.axaml` 作为 `AdditionalFiles` 提供给 Generator。生成器使用结构化 AXAML 分析读取：

- ControlTheme `TargetType`。
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

Semantic Part Generator 将该信息关联到 `Customization=SelectorAndTheme` 的 Part。Selector-only Part 不创建
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

生成器为 Control 产生稳定常量，供 C# runtime-created 节点和测试使用：

```text
Part name constants
Selector class constants
```

AXAML 仍直接使用 `.semantic-*` class。生成常量不是第二套命名来源，其值必须与 descriptor 完全一致。

### 3.3 包级注册

Control 包现有生成式注册入口同时注册 `ControlSemanticDescriptor`。运行时 registry 冻结后不扫描程序集或 AXAML。

Semantic descriptor 注册失败必须和 Control identity、Token descriptor、Theme asset manifest 冲突一样在启动构建边界
明确失败，不能静默覆盖。

## 4. 模板分析规则

### 4.1 Marker 完整性

对于 `RuntimeCreated=false` 的 Part，生成器验证每个适用 ControlTemplate variant：

- `Single` 必须存在且只能映射一个有效职责节点。
- `Optional` 可以不存在，但存在时必须类型兼容。
- `Multiple` 可以映射多个节点，所有节点必须表示同一公开职责。
- marker 的节点类型必须可赋值给 `ContractType`。
- 同一节点不能无声明地承担两个不兼容 Part。

### 4.2 Variant 覆盖

分析范围包括：

- 同一 ControlTheme 中由 selector 分支设置的不同 ControlTemplate。
- 同一 Control 家族的多个叶子 Theme 资产。
- Desktop 与 Browser 主题。
- 派生 Control 明确复用或替换的模板。

生成器必须区分“该 variant 不适用”和“适用但漏标”。无法静态确定适用关系时，要求声明方提供明确资产归属，不能
默认为通过。

### 4.3 Selector 边界

生成器验证内置主题中的公共 Semantic Selector：

- 使用 `.semantic-*` 保留前缀。
- owner selector 最多进入一个 `/template/` 边界。
- 不通过连续 `/template/` 公开子 Control 的内部模板。
- 推荐 selector 的类型与 Part `ContractType` 一致。
- 不把 `PART_*` 或 Name 当作公开 semantic selector 的唯一条件。

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

以下情况必须产生构建错误：

- Part 名称为空、格式非法或在同一 Control 中重复。
- selector class 不以 `semantic-` 开头或不是合法 kebab-case。
- 同一 Control 的不同 Part 使用同一个 selector class。
- `ContractType` 不是公开可引用的 `StyledElement` 类型。
- marker 节点类型与 `ContractType` 不兼容。
- `Single` Part 在一个模板 variant 中缺失或重复。
- 必需 Part 在某个平台主题 variant 中缺失。
- `SelectorAndTheme` 没有对应强类型 `ControlTheme?` 属性。
- Theme property owner、Theme TargetType 与 Part ContractType 不兼容。
- `RuntimeCreated=false` 但所有资产都找不到 marker。
- 同一 Control identity 注册了不一致的 descriptor。

以下情况产生警告或文档验证失败：

- 公开 Part 没有 `Since`。
- descriptor 与控件 `overview.md` 的 Semantic Parts 表不一致。

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

输出按 Control identity 分组。修改一个 Control 的声明或主题资产只失效该 Control 的 descriptor 和所属包级注册，
不能重新生成所有无关 Control。

排序规则必须稳定：

1. Control catalog identity。
2. Control identity。
3. Part path。
4. Theme asset URI。

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

LLMS 和 Control 文档生成器读取静态 descriptor 校验人工维护的 Semantic Parts 表，但不得根据 descriptor 发明
Abstract AXAML Structure。真实结构仍来自明确定位的 ControlTheme 和 ControlTemplate。

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
8. 文档表格不一致诊断。
9. 增量更新只失效受影响 Control。
10. 包级注册、裁剪和 NativeAOT publish。
