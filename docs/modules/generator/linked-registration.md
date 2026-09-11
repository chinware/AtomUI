# Linked Registration Generator

> 状态：截至 2026-08-20，本文定义 Generator 模块的正式实现边界。

本文负责 `AtomUI.Generator`、目标 linked-publish Generator 和 `AtomUI.Build.Tasks` 在 AOT/Trim 注册管线中的职责划分、
增量模型、禁止模式、输出和测试契约。系统级协议由
[AOT Linked Registration Pipeline](../../architecture/foundations/aot-linked-registration-pipeline.md)所有，机器可读字段由
[Linked Registration Sidecar](../../reference/aot/linked-registration-sidecar.md)定义。

## 1. 模块职责

linked registration 工具链只负责构建期事实：

- 从真实 registration entry symbol 派生 Package 入口身份。
- 从统一粒度策略派生 Package/Directory Unit 和 ControlMap。
- 从 C#/AXAML 直接证据生成 UnitEdge、Usage 和 Fallback。
- 生成确定性 sidecar。
- 在入口应用 linked 编译时生成最终静态 Package plan。
- 校验协议、fragment symbol、sidecar hash 和安全 fallback。

它不拥有 Package Core 执行顺序，不解析 `UseXxxControls()` 方法体猜测初始化语义，也不在运行时执行发现。

## 2. 组件边界

### 2.1 Ordinary Generator

普通 `AtomUI.Generator.dll` 保留所有日常构建必需工作：

- Theme Schema、Token、Localization 和 resource wrapper。
- full registrar。
- 不包含依赖调用的 leaf Unit fragment。
- Package entry Attribute 的基本签名诊断，以及普通生成所需的 Unit identity。

Ordinary Generator 不扫描应用 usage，不计算 Unit closure，不读取 transitive sidecar。

### 2.2 Linked-publish Generator

目标 `AtomUI.Generator.LinkedPublish.dll` 只在 AOT/Trim、Pack manifest 或仓库 strict 验证中由 MSBuild 注入。它包含：

- Package Manifest compiler frontend。
- 应用 C#/AXAML usage analyzer。
- Sidecar catalog/codec。
- Application Plan generator。

普通 Debug/Release 的 compiler command line 中不得出现该 assembly。

### 2.3 Build Tasks

`AtomUI.Build.Tasks` 负责适合 MSBuild/文件阶段的工作：

- 结构化 AXAML candidate 收集。
- Sidecar canonical read/write 和 hash 检查。
- ProjectReference target output 聚合。
- Pack asset 与 consumer target 生成。
- Sidecar candidate catalog 的来源标记、程序集身份解析和 canonical resolution。
- 最终 PE/resource direct-evidence 验证。

Build Task 不加载应用运行时，不执行程序集反射，也不构造递归方法调用图。

## 3. 增量输入模型

linked Generator 使用候选驱动的 Incremental API。允许的源候选为：

- Control 类型声明和基类。
- 对象构造、`typeof(...)` 和已知静态 Type 输入。
- registration entry 调用。
- Directory 模式的跨 Unit 直接调用。
- delegate、dynamic、reflection 和已知动态 API。
- Build Task 输出的结构化 AXAML candidates。

每个 transform 输出可比较的 immutable value，例如 metadata name、Unit ID、source identity、reason code 和 location value。
禁止把 `Compilation`、`SemanticModel`、SyntaxNode、ISymbol 或完整 AdditionalText 内容放入持久缓存结果。

`CompilationProvider` 只用于最终 symbol 绑定和输出验证，不能作为“遍历整个 Compilation”的入口。

## 4. 禁止的分析模式

linked registration 实现中禁止：

- 对每个 SyntaxTree 调用 `GetRoot().DescendantNodes()`。
- 从 invocation 递归进入 callee syntax，再从 callee 继续递归。
- 为每个 owner Unit 重复扫描同一个方法、属性或字段初始化器。
- 通过方法名字符串或源码文本猜测 registration entry。
- 把 field/property/parameter/return type 的普通类型引用当成 Control 实例化。
- 用墙钟超时决定 Manifest 内容。
- 把不确定性静默忽略或当成没有 usage。

需要传递关系时，只能把候选阶段产生的直接 UnitEdge 交给应用计划的紧凑 Unit 图闭包。不得把 dependency 重新编码为 Unit
fragment 之间的 C# 调用。

## 5. Package 分析

### 5.1 Package 粒度

