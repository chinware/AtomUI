# AddOnDecoratedBox 性能优化

`AddOnDecoratedBox` 是输入类控件共享的视觉和 addon 抽象。本目录记录本轮围绕“不使用的功能不承担成本”的控件级优化。

阶段中间结果、一次性计划和原始跑分不单独入库；关键过程、决策和最终数据已汇总到本 README。

## 最终结果摘要

| 场景 | 关键改善 |
| --- | --- |
| `LineEdit.Default` | Visual/root `26 -> 20`，KB/item `503.8 -> 398.4`，icon scan `360 -> 0` |
| `CompactSpace.LineEdit.Horizontal` | Visual/root `87 -> 73`，icon scan `1080 -> 240` |
| `SearchEdit.Default` | icon scan `840 -> 0` |

## Follow-up 实施摘要

2026-05-14 已按 follow-up 复评扩展到 `TextArea`、`Select` / `TreeSelect` / `Cascader`、`ComboBox`、Picker 系列和 `AddOnDecoratedBox` slot presenter。默认 `LineEdit.Default` 当前为 Visual/root `16.0`、ContentPresenter/root `1.0`、StackPanel/root `0.0`；`SearchEdit.Default` 保留搜索按钮等专用 part 后为 Visual/root `30.0`、ContentPresenter/root `5.0`、StackPanel/root `0.0`。accessory 生命周期、effective brush、addon state 三组验证均通过。
