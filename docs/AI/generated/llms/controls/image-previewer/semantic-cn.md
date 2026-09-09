# ImagePreviewer 语义结构

> 生成产物：由源文档生成，不要手工编辑。修改内容请回到控件文档、源码 public surface、Token 类型或生成数据、Gallery ShowCase 或源码结构。

## Semantic Parts

ImagePreviewer 家族公开 8 个 Semantic Part，由两个 public owner 分别声明生成式 descriptor：`ImagePreviewer`（单封面入口）与
`ImageGroupPreviewer`（多封面入口）。`root` 由生成器隐式加入，不要求 `.semantic-root`。除 `root` 外每个 Part 生成 public
强类型 Semantic Style，命名规则为 `AtomUI.Theme.Styling.<Control><PartPath>Style`（如 `ImagePreviewerPopupRootStyle`），
用户在外层普通 `Style` 中按 owner 作用域嵌套使用。

8 个 Part 中 `root`/`image`/`cover` 对应 `.ant-image`/`.ant-image-img`/`.ant-image-cover`；`popup.root`/`popup.mask`/
`popup.body`/`popup.footer`/`popup.actions` 对应 `.ant-image-preview` 及其内部 `mask`（半透明遮罩层）、`body`（居中图片区）、
`footer`（底部操作区）、`actions`（footer 内操作按钮组）四个子节点。上游的右上角关闭按钮、左右切换按钮与页码指示不是语义
部件，AtomUI 同样不将其公开为 Part。

`popup.*` 属于独立宿主部件：预览宿主（native `ImagePreviewerDialog` 或 Browser `ImagePreviewerOverlayHost`）由
`OpenDialog()` 运行时创建，并经 logical parent 挂入 owner，宿主 ThemeVariant 经 binding 中继。owner 作用域 Semantic Style
只在 overlay 宿主（与 owner 同 TopLevel 的树内浮层，对齐上游 `.ant-image-preview`）保证命中；native `ImagePreviewerDialog`
是独立 `Window`/TopLevel，Avalonia 样式级联不跨 TopLevel 边界，因此 owner 作用域 Semantic Style 不进入 dialog，dialog 内的
预览视觉经 host 契约（owner 属性/Token 中继与 App 级 `ImageViewer` 主题）定制。`popup.*` 部件随宿主打开而存在、随关闭而销毁，
因此统一声明 `CrossVisualRoot=true`、`CrossNestedOwners=true`、`RuntimeCreated=true`，路由以 `>>` 从 owner 直接定位 marker
节点，不设中间 scope 锚点（overlay 宿主模板根与 viewer 在逻辑树上为兄弟，锚点式路由无法在双宿主间一致命中）。`popup.mask`
仅 Overlay 宿主存在（`Optional`），承担上游浮层"压暗下层页面"的职能；native `ImagePreviewerDialog` 是独立窗口、无下层页面
可压暗。关闭在 overlay 宿主由内嵌关闭按钮（`PART_CloseButton`，非语义部件）承担，native dialog 由 OS 标题栏按钮承担。Gallery Semantic Preview 中 `popup.*` 部件
需示例显式提供宿主 Visual 根作为 `AdditionalRoots` 才能解析；但宿主（`ImagePreviewerDialog` 与 `ImagePreviewerOverlayHost`）
均为 internal、`OpenDialog()` 不返回宿主、产品不暴露任何 Preview 专用 API，Gallery 示例无法取得该根，因此 `popup.*` 部件在
Gallery 仅列出描述、不参与高亮；触发区部件（`root`/`image`/`cover`）在 owner 模板内正常解析。所有 marker 使用静态
`Classes.semantic-*="True"` 声明，内置主题不使用 `.semantic-*` selector 实现默认视觉。

### `root`

| 字段 | 值 |
| --- | --- |
| Owner | `ImagePreviewer` / `ImageGroupPreviewer` |
| Part | `root` |
| Selector | 不适用（root 无 `.semantic-root`） |
| SelectorRoute | 不适用 |
| ContractType | `ImagePreviewer` / `ImageGroupPreviewer` |
| Cardinality | `Single` |
| Customization | `Root` |
| CrossVisualRoot | `false` |
| RuntimeCreated | `false` |
| AtomUI 节点 | 控件根（隐式） |
| 职责 | 单封面入口（`ImagePreviewer`）/ 多封面入口（`ImageGroupPreviewer`）与完整预览 owner |
| 相关 API | `ItemsSource`、`CurrentIndex`、`IsOpen`；group 额外 `ItemsPanel` |
| 相关 Token | SharedToken、ImagePreviewerToken |
| 稳定性 | stable since 6.0 |

### `image`

| 字段 | 值 |
| --- | --- |
| Owner | `ImagePreviewer` / `ImageGroupPreviewer` |
| Part | `image` |
| Selector | `.semantic-image` |
| SelectorRoute | `ImagePreviewer`：`/template/ .semantic-scope-cover /template/ .semantic-image`；`ImageGroupPreviewer`：`/template/ .semantic-scope-items >> .semantic-image` |
| ContractType | `Control` |
| Cardinality | `ImagePreviewer`：`Single`；`ImageGroupPreviewer`：`Multiple` |
| Customization | `Selector` |
| CrossVisualRoot | `false` |
| CrossNestedOwners | `true` |
| RuntimeCreated | `ImagePreviewer`：`false`；`ImageGroupPreviewer`：`true` |
| AtomUI 节点 | `ImagePreviewerCover` 模板内 `ImagePreviewRenderer`（单封面静态 / 多封面 ItemsControl 项运行时物化） |
| 职责 | 关闭态封面图片元素 / 各封面缩略图元素 |
| 相关 API | `EffectiveCoverImage`、`CoverWidth`、`CoverHeight`；group `ItemsSource`、`ItemsPanel` |
| 相关 Token | Cover 尺寸相关 Token |
| 稳定性 | stable since 6.0 |

### `cover`

| 字段 | 值 |
| --- | --- |
| Owner | `ImagePreviewer` / `ImageGroupPreviewer` |
| Part | `cover` |
| Selector | `.semantic-cover` |
| SelectorRoute | `ImagePreviewer`：`/template/ .semantic-scope-cover /template/ .semantic-cover`；`ImageGroupPreviewer`：`/template/ .semantic-scope-items >> .semantic-cover` |
| ContractType | `Border` |
| Cardinality | `ImagePreviewer`：`Single`；`ImageGroupPreviewer`：`Multiple` |
| Customization | `Selector` |
| CrossVisualRoot | `false` |
| CrossNestedOwners | `true` |
| RuntimeCreated | `ImagePreviewer`：`false`；`ImageGroupPreviewer`：`true` |
| AtomUI 节点 | `ImagePreviewerCover` 模板内 `#Mask`（单封面静态 / 多封面 ItemsControl 项运行时物化） |
| 职责 | 封面悬浮提示层：遮罩 + 指示内容。遮罩经负 Margin 铺满整个 owner root（含 padding 环与边框），对齐上游 `genImageCoverStyle` 的 `position:absolute; inset:0` cover 几何 |
| 相关 API | `IsShowCoverMask`、`CoverIndicatorContent(Template)`、owner `Padding` / `BorderThickness`（经中继参与遮罩几何） |
| 相关 Token | `MaskBgColor`、mask 透明度与圆角 Token |
| 稳定性 | stable since 6.0 |

