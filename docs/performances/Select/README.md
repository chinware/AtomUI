# Select 性能优化

`Select` 是 AtomUI 高频数据录入控件，同时也是一组控件的基础体系：

- 直接继承 `AbstractSelect`：`Select`、`TreeSelect`、`Cascader`
- 同构体系：`ComboBox`、`AutoComplete`、`InfoPickerInput`
- 间接受影响场景：`SpaceShowCase` 的 `CompactSpace` 表单组合、Form、Picker 类控件、Gallery DataEntry 页面

当前结论：`Select` 问题严重，主要不是单个算法慢，而是默认关闭状态就创建了大量只在打开下拉、loading、clear、multiple/tags、responsive max tag、自定义 accessory 等场景才需要的对象、模板和订阅。

## 当前状态

| 项 | 状态 |
| --- | --- |
| 真实 Gallery 基线 | 已记录 `SelectShowCase` / `SpaceShowCase` item 7 / `TreeSelectShowCase` / `CascaderShowCase` |
| 控件级基准 | 已新增 `tools/performances/AtomUI.Performance/Suites/Select` |
| 优化方案 | Phase 0-9 已执行，ComboBox/AutoComplete 留作独立控件优化 |
| 实施状态 | Done |

## 本轮结果

| 场景 | 优化前 | 优化后 | 变化 |
| --- | ---: | ---: | ---: |
| `SelectShowCase` repeated mean | `220.31ms` | `143.76ms` | `-34.7%` |
| `SelectShowCase` alloc mean | `28027.36KB` | `23904.61KB` | `-14.7%` |
| `SelectShowCase` visuals | `1483` | `1410` | `-73` |

最终验证中，closed `Select` / `TreeSelect` / `Cascader` 不再创建 candidate list、tree view、cascader view；默认 `Select` 不再创建完整 `SelectAccessoryHost`，loading icon 也改为进入 loading 状态后按需创建。
