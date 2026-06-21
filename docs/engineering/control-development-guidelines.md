# AtomUI 控件研发标准规范

本文档定义 AtomUI 控件研发时需要遵守或优先参考的工程标准，适用于 `AtomUI.Controls`、`AtomUI.Desktop.Controls`、DataGrid、ColorPicker 以及后续新增控件包。

## API 兼容性

优化代码和修复 bug 时，默认不得修改任何既有 API、主题契约或可观察行为。即使重构后新的 API 看起来更合理，也不能擅自引入、重命名、删除或改变现有契约。

API 和主题契约包括但不限于：

- `public`、`protected` 类型、成员、构造函数、枚举值和默认值。
- Avalonia `StyledProperty`、`DirectProperty`、`RoutedEvent`、命令、事件和绑定语义。
- 控件主题资源 key、Design Token 名称、ControlTheme key、template part 名称、伪类和选择器可依赖的状态。
- Gallery、文档或示例中已经暴露给用户使用的 XAML 属性、样式入口和交互语义。

如果确认不修改 API 无法正确完成修复或优化，必须先停止实现，向用户说明：

1. 为什么当前 API 无法保持。
2. 受影响的 API 范围和兼容性风险。
3. 是否存在保持兼容的替代方案，以及代价。
4. 推荐的 API 变更方案和迁移方式。

只有获得用户明确授权后，才能进行 API 变更。没有授权时，必须选择保持兼容的实现，或把问题标记为受 API 约束阻塞。

## 控件文件拆分建议

这是一条推荐规范，不作为强制约定边界。拆分文件时应优先保证控件契约容易发现，而不是单纯追求主文件行数变少。

- 控件主文件建议保留公共契约入口，包括公共属性、事件、方法、Avalonia 属性注册、CLR wrapper、构造函数和主要 `override` 入口。
- 控件实现代码未超过约 `2000` 行时，原则上不做文件拆分，优先通过方法顺序调整、区域归类和私有方法提取来改善可读性。
- 控件实现代码超过约 `2000` 行后，才考虑按稳定职责拆分内部实现，例如视觉状态同步、布局计算、template part 生命周期、资源绑定辅助等。
- 按职责拆分，不按“看起来整齐”拆分。拆出的文件应有稳定、单一的维护主题。
- 生命周期入口建议留在主文件，复杂细节可下沉到私有 helper 或 partial 文件中。
- 不为未来可能复用提前抽象；只有出现真实重复，且抽象不会模糊生命周期、绑定优先级或主题契约时，再提取共享 helper。

## 控件级设计文档与 Changelog

控件级研发文档必须遵循 [AtomUI 控件文档规范](control-documentation-guidelines.md)。控件设计文档只描述最新设计状态；设计、API、主题契约、Token 和实现结构的历史变化记录在对应控件目录下的 `changelog.md`。

## 控件 Token 设计

控件 Token 的分层、命名、计算、Theme Variables 边界、预设色和兼容性规则见 [AtomUI 控件 Token 设计规范](control-token-guidelines.md)。单个控件的 `token.md` 只记录该控件专属的 Token 语义、分类、使用范围和兼容边界，不重复全局 Token 系统规则。

## 控件成员排列建议

这是一条推荐规范，不作为强制约定边界。优化控件代码时应优先遵循现有控件的组织习惯，不要按个人偏好重排成员。

