# Tour 自定义样式指南

Tour 的视觉表现通过 `ControlTheme` + Design Token 系统控制。以下介绍几种常见的自定义方式。

---

## 1. 使用属性直接控制

最简单的方式是通过 Tour 及 TourStep 的公共属性来控制外观：

```xml
<!-- 默认风格（白色背景卡片） -->
<atom:Tour IsOpen="{Binding TourOpened, Mode=TwoWay}">
    <atom:TourStep Target="{Binding ElementName=MyButton}"
                   Title="Upload File"
                   Description="Put your files here." />
</atom:Tour>

<!-- Primary 风格（主色调背景卡片） -->
<atom:Tour IsOpen="{Binding TourOpened, Mode=TwoWay}" StyleType="Primary">
    <atom:TourStep Target="{Binding ElementName=MyButton}"
                   Title="Upload File"
                   Description="Put your files here." />
</atom:Tour>

<!-- 不同弹出位置 -->
<atom:TourStep Target="{Binding ElementName=MyButton}"
               Placement="Top"
               Title="Above target" Description="Shows on top." />
<atom:TourStep Target="{Binding ElementName=MyButton}"
               Placement="Left"
               Title="Left of target" Description="Shows on the left." />

<!-- 无遮罩（非模态），配合 Primary 风格使用 -->
<atom:Tour IsOpen="{Binding TourOpened, Mode=TwoWay}"
           IsShowMask="False" StyleType="Primary">
    ...
</atom:Tour>

<!-- 自定义遮罩颜色 -->
<atom:Tour IsOpen="{Binding TourOpened, Mode=TwoWay}"
           MaskColor="#6650FFFF">
    ...
</atom:Tour>

<!-- 自定义高亮区域参数 -->
<atom:Tour GapRadius="10" GapOffsetX="20" GapOffsetY="20">
    ...
</atom:Tour>

<!-- 隐藏箭头 -->
<atom:Tour IsShowArrow="False">
    ...
</atom:Tour>
```

> 📖 完整示例参考：`controlgallery/AtomUIGallery/ShowCases/Views/DataDisplay/TourShowCase.axaml`

---

## 2. 自定义步骤指示器

### 使用文字指示器

```xml
<atom:Tour IsOpen="{Binding TourOpened, Mode=TwoWay}">
    <atom:Tour.Indicator>
        <atom:TextTourIndicator />
    </atom:Tour.Indicator>
    <atom:TourStep Target="{Binding ElementName=Upload}"
                   Title="Upload" Description="Upload your files." />
    <atom:TourStep Target="{Binding ElementName=Save}"
                   Title="Save" Description="Save your changes." />
</atom:Tour>
```

### 自定义圆点指示器样式

```xml
<atom:Tour IsOpen="{Binding TourOpened, Mode=TwoWay}">
    <atom:Tour.Indicator>
        <atom:DefaultTourIndicator IndicatorSize="8"
                                   IndicatorColor="Gray"
                                   IndicatorActiveColor="Red" />
    </atom:Tour.Indicator>
    ...
</atom:Tour>
```

---

## 3. 自定义操作按钮

通过 `Tour.CustomActions` 集合可以在导航区域添加自定义按钮。自定义按钮需实现 `ITourAction` 接口：

```csharp
public class SkipTourActionButton : Button, ITourAction
{
    public static readonly StyledProperty<int> StepCountProperty =
        AvaloniaProperty.Register<SkipTourActionButton, int>(nameof(StepCount));

    public static readonly StyledProperty<int> ActiveIndexProperty =
        AvaloniaProperty.Register<SkipTourActionButton, int>(nameof(ActiveIndex));

    public static readonly StyledProperty<TourStyleType> StyleTypeProperty =
        Tour.StyleTypeProperty.AddOwner<SkipTourActionButton>();

    public int StepCount
    {
        get => GetValue(StepCountProperty);
        set => SetValue(StepCountProperty, value);
    }

    public int ActiveIndex
    {
        get => GetValue(ActiveIndexProperty);
        set => SetValue(ActiveIndexProperty, value);
    }

    public TourStyleType StyleType
    {
        get => GetValue(StyleTypeProperty);
        set => SetValue(StyleTypeProperty, value);
    }

    protected override Type StyleKeyOverride => typeof(Button);

    private Tour? _tour;

    static SkipTourActionButton()
    {
        SizeTypeProperty.OverrideDefaultValue<SkipTourActionButton>(SizeType.Small);
        ButtonTypeProperty.OverrideDefaultValue<SkipTourActionButton>(ButtonType.Default);
    }

    void ITourAction.NotifyAttached(Tour tour)
    {
        _tour = tour;
    }

    protected override void OnClick()
    {
        base.OnClick();
        _tour?.SetCurrentValue(Tour.IsOpenProperty, false);
    }
}
```

在 AXAML 中使用：

```xml
<atom:Tour IsOpen="{Binding TourOpened, Mode=TwoWay}">
    <atom:Tour.CustomActions>
        <local:SkipTourActionButton>Skip</local:SkipTourActionButton>
    </atom:Tour.CustomActions>
    <atom:TourStep Target="{Binding ElementName=Upload}"
                   Title="Upload" Description="Upload your files." />
    ...
</atom:Tour>
```

