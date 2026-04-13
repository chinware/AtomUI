# Pagination 使用文档

本文档介绍 AtomUI Pagination 控件的各种使用方式，示例代码摘自 Gallery 演示程序。

> 📖 Gallery 源码位置：`controlgallery/AtomUIGallery/ShowCases/Views/Navigation/PaginationShowCase.axaml`

---

## 前置准备

在 AXAML 中使用 Pagination，需要引入以下命名空间：

```xml
xmlns:atom="https://atomui.net"
```

在 C# 代码中引入：

```csharp
using AtomUI.Desktop.Controls;   // Pagination, SimplePagination 控件
using AtomUI.Controls;            // SizeType, PageChangedEventArgs 等共享类型
```

---

## 1. 基本分页

最简单的分页用法，只需设置 `Total` 和 `CurrentPage` 即可。默认每页 10 条数据。

```xml
<atom:Pagination Total="50" CurrentPage="1" />
```

---

## 2. 对齐方式

分页组件支持三种对齐方式：左对齐（默认）、居中、右对齐。

```xml
<StackPanel Orientation="Vertical" Spacing="10">
    <atom:Pagination Total="50" CurrentPage="1" Align="Start" />
    <atom:Pagination Total="50" CurrentPage="1" Align="Center" />
    <atom:Pagination Total="50" CurrentPage="1" Align="End" />
</StackPanel>
```

---

## 3. 更多页码

当数据量较大时，分页会自动使用省略号折叠中间页码，配合 `IsShowSizeChanger` 可让用户切换每页条数。

```xml
<atom:Pagination Total="500" CurrentPage="6" IsShowSizeChanger="True" />
```

---

## 4. 页面大小选择器 + 快速跳转

同时启用页面大小选择器和快速跳转功能，适合大数据量场景：

```xml
<atom:Pagination Total="500" CurrentPage="3"
                 IsShowSizeChanger="True"
                 IsShowQuickJumper="True" />
```

快速跳转栏会显示 "Go to [输入框] Page" 的布局（中文环境显示 "跳至 [输入框] 页"），用户输入页码后按 Enter 即可跳转。

---

## 5. 禁用状态

通过 `IsEnabled="False"` 禁用分页组件，所有页码按钮、下拉框和输入框均不可交互：

```xml
<StackPanel Orientation="Vertical" Spacing="10">
    <!-- 正常 -->
    <atom:Pagination Total="500" CurrentPage="3"
                     IsShowSizeChanger="True"
                     IsShowQuickJumper="True" />
    <!-- 禁用 -->
    <atom:Pagination Total="500" CurrentPage="3"
                     IsShowSizeChanger="True"
                     IsShowQuickJumper="True"
                     IsEnabled="False" />
</StackPanel>
```

---

## 6. 迷你尺寸

通过 `SizeType="Small"` 使用紧凑布局，适合空间受限场景：

```xml
<StackPanel Orientation="Vertical" Spacing="10">
    <!-- 基本迷你分页 -->
    <atom:Pagination Total="50" CurrentPage="1" SizeType="Small" />

    <!-- 迷你 + SizeChanger + QuickJumper -->
    <atom:Pagination Total="50" CurrentPage="1" SizeType="Small"
                     IsShowSizeChanger="True"
                     IsShowQuickJumper="True" />

    <!-- 迷你 + 总数信息 -->
    <atom:Pagination Total="50" CurrentPage="1" SizeType="Small"
                     IsShowTotalInfo="True" />

    <!-- 迷你 + 全功能 + 禁用 -->
    <atom:Pagination Total="50" CurrentPage="1" SizeType="Small"
                     IsShowTotalInfo="True"
                     IsShowSizeChanger="True"
                     IsShowQuickJumper="True"
                     IsEnabled="False" />
</StackPanel>
```

---

## 7. 总数信息

### 默认格式

启用 `IsShowTotalInfo` 后，默认显示本地化的总数文本（如 "Total 85 items" 或 "共 85 项"）：

```xml
<atom:Pagination Total="85" CurrentPage="1" PageSize="20"
                 IsShowSizeChanger="True"
                 IsShowTotalInfo="True" />
```

### 自定义模板

通过 `TotalInfoTemplate` 自定义显示格式，支持三个变量替换：
- `${Total}` — 总数据条数
- `${RangeStart}` — 当前页起始条数
- `${RangeEnd}` — 当前页结束条数

```xml
<atom:Pagination Total="85" CurrentPage="1" PageSize="20"
                 IsShowSizeChanger="True"
                 IsShowTotalInfo="True"
                 TotalInfoTemplate="${RangeStart}-${RangeEnd} of ${Total} items" />
```

---

## 8. 简洁模式

SimplePagination 提供轻量级分页体验，仅包含前后页按钮和当前页/总页数文本。

### 只读模式（默认）

```xml
<atom:SimplePagination Total="50" CurrentPage="1" />
```

