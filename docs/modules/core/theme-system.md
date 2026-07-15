# AtomUI.Core 主题系统

本文记录当前主题系统的运行时架构。历史问题、设计取舍和迁移计划见
[主题系统重构设计](theme-system-redesign.md)。

## 启动链路

应用在 `Application.Initialize()` 中调用 `UseAtomUI(builder => ...)`。Builder 收集默认主题、
默认语言、主题文件路径 Provider、控件主题 Provider、ControlToken 注册和可选算法工厂。

`ThemeManager.Configure()` 会先扫描主题，再注册控件 AXAML 主题和语言资源。主题扫描只解析定义：

```text
theme sources
  -> ThemeDefinitionParser
  -> ThemeCatalog
  -> ThemeDescriptor[]
  -> Theme objects for Default / Dark / Compact variants
```

主题 XML 由 `ThemeDefinitionParser` 通过安全 `XmlReader` 和 `XDocument` 解析，输出不可变
`ThemeDefinition` 与结构化 diagnostics。`ThemeCatalog` 保存解析结果、来源优先级、默认主题选择
和不可用主题诊断；同一个定义文件在进程中只解析一次。

## 编译与缓存

全局主题和 `ThemeConfigProvider` 都走同一个 `ThemeCompiler`。编译输入是
`ThemeCompileRequest`，包含主题定义、父 `ThemeSnapshot`、有序算法、共享 Token override、
组件 Token override、ControlToken registrations 和运行期 override。

`ThemeSnapshotCache` 位于 compiler 前面。缓存键包含：

- 主题 id 和主题定义内容。
- 父 `ThemeSnapshot.Version`。
- 有序算法 id。
- 排序后的 shared/runtime/component override。
- ControlToken registration 集合身份。

等价请求返回同一个 `ThemeSnapshot` 引用；失败结果不进入缓存。缓存只限制 inactive entry，默认上限
为 32。全局激活主题和已 attach 的 `ThemeConfigProvider` 会 pin 当前 snapshot，active snapshot 不会被
缓存淘汰。缓存共享 snapshot，不共享 `ThemeTokenResourceProvider`。

## Snapshot 内容

`ThemeSnapshot` 是发布单元，发布后按逻辑不可变：

```text
ThemeSnapshot
├── Id / Version / Algorithms / IsDark
├── SharedTokenCore
├── SharedResources
├── Components[ComponentTokenIdentity]
│   ├── EffectiveSharedTokenCore
│   ├── SharedResourceDelta
│   ├── ControlTokenCore
│   └── ControlResources
├── ComponentConfigs
└── SharedConfig
```

兼容 API 暂时仍返回可变 `DesignToken` / `IControlDesignToken` 副本。内部资源发布、继承和动态资源查找
必须使用 snapshot 内部状态，不读取这些兼容副本。

## 全局主题切换

`ThemeCoordinator` 是唯一修改全局激活主题的路径。请求以 `ThemeRequest` 表达，包括主题 id、有序算法、
运行期 override 和切换原因。

提交顺序：

```text
Request
  -> Normalize algorithms/runtime overrides
  -> no-op check
  -> Preparing: ThemeCatalog + ThemeSnapshotCache + ThemeCompiler
  -> Committing: ThemeManager.CommitActiveTheme
  -> Application.RequestedThemeVariant
  -> ThemeChanged event
```

准备阶段失败时，`Application`、`ActivatedTheme`、Dark/Compact flags 和资源层保持旧值。提交阶段使用
reentrancy guard；事件处理期间的新请求进入队列，当前提交完成后再执行。相同请求是 no-op，不重新发布资源
也不派发 `ThemeChanged`。

## 局部主题作用域

`ThemeConfigProvider` 通过 `ThemeScope.SnapshotProperty` 继承父 snapshot。`Inherit` 默认是 `true`；
为 `false` 时从默认基础 Token 编译局部 snapshot。

Provider 编译成功后一次性提交：

1. 更新自身兼容 `SharedToken` 和 `ControlTokens` 副本。
2. 把 snapshot 写入 Content 根的 `ThemeScope.SnapshotProperty`。
3. 首次发布时创建一个 `ThemeTokenResourceProvider`。
4. 后续发布只替换 provider 内部 snapshot 并触发一次资源变更通知。

`Algorithms`、`SharedTokenSetters`、`ControlTokenInfoSetters` 及其子项变化都会重新编译。编译失败时保留
上一份已发布 snapshot、资源 provider 和 content scope。Provider detach 时释放父 snapshot 订阅、配置订阅、
content scope 和 cache pin。

## 组件 Token 隔离

组件主题遵循 Ant Design 语义：`theme.components.Button` 只影响 Button 自己的样式计算，不向 Button
Content 或任意后代组件传播。

AXAML 中组件自己的 ControlTheme 使用组件共享资源键：

```text
ComponentSharedTokenResourceKey(catalog, componentId, sharedTokenKind)
```

查询语义：

- `SharedTokenKind.X` 查询全局或局部 snapshot 的 shared resources。
- `ComponentSharedTokenResourceKey(Button, X)` 查询 Button 的 effective shared resources，未覆盖时回退到 shared resources。
- `ButtonTokenKind.X` 查询 Button 自己的 control resources。

控件实例不创建组件 Token ResourceDictionary，不订阅全局 ThemeChanged。

## 扩展点

- 新增主题文件：通过主题资源 Provider 或自定义主题目录进入 `ThemeCatalog`。
- 新增 ControlToken：使用 `[ControlDesignToken]` 和源生成的 registration pool，避免运行时程序集扫描。
- 自定义算法：通过 `ThemeVariantCalculatorFactory` 创建有序算法链中的 calculator。
- 局部主题：使用 `ThemeConfigProvider`，不要手动追加 token ResourceDictionary。

## 验证边界

主题系统相关改动至少运行 `AtomUI.Core.Tests`。涉及 Gallery 文档或示例时运行
`AtomUIGallery.Tests` 对应 ShowCase 测试。涉及控件主题资源键或组件隔离时，补跑相关
`AtomUI.Desktop.Controls.Tests` 过滤测试和 DataGrid/ColorPicker 独立包测试。发布前按计划执行 Release build、
NativeAOT Gallery publish 和 legacy 搜索。
