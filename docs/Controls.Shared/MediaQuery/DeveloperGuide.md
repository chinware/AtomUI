# AtomUI 媒体查询系统 — 开发者指南

> 本文档面向 AtomUI 控件使用者和控件开发者，介绍如何在 AXAML 和 C# 中使用媒体查询实现响应式布局。

---

## 1. 概念：断点体系

AtomUI 遵循 Ant Design 5.0 的六级响应式断点。当窗口内容区域宽度达到某个阈值时，自动切换到对应断点：

| 缩写 | 断点 | 最小宽度 | 说明 |
|---|---|---|---|
| `xs` | ExtraSmall | < 576px | 手机竖屏 |
| `sm` | Small | ≥ 576px | 手机横屏 |
| `md` | Medium | ≥ 768px | 平板竖屏 |
| `lg` | Large | ≥ 992px | 小桌面（**默认**） |
| `xl` | ExtraLarge | ≥ 1200px | 标准桌面 |
| `xxl` | ExtraExtraLarge | ≥ 1600px | 大屏桌面 |

> **前提**：应用的 `Window` 必须使用 AtomUI 提供的 `atom:Window`（而非 Avalonia 原生 `Window`），因为断点检测逻辑内置在 AtomUI Window 的主题模板中。

---

## 2. Grid 栅格系统（Row / Col）

AtomUI 的 `Row` + `Col` 组合实现了 Ant Design 的 24 栏栅格系统。`Row` 是容器，`Col` 是列。

### 2.1 基础用法

```xml
<atom:Row Gutter="16,16">
    <atom:Col Span="8">
        <Border Background="#1677FF" Height="48" />
    </atom:Col>
    <atom:Col Span="8">
        <Border Background="#4096FF" Height="48" />
    </atom:Col>
    <atom:Col Span="8">
        <Border Background="#1677FF" Height="48" />
    </atom:Col>
</atom:Row>
```

- `Span` 指定列跨度（1~24）。
- `Gutter` 指定水平和垂直间距（`"水平,垂直"` 格式）。

### 2.2 响应式列跨度

通过 `Col` 的 `Xs`、`Sm`、`Md`、`Lg`、`Xl`、`Xxl` 属性，为不同断点设置不同的布局配置：

```xml
<atom:Row Gutter="16,16">
    <!-- 在 xs 断点全宽，sm 断点半宽，lg 断点 1/4 宽 -->
    <atom:Col Xs="24" Sm="12" Md="16" Lg="6" Xl="6">
        <Border Background="#1677FF" Height="48" />
    </atom:Col>
    <atom:Col Xs="24" Sm="12" Md="16" Lg="6" Xl="6">
        <Border Background="#4096FF" Height="48" />
    </atom:Col>
    <atom:Col Xs="24" Sm="12" Md="16" Lg="6" Xl="6">
        <Border Background="#1677FF" Height="48" />
    </atom:Col>
    <atom:Col Xs="24" Sm="12" Md="16" Lg="6" Xl="6">
        <Border Background="#4096FF" Height="48" />
    </atom:Col>
</atom:Row>
```

当窗口宽度 < 576px 时（`xs`），每列占满整行（24/24）；宽度 ≥ 576px 时（`sm`），每行两列（12/24）；宽度 ≥ 992px 时（`lg`），每行四列（6/24）。

**字符串格式**：断点属性的值可以是：
- **纯数字**：`"12"` → 仅设置 Span
- **键值对**：`"span:12,offset:2,order:1"` → 设置多个布局属性

### 2.3 使用 ColInfo 进行高级响应式覆盖

`ColInfo` 是 `GridColSize` 的别名，支持在特定断点下完整覆盖 Span、Offset、Order、Push、Pull：

**方式一：属性元素语法**

```xml
<atom:Col Span="24">
    <atom:Col.Md>
        <atom:ColInfo Span="12" Order="2" />
    </atom:Col.Md>
    <atom:Col.Lg>
        <atom:ColInfo Span="6" Order="1" />
    </atom:Col.Lg>
    <Border Background="#1677FF" Height="48" />
</atom:Col>
```