> 📖 参考：`controlgallery/AtomUIGallery/ShowCases/Views/DataDisplay/TourShowCase.axaml` 中 "Custom action" 示例及其 code-behind。

---

## 4. 通过 Style 覆盖样式

可以在 AXAML 中使用 `Style` 对 Tour 相关控件进行样式覆盖：

### 全局样式

```xml
<Window.Styles>
    <!-- 统一所有 Tour 使用 Primary 风格 -->
    <Style Selector="atom|Tour">
        <Setter Property="StyleType" Value="Primary" />
    </Style>

    <!-- 覆盖 TourStep 标题字体 -->
    <Style Selector="atom|TourStep /template/ ContentPresenter#Title">
        <Setter Property="FontSize" Value="16" />
    </Style>
</Window.Styles>
```

### 按步骤风格定制

```xml
<Style Selector="atom|TourStep[StyleType=Primary] /template/ ContentPresenter#Title">
    <Setter Property="Foreground" Value="White" />
    <Setter Property="FontWeight" Value="Bold" />
</Style>
```

---

## 5. Per-Step 属性覆盖

每个 `TourStep` 都可以独立覆盖 Tour 的全局设置，实现混合风格引导：

```xml
<atom:Tour IsOpen="{Binding TourOpened, Mode=TwoWay}"
           IsShowMask="True" MaskColor="#6650FFFF">
    <!-- 第一步：使用全局遮罩颜色 -->
    <atom:TourStep Target="{Binding ElementName=Upload}"
                   Title="Upload" Description="Upload your files." />
    <!-- 第二步：覆盖遮罩颜色 -->
    <atom:TourStep Target="{Binding ElementName=Save}"
                   Title="Save" Description="Save your changes."
                   MaskColor="#662800FF" />
    <!-- 第三步：关闭遮罩 -->
    <atom:TourStep Target="{Binding ElementName=More}"
                   Title="Other Actions" Description="Click to see more."
                   IsShowMask="False" />
</atom:Tour>
```

> 📖 参考：`controlgallery/AtomUIGallery/ShowCases/Views/DataDisplay/TourShowCase.axaml` 中 "Custom indicator" 示例（自定义遮罩颜色）。

---

## 样式选择器速查

> 说明：Avalonia 样式选择器中使用 `atom|Tour` 语法引用 `atom` XML 命名空间下的 `Tour` 类型，其中 `|` 是命名空间分隔符。

### Tour 选择器

| 选择器 | 说明 |
|---|---|
| `atom\|Tour` | 匹配所有 Tour 实例 |
| `atom\|Tour[StyleType=Primary]` | 匹配 Primary 风格的 Tour |
| `atom\|Tour[IsShowMask=False]` | 匹配无遮罩的 Tour |

### TourStep 选择器

| 选择器 | 说明 |
|---|---|
| `atom\|TourStep` | 匹配所有 TourStep 实例 |
| `atom\|TourStep[StyleType=Primary]` | 匹配 Primary 风格的步骤 |
| `atom\|TourStep[IsSelected=True]` | 匹配当前显示的步骤 |

### TourStepsView 选择器（内部控件）

| 选择器 | 说明 |
|---|---|
| `atom\|TourStepsView[StyleType=Primary]` | Primary 风格的步骤视图，用于定制导航按钮样式 |
| `atom\|TourStepsView[IndexPosition=First]` | 处于第一步时的步骤视图 |
| `atom\|TourStepsView[IndexPosition=Last]` | 处于最后一步时的步骤视图 |
| `atom\|TourStepsView[IndexPosition=OnePage]` | 仅有一步时的步骤视图 |

### TourIndicator 选择器

| 选择器 | 说明 |
|---|---|
| `atom\|DefaultTourIndicator` | 默认圆点指示器 |
| `atom\|DefaultTourIndicator[StyleType=Primary]` | Primary 风格下的圆点指示器（颜色切换） |
| `atom\|TextTourIndicator` | 文字指示器 |

### 模板部件选择器

| 选择器 | 说明 |
|---|---|
| `atom\|Tour /template/ atom\|ArrowDecoratedBox#PART_ArrowDecorator` | Tour 的箭头装饰容器 |
| `atom\|Tour /template/ atom\|Popup#PART_Popup` | Tour 的弹出层 |
| `atom\|TourStep /template/ atom\|DialogCaptionButton#CloseButton` | 步骤关闭按钮 |
| `atom\|TourStep /template/ ContentPresenter#Title` | 步骤标题 |
| `atom\|TourStep /template/ ContentPresenter#CoverPresenter` | 步骤封面 |
| `atom\|TourStep /template/ ContentPresenter#DescriptionPresenter` | 步骤描述 |
| `atom\|TourStepsView /template/ atom\|Button#PreviousButton` | 上一步按钮 |
| `atom\|TourStepsView /template/ atom\|Button#NextButton` | 下一步按钮 |
| `atom\|TourStepsView /template/ atom\|Button#FinishButton` | 完成按钮 |
