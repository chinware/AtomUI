# Upload 使用文档

本文档介绍 AtomUI Upload 控件的各种使用方式，示例代码摘自 Gallery 演示程序。

> 📖 Gallery 源码位置：`controlgallery/AtomUIGallery/ShowCases/Views/DataEntry/UploadShowCase.axaml`

---

## 前置准备

在 AXAML 中使用 Upload，需要引入以下命名空间：

```xml
xmlns:atom="https://atomui.net"
xmlns:antdicons="https://atomui.net/icons/antdesign"
```

在 C# 代码中引入：

```csharp
using AtomUI.Desktop.Controls;   // Upload, UploadDefaultDropArea, UploadTaskInfo 等
using AtomUI.Controls;            // IFileUploadTransport, FileUploadResult, UploadFileInfo 等
```

---

## 1. 基本点击上传

最基础的上传用法——点击按钮弹出文件选择对话框：

```xml
<atom:Upload Name="BasicUpload">
    <atom:Button Icon="{antdicons:AntDesignIconProvider UploadOutlined}">
        Click to Upload
    </atom:Button>
</atom:Upload>
```

```csharp
// Code-behind：设置上传传输实现
BasicUpload.UploadTransport = new MyUploadTransport();

// 监听事件
BasicUpload.UploadTaskCompleted += (sender, e) =>
{
    // 上传成功处理
    Console.WriteLine($"{e.UploadFileInfo.Name} uploaded successfully!");
};
BasicUpload.UploadTaskFailed += (sender, e) =>
{
    // 上传失败处理
    Console.WriteLine($"Upload failed: {e.Result.UserFriendlyMessage}");
};
```

**要点**：Upload 控件的 `Content` 可以是任何控件，点击触发区域时会自动弹出文件选择对话框。必须设置 `UploadTransport` 属性才能实际上传文件。

---

## 2. 头像上传（PictureCard / PictureCircle）

使用 `PictureCard` 或 `PictureCircle` 列表类型实现头像上传效果：

```xml
<StackPanel Orientation="Horizontal" Spacing="20">
    <!-- 卡片形式 -->
    <atom:Upload Name="AvatarCardUpload" ListType="PictureCard">
        <StackPanel Orientation="Vertical">
            <Panel>
                <antdicons:PlusOutlined
                    IsVisible="{Binding IsTaskRunning,
                                RelativeSource={RelativeSource AncestorType=atom:Upload},
                                Converter={x:Static BoolConverters.Not}}" />
                <antdicons:LoadingOutlined
                    IsVisible="{Binding IsTaskRunning,
                                RelativeSource={RelativeSource AncestorType=atom:Upload}}"
                    LoadingAnimation="Spin" />
            </Panel>
            <TextBlock Margin="0, 8, 0, 0">Upload</TextBlock>
        </StackPanel>
    </atom:Upload>

    <!-- 圆形形式 -->
    <atom:Upload Name="AvatarCircleUpload" ListType="PictureCircle">
        <StackPanel Orientation="Vertical">
            <antdicons:PlusOutlined />
            <TextBlock Margin="0, 8, 0, 0">Upload</TextBlock>
        </StackPanel>
    </atom:Upload>
</StackPanel>
```

**要点**：
- `PictureCard` 和 `PictureCircle` 类型的触发按钮会嵌入到图片网格中，显示为一个带「+」的卡片。
- 可通过 `IsTaskRunning` 属性绑定动态切换加载图标。

---

## 3. 默认文件列表

使用 `DefaultTaskList` 属性在页面初始化时展示已有文件：

```xml
<atom:Upload Name="DefaultFileList">
    <atom:Button Icon="{antdicons:AntDesignIconProvider UploadOutlined}">
        Click to Upload
    </atom:Button>
</atom:Upload>
```

