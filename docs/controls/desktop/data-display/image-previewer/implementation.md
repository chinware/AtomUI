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
- `IImagePreviewTitleResolver`：预览标题解析接口，基于当前图片来源和集合上下文同步返回可显示标题。
- `ImagePreviewTitleResolveContext`：标题解析上下文，承载 current effective item、显示索引和总数等只读输入。
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

当前项状态流必须保持为一条共享路径：

```text
SourceUri / SourceUris
  -> EffectiveItems
CurrentIndex
  -> current effective item (clamped for display)
  -> inline cover (unless CoverSourceUri)
  -> dialog / overlay current item
```

预览标题状态流必须使用同一 current effective item：

```text
Window.Title / PreviewTitle / PreviewTitleResolver
current effective item + CurrentIndex + Count
  -> effective preview title
  -> ImagePreviewerTitleBar title
```

预览窗口标题图标使用 ImagePreviewer 自有的 PathIcon 路径：

```text
PreviewTitleIcon
  -> ImagePreviewerDialog.TitleIcon
  -> ImagePreviewerTitleBar.Icon
  -> IconPresenter#PART_IconPresenter
```

源码中的状态入口按以下语义维护：

- 图片来源：`SourceUri`、`SourceUris`、`CoverSourceUri`、`FallbackSourceUri`。
- 内容与数据：`CoverIndicatorContent`、`CoverIndicatorContentTemplate`、`LoadingContent`、`LoadingContentTemplate`、`ErrorContent`、`ErrorContentTemplate`、`ImageMaxScale`、`ImageMinScale`、`ImageScaleStep`、`ImageTranslateX`、`ImageTranslateY`。
- 选择与集合：`Count`、`CurrentIndex`。`CurrentIndex` 是控件级当前项索引，不是弹层局部状态；未设置 `CoverSourceUri` 时，普通封面和弹出预览宿主都必须从同一 current effective item 派生展示内容。
- 预览标题：`PreviewTitle`、`PreviewTitleIcon`、`PreviewTitleResolver`、`IImagePreviewTitleResolver`、`ImagePreviewTitleResolveContext`。非空白显式标题优先；显式标题为空时从 current effective item 解析标题；标题图标使用 `PathIcon?`，只在显式设置时进入预览标题栏。
- 交互与状态：`IsDialogModal`、`IsDialogTopmost`、`IsModal`、`IsMotionEnabled`、`IsOpen`、`IsShowCoverMask`。
- 视觉与布局：`CoverHeight`、`CoverWidth`。
- 其他稳定入口：`MaxScale`、`MinScale`、`ScaleStep`、`Stretch`、`Transform`。

维护要求：

- 外部设置的 Avalonia 属性必须在模板应用前后保持一致。
- 集合、当前项、图片来源替换、fallback 和异步 loader 必须能处理 reset、replace 和 clear。
- `CurrentIndexProperty` 变更、`SourceUri` / `SourceUris` 变更、fallback 状态变化和 effective items 重建都必须触发封面 effective item 重新解析。
- 封面 effective item resolver 必须使用 clamp 后的 `CurrentIndex` 选择项，不能把默认封面硬编码为 `EffectiveItems[0]`。
- `CoverSourceUri` 拥有最高封面优先级；它命中时封面走独立的来源、加载和 fallback 路径，不能反向改写 `CurrentIndex`。
- 标题 effective state 必须在 `CurrentIndex`、effective items、显式标题和 resolver 变化时重新计算，且不能使用已经被集合替换或 clamp 结果变更前的旧图片项。
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

## 5. 组合结构模型

ImagePreviewer 家族由 public 控件、预览宿主、标题栏、图片场景、封面、toolbar 和图片项状态对象协作完成。组合结构优先以 `Themes/` 中的 ControlTheme 和 template part 为准；C# 创建的 dialog / overlay host 用于补充打开状态和宿主生命周期。

### 控件角色图

