# Tag 使用文档

本文档介绍 AtomUI Tag 控件的各种使用方式，示例代码涵盖标签的常见交互场景。

> 📖 Gallery 源码位置：`controlgallery/AtomUIGallery/ShowCases/Views/DataDisplay/TagShowCase.axaml`

---

## 前置准备

在 AXAML 中使用 Tag，需要引入以下命名空间：

```xml
xmlns:atom="https://atomui.net"
xmlns:antdicons="https://atomui.net/icons/antdesign"
```

在 C# 代码中引入：

```csharp
using AtomUI.Desktop.Controls;   // Tag 控件
using AtomUI.Controls;            // TagStatus 等共享类型
```

---

## 1. 基本用法

最简单的标签，直接在标签内写文字：

```xml
<WrapPanel>
    <atom:Tag>Tag 1</atom:Tag>
    <atom:Tag>Tag 2</atom:Tag>
    <atom:Tag>Link</atom:Tag>
</WrapPanel>
```

> 💡 `TagText` 是 `[Content]` 属性，可以直接在 `<atom:Tag>` 标签内写文字。

---

## 2. 预设颜色

通过 `TagColor` 属性使用 13 种预设颜色，每种颜色自动生成浅色背景 + 同系边框 + 深色文字：

```xml
<WrapPanel>
    <atom:Tag TagColor="magenta">magenta</atom:Tag>
    <atom:Tag TagColor="red">red</atom:Tag>
    <atom:Tag TagColor="volcano">volcano</atom:Tag>
    <atom:Tag TagColor="orange">orange</atom:Tag>
    <atom:Tag TagColor="gold">gold</atom:Tag>
    <atom:Tag TagColor="lime">lime</atom:Tag>
    <atom:Tag TagColor="green">green</atom:Tag>
    <atom:Tag TagColor="cyan">cyan</atom:Tag>
    <atom:Tag TagColor="blue">blue</atom:Tag>
    <atom:Tag TagColor="geekblue">geekblue</atom:Tag>
    <atom:Tag TagColor="purple">purple</atom:Tag>
    <atom:Tag TagColor="pink">pink</atom:Tag>
    <atom:Tag TagColor="yellow">yellow</atom:Tag>
</WrapPanel>
```

---

## 3. 状态颜色

四种语义化状态色，传达明确的业务含义：

```xml
<WrapPanel>
    <atom:Tag TagColor="success">success</atom:Tag>
    <atom:Tag TagColor="info">info</atom:Tag>
    <atom:Tag TagColor="warning">warning</atom:Tag>
    <atom:Tag TagColor="error">error</atom:Tag>
</WrapPanel>
```

**使用场景**：任务状态、审批结果、系统告警等。

---

## 4. 自定义颜色

传入任意 CSS 颜色值，背景为指定色，文字自动变为白色：

```xml
<WrapPanel>
    <atom:Tag TagColor="#f50">#f50</atom:Tag>
    <atom:Tag TagColor="#2db7f5">#2db7f5</atom:Tag>
    <atom:Tag TagColor="#87d068">#87d068</atom:Tag>
    <atom:Tag TagColor="#108ee9">#108ee9</atom:Tag>
</WrapPanel>
```

> 注意：自定义颜色模式下边框自动隐藏（`Bordered` 被设为 `false`）。

---

## 5. 可关闭标签

通过 `IsClosable="True"` 显示关闭按钮，点击后触发 `Closed` 事件：

```xml
<WrapPanel>
    <atom:Tag IsClosable="True" Closed="HandleTagClosed">Tag 1</atom:Tag>
    <atom:Tag IsClosable="True" Closed="HandleTagClosed">Tag 2</atom:Tag>
    <atom:Tag IsClosable="True" Closed="HandleTagClosed" TagColor="blue">
        Blue Tag
    </atom:Tag>
</WrapPanel>
```

```csharp
private void HandleTagClosed(object? sender, RoutedEventArgs e)
{
    if (sender is Tag tag && tag.Parent is Panel panel)
    {
        panel.Children.Remove(tag);
    }
}
```

---

