# 新控件适配 Space 开发指南

本文档面向 AtomUI 控件开发者，说明如何让新控件正确工作在 `Space` 和 `CompactSpace` 中。

---

## 1. Space（通用间距面板）

### 无需任何适配

`Space` 对子控件**没有接口要求**。任何 `Control` 都可以直接作为 `Space` 的子项：

```xml
<atom:Space Orientation="Horizontal" SizeType="Middle">
    <atom:Button>按钮</atom:Button>
    <atom:TextBlock>文本</atom:TextBlock>
    <Border Background="Gray" Width="50" Height="50"/>
    <YourCustomControl />   <!-- 直接使用，无需适配 -->
</atom:Space>
```

Space 仅负责间距排列，不修改子控件的任何属性。

---

## 2. CompactSpace（紧凑组合面板）

### 必须实现 ICompactSpaceAware 接口

`CompactSpace` 会在添加子控件时强制检查 `ICompactSpaceAware` 接口：

```csharp
private static void EnsureCompactSpaceItem(object item)
{
    if (item is not ICompactSpaceAware)
    {
        throw new ArgumentException($"{item.GetType().FullName} is not ICompactSpaceAware.");
    }
}
```

如果子控件未实现此接口，会在运行时抛出异常。

---

## 3. 适配 CompactSpace 的完整步骤

### 步骤 1：声明实现 ICompactSpaceAware 接口

```csharp
using AtomUI.Controls;
using AtomUI.Desktop.Controls;
using Avalonia;
using Avalonia.Layout;

public class MyControl : TemplatedControl,
                         ISizeTypeAware,        // 建议实现，以支持 SizeType 传递
                         ICompactSpaceAware     // 必须实现
{
    // ...
}
```

### 步骤 2：注册 CompactSpace 相关的 StyledProperty

使用 `CompactSpaceAwareControlProperty` 的 `AddOwner` 模式注册三个内部属性：

```csharp
#region CompactSpace 相关内部属性

internal static readonly StyledProperty<SpaceItemPosition?> CompactSpaceItemPositionProperty = 
    CompactSpaceAwareControlProperty.CompactSpaceItemPositionProperty.AddOwner<MyControl>();

internal static readonly StyledProperty<Orientation> CompactSpaceOrientationProperty = 
    CompactSpaceAwareControlProperty.CompactSpaceOrientationProperty.AddOwner<MyControl>();

internal static readonly StyledProperty<bool> IsUsedInCompactSpaceProperty = 
    CompactSpaceAwareControlProperty.IsUsedInCompactSpaceProperty.AddOwner<MyControl>();

internal SpaceItemPosition? CompactSpaceItemPosition
{
    get => GetValue(CompactSpaceItemPositionProperty);
    set => SetValue(CompactSpaceItemPositionProperty, value);
}

internal Orientation CompactSpaceOrientation
{
    get => GetValue(CompactSpaceOrientationProperty);
    set => SetValue(CompactSpaceOrientationProperty, value);
}

internal bool IsUsedInCompactSpace
{
    get => GetValue(IsUsedInCompactSpaceProperty);
    set => SetValue(IsUsedInCompactSpaceProperty, value);
}

#endregion
```

### 步骤 3：声明 EffectiveCornerRadius 属性

这是 CompactSpace 圆角裁剪的关键——控件的主题 AXAML 中应绑定 `EffectiveCornerRadius` 而不是 `CornerRadius`：

```csharp
internal static readonly DirectProperty<MyControl, CornerRadius> EffectiveCornerRadiusProperty =
    AvaloniaProperty.RegisterDirect<MyControl, CornerRadius>(
        nameof(EffectiveCornerRadius),
        o => o.EffectiveCornerRadius,
        (o, v) => o.EffectiveCornerRadius = v);

private CornerRadius _effectiveCornerRadius;

internal CornerRadius EffectiveCornerRadius
{
    get => _effectiveCornerRadius;
    set => SetAndRaise(EffectiveCornerRadiusProperty, ref _effectiveCornerRadius, value);
}
```

### 步骤 4：监听属性变化并更新有效圆角

在 `OnPropertyChanged` 中监听圆角和 CompactSpace 位置/方向变化：

```csharp
static MyControl()
{
    // 确保 CompactSpace 属性变化触发重新测量/排列
    AffectsMeasure<MyControl>(CompactSpaceItemPositionProperty, CompactSpaceOrientationProperty);
}

protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
{
    base.OnPropertyChanged(change);
    
    if (change.Property == CornerRadiusProperty ||
        change.Property == CompactSpaceItemPositionProperty ||
        change.Property == CompactSpaceOrientationProperty)
    {
        ConfigureEffectiveCornerRadius();
    }
}

private void ConfigureEffectiveCornerRadius()
{
    EffectiveCornerRadius = CompactSpace.CalculateEffectiveCornerRadius(
        CornerRadius,
        IsUsedInCompactSpace,
        CompactSpaceItemPosition,
        CompactSpaceOrientation);
}
```

