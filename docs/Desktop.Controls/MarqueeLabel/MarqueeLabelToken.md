# MarqueeLabel Design Token

MarqueeLabel 使用 `MarqueeLabelToken`（Token ID: `"MarqueeLabel"`）作为组件级 Design Token。Token 控制跑马灯的默认滚动速度和循环间隔。

---

## Token 资源访问方式

在 AXAML 中使用组件级 Token：

```xml
{atom:MarqueeLabelTokenResource CycleSpace}
{atom:MarqueeLabelTokenResource DefaultSpeed}
```

在 AXAML 中使用全局共享 Token：

```xml
{atom:SharedTokenResource FontSize}
{atom:SharedTokenResource ColorText}
```

---

## 组件级 Token 一览

`MarqueeLabelToken` 定义的全部组件级 Token：

| Token 名 | 类型 | 默认值 | 来源 | 说明 |
|---|---|---|---|---|
| `CycleSpace` | `double` | `200` | 固定值 | 循环间隔——两份文本之间的视觉间距（像素）。值越大，前一份文本滚出后、后一份文本出现前的空白区域越宽 |
| `DefaultSpeed` | `double` | `150` | 固定值 | 默认滚动速度（像素/秒）。值越大滚动越快，动画时长会根据文本宽度和此速度动态计算 |

---

## Token 源码定义

```csharp
[ControlDesignToken]
internal class MarqueeLabelToken : AbstractControlDesignToken
{
    public const string ID = "MarqueeLabel";
    public static readonly ControlTokenResourceScopeProvider ScopeProvider = new(ID);

    /// <summary>
    /// 周期之间的间隔
    /// </summary>
    public double CycleSpace { get; set; }

    /// <summary>
    /// 默认速度，像素每秒
    /// </summary>
    public double DefaultSpeed { get; set; }

    public MarqueeLabelToken() : base(ID) { }

    public override void CalculateTokenValues(bool isDarkMode)
    {
        base.CalculateTokenValues(isDarkMode);
        CycleSpace   = 200;
        DefaultSpeed = 150;
    }

    protected override Type GetTokenKindType() => typeof(MarqueeLabelTokenKind);
}
```

---

## Token 与控件属性的映射关系

桌面端 `MarqueeLabel` 在构造函数中通过实例样式将 Token 值绑定到控件属性：

| Token | 映射到的控件属性 | 说明 |
|---|---|---|
| `MarqueeLabelTokenKind.CycleSpace` | `CycleSpaceProperty` | 两份文本之间的间距 |
| `MarqueeLabelTokenKind.DefaultSpeed` | `MoveSpeedProperty` | 滚动速度（像素/秒） |

```csharp
// MarqueeLabel 构造函数中的 Token 绑定
private void ConfigureInstanceStyles()
{
    var style = new Style();
    style.Add(CycleSpaceProperty, MarqueeLabelTokenKind.CycleSpace);
    style.Add(MoveSpeedProperty, MarqueeLabelTokenKind.DefaultSpeed);
    Styles.Add(style);
}
```

---

## Token 对外观的具体影响

### CycleSpace 对滚动体验的影响

`CycleSpace` 控制两份循环文本之间的空白区域宽度：

| CycleSpace 值 | 效果 |
|---|---|
| 较小值（如 `50`） | 文本紧密衔接，视觉上几乎无间隔，适合紧凑信息展示 |
| 默认值（`200`） | 适中的间隔，给用户明确的视觉分隔感 |
| 较大值（如 `500`） | 文本之间有大片空白，适合需要阅读喘息空间的场景 |

### DefaultSpeed 对滚动体验的影响

`DefaultSpeed` 控制文本水平滚动的像素速度。动画时长按以下公式动态计算：

```
duration = 4 × max(1, (textWidth + CycleSpace) / MoveSpeed × 1000) 毫秒
```

| DefaultSpeed 值 | 效果 |
|---|---|
| 较小值（如 `50`） | 缓慢滚动，适合长文本需要用户仔细阅读的场景 |
| 默认值（`150`） | 适中速度，平衡可读性与信息展示效率 |
| 较大值（如 `300`） | 快速滚动，适合短消息循环播报 |

---

## 主题中使用的全局 SharedToken

MarqueeLabel 本身不直接引用全局 SharedToken，但在被其他控件（如 `Alert`）嵌套使用时，父控件的主题会通过样式选择器为 MarqueeLabel 设置字体大小等全局 Token 值：

| Token 资源键 | 使用场景 |
|---|---|
| `FontSize` | Alert 无描述时 MarqueeLabel 的字号 |
| `FontSizeLG` | Alert 有描述时 MarqueeLabel 的字号 |
| `FontHeight` | Alert 中 MarqueeLabel 的行高 |

---

## 自定义 Token（高级用法）

如需全局覆盖 MarqueeLabel 的默认 Token 值，可在主题配置中自定义 `MarqueeLabelToken`：

```csharp
// 在 ThemeManager 配置中覆盖 Token
themeManager.ConfigureTokens(config =>
{
    config.Override<MarqueeLabelToken>(token =>
    {
        token.CycleSpace = 300;    // 加大循环间隔
        token.DefaultSpeed = 100;  // 降低默认速度
    });
});
```

也可以在 AXAML 中通过直接设置属性覆盖 Token 默认值：

```xml
<!-- 直接设置属性覆盖 Token 值 -->
<atom:MarqueeLabel Text="自定义速度文本" CycleSpace="100" MoveSpeed="200" />
```
