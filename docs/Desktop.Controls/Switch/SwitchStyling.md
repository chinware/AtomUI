# Switch 自定义样式指南

ToggleSwitch 的视觉表现通过 `ControlTheme` + Design Token 系统控制。以下介绍常见的自定义方式。

> 📖 ToggleSwitch 在 Gallery 中的使用示例可参考 `controlgallery/AtomUIGallery/ShowCases/Views/Feedback/DrawerShowCase.axaml` 等多个演示页面。

---

## 1. 通过属性直接控制

```xml
<!-- 基本开关 -->
<atom:ToggleSwitch />

<!-- 默认开启 -->
<atom:ToggleSwitch IsChecked="True" />

<!-- 不同尺寸 -->
<atom:ToggleSwitch SizeType="Middle" />
<atom:ToggleSwitch SizeType="Small" />

<!-- 带文字 -->
<atom:ToggleSwitch OnContent="开" OffContent="关" />

<!-- 带图标 -->
<atom:ToggleSwitch
    OnContent="{antdicons:AntDesignIconProvider Kind=CheckOutlined}"
    OffContent="{antdicons:AntDesignIconProvider Kind=CloseOutlined}" />

<!-- 加载中 -->
<atom:ToggleSwitch IsLoading="True" />

<!-- 禁用 -->
<atom:ToggleSwitch IsEnabled="False" />
```

---

## 2. 通过 Style 覆盖

### 全局统一间距

```xml
<Window.Styles>
    <Style Selector="atom|ToggleSwitch">
        <Setter Property="Margin" Value="0,0,8,0" />
    </Style>
</Window.Styles>
```

### 按状态定制

```xml
<!-- 选中时使用绿色 -->
<Style Selector="atom|ToggleSwitch:checked">
    <Setter Property="GrooveBackground" Value="Green" />
</Style>
```

---

## 3. 控制动画行为

```xml
<!-- 禁用滑块过渡动画 -->
<atom:ToggleSwitch IsMotionEnabled="False" />

<!-- 禁用切换波纹 -->
<atom:ToggleSwitch IsWaveSpiritEnabled="False" />
```

---

## 样式选择器速查

> 说明：AXAML 中使用 `atom|ToggleSwitch` 引用开关控件。

### 基础选择器

| 选择器 | 说明 |
|---|---|
| `atom\|ToggleSwitch` | 匹配所有 ToggleSwitch 实例 |

### 按状态伪类选择

| 选择器 | 说明 |
|---|---|
| `atom\|ToggleSwitch:checked` | 开启状态 |
| `atom\|ToggleSwitch:unchecked` | 关闭状态 |
| `atom\|ToggleSwitch:pointerover` | 鼠标悬浮 |
| `atom\|ToggleSwitch:pressed` | 按下 |
| `atom\|ToggleSwitch:disabled` | 禁用 |

### 按属性选择

| 选择器 | 说明 |
|---|---|
| `atom\|ToggleSwitch[SizeType=Small]` | 小号开关 |
| `atom\|ToggleSwitch[IsLoading=True]` | 加载中的开关 |

### 组合选择器示例

| 选择器 | 说明 |
|---|---|
| `atom\|ToggleSwitch:checked:not(:disabled)` | 非禁用的开启状态开关 |
| `atom\|ToggleSwitch[SizeType=Small]:checked` | 小号 + 开启态 |
