# Timeline 使用文档

本文档介绍 AtomUI Timeline 控件的各种使用方式，示例代码涵盖时间轴的常见交互场景。

> 📖 Gallery 源码位置：`controlgallery/AtomUIGallery/ShowCases/Views/DataDisplay/TimelineShowCase.axaml`

---

## 前置准备

在 AXAML 中使用 Timeline，需要引入以下命名空间：

```xml
xmlns:atom="https://atomui.net"
xmlns:antdicons="https://atomui.net/icons/antdesign"
```

在 C# 代码中引入：

```csharp
using AtomUI.Desktop.Controls;   // Timeline, TimelineItem
using AtomUI.Controls;            // TimeLineMode 枚举
```

---

## 1. 基本用法

最简单的时间轴，直接在 `TimelineItem` 内写文字内容：

```xml
<atom:Timeline>
    <atom:TimelineItem>
        2024-01-01 AtomUI Officially Initiated
    </atom:TimelineItem>
    <atom:TimelineItem>
        2024-08-12 After more than 7 months of development, AtomUI is officially open-source.
    </atom:TimelineItem>
    <atom:TimelineItem>
        2024-10-01 Release of the 0.0.1 Preview Version
    </atom:TimelineItem>
</atom:Timeline>
```

---

## 2. 自定义颜色

通过 `IndicatorColor` 属性设置指示器圆点颜色，用于区分不同事件状态：

```xml
<atom:Timeline>
    <atom:TimelineItem IndicatorColor="green">
        成功 — 项目创建完成
    </atom:TimelineItem>
    <atom:TimelineItem IndicatorColor="blue">
        进行中 — 功能开发
    </atom:TimelineItem>
    <atom:TimelineItem IndicatorColor="Red">
        错误 — 构建失败
    </atom:TimelineItem>
    <atom:TimelineItem IndicatorColor="gray">
        未完成 — 等待审批
    </atom:TimelineItem>
    <atom:TimelineItem IndicatorColor="#00CCFF">
        自定义色 — 特殊标记
    </atom:TimelineItem>
</atom:Timeline>
```

> 💡 `IndicatorColor` 接受 `IBrush` 类型，支持颜色名称（`green`/`red`/`blue`/`gray`）、十六进制值（`#00CCFF`）以及任意 Avalonia 画刷。

---

## 3. 自定义图标

通过 `IndicatorIcon` 属性替换默认空心圆点为 Ant Design 图标：

```xml
<atom:Timeline Mode="Alternate">
    <atom:TimelineItem Label="2024-01-01">
        项目创建
    </atom:TimelineItem>
    <atom:TimelineItem>
        功能开发中
    </atom:TimelineItem>
    <atom:TimelineItem>
        测试验收
    </atom:TimelineItem>
    <atom:TimelineItem IndicatorIcon="{antdicons:AntDesignIconProvider Kind=ClockCircleOutlined}"
                       IndicatorColor="Red"
                       Label="2024-01-01">
        紧急修复
    </atom:TimelineItem>
</atom:Timeline>
```

---

## 4. 待办节点（Pending）

通过 `Pending` 属性在时间轴末尾添加一个"进行中"的幽灵节点，默认使用旋转的 `LoadingOutlined` 图标：

```xml
<atom:Timeline Pending="Recording...">
    <atom:TimelineItem Label="2024-01-01">
        AtomUI Officially Initiated
    </atom:TimelineItem>
    <atom:TimelineItem Label="2024-08-12">
        正式开源
    </atom:TimelineItem>
    <atom:TimelineItem Label="2024-10-01">
        发布 0.0.1 预览版
    </atom:TimelineItem>
</atom:Timeline>
```

---

## 5. 反转排列（IsReverse）

通过 `IsReverse` 属性倒序展示时间节点。可结合按钮动态切换：

```xml
<StackPanel>
    <atom:Timeline Pending="Recording..."
                   IsReverse="False"
                   x:Name="ReverseTimeline">
        <atom:TimelineItem Label="2024-01-01">事件一</atom:TimelineItem>
        <atom:TimelineItem Label="2024-08-12">事件二</atom:TimelineItem>
        <atom:TimelineItem Label="2024-10-01">事件三</atom:TimelineItem>
    </atom:Timeline>
    <atom:Button ButtonType="Primary" Click="ToggleReverse">
        Toggle Reverse
    </atom:Button>
</StackPanel>
```

```csharp
private void ToggleReverse(object? sender, RoutedEventArgs e)
{
    ReverseTimeline.IsReverse = !ReverseTimeline.IsReverse;
}
```

---

## 6. 带标签的时间轴（Label）

通过 `Label` 属性为节点附加标签文本。任一节点设置 Label 后，时间轴自动切换为双栏标签布局：

```xml
<atom:Timeline Mode="Left">
    <atom:TimelineItem Label="2024-01-01">
        AtomUI Officially Initiated
    </atom:TimelineItem>
    <atom:TimelineItem Label="2015-09-01 09:12:11">
        Create a services site
    </atom:TimelineItem>
    <atom:TimelineItem>
        Qinware website online
    </atom:TimelineItem>
    <atom:TimelineItem Label="2029-09-01">
        Network problems being solved
    </atom:TimelineItem>
</atom:Timeline>
```

