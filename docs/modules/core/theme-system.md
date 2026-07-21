# AtomUI 主题系统架构

本文是 AtomUI 主题系统唯一的长期架构设计文档。它定义最终运行时模型、公开配置语义、主题来源解析、主题文件
处理、Catalog 刷新、Token 编译、动态作用域、资源发布、事件契约、AOT 边界和验收标准。阶段性分析、迁移过程和
任务进度
不写入本文，统一放在 `docs/superpowers/`。

本文描述目标架构。实现过程中不得为了保留旧主题系统 API 而偏离这些约束。

## 1. 设计目标

主题系统必须满足以下不可协商的约束：

- 建立完整的 Seed Token、Map Token、Alias Token、Control Token、算法和嵌套配置模型。
- AtomUI 的公开 API、内部类型、资源键和文档统一使用 `Control` 术语。
- `ThemeSnapshot` 是运行时 Token 值的唯一真源。
- 全局主题和局部主题使用同一个配置合并器、编译器、缓存和提交协议。
- 主题变更先完成整个受影响作用域子树的编译，再在 UI 线程完成无通知引用交换；任何外部观察者只能在
  Publish 阶段看到完整新状态。
- `ThemeConfig`、`ControlThemeConfig` 和 `ThemeRequest` 是深度不可变值，动态更新通过替换完整配置表达。
- AtomUI Token appearance 与 Avalonia `ActualThemeVariant` 在根作用域和每个局部作用域中始终一致。
- 控件级配置只参与目标控件自身的 Token 计算，不形成控件内容树资源作用域。
- 每个主题作用域只拥有一个稳定的 Token 资源 Provider。
- AXAML 中共享 Token 始终使用统一的 `{atom:SharedTokenResource TokenName}` 语法，Control identity 不泄漏到
  每个资源引用名称中。
- Token 资源查询热路径不分配对象，不执行字符串解析、反射或 LINQ。
- 局部主题只复制发生变化的数据，不能按作用域复制完整全局 Token 和全部 Control Token 值。
- 缓存、作用域图、事件订阅和异步编译任务都必须有确定的容量或释放边界。
- 普通 Popup 通过 Avalonia 逻辑树继承主题；独立 TopLevel 必须通过显式 owner context lease 获得局部主题。
- 无 owner 的静态 Dialog、Notification 等 API 只能使用根主题，不能推断调用位置的局部上下文。
- 内置正常路径不使用运行时程序集扫描、`Activator.CreateInstance` 或 Token 属性反射。
- 内置资源、应用资源和用户配置目录通过统一的 `IThemeDefinitionResolver` 边界产生主题来源；Resolver 不解析
  Token，也不绕过标准 Reader、Binder 和 Compiler。
- 用户主题目录只允许显式启用，手动刷新必须原子更新 Catalog 与全部受影响 Snapshot；失败时旧状态完整保留。
- 第一个可见帧之前完成全局主题准备和挂载。
- 不保留旧主题系统的兼容对象、克隆查询 API、伪生命周期事件或可变资源旁路。

## 2. ThemeConfig 设计结论

### 2.1 主题语义与 Avalonia 映射

AtomUI 主题系统以本项目的公开配置契约、Token 模型和运行时作用域为准。设计上沿用分层 Token、配置继承、
有序算法链和作用域隔离等通用主题语义，但不把外部项目的具体版本、commit、包实现或源码快照作为架构约束。
需要保持的 AtomUI 契约是：

- `ThemeConfig` 对 Token 按 key 合并，对 Control 配置先按 identity、再按字段合并；`Algorithms` 整体替换。
- `Inherit=false` 只切断父 `ThemeContext`，仍从 AtomUI 默认主题基线开始计算。
- Token 按 Seed、Map、Alias、Control 四层派生，Control 可以覆盖自身消费的全局 Token。
- 算法是有序 derivative 链；每个算法接收同一份 Seed 和前一算法的 Map 结果。
- 主题计算和组件计算按完整内容缓存，缓存身份包含算法链和所有会改变结果的配置。
- 嵌套主题依赖上下文隔离；脱离上下文的新渲染根不能自动获得调用点主题。
- 样式结构保持稳定，主题切换只替换作用域内 Token 值。

Web 专属实现不进入 AtomUI：CSS selector hash、CSS Variable 命名、style tag 注入、CSS 序列化、SSR 提取、CSP
nonce 和 React hook 生命周期均不属于 Avalonia 主题系统。它们在 AtomUI 中分别由静态 ControlTheme、稳定资源
key、snapshot-backed ResourceProvider、Avalonia 逻辑树和显式资源宿主生命周期替代：

```text
Ant Design: static component CSS + scoped CSS variables
AtomUI:     static ControlTheme    + scoped snapshot resources
```

因此 AtomUI 不需要 `zeroRuntime` 或 hashed style 开关。ControlTheme 本来就在构建期编译，运行时只计算和发布
Token snapshot。

### 2.2 公开配置语义

AtomUI 使用统一的 `ThemeConfig` 描述全局主题和局部主题输入：

| 配置项 | 设计结论 |
|---|---|
| `ThemeConfig.Tokens` | 覆盖当前作用域的全局 Token |
| `ThemeConfig.Controls[ControlTokenIdentity]` | 覆盖目标 Control 自身 Token 和其消费的全局 Token |
| `ThemeConfig.Algorithms` | 定义 Seed 到 Map、Alias 的有序算法链 |
| `ThemeConfig.Inherit` | 决定是否继承父作用域有效配置，默认 `true` |
| `ControlThemeConfig.Algorithm` | 定义目标 Control 的局部算法策略 |

AtomUI 的控件算法必须能表达四种状态：

1. 未指定：继承父配置中同一 Control 的算法策略。
2. 禁用：Control 内覆盖的 Seed Token 不重新派生 Map 和 Alias Token。
3. 使用全局算法：使用当前作用域的全局算法链重新派生该 Control 的有效 Token。
4. 自定义算法：使用 Control 自己声明的有序算法链。

继承链没有提供 Control 算法策略时，未指定最终解析为 AtomUI 默认策略“禁用”。

算法配置的领域模型是 `Unspecified / Disabled / Global / Custom` 四态策略。可空 `bool` 加自定义算法列表
虽然可以间接编码这些状态，但会把一个领域值拆成两个耦合字段，并产生布尔值与自定义列表同时声明等无效
组合。因此配置规范化后必须使用 `ControlAlgorithmMode` 枚举；Custom 的有序算法 identity 作为不可变 payload
保存，并且只允许在 Mode 为 `Custom` 时非空。`bool` 或 `bool?` 不能作为内部真源。

`ThemeConfig`、`ControlThemeConfig` 及其全部集合在构造完成后不可修改。公开构造器或
`ThemeConfigBuilder` 必须防御性复制输入；Builder 只是一次性构造工具，不进入 Provider、Manager、snapshot 或
缓存。运行时绑定通过替换 `ThemeConfigProvider.Config` 更新主题，不支持对已经提交的 Config 内部集合做原位
修改。

## 3. 总体架构

定义与编译路径：

```text
+--------------------------+     +-------------------------+     +---------------------+     +-----------------------+
| ThemeDefinitionResolvers | --> | ThemeDefinitionSources  | --> | ThemeDocumentReader | --> | ThemeDefinitionBinder | --+
+--------------------------+     +-------------------------+     +---------------------+     +-----------------------+   |
                                                                                                                        |
+-------------+     +-----------------------+                                |
| ThemeConfig | --> | ThemeConfigNormalizer | -------------------------------+
+-------------+     +-----------------------+                                |
                                                                             v
                                                                    +-------------------+
                                                                    | ThemeConfigMerger |
                                                                    +---------+---------+
                                                                              |
                                                                              v
+---------------------+        supplies descriptors                 +-------------------+
| ThemeSchemaRegistry | ------------------------------------------> | ThemeCompiler      |
+---------------------+                                             +---------+---------+
                                                                              |
                                                                              v
                                                                    +-------------------+
                                                                    | ThemeSnapshotCache |
                                                                    +-------------------+

ThemeSchemaRegistry 还为 Binder、Normalizer 和资源投影提供 descriptor。
```

运行时发布路径：

```text
+--------------------+        +--------------+
| ThemeSnapshotCache | -----> | ThemeManager |
+--------------------+        +------+-------+
                                    |
                         atomic snapshot commit
                                    |
                    +---------------+---------------+
                    |                               |
                    v                               v
          +-------------------+           +-----------------+
          | Root ThemeContext |           | ThemeScopeGraph |
          +---------+---------+           +--------+--------+
                    |                              |
                    |                              v
                    |                    +--------------------+
                    |                    | Child ThemeContexts |
                    |                    +----------+---------+
                    |                               |
                    +---------------+---------------+
                                    |
                                    v
                      +----------------------------+
                      | ThemeTokenResourceProvider |
                      +-------------+--------------+
                                    |
                                    v
                      +----------------------------+
                      | AXAML and Control runtime  |
                      +----------------------------+
```

系统分成七个边界：

- 来源层：Resolver 只发现可重开读取的主题定义来源，并提供稳定 source identity/revision。
- 定义层：统一读取所有来源并生成与运行时注册无关的语法模型。
- Schema 层：提供生成式 Token 元数据、强类型赋值器、资源投影器和 Control 身份。
- 配置层：绑定不可变公开配置，生成不可变规范化配置，并执行 AtomUI 主题配置合并。
- 编译层：纯计算 Token，输出不可变 `ThemeSnapshot`。
- 运行时层：管理根主题、作用域图、事务、并发、缓存和事件。
- 资源层：把当前 snapshot 投影到 Avalonia 资源查询，不拥有第二份 Token 状态。

## 4. 配置模型

### 4.1 公开输入

`ThemeConfig` 是面向 C#、AXAML 和绑定的不可变配置输入：

```text
ThemeConfig
+-- Inherit = true
+-- Algorithms: optional ordered list
+-- Tokens: global token overrides
\-- Controls[ControlTokenIdentity]
    \-- ControlThemeConfig
        +-- Algorithm: unspecified / disabled / global / custom
        \-- Tokens: control-own and global token overrides
```

`ThemeConfig` 使用只读集合和不可变 entry。C# 可以通过一次性 Builder 构造，AXAML 通常把 ViewModel 中的
不可变 Config 绑定到 Provider；需要内联声明时，由构建期配置扩展生成同一不可变值。不存在“修改已加入集合
的 setter 后自动编译”的隐式行为。

`ThemeConfigProvider` 只把以下变化视为新输入：

- `Config` 属性被替换。
- Provider attach、detach 或 reparent 导致父 ThemeContext 改变。

Provider 不订阅 Config 内部对象图。Capture 在 UI 线程读取一个深度不可变引用，因此后台 Prepare 不需要再次
复制，也不会观察到中途变化。旧 Config 在属性替换后不再被 Provider、Manager、Compiler、snapshot 或缓存
保留；缓存只保存规范化后的结构化 key。

`Algorithms=null` 表示“未指定”；非空集合表示显式算法链。空集合不是“禁用算法”，属于无效输入，必须
产生 diagnostic 并保留最后一次有效配置。Control 自定义算法链遵守相同规则。

`ThemeConfigProvider` 是轻量局部 ThemeVariant 宿主，只公开一个 `Config` 和继承的内容子节点。旧的
`SharedTokenSetters`、`ControlTokenInfoSetters`、字符串算法集合以及 Token 查询属性不属于新架构。

### 4.2 规范化配置

`ThemeConfigNormalizer` 对一次不可变输入完成以下工作：

- 解析并验证 Token key 和 value。
- 把 Control id 解析为 `ControlTokenIdentity`。
- 保留算法的“未指定”状态。
- 将公开 identity 和 value 绑定为当前 registry revision 下不可变、有确定顺序的值对象。
- 计算稳定的结构 fingerprint。
- 返回 diagnostics，不抛出缺少上下文的反射异常。

