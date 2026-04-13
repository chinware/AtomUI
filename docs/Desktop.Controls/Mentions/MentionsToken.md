# Mentions Design Token

Mentions 使用 `MentionsToken`（Token ID: `"Mentions"`）作为组件级 Design Token。Token 控制候选弹窗的内间距、选项高度和最小宽度，所有值均从全局 `SharedToken` 派生。

---

## Token 资源访问方式

在 AXAML 中使用组件级 Token：

```xml
{atom:MentionsTokenResource OptionHeight}
{atom:MentionsTokenResource PopupContentPadding}
{atom:MentionsTokenResource MinPopupWidth}
```

在 AXAML 中使用全局共享 Token：

```xml
{atom:SharedTokenResource ColorPrimary}
{atom:SharedTokenResource ControlHeight}
{atom:SharedTokenResource ColorBgElevated}
```

---

## 组件级 Token 一览

`MentionsToken` 定义的全部组件级 Token：

| Token 名 | 类型 | 默认值 | 来源 | 说明 |
|---|---|---|---|---|
| `OptionHeight` | `double` | `SharedToken.ControlHeight` | `SharedToken.ControlHeight` | 候选项行高，决定弹窗中每个选项的高度 |
| `PopupContentPadding` | `Thickness` | `SharedToken.UniformlyPaddingXXS / 2` | `SharedToken.UniformlyPaddingXXS` | 弹窗内容内间距 |
| `MinPopupWidth` | `double` | `120` | 固定值 | 候选列表弹窗的最小宽度（像素） |

---

## Token 源码定义

```csharp
[ControlDesignToken]
internal class MentionsToken : AbstractControlDesignToken
{
    public const string ID = "Mentions";
    public static readonly ControlTokenResourceScopeProvider ScopeProvider = new(ID);

    /// <summary>
    /// 菜单内容边距
    /// </summary>
    public Thickness PopupContentPadding { get; set; }

    /// <summary>
    /// 选项高度
    /// </summary>
    public double OptionHeight { get; set; }

    /// <summary>
    /// 候选列表弹窗最小宽度
    /// </summary>
    public double MinPopupWidth { get; set; }

    public MentionsToken() : base(ID) { }

    public override void CalculateTokenValues(bool isDarkMode)
    {
        base.CalculateTokenValues(isDarkMode);
        OptionHeight        = SharedToken.ControlHeight;
        PopupContentPadding = new Thickness(SharedToken.UniformlyPaddingXXS / 2);
        MinPopupWidth       = 120;
    }

    protected override Type GetTokenKindType() => typeof(MentionsTokenKind);
}
```

---

## Token 与控件属性的映射关系

`Mentions` 的 ControlTheme 中通过 Setter 将 Token 值绑定到控件内部属性：

| Token | AXAML 引用 | 映射到的控件属性 | 说明 |
|---|---|---|---|
| `MentionsTokenKind.OptionHeight` | `{atom:MentionsTokenResource OptionHeight}` | `ItemHeight` | 单个候选项的高度 |
| `MentionsTokenKind.PopupContentPadding` | `{atom:MentionsTokenResource PopupContentPadding}` | `PopupContentPadding` | 弹窗内容的内间距 |
| `MentionsTokenKind.MinPopupWidth` | `{atom:MentionsTokenResource MinPopupWidth}` | `MinPopupWidth` | 弹窗最小宽度 |

弹窗最大高度通过以下公式计算：

```
MaxPopupHeight = ItemHeight × DisplayCandidateCount + PopupContentPadding.Top + PopupContentPadding.Bottom
```

---

## Token 对外观的具体影响

### OptionHeight 对弹窗的影响

`OptionHeight` 决定每个候选项的行高，直接影响弹窗的整体高度：

| OptionHeight 值 | 效果 |
|---|---|
| 较小值 | 紧凑的候选列表，单屏可见更多项 |
| 默认值（`SharedToken.ControlHeight`，约 32px） | 与全局控件高度一致，保证视觉统一 |
| 较大值 | 宽松的候选列表，每项有更多视觉呼吸空间 |

### PopupContentPadding 对弹窗的影响

`PopupContentPadding` 控制弹窗内容区域与边框之间的间距：

| PopupContentPadding 值 | 效果 |
|---|---|
| 较小值 | 候选列表紧贴弹窗边框 |
| 默认值 | 微小的内间距，提供视觉层次感 |

### MinPopupWidth 对弹窗的影响

`MinPopupWidth` 确保候选弹窗在候选项文本较短时仍保持合理的宽度：

| MinPopupWidth 值 | 效果 |
|---|---|
| 默认值（`120`） | 弹窗至少 120px 宽，避免过窄导致内容截断 |

---

## 主题中使用的全局 SharedToken

Mentions 的 ControlTheme 还直接引用了以下全局 `SharedToken`：

| Token 资源键 | AXAML 引用 | 使用场景 |
|---|---|---|
| `EnableMotion` | `{atom:SharedTokenResource EnableMotion}` | 全局动画开关，控制 `IsMotionEnabled` 默认值 |
| `BoxShadowsSecondary` | `{atom:SharedTokenResource BoxShadowsSecondary}` | 弹窗阴影 |
| `ColorBgElevated` | `{atom:SharedTokenResource ColorBgElevated}` | 弹窗背景色 |
| `MarginLG` | `{atom:SharedTokenResource MarginLG}` | 加载指示器的外间距 |

此外，内部 `MentionTextArea`（继承自 `TextArea`）还消费 `TextArea` 相关的全局 Token，包括：

| Token 资源键 | 使用场景 |
|---|---|
| `ColorPrimary` / `ColorPrimaryHover` | 焦点态边框颜色 |
| `ColorBorder` | 默认态边框颜色 |
| `ColorError` / `ColorWarning` | 验证状态边框颜色 |
| `ColorBgContainer` | 输入框背景色 |
| `ColorTextPlaceholder` | 占位符文本颜色 |
| `ControlHeight` / `ControlHeightSM` / `ControlHeightLG` | 不同尺寸的控件高度 |
| `BorderRadius` / `BorderRadiusSM` / `BorderRadiusLG` | 不同尺寸的圆角 |
| `FontSize` | 字体大小 |

---

## 自定义 Token（高级用法）

在 AXAML 中通过直接设置属性覆盖 Token 默认值：

```xml
<!-- 调整显示候选项数量（影响弹窗最大高度） -->
<atom:Mentions DisplayCandidateCount="5" />
```

> **注意**：`OptionHeight`、`PopupContentPadding`、`MinPopupWidth` 为内部属性，不建议外部直接覆盖。如需全局自定义，应通过 ThemeManager 的 Token 配置机制。