**方式二：标记扩展语法**

```xml
<atom:Col Span="6" Order="1" Sm="{atom:ColInfo Order=2}">
    <Border Background="#1677FF" Height="48" />
</atom:Col>
```

**方式三：StaticResource 共享**

```xml
<StackPanel.Resources>
    <atom:ColInfo x:Key="MdCol1" Span="12" Order="2" />
    <atom:ColInfo x:Key="LgCol1" Span="6" Order="1" />
</StackPanel.Resources>

<atom:Col Span="24" Md="{StaticResource MdCol1}" Lg="{StaticResource LgCol1}">
    <Border Background="#1677FF" Height="48" />
</atom:Col>
```

### 2.4 响应式 Span

`Col.Span` 属性也支持按断点指定不同的值：

```xml
<!-- 在 AXAML 中使用字符串格式 -->
<atom:Col Span="xs:24,sm:12,lg:8">
    <Border Background="#1677FF" Height="48" />
</atom:Col>
```

### 2.5 响应式 Gutter

`Row.Gutter` 支持按断点设置不同的间距：

```xml
<!-- 固定间距 -->
<atom:Row Gutter="16">...</atom:Row>

<!-- 水平 + 垂直固定间距 -->
<atom:Row Gutter="16,24">...</atom:Row>

<!-- 水平间距按断点变化 -->
<atom:Row Gutter="xs:8,sm:16,lg:24">...</atom:Row>

<!-- 水平和垂直都按断点变化（用分号分隔） -->
<atom:Row Gutter="xs:8,lg:16;xs:4,lg:12">...</atom:Row>
```

### 2.6 Offset / Push / Pull / Order

```xml
<!-- 偏移 -->
<atom:Col Span="8" Offset="8">...</atom:Col>

<!-- 推拉（视觉位置互换） -->
<atom:Col Span="18" Push="6">...</atom:Col>
<atom:Col Span="6" Pull="18">...</atom:Col>

<!-- 排序（逻辑顺序不变，视觉顺序改变） -->
<atom:Col Span="6" Order="4">...</atom:Col>
<atom:Col Span="6" Order="3">...</atom:Col>
<atom:Col Span="6" Order="2">...</atom:Col>
<atom:Col Span="6" Order="1">...</atom:Col>
```

### 2.7 Justify 和 Align

```xml
<!-- 主轴对齐（水平方向） -->
<atom:Row Justify="center">...</atom:Row>
<atom:Row Justify="SpaceBetween">...</atom:Row>
<atom:Row Justify="SpaceAround">...</atom:Row>
<atom:Row Justify="SpaceEvenly">...</atom:Row>

<!-- 交叉轴对齐（垂直方向） -->
<atom:Row Align="top">...</atom:Row>
<atom:Row Align="middle">...</atom:Row>
<atom:Row Align="bottom">...</atom:Row>
<atom:Row Align="stretch">...</atom:Row>    <!-- 默认 -->
```

---

## 3. Form 表单响应式布局

`Form` 和 `FormItem` 的标签/内容列宽可以通过 `MediaBreakGridLength` 实现响应式：

### 3.1 基本用法

```xml
<atom:Form LabelColInfo="xs:1*,lg:120"
           WrapperColInfo="xs:3*,lg:*">
    <atom:FormItem LabelText="Username">
        <atom:TextBox />
    </atom:FormItem>
    <atom:FormItem LabelText="Password">
        <atom:TextBox />
    </atom:FormItem>
</atom:Form>
```

- `LabelColInfo`：标签列的 GridLength，按断点配置。
- `WrapperColInfo`：内容列的 GridLength，按断点配置。
- 值格式支持标准 GridLength 语法：`"120"`（像素）、`"*"`（比例）、`"Auto"`、`"2*"`（2 倍比例）。

### 3.2 字符串格式

```
"xs:1*,lg:120"
```

解析规则：
- 逗号分隔多个断点设置
- 每个断点格式为 `缩写:GridLength值`
- 支持的缩写：`xs`、`sm`、`md`、`lg`、`xl`、`xxl`
- 单一值（无冒号）应用于所有断点：`"120"` → 所有断点均 120px

