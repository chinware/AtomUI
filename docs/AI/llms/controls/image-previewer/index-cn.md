# ImagePreviewer

> 生成产物：由源文档生成，不要手工编辑。修改内容请回到控件文档、Gallery API / Token 表、Gallery ShowCase 或源码结构。

## 概述

ImagePreviewer 是 AtomUI 桌面控件体系中的图片预览控件，用于以统一图片来源 URI 查看、缩放、旋转、切换和窗口化预览图片。

ImagePreviewer 不负责图片编辑器、文件上传控件或媒体资源管理系统。这些职责应由业务层、组合控件或更专用的 AtomUI 控件承担。

主要源码入口：

- `src/AtomUI.Desktop.Controls/ImagePreviewer`

## 包与命名空间

| 项 | 值 |
| --- | --- |
| NuGet 包 | `AtomUI.Desktop.Controls` |
| .NET 命名空间 | `AtomUI.Desktop.Controls` |
| AXAML 命名空间 | `https://atomui.net` |
| Gallery 页面 | `controlgallery/AtomUIGallery/ShowCases/DataDisplay/ImagePreviewer` |
| 状态 | Stable |

## 何时使用

ImagePreviewer 的设计语言围绕控件职责、可观察状态和主题契约组织，而不是围绕模板节点组织。

| 维度 | 含义 | ImagePreviewer 中的表达 |
| --- | --- | --- |
| 产品语义 | 控件在界面中承担的稳定职责。 | ImagePreviewer 是 AtomUI 桌面控件体系中的图片预览控件，用于查看、缩放、旋转、切换和窗口化预览图片。 |
| 内容承载 | 用户数据、展示内容、集合项或操作入口如何进入控件。 | `Source`、`Sources`、`FallbackSource`、`CoverIndex`、`CoverIndicatorContent`、`CoverIndicatorContentTemplate`、`ImageMaxScale`、`ImageMinScale`、`ImageScaleStep`。 |
| 状态反馈 | public API、内部状态和伪类如何形成用户可感知反馈。 | current item、open/close、image loading、loaded/failed、fallback、motion。 |
| 主题语义 | ControlTheme、SharedToken、组件 Token 和模板绑定如何表达视觉。 | ImagePreviewer Token + ControlTheme。 |

## 公共 API

ImagePreviewer 的公共契约由 public/protected 类型成员、Avalonia 属性、事件、命令、template part、伪类、ControlTheme key 和资源 key 共同组成。维护时应先确认这些契约是否已经被源码、Gallery 示例或文档暴露。

核心 public surface 按语义分组维护：

| 契约组 | 代表成员 | 维护含义 |
| --- | --- | --- |
| 图片来源 | `Source`、`Sources`、`FallbackSource`、`IImagePreviewSource` | 统一表达单图、多图和失败兜底图片来源。`IImagePreviewSource` 是唯一来源契约，URI 场景通过 `UriImagePreviewSource` 显式进入来源集合，数据流场景通过 `StreamImagePreviewSource` 按需打开。 |
| 内容与数据 | `CoverIndicatorContent`、`CoverIndicatorContentTemplate`、`LoadingContent`、`LoadingContentTemplate`、`ErrorContent`、`ErrorContentTemplate`、`ImageMaxScale`、`ImageMinScale`、`ImageScaleStep`、`ImageTranslateX`、`ImageTranslateY` | 定义控件展示内容、输入数据、模板或业务对象入口。 |
| 选择与集合 | `Count`、`CurrentIndex` | 维护当前预览项、多图切换和集合状态；`CurrentIndex` 是控件级当前项索引，默认双向绑定，只决定打开预览后的当前图片。 |
| 封面展示 | `CoverIndex` | 只决定关闭态 `ImagePreviewer` 封面显示哪一张来源图片。它不参与打开行为、导航行为或 `CurrentIndex` 同步。 |
| 加载调度 | `MaxConcurrentLoads`、`PreloadCount` | 控制图片加载并发和打开预览后的邻近图片预加载窗口，避免大集合一次性加载全部图片。 |
| 预览标题 | `PreviewTitle`、`PreviewTitleIcon`、`PreviewTitleResolver`、`IImagePreviewTitleResolver`、`ImagePreviewTitleResolveContext` | 定义预览宿主标题和标题图标契约。显式标题非空时优先显示；显式标题为空时由 resolver 基于 current effective item 解析标题；`PreviewTitleIcon` 使用 `PathIcon?`，只在显式设置时显示，不继承应用或主窗口图标。 |
| 交互与状态 | `IsDialogModal`、`IsDialogTopmost`、`IsModal`、`IsMotionEnabled`、`IsOpen`、`IsShowCoverMask` | 表达用户可观察状态、可用性、清除、加载或反馈语义；`IsOpen` 默认双向绑定。 |
| 视觉与布局 | `CoverHeight`、`CoverWidth` | 影响尺寸、位置、颜色、形状、密度和模板视觉变量。 |
| 其他稳定入口 | `MaxScale`、`MinScale`、`ScaleStep`、`Stretch`、`Transform` | 保留为 public surface，变更前需确认 Gallery 和用户 XAML 依赖。 |

