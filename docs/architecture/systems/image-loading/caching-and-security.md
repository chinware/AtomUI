# 图片缓存、HTTP 与内容安全

> 状态：当前架构。本文定义统一 loader 的缓存一致性和不可信内容边界；任何控件都不得降级这些规则。

## 来源索引与内容存储

| 层 | 键与内容 | 默认状态 | Owner |
| --- | --- | --- | --- |
| source snapshot index | `ImageSourceKey -> ImageSourceVersion -> ImageContentId` 与 validator、freshness、commit generation | memory 启用；可选持久化 | `ImageSourceSnapshotIndex` / `ImageFileCache` |
| encoded content | `(partition hash, ImageContentId)` 对应已验证的精确编码字节和内容 metadata | memory 启用；持久层默认关闭 | `ImageEncodedCache` / `ImageFileCache` |
| decoded content | `(partition hash, ImageContentId, ImageDecodeSpec)` 对应 raster 或受限静态 SVG 绘制对象、租约计数 | memory 启用 | `ImageDecodedCache` |

来源地址不是内容身份。`ImageSourceKey` 只描述如何再次访问来源；来源验证先产生 `ImageSourceVersion`，读取到的新正文再以
SHA-256 产生 `ImageContentId`。只有此后才能形成 `ImageDecodeKey` 并查询 decoded store。任何根据旧 SourceKey 直接返回 decoded
image 的捷径都违反一致性契约。相同字节即使来自不同路径或 URI，也可在同一 cache partition 下共享 encoded/decoded 内容；
相同路径的不同字节一定得到不同 ContentId。

缓存同时受最大字节数和最大条目数约束，任一到达上限都触发 LRU 驱逐。单条目大于该层字节预算时可以返回给当前 waiter，
但不得插入该层。容量统计使用实际 encoded buffer 大小；raster decoded 使用 codec 报告值，无法报告时按
`pixelWidth * pixelHeight * 4` 的 checked 上界估算。SVG decoded 成本使用主文档字节、嵌入 raster decoded estimate、元素、
属性和 path/reference complexity 的 checked 估算，并设置最小非零成本；不得用 `viewBox area * 4` 把矢量画布当成像素缓冲，
也不得按对象数量把复杂矢量模型视为零成本。

默认预算如下；应用可以在 `UseImageLoading()` 中降低或提高，但不能超过平台可表示范围，也不能把安全上限设为零来表示
“无限”：

| 平台 | encoded memory | decoded memory | 单响应 | 持久缓存 |
| --- | --- | --- | --- | --- |
| Desktop | 128 MiB / 256 项 | 256 MiB / 256 项 | 32 MiB | 关闭；启用后默认 512 MiB / 2048 项 |
| Browser | 32 MiB / 128 项 | 64 MiB / 128 项 | 16 MiB | 不可用 |

### 租约感知驱逐

decoded entry 有 cache membership、active operation reference 和 active result lease 三类持有。普通 cache hit 必须在 cache lock
内原子建立 result lease；decoded in-flight 的二次 cache hit 必须在同一临界区建立 operation reference，不能先返回裸 entry 再在
lock 外 retain。LRU 驱逐先原子地从 key map/LRU 移除 membership，使后续请求不能再获取该 entry；operation reference 和 lease
均为零时立即 dispose 图片，否则延迟到最后一个引用释放。已驱逐 entry 不得因为旧 operation 或 lease 尚在而重新进入 cache。

替换相同 key、清空缓存和 loader dispose 都走同一状态机。图片 dispose、codec release callback、operation release 和租约完成
回调必须在 cache lock 外执行。decode scheduler 把新 entry 交给 pipeline 后，pipeline 必须先建立 operation reference；其后的
取消、cache insert 拒绝或异常都释放该引用，不能遗留无 owner 的 decoded image。`new BorrowedImageSource(IImage)` 是 borrowed
特例：cache 不获得 membership ownership，结果释放永远不 dispose 调用方对象。

`ClearCacheAsync` 对目标分区/层递增 cache epoch，并默认取消相同 scope 的 encoded/decoded in-flight operation。operation
完成写入前必须比较创建 epoch；旧 epoch 只能把结果交给仍允许完成的直接 waiter，不能重新污染已清理 cache。清理不强制
销毁控件正在持有的租约。

