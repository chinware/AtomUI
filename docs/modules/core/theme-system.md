# AtomUI 主题系统架构

本文是 AtomUI 主题系统唯一的长期架构设计文档。它定义最终运行时模型、公开配置语义、主题文件处理、
Token 编译、动态作用域、资源发布、事件契约、AOT 边界和验收标准。阶段性分析、迁移过程和任务进度
不写入本文，统一放在 `docs/superpowers/`。

本文描述目标架构。实现过程中不得为了保留旧主题系统 API 而偏离这些约束。

## 1. 设计目标

主题系统必须满足以下不可协商的约束：

- 建立完整的 Seed Token、Map Token、Alias Token、Control Token、算法和嵌套配置模型。
- AtomUI 的公开 API、内部类型、资源键和文档统一使用 `Control` 术语。
- `ThemeSnapshot` 是运行时 Token 值的唯一真源。
- 全局主题和局部主题使用同一个配置合并器、编译器、缓存和提交协议。
- 主题变更先完成整个受影响作用域子树的编译，再一次性提交，不发布半成品状态。
- 控件级配置只参与目标控件自身的 Token 计算，不形成控件内容树资源作用域。
- 每个主题作用域只拥有一个稳定的 Token 资源 Provider。
- Token 资源查询热路径不分配对象，不执行字符串解析、反射或 LINQ。
- 局部主题只复制发生变化的数据，不能按作用域复制完整全局 Token 和全部 Control Token 值。
- 缓存、作用域图、事件订阅和异步编译任务都必须有确定的容量或释放边界。
- 内置正常路径不使用运行时程序集扫描、`Activator.CreateInstance` 或 Token 属性反射。
- 第一个可见帧之前完成全局主题准备和挂载。
- 不保留旧主题系统的兼容对象、克隆查询 API、伪生命周期事件或可变资源旁路。

## 2. ThemeConfig 设计结论

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

## 3. 总体架构

定义与编译路径：

```text
+-------------+     +---------------------+     +-----------------------+
| Theme files | --> | ThemeDocumentReader | --> | ThemeDefinitionBinder | --+
+-------------+     +---------------------+     +-----------------------+   |
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
| ThemeSnapshotCache | -----> | ThemeEngine  |
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

系统分成六个边界：

- 定义层：读取主题文件并生成与运行时注册无关的语法模型。
- Schema 层：提供生成式 Token 元数据、强类型赋值器、资源投影器和 Control 身份。
- 配置层：捕获可变 UI 输入，生成不可变规范化配置，并执行 AtomUI 主题配置合并。
- 编译层：纯计算 Token，输出不可变 `ThemeSnapshot`。
- 运行时层：管理根主题、作用域图、事务、并发、缓存和事件。
- 资源层：把当前 snapshot 投影到 Avalonia 资源查询，不拥有第二份 Token 状态。

## 4. 配置模型

### 4.1 公开输入

`ThemeConfig` 是面向 C#、AXAML 和绑定的配置输入：

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

`ThemeConfig` 及其集合可以是可观察对象，以支持 AXAML 和运行时绑定。它们只属于输入层，不允许被
`ThemeSnapshot`、资源 Provider 或控件运行时持有。

`ThemeConfigProvider` 是轻量 `Decorator`，只公开一个 `Config` 和继承的内容子节点。旧的
`SharedTokenSetters`、`ControlTokenInfoSetters`、字符串算法集合以及 Token 查询属性不属于新架构。

### 4.2 规范化配置

`ThemeConfigNormalizer` 在 UI 线程捕获一次完整输入，完成以下工作：

- 解析并验证 Token key 和 value。
- 把 Control id 解析为 `ControlTokenIdentity`。
- 保留算法的“未指定”状态。
- 将字典和列表复制为不可变、有确定顺序的值对象。
- 计算稳定的结构 fingerprint。
- 返回 diagnostics，不抛出缺少上下文的反射异常。

编译器只接收 `NormalizedThemeConfig`。配置对象在捕获后继续变化，不得影响正在执行的编译。

### 4.3 合并规则

`ThemeConfigMerger` 是全局和局部主题唯一的合并入口：

- `Inherit=true` 时以父作用域的有效配置为基础。
- `Inherit=false` 时以 AtomUI 默认配置为基础，不继承当前全局主题。
- 当前 `Tokens` 按 key 覆盖父 `Tokens`。
- `Controls` 先按 `ControlTokenIdentity` 合并，再按 Token key 合并。
- Control 的算法未指定时继承父策略；显式 disabled、global 或 custom 时替换父策略。
- 当前 `Algorithms` 未指定时继承父算法；显式提供时整体替换，不和父算法拼接。
- 主题定义、应用级配置和运行时请求都先转换为规范化配置，再按同一规则合并。

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
    \-- evaluator, AOT factory, appearance effect
```

