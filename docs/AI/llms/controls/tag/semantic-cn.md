# Tag 语义结构

> 生成产物：由源文档生成，不要手工编辑。修改内容请回到控件文档、源码 public surface、Token 类型或生成数据、Gallery ShowCase 或源码结构。

## Semantic Parts

| Part | AtomUI 节点 | 职责 | 相关 API | 相关 Token | 稳定性 |
| --- | --- | --- | --- | --- | --- |
| `root` | `Tag` | 展示标签根区域，承载颜色、Variant、内容和关闭入口。 | `TagColor`、`Variant`、`Text`、`IsClosable` | TagToken | stable |
| `checkable` | `CheckableTag` | 二态选择标签根区域，承载输入、内容、Icon 和选择视觉。 | `IsChecked`、`Content`、`Icon`、`IsEnabled` | TagToken + SharedToken | stable |
| `group` | `CheckableTagGroup` | 选择组根区域，拥有 Options、模式、公开值和 Form 语义。 | `Options`、`IsMultiple`、`CheckedItem(s)` | SharedToken spacing | stable |
| `item` | `CheckableTag container` | Group 内单个可交互选项，只消费 owner 投影的状态。 | `IsChecked`、`ContentTemplate` | TagToken + SharedToken | stable |
| `content` | `ContentPresenter` | 承载 Tag 文本或 CheckableTag 的任意内容。 | `Text` 或 `Content` | typography/spacing | stable |

## Abstract AXAML Structure

来源：`src/AtomUI.Desktop.Controls/Tag/Themes/TagTheme.axaml`

```xml
<Panel>
    <PixelAlignedBorder Name="Frame" />
    <DockPanel>
        <IconPresenter Name="IconPresenter" />
        <IconButton Name="PART_CloseButton" />
        <TextBlock Name="TagTextLabel" />
    </DockPanel>
</Panel>
```

## Composition Model

该章节由控件 `Themes/` 文件夹中的真实主题文件生成，用于说明 public 控件与内部协作对象之间的运行时结构。内部节点只用于理解和维护，不应指导用户代码直接依赖。

### 控件角色图

```text
Tag
  -> CheckableTagItemsControl (control theme, CheckableTagItemsControlTheme.axaml)
     -> ItemsPresenter#PART_ItemsPresenter (template-stable)
  -> Tag (control theme, TagTheme.axaml)
     -> Panel (template-stable)
        -> PixelAlignedBorder#Frame (template-stable)
        -> DockPanel (template-stable)
           -> IconPresenter#IconPresenter (internal-observable)
           -> IconButton#PART_CloseButton (template-stable)
           -> TextBlock#TagTextLabel (template-stable)
```

### 协作节点

| 节点 | 类型 | 来源 | 生命周期 owner | 影响的 public API | 稳定性 | Agent 使用边界 |
| --- | --- | --- | --- | --- | --- | --- |
| `Tag` | public control | `源文档 + public API` | 用户代码 / 控件宿主 | public API | public | 用户可直接使用 public 控件；可作为示例和 API 入口。 |
| `CheckableTagItemsControl` | control theme | `CheckableTagItemsControlTheme.axaml` | Tag | `ItemsPanel` | internal-observable | 用于理解结构和状态流，不应指导用户代码直接依赖。 |
| `PART_ItemsPresenter` | template node (ItemsPresenter) | `CheckableTagItemsControlTheme.axaml` | CheckableTagItemsControl | `ItemsPanel` | template-stable | 用于主题维护；变更需同步主题、实现和 LLMS。 |
| `Tag` | control theme | `TagTheme.axaml` | 用户代码 / 控件宿主 | `Background`, `BorderBrush`, `BorderThickness`, `CloseIcon`, `CornerRadius`, `Foreground` | public | 用户可直接使用 public 控件；可作为示例和 API 入口。 |
| `Panel` | template node (Panel) | `TagTheme.axaml` | Tag | `Background`, `BorderBrush`, `BorderThickness`, `CloseIcon`, `CornerRadius`, `Icon` | template-stable | 用于主题维护；变更需同步主题、实现和 LLMS。 |
| `Frame` | template node (PixelAlignedBorder) | `TagTheme.axaml` | Tag | `Background`, `BorderBrush`, `BorderThickness`, `CornerRadius` | template-stable | 用于主题维护；变更需同步主题、实现和 LLMS。 |
| `DockPanel` | template node (DockPanel) | `TagTheme.axaml` | Tag | `CloseIcon`, `Icon`, `IsClosable`, `Padding`, `TagTextPaddingInline`, `Text` | template-stable | 用于主题维护；变更需同步主题、实现和 LLMS。 |
| `IconPresenter` | template node (IconPresenter) | `TagTheme.axaml` | Tag | `Icon` | internal-observable | 用于理解结构和状态流，不应指导用户代码直接依赖。 |
| `PART_CloseButton` | template node (IconButton) | `TagTheme.axaml` | Tag | `CloseIcon`, `IsClosable` | template-stable | 用于主题维护；变更需同步主题、实现和 LLMS。 |
| `TagTextLabel` | template node (TextBlock) | `TagTheme.axaml` | Tag | `TagTextPaddingInline`, `Text` | template-stable | 用于主题维护；变更需同步主题、实现和 LLMS。 |

