# ImagePreviewer 桌面版实现原理

本文档描述 ImagePreviewer 桌面版的内部实现范围、源码职责、图片源加载管线、状态流、生命周期、资源边界和维护规则。公共设计与 API 契约见 [ImagePreviewer 桌面版架构设计](overview.md)，变化记录见 [ImagePreviewer Changelog](changelog.md)。涉及组件 Token 的实现应同时阅读 [ImagePreviewer Token 设计](token.md)。

## 1. 实现定位

本文档覆盖 ImagePreviewer 的控件实现、统一图片源加载、主题接入、状态同步和 Gallery 可见维护边界。具体属性注册、默认值、绘制细节和 AXAML selector 仍应直接阅读源码；本文只记录维护者必须理解的稳定结构和不变量。

## 2. 源码文件结构

主要源码文件：

- `src/AtomUI.Desktop.Controls/ImagePreviewer`：控件主体、图片源模型、加载状态、预览宿主和 renderer 所在目录，代表文件包括 `AbstractImagePreviewer.cs`、`ImagePreviewer.cs`、`ImageViewer.cs`、`ImagePreviewRenderer.cs`、`ImageSourceUri.cs`、`ImagePreviewItem.cs`、`LoadedImageSource.cs` 和 `IImageSourceLoader.cs`。
- `src/AtomUI.Desktop.Controls/ImagePreviewer/Localization`：3 个文件，代表文件 `en_US.cs`、`zh_CN.cs`、`zh_TW.cs`。
- `src/AtomUI.Desktop.Controls/ImagePreviewer/Themes`：12 个文件，代表文件 `ImageGroupPreviewerTheme.axaml`、`ImagePreviewFloatToolbarTheme.axaml`、`ImagePreviewNavButtonTheme.axaml`、`ImagePreviewToolbarTheme.axaml`、`ImagePreviewerCoverTheme.axaml` 等。

职责边界：

- 控件主文件保留 public/protected API、Avalonia 属性注册、事件和主要生命周期入口。
- 图片源模型文件只表达 `ImageSourceUri`、`ImagePreviewItem`、`LoadedImageSource` 和加载服务契约，不承载视觉模板逻辑。
- Theme 文件负责静态视觉结构、template part、selector 和资源绑定。
- Token 文件只提供组件视觉变量，不保存实例状态。
- Gallery 文件只展示用法、API 表和 Token 表，不作为运行时逻辑 owner。

## 3. 核心类职责

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
- `ImageSourceUri`：图片来源 URI 值对象，负责保存和规范化用户输入的图片来源字符串。
- `ImagePreviewItem`：图片项状态 owner，维护来源、加载状态、加载版本、取消令牌和加载结果。
- `LoadedImageSource`：加载完成后的 renderer 输入，封装 Bitmap、SVG 文本、源尺寸和释放语义。
- `IImageSourceLoader`：统一图片加载服务，封装 `avares://`、本地文件、`file://`、相对路径和 `http(s)://` 的异步加载。
- `en_US`：控件核心或内部协作类型，维护 public surface 与主题可观察行为。
- `zh_CN`：控件核心或内部协作类型，维护 public surface 与主题可观察行为。
- `zh_TW`：控件核心或内部协作类型，维护 public surface 与主题可观察行为。

核心协作规则：

- 控件实例是 public API 和运行时状态 owner。
- `ImagePreviewItem` 是单个图片来源的 loading/loaded/failed 状态 owner；renderer 只消费加载结果，不发起加载。
- Template part 是视觉协作对象，生命周期必须受 `OnApplyTemplate` 或模板加载流程管理。
- 数据对象、选项对象、任务对象或节点对象只保存业务数据，不应反向持有不可释放的视觉对象。
- 弹层、窗口、计时器、异步 loader 和全局管理器必须有明确关闭、解绑或释放路径。

## 4. 状态与数据流

ImagePreviewer 的状态流遵循下面路径：

```text
Public API / ImageSourceUri / Command / Event
  -> 控件实例状态
  -> ImagePreviewItem state / internal state / effective state / pseudo-class
  -> template part property / AXAML selector / loading presenter / error presenter
  -> renderer / popup / adorner / Gallery observable behavior
```

源码中的状态入口按以下语义维护：