源生成器必须为内置 Token 生成：

- 稳定的 `ControlTokenIdentity`，包括 catalog 和 control id。
- Token name 对应的强类型赋值委托查找表。
- Token value 到 Avalonia resource value 的投影。
- Control Token builder 的直接构造委托。
- Control 自身 Token schema 和继承的 Token schema。
- 生成式 Control 资源键和共享 Token 复合资源键扩展。

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
定义是否可用于当前 registry。主题文件每个 revision 在进程中最多读取和绑定一次。

## 7. 编译器

`ThemeCompiler` 是无 UI、无资源宿主、无全局服务访问的纯计算入口：

```text
ThemeCompileInput
  -> merge effective config
  -> initialize global seed token builder
  -> apply global seed overrides
  -> run ordered algorithms
  -> apply map and alias overrides
  -> evaluate each registered Control
  -> freeze token values and resources
  -> ThemeSnapshot
```

编译阶段允许使用可变 builder，因为算法天然需要逐步派生。builder 的生命周期只限于一次编译，不能进入
snapshot，也不能通过公开 API 返回。

每个 Control 的计算顺序为：

1. 从全局最终 Token 值创建 Control 有效 Token builder。
2. 应用该 Control 覆盖的全局 Seed Token。
3. 根据 `ControlThemeConfig.Algorithm` 决定是否重新运行算法。
4. 应用 Control 覆盖的 Map 和 Alias Token。
5. 计算 Control 自身默认 Token。
6. 应用 Control 自身 Token 覆盖。
7. 生成共享 Token 差量、Control Token 值和资源投影。

任何 validation、conversion 或 algorithm 错误都返回失败结果，不返回部分 snapshot。

## 8. ThemeSnapshot

`ThemeSnapshot` 是 Token 编译结果和资源发布单元：

```text
ThemeSnapshot
+-- ThemeId / ContentFingerprint / Algorithms / IsDark
+-- EffectiveConfig
+-- GlobalTokenValues: dense immutable TokenValueTable
+-- GlobalResources
+-- PresetColorPalettes
\-- Controls[registry control slot]
    +-- EffectiveGlobalTokenDelta
    +-- ControlTokenValues: dense immutable TokenValueTable
    \-- ControlResources
```

具体约束：

- Token schema 由生成器分配稳定 slot；完整 Token 值表使用稠密不可变数组，不使用 object-key 字典保存每个值。
- Control 表按 registry slot 存放引用，`ControlTokenIdentity` 只用于配置边界和诊断。
- 稀疏覆盖、共享 Token 差量和 Avalonia 资源投影使用不可变集合或 `FrozenDictionary`。
- 颜色资源在 snapshot 中已经转换为不可变 Brush。
- palette、集合和其他引用值必须在冻结前深复制为不可变值。
- 不保存 `DesignToken`、`IControlDesignToken`、Control、Visual、Provider、事件或订阅。
- 不提供 `SharedToken`、`ControlToken`、`Clone` 或可变字典查询 API。
- imperative 消费者通过只读 Token resolver 查询 snapshot 值。
- snapshot 内容身份由 `ContentFingerprint` 表达，不使用父 snapshot 的递增版本表达语义身份。

## 9. 缓存

`ThemeSnapshotCache` 使用结构化 value key：

- Theme definition revision。
- `NormalizedThemeConfig` fingerprint。
- 有序算法 descriptor id。
- `ThemeSchemaRegistry` revision。

