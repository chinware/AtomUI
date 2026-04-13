# Expander API 参考

## 命名空间

- **C#**：`AtomUI.Desktop.Controls`
- **AXAML**：`xmlns:atom="https://atomui.net"`

---

## 公共属性

### AtomUI 扩展属性

| 属性 | 类型 | 默认值 | 说明 |
|---|---|---|---|
| `SizeType` | `SizeType` | `Middle` | 尺寸类型（Small / Middle / Large），使用 `AddOwner` 模式共享 |
| `IsShowExpandIcon` | `bool` | `true` | 是否显示展开/收起图标 |
| `ExpandIcon` | `PathIcon?` | `RightOutlined` | 自定义展开图标，默认通过 `BindingPriority.Template` 设置 |
| `AddOnContent` | `object?` | `null` | 头部右侧的附加内容 |
| `AddOnContentTemplate` | `IDataTemplate?` | `null` | 附加内容的数据模板 |
| `IsGhostStyle` | `bool` | `false` | 是否为幽灵风格（头部背景透明） |
| `IsBorderless` | `bool` | `false` | 是否为无边框风格 |
| `TriggerType` | `ExpanderTriggerType` | `Header` | 触发展开/收起的方式（Header / Icon） |
| `ExpandIconPosition` | `ExpanderIconPosition` | `Start` | 展开图标的位置（Start 在左 / End 在右） |
| `HeaderPadding` | `Thickness?` | `null` | 自定义头部内边距（设置后覆盖 Token 默认值） |
| `ContentPadding` | `Thickness?` | `null` | 自定义内容内边距（设置后覆盖 Token 默认值） |
| `IsMotionEnabled` | `bool` | 由 `EnableMotion` Token 控制 | 是否启用展开/收起过渡动画，使用 `AddOwner` 模式共享 |

### 继承自 Avalonia Expander 的属性

| 属性 | 类型 | 默认值 | 说明 |
|---|---|---|---|
| `Header` | `object?` | `null` | 头部内容 |
| `HeaderTemplate` | `IDataTemplate?` | `null` | 头部内容模板 |
| `Content` | `object?` | `null` | 折叠区域内容 |
| `ContentTemplate` | `IDataTemplate?` | `null` | 折叠内容模板 |
| `IsExpanded` | `bool` | `false` | 是否展开 |
| `ExpandDirection` | `ExpandDirection` | `Down` | 展开方向（Down / Up / Left / Right） |
| `IsEnabled` | `bool` | `true` | 是否启用（禁用后不可展开/收起） |

---

## 枚举

### ExpanderTriggerType

| 值 | 说明 |
|---|---|
| `Header` | 点击整个头部区域触发展开/收起 |
| `Icon` | 仅点击展开图标触发 |

### ExpanderIconPosition

| 值 | 说明 |
|---|---|
| `Start` | 展开图标在左侧（Grid.Column=0） |
| `End` | 展开图标在右侧（Grid.Column=3） |

### ExpandDirection（Avalonia 内置）

| 值 | 说明 |
|---|---|
| `Down` | 向下展开（头部在上） |
| `Up` | 向上展开（头部在下） |
| `Left` | 向左展开（头部在右，旋转 90°） |
| `Right` | 向右展开（头部在左，旋转 90°） |

---

## 伪类

| 伪类 | 来源 | 触发条件 |
|---|---|---|
| `:expanded` | `ExpanderPseudoClass` | `IsExpanded = true` |
| `:up` | `ExpanderPseudoClass` | `ExpandDirection = Up` |
| `:down` | `ExpanderPseudoClass` | `ExpandDirection = Down` |
| `:left` | `ExpanderPseudoClass` | `ExpandDirection = Left` |
| `:right` | `ExpanderPseudoClass` | `ExpandDirection = Right` |
| `:custom-header-padding` | `ExpanderPseudoClass` | `HeaderPadding != null` |
| `:custom-content-padding` | `ExpanderPseudoClass` | `ContentPadding != null` |
| `:disabled` | Avalonia 标准 | `IsEnabled = false` |

---

## 模板部件

| 部件名 | 类型 | 说明 |
|---|---|---|
| `PART_Frame` | `Border` | 外层边框容器 |
| `PART_MainLayout` | `DockPanel` | 主布局面板 |
| `PART_ExpandButton` | `IconButton` | 展开/收起图标按钮 |
| `PART_HeaderLayoutTransform` | `LayoutTransformControl` | 头部旋转容器（水平展开时使用） |
| `PART_HeaderLayout` | `Grid` | 头部 4 列网格 |
| `PART_HeaderPresenter` | `ContentPresenter` | 头部内容呈现器 |
| `PART_HeaderDecorator` | `Border` | 头部装饰边框（背景色 + 分隔线） |
| `PART_ContentPresenter` | `ContentPresenter` | 折叠内容呈现器 |
| `PART_AddOnContentPresenter` | `ContentPresenter` | 附加内容呈现器 |
| `PART_ContentMotionActor` | `LayoutAwareMotionActor` | 展开/收起动画执行器 |

---

## 实现的接口

| 接口 | 定义位置 | 作用 |
|---|---|---|
| `IMotionAwareControl` | `AtomUI.Controls` | 动画开关（`IsMotionEnabled`） |
