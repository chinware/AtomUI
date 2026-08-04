# Upload 拖动上传设计

本文档定义 Upload 文件拖动输入的公共契约、状态协商、跨平台策略、异步输入管线和资源生命周期。Upload 的整体定位见 [Upload 桌面版架构设计](overview.md)，源码职责入口见 [Upload 桌面版实现原理](implementation.md)，视觉变量见 [Upload Token 设计](token.md)。

## 1. 设计定位

拖动上传是 Upload 的文件输入能力，不是独立的上传状态模型。`UploadDropZone` 负责把平台拖动会话转换为输入批次，`Upload` 负责统一执行目录展开、文件准入、数量决策、任务创建和文件状态维护。

该设计覆盖以下输入与宿主：

- 文件管理器拖入的单文件、多文件、目录以及文件和目录的混合数据。
- Windows、macOS、Linux X11 和 Linux Wayland 桌面宿主。
- 默认 `UploadDefaultDropArea` 和用户提供的自定义 `UploadDropZone.Content`。
- 文件选择器、目录选择器和程序化输入共享的文件准入管线。

网络协议、远端存储、业务附件模型和服务端校验仍由 `IFileUploadTransport` 与业务层负责。

## 2. 设计原则

拖动上传必须遵守以下不变量：

1. `Upload` 是文件状态和上传任务的唯一 owner。
2. `UploadDropZone` 是拖动事件、效果协商和 Drop 数据快照的唯一 owner。
3. `UploadDefaultDropArea` 只负责默认视觉，不读取拖动数据，不创建输入批次。
4. `DragEnter` 和 `DragOver` 只检查数据格式和可接受效果，不物化文件或枚举目录。
5. `Drop` 期间只读取一次文件数据，并且不跨异步边界保留 `DragEventArgs` 或 `IDataTransfer`。
6. AtomUI 只接受 `Copy` 语义，不返回 `Move`，也不移动或删除源文件。
7. 文件选择、目录选择、拖动和程序化输入使用同一准入、数量和任务创建规则。
8. 单项拒绝不终止同一批次中的其他项目，取消和批次级失败不伪装成拒绝项。
9. 每个 `IStorageItem`、文件内容源、Stream、取消源和异步批次都有唯一释放 owner。
10. ownership transfer 必须验证来源 owner，失败时立即抛出，不能静默忽略。
11. 目录枚举、元数据读取和文件准入不在 Avalonia UI 线程执行；数量策略与集合变更在同一个 UI commit 边界完成。
12. 默认 ControlTheme、视觉树、布局、Token 映射和最终渲染结果保持不变。

## 3. 专项模型与 Public API

### 3.1 Owner 模型

| 模型 | Owner | 稳定职责 |
| --- | --- | --- |
| drag session | `UploadDropZone` | 协商效果、维护悬停状态、取得 Drop 快照。 |
| input batch | `Upload` | 串行执行目录展开、元数据读取、准入和数量提交。 |
| file state | `Upload.Files` | 保存用户可观察的文件状态。 |
| upload task | `UploadQueue` | 调度传输、取消和结果回写。 |
| default drop visual | `UploadDefaultDropArea` | 渲染图标、标题、副标题、边框和动效。 |
| storage item lease | input batch 或 accepted file item | 确保平台文件句柄在使用结束后释放一次。 |

### 3.2 UploadDropZone

`UploadDropZone` 公开拖动输入策略和只读运行状态：

```csharp
public sealed class UploadDropZone : ContentControl
{
    public bool IsOpenFileDialogOnClick { get; set; } = true;
    public UploadSourceKind SourceKind { get; set; } = UploadSourceKind.Files;
    public bool IsFileDropEnabled { get; set; } = true;
    public UploadDirectoryDropMode DirectoryDropMode { get; set; } = UploadDirectoryDropMode.Reject;
    public int MaxDirectoryDepth { get; set; } = 32;
    public int MaxEnumeratedItems { get; set; } = 10_000;

    public UploadDragState DragState { get; }
    public bool IsDropProcessing { get; }
}
```

```csharp
public enum UploadDragState
{
    None,
    Accepting,
    Rejecting
}

public enum UploadDirectoryDropMode
{
    Reject,
    TopLevelFiles,
    RecursiveFiles
}
```

API 语义：

