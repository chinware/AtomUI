# Semantic Part Gallery Preview 设计

本文是 Semantic Part Preview 页面模型、目标解析、Adorner 生命周期、性能预算和验证要求的正式所有者。其他架构、
模块和 ShowCase 文档只保留职责摘要与本文链接，不复制完整算法。

Semantic Part Preview 是 Gallery 中面向控件公共视觉契约的交互式检查工具。它读取已经注册的
`ControlSemanticDescriptor`，在独立 Tab 中展示 Part 元数据、真实控件预览和用户侧 AXAML 用法，并在用户主动选择
某个 Part 时高亮当前已实例化的目标节点。

Preview 只属于 `AtomUI.Toolkits.GalleryBase` 和具体产品 Gallery。它不能向 AtomUI Control、ControlTheme、Semantic Part
descriptor 或普通应用运行时注入演示状态。

## 1. 设计目标

- 让 Gallery 用户在真实控件上理解 `root`、`.semantic-*`、`StyleType`、`ContractType`、cardinality 和引入版本。
- 使用现有 `SemanticPartRegistry` 和模板 marker，不重复维护结构元数据。
- 把 Semantic Part 与普通 Examples 分成独立一级 Tab，避免把公共定制契约伪装成普通使用案例。
- 只有用户第一次进入 Semantic Parts Tab 时才创建 Preview 和演示控件。
- 只有用户 Hover 或 Pin 某个 Part 时才查找目标并创建高亮视觉。
- 使用 Avalonia 12 原生 `AdornerLayer` 跟踪布局、Transform、滚动和裁剪。
- 保持 NativeAOT、Browser 和 Desktop 路径不依赖反射、程序集扫描或运行时 AXAML 解析。

## 2. 非目标

Semantic Part Preview 不负责：

- 给 Control 增加 `SemanticClasses`、`SemanticStyles`、调试属性、事件或接口。
- 向目标控件或模板节点注入临时 class、Style、Binding 或 attached behavior。
- 替代 Semantic Part descriptor、Control 文档或应用侧 Selector 用法。
- 扫描 Application、程序集、所有 Window、所有 TopLevel 或任意第三方 AXAML。
- 在 Preview 未激活时缓存 VisualTree 节点、监听布局或跟踪 Popup。
- 恢复 Gallery 的 API 或 Design Token sidecar Tab。

## 3. 依赖与所有权

依赖方向固定为：

```text
具体产品 Gallery
        -> AtomUI.Toolkits.GalleryBase
        -> AtomUI.Desktop.Controls
```

- `AtomUI.Core` 和 Control 包拥有 descriptor、registry 与 `.semantic-*` marker。
- `AtomUI.Toolkits.GalleryBase` 拥有 ShowCase Tab 宿主、目标解析、高亮会话、Adorner 和代码片段构建。
- 具体产品 Gallery 拥有真实演示控件、本地化职责描述和页面接入。
- Control 包不得引用 GalleryBase；GalleryBase 不得把 Preview 状态回写到 Control。

因此普通应用未引用 GalleryBase 时，Semantic Part Preview 不进入其依赖闭包、IL、对象生命周期或运行时初始化链路。

## 4. ShowCase 页面模型

采用 Semantic Part Preview 的标准页面使用以下结构：

```text
GalleryShowCaseHost
  GalleryStickyTabsHost
    Header
      GalleryShowCaseHeader
    StickyContent
      TabStrip
        Examples
        Semantic Parts
    Content
      当前 Tab 内容
```

`GalleryShowCaseHost` 是 GalleryBase 的标准宿主，负责：

- 默认选择 Examples。
- 只有提供 `SemanticPartsContentTemplate` 时才显示 Semantic Parts Tab。
- 延迟创建 Semantic Parts 内容根；内容根可以是单个 `SemanticPartPreview`，也可以是包含多个 Preview 的普通 Control。
- 缓存当前页面生命周期内已经创建的 Tab 内容。
- 枚举该内容根中已经构造的全部 `SemanticPartPreview`，在 Tab 切换和页面 detach 时统一激活、停用和释放。
- detach 时释放 Preview 缓存和宿主持有的 Tab 导航 Visual；reattach 时按当前选择状态重建，禁止已脱离页面通过
  缓存控件继续保留旧合成资源。
- 复用 `GalleryStickyTabsHost` 的滚动上下文和只读 sticky mirror。

