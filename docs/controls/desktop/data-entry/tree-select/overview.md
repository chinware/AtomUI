# TreeSelect 桌面版架构设计

本文档定义 `AtomUI.Desktop.Controls.TreeSelect` 的最新设计定位、公共契约、状态模型、视觉主题关系和兼容边界。通用控件研发约束见 [控件研发标准](../../../../engineering/control-development-guidelines.md)，内部实现原理见 [TreeSelect 桌面版实现原理](implementation.md)，TreeSelect Token 的专项设计见 [TreeSelect Token 设计](token.md)，设计和契约变化记录见 [TreeSelect Changelog](changelog.md)。

## 1. 控件定位

| 项 | 值 |
| --- | --- |
| NuGet 包 | `AtomUI.Desktop.Controls` |
| .NET 命名空间 | `AtomUI.Desktop.Controls` |
| AXAML 命名空间 | `https://atomui.net` |
| Gallery 页面 | `controlgallery/AtomUIGallery/ShowCases/DataEntry/TreeSelect` |
| 控件状态 | Stable |

TreeSelect 是 AtomUI 桌面数据录入体系中的树形选择控件，用于在层级数据中完成单选、多选和勾选式选择。它组合 Select 家族的输入壳体、过滤输入、弹层和 Form / CompactSpace 集成能力，并使用 TreeView 家族承载树节点、展开、勾选、图标、连线和异步加载。

TreeSelect 的职责是从 `ITreeItemNode` 层级数据中选择一个或多个节点。它不负责远程数据协议、业务权限过滤、级联业务规则、树节点编辑、拖放排序或复杂数据持久化；这些能力应由业务层、TreeView 或专用数据控件承担。

## 2. 设计语言

TreeSelect 的设计语言来自输入框、树形候选弹层和已选结果的组合。

| 维度 | 含义 | 典型表达 |
| --- | --- | --- |
| 树形数据 | 候选项按父子层级展示。 | `ItemsSource`、`Items`、`ItemTemplate`。 |
| 选择模式 | 控件支持单选、多选和 checkbox 勾选。 | `IsMultiple`、`IsTreeCheckable`、`IsTreeCheckStrictly`。 |
| 结果展示 | 单选显示节点文本，多选以 tag 展示有效选中节点。 | `SelectedItem`、`SelectedItems`、`ShowCheckedStrategy`。 |
| 搜索过滤 | 输入过滤值后高亮、加粗、展开路径或隐藏不匹配节点。 | `IsFilterEnabled`、`FilterValue`、`Filter`、`FilterStrategy`。 |
| 树视觉 | 控制图标、叶子图标、连线、展开指示和旋转。 | `IsShowIcon`、`IsShowLeafIcon`、`IsShowLine`、`IsShowTreeLine`。 |
| 输入表面 | 继承 Select 家族的边框、尺寸、状态和 AddOn。 | `SizeType`、`StyleVariant`、`Status`、Addon。 |
| 弹层语义 | 候选树在 popup 中打开，可匹配输入宽度。 | `IsDropDownOpen`、`PopupPlacement`、`IsPopupMatchSelectWidth`。 |

`SizeType=Custom` 不是 TreeSelect 的第四套专属 Token。主题层把 `Custom` 归入 `Middle` 的默认字体和 padding 分支；用户显式设置高度等尺寸属性时由 Avalonia 属性优先级决定最终布局。

## 3. API 与契约模型

TreeSelect 继承 `AbstractSelect` 的输入壳体、弹层、清除、状态、尺寸、Addon、Form 和 CompactSpace 契约；自身提供树数据、树选择、过滤和勾选策略契约。

树数据 API：

