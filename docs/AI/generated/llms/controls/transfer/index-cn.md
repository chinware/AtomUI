# Transfer

> 生成产物：由源文档生成，不要手工编辑。修改内容请回到控件文档、源码 public surface、Token 类型或生成数据、Gallery ShowCase 或源码结构。

## 概述

Transfer 是 AtomUI 桌面控件体系中的穿梭框控件，用于在源列表和目标列表之间移动、搜索和选择数据项。

Transfer 不负责表格编辑器、树形穿梭或远程数据同步协议。这些职责应由业务层、组合控件或更专用的 AtomUI 控件承担。

主要源码入口：

- `src/AtomUI.Desktop.Controls/Transfer`

## 包与命名空间

| 项 | 值 |
| --- | --- |
| NuGet 包 | `AtomUI.Desktop.Controls` |
| .NET 命名空间 | `AtomUI.Desktop.Controls` |
| AXAML 命名空间 | `https://atomui.net` |
| Gallery 页面 | `controlgallery/AtomUIGallery/ShowCases/DataEntry/Transfer` |
| 状态 | Stable |

## 何时使用

Transfer 的设计语言围绕控件职责、可观察状态和主题契约组织，而不是围绕模板节点组织。

| 维度 | 含义 | Transfer 中的表达 |
| --- | --- | --- |
| 产品语义 | 控件在界面中承担的稳定职责。 | Transfer 是 AtomUI 桌面控件体系中的穿梭框控件，用于在源列表和目标列表之间移动、搜索和选择数据项。 |
| 内容承载 | 用户数据、展示内容、集合项或操作入口如何进入控件。 | `Content`、`ContentTemplate`、`FilterPlaceholderText`、`FilterValueSelector`、`FooterTemplate`、`ItemTemplate`、`SelectionsIcon`、`SelectionsIconTemplate` 等 22 项。 |
| 状态反馈 | public API、内部状态和伪类如何形成用户可感知反馈。 | selection/checked/active、collection/filter、input/value、motion、visual option。 |
| 主题语义 | ControlTheme、SharedToken、控件 Token 和模板绑定如何表达视觉。 | Transfer Token + ControlTheme。 |

## 公共 API

Transfer 的公共契约由 public/protected 类型成员、Avalonia 属性、事件、命令、template part、伪类、ControlTheme key 和资源 key 共同组成。维护时应先确认这些契约是否已经被源码、Gallery 示例或文档暴露。

核心 public surface 按语义分组维护：

| 契约组 | 代表成员 | 维护含义 |
| --- | --- | --- |
| 内容与数据 | `Content`、`ContentTemplate`、`FilterPlaceholderText`、`FilterValueSelector`、`FooterTemplate`、`ItemTemplate`、`SelectionsIcon`、`SelectionsIconTemplate`、`SourceTitle`、`SourceTitleTemplate` 等 22 项 | 定义控件展示内容、输入数据、模板或业务对象入口。 |
| 选择与集合 | `TargetKeys`、`SelectedKeys`、`Filter`、`IsAllSelected`、`IsFilterEnabled`、`PageSize` | 维护目标集合、当前面板选择、过滤、分页和集合状态。 |
| 交互与状态 | `IsMasked`、`IsMotionEnabled`、`IsOneWay`、`IsPaginationEnabled`、`IsShowSearch`、`IsShowSelectAll`、`IsShowSelectAllCheckbox`、`IsShowSelectDropdownMenu`、`IsStretchView`、`Status` | 表达用户可观察状态、可用性、清除、加载或反馈语义。 |
| 视觉与布局 | `ListHeight`、`ListWidth`、`SizeType` | 影响尺寸、位置、颜色、形状、密度和模板视觉变量。 |
| 其他稳定入口 | `Footer`、`TargetView`、`TargetViewFooter`、`ViewType` | 保留为 public surface，变更前需确认 Gallery 和用户 XAML 依赖。 |

`TargetKeys` 表示已经移动到目标面板的条目 key，是 Transfer 的提交值；`SelectedKeys` 表示当前源面板和目标面板内被选中的条目 key。两者默认 `BindingMode.TwoWay`，并支持 `INotifyCollectionChanged` 集合的原地 `Add`、`Remove`、`Replace`、`Move` 和 `Reset`。当绑定集合可写时，Transfer 交互优先原地更新已有集合，避免替换绑定源造成外部状态不同步。

稳定事件包括 `SelectActionRequest`。事件触发顺序属于兼容契约，不能因内部状态重排而改变。

主要公开类型与枚举：

- 类型：`AbstractTransfer`、`ListTransfer`、`TransferItemDecorator`、`TransferItemsRemovedEventArgs`、`TransferListItem`、`TransferListView`、`TransferRemoveItemButton`、`TransferSelectActionEventArgs`、`TransferSelectDropdown`、`TransferSelectionChangedEventArgs`、`TransferTreeView`、`TransferTreeViewItem`、`TransferTreeViewItemHeader`、`TransferViewCreatedEventArgs` 等 18 项。
- 枚举：`TransferDirection`、`TransferSelectAction`、`TransferViewType`。

