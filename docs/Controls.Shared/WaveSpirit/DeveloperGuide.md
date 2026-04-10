# AtomUI WaveSpirit 系统 — 开发者指南

> 本文档面向 AtomUI 控件开发者，介绍如何在自定义控件中集成 WaveSpirit 波浪动画效果。

---

## 1. 集成概述

为控件添加 WaveSpirit 波浪动画效果，需要在三个层级协作完成：

| 步骤 | 层级 | 操作 |
|---|---|---|
| 声明接口 | 控件 C# 代码 | 实现 `IWaveSpiritAwareControl`，注册共享属性 |
| 放置装饰器 | 控件主题 AXAML | 在模板中放置 `WaveSpiritDecorator` |
| 绑定 Token | 控件主题 AXAML | 设置 `IsWaveSpiritEnabled`、`WaveType` 等属性 |
| 触发动画 | 控件 C# 代码 | 在交互事件中调用 `_waveSpiritDecorator.Play()` |

---

## 2. 完整集成步骤

以下以一个假设的 `MyButton` 控件为例，演示完整集成流程。

### 2.1 步骤一：实现接口并注册属性

```csharp
using AtomUI.Controls;
using AtomUI.Controls.Primitives;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Media;
using Avalonia.Threading;

namespace AtomUI.Desktop.Controls;

public class MyButton : Button, IWaveSpiritAwareControl
{
    // 使用 AddOwner 注册共享属性
    public static readonly StyledProperty<bool> IsMotionEnabledProperty =
        MotionAwareControlProperty.IsMotionEnabledProperty.AddOwner<MyButton>();

    public static readonly StyledProperty<bool> IsWaveSpiritEnabledProperty =
        WaveSpiritAwareControlProperty.IsWaveSpiritEnabledProperty.AddOwner<MyButton>();

    internal static readonly StyledProperty<WaveSpiritType> WaveSpiritTypeProperty =
        WaveSpiritAwareControlProperty.WaveSpiritTypeProperty.AddOwner<MyButton>();

    // 属性包装器
    public bool IsMotionEnabled
    {
        get => GetValue(IsMotionEnabledProperty);
        set => SetValue(IsMotionEnabledProperty, value);
    }

    public bool IsWaveSpiritEnabled
    {
        get => GetValue(IsWaveSpiritEnabledProperty);
        set => SetValue(IsWaveSpiritEnabledProperty, value);
    }

    internal WaveSpiritType WaveSpiritType
    {
        get => GetValue(WaveSpiritTypeProperty);
        set => SetValue(WaveSpiritTypeProperty, value);
    }

    // 持有 WaveSpiritDecorator 引用
    private WaveSpiritDecorator? _waveSpiritDecorator;

    // 在 OnApplyTemplate 中获取装饰器引用
    protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
    {
        base.OnApplyTemplate(e);
        _waveSpiritDecorator = e.NameScope.Find<WaveSpiritDecorator>("PART_WaveSpirit");
    }

    // 在交互事件中触发波浪动画
    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        base.OnPropertyChanged(change);
        if (change.Property == IsPressedProperty)
        {
            if (IsWaveSpiritEnabled &&
                (change.OldValue as bool? == true))
            {
                Dispatcher.UIThread.Post(() =>
                {
                    _waveSpiritDecorator?.Play();
                });
            }
        }
    }
}
```

**关键要点**：

- 使用 `AddOwner<T>()` 复用 `WaveSpiritAwareControlProperty` 中定义的共享属性，**不要自行注册新的同名属性**。
- `WaveSpiritType` 属性通常为 `internal`，因为波纹类型由控件内部逻辑或主题决定，不暴露给用户。
- 触发时机使用 `Dispatcher.UIThread.Post()` 延迟一帧，确保 UI 状态已完全更新。

### 2.2 步骤二：AXAML 模板中放置装饰器

```xml
<ControlTheme xmlns="https://github.com/avaloniaui"
              xmlns:atom="https://atomui.net"
              x:Class="AtomUI.Desktop.Controls.Themes.MyButtonTheme"
              TargetType="atom:MyButton">
    <Style Selector="^:is(atom|MyButton)">
        <Setter Property="Template">
            <ControlTemplate>
                <Panel>
                    <!-- 波浪装饰器放在最底层 -->
                    <atom:WaveSpiritDecorator
                        Name="PART_WaveSpirit"
                        CornerRadius="{TemplateBinding CornerRadius}"
                        WaveType="{TemplateBinding WaveSpiritType}" />

                    <!-- 控件主体内容 -->
                    <Border Name="Frame"
                            Padding="{TemplateBinding Padding}"
                            Background="{TemplateBinding Background}"
                            BorderBrush="{TemplateBinding BorderBrush}"
                            BorderThickness="{TemplateBinding BorderThickness}"
                            CornerRadius="{TemplateBinding CornerRadius}">
                        <ContentPresenter Content="{TemplateBinding Content}" />
                    </Border>
                </Panel>
            </ControlTemplate>
        </Setter>

        <!-- 绑定全局 Token -->
        <Setter Property="IsMotionEnabled"
                Value="{atom:SharedTokenResource EnableMotion}" />
        <Setter Property="IsWaveSpiritEnabled"
                Value="{atom:SharedTokenResource EnableWaveSpirit}" />
        <Setter Property="ClipToBounds" Value="False" />
    </Style>
</ControlTheme>
```

