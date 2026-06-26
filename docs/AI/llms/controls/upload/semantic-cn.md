# Upload 语义结构

> 生成产物：由源文档生成，不要手工编辑。修改内容请回到控件文档、Gallery API / Token 表、Gallery ShowCase 或源码结构。

## Semantic Parts

| Part | AtomUI 节点 | 职责 | 相关 API | 相关 Token | 稳定性 |
| --- | --- | --- | --- | --- | --- |
| `root` | `Upload` | 数据录入控件根语义区域，承载 public API、值状态、验证状态和主题入口。 | 见 API 与契约模型 | 见视觉与主题模型 | stable |
| `input` | `输入或编辑区域` | 承载用户输入、当前值、占位、格式化或只读状态。 | 见 API 与契约模型 | 见视觉与主题模型 | stable |
| `trigger` | `触发区域` | 承载清除、展开、提交、步进、上传或辅助操作。 | 见 API 与契约模型 | 见视觉与主题模型 | stable |
| `popup` | `弹层或候选区域` | 承载下拉、候选项、日历、颜色面板或异步内容。 | 见 API 与契约模型 | 见视觉与主题模型 | stable |
| `validation` | `校验反馈区域` | 承载 Form、status、错误、警告、help 或 loading 状态。 | 见 API 与契约模型 | 见视觉与主题模型 | stable |

## Abstract AXAML Structure

来源：`src/AtomUI.Desktop.Controls/Upload/Themes/UploadTheme.axaml`

```xml
<DockPanel Name="RootLayout">
    <UploadTriggerContent Name="PART_TriggerContent" />
    <UploadList Name="PART_UploadList" />
</DockPanel>
```

## Composition Model

该章节由控件 `Themes/` 文件夹中的真实主题文件生成，用于说明 public 控件与内部协作对象之间的运行时结构。内部节点只用于理解和维护，不应指导用户代码直接依赖。

### 控件角色图

```text
Upload
  -> UploadDefaultDropArea (control theme, UploadDefaultDropAreaTheme.axaml)
     -> DashedBorder#Frame (template-stable)
        -> StackPanel (template-stable)
           -> IconPresenter#IconPresenter (internal-observable)
           -> ContentPresenter#HeaderContentPresenter (internal-observable)
           -> ContentPresenter#SubHeaderContentPresenter (internal-observable)
  -> UploadList (control theme, UploadListTheme.axaml)
     -> Border#Frame (template-stable)
        -> ItemsPresenter#PART_ItemsPresenter (template-stable)
  -> Upload (control theme, UploadTheme.axaml)
     -> DockPanel#RootLayout (template-stable)
        -> UploadTriggerContent#PART_TriggerContent (template-stable)
        -> UploadList#PART_UploadList (template-stable)
     -> UploadPictureShapeList#PART_UploadList (template-stable)
  -> UploadTriggerContent (control theme, UploadTriggerContentTheme.axaml)
     -> DashedBorder#TriggerContentFrame (template-stable)
        -> ContentPresenter#PART_Trigger (template-stable)
```

### 协作节点

