# Gallery 内置主题切换设计

## 1. 背景与目标

AtomUI Gallery 已经支持深色、紧凑、Motion 和 Wave Spirit 运行时配置，但当前全局主题目录只有
`DaybreakBlue`，并且 `CompiledThemeCatalog.LoadBuiltIn()` 直接硬编码读取这一份 Core 资源。

本次功能要为 Gallery 提供多种内置主题色。每个主题必须遵循 AtomUI Theme Definition XML v1 标准，经过统一
Reader、Binder、Compiler 和 ThemeManager 事务加载；Gallery 不直接把颜色转换成临时 `ThemeConfig`，也不维护
第二套资源字典。

设计目标：

- Gallery 提供五种可切换主题色。
- 新增主题归 Gallery 所有，不把展示型主题加入 AtomUI Core 的默认产品目录。
- 主题 XML 是主题身份、显示名称、算法和 Seed Token 的规范来源。
- 主题色、深色、紧凑、Motion 和 Wave Spirit 可以正交组合。
- AtomUI Theme 通过统一 Resolver 同时支持内置资源、Gallery 资源和用户配置目录主题。
- Desktop Gallery 启动时加载用户主题，并允许手动刷新；刷新原子更新 Catalog 与当前主题 Snapshot。
- 主题切换继续复用稳定 `ThemeContext` 和 `ThemeTokenResourceProvider`，不追加资源层。
- 启动与运行时路径保持 NativeAOT 友好，不扫描程序集或资源目录。

本次不包含：

- 自定义颜色选择器或运行时写主题文件。
- 网络主题下载、主题编辑器和 `FileSystemWatcher` 实时监听。
- 跨进程持久化上次选择。Gallery 现有主题开关也不持久化，本次保持相同生命周期。
- 修改成功色、警告色、错误色等语义 Token。主题 XML 只改变 `ColorPrimary`。

## 2. 最终主题集合

| Theme Id | Name | `ColorPrimary` | 所有者 | 默认 |
| --- | --- | --- | --- | --- |
| `DaybreakBlue` | Daybreak Blue | `#1677FF` | AtomUI Core | 是 |
| `PolarGreen` | Polar Green | `#52C41A` | AtomUIGallery | 否 |
| `SunsetOrange` | Sunset Orange | `#FA8C16` | AtomUIGallery | 否 |
| `GoldenPurple` | Golden Purple | `#722ED1` | AtomUIGallery | 否 |
| `Magenta` | Magenta | `#EB2F96` | AtomUIGallery | 否 |

`Polar Cyan` 不进入最终集合。Green 与 Cyan 只保留用户明确选择的 Green。

所有颜色采用 AtomUI `PresetPrimaryColor` 已有的 Ant Design Seed，避免引入未经主题算法验证的新色值。主题名是
品牌名，不进入普通 Gallery 多语言资源；菜单直接显示 XML 中的 `Theme.Name`。

## 3. XML 主题定义

### 3.1 文件布局

Core 默认主题继续由 Core 提供，并把文件名调整到 v1 推荐的 `<theme-id>.theme.xml` 约定：

```text
src/AtomUI.Core/Assets/Themes/DaybreakBlue.theme.xml
```

Gallery 专属主题放在 Gallery 资产目录：

```text
controlgallery/AtomUIGallery/Assets/Themes/
├── PolarGreen.theme.xml
├── SunsetOrange.theme.xml
├── GoldenPurple.theme.xml
└── Magenta.theme.xml
```

`AtomUIGallery.csproj` 已用 `AvaloniaResource Include="Assets/**/*.*"` 打包该目录，不需要增加文件系统复制或运行时
路径推断。

### 3.2 XML 内容

`DaybreakBlue` 显式声明 Ant Design 默认主色，使默认主题 XML 与新增主题拥有相同的自描述结构：

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
</Theme>
```

其余 XML 结构相同，只替换 `Id`、`Name` 和 `ColorPrimary`，并且不声明 `IsDefault`。例如：

```xml
<?xml version="1.0" encoding="utf-8"?>
<Theme xmlns="https://atomui.net/schemas/theme/v1"
       Id="PolarGreen"
       Name="Polar Green"
       Appearance="Light">
  <Algorithms>
    <Algorithm Id="Default" />
  </Algorithms>
  <Tokens>
    <Token Name="ColorPrimary" Value="#52C41A" />
  </Tokens>