页面不得为 Semantic Part 手写 `ScenarioTabs`、`ScenarioContentHost`、私有 lazy controller 或 Tab 切换 code-behind。
API、Design Token 和 Semantic Parts 不能共用旧 sidecar 场景模型。

未提供 Semantic Parts 内容的页面继续直接展示 Examples，不创建空 TabStrip 或空 sticky 行。

## 5. 真延迟创建

Semantic Parts 必须使用 `IDataTemplate` 或等价显式 factory 保存创建描述。模板不能直接放入 `TabItem.Content`，也不能预先
赋给隐藏 `ContentPresenter`，以免 Measure、模板应用或绑定初始化提前构建内容。

首次进入页面时只允许创建：

- `GalleryShowCaseHost`。
- `GalleryShowCaseHeader`。
- Examples 内容。
- 两个轻量 Tab header；没有 Semantic 内容时只保留 Examples 页面结构。

在用户第一次选择 Semantic Parts 前，以下对象和操作必须不存在：

- `SemanticPartPreview`。
- 用于检查的真实 Control。
- `SemanticPartRegistry` 查询。
- Part presentation item。
- VisualTree 枚举。
- Adorner 和高亮画笔。
- Popup、布局、目标 detach 或主题事件订阅。
- 用户侧 AXAML 代码片段生成。

首次选择 Semantic Parts 后，宿主显式构建模板并缓存结果。切回 Examples 时必须立即停用高亮会话并把 Semantic 内容从
VisualTree 移除；当前页面仍附加时保留已创建对象，避免重复构建。页面 detach 时释放缓存内容及其订阅；如果同一页面在
Semantic Parts 仍选中时重新 attach，宿主重新构建 Preview 和轻量 Tab 导航，不复用已经释放的 Visual 引用。首次 attach
应复用属性初始化阶段已经创建的轻量导航，不能无条件重复构建。

一个页面包含多个公开 Semantic owner 时，`SemanticPartsContentTemplate` 使用一个普通布局 Control 作为延迟内容根，并为
每个 owner 创建独立 `SemanticPartPreview`。宿主只负责统一生命周期，不合并 descriptor、Part 列表、owner type 或目标作用域。
内容根必须在 factory 返回时已经通过普通 visual children 包含至少一个 Preview；不得把 Preview 隐藏在尚未应用的 ControlTemplate
中，再依赖宿主主动应用模板或扫描任意逻辑树。

## 6. Preview 模型

`SemanticPartPreview` 是 GalleryBase 控件。它承载一个真实 Preview 内容，并解析一个明确的 owner Control：

- `PreviewContent` 本身是 Control 时，默认把它作为 owner。
- 预览需要额外布局或操作区时，由 Gallery 显式提供 owner target。
- owner type 先按精确 CLR type 查询 `IThemeManager.SemanticParts`；派生示例需要复用基类契约时必须显式指定 owner type，
  不进行程序集或类型层次反射发现。
- 一个 Preview 永远只对应一个 owner descriptor。控件家族拥有多个 public owner 时必须使用多个 Preview，不能把不同 owner 的
  Part 拼接成一个虚拟 descriptor。

Preview presentation item 的结构字段全部来自 `SemanticPartDescriptor`：

```text
Name
Path
SelectorClass
SelectorRoute
StyleType
ContractType
Cardinality
Customization
CrossVisualRoot
Since
RuntimeCreated
```

具体产品 Gallery 只能按 Part path 提供本地化职责描述和必要的代码片段覆盖，不能覆盖 selector、ContractType、cardinality
或其他结构字段。Preview 必须验证描述 key 不包含 descriptor 中不存在的 Part；缺少描述时使用结构化 fallback 文案，不能
阻止 Preview 工作。

具体产品 Gallery 的标题、描述、演示正文和代码片段必须使用 AtomUI 自身的公共契约与术语。参考场景可以用于校准布局、
状态组合和定制意图，但不得把其他平台的 API 名称或机制描述直接带入 AtomUI，例如 `Semantic DOM`、`classNames`、`styles`
或对象函数式样式入口。Semantic Part 示例统一使用 `Semantic Part`、owner-scoped selector、descriptor 中发布的 Part path，
以及控件实际支持的 AXAML Style/Theme API；本地化资源中的 source 与各语言 target 必须同步表达同一机制。

### 6.1 展示结构

Preview 使用一个完整边框包围预览区和 Part 列表，不能把两栏拆成互不关联的卡片：