显示 "1 / 5" 文本，通过左右箭头翻页。

### 可编辑模式

```xml
<atom:SimplePagination Total="50" CurrentPage="1" IsReadOnly="False" />
```

当前页显示为可编辑输入框，用户可直接输入目标页码并按 Enter 跳转。

### 迷你尺寸的简洁分页

```xml
<StackPanel Orientation="Vertical" Spacing="10">
    <atom:SimplePagination Total="50" CurrentPage="1" SizeType="Small" />
    <atom:SimplePagination Total="50" CurrentPage="1" SizeType="Small"
                           IsReadOnly="False" />
</StackPanel>
```

### 禁用的简洁分页

```xml
<atom:SimplePagination Total="50" CurrentPage="1"
                       IsReadOnly="False"
                       IsEnabled="False" />
```

---

## 9. 指定每页条数

通过 `PageSize` 属性指定每页数据条数（允许值：10、20、50、100）：

```xml
<atom:Pagination Total="85" CurrentPage="1" PageSize="20"
                 IsShowSizeChanger="True"
                 IsShowTotalInfo="True" />
```

当同时启用 `IsShowSizeChanger` 时，下拉框会自动选中与 `PageSize` 匹配的选项。

---

## 10. 单页隐藏

当数据量不足一页时自动隐藏分页组件：

```xml
<atom:Pagination Total="5" CurrentPage="1"
                 IsHideOnSinglePage="True" />
```

---

## 11. 页变更事件监听

通过 `CurrentPageChanged` 事件监听页码变化：

```xml
<atom:Pagination Total="500" CurrentPage="1"
                 CurrentPageChanged="HandlePageChanged" />
```

```csharp
// Code-behind
public void HandlePageChanged(object? sender, PageChangedEventArgs args)
{
    Console.WriteLine($"当前页: {args.PageIndex}, 总页数: {args.TotalPages}, 每页: {args.PageSize}");
    // 在此加载对应页的数据
}
```

---

## 12. MVVM 数据绑定

Pagination 的所有属性均支持数据绑定：

```xml
<atom:Pagination Total="{Binding TotalItems}"
                 CurrentPage="{Binding CurrentPage, Mode=TwoWay}"
                 PageSize="{Binding PageSize, Mode=TwoWay}"
                 IsShowSizeChanger="True"
                 IsShowQuickJumper="True"
                 IsShowTotalInfo="True" />
```

```csharp
// ViewModel（使用 ReactiveUI）
public class DataListViewModel : ReactiveObject
{
    [Reactive]
    public int TotalItems { get; set; } = 500;

    [Reactive]
    public int CurrentPage { get; set; } = 1;

    [Reactive]
    public int PageSize { get; set; } = 10;

    public DataListViewModel()
    {
        // 监听页码和每页条数变化，自动刷新数据
        this.WhenAnyValue(x => x.CurrentPage, x => x.PageSize)
            .Subscribe(_ => LoadData());
    }

    private void LoadData()
    {
        // 根据 CurrentPage 和 PageSize 加载数据
    }
}
```

---

## 13. 控制动画行为

```xml
<!-- 禁用页码按钮的背景/边框颜色过渡动画 -->
<atom:Pagination Total="500" CurrentPage="1" IsMotionEnabled="False" />
```

---

## 常见组合模式

### 表格底部分页

```xml
<DockPanel>
    <!-- 表格内容 -->
    <atom:DataGrid DockPanel.Dock="Top" ... />

    <!-- 底部分页栏 -->
    <atom:Pagination DockPanel.Dock="Bottom"
                     Align="End"
                     Total="{Binding TotalItems}"
                     CurrentPage="{Binding CurrentPage, Mode=TwoWay}"
                     PageSize="{Binding PageSize, Mode=TwoWay}"
                     IsShowSizeChanger="True"
                     IsShowTotalInfo="True"
                     Margin="0,16,0,0" />
</DockPanel>
```

### 列表 + 居中分页

```xml
<StackPanel>
    <!-- 列表内容 -->
    <ItemsRepeater Items="{Binding Items}" />

    <!-- 居中分页 -->
    <atom:Pagination Align="Center"
                     Total="{Binding TotalItems}"
                     CurrentPage="{Binding CurrentPage, Mode=TwoWay}"
                     Margin="0,24" />
</StackPanel>
```

### 迷你分页（卡片内嵌）

```xml
<Border Padding="16" CornerRadius="8" Background="White">
    <StackPanel>
        <!-- 卡片内容 -->
        <TextBlock Text="数据列表" FontWeight="Bold" />

        <!-- 紧凑分页 -->
        <atom:SimplePagination Total="{Binding TotalItems}"
                               CurrentPage="{Binding CurrentPage, Mode=TwoWay}"
                               SizeType="Small"
                               Margin="0,8,0,0" />
    </StackPanel>
</Border>
```
