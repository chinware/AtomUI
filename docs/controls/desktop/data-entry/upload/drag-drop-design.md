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
8. 单个文件或目录失败不终止同一批次中的其他项目。
9. 每个 `IStorageItem`、文件内容源、Stream、取消源和异步批次都有唯一释放 owner。
10. 默认 ControlTheme、视觉树、布局、Token 映射和最终渲染结果保持不变。

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
- `InputBatchCompleted` 在 UI 线程触发，每个批次恰好一次，并同时报告接受项、拒绝项和取消状态。

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

public sealed record UploadAdmissionDecision(
    bool IsAccepted,
    string? RejectionCode = null,
    string? Message = null);
```

策略只决定当前文件能否进入 Upload，不修改 `Files`、不创建任务，也不取得 StorageItem 的释放 ownership。

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

public sealed class UploadInputBatchCompletedEventArgs : EventArgs
{
    public Guid BatchId { get; }
    public UploadInputSource Source { get; }
    public IReadOnlyList<UploadFileInfo> AcceptedFiles { get; }
    public IReadOnlyList<UploadRejectedItem> RejectedItems { get; }
    public bool IsCancelled { get; }
}
```

一次输入批次至少包含：

- `BatchId`：批次稳定标识。
- `Source`：`FilePicker`、`DirectoryPicker`、`DragDrop` 或 `Programmatic`。
- `AcceptedFiles`：已经转移给 `Upload` 的文件信息。
- `RejectedItems`：项目名称、可用路径、拒绝原因和诊断异常。
- `IsCancelled`：批次是否在完成全部候选项前取消。

拒绝原因覆盖数据格式、项目类型、文件类型、数量、目录策略、遍历限制、访问权限、元数据读取、内容源创建、自定义准入和取消。拒绝项不进入 `Files`，也不创建上传任务。

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
| admission | `UploadFileAdmissionService` | 文件元数据、AllowedFileTypes、业务策略、剩余容量 | 接受或拒绝决定 | 视觉状态。 |
| storage source | `UploadStorageFileSource` | `IStorageFile` | 可重复请求的读取 Stream | 上传协议。 |
| batch lifetime | `UploadInputBatchOperation` | 快照、取消令牌 | 完成结果和资源释放 | 控件渲染。 |
| default visual | `UploadDefaultDropArea` | 内容属性和主题资源 | 当前默认 DropArea 视觉 | DragDrop、文件读取和 owner 查找。 |

这些职责属于稳定 ownership 边界。内部类型可以按现有文件组织合并，但不得重新形成两个拖动数据 owner 或让视觉控件持有上传任务。

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
3. 把快照中 storage item 的 lease 所有权交给输入批次。
4. 设置最终 `DragEffects` 和 `Handled`。
5. 将 `DragState` 恢复为 `None`。
6. 启动被 `Upload` 跟踪的批次操作。

完成第 4 步后不得访问 `DragEventArgs` 或 `IDataTransfer`。异步任务必须由批次 owner 观察，不使用未跟踪的嵌套 dispatcher async delegate。

### 7.3 目录遍历

目录只通过 `IStorageFolder.GetItemsAsync()` 遍历，不把 StorageItem 降级为 `DirectoryInfo` 或 `Directory.EnumerateFiles`。

- `Reject`：目录生成一个拒绝结果。
- `TopLevelFiles`：只输出根目录直接包含的文件，子目录作为不可展开项跳过。
- `RecursiveFiles`：按输入顺序和目录枚举顺序深度优先输出文件。
- 根目录深度为 `0`；进入子目录前检查 `MaxDirectoryDepth`。
- 每观察一个文件或目录递增枚举计数，超过 `MaxEnumeratedItems` 后终止当前目录树并报告限制拒绝。
- 有稳定 URI 时使用规范化 URI 检测已访问目录；无法取得稳定标识时依赖深度和计数限制保证终止。
- 访问失败只拒绝当前目录分支，不撤销同批次已接受的其他文件。

### 7.4 准入与数量提交

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

准入成功后，文件 source lease 从批次转移给对应文件项；准入失败时由批次立即释放。`RejectBatch` 在提交任何文件项前完成批次级数量判断，避免部分修改；`RejectExcess` 按稳定候选顺序填满剩余容量；`ReplaceExisting` 先通过 `Upload` 的统一 reset/remove 路径释放旧文件和任务，再提交新批次。

### 7.5 取消和释放

| Acquire | Release |
| --- | --- |
| Drop StorageItem 快照 | 拒绝、取消，或转移给 accepted file item |
| 目录枚举产生的子项 | 分支处理完成、拒绝、取消，或转移 |
| accepted file source lease | 文件 remove、reset、collection replacement 或 Upload detach |
| 批次 cancellation source | 批次完成、reset 或 detach |
| metadata/open Stream | 当前异步操作完成或异常 |

输入批次按到达顺序串行执行目录展开、元数据读取、准入和数量提交，使 `MaxCount` 结果确定，并避免 StorageItem 操作形成无界并发。外部集合 remove/reset、`Files` replacement 和 Form Set/Clear 会同步更新文件状态，但 accepted source lease 必须先移交给被观察的取消清理任务，并在对应 upload execution 退出后释放。`ResetAsync` 和 detach 取消等待中的输入批次，并通过同一 lease owner 释放尚未提交的资源。

用户事件、Form 通知或 collection change 观察者抛出的异常不得成为资源释放边界。实现必须先完成 pending/running cancellation、accepted-source ownership 移交和 lease 释放，再向等待方传播原异常；并发的 cancel-all、transport 替换和并发度变更必须串行化，不能留下已经离开 pending queue 但仍为 Pending 的任务。

## 8. 资源、性能与 AOT 边界

- `DragOver` 热路径不分配文件列表，不执行同步 selection 读取，不启动异步任务。
- Drop 只创建一次顶层 StorageItem 数组；目录内容通过异步枚举流入管线，不无界物化整棵目录树。
- 元数据读取按候选稳定顺序执行，不为每个文件创建无约束后台任务。
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

### 9.2 渲染兼容边界

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
| 数据管线 | TryGetFiles 只调用一次，DragOver 不物化数据，文件/目录混合输入保持顺序。 |
| 生命周期 | accepted/rejected/cancel/reset/detach 及用户回调异常路径中的 StorageItem、Stream、Task 和 CTS 恰好释放一次；scheduler 维护操作保持调用顺序和队列一致性。 |
| 主题契约 | 两个 ControlTheme 的视觉树、selector、Token、Transitions、Measure 和 Bounds 不变。 |
| Gallery | 现有拖动示例 AXAML 组合不变，Light/Dark 和不同缩放下截图与基线一致。 |
| Windows/macOS | 真实文件管理器的单文件、多文件、目录、混合输入和取消。 |
| Linux X11 | XDND Copy/None status、Drop-only selection 读取和跨进程输入。 |
| Linux Wayland | compositor 效果反馈、pipe-backed 数据一次物化和 Drop 后异步读取。 |
| NativeAOT | 控件测试和 Gallery 发布路径不引入 reflection、trim warning 或动态代码需求。 |

源码字符串检查只能作为结构辅助，不能替代状态机、输入管线、资源生命周期和平台集成测试。
