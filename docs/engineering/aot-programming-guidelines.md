# AtomUI AOT 编程规范

这份文档给日常写 AtomUI 代码的人用。它不是 AOT 改造记录，而是以后新增控件、主题、图标、语言资源、Gallery 示例和发布配置时要遵守的规则。

目标很简单：

- `src/` 里的库项目在 AOT、trim、single-file analyzer 下不产生项目自身 warning。
- Gallery 需要 NativeAOT 发布时，可以通过 analyzer，也可以完成真实 publish。
- AOT 改造不能改变原来的控件行为、绑定语义、异常语义、资源释放边界和关键性能路径。

## 先看这几条

日常开发先记住这 6 条，绝大多数 AOT 问题都能在写代码时避开。

1. 不要在 AtomUI 内置路径里新增字符串绑定，例如 `new Binding("Name")` 或 AXAML `ReflectionBinding`。
2. 不要运行时扫描 assembly、type、field、property 来完成内置注册。能生成就用 source generator，能显式注册就显式注册。
3. 不要用 `UnconditionalSuppressMessage` 盖掉 trim/AOT warning。它只是不显示 warning，不会保留被 trim 掉的 metadata。
4. 替换 AOT 不安全代码时，先确认旧语义，再改实现。尤其是 binding mode、binding priority、初始值、异常包装、dispose 后行为。
5. 新增 subscription、binding、event handler、activation scope、cache 时，必须能说清楚在哪里释放或失效。
6. Analyzer 通过不等于 NativeAOT publish 一定成功。涉及发布配置、linker、root descriptor 时，要做真实 publish 验证。

一句话总结：AOT 改造的方向是把运行时动态发现变成编译期已知代码，而不是把 warning 压下去。

## 按场景查

| 场景 | 推荐写法 | 避免写法 | Review 重点 |
| --- | --- | --- | --- |
| 控件属性同步 | `AvaloniaProperty`、`GetObservable`、`BindUtils.RelayBind(...)` | `new Binding("Path")` | mode、priority、初始值、dispose 后行为 |
| 模板内 part 同步 | C# 里拿到 template part 后强类型绑定 | AXAML `ReflectionBinding` | template reapply 时旧 part 是否释放 |
| 内置类型注册 | source generator 生成 registry/catalog | `Assembly.GetTypes()` | generator 和生成物是否一致 |
| token converter | generator 生成静态数组 | 运行时扫描 attribute 后 `Activator.CreateInstance` | 数量、顺序、map 行为是否不变 |
| 语言资源 | generated provider wrapper | `GetFields(...)` 枚举资源字段 | 缺字段、异常、日志语义是否不变 |
| 图标创建 | generated factory 或 virtual factory | 扫描 icon assembly 后反射创建 | 非法 kind 的异常包装是否不变 |
| DataGrid 动态 path | `[GenerateDataMemberAccessors]` 或手写 descriptor | 对用户模型直接 `GetProperty(path)` | sort/filter/group/AddNew 是否走 descriptor |
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

SG 的价值不是“把反射挪个地方”，而是让运行时代码变成普通的强类型 C#。这样 trimmer 能看见类型、构造函数和成员，NativeAOT 也不需要动态代码生成。

### Generator 项目边界

`AtomUI.Generator` 是 Roslyn analyzer 项目，不是运行时依赖。引用方式保持：

```xml
<ProjectReference Include="../AtomUI.Generator/AtomUI.Generator.csproj"
                  OutputItemType="Analyzer"
                  ReferenceOutputAssembly="false"
                  PrivateAssets="all" />
```

生成文件可以输出到 `GeneratedFiles/`，但项目要继续排除重复编译：

```xml
<EmitCompilerGeneratedFiles>true</EmitCompilerGeneratedFiles>
<CompilerGeneratedFilesOutputPath>GeneratedFiles</CompilerGeneratedFilesOutputPath>
<Compile Remove="$(CompilerGeneratedFilesOutputPath)/**/*.cs"/>
```

`PublishAot=true` 是 MSBuild global property，会沿着 `ProjectReference` 传播。generator 是 `netstandard2.0` analyzer 项目，不应该参与 NativeAOT 发布，所以 generator 项目要把这些属性隔离在本项目内：

```xml
TreatAsLocalProperty="IsAotCompatible;EnableAotAnalyzer;EnableTrimAnalyzer;EnableSingleFileAnalyzer;PublishAot;PublishTrimmed;PublishSingleFile;SelfContained;RuntimeIdentifier"
```

并且在 generator 项目里显式关闭运行时发布属性。

### 生成物必须稳定

改 generator 时要同时看两层：

