# AtomUI AOT 编程规范

这份文档给日常写 AtomUI 代码的人用。它不是 AOT 改造记录，而是以后新增控件、主题、图标、语言资源、Gallery 示例和发布配置时要遵守的规则。linked publish 的系统架构、模式矩阵、Registration Unit 和安全 fallback 由
[AOT 与裁剪架构](../../architecture/foundations/aot-and-trimming.md)统一定义，Sidecar 和静态计划由
[AOT Linked Registration Pipeline](../../architecture/foundations/aot-linked-registration-pipeline.md)定义，本文不复制其长期契约。

目标很简单：

- `src/` 里的库项目在 AOT、trim、single-file analyzer 下不产生项目自身 warning。
- Gallery 需要 NativeAOT 发布时，可以通过 analyzer，也可以完成真实 publish。
- AOT 改造不能改变原来的控件行为、绑定语义、异常语义、资源释放边界和关键性能路径。

## 先看这几条

日常开发先记住这 7 条，绝大多数 AOT 问题都能在写代码时避开。

1. 新增功能和修复 bug 时，AOT 兼容是第一设计约束。同一需求有 AOT 友好实现和运行时反射/动态发现实现时，必须选择 AOT 友好实现；能用 source generator 就不要用反射。
2. 不要在 AtomUI 内置路径里新增字符串绑定，例如 `new Binding("Name")` 或 AXAML `ReflectionBinding`。
3. 不要运行时扫描 assembly、type、field、property 来完成内置注册。能显式注册就显式注册，能生成 registry/catalog 就用 source generator。
4. 不要用 `UnconditionalSuppressMessage` 盖掉 trim/AOT warning。它只是不显示 warning，不会保留被 trim 掉的 metadata。
5. 替换 AOT 不安全代码时，先确认旧语义，再改实现。尤其是 binding mode、binding priority、初始值、异常包装、dispose 后行为。
6. 新增 subscription、binding、event handler、activation scope、cache 时，必须能说清楚在哪里释放或失效。
7. Analyzer 通过不等于 NativeAOT publish 一定成功。涉及发布配置、linker、root descriptor 时，要做真实 publish 验证。

一句话总结：AOT 改造的方向是把运行时动态发现变成编译期已知代码，而不是把 warning 压下去。

## 按场景查

| 场景 | 推荐写法 | 避免写法 | Review 重点 |
| --- | --- | --- | --- |
| 控件属性同步 | `AvaloniaProperty`、`GetObservable`、`BindUtils.RelayBind(...)` | `new Binding("Path")` | mode、priority、初始值、dispose 后行为 |
| 模板内 part 同步 | C# 里拿到 template part 后强类型绑定 | AXAML `ReflectionBinding` | template reapply 时旧 part 是否释放 |
| 内置类型注册 | source generator 生成 registry/catalog | `Assembly.GetTypes()` | generator 和生成物是否一致 |
| token converter | generator 生成静态数组 | 运行时扫描 attribute 后 `Activator.CreateInstance` | 数量、顺序、map 行为是否不变 |
| 语言资源 | generated Catalog descriptor + compiled Translation Bundle | `GetFields(...)` 枚举资源字段 | Catalog ID、占位符和回退语义是否不变 |
| 图标创建 | generated factory 或 virtual factory | 扫描 icon assembly 后反射创建 | 非法 kind 的异常包装是否不变 |
| DataGrid 动态 path | `[GenerateDataMemberAccessors]` 或手写 descriptor | 对用户模型直接 `GetProperty(path)` | sort/filter/group/AddNew 是否走 descriptor |
| 非 Visual AvaloniaObject 资源宿主 | `[GenerateScopedResourceHost]` 生成 scoped host 生命周期 | 每个对象手写 `IResourceHost` / `IThemeVariantHost` 样板代码 | owner attach/release、WeakReference、资源更新测试 |
| ReactiveUI view activation | AtomUI/Gallery 自己管理 activation scope | view-side `WhenActivated` extension 反射路径 | Loaded/Unloaded 和 VM 切换释放 |
| 发布配置 | analyzer 加真实 NativeAOT publish | 只看普通 build | linker、root、generator 项目是否被错误发布 |

## 适用范围

这份规范覆盖以下项目：

- `src/AtomUI.Core`
- `src/AtomUI.Controls.Shared`
- `src/AtomUI.Controls`
- `src/AtomUI.Desktop.Controls`
- `src/AtomUI.Desktop.Controls.DataGrid`
- `src/AtomUI.Desktop.Controls.ColorPicker`
- `src/AtomUI.Desktop.Controls.Extras`
- `src/AtomUI.Icons.*`
- `src/AtomUI.Generator`
- `controlgallery/AtomUIGallery`
- `controlgallery/AtomUIGallery.Desktop`

## Binding

### 为什么字符串 binding 有风险

`new Binding("Name")`、`ReflectionBinding` 这类写法把成员访问延迟到运行时：运行时拿到字符串，再去找属性、字段或索引器。NativeAOT 和 trimming 会删除静态代码没有证明会用到的 metadata，所以这类路径很容易出现 analyzer warning，严重时运行时才失败。

AtomUI 内置控件和 Gallery AOT 路径应尽量把绑定改成编译期可见的属性访问。

### AvaloniaObject 之间同步属性

两个 Avalonia object 之间同步属性时，优先使用 `AvaloniaProperty`：

```csharp
disposables.Add(BindUtils.RelayBind(
    source,
    SourceControl.SomeProperty,
    target,
    TargetControl.SomeProperty,
    BindingMode.OneWay,
    BindingPriority.Template));
```

`BindUtils.RelayBind(...)` 的语义要求：

- `OneWay`：从 source 的 `GetObservable(sourceProperty)` 绑定到 target。
- `TwoWay`：保留 source-to-target 和 target-to-source 的双向同步，不能把 target 初始值错误写回 source。
- `OneWayToSource`：只把 target 后续变化写回 source，不能额外制造 source-to-target 行为。
- `OneTime`：也要保持旧 binding 的写入 priority 和释放语义，不能简单写成 `SetCurrentValue(...) + Disposable.Empty`，除非旧语义本来就是一次性写入且无需恢复。

不要再新增字符串 path overload。`BindUtils.RelayBind(object, string, ...)` 这类 API 应保持为兼容边界或编译期错误入口。

### 非 AvaloniaObject 数据源

对 `INotifyPropertyChanged` 类型，可以用属性名过滤通知，但取值必须走强类型 getter：

