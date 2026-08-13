# AOT 与裁剪架构

> 状态：已实现。non-trimmed、trimmed JIT 和 NativeAOT 已完成行为与运行验证；WebAssembly AOT 已完成
> 生成、编译、Emscripten 链接和优化验证。本文定义 AtomUI 在 linked publish 下的注册、生成、兼容和验证契约。

本文是 AtomUI AOT 与 trimming 架构的唯一正式所有者。它覆盖 `AtomUI.Core`、`AtomUI.Controls`、
`AtomUI.Desktop.Controls`、可选桌面包、`AtomUI.Generator`、`AtomUI.Build.Tasks`、应用项目和第三方 Control 包。
日常编码规则见 [AOT 编程规范](../../engineering/development/aot-programming-guidelines.md)，具体平台发布命令由对应平台文档维护。

## 1. 问题定义

AtomUI 的 Theme 和 Localization Registry 必须在首帧前完整构建并冻结。当前包级生成代码会构造完整 Control descriptor
数组和完整 Theme Asset descriptor 数组，资源加载器还通过覆盖全包 AXAML factory 的大型分发表引用所有主题资源。
即使运行时随后按 identity 过滤，linker 在静态分析阶段已经把全部 Control、Own Token 和 AXAML factory 判定为可达。

问题根因是注册入口的静态可达性粒度。通过运行时筛选、反射扫描、`DynamicDependency`、linker XML 或控件实例化后的
late registration，都不能同时满足可裁剪、NativeAOT 和 Registry 冻结契约。

Avalonia Fluent Theme 采用包级静态主题注册来保证正确性。AtomUI 在保持同等正确性的基础上，以稳定的控件族
Registration Unit 作为更细的 linker 可达单元，不追求每个内部 CLR 类型独立裁剪。

## 2. 设计目标

本架构必须满足：

1. 普通非裁剪构建继续使用现有全量注册语义，不要求应用迁移。
2. `PublishTrimmed=true`、`PublishAot=true` 和 `RunAOTCompilation=true` 自动使用生成式 Unit 注册。
3. 应用继续调用 `UseDesktopControls()`，普通静态 AXAML/C# 使用不增加配置步骤。
4. 运行时不扫描程序集、类型、AXAML、Attribute 或资源目录。
5. Registry 仍在首帧前一次性构建并冻结，不引入 late registration。
6. 完全未使用的控件族可以连同 descriptor、Own Token、内部控件和 AXAML factory 一起删除。
7. 无法证明精确 Unit 集合时，只能扩大到对应 Package 的全量注册，不能静默生成不完整主题。
8. 语言、全局主题基础设施和包级初始化行为保持当前语义与顺序。
9. 第三方 Control 包可以使用同一版本化 Unit 协议，不依赖 AtomUI 内部程序集扫描。
10. 发布验证同时覆盖运行行为、fallback 和可重复的体积差值。

## 3. 复杂度边界

本架构明确不引入：

- Feature Manifest、Feature Root 或 Feature Phase。
- Catalog Fragment 或按语言闭包。
- 每个 Theme Asset 的跨程序集 Manifest。
- Semantic Part、Initializer、Catalog 等应用级图节点。
- 应用级 BFS、循环闭包或通用依赖图规划器。
- 运行时反射发现、AXAML 扫描或动态代码生成。
- 将无法解析 owner 的资源自动归类为共享资源。

Registration Unit 是唯一细粒度裁剪单位。语言、初始化器和共享基础资源按 Package 保留，以少量固定体积换取稳定行为和
低维护成本。

### 3.1 AOT/Trim 基础设施命名边界

AOT/Trim 注册路径属于发布基础设施，不是通用运行时功能。`AtomUI.Core` 中跨程序集调用的隐藏运行时 ABI 必须使用
明确的 `AotTrim` 前缀，并且不在类型名中重复 `AtomUI`；类型已经位于 `AtomUI.Registration` 命名空间内。当前命名契约为：

