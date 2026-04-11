# Icon 使用规范指南

本文档规范 AtomUI 项目中 Icon 的各种使用方式，涵盖 C# 代码和 AXAML 标记两个场景。

---

## 1. AXAML 中使用 Icon

### 1.1 命名空间声明

在 AXAML 文件头部添加 Ant Design 图标命名空间：

```xml
xmlns:antdicons="https://atomui.net/icons/antdesign"
```

> **约定**: Ant Design 图标推荐前缀 `antdicons`，自定义图标包使用自己的前缀。

### 1.2 直接作为 XML 元素使用

最常用的方式——直接在 AXAML 中实例化图标控件：

```xml
<!-- 基础用法 -->
<antdicons:CheckCircleFilled />
<antdicons:LoadingOutlined />
<antdicons:SettingTwoTone />

<!-- 设置 Loading 动画 -->
<antdicons:LoadingOutlined LoadingAnimation="Spin" />

<!-- 自定义画刷颜色 -->
<antdicons:CheckCircleFilled
    FillBrush="{atom:SharedTokenResource ColorSuccess}" />

<!-- TwoTone 图标设置双色 -->
<antdicons:SettingTwoTone
    StrokeBrush="{atom:SharedTokenResource ColorPrimary}"
    FillBrush="{atom:SharedTokenResource ColorInfoBg}" />

<!-- 设置尺寸 -->
<antdicons:SearchOutlined Width="24" Height="24" />
```

**适用场景**：在 ControlTheme 模板中直接放置图标、需要精细控制图标属性时。

### 1.3 通过 IconProvider MarkupExtension 使用

用于需要将图标赋值给 `PathIcon?` 类型属性的场景：

```xml
<!-- 基本用法 - 简写（省略 Kind=） -->
<atom:Button Icon="{antdicons:AntDesignIconProvider CloseOutlined}" />

<!-- 完整写法 -->
<atom:Button Icon="{antdicons:AntDesignIconProvider Kind=CheckCircleFilled}" />

<!-- 设置画刷和尺寸 -->
<atom:IconButton
    Icon="{antdicons:AntDesignIconProvider Kind=DeleteOutlined,
           FillBrush=Red, Width=16, Height=16}" />

<!-- 设置动画 -->
<atom:Button
    Icon="{antdicons:AntDesignIconProvider Kind=LoadingOutlined,
           Animation=Spin}" />
```

> `AntDesignIconProvider` 的构造函数接受一个 `AntDesignIconKind` 参数，因此 `{antdicons:AntDesignIconProvider CloseOutlined}` 是 `{antdicons:AntDesignIconProvider Kind=CloseOutlined}` 的简写形式。

**适用场景**：给控件的 `Icon` 属性赋值（如 `Button.Icon`、`IconButton.Icon`、`MenuItem.Icon`）。

### 1.4 在 IconTemplate 中使用

```xml
<atom:IconTemplate>
    <antdicons:CloseOutlined />
</atom:IconTemplate>
```

**适用场景**：控件需要 `IconTemplate` 类型属性时（如 `Tour` 控件的关闭图标）。

### 1.5 主题文件中的典型模式

```xml
<ResourceDictionary
    xmlns="https://github.com/avaloniaui"
    xmlns:atom="https://atomui.net"
    xmlns:antdicons="https://atomui.net/icons/antdesign">

    <ControlTheme TargetType="atom:Alert">
        <Setter Property="Template">
            <ControlTemplate>
                <!-- 多个图标按状态显示/隐藏 -->
                <antdicons:CheckCircleFilled Name="SuccessIcon"
                    IsVisible="False" />
                <antdicons:InfoCircleFilled Name="InfoIcon"
                    IsVisible="False" />
                <antdicons:ExclamationCircleFilled Name="WarningIcon"
                    IsVisible="False" />
                <antdicons:CloseCircleFilled Name="ErrorIcon"
                    IsVisible="False" />
            </ControlTemplate>
        </Setter>

        <!-- 通过 Style Selector 控制可见性 -->
        <Style Selector="^:success /template/ antdicons|CheckCircleFilled#SuccessIcon">
            <Setter Property="IsVisible" Value="True" />
        </Style>
    </ControlTheme>
</ResourceDictionary>
```

> **注意**: 在 Style Selector 中引用图标类型时，使用 `antdicons|CheckCircleFilled` 语法（`|` 分隔命名空间和类型名）。

---

## 2. C# 代码中使用 Icon

### 2.1 直接实例化

```csharp
using AtomUI.Icons.AntDesign;

// 基础实例化
var icon = new CheckCircleFilled();

// 设置属性
var loadingIcon = new LoadingOutlined()
{
    LoadingAnimation = IconAnimation.Spin
};

// 通过 SetCurrentValue 设置（推荐，保留属性优先级）
SetCurrentValue(IconProperty, new CloseCircleFilled());

// 通过 SetValue 设置（指定绑定优先级）
SetValue(IconProperty, new CheckCircleFilled(), BindingPriority.Template);
```

