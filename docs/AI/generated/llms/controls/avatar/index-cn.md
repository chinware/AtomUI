# Avatar

> 生成产物：由源文档生成，不要手工编辑。修改内容请回到控件文档、源码 public surface、Token 类型或生成数据、Gallery ShowCase 或源码结构。

## 概述

Avatar 展示用户、组织或对象的图像、文字或图标标识。它不负责资料卡、图片编辑、上传或大图预览。

包边界如下：

| 类型 | NuGet 包 | .NET 命名空间 | 职责 |
| --- | --- | --- | --- |
| `AbstractAvatar`、`Avatar`、`AvatarShape` | `AtomUI.Controls` | `AtomUI.Controls` | 单个头像、统一图片来源与加载状态 |
| `AvatarGroup` | `AtomUI.Desktop.Controls` | `AtomUI.Desktop.Controls` | 桌面端重叠排列、折叠计数与 Flyout |

## 包与命名空间

| 项 | 值 |
| --- | --- |
| NuGet 包 | `AtomUI.Controls` / `AtomUI.Desktop.Controls` |
| .NET 命名空间 | `AtomUI.Controls` / `AtomUI.Desktop.Controls` |
| AXAML 命名空间 | `https://atomui.net` |
| Gallery 页面 | `controlgallery/AtomUIGallery/ShowCases/DataDisplay/Avatar` |
| 状态 | Stable |

## 何时使用

Avatar 使用有限面积内的强识别内容表达主体身份。图片是首选内容，文字缩写和图标是稳定的降级内容；Circle 与 Square
只改变轮廓，不改变身份语义。Small、Middle、Large 与 Custom 提供一致的尺寸层级，`AvatarGroup` 通过重叠排列和折叠计数
表达“多个主体属于同一上下文”，不改变单个 Avatar 的加载和内容规则。

加载过程本身不应造成内容闪烁：旧图片或可用的 Text/Icon 在新请求完成前保持可见，成功提交后才切换为新图片，最终失败则
回到 Text/Icon。加载状态通过状态属性和伪类表达，而不是由模板猜测图片是否存在。

## 公共 API

`Avatar` 的公共 API 由 `AbstractAvatar` 提供：

| 契约 | 默认值 | 语义 |
| --- | --- | --- |
| `Source: ImageSource?` | `null` | 唯一主图片来源，支持 HTTP、File、Asset、Storage、Bytes、Stream 和已有 `IImage` |
| `FallbackSource: ImageSource?` | `null` | 主来源终态失败后的单次备用来源 |
| `RequestOptions: ImageRequestOptions?` | `null` | cache read/storage policy、partition、variant、timeout 和 HTTP headers |
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

- `Reload()` 以单次 `ImageCacheReadPolicy.RefreshSource` 覆盖重新请求当前来源，不修改已绑定的 RequestOptions。
- `ImageOpened` 只在主来源或 fallback 成功并成为当前图片时触发。
- `ImageFailed` 只在最终失败时触发；主来源失败但 fallback 成功不触发最终失败事件。

Avatar 不公开 URL、Bitmap 或 loader 属性族，也不允许单个控件替换应用级 loader。

本地或网络 SVG 的一致性模式由 `UseImageLoading(options => options.Svg.ConformanceMode = ...)` 在 Application loader 构建时统一
选择；默认 `Compatible` 接受通过完整安全/复杂度校验的重复 `id`，`Strict` 将其报告为 `InvalidImageData`。Avatar 不增加 SVG
专属属性，`RequestOptions`、`Source` 和 `FallbackSource` 也不能覆盖应用级模式。DTD、script、外部资源、危险 CSS 和资源预算在
两种模式下始终执行同一套拒绝规则。

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
| `text` | `Viewbox` + `PART_TextPresenter` | 在 Gap 定义的内容区内展示 Text，只在空间不足时等比缩小并保持居中。 | `Text`、`Gap` | Font、size Token | template-stable |
| `icon` | `IconPresenter` | 在无图片和 Text 时展示 Icon。 | `Icon` | Icon size Token | internal-observable |
| `group` | `AvatarGroup` | 排列子 Avatar、折叠超出项并管理 Flyout。 | `Children`、`MaxDisplayCount`、`FoldAvatarFlyoutTriggerType` | AvatarGroup spacing/fold Token | public |

## 事件与命令

命令式入口与事件：
- `ImageFailed` 只在最终失败时触发；主来源失败但 fallback 成功不触发最终失败事件。

## 使用示例

稳定示例来源于 Gallery ShowCase 和源码查看片段。生成器只输出可从 `ShowCaseItem` 追溯的示例，不维护第二套手写示例。

以下示例来自 Gallery 源码查看使用的 `ShowCaseItem` 片段，并已按中文资源规范化。

### 自动设置字号