编译器只接收 `NormalizedThemeConfig`。Normalizer 不读取 Visual、ResourceHost 或全局服务。

### 4.3 合并规则

`ThemeConfigMerger` 是全局和局部主题唯一的合并入口：

配置优先级从低到高固定为：

```text
ThemeSchemaRegistry / AtomUI library defaults
-> selected ThemeDefinition
-> builder/application initial config
-> runtime ThemeRequest.Config
-> parent scope effective config
-> local ThemeConfigProvider.Config
```

根事务先把所选 `ThemeDefinition` 应用到 AtomUI library defaults，再应用 builder/application initial config。
`ThemeRequest.Config` 为 `null` 时保留该根配置；非空时按 `Inherit` 合并：`Inherit=true` 继续继承应用初始
配置，`Inherit=false` 只丢弃较低优先级的应用初始 config，以“AtomUI library defaults + 当前所选
ThemeDefinition”为根基线。选择某个主题后，runtime config 不能通过 `Inherit=false` 意外删除该主题定义。

局部 `ThemeConfigProvider` 的父输入是父 `ThemeContext` 的完整有效配置。局部 `Inherit=false` 的基线是
AtomUI library defaults，不继承父作用域、当前所选主题定义、应用初始配置或 runtime override；随后只应用
当前 Provider 的本地配置。这与 Ant Design ConfigProvider 的局部 `inherit=false` 语义一致。

- `Inherit=true` 时以父作用域的有效配置为基础。
- `Inherit=false` 时使用上述入口对应的基线，不隐式复制父有效配置。
- `Config=null` 等价于不可变空配置 `Inherit=true`，只共享父有效结果。
- 当前 `Tokens` 按 key 覆盖父 `Tokens`。
- `Controls` 先按 `ControlTokenIdentity` 合并，再按 Token key 合并。
- Control 的算法未指定时继承父策略；显式 disabled、global 或 custom 时替换父策略。
- 当前 `Algorithms` 为 `null` 时继承基线算法；显式提供非空列表时整体替换，不和基线算法拼接。
- 主题定义、应用级配置和运行时请求都先转换为规范化配置，再按同一规则合并。

合并器同时计算 `BaseAppearance`：根配置以所选 ThemeDefinition 的已验证 appearance 为基线；局部
`Inherit=true` 以父 snapshot appearance 为基线；局部 `Inherit=false` 以 AtomUI library baseline 的 Light
appearance 为基线。全局算法 descriptor 的 appearance effect 按顺序折叠，最后一个非 `Preserve` effect
决定结果。Control 自定义算法只影响该 Control 的 Token 和 Control appearance，不改变所在 ThemeContext 的
Avalonia ThemeVariant。

Control 的 `Tokens` 使用一个集合表达 Control 自身 Token 和其消费的全局 Token。Schema Binder 根据
生成元数据完成分类；同名 Token 同时存在于两类 schema 时，同一个覆盖值同时应用于两类输入。由此，一个
Control 配置项可以同时覆盖 Control 自身 Token 和该 Control 消费的全局 Token。

## 5. Theme Schema 与注册

`ThemeSchemaRegistry` 在应用主题初始化前一次性构建，内容来自源生成器或显式描述符：

```text
ThemeSchemaRegistry
+-- GlobalTokenSchema
|   \-- token name, kind, stage, value type, parser, setter, getter, resource key
+-- ControlTokenDescriptor[ControlTokenIdentity]
|   \-- factory, own token schema, evaluator, resource projector
\-- ThemeAlgorithmDescriptor[algorithm id]
    \-- revision, evaluator, AOT factory, appearance effect
```

每个 `ThemeAlgorithmDescriptor` 必须声明稳定 id 和显式 revision/version。算法实现、默认参数、依赖 Token 或
输出语义发生变化时必须提升 revision，不能只保留相同 id 让旧缓存继续命中。

算法 descriptor 的求值契约固定为：

```text
Evaluate(effectiveSeed, previousMap?) -> nextMap
```

同一算法链中的 `effectiveSeed` 引用和值保持不变；第一个算法接收 `previousMap=null`，后续算法接收前一个算法
输出的 Map。算法不得修改 Seed 或 previous Map，必须返回新的不可变语义结果或只在本次编译 workspace 中写入
目标 builder。Dark、Compact 在没有 previous Map 时可以内部调用 Default，但这种 fallback 属于算法实现，
不能改变通用链契约。

`ThemeSchemaRegistry.Revision` 根据排序后的完整 descriptor schema 确定性计算，输入至少包含 Control
identity、Token name/kind/stage/value type/resource key schema、算法 id/revision 及其外观声明。注册顺序不影响
revision；任何会改变绑定、编译或资源投影结果的 descriptor 变化都必须改变 revision。

Registry 在首个 snapshot 编译前冻结。Manager 构建完成后不能追加 Token、Control、算法或主题资产
descriptor；可选控件包必须在 `UseAtomUI` Builder 阶段完成显式注册。

生成器分配的 Token 和 Control slot 只保证在同一 registry revision 生命周期内稳定。不同 revision 可以重新
分配 slot，公开协议、持久化配置和跨版本缓存不得依赖整数 slot 不变，而应使用稳定 identity 并在绑定后解析
当前 revision 的 slot。

源生成器必须为内置 Token 生成：

- 稳定的 `ControlTokenIdentity`，包括 catalog 和 control id。
- Token name 对应的强类型赋值委托查找表。
- Token value 到 Avalonia resource value 的投影。
- Control Token builder 的直接构造委托。
- Control 自身 Token schema 和继承的 Token schema。
- 生成式 Control 自身资源键，以及 Control identity 与共享 Token kind 的内部复合 key factory。
- 每个 ControlTheme 资产的 `ControlThemeAssetDescriptor`，包括资产 URI、唯一 Control identity 和资源 key
  schema 摘要。

生成器不得为每个 Control 生成 `AlertTokenSharedTokenResource`、`ButtonTokenSharedTokenResource` 之类的公开
MarkupExtension。Control identity 只属于 schema、配置和主题资源作用域边界，不属于每一次共享 Token 的 AXAML
调用名称。

包注册直接传递完整描述符，不得把生成器提供的 identity 丢弃后只注册 `Type`。内置路径删除
`Activator.CreateInstance`、`Type.GetProperties`、`PropertyInfo.GetValue/SetValue` 和枚举反射。

第三方 Control Token 必须使用 AtomUI 源生成器，或者显式提供完整 descriptor。不存在自动扫描程序集的
fallback。

## 6. 主题文件

### 6.1 文件结构

主题定义 XML v1 使用版本化 namespace、显式 identity 和 Control 术语：

```xml
<?xml version="1.0" encoding="utf-8"?>
<Theme xmlns="https://atomui.net/schemas/theme/v1"
       Id="DaybreakBlue"
       Name="Daybreak Blue"
       Appearance="Light"
       IsDefault="true">
  <Algorithms>
    <Algorithm Id="Default" />
  </Algorithms>
  <Tokens>
    <Token Name="ColorPrimary" Value="#1677FF" />
    <Token Name="ColorLink" Value="#1677FF" />
  </Tokens>
  <Controls>
    <Control Catalog="AtomUI" Id="Button" Algorithm="Global">
      <Tokens>
        <Token Name="ColorPrimary" Value="#0958D9" />
        <Token Name="ContentFontSize" Value="14" />
      </Tokens>
    </Control>
  </Controls>
</Theme>
```

规则如下：

- namespace `https://atomui.net/schemas/theme/v1` 是格式版本，必须显式声明。
- `Theme.Id`、`Appearance` 和顶层 `Algorithms` 必须显式声明。
- `Appearance` 是从 library Light baseline 折叠 definition 算法 effect 后的最终断言，不是覆盖算法结果的独立
  开关。
- 算法使用有序 `Algorithm` 元素，不使用逗号分隔字符串。
- Control 使用 `(Catalog, Id)` 组成稳定身份。
- Control 的 `Algorithm` 省略表示未指定，`Disabled` 表示禁用，`Global` 表示使用全局算法链。
- Control 自定义算法使用子元素 `Algorithms`，并且不能同时声明 `Algorithm` 属性。
- Token 是空元素，必须通过 `Value` 属性提供非空值，不允许正文值或类型分类属性。

完整格式、词法、安全限制、diagnostic 和版本演进规则见
[主题定义 XML v1 规范](theme-definition-xml.md)，机器可读标准见
[AtomUI Theme Definition XML Schema v1](schemas/atomui-theme-v1.xsd)。

### 6.2 两阶段处理

`ThemeDocumentReader` 只负责：

- 使用禁止 DTD、外部实体和网络解析的 validating `XmlReader`，并显式启用 XSD identity constraint。
- 通过进程级复用的 v1 XSD 校验 namespace、结构、属性、容量和局部唯一性。
- 保留源码位置并执行 XML 元素总数等 Reader 级资源限制。
- 输出不依赖当前 Control 注册集合的 `ThemeDocument`。

`ThemeDefinitionBinder` 负责：

- 使用 `ThemeSchemaRegistry` 解析 Token 和算法 id。
- 将字符串值一次性转换为目标类型。
- 校验未知 Token、未知 Control、算法冲突、Appearance 一致性和跨字段语义。
- 输出不可变、typed `ThemeDefinition` 和结构化 diagnostics。

Reader 不得因为某个可选控件包没有安装而修改或过滤语法树。Catalog 保存完整 diagnostic，并决定某个
定义是否可用于当前 registry。`ThemeDefinitionRevision` 是由 Catalog 生成的结构化值，至少包含稳定 source
identity、source revision 和内容摘要：文件来源使用实际字节内容摘要，不能只使用时间戳；包内资源包含包版本、
资源路径和内容摘要；程序化来源必须显式提供 revision。来源字节或程序化定义语义变化时 revision 必须变化。
同一 definition revision 在进程中最多成功读取一次；同一 `(definition revision, registry revision)` 最多成功
绑定一次。更换 registry revision 只重新绑定已有 `ThemeDocument`，不重复解析来源 XML。

### 6.3 Theme Definition Resolver

主题来源统一通过 Resolver 发现。Resolver 只描述“有哪些来源以及如何读取原始字节”，不得自行执行 XML 解析、
Token 绑定、算法求值或资源发布。公开契约使用 `Definition` 而不是 `Config` 术语，因为 `ThemeConfig` 已经专指
运行时 Token/Algorithm 覆盖；主题 XML 是包含 Id、Name、Appearance、默认标记和 revision 的完整 definition。

```csharp
public interface IThemeDefinitionResolver
{
    string Id { get; }
    bool SupportsReload { get; }

    ThemeDefinitionResolveResult Resolve(
        ThemeDefinitionResolveContext context);
}

public interface IThemeDefinitionSource
{
    string SourceIdentity { get; }
    string SourceRevision { get; }

    Stream OpenRead();
}
```

`ThemeDefinitionResolveResult` 包含不可变 source 列表和结构化 diagnostics；Resolver 不用异常表示普通的来源不可用
或目录读取失败。`ThemeDefinitionResolveContext` 至少包含已冻结的 Application Id、应用配置根目录、是否为运行时
刷新以及当次 resolver generation。Resolver 返回后，ThemeManager 防御性复制结果，不保留 Resolver 的可变集合。

启动必须在第一个可见帧前同步得到初始主题，因此 Resolver 合约是同步的。运行时手动刷新由 ThemeManager 在后台
线程调用同一同步合约，再进入统一 Reader/Binder/Compiler；不为了运行时刷新把应用启动改成先挂载空主题的异步
流程。

标准 Resolver 分为：

- `BuiltInThemeDefinitionResolver`：显式列出 AtomUI Core 的 `avares://` 主题，必须成功，不支持刷新。
- `AvaloniaAssetThemeDefinitionResolver`：由应用或控件包显式列出 `avares://` 主题，必须成功，不支持刷新。
- `UserDirectoryThemeDefinitionResolver`：读取应用用户配置目录中的 `*.theme.xml`，允许启动降级，支持手动刷新。