```csharp
disposables.Add(BindUtils.RelayBind(
    viewModel,
    nameof(MyViewModel.Options),
    static vm => vm.Options,
    optionsControl,
    SomeControl.OptionsProperty));
```

这里的 `nameof(...)` 只用于判断是哪一个属性变了，不允许再用反射按名字读取属性值。

### Template part binding

模板里不要新增 `ReflectionBinding`。template part 之间要同步时，在控件类里拿到 part 后绑定：

```csharp
_templateBindingDisposables?.Dispose();
_templateBindingDisposables = new CompositeDisposable();

if (partA is not null && partB is not null)
{
    partB.Bind(
             TargetProperty,
             partA.GetObservable(SourceProperty),
             BindingPriority.LocalValue)
         .DisposeWith(_templateBindingDisposables);
}
```

这里最容易出错的是生命周期：

- `OnApplyTemplate` 开始时先释放旧的 disposable。
- template reapply 后，旧 part 不能继续被 observable、event handler 或 binding 持有。
- 删除旧 binding 前，要确认旧 binding 是否会跟随动态 effective value 变化。不能因为初始默认值一样就删掉。

### Visual ancestor binding

需要找 visual ancestor 时，使用 `BindUtils.BindVisualAncestor(...)`：

```csharp
_relayBindingDisposables.Add(BindUtils.BindVisualAncestor(
    this,
    target,
    TargetControl.OpenOnProperty,
    typeof(TopLevel),
    priority: BindingPriority.Template));
```

注意：

- `ancestorLevel` 从 1 开始。
- 返回的 disposable 必须在 detach 或 template reapply 时释放。
- 不要用一次性的 `TopLevel.GetTopLevel(this)` 替代动态 ancestor binding，除非旧语义就是只查一次。

## Source Generator

### 什么时候该用 SG

如果代码需要做这些事，优先考虑 source generator：

- 扫描类型并注册。
- 根据 attribute 生成 registry。
- 根据 enum 创建具体类。
- 把资源字段写入 dictionary。
- 根据数据模型生成属性 accessor。
- 为 closed generic 或具体类型生成 factory。
- 为 owner-managed 非 Visual `AvaloniaObject` 生成 scoped `IResourceHost` / `IThemeVariantHost` 生命周期样板代码。

SG 的价值不是“把反射挪个地方”，而是让运行时代码变成普通的强类型 C#。这样 trimmer 能看见类型、构造函数和成员，NativeAOT 也不需要动态代码生成。

非 Visual `AvaloniaObject` 资源宿主类需求统一遵循 [Scoped Resource Host 开发规范](scoped-resource-host.md)。不要在每个描述对象中复制手写资源宿主代码；业务属性保留在主文件，资源宿主生命周期由 generator 生成，owner 控件只负责 attach/release。

### Generator 项目边界

`AtomUI.Generator` 是 Roslyn analyzer 项目，不是运行时依赖。引用方式保持：

```xml
<ProjectReference Include="../AtomUI.Generator/AtomUI.Generator.csproj"
                  OutputItemType="Analyzer"
                  ReferenceOutputAssembly="false"
                  PrivateAssets="all" />
```

生成文件可以输出到 `GeneratedFiles/`，但项目要继续排除重复编译；这些编译产物默认不跟踪：

```xml
<EmitCompilerGeneratedFiles>true</EmitCompilerGeneratedFiles>
<CompilerGeneratedFilesOutputPath>GeneratedFiles</CompilerGeneratedFilesOutputPath>
<Compile Remove="$(CompilerGeneratedFilesOutputPath)/**/*.cs"/>
```

`PublishAot=true` 是 MSBuild global property，会沿着 `ProjectReference` 传播。generator 是 `netstandard2.0` analyzer 项目，不应该参与 NativeAOT 发布，所以 generator 项目要把这些属性隔离在本项目内：

```xml
TreatAsLocalProperty="IsAotCompatible;EnableAotAnalyzer;EnableTrimAnalyzer;EnableSingleFileAnalyzer;PublishAot;PublishTrimmed;PublishSingleFile;RunAOTCompilation;SelfContained;RuntimeIdentifier"
```

并且在 generator 项目里显式关闭运行时发布属性。

第一方用户不需要为了 AOT 或 trimming 额外引用 `AtomUI.Generator`。声明 `AtomUIRegistrationPackageId` 的产品包会在
NuGet 中内嵌同版本 Generator、Build Tasks 和 buildTransitive assets。现有应用保留显式 Generator PackageReference
仍然受支持，但最终 `@(Analyzer)` 中只能有一份 `AtomUI.Generator.dll`。

维护这条打包链时遵守以下约束：

- Generator 和 Build Tasks 只能进入 NuGet 的编译期目录，不能进入 `lib/`、普通输出或 publish 目录。
- 多个产品包的入口必须幂等；Analyzer 去重依据 `ResolveReferences` 后的最终编译器输入。
- MSBuild 项目求值阶段的 `Import`、`ItemGroup` 或 item Condition 不得引用 item list；需要检查 `@(Analyzer)` 时放入
  `BeforeTargets="CoreCompile"` 的 Target。
- Release 打包必须先用相同 `AtomUIVersion` 构建 Generator/Build Tasks；修改版本后不能用 `--no-build` 复用旧输出。
- 验证至少覆盖只引用一个产品包、多个产品包并存、保留显式 Generator 引用、普通非裁剪构建和真实 NativeAOT publish。

### 生成物必须稳定

改 generator 时要同时看两层：

- writer 代码是不是符合预期。
- `GeneratedFiles/AtomUI.Generator/**` 里的生成物是不是能由当前 writer 稳定复现。

不能只手改生成物，也不能只改 generator 却不检查生成物 diff。之前 review 里已经出现过 generator writer 和提交的生成物不一致，这类问题会让后续维护很难判断真实来源。

包级生成代码统一位于 `AtomUI.Generated.<AssemblyOwner>`。`AssemblyOwner` 必须是由程序集名生成的单一 PascalCase
标识符，点号、连字符、下划线和其他非字母数字字符只作为单词边界，不进入最终标识符。例如：

```text
AtomUI.Controls                  -> AtomUI.Generated.AtomUIControls
AtomUI.Desktop.Controls         -> AtomUI.Generated.AtomUIDesktopControls
AtomUI.Desktop.Controls.DataGrid -> AtomUI.Generated.AtomUIDesktopControlsDataGrid
```

Source Generator、Localization writer、Linked Registration metadata 和 AXAML Theme wrapper Build Task 必须复用同一
命名 helper，不得各自维护 sanitizer。手写代码引用生成入口时必须引用当前项目自己的 owner namespace；禁止依赖
`InternalsVisibleTo` 从其他 AtomUI 包调用同名 `Generated*` 类型，因为这种错误可能正常编译却注册错误 Package。

