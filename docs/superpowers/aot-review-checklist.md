# AtomUI AOT Review 清单

分支：`feature/aot`

用途：逐项跟踪 AOT 相关修改。每 review 通过一项，就把对应项改成 `[x]`，并把状态标为 `已通过`。

状态说明：

- `[ ] 待 review`：还没有 review。
- `[x] 已通过`：已经 review 并接受。
- `[ ] 需要修改`：已经 review，但需要继续修复后再确认。

通用 review 约束：

- AOT 改造不能改变原有绑定语义。
- AOT 改造不能引入新的资源泄露：新增订阅、binding、event handler、缓存和生成对象必须有明确生命周期，template reapply 时必须释放旧 template part 相关资源。
- AOT 改造不能引入明显性能退化：替换 reflection binding 时优先使用 `AvaloniaProperty`、`GetObservable`、直接 `Bind` 或生成代码，避免运行时字符串 path 解析、反射扫描、动态代码生成和不必要的 per-instance 额外对象。

## 当前 Review 进度

| Review 项 | 状态 | 提交状态 | 说明 |
| --- | --- | --- | --- |
| AOT-RV-001：BindUtils AOT-safe relay binding | `[x] 已通过` | 已提交 | `OneWay`、`OneTime`、`TwoWay`、`OneWayToSource` 和 getter overload 绑定语义已确认。 |
| AOT-RV-002：Template-parent binding 替换 | `[x] 已通过` | 已随 `fix(Button): preserve loading margin with AOT-safe binding` 提交 | HyperLinkButton loading margin 已恢复旧绑定语义，并移除 AXAML reflection binding。 |
| AOT-RV-003：Theme token 注册与 resource key cache | `[x] 已通过` | 未提交 | Theme token 注册、resource key cache、converter 注册 source generator 方案已确认。 |
| AOT-RV-004：Theme manager builder extension 生成 | `[x] 已通过` | 未提交 | `ControlTokenTypePool` 改为返回带 DAM 契约的 `ControlTokenRegistration`，注册集合和顺序不变。 |
| AOT-RV-005：Icon provider 构造与 factory 生成 | `[x] 已通过` | 未提交 | AntDesign icon factory 生成、异常包装和 generator/生成物一致性已确认。 |
| AOT-RV-006：Icon brush clone 路径 | `[x] 已通过` | 未提交 | `Icon.CreateInstance()` 虚方法 + 生成图标同类型构造已确认。 |
| AOT-RV-007：Reflection helper 注解与边界收敛 | `[x] 已通过` | 未提交 | 反射 helper DAM/RUC 边界已确认，`TypeHelper` 动态 path 不再靠 suppression 压 warning。 |
| AOT-RV-008：Language provider 与 dynamic resource 查找 | `[x] 已通过` | 未提交 | SG wrapper language provider 已确认，正常路径不再依赖语言字段反射。 |
| AOT-RV-009：Shared collection view 排序与构造反射 | `[x] 已通过` | 未提交 | `GenerateDataMemberAccessors` SG + collection sort/AddNew 接入已确认。 |
| AOT-RV-010：DataGrid 动态 model binding 与自动生成列 | `[x] 已通过` | 未提交 | sort/filter/group/AddNew 与自动生成列 metadata/read-only/data type inference 已接入 `GenerateDataMemberAccessors` descriptor，语义与验证已确认。 |
| AOT-RV-011：Control template-part binding 替换 | `[x] 已通过` | 未提交 | Drawer `OpenOn` 的 TopLevel 默认来源已改为 AOT-safe visual ancestor binding，本批次已确认通过。 |
| AOT-RV-012：NavMenu node binding | `[x] 已通过` | 未提交 | `INavMenuNode` 字符串 path binding 已替换为强类型 getter relay，容器复用释放旧 node binding 的语义已确认。 |
| AOT-RV-013：ReactiveWindow AOT 边界 | `[x] 已通过` | 未提交 | ReactiveUI view-side `WhenActivated` 已移除，VM activation 语义和释放边界已确认。 |
| AOT-RV-014：Generated files 与 source generator packaging | `[x] 已通过` | 未提交 | 生成物稳定复现，source generator packaging 和 AOT analyzer 属性隔离已确认。 |
| AOT-RV-015：剩余 controlgallery AOT warning | `[ ] 待 review` | 部分已提交；ReactiveUI 批次未提交 | Gallery localization/icon 已通过并提交；ReactiveUI expression API 已改造并清零 analyzer warning，待 review。 |
| AOT-RV-016：Gallery.Desktop NativeAOT publish 配置 | `[ ] 待 review` | 未提交 | NativeAOT publish 已能产出 osx-arm64 Mach-O，剩余 warning 来自 ReactiveUI 包内部和 macOS linker 环境。 |

最近一次验证快照：

- `src/` AOT analyzer：`dotnet build src/AtomUI.Core/AtomUI.Core.csproj -c Release --no-incremental /p:IsAotCompatible=true /p:EnableTrimAnalyzer=true /p:EnableAotAnalyzer=true /p:EnableSingleFileAnalyzer=true /nr:false --nologo -v:minimal` 通过，`0 Warning(s), 0 Error(s)`。
- `src/AtomUI.Desktop.Controls.DataGrid` AOT analyzer：`dotnet build src/AtomUI.Desktop.Controls.DataGrid/AtomUI.Desktop.Controls.DataGrid.csproj -c Release --no-incremental /m:1 /p:IsAotCompatible=true /p:EnableTrimAnalyzer=true /p:EnableAotAnalyzer=true /p:EnableSingleFileAnalyzer=true /nr:false --nologo -v:minimal` 通过，`0 Warning(s), 0 Error(s)`。
- Gallery AOT analyzer：`dotnet build controlgallery/AtomUIGallery/AtomUIGallery.csproj -c Release --no-incremental /m:1 /p:IsAotCompatible=true /p:EnableTrimAnalyzer=true /p:EnableAotAnalyzer=true /p:EnableSingleFileAnalyzer=true /nr:false --nologo -v:minimal` 通过，`0 Warning(s), 0 Error(s)`。
- 全解决方案 AOT analyzer：`dotnet build AtomUI.slnx -c Release --no-incremental /m:1 /p:IsAotCompatible=true /p:EnableTrimAnalyzer=true /p:EnableAotAnalyzer=true /p:EnableSingleFileAnalyzer=true /nr:false --nologo -v:minimal` 通过，`0 Warning(s), 0 Error(s)`。

## Review 项

### AOT-RV-001：BindUtils AOT-safe relay binding

- 状态：`[x] 已通过`
- 主要文件：
  - `src/AtomUI.Core/Data/BindUtils.cs`
- 为什么这样改：
  - 移除 core relay helper 里的 `new Binding(...)` / 字符串路径绑定，因为 Avalonia reflection binding 对 AOT 不安全。
  - 强约束是不能改变旧绑定语义：`BindingPriority`、初始值传播、source 写回、`IDisposable` 清理行为都必须和旧 `Binding` 对齐。
- Review 重点：
  - `OneWay` 仍然委托给 `target.Bind(..., priority)`。
  - `OneTime` 按指定 priority 写入，并和 Avalonia local/direct value 的释放语义保持一致。
  - `TwoWay` 的 source-to-target 走 Avalonia `Bind`，target-to-source 只处理 target 后续变化。
  - `OneWayToSource` 跳过 target 初始值，只把后续 target 变化写回 source。
  - `object + Func getter` 使用 observable + `target.Bind(..., priority)`，不再手写 `SetCurrentValue`。
- 验证：
  - legacy binding 对照测试已覆盖并通过：`OneWay`、`OneTime`、`TwoWay`、`OneWayToSource`、`object + Func getter`。
  - Core AOT analyzer 构建已通过。

### AOT-RV-002：Template-parent binding 替换

- 状态：`[x] 已通过`
- 主要文件：
  - `src/AtomUI.Core/Theme/BaseControlTheme.cs`
  - `src/AtomUI.Desktop.Controls/Buttons/HyperLinkButton.cs`
  - `src/AtomUI.Desktop.Controls/Buttons/Themes/HyperLinkButtonTheme.axaml`
  - 使用生成代码或强类型 template-parent binding 的控件
- 为什么这样改：
  - 字符串 template-parent path 会创建 reflection binding path，在 trimming/AOT 下不安全。
  - 替代实现应尽量使用 `AvaloniaProperty` 或 compiled binding path。
- Review 重点：
  - 字符串 overload 已标记为编译期不可用，能阻止新的 unsafe 调用。
  - 新 overload 保留旧的 binding mode、priority、converter 和目标属性行为。
  - AXAML 修改不能改变视觉状态或 style selector 行为。