</Theme>
```

XML 不复制派生色阶、Control Token、Dark 或 Compact 配置。`Default` 算法从 `ColorPrimary` 生成 Map/Alias Token；
Dark 和 Compact 仍由运行时算法链叠加。

## 4. Theme Definition Resolver

### 4.1 当前缺口

当前公开的 `IThemeManagerBuilder` 没有注册主题定义来源的入口，`CompiledThemeCatalog.LoadBuiltIn()` 也只加载硬编码
的 `DaybreakBlue.xml`。把 Gallery XML 放入资源目录或用户配置目录本身都不会让 ThemeManager 发现它们。

由于主题归 Gallery 所有，不能通过把四个 XML 移入 Core 来绕过该缺口。本次把硬编码加载重构为通用
`IThemeDefinitionResolver` 链。这是一个有意的 Public API 增量，必须同步 API 测试和文档。

### 4.2 公开契约

在 `AtomUI.Theme.Definitions` 增加 Resolver 与 source 契约：

```csharp
public interface IThemeDefinitionResolver
{
    string Id { get; }
    bool SupportsReload { get; }
    ThemeDefinitionResolveResult Resolve(ThemeDefinitionResolveContext context);
}

public interface IThemeDefinitionSource
{
    string SourceIdentity { get; }
    string SourceRevision { get; }
    Stream OpenRead();
}
```

在 `IThemeManagerBuilder` 增加：

```csharp
void AddThemeDefinitionResolver(IThemeDefinitionResolver resolver);
void WithApplicationId(string applicationId);
void UseUserThemeDirectory();
void UseUserThemeDirectory(string directory);
```

边界约束：

- Resolver Id 必须非空且唯一，Resolve 结果在注册/加载边界防御性复制。
- Resolver 只发现并打开来源；XML 仍统一经过受限 Reader、XSD、Binder 和 Compiler。
- 内置与 Gallery Resolver 显式列出绝对 `avares://` URI，不扫描程序集或资源目录。
- 用户 Resolver 只枚举配置目录顶层 `*.theme.xml`，不递归、不跟随链接。
- `ThemeConfig` 继续只表示运行时覆盖，Resolver 不返回 `ThemeConfig`。

Core 内部 Resolver 始终先提供 `DaybreakBlue.theme.xml`。应用 Resolver 只能追加主题，不能移除或替换 Core
默认主题。

### 4.3 Gallery 与用户目录 Resolver

AtomUIGallery 增加 internal Gallery asset Resolver，用静态只读数组明确返回四个资源 URI：

```text
avares://AtomUIGallery/Assets/Themes/PolarGreen.theme.xml
avares://AtomUIGallery/Assets/Themes/SunsetOrange.theme.xml
avares://AtomUIGallery/Assets/Themes/GoldenPurple.theme.xml
avares://AtomUIGallery/Assets/Themes/Magenta.theme.xml
```

`UseGalleryControls()` 负责调用 `AddThemeDefinitionResolver(...)`。因此只有使用 AtomUIGallery 产品模块的应用看到这
四个主题，单独使用 `UseGalleryBase()` 的第三方 Gallery 不会被注入 AtomUIGallery 品牌主题。

Desktop Gallery 另外启用：

```csharp
builder.WithApplicationId("AtomUIGallery");
builder.UseUserThemeDirectory();
```

默认用户目录为 `Environment.SpecialFolder.ApplicationData/AtomUIGallery/Themes`。未显式设置 App Id 的普通应用
使用具体 Application 类型程序集的简单名称作为默认值。Browser Gallery 不注册用户目录 Resolver。

用户 Resolver 支持手动刷新，不使用 `FileSystemWatcher`。任一用户 XML 无效时整批刷新失败并保留旧 Catalog；
首次启动失败则忽略整个用户来源层，以完整静态 Catalog 启动并暴露 diagnostics。

## 5. Catalog 加载与验证

`CompiledThemeCatalog` 从“加载一个硬编码文件”改为“加载 Resolver source 的有序快照”。每个
资源只读取一次字节，并沿用现有管线：

```text
resolver source (avares or user file)
    -> IThemeDefinitionSource.OpenRead
    -> bounded byte buffer + SHA-256
    -> ThemeDocumentReader (XML + XSD)
    -> ThemeDefinitionBinder (frozen registry)
    -> BoundThemeDefinition
    -> CompiledThemeCatalog
```

Catalog 构建规则：

- Resolver 提供的 source identity 是 `ThemeDefinitionRevision.SourceIdentity`；avares 使用资源 URI，用户文件使用
  规范化绝对路径。
- Source revision 使用 `IThemeDefinitionSource.SourceRevision`；实际读取字节的 SHA-256 继续作为内容摘要。
- 不允许重复 `Theme.Id`。
- 合并后的 Catalog 必须恰好有一个 `IsDefault=true`，本次仍为 `DaybreakBlue`。
- 静态资源缺失、XML/XSD 无效、Binder 失败、重复 ID 或默认主题冲突都会使启动失败。
- 可刷新来源不得声明默认主题，也不得覆盖静态 Theme Id。
- 用户目录刷新整批原子；任一文件失败时旧 Catalog 和 Snapshot 不变。
- 不静默跳过坏主题，避免菜单、Catalog 与实际可加载定义不一致。
- `AvailableThemes` 保留 Resolver 与 source 的稳定顺序：Core 默认主题在前，随后是 Gallery 的 Green、Orange、
  Purple、Magenta，最后是用户主题。