```text
ImagePreviewer (ImagePreviewerTheme.axaml)
  -> Border
     -> ImagePreviewerCover
        -> ImagePreviewRenderer
        -> Border#PART_LoadingPresenter
           -> SkeletonImage (default loading content)
        -> Border#PART_ErrorPresenter
           -> 图片失败占位 (default error content)
        -> Border#Mask
           -> ContentPresenter#MaskContentPresenter

ImageGroupPreviewer (ImageGroupPreviewerTheme.axaml)
  -> ItemsControl#PART_CoverItemsControl

ImagePreviewerDialog (ImagePreviewerDialogTheme.axaml + WindowTheme)
  -> ImagePreviewerTitleBar (window title bar)
     -> StackPanel#PART_TitleLayout
        -> IconPresenter#PART_IconPresenter
        -> ContentPresenter#PART_ContentPresenter
  -> ImageViewer
     -> Canvas#PART_ImageViewerScene
        -> ImagePreviewRenderer#PART_ImageRenderer
     -> Border#PART_LoadingPresenter
        -> Spin (default loading content)
     -> Border#PART_ErrorPresenter
        -> 图片失败占位 (default error content)
     -> ImagePreviewNavButton#PART_PreviousButton
     -> ImagePreviewNavButton#PART_NextButton
     -> ImagePreviewFloatToolbar
        -> IconButton#PART_ScaleDownButton
        -> IconButton#PART_ScaleUpButton
        -> ToggleIconButton#PART_FitToWindowButton
        -> IconButton#PART_HorizontalFlipButton
        -> IconButton#PART_VerticalFlipButton
        -> IconButton#PART_RotateLeftButton
        -> IconButton#PART_RotateRightButton

ImagePreviewerOverlayHost (ImagePreviewerThemes.axaml + runtime host)
  -> IconButton#PART_CloseButton
  -> ImageViewer
  -> ImagePreviewToolbar
```

### 协作节点

| 节点 | 类型 | 来源 | 生命周期 owner | 影响的 public API | 稳定性 | Agent 使用边界 |
| --- | --- | --- | --- | --- | --- | --- |
| `ImagePreviewer` / `ImageGroupPreviewer` | public control | `ImagePreviewerTheme.axaml`、`ImageGroupPreviewerTheme.axaml` | 控件实例 | `SourceUri`、`SourceUris`、`CoverSourceUri`、`CurrentIndex`、`IsOpen` | public | 可作为用户 API 和主题入口理解。 |
| `ImagePreviewerCover` | public control | `ImagePreviewerCoverTheme.axaml` | 控件实例 / cover template | `CoverIndicatorContent`、`LoadingContent`、`ErrorContent`、`IsShowCoverMask` | public | 可作为封面视觉语义理解；内部 mask 节点不作为用户 API。 |
| `ImagePreviewerDialog` | public window subclass | `ImagePreviewerDialogTheme.axaml`、runtime open path | `AbstractImagePreviewer` 打开状态 | `IsOpen`、`IsDialogModal`、`IsDialogTopmost`、`PreviewTitle`、`PreviewTitleIcon` | internal-observable | 用户通过 ImagePreviewer API 间接配置，不依赖 dialog 模板内部结构。 |
| `ImagePreviewerTitleBar` | title bar control | `ImagePreviewerTitleBarTheme.axaml` | `ImagePreviewerDialog` / Window template | `PreviewTitle`、`PreviewTitleIcon` | template-stable | `PART_TitleLayout`、`PART_IconPresenter` 是主题维护契约；不得通过 `Window.Icon` 或 `Window.Logo` 显示预览标题图标。 |
| `ImageViewer` | public control | `ImageViewerTheme.axaml` | dialog / overlay host | `CurrentIndex`、`ImageScaleStep`、`ImageMinScale`、`ImageMaxScale`、toolbar request events | public | 负责预览层图片场景和 toolbar 状态，不拥有图片加载任务。 |
| `ImagePreviewRenderer` | renderer control | `ImageViewerTheme.axaml`、`ImagePreviewerCoverTheme.axaml` | cover / viewer template | `Stretch`、`Transform`、loaded image result | template-stable | 只消费 `LoadedImageSource`；不得在 renderer 内发起来源加载。 |
| `ImagePreviewToolbar` / `ImagePreviewFloatToolbar` | toolbar controls | `ImagePreviewToolbarTheme.axaml`、`ImagePreviewFloatToolbarTheme.axaml` | viewer / overlay host | request events、`CurrentIndex`、`Count` | template-stable | 动作按钮 part 可用于主题维护；事件顺序属于兼容边界。 |
| `ImagePreviewItem` | state object | C# runtime model | `AbstractImagePreviewer` / effective items | `SourceUri`、`SourceUris`、`FallbackSourceUri`、loading/error 状态 | internal-observable | 是图片加载状态 owner，不应反向持有视觉对象。 |
| `IImageSourceLoader` | service contract | C# loading model | 控件或注入服务 owner | 图片来源加载、取消、fallback | internal-observable | 统一本地、资源和远程加载；不得同步等待网络 I/O。 |

标题图标组合只允许通过 `PreviewTitleIcon -> ImagePreviewerDialog.TitleIcon -> ImagePreviewerTitleBar.Icon -> PART_IconPresenter` 这条路径进入标题栏。`PART_IconPresenter` 的位置是标题区域视觉契约，图标来源是 ImagePreviewer 自有 `PathIcon?` 契约；未设置时必须保持无图标状态，不能从应用图标、主窗口图标、`Window.Icon` 或 `Window.Logo` fallback。