### `popup.root`

| 字段 | 值 |
| --- | --- |
| Owner | `ImagePreviewer` / `ImageGroupPreviewer` |
| Part | `popup.root` |
| Selector | `.semantic-popup-root` |
| SelectorRoute | `>> .semantic-popup-root` |
| ContractType | `Panel` |
| Cardinality | `Single` |
| Customization | `Selector` |
| CrossVisualRoot | `true` |
| CrossNestedOwners | `true` |
| RuntimeCreated | `true` |
| AtomUI 节点 | native dialog 内容根包裹 Panel（代码创建并注入 marker）；overlay 宿主模板根 Panel（静态 marker、纯容器，不带背景） |
| 职责 | 预览容器根：承载遮罩层、内容区与关闭按钮的根层（对齐上游 `.ant-image-preview`）；窗口 chrome 不属于契约 |
| 相关 API | `IsOpen`、`OpenDialog()`、`IsDialogModal`、`IsDialogTopmost` |
| 相关 Token | Dialog 背景 Token |
| 稳定性 | stable since 6.0 |

### `popup.mask`

| 字段 | 值 |
| --- | --- |
| Owner | `ImagePreviewer` / `ImageGroupPreviewer` |
| Part | `popup.mask` |
| Selector | `.semantic-popup-mask` |
| SelectorRoute | `>> .semantic-popup-mask` |
| ContractType | `Panel` |
| Cardinality | `Optional` |
| Customization | `Selector` |
| CrossVisualRoot | `true` |
| CrossNestedOwners | `true` |
| RuntimeCreated | `true` |
| AtomUI 节点 | Overlay 宿主模板根 Panel 内新增的全铺遮罩子元素（静态 marker；半透明黑背景，由原宿主根 Panel 背景迁移而来） |
| 职责 | 预览遮罩层：全铺 `popup.root` 的半透明暗色背景，位于 `popup.body` 之下（对齐上游 `.ant-image-preview-mask`）；仅 Overlay 宿主存在 |
| 相关 API | `IsOpen`（随 overlay 宿主打开出现；点击关闭行为当前未实现，见兼容性与验证） |
| 相关 Token | 遮罩色使用共享 `ColorBgMask`，与上游 `.ant-image-preview-mask` 的 `colorBgMask` 语义一致 |
| 稳定性 | stable since 6.0 |

### `popup.body`

| 字段 | 值 |
| --- | --- |
| Owner | `ImagePreviewer` / `ImageGroupPreviewer` |
| Part | `popup.body` |
| Selector | `.semantic-popup-body` |
| SelectorRoute | `>> .semantic-popup-body` |
| ContractType | `Panel` |
| Cardinality | `Single` |
| Customization | `Selector` |
| CrossVisualRoot | `true` |
| CrossNestedOwners | `true` |
| RuntimeCreated | `true` |
| AtomUI 节点 | `ImageViewer` 模板内 `PART_ImageViewerScene` Canvas |
| 职责 | 预览内容区：居中承载图片渲染与指针交互（对齐上游 `.ant-image-preview-body`） |
| 相关 API | 缩放、拖拽、旋转与 fit-to-window 交互 API |
| 相关 Token | 无独立 Token（沿用 viewer 背景与交互 Token） |
| 稳定性 | stable since 6.0 |

### `popup.footer`

| 字段 | 值 |
| --- | --- |
| Owner | `ImagePreviewer` / `ImageGroupPreviewer` |
| Part | `popup.footer` |
| Selector | `.semantic-popup-footer` |
| SelectorRoute | `>> .semantic-popup-footer` |
| ContractType | `Control` |
| Cardinality | `Single` |
| Customization | `Selector` |
| CrossVisualRoot | `true` |
| CrossNestedOwners | `true` |
| RuntimeCreated | `true` |
| AtomUI 节点 | `ImageViewer` 模板内 `ImagePreviewFloatToolbar` 节点 |
| 职责 | 预览页脚：底部居中操作区域，含页码指示与操作组（对齐上游 `.ant-image-preview-footer`） |
| 相关 API | `CurrentIndex`、Count 与 scale/fit 状态投影 |
| 相关 Token | `FloatToolbarPadding`、`NavButtonBgColor` |
| 稳定性 | stable since 6.0 |

### `popup.actions`

| 字段 | 值 |
| --- | --- |
| Owner | `ImagePreviewer` / `ImageGroupPreviewer` |
| Part | `popup.actions` |
| Selector | `.semantic-popup-actions` |
| SelectorRoute | `>> .semantic-popup-footer /template/ .semantic-popup-actions` |
| ContractType | `Border` |
| Cardinality | `Single` |
| Customization | `Selector` |
| CrossVisualRoot | `true` |
| CrossNestedOwners | `true` |
| RuntimeCreated | `true` |
| AtomUI 节点 | `ImagePreviewFloatToolbarTheme` 内 `#ActionFrame` |
| 职责 | 预览操作组：footer 内的胶囊形操作按钮组（对齐上游 `.ant-image-preview-actions`） |
| 相关 API | 缩放、翻转、旋转与 fit-to-window 命令 |
| 相关 Token | `PreviewOperationSize`、`PreviewOperationColor` |
| 稳定性 | stable since 6.0 |

## Abstract AXAML Structure

来源：`src/AtomUI.Desktop.Controls/ImagePreviewer/Themes/ImagePreviewerTheme.axaml`

```xml
<PixelAlignedBorder>
    <ImagePreviewerCover />
</PixelAlignedBorder>
```

## Composition Model

该章节由控件 `Themes/` 文件夹中的真实主题文件生成，用于说明 public 控件与内部协作对象之间的运行时结构。内部节点只用于理解和维护，不应指导用户代码直接依赖。

### 控件角色图

