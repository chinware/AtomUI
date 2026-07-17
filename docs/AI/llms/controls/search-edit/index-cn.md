# SearchEdit

> 生成产物：由源文档生成，不要手工编辑。修改内容请回到控件文档、Gallery API / Token 表、Gallery ShowCase 或源码结构。

## 概述

SearchEdit 是 AtomUI 桌面数据录入体系中的搜索输入框，用于把单行文本输入和明确的搜索触发按钮组合成一个一体化输入控件。它继承 `LineEdit` 的文本编辑、输入表面、native validation error 投射、清除按钮、密码 reveal、内部前后缀、Form 和 CompactSpace 能力，并在右侧加入固定搜索按钮。

SearchEdit 的职责是承载搜索关键字和搜索触发事件。它不负责候选项管理、自动完成、远程请求、过滤算法、搜索结果展示或异步任务编排。需要候选项和 popup 的搜索输入时，应使用 `AutoCompleteSearchEdit`；需要普通文本输入时，应使用 `LineEdit`。

## 包与命名空间

| 项 | 值 |
| --- | --- |
| NuGet 包 | `AtomUI.Desktop.Controls` |
| .NET 命名空间 | `AtomUI.Desktop.Controls` |
| AXAML 命名空间 | `https://atomui.net` |
| Gallery 页面 | `controlgallery/AtomUIGallery/ShowCases/DataEntry/LineEdit` |
| 状态 | Stable |

## 何时使用

SearchEdit 的设计语言来自输入框与搜索 action 的一体化组合。

| 维度 | 含义 | 典型表达 |
| --- | --- | --- |
| 搜索语义 | 用户输入查询文本，并通过按钮触发搜索。 | `Text`、`PlaceholderText`、`SearchButtonClick`。 |
| 输入密度 | 控件在表单、工具栏和筛选区中的尺寸等级。 | `SizeType=Large/Middle/Small/Custom`。 |
| 输入表面 | 输入框边框和背景强度。 | `Outlined`、`Filled`、`Borderless`、`Underlined`。 |
| 搜索按钮强调度 | 搜索 action 是普通附加按钮还是主操作按钮。 | `SearchButtonStyle=Default/Primary`。 |
| 运行状态 | 搜索操作进行中，按钮显示 loading 并阻止重复点击。 | `IsOperating=true`。 |
| 输入反馈 | 搜索条件的校验或业务状态。 | `Status=Default/Error/Warning`。 |

`Custom` 尺寸不是 SearchEdit 的第四套专属 Token。它以 `Middle` 作为未显式设置时的视觉基线，并允许用户通过 `Height`、`FontSize`、`Padding` 等常规属性覆盖实际尺寸。

## 公共 API

SearchEdit 的公共 API 由继承的 `LineEdit` 文本输入 API 和搜索专项 API 组成。继承 API 的文本编辑、选择、滚动、只读、清除、reveal、Form 和 CompactSpace 语义必须保持与 LineEdit 一致。

SearchEdit 专项 API：

| API | 类型 | 语义 |
| --- | --- | --- |
| `SearchButtonStyle` | `SearchEditButtonStyle` | 搜索按钮样式，支持 `Default` 和 `Primary`。 |
| `SearchButtonText` | `string` | 搜索按钮显示文本；未设置时按钮以搜索图标为主要视觉。 |
| `IsOperating` | `bool` | 搜索按钮 loading 状态；为 `true` 时阻止重复触发 `SearchButtonClick`。 |
| `SearchButtonClick` | `RoutedEvent<RoutedEventArgs>` | 搜索按钮点击事件，由内部 `SearchButton` 点击冒泡为 SearchEdit 事件。 |

继承输入 API 的 SearchEdit 约束：