### 2.2 动态根据状态创建 Icon

```csharp
using AtomUI.Icons.AntDesign;

// 根据状态选择不同图标
PathIcon? icon = notificationType switch
{
    NotificationType.Success     => new CheckCircleFilled(),
    NotificationType.Information => new InfoCircleFilled(),
    NotificationType.Warning     => new ExclamationCircleFilled(),
    NotificationType.Error       => new CloseCircleFilled(),
    _                            => null
};

// 设置到控件属性
if (icon != null)
{
    SetCurrentValue(IconProperty, icon);
}
```

### 2.3 带动画的 Loading 图标

```csharp
using AtomUI.Icons.AntDesign;
using AtomUI.Controls;

// 创建旋转加载图标
SetCurrentValue(SuffixLoadingIconProperty, new LoadingOutlined()
{
    LoadingAnimation = IconAnimation.Spin
});
```

### 2.4 IconFuncTemplate 用法

在代码中创建可复用的图标模板：

```csharp
using AtomUI.Controls;
using AtomUI.Icons.AntDesign;

// 创建函数式图标模板
var iconTemplate = new IconFuncTemplate(() => new SearchOutlined());
```

---

## 3. Icon 属性体系速查

### 3.1 外观属性

| 属性 | 类型 | 说明 | 默认来源 |
|---|---|---|---|
| `Width` / `Height` | `double` | 图标尺寸 | `SharedToken.IconSize` |
| `StrokeBrush` | `IBrush?` | 主描边画刷（Outlined 主色） | `SharedToken.ColorIconHover` |
| `FillBrush` | `IBrush?` | 主填充画刷（Filled 主色） | `SharedToken.ColorIconHover` |
| `SecondaryStrokeBrush` | `IBrush?` | 次描边画刷（TwoTone 次色） | `IconToken.SecondaryStrokeColor` |
| `SecondaryFillBrush` | `IBrush?` | 次填充画刷（TwoTone 次色） | `IconToken.SecondaryFillColor` |
| `FallbackBrush` | `IBrush?` | 回退画刷 | `White` |
| `StrokeWidth` | `double` | 描边线宽 | `IconToken.StrokeWidth` (4) |
| `StrokeLineCap` | `PenLineCap` | 线端样式 | `IconToken.StrokeLineCap` (Round) |
| `StrokeLineJoin` | `PenLineJoin` | 线连接样式 | `IconToken.StrokeLineJoin` (Round) |

### 3.2 动画属性

| 属性 | 类型 | 说明 | 默认值 |
|---|---|---|---|
| `LoadingAnimation` | `IconAnimation` | 动画类型 | `None` |
| `LoadingAnimationDuration` | `TimeSpan` | 旋转周期 | `1s` |
| `FillAnimationDuration` | `TimeSpan` | 颜色过渡时长 | `200ms`（来自 `MotionDurationMid` token） |
| `IsMotionEnabled` | `bool` | 启用/禁用动画 | 来自 `SharedToken.EnableMotion` |

### 3.3 IconAnimation 枚举

| 值 | 效果 |
|---|---|
| `None` | 无动画 |
| `Spin` | 匀速旋转（平滑） |
| `Pulse` | 步进旋转（8 步阶梯，类似 CSS `steps(8)`） |

---

## 4. 常见使用场景

### 4.1 按钮中的图标

```xml
<!-- 图标按钮 -->
<atom:Button Icon="{antdicons:AntDesignIconProvider SearchOutlined}"
             ButtonType="Primary">
    Search
</atom:Button>

<!-- 纯图标按钮 -->
<atom:IconButton Icon="{antdicons:AntDesignIconProvider DeleteOutlined}" />

<!-- Loading 按钮 -->
<atom:Button IsLoading="True" ButtonType="Primary">
    Submitting
</atom:Button>
<!-- Button 内部会自动显示 LoadingOutlined 带 Spin 动画 -->
```

### 4.2 输入框中的图标

```xml
<!-- 带前缀/后缀图标 -->
<atom:TextBox>
    <atom:TextBox.InnerLeftContent>
        <antdicons:UserOutlined />
    </atom:TextBox.InnerLeftContent>
</atom:TextBox>
```

### 4.3 提示信息中的状态图标

```csharp
// NotificationCard / MessageCard 内部实现模式
PathIcon? icon = messageType switch
{
    MessageType.Success => new CheckCircleFilled(),
    MessageType.Error   => new CloseCircleFilled(),
    MessageType.Warning => new ExclamationCircleFilled(),
    MessageType.Info    => new InfoCircleFilled(),
    MessageType.Loading => new LoadingOutlined()
    {
        LoadingAnimation = IconAnimation.Spin
    },
    _ => null
};
```

