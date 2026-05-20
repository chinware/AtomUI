# Space 性能优化

`Space` 是布局类基础控件，但当前 `SpaceShowCase` 不只是普通间距示例，它同时覆盖了 `CompactSpace`、Button group、输入控件组合、Picker、Select、Cascader、TreeSelect 等复合场景。

本目录记录 `SpaceShowCase` 的真实 Gallery baseline、Space / CompactSpace 体系的性能分析、优化方案和最终实测结果。

工具生成的 micro / Gallery 原始跑分输出、阶段计划和中间结论不入库；关键数据和最终判断已汇总到本 README。

## 当前结论

`SpaceShowCase` 打开慢是事实，但主要矛盾不是 8 个普通 `Space`，而是页面内 24 个 `CompactSpace` 和它们包装出的 wrapper，再叠加 63 个 Button、20 个 LineEdit/SearchEdit、7 个 Select 和 36 个 AddOnDecoratedBox。

本轮已完成 Phase 0-7：

- 新增 Space 控件级 suite 和 `--verify-space-states` 正确性验证。
- 修复 `CompactSpaceFiller` 校验对象错误，并移出 measure 热路径。
- 修复 `PointerExited` sender 判断和 remove / re-template 生命周期清理。
- 移除 `CompactSpaceItem` 三路 relay binding，改为代码直写 compact 状态。
- 缓存 overlap `TranslateTransform`，避免 measure 热路径重复分配。
- 收敛 Grid definitions 重建、LINQ 分配和状态通知。
- z-index 相关事件订阅按需化。
- `CompactSpaceFiller` 不再承担 `CompactSpaceItem` wrapper。
- 普通 `Space` 显式 spacing 后释放对应 token binding。

最终结论：结构和 allocation 有改善，真实 Gallery repeated navigation 有小幅改善，但 cold first navigation 没达到 `<500ms` 目标。剩余大头已经不在 `Space` 本体，而在 ShowCase 中大量输入控件、Button、Select/Pickers 和 AddOnDecoratedBox 的模板 materialization。

## Baseline 摘要

| 指标 | Baseline | Final | 变化 |
| --- | ---: | ---: | ---: |
| cold first navigation | `769.44ms` | `795.77ms` | `-3.42%` |
| repeated navigation mean | `165.32ms` | `150.48ms` | `+8.98%` |
| repeated navigation p95 | `196.46ms` | `168.76ms` | `+14.10%` |
| repeated allocation mean | `39791.91KB` | `39301.50KB` | `+1.23%` |
| runtime visuals | `1864` | `1846` | `-18` |
| runtime logical | `136` | `136` | `0` |
| runtime `Space` | `8` | `8` | `0` |
| runtime `CompactSpace` | `24` | `24` | `0` |
| runtime `CompactSpaceItem` | `93` | `75` | `-18` |
| runtime `Button` | `63` | `63` | `0` |
| runtime `LineEdit total` | `20` | `20` | `0` |
| runtime `Select` | `7` | `7` | `0` |
| runtime `AddOnDecoratedBox` | `36` | `36` | `0` |

`cold first navigation` 只有单样本，受 Debug/headless 首次资源加载、JIT 和系统状态影响较大；本轮不能宣称 cold 改善。更稳定的 repeated 20 轮数据表明控件级减重有效但幅度有限。

触发路径说明：当前 headless 工具对 `SpaceShowCase` 使用 `NavigateToCommand` fallback。`CaseNavigation` 的 `NavMenuItemClick` handler 最终也执行同一个 command，因此这个数据主要衡量 route 创建、XAML materialization、template/style/layout 稳定成本；不包含展开左侧 Layout 分组本身的交互成本。
