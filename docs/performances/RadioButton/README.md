# RadioButton 性能优化

> 状态：已完成 Gallery 实测；本轮追加 RadioButtonGroup checked 同步结构优化。

## 追加结构优化：checked sync 去 OfType

`AbstractRadioButtonGroup.SyncCheckedState()` 在 `CheckedItem` 外部变化时需要扫描 logical children 并同步 radio checked 状态。旧路径通过 `LogicalChildren.OfType<AbstractRadioButton>()` 过滤，本轮改为显式模式匹配，保留 source mode / direct child mode 的匹配逻辑。

正确性补充：旧同步逻辑只会把匹配项设为 checked，不会清理已经 stale 的 checked item，也不会在 `CheckedItem=null` 时清空 children；同时 `_ignoreSyncChecked` 可能让外部第一次切换 `CheckedItem` 被跳过。本轮移除该跳过分支，把同步结果收敛为 `shouldBeChecked`，匹配项设为 true，非匹配项设为 false。

| 指标 | 优化前 | 优化后 | 公式 | 提升 | 结论 |
| --- | ---: | ---: | --- | ---: | --- |
| RadioButtonGroup checked sync `OfType` LINQ operators / sync | 1 operator | 0 operators | `(1 - 0) / 1` | 100.00% | 有效；checked 同步不再创建 LINQ filter iterator |
| RadioButtonGroup stale checked state / CheckedItem change | stale item may stay checked | non-matching items cleared | correctness | 已修复 | 外部切换 / 清空 `CheckedItem` 时 checked 状态一致 |
| RadioButtonGroup external CheckedItem sync skip | first external change may be ignored | every change syncs containers | correctness | 已修复 | 不再用 `_ignoreSyncChecked` 跳过同步 |

说明：这是 checked 状态同步路径的结构性收益；不声明页面导航 timing 提升。