父主题的影响已经包含在合并后的有效配置中，因此 cache key 不包含父 snapshot 的发布序号。

缓存要求：

- 等价内容返回同一个 snapshot 引用。
- 并发等价请求只执行一次编译。
- 失败结果不进入缓存。
- 缓存采用同时受 entry 数量和 token-slot 权重约束的 LRU；达到任一上限即回收最旧 inactive entry。
- snapshot 权重由全局 Token slot、Control Token slot、差量和资源条目数计算，不依赖不稳定的进程 RSS 估算。
- 作用域本身持有当前 snapshot，因此不需要 pin 机制。
- Provider 实例不进入缓存，不在不同资源宿主之间共享。
- 不建立无上限的 Brush、字符串、resource key 或 fingerprint intern pool。

## 10. ThemeEngine 与全局主题

`ThemeEngine` 是唯一允许发布 snapshot 的对象。`ThemeManager` 是公开 facade 和 Avalonia `Styles`
宿主，不再拥有第二份 Theme Token 状态。

新公开模型由以下对象组成：

- `ThemeInfo`：Catalog 中可选择主题的只读元数据。
- `ThemeRequest`：主题 id、`ThemeConfig` 和变更原因。
- `ThemeState`：当前已提交主题的 id、算法、dark 状态、fingerprint 和 transition id。
- `IThemeManager.ApplyThemeAsync(...)`：运行时主题请求入口。
- `IThemeManager.CurrentTheme`：只读 `ThemeState`。
- `IThemeManagerBuilder.WithInitialTheme(...)`：首帧主题 id 和 ThemeConfig 的唯一配置入口。

删除 `Theme` 和 `ITheme`。Catalog 只保存 definition，不为 Default、Dark、Compact 预先创建 Theme 对象。
每个请求都由 definition、配置和算法构成规范化输入，因此同一主题的新 runtime override 必须重新查询缓存
或编译。

AtomUI 主题身份与 Avalonia `ThemeVariant` 解耦：

- AtomUI 使用 `ThemeRequest` 表达主题 id 和算法。
- `Application.RequestedThemeVariant` 只根据已提交 snapshot 设置为 Avalonia Light 或 Dark。
- Compact 和自定义主题 id 不编码进 `ThemeVariant` 名称。
- 跟随系统主题是一种明确的运行时策略，不通过双向绑定自定义 ThemeVariant 实现。

Motion 和 Wave 配置也是 ThemeConfig Token override。任何入口改变这些值时都生成新请求，不直接修改
`ResourceDictionary`。

## 11. 动态作用域

### 11.1 稳定 ThemeContext

`ThemeContext` 是稳定的作用域身份，包含：

- 当前 `ThemeSnapshot` 的原子引用。
- 唯一的 `ThemeTokenResourceProvider`。
- 内部 context changed 通知。

`ThemeContext` 不复制或修改 Token。snapshot 仍是 Token 值唯一真源。

`ThemeScope.ContextProperty` 在树上传递稳定 context，而不是每次主题变化都替换 attached snapshot。
没有局部 Provider 时，控件解析到根 `ThemeContext`。

### 11.2 ThemeScopeGraph

`ThemeEngine` 维护活动作用域图：

