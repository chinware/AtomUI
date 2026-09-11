# Linked Registration Sidecar

本文定义 AtomUI linked registration Sidecar 的机器可读契约。Sidecar 是构建期资产，不是运行时配置文件。

## 文件和编码

- 扩展名：`.atomui-link.json`。
- 编码：UTF-8，无 BOM。
- 内容：单个 JSON object。
- NuGet 位置：`buildTransitive/AtomUI.LinkedRegistration/`。
- ProjectReference 位置：项目 `obj` 输出，由 MSBuild target item 传递。
- ProjectReference 旁路缺失时，consumer 直接从引用 assembly 的 metadata 记录提取 Sidecar 到自身 `obj`（见 `ExtractedManifest`）。
- 预编译消费 DLL 没有正式 Sidecar 时，consumer 可从其 AssemblyRef/IL 恢复 Sidecar 到自身 `obj`（见
  `ExtractedConsumerAssembly`）。
- 不作为 `EmbeddedResource`、`Content`、runtime asset 或 publish asset。

Producer 必须使用结构化 JSON writer。Consumer 必须使用结构化 parser；不得用正则或字符串切割解析。

## 候选来源和 Canonical Resolution

Consumer 可以从 NuGet package、ProjectReference companion 和 assembly metadata extraction 获得 Sidecar candidate。候选的
物理路径只用于定位文件，不是 Manifest 身份；NuGet Sidecar 与程序集不要求位于同一目录。

每个 candidate 必须通过 consumer-side item metadata 携带 `Package`、`ProjectCompanion` 或 `MetadataExtraction` 来源；这是
构建管线元数据，不新增 Sidecar JSON 字段。Consumer 按以下契约解析：

1. 使用 `assembly.name` 作为程序集身份。
2. 使用 `contractHash` 判断同一身份的 canonical 内容是否相同。
3. 同身份同 hash 时按 ProjectReference companion、NuGet package、metadata extraction 的顺序选择 canonical candidate。
4. 同身份不同 hash 时不得按优先级静默覆盖，必须以 `ATOMUILINK005` 构建失败并报告全部来源。
5. 已有 Package 或 ProjectReference 正式 Sidecar 的程序集不得生成 metadata extraction candidate。
6. 只有 canonical candidate 可以进入 Roslyn `AdditionalFiles`。

Metadata extraction 是缺失正式 Sidecar 时的 fallback，不是与正式 Sidecar并行的第二份 Manifest。它继续携带
`ExtractedManifest`，并使相关 Package 使用 full fallback。

## 顶层字段

| 字段 | 必需 | 含义 |
| --- | --- | --- |
| `protocolMajor` | 是 | 协议 major；未知值构建失败 |
| `protocolMinor` | 是 | 协议 minor；未知 optional 字段可以忽略 |
| `producer` | 是 | 生成工具身份，只用于诊断 |
| `assembly` | 是 | 当前 assembly identity 和 contract hash |
| `packages` | 是 | 当前 assembly 定义的 Package records；可以为空 |
| `usages` | 是 | 当前 assembly 对 AtomUI Package/Unit/entry 的 usage records；可以为空 |
| `fallbacks` | 是 | 当前 assembly 已知的不确定性；可以为空 |

数组即使为空也必须存在，避免“字段缺失”和“没有记录”产生歧义。

## Assembly

`assembly` 包含：

| 字段 | 含义 |
| --- | --- |
| `name` | Assembly simple name |
| `contractHash` | 覆盖本文件全部协议事实的 canonical content hash |
| `targetFramework` | 产生 Sidecar 的目标框架 provenance；不是跨兼容 TFM 消费时的去重身份 |

Sidecar 的 `assembly.name` 未绑定到实际引用，或 protocol/`contractHash` 自身验证失败时不得消费。ProjectReference 应重新生成；
无法重新生成的 NuGet/二进制输入对相关 Package 使用 full fallback。NuGet 已通过资产选择确定兼容 TFM 时，不要求
`targetFramework` 字符串与最终 `ReferencePath` 的 TFM 完全相同。

## Package

一个 Package record 包含：

| 字段 | 含义 |
| --- | --- |
| `id` | 稳定 `AtomUIRegistrationPackageId` |
| `assemblyName` | 定义 Package 的 assembly |
| `granularity` | `Package` 或 `Directory` |
| `entryMethods` | 从 `[ControlPackageRegistrationEntry]` 方法符号派生的 metadata names |
| `fullFragment` | full registrar 的 type/method identity |
| `sharedFragment` | 可选 PackageShared fragment identity |
| `units` | 当前 Package 的 Unit records |
| `unitEdges` | 当前 Package 的 direct UnitEdge records |
| `rootUnits` | Package Core 始终需要的 Unit IDs |

同一 Package ID 只能由一个 assembly 定义。重复 Package、重复 Unit、跨 Package UnitEdge 或无对应 Unit 的 ControlMap 均为协议错误。

## Unit

一个 Unit record 包含：

| 字段 | 含义 |
| --- | --- |
| `id` | Package 内稳定 Unit ID |
| `fragmentType` | leaf fragment CLR metadata name |
| `fragmentMethod` | leaf fragment static method name |
| `orderKey` | Package generator 产生的稳定注册顺序 |
| `controls` | 当前 Unit 拥有的 public Control metadata names |