```text
ImagePreviewer
  -> ImagePreviewFloatToolbar (control theme, ImagePreviewFloatToolbarTheme.axaml)
     -> StackPanel#RootLayout (template-stable)
        -> Border#IndicatorFrame (template-stable)
           -> TextBlock (template-stable)
        -> Border#ActionFrame (template-stable)
           -> StackPanel (template-stable)
              -> IconButton#PART_ScaleDownButton (template-stable)
              -> IconButton#PART_ScaleUpButton (template-stable)
              -> ToggleIconButton#PART_FitToWindowButton (template-stable)
              -> IconButton#PART_HorizontalFlipButton (template-stable)
              -> IconButton#PART_VerticalFlipButton (template-stable)
              -> IconButton#PART_RotateLeftButton (template-stable)
              -> IconButton#PART_RotateRightButton (template-stable)
  -> ImagePreviewNavButton (control theme, ImagePreviewNavButtonTheme.axaml)
  -> ImagePreviewToolbar (control theme, ImagePreviewToolbarTheme.axaml)
     -> StackPanel#RootLayout (template-stable)
        -> IconButton#PART_PreviousButton (template-stable)
        -> IconButton#PART_NextButton (template-stable)
        -> IconButton#PART_ScaleDownButton (template-stable)
        -> IconButton#PART_ScaleUpButton (template-stable)
        -> ToggleIconButton#PART_FitToWindowButton (template-stable)
        -> IconButton#PART_HorizontalFlipButton (template-stable)
        -> IconButton#PART_VerticalFlipButton (template-stable)
        -> IconButton#PART_RotateLeftButton (template-stable)
        -> IconButton#PART_RotateRightButton (template-stable)
  -> ImagePreviewerCover (control theme, ImagePreviewerCoverTheme.axaml)
     -> PixelAlignedBorder (template-stable)
        -> Panel (template-stable)
           -> ImagePreviewRenderer (internal-observable)
           -> Border#PART_LoadingPresenter (template-stable)
              -> Panel (template-stable)
                 -> SkeletonImage#PART_LoadingSkeleton (template-stable)
                 -> ContentPresenter (internal-observable)
           -> Border#PART_ErrorPresenter (template-stable)
              -> Panel (template-stable)
                 -> StackPanel#DefaultErrorLayout (template-stable)
                    -> PictureOutlined#DefaultErrorIcon (template-stable)
                    -> TextBlock#DefaultErrorText (template-stable)
                 -> ContentPresenter (internal-observable)
           -> Border#Mask (template-stable)
              -> ContentPresenter#MaskContentPresenter (internal-observable)
  -> ImagePreviewerDialog (control theme, ImagePreviewerDialogTheme.axaml)
  -> ImagePreviewerOverlayHost (control theme, ImagePreviewerOverlayHostTheme.axaml)
     -> Panel (template-stable)
        -> Panel (template-stable)
        -> ContentPresenter (internal-observable)
        -> IconButton#PART_CloseButton (template-stable)
  -> ImagePreviewer (control theme, ImagePreviewerTheme.axaml)
     -> PixelAlignedBorder (template-stable)
        -> ImagePreviewerCover (internal-observable)
  -> ImagePreviewerTitleBar (control theme, ImagePreviewerTitleBarTheme.axaml)
     -> Border#Frame (template-stable)
        -> WindowTitleBarLayoutPanel (template-stable)
           -> ContentPresenter#PART_LeftAddOn (template-stable)
           -> DockPanel#PART_TitleLayout (template-stable)
              -> IconPresenter#PART_IconPresenter (template-stable)
              -> ContentPresenter#PART_ContentPresenter (template-stable)
           -> StackPanel (template-stable)
              -> ContentPresenter#PART_RightAddOn (template-stable)
              -> CaptionButtonGroup#PART_CaptionButtonGroup (template-stable)
     -> Border#Frame (template-stable)
        -> WindowTitleBarLayoutPanel (template-stable)
           -> ContentPresenter#PART_LeftAddOn (template-stable)
           -> DockPanel#PART_TitleLayout (template-stable)
              -> IconPresenter#PART_IconPresenter (template-stable)
              -> ContentPresenter#PART_ContentPresenter (template-stable)
           -> StackPanel (template-stable)
              -> ContentPresenter#PART_RightAddOn (template-stable)
              -> CaptionButtonGroup#PART_CaptionButtonGroup (template-stable)
     -> Border#Frame (template-stable)
        -> WindowTitleBarLayoutPanel (template-stable)
           -> ContentPresenter#PART_LeftAddOn (template-stable)
           -> DockPanel#PART_TitleLayout (template-stable)
              -> IconPresenter#PART_IconPresenter (template-stable)
              -> ContentPresenter#PART_ContentPresenter (template-stable)
           -> StackPanel (template-stable)
              -> ContentPresenter#PART_RightAddOn (template-stable)
              -> CaptionButtonGroup#PART_CaptionButtonGroup (template-stable)
  -> ImageViewer (control theme, ImageViewerTheme.axaml)
     -> Panel (template-stable)
        -> Canvas#PART_ImageViewerScene (template-stable)
           -> ImagePreviewRenderer#PART_ImageRenderer (template-stable)
        -> Border#PART_LoadingPresenter (template-stable)
           -> Panel (template-stable)
              -> Spin (template-stable)
              -> ContentPresenter (internal-observable)
        -> Border#PART_ErrorPresenter (template-stable)
           -> Panel (template-stable)
              -> StackPanel#DefaultErrorLayout (template-stable)
                 -> PictureOutlined#DefaultErrorIcon (template-stable)
                 -> TextBlock#DefaultErrorText (template-stable)
              -> ContentPresenter (internal-observable)
        -> ImagePreviewNavButton#PART_PreviousButton (template-stable)
        -> ImagePreviewNavButton#PART_NextButton (template-stable)
        -> ImagePreviewFloatToolbar (internal-observable)
        -> MediaBreakPointIndicator#{x:Static atom:MediaBreakPointIndicator.MediaQueryIndicatorName} (internal-observable)
```

### 协作节点