## Template Parts

| 契约组 | 代表成员 | 维护含义 |
| --- | --- | --- |
| 内容与数据 | `CloseIcon`、`Icon`、`Text` | 定义控件展示内容、输入数据、模板或业务对象入口。 |
| 交互与状态 | `IsClosable`、`Variant` | 表达关闭能力和 Filled/Solid/Outlined 视觉形态。 |
| 视觉与布局 | `TagColor` | 选择 Default、Preset、Status 或 Custom 颜色类别。 |

## Pseudo Classes

| 状态反馈 | public API、内部状态和伪类如何形成用户可感知反馈。 | Tag 使用颜色分类和 `Variant`；CheckableTag 使用 `IsChecked`；Group 使用 `CheckedItem(s)`。 |
| 主题语义 | ControlTheme、SharedToken、组件 Token 和模板绑定如何表达视觉。 | Tag Token + ControlTheme。 |

## State Flow

Tag 的状态流按以下路径收敛：

```text
TagColor + Variant + ThemeSnapshot
  -> ColorCategory(Default / Preset / Status / Custom)
  -> Foreground / Background / BorderBrush
  -> effective visual state / pseudo-class
  -> ControlTheme selector / presenter / renderer
  -> Gallery 可观察行为
```

状态维护规则：

- `TagColor` 只负责颜色类别，`Variant` 只负责视觉形态；颜色解析不得反向修改 Variant。
- `default`、`null`、空值和无效颜色必须恢复 Default 颜色状态，不能残留之前的 Brush 或伪类。
- `info` 和 `processing` 使用同一组 Info 语义 Token；`default` 使用 Tag 基础 Token。
- 所有 Variant 保持相同边框厚度，Filled 和部分 Solid 通过透明边框表达无可见边界。
- 模板重套用时必须把 public API 对应状态回放到新的 part、伪类和主题变量。

CheckableTag 家族的状态流保持独立：

```text
CheckableTag.IsChecked
  -> ToggleButton input state
  -> checked pseudo-class / ControlTheme

Options + IsMultiple + CheckedItem(s)
  -> Group value normalization
  -> internal SelectionModel
  -> CheckableTag.IsChecked
```

Group 的内部 SelectedItem(s) 只保存归一后的 option wrapper，不是 public state；子项交互必须先回到 Group，再由 Group 通过 `SetCurrentValue` 更新公开值。

## Theme and Token Boundaries

Tag 的视觉模型由控件模板、ControlTheme、SharedToken 和必要的组件 Token 共同构成。

| 主题文件 | 职责 |
| --- | --- |
| `TagTheme.axaml` | 提供控件模板、selector、资源绑定和状态视觉。 |
| `CheckableTagTheme.axaml` | 提供二态标签的内容结构以及 checked、focus、disabled 等状态视觉。 |
| `CheckableTagGroupTheme.axaml` | 组合内部选择控件、ItemsPresenter 和 WrapPanel。 |
| `TagThemes.axaml` | 聚合控件家族主题资源，保证包级引入顺序稳定。 |

Tag 使用 `TagToken` 作为组件 Token scope。Token 只表达组件视觉语义，不承载 input/value、visual option 运行时状态。

