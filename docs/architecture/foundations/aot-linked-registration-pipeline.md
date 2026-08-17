# AOT Linked Registration Pipeline

> 状态：截至 2026-08-16，本文定义 AtomUI linked registration 的正式架构。

本文是 AtomUI linked registration 构建管线、Sidecar Manifest、应用静态计划和运行时边界的正式所有者。
Registration Unit 的产品粒度和资源归属由
[AOT Registration Unit 粒度](aot-registration-unit-granularity.md)定义；整体 AOT 目标和 Package Core 由
[AOT 与裁剪架构](aot-and-trimming.md)定义；Generator 的实现约束见
[Linked Registration Generator](../../modules/generator/linked-registration.md)。

## 1. 架构不变量

本架构必须同时保证：

1. 应用继续只调用真实入口，例如 `builder.UseDesktopControls()`。
2. 普通第三方包默认使用单一 Package Unit，不要求作者维护 Unit 图、Attribute、linker XML 或额外目录规则。
3. 普通 Debug 和未启用 AOT/Trim 的 Release 不加载 linked-publish analyzer，不收集 usage，也不生成应用计划。
4. linked publish 的全部推导在构建期完成；运行时不读取 Manifest、不遍历依赖图、不扫描程序集，也不延迟注册。
5. Unit fragment 是叶子，只注册本 Unit；应用编译期闭包负责去重和排序。
6. 无法证明精确结果时只扩大为当前 Package full registrar，不能生成可能漏注册的计划。
7. 分析规模必须有确定性结构上限；超限只能触发 fallback，不能形成无界运行或数 GB linker 图。

静态输入不足时，优先保证作者零新增负担、运行时零分析成本和运行正确性，不承诺所有动态程序都达到理论最小体积。

## 2. 构建模式

应用级 linked analysis 只由真实 linker 模式启用：

| 构建模式 | Linked analyzer | Application Plan |
| --- | --- | --- |
| Debug build/run | 不加载 | 不生成 |
| 未裁剪 Release | 不加载 | 不生成 |
| 非裁剪 self-contained / ReadyToRun | 不加载 | 不生成 |
| `PublishTrimmed=true` | 加载 | 入口应用生成 |
| `PublishAot=true` | 加载 | 入口应用生成 |
| `RunAOTCompilation=true` | 加载 | 入口应用生成 |

NuGet `Pack` 可以用内部 `AtomUIEmitLinkedManifest=true` 加载 Package Manifest compiler，目的是把依赖摘要预计算一次并交付给
消费者。Pack 不是普通应用构建模式，不能反向让日常 Debug/Release 加载 linked analyzer。

`AtomUIUseGeneratedRegistration=true` 不得再隐式加载完整应用分析管线。仓库测试如果需要在非裁剪环境验证 generated path，
必须通过隔离验证 target 显式启用。

## 3. 组件与数据流

```text
Package source + AXAML
        |
        | Pack / linked ProjectReference
        v
Package Manifest Compiler
        |
        +--> leaf Unit fragments in assembly
        `--> *.atomui-link.json
                    |
                    | buildTransitive / ReferencePath companion Sidecar
                    v
Application AOT/Trim compilation
        |
        +--> current application C#/AXAML usage
        +--> transitive usage sidecars
        +--> package declaration/dependency sidecars
        v
Compile-time package fallback + Unit closure + stable ordering
        v
GeneratedApplicationRegistrationPlan.g.cs
        v
Direct static Unit calls
```

管线包含三个职责：

| 组件 | 职责 |
| --- | --- |
| Ordinary Generator | 生成 Theme/Token/Localization、full registrar 和 leaf Unit fragment |
| Package Manifest Compiler | 生成 Package、Unit、ControlMap、UnitEdge、Usage 和 Fallback sidecar |
| Application Plan Generator | 仅在 linked app 中合并 usage、计算闭包并生成最终静态调用 |

应用编译不重新分析 NuGet Package 源码，ILLink 也不负责推导 AtomUI Unit 关系。

## 4. Analyzer 物理隔离

仅在 Generator 回调中检查 `AtomUILinkedPublish=false` 不能作为 Debug 零成本证明，因为 Analyzer assembly 仍会被 `csc` 加载。
Generator 布局为：

```text
AtomUI.Generator.dll
    ordinary generation

