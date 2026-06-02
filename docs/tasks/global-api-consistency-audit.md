# 全局 API 与属性一致性扫描报告

日期：2026-06-02

当前状态：高优先级问题 H1-H7 已修复；中优先级和低优先级问题仍未处理。

## 扫描范围

本次扫描是对 AtomUI 源码和 Gallery 消费端代码做的全局静态审查，重点关注以下几类问题：

- Avalonia 属性注册名和注册 owner 是否一致。
- 公开 API 命名是否存在缺陷，以及 API 契约是否完整。
- 明显的复制粘贴逻辑错误。
- 公开 API 或框架回调路径中暴露的 `NotImplementedException`。
- 代表性项目的构建烟测结果。

扫描文件范围：3,085 个 `*.cs`、`*.axaml`、`*.csproj`、`*.props`、`*.targets` 文件。

## 总览

| 严重级别 | 数量 | 主要风险 | 当前状态 |
| --- | ---: | --- | --- |
| Critical | 0 | 本次静态扫描未确认安全或数据丢失级别问题。 | 不适用 |
| High | 7 | Avalonia 属性元数据错误、伪类状态错误、集合接口契约未实现。 | 已修复 |
| Moderate | 3 | Gallery 消费端构建失败、Converter 反向转换运行时崩溃风险、公开 API 命名债务。 | 未修复 |
| Low | 1 | 默认接口方法存在可避免的运行时陷阱。 | 未修复 |

## 高优先级问题

本节 H1-H7 已于本轮修复。

| ID | 问题 | 证据 | 影响 | 建议修复 |
| --- | --- | --- | --- | --- |
| H1 | `Icon.FallbackBrushProperty` 注册时使用了错误的属性名。 | `src/AtomUI.Core/Controls/Icon/Icon.cs:42` 中 `FallbackBrushProperty` 使用 `nameof(Icon)` 注册；`src/AtomUI.Controls/Icon/Themes/IconTheme.axaml:38` 中样式设置的是 `FallbackBrush`。 | Avalonia 属性元数据名与 CLR 属性名不一致，会影响样式查找、诊断信息，以及依赖属性名的工具或绑定行为。 | 改为使用 `nameof(FallbackBrush)` 注册，并增加 `FallbackBrushProperty.Name` 的断言测试。 |
| H2 | `WaveSpiritDecorator.OpacityMotionDurationProperty` 被注册成了 `SizeMotionDuration`。 | `src/AtomUI.Controls/Primitives/WaveSpiritDecorator.cs:23` 中使用 `nameof(SizeMotionDuration)` 注册；主题 setter 位于 `src/AtomUI.Desktop.Controls/Primitives/Themes/WaveSpiritDecoratorTheme.axaml:10`。 | 两个不同属性会共享相同的元数据名，透明度动画时长的 setter 或模板路径可能变得模糊，甚至解析失败。 | 改为使用 `nameof(OpacityMotionDuration)` 注册，并测试两个 motion duration 属性名。 |
| H3 | `CalendarItem.HeaderBorderThicknessProperty` 注册 owner 错误，注册到了 `Calendar`，但实际属于 `CalendarItem`。 | `src/AtomUI.Desktop.Controls/DatePicker/CalendarView/CalendarItem.cs:70` 使用 `AvaloniaProperty.Register<Calendar, Thickness>(...)`；`src/AtomUI.Desktop.Controls/DatePicker/Themes/CalendarView/CalendarItemTheme.axaml:22` 和 `:111` 在 `CalendarItem` 模板中绑定/设置该属性。 | 属性 owner 元数据与实际暴露和消费属性的类型不一致，可能导致 TemplateBinding 或 Setter 行为错误或不稳定。 | 注册 owner 改为 `CalendarItem`，并增加 CalendarItem 主题加载烟测。 |
| H4 | `MotionTransformOperationsProperty` 使用了常量字段名，而不是常量值。 | `src/AtomUI.Core/MotionScene/IMotionActor.cs:21` 定义 `MotionTransformOperationsPropertyName = "MotionTransformOperations"`；`:28` 注册时使用 `nameof(MotionTransformOperationsPropertyName)`。 | 实际注册名会变成 `MotionTransformOperationsPropertyName`，而不是 `MotionTransformOperations`，导致 motion 绑定、样式和调试路径与公开 API 名不匹配。 | 注册时直接使用 `MotionTransformOperationsPropertyName`，并增加 motion actor 属性名断言。 |
| H5 | `TimelineIndicator.IndicatorMinHeightProperty` 被注册成了 `RelativeLineHeight`。 | `src/AtomUI.Controls/Timeline/TimelineIndicator.cs:51` 中 `IndicatorMinHeightProperty` 使用 `nameof(RelativeLineHeight)` 注册。 | DirectProperty 名称与 CLR 属性名不一致，会干扰属性查找、诊断信息和布局失效分析。 | 改为使用 `nameof(IndicatorMinHeight)` 注册，并增加属性名断言。 |
| H6 | ProgressBar 结束对齐伪类永远不会被设置。 | `src/AtomUI.Controls/ProgressBar/AbstractGeneralProgressBar.cs:673-676` 中 `PercentLabelInnerCenter` 被设置了两次；`src/AtomUI.Controls/ProgressBar/ProgressBarPseudoClass.cs:10` 已定义 `PercentLabelInnerEnd`。 | `:labelinner-end` 状态永远不会生效，结束对齐样式无法命中；同时结束对齐时可能错误激活 center 样式。 | 最后一行应改为 `ProgressBarPseudoClass.PercentLabelInnerEnd`，并增加 start/center/end 三种状态测试。 |
| H7 | `DataGridSelectedItemsCollection` 实现了 `IList`，但 `CopyTo` 未实现。 | `src/AtomUI.Desktop.Controls.DataGrid/DataGridSelectedItemsCollection.cs:12` 实现 `IList`；`:200` 的 `CopyTo` 直接抛出 `NotImplementedException`。 | 任何把选中项集合当作 `ICollection` 或 `IList` 并调用 `CopyTo` 的消费方都会运行时崩溃。这是接口契约不完整问题。 | 按当前枚举选中项的逻辑实现 `CopyTo`，或者如果不希望暴露该能力，需要收窄公开契约。 |

