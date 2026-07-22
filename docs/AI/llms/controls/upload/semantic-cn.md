# Upload 语义结构

> 生成产物：由源文档生成，不要手工编辑。修改内容请回到控件文档、源码 public surface、Token 类型或生成数据、Gallery ShowCase 或源码结构。

## Semantic Parts

| Part | AtomUI 节点 | 职责 | 相关 API | 相关 Token | 稳定性 |
| --- | --- | --- | --- | --- | --- |
| `root` | `Upload` | 上传状态协调器，拥有文件集合、上传队列、Form 值投影和生命周期。 | `Files`、`UploadTransport`、`FileValueMode` | `UploadToken`、SharedToken | stable |
| `trigger` | `TriggerContent` / `UploadTrigger` | 承载文件或目录选择入口，只提交选择动作，不持有上传状态。 | `TriggerContent`、`SourceKind`、`SelectFilesAsync()`、`SelectDirectoriesAsync()` | Upload trigger 主题资源 | stable |
| `drop-zone` | `UploadDropZone` | 接收拖拽文件并提交给最近的 `Upload`。 | `EnqueueFilesAsync()` | Upload drop-zone 主题资源 | stable |
| `list` | `UploadList` | 渲染 `Files` 并拥有列表滚动边界，不创建第二份文件状态。 | `Files`、`ListType`、`ListMaxHeight`、`ListScrollBarVisibility` | Upload list 主题资源 | internal-observable |
| `item` | `AbstractUploadListItem` 派生容器 | 投射单个 `UploadFileItem` 的状态、进度和操作入口。 | `UploadFileItem.Status`、`Progress`、`ErrorMessage`、`Result` | Upload item 主题资源 | internal-observable |
| `validation` | `Upload` Form / validation 投影 | 按 `FileValueMode` 输出 Form 值，并把错误投射到 `DataValidationErrors`。 | `FileValueMode`、`IFormItemAware` | SharedToken、Form Token | stable |

## Abstract AXAML Structure

来源：`src/AtomUI.Desktop.Controls/Upload/Themes/UploadTheme.axaml`