Builder 公开注册入口为：

```csharp
void AddThemeDefinitionResolver(IThemeDefinitionResolver resolver);
void WithApplicationId(string applicationId);
void UseUserThemeDirectory();
void UseUserThemeDirectory(string directory);
```

Resolver Id 在一次 Builder 中必须非空且唯一。内置 Resolver 总是最先注册，应用 Resolver 按显式注册顺序追加；
不执行程序集扫描、资源目录枚举或基于命名约定的类型发现。

### 6.4 Application Id 与用户主题目录

`UseAtomUI(Application, ...)` 在创建 Builder 时，默认从具体 Application 类型所在程序集的简单名称解析
Application Id：

```csharp
application.GetType().Assembly.GetName().Name
```

这只是无配置默认值，不扫描程序集。`WithApplicationId(...)` 可以覆盖它；显式值优先。`Application.Name` 是平台
显示名称，可能为空、包含空格或随品牌文案变化，不参与默认 App Id 推断。

Application Id 必须是单个安全路径段，只允许 ASCII 字母、数字、`.`、`_` 和 `-`，不得为空、`.`、`..`，也不得
包含目录分隔符。解析结果在 Builder 完成时冻结。启用用户主题但无法得到合法 App Id 时构建失败并给出明确
diagnostic。

无显式目录时，`UseUserThemeDirectory()` 使用：

```text
Environment.SpecialFolder.ApplicationData/{ApplicationId}/Themes
```

对应 Windows `%APPDATA%`、macOS `~/Library/Application Support`，以及 Linux 的 XDG 配置根目录。显式目录重载
只改变根路径，不改变文件、安全、冲突或刷新语义。Browser/WASM 不隐式启用用户目录；没有普通文件系统能力的
宿主只有在提供自己的 Resolver 时才能加入外部主题。

用户目录只枚举顶层 `*.theme.xml`，按规范化完整路径 ordinal 排序，不递归、不跟随 symlink/reparse point。
默认最多 128 个文件、总原始字节 32 MiB；单文件仍受 Reader 的 4 MiB 和元素/Token 上限约束。目录不存在时
Resolver 可以创建空目录；创建或枚举失败产生 diagnostic，不能退回当前工作目录或应用安装目录。
Resolver 在枚举前冻结规范化后的目录真实路径；打开文件时使用平台 no-follow 语义，并从实际读取流再次执行总
字节预算。路径检查结果不能代替文件句柄约束，避免检查后替换为链接时越过主题目录边界。

### 6.5 来源优先级与冲突

来源顺序只决定 `AvailableThemes` 的稳定展示顺序，不提供覆盖语义：

```text
Core built-in -> application/package avares -> user directory
```

- 任意来源之间重复 `Theme.Id` 都是错误，不使用 first-wins、last-wins 或静默覆盖。
- 可刷新 Resolver 不得提供 `IsDefault=true` 的 definition；默认主题只能来自不可刷新、随应用发布的来源。
- 合并后的静态 Catalog 必须恰好有一个默认主题。
- 用户主题只能引用当前冻结 registry 已注册的 Token、Control 和 Algorithm；XML 不能加载代码、程序集、脚本、
  include/import、网络资源或环境变量表达式。
- Core 和应用资源错误属于损坏的发布产物，阻止应用启动；用户目录首次加载错误不阻止应用启动，初始 Catalog
  降级为全部不可刷新来源，并通过 `IThemeManager.ThemeCatalogDiagnostics` 暴露本次错误。

用户目录是一个刷新单元。任一用户主题 XML、目录容量、重复 Id、默认主题声明或 Binder 校验失败时，本次用户
目录解析失败，不提交有效子集，也不维护逐文件 Last Known Good。这样目录内容、候选 Catalog 和刷新结果只有一个
明确版本。

### 6.6 Catalog 构建

Catalog 对所有 Resolver 结果执行同一管线：

```text
Resolver source
    -> bounded byte copy + SHA-256
    -> ThemeDocumentReader + v1 XSD
    -> ThemeDefinitionBinder + frozen ThemeSchemaRegistry
    -> BoundThemeDefinition + ThemeDefinitionRevision
    -> immutable CompiledThemeCatalog
```

`ThemeDefinitionRevision.SourceIdentity` 来自 source identity；`SourceRevision` 来自 Resolver 的结构化 revision；
`ContentDigest` 必须根据本次实际读取的字节计算。文件长度或修改时间只能筛选缓存候选，不能代替内容摘要。
Catalog 构建期间只使用本次复制的字节，避免文件枚举后修改造成 Reader 与 digest 针对不同内容。

## 7. 编译器

`ThemeCompiler` 是无 UI、无资源宿主、无全局服务访问的纯计算入口：

```text
ThemeCompileInput
  -> merge effective config
  -> initialize effective Seed Token
  -> run ordered Evaluate(seed, previousMap) algorithms
  -> apply Map overrides
  -> format Alias Token
  -> apply Alias overrides and AtomUI final normalization
  -> evaluate each registered Control
  -> freeze token values and resources
  -> ThemeSnapshot
```

编译阶段允许使用可变 builder，因为算法天然需要逐步派生。builder 的生命周期只限于一次编译，不能进入
snapshot，也不能通过公开 API 返回。

全局计算顺序固定为：

1. 从 AtomUI library Seed defaults、ThemeDefinition 和有效配置构造唯一的 effective Seed。
2. 将同一 effective Seed 依次传给算法链；第一个算法的 previous Map 为空，后续算法接收前序 Map。
3. 对最终 Map 应用显式 Map Token override。
4. 由 Map 生成 Alias Token，再应用显式 Alias Token override。
5. 执行 AtomUI 最终语义规范化。`MotionEnabled=false` 时，Motion 服务被禁用且全部标准 duration Token 强制为
   `TimeSpan.Zero`；`WaveEnabled=false` 时 Wave 行为不创建动画。该后置条件不依赖单个控件是否正确消费时长。

每个 Control 的计算顺序为：

1. 以全局最终 Alias Token 为基线，叠加该 Control 覆盖的全局 Token。
2. `Disabled` 时只保留直接覆盖，不重新派生；`Global` 或 `Custom` 时，从 Control effective Seed 按相应算法链
   重新生成 Map 和 Alias。
3. 在重新派生后应用该 Control 的 Map、Alias override 和 Motion 最终规范化。
4. 使用 Control 的有效全局 Token 计算 Control 自身默认 Token。
5. 应用 Control 自身 Token override。
6. 根据全局 appearance 和 Control 算法 effect 计算 Control appearance；它只提供给该 Control evaluator，不
   修改 ThemeContext appearance。
7. 生成共享 Token 差量、Control Token 值和资源投影。

这与 Ant Design 的组件计算保持同一语义：未启用组件算法时只是覆盖；启用时以“全局最终 Token + 组件覆盖”
作为重新派生输入。Control 默认 Token evaluator 必须接收 Control appearance，不能继续只读取全局
`IsDark`。

任何 validation、conversion 或 algorithm 错误都返回失败结果，不返回部分 snapshot。

## 8. ThemeSnapshot

`ThemeSnapshot` 是 Token 编译结果和资源发布单元：

```text
ThemeSnapshot
+-- ThemeId / DefinitionRevision / ContentFingerprint
+-- Algorithms[(id, revision)] / Appearance
+-- RegistryRevision
+-- EffectiveConfig
+-- GlobalTokenValues: dense immutable TokenValueTable
+-- GlobalResources
+-- PresetColorPalettes
\-- Controls[registry control slot]
    +-- Appearance
    +-- EffectiveGlobalTokenDelta
    +-- ControlTokenValues: dense immutable TokenValueTable
    \-- ControlResources
```

具体约束：

- Token schema 由生成器分配在当前 registry revision 内稳定的 slot；完整 Token 值表使用稠密不可变数组，不使用
  object-key 字典保存每个值。
- Control 表按 registry slot 存放引用，`ControlTokenIdentity` 只用于配置边界和诊断。
- 稀疏覆盖、共享 Token 差量和 Avalonia 资源投影使用不可变集合或 `FrozenDictionary`。
- 颜色资源在 snapshot 中已经转换为不可变 Brush。
- palette、集合和其他引用值必须在冻结前深复制为不可变值。
- 不保存 `DesignToken`、`IControlDesignToken`、Control、Visual、Provider、事件或订阅。
- 不提供 `SharedToken`、`ControlToken`、`Clone` 或可变字典查询 API。
- imperative 消费者通过只读 Token resolver 查询 snapshot 值。
- `ContentFingerprint` 是规范化内容的快速 hash，不是无碰撞的唯一身份。snapshot 的语义相等性必须比较对应的
  结构化不可变 key，不使用父 snapshot 的递增版本表达语义身份。
- `Appearance` 是 `Light` 或 `Dark` 的已提交结果，是根 Application 和局部 ThemeVariantScope 发布
  Avalonia variant 的唯一依据；不能通过算法名称字符串猜测。

## 9. 缓存

`ThemeSnapshotCache` 使用结构化 value key：

- 完整的不可变 `ThemeDefinition` 结构化 key，包括 definition revision 和 typed 内容。
- 完整的不可变 `NormalizedThemeConfig` 结构化 key。
- 有序算法 descriptor `(id, revision)`。
- `ThemeSchemaRegistry` revision。

父主题的影响已经包含在合并后的有效配置中，因此 cache key 不包含父 snapshot 的发布序号。

缓存要求：

- fingerprint 只用于定位候选 bucket 或加速拒绝；命中后必须比较结构化不可变 key。有限位 hash 相同但结构不同
  的 definition、config、算法链或 schema 不能共享结果。
- 等价内容返回同一个 snapshot 引用。
- 并发等价请求只执行一次编译。
- 失败结果不进入缓存。
- 缓存由 ThemeManager 实例拥有，使用强引用 LRU，并同时受 entry 数量和保守
  `EstimatedRetainedBytes` 上限约束；达到任一上限即回收最久未访问 entry，不区分 active/inactive。
- snapshot 的 retained bytes 根据稠密 Token 表、Control 引用表、差量、资源条目、palette 和不可变 Brush 的
  确定性保守公式估算，不使用进程 RSS，也不声称等于 CLR 精确对象大小。
- 活动 ThemeContext 自己强持有当前 snapshot。cache eviction 只删除复用入口，不影响已发布 snapshot，因此不
  需要 pin、PinCount 或反向 snapshot-key 表。
- Provider 实例不进入缓存，不在不同资源宿主之间共享。
- 不建立无上限的 Brush、字符串、resource key 或 fingerprint intern pool。

single-flight 编译记录与已完成 LRU entry 分离。记录在成功后才进入 LRU；失败、取消或 superseded 结果立即
释放。默认完整 snapshot cache 上限为 32 entry 和 32 MiB estimated retained bytes；Control compilation
cache 上限为 2048 entry 和 32 MiB。应用可以调小；扩大默认值必须提供 benchmark、峰值对象数和 Gallery
导航内存证据。

## 10. ThemeManager 与全局主题

`ThemeManager` 是唯一主题运行时协调者和唯一允许发布 snapshot 的对象。具体类型是 internal Avalonia
`Styles` 宿主并实现公开的 `IThemeManager`；同一个实例同时挂载到 Application Styles 和主题服务容器，不再
创建同生命周期的一对一 `ThemeEngine` 或第二份 Theme Token 状态。

`ThemeManager` 只拥有长期运行时状态和编排职责：当前 `ThemeState`、根 `ThemeContext`、`ThemeScopeGraph`、
请求 generation/队列、Application ThemeVariant 发布以及结果事件。每个请求的阶段状态属于独立
`ThemeTransaction`；规范化、合并、编译、缓存和事件调用分别委托给对应对象。合并运行时所有者不等于把这些
职责的实现代码并入 `ThemeManager`。