### 步骤 5：实现 ICompactSpaceAware 接口方法

```csharp
void ICompactSpaceAware.NotifyPositionChange(SpaceItemPosition? position)
{
    IsUsedInCompactSpace     = position != null;
    CompactSpaceItemPosition = position;
}

void ICompactSpaceAware.NotifyOrientationChange(Orientation orientation)
{
    CompactSpaceOrientation = orientation;
}

double ICompactSpaceAware.GetBorderThickness()
{
    // 返回主轴方向的边框厚度，用于计算偏移量消除边框重叠
    return CompactSpaceOrientation == Orientation.Horizontal 
        ? BorderThickness.Left 
        : BorderThickness.Top;
}
```

#### 可选方法覆盖

```csharp
// 如果控件在 CompactSpace 中应始终保持高 ZIndex（如 Primary 样式）
bool ICompactSpaceAware.IsAlwaysActiveZIndex()
{
    return false;  // 默认为 false；根据业务逻辑判断
}

// 如果控件不应参与 Focus/Hover 的 ZIndex 管理（如纯装饰控件）
bool ICompactSpaceAware.IgnoreZIndexChange()
{
    return false;  // 默认为 false
}
```

### 步骤 6：更新控件主题 AXAML

在控件的主题模板中，**使用 `EffectiveCornerRadius` 代替 `CornerRadius`**：

```xml
<!-- ❌ 错误：直接使用 CornerRadius 会导致 CompactSpace 中圆角裁剪失效 -->
<Border CornerRadius="{TemplateBinding CornerRadius}" ... />

<!-- ✅ 正确：使用 EffectiveCornerRadius -->
<Border CornerRadius="{TemplateBinding EffectiveCornerRadius}" ... />
```

---

## 4. 各方法的行为约定

### 4.1 NotifyPositionChange

CompactSpace 在以下时机调用此方法：
- 子项集合变化（添加/删除/移动）
- 方向改变触发重新计算

传入的 `SpaceItemPosition` 是 Flags 枚举：

| position 值 | 含义 | 圆角策略 |
|---|---|---|
| `First` | 排列中的第一个 | 保留起始侧圆角 |
| `Middle` | 中间位置 | 全部圆角清零 |
| `Last` | 排列中的最后一个 | 保留结束侧圆角 |
| `First \| Last` | 唯一的子项 | 保留全部圆角 |
| `null` | 不在 CompactSpace 中 | 使用原始圆角 |

**控件的响应**：将 `position` 值存入对应的 StyledProperty，后续在 `OnPropertyChanged` 中触发 `ConfigureEffectiveCornerRadius()`。

### 4.2 NotifyOrientationChange

传入当前 CompactSpace 的排列方向。控件需要保存此值，因为圆角裁剪策略依赖方向：
- 水平方向：裁剪左右两侧圆角
- 垂直方向：裁剪上下两侧圆角

### 4.3 GetBorderThickness

返回控件在主轴方向上的边框厚度。CompactSpace 使用此值来计算偏移量，消除相邻控件的重复边框。

**注意事项**：
- 如果控件无边框（如 Borderless 样式），应返回 `0.0`
- 如果控件的边框厚度随样式变化，需要返回当前实际值

```csharp
double ICompactSpaceAware.GetBorderThickness()
{
    // 如果控件有多种样式变体，按条件返回
    if (StyleVariant == InputControlStyleVariant.Borderless)
        return 0.0;
    
    return CompactSpaceOrientation == Orientation.Horizontal 
        ? BorderThickness.Left 
        : BorderThickness.Top;
}
```

### 4.4 IsAlwaysActiveZIndex

当返回 `true` 时，控件在 CompactSpace 中始终保持高 ZIndex（1000）。典型场景：

- `Button` 的 Primary 类型——其填充背景可能遮盖相邻控件的边框
- 有独立视觉层次的特殊控件

### 4.5 IgnoreZIndexChange

当返回 `true` 时，CompactSpace 不会为此控件注册 Focus/Hover 事件来管理 ZIndex。典型场景：

- `CompactSpaceAddOn`——纯装饰性标签，不需要交互反馈

---

## 5. SizeType 传递

如果控件实现了 `ISizeTypeAware`，CompactSpace 会自动将自身的 `SizeType` 绑定到子控件：

