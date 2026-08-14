# QRCode 语义结构

> 生成产物：由源文档生成，不要手工编辑。修改内容请回到控件文档、源码 public surface、Token 类型或生成数据、Gallery ShowCase 或源码结构。

## Semantic Parts

QRCode 公开 `root` 和 `cover` 两个 Semantic Part。二维码 bitmap、中心图标、刷新按钮和状态内容内部节点不属于公共 Part。

### 1.1 `QRCode`

#### `root`

| 字段 | 值 |
| --- | --- |
| Owner | `QRCode` |
| Part | `root` |
| Selector | QRCode 本身 |
| SelectorRoute | 不适用 |
| Style Type | 不适用（root 不生成 Style） |
| ContractType | `QRCode` |
| Cardinality | `Single` |
| Customization | `Root` |
| CrossVisualRoot | `false` |
| RuntimeCreated | `false` |
| AtomUI 节点 | QRCode owner |
| 职责 | 二维码方形根区域，承载背景、边框、圆角、Padding 和整体布局。 |
| 相关 API | `Size`、`IsBordered` 及继承的 root 表面属性 |
| 相关 Token | SharedToken |
| 稳定性 | stable since 6.0 |

#### `cover`

| 字段 | 值 |
| --- | --- |
| Owner | `QRCode` |
| Part | `cover` |
| Selector | `.semantic-cover` |
| SelectorRoute | `/template/ .semantic-cover` |
| Style Type | `QRCodeCoverStyle` |
| ContractType | `Border` |
| Cardinality | `Single` |
| Customization | `Selector` |
| CrossVisualRoot | `false` |
| RuntimeCreated | `false` |
| AtomUI 节点 | 状态 overlay `Border` |
| 职责 | 覆盖完整 root，承载 Loading、Expired、Scanned 状态背景与内容。 |
| 相关 API | `Status`、三组状态内容 API、`RefreshRequested` |
| 相关 Token | `QRCodeMaskBackgroundColor`、SharedToken |
| 稳定性 | stable since 6.0 |

## Abstract AXAML Structure

来源：`src/AtomUI.Desktop.Controls/QRCode/Themes/QRCodeTheme.axaml`

```xml
<PixelAlignedBorder Name="Frame">
    <Grid>
        <Border Name="ContentFrame">
            <Viewbox Name="QRCodeSurfaceScaler">
                <Grid>
                    <Image />
                    <Border Name="ImageFrame">
                        <Image />
                    </Border>
                </Grid>
            </Viewbox>
        </Border>
        <Border Name="Cover">
            <Panel>
                <Panel Name="LoadingLayout">
                    <Spin />
                    <ContentPresenter />
                </Panel>
                <Panel Name="ExpiredLayout">
                    <StackPanel>
                        <TextBlock />
                        <Button Name="PART_RefreshButton" />
                    </StackPanel>
                    <ContentPresenter />
                </Panel>
                <Panel Name="ScannedLayout">
                    <TextBlock />
                    <ContentPresenter />
                </Panel>
            </Panel>
        </Border>
    </Grid>
</PixelAlignedBorder>
```

## Composition Model

该章节由控件 `Themes/` 文件夹中的真实主题文件生成，用于说明 public 控件与内部协作对象之间的运行时结构。内部节点只用于理解和维护，不应指导用户代码直接依赖。

### 控件角色图

```text
QRCode
  -> QRCode (control theme, QRCodeTheme.axaml)
     -> PixelAlignedBorder#Frame (template-stable)
        -> Grid (template-stable)
           -> Border#ContentFrame (template-stable)
              -> Viewbox#QRCodeSurfaceScaler (template-stable)
                 -> Grid (template-stable)
                    -> Image (template-stable)
                    -> Border#ImageFrame (template-stable)
                       -> Image (template-stable)
           -> Border#Cover (template-stable)
              -> Panel (template-stable)
                 -> Panel#LoadingLayout (template-stable)
                    -> Spin (template-stable)
                    -> ContentPresenter (internal-observable)
                 -> Panel#ExpiredLayout (template-stable)
                    -> StackPanel (template-stable)
                       -> TextBlock (template-stable)
                       -> Button#PART_RefreshButton (template-stable)
                    -> ContentPresenter (internal-observable)
                 -> Panel#ScannedLayout (template-stable)
                    -> TextBlock (template-stable)
                    -> ContentPresenter (internal-observable)
```

### 协作节点

