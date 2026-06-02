# Core / Theme 性能优化

> 状态：本轮追加 AtomUI.Core startup / parser / theme config structural-only 收口。

本轮优化集中在主题启动、token 计算、palette 名称、transform 字符串解析和 runtime metadata 查询。它们不是单个控件的模板变更，不改变视觉结构、padding、动画或公开语义；本轮不声明页面级 timing 提升，只记录结构性下降。

## Theme Token Startup Cleanup

`CalculatorUtils.CalculateFontMapTokenValues()` 原先会把 `FontSizeInfo` 拆成两个 LINQ list，再按 index 读取。现在直接读取 `FontSizeInfo` 数组；`CalculateFontSize()` 也从两个 `List<T>` 改成固定 10 项数组，避免 list wrapper 和结果 list 扩容。

`DesignToken.GetTokenProperties()` 原先每次按 kind 查询都会反射扫描属性并返回 LINQ `Where`；`Theme` / `ThemeConfigProvider` 随后再 `Select().ToHashSet()`。现在 token property 和 token name set 在 `DesignToken` 类型级缓存，主题构建只读缓存。

| 指标 | 优化前 | 优化后 | 公式 | 提升 | 结论 |
| --- | ---: | ---: | --- | ---: | --- |
| Font map split LINQ operators / token calculation | 4 operators | 0 operators | `(4 - 0) / 4` | 100.00% | 结构收益；不再 `Select().ToList()` 拆 size / line-height |
| Font-size temporary list wrappers / token calculation | 2 lists | 0 lists | `(2 - 0) / 2` | 100.00% | 结构收益；固定 10 项数组替代 list wrapper |
| Theme token-name hashsets / theme resource build | 3 hashsets | 0 per build | `(3 - 0) / 3` | 100.00% | 结构收益；seed / map / alias token names 类型级缓存 |
| DesignToken property reflection scans / token-name lookup | 3 scans | 0 after type init | `(3 - 0) / 3` | 100.00% | 结构收益；不再每次 build/config 重扫属性 |
| Primary theme algorithm hashsets / theme load | 1 hashset | 0 hashsets | `(1 - 0) / 1` | 100.00% | 结构收益；同 count 后直接逐项 `Contains` |
| Control custom-token key `ToList()` callsites | 2 callsites | 0 callsites | `(2 - 0) / 2` | 100.00% | 结构收益；改为 `List` collection 构造 |
| Default preset color dictionary growth / DesignToken | dynamic | exact 14 capacity | structural | n/a | 结构收益；内置 14 个 preset color 预分配 |

## Theme Asset / ColorMap Cleanup

主题扫描的 4 种 algorithm combination 以前每次 `AddThemesFromFilePaths()` 都创建 1 个 list 和 4 个 hashset；内置主题 asset 路径还会创建 `Select(path => path.ToString())` 迭代器。现在 algorithm combinations 类型级复用，asset path 在 `foreach` 中直接转字符串。

`ColorMap.FromColors(IReadOnlyList<string>)` 以前用 `Select(Color.Parse).ToList()` 物化颜色；现在按 `Count` 创建目标数组并用 indexer 填充。

| 指标 | 优化前 | 优化后 | 公式 | 提升 | 结论 |
| --- | ---: | ---: | --- | ---: | --- |
| Theme algorithm container allocations / file-path scan | 5 containers | 0 after type init | `(5 - 0) / 5` | 100.00% | 结构收益；4 种 algorithm set 复用 |
| Theme asset path LINQ iterators / asset scan | 1 iterator | 0 iterators | `(1 - 0) / 1` | 100.00% | 结构收益；asset URI 直接 foreach 转字符串 |
| ColorMap string parse LINQ operators / palette map | 2 operators | 0 operators | `(2 - 0) / 2` | 100.00% | 结构收益；颜色字符串按 index 填充数组 |
| ColorMap parsed color list wrappers / palette map | 1 list | 0 lists | `(1 - 0) / 1` | 100.00% | 结构收益；目标数组直接传给 `FromColors` |

## Core Parser / Metadata Cleanup

`TransformParser.ParseValue()` 原先在每个数值解析前调用 `part.ToString()`，再交给 `double.Parse(string)`；现在直接调用 span overload。runtime metadata 查询从 `FirstOrDefault(predicate)` 改为直接 `foreach`，找到 key 即返回。

