# SplitButton 使用文档

本文档介绍 AtomUI SplitButton 控件的各种使用方式，示例代码摘自 Gallery 演示程序。

> 📖 Gallery 源码位置：`controlgallery/AtomUIGallery/ShowCases/Views/General/SplitButtonShowCase.axaml`

---

## 前置准备

在 AXAML 中使用 SplitButton，需要引入以下命名空间：

```xml
xmlns:atom="https://atomui.net"
xmlns:antdicons="https://atomui.net/icons/antdesign"
```

在 C# 代码中引入：

```csharp
using AtomUI.Desktop.Controls;   // SplitButton 控件
using AtomUI.Controls;            // SizeType 等共享类型
```

---

## 1. 基本用法

最基本的组合按钮，左侧主按钮执行默认操作，右侧按钮触发下拉菜单：

```xml
<atom:SplitButton TriggerType="Hover">
    Hover me
    <atom:SplitButton.Flyout>
        <atom:MenuFlyout>
            <atom:MenuItem Header="Cut" InputGesture="Ctrl+X"
                           Icon="{antdicons:AntDesignIconProvider Kind=ScissorOutlined}" />
            <atom:MenuItem Header="Copy" InputGesture="Ctrl+C"
                           Icon="{antdicons:AntDesignIconProvider Kind=CopyOutlined}" />
            <atom:MenuItem Header="Delete" InputGesture="Ctrl+D"
                           Icon="{antdicons:AntDesignIconProvider Kind=DeleteOutlined}" />
        </atom:MenuFlyout>
    </atom:SplitButton.Flyout>
</atom:SplitButton>
```

**要点**：
- `Flyout` 属性接受 `MenuFlyout`，其中包含 `MenuItem` 菜单项
- 左侧主按钮点击触发 `Click` 事件，右侧按钮触发 Flyout 弹出
- 默认下拉按钮图标为 `EllipsisOutlined`（省略号）
- 默认使用 `Default` 样式（白色背景 + 边框）

---

## 2. 按钮类型

SplitButton 支持两种按钮类型，通过 `IsPrimaryButtonType` 属性切换：

```xml
<!-- Default 样式（默认） -->
<atom:SplitButton>
    Default
    <atom:SplitButton.Flyout>
        <atom:MenuFlyout>
            <atom:MenuItem Header="Cut" InputGesture="Ctrl+X" />
            <atom:MenuItem Header="Copy" InputGesture="Ctrl+C" />
        </atom:MenuFlyout>
    </atom:SplitButton.Flyout>
</atom:SplitButton>

<!-- Primary 样式 -->
<atom:SplitButton IsPrimaryButtonType="True">
    Primary
    <atom:SplitButton.Flyout>
        <atom:MenuFlyout>
            <atom:MenuItem Header="Cut" InputGesture="Ctrl+X" />
            <atom:MenuItem Header="Copy" InputGesture="Ctrl+C" />
        </atom:MenuFlyout>
    </atom:SplitButton.Flyout>
</atom:SplitButton>
```

**使用场景指引**：
- **Default**：常规场景，并列操作区域中使用
- **Primary**：需要强调主操作的场景，如「提交」+ 下拉更多提交选项

---

## 3. 按钮尺寸

通过 `SizeType` 属性设置尺寸，支持 `Large`、`Middle`（默认）、`Small` 三种：

