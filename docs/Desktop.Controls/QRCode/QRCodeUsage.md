# QRCode 使用文档

本文档介绍 AtomUI QRCode 控件的各种使用方式，示例代码摘自 Gallery 演示程序。

> 📖 Gallery 源码位置：`controlgallery/AtomUIGallery/ShowCases/Views/DataDisplay/QRCodeShowCase.axaml`

---

## 前置准备

在 AXAML 中使用 QRCode，需要引入以下命名空间：

```xml
xmlns:atom="https://atomui.net"
xmlns:antdicons="https://atomui.net/icons/antdesign"
```

在 C# 代码中引入：

```csharp
using AtomUI.Desktop.Controls;   // QRCode 控件
using AtomUI.Controls;            // QRCodeEccLevel, QRCodeStatus 等共享类型
```

---

## 1. 基本用法

最简单的使用方式——指定 `Value` 属性即可生成二维码：

```xml
<atom:QRCode Value="https://atomui.net" />
```

可以与输入框绑定，实现动态修改二维码内容：

```xml
<StackPanel Orientation="Vertical">
    <atom:QRCode Value="{Binding QRCodeInput}" />
    <atom:LineEdit Text="{Binding QRCodeInput}" Margin="0,20,0,0" />
</StackPanel>
```

```csharp
// ViewModel
public class QRCodeViewModel : ReactiveObject
{
    private string _qrCodeInput = "https://atomui.net";
    public string QRCodeInput
    {
        get => _qrCodeInput;
        set => this.RaiseAndSetIfChanged(ref _qrCodeInput, value);
    }
}
```

---

## 2. 带中心图标

通过 `Icon` 属性在二维码中央叠加 Logo 或品牌图标：

```xml
<atom:QRCode Value="https://atomui.net"
             Icon="avares://AtomUIGallery/Assets/ATOMUI-LOGO.png" />
```

> **提示**：使用中心图标时建议将 `EccLevel` 设为 `Q` 或 `H`，以确保被遮挡区域的信息仍能被正确识别。

带图标并指定图标大小：

```xml
<atom:QRCode Value="https://atomui.net"
             Icon="avares://AtomUIGallery/Assets/ATOMUI-LOGO.png"
             IconSize="48"
             EccLevel="H" />
```

---

## 3. 不同状态

通过 `Status` 属性控制二维码的四种状态：

```xml
<WrapPanel ItemSpacing="20" LineSpacing="20" Orientation="Horizontal">
    <!-- 加载中 -->
    <atom:QRCode Value="https://atomui.net" Status="Loading" />
    <!-- 已过期 -->
    <atom:QRCode Value="https://atomui.net" Status="Expired" />
    <!-- 已扫描 -->
    <atom:QRCode Value="https://atomui.net" Status="Scanned" />
</WrapPanel>
```

**各状态的默认表现**：
- **Loading**：半透明遮罩 + 居中旋转加载指示器（`Spin`）
- **Expired**：半透明遮罩 + "二维码过期"文案 + 带刷新图标的链接按钮
- **Scanned**：半透明遮罩 + "已扫描"文案

---

## 4. 自定义状态渲染内容

每种非 Active 状态均支持自定义渲染内容，通过 `LoadingContent`、`ExpiredContent`、`ScannedContent` 属性设置：

### 自定义加载状态

```xml
<atom:QRCode Value="https://atomui.net" Status="Loading">
    <atom:QRCode.LoadingContent>
        <StackPanel Orientation="Vertical"
                    HorizontalAlignment="Center"
                    VerticalAlignment="Center">
            <atom:Spin IsSpinning="True" HorizontalAlignment="Center" />
            <atom:TextBlock>Loading...</atom:TextBlock>
        </StackPanel>
    </atom:QRCode.LoadingContent>
</atom:QRCode>
```

### 自定义过期状态