| 指标 | 优化前 | 优化后 | 公式 | 提升 | 结论 |
| --- | ---: | ---: | --- | ---: | --- |
| Transform numeric temp strings / value parse | 1 string | 0 strings | `(1 - 0) / 1` | 100.00% | 结构收益；直接 `double.Parse(ReadOnlySpan<char>)` |
| Transform invalid-value enum name conversions / error | 1 conversion | 0 conversions | `(1 - 0) / 1` | 100.00% | 结构收益；switch 名称替代 enum `ToString()` |
| Runtime metadata LINQ lookup callsites | 1 `FirstOrDefault` | 0 `FirstOrDefault` | `(1 - 0) / 1` | 100.00% | 结构收益；直接 foreach 命中返回 |
| Preset color name enum lookup / `Name()` call | 1 `Enum.GetName` | 0 `Enum.GetName` | `(1 - 0) / 1` | 100.00% | 结构收益；switch 返回静态名称 |

## Theme Resource Cache Cleanup

本轮继续收敛 theme / language 启动路径，不改控件模板、不改 padding、不改布局。核心思路是把稳定的反射结果和 enum resource key 映射从“每次构建资源字典时重新计算”改为“类型级缓存后复用”；XML theme definition 解析也去掉算法列表的 LINQ 链和布尔属性 `Trim().ToLower()` 临时字符串。

这轮是 structural-only：降低主题资源构建、语言资源构建和 culture 解析的固定成本；没有声明 Gallery 页面 timing 百分比提升。

| 指标 | 优化前 | 优化后 | 公式 | 提升 | 结论 |
| --- | ---: | ---: | --- | ---: | --- |
| Token converter discovery LINQ operators / Core type init | 2 operators | 0 operators | `(2 - 0) / 2` | 100.00% | 结构收益；`Where().Select()` 改为单次 foreach |
| Token property reflection lookups / token type after warm cache | 5 callsites re-scan | 0 re-scan | `(5 - 0) / 5` | 100.00% | 结构收益；LoadConfig / BuildResourceDictionary / Clone / GetTokenValue / SetTokenValue 复用缓存 |
| Shared token resource enum scans / resource build | 1 `GetValues` + N `GetName` | 0 after enum cache | structural | 100.00% | 结构收益；SharedTokenKind key/name 映射类型级缓存 |
| Control token resource enum scans / control token build | 1 `GetValues` + N `GetName` | 0 after enum cache | structural | 100.00% | 结构收益；每类 control token kind 复用 key/name 映射 |
| Shared delta enum parse calls / custom control token | N `Enum.TryParse` | 0 after map cache | structural | 100.00% | 结构收益；shared token name -> enum key 预建 map |
| Language field reflection maps / language provider build | 1 field scan + 1 dictionary | 0 after provider cache | `(2 - 0) / 2` | 100.00% | 结构收益；本地化 const field map 类型级缓存 |
| Language resource enum scans / language provider build | 1 `GetValues` + N `GetName` | 0 after enum cache | structural | 100.00% | 结构收益；language resource kind key/name 映射复用 |
| Theme algorithm parser LINQ/materialization / Algorithms element | `Split` + `Select` + `Distinct` + `ToList` | 0 LINQ/split arrays | `(4 - 0) / 4` | 100.00% | 结构收益；手写 CSV 扫描并保持 distinct 语义 |
| Theme boolean attr temp strings / attr parse | 2 strings | 0 strings | `(2 - 0) / 2` | 100.00% | 结构收益；span trim + ordinal ignore-case compare |
| Culture code normalize temp strings / `FromCultureInfo` | 1 `Replace` string | 0 strings | `(1 - 0) / 1` | 100.00% | 结构收益；stack span 写入 `_` 后直接 enum parse |

## Theme Clone / TypeHelper Cache Cleanup

本轮继续收敛 Core 数据层：theme definition clone、control token config clone、algorithm name set、DataGrid 共用的 `TypeHelper` metadata 读取。改动仍是 structural-only，不改视觉、模板、padding 或布局。

