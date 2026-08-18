# Select 候选交互设计

本文档定义 Select 候选弹层中鼠标、键盘、已确认选择和虚拟化容器之间的统一候选交互模型。控件总体设计见 [Select 桌面版架构设计](overview.md)，源码职责与生命周期见 [Select 桌面版实现原理](implementation.md)。

## 1. 设计定位

Select 使用 active candidate 表达用户在候选弹层中的当前操作目标。active candidate 可以由鼠标移动或键盘导航产生，是 `Enter` 提交或切换选择的唯一候选目标。

该模型覆盖：

- `Single`、`Multiple` 和 `Tags` 三种模式。
- 鼠标与方向键交替使用时的候选迁移。
- active candidate、已确认选择和指针命中状态的职责分离。
- 过滤、分组、禁用项和最大选择数形成的不可用候选。
- 候选项虚拟化、容器回收和重新准备。
- 候选弹层关闭、过滤上下文变化和选项源重建时的状态失效。

## 2. 设计原则

1. `SelectCandidateList` 是 active candidate 的唯一 owner；候选项容器只投影 owner 状态。
2. 鼠标和键盘共享同一个 active candidate，不维护相互独立的 hover candidate 和 keyboard candidate。
3. `:pointerover` 只表达 Avalonia 指针命中事实，不独立决定 Select 候选高亮。
4. active candidate 与已确认选择相互独立；候选迁移不能改变 `SelectedOption` 或 `SelectedOptions`。
5. `Enter` 只提交或切换当前 active candidate，视觉目标与提交目标必须一致。
6. 分组标题、禁用项、隐藏项和达到 `MaxCount` 后不可新增选择的项不能成为新的 active candidate。
7. 鼠标热路径只在目标候选变化时更新状态，不滚动列表，也不遍历全部候选容器。
8. 键盘导航可以滚动 active candidate 到可见区域，但必须复用与鼠标相同的状态写入路径。
9. 虚拟化容器的 `IsCandidateSelected` 是可重建投影，不能成为跨回收周期的状态 owner。

## 3. 候选模型与 Public API

### 3.1 术语

| 术语 | 定义 |
| --- | --- |
| Active candidate | 当前可以由 `Enter` 提交或切换选择的唯一候选项。 |
| Committed selection | 已写入 `SelectedOption` 或 `SelectedOptions` 的确认选择。 |
| Pointer hit | Avalonia 通过 `IsPointerOver` / `:pointerover` 表达的当前指针命中事实。 |
| Source index | 候选项在有效候选源中的索引，是 `CandidateSelectedIndex` 的索引语义。 |
| View index | 过滤和分组后候选项在当前 ListView 投影中的位置。 |
| Candidate projection | 已准备容器上的 `IsCandidateSelected` 状态。 |

active candidate 使用以下 internal 状态表达：

```text
CandidateSelectedIndex
CandidateSelectedItem
```

两者共同表示同一个逻辑候选：

- 无 active candidate 时，index 为 `-1` 且 item 为 `null`。
- 存在 active candidate 时，index 指向有效候选源中的同一项，item 持有该候选对象。
- 已准备的目标容器投影 `IsCandidateSelected=true`，其他已准备容器投影为 `false`。

该设计不新增或改变 Select Public API、Template Part、公共伪类、ControlTheme key 或 Token。`CandidateSelectedIndex`、`CandidateSelectedItem` 和 `IsCandidateSelected` 继续属于内部候选列表协作契约。

### 3.2 状态优先级

候选项具有三个相互独立的状态维度：

| 状态 | Owner | 作用 |
| --- | --- | --- |
| Active candidate | `SelectCandidateList` | 导航、提交目标和候选高亮。 |
| Committed selection | Select 公共选择属性与 ListView selection | 已确认值、selected 视觉和 Form 值。 |
| Pointer hit | Avalonia input system | 产生鼠标候选迁移请求，不直接绘制候选视觉。 |

同一候选项可以同时是 active candidate 和 committed selection。此时 selected 视觉保持优先，active 状态仍作为 `Enter` 的交互目标存在。

## 4. 输入与状态策略