| 节点 | 类型 | 来源 | 生命周期 owner | 影响的 public API | 稳定性 | Agent 使用边界 |
| --- | --- | --- | --- | --- | --- | --- |
| `QRCode` | public control | `源文档 + public API` | 用户代码 / 控件宿主 | public API | public | 用户可直接使用 public 控件；可作为示例和 API 入口。 |
| `QRCode` | control theme | `QRCodeTheme.axaml` | 用户代码 / 控件宿主 | `Background`, `Bitmap`, `BorderBrush`, `BorderThickness`, `CornerRadius`, `ExpiredContent` | public | 用户可直接使用 public 控件；可作为示例和 API 入口。 |
| `Frame` | template node (PixelAlignedBorder) | `QRCodeTheme.axaml` | QRCode | `Background`, `Bitmap`, `BorderBrush`, `BorderThickness`, `CornerRadius`, `ExpiredContent` | template-stable | 用于主题维护；变更需同步主题、实现和 LLMS。 |
| `ContentFrame` | template node (Border) | `QRCodeTheme.axaml` | QRCode | `Bitmap`, `Icon`, `IconBgColor`, `IconSize`, `Padding`, `Size` | template-stable | 用于主题维护；变更需同步主题、实现和 LLMS。 |
| `QRCodeSurfaceScaler` | template node (Viewbox) | `QRCodeTheme.axaml` | QRCode | `Bitmap`, `Icon`, `IconBgColor`, `IconSize`, `Size` | template-stable | 用于主题维护；变更需同步主题、实现和 LLMS。 |
| `ImageFrame` | template node (Border) | `QRCodeTheme.axaml` | QRCode | `Icon`, `IconBgColor`, `IconSize` | template-stable | 用于主题维护；变更需同步主题、实现和 LLMS。 |
| `Cover` | template node (Border) | `QRCodeTheme.axaml` | QRCode | `ExpiredContent`, `ExpiredContentTemplate`, `LoadingContent`, `LoadingContentTemplate`, `ScannedContent`, `ScannedContentTemplate` | template-stable | 用于主题维护；变更需同步主题、实现和 LLMS。 |
| `Panel` | template node (Panel) | `QRCodeTheme.axaml` | QRCode | `ExpiredContent`, `ExpiredContentTemplate`, `LoadingContent`, `LoadingContentTemplate`, `ScannedContent`, `ScannedContentTemplate` | template-stable | 用于主题维护；变更需同步主题、实现和 LLMS。 |
| `LoadingLayout` | template node (Panel) | `QRCodeTheme.axaml` | QRCode | `LoadingContent`, `LoadingContentTemplate` | template-stable | 用于主题维护；变更需同步主题、实现和 LLMS。 |
| `ContentPresenter` | template node (ContentPresenter) | `QRCodeTheme.axaml` | QRCode | `LoadingContent`, `LoadingContentTemplate` | internal-observable | 用于理解结构和状态流，不应指导用户代码直接依赖。 |
| `ExpiredLayout` | template node (Panel) | `QRCodeTheme.axaml` | QRCode | `ExpiredContent`, `ExpiredContentTemplate` | template-stable | 用于主题维护；变更需同步主题、实现和 LLMS。 |
| `StackPanel` | template node (StackPanel) | `QRCodeTheme.axaml` | QRCode | `ExpiredContent` | template-stable | 用于主题维护；变更需同步主题、实现和 LLMS。 |
| `PART_RefreshButton` | template node (Button) | `QRCodeTheme.axaml` | QRCode | 主题状态 / visual state | template-stable | 用于主题维护；变更需同步主题、实现和 LLMS。 |
| `ScannedLayout` | template node (Panel) | `QRCodeTheme.axaml` | QRCode | `ScannedContent`, `ScannedContentTemplate` | template-stable | 用于主题维护；变更需同步主题、实现和 LLMS。 |

## Template Parts

| 契约组 | 代表成员 | 维护含义 |
| --- | --- | --- |
| 内容与数据 | `ExpiredContent`、`ExpiredContentTemplate`、`Icon`、`IconBgColor`、`IconSize`、`LoadingContent`、`LoadingContentTemplate`、`ScannedContent`、`ScannedContentTemplate`、`Value` | 定义控件展示内容、输入数据、模板或业务对象入口。 |
| 交互与状态 | `IsBordered`、`Status`、`RefreshRequested` | 表达边框模式、状态遮罩和过期状态下的刷新请求。 |
| 视觉与布局 | `Color`、`Size` 及继承的 root 表面属性 | `Size` 统一拥有二维码方形边长；颜色、背景、边框、圆角和 Padding 形成 root 视觉。 |
| 其他稳定入口 | `EccLevel` | 保留为 public surface，变更前需确认 Gallery 和用户 XAML 依赖。 |

## Pseudo Classes

QRCode 的公共契约由 public/protected 类型成员、Avalonia 属性、事件、命令、template part、伪类、ControlTheme key 和资源 key 共同组成。维护时应先确认这些契约是否已经被源码、Gallery 示例或文档暴露。

核心 public surface 按语义分组维护：

