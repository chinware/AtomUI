# Avatar 语义结构

> 生成产物：由源文档生成，不要手工编辑。修改内容请回到控件文档、源码 public surface、Token 类型或生成数据、Gallery ShowCase 或源码结构。

## Semantic Parts

| Part | AtomUI 节点 | 职责 | 相关 API | 相关 Token | 稳定性 |
| --- | --- | --- | --- | --- | --- |
| `root` | `Avatar` | 归一图片来源、内容优先级、尺寸、形状和加载状态。 | `Source`、`FallbackSource`、`RequestOptions`、`Text`、`Icon`、`SizeType`、`Size`、`Shape` | `AvatarToken`、SharedToken | public |
| `surface` | `Frame` | 绘制背景、边框和圆角。 | `Shape`、`SizeType`、`Size` | Avatar background、border、size Token | internal-observable |
| `image` | `ImagePresenter` | 展示当前有效 `IImage`，不自行加载来源。 | `LoadState`、`IsLoaded` | Avatar size/radius Token | internal-observable |
| `text` | `PART_TextPresenter` | 展示 Text 并应用基于 Gap 的缩放。 | `Text`、`Gap` | Font、size Token | template-stable |
| `icon` | `IconPresenter` | 在无图片和 Text 时展示 Icon。 | `Icon` | Icon size Token | internal-observable |
| `group` | `AvatarGroup` | 排列子 Avatar、折叠超出项并管理 Flyout。 | `Children`、`MaxDisplayCount`、`FoldAvatarFlyoutTriggerType` | AvatarGroup spacing/fold Token | public |

## Abstract AXAML Structure

未定位到可生成抽象 AXAML 结构的 ControlTheme 模板。生成器不会根据 semantic parts 发明 AXAML 节点；请以 Template Parts、主题文件和源码索引为准。

## Composition Model

该控件主要由 public 控件和 ControlTheme 模板直接表达，没有额外运行时组合层。

## Template Parts

| Part | AtomUI 节点 | 职责 | 相关 API | 相关 Token | 稳定性 |
| --- | --- | --- | --- | --- | --- |
| `root` | `Avatar` | 归一图片来源、内容优先级、尺寸、形状和加载状态。 | `Source`、`FallbackSource`、`RequestOptions`、`Text`、`Icon`、`SizeType`、`Size`、`Shape` | `AvatarToken`、SharedToken | public |
| `surface` | `Frame` | 绘制背景、边框和圆角。 | `Shape`、`SizeType`、`Size` | Avatar background、border、size Token | internal-observable |
| `image` | `ImagePresenter` | 展示当前有效 `IImage`，不自行加载来源。 | `LoadState`、`IsLoaded` | Avatar size/radius Token | internal-observable |
| `text` | `PART_TextPresenter` | 展示 Text 并应用基于 Gap 的缩放。 | `Text`、`Gap` | Font、size Token | template-stable |
| `icon` | `IconPresenter` | 在无图片和 Text 时展示 Icon。 | `Icon` | Icon size Token | internal-observable |
| `group` | `AvatarGroup` | 排列子 Avatar、折叠超出项并管理 Flyout。 | `Children`、`MaxDisplayCount`、`FoldAvatarFlyoutTriggerType` | AvatarGroup spacing/fold Token | public |

## Pseudo Classes

回到 Text/Icon。加载状态通过状态属性和伪类表达，而不是由模板猜测图片是否存在。

## State Flow

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

## Theme and Token Boundaries

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

Token 边界：

Avatar Token 只表达组件级视觉变量，例如尺寸、间距、颜色、圆角、阴影、图标尺寸和弹层边界。Token 不承载运行时选择、展开、加载、错误、上传任务、过滤条件或业务状态。

当前 Token scope：

- `AvatarToken`，scope id 为 `Avatar`，源码位于 `src/AtomUI.Controls/Avatar/AvatarToken.cs`。

## Customization Boundaries

- 当前 API 只有 `Source`、`FallbackSource` 和 `RequestOptions`；不恢复旧来源属性或兼容 shim。
- 图片、Text、Icon 的优先级和 Loading 期间的 fallback 内容必须稳定。
- `ImageOpened`、`ImageFailed`、状态属性和伪类必须来自同一个 generation。
- Template reapply 必须解除旧 `PART_TextPresenter.SizeChanged` 订阅。
- detach 必须取消 waiter、释放图片租约和 motion binding；reattach 根据当前配置重新请求。
- borrowed `ImageLoadSource.FromImage` 永不由 Avatar 销毁。
- Public API、Theme、Gallery 示例和 `AtomUI.Controls.Tests` 必须同步验证。

维护不变量：

- `ImageLoadController` 是 Avatar 与 `AsyncImage` 的单图状态机 owner，不在 Avatar 内复制请求协调逻辑。
- 内容优先级始终为 Image、Text、Icon；Loading 和取消不清空可用降级内容。
- 每个已提交 cache image 必须由一个有效 lease 支撑；borrowed image 永不由控件销毁。
- Source、options、尺寸或 attach generation 变化后，旧结果只能释放，不能回写状态。
- Template reapply、detach、group rebuild 和 Application dispose 都有明确的取消、解绑和释放路径。
- 单头像和 Token 只存在于 `AtomUI.Controls`，Desktop 包只拥有 AvatarGroup 组合能力。
