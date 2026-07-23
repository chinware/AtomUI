# ListBox 桌面版实现原理

本文档描述 ListBox 桌面版的内部源码结构、状态流转、容器生命周期、过滤、点击、空状态和 CandidateList 复用边界。公共设计与 API 契约见 [ListBox 桌面版架构设计](overview.md)，Token 语义见 [ListBox Token 设计](token.md)，变化记录见 [ListBox Changelog](changelog.md)。

## 1. 实现定位

ListBox 的实现目标是在 Avalonia `ListBox` 基类上增加 AtomUI 的视觉、Token、空状态、选中指示器、过滤高亮和候选列表复用能力，并保持 Avalonia selection model 与 keyboard navigation 语义稳定。

本文档覆盖 `ListBox`、`ListBoxItem`、ListBox token、AXAML theme、虚拟化上下文接口和 CandidateList 继承关系。具体属性注册、CLR wrapper 和小型 guard 逻辑仍应直接阅读源码。

## 2. 源码文件结构

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

## 3. 核心类职责

`ListBox` 是列表根控件，负责注册 token scope、创建 `ListBoxItem` 容器、绑定 root 状态到 item 状态、处理空状态、过滤条目、阻止不可选择状态下的 selection 更新、派发 `ItemClicked` 和保存虚拟化容器上下文。

`ListBoxItem` 是条目容器，继承 Avalonia `ListBoxItem`。它负责承载内容、选中状态、pointer 输入、点击 routed event、选中指示器可见性、过滤文本展示输入和条目动效。

`ListBoxToken` 是组件级设计变量层，为 root、item、selected indicator 和 filter highlighter 提供组件语义值。

`CandidateList` 继承 ListBox，增加候选项导航、候选项临时高亮、commit / cancel 事件和最大选择数量控制。它复用 ListBox 的容器生成、过滤、空状态和 item theme。

## 4. 状态与数据流

容器生成流：

```text
Items / ItemsSource
      ↓
ListBox.CreateContainerForItemOverride
      ↓
ListBoxItem
      ↓
PrepareContainerForItemOverride
  ContentTemplate <- ItemTemplate
  SizeType <- ListBox.SizeType
  IsMotionEnabled <- ListBox.IsMotionEnabled
  SelectedIndicator <- ListBox.SelectedIndicator
  ItemHoverBg / ItemSelectedBg
  IsShowSelectedIndicator
  FilterHighlightStrategy / FilterHighlightForeground
  virtual index / restored context
```

选择状态流：

```text
Pointer / keyboard event
      ↓
ListBoxItem.OnPointerPressed / OnPointerReleased 或 ListBox.OnKeyDown
      ↓
ListBox.UpdateSelectionFromPointerEvent / UpdateSelectionFromEvent
      ↓
Avalonia Selection model
      ↓
ListBoxItem.IsSelected
      ↓
IsSelectedIndicatorVisible = IsShowSelectedIndicator && IsSelected
```

过滤状态流：

```text
Filter / FilterValue / FilterValueSelector / FilterHighlightStrategy
      ↓
ListBox.FilterItems
      ↓
SelectFilterValue(item)
      ↓
IValueFilter.Filter(value, FilterValue)
      ↓
ListBoxItem.IsFiltering / FilterValue / IsVisible
      ↓
FilterResultCount + IsEffectiveEmptyVisible
```

空状态流：

```text
ItemCount changed
ItemsSource changed
IsFiltering changed
FilterResultCount changed
      ↓
ConfigureEmptyIndicator
      ↓
IsEffectiveEmptyVisible
      ↓
PART_ScrollViewer hidden / EmptyIndicator shown
```

## 5. 生命周期与模板接入

`ListBox` 静态构造注册三类 class handler：

- `ListBoxItem.ClickedEvent` 收敛到 `HandleListBoxItemClicked`。
- `IsSelectableProperty.Changed` 在关闭可选状态时清空选择。
- `ItemCountProperty.Changed` 触发 `ItemCountChanged`。

