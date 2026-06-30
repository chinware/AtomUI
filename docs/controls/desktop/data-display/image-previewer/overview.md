# ImagePreviewer 桌面版架构设计

本文档定义 `ImagePreviewer` 桌面版的最新设计定位、公共契约、图片源加载模型、状态模型、视觉主题关系和兼容边界。通用控件研发约束见 [控件研发标准](../../../../engineering/control-development-guidelines.md)，内部实现原理见 [ImagePreviewer 桌面版实现原理](implementation.md)，ImagePreviewer Token 的专项设计见 [ImagePreviewer Token 设计](token.md)，设计和契约变化记录见 [ImagePreviewer Changelog](changelog.md)。

## 1. 控件定位

| 项 | 值 |
| --- | --- |
| NuGet 包 | `AtomUI.Desktop.Controls` |
| .NET 命名空间 | `AtomUI.Desktop.Controls` |
| AXAML 命名空间 | `https://atomui.net` |
| Gallery 页面 | `controlgallery/AtomUIGallery/ShowCases/DataDisplay/ImagePreviewer` |
| 控件状态 | Stable |

ImagePreviewer 是 AtomUI 桌面控件体系中的图片预览控件，用于以统一图片来源 URI 查看、缩放、旋转、切换和窗口化预览图片。

ImagePreviewer 不负责图片编辑器、文件上传控件或媒体资源管理系统。这些职责应由业务层、组合控件或更专用的 AtomUI 控件承担。

主要源码入口：

- `src/AtomUI.Desktop.Controls/ImagePreviewer`

## 2. 设计语言

ImagePreviewer 的设计语言围绕控件职责、可观察状态和主题契约组织，而不是围绕模板节点组织。

| 维度 | 含义 | ImagePreviewer 中的表达 |
| --- | --- | --- |
| 产品语义 | 控件在界面中承担的稳定职责。 | ImagePreviewer 是 AtomUI 桌面控件体系中的图片预览控件，用于查看、缩放、旋转、切换和窗口化预览图片。 |
| 内容承载 | 用户数据、展示内容、集合项或操作入口如何进入控件。 | `SourceUri`、`SourceUris`、`CoverSourceUri`、`FallbackSourceUri`、`CoverIndicatorContent`、`CoverIndicatorContentTemplate`、`ImageMaxScale`、`ImageMinScale`、`ImageScaleStep`。 |
| 状态反馈 | public API、内部状态和伪类如何形成用户可感知反馈。 | current item、open/close、image loading、loaded/failed、fallback、motion。 |
| 主题语义 | ControlTheme、SharedToken、组件 Token 和模板绑定如何表达视觉。 | ImagePreviewer Token + ControlTheme。 |

## 3. API 与契约模型

ImagePreviewer 的公共契约由 public/protected 类型成员、Avalonia 属性、事件、命令、template part、伪类、ControlTheme key 和资源 key 共同组成。维护时应先确认这些契约是否已经被源码、Gallery 示例或文档暴露。

核心 public surface 按语义分组维护：

| 契约组 | 代表成员 | 维护含义 |
| --- | --- | --- |
| 图片来源 | `SourceUri`、`SourceUris`、`CoverSourceUri`、`FallbackSourceUri` | 统一表达单图、多图、封面和失败兜底图片来源，来源可以是 `avares://`、本地路径、`file://` 或 `http(s)://`。 |
| 内容与数据 | `CoverIndicatorContent`、`CoverIndicatorContentTemplate`、`LoadingContent`、`LoadingContentTemplate`、`ErrorContent`、`ErrorContentTemplate`、`ImageMaxScale`、`ImageMinScale`、`ImageScaleStep`、`ImageTranslateX`、`ImageTranslateY` | 定义控件展示内容、输入数据、模板或业务对象入口。 |
| 选择与集合 | `Count`、`CurrentIndex` | 维护当前预览项、多图切换和集合状态。 |
| 交互与状态 | `IsDialogModal`、`IsDialogTopmost`、`IsModal`、`IsMotionEnabled`、`IsOpen`、`IsShowCoverMask` | 表达用户可观察状态、可用性、清除、加载或反馈语义。 |
| 视觉与布局 | `CoverHeight`、`CoverWidth` | 影响尺寸、位置、颜色、形状、密度和模板视觉变量。 |
| 其他稳定入口 | `MaxScale`、`MinScale`、`ScaleStep`、`Stretch`、`Transform` | 保留为 public surface，变更前需确认 Gallery 和用户 XAML 依赖。 |