```csharp
// Code-behind 或 ViewModel 中设置默认列表
var defaultTasks = new List<UploadTaskInfo>
{
    new UploadTaskInfo
    {
        TaskId      = Guid.NewGuid(),
        FileName    = "xxx.png",
        IsImageFile = true,
        Status      = FileUploadStatus.Uploading,
        Progress    = 33
    },
    new UploadTaskInfo
    {
        TaskId      = Guid.NewGuid(),
        FileName    = "yyy.png",
        IsImageFile = true,
        Status      = FileUploadStatus.Success,
        Progress    = 100
    },
    new UploadTaskInfo
    {
        TaskId       = Guid.NewGuid(),
        FileName     = "zzz.png",
        IsImageFile  = true,
        Status       = FileUploadStatus.Failed,
        ErrorMessage = "Server Error 500"
    }
};
DefaultFileList.DefaultTaskList = defaultTasks;
```

**要点**：`DefaultTaskList` 在控件首次加载时应用一次，支持展示不同状态（上传中、成功、失败）的文件。

---

## 4. 图片墙（Pictures Wall）

使用 `PictureCard` 列表类型实现图片墙效果，配合默认文件列表展示已有图片：

```xml
<atom:Upload Name="PicturesWallUpload" ListType="PictureCard">
    <StackPanel Orientation="Vertical">
        <antdicons:PlusOutlined />
        <TextBlock Margin="0, 8, 0, 0">Upload</TextBlock>
    </StackPanel>
</atom:Upload>
```

```csharp
// 设置已有图片列表
PicturesWallUpload.DefaultTaskList = new List<UploadTaskInfo>
{
    new UploadTaskInfo
    {
        TaskId      = Guid.NewGuid(),
        FileName    = "image.png",
        IsImageFile = true,
        Status      = FileUploadStatus.Success,
        FilePath    = new Uri("avares://AtomUIGallery/Assets/ImagePreviewerShowCase/1.png")
    },
    // ... 更多图片
};
PicturesWallUpload.UploadTransport = new MyUploadTransport();
```

**要点**：
- `FilePath` 可以使用 `avares://` 协议引用应用程序内嵌资源。
- 成功上传的图片会自动显示缩略图预览。
- 悬浮已上传图片时会显示遮罩层和操作按钮（预览/删除）。

---

## 5. 拖拽上传

使用 `UploadDefaultDropArea` 作为触发区内容，支持拖拽文件上传：

```xml
<atom:Upload Name="DragAndDropUpload">
    <atom:UploadDefaultDropArea />
</atom:Upload>
```

```csharp
DragAndDropUpload.UploadTransport = new MyUploadTransport();
```

**自定义拖拽区域内容**：

```xml
<atom:Upload Name="CustomDragUpload">
    <atom:UploadDefaultDropArea
        Header="拖拽文件到此处上传"
        SubHeader="支持 PNG、JPG、PDF 格式">
        <!-- DropIcon 默认为 InboxOutlined，可自定义 -->
    </atom:UploadDefaultDropArea>
</atom:Upload>
```

**要点**：
- `UploadDefaultDropArea` 开箱即用，已内置 `DragDrop.AllowDrop` 和虚线边框样式。
- 悬浮时边框高亮为主色调。
- `Header` 默认为本地化文本「Click or drag file to this area to upload」。

---

## 6. 图片列表（Picture List）

使用 `Picture` 列表类型，在文本列表基础上增加缩略图预览：

```xml
<atom:Upload Name="PictureListUpload" ListType="Picture">
    <atom:Button Icon="{antdicons:AntDesignIconProvider UploadOutlined}" ButtonType="Primary">
        Click to Upload
    </atom:Button>
</atom:Upload>
```

---

## 7. 最大文件数限制

通过 `MaxCount` 属性限制上传文件数量：

