# Pagination API 参考

## 命名空间

```csharp
namespace AtomUI.Desktop.Controls;
```

AXAML 命名空间前缀：`atom`（`xmlns:atom="https://atomui.net"`）

---

## 枚举类型

### PaginationAlign

分页对齐方式枚举，控制分页组件在父容器中的水平对齐。

| 值 | 说明 |
|---|---|
| `Start` | 左对齐（默认） |
| `Center` | 居中对齐 |
| `End` | 右对齐 |

### SizeType（来自 `AtomUI.Controls`）

尺寸类型枚举。

| 值 | 说明 |
|---|---|
| `Small` | 迷你尺寸 |
| `Middle` | 标准尺寸（默认） |
| `Large` | 大号尺寸 |

### PaginationItemType（internal）

页码导航项类型枚举，控制每个导航项的外观和行为。

| 值 | 说明 |
|---|---|
| `Previous` | 上一页按钮 |
| `PageIndicator` | 页码数字按钮 |
| `Next` | 下一页按钮 |
| `Ellipses` | 省略号按钮 |

---

## AbstractPagination（抽象基类）

`AbstractPagination` 是 `Pagination` 和 `SimplePagination` 的公共抽象基类，定义了所有分页控件共享的核心属性、事件和分页逻辑。

**继承**：`TemplatedControl` → `AbstractPagination`
**实现接口**：`ISizeTypeAware`、`IMotionAwareControl`

### 常量

| 常量名 | 值 | 说明 |
|---|---|---|
| `DefaultPageSize` | `10` | 默认每页条数 |
| `DefaultCurrentPage` | `1` | 默认当前页码 |

### 公共属性（StyledProperty）

| 属性名 | 类型 | 默认值 | 说明 |
|---|---|---|---|
| `IsHideOnSinglePage` | `bool` | `false` | 仅一页时是否自动隐藏分页组件 |
| `Align` | `PaginationAlign` | `PaginationAlign.Start` | 分页组件水平对齐方式 |
| `SizeType` | `SizeType` | `SizeType.Middle` | 分页尺寸（共享属性，通过 `AddOwner` 注册） |
| `CurrentPage` | `int` | `1` | 当前页码（必须 > 0） |
| `PageSize` | `int` | `10` | 每页数据条数（允许值：0, 10, 20, 50, 100） |
| `Total` | `int` | `0` | 数据总条数（必须 ≥ 0） |
| `IsMotionEnabled` | `bool` | 跟随全局 Token | 是否启用过渡动画（共享属性） |

### 只读属性（DirectProperty）

| 属性名 | 类型 | 说明 |
|---|---|---|
| `PageCount` | `int` | 总页数，由 `Total` / `PageSize` 自动计算（向上取整） |

### 事件

| 事件名 | 类型 | 说明 |
|---|---|---|
| `CurrentPageChanged` | `EventHandler<PageChangedEventArgs>` | 当前页码变更时触发 |

### PageChangedEventArgs（来自 `AtomUI.Controls`，共享层）

定义于 `src/AtomUI.Controls.Shared/EventArgs/PageChangedArgs.cs`。

| 属性 | 类型 | 说明 |
|---|---|---|
| `PageIndex` | `int` | 当前页码 |
| `TotalPages` | `int` | 总页数 |
| `PageSize` | `int` | 每页条数 |

---

## Pagination（标准分页）

`Pagination` 继承自 `AbstractPagination`，提供完整的页码导航功能，包括省略号折叠、页面大小选择器、快速跳转和总数信息显示。

**继承**：`TemplatedControl` → `AbstractPagination` → `Pagination`

### 常量

| 常量名 | 值 | 说明 |
|---|---|---|
| `MaxNavItemCount` | `11` | 导航区最大项数（1 上一页 + 9 页码/省略号 + 1 下一页） |

### 公共属性（StyledProperty）

| 属性名 | 类型 | 默认值 | 说明 |
|---|---|---|---|
| `IsShowSizeChanger` | `bool` | `false` | 是否显示页面大小选择器（10/20/50/100） |
| `IsShowQuickJumper` | `bool` | `false` | 是否显示快速跳转输入框 |
| `IsShowTotalInfo` | `bool` | `false` | 是否显示总数信息 |
| `TotalInfoTemplate` | `string?` | 由本地化资源控制 | 总数信息模板字符串，支持 `${Total}`、`${RangeStart}`、`${RangeEnd}` 变量 |

### 继承自 AbstractPagination 的属性