Fragment method 必须可由入口应用 Compilation 解析，并具有生成 ABI 要求的静态签名。Fragment 不得调用其他 Unit、
`AddDependencies` 或 `TryEnterUnit`。

## UnitEdge

一个 UnitEdge record 包含：

| 字段 | 含义 |
| --- | --- |
| `sourceUnitId` | 依赖方 Unit |
| `targetUnitId` | 被依赖 Unit |
| `evidenceKind` | `CSharpType`、`CSharpCall`、`AxamlType` 或 `PackageCore` |

UnitEdge 只表示同 Package 直接依赖。Sidecar 不存储传递闭包；Application Plan Generator 在编译期计算 closure 和 SCC。
重复 edge 必须去重，自环可以保留或规范化删除，但 producer/consumer 必须使用一致规则。

## Usage

Usage record 的 kind 只允许：

| Kind | Identity |
| --- | --- |
| `Entry` | Package registration entry method identity |
| `Control` | CLR Control metadata name |
| `UnitRoot` | 完整 Unit ID |
| `PackageRoot` | Package ID |

Usage 可以带相对 source identity、line 和 column，用于 linked build 诊断。发布 Sidecar 不得包含开发机绝对路径。

缺失 Sidecar 不能解释为没有 Usage。无法恢复精确 Usage 时，对可确定的相关 Package执行 full fallback；无法确定 Package 时，
对应用已显式调用的 AtomUI Packages 执行 fallback。

## Fallback

Fallback record 包含：

| 字段 | 含义 |
| --- | --- |
| `packageId` | 需要 full registrar 的 Package；无法唯一确定时为空 |
| `reason` | 稳定 reason code |
| `source` | 可选相对 source/assembly identity |
| `line` / `column` | 可选位置 |

至少支持以下 reason：

- `UnresolvedOwner`
- `DynamicInvocation`
- `ReflectionType`
- `UnresolvedAxaml`
- `LooseAxaml`
- `GeneratedOutputUnverified`
- `MissingSidecar`
- `StaleSidecar`
- `AnalysisBudgetExceeded`
- `ExtractedManifest`
- `ExtractedConsumerAssembly`

Fallback 只能扩大保留范围。Consumer 不得忽略未知必需 reason，也不得把 fallback 降级为 Exact。以下三类有专门语义：

- `DynamicInvocation`：提示性记录，Consumer 不扩大保留范围，只报告 `ATOMUILINK010` 警告，由显式
  `AtomUIRegistrationUnitRoot` / `AtomUIPackageRoot` 覆盖。
- `ExtractedManifest`：consumer 从普通构建的 ProjectReference assembly metadata 提取的 Sidecar 标记。
  Package/Unit 清单完整，但 C# UnitEdge 只由 linked 库构建计算，所以 Consumer 必须对相关 Package 扩大为
  full fallback，且不产生任何诊断。要获得完整裁剪，让引用库作为当前 linked publish 图中的 ProjectReference 重新构建并产出
  companion Sidecar；AtomUI targets 会自动传播内部 linked context。
- `ExtractedConsumerAssembly`：consumer 从普通预编译 DLL 恢复的证据。Build Task 只扫描直接引用已知 control package assembly
  的候选；每个相关 Package 产生 `PackageRoot`，IL 中直接命中已声明 entry method 才产生 `Entry`。Consumer 必须 full fallback
  且不产生动态代码诊断；没有 Entry 时由 `ATOMUILINK008` 阻止构建。

## 确定性

Canonical writer 必须：

1. 使用固定字段顺序。
2. 按 ordinal Package ID、Unit ID、edge tuple、usage tuple 和 fallback tuple 排序。
3. 使用统一路径分隔符和相对路径。
4. 对重复 records 去重。
5. 在 canonical 内容上计算 `contractHash`。
6. 内容未变化时不重写文件或更新时间戳。

同一输入、SDK、Generator 和目标框架连续生成的 Sidecar 必须 byte-for-byte 相同。

## 验证和错误

以下情况是构建 Error：

- 未知 `protocolMajor`。
- 缺少必需字段或字段类型错误。
- Package/Unit/Control ownership 冲突。
- UnitEdge 跨 Package 或引用不存在 Unit。
- Fragment symbol 不存在或签名不兼容。
- Sidecar 与 ProjectReference assembly identity/hash 不一致且无法重建。
- 同一 `assembly.name` 出现不同 `contractHash` 的 Sidecar candidates。

以下情况产生 Package fallback Warning；strict 验证可以提升为 Error：

- 动态、反射或 Loose AXAML 无法解析。
- 编译输出 direct-evidence 无法由 Sidecar 解释。
- NuGet/未知二进制缺少 Sidecar。
- 确定性分析预算超限。

运行时不得读取、验证或恢复 Sidecar 错误。

## 资产验证

NuGet 和真实 publish 测试必须确认：

- Package 中 Sidecar 与 consumer target 存在且路径正确。
- linked build 可以通过 AdditionalFiles 读取 Sidecar。
- Package Sidecar 已声明的程序集不会再生成 metadata extraction candidate。
- 同身份同 hash 的重复传递只产生一个 canonical `AdditionalFiles` 输入；同身份不同 hash 的候选构建失败并报告来源。
- 普通 Debug/Release 不把 Sidecar加入 compiler input。
- Sidecar 不出现在 `lib/`、runtime assets、应用输出、`.app` bundle 或 publish 目录。
- Generator、Build Tasks、PDB 和分析缓存同样不进入运行时产物。
