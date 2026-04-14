# Transfer 自定义样式指南

Transfer 的视觉表现通过 `ControlTheme` + Design Token 系统控制。以下介绍几种常见的自定义方式。

---

## 1. 通过属性直接控制

最简单的自定义方式是通过 Transfer 的公共属性控制外观和行为：

### 自定义面板尺寸

```xml
<!-- 自定义宽度和高度 -->
<atom:ListTransfer ListWidth="300" ListHeight="400" />

<!-- 拉伸模式填满容器 -->
<atom:ListTransfer IsStretchView="True" />
```

### 自定义面板标题

```xml
<atom:ListTransfer SourceTitle="Source" TargetTitle="Target" />
```

### 自定义穿梭按钮文本

```xml
<atom:ListTransfer ToTargetButtonText="to right"
                   ToSourceButtonText="to left" />
```

### 验证状态

```xml
<atom:ListTransfer Status="Error" />
<atom:ListTransfer Status="Warning" />
```

> 📖 完整示例参考：`controlgallery/AtomUIGallery/ShowCases/Views/DataEntry/TransferShowCase.axaml`

---

## 2. 自定义列表项渲染

通过 `ItemTemplate` 自定义列表项的展示方式：

```xml
<atom:ListTransfer Name="AdvanceTransfer">
    <atom:ListTransfer.ItemTemplate>
        <DataTemplate x:DataType="viewModels:SearchCaseItemData">
            <TextBlock TextTrimming="CharacterEllipsis">
                <Run Text="{Binding Content}" />
                <Run Text="-" />
                <Run Text="{Binding Description}" />
            </TextBlock>
        </DataTemplate>
    </atom:ListTransfer.ItemTemplate>
</atom:ListTransfer>
```

> 📖 参考：Gallery 中 "Advanced" 示例。

---

## 3. 自定义页脚

通过 `SourceViewFooter` / `TargetViewFooter` 在面板底部添加自定义内容：

```xml
<atom:ListTransfer ToTargetButtonText="to right"
                   ToSourceButtonText="to left"
                   ListWidth="300">
    <atom:ListTransfer.SourceViewFooter>
        <atom:Button SizeType="Small" Click="ReloadAdvancedTransferItems">
            Left button reload
        </atom:Button>
    </atom:ListTransfer.SourceViewFooter>
    <atom:ListTransfer.TargetViewFooter>
        <atom:Button SizeType="Small" Click="ReloadAdvancedTransferItems">
            Right button reload
        </atom:Button>
    </atom:ListTransfer.TargetViewFooter>
</atom:ListTransfer>
```

---

## 4. 搜索过滤

启用搜索过滤后面板头部下方会显示搜索框：

```xml
<atom:ListTransfer IsFilterEnabled="True"
                   FilterPlaceholderText="Search here"
                   FilterValueSelector="{Binding TransferFilterValueSelector}" />
```

其中 `FilterValueSelector` 是一个委托，用于从数据项中提取需要过滤的值：

```csharp
// 在 ViewModel 中
public DefaultFilterValueSelector TransferFilterValueSelector => item =>
{
    if (item is SearchCaseItemData data)
    {
        return data.Content;
    }
    return item;
};
```

---

## 5. 自定义树形穿梭视图

通过 `SourceView` 可以自定义树的展示行为：

```xml
<atom:TreeTransfer IsStretchView="True"
                   IsOneWay="{Binding TreeTransferIsOneWay}">
    <atom:TreeTransfer.SourceView>
        <atom:TransferTreeView IsDefaultExpandAll="True" />
    </atom:TreeTransfer.SourceView>
</atom:TreeTransfer>
```

---

## 6. 通过 Style 覆盖样式

可以在 AXAML 中使用 `Style` 对 Transfer 相关控件进行样式覆盖：

### 全局样式

```xml
<Window.Styles>
    <!-- 统一所有 ListTransfer 使用固定宽度 -->
    <Style Selector="atom|ListTransfer">
        <Setter Property="ListWidth" Value="250" />
    </Style>

    <!-- 禁用状态下的前景色 -->
    <Style Selector="atom|ListTransfer:disabled">
        <Setter Property="Foreground" Value="Gray" />
    </Style>
</Window.Styles>
```

### 覆盖面板装饰器样式

```xml
<Style Selector="atom|TransferItemDecorator /template/ Border#HeaderFrame">
    <Setter Property="Background" Value="LightBlue" />
</Style>
```

---

## 样式选择器速查

> 说明：Avalonia 样式选择器中使用 `atom|ListTransfer` 语法引用 `atom` XML 命名空间下的类型，`|` 是命名空间分隔符。

### Transfer 类型选择器

| 选择器 | 说明 |
|---|---|
| `atom\|ListTransfer` | 匹配所有列表穿梭框 |
| `atom\|TreeTransfer` | 匹配所有树形穿梭框 |
| `:is(atom\|AbstractTransfer)` | 匹配所有穿梭框（包含列表和树） |
| `:is(atom\|AbstractTransfer):disabled` | 匹配禁用状态的穿梭框 |

### 状态选择器

| 选择器 | 说明 |
|---|---|
| `atom\|TransferItemDecorator[Status=Error]` | Error 状态（红色边框） |
| `atom\|TransferItemDecorator[Status=Warning]` | Warning 状态（黄色边框） |

### 布局模式选择器

| 选择器 | 说明 |
|---|---|
| `:is(atom\|AbstractTransfer)[IsStretchView=True]` | 拉伸模式 |
| `:is(atom\|AbstractTransfer)[IsStretchView=False]` | 固定宽度模式 |
| `:is(atom\|AbstractTransfer)[IsPaginationEnabled=True]` | 分页模式（宽度自动使用 `ListWidthLG`） |

### 面板装饰器选择器

| 选择器 | 说明 |
|---|---|
| `atom\|TransferItemDecorator` | 面板装饰器 |
| `atom\|TransferItemDecorator[ViewType=Source]` | 源面板 |
| `atom\|TransferItemDecorator[ViewType=Target]` | 目标面板 |
| `atom\|TransferItemDecorator[IsOneWay=True]` | 单向模式面板 |

### 模板部件选择器

| 选择器 | 说明 |
|---|---|
| `:is(atom\|AbstractTransfer) /template/ Grid#RootLayout` | 根布局 Grid |
| `:is(atom\|AbstractTransfer) /template/ StackPanel#ActionsLayout` | 穿梭按钮区域 |
| `atom\|TransferItemDecorator /template/ Border#Frame` | 面板主框架 |
| `atom\|TransferItemDecorator /template/ Border#HeaderFrame` | 面板头部 |
| `atom\|TransferItemDecorator /template/ DockPanel#HeaderLayout` | 头部布局 |
| `atom\|TransferItemDecorator /template/ atom\|CheckBox#SelectAllCheckBox` | 全选复选框 |
| `atom\|TransferItemDecorator /template/ atom\|LineEdit#FilterInput` | 搜索输入框 |
| `atom\|TransferItemDecorator /template/ Border#FooterFrame` | 页脚区域 |
| `atom\|TransferItemDecorator /template/ ContentPresenter#ContentPresenter` | 视图内容区域 |
