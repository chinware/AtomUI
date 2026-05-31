# Descriptions 性能优化

> 状态：已完成 Gallery 实测、结构优化与生命周期验证；本轮追加 collection changed item indexed traversal。

## 追加结构优化：collection changed items indexed traversal

`Descriptions.HandleCollectionChanged()` 处理 `Items` 增删时，旧路径把 `NotifyCollectionChangedEventArgs.NewItems/OldItems` 先包装成 `OfType<DescriptionItem>()`，再交给 add/remove helper。本轮为 `IList` 通知列表增加直接 Count/indexer helper，保留原有 `DescriptionItem` 类型要求、布局生成和移除逻辑。

正确性补充：状态验证暴露了四个动态集合问题。第一，非 `Window` TopLevel / headless 场景下 `_effectiveColumns` 可能保持 0，动态 add 后 `Grid.ColumnSpan=0` 抛异常；本轮在 layout 前补齐 effective columns fallback，默认走 `ExtraExtraLarge` 断点。第二，动态 insert/remove 旧路径没有按 `NewStartingIndex/OldStartingIndex` 操作 generated children，插入到中间或移除已删除项时可能让 visual children 和 `Items` 顺序错位；本轮按 item index 映射到 grid child index。第三，`Items.Clear()` 旧路径抛 `NotSupportedException`，本轮 Reset 直接清空 generated children。第四，`Items` 属性替换后旧集合仍保留订阅、新集合未订阅；本轮替换时释放旧订阅、订阅新集合并重建 generated children。

| 指标 | 优化前 | 优化后 | 公式 | 提升 | 结论 |
| --- | ---: | ---: | --- | ---: | --- |
| Descriptions add item filter LINQ operators / add | 1 operator | 0 operators | `(1 - 0) / 1` | 100.00% | 有效；新增项直接按 `NewItems.Count/indexer` 处理 |
| Descriptions remove item filter LINQ operators / remove | 1 operator | 0 operators | `(1 - 0) / 1` | 100.00% | 有效；移除项直接按 `OldItems.Count/indexer` 处理 |
| Descriptions headless dynamic add column span correctness | may throw `ColumnSpan=0` | fallback columns before layout | correctness | 已修复 | 非 Window TopLevel 下动态 add 不再因 `_effectiveColumns=0` 崩溃 |
| Descriptions generated child index correctness / insert-remove | append / current-index remove | starting-index insert-remove | correctness | 已修复 | 中间插入和移除按通知 index 更新 generated children |
| Descriptions reset collection behavior / clear | throws `NotSupportedException` | clears generated children | correctness | 已修复 | `Items.Clear()` 后 generated visuals 清空 |
| Descriptions Items replacement subscription lifecycle | old collection still subscribed | old unsubscribed / new subscribed | correctness | 已修复 | 替换 Items 后新集合驱动 UI，旧集合 mutation 不再影响 UI |

说明：这是动态增删描述项的结构性收益；不声明页面导航 timing 提升。

## 追加结构优化：ItemsSource 导入去 LINQ

`Descriptions.ItemsSource` 变更时，旧路径使用 `ItemsSource.OfType<DescriptionItem>()` 后再 `Items.AddRange()`。本轮改为显式类型判断：`IList` 来源直接用 `Count/indexer` 收集，普通 `IEnumerable` 来源用 `foreach` 收集，然后仍然一次 `Items.AddRange()` 写入，保留批量通知语义。行为仍只导入 `DescriptionItem`，其他对象继续忽略。

| 指标 | 优化前 | 优化后 | 公式 | 提升 | 结论 |
| --- | ---: | ---: | --- | ---: | --- |
| ItemsSource import LINQ operators / source change | 1 operator | 0 operators | `(1 - 0) / 1` | 100.00% | 有效；去掉 `OfType<DescriptionItem>()` |
| IList ItemsSource collection enumerators / source change | 1 enumerator | 0 enumerators | `(1 - 0) / 1` | 100.00% | 有效；列表来源直接 Count/indexer 导入 |
| ItemsSource collection notifications / source change | 1 batch | 1 batch | `(1 - 1) / 1` | 0.00% | 行为保持；仍然一次 `AddRange()`，不拆成逐项布局 |
| 非 DescriptionItem 过滤语义 | preserved | preserved | n/a | 0.00% | 行为保持；仍只接受 `DescriptionItem` |

说明：这是 ItemsSource 变更路径的结构性收益；未新增 Gallery timing 对比，不声明页面加载速度提升。

## 追加结构优化：MediaBreakInfo 单值解析 span trim

`DescriptionsMediaBreakInfo.Parse()` 的单值 fast path 旧实现先 `input.Trim()` 再 `int.TryParse()`。本轮改为 `input.AsSpan().Trim()` 后直接解析，key-value 复杂格式仍走原有 span parser。

| 指标 | 优化前 | 优化后 | 公式 | 提升 | 结论 |
| --- | ---: | ---: | --- | ---: | --- |
| Descriptions media break single-value trim temp strings / parse | 1 | 0 | `(1 - 0) / 1` | 100.00% | 结构收益；单值解析语义保持 |
| Page-load timing claim | none | none | n/a | n/a | 本轮没有有效前后 timing，不声明页面级速度收益 |

验证补充：`--verify-descriptions-states` 当前在本轮改动和 clean HEAD `d35b21a1a` 上均失败于相同的 HeaderLayout / horizontal bordered item visual 断言，因此不是本轮 parser 优化引入的问题。