- writer 代码是不是符合预期。
- `GeneratedFiles/AtomUI.Generator/**` 里的生成物是不是能由当前 writer 稳定复现。

不能只手改生成物，也不能只改 generator 却不检查生成物 diff。之前 review 里已经出现过 generator writer 和提交的生成物不一致，这类问题会让后续维护很难判断真实来源。

## Theme / Token

### Control token 注册

运行时不要扫描 assembly 查找 control token。应由 generator 生成 `ControlTokenTypePool`，返回带 metadata 契约的注册项：

```csharp
tokenTypes.Add(new ControlTokenRegistration(typeof(MyControlToken)));
```

`ControlTokenRegistration.TokenType` 要携带 `DynamicallyAccessedMembers`：

```csharp
[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicParameterlessConstructor |
                            DynamicallyAccessedMemberTypes.PublicProperties |
                            DynamicallyAccessedMemberTypes.NonPublicProperties)]
public Type TokenType { get; }
```

这里有两个关键点：

- DAM 不是“全局保留开关”。只有 `Type` 值从带 DAM 契约的位置继续传到 `Activator.CreateInstance` 或 property scan，trimmer 才知道要保留哪些 metadata。
- `IList<Type>` 不能表达集合元素的 metadata 要求，所以需要 `ControlTokenRegistration` 这样的包装类型。

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

### 内置 provider 走生成代码

内置语言 provider 可以继续保留 `[LanguageProvider(LanguageCode, LanguageId)]`，但正常注册路径不能再靠反射读取 attribute，也不能通过 `GetFields(...)` 枚举 public static 字段。

规范做法：

- raw provider 声明为 `partial`。
- `LanguageProviderConstructorSourceWriter` 为没有手写无参构造的 provider 生成显式构造：

```csharp
public en_US()
    : base(LanguageCode.en_US, "Button")
{
}
```

- `LanguageProviderPool` 生成 wrapper provider，直接把资源写入 `ResourceDictionary`：

```csharp
dictionary[ButtonLangResourceKind.OkText] = ButtonLang.en_US.OkText;
```

命名使用 `{LanguageId}{NormalizedLanguageCode}LanguageProvider`，例如：

```text
CommonEnUSLanguageProvider
```

Review 时要看：

- 资源 key 集合是否来自同一个 `LanguageId` 下 provider 字段的并集。
- 缺字段时是否保留旧逻辑的运行时异常语义。
- catch、log、throw 行为是否不变。

### fallback 边界

`LanguageProvider` 基类可以保留反射 fallback，但它必须有 DAM/RUC 标注，并且只能作为兼容边界。AtomUI 内置 provider 的正常路径不能依赖它。

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

## Publish / NativeAOT

### Analyzer 和真实 publish 都要跑

AOT analyzer 通过，只说明静态分析没有发现项目自身 warning。它不等于 NativeAOT 链接一定成功。涉及发布配置或 native 依赖时，要做真实 publish。

Windows 11 上 Gallery Desktop 的 NativeAOT 工具链、发布命令、产物验证和排障记录见 [windows-native-aot-publish.md](windows-native-aot-publish.md)。Linux 平台的对应手册见 [linux-native-aot-publish.md](linux-native-aot-publish.md)。

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
  -c Release -r osx-arm64 -p:PublishAot=true --self-contained true \
  --nologo -v:minimal
```

### macOS NativeAOT

当前 macOS 本机发布使用：

```xml
<ItemGroup Condition="'$(PublishAot)' == 'true' and $([MSBuild]::IsOSPlatform('OSX'))">
    <LinkerArg Include="-L/opt/homebrew/lib" Condition="Exists('/opt/homebrew/lib/libssl.dylib')"/>
</ItemGroup>
```

原因：

- NativeAOT 链接 `System.Net.Security.Native` 时需要 `libssl` / `libcrypto`。
- Homebrew OpenSSL 默认在 `/opt/homebrew/lib`，默认 linker 搜索路径可能找不到。

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
  -c Release -r osx-arm64 -p:PublishAot=true --self-contained true \
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

- `LanguageProvider` 基类 fallback 反射。
- `TypeHelper` 动态 path fallback。
- `ObjectExtension` / `TypeMemberExtension` 反射 helper。
- DataGrid 对用户 `Binding` / `ReflectionBinding` 的兼容读取。
- Theme token 创建中基于 `ControlTokenRegistration.TokenType` 的 `Activator.CreateInstance`。

共同要求是：AtomUI 内置正常路径不用这些 fallback；用户动态场景使用时风险要显式暴露；AOT 用户要有 descriptor、generator 或显式注册这样的稳定替代路径。
