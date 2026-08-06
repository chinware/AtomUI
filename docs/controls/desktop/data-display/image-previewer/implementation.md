# ImagePreviewer 桌面版实现原理

本文档描述 ImagePreviewer 桌面版的内部实现范围、源码职责、图片源懒加载管线、状态流、生命周期、资源边界和维护规则。公共设计与 API 契约见 [ImagePreviewer 桌面版架构设计](overview.md)，变化记录见 [ImagePreviewer Changelog](changelog.md)。涉及控件 Token 的实现应同时阅读 [ImagePreviewer Token 设计](token.md)。

## 1. 实现定位

本文档覆盖 ImagePreviewer 的控件实现、统一图片源加载、主题接入、状态同步和 Gallery 可见维护边界。具体属性注册、默认值、绘制细节和 AXAML selector 仍应直接阅读源码；本文只记录维护者必须理解的稳定结构和不变量。

## 2. 源码文件结构

主要源码文件：

- `src/AtomUI.Desktop.Controls/ImagePreviewer`：控件主体、图片源模型、加载状态、预览宿主和 renderer 所在目录，代表文件包括 `AbstractImagePreviewer.cs`、`ImagePreviewer.cs`、`ImageViewer.cs`、`ImagePreviewRenderer.cs`、`IImagePreviewSource.cs`、`UriImagePreviewSource.cs`、`StreamImagePreviewSource.cs`、`ImageSourceUri.cs`、`ImagePreviewItem.cs`、`LoadedImageSource.cs` 和 `IImageSourceLoader.cs`。
- `src/AtomUI.Desktop.Controls/ImagePreviewer/Localization`：`ImagePreviewerLangResourceKind.cs` 定义稳定 Catalog，`en-US.xlf`、`zh-CN.xlf`、`zh-TW.xlf` 提供内置翻译。
- `src/AtomUI.Desktop.Controls/ImagePreviewer/Themes`：12 个文件，代表文件 `ImageGroupPreviewerTheme.axaml`、`ImagePreviewFloatToolbarTheme.axaml`、`ImagePreviewNavButtonTheme.axaml`、`ImagePreviewToolbarTheme.axaml`、`ImagePreviewerCoverTheme.axaml` 等。

职责边界：

- 控件主文件保留 public/protected API、Avalonia 属性注册、事件和主要生命周期入口。
- 图片源模型文件只表达 `IImagePreviewSource`、`IImagePreviewSourceIdentity`、`UriImagePreviewSource`、`StreamImagePreviewSource`、`ImageSourceUri`、`ImagePreviewItem`、`LoadedImageSource`、加载调度器和加载服务契约，不承载视觉模板逻辑。
- Theme 文件负责静态视觉结构、template part、selector 和资源绑定。
- Token 文件只提供组件视觉变量，不保存实例状态。
- Gallery 文件只展示用法和示例，不作为运行时逻辑 owner。

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
- `ImagePreviewerToken`：控件 Token scope，负责从全局 token 派生控件语义变量。
- `ImageViewer`：控件核心或内部协作类型，维护 public surface 与主题可观察行为。
- `IImagePreviewSource`：图片来源主契约，负责提供可选显示名、可选内容类型和按需打开数据流的异步入口，不强制暴露身份字段。
- `IImagePreviewSourceIdentity`：可选高级身份扩展，供需要跨 source 实例复用 item / loaded result 的来源实现显式提供稳定 identity。
- `UriImagePreviewSource`：URI 来源默认实现，负责把 `string`、`Uri` 或 `ImageSourceUri` 输入适配为统一 source，并支持本地、资源和网页图片的普通用法。
- `StreamImagePreviewSource`：数据流来源默认实现，负责保存用户提供的 `OpenReadAsync` 工厂和可选 identity，每次加载返回新的可读流。
- `ImageSourceUri`：图片来源 URI 值对象，负责保存和规范化用户输入的图片来源字符串，并作为 `UriImagePreviewSource` 的 URI 解析基础。
- `ImagePreviewItem`：图片项状态 owner，维护来源、加载状态、加载版本、取消令牌和加载结果。
- `LoadedImageSource`：加载完成后的 renderer 输入，封装 Bitmap、SVG 文本、源尺寸和释放语义。
- `IImageSourceLoader`：统一图片加载服务，封装 `avares://`、本地文件、`file://`、相对路径和 `http(s)://` 的异步加载。
- `ImagePreviewLoadScheduler`：内部加载调度器，统一封面、当前项、预加载和 fallback 的并发控制、优先级、取消、generation 校验和结果回写。
- `IImagePreviewTitleResolver`：预览标题解析接口，基于当前图片来源和集合上下文同步返回可显示标题。
- `ImagePreviewTitleResolveContext`：标题解析上下文，承载 current effective item、显示索引和总数等只读输入。
- `ImagePreviewerLangResourceKind`：稳定的本地化 Catalog enum；生成器从三个 XLIFF 文件编译资源表和 XAML 扩展。

