# Upload

> 生成产物：由源文档生成，不要手工编辑。修改内容请回到控件文档、源码 public surface、Token 类型或生成数据、Gallery ShowCase 或源码结构。

## 概述

Upload 是 AtomUI 桌面控件体系中的上传协调控件，用于管理文件选择、目录选择、拖拽提交、任务状态、列表展示、Form 值投影和上传操作入口。

Upload 不负责具体网络传输协议、文件存储服务或业务附件模型。这些职责由业务层或 `IFileUploadTransport` 承担。

主要源码入口：

- `src/AtomUI.Desktop.Controls/Upload`
- `src/AtomUI.Controls.Shared/Net`
- `controlgallery/AtomUIGallery/ShowCases/DataEntry/Upload`

## 包与命名空间

| 项 | 值 |
| --- | --- |
| NuGet 包 | `AtomUI.Desktop.Controls` |
| .NET 命名空间 | `AtomUI.Desktop.Controls` |
| AXAML 命名空间 | `https://atomui.net` |
| Gallery 页面 | `controlgallery/AtomUIGallery/ShowCases/DataEntry/Upload` |
| 状态 | Stable |

## 何时使用

Upload 的设计语言围绕“单一文件状态 owner + 可组合操作入口 + 独立列表视图”组织，而不是围绕某个固定模板节点组织。

| 维度 | 含义 | Upload 中的表达 |
| --- | --- | --- |
| 产品语义 | 控件在界面中承担的稳定职责。 | `Upload` 是上传状态协调器，统一管理文件、队列、选择入口、拖拽入口和列表视图。 |
| 内容承载 | 用户数据、展示内容、集合项或操作入口如何进入控件。 | `Files` 保存真实文件项；`TriggerContent`、`UploadTrigger`、`UploadDropZone` 和 `UploadList` 负责组合展示。 |
| 状态反馈 | public API、内部状态和伪类如何形成用户可感知反馈。 | `UploadFileItem.Status`、`Progress`、`ErrorMessage`、`Result` 是任务状态来源；Form 错误走 `DataValidationErrors`。 |
| 主题语义 | ControlTheme、SharedToken、控件 Token 和模板绑定如何表达视觉。 | Upload Token + ControlTheme；滚动边界在 `UploadList` 内部，并使用 AtomUI 自动隐藏滚动条。 |

## 公共 API

Upload 的公共契约由 public/protected 类型成员、Avalonia 属性、事件、命令、template part、伪类、ControlTheme key 和资源 key 共同组成。维护时应先确认这些契约是否已经被源码、Gallery 示例或文档暴露。

### 3.1 单一状态 owner

Upload 以 `Files` 作为唯一上传文件状态 owner。触发器、拖拽区和列表只是组合视图或动作入口，不保存第二份任务状态。

- `Upload.Files` 表示所有可观察上传文件。
- `UploadTrigger` 通过 `SourceKind=Files|Directories` 选择文件或目录。
- `UploadDropZone` 独占拖动协商和 Drop 快照，并把输入批次提交给最近的 `Upload`。
- `UploadDefaultDropArea` 只提供默认视觉，不读取拖动数据或维护文件状态。
- `UploadList` 渲染 `Files`，并独立管理滚动区域。
- `PictureCard` / `PictureCircle` 的上传入口通过 append slot 呈现，不作为 `UploadFileItem`。

### 3.2 核心 public surface

