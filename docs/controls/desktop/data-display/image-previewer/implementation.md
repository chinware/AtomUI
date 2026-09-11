# ImagePreviewer 桌面版实现原理

本文档描述 ImagePreviewer 的源码 ownership、entry 状态机、集合同步、应用级加载集成、宿主生命周期和维护不变量。
公共契约见 [ImagePreviewer 桌面版架构设计](overview.md)，Token 语义见 [ImagePreviewer Token 设计](token.md)，
切换显示策略与保留帧状态机见 [ImagePreviewer 切换显示设计](switch-display-design.md)，变化记录见
[ImagePreviewer Changelog](changelog.md)。

## 1. 实现定位

本文覆盖 ImagePreviewer 家族的 internal entry、集合同步、双通道加载、dialog/overlay 宿主、viewer/renderer 协作和结果租约
生命周期。具体 StyledProperty 注册、AXAML selector 和局部变换公式仍以源码为准；本文记录跨文件 owner、状态流和维护边界。

## 2. 源码文件结构

`src/AtomUI.Desktop.Controls/ImagePreviewer/` 的稳定职责如下：

| 类型 | 职责 |
| --- | --- |
| `AbstractImagePreviewer` | public API、ItemsSource 物化、current entry、预加载策略和 dialog/overlay 生命周期 |
| `ImagePreviewItem` | immutable public 配置，包含 `ImageSource` 类型的 Source、Thumbnail、Fallback，以及 Options、Title 和 Tag |
| `ImagePreviewEntry` | internal Full/Thumbnail 状态、generation、取消、结果租约 owner；Dispose 前广播重置通知 |
| `ImagePreviewDisplayTracker` | internal 宿主显示状态机：目标项、tracker 会话标识、会话内单调目标序号与 WaitForLoaded 保留帧跟踪、显示图三值解析（见[切换显示设计](switch-display-design.md)） |
| `ImagePreviewer` | 单封面选择、状态投影和 `ReloadCover()` |
| `ImageGroupPreviewer` | 多封面 ItemsControl、点击索引和关闭态缩略图请求 |
| `ImagePreviewerDialog` | Desktop native window、标题算法、CurrentIndex relay 和 viewer 组合 |
| `ImagePreviewerOverlayHost` | Browser/无原生窗口平台的 overlay 宿主 |
| `ImageViewer` | 导航、变换、fit-to-window 和 loading/error 状态呈现 |
| `ImagePreviewRenderer` | 只渲染 entry 已持有的 `IImage` |
| `IImagePreviewTitleResolver` | 无 I/O 的标题解析扩展点 |

底层 `IImageLoader`、`ImageSource`、Request、Result、source snapshot、encoded/decoded store、scheduler、coordinator、transport
和 codec 位于 `AtomUI.Controls.Shared/ImageLoading/`。Previewer 目录不保留这些类型的副本。

## 3. 核心类职责

`AbstractImagePreviewer` 是 public contract 和 open state owner；`ImagePreviewEntry` 是单 item 的 Full/Thumbnail 运行状态 owner；
`ImagePreviewer` 与 `ImageGroupPreviewer` 只负责不同封面入口；dialog/overlay 负责宿主资源；`ImageViewer` 负责交互；renderer 只绘制
已提交图片。任何类型都不能同时拥有 item 配置、底层缓存和视觉宿主生命周期。

## 4. 状态与数据流

### 4.1 ItemsSource 物化

`MaterializeEffectiveItems()` 把当前 ItemsSource 枚举为 `ObservableCollection<ImagePreviewEntry>`。物化时按
`ImagePreviewItem` record equality 建立可复用 entry 队列，保持重复 item 的顺序和数量；未复用的旧 entry 被 dispose。

Observable collection 的增量处理规则：

| Action | 行为 |
| --- | --- |
| Add | 在 NewStartingIndex 创建并订阅新 entry |
| Remove | 解除订阅、移除并 dispose 对应 entry |
| Replace | 先 dispose 旧 entry，再插入新 entry |
| Move | 单项直接移动；无法确定的多项 move 重新物化 |
| Reset | 重新枚举 ItemsSource 并按 item 复用可保留 entry |

普通 enumerable replacement 同样重新物化。集合变化后统一执行 `ConfigureCurrentEntry()`、封面重配和
`RequestLoadsForCurrentState()`，避免 replacement 与 Reset 走不同加载路径。

