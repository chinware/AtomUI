# GroupBox 桌面版架构设计

关联文档：[Token 设计](token.md)、[Changelog](changelog.md)。

## 1. 控件定位

GroupBox 是桌面端数据展示类分组容器，用于在一块有边框的区域中展示一组相关内容，并通过 Header 标题、图标和标题位置表达分组语义。

GroupBox 的职责是组织和标注内容区域，不负责内容项布局、折叠展开、表单校验、数据绑定集合管理或异步加载。需要折叠行为时应使用 Collapse/Expander 类控件；需要卡片信息组织时应使用 Card。

## 2. 设计语言

GroupBox 的视觉语义接近 fieldset：边框提供分组边界，Header 嵌入在边框上方的视觉通道中，内容区域在边框内部保持稳定留白。标题可以位于左侧、居中或右侧，但标题本身始终是分组名称，不应承担操作入口或复杂工具栏职责。

Header 支持图标，用于强化分组主题。图标不改变 GroupBox 的交互模型，只参与 Header 排版和视觉表达。

## 3. 架构分层

GroupBox 采用以下分层：

| 层级 | 职责 |
| --- | --- |
| Public API | 暴露 Header 文本、图标、标题位置和标题字体样式；继承 ContentControl 的内容、背景、边框和圆角能力。 |
| Effective State | 将 Header 内容实际 bounds、边框 bounds、背景、边框和圆角归一为渲染输入。 |
| Template Contract | 提供 Header 容器、Header 内容、Frame 和 ContentPresenter 的稳定模板节点。 |
| Theme | 设置默认背景、边框、圆角、标题颜色、标题字号和 Header/Content 间距。 |
| Token | 将 SharedToken 映射为 GroupBox 专属内容内边距、Header 外边距、Header 内容内边距和图标间距。 |
| Integration | 通过 `UseDesktopControls()` 注册主题和 Token，不依赖 Form、Compact、Popup 或 ItemsControl 体系。 |

## 4. API 设计

GroupBox 继承 `ContentControl`，公共 API 分为 Header API 和继承的容器 API。

Header API：

| API | 类型 | 语义 |
| --- | --- | --- |
| `HeaderTitle` | `string?` | Header 显示的分组标题。 |
| `HeaderTitleColor` | `IBrush?` | Header 标题颜色。默认来自 `ColorText`。 |
| `HeaderIcon` | `PathIcon?` | Header 标题前的图标。为 `null` 时图标节点不可见。 |
| `HeaderTitlePosition` | `GroupBoxTitlePosition` | Header 内容水平位置，支持 `Left`、`Right`、`Center`。 |
| `HeaderFontSize` | `double` | Header 标题字号。默认来自全局字号。 |
| `HeaderFontStyle` | `FontStyle` | Header 标题字体样式。 |
| `HeaderFontWeight` | `FontWeight` | Header 标题字重。默认 `Normal`。 |

继承 API：

- `Content` / `ContentTemplate` 定义分组内容。
- `Background` 定义内容区域背景。该值允许为透明或半透明。
- `BorderBrush` / `BorderThickness` 定义分组边框。
- `CornerRadius` 定义边框圆角。
- `Padding` 定义内容区域内边距，默认由 GroupBox Token 提供。

API 兼容要求：

- 不新增 Header 内容对象 API 替代 `HeaderTitle` / `HeaderIcon`，除非获得明确授权。
- 不改变 `HeaderTitlePosition` 枚举值名称、默认值或布局语义。
- 不把 `Background="Transparent"` 解释为禁用 Header 缺口；透明背景仍应保持标题下方无边框线。

## 5. 行为交互模型

GroupBox 自身没有 hover、pressed、loading、selected、expanded 或 checked 状态。它不拦截输入事件，也不为 Header 提供默认点击行为。

控件行为由以下模型组成：

- 内容模型：由 `ContentControl` 承载任意单个内容对象。
- Header 显示模型：根据 `HeaderTitle`、`HeaderIcon` 和 `HeaderTitlePosition` 排列标题内容。
- 渲染模型：根据 Header 内容 bounds 计算边框起始位置和标题缺口。
- 主题模型：通过 Theme 和 Token 设置默认视觉值。

## 6. 状态模型

GroupBox 的有效状态主要来自公共属性、模板测量结果和主题资源：

```text
HeaderTitle / HeaderIcon / HeaderTitlePosition
        + Header font properties
        + Template measured bounds
        ↓
Header content bounds
        ↓
Frame border bounds + Header gap bounds
        ↓
Background / BorderBrush / BorderThickness / CornerRadius 渲染
```

`HeaderIcon` 改变会影响 Header 内容宽度和边框缺口宽度，因此必须触发布局重新测量。`Background`、`BorderBrush`、`BorderThickness`、`CornerRadius` 改变会影响自绘边框和背景，必须触发重绘；其中 `BorderThickness` 和 `CornerRadius` 也可能影响布局。

## 7. 模板与视觉架构

GroupBox 的模板包含以下稳定节点：

