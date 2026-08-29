# ComboBox

> 生成产物：由源文档生成，不要手工编辑。修改内容请回到控件文档、源码 public surface、Token 类型或生成数据、Gallery ShowCase 或源码结构。

## 概述

ComboBox 是 AtomUI 桌面控件体系中的组合框控件，用于在可输入文本框和候选列表之间完成选择。

ComboBox 不负责远程自动完成、树形选择或多列数据选择。这些职责应由业务层、组合控件或更专用的 AtomUI 控件承担。

主要源码入口：

- `src/AtomUI.Desktop.Controls/ComboBox`

## 包与命名空间

| 项 | 值 |
| --- | --- |
| NuGet 包 | `AtomUI.Desktop.Controls` |
| .NET 命名空间 | `AtomUI.Desktop.Controls` |
| AXAML 命名空间 | `https://atomui.net` |
| Gallery 页面 | `controlgallery/AtomUIGallery/ShowCases/Navigation/ComboBox` |
| 状态 | Stable |

## 何时使用

ComboBox 的设计语言围绕控件职责、可观察状态和主题契约组织，而不是围绕模板节点组织。

| 维度 | 含义 | ComboBox 中的表达 |
| --- | --- | --- |
| 产品语义 | 控件在界面中承担的稳定职责。 | ComboBox 是 AtomUI 桌面控件体系中的组合框控件，用于在可输入文本框和候选列表之间完成选择。 |
| 内容承载 | 用户数据、展示内容、集合项或操作入口如何进入控件。 | `ContentLeftAddOn`、`ContentLeftAddOnTemplate`、`ContentRightAddOn`、`ContentRightAddOnTemplate`、`FilterValue`、`FilterValueSelector`、`LeftAddOnTemplate`、`OptionFontSize` 等 9 项。 |
| 状态反馈 | public API、内部状态和伪类如何形成用户可感知反馈。 | open/close、collection/filter、input/value、motion、visual option。 |
| 主题语义 | ControlTheme、SharedToken、控件 Token 和模板绑定如何表达视觉。 | ComboBox Token + ControlTheme。 |

## 公共 API

ComboBox 的公共契约由 public/protected 类型成员、Avalonia 属性、事件、命令、template part、伪类、ControlTheme key 和资源 key 共同组成。维护时应先确认这些契约是否已经被源码、Gallery 示例或文档暴露。

核心 public surface 按语义分组维护：

| 契约组 | 代表成员 | 维护含义 |
| --- | --- | --- |
| 内容与数据 | `ContentLeftAddOn`、`ContentLeftAddOnTemplate`、`ContentRightAddOn`、`ContentRightAddOnTemplate`、`FilterValue`、`FilterValueSelector`、`LeftAddOnTemplate`、`OptionFontSize`、`RightAddOnTemplate` | 定义控件展示内容、输入数据、模板或业务对象入口。 |
| 选择与集合 | `SelectedItem`、`SelectedIndex`、`DropDownDisplayPageSize`、`Filter`、`IsFilterEnabled` | 维护选择、展开、过滤、分页、分组或集合状态。 |
| 交互与状态 | `IsAllowClear`、`IsMotionEnabled`、`ShouldUseOverlayPopup`、`Status`、`IsShowOverflowTip`、`OverflowTipDelay`、`OverflowTipPlacement` | 表达用户可观察状态、可用性、清除、加载、反馈和非编辑态选中内容溢出提示语义。Form 校验扩展状态进入内部 `FormStatus`，不覆盖显式 `Status`。 |
| 视觉与布局 | `SizeType`、`StyleVariant` | 影响尺寸、位置、颜色、形状、密度和模板视觉变量。 |
| 其他稳定入口 | `LeftAddOn`、`RightAddOn` | 保留为 public surface，变更前需确认 Gallery 和用户 XAML 依赖。 |

当前没有抽取到控件专属 public 事件；交互通知主要来自继承事件、命令或 Gallery 可观察状态。

主要公开类型与枚举：

- 类型：`ComboBox`、`ComboBoxHandle`、`ComboBoxItem`。
- 枚举：无。

稳定 template part：

