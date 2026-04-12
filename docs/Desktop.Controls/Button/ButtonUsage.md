# Button 使用文档

本文档介绍 AtomUI Button 控件的各种使用方式，示例代码摘自 Gallery 演示程序。

> 📖 Gallery 源码位置：`controlgallery/AtomUIGallery/ShowCases/Views/General/ButtonShowCase.axaml`

---

## 前置准备

在 AXAML 中使用 Button，需要引入以下命名空间：

```xml
xmlns:atom="https://atomui.net"
xmlns:antdicons="https://atomui.net/icons/antdesign"
```

在 C# 代码中引入：

```csharp
using AtomUI.Desktop.Controls;   // Button 控件
using AtomUI.Controls;            // SizeType 等共享类型
```

---

## 1. 按钮类型

AtomUI 提供五种按钮类型，通过 `ButtonType` 属性设置。不设置时默认为 `Default`。

```xml
<WrapPanel Orientation="Horizontal">
    <atom:Button ButtonType="Primary">Primary Button</atom:Button>
    <atom:Button>Default Button</atom:Button>
    <atom:Button ButtonType="Dashed">Dashed</atom:Button>
    <atom:Button ButtonType="Text">Text Button</atom:Button>
    <atom:Button ButtonType="Link">Link Button</atom:Button>
</WrapPanel>
```

**使用场景指引**：
- 一个操作区域只放一个 **Primary** 按钮，引导用户注意力
- 多个并列操作使用 **Default** 按钮
- 添加类操作使用 **Dashed** 按钮
- 内联次级操作使用 **Text** 按钮
- 导航跳转使用 **Link** 按钮

---

## 2. 按钮形状

通过 `Shape` 属性设置按钮形状，支持默认矩形、圆形和胶囊形。

```xml
<!-- 默认矩形（带圆角） -->
<atom:Button ButtonType="Primary">Primary</atom:Button>
<atom:Button>Default</atom:Button>

<!-- 胶囊形（Round） -->
<atom:Button ButtonType="Primary" Shape="Round">Primary</atom:Button>
<atom:Button Shape="Round">Default</atom:Button>

<!-- 圆形（Circle）— 通常只放图标或极短文字 -->
<atom:Button ButtonType="Primary" Shape="Circle">AA</atom:Button>
<atom:Button Shape="Circle">AA</atom:Button>
```

**提示**：圆形按钮会强制 `宽度 = 高度`，最适合只放一个图标的场景。

---

## 3. 按钮尺寸

通过 `SizeType` 属性设置尺寸，支持 `Large`、`Middle`（默认）、`Small` 三种。

```xml
<atom:Button ButtonType="Primary" SizeType="Large">Large</atom:Button>
<atom:Button ButtonType="Primary" SizeType="Middle">Default</atom:Button>
<atom:Button ButtonType="Primary" SizeType="Small">Small</atom:Button>
```

尺寸也支持数据绑定，可在运行时动态切换：

```xml
<!-- AXAML：绑定 ViewModel 的 ButtonSizeType 属性 -->
<atom:Button ButtonType="Primary" SizeType="{Binding ButtonSizeType}">Primary</atom:Button>
<atom:Button ButtonType="Dashed" SizeType="{Binding ButtonSizeType}">Dashed</atom:Button>
<atom:Button ButtonType="Default" SizeType="{Binding ButtonSizeType}">Default</atom:Button>
<atom:Button ButtonType="Link" SizeType="{Binding ButtonSizeType}">Link</atom:Button>
```

```csharp
// ViewModel
public class ButtonViewModel : ReactiveObject
{
    private SizeType _buttonSizeType;
    public SizeType ButtonSizeType
    {
        get => _buttonSizeType;
        set => this.RaiseAndSetIfChanged(ref _buttonSizeType, value);
    }
}
```

尺寸与形状和图标组合使用：

```xml
<!-- 不同尺寸的纯图标按钮 -->
<atom:Button ButtonType="Primary" Shape="Circle"
             Icon="{antdicons:AntDesignIconProvider Kind=DownloadOutlined}"
             SizeType="Large" />
<atom:Button ButtonType="Primary" Shape="Circle"
             Icon="{antdicons:AntDesignIconProvider Kind=DownloadOutlined}"
             SizeType="Middle" />
<atom:Button ButtonType="Primary" Shape="Circle"
             Icon="{antdicons:AntDesignIconProvider Kind=DownloadOutlined}"
             SizeType="Small" />

<!-- 不同尺寸的图标 + 文字按钮 -->
<atom:Button ButtonType="Primary" Shape="Round"
             Icon="{antdicons:AntDesignIconProvider Kind=DownloadOutlined}"
             SizeType="Large">
    Download
</atom:Button>
```