核心协作规则：

- 控件实例是 public API 和运行时状态 owner。
- `ImagePreviewItem` 是单个图片来源的 loading/loaded/failed 状态 owner；renderer 只消费加载结果，不发起加载。
- Template part 是视觉协作对象，生命周期必须受 `OnApplyTemplate` 或模板加载流程管理。
- 数据对象、选项对象、任务对象或节点对象只保存业务数据，不应反向持有不可释放的视觉对象。
- 弹层、窗口、计时器、异步 loader 和全局管理器必须有明确关闭、解绑或释放路径。

## 4. 状态与数据流

ImagePreviewer 的状态流遵循下面路径：

```text
Public API / IImagePreviewSource / Command / Event
  -> 控件实例状态
  -> ImagePreviewItem state / internal state / effective state / pseudo-class
  -> template part property / AXAML selector / loading presenter / error presenter
  -> renderer / popup / adorner / Gallery observable behavior
```

当前项状态流必须保持为一条共享路径：

```text
Source / Sources
  -> EffectiveItems
CurrentIndex
  -> current preview item (clamped for display)
  -> dialog / overlay current item
CoverIndex
  -> inline cover item (clamped for display)
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

- 图片来源：`Source`、`Sources`、`FallbackSource`。`IImagePreviewSource` 是唯一来源契约，URI 场景必须通过 `UriImagePreviewSource` 显式进入同一个 source batch。
- 内容与数据：`CoverIndicatorContent`、`CoverIndicatorContentTemplate`、`LoadingContent`、`LoadingContentTemplate`、`ErrorContent`、`ErrorContentTemplate`、`ImageMaxScale`、`ImageMinScale`、`ImageScaleStep`、`ImageTranslateX`、`ImageTranslateY`。
- 选择与集合：`Count`、`CurrentIndex`。`CurrentIndex` 是控件级当前项索引，默认双向绑定，不是弹层局部状态；普通封面不得从 `CurrentIndex` 派生展示内容。
- 封面展示：`CoverIndex`。它是关闭态封面索引，只用于选择封面加载和展示项，不参与打开后当前项、导航或标题解析。
- 加载调度：`MaxConcurrentLoads`、`PreloadCount`。前者限制同一控件实例内所有图片加载路径的并发；后者控制打开态当前项周围的预加载窗口。
- 预览标题：`PreviewTitle`、`PreviewTitleIcon`、`PreviewTitleResolver`、`IImagePreviewTitleResolver`、`ImagePreviewTitleResolveContext`。非空白显式标题优先；显式标题为空时从 current effective item 解析标题；标题图标使用 `PathIcon?`，只在显式设置时进入预览标题栏。
- 交互与状态：`IsDialogModal`、`IsDialogTopmost`、`IsModal`、`IsMotionEnabled`、`IsOpen`、`IsShowCoverMask`。
- 视觉与布局：`CoverHeight`、`CoverWidth`。
- 其他稳定入口：`MaxScale`、`MinScale`、`ScaleStep`、`Stretch`、`Transform`。

维护要求：

- 外部设置的 Avalonia 属性必须在模板应用前后保持一致。
- 集合、当前项、图片来源替换、fallback 和异步 loader 必须能处理 reset、replace 和 clear。
- `IsOpen` 与 `CurrentIndex` 必须作为受控状态保持默认 `TwoWay`；控件内部打开、关闭或导航应使用 current value 语义，不覆盖用户绑定。
- `CurrentIndex` 转接到 `ImagePreviewerDialog` 与 `ImagePreviewerOverlayHost` 时必须保持 `TwoWay`，转接 binding 归宿主 disposable 管理，关闭或重新创建宿主时释放。
- `CurrentIndexProperty` 变更、`Source` / `Sources` 变更、fallback 状态变化和 effective items 重建都必须触发当前预览项重新解析。
- `CoverIndexProperty` 变更、`Source` / `Sources` 变更、fallback 状态变化和 effective items 重建都必须触发封面 item 重新解析。
- 封面 effective item resolver 必须使用 clamp 后的 `CoverIndex` 选择项，不能把默认封面硬编码为 `EffectiveItems[0]`，也不能复用 `CurrentIndex`。
- 点击封面、封面加载完成或封面加载失败只允许影响封面视觉和 `IsOpen`，不得隐式修改 `CurrentIndex`。
- 标题 effective state 必须在 `CurrentIndex`、effective items、显式标题和 resolver 变化时重新计算，且不能使用已经被集合替换或 clamp 结果变更前的旧图片项。
- 伪类和 internal state 必须从单一 owner 推导，避免双向同步导致循环更新。
- overview.md 的 API 契约说明应与源码实际状态流一致。

图片加载状态流：

```text
Source / Sources
  -> source load batch