```csharp
// CompactSpace 中的自动绑定逻辑
if (target is ISizeTypeAware)
{
    target[!SizeTypeProperty] = this[!SizeTypeProperty];
}
```

因此建议同时实现 `ISizeTypeAware`，确保控件在 CompactSpace 中自动跟随尺寸档位变化。

---

## 6. CompactSpaceSize 尺寸控制

开发者可以通过附加属性 `CompactSpace.ItemSize` 控制子项在布局中的空间分配：

```xml
<!-- Auto（默认）：按子项自身尺寸 -->
<atom:LineEdit />

<!-- 固定像素 -->
<atom:LineEdit atom:CompactSpace.ItemSize="200"/>

<!-- Star 比例值 -->
<atom:LineEdit atom:CompactSpace.ItemSize="3*"/>
```

如果新控件需要动态改变自身在 CompactSpace 中的尺寸分配，可以在控件内部修改 `CompactSpace.ItemSize` 附加属性值。

---

## 7. 使用 CompactSpaceFiller 占位

`CompactSpaceFiller` 用于在 CompactSpace 末尾填充剩余空间：

```xml
<atom:CompactSpace Orientation="Horizontal" HorizontalAlignment="Stretch">
    <atom:LineEdit atom:CompactSpace.ItemSize="3*"/>
    <atom:Button ButtonType="Primary">Submit</atom:Button>
    <atom:CompactSpaceFiller atom:CompactSpace.ItemSize="5*"/>  <!-- 填充剩余空间 -->
</atom:CompactSpace>
```

**约束**：
- CompactSpaceFiller 必须是最后一个子项
- 最多只能有一个 CompactSpaceFiller
- 违反以上规则会抛出 `InvalidSpaceFillerUsageException`

---

## 8. 完整实现示例

以下是一个假设的 `MySearchBox` 控件适配 CompactSpace 的完整实现：

```csharp
using AtomUI.Controls;
using AtomUI.Desktop.Controls;
using Avalonia;
using Avalonia.Controls.Primitives;
using Avalonia.Layout;

public class MySearchBox : TemplatedControl,
                           ISizeTypeAware,
                           ICompactSpaceAware
{
    #region 公共属性
    
    public static readonly StyledProperty<SizeType> SizeTypeProperty =
        SizeTypeControlProperty.SizeTypeProperty.AddOwner<MySearchBox>();

    public SizeType SizeType
    {
        get => GetValue(SizeTypeProperty);
        set => SetValue(SizeTypeProperty, value);
    }
    
    #endregion

    #region CompactSpace 内部属性
    
    internal static readonly DirectProperty<MySearchBox, CornerRadius> EffectiveCornerRadiusProperty =
        AvaloniaProperty.RegisterDirect<MySearchBox, CornerRadius>(
            nameof(EffectiveCornerRadius),
            o => o.EffectiveCornerRadius,
            (o, v) => o.EffectiveCornerRadius = v);
    
    internal static readonly StyledProperty<SpaceItemPosition?> CompactSpaceItemPositionProperty = 
        CompactSpaceAwareControlProperty.CompactSpaceItemPositionProperty.AddOwner<MySearchBox>();
    
    internal static readonly StyledProperty<Orientation> CompactSpaceOrientationProperty = 
        CompactSpaceAwareControlProperty.CompactSpaceOrientationProperty.AddOwner<MySearchBox>();
    
    internal static readonly StyledProperty<bool> IsUsedInCompactSpaceProperty = 
        CompactSpaceAwareControlProperty.IsUsedInCompactSpaceProperty.AddOwner<MySearchBox>();

    private CornerRadius _effectiveCornerRadius;

    internal CornerRadius EffectiveCornerRadius
    {
        get => _effectiveCornerRadius;
        set => SetAndRaise(EffectiveCornerRadiusProperty, ref _effectiveCornerRadius, value);
    }
    
    internal SpaceItemPosition? CompactSpaceItemPosition
    {
        get => GetValue(CompactSpaceItemPositionProperty);
        set => SetValue(CompactSpaceItemPositionProperty, value);
    }
    
    internal Orientation CompactSpaceOrientation
    {
        get => GetValue(CompactSpaceOrientationProperty);
        set => SetValue(CompactSpaceOrientationProperty, value);
    }
    
    internal bool IsUsedInCompactSpace
    {
        get => GetValue(IsUsedInCompactSpaceProperty);
        set => SetValue(IsUsedInCompactSpaceProperty, value);
    }
    
    #endregion

    static MySearchBox()
    {
        AffectsMeasure<MySearchBox>(
            CompactSpaceItemPositionProperty, 
            CompactSpaceOrientationProperty);
    }

    public MySearchBox()
    {
        this.RegisterTokenResourceScope(MySearchBoxToken.ScopeProvider);
    }

    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        base.OnPropertyChanged(change);
        
        if (change.Property == CornerRadiusProperty ||
            change.Property == CompactSpaceItemPositionProperty ||
            change.Property == CompactSpaceOrientationProperty)
        {
            ConfigureEffectiveCornerRadius();
        }
    }

    protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
    {
        base.OnApplyTemplate(e);
        ConfigureEffectiveCornerRadius();
    }

    private void ConfigureEffectiveCornerRadius()
    {
        EffectiveCornerRadius = CompactSpace.CalculateEffectiveCornerRadius(
            CornerRadius,
            IsUsedInCompactSpace,
            CompactSpaceItemPosition,
            CompactSpaceOrientation);
    }

    #region ICompactSpaceAware 实现
    
    void ICompactSpaceAware.NotifyPositionChange(SpaceItemPosition? position)
    {
        IsUsedInCompactSpace     = position != null;
        CompactSpaceItemPosition = position;
    }

    void ICompactSpaceAware.NotifyOrientationChange(Orientation orientation)
    {
        CompactSpaceOrientation = orientation;
    }

    double ICompactSpaceAware.GetBorderThickness()
    {
        return CompactSpaceOrientation == Orientation.Horizontal 
            ? BorderThickness.Left 
            : BorderThickness.Top;
    }
    
    #endregion
}
```

