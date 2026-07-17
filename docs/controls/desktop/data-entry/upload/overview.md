# Upload 桌面版架构设计

本文档定义 `Upload` 桌面版的目标设计、公共契约、状态模型、视觉主题关系和破坏性重构边界。通用控件研发约束见 [控件研发标准](../../../../engineering/control-development-guidelines.md)，内部实现原理见 [Upload 桌面版实现原理](implementation.md)，Upload Token 的专项设计见 [Upload Token 设计](token.md)，设计和契约变化记录见 [Upload Changelog](changelog.md)。

> 当前文档描述 `Unreleased` L3 重构目标。实现阶段必须先让源码、Gallery 和测试逐步对齐本文档，再视情况更新 LLMS 生成产物。

## 1. 控件定位

| 项 | 值 |
| --- | --- |
| NuGet 包 | `AtomUI.Desktop.Controls` |
| .NET 命名空间 | `AtomUI.Desktop.Controls` |
| AXAML 命名空间 | `https://atomui.net` |
| Gallery 页面 | `controlgallery/AtomUIGallery/ShowCases/DataEntry/Upload` |
| 控件状态 | Stable，`Unreleased` 中规划 L3 重构 |

Upload 是 AtomUI 桌面控件体系中的上传协调控件，用于管理文件选择、目录选择、拖拽提交、任务状态、列表展示、Form 值投影和上传操作入口。

Upload 不负责具体网络传输协议、文件存储服务或业务附件模型。这些职责由业务层或 `IFileUploadTransport` 承担。

主要源码入口：

- `src/AtomUI.Desktop.Controls/Upload`
- `src/AtomUI.Controls.Shared/Net`
- `controlgallery/AtomUIGallery/ShowCases/DataEntry/Upload`

## 2. 设计语言

Upload 的设计语言围绕“单一文件状态 owner + 可组合操作入口 + 独立列表视图”组织，而不是围绕某个固定模板节点组织。

| 维度 | 含义 | Upload 中的表达 |
| --- | --- | --- |
| 产品语义 | 控件在界面中承担的稳定职责。 | `Upload` 是上传状态协调器，统一管理文件、队列、选择入口、拖拽入口和列表视图。 |
| 内容承载 | 用户数据、展示内容、集合项或操作入口如何进入控件。 | `Files` 保存真实文件项；`TriggerContent`、`UploadTrigger`、`UploadDropZone` 和 `UploadList` 负责组合展示。 |
| 状态反馈 | public API、内部状态和伪类如何形成用户可感知反馈。 | `UploadFileItem.Status`、`Progress`、`ErrorMessage`、`Result` 是任务状态来源；Form 错误走 `DataValidationErrors`。 |
| 主题语义 | ControlTheme、SharedToken、组件 Token 和模板绑定如何表达视觉。 | Upload Token + ControlTheme；滚动边界在 `UploadList` 内部，并使用 AtomUI 自动隐藏滚动条。 |

## 3. API 与契约模型

Upload 的公共契约由 public/protected 类型成员、Avalonia 属性、事件、命令、template part、伪类、ControlTheme key 和资源 key 共同组成。维护时应先确认这些契约是否已经被源码、Gallery 示例或文档暴露。

### 3.1 单一状态 owner

Upload 以 `Files` 作为唯一上传文件状态 owner。触发器、拖拽区和列表只是组合视图或动作入口，不保存第二份任务状态。

- `Upload.Files` 表示所有可观察上传文件。
- `UploadTrigger` 通过 `SourceKind=Files|Directories` 选择文件或目录。
- `UploadDropZone` 将拖拽文件提交给最近的 `Upload`。
- `UploadList` 渲染 `Files`，并独立管理滚动区域。
- `PictureCard` / `PictureCircle` 的上传入口通过 append slot 呈现，不作为 `UploadFileItem`。

### 3.2 核心 public surface

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

### 3.3 新公共类型

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

### 3.4 破坏性 API 调整