| 指标 | 优化前 | 优化后 | 公式 | 提升 | 结论 |
| --- | ---: | ---: | --- | ---: | --- |
| Token value cache dictionary lookups / cache hit | 2 lookups | 1 lookup | `(2 - 1) / 2` | 50.00% | 结构收益；`ContainsKey + indexer` 改 `TryGetValue` |
| Control token clone source indexer lookups / token setter | 1 lookup | 0 lookups | `(1 - 0) / 1` | 100.00% | 结构收益；`Keys + indexer` 改 key-value enumeration |
| Control token clone target dictionary growth risk / clone | 2 dynamic maps | 0 dynamic maps | `(2 - 0) / 2` | 100.00% | 结构收益；tokens/shared tokens 按源 Count 预分配 |
| Theme definition clone target container growth risk / clone | 3 dynamic containers | 0 dynamic containers | `(3 - 0) / 3` | 100.00% | 结构收益；algorithms/control tokens/shared tokens 按源 Count 预分配 |
| Theme algorithm result HashSet growth risk / parse | 1 dynamic set | 0 dynamic set | `(1 - 0) / 1` | 100.00% | 结构收益；按 algorithm name count 预分配 |
| Theme algorithm parser result list growth risk / Algorithms element | 1 dynamic list | 0 dynamic list | `(1 - 0) / 1` | 100.00% | 结构收益；按 CSV segment count 预分配 |
| Default member metadata reflection / repeated type after cache | 1 probe/array path | 0 probes | `(1 - 0) / 1` | 100.00% | 结构收益；TypeHelper 默认索引器名称按 Type 缓存 |
| ReadOnly metadata reflection / repeated member after cache | 1 probe/array path | 0 probes | `(1 - 0) / 1` | 100.00% | 结构收益；TypeHelper 只读判断按 MemberInfo 缓存 |

## Core Color / Text Token Parser Cleanup

本轮继续收敛 Core parser 路径：`ColorUtils.TryParseCssRgbColor()` 的 `rgb/rgba` 解析不再 `Substring + Split + List`；`MostReadable()` 的黑白 fallback 不再每次创建列表和解析 hex；`TextDecorationTokenValueConverter` 不再为颜色段创建 substring、整体 `Replace` 或逐位拼接 thickness 字符串。

这是 structural-only 优化，不改变控件模板、padding、视觉结构或布局语义；本轮不声明页面级 timing 提升。

| 指标 | 优化前 | 优化后 | 公式 | 提升 | 结论 |
| --- | ---: | ---: | --- | ---: | --- |
| CSS `rgb/rgba` parser managed containers / parse | 4 containers | 0 containers | `(4 - 0) / 4` | 100.00% | 结构收益；去掉 substring、split array、parts list、rgba list |
| CSS `rgb/rgba` channel storage / parse | 1 heap list | 1 stack span | `(1 - 0) / 1` | 100.00% | 结构收益；4 个 channel 用 `stackalloc` 存储 |
| `MostReadable` fallback list allocations / fallback path | 1 list | 0 lists | `(1 - 0) / 1` | 100.00% | 结构收益；黑白 fallback 颜色数组类型级复用 |
| `MostReadable` fallback color parses / fallback path after type init | 2 parses | 0 parses | `(2 - 0) / 2` | 100.00% | 结构收益；`#fff/#000` 只在类型初始化解析一次 |
| TextDecoration color substring allocations / token convert | 1 string | 0 strings | `(1 - 0) / 1` | 100.00% | 结构收益；记录颜色 span range 后直接解析 |
| TextDecoration color removal full-string copies / token convert | 1 `Replace` copy | 0 copies | `(1 - 0) / 1` | 100.00% | 结构收益；扫描 thickness 时跳过颜色 range |
| TextDecoration thickness digit temp strings / token convert | 1 growing string | 0 strings | `(1 - 0) / 1` | 100.00% | 结构收益；直接累加整数 |
| TextDecoration without explicit color correctness risk | empty `Replace` can throw | no empty replace path | correctness | n/a | 正确性修复；无颜色时保留默认 `TextDecorationInfo.Color` |

## Core Dimension / Language Parser Cleanup

本轮继续收敛 Core 基础解析：`Dimension.Parse()` 不再为成功路径创建 uppercase string、Regex match/group 对象；`ParseWidths()` 直接读取 tokenizer span，不再为每段 width 先生成 string；`LanguageCode.ToHyphenString()` 对内置语言码直接返回常量字符串。

这是 structural-only 优化，不改变控件模板、padding、视觉结构或布局语义；本轮不声明页面级 timing 提升。

