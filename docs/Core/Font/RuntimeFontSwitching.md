# AtomUI 字体系统 — 运行时字体切换

> 本文档详细描述 AtomUI 中运行时切换全局字体的机制、实现原理、使用方式，以及注意事项和限制。

---

## 1. 概述

AtomUI 支持在应用运行过程中动态切换全局字体族，无需重启应用。这得益于 Avalonia 的 `ResourceDictionary` 动态资源解析机制 —— 当资源字典中的 `FontFamily` 值被修改时，所有绑定了 `{atom:SharedTokenResource FontFamily}` 的控件会自动更新字体显示。

---

## 2. 字体在主题资源中的存储

### 2.1 初始加载流程

在应用启动时，字体族 Token 经过以下流程加载到主题资源中：

```
1. DesignToken.InitSeedTokenValues()
   └─► FontFamily = "fonts:AlibabaSans#Alibaba Sans, ..."  (Seed Token 默认值)

2. Theme.Load() → Theme.BuildThemeResource()
   └─► ResourceDictionary[SharedTokenKind.FontFamily] = designToken.FontFamily

3. ThemeManager.ThemeLoaded 事件
   └─► 如果调用了 WithDefaultFontFamily()，则覆盖:
       loadedTheme.ThemeResource[SharedTokenKind.FontFamily] = userFontFamily

4. ThemeManager.SetActiveTheme()
   └─► Resources.ThemeDictionaries[themeVariant] = theme.ThemeResource
```

最终，`SharedTokenKind.FontFamily` 作为动态资源存在于当前活动主题的 `ResourceDictionary` 中。

### 2.2 控件如何消费

控件主题通过 Avalonia 的动态资源机制消费字体 Token：

```xml
<!-- WindowTheme.axaml -->
<Setter Property="FontFamily" Value="{atom:SharedTokenResource FontFamily}" />
```

`{atom:SharedTokenResource FontFamily}` 等效于 `{DynamicResource SharedTokenKind.FontFamily}`，它会在资源值变化时自动更新。

---

## 3. 启动时设置默认字体

### 3.1 使用 WithDefaultFontFamily

最简单的方式是在 `UseAtomUI()` 回调中使用 `WithDefaultFontFamily()`：

```csharp
this.UseAtomUI(builder =>
{
    builder.UseAlibabaSansFont();
    builder.UseDesktopControls();
    
    // 启动时设置默认字体
    builder.WithDefaultFontFamily("fonts:AlibabaSans#Alibaba Sans, $Default");
});
```

**API 签名：**

```csharp
public interface IThemeManagerBuilder
{
    /// <summary>
    /// 设置默认字体族（FontFamily 对象）
    /// </summary>
    void WithDefaultFontFamily(FontFamily fontFamily);
    
    /// <summary>
    /// 设置默认字体族（字符串格式，自动解析为 FontFamily）
    /// </summary>
    void WithDefaultFontFamily(string fontFamily);
}
```

### 3.2 内部实现

```csharp
// ThemeManagerBuilder.cs
public void WithDefaultFontFamily(FontFamily fontFamily)
{
    FontFamily = fontFamily;
}

public void WithDefaultFontFamily(string fontFamily)
{
    FontFamily = FontFamily.Parse(fontFamily);
}
```

设置后，在 `ApplicationExtensions.UseAtomUI()` 中注册 `ThemeLoaded` 事件处理器：

```csharp
// ApplicationExtensions.cs
var defaultFontFamily = themeManagerBuilder.FontFamily;
themeManager.ThemeLoaded += (sender, args) =>
{
    if (defaultFontFamily != null && args.Theme != null)
    {
        var loadedTheme = args.Theme;
        loadedTheme.ThemeResource[SharedTokenKind.FontFamily] = defaultFontFamily;
    }
};
```

---

## 4. 运行时动态切换字体

### 4.1 通过修改主题资源字典

