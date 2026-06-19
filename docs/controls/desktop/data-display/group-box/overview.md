# GroupBox 桌面版架构设计

本文档定义 `AtomUI.Desktop.Controls.GroupBox` 桌面版的最新设计定位、公共契约、状态模型、视觉主题关系和兼容边界。通用控件研发约束见 [控件研发标准](../../../../engineering/control-development-guidelines.md)，内部实现原理见 [GroupBox 桌面版实现原理](implementation.md)，GroupBox Token 的专项设计见 [GroupBox Token 设计](token.md)，设计和契约变化记录见 [GroupBox Changelog](changelog.md)。

## 1. 控件定位

GroupBox 是桌面端数据展示类分组容器，用于在一块有边框的区域中展示一组相关内容，并通过 Header 标题、图标和标题位置表达分组语义。

GroupBox 的职责是组织和标注内容区域，不负责内容项布局、折叠展开、表单校验、数据绑定集合管理或异步加载。需要折叠行为时应使用 Collapse/Expander 类控件；需要卡片信息组织时应使用 Card。

## 2. 设计语言

GroupBox 的视觉语义接近 fieldset：边框提供分组边界，Header 嵌入在边框上方的视觉通道中，内容区域在边框内部保持稳定留白。

Header 可以位于左侧、居中或右侧，但标题本身始终是分组名称，不应承担操作入口或复杂工具栏职责。Header 图标用于强化分组主题，不改变 GroupBox 的交互模型。

## 3. API 与契约模型

GroupBox 继承 `ContentControl`，公共 API 分为 Header API 和继承的容器 API。

Header API：

| API | 类型 | 语义 |
| --- | --- | --- |
| `HeaderTitle` | `string?` | Header 显示的分组标题。 |
| `HeaderTitleColor` | `IBrush?` | Header 标题颜色，默认来自 `ColorText`。 |
| `HeaderIcon` | `PathIcon?` | Header 标题前的图标；为 `null` 时图标节点不可见。 |
| `HeaderTitlePosition` | `GroupBoxTitlePosition` | Header 内容水平位置，支持 `Left`、`Right`、`Center`。 |
| `HeaderFontSize` | `double` | Header 标题字号。 |
| `HeaderFontStyle` | `FontStyle` | Header 标题字体样式。 |
| `HeaderFontWeight` | `FontWeight` | Header 标题字重。 |

继承 API：

- `Content` / `ContentTemplate` 定义分组内容。
- `Background` 定义内容区域背景，允许透明或半透明。
- `BorderBrush` / `BorderThickness` 定义分组边框。
- `CornerRadius` 定义边框圆角。
- `Padding` 定义内容区域内边距，默认由 GroupBox Token 提供。

稳定 template part：

| Template Part | 类型 | 职责 |
| --- | --- | --- |
| `PART_Frame` | `Border` | 承载整体布局根节点，并提供边框 bounds 参考。 |
| `PART_HeaderContainer` | `Panel` | Header 行容器。 |
| `PART_HeaderContent` | `Decorator` | Header 内容实际 bounds，用于标题缺口和对齐计算。 |
| `PART_HeaderIconPresenter` | `IconPresenter` | Header 图标展示。 |
| `PART_HeaderPresenter` | `TextBlock` | Header 标题展示。 |
| `PART_ContentPresenter` | `ContentPresenter` | 内容承载。 |

## 4. 行为与状态模型

GroupBox 自身没有 hover、pressed、loading、selected、expanded 或 checked 状态。它不拦截输入事件，也不为 Header 提供默认点击行为。

GroupBox 的有效状态来自公共属性、模板测量结果和主题资源：

```text
HeaderTitle / HeaderIcon / HeaderTitlePosition
      + Header font properties
      + template measured bounds
        ↓
Header content bounds
        ↓
Frame border bounds + Header gap bounds
        ↓
Background / BorderBrush / BorderThickness / CornerRadius render state
```

`HeaderIcon`、Header 字体、标题位置和 Header 内容变化会影响缺口尺寸或位置。`Background`、`BorderBrush`、`BorderThickness`、`CornerRadius` 改变会影响自绘边框和背景。

## 5. 视觉与主题模型