```xml
<StackPanel Orientation="Vertical" Spacing="10">
    <!-- Default 样式的三种尺寸 -->
    <WrapPanel>
        <atom:SplitButton SizeType="Large">
            Large
            <atom:SplitButton.Flyout>
                <atom:MenuFlyout>
                    <atom:MenuItem Header="Cut" InputGesture="Ctrl+X"
                                   Icon="{antdicons:AntDesignIconProvider Kind=ScissorOutlined}" />
                    <atom:MenuItem Header="Copy" InputGesture="Ctrl+C"
                                   Icon="{antdicons:AntDesignIconProvider Kind=CopyOutlined}" />
                    <atom:MenuItem Header="Delete" InputGesture="Ctrl+D"
                                   Icon="{antdicons:AntDesignIconProvider Kind=DeleteOutlined}" />
                </atom:MenuFlyout>
            </atom:SplitButton.Flyout>
        </atom:SplitButton>
        <atom:SplitButton SizeType="Middle">
            Middle
            <atom:SplitButton.Flyout>
                <atom:MenuFlyout>
                    <atom:MenuItem Header="Cut" />
                    <atom:MenuItem Header="Copy" />
                </atom:MenuFlyout>
            </atom:SplitButton.Flyout>
        </atom:SplitButton>
        <atom:SplitButton SizeType="Small">
            Small
            <atom:SplitButton.Flyout>
                <atom:MenuFlyout>
                    <atom:MenuItem Header="Cut" />
                    <atom:MenuItem Header="Copy" />
                </atom:MenuFlyout>
            </atom:SplitButton.Flyout>
        </atom:SplitButton>
    </WrapPanel>

    <!-- Primary 样式的三种尺寸 -->
    <WrapPanel>
        <atom:SplitButton SizeType="Large" IsPrimaryButtonType="True">
            Large
            <atom:SplitButton.Flyout>
                <atom:MenuFlyout>
                    <atom:MenuItem Header="Cut" />
                    <atom:MenuItem Header="Copy" />
                </atom:MenuFlyout>
            </atom:SplitButton.Flyout>
        </atom:SplitButton>
        <atom:SplitButton SizeType="Middle" IsPrimaryButtonType="True">
            Middle
            <atom:SplitButton.Flyout>
                <atom:MenuFlyout>
                    <atom:MenuItem Header="Cut" />
                    <atom:MenuItem Header="Copy" />
                </atom:MenuFlyout>
            </atom:SplitButton.Flyout>
        </atom:SplitButton>
        <atom:SplitButton SizeType="Small" IsPrimaryButtonType="True">
            Small
            <atom:SplitButton.Flyout>
                <atom:MenuFlyout>
                    <atom:MenuItem Header="Cut" />
                    <atom:MenuItem Header="Copy" />
                </atom:MenuFlyout>
            </atom:SplitButton.Flyout>
        </atom:SplitButton>
    </WrapPanel>
</StackPanel>
```

> 📖 参考：`controlgallery/AtomUIGallery/ShowCases/Views/General/SplitButtonShowCase.axaml` 中 "Size" 示例。

---

## 4. 危险按钮

通过 `IsDanger="True"` 标记危险操作，Default 和 Primary 类型均支持：

```xml
<WrapPanel Orientation="Horizontal">
    <atom:SplitButton IsDanger="True">
        Default
        <atom:SplitButton.Flyout>
            <atom:MenuFlyout>
                <atom:MenuItem Header="Cut" InputGesture="Ctrl+X"
                               Icon="{antdicons:AntDesignIconProvider Kind=ScissorOutlined}" />
                <atom:MenuItem Header="Copy" InputGesture="Ctrl+C"
                               Icon="{antdicons:AntDesignIconProvider Kind=CopyOutlined}" />
                <atom:MenuItem Header="Delete" InputGesture="Ctrl+D"
                               Icon="{antdicons:AntDesignIconProvider Kind=DeleteOutlined}" />
            </atom:MenuFlyout>
        </atom:SplitButton.Flyout>
    </atom:SplitButton>

    <atom:SplitButton IsDanger="True" IsPrimaryButtonType="True">
        Primary
        <atom:SplitButton.Flyout>
            <atom:MenuFlyout>
                <atom:MenuItem Header="Cut" InputGesture="Ctrl+X" />
                <atom:MenuItem Header="Copy" InputGesture="Ctrl+C" />
                <atom:MenuItem Header="Delete" InputGesture="Ctrl+D" />
            </atom:MenuFlyout>
        </atom:SplitButton.Flyout>
    </atom:SplitButton>
</WrapPanel>
```

**使用场景**：删除、移动、修改权限等不可逆操作。

> 📖 参考：`controlgallery/AtomUIGallery/ShowCases/Views/General/SplitButtonShowCase.axaml` 中 "Danger Buttons" 示例。

---

## 5. 自定义下拉图标

通过 `OpenIndicator` 属性自定义右侧下拉按钮的图标（默认为 `EllipsisOutlined`）：