- `IsOpenFileDialogOnClick` 只控制 DropZone 的点击选择行为，不控制 `UploadTrigger`。
- `SourceKind` 只决定点击时打开文件选择器还是目录选择器，不改变外部拖动数据的类型判断。
- `IsFileDropEnabled=false` 时 DropZone 可以继续作为点击入口，但对拖动数据返回 `None`。
- `MaxDirectoryDepth` 只约束递归目录展开；根目录的深度为 `0`。
- `MaxEnumeratedItems` 统计目录遍历看到的文件与目录总数，不等同于 `Upload.MaxCount`。
- `DragState` 描述当前拖动协商，`IsDropProcessing` 描述已完成 Drop 后的异步输入批次；两者相互独立。

### 3.3 Upload 输入契约

`Upload` 使用强类型文件规则和明确的数量溢出策略：

```csharp
public IReadOnlyList<FilePickerFileType>? AllowedFileTypes { get; set; }
public UploadCountOverflowBehavior CountOverflowBehavior { get; set; }
public IUploadAdmissionPolicy? AdmissionPolicy { get; set; }

public event EventHandler<UploadInputBatchCompletedEventArgs> InputBatchCompleted;
```

```csharp
public enum UploadCountOverflowBehavior
{
    RejectExcess,
    RejectBatch,
    ReplaceExisting
}
```

- `AllowedFileTypes=null` 或空集合表示不按类型拒绝文件。
- `CountOverflowBehavior` 默认为 `RejectExcess`。
- 文件选择器直接接收同一组 `FilePickerFileType`；统一输入管线独立执行相同对象中的文件名 pattern 和 MIME 规则。
- `Patterns` 按文件名匹配；MIME 只在 `UploadFileInfo.ContentType` 可用时参与匹配。`UploadFileInfo` 不提供可靠 UTI 元数据，因此 `AppleUniformTypeIdentifiers` 不参与管线准入。
- `AdmissionPolicy` 在创建 `UploadFileItem` 前异步执行，用于业务准入；策略结果必须包含稳定的接受或拒绝决定。
- `AdmissionPolicy` 固定在非 UI 执行上下文调用，不能读取或修改 Avalonia 控件。
- `InputBatchCompleted` 在 UI 线程触发，每个批次恰好一次，并同时报告接受项、拒绝项和批次终态。

业务准入契约为：

```csharp
public interface IUploadAdmissionPolicy
{
    ValueTask<UploadAdmissionDecision> EvaluateAsync(
        UploadAdmissionContext context,
        CancellationToken cancellationToken = default);
}

public sealed record UploadAdmissionContext(
    Guid BatchId,
    UploadInputSource Source,
    UploadFileInfo File);

public sealed class UploadAdmissionDecision
{
    public bool IsAccepted { get; }
    public string? RejectionCode { get; }
    public string? Message { get; }

    public static UploadAdmissionDecision Accept();

    public static UploadAdmissionDecision Reject(
        string? rejectionCode = null,
        string? message = null);

    private UploadAdmissionDecision(
        bool isAccepted,
        string? rejectionCode,
        string? message);
}
```

策略只决定当前文件能否进入 Upload，不修改 `Files`、不创建任务，也不取得 StorageItem 或文件内容源的释放 ownership。策略可以读取文件元数据或通过 `Source.OpenReadAsync` 检查内容，但必须释放自己打开的 Stream，不得长期保存 Source。匹配取消令牌的 `OperationCanceledException` 取消整个批次；其他异常只拒绝当前文件并产生 `AdmissionPolicyFailed`。

### 3.4 文件内容源

上传文件使用可打开内容的源，而不是假定 `Uri.LocalPath` 可访问：

```csharp
public interface IUploadFileSource
{
    ValueTask<Stream> OpenReadAsync(CancellationToken cancellationToken = default);
}

public sealed class UploadFileInfo
{
    public string Name { get; }
    public Uri? Path { get; }
    public long? Size { get; }
    public string? ContentType { get; }
    public DateTimeOffset? DateCreated { get; }
    public DateTimeOffset? DateModified { get; }
    public IUploadFileSource Source { get; }
}
```

`IFileUploadTransport` 通过 `Source.OpenReadAsync` 读取内容。Transport 释放自己打开的 Stream，但不释放文件源或平台 StorageItem；文件源的底层 lease 由 `Upload` 管理。

### 3.5 批次结果

批次结果契约为：