GroupBox 的默认 Theme 位于 `src/AtomUI.Desktop.Controls/GroupBox/Themes/GroupBoxTheme.axaml`。Theme 提供 Header、Frame 和 Content 的可测量结构，并设置默认背景、边框、圆角、标题颜色、标题字号和间距。

标题缺口是 GroupBox 的视觉契约：Header 内容区域必须从上边框中形成缺口。缺口不能依赖用 `Background` 覆盖边框线，因为 `Background` 允许为透明或半透明。

Theme 职责：

- 设置 `Background`、`BorderBrush`、`BorderThickness`、`CornerRadius` 默认值。
- 设置 `HeaderTitleColor`、`HeaderFontSize`。
- 通过 GroupBox Token 设置 Header 容器外边距、Header 内容内边距、Header 图标间距和内容内边距。
- 根据 `HeaderTitlePosition` 设置 `PART_HeaderContent` 的水平对齐。

Theme 不负责动态构建 Header 或绘制边框。Header 缺口属于控件渲染模型。

## 6. 控件家族或集成关系

GroupBox 是独立桌面控件，不存在派生控件家族。

集成关系：

- Desktop Controls 主题注册：通过桌面控件主题 provider 引入 GroupBox Theme。
- Token 系统：通过 `GroupBoxToken.ScopeProvider` 注册控件 Token 资源作用域。
- Gallery：通过 GroupBox ShowCase 展示基础用法、标题位置、标题样式和标题图标。

GroupBox 不实现 `IFormItemAware`、`ICompactSpaceAware` 或 ItemsControl 相关接口。

## 7. 兼容性不变量

维护 GroupBox 时必须保持以下不变量：

- `HeaderTitle`、`HeaderTitleColor`、`HeaderIcon`、`HeaderTitlePosition`、`HeaderFontSize`、`HeaderFontStyle`、`HeaderFontWeight` 的 API 名称、类型和默认语义不变。
- `GroupBoxTitlePosition.Left`、`Right`、`Center` 的名称和含义不变。
- `PART_Frame`、`PART_HeaderContainer`、`PART_HeaderContent`、`PART_HeaderIconPresenter`、`PART_HeaderPresenter`、`PART_ContentPresenter` 的 template part 名称不变。
- `Background="Transparent"` 时内容区保持透明，同时 Header 标题下方不应出现边框短线。
- Header 图标为 `null` 时图标节点不可见，不保留额外图标占位宽度。
- Header 内容位置改变只影响 Header 水平对齐，不改变内容区域布局语义。
- Token 名称和语义不擅自重命名或删除。

## 8. 专项模型

### Header 缺口渲染模型

GroupBox 的边框不是普通完整矩形边框。Header 内容嵌入在边框上方，边框顶部需要在 Header 内容区域断开。

缺口语义要求：

- Header 内容区域必须从上边框中形成缺口。
- 缺口宽度至少覆盖 `PART_HeaderContent` 的实际 bounds，包含 Header 内容内边距。
- 圆角、边框厚度和 DPI 半像素对齐不应因为缺口绘制出现断裂、重叠或多余短线。
- 透明背景、半透明背景和父容器复杂背景下都不能依赖背景遮挡边框线。

## 9. 文档导航与验证策略

关联文档：

- [GroupBox 桌面版实现原理](implementation.md)
- [GroupBox Token 设计](token.md)
- [GroupBox Changelog](changelog.md)

验证策略：

| 层次 | 验证内容 |
| --- | --- |
| 文档 | `overview.md`、`implementation.md`、`token.md`、`changelog.md` 链接有效。 |
| Public API | API 表和实际 `GroupBox.cs` 属性、枚举值、默认值一致。 |
| AXAML | Template part 名称、Header 对齐 selector、TokenResource 引用不变。 |
| 渲染 | 验证普通背景、透明背景、不同 Header 位置、带图标和无图标场景下标题缺口无多余边框线。 |
| Token | 验证 Gallery API/Token 表与 `GroupBoxToken` 属性一致。 |
| Gallery | 运行 GroupBox ShowCase 相关测试，确认示例结构、API 表、Token 表和示例快照稳定。 |