作为 `Styles` 宿主，Manager 挂载根 Token Provider、Control themes 和语言资源入口；语言切换及资源构建委托给
Language 子系统，不进入 ThemeTransaction，也不能修改 `ThemeState`。Manager 不实现 Token 算法、XML 读取、
schema binding 或资源查询细节。

不存在供多个 Manager 共享的 headless engine，也不存在一个 Manager 切换多个 Styles 宿主的场景。纯计算复用
由 Compiler 和 Cache 提供，局部主题复用由 ThemeContext 和 ThemeScopeGraph 提供，因此额外 Engine 不形成
有效边界，只会引入 facade 转发、事件桥接和双向生命周期协调。

新公开模型由以下对象组成：

- `ThemeInfo`：Catalog 中可选择主题的只读元数据；`AccentColor` 只投影 Theme Definition 中显式绑定成功的
  `ColorPrimary`，未声明时为 `null`，不从运行时 Snapshot 或算法派生结果反向推断。
- `ThemeRequest`：主题 id、`ThemeConfig` 和变更原因。
- `ThemeState`：当前已提交主题的 id、算法、`Appearance`、仅用于观察和快速比较的 fingerprint，以及
  transition id。
- `ThemeTransitionResult`：一次请求的 transition id、`Committed / NoOp / Superseded / Failed` 状态、已提交
  `ThemeState`、失败 diagnostics/exception 以及独立的 `PublishDiagnostics`。
- `IThemeManager.ApplyThemeAsync(...)`：返回 `Task<ThemeTransitionResult>` 的运行时主题请求入口。
- `ThemeCatalogReloadResult`：一次 Catalog 刷新的 generation、`Committed / NoOp / Superseded / Failed` 状态、
  新主题列表、diagnostics、publish diagnostics 和 exception。
- `IThemeManager.ReloadThemesAsync(...)`：只重新执行 `SupportsReload=true` 的 Resolver，并原子更新 Catalog 与
  当前主题作用域图。
- `IThemeManager.ThemeCatalogChanged`：成功提交新 Catalog 后的结果事件。
- `IThemeManager.ThemeCatalogDiagnostics`：最近一次启动加载或手动刷新的只读 diagnostics。
- `IThemeManager.CurrentTheme`：只读 `ThemeState`。
- `IThemeManagerBuilder.WithInitialTheme(...)`：首帧主题 id 和 ThemeConfig 的唯一配置入口。

删除 `Theme` 和 `ITheme`。Catalog 只保存 definition，不为 Default、Dark、Compact 预先创建 Theme 对象。
每个请求都由 definition、配置和算法构成规范化输入，因此同一主题的新 runtime override 必须重新查询缓存
或编译。

AtomUI 主题身份与 Avalonia `ThemeVariant` 解耦：

- AtomUI 使用 `ThemeRequest` 表达主题 id 和算法。
- `Application.RequestedThemeVariant` 只根据已提交 snapshot 设置为 Avalonia Light 或 Dark。
- Compact 和自定义主题 id 不编码进 `ThemeVariant` 名称。
- `Application.RequestedThemeVariant` 是 ThemeManager 的发布输出；应用不得把它作为第二个主题写入口。
- 跟随系统主题是一种明确的 Manager 策略，不通过双向绑定或让 Application 长期保持
  `ThemeVariant.Default` 实现。

FollowSystem 在 Builder 阶段获得 Light 和 Dark 两个完整、不可变的 root request 模板。Manager 监听平台外观
变化，先根据目标外观 Capture/Prepare 对应 snapshot，再在 Publish 阶段显式设置
`Application.RequestedThemeVariant=Light/Dark`。平台先改变 appearance、AtomUI 后编译 Token 的中间状态不得
暴露。初始系统 appearance 必须在首个 snapshot 编译前解析。

Root ThemeContext 通过 ThemeManager 中的全局 TopLevel style 把稳定的 `ThemeScope.ContextProperty` 注入每个
TopLevel；不能假设 inheritable AvaloniaProperty 会从 `Application` 自动传到独立 TopLevel。由 owner lease
创建的局部 TopLevel 使用 local value 覆盖该根 style 值。

Motion 和 Wave 配置也是 ThemeConfig Token override。任何入口改变这些值时都生成新请求，不直接修改
`ResourceDictionary`。

ThemeManager 还保存最后一次已提交的完整 root `ThemeRequest`。Catalog 刷新导致当前 definition 内容变化或当前
Theme Id 被删除时，Manager 使用该 request 的不可变 `ThemeConfig` 重新编译或回退，确保 Dark、Compact、Motion
和其他 runtime override 不因刷新而丢失。

## 11. 动态作用域

### 11.1 稳定 ThemeContext

`ThemeContext` 是稳定的作用域身份，包含：

- 当前 `ThemeSnapshot` 的原子引用。
- 唯一的 `ThemeTokenResourceProvider`。
- scope identity 和只读 appearance。
- Publish 阶段使用的内部 context changed 通知。

`ThemeContext` 不复制或修改 Token。snapshot 仍是 Token 值唯一真源。

`ThemeScope.ContextProperty` 是 inheritable attached property，在树上传递稳定 context，而不是每次主题变化都
替换 attached snapshot。根值由全局 TopLevel style 注入；局部值由 ThemeConfigProvider 设置。没有局部
Provider 时，控件解析到根 `ThemeContext`。

`ThemeConfigProvider` 继承 Avalonia `ThemeVariantScope`，从而同时拥有局部 AtomUI ThemeContext 和局部
Avalonia variant 边界。它的 Resources 在 attach 期间只挂载一个稳定 Token Provider；更新时不替换 Provider，
只在 CommitCore 替换 ThemeContext snapshot，并在 Publish 设置 `RequestedThemeVariant` 为 snapshot 的 Light
或 Dark appearance。

### 11.2 ThemeScopeGraph

`ThemeManager` 拥有一个活动 `ThemeScopeGraph`：

```text
+---------------------------+
| ThemeManager              |
| owns topology and commits |
+-------------+-------------+
              |
              v
+---------------------------+
| Root ThemeContext         |
| snapshot + provider       |
+-------------+-------------+
              |
       +------+------+
       |             |
       v             v
+-------------+ +-------------+
| Scope A     | | Scope B     |
| context A   | | context B   |
+------+------+ +-------------+
       |
   +---+---+
   |       |
   v       v
+----------+ +----------+
| Scope A1 | | Scope A2 |
+----------+ +----------+
```

每个 `ThemeScopeNode` 保存：

- 单调递增且本次注册唯一的 `RegistrationId`，防止 detach/reattach ABA。
- `ParentRegistrationId` 和子节点集合。
- 每次 Config 替换递增的 `ConfigRevision`。
- 当前不可变规范化配置和 `LastValidConfig`。
- 稳定 ThemeContext、Provider 和 ThemeConfigProvider。

`ThemeScopeGraph.TopologyRevision` 在 attach、detach 或 reparent 时递增。Capture 必须记录 root generation、
TopologyRevision，以及每个受影响节点的 `(RegistrationId, ParentRegistrationId, ConfigRevision)`。Prepare 结果
提交前必须逐项验证；任一值变化都使旧结果成为 `Superseded`，不能提交到新拓扑。

`ThemeConfigProvider` 通过 ThemeManager 的 internal scope registration contract 注册节点；attach 时注册，
detach 时注销，父级变化或内容重新挂接时更新图关系。Provider 负责提交当前可观察配置变化，但不通过静态
`ThemeManager.Current` 查找具体实现，不自行监听父 snapshot，不直接调用 compiler，也不自行实现事件队列。

全局主题变化时，ThemeManager 使用新根 snapshot 和各节点最后一次有效配置，按拓扑顺序准备整个作用域图。
局部配置变化时，只准备该节点及其后代。

所有根和局部事务共用一个 Manager-wide scheduler 和 Commit gate。同一 Manager 同时最多有一个 Prepare
管线和一个 CommitCore；新请求按作用域合并，根请求覆盖尚未提交的全部后代准备请求，从而限制编译峰值并
消除根、局部交错提交。

配置输入无效时：

- 当前作用域保留已发布 snapshot。
- `LastValidConfig` 不更新。
- 派发失败事件和 diagnostics。
- 后续父主题变化使用 `LastValidConfig`，避免该作用域永久冻结在旧全局主题。
- 首次 attach 尚无已发布局部 snapshot 时，使用父 snapshot 作为安全回退，并把空 `Inherit=true` 配置作为
  fallback；无效局部输入不能造成空资源或首帧崩溃。

普通 Popup、Flyout 只要保留 Avalonia 逻辑父级，就自然继承 `ThemeScope.ContextProperty`、资源查询链和
ActualThemeVariant，不创建额外 bridge。

独立 Window、Dialog、Notification host 和其他新 TopLevel 必须通过 owner 创建
`ThemeContextLease`。Lease 在宿主生命周期内：

- 在 TopLevel 设置 owner 的 ThemeContext local value 和 snapshot appearance。
- 挂载一个宿主私有 `ThemeContextResourceBridge`，只委托给 owner context，不复制 Token。
- 订阅 owner context Publish 通知并向该 TopLevel 传播最小资源通知。
- owner 改变、窗口关闭或 API dispose 时对称移除 bridge、属性值和订阅。

AtomUI Desktop `Window` 在调用 Avalonia 的 `Show`、`Show(owner)` 或 `ShowDialog(owner)` 之前完成 lease 获取，
因此 `WindowOpenedEvent`、样式应用和首个布局都只能看到完整的 Context、显式 Light/Dark variant 和私有 bridge。
带 owner 的重载从 owner 解析 Context；无 owner 的根 Window 使用 Root ThemeContext。若 Avalonia 打开流程同步失败，
本次新建 lease 必须在异常离开前释放；`OnClosed` 再执行幂等释放，不能依赖外部调用者补清理。

`ThemeTokenResourceProvider` 仍是每个 scope 唯一的规范 Provider；ResourceBridge 是独立资源根的非拥有代理，
不进入 cache，也不形成第二份 Token 状态。无 owner 的静态 API 只能显式租用 Root ThemeContext，并应提供
diagnostic，不能根据调用栈、当前焦点或全局静态变量猜测局部上下文。

## 12. 事务与发布顺序

一次主题事务分为 Capture、Prepare、CommitCore、Publish、Complete 五个阶段：

```text
+-------------+  +-------------+  +-------------+  +-------------+  +-------------+
| Capture     |->| Prepare     |->| CommitCore  |->| Publish     |->| Complete    |
| UI thread   |  | pure work   |  | UI thread   |  | UI thread   |  | result task |
+-------------+  +-------------+  +-------------+  +-------------+  +-------------+
| immutable   |  | merge/cache |  | silent swap |  | variants    |  | committed / |
| graph stamp |  | compile all |  | all contexts|  | resources   |  | no-op / ... |
| affected set|  | staged data |  | state refs  |  | events      |  |             |
+-------------+  +-------------+  +-------------+  +-------------+  +-------------+
```

Capture 完成不可变 Config 绑定、scope stamp 和 affected set；Prepare 只执行纯合并、single-flight cache 和
完整子树编译。进入 CommitCore 前，Manager 在 UI 线程再次验证 generation、TopologyRevision 和全部 node
stamp。

CommitCore 只执行不会调用外部代码的引用和值交换：一次性替换所有受影响 ThemeContext snapshot、
ThemeState 和内部 appearance。它不设置 AvaloniaProperty、不调用 `RaiseResourcesChanged`、不派发 context 或
结果事件。实现必须预先分配 staged 数组并完成所有可失败工作，使 CommitCore 成为无失败、无分配、无通知的
有限赋值序列。

`Application.RequestedThemeVariant`、局部 `ThemeVariantScope.RequestedThemeVariant`、context changed、
ResourceProvider 通知和结果事件都属于不可回滚 Publish。Publish 开始前所有 context 已指向完整新 snapshot，
因此 Avalonia 同步回调也只能读取完整新状态。不能再承诺在 `RequestedThemeVariant` 或资源通知之后静默回滚。