稳定 template part：

| Template Part | 类型 | 职责 |
| --- | --- | --- |
| `PART_IconPresenter` | `?` | 展示用户内容、文本、图标或模板化数据。 |

当前未抽取到控件专属伪类；主题主要依赖 Avalonia 标准伪类、模板绑定和内部 StyledProperty。

## 事件与命令

Transfer 的公共契约由 public/protected 类型成员、Avalonia 属性、事件、命令、template part、伪类、ControlTheme key 和资源 key 共同组成。维护时应先确认这些契约是否已经被源码、Gallery 示例或文档暴露。
稳定事件包括 `SelectActionRequest`。事件触发顺序属于兼容契约，不能因内部状态重排而改变。
- 类型：`AbstractTransfer`、`ListTransfer`、`TransferItemDecorator`、`TransferItemsRemovedEventArgs`、`TransferListItem`、`TransferListView`、`TransferRemoveItemButton`、`TransferSelectActionEventArgs`、`TransferSelectDropdown`、`TransferSelectionChangedEventArgs`、`TransferTreeView`、`TransferTreeViewItem`、`TransferTreeViewItemHeader`、`TransferViewCreatedEventArgs` 等 18 项。

## 使用示例

稳定示例来源于 Gallery ShowCase 和源码查看片段。生成器只输出可从 `ShowCaseItem` 追溯的示例，不维护第二套手写示例。

以下示例来自 Gallery 源码查看使用的 `ShowCaseItem` 片段，并已按中文资源规范化。

### 基础用法

来源：`controlgallery/AtomUIGallery/ShowCases/DataEntry/Transfer/Views/TransferShowCase.axaml:258`

Gallery key：`ExamplesContent` / item `0`

```axaml
<atom:ListTransfer Name="BasicListTransfer"
```

### 单向模式

来源：`controlgallery/AtomUIGallery/ShowCases/DataEntry/Transfer/Views/TransferShowCase.axaml:272`

Gallery key：`ExamplesContent` / item `1`

```axaml
<StackPanel Spacing="20">
    <atom:ListTransfer Name="OneWayTransferList"
                       SourceTitle="源列表"
                       TargetTitle="目标列表"
                       IsOneWay="True"
                       IsEnabled="{Binding OneWayTransferEnabled}"
                       ItemsSource="{Binding OneWayTransferItems}" />
    <atom:ToggleSwitch IsChecked="{Binding OneWayTransferEnabled, Mode=TwoWay}"
                       OnContent="禁用"
                       OffContent="启用" />
</StackPanel>
```

### 搜索

来源：`controlgallery/AtomUIGallery/ShowCases/DataEntry/Transfer/Views/TransferShowCase.axaml:293`

Gallery key：`ExamplesContent` / item `2`

```axaml
<StackPanel Spacing="20">
    <atom:ListTransfer Name="SearchTransferList"
                       IsFilterEnabled="True"
                       FilterPlaceholderText="在此搜索"
                       FilterValueSelector="{Binding TransferFilterValueSelector}"
                       ItemsSource="{Binding SearchTransferItems}" />
</StackPanel>
```

### 受控 key

来源：`controlgallery/AtomUIGallery/ShowCases/DataEntry/Transfer/Views/TransferShowCase.axaml:311`

Gallery key：`ExamplesContent` / item `3`

```axaml
<StackPanel Spacing="20">
    <atom:ListTransfer Name="ControlledTransferList"
                       SourceTitle="源列表"
                       TargetTitle="目标列表"
                       ItemsSource="{Binding ControlledTransferItems}"
                       TargetKeys="{Binding ControlledTransferTargetKeys}"
                       SelectedKeys="{Binding ControlledTransferSelectedKeys}" />
    <StackPanel Orientation="Horizontal" Spacing="8">
        <atom:Button SizeType="Small"
                     Click="AddControlledTransferTargetKey"
                     Content="添加 key 3 到目标" />
        <atom:Button SizeType="Small"
                     Click="ClearControlledTransferTargetKeys"
                     Content="清空目标 key" />
        <atom:Button SizeType="Small"
                     Click="SelectControlledTransferSourceKey"
                     Content="选中 key 4" />
    </StackPanel>
    <StackPanel Orientation="Horizontal" Spacing="8">
        <atom:TextBlock Text="目标 key：" />
        <atom:TextBlock Text="{Binding ControlledTransferTargetKeys.Count}" />
        <atom:TextBlock Text="选中 key：" />
        <atom:TextBlock Text="{Binding ControlledTransferSelectedKeys.Count}" />
    </StackPanel>
</StackPanel>
```

## 状态模型

Transfer 的状态流按以下路径收敛：