控件 detach 时解除 `INotifyCollectionChanged`；reattach 先重新订阅，再重新物化，因此 detach 期间发生的业务集合变化不会丢失。

### 4.2 Entry 双通道状态机

每个 `ImagePreviewEntry` 对一个 immutable item，Full 与 Thumbnail 通道分别拥有：

- generation counter
- `CancellationTokenSource`
- `ImageLoadResult` lease
- 当前请求的物理像素 bucket、已提交结果对应的 bucket 和活动请求优先级
- `ImageLoadState`、`ImageLoadError?`、`ImageLoadProgress?`
- 派生布尔状态

Full 来源序列是 `[Source, FallbackSource]`；Thumbnail 来源序列是
`[ThumbnailSource, Source, FallbackSource]`。序列按 `ImageSourceKey` 去重。每个 attempt 通过当前 Application 的
`GetImageLoader()` 创建 `ImageLoadRequest`，使用 item RequestOptions、目标物理像素、调用方 priority 和 progress callback。

结果返回后切到 UI dispatcher。只有 entry 未 dispose 且 generation 匹配时才能提交；否则立即 dispose 结果。成功提交先替换
result field、记录该结果对应的请求 bucket、更新状态并通知 renderer，再释放 previous lease。相同 bucket 的 Loaded 请求保持幂等；
相同 bucket 的 Loading 请求复用当前 generation，并通过共享的可提升优先级状态把 Preload→Critical 等变化原位传播到 coordinator
和 scheduler。bucket 变化才启动新 generation。尺寸升级期间继续持有旧 lease，成功后原子替换；终态失败才清空该通道旧图、
保存 typed error 并释放失败结果。

取消不写 Failed。若取消尺寸升级时仍有已提交结果，状态恢复为 Loaded 并继续使用该结果；没有结果时回到 Idle。
`UnloadFull()` 只清理 Full；`Unload()` 清理两个通道。卸载和失败提交都先从 entry 原子断开旧 result，发布一致的 image/state 通知，
再释放旧 lease。`Dispose()` 同样先断开 result，在清空 subscriber 之前以缓存 EventArgs 广播 Full/Thumbnail 重置，最后释放 lease，保证
显示持有者不会观察或继续引用已经释放的图像。

### 4.3 Current 与 Cover 投影

`ConfigureCurrentEntry()` 使用 clamp 后的 CurrentIndex 选择 entry，但不写回 public CurrentIndex。current entry 的 FullState、
FullError 和 FullProgress 投影为控件的 `CurrentLoad*` 属性。只有 current entry 的终态变化触发 ImageOpened/ImageFailed。

单封面 `ImagePreviewer.ConfigureCoverEntry()` 使用 clamp 后的 CoverIndex 选择 entry，订阅它的 Thumbnail 状态并投影为
`CoverLoad*`。CoverIndex、CoverWidth、CoverHeight 或 EffectiveItems 变化都会重配封面。设置新 cover entry 时必须先解除旧 entry
订阅。

Group 不维护单一 cover entry；Theme 的 `PART_CoverItemsControl` 直接消费每个 entry 的 ThumbnailImage 和 Thumbnail 状态。

### 4.4 宿主显示投影与保留帧

Dialog 与 OverlayHost 不各自维护 current-entry 订阅；两者共用 internal `ImagePreviewDisplayTracker`。tracker 以宿主 `CurrentIndex`
解析出的目标项为输入，维护目标项、tracker 会话标识、会话内单调目标序号与仅供 `WaitForLoaded` 使用的保留帧，按 `ImageSwitchMode` 解析显示图、目标加载态和
目标失败态，宿主把结果写入自身 `CurrentImage` / `IsCurrentImageLoading` / `IsCurrentImageFailed` 直通属性。`Immediate` 在目标无图时
同周期输出 `CurrentImage=null` 与 loading；不持有保留帧，也不创建延迟任务。目标项与保留帧的订阅分别只经 `SetCurrentItem` 与
`SetRetained` 成对退订/订阅；`WaitForLoaded` 从 previewer 的 `LatestLoadedFullEntry`（最近完成的全图加载）补充保留帧时，只接受属于
当前 tracker 会话且目标序号晚于当前保留帧的 entry，避免旧请求迟到完成后使画面倒退。纯预加载 entry 和旧宿主会话的 target marker
都不能进入当前显示回退。宿主订阅有效集合的
`INotifyCollectionChanged`：集合封顶裁剪等增量变更下 CurrentIndex 属性值不变但索引处 entry 更换，宿主必须与 previewer 的
`ConfigureCurrentEntry` 同步重配，否则显示跟踪持有过期（可能已 Dispose）的 entry 表现为预览窗口白屏。宿主关闭路径统一经
`Close()` 退订集合并清空 tracker。`ImageSwitchMode` 运行时变化时，open-state owner 主动刷新宿主 tracker；单封面由
`ImagePreviewer.OnPropertyChanged` 同步重算 Thumbnail 显示状态。显示决策算法、显示矩阵与保留帧失效路径见
[ImagePreviewer 切换显示设计](switch-display-design.md)。