AtomUI.Generator.LinkedPublish.dll
    package manifest frontend
    application usage analysis
    application plan generation
```

MSBuild 必须在创建 linked AdditionalFiles、运行 AXAML usage task 和注入 Analyzer 前先判定构建模式。关闭时：

- `Csc` 的 Analyzer item 中没有 `AtomUI.Generator.LinkedPublish.dll`。
- 不创建空 usage XML 或 sidecar。
- 不向 incremental graph 提供 Compilation、SyntaxProvider 或 AdditionalTexts linked input。
- 不因为项目是 Exe、存在 AXAML 或引用 AtomUI Package 而推断开启。

## 5. Sidecar Manifest

### 5.1 资产边界

Sidecar 是纯编译资产：

- NuGet 中位于 `buildTransitive/AtomUI.LinkedRegistration/`。
- Package 自己的 `<PackageId>.targets` 只在 linked build 中把它加入 `AdditionalFiles`。
- linked ProjectReference 在程序集复制到 `TargetPath` 后生成 `<TargetPath>.atomui-link.json`；消费项目从已解析
  `ReferencePath` 的 companion 路径收集它，不递归调用 ProjectReference target。
- companion Sidecar 缺失时（引用库是普通构建，例如 `PublishAot`/`PublishTrimmed` 只在应用项目局部设置），消费项目直接从
  引用 assembly 的 metadata 记录提取 Sidecar 到自身 `obj`。普通库构建始终包含 Package/Unit/ControlMap/Axaml UnitEdge
  记录，但 C# UnitEdge 只由 linked 库构建计算，所以提取的 Sidecar 对每个 Package 附加 `ExtractedManifest` fallback：
  Application Plan 对该 Package 保持 full fallback，且不产生诊断。publish 时以全局属性传入
  `-p:PublishAot=true` / `-p:PublishTrimmed=true` / `-p:AtomUILinkedPublish=true`，引用库即自行产出完整 Sidecar，
  提取路径自动旁路。
- `CopyToOutputDirectory`、`CopyToPublishDirectory` 均为 `Never`。
- 不作为 EmbeddedResource，不进入运行时程序集、应用输出或 publish 目录。

### 5.2 记录模型

一个 assembly sidecar 可以包含：

| 记录 | 职责 |
| --- | --- |
| Package | Package ID、entry identities、full/shared fragment、粒度和稳定顺序 |
| Unit | Unit ID、leaf fragment type/method 和 order key |
| ControlMap | CLR metadata name 到 Package/Unit 的唯一 ownership |
| UnitEdge | source Unit 到 target Unit 的直接构建期依赖 |
| Usage | entry、Control Unit、Unit root 或 Package root 使用 |
| Fallback | Package fallback 和稳定 reason code |

Sidecar 使用 UTF-8、确定性 JSON 和唯一强类型 codec。Writer 统一控制字段顺序、数组排序、转义和路径规范化；协议实现不得
手工拼接 JSON 字符串。字段级契约由
[Linked Registration Sidecar](../../reference/aot/linked-registration-sidecar.md)定义。

### 5.3 版本和一致性

- 未知 major version 必须构建失败。
- 未识别的 optional minor 字段可以忽略。
- `contractHash` 覆盖 Package、Unit、ControlMap、fragment identity 和 dependency records。
- ProjectReference sidecar 与实际引用输出不匹配时必须重建。
- 首次冷构建必须在 ProjectReference 的 `Build` 返回前生成 Sidecar；不得依赖旧 `TargetPath` 让第二次构建偶然成功。
- 缺失或陈旧 sidecar 不能被解释为“没有 usage”，只能按兼容规则 full fallback。
- 发布 sidecar 不包含绝对源码路径；本地编译诊断可以使用真实 Roslyn Location。

## 6. Package Manifest 编译

### 6.1 Package 粒度

默认 Package 粒度只有一个 Unit，不需要 Unit dependency 推导。Manifest compiler 只记录 Package、单一 Unit、ControlMap、
entry 和 uncertainty。这是普通第三方包的默认正确性边界。

### 6.2 Directory 粒度

Directory 模式只分析直接构建期证据，不允许从调用点递归进入另一个方法体，也不允许遍历所有 SyntaxTree 的全部
`DescendantNodes()`。

允许的候选包括：

- Control 类型声明和基类。
- 显式或隐式对象构造。
- `typeof(...)` 和可静态解析的 Type 参数。
- Package registration entry 调用。
- 同程序集跨 Unit 的直接方法或构造调用。
- delegate、dynamic、reflection 和已知动态创建 API。
- 结构化 AXAML 的元素类型、TargetType、BasedOn、selector、template 和 `x:Type`。

每个源码/AXAML 文件先由统一粒度策略归入一个 Unit 或 Package Core。直接证据生成紧凑 UnitEdge；传递闭包只在应用编译期
对 Unit 图计算，不在 Roslyn 方法体或生成的 C# 调用图中展开。

Package Core 的普通成员调用不直接生成 Unit root：附加属性访问器和全局服务经常调用 Control 类型上的静态方法，但这不等于
实例化该控件或需要其主题。Package Core 的显式/隐式构造、`typeof(Control)` 等直接 Type 证据仍生成 Unit root；方法声明的
返回类型可证明为具体 Control 时，也为该返回 Control 生成 Type 证据。泛型方法在调用点把 `T` 替换为 Control 不视为工厂
证据，避免 `GetValue<TControl>()` 等读取路径错误 root Unit。Unit 到 Unit 的直接调用继续生成 `CSharpCall`；应用和普通类库
调用 Control 类型成员时记录 Control usage，因此 C# 附加属性使用仍能选择对应 Unit。

### 6.3 最终输出验证

Pack 或 linked ProjectReference 可以在编译后读取 PE metadata 和已编译资源，校验已知 `newobj`、`ldtoken`、entry call 和
generated resource wrapper 是否都能被 sidecar 解释。验证器不得构造程序集级递归调用图。发现未覆盖证据时，结果降级为
对应 Package fallback。

### 6.4 有界完成

分析使用候选数、Unit 数、直接边数和 sidecar bytes 等确定性结构预算，不使用墙钟超时决定输出。预算超限时写入
`AnalysisBudgetExceeded` fallback；strict 验证可以把该 Warning 提升为 Error。

当前硬上限为：单次 C# 或 AXAML 候选 50,000、Registration Unit 2,048、直接 UnitEdge 10,000、ControlMap 50,000、
Usage 50,000、Fallback 4,096、程序集 Manifest record 64,000、单个 sidecar 4 MiB、当前编译累计 sidecar 32 MiB。
这些数值属于编译协议实现约束，不能根据机器速度动态变化。

超过候选或 sidecar 预算后，分析器不再排序或构造细粒度 Unit 图，只线性保留可证明的 registration entry 和 Package root，
并把受影响 Package 扩大为 full registrar。Sidecar writer 同样把超预算输出压缩为 Package、entry 和
`AnalysisBudgetExceeded` fallback，不把异常大的精细记录交给消费端。

## 7. 传递类库

普通类库在日常 Debug/Release 中不扫描 usage，也不生成 Usage Sidecar。Sidecar 只在以下场景自动生成：

- 被 linked 应用作为 ProjectReference 构建。
- 自身被 Pack 为 NuGet Package。

NuGet Pack 自动交付 usage sidecar 和唯一 consumer target。缺少完整编译期输入的二进制按以下规则处理：

| 输入 | linked app 行为 |
| --- | --- |
| 完整且可验证的 sidecar | 使用精确 usages |
| 引用 AtomUI controls 但无 usage sidecar | 对相关且已调用入口的 Package fallback |
| 无法确定具体 Package | 对应用已调用的 AtomUI Packages fallback，并报告来源 assembly |

## 8. 应用静态计划

Application Plan Generator 只执行：

1. 验证 sidecar version、hash、Package ownership 和 fragment symbol。
2. 汇总当前应用与传递类库的 entry、Control usage、Unit root 和 Package root。
3. 检查每个已使用 Package 是否显式调用真实 registration entry。
4. 对 Exact Package 计算 Unit closure 和 SCC。
5. 对存在 uncertainty 的 Package 选择 full registrar。
6. 按 dependency SCC 和 Package order key 产生确定顺序。
7. 生成按 Package ID 分派的强类型静态调用。

复杂度接近：

```text
O(application candidates + sidecar bytes + selected Unit vertices + selected Unit edges)
```

应用计划不分析第三方方法体，不遍历 Theme Asset、Catalog、Initializer 或通用 Feature 图。

## 9. 叶子 Unit 与运行时边界

Unit fragment 只注册本 Unit：

```csharp
public static void Add(AotTrimControlPackageRegistrationBuilder builder)
{
    builder.AddControl(GeneratedThemeSchemaDescriptorFactory.CreateButton());
    AddThemes(builder);
}
```

fragment 中禁止：

```csharp
builder.TryEnterUnit(...);
OtherUnit.Add(builder);
AddDependencies(builder);
```

应用生成代码在编译期完成去重后，每个 leaf fragment 直接调用一次。Unit cycle 先折叠成 SCC，再按稳定 Package 顺序输出，
运行时不需要 visited set 或 dependency graph。

Sidecar 不进入运行时。`UseXxxControls()` 仍拥有 Package Core、Provider、Localization 和 initializer 顺序；FeatureSwitch 只在
普通 full registrar 与应用静态计划之间选择。运行时不得通过异常重试 full registrar。

## 10. Fallback

分析结果只有 `Exact` 和 `PackageFallback`，不存在 BestEffort：

| 条件 | 行为 |
| --- | --- |
| 精确静态 usage | 选择 Unit closure |
| 默认 Package 粒度 | 选择单一完整 Package Unit |
| loose AXAML / 动态主题 | 当前 Package full registrar |
| 无法静态解析的 C# 动态创建 | 不扩大保留范围，报告 `ATOMUILINK010` 警告，由显式 root 覆盖 |
| sidecar 缺失、陈旧或无法绑定 | 当前相关 Package full registrar |
| 未知 protocol major | 构建 Error |
| fragment symbol 不存在 | 构建 Error |
| 分析预算超限 | 当前 Package full registrar |
| usage 存在但 entry 未调用 | 构建 Error |
| Unit cycle | SCC 全选，每个 Unit 调用一次 |

Warning 必须包含 Package、reason 和可定位的输入身份。不能只报告“可能不兼容 AOT”。

## 11. 验证契约

### 11.1 普通构建

- `Csc` Analyzer item 中不存在 linked analyzer。
- linked AXAML/sidecar targets skipped。
- `obj` 中不产生 usage XML、sidecar 或 Application Plan。
- 与不安装 linked 功能的对照构建相比，耗时与内存差异处于测量噪声内，目标不超过 1%。

### 11.2 linked 编译

- Generator 代码中不存在全树 `DescendantNodes()` 和递归 member body traversal。
- no-op rebuild 命中 hash/cache，不重写 sidecar。
- 异常规模在确定性预算内完成或 Package fallback。
- Application Plan 只包含选中 Unit 的直接调用。

### 11.3 运行与体积

- ordinary/generated 的 descriptor、Theme asset、Language、Provider、initializer 和冻结时序一致。
- trimmed JIT、NativeAOT、WebAssembly AOT 和 Gallery NativeAOT smoke 通过。
- sidecar、Generator、Build Tasks、PDB 和分析缓存不进入 publish。
- 固定 `osx-arm64` Button/Window 样例移除中文字体后，第一阶段目标不超过 `18 MiB`，后续目标不超过
  `16 MiB` 或同场景 Fluent 的 125%。
- ILC map 不得保留没有直接或传递证据的 Picker、ListBox、DatePicker 等 Unit。

体积门槛统一使用 NativeAOT 主程序文件。publish payload 报告排除 `.dSYM`、`.pdb` 和 `.dbg`；调试符号以及所有场景
共享的 native dependency 都不能作为 Registration Unit 保留量的证据。门槛必须基于删除缓存后的真实 publish 和 ILC map；
设计目标不能替代实测结果。