| 类型 | 职责 |
| --- | --- |
| `AotTrimRegistration` | 读取发布期注册模式标记，暴露 `IsEnabled`；不执行注册、不执行裁剪 |
| `AotTrimControlPackageRegistrationBuilder` | 收集已选 Registration Unit 的 descriptor 和 Theme asset，并提交一个完整 Package registration |
| `AotTrimRegistrationPlan` | 应用级 AOT/Trim Package plan 的跨程序集调用协议 |
| `AotTrimRegistrationPlanRegistry` | 由模块初始化器安装唯一 plan，并按 Package 分派调用 |

对应的 AppContext key 必须保留 `AtomUI` 前缀，以避免全局字符串冲突：

```text
AtomUI.AotTrimRegistration.Enabled
```

普通运行时功能不得依赖这些类型或开关。`GeneratedApplicationRegistrationPlan`、`Generated*UnitFragment` 等名称只用于
Source Generator 产生的内部代码；它们描述生成物，不构成用户运行时 API。Generator 内部的 `LinkedRegistration*`
类型位于明确的 `AtomUI.Generator.LinkedRegistration` 命名空间，可以保留协议实现语义；Build Task 则使用动作加对象的
名称，例如 `CollectAxamlUsageTask` 和 `ValidateAssemblyMetadataMarkerTask`。

包级生成代码使用 `AtomUI.Generated.<AssemblyOwner>`，其中 `AssemblyOwner` 是程序集名折叠得到的单一 PascalCase
标识符，分隔符不保留。例如 `AtomUI.Desktop.Controls` 对应 `AtomUI.Generated.AtomUIDesktopControls`。Theme Schema、
Theme Asset、Localization、Linked Registration 和 AXAML wrapper 必须共享这一命名规则；该名称会进入编译引用和
Assembly Metadata，属于需要稳定验证的内部生成 ABI。

## 4. 构建模式契约

AtomUI 以是否进入 linker 处理作为注册模式边界：

| 构建模式 | `UseDesktopControls()` 注册模式 |
| --- | --- |
| 普通 `dotnet build` / `dotnet run` | 全量兼容注册 |
| 非裁剪 Release、SelfContained、ReadyToRun | 全量兼容注册 |
| `PublishTrimmed=true`，任意 `TrimMode` | 生成式 Unit 注册 |
| `PublishAot=true` | 生成式 Unit 注册 |
| `RunAOTCompilation=true`，WebAssembly | 生成式 Unit 注册 |
| `AtomUIUseGeneratedRegistration=true` | 生成式 Unit 注册，用于非裁剪环境验证 |
| 显式 `UseAllDesktopControls()` | 全量注册 |

Build Targets 把上述条件规范化为内部 `AtomUILinkedPublish`，并把
`AtomUI.AotTrimRegistration.Enabled` 设置为 linker 可替换的 AOT/Trim feature switch。Analyzer 和库声明不单独改变注册模式。

普通非裁剪路径必须保持 Common、Desktop 和可选包的 descriptor、asset、Catalog、Bundle、Provider 和 initializer
集合及顺序。允许的非行为差异仅限一次缓存的 feature-switch 读取和少量编译器 metadata。

## 5. Registration Unit

Registration Unit 表示一个可以独立保留或删除的稳定控件族。例如：

```text
DatePicker Unit
├── DatePicker / RangeDatePicker
├── Presenter、Cell 和内部辅助控件
├── Control descriptor
├── Own Token schema
├── 控件族专属 AXAML factory
└── 同 Package 内直接依赖的 Unit
```

使用 Unit 中任一公开 Control 时，保留整个 Unit。内部 Presenter、Cell、Semantic Part 或命名基础 Theme 不继续拆分。
这保证 Unit 自身是完整可运行的主题注册单元，不要求应用 Generator 理解控件内部实现。

每个 Unit 生成一个跨程序集可调用的强类型入口：

```csharp
public static void Add(
    AotTrimControlPackageRegistrationBuilder builder)
```

入口位于版本化生成命名空间并标记 `EditorBrowsableState.Never`。Unit 方法先调用
`builder.TryEnterUnit(unitId)`；同 Package 依赖通过 Unit 方法之间的直接调用表达，重复依赖和循环由 builder 去重。
应用计划不展开或重新计算这些依赖。