| API | 语义 |
| --- | --- |
| `Text` / `PlaceholderText` | 搜索关键字和空文本提示。 |
| `SizeType` | 输入尺寸密度，类型为 `CustomizableSizeType`。 |
| `StyleVariant` | 输入表面样式。 |
| `Status` | 手动输入反馈状态；native validation error 以 `DataValidationErrors` 为最高优先级。 |
| `IsAllowClear` / `ClearIcon` | 搜索文本清除入口。 |
| `InnerLeftContent` / `InnerRightContent` | 输入框内部前后缀内容。 |
| `LeftAddOn` / `LeftAddOnTemplate` | 输入框左侧外部附加内容。 |
| `IsMotionEnabled` | 内部按钮和输入壳体动效开关。 |

SearchEdit 的右侧外部 add-on 位置由搜索按钮占用。维护时不应把用户 `RightAddOn` 与搜索按钮混用；需要放在输入框内部右侧的内容应使用 `InnerRightContent`。

稳定 template part：

| Template Part | 类型 | 所属主题 | 职责 |
| --- | --- | --- | --- |
| `PART_AddOnDecoratedBox` | `SearchEditDecoratedBox` | `SearchEditTheme.axaml` | 搜索输入壳体、状态、Addon、CompactSpace 和搜索按钮协作入口。 |
| `PART_RightAddOn` | `SearchButton` | `SearchEditDecoratedBoxTheme.axaml` | 搜索按钮，承载图标、文字、loading、按钮样式和点击事件。 |
| `PART_ContentFrame` | `Border` | `SearchEditDecoratedBoxTheme.axaml` | 文本输入框视觉边框和背景。 |
| `PART_ScrollViewer` | `ScrollViewer` | `SearchEditTheme.axaml` | 文本滚动区域。 |
| `PART_TextPresenter` | `InputTextPresenter` | `SearchEditTheme.axaml` | 文本显示、光标、选择和密码 reveal。 |
| `Placeholder` | `TextBlock` | `SearchEditTheme.axaml` | 空文本占位提示。 |
| `PART_ClearButton` | `InputClearIconButton` | `SearchEditTheme.axaml` | 清除当前搜索文本。 |
| `PART_RevealButton` | `RevealButton` | `SearchEditTheme.axaml` | 密码 reveal 兼容入口。 |
| `InnerRightContentPresenter` | `ContentPresenter` | `SearchEditTheme.axaml` | 输入框内部右侧内容承载。 |

## 事件与命令

| `SearchButtonClick` | `RoutedEvent<RoutedEventArgs>` | 搜索按钮点击事件，由内部 `SearchButton` 点击冒泡为 SearchEdit 事件。 |
| `PART_RightAddOn` | `SearchButton` | `SearchEditDecoratedBoxTheme.axaml` | 搜索按钮，承载图标、文字、loading、按钮样式和点击事件。 |

## 使用示例

稳定示例来源于 Gallery ShowCase 和源码查看片段。生成器只输出可从 `ShowCaseItem` 追溯的示例，不维护第二套手写示例。

以下示例来自 Gallery 源码查看使用的 `ShowCaseItem` 片段，并已按中文资源规范化。

### 基础用法

来源：`controlgallery/AtomUIGallery/ShowCases/DataEntry/LineEdit/Views/LineEditBasicShowCase.axaml:13`

Gallery key：`ExamplesContent` / item `0`

```axaml
<atom:LineEdit PlaceholderText="基础用法" />
```

### 变体

来源：`controlgallery/AtomUIGallery/ShowCases/DataEntry/LineEdit/Views/LineEditBasicShowCase.axaml:32`

Gallery key：`ExamplesContent` / item `2`