Publish 的每个外部通知边界必须独立捕获、记录并继续后续通知。观察者异常不把已提交事务改写成 Failed；结果
仍为 `Committed`，并携带 publish warning diagnostics。只有 Capture、Prepare 或 CommitCore 前的 stamp/invariant
验证失败可以返回 `Failed` 或 `Superseded`，且此时尚未交换任何已发布引用。

同一作用域的新请求使用 generation 标识。旧的异步准备结果在提交前发现 generation 已过期时直接丢弃。
提交期间发生的重入请求进入队列，同一作用域尚未执行的请求合并为最新请求。

`ApplyThemeAsync` 的请求完成语义固定如下：

- `Committed`：新 snapshot 已完整提交，全部 Publish 边界均已按顺序尝试；资源或观察者异常作为
  `PublishDiagnostics` 返回，不改变该状态。
- `NoOp`：规范化后的目标状态与当前状态结构相等；不创建 snapshot，不发送资源通知或结果事件。
- `Superseded`：请求的 generation 在 Commit 前被更新请求替代；不发布该请求的状态，也不能伪装成成功。
- `Failed`：Capture、Prepare 或 CommitCore 前验证失败；旧状态从未被修改，`ThemeChangeFailed` 已派发。

主题转换自身的失败通过 `Failed` 结果表达，不以异常代替结果。调用参数违反 API 前置条件仍可以同步抛出；调用
方取消等待时，仅该调用返回取消完成，不把底层 transaction 改写为 `Failed`。

等价并发请求加入同一个 in-flight transaction，共享相同 transition id 和 Prepare/Commit 结果。调用方传入的
`CancellationToken` 只取消该调用方的等待，不得取消已经开始的 Commit，也不得把其他等待者的请求取消。
Prepare 阶段只有在所有等待者都已取消且事务尚未进入 Commit 时，Manager 才可以取消底层工作；新 generation
替代旧 generation 时，仍按 `Superseded` 完成未取消的旧等待者。

结果顺序也属于公开契约：`Committed` 先按顺序尝试 variant、context、资源通知和 `ThemeChanged` 的全部 Publish
边界，再携带可能的 warnings 完成对应 Task；`Failed` 先派发 `ThemeChangeFailed`，然后对应 Task 才完成；
`NoOp` 和 `Superseded` 不派发主题结果事件，直接完成结果。

### 12.1 Theme Catalog 手动刷新事务

`ReloadThemesAsync()` 与主题切换共享同一个 Manager mutation scheduler、generation 和 Commit gate，不能在
ThemeTransaction 之外直接替换 `_compiledThemeCatalog`。一次刷新同样遵循五阶段：

1. **Capture（UI thread）**：捕获 Catalog generation、最后一次已提交 root request、当前 ThemeState、
   ScopeGraph stamp 和全部可刷新 Resolver 的不可变快照。
2. **Resolve/Prepare（background）**：重新执行可刷新 Resolver，读取和绑定全部候选来源，合并不可刷新定义，
   构建候选 Catalog。当前 Theme Id 仍存在时使用候选 definition 重编译；不存在时选择静态默认主题，并保留当前
   request 的 `ThemeConfig`。随后按拓扑准备全部局部 snapshot。
3. **CommitCore（UI thread）**：复核 manager generation、Catalog generation、TopologyRevision 和全部 node
   stamp；一次性交换 Catalog、根/局部 snapshot、ThemeState、最后 request 和 cache key。该阶段不通知、不分配、
   不调用 Resolver 或观察者。
4. **Publish（UI thread）**：snapshot 发生变化时依次发布 Application/局部 variant、资源和 `ThemeChanged`；随后
   发布 `ThemeCatalogChanged`。所有事件开始前 `AvailableThemes`、`CurrentTheme` 和全部 Context 已经指向同一代
   状态。
5. **Complete**：返回 `ThemeCatalogReloadResult`。调用方取消只取消自己的等待，不撤销已进入 Commit 的刷新。

刷新结果语义：

- `Committed`：候选 Catalog 或当前有效 snapshot 发生结构变化并完整提交。
- `NoOp`：Resolver 结果、definition revision 和当前 snapshot 均与已提交状态相同，不发布事件。
- `Superseded`：新的主题切换、局部配置更新或 Catalog 刷新改变 generation/stamp，本次不提交。
- `Failed`：Resolver、读取、绑定、冲突、编译或 Commit 前验证失败；旧 Catalog、ThemeState 和全部 snapshot 未
  修改。

用户目录整批原子：任一文件失败时刷新返回 `Failed`，不提交有效子集。第一次启动没有旧用户 Catalog 可保留时，
用户 Resolver 失败只排除整个用户来源层，应用使用完整静态 Catalog 启动，并把 diagnostics 保存到
`ThemeCatalogDiagnostics`。Core/应用资源 Resolver 失败仍阻止启动。

如果用户删除当前主题且其余候选定义全部有效，刷新提交静态默认 Theme Id，并复用最后 request 的 runtime
`ThemeConfig`。这属于 `Committed`，同时发布 `ThemeChanged` 和 `ThemeCatalogChanged`。不保留“CurrentTheme 不在
AvailableThemes 中”的悬空状态。

事件处理期间发起新请求遵守现有重入队列。`ThemeCatalogChanged` 观察者异常只形成 publish warning，不回滚已经
提交的 Catalog；刷新结果仍为 `Committed`。

## 13. 资源解析

根 ThemeContext 的 Provider 在 ThemeManager 初始化时挂载一次。每个局部 ThemeContext 也只拥有一个
Provider。主题切换不追加 `MergedDictionaries`，不创建新的 Provider。

Root Provider 由 ThemeManager 的 Styles 资源拥有；局部 Provider 由对应 ThemeConfigProvider.Resources
拥有；独立 TopLevel 通过自己的 ResourceBridge 委托给 owner context。Provider 和 bridge 都忽略调用方传入的
自定义 AtomUI theme id，只使用已提交 snapshot；Avalonia 的 `ThemeVariant` 参数仍用于同一资源树中的普通
ThemeDictionary 查询。

查询规则：

```text
                             +----------------------+
                             | Resource key         |
                             +----------+-----------+
                                        |
                  +---------------------+---------------------+
                  |                     |                     |
                  v                     v                     v
        +-------------------+ +-------------------+ +-------------------+
        | SharedTokenKind.X | | Control shared    | | Control own       |
        +---------+---------+ +---------+---------+ +---------+---------+
                  |                     |                     |
                  v                     v                     v
        GlobalResources[X]   Control delta[X]       ControlResources[X]
                                        |
                                   miss |
                                        v
                              GlobalResources[X]
```

ControlTheme 中消费的全局 Token 必须使用对应 Control 的共享 Token 复合资源键。普通应用内容和没有
Control Token 身份的通用样式继续使用 `SharedTokenKind`。

### 13.1 AXAML 访问契约

Token 资源的公开 AXAML 语法固定为：

```xml
<!-- 全局共享 Token；在 Control token scope 内自动读取该 Control 的有效共享 Token 差量。 -->
<Setter Property="Background" Value="{atom:SharedTokenResource ColorErrorBg}" />

<!-- Control 自身 Token。 -->
<Setter Property="Padding" Value="{atom:AlertTokenResource DefaultPadding}" />
```

禁止生成或使用以下把 Control identity 拼进 MarkupExtension 类型名的语法：

```xml
<Setter Property="Background" Value="{atom:AlertTokenSharedTokenResource ColorErrorBg}" />
```

`SharedTokenResourceExtension` 根据所在主题资产的 ambient Control token scope 选择 key：

- 没有 Control token scope 时，返回普通 `SharedTokenKind`，读取当前 ThemeContext 的全局资源。
- 存在 Control token scope 时，使用 `(ControlTokenIdentity, SharedTokenKind)` 获取缓存后的内部复合 key，先读取
  对应 Control 的有效共享 Token 差量，miss 时回退全局资源。
- Control 自身 Token 继续使用生成的 `<ControlTokenName>Resource` 扩展，不经过共享 Token fallback。

Control token scope 在一个主题资产边界只声明一次，不在每个 Setter 上重复。Theme 资源文件使用单一、生成式
`ControlTokenScope.Identity` 把 ResourceDictionary 或 ControlTheme 关联到已注册的
`ControlTokenIdentity`：

```xml
<ResourceDictionary xmlns="https://github.com/avaloniaui"
                    xmlns:atom="https://atomui.net"
                    xmlns:generated="using:AtomUI.Generated.AtomUI_Desktop_Controls"
                    atom:ControlTokenScope.Identity="{x:Static generated:AlertThemeAsset.Identity}">
    <!-- 这里的 SharedTokenResource 自动使用 AtomUI:Alert 的共享 Token 投影。 -->
</ResourceDictionary>
```

`ControlTokenScope` 只是 AXAML 构建阶段的主题资产元数据，不创建 ThemeContext、ResourceProvider 或控件实例
作用域，也不参与运行时 Visual 继承。

直接以 `ControlTheme` 为根时在该 ControlTheme 上声明相同 Identity。一个 ResourceDictionary 中存在多个
Control token family 时，应在各自最近的 ControlTheme 上声明；最近的 scope 优先。Identity 必须是生成器为
该资产声明的静态值，禁止使用两个松散的 Catalog/Id 字符串，也禁止根据 TargetType、Token CLR 类型、文件
路径或运行时反射猜测 identity。

MarkupExtension 在 AXAML 构建时从 ambient parent context 捕获 scope，并生成缓存后的稳定 key。资源查询热路径
仍然只接收 key 和 ThemeVariant，不遍历 Visual、不查找 templated parent，也不进行字符串解析。缺少 scope 本身
是合法的全局共享 Token 用法；AtomUI 自有 ControlTheme 资产必须由静态资源审计确认其声明了正确 scope，避免
Control override 被静默当作全局查询。

ambient 行为必须覆盖 eager AXAML、deferred ControlTemplate、ResourceInclude、ThemeDictionary 和以
ControlTheme 为根的资产。不能仅凭普通 ResourceDictionary 样例推断 deferred 场景正确。生成器或构建 analyzer
为每个资产生成 `ControlThemeAssetDescriptor`，并在构建期执行以下校验：

- 资产声明的 Identity 与 Provider 注册的 manifest identity 一致。
- 使用 Control 自身 Token Resource 的资产必须属于相同 Control identity。
- 使用 `SharedTokenResource` 的 AtomUI ControlTheme 必须能在最近资产边界解析到唯一 identity。
- identity 未注册、多个 identity 冲突、deferred parent stack 丢失或资产 URI 重复时构建失败。

运行时注册只加载已经通过 manifest 校验的 descriptor，并再次对照当前 RegistryRevision；它不重新扫描 AXAML
文本。`SharedTokenResourceExtension.ProvideValue` 只在构建阶段解析 ambient identity，并从当前 registry 的
有界预生成 key 表取得稳定 boxed key。

控件实例不创建 Token `ResourceDictionary`，不注册 Control 自己的资源作用域，也不订阅全局主题事件。
Control 配置只修改当前 `ThemeContext` 中指定 `ControlTokenIdentity` 的 Token 投影，不覆盖全局
`SharedToken`，也不由某个控件实例创建私有子作用域。当前 `ThemeContext` 子树中的所有同 identity Control，
包括位于用户 Content 或嵌套控件中的实例，都使用该 Control 配置；其他 Control identity 和子
`ThemeConfigProvider` 的独立 ThemeContext 按各自有效配置解析。

## 14. Imperative Token 消费

少数控件需要在 C# 中计算 palette、断点或运行时绘制值。它们使用 `ThemeTokenResolver`：

- Resolver 必须以明确的 `AvaloniaObject` owner 或 `ThemeContext` 为入口；不能通过
  `ThemeManager.Current`、当前焦点、调用栈或 Visual 全局扫描猜测作用域。
