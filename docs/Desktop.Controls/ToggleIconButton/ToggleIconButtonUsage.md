# ToggleIconButton 使用文档

本文档介绍 AtomUI ToggleIconButton 控件的各种使用方式，涵盖常见交互场景的代码示例。

---

## 前置准备

在 AXAML 中使用 ToggleIconButton，需要引入以下命名空间：

```xml
xmlns:atom="https://atomui.net"
xmlns:antdicons="https://atomui.net/icons/antdesign"
```

在 C# 代码中引入：

```csharp
using AtomUI.Desktop.Controls;   // ToggleIconButton 控件
```

---

## 1. 基本用法

最基本的切换图标按钮，点击在两个图标之间切换：

```xml
<atom:ToggleIconButton
    CheckedIcon="{antdicons:AntDesignIconProvider Kind=EyeOutlined}"
    UnCheckedIcon="{antdicons:AntDesignIconProvider Kind=EyeInvisibleOutlined}" />
```

**要点**：
- `CheckedIcon` 在 `IsChecked=True` 时显示
- `UnCheckedIcon` 在 `IsChecked=False` 时显示
- 点击自动切换 `IsChecked` 状态

---

## 2. 收藏/取消收藏

经典的心形收藏按钮，选中时变为红色实心心形：

```xml
<atom:ToggleIconButton
    CheckedIcon="{antdicons:AntDesignIconProvider Kind=HeartFilled}"
    UnCheckedIcon="{antdicons:AntDesignIconProvider Kind=HeartOutlined}"
    SelectedIconBrush="Red"
    NormalIconBrush="Gray" />
```

**使用场景**：内容卡片右上角的收藏操作、评论列表中的点赞按钮等。

---

## 3. 星标/评分

星形收藏按钮，选中时变为金色实心星形：

```xml
<atom:ToggleIconButton
    CheckedIcon="{antdicons:AntDesignIconProvider Kind=StarFilled}"
    UnCheckedIcon="{antdicons:AntDesignIconProvider Kind=StarOutlined}"
    SelectedIconBrush="Gold"
    NormalIconBrush="Gray"
    IconWidth="20" IconHeight="20" />
```

**使用场景**：文件/邮件列表的标星操作。

---

## 4. 折叠/展开指示

使用方向箭头图标表达面板的折叠/展开状态：

```xml
<atom:ToggleIconButton
    CheckedIcon="{antdicons:AntDesignIconProvider Kind=UpOutlined}"
    UnCheckedIcon="{antdicons:AntDesignIconProvider Kind=DownOutlined}"
    IsChecked="{Binding IsPanelExpanded, Mode=TwoWay}" />
```

**使用场景**：Collapse 面板头部、侧边栏折叠控制、可展开区域的切换开关。

---

## 5. 显示/隐藏密码

在密码输入框旁放置切换按钮，控制密码的可见性：

```xml
<StackPanel Orientation="Horizontal" Spacing="4">
    <atom:LineEdit PasswordChar="•"
                   RevealPassword="{Binding IsPasswordVisible}" />
    <atom:ToggleIconButton
        CheckedIcon="{antdicons:AntDesignIconProvider Kind=EyeOutlined}"
        UnCheckedIcon="{antdicons:AntDesignIconProvider Kind=EyeInvisibleOutlined}"
        IsChecked="{Binding IsPasswordVisible, Mode=TwoWay}" />
</StackPanel>
```

---

## 6. 声音/静音切换

```xml
<atom:ToggleIconButton
    CheckedIcon="{antdicons:AntDesignIconProvider Kind=SoundFilled}"
    UnCheckedIcon="{antdicons:AntDesignIconProvider Kind=MutedOutlined}"
    IsChecked="{Binding IsSoundEnabled, Mode=TwoWay}" />
```

```csharp
// ViewModel（使用 ReactiveUI）
private bool _isSoundEnabled = true;
public bool IsSoundEnabled
{
    get => _isSoundEnabled;
    set => this.RaiseAndSetIfChanged(ref _isSoundEnabled, value);
}
```

---

## 7. 排序方向切换

表格列标题中的排序图标：

```xml
<atom:ToggleIconButton
    CheckedIcon="{antdicons:AntDesignIconProvider Kind=SortAscendingOutlined}"
    UnCheckedIcon="{antdicons:AntDesignIconProvider Kind=SortDescendingOutlined}"
    IsChecked="{Binding IsAscending, Mode=TwoWay}" />
```

---

## 8. 自定义图标尺寸

通过 `IconWidth` 和 `IconHeight` 精确控制图标渲染尺寸：