- 宽屏左侧是白色真实控件预览区，右侧是固定范围宽度的 Part 列表，中间只保留一条分隔线。
- 窄屏按预览区、Part 列表顺序上下堆叠，分隔线切换到列表顶部。
- 面板按内容决定高度并在内容区顶部对齐，不能被页面剩余高度强制拉伸成大面积空白舞台。
- Part 行使用平铺列表和行分隔线，不使用独立边框、圆角卡片、嵌套卡片或额外的 Semantic Parts 标题。
- 主行只常驻显示 Part 名称、引入版本、本地化职责描述、Pin 和 Info；selector、SelectorRoute、ContractType、cardinality、
  customization、跨视觉根和运行时创建等技术字段只在用户打开 Info 后显示。
- Info 打开后，下方详情区横跨检查面板完整宽度：selector、SelectorRoute、ContractType、StyleType、cardinality、customization 等技术元数据
  位于左侧固定宽度栏，Styling example 代码区位于右侧并占据主要宽度。
- 紧凑布局中，下方详情区按技术元数据、Styling example 的顺序上下堆叠，两者均占满宽度。
- 代码示例不得挤在固定宽度的 Part 右栏、拆成独立卡片或使用会遮挡目标预览的浮层。
- Pointer 经过的行使用克制的填充色；目标 Adorner 使用在浅色背景和 Primary Control 上都可辨认的高对比描边，
  但不得改变目标 Control 的属性、Style、class 或模板。

Info 元数据视图和代码查看器继续按需创建。未点击 Info 时，右栏元数据区域和全宽代码行都保持零高度，技术元数据视图、
代码文本、语法高亮器及其订阅都不存在。

## 7. 交互状态

Preview 只允许一个有效选择：

```text
effectivePart = pinnedPart ?? hoveredPart
```

- Pointer 进入列表项时设置 `hoveredPart`。
- Pointer 离开时清除 `hoveredPart`。
- Pin 对当前 Part 进行固定或取消固定。
- Pin 存在时，经过其他列表项不改变有效选择。
- Info 按需显示该 Part 的技术元数据并生成用户侧 AXAML，不触发目标查找。
- 用户侧 AXAML 中 AtomUI 控件命名空间统一使用 `atom` 前缀；Avalonia Contract 类型位于其他命名空间时使用
  `contract` 前缀。
- 从 Semantic Parts 切回 Examples 时清除 Hover 并停用当前 `SemanticPartHighlightSession`。
- 返回 Semantic Parts 时，如果仍保留 Pin，可以重新解析一次目标，但不能保留旧 Visual 引用。

列表主行显示名称、引入版本和本地化职责描述，不附加高亮结果 Tooltip。未实例化、当前不可见或超过高亮预算只影响
Adorner 结果集，不新增常驻文本、弹层或状态提示。

## 8. 目标解析

目标解析只在 `effectivePart` 变化时执行，并按 `DispatcherPriority.Render` 合并同一帧内连续 Hover 变化。稳定选择期间不重复
扫描 VisualTree。

### 8.1 Root

`root` 直接对应 owner Control，不依赖 `.semantic-root`。

### 8.2 静态模板 Part

静态模板 Part 以 Avalonia 12 公开的 `TemplatedControl.GetTemplateDescendants()` 为 owner 作用域入口。该 API 只返回
`TemplatedParent` 为当前 owner 的模板后代；Preview 在结果上按 descriptor class 过滤，不自行实现跨嵌套模板的全子树
class 搜索。

声明静态 Selector Part 的 owner 必须能够作为 `TemplatedControl` 提供这一作用域。非模板 Control 只能高亮 `root`，或者把
实际节点按 `RuntimeCreated` / `CrossVisualRoot` 契约交给对应的 owner-scoped resolver，不能退化为无边界 class 搜索。

候选节点必须同时满足：

- 节点是已附加的 Visual。
- `Classes` 包含 descriptor 的 `SelectorClass`。
- 节点的 `TemplatedParent` 与 owner 相同。
- 节点 `IsEffectivelyVisible=true`，Bounds 非零，并且所在祖先链没有完全透明节点。

不能只按 `.semantic-*` 扫描整个 owner 子树。不同 Control 可以合法复用 `.semantic-icon` 等 class，缺少 owner 边界会误命中
嵌套子 Control 的 Semantic Part。

### 8.3 Runtime-created Part

