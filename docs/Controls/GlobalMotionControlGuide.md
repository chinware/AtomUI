# 全局动画与涟漪效果开关使用指南

## 1. 概述

AtomUI 提供应用级别的全局开关，用于统一控制所有控件的**过渡动画**（`EnableMotion`）和**点击涟漪效果**（`EnableWaveSpirit`）。开发者可在运行时随时切换这两个开关，所有已实现 `IMotionAwareControl` 或 `IWaveSpiritAwareControl` 接口的控件会自动响应。

> **前置阅读**：
> - [动画与涟漪效果控制系统设计原理](./MotionAndWaveSpiritDesign.md)
> - [自定义控件接入动画与涟漪效果开发指南](./MotionAndWaveSpiritGuide.md)

### 适用场景

| 场景 | 操作 |
|---|---|
| 无障碍模式（`prefers-reduced-motion`） | 关闭全局动画 |
| 低端设备性能优化 | 关闭全局动画 |
| 用户偏好设置 | 提供开关选项，运行时切换 |
| 仅关闭涟漪效果，保留过渡动画 | 单独关闭涟漪 |

---

## 2. Application 扩展方法（推荐方式）

`AtomUI.Controls.Shared` 提供了 `ApplicationExtensions` 类（命名空间 `AtomUI.Controls`），封装了对 `ThemeManager` 的访问，是**应用层控制动画开关的推荐入口**。

### 2.1 API 一览

```csharp
// 命名空间：AtomUI.Controls（来自 AtomUI.Controls.Shared）
public static class ApplicationExtensions
{
    // ===== 动画控制 =====
    public static bool IsMotionEnabled(this Application application);
    public static void SetMotionEnabled(this Application application, bool enabled);

    // ===== 涟漪控制 =====
    public static bool IsWaveSpiritEnabled(this Application application);
    public static void SetWaveSpiritEnabled(this Application application, bool enabled);

    // ===== 其他主题控制（参考） =====
    public static void SetDarkThemeMode(this Application application, bool enabled);
    public static void SetCompactThemeMode(this Application application, bool enabled);
    public static void SetLanguageVariant(this Application application, LanguageVariant variant);
}
```

### 2.2 基本用法

```csharp
using AtomUI.Controls;
using Avalonia;

// 获取当前应用实例
var application = Application.Current!;

// 关闭全局动画（过渡动画 + 涟漪效果全部禁用）
application.SetMotionEnabled(false);

// 重新开启全局动画
application.SetMotionEnabled(true);

// 仅关闭涟漪效果，保留过渡动画
application.SetWaveSpiritEnabled(false);

// 查询当前状态
bool motionOn = application.IsMotionEnabled();
bool waveOn   = application.IsWaveSpiritEnabled();
```

> **注意**：这些扩展方法内部通过 `AvaloniaLocator` 获取 `ThemeManager` 实例。必须在 `UseAtomUI()` 初始化完成之后才能调用。

---

## 3. 两个开关的关系

`EnableMotion` 和 `EnableWaveSpirit` 是两个**独立的** Seed Token，但在逻辑上存在层级关系：

```
EnableMotion = false  →  所有过渡动画停止 + 涟漪效果停止
EnableMotion = true   →  过渡动画恢复，涟漪是否播放取决于 EnableWaveSpirit

EnableWaveSpirit = false  →  仅涟漪效果停止，过渡动画不受影响
EnableWaveSpirit = true   →  涟漪效果恢复（前提：EnableMotion 也为 true）
```

这是因为 `IWaveSpiritAwareControl` 继承自 `IMotionAwareControl`，涟漪控件在触发 `Play()` 前会检查 `IsWaveSpiritEnabled`，而涟漪本身也依赖动画系统。因此在实践中：

| EnableMotion | EnableWaveSpirit | 过渡动画 | 涟漪效果 |
|---|---|---|---|
| `true` | `true` | ✅ 启用 | ✅ 启用 |
| `true` | `false` | ✅ 启用 | ❌ 禁用 |
| `false` | `true` | ❌ 禁用 | ❌ 禁用 |
| `false` | `false` | ❌ 禁用 | ❌ 禁用 |

> **最佳实践**：关闭动画时同步关闭涟漪，开启动画时恢复涟漪。Gallery 中的 `WorkspaceWindow` 实现了这一逻辑（见第 5 节）。

---

## 4. 内部传播机制

了解全局开关的传播链路有助于调试和排查问题。

### 4.1 传播链路

```
Application.SetMotionEnabled(false)
    │
    ▼
ThemeManager.IsMotionEnabled = false          ← AvaloniaProperty 变更
    │
    ▼
ThemeManager.OnPropertyChanged()
    → ConfigureEnableMotion()
        → globalResourceDictionary[SharedTokenKind.EnableMotion] = false   ← 修改全局 Token 资源
    │
    ▼
所有通过 DynamicResource 绑定到 SharedTokenKind.EnableMotion 的控件自动获得新值
    │
    ▼
控件的 IsMotionEnabledProperty 值变更
    → OnPropertyChanged() → ConfigureTransitions(true)
        → Transitions = null  （禁用过渡动画）
```