**关键要点**：

- `WaveSpiritDecorator` 必须放在 `Panel` 的**第一个子元素**（最底层 z-order），波纹在控件内容下方绘制。
- 使用 `{TemplateBinding CornerRadius}` 将控件圆角同步给装饰器。
- **必须设置** `ClipToBounds="False"`，否则波纹超出边界的部分会被裁剪。
- 通过 `{atom:SharedTokenResource EnableWaveSpirit}` 绑定全局 Token。

### 2.3 步骤三：按需覆盖 WaveType

通过 Style Selector 设置固定类型：

```xml
<Style Selector="^ /template/ atom|WaveSpiritDecorator#PART_WaveSpirit">
    <Setter Property="WaveType" Value="RoundRectWave" />
</Style>
```

或在代码中根据外观动态设置：

```csharp
private void ConfigureWaveSpiritType()
{
    WaveSpiritType waveType = Shape switch
    {
        ButtonShape.Default => WaveSpiritType.RoundRectWave,
        ButtonShape.Round   => WaveSpiritType.PillWave,
        ButtonShape.Circle  => WaveSpiritType.CircleWave,
        _                   => WaveSpiritType.RoundRectWave
    };
    WaveSpiritType = waveType;
}
```

### 2.4 步骤四（可选）：动态覆盖 WaveBrush

默认 `WaveBrush` 由全局主题绑定为 `ColorPrimary`。某些场景需覆盖：

```csharp
if (IsDanger)
{
    IBrush? waveBrush = ButtonType == ButtonType.Primary && !IsGhost
        ? Background : Foreground;
    _waveSpiritDecorator.WaveBrush = waveBrush;
}
_waveSpiritDecorator?.Play();
```

---

## 3. 现有控件实现示例

### 3.1 Button（按钮）

**波纹触发时机**：按钮释放时（`IsPressed` 从 `true` 变为 `false`）。

**波纹类型选择**：根据 `Shape` 属性动态决定。

| Button Shape | WaveSpiritType |
|---|---|
| `Default` | `RoundRectWave` |
| `Round` | `PillWave` |
| `Circle` | `CircleWave` |

**触发条件**：`IsWaveSpiritEnabled && !IsLoading && ButtonType in {Primary, Default, Dashed}`

**AXAML 模板**（`ButtonTheme.axaml`）：

```xml
<Panel>
    <atom:WaveSpiritDecorator
        Name="{x:Static atom:ButtonThemeConstants.WaveSpiritPart}"
        CornerRadius="{TemplateBinding EffectiveCornerRadius}"
        WaveType="{TemplateBinding WaveSpiritType}" />
    <Border Name="Frame" ... />
</Panel>
```

### 3.2 CheckBox（复选框）

**波纹触发时机**：`State` 变为 `Checked` 时。

**波纹类型**：`RoundRectWave`（固定）。

**触发条件**：`IsWaveSpiritEnabled && IsEnabled && IsMotionEnabled && State == Checked`

```xml
<Style Selector="^ /template/ atom|WaveSpiritDecorator#PART_WaveSpirit">
    <Setter Property="WaveType" Value="RoundRectWave" />
</Style>
```

### 3.3 RadioButton（单选按钮）

**波纹触发时机**：`IsChecked` 变为 `true` 时。

**波纹类型**：`CircleWave`（固定，因为单选按钮是圆形）。

```xml
<Style Selector="^ /template/ atom|WaveSpiritDecorator#PART_WaveSpirit">
    <Setter Property="WaveType" Value="CircleWave" />
</Style>
```

### 3.4 ToggleSwitch（开关）

**波纹触发时机**：`IsChecked` 任意变化时。

**波纹类型**：`PillWave`（固定，开关为药丸形）。

**特殊处理**：需要手动为装饰器设置布局区域：

```csharp
_waveSpiritDecorator?.Arrange(new Rect(new Point(0, 0), DesiredSize.Deflate(Margin)));
```

### 3.5 OptionButton（选项按钮）

**波纹触发时机**：按钮释放时（`IsPressed` 从 `true` 变为 `false`）。