稳定事件包括 `FitToWindowRequest`、`HorizontalFlipRequest`、`NextRequest`、`PreviousRequest`、`RotateLeftRequest`、`RotateRightRequest`、`ScaleDownRequest`、`ScaleUpRequest`、`VerticalFlipRequest`。事件触发顺序属于兼容契约，不能因内部状态重排而改变。

主要公开类型与枚举：

- 类型：`AbstractImagePreviewer`、`ImageSourceUri`、`ImageFitToWindowEventArgs`、`ImageGroupPreviewer`、`ImagePreviewBaseToolbar`、`ImagePreviewFloatToolbar`、`ImagePreviewNavButton`、`ImagePreviewRenderer`、`ImagePreviewToolbar`、`ImagePreviewToolbarRequestEventArgs`、`ImagePreviewer`、`ImagePreviewerCover`、`ImagePreviewerDialog`、`ImagePreviewerOverlayHost`、`ImagePreviewerTitleBar` 等。
- 枚举：`ImagePreviewItemState`。

稳定 template part：

| Template Part | 类型 | 职责 |
| --- | --- | --- |
| `PART_CloseButton` | `?` | 承载用户触发入口、导航或关闭动作。 |
| `PART_CoverItemsControl` | `?` | 承载集合项、布局面板或虚拟化内容。 |
| `PART_FitToWindowButton` | `?` | 承载用户触发入口、导航或关闭动作。 |
| `PART_HorizontalFlipButton` | `?` | 承载用户触发入口、导航或关闭动作。 |
| `PART_ImageRenderer` | `?` | 稳定模板协作入口，重命名前必须同步主题和实现。 |
| `PART_ImageViewerScene` | `?` | 稳定模板协作入口，重命名前必须同步主题和实现。 |
| `PART_LoadingPresenter` | `ContentPresenter` | 承载图片加载状态内容。默认封面使用 Skeleton 风格占位，预览层使用居中 Spin。 |
| `PART_ErrorPresenter` | `ContentPresenter` | 承载图片加载失败内容；存在 `FallbackSourceUri` 时优先展示 fallback 结果。 |
| `PART_Logo` | `?` | 稳定模板协作入口，重命名前必须同步主题和实现。 |
| `PART_NextButton` | `?` | 承载用户触发入口、导航或关闭动作。 |
| `PART_PreviousButton` | `?` | 承载用户触发入口、导航或关闭动作。 |
| `PART_RotateLeftButton` | `?` | 承载用户触发入口、导航或关闭动作。 |
| `PART_RotateRightButton` | `?` | 承载用户触发入口、导航或关闭动作。 |
| `PART_ScaleDownButton` | `?` | 承载用户触发入口、导航或关闭动作。 |
| `PART_ScaleUpButton` | `?` | 承载用户触发入口、导航或关闭动作。 |
| `PART_VerticalFlipButton` | `?` | 承载用户触发入口、导航或关闭动作。 |

当前未抽取到控件专属伪类；主题主要依赖 Avalonia 标准伪类、模板绑定和内部 StyledProperty。

## 4. 行为与状态模型

ImagePreviewer 的状态流按以下路径收敛：

```text
Public API / ImageSourceUri / inherited command / user input
  -> 控件实例状态
  -> ImagePreviewItem state / effective state / pseudo-class / template property
  -> ControlTheme selector / loading presenter / error presenter / renderer
  -> Gallery 可观察行为
```

状态维护规则：

- Disabled 或不可交互状态优先屏蔽 pointer、keyboard、motion 和提交类反馈。
- `ImageSourceUri` 是用户输入层，`ImagePreviewItem` 是控件内部图片项状态 owner，`LoadedImageSource` 是加载完成结果。三者不能混用职责。
- 图片项状态按 `Pending -> Loading -> Loaded/Failed` 收敛，加载失败且存在 `FallbackSourceUri` 时转入 fallback 加载，不直接吞掉失败。
- current item、open/close、image loading、loaded/failed、fallback、motion 状态由控件实例或明确的数据 owner 推导，不能在 template part 之间双向竞争。
- 模板重套用时必须把 public API 对应状态回放到新的 part、伪类和主题变量。
- 集合、弹层、异步、动效或窗口相关状态必须能处理 reset、close、cancel、detach 和 owner 释放；过期异步加载结果不能回写新来源。