### 3.3 FormItem 级别覆盖

`FormItem` 也有自己的 `LabelColInfo` 和 `WrapperColInfo` 属性，可以覆盖 `Form` 级别的设置：

```xml
<atom:Form LabelColInfo="120" WrapperColInfo="*">
    <!-- 大多数表单项使用 Form 级别的设置 -->
    <atom:FormItem LabelText="Name">
        <atom:TextBox />
    </atom:FormItem>
    
    <!-- 这个表单项使用自定义的响应式列宽 -->
    <atom:FormItem LabelText="Address"
                   LabelColInfo="xs:1*,lg:200"
                   WrapperColInfo="xs:3*,lg:*">
        <atom:TextBox />
    </atom:FormItem>
</atom:Form>
```

---

## 4. Descriptions 描述列表

`Descriptions` 控件的列数支持响应式配置：

### 4.1 基本用法

```xml
<atom:Descriptions Header="User Info" ColumnInfo="3">
    <atom:DescriptionItem Label="UserName" Content="Zhou Maomao" />
    <atom:DescriptionItem Label="Telephone" Content="1810000000" />
    <atom:DescriptionItem Label="Live" Content="Hangzhou, Zhejiang" />
</atom:Descriptions>
```

### 4.2 响应式列数

```xml
<!-- xs 断点 1 列，sm 断点 2 列，lg 断点 3 列 -->
<atom:Descriptions Header="User Info" ColumnInfo="xs:1,sm:2,lg:3">
    <atom:DescriptionItem Label="UserName" Content="Zhou Maomao" />
    <atom:DescriptionItem Label="Telephone" Content="1810000000" />
    <atom:DescriptionItem Label="Live" Content="Hangzhou, Zhejiang" />
</atom:Descriptions>
```

### 4.3 响应式项目跨度

`DescriptionItem.Span` 属性也支持按断点配置：

```xml
<atom:DescriptionItem Label="Address"
                       Content="No. 18, Wantang Road"
                       Span="xs:1,lg:2" />
```

---

## 5. 控件开发者：如何创建响应式控件

如果你正在为 AtomUI 开发新控件并需要响应断点变化，请遵循以下模式。

### 5.1 订阅断点变更

```csharp
using AtomUI.Controls;
using Avalonia;
using Avalonia.Controls;

namespace AtomUI.Desktop.Controls;

public class MyResponsiveControl : Control
{
    private MediaBreakPoint? _breakPoint;
    private IMediaBreakAwareControl? _mediaOwner;

    protected override void OnAttachedToVisualTree(VisualTreeAttachmentEventArgs e)
    {
        base.OnAttachedToVisualTree(e);
        // 通过接口获取（推荐，更通用）
        if (TopLevel.GetTopLevel(this) is IMediaBreakAwareControl mediaOwner)
        {
            _mediaOwner = mediaOwner;
            _breakPoint = mediaOwner.MediaBreakPoint;
            mediaOwner.MediaBreakPointChanged += HandleMediaBreakChanged;
        }
    }

    protected override void OnDetachedFromVisualTree(VisualTreeAttachmentEventArgs e)
    {
        base.OnDetachedFromVisualTree(e);
        if (_mediaOwner != null)
        {
            _mediaOwner.MediaBreakPointChanged -= HandleMediaBreakChanged;
            _mediaOwner = null;
        }
    }

    private void HandleMediaBreakChanged(object? sender, MediaBreakPointChangedEventArgs args)
    {
        _breakPoint = args.MediaBreakPoint;
        // 触发重新布局或刷新状态
        InvalidateMeasure();
        InvalidateArrange();
    }

    private MediaBreakPoint GetBreakPoint()
    {
        if (_breakPoint.HasValue)
        {
            return _breakPoint.Value;
        }
        // 降级：尝试即时获取
        if (TopLevel.GetTopLevel(this) is IMediaBreakAwareControl mediaOwner)
        {
            _breakPoint = mediaOwner.MediaBreakPoint;
            return _breakPoint.Value;
        }
        // 最终降级：使用默认值
        return MediaBreakPoint.Large;
    }
}
```