```csharp
public enum UploadInputSource
{
    FilePicker,
    DirectoryPicker,
    DragDrop,
    Programmatic
}

public enum UploadInputBatchStatus
{
    Completed,
    Cancelled,
    Failed
}

public enum UploadInputFailureReason
{
    DataSnapshotFailed,
    ProcessingFailed
}

public enum UploadRejectionReason
{
    UnsupportedStorageItem,
    DirectoryNotAllowed,
    DirectoryDepthExceeded,
    DirectoryCycleDetected,
    EnumerationLimitExceeded,
    AccessDenied,
    StorageReadFailed,
    FileTypeNotAllowed,
    AdmissionRejected,
    AdmissionPolicyFailed,
    CountLimitExceeded
}

public sealed class UploadRejectedItem
{
    public string Name { get; }
    public Uri? Path { get; }
    public UploadRejectionReason Reason { get; }
    public string? RejectionCode { get; }
    public string? Message { get; }

    internal UploadRejectedItem(
        string name,
        Uri? path,
        UploadRejectionReason reason,
        string? rejectionCode = null,
        string? message = null);
}

public sealed class UploadInputBatchCompletedEventArgs : EventArgs
{
    public Guid BatchId { get; }
    public UploadInputSource Source { get; }
    public UploadInputBatchStatus Status { get; }
    public UploadInputFailureReason? FailureReason { get; }
    public IReadOnlyList<UploadFileInfo> AcceptedFiles { get; }
    public IReadOnlyList<UploadRejectedItem> RejectedItems { get; }

    internal UploadInputBatchCompletedEventArgs(
        Guid batchId,
        UploadInputSource source,
        UploadInputBatchStatus status,
        UploadInputFailureReason? failureReason,
        IReadOnlyList<UploadFileInfo> acceptedFiles,
        IReadOnlyList<UploadRejectedItem> rejectedItems);
}
```

批次状态必须满足：

| `Status` | `FailureReason` | 语义 |
| --- | --- | --- |
| `Completed` | `null` | 批次自然结束，可以同时包含接受项和拒绝项。 |
| `Cancelled` | `null` | 取消在自然完成前终止批次。 |
| `Failed` | 非空 | 数据快照、管线处理、UI commit 或资源清理发生批次级失败。 |

`BatchId` 是批次稳定标识，`Source` 区分 `FilePicker`、`DirectoryPicker`、`DragDrop` 和 `Programmatic`。`AcceptedFiles` 只包含已经提交并把内部 source lease 转移给 `Upload` 的文件；`RejectedItems` 只包含项目级拒绝或项目级读取失败。结果集合是按候选顺序生成的只读快照。

拒绝项不公开平台或策略抛出的原始 `Exception`。应用根据稳定的 `Reason` 决定行为；`RejectionCode` 和策略 `Message` 只来自策略显式返回的 `Reject` 决定。平台异常和策略异常只进入内部诊断。`UnsupportedDataFormat`、`Cancelled` 和 `InputFailed` 不属于拒绝原因；元数据和内容源创建失败统一为 `StorageReadFailed`。

终态优先级为：

1. 处理、commit 或清理异常产生 `Failed/ProcessingFailed`。
2. 没有失败且观察到取消时产生 `Cancelled`。
3. 其余情况产生 `Completed`。

清理异常优先于同时发生的取消。批次级失败可以保留失败前已经完整 commit 的接受项，但不得把未转移 ownership 的文件报告为接受项。

## 4. 状态与平台策略

### 4.1 拖动状态机

| 当前状态 | 输入 | 条件 | 下一状态 | 返回效果 |
| --- | --- | --- | --- | --- |
| `None` | `DragEnter` | 文件格式存在、允许 Copy、DropZone 可接收 | `Accepting` | `Copy` |
| `None` | `DragEnter` | 任一协商条件不满足 | `Rejecting` | `None` |
| `Accepting` / `Rejecting` | `DragOver` | 重新计算协商结果 | `Accepting` 或 `Rejecting` | `Copy` 或 `None` |
| `Accepting` / `Rejecting` | `DragLeave` | 指针已离开 DropZone 边界 | `None` | 不适用 |
| `Accepting` | `Drop` | 快照成功 | `None` | `Copy` |
| 任意状态 | detach、禁用或 owner 失效 | 无 | `None` | `None` |

