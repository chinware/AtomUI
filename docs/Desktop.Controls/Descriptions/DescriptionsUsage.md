# Descriptions 使用文档

本文档介绍 AtomUI Descriptions 控件的使用方式，涵盖常见场景和完整用例。

> 📖 Gallery 源码位置：`controlgallery/AtomUIGallery/ShowCases/Views/DataDisplay/DescriptionsShowCase.axaml`

---

## 前置准备

### AXAML 命名空间

```xml
xmlns:atom="https://atomui.net"
```

### C# 命名空间

```csharp
using AtomUI.Desktop.Controls;
```

---

## 基本用法

最简单的描述列表：水平布局、3 列、无边框。

```xml
<atom:Descriptions Header="User Info">
    <atom:DescriptionItem Label="UserName" Content="Zhou Maomao" />
    <atom:DescriptionItem Label="Telephone" Content="1810000000" />
    <atom:DescriptionItem Label="Live" Content="Hangzhou, Zhejiang" />
    <atom:DescriptionItem Label="Remark" Content="empty" />
    <atom:DescriptionItem Label="Address"
                          Content="No. 18, Wantang Road, Xihu District, Hangzhou, Zhejiang, China" />
</atom:Descriptions>
```

> 📖 对应 Gallery 示例："Basic"

---

## 有边框模式

设置 `IsBordered="True"` 开启表格风格展示，标签列有背景色，内容列无背景：

```xml
<atom:Descriptions IsBordered="True">
    <atom:DescriptionItem Label="Product" Content="Cloud Database" />
    <atom:DescriptionItem Label="Billing Mode" Content="Prepaid" />
    <atom:DescriptionItem Label="Automatic Renewal" Content="YES" />
    <atom:DescriptionItem Label="Order time" Content="2018-04-24 18:00:00" />
    <atom:DescriptionItem Label="Usage Time" Content="2019-04-24 18:00:00" Span="2" />
    <atom:DescriptionItem Label="Status" Content="Running" Span="3" />
    <atom:DescriptionItem Label="Negotiated Amount" Content="$80.00" />
    <atom:DescriptionItem Label="Discount" Content="$20.00" />
    <atom:DescriptionItem Label="Official Receipts" Content="$60.00" />
    <atom:DescriptionItem Label="Config Info">
        <atom:DescriptionItem.Content>
            <StackPanel Orientation="Vertical" Spacing="5">
                <TextBlock>Data disk type: MongoDB</TextBlock>
                <TextBlock>Database version: 3.4</TextBlock>
                <TextBlock>Package: dds.mongo.mid</TextBlock>
                <TextBlock>Storage space: 10 GB</TextBlock>
                <TextBlock>Replication factor: 3</TextBlock>
                <TextBlock>Region: East China 1</TextBlock>
            </StackPanel>
        </atom:DescriptionItem.Content>
    </atom:DescriptionItem>
</atom:Descriptions>
```

> 📖 对应 Gallery 示例："border"

---

## 自定义尺寸

通过 `SizeType` 属性控制子项内间距。配合 `Header` 和 `Extra` 使用：

```xml
<atom:Descriptions IsBordered="True" SizeType="Middle" Header="Custom Size">
    <atom:Descriptions.Extra>
        <atom:Button ButtonType="Primary">Edit</atom:Button>
    </atom:Descriptions.Extra>
    <atom:DescriptionItem Label="Product" Content="Cloud Database" />
    <atom:DescriptionItem Label="Billing Mode" Content="Prepaid" />
    <atom:DescriptionItem Label="Automatic Renewal" Content="YES" />
    <atom:DescriptionItem Label="Order time" Content="2018-04-24 18:00:00" />
    <atom:DescriptionItem Label="Usage Time" Content="2019-04-24 18:00:00" Span="2" />
</atom:Descriptions>
```

三种尺寸对比：

| SizeType | 效果 |
|---|---|
| `Large` (默认) | 最大内间距，宽松布局 |
| `Middle` | 中等内间距 |
| `Small` | 最小内间距，紧凑布局 |

> 📖 对应 Gallery 示例："Custom size"

---

## 响应式列数

使用 `ColumnInfo` 属性按窗口断点自动调整列数：

```xml
<atom:Descriptions IsBordered="True"
                   Header="Responsive Descriptions"
                   ColumnInfo="xs: 1, sm: 2, md: 3, lg: 3, xl: 4, xxl: 4">
    <atom:DescriptionItem Label="Product" Content="Cloud Database" />
    <atom:DescriptionItem Label="Billing" Content="Prepaid" />
    <atom:DescriptionItem Label="Time" Content="18:00:00" />
    <atom:DescriptionItem Label="Amount" Content="$80.00" />
    <atom:DescriptionItem Label="Discount" Content="$20.00" Span="xl: 2, xxl: 2" />
    <atom:DescriptionItem Label="Official" Content="$60.00" Span="xl: 2, xxl: 2" />
    <atom:DescriptionItem Label="Config Info"
                          Span="xs: 1, sm: 2, md: 3, lg: 3, xl: 2, xxl: 2">
        <atom:DescriptionItem.Content>
            <StackPanel Orientation="Vertical">
                <TextBlock>Data disk type: MongoDB</TextBlock>
                <TextBlock>Database version: 3.4</TextBlock>
                <TextBlock>Package: dds.mongo.mid</TextBlock>
            </StackPanel>
        </atom:DescriptionItem.Content>
    </atom:DescriptionItem>
    <atom:DescriptionItem Label="Hardware Info"
                          Span="xs: 1, sm: 2, md: 3, lg: 3, xl: 2, xxl: 2">
        <atom:DescriptionItem.Content>
            <StackPanel Orientation="Vertical">
                <TextBlock>CPU: 6 Core 3.5 GHz</TextBlock>
                <TextBlock>Replication factor: 3</TextBlock>
                <TextBlock>Region: East China 1</TextBlock>
            </StackPanel>
        </atom:DescriptionItem.Content>
    </atom:DescriptionItem>
</atom:Descriptions>
```

