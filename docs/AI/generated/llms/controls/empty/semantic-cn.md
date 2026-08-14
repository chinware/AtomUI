# Empty 语义结构

> 生成产物：由源文档生成，不要手工编辑。修改内容请回到控件文档、源码 public surface、Token 类型或生成数据、Gallery ShowCase 或源码结构。

## Semantic Parts

Empty 公开 `root`、`image`、`description` 和 `footer` 四个 Semantic Part。该契约以 AtomUI 的 public API、静态
ControlTemplate 和 Avalonia 样式优先级作为实现事实。

### 1.1 `Empty`

#### `root`

| 字段 | 值 |
| --- | --- |
| Owner | `Empty` |
| Part | `root` |
| Selector | Empty 本身 |
| SelectorRoute | 不适用 |
| Style Type | 不适用（root 不生成 Style） |
| ContractType | `Empty` |
| Cardinality | `Single` |
| Customization | `Root` |
| CrossVisualRoot | `false` |
| RuntimeCreated | `false` |
| AtomUI 节点 | Empty owner |
| 职责 | 空状态的根布局、整体对齐、可见性和根视觉样式 owner。 |
| 相关 API | `Background`、`BorderBrush`、`BorderThickness`、`CornerRadius`、`Padding`、`StrokeDashArray` 及标准布局属性 |
| 相关 Token | EmptyToken、SharedToken |
| 稳定性 | stable since 6.0 |

#### `image`

| 字段 | 值 |
| --- | --- |
| Owner | `Empty` |
| Part | `image` |
| Selector | `.semantic-image` |
| SelectorRoute | `/template/ .semantic-image` |
| Style Type | `EmptyImageStyle` |
| ContractType | `Control` |
| Cardinality | `Single` |
| Customization | `Selector` |
| CrossVisualRoot | `false` |
| RuntimeCreated | `false` |
| AtomUI 节点 | `PART_SvgImage` 对应的 `Avalonia.Svg.Svg` |
| 职责 | 展示内置 Default/Simple 图形或 `ImagePath`、`ImageSource` 指定的 SVG 图形。 |
| 相关 API | `PresetImage`、`ImagePath`、`ImageSource`、`SizeType` |
| 相关 Token | `EmptyImgHeight`、`EmptyImgHeightMD`、`EmptyImgHeightSM`、图形颜色资源 |
| 稳定性 | stable since 6.0 |

#### `description`

| 字段 | 值 |
| --- | --- |
| Owner | `Empty` |
| Part | `description` |
| Selector | `.semantic-description` |
| SelectorRoute | `/template/ .semantic-description` |
| Style Type | `EmptyDescriptionStyle` |
| ContractType | `TextBlock` |
| Cardinality | `Single` |
| Customization | `Selector` |
| CrossVisualRoot | `false` |
| RuntimeCreated | `false` |
| AtomUI 节点 | 描述 `TextBlock` |
| 职责 | 展示本地化默认描述或调用方提供的 `Description`。 |
| 相关 API | `Description`、`IsDescriptionVisible`、`SizeType` |
| 相关 Token | `DescriptionMargin`、`DescriptionMarginSM`、SharedToken 文本颜色 |
| 稳定性 | stable since 6.0 |

#### `footer`

| 字段 | 值 |
| --- | --- |
| Owner | `Empty` |
| Part | `footer` |
| Selector | `.semantic-footer` |
| SelectorRoute | `/template/ .semantic-footer` |
| Style Type | `EmptyFooterStyle` |
| ContractType | `ContentPresenter` |
| Cardinality | `Single` |
| Customization | `Selector` |
| CrossVisualRoot | `false` |
| RuntimeCreated | `false` |
| AtomUI 节点 | Footer `ContentPresenter` |
| 职责 | 承载创建、刷新、返回或其他空状态后续操作。 |
| 相关 API | `Footer`、`FooterTemplate` |
| 相关 Token | `FooterMargin`、SharedToken |
| 稳定性 | stable since 6.0 |

## Abstract AXAML Structure

来源：`src/AtomUI.Desktop.Controls/Empty/Themes/EmptyTheme.axaml`

```xml
<DashedBorder>
    <StackPanel>
        <Svg Name="PART_SvgImage" />
        <TextBlock Name="Description" />
        <ContentPresenter />
    </StackPanel>
</DashedBorder>
```

## Composition Model

该章节由控件 `Themes/` 文件夹中的真实主题文件生成，用于说明 public 控件与内部协作对象之间的运行时结构。内部节点只用于理解和维护，不应指导用户代码直接依赖。

### 控件角色图

```text
Empty
  -> Empty (control theme, EmptyTheme.axaml)
     -> DashedBorder (template-stable)
        -> StackPanel (template-stable)
           -> Svg#PART_SvgImage (template-stable)
           -> TextBlock#Description (template-stable)
           -> ContentPresenter (internal-observable)
```

### 协作节点

| 节点 | 类型 | 来源 | 生命周期 owner | 影响的 public API | 稳定性 | Agent 使用边界 |
| --- | --- | --- | --- | --- | --- | --- |
| `Empty` | public control | `源文档 + public API` | 用户代码 / 控件宿主 | public API | public | 用户可直接使用 public 控件；可作为示例和 API 入口。 |
| `Empty` | control theme | `EmptyTheme.axaml` | 用户代码 / 控件宿主 | `Background`, `BorderBrush`, `BorderThickness`, `CornerRadius`, `Description`, `Footer` | public | 用户可直接使用 public 控件；可作为示例和 API 入口。 |
| `StackPanel` | template node (StackPanel) | `EmptyTheme.axaml` | Empty | `Description`, `Footer`, `FooterTemplate`, `IsDescriptionVisible` | template-stable | 用于主题维护；变更需同步主题、实现和 LLMS。 |
| `PART_SvgImage` | template node (Svg) | `EmptyTheme.axaml` | Empty | 主题状态 / visual state | template-stable | 用于主题维护；变更需同步主题、实现和 LLMS。 |
| `Description` | template node (TextBlock) | `EmptyTheme.axaml` | Empty | `Description`, `IsDescriptionVisible` | template-stable | 用于主题维护；变更需同步主题、实现和 LLMS。 |
| `ContentPresenter` | template node (ContentPresenter) | `EmptyTheme.axaml` | Empty | `Footer`, `FooterTemplate` | internal-observable | 用于理解结构和状态流，不应指导用户代码直接依赖。 |