```xml
<!-- MaxCount=1：新文件替换旧文件 -->
<atom:Upload Name="MaxCount1Upload" MaxCount="1">
    <atom:Button Icon="{antdicons:AntDesignIconProvider UploadOutlined}">
        Upload (Max: 1)
    </atom:Button>
</atom:Upload>

<!-- MaxCount=3 + 多选：最多3个文件 -->
<atom:Upload Name="MaxCount3Upload" MaxCount="3" IsMultipleEnabled="True">
    <atom:Button Icon="{antdicons:AntDesignIconProvider UploadOutlined}">
        Upload (Max: 3)
    </atom:Button>
</atom:Upload>
```

**行为说明**：
- `MaxCount=1` 时，选择新文件会自动取消并替换现有文件。
- `MaxCount>1` 时，达到上限后选择的新文件会被静默忽略。

---

## 8. 目录上传

启用 `IsUploadDirectoryEnabled` 属性可选择整个目录上传：

```xml
<atom:Upload Name="DirectoryUpload" IsUploadDirectoryEnabled="True">
    <atom:Button Icon="{antdicons:AntDesignIconProvider UploadOutlined}">
        Upload Directory
    </atom:Button>
</atom:Upload>
```

**要点**：选择目录后，会遍历目录中的所有文件（仅顶层）逐一创建上传任务。

---

## 9. 文件类型过滤

### 使用 Accepts 属性过滤（对话框级别）

```xml
<atom:Upload Name="PngOnlyUpload" Accepts="*.png">
    <atom:Button Icon="{antdicons:AntDesignIconProvider UploadOutlined}">
        Upload png only
    </atom:Button>
</atom:Upload>
```

### 使用 UploadTaskAboutToScheduling 事件拦截（逻辑级别）

```csharp
// 仅允许 JPG/PNG 且小于 2MB 的图片
AvatarUpload.UploadTaskAboutToScheduling += (sender, e) =>
{
    var fileInfo = e.UploadFileInfo;
    var ext = Path.GetExtension(fileInfo.FilePath.LocalPath);

    // 文件类型校验
    if (ext != ".jpeg" && ext != ".jpg" && ext != ".png")
    {
        e.Result       = UploadPredicateResult.CancelWithInTaskList;
        e.CancelReason = "You can only upload JPG/PNG file!";
        return;
    }

    // 文件大小校验
    var isLt2M = (double)fileInfo.Size / 1024 / 1024 < 2;
    if (!isLt2M)
    {
        e.Result       = UploadPredicateResult.CancelWithInTaskList;
        e.CancelReason = "Image must smaller than 2MB!";
    }
};
```

```csharp
// 仅允许 PNG 文件，不符合条件的文件从列表中隐藏
PngOnlyUpload.UploadTaskAboutToScheduling += (sender, e) =>
{
    var ext = Path.GetExtension(e.UploadFileInfo.FilePath.LocalPath);
    if (ext != ".png")
    {
        e.Result       = UploadPredicateResult.Cancel;      // 不显示在列表中
        e.CancelReason = "You can only upload PNG file!";
    }
};
```

**`UploadPredicateResult` 行为对比**：
- `Cancel`：文件不显示在列表中（对标 Ant Design 的 `Upload.LIST_IGNORE`）
- `CancelWithInTaskList`：文件显示在列表中但标记为失败

---

## 10. 实现自定义上传传输

实现 `IFileUploadTransport` 接口提供实际的上传逻辑：

