# GroupBox 使用文档

本文档介绍 AtomUI GroupBox 控件的各种使用方式，示例代码摘自 Gallery 演示程序。

> 📖 Gallery 源码位置：`controlgallery/AtomUIGallery/ShowCases/Views/DataDisplay/GroupBoxShowCase.axaml`

---

## 前置准备

在 AXAML 中使用 GroupBox，需要引入以下命名空间：

```xml
xmlns:atom="https://atomui.net"
xmlns:antdicons="https://atomui.net/icons/antdesign"
```

在 C# 代码中引入：

```csharp
using AtomUI.Desktop.Controls;   // GroupBox 控件、GroupBoxTitlePosition 枚举
```

---

## 1. 基本用法

最基本的 GroupBox 用法——设置标题并放入内容：

```xml
<atom:GroupBox HeaderTitle="Title Info">
    <Panel Height="100">
        <atom:TextBlock HorizontalAlignment="Center" VerticalAlignment="Center">
            Content of group box
        </atom:TextBlock>
    </Panel>
</atom:GroupBox>
```

GroupBox 继承自 `ContentControl`，可以容纳任意单个子控件。通常放置一个布局面板（`StackPanel`、`Grid`、`DockPanel` 等）来组织多个子控件。

---

## 2. 标题位置

通过 `HeaderTitlePosition` 属性控制标题在顶部边框上的水平位置，支持 `Left`（默认）、`Center`、`Right` 三种。

```xml
<StackPanel Orientation="Vertical" Spacing="10">
    <!-- 左对齐（默认） -->
    <atom:GroupBox HeaderTitle="Title Info">
        <Panel Height="40">
            <atom:TextBlock HorizontalAlignment="Center" VerticalAlignment="Center">
                Content of group box
            </atom:TextBlock>
        </Panel>
    </atom:GroupBox>

    <!-- 居中 -->
    <atom:GroupBox HeaderTitle="Title Info" HeaderTitlePosition="Center">
        <Panel Height="40">
            <atom:TextBlock HorizontalAlignment="Center" VerticalAlignment="Center">
                Content of group box
            </atom:TextBlock>
        </Panel>
    </atom:GroupBox>

    <!-- 右对齐 -->
    <atom:GroupBox HeaderTitle="Title Info" HeaderTitlePosition="Right">
        <Panel Height="40">
            <atom:TextBlock HorizontalAlignment="Center" VerticalAlignment="Center">
                Content of group box
            </atom:TextBlock>
        </Panel>
    </atom:GroupBox>
</StackPanel>
```

**使用场景指引**：
- **Left**：最常见的位置，适用于大多数表单分组场景
- **Center**：适用于对称布局或需要强调分组标题的场景
- **Right**：适用于特殊的设计需求或 RTL（从右到左）布局

---

## 3. 标题字体样式

GroupBox 提供 `HeaderFontStyle`、`HeaderFontWeight`、`HeaderTitleColor` 三个属性来精细控制标题的字体外观：

```xml
<StackPanel Orientation="Vertical" Spacing="10">
    <!-- 斜体标题 -->
    <atom:GroupBox HeaderTitle="Title Info" HeaderFontStyle="Italic">
        <Panel Height="40">
            <atom:TextBlock HorizontalAlignment="Center" VerticalAlignment="Center">
                Content of group box
            </atom:TextBlock>
        </Panel>
    </atom:GroupBox>

    <!-- 粗体标题 -->
    <atom:GroupBox HeaderTitle="Title Info"
                   HeaderTitlePosition="Center"
                   HeaderFontWeight="Bold">
        <Panel Height="40">
            <atom:TextBlock HorizontalAlignment="Center" VerticalAlignment="Center">
                Content of group box
            </atom:TextBlock>
        </Panel>
    </atom:GroupBox>

    <!-- 倾斜标题 -->
    <atom:GroupBox HeaderTitle="Title Info"
                   HeaderTitlePosition="Right"
                   HeaderFontStyle="Oblique">
        <Panel Height="40">
            <atom:TextBlock HorizontalAlignment="Center" VerticalAlignment="Center">
                Content of group box
            </atom:TextBlock>
        </Panel>
    </atom:GroupBox>

    <!-- 自定义颜色 + 字重 + 字体风格组合 -->
    <atom:GroupBox HeaderTitle="Title Info"
                   HeaderTitlePosition="Center"
                   HeaderFontStyle="Oblique"
                   HeaderTitleColor="Coral"
                   HeaderFontWeight="Medium">
        <Panel Height="40">
            <atom:TextBlock HorizontalAlignment="Center" VerticalAlignment="Center">
                Content of group box
            </atom:TextBlock>
        </Panel>
    </atom:GroupBox>
</StackPanel>
```

---

## 4. 标题图标

通过 `HeaderIcon` 属性在标题文本左侧添加图标。图标使用 Ant Design 图标集提供：

```xml
<atom:GroupBox HeaderTitle="Title Info"
               HeaderIcon="{antdicons:AntDesignIconProvider Kind=GithubOutlined}">
    <Panel Height="100">
        <TextBlock HorizontalAlignment="Center" VerticalAlignment="Center">
            Content of group box
        </TextBlock>
    </Panel>
</atom:GroupBox>
```

图标可以与标题位置、字体样式自由组合：

