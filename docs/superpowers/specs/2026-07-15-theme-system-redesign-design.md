# AtomUI 主题系统重构设计

## 1. 背景与结论

AtomUI 已经实现 Ant Design 的 Seed Token、Map Token、Alias Token、Component Token、默认算法、暗色算法、紧凑算法和组件级 `EnableAlgorithm` 语义。这些领域模型应当保留。

当前主要问题不在 Token 数值或算法本身，而在定义解析、Token 编译、资源发布、主题切换、局部作用域和事件派发被组合在同一批可变对象中。静态内置主题和首次局部覆盖可以工作，但失败恢复、嵌套继承、运行期更新、自定义主题和组件级共享 Token 缺少一致性保证。

本次设计参考 Ant Design 6.5.1 和 `@ant-design/cssinjs` 2.1.2。借鉴的是父配置合并、稳定主题身份、缓存 Token 结果和明确生命周期，不照搬 React Context 或 CSS-in-JS。

## 2. 重构目标

- 保留现有 Seed、Map、Alias、ControlToken 类型、Token 数值和算法实现。
- 主题定义只解析一次，解析阶段不修改运行时 Theme。
- 全局主题和局部主题使用同一套纯 Token 编译器。
- 主题切换先完整准备目标快照，再在 UI 线程原子提交。
- `ThemeConfigProvider` 默认继承父主题，与 Ant Design `inherit = true` 一致。
- 组件级 Token 只影响该组件自己的样式计算，不形成控件子树资源作用域。
- 每个全局主题变体或局部 Provider 只拥有一个可替换的资源层。
- 主题事件只描述已定义的状态转换，并携带完整上下文和异常。
- 保持 NativeAOT 兼容，不新增字符串反射 Binding 或运行时类型扫描。
- 允许分阶段迁移，任何阶段都能独立测试和回滚。

## 3. 完整问题清单

### 3.1 主题切换重入与多状态源

- `ThemeVariant`、`Application.ActualThemeVariant`、`Application.RequestedThemeVariant`、`ActivatedTheme`、`IsDarkThemeMode` 和 `IsCompactThemeMode` 都参与决定当前主题。
- `ConfigureThemeVariant()` 回写 Dark/Compact 布尔属性；属性回调又改写 `RequestedThemeVariant`，同步触发新的主题切换。
- Dark+Compact 首次启动可能经历 `Dark-Compact -> Dark -> Dark-Compact`，随后 `AttachApplication()` 再重复激活一次。
- 同一 ThemeVariant 没有 no-op 判断，重复激活仍派发生命周期事件。
- 没有 UI 线程验证、重入保护、请求队列或 transition id。

### 3.2 加载和激活不是事务

- `Theme.Load()` 直接修改在线 `ThemeDefinition`、`DesignToken`、ControlToken 和 `ResourceDictionary`。
- 解析、转换、计算、资源构建或事件订阅者失败后没有回滚。
- Application 的实际变体可能已经改变，而 `ActivatedTheme` 仍指向旧主题。
- 失败重试会复用已经被部分修改的 Token 和资源字典。
- 默认字体通过 `ThemeLoaded` 事件事后修改资源字典，造成 SharedToken 与资源值可能不一致。
- Motion 和 Wave 开关直接修改已发布主题字典，破坏快照语义。

### 3.3 定义发现和解析职责混乱

- 每个主题文件创建四个 `Theme` 对象，每个变体首次加载时重新读取同一个 XML。
- 主题发现阶段无法可靠获得 DisplayName、算法和诊断信息。
- `IsDefault` 没有参与默认主题选择。
- 定义中的算法只用于 `IsPrimary` 比较，不控制实际编译顺序。
- 文件系统扫描顺序、重复主题和无效主题缺少结构化诊断。

### 3.4 XML 解析器存在正确性缺陷

