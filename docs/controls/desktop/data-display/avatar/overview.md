# Avatar 桌面版架构设计

本文档定义 Avatar 控件家族的当前定位、公共契约、图片加载状态、主题关系和包边界。内部实现原理见
[Avatar 实现原理](implementation.md)，Token 语义见 [Avatar Token 设计](token.md)，变化记录见
[Avatar Changelog](changelog.md)。统一图片加载契约见
[AtomUI 统一图片加载系统](../../../../architecture/systems/image-loading/overview.md)。

该控件的 Popup 钉住打开属于共享弹层契约，详见 [Popup 钉住打开设计](../../other/popup/popup-pinned-open-design.md)。本控件的语义 owner 为 `AvatarGroup`，其 internal `IsPopupPinnedOpen` 只供测试和内部诊断使用；设置为 true 时保持 fold-count Flyout open state，并 relay 到内部 `FlyoutHost` 及其 Popup，设置为 false 时只解除关闭拦截。控件卸载、锚点失效、TopLevel 改变和模板重建仍按共享生命周期规则清理。

## 1. 控件定位

| 项 | 值 |
| --- | --- |
| NuGet 包 | `AtomUI.Controls` / `AtomUI.Desktop.Controls` |
| .NET 命名空间 | `AtomUI.Controls` / `AtomUI.Desktop.Controls` |
| AXAML 命名空间 | `https://atomui.net` |
| Gallery 页面 | `controlgallery/AtomUIGallery/ShowCases/DataDisplay/Avatar` |
| 控件状态 | Stable |

Avatar 展示用户、组织或对象的图像、文字或图标标识。它不负责资料卡、图片编辑、上传或大图预览。

包边界如下：

| 类型 | NuGet 包 | .NET 命名空间 | 职责 |
| --- | --- | --- | --- |
| `AbstractAvatar`、`Avatar`、`AvatarShape` | `AtomUI.Controls` | `AtomUI.Controls` | 单个头像、统一图片来源与加载状态 |
| `AvatarGroup` | `AtomUI.Desktop.Controls` | `AtomUI.Desktop.Controls` | 桌面端重叠排列、折叠计数与 Flyout |

## 2. 设计语言

Avatar 使用有限面积内的强识别内容表达主体身份。图片是首选内容，文字缩写和图标是稳定的降级内容；Circle 与 Square
只改变轮廓，不改变身份语义。Small、Middle、Large 与 Custom 提供一致的尺寸层级，`AvatarGroup` 通过重叠排列和折叠计数
表达“多个主体属于同一上下文”，不改变单个 Avatar 的加载和内容规则。

加载过程本身不应造成内容闪烁：旧图片或可用的 Text/Icon 在新请求完成前保持可见，成功提交后才切换为新图片，最终失败则
回到 Text/Icon。加载状态通过状态属性和伪类表达，而不是由模板猜测图片是否存在。

## 3. API 与契约模型

`Avatar` 的公共 API 由 `AbstractAvatar` 提供：

| 契约 | 默认值 | 语义 |
| --- | --- | --- |
| `Source: ImageLoadSource?` | `null` | 唯一主图片来源，支持 HTTP、File、Asset、Storage、Bytes、Stream 和已有 `IImage` |
| `FallbackSource: ImageLoadSource?` | `null` | 主来源终态失败后的单次备用来源 |
| `RequestOptions: ImageRequestOptions?` | `null` | cache mode、partition、variant、timeout 和 HTTP headers |
| `Text: string?` | `null` | 无已加载图片时优先于 Icon 展示的文字内容，也是 content property |
| `Icon: PathIcon?` | `null` | 无已加载图片和 Text 时展示的图标 |
| `Gap: double` | `4` | 文本与头像边缘的最小逻辑间距，用于文本缩放 |
| `SizeType: CustomizableSizeType` | `Middle` | Small、Middle、Large 或 Custom 尺寸语义 |
| `Size: double` | `NaN` | 显式尺寸；非 `NaN` 时进入 Custom，恢复 `NaN` 时恢复原 SizeType |
| `Shape: AvatarShape` | `Circle` | Circle 或 Square |
| `IsMotionEnabled: bool` | SharedToken | 控制主题动效，不改变加载状态机 |

只读加载状态：

- `LoadState: ImageLoadState`
- `LoadError: ImageLoadError?`
- `LoadProgress: ImageLoadProgress?`
- `IsLoading`、`IsLoaded`、`IsFailed`

命令式入口与事件：

