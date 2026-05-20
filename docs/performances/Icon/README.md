# Icon 性能优化

`Icon` 是 AtomUI 最基础、最高频的视觉控件之一。它出现在 Button、Input、Select、Menu、NavMenu、Tabs、Feedback、Data Display 等大量控件中，单个实例成本不高时也会被批量模板放大。

本目录记录 `Icon` 体系的性能分析、优化计划、基线与最终结果。当前阶段已完成 Phase 6 最终验证；generated geometry metadata 与 transform literal 策略已同步到 AntDesign、Material、IconPark 三个图标包。

阶段中间结果和逐次 sample 原始输出不入库；需要时使用文档中的复现命令重新生成。

## 当前结论

`Icon` 当前有明确优化空间，主要不是单点算法慢，而是基础 Control 成本、模板隐藏节点、默认初始化成本和 render 热路径分配在高频使用场景中叠加。

最终 `IconShowCase` repeated navigation 从 baseline `193.31ms` 到 `173.36ms`，提升 `10.3%`；分配从 `57900.83KB` 到 `46331.65KB`，下降 `20.0%`。Phase 3 曾测得 repeated `149.08ms`，说明 headless timing 存在明显波动，但 allocation 和结构性减重是稳定收益。外部 `MaterialIconsPackages` 与 `IconParkIconsPackage` 已生成同类静态 metadata，分别覆盖 10751 / 2658 个 generated icon class。

Phase 4 已处理 `SelectHandle`、`MenuItem`、`ToggleIconButton`、`NavMenuItemHeader`、`Button` 的隐藏 icon slot。`Select.Default` micro 场景中默认路径从 `Icon/root 3` 降到 `1`、`Button/root 1` 降到 `0`，KB/item 从 baseline `594.7` 降到最终 `427.6`；`Button.Default` 默认路径不再创建 `LoadingOutlined`。真实 Gallery 已补跑 `ButtonShowCase`、`SelectShowCase`、`MenuShowCase` 的结构观测，89 个 Button 不再带来 89 个默认 loading icon，33 个 Select 不再承担 loading/search/clear 的默认隐藏控件。该阶段主要收益是结构性减重，Gallery timing 仍按多轮样本判断。

Phase 5 已补齐 `IconProviderCache.ClearCache()` 对 `TypeToCreator` 的清理语义，并把 AntDesign/Material/IconPark generated transform 从运行时 `TransformParser.Parse("...")` 改为生成期 `Matrix` literal。Provider micro 当前为 `0.065ms/item`，不是主导瓶颈，因此 enum -> factory switch 暂不实施。

后续继续沿用的方向：

- 不使用动画、不使用多色、不使用 stroke 的静态图标不应创建完整 transition / pen 成本；本轮已改为按需创建。
- `Icon` 自绘后不再保留默认 `Border` 模板节点；micro direct icon 已从 `Visual/root 2` 降到 `1`。
- render 热路径已避免每帧分配 `MatrixTransform`，也不再修改静态共享 geometry。
- Select/Menu/NavMenu/ToggleIconButton/Button 等高频模板里的隐藏 Icon 已改为按需创建或单 slot 状态切换；IconButton 暂未发现常驻 hidden loading slot。
- 所有新增缓存必须只缓存不可变元数据、geometry、bounds 或 factory，不缓存 Control 实例。

兼容性边界：

- `IconParkIconsPackage` 依赖 `Icon` 基类的多色、stroke、`IconTheme`、`ProcessBrush()` 与 `FindIconBrush()` 扩展点。
- `MaterialIconsPackages` 生成大量继承 `MaterialIcon` 的主题类，`MaterialIcon` 继续继承 `Icon`，依赖现有 provider、theme 和 drawing instruction 模型。
- 因此 Icon 优化的边界是“按需付费”，不是删掉基类能力。