| 节点 | 类型 | 来源 | 生命周期 owner | 影响的 public API | 稳定性 | Agent 使用边界 |
| --- | --- | --- | --- | --- | --- | --- |
| `ImagePreviewer` | public control | `源文档 + public API` | 用户代码 / 控件宿主 | public API | public | 用户可直接使用 public 控件；可作为示例和 API 入口。 |
| `ImagePreviewFloatToolbar` | control theme | `ImagePreviewFloatToolbarTheme.axaml` | ImagePreviewer | `Background`, `CornerRadius`, `IndicatorText`, `IsImageFitToWindow`, `IsMultiImages`, `IsScaleDownEnabled` | internal-observable | 用于理解结构和状态流，不应指导用户代码直接依赖。 |
| `RootLayout` | template node (StackPanel) | `ImagePreviewFloatToolbarTheme.axaml` | ImagePreviewFloatToolbar | `Background`, `CornerRadius`, `IndicatorText`, `IsImageFitToWindow`, `IsMultiImages`, `IsScaleDownEnabled` | template-stable | 用于主题维护；变更需同步主题、实现和 LLMS。 |
| `IndicatorFrame` | template node (Border) | `ImagePreviewFloatToolbarTheme.axaml` | ImagePreviewFloatToolbar | `Background`, `CornerRadius`, `IndicatorText`, `IsMultiImages` | template-stable | 用于主题维护；变更需同步主题、实现和 LLMS。 |
| `ActionFrame` | template node (Border) | `ImagePreviewFloatToolbarTheme.axaml` | ImagePreviewFloatToolbar | `Background`, `CornerRadius`, `IsImageFitToWindow`, `IsScaleDownEnabled`, `IsScaleUpEnabled`, `Padding` | template-stable | 用于主题维护；变更需同步主题、实现和 LLMS。 |
| `StackPanel` | template node (StackPanel) | `ImagePreviewFloatToolbarTheme.axaml` | ImagePreviewFloatToolbar | `IsImageFitToWindow`, `IsScaleDownEnabled`, `IsScaleUpEnabled` | template-stable | 用于主题维护；变更需同步主题、实现和 LLMS。 |
| `PART_ScaleDownButton` | template node (IconButton) | `ImagePreviewFloatToolbarTheme.axaml` | ImagePreviewFloatToolbar | `IsScaleDownEnabled` | template-stable | 用于主题维护；变更需同步主题、实现和 LLMS。 |
| `PART_ScaleUpButton` | template node (IconButton) | `ImagePreviewFloatToolbarTheme.axaml` | ImagePreviewFloatToolbar | `IsScaleUpEnabled` | template-stable | 用于主题维护；变更需同步主题、实现和 LLMS。 |
| `PART_FitToWindowButton` | template node (ToggleIconButton) | `ImagePreviewFloatToolbarTheme.axaml` | ImagePreviewFloatToolbar | `IsImageFitToWindow` | template-stable | 用于主题维护；变更需同步主题、实现和 LLMS。 |
| `PART_HorizontalFlipButton` | template node (IconButton) | `ImagePreviewFloatToolbarTheme.axaml` | ImagePreviewFloatToolbar | 主题状态 / visual state | template-stable | 用于主题维护；变更需同步主题、实现和 LLMS。 |
| `PART_VerticalFlipButton` | template node (IconButton) | `ImagePreviewFloatToolbarTheme.axaml` | ImagePreviewFloatToolbar | 主题状态 / visual state | template-stable | 用于主题维护；变更需同步主题、实现和 LLMS。 |
| `PART_RotateLeftButton` | template node (IconButton) | `ImagePreviewFloatToolbarTheme.axaml` | ImagePreviewFloatToolbar | 主题状态 / visual state | template-stable | 用于主题维护；变更需同步主题、实现和 LLMS。 |
| `PART_RotateRightButton` | template node (IconButton) | `ImagePreviewFloatToolbarTheme.axaml` | ImagePreviewFloatToolbar | 主题状态 / visual state | template-stable | 用于主题维护；变更需同步主题、实现和 LLMS。 |
| `ImagePreviewNavButton` | control theme | `ImagePreviewNavButtonTheme.axaml` | ImagePreviewer | 主题状态 / visual state | internal-observable | 用于理解结构和状态流，不应指导用户代码直接依赖。 |
| `ImagePreviewToolbar` | control theme | `ImagePreviewToolbarTheme.axaml` | ImagePreviewer | `IsFirstImage`, `IsImageFitToWindow`, `IsLastImage`, `IsMultiImages`, `IsScaleDownEnabled`, `IsScaleUpEnabled` | internal-observable | 用于理解结构和状态流，不应指导用户代码直接依赖。 |
| `RootLayout` | template node (StackPanel) | `ImagePreviewToolbarTheme.axaml` | ImagePreviewToolbar | `IsFirstImage`, `IsImageFitToWindow`, `IsLastImage`, `IsMultiImages`, `IsScaleDownEnabled`, `IsScaleUpEnabled` | template-stable | 用于主题维护；变更需同步主题、实现和 LLMS。 |
| `PART_PreviousButton` | template node (IconButton) | `ImagePreviewToolbarTheme.axaml` | ImagePreviewToolbar | `IsFirstImage`, `IsMultiImages` | template-stable | 用于主题维护；变更需同步主题、实现和 LLMS。 |
| `PART_NextButton` | template node (IconButton) | `ImagePreviewToolbarTheme.axaml` | ImagePreviewToolbar | `IsLastImage`, `IsMultiImages` | template-stable | 用于主题维护；变更需同步主题、实现和 LLMS。 |
| `PART_ScaleDownButton` | template node (IconButton) | `ImagePreviewToolbarTheme.axaml` | ImagePreviewToolbar | `IsScaleDownEnabled` | template-stable | 用于主题维护；变更需同步主题、实现和 LLMS。 |
| `PART_ScaleUpButton` | template node (IconButton) | `ImagePreviewToolbarTheme.axaml` | ImagePreviewToolbar | `IsScaleUpEnabled` | template-stable | 用于主题维护；变更需同步主题、实现和 LLMS。 |
| `PART_FitToWindowButton` | template node (ToggleIconButton) | `ImagePreviewToolbarTheme.axaml` | ImagePreviewToolbar | `IsImageFitToWindow` | template-stable | 用于主题维护；变更需同步主题、实现和 LLMS。 |
| `PART_HorizontalFlipButton` | template node (IconButton) | `ImagePreviewToolbarTheme.axaml` | ImagePreviewToolbar | 主题状态 / visual state | template-stable | 用于主题维护；变更需同步主题、实现和 LLMS。 |
| `PART_VerticalFlipButton` | template node (IconButton) | `ImagePreviewToolbarTheme.axaml` | ImagePreviewToolbar | 主题状态 / visual state | template-stable | 用于主题维护；变更需同步主题、实现和 LLMS。 |
| `PART_RotateLeftButton` | template node (IconButton) | `ImagePreviewToolbarTheme.axaml` | ImagePreviewToolbar | 主题状态 / visual state | template-stable | 用于主题维护；变更需同步主题、实现和 LLMS。 |
| `PART_RotateRightButton` | template node (IconButton) | `ImagePreviewToolbarTheme.axaml` | ImagePreviewToolbar | 主题状态 / visual state | template-stable | 用于主题维护；变更需同步主题、实现和 LLMS。 |
| `ImagePreviewerCover` | control theme | `ImagePreviewerCoverTheme.axaml` | ImagePreviewer | `Background`, `BorderBrush`, `BorderThickness`, `Content`, `ContentTemplate`, `ErrorContent` | internal-observable | 用于理解结构和状态流，不应指导用户代码直接依赖。 |
| `Panel` | template node (Panel) | `ImagePreviewerCoverTheme.axaml` | ImagePreviewerCover | `Content`, `ContentTemplate`, `ErrorContent`, `ErrorContentTemplate`, `HasError`, `ImageSource` | template-stable | 用于主题维护；变更需同步主题、实现和 LLMS。 |
| `PART_LoadingPresenter` | template node (Border) | `ImagePreviewerCoverTheme.axaml` | ImagePreviewerCover | `LoadingContent`, `LoadingContentTemplate`, `OwnerCornerRadius` | template-stable | 用于主题维护；变更需同步主题、实现和 LLMS。 |
| `PART_LoadingSkeleton` | template node (SkeletonImage) | `ImagePreviewerCoverTheme.axaml` | ImagePreviewerCover | `LoadingContent` | template-stable | 用于主题维护；变更需同步主题、实现和 LLMS。 |
| `ContentPresenter` | template node (ContentPresenter) | `ImagePreviewerCoverTheme.axaml` | ImagePreviewerCover | `LoadingContent`, `LoadingContentTemplate` | internal-observable | 用于理解结构和状态流，不应指导用户代码直接依赖。 |
| `PART_ErrorPresenter` | template node (Border) | `ImagePreviewerCoverTheme.axaml` | ImagePreviewerCover | `ErrorContent`, `ErrorContentTemplate`, `HasError`, `OwnerCornerRadius` | template-stable | 用于主题维护；变更需同步主题、实现和 LLMS。 |
| `DefaultErrorLayout` | template node (StackPanel) | `ImagePreviewerCoverTheme.axaml` | ImagePreviewerCover | `ErrorContent` | template-stable | 用于主题维护；变更需同步主题、实现和 LLMS。 |
| `DefaultErrorIcon` | template node (PictureOutlined) | `ImagePreviewerCoverTheme.axaml` | ImagePreviewerCover | 主题状态 / visual state | template-stable | 用于主题维护；变更需同步主题、实现和 LLMS。 |
| `DefaultErrorText` | template node (TextBlock) | `ImagePreviewerCoverTheme.axaml` | ImagePreviewerCover | 主题状态 / visual state | template-stable | 用于主题维护；变更需同步主题、实现和 LLMS。 |
| `Mask` | template node (Border) | `ImagePreviewerCoverTheme.axaml` | ImagePreviewerCover | `Content`, `ContentTemplate`, `IsCoverMaskVisible`, `MaskOpacity`, `OwnerCornerRadius` | template-stable | 用于主题维护；变更需同步主题、实现和 LLMS。 |
| `MaskContentPresenter` | template node (ContentPresenter) | `ImagePreviewerCoverTheme.axaml` | ImagePreviewerCover | `Content`, `ContentTemplate` | internal-observable | 用于理解结构和状态流，不应指导用户代码直接依赖。 |