| API | 类型 | 语义 |
| --- | --- | --- |
| `ItemsSource` | `IEnumerable<ITreeItemNode>?` | 外部树节点集合。变化时同步到内部 `Items`。 |
| `Items` | `ItemCollection` | XAML 内容子项入口，也是候选树实际数据入口。 |
| `ItemTemplate` | `IDataTemplate?` | 树节点显示模板，默认显示 `ITreeItemNode.Header`。 |
| `DataLoader` | `ITreeItemNodeLoader?` | 展开节点时加载子节点的数据加载边界。 |
| `TreeDefaultExpandedPaths` | `IList<TreeNodePath>?` | 默认展开路径集合。 |
| `IsDefaultExpandAll` | `bool` | 默认展开全部节点。 |

选择 API：

| API | 类型 | 语义 |
| --- | --- | --- |
| `SelectedItem` | `ITreeItemNode?` | 单选模式当前节点。 |
| `SelectedItems` | `IList<ITreeItemNode>?` | 多选或勾选模式当前节点集合。 |
| `IsMultiple` | `bool` | 是否使用多选结果模型。 |
| `IsTreeCheckable` | `bool` | 是否使用 checkbox 勾选。启用后 TreeSelect 使用多选结果模型。 |
| `IsTreeCheckStrictly` | `bool` | 勾选时父子节点是否严格独立。 |
| `ShowCheckedStrategy` | `TreeSelectCheckedStrategy` | 多选 tag 的展示策略，决定展示全部、父节点或叶子节点。 |
| `MaxCount` | `int` | 继承自 `AbstractSelect` 的最大可选数量。 |
| `Clear()` | method | 清空 `SelectedItem` 和 `SelectedItems`。 |

过滤与树视觉 API：

| API | 类型 | 语义 |
| --- | --- | --- |
| `Filter` | `IValueFilter?` | 树节点过滤器；为空时初始化为 contains 过滤。 |
| `FilterValueSelector` | `DefaultFilterValueSelector?` | 从节点中提取过滤文本。 |
| `FilterStrategy` | `TreeFilterStrategy` | 过滤后的高亮、加粗、展开路径和隐藏不匹配策略。 |
| `FilterHighlightForeground` | `IBrush?` | 过滤命中高亮前景色。 |
| `AutoScrollToSelectedItem` | `bool` | 候选树打开或同步时滚动到选中节点。 |
| `IsShowIcon` / `IsShowLeafIcon` | `bool` | 是否显示节点图标和叶子节点图标。 |
| `IsShowLine` / `IsShowTreeLine` | `bool` | 是否显示层级连线。 |
| `IsSwitcherRotation` | `bool` | 展开图标是否旋转。 |

稳定 template part：

| Template Part | 类型 | 职责 |
| --- | --- | --- |
| `PART_AddOnDecoratedBox` | `TreeSelectAddOnDecoratedBox` | 输入壳体、Addon、variant、status、CompactSpace 和多选状态承载。 |
| `PART_SingleFilterInput` | `SelectFilterTextBox` | 单选模式结果显示和搜索输入。 |
| `SelectedItemsBox` | `SelectTagAwareTextBox` | 多选和勾选模式已选 tag 展示。 |
| `PART_SelectMaxCountIndicator` | `SelectMaxCountIndicator` | 最大选择数量提示。 |
| `PART_ContentRightAddOnPresenter` | `ContentPresenter` | 用户内部右侧内容承载。 |
| `PART_SelectHandle` | `SelectHandle` | 展开、loading、清除和 Form feedback 图标入口。 |
| `PART_Popup` | `Popup` | 候选弹层宿主。 |
| `PopupFrame` | `Border` | 懒创建的候选弹层外壳。 |
| `PART_TreeView` | `TreeSelectTreeView` | 懒创建的树候选控件。 |

## 4. 行为与状态模型

TreeSelect 的核心状态流：

```text
ItemsSource / Items
      ↓
TreeSelectTreeView
      ↓
SelectedItem / SelectedItems
      ↓
SingleFilterInput / SelectedItemsBox
      ↓
Form value + MaxCount state
```

选择模式语义：

