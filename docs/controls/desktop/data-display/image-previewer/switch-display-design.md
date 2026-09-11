# ImagePreviewer 切换显示设计

本专项定义 ImagePreviewer 预览窗口与单封面在切换目标图片时的显示策略、保留帧状态机、entry 释放通知契约和加载指示器门控。公共契约与行为模型见
[ImagePreviewer 桌面版架构设计](overview.md)，源码结构与整体实现原理见 [ImagePreviewer 桌面版实现原理](implementation.md)。

## 1. 设计定位

预览窗口（Desktop native dialog 与 Browser overlay）在 `CurrentIndex` 变化或集合变化切换目标图片时，必须决定当前渲染内容：目标图已加载则立即显示；
目标图未加载时，`Immediate` 清空旧图并进入加载占位，`WaitForLoaded` 保持上一张已加载图直到目标图就绪。单封面在 `CoverIndex`
变化时遵守相同模式语义。高频追加图片并跟踪最新项时，封面索引与预览索引是否同步由应用绑定决定，控件不隐式耦合两个公共属性。

本设计覆盖：

- `AbstractImagePreviewer.ImageSwitchMode` 公共契约与两种模式的显示矩阵。
- 宿主显示状态机（internal `ImagePreviewDisplayTracker`）和单封面显示状态的职责、算法与资源上界。
- `ImagePreviewEntry.Dispose()` 的重置通知契约。
- `ImageViewer` 加载指示器的伪类门控。

本设计不覆盖 group 封面集合、预加载窗口、解码尺寸策略和标题模型；它们见主文档对应章节。

## 2. 设计原则

- 显示策略由公共 API 显式选择；默认 `Immediate` 在目标没有可显示图片时于同一 UI 更新周期清空旧图并报告 loading，
  不额外持有保留帧 entry、位图、订阅或延迟任务。
- 显示状态是宿主内的单一 owner（目标项 + 保留帧），dialog 与 overlay 共用同一状态机，不允许在两个宿主各自复制。
- 保留帧只在明确上界内存在：至多一个"最近持有可用完整图的 entry"，且仅 `WaitForLoaded` 模式维护。
- 加载指示器只在无可显示图片时呈现；有图显示时不叠加占位视觉。
- `WaitForLoaded` 的保留帧只能向更新的显示目标前进；异步乱序完成不能使画面倒退到更旧目标。
- `ImageOpened` / `ImageFailed` 时序只由目标 entry 的 Full 状态机决定，显示策略不改变事件顺序与触发条件。

## 3. 专项模型与 Public API

### 3.1 术语

| 术语 | 定义 |
| --- | --- |
| 目标项（target） | 当前索引 clamp 后选中的 `ImagePreviewEntry`；其 Full 通道状态决定加载与失败呈现 |
| 保留帧（retained frame） | 最近一个已知持有可用图片且显示目标序号不早于当前保留帧的 entry 引用，仅作为 `WaitForLoaded` 的显示回退源 |
| 显示图（display image） | 宿主当前实际渲染的 `IImage`，由目标项 `FullImage` 或保留帧解析得到 |
| 目标标记（target marker） | entry 成为打开态显示目标时取得的 tracker 会话标识与会话内单调序号，用于隔离不同宿主会话并拒绝异步乱序完成导致的显示回退 |

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

显示解析统一为“优先显示目标项已有图片；目标无图且未失败时，只有 `WaitForLoaded` 可以使用保留帧”：

| 目标项状态 | `Immediate` 显示图 / 占位 | `WaitForLoaded` 显示图 / 占位 |
| --- | --- | --- |
| 有可显示图片（Loaded，或尺寸升级/Reload Loading 且旧结果仍有效） | 目标图 / 无 | 目标图 / 无（保留帧更新为目标） |
| 无图且未失败（Idle/Loading），保留帧可用 | `null` / LoadingContent 或 Spin | **保留帧图** / 无 |
| 无图且未失败，无保留帧（首图/保留帧失效） | `null` / LoadingContent 或 Spin | 同左 |
| Failed | `null` / ErrorContent | `null` / ErrorContent（放弃保留帧呈现） |
| 无目标项 | `null` / 无 | `null` / 无（不保留） |

退化与边界规则：

- 切换瞬间的 Idle 态已经表示“存在目标但尚无图”，必须投影为 loading；不能等待 `LoadFull` / `LoadThumbnail`
  把通道切到 Loading 后才显示提示，否则 UI 会先出现无图且无提示的空洞。