Package 粒度输出一个完整 Unit，不运行 dependency analyzer。所有 Control-owned 内容天然属于该 Unit。

### 5.2 Directory 粒度

每个 source/AXAML file 必须先经同一个 `RegistrationUnitId` 策略归入 Unit 或 Package Core。直接证据转换规则为：

- Unit A 直接创建或静态引用 Unit B Control：`A -> B`。
- Unit A 直接调用 Unit B 拥有的成员：保守 `A -> B`，不进入 B body。
- Package Core 对 Unit B 的直接 Control 类型证据（显式/隐式构造、`typeof`）生成 Package root 到 B。
- Package Core 调用的方法声明直接返回具体 Control 时，对返回 Control 的 Unit 生成 Package root；普通成员调用和泛型返回类型的调用点替换本身不形成 root。
- AXAML owner Unit 引用 Unit B：`A -> B`。

Package Core 会随 Package 入口无条件执行，因此它对附加属性访问器、静态事件帮助方法等 Control 成员的调用只证明运行时代码依赖，
不证明对应 Control Theme 会被实例化。把这类 `Call` 直接提升为 Unit root 会让全局服务错误保留完整控件族。Unit 内的跨 Unit
调用仍保守生成 `CSharpCall`；Package Core 只有直接 Type 证据或方法声明的具体 Control 返回值证据才 root Unit。应用和普通
类库直接调用 Control 类型成员时仍记录该 Control usage，使 C# 附加属性访问可以选择对应 Unit。

无法唯一归属、动态 target set 不封闭、generated IL 无法解释或结构预算超限时，输出 PackageFallback。

## 6. Sidecar 输出

Sidecar writer 必须：

- 使用唯一强类型 model 和 JSON codec。
- 对 Package、Unit、ControlMap、UnitEdge、Usage 和 Fallback 稳定排序。
- 规范化为相对路径，不写入开发机绝对路径。
- 计算覆盖全部协议事实的 contract hash。
- 内容未改变时不更新时间戳。
- 不把 sidecar 添加为 EmbeddedResource、Content 或 publish asset。

Codec 的 unknown-major、optional-minor、缺字段、重复 ownership、hash mismatch 和 invalid fragment identity 必须分别测试。

### 6.1 Consumer Resolution Contract

Generator 接收的 `AdditionalFiles` 必须已经是消费端 canonical resolution 的结果。Generator 不负责通过“保留第一份”来修复
MSBuild 传入的重复 Sidecar；如果同一程序集的多个候选未经解析就到达 Generator，应按协议冲突报告 `ATOMUILINK005`。

消费端解析以 Sidecar 声明的 `assembly.name` 为身份，以 `contractHash` 判断内容是否相同：

- ProjectReference companion 和 NuGet package 的优先级只用于同 hash 等价候选；metadata extraction 只在正式 Sidecar 缺失时生成。
- 同身份同 hash 的候选可以折叠，但最终只保留一个 canonical input。
- 同身份不同 hash 不能静默选择，必须报告所有来源和冲突 hash。
- 已有正式 Package/companion Sidecar 的程序集不允许再次生成 `ExtractedManifest`。
- linked 应用的源码 ProjectReference 自动接收 `AtomUILinkedPublish=true` 和 `AtomUIRegistrationPlanOwner=false`；传播同时覆盖
  evaluation direct refs 与 SDK 展开的 transitive refs，并排除 analyzer refs。
- 没有正式 Sidecar 的预编译消费 DLL 先按 AssemblyRef 过滤，再从 IL entry call 恢复 `PackageRoot`/`Entry`，并以
  `ExtractedConsumerAssembly` 标记 full fallback。只有 PackageRoot 没有 Entry 时报告 `ATOMUILINK008`。

这个契约保证 Generator 看到的是“每个程序集一个 Manifest”，同时保留普通 ProjectReference 缺少 companion 时的 full fallback
兼容路径。路径名、包目录和 DLL 相邻文件关系只能作为候选发现线索，不能作为最终身份判断。

## 7. Application Plan 输出

Application Plan generator 必须在生成源码前完成：

1. sidecar catalog 验证。
2. entry 与 usage 匹配。
3. Package fallback 决策。
4. Unit closure 和 SCC。
5. fragment symbol 验证。
6. stable package/unit ordering。

输出只包含：

```csharp
GeneratedPackageSharedFragment.Add(packageBuilder);
GeneratedWindowUnit.Add(packageBuilder);
GeneratedTextBoxUnit.Add(packageBuilder);
packageBuilder.Register();
```

