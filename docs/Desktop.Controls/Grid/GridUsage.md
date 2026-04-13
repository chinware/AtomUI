# Grid 使用文档

本文档介绍 AtomUI Grid 栅格系统（Row / Col）的各种使用方式，示例代码摘自 Gallery 演示程序。

> 📖 Gallery 源码位置：`controlgallery/AtomUIGallery/ShowCases/Views/Layout/GridShowCase.axaml`

---

## 前置准备

在 AXAML 中使用 Grid，需要引入以下命名空间：

```xml
xmlns:atom="https://atomui.net"
```

在 C# 代码中引入：

```csharp
using AtomUI.Desktop.Controls;  // Row, Col, ColInfo, GridGutter 等
using AtomUI.Controls;           // MediaBreakPoint 等共享类型
```

---

## 1. 基础栅格

最基本的 24 栅格布局。每个 `Col` 的 `Span` 属性指定占据的栅格数，同一 `Row` 内所有 `Col` 的 Span 之和建议不超过 24。

```xml
<StackPanel Spacing="12">
    <!-- 1 列占满 -->
    <atom:Row Gutter="16,16">
        <atom:Col Span="24">
            <Border Height="48" Background="#1677FF" />
        </atom:Col>
    </atom:Row>
    
    <!-- 2 等分列 -->
    <atom:Row Gutter="16,16">
        <atom:Col Span="12">
            <Border Height="48" Background="#1677FF" />
        </atom:Col>
        <atom:Col Span="12">
            <Border Height="48" Background="#4096FF" />
        </atom:Col>
    </atom:Row>
    
    <!-- 3 等分列 -->
    <atom:Row Gutter="16,16">
        <atom:Col Span="8">
            <Border Height="48" Background="#1677FF" />
        </atom:Col>
        <atom:Col Span="8">
            <Border Height="48" Background="#4096FF" />
        </atom:Col>
        <atom:Col Span="8">
            <Border Height="48" Background="#1677FF" />
        </atom:Col>
    </atom:Row>
    
    <!-- 4 等分列 -->
    <atom:Row Gutter="16,16">
        <atom:Col Span="6">
            <Border Height="48" Background="#1677FF" />
        </atom:Col>
        <atom:Col Span="6">
            <Border Height="48" Background="#4096FF" />
        </atom:Col>
        <atom:Col Span="6">
            <Border Height="48" Background="#1677FF" />
        </atom:Col>
        <atom:Col Span="6">
            <Border Height="48" Background="#4096FF" />
        </atom:Col>
    </atom:Row>
</StackPanel>
```

**关键要点**：
- `Span="24"` 表示占满整行
- `Span="12"` 表示占半行
- 常见分割比例：24（全宽）、12+12（二等分）、8+8+8（三等分）、6+6+6+6（四等分）

---

## 2. 间距（Gutter）

`Gutter` 属性控制列之间的间距，支持多种写法。

### 水平间距

```xml
<atom:Row Gutter="16">
    <atom:Col Span="6">
        <Border Height="48" Background="#1677FF" />
    </atom:Col>
    <atom:Col Span="6">
        <Border Height="48" Background="#4096FF" />
    </atom:Col>
    <atom:Col Span="6">
        <Border Height="48" Background="#1677FF" />
    </atom:Col>
    <atom:Col Span="6">
        <Border Height="48" Background="#4096FF" />
    </atom:Col>
</atom:Row>
```

### 水平 + 垂直间距

```xml
<atom:Row Gutter="16,24" Wrap="True">
    <atom:Col Span="6">
        <Border Height="48" Background="#1677FF" />
    </atom:Col>
    <atom:Col Span="6">
        <Border Height="48" Background="#4096FF" />
    </atom:Col>
    <atom:Col Span="6">
        <Border Height="48" Background="#1677FF" />
    </atom:Col>
    <atom:Col Span="6">
        <Border Height="48" Background="#4096FF" />
    </atom:Col>
    <!-- 换行后的列 -->
    <atom:Col Span="6">
        <Border Height="48" Background="#1677FF" />
    </atom:Col>
    <atom:Col Span="6">
        <Border Height="48" Background="#4096FF" />
    </atom:Col>
    <atom:Col Span="6">
        <Border Height="48" Background="#1677FF" />
    </atom:Col>
    <atom:Col Span="6">
        <Border Height="48" Background="#4096FF" />
    </atom:Col>
</atom:Row>
```

### 响应式间距