来源：`controlgallery/AtomUIGallery/ShowCases/DataDisplay/Avatar/Views/AvatarShowCase.axaml:107`

Gallery key：`ExamplesContent` / item `3`

```axaml
<StackPanel Orientation="Horizontal" Spacing="10">
    <atom:Avatar Background="{Binding AvatarBackground}"
                 Gap="{Binding AvatarGap}"
                 SizeType="Large"
                 Text="{Binding AvatarText}"/>
    <atom:Button Name="ChangeUserButton"
                 ButtonType="Default"
                 SizeType="Small"
                 Command="{Binding ChangeUserCommand}"
                 VerticalAlignment="Center" Content="切换用户" />
    <atom:Button Name="ChangeGapButton"
                 ButtonType="Default"
                 SizeType="Small"
                 Command="{Binding ChangeGapCommand}"
                 VerticalAlignment="Center" Content="切换间距" />
</StackPanel>
```

## 状态模型

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

HTTP 条件重验证、非网络来源 Reload 强制重读、source/decode 两级请求合并、缓存、安全限制和应用销毁由统一 loader 负责，Avatar
不复制 transport、cache 或 scheduler。

## 主题与 Design Token

`AvatarTheme.axaml` 位于 `AtomUI.Controls/Avatar/Themes`，使用以下稳定视觉节点：

| 节点 | 职责 |
| --- | --- |
| `Frame` | 背景、边框和圆角 |
| `IconPresenter` | Icon 内容 |
| `ImagePresenter` | 已加载 `IImage` |
| `PART_TextPresenter` | Avalonia 原生 `TextBlock`，提供文字的自然排版尺寸 |
| Text `Viewbox` | 在 Gap 定义的水平内容区内向下缩放并居中文字 |

`PART_TextPresenter` 不依赖 Desktop Controls，保证 `Avatar` 可以由 `AtomUI.Controls` 独立提供。Circle 形状根据最终宽度设置
圆角；Text `Viewbox` 使用 `Gap` 投影出的左右内边距作为可用区域，并直接消费 TextBlock 的自然排版尺寸。空间不足时只做
等比缩小，短文本不放大，缩放前后都由模板布局保持水平和垂直居中。

`AvatarGroupTheme.axaml` 位于 `AtomUI.Desktop.Controls/Avatar/Themes`，消费同一个 `AvatarToken`，但只负责 group spacing、
overlap、折叠头像颜色和桌面 Flyout 视觉。

Token 来源：

Avatar Token 只表达组件级视觉变量，例如尺寸、间距、颜色、圆角、阴影、图标尺寸和弹层边界。Token 不承载运行时选择、展开、加载、错误、上传任务、过滤条件或业务状态。

当前 Token scope：

- `AvatarToken`，scope id 为 `Avatar`，源码位于 `src/AtomUI.Controls/Avatar/AvatarToken.cs`。

## AOT 与裁剪注意事项

- 不进行同步网络或文件 I/O。
- 不使用固定延迟等待布局；Arrange 和尺寸 bucket 是唯一尺寸就绪信号。
- 文字适配使用无状态的 Border/Viewbox 布局，不重复测量字形，不持有 template part 事件订阅。
- 不创建控件私有 `HttpClient`、cache 或 scheduler。
- Theme、codec 和 loader 通过静态注册保留，不使用反射扫描。
- Browser 使用统一 loader 的 Browser 能力矩阵；Avatar 不增加平台分支 API。
- SVG 一致性模式是封闭的应用级 enum；Avatar 不持有 validator delegate，不增加反射或 NativeAOT 动态发现边界。
- 快速 Source 切换只允许当前 generation 回写，旧结果必须及时释放。

## 源码索引

`AtomUI.Controls` 拥有单头像实现：

```text
src/AtomUI.Controls/Avatar/
├── AbstractAvatar.cs
├── Avatar.cs
├── AvatarPseudoClass.cs
├── AvatarShape.cs
├── AvatarToken.cs
└── Themes/AvatarTheme.axaml
```

`AtomUI.Desktop.Controls` 只拥有桌面组合控件：

```text
src/AtomUI.Desktop.Controls/Avatar/
├── AvatarGroup.cs
├── AvatarGroupFoldInfo.cs
└── Themes/AvatarGroupTheme.axaml
```

`Avatar` 和 `AvatarToken` 不在 Desktop 包中保留副本。Desktop 通过对 Controls 的正常项目引用复用类型和 Token resource。

## 相关文档

- 源设计文档：`docs/controls/desktop/data-display/avatar/overview.md`
- 实现文档：`docs/controls/desktop/data-display/avatar/implementation.md`
- Token 文档：`docs/controls/desktop/data-display/avatar/token.md`
- 变更记录：`docs/controls/desktop/data-display/avatar/changelog.md`
- 语义结构：`./semantic-cn.md`
