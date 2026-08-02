# ComboBox 桌面版架构设计

本文档定义 `ComboBox` 桌面版的最新设计定位、公共契约、状态模型、视觉主题关系和兼容边界。通用控件研发约束见 [控件研发标准](../../../../engineering/control-development-guidelines.md)，内部实现原理见 [ComboBox 桌面版实现原理](implementation.md)，ComboBox Token 的专项设计见 [ComboBox Token 设计](token.md)，设计和契约变化记录见 [ComboBox Changelog](changelog.md)。

## 1. 控件定位

| 项 | 值 |
| --- | --- |
| NuGet 包 | `AtomUI.Desktop.Controls` |
| .NET 命名空间 | `AtomUI.Desktop.Controls` |
| AXAML 命名空间 | `https://atomui.net` |
| Gallery 页面 | `controlgallery/AtomUIGallery/ShowCases/Navigation/ComboBox` |
| 控件状态 | Stable |

ComboBox 是 AtomUI 桌面控件体系中的组合框控件，用于在可输入文本框和候选列表之间完成选择。

ComboBox 不负责远程自动完成、树形选择或多列数据选择。这些职责应由业务层、组合控件或更专用的 AtomUI 控件承担。

主要源码入口：

- `src/AtomUI.Desktop.Controls/ComboBox`

## 2. 设计语言

ComboBox 的设计语言围绕控件职责、可观察状态和主题契约组织，而不是围绕模板节点组织。

| 维度 | 含义 | ComboBox 中的表达 |
| --- | --- | --- |
| 产品语义 | 控件在界面中承担的稳定职责。 | ComboBox 是 AtomUI 桌面控件体系中的组合框控件，用于在可输入文本框和候选列表之间完成选择。 |
| 内容承载 | 用户数据、展示内容、集合项或操作入口如何进入控件。 | `ContentLeftAddOn`、`ContentLeftAddOnTemplate`、`ContentRightAddOn`、`ContentRightAddOnTemplate`、`FilterValue`、`FilterValueSelector`、`LeftAddOnTemplate`、`OptionFontSize` 等 9 项。 |
| 状态反馈 | public API、内部状态和伪类如何形成用户可感知反馈。 | open/close、collection/filter、input/value、motion、visual option。 |
| 主题语义 | ControlTheme、SharedToken、控件 Token 和模板绑定如何表达视觉。 | ComboBox Token + ControlTheme。 |

## 3. API 与契约模型

ComboBox 的公共契约由 public/protected 类型成员、Avalonia 属性、事件、命令、template part、伪类、ControlTheme key 和资源 key 共同组成。维护时应先确认这些契约是否已经被源码、Gallery 示例或文档暴露。

核心 public surface 按语义分组维护：

| 契约组 | 代表成员 | 维护含义 |
| --- | --- | --- |
| 内容与数据 | `ContentLeftAddOn`、`ContentLeftAddOnTemplate`、`ContentRightAddOn`、`ContentRightAddOnTemplate`、`FilterValue`、`FilterValueSelector`、`LeftAddOnTemplate`、`OptionFontSize`、`RightAddOnTemplate` | 定义控件展示内容、输入数据、模板或业务对象入口。 |
| 选择与集合 | `SelectedItem`、`SelectedIndex`、`DropDownDisplayPageSize`、`Filter`、`IsFilterEnabled` | 维护选择、展开、过滤、分页、分组或集合状态。 |
| 交互与状态 | `IsAllowClear`、`IsMotionEnabled`、`ShouldUseOverlayPopup`、`Status`、`IsShowOverflowTip`、`OverflowTipDelay`、`OverflowTipPlacement` | 表达用户可观察状态、可用性、清除、加载、反馈和非编辑态选中内容溢出提示语义。 |
| 视觉与布局 | `SizeType`、`StyleVariant` | 影响尺寸、位置、颜色、形状、密度和模板视觉变量。 |
| 其他稳定入口 | `LeftAddOn`、`RightAddOn` | 保留为 public surface，变更前需确认 Gallery 和用户 XAML 依赖。 |

当前没有抽取到控件专属 public 事件；交互通知主要来自继承事件、命令或 Gallery 可观察状态。

主要公开类型与枚举：

- 类型：`ComboBox`、`ComboBoxHandle`、`ComboBoxItem`。
- 枚举：无。

稳定 template part：