```text
+---------------------------+
| ThemeEngine               |
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

每个 `ThemeScopeNode` 保存父节点、当前规范化配置、最后一次有效配置、ThemeContext、Provider 和子节点。

`ThemeConfigProvider` attach 时注册节点，detach 时注销节点，父级变化或内容重新挂接时更新图关系。它不自行
监听父 snapshot，不直接调用 compiler，也不自行实现事件队列。

全局主题变化时，Runtime 使用新根 snapshot 和各节点最后一次有效配置，按拓扑顺序准备整个作用域图。
局部配置变化时，只准备该节点及其后代。

配置输入无效时：

- 当前作用域保留已发布 snapshot。
- `LastValidConfig` 不更新。
- 派发失败事件和 diagnostics。
- 后续父主题变化使用 `LastValidConfig`，避免该作用域永久冻结在旧全局主题。

Popup、Flyout、Dialog 和其他脱离普通逻辑树的宿主必须显式传播 owner 的 `ThemeContext`。不得回退到
Application 全局资源来掩盖上下文丢失。

## 12. 事务与发布顺序

一次主题事务分为 Capture、Prepare、Commit、Notify 四个阶段：

```text
+----------------+    +----------------+    +----------------+    +----------------+
| Capture        | -> | Prepare        | -> | Commit         | -> | Notify         |
| UI thread      |    | pure work      |    | UI thread      |    | UI thread      |
+----------------+    +----------------+    +----------------+    +----------------+
| immutable      |    | merge          |    | swap every     |    | resources      |
| config         |    | cache          |    | context        |    | contexts       |
| affected nodes |    | compile        |    | update state   |    | outcome event  |
+----------------+    +----------------+    +----------------+    +----------------+
```

提交前不得修改 Application、ThemeManager、ThemeContext 或资源 Provider。所有 context 引用完成替换后，
才允许发送第一条资源通知，因此任意资源回调看到的都是完整新状态。

配置规范化或编译失败时不进入 Commit。Commit 中尚未发送通知的状态修改发生异常时必须回滚到旧引用。

同一作用域的新请求使用 generation 标识。旧的异步准备结果在提交前发现 generation 已过期时直接丢弃。
提交期间发生的重入请求进入队列，同一作用域尚未执行的请求合并为最新请求。

## 13. 资源解析

根 ThemeContext 的 Provider 在 ThemeManager 初始化时挂载一次。每个局部 ThemeContext 也只拥有一个
Provider。主题切换不追加 `MergedDictionaries`，不创建新的 Provider。

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

控件实例不创建 Token `ResourceDictionary`，不注册 Control 自己的资源作用域，也不订阅全局主题事件。
用户 Content 和嵌套控件只能看到当前 ThemeContext，不能看到父控件私有的 Control Token 输入。

## 14. Imperative Token 消费

少数控件需要在 C# 中计算 palette、断点或运行时绘制值。它们使用 `ThemeTokenResolver`：

- Resolver 从当前控件继承的 `ThemeContext` 读取一次 snapshot。
- 全局 Token 使用 `SharedTokenKind` 强类型查询。
- Control Token 使用 `ControlTokenIdentity` 和生成的 Token kind 查询。
- palette 读取 snapshot 中不可变的 `PresetColorPalettes`。
- 需要动态更新时订阅当前 `ThemeContext`，不订阅 `ThemeManager.ThemeChanged`。

优先把固定视觉结果编译为 Control 资源。只有值确实参与控件运行时算法时才使用 resolver。

## 15. 事件契约

新主题系统只保留结果事件：

- `ThemeChanged`：事务已经提交且资源引用一致后派发。
- `ThemeChangeFailed`：Capture 或 Prepare 失败后派发，携带 request、scope、transition id、diagnostics 和 exception。

不提供可取消的 Changing 事件，也不保留 Created、AboutToLoad、Loaded、AboutToChange、Unload 等伪生命周期。

事件由统一 `ThemeEventDispatcher` 派发：

- 每个订阅者独立调用。
- 一个订阅者异常不阻止后续订阅者。
- 订阅者异常通过 Avalonia Logger 记录完整上下文。
- 订阅者异常不把成功提交改写为主题失败。
- 事件处理期间发起的请求遵守 Runtime 重入队列。

全局事件由 `IThemeManager` 暴露；局部结果由对应 `ThemeConfigProvider` 暴露。两者使用相同 event args 和
派发规则。

## 16. 启动时序

`Application.UseAtomUI()` 的主题启动顺序固定为：

1. Builder 收集主题包、主题文件来源、算法 descriptor、默认主题配置和语言配置。
2. 构建 `ThemeSchemaRegistry`，在此时拒绝重复 Control identity 和无效 descriptor。
3. Catalog 读取并绑定主题定义，确定默认主题。
4. 同步准备第一个根 `ThemeSnapshot`。
5. 创建 Root ThemeContext、稳定 Provider、ThemeEngine 和 ThemeManager。
6. 将已经包含有效 snapshot 的 ThemeManager 挂载到 Application Styles。
7. 设置 Avalonia Light/Dark variant，发布一次资源通知。
8. 绑定主题服务并执行与主题生命周期无关的模块初始化。

不得先挂载空资源层再异步补主题，也不得通过 ThemeLoaded 事件修补 DefaultFont、断点或其他 Token。

## 17. 线程与生命周期

- 配置输入只能在 UI 线程捕获。
- ThemeCompiler 不访问 UI 对象，可以在后台准备运行时变更。
- 初始根主题和 ThemeConfigProvider 首次 attach 同步准备，避免首帧闪烁。
- 运行时更新可以后台编译，但最终 Commit 和资源通知必须在 UI 线程。
- Scope attach、detach、reparent 和 Content 替换必须在 ThemeScopeGraph 中对称登记和释放。
- snapshot 不持有作用域、Control、Visual 或事件订阅。
- Provider 生命周期归所属 ThemeContext，不进入全局静态缓存。
- Popup/Flyout 的 context owner 变化和关闭必须释放传播订阅。

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
- Control compilation cache：以 global token fingerprint、Control identity、Control config fingerprint 和
  algorithm id 为 key，复用跨作用域的 Control 计算结果。

两级缓存共用容量策略，失败和取消结果不缓存。

默认容量由 `ThemeCacheOptions` 固定为：

- 完整 snapshot cache：最多 32 个 entry，最多 131072 个 weight unit。
- Control compilation cache：最多 2048 个 entry，最多 131072 个 weight unit。
- 一个 Token slot、资源条目或差量条目计为一个 weight unit。

容量可以由应用显式调小；扩大默认值必须附带 benchmark、峰值对象计数和 Gallery 导航内存结果。

### 18.3 编译分配

- 编译前根据 schema 中已知的 Token 和 Control 数量预分配数组及稀疏字典容量。
- 临时 builder 使用 copy-on-write global token overlay，Control 计算不得深克隆完整 `DesignToken` 对象图。
- 大型临时数组可以由有界 workspace pool 复用，但 pooled buffer 绝不进入 snapshot。
- 取消、异常和 stale generation 路径必须在 `finally` 中归还 workspace。
- 同一 ThemeEngine 同时只运行一个根事务准备管线；后续请求合并，避免并发编译造成内存峰值。
- 初始同步编译完成后及时释放 ThemeDocument 中不再需要的 XML 对象和临时字符串。

### 18.4 资源查询热路径

`ThemeTokenResourceProvider.TryGetResource` 是控件布局和样式更新的热路径，要求：

- global、Control shared 和 Control own 命中查询均为零托管分配。
- 生成的资源扩展持有缓存后的稳定 key，不在查询时创建 key 或拼接字符串。
- `ControlSharedTokenResourceKey` 直接包含 `ControlTokenIdentity` 或 registry slot，不在每次查询时重新构造身份。
- 查询只执行类型分支、slot 索引和最多一次稀疏差量 fallback。
- miss 不创建 diagnostics、异常或临时集合。
- 一次事务对每个受影响 Provider 最多调用一次 `RaiseResourcesChanged()`。

### 18.5 生命周期与可回收性

ThemeScopeGraph 在注册有效期间强持有 ScopeNode 和对应 ThemeConfigProvider；Provider detach 时必须通过
注册 token 同步移除节点和全部边。Runtime 不保存已 detach 节点历史。

- 一个活动 ThemeConfigProvider 对应一个 ScopeNode、ThemeContext 和 ResourceProvider。
- ScopeNode 除已 attach 的 ThemeConfigProvider 外不能单独持有 Content、后代 Control 或其他 Visual。
- ThemeConfigProvider 持有 registration disposable；detach、reparent 和异常 attach 路径都必须释放它。
- imperative Token 订阅返回 `IDisposable`，由控件在 detach、re-template 或 owner 变化时释放。
- 非 Visual `AvaloniaObject` 使用 DynamicResource 时必须有 scoped `IResourceHost` 和对称 release 路径。
- Popup、Flyout 和 overlay 关闭后必须释放 owner ThemeContext 传播订阅。
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
- 资源查询：global、Control shared、Control own 的 hit、fallback 和 miss。
- 生命周期：100 次 Provider attach/detach、Popup/Flyout open/close 和 Gallery 页面导航。

验收指标至少包括：

- `TryGetResource` 稳态查询为 0 B/op。
- no-op 请求不创建 snapshot、Provider、Context，不派发资源或主题事件。
- 单 Control 局部覆盖只产生一个新 Control snapshot，其他 Control snapshot 引用保持不变。
- 重复更新后每个作用域仍只有一个 Provider。
- cache entry 和 token-slot weight 始终不超过配置上限。
- attach/detach 后通过 WeakReference 和强制 GC 验证 ScopeNode、ThemeContext、Provider 及旧 Content 可回收。
- Gallery 随机导航稳定后旧 ShowCase、DynamicResourceExpression 和主题作用域对象数量不单调增长。
- RSS 只作为辅助信息；对象数量和可达链才是内存泄漏判据。

## 19. AOT 约束

- 内置 Token 注册和 schema 全部由源生成器生成。
- 不运行时扫描程序集或 Token 属性。
- 不使用 `Activator.CreateInstance` 创建内置 Control Token。
- 不使用字符串属性路径 Binding 完成主题配置或 Token 查询。
- 自定义算法和第三方 Control Token 必须提供稳定 descriptor id。
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
+-- ThemeManager.cs                   Avalonia Styles facade
+-- ThemeManagerBuilder.cs            启动组装器
+-- ThemeManagerBuilderExtensions.cs  默认注册扩展
+-- ThemeConfigProvider.cs            局部主题配置宿主
+-- IThemeConfigProvider.cs           Provider 公开契约
+-- IThemeAssetPathProvider.cs        主题资源路径契约
+-- ThemeEngine.cs                    唯一 snapshot 事务提交者
+-- ThemeTransaction.cs               Capture/Prepare/Commit/Notify 事务
+-- ThemeState.cs                     当前已提交主题状态
+-- ThemeContext.cs                   稳定主题上下文
+-- ThemeScopeGraph.cs                局部主题作用域图
+-- ThemeScopeNode.cs                 作用域节点
+-- ThemeRequest.cs                   主题变更请求
+-- ThemeEventDispatcher.cs           结果事件派发
+-- ThemeExceptions.cs                公开错误类型
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
|   \-- 仅用于捕获配置输入的 setter 类型
|
+-- Definitions/
|   +-- 主题源、ThemeInfo、definition revision 和默认主题选择
|   +-- XML reader、syntax document 和 diagnostics
|   \-- Binder、typed ThemeDefinition 和 bind result
|
+-- Schema/
|   +-- ThemeSchemaRegistry
|   +-- TokenDescriptor、ControlTokenDescriptor
|   +-- ThemeAlgorithmDescriptor
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
|   +-- global/control resource keys and markup extensions
|   +-- Control theme provider contracts and attributes
|   +-- BaseControlTheme and provider aggregation
|   \-- snapshot lookup、style/template 和 Control theme 加载辅助
|
+-- Tokens/
|   +-- global/control compile-only builders
|   +-- Token definitions and kind metadata
|   \-- Token value converters
```

