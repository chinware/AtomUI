# Rate 自定义样式指南

Rate 的视觉表现通过 `ControlTheme` + Design Token 系统控制。以下介绍常见自定义方式。

---

## 1. 使用属性直接控制

最简单的方式是通过 Rate 的公共属性来控制外观：

```xml
<!-- 基本评分（默认 5 颗星，默认值 0） -->
<atom:Rate />

<!-- 半星模式，默认 3.5 分 -->
<atom:Rate IsAllowHalf="True" DefaultValue="3.5" />

<!-- 只读模式（展示已有评分） -->
<atom:Rate DefaultValue="4" IsEnabled="False" />

<!-- 禁止清除 -->
<atom:Rate DefaultValue="3" IsAllowClear="False" />

<!-- 自定义字符 — 图标 -->
<atom:Rate Character="{antdicons:AntDesignIconProvider HeartOutlined}" />

<!-- 自定义字符 — 字母 -->
<atom:Rate Character="A" />

<!-- 自定义字符 — 汉字 -->
<atom:Rate Character="秦" />

<!-- 自定义星星数量 -->
<atom:Rate Count="10" DefaultValue="7" />
```

不同尺寸：

```xml
<atom:Rate SizeType="Large" DefaultValue="3" />
<atom:Rate SizeType="Middle" DefaultValue="3" />
<atom:Rate SizeType="Small" DefaultValue="3" />
```

自定义颜色：

```xml
<!-- 自定义选中色（红色爱心） -->
<atom:Rate Character="{antdicons:AntDesignIconProvider HeartOutlined}"
           StarColor="Red"
           DefaultValue="3" />

<!-- 自定义背景色 -->
<atom:Rate StarBgColor="LightGray" DefaultValue="2" />
```

> 📖 完整示例参考：`controlgallery/AtomUIGallery/ShowCases/Views/DataEntry/RateShowCase.axaml`

---

## 2. 通过 Style 覆盖样式

可以在 AXAML 中使用 `Style` 对 Rate 进行全局或局部样式覆盖：

### 全局统一尺寸

```xml
<Window.Styles>
    <Style Selector="atom|Rate">
        <Setter Property="SizeType" Value="Large" />
    </Style>
</Window.Styles>
```

### 全局统一颜色

```xml
<Window.Styles>
    <!-- 所有评分使用红色 -->
    <Style Selector="atom|Rate">
        <Setter Property="StarColor" Value="#ff4d4f" />
    </Style>
</Window.Styles>
```

### 禁用态自定义样式

```xml
<Style Selector="atom|Rate:disabled">
    <Setter Property="Opacity" Value="0.5" />
</Style>
```

### 定制星星间距

通过访问内部 `RateItemsControl` 的 StackPanel 模板部件：

```xml
<Style Selector="atomc|RateItemsControl /template/ StackPanel#ItemsLayout">
    <Setter Property="Spacing" Value="12" />
</Style>
```

---

## 3. 通过 ControlTheme 完全替换主题

如果需要彻底替换 Rate 的模板和样式，可以定义自定义 `ControlTheme`：

```xml
<ControlTheme x:Key="MyCustomRate" TargetType="atom:Rate">
    <Setter Property="Template">
        <ControlTemplate>
            <Border Background="{TemplateBinding Background}">
                <atomc:RateItemsControl Name="PART_RateItems"
                                        Character="{TemplateBinding Character}"
                                        StarColor="{TemplateBinding StarColor}"
                                        StarBgColor="{TemplateBinding StarBgColor}"
                                        IsAllowClear="{TemplateBinding IsAllowClear}"
                                        IsAllowHalf="{TemplateBinding IsAllowHalf}"
                                        IsMotionEnabled="{TemplateBinding IsMotionEnabled}" />
            </Border>
        </ControlTemplate>
    </Setter>
</ControlTheme>

<!-- 使用 -->
<atom:Rate Theme="{StaticResource MyCustomRate}" />
```

> ⚠️ 注意：模板中必须保留 `PART_RateItems`（`RateItemsControl` 类型），否则控件将无法正常工作。

---

## 4. 控制动画行为

```xml
<!-- 禁用悬浮缩放动画 -->
<atom:Rate IsMotionEnabled="False" DefaultValue="3" />
```

---

## 样式选择器速查

> 说明：Avalonia 样式选择器中使用 `atom|Rate` 语法引用 `atom` XML 命名空间下的 `Rate` 类型，其中 `|` 是命名空间分隔符。

### Rate 选择器

| 选择器 | 说明 |
|---|---|
| `atom\|Rate` | 匹配所有 Rate 实例 |
| `atom\|Rate:disabled` | 匹配禁用（只读）态的 Rate |
| `atom\|Rate[SizeType=Large]` | 匹配大号 Rate |
| `atom\|Rate[SizeType=Small]` | 匹配小号 Rate |
| `atom\|Rate[IsAllowHalf=True]` | 匹配启用半星的 Rate |

### 内部模板部件选择器

| 选择器 | 说明 |
|---|---|
| `atom\|Rate /template/ atomc\|RateItemsControl#PART_RateItems` | 星星集合容器 |
| `atom\|Rate /template/ Border#Frame` | Rate 根框架 |
| `atomc\|RateItemsControl /template/ StackPanel#ItemsLayout` | 星星水平排列容器（可定制间距） |

### RateItem 选择器（内部控件）

| 选择器 | 说明 |
|---|---|
| `atomc\|RateItem` | 匹配所有 RateItem |
| `atomc\|RateItem:pointerover` | 匹配鼠标悬浮的 RateItem |
| `atomc\|RateItem[SizeType=Large]` | 匹配大号 RateItem |
| `atomc\|RateItem[SizeType=Middle]` | 匹配中号 RateItem |
| `atomc\|RateItem[SizeType=Small]` | 匹配小号 RateItem |
| `atomc\|RateItem[SelectedState=FullSelected]` | 匹配整星选中的 RateItem |
| `atomc\|RateItem[SelectedState=HalfSelected]` | 匹配半星选中的 RateItem |
| `atomc\|RateItem[SelectedState=None]` | 匹配未选中的 RateItem |

### 注意事项

- `RateItem` 和 `RateItemsControl` 是内部控件（`internal`），使用 `atomc` 命名空间前缀（`xmlns:atomc="https://atomui.net/common-controls"`）。
- 悬浮缩放效果通过 `RenderTransform` 实现，如需自定义缩放比例，需通过 Token 覆盖。