### 4.4 导航菜单中的图标

```xml
<atom:NavMenuItem Header="Dashboard"
                  Icon="{antdicons:AntDesignIconProvider DashboardOutlined}">
</atom:NavMenuItem>
```

### 4.5 进度条完成/异常图标

```csharp
// 在控件内部设置默认图标
SetValue(SuccessCompletedIconProperty,
    new CheckCircleFilled(), BindingPriority.Template);
SetValue(ExceptionCompletedIconProperty,
    new CloseCircleFilled(), BindingPriority.Template);
```

---

## 5. 颜色控制最佳实践

### 5.1 通过 Token 系统控制颜色（推荐）

在主题文件中使用 Token 资源，确保 Icon 颜色跟随主题变化：

```xml
<!-- 使用全局 SharedToken -->
<antdicons:CheckCircleFilled
    FillBrush="{atom:SharedTokenResource ColorSuccess}" />

<!-- 使用组件 Token -->
<antdicons:CloseOutlined
    StrokeBrush="{atom:TokenResource NotificationCloseIconColor}" />
```

### 5.2 IconPresenter 统一颜色

当 Icon 嵌入在复杂控件中时，使用 `IconPresenter` 统一管理颜色：

```xml
<atom:IconPresenter Icon="{TemplateBinding Icon}"
                    IconBrush="{atom:SharedTokenResource ColorText}" />
```

`IconPresenter` 会自动将 `IconBrush` 同时绑定到子 Icon 的 `StrokeBrush` 和 `FillBrush`，无论图标是 Outlined 还是 Filled 类型都能正确着色。

### 5.3 TwoTone 图标颜色定制

```xml
<!-- 默认主题色 -->
<antdicons:SettingTwoTone />
<!-- 主色=ColorPrimary，次色=ColorInfoBg（由 IconTheme.axaml 定义） -->

<!-- 自定义双色 -->
<antdicons:HeartTwoTone
    StrokeBrush="Red"
    FillBrush="Pink" />
```

---

## 6. 编码规范与约定

### 6.1 命名空间导入

```csharp
// 在 C# 中引用 Ant Design 图标
using AtomUI.Icons.AntDesign;

// 引用核心 Icon 类型
using AtomUI.Controls;  // Icon, IconAnimation, IconThemeType, etc.
```

### 6.2 在控件代码中使用图标的规范

- **优先使用 `SetCurrentValue`** 设置图标，保留属性优先级链：
  ```csharp
  SetCurrentValue(IconProperty, new CloseCircleFilled());
  ```

- **使用 `SetValue` + `BindingPriority.Template`** 设置模板级默认值：
  ```csharp
  SetValue(IconProperty, new CheckCircleFilled(), BindingPriority.Template);
  ```

- **不要在 AXAML 中硬编码图标颜色**，始终使用 Token 资源引用

### 6.3 图标类命名约定

每个图标类的命名格式为 `{IconName}{ThemeType}`：

| 格式 | 示例 |
|---|---|
| `{Name}Filled` | `CheckCircleFilled`、`HomeFilled` |
| `{Name}Outlined` | `SearchOutlined`、`LoadingOutlined` |
| `{Name}TwoTone` | `SettingTwoTone`、`HeartTwoTone` |

### 6.4 Style Selector 中引用图标类型

在 AXAML Style Selector 中，使用 `命名空间|类型名` 语法：

```xml
<!-- 正确 -->
<Style Selector="^:success /template/ antdicons|CheckCircleFilled#SuccessIcon">

<!-- 带名称 -->
<Style Selector="/template/ antdicons|LoadingOutlined#LoadingIcon">
```

---

## 7. 常用图标参考

| 用途 | 推荐图标 |
|---|---|
| 成功状态 | `CheckCircleFilled` / `CheckCircleOutlined` |
| 错误状态 | `CloseCircleFilled` / `CloseCircleOutlined` |
| 警告状态 | `ExclamationCircleFilled` / `ExclamationCircleOutlined` |
| 信息状态 | `InfoCircleFilled` / `InfoCircleOutlined` |
| 加载中 | `LoadingOutlined`（配合 `LoadingAnimation="Spin"`） |
| 关闭 | `CloseOutlined` |
| 搜索 | `SearchOutlined` |
| 删除 | `DeleteOutlined` |
| 编辑 | `EditOutlined` |
| 清除 | `CloseCircleFilled`（input 清除按钮） |
| 展开/收起 | `RightOutlined` / `DownOutlined` |
| 上传 | `UploadOutlined` |
| 下载 | `DownloadOutlined` |
| 设置 | `SettingOutlined` / `SettingTwoTone` |
| 用户 | `UserOutlined` |
| 眼睛（显示/隐藏） | `EyeOutlined` / `EyeInvisibleOutlined` |