```xml
<!-- 不同断点使用不同间距 -->
<atom:Row Gutter="xs:8,sm:16,md:24,lg:32">
    <atom:Col Span="6">
        <Border Height="48" Background="#1677FF" />
    </atom:Col>
    <atom:Col Span="6">
        <Border Height="48" Background="#4096FF" />
    </atom:Col>
</atom:Row>
```

**间距格式汇总**：

| 写法 | 含义 |
|---|---|
| `Gutter="16"` | 仅水平间距 16px |
| `Gutter="16,24"` | 水平 16px，垂直 24px |
| `Gutter="xs:8,sm:16,md:24"` | 响应式水平间距 |
| `Gutter="xs:8,sm:16;xs:16,sm:24"` | 响应式水平 + 垂直间距 |

---

## 3. 偏移（Offset）

使用 `Offset` 属性将列向右偏移指定的栅格数。偏移会占据空间，影响后续列的位置。

```xml
<StackPanel Spacing="12">
    <atom:Row Gutter="16,16">
        <atom:Col Span="8">
            <Border Height="48" Background="#1677FF">
                <TextBlock Text="col-8" HorizontalAlignment="Center" VerticalAlignment="Center" Foreground="White" />
            </Border>
        </atom:Col>
        <atom:Col Span="8" Offset="8">
            <Border Height="48" Background="#4096FF">
                <TextBlock Text="col-8 col-offset-8" HorizontalAlignment="Center" VerticalAlignment="Center" Foreground="White" />
            </Border>
        </atom:Col>
    </atom:Row>
    
    <atom:Row Gutter="16,16">
        <atom:Col Span="6" Offset="6">
            <Border Height="48" Background="#1677FF">
                <TextBlock Text="col-6 col-offset-6" HorizontalAlignment="Center" VerticalAlignment="Center" Foreground="White" />
            </Border>
        </atom:Col>
    </atom:Row>
    
    <atom:Row Gutter="16,16">
        <atom:Col Span="12" Offset="6">
            <Border Height="48" Background="#4096FF">
                <TextBlock Text="col-12 col-offset-6" HorizontalAlignment="Center" VerticalAlignment="Center" Foreground="White" />
            </Border>
        </atom:Col>
    </atom:Row>
</StackPanel>
```

---

## 4. 推拉（Push & Pull）

使用 `Push` 和 `Pull` 改变列的视觉顺序，但不影响布局流。常用于交换两列的视觉位置。

```xml
<atom:Row Gutter="16,16">
    <atom:Col Span="18" Push="6">
        <Border Height="48" Background="#4096FF">
            <TextBlock Text="col-18 col-push-6" HorizontalAlignment="Center" VerticalAlignment="Center" Foreground="White" />
        </Border>
    </atom:Col>
    <atom:Col Span="6" Pull="18">
        <Border Height="48" Background="#1677FF">
            <TextBlock Text="col-6 col-pull-18" HorizontalAlignment="Center" VerticalAlignment="Center" Foreground="White" />
        </Border>
    </atom:Col>
</atom:Row>
```

**说明**：上例中，第一列占 18 格但 Push 6 格右移，第二列占 6 格但 Pull 18 格左移。视觉上 6 格的列显示在前，18 格的列显示在后。

---

## 5. 主轴对齐（Justify）

通过 `Justify` 属性控制列在主轴（水平方向）的对齐方式，支持 6 种模式。