```axaml
<StackPanel Orientation="Vertical" Spacing="10">
    <atom:Separator Title="普通" />
    <atom:LineEdit PlaceholderText="线框风格" StyleVariant="Outlined" />
    <atom:LineEdit PlaceholderText="填充风格" StyleVariant="Filled" />
    <atom:LineEdit PlaceholderText="无边框" StyleVariant="Borderless" />
    <atom:LineEdit PlaceholderText="下划线" StyleVariant="Underlined" />
    <atom:SearchEdit PlaceholderText="填充风格" StyleVariant="Filled" />
    <atom:Separator Title="左侧附加和右侧附加" />
    <atom:LineEdit LeftAddOn="http://" RightAddOn=".com" Text="我的站点" />
    <atom:LineEdit LeftAddOn="http://" RightAddOn=".com" Text="我的站点" StyleVariant="Filled" />
    <atom:LineEdit LeftAddOn="http://" RightAddOn=".com" Text="我的站点" StyleVariant="Borderless" />
    <atom:LineEdit LeftAddOn="http://" RightAddOn=".com" Text="我的站点" StyleVariant="Underlined" />
</StackPanel>
```

### 带清除图标

来源：`controlgallery/AtomUIGallery/ShowCases/DataEntry/LineEdit/Views/LineEditBasicShowCase.axaml:64`

Gallery key：`ExamplesContent` / item `4`

```axaml
<StackPanel Orientation="Vertical" Spacing="10">
    <atom:LineEdit PlaceholderText="带清除图标的输入框" IsAllowClear="True" />
    <atom:TextArea PlaceholderText="带清除图标的文本域" IsAllowClear="True" />
</StackPanel>
```

### 密码框

来源：`controlgallery/AtomUIGallery/ShowCases/DataEntry/LineEdit/Views/LineEditBasicShowCase.axaml:73`

Gallery key：`ExamplesContent` / item `5`

```axaml
<StackPanel Orientation="Vertical" Spacing="10">
    <atom:LineEdit PlaceholderText="输入密码"
                   Width="400"
                   RevealPassword="False"
                   PasswordChar="•"
                   HorizontalAlignment="Left"
                   IsEnableRevealButton="True" />
    <atom:LineEdit PlaceholderText="输入密码"
                   Width="400"
                   RevealPassword="False"
                   HorizontalAlignment="Left"
                   PasswordChar="•"
                   IsEnableRevealButton="True"
                   IsAllowClear="True" />
</StackPanel>
```

## 状态模型

SearchEdit 的文本输入行为继承 LineEdit，搜索按钮行为独立建模：

```text
SearchButton.Click
  ↓
SearchEditDecoratedBox.HandleSearchButtonClick
  ↓
SearchEdit.NotifySearchButtonClicked()
  ↓
if !IsOperating raise SearchButtonClick
```

`IsOperating=true` 只表示搜索按钮处于操作中状态。它不改变 `Text`、不自动禁用文本编辑、不管理异步任务，也不清空搜索结果。业务层负责在搜索开始和结束时设置该属性。

状态优先级：

```text
Disabled
> Operating button loading
> Error / Warning
> Focus
> PointerOver / Pressed
> Normal
```

`IsEnabled=false` 会传递给搜索按钮，使输入壳体和按钮一起进入 disabled 视觉。native validation error 通过 `DataValidationErrors` 优先影响输入框边框、文本前景和搜索按钮状态色；`Status=Warning` 继续表达 AtomUI warning 视觉，显式 `Status=Error` 只作为无 native error 时的手动错误视觉请求。`SearchButtonStyle` 只控制按钮强调度，不改变文本编辑、清除、Form 或搜索事件语义。

## 主题与 Design Token

SearchEdit 使用三层主题协作：

| 主题 | 职责 |
| --- | --- |
| `SearchEditTheme.axaml` | SearchEdit 根模板、文本 presenter、placeholder、clear/reveal/inner-right 内容、focus/status 视觉。 |
| `SearchEditDecoratedBoxTheme.axaml` | 输入框内容边框、搜索按钮、左右布局、搜索按钮 style 和 z-index 关系。 |
| `SearchButtonTheme.axaml` | 搜索按钮在不同输入表面中的背景、前景和状态色。 |