| 契约组 | 代表成员 | 维护含义 |
| --- | --- | --- |
| 文件状态 | `Files`、`UploadFileItem` | 唯一文件状态 owner，支持绑定、Form 投影和列表渲染。 |
| 文件选择 | `UploadTrigger`、`UploadSourceKind`、`SelectFilesAsync`、`SelectDirectoriesAsync` | 文件与目录选择是独立动作入口，不再由根控件 bool 互斥。 |
| 拖拽提交 | `UploadDropZone`、`UploadDirectoryDropMode`、`UploadDragState` | DropZone 负责平台协商和快照，`Upload` 负责统一准入与文件状态。 |
| 文件准入 | `AllowedFileTypes`、`CountOverflowBehavior`、`AdmissionPolicy` | picker、directory、drop 和 programmatic 输入共享同一准入与数量语义。 |
| 输入结果 | `InputBatchCompleted`、`UploadInputBatchCompletedEventArgs` | 每个输入批次在 UI 线程统一报告接受项、拒绝项以及 Completed、Cancelled 或 Failed 终态。 |
| 文件内容 | `UploadFileInfo`、`IUploadFileSource` | Transport 通过可打开内容源读取文件，不假定本地路径可访问。 |
| 上传队列 | `UploadTransport`、`AutoUpload`、`MaxConcurrentTasks`、`UploadQueue` | 上传调度与视觉控件解耦，生命周期由 `Upload` 统一释放。 |
| 列表展示 | `UploadList`、`ListType`、`ListMaxHeight`、`ListScrollBarVisibility` | 列表内部滚动，触发区保持固定。 |
| 触发入口 | `TriggerContent`、`UploadTrigger`、Picture append slot | 文件/目录触发器由用户布局组合，PictureCard/PictureCircle 通过显示源 append slot 呈现。 |
| 状态反馈 | `SuccessAutoRemoveDelay`、`PendingText`、`FileValueMode` | 成功自动移除、待上传文案和 Form 值投影可配置。 |
| 视觉与动效 | `IsMotionEnabled`、Upload Token | 只表达视觉状态，不保存业务任务状态。 |

输入与准入 API 的默认契约：

| 成员 | 默认值 | 语义 |
| --- | --- | --- |
| `Upload.AllowedFileTypes` | `null` | 不按文件名 pattern 或 MIME 拒绝文件；空集合含义相同。 |
| `Upload.CountOverflowBehavior` | `RejectExcess` | 按候选稳定顺序接受剩余容量内的文件，并拒绝超出项。 |
| `Upload.AdmissionPolicy` | `null` | 不执行额外业务准入；非空策略固定在非 UI 执行上下文调用。 |
| `Upload.MaxCount` | `int.MaxValue` | 输入管线可提交的最大 effective 文件数。 |
| `UploadDropZone.IsOpenFileDialogOnClick` | `true` | 主指针点击 DropZone 时按 `SourceKind` 打开选择器。 |
| `UploadDropZone.SourceKind` | `Files` | 点击时打开文件选择器。 |
| `UploadDropZone.IsFileDropEnabled` | `true` | DropZone 参与文件 DragDrop 协商。 |
| `UploadDropZone.DirectoryDropMode` | `Reject` | 外部拖入目录时拒绝目录。 |
| `UploadDropZone.MaxDirectoryDepth` | `32` | 递归目录根深度为 `0`，属性值不得小于 `0`。 |
| `UploadDropZone.MaxEnumeratedItems` | `10000` | 每个顶层目录树最多观察的文件和目录总数，属性值必须大于 `0`。 |
| `UploadDropZone.DragState` | `None` | 只读拖动协商状态。 |
| `UploadDropZone.IsDropProcessing` | `false` | 只读活动 Drop 批次状态。 |

`Upload.EnqueueFilesAsync`、文件选择器、目录选择器和拖动输入分别产生 `Programmatic`、`FilePicker`、`DirectoryPicker` 和 `DragDrop` 批次。每个实际启动的批次只触发一次 `InputBatchCompleted`；事件在输入处理进入终态、剩余批次资源完成释放且接受项已经提交后于 UI 线程触发。正常批次在事件后完成 Task，取消批次在事件后传播 `OperationCanceledException`，失败批次在事件后传播原始异常或清理聚合异常。

`UploadInputBatchStatus` 通过 `Completed`、`Cancelled` 和 `Failed` 区分互斥终态。只有 `Failed` 携带 `UploadInputFailureReason`；取消和批次级失败不再构造虚假的 rejected item。`UploadRejectedItem` 只公开稳定的 `UploadRejectionReason`、业务 rejection code 和业务 message，不公开平台 `Exception`。`UploadAdmissionDecision` 通过 `Accept()` 和 `Reject(...)` 创建，不能构造“接受但携带拒绝信息”的矛盾状态。