```xml
<StackPanel Spacing="12">
    <!-- 左对齐（默认） -->
    <Border Background="#F5F5F5" Padding="8">
        <atom:Row Gutter="16,16" Justify="Start">
            <atom:Col Span="4"><Border Height="48" Background="#1677FF" /></atom:Col>
            <atom:Col Span="4"><Border Height="48" Background="#4096FF" /></atom:Col>
            <atom:Col Span="4"><Border Height="48" Background="#1677FF" /></atom:Col>
            <atom:Col Span="4"><Border Height="48" Background="#4096FF" /></atom:Col>
        </atom:Row>
    </Border>

    <!-- 居中对齐 -->
    <Border Background="#F5F5F5" Padding="8">
        <atom:Row Gutter="16,16" Justify="Center">
            <atom:Col Span="4"><Border Height="48" Background="#1677FF" /></atom:Col>
            <atom:Col Span="4"><Border Height="48" Background="#4096FF" /></atom:Col>
            <atom:Col Span="4"><Border Height="48" Background="#1677FF" /></atom:Col>
            <atom:Col Span="4"><Border Height="48" Background="#4096FF" /></atom:Col>
        </atom:Row>
    </Border>

    <!-- 右对齐 -->
    <Border Background="#F5F5F5" Padding="8">
        <atom:Row Gutter="16,16" Justify="End">
            <atom:Col Span="4"><Border Height="48" Background="#1677FF" /></atom:Col>
            <atom:Col Span="4"><Border Height="48" Background="#4096FF" /></atom:Col>
            <atom:Col Span="4"><Border Height="48" Background="#1677FF" /></atom:Col>
            <atom:Col Span="4"><Border Height="48" Background="#4096FF" /></atom:Col>
        </atom:Row>
    </Border>

    <!-- 两端对齐 -->
    <Border Background="#F5F5F5" Padding="8">
        <atom:Row Gutter="16,16" Justify="SpaceBetween">
            <atom:Col Span="4"><Border Height="48" Background="#1677FF" /></atom:Col>
            <atom:Col Span="4"><Border Height="48" Background="#4096FF" /></atom:Col>
            <atom:Col Span="4"><Border Height="48" Background="#1677FF" /></atom:Col>
            <atom:Col Span="4"><Border Height="48" Background="#4096FF" /></atom:Col>
        </atom:Row>
    </Border>

    <!-- 每项两侧等间距 -->
    <Border Background="#F5F5F5" Padding="8">
        <atom:Row Gutter="16,16" Justify="SpaceAround">
            <atom:Col Span="4"><Border Height="48" Background="#1677FF" /></atom:Col>
            <atom:Col Span="4"><Border Height="48" Background="#4096FF" /></atom:Col>
            <atom:Col Span="4"><Border Height="48" Background="#1677FF" /></atom:Col>
            <atom:Col Span="4"><Border Height="48" Background="#4096FF" /></atom:Col>
        </atom:Row>
    </Border>

    <!-- 所有间距完全相等 -->
    <Border Background="#F5F5F5" Padding="8">
        <atom:Row Gutter="16,16" Justify="SpaceEvenly">
            <atom:Col Span="4"><Border Height="48" Background="#1677FF" /></atom:Col>
            <atom:Col Span="4"><Border Height="48" Background="#4096FF" /></atom:Col>
            <atom:Col Span="4"><Border Height="48" Background="#1677FF" /></atom:Col>
            <atom:Col Span="4"><Border Height="48" Background="#4096FF" /></atom:Col>
        </atom:Row>
    </Border>
</StackPanel>
```

---

## 6. 交叉轴对齐（Align）

通过 `Align` 属性控制列在交叉轴（垂直方向）的对齐方式。当各列高度不同时效果最明显。

```xml
<StackPanel Spacing="12">
    <!-- 顶部对齐 -->
    <Border Background="#F5F5F5" Padding="8">
        <atom:Row Gutter="16,16" Justify="Center" Align="Top">
            <atom:Col Span="4"><Border Height="72" Background="#1677FF" /></atom:Col>
            <atom:Col Span="4"><Border Height="48" Background="#4096FF" /></atom:Col>
            <atom:Col Span="4"><Border Height="64" Background="#1677FF" /></atom:Col>
            <atom:Col Span="4"><Border Height="56" Background="#4096FF" /></atom:Col>
        </atom:Row>
    </Border>

    <!-- 垂直居中 -->
    <Border Background="#F5F5F5" Padding="8">
        <atom:Row Gutter="16,16" Justify="Center" Align="Middle">
            <atom:Col Span="4"><Border Height="72" Background="#1677FF" /></atom:Col>
            <atom:Col Span="4"><Border Height="48" Background="#4096FF" /></atom:Col>
            <atom:Col Span="4"><Border Height="64" Background="#1677FF" /></atom:Col>
            <atom:Col Span="4"><Border Height="56" Background="#4096FF" /></atom:Col>
        </atom:Row>
    </Border>

    <!-- 底部对齐 -->
    <Border Background="#F5F5F5" Padding="8">
        <atom:Row Gutter="16,16" Justify="Center" Align="Bottom">
            <atom:Col Span="4"><Border Height="72" Background="#1677FF" /></atom:Col>
            <atom:Col Span="4"><Border Height="48" Background="#4096FF" /></atom:Col>
            <atom:Col Span="4"><Border Height="64" Background="#1677FF" /></atom:Col>
            <atom:Col Span="4"><Border Height="56" Background="#4096FF" /></atom:Col>
        </atom:Row>
    </Border>
</StackPanel>
```

