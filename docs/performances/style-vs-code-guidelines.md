# Style Selector 与代码状态机边界指南

## 背景

AtomUI 的控件样式默认应优先使用 XAML `Style` / `ControlTheme` 表达。声明式样式更直观，也更方便主题作者理解和覆盖。

但基础控件 primitive 在大量实例化时，复杂 selector 会变成真实运行时成本：selector matching、activator、属性监听、伪类监听、`/template/` 匹配和 style frame 激活都会被每个实例承担。

因此本指南定义一个边界：

**Style 是默认表达方式；代码状态机是性能 primitive 的优化手段，不是默认风格。**

## 核心原则

1. 视觉 token 留在 Style，状态决策可以下沉到代码。
2. 简单、低频、易覆盖的样式留在 Style。
3. 高频、组合爆炸、实例级成本高的状态计算才考虑代码实现。
4. 任何从 Style 下沉到代码的状态矩阵，都必须有 headless verification 覆盖。
5. Gallery 体验结论必须用真实 Gallery 场景复现，不能只依赖控件级 micro benchmark。

## 保留在 Style 的情况

优先保留 XAML selector / setter：

- 静态外观：padding、margin、font、corner radius、border thickness、默认背景。
- 简单状态：`:disabled`、`:checked`、`:selected` 等单状态、低组合样式。
- 非基础 primitive，实例数量通常较少的业务型控件。
- selector 数量少，不形成状态矩阵。
- 主题作者明显需要覆盖的样式点。
- 不在批量创建热路径上的 showcase 或 demo 装饰样式。
- 只影响低频状态或初始化后很少变化的属性。

## 考虑下沉到代码的情况

满足以下条件时，应进入性能 review：

- 控件是基础 primitive，会被多个组合控件大量复用。
- 同一 part 的样式由 `variant`、`status`、`hover`、`pressed`、`focus`、`disabled` 等多个状态共同决定。
- selector 数量呈矩阵增长。
- 同一视觉属性最终只是计算一个 effective value，例如 brush、background、border。
- 状态变化高频，例如 pointer over、pressed、focus within、text changing、dropdown opened。
- selector 使用 `/template/` 穿透模板匹配，并作用于热路径元素。
- 一个页面里可能出现 20 个以上实例。
- 控件级基准显示 style/selector 成本已经进入瓶颈。

## Review 阈值

以下不是绝对禁止线，但达到后必须解释为什么仍然留在 Style：

| 条件 | Review 要求 |
| --- | --- |
| 同一 template part 的状态 selector 超过 8 条 | 检查是否应收敛为 effective property |
| 同一视觉属性由 3 个以上状态共同决定 | 检查是否已形成状态矩阵 |
| `/template/` selector 超过 5 条 | 检查是否能改成模板绑定、effective property 或代码状态机 |
| 控件在一个真实页面可能出现 20+ 实例 | 建立控件级性能基线 |
| 状态变化会频繁触发 style activator | 优先考虑代码侧状态决策 |

## 推荐实现方式

如果决定下沉到代码，推荐模式是：

1. 在 theme 中继续提供 source token / source brush。
2. 控件代码监听必要状态。
3. 控件代码计算 `Effective*` 属性。
4. 模板只绑定 `Effective*` 属性。
5. 状态矩阵用 headless verification 覆盖。

示例边界：

- `InnerBoxDefaultBorderBrush`、`InnerBoxErrorBorderBrush` 等 source brush 留在 theme。
- `StyleVariant + Status + Hover + Pressed + Focus + Disabled` 的决策放在代码。
- 模板绑定 `EffectiveInnerBoxBorderBrush`、`EffectiveInnerBoxBackground`。

这样保留主题可配置性，同时避免把状态矩阵展开成大量 selector。

## 不推荐的做法

- 为了性能把颜色、尺寸、字体直接硬编码到控件代码。
- 没有基准数据就提前把所有 selector 改成代码。
- 只因为 selector 写起来多，就下沉代码，但控件并不在热路径。
- 下沉后不补状态验证。
- 用合成控件数据证明 Gallery 真实页面体验改善。

## 新控件流程

新增控件时按以下流程处理：

1. 先用 Style / ControlTheme 写出清晰版本。
2. 如果控件是基础 primitive 或高频控件，建立控件级基线。
3. 检查 selector 是否超过 review 阈值。
4. 超过阈值时，评估是否抽成代码 effective property。
5. 下沉后补 headless verification。
6. 如果该控件有 Gallery showcase，补真实 Gallery 场景复现。
7. 将基线、优化记录和结论写入对应 `docs/performances/<ControlName>/` 目录。

## AddOnDecoratedBox 的经验

`AddOnDecoratedBox` 是适合下沉状态矩阵的典型 case：

- 它是输入类控件共享 primitive。
- 会被 `LineEdit`、`SearchEdit`、`Select`、`TreeSelect`、`Cascader`、`DatePicker` 等大量复用。
- 同一 inner frame brush/background 由 variant、status、hover、pressed、focus、disabled、dropdown open 等状态共同决定。
- 原先 XAML selector 数量大且包含 `/template/` 匹配。
- 下沉后仍保留 theme source token，代码只负责 effective value。

这不是“放弃 Style”，而是把已经具备状态机性质的样式逻辑显式化、可测化。
