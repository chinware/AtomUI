# ListBox

> 生成产物：由源文档生成，不要手工编辑。修改内容请回到控件文档、Gallery API / Token 表、Gallery ShowCase 或源码结构。

## 概述

ListBox 是桌面端数据展示类轻量列表选择控件，用于展示一组纵向排列的简单选项、候选项、搜索结果或可点击条目。它以 Avalonia `ListBox` 为基础，扩展 AtomUI 的尺寸、选中指示器、空状态、文本过滤高亮、Token 和 CandidateList 复用能力。

ListBox 的职责是管理列表容器生成、单选或多选、键盘导航、条目点击、空状态展示、文本过滤展示和虚拟化容器状态回放。它不负责分组、排序、分页、复杂数据视图刷新、远程搜索请求、树形层级、表格列模型或业务命令编排；这些场景应使用 `ListView`、TreeView、DataGrid 或组合控件。

ListBox 支持两种条目提供方式：

- 直接放置 `ListBoxItem`。
- 通过 `ItemsSource` 绑定数据项，并由 ListBox 生成 `ListBoxItem` 容器。

直接子元素适合静态少量条目；`ItemsSource` 适合数据驱动列表、候选列表和需要模板复用的场景。

## 包与命名空间

| 项 | 值 |
| --- | --- |
| NuGet 包 | `AtomUI.Desktop.Controls` |
| .NET 命名空间 | `AtomUI.Desktop.Controls` |
| AXAML 命名空间 | `https://atomui.net` |
| Gallery 页面 | `controlgallery/AtomUIGallery/ShowCases/DataDisplay/List` |
| 状态 | Stable |

## 何时使用

ListBox 表达的是“紧凑纵向条目 + 当前选择目标”的信息浏览语义。用户应能通过行距、hover 背景、selected 背景、可选选中标记、禁用状态和过滤高亮理解当前可操作条目。

设计语言由以下维度组成：

| 维度 | 含义 | 典型表达 |
| --- | --- | --- |
| 列表结构 | 一组同级条目按纵向排列。 | ScrollViewer、ItemsPresenter、条目间距。 |
| 选择状态 | 当前业务选择目标。 | selected 背景、`SelectedItem` / `SelectedItems`、选中指示器。 |
| 交互状态 | 鼠标、触摸、键盘导致的临时反馈。 | pointerover 背景、键盘移动选择。 |
| 禁用状态 | 条目或列表不可交互。 | disabled 文本色、禁用选择更新。 |
| 过滤状态 | 搜索或过滤命中的文本。 | 高亮文本、可选隐藏未命中项、过滤空状态。 |
| 空状态 | 无数据或过滤无结果。 | `EmptyIndicator` 内容区域。 |

ListBox 的视觉强度应保持克制。条目是可扫描的信息行，不应被绘制成卡片；选中指示器是补充信号，不应替代 selected 背景和文本状态。

## 公共 API

ListBox 的公共契约由 ListBox API、ListBoxItem API、事件 API、template part 和主题入口组成。

ListBox 核心 API：

| API | 语义 |
| --- | --- |
| `IsSelectable` | 是否允许用户交互更新选择。关闭时清空当前选择。 |
| `SelectionMode` / `SelectedIndex` / `SelectedItem` / `SelectedItems` / `Selection` | 继承 Avalonia `ListBox` 的选择模型。 |
| `SizeType` | 控制整体圆角、空状态 padding 和条目高度、padding。 |
| `IsBorderless` | 是否隐藏 root 边框。 |
| `ItemHoverBg` / `ItemSelectedBg` | 条目 hover 和 selected 背景入口。 |
| `IsShowSelectedIndicator` | 是否在选中条目右侧显示选中标记。 |
| `SelectedIndicator` | 选中标记图标模板入口。 |
| `IsMotionEnabled` | 是否启用条目背景和前景过渡动效。 |
| `EmptyIndicator` / `EmptyIndicatorTemplate` / `EmptyIndicatorPadding` / `IsShowEmptyIndicator` | 空状态展示入口。 |
| `Filter` / `FilterValue` / `FilterValueSelector` | 文本过滤入口。 |
| `FilterHighlightStrategy` | 过滤文本高亮和隐藏未命中项的策略入口。 |
| `FilterResultCount` | 当前过滤命中数量。 |
| `IsFiltering` | 当前是否处于过滤状态。 |
| `FilterHighlightForeground` | 过滤高亮前景色入口。 |

ListBoxItem 容器 API：

| API | 语义 |
| --- | --- |
| `Clicked` | 条目 pointer press 触发的 routed event。 |
| `IsSelected` | 继承 Avalonia `ListBoxItem` 的选择状态。 |
| `Content` / `ContentTemplate` | 条目内容和模板。 |

事件 API：

| 事件 | 语义 |
| --- | --- |
| `ItemClicked` | ListBoxItem `Clicked` 冒泡到 ListBox 后触发，携带被点击容器。 |
| `ItemCountChanged` | `ItemCount` 变化后触发。 |

稳定 template part：

