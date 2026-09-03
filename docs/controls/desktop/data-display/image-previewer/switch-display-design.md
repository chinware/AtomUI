# ImagePreviewer 切换显示设计

本专项定义 ImagePreviewer 预览窗口在切换当前图片时的显示策略、保留帧状态机、entry 释放通知契约和加载指示器门控。公共契约与行为模型见
[ImagePreviewer 桌面版架构设计](overview.md)，源码结构与整体实现原理见 [ImagePreviewer 桌面版实现原理](implementation.md)。

## 1. 设计定位

预览窗口（Desktop native dialog 与 Browser overlay）在 `CurrentIndex` 变化或集合变化切换目标图片时，必须决定当前渲染内容：目标图已加载则立即显示；
目标图未加载时，要么清空显示进入加载占位，要么保持上一张已加载图直到目标图就绪。高频追加图片并跟踪最新项的场景要求后一种策略不产生空白帧与
占位闪烁。

本设计覆盖：

- `AbstractImagePreviewer.ImageSwitchMode` 公共契约与两种模式的显示矩阵。
- 宿主显示状态机（internal `ImagePreviewDisplayTracker`）的职责、算法与资源上界。
- `ImagePreviewEntry.Dispose()` 的重置通知契约。
- `ImageViewer` 加载指示器的伪类门控。

本设计不覆盖关闭态封面（Thumbnail 通道）、预加载窗口、解码尺寸策略和标题模型；它们见主文档对应章节。

## 2. 设计原则

- 显示策略由公共 API 显式选择；默认 `Immediate` 保持既有清空切换行为，不额外持有任何 entry、位图或订阅。
- 显示状态是宿主内的单一 owner（目标项 + 保留帧），dialog 与 overlay 共用同一状态机，不允许在两个宿主各自复制。
- 保留帧只在明确上界内存在：至多一个"最近持有可用完整图的 entry"，且仅 `WaitForLoaded` 模式维护。
- 加载指示器只在无可显示图片时呈现；有图显示时不叠加占位视觉。
- `ImageOpened` / `ImageFailed` 时序只由目标 entry 的 Full 状态机决定，显示策略不改变事件顺序与触发条件。

## 3. 专项模型与 Public API

### 3.1 术语

| 术语 | 定义 |
| --- | --- |
| 目标项（target） | 当前索引 clamp 后选中的 `ImagePreviewEntry`；其 Full 通道状态决定加载与失败呈现 |
| 保留帧（retained frame） | 最近一个已知持有可用 `FullImage` 的 entry 引用，仅作为目标项加载期间的显示回退源 |
| 显示图（display image） | 宿主当前实际渲染的 `IImage`，由目标项 `FullImage` 或保留帧解析得到 |

### 3.2 Public API

```csharp
public enum ImageSwitchMode
{
    Immediate = 0,
    WaitForLoaded = 1,
}
```

`AbstractImagePreviewer.ImageSwitchMode` 默认 `Immediate`，`ImagePreviewer` 与 `ImageGroupPreviewer` 继承。属性只表达显示策略，
不影响加载请求、优先级、预加载窗口、事件和关闭态封面。

### 3.3 显示矩阵

显示解析统一为"目标有图显示目标图，否则在保留帧可用期间保留上一张，Immediate 超过宽限期回退占位"：

| 目标项状态 | `Immediate` 显示图 / 占位 | `WaitForLoaded` 显示图 / 占位 |
| --- | --- | --- |
| 有图（Loaded） | 目标图 / 无 | 目标图 / 无（保留帧更新为目标） |
| 无图且未失败（Idle/Loading），保留帧可用且未过宽限（300ms） | **保留帧图** / 无 | **保留帧图** / 无 |
| 无图且未失败，超过宽限期 | `null` / LoadingContent 或 Spin | **保留帧图** / 无（无限期保持） |
| 无图且未失败，无保留帧（首图/保留帧失效） | `null` / LoadingContent 或 Spin | 同左 |
| Failed | `null` / ErrorContent | `null` / ErrorContent（放弃保留帧呈现） |
| 无目标项 | `null` / 视状态 | `null` / 视状态（不保留） |

退化与边界规则：

- 显示解析在构造上无瞬态空洞：回退条件是"目标尚无图且未失败"（含切换瞬间的 Idle 态），
  而不是"目标在 Loading"——后者会在 `LoadFull/LoadThumbnail` 调用前输出一帧 null，
  经透明度过渡等视觉放大为闪烁。
- 保留帧来源有两级：当前项完成加载时更新为当前项；当前项被更快的新目标取代而永不完成时，
  从"最近完成全图加载且图像可用"的种子 entry 补充（切换快于加载完成场景）。
- 加载指示器延迟出现：两种模式在宽限期内都不显示指示器（直接换图）；Immediate 超过
  300ms 宽限仍无图才回退占位。高频切换下两种模式均无空白帧频闪与转圈频闪。
