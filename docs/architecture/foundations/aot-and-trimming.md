# AOT 与裁剪架构

> 状态：截至 2026-08-20，本文定义 AtomUI Registration Unit、Sidecar Manifest、应用静态计划和安全 fallback 的正式架构。

本文是 AtomUI AOT 与 trimming 整体架构的正式所有者，覆盖 `AtomUI.Core`、Control Packages、Generator、Build Tasks、
应用项目和第三方包。专项契约分别由以下文档维护：

- [AOT Linked Registration Pipeline](aot-linked-registration-pipeline.md)：Analyzer 激活、Sidecar、UnitEdge、静态计划和运行时边界。
- [AOT Registration Unit 粒度](aot-registration-unit-granularity.md)：Package/Directory 粒度和资源归属。
- [Linked Registration Sidecar](../../reference/aot/linked-registration-sidecar.md)：机器可读协议。
- [AOT 编程规范](../../engineering/development/aot-programming-guidelines.md)：日常开发和 review 规则。
- [第三方 Control Package 指南](../../guides/theming/third-party-control-packages.md)：包作者接入步骤。

## 1. 问题定义

AtomUI 的 Theme 和 Localization Registry 必须在首帧前完整构建并冻结。构造全包 descriptor、Theme Asset 或 AXAML factory
后再运行时过滤，不能让 ILLink 删除未使用控件；反射扫描、linker XML、控件实例化后追加注册又不能同时满足 NativeAOT、
冻结时序和作者体验。

AtomUI 因此把静态可达性拆成 Registration Unit。普通包以整个 Package 为安全 Unit；只有大型多控件包才按稳定控件族拆分。
应用 AOT/Trim 编译读取纯构建期 Sidecar，计算 Unit closure，并生成最终强类型静态调用。

## 2. 架构不变量

1. 应用继续只调用真实入口，例如 `builder.UseDesktopControls()`。
2. 普通 Debug 和未启用 AOT/Trim 的 Release 使用完整兼容注册，不加载 linked-publish Analyzer。
3. `PublishTrimmed=true`、`PublishAot=true` 和 `RunAOTCompilation=true` 自动使用应用静态计划。
4. 普通第三方包默认只有一个 Package Unit，不声明 Unit dependency、ownership 修补或 linker XML。
5. 运行时不读取 Sidecar、不遍历 Unit 图、不扫描程序集、类型、AXAML、Attribute 或资源目录。
6. Registry 仍在首帧前一次性构建并冻结，不引入 late registration。
7. Unit fragment 是叶子，不调用其他 Unit，也不运行时去重。
8. 无法证明精确 Unit 集合时，只扩大为对应 Package full registrar。
9. Language、Provider、Global Token、Theme Algorithm 和初始化逻辑保持 Package 级语义与顺序。
10. 分析必须增量且有确定性结构预算；超限触发 fallback，不能形成无界编译或巨大 linker 方法图。
11. 每个程序集在一次 linked build 中只能有一个生效的 Sidecar。Package、ProjectReference companion 和 metadata 提取
    是有优先级的候选来源；提取 fallback 只能补足缺失的正式 Sidecar，不能与已交付 Sidecar 并存。

对于任意动态程序集、反射、脚本或发布后插件，不可能同时保证理论最小体积、作者零声明和运行时零发现。本架构保证后两项和
运行正确性；静态证据不足时牺牲体积。

## 3. 构建模式

| 构建模式 | 注册模式 | Linked Analyzer |
| --- | --- | --- |
| 普通 `dotnet build` / `dotnet run` | full registrar | 不加载 |
| 未裁剪 Release、self-contained、ReadyToRun | full registrar | 不加载 |
| `PublishTrimmed=true` | application static plan | 加载 |
| `PublishAot=true` | application static plan | 加载 |
| `RunAOTCompilation=true` | application static plan | 加载 |
| 显式 `UseAllDesktopControls()` | full registrar | 不改变构建模式 |

Build Targets 把真实 linker 条件规范化为内部 `AtomUILinkedPublish`，并配置 linker 可替换的
`AtomUI.AotTrimRegistration.Enabled` feature switch。`AtomUIUseGeneratedRegistration=true` 不得隐式加载完整应用分析；仓库测试
通过隔离 target 验证非裁剪 generated path。

普通构建必须同时满足：compiler command line 不含 linked Analyzer、AXAML/Sidecar targets skipped、`obj` 中没有 linked usage、
Sidecar 或 Application Plan。

## 4. Registration Unit

Registration Unit 是唯一细粒度裁剪单位，不等于单个 CLR 类型。一个 Unit 包含可以独立运行的控件族：