- 当前 review 备注：
  - `HyperLinkButtonTheme.axaml` 中直接删除 `PART_LoadingIcon.Margin` 的 `ReflectionBinding` 不严格等价；默认 token/style 路径下两个 icon 的 margin 一致，但旧绑定会跟随 `PART_ButtonIcon.Margin` 的任意 effective value。
  - `[x] 已通过`：已改为在 `HyperLinkButton.OnApplyTemplate` 中用 `PART_LoadingIcon.Bind(Margin, PART_ButtonIcon.GetObservable(Margin), LocalValue)` 恢复旧语义，同时避免 AXAML reflection binding。
  - 资源生命周期：`HyperLinkButton.OnApplyTemplate` 会先 dispose 旧 binding，再绑定新的 template part，避免 template reapply 后旧 part 被订阅链持有。
  - 性能影响：新实现每个 `HyperLinkButton` 保留一个 margin 观察关系，和旧 `ReflectionBinding` 的实例级绑定数量一致；实现改为直接 `AvaloniaObject.Bind + GetObservable`，不再做 reflection path 解析，运行时成本低于旧 reflection binding。
- 验证：
  - 构建应能发现仍在调用 obsolete string overload 的位置。
  - 建议对变更 template binding 的控件做视觉 smoke check。
  - `HyperLinkButton` margin 同步修复后，`dotnet build src/AtomUI.Desktop.Controls/AtomUI.Desktop.Controls.csproj -c Release --no-incremental /nr:false --nologo -v:minimal` 通过，`0 Warning(s), 0 Error(s)`。
  - `HyperLinkButton` margin 同步修复后，`dotnet build src/AtomUI.Desktop.Controls/AtomUI.Desktop.Controls.csproj -c Release --no-incremental /p:IsAotCompatible=true /p:EnableTrimAnalyzer=true /p:EnableAotAnalyzer=true /p:EnableSingleFileAnalyzer=true /nr:false --nologo -v:minimal` 通过，`0 Warning(s), 0 Error(s)`。

### AOT-RV-003：Theme token 注册与 resource key cache

- 状态：`[x] 已通过`
- 主要文件：
  - `src/AtomUI.Core/Theme/ControlTokenRegistration.cs`
  - `src/AtomUI.Core/Theme/ThemeManager.cs`
  - `src/AtomUI.Core/Theme/ThemeManagerBuilder.cs`
  - `src/AtomUI.Core/Theme/IThemeManagerBuilder.cs`
  - `src/AtomUI.Core/Theme/ThemeConfigProvider.cs`
  - `src/AtomUI.Core/Theme/ThemeResourceKeyCache.cs`
  - `src/AtomUI.Core/Theme/TokenSystem/AbstractDesignToken.cs`
  - `src/AtomUI.Core/Theme/TokenSystem/AbstractControlDesignToken.cs`
  - `src/AtomUI.Generator/DesignToken/TokenValueConverterRegistrationGenerator.cs`
  - `src/AtomUI.Generator/DesignToken/TokenValueConverterRegistryClassWriter.cs`
  - `src/AtomUI.Core/GeneratedFiles/AtomUI.Generator/AtomUI.Generator.TokenValueConverterRegistrationGenerator/TokenValueConverterRegistry.g.cs`
- 为什么这样改：
  - Theme token 发现和 resource key 创建之前依赖运行时类型/成员反射。
  - AOT-safe 注册需要显式 control token metadata，不能在运行时扫描任意成员。
  - `TokenValueConverter` 注册保留原来的 attribute 声明语义，但把 `Assembly.GetExecutingAssembly().GetTypes()`、`IsDefined(...)` 和 `Activator.CreateInstance(...)` 从运行时移到 source generator 编译期收集。
- Review 重点：
  - Token 注册顺序仍然确定。
  - Global、alias、control token 的 resource key 保持稳定。
  - Resource cache 的失效行为不变。
  - 用户自定义 theme 仍能注册并解析预期 token。
  - `[x] 已通过`：`TokenValueConverterRegistry.g.cs` 由 `[TokenValueConverter]` 生成显式 converter 列表，`AbstractDesignToken` 只调用 `TokenValueConverterRegistry.Create()`，`LoadConfig()` 后续查表和 `SetValue` 逻辑不变。
  - 资源生命周期：converter registry 仍是静态字典，每个 converter 类型只构造一次；没有新增事件、observable、binding 或需要释放的资源。
  - 性能影响：运行时不再扫描 assembly、不再做 attribute 反射判断、不再用 `Activator` 构造 converter；初始化成本低于旧逻辑。
- 验证：
  - 在 Gallery 或 focused sample 中检查 theme 切换和 token override。
  - `dotnet build src/AtomUI.Generator/AtomUI.Generator.csproj -c Release --no-incremental /nr:false --nologo -v:minimal` 通过，`0 Warning(s), 0 Error(s)`。
  - `dotnet build src/AtomUI.Core/AtomUI.Core.csproj -c Release --no-incremental /nr:false --nologo -v:minimal` 通过，`0 Warning(s), 0 Error(s)`。
  - `dotnet build src/AtomUI.Core/AtomUI.Core.csproj -c Release --no-incremental /p:IsAotCompatible=true /p:EnableTrimAnalyzer=true /p:EnableAotAnalyzer=true /p:EnableSingleFileAnalyzer=true /nr:false --nologo -v:minimal` 通过，`0 Warning(s), 0 Error(s)`。

### AOT-RV-004：Theme manager builder extension 生成

- 状态：`[x] 已通过`
- 主要文件：
  - `src/AtomUI.Generator/AtomUI.Generator.csproj`
  - `src/AtomUI.Generator/DesignToken/ControlTokenTypePoolClassWriter.cs`
  - `src/AtomUI.Controls/GeneratedFiles/AtomUI.Generator/AtomUI.Generator.TokenResourceKeyGenerator/ControlTokenTypePool.g.cs`
  - `src/AtomUI.Desktop.Controls/GeneratedFiles/AtomUI.Generator/AtomUI.Generator.TokenResourceKeyGenerator/ControlTokenTypePool.g.cs`
  - `src/AtomUI.Desktop.Controls.ColorPicker/GeneratedFiles/AtomUI.Generator/AtomUI.Generator.TokenResourceKeyGenerator/ControlTokenTypePool.g.cs`
  - `src/AtomUI.Desktop.Controls.DataGrid/GeneratedFiles/AtomUI.Generator/AtomUI.Generator.TokenResourceKeyGenerator/ControlTokenTypePool.g.cs`
  - `src/AtomUI.Controls/ThemeManagerBuildExtensions.cs`
  - `src/AtomUI.Desktop.Controls/ThemeManagerBuilderExtensions.cs`
  - `src/AtomUI.Desktop.Controls.ColorPicker/ThemeManagerBuilderExtensions.cs`
  - `src/AtomUI.Desktop.Controls.DataGrid/ThemeManagerBuilderExtensions.cs`
- 为什么这样改：
  - 用生成 metadata 替代反射式 token type 发现。
  - Extension API 显式注册具体 token pool，便于 AOT 保留和分析。
  - `IList<Type>` 无法把 trimming metadata requirement 传递到集合元素，改为 `ControlTokenRegistration` 后，`TokenType` 属性可携带 `DynamicallyAccessedMembers` 契约。
- Review 重点：
  - 生成的 token pool 内容完整，没有遗漏控件。
  - Extension method 仍注册和之前相同的模块。
  - 生成文件的变更是有意的，并且可以由 generator 稳定复现。
  - `[x] 已通过`：`ControlTokenTypePool.GetTokenTypes()` 从 `IList<Type>` 改为 `IList<ControlTokenRegistration>`，调用方仍把同一批 token type 传给 `AddControlToken(...)`，注册顺序和集合内容不变。
  - 资源生命周期：只新增一次性注册阶段的 readonly struct 包装，不引入订阅、binding、event handler 或 disposable。
  - 性能影响：启动注册时多创建少量 struct wrapper；没有额外反射扫描或长期缓存，运行时影响可忽略。
- 验证：
  - 如果 generator 流程可用，重新生成 token 文件并比较 diff。
  - 构建所有消费 theme builder extension 的包。

### AOT-RV-005：Icon provider 构造与 factory 生成

- 状态：`[x] 已通过`
- 主要文件：
  - `src/AtomUI.Core/Controls/Icon/IconProvider.cs`
  - `src/AtomUI.Core/Controls/Icon/IconProviderCache.cs`
  - `src/AtomUI.Icons.AntDesign/AntDesignIconProvider.cs`
  - `src/AtomUI.Icons.AntDesign/AntDesignIconProvider.Factory.g.cs`
  - `src/AtomUI.Icons.Shared/IconPackageGenerator.cs`
  - `src/AtomUI.Icons.Shared/DefaultIconPackageGenerator.cs`
- 为什么这样改：
  - Expression 编译和宽泛的 `Activator.CreateInstance` 对 AOT 不友好。
  - Icon factory 应显式或生成，避免动态代码并保证构造函数被保留。
