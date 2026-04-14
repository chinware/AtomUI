# Upload API 参考

## 命名空间

```csharp
namespace AtomUI.Desktop.Controls;
```

AXAML 命名空间前缀：`atom`（`xmlns:atom="https://atomui.net"`）

---

## 枚举类型

### UploadListType

上传列表展示类型枚举，控制文件列表的外观和布局。

| 值 | 说明 |
|---|---|
| `Text` | 文本列表，仅显示文件名、进度条和操作按钮 |
| `Picture` | 图片列表，在文本列表基础上增加缩略图预览 |
| `PictureCard` | 图片卡片墙，以卡片网格形式展示，触发按钮嵌入网格中 |
| `PictureCircle` | 圆形图片墙，与 PictureCard 类似但使用圆形展示 |

### UploadPredicateResult

上传拦截结果枚举，用于 `UploadTaskAboutToScheduling` 事件中控制上传行为。

| 值 | 说明 |
|---|---|
| `Schedule` | 正常调度上传（默认值） |
| `Cancel` | 取消上传，文件不显示在列表中（对标 Ant Design `LIST_IGNORE`） |
| `CancelWithInTaskList` | 取消上传，但文件显示在列表中并标记为失败 |

### FileUploadStatus（来自 `AtomUI.Controls`）

文件上传状态枚举，定义于设备无关层 `AtomUI.Controls.Shared`。

| 值 | 说明 |
|---|---|
| `Pending` | 等待调度 |
| `Uploading` | 上传中 |
| `Success` | 上传成功 |
| `Failed` | 上传失败 |
| `Cancelled` | 已取消 |

### FileUploadErrorCode（来自 `AtomUI.Controls`）

文件上传错误码枚举，定义于设备无关层 `AtomUI.Controls.Shared`。

| 值 | 说明 |
|---|---|
| `None` | 无错误 |
| `NetworkError` | 网络连接/超时问题 |
| `ServerError` | 服务器 5xx 错误 |
| `ClientError` | 服务器 4xx 错误（如认证失败、参数错误） |
| `FileError` | 本地文件错误（找不到、无权限） |
| `ValidationFailed` | 验证失败（如文件类型、大小不符） |
| `Cancelled` | 操作被取消 |
| `Unknown` | 未知错误 |

---

## Upload 公共属性

### StyledProperty

| 属性名 | 类型 | 默认值 | 说明 |
|---|---|---|---|
| `Accepts` | `IReadOnlyList<string>?` | `null` | 文件类型过滤列表，支持模式匹配（如 `*.png`、`*.jpg`），应用于文件选择对话框 |
| `ExtraContext` | `object?` | `null` | 附加上下文对象，随上传任务传递到 `IFileUploadTransport.UploadAsync` 的 `context` 参数 |
| `MaxCount` | `int` | `int.MaxValue` | 最大文件数量限制。`1` 时新文件替换旧文件，`>1` 时达上限后忽略新文件 |
| `IsUploadDirectoryEnabled` | `bool` | `false` | 是否启用目录上传模式（弹出文件夹选择对话框） |
| `ListType` | `UploadListType` | `UploadListType.Text` | 文件列表展示类型 |
| `IsMultipleEnabled` | `bool` | `false` | 是否允许多选文件 |
| `IsOpenFileDialogOnClick` | `bool` | `true` | 点击触发区时是否打开文件选择对话框 |
| `IsShowUploadList` | `bool` | `true` | 是否显示文件列表 |
| `IsShowUploadTrigger` | `bool` | `true` | 是否显示上传触发区 |
| `DefaultTaskList` | `IList<UploadTaskInfo>?` | `null` | 默认文件列表，页面初始化时展示已有文件 |
| `MaxConcurrentTasks` | `int` | `3` | 最大并发上传任务数 |
| `IsMotionEnabled` | `bool` | 跟随全局 Token | 是否启用过渡动画（共享属性，通过 `AddOwner` 注册） |

### DirectProperty

| 属性名 | 类型 | 默认值 | 说明 |
|---|---|---|---|
| `UploadTransport` | `IFileUploadTransport?` | `null` | 上传传输实现，未设置时文件选择后不会启动上传 |
| `IsTaskRunning` | `bool` | `false` | 只读，是否有上传任务正在运行 |

### 公共集合属性

| 属性名 | 类型 | 说明 |
|---|---|---|
| `TaskInfoList` | `AvaloniaList<UploadTaskInfo>` | 当前文件列表（可观察集合），包含所有上传任务的信息 |

### 公共回调

| 属性名 | 类型 | 说明 |
|---|---|---|
| `IsImageFilePredicate` | `Func<UploadFileInfo, bool>?` | 自定义图片文件判断回调。未设置时使用内置的扩展名匹配（支持 `.webp`、`.svg`、`.png`、`.gif`、`.jpg` 等 12 种格式） |