### 4.2 ThemeManager 核心代码

```csharp
// ThemeManager 中的属性变更处理
protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
{
    base.OnPropertyChanged(change);
    // ...
    if (change.Property == IsMotionEnabledProperty)
    {
        ConfigureEnableMotion();
    }
    else if (change.Property == IsWaveSpiritEnabledProperty)
    {
        ConfigureEnableWaveSpirit();
    }
}

// 将属性值写入全局 Token 资源字典
private void ConfigureEnableMotion()
{
    var themeResource = Resources.ThemeDictionaries[ThemeVariant];
    if (themeResource is ResourceDictionary globalResourceDictionary)
    {
        globalResourceDictionary[SharedTokenKind.EnableMotion] = IsMotionEnabled;
    }
}

private void ConfigureEnableWaveSpirit()
{
    var themeResource = Resources.ThemeDictionaries[ThemeVariant];
    if (themeResource is ResourceDictionary globalResourceDictionary)
    {
        globalResourceDictionary[SharedTokenKind.EnableWaveSpirit] = IsWaveSpiritEnabled;
    }
}
```

### 4.3 控件端的自动绑定

控件在构造函数中调用 `ConfigureMotionBindingStyle()` 或 `ConfigureWaveSpiritBindingStyle()`，内部通过 `Style` 将属性绑定到 `SharedTokenKind.EnableMotion` / `SharedTokenKind.EnableWaveSpirit` 动态资源。当 `ThemeManager` 修改全局资源字典中的值时，绑定链自动更新控件属性。

```
ThemeManager  ──写入──►  ResourceDictionary[SharedTokenKind.EnableMotion]
                                │
                         DynamicResource 绑定
                                │
                                ▼
控件 IsMotionEnabledProperty  ←──  自动更新
```

---

## 5. 完整示例：Gallery WorkspaceWindow 实现

AtomUI Gallery 的 `WorkspaceWindow` 通过菜单项提供了运行时切换动画和涟漪的完整示例。

### 5.1 AXAML 菜单定义

```xml
<atom:MenuItem Header="主题">
    <!-- 动画开关 -->
    <atom:MenuItem Header="开启动画"
                   ToggleType="CheckBox"
                   IsChecked="True"
                   Tag="{x:Static local:WindowMenuItemKind.Motion}"/>
    <!-- 涟漪开关 -->
    <atom:MenuItem Header="开启波浪动画"
                   ToggleType="CheckBox"
                   IsChecked="True"
                   Tag="{x:Static local:WindowMenuItemKind.WaveSpirit}"/>
</atom:MenuItem>
```

### 5.2 菜单事件处理

```csharp
private void HandleMenuItemCheckChanged(object? sender, RoutedEventArgs e)
{
    if (e.Source is MenuItem menuItem && menuItem.Tag is WindowMenuItemKind kind)
    {
        var application = Application.Current!;

        if (kind == WindowMenuItemKind.Motion)
        {
            // 关闭动画时，同步关闭涟漪开关的 UI 状态
            if (menuItem.Parent is MenuItem themeMenuItem)
            {
                foreach (var item in themeMenuItem.Items)
                {
                    if (item is MenuItem child &&
                        child.Tag is WindowMenuItemKind childKind &&
                        childKind == WindowMenuItemKind.WaveSpirit)
                    {
                        if (!menuItem.IsChecked)
                        {
                            child.IsChecked = false;
                        }
                    }
                }
            }
            // 调用全局 API
            application.SetMotionEnabled(menuItem.IsChecked);
        }
        else if (kind == WindowMenuItemKind.WaveSpirit)
        {
            application.SetWaveSpiritEnabled(menuItem.IsChecked);
        }
    }
}
```

**关键设计点**：

1. 使用 `ToggleType="CheckBox"` 让菜单项具备切换能力
2. 通过 `Tag` 区分不同的菜单项类型
3. 关闭动画（Motion）时，**同步将涟漪（WaveSpirit）菜单项置为 unchecked**——因为涟漪依赖动画系统
4. 最终调用 `application.SetMotionEnabled()` / `application.SetWaveSpiritEnabled()` 完成全局切换

---

## 6. 在应用启动时配置默认值

### 6.1 通过 Seed Token 默认值

`EnableMotion` 和 `EnableWaveSpirit` 在 `DesignToken.Seed` 中的默认值均为 `true`：

```csharp
[DesignTokenKind(DesignTokenKind.Seed)]
public bool EnableMotion { get; set; } = true;

[DesignTokenKind(DesignTokenKind.Seed)]
public bool EnableWaveSpirit { get; set; } = true;
```

因此，应用启动后默认所有动画和涟漪效果均为开启状态。

### 6.2 在 OnFrameworkInitializationCompleted 中修改

如果需要在启动时默认关闭动画（例如针对特定设备），可在应用初始化完成后设置：

```csharp
public override void OnFrameworkInitializationCompleted()
{
    base.OnFrameworkInitializationCompleted();

    // 在低端设备上默认关闭动画
    if (IsLowEndDevice())
    {
        Application.Current!.SetMotionEnabled(false);
    }
}
```