| Removed API | Replacement |
| --- | --- |
| `TaskInfoList` | `Files` |
| `UploadTaskInfo` 作为 public 状态模型 | `UploadFileItem` |
| `DefaultTaskList` | 初始化或绑定 `Files` |
| `CurrentTaskList` | 不再公开或内部复制任务视图 |
| `IsUploadDirectoryEnabled` | `UploadTrigger.SourceKind=Directories` |
| `IsShowUploadTrigger` | 由用户布局控制 trigger 可见性 |
| fake picture trigger task | `TriggerContent` / display append slot |
| `UploadTriggerContent` 作为内部统一触发器 | public `UploadTrigger` |

### 3.5 推荐用法

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

## 4. 行为与状态模型

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

## 5. 视觉与主题模型

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

## 6. 控件家族或集成关系

主要协作类型：

- `Upload`：上传协调器，持有 public API、`Files`、上传队列、Form 适配和生命周期释放。
- `UploadFileItem`：文件状态模型，承载文件元数据、上传状态、进度、错误、结果和自定义 pending 文案。
- `UploadQueue`：内部上传队列协调器，映射 `UploadFileItem` 与 `FileUploadTask`。
- `UploadTrigger`：独立触发控件，按 `UploadSourceKind` 调用最近 `Upload` 的文件或目录选择方法。
- `UploadDropZone`：独立拖拽控件，将 drop 文件提交给最近 `Upload`。
- `UploadList`：列表视图控件，渲染 `Files` 并维护滚动边界。
- `AbstractUploadListItem`：列表项基类，只绑定 `UploadFileItem` 属性并发出 item action。
- `AbstractUploadPictureContent`：图片类内容基类，投射 `UploadFileItem` 的视觉状态。
- `FileUploadScheduler`：共享上传调度器，必须正确跟踪 pending/running/completed/cancelled 状态。
- `IFileUploadTransport`：业务上传传输边界。

集成关系：

- Gallery 展示新的组合用法，不再示例 `DefaultTaskList`、`IsUploadDirectoryEnabled` 或 `IsShowUploadTrigger`。
- Form 读取 `FileValueMode` 投影结果；Form 写入应更新 `Files` 内容而不是替换集合实例。
- `DataValidationErrors` 是 error 状态来源，Upload 不独立维护另一套 error 机制。

## 7. 兼容性不变量

本次重构是 L3 breaking change。维护 Upload 时必须保持以下新不变量：

- 不重新引入 `TaskInfoList`、`DefaultTaskList`、`CurrentTaskList` 或 fake trigger task。
- 不让视觉容器反向持有业务任务状态。
- 不用延时、强制刷新或 suppression flag 掩盖状态不同步。
- Template reapply、集合替换、remove、reset、detach 都必须释放旧订阅、取消运行任务和取消 pending auto-remove。
- 不通过运行时反射扫描 public API、Token 或 Gallery 表格数据。
- 文档只描述当前目标设计；历史变化记录在 `changelog.md`。

## 8. 专项模型

### 8.1 文件选择模型

`UploadTrigger.SourceKind` 决定点击后的选择动作：

- `Files`：打开文件选择器，支持 `Accepts` 和 `MaxCount`。
- `Directories`：打开目录选择器，枚举目录顶层文件并提交给 `Upload.EnqueueFilesAsync`。

文件和目录触发器可以同时存在于一个 `Upload.TriggerContent` 中。

### 8.2 列表滚动模型

`UploadList` 内部拥有滚动边界：

- `ListMaxHeight` 控制列表最大高度。
- `ListScrollBarVisibility` 控制垂直滚动条。
- 内部滚动容器应使用 AtomUI `ScrollViewer` 的 lite/auto-hide 模式。
- trigger 位于滚动区域外，用户滚动列表时上传按钮不移动。

### 8.3 自动移除模型

`SuccessAutoRemoveDelay` 不为空时，文件进入 success 后开始延迟移除。延迟任务由 `Upload` 统一管理，并在以下场景取消：