SearchEdit 不定义独立组件 Token。主题资源来源：

| Token 来源 | 用途 |
| --- | --- |
| `LineEditToken` | 输入文本字号。 |
| `AddOnDecoratedBoxToken` | 输入壳体 focus shadow、active 背景和状态视觉。 |
| `ButtonToken` | 搜索按钮字号、padding、图标和按钮状态。 |
| `SharedToken` | 控件高度、字体高度、边框、颜色、间距、motion 和 disabled 语义。 |

搜索按钮必须与输入框视觉上组成单一控件。Custom 高度下，搜索按钮的可视 `Frame` 高度必须跟随 `SearchEditDecoratedBox` 的实际布局高度，避免按钮边框和输入框边框错位。

Token 来源：

- SearchEdit 当前没有专属 `token.md`；LLMS 生成按第 5 节视觉与主题模型、SharedToken、控件家族 Token 或主题资源说明 Token 边界。

## AOT 与裁剪注意事项

SearchEdit 不依赖运行时反射发现模板结构。跨模板协作使用固定 template part、`TemplateBinding`、selector 和 owner 引用完成。

资源和生命周期边界：

- 搜索按钮 click 订阅必须在重新套用模板时解绑旧实例。
- `OwningSearchEdit` 只保存当前模板 owner，不创建全局订阅。
- 搜索按钮高度同步使用 XAML binding，不在布局过程中写本地 `Height` 值。
- 搜索按钮状态不创建异步任务；业务异步状态由外部设置 `IsOperating`。
- SearchEdit 没有独立 Token scope，不应为运行时状态新增 Token。

AOT 边界：

- SearchEdit 主题通过显式 ResourceInclude 注册。
- `AutoCompleteSearchEditBox` 使用显式 `StyleKeyOverride` 复用 SearchEdit 主题。
- Gallery API / Token 表使用显式 view model 数据，不依赖运行时反射扫描 SearchEdit 成员。

## 源码索引

主要源码：

- `src/AtomUI.Desktop.Controls/Input/SearchEdit.cs`：public API、默认 clear icon、模板接入和搜索点击事件抛出。
- `src/AtomUI.Desktop.Controls/Input/SearchEditDecoratedBox.cs`：内部输入壳体，转接 SearchEdit 属性并订阅搜索按钮 click。
- `src/AtomUI.Desktop.Controls/Input/SearchButton.cs`：搜索按钮类型，继承 Button 并接入输入 `StyleVariant`、`Status`。
- `src/AtomUI.Desktop.Controls/Input/SearchEditPanel.cs`：搜索输入布局面板，负责左侧 AddOn、搜索按钮和内容框重叠边框排布。
- `src/AtomUI.Desktop.Controls/Input/Themes/SearchEditTheme.axaml`：SearchEdit 根模板、文本区域、内部 action 和 focus/status selector。
- `src/AtomUI.Desktop.Controls/Input/Themes/SearchEditDecoratedBoxTheme.axaml`：搜索按钮和输入框一体化模板。
- `src/AtomUI.Desktop.Controls/Input/Themes/SearchButtonTheme.axaml`：搜索按钮在不同输入表面下的状态视觉。
- `src/AtomUI.Desktop.Controls/AutoComplete/AutoCompleteSearchEdit.cs`：AutoComplete 搜索输入入口，复用 SearchEdit 搜索按钮属性。
- `src/AtomUI.Desktop.Controls/AutoComplete/AutoCompleteSearchEditBox.cs`：AutoComplete 内部 SearchEdit box，使用 SearchEdit style key。

## 相关文档

- 源设计文档：`docs/controls/desktop/data-entry/search-edit/overview.md`
- 实现文档：`docs/controls/desktop/data-entry/search-edit/implementation.md`
- 变更记录：`docs/controls/desktop/data-entry/search-edit/changelog.md`
- 语义结构：`./semantic-cn.md`
