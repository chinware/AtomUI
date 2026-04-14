# TreeSelect 自定义样式指南

TreeSelect 的视觉表现通过 `ControlTheme` + Design Token 系统控制。以下介绍几种常见的自定义方式。

---

## 1. 使用属性直接控制

最简单的方式是通过 TreeSelect 的公共属性来控制外观：

```xml
<!-- 不同样式变体 -->
<atom:TreeSelect StyleVariant="Outline" PlaceholderText="Outline" />
<atom:TreeSelect StyleVariant="Filled" PlaceholderText="Filled" />
<atom:TreeSelect StyleVariant="Borderless" PlaceholderText="Borderless" />
<atom:TreeSelect StyleVariant="Underlined" PlaceholderText="Underlined" />

<!-- 不同状态 -->
<atom:TreeSelect Status="Error" PlaceholderText="Error" />
<atom:TreeSelect Status="Warning" PlaceholderText="Warning" />

<!-- 不同尺寸 -->
<atom:TreeSelect SizeType="Large" PlaceholderText="Large" />
<atom:TreeSelect SizeType="Middle" PlaceholderText="Middle" />
<atom:TreeSelect SizeType="Small" PlaceholderText="Small" />

<!-- 多选模式 -->
<atom:TreeSelect IsMultiple="True" PlaceholderText="Multiple" />

<!-- 勾选模式 -->
<atom:TreeSelect IsTreeCheckable="True" ShowCheckedStrategy="ShowParent" PlaceholderText="Checkable" />

<!-- 带前缀 AddOn -->
<atom:TreeSelect LeftAddOn="Prefix" PlaceholderText="With prefix" />
<atom:TreeSelect ContentLeftAddOn="Prefix" PlaceholderText="With content prefix" />

<!-- 显示树线 -->
<atom:TreeSelect IsShowLine="True" IsSwitcherRotation="False" PlaceholderText="With tree line" />

<!-- 搜索筛选 -->
<atom:TreeSelect IsFilterEnabled="True" IsAllowClear="True" PlaceholderText="Search..." />

<!-- 最大选择数量 -->
<atom:TreeSelect IsMultiple="True" MaxCount="3" IsShowMaxCountIndicator="True" PlaceholderText="Max 3" />
```

> 📖 完整示例参考：`controlgallery/AtomUIGallery/ShowCases/Views/DataEntry/TreeSelectShowCase.axaml`

---

## 2. 通过 Style 覆盖样式

可以在 AXAML 中使用 `Style` 对 TreeSelect 进行全局或局部样式覆盖：

### 全局统一宽度

```xml
<Window.Styles>
    <Style Selector="atom|TreeSelect">
        <Setter Property="HorizontalAlignment" Value="Stretch" />
    </Style>
</Window.Styles>
```

### 按样式变体定制边框颜色

```xml
<Style Selector="atom|TreeSelect:outline:not(:disabled)">
    <Style Selector="^ /template/ atom|TreeSelectAddOnDecoratedBox#PART_AddOnDecoratedBox">
        <Setter Property="BorderBrush" Value="#722ed1" />
    </Style>
</Style>
```

### 按状态定制

```xml
<!-- 错误状态下自定义文字颜色 -->
<Style Selector="atom|TreeSelect:error">
    <Setter Property="Foreground" Value="Red" />
</Style>

<!-- 警告状态下自定义文字颜色 -->
<Style Selector="atom|TreeSelect:warning">
    <Setter Property="Foreground" Value="Orange" />
</Style>
```

### 使用伪类选择器

```xml
<!-- 下拉打开时的自定义样式 -->
<Style Selector="atom|TreeSelect:dropdownopen">
    <Setter Property="Opacity" Value="0.9" />
</Style>

<!-- 禁用状态的自定义样式 -->
<Style Selector="atom|TreeSelect:disabled">
    <Setter Property="Foreground" Value="Gray" />
</Style>
```

### 按属性选择器

```xml
<!-- 所有多选模式的 TreeSelect 设置最小高度 -->
<Style Selector="atom|TreeSelect[IsMultiple=True]">
    <Setter Property="MinHeight" Value="40" />
</Style>

<!-- 所有大号 TreeSelect 使用粗体 -->
<Style Selector="atom|TreeSelect[SizeType=Large]">
    <Setter Property="FontWeight" Value="SemiBold" />
</Style>
```

---

## 3. 通过 ControlTheme 完全替换主题

如果需要彻底替换 TreeSelect 的模板和样式，可以定义自定义 `ControlTheme`：

```xml
<ControlTheme x:Key="MyCustomTreeSelect" TargetType="atom:TreeSelect">
    <Setter Property="Template">
        <ControlTemplate>
            <!-- 自定义模板 -->
        </ControlTemplate>
    </Setter>
</ControlTheme>

<!-- 使用 -->
<atom:TreeSelect Theme="{StaticResource MyCustomTreeSelect}" />
```

> ⚠️ 注意：完全替换 ControlTheme 将丢失内置的弹窗管理、装饰框、筛选、树线等功能。建议优先使用 Style 覆盖。

---

## 4. 自定义弹窗弹出位置