---

## 4. 图标按钮

通过 `Icon` 属性设置按钮图标，图标使用 Ant Design 图标集提供：

```xml
<!-- 纯图标按钮（圆形） -->
<atom:Button ButtonType="Primary" Shape="Circle"
             Icon="{antdicons:AntDesignIconProvider Kind=SearchOutlined}" />

<!-- 图标 + 文字按钮 -->
<atom:Button ButtonType="Primary" Shape="Round"
             Icon="{antdicons:AntDesignIconProvider Kind=SearchOutlined}">
    Search
</atom:Button>

<!-- 不同类型均支持图标 -->
<atom:Button ButtonType="Default" Shape="Circle"
             Icon="{antdicons:AntDesignIconProvider Kind=SearchOutlined}" />
<atom:Button ButtonType="Default" Shape="Round"
             Icon="{antdicons:AntDesignIconProvider Kind=SearchOutlined}">
    Search
</atom:Button>
<atom:Button ButtonType="Text"
             Icon="{antdicons:AntDesignIconProvider Kind=SearchOutlined}">
    Search
</atom:Button>
<atom:Button ButtonType="Link"
             Icon="{antdicons:AntDesignIconProvider Kind=SearchOutlined}">
    Search
</atom:Button>
<atom:Button ButtonType="Dashed" Shape="Circle"
             Icon="{antdicons:AntDesignIconProvider Kind=SearchOutlined}" />
```

**图标 + 危险样式组合**：

```xml
<atom:Button ButtonType="Primary" IsDanger="True"
             Icon="{antdicons:AntDesignIconProvider Kind=SearchOutlined}">
    Search
</atom:Button>
<atom:Button ButtonType="Default" Shape="Round" IsDanger="True"
             Icon="{antdicons:AntDesignIconProvider Kind=SearchOutlined}">
    Search
</atom:Button>
<atom:Button ButtonType="Dashed" Shape="Circle" IsDanger="True"
             Icon="{antdicons:AntDesignIconProvider Kind=SearchOutlined}" />
```

---

## 5. 加载中状态

通过设置 `IsLoading` 属性控制按钮加载状态。加载中时按钮显示旋转图标，不透明度降低。

### 静态加载状态

```xml
<atom:Button ButtonType="Primary" IsLoading="True">Loading</atom:Button>
<atom:Button ButtonType="Primary" SizeType="Small" IsLoading="True">Loading</atom:Button>

<!-- 纯图标按钮的加载态 -->
<atom:Button ButtonType="Primary" IsLoading="True"
             Icon="{antdicons:AntDesignIconProvider Kind=PoweroffOutlined}" />
```

### 点击后动态进入加载态

这是最常见的使用场景——点击按钮发起异步请求，完成后恢复：

```xml
<atom:Button ButtonType="Primary"
             Click="HandleLoadingBtnClick">
    Click me!
</atom:Button>

<atom:Button ButtonType="Primary"
             Click="HandleLoadingBtnClick"
             Icon="{antdicons:AntDesignIconProvider Kind=PoweroffOutlined}">
    Click me!
</atom:Button>

<!-- 纯图标按钮也支持 -->
<atom:Button ButtonType="Primary"
             Click="HandleLoadingBtnClick"
             Icon="{antdicons:AntDesignIconProvider Kind=PoweroffOutlined}" />
```

```csharp
// Code-behind
public void HandleLoadingBtnClick(object? sender, RoutedEventArgs args)
{
    if (sender is Button button)
    {
        button.IsLoading = true;
        Dispatcher.UIThread.InvokeAsync(async () =>
        {
            await Task.Delay(TimeSpan.FromSeconds(3)); // 模拟异步操作
            button.IsLoading = false;
        });
    }
}
```

---

## 6. 危险按钮

通过 `IsDanger="True"` 标记危险操作，所有按钮类型均支持：

