# AtomUI 主题定义 XML v1 规范

本文定义 AtomUI 主题定义文件的 v1 标准格式。它是 `ThemeDocumentReader`、
`ThemeDefinitionBinder`、主题编辑器、CI 校验器和第三方主题包共同遵守的协议。

规范配套文件：

- XML namespace：`https://atomui.net/schemas/theme/v1`
- XML Schema：[schemas/atomui-theme-v1.xsd](schemas/atomui-theme-v1.xsd)
- 完整示例：[examples/daybreak-blue.theme.xml](examples/daybreak-blue.theme.xml)

本文中的“必须”“不得”“应当”和“可以”是规范性要求。

## 1. 设计结论

- 一个 XML 文件只定义一个主题。
- XML namespace 是格式版本，不使用额外的 `Version` 属性。
- v1 文档严格使用 `Theme -> Algorithms -> Tokens -> Controls` 的固定顺序。
- 算法使用有序 `<Algorithm Id="..." />` 元素，不使用逗号分隔字符串。
- Control 使用 `(Catalog, Id)` 组成稳定身份，不根据 CLR 类型名或程序集扫描推断身份。
- Token 只使用 `Value` 属性，不允许正文值、类型属性或 `IsShared` 分类标记。
- Token 类型、阶段和赋值器由 `ThemeSchemaRegistry` 决定，XML 不重复声明 schema 信息。
- XSD 负责结构、基础词法、容量和局部唯一性；Binder 负责 registry 相关的语义验证。
- 文档不支持 include、import、继承文件、表达式、脚本或环境变量替换。
- 主题定义文件只产生不可变 `ThemeDefinition`，不能直接修改在线主题资源。

主题定义文件应使用 `<theme-id>.theme.xml` 扩展名约定。文件名只用于来源定位和诊断，不参与主题身份。

## 2. 完整示例

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
    <Token Name="BorderRadius" Value="6" />
  </Tokens>

  <Controls>
    <Control Catalog="AtomUI" Id="Button" Algorithm="Global">
      <Tokens>
        <Token Name="ColorPrimary" Value="#0958D9" />
        <Token Name="ContentFontSize" Value="14" />
      </Tokens>
    </Control>

    <Control Catalog="AtomUI" Id="Input" Algorithm="Disabled">
      <Tokens>
        <Token Name="ActiveBorderColor" Value="#1677FF" />
      </Tokens>
    </Control>

    <Control Catalog="AtomUI" Id="DataGrid">
      <Algorithms>
        <Algorithm Id="Default" />
        <Algorithm Id="Compact" />
      </Algorithms>
      <Tokens>
        <Token Name="HeaderBg" Value="#FAFAFA" />
      </Tokens>
    </Control>
  </Controls>
</Theme>
```

## 3. 文档结构

```text
Theme                                  exactly 1
+-- Algorithms                        exactly 1
|   \-- Algorithm                     1..64
+-- Tokens                            0..1
|   \-- Token                         1..4096
\-- Controls                          0..1
    \-- Control                       1..1024
        +-- Algorithms                0..1
        |   \-- Algorithm             1..64
        \-- Tokens                    0..1
            \-- Token                 1..4096