encoded entry 的 byte owner 只属于 cache 或当前 pipeline。source snapshot 只有在所引用 encoded content 已成功写入后才能提交；
每个 SourceKey 的单调 commit generation 阻止较早完成的刷新覆盖较新映射。file cache 读取必须先验证 metadata 格式、长度、
内容摘要、ContentId 和集中定义的安全策略版本，任何不一致都按 miss 删除，不能把损坏
或旧策略条目送入 codec。当前 `ImageSecurityPolicy.Version` 为 `2`；reader raw content 使用 `0`，完整验证后才标记为 `2`。
memory encoded entry 同样记录安全策略版本；策略版本变化时必须重新验证完全相同的不可变字节，重新验证失败即移除，不能依赖
旧验证结论。

## 缓存读取与存储策略

读取策略和存储位置是两个正交维度：

| `ImageCacheReadPolicy` | 现有 snapshot | 来源访问 | 结果验证状态 |
| --- | --- | --- | --- |
| `ValidateSource` | 只有按来源规则确认仍有效后才能复用 | freshness/version 不足时访问 | `Current`、`Revalidated` 或 `NotRequired` |
| `RefreshSource` | 不直接接受旧 snapshot | 始终访问来源；HTTP 可发条件请求 | `Revalidated` 或新内容的 `Current` |
| `PreferCache` | 有完整 snapshot + content 即接受 | 仅 cache miss 时访问 | cache hit 为 `Unverified` |
| `CacheOnly` | 有完整 snapshot + content 即接受 | 禁止访问原始来源 | cache hit 为 `Unverified`，miss 为 `CacheMiss` |

| `ImageCacheStoragePolicy` | 读取共享 cache | memory 写入 | persistent 写入 |
| --- | --- | --- | --- |
| `None` | 允许，除非认证隔离规则禁止 | 否 | 否 |
| `Memory` | 允许 | 是 | 否 |
| `MemoryAndDisk` | 允许 | 是 | 仅来源具备可持久身份且应用启用 file cache 时 |

`CacheStorage=None` 不是“强制回源”；它只禁止本次结果进入 store。需要强制访问来源时使用
`CacheRead=RefreshSource`。从 persistent 读取的命中在通过当前安全策略校验前不能进入 memory；`CacheStorage=None` 的命中
不得执行这种 read-through 提升。错误结果和 4xx/5xx 不做负缓存；控件的 `Reload()` 仅为一次请求覆盖
`CacheRead=RefreshSource`，不修改调用方保存的 `ImageRequestOptions`。

本地 Asset/File/Storage 的版本判断由 source reader 提供。File 的 `Metadata` 模式在同一已打开句柄上取得长度并读取正文，
以 creation/length/last-write token 判断已有 snapshot；`ContentHash` 模式每次读取并计算正文摘要，用于元数据可能不变的原子
替换场景。Asset 在单个 Application resource identity 内视为不可变；StorageFile/Stream 只有调用方提供稳定 revision 才跨实例
复用或进入持久缓存。`PreferCache` 和 `CacheOnly` 明确允许 stale snapshot，因此返回 `Unverified`，不能与默认
`ValidateSource` 的一致性保证混淆。

## HTTP 缓存一致性

HTTP transport 使用 `HttpCompletionOption.ResponseHeadersRead`。`HttpResponseMessage` 拥有响应 stream，pipeline 在完整读取、取消、
校验失败或异常时按确定顺序释放 stream/response；控件永远拿不到网络 stream。

HTTP entry 保存 `ETag`、`Last-Modified`、`Date`、`Age`、`Expires`、解析后的 `Cache-Control`、`Vary` 字段集合、接收时间、
媒体类型、内容摘要和安全策略版本。规则如下：

- `no-store`：不写任一缓存。
- `no-cache`：允许存储，但每次复用前必须条件请求。
- `max-age`/`s-maxage`：应用私有 cache 使用 `max-age`；不把 shared-cache 的 `s-maxage` 当成放宽依据。
- `must-revalidate`：过期后不得在网络错误时返回 stale 内容。
- `private`：允许当前 Application 私有缓存；不得跨 Application 或分区共享。
- 无显式 freshness：使用保守的 immediately-stale 策略，有 validator 就重验证，没有 validator 就重新下载，不自造长期 TTL。
- `Vary: *`：不缓存；其他 `Vary` 字段记录请求 header 摘要，字段值不匹配视为 miss。

重验证优先使用 `If-None-Match`，其次 `If-Modified-Since`。`304 Not Modified` 必须合并规范允许更新的 metadata，保留原 body，
重新执行当前安全策略版本校验，并以原内容 `Origin` 加 `SourceValidation=Revalidated` 返回。304 没有可用旧 body 属于协议/缓存损坏，重新完整请求
一次后仍异常才失败，不能构造空图片。

