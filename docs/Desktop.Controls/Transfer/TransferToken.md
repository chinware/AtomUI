# Transfer Design Token

Transfer 使用 `TransferToken`（Token ID: `"Transfer"`）作为组件级 Design Token。所有视觉属性均从全局 `SharedToken` 派生，不包含硬编码的魔法数字。

---

## Token 资源访问方式

### 在 AXAML 中引用

```xml
<!-- 组件级 Token -->
{atom:TransferTokenResource ListWidth}
{atom:TransferTokenResource ListHeight}
{atom:TransferTokenResource HeaderHeight}

<!-- 全局共享 Token -->
{atom:SharedTokenResource ColorPrimary}
{atom:SharedTokenResource ColorBorder}
{atom:SharedTokenResource BorderRadiusLG}
```

### 在 C# 中访问

Token 通过 `TransferToken.ScopeProvider` 注册到控件的 Token 作用域：

```csharp
this.RegisterTokenResourceScope(TransferToken.ScopeProvider);
```

---

## 组件级 Token 列表

以下 Token 在 `TransferToken` 中定义，所有值在 `CalculateTokenValues(bool isDarkMode)` 中从 `SharedToken` 派生。

| Token 名称 | 类型 | 计算值 | 说明 |
|---|---|---|---|
| `ListWidth` | `double` | `180` | 列表面板默认宽度 |
| `ListWidthLG` | `double` | `250` | 启用分页时的列表面板宽度（大号） |
| `ListHeight` | `double` | `200` | 列表内容区域默认高度 |
| `ItemHeight` | `double` | `SharedToken.ControlHeight` | 列表项高度，跟随全局控件高度 |
| `ItemPadding` | `Thickness` | `(0, (ControlHeight - FontHeight) / 2)` | 列表项纵向内边距，保证文字垂直居中 |
| `HeaderHeight` | `double` | `SharedToken.ControlHeightLG` | 面板头部信息栏高度 |
| `HeaderPadding` | `Thickness` | `(PaddingSM, ceil((ControlHeightLG - LineWidth - FontHeight) / 2))` | 面板头部内边距 |
| `PaginationMargin` | `Thickness` | `(0, MarginXXS)` | 分页器外边距 |
| `DataGridSelectionHeaderMargin` | `Thickness` | `(PaddingXS * 2, 0, 0, 0)` | DataGrid 选择表头外边距（预留） |

---

## 引用的全局 SharedToken

Transfer 的主题中广泛引用以下全局 Token，这些 Token 控制了面板的基础视觉样式：

### 颜色 Token

| SharedToken | 用途 |
|---|---|
| `ColorBorder` | 面板边框颜色 |
| `ColorSplit` | 头部/页脚分割线颜色 |
| `ColorBgContainer` | 头部背景色 |
| `ColorTextDisabled` | 禁用状态文字颜色 |
| `ColorTextDescription` | 搜索框图标颜色 |
| `ColorError` | Error 状态边框颜色 |
| `ColorWarning` | Warning 状态边框颜色 |

### 尺寸与间距 Token

| SharedToken | 用途 |
|---|---|
| `BorderThickness` | 面板边框厚度 |
| `BorderRadiusLG` | 面板圆角半径 |
| `SpacingXXS` | 穿梭按钮之间的间距 |
| `SpacingXS` | 列布局间距 |
| `MarginXS` | 搜索输入框外边距 |
| `FontSizeIcon` | 选择操作下拉菜单图标大小 |
| `EnableMotion` | 是否启用全局动画（绑定到 `IsMotionEnabled`） |

### 本地化 Token

| LangToken | 用途 |
|---|---|
| `TransferLangResource Item` | 单数单位文本（如 "项" / "item"） |
| `TransferLangResource Items` | 复数单位文本（如 "项" / "items"） |

---

## Token 与面板视觉的映射关系

```
┌─────────────────────────────────────────────────────────────┐
│ TransferItemDecorator                                        │
│ BorderBrush ← SharedToken.ColorBorder                       │
│ BorderThickness ← SharedToken.BorderThickness               │
│ CornerRadius ← SharedToken.BorderRadiusLG                   │
│                                                              │
│ ┌─ HeaderFrame ─────────────────────────────────────────┐   │
│ │ Height ← TransferToken.HeaderHeight                    │   │
│ │ Padding ← TransferToken.HeaderPadding                  │   │
│ │ BorderBrush ← SharedToken.ColorSplit                   │   │
│ │ Background ← SharedToken.ColorBgContainer              │   │
│ │ ┌ SelectAllCheckBox ┐ ┌ SelectedInfo ┐ ┌ Title ┐      │   │
│ └──────────────────────────────────────────────────────── │   │
│                                                              │
│ ┌─ FilterInput ─────────────────────────────────────────┐   │
│ │ Margin ← SharedToken.MarginXS                          │   │
│ └──────────────────────────────────────────────────────── │   │
│                                                              │
│ ┌─ ContentPresenter ────────────────────────────────────┐   │
│ │ Height ← TransferToken.ListHeight                      │   │
│ └──────────────────────────────────────────────────────── │   │
│                                                              │
│ ┌─ FooterFrame ─────────────────────────────────────────┐   │
│ │ Padding ← TransferToken.HeaderPadding                  │   │
│ │ BorderBrush ← SharedToken.ColorSplit                   │   │
│ └──────────────────────────────────────────────────────── │   │
└─────────────────────────────────────────────────────────────┘
```

---

## 宽度自适应规则

| 条件 | 使用的宽度 Token |
|---|---|
| `IsStretchView=False` 且 `IsPaginationEnabled=False` | `TransferToken.ListWidth` (180) |
| `IsStretchView=False` 且 `IsPaginationEnabled=True` | `TransferToken.ListWidthLG` (250) |
| `IsStretchView=True` | `double.NaN`（由父容器决定） |

---

## Token 源码位置

- Token 定义：`src/AtomUI.Desktop.Controls/Transfer/TransferToken.cs`
- 基类主题引用：`src/AtomUI.Desktop.Controls/Transfer/Themes/AbstractTransferTheme.axaml`
- 装饰器主题引用：`src/AtomUI.Desktop.Controls/Transfer/Themes/TransferItemDecoratorTheme.axaml`
