# AddOnDecoratedBox 性能优化

`AddOnDecoratedBox` 是输入类控件共享的视觉和 addon 抽象。本目录记录本轮围绕“不使用的功能不承担成本”的控件级优化。

## 文档

| 文档 | 内容 |
| --- | --- |
| [addon-decorated-box-performance-plan.md](addon-decorated-box-performance-plan.md) | 完整方案、风险评估和实施任务 |
| [addon-decorated-box-baseline.md](addon-decorated-box-baseline.md) | 优化前控件级基线 |
| [addon-decorated-box-final.md](addon-decorated-box-final.md) | 最终性能结果 |

阶段中间结果不单独入库；关键过程、决策和最终数据已汇总到方案与最终结果文档。

## 最终结果摘要

| 场景 | 关键改善 |
| --- | --- |
| `LineEdit.Default` | Visual/root `26 -> 20`，KB/item `503.8 -> 398.4`，icon scan `360 -> 0` |
| `CompactSpace.LineEdit.Horizontal` | Visual/root `87 -> 73`，icon scan `1080 -> 240` |
| `SearchEdit.Default` | icon scan `840 -> 0` |