- Review 重点：
  - 每个 icon enum value 都映射到正确 icon type/factory。
  - Cache key 行为和淘汰逻辑仍然正确。
  - 生成的 factory 文件被正确提交或能稳定生成。
  - 缺少 icon constructor 时失败信息清晰。
  - `[x] 已通过`：`AntDesignIconProvider.GetIcon` 已恢复旧 base 路径的异常包装语义，`CreateIcon(kind)` 异常会包装成 `InvalidOperationException($"Create icon {kind} failed", ex)`。
  - `[x] 已通过`：`DefaultIconPackageGenerator` 的 `GetIconType` 输出模板已和 `AntDesignIconProvider.Factory.g.cs` 对齐为 `switch (kind)` statement，生成文件头部也补齐 `using System;`。
  - 覆盖检查：`AntDesignIconKind` 为 843 个 enum 成员，`GetIconType` 为 843 个分支，`CreateIcon` 为 843 个分支。
- 验证：
  - 抽查多个 AntDesign icon enum。
  - AOT analyzer 构建不再出现 icon provider 相关 warning。
  - `dotnet build src/AtomUI.Icons.Shared/AtomUI.Icons.Shared.csproj -c Release --no-incremental /nr:false --nologo -v:minimal` 通过，`0 Warning(s), 0 Error(s)`。
  - `dotnet build src/AtomUI.Icons.AntDesign/AtomUI.Icons.AntDesign.csproj -c Release --no-incremental /p:IsAotCompatible=true /p:EnableTrimAnalyzer=true /p:EnableAotAnalyzer=true /p:EnableSingleFileAnalyzer=true /nr:false --nologo -v:minimal` 通过，`0 Warning(s), 0 Error(s)`。

### AOT-RV-006：Icon brush clone 路径

- 状态：`[x] 已通过`
- 主要文件：
  - `src/AtomUI.Core/Controls/Icon/Icon.cs`
  - `src/AtomUI.Controls/Rate/RateItem.cs`
  - `src/AtomUI.Icons.AntDesign/AntDesignIcon.cs`
  - `src/AtomUI.Icons.AntDesign.Generator/AntDesignGenerator.cs`
  - `src/AtomUI.Icons.AntDesign/GeneratedIcons/*.g.cs`
- 为什么这样改：
  - 旧 `RateItem` 通过 `iconChar.GetType()` + `Activator.CreateInstance(iconType)` 创建同一具体 icon 类型；这依赖运行时反射构造，对 AOT/trimming 不安全。
  - 之前尝试的 `BrushCloneIcon` wrapper 不严格等价：它不再创建同一具体类型，也可能额外复制或覆盖旧实现没有设置的 brush/animation/line 属性。
  - 当前方案把“如何创建同类型 icon”的责任下沉到 `Icon.CreateInstance()` 虚方法；AntDesign 生成图标由 generator 生成 `return new SameIconType();`，从根上消除反射构造。
- Review 重点：
  - 生成图标的 clone 必须是同一具体类型，等价于旧 `Activator.CreateInstance(iconChar.GetType())`。
  - `AntDesignIcon` 自身是 public concrete class，也补了 `CreateInstance()` 返回 `new AntDesignIcon()`，避免它落到基类不支持路径。
  - `RateItem` 仍然只给 clone 设置 `FillBrush` 和 `StrokeBrush`，不额外改 `Secondary*`、`FallbackBrush`、stroke width、line cap/join 或 animation 属性。
  - `Icon.CreateInstance()` 默认抛 `NotSupportedException`，自定义 icon 如果要支持 AOT-safe clone，需要显式 override；不能静默换成 wrapper 导致语义漂移。
  - `Rate` 在 normal、hover、selected、disabled 状态下视觉不变。
  - 资源生命周期：没有新增 binding、observable、event handler 或缓存；每次 brush visual 仍创建新的 icon 实例，不持有源 icon。
  - 性能影响：用一次虚调用 + 直接构造替代反射构造；对象数量和旧实现一致，运行时成本低于旧反射路径。
- 验证：
  - Icon 和 `Rate` 视觉 smoke check。
  - `dotnet build src/AtomUI.Icons.AntDesign/AtomUI.Icons.AntDesign.csproj -c Release --no-incremental /p:IsAotCompatible=true /p:EnableTrimAnalyzer=true /p:EnableAotAnalyzer=true /p:EnableSingleFileAnalyzer=true /nr:false --nologo -v:minimal` 通过，`0 Warning(s), 0 Error(s)`。
  - `dotnet build src/AtomUI.Controls/AtomUI.Controls.csproj -c Release --no-incremental /p:IsAotCompatible=true /p:EnableTrimAnalyzer=true /p:EnableAotAnalyzer=true /p:EnableSingleFileAnalyzer=true /nr:false --nologo -v:minimal` 通过，`0 Warning(s), 0 Error(s)`。

### AOT-RV-007：Reflection helper 注解与边界收敛

- 状态：`[x] 已通过`
- 主要文件：
  - `src/AtomUI.Core/Reflection/ObjectExtension.cs`
  - `src/AtomUI.Core/Reflection/TypeMemberExtension.cs`
  - `src/AtomUI.Core/Utils/TypeHelper.cs`
  - `src/AtomUI.Core/Utils/AvaloniaPropertyReflectionExtensions.cs`
- 为什么这样改：
  - 部分反射 helper 仍有必要，但需要 trimming annotation 和明确约束。
  - AOT-compatible API 不能隐藏对 public constructor、public property、indexer 或 interface metadata 的假设。
  - `TypeHelper` 的动态 data model path 不能靠 `UnconditionalSuppressMessage` 压 warning；当前只把风险显式传递出去，真正的 collection/DataGrid AOT-safe 访问器放到 AOT-RV-009/AOT-RV-010 处理。
- Review 重点：
  - `TypeHelper.cs` 不再包含 `UnconditionalSuppressMessage`；动态 path 入口改为 `RequiresUnreferencedCode`，让调用点显式暴露。
  - `GetPropertyOrIndexer` 已移除 `MakeGenericType`，改为扫描已有 `IList<>` / `IReadOnlyList<>` 接口，避免在 NativeAOT 路径引入动态泛型构造。
  - 公开 API 在需要调用方保留 metadata 时暴露 `DynamicallyAccessedMembers` 要求。
  - Metadata 被 trim 后 helper 不会静默返回错误结果。
- 验证：
  - `dotnet build src/AtomUI.Core/AtomUI.Core.csproj -c Release --no-incremental /p:IsAotCompatible=true /p:EnableTrimAnalyzer=true /p:EnableAotAnalyzer=true /p:EnableSingleFileAnalyzer=true /nr:false --nologo -v:minimal` 通过，`0 Warning(s), 0 Error(s)`。
  - `dotnet build src/AtomUI.Controls.Shared/AtomUI.Controls.Shared.csproj -c Release --no-incremental /p:IsAotCompatible=true /p:EnableTrimAnalyzer=true /p:EnableAotAnalyzer=true /p:EnableSingleFileAnalyzer=true /nr:false --nologo -v:minimal` 通过但保留 `8 Warning(s)`，这些 warning 是 collection 动态 path 调用 `TypeHelper` RUC 入口后暴露出来的真实边界，归入 AOT-RV-009 处理。

### AOT-RV-008：Language provider 与 dynamic resource 查找

- 状态：`[x] 已通过`
- 主要文件：
  - `src/AtomUI.Core/Language/LanguageProvider.cs`
  - `src/AtomUI.Generator/LanguageGenerator.cs`
  - `src/AtomUI.Generator/Language/LanguageProviderWalker.cs`
  - `src/AtomUI.Generator/Language/LanguageInfo.cs`
  - `src/AtomUI.Generator/Language/LanguageProviderPoolClassSourceWriter.cs`
  - `src/AtomUI.Controls*/**/Localization/*.cs`
  - `src/AtomUI.*Controls*/GeneratedFiles/AtomUI.Generator/AtomUI.Generator.LanguageGenerator/LanguageProviderPool.g.cs`
- 为什么这样改：
  - 旧 `LanguageProviderPool` 直接 `new` 原始 provider，运行时会进入 `LanguageProvider` 无参构造，通过 `GetCustomAttribute<LanguageProviderAttribute>()` 解析 `LangCode/LangId`。
  - 旧 `BuildResourceDictionary(...)` 通过 provider runtime type 的 public static fields 做资源写入，依赖 `GetFields(...)` 和 `FieldInfo.GetValue(...)`。
  - AOT 正常注册路径应该由 Source Generator 生成强类型 provider wrapper：构造器直接传入 `LanguageCode/LangId`，`BuildResourceDictionary(...)` 直接把 enum key 映射到原始 const/static field。
  - 原始 provider 仍保留作为兼容 fallback，但内置 provider 已显式调用 `base(LanguageCode.X, Id)`，不再走 attribute 反射构造。
- Review 重点：
  - 生成类型命名为 `{LanguageId}{NormalizedLanguageCode}LanguageProvider`，例如 `CommonEnUSLanguageProvider`、`DatePickerZhCNLanguageProvider`。
  - pool 只实例化生成 wrapper，不再实例化原始 localization provider，也不再生成 `DynamicDependency(PublicFields, 原始 provider)`。
  - wrapper 的资源 key 集合和 `LanguageResourceConst.g.cs` 的 enum 使用同一份 SG 收集到的同 `LanguageId` provider 字段并集；缺字段时生成与旧逻辑同文案的 runtime exception：`Language item: {Name} does not exist in {ProviderType}`。
  - wrapper 保留旧 catch/log/throw 语义：写资源失败时仍记录 `Build Resource for Language {resourceKindType.FullName} error.` 后重新抛出。
  - 基类反射 fallback 仍保留；类级 DAM 明确 fallback 需要 provider public fields metadata，不是正常 SG 路径的依赖。
  - 没有新增订阅、binding、event handler 或需要释放的资源；运行时从反射扫描/缓存变为直接赋值，性能路径更短。
  - Language variant 切换和 `ResourceDictionary` 替换流程未改。