```text
Public API / inherited command / item source / user input
  -> 控件实例状态
  -> effective state / pseudo-class / template property
  -> ControlTheme selector / presenter / renderer
  -> Gallery 可观察行为
```

状态维护规则：

- Disabled 或不可交互状态优先屏蔽 pointer、keyboard、motion 和提交类反馈。
- selection/checked/active、collection/filter、input/value、motion、visual option 状态由控件实例或明确的数据 owner 推导，不能在 template part 之间双向竞争。
- `TargetKeys` 是目标集合的 public owner，源/目标面板数据由 `ItemsSource` 与 `TargetKeys` 推导；`SelectedKeys` 是当前选择的 public owner，内部源面板选择和目标面板选择按 key 是否存在于 `TargetKeys` 自动拆分。
- 模板重套用时必须把 public API 对应状态回放到新的 part、伪类和主题变量。
- 集合、弹层、异步、动效或窗口相关状态必须能处理 reset、close、cancel、detach 和 owner 释放。

## 主题与 Design Token

Transfer 的视觉模型由控件模板、ControlTheme、SharedToken 和必要的控件 Token 共同构成。

| 主题文件 | 职责 |
| --- | --- |
| `AbstractTransferTheme.axaml` | 提供控件模板、selector、资源绑定和状态视觉。 |
| `ListTransferTheme.axaml` | 提供控件模板、selector、资源绑定和状态视觉。 |
| `TransferItemDecoratorTheme.axaml` | 定义集合项、容器项或局部单元的状态视觉。 |
| `TransferListItemTheme.axaml` | 定义集合项、容器项或局部单元的状态视觉。 |
| `TransferListViewTheme.axaml` | 提供控件模板、selector、资源绑定和状态视觉。 |
| `TransferSelectDropdownTheme.axaml` | 提供控件模板、selector、资源绑定和状态视觉。 |
| `TransferTreeViewItemHeaderTheme.axaml` | 定义集合项、容器项或局部单元的状态视觉。 |
| `TransferTreeViewItemTheme.axaml` | 定义集合项、容器项或局部单元的状态视觉。 |
| `TransferTreeViewTheme.axaml` | 提供控件模板、selector、资源绑定和状态视觉。 |
| `TreeTransferTheme.axaml` | 提供控件模板、selector、资源绑定和状态视觉。 |

Transfer 使用 `TransferToken` 作为控件 Token scope。Token 只表达组件视觉语义，不承载 selection/checked/active、collection/filter、input/value、motion、visual option 运行时状态。

主题维护规则：

- 不删除或重命名已经稳定的 ControlTheme key、template part、伪类和资源 key。
- 不把可由 AXAML 表达的模板状态迁移为 C# 动态创建视觉。
- 不把 hover、pressed、selected、expanded、loading、filter、popup open 等运行时状态写入 Token。
- Browser 或平台特化主题必须保持同一 API 的语义一致。

Token 来源：

Transfer Token 只表达组件级视觉变量，例如尺寸、间距、颜色、圆角、阴影、图标尺寸和弹层边界。Token 不承载运行时选择、展开、加载、错误、上传任务、过滤条件或业务状态。

当前 Token scope：

- `TransferToken`，scope id 为 `Transfer`，源码位于 `src/AtomUI.Desktop.Controls/Transfer/TransferToken.cs`。

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

- `src/AtomUI.Desktop.Controls/Transfer`：21 个文件，代表文件 `AbstractTransfer.cs`、`ITransferTreeView.cs`、`ITransferView.cs`、`ListTransfer.cs`、`TransferDirection.cs` 等。
- `src/AtomUI.Desktop.Controls/Transfer/Localization`：`TransferLangResourceKind.cs` 定义稳定 Catalog，`en-US.xlf`、`zh-CN.xlf`、`zh-TW.xlf` 提供内置翻译。
- `src/AtomUI.Desktop.Controls/Transfer/Themes`：12 个文件，代表文件 `AbstractTransferTheme.axaml`、`AbstractTransferTheme.cs`、`ListTransferTheme.axaml`、`TransferItemDecoratorTheme.axaml`、`TransferListItemTheme.axaml` 等。

职责边界：

- 控件主文件保留 public/protected API、Avalonia 属性注册、事件和主要生命周期入口。
- Theme 文件负责静态视觉结构、template part、selector 和资源绑定。
- Token 文件只提供组件视觉变量，不保存实例状态。
- Gallery 文件只展示用法和示例，不作为运行时逻辑 owner。

## 相关文档

- 源设计文档：`docs/controls/desktop/data-entry/transfer/overview.md`
- 实现文档：`docs/controls/desktop/data-entry/transfer/implementation.md`
- Semantic Part 文档：`docs/controls/desktop/data-entry/transfer/semantic-part.md`
- Token 文档：`docs/controls/desktop/data-entry/transfer/token.md`
- 变更记录：`docs/controls/desktop/data-entry/transfer/changelog.md`
- 语义结构：`./semantic-cn.md`