| 指标 | 优化前 | 优化后 | 公式 | 提升 | 结论 |
| --- | ---: | ---: | --- | ---: | --- |
| Dimension uppercase temp strings / successful parse | 1 string | 0 strings | `(1 - 0) / 1` | 100.00% | 结构收益；错误路径才生成 upper-case 诊断文本 |
| Dimension Regex match allocations / successful parse | 1 match path | 0 match paths | `(1 - 0) / 1` | 100.00% | 结构收益；span parser 替代 Regex |
| Dimension numeric group value strings / successful parse | 1 string | 0 strings | `(1 - 0) / 1` | 100.00% | 结构收益；直接 `double.Parse(ReadOnlySpan<char>)` |
| ParseWidths token strings / width token | 1 string | 0 strings | `(1 - 0) / 1` | 100.00% | 结构收益；`TryReadString` 改 `TryReadSpan` |
| ParseWidths result list growth risk / width list parse | dynamic capacity | exact token count capacity | structural | 分配更紧 | 结构收益；先数 token 后预分配结果 list |
| Language code enum name conversions / known language code | 1 `ToString` | 0 conversions | `(1 - 0) / 1` | 100.00% | 结构收益；`zh_CN` / `en_US` 直接返回常量 |
| Language code hyphen replacement strings / known language code | 1 string | 0 strings | `(1 - 0) / 1` | 100.00% | 结构收益；无需 `Replace('_', '-')` |

## Theme Variant / TypeHelper Parser Cleanup

本轮继续收敛 theme variant 与 DataGrid 共用的 property path 解析。`Theme.BuildThemeVariant()` 不再为了最多 3 段字符串创建临时 `List<string>`；主动切换 dark / compact 时直接使用当前 bool 状态构造目标 `ThemeVariant`。`TypeHelper.GetPropertyOrIndexer("[0]")` 对 int indexer 不再先 `Substring`，只有实际需要 string indexer 时才 materialize index string。

这是 structural-only 优化，不改变 theme id 组合顺序、主题切换语义或 DataGrid property path 解析结果；本轮不声明页面级 timing 提升。

| 指标 | 优化前 | 优化后 | 公式 | 提升 | 结论 |
| --- | ---: | ---: | --- | ---: | --- |
| ThemeVariant parts list allocations / variant build | 1 list | 0 lists | `(1 - 0) / 1` | 100.00% | 结构收益；直接按 `id[-Dark][-Compact]` 拼接 |
| ThemeVariant `string.Join` enumerable path / variant build | 1 join over list | 0 joins | `(1 - 0) / 1` | 100.00% | 结构收益；小固定段数不再走 list + join |
| Active theme algorithm list allocations / dark-compact toggle | 1 list | 0 lists | `(1 - 0) / 1` | 100.00% | 结构收益；切换时直接用 bool 构造 requested variant |
| Active theme algorithm add operations / dark-compact toggle | 1-3 adds | 0 adds | `(3 - 0) / 3` | up to 100.00% | 结构收益；不再创建临时算法列表 |
| TypeHelper int indexer temp strings / `[0]` path lookup | 1 substring | 0 strings | `(1 - 0) / 1` | 100.00% | 结构收益；int indexer 直接从 span parse |
| TypeHelper string indexer behavior / `[key]` path lookup | eager substring | lazy materialize | structural | 更少成功 int 路径分配 | string indexer 仍保留原 string index 语义 |

## Theme Config Bucket / Lookup Cleanup

本轮继续收敛 theme startup 与 DataGrid 共用 metadata 路径：主题 token 配置按 seed / map / alias 分桶时不再创建外层 `Dictionary<DesignTokenKind, Dictionary<string,string>>`，空桶复用共享空配置；`TypeHelper.GetNestedProperty()` 对简单属性名直接走单段查询，不再先调用 `SplitPropertyPath()` 创建列表；ThemeManager / ThemeManagerBuilder 的常见注册、查找路径改用 `TryGetValue` / `TryAdd` / `HashSet.Add` 合并重复 lookup。

这是 structural-only 优化，不改变 token 覆盖顺序、主题加载语义、DataGrid property path 解析结果、控件模板、padding 或布局；本轮不声明页面级 timing 提升。