稳定事件包括 `FitToWindowRequest`、`HorizontalFlipRequest`、`NextRequest`、`PreviousRequest`、`RotateLeftRequest`、`RotateRightRequest`、`ScaleDownRequest`、`ScaleUpRequest`、`VerticalFlipRequest`。事件触发顺序属于兼容契约，不能因内部状态重排而改变。

受控状态契约：

- `IsOpen` 与 `CurrentIndex` 是用户可拥有的受控状态，Avalonia Binding 默认使用 `TwoWay`；预览关闭、图片切换、dialog 导航和 overlay 导航都必须回写同一 public 属性。
- `CoverIndex` 是封面展示状态，不是选择状态。点击封面只打开预览，不把 `CurrentIndex` 改成 `CoverIndex`。
- `IsOpen`、`CurrentIndex` 和 `CoverIndex` 都不是 Form value，不写入 `DataValidationErrors`；验证语义仍由图片来源、加载状态或业务 ViewModel 自行表达。

主要公开类型与枚举：

- 类型：`AbstractImagePreviewer`、`IImagePreviewSource`、`IImagePreviewSourceIdentity`、`UriImagePreviewSource`、`StreamImagePreviewSource`、`ImageSourceUri`、`ImageFitToWindowEventArgs`、`ImageGroupPreviewer`、`ImagePreviewBaseToolbar`、`ImagePreviewFloatToolbar`、`ImagePreviewNavButton`、`ImagePreviewRenderer`、`ImagePreviewToolbar`、`ImagePreviewToolbarRequestEventArgs`、`ImagePreviewer`、`ImagePreviewerCover`、`ImagePreviewerDialog`、`ImagePreviewerOverlayHost`、`ImagePreviewerTitleBar`、`IImagePreviewTitleResolver`、`ImagePreviewTitleResolveContext` 等。
- 枚举：`ImagePreviewItemState`。

稳定 template part：

| Template Part | 类型 | 职责 |
| --- | --- | --- |
| `PART_CloseButton` | `IconButton` | 承载预览宿主关闭动作。 |
| `PART_CoverItemsControl` | `ItemsControl` | 承载多图封面项。 |
| `PART_FitToWindowButton` | `ToggleIconButton` | 承载 fit-to-window 切换动作。 |
| `PART_HorizontalFlipButton` | `IconButton` | 承载水平翻转动作。 |
| `PART_ImageRenderer` | `ImagePreviewRenderer` | 承载当前图片渲染。 |
| `PART_ImageViewerScene` | `Canvas` | 承载预览层图片场景、变换和拖拽坐标空间。 |
| `PART_LoadingPresenter` | `Border` | 承载图片加载状态内容。默认封面使用图片 Skeleton 占位，预览层使用居中 Spin；自定义 `LoadingContent` 只替换内容，不改变加载状态 owner。 |
| `PART_ErrorPresenter` | `Border` | 承载图片加载失败内容。没有可用 fallback 时展示本地化失败占位；自定义 `ErrorContent` 只替换内容，不改变失败状态 owner。 |
| `PART_IconPresenter` | `IconPresenter` | 承载预览窗口标题图标，内容来自 `ImagePreviewer.PreviewTitleIcon`。 |
| `PART_TitleLayout` | `StackPanel` | 承载预览窗口标题图标和标题文字，两者使用标题栏 Logo 与 Title 间距。 |
| `PART_NextButton` | `ImagePreviewNavButton` / `IconButton` | 承载下一张导航动作。 |
| `PART_PreviousButton` | `ImagePreviewNavButton` / `IconButton` | 承载上一张导航动作。 |
| `PART_RotateLeftButton` | `IconButton` | 承载向左旋转动作。 |
| `PART_RotateRightButton` | `IconButton` | 承载向右旋转动作。 |
| `PART_ScaleDownButton` | `IconButton` | 承载缩小动作。 |
| `PART_ScaleUpButton` | `IconButton` | 承载放大动作。 |
| `PART_VerticalFlipButton` | `IconButton` | 承载垂直翻转动作。 |

