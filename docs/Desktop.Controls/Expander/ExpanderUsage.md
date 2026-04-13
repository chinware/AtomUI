# Expander 使用文档

本文档介绍 AtomUI Expander 控件的各种使用方式，示例代码摘自 Gallery 演示程序。

> 📖 Gallery 源码位置：`controlgallery/AtomUIGallery/ShowCases/Views/DataDisplay/ExpanderShowCase.axaml`

---

## 前置准备

```xml
xmlns:atom="https://atomui.net"
xmlns:antdicons="https://atomui.net/icons/antdesign"
```

```csharp
using AtomUI.Desktop.Controls;
```

---

## 1. 基本用法

最简单的用法是创建一个向下展开的面板：

```xml
<atom:Expander Header="This is panel header 1">
    <atom:TextBlock TextWrapping="Wrap">
        A dog is a type of domesticated animal. Known for its loyalty and faithfulness,
        it can be found as a welcome guest in many households across the world.
    </atom:TextBlock>
</atom:Expander>
```

---

## 2. 三种尺寸

Expander 支持 Large / Middle / Small 三种尺寸，通过 `SizeType` 属性控制：

```xml
<StackPanel Orientation="Vertical" Spacing="20">
    <atom:Expander SizeType="Middle" Header="This is default size panel header">
        <atom:TextBlock TextWrapping="Wrap">
            A dog is a type of domesticated animal...
        </atom:TextBlock>
    </atom:Expander>

    <atom:Expander SizeType="Small" Header="This is small size panel header">
        <atom:TextBlock TextWrapping="Wrap">
            A dog is a type of domesticated animal...
        </atom:TextBlock>
    </atom:Expander>

    <atom:Expander SizeType="Large" Header="This is large size panel header">
        <atom:TextBlock TextWrapping="Wrap">
            A dog is a type of domesticated animal...
        </atom:TextBlock>
    </atom:Expander>
</StackPanel>
```

---

## 3. 展开方向

Expander 支持四个方向的展开（Down / Up / Left / Right），水平方向时头部会自动旋转：

```xml
<atom:Expander Header="This is panel header"
               ExpandDirection="Down">
    <atom:TextBlock TextWrapping="Wrap">
        A dog is a type of domesticated animal...
    </atom:TextBlock>
</atom:Expander>
```

可通过 `OptionButtonGroup` 动态切换展开方向（参见 Gallery 示例）。

---

## 4. 嵌套面板

Expander 可以嵌套在另一个 Expander 内部：

```xml
<atom:Expander Header="This is panel header 1">
    <atom:Expander Header="This is nested panel header">
        <atom:TextBlock TextWrapping="Wrap">
            A dog is a type of domesticated animal...
        </atom:TextBlock>
    </atom:Expander>
</atom:Expander>
```

---

## 5. 无边框模式

通过 `IsBorderless` 属性移除外边框，适用于嵌入到有自己边框容器的场景：

```xml
<atom:Expander IsBorderless="True" Header="This is panel header 1">
    <atom:TextBlock TextWrapping="Wrap">
        A dog is a type of domesticated animal...
    </atom:TextBlock>
</atom:Expander>
```

---

## 6. 隐藏展开图标

通过 `IsShowExpandIcon` 属性隐藏箭头图标：

```xml
<atom:Expander Header="This is panel header 1" IsShowExpandIcon="False">
    <atom:TextBlock TextWrapping="Wrap">
        A dog is a type of domesticated animal...
    </atom:TextBlock>
</atom:Expander>
```

---

## 7. 展开图标位置 + 附加内容

通过 `ExpandIconPosition` 切换图标在头部的左侧或右侧。同时可以通过 `AddOnContent` 在头部添加附加内容（如设置图标）：

```xml
<atom:Expander Header="This is panel header 1"
               AddOnContent="{antdicons:AntDesignIconProvider Kind=SettingOutlined}"
               ExpandIconPosition="End">
    <atom:TextBlock TextWrapping="Wrap">
        A dog is a type of domesticated animal...
    </atom:TextBlock>
</atom:Expander>
```

---

## 8. 幽灵模式

通过 `IsGhostStyle` 使头部背景透明：

```xml
<atom:Expander IsGhostStyle="True" Header="This is panel header 1">
    <atom:TextBlock TextWrapping="Wrap">
        A dog is a type of domesticated animal...
    </atom:TextBlock>
</atom:Expander>
```

---

## 9. 触发方式

通过 `TriggerType` 属性控制触发区域。`Header` 模式下点击头部任意位置可展开/收起；`Icon` 模式下仅点击图标可触发：

```xml
<StackPanel Orientation="Vertical" Spacing="10">
    <!-- 点击头部触发（默认） -->
    <atom:Expander Header="This panel can only be collapsed by clicking text">
        <atom:TextBlock TextWrapping="Wrap">
            A dog is a type of domesticated animal...
        </atom:TextBlock>
    </atom:Expander>

    <!-- 仅图标触发 -->
    <atom:Expander TriggerType="Icon"
                   Header="This panel can only be collapsed by clicking icon">
        <atom:TextBlock TextWrapping="Wrap">
            A dog is a type of domesticated animal...
        </atom:TextBlock>
    </atom:Expander>

    <!-- 禁用状态（不可折叠） -->
    <atom:Expander IsEnabled="False" Header="This panel can't be collapsed">
        <atom:TextBlock TextWrapping="Wrap">
            A dog is a type of domesticated animal...
        </atom:TextBlock>
    </atom:Expander>
</StackPanel>
```

---

## 10. 自定义间距

通过 `HeaderPadding` 和 `ContentPadding` 属性自定义面板头部和内容区域的内边距：

```xml
<StackPanel Orientation="Vertical" Spacing="20">
    <atom:Expander Header="This is panel header 1"
                   HeaderPadding="5" ContentPadding="5">
        <atom:TextBlock TextWrapping="Wrap">
            A dog is a type of domesticated animal...
        </atom:TextBlock>
    </atom:Expander>

    <atom:Expander Header="This is panel header 1"
                   IsGhostStyle="True" HeaderPadding="5" ContentPadding="5">
        <atom:TextBlock TextWrapping="Wrap">
            A dog is a type of domesticated animal...
        </atom:TextBlock>
    </atom:Expander>
</StackPanel>
```