```text
DatePicker Unit
├── DatePicker / RangeDatePicker
├── Presenter、Cell 和内部辅助控件
├── Control descriptors
├── Own Token schema
└── 控件族专属 AXAML factories
```

默认 `Package` 粒度把当前 Control Package 的所有 Control-owned 内容放入一个 Unit。显式 `Directory` 粒度才按稳定控件族拆分。
Unit ownership、`AtomUIRegistrationUnit` 和 `AtomUIPackageSharedTheme` 的使用边界由
[AOT Registration Unit 粒度](aot-registration-unit-granularity.md)定义。

每个 Unit 生成一个跨程序集可调用的 leaf entry：

```csharp
public static void Add(AotTrimControlPackageRegistrationBuilder builder)
```

入口只添加本 Unit descriptor、Theme Asset 和 resource factory。UnitEdge 写入 Sidecar；应用编译期计算 closure/SCC，并让每个
fragment 最多出现一次。禁止生成 `AddDependencies`、调用其他 Unit 或调用 `TryEnterUnit`。

## 5. Package Core

以下内容按 Package 整体保留，不参与 Unit 拆分：

- Language Catalog 和内置 Translation Bundles。
- Dialog、Tooltip、Motion、Responsive 等初始化逻辑。
- Global Token、Theme Algorithm 和 Package Provider。
- 平台 Asset Selector。
- 显式 `PackageShared` 主题资源。

以 Desktop 为例，生成式入口顺序必须保持：

1. 注册依赖 Package；跨 Package 不计算 Unit closure。
2. 执行当前 Package 提交前初始化。
3. 创建平台 Provider，并应用静态 Unit plan 或 full fallback。
4. 注册完整 Language Module。
5. 按既有顺序添加 Theme initializer。

`AtomUI.Controls` Common 由 Desktop 完整注册，不是独立 linked Package，不声明 registration entry，也不进入应用 Package plan。

## 6. 构建期 Pipeline

Control Package 在 NuGet Pack 或 linked ProjectReference 构建时自动生成 Sidecar。Sidecar 记录 Package、Unit、ControlMap、
UnitEdge、Usage 和 Fallback；它通过 `buildTransitive` 或已解析 `ReferencePath` 旁的 companion Sidecar 传递，只作为 linked
build 的 AdditionalFile，不进入运行时程序集或 publish 目录。ProjectReference 的 Sidecar 必须在目标程序集复制到
`TargetPath` 后生成，使首次冷构建和增量构建具有相同输入。

### 6.1 Sidecar 来源与唯一性

Sidecar 物理路径不能代表 Manifest 身份。消费项目必须先解析 Package 和 ProjectReference 正式 Sidecar，再只对没有正式
Manifest 的引用生成 metadata extraction fallback；最终按 `assembly.name` 和 `contractHash` 解析每个程序集唯一的 canonical
Sidecar。不同 hash 的同身份候选必须构建失败，不能依赖文件名、路径或 MSBuild item 顺序选择。