`UploadFileInfo` 是不可变输入描述。`Name` 和 `Source` 必须存在；`Path`、`Size`、`ContentType`、`DateCreated` 与 `DateModified` 都允许为空，因为跨进程或受限平台存储项不保证提供本地路径和完整元数据。`IUploadFileSource.OpenReadAsync` 是 transport 读取内容的唯一稳定入口。

### 3.3 公共状态类型

```csharp
public class UploadFileItem : AvaloniaObject
{
    public Guid Id { get; set; }
    public string? Name { get; set; }
    public Uri? Path { get; set; }
    public long Size { get; set; }
    public FileUploadStatus Status { get; set; }
    public double Progress { get; set; }
    public string? ErrorMessage { get; set; }
    public FileUploadResult? Result { get; set; }
    public object? UserData { get; set; }
    public string? PendingText { get; set; }
    public bool IsImageFile { get; set; }
}

public enum UploadSourceKind
{
    Files,
    Directories
}

public enum UploadFileValueMode
{
    AllFiles,
    SuccessfulFiles,
    Results
}
```

### 3.4 推荐用法

```xml
<atom:Upload Files="{Binding Attachments}"
             UploadTransport="{Binding UploadTransport}"
             ListMaxHeight="180"
             ListScrollBarVisibility="Auto"
             SuccessAutoRemoveDelay="0:0:3">
    <atom:Upload.TriggerContent>
        <StackPanel Orientation="Horizontal" Spacing="8">
            <atom:UploadTrigger SourceKind="Files">
                <atom:Button Content="上传文件" />
            </atom:UploadTrigger>
            <atom:UploadTrigger SourceKind="Directories">
                <atom:Button Content="上传文件夹" />
            </atom:UploadTrigger>
        </StackPanel>
    </atom:Upload.TriggerContent>
</atom:Upload>
```

## 事件与命令

Upload 的公共契约由 public/protected 类型成员、Avalonia 属性、事件、命令、template part、伪类、ControlTheme key 和资源 key 共同组成。维护时应先确认这些契约是否已经被源码、Gallery 示例或文档暴露。
| 输入结果 | `InputBatchCompleted`、`UploadInputBatchCompletedEventArgs` | 每个输入批次在 UI 线程统一报告接受项、拒绝项以及 Completed、Cancelled 或 Failed 终态。 |
`Upload.EnqueueFilesAsync`、文件选择器、目录选择器和拖动输入分别产生 `Programmatic`、`FilePicker`、`DirectoryPicker` 和 `DragDrop` 批次。每个实际启动的批次只触发一次 `InputBatchCompleted`；事件在输入处理进入终态、剩余批次资源完成释放且接受项已经提交后于 UI 线程触发。正常批次在事件后完成 Task，取消批次在事件后传播 `OperationCanceledException`，失败批次在事件后传播原始异常或清理聚合异常。

## 使用示例

稳定示例来源于 Gallery ShowCase 和源码查看片段。生成器只输出可从 `ShowCaseItem` 追溯的示例，不维护第二套手写示例。

以下示例来自 Gallery 源码查看使用的 `ShowCaseItem` 片段，并已按中文资源规范化。

### 拖拽上传

来源：`controlgallery/AtomUIGallery/ShowCases/DataEntry/Upload/Views/UploadShowCase.axaml:183`

Gallery key：`ExamplesContent` / item `5`

```axaml
<atom:Upload Name="DragAndDropUpload"
             UploadTransport="{Binding UploadTransport}"
             UploadTaskFailed="HandleUploadFailed"
             UploadTaskCompleted="HandleUploadCompleted">
    <atom:Upload.TriggerContent>
        <atom:UploadDropZone>
            <atom:UploadDefaultDropArea />
        </atom:UploadDropZone>
    </atom:Upload.TriggerContent>
</atom:Upload>
```

