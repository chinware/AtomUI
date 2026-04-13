# Message 自定义样式指南

Message 系统由 `MessageCard`（单条消息）和 `WindowMessageManager`（容器管理器）两部分组成，各自有独立的 `ControlTheme`。由于 Message 是通过代码创建（而非 AXAML 声明式使用），自定义样式主要通过 Style 选择器和 Token 资源覆盖实现。

---

## 1. 通过构造参数控制

最直接的方式是通过 `Message` 构造函数参数控制消息的表现：

```csharp
// 不同消息类型
_messageManager?.Show(new Message("操作成功", MessageType.Success));
_messageManager?.Show(new Message("发生错误", MessageType.Error));
_messageManager?.Show(new Message("请注意", MessageType.Warning));
_messageManager?.Show(new Message("正在加载...", MessageType.Loading));

// 自定义过期时间
_messageManager?.Show(new Message(
    content: "这条消息 10 秒后关闭",
    expiration: TimeSpan.FromSeconds(10)
));

// 永不自动关闭
_messageManager?.Show(new Message(
    content: "需要手动关闭",
    expiration: TimeSpan.Zero
));

// 自定义图标
_messageManager?.Show(new Message(
    content: "自定义图标消息",
    icon: new HeartFilled()
));
```

> 📖 完整示例参考：`controlgallery/AtomUIGallery/ShowCases/Views/Feedback/MessageShowCase.axaml.cs`

---

## 2. 通过 Style 覆盖 MessageCard 样式

可以在 AXAML 中对 `MessageCard` 进行全局样式覆盖：

### 自定义消息卡片外观

```xml
<Window.Styles>
    <!-- 统一调整消息卡片的圆角 -->
    <Style Selector="atom|MessageCard /template/ Border#PART_Frame">
        <Setter Property="CornerRadius" Value="4" />
    </Style>

    <!-- 调整消息文本样式 -->
    <Style Selector="atom|MessageCard /template/ SelectableTextBlock#PART_Message">
        <Setter Property="FontWeight" Value="SemiBold" />
    </Style>
</Window.Styles>
```

### 按消息类型定制图标颜色

```xml
<Window.Styles>
    <!-- 自定义成功消息的图标颜色 -->
    <Style Selector="atom|MessageCard[MessageType=Success] /template/ atom|IconPresenter">
        <Setter Property="IconBrush" Value="#52c41a" />
    </Style>

    <!-- 自定义错误消息的图标颜色 -->
    <Style Selector="atom|MessageCard[MessageType=Error] /template/ atom|IconPresenter">
        <Setter Property="IconBrush" Value="#ff4d4f" />
    </Style>
</Window.Styles>
```

### 通过样式类附加自定义样式

`WindowMessageManager.Show` 方法支持附加样式类名，可以为特定消息指定独立样式：

```csharp
// 显示消息时附加样式类
_messageManager?.Show(
    new Message("Important!", MessageType.Warning),
    classes: new[] { "important" }
);
```

```xml
<Window.Styles>
    <!-- 仅对带 "important" 样式类的消息生效 -->
    <Style Selector="atom|MessageCard.important /template/ Border#PART_Frame">
        <Setter Property="BorderBrush" Value="Orange" />
        <Setter Property="BorderThickness" Value="1" />
    </Style>
</Window.Styles>
```

---

## 3. 通过 WindowMessageManager 属性控制

```csharp
// 在 OnAttachedToVisualTree 中初始化
var topLevel = TopLevel.GetTopLevel(this);
_messageManager = new WindowMessageManager(topLevel)
{
    MaxItems = 10,          // 最多同时显示 10 条消息
    Position = NotificationPosition.TopCenter  // 顶部居中（默认行为）
};
```

---

## 4. 控制动画行为

```csharp
// 禁用消息入场/出场动画
_messageManager = new WindowMessageManager(topLevel)
{
    IsMotionEnabled = false
};
```