- `ReadElementContentAsString()` 已将 reader 移动到后继节点，外层循环再次 `Read()`，导致无空白 XML 跳过元素。
- 正文 Token 在读取正文后才读取 `IsShared`，读取的是错误节点的属性。
- 空元素没有统一处理，元素栈和上下文状态可能不平衡。
- 空文档、缺失根节点和未完成解析没有最终验证。
- 重复 Token、重复 ControlToken 和未知元素产生不一致的异常。
- 未知共享 Token 和未知 ControlToken id 会被静默忽略。
- `double.Parse`、`float.Parse` 等使用当前 Culture，主题文件不可移植。
- 错误信息可能混淆属性名和元素名，失败事件也不携带原始异常。

### 3.5 全局和局部编译逻辑重复

- `Theme.BuildThemeResource()` 与 `ThemeConfigProvider.CalculateTokenResources()` 重复 Seed、Map、Alias、组件算法和资源构建逻辑。
- 两条路径已经拥有不同的生命周期、错误处理和更新语义。
- 编译直接修改输入 Token，结果依赖此前调用历史，不是确定性的纯函数。
- `AbstractDesignToken.GetTokenValue()` 的值缓存可能在后续直接属性修改后过期。
- `Clone()` 对集合属性为浅复制，不能作为严格隔离边界。

### 3.6 `ThemeConfigProvider` 不符合 Ant Design

- Provider 从 `new DesignToken()` 开始，不读取父 Provider 或当前全局快照。
- 未覆盖 Token 回到 AtomUI 默认值，而不是继承父主题。
- Provider 在全局暗色主题中只设置一个颜色时，会重新生成默认亮色 Token。
- `Algorithms`、`SharedTokenSetters` 和 `ControlTokenInfoSetters` 使用可变 `List<T>`；集合内容修改不触发 StyledProperty 变化。
- 重算复用 `_sharedToken`，删除 Seed override 后旧值可能继续保留。
- 每次重算向 `MergedDictionaries` 追加字典，不替换旧层。
- 编译失败可能留下“新 Token 对象 + 旧可见资源”的混合状态。
- 父 Provider 更新和全局主题切换不会触发子 Provider 重算。

### 3.7 组件级共享 Token 作用域语义错误

Ant Design 的 `theme.components.Button` 是 Button 样式计算的私有 Token 输入，只影响 Button 自己的 style hook 和内部子样式。它不向 Button 的 Content 或任意后代组件传播。

当前 `ControlTokenResourcesScopeHostExtensions` 在控件 Attach 时把共享 Token 差量复制到 `control.Resources`：

- Button 的组件 override 会覆盖整个 Button 逻辑子树。
- 用户 Content 和嵌套控件可能读取到 Button 的私有 Token。
- 每个控件实例创建自己的 ResourceDictionary。
- 差量只在 Attach 时复制，全局切换和局部重算后不会更新。
- Attach/Detach、重复注册和资源清理形成额外生命周期负担。
- `ResourceCatalog` 参数没有进入实际查找键，组件 id 存在潜在冲突边界。
- `SharedTokenResourceValue` 在 XAML 加载时从 Application 静态取值，绕过局部 scope、组件 Effective Token 和后续主题更新。

该机制最终必须删除，不能通过增加更多事件订阅作为长期修复。

### 3.8 事件模型不可靠

- `ThemeAboutToLoad` 位于 `try` 外，`ThemeLoaded` 位于 `try` 内。
- `ThemeLoaded` 订阅者异常会触发 `ThemeLoadFailed`，即使 Theme 已经标记为 Loaded。
- `ThemeChanged` 早于 Dark/Compact、Motion 和 Wave 状态同步。
- `ThemeAboutToChange` 不携带目标主题和切换原因。
- `ThemeLoadFailed` 不携带异常。
- `ThemeCreated` 在 ThemeManager 发布前派发，初始创建基本不可观察。
- 未实现实际卸载，却派发 `ThemeUnloaded`。
- 同步事件允许订阅者重入主题切换，订阅者异常会破坏主题事务。
- 这些事件不属于公共 `IThemeManager` 契约，外部语义不明确。