IImagePreviewSource
  -> ImagePreviewItem(Pending)
  -> ImagePreviewLoadScheduler.Enqueue(...)
  -> IImageSourceLoader.LoadAsync(...)
  -> OpenReadAsync(...) when scheduled
  -> ImagePreviewItem(Loading)
  -> LoadedImageSource / Failed
source load batch
  -> keep loaded items / skip failed items / fallback when all failed
  -> ImagePreviewRenderer
  -> ImageViewer measure / arrange / fit-to-window
```

`ImagePreviewItemState` 只能由 item owner 写入。加载任务返回时必须校验 generation、来源身份解析结果和取消状态；过期结果必须释放后丢弃，不能覆盖新来源。`Source` / `Sources` 的 fallback 决策必须由当前来源集合加载批次统一完成，单个 `ImagePreviewItem` 失败只能更新自身状态，不能直接替换整组 `EffectiveItems`。

`IImagePreviewSource.OpenReadAsync` 是唯一的数据流打开入口，只能由加载调度器启动后的 loader 调用。实现不得在来源物化阶段、标题解析阶段、封面选择阶段或模板渲染阶段提前打开流。`OpenReadAsync` 返回的流由 loader 在解码完成或失败后释放；用户实现必须每次返回新的可读流，不能返回可被多次消费的共享 `Stream` 实例。

`UriImagePreviewSource` 负责把 URI 来源纳入同一 source 流程，普通网页图片可以通过 `new UriImagePreviewSource("https://example.com/image.png")` 配置，并使用规范化后的 `ImageSourceUri.CacheKey` 作为可复用 identity。`StreamImagePreviewSource` 只接受 `Func<CancellationToken, ValueTask<Stream>>` 工厂和可选 `identity`，不提供 `Stream` 直接构造函数；未提供 identity 时使用 source 对象引用作为身份。内存字节数组应通过工厂返回新的只读 `MemoryStream`。source 对象不承载 cover、fallback、title metadata 或业务 metadata，避免把来源契约扩展成复合业务对象。

加载结果按用途拆分维护：

- 封面加载状态和预览加载状态可以共享同一来源身份，但必须有独立 owner，避免关闭态封面释放时误释放正在显示的预览图片。
- 当前预览项结果优先保留；邻近预加载结果可在 `CurrentIndex` 移动、预加载窗口变化、关闭预览宿主或来源 generation 变化时释放。
- 如果同一来源同时被封面和预览消费，内部可以复用同一个 `LoadedImageSource`，但必须通过明确 owner 或引用计数保证最后一个消费者释放时才 dispose。

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

ImagePreviewerOverlayHost (ImagePreviewerOverlayHostTheme.axaml + runtime host)
	  -> IconButton#PART_CloseButton
	  -> ImageViewer
	  -> ImagePreviewToolbar
```

### 协作节点