| 节点 | 类型 | 来源 | 生命周期 owner | 影响的 public API | 稳定性 | Agent 使用边界 |
| --- | --- | --- | --- | --- | --- | --- |
| `Upload` | public control | `源文档 + public API` | 用户代码 / 控件宿主 | public API | public | 用户可直接使用 public 控件；可作为示例和 API 入口。 |
| `UploadDefaultDropArea` | control theme | `UploadDefaultDropAreaTheme.axaml` | 用户代码 / 控件宿主 | `Background`, `BorderBrush`, `BorderThickness`, `CornerRadius`, `DropIcon`, `Header` | public | 用户可直接使用 public 控件；可作为示例和 API 入口。 |
| `Frame` | template node (DashedBorder) | `UploadDefaultDropAreaTheme.axaml` | UploadDefaultDropArea | `Background`, `BorderBrush`, `BorderThickness`, `CornerRadius`, `DropIcon`, `Header` | template-stable | 用于主题维护；变更需同步主题、实现和 LLMS。 |
| `StackPanel` | template node (StackPanel) | `UploadDefaultDropAreaTheme.axaml` | UploadDefaultDropArea | `DropIcon`, `Header`, `HeaderTemplate`, `SubHeader`, `SubHeaderTemplate` | template-stable | 用于主题维护；变更需同步主题、实现和 LLMS。 |
| `IconPresenter` | template node (IconPresenter) | `UploadDefaultDropAreaTheme.axaml` | UploadDefaultDropArea | `DropIcon` | internal-observable | 用于理解结构和状态流，不应指导用户代码直接依赖。 |
| `HeaderContentPresenter` | template node (ContentPresenter) | `UploadDefaultDropAreaTheme.axaml` | UploadDefaultDropArea | `Header`, `HeaderTemplate` | internal-observable | 用于理解结构和状态流，不应指导用户代码直接依赖。 |
| `SubHeaderContentPresenter` | template node (ContentPresenter) | `UploadDefaultDropAreaTheme.axaml` | UploadDefaultDropArea | `SubHeader`, `SubHeaderTemplate` | internal-observable | 用于理解结构和状态流，不应指导用户代码直接依赖。 |
| `UploadList` | control theme | `UploadListTheme.axaml` | Upload | `CornerRadius`, `ItemsPanel`, `Padding` | internal-observable | 用于理解结构和状态流，不应指导用户代码直接依赖。 |
| `Frame` | template node (Border) | `UploadListTheme.axaml` | UploadList | `CornerRadius`, `ItemsPanel`, `Padding` | template-stable | 用于主题维护；变更需同步主题、实现和 LLMS。 |
| `PART_ItemsPresenter` | template node (ItemsPresenter) | `UploadListTheme.axaml` | UploadList | `ItemsPanel` | template-stable | 用于主题维护；变更需同步主题、实现和 LLMS。 |
| `Upload` | control theme | `UploadTheme.axaml` | 用户代码 / 控件宿主 | `Content`, `ContentTemplate`, `CurrentTaskList`, `HorizontalContentAlignment`, `IsMotionEnabled`, `IsShowUploadList` | public | 用户可直接使用 public 控件；可作为示例和 API 入口。 |
| `RootLayout` | template node (DockPanel) | `UploadTheme.axaml` | Upload | `Content`, `ContentTemplate`, `CurrentTaskList`, `HorizontalContentAlignment`, `IsMotionEnabled`, `IsShowUploadList` | template-stable | 用于主题维护；变更需同步主题、实现和 LLMS。 |
| `PART_TriggerContent` | template node (UploadTriggerContent) | `UploadTheme.axaml` | Upload | `Content`, `ContentTemplate`, `HorizontalContentAlignment`, `IsMotionEnabled`, `IsShowUploadTrigger`, `ListType` | template-stable | 用于主题维护；变更需同步主题、实现和 LLMS。 |
| `PART_UploadList` | template node (UploadList) | `UploadTheme.axaml` | Upload | `CurrentTaskList`, `IsMotionEnabled`, `IsShowUploadList`, `ListType` | template-stable | 用于主题维护；变更需同步主题、实现和 LLMS。 |
| `PART_UploadList` | template node (UploadPictureShapeList) | `UploadTheme.axaml` | Upload | `Content`, `ContentTemplate`, `CurrentTaskList`, `IsMotionEnabled`, `IsShowUploadList`, `IsShowUploadTrigger` | template-stable | 用于主题维护；变更需同步主题、实现和 LLMS。 |
| `UploadTriggerContent` | control theme | `UploadTriggerContentTheme.axaml` | Upload | `Background`, `BorderBrush`, `BorderThickness`, `Content`, `ContentTemplate`, `CornerRadius` | internal-observable | 用于理解结构和状态流，不应指导用户代码直接依赖。 |
| `TriggerContentFrame` | template node (DashedBorder) | `UploadTriggerContentTheme.axaml` | UploadTriggerContent | `Background`, `BorderBrush`, `BorderThickness`, `Content`, `ContentTemplate`, `CornerRadius` | template-stable | 用于主题维护；变更需同步主题、实现和 LLMS。 |
| `PART_Trigger` | template node (ContentPresenter) | `UploadTriggerContentTheme.axaml` | UploadTriggerContent | `Content`, `ContentTemplate`, `HorizontalContentAlignment`, `VerticalContentAlignment` | template-stable | 用于主题维护；变更需同步主题、实现和 LLMS。 |

## Template Parts

| 契约组 | 代表成员 | 维护含义 |
| --- | --- | --- |
| 内容与数据 | `DropIcon`、`FileName`、`FilePath`、`Header`、`HeaderTemplate`、`IsImageFile`、`IsOpenFileDialogOnClick`、`IsShowUploadList`、`IsShowUploadTrigger`、`IsUploadDirectoryEnabled` 等 15 项 | 定义控件展示内容、输入数据、模板或业务对象入口。 |
| 选择与集合 | `IsMultipleEnabled`、`MaxCount` | 维护选择、展开、过滤、分页、分组或集合状态。 |
| 交互与状态 | `IsMotionEnabled`、`IsTaskRunning`、`Status` | 表达用户可观察状态、可用性、清除、加载或反馈语义。 |
| 动效与异步 | `MaxConcurrentTasks`、`Progress`、`TaskId` | 约束动效开关、异步加载、播放速度、超时和任务边界。 |
| 其他稳定入口 | `ErrorMessage`、`ExtraContext`、`ListType` | 保留为 public surface，变更前需确认 Gallery 和用户 XAML 依赖。 |

## Pseudo Classes

| 状态反馈 | public API、内部状态和伪类如何形成用户可感知反馈。 | open/close、loading/async、motion。 |
| 主题语义 | ControlTheme、SharedToken、组件 Token 和模板绑定如何表达视觉。 | Upload Token + ControlTheme。 |

## State Flow

Upload 的状态流按以下路径收敛：

```text
Public API / inherited command / item source / user input
  -> 控件实例状态
  -> effective state / pseudo-class / template property
  -> ControlTheme selector / presenter / renderer
  -> Gallery 可观察行为
```