## 状态模型

Upload 的状态流只允许按以下路径收敛：

```text
Public API / UploadTrigger / UploadDropZone
  -> UploadInputPipeline
  -> directory traversal / file admission
  -> UI count policy / accepted file commit
  -> Files collection
  -> UploadQueue / FileUploadScheduler
  -> UploadFileItem.Status / Progress / Result
  -> UploadList item containers
  -> Gallery 可观察行为
```

状态维护规则：

- `Files` 是唯一文件状态 owner；实现中按文件 id 保存的 accepted `UploadFileInfo` 只承担内容源 lease，不形成第二份可观察任务状态。
- `UploadQueue` 只负责把 `UploadFileItem` 映射到 `FileUploadTask` 并转发调度结果，不直接操作视觉容器。
- `UploadList` 只渲染 `Files`，不得创建、删除或隐藏真实任务状态。
- 文件选择和目录选择由 `UploadTrigger.SourceKind` 决定，可以在同一 `Upload` 下并存。
- `SuccessAutoRemoveDelay` 的延迟任务必须在 remove、reset、detach 和状态离开 success 时取消。
- Form 值由 `FileValueMode` 投影，错误状态以 Avalonia `DataValidationErrors` 为准。

## 主题与 Design Token

Upload 的视觉模型由控件模板、ControlTheme、SharedToken 和控件 Token 共同构成。

| 主题文件 | 职责 |
| --- | --- |
| `UploadTheme.axaml` | 根模板，连接 `TriggerContent`、list 和 picture display source。 |
| `UploadTriggerTheme.axaml` | 触发器 shell，只承载用户内容和点击动作，不硬编码 Button。 |
| `UploadDropZoneTheme.axaml` | 拖拽行为 shell，只承载用户内容和内容对齐。 |
| `UploadDefaultDropAreaTheme.axaml` | 默认拖动视觉，保留边框、图标、标题、副标题、状态 selector 和动效，不处理拖动数据。 |
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

主题维护规则：

- Trigger 不作为文件项渲染，PictureCard/PictureCircle 使用不进入 `Files` 的 display append slot 保持同一 wrap flow。
- Text/Picture 根模板必须在 trigger 与 list 之间保留 Shared spacing，避免触发按钮和第一条文件项贴在一起。
- 滚动区域只包裹列表，不包裹 trigger。
- 可由 AXAML 表达的模板状态必须优先留在 AXAML。
- Token 只表达视觉变量，不承载上传状态、队列状态或 Form 错误。

Token 来源：

Upload Token 只表达组件级视觉变量，例如尺寸、间距、颜色、圆角、阴影、图标尺寸和弹层边界。Token 不承载运行时选择、展开、加载、错误、上传任务、过滤条件或业务状态。

当前 Token scope：

- `UploadToken`，scope id 为 `Upload`，源码位于 `src/AtomUI.Desktop.Controls/Upload/UploadToken.cs`。

## AOT 与裁剪注意事项

资源边界：

- 异步上传任务、取消源、delay、drag/drop 事件、collection change 和 item container 绑定必须有确定释放点。
- DragOver 不读取文件值或元数据；Drop 只物化一次顶层 StorageItem 数据。
- 输入批次通过 arrival gate 串行执行；每个批次只建立一次显式 worker dispatch，目录展开、元数据读取和准入按候选稳定顺序处理，不创建无界并发任务；数量决策与集合变更保持在同一个 UI commit 边界。
- `UploadInputBatchOperation` 不保存泛化 `IDisposable`；StorageItem 和 source lease 使用独立的引用相等 owner set，释放遍历不得因单个异常提前中止。
- `UploadFileItem` 不持有视觉控件，避免文件项生命周期反向保留控件树。
- `UploadQueue` 不捕获 `Upload` 外的视觉对象；回调只写 item 状态。
- scheduler 的取消完成语义必须等待 transport execution task 真正退出，source lease 才能被上层释放。
- scheduler maintenance 必须先把全部 Pending task 转入 Cancelled、向全部 Running task 发出取消，再观察调用方取消令牌；并发 transport/并发度变更按调用顺序生效。
- C# binding 仅用于 AXAML 无法表达的动态关系，并必须挂到明确 owner 上释放。