`RefreshSource` 表示强制重验证/重新获取，不等同 `CacheStorage=None`。服务端返回 `no-store` 时必须在 source generation 与 cache
epoch 仍为当前值时清除已有相同 source、encoded 和 decoded variant；persistent 清理与写入串行化，迟到的 `no-store` 响应不能
删除更新 snapshot。

## 认证与分区

以下 header 被视为认证上下文：`Authorization`、`Proxy-Authorization`、`Cookie`，以及应用在全局配置中声明的自定义认证
header。存在认证上下文但 `ImageRequestOptions.CachePartition` 为空时，loader 强制 `CacheStorage=None`、禁止读取共享 cache，
且禁止与其他请求合并。

存在 partition 时，partition 的不可逆摘要进入 SourceKey、encoded content key 和 DecodeKey；原值不写日志、文件名或 metadata。
所有自定义请求 header 的名称和值摘要都进入第一次 source-resolution identity，避免在服务器返回 `Vary` 之前错误合并。持久化认证响应还需应用显式启用
`AllowAuthenticatedPersistentCache`，其默认值为 false；启用后 metadata 只保存摘要，不保存认证值。

Application 之间永不共享内存或文件 cache 实例。多租户应用必须为用户/租户切换新的 `CachePartition`；注销时通过
`IImageLoader.ClearCacheAsync(ImageCacheClearRequest)` 清除该 partition，控件不提供清缓存按钮或 partition 推断。

## URI 与重定向

- HTTP source 只接受 `http` 和 `https`，拒绝 embedded credentials、空 host、无效 IDN 和超出配置的 URI 长度。
- Desktop 禁用 handler 自动重定向，逐跳解析相对 `Location`、重新校验 scheme/host，并限制最大次数；默认上限 8。
- `https` 到 `http` 的降级始终拒绝，不提供控件或 per-request 绕过开关。
- 重定向后的每个请求重新应用认证 header 转发策略；跨 origin 不转发 Authorization、Cookie 等敏感 header，除非应用级
  allowlist 明确允许目标 origin。
- Browser 的中间跳转受 Fetch/CORS/mixed-content 策略控制；transport 校验可观测的初始和最终 URI。平台不能暴露中间跳时，
  不声称完成 Desktop 式逐跳审计，详细能力矩阵见 [平台、性能与 AOT](platforms-and-aot.md)。

Loader 是显式 URL 消费方，不擅自封禁 loopback 或私网地址；需要 SSRF 防护的宿主应在应用级 origin allowlist 中收紧。
allowlist 检查发生在 DNS/请求前和每次可观测重定向后，不能由 Control 覆盖。

HTTP 只发起无 body 的 GET。`Host`、`Content-Length`、`Connection`、`Transfer-Encoding`、`Upgrade`、`Range` 和 proxy
控制 header 不能通过 `ImageRequestOptions.Headers` 覆盖；名称和值在请求快照阶段验证。Desktop handler 关闭自动 Cookie
container 并忽略 `Set-Cookie`；Browser transport 默认使用 Fetch `credentials=omit`。需要鉴权时使用显式 Authorization/
业务 header 与 `CachePartition`，不让 ambient cookie 在调用方不可见时进入共享 cache key。

## 内容验证

所有编码内容必须在 codec 构建完整 raster buffer 或 SVG 绘制模型前通过分层一致性检查：

1. 响应声明：HTTP status、Content-Length 和规范化 MIME。
2. 格式探测：raster 使用有界 Magic Bytes；markup 候选进入安全 XML reader，不按字符串片段猜测 SVG。
3. 格式验证：raster codec header probe 解析宽高、帧与预计解码成本；`SvgContentValidator` 解析完整 XML/CSS、引用图、
   data raster 和复杂度预算。
4. codec 选择：只有携带当前安全策略验证 metadata 的内容才能匹配唯一显式 codec。

允许 `application/octet-stream` 或缺失 MIME 在实际内容验证一致时加载；声明为具体 `image/png` 但内容实际为 JPEG/SVG 属于
`ContentTypeMismatch`。SVG 实际内容允许 `image/svg+xml`、`application/xml`、`text/xml`、`application/octet-stream` 或缺失
MIME；`text/html`、`application/xhtml+xml`、非 SVG XML 和 HTML/XML/SVG polyglot 始终拒绝。

