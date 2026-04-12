# Descriptions 自定义样式指南

Descriptions 的视觉表现通过 `ControlTheme` + Design Token 系统控制。以下介绍几种常见的自定义方式。

---

## 1. 使用属性直接控制

最简单的方式是通过控件的公共属性来控制外观：

```xml
<!-- 基础描述列表 -->
<atom:Descriptions Header="User Info">
    <atom:DescriptionItem Label="UserName" Content="Zhou Maomao" />
    <atom:DescriptionItem Label="Telephone" Content="1810000000" />
</atom:Descriptions>

<!-- 有边框模式 -->
<atom:Descriptions IsBordered="True">
    <atom:DescriptionItem Label="Product" Content="Cloud Database" />
    <atom:DescriptionItem Label="Status" Content="Running" />
</atom:Descriptions>

<!-- 垂直布局 -->
<atom:Descriptions Layout="Vertical" Header="User Info">
    <atom:DescriptionItem Label="UserName" Content="Zhou Maomao" />
</atom:Descriptions>

<!-- 隐藏冒号 -->
<atom:Descriptions IsShowColon="False">
    <atom:DescriptionItem Label="Product" Content="Cloud Database" />
</atom:Descriptions>

<!-- 不同尺寸 -->
<atom:Descriptions SizeType="Large" IsBordered="True" />
<atom:Descriptions SizeType="Middle" IsBordered="True" />
<atom:Descriptions SizeType="Small" IsBordered="True" />

<!-- 固定列数 -->
<atom:Descriptions ColumnInfo="4" />

<!-- 响应式列数 -->
<atom:Descriptions ColumnInfo="xs: 1, sm: 2, md: 3, lg: 3, xl: 4, xxl: 4" />
```

> 📖 完整示例参考：`controlgallery/AtomUIGallery/ShowCases/Views/DataDisplay/DescriptionsShowCase.axaml`

---

## 2. 通过 Style 覆盖样式

可以在 AXAML 中使用 `Style` 对 Descriptions 进行全局或局部样式覆盖：

### 全局设置边框模式

```xml
<Window.Styles>
    <Style Selector="atom|Descriptions">
        <Setter Property="IsBordered" Value="True" />
    </Style>
</Window.Styles>
```

### 按尺寸定制样式

```xml
<!-- 小号描述列表使用更小的字号 -->
<Style Selector="atom|Descriptions[SizeType=Small]">
    <Setter Property="FontSize" Value="12" />
</Style>
```

### 按布局方向定制

```xml
<!-- 垂直布局下增加间距 -->
<Style Selector="atom|Descriptions[Layout=Vertical]">
    <Setter Property="Margin" Value="0,10" />
</Style>
```

---

## 3. 通过 ControlTheme 完全替换主题

如果需要自定义 Descriptions 的模板和样式，可以定义自定义 `ControlTheme`：

```xml
<ControlTheme x:Key="MyCustomDescriptions" TargetType="atom:Descriptions">
    <!-- 基于默认主题扩展 -->
    <Setter Property="Template">
        <ControlTemplate>
            <StackPanel Orientation="Vertical">
                <DockPanel Name="HeaderLayout" LastChildFill="True"
                           IsVisible="{TemplateBinding IsHeaderLayoutVisible}">
                    <ContentPresenter Name="ExtraPresenter" DockPanel.Dock="Right"
                                      Content="{TemplateBinding Extra}"
                                      ContentTemplate="{TemplateBinding ExtraTemplate}" />
                    <ContentPresenter Name="HeaderPresenter"
                                      Content="{TemplateBinding Header}"
                                      ContentTemplate="{TemplateBinding HeaderTemplate}" />
                </DockPanel>
                <Border Name="ContentFrame">
                    <Grid Name="{x:Static atom:DescriptionsThemeConstants.GridLayoutPart}" />
                </Border>
            </StackPanel>
        </ControlTemplate>
    </Setter>
    <!-- 自定义样式... -->
</ControlTheme>

<atom:Descriptions Theme="{StaticResource MyCustomDescriptions}" />
```

> ⚠️ 注意：自定义 ControlTheme 时必须包含 `PART_GridLayout` 模板部件（`Grid`），否则子项布局将失效。

---

## 4. 标题和额外操作区域

通过 `Header` 和 `Extra` 属性自定义标题栏：

```xml
<atom:Descriptions Header="Custom Size" IsBordered="True" SizeType="{Binding SizeType}">
    <atom:Descriptions.Extra>
        <atom:Button ButtonType="Primary">Edit</atom:Button>
    </atom:Descriptions.Extra>
    <atom:DescriptionItem Label="Product" Content="Cloud Database" />
    <atom:DescriptionItem Label="Billing Mode" Content="Prepaid" />
</atom:Descriptions>
```