- 验证：
  - `dotnet build src/AtomUI.Generator/AtomUI.Generator.csproj -c Release --no-incremental /nr:false --nologo -v:minimal` 通过，`0 Warning(s), 0 Error(s)`。
  - `dotnet build src/AtomUI.Core/AtomUI.Core.csproj -c Release --no-incremental /m:1 /p:IsAotCompatible=true /p:EnableTrimAnalyzer=true /p:EnableAotAnalyzer=true /p:EnableSingleFileAnalyzer=true /nr:false --nologo -v:minimal` 通过，`0 Warning(s), 0 Error(s)`。
  - `dotnet build src/AtomUI.Controls/AtomUI.Controls.csproj -c Release --no-incremental /p:IsAotCompatible=true /p:EnableTrimAnalyzer=true /p:EnableAotAnalyzer=true /p:EnableSingleFileAnalyzer=true /nr:false --nologo -v:minimal` 通过；Language 相关 warning 为 0，剩余 8 个 warning 属于 AOT-RV-009 Shared collection 动态 path。
  - `dotnet build src/AtomUI.Desktop.Controls/AtomUI.Desktop.Controls.csproj -c Release --no-incremental /m:1 /p:IsAotCompatible=true /p:EnableTrimAnalyzer=true /p:EnableAotAnalyzer=true /p:EnableSingleFileAnalyzer=true /nr:false --nologo -v:minimal` 通过；Language 相关 warning 为 0，剩余 8 个 warning 属于 AOT-RV-009。
  - `dotnet build src/AtomUI.Desktop.Controls.ColorPicker/AtomUI.Desktop.Controls.ColorPicker.csproj -c Release --no-incremental /m:1 /p:IsAotCompatible=true /p:EnableTrimAnalyzer=true /p:EnableAotAnalyzer=true /p:EnableSingleFileAnalyzer=true /nr:false --nologo -v:minimal` 通过；Language 相关 warning 为 0，剩余 8 个 warning 属于 AOT-RV-009。
  - AOT-RV-008 当时的 `dotnet build src/AtomUI.Desktop.Controls.DataGrid/AtomUI.Desktop.Controls.DataGrid.csproj -c Release --no-incremental /m:1 /p:IsAotCompatible=true /p:EnableTrimAnalyzer=true /p:EnableAotAnalyzer=true /p:EnableSingleFileAnalyzer=true /nr:false --nologo -v:minimal` 通过；Language 相关 warning 为 0，当时剩余 warning 属于 AOT-RV-009/AOT-RV-010 动态 data model path，现已继续处理。
  - `rg "DynamicDependency|DynamicallyAccessedMemberTypes.PublicFields|new AtomUI\\..*Localization|new AtomUI\\..*Lang\\." src/*/GeneratedFiles/AtomUI.Generator/AtomUI.Generator.LanguageGenerator/LanguageProviderPool.g.cs` 无命中。

### AOT-RV-009：Shared collection view 排序与构造反射

- 状态：`[x] 已通过`
- 主要文件：
  - `src/AtomUI.Controls.Shared/Data/DataMemberAccessors/GenerateDataMemberAccessorsAttribute.cs`
  - `src/AtomUI.Controls.Shared/Data/DataMemberAccessors/IDataMemberAccessor.cs`
  - `src/AtomUI.Controls.Shared/Data/DataMemberAccessors/IDataMemberAccessorDescriptor.cs`
  - `src/AtomUI.Controls.Shared/Data/DataMemberAccessors/DataMemberAccessor.cs`
  - `src/AtomUI.Controls.Shared/Data/DataMemberAccessors/DataMemberAccessorDescriptor.cs`
  - `src/AtomUI.Controls.Shared/Data/DataMemberAccessors/DataMemberAccessorRegistry.cs`
  - `src/AtomUI.Controls.Shared/Data/DataMemberAccessors/DataMemberValueComparer.cs`
  - `src/AtomUI.Controls.Shared/Data/DataMemberAccessors/DataMemberRuntimeComparerFactory.cs`
  - `src/AtomUI.Controls.Shared/Data/ListCollectionViews/ListCollectionView.cs`
  - `src/AtomUI.Controls.Shared/Data/ListCollectionViews/ListSortDescription.cs`
  - `src/AtomUI.Generator/DataMemberAccessors/DataMemberAccessorGenerator.cs`
  - `src/AtomUI.Generator/DataMemberAccessors/DataMemberAccessorSourceWriter.cs`
  - `tests/AtomUI.Controls.Shared.Tests/DataMemberAccessors/GenerateDataMemberAccessorsTests.cs`
- 为什么这样改：
  - Shared collection view 的 string path sort 旧逻辑依赖 `TypeHelper.GetNestedPropertyType(...)` / `GetNestedPropertyValue(...)`，本质是运行时反射解析用户 model property；这在 NativeAOT/trimming 下不可靠。
  - AOT-safe 根治方案不是压 warning，而是让用户 model 用 `[GenerateDataMemberAccessors]` 生成编译期可见的 property metadata：path、value type、getter/setter、comparer、new item factory。
  - `ListCollectionView(source, descriptor)` 现在会把 descriptor 传给 sort 初始化；SG module initializer 注册到 `DataMemberAccessorRegistry`，手工 descriptor 也可以直接传给 view，不强制全局注册。
  - `AddNew` 优先走 descriptor 的 `NewItemFactory`，这样非泛型集合或 item type metadata 不完整时也不需要反射构造。
  - 无 descriptor 且 `RuntimeFeature.IsDynamicCodeSupported == false` 时，path sort 不再进入 `TypeHelper`，而是抛明确异常，提示加 `[GenerateDataMemberAccessors]` 或传 `IDataMemberAccessorDescriptor`。
  - 为了保留 JIT 下旧 `FromPath` 反射语义，旧反射 path 被隔离到 `RuntimeReflectionPropertyPathAccessor`；NativeAOT 路径由 runtime guard 阻断。该 legacy 类型内部有局部 `IL2026` suppression，review 重点是确认这只是 JIT fallback，不是 AOT 主路径。
- Review 重点：
  - `[GenerateDataMemberAccessors]` 命名是否准确：它生成的是 data member access metadata/accessor，不是标记某个具体 UI 模型。
  - 生成属性集合只包含可从生成代码访问的实例属性；setter 只在非 init 且可访问时生成。
  - string 排序仍使用原来的 culture comparer；显式 comparer 优先级高于生成 accessor 的默认 comparer。
  - nullable / `IComparable` 类型在无动态代码路径下不能静默返回相等；没有 AOT-safe comparer 时必须明确失败。
  - `ListCollectionView(source, descriptor)` 的 descriptor 要同时服务 `AddNew` 和 path sort。
  - 旧 JIT reflection fallback 保留原 `TypeHelper` 语义；NativeAOT 缺 accessor 时不进入旧 reflection path。
  - 资源生命周期：新增 registry 是静态 descriptor 字典，没有事件、observable、binding 或需要释放的资源；sort description 只缓存一次 accessor/comparer，不持有 view 或 source collection。
  - 性能影响：AOT/SG 路径由每次比较时的反射 property path 解析变为一次 dictionary lookup + getter delegate；JIT fallback 仍保留旧成本，不增加额外 per-item 反射。
- 验证：
  - `dotnet test tests/AtomUI.Controls.Shared.Tests/AtomUI.Controls.Shared.Tests.csproj --nologo -v:minimal` 通过，`9` 个测试全部通过。
  - `dotnet build src/AtomUI.Controls.Shared/AtomUI.Controls.Shared.csproj -c Release --no-incremental /m:1 /p:IsAotCompatible=true /p:EnableTrimAnalyzer=true /p:EnableAotAnalyzer=true /p:EnableSingleFileAnalyzer=true /nr:false --nologo -v:minimal` 通过，`0 Warning(s), 0 Error(s)`。
  - `dotnet build src/AtomUI.Generator/AtomUI.Generator.csproj -c Release --no-incremental /nr:false --nologo -v:minimal` 通过，`0 Warning(s), 0 Error(s)`。
  - 历史边界检查：AOT-RV-009 完成时 `src/AtomUI.Desktop.Controls.DataGrid` 仍有 `26 Warning(s), 0 Error(s)`；这些 DataGrid 动态 model path warning 已继续归入 AOT-RV-010 处理。
  - 覆盖场景：生成 descriptor 注册、nullable comparer null ordering、getter/setter/factory、非泛型 `ArrayList` 的 `AddNew`、descriptor-backed path sort、无动态代码下缺 accessor 明确失败、动态代码可用时保留旧 reflection fallback。

