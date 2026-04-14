# PopupConfirm 自定义样式指南

PopupConfirm 的视觉表现通过 `ControlTheme` + Design Token 系统控制。以下介绍几种常见的自定义方式。

---

## 1. 使用属性直接控制

最简单的方式是通过 PopupConfirm 的公共属性来控制外观和行为：

```xml
<!-- 基本用法：标题 + 描述 -->
<atom:PopupConfirm
    Title="Delete the task"
    ConfirmContent="Are you sure to delete this task?"
    OkText="Ok"
    CancelText="Cancel"
    Placement="Top"
    IsShowArrow="True">
    <atom:Button ButtonType="Default" IsDanger="True">Delete</atom:Button>
</atom:PopupConfirm>

<!-- 不同确认状态 -->
<atom:PopupConfirm Title="Information" ConfirmStatus="Info">
    <atom:Button>Info</atom:Button>
</atom:PopupConfirm>
<atom:PopupConfirm Title="Warning" ConfirmStatus="Warning">
    <atom:Button>Warning</atom:Button>
</atom:PopupConfirm>
<atom:PopupConfirm Title="Danger" ConfirmStatus="Error">
    <atom:Button IsDanger="True">Error</atom:Button>
</atom:PopupConfirm>

<!-- 自定义图标 -->
<atom:PopupConfirm
    Title="Delete the task"
    ConfirmContent="Are you sure to delete this task?"
    Icon="{antdicons:AntDesignIconProvider Kind=QuestionCircleOutlined}"
    ConfirmStatus="Error">
    <atom:Button ButtonType="Default" IsDanger="True">Delete</atom:Button>
</atom:PopupConfirm>

<!-- 自定义按钮文本 -->
<atom:PopupConfirm Title="确认删除？" OkText="是的" CancelText="不了">
    <atom:Button>删除</atom:Button>
</atom:PopupConfirm>

<!-- 隐藏取消按钮 -->
<atom:PopupConfirm Title="I know what I'm doing" IsShowCancelButton="False">
    <atom:Button>Confirm Only</atom:Button>
</atom:PopupConfirm>
```

> 📖 完整示例参考：`controlgallery/AtomUIGallery/ShowCases/Views/Feedback/PopupConfirmShowCase.axaml`

---

## 2. 通过 Style 覆盖样式

可以在 AXAML 中使用 `Style` 对 PopupConfirm 进行全局或局部样式覆盖。

### 全局统一间距

```xml
<Window.Styles>
    <Style Selector="atom|PopupConfirm">
        <Setter Property="Margin" Value="5" />
    </Style>
</Window.Styles>
```

### 统一设置弹出位置和箭头

```xml
<Style Selector="atom|PopupConfirm">
    <Setter Property="Placement" Value="Bottom" />
    <Setter Property="IsShowArrow" Value="True" />
</Style>
```

### 通过属性选择器匹配确认状态

PopupConfirm 本身的属性选择器可用于批量定制不同状态的外观：

```xml
<!-- 所有 Error 状态的 PopupConfirm 禁用动画 -->
<Style Selector="atom|PopupConfirm[ConfirmStatus=Error]">
    <Setter Property="IsMotionEnabled" Value="False" />
</Style>
```

---

## 3. 自定义 PopupConfirmContainer 样式

由于弹出内容由内部的 `PopupConfirmContainer` 承载，可以通过其选择器定制气泡卡片内部样式。

### 按确认状态定制图标颜色

`PopupConfirmContainerTheme` 已经内置了三种状态的图标颜色选择器：

```xml
<!-- 默认主题中的实现方式（仅供参考，通常无需覆盖） -->
<Style Selector="atom|PopupConfirmContainer[ConfirmStatus=Info] /template/ atom|IconPresenter#PART_IconPresenter">
    <Setter Property="IconBrush" Value="{atom:SharedTokenResource ColorPrimary}" />
</Style>
<Style Selector="atom|PopupConfirmContainer[ConfirmStatus=Warning] /template/ atom|IconPresenter#PART_IconPresenter">
    <Setter Property="IconBrush" Value="{atom:SharedTokenResource ColorWarning}" />
</Style>
<Style Selector="atom|PopupConfirmContainer[ConfirmStatus=Error] /template/ atom|IconPresenter#PART_IconPresenter">
    <Setter Property="IconBrush" Value="{atom:SharedTokenResource ColorError}" />
</Style>
```

### 定制标题样式

```xml
<Style Selector="atom|PopupConfirmContainer /template/ atom|TextBlock#PART_Title">
    <Setter Property="FontSize" Value="14" />
    <Setter Property="FontWeight" Value="Bold" />
</Style>
```

### 空内容时的特殊样式

当 `ConfirmContent` 为 `null` 时，`:empty-content` 伪类激活：

```xml
<!-- 默认行为：移除按钮区域上边距 -->
<Style Selector="atom|PopupConfirmContainer:empty-content /template/ StackPanel#PART_ButtonLayout">
    <Setter Property="Margin" Value="0" />
</Style>
```

---

## 4. 通过 ControlTheme 完全替换主题