| Template Part | 类型 | 职责 |
| --- | --- | --- |
| `PART_ComboBoxHandle` | `?` | 稳定模板协作入口，重命名前必须同步主题和实现。 |
| `PART_ContentRightAddOnPresenter` | `?` | 展示用户内容、文本、图标或模板化数据。 |
| `PART_EditableTextBox` | `?` | 承载文本输入、过滤、显示或编辑入口。 |
| `PART_EmptyIndicator` | `?` | 展示指示器、进度、分页或状态反馈。 |
| `PART_FormFeedBack` | `?` | 稳定模板协作入口，重命名前必须同步主题和实现。 |
| `PART_ItemsPresenter` | `?` | 展示用户内容、文本、图标或模板化数据。 |
| `PART_OpenIndicatorButton` | `?` | 承载用户触发入口、导航或关闭动作。 |
| `PART_Popup` | `?` | 承载弹层宿主、打开关闭或候选内容。 |
| `PART_TextPresenter` | `?` | 展示用户内容、文本、图标或模板化数据。 |

当前未抽取到控件专属伪类；主题主要依赖 Avalonia 标准伪类、模板绑定和内部 StyledProperty。

## 事件与命令

ComboBox 的公共契约由 public/protected 类型成员、Avalonia 属性、事件、命令、template part、伪类、ControlTheme key 和资源 key 共同组成。维护时应先确认这些契约是否已经被源码、Gallery 示例或文档暴露。
当前没有抽取到控件专属 public 事件；交互通知主要来自继承事件、命令或 Gallery 可观察状态。

## 使用示例

稳定示例来源于 Gallery ShowCase 和源码查看片段。生成器只输出可从 `ShowCaseItem` 追溯的示例，不维护第二套手写示例。

以下示例来自 Gallery 源码查看使用的 `ShowCaseItem` 片段，并已按中文资源规范化。

### 基础用法

来源：`controlgallery/AtomUIGallery/ShowCases/Navigation/ComboBox/Views/ComboBoxShowCase.axaml:37`

Gallery key：`ExamplesContent` / item `0`

```axaml
<atom:ComboBox PlaceholderText="请选择" Width="300">
    <atom:ComboBoxItem Content="床前明月光" />
    <atom:ComboBoxItem Content="疑是地上霜" />
    <atom:ComboBoxItem Content="举头望明月" />
    <atom:ComboBoxItem Content="低头思故乡" />
    <atom:ComboBoxItem Content="床前明月光" />
    <atom:ComboBoxItem Content="疑是地上霜" />
    <atom:ComboBoxItem Content="举头望明月" />
    <atom:ComboBoxItem Content="低头思故乡" />
    <atom:ComboBoxItem Content="床前明月光" />
    <atom:ComboBoxItem Content="疑是地上霜" />
    <atom:ComboBoxItem Content="举头望明月" />
    <atom:ComboBoxItem Content="低头思故乡" />
    <atom:ComboBoxItem Content="床前明月光" />
    <atom:ComboBoxItem Content="疑是地上霜" />
    <atom:ComboBoxItem Content="举头望明月" />
    <atom:ComboBoxItem Content="低头思故乡" />
    <atom:ComboBoxItem Content="床前明月光" />
    <atom:ComboBoxItem Content="疑是地上霜" />
    <atom:ComboBoxItem Content="举头望明月" />
    <atom:ComboBoxItem Content="低头思故乡" />
    <atom:ComboBoxItem Content="床前明月光" />
    <atom:ComboBoxItem Content="疑是地上霜" />
    <atom:ComboBoxItem Content="举头望明月" />
    <atom:ComboBoxItem Content="低头思故乡" />
    <atom:ComboBoxItem Content="床前明月光" />
    <atom:ComboBoxItem Content="疑是地上霜" />
    <atom:ComboBoxItem Content="举头望明月" />
    <atom:ComboBoxItem Content="低头思故乡" />
    <atom:ComboBoxItem Content="床前明月光" />
    <atom:ComboBoxItem Content="疑是地上霜" />
    <atom:ComboBoxItem Content="举头望明月" />
    <atom:ComboBoxItem Content="低头思故乡" />
    <atom:ComboBoxItem Content="床前明月光" />
    <atom:ComboBoxItem Content="疑是地上霜" />
    <atom:ComboBoxItem Content="举头望明月" />
    <atom:ComboBoxItem Content="低头思故乡" />
    <atom:ComboBoxItem Content="床前明月光" />
    <atom:ComboBoxItem Content="疑是地上霜" />
    <atom:ComboBoxItem Content="举头望明月" />
    <atom:ComboBoxItem Content="低头思故乡" />
</atom:ComboBox>
```