---

## 7. 高级场景

### 7.1 监听动画状态变化

如果需要在动画状态变化时执行自定义逻辑，可以通过 `ThemeManager` 属性监听：

```csharp
var themeManager = Application.Current!.GetThemeManager();
if (themeManager is AvaloniaObject avaloniaObject)
{
    avaloniaObject.GetObservable(ThemeManager.IsMotionEnabledProperty)
        .Subscribe(isEnabled =>
        {
            // 动画状态变化时的自定义逻辑
            Debug.WriteLine($"Motion enabled: {isEnabled}");
        });
}
```

### 7.2 与暗色模式 / 紧凑模式的组合

动画和涟漪开关与主题模式**完全独立**，可以自由组合：

```csharp
var app = Application.Current!;

// 暗色 + 紧凑 + 无动画
app.SetDarkThemeMode(true);
app.SetCompactThemeMode(true);
app.SetMotionEnabled(false);
```

切换暗色/紧凑模式不会影响动画开关状态，反之亦然。

### 7.3 主题切换时的行为

当通过 `Application.RequestedThemeVariant` 切换主题（如从 Default 切换到 Dark+Compact）时，`ThemeManager` 会重新应用当前的 `IsMotionEnabled` 和 `IsWaveSpiritEnabled` 值到新主题的资源字典中：

```csharp
// ThemeManager.ConfigureThemeVariant() 中
if (oldTheme != null)
{
    ConfigureEnableMotion();      // 将当前值写入新主题资源
    ConfigureEnableWaveSpirit();  // 将当前值写入新主题资源
}
```

因此，动画开关状态在主题切换后**保持不变**。

---

## 8. API 参考

### ApplicationExtensions（AtomUI.Controls 命名空间）

| 方法 | 说明 |
|---|---|
| `GetThemeManager(this Application)` | 获取 `IThemeManager` 实例 |
| `IsMotionEnabled(this Application)` | 查询全局动画是否开启 |
| `SetMotionEnabled(this Application, bool)` | 设置全局动画开关 |
| `IsWaveSpiritEnabled(this Application)` | 查询全局涟漪是否开启 |
| `SetWaveSpiritEnabled(this Application, bool)` | 设置全局涟漪开关 |
| `IsDarkThemeMode(this Application)` | 查询是否暗色模式 |
| `SetDarkThemeMode(this Application, bool)` | 设置暗色模式 |
| `IsCompactThemeMode(this Application)` | 查询是否紧凑模式 |
| `SetCompactThemeMode(this Application, bool)` | 设置紧凑模式 |

### IThemeManager 属性

| 属性 | 类型 | 说明 |
|---|---|---|
| `IsMotionEnabled` | `bool` | 全局动画开关 |
| `IsWaveSpiritEnabled` | `bool` | 全局涟漪开关 |
| `IsDarkThemeMode` | `bool` | 暗色模式开关 |
| `IsCompactThemeMode` | `bool` | 紧凑模式开关 |

### Seed Token

| Token | 类型 | 默认值 | 说明 |
|---|---|---|---|
| `EnableMotion` | `bool` | `true` | 全局动画开关 |
| `EnableWaveSpirit` | `bool` | `true` | 全局涟漪开关 |

---

## 9. 常见问题

### Q: 调用 `SetMotionEnabled(false)` 后部分控件动画仍然存在？

**A**: 检查该控件是否实现了 `IMotionAwareControl` 接口，以及是否在构造函数中调用了 `ConfigureMotionBindingStyle()` 或 `ConfigureWaveSpiritBindingStyle()`。只有完成了这些步骤的控件才会响应全局开关。

### Q: 关闭动画后，涟漪效果是否也会关闭？

**A**: 是的。涟漪控件在触发 `Play()` 前会检查 `IsWaveSpiritEnabled`，而 `IsWaveSpiritEnabled` 的绑定链中不直接依赖 `EnableMotion`。但在实践中，涟漪控件内部的动画引擎（`WaveSpiritDecorator` 的尺寸和透明度动画）本身就是过渡动画，当控件的 `IsMotionEnabled` 为 `false` 时，涟漪的视觉效果自然不会播放。建议在关闭 `EnableMotion` 时同步设置 `EnableWaveSpirit = false`，保持状态一致（参考 Gallery 示例）。

### Q: 可以在 AXAML 中直接控制单个控件的动画吗？

**A**: 可以。每个实现了 `IMotionAwareControl` 的控件都暴露了 `IsMotionEnabled` 属性，可以在 AXAML 中直接设置：

```xml
<!-- 禁用此按钮的过渡动画 -->
<atom:Button IsMotionEnabled="False" Content="No Animation" />
```

但请注意，直接设置会覆盖全局 Token 绑定。如果后续需要恢复全局控制，需要清除该属性的本地值。

### Q: `UseAtomUI()` 之前可以调用 `SetMotionEnabled()` 吗？

**A**: 不可以。`SetMotionEnabled()` 内部通过 `AvaloniaLocator` 获取 `ThemeManager`，而 `ThemeManager` 在 `UseAtomUI()` 中才会创建并注册。在此之前调用会导致空引用。