| 输入或变化 | Active candidate 结果 | 滚动 | 选择结果 |
| --- | --- | --- | --- |
| 鼠标移动到新的可用候选项 | 迁移到鼠标所在项 | 不滚动 | 不变 |
| 鼠标继续在当前 active candidate 内移动 | 不变 | 不滚动 | 不变 |
| 鼠标移动到分组标题或不可用候选项 | 保留原 active candidate | 不滚动 | 不变 |
| 鼠标移出候选列表 | 保留原 active candidate | 不滚动 | 不变 |
| `Up` / `Down` | 按当前视图顺序迁移到上一个或下一个可用候选项 | 滚动到可见 | 不变 |
| `Enter`，`Single` | 提交 active candidate | 不额外滚动 | 更新 `SelectedOption` 并关闭弹层 |
| `Enter`，`Multiple/Tags` | 保持 active candidate | 不额外滚动 | 切换该候选项是否位于 `SelectedOptions` |
| 指针点击可用候选项 | 该项作为交互目标 | 不额外滚动 | 按当前模式提交或切换选择 |
| `Escape` 或弹层关闭 | 清除 active candidate | 不滚动 | 不提交候选 |
| 过滤上下文或有效候选源重建 | 清除失效候选；重新导航时从有效视图解析 | 按后续键盘输入决定 | 已确认选择按 Select 选择契约保留 |

键盘导航跳过组标题、禁用项、隐藏项和不可新增选择项，并在当前有效候选视图内循环。鼠标移动到不可用项时不替换现有 active candidate，避免不可提交项成为视觉或键盘提交目标。

## 5. 架构与状态所有权

| 组件 | 职责 | 不负责 |
| --- | --- | --- |
| `Select` | 公共选择属性、候选源、过滤、模式和 popup 生命周期协调 | 不直接维护容器 active 视觉 |
| `SelectCandidateList` | active candidate owner、输入映射、索引转换、键盘滚动和提交/取消 | 不把 active 状态写入业务 option |
| `SelectCandidateListItem` | 投影 `IsCandidateSelected`、`IsSelected`、启用和分组状态 | 不持有跨回收周期的候选状态 |
| `CandidateVirtualizingStackPanel` | 键盘候选滚动到可见区域 | 不决定 active candidate |
| `SelectCandidateListItemTheme` | 把 active、selected、disabled 和隐藏状态映射为视觉 | 不从 `:pointerover` 推导独立候选 |

状态流保持单向：

```text
pointer move / Up / Down
    -> resolve valid candidate source index
    -> SelectCandidateList active candidate
    -> realized container IsCandidateSelected
    -> SelectCandidateListItemTheme active visual

Enter / pointer click
    -> active or clicked candidate
    -> ListView selection
    -> SelectedOption / SelectedOptions
    -> result display + Form value + SelectionChanged
```

## 6. Theme 与组合契约

`SelectCandidateListItemTheme` 基于 `ListViewItemTheme`，但 Select 的候选视觉必须由统一 active candidate 状态控制。

主题遵守以下层次：

1. 普通候选项使用默认前景和背景。
2. `:pointerover` 保留为输入命中事实，但不能独立应用候选 hover 背景。
3. `IsCandidateSelected=true` 且未确认选中时使用候选 active 背景。
4. `IsSelected=true` 使用已确认选择的背景、前景和字重，并覆盖 active 候选视觉。
5. disabled 和 group item 不显示 active candidate 视觉。
6. `IsHideSelectedOptions=true` 时已选项继续按现有契约隐藏。

该设计不改变 `ListViewItemTheme` 的通用 `:pointerover` 契约。Select 的 internal item theme 只在自己的类型边界内覆盖继承视觉，避免影响普通 ListView、Transfer 或其他 ListView 派生控件。

## 7. 核心算法与生命周期

### 7.1 鼠标候选迁移

鼠标移动路径：

```text
PointerMoved
    -> 从事件源解析最近的 SelectCandidateListItem
    -> 验证非 group、可见、启用且允许选择
    -> view index 映射为 source index
    -> 与 CandidateSelectedIndex 比较
    -> 相同则返回
    -> 更新 active candidate，不调用 ScrollCandidateItemIntoView
```