## Template Parts

| Template Part | AtomUI 节点 | 职责 | 稳定性 |
| --- | --- | --- | --- |
| `PART_CoverItemsControl` | `ImageGroupPreviewerTheme` | group 封面集合 | template-stable |
| `PART_ImageViewerScene` | `ImageViewerTheme` | 预览图片坐标空间 | template-stable |
| `PART_ImageRenderer` | `ImageViewerTheme` | 只渲染 entry 已持有的 `IImage` | template-stable |
| `PART_LoadingPresenter` | `ImagePreviewerCoverTheme` / `ImageViewerTheme` | 封面 Skeleton / viewer Spin 状态占位 | template-stable |
| `PART_ErrorPresenter` | `ImagePreviewerCoverTheme` / `ImageViewerTheme` | 失败内容占位 | template-stable |
| `PART_PreviousButton` / `PART_NextButton` | `ImageViewerTheme` | 上一张 / 下一张导航 | template-stable |
| `PART_TitleLayout` / `PART_IconPresenter` | `ImagePreviewerTitleBarTheme` | dialog 标题与图标 | template-stable |
| `PART_CloseButton` | `ImagePreviewerOverlayHostTheme` | overlay 关闭入口 | template-stable |

## Pseudo Classes

- Template Part、伪类、loading/error 门控、标题、导航、缩放和 Token 视觉语义保持稳定。
- 不允许同步 I/O、固定延迟、反射发现 reader/codec/serializer，或在 Previewer 内创建 `HttpClient`、cache 或 scheduler。
- SourceSnapshot、encoded content 和 decoded content 的共享范围都受 CachePartition 限制；安全分区之间不共享命中或诊断。
- borrowed `IImage` 始终由调用方拥有，任何 loader/cache/control 生命周期都不能 dispose 它。
- Semantic Part：删除或重命名 Part、修改 SelectorRoute 命中范围、收窄 `ContractType` 都属于破坏性变更；`popup.*` 部件仅宿主
  打开期间存在，关闭后不残留任何 marker 节点；内置主题不使用 `.semantic-*` selector 实现默认视觉；`.semantic-scope-*`
  路由锚点不是公开 Part，不进入兼容承诺；native dialog 的窗口 chrome（标题栏、caption 按钮、窗口边框）不属于任何 Part；
  宿主必须保持挂入 owner 的 logical parent 链，popup 部件的生成 Selector 依赖该链命中 overlay 宿主（与 owner 同 TopLevel）；
  native dialog 是独立 TopLevel，owner 作用域样式不跨窗口级联，预览视觉经 host 契约定制。
- 宿主分层与上游 DOM 对齐：overlay 宿主模板根 Panel 只承担 `popup.root` 容器职责、不带背景，遮罩背景必须由独立的
  `popup.mask` 子元素承担；`popup.mask` 仅 Overlay 宿主存在（`Optional`），native dialog 不物化该部件。
- 遮罩点击关闭（上游 `maskClosable=true` 默认）当前未在 overlay 宿主实现，关闭经 overlay 宿主内嵌关闭按钮（非语义部件）；Part 契约只承诺样式命中，

## State Flow

### 4.1 内容身份与来源验证

内部身份链固定为：

```text
ImageSourceKey --validate--> ImageSourceVersion --maps-to--> ImageContentId
ImageContentId + ImageDecodeSpec ---------------------------> ImageDecodeKey
```

| 术语 | 定义 | 是否包含解码尺寸 |
| --- | --- | --- |
| `ImageSourceKey` | 规范来源地址或 identity、representation header 摘要、Variant、partition 摘要和 reader contract | 否 |
| `ImageSourceVersion` | 某次来源验证观察到的版本令牌 | 否 |
| `ImageContentId` | 对读取到的精确编码字节计算的 SHA-256 内容身份 | 否 |
| `ImageDecodeSpec` | codec、安全策略、目标物理像素、颜色和方向等影响输出的参数 | 按 codec 决定 |
| `ImageDecodeKey` | partition、ContentId 与 DecodeSpec 组成的解码身份 | 按 codec 决定 |
| `ImageSourceSnapshot` | SourceKey、SourceVersion、ContentId、freshness/validator 和提交 generation 的不可变映射 | 否 |

`Timeout`、Priority、progress callback、CacheRead 和 CacheStorage 不进入内容身份。它们只控制单个 waiter 的执行方式。

来源验证策略：

| Source | SourceVersion 或验证契约 | 跨请求来源映射 |
| --- | --- | --- |
| File | 已打开句柄的 file identity、长度和 modify/change stamp；严格模式读取哈希 | Desktop 可持久化 |
| HTTP | FreshUntil、ETag、Last-Modified 和 Vary | 服从响应缓存指令 |
| Asset | 当前 Application build/resource identity 内不可变 | 可持久化 |
| StorageFile | 显式 revision，或平台可观察 basic properties | 仅稳定 identity/revision 可持久化 |
| Bytes | 防御性副本直接计算 ContentId | 不建立来源持久映射 |
| Stream | 显式 identity + revision；否则每次打开并计算 ContentId | 仅稳定 identity/revision 可持久化 |
| Borrowed image | 对象 identity；不进入 encoded/decoded cache | 不持久化 |