## 中优先级问题

| ID | 问题 | 证据 | 影响 | 建议修复 |
| --- | --- | --- | --- | --- |
| M1 | Gallery Release 构建失败，说明 API 消费端烟测不过。 | 执行 `dotnet build controlgallery/AtomUIGallery/AtomUIGallery.csproj -c Release --framework net10.0 --no-restore`，结果为 308 个 warning、236 个 error。典型错误包括 `https://atomui.net` 命名空间下大量 XAML 类型无法解析，以及 `ICascaderOption`、`INavMenuNode`、`NavMenuMode`、`TabItemData`、`SliderMark`、`SelectPopupPlacement` 等公开类型找不到。 | Gallery 是主要集成消费面。如果不是 restore/package 状态陈旧导致的假阳性，就说明公开 API 导出、包引用、XML 命名空间映射或 Gallery 源码引用存在断裂。 | 先带 restore 重新构建排除缓存/包状态问题；如果仍复现，修复包引用、命名空间导出或 Gallery 源码引用，并把 Gallery Release 构建纳入 CI。 |
| M2 | 多个 `IValueConverter.ConvertBack` 直接抛出 `NotImplementedException`。 | 示例：`src/AtomUI.Controls.Shared/Converters/StringToTextBlockConverter.cs:39`、`src/AtomUI.Controls.Shared/Converters/DoubleToGridLengthConverter.cs:20`、`src/AtomUI.Controls.Shared/Converters/CornerRadiusFilterConverter.cs:32`、`src/AtomUI.Desktop.Controls/Select/Converters/OptionValueConverter.cs:19`、`src/AtomUI.Desktop.Controls/Input/Converters/TextAreaFramePaddingConverter.cs:20`、`src/AtomUI.Desktop.Controls/TreeView/Converters/NodeSwitcherIconModeConverter.cs:23`、`src/AtomUI.Controls/CheckBox/Converters/CheckBoxIndicatorStateConverter.cs:23`、`src/AtomUI.Desktop.Controls.DataGrid/Utils/Converters/DataGridPaginationVisibilityConvertor.cs:31`、`src/AtomUI.Desktop.Controls.DataGrid/Utils/Converters/DataGridUniformBorderThicknessToScalarConverter.cs:21`。 | 单向 converter 本身可以不支持反向转换，但 `NotImplementedException` 会让绑定模式变化或 converter 被复用时表现为“未完成代码”的运行时崩溃。 | 根据场景返回 `BindingOperations.DoNothing` 或 `AvaloniaProperty.UnsetValue`；如果确实不支持反向转换，应抛出语义明确的 `NotSupportedException`。 |
| M3 | `DataGridPaginationVisibilityConvertor` 拼写错误，应该是 `Converter`。 | `src/AtomUI.Desktop.Controls.DataGrid/Utils/Converters/DataGridPaginationVisibilityConvertor.cs:6`；引用位置包括 `src/AtomUI.Desktop.Controls.DataGrid/Themes/DataGridTheme.cs:24`、`:28`，以及 `src/AtomUI.Desktop.Controls.DataGrid/Themes/DataGridTheme.axaml:30`、`:46`。 | 如果该类属于公开 API，拼写错误会变成 API 债务；后续再改名会变成破坏性变更。 | 新增拼写正确的 `DataGridPaginationVisibilityConverter`，如果需要兼容已有源码/二进制，则保留旧类作为 obsolete alias。 |