- 图片来源：`SourceUri`、`SourceUris`、`CoverSourceUri`、`FallbackSourceUri`。
- 内容与数据：`CoverIndicatorContent`、`CoverIndicatorContentTemplate`、`LoadingContent`、`LoadingContentTemplate`、`ErrorContent`、`ErrorContentTemplate`、`ImageMaxScale`、`ImageMinScale`、`ImageScaleStep`、`ImageTranslateX`、`ImageTranslateY`。
- 选择与集合：`Count`、`CurrentIndex`。
- 交互与状态：`IsDialogModal`、`IsDialogTopmost`、`IsModal`、`IsMotionEnabled`、`IsOpen`、`IsShowCoverMask`。
- 视觉与布局：`CoverHeight`、`CoverWidth`。
- 其他稳定入口：`MaxScale`、`MinScale`、`ScaleStep`、`Stretch`、`Transform`。

维护要求：

- 外部设置的 Avalonia 属性必须在模板应用前后保持一致。
- 集合、当前项、图片来源替换、fallback 和异步 loader 必须能处理 reset、replace 和 clear。
- 伪类和 internal state 必须从单一 owner 推导，避免双向同步导致循环更新。
- Gallery API 表中的状态说明应与源码实际状态流一致。

图片加载状态流：

```text
ImageSourceUri
  -> ImagePreviewItem(Pending)
  -> IImageSourceLoader.LoadAsync(...)
  -> ImagePreviewItem(Loading)
  -> LoadedImageSource / Failed
  -> ImagePreviewRenderer
  -> ImageViewer measure / arrange / fit-to-window
```

`ImagePreviewItemState` 只能由 item owner 写入。加载任务返回时必须校验版本号和取消状态；过期结果必须释放后丢弃，不能覆盖新来源。

## 5. 生命周期与模板接入

生命周期规则：

- 构造阶段只注册必要状态，不依赖 template part。
- 模板应用时获取 part、建立事件订阅和绑定，并先释放旧 part 订阅。
- 控件卸载、弹层关闭、窗口关闭、集合替换或 container recycle 时释放事件订阅和资源宿主。
- DynamicResource、TokenResourceBinder 或 C# binding 必须有明确 owner 和释放点。
- 图片加载任务由图片项或控件 owner 创建取消令牌。`SourceUri` 变更、`SourceUris` 替换、弹层关闭、控件 detach 和 owner dispose 时必须取消仍在运行的加载。
- 加载完成后需要通知 `ImagePreviewRenderer` 和 `ImageViewer` 重新 measure/arrange，保证居中、fit-to-window、缩放按钮状态和拖拽边界基于真实图片尺寸计算。
- Browser 和 Desktop 宿主下的主题加载顺序不得影响 public API 语义。

稳定 template part 接入点：

- `PART_CloseButton`：承载用户触发入口、导航或关闭动作。
- `PART_CoverItemsControl`：承载集合项、布局面板或虚拟化内容。
- `PART_FitToWindowButton`：承载用户触发入口、导航或关闭动作。
- `PART_HorizontalFlipButton`：承载用户触发入口、导航或关闭动作。
- `PART_ImageRenderer`：稳定模板协作入口，重命名前必须同步主题和实现。
- `PART_ImageViewerScene`：稳定模板协作入口，重命名前必须同步主题和实现。
- `PART_LoadingPresenter`：承载图片加载状态内容。默认封面使用 Skeleton 风格占位，预览层使用居中 Spin。
- `PART_ErrorPresenter`：承载图片加载失败内容；存在 `FallbackSourceUri` 时优先展示 fallback 结果。
- `PART_Logo`：稳定模板协作入口，重命名前必须同步主题和实现。
- `PART_NextButton`：承载用户触发入口、导航或关闭动作。
- `PART_PreviousButton`：承载用户触发入口、导航或关闭动作。
- `PART_RotateLeftButton`：承载用户触发入口、导航或关闭动作。
- `PART_RotateRightButton`：承载用户触发入口、导航或关闭动作。
- `PART_ScaleDownButton`：承载用户触发入口、导航或关闭动作。
- `PART_ScaleUpButton`：承载用户触发入口、导航或关闭动作。
- `PART_VerticalFlipButton`：承载用户触发入口、导航或关闭动作。

## 6. 交互与事件处理

ImagePreviewer 的交互事件应从输入源收敛到控件级语义事件：