- 用户手动移除文件。
- 文件状态离开 success。
- `ResetAsync`。
- 控件 detach。

### 8.4 Form 与验证模型

`FileValueMode` 决定 Form 值：

- `AllFiles`：返回全部 `UploadFileItem`。
- `SuccessfulFiles`：返回成功文件。
- `Results`：返回成功文件的 `FileUploadResult`。

错误状态必须投射到 Avalonia `DataValidationErrors`，不得另建 Upload 专属 error 机制。

## 9. 文档导航、LLMS 导出与验证策略

关联文档：

- [Upload 桌面版实现原理](implementation.md)
- [Upload Token 设计](token.md)
- [Upload Changelog](changelog.md)

LLMS 语义区域：

| Part | AtomUI 节点 | 职责 | 相关 API | 相关 Token | 稳定性 |
| --- | --- | --- | --- | --- | --- |
| `root` | `Upload` | 上传状态协调器，拥有文件集合、上传队列、Form 值投影和生命周期。 | `Files`、`UploadTransport`、`FileValueMode` | `UploadToken`、SharedToken | stable |
| `trigger` | `TriggerContent` / `UploadTrigger` | 承载文件或目录选择入口，只提交选择动作，不持有上传状态。 | `TriggerContent`、`SourceKind`、`SelectFilesAsync()`、`SelectDirectoriesAsync()` | Upload trigger 主题资源 | stable |
| `drop-zone` | `UploadDropZone` | 接收拖拽文件并提交给最近的 `Upload`。 | `EnqueueFilesAsync()` | Upload drop-zone 主题资源 | stable |
| `list` | `UploadList` | 渲染 `Files` 并拥有列表滚动边界，不创建第二份文件状态。 | `Files`、`ListType`、`ListMaxHeight`、`ListScrollBarVisibility` | Upload list 主题资源 | internal-observable |
| `item` | `AbstractUploadListItem` 派生容器 | 投射单个 `UploadFileItem` 的状态、进度和操作入口。 | `UploadFileItem.Status`、`Progress`、`ErrorMessage`、`Result` | Upload item 主题资源 | internal-observable |
| `validation` | `Upload` Form / validation 投影 | 按 `FileValueMode` 输出 Form 值，并把错误投射到 `DataValidationErrors`。 | `FileValueMode`、`IFormItemAware` | SharedToken、Form Token | stable |

LLMS 导出来源：

| LLMS 内容 | 来源 | 说明 |
| --- | --- | --- |
| 单控件完整文档 | `overview.md` + Gallery API / Token / ShowCase | 生成 `controls/upload/index-cn.md` |
| 单控件语义文档 | `overview.md` + `implementation.md` + theme/template 信息 | 生成 `controls/upload/semantic-cn.md` |
| API 表 | Gallery ApiDataGrid 或源码 public surface | 不在 `overview.md` 中复制完整 API 表 |
| Design Token 表 | Gallery DesignTokenDataGrid、Token 类型或第 5 节主题模型 | 不在生成产物中手工维护第二份 Token 表 |
| 示例 | Gallery ShowCase + source snippet catalog | 只引用稳定示例 |
| 源码索引 | `implementation.md` | 用于定位控件源码、主题和测试 |

验证策略：

| 改动类型 | 验证要求 |
| --- | --- |
| 文档改动 | 运行 `git diff --check`，检查相对链接存在。 |
| Public API | 覆盖属性默认值、事件触发、命令和绑定语义。 |
| 状态模型 | 覆盖 add、remove、reset、detach、collection replacement、cancel 和 auto-remove。 |
| AXAML/Theme | 检查 trigger 固定、list 内部滚动、display append slot、template part、资源 key 和 Light/Dark 主题。 |
| Token | 检查 TokenKind、AXAML token resource、Gallery Token 表和文档同步。 |
| Gallery | 走查 fixed trigger、scrollable list、file/directory dual trigger 和 auto-remove 示例。 |