`RuntimeCreated=true` 时允许在 owner 的局部 Visual 作用域中查找已实例化节点。`SelectorRoute` 是 descriptor 与生成 Style 的静态
契约，不作为 Preview 的 runtime traversal 程序，也不直接生成用户主 selector；Preview 继续通过 registry owner 边界、AdditionalRoots 和 class marker
解析目标。遍历遇到另一个已注册 Semantic Control
时停止进入其内部模板，防止同名 marker 跨 Control owner 泄漏。运行时节点必须遵守 Semantic Part 总架构定义的 parent
和 owner 契约；尚未附加到 VisualTree 的节点不属于可高亮目标。

解析结果不建立长期索引。每次有效选择只保存当前 `SemanticPartHighlightSession` 需要的可见目标，选择结束后释放全部引用。

## 9. Adorner 高亮

每个可见目标通过 Avalonia 12 公开的 `AdornerLayer.GetAdornerLayer(target)` 获取对应 layer，并在该 layer 中创建一个临时
`SemanticPartAdorner`。不得从 Preview 自身寻找共享 layer，不得把所有目标坐标转换到 Preview 所在 Canvas，也不得跨
VisualRoot 手动换算屏幕坐标。

`SemanticPartAdorner` 必须：

- 只绘制高亮边框，不设置背景或半透明填充，不覆盖或改变目标原有颜色、文字和图形。
- 主目标使用 `#FAAD14`、`2px`、全不透明描边；其余目标使用同色 `1px`、85% 不透明描边。
- 不依赖目标 ControlTemplate。
- `IsHitTestVisible=false`、`Focusable=false`。
- 在目标 bounds 内绘制，遵守原生 clip。
- 第一个目标使用主高亮样式，其余目标使用次级样式。
- 不使用 `AdornerLayer.AdornerProperty`，避免覆盖焦点、验证或其他现有 Adorner。

`SemanticPartHighlightSession` 按以下顺序释放每个高亮 Adorner：

1. `AdornerLayer.SetAdornedElement(adorner, null)`。
2. 从对应 `AdornerLayer.Children` 删除 adorner。
3. 退订 Gallery 自己建立的临时事件。
4. 清空 adorner、target 和 layer 引用。

清空 `AdornedElement` 是强制步骤。Avalonia 12 的 Adorner 实现通过该变化释放内部 ancestor property subscription；仅从
Children 删除 Visual 不能作为完整生命周期契约。

## 10. Popup 与独立宿主

`CrossVisualRoot=true` 的模板 Popup 使用 owner-scoped 路径：

1. 只从 owner 的 `GetTemplateDescendants()` 结果中枚举 `Popup`。
2. Popup 打开后从公开的 `Popup.Child` 进入实际内容树。
3. 在 Child 所属 VisualRoot 中解析 marker。
4. 对匹配目标调用 `AdornerLayer.GetAdornerLayer(target)` 并添加 Adorner。
5. 只在该 Part 有效选择期间订阅对应 Popup 的 `Opened`、`Closed`。
6. Popup 打开后在 Render 优先级执行一次重新解析；关闭后立即清除对应高亮。

这一模型同时覆盖 OverlayPopupHost 和 PopupRoot，不依赖普通视觉树跨根继承，也不扫描所有 TopLevel。

Modal、Message、Notification 等由服务创建且不再能从 owner Popup 到达的独立宿主，不使用隐藏全局搜索。具体 Gallery Demo
可以向 Preview 显式提供 `AdditionalRoots`；该能力属于 Gallery 接入信息，不得要求产品 Control 增加 Preview 接口。

## 11. 高密度预算

`Multiple` Part 只处理已实例化且可见的节点。单次 `SemanticPartHighlightSession` 默认最多创建 32 个 Adorner：

- 匹配数量不超过预算时高亮全部目标。
- 超出预算时只高亮按稳定遍历顺序得到的前 32 个目标。
- 列表显示总匹配数和当前高亮数量。
- 不为不可见、零尺寸、完全透明或未附加节点创建 Adorner。

该预算保护 DataGrid cell、虚拟化 item 和其他高密度场景，不能通过缓存所有 Visual 引用规避预算。

## 12. 用户侧代码片段

代码片段由 descriptor 按需生成：

- `root` 生成 owner type selector 或实例级 Styles 示例。
- Selector Part 生成 owner scope、`StyleType` 和 `x:SetterTargetType="ContractType"`；生成 Style 内部封装 descriptor
  `SelectorRoute`。
