# AtomUI 过滤系统架构

过滤系统统一候选项 Control 的谓词、过滤值和 item 取值模型。共享契约位于 `AtomUI.Controls.Shared`，具体 Control
拥有候选视图、层级策略、UI 状态和高亮实现。

使用方式见 [过滤使用指南](../../../guides/control-infrastructure/filtering.md)。

## 设计原则

1. 统一 `Filter + FilterValue + FilterValueSelector`，不统一具体 Control 的候选视图和布局算法。
2. 谓词只比较 value 与 filter value，不读取 owner、Visual 或 item 内部状态。
3. item 到 value 的领域映射由 Selector 完成。
4. `IsFilterEnabled` 表达用户意图，不从 Filter 是否为空派生。
5. 树路径、父链、双栏和文本高亮由具体 Control 保留自身策略。
6. 过滤不能修改应用提供的原始 ItemsSource 或破坏 selection identity。

## 公共契约

```csharp
public interface IValueFilter
{
    ValueFilterMode Mode => ValueFilterMode.Custom;
    bool Filter(object? value, object? filterValue);
}

public delegate object? DefaultFilterValueSelector(object? value);
```

`ValueFilterMode` 定义 StartsWith、Contains 和 Equals 三类匹配，以及 culture/ordinal、大小写敏感/不敏感组合。
`ValueFilterFactory` 为内置 mode 返回无状态 `IValueFilter`。

## Control 属性模型

| 属性 | Owner |
| --- | --- |
| `Filter` | 匹配谓词；Control 可以通过 `SetCurrentValue` 提供默认值 |
| `FilterValue` | 当前过滤条件；通常由内部搜索输入或外部 binding 更新 |
| `FilterValueSelector` | item 到比较 value 的转换 |
| `IsFilterEnabled` | 搜索 UI 和过滤路径的用户意图开关 |

Control 可以增加 `FilterStrategy`、`FilterHighlightStrategy`、`FilterHighlightForeground` 或结果计数，但不能改变
基础谓词的两参数语义。

## 数据流

```text
Search input
  -> FilterValue
  -> Control candidate view
  -> FilterValueSelector(item)
  -> IValueFilter.Filter(value, filterValue)
  -> include / exclude / highlight
  -> result count and visual state
```

Selector 可以把 Cascader path 或业务对象复合字段编码成一个 value。Filter 不回查原始 item，也不解析 binding path。

## 有效状态

`IsFilterEnabled` 与 Filter 是否为空保持正交：

- `IsFilterEnabled=true` 可以显示搜索 UI，并由 Control fallback 到默认 Filter。
- 非空 Filter 不自动开启搜索 UI。
- ComboBox 使用 internal effective state 表达 `IsEditable && IsFilterEnabled`。
- Select 等 Control 可以在特定模式下产生 internal effective state，但公共属性语义不变。

Internal effective state 只供主题和实现消费，不形成第二套公共过滤 API。

## Control 责任边界

- AutoComplete、ComboBox、Mentions 和 Select 拥有平铺候选视图。
- Cascader、TreeView 和 TreeSelect 拥有父链、展开、层级隐藏和路径高亮。
- ListBox 和 ListView 拥有 container 可见性、回收和文本高亮。
- Transfer 拥有左右独立过滤值与结果视图，但共享谓词和 Selector。

原始数据源、selection model 和稳定 item identity 不因过滤被替换。过滤结果通过派生视图、container state 或
Control 自身的可见性策略表达。

## 生命周期与性能

- Filter 与 Selector 是无状态或应用提供的长期对象，不在 item 热路径重复创建。
- 数据源、FilterValue、Filter、Selector 或策略变化时，Control 使候选视图失效并重新求值。
- 虚拟化 container recycle 必须清理旧高亮、匹配状态和业务 class。
- tree filter 的父链与结果缓存必须在数据源或结构变化时失效。
- 内置路径不使用反射读取 item 属性；复杂对象由强类型或显式 Selector 投影。

## AOT 边界

`DefaultFilterValueSelector` 是显式委托，不接受字符串 path。内置 Control 可以使用 Avalonia 已有强类型属性、
compiled binding 或生成式 accessor，但不得通过 `PropertyInfo` 或 `GetProperty(filterPath)` 实现内置过滤。

## 兼容性

- `IValueFilter.Filter(value, filterValue)` 参数含义和返回值语义是稳定契约。
- `IsFilterEnabled` 从用户意图改为派生状态属于破坏性行为变更。
- 修改内置 `ValueFilterMode` 的比较规则或默认 culture 语义需要兼容性评估。
- Control 可以调整内部候选视图，但必须保持原始数据源和 selection identity。

## 验证要求

1. 内置 mode 的 culture、ordinal 和大小写组合符合定义。
2. null value、null filter value 和空字符串边界稳定。
3. Selector 在谓词之前执行，谓词只收到投影值。
4. Filter 与 `IsFilterEnabled` 保持正交。
5. ComboBox 过滤不修改原始 ItemsSource 和 selection。
6. tree Control 的父链、展开和高亮策略在过滤清除后正确恢复。
7. Transfer 左右过滤状态互不覆盖。
8. container recycle 不保留旧匹配或高亮状态。
9. NativeAOT 路径不依赖反射成员访问。