```xml
<atom:Button ButtonType="Primary" IsDanger="True">Primary</atom:Button>
<atom:Button ButtonType="Default" IsDanger="True">Default</atom:Button>
<atom:Button ButtonType="Dashed" IsDanger="True">Dashed</atom:Button>
<atom:Button ButtonType="Text" IsDanger="True">Text</atom:Button>
<atom:Button ButtonType="Link" IsDanger="True">Link</atom:Button>
```

**使用场景**：删除、移动、修改权限等不可逆操作，通常需配合二次确认弹窗。

---

## 7. 幽灵按钮

通过 `IsGhost="True"` 使按钮背景透明，适合放置在有色背景上：

```xml
<Border Background="rgb(190, 200, 200)" Padding="10">
    <StackPanel Orientation="Vertical">
        <!-- 纯文字幽灵按钮 -->
        <WrapPanel>
            <atom:Button ButtonType="Primary" IsGhost="True">Primary</atom:Button>
            <atom:Button ButtonType="Default" IsGhost="True">Default</atom:Button>
            <atom:Button ButtonType="Dashed" IsGhost="True">Dashed</atom:Button>
            <atom:Button ButtonType="Text" IsGhost="True">Text</atom:Button>
            <atom:Button ButtonType="Link" IsGhost="True">Link</atom:Button>
            <atom:Button ButtonType="Primary" IsDanger="True" IsGhost="True">
                Danger
            </atom:Button>
        </WrapPanel>

        <!-- 带图标的幽灵按钮 -->
        <WrapPanel>
            <atom:Button ButtonType="Primary" IsGhost="True"
                         Icon="{antdicons:AntDesignIconProvider Kind=SearchOutlined}">
                Primary
            </atom:Button>
            <atom:Button ButtonType="Default" IsGhost="True"
                         Icon="{antdicons:AntDesignIconProvider Kind=SearchOutlined}">
                Default
            </atom:Button>
            <atom:Button ButtonType="Primary" IsDanger="True" IsGhost="True"
                         Icon="{antdicons:AntDesignIconProvider Kind=SearchOutlined}">
                Danger
            </atom:Button>
        </WrapPanel>
    </StackPanel>
</Border>
```

---

## 8. Block 按钮

Ant Design 的 `block` 属性在 AtomUI 中通过 Avalonia 原生布局属性 `HorizontalAlignment="Stretch"` 实现，使按钮撑满父容器宽度：

```xml
<StackPanel Orientation="Vertical" Margin="10">
    <atom:Button ButtonType="Primary" HorizontalAlignment="Stretch">
        Primary
    </atom:Button>
    <atom:Button ButtonType="Default" HorizontalAlignment="Stretch">
        Default
    </atom:Button>
    <atom:Button ButtonType="Dashed" HorizontalAlignment="Stretch">
        Dashed
    </atom:Button>
    <atom:Button ButtonType="Text" HorizontalAlignment="Stretch">
        Text
    </atom:Button>
    <atom:Button ButtonType="Link" HorizontalAlignment="Stretch">
        Link
    </atom:Button>
</StackPanel>
```

---

## 9. 禁用状态

通过 `IsEnabled="False"` 禁用按钮，所有类型均支持：

```xml
<!-- 正常 vs 禁用对比 -->
<StackPanel Orientation="Horizontal">
    <atom:Button ButtonType="Primary"
                 Icon="{antdicons:AntDesignIconProvider Kind=SearchOutlined}">
        Primary
    </atom:Button>
    <atom:Button ButtonType="Primary" IsEnabled="False"
                 Icon="{antdicons:AntDesignIconProvider Kind=SearchOutlined}">
        Primary(disabled)
    </atom:Button>
</StackPanel>

<StackPanel Orientation="Horizontal">
    <atom:Button ButtonType="Default"
                 Icon="{antdicons:AntDesignIconProvider Kind=SearchOutlined}">
        Default
    </atom:Button>
    <atom:Button ButtonType="Default" IsEnabled="False"
                 Icon="{antdicons:AntDesignIconProvider Kind=SearchOutlined}">
        Default(disabled)
    </atom:Button>
</StackPanel>

<!-- 危险按钮的禁用 -->
<StackPanel Orientation="Horizontal">
    <atom:Button ButtonType="Primary" IsDanger="True"
                 Icon="{antdicons:AntDesignIconProvider Kind=SearchOutlined}">
        Danger Primary
    </atom:Button>
    <atom:Button ButtonType="Primary" IsDanger="True" IsEnabled="False"
                 Icon="{antdicons:AntDesignIconProvider Kind=SearchOutlined}">
        Danger Primary(disabled)
    </atom:Button>
</StackPanel>

<!-- 幽灵按钮的禁用 -->
<Border Background="rgb(190, 200, 200)">
    <StackPanel Orientation="Horizontal">
        <atom:Button IsGhost="True"
                     Icon="{antdicons:AntDesignIconProvider Kind=SearchOutlined}">
            Ghost
        </atom:Button>
        <atom:Button IsGhost="True" IsEnabled="False"
                     Icon="{antdicons:AntDesignIconProvider Kind=SearchOutlined}">
            Ghost(disabled)
        </atom:Button>
    </StackPanel>
</Border>
```