`DragLeave` 必须通过 `GetPosition(UploadDropZone)` 与控件边界确认是否真正离开。拖动目标在 DropZone 子元素之间切换时，不得清空状态或产生视觉闪烁。

最近的已启用 `UploadDropZone` 拥有路由事件。取得所有权后设置 `Handled`，祖先 DropZone 不再处理同一会话。

### 4.2 平台矩阵

| 平台 | 协商要求 | 数据取得 | 稳定保证 |
| --- | --- | --- | --- |
| Windows | 每次 Enter/Over 明确返回 `Copy` 或 `None` | Drop 时一次性取得 StorageItem | 不重复读取系统 DataObject。 |
| macOS | Copy 语义保持一致 | Drop 时取得并持有文件句柄 | 文件访问能力保持到 accepted item 释放。 |
| Linux X11 | 每次 Enter/Over 都返回 XDND status 所需效果 | 只在 Drop 时读取 selection | 高频 DragOver 不触发同步数据物化。 |
| Linux Wayland | 效果反馈给 compositor | Drop 时一次性物化 pipe-backed 数据 | 不跨 await 持有 DataTransfer，不假定本地路径。 |

共享语义不得复制为平台分支。只有上游平台能力差异可以进入 backend 适配层；AtomUI 公共 API 不暴露 X11、Wayland、Windows 或 macOS 专用开关。

## 5. 架构、文件结构与职责

| 边界 | 类型或文件 | 输入 | 输出 | 不负责 |
| --- | --- | --- | --- | --- |
| DropTarget / drag session | `UploadDropZone` | DragDrop routed events、数据格式、允许效果、位置和 owner 能力 | DragState、DragEffects、Handled 和同步 StorageItem 快照 | 目录枚举、元数据和任务状态。 |
| input coordinator | `UploadInputPipeline` | 文件选择、目录选择、Drop 或程序化批次 | accepted/rejected batch result | 网络上传。 |
| storage enumeration | `UploadStorageItemEnumerator` | `IStorageItem` 和目录策略 | 有序文件候选项 | 文件类型和数量决策。 |
| input candidate | `UploadInputCandidate` | 批次已拥有的 `IStorageFile` | 非 owning 文件候选及元数据读取入口 | StorageItem 或 source lease 释放。 |
| admission | `UploadFileAdmissionService` | 文件元数据、AllowedFileTypes、业务策略 | 接受或拒绝决定 | 数量策略和视觉状态。 |
| storage source | `UploadStorageFileSource` | `IStorageFile` | 可重复请求的读取 Stream | 上传协议。 |
| batch lifetime | `UploadInputBatchOperation` | 快照、取消令牌 | 批次终态、typed StorageItem/source ownership 和完整释放 | 控件渲染。 |
| UI commit | `Upload` | 准入后的文件与 typed batch operation | `UploadFileItem`、accepted source ownership 和 queue task | 平台 DragDrop 数据。 |
| default visual | `UploadDefaultDropArea` | 内容属性和主题资源 | 当前默认 DropArea 视觉 | DragDrop、文件读取和 owner 查找。 |

这些职责属于稳定 ownership 边界。不得重新形成两个拖动数据 owner、让视觉控件持有上传任务，或用平台 adapter 层、事件总线和 service locator 复制已经由输入管线承担的职责。

## 6. Template、组合与集成契约

默认组合保持以下结构：

```text
UploadDropZone
  -> ContentPresenter#PART_ContentPresenter
     -> UploadDefaultDropArea
        -> DashedBorder#Frame
           -> StackPanel
              -> IconPresenter#IconPresenter
              -> ContentPresenter#HeaderContentPresenter
              -> ContentPresenter#SubHeaderContentPresenter
```

Template 契约：

- `UploadDropZoneTheme.axaml` 继续只提供 `PART_ContentPresenter`，不自动创建 `UploadDefaultDropArea`。
- Gallery 和用户仍以 `<UploadDropZone><UploadDefaultDropArea /></UploadDropZone>` 组合默认界面。
- 自定义 `UploadDropZone.Content` 只替换内容，不替换 DragDrop 行为。
- `UploadDefaultDropAreaTheme.axaml` 保持 `Frame`、三个 presenter、属性绑定、selector、Token、Transitions 和布局结构。
- `DragDrop.AllowDrop` 由 `UploadDropZone` 拥有；`UploadDefaultDropArea` 不再拥有 attached property 或 Drop handler。
- DropZone 可以暴露 `:drag-over`、`:drag-accepting`、`:drag-rejecting` 和 `:drop-processing`，但默认主题不使用这些伪类改变视觉。