### Linked registration

`PublishTrimmed=true`、`PublishAot=true` 和 WebAssembly `RunAOTCompilation=true` 使用同一套生成式 Registration Unit
计划。Control descriptor、Own Token schema、内部控件和控件族专属 Theme Asset factory 必须能聚合为完整 Unit；不得新增
全包静态数组、全资产 `switch` 或“构造全集后过滤”的 linked 路径。

linked analysis 必须位于独立 Analyzer assembly。普通 Debug 和未启用 AOT/Trim 的 Release 不得把该 Analyzer 传给 `csc`，
也不得运行 AXAML usage、Sidecar 或 Application Plan target。仅在 Generator callback 中快速 return 不算满足零成本要求。

Control Package 默认使用 Package 粒度：

```xml
<AtomUIRegistrationGranularity>Package</AtomUIRegistrationGranularity>
```

该属性可以省略，省略即为 `Package`。整个包生成一个安全 Unit，普通第三方包、DataGrid、ColorPicker、Extras 和
GalleryBase 都使用该模式。包作者不声明 `AtomUIRegistrationUnit`，也不维护 Unit dependency。

只有包含大量独立控件族、并且已经建立真实 linked publish 和体积回归验证的包才允许显式设置：

```xml
<AtomUIRegistrationGranularity>Directory</AtomUIRegistrationGranularity>
```

当前第一方只有 `AtomUI.Desktop.Controls` 使用 Directory 模式。启用 Directory 后，控件族目录才成为 Unit 边界；
Presenter、Cell、View、Semantic Part 和基础主题仍跟随所属控件族，不能继续按内部目录细分。

