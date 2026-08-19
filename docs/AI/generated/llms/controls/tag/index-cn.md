# Tag

> 生成产物：由源文档生成，不要手工编辑。修改内容请回到控件文档、源码 public surface、Token 类型或生成数据、Gallery ShowCase 或源码结构。

## 概述

Tag 家族覆盖展示标签与选择标签。`Tag` 用于展示状态、分类、可关闭标记和预设色语义；`CheckableTag` 用标签视觉表达二态选择；`CheckableTagGroup` 在有限选项集合上提供可取消单选和多选。普通 Tag 的颜色模型由 `TagColor` 和 `TagVariant` 两个正交维度组成，按照当前三种变体规范的 `Color × Variant` 语义生成最终视觉。

Tag 不负责复杂筛选器、按钮或徽标计数。这些职责应由业务层、组合控件或更专用的 AtomUI 控件承担。

主要源码入口：

- `src/AtomUI.Controls/Tag`
- `src/AtomUI.Desktop.Controls/Tag`

## 包与命名空间

| 项 | 值 |
| --- | --- |
| NuGet 包 | `AtomUI.Desktop.Controls` |
| .NET 命名空间 | `AtomUI.Desktop.Controls` |
| AXAML 命名空间 | `https://atomui.net` |
| Gallery 页面 | `controlgallery/AtomUIGallery/ShowCases/DataDisplay/Tag` |
| 状态 | Stable |

## 何时使用

Tag 的设计语言围绕控件职责、可观察状态和主题契约组织，而不是围绕模板节点组织。

| 维度 | 含义 | Tag 中的表达 |
| --- | --- | --- |
| 产品语义 | 控件在界面中承担的稳定职责。 | Tag 表达展示语义，CheckableTag 表达二态选择，CheckableTagGroup 表达一组选项的单选或多选。 |
| 内容承载 | 用户数据、展示内容、集合项或操作入口如何进入控件。 | Tag 使用 `Text`/`Icon`；CheckableTag 使用 `Content`/`Icon`；Group 使用 `Options` 和 `ItemTemplate`。 |
| 状态反馈 | public API、内部状态和伪类如何形成用户可感知反馈。 | Tag 使用颜色分类和 `Variant`；CheckableTag 使用 `IsChecked`；Group 使用 `CheckedItem(s)`。 |
| 主题语义 | ControlTheme、SharedToken、控件 Token 和模板绑定如何表达视觉。 | Tag Token + ControlTheme。 |

## 公共 API

Tag 的公共契约由 public/protected 类型成员、Avalonia 属性、事件、命令、template part、伪类、ControlTheme key 和资源 key 共同组成。维护时应先确认这些契约是否已经被源码、Gallery 示例或文档暴露。

核心 public surface 按语义分组维护：

| 契约组 | 代表成员 | 维护含义 |
| --- | --- | --- |
| 内容与数据 | `CloseIcon`、`Icon`、`Text` | 定义控件展示内容、输入数据、模板或业务对象入口。 |
| 交互与状态 | `IsClosable`、`Variant` | 表达关闭能力和 Filled/Solid/Outlined 视觉形态。 |
| 视觉与布局 | `TagColor` | 选择 Default、Preset、Status 或 Custom 颜色类别。 |

稳定事件包括 `Closed`。事件触发顺序属于兼容契约，不能因内部状态重排而改变。

主要公开类型与枚举：

- 类型：`AbstractTag`、`Tag`。
- 枚举：`TagStatus`、`TagVariant`。

`Variant` 的默认值为 `TagVariant.Filled`。Tag 不提供全局 Variant 配置，也不把 Variant 写入 `ThemeConfig`；应用需要统一样式时使用 Avalonia Style，控件实例的本地值优先于样式值。

选择标签的 public surface 按以下边界维护：

| 控件 | 主要契约 | 默认与状态 owner |
| --- | --- | --- |
| `CheckableTag` | `IsChecked`、`IsEnabled`、`Content`、`ContentTemplate`、`Command`、`Icon`、`IsMotionEnabled` | 复用 ToggleButton 二态状态；不继承普通 Tag 的颜色、Variant 或关闭 API。 |
| `CheckableTagGroup` | `Options`、`IsMultiple`、`CheckedItem`、`CheckedItems`、Default 值、`ItemTemplate`、布局属性和 `CheckedChanged` | 默认可取消单选；Group 是公开选择值的唯一 owner。 |
| `ICheckableTagOption` / `CheckableTagOption` | `Value`、`Content` | Value 非空且在同一 Group 中唯一。 |