| 节点 | 类型 | 来源 | 生命周期 owner | 影响的 public API | 稳定性 | Agent 使用边界 |
| --- | --- | --- | --- | --- | --- | --- |
| `ImagePreviewer` / `ImageGroupPreviewer` | public control | `ImagePreviewerTheme.axaml`、`ImageGroupPreviewerTheme.axaml` | 控件实例 | `Source`、`Sources`、`FallbackSource`、`CurrentIndex`、`CoverIndex`、`IsOpen` | public | 可作为用户 API 和主题入口理解。 |
| `ImagePreviewerCover` | public control | `ImagePreviewerCoverTheme.axaml` | 控件实例 / cover template | `CoverIndicatorContent`、`LoadingContent`、`ErrorContent`、`IsShowCoverMask` | public | 可作为封面视觉语义理解；内部 mask 节点不作为用户 API。 |
| `ImagePreviewerDialog` | public window subclass | `ImagePreviewerDialogTheme.axaml`、runtime open path | `AbstractImagePreviewer` 打开状态 | `IsOpen`、`IsDialogModal`、`IsDialogTopmost`、`PreviewTitle`、`PreviewTitleIcon` | internal-observable | 用户通过 ImagePreviewer API 间接配置，不依赖 dialog 模板内部结构。 |
| `ImagePreviewerTitleBar` | title bar control | `ImagePreviewerTitleBarTheme.axaml` | `ImagePreviewerDialog` / Window template | `PreviewTitle`、`PreviewTitleIcon` | template-stable | `PART_TitleLayout`、`PART_IconPresenter` 是主题维护契约；不得通过 `Window.Icon` 或 `Window.Logo` 显示预览标题图标。 |
| `ImageViewer` | public control | `ImageViewerTheme.axaml` | dialog / overlay host | `CurrentIndex`、`ImageScaleStep`、`ImageMinScale`、`ImageMaxScale`、toolbar request events | public | 负责预览层图片场景和 toolbar 状态，不拥有图片加载任务。 |
| `ImagePreviewRenderer` | renderer control | `ImageViewerTheme.axaml`、`ImagePreviewerCoverTheme.axaml` | cover / viewer template | `Stretch`、`Transform`、loaded image result | template-stable | 只消费 `LoadedImageSource`；不得在 renderer 内发起来源加载。 |
| `ImagePreviewToolbar` / `ImagePreviewFloatToolbar` | toolbar controls | `ImagePreviewToolbarTheme.axaml`、`ImagePreviewFloatToolbarTheme.axaml` | viewer / overlay host | request events、`CurrentIndex`、`Count` | template-stable | 动作按钮 part 可用于主题维护；事件顺序属于兼容边界。 |
| `IImagePreviewSource` / `IImagePreviewSourceIdentity` / `UriImagePreviewSource` / `StreamImagePreviewSource` | source model | C# runtime model | 用户数据 / `AbstractImagePreviewer` 物化流程 | `Source`、`Sources`、`FallbackSource` | public | 是图片来源公共契约；主接口只表达按需打开流，可选身份接口只服务复用和防旧结果回写，不承载封面、fallback、title metadata 或视觉状态。 |
| `ImagePreviewItem` | state object | C# runtime model | `AbstractImagePreviewer` / effective items | `Source`、`Sources`、`FallbackSource`、loading/error 状态 | internal-observable | 是图片加载状态 owner，不应反向持有视觉对象。 |
| source load batch | runtime algorithm | `AbstractImagePreviewer` loading path | `AbstractImagePreviewer` 当前加载轮次 | `Source`、`Sources`、`FallbackSource`、`EffectiveItems` | internal-observable | 是集合加载结果归并 owner；负责判断成功项保留、失败项跳过、全部失败 fallback 和旧批次防回写。 |
| `ImagePreviewLoadScheduler` | runtime service | C# loading path | `AbstractImagePreviewer` 当前 generation | `MaxConcurrentLoads`、`PreloadCount`、`CurrentIndex`、`CoverIndex` | internal-observable | 是唯一加载任务入口；负责优先级、并发上限、取消、过期结果丢弃和结果写回。 |
| `IImageSourceLoader` | service contract | C# loading model | 控件或注入服务 owner | 图片来源加载、取消、fallback | internal-observable | 统一本地、资源和远程加载；不得同步等待网络 I/O。 |

标题图标组合只允许通过 `PreviewTitleIcon -> ImagePreviewerDialog.TitleIcon -> ImagePreviewerTitleBar.Icon -> PART_IconPresenter` 这条路径进入标题栏。`PART_IconPresenter` 的位置是标题区域视觉契约，图标来源是 ImagePreviewer 自有 `PathIcon?` 契约；未设置时必须保持无图标状态，不能从应用图标、主窗口图标、`Window.Icon` 或 `Window.Logo` fallback。

`PreviewTitleIcon -> ImagePreviewerDialog.TitleIcon` 由打开预览时创建的 runtime open state 建立 C# relay，并在宿主关闭时随 open state 释放，因为 `ImagePreviewerDialog` 不是 `ImagePreviewer` 控件模板中的稳定 AXAML 节点。`ImagePreviewerDialog.TitleIcon -> ImagePreviewerTitleBar.Icon` 由 `Window.NotifyConfigureTitleBar(...)` 建立绑定，因为标题栏由 `Window.NotifyCreateTitleBar(...)` 运行时创建，不属于 `ImagePreviewerDialogTheme.axaml` 的静态模板节点。`ImagePreviewerTitleBar.Icon -> PART_IconPresenter.Icon` 必须留在 `ImagePreviewerTitleBarTheme.axaml` 中使用 `TemplateBinding` 表达。

