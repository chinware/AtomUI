# Upload

> 生成产物：由源文档生成，不要手工编辑。修改内容请回到控件文档、Gallery API / Token 表、Gallery ShowCase 或源码结构。

## 概述

Upload 是 AtomUI 桌面控件体系中的上传控件，用于管理文件选择、任务状态、列表展示和上传操作入口。

Upload 不负责具体网络传输协议、文件存储服务或业务附件模型。这些职责应由业务层、组合控件或更专用的 AtomUI 控件承担。

主要源码入口：

- `src/AtomUI.Desktop.Controls/Upload`

## 包与命名空间

| 项 | 值 |
| --- | --- |
| NuGet 包 | `AtomUI.Desktop.Controls` |
| .NET 命名空间 | `AtomUI.Desktop.Controls` |
| AXAML 命名空间 | `https://atomui.net` |
| Gallery 页面 | `controlgallery/AtomUIGallery/ShowCases/DataEntry/Upload` |
| 状态 | Stable |

## 何时使用

Upload 的设计语言围绕控件职责、可观察状态和主题契约组织，而不是围绕模板节点组织。

| 维度 | 含义 | Upload 中的表达 |
| --- | --- | --- |
| 产品语义 | 控件在界面中承担的稳定职责。 | Upload 是 AtomUI 桌面控件体系中的上传控件，用于管理文件选择、任务状态、列表展示和上传操作入口。 |
| 内容承载 | 用户数据、展示内容、集合项或操作入口如何进入控件。 | `DropIcon`、`FileName`、`FilePath`、`Header`、`HeaderTemplate`、`IsImageFile`、`IsOpenFileDialogOnClick`、`IsShowUploadList` 等 15 项。 |
| 状态反馈 | public API、内部状态和伪类如何形成用户可感知反馈。 | open/close、loading/async、motion。 |
| 主题语义 | ControlTheme、SharedToken、组件 Token 和模板绑定如何表达视觉。 | Upload Token + ControlTheme。 |

## 公共 API

Upload 的公共契约由 public/protected 类型成员、Avalonia 属性、事件、命令、template part、伪类、ControlTheme key 和资源 key 共同组成。维护时应先确认这些契约是否已经被源码、Gallery 示例或文档暴露。

核心 public surface 按语义分组维护：

| 契约组 | 代表成员 | 维护含义 |
| --- | --- | --- |
| 内容与数据 | `DropIcon`、`FileName`、`FilePath`、`Header`、`HeaderTemplate`、`IsImageFile`、`IsOpenFileDialogOnClick`、`IsShowUploadList`、`IsShowUploadTrigger`、`IsUploadDirectoryEnabled` 等 15 项 | 定义控件展示内容、输入数据、模板或业务对象入口。 |
| 选择与集合 | `IsMultipleEnabled`、`MaxCount` | 维护选择、展开、过滤、分页、分组或集合状态。 |
| 交互与状态 | `IsMotionEnabled`、`IsTaskRunning`、`Status` | 表达用户可观察状态、可用性、清除、加载或反馈语义。 |
| 动效与异步 | `MaxConcurrentTasks`、`Progress`、`TaskId` | 约束动效开关、异步加载、播放速度、超时和任务边界。 |
| 其他稳定入口 | `ErrorMessage`、`ExtraContext`、`ListType` | 保留为 public surface，变更前需确认 Gallery 和用户 XAML 依赖。 |

稳定事件包括 `FileSelectRequest`、`FilesDropped`、`TaskRemoveRequest`。事件触发顺序属于兼容契约，不能因内部状态重排而改变。

主要公开类型与枚举：

- 类型：`AbstractUploadListItem`、`AbstractUploadPictureContent`、`TaskRemoveRequestEventArgs`、`Upload`、`UploadDefaultDropArea`、`UploadFilesDroppedEventArgs`、`UploadImagePreviewer`、`UploadList`、`UploadPictureDefaultContent`、`UploadPictureListItem`、`UploadPicturePendingContent`、`UploadPicturePreviewContent`、`UploadPictureShapeDefaultContent`、`UploadPictureShapeList` 等 33 项。
- 枚举：`UploadListType`、`UploadPredicateResult`。

稳定 template part：

| Template Part | 类型 | 职责 |
| --- | --- | --- |
| `PART_FileName` | `?` | 稳定模板协作入口，重命名前必须同步主题和实现。 |
| `PART_ImagePreviewer` | `?` | 稳定模板协作入口，重命名前必须同步主题和实现。 |
| `PART_ItemsPresenter` | `?` | 展示用户内容、文本、图标或模板化数据。 |
| `PART_Trigger` | `?` | 稳定模板协作入口，重命名前必须同步主题和实现。 |
| `PART_TriggerContent` | `?` | 稳定模板协作入口，重命名前必须同步主题和实现。 |
| `PART_UploadList` | `?` | 承载集合项、布局面板或虚拟化内容。 |

当前未抽取到控件专属伪类；主题主要依赖 Avalonia 标准伪类、模板绑定和内部 StyledProperty。

## 事件与命令

