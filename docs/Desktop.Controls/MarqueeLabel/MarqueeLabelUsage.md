# MarqueeLabel 使用文档

本文档介绍 AtomUI MarqueeLabel 控件的各种使用方式。

> 📖 MarqueeLabel 目前主要通过 Alert 控件间接使用。Gallery 间接示例位置：`controlgallery/AtomUIGallery/ShowCases/Views/Feedback/AlertShowCase.axaml`

---

## 前置准备

在 AXAML 中使用 MarqueeLabel，需要引入以下命名空间：

```xml
xmlns:atom="https://atomui.net"
```

在 C# 代码中引入：

```csharp
using AtomUI.Desktop.Controls;   // MarqueeLabel 控件
```

---

## 1. 基本用法

将 MarqueeLabel 放在有宽度约束的容器中，设置 `Text` 属性即可。当文本宽度超出容器宽度时，控件自动启动水平滚动动画：

```xml
<Border Width="300" BorderBrush="#d9d9d9" BorderThickness="1" Padding="8">
    <atom:MarqueeLabel Text="这是一段很长的文本内容，当它超出容器宽度时会自动从右向左循环滚动，鼠标悬浮时暂停" />
</Border>
```

**行为说明：**
- 文本宽度 ≤ 300px 时，静态显示，不滚动
- 文本宽度 > 300px 时，自动启动向左无缝循环滚动
- 鼠标悬浮时动画暂停，移出后从暂停位置恢复

---

## 2. 自定义滚动速度

通过 `MoveSpeed` 属性控制滚动速度（像素/秒），默认值为 `150`：

```xml
<!-- 慢速滚动（适合需要仔细阅读的场景） -->
<Border Width="300" Padding="8">
    <atom:MarqueeLabel Text="慢速滚动：重要公告内容，请仔细阅读..." MoveSpeed="60" />
</Border>

<!-- 默认速度 -->
<Border Width="300" Padding="8">
    <atom:MarqueeLabel Text="默认速度：标准滚动效果展示" MoveSpeed="150" />
</Border>

<!-- 快速滚动（适合短消息循环播报） -->
<Border Width="300" Padding="8">
    <atom:MarqueeLabel Text="快速滚动：紧急通知！紧急通知！" MoveSpeed="300" />
</Border>
```

---

## 3. 自定义循环间隔

通过 `CycleSpace` 属性控制两份循环文本之间的间距（像素），默认值为 `200`：

```xml
<!-- 紧凑间隔 -->
<Border Width="300" Padding="8">
    <atom:MarqueeLabel Text="紧凑间隔：文本紧密衔接循环" CycleSpace="50" />
</Border>

<!-- 默认间隔 -->
<Border Width="300" Padding="8">
    <atom:MarqueeLabel Text="默认间隔：适中的循环分隔" CycleSpace="200" />
</Border>

<!-- 宽松间隔 -->
<Border Width="300" Padding="8">
    <atom:MarqueeLabel Text="宽松间隔：大空白区域分隔" CycleSpace="500" />
</Border>
```

---

## 4. 自定义文本样式

MarqueeLabel 继承自 `TextBlock`，支持所有文本样式属性：

```xml
<StackPanel Width="400" Spacing="10">
    <!-- 自定义字体大小和颜色 -->
    <atom:MarqueeLabel Text="大号蓝色粗体滚动文本"
                       FontSize="18"
                       FontWeight="Bold"
                       Foreground="DarkBlue" />

    <!-- 自定义字体族 -->
    <atom:MarqueeLabel Text="Monospace scrolling text for code display"
                       FontFamily="Consolas"
                       FontSize="13"
                       Foreground="#52c41a" />

    <!-- 斜体文本 -->
    <atom:MarqueeLabel Text="斜体滚动文本用于引用或强调"
                       FontStyle="Italic"
                       Foreground="#8c8c8c" />
</StackPanel>
```

---

## 5. 文本未溢出时的行为

当文本宽度未超出容器宽度时，MarqueeLabel 的行为与普通 TextBlock 完全一致——静态展示文本，不启动任何动画：

```xml
<StackPanel Width="400" Spacing="10">
    <!-- 短文本——不会滚动，静态显示 -->
    <atom:MarqueeLabel Text="短文本" />

    <!-- 长文本——自动滚动 -->
    <atom:MarqueeLabel Text="这是一段非常长的文本内容，远远超出了 400 像素的容器宽度，因此会自动启动水平循环滚动动画" />
</StackPanel>
```

---

## 6. 在 Alert 中使用（主要使用场景）

MarqueeLabel 当前最主要的使用场景是通过 Alert 控件的 `IsMessageMarqueEnabled` 属性间接使用。当 Alert 消息文本较长时，启用跑马灯可以在不增加 Alert 高度的情况下展示完整消息：