### 继承自 ContentControl 的常用属性

| 属性名 | 类型 | 默认值 | 说明 |
|---|---|---|---|
| `Content` | `object?` | `null` | 上传触发区内容（如 `Button`、`UploadDefaultDropArea`） |
| `ContentTemplate` | `IDataTemplate?` | `null` | 内容模板 |
| `HorizontalContentAlignment` | `HorizontalAlignment` | `Left` | 触发区内容水平对齐 |
| `VerticalContentAlignment` | `VerticalAlignment` | `Center` | 触发区内容垂直对齐 |
| `IsEnabled` | `bool` | `true` | 是否启用 |

---

## Upload 公共方法

| 方法名 | 返回类型 | 说明 |
|---|---|---|
| `ResetAsync()` | `Task` | 异步重置控件：取消所有正在进行的上传任务并清空文件列表 |
| `Reset()` | `void` | 同步版重置，内部调度到 UI 线程异步执行取消操作 |

---

## Upload 事件

| 事件名 | 事件参数类型 | 说明 |
|---|---|---|
| `UploadTaskCreated` | `UploadTaskCreatedEventArgs` | 文件选择后、上传任务创建时触发 |
| `UploadTaskAboutToScheduling` | `UploadTaskAboutToSchedulingEventArgs` | 任务即将调度上传前触发，可通过设置 `Result` 属性拦截上传 |
| `UploadTaskProgress` | `UploadTaskProgressEventArgs` | 上传进度更新时触发 |
| `UploadTaskCompleted` | `UploadTaskCompletedEventArgs` | 上传成功完成时触发 |
| `UploadTaskFailed` | `UploadTaskFailedEventArgs` | 上传失败时触发 |
| `UploadTaskCancelled` | `UploadTaskCancelledEventArgs` | 上传被取消时触发 |
| `UploadTaskRemoved` | `UploadTaskRemovedEventArgs` | 文件从列表中移除时触发 |

### 事件参数详情

#### UploadTaskCreatedEventArgs

| 属性 | 类型 | 说明 |
|---|---|---|
| `TaskId` | `Guid` | 上传任务唯一标识 |
| `UploadFileInfo` | `UploadFileInfo` | 上传文件信息 |

#### UploadTaskAboutToSchedulingEventArgs

| 属性 | 类型 | 说明 |
|---|---|---|
| `TaskId` | `Guid` | 上传任务唯一标识 |
| `UploadFileInfo` | `UploadFileInfo` | 上传文件信息 |
| `Result` | `UploadPredicateResult` | 可设置的拦截结果（默认 `Schedule`） |
| `CancelReason` | `string?` | 取消原因说明（设置 `Result` 为取消时使用） |

#### UploadTaskProgressEventArgs

| 属性 | 类型 | 说明 |
|---|---|---|
| `TaskId` | `Guid` | 上传任务唯一标识 |
| `UploadFileInfo` | `UploadFileInfo` | 上传文件信息 |
| `Progress` | `double` | 上传进度百分比（0~100） |

#### UploadTaskCompletedEventArgs

| 属性 | 类型 | 说明 |
|---|---|---|
| `TaskId` | `Guid` | 上传任务唯一标识 |
| `UploadFileInfo` | `UploadFileInfo` | 上传文件信息 |
| `Result` | `FileUploadResult` | 上传结果（包含远端 URL、文件 ID 等信息） |

#### UploadTaskFailedEventArgs

| 属性 | 类型 | 说明 |
|---|---|---|
| `TaskId` | `Guid` | 上传任务唯一标识 |
| `UploadFileInfo` | `UploadFileInfo` | 上传文件信息 |
| `Result` | `FileUploadResult` | 失败结果（包含错误码和错误消息） |

#### UploadTaskCancelledEventArgs

| 属性 | 类型 | 说明 |
|---|---|---|
| `TaskId` | `Guid` | 上传任务唯一标识 |
| `UploadFileInfo` | `UploadFileInfo` | 上传文件信息 |
| `Result` | `FileUploadResult` | 取消结果 |

#### UploadTaskRemovedEventArgs

| 属性 | 类型 | 说明 |
|---|---|---|
| `TaskId` | `Guid` | 上传任务唯一标识 |
| `UploadFileInfo` | `UploadFileInfo` | 上传文件信息 |

---

## UploadTaskInfo 属性

`UploadTaskInfo` 继承自 `AvaloniaObject`，表示文件列表中每个上传任务的信息。