```xml
<WrapPanel Orientation="Horizontal">
    <!-- 默认图标（EllipsisOutlined） -->
    <atom:SplitButton>
        Default
        <atom:SplitButton.Flyout>
            <atom:MenuFlyout>
                <atom:MenuItem Header="Cut" />
                <atom:MenuItem Header="Copy" />
            </atom:MenuFlyout>
        </atom:SplitButton.Flyout>
    </atom:SplitButton>

    <!-- 自定义图标 -->
    <atom:SplitButton OpenIndicator="{antdicons:AntDesignIconProvider Kind=UserOutlined}">
        Custom Icon
        <atom:SplitButton.Flyout>
            <atom:MenuFlyout>
                <atom:MenuItem Header="Cut" />
                <atom:MenuItem Header="Copy" />
            </atom:MenuFlyout>
        </atom:SplitButton.Flyout>
    </atom:SplitButton>
</WrapPanel>
```

> 📖 参考：`controlgallery/AtomUIGallery/ShowCases/Views/General/SplitButtonShowCase.axaml` 中 "Custom Icon" 示例。

---

## 6. 主按钮带图标

通过 `Icon` 属性为左侧主按钮添加图标：

```xml
<atom:SplitButton Icon="{antdicons:AntDesignIconProvider Kind=DownloadOutlined}">
    Download
    <atom:SplitButton.Flyout>
        <atom:MenuFlyout>
            <atom:MenuItem Header="Download as PDF" />
            <atom:MenuItem Header="Download as Word" />
            <atom:MenuItem Header="Download as Excel" />
        </atom:MenuFlyout>
    </atom:SplitButton.Flyout>
</atom:SplitButton>
```

---

## 7. 触发方式

支持 `Click`（默认）和 `Hover` 两种触发方式：

```xml
<WrapPanel Orientation="Horizontal">
    <atom:SplitButton TriggerType="Hover">
        Hover Me
        <atom:SplitButton.Flyout>
            <atom:MenuFlyout>
                <atom:MenuItem Header="Cut" InputGesture="Ctrl+X"
                               Icon="{antdicons:AntDesignIconProvider Kind=ScissorOutlined}" />
                <atom:MenuItem Header="Copy" InputGesture="Ctrl+C"
                               Icon="{antdicons:AntDesignIconProvider Kind=CopyOutlined}" />
                <atom:MenuItem Header="Delete" InputGesture="Ctrl+D"
                               Icon="{antdicons:AntDesignIconProvider Kind=DeleteOutlined}" />
            </atom:MenuFlyout>
        </atom:SplitButton.Flyout>
    </atom:SplitButton>

    <atom:SplitButton TriggerType="Click">
        Click Me
        <atom:SplitButton.Flyout>
            <atom:MenuFlyout>
                <atom:MenuItem Header="Cut" InputGesture="Ctrl+X"
                               Icon="{antdicons:AntDesignIconProvider Kind=ScissorOutlined}" />
                <atom:MenuItem Header="Copy" InputGesture="Ctrl+C"
                               Icon="{antdicons:AntDesignIconProvider Kind=CopyOutlined}" />
                <atom:MenuItem Header="Delete" InputGesture="Ctrl+D"
                               Icon="{antdicons:AntDesignIconProvider Kind=DeleteOutlined}" />
            </atom:MenuFlyout>
        </atom:SplitButton.Flyout>
    </atom:SplitButton>
</WrapPanel>
```

**触发方式说明**：
- **Click**：点击右侧下拉按钮弹出菜单（桌面端推荐，操作明确）
- **Hover**：鼠标悬浮在右侧按钮上时弹出菜单（适合预览场景，减少点击次数）

使用 `Hover` 模式时，可通过 `MouseEnterDelay` 和 `MouseLeaveDelay` 控制延迟：

```xml
<atom:SplitButton TriggerType="Hover"
                   MouseEnterDelay="300"
                   MouseLeaveDelay="200">
    Custom Delay
    <atom:SplitButton.Flyout>
        <atom:MenuFlyout>
            <atom:MenuItem Header="Option 1" />
            <atom:MenuItem Header="Option 2" />
        </atom:MenuFlyout>
    </atom:SplitButton.Flyout>
</atom:SplitButton>
```

> 📖 参考：`controlgallery/AtomUIGallery/ShowCases/Views/General/SplitButtonShowCase.axaml` 中 "Flyout trigger type" 示例。

---

## 8. 弹出位置和箭头

通过 `Placement` 控制弹出方向，`IsShowArrow` 控制箭头显示：