**提示**：默认 `Align="Stretch"` 会将所有列拉伸到行的最大高度。

---

## 7. 排列顺序（Order）

通过 `Order` 属性改变列的排列顺序，值越小越靠前。

```xml
<atom:Row Gutter="16,16">
    <atom:Col Span="6" Order="4">
        <Border Height="48" Background="#1677FF">
            <TextBlock Text="1 col-order-4" HorizontalAlignment="Center" VerticalAlignment="Center" Foreground="White" />
        </Border>
    </atom:Col>
    <atom:Col Span="6" Order="3">
        <Border Height="48" Background="#4096FF">
            <TextBlock Text="2 col-order-3" HorizontalAlignment="Center" VerticalAlignment="Center" Foreground="White" />
        </Border>
    </atom:Col>
    <atom:Col Span="6" Order="2">
        <Border Height="48" Background="#1677FF">
            <TextBlock Text="3 col-order-2" HorizontalAlignment="Center" VerticalAlignment="Center" Foreground="White" />
        </Border>
    </atom:Col>
    <atom:Col Span="6" Order="1">
        <Border Height="48" Background="#4096FF">
            <TextBlock Text="4 col-order-1" HorizontalAlignment="Center" VerticalAlignment="Center" Foreground="White" />
        </Border>
    </atom:Col>
</atom:Row>
```

### 响应式排列顺序

配合 `ColInfo` MarkupExtension 实现不同断点下的排列顺序：

```xml
<atom:Row Gutter="16,16">
    <atom:Col Span="6" Order="1" Sm="{atom:ColInfo Order=2}">
        <Border Height="48" Background="#1677FF">
            <TextBlock Text="col-order-responsive" HorizontalAlignment="Center" VerticalAlignment="Center" Foreground="White" />
        </Border>
    </atom:Col>
    <atom:Col Span="6" Order="2" Sm="{atom:ColInfo Order=1}">
        <Border Height="48" Background="#4096FF">
            <TextBlock Text="col-order-responsive" HorizontalAlignment="Center" VerticalAlignment="Center" Foreground="White" />
        </Border>
    </atom:Col>
    <atom:Col Span="6" Order="3" Sm="{atom:ColInfo Order=4}">
        <Border Height="48" Background="#1677FF">
            <TextBlock Text="col-order-responsive" HorizontalAlignment="Center" VerticalAlignment="Center" Foreground="White" />
        </Border>
    </atom:Col>
    <atom:Col Span="6" Order="4" Sm="{atom:ColInfo Order=3}">
        <Border Height="48" Background="#4096FF">
            <TextBlock Text="col-order-responsive" HorizontalAlignment="Center" VerticalAlignment="Center" Foreground="White" />
        </Border>
    </atom:Col>
</atom:Row>
```

---

## 8. 响应式布局

通过 `Xs` / `Sm` / `Md` / `Lg` / `Xl` / `Xxl` 属性为不同断点配置不同的列宽。

```xml
<atom:Row Gutter="16,16">
    <atom:Col Xs="24" Sm="12" Md="16" Lg="6" Xl="6">
        <Border Height="48" Background="#1677FF">
            <TextBlock Text="col-6" HorizontalAlignment="Center" VerticalAlignment="Center" Foreground="White" />
        </Border>
    </atom:Col>
    <atom:Col Xs="24" Sm="12" Md="16" Lg="6" Xl="6">
        <Border Height="48" Background="#4096FF">
            <TextBlock Text="col-6" HorizontalAlignment="Center" VerticalAlignment="Center" Foreground="White" />
        </Border>
    </atom:Col>
    <atom:Col Xs="24" Sm="12" Md="16" Lg="6" Xl="6">
        <Border Height="48" Background="#1677FF">
            <TextBlock Text="col-6" HorizontalAlignment="Center" VerticalAlignment="Center" Foreground="White" />
        </Border>
    </atom:Col>
    <atom:Col Xs="24" Sm="12" Md="16" Lg="6" Xl="6">
        <Border Height="48" Background="#4096FF">
            <TextBlock Text="col-6" HorizontalAlignment="Center" VerticalAlignment="Center" Foreground="White" />
        </Border>
    </atom:Col>
</atom:Row>
```

**断点效果**：
- `xs`（< 576px）：每列占满整行（24 格），垂直堆叠
- `sm`（≥ 576px）：每行 2 列（12 格）
- `md`（≥ 768px）：每列 16 格（与换行配合）
- `lg`（≥ 992px）、`xl`（≥ 1200px）：每行 4 列（6 格）