| 契约组 | 代表成员 | 维护含义 |
| --- | --- | --- |
| 内容与数据 | `ExpiredContent`、`ExpiredContentTemplate`、`Icon`、`IconBgColor`、`IconSize`、`LoadingContent`、`LoadingContentTemplate`、`ScannedContent`、`ScannedContentTemplate`、`Value` | 定义控件展示内容、输入数据、模板或业务对象入口。 |
| 交互与状态 | `IsBordered`、`Status`、`RefreshRequested` | 表达边框模式、状态遮罩和过期状态下的刷新请求。 |
| 视觉与布局 | `Color`、`Size` 及继承的 root 表面属性 | `Size` 统一拥有二维码方形边长；颜色、背景、边框、圆角和 Padding 形成 root 视觉。 |
| 其他稳定入口 | `EccLevel` | 保留为 public surface，变更前需确认 Gallery 和用户 XAML 依赖。 |

`RefreshRequested` 是 QRCode 的控件专属 public 事件，由默认过期状态中的 `PART_RefreshButton` 触发。自定义

## State Flow

QRCode 的状态流按以下路径收敛：

```text
Public API / inherited command / item source / user input
  -> 控件实例状态
  -> effective state / pseudo-class / template property
  -> ControlTheme selector / presenter / renderer
  -> Gallery 可观察行为
```

状态维护规则：

- `Active` 隐藏 cover；`Loading`、`Expired`、`Scanned` 显示同一个 cover 节点，并在节点内部切换对应状态内容。
- `Value`、`Color`、`EccLevel` 或 `Size` 变化时重新生成透明背景 bitmap，不替换 Semantic target；`Background` 只更新 root 表面。
- `Icon` 只控制二维码中心图标内容，不增加 Semantic Part，也不改变 root/cover 数量。
- 模板重套用时重新接入 `PART_RefreshButton`，先移除旧按钮订阅，再回放当前 public API 状态。
- `Size` 是二维码外框与绘制源的统一边长；Semantic Style 不建立第二套 Width/Height 尺寸 owner。

## Theme and Token Boundaries

QRCode 的视觉模型由控件模板、ControlTheme、SharedToken 和必要的控件 Token 共同构成。

| 主题文件 | 职责 |
| --- | --- |
| `QRCodeTheme.axaml` | 提供控件模板、selector、资源绑定和状态视觉。 |

QRCode 使用 `QRCodeToken` 作为控件 Token scope。Token 只表达文字色和 cover 背景色等视觉语义，不承载 `Status`、`Value` 或 bitmap
运行时状态。

主题维护规则：

- 不删除或重命名已经稳定的 ControlTheme key、template part、伪类和资源 key。
- 不把可由 AXAML 表达的模板状态迁移为 C# 动态创建视觉。
- 不把 hover、pressed、selected、expanded、loading、filter、popup open 等运行时状态写入 Token。
- Browser 或平台特化主题必须保持同一 API 的语义一致。

Token 边界：

QRCode Token 只表达组件级视觉变量，例如尺寸、间距、颜色、圆角、阴影、图标尺寸和弹层边界。Token 不承载运行时选择、展开、加载、错误、上传任务、过滤条件或业务状态。

当前 Token scope：

- `QRCodeToken`，scope id 为 `QRCode`，源码位于 `src/AtomUI.Desktop.Controls/QRCode/QRCodeToken.cs`。

## Customization Boundaries

维护 QRCode 时必须保持以下不变量：

- 不擅自新增、删除、重命名或改变 public/protected API、Avalonia 属性、事件和默认值。
- 不破坏 template part、伪类、ControlTheme key、Token 名称和资源 key。
- 不改变 Gallery 已展示的 XAML 用法、默认外观、交互顺序和状态优先级；Semantic 示例必须保持与对应公开上游示例一致。
- Template part 重新应用、集合替换、弹层关闭、窗口失活和控件 detach 时必须释放旧订阅和资源宿主。
- 不通过隐藏延迟、强制刷新或吞异常掩盖状态同步问题。
- 不引入运行时反射扫描作为 API、Token 或数据路径发现机制。
- 文档只描述当前稳定设计；历史变化记录在 `changelog.md`。

维护不变量：

维护 QRCode 时不得破坏：

- Public API、默认值、事件顺序和 Gallery 可观察行为。
- `root`、`cover` 的名称、selector、ContractType、cardinality 和静态 marker 身份。
- `Size` 对外框与 bitmap 的统一方形尺寸 ownership。
- Template part 名称、ControlTheme key、伪类和资源 key。
- 旧 template part、事件订阅、Popup/Flyout/Window host 和 collection view 的释放路径。
- Light/Dark、Browser/Desktop 和不同 SizeType 下的主题一致性。
- 控件文档、源码 public surface、Token 类型或生成数据与源码契约的一致性。
