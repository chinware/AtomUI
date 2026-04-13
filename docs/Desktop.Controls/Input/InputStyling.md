# Input 自定义样式指南

Input 控件族的视觉表现通过 `ControlTheme` + Design Token 系统控制。以下介绍各种常见的自定义方式。

---

## 1. 使用属性直接控制

最简单的方式是通过 Input 控件的公共属性来控制外观：

```xml
<!-- 不同样式变体 -->
<atom:LineEdit PlaceholderText="Outlined" StyleVariant="Outline" />
<atom:LineEdit PlaceholderText="Filled" StyleVariant="Filled" />
<atom:LineEdit PlaceholderText="Borderless" StyleVariant="Borderless" />
<atom:LineEdit PlaceholderText="Underlined" StyleVariant="Underlined" />

<!-- 不同尺寸 -->
<atom:LineEdit PlaceholderText="Large" SizeType="Large" />
<atom:LineEdit PlaceholderText="Middle" SizeType="Middle" />
<atom:LineEdit PlaceholderText="Small" SizeType="Small" />

<!-- 验证状态 -->
<atom:LineEdit PlaceholderText="Error" Status="Error" />
<atom:LineEdit PlaceholderText="Warning" Status="Warning" />

<!-- 带前后缀装饰 -->
<atom:LineEdit LeftAddOn="http://" RightAddOn=".com" Text="mysite" />

<!-- 带图标 -->
<atom:LineEdit PlaceholderText="Enter your username"
               InnerLeftContent="{antdicons:AntDesignIconProvider Kind=UserOutlined}" />

<!-- 密码模式 -->
<atom:LineEdit PlaceholderText="input password"
               PasswordChar="•"
               IsEnableRevealButton="True" />

<!-- 清除按钮 -->
<atom:LineEdit PlaceholderText="input with clear icon" IsAllowClear="True" />
```

> 📖 完整示例参考：`controlgallery/AtomUIGallery/ShowCases/Views/DataEntry/LineEditShowCase.axaml`

---

## 2. 通过 Style 覆盖样式

可以在 AXAML 中使用 `Style` 对 Input 控件进行全局或局部样式覆盖：

### 全局统一间距

```xml
<Window.Styles>
    <Style Selector="atom|LineEdit">
        <Setter Property="Margin" Value="0 5" />
    </Style>
</Window.Styles>
```

### 按样式变体定制

```xml
<!-- 自定义 Filled 变体的背景色 -->
<Style Selector="atom|LineEdit[StyleVariant=Filled]:not(:disabled)">
    <Setter Property="Background" Value="#f0f0f0" />
</Style>
<Style Selector="atom|LineEdit[StyleVariant=Filled]:not(:disabled):pointerover">
    <Setter Property="Background" Value="#e6e6e6" />
</Style>
```

### 使用伪类选择器

```xml
<!-- 错误状态的自定义样式 -->
<Style Selector="atom|LineEdit:error">
    <Setter Property="FontWeight" Value="Bold" />
</Style>

<!-- 警告状态的自定义样式 -->
<Style Selector="atom|LineEdit:warning">
    <Setter Property="FontWeight" Value="Bold" />
</Style>

<!-- 禁用状态 -->
<Style Selector="atom|LineEdit:disabled">
    <Setter Property="Opacity" Value="0.5" />
</Style>
```

### 按尺寸选择

```xml
<!-- 大号输入框使用粗体 -->
<Style Selector="atom|LineEdit[SizeType=Large]">
    <Setter Property="FontWeight" Value="SemiBold" />
</Style>

<!-- 小号输入框减小字号 -->
<Style Selector="atom|LineEdit[SizeType=Small]">
    <Setter Property="FontSize" Value="11" />
</Style>
```

---

## 3. 通过 ControlTheme 完全替换主题

如果需要彻底替换输入框的模板和样式，可以定义自定义 `ControlTheme`：

```xml
<ControlTheme x:Key="MyCustomLineEdit" TargetType="atom:LineEdit">
    <Setter Property="Template">
        <ControlTemplate>
            <Border Background="{TemplateBinding Background}"
                    CornerRadius="{TemplateBinding CornerRadius}"
                    Padding="{TemplateBinding Padding}"
                    BorderBrush="{TemplateBinding BorderBrush}"
                    BorderThickness="{TemplateBinding BorderThickness}">
                <TextPresenter Name="PART_TextPresenter"
                               Text="{TemplateBinding Text, Mode=TwoWay}" />
            </Border>
        </ControlTemplate>
    </Setter>
</ControlTheme>

<!-- 使用 -->
<atom:LineEdit Theme="{StaticResource MyCustomLineEdit}" PlaceholderText="Custom" />
```

