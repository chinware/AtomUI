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
