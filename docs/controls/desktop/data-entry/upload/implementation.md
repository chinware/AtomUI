# Upload 桌面版实现原理

本文档定义 Upload 桌面版 L3 重构后的内部实现范围、源码职责、状态流、生命周期、资源边界和维护规则。公共设计与 API 契约见 [Upload 桌面版架构设计](overview.md)，变化记录见 [Upload Changelog](changelog.md)，组件视觉变量见 [Upload Token 设计](token.md)。

## 1. 实现定位

Upload 的实现定位是上传状态协调器，而不是固定上传按钮、拖拽框或列表模板的集合。根控件只拥有文件状态、上传队列、Form 值投影和生命周期释放；`UploadTrigger`、`UploadDropZone`、`UploadList` 只作为可组合动作入口或视图层协作对象。

本文档只记录维护者必须遵守的稳定实现结构和不变量。具体属性注册、AXAML selector、ControlTheme 节点和列表项视觉细节仍以源码为准。

## 2. 源码文件结构

目标源码结构按“状态 owner、动作入口、队列协调、列表视图、主题模板”分层。

| 路径 | 职责 |
| --- | --- |
| `src/AtomUI.Desktop.Controls/Upload/Upload.cs` | 保留 public/protected API、Avalonia 属性注册、构造、display source 桥接、`IFormItemAware` 和顶层上传协调。 |
| `src/AtomUI.Desktop.Controls/Upload/UploadAppendContentItem.cs` | internal picture append visual slot，用于把 `TriggerContent` 放入 PictureCard/PictureCircle 的同一 wrap flow；不进入 `Files`。 |
| `src/AtomUI.Desktop.Controls/Upload/Upload.FileSelection.cs` | 封装文件选择和目录选择动作，供 `UploadTrigger` 调用。 |
| `src/AtomUI.Desktop.Controls/Upload/Upload.AutoRemove.cs` | 管理成功自动移除的延迟任务、取消和生命周期释放。 |
| `src/AtomUI.Desktop.Controls/Upload/UploadFileItem.cs` | public 文件状态模型，承载文件元数据、状态、进度、错误、结果和 pending 文案。 |
| `src/AtomUI.Desktop.Controls/Upload/UploadSourceKind.cs` | public 触发来源枚举，区分文件选择和目录选择。 |
| `src/AtomUI.Desktop.Controls/Upload/UploadFileValueMode.cs` | public Form 值投影枚举，控制提交全部文件、成功文件或上传结果。 |
| `src/AtomUI.Desktop.Controls/Upload/UploadTrigger.cs` | public 可组合触发器，查找最近的 `Upload` 并触发文件或目录选择。 |
| `src/AtomUI.Desktop.Controls/Upload/UploadDropZone.cs` | public 可组合拖拽区，将 drop 文件提交给最近的 `Upload`。 |
| `src/AtomUI.Desktop.Controls/Upload/UploadQueue.cs` | internal 上传队列协调器，映射 `UploadFileItem` 与 `FileUploadTask`。 |
| `src/AtomUI.Desktop.Controls/Upload/UploadList.cs` | 文件列表视图，渲染 effective 文件视图并维护列表滚动。 |
| `src/AtomUI.Desktop.Controls/Upload/AbstractUploadListItem.cs` | 列表项基类，只绑定 `UploadFileItem` 并发出 item action，不拥有任务状态。 |
| `src/AtomUI.Controls.Shared/Net/FileUploadScheduler.cs` | 共享上传调度器，跟踪 pending/running 任务、并发和取消。 |
| `src/AtomUI.Controls.Shared/Net/FileUploadTask.cs` | 共享上传任务对象，承载传输回调和 cancellation 状态。 |

主题结构：

| 路径 | 职责 |
| --- | --- |
| `src/AtomUI.Desktop.Controls/Upload/Themes/UploadTheme.axaml` | 根模板，连接 `TriggerContent`、list 和 picture display source，并保持触发区与列表的稳定间距。 |
| `src/AtomUI.Desktop.Controls/Upload/Themes/UploadTriggerTheme.axaml` | 触发器 shell，只承载用户内容和点击表面。 |
| `src/AtomUI.Desktop.Controls/Upload/Themes/UploadDropZoneTheme.axaml` | 拖拽区 shell，承载 drag/drop 视觉和内容。 |
| `src/AtomUI.Desktop.Controls/Upload/Themes/UploadListTheme.axaml` | 列表 shell，内部拥有自动隐藏的 `atom:ScrollViewer` 和滚动边界。 |
| `src/AtomUI.Desktop.Controls/Upload/Themes/PictureList/*` | Picture 列表项视觉，pending 内容优先读取 `UploadFileItem.PendingText`。 |
| `src/AtomUI.Desktop.Controls/Upload/Themes/PictureShapeList/*` | PictureCard/PictureCircle 列表布局和 item 视觉；append slot 由 display source 承载并进入同一 wrap flow。 |
| `src/AtomUI.Desktop.Controls/Upload/Themes/UploadThemes.axaml` | 聚合 Upload 家族主题，保证资源引入顺序稳定。 |

