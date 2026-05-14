# Icon 性能优化

`Icon` 是 AtomUI 最基础、最高频的视觉控件之一。它出现在 Button、Input、Select、Menu、NavMenu、Tabs、Feedback、Data Display 等大量控件中，单个实例成本不高时也会被批量模板放大。

本目录记录 `Icon` 体系的性能分析、优化计划、基线与最终结果。当前阶段只形成方案，不修改实现。

## 文档

| 文档 | 内容 |
| --- | --- |
| [icon-performance-plan.md](icon-performance-plan.md) | Icon 架构分析、性能问题清单、验证口径和实施任务 |

后续基线、阶段结果和最终结果会继续放在本目录。逐次 sample 原始输出不入库；需要时使用文档中的复现命令重新生成。

## 当前结论

`Icon` 当前有明确优化空间，主要不是单点算法慢，而是基础 Control 成本、模板隐藏节点、默认初始化成本和 render 热路径分配在高频使用场景中叠加。

优先级最高的方向：

- 不使用动画、不使用多色、不使用 stroke 的静态图标不应创建完整 transition / pen 成本。
- `Icon` 自绘后应评估是否仍需要默认 `Border` 模板。
- render 热路径应避免每帧分配 `MatrixTransform`，也应避免修改静态共享 geometry。
- Select/Menu/NavMenu/Button 等高频模板里的隐藏 Icon 应改为按需创建或单 slot 状态切换。
- 所有新增缓存必须只缓存不可变元数据、geometry、bounds 或 factory，不缓存 Control 实例。