## 5. 视觉与主题模型

ImagePreviewer 的视觉模型由控件模板、ControlTheme、SharedToken 和必要的组件 Token 共同构成。

| 主题文件 | 职责 |
| --- | --- |
| `ImageGroupPreviewerTheme.axaml` | 提供控件模板、selector、资源绑定和状态视觉。 |
| `ImagePreviewFloatToolbarTheme.axaml` | 提供控件模板、selector、资源绑定和状态视觉。 |
| `ImagePreviewNavButtonTheme.axaml` | 定义局部操作入口、按钮或 handle 的状态视觉。 |
| `ImagePreviewToolbarTheme.axaml` | 提供控件模板、selector、资源绑定和状态视觉。 |
| `ImagePreviewerCoverTheme.axaml` | 提供控件模板、selector、资源绑定和状态视觉。 |
| `ImagePreviewerDialogTheme.axaml` | 定义弹层、窗口或 overlay 宿主视觉。 |
| `ImagePreviewerTheme.axaml` | 提供控件模板、selector、资源绑定和状态视觉。 |
| `ImagePreviewerThemes.axaml` | 聚合控件家族主题资源，保证包级引入顺序稳定。 |
| `ImagePreviewerTitleBarTheme.axaml` | 提供控件模板、selector、资源绑定和状态视觉。 |
| `ImageViewerTheme.axaml` | 提供控件模板、selector、资源绑定和状态视觉。 |

ImagePreviewer 使用 `ImagePreviewerToken` 作为组件 Token scope。Token 只表达组件视觉语义，不承载 current item、open/close、image loading、loaded/failed、fallback 或 motion 运行时状态。

加载视觉遵循以下规则：

- 封面加载态使用 Skeleton 风格图片占位，保持封面尺寸稳定，不显示 hover mask。
- 预览层加载态使用居中 Spin，缩放、旋转、拖拽和 fit-to-window 在当前图片未加载完成前禁用。
- loading 视觉允许短暂延迟显示以避免本地文件或 `avares://` 资源快速完成造成闪烁；延迟只影响视觉，不影响 `ImagePreviewItemState`。
- `LoadingContent` / `LoadingContentTemplate` 替换默认加载内容，`ErrorContent` / `ErrorContentTemplate` 替换默认失败内容。

主题维护规则：

- 不删除或重命名已经稳定的 ControlTheme key、template part、伪类和资源 key。
- 不把可由 AXAML 表达的模板状态迁移为 C# 动态创建视觉。
- 不把 hover、pressed、current item、loading、failed、fallback、popup open 等运行时状态写入 Token。
- Browser 或平台特化主题必须保持同一 API 的语义一致。

## 6. 控件家族或集成关系

ImagePreviewer 与同分类控件共享尺寸、状态、Token、Gallery 展示和验证规则。组合或派生控件应显式说明哪些 API 被继承、覆盖或不支持。

主要协作类型：