`ListBox` 实例构造注册 `ListBoxToken` scope，并监听 `Items.CollectionChanged`。集合变化时清空虚拟化恢复上下文，刷新空状态，并重新执行过滤。

`OnInitialized` 设置默认 `Filter`，刷新空状态和 `IsFiltering`。`OnLoaded` 在已有 `FilterValue` 时执行过滤。

`PrepareContainerForItemOverride` 是 root 状态同步到 item 的核心入口。它使用 Avalonia `[!]` direct binding 将同生命周期状态绑定到容器，并在虚拟化场景下设置 `VirtualIndex`、恢复缓存上下文和临时关闭 motion，避免回收容器出现初始化动画。

`ClearContainerForItemOverride` 是容器回收清理入口。它保存需要跨回收恢复的上下文，清理容器本地值，再交给基类释放容器。新增容器级状态时必须同时补齐 save、restore、clear 三个路径。

`ListBoxItem.OnInitialized` 初始化选中指示器状态并禁用 transitions。`OnLoaded` 启用 transitions。该顺序用于避免模板初次应用时触发背景和前景过渡。

## 6. 交互与事件处理

pointer selection：

- 鼠标左键和右键按下时立即通过 owner ListBox 更新 selection。
- 触摸和笔输入延迟到 release，避免按下阶段阻断滚动手势。
- touch / pen release 成功更新 selection 后，ListBoxItem 会将事件重新 raise 到 owner，以便上层完成 commit 类行为。

click event：

- 左键 pressed 时，ListBoxItem raise `ClickedEvent`。
- ListBox 通过 class handler 接收后调用 `NotifyListBoxItemClicked`，再触发 public `ItemClicked`。
- CandidateList 通过重写 `NotifyListBoxItemClicked` 在单选模式提交候选项。

keyboard navigation：

- `IsSelectable=true` 时，方向键进入 Avalonia `MoveSelection`。
- 多选模式下，平台 select-all 手势调用 `Selection.SelectAll()`。
- `IsSelectable=false` 时，ListBox 不处理这些键盘选择路径。

context request：

- ListBoxItem 右键 release 且事件 source 为当前 item 时，创建 `ContextRequestedEventArgs` 并 raise context request。

## 7. 内部算法与关键流程

### 7.1 Effective border thickness

`IsBorderless` 和 `BorderThickness` 合成 `EffectiveBorderThickness`。模板只绑定 effective 值，从而保持 borderless 模式与普通边框模式共享同一 root 模板。

### 7.2 过滤值选择

`SelectFilterValue` 按以下顺序选择过滤值：

1. 使用 `FilterValueSelector(item.Content)`。
2. 对 `IListItemData` 使用 `Content?.ToString()`。
3. 对字符串内容直接使用字符串。
4. 其他内容使用 item content 本身。

该路径服务轻量文本列表。复杂模板场景应通过 `FilterValueSelector` 提供稳定文本值。

### 7.3 过滤上下文

隐藏未命中项时，ListBox 使用 `_filterContext` 备份进入过滤前的容器 `IsVisible`。清除过滤时按备份恢复可见性，并清理 item 的 `IsFiltering` 和 `FilterValue`。

维护此路径时必须保证：

- 筛选清除后不会遗留本地 `IsVisible`。
- 容器回收时不会把旧 filter 状态带到新 item。
- `FilterResultCount` 与空状态能表达完整数据结果，而不是只表达当前 viewport 中已实现容器。

### 7.4 虚拟化上下文

ListBox 实现 `IListVirtualizingContextAware`，用 `_virtualRestoreContext` 保存被回收容器的上下文。默认保存 `IsEnabled`，派生控件可以重写 `NotifySaveVirtualizingContext`、`NotifyRestoreVirtualizingContext` 和 `NotifyClearContainerForVirtualizingContext` 扩展上下文。

上下文保存以 `VirtualIndex` 为 key。`PrepareContainerForItemOverride` 会在相同 index 的容器重新实现时恢复上下文并移除缓存。

### 7.5 CandidateList 扩展点

CandidateList 使用 ListBox 的 container pipeline，但替换容器类型为 `CandidateListItem`，并覆盖：