- 单选模式使用 `SelectedItem` 作为表单值，候选节点点击后关闭弹层。
- 多选模式使用 `SelectedItems` 作为表单值，候选树使用多选 selection，并通过 tag 展示结果。
- `IsTreeCheckable=true` 使用 checkbox 作为节点切换入口，TreeView selection 不再作为主要选择入口。
- `ShowCheckedStrategy` 只影响多选 tag 展示集合，不改变 `SelectedItems` 的真实值。

过滤行为：

- `FilterValue` 来自单选搜索输入。
- `FilterStrategy` 控制命中高亮、加粗、展开路径和隐藏不匹配节点。
- 弹层关闭后，单选模式清空过滤值并恢复结果显示。

清除行为通过 `SelectHandle.ClearRequestedEvent` 冒泡到 TreeSelect，并调用 `Clear()` 清空当前选择。

## 5. 视觉与主题模型

TreeSelect 的默认视觉由 TreeSelect 专属主题、Select 家族输入壳体、TreeView 和 PopupHost 协作完成。

| 主题或资源 | 职责 |
| --- | --- |
| `TreeSelectTheme.axaml` | 根模板、输入壳体、单选输入、多选结果区域、handle、popup 和基础 selector。 |
| `SelectTagAwareTextBox` / `SelectTag` 主题 | 多选 tag 布局、高度、关闭按钮和禁用态。 |
| `TreeView` / `TreeViewItem` 主题 | 树节点缩进、展开、勾选、图标、连线和过滤视觉。 |
| `AddOnDecoratedBoxToken` | 输入壳体边框、圆角、状态、focus 和 CompactSpace 视觉。 |
| `PopupHostToken` | popup 阴影、圆角和 anchor margin。 |
| `TreeSelectToken` | TreeSelect 候选弹层最小宽度。 |
| `SelectToken` | TreeSelect 复用的 popup padding、多选 tag 和输入内容 padding。 |

右侧 count、content add-on 和 handle 的稳定 template part 状态由 AXAML compiled ancestor binding 表达。C# 中只保留 AddOnDecoratedBox hover / pressed 到 SelectHandle 的 sibling 状态转发，因为该关系不是 templated parent 绑定，不能用 `TemplateBinding` 表达。

## 6. 控件家族或集成关系

TreeSelect 属于 Data Entry 选择控件家族，与 Select、Cascader、DatePicker、TimePicker 等共享输入尺寸、variant、status、Addon、Form 和 CompactSpace 语义。

集成关系：

- `AbstractSelect`：Select 家族输入壳体、弹层、Form、CompactSpace、Motion 和状态契约。
- `TreeView`：树节点容器、展开、勾选、过滤和异步加载基础能力。
- `TreeSelectTreeView`：TreeSelect 专用 TreeView，负责最大选择数状态下的节点容器协作。
- `TreeViewSelectTreeViewItem`：候选树容器，接收最大选择数状态。
- `SelectHandle`：右侧操作入口，负责展开指示、loading、清除和 Form feedback。
- `IFormItemAware` / `IFormItemFeedbackAware`：将选择值、校验状态和 feedback 接入 Form。
- `ICustomizableSizeTypeAware`：接入支持 `Custom` 的输入尺寸模型。

## 7. 兼容性不变量

维护 TreeSelect 时必须保持以下不变量：

- 单选模式使用 `SelectedItem`，多选和勾选模式使用 `SelectedItems`。
- `IsTreeCheckable=true` 必须继续把 TreeSelect 归入多选结果模型。
- `ShowCheckedStrategy` 只能影响 `EffectiveSelectedItems`，不能改写真实 `SelectedItems`。
- `ItemsSource` 变化必须尽量按节点路径 identity 保留已有选择。
- `Clear()` 必须同时清空单选和多选状态。
- Popup 打开、关闭和 light-dismiss 语义必须继续由 `AbstractSelect` 管理。
- 重新套用模板或 detach 时必须释放旧 popup 内容、TreeView 事件订阅和模板 part relay binding。
- 右侧稳定 template part 优先使用 AXAML binding，不能把可静态表达的绑定重新搬回 C#。
- `SizeType=Custom` 必须继续以 `Middle` 作为未显式覆盖时的默认视觉基线。
- Template part、Token 名称、伪类和主题 selector 不得在未授权情况下重命名或删除。