### 5.2 定义响应式断点信息类型

如果你的控件需要一个新的响应式属性（类似 `GridGutterInfo`），遵循已有的 record 模式：

```csharp
public record MyMediaBreakInfo
{
    public double ExtraSmall { get; init; }
    public double Small { get; init; }
    public double Medium { get; init; }
    public double Large { get; init; }
    public double ExtraLarge { get; init; }
    public double ExtraExtraLarge { get; init; }

    // 统一值构造
    public MyMediaBreakInfo(double value)
    {
        ExtraSmall = Small = Medium = Large = ExtraLarge = ExtraExtraLarge = value;
    }

    // 按断点获取值
    public double GetValue(MediaBreakPoint breakPoint)
    {
        return breakPoint switch
        {
            MediaBreakPoint.ExtraSmall      => ExtraSmall,
            MediaBreakPoint.Small           => Small,
            MediaBreakPoint.Medium          => Medium,
            MediaBreakPoint.Large           => Large,
            MediaBreakPoint.ExtraLarge      => ExtraLarge,
            _                               => ExtraExtraLarge
        };
    }

    // 支持 "xs:10,lg:20" 格式的字符串解析
    public static MyMediaBreakInfo Parse(string input) { ... }
}
```

关键约定：
- 使用 `record` 类型，支持 `with` 表达式。
- 提供统一值构造函数（所有断点相同）。
- 提供 `Parse(string)` 方法，支持 `缩写:值` 的逗号分隔格式。
- 断点缩写统一为小写：`xs`、`sm`、`md`、`lg`、`xl`、`xxl`（解析时忽略大小写）。
- 如果需要 AXAML 字符串转换，提供 `TypeConverter`。

### 5.3 字符串格式规范

所有响应式属性的字符串解析遵循统一规范：

```
单一值:
  "16"         → 所有断点使用 16

分断点（逗号分隔）:
  "xs:8,sm:16,lg:24"  → xs=8, sm=16, lg=24, 未指定的保持默认值

键值对分隔符:
  冒号 (:)      → 断点名与值之间
  逗号 (,)      → 多个断点之间
  分号 (;)      → 仅 GridGutter: 水平与垂直之间
```

---

## 6. 底层原理：Avalonia Container Query

AtomUI 的媒体查询构建在 Avalonia 的 Container Query 功能之上。理解底层机制有助于排查问题。

### 6.1 核心概念

1. **Container（容器）**：通过 `Container.Name` 和 `Container.Sizing` 附加属性声明。容器会跟踪自身尺寸。
2. **ContainerQuery**：声明在 `ControlTheme` 或 `Styles` 中的查询规则。当关联容器的尺寸满足条件时，激活内部的 Style。
3. **查询语法**：
   - `min-width:576` → 宽度 ≥ 576px 时激活
   - `max-width:575` → 宽度 ≤ 575px 时激活
   - `min-width:400 and max-width:800` → 宽度在 400~800px 之间
   - `max-width:300,min-height:600` → 宽度 ≤ 300px **或** 高度 ≥ 600px

### 6.2 AtomUI 的封装

AtomUI 没有直接让每个控件自行声明 ContainerQuery。而是：

1. **Window 的主题模板**中声明了一个全局容器 `PART_GlobalQueryContainer`。
2. 6 组 `ContainerQuery` 监听此容器的宽度变化。
3. 匹配时设置一个不可见的 `WindowMediaQueryIndicator` 控件的 `MediaBreakPoint` 属性。
4. `WindowMediaQueryIndicator` 将变更通知给 `Window`。
5. `Window` 通过 `MediaBreakPointChanged` 事件广播给所有订阅者。

这种集中式设计的优势：
- **单一检测点**：只有一个容器查询管道，避免重复计算。
- **统一断点**：所有控件始终看到相同的断点值。
- **可预测性**：断点变更的时序和顺序是确定的。

### 6.3 限制与注意事项