### 通过 ItemsSource 生成 ComboBoxItem

来源：`controlgallery/AtomUIGallery/ShowCases/Navigation/ComboBox/Views/ComboBoxShowCase.axaml:89`

Gallery key：`ExamplesContent` / item `1`

```axaml
<atom:ComboBox Name="TplComboBox"
               PlaceholderText="请选择" Width="300"
               ItemsSource="{Binding ComboBoxItems}">
    <atom:ComboBox.ItemTemplate>
        <DataTemplate>
            <atom:TextBlock Text="{Binding Text}" VerticalAlignment="Center"/>
        </DataTemplate>
    </atom:ComboBox.ItemTemplate>
</atom:ComboBox>
```

### SelectedItem 绑定

来源：`controlgallery/AtomUIGallery/ShowCases/Navigation/ComboBox/Views/ComboBoxShowCase.axaml:109`

Gallery key：`ExamplesContent` / item `2`

```axaml
<StackPanel Spacing="8" MinWidth="320">
    <TextBlock Text="SelectedItem"
               FontWeight="SemiBold" />
    <atom:ComboBox PlaceholderText="请选择"
                   Width="300"
                   IsAllowClear="True"
                   ItemsSource="{Binding ComboBoxItems}"
                   SelectedItem="{Binding BoundSelectedItem}">
        <atom:ComboBox.ItemTemplate>
            <DataTemplate>
                <atom:TextBlock Text="{Binding Text}" VerticalAlignment="Center" />
            </DataTemplate>
        </atom:ComboBox.ItemTemplate>
    </atom:ComboBox>
    <WrapPanel ItemSpacing="8">
        <atom:Button SizeType="Small"
                     Command="{Binding SetBoundSelectedItemCommand}"
                     Content="设为第三句" />
        <atom:Button SizeType="Small"
                     Command="{Binding ClearBoundSelectedItemCommand}"
                     Content="清空" />
    </WrapPanel>
    <TextBlock Text="ViewModel 值" />
    <TextBlock Text="{Binding BoundSelectedItemText}" />
</StackPanel>
```

### 可编辑过滤

来源：`controlgallery/AtomUIGallery/ShowCases/Navigation/ComboBox/Views/ComboBoxShowCase.axaml:144`

Gallery key：`ExamplesContent` / item `3`

```axaml
<atom:ComboBox PlaceholderText="输入内容过滤"
               Width="300"
               IsEditable="True"
               IsFilterEnabled="True">
    <atom:ComboBoxItem Content="Alpha" />
    <atom:ComboBoxItem Content="Alpine" />
    <atom:ComboBoxItem Content="Beta" />
    <atom:ComboBoxItem Content="Gamma" />
    <atom:ComboBoxItem Content="Delta" />
</atom:ComboBox>
```

## 状态模型

ComboBox 的状态流按以下路径收敛：

```text
Public API / inherited command / item source / user input
  -> 控件实例状态
  -> effective state / pseudo-class / template property
  -> ControlTheme selector / presenter / renderer
  -> Gallery 可观察行为
```

状态维护规则：

- Disabled 或不可交互状态优先屏蔽 pointer、keyboard、motion 和提交类反馈。
- `DataValidationErrors` 是 native error 唯一真源；`FormStatus` 承载 Form 的 warning、success、validating 和 error 投影，`Status` 只表示用户显式请求。输入表面通过 `InputControlState.ResolveEffectiveStatus` 计算有效状态，native/Form error 优先于 Form warning，再优先于显式 warning/error。
- open/close、collection/filter、input/value、motion、visual option 状态由控件实例或明确的数据 owner 推导，不能在 template part 之间双向竞争。
- 单选结果以继承的 `SelectedItem` / `SelectedIndex` 为准；AtomUI Form 集成使用 `SelectedItem` 作为 ComboBox 的默认表单值，`SetFormValue`、`GetFormValue` 和 `ClearFormValue` 不应转换为字符串或读取展示文本。
- 下拉项的 active candidate 与 `SelectedItem` / `SelectedIndex` 分离。鼠标移动和键盘 `Up` / `Down` 共享同一个 active candidate；迁移只改变候选视觉，`Enter` 才提交 `SelectedItem`。`:pointerover` 不得形成第二个候选高亮，selected 视觉优先于 active 视觉。
- 模板重套用时必须把 public API 对应状态回放到新的 part、伪类和主题变量。
- 集合、弹层、异步、动效或窗口相关状态必须能处理 reset、close、cancel、detach 和 owner 释放。