```xml
<atom:QRCode Value="https://atomui.net" Status="Expired">
    <atom:QRCode.ExpiredContent>
        <StackPanel Orientation="Vertical"
                    HorizontalAlignment="Center"
                    VerticalAlignment="Center">
            <StackPanel Orientation="Horizontal"
                        HorizontalAlignment="Center"
                        VerticalAlignment="Center" Spacing="5">
                <antdicons:CloseCircleFilled FillBrush="Red" Width="16" Height="16" />
                <atom:TextBlock HorizontalAlignment="Center"
                                VerticalAlignment="Center">二维码过期</atom:TextBlock>
            </StackPanel>
            <atom:Button HorizontalAlignment="Center"
                         Icon="{antdicons:AntDesignIconProvider Kind=ReloadOutlined}"
                         ButtonType="Link">
                点击刷新
            </atom:Button>
        </StackPanel>
    </atom:QRCode.ExpiredContent>
</atom:QRCode>
```

### 自定义已扫描状态

```xml
<atom:QRCode Value="https://atomui.net" Status="Scanned">
    <atom:QRCode.ScannedContent>
        <StackPanel Orientation="Horizontal"
                    HorizontalAlignment="Center"
                    VerticalAlignment="Center"
                    Spacing="5">
            <antdicons:CheckCircleFilled FillBrush="Green" Width="16" Height="16" />
            <atom:TextBlock HorizontalAlignment="Center"
                            VerticalAlignment="Center">已扫描</atom:TextBlock>
        </StackPanel>
    </atom:QRCode.ScannedContent>
</atom:QRCode>
```

---

## 5. 自定义尺寸

通过 `Size` 属性控制二维码的像素大小，同时可动态调整 `IconSize`：

```xml
<StackPanel Orientation="Vertical">
    <StackPanel Orientation="Horizontal" Spacing="10" Margin="0,0,0,16">
        <atom:Button Name="SmallerBtn"
                     Icon="{antdicons:AntDesignIconProvider Kind=MinusOutlined}">
            Smaller
        </atom:Button>
        <atom:Button Name="LargerBtn"
                     Icon="{antdicons:AntDesignIconProvider Kind=PlusOutlined}">
            Larger
        </atom:Button>
    </StackPanel>
    <atom:QRCode Value="https://atomui.net"
                 Icon="avares://AtomUIGallery/Assets/ATOMUI-LOGO.png"
                 Size="{Binding Size}"
                 IconSize="{Binding IconSize}" />
</StackPanel>
```

```csharp
// ViewModel（使用 ReactiveUI）
public class QRCodeViewModel : ReactiveObject
{
    private const double MinSize = 48;
    private const double MaxSize = 300;

    private int _size = 160;
    public int Size
    {
        get => _size;
        set => this.RaiseAndSetIfChanged(ref _size, value);
    }

    private readonly ObservableAsPropertyHelper<int> _iconSize;
    public int IconSize => _iconSize.Value;

    public ReactiveCommand<Button, Unit> SmallerCommand { get; }
    public ReactiveCommand<Button, Unit> LargerCommand { get; }

    public QRCodeViewModel(IScreen screen)
    {
        var smallerCanExecute = this.WhenAnyValue(x => x.Size, size => size > MinSize);
        var largerCanExecute  = this.WhenAnyValue(x => x.Size, size => size < MaxSize);
        SmallerCommand = ReactiveCommand.Create<Button>(_ => { Size -= 10; }, smallerCanExecute);
        LargerCommand  = ReactiveCommand.Create<Button>(_ => { Size += 10; }, largerCanExecute);
        _iconSize = this.WhenAnyValue(x => x.Size, size => size / 4)
                       .ToProperty(this, x => x.IconSize);
    }
}
```

---

## 6. 自定义颜色

通过 `Color` 属性控制二维码前景色，`Background` 属性控制背景色：

```xml
<StackPanel Orientation="Horizontal" Spacing="10">
    <!-- 使用成功色 -->
    <atom:QRCode Value="https://atomui.net"
                 Color="{atom:SharedTokenResource ColorSuccessText}" />

    <!-- 使用信息色 + 自定义背景 -->
    <atom:QRCode Value="https://atomui.net"
                 Color="{atom:SharedTokenResource ColorInfoText}"
                 Background="{atom:SharedTokenResource ColorBgLayout}" />
</StackPanel>
```