鼠标在同一候选项内部移动是稳态热路径，必须在索引比较后直接返回。目标变化时只清理旧候选容器并设置新候选容器，不扫描全部候选项。

### 7.2 键盘候选迁移

键盘路径：

```text
Up / Down
    -> 从当前 active source index 或 committed selection 解析起点
    -> 在当前 view projection 查找下一个可用候选
    -> 更新同一 active candidate 状态
    -> ScrollCandidateItemIntoView
```

鼠标和键盘路径共享候选状态提交逻辑。滚动属于键盘导航附加行为，不能成为共享状态写入方法的隐式副作用。

### 7.3 虚拟化与失效

- 容器清理或回收时必须清除本地 `IsCandidateSelected`。
- 容器重新准备时，根据 owner 的 active source index 或 item 恢复候选投影。
- active candidate 暂时没有已准备容器时，owner 状态继续存在；容器出现后重建视觉。
- 过滤、选项移除、禁用状态或 `MaxCount` 变化使 active candidate 不再可用时，清除 index、item 和旧容器投影。
- popup 关闭、popup 内容释放和候选列表 detach 时清除 active candidate，重新打开时不继承旧交互目标。

## 8. 性能、资源与 AOT 边界

- 同一候选项内的 PointerMoved 使用 O(1) 比较并直接返回。
- 候选迁移只更新旧、新两个已准备容器，不对全部 `ItemCount` 执行逐项状态写入。
- 鼠标路径不触发布局滚动；键盘路径只在 active candidate 变化后滚动一次。
- 事件源到容器的祖先解析不创建长期缓存、订阅或每项 handler。
- 设计不新增 `DynamicResource`、C# relay binding、timer、异步任务或 owner 外部引用。
- 状态、索引映射和事件处理使用强类型 C# 与现有显式注册，不引入反射、动态成员发现或 trimming root。

## 9. 兼容性与定制边界

- `SelectedOption`、`SelectedOptions`、`SelectionChanged`、选择模式和绑定语义保持不变。
- `OptionTemplate`、稳定 Template Part、Token、资源 key 和 `:dropdownopen` 伪类保持不变。
- 鼠标移动只改变 internal active candidate 和对应视觉，不提前提交公共选择。
- 鼠标 active candidate 与键盘 active candidate 使用相同的 `Enter` 提交语义，不能出现视觉目标与提交目标不一致。
- 自定义 `OptionTemplate` 只定制候选内容，不能成为 active candidate owner。
- Select 专属候选 item theme 可以改变 active 和 selected 的具体颜色，但必须保持单一 active candidate 与 selected 视觉优先级。

## 10. 验证要求

| 分层 | 必须证明的设计不变量 |
| --- | --- |
| 状态模型 | 任意时刻最多一个 active candidate；index、item 和容器投影指向同一候选。 |
| 键盘到鼠标 | 键盘导航后移动鼠标到另一项，active candidate 迁移到鼠标项，旧项不再显示 active。 |
| 鼠标到键盘 | 鼠标停留在旧项时按方向键，active candidate 迁移到键盘项，旧项的 `:pointerover` 不产生第二个候选视觉。 |
| 提交语义 | 鼠标迁移后按 `Enter` 提交鼠标 active candidate；方向键迁移后提交键盘 active candidate。 |
| 选择独立性 | active candidate 迁移不修改公共选择；selected 项保持 selected 视觉。 |
| 可用性 | group、disabled、hidden 和不可新增选择项不能成为新的 active candidate。 |
| 模式 | `Single` 提交并关闭；`Multiple/Tags` 对 active candidate 切换选择且保留弹层语义。 |
| 虚拟化 | recycle、reprepare、过滤和滚动不泄漏旧 `IsCandidateSelected`，未准备候选可恢复投影。 |
| 生命周期 | popup 关闭、内容释放、detach 和候选失效清除 active candidate。 |
| 性能与 AOT | 同项 PointerMoved 不扫描容器或滚动；实现不引入反射、动态发现和额外长期订阅。 |