| 属性名 | 类型 | 默认值 | 说明 |
|---|---|---|---|
| `IsHideOnSinglePage` | `bool` | `false` | 仅一页时是否自动隐藏 |
| `Align` | `PaginationAlign` | `Start` | 水平对齐方式 |
| `SizeType` | `SizeType` | `Middle` | 分页尺寸 |
| `CurrentPage` | `int` | `1` | 当前页码 |
| `PageSize` | `int` | `10` | 每页条数 |
| `Total` | `int` | `0` | 数据总条数 |
| `IsMotionEnabled` | `bool` | 跟随全局 Token | 是否启用过渡动画 |
| `PageCount` | `int` | 自动计算 | 总页数（只读） |

### 继承自 TemplatedControl 的常用属性

| 属性名 | 类型 | 默认值 | 说明 |
|---|---|---|---|
| `IsEnabled` | `bool` | `true` | 是否启用 |
| `Foreground` | `IBrush?` | 由主题控制 | 前景色 |
| `HorizontalAlignment` | `HorizontalAlignment` | `Left` | 水平对齐 |

### 事件

| 事件名 | 类型 | 说明 |
|---|---|---|
| `CurrentPageChanged` | `EventHandler<PageChangedEventArgs>` | 当前页码变更时触发（继承自 `AbstractPagination`） |

---

## SimplePagination（简洁分页）

`SimplePagination` 继承自 `AbstractPagination`，提供紧凑的前进/后退分页体验，支持只读和可编辑两种模式。

**继承**：`TemplatedControl` → `AbstractPagination` → `SimplePagination`

### 公共属性（StyledProperty）

| 属性名 | 类型 | 默认值 | 说明 |
|---|---|---|---|
| `IsReadOnly` | `bool` | `true` | 是否只读模式。`true` 时显示纯文本 "当前页 / 总页数"；`false` 时当前页替换为可编辑输入框 |

### 继承自 AbstractPagination 的属性

| 属性名 | 类型 | 默认值 | 说明 |
|---|---|---|---|
| `IsHideOnSinglePage` | `bool` | `false` | 仅一页时是否自动隐藏 |
| `Align` | `PaginationAlign` | `Start` | 水平对齐方式 |
| `SizeType` | `SizeType` | `Middle` | 分页尺寸 |
| `CurrentPage` | `int` | `1` | 当前页码 |
| `PageSize` | `int` | `10` | 每页条数 |
| `Total` | `int` | `0` | 数据总条数 |
| `IsMotionEnabled` | `bool` | 跟随全局 Token | 是否启用过渡动画 |
| `PageCount` | `int` | 自动计算 | 总页数（只读） |

### 事件

| 事件名 | 类型 | 说明 |
|---|---|---|
| `CurrentPageChanged` | `EventHandler<PageChangedEventArgs>` | 当前页码变更时触发（继承自 `AbstractPagination`） |

---

## 内部控件类型（Internal）

以下控件为 `internal` 类型，开发者不能直接实例化使用，但了解它们有助于理解模板结构和样式选择器。

### PaginationNav

页码导航容器，继承自 `SelectingItemsControl`，管理所有 `PaginationNavItem` 实例。

| 属性名 | 类型 | 说明 |
|---|---|---|
| `SizeType` | `SizeType` | 尺寸类型（双向绑定至父 `Pagination`） |
| `IsMotionEnabled` | `bool` | 是否启用动画 |
| `ItemSpacing` | `double` | 页码按钮之间的间距（internal） |

### PaginationNavItem

单个页码按钮，继承自 `ContentControl`，实现 `ISelectable`。

| 属性名 | 类型 | 说明 |
|---|---|---|
| `IsSelected` | `bool` | 是否被选中（当前页） |
| `PaginationItemType` | `PaginationItemType` | 导航项类型（Previous/PageIndicator/Next/Ellipses） |
| `SizeType` | `SizeType` | 尺寸类型 |
| `IsMotionEnabled` | `bool` | 是否启用动画 |
| `Icon` | `PathIcon?` | 导航项图标（用于 ←/→/... 等） |
| `IsPressed` | `bool` | 是否按下状态（只读） |

| 事件名 | 类型 | 说明 |
|---|---|---|
| `Click` | `EventHandler<RoutedEventArgs>` | 点击事件（冒泡路由） |

### QuickJumperBar

快速跳转栏，显示 "Go to [输入框] Page" 布局。

| 属性名 | 类型 | 说明 |
|---|---|---|
| `JumpToText` | `string?` | 跳转提示文本（如 "Go to"） |
| `PageText` | `string?` | 页文本（如 "Page"） |
| `SizeType` | `SizeType` | 尺寸类型 |

| 事件名 | 类型 | 说明 |
|---|---|---|
| `JumpRequest` | `EventHandler<QuickJumpArgs>` | 用户输入页码按 Enter 后触发 |

### QuickJumpEdit

纯数字输入框，继承自 `LineEdit`，限制输入范围。