- `Immediate` 不读取、不建立保留帧；目标无图时，同一次状态重算同时输出 `displayImage=null` 与 `isLoading=true`。
- 打开态 `WaitForLoaded` 的保留帧来源有两级：当前项持有图片时更新为当前项；当前项被更快的新目标取代时，
  从“最近完成全图加载且图片可用”的种子 entry 补充。种子必须属于当前 tracker 会话，且目标序号晚于当前保留帧。
- 首图（无可保留图）在两种模式下都呈现加载占位。
- 失败呈现：放弃保留帧，显示 ErrorContent（与模式无关）。
- `ReloadCurrent` 重载已加载图期间，目标项旧图仍在，显示保持旧图。
- 运行时切换模式即时触发预览宿主与单封面重算；切换到 `Immediate` 时立即释放保留帧并呈现目标的当前状态。

## 4. 变体、平台或状态策略

| 维度 | 差异 | 共享语义 |
| --- | --- | --- |
| 宿主（Dialog / OverlayHost） | 无 | 两种宿主构造并消费同一 `ImagePreviewDisplayTracker`；宿主订阅有效集合的增量变更，与 previewer 的 `ConfigureCurrentEntry` 同步重配当前项（索引值不变但 entry 更换时不可失效） |
| 模式（Immediate / WaitForLoaded） | 目标无图时分别选择立即占位或保留帧（见 3.3 矩阵） | 目标状态、失败呈现、事件与租约语义完全一致 |
| 显示面（预览窗口 / 封面） | 预览 Full 通道可从“最近完成目标”种子单调补充保留帧；单封面 Thumbnail 通道保留直接前一张可用封面 | `Immediate` 都不持有保留帧；`WaitForLoaded` 都至多持有一个；封面项被集合移除时保留帧随之失效并回退占位 |
| Loading 指示器门控 | 无 | `:loading:not(:has-image)`；mask（悬停操作层）只由 `IsShowCoverMask` 决定，与加载/失败状态解耦 |

## 5. 架构、文件结构与职责

| 类型 | 职责 | 不负责 |
| --- | --- | --- |
| `ImagePreviewDisplayTracker`（internal） | 目标项、tracker 会话标识、会话内单调目标序号与保留帧的引用跟踪、entry 通知订阅、显示图三值解析（显示图/目标加载/目标失败）、变更回调 | 发起或取消加载、持有视觉与模板、订阅 previewer、拥有 ItemsSource |
| `ImagePreviewerDialog` / `ImagePreviewerOverlayHost` | 构造并持有 tracker；把解析结果写入自身 `CurrentImage`、`IsCurrentImageLoading`、`IsCurrentImageFailed` 直通属性；关闭时清空 tracker | 自行维护 current-entry 订阅与显示解析（不允许绕过 tracker 复制状态机） |
| `ImagePreviewer` | 对 Thumbnail 通道应用同一模式矩阵，投影 `EffectiveCoverImage` 与 Cover loading/error 状态 | 隐式同步 `CoverIndex` 与 `CurrentIndex`；为 `Immediate` 创建延迟任务 |
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

输入：目标项（含通道状态、图像与目标标记）、保留帧、当前 tracker 会话、模式。输出：显示图、目标加载态、目标失败态。

```text
显示图 = 目标项图像
       ?? (模式 == WaitForLoaded 且目标尚无图且未失败 ? 保留帧图像 : null)

加载态 = 目标通道处于 Loading
       或（存在目标项 且 目标尚无图且未失败）
```

保留帧两级来源：

- 当前项有图时，`WaitForLoaded` 保留帧更新为当前项；
- 目标无图时，从 `AbstractImagePreviewer.LatestLoadedFullEntry`（最近完成全图加载且图像可用的 entry）
  补充候选；只有候选 `DisplayTargetOwnerId` 等于当前 tracker id 且 `DisplayTargetRevision` 大于当前保留帧序号时才替换，
  防止旧目标迟到完成后使画面倒退；
- 仅作为邻项预加载、从未成为当前会话显示目标的 entry 不具备匹配标记；旧宿主会话留下的标记也不匹配，二者都不得作为保留帧；
- `Immediate` 每次重算都把保留帧归零，不安排 timer 或异步回调。