```

元素顺序是协议的一部分。容器一旦出现就不能为空。空 `Tokens`、空 `Controls` 或空 `Algorithms`
均为结构错误。

## 4. Theme

根元素必须是 v1 namespace 中的 `Theme`。

| 属性 | 必须 | 类型 | 语义 |
|---|---|---|---|
| `Id` | 是 | `Identifier` | ThemeCatalog 内稳定且唯一的主题身份，不从文件名推断 |
| `Name` | 是 | 1..128 字符 | 面向用户显示的名称 |
| `Appearance` | 是 | `Light` 或 `Dark` | 该 definition 算法链求值后的最终 appearance 断言 |
| `IsDefault` | 否 | `true` 或 `false` | 是否参与默认主题选择，默认 `false` |

Theme `Id`、Control `Catalog`、Control `Id` 和 Token `Name` 使用相同 Identifier 词法：

```text
[A-Za-z_][A-Za-z0-9_.-]{0,127}
```

Identifier 区分大小写，使用 ordinal 比较。`DaybreakBlue` 和 `daybreakblue` 是两个不同身份。

一个可用 ThemeCatalog 最多只能有一个 `IsDefault="true"` 的主题。该约束跨文件生效，由 `ThemeCatalog`
验证，不属于单文件 XSD 约束。

`Appearance` 不是一个可以压过算法结果的独立开关。Binder 从 AtomUI library 的 Light baseline 开始，按定义中
的算法顺序折叠 descriptor 的 `Preserve / Light / Dark` appearance effect，并要求计算结果与声明值完全一致：

```text
computed = Fold(Light, definition.Algorithms[*].AppearanceEffect)
require computed == Theme.Appearance
```

AtomUI 的 `Default`、`Dark`、`Compact` descriptor effect 分别为 `Light`、`Dark`、`Preserve`。Binder 不按算法
名称猜测 effect，而是读取枚举键对应 descriptor 的声明。descriptor 未注册、effect 不合法或最终结果与
`Appearance` 不一致时，整个 definition 绑定失败。

绑定后，`Appearance` 作为已验证的 definition 最终 appearance 存入不可变 `ThemeDefinition`。运行时
`ThemeConfig` 可以整体替换有效全局算法链，但不会修改 definition 元数据；编译器始终从 Light baseline 折叠
当前有效算法链，最终 `ThemeSnapshot.Appearance` 才是发布 Avalonia Light/Dark variant 的唯一依据。父 Dark +
子显式 Compact 的结果因此是 Light Compact；Dark Compact 必须显式声明完整算法链。具体根作用域、局部
`Inherit` 和 FollowSystem 规则见 [主题系统架构](../../architecture/systems/theming/runtime.md)。

## 5. Algorithms 与 Algorithm

顶层 `Algorithms` 必须存在，并至少包含一个算法。算法按照文档顺序执行：

```text
M0 = Algorithm[0].Evaluate(effectiveSeed, null)
M1 = Algorithm[1].Evaluate(effectiveSeed, M0)
...
Mn = Algorithm[n].Evaluate(effectiveSeed, M[n-1])
result = ApplyMapAndAliasOverrides(Mn)
```

算法求值签名固定为 `Evaluate(effectiveSeed, previousMap?) -> nextMap`。整条链中的 `effectiveSeed` 是同一份不可变
语义输入；前一个算法的输出只作为下一个算法的 `previousMap`，不能替代 Seed。算法不得修改 Seed 或 previous
Map。第一个算法收到 `previousMap=null`；内置算法需要默认 Map 时可以在自身实现中调用 Default fallback，但
不得改变通用链契约。

`Algorithm` 是空元素，只允许一个必填 `Id` 属性。v1 XSD 的允许值严格为 `Default`、`Dark` 和 `Compact`，并区分
大小写；`dark`、`Unknown`、`1` 和空值都会在 Reader 边界失败并产生带源码位置的 diagnostic。同一算法链内不得
重复枚举值。

Reader 使用精确映射把 XML 文本转换成 `ThemeAlgorithm`；读取成功的 `ThemeDocument` 不保存原始字符串。Binder
再用枚举键查询 `ThemeSchemaRegistry` 中唯一的 `ThemeAlgorithmDescriptor`。因此“XML 值非法”和“合法枚举但
当前 schema 未注册实现”属于两个明确的错误边界。

主题文件只声明算法身份和顺序。算法实例、依赖关系、AOT 构造委托以及外观影响由 descriptor 提供；每个
descriptor 必须提供显式 revision/version，算法行为变化时必须提升 revision。当前算法集合是封闭的，不支持任意
第三方字符串 ID。新增算法必须追加 `ThemeAlgorithm` 显式数值，并同步实现及 Attribute、v1 XSD 枚举和契约测试。

## 6. Tokens 与 Token

顶层 `Tokens` 覆盖当前主题的全局 Token。每个 `Token` 都是空元素：

```xml
<Token Name="ColorPrimary" Value="#1677FF" />
```

| 属性 | 必须 | 语义 |
|---|---|---|
| `Name` | 是 | Token schema 中的稳定名称 |
| `Value` | 是 | 1..4096 字符的词法值 |

同一个 `Tokens` 容器内不得出现重复 `Name`。Token 顺序不影响语义，规范化器按照 Token slot 生成稳定
fingerprint。

Binder 根据生成式 schema 决定 Token 的阶段：

- Seed Token override 在算法链之前应用。
- Map 和 Alias Token override 在算法链之后应用。
- 未知 Token、不可写 Token 或类型转换失败都是错误。
- XML 不允许通过 `Type`、`Stage` 或 `IsShared` 改写 schema 结论。

## 7. Controls 与 Control

`Controls` 包含 Control 级配置。Control 身份由必填的 `Catalog` 和 `Id` 共同组成：

```xml
<Control Catalog="AtomUI" Id="Button" Algorithm="Disabled">
  <Tokens>
    <Token Name="ContentFontSize" Value="14" />
  </Tokens>