```xml
<!-- 下方右对齐（默认） -->
<atom:SplitButton Placement="BottomEdgeAlignedRight">
    Bottom Right
    <atom:SplitButton.Flyout>
        <atom:MenuFlyout>
            <atom:MenuItem Header="Option 1" />
            <atom:MenuItem Header="Option 2" />
        </atom:MenuFlyout>
    </atom:SplitButton.Flyout>
</atom:SplitButton>

<!-- 下方居中 + 显示箭头 -->
<atom:SplitButton Placement="Bottom"
                   IsShowArrow="True"
                   IsPointAtCenter="True">
    Bottom Center with Arrow
    <atom:SplitButton.Flyout>
        <atom:MenuFlyout>
            <atom:MenuItem Header="Option 1" />
            <atom:MenuItem Header="Option 2" />
        </atom:MenuFlyout>
    </atom:SplitButton.Flyout>
</atom:SplitButton>

<!-- 上方弹出 -->
<atom:SplitButton Placement="Top">
    Top
    <atom:SplitButton.Flyout>
        <atom:MenuFlyout>
            <atom:MenuItem Header="Option 1" />
            <atom:MenuItem Header="Option 2" />
        </atom:MenuFlyout>
    </atom:SplitButton.Flyout>
</atom:SplitButton>
```

---

## 9. 主按钮点击事件与 Command

### 事件方式

```xml
<atom:SplitButton Click="HandleSplitButtonClick">
    Submit
    <atom:SplitButton.Flyout>
        <atom:MenuFlyout>
            <atom:MenuItem Header="Save as Draft" />
            <atom:MenuItem Header="Export" />
        </atom:MenuFlyout>
    </atom:SplitButton.Flyout>
</atom:SplitButton>
```

```csharp
// Code-behind
private void HandleSplitButtonClick(object? sender, RoutedEventArgs e)
{
    // 处理主按钮点击
    Debug.WriteLine("SplitButton primary clicked!");
}
```

### Command 方式（MVVM）

```xml
<atom:SplitButton Command="{Binding SubmitCommand}" CommandParameter="draft">
    Submit
    <atom:SplitButton.Flyout>
        <atom:MenuFlyout>
            <atom:MenuItem Header="Save as Draft"
                           Command="{Binding SaveDraftCommand}" />
            <atom:MenuItem Header="Export as PDF"
                           Command="{Binding ExportCommand}"
                           CommandParameter="pdf" />
        </atom:MenuFlyout>
    </atom:SplitButton.Flyout>
</atom:SplitButton>
```

```csharp
// ViewModel（使用 ReactiveUI）
[Reactive]
public ReactiveCommand<string, Unit> SubmitCommand { get; }

public MyViewModel()
{
    var canSubmit = this.WhenAnyValue(x => x.IsValid);
    SubmitCommand = ReactiveCommand.CreateFromTask<string>(
        async (param) => { await _repository.SubmitAsync(param); },
        canSubmit);
}
```

当 `CanExecute` 返回 `false` 时，SplitButton 会**自动禁用**（灰色调 + 不可点击），无需手动管理 `IsEnabled`。

---

## 10. 键盘快捷键

利用 `HotKey` 属性设置键盘快捷键：

```xml
<!-- Ctrl+S 触发主按钮 -->
<atom:SplitButton HotKey="Ctrl+S"
                   Command="{Binding SaveCommand}">
    Save (Ctrl+S)
    <atom:SplitButton.Flyout>
        <atom:MenuFlyout>
            <atom:MenuItem Header="Save as Draft" />
            <atom:MenuItem Header="Save and Close" />
        </atom:MenuFlyout>
    </atom:SplitButton.Flyout>
</atom:SplitButton>
```

**内置键盘交互**（无需配置）：
- `Space` / `Enter`：触发主按钮点击
- `Alt+Down` / `F4`：打开 Flyout
- `Escape`：关闭 Flyout

---

## 11. 禁用状态

通过 `IsEnabled="False"` 禁用 SplitButton，两个内部按钮同步禁用：