### 3.9 资源、缓存和性能模型粗放

- 每个局部 Provider 实例化并计算所有已注册 ControlToken；Gallery 当前配置约 89 个。
- 相同定义、算法和 override 没有共享缓存。
- 同一主题文件最多被四个变体重复解析。
- 首次切换到未加载变体时，在 UI 线程同步进行文件 IO 和全量计算。
- ThemeDictionaries 和局部 MergedDictionaries 只增长，不存在真实卸载或淘汰语义。
- 多个 Button、Tag 等控件订阅全局 ThemeChanged，只为重新读取 Token。

### 3.10 可变 API 和测试缺口

- `ITheme` 和 `IThemeConfigProvider` 暴露可变 DesignToken、Dictionary 和 List，没有版本或变更协议。
- `ThemeManager.Current` 和 `Activator.CreateInstance` 增加全局耦合和测试成本。
- 现有主题测试主要验证 ThemeVariant 名称归一化和首次静态作用域颜色。
- 缺少解析格式、Culture、事件顺序、失败回滚、重入、嵌套继承、动态更新、资源层数量、组件隔离和生命周期测试。

## 4. Ant Design 语义基线

### 4.1 全局主题

全局 `token` 先与父配置合并，再由有序算法链生成 Map 和 Alias Token。Map/Alias 显式 override 在派生后应用。

### 4.2 嵌套主题

- `inherit` 默认是 `true`。
- 子 Provider 的 token、components 和其他主题配置与父配置合并。
- `inherit = false` 时从默认主题配置开始，但仍保留明确规定的基础配置。

### 4.3 组件主题

- Component Token 和 Design Token override 只供目标组件样式使用。
- `algorithm = false` 时仅直接覆盖，不重新派生 Seed。
- `algorithm = true` 时继承当前全局算法并重新派生组件 Effective Token。
- 组件可以提供独立算法链，覆盖全局算法。
- 子组件按自己的 component id 取 Token，不继承父组件的私有 Effective Token。

## 5. 目标架构

```text
ThemeDefinitionParser
        |
        v
ThemeDefinition + Diagnostics
        |
        v
ThemeCatalog -------- ThemeSnapshotCache
        |                     |
        +------> ThemeCompiler+
                              |
                              v
                         ThemeSnapshot
                              |
              +---------------+----------------+
              |                                |
              v                                v
       ThemeCoordinator                 ThemeConfigProvider
              |                                |
              v                                v
    Global ThemeResourceLayer          Scoped ThemeResourceLayer
              |                                |
              +---------------+----------------+
                              v
                     Avalonia DynamicResource
```

### 5.1 `ThemeDefinitionParser`

职责只有 `Stream -> ThemeDefinitionParseResult`。

- 使用带 `DtdProcessing = Prohibit` 的 XmlReader 加载 `XDocument`，保留行列信息。
- 不持有 `Theme`、ThemeManager 或 ResourceDictionary。
- 输出不可变 ThemeDefinition 和结构化 diagnostics。
- Token 字符串转换留在 Compiler，Parser 只验证 XML 结构和基础字段。
- 算法顺序使用 `IReadOnlyList<ThemeAlgorithm>` 保存，不转换为 HashSet。
- Value 属性和正文必须恰好提供一个非空值。
- 重复键、未知全局 Token、已知组件的未知 Token 为错误。
- 未注册组件 id 产生 warning，允许同一主题文件服务于未加载的可选包。
- 所有诊断包含 code、severity、file、line、column、path 和 message。

### 5.2 `ThemeCatalog`

- 每个文件只读取和解析一次。
- 存储 `ThemeDescriptor`，包括 id、路径、来源优先级、定义和 diagnostics。
- 自定义目录优先级保持现有行为，但重复 id 产生诊断。
- 如果 Builder 显式指定主题，以 Builder 为准；否则使用唯一 `IsDefault=true` 定义。
- 内置默认主题解析失败时启动失败；未选中的自定义主题错误只标记为 unavailable。
- 编译缓存键至少包含 definition identity、ordered algorithms、normalized overrides、parent snapshot version 和 registration version。