## 低优先级问题

| ID | 问题 | 证据 | 影响 | 建议修复 |
| --- | --- | --- | --- | --- |
| L1 | 部分公开接口的默认方法抛出 `NotImplementedException`。 | `src/AtomUI.Desktop.Controls/NavMenu/NavMenuNode.cs:14`、`src/AtomUI.Desktop.Controls/Cascader/CascaderOption.cs:18`、`src/AtomUI.Desktop.Controls/TreeView/ITreeItemNode.cs:16`。 | 如果消费方实现接口但没有覆盖默认方法，框架调用时可能运行时失败。该问题优先级较低，因为内置具体节点类型可能已经覆盖。 | 如果父节点更新是必需能力，应让接口方法成为必须实现；如果是可选能力，提供安全的 no-op 默认实现。 |

## 构建烟测结果

| 命令 | 结果 | 备注 |
| --- | --- | --- |
| `dotnet build src/AtomUI.Desktop.Controls.DataGrid/AtomUI.Desktop.Controls.DataGrid.csproj -c Release --framework net10.0 --no-restore` | 通过 | 0 warning，0 error。 |
| `dotnet build controlgallery/AtomUIGallery/AtomUIGallery.csproj -c Release --framework net10.0 --no-restore` | 失败 | 308 warning，236 error。需要先排除 restore/package 状态问题；若仍复现，需要修复 API 导出或包引用问题。 |

## 建议修复顺序

1. 优先修复 Avalonia 属性元数据不一致问题：H1 到 H5。
2. 修复 ProgressBar 伪类复制粘贴错误：H6。
3. 实现 `DataGridSelectedItemsCollection.CopyTo`：H7。
4. 对 Gallery 带 restore 重新构建；如果仍失败，修复集成断裂：M1。
5. 统一 converter 反向转换行为，避免 `NotImplementedException` 暴露到运行时：M2。
6. 清理公开 API 拼写债务，并根据兼容性要求保留 alias：M3。
7. 增加低成本守护测试：
   - 自定义 StyledProperty/DirectProperty 的属性名断言。
   - 使用内部 TemplateBinding 属性的控件模板加载烟测。
   - 互斥视觉状态的伪类状态测试。
   - Gallery Release 构建作为 API 消费契约测试纳入 CI。

## 说明

- 本报告没有列出低置信度匹配，例如继承属性的 `AddOwner` 模式和有意设置的互斥状态切换。
- 上述问题均在全局扫描后做过人工抽样复核。
- 本轮已修复高优先级问题；中优先级和低优先级问题仍按上面的建议顺序保留。
