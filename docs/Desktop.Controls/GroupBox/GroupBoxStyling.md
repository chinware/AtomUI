# GroupBox 自定义样式指南

GroupBox 的视觉表现通过 `ControlTheme` + Design Token 系统控制。以下介绍几种常见的自定义方式。

---

## 1. 使用属性直接控制

最简单的方式是通过 GroupBox 的公共属性来控制外观：

```xml
<!-- 基本用法 -->
<atom:GroupBox HeaderTitle="基本信息">
    <StackPanel>
        <!-- 分组内容 -->
    </StackPanel>
</atom:GroupBox>

<!-- 标题位置 -->
<atom:GroupBox HeaderTitle="居中标题" HeaderTitlePosition="Center">
    <TextBlock>Content</TextBlock>
</atom:GroupBox>
<atom:GroupBox HeaderTitle="右对齐标题" HeaderTitlePosition="Right">
    <TextBlock>Content</TextBlock>
</atom:GroupBox>

<!-- 标题字体定制 -->
<atom:GroupBox HeaderTitle="粗体标题" HeaderFontWeight="Bold">
    <TextBlock>Content</TextBlock>
</atom:GroupBox>
<atom:GroupBox HeaderTitle="斜体标题" HeaderFontStyle="Italic">
    <TextBlock>Content</TextBlock>
</atom:GroupBox>
<atom:GroupBox HeaderTitle="彩色标题" HeaderTitleColor="Coral" HeaderFontWeight="Medium">
    <TextBlock>Content</TextBlock>
</atom:GroupBox>

<!-- 带图标的标题 -->
<atom:GroupBox HeaderTitle="GitHub"
               HeaderIcon="{antdicons:AntDesignIconProvider Kind=GithubOutlined}">
    <TextBlock>Content</TextBlock>
</atom:GroupBox>
```

> 📖 完整示例参考：`controlgallery/AtomUIGallery/ShowCases/Views/DataDisplay/GroupBoxShowCase.axaml`

---

## 2. 通过 Style 覆盖样式

可以在 AXAML 中使用 `Style` 对 GroupBox 进行全局或局部样式覆盖：

### 全局统一边框和圆角

```xml
<Window.Styles>
    <Style Selector="atom|GroupBox">
        <Setter Property="Margin" Value="0,0,0,10" />
        <Setter Property="CornerRadius" Value="8" />
        <Setter Property="BorderThickness" Value="2" />
    </Style>
</Window.Styles>
```

### 自定义背景色和边框色

```xml
<Style Selector="atom|GroupBox">
    <Setter Property="Background" Value="#F5F5F5" />
    <Setter Property="BorderBrush" Value="#D9D9D9" />
</Style>
```

> ⚠️ 注意：`Background` 属性同时影响边框区域的背景和标题遮挡区域的填充色。如果设置为透明，标题区域将不会遮挡边框线段，fieldset 效果会失效。

### 按标题位置定制样式

```xml
<!-- 居中标题使用较大字号 -->
<Style Selector="atom|GroupBox[HeaderTitlePosition=Center]">
    <Setter Property="HeaderFontSize" Value="16" />
    <Setter Property="HeaderFontWeight" Value="SemiBold" />
</Style>
```

### 自定义标题区域模板部件

通过模板选择器可以深入定制标题区域的内部元素：

```xml
<!-- 自定义标题容器的外间距 -->
<Style Selector="atom|GroupBox /template/ Panel#PART_HeaderContainer">
    <Setter Property="Margin" Value="16,8,16,0" />
</Style>

<!-- 自定义标题图标的颜色和大小 -->
<Style Selector="atom|GroupBox /template/ atom|IconPresenter#PART_HeaderIconPresenter">
    <Setter Property="IconBrush" Value="#1677FF" />
    <Setter Property="Width" Value="20" />
    <Setter Property="Height" Value="20" />
</Style>

<!-- 自定义标题文本的样式 -->
<Style Selector="atom|GroupBox /template/ atom|TextBlock#PART_HeaderPresenter">
    <Setter Property="FontFamily" Value="Consolas" />
</Style>
```

---

## 3. 通过 ControlTheme 完全替换主题

如果需要彻底替换 GroupBox 的模板和样式，可以定义自定义 `ControlTheme`：