来源优先级、两阶段解析算法、冲突诊断和构建回归矩阵由
[AOT Linked Registration Pipeline](aot-linked-registration-pipeline.md#52-sidecar-candidate-resolution)统一定义。

Application Plan Generator：

1. 验证 Sidecar protocol、hash、ownership 和 fragment symbol。
2. 汇总当前应用及传递类库的 entry、Control usage、Unit root 和 Package root。
3. 检查被使用 Package 的真实 registration entry 已显式调用。
4. 对 Exact Package 计算 Unit closure 和 SCC。
5. 对不确定 Package 选择 full registrar。
6. 按 dependency SCC 和 Package order key 生成确定性强类型调用。

Package 分析只使用候选驱动的直接 C#/AXAML 证据。禁止全树 `DescendantNodes()`，禁止从 invocation 递归进入 callee body，
禁止把 dependency 编码成 fragment 之间的调用。完整算法和性能预算见
[AOT Linked Registration Pipeline](aot-linked-registration-pipeline.md)。

Package Core 调用 Control 类型上的普通静态成员只表示运行时代码依赖，不自动 root 该 Control Unit。只有直接构造、`typeof`
等 Type 证据，或方法声明直接返回具体 Control 时才生成 Package root；泛型返回值的调用点替换不作为工厂证据。Unit 内跨
Unit 调用仍生成直接 UnitEdge，应用和普通类库的 Control 成员调用仍形成该 Control 的 usage。

## 7. 静态使用和动态 Root

自动发现至少覆盖：

- C# 显式/隐式对象构造、`typeof(...)`、Control 基类和 registration entry 调用。
- Directory 模式同程序集跨 Unit 的直接方法或构造调用。
- AXAML 元素类型、TargetType、BasedOn、selector、DataTemplate、ControlTemplate 和 `x:Type`。

Control 的普通 field/property/parameter/return type 引用不等于实例化，不单独形成 Unit usage。无法验证的 generated output、
dynamic、reflection、Loose AXAML 和插件输入触发 Package fallback。

消费应用处理真正动态输入时可以显式声明：

```xml
<ItemGroup>
  <AtomUIRegistrationUnitRoot Include="AtomUI.Desktop.Controls/DatePicker" />
  <AtomUIPackageRoot Include="MyCompany.DynamicControls" />
</ItemGroup>
```

`AtomUIRegistrationUnitRoot` 保留指定 Unit；`AtomUIPackageRoot` 强制对应 Package full registrar。它们不是普通 Control Package 的
接入步骤。

## 8. 安全 Fallback

分析结果只有 Exact 和 PackageFallback。安全策略是单调扩大：

| 场景 | 结果 |
| --- | --- |
| 静态 Control/AXAML 可确定 | 选择对应 Unit closure |
| 默认 Package 粒度 | 选择完整 Package Unit |
| Unit cycle | SCC 全选，每个 leaf fragment 调用一次 |
| Loose AXAML / 动态主题 | 对应 Package full registrar |
| 无法静态解析的 C# 动态创建（`Activator.CreateInstance(Type)` 等） | 不扩大保留范围，报告 `ATOMUILINK010` 警告，由显式 root 覆盖 |
| Sidecar 缺失、陈旧或无法验证 | 对应 Package full registrar |
| Package 或 ProjectReference 已提供同程序集正式 Sidecar | 禁止再次生成 `ExtractedManifest` |
| 同程序集 Sidecar 身份相同且 `contractHash` 相同 | 合并为一个 canonical Sidecar |
| 同程序集 Sidecar `contractHash` 不同 | 构建 Error，报告来源冲突 |
| ProjectReference Sidecar 由 consumer 从普通构建的 assembly metadata 提取（`ExtractedManifest`） | 对应 Package full registrar，不产生诊断 |
| 预编译消费 DLL 由 consumer 从 AssemblyRef/IL 恢复（`ExtractedConsumerAssembly`） | 对应 Package full registrar，不产生诊断；没有入口调用时构建失败 |
| 分析预算超限 | 对应 Package full registrar |
| 未知 protocol major | 构建 Error |
| Fragment symbol 不存在 | 构建 Error |
| 使用 Package 但未调用 entry | 构建 Error |

Fallback 在编译期确定。运行时不捕获缺失资源后重试，不重新引用 full registrar，也不扫描 Package。

## 9. 公开入口与运行时 ABI

Package 的公开注册方法是入口身份的唯一事实来源：

```csharp
[ControlPackageRegistrationEntry]
public static IAtomUIBuilder UseAcmeControls(this IAtomUIBuilder builder)
{
    // 保持 Package Core 和 full/generated 分支顺序。
    return builder;
}
```

Attribute 不接收 Package ID、类型名或方法名。Generator 从 `IMethodSymbol` 派生稳定 identity，并验证方法是 public、static、
非泛型 `IAtomUIBuilder` 扩展方法。项目文件不得维护入口类型名或方法名字符串。

`UseXxxControls()` 仍拥有 Package Core、Provider、Localization 和 initializer 顺序。应用静态发现不能绕过入口自动启用可选包。

隐藏运行时 ABI 保持明确的 `AotTrim` 命名：

| 类型 | 职责 |
| --- | --- |
| `AotTrimRegistration` | 读取发布 feature switch |
| `AotTrimControlPackageRegistrationBuilder` | 收集已选 Unit 并提交完整 Package registration |
| `AotTrimRegistrationPlan` | 应用静态计划的跨程序集调用协议 |
| `AotTrimRegistrationPlanRegistry` | 安装唯一 plan 并按 Package 静态分派 |

运行时 ABI、fragment 和 Sidecar protocol 都必须有快照和兼容性测试。

## 10. 第三方 Package

普通第三方包只需要：

1. 引用 AtomUI 产品 Package，让它自动提供 Generator、Build Tasks 和 buildTransitive assets。
2. 声明稳定 `AtomUIRegistrationPackageId`。
3. 按 Control、可选 Own Token 和 `Themes/` 约定组织源码。
4. 在真实 `UseXxxControls()` 上添加 `[ControlPackageRegistrationEntry]`。
5. 保持 full/generated、Provider、Localization 和 initializer 顺序。
6. 使用默认 Package 粒度并验证 ordinary/generated 行为。

Pack 自动生成和交付 Sidecar。作者不写 Unit dependency、ownership 修补、Sidecar、linker XML 或运行时扫描。只有大型多控件包
才启用 Directory 粒度并承担真实发布和体积验证。

## 11. Trimmability 与打包边界

runtime 项目只有在真实 trimmed JIT 和 NativeAOT 验证后才能声明 `IsTrimmable`、`IsAotCompatible`；warning suppression 不能
代替兼容实现。

ordinary Generator、linked-publish Generator、Build Tasks、Sidecar、PDB 和分析缓存都是纯构建资产：

- 普通构建只注入 ordinary Generator。
- AOT/Trim 才注入 linked-publish Generator。
- Pack 自动交付 Sidecar 和唯一 consumer target。
- 多个产品包必须幂等注入 Analyzer。
- 构建工具不得进入 `lib/`、runtime dependency graph、应用输出或 publish 目录。

具体 MSBuild 和 NuGet 规则见 [构建与打包](build-and-packaging.md)。

## 12. 诊断契约

| ID | 条件 | 默认严重度 |
| --- | --- | --- |
| `ATOMUILINK001` | 缺少或存在多个 Application Plan owner | Error |
| `ATOMUILINK002` | 静态使用无法精确映射，Package full fallback | Warning |
| `ATOMUILINK003` | Package 缺少可验证 Sidecar，full fallback | Warning |
| `ATOMUILINK004` | 显式 Unit/Package root 无法解析 | Error |
| `ATOMUILINK005` | Package 粒度、Unit/Shared 或 ownership 冲突 | Error |
| `ATOMUILINK006` | Sidecar 或 Generator ABI major 不兼容 | Error |
| `ATOMUILINK007` | Loose AXAML 或动态主题导致 Package fallback | Warning |
| `ATOMUILINK008` | 检测到 Package 使用但缺少 registration entry | Error |
| `ATOMUILINK009` | Registration entry Attribute 或签名无效 | Error |
| `ATOMUILINK010` | C# 动态创建无法静态解析，不扩大保留范围，需显式 root 覆盖 | Warning |

诊断必须包含 Package/Unit identity、稳定 reason 和可定位输入，不能只写“可能不兼容 AOT”。Fallback Warning 只在 linked publish
或显式 strict 验证中产生；strict 模式可以提升为 Error，但不改变保留范围。

## 13. 验证和体积门槛

统一验证必须覆盖：

- Package/Directory、entry、Unit ownership、direct UnitEdge、PackageShared、budget 和 fallback。
- Sidecar codec、hash、确定性、ProjectReference/NuGet 传播、来源优先级、程序集身份去重和 fragment symbol。
- NuGet 正式 Sidecar 与 metadata extraction 不得同时进入 `AdditionalFiles`；相同身份同 hash 只保留一份，异 hash 必须失败。
- 普通 Debug/Release 不加载 linked Analyzer、不运行 linked targets、不产生 linked 中间文件。
- ordinary/generated descriptor、Theme Asset、Language、Provider、initializer 和冻结时序一致。
- trimmed JIT、NativeAOT、WebAssembly AOT 和真实 Gallery 启动 smoke。
- Sidecar、Generator、Build Tasks、PDB 和缓存不进入 publish。

仓库统一入口：

```bash
scripts/verification/verify-aot-trim-registration.sh --full
```

体积比较必须固定 SDK、RID、Configuration、SelfContained、TrimMode 和测量口径。门槛为：

- 最小 Desktop Unit 的 NativeAOT 主程序相对 full registrar 主程序至少缩小 `40%`。
- 固定 `osx-arm64` Button/Window 样例移除中文字体后，第一阶段主程序不超过 `18 MiB`；后续目标不超过 `16 MiB`
  或同场景 Fluent 的 125%。
- 新增一个未使用 Unit 后主程序增量不超过 `256 KiB`。
- ILC map 不保留没有直接或传递证据的其他控件族 Unit。
- full registrar 的非裁剪 Registry 快照无行为差异。

目录总量只统计可交付 payload，排除 `.dSYM`、`.pdb` 和 `.dbg`；缩减率、绝对上限和未使用 Unit 增量统一以主程序
文件为准，避免调试符号和所有场景共享的 native library 扭曲 linked registration 的收益。门槛只能根据删除缓存后的可复现
多平台数据调整，并记录 SDK/RID、原始值和原因。