- Pointer、keyboard、focus 和 command 事件不应绕过 Avalonia 基础控件语义。
- 弹层、窗口或 overlay 类路径必须稳定处理打开、关闭、取消、重复打开和宿主失活。
- 集合类路径必须稳定处理 source prepare、clear、当前项切换和图片项回收。
- 输入类路径必须保持 Form、validation、clear、placeholder 和键盘行为一致。
- loading 状态下，上一张/下一张和关闭动作保持可用；缩放、旋转、拖拽和 fit-to-window 等依赖真实图片尺寸的动作在当前图片未 loaded 前禁用。

稳定事件路径包括 `FitToWindowRequest`、`HorizontalFlipRequest`、`NextRequest`、`PreviousRequest`、`RotateLeftRequest`、`RotateRightRequest`、`ScaleDownRequest`、`ScaleUpRequest`、`VerticalFlipRequest`。事件参数和触发时机属于兼容边界。

## 7. 内部算法与关键流程

维护者需要重点关注以下流程：

- API 默认值到 effective state 的归一。
- Template part 重新应用时的状态回放。
- `ImageSourceUri` 规范化、来源身份 key 生成和来源类型判定。
- `IImageSourceLoader` 对 `avares://`、本地路径、`file://`、相对路径和 `http(s)://` 的统一异步加载。
- `ImagePreviewItemState` 的 pending、loading、loaded、failed 迁移，以及 fallback 加载路径。
- 快速切换图片来源时的取消、版本校验和过期结果释放。
- 主题资源、Token 和 SharedToken 计算后的视觉更新。
- `SourceUris`、current item 和弹层宿主之间的集合同步。
- 动效启停、初始加载阶段 transition 抑制和卸载取消。

实现文档不逐行解释私有方法。若某个私有算法成为稳定维护入口，应在本节补充算法不变量，而不是把代码复述为说明书。

## 8. 资源、性能与 AOT 边界

资源和 AOT 约束：

- 不通过运行时反射扫描 public API、Token 或 Gallery 表格数据。
- 不把可静态声明的模板结构迁移到 C# 动态创建。
- 异步加载、上传、弹层和窗口生命周期必须能取消或释放。
- 网络图片必须通过异步加载服务处理，不允许在 UI 线程同步等待网络 I/O。
- 本地、资源和远程图片共享同一来源身份 key 规范化规则，用于去重、旧结果判定和后续扩展。
- `LoadedImageSource` 由控件当前加载项持有；来源替换、取消或控件释放时必须释放旧结果。
- Source generator 生成文件不手工编辑；需要修改时改输入源或 generator。

性能边界：

- 控件应优先复用 Avalonia 原生虚拟化、模板绑定和资源系统。
- 避免为每次状态变化创建不必要的视觉对象、订阅或动画对象。
- 大集合控件必须保证 container recycle 后不会泄漏旧 item 状态。
- loading 视觉可延迟显示以避免本地文件和 `avares://` 快速完成时闪烁；延迟只影响视觉，不改变状态机和取消语义。

## 9. 维护不变量

维护 ImagePreviewer 时不得破坏：

- Public API、默认值、事件顺序和 Gallery 可观察行为。
- Template part 名称、ControlTheme key、伪类和资源 key。
- 旧 template part、事件订阅、Popup/Flyout/Window host 和 collection view 的释放路径。
- 图片加载状态的单 owner：`ImageSourceUri` 是来源，`ImagePreviewItem` 是状态，`LoadedImageSource` 是结果，`ImagePreviewRenderer` 只负责渲染。
- `SourceUri`、`SourceUris`、`CoverSourceUri` 和 `FallbackSourceUri` 的加载、取消、来源身份和失败处理一致性。
- 加载完成后必须重新计算 `ImageViewer` 的布局、居中、fit-to-window 和交互边界。
- Light/Dark、Browser/Desktop 和不同 SizeType 下的主题一致性。
- 文档、Gallery API 表、Token 表与源码契约的一致性。

## 10. 测试与验证

推荐验证：

- 纯文档改动运行 `git diff --check` 并检查相对链接。
- 控件 API 或行为变更运行对应 `tests/AtomUI.Desktop.Controls.Tests` 或专用包测试。
- 图片加载模型变更覆盖 `ImageSourceUri` 解析、本地文件、`avares://`、fake HTTP、取消、防旧结果回写、fallback 和 loading/error 模板状态。
- DataGrid 相关变更运行 `tests/AtomUI.Desktop.Controls.DataGrid.Tests`。
- Gallery 示例、API 表或 Token 表变更运行 `tests/AtomUIGallery.Tests`。
- AOT、生成器或动态数据路径变更按 Gallery NativeAOT 发布流程验证。