### 5.3 `ThemeCompiler`

唯一入口：

```csharp
ThemeCompileResult Compile(ThemeCompileRequest request);
```

`ThemeCompileRequest` 包含定义、父快照、算法、全局 override、组件 override、ControlToken registrations 和运行期 override。

编译顺序：

```text
parent/default seed
  + global seed override
  -> ordered algorithms
  + global map override
  -> alias calculation
  + global alias override
  -> shared effective token
  -> each component effective shared token
  -> each component token calculation
  + component token override
  -> immutable resource maps
```

每次编译创建新的 Token 图，不复用上一次发布对象。编译失败只返回 diagnostics/exception，不修改在线状态。

### 5.4 `ThemeSnapshot`

```text
ThemeSnapshot
├── Id / Version / Algorithms / IsDark
├── SharedToken
├── SharedResources
└── Components[ComponentTokenIdentity]
    ├── EffectiveSharedToken
    ├── SharedResourceDelta
    ├── ControlToken
    └── ControlResources
```

Snapshot 发布后按逻辑不可变。新内部代码不得修改其中 Token；现有返回可变 DesignToken 的公共兼容 API 由 adapter 暂时保留，并在后续版本迁移到只读视图。

### 5.5 `ThemeTokenResourceProvider`

实现为 Avalonia `ResourceProvider` 子类，持有一个 ThemeSnapshot：

```csharp
internal sealed class ThemeTokenResourceProvider : ResourceProvider
{
    public ThemeSnapshot Snapshot { get; }
    public override bool HasResources => true;
    public override bool TryGetResource(object key, ThemeVariant? theme, out object? value);
    internal void PrepareSnapshot(ThemeSnapshot snapshot);
    internal void PublishSnapshotChanged();
    public void ReplaceSnapshot(ThemeSnapshot snapshot);
}
```

- `TryGetResource` 直接查询预构建资源 map。
- `PrepareSnapshot` 只替换引用，供 Coordinator/Scope 与其他状态一起提交。
- `PublishSnapshotChanged` 在周边状态一致后只调用一次 `RaiseResourcesChanged()`。
- `ReplaceSnapshot` 为简单调用者组合上述两步。
- 一个 ResourceProvider 实例只属于一个 IResourceHost；缓存共享 Snapshot，不共享 Provider 实例。
- 全局 ThemeVariant 和每个局部 Provider 各自只挂载一个 Token ResourceProvider。

### 5.6 组件复合资源键

新增稳定值类型：

```csharp
public readonly record struct ComponentSharedTokenResourceKey(
    string? Catalog,
    string ComponentId,
    SharedTokenKind Kind);
```

查询语义：

```text
SharedTokenKind.X
    -> snapshot.SharedResources[X]

ComponentSharedTokenResourceKey(Button, X)
    -> snapshot.Components[Button].EffectiveSharedResources[X]

ButtonTokenKind.X
    -> snapshot.Components[Button].ControlResources[X]
```

组件 ControlTheme 中消费的 Design Token 必须使用组件复合键。普通应用资源和非组件样式继续使用 SharedTokenKind。

源生成器从 `[ControlDesignToken]` 类型的常量 `ID`、资源 catalog 和生成的 TokenKind 建立强类型身份，并生成组件共享 Token markup extension。禁止运行时通过类型名猜测组件 id。

### 5.7 `ThemeScope` 与 `ThemeConfigProvider`

新增可继承的只读上下文属性 `ThemeScope.SnapshotProperty`，内容树继承的是整个 ThemeSnapshot，不是当前组件 id。

ThemeConfigProvider 自身继承父 Snapshot，编译子 Snapshot，然后：

- 把子 Snapshot 设置到 Content 根元素的 `ThemeScope.SnapshotProperty`。
- 把同一个 Snapshot 发布到自己的 ThemeTokenResourceProvider。
- 父 Snapshot 或本地配置改变时重新编译。
- 编译成功后一次提交；失败时继续保留上一份子 Snapshot。
- `Inherit` 默认 true；false 时使用默认基础快照。
- Provider 永远只保留一个 ResourceProvider，不追加字典。