### AOT-RV-010：DataGrid 动态 model binding 与自动生成列

- 状态：`[x] 已通过`
- 主要文件：
  - `src/AtomUI.Desktop.Controls.DataGrid/Data/DataGridCollectionView.cs`
  - `src/AtomUI.Desktop.Controls.DataGrid/Data/DataGridDataMemberPathAccessor.cs`
  - `src/AtomUI.Desktop.Controls.DataGrid/Data/DataGridSortDescription.cs`
  - `src/AtomUI.Desktop.Controls.DataGrid/Data/DataGridFilterDescription.cs`
  - `src/AtomUI.Desktop.Controls.DataGrid/Data/DataGridPathGroupDescription.cs`
  - `src/AtomUI.Desktop.Controls.DataGrid/Data/DataGridDataConnection.cs`
  - `src/AtomUI.Desktop.Controls.DataGrid/DataGrid.Columns.cs`
  - `src/AtomUI.Desktop.Controls.DataGrid/Column/DataGridBoundColumn.cs`
  - `src/AtomUI.Desktop.Controls.DataGrid/Column/DataGridColumn.cs`
  - `src/AtomUI.Desktop.Controls.DataGrid/DataGrid.Rows.cs`
  - `src/AtomUI.Controls.Shared/Data/DataMemberAccessors/DataMemberAccessorRegistry.cs`
  - `src/AtomUI.Controls.Shared/Data/DataMemberAccessors/DataMemberAccessor.cs`
  - `src/AtomUI.Controls.Shared/Data/DataMemberAccessors/DataMemberAccessorDescriptor.cs`
  - `src/AtomUI.Controls.Shared/Data/DataMemberAccessors/DataMemberAccessorMetadata.cs`
  - `src/AtomUI.Generator/DataMemberAccessors/DataMemberAccessorGenerator.cs`
  - `src/AtomUI.Generator/DataMemberAccessors/DataMemberAccessorSourceWriter.cs`
  - `src/AtomUI.Desktop.Controls.DataGrid/AtomUI.Desktop.Controls.DataGrid.csproj`
  - `tests/AtomUI.Desktop.Controls.DataGrid.Tests/AtomUI.Desktop.Controls.DataGrid.Tests.csproj`
  - `tests/AtomUI.Desktop.Controls.DataGrid.Tests/DataMemberAccessors/DataGridDataMemberAccessorsTests.cs`
- 为什么这样改：
  - DataGrid 自己有一套 collection view，不能只修 shared `ListCollectionView`；旧 sort/filter/group path 会通过 `TypeHelper.GetNestedPropertyType(...)` / `GetNestedPropertyValue(...)` 反射解析用户 model property。
  - 这类动态 path 在 NativeAOT/trimming 下不能靠 suppression 通过；DataGrid 运行时集合操作和自动生成列都要接到 AOT-RV-009 的 `[GenerateDataMemberAccessors]` / `IDataMemberAccessorDescriptor` 上。
  - `DataGridCollectionView(source, descriptor)` 支持手工传 descriptor；没有手工传时会从 `DataMemberAccessorRegistry` 按 item type 查找 SG 注册的 descriptor。
  - registry 查找已从精确类型扩展为 compatible descriptor 查找并缓存结果，允许派生 model 复用 base model 上生成的 accessor，避免 NativeAOT 下比旧 reflection inherited-property 语义更窄。
  - `AddNew` 优先使用 descriptor 的 `NewItemFactory`，避免非泛型集合或 item type metadata 不完整时再通过反射构造 item。
  - 无 descriptor 且 `RuntimeFeature.IsDynamicCodeSupported == false` 时，sort/filter/group path 明确抛异常，提示添加 `[GenerateDataMemberAccessors]` 或传 `IDataMemberAccessorDescriptor`；JIT 下保留旧 reflection fallback，不改变原开发态语义。
  - 自动生成列不再创建 `new Binding(propertyName)` reflection binding；descriptor-backed path 会用 `CompiledBindingExtension + ClrPropertyInfo`，由 `IDataMemberAccessor.GetValue/SetValue` 完成读取和写回。
  - descriptor metadata 扩展为编译期生成 `DisplayName`、`AutoGenerateField`、`Order`、`ReadOnlyAttribute`、`EditableAttribute`、属性 value type、can write 和 data type read-only，覆盖旧自动列/header/sort/read-only 依赖的反射 metadata。
  - 无 descriptor 且 NativeAOT 下不再尝试反射推断 `DataType` / `PropertyInfo[]` / display name / nested property type；行为收敛为保守结果，避免 trim 后运行时崩溃或 silent wrong metadata。JIT 下旧 reflection fallback 保留。
- Review 重点：
  - `DataGridSortDescription.FromPath(...)`：descriptor accessor 优先；显式 comparer 仍优先；string 仍走 culture-sensitive comparer；非 string 走 AOT-RV-009 的 comparer 工厂，不能在 NativeAOT 下静默把不可比较类型排成全相等。
  - `DataGridFilterDescription`：`DataGridDefaultFilter` 仍通过 `PropertyPath` 取值，descriptor-backed path 应返回和旧 reflection path 相同的值。
  - `DataGridPathGroupDescription`：group key 取值改为 descriptor accessor，但 converter 和 string comparison 逻辑不变。
  - `DataGridCollectionView.AddNew`：descriptor `NewItemFactory` 只替代 item 反射构造，不改变 add/cancel/commit 流程。
  - filter 初始化按 item type、descriptor 和 filter accessor version 做惰性缓存；`FilterDescriptions` 集合变化、descriptor resolve、`PropertyPath` 变化都会使缓存失效，避免每条数据重复初始化但不牺牲动态 filter path 修改语义。
  - 自动生成列：`AutoGenerateField == false` 仍跳过；header 仍优先 display short name/name；order 插入算法复用旧逻辑；primitive data source 仍生成 self binding。
  - 自动生成列绑定：descriptor-backed binding 必须保留原 `DataGridBoundColumn` 的默认 TwoWay、`DataGridValueConverter`、cell editing 写回、`BindingNotification` error 回传和 `INotifyPropertyChanged` 更新语义。
  - `DataGridBoundColumn.SetHeaderFromBinding`、`DataGridColumn.CanUserSort`、group header display name 都改为走 `DataConnection` metadata；有 descriptor 时不用反射，无 descriptor 且 JIT 时仍走旧 helper fallback。
  - 资源生命周期：没有新增 template part binding 或外部事件订阅；自动生成列 binding 的 `INotifyPropertyChanged` 订阅仍由 `IPropertyAccessor.Subscribe/Unsubscribe/Dispose` 管理，descriptor/accessor/metadata/compatible lookup cache 是静态或 view/column 生命周期对象，不需要额外释放。
  - 性能影响：SG 路径把旧的每次 path reflection 变为一次 descriptor lookup + delegate getter；sort/filter/group description 会缓存 accessor/comparer，自动列也复用生成 getter/setter。JIT fallback 只保留旧成本，不额外叠加 per-item 反射层。
- 验证：
  - TDD red：`dotnet test tests/AtomUI.Desktop.Controls.DataGrid.Tests/AtomUI.Desktop.Controls.DataGrid.Tests.csproj --nologo -v:minimal /m:1 /nr:false --filter DataGridPathSortUsesBaseGeneratedAccessorForDerivedRowsWhenDynamicCodeIsDisabled` 曾失败，错误为派生类型找不到 base generated accessor。
  - TDD green：同一 filter 测试通过，验证派生类型可在 `isDynamicCodeSupported: false` 下复用 base generated accessor。
  - metadata 覆盖：`GeneratedAccessorsExposeDataGridMetadata` 验证 SG 输出 display name、auto generate、order、read-only、editable、value type、can write 和 data type read-only。
  - empty non-generic collection 覆盖：`DataGridDataConnectionUsesDescriptorMetadataForEmptyNonGenericCollection` 验证空 `ArrayList + DataGridCollectionView(source, descriptor)` 不靠 runtime item type 也能拿到 `DataType`、display name、property type 和 read-only。
  - binding 语义覆盖：`DataGridBoundColumnAppliesDefaultModeAndConverterToCompiledBindingExtension` 与 `DataGridBoundColumnRejectsOneWayToSourceCompiledBindingExtension` 锁住 compiled binding 下的默认 TwoWay、`DataGridValueConverter` 和 OneWayToSource 拒绝行为。
  - `dotnet test tests/AtomUI.Desktop.Controls.DataGrid.Tests/AtomUI.Desktop.Controls.DataGrid.Tests.csproj --nologo -v:minimal /m:1 /nr:false` 通过，`13` 个测试全部通过。
  - `dotnet test tests/AtomUI.Controls.Shared.Tests/AtomUI.Controls.Shared.Tests.csproj --nologo -v:minimal /m:1 /nr:false` 通过，`9` 个测试全部通过。
  - `dotnet build src/AtomUI.Desktop.Controls.DataGrid/AtomUI.Desktop.Controls.DataGrid.csproj -c Release --no-incremental /m:1 /p:IsAotCompatible=true /p:EnableTrimAnalyzer=true /p:EnableAotAnalyzer=true /p:EnableSingleFileAnalyzer=true /nr:false --nologo -v:minimal` 通过，`0 Warning(s), 0 Error(s)`。
  - 边界检查：DataGrid 自身已不再产生 `TypeHelper.GetNestedPropertyValue(...)`、nested property type、display name、item type inference、`PropertyInfo[]` 自动列相关 AOT warning。