## 5. 组合结构模型

### 控件角色图

```text
ImagePreviewer / ImageGroupPreviewer
  -> cover template / PART_CoverItemsControl (template-stable)
  -> ImagePreviewEntry collection (internal-observable)
  -> ImagePreviewerDialog or ImagePreviewerOverlayHost (internal-observable)
     -> ImageViewer (internal-observable)
        -> PART_ImageViewerScene (template-stable)
           -> PART_ImageRenderer (template-stable)
           -> PART_LoadingPresenter / PART_ErrorPresenter (template-stable)
```

### 协作节点

| 节点 | 类型 | 来源 | 生命周期 owner | 影响的 public API | 稳定性 | Agent 使用边界 |
| --- | --- | --- | --- | --- | --- | --- |
| `AbstractImagePreviewer` | public base control | C# | visual tree | Items、current、open、interaction、events | public | 派生控件共享契约 |
| `ImagePreviewEntry` | internal state object | C# | previewer effective collection | current/cover load state | internal-observable | 不作为用户 item 暴露 |
| `ImagePreviewDisplayTracker` | internal display state machine | C# | 打开态宿主（dialog/overlay 各持一份） | `ImageSwitchMode` 显示行为 | internal-observable | 只由宿主消费，不发起加载 |
| `PART_CoverItemsControl` | template part | group Theme | template | `ItemsPanel`、cover states | template-stable | 改名需同步主题和文档 |
| dialog/overlay host | internal host | C# + Theme | open state | IsOpen、modal、title、close events | internal-observable | 平台能力实现，不是替代 API |
| `ImageViewer` | internal control | Theme | host | navigation、move、scale、rotate | internal-observable | 不直接加载 Source |
| `PART_ImageRenderer` | renderer part | viewer Theme | template | current image visuals | template-stable | 只消费 entry 的 `IImage` |

## 6. 生命周期与模板接入

attach 时订阅当前 `ItemsSource`、TopLevel `SizeChanged` / `ScalingChanged`，物化 entries 并按 `IsOpen` 请求 Full 或 Thumbnail。
TopLevel 尺寸或 render scaling 变化后重新计算物理像素 bucket；相同 bucket 由 entry 幂等拒绝，不同 bucket 启动替换请求。detach
时解除这些订阅和集合订阅，取消两个通道的 waiter，释放全部 leases，并清理 open host。打开宿主建立 relay bindings、事件、
logical parent 和 modal subscription，并构造显示 tracker；关闭时先处理可取消 `DialogClosing`，再释放 host state（含清空显示 tracker 与 WaitForLoaded 保留帧）
和全部 Full leases，最后恢复关闭态 Thumbnail 策略。Template 只绑定 entry 状态和图片，不持有结果 lease。

## 7. 交互与事件处理

单封面点击只打开当前项；group 封面点击先更新 `CurrentIndex` 再打开。dialog/overlay 导航通过 TwoWay relay 更新 current，显示 clamp
不回写业务值。拖拽、缩放、旋转和 fit-to-window 由 `ImageViewer` 处理。只有 current Full entry 的终态变化触发
`ImageOpened`/`ImageFailed`；预加载和 Thumbnail 不冒充当前项事件。

## 8. 内部算法与关键流程

### 8.1 请求策略

`RequestLoadsForCurrentState()` 是集合、attach 和状态变化后的唯一协调入口：