当 `IsMotionEnabled = false` 时，消息将直接显示/消失，不播放滑入/滑出动画。

---

## 5. 通过 ControlTheme 替换主题

如果需要彻底替换 MessageCard 的模板，可以定义自定义 `ControlTheme`：

```xml
<ControlTheme x:Key="MyCustomMessageCard" TargetType="atom:MessageCard">
    <Setter Property="Template">
        <ControlTemplate>
            <atom:MotionActor Name="{x:Static atom:BaseMotionActor.MotionActorPart}"
                              ClipToBounds="False">
                <Border Background="#f6ffed" CornerRadius="8" Padding="12,8"
                        BoxShadow="0 2 8 0 #0000001a">
                    <StackPanel Orientation="Horizontal" Spacing="8">
                        <atom:IconPresenter Name="PART_IconContent"
                                            Icon="{TemplateBinding Icon}"
                                            Width="16" Height="16" />
                        <TextBlock Name="PART_Message"
                                   Text="{TemplateBinding Message}"
                                   VerticalAlignment="Center" />
                    </StackPanel>
                </Border>
            </atom:MotionActor>
        </ControlTemplate>
    </Setter>
</ControlTheme>
```

> ⚠️ 注意：替换 ControlTheme 时必须保留 `MotionActor`（名称为 `PART_MotionActor`），否则入场/出场动画将失效。

---

## 样式选择器速查

> 说明：Avalonia 样式选择器中使用 `atom|MessageCard` 语法引用 `atom` XML 命名空间下的 `MessageCard` 类型，其中 `|` 是命名空间分隔符。

### 按控件类型选择

| 选择器 | 说明 |
|---|---|
| `atom\|MessageCard` | 匹配所有消息卡片实例 |
| `atom\|WindowMessageManager` | 匹配消息容器管理器 |

### 按消息类型选择

| 选择器 | 说明 |
|---|---|
| `atom\|MessageCard[MessageType=Information]` | 匹配信息类型消息 |
| `atom\|MessageCard[MessageType=Success]` | 匹配成功类型消息 |
| `atom\|MessageCard[MessageType=Warning]` | 匹配警告类型消息 |
| `atom\|MessageCard[MessageType=Error]` | 匹配错误类型消息 |
| `atom\|MessageCard[MessageType=Loading]` | 匹配加载中类型消息 |

### 按伪类选择

| 选择器 | 说明 |
|---|---|
| `atom\|MessageCard:information` | 信息类型伪类（等效于属性选择器） |
| `atom\|MessageCard:success` | 成功类型伪类 |
| `atom\|MessageCard:warning` | 警告类型伪类 |
| `atom\|MessageCard:error` | 错误类型伪类 |
| `atom\|MessageCard:loading` | 加载中类型伪类 |

### 按状态选择

| 选择器 | 说明 |
|---|---|
| `atom\|MessageCard[IsClosed=True]` | 匹配已关闭的消息（此时 Margin 被清零） |

### 模板部件选择

| 选择器 | 说明 |
|---|---|
| `atom\|MessageCard /template/ Border#PART_Frame` | 消息卡片主框架（背景、阴影、圆角） |
| `atom\|MessageCard /template/ atom\|IconPresenter#PART_IconContent` | 消息图标展示器 |
| `atom\|MessageCard /template/ SelectableTextBlock#PART_Message` | 消息文本 |
| `atom\|MessageCard /template/ DockPanel#PART_HeaderContainer` | 内容布局容器 |

### 组合选择器示例

| 选择器 | 说明 |
|---|---|
| `atom\|MessageCard[MessageType=Error] /template/ atom\|IconPresenter` | 错误消息的图标展示器 |
| `atom\|MessageCard.important /template/ Border#PART_Frame` | 带 "important" 样式类的消息主框架 |
| `atom\|WindowMessageManager:topcenter /template/ ReversibleStackPanel#PART_Items` | 顶部居中位置时的消息列表面板 |