`ImagePreviewerDialog.Background -> ImageViewer.Background` 由 dialog 创建运行时 viewer 时建立 C# binding，并由 `ImageViewerTheme.axaml` 根 `Panel` 使用 `TemplateBinding Background` 绘制。该实心内容背景用于在 Windows CSD maximize/restore 状态切换帧中遮住 `WindowDrawnDecorations` underlay 的通用窗口背景；overlay host 未设置 viewer 背景，因此仍保持 overlay 模式的透明内容层语义。

`ImagePreviewerDialog.SuppressTransformAnimation -> ImageViewer.SuppressTransformAnimation` 由 dialog 创建运行时 viewer 时建立 C# binding。`ImageViewerTheme.axaml` 只在 `IsMotionEnabled=True` 且 `SuppressTransformAnimation=False` 时启用 `ImageRenderTransform`、`ImageTranslateX` 和 `ImageTranslateY` 过渡；dialog 在自身 `WindowState` 或 `SizeChanged` 触发的窗口尺寸切换帧内短暂置位该状态，避免外层窗口 maximize/restore 与内层图片居中平移动画叠加。

## 6. 生命周期与模板接入

生命周期规则：

- 构造阶段只注册必要状态，不依赖 template part。
- 模板应用时获取 part、建立事件订阅和绑定，并先释放旧 part 订阅。
- 控件卸载、弹层关闭、窗口关闭、集合替换或 container recycle 时释放事件订阅和资源宿主。
- DynamicResource、TokenResourceBinder 或 C# binding 必须有明确 owner 和释放点。
- 图片加载任务由 `ImagePreviewLoadScheduler` 创建和持有取消令牌。`Source` 变更、`Sources` 替换、弹层关闭、控件 detach 和 owner dispose 时必须取消仍在运行且不再需要的加载。
- `Source` / `Sources` 每次物化都必须形成当前来源集合加载批次。批次完成前，单项失败不得触发 `FallbackSource` 替换整组结果；批次完成后若存在成功项，effective items 保留成功项并跳过失败项；批次全部失败时才进入 fallback。
- `Source` / `Sources` / `FallbackSource` 是唯一来源入口，不能再引入并行 URI 属性族；URI、本地文件、Avalonia 资源和远程图片统一通过 `UriImagePreviewSource` 进入 source batch。
- effective items 替换必须区分“保留项”和“废弃项”。保留的成功 `ImagePreviewItem` 及其 `LoadedImageSource` 不能在同次替换中被 dispose；旧批次项、被跳过的失败项和被取消项必须释放。
- 关闭态只保持封面显示所需加载。打开态 materialize 当前项、`PreloadCount` 邻近项和 `CoverIndex` 封面加载；关闭预览宿主时取消未完成的预览加载和预加载，并释放不再被封面或 current window 使用的结果。
- 每次来源集合替换都递增 generation。调度器、item、renderer 和 fallback 写回必须校验 generation；旧 generation 的结果即使成功返回也只能释放，不能写回 `EffectiveItems`、封面、预览宿主或标题状态。
- `MaxConcurrentLoads` 必须作为调度器的唯一并发闸门。封面加载、当前项加载、邻近预加载和 fallback 加载不得绕过调度器直接调用 `IImageSourceLoader`。
- 加载完成后需要通知 `ImagePreviewRenderer` 和 `ImageViewer` 重新 measure/arrange，保证居中、fit-to-window、缩放按钮状态和拖拽边界基于真实图片尺寸计算。
- Browser 和 Desktop 宿主下的主题加载顺序不得影响 public API 语义。
- 预览 dialog 的 title bar 不直接恢复通用 `Window.Title` 绑定；`Window.Title` 只作为显式标题输入参与 effective preview title 算法，最终由 `ImagePreviewerTitleBar` 显示算法结果。
- 预览 dialog 的 `TitleAlignment` 默认值为 `WindowCenter`，使 Windows、Linux 和 macOS 都默认按完整窗口水平中心排列标题；用户显式设置 `TitleAlignment` 时仍通过 `Window.NotifyConfigureTitleBar(...)` 投射到 `ImagePreviewerTitleBar`，并由 `WindowTitleBarLayoutPanel` 的既有算法处理左右安全区。
- 预览 dialog 在窗口状态或尺寸变化期间只短暂关闭图片 transform/translate 过渡；关闭窗口、切换图片、缩放、拖拽和 overlay host 生命周期不接管该窗口 resize suppression。

稳定 template part 接入点：