## 3. 核心类职责

| 类型 | 稳定职责 | 禁止职责 |
| --- | --- | --- |
| `Upload` | `Files` 状态 owner、上传协调、Form 适配、生命周期释放、trigger/drop/list 组合入口。 | 不创建第二份任务列表，不把 trigger 伪造成文件项。 |
| `UploadFileItem` | public 文件状态模型，保存文件元数据、上传状态、进度、错误、结果和自定义数据。 | 不持有视觉控件、队列、transport 或 owner 控件。 |
| `UploadQueue` | 管理 `UploadFileItem.Id` 与 `FileUploadTask` 的映射，转发 scheduler 状态到文件项。 | 不操作模板 part、列表容器或 Gallery 视图。 |
| `UploadTrigger` | 根据 `SourceKind` 调用最近 `Upload` 的文件或目录选择方法。 | 不保存文件集合，不直接打开业务上传 transport。 |
| `UploadDropZone` | 处理 drag/drop 交互并调用最近 `Upload.EnqueueFilesAsync`。 | 不独立维护 drop 后的任务状态。 |
| `UploadList` | 渲染 effective 文件视图，维护 `ListMaxHeight` 和 `ListScrollBarVisibility`；Picture shape 派生列表可追加 display-only visual slot。 | 不创建、删除、隐藏真实上传文件状态。 |
| `AbstractUploadListItem` | 从 `UploadFileItem` 投射 item 视觉状态并发出 remove/preview 等动作请求。 | 不反向持有 scheduler task 或复制 item 状态。 |
| `FileUploadScheduler` | 共享并发调度、pending/running 跟踪、取消和 transport 替换。 | 不引用 `Upload`、`UploadList` 或其他视觉控件。 |

## 4. 状态与数据流

Upload state flows through one path:

```text
Public API / Trigger / DropZone
  -> Upload.EnqueueFilesAsync
  -> Files collection
  -> UploadQueue/FileUploadScheduler
  -> UploadFileItem.Status/Progress/Result
  -> UploadList item containers
```

实现中只能有一个文件状态 owner：

- `Upload.Files` 是唯一 public 文件列表入口。
- `Files` 为 `null` 时，`Upload` 可以使用内部 fallback collection，但该 collection 仍是唯一 effective 文件集合。
- 绑定场景下，`ResetAsync` 和 Form 写入必须清空或更新集合内容，不替换外部绑定的集合实例。
- 禁止保留 `_allTaskList`、`TaskInfoList`、`CurrentTaskList` 或同类复制集合。
- 禁止从 `UploadList`、item container 或 picture trigger 反向创建业务任务状态。

队列状态只在 `UploadQueue` 和 `FileUploadScheduler` 内部存在：

- `UploadQueue` 通过 `UploadFileItem.Id` 查找对应 `FileUploadTask`。
- scheduler 回调只更新对应 `UploadFileItem.Status`、`Progress`、`ErrorMessage` 和 `Result`。
- 文件移除、reset、detach 必须先取消相关队列工作，再更新 `Files`。
- `MaxConcurrentTasks` 和 `UploadTransport` 变更必须经由 queue/scheduler 统一处理。

Form 与验证状态：

- `FileValueMode` 控制 `IFormItemAware.GetFormValue` 的投影结果。
- `IFormItemAware.SetFormValue` 接收 `IEnumerable<UploadFileItem>` 并更新 `Files` 内容。
- `IFormItemAware.ClearFormValue` 调用 reset 语义，确保任务与自动移除延迟同步取消。
- error 状态以 Avalonia `DataValidationErrors` 为准，不新增 Upload 专用 error owner。

## 5. 组合结构模型

### 控件角色图

```text
Upload (public coordinator)
  -> TriggerContent slot
     -> UploadTrigger (public action entry)
     -> UploadDropZone (public drop entry)
  -> UploadList / UploadPictureShapeList (public/internal-observable list view)
     -> atom:ScrollViewer (template-stable auto-hide list scroll boundary)
        -> ItemsPresenter#PART_ItemsPresenter (template-stable)
           -> AbstractUploadListItem derived container (internal-observable)
           -> UploadAppendContentItem visual slot (picture display source only)
```

