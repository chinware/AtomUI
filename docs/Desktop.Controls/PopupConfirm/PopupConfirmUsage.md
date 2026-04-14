# PopupConfirm 使用文档

本文档介绍 AtomUI PopupConfirm 控件的各种使用方式，示例代码摘自 Gallery 演示程序。

> 📖 Gallery 源码位置：`controlgallery/AtomUIGallery/ShowCases/Views/Feedback/PopupConfirmShowCase.axaml`

---

## 前置准备

在 AXAML 中使用 PopupConfirm，需要引入以下命名空间：

```xml
xmlns:atom="https://atomui.net"
xmlns:antdicons="https://atomui.net/icons/antdesign"
```

在 C# 代码中引入：

```csharp
using AtomUI.Desktop.Controls;   // PopupConfirm 控件
```

---

## 1. 基本用法

最基本的用法是设置 `Title` 和 `ConfirmContent`，包裹一个触发按钮：

```xml
<atom:PopupConfirm
    Title="Delete the task"
    ConfirmContent="Are you sure to delete this task?"
    OkText="Ok"
    CancelText="Cancel"
    Placement="Top"
    IsShowArrow="True">
    <atom:Button ButtonType="Default" IsDanger="True">Delete</atom:Button>
</atom:PopupConfirm>
```

点击按钮后，在按钮上方弹出带有标题、描述和操作按钮的气泡确认框。点击「Ok」或「Cancel」后气泡自动关闭。

---

## 2. 本地化文本

不显式设置 `OkText` 和 `CancelText` 时，按钮文本由本地化资源自动提供（英文：Ok/Cancel，中文：确定/取消）：

```xml
<atom:PopupConfirm
    Title="Delete the task"
    ConfirmContent="Are you sure to delete this task?">
    <atom:Button ButtonType="Default" IsDanger="True">Delete</atom:Button>
</atom:PopupConfirm>
```

也可以显式设置自定义文本：

```xml
<atom:PopupConfirm
    Title="确认删除"
    ConfirmContent="该操作不可撤销"
    OkText="确认删除"
    CancelText="再想想">
    <atom:Button IsDanger="True">删除记录</atom:Button>
</atom:PopupConfirm>
```

---

## 3. 弹出方位

PopupConfirm 支持 12 种弹出方位，通过 `Placement` 属性设置。以下是完整的 12 方位布局示例：

```xml
<Grid>
    <Grid.Styles>
        <Style Selector="atom|Button">
            <Setter Property="Margin" Value="5" />
            <Setter Property="Width" Value="80" />
        </Style>
    </Grid.Styles>
    <Grid.RowDefinitions>
        <RowDefinition Height="Auto" />
        <RowDefinition Height="Auto" />
        <RowDefinition Height="Auto" />
        <RowDefinition Height="Auto" />
        <RowDefinition Height="Auto" />
    </Grid.RowDefinitions>
    <Grid.ColumnDefinitions>
        <ColumnDefinition Width="Auto" />
        <ColumnDefinition Width="Auto" />
        <ColumnDefinition Width="Auto" />
        <ColumnDefinition Width="Auto" />
        <ColumnDefinition Width="Auto" />
    </Grid.ColumnDefinitions>

    <!-- 左侧列 -->
    <atom:PopupConfirm Grid.Row="1" Grid.Column="0"
                       Placement="LeftEdgeAlignedTop"
                       Title="Delete the task"
                       ConfirmContent="Are you sure to delete this task?">
        <atom:Button ButtonType="Default">LT</atom:Button>
    </atom:PopupConfirm>
    <atom:PopupConfirm Grid.Row="2" Grid.Column="0"
                       Placement="Left"
                       Title="Delete the task"
                       ConfirmContent="Are you sure to delete this task?">
        <atom:Button ButtonType="Default">Left</atom:Button>
    </atom:PopupConfirm>
    <atom:PopupConfirm Grid.Row="3" Grid.Column="0"
                       Placement="LeftEdgeAlignedBottom"
                       Title="Delete the task"
                       ConfirmContent="Are you sure to delete this task?">
        <atom:Button ButtonType="Default">LB</atom:Button>
    </atom:PopupConfirm>

    <!-- 顶部行 -->
    <atom:PopupConfirm Grid.Row="0" Grid.Column="1"
                       Placement="TopEdgeAlignedLeft"
                       Title="Delete the task"
                       ConfirmContent="Are you sure to delete this task?">
        <atom:Button ButtonType="Default">TL</atom:Button>
    </atom:PopupConfirm>
    <atom:PopupConfirm Grid.Row="0" Grid.Column="2"
                       Placement="Top"
                       Title="Delete the task"
                       ConfirmContent="Are you sure to delete this task?">
        <atom:Button ButtonType="Default">Top</atom:Button>
    </atom:PopupConfirm>
    <atom:PopupConfirm Grid.Row="0" Grid.Column="3"
                       Placement="TopEdgeAlignedRight"
                       Title="Delete the task"
                       ConfirmContent="Are you sure to delete this task?">
        <atom:Button ButtonType="Default">TR</atom:Button>
    </atom:PopupConfirm>

    <!-- 右侧列 -->
    <atom:PopupConfirm Grid.Row="1" Grid.Column="4"
                       Placement="RightEdgeAlignedTop"
                       Title="Delete the task"
                       ConfirmContent="Are you sure to delete this task?">
        <atom:Button ButtonType="Default">RT</atom:Button>
    </atom:PopupConfirm>
    <atom:PopupConfirm Grid.Row="2" Grid.Column="4"
                       Placement="Right"
                       Title="Delete the task"
                       ConfirmContent="Are you sure to delete this task?">
        <atom:Button ButtonType="Default">Right</atom:Button>
    </atom:PopupConfirm>
    <atom:PopupConfirm Grid.Row="3" Grid.Column="4"
                       Placement="RightEdgeAlignedBottom"
                       Title="Delete the task"
                       ConfirmContent="Are you sure to delete this task?">
        <atom:Button ButtonType="Default">RB</atom:Button>
    </atom:PopupConfirm>

    <!-- 底部行 -->
    <atom:PopupConfirm Grid.Row="4" Grid.Column="1"
                       Placement="BottomEdgeAlignedLeft"
                       Title="Delete the task"
                       ConfirmContent="Are you sure to delete this task?">
        <atom:Button ButtonType="Default">BL</atom:Button>
    </atom:PopupConfirm>
    <atom:PopupConfirm Grid.Row="4" Grid.Column="2"
                       Placement="Bottom"
                       Title="Delete the task"
                       ConfirmContent="Are you sure to delete this task?">
        <atom:Button ButtonType="Default">Bottom</atom:Button>
    </atom:PopupConfirm>
    <atom:PopupConfirm Grid.Row="4" Grid.Column="3"
                       Placement="BottomEdgeAlignedRight"
                       Title="Delete the task"
                       ConfirmContent="Are you sure to delete this task?">
        <atom:Button ButtonType="Default">BR</atom:Button>
    </atom:PopupConfirm>
</Grid>
```