## 主题与 Design Token

ComboBox 的视觉模型由控件模板、ControlTheme、SharedToken 和必要的控件 Token 共同构成。

| 主题文件 | 职责 |
| --- | --- |
| `ComboBoxHandleTheme.axaml` | 定义局部操作入口、按钮或 handle 的状态视觉。 |
| `ComboBoxItemTheme.axaml` | 定义集合项、容器项或局部单元的状态视觉。 |
| `ComboBoxTheme.axaml` | 定义布局、内容承载或框架节点视觉。 |

非编辑态选中内容的完整文本提示复用共享 `OverflowTip` attached behavior。模板只在 `SelectedContentPresenter` 上接入 `IsShowOverflowTip`、`OverflowTipDelay`、`OverflowTipPlacement` 和 `SelectionBoxItem`；`IsEditable=true` 时编辑输入框不默认启用该提示。

ComboBox 使用 `ComboBoxToken` 作为控件 Token scope。Token 只表达组件视觉语义，不承载 open/close、collection/filter、input/value、motion、visual option 运行时状态。

主题维护规则：

- 不删除或重命名已经稳定的 ControlTheme key、template part、伪类和资源 key。
- 不把可由 AXAML 表达的模板状态迁移为 C# 动态创建视觉。
- 不把 hover、pressed、selected、expanded、loading、filter、popup open 等运行时状态写入 Token。
- Browser 或平台特化主题必须保持同一 API 的语义一致。

Token 来源：

ComboBox Token 只表达组件级视觉变量，例如尺寸、间距、颜色、圆角、阴影、图标尺寸和弹层边界。Token 不承载运行时选择、展开、加载、错误、上传任务、过滤条件或业务状态。

当前 Token scope：

- `ComboBoxToken`，scope id 为 `ComboBox`，源码位于 `src/AtomUI.Desktop.Controls/ComboBox/ComboBoxToken.cs`。

## AOT 与裁剪注意事项

资源和 AOT 约束：

- 不通过运行时反射扫描 public API、Token 或 Gallery 示例数据。
- 不把可静态声明的模板结构迁移到 C# 动态创建。
- 异步加载、上传、弹层和窗口生命周期必须能取消或释放。
- 缓存对象必须与控件、窗口、弹层或数据 owner 生命周期一致。
- Source generator 生成文件不手工编辑；需要修改时改输入源或 generator。

性能边界：

- 控件应优先复用 Avalonia 原生虚拟化、模板绑定和资源系统。
- 避免为每次状态变化创建不必要的视觉对象、订阅或动画对象。
- 大集合控件必须保证 container recycle 后不会泄漏旧 item 状态。

## 源码索引

主要源码文件：

- `src/AtomUI.Desktop.Controls/ComboBox/ComboBox.cs`
- `src/AtomUI.Desktop.Controls/ComboBox/ComboBoxHandle.cs`
- `src/AtomUI.Desktop.Controls/ComboBox/ComboBoxItem.cs`
- `src/AtomUI.Desktop.Controls/ComboBox/ComboBoxReflectionExtensions.cs`
- `src/AtomUI.Desktop.Controls/ComboBox/ComboBoxToken.cs`
- `src/AtomUI.Desktop.Controls/Tooltip/OverflowTip.cs`
- `src/AtomUI.Desktop.Controls/ComboBox/Themes/ComboBoxHandleTheme.axaml`
- `src/AtomUI.Desktop.Controls/ComboBox/Themes/ComboBoxItemTheme.axaml`
- `src/AtomUI.Desktop.Controls/ComboBox/Themes/ComboBoxTheme.axaml`

职责边界：

- 控件主文件保留 public/protected API、Avalonia 属性注册、事件和主要生命周期入口。
- Theme 文件负责静态视觉结构、template part、selector 和资源绑定。
- Token 文件只提供组件视觉变量，不保存实例状态。
- Gallery 文件只展示用法和示例，不作为运行时逻辑 owner。

## 相关文档

- 源设计文档：`docs/controls/desktop/navigation/combo-box/overview.md`
- 实现文档：`docs/controls/desktop/navigation/combo-box/implementation.md`
- Token 文档：`docs/controls/desktop/navigation/combo-box/token.md`
- 变更记录：`docs/controls/desktop/navigation/combo-box/changelog.md`
- 语义结构：`./semantic-cn.md`