```xml
<!-- 小号图标（默认 IconSizeSM） -->
<atom:ToggleIconButton
    CheckedIcon="{antdicons:AntDesignIconProvider Kind=HeartFilled}"
    UnCheckedIcon="{antdicons:AntDesignIconProvider Kind=HeartOutlined}" />

<!-- 中号图标 -->
<atom:ToggleIconButton
    CheckedIcon="{antdicons:AntDesignIconProvider Kind=HeartFilled}"
    UnCheckedIcon="{antdicons:AntDesignIconProvider Kind=HeartOutlined}"
    IconWidth="16" IconHeight="16" />

<!-- 大号图标 -->
<atom:ToggleIconButton
    CheckedIcon="{antdicons:AntDesignIconProvider Kind=HeartFilled}"
    UnCheckedIcon="{antdicons:AntDesignIconProvider Kind=HeartOutlined}"
    IconWidth="24" IconHeight="24" />
```

---

## 9. 自定义各状态颜色

ToggleIconButton 提供四种独立的颜色属性，覆盖不同交互状态：

```xml
<atom:ToggleIconButton
    CheckedIcon="{antdicons:AntDesignIconProvider Kind=StarFilled}"
    UnCheckedIcon="{antdicons:AntDesignIconProvider Kind=StarOutlined}"
    NormalIconBrush="Gray"
    ActiveIconBrush="DarkGoldenrod"
    SelectedIconBrush="Gold"
    DisabledIconBrush="LightGray" />
```

也可以使用 Token 资源作为颜色值：

```xml
<atom:ToggleIconButton
    CheckedIcon="{antdicons:AntDesignIconProvider Kind=HeartFilled}"
    UnCheckedIcon="{antdicons:AntDesignIconProvider Kind=HeartOutlined}"
    SelectedIconBrush="{atom:SharedTokenResource ColorError}"
    NormalIconBrush="{atom:SharedTokenResource ColorIcon}" />
```

---

## 10. 禁用状态

通过 `IsEnabled="False"` 禁用按钮，禁用后图标变为灰色且不响应交互：

```xml
<!-- 正常 vs 禁用对比 -->
<StackPanel Orientation="Horizontal" Spacing="8">
    <atom:ToggleIconButton
        CheckedIcon="{antdicons:AntDesignIconProvider Kind=HeartFilled}"
        UnCheckedIcon="{antdicons:AntDesignIconProvider Kind=HeartOutlined}" />
    <atom:ToggleIconButton
        CheckedIcon="{antdicons:AntDesignIconProvider Kind=HeartFilled}"
        UnCheckedIcon="{antdicons:AntDesignIconProvider Kind=HeartOutlined}"
        IsEnabled="False" />
</StackPanel>

<!-- 选中态 + 禁用 -->
<atom:ToggleIconButton
    CheckedIcon="{antdicons:AntDesignIconProvider Kind=StarFilled}"
    UnCheckedIcon="{antdicons:AntDesignIconProvider Kind=StarOutlined}"
    IsChecked="True"
    IsEnabled="False"
    SelectedIconBrush="Gold"
    DisabledIconBrush="LightGray" />
```

---

## 11. MVVM 命令绑定

ToggleIconButton 继承自 Avalonia 的 `ToggleButton`，完整支持 `Command` / `CommandParameter` 绑定模式：

```xml
<atom:ToggleIconButton
    CheckedIcon="{antdicons:AntDesignIconProvider Kind=HeartFilled}"
    UnCheckedIcon="{antdicons:AntDesignIconProvider Kind=HeartOutlined}"
    Command="{Binding ToggleFavoriteCommand}"
    CommandParameter="{Binding CurrentItem}" />
```

```csharp
// ViewModel（使用 ReactiveUI）
public ReactiveCommand<object?, Unit> ToggleFavoriteCommand { get; }

public MyViewModel()
{
    ToggleFavoriteCommand = ReactiveCommand.Create<object?>(item =>
    {
        if (item is MyItem myItem)
        {
            myItem.IsFavorite = !myItem.IsFavorite;
        }
    });
}
```

当 `CanExecute` 返回 `false` 时，按钮会**自动禁用**（灰色调 + 不可点击），无需手动管理 `IsEnabled`。

---

## 12. 绑定 IsChecked 实现双向同步

使用 `TwoWay` 绑定将 `IsChecked` 与 ViewModel 属性同步：

```xml
<atom:ToggleIconButton
    CheckedIcon="{antdicons:AntDesignIconProvider Kind=PushpinFilled}"
    UnCheckedIcon="{antdicons:AntDesignIconProvider Kind=PushpinOutlined}"
    IsChecked="{Binding IsPinned, Mode=TwoWay}"
    SelectedIconBrush="{atom:SharedTokenResource ColorPrimary}" />
```