- `Reload()` 使用 `ImageCacheMode.Reload` 重新请求当前来源。
- `ImageOpened` 只在主来源或 fallback 成功并成为当前图片时触发。
- `ImageFailed` 只在最终失败时触发；主来源失败但 fallback 成功不触发最终失败事件。

Avatar 不公开 URL、Bitmap 或 loader 属性族，也不允许单个控件替换应用级 loader。

`AvatarGroup` 的桌面端 API 包括 `Children`、`MaxDisplayCount`、`FoldAvatarFlyoutTriggerType`、
`FoldInfoAvatarBackground`、`FoldInfoAvatarForeground`、`SizeType`、`Size` 和 `Shape`。Group 只组织子控件，不接管
每个 Avatar 的 Source、加载状态、结果租约或 Reload。

`PART_TextPresenter` 是稳定 template part。其他命名视觉节点服务主题实现，不构成额外 public API。

LLMS 语义区域：

| Part | AtomUI 节点 | 职责 | 相关 API | 相关 Token | 稳定性 |
| --- | --- | --- | --- | --- | --- |
| `root` | `Avatar` | 归一图片来源、内容优先级、尺寸、形状和加载状态。 | `Source`、`FallbackSource`、`RequestOptions`、`Text`、`Icon`、`SizeType`、`Size`、`Shape` | `AvatarToken`、SharedToken | public |
| `surface` | `Frame` | 绘制背景、边框和圆角。 | `Shape`、`SizeType`、`Size` | Avatar background、border、size Token | internal-observable |
| `image` | `ImagePresenter` | 展示当前有效 `IImage`，不自行加载来源。 | `LoadState`、`IsLoaded` | Avatar size/radius Token | internal-observable |
| `text` | `PART_TextPresenter` | 展示 Text 并应用基于 Gap 的缩放。 | `Text`、`Gap` | Font、size Token | template-stable |
| `icon` | `IconPresenter` | 在无图片和 Text 时展示 Icon。 | `Icon` | Icon size Token | internal-observable |
| `group` | `AvatarGroup` | 排列子 Avatar、折叠超出项并管理 Flyout。 | `Children`、`MaxDisplayCount`、`FoldAvatarFlyoutTriggerType` | AvatarGroup spacing/fold Token | public |

## 4. 行为与状态模型

可见内容优先级固定为：

```text
已加载 Image > Text > Icon
```

主来源处于 Loading 或 Failed、fallback 尚未成功时，不清空可用的 `Text` 或 `Icon`。只有图片成功提交后才切换为 Image。
Source 清空、来源替换、控件 detach 或最终失败会释放不再使用的图片租约。

加载状态由以下伪类表达：

| 伪类 | 条件 |
| --- | --- |
| `:loading` | 当前 generation 正在加载主来源或 fallback |
| `:loaded` | 当前 generation 已成功提交图片 |
| `:failed` | 主来源和可用 fallback 均终态失败 |
| `:fallback` | 当前已加载图片来自 `FallbackSource` |

尺寸伪类 `:small`、`:middle`、`:large`、`:custom-size` 继续表达主题尺寸选择。图片加载伪类和尺寸伪类相互独立。

Avatar 通过 `ImageLoadController` 使用当前 `Application` 的 `IImageLoader`：

1. attach 后根据 Source、RequestOptions、有效 Bounds 和 render scaling 创建请求。
2. 解码目标使用物理像素并向上量化到 16 px bucket，避免微小布局变化反复解码。
3. Source、fallback、options、尺寸 bucket 或 attach 状态变化时递增 generation 并取消旧 waiter。
4. 同来源 Reload 或尺寸变化可在 Loading 期间保留旧图片，成功后原子替换；最终失败后释放旧租约。
5. 结果回到 UI dispatcher 后再次校验 generation 和 attach 状态，旧结果只能释放，不能回写。

HTTP 条件重验证、非网络来源 Reload 强制重读、两级请求合并、缓存、安全限制和应用销毁由统一 loader 负责，Avatar
不复制 transport、cache 或 scheduler。

## 5. 视觉与主题模型

`AvatarTheme.axaml` 位于 `AtomUI.Controls/Avatar/Themes`，使用以下稳定视觉节点：

| 节点 | 职责 |
| --- | --- |
| `Frame` | 背景、边框和圆角 |
| `IconPresenter` | Icon 内容 |
| `ImagePresenter` | 已加载 `IImage` |
| `PART_TextPresenter` | Avalonia 原生 `TextBlock`，负责文字和缩放 transform |

