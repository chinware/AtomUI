# RadioButton 性能优化

> 状态：已完成 Gallery 实测；本轮追加 RadioButtonGroup checked 同步和 group 注册结构优化。

## 追加结构优化：checked sync 去 OfType

`AbstractRadioButtonGroup.SyncCheckedState()` 在 `CheckedItem` 外部变化时需要扫描 logical children 并同步 radio checked 状态。旧路径通过 `LogicalChildren.OfType<AbstractRadioButton>()` 过滤，本轮改为显式模式匹配，保留 source mode / direct child mode 的匹配逻辑。

正确性补充：旧同步逻辑只会把匹配项设为 checked，不会清理已经 stale 的 checked item，也不会在 `CheckedItem=null` 时清空 children；同时 `_ignoreSyncChecked` 可能让外部第一次切换 `CheckedItem` 被跳过。本轮移除该跳过分支，把同步结果收敛为 `shouldBeChecked`，匹配项设为 true，非匹配项设为 false。

| 指标 | 优化前 | 优化后 | 公式 | 提升 | 结论 |
| --- | ---: | ---: | --- | ---: | --- |
| RadioButtonGroup checked sync `OfType` LINQ operators / sync | 1 operator | 0 operators | `(1 - 0) / 1` | 100.00% | 有效；checked 同步不再创建 LINQ filter iterator |
| RadioButtonGroup stale checked state / CheckedItem change | stale item may stay checked | non-matching items cleared | correctness | 已修复 | 外部切换 / 清空 `CheckedItem` 时 checked 状态一致 |
| RadioButtonGroup external CheckedItem sync skip | first external change may be ignored | every change syncs containers | correctness | 已修复 | 不再用 `_ignoreSyncChecked` 跳过同步 |

说明：这是 checked 状态同步路径的结构性收益；不声明页面导航 timing 提升。

---

## 追加结构优化：group 首项列表容量

`RadioButtonGroupManager.Add()` 在首次遇到某个 `GroupName` 时会创建弱引用列表并立即加入当前 radio。本轮把新建列表容量从默认动态增长改为 `1`，避免第一个 `Add()` 的列表增长；group key、弱引用、移除清理和互斥 checked 逻辑不变。

| 指标 | 优化前 | 优化后 | 公式 | 提升 | 结论 |
| --- | ---: | ---: | --- | ---: | --- |
| RadioButton group first add list growth / new group | dynamic growth | capacity 1 | structural | 结构收益 | 新 group 第一个 radio 避免列表首次增长 |
| RadioButton group registration semantics | unchanged | unchanged | n/a | 0.00% | 行为保持 |

说明：这是 RadioButton group 注册路径 structural-only 收益；没有新增页面 timing 对比，不声明页面加载速度提升。