每个 Unit fragment 最多出现一次。Generator 不在源码中输出 adjacency array、visited set、dictionary 或运行时 fallback 分支。

## 8. Leaf Fragment Writer

Theme Schema 和 Theme Asset writer 共同生成一个 leaf Unit partial class：

- Theme Schema 部分添加本 Unit descriptor。
- Theme Asset 部分添加本 Unit asset 和 resource factory。
- PackageShared 使用独立 fragment。
- dependency 只写 sidecar UnitEdge。

writer 不生成 `AddDependencies`，不调用其他 Unit，也不调用 `TryEnterUnit`。

## 9. 诊断与 fallback

诊断必须使用稳定 reason code，并包含 Package 和可用 source identity。至少区分：

- unresolved owner。
- dynamic/reflection usage（不扩大保留范围，报告 `ATOMUILINK010`，由显式 root 覆盖）。
- loose or unresolved AXAML。
- missing/stale sidecar。
- unknown protocol major。
- missing fragment symbol。
- analysis budget exceeded。
- detected usage without registration entry。

可安全扩大的问题使用 PackageFallback Warning；协议不兼容、fragment 不存在和入口缺失使用 Error。strict 模式只提升 fallback
Warning，不改变保留范围。

## 10. 性能预算

结构预算必须是输入确定的计数，不得依赖机器速度。linked-publish 实现统一使用以下硬上限：

| 输入或结构 | 上限 |
| --- | ---: |
| C# source candidates | 50,000 |
| AXAML candidates | 50,000 |
| 单个 AXAML usage 输入 | 4 MiB |
| Package count | 1,024 |
| Registration Unit count | 2,048 |
| direct UnitEdge count | 10,000 |
| Package Core root Unit count | 2,048 |
| ControlMap count | 50,000 |
| Usage count | 50,000 |
| Fallback count | 4,096 |
| assembly Manifest records | 64,000 |
| 单个 sidecar | 4 MiB |
| 当前编译累计 sidecar | 32 MiB |

预算超限立即停止细粒度输出并生成一个 PackageFallback，不继续累积方法、symbol substitution 或 Unit graph。
应用候选超限后只做线性 entry/Package root 汇总；sidecar 超限后 writer 和 consumer 都只保留生成 full registrar 所需的
Package 与 entry 信息。SCC 发现和组件依赖排序必须使用显式栈和有界集合，禁止递归 DFS，避免异常长链导致栈溢出。

普通构建性能以“linked analyzer 未加载”为验收标准，而不是仅以 callback 快速 return 为标准。linked build 的 no-op rebuild 必须
复用 sidecar hash 和 Incremental cache。

## 11. 测试契约

### 11.1 Unit tests

- Package/Directory 粒度和统一 Unit identity。
- C# direct candidates、AXAML candidates 和 direct UnitEdge。
- dynamic、generated output、budget 和 unknown fallback。
- codec、determinism、hash 和 version compatibility。
- closure、SCC、stable order 和 fragment symbol validation。
- leaf fragment 不包含 dependency call 或 `TryEnterUnit`。

### 11.2 Build integration

- Debug/普通 Release 的 `Csc` Analyzer item 不含 linked analyzer。
- linked AXAML target 在普通构建 skipped。
- ProjectReference sidecar 只在 linked build 传递；companion 缺失时 consumer 从引用 assembly metadata 提取，提取结果带 `ExtractedManifest` fallback（full fallback、无诊断）。
- NuGet package Sidecar 与 extracted fallback 的重复程序集回归：包内正式 Sidecar 存在时不得生成 extraction candidate，linked build 不得报 `ATOMUILINK005`。
- Sidecar candidate resolution：同身份同 hash 折叠为一份，同身份不同 hash 以 `ATOMUILINK005` 失败并保留来源诊断。
- NuGet sidecar/consumer target 自动打包且不进入 runtime assets。
- sidecar 缺失、损坏或无法验证时只触发 Package fallback。

### 11.3 Publish verification

- ordinary/generated registry snapshot 一致。
- Minimal、TwoUnits、DynamicFallback、Full 和 unused Unit fixtures。
- trimmed JIT、NativeAOT、WebAssembly AOT 和真实 Gallery smoke。
- ILC map 只保留选中 Unit 和可证明依赖。
- 删除缓存后记录 wall time、peak RSS、主程序大小和 publish asset 清单。