### AOT-RV-011：Control template-part binding 替换

- 状态：`[x] 已通过`
- 主要文件：
  - `src/AtomUI.Desktop.Controls/Cascader/Cascader.cs`
  - `src/AtomUI.Desktop.Controls/ComboBox/ComboBox.cs`
  - `src/AtomUI.Desktop.Controls/Input/LineEdit.cs`
  - `src/AtomUI.Desktop.Controls/Input/TextArea.cs`
  - `src/AtomUI.Desktop.Controls/NumericUpDown/NumericUpDown.cs`
  - `src/AtomUI.Desktop.Controls/Primitives/InfoPickerInput/InfoPickerInput.cs`
  - `src/AtomUI.Desktop.Controls/Select/Select.cs`
  - `src/AtomUI.Desktop.Controls/TreeSelect/TreeSelect.cs`
  - `src/AtomUI.Desktop.Controls/Drawer/Drawer.cs`
  - `src/AtomUI.Core/Data/BindUtils.cs`
  - `tests/AtomUI.Desktop.Controls.Tests/Drawer/DrawerAotBindingTests.cs`
- 为什么这样改：
  - 很多 template-part binding 使用 `new Binding(nameof(...)) { Source = this }`，依赖 reflection binding。
  - 改成 `BindUtils.RelayBind` 可以移除字符串路径反射，同时保留绑定行为。
  - `Drawer.OpenOn` 的 TopLevel 默认来源旧逻辑不是普通 `Source=this` 绑定，而是 `RelativeSource FindAncestor TopLevel`；为了不改变语义，不能用一次性 `TopLevel.GetTopLevel(this)` 代替，需要用 `VisualLocator.Track(...)` 保留 attach/detach 后重新定位祖先的行为。
- Review 重点：
  - 每处替换都使用正确的 source/target `AvaloniaProperty`。
  - `ObjectConverters.IsNotNull` 等 converter 替换保持相同的 null/visibility 语义。
  - Two-way binding，特别是 reveal password 和 popup/dialog 属性，保持更新方向和清理行为。
  - Template detach/reapply 不泄露 binding。
- 当前 review 结论：
  - `[x] 已通过`：绝大多数 template part 替换属于 `new Binding(nameof(...)) { Source = this }` 到 `AvaloniaProperty` relay 的等价迁移，普通属性同步、converter 替换和 `LineEdit.RevealPassword` TwoWay 语义已确认。
  - `Drawer.OpenOn` 的 `TopLevel` 分支已修复：新增 `BindUtils.BindVisualAncestor(...)`，内部用 Avalonia `VisualLocator.Track(relativeTo, ancestorLevel - 1, ancestorType)` 生成 observable，再以 `BindingPriority.Template` 绑定到 `OpenOnProperty`。
  - 资源生命周期：`BindVisualAncestor(...)` 返回 `target.Bind(...)` 的 disposable；Drawer 将其加入 `_relayBindingDisposables`，`OnAttachedToVisualTree` 重新绑定前会 dispose 旧集合，`OnDetachedFromVisualTree` 也会 dispose，因此 `VisualLocator` 对 `AttachedToVisualTree/DetachedFromVisualTree` 的订阅有明确释放路径。
  - 性能影响：相对旧 `RelativeSource FindAncestor`，仍是一次 visual ancestor tracker + property binding；移除了 `ReflectionBinding`/字符串 path，没有新增 per-frame 工作，也没有 C# 动态创建视觉元素。
- 验证：
  - 对每个变更控件做 template apply smoke test。
  - 检查 clear button、reveal password、selection handle、add-on、drawer nesting 交互。
  - TDD red：`dotnet test tests/AtomUI.Desktop.Controls.Tests/AtomUI.Desktop.Controls.Tests.csproj --nologo -v:minimal /m:1 /nr:false --filter Drawer_Default_OpenOn_Does_Not_Use_One_Time_TopLevel_Lookup` 曾失败，原因是 `Drawer.cs` 仍包含 `TopLevel.GetTopLevel(this)`。
  - TDD green：`dotnet test tests/AtomUI.Desktop.Controls.Tests/AtomUI.Desktop.Controls.Tests.csproj --nologo -v:minimal /m:1 /nr:false` 通过，`2` 个测试全部通过。
  - `dotnet build src/AtomUI.Core/AtomUI.Core.csproj -c Release --no-incremental /m:1 /p:IsAotCompatible=true /p:EnableTrimAnalyzer=true /p:EnableAotAnalyzer=true /p:EnableSingleFileAnalyzer=true /nr:false --nologo -v:minimal` 通过，`0 Warning(s), 0 Error(s)`。
  - `dotnet build src/AtomUI.Desktop.Controls/AtomUI.Desktop.Controls.csproj -c Release --no-incremental /m:1 /p:IsAotCompatible=true /p:EnableTrimAnalyzer=true /p:EnableAotAnalyzer=true /p:EnableSingleFileAnalyzer=true /nr:false --nologo -v:minimal` 通过，`0 Warning(s), 0 Error(s)`。

### AOT-RV-012：NavMenu node binding

- 状态：`[x] 已通过`
- 主要文件：
  - `src/AtomUI.Desktop.Controls/NavMenu/NavMenu.cs`
  - `src/AtomUI.Desktop.Controls/NavMenu/NavMenuItem.cs`
- 为什么这样改：
  - `INavMenuNode` item binding 原来对任意对象使用字符串路径。
  - AOT-safe 替代使用强类型 getter-based relay binding。
- Review 重点：
  - Node 的 icon、enabled state、header template 能在 node 触发 `PropertyChanged` 后更新。
  - 容器复用时会释放旧 node binding。
  - 当 node header template 为空时，`ItemTemplate` fallback 仍然正确。
- 当前 review 结论：
  - `[x] 已通过`：`Icon`、`IsEnabled`、`HeaderTemplate` 已从字符串 path 绑定改为强类型 getter-based relay，`nameof(...)` 仅用于 `PropertyChanged` 属性名过滤，不再用于反射取值。
  - `[x] 已通过`：`NavMenuNode` 继承 `AvaloniaObject`，Avalonia 12 下可通过 `INotifyPropertyChanged` 通知驱动 getter observable 更新。
  - `[x] 已通过`：`NavMenuItem.ResetNodeBindingDisposables()` 会在容器重新 prepare 前释放旧 node 订阅，`ClearContainerForItemOverride` 也会释放，避免容器复用时持有旧 node。
  - `[x] 已通过`：`ItemKey` 仍是 prepare 时赋值，`HeaderTemplate` 仍保持“node 初始 template 非空则绑定，否则使用 `ItemTemplate` fallback”的原逻辑。
- 验证：
  - 使用可变 `INavMenuNode` 和容器复用场景测试 NavMenu。

### AOT-RV-013：ReactiveWindow AOT 边界

- 状态：`[x] 已通过`
- 主要文件：
  - `src/AtomUI.Desktop.Controls/Window/ReactiveWindow.cs`
  - `tests/AtomUI.Desktop.Controls.Tests/Window/ReactiveWindowAotTests.cs`
  - `tests/AtomUI.Desktop.Controls.Tests/AvaloniaTestApp.cs`
  - `tests/AtomUI.Desktop.Controls.Tests/AtomUI.Desktop.Controls.Tests.csproj`
- 为什么这样改：
  - `ReactiveWindow<TViewModel>` 原来在构造函数里调用 `this.WhenActivated(disposables => { })`；lambda 虽然为空，但 ReactiveUI view-side `WhenActivated` 会进入内部 view activation 管线，并且该路径使用 expression/string member chain，带有 trim/AOT 风险。
  - 直接删除空函数会改变语义：实现 `IActivatableViewModel` 的 `ViewModel` 不再由 Window 生命周期驱动，VM 内部的 `this.WhenActivated(...)` 可能不执行。
  - 当前改造用 Avalonia `Loaded/Unloaded` 显式桥接 `IActivatableViewModel.Activator.Activate()`，从根上绕开 ReactiveUI view-side reflection path，同时保留 VM activation 能力。
- Review 重点：
  - `[x] 已通过`：`ReactiveWindow` 不再包含 `RequiresUnreferencedCode`，也不再调用 ReactiveUI view-side `WhenActivated`。
  - `[x] 已通过`：`Loaded` 时只激活一次当前 VM，重复 `Loaded` 不重复调用 `Activator.Activate()`。
  - `[x] 已通过`：`Unloaded` 时释放 activation disposable，重复 `Unloaded` 不重复释放。
  - `[x] 已通过`：Window active 状态下切换 `ViewModel` 会先释放旧 VM activation，再激活新 VM，保留原来的 VM activation 语义。
  - `[x] 已通过`：卸载过程发生 reentrancy 时不会在 inactive 状态误激活新 VM，也不会丢失/覆盖 activation disposable 导致资源泄露。
  - `[x] 已通过`：`DataContext` 与 `ViewModel` 的原同步逻辑保留。
  - `[x] 已通过`：显式空构造函数已删除；C# 仍提供 public parameterless constructor，用户构造方式不变。