| 指标 | 优化前 | 优化后 | 公式 | 提升 | 结论 |
| --- | ---: | ---: | --- | ---: | --- |
| Shared token config outer dictionaries / theme resource build | 1 dictionary | 0 dictionaries | `(1 - 0) / 1` | 100.00% | 结构收益；seed/map/alias 改为 stack struct 分桶 |
| Control shared-token config outer dictionaries / custom control token | 1 dictionary / token | 0 dictionaries / token | `(1 - 0) / 1` | 100.00% | 结构收益；每个自定义 control token 少一层 map |
| Empty token config bucket dictionaries / no override bucket | 3 empty dictionaries | 0 new dictionaries | `(3 - 0) / 3` | 100.00% | 结构收益；空 seed/map/alias bucket 复用共享空配置 |
| Simple nested property path list allocations / lookup | 1 list | 0 lists | `(1 - 0) / 1` | 100.00% | 结构收益；`Name` 这类单段 path 直接查 PropertyInfo |
| Theme pool lookups / active or unload theme | 2 lookups | 1 lookup | `(2 - 1) / 2` | 50.00% | 结构收益；`ContainsKey + indexer` 改 `TryGetValue` |
| Theme registration dictionary lookups / variant scan | 2 lookups | 1 lookup | `(2 - 1) / 2` | 50.00% | 结构收益；`ContainsKey + Add` 改 `TryAdd` |
| Builder duplicate-registration hash lookups / add token/provider/lang | 2 lookups | 1 lookup | `(2 - 1) / 2` | 50.00% | 结构收益；`Contains + Add` 改 `HashSet.Add` |

## Theme Startup Capacity / Palette Cleanup

本轮继续收敛 Core 启动和 palette 计算路径：源生成的 token/language pool list 现在按生成项数量直接预分配；`ThemeManager.Build()` 在注册 token/provider/language 前给内部 list 精确容量；`Theme` / `ThemeConfigProvider` 的 control token 字典按 token 类型数量预分配；`ThemeConfigProvider.CalculateTokenResources()` 不再为最多 3 个算法创建临时 list，并修复 dark+compact 时 `IsDarkMode` 被 compact 覆盖成 false 的问题；`PaletteGenerator.GeneratePalette()` 的 10 色结果和暗色结果按固定数量预分配，并把默认暗色背景 `#141414` 从“每个暗色 entry 解析一次”改为类型级缓存；light palette 生成不再为默认参数创建 option；dark theme calculator 复用私有暗色 option；`PresetPalettes` 静态初始化也复用暗色 option，并按 14 个 preset color 预分配字典；`PresetPrimaryColor.Color()` 和 `DesignToken` seed color 初始化现在复用缓存色值，避免重复解析固定 hex。

这是 structural-only 优化，不改变 token 注册顺序、language provider 顺序、theme algorithm 顺序、palette 颜色计算公式、控件模板、padding 或布局；本轮不声明页面级 timing 提升。