| Template Part | 所属控件 | 职责 |
| --- | --- | --- |
| `Frame` | `ListBox` | root 背景、边框、圆角和 padding 边界。 |
| `PART_ScrollViewer` | `ListBox` | 列表滚动容器。 |
| `ItemsPresenter` | `ListBox` | 条目容器承载入口。 |
| `EmptyIndicator` | `ListBox` | 空状态内容展示。 |
| `Frame` | `ListBoxItem` | 条目背景、圆角和 padding 边界。 |
| `SelectedIndicator` | `ListBoxItem` | 选中标记展示入口。 |
| `ContentPresenter` | `ListBoxItem` | 普通内容展示入口。 |

ListBoxItem 内部还包含过滤文本高亮节点。该节点属于过滤展示实现，不作为用户替换模板时的 public template part。

伪类契约主要继承 Avalonia `ListBoxItem`：

- `:pointerover` 表示条目 hover。
- `:selected` 表示条目被选中。
- `:disabled` 表示条目不可用。

## 事件与命令

ListBox 的公共契约由 ListBox API、ListBoxItem API、事件 API、template part 和主题入口组成。
| `Clicked` | 条目 pointer press 触发的 routed event。 |
事件 API：
| 事件 | 语义 |

## 使用示例

稳定示例来源于 Gallery ShowCase 和源码查看片段。生成器只输出可从 `ShowCaseItem` 追溯的示例，不维护第二套手写示例。

以下示例来自 Gallery 源码查看使用的 `ShowCaseItem` 片段，并已按中文资源规范化。

### 筛选

来源：`controlgallery/AtomUIGallery/ShowCases/DataDisplay/List/Views/ListAdvancedShowCase.axaml:12`

Gallery key：`ExamplesContent` / item `0`

```axaml
<atom:ListView Name="FilteredList"
```

### 排序

来源：`controlgallery/AtomUIGallery/ShowCases/DataDisplay/List/Views/ListAdvancedShowCase.axaml:22`

Gallery key：`ExamplesContent` / item `1`

```axaml
<atom:ListView Name="OrderedList"
```

### 简单列表框控件

来源：`controlgallery/AtomUIGallery/ShowCases/DataDisplay/List/Views/ListAdvancedShowCase.axaml:30`

Gallery key：`ExamplesContent` / item `2`

```axaml
<atom:ListBox>
    <atom:ListBoxItem Content="赛车向人群喷出燃烧的燃料。" />
    <atom:ListBoxItem Content="日本公主将嫁给平民。" />
    <atom:ListBoxItem Content="澳大利亚人在内陆车祸后步行 100 公里。" />
    <atom:ListBoxItem Content="男子因婚礼女孩失踪案被起诉。" />
    <atom:ListBoxItem Content="洛杉矶抗击大规模山火。" />
</atom:ListBox>
```

### 简单列表框控件

来源：`controlgallery/AtomUIGallery/ShowCases/DataDisplay/List/Views/ListAdvancedShowCase.axaml:42`

Gallery key：`ExamplesContent` / item `3`

```axaml
<atom:ListBox ItemsSource="{Binding BasicListBoxItems}"
```

## 状态模型

ListBox 的状态模型由选择状态、交互状态、过滤状态、空状态、尺寸状态和动效状态组成。

选择行为：

- `IsSelectable=false` 时，ListBox 不响应 pointer 或 keyboard 选择更新，并清空 `SelectedIndex`、`SelectedItem` 和 `SelectedItems`。
- `SelectionMode` 继承 Avalonia `ListBox` 语义，单选使用 `SelectedItem`，多选使用 `SelectedItems`。
- 方向键按 Avalonia navigation direction 移动选择，`Shift` 支持范围选择，平台 select-all 手势在多选模式下选择全部。
- `IsShowSelectedIndicator=true` 时，选中容器右侧显示 `SelectedIndicator`。

点击行为：

- `ListBoxItem.Clicked` 是 routed event，ListBox 通过 class handler 收敛到 `ItemClicked`。
- 派生控件可重写 `NotifyListBoxItemClicked` 承接点击行为，例如 CandidateList 在单选场景中提交候选项。

过滤行为：

- `Filter`、`FilterValue` 和 `FilterValueSelector` 共同决定条目是否命中。
- `IsFiltering=true` 时条目进入过滤展示状态，并通过 `FilterHighlightStrategy` 决定高亮和隐藏未命中项。
- `FilterResultCount` 表示命中数量；过滤模式下空状态依据 `FilterResultCount == 0` 判断。
- 过滤展示主要面向文本内容。复杂自定义 `ItemTemplate` 需要维护者明确处理过滤态展示入口，避免过滤态绕过用户模板。

空状态行为：

- `ItemCount == 0` 时显示空状态。
- `IsFiltering && FilterResultCount == 0` 时显示过滤空状态。
- `IsShowEmptyIndicator=false` 时不显示空状态内容。

动效行为：

- ListBoxItem 初始化阶段禁用 transitions，loaded 后启用，避免初始状态产生非预期动画。
- `IsMotionEnabled=false` 时不应用背景和前景过渡。