`PreviewTitleIcon -> ImagePreviewerDialog.TitleIcon` 由打开预览时创建的 runtime open state 建立 C# relay，并在宿主关闭时随 open state 释放，因为 `ImagePreviewerDialog` 不是 `ImagePreviewer` 控件模板中的稳定 AXAML 节点。`ImagePreviewerDialog.TitleIcon -> ImagePreviewerTitleBar.Icon` 由 `Window.NotifyConfigureTitleBar(...)` 建立绑定，因为标题栏由 `Window.NotifyCreateTitleBar(...)` 运行时创建，不属于 `ImagePreviewerDialogTheme.axaml` 的静态模板节点。`ImagePreviewerTitleBar.Icon -> PART_IconPresenter.Icon` 必须留在 `ImagePreviewerTitleBarTheme.axaml` 中使用 `TemplateBinding` 表达。

## 6. 生命周期与模板接入

生命周期规则：

- 构造阶段只注册必要状态，不依赖 template part。
- 模板应用时获取 part、建立事件订阅和绑定，并先释放旧 part 订阅。
- 控件卸载、弹层关闭、窗口关闭、集合替换或 container recycle 时释放事件订阅和资源宿主。
- DynamicResource、TokenResourceBinder 或 C# binding 必须有明确 owner 和释放点。
- 图片加载任务由图片项或控件 owner 创建取消令牌。`SourceUri` 变更、`SourceUris` 替换、弹层关闭、控件 detach 和 owner dispose 时必须取消仍在运行的加载。
- 加载完成后需要通知 `ImagePreviewRenderer` 和 `ImageViewer` 重新 measure/arrange，保证居中、fit-to-window、缩放按钮状态和拖拽边界基于真实图片尺寸计算。
- Browser 和 Desktop 宿主下的主题加载顺序不得影响 public API 语义。
- 预览 dialog 的 title bar 不直接恢复通用 `Window.Title` 绑定；`Window.Title` 只作为显式标题输入参与 effective preview title 算法，最终由 `ImagePreviewerTitleBar` 显示算法结果。

稳定 template part 接入点：

- `PART_CloseButton`：承载用户触发入口、导航或关闭动作。
- `PART_CoverItemsControl`：承载集合项、布局面板或虚拟化内容。
- `PART_FitToWindowButton`：承载用户触发入口、导航或关闭动作。
- `PART_HorizontalFlipButton`：承载用户触发入口、导航或关闭动作。
- `PART_ImageRenderer`：稳定模板协作入口，重命名前必须同步主题和实现。
- `PART_ImageViewerScene`：稳定模板协作入口，重命名前必须同步主题和实现。
- `PART_LoadingPresenter`：承载图片加载状态内容。默认封面使用图片 Skeleton 占位，预览层使用居中 Spin；`LoadingContent` / `LoadingContentTemplate` 只替换 presenter 内容，不拥有加载状态。
- `PART_ErrorPresenter`：承载图片加载失败内容；存在 `FallbackSourceUri` 时优先展示 fallback 结果。没有可用 fallback 时显示 `ErrorContent` / `ErrorContentTemplate` 或默认本地化失败占位。
- `PART_IconPresenter`：预览窗口标题图标展示入口，内容来自 `ImagePreviewer.PreviewTitleIcon`，位于 `PART_TitleLayout` 内的标题文字左侧。
- `PART_TitleLayout`：标题图标和标题文字的水平布局，使用 `WindowTitleBarToken.LogoAndTitleSpacing` 作为二者间距。
- `PART_NextButton`：承载用户触发入口、导航或关闭动作。
- `PART_PreviousButton`：承载用户触发入口、导航或关闭动作。
- `PART_RotateLeftButton`：承载用户触发入口、导航或关闭动作。
- `PART_RotateRightButton`：承载用户触发入口、导航或关闭动作。
- `PART_ScaleDownButton`：承载用户触发入口、导航或关闭动作。
- `PART_ScaleUpButton`：承载用户触发入口、导航或关闭动作。
- `PART_VerticalFlipButton`：承载用户触发入口、导航或关闭动作。

## 7. 交互与事件处理

ImagePreviewer 的交互事件应从输入源收敛到控件级语义事件：