Unit fragment 禁止引用全包 descriptor 数组、全包 Theme Asset 数组或包含全部 AXAML factory 的分发表。

## 6. Unit 归属契约

官方包默认使用源码和主题目录约定推导 Unit。一个控件目录中的公开 Control、内部部件和 `Themes/` 资源默认属于同一 Unit。
目录结构无法表达的例外使用编译期 MSBuild metadata 显式覆盖；metadata 只包含稳定字符串，不使用 `typeof(Control)`。

ControlMap 表示 CLR Control 的定义程序集 ownership，不表示该 Control 一定拥有 Theme descriptor。定义程序集中的每个
public、非泛型 Control 都必须归入一个 Unit 并输出 ControlMap；只有原本参与完整 Theme Schema 的 Control 才生成
descriptor 并调用 `builder.AddControl(...)`。因此基础布局控件等 descriptorless Control 仍能把静态使用映射到正确 Unit，
但普通非裁剪 descriptor 集合不会被扩大。引用程序集中的 Control 可以继续贡献既有 descriptor 信息，但不能抢占
ControlMap ownership。没有 Package registration entry 的基础设施程序集不输出 linked Package、Unit 或 ControlMap metadata。

主题资源只能属于以下三类之一：

| 分类 | 行为 |
| --- | --- |
| `ControlOwned` | 跟随对应 Registration Unit 保留或删除 |
| `PackageShared` | 包作者显式声明，Package 被注册时始终保留 |
| `Unknown` | 禁止猜测；使用该 Package 的生成式构建回退到全量注册 |

`PackageShared` 只允许包含确实跨多个 Unit 使用、没有合理单一 owner 的资源，例如全局 Brush/Typography、Popup/Overlay
基础设施、Window decorations 或通用 ScrollBar 基础主题。控件内部 Presenter、Item、Cell 和只服务一个控件族的命名
Theme 必须归入 `ControlOwned`。

显式共享资源使用构建项：

```xml
<ItemGroup>
  <AtomUIPackageSharedTheme Include="Popup/Themes/OverlayPopupHostTheme.axaml" />
</ItemGroup>
```

Generator 不能把 owner 解析失败当成共享声明。重复 Unit、跨目录冲突、共享资源引用私有 Unit 资源等不一致必须产生包作者诊断。

无法由 public owner 自动解析、但只服务单个 Unit 的内部 resource-only Theme 必须使用现有
`AtomUIRegistrationUnit` metadata 显式归属。抽象或基础 typed theme 如果没有独立 generated resource wrapper，不得生成
不存在的 wrapper 调用；它依靠同 Unit 中可加载的具体 ResourceDictionary 或 typed theme 被静态保留。

## 7. Package Core

以下内容按 Package 整体注册，不参与 Unit 裁剪：

- Language Catalog 和全部内置 Translation Bundle。
- Dialog input capture、Tooltip service、Motion animator、Responsive bootstrapper 等初始化逻辑。
- Global Token、Theme Algorithm 和 Package Provider。
- 平台 Asset Selector。
- 显式 `PackageShared` 主题资源。

Package Core 保持当前入口中的执行位置。以 Desktop 为例，生成式路径顺序必须是：

1. 按现有契约注册依赖 Package；跨 Package 依赖不做 Unit 级闭包。
2. 执行当前 package 提交前的初始化行为。
3. 创建当前平台 Provider 并应用 Unit Plan 或 full fallback。
4. 注册完整 Language Module。
5. 按当前顺序添加 Theme initializer。

Desktop 对 Common 的依赖使用完整 Common 注册。当前架构不包含跨 Package Unit 闭包；任何改变都必须基于可复现的
体积数据重新评估协议复杂度和维护成本，并作为独立架构变更评审。

## 8. 最小 Manifest 协议

跨程序集协议只保留四类记录：