File 的 metadata probe 和正文读取基于同一个已打开句柄，避免路径探测与正文读取之间的替换竞态。HTTP fresh snapshot 可直接
复用；过期或 `no-cache` snapshot 必须重验证，优先使用 ETag，其次 Last-Modified；`304` 延续原 ContentId，`200` 对新正文
重新计算 ContentId；`no-store` 不保留 source、encoded 或 decoded 条目。

调用方为 Stream 或 Storage 提供 revision，即承诺 revision 不变时内容不变；无法作出该承诺时必须省略 revision，让 loader
重新读取并以 ContentId 判断内容复用。

### 4.2 集合与双通道状态

`ItemsSource` 物化为控件内部的 `ImagePreviewEntry` 集合。每个 entry 拥有两个互相隔离的通道：

| 通道 | 内容 | 状态与资源 |
| --- | --- | --- |
| Full | 预览完整图 | FullState/Error/Progress、generation、waiter CTS、物理像素 bucket、`ImageLoadResult` lease |
| Thumbnail | 页面封面 | ThumbnailState/Error/Progress、generation、waiter CTS、独立 bucket 与 lease |

集合支持 enumerable replacement，以及 `INotifyCollectionChanged` 的 Add、Remove、Move、Replace 和 Reset。未变的 immutable item
可以复用 entry 与现有 lease；被移除或替换的 entry 立即取消并 dispose。detach 期间解除集合订阅，reattach 时重新物化当前集合，
补齐离线变更。

集合动作只影响 Previewer 自己的 entry、waiter 和 lease，绝不调用全局或分区 cache clear：

| 集合或宿主动作 | Previewer 行为 | Application cache |
| --- | --- | --- |
| Add | 创建 entry，按 open/cover/preload 状态请求 | 正常共享 |
| Remove / Replace | 取消并 dispose 旧 entry | 不清理 |
| Move | 移动 entry，保留其通道与 lease | 不清理 |
| Reset / Clear | 复用仍存在的 item，其余 entry 全部 dispose | 不清理 |
| ItemsSource replacement | 重新物化，释放不再使用的 entry | 不清理 |
| host close | 释放 Full waiter/lease，恢复关闭态 Thumbnail 策略 | 不清理 |
| detach | 释放 Full/Thumbnail waiter/lease 并退订集合 | 不清理 |

缓存正确性不依赖集合 Clear。相同路径的文件被替换后，`ValidateSource` 产生新的 SourceVersion；新字节产生新的 ContentId，进而
产生新的 DecodeKey。旧 decoded entry 不能作为新内容命中，只能保留为受预算约束的 LRU 条目直至驱逐。

### 4.3 加载优先级、尺寸与预加载

| 场景 | Source 顺序 | `ImageRequestPriority` | 默认 CacheRead |
| --- | --- | --- | --- |
| 当前完整图 | `Source -> FallbackSource` | `Critical` | item options；缺省为 `ValidateSource` |
| 单封面或 group 缩略图 | `ThumbnailSource -> Source -> FallbackSource` | `High` | item options |
| 相邻完整图预加载 | `Source -> FallbackSource` | `Preload` | item options |

打开态加载当前完整图和 `PreloadCount` 邻近窗口；离开窗口的未完成 Full waiter 被取消。邻项按与 CurrentIndex 的距离从近到远
提交。关闭宿主时释放所有 Full lease，再恢复关闭态封面请求；Thumbnail lease 保留供页面封面继续显示。

完整图解码目标来自 TopLevel ClientSize，封面目标来自 CoverWidth/CoverHeight 或实际 Bounds，均乘 render scaling 并向上量化
到 16 px bucket。Full 与 Thumbnail 分别按 bucket 保持请求幂等；布局、窗口尺寸或 DPI 变化产生更合适的 bucket 时保留当前图片并
异步升级，成功后原子替换。明确得到 0 x 0 时不发起无意义的封面请求。SVG 等尺寸无关 codec 不把显示尺寸写入 DecodeSpec。

### 4.4 Fallback、Reload 与结果事件

Fallback 是每个 item、每个通道的局部策略。一个 item 失败不能删除其他 item、替换 ItemsSource 或改变 CurrentIndex。取消不触发
fallback，也不提交 Failed。

`ReloadCurrent()`、`ReloadItem(index)` 和 `ReloadCover()` 使用 item 的 RequestOptions 副本，只把目标通道本次请求的
`CacheRead` 覆盖为 `RefreshSource`。调用方 options 不被修改；另一个通道和其他 item 的 generation、waiter、state 与 lease
不受影响。

只有当前 Full entry 从非 Loaded 进入 Loaded 时触发 `ImageOpened`，从非 Failed 进入 Failed 时触发 `ImageFailed`。预加载项和
封面状态不冒充当前项事件。

加载结果把“从哪里取得”和“来源是否已验证”分开表达：

```csharp
public enum ImageLoadOrigin
{
    Borrowed,
    DecodedMemory,
    EncodedMemory,
    Persistent,
    Network,
    Local
}

public enum ImageSourceValidation
{
    NotRequired,
    Current,
    Revalidated,
    Unverified
}
```

成功结果至少包含 `Image`、`ContentId`、`Origin`、`SourceValidation`、原始/解码尺寸、media type 和阶段耗时。borrowed image 的
ContentId 为空，Origin 为 `Borrowed`，SourceValidation 为 `NotRequired`。结果 lease 独立于 cache membership；条目被驱逐时，
仍被控件持有的 image 在最后一个 lease 释放前保持有效。

### 4.5 切换显示、标题与宿主

`ImageSwitchMode` 决定目标图片切换时是否保留上一张图片。`Immediate` 在目标没有可显示图片时于同一 UI 更新周期清空旧图，并把
Idle/Loading 的待完成目标投影为 loading；`WaitForLoaded` 在有效保留帧存在时持续显示旧图，目标加载状态仍保持为 true，但 loading
presenter 由 `:loading:not(:has-image)` 门控而不覆盖旧图。失败呈现、事件时序、请求与租约语义不因模式变化；运行时切换模式立即重算
预览窗口与单封面状态。
完整显示矩阵和保留帧生命周期见 [ImagePreviewer 切换显示设计](switch-display-design.md)。

标题优先级固定为：

```text
宿主显式 Window.Title / PreviewTitle
  > ImagePreviewItem.Title
  > IImagePreviewTitleResolver
  > 空标题
```

默认 resolver 返回 `Item.Source.DisplayName`。resolver 上下文只包含 immutable item、显示用 CurrentIndex 和 Count，不发起 I/O。
`PreviewTitleIcon` 只显示在预览标题栏，不投射到普通 Window icon。

Desktop 支持 native window 时使用 `ImagePreviewerDialog`；Browser 等无 native window 平台使用
`ImagePreviewerOverlayHost`。两种宿主共享 ItemsSource、CurrentIndex TwoWay、交互、占位、动效、加载和关闭语义。

## Theme and Token Boundaries

稳定主题节点包括：

