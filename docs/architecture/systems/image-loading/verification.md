# 图片加载验证与完成门禁

> 状态：当前验证契约。本文列出统一图片加载持续维护时必须通过的测试与回归门禁；API、缓存、安全、生命周期和平台能力
> 必须作为一个整体保持成立。

## 测试分层

| 项目 | 主要责任 |
| --- | --- |
| `AtomUI.Core.Tests` | owned-service 收集、attach/rollback、逆序 dispose、Application store 生命周期 |
| `AtomUI.Controls.Shared.Tests` | source/key、scheduler、两级 dedup、取消、cache、HTTP、安全、codec、租约 |
| `AtomUI.Controls.Tests` | `AsyncImage`、Avatar、controller generation、fallback、Theme 与 owned/borrowed image |
| `AtomUI.Desktop.Controls.Tests` | Previewer items、current/cover/preload 优先级、collection 同步、overlay 关闭释放 |
| `AtomUIGallery.Tests` | Gallery AXAML/API/示例只使用 AtomUI `AsyncImage`，无第三方附加属性 |

测试必须使用可控 fake clock、fake transport、barrier codec 和容量很小的真实 cache 来建立确定竞态；不得用任意 sleep 证明
并发正确。涉及 UI 提交的测试使用 Avalonia dispatcher/test host，明确推进调度队列。

## Shared 引擎测试

### 来源与键

1. HTTP 大小写、默认端口、fragment、dot segment 和 query 顺序的规范化。
2. embedded credentials、未知 scheme、相对歧义路径和非法 Asset URI 的拒绝。
3. File 平台 comparer、Storage/Bytes/Stream 有无 cacheKey/version 的共享边界。
4. header、Variant、CachePartition、reader/codec version 和 decode bucket 对 encoded/decoded key 的影响。
5. 错误、snapshot、event 和 file metadata 不泄漏 header、partition 或 URI user-info。

### 调度与两级合并

1. 同 encoded key、不同 decoded size 只读取一次并解码两次。
2. 同 decoded key 多 waiter 只读取/解码一次，返回不同结果租约。
3. Reload、NoStore、CacheOnly 和 Default 不发生非法跨模式合并。
4. Critical/High/Normal/Low/Preload 排队顺序、同优先级 FIFO、aging 和 preload 空闲启动。
5. 下载和 decode 各自不超过配置并发，取消排队项不打开资源。
6. 当前 Previewer 请求可以越过排队 preload，但不会中断仍被 waiter 共享的运行中操作。

### 取消与竞态

1. 取消一个 waiter 不取消其他 waiter；最后 waiter 取消才触发底层 CTS。
2. 完成与取消、取消与新 waiter、loader dispose 与完成同时发生时，每个 waiter 至多完成一次。
3. `Cancel()`、stream/response dispose、image dispose 和 completion callback 均在 lock 外执行；用可重入 fake 验证无死锁。
4. loader dispose 拒绝新请求、取消全部在途工作、清空 store/cache/event，且重复 dispose 幂等。
5. 启动 attach 中途失败按逆序只清理已成功服务，不留下可查询 loader。

### 缓存与租约

1. encoded/decoded memory 按 bytes 和 entries 两个上限独立触发 LRU。
2. 超大单项可以服务当前请求但不插入 cache。
3. decoded entry 被驱逐但有 lease 时延迟 dispose；最后 lease 释放恰好 dispose 一次。
4. 替换、Clear、partition clear 和 loader dispose 共用正确状态机。
5. borrowed `IImage` 在成功、失败、Source 替换、控件 detach 和 loader dispose 中都不被 AtomUI dispose。
6. file cache 原子写、并发同 key、损坏 metadata/body/hash、schema 升级、容量清理和崩溃残留恢复。
7. partition clear 取消目标在途请求并递增 epoch；清理前启动、清理后完成的 operation 不能重新写入 cache。

### HTTP 与安全

1. `ETag`、`Last-Modified`、304 metadata 合并、无旧 body 的 304 恢复。
2. `no-store`、`no-cache`、`max-age`、`Expires`、`must-revalidate`、`private`、`Vary` 和 `Vary:*`。
3. Authorization/Cookie 有无 partition、跨 partition、认证持久缓存默认禁止。
4. ResponseHeadersRead、Content-Length 预拒绝、chunked/压缩实际字节越界和取消释放。
5. redirect 次数、相对 Location、跨 origin 敏感 header 移除、HTTPS downgrade 拒绝。
6. MIME/Magic Bytes/codec probe 一致与不一致，HTML/XML/SVG polyglot 和未知格式拒绝。
7. 原始宽高、像素乘法溢出、decoded bytes 和动画容器上限在分配前生效。
8. HTTP SVG 即使注册 Asset SVG codec 仍拒绝；`avares` SVG 成功；File/Stream SVG 默认拒绝。

## 控件测试

`AsyncImage` 和 Avatar 共同验证：