因此默认 Light/Dark 渲染、pointerover、disabled 和 motion 视觉保持不变。应用可以在自定义主题中选择消费新增状态，但必须自行承担视觉和可访问性一致性。

## 7. 核心算法、数据流与生命周期

### 7.1 协商

`DragEnter` 和 `DragOver` 的输入是 DataTransfer 格式集合、源允许效果、DropZone 状态和所属 `Upload` 能力。算法只执行常量级检查：

1. DropZone 已启用且 `IsFileDropEnabled=true`。
2. 最近的 `Upload` 存在并可接收输入。
3. DataTransfer 包含文件格式。
4. 源允许效果包含 `Copy`。
5. 根据结果设置 `DragEffects`、`DragState` 和 `Handled`。

协商阶段不得读取文件值、元数据或目录内容。`Accepting` 表示数据格式和操作语义可进入 Drop 管线，不表示每个项目最终都能通过文件类型和业务准入。

### 7.2 Drop 快照

Drop 处理必须保持同步边界：

1. 重新执行协商。
2. 调用一次 typed `TryGetFiles()` 并取得独立的 `IStorageItem[]` 快照。
3. 按对象引用归一化重复项，并把 storage item ownership 同步交给输入批次。
4. 设置最终 `DragEffects` 和 `Handled`。
5. 将 `DragState` 恢复为 `None`。
6. 启动被 `Upload` 跟踪的批次操作。

完成第 4 步后不得访问 `DragEventArgs`、`IDataTransfer` 或延迟的文件 enumerable。Linux portal、X11 selection 和 Wayland pipe-backed 数据都必须在 routed event 有效期内完成顶层快照，异步任务只能持有独立的 `IStorageItem[]`。

`TryGetFiles()` 抛出异常时不创建虚构拒绝项。DropZone 恢复 `DragState=None`、返回 `DragDropEffects.None`，并通过 `InputBatchCompleted` 报告 `Failed/DataSnapshotFailed`；原始异常只由被观察的内部任务记录。数据格式不匹配属于协商失败，不启动输入批次。

### 7.3 目录遍历

目录只通过 `IStorageFolder.GetItemsAsync()` 遍历，不把 StorageItem 降级为 `DirectoryInfo` 或 `Directory.EnumerateFiles`。

- `Reject`：目录生成一个拒绝结果。
- `TopLevelFiles`：只输出根目录直接包含的文件，子目录作为不可展开项跳过。
- `RecursiveFiles`：按输入顺序和目录枚举顺序深度优先输出文件。
- 根目录深度为 `0`；进入子目录前检查 `MaxDirectoryDepth`。
- 每观察一个文件或目录递增枚举计数，超过 `MaxEnumeratedItems` 后终止当前目录树并报告限制拒绝。
- 有稳定 URI 时使用规范化 URI 检测已访问目录；无法取得稳定标识时依赖深度和计数限制保证终止。
- 访问失败只拒绝当前目录分支，不撤销同批次已接受的其他文件。
- 枚举返回的每个子项必须先交给批次 owner，再读取名称、路径或类型。
- 平台返回的重复对象引用在 adopt 前归一化；不同对象即使具有相同 URI 也分别处理。

`UploadInputCandidate` 只引用批次已拥有的 `IStorageFile`，不实现 `IDisposable`。目录在枚举 `finally` 中通过批次释放；拒绝文件通过批次释放；成功物化的文件执行 storage-to-source ownership 转换。Enumerator 不与批次同时拥有同一个对象。

### 7.4 Typed ownership

`UploadInputBatchOperation` 分别维护引用相等的 `IStorageItem` 集合和 `IUploadFileSourceLease` 集合，不使用泛化的 `HashSet<IDisposable>`：

```csharp
internal sealed class UploadInputBatchOperation : IDisposable
{
    private readonly HashSet<IStorageItem> _ownedStorageItems;
    private readonly HashSet<IUploadFileSourceLease> _ownedSourceLeases;

    internal void AdoptStorageItem(IStorageItem item);
    internal void ReleaseStorageItem(IStorageItem item);
    internal UploadStorageFileSource PromoteStorageFile(IStorageFile file);
    internal bool OwnsFileSource(UploadFileInfo file);
    internal void TransferFileSourceToUpload(UploadFileInfo file);
    internal void ReleaseFileSource(UploadFileInfo file);
}
```