---

## 7. 纠错等级

通过 `EccLevel` 属性设置纠错等级，与 `Segmented` 控件组合实现动态切换：

```xml
<StackPanel Orientation="Vertical" Spacing="10">
    <atom:QRCode Value="https://gw.alipayobjects.com/zos/rmsportal/KDpgvguMpGfqaHPjicRK.svg"
                 EccLevel="{Binding #EccLevelSegmented.SelectedItem}" />
    <atom:Segmented Name="EccLevelSegmented" />
</StackPanel>
```

```csharp
// Code-behind：设置 Segmented 的数据源
viewModel.EccLevels = new List<string>
{
    nameof(QRCodeEccLevel.L),
    nameof(QRCodeEccLevel.M),
    nameof(QRCodeEccLevel.Q),
    nameof(QRCodeEccLevel.H)
};
```

---

## 8. 无边框模式

通过 `IsBordered="False"` 移除外边框和内间距，适合嵌入弹出容器：

```xml
<atom:QRCode Value="https://atomui.net" IsBordered="False" />
```

---

## 9. 嵌入 Flyout（高级用法）

将无边框的二维码嵌入 Flyout 弹出层，通过 hover 触发显示：

```xml
<atom:FlyoutHost Trigger="Hover">
    <atom:FlyoutHost.Flyout>
        <atom:Flyout>
            <atom:QRCode Value="https://atomui.net" IsBordered="False" />
        </atom:Flyout>
    </atom:FlyoutHost.Flyout>
    <atom:Button ButtonType="Primary">Hover me</atom:Button>
</atom:FlyoutHost>
```

---

## 10. 刷新事件处理

当二维码处于过期状态时，用户点击刷新按钮会触发 `RefreshRequested` 事件：

```xml
<atom:QRCode Value="{Binding QRCodeValue}"
             Status="{Binding QRCodeStatus}"
             RefreshRequested="OnRefreshRequested" />
```

```csharp
// Code-behind
private void OnRefreshRequested(object? sender, EventArgs e)
{
    if (sender is QRCode qrCode)
    {
        // 设置为加载中
        qrCode.Status = QRCodeStatus.Loading;

        Dispatcher.UIThread.InvokeAsync(async () =>
        {
            // 模拟异步生成新的二维码
            await Task.Delay(TimeSpan.FromSeconds(2));
            qrCode.Value = $"https://atomui.net?t={DateTime.Now.Ticks}";
            qrCode.Status = QRCodeStatus.Active;
        });
    }
}
```

---

## 常见组合模式

### 登录扫码场景

```xml
<StackPanel HorizontalAlignment="Center" Spacing="8">
    <atom:TextBlock HorizontalAlignment="Center"
                    FontSize="16" FontWeight="SemiBold">
        扫码登录
    </atom:TextBlock>
    <atom:QRCode Value="{Binding LoginQRCodeUrl}"
                 Status="{Binding LoginStatus}"
                 Size="200"
                 EccLevel="M"
                 RefreshRequested="OnLoginRefresh" />
    <atom:TextBlock HorizontalAlignment="Center"
                    Foreground="{atom:SharedTokenResource ColorTextSecondary}">
        请使用手机扫描二维码
    </atom:TextBlock>
</StackPanel>
```

### 分享二维码（带 Logo）

```xml
<atom:QRCode Value="https://atomui.net"
             Icon="avares://MyApp/Assets/logo.png"
             IconSize="48"
             EccLevel="H"
             Size="200" />
```

### 气泡卡片中的二维码

```xml
<atom:FlyoutHost Trigger="Hover">
    <atom:FlyoutHost.Flyout>
        <atom:Flyout>
            <StackPanel Spacing="8">
                <atom:TextBlock FontWeight="SemiBold">扫码访问</atom:TextBlock>
                <atom:QRCode Value="https://atomui.net" IsBordered="False" Size="120" />
            </StackPanel>
        </atom:Flyout>
    </atom:FlyoutHost.Flyout>
    <atom:Button ButtonType="Link">查看二维码</atom:Button>
</atom:FlyoutHost>
```
