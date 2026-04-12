# Avatar 自定义样式指南

Avatar 的视觉表现通过 `ControlTheme` + Design Token 系统控制。以下介绍几种常见的自定义方式。

---

## 1. 使用属性直接控制

最简单的方式是通过 Avatar 的公共属性来控制外观：

```xml
<!-- 图标头像 -->
<atom:Avatar Icon="{antdicons:AntDesignIconProvider UserOutlined}" />

<!-- 文字头像 -->
<atom:Avatar>U</atom:Avatar>

<!-- SVG 图片头像 -->
<atom:Avatar Src="avares://AtomUIGallery/Assets/AvatarShowCase/AntDesign.svg" />

<!-- 位图图片头像 -->
<atom:Avatar BitmapSrc="/Assets/AvatarShowCase/PeopleAvatar4.png" />

<!-- 方形 -->
<atom:Avatar Shape="Square" Icon="{antdicons:AntDesignIconProvider UserOutlined}" />

<!-- 三种预设尺寸 -->
<atom:Avatar SizeType="Large" Icon="{antdicons:AntDesignIconProvider UserOutlined}" />
<atom:Avatar SizeType="Middle" Icon="{antdicons:AntDesignIconProvider UserOutlined}" />
<atom:Avatar SizeType="Small" Icon="{antdicons:AntDesignIconProvider UserOutlined}" />

<!-- 自定义尺寸（64 像素） -->
<atom:Avatar Icon="{antdicons:AntDesignIconProvider UserOutlined}" Size="64" />

<!-- 自定义背景和前景色 -->
<atom:Avatar Background="#fde3cf" Foreground="#f56a00">U</atom:Avatar>
<atom:Avatar Background="#87d068" Icon="{antdicons:AntDesignIconProvider UserOutlined}" />
```

> 📖 完整示例参考：`controlgallery/AtomUIGallery/ShowCases/Views/DataDisplay/AvatarShowCase.axaml`

---

## 2. 通过 Style 覆盖样式

可以在 AXAML 中使用 `Style` 对 Avatar 进行全局或局部样式覆盖：

### 全局统一样式

```xml
<Window.Styles>
    <!-- 所有 Avatar 居中对齐 -->
    <Style Selector="atom|Avatar">
        <Setter Property="VerticalAlignment" Value="Center" />
    </Style>
</Window.Styles>
```

### 按形状定制

```xml
<!-- 方形头像使用蓝色背景 -->
<Style Selector="atom|Avatar[Shape=Square]">
    <Setter Property="Background" Value="#1677ff" />
</Style>

<!-- 圆形头像添加边框 -->
<Style Selector="atom|Avatar[Shape=Circle]">
    <Setter Property="BorderThickness" Value="2" />
    <Setter Property="BorderBrush" Value="#d9d9d9" />
</Style>
```

### 按尺寸定制

```xml
<!-- 大号头像使用粗体文字 -->
<Style Selector="atom|Avatar[SizeType=Large]">
    <Setter Property="FontWeight" Value="Bold" />
</Style>

<!-- 小号头像使用较细边框 -->
<Style Selector="atom|Avatar[SizeType=Small]">
    <Setter Property="BorderThickness" Value="1" />
</Style>
```

### 按内容类型定制

```xml
<!-- 图标类型头像使用特定背景色 -->
<Style Selector="atom|Avatar[ContentType=Icon]">
    <Setter Property="Background" Value="#1677ff" />
</Style>

<!-- 文字类型头像使用特定字体大小 -->
<Style Selector="atom|Avatar[ContentType=Text]">
    <Setter Property="FontSize" Value="16" />
</Style>
```

---

## 3. 通过 ControlTheme 完全替换主题

如果需要彻底替换 Avatar 的模板和样式，可以定义自定义 `ControlTheme`：

```xml
<ControlTheme x:Key="MyCustomAvatar" TargetType="atom:Avatar">
    <Setter Property="Template">
        <ControlTemplate>
            <Border Background="{TemplateBinding Background}"
                    CornerRadius="{TemplateBinding CornerRadius}"
                    Width="{TemplateBinding Width}"
                    Height="{TemplateBinding Height}">
                <atom:TextBlock Text="{TemplateBinding Text}"
                                HorizontalAlignment="Center"
                                VerticalAlignment="Center" />
            </Border>
        </ControlTemplate>
    </Setter>
</ControlTheme>

<!-- 使用 -->
<atom:Avatar Theme="{StaticResource MyCustomAvatar}" Background="#f56a00">U</atom:Avatar>
```

> ⚠️ 注意：完全替换 ControlTheme 将丢失内置的多内容类型切换、图标展示、SVG 支持等功能。建议优先使用 Style 覆盖。

---

## 4. 配合 Badge 使用

Avatar 常与 `CountBadge`（计数徽标）和 `DotBadge`（圆点徽标）组合使用，用于提醒和通知：

```xml
<!-- 计数徽标 -->
<atom:CountBadge Count="5">
    <atom:Avatar Shape="Square" Icon="{antdicons:AntDesignIconProvider UserOutlined}" />
</atom:CountBadge>

<!-- 圆点徽标 -->
<atom:DotBadge>
    <atom:Avatar Shape="Square" Icon="{antdicons:AntDesignIconProvider UserOutlined}" />
</atom:DotBadge>
```

> 📖 参考：`controlgallery/AtomUIGallery/ShowCases/Views/DataDisplay/AvatarShowCase.axaml` 中 "With Badge" 示例。

---

## 5. 文字自适应与 Gap 控制

通过 `Gap` 属性控制文字距离左右边缘的间距，影响文字缩放行为：

