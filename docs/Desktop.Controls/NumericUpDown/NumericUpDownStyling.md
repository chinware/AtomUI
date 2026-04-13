# NumericUpDown 自定义样式指南

NumericUpDown 的视觉表现通过 `ControlTheme` + Design Token 系统控制。以下介绍几种常见的自定义方式。

---

## 1. 使用属性直接控制

最简单的方式是通过 NumericUpDown 的公共属性来控制外观：

```xml
<!-- 不同尺寸 -->
<atom:NumericUpDown SizeType="Large" Value="3" />
<atom:NumericUpDown SizeType="Middle" Value="3" />
<atom:NumericUpDown SizeType="Small" Value="3" />

<!-- 不同样式变体 -->
<atom:NumericUpDown StyleVariant="Outline" Value="3" />
<atom:NumericUpDown StyleVariant="Filled" Value="3" />
<atom:NumericUpDown StyleVariant="Borderless" Value="3" />

<!-- 验证状态 -->
<atom:NumericUpDown Status="Error" PlaceholderText="Error" />
<atom:NumericUpDown Status="Warning" PlaceholderText="Warning" />

<!-- 带前后置标签 -->
<atom:NumericUpDown LeftAddOn="http://" RightAddOn=".com" Value="3" />

<!-- 带内部前后置内容 -->
<atom:NumericUpDown InnerLeftContent="￥" InnerRightContent="RMB" />

<!-- 带图标前缀 -->
<atom:NumericUpDown
    InnerLeftContent="{antdicons:AntDesignIconProvider Kind=UserOutlined, FillBrush=#D7D7D7}"
    InnerRightContent="{antdicons:AntDesignIconProvider Kind=InfoCircleOutlined, FillBrush=#8C8C8C}" />

<!-- 启用清除按钮 -->
<atom:NumericUpDown IsAllowClear="True" PlaceholderText="input with clear icon" />
```

> 📖 完整示例参考：`controlgallery/AtomUIGallery/ShowCases/Views/DataEntry/NumberUpDownShowCase.axaml`

---

## 2. 通过 Style 覆盖样式

可以在 AXAML 中使用 `Style` 对 NumericUpDown 进行全局或局部样式覆盖：

### 全局统一宽度

```xml
<Window.Styles>
    <Style Selector="atom|NumericUpDown">
        <Setter Property="HorizontalAlignment" Value="Stretch" />
    </Style>
</Window.Styles>
```

### 按尺寸定制样式

```xml
<!-- 大号数字输入框使用粗体 -->
<Style Selector="atom|NumericUpDown[SizeType=Large]">
    <Setter Property="FontWeight" Value="SemiBold" />
</Style>
```

### 按变体定制颜色

```xml
<!-- 自定义 Outline 变体的聚焦边框颜色 -->
<Style Selector="atom|NumericUpDown[StyleVariant=Outline]:focus-within">
    <Setter Property="BorderBrush" Value="#722ed1" />
</Style>
```

### 按验证状态定制

```xml
<!-- Error 状态下使用粗体 -->
<Style Selector="atom|NumericUpDown[Status=Error]">
    <Setter Property="FontWeight" Value="Bold" />
</Style>
```

---

## 3. 通过 ControlTheme 完全替换主题

如果需要彻底替换 NumericUpDown 的模板和样式，可以定义自定义 `ControlTheme`：

```xml
<ControlTheme x:Key="MyCustomNumericUpDown" TargetType="atom:NumericUpDown">
    <Setter Property="Template">
        <ControlTemplate>
            <!-- 自定义模板 -->
        </ControlTemplate>
    </Setter>
</ControlTheme>

<!-- 使用 -->
<atom:NumericUpDown Theme="{StaticResource MyCustomNumericUpDown}" Value="3" />
```

> ⚠️ 注意：完全替换 ControlTheme 将丢失内置的步进按钮悬浮效果、AddOn 装饰框、清除按钮等功能。NumericUpDown 的模板高度依赖 `ButtonSpinner`，建议优先使用 Style 覆盖。