- Source null 的 Idle、Loading/Loaded/Failed 派生属性和 pseudo-class 一致。
- Source 快速 A -> B，A 晚完成不能覆盖 B；A 结果被释放。
- detach/reattach、跨 TopLevel scaling、Bounds bucket 和 RequestOptions 变化的 generation 行为。
- fallback 至多一次、主失败后 fallback 成功只触发 opened、最终失败才触发 failed；取消不触发 fallback。
- progress 只属于当前 generation，未知长度不产生伪百分比。
- Reload 保留同来源旧图直到原子替换，但使用 Reload cache 语义。
- owned cache image 与 borrowed `FromImage` 的释放责任。
- Theme loading/error content 不改写 loader 状态，Template 清除不会遗留 controller 订阅。

Avatar 额外断言成功 Source、成功 FallbackSource、Text、Icon 的固定优先级，以及加载中继续显示 Text/Icon；本地 SVG 和
raster 使用同一个 Source 属性。`AsyncImage` 额外断言 Auto/Original/Explicit、Stretch 和 loading/error template。

## Previewer 测试

- `ItemsSource` 普通 enumerable 替换与 `INotifyCollectionChanged` 增删移替重置，配置 item 始终不可变。
- `ImagePreviewEntry` 的缩略图/完整图租约、generation 和状态不写回 `ImagePreviewItem`。
- Current 使用 Critical、cover 使用 High、邻项使用 Preload；CurrentIndex 改变后预加载窗口与距离顺序正确。
- host 显示时 clamp 不破坏外部 TwoWay CurrentIndex，集合恢复后可以重新解析调用方索引。
- `ThumbnailSource` 空时使用 Source 的缩略目标尺寸，打开当前项时仍可复用 encoded data、不能复用错误 decoded bitmap。
- 每项 FallbackSource 只作用于该项；一个 item 失败不替换全组。
- `ReloadCurrent()`、`ReloadItem()`、`ReloadCover()` 只影响目标 entry。
- dialog、overlay、cover 和 group host 关闭/切换时取消 waiter 并释放租约，无 window/visual retained reference。
- Title 优先使用 item.Title，resolver 收到 immutable item/index/count，异常不会破坏图片状态机。

## 平台、性能和泄漏验证

1. Desktop headless/真实窗口分别验证 HTTP、Asset、File、Storage、raster 和 trusted Asset SVG。
2. Browser WASM 运行验证 HTTP+CORS、Asset、Storage/Bytes、无 file cache、远程 SVG 拒绝和内存默认值。
3. Gallery Desktop NativeAOT publish/启动，验证 `AsyncImage`、Avatar 和 Previewer codec/reader 未被裁剪。
4. Browser AOT publish，验证 converter、registry、AXAML property 和 codec 静态保留；当前工具链下的 AOT 启动失败发生在
   managed `Main` 之前，不作为图片模块完成门禁，边界以
   [AOT 编程规范的当前 Browser AOT 结论](../../../engineering/development/aot-programming-guidelines.md#当前-browser-aot-结论)
   为准。
5. 10,000 次虚拟化/瀑布流 Source 复用压力测试，峰值 active download/decode、cache bytes 和最终基线受限。
6. `WeakReference` 验证卸载的 AsyncImage、Avatar、Previewer entry/window、已解除的 diagnostics/progress subscriber 和
   Application 可回收。
7. 慢下载、慢 decode、快速滚动和 scaling resize 下 UI dispatcher 无同步 I/O、无固定 10 ms delay、无 stale image flash。
8. cache hit/decoded reuse 的基准测试与无 cache 基线比较，确保 key/hash/lease 没有抵消收益。

## 文档与回归门禁

统一图片加载相关改动必须原子地保持以下门禁：

1. `AtomUI.Controls.Shared/ImageLoading/`、Core owned-service 生命周期、Controls `AsyncImage`/controller/Avatar 和 Desktop
   Previewer 的实现与本文档保持一致。
2. Avatar、ImagePreviewer、Masonry/Gallery 源码、Themes、tests、ShowCase、API 表和 Control 文档同步到新 API。
3. 仓库中 `AsyncImageLoader.Avalonia` package/reference/namespace/attached property 为零。
4. 已删除的旧 public API 在 source、AXAML、Gallery 和 tests 中为零；只允许 release breaking-change 文档和历史
   changelog 提到旧名称。
5. 不存在控件私有 HttpClient、loader、encoded/decoded cache、preview scheduler 或 10 ms 图片加载延迟。
6. 本目录内部相对链接、模块导航和启动/依赖文档一致；生成 LLMS 只能以当前源码和 Control 文档为输入重新生成。
7. Shared、Controls、Desktop、Gallery 图片相关测试、普通 Browser publish/运行验证、Desktop NativeAOT publish/启动和
   Browser AOT publish 全部通过；Browser AOT 启动仅在工具链边界解除后恢复为门禁。
8. `git diff --check`、文档 link check 和 public API baseline 检查通过。

推荐静态门禁：

```bash
rg -n "AsyncImageLoader|BitmapSrc|BitmapUrl|IImagePreviewSource|DefaultImageSourceLoader|LoadedImageSource" \
  src tests controlgallery
rg -n "Task.Delay\\(10\\)|new HttpClient|MaxConcurrentLoads" \
  src/AtomUI.Controls src/AtomUI.Desktop.Controls controlgallery
```

旧名在本架构的删除边界和门禁说明中出现是有意的，不能用无范围全仓零匹配替代按 source surface 的验收。