```xml
<ControlTheme x:Key="MyCustomGroupBox" TargetType="atom:GroupBox">
    <Setter Property="Template">
        <ControlTemplate>
            <Border Background="{TemplateBinding Background}"
                    BorderBrush="{TemplateBinding BorderBrush}"
                    BorderThickness="{TemplateBinding BorderThickness}"
                    CornerRadius="{TemplateBinding CornerRadius}"
                    Padding="{TemplateBinding Padding}">
                <DockPanel>
                    <TextBlock DockPanel.Dock="Top"
                               Text="{TemplateBinding HeaderTitle}"
                               FontWeight="Bold"
                               Margin="0,0,0,8" />
                    <ContentPresenter Content="{TemplateBinding Content}" />
                </DockPanel>
            </Border>
        </ControlTemplate>
    </Setter>
</ControlTheme>

<!-- 使用 -->
<atom:GroupBox Theme="{StaticResource MyCustomGroupBox}"
               HeaderTitle="自定义主题">
    <TextBlock>Content</TextBlock>
</atom:GroupBox>
```

> ⚠️ 注意：完全替换 ControlTheme 将丢失内置的 fieldset 式边框渲染效果（标题遮挡边框），因为该效果依赖 `GroupBox.Render()` 的自定义绘制逻辑和特定的模板部件命名。建议优先使用 Style 覆盖。

---

## 4. 自定义边框和背景的注意事项

GroupBox 的边框和背景通过 `Render()` 方法自定义绘制，而不是通过模板中的 `Border` 控件属性设置。因此：

- `Background`、`BorderBrush`、`BorderThickness`、`CornerRadius` 属性虽然在模板的 `Border#PART_Frame` 上通过 `TemplateBinding` 传递了 `CornerRadius`，但实际的边框和背景绘制由 `Render()` 接管
- 修改这些属性会正确生效，因为 `Render()` 读取的是控件上的这些属性值
- 如果将 `Background` 设为 `null` 或透明色，标题遮挡区域会使用透明填充，导致边框线穿过标题区域

---

## 样式选择器速查

> 说明：Avalonia 样式选择器中使用 `atom|GroupBox` 语法引用 `atom` XML 命名空间下的 `GroupBox` 类型，其中 `|` 是命名空间分隔符。

### 按控件类型选择

| 选择器 | 说明 |
|---|---|
| `atom\|GroupBox` | 匹配所有 AtomUI GroupBox 实例 |

### 按属性选择

| 选择器 | 说明 |
|---|---|
| `atom\|GroupBox[HeaderTitlePosition=Left]` | 匹配标题左对齐的 GroupBox |
| `atom\|GroupBox[HeaderTitlePosition=Center]` | 匹配标题居中的 GroupBox |
| `atom\|GroupBox[HeaderTitlePosition=Right]` | 匹配标题右对齐的 GroupBox |

### 按伪类选择

| 选择器 | 说明 |
|---|---|
| `atom\|GroupBox:pointerover` | 匹配鼠标悬浮状态 |
| `atom\|GroupBox:disabled` | 匹配禁用状态（`IsEnabled == false`） |

### 模板部件选择器

| 选择器 | 说明 |
|---|---|
| `atom\|GroupBox /template/ Border#PART_Frame` | 主框架 Border |
| `atom\|GroupBox /template/ Panel#PART_HeaderContainer` | 标题区域容器 |
| `atom\|GroupBox /template/ Decorator#PART_HeaderContent` | 标题内容定位器 |
| `atom\|GroupBox /template/ atom\|IconPresenter#PART_HeaderIconPresenter` | 标题图标展示器 |
| `atom\|GroupBox /template/ atom\|TextBlock#PART_HeaderPresenter` | 标题文本 |
| `atom\|GroupBox /template/ ContentPresenter#PART_ContentPresenter` | 内容展示器 |

### 组合选择器示例

| 选择器 | 说明 |
|---|---|
| `atom\|GroupBox[HeaderTitlePosition=Center] /template/ Decorator#PART_HeaderContent` | 居中标题的内容定位器 |
| `atom\|GroupBox:disabled /template/ atom\|TextBlock#PART_HeaderPresenter` | 禁用状态下的标题文本 |