```csharp
// ViewModel
private bool _isPinned;
public bool IsPinned
{
    get => _isPinned;
    set => this.RaiseAndSetIfChanged(ref _isPinned, value);
}
```

---

## 13. 控制动画行为

```xml
<!-- 禁用过渡动画 -->
<atom:ToggleIconButton
    CheckedIcon="{antdicons:AntDesignIconProvider Kind=HeartFilled}"
    UnCheckedIcon="{antdicons:AntDesignIconProvider Kind=HeartOutlined}"
    IsMotionEnabled="False" />
```

---

## 常见组合模式

### 工具栏（格式化按钮组）

```xml
<StackPanel Orientation="Horizontal" Spacing="4">
    <atom:ToggleIconButton
        CheckedIcon="{antdicons:AntDesignIconProvider Kind=BoldOutlined}"
        UnCheckedIcon="{antdicons:AntDesignIconProvider Kind=BoldOutlined}"
        SelectedIconBrush="{atom:SharedTokenResource ColorPrimary}"
        IsChecked="{Binding IsBold, Mode=TwoWay}" />
    <atom:ToggleIconButton
        CheckedIcon="{antdicons:AntDesignIconProvider Kind=ItalicOutlined}"
        UnCheckedIcon="{antdicons:AntDesignIconProvider Kind=ItalicOutlined}"
        SelectedIconBrush="{atom:SharedTokenResource ColorPrimary}"
        IsChecked="{Binding IsItalic, Mode=TwoWay}" />
    <atom:ToggleIconButton
        CheckedIcon="{antdicons:AntDesignIconProvider Kind=UnderlineOutlined}"
        UnCheckedIcon="{antdicons:AntDesignIconProvider Kind=UnderlineOutlined}"
        SelectedIconBrush="{atom:SharedTokenResource ColorPrimary}"
        IsChecked="{Binding IsUnderline, Mode=TwoWay}" />
</StackPanel>
```

> 💡 **提示**：当选中和未选中使用同一图标时，通过颜色变化（`SelectedIconBrush`）来区分状态。

### 面板头部操作区

```xml
<DockPanel>
    <TextBlock Text="Details" DockPanel.Dock="Left" VerticalAlignment="Center" />
    <StackPanel Orientation="Horizontal" DockPanel.Dock="Right"
                HorizontalAlignment="Right" Spacing="4">
        <atom:ToggleIconButton
            CheckedIcon="{antdicons:AntDesignIconProvider Kind=PushpinFilled}"
            UnCheckedIcon="{antdicons:AntDesignIconProvider Kind=PushpinOutlined}"
            IsChecked="{Binding IsPinned, Mode=TwoWay}"
            SelectedIconBrush="{atom:SharedTokenResource ColorPrimary}" />
        <atom:ToggleIconButton
            CheckedIcon="{antdicons:AntDesignIconProvider Kind=UpOutlined}"
            UnCheckedIcon="{antdicons:AntDesignIconProvider Kind=DownOutlined}"
            IsChecked="{Binding IsExpanded, Mode=TwoWay}" />
    </StackPanel>
</DockPanel>
```

### 内容卡片操作栏

```xml
<StackPanel Orientation="Horizontal" Spacing="12">
    <atom:ToggleIconButton
        CheckedIcon="{antdicons:AntDesignIconProvider Kind=HeartFilled}"
        UnCheckedIcon="{antdicons:AntDesignIconProvider Kind=HeartOutlined}"
        SelectedIconBrush="Red"
        NormalIconBrush="Gray"
        IconWidth="16" IconHeight="16" />
    <atom:ToggleIconButton
        CheckedIcon="{antdicons:AntDesignIconProvider Kind=StarFilled}"
        UnCheckedIcon="{antdicons:AntDesignIconProvider Kind=StarOutlined}"
        SelectedIconBrush="Gold"
        NormalIconBrush="Gray"
        IconWidth="16" IconHeight="16" />
</StackPanel>
```

### 侧边栏折叠按钮

```xml
<atom:ToggleIconButton
    CheckedIcon="{antdicons:AntDesignIconProvider Kind=MenuFoldOutlined}"
    UnCheckedIcon="{antdicons:AntDesignIconProvider Kind=MenuUnfoldOutlined}"
    IsChecked="{Binding IsSidebarCollapsed, Mode=TwoWay}"
    IconWidth="16" IconHeight="16" />
```