- `CreateContainerForItemOverride`
- `NeedsContainerOverride`
- `ConfigureEmptyIndicator`
- `NotifyRestoreDefaultContext`
- `NotifyClearContainerForVirtualizingContext`

这些扩展点要求 ListBox 的基础 prepare / clear 顺序稳定，避免候选项临时高亮、disabled 状态和 selection 状态在虚拟化回收时串扰。

## 8. 资源、性能与 AOT 边界

ListBox 不通过反射访问模板内部结构。模板接入依赖稳定 part 名称和 Avalonia 属性绑定。

同生命周期的 root 到 item 状态同步使用 Avalonia `[!]` direct binding，不使用 `BindUtils.RelayBind` 和额外 disposable。容器被 ItemsControl 管理，binding 生命周期随容器生命周期结束。

选中指示器、过滤高亮和空状态内容是 AXAML 静态模板节点。隐藏状态依赖 `IsVisible`，不通过 C# 动态创建和销毁模板子节点。

`IsVisible=false` 的节点不参与 measure、arrange 和 render；因此过滤空状态、隐藏未命中项和默认隐藏选中指示器可以优先使用静态模板 + visible state 模式。

过滤和虚拟化路径不得引入固定延时、dispatcher timing hack 或全局订阅。新增缓存必须有明确失效点，并在 collection change、container clear 或 detach 相关路径中清理。

## 9. 维护不变量

内部重构必须保持以下不变量：

- `ListBox.cs` 保留 public API、事件、容器生命周期和虚拟化上下文入口。
- `ListBoxItem.cs` 保持容器角色，不承载业务数据源请求、排序、分页或跨列表全局状态。
- `PrepareContainerForItemOverride` 新增状态同步时，必须在 `ClearContainerForItemOverride` 或 `NotifyClearContainerForVirtualizingContext` 对称清理。
- `FilterValueSelector` 是复杂数据项的过滤值入口，不应在 ListBox 内部硬编码业务数据类型。
- `FilterHighlightStrategy` 只控制过滤展示，不改变 selection model。
- 选中指示器可见性只能由 `IsShowSelectedIndicator && IsSelected` 推导。
- `ItemClicked` 派发顺序必须允许 CandidateList 在 public event 前执行 `NotifyListBoxItemClicked`。
- `IsBorderless` 只影响边框厚度，不改变 root padding、corner radius 或 scroll behavior。
- Token 变更必须同步 `ListBoxTokenKind`、AXAML 引用和 token.md 语义说明。

## 10. 测试与验证

ListBox 相关验证入口：

- `tools/performances/AtomUI.Performance/Suites/ListBox/ListBoxStateVerification.cs`
- `tools/performances/AtomUI.Performance/Suites/ListBox/ListBoxScenarios.cs`
- `controlgallery/AtomUIGallery/ShowCases/DataDisplay/List/`
- AutoComplete、Mentions 和 Cascader 中使用 CandidateList / ListBox 派生控件的场景。

建议验证命令：

```bash
dotnet run --project tools/performances/AtomUI.Performance/AtomUI.Performance.csproj -c Debug --framework net10.0 --no-restore -- --verify-listbox-states
dotnet run --project tools/performances/AtomUI.Performance/AtomUI.Performance.csproj -c Debug --framework net10.0 --no-build -- --suite listbox --count 60
dotnet test tests/AtomUIGallery.Tests/AtomUIGallery.Tests.csproj --framework net10.0 --no-restore
git diff --check
```

验证重点：

- 默认 ListBox 静态 shape：选中指示器和过滤文本节点默认隐藏。
- selected indicator 生命周期：选中、取消选中、切换选中项。
- filter 生命周期：进入过滤、清除过滤、隐藏未命中、空结果。
- CandidateList keyboard navigation、commit、cancel 和 empty navigation。
- 虚拟化回收后 disabled、selected indicator、candidate selected 和 filter 状态不串扰。
- Gallery List ShowCase、AutoComplete、Mentions、Cascader 相关候选列表显示稳定。