**波纹类型**：`RoundRectWave`（固定）。

---

## 4. WaveSpiritDecorator 的 PART 命名约定

```csharp
internal class WaveSpiritDecorator : Control
{
    public const string WaveSpiritPart = "PART_WaveSpirit";
}
```

在 AXAML 中可以通过两种方式引用：

```xml
<!-- 方式一：使用 WaveSpiritDecorator 的常量 -->
<atom:WaveSpiritDecorator
    Name="{x:Static atom:WaveSpiritDecorator.WaveSpiritPart}" />

<!-- 方式二：使用控件自己的 ThemeConstants -->
<atom:WaveSpiritDecorator
    Name="{x:Static atom:ButtonThemeConstants.WaveSpiritPart}" />
```

---

## 5. 全局开关控制

### 5.1 通过 AXAML 绑定 Token（推荐）

```xml
<Setter Property="IsWaveSpiritEnabled"
        Value="{atom:SharedTokenResource EnableWaveSpirit}" />
```

### 5.2 通过扩展方法绑定（代码方式）

```csharp
public MyControl()
{
    this.ConfigureWaveSpiritBindingStyle();
}
```

### 5.3 运行时切换

```csharp
themeManager.IsWaveSpiritEnabled = false;
```

---

## 6. 注意事项与最佳实践

### 6.1 必须设置 ClipToBounds 为 False

```xml
<Setter Property="ClipToBounds" Value="False" />
```

### 6.2 WaveSpiritDecorator 必须放在 Panel 中

```xml
<!-- 正确 -->
<Panel>
    <atom:WaveSpiritDecorator ... />
    <Border ... />
</Panel>
```

### 6.3 CornerRadius 同步

`RoundRectWave` 类型必须同步控件圆角：

```xml
<atom:WaveSpiritDecorator CornerRadius="{TemplateBinding CornerRadius}" />
```

`CircleWave` 和 `PillWave` 不需要设置 `CornerRadius`。

### 6.4 动画触发时机

- **按钮类**：鼠标释放时触发（`IsPressed` true 到 false）
- **选择类**（CheckBox、RadioButton）：选中状态变化时触发
- **开关类**（ToggleSwitch）：`IsChecked` 任意变化时触发

### 6.5 不要硬编码 Token 值

波纹参数已通过 `WaveSpiritDecoratorTheme.axaml` 绑定 Token。如需覆盖，用 Style Selector 设置。唯一例外是 `WaveBrush` 的动态覆盖（如 Danger 按钮的红色波纹）。

### 6.6 使用 Dispatcher.UIThread.Post 延迟触发

```csharp
Dispatcher.UIThread.Post(() => { _waveSpiritDecorator?.Play(); });
```

### 6.7 WaveSpiritDecorator 的 Bounds 自适应

装饰器自动监听 `Bounds` 变化来更新尺寸。对于自定义布局控件（如 ToggleSwitch），需在 `ArrangeOverride` 中手动设置：

```csharp
_waveSpiritDecorator?.Arrange(new Rect(new Point(0, 0), DesiredSize.Deflate(Margin)));
```

---

## 7. 速查清单

集成 WaveSpirit 时，请确认以下各项：

- [ ] 控件类实现了 `IWaveSpiritAwareControl` 接口
- [ ] 使用 `AddOwner<T>()` 注册了 `IsMotionEnabledProperty` 和 `IsWaveSpiritEnabledProperty`
- [ ] 使用 `AddOwner<T>()` 注册了 `WaveSpiritTypeProperty`（如果需要动态切换类型）
- [ ] 在 `OnApplyTemplate` 中通过 `NameScope.Find<WaveSpiritDecorator>(...)` 获取装饰器引用
- [ ] AXAML 模板使用 `Panel` 包裹 `WaveSpiritDecorator` 和控件主体
- [ ] `WaveSpiritDecorator` 放在 `Panel` 的第一个子元素位置
- [ ] `WaveSpiritDecorator` 的 `Name` 使用标准的 `"PART_WaveSpirit"`
- [ ] 设置了 `CornerRadius="{TemplateBinding CornerRadius}"`（对 RoundRectWave 类型）
- [ ] 设置了 `WaveType`（通过 `{TemplateBinding}` 或 Style Selector）
- [ ] 控件设置了 `ClipToBounds="False"`
- [ ] 主题中绑定了 `IsWaveSpiritEnabled` 到 `{atom:SharedTokenResource EnableWaveSpirit}`
- [ ] 主题中绑定了 `IsMotionEnabled` 到 `{atom:SharedTokenResource EnableMotion}`
- [ ] 在合适的交互事件中调用了 `_waveSpiritDecorator?.Play()`
- [ ] 触发前检查了 `IsWaveSpiritEnabled` 条件