运行时切换字体的核心方法是直接修改当前活动主题的 `ResourceDictionary`：

```csharp
using AtomUI.Theme;
using AtomUI.Theme.Styling;
using Avalonia;
using Avalonia.Media;

// 获取 ThemeManager 实例
var themeManager = AvaloniaLocator.Current.GetRequiredService<ThemeManager>();

// 获取当前活动主题的资源字典
if (themeManager.ActivatedTheme is { } activatedTheme)
{
    // 修改 FontFamily Token 值
    activatedTheme.ThemeResource[SharedTokenKind.FontFamily] = 
        FontFamily.Parse("fonts:CustomFont#My Custom Font, $Default");
}
```

修改后，所有使用 `{atom:SharedTokenResource FontFamily}` 绑定的控件会立即更新字体显示。

### 4.2 封装为辅助方法

推荐封装一个辅助方法：

```csharp
public static class FontSwitcher
{
    /// <summary>
    /// 运行时切换全局字体族
    /// </summary>
    /// <param name="fontFamily">新的字体族（含回退链）</param>
    public static void SwitchGlobalFont(string fontFamily)
    {
        SwitchGlobalFont(FontFamily.Parse(fontFamily));
    }
    
    /// <summary>
    /// 运行时切换全局字体族
    /// </summary>
    /// <param name="fontFamily">新的字体族</param>
    public static void SwitchGlobalFont(FontFamily fontFamily)
    {
        var themeManager = AvaloniaLocator.Current.GetRequiredService<ThemeManager>();
        
        if (themeManager.ActivatedTheme is { } activatedTheme)
        {
            activatedTheme.ThemeResource[SharedTokenKind.FontFamily] = fontFamily;
        }
    }
}
```

**使用示例：**

```csharp
// 在 ViewModel 或事件处理器中
FontSwitcher.SwitchGlobalFont("fonts:AlibabaSans#Alibaba Sans, $Default");

// 或切换到系统字体
FontSwitcher.SwitchGlobalFont("Segoe UI, PingFang SC, $Default");
```

### 4.3 在 UI 中提供字体选择

```csharp
// 示例：字体选择下拉框的事件处理
private void OnFontSelectionChanged(object sender, SelectionChangedEventArgs e)
{
    var selectedFont = (e.AddedItems[0] as FontOption)?.FontFamilyString;
    if (!string.IsNullOrEmpty(selectedFont))
    {
        FontSwitcher.SwitchGlobalFont(selectedFont);
    }
}
```

---

## 5. 字体切换与主题切换的关系

### 5.1 主题切换时字体的行为

当用户切换主题（如从亮色切换到暗色）时：

1. `ThemeManager` 加载新主题的 `ResourceDictionary`。
2. 新主题的 `FontFamily` Token 恢复为 `DesignToken.InitSeedTokenValues()` 中的默认值。
3. `ThemeLoaded` 事件触发 — 如果之前调用了 `WithDefaultFontFamily()`，会重新覆盖字体值。

**重要：** 通过 `WithDefaultFontFamily()` 设置的字体在主题切换后**会保留**（因为它注册在 `ThemeLoaded` 事件上，每次主题加载都会执行）。但通过直接修改 `ResourceDictionary` 的运行时切换**不会跨主题保留** — 切换主题后需要重新设置。

### 5.2 确保运行时字体切换跨主题保留

如果需要运行时字体切换在主题切换后依然有效，应当监听 `ThemeLoaded` 事件：

```csharp
var themeManager = AvaloniaLocator.Current.GetRequiredService<ThemeManager>();

FontFamily? currentFontOverride = null;

themeManager.ThemeLoaded += (sender, args) =>
{
    if (currentFontOverride != null && args.Theme != null)
    {
        args.Theme.ThemeResource[SharedTokenKind.FontFamily] = currentFontOverride;
    }
};

// 切换字体时同时记录
public void SwitchFont(FontFamily fontFamily)
{
    currentFontOverride = fontFamily;
    if (themeManager.ActivatedTheme is { } activatedTheme)
    {
        activatedTheme.ThemeResource[SharedTokenKind.FontFamily] = fontFamily;
    }
}
```