网络、File、Storage、Bytes、Stream 和 Asset SVG 使用同一受限静态子集。验证器拒绝 DTD/entity、script、任意 `on*`、
`foreignObject`、`xml:base`、外部 image/use/filter/paint/stylesheet/font URI、危险 CSS 和递归 `data:image/svg+xml`；只允许
同文档 `#id` 引用、内联 CSS、内联 SVG font 和预算内的 `data:image/png|jpeg|webp`。Asset 来源同样不能触发 renderer 隐式 I/O。
完整规则和错误映射见[网络 SVG 加载设计](network-svg.md)。

## 资源上限

在分配完整编码缓冲和像素缓冲前后都检查：

| 上限 | Desktop 默认 | Browser 默认 |
| --- | --- | --- |
| 原始宽或高 | 16,384 px | 8,192 px |
| 原始总像素 | 64,000,000 | 32,000,000 |
| 估算/实际 decoded bytes | 256 MiB | 128 MiB |
| animation frame count | 只允许 1；声明或探测到多帧即拒绝 | 同 Desktop |

乘法使用 checked/饱和检查，不能让整数溢出绕过限制。即使请求目标尺寸很小，codec probe 仍先限制原始维度与格式元数据，防止
畸形 header；随后按目标尺寸再次估算实际 decoded bytes。超限错误不得 fallback 到“试着解码”。

读取循环同时检查 Content-Length 和实际累计字节，处理 chunked、压缩和错误长度。达到上限时立即取消/关闭响应，不继续把剩余
内容读入内存。压缩传输的限制以解压后的实际响应字节为准。

SVG 主文档还受 `ImageLoadingOptionsBuilder.Svg` 的文档字符、元素、属性、深度、path 字符、引用深度、data image 数量和累计
字节预算约束；主文档字节上限取 `MaxResponseBytes` 与 `Svg.MaxDocumentBytes` 的较小值。配置只能调整资源预算，不能允许脚本、
DTD 或外部资源。嵌入 raster 继续受本表的宽高、像素和 decoded-byte 上限约束。

## 持久缓存

`ImageFileCache` 必须完整实现但默认关闭。默认根目录名固定为 `image-cache`，不带版本后缀。启用时的稳定布局为：

```text
image-cache/
├── manifest
├── sources/<partition-hash>/<source-key-hash>.meta
├── content/<partition-hash>/<content-prefix>/<content-id>.bin
├── content-metadata/<partition-hash>/<content-prefix>/<content-id>.meta
├── locks/
└── temp/
```

规则如下：

- 目录由应用显式配置，或使用
  `<LocalApplicationData>/<Application 程序集名>/image-cache/<应用身份摘要>/`；程序集名无法解析时才使用
  `AtomUI`。默认目录绝不使用工作目录。
- `manifest` 中的 `formatRevision` 只是 internal 存储格式号；格式不匹配时丢弃并重建已知 store，不暴露迁移 API。
- content 文件只按 partition 与 `ImageContentId` 寻址；source metadata 单独保存 SourceKey、SourceVersion、ContentId、validator、
  freshness 和 commit generation。
- 文件名只包含身份摘要；metadata 使用 AOT-safe 的显式二进制序列化格式。
- metadata 保存集中定义的 `ImageSecurityPolicy.Version` 和格式验证 metadata；版本不匹配的条目删除后按 miss 处理。
- content/metadata 写入采用临时文件、flush、原子 replace/rename；只有 content 已可读取后才能发布 source snapshot。
- 一个跨进程 lock 保护同 key 写入；竞争失败可以回退网络，不允许读半文件。
- lock 名称按 content/source key 摘要稳定存在，不能在释放句柄后无条件删除：在 Unix 上这会产生删除后重新创建同名 lock 的
  TOCTOU 窗口并破坏跨进程互斥。lock 文件不计入 encoded byte/entry 限额，启动恢复只清理临时文件和孤儿 bin/meta；
  宿主若长期生成大量历史 key，应把持久缓存目录按应用/租户生命周期整体淘汰，而不是由 loader 删除可能仍被其他进程使用的 lock。
- LRU index 可重建，index 损坏不能使整个 loader 启动失败。
- 清理遵守 byte/entry 双上限，删除失败记录 diagnostics 并继续，不阻塞 UI 线程。
- Browser 构建不注册 file-cache implementation；即使配置启用也在启动阶段以明确 diagnostics 失败，而不是静默退化。