通过 `Placement` 属性和 `IsPopupMatchSelectWidth` 属性控制弹窗行为：

```xml
<!-- 弹窗在上方弹出，不匹配控件宽度 -->
<atom:TreeSelect Placement="TopEdgeAlignedLeft"
                 IsPopupMatchSelectWidth="False"
                 PlaceholderText="Top left popup" />

<!-- 弹窗在下方右对齐弹出 -->
<atom:TreeSelect Placement="BottomEdgeAlignedRight"
                 PlaceholderText="Bottom right popup" />
```

> 📖 参考：`controlgallery/AtomUIGallery/ShowCases/Views/DataEntry/TreeSelectShowCase.axaml` 中 "Placement" 示例。

---

## 5. 自定义筛选高亮

通过 `FilterHighlightForeground` 和 `FilterHighlightStrategy` 控制搜索匹配的高亮效果：

```xml
<atom:TreeSelect IsFilterEnabled="True"
                 FilterHighlightForeground="Red" />
```

---

## 6. 控制动画行为

```xml
<!-- 禁用弹窗展开/收起动画 -->
<atom:TreeSelect IsMotionEnabled="False" PlaceholderText="No animation" />
```

---

## 样式选择器速查

> 说明：Avalonia 样式选择器中使用 `atom|TreeSelect` 语法引用 `atom` XML 命名空间下的 `TreeSelect` 类型，其中 `|` 是命名空间分隔符。

### 基础选择器

| 选择器 | 说明 |
|---|---|
| `atom\|TreeSelect` | 匹配所有 AtomUI TreeSelect 实例 |
| `atom\|TreeSelect:disabled` | 匹配禁用状态 |
| `atom\|TreeSelect:dropdownopen` | 匹配下拉弹窗打开状态 |
| `atom\|TreeSelect:pressed` | 匹配按下状态 |
| `atom\|TreeSelect:focus-within` | 匹配内部获得焦点的状态 |

### 按样式变体选择

| 选择器 | 说明 |
|---|---|
| `atom\|TreeSelect:outline` | 匹配 Outline 变体（带边框，默认） |
| `atom\|TreeSelect:filled` | 匹配 Filled 变体（填充背景） |
| `atom\|TreeSelect:borderless` | 匹配 Borderless 变体（无边框） |

### 按状态选择

| 选择器 | 说明 |
|---|---|
| `atom\|TreeSelect:error` | 匹配错误状态（`Status == Error`） |
| `atom\|TreeSelect:warning` | 匹配警告状态（`Status == Warning`） |

### 按尺寸选择

| 选择器 | 说明 |
|---|---|
| `atom\|TreeSelect[SizeType=Large]` | 匹配大号 TreeSelect |
| `atom\|TreeSelect[SizeType=Middle]` | 匹配中号 TreeSelect（默认） |
| `atom\|TreeSelect[SizeType=Small]` | 匹配小号 TreeSelect |

### 按属性选择

| 选择器 | 说明 |
|---|---|
| `atom\|TreeSelect[IsMultiple=True]` | 匹配多选模式 |
| `atom\|TreeSelect[IsMultiple=False]` | 匹配单选模式 |
| `atom\|TreeSelect[IsFilterEnabled=True]` | 匹配启用了搜索筛选的 TreeSelect |
| `atom\|TreeSelect[IsDropDownOpen=True]` | 匹配下拉弹窗打开状态（属性选择器写法） |
| `atom\|TreeSelect[StyleVariant=Filled]` | 按属性匹配 Filled 变体 |
| `atom\|TreeSelect[Status=Error]` | 按属性匹配错误状态 |

### 模板部件访问

| 选择器 | 说明 |
|---|---|
| `atom\|TreeSelect /template/ atom\|TreeSelectAddOnDecoratedBox#PART_AddOnDecoratedBox` | 访问装饰框部件 |
| `atom\|TreeSelect /template/ Border#PopupFrame` | 访问弹窗容器 Border |
| `atom\|TreeSelect /template/ ContentPresenter#SingleSelectResultPresenter` | 访问单选模式下的选中项展示器 |
| `atom\|TreeSelect /template/ atom\|SelectFilterTextBox#PART_SingleFilterInput` | 访问单选搜索输入框 |
| `atom\|TreeSelect /template/ atom\|SelectTagAwareTextBox#SelectedItemsBox` | 访问多选模式下的 Tag 容器 |
| `atom\|TreeSelect /template/ atom\|TreeSelectTreeView#PART_TreeView` | 访问内嵌 TreeView |

### 组合选择器示例

| 选择器 | 说明 |
|---|---|
| `atom\|TreeSelect[IsMultiple=False][IsFilterEnabled=False]` | 单选且未启用筛选的 TreeSelect |
| `atom\|TreeSelect[IsMultiple=False][IsFilterEnabled=True]:dropdownopen` | 单选筛选模式下弹窗打开时 |
| `atom\|TreeSelect[IsMultiple=True]:not(:disabled)` | 非禁用的多选 TreeSelect |
| `atom\|TreeSelect:error:outline` | Outline 变体 + 错误状态 |
| `atom\|TreeSelect:warning:filled` | Filled 变体 + 警告状态 |