Upload 的公共契约由 public/protected 类型成员、Avalonia 属性、事件、命令、template part、伪类、ControlTheme key 和资源 key 共同组成。维护时应先确认这些契约是否已经被源码、Gallery 示例或文档暴露。
稳定事件包括 `FileSelectRequest`、`FilesDropped`、`TaskRemoveRequest`。事件触发顺序属于兼容契约，不能因内部状态重排而改变。
- 类型：`AbstractUploadListItem`、`AbstractUploadPictureContent`、`TaskRemoveRequestEventArgs`、`Upload`、`UploadDefaultDropArea`、`UploadFilesDroppedEventArgs`、`UploadImagePreviewer`、`UploadList`、`UploadPictureDefaultContent`、`UploadPictureListItem`、`UploadPicturePendingContent`、`UploadPicturePreviewContent`、`UploadPictureShapeDefaultContent`、`UploadPictureShapeList` 等 33 项。

## 使用示例

稳定示例来源于 Gallery ShowCase 和源码查看片段。生成器只输出可从 `ShowCaseItem` 追溯的示例，不维护第二套手写示例。

以下示例来自 Gallery 源码查看使用的 `ShowCaseItem` 片段，并已按中文资源规范化。

### 拖拽上传

来源：`controlgallery/AtomUIGallery/ShowCases/DataEntry/Upload/Views/UploadShowCase.axaml:235`

Gallery key：`ExamplesContent` / item `5`

```axaml
<atom:Upload Name="DragAndDropUpload" UploadTransport="{Binding UploadTransport}" UploadTaskFailed="HandleUploadFailed" UploadTaskCompleted="HandleUploadCompleted">
    <atom:UploadDefaultDropArea/>
</atom:Upload>
```

## 状态模型

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

## 主题与 Design Token

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

Token 来源：

Upload Token 只表达组件级视觉变量，例如尺寸、间距、颜色、圆角、阴影、图标尺寸和弹层边界。Token 不承载运行时选择、展开、加载、错误、上传任务、过滤条件或业务状态。

当前 Token scope：

- `UploadToken`，scope id 为 `Upload`，源码位于 `src/AtomUI.Desktop.Controls/Upload/UploadToken.cs`。

## AOT 与裁剪注意事项

资源和 AOT 约束：

- 不通过运行时反射扫描 public API、Token 或 Gallery 表格数据。
- 不把可静态声明的模板结构迁移到 C# 动态创建。
- 异步加载、上传、弹层和窗口生命周期必须能取消或释放。
- 缓存对象必须与控件、窗口、弹层或数据 owner 生命周期一致。
- Source generator 生成文件不手工编辑；需要修改时改输入源或 generator。

性能边界：

- 控件应优先复用 Avalonia 原生虚拟化、模板绑定和资源系统。
- 避免为每次状态变化创建不必要的视觉对象、订阅或动画对象。
- 大集合控件必须保证 container recycle 后不会泄漏旧 item 状态。

## 源码索引

主要源码文件：

- `src/AtomUI.Desktop.Controls/Upload`：13 个文件，代表文件 `AbstractUploadListItem.cs`、`AbstractUploadPictureContent.cs`、`Upload.cs`、`UploadDefaultDropArea.cs`、`UploadEventArgs.cs` 等。
- `src/AtomUI.Desktop.Controls/Upload/DefaultList`：2 个文件，代表文件 `UploadTextListItem.cs`、`UploadTextListItemHeader.cs`。
- `src/AtomUI.Desktop.Controls/Upload/Localization`：3 个文件，代表文件 `en_US.cs`、`zh_CN.cs`、`zh_TW.cs`。
- `src/AtomUI.Desktop.Controls/Upload/PictureList`：5 个文件，代表文件 `UploadPictureDefaultContent.cs`、`UploadPictureListItem.cs`、`UploadPicturePendingContent.cs`、`UploadPicturePreviewContent.cs`、`UploadPictureUploadingContent.cs`。
- `src/AtomUI.Desktop.Controls/Upload/PictureShapeList`：6 个文件，代表文件 `UploadPictureShapeDefaultContent.cs`、`UploadPictureShapeList.cs`、`UploadPictureShapeListItem.cs`、`UploadPictureShapePendingContent.cs`、`UploadPictureShapePreviewContent.cs` 等。
- `src/AtomUI.Desktop.Controls/Upload/Themes`：20 个文件，代表文件 `AbstractUploadPictureContentTheme.axaml`、`AbstractUploadPictureContentTheme.cs`、`UploadTextListItemHeaderTheme.axaml`、`UploadTextListItemTheme.axaml`、`UploadPictureDefaultContentTheme.axaml` 等。

职责边界：

- 控件主文件保留 public/protected API、Avalonia 属性注册、事件和主要生命周期入口。
- Theme 文件负责静态视觉结构、template part、selector 和资源绑定。
- Token 文件只提供组件视觉变量，不保存实例状态。
- Gallery 文件只展示用法、API 表和 Token 表，不作为运行时逻辑 owner。

## 相关文档

- 源设计文档：`docs/controls/desktop/data-entry/upload/overview.md`
- 实现文档：`docs/controls/desktop/data-entry/upload/implementation.md`
- Token 文档：`docs/controls/desktop/data-entry/upload/token.md`
- 变更记录：`docs/controls/desktop/data-entry/upload/changelog.md`
- 语义结构：`./semantic-cn.md`