| 记录 | 职责 |
| --- | --- |
| Package | Package ID、entry method、full registrar、PackageShared fragment 和协议版本 |
| Unit | Unit ID 与 fragment type/method |
| ControlMap | CLR metadata name 到 Package/Unit 的映射 |
| Usage | 当前程序集静态使用的 Package/Unit/entry 和来源位置 |

Manifest 使用版本化 `AssemblyMetadata` 编码稳定字符串，不把 `typeof(Control)` 写入 metadata。序列化、转义和解析只能在
唯一 codec 中实现；未知 major version 必须停止生成式 publish，不能猜测兼容。

普通类库也生成 Usage metadata，因为最终入口应用需要聚合引用类库中的静态控件使用。普通构建不安装运行计划，也不改变
注册行为。无法精确归属的 Usage 和 Theme Asset 在普通构建中仍写入 PackageRoot metadata，但 `ATOMUILINK002` 和
`ATOMUILINK007` 只在 linked publish 或 `AtomUIRegistrationStrict=true` 时报告，避免日常非裁剪构建被发布期 fallback
诊断污染。

只有定义 Control 的 Package 可以输出它的 ControlMap。ControlMap 指向的 Unit 必须存在且属于同一 Package；重复 ownership、
缺失 Unit 或跨 Package 关系均按无效 Package 定义处理。没有任何 registration entry 的程序集不生成空的 linked Package
metadata，避免把纯基础设施程序集误识别为可注册控件包。

## 9. 静态使用发现

### 9.1 AXAML

MSBuild task 在 `CoreCompile` 前使用 XML parser 结构化读取 `@(AvaloniaXaml)`，输出带文件、行和列的类型候选。不得使用
正则扫描文档。至少覆盖元素类型、自定义 Control 基类、`ControlTheme.TargetType`、`BasedOn`、Style selector、
`DataTemplate`、`ControlTemplate` 和 `x:Type`。

Generator 结合 Roslyn Compilation 和 ControlMap 把类型映射为 Unit。无法解析的 AtomUI 类型、Loose AXAML 或动态资源
来源必须把对应 Package 标记为 full fallback。

Package 自身的 Theme Asset 还必须结构化读取模板中的元素类型。能通过 `using:`、`clr-namespace:` 或当前程序集
`XmlnsDefinition` 精确解析到同 Package public Control 的元素，生成 owner Unit 到目标 Unit 的直接依赖；默认 Avalonia
元素、外部 Package 元素和无法证明 ownership 的同名类型不得按短名称猜测。该依赖只影响 Unit 静态可达性，不能加入
Theme Asset descriptor 的 referenced identities，也不能改变普通非裁剪 descriptor、asset 或 fingerprint。

### 9.2 C#

Generator 保守收集构造、字段、属性、参数、返回值、继承、闭合泛型、`typeof(T)` 和已知动态创建 API 中的 AtomUI
Control 类型。自定义 Control 自动保留其 AtomUI 基类所属 Unit。

其他 Source Generator 产生而当前 Generator 无法观察的 AtomUI 使用，必须由该工具输出 Usage metadata 或由应用添加显式 root。

保守误保留是允许的；静默漏保留不允许。

## 10. 应用计划

入口程序集聚合 Package、Unit、ControlMap 和所有引用程序集的 Usage 记录。它只完成：

1. 将 Control 使用映射为 Unit。
2. 按 Package 稳定排序和去重 Unit。
3. 为存在不确定性的 Package 选择 full fallback。
4. 生成按 Package ID 分派的强类型直接调用。

计划不遍历 Theme Asset、Catalog、Feature 或 Initializer 图。生成代码结构固定为：

```csharp
case "AtomUI.Desktop.Controls":
    GeneratedDesktopPackageSharedFragment.Add(packageBuilder);
    GeneratedButtonUnitFragment.Add(packageBuilder);
    GeneratedDatePickerUnitFragment.Add(packageBuilder);
    packageBuilder.Register();
    return true;
```

full fallback case 直接调用该 Package 的版本化 full registrar。fallback 在编译期确定；运行时不尝试发现缺失 Unit，也不从
异常重新引用 full registrar。

