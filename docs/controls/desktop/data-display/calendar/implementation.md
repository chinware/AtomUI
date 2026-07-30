# Calendar 桌面版实现原理

本文档描述 Calendar 桌面版的内部实现范围、源码职责、状态流、生命周期与维护规则。公共设计与 API 契约见 [Calendar 桌面版架构设计](overview.md)，Token 见 [Calendar Token 设计](token.md)，变化记录见 [Calendar Changelog](changelog.md)。

## 1. 实现定位

本文档覆盖 Calendar 的控件实现、主题接入与状态同步。具体属性注册、默认值和 AXAML selector 仍应直接阅读源码；本文只记录维护者必须理解的稳定结构和不变量。

## 2. 源码文件结构

```text
src/AtomUI.Desktop.Controls/Calendar/
├── Calendar.cs                       # Public API、状态转换、生命周期、伪类、Header/View 接线
├── CalendarDateRange.cs              # ValidRange 值对象（public）
├── CalendarEnums.cs                  # CalendarMode / CalendarSelectSource / CalendarCellType
├── CalendarCellContext.cs            # Public Cell 模板上下文
├── CalendarHeaderContext.cs          # Public Header 模板上下文
├── CalendarEventArgs.cs              # 三个 Public 事件参数
├── CalendarControlToken.cs           # 六语义组件 Token（scope id CalendarControl）
├── Internal/
│   ├── CalendarViewMode.cs           # 内部面板模式 Date/Month
│   ├── CalendarViewCellModel.cs      # 不可变 Cell 模型
│   ├── CalendarViewCellBuilder.cs    # 纯日期/周/月网格构建
│   ├── CalendarView.cs               # 面板与容器 owner、焦点导航
│   ├── CalendarViewCell.cs           # Cell 交互外壳、伪类、激活
│   ├── CalendarViewAutomationPeer.cs # Table 语义
│   ├── CalendarViewCellAutomationPeer.cs # GridItem + SelectionItem 语义
│   ├── CalendarHeader.cs             # 默认 Header 控件
│   ├── CalendarHeaderOptions.cs      # 年/月选项与月份收敛（纯逻辑）
│   ├── CalendarRelayCommand.cs       # Header Context 命令
│   └── CalendarPseudoClass.cs        # 根/Cell 伪类常量
└── Themes/
    ├── CalendarTheme.axaml(.cs)       # CalendarControlTheme
    ├── CalendarHeaderTheme.axaml(.cs)
    ├── CalendarViewTheme.axaml(.cs)
    ├── CalendarViewCellTheme.axaml(.cs)
    └── CalendarThemes.axaml           # 聚合并实例化四个 ControlTheme
```

职责边界：

- `Calendar` 是唯一业务状态 owner，负责 Public API、`CommitUserSelection`/`CommitModeChange`、事件顺序、模板生命周期、根伪类、Culture 分发。
- `CalendarView` 是纯面板，按输入重建 Cell Model、管理有界容器池、roving focus 与键盘导航，只通过 `CellSelected` 报告用户意图。
- `CalendarViewCell` 应用一个不可变 Cell Model，投影模板，管理伪类与 Pointer 激活。
- `CalendarViewCellBuilder`、`CalendarHeaderOptions` 是纯逻辑（无 Control 依赖），承载全部网格与选项算法，可独立单测。

## 3. 状态流

单向数据流：`Calendar` 向 `CalendarHeader` 与 `CalendarView` 投影不可变状态。

- 用户 Pointer/键盘激活 Cell → `CalendarView.CellSelected` → `Calendar.CommitUserSelection(target, source)`。
- 默认 Header 选年/月/切模式 → `CalendarHeader.YearSelected/MonthSelected/ModeSwitched` → `Calendar.CommitUserSelection`/`CommitModeChange`。
- 自定义 Header 通过 `CalendarHeaderContext.ChangeValueCommand`（source=Customize）/`ChangeModeCommand` 提交。
- `CommitUserSelection` 用内部 `_isCommitting` flag 写 `Value`，阻止属性路径重复触发，然后按 `PanelChanged -> ValueChanged -> Selected` 顺序触发。

## 4. Cell Model 失效与容器池

- Cell Model 仅在 `Value`/`ViewMode`/`ShowWeek`/`ValidRange`/`DisabledDate`/`Culture`/`Today` 变化时重建，不在 Measure/Arrange 热路径计算。
- Date 模式最多 42 日期容器（ShowWeek 增 6 周序号），Month 模式 12 容器；容器池复用，重复切换 Mode/Value 不无限增长。
- `DisabledDate` 对每个候选值每次重建最多调用一次，异常不被吞掉。

## 5. 生命周期

- `Calendar.OnApplyTemplate` 前解绑旧 `CalendarView`/`CalendarHeader` 事件，接入新 part 后回放 Value/Mode/Range/Culture 并刷新伪类。
- `Calendar.OnAttachedToVisualTree` 订阅语言服务，`OnDetachedFromVisualTree` 解绑；语言变化把新 Culture 推给 View/Header 触发重建。
- FlowDirection/RTL 只改布局方向，不改 Value、事件顺序或日期计算。

## 6. Automation 与 AOT

- `CalendarView` 暴露 Table 语义，`CalendarViewCell` 暴露 ListItem + SelectionItem 语义；选中/名称来自同一 Cell Model。
- 所有 Avalonia 属性静态注册，模板关系用 TemplateBinding/AXAML selector/强类型 binding，Gallery DataTemplate 用 `x:DataType`，无反射日期适配。

## 7. 与 DatePicker 的隔离

新 Calendar 与 DatePicker 的 CalendarView 子系统完全独立：不同命名空间（`Internal.Calendar` vs `CalendarView`）、不同 Token（`CalendarControlToken` vs `CalendarToken`）、不共享类型。本次重构不改动 DatePicker 实现。