- `PART_CoverItemsControl`：group 封面集合；
- `PART_ImageViewerScene`、`PART_ImageRenderer`：预览坐标空间与图片 renderer；
- `PART_LoadingPresenter`、`PART_ErrorPresenter`：封面和 viewer 的状态占位；
- `PART_PreviousButton`、`PART_NextButton` 与 toolbar 操作按钮；
- `PART_TitleLayout`、`PART_IconPresenter`：dialog 标题与图标；
- `PART_CloseButton`：overlay 关闭入口。

Renderer 只消费 entry 已提交的 `IImage`，不得自行打开 Source 或访问 loader/cache。Loading 时封面使用稳定尺寸 Skeleton，viewer
使用居中 Spin；Failed 时使用本地化默认错误内容或用户模板。viewer 的加载指示器由 `:loading:not(:has-image)` 门控，只在无可
显示图片时呈现。Loading/error 自定义模板只替换内容，不能拥有请求、entry 或结果 lease。

本设计不改变现有缩放、拖拽、旋转、导航、标题栏、封面 mask、Token 和 Light/Dark 视觉契约。Token 的来源、计算和消费位置见
[ImagePreviewer Token 设计](token.md)。

Token 边界：

ImagePreviewer Token 只表达组件级视觉变量，例如尺寸、间距、颜色、圆角、阴影、图标尺寸和弹层边界。Token 不承载运行时选择、展开、加载、错误、上传任务、过滤条件或业务状态。

当前 Token scope：

- `ImagePreviewerToken`，scope id 为 `ImagePreviewer`，源码位于 `src/AtomUI.Desktop.Controls/ImagePreviewer/ImagePreviewerToken.cs`。

## Customization Boundaries

- `ImageSource` 来源层次、`ImageCacheReadPolicy` 和 `ImageCacheStoragePolicy` 是唯一公共模型，不保留旧名称或兼容 shim。
- 普通加载默认 `CacheRead=ValidateSource`、`CacheStorage=MemoryAndDisk`；Reload 只对单次目标通道使用 `RefreshSource`。
- 来源变化不依赖集合 Clear、控件重建或手工 cache clear 才能被识别。
- Previewer 的 Add/Remove/Replace/Move/Reset/Clear、host close 和 detach 不能清除 Application cache。
- 关闭 dialog/overlay 释放宿主 binding、订阅、logical parent 和全部 Full lease；detach 释放 Full/Thumbnail waiter 与 lease。
- reattach：重新物化集合并按 IsOpen 选择当前加载策略。
- TopLevel resize 或 render scaling 变化重新计算物理像素 bucket；相同 bucket 不重复读取，不同 bucket 异步升级。
- source replacement、collection remove/reset、旧 generation 和已关闭 host 都不能回写当前 entry。
- Full 与 Thumbnail 通道保持取消、状态、错误、进度、尺寸和 lease 隔离。
- 当前项和封面按显示索引 clamp，但不静默改写外部 TwoWay CurrentIndex。
- Template Part、伪类、loading/error 门控、标题、导航、缩放和 Token 视觉语义保持稳定。
- 不允许同步 I/O、固定延迟、反射发现 reader/codec/serializer，或在 Previewer 内创建 `HttpClient`、cache 或 scheduler。
- SourceSnapshot、encoded content 和 decoded content 的共享范围都受 CachePartition 限制；安全分区之间不共享命中或诊断。
- borrowed `IImage` 始终由调用方拥有，任何 loader/cache/control 生命周期都不能 dispose 它。
- Semantic Part：删除或重命名 Part、修改 SelectorRoute 命中范围、收窄 `ContractType` 都属于破坏性变更；`popup.*` 部件仅宿主
  打开期间存在，关闭后不残留任何 marker 节点；内置主题不使用 `.semantic-*` selector 实现默认视觉；`.semantic-scope-*`
  路由锚点不是公开 Part，不进入兼容承诺；native dialog 的窗口 chrome（标题栏、caption 按钮、窗口边框）不属于任何 Part；
  宿主必须保持挂入 owner 的 logical parent 链，popup 部件的生成 Selector 依赖该链命中 overlay 宿主（与 owner 同 TopLevel）；
  native dialog 是独立 TopLevel，owner 作用域样式不跨窗口级联，预览视觉经 host 契约定制。
- 宿主分层与上游 DOM 对齐：overlay 宿主模板根 Panel 只承担 `popup.root` 容器职责、不带背景，遮罩背景必须由独立的
  `popup.mask` 子元素承担；`popup.mask` 仅 Overlay 宿主存在（`Optional`），native dialog 不物化该部件。
- 遮罩点击关闭（上游 `maskClosable=true` 默认）当前未在 overlay 宿主实现，关闭经 overlay 宿主内嵌关闭按钮（非语义部件）；Part 契约只承诺样式命中，
  不承诺该行为，属行为对齐的既有差异。

维护不变量：

- public `ImagePreviewItem` 保持 immutable，只保存配置；所有可变加载状态只属于 internal entry。
- Full 与 Thumbnail 的 generation、CTS、state、progress 和 lease 相互独立，Reload 一个通道不能干扰另一个通道。
- Full 与 Thumbnail 分别记录活动/已提交物理像素 bucket；布局或 DPI 变化后不得继续放大较小 bucket 的旧结果。
- 同尺寸桶下的优先级提升必须采纳在途请求，不得重启；取消（调用方或共享操作内部）必须良性归位
  （保留旧图或回 Idle），不得产生 Failed 提交或 `ImageFailed`。
- `ImageLoader` 的取消分类：非超时、非销毁的 `OperationCanceledException` 一律按取消交付，禁止上报为源失败。
- current、cover 和 collection clamp 只决定显示 entry，不静默改写外部 TwoWay 索引。
- 所有请求进入 Application-scoped `IImageLoader`；Previewer 不增加本地 semaphore、cache、transport 或 codec。
- 普通请求必须先按 `ImageCacheReadPolicy` 解析或验证 SourceSnapshot，再按 ContentId/DecodeSpec 查询 decoded store；不存在
  SourceKey 直返 decoded image 的 fast path。
- 相同路径或 URI 的来源发生变化时必须形成新的 SourceVersion 和 ContentId；正确性不依赖 collection Clear 或手工 cache clear。
- Add/Remove/Replace/Move/Reset/Clear、host close 和 detach 只释放控件 waiter/lease，不清理 Application cache。
- `ImageCacheReadPolicy` 与 `ImageCacheStoragePolicy` 是正交契约；Reload 只覆盖单次请求的 CacheRead，不修改 item options。
- `CacheStorage=None` 可以读取既有 cache，但不能把 persistent 命中提升到 memory store；任何进入 memory 的 persistent 内容必须先
  通过当前安全策略校验。
- source/decode single-flight、source commit generation 和 cache epoch 必须阻止重复工作、snapshot 回滚和清理后回填。
- host close 释放 Full leases，detach 释放 Full/Thumbnail leases；旧 entry、旧 generation 和已关闭 host 都不能回写。
- dialog 与 overlay 必须共用 `ImagePreviewDisplayTracker`，不允许在任一宿主内复制目标项/保留帧跟踪；目标项与保留帧
  订阅只在 `SetCurrentItem` / `SetRetained` 内成对变更；宿主必须订阅有效集合增量变更（索引不变但 entry 更换时重配），
  宿主关闭必须退订集合并清空 tracker。