- 首图（无可保留图）在两种模式下都呈现加载占位。
- 失败呈现：放弃保留帧，显示 ErrorContent（与模式无关）。
- `ReloadCurrent` 重载已加载图期间，目标项旧图仍在，显示保持旧图。
- 运行时切换模式即时生效（每次重算读取当前值）；宽限回退按目标维度重置。

## 4. 变体、平台或状态策略

| 维度 | 差异 | 共享语义 |
| --- | --- | --- |
| 宿主（Dialog / OverlayHost） | 无 | 两种宿主构造并消费同一 `ImagePreviewDisplayTracker`；宿主订阅有效集合的增量变更，与 previewer 的 `ConfigureCurrentEntry` 同步重配当前项（索引值不变但 entry 更换时不可失效） |
| 模式（Immediate / WaitForLoaded） | 仅"无图超过 300ms 宽限"一格不同（见 3.3 矩阵） | 显示连续性、瞬态无空洞、保留帧两级来源、失败呈现、事件与租约语义完全一致 |
| 显示面（预览窗口 / 封面） | 无 | 同一套保留帧与宽限语义分别实现在 `ImagePreviewDisplayTracker`（全图通道）与 `ImagePreviewer` 封面状态机（缩略图通道）；封面项被集合移除时保留帧随之失效并回退占位（entry 租约释放，位图不可跨生命周期安全保留） |
| Loading 指示器门控 | 无 | `:loading:not(:has-image)`；mask（悬停操作层）只由 `IsShowCoverMask` 决定，与加载/失败状态解耦 |

## 5. 架构、文件结构与职责

| 类型 | 职责 | 不负责 |
| --- | --- | --- |
| `ImagePreviewDisplayTracker`（internal） | 目标项与保留帧的引用跟踪、entry 通知订阅、显示图三值解析（显示图/目标加载/目标失败）、变更回调 | 发起或取消加载、持有视觉与模板、订阅 previewer、拥有 ItemsSource |
| `ImagePreviewerDialog` / `ImagePreviewerOverlayHost` | 构造并持有 tracker；把解析结果写入自身 `CurrentImage`、`IsCurrentImageLoading`、`IsCurrentImageFailed` 直通属性；关闭时清空 tracker | 自行维护 current-entry 订阅与显示解析（不允许绕过 tracker 复制状态机） |
| `ImageViewer` | 由 `CurrentImage` / `IsCurrentImageLoading` 同步 `:has-image` / `:loading` 伪类 | 判定显示策略或持有保留帧 |

tracker 构造时接收模式访问器（`Func<ImageSwitchMode>`，读取 owner previewer 实例）与状态变更回调；每次重算即时读取模式，不缓存、不订阅模式
属性。

## 6. Template、组合与集成契约

- `ImageViewer` 声明伪类 `:loading`（目标项加载中）与 `:has-image`（显示图非空），由控件在对应属性变更时同步，同值短路。
- `PART_LoadingPresenter` 默认不可见，由 `^:loading:not(:has-image) /template/ Border#PART_LoadingPresenter` 选择器置可见；自定义模板必须保留该
  门控语义，不得回退为"加载中即显示"。
- `PART_ImageViewerScene` 的可见性继续绑定显示图非空，契约不变。
- `PART_ErrorPresenter` 继续绑定 `IsCurrentImageFailed`；其依据的不变量是"Failed 必然无显示图"（`Immediate`：失败提交即清空结果；`WaitForLoaded`：
  失败放弃保留帧呈现）。

## 7. 核心算法、数据流与生命周期

### 7.1 显示决策

输入：目标项（含通道状态与图像）、保留帧（含种子来源）、模式、目标维度宽限状态。输出：显示图、目标加载态、目标失败态。

```text
显示图 = 目标项图像
       ?? (目标尚无图且未失败且未过宽限 ? 保留帧图像（两级来源） : null)
```

保留帧两级来源：

- 当前项有图时，保留帧更新为当前项；
- 保留帧耗尽且目标无图时，从 `AbstractImagePreviewer.LatestLoadedFullEntry`（最近完成
  全图加载且图像可用的 entry，由 previewer 在既有全量 entry 订阅中维护，零新增订阅）补充。

宽限期（仅 Immediate）：

- 进入"保留帧兜底"状态即启动 300ms 宽限（`Task.Delay` + `Dispatcher.Post`，世代号使
  在途回调随任何状态变更失效）；届满仍未就绪则按目标维度置过期并回退占位；
- 目标切换或图像到达即重置；WaitForLoaded 不启动宽限（无限期保持）。

保留帧与宽限的清理时机：保留帧源图像失效（卸载、Dispose、提交失败、集合移除）、无目标项、
`Clear()`（宿主关闭/集合清空）。

### 7.2 订阅配对

- 预览侧：目标项订阅只经 `SetCurrentItem` 变更分支配对；保留帧订阅只经 `SetRetained` 配对
  （含重算中的丢弃与种子补充路径）；不变量"entry 被订阅 ⇔ 它是 current 或 retained"。
- 宿主侧：宿主订阅有效集合的 `INotifyCollectionChanged` 以跟随增量变更（索引不变但 entry
  更换时重配当前项/计数/首末标志）；`ItemsSource` 变更时退订旧集合，`Close()` 全量退订，
  防止已关闭宿主被集合引用。