| Template Part | 类型 | 职责 |
| --- | --- | --- |
| `PART_ComboBoxHandle` | `?` | 稳定模板协作入口，重命名前必须同步主题和实现。 |
| `PART_ContentRightAddOnPresenter` | `?` | 展示用户内容、文本、图标或模板化数据。 |
| `PART_EditableTextBox` | `?` | 承载文本输入、过滤、显示或编辑入口。 |
| `PART_EmptyIndicator` | `?` | 展示指示器、进度、分页或状态反馈。 |
| `PART_FormFeedBack` | `?` | 稳定模板协作入口，重命名前必须同步主题和实现。 |
| `PART_ItemsPresenter` | `?` | 展示用户内容、文本、图标或模板化数据。 |
| `PART_OpenIndicatorButton` | `?` | 承载用户触发入口、导航或关闭动作。 |
| `PART_Popup` | `?` | 承载弹层宿主、打开关闭或候选内容。 |
| `PART_TextPresenter` | `?` | 展示用户内容、文本、图标或模板化数据。 |

当前未抽取到控件专属伪类；主题主要依赖 Avalonia 标准伪类、模板绑定和内部 StyledProperty。

## 4. 行为与状态模型

ComboBox 的状态流按以下路径收敛：

```text
Public API / inherited command / item source / user input
  -> 控件实例状态
  -> effective state / pseudo-class / template property
  -> ControlTheme selector / presenter / renderer
  -> Gallery 可观察行为
```

状态维护规则：

- Disabled 或不可交互状态优先屏蔽 pointer、keyboard、motion 和提交类反馈。
- open/close、collection/filter、input/value、motion、visual option 状态由控件实例或明确的数据 owner 推导，不能在 template part 之间双向竞争。
- 单选结果以继承的 `SelectedItem` / `SelectedIndex` 为准；AtomUI Form 集成使用 `SelectedItem` 作为 ComboBox 的默认表单值，`SetFormValue`、`GetFormValue` 和 `ClearFormValue` 不应转换为字符串或读取展示文本。
- 模板重套用时必须把 public API 对应状态回放到新的 part、伪类和主题变量。
- 集合、弹层、异步、动效或窗口相关状态必须能处理 reset、close、cancel、detach 和 owner 释放。

## 5. 视觉与主题模型

ComboBox 的视觉模型由控件模板、ControlTheme、SharedToken 和必要的控件 Token 共同构成。

| 主题文件 | 职责 |
| --- | --- |
| `ComboBoxHandleTheme.axaml` | 定义局部操作入口、按钮或 handle 的状态视觉。 |
| `ComboBoxItemTheme.axaml` | 定义集合项、容器项或局部单元的状态视觉。 |
| `ComboBoxTheme.axaml` | 定义布局、内容承载或框架节点视觉。 |

非编辑态选中内容的完整文本提示复用共享 `OverflowTip` attached behavior。模板只在 `SelectedContentPresenter` 上接入 `IsShowOverflowTip`、`OverflowTipDelay`、`OverflowTipPlacement` 和 `SelectionBoxItem`；`IsEditable=true` 时编辑输入框不默认启用该提示。

ComboBox 使用 `ComboBoxToken` 作为控件 Token scope。Token 只表达组件视觉语义，不承载 open/close、collection/filter、input/value、motion、visual option 运行时状态。

主题维护规则：

- 不删除或重命名已经稳定的 ControlTheme key、template part、伪类和资源 key。
- 不把可由 AXAML 表达的模板状态迁移为 C# 动态创建视觉。
- 不把 hover、pressed、selected、expanded、loading、filter、popup open 等运行时状态写入 Token。
- Browser 或平台特化主题必须保持同一 API 的语义一致。

## 6. 控件家族或集成关系

ComboBox 与同分类控件共享尺寸、状态、Token、Gallery 展示和验证规则。组合或派生控件应显式说明哪些 API 被继承、覆盖或不支持。

主要协作类型：

- `ComboBox`：模板协作类型，承载内容展示、宿主或视觉边界。
- `ComboBoxHandle`：控件核心或内部协作类型，维护 public surface 与主题可观察行为。
- `ComboBoxItem`：集合项、节点或容器类型，承载单项状态和模板协作。
- `ComboBoxToken`：控件 Token scope，负责从全局 token 派生控件语义变量。

集成关系：

- 与 ThemeManager、SharedToken、ControlTheme、控件文档和 Gallery ShowCase 示例保持一致。
- 涉及 ItemsSource、Popup、Flyout、Window、Form 或 CompactSpace 的路径必须保持生命周期释放和数据状态同步。
- 源码目录中的共享基类和内部协作类型形成维护边界，不能只修改桌面包装类而忽略共享状态 owner。

## 7. 兼容性不变量

维护 ComboBox 时必须保持以下不变量：

