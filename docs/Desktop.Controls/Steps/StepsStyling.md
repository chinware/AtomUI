# Steps 自定义样式指南

Steps 控件的视觉表现通过 `ControlTheme` + Design Token 系统控制。由于 Steps 拥有丰富的属性组合（风格 × 方向 × 尺寸 × 状态 × 指示器类型），自定义主要通过属性配置实现。

> 📖 Gallery 源码位置：`controlgallery/AtomUIGallery/ShowCases/Views/Navigation/StepsShowCase.axaml`

---

## 1. 使用属性直接控制

### 基础步骤条

```xml
<atom:Steps CurrentStep="1">
    <atom:StepsItem Header="Finished" Description="This is a description." />
    <atom:StepsItem Header="In Progress" Description="This is a description." SubHeader="Left 00:00:08" />
    <atom:StepsItem Header="Waiting" Description="This is a description." />
</atom:Steps>
```

### 小尺寸

```xml
<atom:Steps CurrentStep="1" SizeType="Small">
    <atom:StepsItem Header="Finished" Description="This is a description." />
    <atom:StepsItem Header="In Progress" Description="This is a description." />
    <atom:StepsItem Header="Waiting" Description="This is a description." />
</atom:Steps>
```

### 垂直方向

```xml
<atom:Steps CurrentStep="1" Orientation="Vertical">
    <atom:StepsItem Header="Finished" Description="This is a description." />
    <atom:StepsItem Header="In Progress" Description="This is a description." />
    <atom:StepsItem Header="Waiting" Description="This is a description." />
</atom:Steps>
```

### 导航风格

```xml
<atom:Steps CurrentStep="0" Style="Navigation" IsItemClickable="True">
    <atom:StepsItem Header="Step 1" Status="Finish" />
    <atom:StepsItem Header="Step 2" Status="Process" />
    <atom:StepsItem Header="Step 3" Status="Wait" />
</atom:Steps>
```

### 点状指示器

```xml
<atom:Steps CurrentStep="1" ItemIndicatorType="Dot">
    <atom:StepsItem Header="Finished" Description="This is a description." />
    <atom:StepsItem Header="In Progress" Description="This is a description." />
    <atom:StepsItem Header="Waiting" Description="This is a description." />
</atom:Steps>
```

### 带进度环

```xml
<atom:Steps CurrentStep="1" ProgressValue="60" IsShowItemProgress="True">
    <atom:StepsItem Header="Finished" Description="This is a description." />
    <atom:StepsItem Header="In Progress" Description="This is a description." />
    <atom:StepsItem Header="Waiting" Description="This is a description." />
</atom:Steps>
```

### 标签垂直排列

```xml
<atom:Steps CurrentStep="1" LabelPlacement="Vertical">
    <atom:StepsItem Header="Finished" Description="This is a description." />
    <atom:StepsItem Header="In Progress" Description="This is a description." />
    <atom:StepsItem Header="Waiting" Description="This is a description." />
</atom:Steps>
```

---

## 2. 通过 Style 覆盖样式

### 自定义连接线颜色

```xml
<Style Selector="atom|StepsItem /template/ Rectangle.IndicatorLine">
    <Setter Property="Fill" Value="#E0E0E0" />
</Style>
```

### 自定义错误状态颜色

```xml
<Style Selector="atom|StepsItem[Status=Error]">
    <Setter Property="Foreground" Value="#FF4D4F" />
</Style>
```

### 可点击步骤的鼠标悬浮效果

```xml
<Style Selector="atom|StepsItem[IsClickable=True][IsSelected=False] /template/ DockPanel#RootLayout">
    <Setter Property="Background" Value="Transparent" />
    <Setter Property="Cursor" Value="Hand" />
</Style>
```

### 禁用动画

```xml
<atom:Steps IsMotionEnabled="False" CurrentStep="1">
    <!-- 步骤切换时无过渡动画 -->
</atom:Steps>
```

---

## 3. 自定义图标

通过 `StepsItem.Icon` 属性替换默认的数字指示器：

```xml
<atom:Steps CurrentStep="1">
    <atom:StepsItem Header="Login" Status="Finish"
                    Icon="{antdicons:AntDesignIconProvider Kind=UserOutlined}" />
    <atom:StepsItem Header="Verification" Status="Finish"
                    Icon="{antdicons:AntDesignIconProvider Kind=SolutionOutlined}" />
    <atom:StepsItem Header="Pay" Status="Process"
                    Icon="{antdicons:AntDesignIconProvider Kind=LoadingOutlined, Animation=Spin}" />
    <atom:StepsItem Header="Done" Status="Wait"
                    Icon="{antdicons:AntDesignIconProvider Kind=SmileOutlined}" />
</atom:Steps>
```