| Template Part | 类型 | 职责 |
| --- | --- | --- |
| `PART_Frame` | `Border` | 承载整体布局根节点，并提供边框 bounds 参考。 |
| `PART_HeaderContainer` | `Panel` | Header 行容器，位于顶部。 |
| `PART_HeaderContent` | `Decorator` | Header 内容实际 bounds，用于标题缺口和对齐计算。 |
| `PART_HeaderIconPresenter` | `IconPresenter` | Header 图标展示。 |
| `PART_HeaderPresenter` | `TextBlock` | Header 标题展示。 |
| `PART_ContentPresenter` | `ContentPresenter` | 内容承载。 |

视觉层级中，`PART_HeaderContent` 不只是布局节点，也是渲染模型中的缺口计算依据，不应在主题优化中删除或替换为不可定位的结构。`PART_Frame` 是自定义渲染的布局参考节点，也不应改名。

标题缺口渲染要求：

- Header 内容区域必须从上边框中形成缺口。
- 缺口不能依赖用 `Background` 覆盖边框线，因为 `Background` 允许为透明或半透明。
- 绘制边框时应使用 Header 内容 bounds 排除边框几何，或使用等价的不透背景无关方案。
- Header 缺口宽度应至少覆盖 `PART_HeaderContent` 的实际 bounds，包含 Header 内容内边距。
- 圆角、边框厚度和 DPI 半像素对齐不应因为缺口绘制出现断裂、重叠或多余短线。

## 8. Theme 架构

GroupBox 的默认 Theme 位于 `src/AtomUI.Desktop.Controls/GroupBox/Themes/GroupBoxTheme.axaml`。

Theme 职责：

- 设置 `Background` 为 `ColorBgContainer`。
- 设置 `BorderBrush` 为 `ColorBorder`。
- 设置 `BorderThickness` 为全局边框厚度。
- 设置 `CornerRadius` 为全局圆角。
- 设置 `HeaderTitleColor` 和 `HeaderFontSize`。
- 通过 GroupBox Token 设置 Header 容器外边距、Header 内容内边距、Header 图标间距和内容内边距。
- 根据 `HeaderTitlePosition` 设置 `PART_HeaderContent` 的水平对齐。

Theme 不负责动态构建 Header 或绘制边框。Header 缺口属于控件渲染模型，Theme 只提供可测量的 Header 结构。

## 9. 控件家族或集成关系

GroupBox 是独立桌面控件，不存在派生控件家族。它与以下系统集成：

- Desktop Controls 主题注册：通过桌面控件主题 provider 引入 GroupBox Theme。
- Token 系统：通过 `GroupBoxToken.ScopeProvider` 注册控件 Token 资源作用域。
- Gallery：通过 GroupBox ShowCase 展示基础用法、标题位置、标题样式和标题图标。

GroupBox 不实现 `IFormItemAware`、`ICompactSpaceAware` 或 ItemsControl 相关接口。

## 10. 兼容性不变量

维护 GroupBox 时必须保持以下不变量：

- `HeaderTitle`、`HeaderTitleColor`、`HeaderIcon`、`HeaderTitlePosition`、`HeaderFontSize`、`HeaderFontStyle`、`HeaderFontWeight` 的 API 名称、类型和默认语义不变。
- `GroupBoxTitlePosition.Left`、`Right`、`Center` 的名称和含义不变。
- `PART_Frame`、`PART_HeaderContainer`、`PART_HeaderContent`、`PART_HeaderIconPresenter`、`PART_HeaderPresenter`、`PART_ContentPresenter` 的 template part 名称不变。
- `Background="Transparent"` 时内容区保持透明，同时 Header 标题下方不应出现边框短线。
- Header 图标为 `null` 时图标节点不可见，不保留额外图标占位宽度。
- Header 内容位置改变只影响 Header 水平对齐，不改变内容区域布局语义。
- Token 名称和语义不擅自重命名或删除。

## 11. 专项模型

### Header 缺口渲染模型

GroupBox 的边框不是普通完整矩形边框。Header 内容嵌入在边框上方，边框顶部需要在 Header 内容区域断开。

渲染模型应按以下职责拆分：

1. 背景绘制：绘制内容区域背景，允许透明、半透明和普通实色。
2. Header bounds 计算：从 `PART_HeaderContent` 获取相对 GroupBox 的实际位置和尺寸。
3. 边框绘制：以边框几何减去 Header gap 几何作为最终边框区域。
4. 缓存策略：当尺寸、边框厚度、圆角或 Header bounds 变化时重建几何；普通重绘复用缓存。

该模型避免把 Header 背景色作为遮挡层，因此能够兼容透明背景、父容器复杂背景和主题切换。

## 12. 验证策略

GroupBox 变更按以下层次验证：

| 层次 | 验证内容 |
| --- | --- |
| 文档 | `overview.md`、`token.md`、`changelog.md` 链接有效；分类入口包含 GroupBox 链接。 |
| Public API | API 表和实际 `GroupBox.cs` 属性、枚举值、默认值一致。 |
| AXAML | Template part 名称、Header 对齐 selector、TokenResource 引用不变。 |
| 渲染 | 验证普通背景、透明背景、不同 Header 位置、带图标和无图标场景下标题缺口无多余边框线。 |
| Token | 验证 Gallery API/Token 表与 `GroupBoxToken` 属性一致。 |
| Gallery | 运行 `GroupBoxShowCasePageTests`，确认 ShowCase 结构、API 表、Token 表和示例快照稳定。 |