</Control>
```

`Catalog` 标识注册 descriptor 的命名域，`Id` 标识该命名域中的 Control。两者都区分大小写。
同一个 `Controls` 容器中不得出现重复的 `(Catalog, Id)`。

AtomUI 内置 Control 使用 `Catalog="AtomUI"`。第三方包必须使用自身稳定的 Catalog，例如
`Catalog="Acme.Controls"`，不得借用 `AtomUI` Catalog。

Control 内部的 `Tokens` 使用一个集合表达两类覆盖：

- 当前 Control 的 Own Token。
- 当前 registry revision 中的任意 Global Token。

Binder 先使用 Control descriptor 匹配 Own Token，未命中时使用完整 Global Token schema 分类。Own Token 与
Global Token 禁止同名；名称在两处都不存在时，整个 definition 绑定失败。合法但未被该 Control 消费的 Global
Token 允许配置并可能没有实际效果。Control Token 不形成 Content 子树资源作用域，局部 Global 覆盖也不会改变
其他 Control 或真正的 Global Token snapshot。具体规则见
[Control Token 设计规范](../../engineering/control-token-guidelines.md)。

Control 配置必须至少声明 `Algorithm`、自定义 `Algorithms` 或 `Tokens` 中的一项。空 Control 是语义错误。

### 7.1 Control 算法四态

| XML 形态 | 规范化状态 | 语义 |
|---|---|---|
| 不声明 `Algorithm` 和 `Algorithms` | Unspecified | 继承父配置中同一 Control 的策略 |
| `Algorithm="Disabled"` | Disabled | Control Seed override 不重新派生 Map 和 Alias |
| `Algorithm="Global"` | Global | 使用当前作用域的全局算法链重新派生 |
| 子元素 `<Algorithms>` | Custom | 使用 Control 声明的有序算法链 |

`Algorithm` 属性和 `<Algorithms>` 子元素互斥，同时出现是语义错误。继承链没有提供 Control 策略时，
Unspecified 最终解析为 AtomUI 默认策略 `Disabled`。

### 7.2 为什么 Algorithm 不是布尔类型

Control 算法配置不是“开启或关闭算法”的二元开关，而是一个四态策略：

```text
ControlAlgorithmMode
+-- Unspecified
+-- Disabled
+-- Global
\-- Custom -> ordered ThemeAlgorithm values
```

`Global` 的含义也不是简单的“启用”，而是“使用当前作用域的全局算法链重新派生该 Control 的有效
Token”。如果写成 `Algorithm="true"`，XML 本身无法表达启用的是哪一条算法链。

Control `Global` 和 `Custom` 使用与全局算法完全相同的
`Evaluate(sameEffectiveSeed, previousMap?) -> nextMap` 契约。Binder 同样解析每个算法 descriptor 的 appearance
effect；计算得到的 Control appearance 只传给该 Control 的 Token evaluator，不修改所在 ThemeContext 的
`ThemeSnapshot.Appearance` 或 Avalonia ThemeVariant。

从编码能力看，可以使用“可空布尔属性 + 子算法列表”拼出四种状态：

| 可空布尔值 | 子算法列表 | 结果 |
|---|---|---|
| 未提供 | 空 | Unspecified |
| `false` | 空 | Disabled |
| `true` | 空 | Global |
| 未提供 | 非空 | Custom |

但这种表示把一个领域值拆成两个耦合字段，并产生 `true + 自定义列表`、`false + 自定义列表` 等无效组合。
它还容易在序列化、绑定或默认值处理中把“未提供”错误地折叠为 `false`，从而把“继承父策略”悄悄改成
“禁用”。

因此 XML 使用有意义的枚举词 `Disabled` 和 `Global`，自定义策略使用结构化 `<Algorithms>`，省略两者表示
Unspecified。后端统一使用以下枚举接收规范化结果：

```csharp
internal enum ControlAlgorithmMode : byte
{
    Unspecified,
    Disabled,
    Global,
    Custom
}
```

`Custom` 对应的有序 `ThemeAlgorithm` 枚举值作为规范化 Control 配置中的不可变 payload 保存，并且只允许在 Mode 为
`Custom` 时非空。Reader 和 Binder 不得先降级为 `bool` 或 `bool?` 再推断。这样 Schema、diagnostic、C#
配置模型和运行时合并器共享同一套领域语义。

自定义算法链示例：

```xml
<Control Catalog="AtomUI" Id="DataGrid">
  <Algorithms>
    <Algorithm Id="Default" />
    <Algorithm Id="Compact" />
  </Algorithms>
  <Tokens>
    <Token Name="HeaderBg" Value="#FAFAFA" />
  </Tokens>