## 8. 专项模型

### 8.1 勾选展示策略

`TreeSelectCheckedStrategy` 是多选结果展示策略：

- `All`：展示所有已选节点。
- `ShowParent`：当父节点下所有子节点都被选中时，结果展示父节点，隐藏对应子孙节点。
- `ShowChild`：只展示叶子节点。

该策略作用于内部 `EffectiveSelectedItems`，最终由 `SelectedItemsBox` 展示，不改变 `SelectedItems` 的表单值。

### 8.2 路径 identity

TreeSelect 使用节点的 `ItemKey` 或 `Value` 组成 `TreeNodePath`，用于 `ItemsSource` 替换后的选择重映射。节点缺少稳定 identity 时，控件无法可靠地跨数据源替换保留选择。

## 9. 文档导航、LLMS 导出与验证策略

- [TreeSelect 桌面版实现原理](implementation.md)
- [TreeSelect Token 设计](token.md)
- [TreeSelect Changelog](changelog.md)

验证要求：

| 改动类型 | 验证要求 |
| --- | --- |
| Public API 或 Form 映射 | 验证单选、多选、勾选和 `Clear()` 行为。 |
| 选择同步 | 验证 TreeView selection / checked items 与 `SelectedItem` / `SelectedItems` 双向同步。 |
| 过滤行为 | 验证单选搜索输入、过滤策略、弹层关闭恢复和 placeholder 状态。 |
| AXAML 或 template part | 验证右侧 AddOn/count/handle binding、hover/pressed relay、popup 内容和 Gallery 示例。 |
| Token 或尺寸 | 验证 Large / Middle / Small / Custom、tag 高度、popup padding 和最小宽度。 |
| 文档改动 | 运行 `git diff --check`，检查文档链接存在。 |

LLMS 语义区域：

| Part | AtomUI 节点 | 职责 | 相关 API | 相关 Token | 稳定性 |
| --- | --- | --- | --- | --- | --- |
| `root` | `TreeSelect` | 数据录入控件根语义区域，承载 public API、值状态、验证状态和主题入口。 | 见 API 与契约模型 | 见视觉与主题模型 | stable |
| `input` | `输入或编辑区域` | 承载用户输入、当前值、占位、格式化或只读状态。 | 见 API 与契约模型 | 见视觉与主题模型 | stable |
| `trigger` | `触发区域` | 承载清除、展开、提交、步进、上传或辅助操作。 | 见 API 与契约模型 | 见视觉与主题模型 | stable |
| `popup` | `弹层或候选区域` | 承载下拉、候选项、日历、颜色面板或异步内容。 | 见 API 与契约模型 | 见视觉与主题模型 | stable |
| `validation` | `校验反馈区域` | 承载 Form、status、错误、警告、help 或 loading 状态。 | 见 API 与契约模型 | 见视觉与主题模型 | stable |

LLMS 导出来源：

| LLMS 内容 | 来源 | 说明 |
| --- | --- | --- |
| 单控件完整文档 | `overview.md` + Gallery API / Token / ShowCase | 生成 `controls/tree-select/index-cn.md` |
| 单控件语义文档 | `overview.md` + `implementation.md` + theme/template 信息 | 生成 `controls/tree-select/semantic-cn.md` |
| API 表 | Gallery ApiDataGrid 或源码 public surface | 不在 `overview.md` 中复制完整 API 表 |
| Design Token 表 | Gallery DesignTokenDataGrid、Token 类型或第 5 节主题模型 | 不在生成产物中手工维护第二份 Token 表 |
| 示例 | Gallery ShowCase + source snippet catalog | 只引用稳定示例 |
| 源码索引 | `implementation.md` | 用于定位控件源码、主题和测试 |