- 控件相关的 `enum`、小型公开类型通常放在控件类之前，便于先理解控件状态模型。
- 控件类开头优先放公共契约区，通常使用 `#region 公共属性定义`。
- 公共属性契约区建议先集中定义 `StyledProperty`、`DirectProperty` 等 Avalonia 属性注册字段，再按相同顺序集中放对应 CLR wrapper。
- `DirectProperty` 的 backing field 建议靠近对应 CLR wrapper，不放入普通 runtime 字段区。
- 公共事件建议单独成区，例如 `#region 公共事件定义`。`RoutedEvent` 注册字段和对应 .NET event wrapper 应保持在同一区域内。
- internal template/theme 契约建议放在公共契约之后，通常使用 `#region 内部属性定义`。这些成员包括模板依赖的 internal 属性、DirectProperty、状态 wrapper 等，不和普通 private 字段混放。
- 普通 runtime 字段建议放在契约区之后、构造函数之前，包括 template part、helper、disposable、运行时状态标志等。
- 构造函数区建议保持 `static` 构造函数在前，实例构造函数在后。
- 构造函数之后的方法不强制按访问级别排序。优先按控件功能流组织，例如 template 接入、属性变更分发、交互处理、布局计算、状态同步、渲染、表单适配等。
- 控件自身 API 成员应放在显式接口实现之前。这里的 API 成员不仅包括 `public` 方法，也包括作为控件扩展契约或生命周期入口的 `protected`、`protected virtual`、`protected override` 方法。
- `public` / `protected` API 方法可以靠近对应功能流，不要求全部放在构造函数后面；但如果是核心用户操作 API，应放在相对靠前且容易发现的位置。
- 控件实现的 public interface 也是对外能力契约。显式接口实现虽然语法上不是普通 public 成员，但应作为“接口契约区”组织，位置通常在控件自身 public/protected API 成员之后、private 实现方法之前。
- 接口契约区的优先级低于真正的 public/protected 控件 API 成员，但高于 private helper。
- 与显式接口实现直接配套的 protected virtual hook 可以留在同一个接口契约区，例如 `IFormItemAware.SetFormValue(...)` 与 `NotifySetFormValue(...)`。
- 如果接口只是纯内部协作接口，且不会作为控件对外能力被用户感知，可以按 internal 实现细节处理；但出现在控件 public 类型声明上的接口，默认按接口契约区处理。
- `protected override`、`protected virtual` 方法可以靠近其服务的功能块，不强制集中到一个 protected 区。
- explicit interface implementation 应按接口功能成区放置；对应的 `NotifyXxx` protected hook 建议留在同一区域。
- 大控件拆成 partial 时，按稳定功能域拆分，例如 Filter、DragAndDrop、AsyncLoad；不要按 public/protected/private 访问级别拆分。
- 代码优化时不要做大面积机械重排。只有当重排能明显改善阅读路径，且不掺杂行为修改时，才调整成员顺序。

## 控件 AXAML 视觉层级建议

这是一条推荐规范，不作为强制约定边界。优化控件 AXAML 时，应先保证可维护性、模板契约、视觉行为和交互语义不变，再减少视觉层级。

- AXAML 视觉树应保持必要最小层级。每一层容器都应有明确职责，例如布局、裁剪、背景/边框、命中测试、动画隔离、状态视觉或模板边界。
- 没有明确职责的 wrapper 应优先合并或删除，以减少最终 VisualTree 的深度和节点数量，降低模板实例化、布局遍历、selector 匹配和渲染遍历成本。
- 不要为了减少层级把 `ControlTemplate` / `ControlTheme` 中的功能视觉节点搬到 C# 动态创建。能用 AXAML 静态结构、`IsVisible`、selector 或 pseudo-class 表达的状态，优先留在 AXAML。
- 不要为了减少层级破坏 `PART_`、`/template/` selector、伪类、Design Token、资源绑定、ControlTheme key、template part 名称或样式入口。
- 不要合并承担不同职责的容器，例如裁剪层、动画层、命中测试层、边框背景层、popup shell 或 template contract 边界。
- 简化布局时优先替换无必要的 wrapper 和过度复杂的布局面板；删除或合并前必须确认圆角、裁剪、背景、命中测试、动画、焦点和 selector 命中行为不变。
- AXAML 层级优化后应通过对应控件测试或 Gallery 走查验证外观、交互和主题切换不变。如果声明性能收益，需要提供前后 VisualTree 节点数、层级深度或测量数据。