**断点说明**：

| 断点缩写 | 枚举值 | 窗口宽度范围 |
|---|---|---|
| `xs` | `ExtraSmall` | ≤576px |
| `sm` | `Small` | ≤768px |
| `md` | `Medium` | ≤992px |
| `lg` | `Large` | ≤1200px |
| `xl` | `ExtraLarge` | ≤1600px |
| `xxl` | `ExtraExtraLarge` | >1600px |

> 📖 对应 Gallery 示例："responsive"

---

## 垂直布局

设置 `Layout="Vertical"` 启用垂直布局，标签在上方、内容在下方：

```xml
<atom:Descriptions Header="User Info" Layout="Vertical">
    <atom:DescriptionItem Label="UserName" Content="Zhou Maomao" />
    <atom:DescriptionItem Label="Telephone" Content="1810000000" />
    <atom:DescriptionItem Label="Live" Content="Hangzhou, Zhejiang" />
    <atom:DescriptionItem Label="Remark" Content="empty" />
    <atom:DescriptionItem Label="Address"
                          Content="No. 18, Wantang Road, Xihu District, Hangzhou, Zhejiang, China" />
</atom:Descriptions>
```

> 📖 对应 Gallery 示例："Vertical"

---

## 垂直布局 + 有边框

垂直布局同样支持边框模式：

```xml
<atom:Descriptions IsBordered="True" Layout="Vertical">
    <atom:DescriptionItem Label="Product" Content="Cloud Database" />
    <atom:DescriptionItem Label="Billing Mode" Content="Prepaid" />
    <atom:DescriptionItem Label="Automatic Renewal" Content="YES" />
    <atom:DescriptionItem Label="Order time" Content="2018-04-24 18:00:00" />
    <atom:DescriptionItem Label="Usage Time" Content="2019-04-24 18:00:00" Span="2" />
    <atom:DescriptionItem Label="Status" Content="Running" Span="3" />
</atom:Descriptions>
```

> 📖 对应 Gallery 示例："Vertical border"

---

## 占满行（IsFilled）

通过 `IsFilled="True"` 强制某个子项占满当前行剩余列：

```xml
<atom:Descriptions Header="User Info" IsBordered="True">
    <atom:DescriptionItem Label="UserName" Content="Zhou Maomao" />
    <atom:DescriptionItem Label="Live" Content="Hangzhou, Zhejiang" IsFilled="True" />
    <atom:DescriptionItem Label="Remark" Content="empty" IsFilled="True" />
    <atom:DescriptionItem Label="Address"
                          Content="No. 18, Wantang Road, Xihu District, Hangzhou, Zhejiang, China" />
</atom:Descriptions>
```

> 📖 对应 Gallery 示例："Row"

---

## 富内容子项

`DescriptionItem.Content` 不仅支持字符串，还支持任意 Avalonia 控件：

```xml
<atom:DescriptionItem Label="Config Info">
    <atom:DescriptionItem.Content>
        <StackPanel Orientation="Vertical" Spacing="5">
            <TextBlock>Data disk type: MongoDB</TextBlock>
            <TextBlock>Database version: 3.4</TextBlock>
            <TextBlock>Package: dds.mongo.mid</TextBlock>
        </StackPanel>
    </atom:DescriptionItem.Content>
</atom:DescriptionItem>
```

---

## 数据绑定

通过 `ItemsSource` 属性从 ViewModel 动态生成子项：

```csharp
// ViewModel
public class MyViewModel : ReactiveObject
{
    public ObservableCollection<DescriptionItem> Items { get; } = new()
    {
        new DescriptionItem { Label = "Product", Content = "Cloud Database" },
        new DescriptionItem { Label = "Billing", Content = "Prepaid" },
        new DescriptionItem { Label = "Status", Content = "Running", Span = new(3) },
    };
}
```

```xml
<atom:Descriptions IsBordered="True" ItemsSource="{Binding Items}" />
```

---

## 代码中动态操作

通过 `Items` 集合在代码中动态增删子项：

```csharp
var descriptions = new Descriptions
{
    Header = "User Info",
    IsBordered = true,
    ColumnInfo = new DescriptionsMediaBreakInfo(3),
    SizeType = SizeType.Middle
};

descriptions.Items.Add(new DescriptionItem { Label = "Name", Content = "John" });
descriptions.Items.Add(new DescriptionItem { Label = "Age", Content = "30" });
descriptions.Items.Add(new DescriptionItem { Label = "Address", Content = "Beijing", IsFilled = true });
```

> 注意：当前 `Items` 集合支持 `Add` 和 `Remove` 操作，但不支持 `Move`、`Replace` 和 `Reset` 操作。