Group 的 `CheckedItem` 和 `CheckedItems` 默认支持 TwoWay binding；前者只在单选模式生效，后者只在多选模式生效。完整模式转换、Default 初始化和事件契约见选择模型专项文档。

稳定 template part：

| Template Part | 类型 | 职责 |
| --- | --- | --- |
| `PART_CloseButton` | `AbstractIconButton` | 承载用户触发入口、导航或关闭动作。 |

控件专属或内部伪类包括 `CustomColor=:custom-color`、`PresetColor=:preset-color`、`StatusColor=:status-color`、`TagPseudoClass.CustomColor`、`TagPseudoClass.PresetColor`、`TagPseudoClass.StatusColor`。这些伪类属于主题 selector 可观察契约，不能在未同步主题和 Gallery 的情况下重命名或删除。

## 事件与命令

Tag 的公共契约由 public/protected 类型成员、Avalonia 属性、事件、命令、template part、伪类、ControlTheme key 和资源 key 共同组成。维护时应先确认这些契约是否已经被源码、Gallery 示例或文档暴露。
稳定事件包括 `Closed`。事件触发顺序属于兼容契约，不能因内部状态重排而改变。
Group 的 `CheckedItem` 和 `CheckedItems` 默认支持 TwoWay binding；前者只在单选模式生效，后者只在多选模式生效。完整模式转换、Default 初始化和事件契约见选择模型专项文档。

## 使用示例

稳定示例来源于 Gallery ShowCase 和源码查看片段。生成器只输出可从 `ShowCaseItem` 追溯的示例，不维护第二套手写示例。

以下示例来自 Gallery 源码查看使用的 `ShowCaseItem` 片段，并已按中文资源规范化。

### 可选择标签

来源：`controlgallery/AtomUIGallery/ShowCases/DataDisplay/Tag/Views/TagShowCase.axaml:113`

Gallery key：`ExamplesContent` / item `1`

```axaml
<atom:Form LabelColInfo="6*"
           WrapperColInfo="18*"
           MaxWidth="560"
           HorizontalAlignment="Stretch">
    <atom:FormItem LabelText="可选择">
        <atom:CheckableTag Content="是"
                           IsChecked="{Binding IsCheckableTagChecked, Mode=TwoWay}" />
    </atom:FormItem>
    <atom:FormItem LabelText="单选">
        <atom:CheckableTagGroup Options="{Binding CheckableTagOptions}"
                                CheckedItem="{Binding SingleCheckedTag, Mode=TwoWay}" />
    </atom:FormItem>
    <atom:FormItem LabelText="多选">
        <atom:CheckableTagGroup IsMultiple="True"
                                Options="{Binding CheckableTagOptions}"
                                CheckedItems="{Binding MultipleCheckedTags, Mode=TwoWay}" />
    </atom:FormItem>
</atom:Form>
```

### 形态变体

来源：`controlgallery/AtomUIGallery/ShowCases/DataDisplay/Tag/Views/TagShowCase.axaml:357`

Gallery key：`ExamplesContent` / item `5`

```axaml
<StackPanel Orientation="Vertical">
    <atom:TextBlock FontWeight="Bold" FontSize="14" Margin="0, 0, 0, 10" Text="默认颜色" />
    <WrapPanel HorizontalAlignment="Left">
        <atom:Tag Variant="Filled" Text="浅色填充" />
        <atom:Tag Variant="Solid" Text="实色填充" />
        <atom:Tag Variant="Outlined" Text="描边" />
    </WrapPanel>

    <atom:TextBlock FontWeight="Bold" FontSize="14" Margin="0, 20, 0, 10" Text="预设颜色" />
    <WrapPanel HorizontalAlignment="Left">
        <atom:Tag TagColor="blue" Variant="Filled" Text="蓝色" />
        <atom:Tag TagColor="blue" Variant="Solid" Text="蓝色" />
        <atom:Tag TagColor="blue" Variant="Outlined" Text="蓝色" />
    </WrapPanel>

    <atom:TextBlock FontWeight="Bold" FontSize="14" Margin="0, 20, 0, 10" Text="状态颜色" />
    <WrapPanel HorizontalAlignment="Left">
        <atom:Tag TagColor="success" Variant="Filled" Text="成功" />
        <atom:Tag TagColor="success" Variant="Solid" Text="成功" />
        <atom:Tag TagColor="success" Variant="Outlined" Text="成功" />
    </WrapPanel>

    <atom:TextBlock FontWeight="Bold" FontSize="14" Margin="0, 20, 0, 10" Text="自定义颜色" />
    <WrapPanel HorizontalAlignment="Left">
        <atom:Tag TagColor="#1677ff" Variant="Filled" Text="#1677ff" />
        <atom:Tag TagColor="#1677ff" Variant="Solid" Text="#1677ff" />
        <atom:Tag TagColor="#1677ff" Variant="Outlined" Text="#1677ff" />
    </WrapPanel>
</StackPanel>
```