`Definitions` 同时拥有主题来源和主题定义，不再用单独的 `Catalog` 目录把一次加载流程拆开。
`ThemeEngine`、事务、上下文和作用域图都属于主题子系统的核心运行时入口，直接位于 `Theme` 根目录；
这些类型必须共享同一个 snapshot 提交边界，但不需要额外的物理目录表达这一事实。
`Theme` 根目录中的 C# 类型统一使用 `AtomUI.Theme` 命名空间；职责子目录中的类型使用与目录对应的
`AtomUI.Theme.<Area>` 命名空间。根目录不保留 `.Scope`、`.Transitions` 等已被物理结构删除的命名空间。
`Algorithms` 同时拥有算法与调色板计算；调色板不是独立运行时子系统。
AXAML Control theme 的发现、聚合和加载属于主题资源所有权，统一位于 `Resources`，不参与 Token 编译。

依赖方向固定为：Definitions/Configuration/Schema 提供输入，Compilation 产生 snapshot，根目录的
ThemeEngine 提交 snapshot，Resources 只读取已提交 snapshot。Compilation 不依赖 Avalonia 资源宿主，Resources
不执行编译，ThemeConfigProvider 不拥有编译器。每个程序集生成自己的
`AtomUI.Generated.<OwnerAssemblyIdentifier>.GeneratedThemeSchema`，程序集标识中的非标识符字符统一
替换为下划线，禁止多个程序集生成相同全限定类型名，也禁止把生成 namespace 嵌套在可能同名的 Control
类型下面。