| 指标 | 优化前 | 优化后 | 公式 | 提升 | 结论 |
| --- | ---: | ---: | --- | ---: | --- |
| Generated token pool list growth steps / `GetTokenTypes()` | at least 1 growth when non-empty | 0 growth | `(1 - 0) / 1` | 100.00% | 结构收益；生成代码使用精确容量 |
| Generated language pool list growth steps / `GetLanguageProviders()` | at least 1 growth when non-empty | 0 growth | `(1 - 0) / 1` | 100.00% | 结构收益；生成代码使用精确容量 |
| ThemeManager registration list growth risk / builder handoff | 4 dynamic lists | 0 dynamic lists | `(4 - 0) / 4` | 100.00% | 结构收益；token/provider/language 注册前按 Count 预分配 |
| Theme algorithm list growth risk / theme instance | dynamic capacity | exact 3 capacity | structural | 分配更紧 | 结构收益；Default/Dark/Compact 最多 3 项 |
| Theme control-token dictionary growth risk / theme load | dynamic capacity | exact token type count | structural | 分配更紧 | 结构收益；收集 control token 前按类型数预分配 |
| ThemeConfigProvider control-token dictionary growth risk / provider attach | dynamic capacity | exact token type count | structural | 分配更紧 | 结构收益；provider 自己的 token 字典同步预分配 |
| ThemeConfigProvider algorithm list allocations / token calculation | 1 list | 0 lists | `(1 - 0) / 1` | 100.00% | 结构收益；Default/Dark/Compact 直接串接 calculator |
| ThemeConfigProvider control-token config map growth risk / provider attach | dynamic capacity | exact setter count capacity | structural | 分配更紧 | 结构收益；control token config map 按 setter count 预分配 |
| ThemeConfigProvider dark+compact `IsDarkMode` / token calculation | false after compact override | true when dark is present | correctness | n/a | 正确性修复；compact 不再覆盖 dark 语义 |
| Palette light pattern list growth risk / palette generation | dynamic capacity | exact 10 capacity | structural | 分配更紧 | 结构收益；固定 10 色结果预分配 |
| Palette default option allocations / default theme token calculation | 20 options | 0 options | `(20 - 0) / 20` | 100.00% | 结构收益；light palette 默认参数不再 new option |
| Palette dark pattern list growth risk / dark palette generation | dynamic capacity | exact 10 capacity | structural | 分配更紧 | 结构收益；dark map 固定 10 项预分配 |
| Dark calculator option allocations / dark token calculation after type init | 20 options | 0 options | `(20 - 0) / 20` | 100.00% | 结构收益；dark calculator 复用私有 option |
| Dark palette default background parses / dark palette generation | 10 parses | 0 after type init | `(10 - 0) / 10` | 100.00% | 结构收益；`#141414` 只在类型初始化解析一次 |
| Static dark color map heap containers / type init | 2 containers | 1 array | `(2 - 1) / 2` | 50.00% | 结构收益；`List<T>` wrapper 改固定数组 |
| Preset palette dictionaries growth risk / static init | 2 dynamic dictionaries | 0 dynamic dictionaries | `(2 - 0) / 2` | 100.00% | 结构收益；light/dark preset map 按 14 色预分配 |
| Preset dark palette option allocations / static init | 14 options | 1 shared option | `(14 - 1) / 14` | 92.86% | 结构收益；暗色 preset palette 复用同一个私有 option |
| Preset dark background parses / static init | 14 parses | 0 parses | `(14 - 0) / 14` | 100.00% | 结构收益；不再为每个 preset 单独解析同一背景色 |
| Preset primary color list containers / type init | array + readonly wrapper | array only | `(2 - 1) / 2` | 50.00% | 结构收益；去掉 `Array.AsReadOnly` wrapper |
| Preset primary `Color()` hex parses / call after type init | 1 parse | 0 parses | `(1 - 0) / 1` | 100.00% | 结构收益；每个 preset primary color 缓存 `Color` struct |
| DesignToken color palette dictionary growth risk / instance | dynamic capacity | exact 14 capacity | structural | 分配更紧 | 结构收益；preset palette 写入前预分配 14 色槽位 |
| DesignToken default preset map allocations / instance | 1 dictionary | 0 dictionaries | `(1 - 0) / 1` | 100.00% | 结构收益；preset color map 类型级复用 |
| DesignToken default preset map adds / instance | 14 adds | 0 adds | `(14 - 0) / 14` | 100.00% | 结构收益；不再每个 token 实例重建相同 map |
| DesignToken seed color parses / instance | 5 parses | 0 parses | `(5 - 0) / 5` | 100.00% | 结构收益；固定 seed 颜色类型级缓存 |
| TypeHelper speculative string-indexer fallback allocations / int-indexer match | 1 string + 1 object array | 0 speculative allocations | `(2 - 0) / 2` | 100.00% | 结构收益；确认需要 string indexer 后才 materialize |
| TypeHelper complex property-path list growth risk / lookup | dynamic capacity | exact segment count capacity | structural | 分配更紧 | 结构收益；复杂 path split 先数段再建 list |

## Verification

```
dotnet build tools/performances/AtomUI.Performance/AtomUI.Performance.csproj -c Release --framework net10.0 --no-restore
dotnet build tools/performances/AtomUI.Performance/AtomUI.Performance.csproj -c Release --framework net8.0
dotnet run --project tools/performances/AtomUI.Performance/AtomUI.Performance.csproj -c Release --framework net10.0 --no-build -- --verify-datagrid-states
dotnet run --project tools/performances/AtomUI.Performance/AtomUI.Performance.csproj -c Release --framework net10.0 --no-build -- --verify-dialog-states --verify-addon-states
dotnet run --project tools/performances/AtomUI.Performance/AtomUI.Performance.csproj -c Release --framework net10.0 --no-build -- --verify-badge-states
dotnet run --project tools/performances/AtomUI.Performance/AtomUI.Performance.csproj -c Release --framework net10.0 --no-build -- --verify-calendar-states --verify-datepicker-states --verify-timepicker-states
dotnet run --project tools/performances/AtomUI.Performance/AtomUI.Performance.csproj -c Release --framework net10.0 --no-build -- --verify-listbox-states --verify-splitter-states
dotnet run --project tools/performances/AtomUI.Performance/AtomUI.Performance.csproj -c Release --framework net10.0 --no-build -- --verify-treeview-states
```