---

## 9. ColInfo 高级响应式配置

`ColInfo` 可以通过三种方式使用，提供比纯数字更灵活的响应式配置。

### 方式一：内联 MarkupExtension

```xml
<atom:Col Span="6" Order="1" Sm="{atom:ColInfo Order=2}">
    <!-- 在 sm 断点下 Order 变为 2 -->
</atom:Col>
```

### 方式二：子元素语法

```xml
<atom:Col Span="24">
    <atom:Col.Md>
        <atom:ColInfo Span="12" Order="2" />
    </atom:Col.Md>
    <atom:Col.Lg>
        <atom:ColInfo Span="6" Order="1" />
    </atom:Col.Lg>
    <Border Height="48" Background="#1677FF" />
</atom:Col>
```

### 方式三：共享资源

```xml
<!-- 在 Resources 中定义 -->
<StackPanel.Resources>
    <atom:ColInfo x:Key="MdCol1" Span="12" Order="2" />
    <atom:ColInfo x:Key="MdCol2" Span="12" Order="1" />
    <atom:ColInfo x:Key="LgCol1" Span="6" Order="1" />
    <atom:ColInfo x:Key="LgCol2" Span="6" Order="2" />
</StackPanel.Resources>

<!-- 引用共享资源 -->
<atom:Row Gutter="16,16">
    <atom:Col Span="24" Md="{StaticResource MdCol1}" Lg="{StaticResource LgCol1}">
        <Border Height="48" Background="#1677FF">
            <TextBlock Text="res-col-1" HorizontalAlignment="Center" VerticalAlignment="Center" Foreground="White" />
        </Border>
    </atom:Col>
    <atom:Col Span="24" Md="{StaticResource MdCol2}" Lg="{StaticResource LgCol2}">
        <Border Height="48" Background="#4096FF">
            <TextBlock Text="res-col-2" HorizontalAlignment="Center" VerticalAlignment="Center" Foreground="White" />
        </Border>
    </atom:Col>
</atom:Row>
```

**提示**：使用共享资源可以避免重复定义相同的响应式配置，适合多处使用相同布局参数的场景。

---

## 常见布局模式

### 经典两栏布局（侧边栏 + 内容区）

```xml
<atom:Row Gutter="16,16">
    <atom:Col Span="6">
        <!-- 侧边栏 -->
        <Border Background="#F0F0F0" Padding="16">
            <TextBlock Text="Sidebar" />
        </Border>
    </atom:Col>
    <atom:Col Span="18">
        <!-- 内容区 -->
        <Border Background="#FAFAFA" Padding="16">
            <TextBlock Text="Content" />
        </Border>
    </atom:Col>
</atom:Row>
```

### 响应式三栏布局

```xml
<atom:Row Gutter="16,16">
    <atom:Col Xs="24" Md="8">
        <Border Background="#1677FF" Height="100" />
    </atom:Col>
    <atom:Col Xs="24" Md="8">
        <Border Background="#4096FF" Height="100" />
    </atom:Col>
    <atom:Col Xs="24" Md="8">
        <Border Background="#1677FF" Height="100" />
    </atom:Col>
</atom:Row>
```

### 表单布局（标签 + 输入框）

```xml
<atom:Row Gutter="16,16">
    <atom:Col Span="4">
        <TextBlock Text="Username:" HorizontalAlignment="Right" VerticalAlignment="Center" />
    </atom:Col>
    <atom:Col Span="8">
        <atom:LineEdit PlaceholderText="Enter username" />
    </atom:Col>
</atom:Row>
<atom:Row Gutter="16,16">
    <atom:Col Span="4">
        <TextBlock Text="Password:" HorizontalAlignment="Right" VerticalAlignment="Center" />
    </atom:Col>
    <atom:Col Span="8">
        <atom:LineEdit PlaceholderText="Enter password" PasswordChar="•" />
    </atom:Col>
</atom:Row>
```

### C# 代码创建栅格

```csharp
var row = new Row
{
    Gutter = GridGutter.Parse("16,16"),
    Justify = RowJustify.SpaceBetween,
    Align = RowAlign.Middle
};

for (int i = 0; i < 4; i++)
{
    var col = new Col
    {
        Span = new GridColSpanInfo(6),
        Content = new Border
        {
            Height = 48,
            Background = i % 2 == 0
                ? Brushes.CornflowerBlue
                : Brushes.SkyBlue
        }
    };
    row.Children.Add(col);
}
```