ownership 状态转换为：

| 来源 | 操作 | 目标 | 失败语义 |
| --- | --- | --- | --- |
| 平台快照或枚举子项 | `AdoptStorageItem` | batch storage set | 重复 adopt 立即抛出；调用方释放尚未 adopt 的对象。 |
| batch storage file | `PromoteStorageFile` | batch source set | 先验证 owner；source 构造失败时释放 storage file。 |
| batch storage item | `ReleaseStorageItem` | disposed | 找不到 owner 时立即抛出。 |
| batch source lease | `ReleaseFileSource` | disposed | 找不到 owner 时立即抛出。 |
| batch source lease | `TransferFileSourceToUpload` | Upload accepted-source map | 找不到 owner 时立即抛出。 |
| Upload accepted source | remove/reset/detach | queue cancellation 后 disposed | upload execution 退出前不得释放。 |

同一对象不能同时存在于两个集合。`PromoteStorageFile` 必须以一个受保护的转换完成 storage owner 验证、storage set 移除、source 构造和 source set adopt。`Own(IDisposable)`、`Transfer(IDisposable)`、重复 `OwnFileSource` 和 `ownsFileSources` 参数全部移除。

批次释放必须逐项尝试全部剩余 StorageItem 和 source lease。单个 `Dispose` 异常不能截断后续释放；所有清理异常进入内部聚合，并在 owner set 清空后统一传播。公共批次结果不携带原始异常。

### 7.5 准入与数量提交

候选文件按以下顺序处理：

```text
StorageFile lease
  -> metadata
  -> AllowedFileTypes
  -> AdmissionPolicy
  -> CountOverflowBehavior
  -> UploadFileInfo/UploadFileItem
  -> UploadQueue
```

UI 线程快照不可变的 filename pattern、MIME 规则、AdmissionPolicy 引用、数量策略和 MaxCount。Worker 不读取调用方可变的规则集合；effective file count 不进入 worker snapshot，而是在 UI commit 紧邻集合变更时读取。准入成功后，文件 source lease 继续由批次持有，直到 UI commit；准入失败时由批次立即释放。

`RejectBatch` 在提交任何文件项前完成批次级数量判断，避免部分修改；`RejectExcess` 按稳定候选顺序填满剩余容量；`ReplaceExisting` 按空 replacement target 计算容量。外部 `Files` 可能在异步处理期间变化，因此普通数量策略在 UI commit 前重新验证可用容量。

### 7.6 UI commit 与 ReplaceExisting

commit 使用 typed batch operation，不通过通用 callback 表达 ownership：

```csharp
internal Task<IReadOnlyList<UploadFileInfo>> CommitInputFilesAsync(
    IReadOnlyList<UploadFileInfo> files,
    UploadInputBatchOperation operation,
    UploadInputPipelineOptions options);

internal Task<IReadOnlyList<UploadFileInfo>> ReplaceInputFilesAsync(
    IReadOnlyList<UploadFileInfo> files,
    UploadInputBatchOperation operation,
    UploadInputPipelineOptions options);
```

两个入口在变更集合前验证所有 lease-backed file 都由当前 batch operation 持有，并在同一个 UI callback 中应用当前容量。返回值只包含因 UI 时刻容量而拒绝的文件，由管线按候选顺序登记 `CountLimitExceeded` 并释放仍属 batch 的 lease。

`UploadFileItem` 加入 effective collection 且 accepted-file map 登记成功后，Upload 才调用 `TransferFileSourceToUpload`。只有完成 collection commit 与 ownership transfer 的文件进入 `AcceptedFiles`；commit 失败时尚未提交的 source 仍由批次释放，已完整提交的文件继续作为接受项报告。

`ReplaceExisting` 不再先异步取消旧队列，再回到 UI 线程清空列表。UI commit 先准备新 item，捕获当前旧 item，在一次连贯的集合变更中替换内容、登记并转移新 source，然后把实际移除的旧 file info 交给统一的 cancel-and-release 流程。旧 source 在对应 queue operation 退出前保持有效，新 source 在 commit 成功前保持 batch-owned，因此中间不存在可被外部集合修改放大的空窗期。

布尔模式方法拆为两个具名生命周期入口：

