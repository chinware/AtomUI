# LineEdit 性能优化

本目录记录 `LineEdit` 及其 Gallery 页面 `LineEditShowCase` 的性能数据。`LineEdit` 的控件级优化主要来自 `AddOnDecoratedBox` 体系和右侧 accessory 按需创建。

逐次 sample 原始输出和阶段中间文档不入库；关键 Gallery 对比数据已汇总到本 README。

## 当前结论

| 指标 | 未优化 | 优化后 | 改善 |
| --- | ---: | ---: | ---: |
| 冷首开耗时 | 830.00 ms | 732.75 ms | -11.72% |
| 重复导航均值 | 229.07 ms | 183.98 ms | -19.68% |
| 重复导航 P95 | 263.45 ms | 207.98 ms | -21.05% |
| Visual 数量 | 2,967 | 2,553 | -13.95% |

## 页面一致性

测量加载的是 Gallery 真实页面 `controlgallery/AtomUIGallery/ShowCases/Views/DataEntry/LineEditShowCase.axaml`。

| 来源 | LineEdit direct | SearchEdit | LineEdit total | TextArea | ShowCaseItem |
| --- | ---: | ---: | ---: | ---: | ---: |
| XAML 源文件 | 52 | 30 | 82 | 11 | 16 |
| 运行时视觉树 | 52 | 30 | 82 | 11 | 16 |