---

## 4. 控制步进行为

```xml
<!-- 禁用键盘步进（只允许手动输入和按钮点击） -->
<atom:NumericUpDown Keyboard="False" Value="3" />

<!-- 禁用鼠标滚轮步进 -->
<atom:NumericUpDown MouseWheel="False" Value="3" />

<!-- 限制输入范围并设置步长 -->
<atom:NumericUpDown Minimum="0" Maximum="100" Increment="0.5" Value="50" />
```

---

## 5. 高精度模式

```xml
<!-- 启用 StringMode 进行高精度数值操作 -->
<atom:NumericUpDown StringMode="True"
                    StringValue="0.123456789012345678901234"
                    Increment="0.0001"
                    Minimum="0"
                    Maximum="100" />
```

> 在 StringMode 下，数值以字符串形式存储和传输，避免 `decimal` 精度丢失。

---

## 6. 禁用与只读

```xml
<!-- 禁用状态 -->
<atom:NumericUpDown Value="3" IsEnabled="False" />
<atom:NumericUpDown Value="3" StyleVariant="Filled" IsEnabled="False" />
<atom:NumericUpDown Value="3" StyleVariant="Borderless" IsEnabled="False" />

<!-- 只读模式（可步进但不可直接编辑文本） -->
<atom:NumericUpDown Value="3" IsReadOnly="True" />
```

---

## 7. 控制动画行为

```xml
<!-- 禁用过渡动画 -->
<atom:NumericUpDown IsMotionEnabled="False" Value="3" />
```

---

## 样式选择器速查

> 说明：Avalonia 样式选择器中使用 `atom|NumericUpDown` 语法引用 `atom` XML 命名空间下的 `NumericUpDown` 类型，其中 `|` 是命名空间分隔符。

### 按变体选择

| 选择器 | 说明 |
|---|---|
| `atom\|NumericUpDown` | 匹配所有 AtomUI NumericUpDown 实例 |
| `atom\|NumericUpDown[StyleVariant=Outline]` | 匹配轮廓样式（默认，标准边框） |
| `atom\|NumericUpDown[StyleVariant=Filled]` | 匹配填充样式（灰色背景） |
| `atom\|NumericUpDown[StyleVariant=Borderless]` | 匹配无边框样式 |

### 按尺寸选择

| 选择器 | 说明 |
|---|---|
| `atom\|NumericUpDown[SizeType=Large]` | 匹配大号（高度 = `ControlHeightLG`） |
| `atom\|NumericUpDown[SizeType=Middle]` | 匹配中号（默认尺寸） |
| `atom\|NumericUpDown[SizeType=Small]` | 匹配小号（高度 = `ControlHeightSM`） |

### 按验证状态选择

| 选择器 | 说明 |
|---|---|
| `atom\|NumericUpDown[Status=Error]` | 匹配错误状态（红色边框） |
| `atom\|NumericUpDown[Status=Warning]` | 匹配警告状态（黄色边框） |

### 按状态伪类选择

| 选择器 | 说明 |
|---|---|
| `atom\|NumericUpDown:disabled` | 匹配禁用状态（`IsEnabled == false`），灰色调 |
| `atom\|NumericUpDown:pointerover` | 匹配鼠标悬浮状态 |
| `atom\|NumericUpDown:focus-within` | 匹配内部文本框聚焦状态 |

### 组合选择器示例

| 选择器 | 说明 |
|---|---|
| `atom\|NumericUpDown[StyleVariant=Outline]:not(:disabled)` | 非禁用态的轮廓样式 |
| `atom\|NumericUpDown[Status=Error][StyleVariant=Filled]` | Filled 变体下的错误状态 |
| `atom\|NumericUpDown[SizeType=Large]:focus-within` | 大号聚焦态 |
| `atom\|NumericUpDown /template/ atom\|ButtonSpinner` | 访问模板内的 ButtonSpinner 部件 |
| `atom\|NumericUpDown /template/ atom\|TextBox#PART_TextBox` | 访问模板内的文本输入框 |