```csharp
private void ReplaceEffectiveFilesContents(
    IReadOnlyList<UploadFileItem> replacementItems);

private void ClearEffectiveFilesAfterQueueCancellation();
```

普通 `Files`/Form replacement 使用第一条路径并为实际移除项安排 cancel-and-release。`ResetAsync` 已等待 queue cancel-all，随后使用第二条路径直接释放 detached source。`queueAlreadyCancelled` 参数和 `Action<UploadFileInfo>` ownership callback 不再存在。

### 7.7 线程、取消和 Task 终态

批次进入串行 gate 并取得 UI option snapshot 后，只建立一次显式 worker dispatch。目录枚举、元数据读取和 AdmissionPolicy 包含在同一个 worker 边界内，不为每个文件创建 `Task.Run`。数量计算、effective collection 修改、Upload 生命周期事件和 `InputBatchCompleted` 只在 UI 线程执行。

一个 Upload 的输入批次串行执行，批次内候选顺序稳定。公共契约不额外承诺并发调用的 FIFO 顺序；数量和 replacement 结果由实际进入 gate 的批次决定。`CancelAllAsync` 关闭当前 generation，取消并等待所有已跟踪批次完成终态事件和资源释放；在 reset 边界结束后才允许新 generation 执行输入。

| Acquire | Release |
| --- | --- |
| Drop StorageItem 快照 | 拒绝、取消，或转移给 accepted file item |
| 目录枚举产生的子项 | 分支处理完成、拒绝、取消，或转移 |
| accepted file source lease | 文件 remove、reset、collection replacement 或 Upload detach |
| 批次 cancellation source | 批次完成、reset 或 detach |
| metadata/open Stream | 当前异步操作完成或异常 |

外部集合 remove/reset、`Files` replacement 和 Form Set/Clear 会同步更新文件状态，但 accepted source lease 必须先移交给被观察的取消清理任务，并在对应 upload execution 退出后释放。`ResetAsync` 和 detach 取消等待中的输入批次，并通过同一 lease owner 释放尚未提交的资源。

`InputBatchCompleted` 在清理完成后于 UI 线程恰好触发一次。单项拒绝使输入 Task 正常完成；批次取消先触发 `Cancelled` 事件，再向等待方传播 `OperationCanceledException`；批次失败先触发 `Failed` 事件，再传播原始异常或包含清理异常的聚合异常。DropZone 观察 fire-and-forget Task，不能留下未观察异常。

用户事件、Form 通知或 collection change 观察者抛出的异常不得成为资源释放边界。实现必须先完成 pending/running cancellation、accepted-source ownership 移交和 lease 释放，再向等待方传播原异常；并发的 cancel-all、transport 替换和并发度变更必须串行化，不能留下已经离开 pending queue 但仍为 Pending 的任务。

## 8. 资源、性能与 AOT 边界

- `DragOver` 热路径不分配文件列表，不执行同步 selection 读取，不启动异步任务。
- Drop 只创建一次顶层 StorageItem 数组；目录内容通过异步枚举流入管线，不无界物化整棵目录树。
- 每批次只有一次显式 worker dispatch；元数据读取和 AdmissionPolicy 按候选稳定顺序执行，不为每个文件创建无约束后台任务。
- typed owner set 和不可变 option snapshot 的容量受批次输入规模约束，并在批次终态清理。
- `IsDropProcessing` 由活动 Drop 批次数量计算，不通过延时或 suppression flag 修正。
- DragDrop handler、批次任务、取消源和 StorageItem lease 都有明确 owner，不依赖 GC 结束平台访问。
- 运行时路径使用静态注册、强类型 API 和显式策略，不使用反射扫描、动态类型发现或运行时代码生成。
- Public API、ControlTheme 和状态枚举对 trimmer 可见；该能力不需要 DynamicDependency、RUC fallback 或自定义 linker root。

## 9. 兼容性与定制边界

### 9.1 API 边界

以下职责归属是稳定契约：

- `Upload.IsOpenFileDialogOnClick` 迁移为 `UploadDropZone.IsOpenFileDialogOnClick`。
- `Upload.Accepts` 由 `Upload.AllowedFileTypes` 替代。
- `UploadDefaultDropArea.FilesDropped` 和 `UploadFilesDroppedEventArgs` 不再作为第二条数据出口。
- 输入结果统一通过 `Upload.InputBatchCompleted` 观察。
- `UploadTrigger`、`SelectFilesAsync`、`SelectDirectoriesAsync` 和程序化输入必须进入同一输入管线。