- 不擅自新增、删除、重命名或改变 public/protected API、Avalonia 属性、事件和默认值。
- 不破坏 template part、伪类、ControlTheme key、Token 名称和资源 key。
- 不改变 Gallery 已展示的 XAML 用法、默认外观、交互顺序和状态优先级。
- Template part 重新应用、集合替换、弹层关闭、窗口失活和控件 detach 时必须释放旧订阅和资源宿主。
- 不通过隐藏延迟、强制刷新或吞异常掩盖状态同步问题。
- 不引入运行时反射扫描作为 API、Token 或数据路径发现机制。
- 文档只描述当前稳定设计；历史变化记录在 `changelog.md`。

## 8. 专项模型

### 8.1 弹层与宿主模型

ComboBox 涉及弹层、窗口或 overlay 宿主时，打开状态、取消事件、定位和宿主释放必须保持一致。重复打开、关闭、窗口失活和 template reapply 都必须释放旧宿主引用。

### 8.2 集合与数据同步模型

ComboBox 的集合状态必须能处理 source replace、reset、clear 和 container recycle。业务数据对象不应反向持有视觉对象，虚拟化或懒创建路径必须在容器回收时清理旧状态。

### 8.3 Form 值模型

ComboBox 实现 `IFormItemAware` 时以 `SelectedItem` 作为表单值 owner：

- `SetFormValue(value)` 设置 `SelectedItem`，保留业务对象实例。
- `GetFormValue()` 返回当前 `SelectedItem`。
- `ClearFormValue()` 清空 `SelectedItem`，并回到未选择状态。
- `SelectionBoxItem` 只用于展示，不作为 Form 真实值，也不能把对象值降级为字符串。

### 8.4 动效模型

ComboBox 的动效只表达状态变化反馈，不应改变 public API 语义。初始加载、禁用态和卸载路径应能抑制或取消动效，避免保留旧控件实例。

### 8.5 视觉选项模型

ComboBox 的视觉选项通过 public API 归一为 theme variables、伪类或模板绑定。Token 保存组件语义值，不能保存实例运行时状态或业务色值。

## 9. 文档导航、LLMS 导出与验证策略

关联文档：

- [ComboBox 桌面版实现原理](implementation.md)
- [ComboBox Token 设计](token.md)
- [ComboBox Changelog](changelog.md)

LLMS 语义区域：

| Part | AtomUI 节点 | 职责 | 相关 API | 相关 Token | 稳定性 |
| --- | --- | --- | --- | --- | --- |
| `root` | `ComboBox` | 导航控件根语义区域，承载 public API、状态归一和主题入口。 | 见 API 与契约模型 | 见视觉与主题模型 | stable |
| `trigger` | `触发区域` | 承载点击、键盘、打开关闭、跳转或提交入口。 | 见 API 与契约模型 | 见视觉与主题模型 | stable |
| `item` | `导航项区域` | 承载当前项、选中项、禁用项、层级项或分页项状态。 | 见 API 与契约模型 | 见视觉与主题模型 | stable |
| `popup` | `弹层或内容区域` | 承载 flyout、dropdown、tab content、submenu 或候选内容。 | 见 API 与契约模型 | 见视觉与主题模型 | stable |
| `motion` | `动效区域` | 表达打开关闭、选中指示、切换和过渡反馈。 | 见 API 与契约模型 | 见视觉与主题模型 | stable |

LLMS 导出来源：

| LLMS 内容 | 来源 | 说明 |
| --- | --- | --- |
| 单控件完整文档 | `overview.md` + `implementation.md` + `token.md` + Gallery ShowCase | 生成 `controls/combo-box/index-cn.md` |
| 单控件语义文档 | `overview.md` + `implementation.md` + theme/template 信息 | 生成 `controls/combo-box/semantic-cn.md` |
| API 表 | overview.md 语义摘要 + 源码 public surface | 不在 `overview.md` 中复制完整 API 表 |
| Design Token 表 | token.md、Token 类型或第 5 节主题模型 | 不在生成产物中手工维护第二份 Token 表 |
| 示例 | Gallery ShowCase + source snippet catalog | 只引用稳定示例 |
| 源码索引 | `implementation.md` | 用于定位控件源码、主题和测试 |

验证策略：

| 改动类型 | 验证要求 |
| --- | --- |
| 文档改动 | 运行 `git diff --check`，检查相对链接存在。 |
| Public API | 覆盖属性默认值、事件触发、命令和继承语义。 |
| 状态模型 | 覆盖 open/close、collection/filter、input/value、motion、visual option、disabled、hover、pressed、focus 以及控件特有状态。 |
| AXAML/Theme | 检查 template part、伪类、资源 key、Light/Dark 主题和 Browser 主题。 |
| Token | 检查 TokenKind、AXAML token resource、Token 类型、生成数据和 token.md和文档同步。 |
| Gallery | 走查对应 ShowCase 示例和源码片段入口。 |