> 📖 参考：`controlgallery/AtomUIGallery/ShowCases/Views/DataDisplay/DescriptionsShowCase.axaml` 中 "Custom size" 示例。

---

## 5. 跨列与占满行

通过 `Span` 和 `IsFilled` 控制子项的列宽：

```xml
<atom:Descriptions IsBordered="True">
    <atom:DescriptionItem Label="UserName" Content="Zhou Maomao" />
    <!-- 占满当前行剩余列 -->
    <atom:DescriptionItem Label="Live" Content="Hangzhou, Zhejiang" IsFilled="True" />
    <!-- 强制独占整行 -->
    <atom:DescriptionItem Label="Remark" Content="empty" IsFilled="True" />
    <atom:DescriptionItem Label="Address"
                          Content="No. 18, Wantang Road, Xihu District, Hangzhou, Zhejiang, China" />
</atom:Descriptions>
```

> 📖 参考：`controlgallery/AtomUIGallery/ShowCases/Views/DataDisplay/DescriptionsShowCase.axaml` 中 "Row" 示例。

---

## 6. 响应式布局

通过断点配置实现响应式列数和跨列：

```xml
<atom:Descriptions IsBordered="True"
                   Header="Responsive Descriptions"
                   ColumnInfo="xs: 1, sm: 2, md: 3, lg: 3, xl: 4, xxl: 4">
    <atom:DescriptionItem Label="Product" Content="Cloud Database" />
    <atom:DescriptionItem Label="Billing" Content="Prepaid" />
    <atom:DescriptionItem Label="Time" Content="18:00:00" />
    <atom:DescriptionItem Label="Amount" Content="$80.00" />
    <!-- 仅在 xl/xxl 断点跨 2 列 -->
    <atom:DescriptionItem Label="Discount" Content="$20.00" Span="xl: 2, xxl: 2" />
    <atom:DescriptionItem Label="Official" Content="$60.00" Span="xl: 2, xxl: 2" />
    <!-- 多断点跨列 -->
    <atom:DescriptionItem Label="Config Info"
                          Span="xs: 1, sm: 2, md: 3, lg: 3, xl: 2, xxl: 2">
        <atom:DescriptionItem.Content>
            <StackPanel Orientation="Vertical">
                <TextBlock>Data disk type: MongoDB</TextBlock>
                <TextBlock>Database version: 3.4</TextBlock>
            </StackPanel>
        </atom:DescriptionItem.Content>
    </atom:DescriptionItem>
</atom:Descriptions>
```

> 📖 参考：`controlgallery/AtomUIGallery/ShowCases/Views/DataDisplay/DescriptionsShowCase.axaml` 中 "responsive" 示例。

---

## 样式选择器速查

> 说明：Avalonia 样式选择器中使用 `atom|Descriptions` 语法引用 `atom` XML 命名空间下的 `Descriptions` 类型，其中 `|` 是命名空间分隔符。

### 按控件类型选择

| 选择器 | 说明 |
|---|---|
| `atom\|Descriptions` | 匹配所有描述列表 |

### 按属性选择

| 选择器 | 说明 |
|---|---|
| `atom\|Descriptions[IsBordered=True]` | 匹配有边框的描述列表 |
| `atom\|Descriptions[IsBordered=False]` | 匹配无边框的描述列表 |
| `atom\|Descriptions[Layout=Horizontal]` | 匹配水平布局 |
| `atom\|Descriptions[Layout=Vertical]` | 匹配垂直布局 |

### 按尺寸选择

| 选择器 | 说明 |
|---|---|
| `atom\|Descriptions[SizeType=Large]` | 匹配大号（默认） |
| `atom\|Descriptions[SizeType=Middle]` | 匹配中号 |
| `atom\|Descriptions[SizeType=Small]` | 匹配小号 |

### 模板内部元素选择

| 选择器 | 说明 |
|---|---|
| `atom\|Descriptions /template/ DockPanel#HeaderLayout` | 标题栏容器 |
| `atom\|Descriptions /template/ ContentPresenter#HeaderPresenter` | 标题内容展示器 |
| `atom\|Descriptions /template/ ContentPresenter#ExtraPresenter` | 额外操作区展示器 |
| `atom\|Descriptions /template/ Border#ContentFrame` | 内容框架（边框模式的容器） |
| `atom\|Descriptions /template/ Grid#PART_GridLayout` | 子项网格布局容器 |

### 组合选择器示例

| 选择器 | 说明 |
|---|---|
| `atom\|Descriptions[IsBordered=True][SizeType=Small]` | 有边框的小号描述列表 |
| `atom\|Descriptions[IsBordered=False][SizeType=Large]` | 无边框的大号描述列表 |