- `PART_CloseButton`：承载用户触发入口、导航或关闭动作。
- `PART_CoverItemsControl`：承载集合项、布局面板或虚拟化内容。
- `PART_FitToWindowButton`：承载用户触发入口、导航或关闭动作。
- `PART_HorizontalFlipButton`：承载用户触发入口、导航或关闭动作。
- `PART_ImageRenderer`：稳定模板协作入口，重命名前必须同步主题和实现。
- `PART_ImageViewerScene`：稳定模板协作入口，重命名前必须同步主题和实现。
- `PART_LoadingPresenter`：承载图片加载状态内容。默认封面使用图片 Skeleton 占位，预览层使用居中 Spin；`LoadingContent` / `LoadingContentTemplate` 只替换 presenter 内容，不拥有加载状态。
- `PART_ErrorPresenter`：承载图片加载失败内容；当前加载批次全部失败且存在 `FallbackSource` 时优先展示 fallback 结果。没有可用 fallback 时显示 `ErrorContent` / `ErrorContentTemplate` 或默认本地化失败占位。
- `PART_IconPresenter`：预览窗口标题图标展示入口，内容来自 `ImagePreviewer.PreviewTitleIcon`，位于 `PART_TitleLayout` 内的标题文字左侧。
- `PART_TitleLayout`：共享 `WindowTitleBarLayoutPanel` 的 Title 角色，承载标题图标和标题文字；仅当两者都有效时使用 `WindowTitleBarToken.LogoAndTitleSpacing`。
- `PART_LeftAddOn`：共享布局的 Leading 角色，承载图片工具栏；为空或隐藏时不产生操作区占位和间距。
- `PART_RightAddOn` 与 `PART_CaptionButtonGroup`：共同位于 Trailing 角色，按当前实测宽度限制标题安全区。
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
- `IImagePreviewSource` 的 `DisplayName` / `ContentType` 推断、`OpenReadAsync` 懒打开和返回流释放，以及 `IImagePreviewSourceIdentity` 可选身份解析。
- `UriImagePreviewSource` 对 `string`、`Uri`、`ImageSourceUri` 的统一适配，以及对 `avares://`、本地路径、`file://`、相对路径和 `http(s)://` 的普通 URI 加载。
- `StreamImagePreviewSource` 对用户数据流工厂的按需调用、取消传递和每次新流约束。
- `IImageSourceLoader` 对 URI 来源和数据流来源的统一异步加载。
- `ImagePreviewItemState` 的 pending、loading、loaded、failed 迁移，以及批次级 fallback 加载路径。
- `Source` / `Sources` 加载批次归并：单项失败只更新 item 状态；批次至少一个成功时保留成功项并跳过失败项；批次全部失败且存在 `FallbackSource` 时才替换为 fallback；旧批次完成时必须通过取消状态和当前 effective items 身份校验避免回写新状态。
- 加载调度窗口：关闭态 enqueue `CoverIndex`；打开态 enqueue `CurrentIndex`、`CoverIndex` 和 `PreloadCount` 邻近项；导航或 `CurrentIndex` 变化时重新计算窗口并取消窗口外未完成任务，但已加载封面 item 不应被同源重新物化清空。
- 加载优先级：当前预览项优先，封面次之，邻近预加载最后。fallback 加载只在当前来源集合达到全部失败条件后进入高优先级加载。
- 并发控制：调度器以 `MaxConcurrentLoads` 作为上限，所有加载结果都经过同一写回路径，禁止在属性变更、template part 或 renderer 中直接启动独立 loader。
- 快速切换图片来源时的取消、版本校验和过期结果释放。
- 封面 loading/error 占位尺寸解析：优先使用显式 `CoverWidth` / `CoverHeight`，其次使用模板布局传入的有效约束，最后使用 `ImagePreviewerToken.CoverImageWidth`。没有 `LoadedImageSource` 时不得让 loading 或 error 内容本身决定封面高度。
- current effective item resolver：从 effective items 和 `CurrentIndex` 计算展示项，显示层对越界索引进行 clamp，并保持 public `CurrentIndex` 原值不被静默改写。
- 封面来源解析：从 effective items 和 `CoverIndex` 计算展示项，显示层对越界索引进行 clamp，并保持 public `CoverIndex` 原值不被静默改写。封面解析不得读取或改写 `CurrentIndex`。
- 预览标题解析：先检查预览窗口或宿主的 `Window.Title`，非空白时直接使用；否则检查 `PreviewTitle`；仍为空时使用 `PreviewTitleResolver` 基于 current effective item 解析标题；解析不到标题时保持空态。
- 预览窗口标题图标：`PreviewTitleIcon` relay 到 `ImagePreviewerDialog.TitleIcon`，再绑定到 `ImagePreviewerTitleBar.Icon`；主题只负责用 `PART_IconPresenter` 把显式 `PathIcon` 放在标题文字左侧，不创建右侧按钮或 action slot 语义，也不走 `Window.Icon` fallback。
- 预览窗口标题对齐：`ImagePreviewerDialog` 覆盖 `TitleAlignment` 默认值为 `WindowCenter`；主题继续通过 `TemplateBinding` 把 `TitleAlignment` 传给 `WindowTitleBarLayoutPanel`，因此默认不触发通用 `Window` 的 `Auto` 平台策略，显式设置仍可覆盖。
- 预览窗口 resize motion：`ImagePreviewerDialog` 在 `WindowState` 与 `SizeChanged` 期间 coalesce `SuppressTransformAnimation` 恢复任务，确保连续 resize 事件只由最后一次恢复重新开启 viewer transitions。
- 默认标题 resolver：`UriImagePreviewSource` 的本地路径和 `file://` 从 `ImageSourceUri.LocalPath` 提取文件名，`http(s)://` 从 `Uri.AbsolutePath` 最后一个非空 path segment 提取文件名并忽略 query/fragment，`avares://` 从资源路径最后一个非空 path segment 提取文件名；非 URI source 只使用 `DisplayName`，unsupported 或无法提取名称时返回 `null`。
- 默认失败占位：封面和预览层共享本地化失败文案，使用图片失败语义图标和低干扰背景。主题不得硬编码 `Image load failed`，也不得把网络失败显示成仅由文本撑开的灰色窄条。
- 主题资源、Token 和 SharedToken 计算后的视觉更新。
- `Sources`、current item 和弹层宿主之间的集合同步。
- 动效启停、初始加载阶段 transition 抑制和卸载取消。

