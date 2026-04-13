# Empty 使用文档

本文档介绍 AtomUI Empty 控件的各种使用方式，示例代码摘自 Gallery 演示程序。

> 📖 Gallery 源码位置：`controlgallery/AtomUIGallery/ShowCases/Views/DataDisplay/EmptyShowCase.axaml`

---

## 前置准备

在 AXAML 中使用 Empty，需要引入以下命名空间：

```xml
xmlns:atom="https://atomui.net"
```

在 C# 代码中引入：

```csharp
using AtomUI.Desktop.Controls;   // Empty 控件
using AtomUI.Controls;            // SizeType、PresetEmptyImage 等共享类型
```

---

## 1. 基本用法

最基本的空状态展示，使用默认预设图片：

```xml
<atom:Empty PresetImage="Default" />
```

默认情况下，Empty 控件会：
- 显示彩色的默认预设图片
- 在图片下方显示本地化的描述文字（英文 `"No Data"`，中文 `"暂无数据"`）
- 水平和垂直居中显示
- 使用 `Middle` 尺寸

---

## 2. 简洁模式

Ant Design 中的 `PRESENTED_IMAGE_SIMPLE`，适用于在列表/表格内显示小尺寸空状态：

```xml
<atom:Empty PresetImage="Simple" />
```

Simple 模式使用灰色线条图，视觉更轻量，适合作为容器内的内嵌空状态。

---

## 3. 尺寸切换

通过 `SizeType` 属性控制图片大小和描述文字间距：

```xml
<StackPanel Orientation="Horizontal" Spacing="10">
    <atom:Empty PresetImage="Simple" SizeType="Small" />
    <atom:Empty PresetImage="Simple" SizeType="Middle" />
    <atom:Empty PresetImage="Simple" SizeType="Large" />
</StackPanel>
```

三种尺寸的具体效果：

| 尺寸 | 图片高度 | 描述文字间距 | 适用场景 |
|---|---|---|---|
| `Small` | 约 35px | 较小 | 表格单元格、紧凑列表 |
| `Middle` | 约 74px | 较小 | 卡片、面板 |
| `Large` | 约 100px | 较大 | 页面级空状态 |

---

## 4. 自定义描述文字

```xml
<!-- 自定义描述文字 -->
<atom:Empty PresetImage="Default" Description="Customize Description" />

<!-- 中文描述 -->
<atom:Empty PresetImage="Simple" Description="暂无搜索结果" />
```

---

## 5. 隐藏描述文字

通过 `IsShowDescription="False"` 隐藏描述文字，仅显示图片：

```xml
<atom:Empty PresetImage="Default" IsShowDescription="False" />
```

---

## 6. 自定义图片

### 使用 SVG 文件路径

通过 `ImagePath` 使用打包在应用资源中的自定义 SVG 图片：

```xml
<atom:Empty ImagePath="avares://AtomUIGallery/Assets/EmptyShowCase/empty.svg"
            SizeType="Large"
            Description="Customize Description" />
```

> 📖 Gallery 中的自定义图片示例：`controlgallery/AtomUIGallery/ShowCases/Views/DataDisplay/EmptyShowCase.axaml`

### 使用 SVG 内容字符串

通过 `ImageSource` 直接传入 SVG 源码字符串，适合动态生成的图片：

```csharp
// Code-behind
var svgContent = """
    <svg width="64" height="41" xmlns="http://www.w3.org/2000/svg">
        <circle cx="32" cy="20" r="18" fill="#e6e6e6" stroke="#d9d9d9" />
        <text x="32" y="25" text-anchor="middle" fill="#999">?</text>
    </svg>
    """;
myEmpty.ImageSource = svgContent;
```

> ⚠️ **互斥规则**：`PresetImage`、`ImagePath`、`ImageSource` 三者只能设置其中一个。同时设置多个会抛出异常。

---

## 7. 配合额外操作按钮

Ant Design 中 Empty 支持 `children` 插槽放置额外内容（如创建按钮）。AtomUI 中可通过外部组合实现：

```xml
<StackPanel Orientation="Vertical" Spacing="10">
    <atom:Empty ImagePath="avares://AtomUIGallery/Assets/EmptyShowCase/empty.svg"
                SizeType="Large"
                Description="Customize Description" />
    <atom:Button HorizontalAlignment="Center" ButtonType="Primary">Create Now</atom:Button>
</StackPanel>
```

---

## 8. 在数据容器中使用

### 在 Card 中使用

```xml
<atom:Card Header="Data List">
    <!-- 无数据时显示 -->
    <atom:Empty PresetImage="Simple" Description="No items" />
</atom:Card>
```

### 在列表中条件显示

```xml
<!-- AXAML：数据绑定示例（伪代码） -->
<Panel>
    <!-- 数据列表 -->
    <ListBox Items="{Binding Items}"
             IsVisible="{Binding HasItems}" />
    
    <!-- 空状态 -->
    <atom:Empty PresetImage="Simple"
                Description="暂无数据"
                IsVisible="{Binding !HasItems}" />
</Panel>
```

---

## 9. 预设图片类型对比

两种预设图片可通过 `PresetImage` 属性切换：

```xml
<!-- 默认预设图：彩色，适合独立展示 -->
<atom:Empty PresetImage="Default" Description="Default style" />

<!-- 简洁预设图：灰色线条，适合内嵌展示 -->
<atom:Empty PresetImage="Simple" Description="Simple style" />
```

**选择指南**：

| 场景 | 推荐预设 | 推荐尺寸 |
|---|---|---|
| 页面级空状态（整个页面无内容） | `Default` | `Large` |
| 面板/卡片内空状态 | `Default` 或 `Simple` | `Middle` |
| 表格/列表内嵌空状态 | `Simple` | `Small` 或 `Middle` |
| 下拉选择器空选项 | `Simple` | `Small` |