```xml
<!-- 基本用法：Alert 中启用跑马灯 -->
<atom:Alert Type="Warning" IsShowIcon="True" IsMessageMarqueEnabled="True">
    I can be a React component, multiple React components, or just some text,
    Info Description Info Description Info Description Info Description
</atom:Alert>

<!-- 不同类型的 Alert 均可启用 -->
<atom:Alert Type="Info" IsShowIcon="True" IsMessageMarqueEnabled="True">
    系统维护公告：本系统将于今晚 22:00-次日 02:00 进行例行维护升级，届时服务可能暂时不可用
</atom:Alert>

<atom:Alert Type="Error" IsShowIcon="True" IsMessageMarqueEnabled="True">
    错误信息：连接超时，请检查网络连接后重试。如果问题持续存在，请联系系统管理员获取帮助
</atom:Alert>
```

> 📖 参考：`controlgallery/AtomUIGallery/ShowCases/Views/Feedback/AlertShowCase.axaml` 中 "Loop Banner" 示例。

---

## 7. C# 代码中动态控制

在 code-behind 中动态创建和控制 MarqueeLabel：

```csharp
using AtomUI.Desktop.Controls;

// 创建 MarqueeLabel
var marquee = new MarqueeLabel
{
    Text = "动态创建的滚动文本",
    MoveSpeed = 120,
    CycleSpace = 150
};

// 添加到容器
myContainer.Children.Add(marquee);

// 运行时动态修改文本
marquee.Text = "更新后的滚动文本内容";

// 运行时调整速度（控件会自动重新配置动画）
marquee.MoveSpeed = 200;

// 运行时调整间隔
marquee.CycleSpace = 300;
```

---

## 8. 数据绑定

MarqueeLabel 的属性支持 MVVM 数据绑定：

```xml
<!-- 绑定文本内容 -->
<atom:MarqueeLabel Text="{Binding NotificationMessage}" />

<!-- 绑定滚动参数 -->
<atom:MarqueeLabel Text="{Binding ScrollText}"
                   MoveSpeed="{Binding ScrollSpeed}"
                   CycleSpace="{Binding ScrollGap}" />
```

```csharp
// ViewModel
public class NotificationViewModel : ReactiveObject
{
    [Reactive]
    public string NotificationMessage { get; set; } = "默认通知";

    [Reactive]
    public double ScrollSpeed { get; set; } = 150;

    [Reactive]
    public double ScrollGap { get; set; } = 200;
}
```

---

## 常见组合模式

### 公告栏

```xml
<Border Background="#fffbe6" BorderBrush="#ffe58f" BorderThickness="1"
        CornerRadius="4" Padding="12,8">
    <DockPanel>
        <TextBlock DockPanel.Dock="Left" Text="📢 " VerticalAlignment="Center" />
        <atom:MarqueeLabel Text="重要公告：系统将于下周一进行版本升级，届时部分功能可能暂时不可用，请提前做好准备。"
                           Foreground="#ad6800"
                           VerticalAlignment="Center" />
    </DockPanel>
</Border>
```

### 通知条

```xml
<Border Background="#e6f4ff" BorderBrush="#91caff" BorderThickness="1"
        CornerRadius="4" Padding="12,8">
    <atom:MarqueeLabel Text="欢迎使用 AtomUI！这是一条循环滚动的通知信息，鼠标悬浮可暂停阅读。"
                       Foreground="#0958d9"
                       MoveSpeed="100" />
</Border>
```

### 股票行情（快速循环）

```xml
<Border Background="#1a1a1a" Padding="8,4">
    <atom:MarqueeLabel Text="AAPL ▲ 189.25 (+1.2%)   GOOGL ▲ 140.50 (+0.8%)   MSFT ▼ 378.10 (-0.3%)   AMZN ▲ 185.60 (+2.1%)"
                       Foreground="#52c41a"
                       FontFamily="Consolas"
                       MoveSpeed="200"
                       CycleSpace="300" />
</Border>
```

---

## 注意事项

1. **宽度约束**：MarqueeLabel 必须在有限宽度的容器中使用才能触发滚动。如果父容器没有宽度限制，文本永远不会溢出，动画不会启动。

2. **性能**：MarqueeLabel 使用 Avalonia `Animation` API 驱动渲染，仅在文本溢出时才创建动画对象。控件从可视树移除时会自动清理动画资源。

3. **TextWrapping**：跑马灯场景下不应设置 `TextWrapping="Wrap"`，因为换行会使文本在可用宽度内全部显示，从而不触发滚动。

4. **悬浮暂停**：鼠标悬浮暂停是自动行为，无需额外配置。恢复时动画从暂停位置无缝继续，不会重置到起始位置。