| 属性名 | 类型 | 默认值 | 说明 |
|---|---|---|---|
| `Minimum` | `int` | `1` | 最小允许输入值 |
| `Maximum` | `int` | `int.MaxValue` | 最大允许输入值 |

### PageSizeComboBoxItem

页面大小下拉选项，继承自 `ComboBoxItem`。

| 属性名 | 类型 | 说明 |
|---|---|---|
| `PageSize` | `int` | 对应的每页条数 |

---

## 伪类（Pseudo-Classes）

### PaginationNavItem 支持的伪类

| 伪类 | 触发条件 |
|---|---|
| `:selected` | 当前页码被选中 |
| `:pressed` | 按钮按下状态 |
| `:pointerover` | 鼠标悬浮 |
| `:disabled` | 禁用状态 |

### Pagination / SimplePagination 支持的伪类

| 伪类 | 触发条件 |
|---|---|
| `:disabled` | `IsEnabled == false` |

### PaginationNavItem 属性选择器

| 选择器 | 说明 |
|---|---|
| `[PaginationItemType=Previous]` | 上一页按钮 |
| `[PaginationItemType=Next]` | 下一页按钮 |
| `[PaginationItemType=PageIndicator]` | 页码数字按钮 |
| `[PaginationItemType=Ellipses]` | 省略号 |
| `[SizeType=Small]` | 迷你尺寸 |
| `[SizeType=Middle]` | 标准尺寸 |
| `[SizeType=Large]` | 大号尺寸 |

---

## 实现的接口

### ISizeTypeAware

```csharp
public SizeType SizeType { get; set; }
```

通过 `SizeType` 属性控制分页组件的整体尺寸，影响页码按钮高度、图标大小、字体大小等。

### IMotionAwareControl

```csharp
public bool IsMotionEnabled { get; set; }
```

控制页码按钮的背景色、边框色、前景色过渡动画是否启用。

---

## 本地化资源

Pagination 通过 `LanguageProvider` 系统支持本地化文本。

### 英文 (en_US)

| 资源键 | 值 | 说明 |
|---|---|---|
| `JumpToText` | `"Go to"` | 快速跳转提示文本 |
| `PageText` | `"Page"` | 页面文本 |
| `TotalInfoFormat` | `"Total ${Total} items"` | 总数信息默认模板 |

### 中文 (zh_CN)

| 资源键 | 值 | 说明 |
|---|---|---|
| `JumpToText` | `"跳至"` | 快速跳转提示文本 |
| `PageText` | `"页"` | 页面文本 |
| `TotalInfoFormat` | `"共 ${Total} 项"` | 总数信息默认模板 |

---

## 模板部件常量

### PaginationThemeConstants

| 常量 | 值 | 说明 |
|---|---|---|
| `RootLayoutPart` | `"PART_RootLayout"` | 根布局 StackPanel |
| `NavPart` | `"PART_Nav"` | PaginationNav 导航容器 |
| `SizeChangerPresenterPart` | `"PART_SizeChangerPresenter"` | 页面大小选择器 ContentPresenter |
| `QuickJumperBarPresenterPart` | `"PART_QuickJumperBarPresenter"` | 快速跳转栏 ContentPresenter |
| `TotalInfoPresenterPart` | `"PART_TotalInfoPresenter"` | 总数信息 ContentPresenter |

### SimplePaginationThemeConstants

| 常量 | 值 | 说明 |
|---|---|---|
| `RootLayoutPart` | `"PART_RootLayoutPart"` | 根布局 StackPanel |
| `PreviousNavItemPart` | `"PART_PreviousNavItem"` | 上一页按钮 |
| `NextNavItemPart` | `"PART_NextNavItem"` | 下一页按钮 |
| `InfoIndicatorPart` | `"PART_InfoIndicator"` | 页码文本 TextBlock |
| `QuickJumperPart` | `"PART_QuickJumper"` | 可编辑页码输入框 |

### PaginationNavThemeConstants

| 常量 | 值 | 说明 |
|---|---|---|
| `FramePart` | `"PART_Frame"` | 导航区外框 Border |
| `ItemsPresenterPart` | `"PART_ItemsPresenter"` | 项目展示器 |

### PaginationQuickJumperBarThemeConstants

| 常量 | 值 | 说明 |
|---|---|---|
| `RootLayoutPart` | `"PART_RootLayout"` | 根布局 StackPanel |
| `JumpToContentPresenterPart` | `"PART_JumpToContentPresenter"` | "Go to" 文本展示器 |
| `PageContentPresenterPart` | `"PART_PageContentPresenter"` | "Page" 文本展示器 |
| `PageLineEditPart` | `"PART_PageLineEdit"` | 页码输入框 |
