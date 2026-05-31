# Avatar 性能优化

> 状态：已完成 Gallery 实测；本轮追加 AvatarGroup 动态 children 同步结构优化。

## 追加结构优化：AvatarGroup collection changed indexed traversal

`AvatarGroup.ChildrenChanged()` 处理动态 add/remove 时，旧路径通过 `e.NewItems/e.OldItems.OfType<Control>()` 枚举 collection changed item 列表。本轮改为 `IList.Count/indexer` 显式访问，保留 add 时先配置 `Avatar` 再插入 logical/visual children 的顺序，remove 时继续从 logical/visual children 移除同一批控件。

| 指标 | 优化前 | 优化后 | 公式 | 提升 | 结论 |
| --- | ---: | ---: | --- | ---: | --- |
| AvatarGroup add item filter LINQ operators / add | 2 operators | 0 operators | `(2 - 0) / 2` | 100.00% | 有效；配置和插入两段都改为 Count/indexer |
| AvatarGroup remove item filter LINQ operators / remove | 1 operator | 0 operators | `(1 - 0) / 1` | 100.00% | 有效；remove 路径不再创建 `OfType` iterator |

说明：这是 AvatarGroup 动态 children 变化路径的结构性收益；不声明页面导航 timing 提升。