> 📖 参考：Gallery ShowCase 中 "With icon" 示例。

---

## 4. 步骤内容切换

通过 `Steps.CurrentContent` 和 `Steps.CurrentContentTemplate` 实现步骤关联内容的动态切换：

```xml
<StackPanel Orientation="Vertical" Spacing="20">
    <atom:Steps Name="MySteps" CurrentStep="{Binding CurrentStep}">
        <atom:StepsItem Header="First" Content="First-content" />
        <atom:StepsItem Header="Second" Content="Second-content" />
        <atom:StepsItem Header="Third" Content="Last-content" />
    </atom:Steps>

    <!-- 步骤内容展示区域 -->
    <Border Background="{atom:SharedTokenResource ColorFillAlter}"
            CornerRadius="{atom:SharedTokenResource BorderRadiusLG}"
            Height="260">
        <ContentPresenter Content="{Binding #MySteps.CurrentContent}"
                          ContentTemplate="{Binding #MySteps.CurrentContentTemplate}"
                          HorizontalAlignment="Center"
                          VerticalAlignment="Center" />
    </Border>

    <!-- 导航按钮 -->
    <StackPanel Orientation="Horizontal" Spacing="10">
        <atom:Button ButtonType="Primary" Click="HandleNextClick">Next</atom:Button>
        <atom:Button Click="HandlePreviousClick">Previous</atom:Button>
    </StackPanel>
</StackPanel>
```

> 📖 参考：Gallery ShowCase 中 "Switch Step" 示例。

---

## 样式选择器速查

### 按风格选择

| 选择器 | 说明 |
|---|---|
| `atom\|Steps[Style=Default]` | 默认风格 |
| `atom\|Steps[Style=Navigation]` | 导航风格 |
| `atom\|Steps[Style=Inline]` | 内联风格 |

### 按方向选择

| 选择器 | 说明 |
|---|---|
| `atom\|Steps:horizontal` | 水平方向（伪类） |
| `atom\|Steps:vertical` | 垂直方向（伪类） |

### 按尺寸选择

| 选择器 | 说明 |
|---|---|
| `atom\|Steps[SizeType=Small]` | 小尺寸 |
| `atom\|Steps[SizeType=Middle]` | 中尺寸（默认） |
| `atom\|Steps[SizeType=Large]` | 大尺寸 |

### 按步骤状态选择

| 选择器 | 说明 |
|---|---|
| `atom\|StepsItem[Status=Wait]` | 等待状态 |
| `atom\|StepsItem[Status=Process]` | 进行中状态 |
| `atom\|StepsItem[Status=Finish]` | 完成状态 |
| `atom\|StepsItem[Status=Error]` | 错误状态 |
| `atom\|StepsItem:finished` | 已完成步骤（伪类） |
| `atom\|StepsItem:selected` | 当前选中步骤（伪类） |

### 按交互选择

| 选择器 | 说明 |
|---|---|
| `atom\|StepsItem[IsClickable=True]` | 可点击步骤 |
| `atom\|StepsItem[IsClickable=True][IsSelected=False]` | 可点击但未选中的步骤 |

### 模板部件选择器

| 选择器 | 说明 |
|---|---|
| `atom\|Steps /template/ ItemsPresenter#PART_ItemsPresenter` | 步骤项容器 |
| `atom\|StepsItem /template/ DockPanel#RootLayout` | 步骤项根布局 |
| `atom\|StepsItem /template/ atom\|StepsItemIndicator#PART_Indicator` | 步骤指示器 |
| `atom\|StepsItem /template/ Rectangle.IndicatorLine` | 连接线 |
| `atom\|StepsItem /template/ ContentPresenter#HeaderPresenter` | 标题展示器 |
| `atom\|StepsItem /template/ ContentPresenter#SubHeaderPresenter` | 副标题展示器 |
| `atom\|StepsItem /template/ ContentPresenter#DescriptionPresenter` | 描述展示器 |

### 组合选择器示例

| 选择器 | 说明 |
|---|---|
| `atom\|Steps[Style=Default][Orientation=Horizontal][SizeType=Small]` | 水平默认风格小尺寸 |
| `atom\|Steps[Style=Navigation]:vertical` | 垂直导航风格 |
| `atom\|StepsItem[Status=Error] /template/ Rectangle.IndicatorLine` | 错误状态的连接线 |
| `atom\|StepsItem[IndicatorType=Dot][Status=Process]` | 进行中的点状指示器 |