- 验证：
  - `dotnet test tests/AtomUI.Desktop.Controls.Tests/AtomUI.Desktop.Controls.Tests.csproj --nologo -v:minimal /m:1 /nr:false --filter ReactiveWindowAotTests` 通过，`4 passed`。
  - `dotnet test tests/AtomUI.Desktop.Controls.Tests/AtomUI.Desktop.Controls.Tests.csproj --nologo -v:minimal /m:1 /nr:false` 通过，`6 passed`。
  - `dotnet build src/AtomUI.Desktop.Controls/AtomUI.Desktop.Controls.csproj -c Release --no-incremental /m:1 /p:IsAotCompatible=true /p:EnableTrimAnalyzer=true /p:EnableAotAnalyzer=true /p:EnableSingleFileAnalyzer=true /nr:false --nologo -v:minimal` 通过，`0 Warning(s), 0 Error(s)`。
  - `git diff --check -- src/AtomUI.Desktop.Controls/Window/ReactiveWindow.cs tests/AtomUI.Desktop.Controls.Tests/AtomUI.Desktop.Controls.Tests.csproj tests/AtomUI.Desktop.Controls.Tests/AvaloniaTestApp.cs tests/AtomUI.Desktop.Controls.Tests/Window/ReactiveWindowAotTests.cs` 通过。

### AOT-RV-014：Generated files 与 source generator packaging

- 状态：`[x] 已通过`
- 主要文件：
  - `src/AtomUI.Generator/AtomUI.Generator.csproj`
  - 生成的 `ControlTokenTypePool.g.cs` 文件
  - 生成的 `LanguageProviderPool.g.cs` 文件
  - 生成的 `TokenValueConverterRegistry.g.cs` 文件
  - `src/AtomUI.Icons.AntDesign/AntDesignIconProvider.Factory.g.cs`
- 为什么这样改：
  - AOT-safe metadata 依赖生成产物确定且被正确包含。
  - Generator packaging 不能在 NuGet 场景下漏掉 analyzer/source output。
  - `AtomUI.Generator` 是 Roslyn source generator，不是要 NativeAOT 发布的运行时库；外层 AOT analyzer 属性应作用在消费 generator 的 `src/` 库项目上，不应误传到 generator 项目自身。
- Review 重点：
  - `[x] 已通过`：`AtomUI.Generator.csproj` 使用 `TreatAsLocalProperty` 隔离 `IsAotCompatible`、`EnableAotAnalyzer`、`EnableTrimAnalyzer`、`EnableSingleFileAnalyzer`，并在 generator 项目内显式设为 `false`。
  - `[x] 已通过`：`AtomUI.Generator` pack 后包含 `analyzers/dotnet/cs/AtomUI.Generator.dll`，NuGet 场景下 generator 会作为 analyzer/source generator 参与消费项目构建。
  - `[x] 已通过`：当前 `src/**/GeneratedFiles/AtomUI.Generator/**` 由当前 generator 稳定复现，solution Release build 后生成文件 hash 未变化。
  - `[x] 已通过`：`DefaultIconPackageGenerator` 与 `AntDesignIconProvider.Factory.g.cs` 一致，`GetIconType` 生成 `switch (kind)` statement，`CreateIcon` 生成 switch expression。
  - `[x] 已通过`：生成文件只作为提交产物/审阅产物保留，项目通过 `Compile Remove="$(CompilerGeneratedFilesOutputPath)/**/*.cs"` 避免把 `GeneratedFiles` 目录源码重复编译。
- 验证：
  - `dotnet pack src/AtomUI.Generator/AtomUI.Generator.csproj -c Release /nr:false --nologo -v:minimal` 通过，并确认 nupkg 内包含 `analyzers/dotnet/cs/AtomUI.Generator.dll`。
  - `dotnet build src/AtomUI.Generator/AtomUI.Generator.csproj -c Release /p:IsAotCompatible=true /p:EnableTrimAnalyzer=true /p:EnableAotAnalyzer=true /p:EnableSingleFileAnalyzer=true /nr:false --nologo -v:minimal` 通过，`0 Warning(s), 0 Error(s)`。
  - `dotnet msbuild src/AtomUI.Generator/AtomUI.Generator.csproj -nologo -getProperty:IsAotCompatible -getProperty:EnableAotAnalyzer -getProperty:EnableTrimAnalyzer -getProperty:EnableSingleFileAnalyzer -p:IsAotCompatible=true -p:EnableTrimAnalyzer=true -p:EnableAotAnalyzer=true -p:EnableSingleFileAnalyzer=true` 确认四个属性在 generator 项目内均为 `false`。
  - `dotnet restore AtomUI.slnx -p:Configuration=Release --force /nr:false --nologo -v:minimal` 后执行 `dotnet build AtomUI.slnx -c Release --no-restore --no-incremental /m:1 /nr:false --nologo -v:minimal`，生成文件 hash 对比结果为 `GENERATED_FILES_STABLE`。
  - `git diff --check -- src/AtomUI.Generator src/AtomUI.Core/GeneratedFiles src/AtomUI.Controls.Shared/GeneratedFiles src/AtomUI.Controls/GeneratedFiles src/AtomUI.Desktop.Controls/GeneratedFiles src/AtomUI.Desktop.Controls.ColorPicker/GeneratedFiles src/AtomUI.Desktop.Controls.DataGrid/GeneratedFiles src/AtomUI.Icons.Shared/DefaultIconPackageGenerator.cs src/AtomUI.Icons.AntDesign/AntDesignIconProvider.Factory.g.cs` 通过。

### AOT-RV-015：剩余 controlgallery AOT warning

- 状态：`[ ] 待 review`
- 主要文件：
  - `controlgallery/AtomUIGallery/**`
- 为什么这样改：
  - 全解决方案 analyzer 曾报告 Gallery warning，最后剩余来自 ReactiveUI expression API。
  - Gallery localization provider 原本没有显式构造函数，编译器会生成隐式 `base()` 调用，触发 `LanguageProvider()` 的反射构造 warning。
  - Gallery localization provider 已改成 `partial`，由 Language SG 读取 `[LanguageProvider(LanguageCode, LanguageId)]` 元数据并生成显式 `base(LanguageCode, LanguageId)` 构造，保留旧 metadata 语义但不再走运行时反射构造。
  - `IconGallery` 原本通过 `Assembly.GetTypes()` 扫描 AntDesign icon 类型，再用 `Activator.CreateInstance(Type)` 创建 icon；现改为由 icon generator 生成 `AntDesignIconCatalog`，Gallery 只消费 catalog。
  - `ReactiveUserControl<T>` 构造阶段会进入 ReactiveUI view activation 管线，`WhenActivated` / `OneWayBind` / `BindCommand` / `WhenAnyValue` 这批表达式 API 会触发 trim/AOT warning。Gallery 如果纳入 AOT 发布范围，就不能靠 suppression 压住。
  - ReactiveUI 本身的 `ViewModelActivator.AddActivationBlock` 在当前包里是 `internal`，不能直接调用；VM 侧需要用公开的 `Activated` observable 或显式属性 setter 保留旧效果。
  - 这些 warning 不阻塞 `src/` library AOT 状态，但 Gallery 已按 AOT-clean 目标处理。
