# Avatar 使用文档

本文档介绍 AtomUI Avatar 控件的各种使用方式，示例代码摘自 Gallery 演示程序。

> 📖 Gallery 源码位置：`controlgallery/AtomUIGallery/ShowCases/Views/DataDisplay/AvatarShowCase.axaml`

---

## 前置准备

在 AXAML 中使用 Avatar，需要引入以下命名空间：

```xml
xmlns:atom="https://atomui.net"
xmlns:antdicons="https://atomui.net/icons/antdesign"
```

在 C# 代码中引入：

```csharp
using AtomUI.Desktop.Controls;   // Avatar, AvatarGroup
using AtomUI.Controls;            // AvatarShape, CustomizableSizeType 等共享类型
```

---

## 1. 基本用法 — 三种尺寸和两种形状

Avatar 支持三种预设尺寸（Large / Middle / Small）和自定义像素尺寸，以及圆形和方形两种形状。

### 圆形头像（默认）

```xml
<StackPanel Orientation="Horizontal" Spacing="10">
    <atom:Avatar Icon="{antdicons:AntDesignIconProvider UserOutlined}" Size="64" />
    <atom:Avatar Icon="{antdicons:AntDesignIconProvider UserOutlined}" SizeType="Large" />
    <atom:Avatar Icon="{antdicons:AntDesignIconProvider UserOutlined}" />
    <atom:Avatar Icon="{antdicons:AntDesignIconProvider UserOutlined}" SizeType="Small" />
    <atom:Avatar Icon="{antdicons:AntDesignIconProvider UserOutlined}" Size="14" />
</StackPanel>
```

### 方形头像

```xml
<StackPanel Orientation="Horizontal" Spacing="10">
    <atom:Avatar Shape="Square" Icon="{antdicons:AntDesignIconProvider UserOutlined}" Size="64" />
    <atom:Avatar Shape="Square" Icon="{antdicons:AntDesignIconProvider UserOutlined}" SizeType="Large" />
    <atom:Avatar Shape="Square" Icon="{antdicons:AntDesignIconProvider UserOutlined}" />
    <atom:Avatar Shape="Square" Icon="{antdicons:AntDesignIconProvider UserOutlined}" SizeType="Small" />
    <atom:Avatar Shape="Square" Icon="{antdicons:AntDesignIconProvider UserOutlined}" Size="14" />
</StackPanel>
```

**使用场景指引**：
- **圆形**（默认）：代表用户/人物的头像
- **方形**：代表应用、团队、组织等实体

> 📖 参考：Gallery 中 "Basic" 示例。

---

## 2. 内容类型

Avatar 支持图标、文字、SVG 图片和位图四种内容类型，可自由组合背景色和前景色。

### 图标头像

```xml
<atom:Avatar Icon="{antdicons:AntDesignIconProvider UserOutlined}" />
```

### 文字头像

文字通过 `Text` 属性或 AXAML 直接嵌套设置（因为 `Text` 标记了 `[Content]`）：

```xml
<!-- 单字母 -->
<atom:Avatar>U</atom:Avatar>

<!-- 多字母（长文字会自动缩放） -->
<atom:Avatar Size="40">USER</atom:Avatar>
```

### SVG 图片头像

通过 `Src` 属性设置 SVG 路径（支持 `avares://` URI）：

```xml
<atom:Avatar Src="avares://AtomUIGallery/Assets/AvatarShowCase/AntDesign.svg" />
```

### 位图图片头像

通过 `BitmapSrc` 属性设置位图图片源：

```xml
<atom:Avatar BitmapSrc="/Assets/AvatarShowCase/PeopleAvatar4.png" />
```

### 自定义颜色

```xml
<!-- 橙色背景 + 深橙前景 -->
<atom:Avatar Background="#fde3cf" Foreground="#f56a00">U</atom:Avatar>

<!-- 绿色背景 + 白色图标 -->
<atom:Avatar Background="#87d068" Icon="{antdicons:AntDesignIconProvider UserOutlined}" />
```

> 📖 参考：Gallery 中 "Type" 示例。

---

## 3. 配合 Badge 使用

Avatar 通常搭配 `CountBadge` 或 `DotBadge` 展示消息数或在线状态提醒。

```xml
<StackPanel Orientation="Horizontal" Spacing="20">
    <!-- 计数徽标 -->
    <atom:CountBadge Count="5">
        <atom:Avatar Shape="Square" Icon="{antdicons:AntDesignIconProvider UserOutlined}" />
    </atom:CountBadge>

    <!-- 圆点徽标 -->
    <atom:DotBadge>
        <atom:Avatar Shape="Square" Icon="{antdicons:AntDesignIconProvider UserOutlined}" />
    </atom:DotBadge>
</StackPanel>
```

> 📖 参考：Gallery 中 "With Badge" 示例。

---

## 4. 文字自适应（Gap 控制）

对于文字类型头像，当文字宽度超出容器时会自动缩小。通过 `Gap` 属性控制文字距离左右边缘的间距（默认 4px）。