```xml
<StackPanel Name="RootLayout">
    <ContentPresenter Name="PART_TriggerContent" />
    <UploadList Name="PART_UploadList" />
</StackPanel>
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
  -> UploadDropZone (control theme, UploadDropZoneTheme.axaml)
     -> ContentPresenter#PART_ContentPresenter (template-stable)
  -> UploadList (control theme, UploadListTheme.axaml)
     -> Border#Frame (template-stable)
        -> ScrollViewer (template-stable)
           -> ItemsPresenter#PART_ItemsPresenter (template-stable)
  -> Upload (control theme, UploadTheme.axaml)
     -> StackPanel#RootLayout (template-stable)
        -> ContentPresenter#PART_TriggerContent (template-stable)
        -> UploadList#PART_UploadList (template-stable)
     -> StackPanel#RootLayout (template-stable)
        -> UploadPictureShapeList#PART_UploadList (template-stable)
  -> UploadTrigger (control theme, UploadTriggerTheme.axaml)
     -> ContentPresenter#PART_ContentPresenter (template-stable)
     -> DashedBorder#TriggerContentFrame (template-stable)
        -> ContentPresenter#PART_ContentPresenter (template-stable)
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
| `UploadDropZone` | control theme | `UploadDropZoneTheme.axaml` | 用户代码 / 控件宿主 | `Content`, `ContentTemplate`, `HorizontalContentAlignment`, `VerticalContentAlignment` | public | 用户可直接使用 public 控件；可作为示例和 API 入口。 |
| `PART_ContentPresenter` | template node (ContentPresenter) | `UploadDropZoneTheme.axaml` | UploadDropZone | `Content`, `ContentTemplate`, `HorizontalContentAlignment`, `VerticalContentAlignment` | template-stable | 用于主题维护；变更需同步主题、实现和 LLMS。 |
| `UploadList` | control theme | `UploadListTheme.axaml` | Upload | `CornerRadius`, `ItemsPanel`, `ListMaxHeight`, `ListScrollBarVisibility`, `Padding` | internal-observable | 用于理解结构和状态流，不应指导用户代码直接依赖。 |
| `Frame` | template node (Border) | `UploadListTheme.axaml` | UploadList | `CornerRadius`, `ItemsPanel`, `ListMaxHeight`, `ListScrollBarVisibility`, `Padding` | template-stable | 用于主题维护；变更需同步主题、实现和 LLMS。 |
| `PART_ItemsPresenter` | template node (ItemsPresenter) | `UploadListTheme.axaml` | UploadList | `ItemsPanel` | template-stable | 用于主题维护；变更需同步主题、实现和 LLMS。 |
| `Upload` | control theme | `UploadTheme.axaml` | 用户代码 / 控件宿主 | `EffectiveFiles`, `EffectivePictureItems`, `HorizontalContentAlignment`, `IsMotionEnabled`, `IsShowUploadList`, `ListMaxHeight` | public | 用户可直接使用 public 控件；可作为示例和 API 入口。 |
| `RootLayout` | template node (StackPanel) | `UploadTheme.axaml` | Upload | `EffectiveFiles`, `HorizontalContentAlignment`, `IsMotionEnabled`, `IsShowUploadList`, `ListMaxHeight`, `ListScrollBarVisibility` | template-stable | 用于主题维护；变更需同步主题、实现和 LLMS。 |
| `PART_TriggerContent` | template node (ContentPresenter) | `UploadTheme.axaml` | Upload | `HorizontalContentAlignment`, `TriggerContent`, `TriggerContentTemplate`, `VerticalContentAlignment` | template-stable | 用于主题维护；变更需同步主题、实现和 LLMS。 |
| `PART_UploadList` | template node (UploadList) | `UploadTheme.axaml` | Upload | `EffectiveFiles`, `IsMotionEnabled`, `IsShowUploadList`, `ListMaxHeight`, `ListScrollBarVisibility`, `ListType` | template-stable | 用于主题维护；变更需同步主题、实现和 LLMS。 |
| `PART_UploadList` | template node (UploadPictureShapeList) | `UploadTheme.axaml` | Upload | `EffectivePictureItems`, `IsMotionEnabled`, `IsShowUploadList`, `ListMaxHeight`, `ListScrollBarVisibility`, `ListType` | template-stable | 用于主题维护；变更需同步主题、实现和 LLMS。 |
| `UploadTrigger` | control theme | `UploadTriggerTheme.axaml` | 用户代码 / 控件宿主 | `Background`, `BorderBrush`, `BorderThickness`, `Content`, `ContentTemplate`, `CornerRadius` | public | 用户可直接使用 public 控件；可作为示例和 API 入口。 |
| `PART_ContentPresenter` | template node (ContentPresenter) | `UploadTriggerTheme.axaml` | UploadTrigger | `Content`, `ContentTemplate`, `HorizontalContentAlignment`, `VerticalContentAlignment` | template-stable | 用于主题维护；变更需同步主题、实现和 LLMS。 |
| `TriggerContentFrame` | template node (DashedBorder) | `UploadTriggerTheme.axaml` | UploadTrigger | `Background`, `BorderBrush`, `BorderThickness`, `Content`, `ContentTemplate`, `CornerRadius` | template-stable | 用于主题维护；变更需同步主题、实现和 LLMS。 |

## Template Parts

| 契约组 | 代表成员 | 维护含义 |
| --- | --- | --- |
| 文件状态 | `Files`、`UploadFileItem` | 唯一文件状态 owner，支持绑定、Form 投影和列表渲染。 |
| 文件选择 | `UploadTrigger`、`UploadSourceKind`、`SelectFilesAsync`、`SelectDirectoriesAsync` | 文件与目录选择是独立动作入口，不再由根控件 bool 互斥。 |
| 拖拽提交 | `UploadDropZone`、`EnqueueFilesAsync` | 拖拽区只提交文件，不保存列表状态。 |
| 上传队列 | `UploadTransport`、`AutoUpload`、`MaxConcurrentTasks`、`UploadQueue` | 上传调度与视觉控件解耦，生命周期由 `Upload` 统一释放。 |
| 列表展示 | `UploadList`、`ListType`、`ListMaxHeight`、`ListScrollBarVisibility` | 列表内部滚动，触发区保持固定。 |
| 触发入口 | `TriggerContent`、`UploadTrigger`、Picture append slot | 文件/目录触发器由用户布局组合，PictureCard/PictureCircle 通过显示源 append slot 呈现。 |
| 状态反馈 | `SuccessAutoRemoveDelay`、`PendingText`、`FileValueMode` | 成功自动移除、待上传文案和 Form 值投影可配置。 |
| 视觉与动效 | `IsMotionEnabled`、Upload Token | 只表达视觉状态，不保存业务任务状态。 |

## Pseudo Classes

| 状态反馈 | public API、内部状态和伪类如何形成用户可感知反馈。 | `UploadFileItem.Status`、`Progress`、`ErrorMessage`、`Result` 是任务状态来源；Form 错误走 `DataValidationErrors`。 |
| 主题语义 | ControlTheme、SharedToken、组件 Token 和模板绑定如何表达视觉。 | Upload Token + ControlTheme；滚动边界在 `UploadList` 内部，并使用 AtomUI 自动隐藏滚动条。 |

## State Flow

Upload 的状态流只允许按以下路径收敛：

```text
Public API / UploadTrigger / UploadDropZone
  -> Upload.EnqueueFilesAsync
  -> Files collection
  -> UploadQueue / FileUploadScheduler
  -> UploadFileItem.Status / Progress / Result
  -> UploadList item containers
  -> Gallery 可观察行为