- 封面侧：`SetCoverEntry` / `SetRetainedCoverEntry` 同一配对纪律；detach 全量释放。

### 7.3 Entry 取消与 Dispose 通知

- `ImageLoader` 的取消分类：调用方取消、共享操作内部取消（并发 waiter 竞争退出触发拆除、
  `ClearCache(CancelInFlight)` 等）一律按取消交付（抛出 `OperationCanceledException`、计入
  取消统计），不得上报为 `InvalidSource` 源失败。
- `ImagePreviewEntry` 对取消统一良性归位：有已提交旧图保持 Loaded，无则回 Idle，不产生
  Failed 提交与 `ImageFailed`；`LoadWithFallbackAsync` 的非致命兜底不得吞掉 OCE。
- `ImagePreviewEntry.Dispose()` 在清空订阅者前广播 Full/Thumbnail 重置通知（缓存
  `EventArgs`，无订阅者零开销），保证显示持有者在位图租约释放前丢弃引用。
- 同尺寸桶下优先级提升（如 Preload→Critical）采纳在途请求而不重启：重启会丢弃接近完成的
  进度，使"切换快于加载"场景下没有请求能完成。

### 7.4 保留帧失效路径

| 路径 | 结果 |
| --- | --- |
| 保留帧源被集合移除/替换（entry Dispose） | 重置通知触发重算，显示图回退到加载占位 |
| 保留帧源通道卸载（`Unload` / `UnloadFull`） | 同上 |
| 保留帧源同通道重载失败提交 | 同上 |
| 宿主关闭 / 集合清空 | `Clear()` 全量释放 |
| 模式切回 `Immediate` | 下一次重算释放 |

## 8. 资源、性能与 AOT 边界

| 项 | `Immediate`（默认） | `WaitForLoaded` |
| --- | --- | --- |
| 保留帧 entry 引用 | 宽限期内至多 1（届满/图像到达即释放或更新） | 至多 1 |
| 额外解码位图（经 `ImageLoadResult` 租约持有） | 同上，至多 1 张 | 至多 1 张（约窗口尺寸解码图） |
| 额外事件订阅 | 至多 1 | 至多 1 |

- 重算热路径零分配；宽限回调为低频定时（每次兜底一次 `Task.Delay`，世代号防陈旧回调）；
  宿主回调带三值变更短路。
- Dispose 重置通知使用静态缓存 `EventArgs`，无订阅者时零开销。
- 不新增加载请求；采纳在途请求反而降低高频切换下的重复读取。高频切换下被取代的中间
  加载不产生视觉工作，布局与视觉重建次数低于清空切换。
- 伪类同步同值短路；门控选择器开销与 `ImagePreviewerCover`、`AsyncImage` 同级。
- 无反射、动态发现或运行时注册，NativeAOT 无新增风险。

## 9. 兼容性与定制边界

- `Immediate` 默认值在"短暂加载（300ms 内完成）"场景下的可见行为变更：不再显示空白帧与
  转圈，直接换图（高频切换无频闪）；慢于宽限期的加载仍回退加载占位，语义差异保留。
- `ImageOpened` / `ImageFailed` 的触发条件与时序不变；取消一律不产生失败事件。
- `LoadingContent` / `ErrorContent` 只替换占位内容的契约不变；占位呈现时机由显示矩阵决定。
- viewer 加载指示器与封面 mask 的门控语义（`:loading:not(:has-image)`、mask 只由
  `IsShowCoverMask` 决定）为主题契约，自定义模板必须保留。
- 自定义 viewer 模板必须保留 `PART_ImageViewerScene` 可见性契约。

## 10. 验证要求

| 层 | 验证 |
| --- | --- |
| 状态机单元 | 两种模式的目标保持/宽限回退/失败呈现/保留帧种子补充与失效/Idle 瞬态无空洞/清空与退订配对/保留帧上界 |
| 宿主行为 | dialog/overlay 直通属性在切换、加载完成、失败、保留帧源移除场景下的取值；宿主跟随有效集合增量变更（索引不变 entry 更换不失效） |
| 加载管线 | 共享操作内部取消按取消交付且不产生源失败；entry 对外来取消良性归位；Dispose 通知使持有者丢弃引用；同桶优先级升级采纳在途请求 |
| 主题契约 | `PART_LoadingPresenter` 门控选择器断言；伪类由显示状态同步；mask 与加载状态解耦 |
| 模式矩阵回归 | 3.3 矩阵逐格覆盖；高频切换（含慢于切换周期的源）显示通道零空洞 |
| 资源 | 保留帧上界与订阅配对不变量；长跑高频切换内存平稳、无单调增长 |
| 实机视觉 | 高频追加并跟踪最新项场景两窗无空白帧/转圈/占位频闪；慢加载下 Immediate 显示 loading 占位、WaitForLoaded 保持旧图；mask 悬停稳定；首图、失败与默认回归走查 |
