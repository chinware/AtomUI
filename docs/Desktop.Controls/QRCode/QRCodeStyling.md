# QRCode 自定义样式指南

QRCode 的视觉表现通过 `ControlTheme` + Design Token 系统控制。以下介绍几种常见的自定义方式。

---

## 1. 通过属性直接控制

最简单的方式是通过 QRCode 的公共属性来控制外观：

```xml
<!-- 基本使用：指定编码内容 -->
<atom:QRCode Value="https://atomui.net" />

<!-- 自定义尺寸 -->
<atom:QRCode Value="https://atomui.net" Size="200" />

<!-- 自定义颜色 -->
<atom:QRCode Value="https://atomui.net"
             Color="{atom:SharedTokenResource ColorSuccessText}" />

<!-- 自定义前景色和背景色 -->
<atom:QRCode Value="https://atomui.net"
             Color="{atom:SharedTokenResource ColorInfoText}"
             Background="{atom:SharedTokenResource ColorBgLayout}" />

<!-- 带中心图标 -->
<atom:QRCode Value="https://atomui.net"
             Icon="avares://AtomUIGallery/Assets/ATOMUI-LOGO.png" />

<!-- 无边框模式（嵌入弹出容器时使用） -->
<atom:QRCode Value="https://atomui.net" IsBordered="False" />

<!-- 设置纠错等级 -->
<atom:QRCode Value="https://atomui.net" EccLevel="H" />

<!-- 不同状态 -->
<atom:QRCode Value="https://atomui.net" Status="Loading" />
<atom:QRCode Value="https://atomui.net" Status="Expired" />
<atom:QRCode Value="https://atomui.net" Status="Scanned" />
```

> 📖 完整示例参考：`controlgallery/AtomUIGallery/ShowCases/Views/DataDisplay/QRCodeShowCase.axaml`

---

## 2. 通过 Style 覆盖样式

可以在 AXAML 中使用 `Style` 对 QRCode 进行全局或局部样式覆盖：

### 全局统一尺寸

```xml
<Window.Styles>
    <Style Selector="atom|QRCode">
        <Setter Property="Size" Value="200" />
    </Style>
</Window.Styles>
```

### 统一设置纠错等级

```xml
<Style Selector="atom|QRCode">
    <Setter Property="EccLevel" Value="H" />
</Style>
```

### 自定义边框样式

```xml
<!-- 所有有边框的二维码使用自定义圆角 -->
<Style Selector="atom|QRCode[IsBordered=True]">
    <Setter Property="CornerRadius" Value="16" />
</Style>
```

### 按状态定制样式

```xml
<!-- 过期状态二维码降低不透明度 -->
<Style Selector="atom|QRCode[Status=Expired]">
    <Setter Property="Opacity" Value="0.8" />
</Style>
```

---

## 3. 自定义状态内容

QRCode 的非 Active 状态均支持完全自定义渲染内容，通过设置对应的 `Content` 属性覆盖默认 UI：

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

> 📖 参考：`controlgallery/AtomUIGallery/ShowCases/Views/DataDisplay/QRCodeShowCase.axaml` 中 "自定义状态渲染器" 示例。

---

## 4. 通过 ControlTheme 完全替换主题

如果需要彻底替换 QRCode 的模板和样式，可以定义自定义 `ControlTheme`：

```xml
<ControlTheme x:Key="MyCustomQRCode" TargetType="atom:QRCode">
    <Setter Property="Template">
        <ControlTemplate>
            <Border Background="{TemplateBinding Background}"
                    CornerRadius="{TemplateBinding CornerRadius}"
                    Padding="{TemplateBinding Padding}">
                <Image Source="{TemplateBinding Bitmap}" />
            </Border>
        </ControlTemplate>
    </Setter>
</ControlTheme>

<!-- 使用 -->
<atom:QRCode Theme="{StaticResource MyCustomQRCode}"
             Value="https://atomui.net" />
```

> ⚠️ 注意：完全替换 ControlTheme 将丢失内置的状态切换、遮罩、刷新按钮等功能。建议优先使用属性控制和 Style 覆盖。

---

## 5. 嵌入弹出容器

QRCode 常见的高级用法是嵌入 Flyout 或 Popover 中，通过 hover 或 click 触发显示：

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

> 📖 参考：`controlgallery/AtomUIGallery/ShowCases/Views/DataDisplay/QRCodeShowCase.axaml` 中 "高级用法" 示例。

---

## 样式选择器速查

> 说明：Avalonia 样式选择器中使用 `atom|QRCode` 语法引用 `atom` XML 命名空间下的 `QRCode` 类型，其中 `|` 是命名空间分隔符。

### 基本选择器

| 选择器 | 说明 |
|---|---|
| `atom\|QRCode` | 匹配所有 QRCode 实例 |
| `atom\|QRCode:disabled` | 匹配禁用状态的 QRCode |

### 按属性选择

| 选择器 | 说明 |
|---|---|
| `atom\|QRCode[IsBordered=True]` | 匹配带边框的二维码 |
| `atom\|QRCode[IsBordered=False]` | 匹配无边框的二维码 |
| `atom\|QRCode[Status=Active]` | 匹配正常状态的二维码 |
| `atom\|QRCode[Status=Loading]` | 匹配加载中状态的二维码 |
| `atom\|QRCode[Status=Expired]` | 匹配已过期状态的二维码 |
| `atom\|QRCode[Status=Scanned]` | 匹配已扫描状态的二维码 |

### 模板部件选择器

| 选择器 | 说明 |
|---|---|
| `atom\|QRCode /template/ Border#Frame` | 访问主框架 Border |
| `atom\|QRCode /template/ Border#Mask` | 访问半透明遮罩层 |
| `atom\|QRCode /template/ Border#ImageFrame` | 访问中心图标框 |
| `atom\|QRCode /template/ Panel#LoadingLayout` | 访问加载状态面板 |
| `atom\|QRCode /template/ Panel#ExpiredLayout` | 访问过期状态面板 |
| `atom\|QRCode /template/ Panel#ScannedLayout` | 访问已扫描状态面板 |

### 组合选择器示例

| 选择器 | 说明 |
|---|---|
| `atom\|QRCode:not(atom\|QRCode[Status=Active])` | 匹配所有非正常状态的二维码 |
| `atom\|QRCode[IsBordered=True] /template/ Border#Frame` | 有边框二维码的主框架 |