性能边界：

- 列表滚动由 `UploadList` 内部 `atom:ScrollViewer` 管理，并启用 lite/auto-hide；避免用户外层 `ScrollViewer` 把 trigger、drop-zone 和 list 一起滚动。
- `UploadList` 依赖 ItemsControl container 生命周期，不为每次进度变化重建列表项。
- 大量文件状态更新时只更新对应 `UploadFileItem` 属性，不替换 `Files` 集合。

AOT 边界：

- 不通过运行时反射扫描 public API、Token、API 契约摘要或上传模型。
- 拖动输入使用 typed DataTransfer、StorageItem 和显式策略，不依赖平台私有反射或动态发现。
- 新增 public 类型应显式引用并由源码、Gallery 和测试覆盖。
- Source generator 生成文件不手工编辑；LLMS 产物也不在本次运行时代码任务中手工修改。

## 源码索引

源码结构按“状态 owner、输入入口、输入管线、队列协调、列表视图、主题模板”分层。

| 路径 | 职责 |
| --- | --- |
| `src/AtomUI.Desktop.Controls/Upload/Upload.cs` | 保留 public/protected API、Avalonia 属性注册、构造、display source 桥接、`IFormItemAware` 和顶层上传协调。 |
| `src/AtomUI.Desktop.Controls/Upload/UploadAppendContentItem.cs` | internal picture append visual slot，用于把 `TriggerContent` 放入 PictureCard/PictureCircle 的同一 wrap flow；不进入 `Files`。 |
| `src/AtomUI.Desktop.Controls/Upload/Upload.FileSelection.cs` | 封装文件选择和目录选择动作，供 `UploadTrigger` 调用。 |
| `src/AtomUI.Desktop.Controls/Upload/IUploadStorageProviderAdapter.cs` | 隔离 Avalonia storage picker 调用，向文件选择和目录选择提供可测试的 typed StorageItem 边界。 |
| `src/AtomUI.Desktop.Controls/Upload/Upload.InputPipeline.cs` | 连接 `Upload` 状态 owner 与输入管线，负责 UI 线程 option 快照、typed commit、批次事件和 ReplaceExisting source handoff。 |
| `src/AtomUI.Desktop.Controls/Upload/UploadInputPipeline.cs` | 串行执行 picker、directory、drop 和 programmatic 批次，在一次 worker 边界内协调遍历与准入，再把数量策略和集合变更交给 UI commit。 |
| `src/AtomUI.Desktop.Controls/Upload/UploadInputBatchOperation.cs` | 分别持有单个批次的 typed StorageItem 与 source lease ownership，验证 transfer，并在完成事件前释放全部未移交资源。 |
| `src/AtomUI.Desktop.Controls/Upload/UploadStorageItemEnumerator.cs` | 仅使用 `IStorageFolder.GetItemsAsync()` 按稳定顺序展开 storage items，并隔离目录分支错误。 |
| `src/AtomUI.Desktop.Controls/Upload/UploadInputCandidate.cs` | 非 owning 地引用批次已拥有的 storage file，并提供基础元数据读取入口。 |
| `src/AtomUI.Desktop.Controls/Upload/UploadFileAdmissionService.cs` | 在非 UI 执行上下文执行 `AllowedFileTypes` 和 `AdmissionPolicy`，生成明确的接受或拒绝结果。 |
| `src/AtomUI.Desktop.Controls/Upload/UploadStorageFileSource.cs` | 以 `IStorageFile` lease 实现 `IUploadFileSource`，允许 transport 在 lease 有效期内打开读取流。 |
| `src/AtomUI.Desktop.Controls/Upload/Upload.AutoRemove.cs` | 管理成功自动移除的延迟任务、取消和生命周期释放。 |
| `src/AtomUI.Desktop.Controls/Upload/UploadFileItem.cs` | public 文件状态模型，承载文件元数据、状态、进度、错误、结果和 pending 文案。 |
| `src/AtomUI.Desktop.Controls/Upload/UploadSourceKind.cs` | public 触发来源枚举，区分文件选择和目录选择。 |
| `src/AtomUI.Desktop.Controls/Upload/UploadFileValueMode.cs` | public Form 值投影枚举，控制提交全部文件、成功文件或上传结果。 |
| `src/AtomUI.Desktop.Controls/Upload/UploadTrigger.cs` | public 可组合触发器，查找最近的 `Upload` 并触发文件或目录选择。 |
| `src/AtomUI.Desktop.Controls/Upload/UploadDropZone.cs` | public 可组合拖拽区，拥有 DragDrop 协商、状态和 Drop 快照。 |
| `src/AtomUI.Desktop.Controls/Upload/UploadDefaultDropArea.cs` | public 默认拖动视觉，只保存图标、标题、副标题和动效属性。 |
| `src/AtomUI.Desktop.Controls/Upload/UploadQueue.cs` | internal 上传队列协调器，映射 `UploadFileItem` 与 `FileUploadTask`。 |
| `src/AtomUI.Desktop.Controls/Upload/UploadList.cs` | 文件列表视图，渲染 effective 文件视图并维护列表滚动。 |
| `src/AtomUI.Desktop.Controls/Upload/AbstractUploadListItem.cs` | 列表项基类，只绑定 `UploadFileItem` 并发出 item action，不拥有任务状态。 |
| `src/AtomUI.Controls.Shared/Net/FileUploadScheduler.cs` | 共享上传调度器，跟踪 pending/running 任务、并发和取消。 |
| `src/AtomUI.Controls.Shared/Net/FileUploadTask.cs` | 共享上传任务对象，承载传输回调和 cancellation 状态。 |
| `src/AtomUI.Controls.Shared/Net/IUploadFileSource.cs` | public 文件内容读取契约，不要求本地路径。 |
| `src/AtomUI.Controls.Shared/Net/IUploadFileSourceLease.cs` | internal AtomUI-owned source lease 标记，只由接受该 lease 的 owner 释放。 |