- 显示解析必须把“存在目标但尚无图且未失败”的 Idle/Loading 状态投影为 loading；`Immediate` 同周期清空旧图且不持有保留帧，
  `WaitForLoaded` 至多持有 1 个保留帧，并以 tracker 会话标识与会话内单调目标序号约束“当前项完成 / 最近完成全图加载”的两级来源；
  乱序完成、纯预加载和旧宿主会话都不能改变当前显示。
- `ImageSwitchMode` 运行时变化必须立即刷新打开态 tracker 与单封面状态，不能等待下一次索引、集合或加载通知。
- entry `Dispose()` 必须先广播 Full/Thumbnail 重置通知再清空 subscriber；显示持有者不得在通知后继续引用已释放位图。
- Full/Thumbnail 卸载与失败提交必须先断开旧 result，再发布一致状态，最后释放旧 lease；禁止暴露“Failed + 旧图”或
  “lease 已释放 + 显示仍持有旧图”的中间状态。
- viewer 加载指示器只在无显示图时呈现（`:loading:not(:has-image)` 门控）；封面 mask 只由 `IsShowCoverMask` 决定，
  与加载/失败状态解耦；错误呈现仍绑定 `IsCurrentImageFailed`。
- 封面 mask 铺满整个 owner root（含 padding 环与边框），对齐上游 `genImageCoverStyle` 的 `position:absolute; inset:0`
  cover：`ImagePreviewerCover` 以 internal `OwnerPadding` / `OwnerBorderThickness` 中继 owner 几何（单封面经
  `TemplateBinding`，组封面 DataTemplate 经 `RelativeSource AncestorType` 绑定），并把两者之和的负值写入
  `OwnerMaskMargin`，owner 模板用 `{Binding OwnerMaskMargin, RelativeSource TemplatedParent}` 应用到 `#Mask` 的
  `Margin`——这是运行时几何（宿主 padding 是用户属性），ControlTheme 无法静态表达，因此以代码计算 + 模板绑定兜底；
  hover 遮罩压暗 padding 环是上游固有视觉（白色 padding 被压成约 178 灰），不得当作缺陷回退该几何。
- 裁剪职责归 owner 根：`ImagePreviewerCover` 的 ControlTheme 不得声明 `ClipToBounds` Setter，且控件静态构造必须
  `ClipToBoundsProperty.OverrideDefaultValue<ImagePreviewerCover>(false)`——Avalonia `TemplatedControl` 的类级默认值
  是 `true`（合成层裁剪，同时约束 hit-test 与 effective viewport），会把负 Margin 铺出边界的遮罩裁回 cover 内区；
  该裁剪不体现在 `Bounds` 上，布局断言不可见，必须以“遮罩矩形在所有 ClipToBounds 祖先坐标空间内完整包含”的
  结构断言锁定。root 圆角对齐上游 `overflow:hidden + border-radius`：owner 模板的 `PixelAlignedBorder`
  （用户可设 `ClipToBounds` + `CornerRadius`）负责 root 圆角裁剪，但遮罩以负 Margin 越过 owner padding、
  不被 owner 圆角裁剪覆盖，因此 `#Mask` 与 cover 模板内 border/loading/error presenter 的圆角必须经
  `OwnerCornerRadius` 中继直接跟随 owner `CornerRadius`（单封面 `TemplateBinding`，组封面
  `RelativeSource AncestorType` 绑定），回归测试断言 `mask.CornerRadius` 与 owner 一致。
- 封面图片圆角独立于 root 圆角：上游 `styles.image` 可为 image 元素单独设置 `borderRadius`（示例 4px，root 8px），
  AtomUI 的 image part（`ImagePreviewRenderer`）以 `Border.CornerRadiusProperty.AddOwner` 暴露 `CornerRadius`，
  并把 `RoundRectGeometryBuilder` 的 WinUI 关键点圆角几何（与 `DashedBorder.ClipContentToCornerRadius` 同算法）
  设到子 `Image` 的 `Clip` 属性上——渲染管线在遍历每个 Visual 时应用其 `Clip`，Image 只渲染一次且带裁剪；
  不得改为 `Render` override 中 `PushGeometryClip` 包着 `image.Render` 手绘——子 Image 是 VisualChild，渲染器在
  父 `Render` 之后还会独立遍历 VisualChildren 再绘制一次无裁剪的 Image，覆盖手绘结果；该值不由内置主题默认设置
  （对齐上游默认 image 无圆角），经生成 `ImagePreviewerImageStyle` 由用户 Semantic Style 定制，Setter 属性名必须写
  限定名 `Property="Border.CornerRadius"`（直接写 `CornerRadius` 会经 internal 渲染器类型自身的字段解析，
  XAML 编译期不做可见性检查，运行时抛 `FieldAccessException`），`x:SetterTargetType="atom:ImagePreviewRenderer"`
  提供类型上下文；回归测试断言生成 Style 的 `CornerRadius` Setter 经 owner 作用域命中模板内 renderer
  （`renderer.CornerRadius == 4`），并断言圆角落到子 `Image.Clip` 的圆角几何（外角点在几何外、直边内点在几何内、
  零圆角清除 Clip、Clip 边界跟随子 Image 布局变化重建）。
- renderer、loading presenter 和 error presenter 只消费状态，不发起 I/O 或拥有结果。
- native dialog 与 Browser overlay 必须共享 item、current、navigation、loading 和关闭语义。
- 两个 owner 的 Semantic descriptor 与所有内置主题的 marker 完整一致；模板变体无法提供部件时必须声明 `Optional`
  （`popup.mask` 即 overlay 宿主限定部件）。
- 内置主题不得用 `.semantic-*` selector 实现默认视觉；`.semantic-scope-*` 锚点不作为公开契约。
- popup 部件的 SelectorRoute 不设中间 scope 锚点（overlay 宿主模板根与 viewer 在逻辑树上为兄弟，锚点式路由无法在双宿主间
  一致命中），`popup.actions` 只允许以已发布的 `.semantic-popup-footer` 作为模板跨入锚点。
- overlay 宿主模板根 Panel 只承担 `popup.root` 容器职责、不得直接涂背景；半透明遮罩背景必须由独立的 `popup.mask` 子元素
  承担，与上游 `.ant-image-preview`（root）与 `.ant-image-preview-mask`（mask）的分层一致。
- 宿主必须保持挂入 owner 的 logical parent 链与 ThemeVariant binding 中继；popup 部件生成 Selector 依赖该链命中 overlay
  宿主子树（overlay 与 owner 同 TopLevel）。native dialog 是独立 Window/TopLevel，owner 作用域样式不跨其边界级联，dialog 内
  预览视觉经 host 契约（owner 属性/Token 中继与 App 级 `ImageViewer` 主题）定制；破坏该链只破坏 overlay 宿主的 `popup.*` 命中。