### 协作节点

| 节点 | 类型 | 来源 | 生命周期 owner | 影响的 public API | 稳定性 | Agent 使用边界 |
| --- | --- | --- | --- | --- | --- | --- |
| `Upload` | 控件 | `Upload.cs` | 自身 | `Files`、`TriggerContent`、`UploadTransport`、`FileValueMode` | public | 可作为用户 API 和实现入口。 |
| `UploadTrigger` | 控件 | `UploadTrigger.cs` / `UploadTriggerTheme.axaml` | visual tree | `UploadSourceKind`、`SelectFilesAsync`、`SelectDirectoriesAsync` | public | 可公开示例，不能持有任务状态。 |
| `UploadDropZone` | 控件 | `UploadDropZone.cs` / `UploadDropZoneTheme.axaml` | visual tree | `EnqueueFilesAsync` | public | 可公开示例，drop 后必须交给 `Upload`。 |
| `UploadList` | 控件 | `UploadList.cs` / `UploadListTheme.axaml` | `Upload` 或用户布局 | `ListMaxHeight`、`ListScrollBarVisibility` | internal-observable | 用于理解布局和滚动，不作为第二状态 owner。 |
| `atom:ScrollViewer` | 模板节点 | `UploadListTheme.axaml` | `UploadList` template | 列表滚动 | template-stable | 使用 AtomUI lite/auto-hide 滚动条，只包裹列表，不包裹 trigger。 |
| `AbstractUploadListItem` | item container | `AbstractUploadListItem.cs` | items control container lifecycle | remove/preview/action 事件 | internal-observable | container recycle 必须释放 item 绑定和事件。 |
| `UploadAppendContentItem` | internal visual slot | `UploadAppendContentItem.cs` | `Upload` display source | `TriggerContent` | internal | 只能承载 picture append 入口，不能写入 `Files` 或持有任务状态。 |
| `UploadQueue` | internal service | `UploadQueue.cs` | `Upload` | 上传状态和取消 | private | 不暴露给用户，不引用视觉控件。 |

## 6. 生命周期与模板接入

Upload 的生命周期释放必须成对设计，不能依赖 GC 或视觉树自然释放。

| Acquire | Release |
| --- | --- |
| upload task cancellation token | task completion, cancel, remove, reset, detach |
| success auto-remove delay | remove, reset, detach, status changes away from Success |
| trigger parent lookup | detached visual tree |
| drop-zone drag events | detached visual tree |
| generated list container bindings | container recycle |

模板和集合接入规则：

- `OnApplyTemplate` 重新应用时先释放旧 template part 订阅，再接入新 part。
- `Files` 集合替换时解绑旧集合 change 订阅，接入新集合，并按新集合重建 queue 映射。
- item container 准备时只绑定当前 `UploadFileItem`；container recycle 时必须释放旧 item 的订阅和 action handler。
- `UploadTrigger` 和 `UploadDropZone` 查找 owner 时只依赖当前 visual ancestor；脱离 visual tree 后不缓存旧 owner。
- detach 时取消全部上传任务、pending auto-remove delay、drag/drop 事件订阅和 C# binding。

## 7. 交互与事件处理

交互入口必须统一收敛到 `Upload` 的 public 方法：

- 文件按钮点击：`UploadTrigger.SourceKind=Files` 调用 `Upload.SelectFilesAsync`。
- 目录按钮点击：`UploadTrigger.SourceKind=Directories` 调用 `Upload.SelectDirectoriesAsync`。
- 拖拽提交：`UploadDropZone` 将文件信息传给 `Upload.EnqueueFilesAsync`。
- 手动移除：列表项发出 remove 请求，`Upload.RemoveFileAsync` 负责取消任务、取消 auto-remove 并从 `Files` 移除。
- 表单清空：`IFormItemAware.ClearFormValue` 使用 `ResetAsync`，不直接清集合绕过生命周期释放。

事件处理不得通过 suppression flag、延时刷新或强制重算来修正状态顺序。若出现状态不同步，优先修正 owner、事件顺序或释放路径。

## 8. 内部算法与关键流程

### 8.1 入队流程

1. 文件选择、目录选择或拖拽入口生成 `UploadFileInfo`。
2. `Upload.EnqueueFilesAsync` 应用 `Accepts`、`MaxCount` 和 `PendingText`。
3. 每个文件转换为一个 `UploadFileItem` 并追加到 effective `Files`。
4. `AutoUpload=true` 时将 item 与 file info 交给 `UploadQueue`。
5. queue/scheduler 回调只更新对应 `UploadFileItem`，列表通过绑定观察变化。