- Pointer、keyboard、focus 和 command 事件不应绕过 Avalonia 基础控件语义。
- 弹层、窗口或 overlay 类路径必须稳定处理打开、关闭、取消、重复打开和宿主失活。
- 集合类路径必须稳定处理 source prepare、clear、当前项切换和图片项回收。
- 输入类路径必须保持 Form、validation、clear、placeholder 和键盘行为一致。
- loading 状态下，上一张/下一张和关闭动作保持可用；缩放、旋转、拖拽和 fit-to-window 等依赖真实图片尺寸的动作在当前图片未 loaded 前禁用。

稳定事件路径包括 `FitToWindowRequest`、`HorizontalFlipRequest`、`NextRequest`、`PreviousRequest`、`RotateLeftRequest`、`RotateRightRequest`、`ScaleDownRequest`、`ScaleUpRequest`、`VerticalFlipRequest`。事件参数和触发时机属于兼容边界。

## 8. 内部算法与关键流程

维护者需要重点关注以下流程：

- API 默认值到 effective state 的归一。
- Template part 重新应用时的状态回放。
- `ImageSourceUri` 规范化、来源身份 key 生成和来源类型判定。
- `IImageSourceLoader` 对 `avares://`、本地路径、`file://`、相对路径和 `http(s)://` 的统一异步加载。
- `ImagePreviewItemState` 的 pending、loading、loaded、failed 迁移，以及 fallback 加载路径。
- 快速切换图片来源时的取消、版本校验和过期结果释放。
- 封面 loading/error 占位尺寸解析：优先使用显式 `CoverWidth` / `CoverHeight`，其次使用模板布局传入的有效约束，最后使用 `ImagePreviewerToken.CoverImageWidth`。没有 `LoadedImageSource` 时不得让 loading 或 error 内容本身决定封面高度。
- current effective item resolver：从 effective items 和 `CurrentIndex` 计算展示项，显示层对越界索引进行 clamp，并保持 public `CurrentIndex` 原值不被静默改写。
- 封面来源解析：先判断 `CoverSourceUri`，命中时使用显式封面；否则使用 current effective item，使普通封面、dialog 和 overlay 的当前项语义一致。
- 预览标题解析：先检查预览窗口或宿主的 `Window.Title`，非空白时直接使用；否则检查 `PreviewTitle`；仍为空时使用 `PreviewTitleResolver` 基于 current effective item 解析标题；解析不到标题时保持空态。
- 预览窗口标题图标：`PreviewTitleIcon` relay 到 `ImagePreviewerDialog.TitleIcon`，再绑定到 `ImagePreviewerTitleBar.Icon`；主题只负责用 `PART_IconPresenter` 把显式 `PathIcon` 放在标题文字左侧，不创建右侧按钮或 action slot 语义，也不走 `Window.Icon` fallback。
- 默认标题 resolver：本地路径和 `file://` 从 `ImageSourceUri.LocalPath` 提取文件名，`http(s)://` 从 `Uri.AbsolutePath` 最后一个非空 path segment 提取文件名并忽略 query/fragment，`avares://` 从资源路径最后一个非空 path segment 提取文件名，unsupported 或无法提取名称时返回 `null`。
- 默认失败占位：封面和预览层共享本地化失败文案，使用图片失败语义图标和低干扰背景。主题不得硬编码 `Image load failed`，也不得把网络失败显示成仅由文本撑开的灰色窄条。
- 主题资源、Token 和 SharedToken 计算后的视觉更新。
- `SourceUris`、current item 和弹层宿主之间的集合同步。
- 动效启停、初始加载阶段 transition 抑制和卸载取消。

实现文档不逐行解释私有方法。若某个私有算法成为稳定维护入口，应在本节补充算法不变量，而不是把代码复述为说明书。

## 9. 资源、性能与 AOT 边界

资源和 AOT 约束：

- 不通过运行时反射扫描 public API、Token 或 Gallery 表格数据。
- 不把可静态声明的模板结构迁移到 C# 动态创建。
- 异步加载、上传、弹层和窗口生命周期必须能取消或释放。
- 网络图片必须通过异步加载服务处理，不允许在 UI 线程同步等待网络 I/O。
- 本地、资源和远程图片共享同一来源身份 key 规范化规则，用于去重、旧结果判定和扩展场景。
- 默认 loading/error 视觉必须保持 AXAML-first。封面 Skeleton、预览 Spin、失败图标和本地化文本应由模板和资源表达；除非需要计算稳定占位尺寸，否则不要用 C# 动态创建视觉节点。
- 默认失败文案属于 ImagePreviewer 控件语言资源，新增或调整文案时同步 `en_US`、`zh_CN`、`zh_TW` 语言提供器和生成语言资源，不在主题中写死英文。
- 标题 resolver 必须是同步、确定性的纯解析逻辑；不得访问文件系统、发起网络请求、等待异步任务或通过运行时反射发现模型成员。
- 预览标题图标必须使用 `PathIcon? PreviewTitleIcon` 链路，不通过运行时反射、文件探测、平台特判、`Window.Icon` 或主窗口 fallback 生成额外图标模型。
- `LoadedImageSource` 由控件当前加载项持有；来源替换、取消或控件释放时必须释放旧结果。
- Source generator 生成文件不手工编辑；需要修改时改输入源或 generator。