当前未抽取到控件专属伪类；主题主要依赖 Avalonia 标准伪类、模板绑定和内部 StyledProperty。

## 事件与命令

ImagePreviewer 的公共契约由 public/protected 类型成员、Avalonia 属性、事件、命令、template part、伪类、ControlTheme key 和资源 key 共同组成。维护时应先确认这些契约是否已经被源码、Gallery 示例或文档暴露。
稳定事件包括 `FitToWindowRequest`、`HorizontalFlipRequest`、`NextRequest`、`PreviousRequest`、`RotateLeftRequest`、`RotateRightRequest`、`ScaleDownRequest`、`ScaleUpRequest`、`VerticalFlipRequest`。事件触发顺序属于兼容契约，不能因内部状态重排而改变。
- 类型：`AbstractImagePreviewer`、`IImagePreviewSource`、`IImagePreviewSourceIdentity`、`UriImagePreviewSource`、`StreamImagePreviewSource`、`ImageSourceUri`、`ImageFitToWindowEventArgs`、`ImageGroupPreviewer`、`ImagePreviewBaseToolbar`、`ImagePreviewFloatToolbar`、`ImagePreviewNavButton`、`ImagePreviewRenderer`、`ImagePreviewToolbar`、`ImagePreviewToolbarRequestEventArgs`、`ImagePreviewer`、`ImagePreviewerCover`、`ImagePreviewerDialog`、`ImagePreviewerOverlayHost`、`ImagePreviewerTitleBar`、`IImagePreviewTitleResolver`、`ImagePreviewTitleResolveContext` 等。

## 使用示例

稳定示例来源于 Gallery ShowCase 和源码查看片段。生成器只输出可从 `ShowCaseItem` 追溯的示例，不维护第二套手写示例。

以下示例来自 Gallery 源码查看使用的 `ShowCaseItem` 片段，并已按中文资源规范化。

### 基础用法

来源：`controlgallery/AtomUIGallery/ShowCases/DataDisplay/ImagePreviewer/Views/ImagePreviewerShowCase.axaml:36`

Gallery key：`ExamplesContent` / item `0`

```axaml
<atom:ImagePreviewer Width="200"
```

### 远程图片加载

来源：`controlgallery/AtomUIGallery/ShowCases/DataDisplay/ImagePreviewer/Views/ImagePreviewerShowCase.axaml:48`

Gallery key：`ExamplesContent` / item `1`

```axaml
<StackPanel Orientation="Horizontal"
            Spacing="{atom:SharedTokenResource SpacingLG}">
    <atom:ImagePreviewer Width="200"
                         Source="{Binding RemoteImage}" />
    <Border Width="200"
            Height="200"
            Background="{atom:SharedTokenResource ColorFillTertiary}">
        <Panel HorizontalAlignment="Center"
               VerticalAlignment="Center">
            <atom:SkeletonImage IsActive="True" />
        </Panel>
    </Border>
</StackPanel>
```

### 容错

来源：`controlgallery/AtomUIGallery/ShowCases/DataDisplay/ImagePreviewer/Views/ImagePreviewerShowCase.axaml:71`

Gallery key：`ExamplesContent` / item `2`

```axaml
<atom:ImagePreviewer Width="200"
```

### 20 张远程图片

来源：`controlgallery/AtomUIGallery/ShowCases/DataDisplay/ImagePreviewer/Views/ImagePreviewerShowCase.axaml:83`

Gallery key：`ExamplesContent` / item `3`

```axaml
<atom:ImagePreviewer Width="200"
```

## 状态模型

ImagePreviewer 的状态流按以下路径收敛：

```text
Public API / IImagePreviewSource / ImageSourceUri / inherited command / user input
  -> 控件实例状态
  -> ImagePreviewItem state / effective state / pseudo-class / template property
  -> ControlTheme selector / loading presenter / error presenter / renderer
  -> Gallery 可观察行为
```