---

## 6. 注意事项与限制

### 6.1 字体必须已注册

运行时切换到某个字体前，该字体必须已经通过 `FontManager.AddFontCollection()` 注册。否则 Avalonia 无法解析 `fonts:` URI，字体会回退到系统默认字体。

```csharp
// 确保字体包已注册
FontManager.Current.AddFontCollection(new CustomFontCollection());

// 然后才能切换
FontSwitcher.SwitchGlobalFont("fonts:CustomFont#My Custom Font, $Default");
```

### 6.2 字号梯度不受影响

运行时修改 `FontFamily` Token 只影响字体族，**不影响字号梯度**。字号梯度（`FontSizeSM`、`FontSizeLG`、`FontSizeHeading*` 等）是从 `FontSize` Seed Token 通过算法派生的，在主题加载时已经计算完成。

如果需要运行时修改基准字号，需要同时修改所有相关的 Map Token 值，或者触发主题重新加载。

### 6.3 Popup/浮层控件

`PopupRoot` 和 `OverlayPopupHost` 控件的主题中同样绑定了 `{atom:SharedTokenResource FontFamily}`。这意味着运行时字体切换会自动影响所有浮层（如下拉菜单、工具提示、对话框等），无需额外处理。

### 6.4 性能考虑

字体切换本质上是修改 `ResourceDictionary` 中的一个值，触发 Avalonia 的样式系统重新解析受影响控件的 `FontFamily` 属性。在控件树较大的应用中可能会有短暂的布局重算，但通常不构成性能瓶颈。

### 6.5 回退链建议

始终在字体族字符串末尾包含 `$Default` 作为终极回退：

```
fonts:CustomFont#My Custom Font, Segoe UI, PingFang SC, $Default
```

这确保即使自定义字体加载失败，应用仍然可以正常显示文本。

---

## 7. 流程图：字体生命周期

```
应用启动
    │
    ▼
UseAtomUI(builder => {
    builder.UseAlibabaSansFont();         ──► FontManager 注册字体集合
    builder.WithDefaultFontFamily(...);   ──► 记录用户指定的字体族
})
    │
    ▼
ThemeManager.Configure()
    │
    ▼
ThemeManager.LoadTheme()
    ├── Theme.BuildThemeResource()
    │   └── ResourceDict[FontFamily] = DesignToken.FontFamily (Seed 默认值)
    │
    ├── ThemeLoaded 事件
    │   └── ResourceDict[FontFamily] = 用户指定字体族 (WithDefaultFontFamily)
    │
    └── 控件主题应用
        └── {atom:SharedTokenResource FontFamily} → 从 ResourceDict 解析字体
    │
    ▼
应用运行中...
    │
    ├── 用户触发字体切换
    │   └── activatedTheme.ThemeResource[FontFamily] = 新字体族
    │       └── 所有绑定控件自动更新 ✅
    │
    ├── 用户触发主题切换（如亮色 → 暗色）
    │   ├── LoadTheme() 加载新主题 ResourceDict
    │   ├── ThemeLoaded 事件 → 重新应用 WithDefaultFontFamily ✅
    │   └── 如果是运行时直接修改的 → 需要重新设置 ⚠️
    │
    ▼
应用退出
```

---

## 8. 总结

| 场景 | 方法 | 跨主题保留 |
|------|------|-----------|
| 启动时设置默认字体 | `builder.WithDefaultFontFamily(...)` | ✅ 是 |
| 运行时切换字体 | 修改 `ThemeResource[SharedTokenKind.FontFamily]` | ❌ 否（需监听 ThemeLoaded） |
| 运行时切换字体（跨主题保留） | 监听 `ThemeLoaded` + 修改 `ThemeResource` | ✅ 是 |