## 主题与 Design Token

ListBox 主题按 root 和 item 两层组织。

```text
ListBoxTheme
  Frame
  PART_ScrollViewer
  ItemsPresenter
  EmptyIndicator

ListBoxItemTheme
  Frame
  SelectedIndicator
  ContentPresenter
  HighlightableTextBlock
```

视觉规则：

- root 边框由 `BorderBrush`、`BorderThickness`、`CornerRadius` 和 `IsBorderless` 共同决定。
- `SizeType` 控制 root 圆角、空状态 padding、条目最小高度和条目 padding。
- 默认条目背景透明，hover 使用 `ItemHoverBg`，selected 使用 `ItemSelectedBg`。
- disabled 内容使用 SharedToken disabled 文本色。
- 选中指示器默认使用 默认 `CheckOutlined`，颜色使用 SharedToken 主色，尺寸使用 SharedToken icon size。
- 空状态默认使用 `Empty` 的 simple preset image。

ListBoxToken 提供内容 padding、条目文字颜色、条目状态背景、条目 padding、条目 margin、选中指示器 margin 和过滤高亮色。Token 详情见 [ListBox Token 设计](token.md)。

Token 来源：

ListBoxToken 是 ListBox 的组件级设计变量层。它把全局颜色、尺寸、间距和状态色转换为 ListBox root、ListBoxItem、selected indicator 和 filter highlighter 可消费的语义值。

ListBoxToken 服务以下主题和控件：

- `ListBoxTheme.axaml`
- `ListBoxItemTheme.axaml`
- `CandidateListTheme.axaml`
- `CandidateListItemTheme.axaml`
- `CascaderViewFilterListTheme.axaml`
- `ListBox` / `ListBoxItem` / `CandidateList` / `CandidateListItem`

ListBoxToken 不承载 `SelectedItem`、`SelectedItems`、`IsSelected`、`IsFiltering`、`FilterValue`、`FilterResultCount`、`IsEffectiveEmptyVisible`、`VirtualIndex` 等实例状态。这些状态由 C# 状态模型、容器属性和主题 selector 处理。

## AOT 与裁剪注意事项

ListBox 不通过反射访问模板内部结构。模板接入依赖稳定 part 名称和 Avalonia 属性绑定。

同生命周期的 root 到 item 状态同步使用 Avalonia `[!]` direct binding，不使用 `BindUtils.RelayBind` 和额外 disposable。容器被 ItemsControl 管理，binding 生命周期随容器生命周期结束。

选中指示器、过滤高亮和空状态内容是 AXAML 静态模板节点。隐藏状态依赖 `IsVisible`，不通过 C# 动态创建和销毁模板子节点。

`IsVisible=false` 的节点不参与 measure、arrange 和 render；因此过滤空状态、隐藏未命中项和默认隐藏选中指示器可以优先使用静态模板 + visible state 模式。

过滤和虚拟化路径不得引入固定延时、dispatcher timing hack 或全局订阅。新增缓存必须有明确失效点，并在 collection change、container clear 或 detach 相关路径中清理。

## 源码索引

主要源码：

- `src/AtomUI.Desktop.Controls/ListBox/ListBox.cs`：public API、事件、容器生成、空状态、过滤、选择拦截、键盘导航和虚拟化上下文管理。
- `src/AtomUI.Desktop.Controls/ListBox/ListBoxItem.cs`：条目容器、点击 routed event、选中指示器状态、过滤文本展示状态、pointer selection 协同和动效启停。
- `src/AtomUI.Desktop.Controls/ListBox/ListBoxItemClickedEventArgs.cs`：`ItemClicked` 事件参数。
- `src/AtomUI.Desktop.Controls/ListBox/ListBoxToken.cs`：ListBox 专属 Token 定义。
- `src/AtomUI.Desktop.Controls/ListBox/Themes/ListBoxTheme.axaml`：root 模板、ScrollViewer、ItemsPresenter、EmptyIndicator、默认 ItemTemplate 和 root 样式。
- `src/AtomUI.Desktop.Controls/ListBox/Themes/ListBoxItemTheme.axaml`：条目模板、选中指示器、普通内容、过滤高亮文本和条目状态样式。
- `src/AtomUI.Desktop.Controls/ListBox/Themes/ListBoxThemes.axaml`：ListBox 和 ListBoxItem theme 注册。
- `src/AtomUI.Desktop.Controls/Primitives/CandidateList/CandidateList.cs`：基于 ListBox 的候选项列表扩展。
- `src/AtomUI.Controls.Shared/IListVirtualizingContextAware.cs`：虚拟化上下文保存、恢复和清理接口。

## 相关文档

- 源设计文档：`docs/controls/desktop/data-display/list-box/overview.md`
- 实现文档：`docs/controls/desktop/data-display/list-box/implementation.md`
- Token 文档：`docs/controls/desktop/data-display/list-box/token.md`
- 变更记录：`docs/controls/desktop/data-display/list-box/changelog.md`
- 语义结构：`./semantic-cn.md`