`PART_TextPresenter` 不依赖 Desktop Controls，保证 `Avatar` 可以由 `AtomUI.Controls` 独立提供。Circle 形状根据最终宽度设置
圆角；文本宽度超过可用区域时按 `Gap` 计算缩放，不改变布局尺寸。

`AvatarGroupTheme.axaml` 位于 `AtomUI.Desktop.Controls/Avatar/Themes`，消费同一个 `AvatarToken`，但只负责 group spacing、
overlap、折叠头像颜色和桌面 Flyout 视觉。

## 6. 控件家族或集成关系

`AbstractAvatar` 定义单头像公共契约和状态归一，`Avatar` 只提供可实例化类型。`AvatarGroup` 位于 Desktop 包，复用
`AvatarToken` 并把尺寸和形状投射给子 Avatar。`ImageLoadController` 与 `AsyncImage` 共用，确保两个控件使用同一套
generation、fallback、Reload 和 lease 规则。所有来源最终进入 Application-scoped `IImageLoader`，Avatar 家族不建立局部
transport、cache、scheduler 或 service locator。

## 7. 兼容性不变量

- 当前 API 只有 `Source`、`FallbackSource` 和 `RequestOptions`；不恢复旧来源属性或兼容 shim。
- 图片、Text、Icon 的优先级和 Loading 期间的 fallback 内容必须稳定。
- `ImageOpened`、`ImageFailed`、状态属性和伪类必须来自同一个 generation。
- Template reapply 必须解除旧 `PART_TextPresenter.SizeChanged` 订阅。
- detach 必须取消 waiter、释放图片租约和 motion binding；reattach 根据当前配置重新请求。
- borrowed `ImageLoadSource.FromImage` 永不由 Avatar 销毁。
- Public API、Theme、Gallery 示例和 `AtomUI.Controls.Tests` 必须同步验证。

## 8. 专项模型

### 8.1 来源与 fallback 模型

`Source` 是唯一主来源，`FallbackSource` 只在主来源终态失败后尝试一次。两者 identity 相同时不重复请求。`Reload()` 只改变
当前请求的 cache mode，不创建第二套来源状态；HTTP 执行条件重验证，File、Asset、StorageFile、Bytes 和 Stream 强制重读。

### 8.2 内容降级模型

可见内容严格遵循 `Image > Text > Icon`。Loading、取消和 generation 过期不等于最终失败，不得提前清空 Text/Icon 或触发
`ImageFailed`。主来源失败但 fallback 成功时，最终状态是 Loaded，并通过 `:fallback` 标识实际来源。

### 8.3 尺寸与包边界模型

逻辑尺寸乘 render scaling 后向上量化到 16 px 解码 bucket。`Size` 非 `NaN` 时进入 Custom，恢复 `NaN` 时回到原
`SizeType`。单头像及 Token 属于 `AtomUI.Controls`；桌面组合能力属于 `AtomUI.Desktop.Controls`，两个包不保留重复类型。

## 9. 文档导航、LLMS 导出与验证策略

- [Avatar 实现原理](implementation.md)
- [Avatar Token 设计](token.md)
- [Avatar Changelog](changelog.md)
- [统一图片加载系统](../../../../architecture/systems/image-loading/overview.md)

LLMS 导出来源：

| LLMS 内容 | 来源 | 说明 |
| --- | --- | --- |
| 单控件完整文档 | overview.md + implementation.md + token.md + Gallery ShowCase | 生成 `controls/avatar/index-cn.md` |
| 单控件语义文档 | overview.md + implementation.md + Themes 文件夹 + theme/template 信息 | 生成 `controls/avatar/semantic-cn.md` |
| API 表 | overview.md 语义摘要 + 源码 public surface | 不在 overview.md 中机械复制完整 API 表 |
| Design Token 表 | token.md + `AvatarToken` | 不在 token.md 中手工复制生成表 |
| 示例 | Gallery Avatar ShowCase + source snippet catalog | 只引用稳定示例 |
| 源码索引 | implementation.md | 定位 `AtomUI.Controls/Avatar` 与 `AtomUI.Desktop.Controls/Avatar` |

验证分为 public API 反射契约、加载状态与生命周期测试、Theme/Template 结构、Gallery 示例、Browser 实机加载和
Desktop/Browser NativeAOT publish。生成内容只由上述当前文档、源码、Token 和 Gallery 输入产生，不手工编辑
`docs/AI/generated/llms/`。