> ⚠️ 注意：完全替换 ControlTheme 将丢失内置的 AddOnDecoratedBox 装饰能力、清除按钮、密码切换按钮等功能。建议优先使用 Style 覆盖。

---

## 4. 控制动画行为

```xml
<!-- 禁用过渡动画 -->
<atom:LineEdit IsMotionEnabled="False" PlaceholderText="No Animation" />
```

---

## 样式选择器速查

> 说明：Avalonia 样式选择器中使用 `atom|LineEdit` 语法引用 `atom` XML 命名空间下的 `LineEdit` 类型，其中 `|` 是命名空间分隔符。

### LineEdit 选择器

| 选择器 | 说明 |
|---|---|
| `atom\|LineEdit` | 匹配所有 LineEdit 实例 |
| `atom\|LineEdit[StyleVariant=Outline]` | 匹配 Outline 样式变体 |
| `atom\|LineEdit[StyleVariant=Filled]` | 匹配 Filled 样式变体 |
| `atom\|LineEdit[StyleVariant=Borderless]` | 匹配 Borderless 样式变体 |
| `atom\|LineEdit[StyleVariant=Underlined]` | 匹配 Underlined 样式变体 |
| `atom\|LineEdit[SizeType=Large]` | 匹配大号输入框 |
| `atom\|LineEdit[SizeType=Middle]` | 匹配中号输入框（默认） |
| `atom\|LineEdit[SizeType=Small]` | 匹配小号输入框 |
| `atom\|LineEdit[Status=Error]` | 匹配错误状态（等同于 `:error`） |
| `atom\|LineEdit[Status=Warning]` | 匹配警告状态（等同于 `:warning`） |

### 伪类选择器

| 选择器 | 说明 |
|---|---|
| `atom\|LineEdit:error` | 匹配错误验证状态 |
| `atom\|LineEdit:warning` | 匹配警告验证状态 |
| `atom\|LineEdit:outline` | 匹配 Outline 样式（由伪类驱动） |
| `atom\|LineEdit:filled` | 匹配 Filled 样式（由伪类驱动） |
| `atom\|LineEdit:borderless` | 匹配 Borderless 样式（由伪类驱动） |
| `atom\|LineEdit:pointerover` | 鼠标悬浮状态 |
| `atom\|LineEdit:focus` | 获得焦点状态 |
| `atom\|LineEdit:focus-visible` | 通过键盘获得焦点 |
| `atom\|LineEdit:disabled` | 禁用状态 |
| `atom\|LineEdit:empty` | 文本为空 |

### SearchEdit 选择器

| 选择器 | 说明 |
|---|---|
| `atom\|SearchEdit` | 匹配所有 SearchEdit 实例 |
| `atom\|SearchEdit[SearchButtonStyle=Primary]` | 匹配主色调搜索按钮风格 |
| `atom\|SearchEdit[IsOperating=True]` | 匹配加载中状态 |

### TextArea 选择器

| 选择器 | 说明 |
|---|---|
| `atom\|TextArea` | 匹配所有 TextArea 实例 |
| `atom\|TextArea[StyleVariant=Filled]` | 匹配 Filled 样式变体 |
| `atom\|TextArea[IsResizable=True]` | 匹配可拖拽调整大小的 TextArea |
| `atom\|TextArea:error` | 匹配错误验证状态 |
| `atom\|TextArea:warning` | 匹配警告验证状态 |

### 组合选择器示例

| 选择器 | 说明 |
|---|---|
| `atom\|LineEdit[StyleVariant=Outline]:not(:disabled):focus` | 聚焦状态的 Outline 输入框 |
| `atom\|LineEdit[StyleVariant=Filled]:error:pointerover` | 悬浮状态的错误 Filled 输入框 |
| `atom\|LineEdit[SizeType=Large]:not(:disabled)` | 非禁用的大号输入框 |
| `atom\|LineEdit /template/ ScrollViewer#PART_ScrollViewer` | 访问输入框模板内的 ScrollViewer |
| `atom\|TextArea /template/ Panel#ContentLayout` | 访问 TextArea 模板内的内容面板 |