- `AbstractImagePreviewer`：跨平台或共享基类，承载公共 API、状态归一和模板生命周期。
- `ImageGroupPreviewer`：控件核心或内部协作类型，维护 public surface 与主题可观察行为。
- `ImagePreviewBaseToolbar`：控件核心或内部协作类型，维护 public surface 与主题可观察行为。
- `ImagePreviewFloatToolbar`：控件核心或内部协作类型，维护 public surface 与主题可观察行为。
- `ImagePreviewNavButton`：动作触发类型，负责点击、导航或局部操作状态。
- `ImagePreviewRenderer`：控件核心或内部协作类型，维护 public surface 与主题可观察行为。
- `ImagePreviewToolbar`：控件核心或内部协作类型，维护 public surface 与主题可观察行为。
- `ImagePreviewer`：控件核心或内部协作类型，维护 public surface 与主题可观察行为。
- `ImagePreviewerCover`：控件核心或内部协作类型，维护 public surface 与主题可观察行为。
- `ImagePreviewerDialog`：控件核心或内部协作类型，维护 public surface 与主题可观察行为。
- `ImagePreviewerDialogTheme`：ControlTheme 类型入口，连接主题资源和控件类型。
- `ImagePreviewerOverlayHost`：模板协作类型，承载内容展示、宿主或视觉边界。
- `ImagePreviewerTitleBar`：控件核心或内部协作类型，维护 public surface 与主题可观察行为。
- `ImagePreviewerTitleBarTheme`：ControlTheme 类型入口，连接主题资源和控件类型。
- `ImagePreviewerToken`：组件 Token scope，负责从全局 token 派生控件语义变量。
- `ImageViewer`：控件核心或内部协作类型，维护 public surface 与主题可观察行为。
- `ImageSourceUri`：图片来源 URI 值对象，统一表达 `avares://`、本地路径、`file://` 和 `http(s)://`。
- `ImagePreviewItem`：图片项状态对象，维护 `Pending`、`Loading`、`Loaded`、`Failed` 状态和异步加载版本。
- `LoadedImageSource`：已加载图片结果，承载 Bitmap 或 SVG 文本和源尺寸。
- `IImageSourceLoader`：统一图片加载服务，负责本地、资源和远程图片加载、取消与来源身份处理。
- `en_US`：控件核心或内部协作类型，维护 public surface 与主题可观察行为。
- `zh_CN`：控件核心或内部协作类型，维护 public surface 与主题可观察行为。
- `zh_TW`：控件核心或内部协作类型，维护 public surface 与主题可观察行为。

集成关系：

- 与 ThemeManager、SharedToken、ControlTheme 和 Gallery ShowCase 的示例/API/Token 表保持一致。
- 涉及 ItemsSource、Popup、Flyout、Window、Form 或 CompactSpace 的路径必须保持生命周期释放和数据状态同步。
- 源码目录中的共享基类和内部协作类型形成维护边界，不能只修改桌面包装类而忽略共享状态 owner。

## 7. 兼容性不变量

维护 ImagePreviewer 时必须保持以下不变量：

- 不擅自新增、删除、重命名或改变已批准的 public/protected API、Avalonia 属性、事件和默认值。
- 不破坏 template part、伪类、ControlTheme key、Token 名称和资源 key。
- 不改变 Gallery 已展示的 XAML 用法、默认外观、交互顺序和状态优先级。
- Template part 重新应用、集合替换、弹层关闭、窗口失活和控件 detach 时必须释放旧订阅和资源宿主。
- 图片加载不得在 UI 线程执行网络 I/O，不得通过同步 `Stream` API 承载远程来源。
- `SourceUri` / `SourceUris` / `CoverSourceUri` / `FallbackSourceUri` 的解析、来源身份 key 和取消语义必须一致。
- 不通过隐藏延迟、强制刷新或吞异常掩盖状态同步问题。
- 不引入运行时反射扫描作为 API、Token 或数据路径发现机制。
- 文档只描述当前稳定设计；历史变化记录在 `changelog.md`。

## 8. 专项模型

### 8.1 选择与当前项模型

ImagePreviewer 的当前项状态必须由单一 owner 推导。public 选择属性、集合项容器和伪类之间只能做单向同步，集合替换、清空和模板重套用时必须回放当前状态。

### 8.2 弹层与宿主模型

ImagePreviewer 涉及弹层、窗口或 overlay 宿主时，打开状态、取消事件、定位和宿主释放必须保持一致。重复打开、关闭、窗口失活和 template reapply 都必须释放旧宿主引用。

### 8.3 图片来源与加载模型

ImagePreviewer 使用 `ImageSourceUri` 作为图片来源公共契约。`ImageSourceUri` 表示图片来源地址，不表示加载结果；它支持 `avares://`、本地绝对路径、本地相对路径、`file://`、`http://` 和 `https://`。

图片来源通过 `IImageSourceLoader` 统一异步加载。加载结果收敛为 `LoadedImageSource`，并由 `ImagePreviewRenderer` 渲染为 Bitmap 或 SVG。远程图片、本地文件和 Avalonia 资源必须共享同一条取消、来源身份和失败处理路径。

`SourceUri` 表达单图来源，`SourceUris` 表达多图来源，`CoverSourceUri` 覆盖封面来源，`FallbackSourceUri` 表达加载失败后的兜底来源。`CoverSourceUri` 为空时，封面使用当前 effective source 中的首个图片项。