`ThemeInfo` 不增加 `ColorPrimary`。它继续只表示主题目录元数据，菜单也不通过运行时 Token 反向读取主题色。这样
不会把任意主题 Token 提升成 Catalog 公开契约。

## 6. Gallery 菜单与 ViewModel

### 6.1 菜单结构

标题栏“主题”菜单调整为：

```text
主题
├── Daybreak Blue       (Radio, ThemeColor)
├── Polar Green         (Radio, ThemeColor)
├── Sunset Orange       (Radio, ThemeColor)
├── Golden Purple       (Radio, ThemeColor)
├── Magenta             (Radio, ThemeColor)
├── user themes...      (Radio, ThemeColor)
├── separator
├── 重新加载用户主题
├── 暗黑模式             (CheckBox)
├── 紧凑模式             (CheckBox)
├── 启用动画             (CheckBox)
└── 启用点击波纹          (CheckBox)
```

主题 Radio 项由 `IThemeManager.AvailableThemes` 构建，Header 使用 `ThemeInfo.Name`，命令参数使用
`ThemeInfo.Id`。不在 AXAML 中复制主题名和主题 ID。所有颜色项使用同一个 `GroupName`，同一时刻只选中一项。

第一版不显示色点。XML v1 没有“菜单色样”元数据，重复维护一份 Id 到颜色的映射会制造第二来源。主题名已经足以
区分选项；如果以后要显示色样，应另行设计通用主题展示元数据，而不是从已编译 Snapshot 猜测。

### 6.2 GalleryWorkspaceViewModel 状态

`GalleryWorkspaceViewModel` 增加：

- `AvailableThemes`：从 ThemeManager 的只读主题列表初始化，并在 `ThemeCatalogChanged` 后原子替换菜单数据。
- `CurrentThemeId`：只在已提交主题状态变化后更新。
- `SwitchThemeCommand`：接收目标 Theme Id，调用统一主题请求路径。
- `ReloadThemesCommand`：调用 `ReloadThemesAsync()` 并返回 result；失败时保留旧菜单和选中态，diagnostics 交给
  调用方记录或展示。

`ApplyThemeSettingsAsync()` 改为接收可选目标 Theme Id，并始终一次性组合完整请求：

```text
Theme Id: requested id or CurrentTheme.ThemeId
Algorithms: base algorithms + Compact? + Dark?
Tokens:
  EnableMotion
  EnableWaveSpirit
```

主题色不写入运行时 `ThemeConfig`。切换 Theme Id 后，新的 XML definition 自己提供 `ColorPrimary`，从而不会把旧
主题 Seed 带入新主题。

### 6.3 正交组合

以下状态必须在切换主题色时保留：

- Dark：保留并把 `Dark` 算法附加在 definition 的 `Default` 基线之后。
- Compact：保留 `Compact` 算法。
- Motion：保留 `EnableMotion`。
- Wave Spirit：保留 `EnableWaveSpirit`，且继续服从 Motion 的现有依赖关系。

不为每个主题生成 Dark XML 或 Compact XML。五个 Theme Id 与两个布尔算法维度组合，但 Catalog 中仍只有五个
definition。

`ThemeChanged` 是菜单选中态的唯一提交信号。用户连续快速选择多个主题时，由 ThemeManager generation 规则让旧
请求返回 `Superseded`；UI 不提前把 `CurrentThemeId` 当成已提交状态，因此不会停留在过期选项。

## 7. 失败处理

### 7.1 启动失败

主题资源属于随应用发布的内置资产。缺少资源、非法 XML、未知 Token/Algorithm、重复 ID 或多个默认主题表示构建
产物损坏，应在 ThemeManager 初始化时抛出带结构化 diagnostic 的 `ThemeLoadException`，而不是带着不完整菜单
继续启动。

用户目录是外部可编辑输入。首次加载失败不使 Gallery 无法启动，而是排除整个用户来源层，保留 Core + Gallery
静态 Catalog，并把 diagnostics 提供给 Gallery。静态 Resolver 失败仍按损坏发布产物处理。

### 7.2 运行时失败

运行时切换失败时：

- ThemeManager 保证旧 Snapshot 和资源未修改。
- `CurrentThemeId` 保持旧值。
- 菜单 Radio 重新同步到旧主题。
- diagnostic 进入现有 `ThemeLoadException`/ReactiveCommand 错误路径，不吞掉异常。
- `NoOp` 视为有效结果，不重复发布资源或事件。
- `Superseded` 不显示错误，也不覆盖较新请求的选中态。