### 8.2 移除与 reset 流程

- `RemoveFileAsync(id)` 查找对应 `UploadFileItem`，取消上传任务和 auto-remove delay，再从 effective `Files` 移除。
- `ResetAsync()` 取消全部运行和 pending 工作，取消全部 auto-remove delay，清空 effective `Files`，不替换绑定集合实例。
- detach 使用 reset/cancel 语义释放资源，但不应触发用户不可预期的业务上传重试。

### 8.3 成功自动移除流程

- 当 item 状态进入 `Success` 且 `SuccessAutoRemoveDelay` 不为空时，按 item id 注册延迟取消源。
- 延迟到期后通过 `RemoveFileAsync` 移除文件，确保与手动移除共享同一释放路径。
- item 状态离开 `Success`、手动移除、reset、detach 时取消对应延迟。

## 9. 资源、性能与 AOT 边界

资源边界：

- 异步上传任务、取消源、delay、drag/drop 事件、collection change 和 item container 绑定必须有确定释放点。
- `UploadFileItem` 不持有视觉控件，避免文件项生命周期反向保留控件树。
- `UploadQueue` 不捕获 `Upload` 外的视觉对象；回调只写 item 状态。
- C# binding 仅用于 AXAML 无法表达的动态关系，并必须挂到明确 owner 上释放。

性能边界：

- 列表滚动由 `UploadList` 内部 `atom:ScrollViewer` 管理，并启用 lite/auto-hide；避免用户外层 `ScrollViewer` 把 trigger、drop-zone 和 list 一起滚动。
- `UploadList` 依赖 ItemsControl container 生命周期，不为每次进度变化重建列表项。
- 大量文件状态更新时只更新对应 `UploadFileItem` 属性，不替换 `Files` 集合。

AOT 边界：

- 不通过运行时反射扫描 public API、Token、Gallery API 表或上传模型。
- 新增 public 类型应显式引用并由源码、Gallery 和测试覆盖。
- Source generator 生成文件不手工编辑；LLMS 产物也不在本次运行时代码任务中手工修改。

## 10. 维护不变量

维护 Upload 时不得破坏以下不变量：

- `Files` 是唯一文件状态 owner。
- `UploadTaskInfo`、`TaskInfoList`、`DefaultTaskList`、`CurrentTaskList` 和 fake picture trigger task 不得重新进入目标实现。
- trigger、drop-zone、list、item container 都不能保存第二份业务任务状态。
- PictureCard/PictureCircle 的上传入口只能通过 `EffectivePictureItems` 中的 display append slot 呈现，确保与图片项处于同一 wrap flow。
- `RemoveFileAsync`、`ResetAsync`、detach 必须释放上传任务、auto-remove delay、集合订阅和 container 绑定。
- `DataValidationErrors` 是 error 状态来源，Upload 不维护独立 error 机制。
- AXAML-first binding 是默认选择；C# binding 必须说明 AXAML 不能表达的原因和释放 owner。
- Gallery 示例、API 表、控件文档和测试必须使用同一套 public contract。

## 11. 测试与验证

重构任务按以下层级验证：

| 变更范围 | 验证 |
| --- | --- |
| public API 与破坏性删除 | `UploadRedesignContractTests` 检查新属性、默认值和 legacy 成员移除。 |
| 单一状态 owner | `UploadFileStateTests` 覆盖 add、remove、reset、集合替换和外部绑定集合保持。 |
| 队列生命周期 | `UploadSchedulerTests` 覆盖 running 计数、完成释放、cancel all 和 transport 替换。 |
| 触发器与拖拽 | `UploadTriggerTests` 覆盖文件触发、目录触发和 owner 委托。 |
| 成功自动移除 | `UploadAutoRemoveTests` 覆盖 delay 到期、remove、reset、detach 和状态变更取消。 |
| Form 与验证 | `UploadFormValueTests` 覆盖 `FileValueMode`、set/clear 和 `DataValidationErrors`。 |
| Gallery 示例 | `UploadShowCasePageTests` 和 snapshot 覆盖示例、API 表、本地化和源码片段。 |
| 文档卫生 | `git diff --check` 和关键术语扫描。 |

Task 1 文档验证命令：

```bash
rg -n "UploadFileItem|UploadTrigger|UploadDropZone|UploadQueue|SuccessAutoRemoveDelay|UploadFileValueMode" docs/controls/desktop/data-entry/upload
git diff --check
```