**12 种方位对照**：

| 方位组 | 值 | 说明 |
|---|---|---|
| 上方 | `Top` / `TopEdgeAlignedLeft` / `TopEdgeAlignedRight` | 气泡在触发元素上方 |
| 下方 | `Bottom` / `BottomEdgeAlignedLeft` / `BottomEdgeAlignedRight` | 气泡在触发元素下方 |
| 左侧 | `Left` / `LeftEdgeAlignedTop` / `LeftEdgeAlignedBottom` | 气泡在触发元素左侧 |
| 右侧 | `Right` / `RightEdgeAlignedTop` / `RightEdgeAlignedBottom` | 气泡在触发元素右侧 |

---

## 4. 自定义图标和确认状态

通过 `Icon` 属性替换默认的 `ExclamationCircleFilled` 图标，通过 `ConfirmStatus` 控制图标颜色：

```xml
<atom:PopupConfirm
    Title="Delete the task"
    ConfirmContent="Are you sure to delete this task?"
    Icon="{antdicons:AntDesignIconProvider Kind=QuestionCircleOutlined}"
    ConfirmStatus="Error"
    OkText="Ok"
    CancelText="Cancel">
    <atom:Button ButtonType="Default" IsDanger="True">Delete</atom:Button>
</atom:PopupConfirm>
```

不同 `ConfirmStatus` 的使用场景：

```xml
<!-- Info：信息性确认，蓝色图标 -->
<atom:PopupConfirm Title="提示"
                   ConfirmContent="此操作将刷新页面"
                   ConfirmStatus="Info">
    <atom:Button>刷新</atom:Button>
</atom:PopupConfirm>

<!-- Warning（默认）：一般性确认，橙色图标 -->
<atom:PopupConfirm Title="确认"
                   ConfirmContent="确定要提交吗？">
    <atom:Button ButtonType="Primary">提交</atom:Button>
</atom:PopupConfirm>

<!-- Error：高风险操作确认，红色图标 -->
<atom:PopupConfirm Title="危险操作"
                   ConfirmContent="此操作不可撤销，数据将被永久删除"
                   ConfirmStatus="Error">
    <atom:Button IsDanger="True">永久删除</atom:Button>
</atom:PopupConfirm>
```

---

## 5. 事件处理

PopupConfirm 提供三个事件用于处理用户操作：

### 分别处理确认和取消

```xml
<atom:PopupConfirm
    Title="Delete?"
    ConfirmContent="This action cannot be undone."
    Confirmed="OnConfirmed"
    Cancelled="OnCancelled">
    <atom:Button IsDanger="True">Delete</atom:Button>
</atom:PopupConfirm>
```

```csharp
private void OnConfirmed(object? sender, RoutedEventArgs e)
{
    // 用户点击了确认按钮
    Console.WriteLine("User confirmed the action");
    // 执行删除逻辑...
}

private void OnCancelled(object? sender, RoutedEventArgs e)
{
    // 用户点击了取消按钮
    Console.WriteLine("User cancelled the action");
}
```

### 使用统一的 PopupClick 事件