本次不新增 Toast 或 Dialog 错误 UI；内置主题在构建和启动阶段已经验证，正常发布产物不应出现运行时定义错误。

### 7.3 Catalog 刷新失败

手动刷新遵循 Capture、Resolve/Prepare、CommitCore、Publish、Complete 五阶段。任一用户文件错误、重复 Id、
默认主题声明、读取或编译失败时返回 `Failed`，旧 Catalog、CurrentTheme 和全部 root/local Snapshot 完整保留。
如果当前用户主题文件被正常删除且其余目录有效，则原子回退 Daybreak Blue，并保留 Dark、Compact、Motion 和
Wave Spirit 配置。

## 8. 测试策略

### 8.1 AtomUI.Core

增加或扩展 Core 主题测试：

- Builder 拒绝空/重复 Resolver Id 和非法 Application Id。
- Catalog 能合并 Core、应用 avares 与用户目录 Resolver 的多份有效 XML。
- `AvailableThemes` 顺序与显式注册顺序一致。
- 重复 Theme Id、零个/多个默认主题、缺失资源和无效 XML 使 Catalog 原子失败。
- 每个 definition 生成独立 revision/content fingerprint。
- 对每个 Theme Id 调用 `ApplyThemeAsync` 后，`CurrentTheme.ThemeId` 与编译后的 `ColorPrimary` 一致。
- Public API 审计更新，确认只增加计划内的 Resolver、Catalog reload 和 Builder 方法。
- 用户目录文件数、总字节、symlink、目录逃逸、重复 Id、默认主题和整批失败策略均有测试。
- `ReloadThemesAsync` 覆盖 Committed、NoOp、Superseded、Failed、当前主题重编译和删除回退。
- Catalog、CurrentTheme 与全部 scope Snapshot 在刷新通知前属于同一 generation。

### 8.2 AtomUIGallery

增加 Gallery 测试：

- 四个 Gallery XML 均通过 v1 XSD 与当前 Gallery 完整 ThemeSchemaRegistry 绑定。
- `UseGalleryControls()` 注册后 `AvailableThemes` 恰好包含五个预期主题。
- 菜单创建五个同组 Radio 项，并且只有当前主题被选中。
- 从 Blue 切到 Green、Orange、Purple、Magenta 时 Theme Id 与主色正确。
- 在 Dark + Compact + Motion/Wave 不同组合下切换主题色，所有正交状态保持不变。
- 连续切换时旧请求 `Superseded` 不覆盖最终选中项。
- 失败结果保持旧菜单选中态。
- 手动刷新成功后用户主题进入菜单；失败后旧列表和选中态不变。

### 8.3 验证命令

实现完成后至少运行：

```bash
dotnet test tests/AtomUI.Core.Tests/AtomUI.Core.Tests.csproj --framework net10.0 --no-restore
dotnet test tests/AtomUIGallery.Tests/AtomUIGallery.Tests.csproj --framework net10.0 --no-restore
dotnet build controlgallery/AtomUIGallery.Desktop/AtomUIGallery.Desktop.csproj --framework net10.0 --no-restore
pwsh -NoLogo -NoProfile -File controlgallery/AtomUIGallery.Desktop/scripts/PublishToLocal.ps1 \
  -publishRootPath /tmp/atomui-gallery-aot-run \
  -runtime osx-arm64 \
  -buildType Release \
  -publishAot true
git diff --check
```

NativeAOT publish 是必需验收项，因为本次增加跨程序集 Avalonia 资源、用户目录读取和公开 Resolver 路径。

## 9. 文档影响

实现时同步更新：

- `docs/architecture/startup-and-registration.md`：记录 Resolver、Application Id 与用户目录注册契约。
- `docs/modules/core/theme-system.md`：记录 Resolver、来源优先级、Catalog 原子刷新和用户文件安全限制。
- `docs/modules/core/theme-definition-xml.md`：只在需要澄清内置 Avalonia 资源注册示例时补充，不改变 XML v1。
- `docs/modules/toolkits-gallery-base/theming-localization.md`：记录 `AvailableThemes`、`CurrentThemeId` 和
  `SwitchThemeCommand` 的 Gallery Shell 行为。

XML v1 namespace、XSD 和 Token schema 不发生变更。

## 10. 实施边界总结

本方案选择“Gallery 拥有 XML，Core 提供通用 Resolver 与 Catalog 刷新能力”。它避免把 Gallery 展示主题变成
所有 AtomUI 应用的默认产品契约，同时让 AtomUI 从应用资源和用户配置目录加载标准主题定义。

运行时只切换 `ThemeRequest.ThemeId` 并组合现有算法/Token 开关；主题色仍经标准 definition、compiler、snapshot
和稳定资源 Provider 发布。没有动态扫描、临时资源字典或针对单个控件的换色补丁。
