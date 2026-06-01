# Form 性能优化

> 状态：已完成低风险结构与生命周期优化；本轮追加验证路径 structural-only 容量收敛。

## 追加结构优化：验证集合容量

`Form.ValidateAsync()` 和 `FormItem.ValidateValueAsync()` 的任务、结果和消息集合按 `Items.Count` / `Validators.Count` 预分配。验证顺序、并发/串行策略、错误/警告消息内容均保持不变。

| 指标 | 优化前 | 优化后 | 公式 | 提升 | 结论 |
| --- | ---: | ---: | --- | ---: | --- |
| Form validate tasks list / validate | dynamic growth | exact upper-bound capacity | structural | 分配更紧 | 按 `Items.Count` 预分配 |
| Form validate results list / validate | dynamic growth | exact upper-bound capacity | structural | 分配更紧 | 按 `Items.Count` 预分配 |
| Form validate messages list / validate | dynamic growth | exact upper-bound capacity | structural | 分配更紧 | 按 `Items.Count` 预分配 |
| FormItem validator task dictionary / parallel validate | dynamic growth | exact upper-bound capacity | structural | 分配更紧 | 按 `Validators.Count` 预分配 |
| FormItem warning/error message lists / validate | dynamic growth | exact upper-bound capacity | structural | 分配更紧 | 按 `Validators.Count` 预分配 |

说明：这是交互验证路径的结构性收益；不声明页面加载 timing 提升。

## 追加结构优化：ValidateAsync error result fast path

`Form.ValidateAsync()` 旧路径会把每个 `FormItem.ValidateResult` 收集进临时 `List<FormValidateResult>`，再用 `Any(predicate)` 判断是否存在错误。本轮改为在收集验证消息的同一轮扫描中维护 `hasError`，错误 / 警告消息内容和 `Validated` 事件语义保持不变。

| 指标 | 优化前 | 优化后 | 公式 | 提升 | 结论 |
| --- | ---: | ---: | --- | ---: | --- |
| Form validate results list allocations / validate | 1 list | 0 lists | `(1 - 0) / 1` | 100.00% | 有效；不再为结果归并创建临时 list |
| Form validate error check LINQ operators / validate | 1 `Any(predicate)` | 0 LINQ operators | `(1 - 0) / 1` | 100.00% | 有效；错误判断并入消息收集循环 |

说明：这是表单验证交互路径的结构性收益；不声明页面导航 timing 提升。

## 追加结构优化：Items collection handler

`Form` 构造器里 `Items.CollectionChanged` 原先使用匿名 lambda 调用 `InvalidateMeasure()`。本轮改为命名 method-group handler，行为不变，但每个 `Form` 实例不再创建匿名 handler，后续排查订阅链也更清晰。

| 指标 | 优化前 | 优化后 | 公式 | 提升 | 结论 |
| --- | ---: | ---: | --- | ---: | --- |
| Form `Items.CollectionChanged` anonymous handler / instance | 1 | 0 | `(1 - 0) / 1` | 100.00% | 有效；改为 `HandleItemsCollectionChanged` method group |
| Items change measure invalidation behavior | `InvalidateMeasure()` | `InvalidateMeasure()` | n/a | 0.00% | 行为保持不变 |

验证说明：`AtomUI.Performance` 构建通过。当前 `--verify-form-states` 仍有历史断言/期望失败，本轮不把该 suite 作为收益证明。

## 追加结构优化：error message inline last lookup

`FormItem.BuildErrorMessageInlines()` 在 warning message 拼接前需要判断当前最后一个 inline 是否已经是 `LineBreak`。旧实现使用 `inlines.Last()`；本轮在 `inlines.Count > 0` 保护下改为 `inlines[inlines.Count - 1]`，错误/警告消息顺序和换行规则保持不变。

| 指标 | 优化前 | 优化后 | 公式 | 提升 | 结论 |
| --- | ---: | ---: | --- | ---: | --- |
| FormItem warning inline LINQ `Last()` calls / warning message | 1 | 0 | `(1 - 0) / 1` | 100.00% | structural-only；最后一个 inline 直接按 index 读取 |
| Error/warning line-break semantics | unchanged | unchanged | n/a | 0.00% | 行为保持；只在需要时插入分隔换行 |
| Page-load timing claim | none | none | n/a | n/a | 本轮没有有效前后 timing，不声明页面级速度收益 |

验证补充：复跑 `--verify-form-states` 仍失败在既有 feedback control、tooltip icon、debounce timer 和 detach token source 断言；本轮只改错误/警告消息 inline 集合的最后一项读取方式，不改变这些生命周期路径，因此该失败不作为本轮结构优化引入的新回归。