```csharp
public class HttpUploadTransport : IFileUploadTransport
{
    private readonly HttpClient _httpClient;
    private readonly string _uploadUrl;

    public HttpUploadTransport(string uploadUrl)
    {
        _httpClient = new HttpClient();
        _uploadUrl  = uploadUrl;
    }

    public async Task<FileUploadResult> UploadAsync(
        UploadFileInfo fileInfo,
        object? context = null,
        IProgress<FileUploadProgress>? progress = null,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var startTime = DateTime.Now;
            using var stream  = File.OpenRead(fileInfo.FilePath.LocalPath);
            using var content = new StreamContent(stream);
            using var form    = new MultipartFormDataContent();
            form.Add(content, "file", fileInfo.Name);

            var response = await _httpClient.PostAsync(_uploadUrl, form, cancellationToken);
            response.EnsureSuccessStatusCode();

            var elapsed = DateTime.Now - startTime;
            return FileUploadResult.SuccessResult(
                new Uri($"{_uploadUrl}/{fileInfo.Name}"),
                fileInfo.Size,
                elapsed);
        }
        catch (OperationCanceledException)
        {
            throw;
        }
        catch (Exception ex)
        {
            return FileUploadResult.FailureResult(
                FileUploadErrorCode.NetworkError,
                $"Upload failed: {ex.Message}",
                exception: ex);
        }
    }
}
```

```csharp
// 使用自定义传输
myUpload.UploadTransport = new HttpUploadTransport("https://api.example.com/upload");
```

---

## 11. 上传事件处理与消息反馈

结合消息提示组件提供上传反馈：

```csharp
// 上传失败时显示错误消息
upload.UploadTaskFailed += (sender, e) =>
{
    messageManager?.Show(new Message(
        type: MessageType.Error,
        content: e.Result.UserFriendlyMessage ?? "Upload failed"
    ));
};

// 上传成功时显示成功消息
upload.UploadTaskCompleted += (sender, e) =>
{
    messageManager?.Show(new Message(
        type: MessageType.Success,
        content: $"{e.UploadFileInfo.Name} upload successfully!"
    ));
};
```

> 📖 参考：`controlgallery/AtomUIGallery/ShowCases/Views/DataEntry/UploadShowCase.axaml.cs`

---

## 12. 控制显示/隐藏

```xml
<!-- 隐藏文件列表（仅保留触发区） -->
<atom:Upload IsShowUploadList="False">
    <atom:Button>Upload</atom:Button>
</atom:Upload>

<!-- 隐藏触发区（仅保留文件列表） -->
<atom:Upload IsShowUploadTrigger="False" />

<!-- 禁用点击打开文件对话框 -->
<atom:Upload IsOpenFileDialogOnClick="False">
    <atom:UploadDefaultDropArea />
</atom:Upload>
```

---

## 13. 重置上传控件

```csharp
// 异步重置（取消所有任务并清空列表）
await myUpload.ResetAsync();

// 同步版本（内部调度到 UI 线程）
myUpload.Reset();
```

---

## 常见组合模式

### 带反馈的完整上传流程

```xml
<atom:Upload Name="MyUpload">
    <atom:Button Icon="{antdicons:AntDesignIconProvider UploadOutlined}" ButtonType="Primary">
        Click to Upload
    </atom:Button>
</atom:Upload>
```

```csharp
// 初始化
MyUpload.UploadTransport = new HttpUploadTransport("https://api.example.com/upload");
MyUpload.MaxConcurrentTasks = 2;

// 上传前校验
MyUpload.UploadTaskAboutToScheduling += (s, e) =>
{
    if (e.UploadFileInfo.Size > 10 * 1024 * 1024)
    {
        e.Result = UploadPredicateResult.CancelWithInTaskList;
        e.CancelReason = "File must be smaller than 10MB";
    }
};

// 事件监听
MyUpload.UploadTaskCompleted += HandleCompleted;
MyUpload.UploadTaskFailed    += HandleFailed;

// 清理
Disposable.Create(() =>
{
    MyUpload.UploadTransport      = null;
    MyUpload.UploadTaskCompleted -= HandleCompleted;
    MyUpload.UploadTaskFailed    -= HandleFailed;
    MyUpload.Reset();
});
```

### 使用 ReactiveUI 绑定

```csharp
this.WhenActivated(disposables =>
{
    this.OneWayBind(ViewModel, m => m.DefaultFiles, v => v.MyUpload.DefaultTaskList)
        .DisposeWith(disposables);
});
```