</Control>
```

## 8. Token Value 词法

`Value` 先经过 XML 1.0 属性解码，再交给 Token descriptor 的强类型 parser。解析必须使用
`CultureInfo.InvariantCulture`，不得依赖当前系统语言。

内置基础类型使用以下规范词法：

| 目标类型 | 规范词法 | 示例 |
|---|---|---|
| `string` | XML 解码后的原值，不自动 trim | `Alibaba Sans` |
| `bool` | 小写 `true` 或 `false` | `true` |
| `int` | 十进制整数，不使用分组符 | `14` |
| `double` / `float` | 有限十进制或科学计数法，`.` 为小数点 | `1.5` |
| `Color` | `#RRGGBB` 或 `#AARRGGBB` | `#1677FF` |
| `TimeSpan` | invariant constant format | `00:00:00.1000000` |
| `Thickness` | 1、2 或 4 个逗号分隔的有限数字 | `8,4,8,4` |
| `CornerRadius` | 1、2 或 4 个逗号分隔的有限数字 | `6` |
| `Point` / `Size` | 两个逗号分隔的有限数字 | `12,8` |
| enum | descriptor 生成的区分大小写名称 | `Round` |

Brush、FontFamily、BoxShadows、Easing 和其他复合值必须由对应 descriptor 明确声明 parser 和规范 formatter。
第三方 Token parser/formatter 必须由 AtomUI generator 写入 descriptor，不得手工注册，也不得退回
`TypeConverter` 反射发现或 `Convert.ChangeType`。

规范 writer 必须输出 formatter 的 canonical value。Reader 可以接受 descriptor 明确声明的等价词法，
但 fingerprint 必须基于转换后的 typed value，而不是原始字符串。

## 9. 校验管线

```text
UTF-8 bytes
    |
    v
secure XmlReader + compiled XSD
    |
    v
ThemeDocument
    |
    v
ThemeDefinitionBinder + ThemeSchemaRegistry
    |
    v
typed immutable ThemeDefinition + diagnostics
```

校验分为五层，任一层失败都不得产生可发布的部分定义：

1. XML well-formedness：编码、标签、属性引用和 namespace 正确。
2. XSD validation：元素顺序、数量、属性、基础词法和单文件唯一性正确。
3. Registry binding：已解析的算法枚举、Control 和 Token identity 均存在。
4. Value conversion：每个 Token 值转换为 schema 指定的强类型值。
5. Semantic validation：算法互斥、外观一致性、空 Control、跨文件默认主题冲突等规则成立。

XSD validation 不能替代 Binder。XSD 固定算法枚举的可读文本，但不保证当前进程已注册相应 descriptor，也不包含
当前进程安装的 Control、Token 集合或 Token 值目标类型。

每条 diagnostic 必须包含：

- 稳定 code 和 severity。
- 文件或资源身份。
- 1-based line 和 column。
- 元素路径，例如 `/Theme/Controls/Control[@Catalog='AtomUI'][@Id='Button']/Tokens/Token[@Name='ColorPrimary']`。
- 不依赖异常文本的稳定 message。
- 底层异常仅作为内部 exception context 保存。

diagnostic code 按责任分段：

| 范围 | 责任 |
|---|---|
| `ATMTHM1xxx` | XML、namespace 和 XSD 结构 |
| `ATMTHM2xxx` | Registry identity 和 schema binding |
| `ATMTHM3xxx` | Token value 转换和配置语义 |
| `ATMTHM4xxx` | Catalog、来源优先级和跨文件冲突 |

## 10. 安全与资源边界

Reader 必须满足：

- `DtdProcessing = Prohibit`。
- `XmlResolver = null`。
- `ValidationType = Schema`，并启用 `XmlSchemaValidationFlags.ProcessIdentityConstraints`。
- 不启用 `ProcessInlineSchema` 或 `ProcessSchemaLocation`，只使用应用内置并预编译的 v1 XSD。
- 不读取 namespace URI 或 `xsi:schemaLocation` 指向的网络资源。
- 不展开外部实体，不执行 XInclude，不解释处理指令。
- XSD 在进程初始化时编译一次，后续读取复用同一不可变 schema set。
- 使用 validating `XmlReader` 单次流式生成 `ThemeDocument`，不为正常路径构建 `XDocument`。
- Reader 以 definition revision 为缓存边界，同一结构化来源最多成功读取一次；Binder 以
  `(definition revision, schema registry revision)` 为缓存边界，同一结构化输入最多成功绑定一次。revision
  只用于筛选缓存候选，不能跳过最终的来源或 typed 结构比较。