## 21. 明确删除的旧设计

新架构实现时删除：

- `Theme`、`ITheme` 以及预创建 Theme variant pool。
- `Theme.SharedToken`、`Theme.GetControlToken()`、`ThemeConfigProvider.SharedToken`、
  `ThemeConfigProvider.ControlTokens` 和所有 clone compatibility map。
- snapshot 中的 `DesignToken`、`IControlDesignToken` 和对应 Clone API。
- `IDesignToken`、`IControlDesignToken` 中面向运行时修改和资源构建的接口。
- `ThemeScope.SnapshotProperty` 和基于 snapshot attached value 的父级订阅。
- `TokenFinderUtils` 和沿 StylingParent 查找旧 Provider 的逻辑。
- `ThemeCreated`、`ThemeAboutToLoad`、`ThemeLoaded`、`ThemeLoadFailed`、`ThemeAboutToChange`。
- 对 Theme ResourceDictionary 的 Motion/Wave 直接修改。
- 自定义主题 id 编码的 Avalonia ThemeVariant。
- `ControlTokenRegistration(Type)`、反射 schema、Activator fallback 和对应 AOT suppress。
- cache pin、父 snapshot Version cache key 和字符串拼接 cache key。
- 旧主题 XML 的 `ControlTokens/ControlToken` 结构。