- `IsOpen=true` 调用 `RequestPreviewLoads()`。
- `IsOpen=false` 调用派生控件的 `RequestClosedStateLoads()`。

打开态算法：

1. Clamp CurrentIndex。
2. 建立 current 加前后 `PreloadCount` 的 active index set。
3. 取消 active set 外的未完成 Full loads。
4. 以 Critical 请求 current。
5. 按距离由近到远以 Preload 请求邻项。

单封面关闭态以 High 请求 CoverIndex 的 Thumbnail。Group 关闭态以 High 请求所有 effective entries 的 Thumbnail。
`RequestFullLoad` 与 `RequestThumbnailLoad` 把请求交给 entry；entry 以通道、物理像素 bucket 和活动优先级判定幂等。相同 bucket 的
Loading/Loaded 请求不重复读取，bucket 变化会替换旧 generation，Loading 中的更高优先级请求会提升该通道；Reload 显式绕过
幂等判定。

Previewer 没有本地 semaphore 或并发属性。Application loader 统一执行 download/decode 并发、priority queue、aging、请求提升、
source/decode 两级 single-flight、来源验证、content-addressed encoded/decoded store 和 cache policy。

### 8.2 解码尺寸

完整图目标尺寸使用 TopLevel ClientSize；无法取得 TopLevel 时使用控件 Bounds。封面使用 CoverWidth/CoverHeight，值为 NaN 时
回退到当前 Bounds。所有逻辑尺寸乘 render scaling 后向上量化到 16 px。初始占位测量、后续真实 Arrange、Cover 尺寸修改、
TopLevel resize 和 DPI 变化都必须重新提交当前 bucket；entry 负责过滤重复 bucket，不能用 Loaded 状态永久阻止尺寸升级。

Full 请求允许 0 x 0 表示原始尺寸。封面在宽高都为 0 时等待真实 Arrange，不用 timer 或固定 delay 猜测布局就绪。

ThumbnailSource 与 Source 即使解析到相同 ContentId，也会因解码目标不同形成正确的 decoded key；不能把封面 bitmap 当作完整图。

### 8.3 Reload 与 fallback

entry 的 `LoadWithFallbackAsync()` 在 reload 时以 record `with` 创建 RequestOptions 副本，只覆盖
`CacheRead=ImageCacheReadPolicy.RefreshSource`。调用方原 RequestOptions 不被修改。

ReloadCurrent 和 ReloadItem 只增加目标 entry Full generation；ReloadCover 只增加 cover entry Thumbnail generation。各通道的
CTS、progress、state 和 lease 独立，确保重载封面不会取消当前完整图。

Fallback 按 item、按通道执行。任一 item 的失败不会重建整个 effective collection，也不会让另一个 item 使用它的 fallback。

### 8.4 来源解析、内容身份与解码

Previewer 把纯值请求交给 `IImageLoader` 后，底层必须先解析来源，再查询 decoded cache：

```text
ImageSource
  -> ImageSourceKey
  -> validate ImageSourceVersion
  -> ImageContentId
  -> ImageContentId + ImageDecodeSpec
  -> ImageDecodeKey
  -> ImageLoadResult lease
```

`ImageSourceSnapshotIndex` 保存 SourceKey 到 SourceVersion/ContentId 的不可变映射；`ImageEncodedContentStore` 只保存经过验证的精确
编码字节；`ImageDecodedContentStore` 只按 CachePartition、ContentId 和 DecodeSpec 保存解码结果。来源地址不进入内容身份，
Previewer 也不能根据旧 SourceKey 构造 decoded candidate 并在来源验证前返回。

`ImageCacheReadPolicy.ValidateSource` 按来源规则验证已有 snapshot；`RefreshSource` 强制访问来源；`PreferCache` 只在 miss 时回源；
`CacheOnly` 禁止回源。Reload 三个入口仅把目标通道的本次策略覆盖为 `RefreshSource`。File 的 Metadata 模式使用同一已打开句柄
完成版本探测与必要的正文读取；ContentHash 模式每次读取并计算摘要。HTTP 使用 freshness、ETag、Last-Modified 与 Vary，`304`
延续已有 ContentId，`200` 提交新 ContentId。`no-store` 不写任何 store，并在 generation/epoch 仍为当前值时清除该来源旧 variant
的 source、encoded 和 decoded 条目；磁盘清理与 persistent 写入使用同一串行边界，迟到响应不能删除更新的 snapshot。