```

状态维护规则：

- `Files` 是唯一文件状态 owner；实现中不得保留 `_allTaskList`、`TaskInfoList`、`CurrentTaskList` 或同类复制集合。
- `UploadQueue` 只负责把 `UploadFileItem` 映射到 `FileUploadTask` 并转发调度结果，不直接操作视觉容器。
- `UploadList` 只渲染 `Files`，不得创建、删除或隐藏真实任务状态。
- 文件选择和目录选择由 `UploadTrigger.SourceKind` 决定，可以在同一 `Upload` 下并存。
- `SuccessAutoRemoveDelay` 的延迟任务必须在 remove、reset、detach 和状态离开 success 时取消。
- Form 值由 `FileValueMode` 投影，错误状态以 Avalonia `DataValidationErrors` 为准。

## Theme and Token Boundaries

Upload 的视觉模型由控件模板、ControlTheme、SharedToken 和组件 Token 共同构成。

| 主题文件 | 职责 |
| --- | --- |
| `UploadTheme.axaml` | 根模板，连接 `TriggerContent`、list 和 picture display source。 |
| `UploadTriggerTheme.axaml` | 触发器 shell，只承载用户内容和点击动作，不硬编码 Button。 |
| `UploadDropZoneTheme.axaml` | 拖拽区域 shell，承载 drop 视觉和用户内容。 |
| `UploadListTheme.axaml` | 上传列表 shell，内部拥有自动隐藏的 `atom:ScrollViewer` 和滚动边界。 |
| `UploadTextListItemTheme.axaml` | Text 列表项状态视觉。 |
| `UploadTextListItemHeaderTheme.axaml` | Text 列表项头部状态视觉。 |
| `UploadPictureListItemTheme.axaml` | Picture 列表项状态视觉。 |
| `UploadPicturePendingContentTheme.axaml` | Picture pending 内容，优先显示 `UploadFileItem.PendingText`。 |
| `UploadPicturePreviewContentTheme.axaml` | Picture preview 内容。 |
| `UploadPictureUploadingContentTheme.axaml` | Picture uploading 内容。 |
| `UploadPictureDefaultContentTheme.axaml` | Picture fallback 内容。 |
| `UploadPictureShapeListTheme.axaml` | PictureCard/PictureCircle 列表布局。 |
| `UploadPictureShapeListItemTheme.axaml` | PictureCard/PictureCircle 列表项状态视觉。 |
| `UploadPictureShapePendingContentTheme.axaml` | Shape pending 内容，优先显示 `UploadFileItem.PendingText`。 |
| `UploadPictureShapePreviewContentTheme.axaml` | Shape preview 内容。 |
| `UploadPictureShapeUploadingContentTheme.axaml` | Shape uploading 内容。 |
| `UploadPictureShapeDefaultContentTheme.axaml` | Shape fallback 内容。 |
| `UploadThemes.axaml` | 聚合控件家族主题资源，保证包级引入顺序稳定。 |

主题维护规则：

- Trigger 不作为文件项渲染，PictureCard/PictureCircle 使用不进入 `Files` 的 display append slot 保持同一 wrap flow。
- Text/Picture 根模板必须在 trigger 与 list 之间保留 Shared spacing，避免触发按钮和第一条文件项贴在一起。
- 滚动区域只包裹列表，不包裹 trigger。
- 可由 AXAML 表达的模板状态必须优先留在 AXAML。
- Token 只表达视觉变量，不承载上传状态、队列状态或 Form 错误。

Token 边界：

Upload Token 只表达组件级视觉变量，例如尺寸、间距、颜色、圆角、阴影、图标尺寸和弹层边界。Token 不承载运行时选择、展开、加载、错误、上传任务、过滤条件或业务状态。

当前 Token scope：

- `UploadToken`，scope id 为 `Upload`，源码位于 `src/AtomUI.Desktop.Controls/Upload/UploadToken.cs`。

## Customization Boundaries

本次重构是 L3 breaking change。维护 Upload 时必须保持以下新不变量：

- 不重新引入 `TaskInfoList`、`DefaultTaskList`、`CurrentTaskList` 或 fake trigger task。
- 不让视觉容器反向持有业务任务状态。
- 不用延时、强制刷新或 suppression flag 掩盖状态不同步。
- Template reapply、集合替换、remove、reset、detach 都必须释放旧订阅、取消运行任务和取消 pending auto-remove。
- 不通过运行时反射扫描 public API、Token 或 Gallery 表格数据。
- 文档只描述当前目标设计；历史变化记录在 `changelog.md`。

维护不变量：

维护 Upload 时不得破坏以下不变量：

- `Files` 是唯一文件状态 owner。
- `UploadTaskInfo`、`TaskInfoList`、`DefaultTaskList`、`CurrentTaskList` 和 fake picture trigger task 不得重新进入目标实现。
- trigger、drop-zone、list、item container 都不能保存第二份业务任务状态。
- PictureCard/PictureCircle 的上传入口只能通过 `EffectivePictureItems` 中的 display append slot 呈现，确保与图片项处于同一 wrap flow。
- `RemoveFileAsync`、`ResetAsync`、detach 必须释放上传任务、auto-remove delay、集合订阅和 container 绑定。
- `DataValidationErrors` 是 error 状态来源，Upload 不维护独立 error 机制。
- AXAML-first binding 是默认选择；C# binding 必须说明 AXAML 不能表达的原因和释放 owner。
- Gallery 示例、API 表、控件文档和测试必须使用同一套 public contract。