```xml
<!-- 正常 vs 禁用对比 -->
<WrapPanel>
    <atom:SplitButton>
        Default
        <atom:SplitButton.Flyout>
            <atom:MenuFlyout>
                <atom:MenuItem Header="Cut" />
                <atom:MenuItem Header="Copy" />
            </atom:MenuFlyout>
        </atom:SplitButton.Flyout>
    </atom:SplitButton>
    <atom:SplitButton IsEnabled="False">
        Default (disabled)
        <atom:SplitButton.Flyout>
            <atom:MenuFlyout>
                <atom:MenuItem Header="Cut" />
                <atom:MenuItem Header="Copy" />
            </atom:MenuFlyout>
        </atom:SplitButton.Flyout>
    </atom:SplitButton>
</WrapPanel>

<!-- Primary 禁用 -->
<WrapPanel>
    <atom:SplitButton IsPrimaryButtonType="True">
        Primary
        <atom:SplitButton.Flyout>
            <atom:MenuFlyout>
                <atom:MenuItem Header="Cut" />
                <atom:MenuItem Header="Copy" />
            </atom:MenuFlyout>
        </atom:SplitButton.Flyout>
    </atom:SplitButton>
    <atom:SplitButton IsPrimaryButtonType="True" IsEnabled="False">
        Primary (disabled)
        <atom:SplitButton.Flyout>
            <atom:MenuFlyout>
                <atom:MenuItem Header="Cut" />
                <atom:MenuItem Header="Copy" />
            </atom:MenuFlyout>
        </atom:SplitButton.Flyout>
    </atom:SplitButton>
</WrapPanel>
```

---

## 12. 控制动画行为

```xml
<!-- 禁用过渡动画（背景色、前景色不再渐变过渡） -->
<atom:SplitButton IsMotionEnabled="False">
    No Animation
    <atom:SplitButton.Flyout>
        <atom:MenuFlyout>
            <atom:MenuItem Header="Option 1" />
        </atom:MenuFlyout>
    </atom:SplitButton.Flyout>
</atom:SplitButton>

<!-- 禁用点击波纹效果 -->
<atom:SplitButton IsWaveSpiritEnabled="False">
    No Wave
    <atom:SplitButton.Flyout>
        <atom:MenuFlyout>
            <atom:MenuItem Header="Option 1" />
        </atom:MenuFlyout>
    </atom:SplitButton.Flyout>
</atom:SplitButton>
```

---

## 常见组合模式

### 主操作 + 更多操作

最常见的场景——主按钮执行默认操作，下拉菜单提供替代方案：

```xml
<atom:SplitButton IsPrimaryButtonType="True"
                   Click="HandleSubmit">
    Submit
    <atom:SplitButton.Flyout>
        <atom:MenuFlyout>
            <atom:MenuItem Header="Save as Draft"
                           Icon="{antdicons:AntDesignIconProvider Kind=FileOutlined}" />
            <atom:MenuItem Header="Export as PDF"
                           Icon="{antdicons:AntDesignIconProvider Kind=FilePdfOutlined}" />
            <atom:MenuItem Header="Print"
                           Icon="{antdicons:AntDesignIconProvider Kind=PrinterOutlined}" />
        </atom:MenuFlyout>
    </atom:SplitButton.Flyout>
</atom:SplitButton>
```

### 下载按钮 + 格式选择

```xml
<atom:SplitButton Icon="{antdicons:AntDesignIconProvider Kind=DownloadOutlined}">
    Download
    <atom:SplitButton.Flyout>
        <atom:MenuFlyout>
            <atom:MenuItem Header="Download as PDF" />
            <atom:MenuItem Header="Download as Word" />
            <atom:MenuItem Header="Download as Excel" />
        </atom:MenuFlyout>
    </atom:SplitButton.Flyout>
</atom:SplitButton>
```

### 危险操作 + 确认选项

```xml
<atom:SplitButton IsDanger="True" IsPrimaryButtonType="True"
                   Icon="{antdicons:AntDesignIconProvider Kind=DeleteOutlined}">
    Delete
    <atom:SplitButton.Flyout>
        <atom:MenuFlyout>
            <atom:MenuItem Header="Delete Selected" />
            <atom:MenuItem Header="Delete All" />
            <atom:MenuItem Header="Move to Trash" />
        </atom:MenuFlyout>
    </atom:SplitButton.Flyout>
</atom:SplitButton>
```

### 操作栏组合（SplitButton + Button）

```xml
<StackPanel Orientation="Horizontal" Spacing="8">
    <atom:SplitButton IsPrimaryButtonType="True"
                       Command="{Binding SaveCommand}">
        Save
        <atom:SplitButton.Flyout>
            <atom:MenuFlyout>
                <atom:MenuItem Header="Save as Draft" />
                <atom:MenuItem Header="Save and Close" />
            </atom:MenuFlyout>
        </atom:SplitButton.Flyout>
    </atom:SplitButton>
    <atom:Button>Cancel</atom:Button>
</StackPanel>
```