```xml
<!-- 图标 + 居中 + 粗体 -->
<atom:GroupBox HeaderTitle="Settings"
               HeaderIcon="{antdicons:AntDesignIconProvider Kind=SettingOutlined}"
               HeaderTitlePosition="Center"
               HeaderFontWeight="Bold">
    <StackPanel Spacing="8" Margin="8">
        <atom:TextBlock>Option 1</atom:TextBlock>
        <atom:TextBlock>Option 2</atom:TextBlock>
    </StackPanel>
</atom:GroupBox>
```

---

## 5. 嵌套内容

GroupBox 作为 `ContentControl`，可以容纳任意复杂的子控件树：

### 嵌套表单控件

```xml
<atom:GroupBox HeaderTitle="个人信息">
    <Grid ColumnDefinitions="Auto,*" RowDefinitions="Auto,Auto,Auto" Margin="8">
        <atom:TextBlock Grid.Row="0" Grid.Column="0" VerticalAlignment="Center"
                        Margin="0,0,8,8">姓名：</atom:TextBlock>
        <atom:LineEdit Grid.Row="0" Grid.Column="1" Margin="0,0,0,8" />

        <atom:TextBlock Grid.Row="1" Grid.Column="0" VerticalAlignment="Center"
                        Margin="0,0,8,8">邮箱：</atom:TextBlock>
        <atom:LineEdit Grid.Row="1" Grid.Column="1" Margin="0,0,0,8" />

        <atom:TextBlock Grid.Row="2" Grid.Column="0" VerticalAlignment="Center"
                        Margin="0,0,8,0">备注：</atom:TextBlock>
        <atom:LineEdit Grid.Row="2" Grid.Column="1" />
    </Grid>
</atom:GroupBox>
```

### 嵌套其他 GroupBox

```xml
<atom:GroupBox HeaderTitle="系统设置">
    <StackPanel Spacing="10" Margin="8">
        <atom:GroupBox HeaderTitle="显示设置"
                       HeaderIcon="{antdicons:AntDesignIconProvider Kind=DesktopOutlined}">
            <StackPanel Margin="8">
                <atom:TextBlock>分辨率：1920 × 1080</atom:TextBlock>
            </StackPanel>
        </atom:GroupBox>

        <atom:GroupBox HeaderTitle="网络设置"
                       HeaderIcon="{antdicons:AntDesignIconProvider Kind=WifiOutlined}">
            <StackPanel Margin="8">
                <atom:TextBlock>状态：已连接</atom:TextBlock>
            </StackPanel>
        </atom:GroupBox>
    </StackPanel>
</atom:GroupBox>
```

---

## 6. C# 代码操作

### 动态创建 GroupBox

```csharp
using AtomUI.Desktop.Controls;
using Avalonia.Controls;
using Avalonia.Media;

var groupBox = new GroupBox
{
    HeaderTitle = "动态创建的分组",
    HeaderTitlePosition = GroupBoxTitlePosition.Center,
    HeaderFontWeight = FontWeight.Bold,
    Content = new TextBlock { Text = "这是动态创建的内容" }
};

parentPanel.Children.Add(groupBox);
```

### 动态修改属性

```csharp
// 修改标题位置
groupBox.HeaderTitlePosition = GroupBoxTitlePosition.Right;

// 修改标题颜色
groupBox.HeaderTitleColor = new SolidColorBrush(Colors.Coral);

// 修改标题文本
groupBox.HeaderTitle = "新标题";

// 修改字体样式
groupBox.HeaderFontStyle = FontStyle.Italic;
groupBox.HeaderFontWeight = FontWeight.SemiBold;
```

### 数据绑定

```xml
<atom:GroupBox HeaderTitle="{Binding GroupTitle}"
               HeaderTitlePosition="{Binding TitlePosition}"
               HeaderTitleColor="{Binding TitleBrush}">
    <ContentControl Content="{Binding GroupContent}" />
</atom:GroupBox>
```

---

## 常见组合模式

### 表单分组

```xml
<StackPanel Spacing="16">
    <atom:GroupBox HeaderTitle="基本信息"
                   HeaderIcon="{antdicons:AntDesignIconProvider Kind=UserOutlined}">
        <StackPanel Spacing="8" Margin="8">
            <!-- 表单字段 -->
        </StackPanel>
    </atom:GroupBox>

    <atom:GroupBox HeaderTitle="联系方式"
                   HeaderIcon="{antdicons:AntDesignIconProvider Kind=PhoneOutlined}">
        <StackPanel Spacing="8" Margin="8">
            <!-- 表单字段 -->
        </StackPanel>
    </atom:GroupBox>
</StackPanel>
```

### 配置面板

```xml
<atom:GroupBox HeaderTitle="高级设置"
               HeaderTitlePosition="Center"
               HeaderFontWeight="Bold">
    <Grid ColumnDefinitions="*,*" RowDefinitions="Auto,Auto" Margin="8">
        <!-- 配置项 -->
    </Grid>
</atom:GroupBox>
```

### 禁用状态

```xml
<atom:GroupBox HeaderTitle="已锁定的设置" IsEnabled="False">
    <StackPanel Margin="8">
        <atom:TextBlock>这些设置当前不可修改</atom:TextBlock>
    </StackPanel>
</atom:GroupBox>
```