主题结构：

| 路径 | 职责 |
| --- | --- |
| `src/AtomUI.Desktop.Controls/Upload/Themes/UploadTheme.axaml` | 根模板，连接 `TriggerContent`、list 和 picture display source，并保持触发区与列表的稳定间距。 |
| `src/AtomUI.Desktop.Controls/Upload/Themes/UploadTriggerTheme.axaml` | 触发器 shell，只承载用户内容和点击表面。 |
| `src/AtomUI.Desktop.Controls/Upload/Themes/UploadDropZoneTheme.axaml` | 拖拽行为 shell，只承载用户内容和内容对齐。 |
| `src/AtomUI.Desktop.Controls/Upload/Themes/UploadDefaultDropAreaTheme.axaml` | 默认拖动视觉，保持 Frame、内容 presenter、Token 和动效结构。 |
| `src/AtomUI.Desktop.Controls/Upload/Themes/UploadListTheme.axaml` | 列表 shell，内部拥有自动隐藏的 `atom:ScrollViewer` 和滚动边界。 |
| `src/AtomUI.Desktop.Controls/Upload/Themes/PictureList/*` | Picture 列表项视觉，pending 内容优先读取 `UploadFileItem.PendingText`。 |
| `src/AtomUI.Desktop.Controls/Upload/Themes/PictureShapeList/*` | PictureCard/PictureCircle 列表布局和 item 视觉；append slot 由 display source 承载并进入同一 wrap flow。 |

## 相关文档

- 源设计文档：`docs/controls/desktop/data-entry/upload/overview.md`
- 实现文档：`docs/controls/desktop/data-entry/upload/implementation.md`
- Token 文档：`docs/controls/desktop/data-entry/upload/token.md`
- 变更记录：`docs/controls/desktop/data-entry/upload/changelog.md`
- 语义结构：`./semantic-cn.md`