Package 注册入口的正式契约见
[AOT 与裁剪架构](../../architecture/foundations/aot-and-trimming.md#81-package-注册入口声明)。新增或迁移 Control Package 的
真实 `UseXxxControls()` 方法必须使用
`[ControlPackageRegistrationEntry]` 声明入口。Generator 从 `IMethodSymbol` 派生 manifest method identity，并在 Package
自身编译阶段验证扩展方法签名、可见性和重载集合。禁止在 `.csproj`、props、targets 或 NuGet metadata 中手写入口类型名
和方法名，也禁止使用方法命名约定或方法体扫描猜测入口。

`AtomUIRegistrationPackageId` 只表示 Package identity 和产品包 build asset 注入边界。它不能携带注册方法信息，也不能作为
是否存在源码入口的替代判断。没有 `[ControlPackageRegistrationEntry]` 的程序集不得输出 linked Package、Unit、ControlMap
或 full fragment metadata，但仍可以生成普通非裁剪路径使用的 full registration helper。

Language Catalog、内置 Translation Bundle、Dialog/Tooltip/Motion/Responsive 初始化、Global Token、Theme Algorithm、
Provider 和平台 selector 属于 Package Core，不为它们创建细粒度 fragment。只有必须在未选择 Control Unit 时仍随入口
加载，或在 Directory 模式下确实跨多个 Unit 的共享主题资源，才通过 `AtomUIPackageSharedTheme` 显式声明；owner 解析
失败不得自动归类为共享资源。Package 模式下的普通 Control Theme 不需要该 metadata。

普通 AXAML/C# 使用由 Generator 自动发现。`AtomUIRegistrationUnitRoot` 和 `AtomUIPackageRoot` 只用于类型字符串、Loose
AXAML、动态插件等编译期无法确定的场景，不能成为普通 Control 接入步骤。无法可靠确定 Unit 时应在编译期只把对应
Package 扩大为 full fallback 并给出诊断，不能依赖运行时反射或 late registration 修补。

Directory 模式的 Package Theme 如果直接实例化另一个 AtomUI Control，该元素必须能通过 `using:`、
`clr-namespace:` 或当前程序集 `XmlnsDefinition` 精确解析，Generator 会把同 Package Unit 的直接依赖写入 Sidecar UnitEdge。
C# 中可证明的跨 Unit Control 使用也必须形成直接 UnitEdge。不要依赖短类型名猜测，也不要把模板元素依赖写进 Theme Asset descriptor 的
referenced identities；后者会改变普通非裁剪 schema 和 fingerprint。无法证明 owner 或依赖时必须 full fallback。

Unit fragment 必须是叶子，只注册本 Unit 的 descriptor、Theme asset 和 resource factory。禁止生成 `AddDependencies`、调用
其他 Unit 或调用 `TryEnterUnit`。应用 Generator 在编译期对 UnitEdge 计算 closure/SCC，并让每个 fragment 最多出现一次。

依赖分析使用候选驱动 Incremental API。禁止对所有 SyntaxTree 执行 `DescendantNodes()`，禁止从 invocation 递归进入 callee
body，也禁止为不同 owner Unit 重复扫描相同方法、属性或字段。分析必须有确定性结构预算；超限时当前 Package full fallback，
不能使用墙钟超时产生非确定输出。

ControlMap 是 CLR Control ownership，不是 descriptor 清单。定义程序集里的 public、非泛型 Control 即使没有 Theme
descriptor，也要归入 Registration Unit 并拥有 ControlMap；只有原本可主题化的 Control 才能进入 `builder.AddControl`。
引用程序集不得重复输出 ControlMap。纯基础设施程序集没有 `[ControlPackageRegistrationEntry]` 时，不应生成
Package、Unit、ControlMap 或 full fragment metadata。

`AtomUI.Controls` Common 层始终由 Desktop 完整注册，不是独立 linked Package。不要为 `UseCommonControls()` 添加入口
Attribute，不要为了让 Common 参与应用计划而复制 `UseDesktopControls()` 的方法 identity，也不要引入跨 Package Unit 闭包。

类库只在被 linked 应用作为 ProjectReference 构建或执行 NuGet Pack 时生成 Usage Sidecar。普通 Debug/Release 必须跳过 usage
分析和 Sidecar 生成。应用在项目文件内设置 `PublishAot`、`PublishTrimmed` 或 `RunAOTCompilation` 后，AtomUI targets 自动把
`AtomUILinkedPublish=true`、`AtomUIRegistrationPlanOwner=false` 沿直接和传递 ProjectReference 递归传播；不要要求用户补传内部
属性，也不要把 `PublishAot`、RID、SelfContained 传播给类库。analyzer 类型 ProjectReference 必须排除。

如果 ProjectReference companion 仍缺失，consumer 从 package producer assembly 的 metadata 提取清单；结果带
`ExtractedManifest`，相关 Package full fallback 且不产生诊断。对于不能随图重编译的普通预编译 DLL，只扫描直接引用已知
control package assembly 的候选，通过 IL `call`/`callvirt`/`ldftn`/`ldvirtftn` 恢复 entry，并为相关 Package 产生
`PackageRoot` 与 `ExtractedConsumerAssembly`。恢复路径始终 full fallback；只有 PackageRoot 没有 Entry 时必须用
`ATOMUILINK008` 报告消费 DLL。缺失或无法验证的 Sidecar 不能解释为没有 usage。
`ATOMUILINK002`、`ATOMUILINK007` 和 `ATOMUILINK010` 只在 linked publish 或显式 `AtomUIRegistrationStrict=true` 验证中显示；strict 模式用于
CI 把自动 full fallback 或未覆盖的动态创建提升为 error。`ATOMUILINK010` 不触发 full fallback，只提示用显式 root 覆盖。

### Sidecar 来源和重复防护

维护 linked-registration 构建资产时，必须遵守以下不变量：

- 一个程序集在一次 linked build 中只能有一个生效的 Sidecar。
- ProjectReference companion 与 NuGet package 的顺序只用于选择同 hash 等价候选；metadata extraction 只在正式 Sidecar 缺失时使用。
- 不能用“DLL 旁边没有 Sidecar”证明“构建中没有 Sidecar”；NuGet 正式 Sidecar 通常位于 `buildTransitive`。
- 收集阶段必须先解析 `assembly.name` 和 `contractHash`，再按程序集身份去重，最后才注入 `AdditionalFiles`。
- 同身份同 hash 可以折叠为一份；同身份不同 hash 必须失败并报告来源，不能随机保留第一份。
- 已有正式 Package/companion Sidecar 的程序集禁止再次生成 `ExtractedManifest`；普通 ProjectReference 缺失 companion 时仍须保留
  extraction fallback。
- `ExtractedConsumerAssembly` 只用于没有正式 Sidecar 的预编译消费 DLL；它必须同时经过最终 canonical resolution，不能覆盖或
  合并掉正式 consumer Sidecar。

任何新增 Sidecar、consumer target、Pack asset 或 extraction 逻辑，都必须同时验证 NuGet、ProjectReference、混合引用和重复传递
包场景。不要在 Generator 中吞掉重复声明，也不要通过关闭 extraction、修改文件名或 suppress `ATOMUILINK005` 掩盖来源冲突。

修改 Generator ABI、Sidecar schema、feature switch 或 Public fragment entry point 时，按 Public API 和协议 review，并运行
普通构建零 linked-analysis、trimmed JIT、NativeAOT 和非裁剪兼容验证。系统契约见
[AOT Linked Registration Pipeline](../../architecture/foundations/aot-linked-registration-pipeline.md)。

只有 Directory 模式的内部 resource-only Theme 无法按目录推导到正确 Unit 时，才使用 `AtomUIRegistrationUnit`
metadata 明确归属，不要把它升级为 `AtomUIPackageSharedTheme`。Package 模式不得添加这类 ownership 修补。抽象 typed
theme 没有 generated resource wrapper 时，不得生成虚假的 wrapper 调用；应由同 Unit 的具体资源静态保留。

### 第三方 Control Package 检查

普通第三方 Control Package 的 AOT 接入必须保持简单：

1. 声明稳定的 `AtomUIRegistrationPackageId`。
2. 引用兼容版本的 AtomUI 产品包，复用其内嵌的同版本 Generator 和构建资产；普通包不重复添加 Generator 引用。
3. 在真实 public `UseXxxControls()` 扩展方法上添加 `[ControlPackageRegistrationEntry]`。
4. 保持 full/generated 注册分支，以及 Provider、Localization 和 initializer 的顺序。
5. 使用默认 Package 粒度，不写 Unit ownership、Unit dependency 或 linker XML。
6. 至少验证 ordinary 与 generated registration 行为快照；发布 AOT 兼容声明前跑真实 trimmed JIT 和 NativeAOT。

操作示例见
[第三方 AtomUI Control Package 指南](../../guides/theming/third-party-control-packages.md)。
粒度和资源归属的正式规则见
[AOT Registration Unit 粒度](../../architecture/foundations/aot-registration-unit-granularity.md)。

### AOT/Trim 注册命名

AOT/Trim 注册是发布基础设施，不是通用运行时 feature。`AtomUI.Core` 中跨程序集使用的隐藏 ABI 统一位于
`AtomUI.Registration` 命名空间，并采用以下命名：

```text
AotTrimRegistration
AotTrimControlPackageRegistrationBuilder
AotTrimRegistrationPlan
AotTrimRegistrationPlanRegistry
```

`AotTrimRegistration.IsEnabled` 只读取发布期标记；它不负责执行裁剪、发现控件或安装注册计划。对应的全局
AppContext key 为 `AtomUI.AotTrimRegistration.Enabled`。类型名不重复 `AtomUI`，因为命名空间已经提供产品边界；
AppContext key 则必须保留 `AtomUI` 前缀，因为它是进程级字符串协议。

`GeneratedApplicationRegistrationPlan`、`Generated*UnitFragment` 等 `Generated*` 名称仅限 Generator 生成的内部输出，
不要把它们作为运行时 ABI 的通用命名。Generator 内部协议类型可以使用 `LinkedRegistration*`，Build Task 使用
`CollectAxamlUsageTask`、`WriteLinkedRegistrationSidecarTask`、`ValidateApplicationRegistrationPlanTask` 这类动作导向名称。

## Theme / Token

### Control 与 Token 注册

运行时不要扫描 assembly 查找可主题化 Control 或 Control Token。应由 generator 生成
`ControlTokenDescriptorPool`，为每个对外可主题化 Control 返回完整 descriptor；没有 Own Token 的 Control 也必须
拥有 identity 和 descriptor：

```csharp
descriptors.Add(MyControlTokenDescriptor.Instance);
```

descriptor 必须直接提供以下静态已知信息：

- exact Control CLR type、`ControlTokenIdentity` 和 registry slot。
- 无参数 `[ControlDesignToken]` 标记的可选 Own Token 类型；Attribute 不携带 Control 类型或 identity。
- 可选的 Own Token builder 直接构造委托。
- Own Token name、value type、stage 和 slot。
- 强类型 parse、set、get 和 resource projection 委托。
- `OwnTokens` schema；Control 的可配置 Global Token 集合始终是完整 Global Token schema，不生成消费白名单。
- ControlTheme asset owner、引用的 Control identities、Semantic Part 契约和包级注册信息。

Semantic Part descriptor 必须由 Control 声明和构建输入静态生成。运行时不得扫描 AXAML、ControlTheme、
`Classes` 或 VisualTree 来发现公共 Part，也不得使用 `Dictionary<string, Style>`、反射 Property 查找或动态 Theme
factory 合并 Part 样式。`.semantic-*` marker 只参与 Avalonia 原生 Selector；Gallery 的 VisualTree 高亮属于开发
工具路径，不能进入 Control 运行时。

这里有三个关键点：

- Builder 必须原样传递 descriptor，不能丢弃 identity 后退化为 `Type` 注册。
- 内置正常路径不调用 `Activator.CreateInstance`、`Type.GetProperties` 或 `PropertyInfo.GetValue/SetValue`。
- 第三方 Control 包必须使用 AtomUI generator，并只通过一个真实的包级入口调用生成 registration helper，注册 Control、
  可选 Own Token 和主题资产；不提供手写 descriptor、手工 manifest 或反射 fallback 旁路。
- Own Token 可以放在包内正常源码位置并使用 `[ControlDesignToken]` 标记；禁止泛型 Control 参数和手写 ID。

### Token value converter 注册

不要在运行时通过 `Assembly.GetTypes()`、`IsDefined(...)`、`Activator.CreateInstance(...)` 扫描 converter。应由 `TokenValueConverterRegistrationGenerator` 生成静态注册：

```csharp
ITokenValueConverter[] valueConverters =
[
    new StringTokenValueConverter(),
    new IntegerTokenValueConverter(),
    new DoubleTokenValueConverter()
];
```

改这里时要确认：

- converter 实例数量和旧逻辑一致。
- `TargetType()` 到 converter 的 map 行为不变。
- 新增 converter 后能在 `TokenValueConverterRegistry.g.cs` 里看到生成结果。

### Resource key cache

Resource key cache 应该基于显式 token metadata。不要在资源生成或 theme 切换的 hot path 里反射查找 token property。

cache 要满足：

- key 稳定。
- theme/token 更新时失效边界清楚。
- 不引入无上限增长的缓存。

## Language

### Catalog 与翻译表走生成代码

所有应用、控件和类库使用 `[LanguageCatalog]` enum 与 XLIFF 2.1。`LocalizationGenerator` 在编译期直接生成：

- enum 数字 ID 到 slot 的静态映射。
- `LanguageCatalogDescriptor` 和编译后 `TranslationBundleDescriptor`。
- 强类型 `XxxLangResourceExtension`。
- `GeneratedLanguageModuleRegistration` 和最终应用 bootstrap。

正常路径禁止通过 `Assembly.GetTypes()`、`GetFields()`、`Enum.GetNames()` 或 Attribute 反射发现 Catalog，也禁止
运行时解析 XLIFF。模块主包的 `en-US` 与静态语言包目标 XLIFF 只作为 `AdditionalFiles` 进入最终应用编译；运行时
只保留不可变 Snapshot 和字符串表。

Review 时要看：

- Catalog enum 是否公开、使用稳定显式正整数 ID，且不含别名或 `[Flags]`。
- 所有 Catalog 是否具有完整 `en-US`，目标语言的 unit/占位符契约是否一致。
- 类库包级入口是否直接调用生成的模块注册，不扫描程序集。
- 应用 bootstrap 是否只直接注册静态语言包和 Override Bundle。
- 发布目录是否没有 XLIFF、Build Tasks 或 Generator 程序集。

## Icon

### 图标创建走 generated factory

图标 provider 不要运行时扫描 icon assembly，也不要用 `Activator.CreateInstance(iconType)` 创建图标。生成器负责生成 `GetIconType(kind)` 和 `CreateIcon(kind)`。

`GetIcon(kind)` 的异常包装语义要保留：

```csharp
try
{
    return CreateIcon(kind);
}
catch (Exception ex)
{
    throw new InvalidOperationException($"Create icon {kind} failed", ex);
}
```

这不是细节。非法 kind、构造失败、内部异常的外层异常类型和 message 都属于对外行为。

### Gallery icon catalog

Gallery 展示图标时使用 generated catalog：

```csharp
AntDesignIconCatalog.GetIcons()
```

catalog entry 应包含：

- `Name`
- `ThemeType`
- `Kind`
- `IconType`
- direct `Func<Icon>` creator

Gallery 不要运行时扫描 icon assembly。

### 同类型 icon clone

需要创建同类型 icon 时，不要通过反射构造。使用虚方法：

```csharp
public override Icon CreateInstance()
{
    return new SameIconType();
}
```

要求：

- 每个生成图标 override 后返回同一具体类型。
- base `Icon.CreateInstance()` 默认失败，避免静默创建错误类型。
- clone 后只复制旧逻辑复制过的属性，不能额外覆盖 brush、animation、stroke 等状态。

## DataGrid / Collection 动态数据

### 问题本质

DataGrid、collection sort/filter/group、自动生成列这些场景经常拿到用户数据模型和字符串 property path。这里的动态性是功能需求，不能简单把 warning suppress 掉。

正确方向是：对我们能控制的模型生成 accessor；对用户模型提供显式 descriptor；最后才保留带 warning 的兼容 fallback。

### 推荐写法

可控制的数据模型加 `[GenerateDataMemberAccessors]`：

```csharp
[GenerateDataMemberAccessors]
public partial class PersonRow
{
    public string? Name { get; set; }
    public int Age { get; set; }
}
```

如果集合以接口或基类作为 item type 暴露，并且排序、过滤或分组 path 来自这个接口/基类，也要在对应接口或基类上生成 accessor；不要依赖运行时从首个 item 反推具体类型。

不可加 attribute 的模型，显式传入 `IDataMemberAccessorDescriptor`：

```csharp
var descriptor = new DataMemberAccessorDescriptor<PersonRow>(
    static () => new PersonRow(),
    new IDataMemberAccessor[]
    {
        new DataMemberAccessor<PersonRow, string?>("Name", static row => row.Name),
        new DataMemberAccessor<PersonRow, int>("Age", static row => row.Age)
    });
```

相关逻辑优先从 descriptor 取信息：

- sort
- filter
- group
- AddNew
- auto-generate columns
- data type inference
- read-only metadata

如果 descriptor 不存在，可以进入 RUC fallback，但调用点必须显式看到风险。`RequiresUnreferencedCode` 不会保留成员，它只会把风险传递给调用方，让 analyzer 在 AOT/trim 场景下报警。

所以结论是：AtomUI 内置模型要走生成 accessor；用户如果要 NativeAOT 稳定发布，就要提供 generated 或手写 descriptor。

### 编译期诊断

DataGrid、List、collection view 等使用字符串 path 做排序、过滤、分组、自动列或数据成员读取时，必须优先让问题在编译期暴露，而不是等到 NativeAOT 运行时才失败。

可静态判断的场景必须提供 analyzer warning。诊断 ID、ID 命名、severity、编码组织和测试规则统一维护在 [compiler-diagnostics-guidelines.md](compiler-diagnostics-guidelines.md)。

## Reflection helper

反射 helper 只能存在于明确边界：

- public compatibility API。
- 用户动态模型 fallback。
- 平台或框架能力探测。

保留反射时要写清楚风险：

- 需要成员 metadata 的 `Type` 参数或返回值，要用 `DynamicallyAccessedMembers` 标注。
- 无法证明 trim 安全的 API，要用 `RequiresUnreferencedCode` 标注。
- helper 内不要动态构造泛型类型。优先扫描对象已经实现的 closed interface。
- 不要用 `UnconditionalSuppressMessage` 让调用方误以为代码安全。

可以这样理解：

- DAM 用来告诉 trimmer：“这个 Type 值流到这里时，需要保留哪些成员。”
- RUC 用来告诉调用者：“这个 API 在 trim/AOT 下不保证安全。”
- suppression 只是不显示 warning，不解决问题。

## ReactiveUI

### AtomUI runtime controls

AtomUI runtime 控件不能依赖 ReactiveUI view-side reflection activation path。`ReactiveWindow<TViewModel>` 这类基类要自己完成关键行为：

- 实现 `IViewFor<TViewModel>`。
- 维护 `ViewModel` 和 `DataContext` 同步。
- `Loaded` 时激活当前 `IActivatableViewModel`。
- `Unloaded` 或切换 `ViewModel` 时释放旧 activation disposable。
- 处理 reentrancy，避免 unload 过程中误激活新 VM。

构造函数里空的 `this.WhenActivated(...)` 没有业务价值，应删除。非空 activation block 要迁移到 AOT 友好的本地 activation API。

### Gallery controls

Gallery AOT 路径使用 `GalleryReactiveUserControl<TViewModel>` 替代 `ReactiveUserControl<TViewModel>`。

它要保留 ReactiveUI 的核心使用体验：

- `IViewFor<TViewModel>`。
- `ViewModel` 和 `DataContext` 双向同步。
- Loaded/Unloaded 对应 view activation scope。
- `IActivatableViewModel.Activator.Activate()`。
- `WhenActivated(Action<CompositeDisposable>)` 的注册、激活、卸载释放语义。

也就是说，我们不是放弃 ReactiveUI，而是避开它 view-side extension API 中对 AOT 不友好的表达式/反射路径。

### Gallery binding / command

Gallery code-behind 不要新增 ReactiveUI expression binding：

```csharp
this.OneWayBind(...);
this.BindCommand(...);
this.WhenAnyValue(...);
observable.ToProperty(...);
```

替代写法：

- `GalleryBindingUtils.OneWay(...)`
- `GalleryBindingUtils.BindCommand(...)`
- setter 内显式更新派生属性
- 显式 `BehaviorSubject<T>` 或 `IObservable<T>` 管理 command canExecute

`GalleryBindingUtils.BindCommand` dispose 时要恢复绑定前 command，避免 activation scope 结束后留下旧 command。

## Publish / trimming / NativeAOT

### 日常构建不运行发布分析器

普通 `Debug` 和非 AOT、非裁剪的 `Release` 构建必须关闭 SDK 的 trim、AOT 和 single-file analyzer，避免把发布期静态分析成本带入日常开发。Gallery Desktop 的 Release 项目只保留 `IsTrimmable=true` 和 `IsAotCompatible=true` 兼容性声明；`PublishTrimmed`、`PublishAot` 和自包含发布属性由发布脚本显式传入，不能隐式改变普通构建的 analyzer 开关。

仓库构建按以下输入自动启用对应 analyzer：

- `PublishTrimmed=true`：启用 trim analyzer。
- `PublishAot=true` 或 `RunAOTCompilation=true`：启用 trim 和 AOT analyzer。
- `PublishSingleFile=true`：启用 single-file analyzer。
- 显式传入 `EnableTrimAnalyzer`、`EnableAotAnalyzer` 或 `EnableSingleFileAnalyzer` 时，保留调用方选择，用于专项验证。

ordinary Generator 可以继续生成普通构建所需的 Theme/Token、full registrar 和 leaf Unit fragment，但不能运行应用 usage 分析。
Package/Usage Sidecar 只在 NuGet Pack 或 linked ProjectReference 构建时生成；Application Plan 只能在真实 AOT/Trim linked build
中生成。普通构建必须从 `Csc` Analyzer item、AXAML target 和 `obj` 中同时看不到 linked analysis。

### Analyzer 和真实 publish 都要跑

AOT/trim analyzer 通过，只说明静态分析没有发现项目自身 warning。它不等于 trimmed JIT 或 NativeAOT 链接和运行一定成功。涉及 Registration Unit、Package fallback、发布配置或 native 依赖时，要做对应模式的真实 publish。

Windows 11 上 Gallery Desktop 的 NativeAOT 工具链、发布命令、产物验证和排障记录见 [Windows NativeAOT 发布](../platforms/windows-native-aot-publish.md)。Linux 平台的对应手册见 [Linux NativeAOT 发布](../platforms/linux-native-aot-publish.md)。

库项目 analyzer：

```bash
dotnet build src/AtomUI.Core/AtomUI.Core.csproj -c Release --no-incremental \
  /p:IsAotCompatible=true \
  /p:EnableTrimAnalyzer=true \
  /p:EnableAotAnalyzer=true \
  /p:EnableSingleFileAnalyzer=true \
  /nr:false --nologo -v:minimal
```

Gallery analyzer：

```bash
dotnet build controlgallery/AtomUIGallery/AtomUIGallery.csproj -c Release --no-incremental /m:1 \
  /p:IsAotCompatible=true \
  /p:EnableTrimAnalyzer=true \
  /p:EnableAotAnalyzer=true \
  /p:EnableSingleFileAnalyzer=true \
  /nr:false --nologo -v:minimal
```

Gallery NativeAOT publish：

```bash
dotnet publish controlgallery/AtomUIGallery.Desktop/AtomUIGallery.Desktop.csproj \
  -c Release -r osx-arm64 -p:GalleryPublishTrimmed=true \
  -p:GalleryPublishAot=true --self-contained true \
  --nologo -v:minimal
```

Linked registration 完整回归与体积门槛：

```bash
scripts/verification/verify-aot-trim-registration.sh --full
```

脚本成功仍不能代替 Gallery Desktop 启动 smoke。Theme template 可以静态保留 CLR 类型而漏注册它的 descriptor；这类错误
只有窗口模板应用和首帧布局实际运行时才会暴露。发布后至少确认进程稳定进入主窗口，无 active theme schema、资源加载或
initializer 异常，再主动终止 smoke 进程。

### macOS NativeAOT

当前 macOS 本机发布使用：

```xml
<ItemGroup Condition="'$(PublishAot)' == 'true' and $([MSBuild]::IsOSPlatform('OSX'))">
    <LinkerArg Include="-L/opt/homebrew/lib"
               Condition="Exists('/opt/homebrew/lib/libbrotlienc.dylib')"/>
    <LinkerArg Include="-L/opt/homebrew/opt/openssl@3/lib"
               Condition="Exists('/opt/homebrew/opt/openssl@3/lib/libssl.dylib')"/>
    <LinkerArg Include="-L/usr/local/lib"
               Condition="Exists('/usr/local/lib/libbrotlienc.dylib')"/>
    <LinkerArg Include="-L/usr/local/opt/openssl@3/lib"
               Condition="Exists('/usr/local/opt/openssl@3/lib/libssl.dylib')"/>
</ItemGroup>
```

原因：

- NativeAOT 链接 `System.Net.Security.Native` 时需要 `libssl` / `libcrypto`。
- NativeAOT 链接 `System.IO.Compression.Native` 时还需要 Brotli 原生库。
- Apple Silicon Homebrew 的通用库目录通常是 `/opt/homebrew/lib`，`openssl@3` keg-only 库位于
  `/opt/homebrew/opt/openssl@3/lib`；Intel Homebrew 对应 `/usr/local` 路径。默认 linker 搜索路径可能找不到这些库。
- 仓库内 macOS 验证统一复用 `build/MacOSHomebrewNativeAot.targets`，不要在 Gallery 或 fixture 中重复硬编码路径。
- 该文件只补充 Homebrew OpenSSL/Brotli linker 搜索路径，不进入 NuGet，也不是 AtomUI 的通用 NativeAOT 配置。
  Windows 和 Linux 使用共享 AOT 配置及各自平台工具链，不需要空的对称 targets 文件。

注意：

- 默认不要添加 `-ld_classic`。删除粗粒度 root 后，当前 NativeAOT 产物已经可以用默认 Apple linker 完成链接。
- 如果某个 Xcode/NativeAOT 组合再次触发 `too many large addends`，先验证是否是环境问题，再作为本机或 CI 发布参数处理，不要默认固化到项目文件里。
- 如果 CI 或开发机不是 Homebrew ARM64 路径，需要在发布脚本里提供正确的 linker search path。
- NativeAOT publish 输出的是裸 Mach-O 可执行文件。macOS 图形应用要双击或通过 LaunchServices 启动，需要 `.app` bundle。

### Roots.xml

`Roots.xml` 不能保留解析不到的 assembly：

```xml
<assembly fullname="Some.Old.Assembly" preserve="All"/>
```

无效 root 只会产生 IL2007 warning，不会保留任何东西。删除 stale root 不改变运行时行为。

新增 root 前要说明：

- 为什么不能通过静态引用或 generator 保留。
- preserve 范围为什么不能更小。
- 是否会明显扩大 NativeAOT 体积。

AtomUI linked registration 的 `AtomUIRegistrationUnitRoot` 和 `AtomUIPackageRoot` 是生成器语义 root，不是 linker XML
的替代写法。它们只用于应用动态输入，并由 Application Registration Plan 展开为强类型 Unit 调用或 Package full
fallback；不要把包级 `preserve="All"` 搬进 `Roots.xml` 来绕过 Manifest、Unit 归属或 fallback 缺陷。

### Browser WebAssembly 发布

Browser 发布要先区分两类问题：

- runtime 在 `dotnet.create()` / `mono_wasm_load_runtime` 阶段失败：优先怀疑 SDK、workload、runtime pack、WebAssembly 输出格式或浏览器兼容性。
- managed `Main` 已进入后失败：再回到 AtomUI、Gallery 业务代码和资源加载路径。

不要在 runtime 初始化阶段的错误上盲改业务代码。先用最小纯 .NET Browser 项目验证，再用最小 Avalonia Browser 项目验证，最后才回到 Gallery。

当前 `AtomUIGallery.Browser` 普通发布固定关闭 Webcil：

```xml
<WasmEnableWebcil>false</WasmEnableWebcil>
```

原因是当前 .NET 10 browser-wasm 工具链生成的 Webcil managed assembly 在 Chromium 下会在 `mono_wasm_load_runtime` 阶段失败；同一个最小纯 .NET Browser 项目关闭 Webcil 后可以正常进入 managed `Main`。这个配置只改变 managed assembly 的发布包装格式，不改变 AtomUI/Gallery 的运行语义。

后续升级 .NET SDK、wasm-tools workload 或 runtime pack 时，可以用临时最小 Browser 项目重新做单变量验证。普通发布的关键对照是：

```bash
dotnet publish path/to/DotNetBrowserSmoke.csproj \
  -c Release -p:RunAOTCompilation=false \
  --nologo -v:minimal

dotnet publish path/to/DotNetBrowserSmoke.csproj \
  -c Release -p:RunAOTCompilation=false -p:WasmEnableWebcil=false \
  --nologo -v:minimal
```

只有默认 Webcil 输出也能稳定启动时，才能考虑删除 `WasmEnableWebcil=false`。

#### 当前 Browser AOT 结论

截至 2026-06-11，在下面这套环境里，`AtomUIGallery.Browser` 的 WebAssembly AOT 结论是：

| 发布方式 | 结果 | 说明 |
| --- | --- | --- |
| `RunAOTCompilation=false` + `WasmEnableWebcil=false` | 可以发布，也可以运行 | Gallery Browser 可以进入 Avalonia canvas，页面显示加载完成。 |
| `RunAOTCompilation=true` + `WasmEnableWebcil=false` | 可以发布，但不能运行 | 浏览器启动阶段在 `mono_wasm_load_runtime` 失败，错误是 `RuntimeError: remainder by zero`，未进入 managed `Main`。 |

也就是说，`WasmEnableWebcil=false` 只解决普通 Browser 发布的 Webcil 加载问题；它不能解决当前环境下 WebAssembly AOT runtime 初始化失败的问题。

本次验证环境：

| 项 | 版本 |
| --- | --- |
| OS | macOS 26.3.1(a), build `25D771280a`, `arm64`；`dotnet --info` 识别为 Mac OS X 26.3 |
| .NET SDK | `10.0.300` |
| `global.json` SDK | `10.0.300`, `rollForward=latestFeature` |
| .NET host runtime | `10.0.8`, `osx-arm64` |
| MSBuild | `18.6.3+caa81fa49` |
| workload set | `10.0.301.1` |
| wasm-tools manifest | `10.0.109/10.0.100` |
| `Microsoft.NETCore.App.Runtime.Mono.browser-wasm` pack | `10.0.9` |
| Emscripten `3.1.56` SDK / Node / Cache packs | `10.0.9` |
| Avalonia / Avalonia.Browser | `12.0.4` |
| ReactiveUI.Avalonia | `12.0.3` |
| ReactiveUI | `23.2.28` |
| SkiaSharp native WebAssembly assets | `3.119.4` |
| HarfBuzzSharp native WebAssembly assets | `8.3.1.3` |
| 浏览器 | Codex in-app Browser；当前插件环境未能读取具体 UA 版本 |

本次产物规模：

| 发布方式 | 发布目录 | `dotnet.native*.wasm` |
| --- | ---: | ---: |
| 普通发布，关闭 Webcil | 约 105 MB | 约 9.3 MB |
| AOT 发布，关闭 Webcil | 约 240 MB | 约 111 MB |

当前工程决策：

- Gallery Browser 发布保持 `RunAOTCompilation=false`。
- `AtomUIGallery.Browser` 保留 `WasmEnableWebcil=false`，用于保证普通 Browser 发布可运行。
- 当前不要把 Browser AOT 作为可交付发布目标；只有在 .NET SDK、wasm-tools、runtime pack、Avalonia Browser 或浏览器版本升级后，重新验证 `RunAOTCompilation=true` 能稳定启动，才能打开。
- 如果重新验证 Browser AOT，先跑最小纯 .NET Browser AOT，再跑最小 Avalonia Browser AOT，最后跑 Gallery Browser AOT。runtime 初始化阶段失败时，不要先改 AtomUI/Gallery 业务代码。

## Review 时看什么

每个 AOT 改动 review 时，至少回答这些问题：

- 旧实现依赖了哪种 AOT 不安全机制。
- 新实现如何消除这个机制。
- 旧语义是否保持，包括 binding priority、初始值、异常包装、排序、过滤、缓存顺序等。
- 新增 disposable、event handler、binding、activation scope 在哪里释放。
- 是否引入额外 per-instance 成本或 hot path 成本。
- generator 和生成物是否一致。
- analyzer 和必要测试是否通过。
- 是否需要真实 NativeAOT publish 验证。

Review 进度维护在：

```text
docs/superpowers/aot-review-checklist.md
```

每通过一个 review 点，要立即更新状态。

## 新增代码自查

新增或修改 AtomUI 代码时，先扫一遍：

- 是否新增 `new Binding("...")`、AXAML `ReflectionBinding` 或字符串 path binding。
- 是否新增 assembly/type/member 扫描。
- 是否新增 `Activator.CreateInstance(Type)`、`Expression.Compile()` 或 `MakeGenericType(...)`。
- 是否新增 ReactiveUI expression/view activation API。
- 是否新增动态 data model path，但没有 descriptor 或 generator。
- 是否新增 DataGrid/List/collection 字符串 path，但没有可在编译期报警的 analyzer 覆盖。
- 是否新增订阅、binding、event handler，但没有 release path。
- 是否新增 source generator 逻辑，但没有检查生成物稳定性。
- 是否新增 suppress trim/AOT warning。
- 是否修改 publish/linker/root descriptor，但没有真实 publish 验证。

建议 grep：

```bash
rg -n "new Binding\\(|ReflectionBinding|Assembly\\.GetTypes|GetFields\\(|GetCustomAttribute|Activator\\.CreateInstance|Expression\\.Compile|MakeGenericType|WhenAnyValue|ToProperty|OneWayBind|BindCommand|ReactiveUserControl" src controlgallery -g '*.cs' -g '*.axaml'
```

grep 命中不一定都是错误，但每个命中都要能说明边界和原因。

## 推荐验证命令

常规源码构建：

```bash
dotnet build AtomUI.slnx -c Release --no-incremental /m:1 /nr:false --nologo -v:minimal
```

全解决方案 AOT analyzer：

```bash
dotnet build AtomUI.slnx -c Release --no-incremental /m:1 \
  /p:IsAotCompatible=true \
  /p:EnableTrimAnalyzer=true \
  /p:EnableAotAnalyzer=true \
  /p:EnableSingleFileAnalyzer=true \
  /nr:false --nologo -v:minimal
```

Gallery NativeAOT publish：

```bash
dotnet publish controlgallery/AtomUIGallery.Desktop/AtomUIGallery.Desktop.csproj \
  -c Release -r osx-arm64 -p:GalleryPublishTrimmed=true \
  -p:GalleryPublishAot=true --self-contained true \
  --nologo -v:minimal
```

Focused tests：

```bash
dotnet test tests/AtomUI.Desktop.Controls.Tests/AtomUI.Desktop.Controls.Tests.csproj --nologo -v:minimal /m:1 /nr:false
dotnet test tests/AtomUI.Controls.Shared.Tests/AtomUI.Controls.Shared.Tests.csproj --nologo -v:minimal /m:1 /nr:false
dotnet test tests/AtomUI.Desktop.Controls.DataGrid.Tests/AtomUI.Desktop.Controls.DataGrid.Tests.csproj --nologo -v:minimal /m:1 /nr:false
dotnet test tests/AtomUIGallery.Tests/AtomUIGallery.Tests.csproj --nologo -v:minimal /m:1 /nr:false
```

Diff hygiene：

```bash
git diff --check
```

## 已知受控边界

下面这些命中不等于必须删除，但必须维持注解和文档边界：

- `TypeHelper` 动态 path fallback。
- `ObjectExtension` / `TypeMemberExtension` 反射 helper。
- DataGrid 对用户 `Binding` / `ReflectionBinding` 的兼容读取。

共同要求是：AtomUI 内置正常路径不用这些 fallback；用户动态场景使用时风险要显式暴露；AOT 用户要有 descriptor、generator 或显式注册这样的稳定替代路径。