性能边界：

- 控件应优先复用 Avalonia 原生虚拟化、模板绑定和资源系统。
- 避免为每次状态变化创建不必要的视觉对象、订阅或动画对象。
- 大集合控件必须保证 container recycle 后不会泄漏旧 item 状态。
- loading 视觉可延迟显示以避免本地文件和 `avares://` 快速完成时闪烁；延迟只影响视觉，不改变状态机和取消语义。

## 10. 维护不变量

维护 ImagePreviewer 时不得破坏：

- Public API、默认值、事件顺序和 Gallery 可观察行为。
- Template part 名称、ControlTheme key、伪类和资源 key。
- 旧 template part、事件订阅、Popup/Flyout/Window host 和 collection view 的释放路径。
- 图片加载状态的单 owner：`ImageSourceUri` 是来源，`ImagePreviewItem` 是状态，`LoadedImageSource` 是结果，`ImagePreviewRenderer` 只负责渲染。
- `SourceUri`、`SourceUris`、`CoverSourceUri` 和 `FallbackSourceUri` 的加载、取消、来源身份和失败处理一致性。
- 单图 `ImagePreviewer` 与 `ImageGroupPreviewer` 对 `CoverWidth` / `CoverHeight` 的消费一致性。远程图片 loading/failed 时不能因为没有图片自然尺寸而丢失封面高度。
- 默认 loading/error 视觉不能破坏自定义内容入口。用户设置 `LoadingContent` 或 `ErrorContent` 后，模板仍负责稳定尺寸、状态显隐和 mask 抑制。
- 默认失败文案必须走控件本地化资源，不允许在 `ImagePreviewerCoverTheme.axaml` 或 `ImageViewerTheme.axaml` 中硬编码英文字符串。
- `CurrentIndex` 的单一语义：普通封面、dialog 和 overlay 使用同一 current effective item；`CoverSourceUri` 只覆盖封面来源，不改变弹层当前项。
- 预览标题的单一算法：非空白 `Window.Title` 或 `PreviewTitle` 优先，resolver 只在显式标题为空时运行；标题必须跟随 current effective item，不能保留旧集合项的文件名。
- 预览标题图标的单一路径：`PreviewTitleIcon` 只能进入 `ImagePreviewerDialog.TitleIcon`，再绑定到 `ImagePreviewerTitleBar.Icon` 和 `PART_IconPresenter`；不得转接 `Window.Icon`、`Window.Logo` 或右侧扩展区域来表达标题图标。
- 加载完成后必须重新计算 `ImageViewer` 的布局、居中、fit-to-window 和交互边界。
- Light/Dark、Browser/Desktop 和不同 SizeType 下的主题一致性。
- 文档、Gallery API 表、Token 表与源码契约的一致性。

## 11. 测试与验证

推荐验证：

- 纯文档改动运行 `git diff --check` 并检查相对链接。
- 控件 API 或行为变更运行对应 `tests/AtomUI.Desktop.Controls.Tests` 或专用包测试。
- 图片加载模型变更覆盖 `ImageSourceUri` 解析、本地文件、`avares://`、fake HTTP、取消、防旧结果回写、fallback 和 loading/error 模板状态。
- loading/error 默认视觉变更覆盖：单图和多图封面均消费 `CoverWidth` / `CoverHeight`，封面默认 loading 使用 `SkeletonImage`，封面和预览层默认失败文案来自 `ImagePreviewerLangResource`，主题中不再出现硬编码 `Image load failed`。
- 预览标题模型变更覆盖显式标题优先级、默认文件名解析、current item 切换、集合替换、越界 `CurrentIndex` clamp、resolver 更换、`PreviewTitleIcon` 到 `TitleIcon` 的转接、未设置图标时不显示 fallback 图标，以及标题栏 `PART_IconPresenter` 布局。
- DataGrid 相关变更运行 `tests/AtomUI.Desktop.Controls.DataGrid.Tests`。
- Gallery 示例、API 表或 Token 表变更运行 `tests/AtomUIGallery.Tests`。
- AOT、生成器或动态数据路径变更按 Gallery NativeAOT 发布流程验证。