### 静态示例

```xml
<!-- Gap 较大，文字可用空间小，缩放更明显 -->
<atom:Avatar SizeType="Large" Gap="8">AtomUI</atom:Avatar>

<!-- Gap 较小，文字可用空间大 -->
<atom:Avatar SizeType="Large" Gap="2">AtomUI</atom:Avatar>
```

### 动态切换（数据绑定）

AXAML：

```xml
<StackPanel Orientation="Horizontal" Spacing="10">
    <atom:Avatar Background="{Binding AvatarBackground}"
                 Gap="{Binding AvatarGap}"
                 SizeType="Large"
                 Text="{Binding AvatarText}" />
    <atom:Button Name="ChangeUserButton" ButtonType="Default" SizeType="Small"
                 VerticalAlignment="Center">
        ChangeUser
    </atom:Button>
    <atom:Button Name="ChangeGapButton" ButtonType="Default" SizeType="Small"
                 VerticalAlignment="Center">
        ChangeGap
    </atom:Button>
</StackPanel>
```

Code-behind：

```csharp
public partial class AvatarShowCase : ReactiveUserControl<AvatarViewModel>
{
    public AvatarShowCase()
    {
        InitializeComponent();
        this.WhenActivated(disposables =>
        {
            if (DataContext is AvatarViewModel viewModel)
            {
                ChangeUserButton.Click += viewModel.HandleChangeUserClicked;
                ChangeGapButton.Click  += viewModel.HandleChangeGapClicked;
                Disposable.Create(() =>
                {
                    ChangeUserButton.Click -= viewModel.HandleChangeUserClicked;
                    ChangeGapButton.Click  -= viewModel.HandleChangeGapClicked;
                }).DisposeWith(disposables);
            }
        });
    }
}
```

ViewModel：

```csharp
public class AvatarViewModel : ReactiveObject, IRoutableViewModel, IActivatableViewModel
{
    private string? _avatarText;
    public string? AvatarText
    {
        get => _avatarText;
        set => this.RaiseAndSetIfChanged(ref _avatarText, value);
    }

    private double? _avatarGap;
    public double? AvatarGap
    {
        get => _avatarGap;
        set => this.RaiseAndSetIfChanged(ref _avatarGap, value);
    }

    private string? _avatarBackground;
    public string? AvatarBackground
    {
        get => _avatarBackground;
        set => this.RaiseAndSetIfChanged(ref _avatarBackground, value);
    }

    private List<string> _userList  = ["U", "Lucy", "Tom", "Edward"];
    private List<string> _colorList = ["#f56a00", "#7265e6", "#ffbf00", "#00a2ae"];
    private List<double> _gapList   = [4, 3, 2, 1];

    public void HandleChangeUserClicked(object? sender, EventArgs e)
    {
        var index = (_textCurrentIndex++) % 4;
        AvatarText       = _userList[index];
        AvatarBackground = _colorList[index];
    }

    public void HandleChangeGapClicked(object? sender, EventArgs e)
    {
        var index = (_gapCurrentIndex++) % 4;
        AvatarGap = _gapList[index];
    }
}
```

> 📖 参考：Gallery 中 "Autoset Font Size" 示例，完整代码见：
> - `controlgallery/AtomUIGallery/ShowCases/Views/DataDisplay/AvatarShowCase.axaml`
> - `controlgallery/AtomUIGallery/ShowCases/ViewModels/DataDisplay/AvatarViewModel.cs`

---

## 5. 头像组（AvatarGroup）

### 基本头像组

多个头像以重叠方式排列：

```xml
<atom:AvatarGroup>
    <atom:Avatar Src="avares://AtomUIGallery/Assets/AvatarShowCase/PeopleAvatar1.svg" />
    <atom:Avatar Background="#f56a00">K</atom:Avatar>
    <atom:Avatar Background="#87d068" Icon="{antdicons:AntDesignIconProvider UserOutlined}" />
    <atom:Avatar Icon="{antdicons:AntDesignIconProvider AntDesignOutlined}" Background="#1677ff" />
</atom:AvatarGroup>
```

### 限制最大显示数（折叠）

超出 `MaxDisplayCount` 的头像会折叠为 `+N` 计数头像，悬浮时弹出查看被折叠的头像：

```xml
<atom:AvatarGroup MaxDisplayCount="2"
                  FoldInfoAvatarForeground="#f56a00"
                  FoldInfoAvatarBackground="#fde3cf">
    <atom:Avatar Src="avares://AtomUIGallery/Assets/AvatarShowCase/PeopleAvatar2.svg" />
    <atom:Avatar Background="#f56a00">K</atom:Avatar>
    <atom:Avatar Background="#87d068" Icon="{antdicons:AntDesignIconProvider UserOutlined}" />
    <atom:Avatar Icon="{antdicons:AntDesignIconProvider AntDesignOutlined}" Background="#1677ff" />
</atom:AvatarGroup>
```