- Resolver 从 owner 继承的稳定 `ThemeContext` 原子读取一次 snapshot；同一次布局、绘制或计算必须只使用该
  snapshot，不能在多个 Token 查询之间重新读取 context 而混合两个事务的值。
- 全局 Token 使用 `SharedTokenKind` 强类型查询。
- Control Token 使用生成 descriptor 中的 `ControlTokenIdentity` 和强类型 Token kind 查询。
- palette 读取 snapshot 中不可变的 `PresetColorPalettes`。
- Resolver 不缓存第二份 Token 对象图，也不把 snapshot、Provider 或 owner 放入静态缓存。
- 需要动态更新时订阅当前 `ThemeContext` 的内部 Publish 通知，并在 detach、re-template 或 owner 变化时释放
  subscription；控件不订阅 `ThemeManager.ThemeChanged`。
- 独立 TopLevel 中的 imperative 查询使用 lease 注入的同一个 owner ThemeContext。无 owner 的静态 API 只能
  显式解析 Root ThemeContext，并遵守根主题语义。

优先把固定视觉结果编译为 Control 资源。只有值确实参与控件运行时算法时才使用 resolver。

## 15. 事件契约

新主题系统只保留结果事件：

- `ThemeChanged`：事务已完成 CommitCore，variant、context 和资源通知已经发布后派发；携带 request、scope、
  transition id、已提交 `ThemeState` 以及派发到该事件之前累计的 Publish warning diagnostics。
- `ThemeChangeFailed`：Capture、Prepare 或 CommitCore 前的 stamp/invariant 验证失败，且任何已发布引用都尚未
  交换时派发；携带 request、scope、transition id、diagnostics 和 exception。

不提供可取消的 Changing 事件，也不保留 Created、AboutToLoad、Loaded、AboutToChange、Unload 等伪生命周期。

事件由统一 `ThemeEventDispatcher` 派发：

- 每个订阅者独立调用。
- 一个订阅者异常不阻止后续订阅者。
- Publish 中设置 AvaloniaProperty、context 通知、资源通知或事件订阅者抛出的异常统一转换为 warning
  diagnostic，并通过 Avalonia Logger 记录完整上下文。
- Publish warning 不触发 `ThemeChangeFailed`，不回滚 snapshot、variant 或已发出的通知，也不把成功提交改写为
  主题失败。
- `ThemeChanged` 订阅者自身产生的 warning 无法回填给已经开始执行的同一事件参数；它必须进入最终
  `ThemeTransitionResult.PublishDiagnostics`，供请求调用方观察。
- 事件处理期间发起的请求遵守 ThemeManager 重入队列。

全局事件由 `IThemeManager` 暴露；局部结果由对应 `ThemeConfigProvider` 暴露。两者使用相同 event args 和
派发规则。内部 `ThemeContext` changed 通知只用于 ResourceBridge 和 imperative 消费者，不是第二套公开主题
生命周期事件。

## 16. 启动时序

`Application.UseAtomUI()` 的主题启动顺序固定为：

1. Builder 解析并冻结 Application Id，收集 Theme Definition Resolver、算法 descriptor、Control theme asset
   manifest、不可变初始 root request 模板和语言配置。
2. 构建并冻结 `ThemeSchemaRegistry`；在此时拒绝重复 Control identity、无效 descriptor、资产 identity/URI
   冲突以及 manifest 与 registry 不一致。
3. 同步执行不可刷新 Resolver；如启用用户主题目录，再同步执行用户 Resolver。Catalog 统一读取并绑定全部成功
   来源，确定静态默认主题和 definition revision。静态来源失败终止启动；用户来源失败降级到静态 Catalog 并
   保存 diagnostics。
4. 如果启用 FollowSystem，先同步读取初始系统 appearance，再从 Light/Dark 不可变 request 模板中选择首帧
   request；不得先按 Default 编译后再补一次 Dark/Light 切换。
5. 同步完成初始 request 的 Capture/Prepare，生成第一个完整根 `ThemeSnapshot`。
6. 使用该 snapshot 创建 Root ThemeContext、稳定 Provider 和唯一 ThemeManager；Manager 的 Styles 同时预装
   Control themes、语言资源入口以及向全部 TopLevel 注入 Root ThemeContext 的全局 style。
7. 将同一个已完整初始化的 ThemeManager 实例挂载到 Application Styles，并绑定为 `IThemeManager` 服务。
8. 显式设置与 snapshot appearance 一致的 Avalonia Light/Dark variant，并且只发布一次初始 Token 资源通知。
9. 执行与主题生命周期无关的模块初始化。

不得先挂载空资源层再异步补主题，也不得通过 ThemeLoaded 事件修补 DefaultFont、断点或其他 Token。

独立 `TopLevel` 的首帧还包括平台窗口在 Avalonia 完成 `ApplyStyling` 前可能展示的客户区表面。宿主必须在调用
平台 `Show` 前挂载目标 `ThemeContext`、设置与 snapshot 一致的显式 variant，并从当前作用域 Snapshot 同步读取窗口
背景 Token，预热 `Background` 与透明度回退背景。该预热只能使用低于用户 local value 的可释放临时值，不创建
资源 observable 或订阅，并在同步样式应用完成后立即释放，由正式 `ControlTheme` 接管；不能依赖 Avalonia
`Window` 的白色默认值，也不能保留第二套长期背景状态。

`ThemeConfigProvider` 首次 attach 也必须在其内容进入首个可见帧前同步解析父 ThemeContext、准备局部 snapshot、
初始化稳定 Provider，并设置与 snapshot 一致的 `RequestedThemeVariant`。首次 Config 无效时直接使用父 snapshot
和父 appearance 作为安全回退；不能先暴露空 context，再依赖后续异步事务纠正。

## 17. 线程与生命周期

- `ThemeConfig`、`ControlThemeConfig` 和 `ThemeRequest` 深度不可变。配置输入只在 UI 线程捕获完整引用；Provider
  只观察 `Config` 属性替换，不订阅 Config 内部集合或 setter 对象。
- ThemeCompiler 不访问 UI 对象，可以在后台准备运行时变更。
- 初始根主题和 ThemeConfigProvider 首次 attach 同步准备，避免首帧闪烁。
- 运行时更新可以后台编译，但 CommitCore、Publish 和 Complete 排序必须回到 UI 线程。
- 手动 Catalog 刷新在后台执行 Resolver、文件读取、Reader、Binder 和编译；Resolver 及 source stream 不进入
  CommitCore，也不能被 snapshot、Catalog 或事件参数长期持有。
- Catalog 刷新与根/局部主题事务共享 scheduler 和 generation，不能并行提交两代 Catalog/snapshot。
- Capture 的 generation、TopologyRevision 和 node stamp 只描述当次不可变图快照；Prepare 不得持有可变
  ScopeNode 集合。提交前 graph stamp 不一致时必须丢弃 staged result，不能尝试修补后提交。
- Scope attach、detach、reparent 和 Content 替换必须在 ThemeScopeGraph 中对称登记和释放。
- snapshot 不持有作用域、Control、Visual 或事件订阅。
- Provider 生命周期归所属 ThemeContext，不进入全局静态缓存。
- Root ThemeContext 生命周期归 ThemeManager；局部 ThemeContext/Provider 生命周期归已注册
  ThemeConfigProvider；Manager dispose 时按拓扑逆序释放局部注册、bridge 和根资源宿主。
- 普通 Popup/Flyout 依靠逻辑父级继承，不创建 lease。独立 TopLevel 的 `ThemeContextLease` 和
  `ThemeContextResourceBridge` 由宿主 owner 关系拥有；owner 改变、关闭、异常打开或 dispose 都必须对称替换或
  释放 local value、资源挂载和 context subscription。
- lease 只强持有其活动宿主和 owner context，不反向让 owner Visual、已关闭 TopLevel 或旧 snapshot 超过宿主
  生命周期存活。
- `UserDirectoryThemeDefinitionResolver` 不创建 `FileSystemWatcher`，不持有打开文件句柄。每次启动或显式刷新
  枚举一次目录，读取完成即释放 stream；ThemeManager dispose 后不保留 Resolver、source、路径集合或旧 Catalog。

## 18. 性能与内存模型

### 18.1 数据布局

源生成器为全局 Token、每个 Control Token 和注册 Control 分配稳定的整数 slot。编译器内部和 snapshot
使用 slot 索引，不在热路径使用 Token name、`PropertyInfo` 或 object-key 字典。

数据布局遵循以下原则：

- 全局 Token 值只在 snapshot 中保存一次。
- Control snapshot 只保存 Control 自身 Token 值和相对全局 Token 的有效差量。
- 不为每个 Control 保存一份完整全局 Token 表。
- 未修改的 palette、不可变 Brush、全局 Token 表和 Control snapshot 在父子 snapshot 之间共享引用。
- Control 表可以复制固定长度的引用数组，但不得复制未变化 Control 的值表和资源表。
- `ThemeSnapshot` 不缓存面向兼容 API 的第二份对象图。

目标内存复杂度是：

```text
root snapshot = O(global token slots + all control-own slots + control deltas)
local scope   = O(global changes + changed control values + control reference table)
runtime       = O(active scopes + bounded cache + one in-flight transaction)
```

禁止退化为 `O(scope count * control count * global token count)` 的完整 Token 克隆模型。

### 18.2 增量编译与结构共享

`ThemeConfigMerger` 除有效配置外还输出确定的 change set：

- 算法或全局 Token 改变时，重新计算全局表和所有依赖全局值的 Control。
- 只修改某个 Control 配置时，仅重新计算该 Control。
- 只修改局部作用域 Button 时，复用父 snapshot 的全局表、palette、资源表和其他 Control snapshot。
- 配置规范化后与当前有效配置相等时直接 no-op，不创建 snapshot，不通知资源。
- 空配置且 `Inherit=true` 的 ThemeConfigProvider 可以直接共享父 snapshot。

单 Control 局部覆盖的结构共享结果如下：

```text
Parent snapshot P
+-- Global table G
+-- Control[Button] B1
+-- Control[Input]  I1
\-- Control[...]    N1

Child snapshot C (Button override only)
+-- Global table G       same reference as P
+-- Control[Button] B2   new value
+-- Control[Input]  I1   same reference as P
\-- Control[...]    N1   same references as P
```

编译缓存分两级：

- 完整 snapshot cache：复用完全等价的有效主题。
- Control compilation cache：以 global token 结构化 key、Control identity、Control config 结构化 key、
  algorithm `(id, revision)` 和 registry revision 为 key，复用跨作用域的 Control 计算结果；fingerprint 只用于
  候选定位，命中后仍执行结构相等比较。

两级缓存共用容量策略，失败和取消结果不缓存。

默认容量由 `ThemeCacheOptions` 固定为：

- 完整 snapshot cache：最多 32 个 entry，最多 32 MiB `EstimatedRetainedBytes`。
- Control compilation cache：最多 2048 个 entry，最多 32 MiB `EstimatedRetainedBytes`。

每个 entry 在插入前按第 9 节的保守公式计算 retained bytes；共享引用允许重复计入以维持上界保守性，但不能
通过抽样 RSS、GC generation 或固定“每个 Token 一个单位”替代。任一 entry 单独超过对应 bytes 上限时可以返回
给当前事务使用，但不得进入 LRU。

容量可以由应用显式调小；扩大默认值必须附带 benchmark、峰值对象计数和 Gallery 导航内存结果。

### 18.3 编译分配

- 编译前根据 schema 中已知的 Token 和 Control 数量预分配数组及稀疏字典容量。
- 临时 builder 使用 copy-on-write global token overlay，Control 计算不得深克隆完整 `DesignToken` 对象图。
- 大型临时数组可以由有界 workspace pool 复用，但 pooled buffer 绝不进入 snapshot。
- 取消、异常和 stale generation 路径必须在 `finally` 中归还 workspace。
- 同一 ThemeManager 同时只运行一个根事务准备管线；后续请求合并，避免并发编译造成内存峰值。
- 初始同步编译完成后及时释放 ThemeDocument 中不再需要的 XML 对象和临时字符串。