模块初始化器只把 plan delegate 安装到 `AotTrimRegistrationPlanRegistry`。真正的 Theme、Language 和 initializer
注册仍由用户调用的 `UseXxxControls()` 入口触发，保持可选包 gating 和调用顺序。

## 11. 动态输入与安全回退

安全策略是单调扩大：任何不确定性只能从 Unit 集合扩大为 Package full，不能缩小注册范围。

| 场景 | 结果 |
| --- | --- |
| 静态 Control 可确定 | 注册对应 Unit |
| 自定义 Control 的 AtomUI 基类可确定 | 注册基类 Unit |
| Unit 内部依赖 | 由 Unit fragment 直接调用并去重 |
| 显式 `PackageShared` | Package 注册时始终保留 |
| owner、AXAML 类型或已知动态 API 无法解析 | 对应 Package full fallback，并警告 |
| 旧第三方包只有 full registrar | 仅该第三方 Package full fallback，并警告 |
| Manifest major version 不兼容 | 构建错误 |
| 检测到 Control 使用但缺少对应 `UseXxxControls()` | 构建错误 |

显式 roots 只保留两类：

```xml
<ItemGroup>
  <AtomUIRegistrationUnitRoot Include="AtomUI.Desktop.Controls/DatePicker" />
  <AtomUIPackageRoot Include="MyCompany.DynamicControls" />
</ItemGroup>
```

`AtomUIRegistrationUnitRoot` 保留指定 Unit；`AtomUIPackageRoot` 强制对应 Package 使用 full registrar。完全不可观察的字符串、
网络数据或发布后插件必须使用 Package root。`AtomUIRegistrationStrict=true` 把自动 full fallback warning 提升为 error，
但不改变已计算的保留范围。

## 12. 公开入口与兼容性

用户入口保持：

```csharp
builder.UseDesktopControls();
```

显式全量入口为：

```csharp
builder.UseAllDesktopControls();
```

`UseDesktopControls()` 只在 `AotTrimRegistration.IsEnabled` 上分支。普通路径调用当前 full helper；生成式路径保留 Package Core
的原有顺序，仅把 package theme 提交替换为 `AotTrimRegistrationPlanRegistry.ApplyPackage(...)`。

DataGrid、ColorPicker、Extras 和第三方包仍由各自 `UseXxxControls()` 入口触发。静态发现不能绕过用户入口自动注册可选包。
同一应用进程中的多个 Builder 可以复用同一不可变计划，但每次应用都创建独立 package registration 实例。

`AotTrimRegistrationPlanRegistry`、Unit fragment 和 manifest ABI 虽然对用户隐藏，仍是跨程序集 Public API，必须有生成源码
ABI 快照和协议测试。运行时 ABI 的命名必须保持 AOT/Trim 语义，不能退化为 `RuntimeFeatures`、`GeneratedFeatures` 等通用名称。

## 13. 第三方包

第三方包参与 Unit 裁剪时必须：

- 引用兼容版本的 AtomUI Generator 和 buildTransitive targets。
- 输出 Package、Unit 和 ControlMap records。
- 为每个 Unit 输出公共隐藏 fragment。
- 显式声明 PackageShared 资源。
- 提供版本化 full registrar。
- 验证普通 full 注册与生成式注册的行为快照。

无法提供完整 Unit 契约的包可以只提供 full registrar。生成式应用仅对该 Package 执行 full fallback，不运行时扫描旧包。

## 14. Trimmability 与构建边界

参与生成式注册的 runtime 项目必须在真实 trimmed JIT 和 NativeAOT 验证后声明 `IsTrimmable`、`IsAotCompatible`。
这允许 linker 删除未引用 Unit fragment，但不能通过 warning suppression 伪造兼容性。

`AtomUI.Generator` 和 `AtomUI.Build.Tasks` 是纯构建期工具，不得进入应用运行依赖。它们必须隔离 `PublishAot`、
`PublishTrimmed`、`RunAOTCompilation`、RID 和 SelfContained 等 global property，避免工具项目被误判为发布入口。