### 8.4 加载、失败与 fallback 模型

每个图片项由 `ImagePreviewItemState` 表达状态：`Pending`、`Loading`、`Loaded`、`Failed`。状态属于图片项，不属于 renderer 局部视觉状态。

加载状态可视化分层处理：

- 封面区域在 loading 时显示 Skeleton 风格占位，并保持封面尺寸稳定。
- 预览层在 loading 时显示居中 Spin，禁用依赖真实图片尺寸的缩放、旋转、拖拽和 fit-to-window。
- 加载失败时先尝试 `FallbackSourceUri`；没有 fallback 或 fallback 失败时显示 `ErrorContent` / `ErrorContentTemplate` 或默认失败占位。
- 本地或资源图片快速加载完成时可以延迟显示 loading 视觉以避免闪烁，但状态机仍必须进入 `Loading` 并接受取消。

### 8.5 集合与数据同步模型

ImagePreviewer 的集合状态必须能处理 source replace、reset、clear 和 container recycle。业务数据对象不应反向持有视觉对象，虚拟化或懒创建路径必须在容器回收时清理旧状态。

### 8.6 动效模型

ImagePreviewer 的动效只表达状态变化反馈，不应改变 public API 语义。初始加载、禁用态和卸载路径应能抑制或取消动效，避免保留旧控件实例。

## 9. 文档导航、LLMS 导出与验证策略

关联文档：

- [ImagePreviewer 桌面版实现原理](implementation.md)
- [ImagePreviewer Token 设计](token.md)
- [ImagePreviewer Changelog](changelog.md)

LLMS 语义区域：

| Part | AtomUI 节点 | 职责 | 相关 API | 相关 Token | 稳定性 |
| --- | --- | --- | --- | --- | --- |
| `root` | `ImagePreviewer` | 数据展示控件根语义区域，承载 public API、数据状态和主题入口。 | 见 API 与契约模型 | 见视觉与主题模型 | stable |
| `item` | `条目或容器区域` | 承载集合项、单元格、标签、时间节点、卡片或展示单元。 | 见 API 与契约模型 | 见视觉与主题模型 | stable |
| `header` | `标题或头部区域` | 承载标题、字段名、列头、操作入口或摘要信息。 | 见 API 与契约模型 | 见视觉与主题模型 | stable |
| `content` | `内容区域` | 承载主体内容、媒体、文本、空状态、加载状态或详情区域。 | 见 API 与契约模型 | 见视觉与主题模型 | stable |
| `motion` | `动效或浮层区域` | 表达展开收起、轮播、tooltip、tour、预览或虚拟化反馈。 | 见 API 与契约模型 | 见视觉与主题模型 | stable |

LLMS 导出来源：

| LLMS 内容 | 来源 | 说明 |
| --- | --- | --- |
| 单控件完整文档 | `overview.md` + Gallery API / Token / ShowCase | 生成 `controls/image-previewer/index-cn.md` |
| 单控件语义文档 | `overview.md` + `implementation.md` + theme/template 信息 | 生成 `controls/image-previewer/semantic-cn.md` |
| API 表 | Gallery ApiDataGrid 或源码 public surface | 不在 `overview.md` 中复制完整 API 表 |
| Design Token 表 | Gallery DesignTokenDataGrid、Token 类型或第 5 节主题模型 | 不在生成产物中手工维护第二份 Token 表 |
| 示例 | Gallery ShowCase + source snippet catalog | 只引用稳定示例 |
| 源码索引 | `implementation.md` | 用于定位控件源码、主题和测试 |

验证策略：

| 改动类型 | 验证要求 |
| --- | --- |
| 文档改动 | 运行 `git diff --check`，检查相对链接存在。 |
| Public API | 覆盖属性默认值、事件触发、命令和继承语义。 |
| 状态模型 | 覆盖 selection/checked/active、open/close、collection/filter、input/value、motion、disabled、hover、pressed、focus 以及控件特有状态。 |
| AXAML/Theme | 检查 template part、伪类、资源 key、Light/Dark 主题和 Browser 主题。 |
| Token | 检查 TokenKind、AXAML token resource、Gallery Token 表和文档同步。 |
| Gallery | 走查对应 ShowCase 示例、API 表和 Token 表入口。 |