## 6. 带图标标签

通过 `Icon` 属性在文字前显示图标：

```xml
<WrapPanel>
    <atom:Tag Icon="{antdicons:AntDesignIconProvider Kind=CheckCircleOutlined}"
             TagColor="success">
        success
    </atom:Tag>
    <atom:Tag Icon="{antdicons:AntDesignIconProvider Kind=SyncOutlined}"
             TagColor="blue">
        processing
    </atom:Tag>
    <atom:Tag Icon="{antdicons:AntDesignIconProvider Kind=CloseCircleOutlined}"
             TagColor="error">
        error
    </atom:Tag>
    <atom:Tag Icon="{antdicons:AntDesignIconProvider Kind=ExclamationCircleOutlined}"
             TagColor="warning">
        warning
    </atom:Tag>
</WrapPanel>
```

---

## 7. 无边框标签

通过 `Bordered="False"` 隐藏边框：

```xml
<WrapPanel>
    <atom:Tag Bordered="False" TagColor="blue">blue</atom:Tag>
    <atom:Tag Bordered="False" TagColor="green">green</atom:Tag>
    <atom:Tag Bordered="False" TagColor="orange">orange</atom:Tag>
    <atom:Tag Bordered="False" TagColor="red">red</atom:Tag>
</WrapPanel>
```

---

## 8. 自定义关闭图标

通过 `CloseIcon` 属性替换默认的关闭图标：

```xml
<atom:Tag IsClosable="True"
          CloseIcon="{antdicons:AntDesignIconProvider Kind=CloseCircleOutlined}">
    Custom Close Icon
</atom:Tag>
```

---

## 9. 数据绑定

### 绑定 TagColor

```xml
<atom:Tag TagColor="{Binding Status}"
          TagText="{Binding StatusText}" />
```

> 💡 `TagText` 虽然是 `[Content]` 属性（可以在标签体内直接写文字），但数据绑定时必须显式使用 `TagText="{Binding ...}"` 语法。

### 动态生成标签列表

```xml
<ItemsControl ItemsSource="{Binding Tags}">
    <ItemsControl.ItemTemplate>
        <DataTemplate>
            <atom:Tag TagColor="{Binding Color}"
                      TagText="{Binding Name}"
                      IsClosable="{Binding CanRemove}"
                      Closed="HandleTagClosed" />
        </DataTemplate>
    </ItemsControl.ItemTemplate>
    <ItemsControl.ItemsPanel>
        <ItemsPanelTemplate>
            <WrapPanel />
        </ItemsPanelTemplate>
    </ItemsControl.ItemsPanel>
</ItemsControl>
```

---

## 常见组合模式

### 文章标签列表

```xml
<WrapPanel>
    <atom:Tag TagColor="blue">Avalonia</atom:Tag>
    <atom:Tag TagColor="green">.NET</atom:Tag>
    <atom:Tag TagColor="purple">UI Framework</atom:Tag>
    <atom:Tag TagColor="orange">Cross-Platform</atom:Tag>
</WrapPanel>
```

### 状态标注

```xml
<StackPanel Orientation="Horizontal" Spacing="8">
    <TextBlock Text="Status:" VerticalAlignment="Center" />
    <atom:Tag Icon="{antdicons:AntDesignIconProvider Kind=CheckCircleOutlined}"
             TagColor="success">
        Deployed
    </atom:Tag>
</StackPanel>
```

### 可编辑标签组

```xml
<WrapPanel>
    <atom:Tag IsClosable="True" Closed="HandleTagClosed" TagColor="blue">
        Frontend
    </atom:Tag>
    <atom:Tag IsClosable="True" Closed="HandleTagClosed" TagColor="green">
        Backend
    </atom:Tag>
    <atom:Tag IsClosable="True" Closed="HandleTagClosed" TagColor="orange">
        DevOps
    </atom:Tag>
    <atom:Button ButtonType="Dashed" SizeType="Small"
                 Icon="{antdicons:AntDesignIconProvider Kind=PlusOutlined}">
        New Tag
    </atom:Button>
</WrapPanel>
```