### 大号头像组

通过 `SizeType` 统一设置组内所有头像的尺寸：

```xml
<atom:AvatarGroup MaxDisplayCount="2"
                  FoldInfoAvatarForeground="#f56a00"
                  FoldInfoAvatarBackground="#fde3cf"
                  SizeType="Large">
    <atom:Avatar Src="avares://AtomUIGallery/Assets/AvatarShowCase/PeopleAvatar3.svg" />
    <atom:Avatar Background="#f56a00">K</atom:Avatar>
    <atom:Avatar Background="#87d068" Icon="{antdicons:AntDesignIconProvider UserOutlined}" />
    <atom:Avatar Icon="{antdicons:AntDesignIconProvider AntDesignOutlined}" Background="#1677ff" />
</atom:AvatarGroup>
```

### 点击触发弹出

通过 `FoldAvatarFlyoutTriggerType="Click"` 将折叠头像的弹出方式从悬浮改为点击：

```xml
<atom:AvatarGroup MaxDisplayCount="2"
                  FoldInfoAvatarForeground="#f56a00"
                  FoldInfoAvatarBackground="#fde3cf"
                  SizeType="Large"
                  FoldAvatarFlyoutTriggerType="Click">
    <atom:Avatar BitmapSrc="/Assets/AvatarShowCase/PeopleAvatar4.png" />
    <atom:Avatar Background="#f56a00">K</atom:Avatar>
    <atom:Avatar Background="#87d068" Icon="{antdicons:AntDesignIconProvider UserOutlined}" />
    <atom:Avatar Icon="{antdicons:AntDesignIconProvider AntDesignOutlined}" Background="#1677ff" />
</atom:AvatarGroup>
```

### 方形头像组

通过 `Shape="Square"` 统一设置组内头像形状：

```xml
<atom:AvatarGroup Shape="Square">
    <atom:Avatar Background="#fde3cf">A</atom:Avatar>
    <atom:Avatar Background="#f56a00">K</atom:Avatar>
    <atom:Avatar Background="#87d068" Icon="{antdicons:AntDesignIconProvider UserOutlined}" />
    <atom:Avatar Icon="{antdicons:AntDesignIconProvider AntDesignOutlined}" Background="#1677ff" />
</atom:AvatarGroup>
```

> 📖 参考：Gallery 中 "Avatar.Group" 示例。

---

## 6. 自定义尺寸

通过 `Size` 属性设置精确像素尺寸。设置后 `SizeType` 会自动切换为 `Custom`：

```xml
<!-- 64px 圆形 -->
<atom:Avatar Icon="{antdicons:AntDesignIconProvider UserOutlined}" Size="64" />

<!-- 14px 迷你头像 -->
<atom:Avatar Icon="{antdicons:AntDesignIconProvider UserOutlined}" Size="14" />

<!-- 64px 方形 -->
<atom:Avatar Shape="Square" Icon="{antdicons:AntDesignIconProvider UserOutlined}" Size="64" />
```

---

## 常见组合模式

### 用户信息卡片

```xml
<StackPanel Orientation="Horizontal" Spacing="8">
    <atom:Avatar Icon="{antdicons:AntDesignIconProvider UserOutlined}" SizeType="Large" />
    <StackPanel VerticalAlignment="Center">
        <atom:TextBlock FontWeight="Bold">John Doe</atom:TextBlock>
        <atom:TextBlock Foreground="{atom:SharedTokenResource ColorTextSecondary}">
            Administrator
        </atom:TextBlock>
    </StackPanel>
</StackPanel>
```

### 评论区头像

```xml
<StackPanel Orientation="Horizontal" Spacing="8">
    <atom:Avatar SizeType="Small" Background="#1677ff">J</atom:Avatar>
    <StackPanel VerticalAlignment="Center">
        <atom:TextBlock>John commented on this issue</atom:TextBlock>
    </StackPanel>
</StackPanel>
```

### 团队成员列表

```xml
<atom:AvatarGroup MaxDisplayCount="5" SizeType="Small">
    <atom:Avatar Background="#f56a00">A</atom:Avatar>
    <atom:Avatar Background="#7265e6">B</atom:Avatar>
    <atom:Avatar Background="#ffbf00">C</atom:Avatar>
    <atom:Avatar Background="#00a2ae">D</atom:Avatar>
    <atom:Avatar Background="#87d068">E</atom:Avatar>
    <atom:Avatar Background="#1677ff">F</atom:Avatar>
</atom:AvatarGroup>
```

### 带徽标的消息通知

```xml
<StackPanel Orientation="Horizontal" Spacing="16">
    <atom:CountBadge Count="99">
        <atom:Avatar SizeType="Large" Background="#1677ff"
                     Icon="{antdicons:AntDesignIconProvider UserOutlined}" />
    </atom:CountBadge>
    <atom:DotBadge>
        <atom:Avatar Background="#87d068">U</atom:Avatar>
    </atom:DotBadge>
</StackPanel>
```