声明 `AtomUIRegistrationPackageId` 的第一方产品 NuGet 必须自动携带同版本 Generator、Build Tasks 和 buildTransitive
资产。用户只引用产品包即可获得应用级计划；显式 Generator PackageReference 仍兼容。多个产品包或显式 Generator
并存时，构建入口必须根据 `ResolveReferences` 后的最终 `@(Analyzer)` 幂等注入，编译器只能接收一份 Generator。

buildTransitive targets 负责暴露模式属性、生成结构化 AXAML 输入、配置 feature switch，并在 ILLink/ILCompiler 前验证
应用计划标记。它们的项目求值阶段 Condition 不得读取 item list；需要检查 `@(Analyzer)` 的逻辑必须放入构建 Target
执行阶段。普通构建不得安装运行计划或改变包注册方式。Generator、Build Tasks 及其私有依赖只能存在于 NuGet 的
构建工具目录，不得进入 runtime dependency graph、`lib/`、应用输出或发布目录。

同版本第一方产品包内嵌的 Generator、Build Tasks 和共享 MSBuild 资产必须内容一致。发布打包不得跨版本使用
`--no-build` 复用陈旧工具输出。

## 15. 诊断契约

| ID | 条件 | 默认严重度 |
| --- | --- | --- |
| `ATOMUILINK001` | 生成式 publish 缺少或存在多个 Application Plan owner | Error |
| `ATOMUILINK002` | 静态使用无法精确映射，Package 已 full fallback | Warning |
| `ATOMUILINK003` | 第三方包缺少 Unit 协议，使用 full fallback | Warning |
| `ATOMUILINK004` | 显式 Unit/Package root 无法解析 | Error |
| `ATOMUILINK005` | Package Unit/Shared 定义冲突或不完整 | Error |
| `ATOMUILINK006` | Manifest 或 Generator ABI major version不兼容 | Error |
| `ATOMUILINK007` | Loose AXAML 或动态主题导致 Package full fallback | Warning |
| `ATOMUILINK008` | 检测到 Package 使用但缺少对应注册入口 | Error |

可修复诊断必须包含来源位置、Package/Unit identity 和可直接采用的 MSBuild root 示例。不能只输出“可能不兼容 AOT”。
`ATOMUILINK002` 和 `ATOMUILINK007` 的 metadata 传播不依赖当前项目是否 linked；其 Warning 仅在 linked publish 或
strict 模式可见，strict 模式继续把自动 full fallback 提升为 Error。其他协议错误不因普通构建而静默。

## 16. 验证与体积门槛

验证分为：

1. Package Generator 测试：Unit 归属、同包依赖、PackageShared、unknown 和 full fallback。
2. Usage 测试：AXAML/C#、类库传播、显式 roots、稳定排序和 diagnostics。
3. 运行集成测试：full/generated 的 descriptor、asset、语言、initializer、Provider 和冻结时序。
4. 真实发布测试：trimmed JIT 和 NativeAOT 的构建与运行 smoke；WebAssembly AOT 的生成、编译、链接和优化，
   并在平台基线允许时执行浏览器 runtime smoke。

仓库提供统一验证入口：

```bash
build/scripts/verify-aot-trim-registration.sh --full
```

该脚本验证 ordinary/generated 行为快照、trimmed JIT、Minimal/TwoUnits/DynamicFallback/Full NativeAOT、未使用 Unit
增量、Browser AOT 以及下列体积门槛。Gallery Desktop 仍需使用发布 workflow 的标准 NativeAOT 命令进行独立启动 smoke，
以覆盖真实窗口、主题资源和首帧布局路径。

体积比较必须固定 SDK、RID、Configuration、SelfContained、TrimMode 和测量口径。验收门槛为：

- 最小 Desktop Unit 应用相对 full registrar 至少缩小 `40%`。
- 新增一个未使用 Unit 后，同一最小应用产物增量不超过 `256 KiB`。
- 显式 PackageShared、Language 和 initializer 的固定成本在基线报告中单独记录。
- full registrar 的非裁剪 Registry 快照无行为差异。

门槛只能根据可复现的多平台基线调整，并记录 SDK/RID、原始值和调整原因。