| 属性名 | 类型 | 说明 |
|---|---|---|
| `TaskId` | `Guid` | 上传任务唯一标识 |
| `FileName` | `string?` | 文件名 |
| `Progress` | `double` | 上传进度（0~100） |
| `IsImageFile` | `bool` | 是否为图片文件 |
| `Status` | `FileUploadStatus` | 上传状态 |
| `ErrorMessage` | `string?` | 错误消息（失败时使用） |
| `FilePath` | `Uri?` | 文件路径（本地路径或 `avares://` 资源路径） |

---

## UploadDefaultDropArea 公共属性

`UploadDefaultDropArea` 是公共拖拽上传区域控件，可作为 `Upload.Content` 使用。

| 属性名 | 类型 | 默认值 | 说明 |
|---|---|---|---|
| `DropIcon` | `PathIcon?` | `InboxOutlined` | 拖拽区域图标 |
| `Header` | `object?` | 本地化字符串 `DragUploadHead` | 拖拽区域主标题 |
| `HeaderTemplate` | `IDataTemplate?` | `null` | 主标题模板 |
| `SubHeader` | `object?` | `null` | 拖拽区域副标题 |
| `SubHeaderTemplate` | `IDataTemplate?` | `null` | 副标题模板 |
| `IsMotionEnabled` | `bool` | 跟随全局 Token | 是否启用过渡动画 |

### UploadDefaultDropArea 事件

| 事件名 | 事件参数类型 | 说明 |
|---|---|---|
| `FilesDropped` | `UploadFilesDroppedEventArgs` | 文件拖放到区域时触发（冒泡路由事件） |

#### UploadFilesDroppedEventArgs

| 属性 | 类型 | 说明 |
|---|---|---|
| `Files` | `IReadOnlyList<IStorageFile>` | 被拖放的文件列表 |

---

## 设备无关层类型（AtomUI.Controls.Shared）

### IFileUploadTransport 接口

```csharp
namespace AtomUI.Controls;

public interface IFileUploadTransport
{
    Task<FileUploadResult> UploadAsync(
        UploadFileInfo fileInfo,
        object? context = null,
        IProgress<FileUploadProgress>? progress = null,
        CancellationToken cancellationToken = default);
}
```

### UploadFileInfo record

| 属性 | 类型 | 说明 |
|---|---|---|
| `Name` | `string` | 文件名 |
| `FilePath` | `Uri` | 文件路径 |
| `Size` | `long` | 文件大小（字节） |
| `DateCreated` | `DateTimeOffset?` | 文件创建时间 |
| `DateModified` | `DateTimeOffset?` | 文件修改时间 |

### FileUploadProgress record

| 属性 | 类型 | 说明 |
|---|---|---|
| `BytesSent` | `long` | 已发送字节数 |
| `TotalBytes` | `long` | 总字节数 |
| `Percentage` | `double` | 进度百分比（只读，自动计算） |

### FileUploadResult record

| 属性 | 类型 | 说明 |
|---|---|---|
| `Status` | `FileUploadStatus` | 上传最终状态 |
| `IsSuccess` | `bool` | 是否成功（只读便捷属性） |
| `ErrorCode` | `FileUploadErrorCode` | 分类错误码 |
| `UserFriendlyMessage` | `string?` | 用户友好的错误消息 |
| `InternalErrorMessage` | `string?` | 内部错误信息（用于调试） |
| `Exception` | `Exception?` | 关联的异常 |
| `RemoteUrl` | `Uri?` | 文件在服务器上的访问地址 |
| `FileId` | `string?` | 服务器返回的文件标识 |
| `FileHash` | `string?` | ETag 或文件哈希 |
| `FileSize` | `long` | 文件大小（字节） |
| `ElapsedTime` | `TimeSpan` | 上传耗时 |
| `AverageSpeed` | `double` | 平均上传速度（字节/秒，只读） |
| `Metadata` | `Dictionary<string, object>?` | 服务器返回的自定义元数据 |

**静态工厂方法：**

| 方法 | 说明 |
|---|---|
| `SuccessResult(remoteUrl, fileSize, elapsedTime, ...)` | 创建成功结果 |
| `FailureResult(errorCode, userFriendlyMessage, ...)` | 创建失败结果 |
| `CancelledResult(userFriendlyMessage)` | 创建取消结果 |

---

## 实现的接口

### IMotionAwareControl

```csharp
public bool IsMotionEnabled { get; set; }
```

控制 Upload 及其子组件的过渡动画开关。

### IFormItemAware

可作为 `FormItem` 的子控件参与表单验证流程。提供 `ValueChanged` 事件、`SetFormValue` / `GetFormValue` / `ClearFormValue` 方法。

> 注意：Upload 的表单值定义目前尚在开发中（源码中标注 TODO）。