### 8.5 请求合并、提交顺序与取消

source resolution 以 SourceKey 和兼容读取策略做 single-flight；decode 以 DecodeKey 做 single-flight。每个 waiter 保留自己的
cancellation token、timeout、progress 和 completion。一个 waiter 离开不取消其他 waiter；最后一个 waiter 离开时，coordinator
先在锁内完成状态迁移，再在锁外取消和 dispose 底层 CTS。

每个 SourceKey 的来源访问取得单调递增 commit generation，较早 generation 的迟到结果不能覆盖更新的 SourceSnapshot。全局或
分区 cache clear 递增 epoch，清理前启动的 source/decode operation 不能回填已清理 scope。callback、result/stream/image dispose
和 cancellation 均在 coordinator/cache lock 外执行。

Previewer 在 UI dispatcher 提交结果前再次检查 entry 与通道 generation。取消属于良性终止：保留已有图片或回到 Idle，不触发
fallback、Failed 或 `ImageFailed`。

### 8.6 Persistent cache

持久缓存使用稳定目录名，不把格式 revision 编入目录、类型或公共 API：

```text
image-cache/
├── manifest
├── sources/<partition-hash>/<source-key-hash>.meta
├── content/<partition-hash>/<content-prefix>/<content-id>.bin
├── content-metadata/<partition-hash>/<content-prefix>/<content-id>.meta
├── locks/
└── temp/
```

`manifest` 保存内部 `formatRevision`。格式不匹配时关闭并重建 store，不维护迁移分支。content、metadata 和 source snapshot 经临时
文件、flush 和原子 replace/rename 发布；snapshot 只能在对应 content 可读取后提交。读取发现 metadata、长度、摘要或安全策略不一致
时删除损坏条目并按 miss 处理。持久文件名只使用摘要，不暴露路径、URI、header 或 CachePartition 原值。

### 8.7 Dialog 与 Overlay 打开关闭流程

`OpenDialog()` 在打开前确保 effective items 存在并请求打开态图片。支持 native window 时创建 `ImagePreviewerDialog`；否则通过
`OverlayLayerResolver` 创建 `ImagePreviewerOverlayHost`。

宿主 relay bindings 包括 interaction、scale、effective entries、占位内容、motion、modal 和 CurrentIndex TwoWay。Native dialog
额外转接 PreviewTitle、PreviewTitleIcon 和 resolver。所有 relay、主题绑定、事件订阅、logical parent 和 overlay SizeChanged
handler 都归 open-state cleanup 所有。

关闭流程先发送可取消 `DialogClosing`。允许关闭后 dispose open state 和 modal subscription，调用 `ReleaseFullImageLoads()`，
再恢复关闭态缩略图策略并把 IsOpen 设为 false。这样页面封面 lease 可以保留，而预览大图及时释放。

父 TopLevel 关闭、placement target detach 或 overlay close 都进入同一 cleanup。重复打开和重复关闭通过 `_dialogOpening`、
`_dialogClosing` 与 `_openState` 防重入。

### 8.8 标题与 CurrentIndex

Native dialog 的 effective title 算法为：非空 Window.Title、item.Title、resolver、null。PreviewTitle 通过 relay 成为 Window.Title，
因此它处于 item title 之前。默认 resolver返回 Source.DisplayName。

Dialog 和 overlay 对 CurrentIndex 只做显示 clamp，并通过 TwoWay relay 接收用户导航。集合暂时缩短时不改写外部索引；集合恢复后
可以重新解析原值。CoverIndex 不参与标题或打开态 current 计算。

### 8.9 Theme 与 renderer 边界

Theme 文件负责 cover mask、Skeleton/Spin、错误内容、viewer scene、toolbar、title bar 和 platform title layout。
`ImagePreviewRenderer` 只接收当前 `IImage` 与 transform/stretch 状态，不调用 loader。图片尺寸变化需要让 viewer 重新计算
fit-to-window 与缩放能力，但不能反向修改 entry 状态。`ImageViewer` 由显示状态同步 `:loading` / `:has-image` 伪类，
`PART_LoadingPresenter` 由 `^:loading:not(:has-image)` 选择器置可见，只在无可显示图片时呈现，与封面和 `AsyncImage` 门控语义一致。