### 18.4 资源查询热路径

`ThemeTokenResourceProvider.TryGetResource` 是控件布局和样式更新的热路径，要求：

- global、Control shared 和 Control own 命中查询均为零托管分配。
- `SharedTokenResourceExtension` 在 AXAML 构建阶段根据 ambient scope 获取缓存后的稳定 key；资源查询时不创建
  key、不拼接字符串，也不解析 Control identity。
- `ControlSharedTokenResourceKey` 直接包含 `ControlTokenIdentity` 或 registry slot，不在每次查询时重新构造身份。
- `SharedTokenKind` 对应的全局资源 key 在 `ThemeSchemaRegistry` 冻结时预装箱并按 slot 保存；Control shared 查询
  必须复用该对象，禁止把枚举传给 `object` 参数造成每次查询装箱。
- 查询只执行类型分支、slot 索引和最多一次稀疏差量 fallback。
- miss 不创建 diagnostics、异常或临时集合。
- 一次事务对每个受影响 Provider 最多调用一次 `RaiseResourcesChanged()`。

资源通知使用最小通知前沿：

- 根事务只通知 Root Provider；Application/TopLevel 的既有资源传播会使普通逻辑树中的局部 Provider 重新查询，
  不再逐个通知所有后代 scope。
- 局部事务只通知 affected subtree 的最高变化节点 Provider；其后代 snapshot 已在 CommitCore 替换，随父通知
  重新查询。
- 脱离该逻辑树的活动 ThemeContextResourceBridge 需要各通知一次，因为它们是独立资源根。
- appearance 变化的每个显式 ThemeVariantScope 仍必须设置自己的 RequestedThemeVariant；子 scope 的 local
  value 会阻断父 variant 继承，不能只更新根。

### 18.5 生命周期与可回收性

ThemeScopeGraph 在注册有效期间强持有 ScopeNode 和对应 ThemeConfigProvider；Provider detach 时必须通过
注册 token 同步移除节点和全部边。Runtime 不保存已 detach 节点历史。

- 一个活动 ThemeConfigProvider 对应一个 ScopeNode、ThemeContext 和 ResourceProvider。
- ScopeNode 除已 attach 的 ThemeConfigProvider 外不能单独持有 Content、后代 Control 或其他 Visual。
- ThemeConfigProvider 持有 registration disposable；detach、reparent 和异常 attach 路径都必须释放它。
- 一个活动独立 TopLevel 对应一个 `ThemeContextLease` 和一个宿主私有 `ThemeContextResourceBridge`；bridge 只持有
  owner ThemeContext 和通知 subscription，不复制 snapshot 或 Token 表。
- owner 变化必须先建立新 lease 的完整 local context/variant/resource bridge，再释放旧 lease；关闭或创建失败
  后不得在 owner context 的订阅列表中残留 bridge。
- imperative Token 订阅返回 `IDisposable`，由控件在 detach、re-template 或 owner 变化时释放。
- 非 Visual `AvaloniaObject` 使用 DynamicResource 时必须有 scoped `IResourceHost` 和对称 release 路径。
- 普通 Popup、Flyout 和逻辑树内 overlay 不额外创建 ThemeContext 传播订阅；仅独立资源根使用 lease/bridge。
- static cache 只能持有 schema、descriptor 和不可变无 UI 值，不得持有 ThemeContext、Provider、Control 或 Visual。
- 事件 dispatcher 不保留已完成 transaction、event args 或 invocation list。

旧 snapshot 的可达数量上界必须由“活动 context 数量 + 有界 cache entry + 当前事务 staged snapshot”解释。
无法解释的额外存活 snapshot 视为内存泄漏。

### 18.6 性能验证

主题系统必须建立可重复的 benchmark 和 allocation 场景：

- 冷启动：registry、主题文件绑定、默认主题编译和首次挂载。
- 热启动：相同 definition 和 config 的缓存命中。
- 全局切换：Default、Dark、Compact、Dark + Compact。
- 局部变更：只覆盖一个 Control、只覆盖一个全局 Token、十层嵌套作用域。
- 高频变更：连续 100 次配置更新后的请求合并和 stale result 丢弃。
- 拓扑竞争：Prepare 期间执行 scope detach、reattach、reparent 和 Config 替换，验证旧 graph stamp 从不提交。
- FollowSystem：启动时分别模拟 Light/Dark，并连续切换平台 appearance，验证不存在 `Default` variant 或
  snapshot/variant 中间不一致。
- 资源查询：global、Control shared、Control own 的 hit、fallback 和 miss。
- 独立 TopLevel：根 owner、局部 owner、owner 替换和无 owner 静态 API 的资源与 appearance 语义。
- 生命周期：100 次 Provider attach/detach、Popup/Flyout open/close、独立 TopLevel lease/bridge 创建释放和
  Gallery 页面导航。

验收指标至少包括：

- `TryGetResource` 稳态查询为 0 B/op。
- no-op 请求不创建 snapshot、Provider、Context，不派发资源或主题事件。
- 单 Control 局部覆盖只产生一个新 Control snapshot，其他 Control snapshot 引用保持不变。
- 重复更新后每个作用域仍只有一个 Provider。
- 两级 cache 的 entry 数量和 `EstimatedRetainedBytes` 始终不超过各自配置上限；超大单 entry 不进入 LRU。
- attach/detach 后通过 WeakReference 和强制 GC 验证 ScopeNode、ThemeContext、Provider 及旧 Content 可回收。
- 独立 TopLevel 关闭或 owner 替换后，通过 WeakReference 和订阅计数验证 lease、bridge、旧 owner Visual 和旧
  snapshot 可回收。
- Gallery 随机导航稳定后旧 ShowCase、DynamicResourceExpression 和主题作用域对象数量不单调增长。
- RSS 只作为辅助信息；对象数量和可达链才是内存泄漏判据。

## 19. AOT 约束

- 内置 Token 注册和 schema 全部由源生成器生成。
- 不运行时扫描程序集或 Token 属性。
- 不使用 `Activator.CreateInstance` 创建内置 Control Token。
- 不使用字符串属性路径 Binding 完成主题配置或 Token 查询。
- Application Id 默认值只读取具体 Application 类型所在程序集的简单名称，不扫描已加载程序集。
- 内置和应用资源 Resolver 只使用显式 `avares://` URI；用户 Resolver 只枚举显式配置目录的顶层
  `*.theme.xml`，不反射发现 Resolver、Theme 或 Token。
- 手动刷新不使用 `FileSystemWatcher`、动态代码生成或运行时类型构造。
- 自定义算法和第三方 Control Token 必须提供稳定 descriptor id；自定义算法还必须提供显式 revision/version。
- 生成器测试必须验证 identity、继承 Token、强类型 setter、资源投影和输出稳定性。
- 主题系统完成后必须执行真实 Gallery NativeAOT publish，不能只依赖 analyzer。

## 20. 模块组织

主题源码按稳定职责边界组织，不为少量运行时入口创建目录，也不保留 `Catalog`、`Scope`、`Engine`、
`ControlThemes`、`Styling`、`Transitions` 或 `TokenSystem` 这种过细、语义重叠或已经失真的目录。

```text
Theme/
|
+-- IThemeManager.cs                  公开主题管理入口
+-- IThemeManagerBuilder.cs           公开注册入口
+-- ThemeManager.cs                   唯一运行时协调者、snapshot 发布者和 Styles 宿主
+-- ThemeManagerBuilder.cs            启动组装器
+-- ThemeConfigProvider.cs            局部主题配置宿主
+-- ThemeTransaction.cs               Capture/Prepare/CommitCore/Publish/Complete 事务
+-- ThemeCatalogReloadTransaction.cs  Catalog 与全部受影响 Snapshot 的原子刷新事务
+-- ThemeState.cs                     当前已提交主题状态
+-- ThemeTransitionResult.cs          事务结果和值状态
+-- ThemeCatalogReloadResult.cs       Catalog 刷新结果和值状态
+-- ThemeChangedEventArgs.cs          成功结果事件
+-- ThemeChangeFailedEventArgs.cs     失败结果事件
+-- ThemeCatalogChangedEventArgs.cs   Catalog 成功刷新事件
+-- ThemeDiagnostic.cs                结构化诊断
+-- ThemeContext.cs                   稳定主题上下文
+-- ThemeContextLease.cs              独立 TopLevel 的 owner context 生命周期租约
+-- ThemeContextResourceBridge.cs     独立资源根到 owner context 的稳定资源代理
+-- ThemeScope.cs                     唯一可继承的 Context attached property
+-- ThemeScopeGraph.cs                局部主题作用域图
+-- ThemeScopeNode.cs                 作用域节点
+-- ThemeRequest.cs                   主题变更请求
+-- ThemeEventDispatcher.cs           结果事件派发
+-- ThemeExceptions.cs                公开错误类型
+-- ThemeAppearance.cs                snapshot 的 Light/Dark 外观
+-- ThemeInfo.cs                      可用主题元数据
|
+-- Algorithms/
|   +-- 算法 descriptor、attribute、接口与内置算法
|   +-- Default、Dark、Compact 计算器
|   \-- 调色板、颜色映射和算法计算辅助
|
+-- Configuration/
|   +-- ThemeConfig、ControlThemeConfig
|   +-- ControlAlgorithmMode
|   +-- Normalizer、Merger 和 immutable normalized config
|   \-- 一次性 immutable config builder 与结构化 value key
|
+-- Definitions/
|   +-- Resolver/source contracts、内置/avares/用户目录 Resolver
|   +-- CompiledThemeCatalog、ThemeInfo、definition revision 和默认主题选择
|   +-- XML reader、syntax document 和 diagnostics
|   \-- Binder、typed ThemeDefinition 和 bind result
|
+-- Schema/
|   +-- ThemeSchemaRegistry
|   +-- TokenDescriptor、ControlTokenDescriptor
|   +-- ThemeAlgorithmDescriptor
|   +-- ControlThemeAssetDescriptor 和 asset manifest contracts
|   \-- identity、stage、parser 和 resource projector contracts
|
+-- Compilation/
|   +-- CompileInput、Normalizer output consumption
|   +-- ThemeCompiler、TokenValueTable
|   +-- ThemeSnapshot、ControlThemeSnapshot
|   \-- bounded snapshot/control compilation caches
|
+-- Resources/
|   +-- ThemeTokenResourceProvider、ThemeTokenResolver
|   +-- SharedTokenResource、ControlTokenScope、Control own resource extensions 和内部复合 keys
|   +-- Control theme provider contracts and attributes
|   +-- BaseControlTheme and provider aggregation
|   \-- snapshot lookup、style/template 和 Control theme 加载辅助
|
+-- DesignTokens/
|   +-- global/control compile-only builders
|   +-- Token definitions and kind metadata
|   \-- Token value converters
```

`AtomUI.Generator` 根据每个控件包的 AXAML 资产生成 `ControlThemeAssetDescriptor`、manifest 和稳定
`ControlTokenScope.Identity` 引用，并通过构建 analyzer 校验 deferred template、ResourceInclude、
ThemeDictionary 和 ControlTheme 根场景。运行时 `Theme/Schema` 只消费生成结果，不解析 AXAML 文本，也不通过
反射补全缺失 identity。

`Definitions` 同时拥有主题来源和主题定义，不再用单独的 `Catalog` 目录把一次加载流程拆开。
`ThemeManager`、事务、上下文和作用域图都属于主题子系统的核心运行时入口，直接位于 `Theme` 根目录；
这些类型共享同一个 snapshot 提交边界，但不需要额外的 `ThemeEngine` 长期对象或物理目录表达这一事实。

物理目录与 CLR namespace 必须严格一一对应：

