# Upload 上传

## 概述

上传组件（Upload）用于将文件通过点击或拖拽方式上传到服务端。它支持多种展示形式（文本列表、图片列表、图片卡片墙、圆形图片墙）、文件类型过滤、最大数量限制、目录上传、拖拽上传、上传进度跟踪等功能。Upload 是数据录入类控件中复杂度较高的一个，涉及文件选择、上传调度、进度反馈、错误处理等完整的生命周期管理。

AtomUI 的 `Upload` 控件对标 [Ant Design 5.0 Upload](https://ant.design/components/upload-cn) 的设计规范，在 .NET / Avalonia 平台上提供与 Ant Design React 版一致的视觉体验和交互行为。

---

## 设计原理

### Ant Design 的上传设计哲学

Ant Design 将上传控件定位为：**「将文件通过网页上传到远端服务器」**。其核心设计理念：

- **触发区可自定义**：上传按钮（触发区）支持任意内容——可以是一个按钮、一个拖拽区域、甚至一组图片卡片中的「+」按钮。
- **文件列表与触发区分离**：上传列表和触发区是两个独立区域，列表支持多种展示风格。
- **完整的生命周期**：从文件选择、上传前校验（`beforeUpload`）、上传进度、上传成功/失败，到文件移除，每个环节都有对应的回调。

**四种列表展示类型**（对应 `listType`）：

| 类型 | 设计意图 | 典型用途 |
|---|---|---|
| 📄 **Text**（文本列表） | 最简洁的列表形式，仅显示文件名和操作按钮 | 常规文件上传（文档、压缩包等） |
| 🖼 **Picture**（图片列表） | 在文本列表基础上增加缩略图预览 | 图片上传，同时需要查看文件名 |
| 🃏 **PictureCard**（图片卡片墙） | 以卡片网格形式展示已上传图片，触发按钮也是一个卡片 | 图片墙、头像上传、产品图管理 |
| ⭕ **PictureCircle**（圆形图片墙） | 与 PictureCard 类似但使用圆形展示 | 头像上传等需要圆形展示的场景 |

### Avalonia ContentControl 基础能力

AtomUI 的 `Upload` 继承自 Avalonia 框架的 `Avalonia.Controls.ContentControl`。`ContentControl` 提供了以下基础能力：

**继承链：**

```
Control → TemplatedControl → ContentControl → Upload
```

作为 `ContentControl`，Upload 能容纳任意内容作为「上传触发区」——用户可以放置 `Button`、`UploadDefaultDropArea`（拖拽区域）或任何自定义控件。当用户点击触发区时，会弹出文件选择对话框。

**ContentControl 提供的基础属性：**

| 属性 | 说明 |
|---|---|
| `Content` | 上传触发区内容，可以是按钮、拖拽区域或任意控件 |
| `ContentTemplate` | 内容模板，用于数据绑定场景下的内容呈现 |
| `HorizontalContentAlignment` | 触发区内容水平对齐方式 |
| `VerticalContentAlignment` | 触发区内容垂直对齐方式 |

### AtomUI 的扩展设计

AtomUI `Upload` 在 Avalonia ContentControl 基础上做了以下扩展：

| 扩展能力 | 实现方式 | 设计动机 |
|---|---|---|
| **四种列表类型** | `UploadListType` 枚举驱动不同模板 | 对齐 Ant Design 的 `listType`，支持 Text / Picture / PictureCard / PictureCircle |
| **上传调度系统** | `FileUploadScheduler` + `IFileUploadTransport` 接口 | 解耦上传逻辑，支持并发控制和自定义传输实现 |
| **拖拽上传** | `UploadDefaultDropArea` 组件 + `DragDrop` 支持 | 开箱即用的拖拽上传体验 |
| **文件数量限制** | `MaxCount` 属性 | 限制上传列表中的最大文件数 |
| **文件类型过滤** | `Accepts` 属性 + `UploadTaskAboutToScheduling` 事件 | 在文件选择对话框和上传前双重过滤 |
| **上传前拦截** | `UploadTaskAboutToScheduling` 事件 + `UploadPredicateResult` | 对标 Ant Design `beforeUpload`，支持取消、显示在列表中但不上传等行为 |
| **目录上传** | `IsUploadDirectoryEnabled` 属性 | 支持选择整个目录上传 |
| **默认文件列表** | `DefaultTaskList` 属性 | 支持页面初始化时展示已有文件 |
| **图片判断** | `IsImageFilePredicate` 回调 | 自定义图片文件判断逻辑 |
| **并发控制** | `MaxConcurrentTasks` 属性 | 控制同时上传的最大任务数 |
| **表单集成** | `IFormItemAware` 接口 | 可参与 FormItem 验证流程 |
| **Design Token** | `UploadToken` + `RegisterTokenResourceScope` | 所有视觉值从全局/组件级 Token 派生 |
| **本地化** | `LanguageProvider` 系统 | 支持中英文切换（`en_US` / `zh_CN`） |

---

## 功能详解

### 上传调度架构

Upload 控件的上传功能基于 **调度器 + 传输接口** 的解耦架构：

```
用户选择文件 → Upload 创建 FileUploadTask
                         ↓
              FileUploadScheduler（内部调度器）
                         ↓ 并发控制（MaxConcurrentTasks）
              IFileUploadTransport（用户提供的传输实现）
                         ↓
              上传进度 / 完成 / 失败 → 事件通知
```

- **`IFileUploadTransport`**（定义于 `AtomUI.Controls.Shared`）：上传传输接口，用户需实现此接口提供具体的上传逻辑（如 HTTP 上传、FTP 上传等）。
- **`FileUploadScheduler`**（内部实现）：调度器管理上传队列，支持并发数控制（默认最大 3 个并发任务）。
- **`UploadTransport`** 属性：设置 `IFileUploadTransport` 实现。未设置传输实现时，文件选择后不会启动上传。

### 列表类型（UploadListType）

Upload 根据 `ListType` 属性选择不同的模板和列表项容器：

| 列表类型 | 模板结构 | 列表项容器 | 说明 |
|---|---|---|---|
| `Text` | DockPanel（触发区 + 列表） | `UploadTextListItem` | 文件名 + 进度条 + 操作按钮 |
| `Picture` | DockPanel（触发区 + 列表） | `UploadPictureListItem` | 缩略图 + 文件名 + 进度条 + 操作按钮 |
| `PictureCard` | `UploadPictureShapeList`（网格） | `UploadPictureShapeListItem` | 卡片形式，缩略图 + 遮罩层操作 |
| `PictureCircle` | `UploadPictureShapeList`（网格） | `UploadPictureShapeListItem` | 圆形展示，与 PictureCard 类似 |

**模板差异：**
- **Text / Picture**：使用 `DockPanel` 布局，触发区（`UploadTriggerContent`）在上方，文件列表（`UploadList`）在下方。
- **PictureCard / PictureCircle**：使用 `UploadPictureShapeList`，触发按钮作为列表中的最后一个项（特殊的 `IsPictureTriggerTask`），与已上传文件卡片并排排列形成网格。

### MaxCount 行为

`MaxCount` 属性控制文件列表的最大数量：

- **`MaxCount > 1`**：当文件数已达上限，新选择的文件将被静默忽略。
- **`MaxCount == 1`**：当已有一个文件时，新选择的文件会替换现有文件（先取消所有现有任务）。
- **`MaxCount == int.MaxValue`**（默认）：无数量限制。

### 上传前拦截（UploadTaskAboutToScheduling）

`UploadTaskAboutToScheduling` 事件在文件创建上传任务后、实际调度上传前触发，通过 `UploadPredicateResult` 控制后续行为：

| 结果值 | 行为 |
|---|---|
| `Schedule` | 正常调度上传（默认） |
| `Cancel` | 取消上传，文件**不显示**在列表中 |
| `CancelWithInTaskList` | 取消上传，但文件**显示**在列表中（标记为失败） |

这对标了 Ant Design 的 `beforeUpload` 回调和 `Upload.LIST_IGNORE` 常量。

### 图片文件判断

Upload 内置了基于扩展名的图片文件判断（支持 `.webp`、`.svg`、`.png`、`.gif`、`.jpg`、`.jpeg`、`.jfif`、`.bmp`、`.dpg`、`.ico`、`.heic`、`.heif`），通过 `IsImageFilePredicate` 回调可自定义判断逻辑。

### 本地化支持

Upload 控件支持以下本地化字符串：

| 资源键 | en_US | zh_CN |
|---|---|---|
| `Uploading` | Uploading... | 上传中... |
| `Pending` | Pending... | 等待调度... |
| `DragUploadHead` | Click or drag file to this area to upload | 点击或拖动文件到此区域进行上传 |

---

## 与 Ant Design 规范的对齐程度

| 特性 | Ant Design (React) | AtomUI (Avalonia) | 对齐情况 |
|---|---|---|---|
| 四种列表类型 `listType` | ✅ `text / picture / picture-card / picture-circle` | ✅ `UploadListType` 枚举 | ✅ 完全对齐 |
| 拖拽上传 | ✅ `<Upload.Dragger>` | ✅ `UploadDefaultDropArea` 组件 | ✅ 完全对齐 |
| 上传前校验 `beforeUpload` | ✅ 回调函数 | ✅ `UploadTaskAboutToScheduling` 事件 | ✅ 对齐（实现方式不同） |
| `LIST_IGNORE` | ✅ 从列表中忽略 | ✅ `UploadPredicateResult.Cancel` | ✅ 对齐 |
| 文件数量限制 `maxCount` | ✅ 数值属性 | ✅ `MaxCount` 属性 | ✅ 完全对齐 |
| 多选 `multiple` | ✅ 布尔属性 | ✅ `IsMultipleEnabled` 属性 | ✅ 完全对齐 |
| 目录上传 `directory` | ✅ 布尔属性 | ✅ `IsUploadDirectoryEnabled` 属性 | ✅ 完全对齐 |
| 文件类型过滤 `accept` | ✅ MIME 类型 | ✅ `Accepts` 属性 | ⚠️ 支持模式匹配（`*.png`），非 MIME 类型 |
| 默认文件列表 `defaultFileList` | ✅ 数组 | ✅ `DefaultTaskList` 属性 | ✅ 完全对齐 |
| 自定义上传 `customRequest` | ✅ 回调函数 | ✅ `IFileUploadTransport` 接口 | ✅ 对齐（接口化实现，更易扩展） |
| `action` URL | ✅ 字符串 | ❌ 无直接对应 | ⚠️ AtomUI 使用 `IFileUploadTransport` 替代 |
| `headers` / `data` / `method` | ✅ 请求配置 | ❌ 由 Transport 实现管理 | ⚠️ 传输细节封装在 `IFileUploadTransport` 中 |
| 进度事件 `onChange` | ✅ 统一回调 | ✅ 细分事件（Created/Progress/Completed/Failed/Cancelled/Removed） | ✅ 更精细的事件模型 |
| 文件预览 `onPreview` | ✅ 回调 | ✅ 内置 `UploadImagePreviewer` | ✅ 内置图片预览能力 |
| 显示/隐藏上传列表 `showUploadList` | ✅ 布尔/对象 | ✅ `IsShowUploadList` 属性 | ✅ 完全对齐 |
| `openFileDialogOnClick` | ✅ 布尔 | ✅ `IsOpenFileDialogOnClick` 属性 | ✅ 完全对齐 |
| 并发控制 | ❌ 无内置 | ✅ `MaxConcurrentTasks` 属性 | ⭐ AtomUI 增强特性 |
| `ExtraContext` | ❌ 无 | ✅ 传递到 Transport 的附加上下文 | ⭐ AtomUI 增强特性 |

---

## 继承关系

```
Avalonia.Controls.Primitives.TemplatedControl
  └── Avalonia.Controls.ContentControl
        └── AtomUI.Desktop.Controls.Upload
              ├── implements IMotionAwareControl
              └── implements IFormItemAware
```

**各层级职责划分：**

| 层级 | 提供的能力 |
|---|---|
| `ContentControl` | 任意内容容纳（作为上传触发区）、`Content` / `ContentTemplate`、`ContentPresenter` 模板化呈现 |
| `Upload` | 文件上传全生命周期管理（选择/调度/进度/完成/失败/移除）、四种列表类型、拖拽上传、文件过滤、数量限制、Design Token 集成、本地化 |

**实现的共享接口：**

| 接口 | 定义位置 | 作用 |
|---|---|---|
| `IMotionAwareControl` | `AtomUI.Controls` | 支持 `IsMotionEnabled` 控制过渡动画 |
| `IFormItemAware` | `AtomUI.Controls.Shared` | 可作为 `FormItem` 的子控件参与表单验证 |

---

## 内部子组件

Upload 控件由多个内部子组件协同工作：

| 子组件 | 可见性 | 说明 |
|---|---|---|
| `UploadTriggerContent` | `internal` | 上传触发区容器，处理点击事件并冒泡 `FileSelectRequestEvent` |
| `UploadDefaultDropArea` | **`public`** | 拖拽上传区域，可作为 `Upload.Content` 使用 |
| `UploadList` | `internal` | 文件列表容器（Text/Picture 类型），继承自 `ItemsControl` |
| `UploadPictureShapeList` | `internal` | 图片卡片/圆形列表容器，继承自 `UploadList` |
| `AbstractUploadListItem` | `internal` | 列表项基类，定义共享属性和删除事件 |
| `UploadTextListItem` | `internal` | Text 类型列表项 |
| `UploadPictureListItem` | `internal` | Picture 类型列表项 |
| `UploadPictureShapeListItem` | `internal` | PictureCard/PictureCircle 类型列表项 |
| `AbstractUploadPictureContent` | `internal` | 图片内容展示基类（处理遮罩层动画） |
| `UploadImagePreviewer` | `internal` | 图片预览器（继承自 `ImagePreviewer`） |

---

## 源码位置

| 层级 | 文件路径 | 说明 |
|---|---|---|
| 控件类 | `src/AtomUI.Desktop.Controls/Upload/Upload.cs` | 主控件类 |
| 拖拽区域 | `src/AtomUI.Desktop.Controls/Upload/UploadDefaultDropArea.cs` | 拖拽上传区域（public） |
| 事件参数 | `src/AtomUI.Desktop.Controls/Upload/UploadEventArgs.cs` | 所有上传事件参数类 |
| 任务信息 | `src/AtomUI.Desktop.Controls/Upload/UploadTaskInfo.cs` | 上传任务信息（AvaloniaObject） |
| 枚举定义 | `src/AtomUI.Desktop.Controls/Upload/UploadListType.cs` | 列表类型枚举 |
| 枚举定义 | `src/AtomUI.Desktop.Controls/Upload/UploadPredicateResult.cs` | 上传拦截结果枚举 |
| Token 定义 | `src/AtomUI.Desktop.Controls/Upload/UploadToken.cs` | 组件级 Design Token |
| 模板常量 | `src/AtomUI.Desktop.Controls/Upload/Themes/UploadThemeConstants.cs` | 模板部件名称常量 |
| 主题模板 | `src/AtomUI.Desktop.Controls/Upload/Themes/UploadTheme.axaml` | ControlTheme AXAML |
| 触发区主题 | `src/AtomUI.Desktop.Controls/Upload/Themes/UploadTriggerContentTheme.axaml` | 触发区 ControlTheme |
| 拖拽区主题 | `src/AtomUI.Desktop.Controls/Upload/Themes/UploadDefaultDropAreaTheme.axaml` | 拖拽区域 ControlTheme |
| 列表主题 | `src/AtomUI.Desktop.Controls/Upload/Themes/UploadListTheme.axaml` | 列表 ControlTheme |
| 本地化 | `src/AtomUI.Desktop.Controls/Upload/Localization/` | 多语言资源（en_US、zh_CN） |
| 共享类型 | `src/AtomUI.Controls.Shared/Net/IFileUploadTransport.cs` | 上传传输接口（设备无关） |
| 共享类型 | `src/AtomUI.Controls.Shared/Net/FileUploadResult.cs` | 上传结果、状态、错误码（设备无关） |
| 共享类型 | `src/AtomUI.Controls.Shared/Net/UploadFileInfo.cs` | 上传文件信息（设备无关） |
| Gallery 示例 | `controlgallery/AtomUIGallery/ShowCases/Views/DataEntry/UploadShowCase.axaml` | 使用范例 |

### 设备无关层（AtomUI.Controls.Shared）

Upload 控件的核心数据类型定义在设备无关层 `AtomUI.Controls.Shared` 中，确保未来 Mobile 平台可以复用：

| 类型 | 文件路径 | 说明 |
|---|---|---|
| `IFileUploadTransport` | `AtomUI.Controls.Shared/Net/IFileUploadTransport.cs` | 上传传输接口 |
| `FileUploadResult` | `AtomUI.Controls.Shared/Net/FileUploadResult.cs` | 上传结果 record |
| `FileUploadStatus` | `AtomUI.Controls.Shared/Net/FileUploadResult.cs` | 上传状态枚举 |
| `FileUploadErrorCode` | `AtomUI.Controls.Shared/Net/FileUploadResult.cs` | 上传错误码枚举 |
| `FileUploadProgress` | `AtomUI.Controls.Shared/Net/IFileUploadTransport.cs` | 上传进度 record |
| `UploadFileInfo` | `AtomUI.Controls.Shared/Net/UploadFileInfo.cs` | 上传文件信息 record |

---

## 模板结构

Upload 根据 `ListType` 使用两套不同的模板：

### Text / Picture 模板

```
DockPanel#RootLayout
├── UploadTriggerContent (PART_TriggerContent)       ← 上传触发区（DockPanel.Dock="Top"）
│   └── DashedBorder#TriggerContentFrame
│       └── ContentPresenter (PART_Trigger)          ← 用户自定义内容（如 Button）
└── UploadList (PART_UploadList)                     ← 文件列表
    └── ItemsPresenter                               ← 列表项容器
        ├── UploadTextListItem / UploadPictureListItem  ← 每个文件一个列表项
        └── ...
```

### PictureCard / PictureCircle 模板

```
UploadPictureShapeList (PART_UploadList)             ← 图片列表（网格布局）
└── WrapPanel                                        ← 网格容器
    ├── UploadPictureShapeListItem                   ← 已上传文件卡片
    ├── UploadPictureShapeListItem                   ← 已上传文件卡片
    ├── ...
    └── UploadTriggerContent                         ← 上传触发卡片（"+" 按钮）
```

### 模板部件常量

| 常量名 | 值 | 说明 |
|---|---|---|
| `UploadThemeConstants.TriggerContentPart` | `"PART_TriggerContent"` | 上传触发区 |
| `UploadThemeConstants.UploadListPart` | `"PART_UploadList"` | 文件列表 |
| `UploadThemeConstants.ImagePreviewerPart` | `"PART_ImagePreviewer"` | 图片预览器 |
| `UploadThemeConstants.FileNamePart` | `"PART_FileName"` | 文件名展示 |
| `UploadTriggerContentThemeConstants.TriggerPart` | `"PART_Trigger"` | 触发区内容呈现器 |