---

## 10. MVVM 命令绑定

Button 继承自 Avalonia 的 `Button`，完整支持 `Command` / `CommandParameter` 绑定模式：

```xml
<atom:Button ButtonType="Primary"
             Command="{Binding SaveCommand}">
    Save
</atom:Button>

<atom:Button ButtonType="Primary" IsDanger="True"
             Command="{Binding DeleteCommand}"
             CommandParameter="{Binding SelectedItem}">
    Delete
</atom:Button>
```

```csharp
// ViewModel（使用 ReactiveUI）
[Reactive]
public ReactiveCommand<Unit, Unit> SaveCommand { get; }

public MyViewModel()
{
    var canSave = this.WhenAnyValue(x => x.IsValid);
    SaveCommand = ReactiveCommand.CreateFromTask(
        async () => { await _repository.SaveAsync(); },
        canSave);
}
```

当 `CanExecute` 返回 `false` 时，按钮会**自动禁用**（灰色调 + 不可点击），无需手动管理 `IsEnabled`。

---

## 11. 键盘快捷键

利用继承自 Avalonia Button 的 `HotKey`、`IsDefault`、`IsCancel` 属性：

```xml
<!-- Enter 键触发确认 -->
<atom:Button ButtonType="Primary" IsDefault="True"
             Command="{Binding ConfirmCommand}">
    OK
</atom:Button>

<!-- Escape 键触发取消 -->
<atom:Button IsCancel="True"
             Command="{Binding CancelCommand}">
    Cancel
</atom:Button>

<!-- 自定义快捷键 -->
<atom:Button ButtonType="Primary"
             HotKey="Ctrl+S"
             Command="{Binding SaveCommand}">
    Save (Ctrl+S)
</atom:Button>
```

---

## 12. 控制动画行为

```xml
<!-- 禁用过渡动画（背景色、前景色不再渐变过渡） -->
<atom:Button ButtonType="Primary" IsMotionEnabled="False">
    No Animation
</atom:Button>

<!-- 禁用点击波纹效果 -->
<atom:Button ButtonType="Primary" IsWaveSpiritEnabled="False">
    No Wave
</atom:Button>
```

---

## 常见组合模式

### 操作栏（主 + 次按钮）

```xml
<StackPanel Orientation="Horizontal" Spacing="8">
    <atom:Button ButtonType="Primary">Submit</atom:Button>
    <atom:Button>Cancel</atom:Button>
</StackPanel>
```

### 确认/取消对话框

```xml
<StackPanel Orientation="Horizontal" Spacing="8"
            HorizontalAlignment="Right">
    <atom:Button IsCancel="True">Cancel</atom:Button>
    <atom:Button ButtonType="Primary" IsDefault="True">OK</atom:Button>
</StackPanel>
```

### 危险操作确认

```xml
<StackPanel Orientation="Horizontal" Spacing="8">
    <atom:Button>Cancel</atom:Button>
    <atom:Button ButtonType="Primary" IsDanger="True"
                 Icon="{antdicons:AntDesignIconProvider Kind=DeleteOutlined}">
        Delete
    </atom:Button>
</StackPanel>
```

### 工具栏（纯图标按钮组）

```xml
<StackPanel Orientation="Horizontal" Spacing="4">
    <atom:Button ButtonType="Text"
                 Icon="{antdicons:AntDesignIconProvider Kind=BoldOutlined}" />
    <atom:Button ButtonType="Text"
                 Icon="{antdicons:AntDesignIconProvider Kind=ItalicOutlined}" />
    <atom:Button ButtonType="Text"
                 Icon="{antdicons:AntDesignIconProvider Kind=UnderlineOutlined}" />
</StackPanel>
```