```xml
<atom:PopupConfirm
    Title="Delete?"
    ConfirmContent="This action cannot be undone."
    PopupClick="OnPopupClick">
    <atom:Button IsDanger="True">Delete</atom:Button>
</atom:PopupConfirm>
```

```csharp
private void OnPopupClick(object? sender, PopupConfirmClickEventArgs e)
{
    if (e.IsConfirmed)
    {
        Console.WriteLine("Confirmed!");
        // 执行确认逻辑...
    }
    else
    {
        Console.WriteLine("Cancelled!");
        // 执行取消逻辑...
    }
}
```

**事件触发顺序**：
1. 点击确认按钮：`Confirmed` → `PopupClick`（`IsConfirmed = true`）→ 自动关闭弹出层
2. 点击取消按钮：`Cancelled` → `PopupClick`（`IsConfirmed = false`）→ 自动关闭弹出层

---

## 6. 仅标题模式

当不需要详细描述时，可以仅设置 `Title`，不设置 `ConfirmContent`：

```xml
<atom:PopupConfirm Title="Are you sure?">
    <atom:Button IsDanger="True">Delete</atom:Button>
</atom:PopupConfirm>
```

此时 `:empty-content` 伪类激活，按钮区域上边距自动移除，气泡更加紧凑。

---

## 7. 隐藏取消按钮

当确认操作不需要取消选项时，可以隐藏取消按钮：

```xml
<atom:PopupConfirm
    Title="Acknowledged"
    ConfirmContent="This is a notification that requires acknowledgment."
    IsShowCancelButton="False"
    OkText="Got it">
    <atom:Button>Show Notice</atom:PopupConfirm>
</atom:PopupConfirm>
```

---

## 8. 自定义确认按钮类型

通过 `OkButtonType` 属性设置确认按钮的视觉类型：

```xml
<!-- 默认：Primary 类型（蓝色实心） -->
<atom:PopupConfirm Title="Confirm?" OkButtonType="Primary">
    <atom:Button>Default OK Style</atom:Button>
</atom:PopupConfirm>

<!-- 使用 Default 类型（白色背景 + 边框） -->
<atom:PopupConfirm Title="Confirm?" OkButtonType="Default">
    <atom:Button>Default OK Style</atom:Button>
</atom:PopupConfirm>

<!-- 使用 Dashed 类型 -->
<atom:PopupConfirm Title="Confirm?" OkButtonType="Dashed">
    <atom:Button>Dashed OK Style</atom:Button>
</atom:PopupConfirm>
```

---

## 9. 带箭头的气泡

通过 `IsShowArrow` 属性显示箭头指向触发元素，通过 `IsPointAtCenter` 控制箭头是否指向元素中心：

```xml
<!-- 显示箭头 -->
<atom:PopupConfirm
    Title="Delete?"
    IsShowArrow="True">
    <atom:Button>With Arrow</atom:Button>
</atom:PopupConfirm>

<!-- 箭头指向元素中心 -->
<atom:PopupConfirm
    Title="Delete?"
    IsShowArrow="True"
    IsPointAtCenter="True">
    <atom:Button>Point at Center</atom:Button>
</atom:PopupConfirm>
```

---

## 10. 程序化控制显示/隐藏

通过 `ShowFlyout` 和 `HideFlyout` 方法程序化控制弹出层：

```xml
<atom:PopupConfirm x:Name="MyConfirm"
                   Title="Confirm action?"
                   Trigger="Click">
    <atom:Button>Show Confirm</atom:Button>
</atom:PopupConfirm>

<atom:Button Click="ShowConfirmProgrammatically">Show via Code</atom:Button>
```

```csharp
private void ShowConfirmProgrammatically(object? sender, RoutedEventArgs e)
{
    MyConfirm.ShowFlyout(immediately: false); // 带动画显示
}
```

---

## 常见组合模式

### 危险操作确认

```xml
<atom:PopupConfirm
    Title="确认删除"
    ConfirmContent="删除后数据不可恢复，确认继续？"
    ConfirmStatus="Error"
    Confirmed="OnDeleteConfirmed">
    <atom:Button ButtonType="Primary" IsDanger="True"
                 Icon="{antdicons:AntDesignIconProvider Kind=DeleteOutlined}">
        删除
    </atom:Button>
</atom:PopupConfirm>
```

### 与列表/表格结合

```xml
<!-- 在 DataGrid 行操作列中使用 -->
<atom:PopupConfirm
    Title="Delete this record?"
    ConfirmContent="This action cannot be undone."
    ConfirmStatus="Error"
    Confirmed="OnRowDeleteConfirmed">
    <atom:Button ButtonType="Link" IsDanger="True">Delete</atom:Button>
</atom:PopupConfirm>
```

### 表单取消确认

```xml
<atom:PopupConfirm
    Title="Unsaved changes"
    ConfirmContent="You have unsaved changes. Discard them?"
    OkText="Discard"
    CancelText="Keep editing"
    ConfirmStatus="Warning"
    Confirmed="OnDiscardConfirmed">
    <atom:Button>Cancel</atom:Button>
</atom:PopupConfirm>
```
