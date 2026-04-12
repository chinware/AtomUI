# ComboBox 自定义样式指南

ComboBox 的视觉表现通过 `ControlTheme` + Design Token 系统控制。以下介绍几种常见的自定义方式。

---

## 1. 使用属性直接控制

最简单的方式是通过 ComboBox 的公共属性来控制外观：

```xml
<!-- 不同样式变体 -->
<atom:ComboBox StyleVariant="Outline" PlaceholderText="Please select" Width="300">
    <atom:ComboBoxItem>选项一</atom:ComboBoxItem>
    <atom:ComboBoxItem>选项二</atom:ComboBoxItem>
</atom:ComboBox>
<atom:ComboBox StyleVariant="Filled" PlaceholderText="Please select" Width="300">
    <atom:ComboBoxItem>选项一</atom:ComboBoxItem>
    <atom:ComboBoxItem>选项二</atom:ComboBoxItem>
</atom:ComboBox>
<atom:ComboBox StyleVariant="Borderless" PlaceholderText="Please select" Width="300">
    <atom:ComboBoxItem>选项一</atom:ComboBoxItem>
    <atom:ComboBoxItem>选项二</atom:ComboBoxItem>
</atom:ComboBox>

<!-- 不同尺寸 -->
<atom:ComboBox SizeType="Large" PlaceholderText="Large" />
<atom:ComboBox SizeType="Middle" PlaceholderText="Default" />
<atom:ComboBox SizeType="Small" PlaceholderText="Small" />

<!-- 验证状态 -->
<atom:ComboBox Status="Error" PlaceholderText="Error" Width="300" />
<atom:ComboBox Status="Warning" PlaceholderText="Warning" Width="300" />

<!-- 前置/后置标签 -->
<atom:ComboBox LeftAddOn="http://" RightAddOn=".com" PlaceholderText="Please select" Width="300">
    <atom:ComboBoxItem>选项一</atom:ComboBoxItem>
</atom:ComboBox>

<!-- 内部前缀/后缀 -->
<atom:ComboBox
    ContentLeftAddOn="{antdicons:AntDesignIconProvider Kind=UserOutlined, FillBrush=#D7D7D7}"
    ContentRightAddOn="{antdicons:AntDesignIconProvider Kind=InfoCircleOutlined, FillBrush=#8C8C8C}"
    PlaceholderText="Please select" Width="300">
    <atom:ComboBoxItem>选项一</atom:ComboBoxItem>
</atom:ComboBox>
```

> 📖 完整示例参考：`controlgallery/AtomUIGallery/ShowCases/Views/Navigation/ComboBoxShowCase.axaml`

---

## 2. 通过 Style 覆盖样式

可以在 AXAML 中使用 `Style` 对 ComboBox 进行全局或局部样式覆盖：

### 全局统一宽度

```xml
<Window.Styles>
    <Style Selector="atom|ComboBox">
        <Setter Property="Width" Value="300" />
        <Setter Property="Margin" Value="0,5" />
    </Style>
</Window.Styles>
```

### 按样式变体定制

```xml
<!-- 填充样式的自定义背景 -->
<Style Selector="atom|ComboBox[StyleVariant=Filled]">
    <Setter Property="Opacity" Value="0.9" />
</Style>
```

### 使用伪类选择器

```xml
<!-- 错误状态下加粗文本 -->
<Style Selector="atom|ComboBox:error">
    <Setter Property="FontWeight" Value="SemiBold" />
</Style>

<!-- 警告状态的自定义样式 -->
<Style Selector="atom|ComboBox:warning">
    <Setter Property="FontWeight" Value="SemiBold" />
</Style>
```

### 通过属性选择器定制尺寸

```xml
<!-- 所有大号 ComboBox 使用粗体 -->
<Style Selector="atom|ComboBox[SizeType=Large]">
    <Setter Property="FontWeight" Value="SemiBold" />
</Style>

<!-- 所有小号 ComboBox 限制宽度 -->
<Style Selector="atom|ComboBox[SizeType=Small]">
    <Setter Property="MaxWidth" Value="200" />
</Style>
```

### 定制 ComboBoxItem 选项样式

```xml
<!-- 所有选中项使用特定背景色 -->
<Style Selector="atom|ComboBoxItem:selected">
    <Setter Property="FontWeight" Value="Bold" />
</Style>

<!-- 悬浮态选项样式 -->
<Style Selector="atom|ComboBoxItem:pointerover">
    <Setter Property="Opacity" Value="0.9" />
</Style>
```

---

## 3. 通过 ControlTheme 完全替换主题

如果需要彻底替换 ComboBox 的模板和样式，可以定义自定义 `ControlTheme`：

```xml
<ControlTheme x:Key="MyCustomComboBox" TargetType="atom:ComboBox">
    <Setter Property="Template">
        <ControlTemplate>
            <!-- 自定义模板结构 -->
        </ControlTemplate>
    </Setter>
</ControlTheme>

<!-- 使用 -->
<atom:ComboBox Theme="{StaticResource MyCustomComboBox}" />
```

