# Pagination 自定义样式指南

Pagination 的视觉表现通过 `ControlTheme` + Design Token 系统控制。以下介绍几种常见的自定义方式。

---

## 1. 使用属性直接控制

最简单的方式是通过 Pagination 的公共属性来控制外观和功能：

```xml
<!-- 不同模式 -->
<atom:Pagination Total="500" CurrentPage="1" />
<atom:SimplePagination Total="500" CurrentPage="1" />

<!-- 不同尺寸 -->
<atom:Pagination Total="50" CurrentPage="1" SizeType="Middle" />
<atom:Pagination Total="50" CurrentPage="1" SizeType="Small" />

<!-- 不同对齐方式 -->
<atom:Pagination Total="50" CurrentPage="1" Align="Start" />
<atom:Pagination Total="50" CurrentPage="1" Align="Center" />
<atom:Pagination Total="50" CurrentPage="1" Align="End" />

<!-- 显示附加功能 -->
<atom:Pagination Total="500" CurrentPage="1"
                 IsShowSizeChanger="True"
                 IsShowQuickJumper="True"
                 IsShowTotalInfo="True" />

<!-- 自定义总数模板 -->
<atom:Pagination Total="85" CurrentPage="1" PageSize="20"
                 IsShowTotalInfo="True"
                 TotalInfoTemplate="${RangeStart}-${RangeEnd} of ${Total} items" />

<!-- 单页隐藏 -->
<atom:Pagination Total="5" CurrentPage="1" IsHideOnSinglePage="True" />

<!-- 简洁模式：可编辑 vs 只读 -->
<atom:SimplePagination Total="50" CurrentPage="1" IsReadOnly="True" />
<atom:SimplePagination Total="50" CurrentPage="1" IsReadOnly="False" />
```

> 📖 完整示例参考：`controlgallery/AtomUIGallery/ShowCases/Views/Navigation/PaginationShowCase.axaml`

---

## 2. 通过 Style 覆盖样式

可以在 AXAML 中使用 `Style` 对 Pagination 进行全局或局部样式覆盖：

### 全局统一间距

```xml
<Window.Styles>
    <Style Selector="atom|Pagination">
        <Setter Property="Margin" Value="0,10" />
    </Style>
</Window.Styles>
```

### 按尺寸定制

```xml
<!-- 迷你尺寸的额外样式 -->
<Style Selector="atom|Pagination[SizeType=Small]">
    <Setter Property="Opacity" Value="0.9" />
</Style>
```

### 按对齐方式定制

```xml
<!-- 居中对齐时添加上下间距 -->
<Style Selector="atom|Pagination[Align=Center]">
    <Setter Property="Margin" Value="0,16" />
</Style>
```

### 禁用态样式覆盖

```xml
<!-- 禁用态降低整体透明度 -->
<Style Selector="atom|Pagination:disabled">
    <Setter Property="Opacity" Value="0.5" />
</Style>
```

---

## 3. 通过 ControlTheme 完全替换主题

如果需要彻底替换 Pagination 的模板和样式，可以定义自定义 `ControlTheme`：

```xml
<ControlTheme x:Key="MyCustomPagination" TargetType="atom:Pagination">
    <Setter Property="Template">
        <ControlTemplate>
            <StackPanel Name="PART_RootLayout" Orientation="Horizontal">
                <atom:PaginationNav Name="PART_Nav"
                                    SizeType="{TemplateBinding SizeType}"
                                    IsMotionEnabled="{TemplateBinding IsMotionEnabled}" />
            </StackPanel>
        </ControlTemplate>
    </Setter>
    <!-- 自定义样式... -->
</ControlTheme>

<!-- 使用 -->
<atom:Pagination Theme="{StaticResource MyCustomPagination}"
                 Total="100" CurrentPage="1" />
```

> ⚠️ 注意：完全替换 ControlTheme 需要保证模板部件名称（如 `PART_Nav`）与代码中的常量一致，否则控件逻辑将无法正确绑定。建议优先使用 Style 覆盖。

---

## 4. 禁用动画

```xml
<!-- 禁用页码按钮的过渡动画 -->
<atom:Pagination Total="500" CurrentPage="1" IsMotionEnabled="False" />
<atom:SimplePagination Total="50" CurrentPage="1" IsMotionEnabled="False" />
```

---

## 5. 禁用状态

```xml
<!-- 禁用整个分页组件 -->
<atom:Pagination Total="500" CurrentPage="3"
                 IsShowSizeChanger="True"
                 IsShowQuickJumper="True"
                 IsEnabled="False" />

<atom:SimplePagination Total="50" CurrentPage="1"
                       IsReadOnly="False"
                       IsEnabled="False" />
```

---

## 样式选择器速查

> 说明：Avalonia 样式选择器中使用 `atom|Pagination` 语法引用 `atom` XML 命名空间下的类型，其中 `|` 是命名空间分隔符。

### 按控件类型选择

| 选择器 | 说明 |
|---|---|
| `atom\|Pagination` | 匹配所有标准分页实例 |
| `atom\|SimplePagination` | 匹配所有简洁分页实例 |

### 按尺寸选择

| 选择器 | 说明 |
|---|---|
| `atom\|Pagination[SizeType=Large]` | 大号分页 |
| `atom\|Pagination[SizeType=Middle]` | 标准分页（默认） |
| `atom\|Pagination[SizeType=Small]` | 迷你分页 |

### 按对齐方式选择

| 选择器 | 说明 |
|---|---|
| `atom\|Pagination[Align=Start]` | 左对齐分页 |
| `atom\|Pagination[Align=Center]` | 居中分页 |
| `atom\|Pagination[Align=End]` | 右对齐分页 |

### 按状态选择

| 选择器 | 说明 |
|---|---|
| `atom\|Pagination:disabled` | 禁用状态的分页 |

### 页码项选择器（通过模板内部）

| 选择器 | 说明 |
|---|---|
| `atom\|PaginationNavItem` | 匹配所有页码导航项 |
| `atom\|PaginationNavItem[PaginationItemType=PageIndicator]` | 匹配页码数字按钮 |
| `atom\|PaginationNavItem[PaginationItemType=Previous]` | 匹配上一页按钮 |
| `atom\|PaginationNavItem[PaginationItemType=Next]` | 匹配下一页按钮 |
| `atom\|PaginationNavItem[PaginationItemType=Ellipses]` | 匹配省略号 |
| `atom\|PaginationNavItem:selected` | 匹配选中（当前页）的页码按钮 |
| `atom\|PaginationNavItem:pointerover` | 匹配鼠标悬浮的页码按钮 |
| `atom\|PaginationNavItem:pressed` | 匹配按下的页码按钮 |
| `atom\|PaginationNavItem:disabled` | 匹配禁用的页码按钮 |

### SimplePagination 特有选择器

| 选择器 | 说明 |
|---|---|
| `atom\|SimplePagination[IsReadOnly=True]` | 只读模式（纯文本显示当前页/总页数） |
| `atom\|SimplePagination[IsReadOnly=False]` | 可编辑模式（当前页为输入框） |

### 组合选择器示例

| 选择器 | 说明 |
|---|---|
| `atom\|Pagination[SizeType=Small]:disabled` | 禁用的迷你分页 |
| `atom\|PaginationNavItem[PaginationItemType=PageIndicator]:selected:pointerover` | 悬浮在选中页码上 |
| `atom\|PaginationNavItem[SizeType=Small]` | 迷你尺寸的页码项 |
| `atom\|Pagination /template/ StackPanel#PART_RootLayout` | 访问分页模板内的根布局容器 |