> 💡 标签布局下，Mode 控制标签和内容的左右位置关系。`Left` 模式下标签在左、内容在右；`Right` 模式下反之。

---

## 7. 右对齐时间轴

通过 `Mode="Right"` 将指示器放在右侧，内容在左侧显示：

```xml
<atom:Timeline Mode="Right">
    <atom:TimelineItem>事件一</atom:TimelineItem>
    <atom:TimelineItem>事件二</atom:TimelineItem>
    <atom:TimelineItem>事件三</atom:TimelineItem>
    <atom:TimelineItem>事件四</atom:TimelineItem>
</atom:Timeline>
```

---

## 8. 交替布局时间轴

通过 `Mode="Alternate"` 让内容交替出现在指示器的左右两侧：

```xml
<atom:Timeline Mode="Alternate">
    <atom:TimelineItem>左侧内容（偶数位）</atom:TimelineItem>
    <atom:TimelineItem>右侧内容（奇数位）</atom:TimelineItem>
    <atom:TimelineItem>左侧内容（偶数位）</atom:TimelineItem>
    <atom:TimelineItem>右侧内容（奇数位）</atom:TimelineItem>
</atom:Timeline>
```

---

## 9. 动态切换布局模式

通过代码动态更改 `Mode` 属性，实现布局模式的实时切换：

```xml
<StackPanel>
    <WrapPanel Margin="0,0,0,20">
        <atom:RadioButton IsChecked="True" x:Name="ModeLeft">Left</atom:RadioButton>
        <atom:RadioButton x:Name="ModeRight">Right</atom:RadioButton>
        <atom:RadioButton x:Name="ModeAlternate">Alternate</atom:RadioButton>
    </WrapPanel>
    <atom:Timeline Mode="Left" x:Name="LabelTimeline">
        <atom:TimelineItem Label="2024-01-01">创建项目</atom:TimelineItem>
        <atom:TimelineItem Label="2024-08-12">正式开源</atom:TimelineItem>
        <atom:TimelineItem Label="2024-10-01">发布预览版</atom:TimelineItem>
    </atom:Timeline>
</StackPanel>
```

```csharp
private void ModeChecked(object? sender, RoutedEventArgs e)
{
    if (sender is RadioButton rb)
    {
        if (rb == ModeLeft && ModeLeft.IsChecked == true)
            LabelTimeline.Mode = TimeLineMode.Left;
        else if (rb == ModeRight && ModeRight.IsChecked == true)
            LabelTimeline.Mode = TimeLineMode.Right;
        else if (rb == ModeAlternate && ModeAlternate.IsChecked == true)
            LabelTimeline.Mode = TimeLineMode.Alternate;
    }
}
```

---

## 10. 数据绑定

### 绑定节点列表

```xml
<atom:Timeline ItemsSource="{Binding Events}">
    <atom:Timeline.ItemTemplate>
        <DataTemplate>
            <atom:TimelineItem Label="{Binding Date}"
                               IndicatorColor="{Binding StatusColor}"
                               Content="{Binding Description}" />
        </DataTemplate>
    </atom:Timeline.ItemTemplate>
</atom:Timeline>
```

---

## 常见组合模式

### 版本更新日志

```xml
<atom:Timeline>
    <atom:TimelineItem IndicatorColor="green" Label="v1.0.0">
        正式版发布，包含 50+ 组件
    </atom:TimelineItem>
    <atom:TimelineItem IndicatorColor="blue" Label="v0.9.0">
        公测版发布，修复已知问题
    </atom:TimelineItem>
    <atom:TimelineItem IndicatorColor="gray" Label="v0.1.0">
        首个预览版发布
    </atom:TimelineItem>
</atom:Timeline>
```

### 操作审批流程

```xml
<atom:Timeline>
    <atom:TimelineItem IndicatorColor="green"
                       IndicatorIcon="{antdicons:AntDesignIconProvider Kind=CheckCircleOutlined}">
        提交申请 — 张三 2024-01-01
    </atom:TimelineItem>
    <atom:TimelineItem IndicatorColor="green"
                       IndicatorIcon="{antdicons:AntDesignIconProvider Kind=CheckCircleOutlined}">
        部门审批通过 — 李四 2024-01-02
    </atom:TimelineItem>
    <atom:TimelineItem IndicatorColor="blue"
                       IndicatorIcon="{antdicons:AntDesignIconProvider Kind=ClockCircleOutlined}">
        等待总监审批
    </atom:TimelineItem>
    <atom:TimelineItem IndicatorColor="gray">
        财务确认
    </atom:TimelineItem>
</atom:Timeline>
```

### 进度追踪（带 Pending）

```xml
<atom:Timeline Pending="部署中...">
    <atom:TimelineItem IndicatorColor="green">代码提交</atom:TimelineItem>
    <atom:TimelineItem IndicatorColor="green">单元测试通过</atom:TimelineItem>
    <atom:TimelineItem IndicatorColor="green">代码审查通过</atom:TimelineItem>
</atom:Timeline>
```