状态维护规则：

- Disabled 或不可交互状态优先屏蔽 pointer、keyboard、motion 和提交类反馈。
- open/close、loading/async、motion 状态由控件实例或明确的数据 owner 推导，不能在 template part 之间双向竞争。
- 模板重套用时必须把 public API 对应状态回放到新的 part、伪类和主题变量。
- 集合、弹层、异步、动效或窗口相关状态必须能处理 reset、close、cancel、detach 和 owner 释放。

## Theme and Token Boundaries

Upload 的视觉模型由控件模板、ControlTheme、SharedToken 和必要的组件 Token 共同构成。

| 主题文件 | 职责 |
| --- | --- |
| `AbstractUploadPictureContentTheme.axaml` | 提供控件模板、selector、资源绑定和状态视觉。 |
| `UploadTextListItemHeaderTheme.axaml` | 定义集合项、容器项或局部单元的状态视觉。 |
| `UploadTextListItemTheme.axaml` | 定义集合项、容器项或局部单元的状态视觉。 |
| `UploadPictureDefaultContentTheme.axaml` | 提供控件模板、selector、资源绑定和状态视觉。 |
| `UploadPictureListItemTheme.axaml` | 定义集合项、容器项或局部单元的状态视觉。 |
| `UploadPicturePendingContentTheme.axaml` | 提供控件模板、selector、资源绑定和状态视觉。 |
| `UploadPicturePreviewContentTheme.axaml` | 提供控件模板、selector、资源绑定和状态视觉。 |
| `UploadPictureUploadingContentTheme.axaml` | 提供控件模板、selector、资源绑定和状态视觉。 |
| `UploadPictureShapeDefaultContentTheme.axaml` | 提供控件模板、selector、资源绑定和状态视觉。 |
| `UploadPictureShapeListItemTheme.axaml` | 定义集合项、容器项或局部单元的状态视觉。 |
| `UploadPictureShapeListTheme.axaml` | 提供控件模板、selector、资源绑定和状态视觉。 |
| `UploadPictureShapePendingContentTheme.axaml` | 提供控件模板、selector、资源绑定和状态视觉。 |
| `UploadPictureShapePreviewContentTheme.axaml` | 提供控件模板、selector、资源绑定和状态视觉。 |
| `UploadPictureShapeUploadingContentTheme.axaml` | 提供控件模板、selector、资源绑定和状态视觉。 |
| `UploadDefaultDropAreaTheme.axaml` | 提供控件模板、selector、资源绑定和状态视觉。 |
| `UploadListTheme.axaml` | 提供控件模板、selector、资源绑定和状态视觉。 |
| `UploadTheme.axaml` | 提供控件模板、selector、资源绑定和状态视觉。 |
| `UploadThemes.axaml` | 聚合控件家族主题资源，保证包级引入顺序稳定。 |
| `UploadTriggerContentTheme.axaml` | 提供控件模板、selector、资源绑定和状态视觉。 |

Upload 使用 `UploadToken` 作为组件 Token scope。Token 只表达组件视觉语义，不承载 open/close、loading/async、motion 运行时状态。

主题维护规则：

- 不删除或重命名已经稳定的 ControlTheme key、template part、伪类和资源 key。
- 不把可由 AXAML 表达的模板状态迁移为 C# 动态创建视觉。
- 不把 hover、pressed、selected、expanded、loading、filter、popup open 等运行时状态写入 Token。
- Browser 或平台特化主题必须保持同一 API 的语义一致。

Token 边界：

Upload Token 只表达组件级视觉变量，例如尺寸、间距、颜色、圆角、阴影、图标尺寸和弹层边界。Token 不承载运行时选择、展开、加载、错误、上传任务、过滤条件或业务状态。

当前 Token scope：

- `UploadToken`，scope id 为 `Upload`，源码位于 `src/AtomUI.Desktop.Controls/Upload/UploadToken.cs`。

## Customization Boundaries

维护 Upload 时必须保持以下不变量：

- 不擅自新增、删除、重命名或改变 public/protected API、Avalonia 属性、事件和默认值。
- 不破坏 template part、伪类、ControlTheme key、Token 名称和资源 key。
- 不改变 Gallery 已展示的 XAML 用法、默认外观、交互顺序和状态优先级。
- Template part 重新应用、集合替换、弹层关闭、窗口失活和控件 detach 时必须释放旧订阅和资源宿主。
- 不通过隐藏延迟、强制刷新或吞异常掩盖状态同步问题。
- 不引入运行时反射扫描作为 API、Token 或数据路径发现机制。
- 文档只描述当前稳定设计；历史变化记录在 `changelog.md`。

维护不变量：

维护 Upload 时不得破坏：

- Public API、默认值、事件顺序和 Gallery 可观察行为。
- Template part 名称、ControlTheme key、伪类和资源 key。
- 旧 template part、事件订阅、Popup/Flyout/Window host 和 collection view 的释放路径。
- Light/Dark、Browser/Desktop 和不同 SizeType 下的主题一致性。
- 文档、Gallery API 表、Token 表与源码契约的一致性。