### 9.2 批次 API 变更

输入结果使用新的稳定状态模型，不保留 obsolete alias 或 compatibility wrapper：

| 旧契约 | 新契约 |
| --- | --- |
| `UploadInputBatchCompletedEventArgs.IsCancelled` | `Status == UploadInputBatchStatus.Cancelled` |
| `UploadRejectionReason.InputFailed` | `Status == Failed` 与 `FailureReason` |
| `UploadRejectionReason.Cancelled` | `Status == Cancelled` |
| `UploadRejectionReason.UnsupportedDataFormat` | 不创建批次，拖动协商返回 `None` |
| `MetadataReadFailed` / `ContentSourceCreationFailed` | `StorageReadFailed` |
| policy 异常形成 `AdmissionRejected` | `AdmissionPolicyFailed` |
| `UploadRejectedItem.Exception` | 删除；原始异常只进入内部诊断 |
| `new UploadAdmissionDecision(bool, string?, string?)` | `UploadAdmissionDecision.Accept()` 或 `.Reject(...)` |

### 9.3 渲染兼容边界

重构不得改变：

- ControlTheme key、模板节点名称和组合层级。
- `DropIcon`、`Header`、`SubHeader` 及对应 template 属性。
- Background、BorderBrush、BorderThickness、CornerRadius、Padding 和 alignment。
- pointerover、disabled、Light/Dark 和 motion 视觉。
- Upload Token 的名称、计算和消费位置。
- Measure、Arrange、最终 Bounds 和自定义 Content 布局。
- Gallery 拖动上传示例的默认截图。

新增状态只提供定制入口，不赋予默认主题新的渲染效果。应用替换 `UploadDropZone.Content` 或自定义主题时，不得绕过 DropZone 的行为 owner，也不得在内容控件中再次读取 DataTransfer。

## 10. 验证要求

| 层级 | 必须证明的设计不变量 |
| --- | --- |
| 纯逻辑 | 状态转换、Copy/None 协商、目录深度/计数、数量溢出和拒绝原因确定。 |
| headless 控件 | Enter/Over/Leave/Drop 路由、嵌套 DropZone、子元素切换、只读状态和 owner 失效。 |
| Public API | batch status/failure 不变量、rejection enum、AdmissionDecision factory 和 raw Exception 移除。 |
| 数据管线 | TryGetFiles 只调用一次，DragOver 不物化数据，文件/目录混合输入保持顺序；同步完成的 AdmissionPolicy 仍不在 UI 线程执行。 |
| typed ownership | duplicate adopt、non-owner transfer、storage-to-source promotion、拒绝、取消、失败和 commit 路径都恰好释放或转移一次。 |
| 异常聚合 | 单个 disposer 失败不截断其他释放，主异常与清理异常完整聚合，公共结果不包含原始异常。 |
| 生命周期 | accepted/rejected/cancel/reset/detach 及用户回调异常路径中的 StorageItem、Stream、Task 和 CTS 恰好释放一次；scheduler 维护操作保持调用顺序和队列一致性。 |
| replacement | `ReplaceExisting` 不提前释放旧 source，只取消实际移除项；Reset 不重复取消已经完成 cancel-all 的 queue generation。 |
| 主题契约 | 两个 ControlTheme 的视觉树、selector、Token、Transitions、Measure 和 Bounds 不变。 |
| Gallery | 现有拖动示例 AXAML 组合不变，Light/Dark 和不同缩放下截图与基线一致。 |
| Windows/macOS | 真实文件管理器的单文件、多文件、目录、混合输入和取消。 |
| Linux X11 | XDND Copy/None status、Drop-only selection 读取和跨进程输入。 |
| Linux Wayland | compositor 效果反馈、pipe-backed 数据一次物化和 Drop 后异步读取。 |
| NativeAOT | 控件测试和 Gallery 发布路径不引入 reflection、trim warning 或动态代码需求。 |

源码字符串检查只能作为结构辅助，不能替代状态机、输入管线、资源生命周期和平台集成测试。测试不得锁定 private 方法名称、boolean 参数或 callback 形状；视觉契约测试可以继续验证 public 类型、ControlTheme、selector、Token 和 Gallery AXAML 组合。