## 状态模型

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

## 主题与 Design Token

Tag 的视觉模型由控件模板、ControlTheme、SharedToken 和必要的控件 Token 共同构成。

| 主题文件 | 职责 |
| --- | --- |
| `TagTheme.axaml` | 提供控件模板、selector、资源绑定和状态视觉。 |
| `CheckableTagTheme.axaml` | 提供二态标签的内容结构以及 checked、focus、disabled 等状态视觉。 |
| `CheckableTagGroupTheme.axaml` | 组合内部选择控件、ItemsPresenter 和 WrapPanel。 |

Tag 使用 `TagToken` 作为控件 Token scope。Token 只表达组件视觉语义，不承载 input/value、visual option 运行时状态。

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

Token 来源：

Tag Token 只表达组件级视觉变量，例如尺寸、间距、颜色、圆角、阴影、图标尺寸和弹层边界。Token 不承载运行时选择、展开、加载、错误、上传任务、过滤条件或业务状态。

当前 Token scope：

- `TagToken`，scope id 为 `Tag`，源码位于 `src/AtomUI.Desktop.Controls/Tag/TagToken.cs`。

## AOT 与裁剪注意事项

资源和 AOT 约束：

- 不通过运行时反射扫描 public API、Token 或 Gallery 示例数据。
- 不把可静态声明的模板结构迁移到 C# 动态创建。
- 异步加载、上传、弹层和窗口生命周期必须能取消或释放。
- 缓存对象必须与控件、窗口、弹层或数据 owner 生命周期一致。
- Source generator 生成文件不手工编辑；需要修改时改输入源或 generator。
- CheckableTagGroup 的 option 解析和属性同步使用静态类型、AvaloniaProperty 和强类型 getter，不使用 ReflectionBinding、字符串 path 或运行时类型扫描。

性能边界：

- 控件应优先复用 Avalonia 原生虚拟化、模板绑定和资源系统。
- 避免为每次状态变化创建不必要的视觉对象、订阅或动画对象。
- 大集合控件必须保证 container recycle 后不会泄漏旧 item 状态。
- Group 只在 Options、模式或公开选择值变化时执行 O(N) 归一和同步；单项交互不能通过遍历 VisualTree 查找业务值。
- 外部 CheckedItems 必须复制为内部快照，不能直接作为 internal SelectedItems 持有或原地修改。

## 源码索引

主要源码文件：

- `src/AtomUI.Controls/Tag/AbstractTag.cs`
- `src/AtomUI.Controls/Tag/TagEnums.cs`
- `src/AtomUI.Controls/Tag/TagPseudoClass.cs`
- `src/AtomUI.Desktop.Controls/Tag/Tag.cs`
- `src/AtomUI.Desktop.Controls/Tag/TagToken.cs`
- `src/AtomUI.Desktop.Controls/Tag/Themes/TagTheme.axaml`
- `src/AtomUI.Desktop.Controls/Tag/Themes/TagTheme.cs`

职责边界：

- 控件主文件保留 public/protected API、Avalonia 属性注册、事件和主要生命周期入口。
- Theme 文件负责静态视觉结构、template part、selector 和资源绑定。
- Token 文件只提供组件视觉变量，不保存实例状态。
- Gallery 文件只展示用法和示例，不作为运行时逻辑 owner。

## 相关文档

- 源设计文档：`docs/controls/desktop/data-display/tag/overview.md`
- 实现文档：`docs/controls/desktop/data-display/tag/implementation.md`
- Semantic Part 文档：`docs/controls/desktop/data-display/tag/semantic-part.md`
- Token 文档：`docs/controls/desktop/data-display/tag/token.md`
- 变更记录：`docs/controls/desktop/data-display/tag/changelog.md`
- 语义结构：`./semantic-cn.md`