实现文档不逐行解释私有方法。若某个私有算法成为稳定维护入口，应在本节补充算法不变量，而不是把代码复述为说明书。

## 9. 资源、性能与 AOT 边界

资源和 AOT 约束：

- 不通过运行时反射扫描 public API、Token 或 Gallery 示例数据。
- 不把可静态声明的模板结构迁移到 C# 动态创建。
- 异步加载、上传、弹层和窗口生命周期必须能取消或释放。
- 网络图片必须通过异步加载服务处理，不允许在 UI 线程同步等待网络 I/O。
- 本地、资源和远程 URI 图片共享同一 `ImageSourceUri.CacheKey` identity 规范化规则，用于去重、旧结果判定和扩展场景；非 URI 来源只有显式实现 `IImagePreviewSourceIdentity` 时才跨实例复用。
- 默认 loading/error 视觉必须保持 AXAML-first。封面 Skeleton、预览 Spin、失败图标和本地化文本应由模板和资源表达；除非需要计算稳定占位尺寸，否则不要用 C# 动态创建视觉节点。
- 默认失败文案属于 ImagePreviewer Catalog；新增或调整文案时同步 Catalog enum 与 `en-US`、`zh-CN`、`zh-TW` XLIFF，不在主题中写死英文。
- 标题 resolver 必须是同步、确定性的纯解析逻辑；不得访问文件系统、发起网络请求、等待异步任务或通过运行时反射发现模型成员。
- 预览标题图标必须使用 `PathIcon? PreviewTitleIcon` 链路，不通过运行时反射、文件探测、平台特判、`Window.Icon` 或主窗口 fallback 生成额外图标模型。
- `LoadedImageSource` 由控件当前加载项持有；来源替换、取消或控件释放时必须释放旧结果。
- `ImagePreviewLoadScheduler` 不持有视觉对象，只持有来源、item 身份、generation、取消令牌和有限任务队列。调度器 dispose 时必须取消队列、取消运行任务并释放未交付结果。
- Source generator 生成文件不手工编辑；需要修改时改输入源或 generator。

性能边界：

- 控件应优先复用 Avalonia 原生虚拟化、模板绑定和资源系统。
- 避免为每次状态变化创建不必要的视觉对象、订阅或动画对象。
- 大图集合不得 eager load。`Sources` 包含 100 张图片时，关闭态只加载封面；打开态只加载当前项、封面项和预加载窗口内图片。
- 预加载窗口不得超过有效来源范围，且不能因快速导航叠加多个旧窗口任务。
- 大集合控件必须保证 container recycle 后不会泄漏旧 item 状态。
- loading 视觉可延迟显示以避免本地文件和 `avares://` 快速完成时闪烁；延迟只影响视觉，不改变状态机和取消语义。

## 10. 维护不变量

维护 ImagePreviewer 时不得破坏：