状态维护规则：

- Disabled 或不可交互状态优先屏蔽 pointer、keyboard、motion 和提交类反馈。
- `IImagePreviewSource` 是用户输入层，`ImagePreviewItem` 是控件内部图片项状态 owner，`LoadedImageSource` 是加载完成结果。三者不能混用职责。
- 图片项状态按 `Pending -> Loading -> Loaded/Failed` 收敛。单项加载失败只影响该项自身；`FallbackSource` 只在当前来源集合全部失败时作为整组兜底。
- current item、open/close、image loading、loaded/failed、fallback、motion 状态由控件实例或明确的数据 owner 推导，不能在 template part 之间双向竞争。
- `IsOpen` 与 `CurrentIndex` 是默认 `TwoWay` 的受控状态；dialog 和 overlay 只能消费或回写这条 public 状态路径，不能保留独立打开状态或当前项状态。
- `CurrentIndex` 是弹出 dialog 和 overlay host 共享的当前项状态。普通封面只消费 `CoverIndex`，不得反向改写 `CurrentIndex`。
- `CoverIndex` 从当前来源集合中选择关闭态封面。显示层可以对越界 `CoverIndex` 做有效范围 clamp 以稳定渲染，但不能静默修改用户设置的 public 属性值。
- 预览标题由单一 effective title 算法生成：非空白显式标题优先；显式标题为空时，使用 resolver 基于 current effective item 解析标题；解析不到标题时标题区域保持空态。
- 模板重套用时必须把 public API 对应状态回放到新的 part、伪类和主题变量。
- 集合、弹层、异步、动效或窗口相关状态必须能处理 reset、close、cancel、detach 和 owner 释放；过期异步加载结果不能回写新来源。
- 关闭态只加载封面所需图片；打开态加载当前图片、`PreloadCount` 定义的邻近图片，并保留或补加载 `CoverIndex` 对应封面，保证非模态预览切换时页面封面不消失。控件不得因为 `Sources` 包含大量来源而一次性加载全部图片。

## 主题与 Design Token

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

预览窗口标题栏使用 `ImagePreviewer.PreviewTitleIcon` 作为标题图标来源。`PreviewTitleIcon` 是 `PathIcon?` 契约，表示只属于 ImagePreviewer 预览窗口标题的显式图标；未设置时标题栏不显示图标，也不从 `Window.Icon`、`Window.Logo`、应用图标或主窗口图标回退。`ImagePreviewerTitleBarTheme` 将 `PART_IconPresenter` 和标题内容放入 `PART_TitleLayout`，图标位于标题左侧，二者之间使用 `WindowTitleBarToken.LogoAndTitleSpacing`。Windows 和 Linux 模板把系统 caption buttons 放在标题区域右侧之外；macOS 模板依赖 `Window.TitleBarOffsetMargin` 给左侧原生窗口按钮预留安全区，不能通过继承应用图标规避碰撞。

加载视觉遵循以下规则：

- 封面加载态使用图片 Skeleton 占位，保持封面尺寸稳定，不显示 hover mask。
- 预览层加载态使用居中 Spin，缩放、旋转、拖拽和 fit-to-window 在当前图片未加载完成前禁用。
- loading 视觉允许短暂延迟显示以避免本地文件或 `avares://` 资源快速完成造成闪烁；延迟只影响视觉，不影响 `ImagePreviewItemState`。
- `LoadingContent` / `LoadingContentTemplate` 替换默认加载内容，`ErrorContent` / `ErrorContentTemplate` 替换默认失败内容；替换内容不得重新定义 `Pending -> Loading -> Loaded/Failed` 状态机。
- 封面 loading 和 failed 状态必须使用稳定占位尺寸。尺寸解析优先使用显式 `CoverWidth` / `CoverHeight`，其次使用控件布局约束中的有效宽高，最后使用 `ImagePreviewerToken.CoverImageWidth` 作为兜底基准。没有图片自然尺寸时，失败态不能由错误文案撑开成窄条。
- 默认失败态使用图片失败占位视觉：图标、简短本地化文案和低干扰背景共同表达失败。失败文案来自 ImagePreviewer 控件语言资源，主题中不得硬编码英文 `Image load failed`。