Tag 的视觉组合由以下矩阵表达：

| 颜色类别 | Filled | Solid | Outlined |
| --- | --- | --- | --- |
| Default | 浅背景、透明边框、默认文字 | `ColorBgSolid` 背景、对比文字 | 默认背景、默认边框、默认文字 |
| Preset | palette 1 背景、palette 7 文字 | palette 6 背景、浅色文字、palette 6 边框 | palette 1 背景、palette 3 边框、palette 7 文字 |
| Status | 状态浅背景、状态主色文字 | 状态主色背景、浅色文字、状态主色边框 | 状态浅背景、状态边框、状态主色文字 |
| Custom | HSL 亮度 0.95 背景、原色文字 | 原色背景、浅色文字 | HSL 亮度 0.95 背景、原色边框和文字 |

主题维护规则：

- `TagVariant`、`Variant=Filled` 默认值、`TagColor` 颜色分类、ControlTheme key、template part 和伪类是稳定契约。
- `IsBordered` 不属于当前契约，不得重新引入；旧 `bordered` 和 `color="xxx-inverse"` 兼容入口不属于当前设计。
- 不把可由 AXAML 表达的模板状态迁移为 C# 动态创建视觉。
- 不在控件中增加全局 Tag 配置或 ThemeConfig 组件配置；全局默认样式由 Avalonia Style 承担。
- 不把 hover、pressed、selected、expanded、loading、filter、popup open 等运行时状态写入 Token。
- Browser 或平台特化主题必须保持同一 API 的语义一致。

Token 边界：

Tag Token 只表达组件级视觉变量，例如尺寸、间距、颜色、圆角、阴影、图标尺寸和弹层边界。Token 不承载运行时选择、展开、加载、错误、上传任务、过滤条件或业务状态。

当前 Token scope：

- `TagToken`，scope id 为 `Tag`，源码位于 `src/AtomUI.Desktop.Controls/Tag/TagToken.cs`。

## Customization Boundaries

维护 Tag 时必须保持以下不变量：

- 不擅自新增、删除、重命名或改变 public/protected API、Avalonia 属性、事件和默认值。
- 不破坏 template part、伪类、ControlTheme key、Token 名称和资源 key。
- 不改变 Gallery 已展示的 XAML 用法、默认外观、交互顺序和状态优先级。
- Template part 重新应用、集合替换、弹层关闭、窗口失活和控件 detach 时必须释放旧订阅和资源宿主。
- 不通过隐藏延迟、强制刷新或吞异常掩盖状态同步问题。
- 不引入运行时反射扫描作为 API、Token 或数据路径发现机制。
- CheckableTagGroup 的公开值始终是 option Value，不能泄漏 internal wrapper、容器或内部 SelectedItem(s)。
- Options/CheckedItems 集合替换、模板重应用和 detach 必须释放旧订阅，容器回收不能残留旧 IsChecked。
- 文档只描述当前稳定设计；历史变化记录在 `changelog.md`。

维护不变量：

维护 Tag 时不得破坏：

- Public API、`Variant=Filled` 默认值、事件顺序和 Gallery 可观察行为。
- `TagColor × Variant` 视觉矩阵、颜色清除后的状态恢复和 Light/Dark 主题响应。
- Template part 名称、ControlTheme key、伪类和资源 key。
- `IsBordered`、`bordered` 和 `color="xxx-inverse"` 不得重新成为 Tag 的兼容入口。
- CheckableTag 不继承 TagColor、Variant、IsClosable、CloseIcon 或 Closed；其二态选择由 ToggleButton.IsChecked 唯一表达。
- CheckableTagGroup 默认可取消单选；CheckedItem(s) 始终表达 option Value，internal wrapper 和 SelectionModel 不得泄漏。
- PART_CheckableTagItems 重应用、Options/CheckedItems 替换和 detach 必须释放旧订阅，容器回收必须清除旧状态。
- 旧 template part、事件订阅、Popup/Flyout/Window host 和 collection view 的释放路径。
- Light/Dark、Browser/Desktop 和不同 SizeType 下的主题一致性。
- 控件文档、源码 public surface、Token 类型或生成数据与源码契约的一致性。