1. **必须使用 `atom:Window`**：标准 Avalonia `Window` 不包含 ContainerQuery 声明，断点不会生效。
2. **ContainerQuery 不能影响容器本身**：这是 Avalonia 的设计约束，因此需要 `WindowMediaQueryIndicator` 作为中继。
3. **断点基于内容区域宽度**：不是整个窗口宽度，而是 `ContentFrame`（扣除标题栏和 Padding 后的区域）的宽度。
4. **默认断点是 `Large`**：如果控件在附加到视觉树之前需要断点信息，应使用 `MediaBreakPoint.Large` 作为回退值。

---

## 7. 完整示例

### 7.1 响应式三栏布局

```xml
<atom:Row Gutter="16,16" Wrap="True">
    <!-- 大屏三栏，中屏两栏，小屏单栏 -->
    <atom:Col Xs="24" Md="12" Lg="8">
        <Border Background="#1677FF" Height="100" CornerRadius="4">
            <TextBlock Text="Panel 1" Foreground="White"
                       HorizontalAlignment="Center" VerticalAlignment="Center" />
        </Border>
    </atom:Col>
    <atom:Col Xs="24" Md="12" Lg="8">
        <Border Background="#4096FF" Height="100" CornerRadius="4">
            <TextBlock Text="Panel 2" Foreground="White"
                       HorizontalAlignment="Center" VerticalAlignment="Center" />
        </Border>
    </atom:Col>
    <atom:Col Xs="24" Md="24" Lg="8">
        <Border Background="#1677FF" Height="100" CornerRadius="4">
            <TextBlock Text="Panel 3" Foreground="White"
                       HorizontalAlignment="Center" VerticalAlignment="Center" />
        </Border>
    </atom:Col>
</atom:Row>
```

### 7.2 响应式表单

```xml
<atom:Form LabelColInfo="xs:*,lg:120"
           WrapperColInfo="xs:3*,lg:*">
    <atom:FormItem LabelText="Username">
        <atom:TextBox Watermark="Please input username" />
    </atom:FormItem>
    <atom:FormItem LabelText="Email">
        <atom:TextBox Watermark="Please input email" />
    </atom:FormItem>
    <atom:FormItem LabelText="Address"
                   LabelColInfo="xs:*,lg:200"
                   WrapperColInfo="xs:2*,lg:*">
        <atom:TextBox Watermark="Detailed address" />
    </atom:FormItem>
</atom:Form>
```

### 7.3 响应式描述列表

```xml
<atom:Descriptions Header="Order Info"
                    ColumnInfo="xs:1,sm:2,lg:3"
                    IsBordered="True">
    <atom:DescriptionItem Label="Product" Content="Cloud Database" />
    <atom:DescriptionItem Label="Billing" Content="Prepaid" />
    <atom:DescriptionItem Label="Renewal" Content="YES" />
    <atom:DescriptionItem Label="Order Time" Content="2024-01-15 10:00:00" />
    <atom:DescriptionItem Label="Usage Time" Content="2025-01-15 10:00:00" Span="2" />
    <atom:DescriptionItem Label="Status" Content="Running" Span="xs:1,lg:3" />
</atom:Descriptions>
```

### 7.4 响应式列序变换

```xml
<atom:Row Gutter="16,16">
    <!-- 大屏：按 1-2-3-4 排列；小屏（sm）：按 2-1-4-3 排列 -->
    <atom:Col Span="6" Order="1" Sm="{atom:ColInfo Order=2}">
        <Border Background="#1677FF" Height="48" />
    </atom:Col>
    <atom:Col Span="6" Order="2" Sm="{atom:ColInfo Order=1}">
        <Border Background="#4096FF" Height="48" />
    </atom:Col>
    <atom:Col Span="6" Order="3" Sm="{atom:ColInfo Order=4}">
        <Border Background="#1677FF" Height="48" />
    </atom:Col>
    <atom:Col Span="6" Order="4" Sm="{atom:ColInfo Order=3}">
        <Border Background="#4096FF" Height="48" />
    </atom:Col>
</atom:Row>
```