如果需要彻底替换 PopupConfirm 的模板和样式，可以定义自定义 `ControlTheme`：

```xml
<ControlTheme x:Key="MyCustomPopupConfirm" TargetType="atom:PopupConfirm">
    <Setter Property="Template">
        <ControlTemplate>
            <ContentPresenter Content="{TemplateBinding Content}"
                              ContentTemplate="{TemplateBinding ContentTemplate}" />
        </ControlTemplate>
    </Setter>
    <!-- 自定义样式... -->
</ControlTheme>

<!-- 使用 -->
<atom:PopupConfirm Theme="{StaticResource MyCustomPopupConfirm}"
                   Title="Custom">
    <atom:Button>Custom Theme</atom:Button>
</atom:PopupConfirm>
```

> ⚠️ 注意：完全替换 ControlTheme 将丢失内置的弹出层创建逻辑和事件处理。建议优先使用 Style 覆盖。

---

## 5. 控制弹出位置

PopupConfirm 支持 12 种弹出方位，通过 `Placement` 属性设置：

```xml
<!-- 上方 -->
<atom:PopupConfirm Placement="Top" ...>
<atom:PopupConfirm Placement="TopEdgeAlignedLeft" ...>
<atom:PopupConfirm Placement="TopEdgeAlignedRight" ...>

<!-- 下方 -->
<atom:PopupConfirm Placement="Bottom" ...>
<atom:PopupConfirm Placement="BottomEdgeAlignedLeft" ...>
<atom:PopupConfirm Placement="BottomEdgeAlignedRight" ...>

<!-- 左侧 -->
<atom:PopupConfirm Placement="Left" ...>
<atom:PopupConfirm Placement="LeftEdgeAlignedTop" ...>
<atom:PopupConfirm Placement="LeftEdgeAlignedBottom" ...>

<!-- 右侧 -->
<atom:PopupConfirm Placement="Right" ...>
<atom:PopupConfirm Placement="RightEdgeAlignedTop" ...>
<atom:PopupConfirm Placement="RightEdgeAlignedBottom" ...>
```

> 📖 参考：`controlgallery/AtomUIGallery/ShowCases/Views/Feedback/PopupConfirmShowCase.axaml` 中 "Placement" 示例。

---

## 6. 控制动画行为

```xml
<!-- 禁用弹出/收起动画 -->
<atom:PopupConfirm IsMotionEnabled="False" Title="No animation">
    <atom:Button>Click</atom:Button>
</atom:PopupConfirm>
```

---

## 样式选择器速查

> 说明：Avalonia 样式选择器中使用 `atom|PopupConfirm` 语法引用 `atom` XML 命名空间下的类型，其中 `|` 是命名空间分隔符。

### 按控件类型选择

| 选择器 | 说明 |
|---|---|
| `atom\|PopupConfirm` | 匹配所有 PopupConfirm 实例 |
| `atom\|PopupConfirmContainer` | 匹配所有弹出内容容器（internal，仅限主题中使用） |

### 按确认状态选择

| 选择器 | 说明 |
|---|---|
| `atom\|PopupConfirmContainer[ConfirmStatus=Info]` | 信息状态的弹出内容（蓝色图标） |
| `atom\|PopupConfirmContainer[ConfirmStatus=Warning]` | 警告状态的弹出内容（橙色图标） |
| `atom\|PopupConfirmContainer[ConfirmStatus=Error]` | 错误状态的弹出内容（红色图标） |

### 按伪类选择

| 选择器 | 说明 |
|---|---|
| `atom\|PopupConfirmContainer:empty-content` | 匹配未设置描述内容的弹出容器，可用于调整仅标题模式下的间距 |
| `atom\|PopupConfirm:disabled` | 匹配禁用状态的 PopupConfirm |

### 模板部件访问

| 选择器 | 说明 |
|---|---|
| `atom\|PopupConfirmContainer /template/ DockPanel#PART_MainLayout` | 主布局面板 |
| `atom\|PopupConfirmContainer /template/ StackPanel#PART_ButtonLayout` | 按钮区域容器 |
| `atom\|PopupConfirmContainer /template/ atom\|Button#PART_OkButton` | 确认按钮 |
| `atom\|PopupConfirmContainer /template/ atom\|Button#PART_CancelButton` | 取消按钮 |
| `atom\|PopupConfirmContainer /template/ atom\|TextBlock#PART_Title` | 标题文本 |
| `atom\|PopupConfirmContainer /template/ ContentPresenter#PART_Content` | 内容展示器 |
| `atom\|PopupConfirmContainer /template/ atom\|IconPresenter#PART_IconPresenter` | 图标展示器 |

### 组合选择器示例

| 选择器 | 说明 |
|---|---|
| `atom\|PopupConfirmContainer[ConfirmStatus=Error] /template/ atom\|IconPresenter#PART_IconPresenter` | 错误状态下的图标展示器 |
| `atom\|PopupConfirmContainer:empty-content /template/ StackPanel#PART_ButtonLayout` | 无描述内容时的按钮区域 |
