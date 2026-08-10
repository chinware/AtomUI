# AtomUI 异步加载架构

异步加载系统为搜索类和展开类 Control 提供共享的请求时序、取消、超时和去重模型。公共协调器由
`AtomUI.Controls.Shared` 拥有，具体 Control 负责触发加载、投射 UI 状态和应用成功结果。

使用方式见 [异步加载使用指南](../../../guides/control-infrastructure/async-loading.md)。

## 设计原则

1. 搜索覆盖与节点展开是两种不同并发模型，不使用一个可配置巨型协调器表达。
2. 协调器只拥有请求时序，不直接修改 Control、集合或 UI 状态。
3. 所有调用都产生结构化终态，不以异常、null 或回调顺序表达结果。
4. timeout、外部取消、搜索覆盖和 loader fault 必须保持可区分。
5. 请求完成后由 Control 在正确线程更新状态和数据。
6. detach、owner 变化和 Control 释放必须具有明确取消路径。

## 公共模型

`AsyncLoadStatus` 定义五种终态：

| 状态 | 语义 |
| --- | --- |
| `Success` | loader 返回有效结果 |
| `Cancelled` | 外部取消或 owner 生命周期结束 |
| `TimedOut` | 请求超过配置的总时限 |
| `Faulted` | loader 抛出非取消异常 |
| `Skipped` | 搜索请求被更新请求覆盖，旧结果不得发布 |

`AsyncLoadOutcome<TResult>` 保存 `Status`、可选 `Result` 和可选 `Error`，并提供对应状态快捷属性。
`TResult` 为引用类型；Success 必须携带非空结果，Faulted 才携带原始异常。

## 搜索协调器

`AsyncSearchLoadCoordinator<TContext, TResult>` 面向高频输入：

- 每次调用取消前一个进行中的搜索。
- `DebounceInterval` 延迟 loader 启动；延迟期间被替换的调用返回 `Skipped`。
- `Timeout` 从 `LoadAsync` 调用开始计时，包含 debounce。
- `TContext` 允许 null，支持空搜索或清空输入场景。
- `Cancel()` 结束当前搜索，但不拥有 Control 的 loading 属性。

只有最新请求能够发布结果。旧请求即使底层 loader 忽略取消并最终返回，也不能越过请求版本边界覆盖新状态。

## 展开协调器

`AsyncExpandLoadCoordinator<TContext, TResult>` 面向树和级联节点：

- 通过 comparer 对同一 context 的在飞任务去重。
- 相同 context 的调用共享 Task 和终态。
- 不同 context 可以并发运行。
- 每个请求独立 timeout。
- `CancelAll()` 用于 Control detach、数据源替换或 owner 释放。

节点默认使用引用身份，避免两个值相等但实际属于不同树位置的对象错误共享请求。

## 状态所有权

```text
User interaction
  -> Control trigger
  -> shared coordinator
  -> loader
  -> AsyncLoadOutcome
  -> Control state/result projection
  -> public event and collection update
```

- Coordinator 拥有 debounce、timeout、linked cancellation 和 in-flight task map。
- Loader 拥有外部 I/O，并必须消费 CancellationToken。
- Control 拥有 `IsLoading`、item loading、`AsyncLoaded`、公开事件和集合修改。
- 应用只消费 Control 的公开结果类型，不依赖 coordinator 实现。

搜索请求的 `Skipped` 不触发公开完成事件，新请求继续拥有 loading 状态。展开请求成功后是否设置 `AsyncLoaded` 以及
如何合并子节点由具体 Control 决定。

## 生命周期与线程

- Search coordinator 在 Control 实例生命周期内复用，避免每次输入重新创建协调对象。
- Expand coordinator 的在飞 map 在终态后移除条目，不能永久保留节点。
- detach、数据 owner 切换和控件释放必须调用取消入口。
- loader 可以在任意线程完成；Visual、item 属性和可观察集合更新必须回到 UI 线程。
- 超时与外部取消通过 linked token 传播，不使用额外 timer callback 修改 UI。

## 性能与 AOT

- 正常路径不使用反射、动态成员访问或运行时类型扫描。
- 搜索只保留当前请求状态；展开只保留在飞 context。
- 去重 comparer 在协调器构造时确定，热路径不创建 comparer。
- 结果模型和 loader 调用均为强类型泛型契约。

## 兼容性

- 修改状态含义、timeout 是否包含 debounce、`Skipped` 发布规则或同 context 任务共享语义属于行为变更。
- Control 可以暴露不同属性名和默认 timeout，但必须把终态稳定映射到自身公开结果模型。
- 新 Control 不能通过吞异常、静默 timeout 或私有 fire-and-forget Task 绕过共享终态模型。

## 验证要求

1. 搜索 debounce 期间覆盖返回 `Skipped`。
2. loader 已启动后，新搜索取消旧请求且旧结果不能发布。
3. timeout 与外部取消映射为不同状态。
4. 同一展开 context 只执行一个 loader，不同 context 可以并发。
5. fault 保留原始异常且不会泄漏未观察 Task。
6. detach 和 owner 切换取消在飞请求并释放 context。
7. UI 状态和集合更新发生在 UI 线程。
8. NativeAOT 下不依赖反射或动态代码。