Provider 的集合 API 迁移到可观察 AvaloniaList，并订阅 TokenSetter/ControlTokenInfoSetter 的属性变化；后续可以增加不可变 `ThemeConfig` 入口。迁移期间保留现有 AXAML 集合语法。

### 5.8 `ThemeCoordinator`

唯一允许修改全局活动主题的对象。

状态机：

```text
Idle -> Preparing -> Committing -> Idle
                    \-> Failed -> Idle
```

- 所有请求在 Dispatcher.UIThread 串行执行并验证线程访问。
- 单一 `ThemeRequest` 同时描述 theme id、ordered algorithms、runtime overrides 和 reason。
- Preparing 阶段查询 Catalog/Cache 并得到完整 Snapshot。
- Preparing 失败时 Application、ActivatedTheme 和资源层保持原值。
- Committing 阶段挂载 Snapshot、同步 manager 属性、设置 RequestedThemeVariant，并在 guard 下消费 ActualThemeVariant 回调。
- 相同请求为 no-op。
- 提交期间的新请求进入队列，不能递归执行。
- 提交全部完成后才派发 Changed。

### 5.9 事件契约

```csharp
ThemeChanging(oldSnapshot, newSnapshot, request, transitionId)
ThemeChanged(oldSnapshot, newSnapshot, request, transitionId)
ThemeChangeFailed(oldSnapshot, request, transitionId, exception, diagnostics)
```

- Changing 只允许观察，不允许修改候选 Snapshot。
- Changed 在 Application、manager、flags 和 resources 一致后派发。
- 订阅者异常单独记录或聚合，不转换为主题加载失败。
- 重入请求排队到当前事件派发结束后。
- 删除没有真实实现的 unload 生命周期和不可观察的 created 生命周期。
- DefaultFont、MediaBreakPoint 等内部副作用迁移到 Compiler options 或提交后 observer，不再借 ThemeLoaded 修改快照。

## 6. 组件资源迁移策略

迁移分两层：

1. 先让 ResourceProvider 同时支持旧普通资源键和新组件复合键。
2. 按控件包迁移 ControlTheme 中对 SharedToken 的消费，并保留旧 Host 作为短期兼容。
3. 每个已迁移控件不再调用 `RegisterTokenResourceScope()`。
4. 单独审计 `SharedTokenResourceValue`：StyledElement 目标改为组件复合 DynamicResource；非 Visual transition/animation 通过拥有明确释放路径的控件属性消费 Token，禁止把 DynamicResource 直接挂到无 scope host 的非 Visual 对象。
5. 所有包迁移完成后删除 `ControlTokenResourcesScopeHostExtensions`、attached properties 和 ScopeProvider 静态字段。

迁移验收必须验证：

- Button component ColorPrimary 影响 Button 自身背景。
- Button Content 中独立 TextBlock 仍读取 scope global ColorPrimary。
- Button 内嵌 Input 使用 Input component config，不使用 Button config。
- 全局切换和局部 Provider 更新无需重新 Attach 即可更新。
- 多个相同控件共享 Snapshot，不创建实例 ResourceDictionary。

## 7. AOT 与生命周期约束

- 不新增字符串属性路径 Binding、运行时程序集扫描或未标注 Activator。
- Token registration metadata 和 markup extensions 由现有增量源生成器生成。
- ThemeTokenResourceProvider 使用公开 ResourceProvider API。
- Snapshot 不持有 Visual、Control、ThemeConfigProvider 或事件订阅。
- Provider 只订阅直接父 ThemeScope，Content 替换、Detach 和 reattach 时对称释放。
- Popup、Flyout 和非 Visual DynamicResource 继续遵守 scoped IResourceHost 生命周期规范。
- Gallery NativeAOT publish 是最终验收项。