v1 portable profile 的默认上限：

| 项目 | 上限 |
|---|---|
| 单文件 UTF-8 字节数 | 4 MiB |
| XML 元素总数 | 65536 |
| 单算法链 | 64 |
| 顶层 Token | 4096 |
| Control 数量 | 1024 |
| 单 Control Token | 4096 |
| Identifier 长度 | 128 字符 |
| Token Value 长度 | 4096 字符 |

实现可以通过 `ThemeDocumentReaderOptions` 调低运行时上限，但不得高于实现可安全处理的硬上限。触发限制
必须返回结构化 diagnostic，不能静默截断。

## 11. 规范化与内容身份

Reader 保留源码位置和声明顺序；Binder 输出 typed、不可变定义。内容 fingerprint 使用规范化结果：

- namespace 版本。
- Theme metadata。
- 有序算法 `(ThemeAlgorithm, descriptor revision)`。
- 按 Token slot 排序的 typed Token value。
- 按 `(Catalog, Id)` 排序的 Control 配置。
- Schema registry revision。

空白、属性顺序、注释、Token 声明顺序和 Control 声明顺序不影响 fingerprint。算法顺序影响 fingerprint。

Catalog 另外维护 `ThemeDefinitionRevision`，用于限定某次来源读取和绑定结果。revision 是结构化来源身份，
至少包含稳定 source identity、source revision 和实际内容摘要；文件来源不能只依赖修改时间，程序化来源必须
显式提供 revision。Reader 缓存以 definition revision 为边界，绑定缓存以
`(definition revision, schema registry revision)` 为边界。

definition revision、内容 fingerprint 和来源内容摘要都只能用于缓存候选定位，不能单独充当无碰撞的唯一
身份。Reader 缓存命中后必须比较不可变来源 key，Binder 和编译缓存命中后必须比较 typed、不可变的结构化
定义 key；相同 fingerprint 但 metadata、算法链、Token、Control 配置或 registry revision 不同的定义不得
共享绑定或编译结果。

规范 writer 应使用 UTF-8、两个空格缩进、双引号属性和本文定义的元素顺序。Writer 应按 Token name 以及
Control `(Catalog, Id)` 进行 ordinal 排序，使代码评审 diff 稳定。

## 12. 版本演进

v1 namespace 和 XSD 发布后保持不可变。只允许不改变验证结果的文档澄清。

破坏性格式变更必须使用新 namespace，例如 `https://atomui.net/schemas/theme/v2`，并提供独立 XSD、Reader
和迁移工具。Reader 只接受显式注册的 namespace，不猜测版本，不把无 namespace 文档当作 v1，也不建立旧格式
adapter。

## 13. 一致性验收

主题定义实现必须具备以下测试：

- XSD 自身可以被 .NET `XmlSchemaSet` 编译。
- 标准示例通过 XSD 和 Binder。
- 未声明 namespace、错误元素顺序、未知属性、空容器和重复 identity 被拒绝。
- Control 算法四态全部产生确定的规范化结果。
- 全局和 Control 算法都以相同 effective Seed、previous Map 链式求值，算法不能修改输入。
- Binder 从 library Light baseline 折叠 definition 算法 effect；声明 `Appearance` 与计算结果不一致时拒绝绑定。
- 运行时替换全局算法链后只改变新 snapshot appearance；Control custom appearance 不改变作用域 variant。
- `Algorithm` 属性与 `<Algorithms>` 同时出现时被拒绝。
- 非法算法文本、未注册算法枚举、未知 Control/Token 和非法 Value 产生带行列及路径的 diagnostic。
- DTD、外部实体、超限文件和超限元素数量在发布定义前失败。
- 相同 typed 内容的不同 XML 排版得到相同 fingerprint。
- 相同 fingerprint、不同 typed 结构的碰撞样例不会被判定为同一主题定义。
- 文件内容或 schema/algorithm descriptor revision 变化时旧绑定和编译缓存失效。
- 同一算法枚举值提升 descriptor revision 后 definition fingerprint、Binder 缓存和 Compiler 缓存全部失效；
  只改变 XML revision 但 typed 内容不变时，仍通过结构比较安全复用可复用的编译结果。
- Schema 校验、Reader 和 Binder 的正常路径通过 NativeAOT 验证，不执行反射扫描或动态代码生成。