Loading/error 自定义模板只替换内容。模板不能通过视觉存在与否推导加载成功，也不能持有 entry 之外的结果租约。

## 9. 资源、性能与 AOT 边界

- ItemsSource、entry、host 和 loader 之间没有 Visual -> business item 的反向长期引用。
- Previewer collection、host 和 entry 生命周期只管理 waiter 与 result lease；任何 Add/Remove/Replace/Move/Reset/Clear、host close
  或 detach 都不能触发 Application cache clear。
- cache membership、source/decode operation reference 和 result lease 是三个独立持有关系；LRU 驱逐先移除 membership，活动引用
  归零后再释放 owned image。
- 显示 tracker 重算热路径零分配；目标项与保留帧订阅严格经单一变更通道配对，`Immediate` 模式不持有保留帧（额外持有恒为 0），
  `WaitForLoaded` 至多持有 1 个保留帧 entry 及其解码图租约。
- entry Dispose 的重置通知使用静态缓存 EventArgs，无订阅者时零开销。
- 任何 cancellation、result dispose 或 event callback 都不在 Shared coordinator/cache lock 内由 Previewer执行。
- 不进行同步 HTTP/File I/O，不使用固定延迟，不创建私有 cache/scheduler。
- `ImageSource` reader、validator、codec 和 metadata serializer 使用静态注册或 source-generated serializer；Public data model、
  AXAML property 和 title resolver 都不依赖反射扫描。
- Browser overlay 与 Desktop dialog 共用同一 entry/load model；平台差异只位于宿主能力。
- Application dispose 统一取消底层 loader；控件仍负责尽快取消 waiter 和释放自身租约。
- borrowed `IImage` 始终由调用方拥有，loader、cache、entry 和 Application dispose 都不能销毁它。

## 10. 维护不变量

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
- renderer、loading presenter 和 error presenter 只消费状态，不发起 I/O 或拥有结果。
- native dialog 与 Browser overlay 必须共享 item、current、navigation、loading 和关闭语义。

## 11. 测试与验证

必须维护以下回归覆盖：

- enumerable replacement 与 collection Add/Remove/Move/Replace/Reset。
- item immutable、entry state isolation、detach 期间变更和 reattach 补齐。
- Critical、High、Preload 顺序和 active preload window 取消。
- Full/Thumbnail fallback、progress、ReloadCurrent/Item/Cover 隔离。
- `ValidateSource`、`RefreshSource`、`PreferCache`、`CacheOnly` 与三种 CacheStorage 的策略矩阵。
- File Metadata/ContentHash 替换识别，HTTP fresh/304/200/no-store/Vary，以及 Storage/Stream revision 契约。
- 同一路径文件替换后无需清空集合即可显示新 ContentId；集合 Clear/Reset 不扫描或清理 Application cache。
- source/decode single-flight、早 generation 晚完成、waiter-local cancellation、cache clear epoch 和 lease 延迟释放竞态。
- auto-sized cover 从占位 Bounds 升级到最终物理像素 bucket、显式 cover resize、同 bucket 幂等和替换期间旧 lease 保留。
- dialog/overlay close 释放 Full lease、保留或恢复 Thumbnail；detach 释放两者。
- 两种 `ImageSwitchMode` 的显示矩阵：`WaitForLoaded` 加载期间保持上一张、完成换新、失败呈现错误、保留帧源移除后回退占位；
  `Immediate` 从目标切换的同一 UI 周期清空旧图并呈现 loading，包括请求开始前的 Idle 瞬态与持续快速切换场景。
- 显示 tracker 的订阅配对（Clear/切换后事件不再触达）、`Immediate` 零持有、WaitForLoaded 保留帧上界、会话隔离与目标序号单调前进不变量。
- 打开态预览与单封面运行时切换到 `Immediate` 后立即重算，不等待下一条 entry 通知。
- viewer `:loading` / `:has-image` 伪类与 `PART_LoadingPresenter` 门控选择器的主题契约断言。
- current/cover clamp 不破坏 TwoWay CurrentIndex。
- PreviewTitle、item.Title、resolver 的优先级与默认 DisplayName。
- Gallery 高频场景显式把 `CoverIndex` 与 `CurrentIndex` 绑定到同一索引，验证封面与预览目标同步；同时维护 Browser、NativeAOT 和 public API baseline。