---

## 9. 验证方法

### 9.1 Gallery 可视化验证

1. 在 `controlgallery/AtomUIGallery/ShowCases/Views/Layout/SpaceShowCase.axaml` 中添加测试用例
2. 运行 Gallery 应用（`controlgallery/AtomUIGallery.Desktop/`）
3. 导航到 Layout → Space 页面

### 9.2 CompactSpace 验证检查清单

| 检查项 | 验证方法 |
|---|---|
| 控件可以放入 CompactSpace | 无运行时异常 |
| 圆角裁剪正确 | 首项保留起始圆角，末项保留结束圆角，中间项无圆角 |
| 边框不重叠 | 相邻控件之间无双倍边框线 |
| Focus/Hover ZIndex 正确 | 聚焦/悬停时控件边框完整可见，不被相邻控件遮盖 |
| SizeType 传递 | CompactSpace 的 `SizeType` 变化时，子控件同步变化 |
| 水平/垂直方向 | 分别测试两个方向，圆角裁剪方向正确 |
| 与其他控件组合 | 与 Button、LineEdit 等已适配控件混合使用 |

### 9.3 示例 AXAML 测试模板

```xml
<!-- 水平方向测试 -->
<atom:CompactSpace Orientation="Horizontal" HorizontalAlignment="Stretch">
    <atom:LineEdit Text="前缀" atom:CompactSpace.ItemSize="3*"/>
    <MyControl />
    <atom:Button ButtonType="Primary">提交</atom:Button>
    <atom:CompactSpaceFiller atom:CompactSpace.ItemSize="5*"/>
</atom:CompactSpace>

<!-- 垂直方向测试 -->
<atom:CompactSpace Orientation="Vertical">
    <atom:Button>按钮 1</atom:Button>
    <MyControl />
    <atom:Button>按钮 3</atom:Button>
</atom:CompactSpace>

<!-- SizeType 变化测试 -->
<atom:CompactSpace Orientation="Horizontal" SizeType="Small">
    <MyControl />
    <atom:LineEdit Text="input" atom:CompactSpace.ItemSize="3*"/>
</atom:CompactSpace>
```

---

## 10. 常见问题

### Q: 控件放入 CompactSpace 后圆角没有变化？

确认以下几点：
1. 控件的主题 AXAML 中使用了 `{TemplateBinding EffectiveCornerRadius}` 而非 `{TemplateBinding CornerRadius}`
2. `OnApplyTemplate` 中调用了 `ConfigureEffectiveCornerRadius()`
3. `OnPropertyChanged` 中正确监听了 `CompactSpaceItemPositionProperty` 和 `CompactSpaceOrientationProperty`

### Q: 控件在 CompactSpace 中边框出现双倍粗细？

检查 `GetBorderThickness()` 返回值是否正确：
- 该方法应返回控件在主轴方向的实际边框厚度
- 如果控件无边框（Borderless 样式），应返回 `0.0`

### Q: 控件的 ZIndex 行为异常？

- 如果控件不需要参与 ZIndex 管理，让 `IgnoreZIndexChange()` 返回 `true`
- 如果控件需要始终保持高 ZIndex，让 `IsAlwaysActiveZIndex()` 返回 `true`
- 否则保持默认返回 `false`