```xml
<!-- 默认 Gap = 4 -->
<atom:Avatar SizeType="Large">AtomUI</atom:Avatar>

<!-- 较小的 Gap，文字可用空间更大 -->
<atom:Avatar SizeType="Large" Gap="2">AtomUI</atom:Avatar>

<!-- 较大的 Gap，文字可用空间更小，缩放更明显 -->
<atom:Avatar SizeType="Large" Gap="8">AtomUI</atom:Avatar>
```

动态切换 Gap 和文字内容（绑定 ViewModel）：

```xml
<atom:Avatar Background="{Binding AvatarBackground}"
             Gap="{Binding AvatarGap}"
             SizeType="Large"
             Text="{Binding AvatarText}" />
<atom:Button Name="ChangeUserButton" ButtonType="Default" SizeType="Small">
    ChangeUser
</atom:Button>
<atom:Button Name="ChangeGapButton" ButtonType="Default" SizeType="Small">
    ChangeGap
</atom:Button>
```

> 📖 参考：`controlgallery/AtomUIGallery/ShowCases/Views/DataDisplay/AvatarShowCase.axaml` 中 "Autoset Font Size" 示例。

---

## 6. AvatarGroup 样式定制

### 折叠计数头像颜色

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

### 统一尺寸和形状

AvatarGroup 的 `SizeType` 和 `Shape` 会自动传递给所有子 Avatar：

```xml
<atom:AvatarGroup SizeType="Large" Shape="Square">
    <atom:Avatar Background="#fde3cf">A</atom:Avatar>
    <atom:Avatar Background="#f56a00">K</atom:Avatar>
    <atom:Avatar Background="#87d068" Icon="{antdicons:AntDesignIconProvider UserOutlined}" />
</atom:AvatarGroup>
```

### 点击触发折叠弹出

```xml
<atom:AvatarGroup MaxDisplayCount="2"
                  FoldAvatarFlyoutTriggerType="Click"
                  SizeType="Large">
    <atom:Avatar BitmapSrc="/Assets/AvatarShowCase/PeopleAvatar4.png" />
    <atom:Avatar Background="#f56a00">K</atom:Avatar>
    <atom:Avatar Background="#87d068" Icon="{antdicons:AntDesignIconProvider UserOutlined}" />
</atom:AvatarGroup>
```

> 📖 参考：`controlgallery/AtomUIGallery/ShowCases/Views/DataDisplay/AvatarShowCase.axaml` 中 "Avatar.Group" 示例。

---

## 7. 控制动画行为

```xml
<!-- 禁用过渡动画 -->
<atom:Avatar IsMotionEnabled="False" Icon="{antdicons:AntDesignIconProvider UserOutlined}" />

<!-- AvatarGroup 统一控制子 Avatar 动画 -->
<atom:AvatarGroup IsMotionEnabled="False">
    <atom:Avatar>A</atom:Avatar>
    <atom:Avatar>B</atom:Avatar>
</atom:AvatarGroup>
```

---

## 样式选择器速查

> 说明：Avalonia 样式选择器中使用 `atom|Avatar` 语法引用 `atom` XML 命名空间下的 `Avatar` 类型，其中 `|` 是命名空间分隔符。

### 按控件类型选择

| 选择器 | 说明 |
|---|---|
| `atom\|Avatar` | 匹配所有 Avatar 实例 |
| `atom\|AvatarGroup` | 匹配所有 AvatarGroup 实例 |

### 按形状选择

| 选择器 | 说明 |
|---|---|
| `atom\|Avatar[Shape=Circle]` | 匹配圆形头像（默认形状） |
| `atom\|Avatar[Shape=Square]` | 匹配方形头像 |

### 按尺寸选择

| 选择器 | 说明 |
|---|---|
| `atom\|Avatar[SizeType=Large]` | 匹配大号头像（`ContainerSizeLG`） |
| `atom\|Avatar[SizeType=Middle]` | 匹配中号头像（默认尺寸） |
| `atom\|Avatar[SizeType=Small]` | 匹配小号头像（`ContainerSizeSM`） |
| `atom\|Avatar[SizeType=Custom]` | 匹配自定义尺寸头像 |

### 按内容类型选择

| 选择器 | 说明 |
|---|---|
| `atom\|Avatar[ContentType=Icon]` | 匹配图标内容的头像 |
| `atom\|Avatar[ContentType=Text]` | 匹配文字内容的头像 |
| `atom\|Avatar[ContentType=BitmapImage]` | 匹配位图图片内容的头像 |
| `atom\|Avatar[ContentType=SvgImage]` | 匹配 SVG 图片内容的头像 |

### 模板内部选择器

| 选择器 | 说明 |
|---|---|
| `atom\|Avatar /template/ Border#Frame` | 访问主框架 Border |
| `atom\|Avatar /template/ atom\|IconPresenter#IconPresenter` | 访问图标展示器 |
| `atom\|Avatar /template/ Image#ImagePresenter` | 访问位图展示器 |
| `atom\|Avatar /template/ Svg#SvgPresenter` | 访问 SVG 展示器 |
| `atom\|Avatar /template/ atom\|TextBlock#PART_TextPresenter` | 访问文字展示器 |

### 组合选择器示例

| 选择器 | 说明 |
|---|---|
| `atom\|Avatar[Shape=Square][SizeType=Large]` | 大号方形头像 |
| `atom\|Avatar[ContentType=Icon][SizeType=Small]` | 小号图标头像 |
| `atom\|Avatar[Shape=Circle]:pointerover` | 悬浮状态的圆形头像 |