- 不把复杂 route 拼接成用户主 selector，也不根据 `RuntimeCreated` 推断 descendant selector。
- `SelectorAndTheme` 可以补充强类型 Theme 属性入口，但 Selector 仍是基础契约。
- 不生成 `ContractType.semantic-*` 或 `:is(ContractType).semantic-*`。

代码展示复用 GalleryBase 已有源码查看能力，不引入第二套语法高亮器。Snippet 只有在用户打开 Info 时生成并缓存为纯字符串。

## 13. 性能与 AOT

成本模型必须分开描述：

| 状态 | 允许成本 |
|---|---|
| 普通应用未引用 GalleryBase | 无 Preview 增量成本 |
| Gallery 未创建 Semantic Tab 内容 | 只有 Tab header；无 Preview 对象和 registry 查询 |
| Preview 已创建但无有效选择 | descriptor 查询和列表 Visual；无目标扫描、Adorner 或目标事件订阅 |
| Hover/Pin 有效 Part | 一次 owner-scoped 查找、有限 Adorner 和原生布局跟踪 |
| 离开、取消 Pin、切换 Tab 或 detach | `SemanticPartHighlightSession` 全量释放 |

实现不得使用：

- 反射扫描 Control、属性、程序集或 AXAML。
- `Activator.CreateInstance` 创建 owner 或 Part。
- 全局 VisualTree/TopLevel registry。
- 常驻 `LayoutUpdated`、ScrollChanged、pointer move 或定时器。
- 每帧坐标重算和矩形分配。
- 静态 Visual、Control、Popup、Adorner 或 descriptor-to-instance cache。

Type identity、Part 元数据和 registry 均来自生成式静态 descriptor，满足 NativeAOT 与裁剪边界。

## 14. Button 首个样例

Button 是首个完整验收样例，不拥有 Preview 架构。Button ShowCase 只负责：

- 提供 `SemanticPartsContentTemplate`。
- 在模板中创建一个带图标和内容的真实 Button。
- 为 `root`、`icon`、`content` 提供本地化职责描述。
- 验证 `icon` 的 Multiple/可见替代实现和 `content` 的 Single 契约。

Button、ButtonTheme 和 Button Browser Theme 不因 Gallery Preview 新增任何属性、事件、marker、Style、Binding 或生命周期代码。

## 15. 验证要求

实现至少验证：

1. 页面初始化、Measure、Arrange、滚动、主题和语言切换均不构建 Semantic Parts 内容。
2. 第一次选择 Semantic Parts 时 factory 只执行一次；当前页面内重复切换复用缓存。
3. 未选择 Part 时不枚举 owner VisualTree，不存在 Adorner 和目标事件订阅。
4. Root、Single、Optional、Multiple 和不可见替代节点解析正确。
5. 嵌套 Semantic Control 的同名 class 不被误命中。
6. OverlayPopupHost 与 PopupRoot 使用目标所在 root 的 AdornerLayer。
7. 切回 Examples、Popup 关闭、目标 detach 和页面 detach 后释放全部高亮 Adorner 与订阅。
8. 超过 32 个可见目标时严格执行高亮预算，且不创建额外状态提示。
9. Info 生成的 AXAML 使用 owner selector、生成 `StyleType` 和 `x:SetterTargetType` 正式写法。
10. Preview 使用单外框、相邻双栏、平铺分隔行；主行不显示 selector、ContractType、cardinality 或 customization。
11. Info 打开前不创建技术元数据视图和代码查看器；打开后显示 selector、StyleType、SelectorRoute 等 descriptor 技术字段与样式示例。
12. 复杂 runtime Part 的代码示例使用生成 Style，并验证不会命中嵌套 Semantic owner 的同名 Part。
13. Button Control 和 Theme 的 public surface、模板 marker 与运行时路径没有因 Preview 变化。
14. Preview、演示 Control 和 Popup 在页面释放后可以被 GC。
15. 单 owner 页面和多 owner 家族页面都保持真延迟创建；多 Preview 内容根切换 Tab 时统一激活、停用和释放。
16. Desktop、Browser、裁剪和 NativeAOT 路径不需要反射保留配置。

## 16. 相关文档

- [Semantic Part 系统](../../architecture/systems/theming/semantic-parts.md)
- [Gallery ShowCase 页面设计](gallery-showcase-design-pattern.md)
- [GalleryBase ShowCase 控件设计](../../modules/toolkits-gallery-base/showcase-controls.md)
- [GalleryBase 架构](../../modules/toolkits-gallery-base/architecture.md)
- [AOT 编程规范](../../engineering/development/aot-programming-guidelines.md)