主题维护规则：

- 不删除或重命名已经稳定的 ControlTheme key、template part、伪类和资源 key。
- 不把可由 AXAML 表达的模板状态迁移为 C# 动态创建视觉。
- 不把 hover、pressed、current item、loading、failed、fallback、popup open 等运行时状态写入 Token。
- Browser 或平台特化主题必须保持同一 API 的语义一致。

Token 来源：

ImagePreviewer Token 只表达组件级视觉变量，例如尺寸、间距、颜色、圆角、阴影、图标尺寸和弹层边界。Token 不承载运行时选择、展开、加载、错误、上传任务、过滤条件或业务状态。

当前 Token scope：

- `ImagePreviewerToken`，scope id 为 `ImagePreviewer`，源码位于 `src/AtomUI.Desktop.Controls/ImagePreviewer/ImagePreviewerToken.cs`。

## AOT 与裁剪注意事项

资源和 AOT 约束：

- 不通过运行时反射扫描 public API、Token 或 Gallery 表格数据。
- 不把可静态声明的模板结构迁移到 C# 动态创建。
- 异步加载、上传、弹层和窗口生命周期必须能取消或释放。
- 网络图片必须通过异步加载服务处理，不允许在 UI 线程同步等待网络 I/O。
- 本地、资源和远程 URI 图片共享同一 `ImageSourceUri.CacheKey` identity 规范化规则，用于去重、旧结果判定和扩展场景；非 URI 来源只有显式实现 `IImagePreviewSourceIdentity` 时才跨实例复用。
- 默认 loading/error 视觉必须保持 AXAML-first。封面 Skeleton、预览 Spin、失败图标和本地化文本应由模板和资源表达；除非需要计算稳定占位尺寸，否则不要用 C# 动态创建视觉节点。
- 默认失败文案属于 ImagePreviewer 控件语言资源，新增或调整文案时同步 `en_US`、`zh_CN`、`zh_TW` 语言提供器和生成语言资源，不在主题中写死英文。
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

## 源码索引

主要源码文件：

- `src/AtomUI.Desktop.Controls/ImagePreviewer`：控件主体、图片源模型、加载状态、预览宿主和 renderer 所在目录，代表文件包括 `AbstractImagePreviewer.cs`、`ImagePreviewer.cs`、`ImageViewer.cs`、`ImagePreviewRenderer.cs`、`IImagePreviewSource.cs`、`UriImagePreviewSource.cs`、`StreamImagePreviewSource.cs`、`ImageSourceUri.cs`、`ImagePreviewItem.cs`、`LoadedImageSource.cs` 和 `IImageSourceLoader.cs`。
- `src/AtomUI.Desktop.Controls/ImagePreviewer/Localization`：3 个文件，代表文件 `en_US.cs`、`zh_CN.cs`、`zh_TW.cs`。
- `src/AtomUI.Desktop.Controls/ImagePreviewer/Themes`：12 个文件，代表文件 `ImageGroupPreviewerTheme.axaml`、`ImagePreviewFloatToolbarTheme.axaml`、`ImagePreviewNavButtonTheme.axaml`、`ImagePreviewToolbarTheme.axaml`、`ImagePreviewerCoverTheme.axaml` 等。

职责边界：

- 控件主文件保留 public/protected API、Avalonia 属性注册、事件和主要生命周期入口。
- 图片源模型文件只表达 `IImagePreviewSource`、`IImagePreviewSourceIdentity`、`UriImagePreviewSource`、`StreamImagePreviewSource`、`ImageSourceUri`、`ImagePreviewItem`、`LoadedImageSource`、加载调度器和加载服务契约，不承载视觉模板逻辑。
- Theme 文件负责静态视觉结构、template part、selector 和资源绑定。
- Token 文件只提供组件视觉变量，不保存实例状态。
- Gallery 文件只展示用法、API 表和 Token 表，不作为运行时逻辑 owner。

## 相关文档

- 源设计文档：`docs/controls/desktop/data-display/image-previewer/overview.md`
- 实现文档：`docs/controls/desktop/data-display/image-previewer/implementation.md`
- Token 文档：`docs/controls/desktop/data-display/image-previewer/token.md`
- 变更记录：`docs/controls/desktop/data-display/image-previewer/changelog.md`
- 语义结构：`./semantic-cn.md`