这些内容不建立 adapter，不标记 obsolete 后继续保留，也不作为迁移阶段的内部真源。

## 22. 验收标准

### 22.1 语义

- 全局 Tokens、Controls、Algorithms 和 Inherit 遵守本文定义的配置合并规则。
- 顶层 ThemeConfigProvider 默认继承当前全局主题。
- 嵌套 Provider 正确合并父 Tokens、Control 配置和算法四态。
- `Inherit=false` 从 AtomUI 默认配置开始。
- Control 覆盖不泄漏到 Content 或嵌套控件。
- 同一主题的新 runtime override 会产生新有效配置，不复用过期 Theme 对象。

### 22.2 事务

- Dark + Compact 只产生一次完整提交。
- 根主题变化和所有受影响局部作用域在通知前完成引用替换。
- 配置失败、编译失败和 stale result 不发布部分状态。
- 重入和高频更新不会递归提交，latest request 语义确定。
- 订阅者异常不阻断提交或其他订阅者。

### 22.3 资源与生命周期

- 根作用域和每个局部作用域始终只有一个 Token ResourceProvider。
- 重复更新不增长 `MergedDictionaries`。
- 控件实例不创建 Control Token 资源字典。
- Popup、Flyout、Dialog 和窗口覆盖层继承 owner ThemeContext。
- detach、reparent、关闭和回收后不保留旧作用域或 Control。

### 22.4 性能、内存与 AOT

- 同一主题定义 revision 只读取和绑定一次。
- 等价有效配置只编译一次。
- 单 Control 配置变化复用全局表和其他 Control snapshot。
- Token 资源查询热路径保持零分配。
- cache 和旧 snapshot 存活数量满足本文定义的容量上界。
- ThemeConfigProvider、Popup、Flyout 和 Gallery 导航生命周期 WeakReference 测试通过。
- 正常主题切换路径不执行 Token 反射或 Activator。
- Core、Controls、Desktop、DataGrid、ColorPicker、Extras、GalleryBase 和 Gallery 相关测试通过。
- Release 多目标构建和 Gallery NativeAOT publish 通过。
- `git diff --check` 通过，仓库搜索不存在已删除旧 API 的引用。