保留帧清理时机：切换到 `Immediate`、保留帧源图像失效（卸载、Dispose、提交失败、集合移除）、无目标项、
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
- 通道卸载和失败提交先原子断开旧结果，再发布一致的 image/state 通知，最后释放旧 lease；观察者不能收到
  “Failed 且仍显示旧 FullImage”或“旧图 lease 已释放但显示引用尚未清空”的中间组合。
- 同尺寸桶下优先级提升（如 Preload→Critical）通过 waiter 共享的可提升优先级状态原位更新
  coordinator 和 scheduler，不取消、不重启源读取或解码；切换快于加载完成时，请求仍可按最新优先级取得执行机会并完成。

### 7.4 保留帧失效路径

| 路径 | 结果 |
| --- | --- |
| 保留帧源被集合移除/替换（entry Dispose） | 重置通知触发重算，显示图回退到加载占位 |
| 保留帧源通道卸载（`Unload` / `UnloadFull`） | 同上 |
| 保留帧源同通道重载失败提交 | 同上 |
| 宿主关闭 / 集合清空 | `Clear()` 全量释放 |
| 模式切回 `Immediate` | 属性变化主动触发重算并立即释放 |

## 8. 资源、性能与 AOT 边界

| 项 | `Immediate`（默认） | `WaitForLoaded` |
| --- | --- | --- |
| 保留帧 entry 引用 | 0 | 至多 1 |
| 额外解码位图（经 `ImageLoadResult` 租约持有） | 0 | 至多 1 张（约窗口尺寸解码图） |
| 额外事件订阅 | 0 | 至多 1 |

- 重算热路径零分配，不创建 timer、`Task.Delay` 或 dispatcher 延迟回调；宿主回调带三值变更短路。
- Dispose 重置通知使用静态缓存 `EventArgs`，无订阅者时零开销。
- 不新增加载请求；采纳在途请求反而降低高频切换下的重复读取。高频切换下被取代的中间
  加载不产生视觉工作，布局与视觉重建次数低于清空切换。
- 伪类同步同值短路；门控选择器开销与 `ImagePreviewerCover`、`AsyncImage` 同级。
- 无反射、动态发现或运行时注册，NativeAOT 无新增风险。

## 9. 兼容性与定制边界

- `Immediate` 的稳定语义是目标无图时立即清空并显示 loading，占位时机不依赖加载耗时；
  `WaitForLoaded` 的稳定语义是存在有效保留帧时保持旧图，直到目标完成或失败。
- `ImageOpened` / `ImageFailed` 的触发条件与时序不变；取消一律不产生失败事件。
- `LoadingContent` / `ErrorContent` 只替换占位内容的契约不变；占位呈现时机由显示矩阵决定。
- viewer 加载指示器与封面 mask 的门控语义（`:loading:not(:has-image)`、mask 只由
  `IsShowCoverMask` 决定）为主题契约，自定义模板必须保留。
- 自定义 viewer 模板必须保留 `PART_ImageViewerScene` 可见性契约。

## 10. 验证要求

| 层 | 验证 |
| --- | --- |
| 状态机单元 | Immediate 同周期清空与 Idle/Loading 提示、WaitForLoaded 保持/完成切换/失败呈现、保留帧种子的会话隔离与单调补充、清空与退订配对、保留帧上界 |
| 宿主行为 | dialog/overlay 直通属性在切换、加载完成、失败、保留帧源移除场景下的取值；宿主跟随有效集合增量变更（索引不变 entry 更换不失效） |
| 加载管线 | 共享操作内部取消按取消交付且不产生源失败；entry 对外来取消良性归位；Dispose 通知使持有者丢弃引用；同桶优先级升级采纳在途请求 |
| 主题契约 | `PART_LoadingPresenter` 门控选择器断言；伪类由显示状态同步；mask 与加载状态解耦 |
| 模式矩阵回归 | 3.3 矩阵逐格覆盖；高频切换中 Immediate 每次均为无图 + loading，WaitForLoaded 在有保留帧时不出现空白；运行时模式切换立即重算 |
| 资源 | 保留帧上界与订阅配对不变量；长跑高频切换内存平稳、无单调增长 |
| Gallery / 实机视觉 | 高频追加并跟踪最新项时显式绑定 `CoverIndex` 与 `CurrentIndex`，确认封面和预览目标同步；慢加载下 Immediate 显示 loading 占位、WaitForLoaded 保持旧图；mask 悬停稳定；首图、失败与默认回归走查 |