- Review 重点：
  - `[x] 已通过`：`222` 个 Gallery raw localization provider 只增加 `partial`，不修改资源文本、resource kind 或 `BuildResourceDictionary` 逻辑。
  - `[x] 已通过`：`LanguageProviderConstructorSourceWriter` 只对 `partial && !HasParameterlessConstructor` 的 provider 生成构造函数，避免和已有手写构造函数冲突。
  - `[x] 已通过`：`AntDesignIconCatalog.g.cs` 由现有 icon generator 的 `IconFiles` 生成，和 `AntDesignIconProvider.Factory.g.cs` 同源；每个 descriptor 保存 `Name`、`ThemeType`、`Kind`、`IconType` 和直接 `new XxxIcon()` 的 creator。
  - `[x] 已通过`：`IconGallery` 仍保留旧排序、theme 过滤、搜索过滤和懒加载语义，但不再运行时扫描 assembly 或反射构造 icon。
  - `[ ] 待 review`：`GalleryReactiveUserControl<TViewModel>` 替换 Gallery 里的 `ReactiveUserControl<TViewModel>`；保留 `IViewFor<TViewModel>`、`ViewModel`/`DataContext` 双向同步、Loaded/Unloaded activation 和 `IActivatableViewModel.Activator.Activate()` 语义，Unload 时先 deactivate VM、再释放 view disposables；`WhenActivated` 返回的 disposable 会移除 callback 并释放该 callback 当前 activation scope，避开 ReactiveUI view-side reflection activation path。
  - `[ ] 待 review`：空 `this.WhenActivated(...)` 删除；非空 activation block 保留在 view 侧，但调用目标改为 `GalleryReactiveUserControl.WhenActivated`，disposable 仍在 Unloaded 时释放。
  - `[ ] 待 review`：`GalleryBindingUtils.OneWay` 替换 Gallery 的 ReactiveUI `OneWayBind`；强类型 AvaloniaProperty 走 `BindUtils.RelayBind`，非泛型 AvaloniaProperty 走本地 `INotifyPropertyChanged` observable + `target.Bind(...)`，保留初始值、PropertyChanged 更新和 dispose 释放语义。
  - `[ ] 待 review`：`GalleryBindingUtils.BindCommand` 替换 ReactiveUI expression `BindCommand`；设置 `Button.CommandProperty`，dispose 时恢复进入绑定前的 command，disposable 挂在 activation scope 下。
  - `[ ] 待 review`：`AvatarViewModel` / `CaseNavigationViewModel` 的 VM activation 初始化改为监听 `Activator.Activated`，保持“VM 被 view 激活时执行”的语义，不再调用会触发 AOT warning 的 VM `WhenActivated` extension。
  - `[ ] 待 review`：`BadgeViewModel` / `QRCodeViewModel` 移除 `WhenAnyValue` / `ToProperty`；Badge 反应逻辑进入 setter，QRCode 的 `IconSize` 改为派生属性并在 `Size` setter 里 raise，command canExecute 用 `BehaviorSubject<bool>` 显式更新。
- 验证：
  - `dotnet test tests/AtomUI.Generator.Tests/AtomUI.Generator.Tests.csproj --filter LanguageProviderConstructorGeneratorTests --nologo -v:minimal` 通过，`Failed: 0, Passed: 1`。
  - `dotnet test tests/AtomUI.Icons.Shared.Tests/AtomUI.Icons.Shared.Tests.csproj --filter IconCatalogGeneratorTests --nologo -v:minimal` 通过，`Failed: 0, Passed: 1`。
  - `dotnet test tests/AtomUIGallery.Tests/AtomUIGallery.Tests.csproj --filter GalleryReactiveUserControlTests --nologo -v:minimal` 通过，`Failed: 0, Passed: 7`。
  - `dotnet build controlgallery/AtomUIGallery/AtomUIGallery.csproj -c Release --no-incremental /m:1 /nr:false --nologo -v:minimal` 通过，`0 Warning(s), 0 Error(s)`。
  - 当前 Gallery AOT analyzer 能成功退出，`0 Warning(s), 0 Error(s)`。
  - 当前全解决方案 AOT analyzer 能成功退出，`0 Warning(s), 0 Error(s)`。
  - `LanguageProvider()` warning 已清零；Gallery `222` 个 raw localization provider 对应生成 `222` 个显式构造函数。
  - `IconGallery` warning 已清零：`Assembly.GetTypes()` 和 `Activator.CreateInstance(Type)` 不再出现在 Gallery/full solution AOT analyzer 日志中。
  - ReactiveUI expression API warning 已清零：旧 `.OneWayBind(...)`、ReactiveUI `.BindCommand(...)`、`.WhenAnyValue(...)`、`.ToProperty(...)`、`ReactiveUserControl<T>` 在 Gallery 源码里不再残留。

### AOT-RV-016：Gallery.Desktop NativeAOT publish 配置

- 状态：`[ ] 待 review`
- 主要文件：
  - `controlgallery/AtomUIGallery.Desktop/AtomUIGallery.Desktop.csproj`
  - `src/AtomUI.Generator/AtomUI.Generator.csproj`
- 为什么这样改：
  - `dotnet publish ... -p:PublishAot=true` 会把 `PublishAot` 作为 MSBuild global property 传给 ProjectReference 图；`AtomUI.Generator` 是 `netstandard2.0` Roslyn source generator，不是运行时发布目标，因此会触发 `NETSDK1207`。
  - `AtomUI.Generator` 需要把 `PublishAot`、`PublishTrimmed`、`PublishSingleFile`、`SelfContained`、`RuntimeIdentifier` 也纳入 `TreatAsLocalProperty` 并在自身关闭，避免外层应用发布属性污染 generator 项目。
  - macOS NativeAOT 链接阶段需要找到 Homebrew OpenSSL；本机 `libssl/libcrypto` 在 `/opt/homebrew/lib`，默认 clang/ld 搜索路径没有覆盖。
  - 历史验证中，保留粗粒度 `Roots.xml` 时默认 Apple linker 曾触发 `too many large addends` 断言；删除 Desktop `Roots.xml` 后，默认 linker 已能完成当前 NativeAOT 产物链接，因此不再固化 `-ld_classic`。
  - `Roots.xml` 原本是整包 `preserve="All"` 的粗粒度兜底，会掩盖真实 trim 边界并扩大 NativeAOT 保留面。当前 Gallery/AtomUI 内置路径已通过 AOT 改造消除关键动态发现路径，Desktop 这组 root 可以整体删除。
- Review 重点：
  - `AtomUI.Generator` 的 publish/runtime 属性隔离只影响 generator 项目自身，不应改变消费项目的 AOT analyzer 或 source generator 执行。
  - `AtomUIGallery.Desktop.csproj` 的 linker 参数只在 `PublishAot=true` 且 macOS 环境生效。
  - `-L/opt/homebrew/lib` 使用 `Exists('/opt/homebrew/lib/libssl.dylib')` 条件，避免非 Homebrew/macOS 环境硬失败。
  - `-ld_classic` 不应作为默认项目配置；如果某个 Xcode/NativeAOT 组合再次触发 linker bug，应作为本机或 CI 发布参数单独处理。
  - 删除 `Roots.xml` 后不能影响真实需要保留的 runtime assembly；需要用真实 NativeAOT publish 和启动 smoke test 证明 AXAML、主题、图标、语言资源等路径仍能加载。
- 验证：
  - 初始失败：`dotnet publish controlgallery/AtomUIGallery.Desktop/AtomUIGallery.Desktop.csproj -c Release -r osx-arm64 -p:PublishAot=true --self-contained true --nologo -v:minimal` 报 `NETSDK1207`，原因是 `PublishAot=true` 传入 `AtomUI.Generator`。
  - 修复 generator 属性隔离后，NativeAOT 进入 native code 阶段，但首次链接失败于 `ld: library 'ssl' not found`。
  - 使用 `LIBRARY_PATH=/opt/homebrew/lib` 后 OpenSSL 问题消失，但在保留粗粒度 `Roots.xml` 的历史状态下，Apple ld 报 `too many large addends` 断言。
  - 历史上加入 `-ld_classic` 后可绕过该 linker 断言，但该参数已被 Apple 标记 deprecated，不适合长期固化。
  - 完整 NativeAOT 发布：`dotnet publish controlgallery/AtomUIGallery.Desktop/AtomUIGallery.Desktop.csproj -c Release -r osx-arm64 -p:PublishAot=true --self-contained true --nologo -v:minimal` 通过，输出 `output/bin/Release/net10.0/osx-arm64/publish/`。
  - `file output/bin/Release/net10.0/osx-arm64/publish/AtomUIGallery.Desktop` 确认产物为 `Mach-O 64-bit executable arm64`。
  - 删除 Desktop `Roots.xml` 和 `TrimmerRootDescriptor` 后重新发布：`dotnet publish controlgallery/AtomUIGallery.Desktop/AtomUIGallery.Desktop.csproj -c Release -r osx-arm64 -p:PublishAot=true --self-contained true --nologo -v:minimal` 通过。
  - 删除 Desktop `Roots.xml` 后，裸 AOT 可执行文件大小为 `53,132,072 bytes`，`ls -lh` 显示约 `51M`。
  - 删除 Desktop `Roots.xml` 后，用 `.app` bundle 启动最新 NativeAOT 产物，进程运行超过 60 秒，没有终端异常输出；随后手动结束验证进程。
  - 移除 `-ld_classic` 后，重新执行 `dotnet clean controlgallery/AtomUIGallery.Desktop/AtomUIGallery.Desktop.csproj -c Release -r osx-arm64 --nologo -v:minimal && dotnet publish controlgallery/AtomUIGallery.Desktop/AtomUIGallery.Desktop.csproj -c Release -r osx-arm64 -p:PublishAot=true --self-contained true --nologo -v:minimal` 通过。
  - 移除 `-ld_classic` 后，裸 AOT 可执行文件大小为 `53,082,536 bytes`，`ls -lh` 显示约 `51M`。
  - 移除 `-ld_classic` 后，直接启动裸 AOT 可执行文件，进程运行超过 30 秒，没有终端异常输出；随后手动结束验证进程。
  - `git diff --check` 通过。
  - 当前剩余 warning：`ReactiveUI` / `ReactiveUI.Avalonia` 包内部 IL2104/IL3053 汇总；macOS linker 报 Homebrew dylib 构建版本高于链接目标版本。