- Public API、默认值、事件顺序和 Gallery 可观察行为。
- Template part 名称、ControlTheme key、伪类和资源 key。
- 旧 template part、事件订阅、Popup/Flyout/Window host 和 collection view 的释放路径。
- 图片加载状态的单 owner：`IImagePreviewSource` 是来源，`ImagePreviewItem` 是状态，`LoadedImageSource` 是结果，`ImagePreviewRenderer` 只负责渲染。
- `Source`、`Sources`、`FallbackSource`、URI 便利入口和数据流来源的加载、取消、来源身份和失败处理一致性。
- 单图 `ImagePreviewer` 与 `ImageGroupPreviewer` 对 `CoverWidth` / `CoverHeight` 的消费一致性。远程图片 loading/failed 时不能因为没有图片自然尺寸而丢失封面高度。
- 默认 loading/error 视觉不能破坏自定义内容入口。用户设置 `LoadingContent` 或 `ErrorContent` 后，模板仍负责稳定尺寸、状态显隐和 mask 抑制。
- 默认失败文案必须走控件本地化资源，不允许在 `ImagePreviewerCoverTheme.axaml` 或 `ImageViewerTheme.axaml` 中硬编码英文字符串。
- `CurrentIndex` 的单一语义：dialog 和 overlay 使用同一 current effective item；普通封面只使用 `CoverIndex`。点击封面、封面加载状态和封面错误状态不得修改 `CurrentIndex`。
- `CoverIndex` 的单一语义：它只选择关闭态封面图片，不参与打开、导航、标题解析、fallback 批次判定或 selection 同步。
- `MaxConcurrentLoads` 的单一闸门语义：任何新增加载入口都必须接入同一调度器，不能形成第二套并发队列。
- `PreloadCount` 的窗口语义：只影响打开态邻近加载范围，不改变 public selection、封面索引或 `Count`。
- 预览标题的单一算法：非空白 `Window.Title` 或 `PreviewTitle` 优先，resolver 只在显式标题为空时运行；标题必须跟随 current effective item，不能保留旧集合项的文件名。
- 预览标题图标的单一路径：`PreviewTitleIcon` 只能进入 `ImagePreviewerDialog.TitleIcon`，再绑定到 `ImagePreviewerTitleBar.Icon` 和 `PART_IconPresenter`；不得转接 `Window.Icon`、`Window.Logo` 或右侧扩展区域来表达标题图标。
- 加载完成后必须重新计算 `ImageViewer` 的布局、居中、fit-to-window 和交互边界。
- Light/Dark、Browser/Desktop 和不同 SizeType 下的主题一致性。
- 控件文档、源码 public surface、Token 类型或生成数据与源码契约的一致性。

## 11. 测试与验证

推荐验证：

- 纯文档改动运行 `git diff --check` 并检查相对链接。
- 控件 API 或行为变更运行对应 `tests/AtomUI.Desktop.Controls.Tests` 或专用包测试。
- 图片加载模型变更覆盖 `IImagePreviewSource` 懒打开、`ImageSourceUri` 解析、本地文件、`avares://`、fake HTTP、stream source、取消、防旧结果回写、fallback 和 loading/error 模板状态。
- 懒加载模型变更覆盖：关闭态只加载 `CoverIndex`，打开态加载 `CurrentIndex`、`CoverIndex` 和 `PreloadCount` 邻近项，`MaxConcurrentLoads` 限制所有加载入口，快速导航取消旧预加载窗口，关闭预览宿主释放未完成预加载。
- 封面语义变更覆盖：`CoverIndex` 可指定关闭态封面，点击封面不改变 `CurrentIndex`，`CurrentIndex` 导航不反向改变 `CoverIndex`，两者越界 clamp 不写回 public 属性。
- loading/error 默认视觉变更覆盖：单图和多图封面均消费 `CoverWidth` / `CoverHeight`，封面默认 loading 使用 `SkeletonImage`，封面和预览层默认失败文案来自 `ImagePreviewerLangResource`，主题中不再出现硬编码 `Image load failed`。
- 预览标题模型变更覆盖显式标题优先级、默认文件名解析、current item 切换、集合替换、越界 `CurrentIndex` clamp、resolver 更换、`PreviewTitleIcon` 到 `TitleIcon` 的转接、未设置图标时不显示 fallback 图标，以及标题栏 `PART_IconPresenter` 布局。
- DataGrid 相关变更运行 `tests/AtomUI.Desktop.Controls.DataGrid.Tests`。
- Gallery 示例或源码片段变更运行 `tests/AtomUIGallery.Tests`。
- AOT、生成器或动态数据路径变更按 Gallery NativeAOT 发布流程验证。