## 8. 性能目标

- 每个定义文件每个进程最多解析一次。
- 同一编译缓存键只生成一个共享 ThemeSnapshot。
- 每个 ThemeConfigProvider 永远只有一个 Token ResourceProvider。
- 控件实例不创建组件 Token ResourceDictionary，不订阅全局 ThemeChanged。
- 相同主题请求不触发资源更新或事件。
- 组件 Token 可先保持预计算，后续基于实测引入按 component id 懒加载；首轮重构不同时引入复杂 lazy provider。

## 9. 测试矩阵

### 9.1 Parser

- 紧凑 XML、格式化 XML、自闭合元素和正文 Token 结果一致。
- Value/正文二选一、IsShared、算法顺序、重复键和错误路径正确。
- 空文档、错误根、未知 Token、未知算法和非法 bool 产生精确 diagnostic。
- `en-US`、`fr-FR`、`zh-CN` 下数值结果一致。

### 9.2 Compiler

- Seed -> Map -> Alias 覆盖顺序固定。
- Dark/Compact 有序组合结果稳定。
- Component algorithm false/true 与 Ant Design 语义一致。
- 编译不修改 parent snapshot 或 request 输入。
- 相同输入产生相同资源图；失败不发布部分结果。

### 9.3 Coordinator

- 默认、Dark、Compact、Dark+Compact 各只提交一次。
- 相同请求无事件。
- 重入请求排队，不出现中间变体。
- 解析、编译、资源准备和订阅者异常分别处理。
- 失败后 Application、ActivatedTheme、flags 和资源仍指向旧 Snapshot。

### 9.4 Scope

- 子 Provider 默认继承父 Shared Token 和 component config。
- `Inherit=false` 从默认快照开始。
- 父更新传播到所有子 Provider。
- List add/remove、Setter 值修改和算法修改都触发一次新 Snapshot。
- 编译失败保留上一 Snapshot。
- 重复更新不增加 MergedDictionaries 数量。

### 9.5 Component isolation

- 组件 override 只影响目标组件样式。
- Content 和嵌套组件不继承父组件 private token。
- 全局、父 scope 和子 scope 的 component config 按 Ant Design 规则合并。
- 运行期切换不依赖逻辑树 reattach。

### 9.6 Lifecycle and AOT

- Provider detach 后释放父 scope 订阅。
- Popup/Flyout 内 Token 能跟随所属 scope。
- DynamicResource 不通过 Application 意外持有旧 ShowCase。
- Release net8.0、Debug net10.0 和 Gallery NativeAOT publish 通过。

## 10. 兼容边界

- 首轮重构不改变现有 Token 名称、默认值、ControlToken public API、AXAML ControlTheme key 或现有主题视觉数值。
- `ThemeConfigProvider` 现有集合语法继续可用，并增加正确的 change tracking。
- `IThemeManager` 的现有查询和 Application 扩展继续工作，内部转发到 Coordinator。
- 组件级资源语义修复可能改变此前错误泄漏到 Content 的样式；这是有意的 Ant Design 对齐，不作为兼容行为保留。
- 旧事件和 ScopeHost 在全部内部消费者迁移前保留 adapter，最终删除前先执行仓库级引用扫描。

## 11. 完成标准

- Parser、Compiler、Catalog、Coordinator、Scope 和 ResourceProvider 职责独立。
- 全局和局部主题只调用同一个 ThemeCompiler。
- 任何失败都不会发布部分主题状态。
- Dark+Compact 不产生中间 Dark 激活或重复 Changed。
- 嵌套 Provider 的未覆盖 Token 继承父 Snapshot。
- 组件 Token 不再通过实例 ResourceDictionary 形成子树作用域。
- `ControlTokenResourcesScopeHostExtensions` 和对应 attached resources 被删除。
- 主题资源更新、事件顺序、scope lifecycle 和组件隔离测试通过。
- 目标测试、Release 双目标构建、Gallery 测试、NativeAOT publish 和 `git diff --check` 通过。