## Template Parts

| 契约组 | 代表成员 | 维护含义 |
| --- | --- | --- |
| 内容与数据 | `Description`、`Footer`、`FooterTemplate`、`ImagePath`、`ImageSource`、`IsDescriptionVisible`、`PresetImage` | 定义空状态图片、描述和后续操作内容。 |
| 视觉与布局 | `SizeType`、`StrokeDashArray` | `SizeType` 选择预设尺寸基线；`StrokeDashArray` 配置 root 边框的虚线节奏。 |

## Pseudo Classes

Empty 的公共契约由 public/protected 类型成员、Avalonia 属性、事件、命令、template part、伪类、ControlTheme key 和资源 key 共同组成。维护时应先确认这些契约是否已经被源码、Gallery 示例或文档暴露。

核心 public surface 按语义分组维护：

| 契约组 | 代表成员 | 维护含义 |
| --- | --- | --- |
| 内容与数据 | `Description`、`Footer`、`FooterTemplate`、`ImagePath`、`ImageSource`、`IsDescriptionVisible`、`PresetImage` | 定义空状态图片、描述和后续操作内容。 |
| 视觉与布局 | `SizeType`、`StrokeDashArray` | `SizeType` 选择预设尺寸基线；`StrokeDashArray` 配置 root 边框的虚线节奏。 |

当前没有抽取到控件专属 public 事件；交互通知主要来自继承事件、命令或 Gallery 可观察状态。

主要公开类型与枚举：

## State Flow

Empty 的状态流按以下路径收敛：

```text
Public API / inherited command / item source / user input
  -> 控件实例状态
  -> effective state / pseudo-class / template property
  -> ControlTheme selector / presenter / renderer
  -> Gallery 可观察行为
```

状态维护规则：

- `PresetImage`、`ImagePath`、`ImageSource` 三者互斥，并始终更新同一个 image 模板节点。
- `IsDescriptionVisible` 直接控制 description 模板节点的可见性，不通过 C# 动态创建或删除 Visual。
- `Footer=null` 时 footer Presenter 隐藏；非空时由 `FooterTemplate` 或 Avalonia DataTemplate 机制生成内容。
- Large、Middle、Small 只改变 image 高度和描述间距，不改变 Semantic marker 数量。
- 模板重套用时必须把 public API 对应状态回放到新的 part 和主题变量。

## Theme and Token Boundaries

Empty 的视觉模型由控件模板、ControlTheme、SharedToken 和必要的控件 Token 共同构成。

| 主题文件 | 职责 |
| --- | --- |
| `EmptyTheme.axaml` | 提供控件模板、selector、资源绑定和状态视觉。 |

Empty 使用 `EmptyToken` 作为控件 Token scope。Token 只表达图片高度、描述间距、Footer 间距和图形颜色等视觉语义，不承载
实例内容或可见性状态。

主题维护规则：

- 不删除或重命名已经稳定的 ControlTheme key、template part、伪类和资源 key。
- 不把可由 AXAML 表达的模板状态迁移为 C# 动态创建视觉。
- 不把 hover、pressed、selected、expanded、loading、filter、popup open 等运行时状态写入 Token。
- Browser 或平台特化主题必须保持同一 API 的语义一致。

Token 边界：

Empty Token 只表达组件级视觉变量，例如尺寸、间距、颜色、圆角、阴影、图标尺寸和弹层边界。Token 不承载运行时选择、展开、加载、错误、上传任务、过滤条件或业务状态。

当前 Token scope：

- `EmptyToken`，scope id 为 `Empty`，源码位于 `src/AtomUI.Desktop.Controls/Empty/EmptyToken.cs`。

## Customization Boundaries

维护 Empty 时必须保持以下不变量：

- 除已经批准的 `Footer`、`FooterTemplate` 外，不擅自新增、删除、重命名或改变 public/protected API、Avalonia 属性、事件和默认值。
- 不破坏 template part、伪类、ControlTheme key、Token 名称和资源 key。
- 不改变 Gallery 已展示的 XAML 用法、默认外观、交互顺序和状态优先级。
- Template part 重新应用、集合替换、弹层关闭、窗口失活和控件 detach 时必须释放旧订阅和资源宿主。
- 不通过隐藏延迟、强制刷新或吞异常掩盖状态同步问题。
- 不引入运行时反射扫描作为 API、Token 或数据路径发现机制。
- 文档只描述当前稳定设计；历史变化记录在 `changelog.md`。

维护不变量：

维护 Empty 时不得破坏：

- Public API、默认值、事件顺序，以及新增 `Footer`/`FooterTemplate` 的内容语义。
- Template part 名称、ControlTheme key、伪类和资源 key。
- 旧 template part、事件订阅、Popup/Flyout/Window host 和 collection view 的释放路径。
- Light/Dark、Browser/Desktop 和不同 SizeType 下的主题一致性。
- 控件文档、源码 public surface、Token 类型或生成数据与源码契约的一致性。
