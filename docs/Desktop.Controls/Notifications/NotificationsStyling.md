# Notification 自定义样式指南

Notification 系统的视觉表现通过 `ControlTheme` + Design Token 系统控制。以下介绍几种常见的自定义方式。

---

## 1. 使用属性直接控制

### 设置弹出位置

```csharp
var manager = new WindowNotificationManager(topLevel)
{
    Position = NotificationPosition.TopLeft,  // 左上角弹出
    MaxItems = 3                              // 最多同时显示 3 条
};
```

### 设置通知类型

```csharp
manager.Show(new Notification(
    title: "操作成功",
    content: "数据已保存",
    type: NotificationType.Success
));
```

### 设置自定义图标

```csharp
manager.Show(new Notification(
    title: "设置提醒",
    content: "请检查系统配置",
    icon: new SettingOutlined()
));
```

### 设置永不自动关闭

```csharp
manager.Show(new Notification(
    title: "重要通知",
    content: "此通知需要手动关闭",
    expiration: TimeSpan.Zero
));
```

### 显示倒计时进度条

```csharp
manager.Show(new Notification(
    title: "通知标题",
    content: "带有进度条的通知",
    showProgress: true
));
```

### 控制悬停暂停行为

```csharp
var manager = new WindowNotificationManager(topLevel)
{
    IsPauseOnHover = false  // 鼠标悬停时不暂停自动关闭
};
```

> 📖 完整示例参考：`controlgallery/AtomUIGallery/ShowCases/Views/Feedback/NotificationShowCase.axaml`

---

## 2. 通过 Style 覆盖 NotificationCard 样式

### 全局自定义通知卡片宽度

```xml
<Window.Styles>
    <Style Selector="atom|NotificationCard">
        <Setter Property="Width" Value="450" />
    </Style>
</Window.Styles>
```

### 按通知类型定制图标颜色

```xml
<!-- 自定义 Success 类型的图标颜色 -->
<Style Selector="atom|NotificationCard[NotificationType=Success] /template/ atom|IconPresenter#IconPresenter">
    <Setter Property="IconBrush" Value="#52c41a" />
</Style>
```

### 自定义关闭按钮样式

```xml
<Style Selector="atom|NotificationCard /template/ atom|IconButton#PART_CloseButton">
    <Setter Property="IconHeight" Value="14" />
    <Setter Property="IconWidth" Value="14" />
</Style>
```

### 自定义标题文本样式

```xml
<Style Selector="atom|NotificationCard /template/ atom|SelectableTextBlock#HeaderTitle">
    <Setter Property="FontWeight" Value="Bold" />
</Style>
```

---

## 3. 通过 Style 覆盖 WindowNotificationManager 样式

### 自定义容器对齐方式

```xml
<Style Selector="atom|WindowNotificationManager /template/ ReversibleStackPanel#PART_Items">
    <Setter Property="Margin" Value="20" />
</Style>
```

---

## 4. 通过样式类（Style Classes）定制

`Show()` 方法支持传入样式类名数组，可为特定通知应用自定义样式：

```csharp
// 添加自定义样式类
manager.Show(new Notification("标题", "内容"), classes: new[] { "important" });
```

```xml
<!-- 在 AXAML 中定义对应样式 -->
<Style Selector="atom|NotificationCard.important /template/ Border#Frame">
    <Setter Property="BorderBrush" Value="Red" />
    <Setter Property="BorderThickness" Value="2" />
</Style>
```

---

## 5. 控制动画行为

```csharp
// 禁用出入动画
var manager = new WindowNotificationManager(topLevel)
{
    IsMotionEnabled = false
};
```

---

## 样式选择器速查

> 说明：Avalonia 样式选择器中使用 `atom|NotificationCard` 语法引用 `atom` XML 命名空间下的 `NotificationCard` 类型，其中 `|` 是命名空间分隔符。

### 按控件类型选择

| 选择器 | 说明 |
|---|---|
| `atom\|NotificationCard` | 匹配所有通知卡片 |
| `atom\|WindowNotificationManager` | 匹配通知管理器容器 |

### 按通知类型选择

| 选择器 | 说明 |
|---|---|
| `atom\|NotificationCard[NotificationType=Information]` | 匹配信息类通知 |
| `atom\|NotificationCard[NotificationType=Success]` | 匹配成功类通知 |
| `atom\|NotificationCard[NotificationType=Warning]` | 匹配警告类通知 |
| `atom\|NotificationCard[NotificationType=Error]` | 匹配错误类通知 |

### 按位置选择

| 选择器 | 说明 |
|---|---|
| `atom\|NotificationCard[Position=TopLeft]` | 匹配左上角位置的通知卡片 |
| `atom\|NotificationCard[Position=TopRight]` | 匹配右上角位置的通知卡片 |
| `atom\|NotificationCard[Position=TopCenter]` | 匹配顶部居中位置的通知卡片 |
| `atom\|NotificationCard[Position=BottomLeft]` | 匹配左下角位置的通知卡片 |
| `atom\|NotificationCard[Position=BottomRight]` | 匹配右下角位置的通知卡片 |
| `atom\|NotificationCard[Position=BottomCenter]` | 匹配底部居中位置的通知卡片 |

### 按状态选择

| 选择器 | 说明 |
|---|---|
| `atom\|NotificationCard[IsClosed=True]` | 匹配已关闭的通知卡片（动画完成后） |
| `atom\|NotificationCard[IsShowProgress=True]` | 匹配显示进度条的通知卡片 |

### 访问模板部件

| 选择器 | 说明 |
|---|---|
| `atom\|NotificationCard /template/ Border#Frame` | 通知卡片的主框架 Border |
| `atom\|NotificationCard /template/ atom\|IconPresenter#IconPresenter` | 类型图标展示器 |
| `atom\|NotificationCard /template/ DockPanel#HeaderContainer` | 标题栏容器 |
| `atom\|NotificationCard /template/ atom\|SelectableTextBlock#HeaderTitle` | 标题文本 |
| `atom\|NotificationCard /template/ atom\|IconButton#PART_CloseButton` | 关闭按钮 |
| `atom\|NotificationCard /template/ ContentPresenter#Content` | 正文内容展示器 |
| `atom\|NotificationCard /template/ atom\|NotificationProgressBar#ProgressBar` | 进度条 |

### 组合选择器示例

| 选择器 | 说明 |
|---|---|
| `atom\|NotificationCard[NotificationType=Error] /template/ Border#Frame` | 错误类型通知的主框架（可自定义边框颜色） |
| `atom\|NotificationCard[Position=TopLeft] /template/ Border#Frame` | 左上角通知的主框架（可自定义边距） |
| `atom\|NotificationCard.important` | 带有 `important` 样式类的通知卡片 |