> ⚠️ 注意：完全替换 ControlTheme 将丢失内置的 AddOnDecoratedBox 装饰、下拉手柄、动画效果等功能。建议优先使用 Style 覆盖。

---

## 4. 控制下拉面板行为

```xml
<!-- 设置下拉面板可见项数量（默认 10） -->
<atom:ComboBox DropDownDisplayPageSize="5" PlaceholderText="仅显示 5 项" Width="300">
    <atom:ComboBoxItem>选项一</atom:ComboBoxItem>
    <atom:ComboBoxItem>选项二</atom:ComboBoxItem>
    <atom:ComboBoxItem>选项三</atom:ComboBoxItem>
    <atom:ComboBoxItem>选项四</atom:ComboBoxItem>
    <atom:ComboBoxItem>选项五</atom:ComboBoxItem>
    <atom:ComboBoxItem>选项六</atom:ComboBoxItem>
    <atom:ComboBoxItem>选项七</atom:ComboBoxItem>
    <atom:ComboBoxItem>选项八</atom:ComboBoxItem>
</atom:ComboBox>
```

---

## 5. 控制动画行为

```xml
<!-- 禁用过渡动画（下拉面板不再有展开/收起动画） -->
<atom:ComboBox IsMotionEnabled="False" PlaceholderText="无动画" Width="300">
    <atom:ComboBoxItem>选项一</atom:ComboBoxItem>
</atom:ComboBox>
```

---

## 样式选择器速查

> 说明：Avalonia 样式选择器中使用 `atom|ComboBox` 语法引用 `atom` XML 命名空间下的 `ComboBox` 类型，其中 `|` 是命名空间分隔符。

### 按样式变体选择

| 选择器 | 说明 |
|---|---|
| `atom\|ComboBox` | 匹配所有 AtomUI ComboBox 实例，用于设置全局通用样式（如统一 Width、Margin 等） |
| `atom\|ComboBox[StyleVariant=Outline]` | 匹配描边样式（默认），白色背景 + 实线边框 |
| `atom\|ComboBox[StyleVariant=Filled]` | 匹配填充样式，灰色填充背景 |
| `atom\|ComboBox[StyleVariant=Borderless]` | 匹配无边框样式，无背景无边框 |

### 按尺寸选择

| 选择器 | 说明 |
|---|---|
| `atom\|ComboBox[SizeType=Large]` | 匹配大号 ComboBox（高度 = `ControlHeightLG`，字号 = `FontSizeLG`） |
| `atom\|ComboBox[SizeType=Middle]` | 匹配中号 ComboBox（默认尺寸） |
| `atom\|ComboBox[SizeType=Small]` | 匹配小号 ComboBox（高度 = `ControlHeightSM`，字号 = `FontSizeSM`） |

### 按伪类选择

| 选择器 | 说明 |
|---|---|
| `atom\|ComboBox:error` | 匹配错误验证状态（`Status == Error`），边框/背景变为红色系 |
| `atom\|ComboBox:warning` | 匹配警告验证状态（`Status == Warning`），边框/背景变为橙色系 |
| `atom\|ComboBox:pointerover` | 匹配鼠标悬浮状态 |
| `atom\|ComboBox:pressed` | 匹配按下状态 |
| `atom\|ComboBox:disabled` | 匹配禁用状态（`IsEnabled == false`） |
| `atom\|ComboBox:focus` | 匹配获得焦点的状态 |
| `atom\|ComboBox:focus-visible` | 匹配通过键盘（Tab 键）获得焦点的状态 |

### ComboBoxItem 选择器

| 选择器 | 说明 |
|---|---|
| `atom\|ComboBoxItem` | 匹配所有下拉选项 |
| `atom\|ComboBoxItem:pointerover` | 匹配鼠标悬浮的选项，背景变为 `ItemHoverBgColor` |
| `atom\|ComboBoxItem:selected` | 匹配被选中的选项，背景变为 `ItemSelectedBgColor`，字体加粗 |
| `atom\|ComboBoxItem:disabled` | 匹配禁用的选项，文字变为 `ColorTextDisabled` |

### 组合选择器示例

| 选择器 | 说明 |
|---|---|
| `atom\|ComboBox[StyleVariant=Filled]:error` | 填充样式 + 错误状态 |
| `atom\|ComboBox[StyleVariant=Borderless]:warning` | 无边框样式 + 警告状态 |
| `atom\|ComboBox[SizeType=Large]:not(:disabled)` | 非禁用的大号 ComboBox |
| `atom\|ComboBox /template/ Border#PopupFrame` | 访问模板内的弹出框架 Border |
| `atom\|ComboBox /template/ atom\|AddOnDecoratedBox` | 访问模板内的 AddOn 装饰容器 |
| `atom\|ComboBox /template/ ContentPresenter#SelectedContentPresenter` | 访问模板内的选中内容展示器 |