| 物理边界 | CLR namespace |
|---|---|
| `Theme/*.cs` | `AtomUI.Theme` |
| `Theme/Algorithms/**` | `AtomUI.Theme.Algorithms` |
| `Theme/Compilation/**` | `AtomUI.Theme.Compilation` |
| `Theme/Configuration/**` | `AtomUI.Theme.Configuration` |
| `Theme/Definitions/**` | `AtomUI.Theme.Definitions` |
| `Theme/Resources/**` | `AtomUI.Theme.Resources` |
| `Theme/Schema/**` | `AtomUI.Theme.Schema` |
| `Theme/DesignTokens/**` | `AtomUI.Theme.DesignTokens` |

职责目录下用于拆分 partial class 或文件数量的组织子目录不继续创建 CLR namespace，例如
`Theme/DesignTokens/Definitions/**` 仍使用 `AtomUI.Theme.DesignTokens`。根目录不保留 `.Scope`、
`.Transitions` 等已经被物理结构删除的 namespace。
`Theme/Algorithms` 与 `AtomUI.Theme.Algorithms` 一一对应：`ThemeAlgorithm`、算法 contract/attribute、内置
calculator、调色板生成、预设色、`ColorMap` 和计算辅助均归该命名空间所有。调色板不是独立运行时子系统，
因此不保留 `AtomUI.Theme.Palette`；算法实现也不属于资源样式层，因此不保留
`AtomUI.Theme.Styling` 兼容命名空间。
AXAML Control theme 的发现、聚合和加载属于主题资源所有权，统一位于 `AtomUI.Theme.Resources`，不参与 Token
编译。`SharedTokenKind`、Token Resource markup extension 和相关生成代码同样输出到该 namespace；不保留
`AtomUI.Theme.Styling`。Token builder、Token kind、Token attribute 和 value converter 统一位于
`AtomUI.Theme.DesignTokens`，不保留 `AtomUI.Theme.Tokens` 或 `AtomUI.Theme.TokenSystem`。源生成器必须使用
相同映射，禁止构建后重新生成旧 namespace。

依赖方向固定为：Definitions/Configuration/Schema 提供输入，Compilation 产生 snapshot，根目录的
ThemeManager 提交 snapshot，Resources 只读取已提交 snapshot。Compilation 不依赖 Avalonia 资源宿主，Resources
不执行编译，ThemeConfigProvider 不拥有编译器。每个程序集生成自己的
`AtomUI.Generated.<OwnerAssemblyIdentifier>.GeneratedThemeSchema`，程序集标识中的非标识符字符统一
替换为下划线，禁止多个程序集生成相同全限定类型名，也禁止把生成 namespace 嵌套在可能同名的 Control
类型下面。

## 21. 明确删除的旧设计

新架构实现时删除：

- `Theme`、`ITheme` 以及预创建 Theme variant pool。
- `ThemeCoordinator` 以及一对一 `ThemeManager`/`ThemeEngine` facade 分层；事务队列和当前状态只有 Manager 一个
  长期所有者。
- `Theme.SharedToken`、`Theme.GetControlToken()`、`ThemeConfigProvider.SharedToken`、
  `ThemeConfigProvider.ControlTokens` 和所有 clone compatibility map。
- `SharedTokenSetters`、`ControlTokenInfoSetters`、字符串算法集合、Token 查询属性以及对可变配置对象图的
  collection/property changed 订阅。
- snapshot 中的 `DesignToken`、`IControlDesignToken` 和对应 Clone API。
- `IDesignToken`、`IControlDesignToken` 中面向运行时修改和资源构建的接口。
- `ThemeScope.SnapshotProperty` 和基于 snapshot attached value 的父级订阅。
- `TokenFinderUtils` 和沿 StylingParent 查找旧 Provider 的逻辑。
- `ThemeCreated`、`ThemeAboutToLoad`、`ThemeLoaded`、`ThemeLoadFailed`、`ThemeAboutToChange`。
- 对 Theme ResourceDictionary 的 Motion/Wave 直接修改。
- 自定义主题 id 编码的 Avalonia ThemeVariant。
- `ControlTokenRegistration(Type)`、反射 schema、Activator fallback 和对应 AOT suppress。
- `ControlTokenScope.Catalog`、`ControlTokenScope.Id` 两字段身份以及按 TargetType、CLR Token 类型或资产路径推断
  Control identity 的逻辑。
- cache pin、父 snapshot Version cache key 和字符串拼接 cache key。
- 旧主题 XML 的 `ControlTokens/ControlToken` 结构。

这些内容不建立 adapter，不标记 obsolete 后继续保留，也不作为迁移阶段的内部真源。

## 22. 验收标准

### 22.1 语义

- 全局 Tokens、Controls、Algorithms 和 Inherit 遵守本文定义的配置合并规则。
- 顶层 ThemeConfigProvider 默认继承当前全局主题。
- 嵌套 Provider 正确合并父 Tokens、Control 配置和算法四态。
- 局部 `Inherit=false` 从 AtomUI library defaults 开始，不继承当前主题定义或父作用域配置。
- Control 覆盖不修改全局 SharedToken，也不创建实例私有作用域；同一 ThemeContext 中全部同 identity Control
  使用一致的覆盖结果。
- 同一主题的新 runtime override 会产生新有效配置，不复用过期 Theme 对象。
- `Algorithms=null` 继承基线算法，空算法列表被诊断为无效输入。
- `ThemeConfig`、`ControlThemeConfig`、`ThemeRequest` 及其全部集合深度不可变；替换 Provider.Config 是唯一局部
  动态配置入口，修改旧输入对象不可能触发事务。
- 所有全局和 Control 算法严格遵守 `Evaluate(sameEffectiveSeed, previousMap?) -> nextMap`；后续算法不能把前一
  Map 当作新的 Seed，也不能修改输入。
- 根和每个 `ThemeConfigProvider` 的 snapshot appearance 始终等于其 Avalonia
  `ActualThemeVariant`；Control custom algorithm 的 appearance 不泄漏到作用域 variant。
- 无 owner 的静态 Dialog、Notification 等 API 始终使用 Root ThemeContext，并产生可诊断的根主题语义，不能
  根据当前焦点猜测局部作用域。
- Resolver 只产生 source，不产生 `ThemeConfig`、`ThemeDocument`、Token 或 snapshot；所有来源统一经过标准
  Reader/Binder/Compiler。
- 默认 Application Id 来自具体 Application 类型程序集名，显式 `WithApplicationId` 可稳定覆盖；
  `Application.Name` 不参与路径身份。
- 用户主题目录固定为应用配置根下 `{ApplicationId}/Themes`，不递归、不跟随链接、不访问网络。
- 用户主题不得覆盖静态 Theme Id、不得声明默认主题；任意重复 Id 都产生确定 diagnostic。

### 22.2 事务

- Dark + Compact 只产生一次完整提交。
- 根主题变化和所有受影响局部作用域在通知前完成引用替换。
- 配置失败、编译失败和 stale result 不发布部分状态。
- `ApplyThemeAsync` 分别返回可区分的 `Committed`、`NoOp`、`Superseded` 和 `Failed` 结果；被替代请求不返回
  成功；`Failed` 只发生在任何已发布引用交换之前。
- Capture 后 TopologyRevision、RegistrationId、ParentRegistrationId 或 ConfigRevision 任一变化都会使 staged
  result 成为 `Superseded`，旧结果绝不提交到新拓扑。
- Publish 开始后不回滚。AvaloniaProperty、资源通知或观察者异常被记录为 publish warning，事务仍返回
  `Committed`，最终结果携带完整 `PublishDiagnostics`。
- 等价并发请求共享一次 Prepare/Commit；单个等待者取消不取消其他等待者或已经开始的 Commit。
- `ThemeChanged` 或 `ThemeChangeFailed` 的派发先于对应 transition Task 完成。
- 重入和高频更新不会递归提交，latest request 语义确定。
- 订阅者异常不阻断提交或其他订阅者。
- FollowSystem 在首帧前解析初始系统 appearance，运行期切换只发布完整 Light/Dark snapshot 和对应显式
  variant，不出现 `ThemeVariant.Default` 或新旧状态混合帧。
- `ReloadThemesAsync` 对用户目录执行整批原子刷新；任一文件失败时旧 Catalog 与全部 snapshot 保持不变。
- Catalog 刷新提交前同时验证 Catalog generation 和 ScopeGraph stamps；任何观察者只能看到同一代 Catalog、
  CurrentTheme 和 root/local snapshots。
- 当前用户主题被删除时刷新原子回退静态默认主题，并保留最后一次 runtime ThemeConfig。
- Catalog 内容相等时返回 `NoOp`，较新主题或刷新请求使旧刷新返回 `Superseded`。
- `ThemeCatalogChanged` 在完整提交与必要的资源/ThemeChanged 发布后派发，先于 reload Task 完成；订阅者异常只形成
  publish warning。

### 22.3 资源与生命周期

- 根作用域和每个局部作用域始终只有一个 Token ResourceProvider。
- 重复更新不增长 `MergedDictionaries`。
- 控件实例不创建 Control Token 资源字典。
- 所有共享 Token AXAML 引用使用 `{atom:SharedTokenResource ...}`；生产源码、生成输出和 AXAML 资产中不存在
  `*TokenSharedTokenResourceExtension` 类型或 `{atom:*TokenSharedTokenResource ...}` 用法。
- global、ambient Control scope、Control delta hit 和 global fallback 均返回正确值；不同 Control identity 的同名
  SharedToken 不串值。
- AtomUI ControlTheme 资产都具有可静态验证的 Control token scope；scope identity 未注册或与资产声明不一致时
  在主题注册阶段失败。
- Popup、Flyout、Dialog 和窗口覆盖层继承 owner ThemeContext。
- 独立 `TopLevel` 在平台 `Show` 前已获得目标 ThemeContext、显式 Light/Dark variant 和作用域窗口背景，暗色初始
  主题不会暴露 Avalonia 的白色默认客户区；临时首帧值不覆盖用户 local value、不建立资源订阅，并在样式接管后释放。
- 普通 Popup/Flyout 依靠逻辑树自然继承；独立 TopLevel 通过一个可释放 lease 和宿主私有 ResourceBridge 继承
  owner context。owner 替换后资源、imperative Token 和 variant 同时切换。
- detach、reparent、关闭和回收后不保留旧作用域或 Control。
- 独立 TopLevel 关闭、owner 替换和异常创建后不保留旧 bridge、lease、owner Visual 或 context subscription。
- 替换 `ThemeConfigProvider.Config` 后旧配置和旧集合变化不再触发 Capture；detach 后配置订阅全部释放。

### 22.4 性能、内存与 AOT

- 同一 definition revision 只成功读取一次，同一 `(definition revision, registry revision)` 只成功绑定一次；
  来源内容变化使 Reader/Binder 缓存失效，descriptor revision 变化只使 Binder/Compiler 缓存失效。
- 用户目录文件数、总字节和单文件字节均受限；超限、symlink/reparse point、目录逃逸和读取竞态有覆盖测试。
- NativeAOT 下可以使用默认程序集名解析 Application Id、读取用户目录、手动刷新并切换到用户主题。
- 等价有效配置只编译一次。
- 人工构造相同 fingerprint、不同结构化配置的碰撞测试不会错误复用 snapshot 或 Control 编译结果。
- 不同 registry revision 允许 slot 重新分配，绑定、缓存和资源查询仍解析到当前 revision 的正确 slot。
- 单 Control 配置变化复用全局表和其他 Control snapshot。
- Token 资源查询热路径保持零分配。
- cache 和旧 snapshot 存活数量满足本文定义的容量上界。
- ThemeConfigProvider、Popup、Flyout 和 Gallery 导航生命周期 WeakReference 测试通过。
- 正常主题切换路径不执行 Token 反射或 Activator。
- Core、Controls、Desktop、DataGrid、ColorPicker、Extras、GalleryBase 和 Gallery 相关测试通过。
- Release 多目标构建和 Gallery NativeAOT publish 通过。
- `git diff --check` 通过，仓库搜索不存在已删除旧 API 的引用。
